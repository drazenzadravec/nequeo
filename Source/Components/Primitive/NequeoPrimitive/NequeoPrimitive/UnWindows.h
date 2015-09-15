/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          UnWindows.h
*  Purpose :       UnWindows class.
*
*/

/*
Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#pragma once

#ifndef _UNWINDOWS_H
#define _UNWINDOWS_H

#include "GlobalPrimitive.h"

#include <windows.h>

#if !defined(NEQUEO_NO_UNWINDOWS)
// A list of annoying macros to #undef.
// Feel free to extend as required.
#undef GetBinaryType
#undef GetShortPathName
#undef GetLongPathName
#undef GetEnvironmentStrings
#undef SetEnvironmentStrings
#undef FreeEnvironmentStrings
#undef FormatMessage
#undef EncryptFile
#undef DecryptFile
#undef CreateMutex
#undef OpenMutex
#undef CreateEvent
#undef OpenEvent
#undef CreateSemaphore
#undef OpenSemaphore
#undef LoadLibrary
#undef GetModuleFileName
#undef CreateProcess
#undef GetCommandLine
#undef GetEnvironmentVariable
#undef SetEnvironmentVariable
#undef ExpandEnvironmentStrings
#undef OutputDebugString
#undef FindResource
#undef UpdateResource
#undef FindAtom
#undef AddAtom
#undef GetSystemDirectory
#undef GetTempPath
#undef GetTempFileName
#undef SetCurrentDirectory
#undef GetCurrentDirectory
#undef CreateDirectory
#undef RemoveDirectory
#undef CreateFile
#undef DeleteFile
#undef SearchPath
#undef CopyFile
#undef MoveFile
#undef ReplaceFile
#undef GetComputerName
#undef SetComputerName
#undef GetUserName
#undef LogonUser
#undef GetVersion
#undef GetObject
#endif

#endif