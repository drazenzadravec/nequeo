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
   | Module: wincache_fcnotify.h                                                                  |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   +----------------------------------------------------------------------------------------------+
*/

#ifndef _WINCACHE_FCNOTIFY_H_
#define _WINCACHE_FCNOTIFY_H_

/* fcnotify_value   - LOCAL */
/* fcnotify_context - LOCAL */

struct fcnotify_value;
typedef struct fcnotify_value fcnotify_value;

typedef struct fcnotify_listen fcnotify_listen;
struct fcnotify_listen
{
    unsigned int            lrefcount;     /* reference count for fcnotify_listen */
    unsigned int            stopcalled;    /* indicates if handle is getting closed */
    fcnotify_value *        pfcnvalue;     /* fcnotify_value with pointer to this */
    char *                  folder_path;   /* path to folder excluding '\\' */
    HANDLE                  folder_handle; /* handle to folder */
    OVERLAPPED              overlapped;    /* overlapped structure */
    BYTE                    fninfo[1024];  /* file notify information */
};

typedef struct fcnotify_value fcnotify_value;
struct fcnotify_value
{
    size_t                  folder_path;   /* folder path */
    unsigned int            owner_pid;     /* pid listening to changes */
    unsigned int            palivechk;     /* last process alive check tickcount */
    unsigned int            reusecount;    /* count of reuses after handoff */
    FILETIME                listen_time;   /* system time when listener was activated */
    fcnotify_listen *       plistener;     /* listener information */
    unsigned int            refcount;      /* number of aplist entries for this folder */
    size_t                  prev_value;    /* previous aplist_value offset */
    size_t                  next_value;    /* next aplist_value offset */
};

typedef struct fcnotify_header fcnotify_header;
struct fcnotify_header
{
    unsigned int            last_owner;    /* PID of last process to acquire the fcnotify_context lock */
    unsigned int            itemcount;     /* Folders count which have active listeners */
    unsigned int            valuecount;    /* Total number of entries in entries */
    size_t                  values[1];     /* HashTable for fcnotify_value entries */
};

typedef struct fcnotify_context fcnotify_context;
struct fcnotify_context
{
    unsigned short          islocal;       /* Is local or shared */
    unsigned char           isshutting;    /* True means process is shutting down */
    unsigned char           iswow64;       /* Take care of wow64 when handling paths */
    unsigned int            processid;     /* Process id of this process */
    unsigned int            lscavenge;     /* Last scavenger run for this process */
    unsigned int            ttlticks;      /* How frequently scavenger should run */

    char *                  fcmemaddr;     /* Memory address where data is stored */
    fcnotify_header *       fcheader;      /* Pointer to fcnotify_header */
    alloc_context *         fcalloc;       /* Allocator to be used for fcnotify */
    lock_context *          fclock;        /* Lock to deal with data in shared memory */
    void *                  fcaplist;      /* Aplist which receives change notification */

    HANDLE                  listen_thread; /* Change listener thread handle */
    HANDLE                  port_handle;   /* Completion port for change notifications */
    HashTable *             pidhandles;    /* Key = processid, value = process handle */
};

typedef struct fcnotify_entry_info fcnotify_entry_info;
struct fcnotify_entry_info
{
    char *                  folderpath;    /* folder path for which fcn is active */
    unsigned int            ownerpid;      /* owner pid listening to changes */
    unsigned int            filecount;     /* Number of files under this folder */
    fcnotify_entry_info *   next;          /* next entry */
};

typedef struct fcnotify_info fcnotify_info;
struct fcnotify_info
{
    unsigned int            itemcount;     /* Total number of items in subcache */
    fcnotify_entry_info *   entries;       /* Individual entries */
};

extern int  fcnotify_create(fcnotify_context ** ppnotify);
extern void fcnotify_destroy(fcnotify_context * pnotify);
extern int  fcnotify_initialize(fcnotify_context * pnotify, unsigned short islocal, void * paplist, alloc_context * palloc, unsigned int filecount);
extern void fcnotify_initheader(fcnotify_context * pnotify, unsigned int filecount);
extern void fcnotify_terminate(fcnotify_context * pnotify);

extern int  fcnotify_check(fcnotify_context * pnotify, const char * filepath, size_t * poffset, unsigned int * pcount);
extern void fcnotify_close(fcnotify_context * pnotify, size_t * poffset, unsigned int * pcount);
extern int  fcnotify_getinfo(fcnotify_context * pcache, zend_bool summaryonly, fcnotify_info ** ppinfo);
extern void fcnotify_freeinfo(fcnotify_info * pinfo);

extern int fcnotify_listenerexists(fcnotify_context *pnotify, const char * folderpath, unsigned char* listenerexists);

extern void fcnotify_runtest();

#endif /* _WINCACHE_FCNOTIFY_H_ */
