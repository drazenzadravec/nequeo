/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          StreamSocket.cpp
*  Purpose :       StreamSocket class.
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

#include "StreamSocket.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

using Nequeo::Exceptions::InvalidArgumentException;

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			StreamSocket::StreamSocket() : Socket(new StreamSocketProvider)
			{
			}


			StreamSocket::StreamSocket(const SocketAddress& address) : Socket(new StreamSocketProvider(address.addressFamily()))
			{
				connect(address);
			}


			StreamSocket::StreamSocket(AddressFamily family) : Socket(new StreamSocketProvider(family))
			{
			}


			StreamSocket::StreamSocket(const Socket& socket) : Socket(socket)
			{
				if (!dynamic_cast<StreamSocketProvider*>(impl()))
					throw InvalidArgumentException("Cannot assign incompatible socket");
			}


			StreamSocket::StreamSocket(SocketProvider* pImpl) : Socket(pImpl)
			{
				if (!dynamic_cast<StreamSocketProvider*>(impl()))
					throw InvalidArgumentException("Cannot assign incompatible socket");
			}


			StreamSocket::~StreamSocket()
			{
			}


			StreamSocket& StreamSocket::operator = (const Socket& socket)
			{
				if (dynamic_cast<StreamSocketProvider*>(socket.impl()))
					Socket::operator = (socket);
				else
					throw InvalidArgumentException("Cannot assign incompatible socket");
				return *this;
			}


			void StreamSocket::connect(const SocketAddress& address)
			{
				impl()->connect(address);
			}


			void StreamSocket::connect(const SocketAddress& address, const Nequeo::Timespan& timeout)
			{
				impl()->connect(address, timeout);
			}


			void StreamSocket::connectNB(const SocketAddress& address)
			{
				impl()->connectNB(address);
			}


			void StreamSocket::shutdownReceive()
			{
				impl()->shutdownReceive();
			}


			void StreamSocket::shutdownSend()
			{
				impl()->shutdownSend();
			}


			void StreamSocket::shutdown()
			{
				impl()->shutdown();
			}


			int StreamSocket::sendBytes(const void* buffer, int length, int flags)
			{
				return impl()->sendBytes(buffer, length, flags);
			}


			int StreamSocket::receiveBytes(void* buffer, int length, int flags)
			{
				return impl()->receiveBytes(buffer, length, flags);
			}


			void StreamSocket::sendUrgent(unsigned char data)
			{
				impl()->sendUrgent(data);
			}
		}
	}
}