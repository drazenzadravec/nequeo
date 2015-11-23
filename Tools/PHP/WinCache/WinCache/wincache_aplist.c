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
   | Module: wincache_aplist.c                                                                    |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#include "precomp.h"

#define PER_RUN_SCAVENGE_COUNT        256
#define DO_PARTIAL_SCAVENGER_RUN      0
#define DO_FULL_SCAVENGER_RUN         1

#define SCAVENGER_STATUS_INVALID      255
#define SCAVENGER_STATUS_INACTIVE     0
#define SCAVENGER_STATUS_ACTIVE       1

#define DO_FILE_CHANGE_CHECK          1
#define NO_FILE_CHANGE_CHECK          0
#define FILE_IS_NOT_CHANGED           0
#define FILE_IS_CHANGED               1

#define APLIST_VALUE(p, o)            ((aplist_value *)alloc_get_cachevalue(p, o))

static int  find_aplist_entry(aplist_context * pcache, const char * filename, unsigned int index, unsigned char docheck, aplist_value ** ppvalue, aplist_value ** ppdelete);
static int  is_file_changed(aplist_context * pcache, aplist_value * pvalue);
static int  create_aplist_data(aplist_context * pcache, const char * filename, aplist_value ** ppvalue);
static void destroy_aplist_data(aplist_context * pcache, aplist_value * pvalue);
static void add_aplist_entry(aplist_context * pcache, unsigned int index, aplist_value * pvalue);
static void remove_aplist_entry(aplist_context * pcache, unsigned int index, aplist_value * pvalue);
static void delete_aplist_fileentry(aplist_context * pcache, const char * filename);
static void run_aplist_scavenger(aplist_context * pcache, unsigned char ffull);
static int  set_lastcheck_time(aplist_context * pcache, const char * filename, unsigned int newvalue);

/* Globals */
unsigned short glcacheid = 1;

/* Private methods */

/* Call this method atleast under a read lock */
/* If an entry for filename with is_deleted is found, return that in ppdelete */
static int find_aplist_entry(aplist_context * pcache, const char * filename, unsigned int index, unsigned char docheck, aplist_value ** ppvalue, aplist_value ** ppdelete)
{
    int             result = NONFATAL;
    aplist_header * header = NULL;
    aplist_value *  pvalue = NULL;

    dprintverbose("start find_aplist_entry");

    _ASSERT(pcache   != NULL);
    _ASSERT(filename != NULL);
    _ASSERT(ppvalue  != NULL);

    *ppvalue  = NULL;
    if(ppdelete != NULL)
    {
        *ppdelete = NULL;
    }

    header = pcache->apheader;
    pvalue = APLIST_VALUE(pcache->apalloc, header->values[index]);

    while(pvalue != NULL)
    {
        if(!_stricmp(pcache->apmemaddr + pvalue->file_path, filename))
        {
            /* Ignore entries which are marked deleted */
            if(pvalue->is_deleted == 1)
            {
                /* If this process is good to delete the */
                /* cache data tell caller to delete it */
                if(ppdelete != NULL && *ppdelete == NULL &&
                   pcache->apctype == APLIST_TYPE_GLOBAL)
                {
                    *ppdelete = pvalue;
                }

                pvalue = APLIST_VALUE(pcache->apalloc, pvalue->next_value);
                continue;
            }

            if(docheck)
            {
                /* If fcnotify is 0, use traditional file change check */
                if(pvalue->fcnotify == 0)
                {
                    if(is_file_changed(pcache, pvalue))
                    {
                        result = FATAL_FCACHE_FILECHANGED;
                    }
                }
                else
                {
                    if(pvalue->is_changed == 1)
                    {
                        result = FATAL_FCACHE_FILECHANGED;
                    }
                    else
                    {
                        /* If a listener is present, just check if listener is still active */
                        if(pcache->pnotify != NULL)
                        {
                            result = fcnotify_check(pcache->pnotify, NULL, &pvalue->fcnotify, &pvalue->fcncount);
                            if(FAILED(result))
                            {
                                /* Do a hard file change check if file listener is suggesting it */
                                if(result == WARNING_FCNOTIFY_FORCECHECK)
                                {
                                    result = NONFATAL;
                                    if(is_file_changed(pcache, pvalue))
                                    {
                                        result = FATAL_FCACHE_FILECHANGED;
                                    }
                                }
                                else
                                {
                                    goto Finished;
                                }
                            }
                        }
                    }
                }
            }

            break;
        }

        pvalue = APLIST_VALUE(pcache->apalloc, pvalue->next_value);
    }

    *ppvalue = pvalue;

Finished:

    dprintverbose("end find_aplist_entry");

    return result;
}

/* Call this method atleast under a read lock */
static int is_file_changed(aplist_context * pcache, aplist_value * pvalue)
{
    HRESULT                   hr        = S_OK;
    int                       retvalue  = 0;
    unsigned int              tickcount = 0;
    unsigned int              difftime  = 0;
    WIN32_FILE_ATTRIBUTE_DATA fileData;

    dprintverbose("start is_file_changed");

    _ASSERT(pcache != NULL);
    _ASSERT(pvalue != NULL);

    /* If fchangefreq is set to 0, dont do the file change check */
    /* last_check value of 0 means that a check must be done */
    if(pvalue->last_check != 0 && pcache->fchangefreq == 0)
    {
        goto Finished;
    }

    /* Calculate difftime while taking care of rollover */
    tickcount = GetTickCount();
    difftime = utils_ticksdiff(tickcount, pvalue->last_check);

    if(pvalue->last_check != 0 && difftime < pcache->fchangefreq)
    {
        goto Finished;
    }

    retvalue = 1;
    if (!GetFileAttributesEx(pcache->apmemaddr + pvalue->file_path, GetFileExInfoStandard, &fileData))
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        dprintimportant("GetFileAttributesEx returned error %d", hr);

        if (hr == HRESULT_FROM_WIN32(ERROR_BAD_NET_NAME) ||
            hr == HRESULT_FROM_WIN32(ERROR_BAD_NETPATH) ||
            hr == HRESULT_FROM_WIN32(ERROR_NETNAME_DELETED) ||
            hr == HRESULT_FROM_WIN32(ERROR_REM_NOT_LIST) ||
            hr == HRESULT_FROM_WIN32(ERROR_SEM_TIMEOUT) ||
            hr == HRESULT_FROM_WIN32(ERROR_NETWORK_BUSY) ||
            hr == HRESULT_FROM_WIN32(ERROR_UNEXP_NET_ERR) ||
            hr == HRESULT_FROM_WIN32(ERROR_NETWORK_UNREACHABLE))
        {
            retvalue = 0;
        }

        goto Finished;
    }

    _ASSERT(fileData.nFileSizeHigh == 0);

    /* If the attributes, WriteTime, and if it is a file, size are same */
    /* then the file has not changed. File sizes greater than 4GB won't */
    /* get added to cache. So comparing only nFileSizeLow is sufficient */
    if (fileData.dwFileAttributes == pvalue->attributes &&
        *(__int64 *)&fileData.ftLastWriteTime / 10000000 ==
         *(__int64 *)&pvalue->modified_time / 10000000 &&
        ((fileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) ||
         (fileData.nFileSizeLow == pvalue->file_size)))
    {
        retvalue = 0;
    }

    pvalue->last_check = tickcount;

Finished:

    dprintverbose("end is_file_changed");

    return retvalue;
}

