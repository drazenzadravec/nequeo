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
   | Module: wincache_lock.c                                                                      |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#include "precomp.h"

#ifdef _WIN64
#define LOCK_SUFFIX_FORMAT              "_%u_%u_%u_x64_"
#define LOCK_NAMESALT_SUFFIX_FORMAT     "_%u_%s_%u_%u_x64_"
#else  /* not _WIN64 */
#define LOCK_SUFFIX_FORMAT              "_%u_%u_%u_"
#define LOCK_NAMESALT_SUFFIX_FORMAT     "_%u_%s_%u_%u_"
#endif /* _WIN64 */


/* Globals */
unsigned short glockid = 1;

/* Create a new lock context */
/* Call lock_initialize to actually create the lock */
int lock_create(lock_context ** pplock)
{
    int            result = NONFATAL;
    lock_context * plock  = NULL;

    dprintverbose("start lock_create");

    _ASSERT( pplock != NULL );
    *pplock = NULL;

    plock = (lock_context *)alloc_pemalloc(sizeof(lock_context));
    if(plock == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    /* Initialize lock_context structure with default values */
    plock->id         = glockid++;
    plock->type       = LOCK_TYPE_INVALID;
    plock->unused     = 0;
    plock->state      = LOCK_STATE_UNLOCKED;
    plock->nameprefix = NULL;
    plock->namelen    = 0;
    plock->hxlock     = NULL;
    plock->last_owner = NULL;

    /* id is 8 bit field. Keep maximum to 255 */
    if(glockid == 256)
    {
        glockid = 1;
    }

    *pplock = plock;
    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in lock_create", result);
    }

    dprintverbose("end lock_create");

    return result;
}

/* Destroy the lock context */
void lock_destroy(lock_context * plock)
{
    dprintverbose("start lock_destroy");

    if( plock != NULL )
    {
        _ASSERT(plock->nameprefix == NULL);
        _ASSERT(plock->hxlock     == NULL);

        alloc_pefree(plock);
        plock = NULL;
    }

    dprintverbose("end lock_destroy");
}

