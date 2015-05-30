/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          StreamSocket.h
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

#pragma once

#ifndef _STREAMSOCKET_H
#define _STREAMSOCKET_H

#include "GlobalSocket.h"
#include "Socket.h"
#include "AddressFamily.h"
#include "SocketProvider.h"
#include "StreamSocketProvider.h"
#include "Base\Timestamp.h"

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			class StreamSocket : public Socket
				/// This class provides an interface to a
				/// TCP stream socket.
			{
			public:
				StreamSocket();
				/// Creates an unconnected stream socket.
				///
				/// Before sending or receiving data, the socket
				/// must be connected with a call to connect().

				explicit StreamSocket(const SocketAddress& address);
				/// Creates a stream socket and connects it to
				/// the socket specified by address.

				explicit StreamSocket(AddressFamily family);
				/// Creates an unconnected stream socket
				/// for the given address family.
				///
				/// This is useful if certain socket options
				/// (like send and receive buffer) sizes, that must 
				/// be set before connecting the socket, will be
				/// set later on.

				StreamSocket(const Socket& socket);
				/// Creates the StreamSocket with the SocketImpl
				/// from another socket. The SocketImpl must be
				/// a StreamSocketImpl, otherwise an InvalidArgumentException
				/// will be thrown.

				virtual ~StreamSocket();
				/// Destroys the StreamSocket.

				StreamSocket& operator = (const Socket& socket);
				/// Assignment operator.
				///
				/// Releases the socket's SocketImpl and
				/// attaches the SocketImpl from the other socket and
				/// increments the reference count of the SocketImpl.	

				void connect(const SocketAddress& address);
				/// Initializes the socket and establishes a connection to 
				/// the TCP server at the given address.
				///
				/// Can also be used for UDP sockets. In this case, no
				/// connection is established. Instead, incoming and outgoing
				/// packets are restricted to the specified address.

				void connect(const SocketAddress& address, const Nequeo::Timespan& timeout);
				/// Initializes the socket, sets the socket timeout and 
				/// establishes a connection to the TCP server at the given address.

				void connectNB(const SocketAddress& address);
				/// Initializes the socket and establishes a connection to 
				/// the TCP server at the given address. Prior to opening the
				/// connection the socket is set to nonblocking mode.

				void shutdownReceive();
				/// Shuts down the receiving part of the socket connection.

				void shutdownSend();
				/// Shuts down the sending part of the socket connection.

				void shutdown();
				/// Shuts down both the receiving and the sending part
				/// of the socket connection.

				int sendBytes(const void* buffer, int length, int flags = 0);
				/// Sends the contents of the given buffer through
				/// the socket.
				///
				/// Returns the number of bytes sent, which may be
				/// less than the number of bytes specified.
				///
				/// Certain socket implementations may also return a negative
				/// value denoting a certain condition.

				int receiveBytes(void* buffer, int length, int flags = 0);
				/// Receives data from the socket and stores it
				/// in buffer. Up to length bytes are received.
				///
				/// Returns the number of bytes received. 
				/// A return value of 0 means a graceful shutdown 
				/// of the connection from the peer.
				///
				/// Throws a TimeoutException if a receive timeout has
				/// been set and nothing is received within that interval.
				/// Throws a NetException (or a subclass) in case of other errors.

				void sendUrgent(unsigned char data);
				/// Sends one byte of urgent data through
				/// the socket.
				///
				/// The data is sent with the MSG_OOB flag.
				///
				/// The preferred way for a socket to receive urgent data
				/// is by enabling the SO_OOBINLINE option.

			public:
				StreamSocket(SocketProvider* pImpl);
				/// Creates the Socket and attaches the given SocketImpl.
				/// The socket takes owership of the SocketImpl.
				///
				/// The SocketImpl must be a StreamSocketImpl, otherwise
				/// an InvalidArgumentException will be thrown.

				friend class SocketIOS;
			};
		}
	}
}
#endif