/* Call this method without any lock */
static int create_aplist_data(aplist_context * pcache, const char * filename, aplist_value ** ppvalue)
{
    int                        result    = NONFATAL;
    HRESULT                    hr        = S_OK;
    unsigned int               ticks     = 0;
    char *                     filepath  = NULL;

    HANDLE                     hFile     = INVALID_HANDLE_VALUE;
    LARGE_INTEGER              li        = { 0 };
    unsigned int               openflags = 0;
    BY_HANDLE_FILE_INFORMATION finfo;
    aplist_value *             pvalue    = NULL;

    size_t                     flength   = 0;
    size_t                     alloclen  = 0;
    char *                     pbaseadr  = NULL;
    char *                     pcurrent  = NULL;

    dprintverbose("start create_aplist_data");

    _ASSERT(pcache   != NULL);
    _ASSERT(filename != NULL);
    _ASSERT(ppvalue  != NULL);

    *ppvalue = NULL;

    flength = strlen(filename);
    alloclen = sizeof(aplist_value) + ALIGNDWORD(flength + 1);

    /* Allocate memory for cache entry in shared memory */
    pbaseadr = (char *)alloc_smalloc(pcache->apalloc, alloclen);
    if(pbaseadr == NULL)
    {
        /* If scavenger is active, run it and try allocating again */
        if(pcache->scstatus == SCAVENGER_STATUS_ACTIVE)
        {
            lock_lock(pcache->aplock);

            run_aplist_scavenger(pcache, DO_FULL_SCAVENGER_RUN);
            pcache->apheader->lscavenge = GetTickCount();

            lock_unlock(pcache->aplock);

            pbaseadr = (char *)alloc_smalloc(pcache->apalloc, alloclen);
            if(pbaseadr == NULL)
            {
                result = FATAL_OUT_OF_SMEMORY;
                goto Finished;
            }
        }
        else
        {
            result = FATAL_OUT_OF_SMEMORY;
            goto Finished;
        }
    }

    pcurrent = pbaseadr;

    pvalue = (aplist_value *)pcurrent;
    pvalue->file_path = 0;

    /* Open the file in shared read mode */
    openflags |= FILE_ATTRIBUTE_ENCRYPTED;
    openflags |= FILE_FLAG_OVERLAPPED;
    openflags |= FILE_FLAG_BACKUP_SEMANTICS;
    openflags |= FILE_FLAG_SEQUENTIAL_SCAN;

    hFile = CreateFile(filename, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, openflags, NULL);
    if(hFile == INVALID_HANDLE_VALUE)
    {
        error_setlasterror();
        result = FATAL_FCACHE_CREATEFILE;

        goto Finished;
    }

    /* Fail if file type is not of type_disk */
    if(GetFileType(hFile) != FILE_TYPE_DISK)
    {
        result = FATAL_FCACHE_GETFILETYPE;
        goto Finished;
    }

    if(0 == GetFileSizeEx(hFile, &li))
    {
        error_setlasterror();
        result = FATAL_FCACHE_GETFILESIZE;

        goto Finished;
    }

    /* Fail if file is larger than 4GB */
    if (li.HighPart != 0)
    {
        result = FATAL_FCACHE_FILE_TOO_BIG;
    }

    if(!GetFileInformationByHandle(hFile, &finfo))
    {
        error_setlasterror();
        result = FATAL_FCACHE_FILEINFO;

        goto Finished;
    }

    /* Point pcurrent to end of aplist_value */
    /* No goto Finished should exist after this */
    pcurrent += sizeof(aplist_value);

    /* Fill the details in aplist_value */
    /* memcpy_s error code ignored as buffer is of right size */
    memcpy_s(pcurrent, flength, filename, flength);
    *(pcurrent + flength) = 0;
    pvalue->file_path     = pcurrent - pcache->apmemaddr;

    pvalue->file_size     = li.LowPart;
    pvalue->modified_time = finfo.ftLastWriteTime;
    pvalue->attributes    = finfo.dwFileAttributes;

    ticks               = GetTickCount();
    pvalue->add_ticks   = ticks;
    pvalue->use_ticks   = ticks;
    pvalue->last_check  = ticks;
    pvalue->is_deleted  = 0;
    pvalue->is_changed  = 0;

    pvalue->fcacheval   = 0;
    pvalue->resentry    = 0;
    pvalue->fcnotify    = 0;
    pvalue->fcncount    = 0;
    pvalue->prev_value  = 0;
    pvalue->next_value  = 0;

    /* Add file listener only if the file is not pointing to a UNC share */
    if(pcache->pnotify != NULL && IS_UNC_PATH(filename, flength) == 0)
    {
        result = fcnotify_check(pcache->pnotify, filename, &pvalue->fcnotify, &pvalue->fcncount);
        if(FAILED(result))
        {
            goto Finished;
        }
    }

    *ppvalue = pvalue;

    _ASSERT(SUCCEEDED(result));

Finished:

    if(hFile != INVALID_HANDLE_VALUE)
    {
        CloseHandle(hFile);
        hFile = INVALID_HANDLE_VALUE;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in create_aplist_data", result);

        if(pbaseadr != NULL)
        {
            alloc_sfree(pcache->apalloc, pbaseadr);
            pbaseadr = NULL;
        }
    }

    dprintverbose("end create_aplist_data");

    return result;
}

/* Call this method under lock if resentry can be non-zero so that */
/* rplist can never have an offset of aplist which is not valid */
static void destroy_aplist_data(aplist_context * pcache, aplist_value * pvalue)
{
    dprintverbose("start destroy_aplist_data");

    if(pvalue != NULL)
    {
        if(pcache->pnotify != NULL && pvalue->fcnotify != 0)
        {
            fcnotify_close(pcache->pnotify, &pvalue->fcnotify, &pvalue->fcncount);
        }

        /* Resolve path cache and file cache entries */
        /* should be deleted by a call to remove_aplist_entry */
        _ASSERT(pvalue->resentry  == 0);
        _ASSERT(pvalue->fcacheval == 0);

        alloc_sfree(pcache->apalloc, pvalue);
        pvalue = NULL;
    }

    dprintverbose("end destroy_aplist_data");
}

/* Call this method under the lock */
static void add_aplist_entry(aplist_context * pcache, unsigned int index, aplist_value * pvalue)
{
    aplist_header * header  = NULL;
    aplist_value *  pcheck  = NULL;

    dprintverbose("start add_aplist_entry");

    _ASSERT(pcache            != NULL);
    _ASSERT(pvalue            != NULL);
    _ASSERT(pvalue->file_path != 0);

    header = pcache->apheader;
    pcheck = APLIST_VALUE(pcache->apalloc, header->values[index]);

    /* New entry gets added in the end. This is required for is_deleted to work correctly */
    while(pcheck != NULL)
    {
        if(pcheck->next_value == 0)
        {
            break;
        }

        pcheck = APLIST_VALUE(pcache->apalloc, pcheck->next_value);
    }

    if(pcheck != NULL)
    {
        pcheck->next_value = alloc_get_valueoffset(pcache->apalloc, pvalue);
        pvalue->next_value = 0;
        pvalue->prev_value = alloc_get_valueoffset(pcache->apalloc, pcheck);
    }
    else
    {
        header->values[index] = alloc_get_valueoffset(pcache->apalloc, pvalue);
        pvalue->next_value = 0;
        pvalue->prev_value = 0;
    }

    header->itemcount++;

    dprintverbose("end add_aplist_entry");
    return;
}

