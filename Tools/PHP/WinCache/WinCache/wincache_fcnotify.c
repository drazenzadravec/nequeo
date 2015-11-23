/*
   +----------------------------------------------------------------------------------------------+
   | Windows Cache for PHP                                                                        |
   +----------------------------------------------------------------------------------------------+
   | Copyright (c) 2009, Microsoft Corporation. All rights reserved.                              |
   |                                                                                              |
   | Redistribution and use in source and binary forms, with or without modification, are         |
   | permitted provided that the following conditions are met:                                    |
   | - Redistributions of source code must retain the above copyright notice, this list of        |
   | conditions and the following disclaimer.                                                     |
   | - Redistributions in binary form must reproduce the above copyright notice, this list of     |
   | conditions and the following disclaimer in the documentation and/or other materials provided |
   | with the distribution.                                                                       |
   | - Neither the name of the Microsoft Corporation nor the names of its contributors may be     |
   | used to endorse or promote products derived from this software without specific prior written|
   | permission.                                                                                  |
   |                                                                                              |
   | THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS  |
   | OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF              |
   | MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE   |
   | COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,    |
   | EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE|
   | GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED   |
   | AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING    |
   | NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED |
   | OF THE POSSIBILITY OF SUCH DAMAGE.                                                           |
   +----------------------------------------------------------------------------------------------+
   | Module: wincache_fcnotify.c                                                                  |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   +----------------------------------------------------------------------------------------------+
*/
#define   _WIN32_WINNT   0x0500
#include "precomp.h"

#define PROCESS_IS_ALIVE              0
#define PROCESS_IS_DEAD               1
#define FCNOTIFY_VALUE(p, o)          ((fcnotify_value *)alloc_get_cachevalue(p, o))
#define FCNOTIFY_TERMKEY              ((ULONG_PTR)-1)
#ifdef WINCACHE_DEBUG
#define SCAVENGER_FREQUENCY           (10 * 1000)
#else /* WINCACHE_DEBUG */
#define SCAVENGER_FREQUENCY           (900 * 1000)
#endif /* WINCACHE_DEBUG */
#define PALIVECHK_FREQUENCY           1000

static unsigned int WINAPI change_notification_thread(void * parg);
static void WINAPI process_change_notification(fcnotify_context * pnotify, fcnotify_listen * plistener, unsigned int dwerror, unsigned int bytecount);
static int  findfolder_in_cache(fcnotify_context * pnotify, const char * folderpath, unsigned int index, fcnotify_value ** ppvalue);
static void unregister_directory_changes(fcnotify_listen * plistener);
static unsigned int register_directory_changes(fcnotify_context * pnotify, fcnotify_listen * plistener);

static int  create_fcnotify_data(fcnotify_context * pnotify, const char * folderpath, fcnotify_value ** ppvalue);
static void listener_refinc(fcnotify_listen * plistener);
static void listener_refdec(fcnotify_context * pnotify, fcnotify_listen * plistener);
static void destroy_fcnotify_data(fcnotify_context * pnotify, fcnotify_value * pvalue);
static void add_fcnotify_entry(fcnotify_context * pnotify, unsigned int index, fcnotify_value * pvalue);
static void remove_fcnotify_entry(fcnotify_context * pnotify, unsigned int index, fcnotify_value * pvalue);
static int  pidhandles_apply(zval * pdestination);
static void run_fcnotify_scavenger(fcnotify_context * pnotify);
static unsigned char process_alive_check(fcnotify_context * pnotify, fcnotify_value * pvalue);

static unsigned int WINAPI change_notification_thread(void * parg)
{
    fcnotify_context * pnotify   = NULL;
    fcnotify_listen *  plistener = NULL;
    zend_bool          retvalue  = 0;
    unsigned int       dwerror   = 0;
    unsigned int       bytecount = 0;
    ULONG_PTR          compkey   = 0;
    OVERLAPPED *       poverlap  = NULL;

    pnotify = (fcnotify_context *)parg;

    while(1)
    {
        compkey   = 0;
        bytecount = 0;
        poverlap  = NULL;

        /* Get completion port message. Ok to listen infinitely */
        /* as terminate is going to send termination key */
        retvalue = GetQueuedCompletionStatus(pnotify->port_handle, &bytecount, &compkey, &poverlap, INFINITE);

        /* process change notification if success and key is non termination key */
        dwerror = (retvalue ? ERROR_SUCCESS : GetLastError());

        if(compkey == FCNOTIFY_TERMKEY)
        {
            break;
        }
        else if(poverlap != NULL)
        {
            plistener = CONTAINING_RECORD(poverlap, fcnotify_listen, overlapped);
            _ASSERT(plistener != NULL);

            listener_refinc(plistener);
            process_change_notification(pnotify, plistener, dwerror, bytecount);
            listener_refdec(pnotify, plistener);
        }
    }

    return ERROR_SUCCESS;
}

