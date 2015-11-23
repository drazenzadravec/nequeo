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
   | Module: wincache_fcache.c                                                                    |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#include "precomp.h"

#define READAHEAD_MAXIMUM 0x10000000

static int read_file_security(fcache_context * pfcache, const char * filename, unsigned char ** ppsec);
static int read_file_content(HANDLE hFile, unsigned int filesize, void ** ppbuffer);

/* internal helper functions */
void fcache_init_php_handle(fcache_handle *fhandle);

/* Globals */
unsigned short gfcacheid = 1;

/* Private methods */
static int read_file_security(fcache_context * pfcache, const char * filename, unsigned char ** ppsec)
{
    int                  result      = NONFATAL;
    SECURITY_INFORMATION sec_info    = 0;
    unsigned int         desc_length = 0;
    unsigned char *      psec_desc   = NULL;

    dprintdecorate("start read_file_security");

    _ASSERT(pfcache  != NULL);
    _ASSERT(filename != NULL);
    _ASSERT(ppsec    != NULL);

    *ppsec = NULL;

    sec_info |= OWNER_SECURITY_INFORMATION;
    sec_info |= GROUP_SECURITY_INFORMATION;
    sec_info |= DACL_SECURITY_INFORMATION;

    /* Get size of security buffer. Call is expected to fail */
    if(GetFileSecurity(filename, sec_info, NULL, 0, &desc_length))
    {
        goto Finished;
    }

    psec_desc = (unsigned char *)alloc_smalloc(pfcache->palloc, desc_length);
    if(psec_desc == NULL)
    {
        result = FATAL_OUT_OF_SMEMORY;
        goto Finished;
    }

    if(!GetFileSecurity(filename, sec_info, (PSECURITY_DESCRIPTOR)psec_desc, desc_length, &desc_length))
    {
        goto Finished;
    }

    *ppsec = psec_desc;
    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintverbose("failure %d in read_file_security", result);

        if(psec_desc != NULL)
        {
            alloc_sfree(pfcache->palloc, psec_desc);
            psec_desc = NULL;
        }
    }

    dprintdecorate("end read_file_security");

    return result;
}

static int read_file_content(HANDLE hFile, unsigned int filesize, void ** ppbuffer)
{
    int            result    = NONFATAL;
    HRESULT        hr        = S_OK;

    void *         pbuffer   = NULL;
    size_t         coffset   = 0;
    unsigned int   readahead = 0;
    char *         pvread    = NULL;
    OVERLAPPED     Overlapped;
    zend_bool      breturn;

    dprintverbose("start read_file_content");

    _ASSERT(hFile    != NULL);
    _ASSERT(filesize >  0);
    _ASSERT(ppbuffer != NULL);

    pbuffer = *ppbuffer;
    _ASSERT(pbuffer != NULL);

    ZeroMemory(&Overlapped, sizeof(OVERLAPPED));

    pvread = (char *)pbuffer;
    while(coffset < filesize)
    {
        Overlapped.Offset = (DWORD)coffset;
        if(filesize - coffset > READAHEAD_MAXIMUM)
        {
            readahead = READAHEAD_MAXIMUM;
        }
        else
        {
            /*
             * we've validated that the difference is safe to cast to
             * a 32-bit value before making this assignment.
             */
            readahead = (unsigned int)(filesize - coffset);
        }

        breturn = ReadFile(hFile, pvread, readahead, &readahead, &Overlapped);
        if(!breturn)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
            if(hr != HRESULT_FROM_WIN32(ERROR_IO_PENDING))
            {
                error_setlasterror();
                result = FATAL_FCACHE_READFILE;
                goto Finished;
            }

            hr = S_OK;

            breturn = GetOverlappedResult(hFile, &Overlapped, &readahead, TRUE);
            if(!breturn)
            {
                error_setlasterror();
                result = FATAL_FCACHE_READFILE;

                goto Finished;
            }
        }

        coffset += readahead;
        pvread += readahead;
    }

    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in read_file_content", result);
    }

    dprintverbose("end read_file_content");

    return result;
}

