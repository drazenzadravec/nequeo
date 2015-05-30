/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TCPServerDispatcher.h
*  Purpose :       TCPServerDispatcher class.
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

#ifndef _TCPSERVERDISPATCHER_H
#define _TCPSERVERDISPATCHER_H

#include "GlobalSocket.h"
#include "StreamSocket.h"
#include "TCPServerConnectionFactory.h"
#include "TCPServerParams.h"
#include "Threading\Runnable.h"
#include "Threading\NotificationQueue.h"
#include "Threading\ThreadPool.h"
#include "Threading\Mutex.h"

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			/// A helper class for TCPServer that dispatches
			/// connections to server connection threads.
			class TCPServerDispatcher : public Nequeo::Threading::Runnable
			{
			public:
				TCPServerDispatcher(TCPServerConnectionFactory::Ptr pFactory, Nequeo::Threading::ThreadPool& threadPool, TCPServerParams::Ptr pParams);
				/// Creates the TCPServerDispatcher.

				void duplicate();
				/// Increments the object's reference count.

				void release();
				/// Decrements the object's reference count
				/// and deletes the object if the count
				/// reaches zero.	

				void run();
				/// Runs the dispatcher.

				void enqueue(const Nequeo::Net::Sockets::StreamSocket& socket);
				/// Queues the given socket connection.

				void stop();
				/// Stops the dispatcher.

				int currentThreads() const;
				/// Returns the number of currently used threads.

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

				const TCPServerParams& params() const;
				/// Returns a const reference to the TCPServerParam object.

			protected:
				~TCPServerDispatcher();
				/// Destroys the TCPServerDispatcher.

				void beginConnection();
				/// Updates the performance counters.

				void endConnection();
				/// Updates the performance counters.

			private:
				TCPServerDispatcher();
				TCPServerDispatcher(const TCPServerDispatcher&);
				TCPServerDispatcher& operator = (const TCPServerDispatcher&);

				int _rc;
				TCPServerParams::Ptr _pParams;
				int  _currentThreads;
				int  _totalConnections;
				int  _currentConnections;
				int  _maxConcurrentConnections;
				int  _refusedConnections;
				bool _stopped;
				Nequeo::Threading::Notifications::NotificationQueue _queue;
				TCPServerConnectionFactory::Ptr _pConnectionFactory;
				Nequeo::Threading::ThreadPool& _threadPool;
				mutable Nequeo::Threading::FastMutex _mutex;
			};


			//
			// inlines
			//
			inline const TCPServerParams& TCPServerDispatcher::params() const
			{
				return *_pParams;
			}
		}
	}
}
#endif
