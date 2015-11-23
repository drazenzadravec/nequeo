/*
   +----------------------------------------------------------------------------------------------+
   | Windows Cache for PHP                                                                        |
   +----------------------------------------------------------------------------------------------+
   | Copyright (c) 2015, Microsoft Corporation. All rights reserved.                              |
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
   | Module: wincache_dummy_etw.h                                                                 |
   +----------------------------------------------------------------------------------------------+
   | Author: Eric Stenson <ericsten@php.net>                                                      |
   +----------------------------------------------------------------------------------------------+
*/

#pragma once

#define EventRegisterPHP_Wincache()
#define EventUnregisterPHP_Wincache()
#define EventWriteModuleInitErrorEvent(Error)
#define EventWriteInitMutexErrorEvent(Name, Handle)
#define EventWriteInitOpcacheLocalFallbackEvent()
#define EventWriteMemBlockNotInUse(Block, AllocContext, File, Line)
#define EventWriteMemFreeAddrNotInSegment(Block, AllocContext, File, Line)
#define EventWriteMemCombineNonFreeBlock(Block, AllocContext, File, Line)
#define EventWriteMemFreeListCorrupt(Block, AllocContext, File, Line)
#define EventWriteLockAbandonedMutex(LockName, LastReaderPid, LastWriterPid, File, Line)
#define EventWriteLockFailedWaitForLock(LockName, LastReaderPid, LastWriterPid, File, Line)
#define EventWriteUnlockAbandonedMutex(LockName, LastReaderPid, LastWriterPid, File, Line)
#define EventWriteUnlockFailedWaitForLock(LockName, LastReaderPid, LastWriterPid, File, Line)