/* Public methods */
int fcache_create(fcache_context ** ppfcache)
{
    int              result  = NONFATAL;
    fcache_context * pfcache = NULL;

    dprintverbose("start fcache_create");

    _ASSERT(ppfcache != NULL);
    *ppfcache = NULL;

    pfcache = (fcache_context *)alloc_pemalloc(sizeof(fcache_context));
    if(pfcache == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    pfcache->id        = gfcacheid++;
    pfcache->islocal   = 0;
    pfcache->cachekey  = 0;
    pfcache->hinitdone = NULL;
    pfcache->maxfsize  = 0;
    pfcache->memaddr   = NULL;
    pfcache->header    = NULL;
    pfcache->pfilemap  = NULL;
    pfcache->prwlock   = NULL;
    pfcache->palloc    = NULL;

    *ppfcache = pfcache;

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in fcache_create", result);
    }

    dprintverbose("end fcache_create");

    return result;
}

void fcache_destroy(fcache_context * pfcache)
{
    dprintverbose("start fcache_destroy");

    if(pfcache != NULL)
    {
        alloc_pefree(pfcache);
        pfcache = NULL;
    }

    dprintverbose("end fcache_destroy");

    return;
}

int fcache_initialize(fcache_context * pfcache, unsigned short islocal, unsigned short cachekey, unsigned int cachesize, unsigned int maxfsize)
{
    int             result      = NONFATAL;
    size_t          size        = 0;
    fcache_header * header      = NULL;
    unsigned short  mapclass    = FILEMAP_MAP_SRANDOM;
    unsigned short  locktype    = LOCK_TYPE_SHARED;
    unsigned char   isfirst     = 1;
    unsigned char   islocked    = 0;
    unsigned int    initmemory  = 0;
    DWORD           ret         = 0;

    dprintverbose("start fcache_initialize");

    _ASSERT(pfcache   != NULL);
    _ASSERT(cachekey  != 0);
    _ASSERT(cachesize >= FCACHE_SIZE_MINIMUM && cachesize <= FCACHE_SIZE_MAXIMUM);
    _ASSERT(maxfsize  >= FILE_SIZE_MINIMUM   && maxfsize  <= FILE_SIZE_MAXIMUM);

    /* Initialize memory map to store code files */
    result = filemap_create(&pfcache->pfilemap);
    if(FAILED(result))
    {
        goto Finished;
    }

    pfcache->cachekey = cachekey;

    if(islocal)
    {
        mapclass = FILEMAP_MAP_LRANDOM;
        locktype = LOCK_TYPE_LOCAL;

        pfcache->islocal = islocal;
    }

    /* Create xread xwrite lock for the filecache */
    result = lock_create(&pfcache->prwlock);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = lock_initialize(pfcache->prwlock, "FILECONTENT_CACHE", cachekey, locktype, NULL);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = utils_create_init_event(pfcache->prwlock->nameprefix, "FCACHE_INIT", &pfcache->hinitdone, &isfirst);
    if (FAILED(result))
    {
        result = FATAL_FCACHE_INIT_EVENT;
        goto Finished;
    }

    islocked = 1;

    /* shmfilepath = NULL to use page file for shared memory */
    result = filemap_initialize(pfcache->pfilemap, FILEMAP_TYPE_FILECONTENT, cachekey, mapclass, cachesize, isfirst, NULL);
    if(FAILED(result))
    {
        goto Finished;
    }

    pfcache->memaddr = (char *)pfcache->pfilemap->mapaddr;
    size = filemap_getsize(pfcache->pfilemap);
    initmemory = (pfcache->pfilemap->existing == 0);

    /* Create allocator for filecache segment */
    result = alloc_create(&pfcache->palloc);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* initmemory = 1 for all page file backed shared memory allocators */
    result = alloc_initialize(pfcache->palloc, islocal, "FILECONTENT_SEGMENT", cachekey, pfcache->memaddr, size, 1);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* Get memory for cache header */
    pfcache->header = (fcache_header *)alloc_get_cacheheader(pfcache->palloc, sizeof(fcache_header), CACHE_TYPE_FILECONTENT);
    if(pfcache->header == NULL)
    {
        result = FATAL_FCACHE_INITIALIZE;
        goto Finished;
    }

    header = pfcache->header;

    /* Initialize the fcache_header if its not initialized already */
    if(islocal || isfirst)
    {
        if (initmemory)
        {
            /* No need to get a write lock as other processes */
            /* are blocked waiting for hinitdone event */

            header->mapcount    = 1;
            header->itemcount   = 0;
            header->hitcount    = 0;
            header->misscount   = 0;

            ReleaseMutex(pfcache->hinitdone);
            islocked = 0;
        }
    }
    else
    {
        /* Increment the mapcount */
        InterlockedIncrement(&header->mapcount);
    }

    /* Keep maxfsize in fcache_context */
    pfcache->maxfsize = maxfsize * 1024;

Finished:

    if (islocked)
    {
        ReleaseMutex(pfcache->hinitdone);
        islocked = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in fcache_initialize", result);

        if(pfcache->palloc != NULL)
        {
            alloc_terminate(pfcache->palloc);
            alloc_destroy(pfcache->palloc);

            pfcache->palloc = NULL;
        }

        if(pfcache->pfilemap != NULL)
        {
            filemap_terminate(pfcache->pfilemap);
            filemap_destroy(pfcache->pfilemap);

            pfcache->pfilemap = NULL;
        }

        if(pfcache->prwlock != NULL)
        {
            lock_terminate(pfcache->prwlock);
            lock_destroy(pfcache->prwlock);

            pfcache->prwlock = NULL;
        }

        if(pfcache->hinitdone != NULL)
        {
            CloseHandle(pfcache->hinitdone);
            pfcache->hinitdone = NULL;
        }
    }

    dprintverbose("end fcache_initialize");

    return result;
}