static void WINAPI process_change_notification(fcnotify_context * pnotify, fcnotify_listen * plistener, unsigned int dwerror, unsigned int bytecount)
{
    HRESULT                   hr         = S_OK;
    int                       result     = NONFATAL;
    FILE_NOTIFY_INFORMATION * pnitem     = NULL;
    FILE_NOTIFY_INFORMATION * pnnext     = NULL;
    wchar_t *                 pwchar     = NULL;
    char *                    pfname     = NULL;
    unsigned int              fnlength   = 0;

    dprintverbose("start process_change_notification");

    _ASSERT(plistener   != NULL);

    /* Async IO completed. Update refcount to reflect that */
    listener_refdec(pnotify, plistener);

    if(plistener->stopcalled)
    {
        goto Finished;
    }

    if(bytecount == 0)
    {
        if(dwerror == ERROR_ACCESS_DENIED)
        {
            /* Unregister change notification and set plistener to NULL to */
            /* make system set a new change notification system if required */
            lock_lock(pnotify->fclock);

            unregister_directory_changes(plistener);
            plistener->pfcnvalue->plistener = NULL;

            lock_unlock(pnotify->fclock);
        }

        // TODO: Handle ERROR_NOTIFY_ENUM_DIR - change buffer overflowed & we
        // TODO: missed some changes.
        // TODO: Not sure what to do here, since we don't have a mechanism for
        // TODO: invalidating a directory and all files under the directory.
        // TODO: The aplist cache is only designed to lookup full paths. Any
        // TODO: other enumeration would be an O(1) traversal of the cache.

        goto Finished;
    }
    else
    {
        pnnext = (FILE_NOTIFY_INFORMATION *)plistener->fninfo;
        while(pnnext != NULL)
        {
            pnitem = pnnext;
            pnnext = (FILE_NOTIFY_INFORMATION *)((char *)pnitem + pnitem->NextEntryOffset);

            fnlength = pnitem->FileNameLength / 2;

            pwchar = (wchar_t *)alloc_pemalloc((fnlength + 1) * sizeof(wchar_t));
            pfname = (char *)alloc_pemalloc(fnlength + 1);
            if(pwchar == NULL || pfname == NULL)
            {
                hr = E_OUTOFMEMORY;
                goto Finished;
            }

            ZeroMemory(pwchar, (fnlength + 1) * sizeof(wchar_t));
            wcsncpy(pwchar, pnitem->FileName, fnlength);

            ZeroMemory(pfname, fnlength + 1);
            if(!WideCharToMultiByte(CP_ACP, WC_NO_BEST_FIT_CHARS, pwchar, fnlength, pfname, fnlength, NULL, NULL))
            {
                hr = GetLastError();
                goto Finished;
            }

            dprintverbose("received change notification for file = %s\\%s", plistener->folder_path, pfname);
            aplist_mark_changed(pnotify->fcaplist, plistener->folder_path, pfname);

            alloc_pefree(pwchar);
            pwchar = NULL;

            alloc_pefree(pfname);
            pfname = NULL;

            if(pnitem == pnnext)
            {
                break;
            }
        }

        /* register for change notification again */
        /* If bytecount was 0, no need to register again */
        register_directory_changes(pnotify, plistener);
    }

Finished:

    if(pwchar != NULL)
    {
        alloc_pefree(pwchar);
        pwchar = NULL;
    }

    if(pfname != NULL)
    {
        alloc_pefree(pfname);
        pfname = NULL;
    }

    dprintverbose("end process_change_notification");
    return;
}

static int findfolder_in_cache(fcnotify_context * pnotify, const char * folderpath, unsigned int index, fcnotify_value ** ppvalue)
{
    int               result  = NONFATAL;
    fcnotify_header * pheader = NULL;
    fcnotify_value *  pvalue  = NULL;

    dprintverbose("start findfolder_in_cache");

    _ASSERT(pnotify    != NULL);
    _ASSERT(folderpath != NULL);
    _ASSERT(ppvalue    != NULL);

    *ppvalue = NULL;

    pheader = pnotify->fcheader;
    pvalue = FCNOTIFY_VALUE(pnotify->fcalloc, pheader->values[index]);

    while(pvalue != NULL)
    {
        if(!_stricmp(pnotify->fcmemaddr + pvalue->folder_path, folderpath))
        {
            *ppvalue = pvalue;
            break;
        }

        pvalue = FCNOTIFY_VALUE(pnotify->fcalloc, pvalue->next_value);
    }

    dprintverbose("end findfolder_in_cache");

    return result;
}

static void unregister_directory_changes(fcnotify_listen * plistener)
{
    InterlockedExchange(&plistener->stopcalled, 1);

    if(plistener->folder_handle != INVALID_HANDLE_VALUE)
    {
        CloseHandle(plistener->folder_handle);
        plistener->folder_handle = INVALID_HANDLE_VALUE;
    }
}

static unsigned int register_directory_changes(fcnotify_context * pnotify, fcnotify_listen * plistener)
{
    unsigned int cflags    = 0;
    unsigned int bytecount = 0;
    unsigned int result    = 0;

    _ASSERT(plistener                != NULL);
    _ASSERT(plistener->pfcnvalue     != NULL);
    _ASSERT(plistener->folder_handle != INVALID_HANDLE_VALUE);

    memset(&plistener->overlapped, 0, sizeof(OVERLAPPED));
    listener_refinc(plistener);

    /* file name, last write, attributes and security change should be delivered */
    cflags = FILE_NOTIFY_CHANGE_FILE_NAME  |
             FILE_NOTIFY_CHANGE_DIR_NAME   |
             FILE_NOTIFY_CHANGE_LAST_WRITE |
             FILE_NOTIFY_CHANGE_ATTRIBUTES |
             FILE_NOTIFY_CHANGE_SECURITY;
    result = ReadDirectoryChangesW(plistener->folder_handle, &plistener->fninfo, 1024, FALSE, cflags, &bytecount, &plistener->overlapped, NULL);

    if(!result)
    {
        listener_refdec(pnotify, plistener);
    }

    return result;
}

