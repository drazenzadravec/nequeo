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
   | Module: wincache_alloc.h                                                                     |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#ifndef _WINCACHE_ALLOC_H_
#define _WINCACHE_ALLOC_H_

/* Full fledged memory allocator to allocate memory from shared memory segments */
/* Has ability to manage shared memory in memory pools in which case */
/* free is only supported for the pool and not for individual allocation */
/* Also wrappers for emalloc, efree, pemalloc and pefree */

#define ALLOC_TYPE_INVALID              0
#define ALLOC_TYPE_SHAREDMEM            1
#define ALLOC_TYPE_PROCESS              2
#define ALLOC_TYPE_PROCESS_PERSISTENT   3

/* alloc_context        - LOCAL */
/* alloc_info           - LOCAL */
/* alloc_segment_header - SHARED */
/* alloc_free_header    - SHARED */
/* alloc_used_header    - SHARED */
/* alloc_mpool_header   - SHARED */
/* alloc_mpool_segment  - SHARED */

typedef void * (*fn_malloc)(void * palloc, size_t hoffset, size_t size);
typedef void * (*fn_realloc)(void * palloc, size_t hoffset, void * addr, size_t size);
typedef char * (*fn_strdup)(void * palloc, size_t hoffset, const char * str);
typedef void   (*fn_free)(void * palloc, size_t hoffset, void * addr);

/* TBD? Right now I am doing a custom implementation which will probably work */
/* best for the problem at hand. Try a quick fit memory allocator later. */

typedef struct alloc_segment_header alloc_segment_header;
struct alloc_segment_header
{
    unsigned int mapcount;    /* How many processes mapped this segment */
    unsigned int usedcount;   /* Number of used blocks. Determine fragmentation */
    unsigned int freecount;   /* Number of free blocks Determine when to defrag */
    unsigned int last_owner;  /* PID of last process to acquire the lock for this segment */
    size_t       total_size;  /* Total size of shared memory segment */
    size_t       free_size;   /* Bytes left to be allocated */
    size_t       cacheheader1;/* Offset to memory which contains 1st cache_header */
    size_t       cacheheader2;/* Offset to memory which contains 2nd cache header */
};

/* Each free block will have following information */
/* Each allocation will be appended by size in end as well */
typedef struct alloc_free_header alloc_free_header;
struct alloc_free_header
{
    size_t       size;        /* size of free segment including this header */
    unsigned int is_free;     /* marker which differentiate this block from used block */
    size_t       prev_free;   /* offset to previous free block */
    size_t       next_free;   /* offset to next free block */
    /* free block will have size written in the end */
};

typedef struct alloc_used_header alloc_used_header;
struct alloc_used_header
{
    size_t       size;        /* size of this used block */
    unsigned int is_free;     /* marker which differentiate this block from free block */
    size_t       dummy1;      /* Dummy variable to make size equal to free header */
    size_t       dummy2;      /* Dummy variable to make size equal to free header */
    /* used block will have size written in the end */
};

typedef struct alloc_mpool_header alloc_mpool_header;
struct alloc_mpool_header
{
    size_t       foffset;     /* First memory chunk offset */
    size_t       loffset;     /* Last memory chunk offset */
    size_t       socurr;      /* Current chunk for small allocations */
    size_t       mocurr;      /* Current chunk for medium allocations */
    size_t       locurr;      /* Current chunk for large allocations */
};

typedef struct alloc_mpool_segment alloc_mpool_segment;
struct alloc_mpool_segment
{
    size_t       aoffset;     /* Address of this memory chunk offset */
    size_t       size;        /* Size of this memory chunk */
    size_t       position;    /* Current position where allocation can be made */
    size_t       next;        /* Next chunk of memory offset */
};

typedef struct alloc_context alloc_context;
struct alloc_context
{
    unsigned short         id;        /* Unique id of this alloc_context */
    unsigned short         islocal;   /* Is this allocator local to this process */
    HANDLE                 hinitdone; /* event to indicate if memory is initialized */
    void *                 memaddr;   /* Shared memory segment which is manipulated */
    size_t                 size;      /* Size of memory segment to be allocated */
    lock_context *         lock;      /* Lock to acquire before manipulating segment */
    alloc_segment_header * header;    /* Address of header for this alloc_context */
    unsigned int           localheap; /* Set to 1 when localheap should be used */
};

typedef struct alloc_info alloc_info;
struct alloc_info
{
    size_t       total_size;  /* total size of memory segment */
    size_t       free_size;   /* free memory */
    unsigned int usedcount;   /* number of used blocks */
    unsigned int freecount;   /* number of free blocks */
    size_t       mem_overhead;/* overhead of used/free headers */
};

extern int    alloc_create(alloc_context ** ppalloc);
extern void   alloc_destroy(alloc_context * palloc);
extern int    alloc_initialize(alloc_context * palloc, unsigned short islocal, char * name, unsigned short cachekey, void * staddr, size_t size, unsigned char initmemory);
extern void   alloc_terminate(alloc_context * palloc);
extern int    alloc_create_mpool(alloc_context * palloc, size_t * phoffset);
extern void   alloc_free_mpool(alloc_context * palloc, size_t hoffset);

extern void * alloc_get_cacheheader(alloc_context * palloc, unsigned int msize, unsigned int type);
extern void * alloc_get_cachevalue(alloc_context * palloc, size_t offset);
extern size_t alloc_get_valueoffset(alloc_context * palloc, void * value);
extern int    alloc_getinfo(alloc_context * palloc, alloc_info ** ppinfo);
extern void   alloc_freeinfo(alloc_info * pinfo);

extern void * alloc_emalloc(size_t size);
extern void * alloc_pemalloc(size_t size);
extern void * alloc_smalloc(alloc_context * palloc, size_t size);

extern void * alloc_erealloc(void * addr, size_t size);
extern void * alloc_perealloc(void * addr, size_t size);
extern void * alloc_srealloc(alloc_context * palloc, void * addr, size_t size);

extern char * alloc_estrdup(const char * str);
extern char * alloc_pestrdup(const char * str);
extern char * alloc_sstrdup(alloc_context * palloc, const char * str);

extern void   alloc_efree(void * addr);
extern void   alloc_pefree(void * addr);
extern void   alloc_sfree(alloc_context * palloc, void * addr);

extern void * alloc_oemalloc(alloc_context * palloc, size_t hoffset, size_t size);
extern void * alloc_osmalloc(alloc_context * palloc, size_t hoffset, size_t size);
extern void * alloc_ommalloc(alloc_context * palloc, size_t hoffset, size_t size);
extern void * alloc_oerealloc(alloc_context * palloc, size_t hoffset, void * addr, size_t size);
extern void * alloc_osrealloc(alloc_context * palloc, size_t hoffset, void * addr, size_t size);
extern void * alloc_omrealloc(alloc_context * palloc, size_t hoffset, void * addr, size_t size);
extern char * alloc_oestrdup(alloc_context * palloc, size_t hoffset, const char * str);
extern char * alloc_osstrdup(alloc_context * palloc, size_t hoffset, const char * str);
extern char * alloc_omstrdup(alloc_context * palloc, size_t hoffset, const char * str);
extern void   alloc_oefree(alloc_context * palloc, size_t hoffset, void * addr);
extern void   alloc_osfree(alloc_context * palloc, size_t hoffset, void * addr);
extern void   alloc_omfree(alloc_context * palloc, size_t hoffset, void * addr);

extern void   alloc_runtest();

#endif /* _WINCACHE_ALLOC_H_ */