/* Call this method under the lock */
static void remove_aplist_entry(aplist_context * pcache, unsigned int index, aplist_value * pvalue)
{
    alloc_context * apalloc = NULL;
    aplist_header * header  = NULL;
    aplist_value *  ptemp   = NULL;
    fcache_value *  pfvalue = NULL;

    dprintverbose("start remove_aplist_entry");

    _ASSERT(pcache            != NULL);
    _ASSERT(pvalue            != NULL);
    _ASSERT(pvalue->file_path != 0);

    /* Delete resolve path cache entries */
    if(pvalue->resentry != 0)
    {
        _ASSERT(pcache->prplist != NULL);

        rplist_deleteval(pcache->prplist, pvalue->resentry);
        pvalue->resentry = 0;
    }

    /* Delete file cache entry */
    if(pvalue->fcacheval != 0)
    {
        _ASSERT(pcache->pfcache != NULL);
        pfvalue = fcache_getvalue(pcache->pfcache, pvalue->fcacheval);

        InterlockedExchange(&pfvalue->is_deleted, 1);
        pvalue->fcacheval = 0;

        if(InterlockedCompareExchange(&pfvalue->refcount, 0, 0) == 0)
        {
            fcache_destroyval(pcache->pfcache, pfvalue);
            pfvalue = NULL;
        }
    }

    apalloc = pcache->apalloc;
    header  = pcache->apheader;

    /* Decrement itemcount and remove entry from hashtable */
    header->itemcount--;
    if(pvalue->prev_value == 0)
    {
        header->values[index] = pvalue->next_value;
        if(pvalue->next_value != 0)
        {
            ptemp = APLIST_VALUE(apalloc, pvalue->next_value);
            ptemp->prev_value = 0;
        }
    }
    else
    {
        ptemp = APLIST_VALUE(apalloc, pvalue->prev_value);
        ptemp->next_value = pvalue->next_value;

        if(pvalue->next_value != 0)
        {
            ptemp = APLIST_VALUE(apalloc, pvalue->next_value);
            ptemp->prev_value = pvalue->prev_value;
        }
    }

    /* Destroy aplist data now that fcache is deleted */
    destroy_aplist_data(pcache, pvalue);
    pvalue = NULL;

    dprintverbose("end remove_aplist_entry");
    return;
}

/* Call this method under the lock */
static void delete_aplist_fileentry(aplist_context * pcache, const char * filename)
{
    unsigned int   findex = 0;
    aplist_value * pvalue = NULL;

    dprintverbose("start delete_aplist_fileentry");

    /* This method should be called to remove file entry from a local cache */
    /* list when the file is removed the global list to keep them in sync */
    _ASSERT(pcache          != NULL);
    _ASSERT(pcache->islocal != 0);
    _ASSERT(filename        != NULL);
    _ASSERT(IS_ABSOLUTE_PATH(filename, strlen(filename)));

    findex = utils_getindex(filename, pcache->apheader->valuecount);
    find_aplist_entry(pcache, filename, findex, NO_FILE_CHANGE_CHECK, &pvalue, NULL);

    /* If an entry is found for this file, remove it */
    if(pvalue != NULL)
    {
        remove_aplist_entry(pcache, findex, pvalue);
        pvalue = NULL;
    }

    dprintverbose("end delete_aplist_fileentry");
    return;
}

/* Call this method under tje lock */
static void run_aplist_scavenger(aplist_context * pcache, unsigned char ffull)
{
    unsigned int    sindex   = 0;
    unsigned int    eindex   = 0;

    alloc_context * apalloc  = NULL;
    aplist_header * apheader = NULL;
    aplist_value *  ptemp    = NULL;
    aplist_value *  pvalue   = NULL;
    unsigned int    ticks    = 0;

    dprintverbose("start run_aplist_scavenger");

    _ASSERT(pcache           != NULL);
    _ASSERT(pcache->scstatus == SCAVENGER_STATUS_ACTIVE);

    ticks    = GetTickCount();
    apalloc  = pcache->apalloc;
    apheader = pcache->apheader;

    if(ffull)
    {
        sindex = 0;
        eindex = apheader->valuecount;
        apheader->scstart = 0;
    }
    else
    {
        sindex = apheader->scstart;
        eindex = sindex + PER_RUN_SCAVENGE_COUNT;
        apheader->scstart = eindex;

        if(eindex >= apheader->valuecount)
        {
            eindex = apheader->valuecount;
            apheader->scstart = 0;
        }
    }

    dprintimportant("aplist scavenger sindex = %d, eindex = %d", sindex, eindex);
    for( ;sindex < eindex; sindex++)
    {
        if(apheader->values[sindex] == 0)
        {
            continue;
        }

        pvalue = APLIST_VALUE(apalloc, apheader->values[sindex]);
        while(pvalue != NULL)
        {
            ptemp = pvalue;
            pvalue = APLIST_VALUE(apalloc, pvalue->next_value);

            /* Remove entry if its marked deleted or is unused for ttlmax time */
            if(ptemp->is_deleted || (utils_ticksdiff(ticks, ptemp->use_ticks) > apheader->ttlmax))
            {
                remove_aplist_entry(pcache, sindex, ptemp);
                ptemp = NULL;
            }
        }
    }

    dprintverbose("end run_aplist_scavenger");
    return;
}

/* Public functions */
int aplist_create(aplist_context ** ppcache)
{
    int              result = NONFATAL;
    aplist_context * pcache = NULL;

    dprintverbose("start aplist_create");

    _ASSERT(ppcache != NULL);
    *ppcache = NULL;

    pcache = (aplist_context *)alloc_pemalloc(sizeof(aplist_context));
    if(pcache == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    pcache->id          = glcacheid++;
    pcache->islocal     = 0;
    pcache->apctype     = APLIST_TYPE_INVALID;
    pcache->scstatus    = SCAVENGER_STATUS_INVALID;
    pcache->hinitdone   = NULL;
    pcache->fchangefreq = 0;

    pcache->apmemaddr   = NULL;
    pcache->apheader    = NULL;
    pcache->apfilemap   = NULL;
    pcache->aplock      = NULL;
    pcache->apalloc     = NULL;

    pcache->prplist     = NULL;
    pcache->pnotify     = NULL;
    pcache->pfcache     = NULL;
    pcache->resnumber   = -1;

    *ppcache = pcache;

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in aplist_create", result);
    }

    dprintverbose("end aplist_create");

    return result;
}

void aplist_destroy(aplist_context * pcache)
{
    dprintverbose("start aplist_destroy");

    if(pcache != NULL)
    {
        alloc_pefree(pcache);
        pcache = NULL;
    }

    dprintverbose("end aplist_destroy");

    return;
}

