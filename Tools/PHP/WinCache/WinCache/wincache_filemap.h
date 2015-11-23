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
   | Module: wincache_filemap.h                                                                   |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#ifndef _WINCACHE_FILEMAP_H_
#define _WINCACHE_FILEMAP_H_

/* TBD?? Below are the types of shared memory segments we will create. Their */
/* names will have corresponding prefix. Only one INFORMATION and FILELIST */
/* segment will be there but there will be multiple OPCODES segments. If */
/* FILELIST is finished, we will clean entries which aren't used for a while */

/* TBD?? Maintaining multiple segments will require one segment to not refer */
/* memory from other or would require using parts of pointer values to */
/* calculate segement number. For now I will go with one segment to keep */
/* opcodes. Will look into multiple segments later */

/* TBD?? Should we look at commandline arguments of the process to find what */
/* all processes got launched for a single fastcgi process pool. Using ppid */
/* won't work for per site php.ini configuration. We will end up sharing the */
/* cache with all processes which won't be too bad either */

#define PID_MAX_LENGTH                          6   /* 999999 is the max value */
#define READWRITE_LOCK_MAX_COUNT                20  /* NOT USED */

#define FILEMAP_TYPE_INVALID                    0   /* Invalid value */
#define FILEMAP_TYPE_FILELIST                   1   /* File list map */
#define FILEMAP_TYPE_RESPATHS                   2   /* Resolve path cache */
#define FILEMAP_TYPE_FILECONTENT                3   /* File cache map */
#define FILEMAP_TYPE_BYTECODES                  4   /* Byte code map */
#define FILEMAP_TYPE_USERZVALS                  5   /* User cache map */
#define FILEMAP_TYPE_SESSZVALS                  6   /* Session cache map */
#define FILEMAP_TYPE_UNUSED                     255 /* UNUSED SEGMENT MARKER */
#define FILEMAP_TYPE_MAX                        255

#define FILEMAP_MAP_INVALID                     0   /* Invalid value */
#define FILEMAP_MAP_SRANDOM                     1   /* Map at any random address  */
#define FILEMAP_MAP_SFIXED                      2   /* Try mapping at a particular address */
#define FILEMAP_MAP_LRANDOM                     3   /* Create a local only filemap */

#define FILEMAP_MAX_COUNT                       30
#define FILEMAP_MIN_SIZE                        1048576    /* 1   MB */
#define FILEMAP_MAX_SIZE                        268435456  /* 256 MB */

/* Following prefixes should be less than 254 characters */
#define FILEMAP_INFORMATION_PREFIX              "WINCACHE_" STRVER2(PHP_MAJOR_VERSION, PHP_MINOR_VERSION) "_" PHP_WINCACHE_VERSION "_FILEMAP_INFORMATION"
#define FILEMAP_INFORMATION_PREFIX_LENGTH       (sizeof(FILEMAP_INFORMATION_PREFIX))

#define FILEMAP_FILELIST_PREFIX                 "WINCACHE_" STRVER2(PHP_MAJOR_VERSION, PHP_MINOR_VERSION) "_" PHP_WINCACHE_VERSION "_FILEMAP_FILELIST"
#define FILEMAP_FILELIST_PREFIX_LENGTH          (sizeof(FILEMAP_FILELIST_PREFIX))

#define FILEMAP_RESPATHS_PREFIX                 "WINCACHE_" STRVER2(PHP_MAJOR_VERSION, PHP_MINOR_VERSION) "_" PHP_WINCACHE_VERSION "_FILEMAP_RESPATHS"
#define FILEMAP_RESPATHS_PREFIX_LENGTH          (sizeof(FILEMAP_RESPATHS_PREFIX))

#define FILEMAP_FILECONTENT_PREFIX              "WINCACHE_" STRVER2(PHP_MAJOR_VERSION, PHP_MINOR_VERSION) "_" PHP_WINCACHE_VERSION "_FILEMAP_FILECONTENT"
#define FILEMAP_FILECONTENT_PREFIX_LENGTH       (sizeof(FILEMAP_FILECONTENT_PREFIX))

#define FILEMAP_BYTECODES_PREFIX                "WINCACHE_" STRVER2(PHP_MAJOR_VERSION, PHP_MINOR_VERSION) "_" PHP_WINCACHE_VERSION "_FILEMAP_BYTECODES"
#define FILEMAP_BYTECODES_PREFIX_LENGTH         (sizeof(FILEMAP_BYTECODES_PREFIX))