void fcache_terminate(fcache_context * pfcache)
{
    dprintverbose("start fcache_terminate");

    if(pfcache != NULL)
    {
        if(pfcache->header != NULL)
        {
            InterlockedDecrement(&pfcache->header->mapcount);
            pfcache->header = NULL;
        }

        if(pfcache->palloc != NULL)
        {
            alloc_terminate(pfcache->palloc);
            alloc_destroy(pfcache->palloc);

            pfcache->palloc = NULL;
        }

        if(pfcache->pfilemap != NULL)
        {
            filemap_terminate(pfcache->pfilemap);
            filemap_destroy(pfcache->pfilemap);

            pfcache->pfilemap = NULL;
        }

        if(pfcache->prwlock != NULL)
        {
            lock_terminate(pfcache->prwlock);
            lock_destroy(pfcache->prwlock);

            pfcache->prwlock = NULL;
        }

        if(pfcache->hinitdone != NULL)
        {
            CloseHandle(pfcache->hinitdone);
            pfcache->hinitdone = NULL;
        }
    }

    dprintverbose("end fcache_terminate");

    return;
}

int fcache_createval(fcache_context * pfcache, const char * filename, fcache_value ** ppvalue)
{
    int             result     = NONFATAL;
    fcache_value *  pvalue     = NULL;
    HANDLE          hFile      = INVALID_HANDLE_VALUE;
    void *          buffer     = NULL;
    unsigned int    filesize   = 0;
    unsigned char * psec       = NULL;
    unsigned int    openflags  = 0;
    unsigned int    attributes = 0;
    unsigned int    fileflags  = 0;

    BY_HANDLE_FILE_INFORMATION bhfi = {0};

    dprintverbose("start fcache_createval");

    _ASSERT(pfcache  != NULL);
    _ASSERT(filename != NULL);
    _ASSERT(ppvalue  != NULL);

    *ppvalue = NULL;

    /* Allocate memory for cache entry in shared memory */
    pvalue = (fcache_value *)alloc_smalloc(pfcache->palloc, sizeof(fcache_value));
    if(pvalue == NULL)
    {
        result = FATAL_OUT_OF_SMEMORY;
        goto Finished;
    }

    pvalue->file_sec     = 0;
    pvalue->file_content = 0;

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

    if(!GetFileInformationByHandle(hFile, &bhfi))
    {
        result = FATAL_FCACHE_BYHANDLE_INFO;
        goto Finished;
    }

    attributes = bhfi.dwFileAttributes;
    if(attributes & FILE_ATTRIBUTE_DIRECTORY)
    {
        fileflags |= FILE_IS_FOLDER;
    }
    else
    {
        /* Dont cache files bigger than maxfsize */
        if(bhfi.nFileSizeHigh > 0 || bhfi.nFileSizeLow > pfcache->maxfsize)
        {
            result = WARNING_FCACHE_TOOBIG;
            goto Finished;
        }

        /* File size is just the low file size */
        filesize = bhfi.nFileSizeLow;

        /* Allocate memory for entire file in shared memory */
        buffer = alloc_smalloc(pfcache->palloc, filesize);
        if(buffer == NULL)
        {
            result = FATAL_OUT_OF_SMEMORY;
            goto Finished;
        }

        /* If filesize is greater than 0, read file content */
        if(filesize > 0)
        {
            result = read_file_content(hFile, filesize, &buffer);
            if(FAILED(result))
            {
                goto Finished;
            }
        }
    }

    /* Read security information */
    result = read_file_security(pfcache, filename, &psec);
    if(FAILED(result))
    {
        goto Finished;
    }

    fileflags |= FILE_IS_RUNAWARE;
    fileflags |= FILE_IS_WUNAWARE;

    pvalue->file_size    = filesize;
    pvalue->file_sec     = psec - pfcache->memaddr;
    pvalue->file_content = ((buffer == NULL) ? 0 : ((char *)buffer - pfcache->memaddr));
    pvalue->file_flags   = fileflags;
    pvalue->is_deleted   = 0;
    pvalue->hitcount     = 0;
    pvalue->refcount     = 0;

    InterlockedIncrement(&pfcache->header->itemcount);
    InterlockedIncrement(&pfcache->header->misscount);

    /* Decrement the hitcounts here. They will be incremented*/
    /* back in refinc so that hitcount remains the same */
    InterlockedDecrement(&pfcache->header->hitcount);
    InterlockedDecrement(&pvalue->hitcount);

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
        dprintverbose("failure %d in fcache_createval", result);

        if(buffer != NULL)
        {
            alloc_sfree(pfcache->palloc, buffer);
            buffer = NULL;
        }

        if(psec != NULL)
        {
            alloc_sfree(pfcache->palloc, psec);
            psec = NULL;
        }

        if(pvalue != NULL)
        {
            pvalue->file_sec     = 0;
            pvalue->file_content = 0;

            alloc_sfree(pfcache->palloc, pvalue);
            pvalue = NULL;
        }
    }

    dprintverbose("end fcache_createval");

    return result;
}

