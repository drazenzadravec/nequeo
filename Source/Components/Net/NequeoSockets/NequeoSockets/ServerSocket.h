/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ServerSocket.h
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

#pragma once

#ifndef _SERVERSOCKET_H
#define _SERVERSOCKET_H

#include "GlobalSocket.h"
#include "Socket.h"
#include "StreamSocket.h"
#include "Base\Types.h"

using Nequeo::Net::Sockets::Socket;
using Nequeo::Net::Sockets::StreamSocket;
using Nequeo::Net::Sockets::SocketAddress;
using Nequeo::Net::Sockets::SocketProvider;

using Nequeo::UInt8;
using Nequeo::UInt16;
using Nequeo::UInt32;

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			/// This class provides an interface to a
			/// TCP server socket.
			class ServerSocket : public Socket
			{
			public:
				ServerSocket();
				/// Creates a server socket.
				///
				/// The server socket must be bound to
				/// and address and put into listening state.

				ServerSocket(const Socket& socket);
				/// Creates the ServerSocket with the SocketImpl
				/// from another socket. The SocketImpl must be
				/// a ServerSocketImpl, otherwise an InvalidArgumentException
				/// will be thrown.

				ServerSocket(const SocketAddress& address, int backlog = 64);
				/// Creates a server socket, binds it
				/// to the given address and puts it in listening
				/// state.
				///
				/// After successful construction, the server socket
				/// is ready to accept connections.

				ServerSocket(Nequeo::UInt16 port, int backlog = 64);
				/// Creates a server socket, binds it
				/// to the given port and puts it in listening
				/// state.
				///
				/// After successful construction, the server socket
				/// is ready to accept connections.

				virtual ~ServerSocket();
				/// Destroys the StreamSocket.

				ServerSocket& operator = (const Socket& socket);
				/// Assignment operator.
				///
				/// Releases the socket's SocketImpl and
				/// attaches the SocketImpl from the other socket and
				/// increments the reference count of the SocketImpl.	

				virtual void bind(const SocketAddress& address, bool reuseAddress = false);
				/// Bind a local address to the socket.
				///
				/// This is usually only done when establishing a server
				/// socket. TCP clients should not bind a socket to a
				/// specific address.
				///
				/// If reuseAddress is true, sets the SO_REUSEADDR
				/// socket option.

				virtual void bind(Nequeo::UInt16 port, bool reuseAddress = false);
				/// Bind a local port to the socket.
				///
				/// This is usually only done when establishing a server
				/// socket. 
				///
				/// If reuseAddress is true, sets the SO_REUSEADDR
				/// socket option.

				virtual void bind6(const SocketAddress& address, bool reuseAddress = false, bool ipV6Only = false);
				/// Bind a local IPv6 address to the socket.
				///
				/// This is usually only done when establishing a server
				/// socket. TCP clients should not bind a socket to a
				/// specific address.
				///
				/// If reuseAddress is true, sets the SO_REUSEADDR
				/// socket option.
				///
				/// The given address must be an IPv6 address. The
				/// IPPROTO_IPV6/IPV6_V6ONLY option is set on the socket
				/// according to the ipV6Only parameter.
				///
				/// If the library has not been built with IPv6 support,
				/// a Poco::NotImplementedException will be thrown.

				virtual void bind6(Nequeo::UInt16 port, bool reuseAddress = false, bool ipV6Only = false);
				/// Bind a local IPv6 port to the socket.
				///
				/// This is usually only done when establishing a server
				/// socket. 
				///
				/// If reuseAddress is true, sets the SO_REUSEADDR
				/// socket option.
				///
				/// The given address must be an IPv6 address. The
				/// IPPROTO_IPV6/IPV6_V6ONLY option is set on the socket
				/// according to the ipV6Only parameter.
				///
				/// If the library has not been built with IPv6 support,
				/// a Poco::NotImplementedException will be thrown.

				virtual void listen(int backlog = 64);
				/// Puts the socket into listening state.
				///
				/// The socket becomes a passive socket that
				/// can accept incoming connection requests.
				///
				/// The backlog argument specifies the maximum
				/// number of connections that can be queued
				/// for this socket.

				virtual StreamSocket acceptConnection(SocketAddress& clientAddr);
				/// Get the next completed connection from the
				/// socket's completed connection queue.
				///
				/// If the queue is empty, waits until a connection
				/// request completes.
				///
				/// Returns a new TCP socket for the connection
				/// with the client.
				///
				/// The client socket's address is returned in clientAddr.

				virtual StreamSocket acceptConnection();
				/// Get the next completed connection from the
				/// socket's completed connection queue.
				///
				/// If the queue is empty, waits until a connection
				/// request completes.
				///
				/// Returns a new TCP socket for the connection
				/// with the client.

			protected:
				ServerSocket(SocketProvider* pImpl, bool);
				/// The bool argument is to resolve an ambiguity with
				/// another constructor.
			};
		}
	}
}
#endif