int aplist_initialize(aplist_context * pcache, unsigned short apctype, unsigned int filecount, unsigned int fchangefreq, unsigned int ttlmax)
{
    int             result      = NONFATAL;
    unsigned int    mapsize     = 0;
    size_t          segsize     = 0;
    aplist_header * header      = NULL;
    unsigned int    msize       = 0;
    unsigned short  cachekey    = 1;

    unsigned int    cticks      = 0;
    unsigned short  mapclass    = FILEMAP_MAP_SRANDOM;
    unsigned short  locktype    = LOCK_TYPE_SHARED;
    unsigned char   isfirst     = 1;
    unsigned char   islocked    = 0;
    unsigned int    initmemory  = 0;
    char          * prefix      = NULL;
    size_t          cchprefix   = 0;
    DWORD           ret         = 0;

    dprintverbose("start aplist_initialize");

    _ASSERT(pcache       != NULL);
    _ASSERT(filecount    >= NUM_FILES_MINIMUM   && filecount   <= NUM_FILES_MAXIMUM);
    _ASSERT((fchangefreq >= FCHECK_FREQ_MINIMUM && fchangefreq <= FCHECK_FREQ_MAXIMUM) || fchangefreq == 0);
    _ASSERT((ttlmax      >= TTL_VALUE_MINIMUM   && ttlmax      <= TTL_VALUE_MAXIMUM)   || ttlmax == 0);

    /* If more APLIST_TYPE entries are added, add code to control value of islocal carefully */
    pcache->apctype = apctype;
    pcache->islocal = pcache->apctype;

    /* Disable scavenger if ttlmax value is set to 0 */
    pcache->scstatus = SCAVENGER_STATUS_ACTIVE;
    if(ttlmax == 0)
    {
        pcache->scstatus = SCAVENGER_STATUS_INACTIVE;
    }

    /* Initialize memory map to store file list */
    result = filemap_create(&pcache->apfilemap);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* Desired cache size 2MB for 1024 entries. Expression below gives that */
    mapsize = ((filecount + 1023) * 2) / 1024;
    if(pcache->islocal)
    {
        mapclass = FILEMAP_MAP_LRANDOM;
        locktype = LOCK_TYPE_LOCAL;
    }

    /* get object name prefix */
    result = lock_get_nameprefix("FILELIST_CACHE", cachekey, locktype, &prefix, &cchprefix);
    if (FAILED(result))
    {
        goto Finished;
    }

    result = utils_create_init_event(prefix, "APLIST_INIT", &pcache->hinitdone, &isfirst);
    if (FAILED(result))
    {
        result = FATAL_APLIST_INIT_EVENT;
        goto Finished;
    }

    islocked = 1;

    /* shmfilepath = NULL to make it use page file */
    result = filemap_initialize(pcache->apfilemap, FILEMAP_TYPE_FILELIST, cachekey, mapclass, mapsize, isfirst, NULL);
    if(FAILED(result))
    {
        goto Finished;
    }

    pcache->apmemaddr = (char *)pcache->apfilemap->mapaddr;
    segsize = filemap_getsize(pcache->apfilemap);
    initmemory = (pcache->apfilemap->existing == 0);

    /* Create allocator for file list segment */
    result = alloc_create(&pcache->apalloc);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* initmemory = 1 for all page file backed shared memory allocators */
    result = alloc_initialize(pcache->apalloc, pcache->islocal, "FILELIST_SEGMENT", cachekey, pcache->apfilemap->mapaddr, segsize, 1);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* Get memory for cache header */
    msize = sizeof(aplist_header) + ((filecount - 1) * sizeof(size_t));
    pcache->apheader = (aplist_header *)alloc_get_cacheheader(pcache->apalloc, msize, CACHE_TYPE_FILELIST);
    if(pcache->apheader == NULL)
    {
        result = FATAL_FCACHE_INITIALIZE;
        goto Finished;
    }

    header = pcache->apheader;

    /* Create reader writer locks for the aplist */
    result = lock_create(&pcache->aplock);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = lock_initialize(pcache->aplock, "FILELIST_CACHE", cachekey, locktype, &header->last_owner);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* Create resolve path cache for global aplist cache */
    if(apctype == APLIST_TYPE_GLOBAL)
    {
        result = rplist_create(&pcache->prplist);
        if(FAILED(result))
        {
            goto Finished;
        }

        result = rplist_initialize(pcache->prplist, pcache->islocal, isfirst, cachekey, filecount);
        if(FAILED(result))
        {
            goto Finished;
        }

        /* If file change notification is enabled, create fcnotify_context */
        if(WCG(fcndetect))
        {
            result = fcnotify_create(&pcache->pnotify);
            if(FAILED(result))
            {
                goto Finished;
            }

            /* Number of folders on which listeners will be active will */
            /* be very small. Using filecount as 32 so that scavenger is quick */
            result = fcnotify_initialize(pcache->pnotify, pcache->islocal, pcache, pcache->apalloc, 32);
            if(FAILED(result))
            {
                goto Finished;
            }
        }
    }

    /* Initialize the aplist_header if this is the first process */
    if(pcache->islocal || isfirst)
    {
        if (initmemory)
        {
            /* No need to get a lock as other processes */
            /* are blocked waiting for hinitdone event */

            /* Initialize resolve path cache header */
            if(pcache->prplist != NULL)
            {
                rplist_initheader(pcache->prplist, filecount);
            }

            if(pcache->pnotify != NULL)
            {
                fcnotify_initheader(pcache->pnotify, 32);
            }

            cticks = GetTickCount();

            /* We can set last_owner to 0 safely as other processes are */
            /* blocked and this process is right now not using lock */
            header->mapcount     = 1;
            header->init_ticks   = cticks;
            header->last_owner   = 0;
            header->itemcount    = 0;

            _ASSERT(filecount > PER_RUN_SCAVENGE_COUNT);

            /* Calculate scavenger frequency if ttlmax is not 0 */
            if(ttlmax != 0)
            {
                header->ttlmax       = ttlmax * 1000;
                header->scfreq       = header->ttlmax/(filecount/PER_RUN_SCAVENGE_COUNT);
                header->lscavenge    = cticks;
                header->scstart      = 0;

                _ASSERT(header->scfreq > 0);
            }

            header->valuecount   = filecount;
            memset((void *)header->values, 0, sizeof(size_t) * filecount);

            ReleaseMutex(pcache->hinitdone);
            islocked = 0;
        }
    }
    else
    {
        /* Increment the mapcount */
        InterlockedIncrement(&header->mapcount);
    }

    /* Keep fchangefreq in aplist_context */
    pcache->fchangefreq = fchangefreq * 1000;

Finished:

    if (islocked)
    {
        ReleaseMutex(pcache->hinitdone);
        islocked = 0;
    }

    if (prefix != NULL)
    {
        alloc_pefree(prefix);
        prefix = NULL;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in aplist_initialize", result);

        /* Must cleanup in exactly reverse order of creation. */

        if(pcache->hinitdone != NULL)
        {
            CloseHandle(pcache->hinitdone);
            pcache->hinitdone = NULL;
        }

        if(pcache->pnotify != NULL)
        {
            fcnotify_terminate(pcache->pnotify);
            fcnotify_destroy(pcache->pnotify);

            pcache->pnotify = NULL;
        }

        if(pcache->prplist != NULL)
        {
            rplist_terminate(pcache->prplist);
            rplist_destroy(pcache->prplist);

            pcache->prplist = NULL;
        }

        if(pcache->aplock != NULL)
        {
            lock_terminate(pcache->aplock);
            lock_destroy(pcache->aplock);

            pcache->aplock = NULL;
        }

        if (pcache->apheader != NULL)
        {
            /*
             * We don't need to decrement the mapcount, since we never
             * incremented it.
             */
            pcache->apheader = NULL;
        }

        if(pcache->apalloc != NULL)
        {
            alloc_terminate(pcache->apalloc);
            alloc_destroy(pcache->apalloc);

            pcache->apalloc = NULL;
        }

        if(pcache->apfilemap != NULL)
        {
            filemap_terminate(pcache->apfilemap);
            filemap_destroy(pcache->apfilemap);

            pcache->apfilemap = NULL;
        }

        pcache->apheader = NULL;
    }

    dprintverbose("end aplist_initialize");

    return result;
}

int aplist_fcache_initialize(aplist_context * plcache, unsigned int size, unsigned int maxfilesize)
{
    int              result  = NONFATAL;
    fcache_context * pfcache = NULL;

    dprintverbose("start aplist_fcache_initialize");

    _ASSERT(plcache != NULL);

    result = fcache_create(&pfcache);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = fcache_initialize(pfcache, plcache->islocal, 1, size, maxfilesize);
    if(FAILED(result))
    {
        goto Finished;
    }

    plcache->pfcache = pfcache;

    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in aplist_fcache_initialize", result);

        if(pfcache != NULL)
        {
            fcache_terminate(pfcache);
            fcache_destroy(pfcache);

            pfcache = NULL;
        }
    }

    dprintverbose("end aplist_fcache_initialize");

    return result;
}