void fcache_destroyval(fcache_context * pfcache, fcache_value * pvalue)
{
    dprintverbose("start fcache_destroyval");

    _ASSERT(pvalue->refcount == 0);

    if(pvalue != NULL)
    {
        if(pvalue->file_sec != 0)
        {
            alloc_sfree(pfcache->palloc, pfcache->memaddr + pvalue->file_sec);
            pvalue->file_sec = 0;
        }

        if(pvalue->file_content != 0)
        {
            alloc_sfree(pfcache->palloc, pfcache->memaddr + pvalue->file_content);
            pvalue->file_content = 0;
        }

        alloc_sfree(pfcache->palloc, pvalue);
        pvalue = NULL;

        InterlockedDecrement(&pfcache->header->itemcount);
    }

    dprintverbose("end fcache_destroyval");

    return;
}

int fcache_useval(fcache_context * pcache, const char * filename, fcache_value * pvalue, zend_file_handle ** pphandle)
{
    int                result    = NONFATAL;
    int                allocated = 0;
    zend_file_handle * phandle   = NULL;
    fcache_handle *    fhandle   = NULL;

    dprintverbose("start fcache_useval");

    _ASSERT(filename != NULL);
    _ASSERT(pvalue   != NULL);
    _ASSERT(pphandle != NULL);

    if(*pphandle == NULL)
    {
        phandle = (zend_file_handle *)alloc_emalloc(sizeof(zend_file_handle));
        if(phandle == NULL)
        {
            result = FATAL_OUT_OF_LMEMORY;
            goto Finished;
        }

        allocated = 1;
    }
    else
    {
        phandle = *pphandle;
    }

    /* Allocate memory for fcache_handle. Release memory in fcache_closer */
    fhandle = (fcache_handle *)alloc_emalloc(sizeof(fcache_handle));
    if(fhandle == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    phandle->filename      = (char *)filename;
    phandle->free_filename = 0;
    phandle->opened_path   = zend_string_init(filename, strlen(filename), 0);

    ZeroMemory(&phandle->handle.stream, sizeof(zend_stream));

    phandle->handle.stream.reader  = (zend_stream_reader_t)fcache_reader;
    phandle->handle.stream.closer  = (zend_stream_closer_t)fcache_closer;
    phandle->handle.stream.fsizer  = (zend_stream_fsizer_t)fcache_fsizer;

    fhandle->pfcache = pcache;
    fhandle->pfvalue = pvalue;
    fhandle->map     = (void *)(pcache->memaddr + pvalue->file_content);
    fhandle->buf     = pcache->memaddr + pvalue->file_content;
    fhandle->len     = pvalue->file_size;
    fhandle->pos     = 0;

    /* Init the php_handle in the fcache_handle object */
    fcache_init_php_handle(fhandle);

    phandle->handle.stream.handle = fhandle;
    phandle->type = ZEND_HANDLE_STREAM;

    *pphandle = phandle;

    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in fcache_useval", result);

        if(fhandle != NULL)
        {
            alloc_efree(fhandle);
            fhandle = NULL;
        }

        if(phandle != NULL && allocated == 1)
        {
            alloc_efree(phandle);
            phandle = NULL;
        }
    }

    dprintverbose("end fcache_useval");

    return result;
}

