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
   | Module: wincache_error.h                                                                     |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#ifndef _WINCACHE_FATAL_H_
#define _WINCACHE_FATAL_H_

/* Basic error codes */
#define NONFATAL                            0
#define FATAL                               1

/* General purpose error codes */
#define FATAL_OUT_OF_MEMORY                 10
#define FATAL_OUT_OF_LMEMORY                10
#define FATAL_OUT_OF_SMEMORY                11
#define FATAL_ACCESS_DENIED                 12
#define FATAL_FILE_NOT_FOUND                13
#define FATAL_INVALID_DATA                  14
#define FATAL_INVALID_ARGUMENT              15
#define FATAL_NEED_MORE_MEMORY              16
#define FATAL_UNEXPECTED_DATA               17
#define FATAL_UNEXPECTED_FCALL              18
#define FATAL_CRYPTO_FAILED                 19
#define FATAL_ACL_FAILED                    20

#define FATAL_OPEN_TOKEN                    97
#define FATAL_ZEND_BAILOUT                  98
#define FATAL_UNKNOWN_FATAL                 99

/* Error codes used by lock functionality */
#define FATAL_LOCK_BASE                     100
#define FATAL_LOCK_INIT_CREATEMUTEX         FATAL_LOCK_BASE + 1
#define FATAL_LOCK_INIT_CREATEEVENT         FATAL_LOCK_BASE + 2
#define FATAL_LOCK_LONGNAME                 FATAL_LOCK_BASE + 3
#define FATAL_LOCK_SHORT_BUFFER             FATAL_LOCK_BASE + 4
#define FATAL_LOCK_NUMBER_LARGE             FATAL_LOCK_BASE + 5
#define FATAL_LOCK_INVALID_TYPE             FATAL_LOCK_BASE + 6
#define FATAL_LOCK_NOT_FOUND                FATAL_LOCK_BASE + 7

/* Error codes used in filemap functionality */
#define FATAL_FILEMAP_BASE                  200
#define FATAL_FILEMAP_CREATION              FATAL_FILEMAP_BASE + 1
#define FATAL_FILEMAP_INFOCREATE            FATAL_FILEMAP_BASE + 2
#define FATAL_FILEMAP_INFOMAP               FATAL_FILEMAP_BASE + 3
#define FATAL_FILEMAP_CREATE_SNAPSHOT       FATAL_FILEMAP_BASE + 4
#define FATAL_FILEMAP_INITIALIZE            FATAL_FILEMAP_BASE + 5
#define FATAL_FILEMAP_NOFREE                FATAL_FILEMAP_BASE + 6
#define FATAL_FILEMAP_INIT_EVENT            FATAL_FILEMAP_BASE + 7
#define FATAL_FILEMAP_MAPVIEW               FATAL_FILEMAP_BASE + 8
#define FATAL_FILEMAP_CREATEFILE            FATAL_FILEMAP_BASE + 9
#define FATAL_FILEMAP_GETFILETIME           FATAL_FILEMAP_BASE + 10
#define FATAL_FILEMAP_SYSTOFILE             FATAL_FILEMAP_BASE + 11
#define FATAL_FILEMAP_CREATEFILEMAP         FATAL_FILEMAP_BASE + 12
#define FATAL_FILEMAP_ALLOC_LOCALFILEMAP    FATAL_FILEMAP_BASE + 13
#define FATAL_FILEMAP_REVERT_FAILED         FATAL_FILEMAP_BASE + 14
#define FATAL_FILEMAP_IMPERSONATE_FAILED    FATAL_FILEMAP_BASE + 15

/* Error codes used by shared memory allocator */
#define FATAL_ALLOC_BASE                    300
#define FATAL_ALLOC_NO_MEMORY               FATAL_ALLOC_BASE + 1
#define FATAL_ALLOC_INIT_EVENT              FATAL_ALLOC_BASE + 2
#define FATAL_ALLOC_SEGMENT_CORRUPT         FATAL_ALLOC_BASE + 3

/* Error codes used by aplist code */
#define FATAL_APLIST_BASE                   400
#define FATAL_APLIST_CREATION               FATAL_APLIST_BASE + 1
#define FATAL_APLIST_INIT_EVENT             FATAL_APLIST_BASE + 2
#define FATAL_APLIST_OCACHE_INIT_EVENT      FATAL_APLIST_BASE + 3

/* Error codes used by rplist code */
#define FATAL_RPLIST_BASE                   500
#define FATAL_RPLIST_CREATION               FATAL_RPLIST_BASE + 1
#define FATAL_RPLIST_INITIALIZE             FATAL_RPLIST_BASE + 2

/* Error codes used by file cache */
#define FATAL_FCACHE_BASE                   600
#define FATAL_FCACHE_CREATION               FATAL_FCACHE_BASE + 1
#define FATAL_FCACHE_CREATEFILE             FATAL_FCACHE_BASE + 2
#define FATAL_FCACHE_GETFILETYPE            FATAL_FCACHE_BASE + 3
#define FATAL_FCACHE_GETFILESIZE            FATAL_FCACHE_BASE + 4
#define FATAL_FCACHE_READFILE               FATAL_FCACHE_BASE + 5
#define FATAL_FCACHE_INITIALIZE             FATAL_FCACHE_BASE + 6
#define FATAL_FCACHE_FILEINFO               FATAL_FCACHE_BASE + 7
#define FATAL_FCACHE_FILECHANGED            FATAL_FCACHE_BASE + 8
#define FATAL_FCACHE_INIT_EVENT             FATAL_FCACHE_BASE + 9
#define FATAL_FCACHE_ORIGINAL_OPEN          FATAL_FCACHE_BASE + 10
#define FATAL_FCACHE_BYHANDLE_INFO          FATAL_FCACHE_BASE + 11
#define FATAL_FCACHE_FILE_TOO_BIG           FATAL_FCACHE_BASE + 12