/* Caller must free ppnew_prefix using alloc_pefree() */
int lock_get_nameprefix(
    char * name,
    unsigned short cachekey,
    unsigned short type,
    char **ppnew_prefix,
    size_t * pcchnew_prefix
    )
{
    int    result  = NONFATAL;
    char * objname = 0;
    size_t namelen = 0;
    int    actual_len = 0;
    int    pid     = 0;
    int    ppid    = 0;
    char * scopePrefix = "";

    /* Get the initial length of name prefix */
    namelen = strlen(name);
    _ASSERT(namelen > 0);

    /* If a namesalt is specified, put _<salt> in the name */
    if(WCG(namesalt) != NULL)
    {
        namelen += strlen(WCG(namesalt)) + 1;
    }

    /* Depending on what type of lock to create, get pid and ppid */
    switch(type)
    {
        case LOCK_TYPE_SHARED:
            ppid = WCG(fmapgdata)->ppid;
            namelen += PID_MAX_LENGTH;
            break;
        case LOCK_TYPE_LOCAL:
            pid = WCG(fmapgdata)->pid;
            namelen += PID_MAX_LENGTH;
            break;
        case LOCK_TYPE_GLOBAL:
            break;
        default:
            result = FATAL_LOCK_INVALID_TYPE;
            goto Finished;
    }

    /* Add 6 for underscores, 1 for null termination, */
    /* LOCK_NUMBER_MAX_STRLEN for adding cachekey, 2 for 0 (for global), */
    /* PHP_WINCACHE_VERSION_LEN, plus the length of the PHP Maj.Min version. */
    namelen  = namelen + LOCK_NUMBER_MAX_STRLEN + 9;
    namelen += sizeof(STRVER2(PHP_MAJOR_VERSION, PHP_MINOR_VERSION));
    namelen += PHP_WINCACHE_VERSION_LEN;

    /* If we're on an app pool, we need to create all named objects in */
    /* the Global scope.  */
    if (WCG(apppoolid))
    {
        scopePrefix = GLOBAL_SCOPE_PREFIX;
        namelen += GLOBAL_SCOPE_PREFIX_LEN;
    }

    /* Add 2 for lock-type padding.  The buffer will be used to tack on
     * an extra char for the lock type by the caller. */
    namelen += 2;

    /* Synchronization object names are limited to MAX_PATH characters */
    if(namelen >= MAX_PATH - 1)
    {
        result = FATAL_LOCK_LONGNAME;
        goto Finished;
    }

    objname = (char *)alloc_pemalloc(namelen + 1);
    if(objname == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    ZeroMemory(objname, namelen + 1);

    /* Create nameprefix as name_pid_ppid_ */
    if(WCG(namesalt) == NULL)
    {
        actual_len = _snprintf_s(objname, namelen + 1, namelen, "%s%s_" STRVER2(PHP_MAJOR_VERSION, PHP_MINOR_VERSION) "_" PHP_WINCACHE_VERSION LOCK_SUFFIX_FORMAT, scopePrefix, name, cachekey, pid, ppid);
    }
    else
    {
        actual_len = _snprintf_s(objname, namelen + 1, namelen, "%s%s_" STRVER2(PHP_MAJOR_VERSION, PHP_MINOR_VERSION) "_" PHP_WINCACHE_VERSION LOCK_NAMESALT_SUFFIX_FORMAT, scopePrefix, name, cachekey, WCG(namesalt), pid, ppid);
    }

    if (-1 == actual_len)
    {
        error_setlasterror();
        result = FATAL_LOCK_LONGNAME;
        goto Finished;
    }

    /* Zero out the trailing portion of the buffer, for safety! */
    ZeroMemory(objname + actual_len, namelen - actual_len);

    *ppnew_prefix   = objname;
    *pcchnew_prefix = actual_len;

Finished:
    return result;
}

/* Initialize the lock context with valid information */
/* lock is not ready to use unless initialize is called */
/*++

Arguments:

    plock           pointer to lock_context to be initialized
    name            unique name for this lock
    cachekey        ?
    type            scope of this lock (Local/Shared/Global)
    plast_owner     Optional pointer to unsigned int that will receive the PID
                    of the process which last successfully acquired the lock.

Return Value:

    NONFATAL                    Success
    FAIL_LOCK_INIT_CREATMUTEX   Failed to create named mutex.  Call error_getlasterror() for more info.

--*/
int lock_initialize(lock_context * plock, char * name, unsigned short cachekey, unsigned short type, unsigned int * plast_owner)
{
    int    result  = NONFATAL;
    char * objname = 0;
    size_t namelen = 0;
    int    pid     = 0;
    int    ppid    = 0;

    dprintverbose("start lock_initialize");

    _ASSERT(plock    != NULL);
    _ASSERT(name     != NULL);
    _ASSERT(cachekey != 0);
    _ASSERT(cachekey <= LOCK_NUMBER_MAXIMUM);
    _ASSERT(type     <= LOCK_TYPE_MAXIMUM);

    if(cachekey > LOCK_NUMBER_MAXIMUM)
    {
        result = FATAL_LOCK_NUMBER_LARGE;
        goto Finished;
    }

    result = lock_get_nameprefix(name, cachekey, type, &objname, &namelen);
    if (FAILED(result))
    {
        goto Finished;
    }

    /* lock_get_nameprefix validates that 'type' is a valid lock type */
    plock->type = type;

    /* Use the name prefix as the lock name */
    plock->nameprefix = objname;
    _ASSERT(namelen < (MAX_PATH));
    plock->namelen = (unsigned short)namelen;

    /* Create mutex which will be used to synchronize access */
    plock->hxlock = CreateMutex(NULL, FALSE, objname);
    if(plock->hxlock == NULL)
    {
        dprintimportant("Failed to create lock %s due to error %u", objname, error_setlasterror());
        result = FATAL_LOCK_INIT_CREATEMUTEX;
        goto Finished;
    }

    plock->state = LOCK_STATE_UNLOCKED;
    plock->last_owner = plast_owner;
    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in lock_initialize", result);

        if(plock->hxlock != NULL)
        {
            CloseHandle(plock->hxlock);
            plock->hxlock = NULL;
        }

        if(plock->nameprefix != NULL)
        {
            alloc_pefree(plock->nameprefix);
            plock->nameprefix = NULL;

            plock->namelen = 0;
        }

        plock->type       = LOCK_TYPE_INVALID;
        plock->last_owner = NULL;
    }

    dprintverbose("end lock_initialize");

    return result;
}

void lock_terminate(lock_context * plock)
{
    dprintverbose("start lock_terminate");

    if(plock != NULL)
    {
        if(plock->hxlock != NULL)
        {
            CloseHandle(plock->hxlock);
            plock->hxlock = NULL;
        }

        if(plock->nameprefix != NULL)
        {
            alloc_pefree(plock->nameprefix);
            plock->nameprefix = NULL;

            plock->namelen = 0;
        }

        plock->type    = LOCK_TYPE_INVALID;
        plock->state   = LOCK_STATE_UNLOCKED;
        plock->last_owner = NULL;
    }

    dprintverbose("end lock_terminate");
}

