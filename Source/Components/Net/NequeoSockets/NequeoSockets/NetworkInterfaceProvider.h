/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NetworkInterfaceProvider.h
*  Purpose :       NetworkInterfaceProvider enum.
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

#ifndef _NETWORKINTERFACEPROVIDER_H
#define _NETWORKINTERFACEPROVIDER_H

#include "GlobalSocket.h"
#include "IPAddress.h"
#include "Base\Types.h"
#include "Base\RefCountedObject.h"

using Nequeo::RefCountedObject;

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			class NetworkInterfaceProvider : public RefCountedObject
			{
			public:
				NetworkInterfaceProvider();
				NetworkInterfaceProvider(const std::string& name, const std::string& displayName, const IPAddress& address, int index = -1);
				NetworkInterfaceProvider(const std::string& name, const std::string& displayName, const IPAddress& address, const IPAddress& subnetMask, const IPAddress& broadcastAddress, int index = -1);

				int index() const;
				const std::string& name() const;
				const std::string& displayName() const;
				const IPAddress& address() const;
				const IPAddress& subnetMask() const;
				const IPAddress& broadcastAddress() const;

			protected:
				~NetworkInterfaceProvider();

			private:
				std::string _name;
				std::string _displayName;
				IPAddress   _address;
				IPAddress   _subnetMask;
				IPAddress   _broadcastAddress;
				int         _index;
			};
		}
	}
}
#endif