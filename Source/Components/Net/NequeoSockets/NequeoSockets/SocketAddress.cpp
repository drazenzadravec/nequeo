/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          SocketAddress.cpp
*  Purpose :       SocketAddress class.
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

#include "SocketAddress.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			///
			struct AFLT
			{
				///
				bool operator () (const IPAddress& a1, const IPAddress& a2)
				{
					return a1.af() < a2.af();
				}
			};
		}
	}
}

Nequeo::Net::Sockets::SocketAddress::SocketAddress() : _disposed(false)
{
	_pImpl = new Nequeo::Net::Sockets::IPv4SocketAddressProvider;
}


Nequeo::Net::Sockets::SocketAddress::SocketAddress(const IPAddress& addr, UInt16 port) : _disposed(false)
{
	init(addr, port);
}


Nequeo::Net::Sockets::SocketAddress::SocketAddress(const std::string& addr, UInt16 port) : _disposed(false)
{
	init(addr, port);
}


Nequeo::Net::Sockets::SocketAddress::SocketAddress(const std::string& addr, const std::string& port) : _disposed(false)
{
	init(addr, resolveService(port));
}


Nequeo::Net::Sockets::SocketAddress::SocketAddress(const std::string& hostAndPort) : _disposed(false)
{
	if (!hostAndPort.empty())
	{
		std::string host;
		std::string port;
		std::string::const_iterator it = hostAndPort.begin();
		std::string::const_iterator end = hostAndPort.end();
		if (*it == '[')
		{
			++it;
			while (it != end && *it != ']') host += *it++;
			if (it == end) throw Nequeo::Exceptions::InvalidArgumentException("Malformed IPv6 address");
			++it;
		}
		else
		{
			while (it != end && *it != ':') host += *it++;
		}
		if (it != end && *it == ':')
		{
			++it;
			while (it != end) port += *it++;
		}
		else throw Nequeo::Exceptions::InvalidArgumentException("Missing port number");
		init(host, resolveService(port));
	}
}


Nequeo::Net::Sockets::SocketAddress::SocketAddress(const Nequeo::Net::Sockets::SocketAddress& addr) : _disposed(false)
{
	_pImpl = addr._pImpl;
	_pImpl->duplicate();
}


Nequeo::Net::Sockets::SocketAddress::SocketAddress(const struct sockaddr* addr, nequeo_socklen_t length) : _disposed(false)
{
	if (length == sizeof(struct sockaddr_in))
		_pImpl = new Nequeo::Net::Sockets::IPv4SocketAddressProvider(reinterpret_cast<const struct sockaddr_in*>(addr));
	else if (length == sizeof(struct sockaddr_in6))
		_pImpl = new Nequeo::Net::Sockets::IPv6SocketAddressProvider(reinterpret_cast<const struct sockaddr_in6*>(addr));
	else throw Nequeo::Exceptions::InvalidArgumentException("Invalid address length passed to SocketAddress()");
}


Nequeo::Net::Sockets::SocketAddress::~SocketAddress()
{
	// If not disposed.
	if (!_disposed)
	{
		_pImpl->release();
		_disposed = true;
	}
}


bool Nequeo::Net::Sockets::SocketAddress::operator < (const Nequeo::Net::Sockets::SocketAddress& addr) const
{
	if (addressFamily() < addr.addressFamily()) return true;
	if (host() < addr.host()) return true;
	return (port() < addr.port());
}


Nequeo::Net::Sockets::SocketAddress& Nequeo::Net::Sockets::SocketAddress::operator = (const Nequeo::Net::Sockets::SocketAddress& addr)
{
	if (&addr != this)
	{
		_pImpl->release();
		_pImpl = addr._pImpl;
		_pImpl->duplicate();
	}
	return *this;
}


void Nequeo::Net::Sockets::SocketAddress::swap(Nequeo::Net::Sockets::SocketAddress& addr)
{
	std::swap(_pImpl, addr._pImpl);
}


Nequeo::Net::Sockets::IPAddress Nequeo::Net::Sockets::SocketAddress::host() const
{
	return _pImpl->host();
}


UInt16 Nequeo::Net::Sockets::SocketAddress::port() const
{
	return ntohs(_pImpl->port());
}


nequeo_socklen_t Nequeo::Net::Sockets::SocketAddress::length() const
{
	return _pImpl->length();
}


const struct sockaddr* Nequeo::Net::Sockets::SocketAddress::addr() const
{
	return _pImpl->addr();
}


int Nequeo::Net::Sockets::SocketAddress::af() const
{
	return _pImpl->af();
}


std::string Nequeo::Net::Sockets::SocketAddress::toString() const
{
	std::string result;
	if (host().addressFamily() == Nequeo::Net::Sockets::IPv6)
		result.append("[");
	result.append(host().toString());
	if (host().addressFamily() == Nequeo::Net::Sockets::IPv6)
		result.append("]");
	result.append(":");
	NumberFormatter::append(result, port());
	return result;
}


void Nequeo::Net::Sockets::SocketAddress::init(const IPAddress& host, UInt16 port)
{
	if (host.addressFamily() == Nequeo::Net::Sockets::IPv4)
		_pImpl = new Nequeo::Net::Sockets::IPv4SocketAddressProvider(host.addr(), htons(port));
	else if (host.addressFamily() == Nequeo::Net::Sockets::IPv6)
		_pImpl = new Nequeo::Net::Sockets::IPv6SocketAddressProvider(host.addr(), htons(port), host.scope());
	else throw Nequeo::Exceptions::NotImplementedException("unsupported IP address family");
}


void Nequeo::Net::Sockets::SocketAddress::init(const std::string& host, UInt16 port)
{
	IPAddress ip;
	if (IPAddress::tryParse(host, ip))
	{
		init(ip, port);
	}
	else
	{
		HostEntry he = DNS::hostByName(host);
		HostEntry::AddressList addresses = he.addresses();
		if (addresses.size() > 0)
		{
			// if we get both IPv4 and IPv6 addresses, prefer IPv4
			std::sort(addresses.begin(), addresses.end(), AFLT());
			init(addresses[0], port);
		}
		else throw Nequeo::Exceptions::Net::HostNotFoundException("No address found for host", host);
	}
}


UInt16 Nequeo::Net::Sockets::SocketAddress::resolveService(const std::string& service)
{
	unsigned port;
	if (Nequeo::Primitive::NumberParser::tryParseUnsigned(service, port) && port <= 0xFFFF)
	{
		return (UInt16)port;
	}
	else
	{
		struct servent* se = getservbyname(service.c_str(), NULL);
		if (se)
			return ntohs(se->s_port);
		else
			throw Nequeo::Exceptions::Net::ServiceNotFoundException(service);
	}
}

//
// inlines
//
inline void swap(Nequeo::Net::Sockets::SocketAddress& a1, Nequeo::Net::Sockets::SocketAddress& a2)
{
	a1.swap(a2);
}

///
inline Nequeo::Net::Sockets::AddressFamily Nequeo::Net::Sockets::SocketAddress::addressFamily() const
{
	return host().addressFamily();
}

///
inline 	bool Nequeo::Net::Sockets::SocketAddress::operator == (const Nequeo::Net::Sockets::SocketAddress& addr) const
{
	return host() == addr.host() && port() == addr.port();
}

///
inline bool Nequeo::Net::Sockets::SocketAddress::operator != (const Nequeo::Net::Sockets::SocketAddress& addr) const
{
	return host() != addr.host() || port() != addr.port();
}