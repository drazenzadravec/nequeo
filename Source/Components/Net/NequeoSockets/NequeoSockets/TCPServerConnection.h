/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TCPServerConnection.h
*  Purpose :       TCPServerConnection class.
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

#ifndef _TCPSERVERCONNECTION_H
#define _TCPSERVERCONNECTION_H

#include "GlobalSocket.h"
#include "StreamSocket.h"
#include "TCPServerConnection.h"
#include "Threading\Runnable.h"
#include "Base\SharedPtr.h"

using Nequeo::Net::Sockets::StreamSocket;

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			/// The abstract base class for TCP server connections
			/// created by TCPServer.
			///
			/// Derived classes must override the run() method
			/// (inherited from Runnable). Furthermore, a
			/// TCPServerConnectionFactory must be provided for the subclass.
			///
			/// The run() method must perform the complete handling
			/// of the client connection. As soon as the run() method
			/// returns, the server connection object is destroyed and
			/// the connection is automatically closed.
			///
			/// A new TCPServerConnection object will be created for
			/// each new client connection that is accepted by
			/// TCPServer.
			class TCPServerConnection : public Nequeo::Threading::Runnable
			{
			public:
				TCPServerConnection(const StreamSocket& socket);
				/// Creates the TCPServerConnection using the given
				/// stream socket.

				virtual ~TCPServerConnection();
				/// Destroys the TCPServerConnection.

			protected:
				StreamSocket& socket();
				/// Returns a reference to the underlying socket.

				void start();
				/// Calls run() and catches any exceptions that
				/// might be thrown by run().

			private:
				TCPServerConnection();
				TCPServerConnection(const TCPServerConnection&);
				TCPServerConnection& operator = (const TCPServerConnection&);

				StreamSocket _socket;

				friend class TCPServerDispatcher;
			};


			//
			// inlines
			//
			inline StreamSocket& TCPServerConnection::socket()
			{
				return _socket;
			}
		}
	}
}
#endif
