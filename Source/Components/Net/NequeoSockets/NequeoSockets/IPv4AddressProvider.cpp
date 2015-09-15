/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          IPv4AddressProvider.cpp
*  Purpose :       IPv4AddressProvider class.
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

#include "IPAddressProvider.h"
#include "NumberFormatter.h"

using Nequeo::Net::Sockets::IPAddressProvider;
using Nequeo::Primitive::NumberFormatter;
using Nequeo::Primitive::UInt8;
using Nequeo::Primitive::UInt16;
using Nequeo::Primitive::UInt32;

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			// IPv4Address Provider.
			class IPv4AddressProvider : public IPAddressProvider
			{
			public:
				IPv4AddressProvider()
				{
					std::memset(&_addr, 0, sizeof(_addr));
				}

				IPv4AddressProvider(const void* addr)
				{
					std::memcpy(&_addr, addr, sizeof(_addr));
				}

				std::string toString() const
				{
					const UInt8* bytes = reinterpret_cast<const UInt8*>(&_addr);
					std::string result;
					result.reserve(16);
					NumberFormatter::append(result, bytes[0]);
					result.append(".");
					NumberFormatter::append(result, bytes[1]);
					result.append(".");
					NumberFormatter::append(result, bytes[2]);
					result.append(".");
					NumberFormatter::append(result, bytes[3]);
					return result;
				}

				poco_socklen_t length() const
				{
					return sizeof(_addr);
				}

				const void* addr() const
				{
					return &_addr;
				}

				IPAddress::Family family() const
				{
					return IPAddress::IPv4;
				}

				int af() const
				{
					return AF_INET;
				}

				Nequeo::Primitive::UInt32 scope() const
				{
					return 0;
				}

				bool isWildcard() const
				{
					return _addr.s_addr == INADDR_ANY;
				}

				bool isBroadcast() const
				{
					return _addr.s_addr == INADDR_NONE;
				}

				bool isLoopback() const
				{
					return (ntohl(_addr.s_addr) & 0xFF000000) == 0x7F000000; // 127.0.0.1 to 127.255.255.255
				}

				bool isMulticast() const
				{
					return (ntohl(_addr.s_addr) & 0xF0000000) == 0xE0000000; // 224.0.0.0/24 to 239.0.0.0/24
				}

				bool isLinkLocal() const
				{
					return (ntohl(_addr.s_addr) & 0xFFFF0000) == 0xA9FE0000; // 169.254.0.0/16
				}

				bool isSiteLocal() const
				{
					UInt32 addr = ntohl(_addr.s_addr);
					return (addr & 0xFF000000) == 0x0A000000 ||        // 10.0.0.0/24
						(addr & 0xFFFF0000) == 0xC0A80000 ||        // 192.68.0.0/16
						(addr >= 0xAC100000 && addr <= 0xAC1FFFFF); // 172.16.0.0 to 172.31.255.255
				}

				bool isIPv4Compatible() const
				{
					return true;
				}

				bool isIPv4Mapped() const
				{
					return true;
				}

				bool isWellKnownMC() const
				{
					return (ntohl(_addr.s_addr) & 0xFFFFFF00) == 0xE0000000; // 224.0.0.0/8
				}

				bool isNodeLocalMC() const
				{
					return false;
				}

				bool isLinkLocalMC() const
				{
					return (ntohl(_addr.s_addr) & 0xFF000000) == 0xE0000000; // 244.0.0.0/24
				}

				bool isSiteLocalMC() const
				{
					return (ntohl(_addr.s_addr) & 0xFFFF0000) == 0xEFFF0000; // 239.255.0.0/16
				}

				bool isOrgLocalMC() const
				{
					return (ntohl(_addr.s_addr) & 0xFFFF0000) == 0xEFC00000; // 239.192.0.0/16
				}

				bool isGlobalMC() const
				{
					UInt32 addr = ntohl(_addr.s_addr);
					return addr >= 0xE0000100 && addr <= 0xEE000000; // 224.0.1.0 to 238.255.255.255
				}

				static IPv4AddressImpl* parse(const std::string& addr)
				{
					if (addr.empty()) return 0;

					struct in_addr ia;
					ia.s_addr = inet_addr(addr.c_str());
					if (ia.s_addr == INADDR_NONE && addr != "255.255.255.255")
						return 0;
					else
						return new IPv4AddressImpl(&ia);
				}

				void mask(const IPAddressImpl* pMask, const IPAddressImpl* pSet)
				{
					poco_assert(pMask->af() == AF_INET && pSet->af() == AF_INET);

					_addr.s_addr &= static_cast<const IPv4AddressImpl*>(pMask)->_addr.s_addr;
					_addr.s_addr |= static_cast<const IPv4AddressImpl*>(pSet)->_addr.s_addr & ~static_cast<const IPv4AddressImpl*>(pMask)->_addr.s_addr;
				}

				IPAddressImpl* clone() const
				{
					return new IPv4AddressImpl(&_addr);
				}

			private:
				struct in_addr _addr;
			};
		}
	}
}