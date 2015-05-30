/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RawSocket.cpp
*  Purpose :       RawSocket class.
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

#include "RawSocket.h"
#include "RawSocketProvider.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

using Nequeo::Exceptions::InvalidArgumentException;

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			RawSocket::RawSocket() :
				Socket(new RawSocketProvider)
			{
			}


			RawSocket::RawSocket(Nequeo::Net::Sockets::AddressFamily family, int proto) :
				Socket(new RawSocketProvider(family, proto))
			{
			}


			RawSocket::RawSocket(const SocketAddress& address, bool reuseAddress) :
				Socket(new RawSocketProvider(address.addressFamily()))
			{
				bind(address, reuseAddress);
			}


			RawSocket::RawSocket(const Socket& socket) : Socket(socket)
			{
				if (!dynamic_cast<RawSocketProvider*>(impl()))
					throw InvalidArgumentException("Cannot assign incompatible socket");
			}


			RawSocket::RawSocket(SocketProvider* pImpl) : Socket(pImpl)
			{
				if (!dynamic_cast<RawSocketProvider*>(impl()))
					throw InvalidArgumentException("Cannot assign incompatible socket");
			}


			RawSocket::~RawSocket()
			{
			}


			RawSocket& RawSocket::operator = (const Socket& socket)
			{
				if (dynamic_cast<RawSocketProvider*>(socket.impl()))
					Socket::operator = (socket);
				else
					throw InvalidArgumentException("Cannot assign incompatible socket");
				return *this;
			}


			void RawSocket::connect(const SocketAddress& address)
			{
				impl()->connect(address);
			}


			void RawSocket::bind(const SocketAddress& address, bool reuseAddress)
			{
				impl()->bind(address, reuseAddress);
			}


			int RawSocket::sendBytes(const void* buffer, int length, int flags)
			{
				return impl()->sendBytes(buffer, length, flags);
			}


			int RawSocket::receiveBytes(void* buffer, int length, int flags)
			{
				return impl()->receiveBytes(buffer, length, flags);
			}


			int RawSocket::sendTo(const void* buffer, int length, const SocketAddress& address, int flags)
			{
				return impl()->sendTo(buffer, length, address, flags);
			}


			int RawSocket::receiveFrom(void* buffer, int length, SocketAddress& address, int flags)
			{
				return impl()->receiveFrom(buffer, length, address, flags);
			}
		}
	}
}