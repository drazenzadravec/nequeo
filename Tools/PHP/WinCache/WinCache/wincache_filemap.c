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
   | Module: wincache_filemap.c                                                                   |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#include "precomp.h"

#define FILEMAP_INFO_HEADER_SIZE ALIGNQWORD(sizeof(filemap_information_header))
#define FILEMAP_INFO_ENTRY_SIZE  ALIGNQWORD(sizeof(filemap_information_entry))

static unsigned int getppid();
static int create_rwlock(char * lockname, lock_context ** pplock);
static void destroy_rwlock(lock_context * plock);
static int create_file_mapping(char * name, char * shmfilepath, unsigned char isfirst,size_t size, HANDLE * pshmfile, unsigned int * pexisting, HANDLE * pmap);
static void * map_viewof_file(HANDLE handle, void * baseaddr);
static int create_information_filemap(filemap_information ** ppinfo);
static void destroy_information_filemap(filemap_information * pinfo);

/* Array of filemap prefixes, in the same order & value as FILEMAP_TYPE_*
 * definitions. */
static char * g_filemap_prefix[] = {
    "WINCACHE_FILEMAP_INVALID", /* FILEMAP_TYPE_INVALID     */
    FILEMAP_FILELIST_PREFIX,    /* FILEMAP_TYPE_FILELIST    */
    FILEMAP_RESPATHS_PREFIX,    /* FILEMAP_TYPE_RESPATHS    */
    FILEMAP_FILECONTENT_PREFIX, /* FILEMAP_TYPE_FILECONTENT */
    FILEMAP_BYTECODES_PREFIX,   /* FILEMAP_TYPE_BYTECODES   */
    FILEMAP_USERZVALS_PREFIX,   /* FILEMAP_TYPE_USERZVALS   */
    FILEMAP_SESSZVALS_PREFIX,   /* FILEMAP_TYPE_SESSZVALS   */
};

#ifdef _WIN64
#define FILEMAP_PREFIX_FORMAT           "%s_%u_%u_x64"
#define FILEMAP_NAMESALT_PREFIX_FORMAT  "%s_%u_%s_%u_x64"
#else  /* not _WIN64 */
#define FILEMAP_PREFIX_FORMAT           "%s_%u_%u"
#define FILEMAP_NAMESALT_PREFIX_FORMAT  "%s_%u_%s_%u"
#endif /* _WIN64 */

/* Global information containing information */
/* about all the memory maps which got created */
unsigned short gfilemapid = 1;

/* private method to get parent process id */
static unsigned int getppid()
{
    int            result    = NONFATAL;
    unsigned int   pid       = 0;
    HANDLE         hSnapShot = INVALID_HANDLE_VALUE;
    PROCESSENTRY32 pe        = {0};
    int            poolpid   = -1;

    dprintverbose("start getppid");

    /* Parent process ID will remain constant */
    /* Just return what was calculated last time */
    if(WCG(parentpid) != 0)
    {
        goto Finished;
    }

    pid = GetCurrentProcessId();

    /* If localheap setting is set, set ppid as pid */
    if(WCG(localheap) != 0)
    {
        WCG(parentpid) = pid;
        goto Finished;
    }

    /* Use CRC of user provided apppoolid as ppid if available */
    poolpid = utils_apoolpid();
    if(poolpid != -1)
    {
        WCG(parentpid) = poolpid;
        goto Finished;
    }

    /* Get the current snapshot and look for */
    /* current process by matching PIDs */
    hSnapShot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if(hSnapShot == INVALID_HANDLE_VALUE)
    {
        error_setlasterror();
        result = FATAL_FILEMAP_CREATE_SNAPSHOT;

        goto Finished;
    }

    pe.dwSize = sizeof(PROCESSENTRY32);

    /* Go through all the entries in the snapshot */
    /* looking for current process */
    if(Process32First(hSnapShot, &pe))
    {
        do
        {
            if(pe.th32ProcessID == pid)
            {
                /* If a debugger is present, find parent to parent processId */
                if(IsDebuggerPresent())
                {
                    dprintimportant("Debugger present. Finding parent to parent");
                    pid = pe.th32ParentProcessID;

                    if(Process32First(hSnapShot, &pe))
                    {
                        do
                        {
                            if(pe.th32ProcessID == pid)
                            {
                                WCG(parentpid) = pe.th32ParentProcessID;
                                break;
                            }
                        }while(Process32Next(hSnapShot, &pe));
                    }
                }
                else
                {
                    WCG(parentpid) = pe.th32ParentProcessID;
                }

                break;
            }
        }
        while(Process32Next(hSnapShot, &pe));
    }

Finished:

    if(hSnapShot != INVALID_HANDLE_VALUE)
    {
        CloseHandle(hSnapShot);
        hSnapShot = INVALID_HANDLE_VALUE;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in getppid", result);
    }

    dprintverbose("end getppid");
    return WCG(parentpid);
}

static char * get_filemap_prefix(
    unsigned short fmaptype
    )
{
    if (fmaptype > FILEMAP_TYPE_SESSZVALS)
    {
        fmaptype = FILEMAP_TYPE_INVALID;
    }
    return g_filemap_prefix[fmaptype];
}

