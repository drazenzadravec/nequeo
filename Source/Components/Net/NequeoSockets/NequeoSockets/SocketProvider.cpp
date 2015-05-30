/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          SocketProvider.cpp
*  Purpose :       SocketProvider class.
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

#include "SocketProvider.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

Nequeo::Net::Sockets::SocketProvider::SocketProvider() : _sockfd(NEQUEO_INVALID_SOCKET), _blocking(true), _disposed(false), _current(Nequeo::Net::Sockets::IPv4)
{
	Nequeo::Net::Sockets::InitializeNetwork();
}


Nequeo::Net::Sockets::SocketProvider::SocketProvider(nequeo_socket_t sockfd) : _sockfd(sockfd), _blocking(true), _disposed(false), _current(Nequeo::Net::Sockets::IPv4)
{
	Nequeo::Net::Sockets::InitializeNetwork();
}

Nequeo::Net::Sockets::SocketProvider::~SocketProvider()
{
	// If not disposed.
	if (!_disposed)
	{
		close();
		Nequeo::Net::Sockets::UninitializeNetwork();
		_disposed = true;
	}
}


Nequeo::Net::Sockets::SocketProvider* Nequeo::Net::Sockets::SocketProvider::acceptConnection(SocketAddress& clientAddr)
{
	if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();

	_current = clientAddr.addressFamily();

	char buffer[Nequeo::Net::Sockets::IPv4Length];

	if (clientAddr.addressFamily() == Nequeo::Net::Sockets::IPv6)
		buffer[Nequeo::Net::Sockets::IPv6Length];

	struct sockaddr* pSA = reinterpret_cast<struct sockaddr*>(buffer);
	nequeo_socklen_t saLen = sizeof(buffer);
	nequeo_socket_t sd;
	do
	{
		sd = ::accept(_sockfd, pSA, &saLen);
	} while (sd == NEQUEO_INVALID_SOCKET && lastError() == NEQUEO_EINTR);
	if (sd != NEQUEO_INVALID_SOCKET)
	{
		clientAddr = SocketAddress(pSA, saLen);
		return new Nequeo::Net::Sockets::SocketProvider(sd);
	}
	error(); // will throw
	return 0;
}


void Nequeo::Net::Sockets::SocketProvider::connect(const SocketAddress& address)
{
	if (_sockfd == NEQUEO_INVALID_SOCKET)
	{
		init(address.af());
	}

	_current = address.addressFamily();
	int rc;
	do
	{
		rc = ::connect(_sockfd, address.addr(), address.length());
	} while (rc != 0 && lastError() == NEQUEO_EINTR);
	if (rc != 0)
	{
		int err = lastError();
		error(err, address.toString());
	}
}


void Nequeo::Net::Sockets::SocketProvider::connect(const SocketAddress& address, const Nequeo::Timespan& timeout)
{
	if (_sockfd == NEQUEO_INVALID_SOCKET)
	{
		init(address.af());
	}

	_current = address.addressFamily();
	setBlocking(false);
	try
	{
		int rc = ::connect(_sockfd, address.addr(), address.length());
		if (rc != 0)
		{
			int err = lastError();
			if (err != NEQUEO_EINPROGRESS && err != NEQUEO_EWOULDBLOCK)
				error(err, address.toString());
			if (!poll(timeout, Nequeo::Net::Sockets::SELECT_READ | Nequeo::Net::Sockets::SELECT_WRITE | Nequeo::Net::Sockets::SELECT_ERROR))
				throw Nequeo::Exceptions::TimeoutException("connect timed out", address.toString());
			err = socketError();
			if (err != 0) error(err);
		}
	}
	catch (Nequeo::Exception&)
	{
		setBlocking(true);
		throw;
	}
	setBlocking(true);
}


void Nequeo::Net::Sockets::SocketProvider::connectNB(const SocketAddress& address)
{
	if (_sockfd == NEQUEO_INVALID_SOCKET)
	{
		init(address.af());
	}

	_current = address.addressFamily();
	setBlocking(false);
	int rc = ::connect(_sockfd, address.addr(), address.length());
	if (rc != 0)
	{
		int err = lastError();
		if (err != NEQUEO_EINPROGRESS && err != NEQUEO_EWOULDBLOCK)
			error(err, address.toString());
	}
}


