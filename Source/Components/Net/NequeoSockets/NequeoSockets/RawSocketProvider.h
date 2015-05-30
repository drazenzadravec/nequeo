/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RawSocketProvider.h
*  Purpose :       RawSocketProvider class.
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

#ifndef _RAWSOCKETPROVIDER_H
#define _RAWSOCKETPROVIDER_H

#include "GlobalSocket.h"
#include "SocketProvider.h"
#include "AddressFamily.h"

using Nequeo::Net::Sockets::SocketProvider;

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			/// This class implements a raw socket.
			class RawSocketProvider : public SocketProvider
			{
			public:
				/// Creates an unconnected IPv4 raw socket with IPPROTO_RAW.
				RawSocketProvider();
				
				/// Creates an unconnected raw socket.
				///
				/// The socket will be created for the
				/// given address family.
				RawSocketProvider(Nequeo::Net::Sockets::AddressFamily family, int proto = IPPROTO_RAW);
				
				/// Creates a RawSocketProvider using the given native socket.
				RawSocketProvider(nequeo_socket_t sockfd);

			protected:
				void init(int af);
				void init2(int af, int proto);

				~RawSocketProvider();
			};
		}
	}
}
#endif