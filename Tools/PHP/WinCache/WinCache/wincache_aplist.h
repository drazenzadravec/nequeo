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
   | Module: wincache_aplist.h                                                                    |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#ifndef _WINCACHE_APLIST_H_
#define _WINCACHE_APLIST_H_

/* aplist_value   - SHARED */
/* aplist_header  - SHARED */
/* aplist_context - LOCAL */

#define USE_STREAM_OPEN_CHECK         1
#define SKIP_STREAM_OPEN_CHECK        0

typedef struct aplist_value aplist_value;
struct aplist_value
{
    size_t          file_path;      /* Full path to file */
    FILETIME        modified_time;  /* Modified time of file */
    unsigned int    attributes;     /* File attributes */
    unsigned int    file_size;      /* File size in bytes */

    unsigned int    add_ticks;      /* Ticks when this entry was created */
    unsigned int    use_ticks;      /* Ticks when this entry was last used */
    unsigned int    last_check;     /* Ticks when last file change check was made */
    unsigned short  is_deleted;     /* If set to 1, value is marked for deletion */
    unsigned short  is_changed;     /* If set to 1, value is marked changed */

    size_t          fcacheval;      /* File cache value offset */
    size_t          resentry;       /* Offset of first entry in rplist */
    size_t          fcnotify;       /* Offset of file change notification entry */
    unsigned int    fcncount;       /* fcnotify_value reusecount to detect handoff */
    size_t          prev_value;     /* previous aplist_value offset */
    size_t          next_value;     /* next aplist_value offset */
};

typedef struct aplist_header aplist_header;
struct aplist_header
{
    unsigned int       mapcount;    /* Number of processes using this file cache */
    unsigned int       init_ticks;  /* Tick count when the cache first got created */
    unsigned int       last_owner;  /* PID of last owner of lock */

    unsigned int       ttlmax;      /* Max time a file can stay w/o being used */
    unsigned int       scfreq;      /* How frequently should scavenger run */
    unsigned int       lscavenge;   /* Ticks when last scavenger run happened */
    unsigned int       scstart;     /* Index from where scavenger should start */

    unsigned int       itemcount;   /* Number of valid items */
    unsigned int       valuecount;  /* Total values starting from last entry */
    size_t             values[1];   /* Valuecount aplist_value offsets */
};

typedef struct aplist_context aplist_context;
struct aplist_context
{
    unsigned short     id;          /* Unique identifier for cache */
    unsigned short     islocal;     /* islocal value for lock/alloc/filemap */
    unsigned short     apctype;     /* Is the list shared or separate */
    unsigned short     scstatus;    /* Indicates if scavenger is active or not */
    HANDLE             hinitdone;   /* Event indicating if memory is initialized */
    unsigned int       fchangefreq; /* File change check frequency in mseconds */

    char *             apmemaddr;   /* Base addr of memory segment */
    aplist_header *    apheader;    /* Aplist cache header */
    filemap_context *  apfilemap;   /* Filemap where aplist is kept */
    lock_context *     aplock;      /* Lock for aplist header */
    alloc_context *    apalloc;     /* Alloc context for aplist segment */

    rplist_context *   prplist;     /* Resolve path cache to resolve all paths */
    fcache_context *   pfcache;     /* File cache containing file content */
    fcnotify_context * pnotify;     /* File change notification context */
    int                resnumber;   /* Resource number for this extension */
};

typedef struct cache_entry_info cache_entry_info;
struct cache_entry_info
{
    char *             filename;    /* File name */
    unsigned int       addage;      /* Seconds elapsed after add */
    unsigned int       useage;      /* Seconds elapsed after last use */
    unsigned int       lchkage;     /* Seconds elapsed after last check */
    void *             cdata;       /* Custom data for file cache */
    cache_entry_info * next;        /* Next entry */
};

typedef struct cache_info cache_info;
struct cache_info
{
    unsigned int       initage;     /* Seconds elapsed after cache init */
    unsigned int       islocal;     /* Is the aplist local or global */
    unsigned int       itemcount;   /* Total number of items in subcache */
    unsigned int       hitcount;    /* Hit count of subcache */
    unsigned int       misscount;   /* Miss count of subcache */
    cache_entry_info * entries;     /* Individual entries */
};

extern int  aplist_create(aplist_context ** ppcache);
extern void aplist_destroy(aplist_context * pcache);
extern int  aplist_initialize(aplist_context * pcache, unsigned short apctype, unsigned int filecount, unsigned int fchangefreq, unsigned int ttlmax);
extern void aplist_terminate(aplist_context * pcache);

extern void aplist_setsc_olocal(aplist_context * pcache, aplist_context * plocal);
extern int  aplist_getinfo(aplist_context * pcache, unsigned char type, zend_bool summaryonly, cache_info ** ppinfo);
extern void aplist_freeinfo(unsigned char type, cache_info * pinfo);
extern int  aplist_getentry(aplist_context * pcache, const char * filename, unsigned int findex, aplist_value ** ppvalue);
extern int  aplist_force_fccheck(aplist_context * pcache, zval * filelist);
extern void aplist_mark_changed(aplist_context * pcache, char * folderpath, char * filename);
extern void aplist_mark_file_changed(aplist_context * pcache, char * filepath);

extern int  aplist_fcache_initialize(aplist_context * plcache, unsigned int size, unsigned int maxfilesize);
extern int  aplist_fcache_get(aplist_context * pcache, const char * filename, unsigned char usesopen, char ** ppfullpath, fcache_value ** ppvalue);
extern int  aplist_fcache_use(aplist_context * pcache, const char * fullpath, fcache_value * pvalue, zend_file_handle ** pphandle);
extern void aplist_fcache_close(aplist_context * pcache, fcache_value * pvalue);
extern int  aplist_fcache_delete(aplist_context * pcache, const char * filename);

extern void aplist_runtest();

#endif /* _WINCACHE_APLIST_H_ */

