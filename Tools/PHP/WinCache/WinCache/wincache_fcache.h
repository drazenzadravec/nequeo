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
   | Module: wincache_fcache.h                                                                    |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#ifndef _WINCACHE_FCACHE_H_
#define _WINCACHE_FCACHE_H_

/* fcache_value      - SHARED */
/* fcache_header     - SHARED */
/* fcache_context    - LOCAL  */
/* fcache_entry_info - LOCAL  */

#define FILE_IS_FOLDER    1
#define FILE_IS_RUNAWARE  2
#define FILE_IS_READABLE  4
#define FILE_IS_WUNAWARE  8
#define FILE_IS_WRITABLE  16

typedef struct fcache_value fcache_value;
struct fcache_value
{
    size_t            file_sec;     /* File security information */
    size_t            file_content; /* Pointer to content saved in shared memory */
    unsigned int      file_size;    /* File size in bytes (Max is 2 MB) */
    unsigned int      file_flags;   /* Boolean information about this file */
    unsigned int      is_deleted;   /* If 1, this entry is marked for deletion */
    unsigned int      hitcount;     /* Number of times this file entry got used */
    unsigned int      refcount;     /* How many processes are currently using this */
};

typedef struct fcache_header fcache_header;
struct fcache_header
{
    unsigned int      mapcount;     /* Number of processes using this file cache */
    unsigned int      itemcount;    /* Number of items in file cache */
    unsigned int      hitcount;     /* Cache items cumulative hit count */
    unsigned int      misscount;    /* Cache items cumulative miss count */
};

typedef struct fcache_context fcache_context;
struct fcache_context
{
    unsigned int      id;           /* unique identifier for cache */
    unsigned short    islocal;      /* is cache local to process */
    unsigned short    cachekey;     /* unique cache key used in names */
    HANDLE            hinitdone;    /* event indicating if memory is initialized */
    unsigned int      maxfsize;     /* maximum size of file in bytes */
    char *            memaddr;      /* base memory address of segment */
    fcache_header *   header;       /* cache header */
    filemap_context * pfilemap;     /* filemap where file content is kept */
    lock_context *    prwlock;      /* writer lock for filecache header */
    alloc_context *   palloc;       /* alloc context for filecache segment */
};

typedef struct fcache_handle fcache_handle;
struct fcache_handle
{
    php_stream        wrapper;      /* Dummy php_stream wrapper, MUST BE FIRST! */
    fcache_context *  pfcache;      /* Cache context for this process */
    fcache_value *    pfvalue;      /* Cache value which has file content */
    size_t            len;          /* Length of file in bytes */
    size_t            pos;          /* Current position where read is active */
    void *            map;          /* Address where file content is present */
    char *            buf;          /* Buffer pointing to file content */
};

typedef struct fcache_entry_info fcache_entry_info;
struct fcache_entry_info
{
    unsigned int      filesize;     /* Size of file content */
    unsigned int      hitcount;     /* Hit count for this file entry */
};

extern int  fcache_create(fcache_context ** ppfcache);
extern void fcache_destroy(fcache_context * pfcache);
extern int  fcache_initialize(fcache_context * pfcache, unsigned short islocal, unsigned short cachekey, unsigned int cachesize, unsigned int maxfsize);
extern void fcache_terminate(fcache_context * pfcache);

extern int  fcache_createval(fcache_context * pfcache, const char * filename, fcache_value ** ppvalue);
extern void fcache_destroyval(fcache_context * pfcache, fcache_value * pvalue);
extern int  fcache_useval(fcache_context * pcache, const char * filename, fcache_value * pvalue, zend_file_handle ** pphandle);
extern fcache_value * fcache_getvalue(fcache_context * pfcache, size_t offset);
extern size_t fcache_getoffset(fcache_context * pfcache, fcache_value * pvalue);
extern void fcache_refinc(fcache_context * pfcache, fcache_value * pvalue);
extern void fcache_refdec(fcache_context * pfcache, fcache_value * pvalue);
extern int  fcache_getinfo(fcache_value * pvalue, fcache_entry_info ** ppinfo);
extern void fcache_freeinfo(fcache_entry_info * pinfo);

extern size_t fcache_fsizer(void * handle);
extern size_t fcache_reader(void * handle, char * buf, size_t length);
extern void   fcache_closer(void * handle);

extern void fcache_runtest();

#endif /* _WINCACHE_FCACHE_H_ */
