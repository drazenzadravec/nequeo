/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          GlobalSocket.h
*  Purpose :       Global definition header.
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

#ifndef _GLOBALSOCKET_H
#define _GLOBALSOCKET_H

#include "stdafx.h"

#include <windows.h>
#include <fwpmu.h>
#include <stdio.h>
#include <string>
#include <string.h>
#include <iomanip>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <vector>
#include <time.h>
#include <algorithm>

#include "Primitive\GlobalPrimitive.h"
#include "IO\GlobalStreaming.h"

#define RECV_DATA_BUF_SIZE 8192

#define nequeo_socket_t				SOCKET
#define nequeo_socklen_t			int
#define nequeo_ioctl_request_t		int
#define nequeo_closesocket(s)		closesocket(s)

#define nequeo_set_sa_len(pSA, len) (void) 0
#define nequeo_set_sin_len(pSA)     (void) 0
#define nequeo_set_sin6_len(pSA)    (void) 0

const int IPv4_Length = sizeof(struct sockaddr_in);
const int IPv6_Length = sizeof(struct sockaddr_in6);

#define SO_REUSEPORT				0
#define NEQUEO_INVALID_SOCKET		INVALID_SOCKET

#define NEQUEO_EINTR           WSAEINTR
#define NEQUEO_EACCES          WSAEACCES
#define NEQUEO_EFAULT          WSAEFAULT
#define NEQUEO_EINVAL          WSAEINVAL
#define NEQUEO_EMFILE          WSAEMFILE
#define NEQUEO_EAGAIN          WSAEWOULDBLOCK
#define NEQUEO_EWOULDBLOCK     WSAEWOULDBLOCK
#define NEQUEO_EINPROGRESS     WSAEINPROGRESS
#define NEQUEO_EALREADY        WSAEALREADY
#define NEQUEO_ENOTSOCK        WSAENOTSOCK
#define NEQUEO_EDESTADDRREQ    WSAEDESTADDRREQ
#define NEQUEO_EMSGSIZE        WSAEMSGSIZE
#define NEQUEO_EPROTOTYPE      WSAEPROTOTYPE
#define NEQUEO_ENOPROTOOPT     WSAENOPROTOOPT
#define NEQUEO_EPROTONOSUPPORT WSAEPROTONOSUPPORT
#define NEQUEO_ESOCKTNOSUPPORT WSAESOCKTNOSUPPORT
#define NEQUEO_ENOTSUP         WSAEOPNOTSUPP
#define NEQUEO_EPFNOSUPPORT    WSAEPFNOSUPPORT
#define NEQUEO_EAFNOSUPPORT    WSAEAFNOSUPPORT
#define NEQUEO_EADDRINUSE      WSAEADDRINUSE
#define NEQUEO_EADDRNOTAVAIL   WSAEADDRNOTAVAIL
#define NEQUEO_ENETDOWN        WSAENETDOWN
#define NEQUEO_ENETUNREACH     WSAENETUNREACH
#define NEQUEO_ENETRESET       WSAENETRESET
#define NEQUEO_ECONNABORTED    WSAECONNABORTED
#define NEQUEO_ECONNRESET      WSAECONNRESET
#define NEQUEO_ENOBUFS         WSAENOBUFS
#define NEQUEO_EISCONN         WSAEISCONN
#define NEQUEO_ENOTCONN        WSAENOTCONN
#define NEQUEO_ESHUTDOWN       WSAESHUTDOWN
#define NEQUEO_ETIMEDOUT       WSAETIMEDOUT
#define NEQUEO_ECONNREFUSED    WSAECONNREFUSED
#define NEQUEO_EHOSTDOWN       WSAEHOSTDOWN
#define NEQUEO_EHOSTUNREACH    WSAEHOSTUNREACH
#define NEQUEO_ESYSNOTREADY    WSASYSNOTREADY
#define NEQUEO_ENOTINIT        WSANOTINITIALISED
#define NEQUEO_HOST_NOT_FOUND  WSAHOST_NOT_FOUND
#define NEQUEO_TRY_AGAIN       WSATRY_AGAIN
#define NEQUEO_NO_RECOVERY     WSANO_RECOVERY
#define NEQUEO_NO_DATA         WSANO_DATA

#if (NEQUEO_OS == NEQUEO_OS_HPUX) || (NEQUEO_OS == NEQUEO_OS_SOLARIS) || (NEQUEO_OS == NEQUEO_OS_WINDOWS_CE)
	#define NEQUEO_BROKEN_TIMEOUTS 1
#endif

using namespace std;

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			/// Initialize the network subsystem.
			/// Calls WSAStartup() on Windows, does nothing
			/// on other platforms.
			void InitializeNetwork();

			/// Uninitialize the network subsystem.
			/// Calls WSACleanup() on Windows, does nothing
			/// on other platforms.
			void UninitializeNetwork();
		}
	}
}
#endif