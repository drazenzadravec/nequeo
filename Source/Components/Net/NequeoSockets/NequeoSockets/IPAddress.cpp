/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          IPAddress.cpp
*  Purpose :       IPAddress class.
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

#include "IPAddress.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

///
Nequeo::Net::Sockets::IPAddress::IPAddress() : _pImpl(new Nequeo::Net::Sockets::IPv4AddressProvider), _disposed(false)
{
}

///
Nequeo::Net::Sockets::IPAddress::IPAddress(const Nequeo::Net::Sockets::IPAddress& addr) : _pImpl(addr._pImpl), _disposed(false)
{
	_pImpl->duplicate();
}

///
Nequeo::Net::Sockets::IPAddress::IPAddress(Nequeo::Net::Sockets::AddressFamily addressFamily) : _pImpl(0), _disposed(false)
{
	if (addressFamily == IPv4)
		_pImpl = new Nequeo::Net::Sockets::IPv4AddressProvider();
	else if (addressFamily == IPv6)
		_pImpl = new Nequeo::Net::Sockets::IPv6AddressProvider();
	else
		throw Nequeo::Exceptions::InvalidArgumentException("Invalid or unsupported address family passed to IPAddress()");
}


Nequeo::Net::Sockets::IPAddress::IPAddress(const std::string& addr)
{
	_pImpl = Nequeo::Net::Sockets::IPv4AddressProvider::parse(addr);
	if (!_pImpl)
		_pImpl = Nequeo::Net::Sockets::IPv6AddressProvider::parse(addr);

	if (!_pImpl) throw Nequeo::Exceptions::Net::InvalidAddressException(addr);
}


Nequeo::Net::Sockets::IPAddress::IPAddress(const std::string& addr, Nequeo::Net::Sockets::AddressFamily addressFamily) : _pImpl(0), _disposed(false)
{
	if (addressFamily == IPv4)
		_pImpl = Nequeo::Net::Sockets::IPv4AddressProvider::parse(addr);
	else if (addressFamily == IPv6)
		_pImpl = Nequeo::Net::Sockets::IPv6AddressProvider::parse(addr);
	else throw Nequeo::Exceptions::InvalidArgumentException("Invalid or unsupported address family passed to IPAddress()");

	if (!_pImpl) throw Nequeo::Exceptions::Net::InvalidAddressException(addr);
}


Nequeo::Net::Sockets::IPAddress::IPAddress(const void* addr, nequeo_socklen_t length)
{
	if (length == sizeof(struct in_addr))
		_pImpl = new Nequeo::Net::Sockets::IPv4AddressProvider(addr);
	else if (length == sizeof(struct in6_addr))
		_pImpl = new Nequeo::Net::Sockets::IPv6AddressProvider(addr);
	else throw Nequeo::Exceptions::InvalidArgumentException("Invalid address length passed to IPAddress()");
}


Nequeo::Net::Sockets::IPAddress::IPAddress(const void* addr, nequeo_socklen_t length, UInt32 scope)
{
	if (length == sizeof(struct in_addr))
		_pImpl = new Nequeo::Net::Sockets::IPv4AddressProvider(addr);
	else if (length == sizeof(struct in6_addr))
		_pImpl = new Nequeo::Net::Sockets::IPv6AddressProvider(addr, scope);
	else throw Nequeo::Exceptions::InvalidArgumentException("Invalid address length passed to IPAddress()");
}

///
Nequeo::Net::Sockets::IPAddress::~IPAddress()
{
	// If not disposed.
	if (!_disposed)
	{
		_pImpl->release();
		_disposed = true;
	}
}


Nequeo::Net::Sockets::IPAddress& Nequeo::Net::Sockets::IPAddress::operator = (const Nequeo::Net::Sockets::IPAddress& addr)
{
	if (&addr != this)
	{
		_pImpl->release();
		_pImpl = addr._pImpl;
		_pImpl->duplicate();
	}
	return *this;
}


void Nequeo::Net::Sockets::IPAddress::swap(Nequeo::Net::Sockets::IPAddress& address)
{
	std::swap(_pImpl, address._pImpl);
}


Nequeo::Net::Sockets::AddressFamily Nequeo::Net::Sockets::IPAddress::addressFamily() const
{
	return _pImpl->addressFamily();
}

///
UInt32 Nequeo::Net::Sockets::IPAddress::scope() const
{
	return _pImpl->scope();
}


std::string Nequeo::Net::Sockets::IPAddress::toString() const
{
	return _pImpl->toString();
}


bool Nequeo::Net::Sockets::IPAddress::isWildcard() const
{
	return _pImpl->isWildcard();
}

bool Nequeo::Net::Sockets::IPAddress::isBroadcast() const
{
	return _pImpl->isBroadcast();
}


bool Nequeo::Net::Sockets::IPAddress::isLoopback() const
{
	return _pImpl->isLoopback();
}


bool Nequeo::Net::Sockets::IPAddress::isMulticast() const
{
	return _pImpl->isMulticast();
}


bool Nequeo::Net::Sockets::IPAddress::isUnicast() const
{
	return !isWildcard() && !isBroadcast() && !isMulticast();
}


bool Nequeo::Net::Sockets::IPAddress::isLinkLocal() const
{
	return _pImpl->isLinkLocal();
}


bool Nequeo::Net::Sockets::IPAddress::isSiteLocal() const
{
	return _pImpl->isSiteLocal();
}


bool Nequeo::Net::Sockets::IPAddress::isIPv4Compatible() const
{
	return _pImpl->isIPv4Compatible();
}


bool Nequeo::Net::Sockets::IPAddress::isIPv4Mapped() const
{
	return _pImpl->isIPv4Mapped();
}


