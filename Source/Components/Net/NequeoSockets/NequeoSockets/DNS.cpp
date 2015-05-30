/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          DNS.cpp
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

#include "stdafx.h"

#include "DNS.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			/// Network initializer.
			class NetworkInitializer
			{
			public:
				NetworkInitializer()
				{
					InitializeNetwork();
				}

				~NetworkInitializer()
				{
					UninitializeNetwork();
				}
			};
		}
	}
}

Nequeo::Net::Sockets::HostEntry Nequeo::Net::Sockets::DNS::hostByName(const std::string& hostname)
{
	NetworkInitializer networkInitializer;

	struct addrinfo* pAI;
	struct addrinfo hints;
	std::memset(&hints, 0, sizeof(hints));
	hints.ai_flags = AI_CANONNAME | AI_ADDRCONFIG;
	int rc = getaddrinfo(hostname.c_str(), NULL, &hints, &pAI);
	if (rc == 0)
	{
		HostEntry result(pAI);
		freeaddrinfo(pAI);
		return result;
	}
	else
	{
		aierror(rc, hostname);
	}

	error(lastError(), hostname); // will throw an appropriate exception
	throw Nequeo::Exceptions::Net::NetException(); // to silence compiler
}


Nequeo::Net::Sockets::HostEntry Nequeo::Net::Sockets::DNS::hostByAddress(const IPAddress& address)
{
	NetworkInitializer networkInitializer;

	SocketAddress sa(address, 0);
	static char fqname[1024];
	int rc = getnameinfo(sa.addr(), sa.length(), fqname, sizeof(fqname), NULL, 0, NI_NAMEREQD);
	if (rc == 0)
	{
		struct addrinfo* pAI;
		struct addrinfo hints;
		std::memset(&hints, 0, sizeof(hints));
		hints.ai_flags = AI_CANONNAME | AI_ADDRCONFIG;
		rc = getaddrinfo(fqname, NULL, &hints, &pAI);
		if (rc == 0)
		{
			HostEntry result(pAI);
			freeaddrinfo(pAI);
			return result;
		}
		else
		{
			aierror(rc, address.toString());
		}
	}
	else
	{
		aierror(rc, address.toString());
	}

	int err = lastError();
	error(err, address.toString());      // will throw an appropriate exception
	throw Nequeo::Exceptions::Net::NetException(); // to silence compiler
}


Nequeo::Net::Sockets::HostEntry Nequeo::Net::Sockets::DNS::resolve(const std::string& address)
{
	NetworkInitializer networkInitializer;

	IPAddress ip;
	if (IPAddress::tryParse(address, ip))
		return hostByAddress(ip);
	else
		return hostByName(address);
}


Nequeo::Net::Sockets::IPAddress Nequeo::Net::Sockets::DNS::resolveOne(const std::string& address)
{
	NetworkInitializer networkInitializer;

	const HostEntry& entry = resolve(address);
	if (!entry.addresses().empty())
		return entry.addresses()[0];
	else
		throw Nequeo::Exceptions::Net::NoAddressFoundException(address);
}


Nequeo::Net::Sockets::HostEntry Nequeo::Net::Sockets::DNS::thisHost()
{
	return hostByName(hostName());
}


void Nequeo::Net::Sockets::DNS::reload()
{
}


void Nequeo::Net::Sockets::DNS::flushCache()
{
}


std::string Nequeo::Net::Sockets::DNS::hostName()
{
	NetworkInitializer networkInitializer;

	char buffer[256];
	int rc = gethostname(buffer, sizeof(buffer));
	if (rc == 0)
		return std::string(buffer);
	else
		throw Nequeo::Exceptions::Net::NetException("Cannot get host name");
}


int Nequeo::Net::Sockets::DNS::lastError()
{
	return GetLastError();
}


void Nequeo::Net::Sockets::DNS::error(int code, const std::string& arg)
{
	switch (code)
	{
	case NEQUEO_ESYSNOTREADY:
		throw Nequeo::Exceptions::Net::NetException("Net subsystem not ready");
	case NEQUEO_ENOTINIT:
		throw Nequeo::Exceptions::Net::NetException("Net subsystem not initialized");
	case NEQUEO_HOST_NOT_FOUND:
		throw Nequeo::Exceptions::Net::HostNotFoundException(arg);
	case NEQUEO_TRY_AGAIN:
		throw Nequeo::Exceptions::Net::DNSException("Temporary DNS error while resolving", arg);
	case NEQUEO_NO_RECOVERY:
		throw Nequeo::Exceptions::Net::DNSException("Non recoverable DNS error while resolving", arg);
	case NEQUEO_NO_DATA:
		throw Nequeo::Exceptions::Net::NoAddressFoundException(arg);
	default:
		throw Nequeo::Exceptions::IOException(NumberFormatter::format(code));
	}
}


void Nequeo::Net::Sockets::DNS::aierror(int code, const std::string& arg)
{
	switch (code)
	{
	case EAI_AGAIN:
		throw Nequeo::Exceptions::Net::DNSException("Temporary DNS error while resolving", arg);
	case EAI_FAIL:
		throw Nequeo::Exceptions::Net::DNSException("Non recoverable DNS error while resolving", arg);
	case EAI_NONAME:
		throw Nequeo::Exceptions::Net::HostNotFoundException(arg);
	case WSANO_DATA: // may happen on XP
		throw Nequeo::Exceptions::Net::HostNotFoundException(arg);
	default:
		throw Nequeo::Exceptions::Net::DNSException("EAI", NumberFormatter::format(code));
	}
}

///
void InitializeNetwork()
{
	WORD    version = MAKEWORD(2, 2);
	WSADATA data;
	if (WSAStartup(version, &data) != 0)
		throw Nequeo::Exceptions::Net::NetException("Failed to initialize network subsystem");
}

///
void UninitializeNetwork()
{
	WSACleanup();
}