void Nequeo::Net::Sockets::SocketProvider::bind(const SocketAddress& address, bool reuseAddress)
{
	if (_sockfd == NEQUEO_INVALID_SOCKET)
	{
		init(address.af());
	}
	if (reuseAddress)
	{
		setReuseAddress(true);
		setReusePort(true);
	}

	_current = address.addressFamily();
	int rc = ::bind(_sockfd, address.addr(), address.length());
	if (rc != 0) error(address.toString());
}


void Nequeo::Net::Sockets::SocketProvider::bind6(const SocketAddress& address, bool reuseAddress, bool ipV6Only)
{
	if (address.addressFamily() == Nequeo::Net::Sockets::IPv6)
		throw Nequeo::Exceptions::InvalidArgumentException("SocketAddress must be an IPv6 address");

	if (_sockfd == NEQUEO_INVALID_SOCKET)
	{
		init(address.af());
	}
	setOption(IPPROTO_IPV6, IPV6_V6ONLY, ipV6Only ? 1 : 0);
	if (reuseAddress)
	{
		setReuseAddress(true);
		setReusePort(true);
	}

	_current = address.addressFamily();
	int rc = ::bind(_sockfd, address.addr(), address.length());
	if (rc != 0) error(address.toString());
}


void Nequeo::Net::Sockets::SocketProvider::listen(int backlog)
{
	if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();

	int rc = ::listen(_sockfd, backlog);
	if (rc != 0) error();
}


void Nequeo::Net::Sockets::SocketProvider::close()
{
	if (_sockfd != NEQUEO_INVALID_SOCKET)
	{
		nequeo_closesocket(_sockfd);
		_sockfd = NEQUEO_INVALID_SOCKET;
	}
}


void Nequeo::Net::Sockets::SocketProvider::shutdownReceive()
{
	if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();

	int rc = ::shutdown(_sockfd, 0);
	if (rc != 0) error();
}


void Nequeo::Net::Sockets::SocketProvider::shutdownSend()
{
	if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();

	int rc = ::shutdown(_sockfd, 1);
	if (rc != 0) error();
}


void Nequeo::Net::Sockets::SocketProvider::shutdown()
{
	if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();

	int rc = ::shutdown(_sockfd, 2);
	if (rc != 0) error();
}


int Nequeo::Net::Sockets::SocketProvider::sendBytes(const void* buffer, int length, int flags)
{
#if defined(NEQUEO_BROKEN_TIMEOUTS)
	if (_sndTimeout.totalMicroseconds() != 0)
	{
		if (!poll(_sndTimeout, Nequeo::Net::Sockets::SELECT_WRITE))
			throw Nequeo::Exceptions::TimeoutException();
	}
#endif

	int rc;
	do
	{
		if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();
		rc = ::send(_sockfd, reinterpret_cast<const char*>(buffer), length, flags);
	} while (rc < 0 && lastError() == NEQUEO_EINTR);
	if (rc < 0) error();
	return rc;
}


int Nequeo::Net::Sockets::SocketProvider::receiveBytes(void* buffer, int length, int flags)
{
#if defined(POCO_BROKEN_TIMEOUTS)
	if (_recvTimeout.totalMicroseconds() != 0)
	{
		if (!poll(_recvTimeout, Nequeo::Net::Sockets::SELECT_READ))
			throw Nequeo::Exceptions::TimeoutException();
	}
#endif

	int rc;
	do
	{
		if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();
		rc = ::recv(_sockfd, reinterpret_cast<char*>(buffer), length, flags);
	} while (rc < 0 && lastError() == NEQUEO_EINTR);
	if (rc < 0)
	{
		int err = lastError();
		if (err == NEQUEO_EAGAIN || err == NEQUEO_ETIMEDOUT)
			throw Nequeo::Exceptions::TimeoutException();
		else
			error(err);
	}
	return rc;
}


int Nequeo::Net::Sockets::SocketProvider::sendTo(const void* buffer, int length, const SocketAddress& address, int flags)
{
	_current = address.addressFamily();
	int rc;
	do
	{
		if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();
		rc = ::sendto(_sockfd, reinterpret_cast<const char*>(buffer), length, flags, address.addr(), address.length());

	} while (rc < 0 && lastError() == NEQUEO_EINTR);
	if (rc < 0) error();
	return rc;
}