/* This method will either create a new entry altogether or modify an existing entry */
static int create_fcnotify_data(fcnotify_context * pnotify, const char * folderpath, fcnotify_value ** ppvalue)
{
    int               result    = NONFATAL;
    fcnotify_value *  pvalue    = NULL;
    alloc_context *   palloc    = NULL;
    size_t            pathlen   = 0;
    char *            paddr     = NULL;
    unsigned int      fshare    = 0;
    unsigned int      flags     = 0;
    unsigned int      rdcresult = 0;
    fcnotify_listen * plistener = NULL;

    dprintverbose("start create_fcnotify_data");

    _ASSERT(pnotify    != NULL);
    _ASSERT(folderpath != NULL);
    _ASSERT(ppvalue    != NULL);

    palloc = pnotify->fcalloc;
    pathlen = strlen(folderpath);

    /* Allocate memory only if its not already allocated */
    if(*ppvalue == NULL)
    {
        pvalue = (fcnotify_value *)alloc_smalloc(palloc, sizeof(fcnotify_value) + ALIGNDWORD(pathlen + 1));
        if(pvalue == NULL)
        {
            result = FATAL_OUT_OF_SMEMORY;
            goto Finished;
        }

        /* Set these to 0 only if a new structure is created */
        pvalue->reusecount  = 0;
        pvalue->folder_path = 0;
        pvalue->refcount    = 0;
        pvalue->prev_value  = 0;
        pvalue->next_value  = 0;

        /* Set folderpath only if memory is just allocated */
        paddr = (char *)(pvalue + 1);
        memcpy_s(paddr, pathlen, folderpath, pathlen);
        *(paddr + pathlen) = 0;
        pvalue->folder_path = ((char *)paddr) - pnotify->fcmemaddr;
    }
    else
    {
        pvalue = *ppvalue;
    }

    pvalue->owner_pid = pnotify->processid;
    pvalue->palivechk = GetTickCount();
    pvalue->plistener = NULL;

    pvalue->listen_time.dwHighDateTime = 0;
    pvalue->listen_time.dwLowDateTime  = 0;

    /* Allocate memory for listener locally */
    plistener = (fcnotify_listen *)alloc_pemalloc(sizeof(fcnotify_listen));
    if(plistener == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    ZeroMemory(plistener, sizeof(fcnotify_listen));

    /* Set initial refcount and pointer to fcnotify_value */
    plistener->lrefcount  = 0;
    plistener->pfcnvalue  = pvalue;

    /* Open folder_path and attach it to completion port */
    fshare = FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE;
    flags = FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OVERLAPPED;

    plistener->folder_handle = CreateFile(folderpath, FILE_LIST_DIRECTORY, fshare, NULL, OPEN_EXISTING, flags, NULL);
    if(plistener->folder_handle == INVALID_HANDLE_VALUE)
    {
        result = FATAL_FCNOTIFY_CREATEFILE;
        goto Finished;
    }

    plistener->folder_path = alloc_pestrdup(folderpath);

    if(CreateIoCompletionPort(plistener->folder_handle, pnotify->port_handle, (ULONG_PTR)0, 0) == NULL)
    {
        result = FATAL_FCNOTIFY_COMPPORT;
        goto Finished;
    }

    rdcresult = register_directory_changes(pnotify, plistener);
    if(!rdcresult)
    {
        result = FATAL_FCNOTIFY_RDCFAILURE;
        goto Finished;
    }

    pvalue->plistener = plistener;
    plistener = NULL;

    /* Update the time when the listener was set */
    GetSystemTimeAsFileTime(&pvalue->listen_time);

    pvalue->reusecount++;
    *ppvalue = pvalue;

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in create_fcnotify_data", result);

        if(pvalue != NULL)
        {
            if(pvalue->plistener != NULL)
            {
                if(pvalue->plistener->folder_path != NULL)
                {
                    alloc_pefree(pvalue->plistener->folder_path);
                    pvalue->plistener->folder_path = NULL;
                }

                if(pvalue->plistener->folder_handle != INVALID_HANDLE_VALUE)
                {
                    CloseHandle(pvalue->plistener->folder_handle);
                    pvalue->plistener->folder_handle = INVALID_HANDLE_VALUE;
                }

                alloc_pefree(pvalue->plistener);
                pvalue->plistener = NULL;
            }

            alloc_sfree(palloc, pvalue);
            pvalue = NULL;
        }
    }

    dprintverbose("end create_fcnotify_data");

    return result;
}

__inline
static void listener_refinc(fcnotify_listen * plistener)
{
    InterlockedIncrement(&plistener->lrefcount);
}

__inline
static void listener_refdec(fcnotify_context * pnotify, fcnotify_listen * plistener)
{
    if(InterlockedDecrement(&plistener->lrefcount) == 0)
    {
        lock_lock(pnotify->fclock);

        unregister_directory_changes(plistener);

        if(plistener->folder_path != NULL)
        {
            alloc_pefree(plistener->folder_path);
            plistener->folder_path = NULL;
        }

        /*
         * Set listener to null to rehook change notification if this folder is
         * re-added before scavenger run
         */
        plistener->pfcnvalue->plistener = NULL;
        alloc_pefree(plistener);
        plistener = NULL;

        lock_unlock(pnotify->fclock);
    }
}

