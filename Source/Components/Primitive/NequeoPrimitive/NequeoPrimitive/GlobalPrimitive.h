/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          GlobalPrimitive.h
*  Purpose :       GlobalPrimitive class.
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

#ifndef _GLOBALPRIMITIVE_H
#define _GLOBALPRIMITIVE_H

#include "stdafx.h"

#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <iomanip>

#define NEQUEO_OS_FREE_BSD      0x0001
#define NEQUEO_OS_AIX           0x0002
#define NEQUEO_OS_HPUX          0x0003
#define NEQUEO_OS_TRU64         0x0004
#define NEQUEO_OS_LINUX         0x0005
#define NEQUEO_OS_MAC_OS_X      0x0006
#define NEQUEO_OS_NET_BSD       0x0007
#define NEQUEO_OS_OPEN_BSD      0x0008
#define NEQUEO_OS_IRIX          0x0009
#define NEQUEO_OS_SOLARIS       0x000a
#define NEQUEO_OS_QNX           0x000b
#define NEQUEO_OS_VXWORKS       0x000c
#define NEQUEO_OS_CYGWIN        0x000d
#define NEQUEO_OS_UNKNOWN_UNIX  0x00ff
#define NEQUEO_OS_WINDOWS_NT    0x1001
#define NEQUEO_OS_WINDOWS_CE    0x1011
#define NEQUEO_OS_VMS           0x2001

#if defined(__FreeBSD__)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS_FAMILY_BSD 1
	#define NEQUEO_OS NEQUEO_OS_FREE_BSD
#elif defined(_AIX) || defined(__TOS_AIX__)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS NEQUEO_OS_AIX
#elif defined(hpux) || defined(_hpux)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS NEQUEO_OS_HPUX
#elif defined(__digital__) || defined(__osf__)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS NEQUEO_OS_TRU64
#elif defined(linux) || defined(__linux) || defined(__linux__) || defined(__TOS_LINUX__)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS NEQUEO_OS_LINUX
#elif defined(__APPLE__) || defined(__TOS_MACOS__)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS_FAMILY_BSD 1
	#define NEQUEO_OS NEQUEO_OS_MAC_OS_X
#elif defined(__NetBSD__)
	#define NEQUEO_OS_FAMILY_UNIX 1
#define NEQUEOO_OS_FAMILY_BSD 1
	#define NEQUEO_OS NEQUEO_OS_NET_BSD
#elif defined(__OpenBSD__)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS_FAMILY_BSD 1
	#define NEQUEO_OS NEQUEO_OS_OPEN_BSD
#elif defined(sgi) || defined(__sgi)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS NEQUEO_OS_IRIX
#elif defined(sun) || defined(__sun)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS NEQUEO_OS_SOLARIS
#elif defined(__QNX__)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS NEQUEO_OS_QNX
#elif defined(unix) || defined(__unix) || defined(__unix__)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS NEQUEO_OS_UNKNOWN_UNIX
#elif defined(_WIN32_WCE)
	#define NEQUEO_OS_FAMILY_WINDOWS 1
	#define NEQUEO_OS NEQUEO_OS_WINDOWS_CE
#elif defined(_WIN32) || defined(_WIN64)
	#define NEQUEO_OS_FAMILY_WINDOWS 1
	#define NEQUEO_OS NEQUEO_OS_WINDOWS_NT
#elif defined(__CYGWIN__)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS NEQUEO_OS_CYGWIN
#elif defined(__VMS)
	#define NEQUEO_OS_FAMILY_VMS 1
	#define NEQUEO_OS NEQUEO_OS_VMS
#elif defined(POCO_VXWORKS)
	#define NEQUEO_OS_FAMILY_UNIX 1
	#define NEQUEO_OS NEQUEO_OS_VXWORKS
#endif

#if !defined(NEQUEO_OS)
	#error "Unknown Platform."
#endif

using namespace std;

#endif