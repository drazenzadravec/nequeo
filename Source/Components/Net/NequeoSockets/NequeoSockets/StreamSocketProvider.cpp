/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          StreamSocketProvider.cpp
*  Purpose :       StreamSocketProvider class.
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

#include "stdafx.h"

#include "StreamSocketProvider.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"
#include "Threading\Thread.h"

Nequeo::Net::Sockets::StreamSocketProvider::StreamSocketProvider()
{
}

///
Nequeo::Net::Sockets::StreamSocketProvider::StreamSocketProvider(Nequeo::Net::Sockets::AddressFamily addressFamily)
{
	if (addressFamily == Nequeo::Net::Sockets::IPv4)
		init(AF_INET);
	else if (addressFamily == Nequeo::Net::Sockets::IPv6)
		init(AF_INET6);
	else throw Nequeo::Exceptions::InvalidArgumentException("Invalid or unsupported address family passed to StreamSocketImpl");
}


Nequeo::Net::Sockets::StreamSocketProvider::StreamSocketProvider(nequeo_socket_t sockfd) : SocketProvider(sockfd)
{
}


Nequeo::Net::Sockets::StreamSocketProvider::~StreamSocketProvider()
{
}

///
int Nequeo::Net::Sockets::StreamSocketProvider::sendBytes(const void* buffer, int length, int flags)
{
	const char* p = reinterpret_cast<const char*>(buffer);
	int remaining = length;
	int sent = 0;
	bool blocking = getBlocking();
	while (remaining > 0)
	{
		int n = SocketProvider::sendBytes(p, remaining, flags);
		if (n >= 0)
		{
			p += n;
			sent += n;
			remaining -= n;
			if (blocking && remaining > 0)
				Nequeo::Threading::Thread::yield();
			else
				break;
		}
	}
	return sent;
}