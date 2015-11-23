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
   | Module: wincache_rplist.c                                                                    |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   +----------------------------------------------------------------------------------------------+
*/

#include "precomp.h"

#define RPLIST_VALUE(p, o)            ((rplist_value *)alloc_get_cachevalue(p, o))

static int  findrpath_in_cache(rplist_context * pcache, const char * filename, const char * cwd_cexec, unsigned int index, rplist_value ** ppvalue);
static int  create_rplist_data(rplist_context * pcache, const char * filename, const char * cwdcexec, rplist_value ** ppvalue);
static void destroy_rplist_data(rplist_context * pcache, rplist_value * pvalue);
static void add_rplist_entry(rplist_context * pcache, unsigned int index, rplist_value * pvalue);
static void remove_rplist_entry(rplist_context * pcache, unsigned int index, rplist_value * pvalue);

/* Private methods */

/* Call this method atleast under a read lock */
static int findrpath_in_cache(rplist_context * pcache, const char * filename, const char * cwd_cexec, unsigned int index, rplist_value ** ppvalue)
{
    int             result   = NONFATAL;
    rplist_header * rpheader = NULL;
    rplist_value *  pvalue   = NULL;

    dprintverbose("start findrpath_in_cache");

    _ASSERT(pcache    != NULL);
    _ASSERT(filename  != NULL);
    _ASSERT(cwd_cexec != NULL);
    _ASSERT(ppvalue   != NULL);

    *ppvalue = NULL;

    rpheader = pcache->rpheader;
    pvalue = RPLIST_VALUE(pcache->rpalloc, rpheader->values[index]);

    while(pvalue != NULL)
    {
        /* Ignore deleted entries */
        if(pvalue->is_deleted == 0 &&
           !_stricmp(pcache->rpmemaddr + pvalue->file_path, filename) &&
           !_stricmp(pcache->rpmemaddr + pvalue->cwd_cexec, cwd_cexec) &&
           ((PG(include_path) == NULL && strlen(pcache->rpmemaddr + pvalue->inc_path) == 0) ||
            (PG(include_path) != NULL && _stricmp(pcache->rpmemaddr + pvalue->inc_path, PG(include_path)) == 0)) &&
           ((PG(open_basedir) == NULL && strlen(pcache->rpmemaddr + pvalue->open_based) == 0) ||
            (PG(open_basedir) != NULL && _stricmp(pcache->rpmemaddr + pvalue->open_based, PG(open_basedir)) == 0)))
        {
            *ppvalue = pvalue;
            break;
        }

        pvalue = RPLIST_VALUE(pcache->rpalloc, pvalue->next_value);
    }

    dprintverbose("end findrpath_in_cache");

    return result;
}

