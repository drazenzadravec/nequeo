/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TCPServerConnectionFactory.h
*  Purpose :       TCPServerConnectionFactory class.
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

#ifndef _TCPSERVERCONNECTIONFACTORY_H
#define _TCPSERVERCONNECTIONFACTORY_H

#include "GlobalSocket.h"
#include "StreamSocket.h"
#include "TCPServerConnection.h"
#include "Base\SharedPtr.h"

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			/// A factory for TCPServerConnection objects.
			///
			/// The TCPServer class uses a TCPServerConnectionFactory
			/// to create a connection object for each new connection
			/// it accepts.
			///
			/// Subclasses must override the createConnection()
			/// method.
			///
			/// The TCPServerConnectionFactoryImpl template class
			/// can be used to automatically instantiate a
			/// TCPServerConnectionFactory for a given subclass
			/// of TCPServerConnection.
			class TCPServerConnectionFactory
			{
			public:
				typedef Nequeo::SharedPtr<TCPServerConnectionFactory> Ptr;

				virtual ~TCPServerConnectionFactory();
				/// Destroys the TCPServerConnectionFactory.

				virtual TCPServerConnection* createConnection(const Nequeo::Net::Sockets::StreamSocket& socket) = 0;
				/// Creates an instance of a subclass of TCPServerConnection,
				/// using the given StreamSocket.

			protected:
				TCPServerConnectionFactory();
				/// Creates the TCPServerConnectionFactory.

			private:
				TCPServerConnectionFactory(const TCPServerConnectionFactory&);
				TCPServerConnectionFactory& operator = (const TCPServerConnectionFactory&);
			};


			template <class S>
			class TCPServerConnectionFactoryImpl : public TCPServerConnectionFactory
				/// This template provides a basic implementation of
				/// TCPServerConnectionFactory.
			{
			public:
				TCPServerConnectionFactoryImpl()
				{
				}

				~TCPServerConnectionFactoryImpl()
				{
				}

				TCPServerConnection* createConnection(const StreamSocket& socket)
				{
					return new S(socket);
				}
			};

		}
	}
}
#endif