fcache_value * fcache_getvalue(fcache_context * pfcache, size_t offset)
{
    _ASSERT(pfcache != NULL);
    return ((offset != 0) ? ((fcache_value *)(pfcache->memaddr + offset)) : NULL);
}

size_t fcache_getoffset(fcache_context * pfcache, fcache_value * pvalue)
{
    _ASSERT(pfcache != NULL);
    return ((pvalue != NULL) ? ((char *)pvalue - pfcache->memaddr) : 0);
}

void fcache_refinc(fcache_context * pfcache, fcache_value * pvalue)
{
    _ASSERT(pfcache != NULL);
    _ASSERT(pvalue  != NULL);

    InterlockedIncrement(&pvalue->refcount);
    InterlockedIncrement(&pvalue->hitcount);
    InterlockedIncrement(&pfcache->header->hitcount);

    return;
}

void fcache_refdec(fcache_context * pfcache, fcache_value * pvalue)
{
    _ASSERT(pfcache != NULL);
    _ASSERT(pvalue  != NULL);

    InterlockedDecrement(&pvalue->refcount);

    if(InterlockedCompareExchange(&pvalue->is_deleted, 1, 1) == 1 &&
       InterlockedCompareExchange(&pvalue->refcount, 0, 0) == 0)
    {
        fcache_destroyval(pfcache, pvalue);
        pvalue = NULL;
    }

    return;
}

int fcache_getinfo(fcache_value * pvalue, fcache_entry_info ** ppinfo)
{
    int                 result = NONFATAL;
    fcache_entry_info * pinfo  = NULL;

    dprintverbose("start fcache_getinfo");

    _ASSERT(pvalue != NULL);
    _ASSERT(ppinfo != NULL);

    *ppinfo = NULL;

    pinfo = (fcache_entry_info *)alloc_emalloc(sizeof(fcache_entry_info));
    if(pinfo == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    pinfo->filesize = pvalue->file_size;
    pinfo->hitcount = pvalue->hitcount;

    *ppinfo = pinfo;

    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in fcache_getinfo", result);
    }

    dprintverbose("end fcache_getinfo");

    return result;
}