static void destroy_fcnotify_data(fcnotify_context * pnotify, fcnotify_value * pvalue)
{
    dprintverbose("start destroy_fcnotify_data");

    if(pvalue != NULL)
    {
        /* Free memory occupied by plistener and close the handle */
        /* if owner process is destroying fcnotify data */
        /* Detect recycled ownerpid by comparing process times with listen_time */
        FILETIME ftpstart, ftpexit, ftpkernel, ftpuser;
        if((pnotify->processid == pvalue->owner_pid) &&
           (pvalue->plistener != NULL) &&
           (GetProcessTimes(GetCurrentProcess(), &ftpstart, &ftpexit, &ftpkernel, &ftpuser) != 0) &&
           (ftpstart.dwHighDateTime <= pvalue->listen_time.dwHighDateTime && ftpstart.dwLowDateTime <= pvalue->listen_time.dwLowDateTime))
        {
            /* Just close the directory handle */
            /* Change notification will make refcount 0 and free memory */
            if(pvalue->plistener != NULL && pvalue->plistener->folder_handle != INVALID_HANDLE_VALUE)
            {
                CloseHandle(pvalue->plistener->folder_handle);
                pvalue->plistener->folder_handle = INVALID_HANDLE_VALUE;
            }

            pvalue->plistener = NULL;
        }
        else
        {
            /* Process which owned memory is probably gone and ownerpid is recycled */
            pvalue->plistener = NULL;
        }

        alloc_sfree(pnotify->fcalloc, pvalue);
        pvalue = NULL;
    }

    dprintverbose("end destroy_fcnotify_data");
}

static void add_fcnotify_entry(fcnotify_context * pnotify, unsigned int index, fcnotify_value * pvalue)
{
    fcnotify_header * fcheader = NULL;
    fcnotify_value *  pcheck   = NULL;

    dprintverbose("start add_fcnotify_entry");

    _ASSERT(pnotify             != NULL);
    _ASSERT(pvalue              != NULL);
    _ASSERT(pvalue->folder_path != 0);

    fcheader = pnotify->fcheader;
    pcheck = FCNOTIFY_VALUE(pnotify->fcalloc, fcheader->values[index]);

    while(pcheck != NULL)
    {
        if(pcheck->next_value == 0)
        {
            break;
        }

        pcheck = FCNOTIFY_VALUE(pnotify->fcalloc, pcheck->next_value);
    }

    if(pcheck != NULL)
    {
        pcheck->next_value = alloc_get_valueoffset(pnotify->fcalloc, pvalue);
        pvalue->next_value = 0;
        pvalue->prev_value = alloc_get_valueoffset(pnotify->fcalloc, pcheck);
    }
    else
    {
        fcheader->values[index] = alloc_get_valueoffset(pnotify->fcalloc, pvalue);
        pvalue->next_value = 0;
        pvalue->prev_value = 0;
    }

    fcheader->itemcount++;

    dprintverbose("end add_fcnotify_entry");
    return;
}

static void remove_fcnotify_entry(fcnotify_context * pnotify, unsigned int index, fcnotify_value * pvalue)
{
    alloc_context *   palloc  = NULL;
    fcnotify_header * header  = NULL;
    fcnotify_value *  ptemp   = NULL;

    dprintverbose("start remove_fcnotify_entry");

    _ASSERT(pnotify             != NULL);
    _ASSERT(pvalue              != NULL);
    _ASSERT(pvalue->folder_path != 0);

    header = pnotify->fcheader;
    palloc = pnotify->fcalloc;

    header->itemcount--;
    if(pvalue->prev_value == 0)
    {
        header->values[index] = pvalue->next_value;
        if(pvalue->next_value != 0)
        {
            ptemp = FCNOTIFY_VALUE(palloc, pvalue->next_value);
            if (ptemp != NULL)
            {
                ptemp->prev_value = 0;
            }
        }
    }
    else
    {
        ptemp = FCNOTIFY_VALUE(palloc, pvalue->prev_value);
        if (ptemp != NULL)
        {
            ptemp->next_value = pvalue->next_value;
        }

        if(pvalue->next_value != 0)
        {
            ptemp = FCNOTIFY_VALUE(palloc, pvalue->next_value);
            if (ptemp != NULL)
            {
                ptemp->prev_value = pvalue->prev_value;
            }
        }
    }

    destroy_fcnotify_data(pnotify, pvalue);
    pvalue = NULL;

    dprintverbose("end remove_fcnotify_entry");
    return;
}

/* Public functions */
int fcnotify_create(fcnotify_context ** ppnotify)
{
    int                result   = NONFATAL;
    fcnotify_context * pcontext = NULL;

    dprintverbose("start fcnotify_create");

    _ASSERT(ppnotify != NULL);
    *ppnotify = NULL;

    pcontext = (fcnotify_context *)alloc_pemalloc(sizeof(fcnotify_context));
    if(pcontext == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    pcontext->islocal       = 0;
    pcontext->isshutting    = 0;
    pcontext->iswow64       = 0;
    pcontext->processid     = 0;
    pcontext->lscavenge     = 0;
    pcontext->ttlticks      = 0;

    pcontext->fcmemaddr     = NULL;
    pcontext->fcheader      = NULL;
    pcontext->fcalloc       = NULL;
    pcontext->fclock        = NULL;
    pcontext->fcaplist      = NULL;

    pcontext->listen_thread = NULL;
    pcontext->port_handle   = NULL;
    pcontext->pidhandles    = NULL;

    *ppnotify = pcontext;

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in fcnotify_create", result);
    }

    dprintverbose("end fcnotify_create");

    return result;
}

