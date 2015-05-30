/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          HostEntry.h
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

#pragma once

#ifndef _HOSTENTRY_H
#define _HOSTENTRY_H

#include "GlobalSocket.h"
#include "IPAddress.h"

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			/// This class stores information about a host
			/// such as host name, alias names and a list
			/// of IP addresses.
			class HostEntry
			{
			public:
				typedef std::vector<std::string> AliasList;
				typedef std::vector<IPAddress>   AddressList;

				HostEntry();
				/// Creates an empty HostEntry.

				HostEntry(struct hostent* entry);
				/// Creates the HostEntry from the data in a hostent structure.

				HostEntry(struct addrinfo* info);
				/// Creates the HostEntry from the data in an addrinfo structure.

				HostEntry(const HostEntry& entry);
				/// Creates the HostEntry by copying another one.

				HostEntry& operator = (const HostEntry& entry);
				/// Assigns another HostEntry.

				void swap(HostEntry& hostEntry);
				/// Swaps the HostEntry with another one.	

				~HostEntry();
				/// Destroys the HostEntry.

				const std::string& name() const;
				/// Returns the canonical host name.

				const AliasList& aliases() const;
				/// Returns a vector containing alias names for
				/// the host name.

				const AddressList& addresses() const;
				/// Returns a vector containing the IPAddresses
				/// for the host.

			private:
				std::string _name;
				AliasList   _aliases;
				AddressList _addresses;
			};
		}
	}
}
#endif