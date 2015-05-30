/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TCPServer.h
*  Purpose :       TCPServer class.
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

#ifndef _TCPSERVER_H
#define _TCPSERVER_H

#include "GlobalSocket.h"
#include "ServerSocket.h"
#include "TCPServerConnectionFactory.h"
#include "TCPServerParams.h"
#include "Threading\Runnable.h"
#include "Threading\Thread.h"
#include "Threading\ThreadPool.h"
#include "Base\Types.h"

using Nequeo::UInt8;
using Nequeo::UInt16;
using Nequeo::UInt32;

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			class TCPServerDispatcher;

			/// This class implements a multithreaded TCP server.
			///
			/// The server uses a ServerSocket to listen for incoming
			/// connections. The ServerSocket must have been bound to
			/// an address before it is passed to the TCPServer constructor.
			/// Additionally, the ServerSocket must be put into listening
			/// state before the TCPServer is started by calling the start()
			/// method.
			///
			/// The server uses a thread pool to assign threads to incoming
			/// connections. Before incoming connections are assigned to
			/// a connection thread, they are put into a queue.
			/// Connection threads fetch new connections from the queue as soon
			/// as they become free. Thus, a connection thread may serve more
			/// than one connection.
			///
			/// As soon as a connection thread fetches the next connection from
			/// the queue, it creates a TCPServerConnection object for it
			/// (using the TCPServerConnectionFactory passed to the constructor)
			/// and calls the TCPServerConnection's start() method. When the
			/// start() method returns, the connection object is deleted.
			///
			/// The number of connection threads is adjusted dynamically, depending
			/// on the number of connections waiting to be served.
			///
			/// It is possible to specify a maximum number of queued connections.
			/// This prevents the connection queue from overflowing in the 
			/// case of an extreme server load. In such a case, connections that
			/// cannot be queued are silently and immediately closed.
			///
			/// TCPServer uses a separate thread to accept incoming connections.
			/// Thus, the call to start() returns immediately, and the server
			/// continues to run in the background.
			///
			/// To stop the server from accepting new connections, call stop().
			///
			/// After calling stop(), no new connections will be accepted and
			/// all queued connections will be discarded.
			/// Already served connections, however, will continue being served.
			class TCPServer : public Nequeo::Threading::Runnable
			{
			public:
				TCPServer(TCPServerConnectionFactory::Ptr pFactory, const ServerSocket& socket, TCPServerParams::Ptr pParams = 0);
				/// Creates the TCPServer, using the given ServerSocket.
				///
				/// If no TCPServerParams object is given, the server's TCPServerDispatcher
				/// creates its own one.
				///
				/// New threads are taken from the default thread pool.

				TCPServer(TCPServerConnectionFactory::Ptr pFactory, Nequeo::Threading::ThreadPool& threadPool, const ServerSocket& socket, TCPServerParams::Ptr pParams = 0);
				/// Creates the TCPServer, using the given ServerSocket.
				///
				/// If no TCPServerParams object is given, the server's TCPServerDispatcher
				/// creates its own one.
				///
				/// New threads are taken from the given thread pool.

				virtual ~TCPServer();
				/// Destroys the TCPServer and its TCPServerConnectionFactory.

				const TCPServerParams& params() const;
				/// Returns a const reference to the TCPServerParam object
				/// used by the server's TCPServerDispatcher.	

				void start();
				/// Starts the server. A new thread will be
				/// created that waits for and accepts incoming
				/// connections.
				///
				/// Before start() is called, the ServerSocket passed to
				/// TCPServer must have been bound and put into listening state.

				void stop();
				/// Stops the server.
				///
				/// No new connections will be accepted.
				/// Already handled connections will continue to be served.
				///
				/// Once the server has been stopped, it cannot be restarted.

				int currentThreads() const;
				/// Returns the number of currently used connection threads.

				int totalConnections() const;
				/// Returns the total number of handled connections.

				int currentConnections() const;
				/// Returns the number of currently handled connections.

				int maxConcurrentConnections() const;
				/// Returns the maximum number of concurrently handled connections.	

				int queuedConnections() const;
				/// Returns the number of queued connections.

				int refusedConnections() const;
				/// Returns the number of refused connections.

				UInt16 port() const;
				/// Returns the port the server socket listens on.

			protected:
				void run();
				/// Runs the server. The server will run until
				/// the stop() method is called, or the server
				/// object is destroyed, which implicitly calls
				/// the stop() method.

				static std::string threadName(const ServerSocket& socket);
				/// Returns a thread name for the server thread.

			private:
				TCPServer();
				TCPServer(const TCPServer&);
				TCPServer& operator = (const TCPServer&);

				ServerSocket         _socket;
				TCPServerDispatcher* _pDispatcher;
				Nequeo::Threading::Thread         _thread;
				bool                 _stopped;
			};


			inline UInt16 TCPServer::port() const
			{
				return _socket.address().port();
			}
		}
	}
}
#endif