int fcnotify_listenerexists(fcnotify_context *pnotify, const char * folderpath, unsigned char * listenerexists)
{
    int                 result    = NONFATAL;
    int                 index     = 0;
    fcnotify_value *    pvalue    = NULL;

    dprintverbose("start fcnotify_listenerexists");

    *listenerexists = 0;

    lock_lock(pnotify->fclock);

    /* Look if folder path is present in cache */
    index = utils_getindex(folderpath, pnotify->fcheader->valuecount);
    result = findfolder_in_cache(pnotify, folderpath, index, &pvalue);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* Check if listener for this folder still exists */
    if (pvalue != NULL && pvalue->plistener != NULL)
    {
        *listenerexists = 1;
    }

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in fcnotify_listenerexists", result);
    }

    lock_unlock(pnotify->fclock);

    dprintverbose("end fcnotify_listenerexists");

    return result;
}

void fcnotify_destroy(fcnotify_context * pnotify)
{
    dprintverbose("start fcnotify_destroy");

    if(pnotify != NULL)
    {
        alloc_pefree(pnotify);
        pnotify = NULL;
    }

    dprintverbose("end fcnotify_destroy");

    return;
}

int fcnotify_initialize(fcnotify_context * pnotify, unsigned short islocal, void * paplist, alloc_context * palloc, unsigned int filecount)
{
    int               result   = NONFATAL;
    unsigned short    locktype = LOCK_TYPE_SHARED;
    fcnotify_header * header   = NULL;
    unsigned int      msize    = 0;

    _ASSERT(pnotify != NULL);
    _ASSERT(paplist != NULL);
    _ASSERT(palloc  != NULL);
    _ASSERT(palloc->memaddr != NULL);

    if(islocal)
    {
        locktype = LOCK_TYPE_LOCAL;
        pnotify->islocal = islocal;
    }

    pnotify->islocal   = islocal;
    pnotify->fcaplist  = paplist;
    pnotify->fcalloc   = palloc;

    pnotify->processid = WCG(fmapgdata)->pid;
    pnotify->fcmemaddr = palloc->memaddr;
    pnotify->lscavenge = GetTickCount();

    /* Get memory for fcnotify header */
    msize = sizeof(fcnotify_header) + (filecount - 1) * sizeof(size_t);
    pnotify->fcheader = (fcnotify_header *)alloc_get_cacheheader(pnotify->fcalloc, msize, CACHE_TYPE_FCNOTIFY);
    if(pnotify->fcheader == NULL)
    {
        result = FATAL_FCNOTIFY_INITIALIZE;
        goto Finished;
    }

    header = pnotify->fcheader;

    /* Create reader writer lock for the file change notification hashtable */
    result = lock_create(&pnotify->fclock);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = lock_initialize(pnotify->fclock, "FILE_CHANGE_NOTIFY", 1, locktype, &header->last_owner);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* Create IO completion port */
    pnotify->port_handle = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, (ULONG_PTR)0, 0);
    if(pnotify->port_handle == NULL)
    {
        result = FATAL_FCNOTIFY_INITIALIZE;
        goto Finished;
    }

    /* Create listener thread */
    pnotify->listen_thread = CreateThread(NULL, 0, change_notification_thread, (void *)pnotify, 0, NULL);
    if(pnotify->listen_thread == NULL)
    {
        result = FATAL_FCNOTIFY_INITIALIZE;
        goto Finished;
    }

    /* Create pidhandles hashtable */
    pnotify->pidhandles = (HashTable *)alloc_pemalloc(sizeof(HashTable));
    if(pnotify->pidhandles == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    zend_hash_init(pnotify->pidhandles, 0, NULL, NULL, 1);

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in fcnotify_initialize", result);

        if(pnotify->listen_thread != NULL)
        {
            CloseHandle(pnotify->listen_thread);
            pnotify->listen_thread = NULL;
        }

        if(pnotify->port_handle != NULL)
        {
            CloseHandle(pnotify->port_handle);
            pnotify->port_handle = NULL;
        }

        if(pnotify->fclock != NULL)
        {
            lock_terminate(pnotify->fclock);
            lock_destroy(pnotify->fclock);

            pnotify->fclock = NULL;
        }

        if(pnotify->pidhandles != NULL)
        {
            zend_hash_destroy(pnotify->pidhandles);
            alloc_pefree(pnotify->pidhandles);

            pnotify->pidhandles = NULL;
        }

        pnotify->fcaplist = NULL;
        pnotify->fcheader = NULL;
        pnotify->fcalloc  = NULL;
    }

    return result;
}

void fcnotify_initheader(fcnotify_context * pnotify, unsigned int filecount)
{
     fcnotify_header * pheader = NULL;

     dprintverbose("start fcnotify_initheader");

     _ASSERT(pnotify           != NULL);
     _ASSERT(pnotify->fcheader != NULL);
     _ASSERT(filecount         >  0);

     pheader = pnotify->fcheader;
     _ASSERT(pheader->values != NULL);

     /* This method is called by aplist_initialize which is */
     /* taking care of blocking other processes */
     /* Also rdcount can be safely set to 0 as lock is not active */
     pheader->last_owner    = 0;
     pheader->itemcount     = 0;
     pheader->valuecount    = filecount;
     memset((void *)pheader->values, 0, sizeof(size_t) * filecount);

     dprintverbose("end fcnotify_initheader");

     return;
}

