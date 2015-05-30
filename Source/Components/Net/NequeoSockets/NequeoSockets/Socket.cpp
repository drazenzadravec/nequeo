/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Socket.cpp
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

#include "stdafx.h"

#include "Socket.h"
#include "StreamSocketProvider.h"
#include "Base\Timestamp.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			Socket::Socket() : _pImpl(new StreamSocketProvider), _disposed(false)
			{
			}

			Socket::Socket(SocketProvider* pImpl) : _pImpl(pImpl), _disposed(false)
			{
				if (_pImpl == nullptr)
					throw Nequeo::Exceptions::NullPointerException();
			}

			Socket::Socket(const Socket& socket) : _pImpl(socket._pImpl), _disposed(false)
			{
				if (_pImpl == nullptr)
					throw Nequeo::Exceptions::NullPointerException();

				_pImpl->duplicate();
			}

			Socket& Socket::operator = (const Socket& socket)
			{
				if (&socket != this)
				{
					if (_pImpl) _pImpl->release();
					_pImpl = socket._pImpl;
					if (_pImpl) _pImpl->duplicate();
				}
				return *this;
			}

			Socket::~Socket()
			{
				// If not disposed.
				if (!_disposed)
				{
					_disposed = true;
					_pImpl->release();
				}
			}

			int Socket::select(SocketList& readList, SocketList& writeList, SocketList& exceptList, const Nequeo::Timespan& timeout)
			{
				fd_set fdRead;
				fd_set fdWrite;
				fd_set fdExcept;
				int nfd = 0;
				FD_ZERO(&fdRead);
				for (SocketList::const_iterator it = readList.begin(); it != readList.end(); ++it)
				{
					nequeo_socket_t fd = it->sockfd();
					if (fd != NEQUEO_INVALID_SOCKET)
					{
						if (int(fd) > nfd)
							nfd = int(fd);
						FD_SET(fd, &fdRead);
					}
				}
				FD_ZERO(&fdWrite);
				for (SocketList::const_iterator it = writeList.begin(); it != writeList.end(); ++it)
				{
					nequeo_socket_t fd = it->sockfd();
					if (fd != NEQUEO_INVALID_SOCKET)
					{
						if (int(fd) > nfd)
							nfd = int(fd);
						FD_SET(fd, &fdWrite);
					}
				}
				FD_ZERO(&fdExcept);
				for (SocketList::const_iterator it = exceptList.begin(); it != exceptList.end(); ++it)
				{
					nequeo_socket_t fd = it->sockfd();
					if (fd != NEQUEO_INVALID_SOCKET)
					{
						if (int(fd) > nfd)
							nfd = int(fd);
						FD_SET(fd, &fdExcept);
					}
				}
				if (nfd == 0) return 0;
				Nequeo::Timespan remainingTime(timeout);
				int rc;
				do
				{
					struct timeval tv;
					tv.tv_sec = (long)remainingTime.totalSeconds();
					tv.tv_usec = (long)remainingTime.useconds();
					Nequeo::Timestamp start;
					rc = ::select(nfd + 1, &fdRead, &fdWrite, &fdExcept, &tv);
					if (rc < 0 && SocketProvider::lastError() == NEQUEO_EINTR)
					{
						Nequeo::Timestamp end;
						Nequeo::Timespan waited = end - start;
						if (waited < remainingTime)
							remainingTime -= waited;
						else
							remainingTime = 0;
					}
				} while (rc < 0 && SocketProvider::lastError() == NEQUEO_EINTR);
				if (rc < 0) SocketProvider::error();

				SocketList readyReadList;
				for (SocketList::const_iterator it = readList.begin(); it != readList.end(); ++it)
				{
					nequeo_socket_t fd = it->sockfd();
					if (fd != NEQUEO_INVALID_SOCKET)
					{
						if (FD_ISSET(fd, &fdRead))
							readyReadList.push_back(*it);
					}
				}
				std::swap(readList, readyReadList);
				SocketList readyWriteList;
				for (SocketList::const_iterator it = writeList.begin(); it != writeList.end(); ++it)
				{
					nequeo_socket_t fd = it->sockfd();
					if (fd != NEQUEO_INVALID_SOCKET)
					{
						if (FD_ISSET(fd, &fdWrite))
							readyWriteList.push_back(*it);
					}
				}
				std::swap(writeList, readyWriteList);
				SocketList readyExceptList;
				for (SocketList::const_iterator it = exceptList.begin(); it != exceptList.end(); ++it)
				{
					nequeo_socket_t fd = it->sockfd();
					if (fd != NEQUEO_INVALID_SOCKET)
					{
						if (FD_ISSET(fd, &fdExcept))
							readyExceptList.push_back(*it);
					}
				}
				std::swap(exceptList, readyExceptList);
				return rc;
			}

			bool Socket::supportsIPv6()
			{
				return true;
			}

			void Socket::init(int af)
			{
				_pImpl->init(af);
			}
		}
	}
}