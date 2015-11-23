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
   | Module: wincache_zvcache.h                                                                   |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#ifndef _WINCACHE_ZVCACHE_H_
#define _WINCACHE_ZVCACHE_H_

typedef struct _zvcache_hashtable_pool_tracker zvcache_hashtable_pool_tracker;
struct _zvcache_hashtable_pool_tracker
{
    size_t        val;                   /* Hashtable *ht; Offset */
    size_t        hoff;                  /* Memory pool offset */
};

typedef struct zvcache_value zvcache_value;
struct zvcache_value
{
    size_t            zvalue;            /* Offset of zval value stored */
    size_t            keystr;            /* Offset of key string */
    unsigned short    keylen;            /* Length of key string */
    size_t            sizeb;             /* Memory allocated for zvalue in bytes */

    unsigned int      add_ticks;         /* Tick count when entry was created */
    unsigned int      use_ticks;         /* Tick count when entry was last used */
    unsigned int      ttlive;            /* Max time to live for this entry */

    unsigned int      hitcount;          /* Hit count for this entry */
    size_t            next_value;        /* Next value with same index */
    size_t            prev_value;        /* Previous value with same index */
};

typedef struct zvcache_header zvcache_header;
struct zvcache_header
{
    unsigned int      mapcount;          /* Number of processes using this zval cache */
    unsigned int      hitcount;          /* Cache items cumulative hit count */
    unsigned int      misscount;         /* Cache items cumulative miss count */
    unsigned int      init_ticks;        /* Tick count when the cache first got created */
    unsigned int      last_owner;        /* PID of last owner for lock */

    unsigned int      lscavenge;         /* Ticks when last scavenger run happened */
    unsigned int      scstart;           /* Index from where scavenger should start */
    unsigned int      itemcount;         /* Number of active items in zval cache */
    unsigned int      valuecount;        /* Total values starting from last entry */
    size_t            values[1];         /* valuecount zvcache_value offsets */
};

typedef struct zvcopy_context zvcopy_context;
struct zvcopy_context
{
    unsigned int      oomcode;           /* Out of memory error code to use */
    void *            palloc;            /* Allocator for alloc_a* functions */
    char *            pbaseadr;          /* Base address of the segment */
    size_t            hoffset;           /* Offset of mpool_header */
    size_t            allocsize;         /* Amount of memory allocated */

    fn_malloc         fnmalloc;          /* Function to use for malloc */
    fn_realloc        fnrealloc;         /* Function to use for realloc */
    fn_strdup         fnstrdup;          /* Function to use for strdup */
    fn_free           fnfree;            /* Function to use for free */
};

typedef struct zvcache_context zvcache_context;
struct zvcache_context
{
    unsigned int      id;                /* unique identifier for cache */
    unsigned int      issession;         /* session cache or user cache */
    unsigned short    islocal;           /* is the cache local or shared */
    unsigned short    cachekey;          /* unique cache key used in names */
    HANDLE            hinitdone;         /* event indicating if memory is initialized */

    zvcopy_context *  incopy;            /* zvcopy context to use for non-array copyin */
    zvcopy_context *  outcopy;           /* zvcopy context to use for all copyout */

    char *            zvmemaddr;         /* base addr of memory segment */
    zvcache_header *  zvheader;          /* zvcache cache header */
    filemap_context * zvfilemap;         /* filemap where zvcache is kept */
    lock_context *    zvlock;            /* lock for zvcache header */
    alloc_context *   zvalloc;           /* alloc context for zvcache segment */
};

typedef struct zvcache_info zvcache_info;
struct zvcache_info
{
    unsigned int      initage;           /* Seconds elapsed after cache init */
    unsigned int      islocal;           /* Is the zvcache local or global */
    unsigned int      hitcount;          /* Cumulative hitcount of zvcache */
    unsigned int      misscount;         /* Cumulative misscount of zvcache */
    unsigned int      itemcount;         /* Number of items including stale ones */
};

typedef struct zvcache_info_entry zvcache_info_entry;
struct zvcache_info_entry
{
    char *            key;               /* cache item key */
    unsigned int      ttl;               /* ttl of this entry */
    unsigned int      age;               /* Age in seconds */
    zend_uchar        type;              /* type of zval which is stored as value */
    size_t            sizeb;             /* memory allocated for zval in bytes */
    unsigned int      hitcount;          /* hitcount for this entry */
};

extern int  zvcache_create(zvcache_context ** ppcache);
extern void zvcache_destroy(zvcache_context * pcache);

extern int  zvcache_initialize(
    zvcache_context * pcache,
    unsigned int issession,
    unsigned short islocal,
    unsigned short cachekey,
    unsigned int zvcount,
    unsigned int cachesize,
    char * shmfilepath
    );

extern void zvcache_terminate(zvcache_context * pcache);

extern int  zvcache_get(zvcache_context * pcache, const char * key, zval ** pvalue);
extern int  zvcache_set(zvcache_context * pcache, const char * key, zval * pzval, unsigned int ttl, unsigned char isadd);
extern int  zvcache_delete(zvcache_context * pcache, const char * key);
extern int  zvcache_clear(zvcache_context * pcache);
extern int  zvcache_exists(zvcache_context * pcache, const char * key, unsigned char * pexists);
extern int  zvcache_list(zvcache_context * pcache, zend_bool summaryonly, char * pkey, zvcache_info * pcinfo, zend_llist * plist);
extern int  zvcache_change(zvcache_context * pcache, const char * key, zend_long delta, zend_long * newvalue);
extern int  zvcache_compswitch(zvcache_context * pcache, const char * key, zend_long oldvalue, zend_long newvalue);

#endif /* _WINCACHE_ZVCACHE_H_ */