/* Error codes used by opcode cache */
#define FATAL_OCACHE_BASE                   700
#define FATAL_OCACHE_CREATION               FATAL_OCACHE_BASE + 1
#define FATAL_OCACHE_INITIALIZE             FATAL_OCACHE_BASE + 2
#define FATAL_OCACHE_FIND                   FATAL_OCACHE_BASE + 3
#define FATAL_OCACHE_ADD                    FATAL_OCACHE_BASE + 4
#define FATAL_OCACHE_DELETE                 FATAL_OCACHE_BASE + 5
#define FATAL_OCACHE_INIT_EVENT             FATAL_OCACHE_BASE + 6
#define FATAL_OCACHE_ORIGINAL_COMPILE       FATAL_OCACHE_BASE + 7

/* Error codes used by opcopy code */
#define FATAL_OPCOPY_BASE                   800
#define FATAL_OPCOPY_ZVAL                   FATAL_OPCOPY_BASE + 1
#define FATAL_OPCOPY_ZNODE                  FATAL_OPCOPY_BASE + 2
#define FATAL_OPCOPY_ZEND_ARG_INFO          FATAL_OPCOPY_BASE + 3
#define FATAL_OPCOPY_ZEND_PROPERTY_INFO     FATAL_OPCOPY_BASE + 4
#define FATAL_OPCOPY_ZEND_FUNCTION          FATAL_OPCOPY_BASE + 5
#define FATAL_OPCOPY_ZEND_CLASS             FATAL_OPCOPY_BASE + 6
#define FATAL_OPCOPY_ZEND_CLASS_ENTRY       FATAL_OPCOPY_BASE + 7

/* Error codes used by zvcache */
#define FATAL_ZVCACHE_BASE                  900
#define FATAL_ZVCACHE_INITIALIZE            FATAL_ZVCACHE_BASE + 1
#define FATAL_ZVCACHE_INIT_EVENT            FATAL_ZVCACHE_BASE + 2
#define FATAL_ZVCACHE_INVALID_ZVAL          FATAL_ZVCACHE_BASE + 3
#define FATAL_ZVCACHE_INVALID_KEY_LENGTH    FATAL_ZVCACHE_BASE + 4

/* Error codes used by session handler */
#define FATAL_SESSION_BASE                  1000
#define FATAL_SESSION_INITIALIZE            FATAL_SESSION_BASE + 1
#define FATAL_SESSION_PATHLONG              FATAL_SESSION_BASE + 2
#define FATAL_SESSION_EMPTY_ID              FATAL_SESSION_BASE + 3

/* Error codes used by file change notification */
#define FATAL_FCNOTIFY_BASE                 1100
#define FATAL_FCNOTIFY_INITIALIZE           FATAL_FCNOTIFY_BASE + 1
#define FATAL_FCNOTIFY_OPENPROCESS          FATAL_FCNOTIFY_BASE + 2
#define FATAL_FCNOTIFY_CREATEFILE           FATAL_FCNOTIFY_BASE + 3
#define FATAL_FCNOTIFY_COMPPORT             FATAL_FCNOTIFY_BASE + 4
#define FATAL_FCNOTIFY_RDCFAILURE           FATAL_FCNOTIFY_BASE + 5

/* Warning codes */
#define WARNING_COMMON_BASE                 5000
#define WARNING_FCACHE_TOOBIG               WARNING_COMMON_BASE + 1
#define WARNING_OPCOPY_MISSING_PARENT       WARNING_COMMON_BASE + 2
#define WARNING_FILEMAP_MAPVIEW             WARNING_COMMON_BASE + 3
#define WARNING_ZVCACHE_EMISSING            WARNING_COMMON_BASE + 4
#define WARNING_ZVCACHE_EXISTS              WARNING_COMMON_BASE + 5
#define WARNING_ZVCACHE_NOTLONG             WARNING_COMMON_BASE + 6
#define WARNING_ZVCACHE_ARGUMENT            WARNING_COMMON_BASE + 7
#define WARNING_ZVCACHE_RESCOPYIN           WARNING_COMMON_BASE + 8
#define WARNING_ZVCACHE_CASNEQ              WARNING_COMMON_BASE + 9
#define WARNING_LOCK_IGNORE                 WARNING_COMMON_BASE + 10
#define WARNING_FCNOTIFY_FORCECHECK         WARNING_COMMON_BASE + 11
#define WARNING_ORESOLVE_FAILURE            WARNING_COMMON_BASE + 12
#define WARNING_APPPOOL_NAME_NOT_FOUND      WARNING_COMMON_BASE + 13

/* SUCCEEDED and FAILED macros */
#ifdef SUCCEEDED
#undef SUCCEEDED
#endif
#define SUCCEEDED(r)                        ((r == NONFATAL) ? 1 : 0)
#ifdef FAILED
#undef FAILED
#endif
#define FAILED(r)                           ((r != NONFATAL) ? 1 : 0)

typedef struct error_context error_context;
struct error_context
{
    unsigned int    error_code;
    unsigned char * error_message;
};

extern unsigned int error_getlasterror();
extern unsigned int error_setlasterror();
extern char * error_gethrmessage();
extern char * error_getmessage(unsigned int error_code);

#endif /* _WINCACHE_FATAL_H_ */