int Nequeo::Net::Sockets::SocketProvider::receiveFrom(void* buffer, int length, SocketAddress& address, int flags)
{
#if defined(POCO_BROKEN_TIMEOUTS)
	if (_recvTimeout.totalMicroseconds() != 0)
	{
		if (!poll(_recvTimeout, Nequeo::Net::Sockets::SELECT_READ))
			throw Nequeo::Exceptions::TimeoutException();
	}
#endif

	_current = address.addressFamily();
	char abuffer[Nequeo::Net::Sockets::IPv4Length];

	if (address.addressFamily() == Nequeo::Net::Sockets::IPv6)
		abuffer[Nequeo::Net::Sockets::IPv6Length];

	struct sockaddr* pSA = reinterpret_cast<struct sockaddr*>(abuffer);
	nequeo_socklen_t saLen = sizeof(abuffer);
	int rc;
	do
	{
		if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();
		rc = ::recvfrom(_sockfd, reinterpret_cast<char*>(buffer), length, flags, pSA, &saLen);
	} while (rc < 0 && lastError() == NEQUEO_EINTR);
	if (rc >= 0)
	{
		address = SocketAddress(pSA, saLen);
	}
	else
	{
		int err = lastError();
		if (err == NEQUEO_EAGAIN || err == NEQUEO_ETIMEDOUT)
			throw Nequeo::Exceptions::TimeoutException();
		else
			error(err);
	}
	return rc;
}


void Nequeo::Net::Sockets::SocketProvider::sendUrgent(unsigned char data)
{
	if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();

	int rc = ::send(_sockfd, reinterpret_cast<const char*>(&data), sizeof(data), MSG_OOB);
	if (rc < 0) error();
}


int Nequeo::Net::Sockets::SocketProvider::available()
{
	int result;
	ioctl(FIONREAD, result);
	return result;
}


bool Nequeo::Net::Sockets::SocketProvider::secure() const
{
	return false;
}

