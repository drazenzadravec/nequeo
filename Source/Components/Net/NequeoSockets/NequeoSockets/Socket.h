/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Socket.h
*  Purpose :       Socket class.
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

#ifndef _SOCKET_H
#define _SOCKET_H

#include "GlobalSocket.h"
#include "IPAddress.h"
#include "Base\Timespan.h"
#include "Base\Timestamp.h"
#include "SocketAddressProvider.h"
#include "SocketAddress.h"
#include "SocketProvider.h"

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			// Socket class.
			class Socket
			{
				public:
					// Socket class.
					Socket();

					// Destroys the Socket and releases resources.
					virtual ~Socket();

					// Copy constructor.
					Socket(const Socket& socket);

					// Assignment operator.
					Socket& operator = (const Socket& socket);

					bool operator == (const Socket& socket) const;
					/// Returns true if both sockets share the same
					/// SocketImpl, false otherwise.

					bool operator != (const Socket& socket) const;
					/// Returns false if both sockets share the same
					/// SocketImpl, true otherwise.

					bool operator <  (const Socket& socket) const;
					/// Compares the SocketImpl pointers.

					bool operator <= (const Socket& socket) const;
					/// Compares the SocketImpl pointers.

					bool operator >  (const Socket& socket) const;
					/// Compares the SocketImpl pointers.

					bool operator >= (const Socket& socket) const;
					/// Compares the SocketImpl pointers.

					void close();
					/// Closes the socket.

					bool poll(const Nequeo::Timespan& timeout, int mode) const;
					/// Determines the status of the socket, using a 
					/// call to select().
					/// 
					/// The mode argument is constructed by combining the values
					/// of the SelectMode enumeration.
					///
					/// Returns true if the next operation corresponding to
					/// mode will not block, false otherwise.

					int available() const;
					/// Returns the number of bytes available that can be read
					/// without causing the socket to block.

					void setSendBufferSize(int size);
					/// Sets the size of the send buffer.

					int getSendBufferSize() const;
					/// Returns the size of the send buffer.
					///
					/// The returned value may be different than the
					/// value previously set with setSendBufferSize(),
					/// as the system is free to adjust the value.

					void setReceiveBufferSize(int size);
					/// Sets the size of the receive buffer.

					int getReceiveBufferSize() const;
					/// Returns the size of the receive buffer.
					///
					/// The returned value may be different than the
					/// value previously set with setReceiveBufferSize(),
					/// as the system is free to adjust the value.

					void setSendTimeout(const Nequeo::Timespan& timeout);
					/// Sets the send timeout for the socket.

					Nequeo::Timespan getSendTimeout() const;
					/// Returns the send timeout for the socket.
					///
					/// The returned timeout may be different than the
					/// timeout previously set with setSendTimeout(),
					/// as the system is free to adjust the value.

					void setReceiveTimeout(const Nequeo::Timespan& timeout);
					/// Sets the send timeout for the socket.
					///
					/// On systems that do not support SO_RCVTIMEO, a
					/// workaround using poll() is provided.

					Nequeo::Timespan getReceiveTimeout() const;
					/// Returns the receive timeout for the socket.
					///
					/// The returned timeout may be different than the
					/// timeout previously set with getReceiveTimeout(),
					/// as the system is free to adjust the value.

					void setOption(int level, int option, int value);
					/// Sets the socket option specified by level and option
					/// to the given integer value.

					void setOption(int level, int option, unsigned value);
					/// Sets the socket option specified by level and option
					/// to the given integer value.

					void setOption(int level, int option, unsigned char value);
					/// Sets the socket option specified by level and option
					/// to the given integer value.

					void setOption(int level, int option, const Nequeo::Timespan& value);
					/// Sets the socket option specified by level and option
					/// to the given time value.

					void setOption(int level, int option, const IPAddress& value);
					/// Sets the socket option specified by level and option
					/// to the given time value.

					void getOption(int level, int option, int& value) const;
					/// Returns the value of the socket option 
					/// specified by level and option.

					void getOption(int level, int option, unsigned& value) const;
					/// Returns the value of the socket option 
					/// specified by level and option.

					void getOption(int level, int option, unsigned char& value) const;
					/// Returns the value of the socket option 
					/// specified by level and option.

					void getOption(int level, int option, Nequeo::Timespan& value) const;
					/// Returns the value of the socket option 
					/// specified by level and option.

					void getOption(int level, int option, IPAddress& value) const;
					/// Returns the value of the socket option 
					/// specified by level and option.

					void setLinger(bool on, int seconds);
					/// Sets the value of the SO_LINGER socket option.

					void getLinger(bool& on, int& seconds) const;
					/// Returns the value of the SO_LINGER socket option.

					void setNoDelay(bool flag);
					/// Sets the value of the TCP_NODELAY socket option.

					bool getNoDelay() const;
					/// Returns the value of the TCP_NODELAY socket option.

					void setKeepAlive(bool flag);
					/// Sets the value of the SO_KEEPALIVE socket option.

					bool getKeepAlive() const;
					/// Returns the value of the SO_KEEPALIVE socket option.

					void setReuseAddress(bool flag);
					/// Sets the value of the SO_REUSEADDR socket option.

					bool getReuseAddress() const;
					/// Returns the value of the SO_REUSEADDR socket option.

					void setReusePort(bool flag);
					/// Sets the value of the SO_REUSEPORT socket option.
					/// Does nothing if the socket implementation does not
					/// support SO_REUSEPORT.

					bool getReusePort() const;
					/// Returns the value of the SO_REUSEPORT socket option.
					///
					/// Returns false if the socket implementation does not
					/// support SO_REUSEPORT.

					void setOOBInline(bool flag);
					/// Sets the value of the SO_OOBINLINE socket option.

					bool getOOBInline() const;
					/// Returns the value of the SO_OOBINLINE socket option.

					void setBlocking(bool flag);
					/// Sets the socket in blocking mode if flag is true,
					/// disables blocking mode if flag is false.

					bool getBlocking() const;
					/// Returns the blocking mode of the socket.
					/// This method will only work if the blocking modes of 
					/// the socket are changed via the setBlocking method!

					SocketAddress address() const;
					/// Returns the IP address and port number of the socket.

					SocketAddress peerAddress() const;
					/// Returns the IP address and port number of the peer socket.

					SocketProvider* impl() const;
					/// Returns the SocketImpl for this socket.

					bool secure() const;
					/// Returns true if the socket's connection is secure
					/// (using SSL or TLS).

					static bool supportsIPv4();
					/// Returns true if the system supports IPv4.

					static bool supportsIPv6();
					/// Returns true if the system supports IPv6.

					void init(int af);
					/// Creates the underlying system socket for the given
					/// address family. 
					///
					/// Normally, this method should not be called directly, as
					/// socket creation will be handled automatically. There are
					/// a few situations where calling this method after creation
					/// of the Socket object makes sense. One example is setting
					/// a socket option before calling bind() on a ServerSocket.

					// Contains the list of select sockets.
					typedef std::vector<Socket> SocketList;

					static int select(SocketList& readList, SocketList& writeList, SocketList& exceptList, const Nequeo::Timespan& timeout);
					/// Determines the status of one or more sockets, 
					/// using a call to select().
					///
					/// ReadList contains the list of sockets which should be
					/// checked for readability.
					///
					/// WriteList contains the list of sockets which should be
					/// checked for writeability.
					///
					/// ExceptList contains a list of sockets which should be
					/// checked for a pending error.
					///
					/// Returns the number of sockets ready.
					///
					/// After return, 
					///   * readList contains those sockets ready for reading,
					///   * writeList contains those sockets ready for writing,
					///   * exceptList contains those sockets with a pending error.
					///
					/// If the total number of sockets passed in readList, writeList and
					/// exceptList is zero, select() will return immediately and the
					/// return value will be 0.
					///
					/// If one of the sockets passed to select() is closed while
					/// select() runs, select will return immediately. However,
					/// the closed socket will not be included in any list.
					/// In this case, the return value may be greater than the sum
					/// of all sockets in all list.

			protected:
				Socket(SocketProvider* pImpl);
				/// Creates the Socket and attaches the given SocketImpl.
				/// The socket takes owership of the SocketImpl.

				nequeo_socket_t sockfd() const;
				/// Returns the socket descriptor for this socket.

			private:
				bool _disposed;
				SocketProvider* _pImpl;
			};

			//
			// inlines
			//
			inline bool Socket::operator == (const Socket& socket) const
			{
				return _pImpl == socket._pImpl;
			}


			inline bool Socket::operator != (const Socket& socket) const
			{
				return _pImpl != socket._pImpl;
			}


			inline bool Socket::operator <  (const Socket& socket) const
			{
				return _pImpl < socket._pImpl;
			}


			inline bool Socket::operator <= (const Socket& socket) const
			{
				return _pImpl <= socket._pImpl;
			}


			inline bool Socket::operator >(const Socket& socket) const
			{
				return _pImpl > socket._pImpl;
			}


			inline bool Socket::operator >= (const Socket& socket) const
			{
				return _pImpl >= socket._pImpl;
			}


			inline void Socket::close()
			{
				_pImpl->close();
			}


			inline bool Socket::poll(const Nequeo::Timespan& timeout, int mode) const
			{
				return _pImpl->poll(timeout, mode);
			}


			inline int Socket::available() const
			{
				return _pImpl->available();
			}


			inline void Socket::setSendBufferSize(int size)
			{
				_pImpl->setSendBufferSize(size);
			}


			inline int Socket::getSendBufferSize() const
			{
				return _pImpl->getSendBufferSize();
			}


			inline void Socket::setReceiveBufferSize(int size)
			{
				_pImpl->setReceiveBufferSize(size);
			}


			inline int Socket::getReceiveBufferSize() const
			{
				return _pImpl->getReceiveBufferSize();
			}


			inline void Socket::setSendTimeout(const Nequeo::Timespan& timeout)
			{
				_pImpl->setSendTimeout(timeout);
			}


			inline Nequeo::Timespan Socket::getSendTimeout() const
			{
				return _pImpl->getSendTimeout();
			}


			inline void Socket::setReceiveTimeout(const Nequeo::Timespan& timeout)
			{
				_pImpl->setReceiveTimeout(timeout);
			}


			inline Nequeo::Timespan Socket::getReceiveTimeout() const
			{
				return _pImpl->getReceiveTimeout();
			}


			inline void Socket::setOption(int level, int option, int value)
			{
				_pImpl->setOption(level, option, value);
			}


			inline void Socket::setOption(int level, int option, unsigned value)
			{
				_pImpl->setOption(level, option, value);
			}


			inline void Socket::setOption(int level, int option, unsigned char value)
			{
				_pImpl->setOption(level, option, value);
			}


			inline void Socket::setOption(int level, int option, const Nequeo::Timespan& value)
			{
				_pImpl->setOption(level, option, value);
			}


			inline void Socket::setOption(int level, int option, const IPAddress& value)
			{
				_pImpl->setOption(level, option, value);
			}


			inline void Socket::getOption(int level, int option, int& value) const
			{
				_pImpl->getOption(level, option, value);
			}


			inline void Socket::getOption(int level, int option, unsigned& value) const
			{
				_pImpl->getOption(level, option, value);
			}


			inline void Socket::getOption(int level, int option, unsigned char& value) const
			{
				_pImpl->getOption(level, option, value);
			}


			inline void Socket::getOption(int level, int option, Nequeo::Timespan& value) const
			{
				_pImpl->getOption(level, option, value);
			}


			inline void Socket::getOption(int level, int option, IPAddress& value) const
			{
				_pImpl->getOption(level, option, value);
			}


			inline void Socket::setLinger(bool on, int seconds)
			{
				_pImpl->setLinger(on, seconds);
			}


			inline void Socket::getLinger(bool& on, int& seconds) const
			{
				_pImpl->getLinger(on, seconds);
			}


			inline void Socket::setNoDelay(bool flag)
			{
				_pImpl->setNoDelay(flag);
			}


			inline bool Socket::getNoDelay() const
			{
				return _pImpl->getNoDelay();
			}


			inline void Socket::setKeepAlive(bool flag)
			{
				_pImpl->setKeepAlive(flag);
			}


			inline bool Socket::getKeepAlive() const
			{
				return _pImpl->getKeepAlive();
			}


			inline void Socket::setReuseAddress(bool flag)
			{
				_pImpl->setReuseAddress(flag);
			}


			inline bool Socket::getReuseAddress() const
			{
				return _pImpl->getReuseAddress();
			}


			inline void Socket::setReusePort(bool flag)
			{
				_pImpl->setReusePort(flag);
			}


			inline bool Socket::getReusePort() const
			{
				return _pImpl->getReusePort();
			}


			inline void Socket::setOOBInline(bool flag)
			{
				_pImpl->setOOBInline(flag);
			}


			inline bool Socket::getOOBInline() const
			{
				return _pImpl->getOOBInline();
			}


			inline void Socket::setBlocking(bool flag)
			{
				_pImpl->setBlocking(flag);
			}


			inline bool Socket::getBlocking() const
			{
				return _pImpl->getBlocking();
			}


			inline SocketProvider* Socket::impl() const
			{
				return _pImpl;
			}


			inline nequeo_socket_t Socket::sockfd() const
			{
				return _pImpl->sockfd();
			}


			inline SocketAddress Socket::address() const
			{
				return _pImpl->address();
			}


			inline SocketAddress Socket::peerAddress() const
			{
				return _pImpl->peerAddress();
			}


			inline bool Socket::secure() const
			{
				return _pImpl->secure();
			}


			inline bool Socket::supportsIPv4()
			{
				return true;
			}
		}
	}
}
#endif