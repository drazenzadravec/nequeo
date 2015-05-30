/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          IPAddressProvider.h
*  Purpose :       IPAddressProvider class.
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

#ifndef _IPADDRESSPROVIDER_H
#define _IPADDRESSPROVIDER_H

#include "GlobalSocket.h"
#include "AddressFamily.h"
#include "Base\Types.h"
#include "Base\RefCountedObject.h"

using Nequeo::RefCountedObject;

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			// IPAddressProvider abstract class.
			class IPAddressProvider : public RefCountedObject
			{
			public:
				virtual std::string toString() const = 0;
				virtual nequeo_socklen_t length() const = 0;
				virtual const void* addr() const = 0;
				virtual Nequeo::Net::Sockets::AddressFamily addressFamily() const = 0;
				virtual int af() const = 0;
				virtual Nequeo::UInt32 scope() const = 0;
				virtual bool isWildcard() const = 0;
				virtual bool isBroadcast() const = 0;
				virtual bool isLoopback() const = 0;
				virtual bool isMulticast() const = 0;
				virtual bool isLinkLocal() const = 0;
				virtual bool isSiteLocal() const = 0;
				virtual bool isIPv4Mapped() const = 0;
				virtual bool isIPv4Compatible() const = 0;
				virtual bool isWellKnownMC() const = 0;
				virtual bool isNodeLocalMC() const = 0;
				virtual bool isLinkLocalMC() const = 0;
				virtual bool isSiteLocalMC() const = 0;
				virtual bool isOrgLocalMC() const = 0;
				virtual bool isGlobalMC() const = 0;
				virtual void mask(const IPAddressProvider* pMask, const IPAddressProvider* pSet) = 0;
				virtual IPAddressProvider* clone() const = 0;

			protected:
				// IPAddressProvider class.
				IPAddressProvider();

				// Destroys the Socket and releases resources.
				virtual ~IPAddressProvider();

			private:
				bool _disposed;

				IPAddressProvider(const IPAddressProvider&);
				IPAddressProvider& operator = (const IPAddressProvider&);
			};
		}
	}
}
#endif