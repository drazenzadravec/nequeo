/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TCPServerParams.h
*  Purpose :       TCPServerParams class.
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

#ifndef _TCPSERVERPARAMS_H
#define _TCPSERVERPARAMS_H

#include "GlobalSocket.h"
#include "Base\RefCountedObject.h"
#include "Base\Timespan.h"
#include "Base\AutoPtr.h"
#include "Threading\Thread.h"

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			/// This class is used to specify parameters to both the
			/// TCPServer, as well as to TCPServerDispatcher objects.
			///
			/// Subclasses may add new parameters to the class.
			class TCPServerParams : public Nequeo::RefCountedObject
			{
			public:
				typedef Nequeo::AutoPtr<TCPServerParams> Ptr;

				TCPServerParams();
				/// Creates the TCPServerParams.
				///
				/// Sets the following default values:
				///   - threadIdleTime:       10 seconds
				///   - maxThreads:           0
				///   - maxQueued:            64

				void setThreadIdleTime(const Nequeo::Timespan& idleTime);
				/// Sets the maximum idle time for a thread before
				/// it is terminated.
				///
				/// The default idle time is 10 seconds;

				const Nequeo::Timespan& getThreadIdleTime() const;
				/// Returns the maximum thread idle time.

				void setMaxQueued(int count);
				/// Sets the maximum number of queued connections.
				/// Must be greater than 0.
				///
				/// If there are already the maximum number of connections
				/// in the queue, new connections will be silently discarded.
				///
				/// The default number is 64.

				int getMaxQueued() const;
				/// Returns the maximum number of queued connections.

				void setMaxThreads(int count);
				/// Sets the maximum number of simultaneous threads
				/// available for this TCPServerDispatcher.
				///
				/// Must be greater than or equal to 0.
				/// If 0 is specified, the TCPServerDispatcher will
				/// set this parameter to the number of available threads
				/// in its thread pool.
				///
				/// The thread pool used by the TCPServerDispatcher
				/// must at least have the capacity for the given
				/// number of threads.

				int getMaxThreads() const;
				/// Returns the maximum number of simultaneous threads
				/// available for this TCPServerDispatcher.	

				void setThreadPriority(Nequeo::Threading::Priority prio);
				/// Sets the priority of TCP server threads 
				/// created by TCPServer.

				Nequeo::Threading::Priority getThreadPriority() const;
				/// Returns the priority of TCP server threads
				/// created by TCPServer. 

			protected:
				virtual ~TCPServerParams();
				/// Destroys the TCPServerParams.

			private:
				Nequeo::Timespan _threadIdleTime;
				int _maxThreads;
				int _maxQueued;
				Nequeo::Threading::Priority _threadPriority;
			};


			//
			// inlines
			//
			inline const Nequeo::Timespan& TCPServerParams::getThreadIdleTime() const
			{
				return _threadIdleTime;
			}


			inline int TCPServerParams::getMaxThreads() const
			{
				return _maxThreads;
			}


			inline int TCPServerParams::getMaxQueued() const
			{
				return _maxQueued;
			}


			inline Nequeo::Threading::Priority TCPServerParams::getThreadPriority() const
			{
				return _threadPriority;
			}
		}
	}
}
#endif