static int create_rplist_data(rplist_context * pcache, const char * filename, const char * cwdcexec, rplist_value ** ppvalue)
{
    int            result   = NONFATAL;
    rplist_value * pvalue   = NULL;
    char *         filepath = NULL;
    char *         fileinfo = NULL;

    size_t         flength  = 0;
    size_t         cclength = 0;
    size_t         incplen  = 0;
    size_t         openblen = 0;
    size_t         alloclen = 0;
    size_t         memlen   = 0;
    char *         pbaseadr = NULL;

    dprintverbose("start create_rplist_data");

    _ASSERT(pcache   != NULL);
    _ASSERT(filename != NULL);
    _ASSERT(cwdcexec != NULL);
    _ASSERT(ppvalue  != NULL);

    *ppvalue = NULL;

    flength  = strlen(filename);
    cclength = strlen(cwdcexec);

    if(PG(include_path) != NULL)
    {
        incplen  = strlen(PG(include_path));
    }

    if(PG(open_basedir) != NULL)
    {
        openblen = strlen(PG(open_basedir));
    }

    alloclen = sizeof(rplist_value) + ALIGNDWORD(flength + 1) + ALIGNDWORD(cclength + 1) + ALIGNDWORD(incplen + 1) + ALIGNDWORD(openblen + 1);

    /* Allocate memory for cache entry in shared memory */
    pbaseadr = (char *)alloc_smalloc(pcache->rpalloc, alloclen);
    if(pbaseadr == NULL)
    {
        result = FATAL_OUT_OF_SMEMORY;
        goto Finished;
    }

    pvalue = (rplist_value *)pbaseadr;

    /* Point pbaseadr to end of rplist_value */
    /* No goto Finished should exist after this */
    pbaseadr += sizeof(rplist_value);

    pvalue->file_path   = 0;
    pvalue->cwd_cexec   = 0;
    pvalue->inc_path    = 0;
    pvalue->open_based  = 0;

    pvalue->is_deleted  = 0;
    pvalue->is_verified = VERIFICATION_NOTDONE;
    pvalue->absentry    = 0;
    pvalue->same_value  = 0;
    pvalue->prev_value  = 0;
    pvalue->next_value  = 0;

    /* Append filepath and keep offset in file_path */
    memlen = ALIGNDWORD(flength + 1);
    memcpy_s(pbaseadr, flength, filename, flength);
    *(pbaseadr + flength) = 0;
    pvalue->file_path = pbaseadr - pcache->rpmemaddr;
    pbaseadr += memlen;

    /* Append current working directory and currently */
    /* executing file. Store offset in cwd_cexec */
    memlen = ALIGNDWORD(cclength + 1);
    memcpy_s(pbaseadr, cclength, cwdcexec, cclength);
    *(pbaseadr + cclength) = 0;
    pvalue->cwd_cexec = pbaseadr - pcache->rpmemaddr;
    pbaseadr += memlen;

    /* Append current include_path value and save offset */
    memlen = ALIGNDWORD(incplen + 1);
    if(PG(include_path) != NULL)
    {
        memcpy_s(pbaseadr, incplen, PG(include_path), incplen);
    }
    *(pbaseadr + incplen) = 0;
    pvalue->inc_path = pbaseadr - pcache->rpmemaddr;
    pbaseadr += memlen;

    /* Append current open_basedir value and save offset */
    memlen = ALIGNDWORD(openblen + 1);
    if(PG(open_basedir) != NULL)
    {
        memcpy_s(pbaseadr, openblen, PG(open_basedir), openblen);
    }
    *(pbaseadr + openblen) = 0;
    pvalue->open_based = pbaseadr - pcache->rpmemaddr;
    pbaseadr += memlen;

    /* Fill the pointer value */
    *ppvalue = pvalue;

    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintverbose("failure %d in create_rplist_data", result);

        if(pbaseadr != NULL)
        {
            alloc_sfree(pcache->rpalloc, pbaseadr);
            pbaseadr = NULL;
        }
    }

    dprintverbose("end create_rplist_data");

    return result;
}

static void destroy_rplist_data(rplist_context * pcache, rplist_value * pvalue)
{
    dprintverbose("start destroy_rplist_data");

    if(pvalue != NULL)
    {
        /* file_path, cwd_cexec, inc_path and open_based are appended */
        /* to rplist_value and should not be freed separately */
        alloc_sfree(pcache->rpalloc, pvalue);
        pvalue = NULL;
    }

    dprintverbose("end destroy_rplist_data");
}

/* Call this method under write lock */
static void add_rplist_entry(rplist_context * pcache, unsigned int index, rplist_value * pvalue)
{
    rplist_header * rpheader = NULL;
    rplist_value *  pcheck   = NULL;

    dprintverbose("start add_rplist_entry");

    _ASSERT(pcache            != NULL);
    _ASSERT(pvalue            != NULL);
    _ASSERT(pvalue->file_path != 0);
    _ASSERT(pvalue->cwd_cexec != 0);

    rpheader = pcache->rpheader;
    pcheck = RPLIST_VALUE(pcache->rpalloc, rpheader->values[index]);

    while(pcheck != NULL)
    {
        if(pcheck->next_value == 0)
        {
            break;
        }

        pcheck = RPLIST_VALUE(pcache->rpalloc, pcheck->next_value);
    }

    if(pcheck != NULL)
    {
        pcheck->next_value = alloc_get_valueoffset(pcache->rpalloc, pvalue);
        pvalue->next_value = 0;
        pvalue->prev_value = alloc_get_valueoffset(pcache->rpalloc, pcheck);
    }
    else
    {
        rpheader->values[index] = alloc_get_valueoffset(pcache->rpalloc, pvalue);
        pvalue->next_value = 0;
        pvalue->prev_value = 0;
    }

    rpheader->itemcount++;

    dprintverbose("end add_rplist_entry");
    return;
}

