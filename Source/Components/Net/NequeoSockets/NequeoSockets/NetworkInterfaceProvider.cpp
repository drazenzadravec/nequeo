/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NetworkInterfaceProvider.cpp
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

#include "stdafx.h"

#include "NetworkInterfaceProvider.h"

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			NetworkInterfaceProvider::NetworkInterfaceProvider() :
				_index(-1)
			{
			}

			///
			NetworkInterfaceProvider::NetworkInterfaceProvider(const std::string& name, const std::string& displayName, const IPAddress& address, int index) :
				_name(name),
				_displayName(displayName),
				_address(address),
				_index(index)
			{
			}


			NetworkInterfaceProvider::NetworkInterfaceProvider(const std::string& name, const std::string& displayName, const IPAddress& address, const IPAddress& subnetMask, const IPAddress& broadcastAddress, int index) :
				_name(name),
				_displayName(displayName),
				_address(address),
				_subnetMask(subnetMask),
				_broadcastAddress(broadcastAddress),
				_index(index)
			{
			}


			NetworkInterfaceProvider::~NetworkInterfaceProvider()
			{
			}


			inline int NetworkInterfaceProvider::index() const
			{
				return _index;
			}


			inline const std::string& NetworkInterfaceProvider::name() const
			{
				return _name;
			}


			inline const std::string& NetworkInterfaceProvider::displayName() const
			{
				return _displayName;
			}


			inline const IPAddress& NetworkInterfaceProvider::address() const
			{
				return _address;
			}


			inline const IPAddress& NetworkInterfaceProvider::subnetMask() const
			{
				return _subnetMask;
			}


			inline const IPAddress& NetworkInterfaceProvider::broadcastAddress() const
			{
				return _broadcastAddress;
			}
		}
	}
}