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
   | Module: wincache_error.c                                                                     |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   +----------------------------------------------------------------------------------------------+
*/

#include "precomp.h"

error_context wincache_errors[] =
{
    { FATAL_OUT_OF_MEMORY, "Fatal: Out of memory error" },
    { FATAL_OUT_OF_LMEMORY, "Fatal: Out of local memory error" },
    { FATAL_OUT_OF_SMEMORY, "Fatal: Out of shared memor error" },
    { FATAL_ACCESS_DENIED, "Fatal: Access Denied" },
    { FATAL_LOCK_INIT_CREATEMUTEX, "Fatal: CreateMutex operation failed in lock_initialize" },
    { FATAL_LOCK_LONGNAME, "Fatal: Lock prefix is too long" },
    { FATAL_FILEMAP_CREATION, "Fatal: Filemap creation error" },
    { FATAL_FILEMAP_INFOCREATE, "Fatal: CreateFileMapping operation for information filemap failed" },
    { FATAL_FILEMAP_INFOMAP, "Fatal: MapViewOfFileEx operation for information filemap failed" },
    { FATAL_FILEMAP_CREATE_SNAPSHOT, "Fatal: CreateToolhelp32Snapshot call failed unexpectedly" },
    { FATAL_FILEMAP_INITIALIZE, "Fatal: New filemap couldn't be created" },
    { FATAL_FILEMAP_NOFREE, "Fatal: No free filemap_information_entry found in info filemap" },
    { FATAL_ALLOC_NO_MEMORY, "Fatal: Shared memory segment is completely full" },
    { FATAL_FCACHE_CREATION, "" },
    { FATAL_FCACHE_CREATEFILE, "Fatal: CreateFile call failed" },
    { FATAL_FCACHE_GETFILETYPE, "Fatal: GetFileType call failed" },
    { FATAL_FCACHE_GETFILESIZE, "Fatal: GetFileSize call failed" },
    { FATAL_FCACHE_READFILE, "Fatal: ReadFile call failed" },
    { FATAL_FCACHE_INITIALIZE, "Fatal: Failure while initializing file cache module" },
    { WARNING_FCACHE_TOOBIG, "Warning: File too big for file cache" },
    { FATAL_FCACHE_FILEINFO, "Fatal: Failure in FileInfo" }
};

/* Windows maintain last error code for each thread */
/* We are only maintaining for the process */
unsigned int error_glerror = 0;

unsigned int error_getlasterror()
{
    return error_glerror;
}

unsigned int error_setlasterror()
{
    error_glerror = GetLastError();
    WCG(lasterror) = error_glerror;
    return error_glerror;
}

char * error_gethrmessage()
{
    return error_getmessage(error_glerror);
}

char * error_getmessage(unsigned int error_code)
{
    int    count   = 0;
    int    index   = 0;
    char * message = NULL;

    count = sizeof(wincache_errors)/sizeof(wincache_errors[0]);
    for(index = 0; index < count; index++)
    {
        if(wincache_errors[index].error_code == error_glerror)
        {
            message = wincache_errors[index].error_message;
            break;
        }
    }

    return message;
}