bool Nequeo::Net::Sockets::SocketProvider::poll(const Nequeo::Timespan& timeout, int mode)
{
	nequeo_socket_t sockfd = _sockfd;
	if (sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();

	fd_set fdRead;
	fd_set fdWrite;
	fd_set fdExcept;
	FD_ZERO(&fdRead);
	FD_ZERO(&fdWrite);
	FD_ZERO(&fdExcept);
	if (mode & SELECT_READ)
	{
		FD_SET(sockfd, &fdRead);
	}
	if (mode & SELECT_WRITE)
	{
		FD_SET(sockfd, &fdWrite);
	}
	if (mode & SELECT_ERROR)
	{
		FD_SET(sockfd, &fdExcept);
	}
	Nequeo::Timespan remainingTime(timeout);
	int errorCode;
	int rc;
	do
	{
		struct timeval tv;
		tv.tv_sec = (long)remainingTime.totalSeconds();
		tv.tv_usec = (long)remainingTime.useconds();
		Nequeo::Timestamp start;
		rc = ::select(int(sockfd) + 1, &fdRead, &fdWrite, &fdExcept, &tv);
		if (rc < 0 && (errorCode = lastError()) == NEQUEO_EINTR)
		{
			Nequeo::Timestamp end;
			Nequeo::Timespan waited = end - start;
			if (waited < remainingTime)
				remainingTime -= waited;
			else
				remainingTime = 0;
		}
	} while (rc < 0 && errorCode == NEQUEO_EINTR);
	if (rc < 0) error(errorCode);
	return rc > 0;
}


void Nequeo::Net::Sockets::SocketProvider::setSendBufferSize(int size)
{
	setOption(SOL_SOCKET, SO_SNDBUF, size);
}


int Nequeo::Net::Sockets::SocketProvider::getSendBufferSize()
{
	int result;
	getOption(SOL_SOCKET, SO_SNDBUF, result);
	return result;
}


void Nequeo::Net::Sockets::SocketProvider::setReceiveBufferSize(int size)
{
	setOption(SOL_SOCKET, SO_RCVBUF, size);
}


int Nequeo::Net::Sockets::SocketProvider::getReceiveBufferSize()
{
	int result;
	getOption(SOL_SOCKET, SO_RCVBUF, result);
	return result;
}


void Nequeo::Net::Sockets::SocketProvider::setSendTimeout(const Nequeo::Timespan& timeout)
{
#if defined(_WIN32) && !defined(NEQUEO_BROKEN_TIMEOUTS)
	int value = (int)timeout.totalMilliseconds();
	setOption(SOL_SOCKET, SO_SNDTIMEO, value);
#elif defined(NEQUEO_BROKEN_TIMEOUTS)
	_sndTimeout = timeout;
#else
	setOption(SOL_SOCKET, SO_SNDTIMEO, timeout);
#endif
}


Nequeo::Timespan Nequeo::Net::Sockets::SocketProvider::getSendTimeout()
{
	Nequeo::Timespan result;
#if defined(_WIN32) && !defined(NEQUEO_BROKEN_TIMEOUTS)
	int value;
	getOption(SOL_SOCKET, SO_SNDTIMEO, value);
	result = Nequeo::Timespan::TimeDiff(value) * 1000;
#elif defined(NEQUEO_BROKEN_TIMEOUTS)
	result = _sndTimeout;
#else
	getOption(SOL_SOCKET, SO_SNDTIMEO, result);
#endif
	return result;
}


void Nequeo::Net::Sockets::SocketProvider::setReceiveTimeout(const Nequeo::Timespan& timeout)
{
#ifndef NEQUEO_BROKEN_TIMEOUTS
#if defined(_WIN32)
	int value = (int)timeout.totalMilliseconds();
	setOption(SOL_SOCKET, SO_RCVTIMEO, value);
#else
	setOption(SOL_SOCKET, SO_RCVTIMEO, timeout);
#endif
#else
	_recvTimeout = timeout;
#endif
}


Nequeo::Timespan Nequeo::Net::Sockets::SocketProvider::getReceiveTimeout()
{
	Nequeo::Timespan result;
#if defined(_WIN32) && !defined(NEQUEO_BROKEN_TIMEOUTS)
	int value;
	getOption(SOL_SOCKET, SO_RCVTIMEO, value);
	result = Nequeo::Timespan::TimeDiff(value) * 1000;
#elif defined(NEQUEO_BROKEN_TIMEOUTS)
	result = _recvTimeout;
#else
	getOption(SOL_SOCKET, SO_RCVTIMEO, result);
#endif
	return result;
}


Nequeo::Net::Sockets::SocketAddress Nequeo::Net::Sockets::SocketProvider::address()
{
	if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();
	
	char buffer[Nequeo::Net::Sockets::IPv4Length];

	if (_current == Nequeo::Net::Sockets::IPv6)
		buffer[Nequeo::Net::Sockets::IPv6Length];

	struct sockaddr* pSA = reinterpret_cast<struct sockaddr*>(buffer);
	nequeo_socklen_t saLen = sizeof(buffer);
	int rc = ::getsockname(_sockfd, pSA, &saLen);
	if (rc == 0)
		return SocketAddress(pSA, saLen);
	else
		error();
	return SocketAddress();
}


Nequeo::Net::Sockets::SocketAddress Nequeo::Net::Sockets::SocketProvider::peerAddress()
{
	if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();

	char buffer[Nequeo::Net::Sockets::IPv4Length];

	if (_current == Nequeo::Net::Sockets::IPv6)
		buffer[Nequeo::Net::Sockets::IPv6Length];

	struct sockaddr* pSA = reinterpret_cast<struct sockaddr*>(buffer);
	nequeo_socklen_t saLen = sizeof(buffer);
	int rc = ::getpeername(_sockfd, pSA, &saLen);
	if (rc == 0)
		return SocketAddress(pSA, saLen);
	else
		error();
	return SocketAddress();
}


void Nequeo::Net::Sockets::SocketProvider::setOption(int level, int option, int value)
{
	setRawOption(level, option, &value, sizeof(value));
}


void Nequeo::Net::Sockets::SocketProvider::setOption(int level, int option, unsigned value)
{
	setRawOption(level, option, &value, sizeof(value));
}


void Nequeo::Net::Sockets::SocketProvider::setOption(int level, int option, unsigned char value)
{
	setRawOption(level, option, &value, sizeof(value));
}


void Nequeo::Net::Sockets::SocketProvider::setOption(int level, int option, const IPAddress& value)
{
	setRawOption(level, option, value.addr(), value.length());
}


void Nequeo::Net::Sockets::SocketProvider::setOption(int level, int option, const Nequeo::Timespan& value)
{
	struct timeval tv;
	tv.tv_sec = (long)value.totalSeconds();
	tv.tv_usec = (long)value.useconds();

	setRawOption(level, option, &tv, sizeof(tv));
}


void Nequeo::Net::Sockets::SocketProvider::setRawOption(int level, int option, const void* value, nequeo_socklen_t length)
{
	if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();

	int rc = ::setsockopt(_sockfd, level, option, reinterpret_cast<const char*>(value), length);
	if (rc == -1) error();
}


void Nequeo::Net::Sockets::SocketProvider::getOption(int level, int option, int& value)
{
	nequeo_socklen_t len = sizeof(value);
	getRawOption(level, option, &value, len);
}


void Nequeo::Net::Sockets::SocketProvider::getOption(int level, int option, unsigned& value)
{
	nequeo_socklen_t len = sizeof(value);
	getRawOption(level, option, &value, len);
}


void Nequeo::Net::Sockets::SocketProvider::getOption(int level, int option, unsigned char& value)
{
	nequeo_socklen_t len = sizeof(value);
	getRawOption(level, option, &value, len);
}


void Nequeo::Net::Sockets::SocketProvider::getOption(int level, int option, Nequeo::Timespan& value)
{
	struct timeval tv;
	nequeo_socklen_t len = sizeof(tv);
	getRawOption(level, option, &tv, len);
	value.assign(tv.tv_sec, tv.tv_usec);
}


void Nequeo::Net::Sockets::SocketProvider::getOption(int level, int option, IPAddress& value)
{
	char buffer[Nequeo::Net::Sockets::IPv4Length];

	if (value.addressFamily() == Nequeo::Net::Sockets::IPv6)
		buffer[Nequeo::Net::Sockets::IPv6Length];

	nequeo_socklen_t len = sizeof(buffer);
	getRawOption(level, option, buffer, len);
	value = IPAddress(buffer, len);
}


void Nequeo::Net::Sockets::SocketProvider::getRawOption(int level, int option, void* value, nequeo_socklen_t& length)
{
	if (_sockfd == NEQUEO_INVALID_SOCKET) throw Nequeo::Exceptions::Net::InvalidSocketException();

	int rc = ::getsockopt(_sockfd, level, option, reinterpret_cast<char*>(value), &length);
	if (rc == -1) error();
}


void Nequeo::Net::Sockets::SocketProvider::setLinger(bool on, int seconds)
{
	struct linger l;
	l.l_onoff = on ? 1 : 0;
	l.l_linger = seconds;
	setRawOption(SOL_SOCKET, SO_LINGER, &l, sizeof(l));
}


void Nequeo::Net::Sockets::SocketProvider::getLinger(bool& on, int& seconds)
{
	struct linger l;
	nequeo_socklen_t len = sizeof(l);
	getRawOption(SOL_SOCKET, SO_LINGER, &l, len);
	on = l.l_onoff != 0;
	seconds = l.l_linger;
}


void Nequeo::Net::Sockets::SocketProvider::setNoDelay(bool flag)
{
	int value = flag ? 1 : 0;
	setOption(IPPROTO_TCP, TCP_NODELAY, value);
}


bool Nequeo::Net::Sockets::SocketProvider::getNoDelay()
{
	int value(0);
	getOption(IPPROTO_TCP, TCP_NODELAY, value);
	return value != 0;
}


void Nequeo::Net::Sockets::SocketProvider::setKeepAlive(bool flag)
{
	int value = flag ? 1 : 0;
	setOption(SOL_SOCKET, SO_KEEPALIVE, value);
}


bool Nequeo::Net::Sockets::SocketProvider::getKeepAlive()
{
	int value(0);
	getOption(SOL_SOCKET, SO_KEEPALIVE, value);
	return value != 0;
}


void Nequeo::Net::Sockets::SocketProvider::setReuseAddress(bool flag)
{
	int value = flag ? 1 : 0;
	setOption(SOL_SOCKET, SO_REUSEADDR, value);
}


bool Nequeo::Net::Sockets::SocketProvider::getReuseAddress()
{
	int value(0);
	getOption(SOL_SOCKET, SO_REUSEADDR, value);
	return value != 0;
}


void Nequeo::Net::Sockets::SocketProvider::setReusePort(bool flag)
{
	try
	{
		int value = flag ? 1 : 0;
		setOption(SOL_SOCKET, SO_REUSEPORT, value);
	}
	catch (Nequeo::Exceptions::IOException&)
	{
		// ignore error, since not all implementations
		// support SO_REUSEPORT, even if the macro
		// is defined.
	}
}


bool Nequeo::Net::Sockets::SocketProvider::getReusePort()
{
#ifdef SO_REUSEPORT
	int value(0);
	getOption(SOL_SOCKET, SO_REUSEPORT, value);
	return value != 0;
#else
	return false;
#endif
}


void Nequeo::Net::Sockets::SocketProvider::setOOBInline(bool flag)
{
	int value = flag ? 1 : 0;
	setOption(SOL_SOCKET, SO_OOBINLINE, value);
}


bool Nequeo::Net::Sockets::SocketProvider::getOOBInline()
{
	int value(0);
	getOption(SOL_SOCKET, SO_OOBINLINE, value);
	return value != 0;
}


void Nequeo::Net::Sockets::SocketProvider::setBroadcast(bool flag)
{
	int value = flag ? 1 : 0;
	setOption(SOL_SOCKET, SO_BROADCAST, value);
}


bool Nequeo::Net::Sockets::SocketProvider::getBroadcast()
{
	int value(0);
	getOption(SOL_SOCKET, SO_BROADCAST, value);
	return value != 0;
}


void Nequeo::Net::Sockets::SocketProvider::setBlocking(bool flag)
{
	int arg = flag ? 0 : 1;
	ioctl(FIONBIO, arg);
	_blocking = flag;
}


int Nequeo::Net::Sockets::SocketProvider::socketError()
{
	int result(0);
	getOption(SOL_SOCKET, SO_ERROR, result);
	return result;
}


void Nequeo::Net::Sockets::SocketProvider::init(int af)
{
	initSocket(af, SOCK_STREAM);
}


void Nequeo::Net::Sockets::SocketProvider::initSocket(int af, int type, int proto)
{
	if (_sockfd == NEQUEO_INVALID_SOCKET)
	{
		_sockfd = ::socket(af, type, proto);
		if (_sockfd == NEQUEO_INVALID_SOCKET)
			error();
	}
}


void Nequeo::Net::Sockets::SocketProvider::ioctl(nequeo_ioctl_request_t request, int& arg)
{
	int rc = ioctlsocket(_sockfd, request, reinterpret_cast<u_long*>(&arg));
	if (rc != 0) error();
}


void Nequeo::Net::Sockets::SocketProvider::ioctl(nequeo_ioctl_request_t request, void* arg)
{
	int rc = ioctlsocket(_sockfd, request, reinterpret_cast<u_long*>(arg));
	if (rc != 0) error();
}


void Nequeo::Net::Sockets::SocketProvider::reset(nequeo_socket_t aSocket)
{
	_sockfd = aSocket;
}


void Nequeo::Net::Sockets::SocketProvider::error()
{
	int err = lastError();
	std::string empty;
	error(err, empty);
}


void Nequeo::Net::Sockets::SocketProvider::error(const std::string& arg)
{
	error(lastError(), arg);
}


void Nequeo::Net::Sockets::SocketProvider::error(int code)
{
	std::string arg;
	error(code, arg);
}


void Nequeo::Net::Sockets::SocketProvider::error(int code, const std::string& arg)
{
	switch (code)
	{
	case NEQUEO_ESYSNOTREADY:
		throw  Nequeo::Exceptions::Net::NetException("Net subsystem not ready", code);
	case NEQUEO_ENOTINIT:
		throw  Nequeo::Exceptions::Net::NetException("Net subsystem not initialized", code);
	case NEQUEO_EINTR:
		throw  Nequeo::Exceptions::IOException("Interrupted", code);
	case NEQUEO_EACCES:
		throw  Nequeo::Exceptions::IOException("Permission denied", code);
	case NEQUEO_EFAULT:
		throw  Nequeo::Exceptions::IOException("Bad address", code);
	case NEQUEO_EINVAL:
		throw  Nequeo::Exceptions::InvalidArgumentException(code);
	case NEQUEO_EMFILE:
		throw  Nequeo::Exceptions::IOException("Too many open files", code);
	case NEQUEO_EWOULDBLOCK:
		throw  Nequeo::Exceptions::IOException("Operation would block", code);
	case NEQUEO_EINPROGRESS:
		throw  Nequeo::Exceptions::IOException("Operation now in progress", code);
	case NEQUEO_EALREADY:
		throw  Nequeo::Exceptions::IOException("Operation already in progress", code);
	case NEQUEO_ENOTSOCK:
		throw  Nequeo::Exceptions::IOException("Socket operation attempted on non-socket", code);
	case NEQUEO_EDESTADDRREQ:
		throw  Nequeo::Exceptions::Net::NetException("Destination address required", code);
	case NEQUEO_EMSGSIZE:
		throw  Nequeo::Exceptions::Net::NetException("Message too long", code);
	case NEQUEO_EPROTOTYPE:
		throw  Nequeo::Exceptions::Net::NetException("Wrong protocol type", code);
	case NEQUEO_ENOPROTOOPT:
		throw  Nequeo::Exceptions::Net::NetException("Protocol not available", code);
	case NEQUEO_EPROTONOSUPPORT:
		throw  Nequeo::Exceptions::Net::NetException("Protocol not supported", code);
	case NEQUEO_ESOCKTNOSUPPORT:
		throw  Nequeo::Exceptions::Net::NetException("Socket type not supported", code);
	case NEQUEO_ENOTSUP:
		throw  Nequeo::Exceptions::Net::NetException("Operation not supported", code);
	case NEQUEO_EPFNOSUPPORT:
		throw  Nequeo::Exceptions::Net::NetException("Protocol family not supported", code);
	case NEQUEO_EAFNOSUPPORT:
		throw  Nequeo::Exceptions::Net::NetException("Address family not supported", code);
	case NEQUEO_EADDRINUSE:
		throw  Nequeo::Exceptions::Net::NetException("Address already in use", arg, code);
	case NEQUEO_EADDRNOTAVAIL:
		throw  Nequeo::Exceptions::Net::NetException("Cannot assign requested address", arg, code);
	case NEQUEO_ENETDOWN:
		throw  Nequeo::Exceptions::Net::NetException("Network is down", code);
	case NEQUEO_ENETUNREACH:
		throw  Nequeo::Exceptions::Net::NetException("Network is unreachable", code);
	case NEQUEO_ENETRESET:
		throw  Nequeo::Exceptions::Net::NetException("Network dropped connection on reset", code);
	case NEQUEO_ECONNABORTED:
		throw  Nequeo::Exceptions::Net::ConnectionAbortedException(code);
	case NEQUEO_ECONNRESET:
		throw  Nequeo::Exceptions::Net::ConnectionResetException(code);
	case NEQUEO_ENOBUFS:
		throw  Nequeo::Exceptions::IOException("No buffer space available", code);
	case NEQUEO_EISCONN:
		throw  Nequeo::Exceptions::Net::NetException("Socket is already connected", code);
	case NEQUEO_ENOTCONN:
		throw  Nequeo::Exceptions::Net::NetException("Socket is not connected", code);
	case NEQUEO_ESHUTDOWN:
		throw  Nequeo::Exceptions::Net::NetException("Cannot send after socket shutdown", code);
	case NEQUEO_ETIMEDOUT:
		throw  Nequeo::Exceptions::TimeoutException(code);
	case NEQUEO_ECONNREFUSED:
		throw  Nequeo::Exceptions::Net::ConnectionRefusedException(arg, code);
	case NEQUEO_EHOSTDOWN:
		throw  Nequeo::Exceptions::Net::NetException("Host is down", arg, code);
	case NEQUEO_EHOSTUNREACH:
		throw  Nequeo::Exceptions::Net::NetException("No route to host", arg, code);
	default:
		throw  Nequeo::Exceptions::IOException(NumberFormatter::format(code), arg, code);
	}
}

//
// inlines
//
inline nequeo_socket_t Nequeo::Net::Sockets::SocketProvider::sockfd() const
{
	return _sockfd;
}


inline bool Nequeo::Net::Sockets::SocketProvider::initialized() const
{
	return _sockfd != NEQUEO_INVALID_SOCKET;
}


inline int Nequeo::Net::Sockets::SocketProvider::lastError()
{
	return WSAGetLastError();
}


inline bool Nequeo::Net::Sockets::SocketProvider::getBlocking() const
{
	return _blocking;
}