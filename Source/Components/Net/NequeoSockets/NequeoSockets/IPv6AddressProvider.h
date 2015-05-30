/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          IPv6AddressProvider.cpp
*  Purpose :       IPv6AddressProvider class.
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

#ifndef _IPV6_ADDRESSPROVIDER_H
#define _IPV6_ADDRESSPROVIDER_H

#include "GlobalSocket.h"
#include "Primitive\NumberFormatter.h"
#include "AddressFamily.h"
#include "IPAddressProvider.h"

using Nequeo::Primitive::NumberFormatter;
using Nequeo::UInt8;
using Nequeo::UInt16;
using Nequeo::UInt32;

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			// IPv6Address Provider.
			class IPv6AddressProvider : public IPAddressProvider
			{
			public:
				IPv6AddressProvider() : _scope(0)
				{
					std::memset(&_addr, 0, sizeof(_addr));
				}

				IPv6AddressProvider(const void* addr) :_scope(0)
				{
					std::memcpy(&_addr, addr, sizeof(_addr));
				}

				IPv6AddressProvider(const void* addr, UInt32 scope) : _scope(scope)
				{
					std::memcpy(&_addr, addr, sizeof(_addr));
				}

				std::string toString() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					if (isIPv4Compatible() || isIPv4Mapped())
					{
						std::string result;
						result.reserve(24);
						if (words[5] == 0)
							result.append("::");
						else
							result.append("::FFFF:");
						const UInt8* bytes = reinterpret_cast<const UInt8*>(&_addr);
						NumberFormatter::append(result, bytes[12]);
						result.append(".");
						NumberFormatter::append(result, bytes[13]);
						result.append(".");
						NumberFormatter::append(result, bytes[14]);
						result.append(".");
						NumberFormatter::append(result, bytes[15]);
						return result;
					}
					else
					{
						std::string result;
						result.reserve(64);
						bool zeroSequence = false;
						int i = 0;
						while (i < 8)
						{
							if (!zeroSequence && words[i] == 0)
							{
								int zi = i;
								while (zi < 8 && words[zi] == 0) ++zi;
								if (zi > i + 1)
								{
									i = zi;
									result.append(":");
									zeroSequence = true;
								}
							}
							if (i > 0) result.append(":");
							if (i < 8) NumberFormatter::appendHex(result, ntohs(words[i++]));
						}
						if (_scope > 0)
						{
							result.append("%");

							NumberFormatter::append(result, _scope);
						}
						return result;
					}
				}

				nequeo_socklen_t length() const
				{
					return sizeof(_addr);
				}

				const void* addr() const
				{
					return &_addr;
				}

				Nequeo::Net::Sockets::AddressFamily addressFamily() const
				{
					return Nequeo::Net::Sockets::IPv6;
				}

				int af() const
				{
					return AF_INET6;
				}

				UInt32 scope() const
				{
					return _scope;
				}

				bool isWildcard() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return words[0] == 0 && words[1] == 0 && words[2] == 0 && words[3] == 0 &&
						words[4] == 0 && words[5] == 0 && words[6] == 0 && words[7] == 0;
				}

				bool isBroadcast() const
				{
					return false;
				}

				bool isLoopback() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return words[0] == 0 && words[1] == 0 && words[2] == 0 && words[3] == 0 &&
						words[4] == 0 && words[5] == 0 && words[6] == 0 && ntohs(words[7]) == 0x0001;
				}

				bool isMulticast() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return (ntohs(words[0]) & 0xFFE0) == 0xFF00;
				}

				bool isLinkLocal() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return (ntohs(words[0]) & 0xFFE0) == 0xFE80;
				}

				bool isSiteLocal() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return (ntohs(words[0]) & 0xFFE0) == 0xFEC0;
				}

				bool isIPv4Mapped() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return words[0] == 0 && words[1] == 0 && words[2] == 0 && words[3] == 0 && words[4] == 0 && ntohs(words[5]) == 0xFFFF;
				}

				bool isIPv4Compatible() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return words[0] == 0 && words[1] == 0 && words[2] == 0 && words[3] == 0 && words[4] == 0 && words[5] == 0;
				}

				bool isWellKnownMC() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return (ntohs(words[0]) & 0xFFF0) == 0xFF00;
				}

				bool isNodeLocalMC() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return (ntohs(words[0]) & 0xFFEF) == 0xFF01;
				}

				bool isLinkLocalMC() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return (ntohs(words[0]) & 0xFFEF) == 0xFF02;
				}

				bool isSiteLocalMC() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return (ntohs(words[0]) & 0xFFEF) == 0xFF05;
				}

				bool isOrgLocalMC() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return (ntohs(words[0]) & 0xFFEF) == 0xFF08;
				}

				bool isGlobalMC() const
				{
					const UInt16* words = reinterpret_cast<const UInt16*>(&_addr);
					return (ntohs(words[0]) & 0xFFEF) == 0xFF0F;
				}

				void mask(const IPAddressProvider* pMask, const IPAddressProvider* pSet)
				{
					// mask() is only supported for IPv4 addresses.
				}

				IPAddressProvider* clone() const
				{
					return new IPv6AddressProvider(&_addr, _scope);
				}

				static IPv6AddressProvider* parse(const std::string& addr)
				{
					if (addr.empty()) return 0;

					struct addrinfo* pAI;
					struct addrinfo hints;
					std::memset(&hints, 0, sizeof(hints));
					hints.ai_flags = AI_NUMERICHOST;
					int rc = getaddrinfo(addr.c_str(), NULL, &hints, &pAI);
					if (rc == 0)
					{
						IPv6AddressProvider* pResult = new IPv6AddressProvider(&reinterpret_cast<struct sockaddr_in6*>(pAI->ai_addr)->sin6_addr, static_cast<int>(reinterpret_cast<struct sockaddr_in6*>(pAI->ai_addr)->sin6_scope_id));
						freeaddrinfo(pAI);
						return pResult;
					}
					else return nullptr;
				}

			private:
				struct in6_addr _addr;
				UInt32    _scope;
			};
		}
	}
}
#endif