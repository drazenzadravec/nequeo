/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          StreamSocketProvider.h
*  Purpose :       StreamSocketProvider class.
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

#ifndef _STREAMSOCKETPROVIDER_H
#define _STREAMSOCKETPROVIDER_H

#include "GlobalSocket.h"
#include "SocketProvider.h"

namespace Nequeo {
	namespace Net {
		namespace Sockets
		{
			/// This class implements a TCP socket.
			class StreamSocketProvider : public SocketProvider
			{
			public:
				StreamSocketProvider();
				/// Creates a StreamSocketImpl.

				explicit StreamSocketProvider(Nequeo::Net::Sockets::AddressFamily addressFamily);
				/// Creates a SocketImpl, with the underlying
				/// socket initialized for the given address family.

				StreamSocketProvider(nequeo_socket_t sockfd);
				/// Creates a StreamSocketImpl using the given native socket.

				virtual int sendBytes(const void* buffer, int length, int flags = 0);
				/// Ensures that all data in buffer is sent if the socket
				/// is blocking. In case of a non-blocking socket, sends as
				/// many bytes as possible.
				///
				/// Returns the number of bytes sent. The return value may also be
				/// negative to denote some special condition.

			protected:
				virtual ~StreamSocketProvider();
			};
		}
	}
}
#endif