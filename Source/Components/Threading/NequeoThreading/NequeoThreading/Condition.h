/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Condition.h
*  Purpose :       Condition class.
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

#ifndef _CONDITION_H
#define _CONDITION_H

#include "GlobalThreading.h"

#include "Mutex.h"
#include "ScopedUnlock.h"
#include "Event.h"
#include "Exceptions\Exception.h"

namespace Nequeo {
	namespace Threading
	{
		/// A Condition is a synchronization object used to block a thread 
		/// until a particular condition is met. 
		/// A Condition object is always used in conjunction with
		/// a Mutex (or FastMutex) object.
		///
		/// Condition objects are similar to POSIX condition variables, which the
		/// difference that Condition is not subject to spurious wakeups.
		///
		/// Threads waiting on a Condition are resumed in FIFO order.
		class Condition
		{
		public:
			Condition();
			/// Creates the Condition.

			~Condition();
			/// Destroys the Condition.

			template <class Mtx>
			void wait(Mtx& mutex)
				/// Unlocks the mutex (which must be locked upon calling
				/// wait()) and waits until the Condition is signalled.
				///
				/// The given mutex will be locked again upon 
				/// leaving the function, even in case of an exception.
			{
				ScopedUnlock<Mtx> unlock(mutex, false);
				Event event;
				{
					FastMutex::ScopedLock lock(_mutex);
					mutex.unlock();
					enqueue(event);
				}
				event.wait();
			}

			template <class Mtx>
			void wait(Mtx& mutex, long milliseconds)
				/// Unlocks the mutex (which must be locked upon calling
				/// wait()) and waits for the given time until the Condition is signalled.
				///
				/// The given mutex will be locked again upon successfully leaving the 
				/// function, even in case of an exception.
				///
				/// Throws a TimeoutException if the Condition is not signalled
				/// within the given time interval.
			{
				if (!tryWait(mutex, milliseconds))
					throw TimeoutException();
			}

			template <class Mtx>
			bool tryWait(Mtx& mutex, long milliseconds)
				/// Unlocks the mutex (which must be locked upon calling
				/// tryWait()) and waits for the given time until the Condition is signalled.
				///
				/// The given mutex will be locked again upon leaving the 
				/// function, even in case of an exception.
				///
				/// Returns true if the Condition has been signalled
				/// within the given time interval, otherwise false.
			{
				ScopedUnlock<Mtx> unlock(mutex, false);
				Event event;
				{
					FastMutex::ScopedLock lock(_mutex);
					mutex.unlock();
					enqueue(event);
				}
				if (!event.tryWait(milliseconds))
				{
					FastMutex::ScopedLock lock(_mutex);
					dequeue(event);
					return false;
				}
				return true;
			}

			void signal();
			/// Signals the Condition and allows one waiting thread
			/// to continue execution.

			void broadcast();
			/// Signals the Condition and allows all waiting
			/// threads to continue their execution.

		protected:
			void enqueue(Event& event);
			void dequeue();
			void dequeue(Event& event);

		private:
			Condition(const Condition&);
			Condition& operator = (const Condition&);

			typedef std::deque<Event*> WaitQueue;

			FastMutex _mutex;
			WaitQueue _waitQueue;
		};
	}
}
#endif