void aplist_terminate(aplist_context * pcache)
{
    dprintverbose("start aplist_terminate");

    if(pcache != NULL)
    {
        /* Must cleanup in exactly reverse order of creation. */

        if(pcache->hinitdone != NULL)
        {
            CloseHandle(pcache->hinitdone);
            pcache->hinitdone = NULL;
        }

        if(pcache->pnotify != NULL)
        {
            fcnotify_terminate(pcache->pnotify);
            fcnotify_destroy(pcache->pnotify);

            pcache->pnotify = NULL;
        }

        if(pcache->prplist != NULL)
        {
            rplist_terminate(pcache->prplist);
            rplist_destroy(pcache->prplist);

            pcache->prplist = NULL;
        }

        if(pcache->pfcache != NULL)
        {
            fcache_terminate(pcache->pfcache);
            fcache_destroy(pcache->pfcache);

            pcache->pfcache = NULL;
        }

        if(pcache->aplock != NULL)
        {
            lock_terminate(pcache->aplock);
            lock_destroy(pcache->aplock);

            pcache->aplock = NULL;
        }

        if(pcache->apheader != NULL)
        {
            InterlockedDecrement(&pcache->apheader->mapcount);
            pcache->apheader = NULL;
        }

        if(pcache->apalloc != NULL)
        {
            alloc_terminate(pcache->apalloc);
            alloc_destroy(pcache->apalloc);

            pcache->apalloc = NULL;
        }

        if(pcache->apfilemap != NULL)
        {
            filemap_terminate(pcache->apfilemap);
            filemap_destroy(pcache->apfilemap);

            pcache->apfilemap = NULL;
        }
    }

    dprintverbose("end aplist_terminate");

    return;
}