#define FILEMAP_USERZVALS_PREFIX                "WINCACHE_" STRVER2(PHP_MAJOR_VERSION, PHP_MINOR_VERSION) "_" PHP_WINCACHE_VERSION "_FILEMAP_USERZVALS"
#define FILEMAP_USERZVALS_PREFIX_LENGTH         (sizeof(FILEMAP_USERZVALS_PREFIX))

#define FILEMAP_SESSZVALS_PREFIX                "WINCACHE_" STRVER2(PHP_MAJOR_VERSION, PHP_MINOR_VERSION) "_" PHP_WINCACHE_VERSION "_FILEMAP_SESSZVALS"
#define FILEMAP_SESSZVALS_PREFIX_LENGTH         (sizeof(FILEMAP_SESSZVALS_PREFIX))

/* filemap_information_header - SHARED - 16 bytes */
/* filemap_information_entry  - SHARED - 276 bytes */
/* filemap_global_context     - LOCAL -  8 bytes */
/* filemap_information        - LOCAL -  20 bytes */
/* filemap_context            - LOCAL -  14 bytes (not aligned at 4 bytes) */

typedef struct filemap_information_header filemap_information_header;
struct filemap_information_header
{
    size_t                       size;          /* Total size of the memory map */
    unsigned int                 mapcount;      /* Total times this shared area got mapped */
    unsigned short               entry_count;   /* How many other filemap objects are present */
    unsigned short               maxcount;      /* Maximum number of filemaps which can be created */
};

typedef struct filemap_information_entry filemap_information_entry;
struct filemap_information_entry
{
    unsigned short               fmaptype;      /* Type which tells what is the purpose of this filemap */
    unsigned short               cachekey;      /* short key which uniquely identifies fmap of this type */
    char                         name[MAX_PATH];/* name of filemap */
    size_t                       size;          /* Size of this filemap */
    unsigned int                 mapcount;      /* How many processes mapped this filemap */
    unsigned int                 cpid;          /* ProcessID of process which initially created this map */
    unsigned int                 opid;          /* Current ProcessID which is the owner */
    void *                       mapaddr;       /* Map address where owner has this memory mapped */
};

typedef struct filemap_context filemap_context;
struct filemap_context
{
    unsigned short               id;            /* unique id of this filemap context */
    unsigned short               islocal;       /* tells if specified address couldn't be used */
    filemap_information_entry *  infoentry;     /* pointer to filemap information entry */
    HANDLE                       hshmfile;      /* file handle for file backed shared memory */
    unsigned int                 existing;      /* If value is 1, skip initialization for this */
    HANDLE                       hfilemap;      /* handle to memory map object */
    void *                       mapaddr;       /* local adddress mapped for this process */
};

typedef struct filemap_information filemap_information;
struct filemap_information
{
    HANDLE                       hinfomap;      /* Handle to filemap object */
    char *                       infoname;      /* Name of memory map to store filemap info */
    size_t                       infonlen;      /* Length of name buffer */
    filemap_information_header * header;        /* Mapped memory address to information segment */
    HANDLE                       hinitdone;     /* event inidicating if memory is initialized */
    lock_context *               hlock;         /* Lock object for info filemap */
};

typedef struct filemap_global_context filemap_global_context;
struct filemap_global_context
{
    unsigned int                 pid;           /* PID of this process */
    unsigned int                 ppid;          /* Parent process id */
    filemap_information *        info;          /* pointer to filemap_information */
};

extern int  filemap_global_initialize();
extern void filemap_global_terminate();
extern unsigned int filemap_getpid();
extern unsigned int filemap_getppid();

extern int    filemap_create(filemap_context ** ppfilemap);
extern void   filemap_destroy(filemap_context * pfilemap);

extern int
filemap_initialize(
    filemap_context * pfilemap,
    unsigned short fmaptype,
    unsigned short cachekey,
    unsigned short fmclass,
    unsigned int size_mb,
    unsigned char isfirst,
    char * shmfilepath);

extern void   filemap_terminate(filemap_context * pfilemap);

extern size_t filemap_getsize(filemap_context * pfilemap);
extern unsigned int filemap_getcpid(filemap_context * pfilemap);

extern void   filemap_runtest();

#endif /* _WINCACHE_FILEMAP_H_ */
