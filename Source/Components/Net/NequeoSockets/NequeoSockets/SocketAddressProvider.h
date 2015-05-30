/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          SocketAddressProvider.h
*  Purpose :       SocketAddressProvider class.
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

#ifndef _SOCKETADDRESSPROVIDER_H
#define _SOCKETADDRESSPROVIDER_H

#include "GlobalSocket.h"
#include "AddressFamily.h"
#include "Base\Types.h"
#include "Base\RefCountedObject.h"
#include "IPAddress.h"

using Nequeo::RefCountedObject;

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			// SocketAddressProvider abstract class.
			class SocketAddressProvider : public Nequeo::RefCountedObject
			{
			public:
				virtual IPAddress host() const = 0;
				virtual UInt16 port() const = 0;
				virtual nequeo_socklen_t length() const = 0;
				virtual const struct sockaddr* addr() const = 0;
				virtual int af() const = 0;

			protected:
				// SocketAddressProvider class.
				SocketAddressProvider();

				// Destroys the Socket and releases resources.
				virtual ~SocketAddressProvider();

			private:
				bool _disposed;

				SocketAddressProvider(const SocketAddressProvider&);
				SocketAddressProvider& operator = (const SocketAddressProvider&);
			};
		}
	}
}
#endif