void fcnotify_terminate(fcnotify_context * pnotify)
{
    dprintverbose("start fcnotify_terminate");

    if(pnotify != NULL)
    {
        if(pnotify->listen_thread != NULL)
        {
            if(pnotify->port_handle != NULL)
            {
                DWORD ret = 0;
                /* Post termination key to completion port to terminate thread */
                PostQueuedCompletionStatus(pnotify->port_handle, 0, FCNOTIFY_TERMKEY, NULL);

                /* Wait for thread to finish */
                WaitForSingleObject(pnotify->listen_thread, FIVE_SECOND_WAIT);

                if (ret == WAIT_TIMEOUT || ret == WAIT_FAILED)
                {
                    dprintimportant("Timed out waiting for fcnotify thread to terminate");
                }
            }

            CloseHandle(pnotify->listen_thread);
            pnotify->listen_thread = NULL;
        }

        if(pnotify->port_handle != NULL)
        {
            CloseHandle(pnotify->port_handle);
            pnotify->port_handle = NULL;
        }

        if(pnotify->fclock != NULL)
        {
            lock_terminate(pnotify->fclock);
            lock_destroy(pnotify->fclock);

            pnotify->fclock = NULL;
        }

        if(pnotify->pidhandles != NULL)
        {
            zend_hash_destroy(pnotify->pidhandles);
            alloc_pefree(pnotify->pidhandles);

            pnotify->pidhandles = NULL;
        }

        pnotify->fcaplist = NULL;
        pnotify->fcheader = NULL;
        pnotify->fcalloc  = NULL;
    }

    dprintverbose("end fcnotify_terminate");

    return;
}

static int pidhandles_apply(zval * pdestination)
{
    HANDLE        hprocess = NULL;
    unsigned int  exitcode = 0;

    _ASSERT(pdestination != NULL);
    hprocess = (HANDLE)Z_PTR_P(pdestination);

    if(GetExitCodeProcess(hprocess, &exitcode) && exitcode != STILL_ACTIVE)
    {
        CloseHandle(hprocess);
        return ZEND_HASH_APPLY_REMOVE;
    }
    else
    {
        return ZEND_HASH_APPLY_KEEP;
    }
}

/* This method acquires write lock. Call without any lock */
static void run_fcnotify_scavenger(fcnotify_context * pnotify)
{
    fcnotify_header * pheader    = NULL;
    fcnotify_value *  pvalue     = NULL;
    fcnotify_value *  pnext      = NULL;
    unsigned int      index      = 0;
    unsigned int      count      = 0;
    HashTable *       phashtable = NULL;

    dprintverbose("start run_fcnotify_scavenger");

    pheader = pnotify->fcheader;
    count   = pheader->valuecount;

    phashtable = pnotify->pidhandles;

    /* Go through all the entries and remove entries for which refcount is 0 */
    /* Do it only for processes which are dead or if this was the owner pid */
    lock_lock(pnotify->fclock);

    for(index = 0; index < count; index++)
    {
        pnext = FCNOTIFY_VALUE(pnotify->fcalloc, pheader->values[index]);
        while(pnext != NULL)
        {
            pvalue = pnext;
            pnext = FCNOTIFY_VALUE(pnotify->fcalloc, pvalue->next_value);

            /* process alive check will remove entry from pidhandles if necessary */
            if(pvalue->refcount == 0 &&
               (pvalue->owner_pid == pnotify->processid || process_alive_check(pnotify, pvalue) == PROCESS_IS_DEAD))
            {
                remove_fcnotify_entry(pnotify, index, pvalue);
                pvalue = NULL;
            }
        }
    }

    lock_unlock(pnotify->fclock);

    /* Go through pidhandles table and remove entries for dead processes */
    zend_hash_apply(phashtable, pidhandles_apply);
    pnotify->lscavenge = GetTickCount();

    dprintverbose("end run_fcnotify_scavenger");

    return;
}

static unsigned char process_alive_check(fcnotify_context * pnotify, fcnotify_value * pvalue)
{
    HashTable *   phashtable = NULL;
    HANDLE        hprocess   = NULL;
    unsigned char listenp    = PROCESS_IS_ALIVE;
    unsigned int  exitcode   = 0;
    HANDLE        htoken     = NULL;
    unsigned int  bthread    = 0;
    unsigned int  ownerpid   = 0;

    _ASSERT(pnotify != NULL);
    _ASSERT(pvalue  != NULL);

    ownerpid = pvalue->owner_pid;

    /* If the check is for this process, return alive */
    if(pnotify->processid == ownerpid)
    {
        goto Finished;
    }

    phashtable = pnotify->pidhandles;

    hprocess = (HANDLE)zend_hash_index_find_ptr(phashtable, (zend_ulong)ownerpid);
    if(hprocess == NULL)
    {
        /* Check if impersonation is enabled */
        /* If it is, get impersonated token and set it back after calling OpenProcess */
        bthread = OpenThreadToken(GetCurrentThread(), TOKEN_IMPERSONATE, TRUE, &htoken);
        if(bthread)
        {
            RevertToSelf();
        }

        hprocess = OpenProcess(PROCESS_QUERY_INFORMATION, FALSE, ownerpid);

        if(bthread)
        {
            SetThreadToken(NULL, htoken);
            CloseHandle(htoken);
        }

        if(hprocess != NULL)
        {
            /* Check if process start time is greater than listener set time */
            FILETIME ftpstart, ftpexit, ftpkernel, ftpuser;
            if((GetProcessTimes(hprocess, &ftpstart, &ftpexit, &ftpkernel, &ftpuser) != 0) &&
               (ftpstart.dwHighDateTime > pvalue->listen_time.dwHighDateTime || ftpstart.dwLowDateTime > pvalue->listen_time.dwLowDateTime))
            {
                listenp = PROCESS_IS_DEAD;
            }

            /* Keep the handle around to save OpenProcess calls */
            zend_hash_index_update_ptr(phashtable, (zend_ulong)ownerpid, (void *)hprocess);
        }
        else
        {
            /* OpenProcess failure means process is gone */
            listenp = PROCESS_IS_DEAD;
        }
    }
    else
    {
        if(GetExitCodeProcess(hprocess, &exitcode) && exitcode != STILL_ACTIVE)
        {
            /* GetProcessId failure means process is gone */
            CloseHandle(hprocess);
            zend_hash_index_del(phashtable, (zend_ulong)ownerpid);

            listenp = PROCESS_IS_DEAD;
        }
    }

Finished:

    return listenp;
}

