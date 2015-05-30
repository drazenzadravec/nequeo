/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ServerSocket.cpp
*  Purpose :       ServerSocket class.
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

#include "ServerSocket.h"
#include "ServerSocketProvider.h"
#include "IPAddress.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

using Nequeo::Exceptions::InvalidArgumentException;
using Nequeo::Net::Sockets::IPAddress;

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			ServerSocket::ServerSocket() : Socket(new ServerSocketProvider)
			{
			}


			ServerSocket::ServerSocket(const Socket& socket) : Socket(socket)
			{
				if (!dynamic_cast<ServerSocketProvider*>(impl()))
					throw InvalidArgumentException("Cannot assign incompatible socket");
			}


			ServerSocket::ServerSocket(const SocketAddress& address, int backlog) : Socket(new ServerSocketProvider)
			{
				impl()->bind(address, true);
				impl()->listen(backlog);
			}


			ServerSocket::ServerSocket(UInt16 port, int backlog) : Socket(new ServerSocketProvider)
			{
				IPAddress wildcardAddr;
				SocketAddress address(wildcardAddr, port);
				impl()->bind(address, true);
				impl()->listen(backlog);
			}


			ServerSocket::ServerSocket(SocketProvider* pImpl, bool ignore) : Socket(pImpl)
			{
			}


			ServerSocket::~ServerSocket()
			{
			}


			ServerSocket& ServerSocket::operator = (const Socket& socket)
			{
				if (dynamic_cast<ServerSocketProvider*>(socket.impl()))
					Socket::operator = (socket);
				else
					throw InvalidArgumentException("Cannot assign incompatible socket");
				return *this;
			}


			void ServerSocket::bind(const SocketAddress& address, bool reuseAddress)
			{
				impl()->bind(address, reuseAddress);
			}


			void ServerSocket::bind(UInt16 port, bool reuseAddress)
			{
				IPAddress wildcardAddr;
				SocketAddress address(wildcardAddr, port);
				impl()->bind(address, reuseAddress);
			}


			void ServerSocket::bind6(const SocketAddress& address, bool reuseAddress, bool ipV6Only)
			{
				impl()->bind6(address, reuseAddress, ipV6Only);
			}


			void ServerSocket::bind6(UInt16 port, bool reuseAddress, bool ipV6Only)
			{
				IPAddress wildcardAddr;
				SocketAddress address(wildcardAddr, port);
				impl()->bind6(address, reuseAddress, ipV6Only);
			}


			void ServerSocket::listen(int backlog)
			{
				impl()->listen(backlog);
			}


			StreamSocket ServerSocket::acceptConnection(SocketAddress& clientAddr)
			{
				return StreamSocket(impl()->acceptConnection(clientAddr));
			}


			StreamSocket ServerSocket::acceptConnection()
			{
				SocketAddress clientAddr;
				return StreamSocket(impl()->acceptConnection(clientAddr));
			}
		}
	}
}