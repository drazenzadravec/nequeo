/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NetworkInterface.cpp
*  Purpose :       NetworkInterface enum.
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

#include "NetworkInterface.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

using Nequeo::Threading::FastMutex;

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			FastMutex NetworkInterface::_mutex;

			NetworkInterface::NetworkInterface() :
				_pImpl(new NetworkInterfaceProvider), _disposed(false)
			{
			}


			NetworkInterface::NetworkInterface(const NetworkInterface& interfc) :
				_pImpl(interfc._pImpl), _disposed(false)
			{
				_pImpl->duplicate();
			}


			NetworkInterface::NetworkInterface(const std::string& name, const std::string& displayName, const IPAddress& address, int index) :
				_pImpl(new NetworkInterfaceProvider(name, displayName, address, index)), _disposed(false)
			{
			}


			NetworkInterface::NetworkInterface(const std::string& name, const std::string& displayName, const IPAddress& address, const IPAddress& subnetMask, const IPAddress& broadcastAddress, int index) :
				_pImpl(new NetworkInterfaceProvider(name, displayName, address, subnetMask, broadcastAddress, index)), _disposed(false)
			{
			}


			NetworkInterface::NetworkInterface(const std::string& name, const IPAddress& address, int index) :
				_pImpl(new NetworkInterfaceProvider(name, name, address, index)), _disposed(false)
			{
			}


			NetworkInterface::NetworkInterface(const std::string& name, const IPAddress& address, const IPAddress& subnetMask, const IPAddress& broadcastAddress, int index) :
				_pImpl(new NetworkInterfaceProvider(name, name, address, subnetMask, broadcastAddress, index)), _disposed(false)
			{
			}


			NetworkInterface::~NetworkInterface()
			{
				// If not disposed.
				if (!_disposed)
				{
					_disposed = true;
					_pImpl->release();
				}
			}


			NetworkInterface& NetworkInterface::operator = (const NetworkInterface& interfc)
			{
				NetworkInterface tmp(interfc);
				swap(tmp);
				return *this;
			}


			void NetworkInterface::swap(NetworkInterface& other)
			{
				using std::swap;
				swap(_pImpl, other._pImpl);
			}


			int NetworkInterface::index() const
			{
				return _pImpl->index();
			}


			const std::string& NetworkInterface::name() const
			{
				return _pImpl->name();
			}


			const std::string& NetworkInterface::displayName() const
			{
				return _pImpl->displayName();
			}


			const IPAddress& NetworkInterface::address() const
			{
				return _pImpl->address();
			}


			const IPAddress& NetworkInterface::subnetMask() const
			{
				return _pImpl->subnetMask();
			}


			const IPAddress& NetworkInterface::broadcastAddress() const
			{
				return _pImpl->broadcastAddress();
			}


			bool NetworkInterface::supportsIPv4() const
			{
				return _pImpl->address().addressFamily() == Nequeo::Net::Sockets::AddressFamily::IPv4;
			}


			bool NetworkInterface::supportsIPv6() const
			{
				return _pImpl->address().addressFamily() == Nequeo::Net::Sockets::AddressFamily::IPv6;
			}


			NetworkInterface NetworkInterface::forName(const std::string& name, bool requireIPv6)
			{
				NetworkInterfaceList ifs = list();
				for (NetworkInterfaceList::const_iterator it = ifs.begin(); it != ifs.end(); ++it)
				{
					if (it->name() == name && ((requireIPv6 && it->supportsIPv6()) || !requireIPv6))
						return *it;
				}
				throw Nequeo::Exceptions::Net::InterfaceNotFoundException(name);
			}


			NetworkInterface NetworkInterface::forName(const std::string& name, IPVersion ipVersion)
			{
				NetworkInterfaceList ifs = list();
				for (NetworkInterfaceList::const_iterator it = ifs.begin(); it != ifs.end(); ++it)
				{
					if (it->name() == name)
					{
						if (ipVersion == IPv4_ONLY && it->supportsIPv4())
							return *it;
						else if (ipVersion == IPv6_ONLY && it->supportsIPv6())
							return *it;
						else if (ipVersion == IPv4_OR_IPv6)
							return *it;
					}
				}
				throw Nequeo::Exceptions::Net::InterfaceNotFoundException(name);
			}


			NetworkInterface NetworkInterface::forAddress(const IPAddress& addr)
			{
				NetworkInterfaceList ifs = list();
				for (NetworkInterfaceList::const_iterator it = ifs.begin(); it != ifs.end(); ++it)
				{
					if (it->address() == addr)
						return *it;
				}
				throw Nequeo::Exceptions::Net::InterfaceNotFoundException(addr.toString());
			}


			NetworkInterface NetworkInterface::forIndex(int i)
			{
				if (i != 0)
				{
					NetworkInterfaceList ifs = list();
					for (NetworkInterfaceList::const_iterator it = ifs.begin(); it != ifs.end(); ++it)
					{
						if (it->index() == i)
							return *it;
					}
					throw Nequeo::Exceptions::Net::InterfaceNotFoundException("#" + NumberFormatter::format(i));
				}
				else return NetworkInterface();
			}
		}
	}
}