int fcnotify_check(fcnotify_context * pnotify, const char * filepath, size_t * poffset, unsigned int * pcount)
{
    int               result     = NONFATAL;
    size_t            flength    = 0;
    char *            folderpath = NULL;
    unsigned char     allocated  = 0;
    unsigned int      index      = 0;
    fcnotify_header * pheader    = NULL;
    fcnotify_value *  pvalue     = NULL;
    fcnotify_value *  ptemp      = NULL;
    fcnotify_value *  pnewval    = NULL;
    unsigned char     listenp    = 0;
    unsigned char     flock      = 0;
    unsigned int      cticks     = 0;
    unsigned int      reusecount = 0;

    dprintverbose("start fcnotify_check");

    _ASSERT(pnotify  != NULL);
    _ASSERT(poffset  != NULL);
    _ASSERT(pcount   != NULL);

    pheader = pnotify->fcheader;

    /* Acquire lock before reading any values */
    lock_lock(pnotify->fclock);
    flock = 1;

    if(filepath == NULL)
    {
        _ASSERT(*poffset != 0);
        _ASSERT(*pcount  != 0);

        /* Use offset to get to fcnotify_value */
        pvalue = FCNOTIFY_VALUE(pnotify->fcalloc, *poffset);

        /* If listener in fcnotify_value is rehooked, force a file change check */
        if(pvalue && pvalue->reusecount != *pcount)
        {
            /* Recheck after switching to write lock */
            if(pvalue->reusecount != *pcount)
            {
                *pcount = pvalue->reusecount;
            }

            result = WARNING_FCNOTIFY_FORCECHECK;
            goto Finished;
        }
    }
    else
    {
        _ASSERT(*poffset == 0);
        _ASSERT(*pcount  == 0);

        /* Get folderpath from filepath */
        flength = strlen(filepath);

        folderpath = alloc_emalloc(flength);
        if(folderpath == NULL)
        {
            result = FATAL_OUT_OF_LMEMORY;
            goto Finished;
        }

        allocated = 1;

        result = utils_filefolder(filepath, flength, folderpath, flength);
        if(FAILED(result))
        {
            goto Finished;
        }

        index = utils_getindex(folderpath, pheader->valuecount);

        /* Look if folder path is already present in cache */
        result = findfolder_in_cache(pnotify, folderpath, index, &pvalue);
        if(FAILED(result))
        {
            goto Finished;
        }
    }

    if(pvalue != NULL)
    {
        /* Check if this entry has a valid non-null listener */
        /* If not, make the function hook up change notification again */
        if(pvalue->plistener == NULL && folderpath != NULL)
        {
            listenp = 1;
            reusecount = pvalue->reusecount;
        }
        else
        {
            /* Check if listener is actually present and owner_pid */
            /* process is still there. Do the check once each second */
            cticks = GetTickCount();
            if(utils_ticksdiff(cticks, pvalue->palivechk) >= PALIVECHK_FREQUENCY)
            {
                listenp = process_alive_check(pnotify, pvalue);
                reusecount = pvalue->reusecount;
                InterlockedExchange(&pvalue->palivechk, cticks);

                if(listenp && folderpath == NULL)
                {
                    folderpath = (char *)(pnotify->fcmemaddr + pvalue->folder_path);
                    index = utils_getindex(folderpath, pheader->valuecount);
                }
            }
        }
    }
    else
    {
        /* If folder is not even there yet, add process listener */
        listenp = 1;
    }

    /* If folder is not there, register for change notification */
    /* and add the folder to the hashtable */
    if(listenp)
    {
        _ASSERT(folderpath != NULL);

        /* Check again to make sure no one else already registered */
        result = findfolder_in_cache(pnotify, folderpath, index, &ptemp);
        if(FAILED(result))
        {
            goto Finished;
        }

        if(ptemp != NULL)
        {
            if(pvalue != NULL && ptemp->reusecount == reusecount)
            {
                /* Change existing fcnotify entry to rehook change notification */
                create_fcnotify_data(pnotify, folderpath, &pvalue);
            }
            else
            {
                /* Some other process added the entry before this process could */
                pvalue = ptemp;
                ptemp = NULL;
            }
        }
        else
        {
            _ASSERT(pvalue == NULL);

            result = create_fcnotify_data(pnotify, folderpath, &ptemp);
            if(FAILED(result))
            {
                goto Finished;
            }

            pvalue = ptemp;
            ptemp  = NULL;
            add_fcnotify_entry(pnotify, index, pvalue);
        }

        /* Increment refcount if filepath was passed and not offset */
        if(filepath != NULL)
        {
            pvalue->refcount++;
        }

        *poffset = ((char *)pvalue - pnotify->fcmemaddr);
        *pcount  = pvalue->reusecount;
    }
    else
    {
        /* A valid listener is already there. Just return */
        /* offset if this is a first call to fcnotify_check */
        if(pvalue != NULL && filepath != NULL)
        {
            pvalue->refcount++;

            *poffset = ((char *)pvalue - pnotify->fcmemaddr);
            *pcount  = pvalue->reusecount;
        }
    }

    /* Run scavenger every SCAVENGER_FREQUENCY milliseconds */
    /* scavenger function will take a lock */
    if(utils_ticksdiff(0, pnotify->lscavenge) >= SCAVENGER_FREQUENCY)
    {
        if(flock == 1)
        {
            lock_unlock(pnotify->fclock);
            flock = 0;
        }
        run_fcnotify_scavenger(pnotify);
    }

Finished:

    if(flock == 1)
    {
        lock_unlock(pnotify->fclock);
        flock = 0;
    }

    if(allocated && folderpath != NULL)
    {
        alloc_efree(folderpath);
        folderpath = NULL;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in fcnotify_check", result);

        if(ptemp != NULL)
        {
            destroy_fcnotify_data(pnotify, ptemp);
            ptemp = NULL;
        }
    }

    dprintverbose("end fcnotify_check");

    return result;
}

