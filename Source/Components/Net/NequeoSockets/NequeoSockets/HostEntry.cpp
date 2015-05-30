/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          HostEntry.cpp
*  Purpose :       HostEntry class.
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

#include"HostEntry.h"

Nequeo::Net::Sockets::HostEntry::HostEntry()
{
}


Nequeo::Net::Sockets::HostEntry::HostEntry(struct hostent* entry)
{
	if (entry != nullptr)
	{
		_name = entry->h_name;
		char** alias = entry->h_aliases;
		if (alias)
		{
			while (*alias)
			{
				_aliases.push_back(std::string(*alias));
				++alias;
			}
		}
		char** address = entry->h_addr_list;
		if (address)
		{
			while (*address)
			{
				_addresses.push_back(IPAddress(*address, entry->h_length));
				++address;
			}
		}
	}
}

Nequeo::Net::Sockets::HostEntry::HostEntry(struct addrinfo* ainfo)
{
	if (ainfo != nullptr)
	{
		for (struct addrinfo* ai = ainfo; ai; ai = ai->ai_next)
		{
			if (ai->ai_canonname)
			{
				_name.assign(ai->ai_canonname);
			}
			if (ai->ai_addrlen && ai->ai_addr)
			{
				switch (ai->ai_addr->sa_family)
				{
				case AF_INET:
					_addresses.push_back(IPAddress(&reinterpret_cast<struct sockaddr_in*>(ai->ai_addr)->sin_addr, sizeof(in_addr)));
					break;
				case AF_INET6:
					_addresses.push_back(IPAddress(&reinterpret_cast<struct sockaddr_in6*>(ai->ai_addr)->sin6_addr, sizeof(in6_addr), reinterpret_cast<struct sockaddr_in6*>(ai->ai_addr)->sin6_scope_id));
					break;
				}
			}
		}
	}
}

///
Nequeo::Net::Sockets::HostEntry::HostEntry(const Nequeo::Net::Sockets::HostEntry& entry) :
	_name(entry._name),
	_aliases(entry._aliases),
	_addresses(entry._addresses)
{
}


Nequeo::Net::Sockets::HostEntry& Nequeo::Net::Sockets::HostEntry::operator = (const Nequeo::Net::Sockets::HostEntry& entry)
{
	if (&entry != this)
	{
		_name = entry._name;
		_aliases = entry._aliases;
		_addresses = entry._addresses;
	}
	return *this;
}


void Nequeo::Net::Sockets::HostEntry::swap(Nequeo::Net::Sockets::HostEntry& hostEntry)
{
	std::swap(_name, hostEntry._name);
	std::swap(_aliases, hostEntry._aliases);
	std::swap(_addresses, hostEntry._addresses);
}


Nequeo::Net::Sockets::HostEntry::~HostEntry()
{
}

///
inline const std::string& Nequeo::Net::Sockets::HostEntry::name() const
{
	return _name;
}


inline const Nequeo::Net::Sockets::HostEntry::AliasList& Nequeo::Net::Sockets::HostEntry::aliases() const
{
	return _aliases;
}


inline const Nequeo::Net::Sockets::HostEntry::AddressList& Nequeo::Net::Sockets::HostEntry::addresses() const
{
	return _addresses;
}


inline void swap(Nequeo::Net::Sockets::HostEntry& h1, Nequeo::Net::Sockets::HostEntry& h2)
{
	h1.swap(h2);
}