/* Call this method under write lock */
static void remove_rplist_entry(rplist_context * pcache, unsigned int index, rplist_value * pvalue)
{
    alloc_context * palloc  = NULL;
    rplist_header * header  = NULL;
    rplist_value *  ptemp   = NULL;

    dprintverbose("start remove_rplist_entry");

    _ASSERT(pcache            != NULL);
    _ASSERT(pvalue            != NULL);
    _ASSERT(pvalue->file_path != 0);

    header = pcache->rpheader;
    palloc = pcache->rpalloc;

    header->itemcount--;
    if(pvalue->prev_value == 0)
    {
        header->values[index] = pvalue->next_value;
        if(pvalue->next_value != 0)
        {
            ptemp = RPLIST_VALUE(palloc, pvalue->next_value);
            ptemp->prev_value = 0;
        }
    }
    else
    {
        ptemp = RPLIST_VALUE(palloc, pvalue->prev_value);
        ptemp->next_value = pvalue->next_value;

        if(pvalue->next_value != 0)
        {
            ptemp = RPLIST_VALUE(palloc, pvalue->next_value);
            ptemp->prev_value = pvalue->prev_value;
        }
    }

    destroy_rplist_data(pcache, pvalue);
    pvalue = NULL;

    dprintverbose("end remove_rplist_entry");
    return;
}

/* Public functions */
int rplist_create(rplist_context ** ppcache)
{
    int              result = NONFATAL;
    rplist_context * pcache = NULL;

    dprintverbose("start rplist_create");

    _ASSERT(ppcache != NULL);
    *ppcache = NULL;

    pcache = (rplist_context *)alloc_pemalloc(sizeof(rplist_context));
    if(pcache == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    pcache->rpmemaddr = NULL;
    pcache->rpheader  = NULL;
    pcache->rpfilemap = NULL;
    pcache->rplock    = NULL;
    pcache->rpalloc   = NULL;

    *ppcache = pcache;

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in rplist_create", result);
    }

    dprintverbose("end rplist_create");

    return result;
}

void rplist_destroy(rplist_context * pcache)
{
    dprintverbose("start rplist_destroy");

    if(pcache != NULL)
    {
        alloc_pefree(pcache);
        pcache = NULL;
    }

    dprintverbose("end rplist_destroy");

    return;
}