bool Nequeo::Net::Sockets::IPAddress::isWellKnownMC() const
{
	return _pImpl->isWellKnownMC();
}


bool Nequeo::Net::Sockets::IPAddress::isNodeLocalMC() const
{
	return _pImpl->isNodeLocalMC();
}


bool Nequeo::Net::Sockets::IPAddress::isLinkLocalMC() const
{
	return _pImpl->isLinkLocalMC();
}


bool Nequeo::Net::Sockets::IPAddress::isSiteLocalMC() const
{
	return _pImpl->isSiteLocalMC();
}


bool Nequeo::Net::Sockets::IPAddress::isOrgLocalMC() const
{
	return _pImpl->isOrgLocalMC();
}


bool Nequeo::Net::Sockets::IPAddress::isGlobalMC() const
{
	return _pImpl->isGlobalMC();
}


bool Nequeo::Net::Sockets::IPAddress::operator == (const Nequeo::Net::Sockets::IPAddress& a) const
{
	nequeo_socklen_t l1 = length();
	nequeo_socklen_t l2 = a.length();
	if (l1 == l2)
		return std::memcmp(addr(), a.addr(), l1) == 0;
	else
		return false;
}


bool Nequeo::Net::Sockets::IPAddress::operator != (const Nequeo::Net::Sockets::IPAddress& a) const
{
	nequeo_socklen_t l1 = length();
	nequeo_socklen_t l2 = a.length();
	if (l1 == l2)
		return std::memcmp(addr(), a.addr(), l1) != 0;
	else
		return true;
}


bool Nequeo::Net::Sockets::IPAddress::operator < (const Nequeo::Net::Sockets::IPAddress& a) const
{
	nequeo_socklen_t l1 = length();
	nequeo_socklen_t l2 = a.length();
	if (l1 == l2)
		return std::memcmp(addr(), a.addr(), l1) < 0;
	else
		return l1 < l2;
}


bool Nequeo::Net::Sockets::IPAddress::operator <= (const Nequeo::Net::Sockets::IPAddress& a) const
{
	nequeo_socklen_t l1 = length();
	nequeo_socklen_t l2 = a.length();
	if (l1 == l2)
		return std::memcmp(addr(), a.addr(), l1) <= 0;
	else
		return l1 < l2;
}


bool Nequeo::Net::Sockets::IPAddress::operator >(const Nequeo::Net::Sockets::IPAddress& a) const
{
	nequeo_socklen_t l1 = length();
	nequeo_socklen_t l2 = a.length();
	if (l1 == l2)
		return std::memcmp(addr(), a.addr(), l1) > 0;
	else
		return l1 > l2;
}


bool Nequeo::Net::Sockets::IPAddress::operator >= (const Nequeo::Net::Sockets::IPAddress& a) const
{
	nequeo_socklen_t l1 = length();
	nequeo_socklen_t l2 = a.length();
	if (l1 == l2)
		return std::memcmp(addr(), a.addr(), l1) >= 0;
	else
		return l1 > l2;
}


nequeo_socklen_t Nequeo::Net::Sockets::IPAddress::length() const
{
	return _pImpl->length();
}


const void* Nequeo::Net::Sockets::IPAddress::addr() const
{
	return _pImpl->addr();
}


int Nequeo::Net::Sockets::IPAddress::af() const
{
	return _pImpl->af();
}


void Nequeo::Net::Sockets::IPAddress::init(IPAddressProvider* pImpl)
{
	_pImpl->release();
	_pImpl = pImpl;
}


Nequeo::Net::Sockets::IPAddress Nequeo::Net::Sockets::IPAddress::parse(const std::string& addr)
{
	return Nequeo::Net::Sockets::IPAddress(addr);
}


bool Nequeo::Net::Sockets::IPAddress::tryParse(const std::string& addr, Nequeo::Net::Sockets::IPAddress& result)
{
	IPAddressProvider* pImpl = Nequeo::Net::Sockets::IPv4AddressProvider::parse(addr);
	if (!pImpl) pImpl = Nequeo::Net::Sockets::IPv6AddressProvider::parse(addr);
	if (pImpl)
	{
		result.init(pImpl);
		return true;
	}
	else return false;
}


void Nequeo::Net::Sockets::IPAddress::mask(const Nequeo::Net::Sockets::IPAddress& mask)
{
	IPAddressProvider* pClone = _pImpl->clone();
	_pImpl->release();
	_pImpl = pClone;
	IPAddress null;
	_pImpl->mask(mask._pImpl, null._pImpl);
}


void Nequeo::Net::Sockets::IPAddress::mask(const Nequeo::Net::Sockets::IPAddress& mask, const Nequeo::Net::Sockets::IPAddress& set)
{
	IPAddressProvider* pClone = _pImpl->clone();
	_pImpl->release();
	_pImpl = pClone;
	_pImpl->mask(mask._pImpl, set._pImpl);
}


Nequeo::Net::Sockets::IPAddress Nequeo::Net::Sockets::IPAddress::wildcard(Nequeo::Net::Sockets::AddressFamily addressFamily)
{
	return Nequeo::Net::Sockets::IPAddress(addressFamily);
}


Nequeo::Net::Sockets::IPAddress Nequeo::Net::Sockets::IPAddress::broadcast()
{
	struct in_addr ia;
	ia.s_addr = INADDR_NONE;
	return Nequeo::Net::Sockets::IPAddress(&ia, sizeof(ia));
}

///
inline void swap(Nequeo::Net::Sockets::IPAddress& addr1, Nequeo::Net::Sockets::IPAddress& addr2)
{
	addr1.swap(addr2);
}