/* Acquire write lock */
void lock_lock(lock_context * plock)
{
    DWORD  ret       = 0;
    const char *filename;
    uint   lineno;
    unsigned int last_owner = 0;

    dprintverbose("start lock_lock   %p", plock);

    _ASSERT(plock          != NULL);
    _ASSERT(plock->hxlock  != NULL);

    /* Simple case. Just lock hxwrite */
    ret = WaitForSingleObject(plock->hxlock, INFINITE);

    if (plock->last_owner)
    {
        last_owner = *plock->last_owner;
    }

    if (ret == WAIT_ABANDONED)
    {
        error_setlasterror();
        dprintcritical("lock_lock: acquired abandoned mutex %s. Something bad happend in another process!",
                        plock->nameprefix);
        utils_get_filename_and_line(&filename, &lineno);
        EventWriteLockAbandonedMutex(plock->nameprefix,
            last_owner, last_owner, filename, lineno);
        goto Finished;
    }

    if (ret == WAIT_FAILED)
    {
        dprintcritical("lock_lock: Failure waiting on lock %s (%d). Something bad happened!", 
                        plock->nameprefix, error_setlasterror());
        utils_get_filename_and_line(&filename, &lineno);
        EventWriteLockFailedWaitForLock(plock->nameprefix,
            last_owner, last_owner, filename, lineno);
        goto Finished;
    }

    if (plock->last_owner)
    {
        *plock->last_owner = GetCurrentProcessId();
    }

    _ASSERT(plock->state == LOCK_STATE_UNLOCKED);
    plock->state = LOCK_STATE_LOCKED;

Finished:

    dprintverbose("end lock_lock     %p", plock);
    return;
}

/* Release write lock */
void lock_unlock(lock_context * plock)
{
    dprintverbose("start lock_unlock %p", plock);

    _ASSERT(plock          != NULL);
    _ASSERT(plock->hxlock  != NULL);
    _ASSERT(plock->state   == LOCK_STATE_LOCKED);

    /* Simple case. Just release the mutex */
    plock->state = LOCK_STATE_UNLOCKED;
    ReleaseMutex(plock->hxlock);

    dprintverbose("end lock_unlock   %p", plock);
    return;
}

void lock_runtest()
{
    int            result  = NONFATAL;
    unsigned int   last_owner = 0;
    lock_context * plock1  = NULL;
    lock_context * plock2  = NULL;
    unsigned int   pid = GetCurrentProcessId();

    dprintverbose("*** STARTING LOCK TESTS ***");

    /* Create two locks of different types */
    result = lock_create(&plock1);
    if(FAILED(result))
    {
        dprintverbose("lock_create for plock1 failed");
        goto Finished;
    }

    result = lock_create(&plock2);
    if(FAILED(result))
    {
        dprintverbose("lock_create for plock2 failed");
        goto Finished;
    }

    /* Verify IDs of two locks are different */
    _ASSERT(plock1->id != plock2->id);

    /* Initialize first lock */
    result = lock_initialize(plock1, "LOCK_TEST1", 1, LOCK_TYPE_SHARED, &last_owner);
    if(FAILED(result))
    {
        dprintverbose("lock_initialize for plock1 failed");
        goto Finished;
    }

    /* Verify strings and handles got created properly */
    _ASSERT(plock1->namelen   == strlen(plock1->nameprefix));
    _ASSERT(plock1->hxlock    != NULL);

    /* Initialize second lock */
    result = lock_initialize(plock2, "LOCK_TEST2", 1, LOCK_TYPE_LOCAL, NULL);
    if(FAILED(result))
    {
        dprintverbose("lock_intialize for plock2 failed");
        goto Finished;
    }

    /* Verify strings and handles got created properly */
    _ASSERT(plock2->namelen   == strlen(plock2->nameprefix));
    _ASSERT(plock2->hxlock    != NULL);

    /* Get lock on both locks */
    lock_lock(plock1);
    lock_lock(plock2);

    /* Verify reader count for shared read lock */
    _ASSERT(pid == last_owner);

    lock_unlock(plock2);
    lock_unlock(plock1);

    _ASSERT(SUCCEEDED(result));

Finished:

    if(plock1 != NULL)
    {
        lock_terminate(plock1);
        lock_destroy(plock1);

        plock1 = NULL;
    }

    if(plock2 != NULL)
    {
        lock_terminate(plock2);
        lock_destroy(plock2);

        plock2 = NULL;
    }

    dprintverbose("*** ENDING LOCK TESTS ***");

    return;
}