int rplist_initialize(rplist_context * pcache, unsigned short islocal, unsigned char isfirst, unsigned short cachekey, unsigned int filecount)
{
    int             result   = NONFATAL;
    unsigned int    mapsize  = 0;
    size_t          segsize  = 0;
    unsigned short  mapclass = FILEMAP_MAP_SRANDOM;
    unsigned short  locktype = LOCK_TYPE_SHARED;
    unsigned int    msize    = 0;

    dprintverbose("start rplist_initialize");

    _ASSERT(pcache    != NULL);
    _ASSERT(filecount >= NUM_FILES_MINIMUM && filecount <= NUM_FILES_MAXIMUM);

    /* Create respaths segment */
    result = filemap_create(&pcache->rpfilemap);
    if(FAILED(result))
    {
        goto Finished;
    }

    mapsize = ((filecount + 1023) * 2) / 1024;
    if(islocal)
    {
        mapclass = FILEMAP_MAP_LRANDOM;
        locktype = LOCK_TYPE_LOCAL;
    }

    /* shmfilepath = NULL to create filemap on page file */
    result = filemap_initialize(pcache->rpfilemap, FILEMAP_TYPE_RESPATHS, cachekey, mapclass, mapsize, isfirst, NULL);
    if(FAILED(result))
    {
        goto Finished;
    }

    pcache->rpmemaddr = (char *)pcache->rpfilemap->mapaddr;
    segsize = filemap_getsize(pcache->rpfilemap);

    /* Create allocator for respaths segment */
    result = alloc_create(&pcache->rpalloc);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* initmemory = 1 for all page file backed shared memory allocators */
    result = alloc_initialize(pcache->rpalloc, islocal, "RESPATHS_SEGMENT", cachekey, pcache->rpfilemap->mapaddr, segsize, 1);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* Get memory for cache header */
    msize = sizeof(rplist_header) + ((filecount - 1) * sizeof(size_t));
    pcache->rpheader = (rplist_header *)alloc_get_cacheheader(pcache->rpalloc, msize, CACHE_TYPE_RESPATHS);
    if(pcache->rpheader == NULL)
    {
        result = FATAL_RPLIST_INITIALIZE;
        goto Finished;
    }

    /* Create reader writer lock for the file list */
    result = lock_create(&pcache->rplock);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = lock_initialize(pcache->rplock, "RESPATHS_CACHE", cachekey, locktype, &pcache->rpheader->last_owner);
    if(FAILED(result))
    {
        goto Finished;
    }

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in rplist_initialize", result);

        rplist_terminate(pcache);
    }

    dprintverbose("end rplist_initialize");

    return result;
}

void rplist_initheader(rplist_context * pcache, unsigned int filecount)
{
     rplist_header * rpheader = NULL;

     dprintverbose("start rplist_initheader");

     _ASSERT(pcache           != NULL);
     _ASSERT(pcache->rpheader != NULL);
     _ASSERT(filecount        >  0);

     rpheader = pcache->rpheader;
     _ASSERT(rpheader->values != NULL);

     /* This method is called by aplist_initialize which is */
     /* taking care of blocking other processes */
     /* Also last_owner can be safely set to 0 as lock is not active */
     rpheader->itemcount  = 0;
     rpheader->last_owner = 0;
     rpheader->valuecount = filecount;
     memset((void *)rpheader->values, 0, sizeof(size_t) * filecount);

     dprintverbose("end rplist_initheader");

     return;
}

void rplist_terminate(rplist_context * pcache)
{
    dprintverbose("start rplist_terminate");

    if(pcache != NULL)
    {
        if(pcache->rpalloc != NULL)
        {
            alloc_terminate(pcache->rpalloc);
            alloc_destroy(pcache->rpalloc);

            pcache->rpalloc = NULL;
        }

        if(pcache->rpfilemap != NULL)
        {
            filemap_terminate(pcache->rpfilemap);
            filemap_destroy(pcache->rpfilemap);

            pcache->rpfilemap = NULL;
        }

        if(pcache->rplock != NULL)
        {
            lock_terminate(pcache->rplock);
            lock_destroy(pcache->rplock);

            pcache->rplock = NULL;
        }

        pcache->rpheader = NULL;
    }

    dprintverbose("end rplist_terminate");
    return;
}

