/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          DNS.h
*  Purpose :       DNS class.
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

#ifndef _DNS_H
#define _DNS_H

#include "GlobalSocket.h"
#include "IPAddress.h"
#include "HostEntry.h"
#include "SocketAddress.h"

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			/// This class provides an interface to the
			/// domain name service.
			///
			/// An internal DNS cache is used to speed up name lookups.
			class DNS
			{
			public:
				static HostEntry hostByName(const std::string& hostname);
				/// Returns a HostEntry object containing the DNS information
				/// for the host with the given name.
				///
				/// Throws a HostNotFoundException if a host with the given
				/// name cannot be found.
				///
				/// Throws a NoAddressFoundException if no address can be
				/// found for the hostname.
				///
				/// Throws a DNSException in case of a general DNS error.
				///
				/// Throws an IOException in case of any other error.

				static HostEntry hostByAddress(const IPAddress& address);
				/// Returns a HostEntry object containing the DNS information
				/// for the host with the given IP address.
				///
				/// Throws a HostNotFoundException if a host with the given
				/// name cannot be found.
				///
				/// Throws a DNSException in case of a general DNS error.
				///
				/// Throws an IOException in case of any other error.

				static HostEntry resolve(const std::string& address);
				/// Returns a HostEntry object containing the DNS information
				/// for the host with the given IP address or host name.
				///
				/// Throws a HostNotFoundException if a host with the given
				/// name cannot be found.
				///
				/// Throws a NoAddressFoundException if no address can be
				/// found for the hostname.
				///
				/// Throws a DNSException in case of a general DNS error.
				///
				/// Throws an IOException in case of any other error.

				static IPAddress resolveOne(const std::string& address);
				/// Convenience method that calls resolve(address) and returns 
				/// the first address from the HostInfo.

				static HostEntry thisHost();
				/// Returns a HostEntry object containing the DNS information
				/// for this host.
				///
				/// Throws a HostNotFoundException if DNS information 
				/// for this host cannot be found.
				///
				/// Throws a NoAddressFoundException if no address can be
				/// found for this host.
				///
				/// Throws a DNSException in case of a general DNS error.
				///
				/// Throws an IOException in case of any other error.

				static void reload();
				/// Reloads the resolver configuration.
				///
				/// This method will call res_init() if the Net library
				/// has been compiled with -DPOCO_HAVE_LIBRESOLV. Otherwise
				/// it will do nothing.

				//@ deprecated
				static void flushCache();
				/// Flushes the internal DNS cache.
				///
				/// As of 1.4.2, the DNS cache is no longer used
				/// and this method does not do anything.

				static std::string hostName();
				/// Returns the host name of this host.

			protected:
				static int lastError();
				/// Returns the code of the last error.

				static void error(int code, const std::string& arg);
				/// Throws an exception according to the error code.

				static void aierror(int code, const std::string& arg);
				/// Throws an exception according to the getaddrinfo() error code.
			};
		}
	}
}
#endif