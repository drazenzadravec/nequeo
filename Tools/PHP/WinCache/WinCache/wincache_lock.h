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
   | Module: wincache_lock.h                                                                      |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#ifndef _WINCACHE_LOCK_H_
#define _WINCACHE_LOCK_H_

#define PID_MAX_LENGTH          6     /* 999999 being maximum */
#define LOCK_WAIT_TIMEOUT       200   /* Wait timeout is 200 ms */
#define LOCK_NUMBER_MAXIMUM     65535 /* Maximum number of instances for same type */
#define LOCK_NUMBER_MAX_STRLEN  5     /* 65535 is the maximum cachekey supported */

#define LOCK_TYPE_INVALID       0     /* Invalid type */
#define LOCK_TYPE_SHARED        1     /* Lock shared by child processes. Use PPID */
#define LOCK_TYPE_LOCAL         2     /* Lock used by this process. Use PID in name */
#define LOCK_TYPE_GLOBAL        3     /* Global lock. Use exact name as specified */
#define LOCK_TYPE_MAXIMUM       7     /* type is a 3 bit field. Max value is 7 */

#define LOCK_STATE_UNLOCKED     0     /* Lock is not acquired */
#define LOCK_STATE_LOCKED       1     /* Lock is acquired */
#define LOCK_STATE_MAXIMUM      1     /* state is a 1 bit field. Max value is 1 */

/* lock_context - LOCAL - 32 bytes */
/* last_owner - SHARED */

typedef struct lock_context lock_context;
struct lock_context
{
    unsigned       id:8;              /* Unique identifier for the lock */
    unsigned       type:3;            /* Type of lock (shared/local/global) */
    unsigned       unused:3;          /* padding */
    unsigned       state:2;           /* Current state of the lock for debugging */
    unsigned       namelen:16;        /* length of name buffers */

    char *         nameprefix;        /* Name prefix to use for named objects */
    HANDLE         hxlock;            /* Handle to mutex to prevent multiple writers */
    unsigned int * last_owner;        /* Pointer to shared memory which has the last owner PID */
};

extern int  lock_create(lock_context ** pplock);
extern void lock_destroy(lock_context * plock);

extern int lock_get_nameprefix(
    char * name,
    unsigned short cachekey,
    unsigned short type,
    char **ppnew_prefix,
    size_t * pcchnew_prefix
    );
extern int  lock_initialize(lock_context * plock, char * name, unsigned short cachekey, unsigned short type, unsigned int * plast_owner);
extern void lock_terminate(lock_context * plock);

extern void lock_lock(lock_context * plock);
extern void lock_unlock(lock_context * plock);

extern void lock_runtest();

#endif /* _WINCACHE_LOCK_H_ */