static int build_filemap_name(
    char * dest,
    size_t dest_size,
    char * namesalt,
    unsigned short fmaptype,
    unsigned short cachekey,
    unsigned int   pid
    )
{
    int ret;
    if (namesalt == NULL)
    {
        ret = _snprintf_s(dest, dest_size, dest_size - 1, FILEMAP_PREFIX_FORMAT, get_filemap_prefix(fmaptype), cachekey, pid);
    }
    else
    {
        ret = _snprintf_s(dest, dest_size, dest_size - 1, FILEMAP_NAMESALT_PREFIX_FORMAT, get_filemap_prefix(fmaptype), cachekey, namesalt, pid);
    }

    return ret;
}

static int create_rwlock(char * lockname, lock_context ** pplock)
{
    int            result = NONFATAL;
    lock_context * plock  = NULL;

    dprintverbose("start create_rwlock");

    _ASSERT(lockname != NULL);
    _ASSERT(pplock   != NULL);

    *pplock = NULL;

    /* Create a shared read exclusive write lock */
    result = lock_create(&plock);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = lock_initialize(plock, lockname, 1, LOCK_TYPE_SHARED, NULL);
    if(FAILED(result))
    {
        goto Finished;
    }

    *pplock = plock;

    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in create_rwlock", result);

        if(plock != NULL)
        {
            lock_terminate(plock);
            lock_destroy(plock);

            plock = NULL;
        }
    }

    dprintverbose("end create_rwlock");

    return result;
}

static void destroy_rwlock(lock_context * plock)
{
    dprintverbose("start destroy_rwlock");

    if(plock != NULL)
    {
        lock_terminate(plock);
        lock_destroy(plock);

        plock = NULL;
    }

    dprintverbose("end destroy_rwlock");

    return;
}

static int create_file_mapping(
    char * name,
    char * shmfilepath,
    unsigned char isfirst,
    size_t size,
    HANDLE * pshmfile,
    unsigned int * pexisting,
    HANDLE * pmap
    )
{
    int             result     = NONFATAL;
    HANDLE          filehandle = INVALID_HANDLE_VALUE;
    HANDLE          maphandle  = NULL;
    unsigned int    isexisting = 0;
    unsigned int    attributes = FILE_ATTRIBUTE_NORMAL | FILE_FLAG_RANDOM_ACCESS;
    unsigned int    sharemode  = FILE_SHARE_READ | FILE_SHARE_WRITE;
    unsigned int    access     = GENERIC_READ | GENERIC_WRITE;
    unsigned char   globalName[MAX_PATH+1];
    ULARGE_INTEGER  li         = { 0 };

    dprintverbose("start create_file_mapping");

    _ASSERT(name != NULL);
    _ASSERT(size >  0);
    _ASSERT(pmap != NULL);

    /* If a shmfilepath is passed, map the file pointed by path */
    if(shmfilepath != NULL)
    {
        _ASSERT(pshmfile     != NULL);
        _ASSERT(pexisting    != NULL);
        _ASSERT(*shmfilepath != '\0');

        if (isfirst)
        {
            /* Delete the file, since we're creating it for the first time */
            (void)DeleteFile(shmfilepath);
        }

        /* Create a new file or open existing */
        filehandle = CreateFile(shmfilepath, access, sharemode, NULL, OPEN_ALWAYS, attributes, NULL);
        if(filehandle == INVALID_HANDLE_VALUE)
        {
            error_setlasterror();
            result = FATAL_FILEMAP_CREATEFILE;

            goto Finished;
        }

        /* If file already exists, mark existing so that initialization is skipped */
        if(GetLastError() == ERROR_ALREADY_EXISTS)
        {
            isexisting = 1;

            /* TBD?? Check file size and error out if its greater than size */
            /* TBD?? If file never got initialized properly, mark isexisting = 0 */
            /* TBD?? Detect memory corruption. Mark isexisting = 0 */
        }
        else
        {
            int aclRet = utils_set_apppool_acl(shmfilepath);

            if ( FAILED(aclRet) )
            {
                /* TODO: If the ACL'ing fails, we should close the file and fall */
                /* back to using the system page file. */
                CloseHandle(filehandle);
                filehandle = INVALID_HANDLE_VALUE;

                dprintimportant( "create_file_mapping[%d]: failed to set acl on %s (%d).",
                                 GetCurrentProcessId(),
                                 shmfilepath,
                                 aclRet);
            }
        }
    }

    if (WCG(apppoolid))
    {
        /* prefix the name with "Global\", to ensure the named filemap is in the global space. */
        if ( -1 == sprintf_s(globalName, MAX_PATH+1, GLOBAL_SCOPE_PREFIX "%s", name) )
        {
            result = FATAL_FILEMAP_CREATEFILEMAP;
            goto Finished;
        }
        name = globalName;
    }

    /* Call CreateFileMapping to create new or open existing file mapping object */
    li.QuadPart = (ULONGLONG)size; /* safely handle size_t on x64 */
    maphandle = CreateFileMapping(filehandle, NULL, PAGE_READWRITE, li.HighPart, li.LowPart, name);

    /* handle value null means a fatal error */
    if(maphandle == NULL)
    {
        error_setlasterror();
        result = FATAL_FILEMAP_CREATEFILEMAP;

        goto Finished;
    }

    if(shmfilepath != NULL)
    {
        _ASSERT(filehandle != INVALID_HANDLE_VALUE);

        *pshmfile  = filehandle;
        filehandle = INVALID_HANDLE_VALUE;
        *pexisting = isexisting;
    }

    *pmap = maphandle;
    maphandle = NULL;

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in create_file_mapping", result);

        if(maphandle != NULL)
        {
            CloseHandle(maphandle);
            maphandle = NULL;
        }

        if(filehandle != INVALID_HANDLE_VALUE)
        {
            CloseHandle(filehandle);
            filehandle = INVALID_HANDLE_VALUE;
        }
    }

    dprintverbose("end create_file_mapping");

    return result;
}

