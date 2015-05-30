/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          SocketAddress.h
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

#pragma once

#ifndef _SOCKETADDRESS_H
#define _SOCKETADDRESS_H

#include "GlobalSocket.h"
#include "IPAddress.h"
#include "Base\Timespan.h"
#include "Base\Timestamp.h"
#include "SocketAddressProvider.h"
#include "IPv4SocketAddressProvider.h"
#include "IPv6SocketAddressProvider.h"
#include "DNS.h"
#include "HostEntry.h"
#include "Primitive\NumberParser.h"

using Nequeo::UInt8;
using Nequeo::UInt16;
using Nequeo::UInt32;

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			/// This class represents an internet (IP) endpoint/socket
			/// address. The address can belong either to the
			/// IPv4 or the IPv6 address family and consists of a
			/// host address and a port number.
			class SocketAddress
			{
			public:
				SocketAddress();
				/// Creates a wildcard (all zero) IPv4 SocketAddress.

				SocketAddress(const IPAddress& host, UInt16 port);
				/// Creates a SocketAddress from an IP address and a port number.

				SocketAddress(const std::string& host, UInt16 port);
				/// Creates a SocketAddress from an IP address and a port number.
				///
				/// The IP address must either be a domain name, or it must
				/// be in dotted decimal (IPv4) or hex string (IPv6) format.

				SocketAddress(const std::string& host, const std::string& port);
				/// Creates a SocketAddress from an IP address and a 
				/// service name or port number.
				///
				/// The IP address must either be a domain name, or it must
				/// be in dotted decimal (IPv4) or hex string (IPv6) format.
				///
				/// The given port must either be a decimal port number, or 
				/// a service name.

				explicit SocketAddress(const std::string& hostAndPort);
				/// Creates a SocketAddress from an IP address or host name and a
				/// port number/service name. Host name/address and port number must
				/// be separated by a colon. In case of an IPv6 address,
				/// the address part must be enclosed in brackets.
				///
				/// Examples:
				///     192.168.1.10:80
				///     [::ffff:192.168.1.120]:2040
				///     www.appinf.com:8080

				SocketAddress(const SocketAddress& addr);
				/// Creates a SocketAddress by copying another one.

				SocketAddress(const struct sockaddr* addr, nequeo_socklen_t length);
				/// Creates a SocketAddress from a native socket address.

				~SocketAddress();
				/// Destroys the SocketAddress.

				SocketAddress& operator = (const SocketAddress& addr);
				/// Assigns another SocketAddress.

				void swap(SocketAddress& addr);
				/// Swaps the SocketAddress with another one.

				IPAddress host() const;
				/// Returns the host IP address.

				UInt16 port() const;
				/// Returns the port number.

				nequeo_socklen_t length() const;
				/// Returns the length of the internal native socket address.

				const struct sockaddr* addr() const;
				/// Returns a pointer to the internal native socket address.

				int af() const;
				/// Returns the address family (AF_INET or AF_INET6) of the address.

				std::string toString() const;
				/// Returns a string representation of the address.

				Nequeo::Net::Sockets::AddressFamily addressFamily() const;
				/// Returns the address family of the host's address.

				bool operator < (const SocketAddress& addr) const;
				bool operator == (const SocketAddress& addr) const;
				bool operator != (const SocketAddress& addr) const;

			protected:
				void init(const IPAddress& host, UInt16 port);
				void init(const std::string& host, UInt16 port);
				UInt16 resolveService(const std::string& service);

			private:
				bool _disposed;
				SocketAddressProvider* _pImpl;
			};
		}
	}
}
#endif