void fcache_freeinfo(fcache_entry_info * pinfo)
{
    if(pinfo != NULL)
    {
        alloc_efree(pinfo);
        pinfo = NULL;
    }

    return;
}

size_t fcache_fsizer(void * handle)
{
    size_t          size    = 0;
    fcache_handle * fhandle = NULL;

    dprintverbose("start fcache_fsizer");

    _ASSERT(handle != NULL);

    fhandle = (fcache_handle *)handle;
    size    = fhandle->len;

    dprintverbose("end fcache_fsizer");

    return size;
}

size_t fcache_reader(void * handle, char * buf, size_t length)
{
    size_t          size    = 0;
    fcache_handle * fhandle = NULL;
    fcache_value *  pvalue  = NULL;
    size_t          bleft   = 0;

    dprintverbose("start fcache_reader");

    _ASSERT(handle != NULL);
    _ASSERT(buf    != NULL);

    fhandle = (fcache_handle *)handle;
    pvalue = fhandle->pfvalue;

    _ASSERT(fhandle->len == pvalue->file_size);

    /* Read either length bytes or the number of bytes left */
    bleft = fhandle->len - fhandle->pos;
    if(bleft > length)
    {
        size = length;
    }
    else
    {
        size = bleft;
    }

    /* Copy characters from file buffer to buffer */
    /* and update current position and buffer pointer */
    memcpy_s(buf, size, fhandle->buf, size);
    fhandle->pos += size;
    fhandle->buf += size;

    dprintverbose("end fcache_reader");

    return size;
}

void fcache_closer(void * handle)
{
    fcache_handle * fhandle = NULL;

    dprintverbose("start fcache_closer");

    _ASSERT(handle != NULL);

    /* Just decrement the refcount. Scavenger will remove the entry */
    fhandle = (fcache_handle *)handle;
    fcache_refdec(fhandle->pfcache, fhandle->pfvalue);

    /* Free memory allocated for fcache_handle */
    alloc_efree(fhandle);
    fhandle = handle = NULL;

    dprintverbose("end fcache_closer");

    return;
}

void fcache_runtest()
{
    int              result    = NONFATAL;
    fcache_context * pfcache   = NULL;
    fcache_value *   pvalue    = NULL;
    unsigned char    islocal   = 0;
    unsigned int     cachesize = 32;
    unsigned int     maxfsize  = 250;
    char *           filename  = "testfile.php";

    dprintverbose("*** STARTING FCACHE TESTS ***");

    result = fcache_create(&pfcache);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = fcache_initialize(pfcache, islocal, 57, cachesize, maxfsize);
    if(FAILED(result))
    {
        goto Finished;
    }

    _ASSERT(pfcache->id       != 0);
    _ASSERT(pfcache->maxfsize == maxfsize * 1024);
    _ASSERT(pfcache->header   != NULL);
    _ASSERT(pfcache->pfilemap != NULL);
    _ASSERT(pfcache->prwlock  != NULL);
    _ASSERT(pfcache->palloc   != NULL);

    _ASSERT(pfcache->header->itemcount  == 0);
    _ASSERT(pfcache->header->hitcount   == 0);
    _ASSERT(pfcache->header->misscount  == 0);

Finished:

    if(pfcache != NULL)
    {
        fcache_terminate(pfcache);
        fcache_destroy(pfcache);

        pfcache =  NULL;
    }

    dprintverbose("*** ENDING FCACHE TESTS ***");

    return;
}

/*
 * php_stream_ops functions
 */

size_t wincache_stream_write(php_stream *stream, const char *buf, size_t count)
{
    /* ignore writes */
    return 0;
}

size_t wincache_stream_read(php_stream *stream, char *buf, size_t count)
{
    size_t toread = 0;

    toread = stream->writepos - stream->readpos;
    if (toread > count)
    {
        toread = count;
    }

    if (count == 0)
    {
        return 0;
    }

    memcpy_s(buf, toread, stream->readbuf + stream->readpos, toread);
    stream->readpos += toread;
    stream->position = stream->readpos;

    /* Check if we're at EOF */
    if (stream->readpos == stream->writepos)
    {
        stream->eof = 1;
    }
    else
    {
        stream->eof = 0;
    }

    return toread;
}