void fcnotify_close(fcnotify_context * pnotify, size_t * poffset, unsigned int * pcount)
{
    fcnotify_value * pvalue = NULL;
    unsigned int     index  = 0;

    dprintverbose("start fcnotify_close");

    _ASSERT(pnotify != NULL);
    _ASSERT(poffset != NULL);
    _ASSERT(pcount  != NULL);

    lock_lock(pnotify->fclock);
    _ASSERT(*poffset > 0);

    /* Just decrement the refcount. Scavenger will take care of deleting the entry */
    pvalue = FCNOTIFY_VALUE(pnotify->fcalloc, *poffset);
    if (pvalue)
    {
        pvalue->refcount--;
    }

    *poffset = 0;
    *pcount  = 0;

    lock_unlock(pnotify->fclock);

    dprintverbose("end fcnotify_close");

    return;
}

int fcnotify_getinfo(fcnotify_context * pnotify, zend_bool summaryonly, fcnotify_info ** ppinfo)
{
    int                    result  = NONFATAL;
    fcnotify_info *        pcinfo  = NULL;
    fcnotify_entry_info *  peinfo  = NULL;
    fcnotify_entry_info *  ptemp   = NULL;
    fcnotify_value *       pvalue  = NULL;
    unsigned char          flock   = 0;
    size_t                 offset  = 0;
    unsigned int           count   = 0;
    unsigned int           index   = 0;

    dprintverbose("start fcnotify_getinfo");

    _ASSERT(pnotify != NULL);
    _ASSERT(ppinfo  != NULL);

    *ppinfo = NULL;

    pcinfo = (fcnotify_info *)alloc_emalloc(sizeof(fcnotify_info));
    if(pcinfo == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    lock_lock(pnotify->fclock);
    flock = 1;

    pcinfo->itemcount = pnotify->fcheader->itemcount;
    pcinfo->entries   = NULL;

    /* Leave count to 0 if only summary is needed */
    if(!summaryonly)
    {
        count = pnotify->fcheader->valuecount;
    }

    for(index = 0; index < count; index++)
    {
        offset = pnotify->fcheader->values[index];
        if(offset == 0)
        {
            continue;
        }

        pvalue = (fcnotify_value *)alloc_get_cachevalue(pnotify->fcalloc, offset);
        while(pvalue != NULL)
        {
            ptemp = (fcnotify_entry_info *)alloc_emalloc(sizeof(fcnotify_entry_info));
            if(ptemp == NULL)
            {
                result = FATAL_OUT_OF_LMEMORY;
                goto Finished;
            }

            _ASSERT(pvalue->folder_path != 0);

            ptemp->folderpath = alloc_estrdup(pnotify->fcmemaddr + pvalue->folder_path);
            ptemp->ownerpid   = pvalue->owner_pid;
            ptemp->filecount  = pvalue->refcount;
            ptemp->next       = NULL;

            if(peinfo != NULL)
            {
                peinfo->next = ptemp;
            }
            else
            {
                pcinfo->entries = ptemp;
            }

            peinfo = ptemp;

            offset = pvalue->next_value;
            if(offset == 0)
            {
                break;
            }

            pvalue = (fcnotify_value *)alloc_get_cachevalue(pnotify->fcalloc, offset);
        }
    }

    *ppinfo = pcinfo;
    _ASSERT(SUCCEEDED(result));

Finished:

    if(flock)
    {
        lock_unlock(pnotify->fclock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in fcnotify_getinfo", result);

        if(pcinfo != NULL)
        {
            peinfo = pcinfo->entries;
            while(peinfo != NULL)
            {
                ptemp = peinfo;
                peinfo = peinfo->next;

                if(ptemp->folderpath != NULL)
                {
                    alloc_efree(ptemp->folderpath);
                    ptemp->folderpath = NULL;
                }

                alloc_efree(ptemp);
                ptemp = NULL;
            }

            alloc_efree(pcinfo);
            pcinfo = NULL;
        }
    }

    dprintverbose("start fcnotify_getinfo");

    return result;
}

void fcnotify_freeinfo(fcnotify_info * pinfo)
{
    fcnotify_entry_info * peinfo = NULL;
    fcnotify_entry_info * petemp = NULL;

    if(pinfo != NULL)
    {
        peinfo = pinfo->entries;
        while(peinfo != NULL)
        {
            petemp = peinfo;
            peinfo = peinfo->next;

            if(petemp->folderpath != NULL)
            {
                alloc_efree(petemp->folderpath);
                petemp->folderpath = NULL;
            }

            alloc_efree(petemp);
            petemp = NULL;
        }

        alloc_efree(pinfo);
        pinfo = NULL;
    }
}

void fcnotify_runtest()
{
    return;
}