static void * map_viewof_file(HANDLE handle, void * baseaddr)
{
    void * retaddr = NULL;

    if(baseaddr != NULL)
    {
        dprintverbose("Mapping file map at address = %p", baseaddr);
    }
    else
    {
        dprintverbose("Mapping file map at any random address");
    }

    retaddr = MapViewOfFileEx(handle, FILE_MAP_ALL_ACCESS, 0, 0, 0, baseaddr);
    if(retaddr == NULL)
    {
        error_setlasterror();
        goto Finished;
    }

    _ASSERT(retaddr != NULL);

Finished:

    return retaddr;
}

inline size_t ALIGN_UP( size_t size, size_t page_size )
{
    size_t pad = size % page_size;

    if (pad)
    {
        size += (page_size - pad);
    }
    return size;
}

/*++

Routine Description:

    Private method to find an open chunk of virtual memory, large enough to
    accomodate an allocation of the requested size.

Arguments:

    size - requested allocation size.

Return Value:

    pointer to base address which should accomodate an allocation of the
    requested size.

 --*/
void * get_free_vm_base_address(size_t size)
{
    SYSTEM_INFO  SystemInfo;
    MEMORY_BASIC_INFORMATION MemInfo;
    unsigned char fDone = FALSE;
    size_t cbUpSize;
    size_t cbRet;
    unsigned char *pBaseTemp;

    GetSystemInfo( &SystemInfo );

    /* Round up to a page-size aligned allocation */
    cbUpSize = ALIGN_UP( size, SystemInfo.dwPageSize );

    /* Pick the first candidate base address */
    pBaseTemp = ((unsigned char *)SystemInfo.lpMaximumApplicationAddress - cbUpSize);
    pBaseTemp++; /* address should now be page-size aligned */

    while (!fDone)
    {
        cbRet = VirtualQuery( pBaseTemp, &MemInfo, sizeof(MEMORY_BASIC_INFORMATION) );
        if (!cbRet)
        {
            pBaseTemp = NULL;
            goto Finished;
        }

        if (MemInfo.State == MEM_FREE)
        {
            if (MemInfo.RegionSize >= cbUpSize)
            {
                fDone = TRUE;
            }
            else
            {
                pBaseTemp -= ALIGN_UP(cbUpSize - MemInfo.RegionSize, SystemInfo.dwPageSize);
            }
        }
        else
        {
            pBaseTemp -= cbUpSize;
        }

        /* Did we run out of candidates? */
        if (pBaseTemp <= (unsigned char *)SystemInfo.lpMinimumApplicationAddress)
        {
            pBaseTemp = NULL;
            fDone = TRUE;
        }
    }

Finished:
    return pBaseTemp;
}