int aplist_getentry(aplist_context * pcache, const char * filename, unsigned int findex, aplist_value ** ppvalue)
{
    int              result   = NONFATAL;
    unsigned int     ticks    = 0;
    unsigned char    flock    = 0;
    unsigned char    fchange  = FILE_IS_NOT_CHANGED;
    unsigned int     index    = 0;
    unsigned int     addtick  = 0;

    aplist_value *   pvalue   = NULL;
    aplist_value *   pdelete  = NULL;
    aplist_value *   pdummy   = NULL;
    aplist_value *   pnewval  = NULL;
    aplist_header *  apheader = NULL;

    dprintverbose("start aplist_getentry");

    _ASSERT(pcache   != NULL);
    _ASSERT(filename != NULL);
    _ASSERT(ppvalue  != NULL);

    *ppvalue = NULL;

    apheader = pcache->apheader;
    ticks    = GetTickCount();

    lock_lock(pcache->aplock);
    flock = 1;

    /* Check if scavenger is active for this process and if yes run the partual scavenger */
    if(pcache->scstatus == SCAVENGER_STATUS_ACTIVE && (utils_ticksdiff(ticks, apheader->lscavenge) > apheader->scfreq))
    {
        if(utils_ticksdiff(ticks, apheader->lscavenge) > apheader->scfreq)
        {
            run_aplist_scavenger(pcache, DO_PARTIAL_SCAVENGER_RUN);
            apheader->lscavenge = GetTickCount();
        }
    }

    /* Find file in hashtable and also get any entry for the file if mark deleted */
    result = find_aplist_entry(pcache, filename, findex, DO_FILE_CHANGE_CHECK, &pvalue, &pdelete);
    if(pvalue != NULL)
    {
        addtick = pvalue->add_ticks;
        if(SUCCEEDED(result))
        {
            pvalue->use_ticks = ticks;
        }
    }

    if(FAILED(result))
    {
        if(result != FATAL_FCACHE_FILECHANGED)
        {
            goto Finished;
        }

        fchange = FILE_IS_CHANGED;
    }

    /* If the entry was not found in cache */
    /* or if the cache entry is stale, create a new entry */
    if(fchange == FILE_IS_CHANGED || pvalue == NULL)
    {
        /* Must drop lock before calling create_aplist_data */
        lock_unlock(pcache->aplock);
        flock = 0;

        result = create_aplist_data(pcache, filename, &pnewval);
        if(FAILED(result))
        {
            goto Finished;
        }

        /* reaquire lock after creating aplist data */
        lock_lock(pcache->aplock);
        flock = 1;

        /* Check again after getting lock to see if something changed */
        result = find_aplist_entry(pcache, filename, findex, NO_FILE_CHANGE_CHECK, &pvalue, &pdelete);
        if(FAILED(result))
        {
            goto Finished;
        }

        /* If entry still needs to be deleted, delete it now */
        if(pdelete != NULL)
        {
            remove_aplist_entry(pcache, findex, pdelete);
            pdelete = NULL;
        }

        if(pvalue != NULL)
        {
            if(pvalue->add_ticks == addtick)
            {
                /* Only possible if a file change was detected. Remove this */
                /* entry from the hashtable. If some other process already */
                /* deleted and added the entry add_ticks value won't match */
                _ASSERT(fchange == FILE_IS_CHANGED);

                remove_aplist_entry(pcache, findex, pvalue);
                pvalue = NULL;
            }
            else
            {
                /* Some other process created a new value for this file */
                /* Destroy cache data we created and use value from cache */
                destroy_aplist_data(pcache, pnewval);
                pnewval = NULL;
            }
        }

        if(pnewval != NULL)
        {
            pvalue = pnewval;
            pnewval = NULL;

            add_aplist_entry(pcache, findex, pvalue);
        }
    }

    /* If an entry is to be deleted and is not already */
    /* deleted while creating new value, delete it now */
    if(pdelete != NULL)
    {
        /* Check again to see if anything changed before getting lock */
        result = find_aplist_entry(pcache, filename, findex, NO_FILE_CHANGE_CHECK, &pdummy, &pdelete);
        if(FAILED(result))
        {
            goto Finished;
        }

        /* If entry still needs to be deleted, delete it now */
        if(pdelete != NULL)
        {
            remove_aplist_entry(pcache, findex, pdelete);
            pdelete = NULL;
        }
    }

    _ASSERT(pvalue != NULL);
    *ppvalue = pvalue;

    _ASSERT(SUCCEEDED(result));

Finished:

    if(flock)
    {
        lock_unlock(pcache->aplock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in aplist_getentry", result);

        if(pnewval != NULL)
        {
            destroy_aplist_data(pcache, pnewval);
            pnewval = NULL;
        }
    }

    dprintverbose("end aplist_getentry");

    return result;
}

int aplist_force_fccheck(aplist_context * pcache, zval * filelist)
{
    int              result    = NONFATAL;
    zval *           fileentry = NULL;
    aplist_value *   pvalue    = NULL;
    unsigned char    flock     = 0;

    unsigned int     count     = 0;
    unsigned int     index     = 0;
    size_t           offset    = 0;
    const char *     execfile  = NULL;

    dprintverbose("start aplist_force_fccheck");

    _ASSERT(pcache   != NULL);
    _ASSERT(filelist == NULL || Z_TYPE_P(filelist) == IS_STRING || Z_TYPE_P(filelist) == IS_ARRAY);

    /* Get the lock once instead of taking the lock for each file */
    lock_lock(pcache->aplock);
    flock = 1;

    /* Always make currently executing file refresh on next load */
    if(filelist != NULL && zend_is_executing())
    {
        execfile = zend_get_executed_filename();

        result = set_lastcheck_time(pcache, execfile, 0);
        if(FAILED(result))
        {
            goto Finished;
        }
    }

    if(filelist == NULL)
    {
        /* Go through all the entries and set last_check to 0 */
        count = pcache->apheader->valuecount;
        for(index = 0; index < count; index++)
        {
            offset = pcache->apheader->values[index];
            if(offset == 0)
            {
                continue;
            }

            pvalue = (aplist_value *)alloc_get_cachevalue(pcache->apalloc, offset);
            while(pvalue != NULL)
            {
                _ASSERT(pvalue->file_path != 0);
                pvalue->last_check = 0;

                offset = pvalue->next_value;
                if(offset == 0)
                {
                    break;
                }

                pvalue = (aplist_value *)alloc_get_cachevalue(pcache->apalloc, offset);
            }
        }
    }
    else if(Z_TYPE_P(filelist) == IS_STRING)
    {
        if(Z_STRLEN_P(filelist) == 0)
        {
            goto Finished;
        }

        /* Set the last_check time of this file to 0 */
        result = set_lastcheck_time(pcache, Z_STRVAL_P(filelist), 0);
        if(FAILED(result))
        {
            goto Finished;
        }
    }
    else if(Z_TYPE_P(filelist) == IS_ARRAY)
    {
        ZEND_HASH_FOREACH_VAL(Z_ARRVAL_P(filelist), fileentry)
        {
            /* If array contains an entry which is not string, return false */
            if(Z_TYPE_P(fileentry) != IS_STRING)
            {
                result = FATAL_INVALID_ARGUMENT;
                goto Finished;
            }

            if(Z_STRLEN_P(fileentry) == 0)
            {
                continue;
            }

            result = set_lastcheck_time(pcache, Z_STRVAL_P(fileentry), 0);
            if(FAILED(result))
            {
                goto Finished;
            }

        } ZEND_HASH_FOREACH_END();
    }

    _ASSERT(SUCCEEDED(result));

Finished:

    if(flock)
    {
        lock_unlock(pcache->aplock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in aplist_force_fccheck", result);
    }

    dprintverbose("end aplist_force_fccheck");

    return result;
}

/* This method is called by change listener thread which will run in parallel */
/* to main thread. Make sure functions used can be used in multithreaded environment */
void aplist_mark_changed(aplist_context * pcache, char * folderpath, char * filename)
{
    unsigned int   findex   = 0;
    aplist_value * pvalue   = NULL;
    char           filepath[  MAX_PATH];

    _ASSERT(pcache     != NULL);
    _ASSERT(folderpath != NULL);
    _ASSERT(filename   != NULL);

    dprintverbose("start aplist_mark_changed");

    ZeroMemory(filepath, MAX_PATH);
    _snprintf_s(filepath, MAX_PATH, MAX_PATH - 1, "%s\\%s", folderpath, filename);

    aplist_mark_file_changed(pcache, filepath);

    dprintverbose("end aplist_mark_changed");

    return;
}

void aplist_mark_file_changed(aplist_context * pcache, char * filepath)
{
    unsigned int   findex   = 0;
    aplist_value * pvalue   = NULL;

    _ASSERT(pcache     != NULL);
    _ASSERT(filepath   != NULL);

    dprintverbose("start aplist_mark_file_changed");

    lock_lock(pcache->aplock);

    /* Find the entry for filepath and mark it deleted */
    findex = utils_getindex(filepath, pcache->apheader->valuecount);
    find_aplist_entry(pcache, filepath, findex, NO_FILE_CHANGE_CHECK, &pvalue, NULL);

    if(pvalue != NULL)
    {
        pvalue->is_changed = 1;
    }

    lock_unlock(pcache->aplock);

    dprintverbose("end aplist_mark_file_changed");

    return;
}

static int set_lastcheck_time(aplist_context * pcache, const char * filename, unsigned int newvalue)
{
    int           result       = NONFATAL;
    char *        resolve_path = NULL;
    unsigned int  findex       = 0;
    aplist_value * pvalue      = NULL;

    dprintverbose("start set_lastcheck_time");

    _ASSERT(pcache   != NULL);
    _ASSERT(filename != NULL);

    /* Ok to call aplist_fcache_get with lock acquired when last param is NULL */
    /* If file is not accessible or not present, we will throw an error */
    result = aplist_fcache_get(pcache, filename, SKIP_STREAM_OPEN_CHECK, &resolve_path, NULL);
    if(FAILED(result))
    {
        goto Finished;
    }

    findex = utils_getindex(resolve_path, pcache->apheader->valuecount);

    /* failure to find the entry in cache should be ignored */
    find_aplist_entry(pcache, resolve_path, findex, NO_FILE_CHANGE_CHECK, &pvalue, NULL);
    if(pvalue != NULL)
    {
        pvalue->last_check = 0;
    }

    _ASSERT(SUCCEEDED(result));

Finished:

    if (resolve_path)
    {
        alloc_efree(resolve_path);
        resolve_path = NULL;
    }

    if(FAILED(result))
    {
        dprintverbose("failure %d in set_lastcheck_time", result);
    }

    dprintverbose("end set_lastcheck_time");

    return result;
}

/* Used by wincache_resolve_path and wincache_stream_open_function. */
/* If ppvalue is passed as null, this function return the standardized form of */
/* filename which can include resolve path to absolute path mapping as well. */
/* Make sure this function is called without lock when ppvalue is non-null. */
int aplist_fcache_get(aplist_context * pcache, const char * filename, unsigned char usesopen, char ** ppfullpath, fcache_value ** ppvalue)
{
    int              result   = NONFATAL;
    size_t           length   = 0;
    unsigned int     findex   = 0;
    unsigned int     addticks = 0;
    unsigned char    fchanged = 0;
    unsigned char    flock    = 0;

    aplist_value *   pvalue   = NULL;
    fcache_value *   pfvalue  = NULL;
    rplist_value *   rpvalue  = NULL;
    size_t           resentry = 0;
    char *           fullpath = NULL;
    zend_file_handle fhandle  = {0};

    dprintverbose("start aplist_fcache_get (%s)", filename);

    _ASSERT(pcache     != NULL);
    _ASSERT(filename   != NULL);
    _ASSERT(ppfullpath != NULL);

    _ASSERT(pcache->prplist != NULL);

    if (ppvalue != NULL)
    {
        lock_lock(pcache->aplock);
        flock = 1;
    }

    /* Look for absolute path in resolve path cache first */
    /* All paths in resolve path cache are resolved using */
    /* include_path and don't need checks against open_basedir */
    result = rplist_getentry(pcache->prplist, filename, &rpvalue, &resentry);
    if(FAILED(result))
    {
        goto Finished;
    }

    _ASSERT(rpvalue  != NULL);
    _ASSERT(resentry != 0);

    /* If found, use new path to look into absolute path cache */
    if(rpvalue->absentry != 0)
    {
        pvalue = APLIST_VALUE(pcache->apalloc, rpvalue->absentry);

        /* If pvalue came directly from resolve path absentry, and if the */
        /* call is not just to get the fullpath, do a file change check here */
        if(pvalue != NULL && ppvalue != NULL)
        {
            if(pvalue->fcnotify == 0)
            {
                if(is_file_changed(pcache, pvalue))
                {
                    fchanged = 1;
                }
            }
            else
            {
                if(pvalue->is_changed == 1)
                {
                    fchanged = 1;
                }
                else
                {
                    /* If a listener is present, just check if listener is still active */
                    if(pcache->pnotify != NULL)
                    {
                        result = fcnotify_check(pcache->pnotify, NULL, &pvalue->fcnotify, &pvalue->fcncount);
                        if(FAILED(result))
                        {
                            /* Do a hard file change check if listener suggested it */
                            if(result == WARNING_FCNOTIFY_FORCECHECK)
                            {
                                result = NONFATAL;
                                if(is_file_changed(pcache, pvalue))
                                {
                                    fchanged = 1;
                                }
                            }
                            else
                            {
                                goto Finished;
                            }
                        }
                    }
                }
            }

            if(fchanged)
            {
                addticks = pvalue->add_ticks;

                /* If the entry is unchanged during lock change, remove it from hashtable */
                /* Deleting the aplist entry will delete rplist entries as well */
                if(pvalue->add_ticks == addticks)
                {
                    findex = utils_getindex(pcache->apmemaddr + pvalue->file_path, pcache->apheader->valuecount);
                    remove_aplist_entry(pcache, findex, pvalue);
                }

                /* These values should not be used as they belong to changed file */
                pvalue   = NULL;
                rpvalue  = NULL;
                resentry = 0;
            }
        }

        if(pvalue != NULL)
        {
            fullpath = alloc_estrdup(pcache->apmemaddr + pvalue->file_path);
            if(fullpath == NULL)
            {
                result = FATAL_OUT_OF_LMEMORY;
                goto Finished;
            }

            dprintverbose("stored fullpath in aplist is %s", fullpath);
        }
    }

    /* If no valid absentry is found so far, get the fullpath from php-core */
    if(pvalue == NULL)
    {
        length = strlen(filename);
        if (length > MAXPATHLEN)
        {
            result = WARNING_ORESOLVE_FAILURE;
            goto Finished;
        }

        /* Get fullpath by using copy of php_resolve_path */
        fullpath = utils_resolve_path(filename, (int)length, PG(include_path));
        if(fullpath == NULL)
        {
            result = WARNING_ORESOLVE_FAILURE;
            goto Finished;
        }

        /* Convert to lower case and change / to \\ */
        length = strlen(fullpath);
        for(findex = 0; findex < length; findex++)
        {
            if(fullpath[findex] == '/')
            {
                fullpath[findex] = '\\';
            }
        }
    }

    /* If ppvalue is NULL, just set the fullpath and return */
    /* Resolve path cache entry won't have a corresponding absentry offset */
    if(ppvalue == NULL)
    {
        *ppfullpath = fullpath;
        goto Finished;
    }

    if(pvalue == NULL)
    {
        findex = utils_getindex(fullpath, pcache->apheader->valuecount);

        _ASSERT(flock == 1);
        lock_unlock(pcache->aplock);
        flock = 0;

        result = aplist_getentry(pcache, fullpath, findex, &pvalue);
        if(FAILED(result))
        {
            goto Finished;
        }

        _ASSERT(ppvalue);
        lock_lock(pcache->aplock);
        flock = 1;
    }

    if(pvalue->fcacheval == 0)
    {
        /*
         * Unlock here, since the fcache_createval will read the entire file
         * into memory. Also, the original_stream_open_function could take a
         * long time with all the file I/O.
         */
        _ASSERT(flock == 1);
        lock_unlock(pcache->aplock);
        flock = 0;

        if(usesopen == USE_STREAM_OPEN_CHECK)
        {
            if(rpvalue == NULL || rpvalue->is_verified == VERIFICATION_NOTDONE)
            {
                /* Do a check for include_path, open_basedir validity */
                /* by calling original stream open function */
                result = original_stream_open_function(fullpath, &fhandle);

                /* Set is_verified status in rpvalue */
                if(rpvalue != NULL)
                {
                    InterlockedExchange(&rpvalue->is_verified, ((result == SUCCESS) ? VERIFICATION_PASSED : VERIFICATION_FAILED));
                }

                if(result != SUCCESS)
                {
                    result = FATAL_FCACHE_ORIGINAL_OPEN;
                    goto Finished;
                }

                zend_file_handle_dtor(&fhandle);
            }
            else
            {
                /* Fail if is_verified is set to verification failed */
                if(rpvalue->is_verified == VERIFICATION_FAILED)
                {
                    result = FATAL_FCACHE_ORIGINAL_OPEN;
                    goto Finished;
                }
            }
        }

        result = fcache_createval(pcache->pfcache, fullpath, &pfvalue);
        if(FAILED(result))
        {
            goto Finished;
        }

        _ASSERT(ppvalue);
        lock_lock(pcache->aplock);
        flock = 1;

        if(pvalue->fcacheval == 0)
        {
            pvalue->fcacheval = fcache_getoffset(pcache->pfcache, pfvalue);
            if(rpvalue != NULL)
            {
                rplist_setabsval(pcache->prplist, rpvalue, alloc_get_valueoffset(pcache->apalloc, pvalue), pvalue->resentry);
                pvalue->resentry = resentry;
            }
        }
        else
        {
            fcache_destroyval(pcache->pfcache, pfvalue);
            pfvalue = NULL;
        }
    }
    else
    {
        if(rpvalue != NULL && rpvalue->absentry == 0)
        {
            rplist_setabsval(pcache->prplist, rpvalue, alloc_get_valueoffset(pcache->apalloc, pvalue), pvalue->resentry);
#ifdef _WIN64
            InterlockedExchange64(&pvalue->resentry, resentry);
#else
            InterlockedExchange(&pvalue->resentry, resentry);
#endif
        }
    }

    if(pfvalue == NULL)
    {
        _ASSERT(pvalue->fcacheval != 0);
        pfvalue = fcache_getvalue(pcache->pfcache, pvalue->fcacheval);
        _ASSERT(pfvalue != NULL);
    }

    if(pfvalue != NULL)
    {
        /* Increment refcount while holding aplist readlock */
        fcache_refinc(pcache->pfcache, pfvalue);
    }

    /* If this is the first time this entry */
    /* got created, let original function do its job */
    *ppvalue = pfvalue;
    *ppfullpath = fullpath;

    _ASSERT(SUCCEEDED(result));

Finished:

    if(flock == 1)
    {
        lock_unlock(pcache->aplock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintverbose("failure %d in aplist_fcache_get", result);

        if(fullpath != NULL)
        {
            alloc_efree(fullpath);
            fullpath = NULL;
        }
    }

    dprintverbose("end aplist_fcache_get");

    return result;
}

int aplist_fcache_delete(aplist_context * pcache, const char * filename)
{
    int              result   = NONFATAL;
    aplist_value *   pvalue   = NULL;
    fcache_value *   pfvalue  = NULL;
    rplist_value *   rpvalue  = NULL;
    size_t           resentry = 0;
    unsigned int     findex   = 0;

    dprintverbose("start aplist_fcache_delete: (%s)", filename);

    lock_lock(pcache->aplock);

    result = rplist_getentry(pcache->prplist, filename, &rpvalue, &resentry);
    if(FAILED(result))
    {
        goto Finished;
    }

    _ASSERT(rpvalue  != NULL);
    _ASSERT(resentry != 0);

    /* If found, use new path to look into absolute path cache */
    if(rpvalue->absentry == 0)
    {
        goto Finished;
    }

    pvalue = APLIST_VALUE(pcache->apalloc, rpvalue->absentry);

    findex = utils_getindex(pcache->apmemaddr + pvalue->file_path, pcache->apheader->valuecount);
    remove_aplist_entry(pcache, findex, pvalue);

    /* These values should not be used as they belong to deleted file */
    pvalue   = NULL;
    rpvalue  = NULL;
    resentry = 0;

Finished:

    lock_unlock(pcache->aplock);

    if(FAILED(result))
    {
        dprintimportant("failure %d in aplist_fcache_delete", result);
    }

    dprintverbose("end aplist_fcache_delete");

    return result;
}

int aplist_fcache_use(aplist_context * pcache, const char * fullpath, fcache_value * pfvalue, zend_file_handle ** pphandle)
{
    int result = NONFATAL;

    dprintverbose("start aplist_fcache_use");

    _ASSERT(pfvalue  != NULL);
    _ASSERT(fullpath != NULL);
    _ASSERT(pphandle != NULL);

    result = fcache_useval(pcache->pfcache, fullpath, pfvalue, pphandle);
    if(FAILED(result))
    {
        goto Finished;
    }

    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in aplist_fcache_use", result);
    }

    dprintverbose("end aplist_fcache_use");
    return result;
}

void aplist_fcache_close(aplist_context * pcache, fcache_value * pfvalue)
{
    fcache_refdec(pcache->pfcache, pfvalue);
    return;
}

int aplist_getinfo(aplist_context * pcache, unsigned char type, zend_bool summaryonly, cache_info ** ppinfo)
{
    int                  result  = NONFATAL;
    cache_info *         pcinfo  = NULL;
    cache_entry_info *   peinfo  = NULL;
    cache_entry_info *   ptemp   = NULL;
    aplist_value *       pvalue  = NULL;

    unsigned char        flock   = 0;
    fcache_value *       pfvalue = NULL;

    unsigned int         ticks   = 0;
    unsigned int         index   = 0;
    unsigned int         count   = 0;
    size_t               offset  = 0;

    dprintverbose("start aplist_getinfo");

    _ASSERT(pcache != NULL);
    _ASSERT(ppinfo != NULL);

    *ppinfo = NULL;

    pcinfo = (cache_info *)alloc_emalloc(sizeof(cache_info));
    if(pcinfo == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    ticks = GetTickCount();

    lock_lock(pcache->aplock);
    flock = 1;

    pcinfo->initage = utils_ticksdiff(ticks, pcache->apheader->init_ticks) / 1000;
    pcinfo->islocal = pcache->islocal;

    if(type == CACHE_TYPE_FILECONTENT)
    {
        pcinfo->itemcount = pcache->pfcache->header->itemcount;
        pcinfo->hitcount  = pcache->pfcache->header->hitcount;
        pcinfo->misscount = pcache->pfcache->header->misscount;
    }
    else if(type == CACHE_TYPE_BYTECODES)
    {
        dprintimportant("Opcode Cache is no longer supported.");
        result = FATAL_OCACHE_CREATION;
        goto Finished;
    }

    pcinfo->entries = NULL;

    /* Leave count to 0 when only summary is required */
    if(!summaryonly)
    {
        count = pcache->apheader->valuecount;
    }

    for(index = 0; index < count; index++)
    {
        offset = pcache->apheader->values[index];
        if(offset == 0)
        {
            continue;
        }

        pvalue = (aplist_value *)alloc_get_cachevalue(pcache->apalloc, offset);
        while(pvalue != NULL)
        {
            if (type == CACHE_TYPE_FILECONTENT && pvalue->fcacheval != 0)
            {
                ptemp = (cache_entry_info *)alloc_emalloc(sizeof(cache_entry_info));
                if(ptemp == NULL)
                {
                    result = FATAL_OUT_OF_LMEMORY;
                    goto Finished;
                }

                _ASSERT(pvalue->file_path != 0);

                ptemp->filename = alloc_estrdup(pcache->apmemaddr + pvalue->file_path);
                ptemp->addage   = utils_ticksdiff(ticks, pvalue->add_ticks) / 1000;
                ptemp->useage   = utils_ticksdiff(ticks, pvalue->use_ticks) / 1000;

                /* If last_check value is 0, leave it as 0 */
                ptemp->lchkage = 0;
                if(pvalue->last_check != 0)
                {
                    ptemp->lchkage  = utils_ticksdiff(ticks, pvalue->last_check) / 1000;
                }

                ptemp->cdata    = NULL;
                ptemp->next     = NULL;

                if(type == CACHE_TYPE_FILECONTENT)
                {
                    pfvalue = fcache_getvalue(pcache->pfcache, pvalue->fcacheval);
                    result = fcache_getinfo(pfvalue, (fcache_entry_info **)&ptemp->cdata);
                }
                else if(type == CACHE_TYPE_BYTECODES)
                {
                    dprintimportant("Opcode Cache no longer supported.");
                    result = FATAL_OCACHE_CREATION;
                    goto Finished;
                }

                if(FAILED(result))
                {
                    goto Finished;
                }

                if(peinfo != NULL)
                {
                    peinfo->next = ptemp;
                }
                else
                {
                    pcinfo->entries = ptemp;
                }

                peinfo = ptemp;
            }

            offset = pvalue->next_value;
            if(offset == 0)
            {
                break;
            }

            pvalue = (aplist_value *)alloc_get_cachevalue(pcache->apalloc, offset);
        }
    }

    *ppinfo = pcinfo;
    _ASSERT(SUCCEEDED(result));

Finished:

    if(flock)
    {
        lock_unlock(pcache->aplock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in aplist_getinfo", result);

        if(pcinfo != NULL)
        {
            peinfo = pcinfo->entries;
            while(peinfo != NULL)
            {
                ptemp = peinfo;
                peinfo = peinfo->next;

                if(ptemp->filename != NULL)
                {
                    alloc_efree(ptemp->filename);
                    ptemp->filename = NULL;
                }

                if(ptemp->cdata != NULL)
                {
                    if(type == CACHE_TYPE_FILECONTENT)
                    {
                        fcache_freeinfo(ptemp->cdata);
                    }
                    ptemp->cdata = NULL;
                }

                alloc_efree(ptemp);
                ptemp = NULL;
            }

            alloc_efree(pcinfo);
            pcinfo = NULL;
        }
    }

    dprintverbose("start aplist_getinfo");

    return result;
}

void aplist_freeinfo(unsigned char type, cache_info * pinfo)
{
    cache_entry_info * peinfo = NULL;
    cache_entry_info * petemp = NULL;

    dprintverbose("start aplist_freeinfo");

    if(pinfo != NULL)
    {
        peinfo = pinfo->entries;
        while(peinfo != NULL)
        {
            petemp = peinfo;
            peinfo = peinfo->next;

            if(petemp->filename != NULL)
            {
                alloc_efree(petemp->filename);
                petemp->filename = NULL;
            }

            if(petemp->cdata != NULL)
            {
                if(type == CACHE_TYPE_FILECONTENT)
                {
                    fcache_freeinfo(petemp->cdata);
                }
                petemp->cdata = NULL;
            }

            alloc_efree(petemp);
            petemp = NULL;
        }

        alloc_efree(pinfo);
        pinfo = NULL;
    }

    dprintverbose("end aplist_freeinfo");
    return;
}

void aplist_runtest()
{
    int              result    = NONFATAL;
    aplist_context * pcache    = NULL;
    aplist_value *   pvalue    = NULL;
    unsigned short   islocal   = 0;
    unsigned int     filecount = WCG(numfiles);
    unsigned int     fchfreq   = 5;
    unsigned int     ttlmax    = 0;
    char *           filename  = "testfile.php";

    dprintverbose("*** STARTING APLIST TESTS ***");

    result = aplist_create(&pcache);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = aplist_initialize(pcache, APLIST_TYPE_GLOBAL, filecount, fchfreq, ttlmax);
    if(FAILED(result))
    {
        goto Finished;
    }

    _ASSERT(pcache->id          != 0);
    _ASSERT(pcache->fchangefreq == fchfreq * 1000);
    _ASSERT(pcache->apheader    != NULL);
    _ASSERT(pcache->apfilemap   != NULL);
    _ASSERT(pcache->aplock      != NULL);
    _ASSERT(pcache->apalloc     != NULL);

    _ASSERT(pcache->apheader->valuecount == WCG(numfiles));
    _ASSERT(pcache->apheader->itemcount  == (WCG(fcenabled) ? 1 : 0));

Finished:

    if(pcache != NULL)
    {
        aplist_terminate(pcache);
        aplist_destroy(pcache);

        pcache =  NULL;
    }

    dprintverbose("*** ENDING APLIST TESTS ***");

    return;
}