int rplist_getentry(rplist_context * pcache, const char * filename, rplist_value ** ppvalue, size_t * poffset)
{
    int             result   = NONFATAL;
    unsigned char   flock    = 0;
    unsigned int    findex   = 0;
    char            cwdcexec[  MAX_PATH * 2];

    rplist_value *  pvalue   = NULL;
    rplist_value *  pnewval  = NULL;
    rplist_header * rpheader = NULL;

    dprintverbose("start rplist_getentry");

    _ASSERT(pcache   != NULL);
    _ASSERT(filename != NULL);
    _ASSERT(ppvalue  != NULL);
    _ASSERT(poffset  != NULL);

    *ppvalue = NULL;
    *poffset = 0;

    rpheader = pcache->rpheader;
    findex = utils_getindex(filename, rpheader->valuecount);

    result = utils_cwdcexec(cwdcexec, MAX_PATH * 2);
    if(FAILED(result))
    {
        goto Finished;
    }

    lock_lock(pcache->rplock);
    flock = 1;

    result = findrpath_in_cache(pcache, filename, cwdcexec, findex, &pvalue);

    if(FAILED(result))
    {
        goto Finished;
    }

    /* If the entry was not found in cache */
    if(pvalue == NULL)
    {
        result = create_rplist_data(pcache, filename, cwdcexec, &pnewval);
        if(FAILED(result))
        {
            goto Finished;
        }

        pvalue = pnewval;
        pnewval = NULL;

        add_rplist_entry(pcache, findex, pvalue);
    }

    _ASSERT(pvalue != NULL);

    *ppvalue = pvalue;
    *poffset = ((char *)pvalue - pcache->rpmemaddr);

    _ASSERT(SUCCEEDED(result));

Finished:

    if(flock)
    {
        lock_unlock(pcache->rplock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintverbose("failure %d in rplist_getentry", result);

        if(pnewval != NULL)
        {
            destroy_rplist_data(pcache, pnewval);
            pnewval = NULL;
        }
    }

    dprintverbose("end rplist_getentry");

    return result;
}

void rplist_setabsval(rplist_context * pcache, rplist_value * pvalue, size_t absentry, size_t prevsame)
{
    size_t coffset = 0;

    dprintverbose("start rplist_setabsval");

    _ASSERT(pcache   != NULL);
    _ASSERT(pvalue   != NULL);
    _ASSERT(absentry != 0);

    /* Acquire write lock before changing absentry */
    lock_lock(pcache->rplock);

    pvalue->absentry   = absentry;

    /* Set same_value to make a list only if */
    /* prevsame is pointing to a different entry */
    coffset = (char *)pvalue - pcache->rpmemaddr;
    if(coffset != prevsame)
    {
        pvalue->same_value = prevsame;
    }

    lock_unlock(pcache->rplock);

    dprintverbose("end rplist_setabsval");
    return;
}

void rplist_deleteval(rplist_context * pcache, size_t valoffset)
{
    unsigned int   index  = 0;
    rplist_value * pvalue = NULL;
    rplist_value * ptemp  = NULL;

    dprintverbose("start rplist_deleteval");

    _ASSERT(pcache    != NULL);
    _ASSERT(valoffset != 0);

    lock_lock(pcache->rplock);

    pvalue = RPLIST_VALUE(pcache->rpalloc, valoffset);
    while(pvalue != NULL)
    {
        ptemp = pvalue;
        pvalue = NULL;

        if(ptemp->same_value != 0)
        {
            pvalue = RPLIST_VALUE(pcache->rpalloc, ptemp->same_value);
        }

        index = utils_getindex(pcache->rpmemaddr + ptemp->file_path, pcache->rpheader->valuecount);
        remove_rplist_entry(pcache, index, ptemp);
    }

    lock_unlock(pcache->rplock);

    dprintverbose("end rplist_deleteval");
    return;
}

void rplist_markdeleted(rplist_context * pcache, size_t valoffset)
{
    unsigned int   index  = 0;
    rplist_value * pvalue = NULL;

    dprintverbose("start rplist_markdeleted");

    _ASSERT(pcache    != NULL);
    _ASSERT(valoffset != 0);

    lock_lock(pcache->rplock);

    pvalue = RPLIST_VALUE(pcache->rpalloc, valoffset);
    while(pvalue != NULL)
    {
        pvalue->is_deleted = 1;

        /* Continue until all entries pointing to same aplist value are marked deleted */
        if(pvalue->same_value == 0)
        {
            break;
        }

        pvalue = RPLIST_VALUE(pcache->rpalloc, pvalue->same_value);
    }

    lock_unlock(pcache->rplock);

    dprintverbose("end rplist_markdeleted");
    return;
}

int rplist_getinfo(rplist_context * pcache, zend_bool summaryonly, rplist_info ** ppinfo)
{
    int                  result  = NONFATAL;
    rplist_info *        pcinfo  = NULL;
    rplist_entry_info *  peinfo  = NULL;
    rplist_entry_info *  ptemp   = NULL;
    rplist_value *       pvalue  = NULL;
    unsigned char        flock   = 0;
    size_t               offset  = 0;
    unsigned int         count   = 0;
    unsigned int         index   = 0;

    dprintverbose("start rplist_getinfo");

    _ASSERT(pcache != NULL);
    _ASSERT(ppinfo != NULL);

    *ppinfo = NULL;

    pcinfo = (rplist_info *)alloc_emalloc(sizeof(rplist_info));
    if(pcinfo == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    lock_lock(pcache->rplock);
    flock = 1;

    pcinfo->itemcount = pcache->rpheader->itemcount;
    pcinfo->entries   = NULL;

    /* Leave count to 0 if only summary is needed */
    if(!summaryonly)
    {
        count = pcache->rpheader->valuecount;
    }

    for(index = 0; index < count; index++)
    {
        offset = pcache->rpheader->values[index];
        if(offset == 0)
        {
            continue;
        }

        pvalue = (rplist_value *)alloc_get_cachevalue(pcache->rpalloc, offset);
        while(pvalue != NULL)
        {
            ptemp = (rplist_entry_info *)alloc_emalloc(sizeof(rplist_entry_info));
            if(ptemp == NULL)
            {
                result = FATAL_OUT_OF_LMEMORY;
                goto Finished;
            }

            _ASSERT(pvalue->file_path != 0);

            ptemp->pathkey = alloc_estrdup(pcache->rpmemaddr + pvalue->file_path);
            ptemp->subkey  = alloc_estrdup(pcache->rpmemaddr + pvalue->cwd_cexec);
            ptemp->abspath = NULL;
            ptemp->next    = NULL;

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

            pvalue = (rplist_value *)alloc_get_cachevalue(pcache->rpalloc, offset);
        }
    }

    *ppinfo = pcinfo;
    _ASSERT(SUCCEEDED(result));

Finished:

    if(flock)
    {
        lock_unlock(pcache->rplock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in rplist_getinfo", result);

        if(pcinfo != NULL)
        {
            peinfo = pcinfo->entries;
            while(peinfo != NULL)
            {
                ptemp = peinfo;
                peinfo = peinfo->next;

                if(ptemp->pathkey != NULL)
                {
                    alloc_efree(ptemp->pathkey);
                    ptemp->pathkey = NULL;
                }

                if(ptemp->subkey != NULL)
                {
                    alloc_efree(ptemp->subkey);
                    ptemp->subkey = NULL;
                }

                if(ptemp->abspath != NULL)
                {
                    alloc_efree(ptemp->abspath);
                    ptemp->abspath = NULL;
                }

                alloc_efree(ptemp);
                ptemp = NULL;
            }

            alloc_efree(pcinfo);
            pcinfo = NULL;
        }
    }

    dprintverbose("start rplist_getinfo");

    return result;
}

void rplist_freeinfo(rplist_info * pinfo)
{
    rplist_entry_info * peinfo = NULL;
    rplist_entry_info * petemp = NULL;

    if(pinfo != NULL)
    {
        peinfo = pinfo->entries;
        while(peinfo != NULL)
        {
            petemp = peinfo;
            peinfo = peinfo->next;

            if(petemp->pathkey != NULL)
            {
                alloc_efree(petemp->pathkey);
                petemp->pathkey = NULL;
            }

            if(petemp->subkey != NULL)
            {
                alloc_efree(petemp->subkey);
                petemp->subkey = NULL;
            }

            if(petemp->abspath != NULL)
            {
                alloc_efree(petemp->abspath);
                petemp->abspath = NULL;
            }

            alloc_efree(petemp);
            petemp = NULL;
        }

        alloc_efree(pinfo);
        pinfo = NULL;
    }
}

void rplist_runtest()
{
    return;
}
