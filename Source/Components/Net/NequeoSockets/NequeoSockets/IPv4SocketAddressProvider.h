/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          IPv4SocketAddressProvider.cpp
*  Purpose :       IPv4SocketAddressProvider class.
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

#ifndef _IPV4_SOCKETADDRESSPROVIDER_H
#define _IPV4_SOCKETADDRESSPROVIDER_H

#include "GlobalSocket.h"
#include "Primitive\NumberFormatter.h"
#include "AddressFamily.h"
#include "IPAddress.h"
#include "SocketAddressProvider.h"

using Nequeo::Primitive::NumberFormatter;
using Nequeo::UInt8;
using Nequeo::UInt16;
using Nequeo::UInt32;

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			class IPv4SocketAddressProvider : public SocketAddressProvider
			{
			public:
				IPv4SocketAddressProvider()
				{
					std::memset(&_addr, 0, sizeof(_addr));
					_addr.sin_family = AF_INET;
					nequeo_set_sin_len(&_addr);
				}

				IPv4SocketAddressProvider(const struct sockaddr_in* addr)
				{
					std::memcpy(&_addr, addr, sizeof(_addr));
				}

				IPv4SocketAddressProvider(const void* addr, UInt16 port)
				{
					std::memset(&_addr, 0, sizeof(_addr));
					_addr.sin_family = AF_INET;
					std::memcpy(&_addr.sin_addr, addr, sizeof(_addr.sin_addr));
					_addr.sin_port = port;
				}

				IPAddress host() const
				{
					return IPAddress(&_addr.sin_addr, sizeof(_addr.sin_addr));
				}

				UInt16 port() const
				{
					return _addr.sin_port;
				}

				nequeo_socklen_t length() const
				{
					return sizeof(_addr);
				}

				const struct sockaddr* addr() const
				{
					return reinterpret_cast<const struct sockaddr*>(&_addr);
				}

				int af() const
				{
					return _addr.sin_family;
				}

			private:
				struct sockaddr_in _addr;
			};
		}
	}
}
#endif