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
   | Module: wincache_debug.c                                                                     |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   +----------------------------------------------------------------------------------------------+
*/

#include "precomp.h"

#define WINCACHE_DEBUG_MTYPE_DISABLED  0
#define WINCACHE_DEBUG_MTYPE_ALWAYS    101
#define WINCACHE_DEBUG_MTYPE_CRITICAL  201
#define WINCACHE_DEBUG_MTYPE_IMPORTANT 301
#define WINCACHE_DEBUG_MTYPE_VERBOSE   401
#define WINCACHE_DEBUG_MTYPE_DECORATE  501

unsigned int gdebuglevel = 0;

void dprintsetlevel(unsigned int level)
{
    if(level == WINCACHE_DEBUG_MTYPE_DISABLED  ||
       level == WINCACHE_DEBUG_MTYPE_ALWAYS    ||
       level == WINCACHE_DEBUG_MTYPE_CRITICAL  ||
       level == WINCACHE_DEBUG_MTYPE_IMPORTANT ||
       level == WINCACHE_DEBUG_MTYPE_VERBOSE   ||
       level == WINCACHE_DEBUG_MTYPE_DECORATE)
    {
        gdebuglevel = level;
    }

    return;
}

void dprintmessage(char * format, va_list args)
{
    char debug_message[255];

    sprintf_s(debug_message, 255, "WINCACHE: ");
    vsprintf_s(debug_message + 10, 245, format, args);

    OutputDebugStringA(debug_message);
    if(IsDebuggerPresent())
    {
        OutputDebugStringA("\n");
    }

    return;
}

void dprintalways(char * format, ...)
{
    va_list args;
    if(gdebuglevel >= WINCACHE_DEBUG_MTYPE_ALWAYS)
    {
        va_start(args, format);
        dprintmessage(format, args);
        va_end(args);
    }
}

void dprintcritical(char * format, ...)
{
    va_list args;
    if(gdebuglevel >= WINCACHE_DEBUG_MTYPE_CRITICAL)
    {
        va_start(args, format);
        dprintmessage(format, args);
        va_end(args);
    }
}

void dprintimportant(char * format, ...)
{
    va_list args;
    if(gdebuglevel >= WINCACHE_DEBUG_MTYPE_IMPORTANT)
    {
        va_start(args, format);
        dprintmessage(format, args);
        va_end(args);
    }
}

void dprintverbose(char * format, ...)
{
    va_list args;
    if(gdebuglevel >= WINCACHE_DEBUG_MTYPE_VERBOSE)
    {
        va_start(args, format);
        dprintmessage(format, args);
        va_end(args);
    }
}

void dprintdecorate(char * format, ...)
{
    va_list args;
    if(gdebuglevel >= WINCACHE_DEBUG_MTYPE_DECORATE)
    {
        va_start(args, format);
        dprintmessage(format, args);
        va_end(args);
    }
}

ZEND_INI_MH(wincache_modify_debuglevel)
{
    if (ZSTR_LEN(new_value) == 0)
    {
        dprintsetlevel(WINCACHE_DEBUG_MTYPE_DISABLED);
    }
    else
    {
        dprintsetlevel(atoi(ZSTR_VAL(new_value)));
        WCG(debuglevel) = gdebuglevel;
    }
    return SUCCESS;
}
