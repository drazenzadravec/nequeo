/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ActiveDispatcher.h
*  Purpose :       ActiveDispatcher class.
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

#ifndef _ACTIVEDISPATCHER_H
#define _ACTIVEDISPATCHER_H

#include "GlobalThreading.h"

#include "Runnable.h"
#include "Thread.h"
#include "ActiveStarter.h"
#include "ActiveRunnable.h"
#include "NotificationQueue.h"

using Nequeo::Threading::Notifications::NotificationQueue;

namespace Nequeo {
	namespace Threading
	{
		class ActiveDispatcher : protected Runnable
			/// This class is used to implement an active object
			/// with strictly serialized method execution.
			///
			/// An active object, with is an ordinary object
			/// containing ActiveMethod members, executes all
			/// active methods in their own thread. 
			/// This behavior does not fit the "classic"
			/// definition of an active object, which serializes
			/// the execution of active methods (in other words,
			/// only one active method can be running at any given
			/// time).
			///
			/// Using this class as a base class, the serializing
			/// behavior for active objects can be implemented.
			/// 
			/// The following example shows how this is done:
			///
			///     class ActiveObject: public ActiveDispatcher
			///     {
			///     public:
			///         ActiveObject():
			///             exampleActiveMethod(this, &ActiveObject::exampleActiveMethodImpl)
			///         {
			///         }
			///
			///         ActiveMethod<std::string, std::string, ActiveObject, ActiveStarter<ActiveDispatcher> > exampleActiveMethod;
			///
			///     protected:
			///         std::string exampleActiveMethodImpl(const std::string& arg)
			///         {
			///             ...
			///         }
			///     };
			///
			/// The only things different from the example in
			/// ActiveMethod is that the ActiveObject in this case
			/// inherits from ActiveDispatcher, and that the ActiveMethod
			/// template for exampleActiveMethod has an additional parameter,
			/// specifying the specialized ActiveStarter for ActiveDispatcher.
		{
		public:
			ActiveDispatcher();
			/// Creates the ActiveDispatcher.

			ActiveDispatcher(Nequeo::Threading::Priority prio);
			/// Creates the ActiveDispatcher and sets
			/// the priority of its thread.

			virtual ~ActiveDispatcher();
			/// Destroys the ActiveDispatcher.

			void start(ActiveRunnableBase::Ptr pRunnable);
			/// Adds the Runnable to the dispatch queue.

			void cancel();
			/// Cancels all queued methods.

		protected:
			void run();
			void stop();

		private:
			Thread            _thread;
			NotificationQueue _queue;
		};


		template <>
		class ActiveStarter < ActiveDispatcher >
			/// A specialization of ActiveStarter
			/// for ActiveDispatcher.
		{
		public:
			static void start(ActiveDispatcher* pOwner, ActiveRunnableBase::Ptr pRunnable)
			{
				pOwner->start(pRunnable);
			}
		};
	}
}
#endif