static int create_information_filemap(filemap_information ** ppinfo)
{
    int                         result      = NONFATAL;
    int                         index       = 0;
    int                         infonamelen = 0;
    size_t                      size        = 0;
    size_t                      namelen     = 0;
    filemap_information *       pinfo       = NULL;
    filemap_information_entry * pentry      = NULL;
    unsigned char               isfirst     = 1;
    unsigned char               islocked    = 0;
    unsigned int                isexisting  = 0;
    DWORD                       ret         = 0;
    char *                      scopePrefix = "";
    char *                      sectionName = NULL;

    dprintverbose("start create_information_filemap");

    _ASSERT(ppinfo != NULL);
    *ppinfo = NULL;

    /* Allocate memory for filemap_information */
    pinfo = (filemap_information *)alloc_pemalloc(sizeof(filemap_information));
    if(pinfo == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    /* Initialize structure values */
    pinfo->hinfomap  = NULL;
    pinfo->infoname  = NULL;
    pinfo->infonlen  = 0;
    pinfo->header    = NULL;
    pinfo->hinitdone = NULL;
    pinfo->hlock     = NULL;

    /* First thing to do is create the lock */
    /* As the lock is xread_xwrite, doing this before mapping is fine */
    result = create_rwlock("FILEMAP_INFO_HRWLOCK", &pinfo->hlock);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* Use PPID in the information filemap so that processes under */
    /* one w3wp can share filemap information. Add PID and 2 more for */
    /* underscore and terminating NULL */
    namelen = FILEMAP_INFORMATION_PREFIX_LENGTH + PID_MAX_LENGTH + 2;

    /* If a name salt is specified, use _<namesalt> in name */
    if(WCG(namesalt) != NULL)
    {
        namelen += strlen(WCG(namesalt)) + 1;
    }

    /* If we're on an app pool, we need to create all named objects in */
    /* the Global scope. */
    if (WCG(apppoolid))
    {
        scopePrefix = GLOBAL_SCOPE_PREFIX;
        namelen += GLOBAL_SCOPE_PREFIX_LEN;
    }

    /* Allocate memory to keep name of the information filemap */
    pinfo->infoname = (char *)alloc_pemalloc(namelen);
    if(pinfo->infoname == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    ZeroMemory(pinfo->infoname, namelen);

    /* Create name as FILE_INFORMATION_PREFIX_<ppid> */
    if(WCG(namesalt) == NULL)
    {
        infonamelen = _snprintf_s(pinfo->infoname, namelen, namelen - 1, "%s%s_%u", scopePrefix, FILEMAP_INFORMATION_PREFIX, WCG(fmapgdata)->ppid);
    }
    else
    {
        infonamelen = _snprintf_s(pinfo->infoname, namelen, namelen - 1, "%s%s_%s_%u", scopePrefix, FILEMAP_INFORMATION_PREFIX, WCG(namesalt), WCG(fmapgdata)->ppid);
    }

    if (infonamelen < 0)
    {
        result = FATAL_INVALID_DATA;
        goto Finished;
    }

    pinfo->infonlen = infonamelen;

    result = utils_create_init_event(pinfo->infoname, "_FCACHE_INIT", &pinfo->hinitdone, &isfirst);
    if (FAILED(result))
    {
        result = FATAL_FCACHE_INIT_EVENT;
        goto Finished;
    }

    islocked = 1;

    /* Calculate size and try to get the filemap handle */
    /* Adding two aligned qwords sizes will produce qword */
    size = FILEMAP_INFO_HEADER_SIZE + (FILEMAP_MAX_COUNT * FILEMAP_INFO_ENTRY_SIZE);

    if (WCG(apppoolid))
    {
        /* NOTE: We need to pass the un-Global'd prefixed name to create_file_mapping. */
        sectionName = &pinfo->infoname[GLOBAL_SCOPE_PREFIX_LEN];
    }
    else
    {
        sectionName = pinfo->infoname;
    }

    /* shmfilepath = NULL, pfilehandle = NULL, pexisting = NULL */
    result = create_file_mapping(sectionName, NULL, isfirst, size, NULL, &isexisting, &pinfo->hinfomap);
    if(FAILED(result))
    {
        goto Finished;
    }

    if (!isfirst && isexisting)
    {
        dprintimportant("create_information_filemap[%d]: Warning: We thought we were first, but we found an existing file.\n", WCG(fmapgdata)->pid);
    }

    /* We have the handle to information file mapping object */
    /* Map file mapping object in this process's virtual memory */
    pinfo->header = (filemap_information_header *)map_viewof_file(pinfo->hinfomap, NULL);
    if(pinfo->header == NULL)
    {
        result = FATAL_FILEMAP_INFOMAP;
        goto Finished;
    }

    /* Initialize if its not already initialized */
    if(isfirst)
    {
        lock_lock(pinfo->hlock);

        /* This is the first process which got the pointer */
        /* to information filemap. This should initialize header. */
        /* Other processes will be blocked trying to get writelock */
        pinfo->header->size        = size;
        pinfo->header->mapcount    = 1;
        pinfo->header->maxcount    = FILEMAP_MAX_COUNT;
        pinfo->header->entry_count = 0;

        /* Initialize all entries to type UNUSED */
        pentry = (filemap_information_entry *)((char *)pinfo->header + FILEMAP_INFO_HEADER_SIZE);
        for(index = 0; index < pinfo->header->maxcount; index++)
        {
            pentry->fmaptype = FILEMAP_TYPE_UNUSED;
            pentry->cachekey = 0;
            pentry = (filemap_information_entry *)((char *)pentry + FILEMAP_INFO_ENTRY_SIZE);
        }

        ReleaseMutex(pinfo->hinitdone);
        islocked = 0;

        lock_unlock(pinfo->hlock);
    }
    else
    {
        /* Increment the number of times information filemap is mapped */
        InterlockedIncrement(&pinfo->header->mapcount);
    }

    *ppinfo = pinfo;
    _ASSERT(SUCCEEDED(result));

Finished:

    if(islocked)
    {
        ReleaseMutex(pinfo->hinitdone);
        islocked = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in create_information_filemap", result);

        if(pinfo != NULL)
        {
            if(pinfo->header != NULL)
            {
                UnmapViewOfFile((void *)pinfo->header);
                pinfo->header = NULL;
            }

            if(pinfo->hinfomap != NULL)
            {
                CloseHandle(pinfo->hinfomap);
                pinfo->hinfomap = NULL;
            }

            if(pinfo->hlock != NULL)
            {
                lock_terminate(pinfo->hlock);
                lock_destroy(pinfo->hlock);

                pinfo->hlock = NULL;
            }

            if(pinfo->infoname != NULL)
            {
                alloc_pefree(pinfo->infoname);
                pinfo->infoname = NULL;

                pinfo->infonlen = 0;
            }

            if(pinfo->hinitdone != NULL)
            {
                CloseHandle(pinfo->hinitdone);
                pinfo->hinitdone = NULL;
            }

            alloc_pefree(pinfo);
            pinfo = NULL;
        }
    }

    dprintverbose("end create_information_filemap");

    return result;
}

/* destroy the information filemap structure we created */
static void destroy_information_filemap(filemap_information * pinfo)
{
    dprintverbose("start destroy_information_filemap");

    if(pinfo != NULL)
    {
        if(pinfo->header != NULL)
        {
            InterlockedDecrement(&pinfo->header->mapcount);
            UnmapViewOfFile((void *)pinfo->header);

            pinfo->header = NULL;
        }

        if(pinfo->hinfomap != NULL)
        {
            CloseHandle(pinfo->hinfomap);
            pinfo->hinfomap = NULL;
        }

        if(pinfo->hlock != NULL)
        {
            lock_terminate(pinfo->hlock);
            lock_destroy(pinfo->hlock);

            pinfo->hlock = NULL;
        }

        if(pinfo->infoname != NULL)
        {
            alloc_pefree(pinfo->infoname);
            pinfo->infoname = NULL;

            pinfo->infonlen = 0;
        }

        if(pinfo->hinitdone != NULL)
        {
            CloseHandle(pinfo->hinitdone);
            pinfo->hinitdone = NULL;
        }

        alloc_pefree(pinfo);
        pinfo = NULL;
    }

    dprintverbose("end destroy_information_filemap");

    return;
}

/* Global initializer which should be called once per process */
int filemap_global_initialize()
{
    int                      result    = NONFATAL;
    filemap_global_context * fgcontext = NULL;

    dprintverbose("start filemap_global_initialize");

    /* If global_initialize has already been called, just return */
    if(WCG(fmapgdata) != NULL)
    {
        goto Finished;
    }

    /* allocate persistent memory for fgcontext */
    fgcontext = (filemap_global_context *)alloc_pemalloc(sizeof(filemap_global_context));
    if(fgcontext == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    /* Set default values of structure members */
    fgcontext->pid  = GetCurrentProcessId();
    fgcontext->ppid = getppid();
    fgcontext->info = NULL;

    /* Set global as soon as pid and ppid are set */
    WCG(fmapgdata) = fgcontext;

    result = create_information_filemap(&fgcontext->info);
    if(FAILED(result))
    {
        goto Finished;
    }

    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in filemap_global_initialize", result);

        if(fgcontext != NULL)
        {
            WCG(fmapgdata) = NULL;
            if(fgcontext->info != NULL)
            {
                destroy_information_filemap(fgcontext->info);
                fgcontext->info = NULL;
            }

            alloc_pefree(fgcontext);
            fgcontext = NULL;
        }
    }

    dprintverbose("end filemap_global_initialize");

    return result;
}

/* Terminate global information including information filemap */
void filemap_global_terminate()
{
    dprintverbose("start filemap_global_terminate");

    if(WCG(fmapgdata) != NULL)
    {
        if(WCG(fmapgdata)->info != NULL)
        {
            destroy_information_filemap(WCG(fmapgdata)->info);
            WCG(fmapgdata)->info = NULL;
        }

        alloc_pefree(WCG(fmapgdata));
        WCG(fmapgdata) = NULL;
    }

    dprintverbose("end filemap_global_terminate");

    return;
}

/* API to get current process ID */
unsigned int filemap_getpid()
{
    _ASSERT(WCG(fmapgdata) != NULL);
    return WCG(fmapgdata)->pid;
}

/* API tp get the parent process ID */
/* Use parent process identifier to create */
/* separate caches for processes under a process */
unsigned int filemap_getppid()
{
    _ASSERT(WCG(fmapgdata) != NULL);
    return WCG(fmapgdata)->ppid;
}

/* create new filemap context */
int filemap_create(filemap_context ** ppfilemap)
{
    int               result   = NONFATAL;
    filemap_context * pfilemap = NULL;

    dprintverbose("start filemap_create");

    _ASSERT(ppfilemap != NULL);
    *ppfilemap = NULL;

    pfilemap = (filemap_context *)alloc_pemalloc(sizeof(filemap_context));
    if(pfilemap == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    pfilemap->id        = gfilemapid++;
    pfilemap->islocal   = 0;
    pfilemap->infoentry = NULL;
    pfilemap->hfilemap  = NULL;
    pfilemap->hshmfile  = INVALID_HANDLE_VALUE;
    pfilemap->existing  = 0;
    pfilemap->mapaddr   = NULL;

    *ppfilemap = pfilemap;

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in filemap_create", result);
    }

    dprintverbose("end filemap_create");

    return result;
}

void filemap_destroy(filemap_context * pfilemap)
{
    dprintverbose("start filemap_destroy");

    if(pfilemap != NULL)
    {
        alloc_pefree(pfilemap);
        pfilemap = NULL;
    }

    dprintverbose("end filemap_destroy");

    return;
}

int filemap_initialize(filemap_context * pfilemap, unsigned short fmaptype, unsigned short cachekey, unsigned short fmclass, unsigned int size_mb, unsigned char isfirst, char * shmfilepath)
{
    int           result  = NONFATAL;
    unsigned int  ffree   = 0;
    unsigned int  size    = 0;
    unsigned int  index   = 0;
    unsigned int  found   = 0;
    void *        mapaddr = NULL;
    unsigned char flock   = 0;
    unsigned int  fcreate_file_for_sm = 1;
    char *        sm_file_path        = shmfilepath;
    HANDLE        hOriginalToken      = NULL;

    filemap_information *        pinfo  = NULL;
    filemap_information_header * pinfoh = NULL;
    filemap_information_entry *  pentry = NULL;

    dprintverbose("start filemap_initialize");

    _ASSERT(WCG(fmapgdata) != NULL);
    _ASSERT(cachekey       != 0);
    _ASSERT(pfilemap       != NULL);
    _ASSERT(size_mb        >  0);

    size = size_mb * 1024 * 1024;

    /* If parentpid is greater than 99999, use shmfilepath */
    /* Else don't create file backed shared memory */
    if(WCG(fmapgdata)->ppid <= 99999)
    {
        sm_file_path = NULL;
        fcreate_file_for_sm = 0;
    }

    /* See if this is already there in the list of filemaps */
    /* If not create a new filemap and add to the list */
    pinfo = WCG(fmapgdata)->info;
    pinfoh = pinfo->header;

    _ASSERT(pinfoh != NULL);

    if(fmclass != FILEMAP_MAP_LRANDOM)
    {
        /* Check if fmaptype is already there in the list of filemaps available */
        lock_lock(pinfo->hlock);
        flock = 1;

        pentry = (filemap_information_entry *)((char *)pinfoh + FILEMAP_INFO_HEADER_SIZE);
        found = 0;

        for(index = 0; index < pinfoh->maxcount; index++)
        {
            if(pentry->fmaptype == FILEMAP_TYPE_UNUSED && ffree == 0)
            {
                ffree = index;
            }

            if(pentry->fmaptype == fmaptype && pentry->cachekey == cachekey)
            {
                found = 1;
                break;
            }

            pentry = (filemap_information_entry *)((char *)pentry + FILEMAP_INFO_ENTRY_SIZE);
        }

        _ASSERT(fmclass == FILEMAP_MAP_SRANDOM || fmclass == FILEMAP_MAP_SFIXED);
        if(found == 1)
        {
            /* Another process has already this filemap created */
            /* Bump the mapcount and try to map at the same address */
            /* if caller wants to create a fixed filemap */
            if(fmclass == FILEMAP_MAP_SFIXED)
            {
                mapaddr = pentry->mapaddr;
            }
        }
        else
        {
            /* If no free slots found, throw nofree error */
            if(ffree == 0)
            {
                result = FATAL_FILEMAP_NOFREE;
                goto Finished;
            }

            /* Get the pointer to first free entry in the table */
            pentry = (filemap_information_entry *)((char *)pinfoh + FILEMAP_INFO_HEADER_SIZE);
            pentry = (filemap_information_entry *)((char *)pentry + (ffree * FILEMAP_INFO_ENTRY_SIZE));

            pentry->fmaptype = fmaptype;
            pentry->cachekey = cachekey;

            /* Create name with ppid in it */
            ZeroMemory(pentry->name, MAX_PATH);

            build_filemap_name(pentry->name, MAX_PATH, WCG(namesalt), pentry->fmaptype, cachekey, WCG(fmapgdata)->ppid);

            dprintverbose("Creating a shared filemap with name %s", pentry->name);

            pentry->size     = size;
            pentry->mapcount = 0;
            pentry->cpid     = filemap_getpid();
            pentry->opid     = filemap_getpid();
            pentry->mapaddr  = NULL;

            if (fmclass == FILEMAP_MAP_SFIXED)
            {
                mapaddr = get_free_vm_base_address(size);
            }
        }

        /* If we should have a temp file for shared memory, but we don't have a
         * file name yet, build one. */
        if (fcreate_file_for_sm && sm_file_path == NULL)
        {
            sm_file_path = utils_build_temp_filename(pentry->name);
        }

        /*
         * RevertToSelf if needed
         */
        result = utils_revert_if_necessary(&hOriginalToken);
        if (FAILED(result))
        {
            goto Finished;
        }

        result = create_file_mapping(
                    pentry->name,
                    sm_file_path,
                    /* Session files shouldn't get deleted */
                    (fmaptype == FILEMAP_TYPE_SESSZVALS) ? 0 : isfirst,
                    pentry->size,
                    &pfilemap->hshmfile,
                    &pfilemap->existing,
                    &pfilemap->hfilemap);
        if(FAILED(result))
        {
            /* OK to goto Finished. mapcount is not incremented yet */
            goto Finished;
        }

        /* We will map to the same address. If that fails, */
        /* we will map to address of OS choice */
        pfilemap->mapaddr = map_viewof_file(pfilemap->hfilemap, mapaddr);
        if(pfilemap->mapaddr == NULL)
        {
            if(fmclass == FILEMAP_MAP_SFIXED)
            {
                /* Error trying to map at a particular address is a warning to caller */
                result = WARNING_FILEMAP_MAPVIEW;
            }
            else
            {
                /* Error mapping at a random address is fatal */
                result = FATAL_FILEMAP_MAPVIEW;
            }

            goto Finished;
        }
    }
    else
    {
        /* Couldn't map shared memory at the specified address */
        /* Map at a random address and indicate that its not shared */
        /* Change name to use pid instead of PPID */

        /* Allocate memory for the information entry */
        pentry = alloc_pemalloc(sizeof(filemap_information_entry));
        if(pentry == NULL)
        {
            result = FATAL_OUT_OF_LMEMORY;
            goto Finished;
        }

        pentry->fmaptype = fmaptype;
        pentry->cachekey = cachekey;

        /* Create name with ppid in it */
        ZeroMemory(pentry->name, MAX_PATH);

        build_filemap_name(pentry->name, MAX_PATH, WCG(namesalt), pentry->fmaptype, cachekey, WCG(fmapgdata)->pid);

        dprintimportant("Creating a local filemap with name %s", pentry->name);

        pentry->size     = size;
        pentry->mapcount = 0;
        pentry->cpid     = filemap_getpid();
        pentry->opid     = filemap_getpid();
        pentry->mapaddr  = NULL;

        /* This filemap is local only */
        pfilemap->islocal = 1;

        pfilemap->mapaddr  = alloc_pemalloc(size);
        if (pfilemap->mapaddr == NULL)
        {
            result = FATAL_FILEMAP_ALLOC_LOCALFILEMAP;
            goto Finished;
        }
    }

    pfilemap->infoentry = pentry;

    /* All went fine. Increment entry_count for debugging */
    /* purposes and keep mapaddr of creator */
    if(found == 0)
    {
        pentry->mapaddr = pfilemap->mapaddr;
        if(pfilemap->islocal == 0)
        {
            _ASSERT(pinfoh != NULL);
            pinfoh->entry_count++;
        }
    }

    pentry->mapcount++;

Finished:

    utils_reimpersonate_if_necessary(hOriginalToken);

    if(flock)
    {
        if(FAILED(result))
        {
            /* If we alloc'd a filemap_information_entry in the shared map,
            /* we MUST clean it up while still under the lock! */
            if (fmclass != FILEMAP_MAP_LRANDOM && found == 0 && ffree != 0)
            {
                _ASSERT(pentry);
                pentry->fmaptype = FILEMAP_TYPE_UNUSED;
            }
        }

        lock_unlock(pinfo->hlock);
        flock = 0;
    }

    if (sm_file_path && sm_file_path != shmfilepath)
    {
        alloc_pefree(sm_file_path);
        sm_file_path = NULL;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in filemap_initialize", result);

        /* mapcount and entry_count are updated in the last */
        /* so no need to worry about that getting messed up */
        if(pfilemap->mapaddr != NULL)
        {
            if (pfilemap->islocal)
            {
                alloc_pefree(pfilemap->mapaddr);
            }
            else
            {
                UnmapViewOfFile(pfilemap->mapaddr);
            }
            pfilemap->mapaddr = NULL;
        }

        if(pfilemap->hshmfile != INVALID_HANDLE_VALUE)
        {
            CloseHandle(pfilemap->hshmfile);
            pfilemap->hshmfile = INVALID_HANDLE_VALUE;
        }

        if(pfilemap->hfilemap != NULL)
        {
            CloseHandle(pfilemap->hfilemap);
            pfilemap->hfilemap = NULL;
        }
    }

    dprintverbose("end filemap_initialize");

    return result;
}

void filemap_terminate(filemap_context * pfilemap)
{
    char * sm_file_path = NULL;
    unsigned short fmaptype = FILEMAP_TYPE_UNUSED;

    dprintverbose("start filemap_terminate");

    if(pfilemap != NULL)
    {
        if(pfilemap->infoentry != NULL)
        {
            if(!pfilemap->islocal)
            {
                lock_lock(WCG(fmapgdata)->info->hlock);

                /* Decrement the mapcount */
                pfilemap->infoentry->mapcount--;

                /* If all the processes unmapped the filemap,  */
                /* remove the entry from information list */
                if(pfilemap->infoentry->mapcount == 0)
                {
                    fmaptype = pfilemap->infoentry->fmaptype;
                    pfilemap->infoentry->fmaptype = FILEMAP_TYPE_UNUSED;
                    pfilemap->infoentry->cachekey = 0;
                    WCG(fmapgdata)->info->header->entry_count--;

                    /* Get the name of the backing file, so we can delete it
                     * after we close the handle.
                     * Oh, and DO NOT delete session files! */
                    if (pfilemap->hshmfile != INVALID_HANDLE_VALUE &&
                        fmaptype != FILEMAP_TYPE_SESSZVALS)
                    {
                        sm_file_path = utils_build_temp_filename(pfilemap->infoentry->name);
                    }
                }

                lock_unlock(WCG(fmapgdata)->info->hlock);
            }
            else
            {
                alloc_pefree(pfilemap->infoentry);
                pfilemap->infoentry = NULL;
            }
        }

        if(pfilemap->mapaddr != NULL)
        {
            if (pfilemap->islocal)
            {
                alloc_pefree(pfilemap->mapaddr);
            }
            else
            {
                UnmapViewOfFile(pfilemap->mapaddr);
            }
            pfilemap->mapaddr = NULL;
        }

        if(pfilemap->hfilemap != NULL)
        {
            CloseHandle(pfilemap->hfilemap);
            pfilemap->hfilemap = NULL;
        }

        if(pfilemap->hshmfile != INVALID_HANDLE_VALUE)
        {
            CloseHandle(pfilemap->hshmfile);
            pfilemap->hshmfile = INVALID_HANDLE_VALUE;

            /* if we are the last one out, delete the file */
            if (sm_file_path)
            {
                (void)DeleteFile(sm_file_path);
                alloc_pefree(sm_file_path);
                sm_file_path = NULL;
            }
        }
    }

    dprintverbose("end filemap_terminate");
    return;
}

size_t filemap_getsize(filemap_context * pfilemap)
{
    size_t size = 0;

    dprintverbose("start filemap_getsize");

    _ASSERT(pfilemap != NULL);

    lock_lock(WCG(fmapgdata)->info->hlock);

    size = pfilemap->infoentry->size;

    lock_unlock(WCG(fmapgdata)->info->hlock);

    dprintverbose("end filemap_getsize");
    return size;
}

unsigned int filemap_getcpid(filemap_context * pfilemap)
{
    unsigned int cpid = 0;

    dprintverbose("start filemap_getcpid");

    _ASSERT(pfilemap != NULL);

    lock_lock(WCG(fmapgdata)->info->hlock);

    cpid = pfilemap->infoentry->cpid;

    lock_unlock(WCG(fmapgdata)->info->hlock);

    dprintverbose("end filemap_getcpid");

    return cpid;
}

void filemap_runtest()
{
    int                          result      = NONFATAL;
    unsigned int                 index       = 0;
    int                          initialized = 0;

    filemap_context *            pfilemap1   = NULL;
    filemap_context *            pfilemap2   = NULL;

    filemap_information *        pinfo       = NULL;
    filemap_information_header * pinfoh      = NULL;
    filemap_information_entry *  pentry      = NULL;

    unsigned int                 orig_mapcount    = 0;
    unsigned short               orig_entry_count = 0;

    dprintverbose("*** STARTING FILEMAP TESTS ***");

    if(WCG(fmapgdata) == NULL)
    {
       result = filemap_global_initialize();
       if(FAILED(result))
       {
           dprintverbose("filemap_global_initialize failed");
           goto Finished;
       }

       initialized = 1;
    }

    _ASSERT(WCG(fmapgdata) != NULL);

    /* Verify filemap_information_header setup correctly */
    _ASSERT(WCG(fmapgdata)->pid  != 0);
    _ASSERT(WCG(fmapgdata)->ppid != 0);

    pinfo = WCG(fmapgdata)->info;

    _ASSERT(pinfo->hinfomap != NULL);
    _ASSERT(pinfo->infonlen == strlen(pinfo->infoname));
    _ASSERT(pinfo->header   != NULL);
    _ASSERT(pinfo->hlock    != NULL);

    pinfoh = pinfo->header;
    _ASSERT(pinfoh != NULL);

    _ASSERT(pinfoh->size        != 0);
    orig_mapcount               = pinfoh->mapcount;
    orig_entry_count            = pinfoh->entry_count;
    _ASSERT(pinfoh->maxcount    >  0);

    /* Create two filemap_context. Verify filemap contexts, */
    /* filemap info entries and info header structures */
    result = filemap_create(&pfilemap1);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = filemap_initialize(pfilemap1, FILEMAP_TYPE_FILECONTENT, 58, FILEMAP_MAP_SRANDOM, 20, TRUE, NULL);
    if(FAILED(result))
    {
        goto Finished;
    }

    _ASSERT(pfilemap1->hfilemap != NULL);
    _ASSERT(pfilemap1->mapaddr  != NULL);
    _ASSERT(pfilemap1->islocal  == 0);

    pentry = pfilemap1->infoentry;

    _ASSERT(pentry->fmaptype == FILEMAP_TYPE_FILECONTENT);
    _ASSERT(pentry->cachekey == 58);
    _ASSERT(pentry->size     == 20 * 1024 * 1024);
    _ASSERT(pentry->mapcount == 1);
    _ASSERT(pentry->cpid     == filemap_getpid());
    _ASSERT(pentry->opid     == filemap_getpid());
    _ASSERT(pentry->mapaddr  != NULL);

    _ASSERT(pinfoh->mapcount    == orig_mapcount);
    _ASSERT(pinfoh->entry_count == orig_entry_count + 1);

    result = filemap_create(&pfilemap2);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = filemap_initialize(pfilemap2, FILEMAP_TYPE_BYTECODES, 59, FILEMAP_MAP_SFIXED, 10, TRUE, NULL);
    if(FAILED(result))
    {
        goto Finished;
    }

    _ASSERT(pfilemap2->hfilemap != NULL);
    _ASSERT(pfilemap2->mapaddr  != NULL);
    _ASSERT(pfilemap2->islocal  == 0);

    pentry = pfilemap2->infoentry;

    _ASSERT(pentry->fmaptype == FILEMAP_TYPE_BYTECODES);
    _ASSERT(pentry->cachekey == 59);
    _ASSERT(pentry->size     == 10 * 1024 * 1024);
    _ASSERT(pentry->mapcount == 1);
    _ASSERT(pentry->cpid     == filemap_getpid());
    _ASSERT(pentry->opid     == filemap_getpid());
    _ASSERT(pentry->mapaddr  != NULL);

    _ASSERT(pinfoh->mapcount    == orig_mapcount);
    _ASSERT(pinfoh->entry_count == orig_entry_count + 2);

    /* Delete one of the filemap context and verify everything */
    filemap_terminate(pfilemap1);
    filemap_destroy(pfilemap1);

    pfilemap1 = NULL;

    _ASSERT(pinfoh->mapcount    == orig_mapcount);
    _ASSERT(pinfoh->entry_count == orig_entry_count + 1);

    /* Create filemap context for the second filemap again */
    /* Delete second filemap context and verify everything */
    result = filemap_create(&pfilemap1);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = filemap_initialize(pfilemap1, FILEMAP_TYPE_BYTECODES, 58, FILEMAP_MAP_SFIXED, 10, TRUE, NULL);
    if(FAILED(result))
    {
        goto Finished;
    }

    _ASSERT(SUCCEEDED(result));

Finished:

    if(pfilemap1 != NULL)
    {
        filemap_terminate(pfilemap1);
        filemap_destroy(pfilemap1);

        pfilemap1 = NULL;
    }

    if(pfilemap2 != NULL)
    {
        filemap_terminate(pfilemap2);
        filemap_destroy(pfilemap2);

        pfilemap2 = NULL;
    }

    /* Validate the two entries we just nuked are actually unused */
    pentry = (filemap_information_entry *)(((char *)pinfoh) + FILEMAP_INFO_HEADER_SIZE);
    /* Advance past the already in-use entries */
    pentry = (filemap_information_entry *)(((char *)pentry) + ((orig_entry_count + 1) * FILEMAP_INFO_ENTRY_SIZE));
    for(index = orig_entry_count+1; index < pinfoh->maxcount; index++)
    {
        _ASSERT(pentry->fmaptype == FILEMAP_TYPE_UNUSED);
        pentry = (filemap_information_entry *)((char *)pentry + FILEMAP_INFO_ENTRY_SIZE);
    }

    if(initialized == 1)
    {
        filemap_global_terminate();
    }

    dprintverbose("*** ENDING FILEMAP TESTS ***");

    return;
}