int  wincache_stream_close(php_stream *stream, int close_handle)
{
    /* ignore close */
    return 0;
}
int  wincache_stream_flush(php_stream *stream)
{
    /* ignore flush */
    return 0;
}

/*++

Routine Description:

    Seek on the php_stream wrapper.  Note that for Wincache streams, the entire
    file is mapped into a single memory buffer.

Arguments:

    stream - php_stream object for this fcache instance.
    offset - Number of bytes to offset from whence.
    whence - Location in file from which to calculate the new position.
    newoffset - Upon success, the new location within the file of the position.

Returns:

    0 - Success
    -1 - Failure
 -*/
int  wincache_stream_seek(php_stream *stream, zend_off_t offset, int whence, zend_off_t *newoffset)
{
    int ret = -1;
    zend_off_t newPos = 0;

    switch (whence) {
    case SEEK_CUR: /* Current Position.  Offset may be negative. */
        newPos = (offset + stream->position);
        if (newPos >= 0 && newPos <= stream->writepos)
        {
            /* offset is within the memory buffer for the file */
            stream->position = newPos;
            stream->readpos = newPos;
            ret = 0;
        }
        break;
    case SEEK_SET: /* Beginning of file */
        if (offset >= 0 && offset <= stream->writepos)
        {
            stream->position = offset;
            stream->readpos = offset;
            ret = 0;
        }
        break;
    case SEEK_END: /* End of file */
        if (offset == 0)
        {
            stream->position = stream->writepos;
            stream->readpos = stream->writepos;
            ret = 0;
        }
        break;
    }

    /* Check if we're at EOF */
    if (stream->readpos == stream->writepos)
    {
        stream->eof = 1;
    }
    else
    {
        stream->eof = 0;
    }

    if (newoffset)
    {
        *newoffset = stream->position;
    }

    return ret;
}

int  wincache_stream_cast(php_stream *stream, int castas, void **ret)
{
    /* ignore cast */
    return 0;
}

int wincache_stream_stat(php_stream *stream, php_stream_statbuf *ssb)
{
    /* never return stat info */
    return -1;
}

int wincache_stream_set_option(php_stream *stream, int option, int value, void *ptrparam)
{
    /* ignore set option */
    return 0;
}

/*
 * php_stream_ops for wincache streams
 */

php_stream_ops g_fcache_stream_ops = {
    wincache_stream_write,
    wincache_stream_read,
    wincache_stream_close,
    wincache_stream_flush,
    "wincache_stream_ops",
    wincache_stream_seek,
    wincache_stream_cast,
    wincache_stream_stat,
    wincache_stream_set_option
};

/*
 * NOTE:
 * PHP 5.4 performs a '#!' (a.k.a. 'shebang') check on *all* files in
 * php_cgi!main by default.  This includes our streams.  We can either turn
 * off the shebang check (cgi.check_shebang_line = Off), or we can add
 * support for php_stream's to the wincache fcache_handle.  The easiest is
 * to turn off the shebang check.  However, we can't do it programatically.
 */
void fcache_init_php_handle(fcache_handle *fhandle)
{
    php_stream *phpHandle = NULL;

    _ASSERT(fhandle);

    phpHandle = &fhandle->wrapper;

    ZeroMemory(phpHandle, sizeof(php_stream));

    /* set up the ops for our internal handle */
    phpHandle->ops = &g_fcache_stream_ops;

    /* Not sure if we really need to do this */
    phpHandle->readfilters.stream = phpHandle;
    phpHandle->writefilters.stream = phpHandle;

    /* set up the internal buffer pointers */
    phpHandle->writepos = (off_t)fhandle->len;
    phpHandle->readpos = 0;
    phpHandle->readbuf = (unsigned char *)fhandle->map;

    /* make sure no one can close this stream */
    phpHandle->flags = (PHP_STREAM_FLAG_NO_CLOSE  |
                        PHP_STREAM_FLAG_NO_FCLOSE);
    phpHandle->fclose_stdiocast = PHP_STREAM_FCLOSE_NONE;

    return;
}
