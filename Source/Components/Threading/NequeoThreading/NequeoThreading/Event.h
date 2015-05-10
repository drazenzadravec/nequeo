/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Event.h
*  Purpose :       Event class.
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

#ifndef _EVENT_H
#define _EVENT_H

#include "GlobalThreading.h"

#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"
#include "Event_WIN32.h"

namespace Nequeo {
	namespace Threading
	{
		/// An Event is a synchronization object that
		/// allows one thread to signal one or more
		/// other threads that a certain event
		/// has happened.
		/// Usually, one thread signals an event,
		/// while one or more other threads wait
		/// for an event to become signalled.
		class Event : private EventImpl
		{
		public:
			Event(bool autoReset = true);
			/// Creates the event. If autoReset is true,
			/// the event is automatically reset after
			/// a wait() successfully returns.

			~Event();
			/// Destroys the event.

			void set();
			/// Signals the event. If autoReset is true,
			/// only one thread waiting for the event 
			/// can resume execution.
			/// If autoReset is false, all waiting threads
			/// can resume execution.

			void wait();
			/// Waits for the event to become signalled.

			void wait(long milliseconds);
			/// Waits for the event to become signalled.
			/// Throws a TimeoutException if the event
			/// does not become signalled within the specified
			/// time interval.

			bool tryWait(long milliseconds);
			/// Waits for the event to become signalled.
			/// Returns true if the event
			/// became signalled within the specified
			/// time interval, false otherwise.

			void reset();
			/// Resets the event to unsignalled state.

		private:
			Event(const Event&);
			Event& operator = (const Event&);
		};


		//
		// inlines
		//
		inline void Event::set()
		{
			setImpl();
		}


		inline void Event::wait()
		{
			waitImpl();
		}


		inline void Event::wait(long milliseconds)
		{
			if (!waitImpl(milliseconds))
				throw Nequeo::Exceptions::TimeoutException();
		}


		inline bool Event::tryWait(long milliseconds)
		{
			return waitImpl(milliseconds);
		}


		inline void Event::reset()
		{
			resetImpl();
		}
	}
}
#endif
