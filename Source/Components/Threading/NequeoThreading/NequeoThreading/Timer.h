/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Timer.h
*  Purpose :       Timer class.
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

#ifndef _TIMER_H
#define _TIMER_H

#include "GlobalThreading.h"

#include "Runnable.h"
#include "Mutex.h"
#include "Event.h"
#include "Thread.h"
#include "Base\Timestamp.h"

namespace Nequeo {
	namespace Threading
	{
		class AbstractTimerCallback;
		class ThreadPool;

		/// This class implements a thread-based timer.
		/// A timer starts a thread that first waits for a given start interval.
		/// Once that interval expires, the timer callback is called repeatedly
		/// in the given periodic interval. If the interval is 0, the timer is only
		/// called once.
		/// The timer callback method can stop the timer by setting the 
		/// timer's periodic interval to 0.
		///
		/// The timer callback runs in its own thread, so multithreading
		/// issues (proper synchronization) have to be considered when writing 
		/// the callback method.
		///
		/// The exact interval at which the callback is called depends on many 
		/// factors like operating system, CPU performance and system load and
		/// may differ from the specified interval.
		///
		/// The time needed to execute the timer callback is not included
		/// in the interval between invocations. For example, if the interval
		/// is 500 milliseconds, and the callback needs 400 milliseconds to
		/// execute, the callback function is nevertheless called every 500
		/// milliseconds. If the callback takes longer to execute than the
		/// interval, the callback function will not be called until the next
		/// proper interval. The number of skipped invocations since the last
		/// invocation will be recorded and can be obtained by the callback
		/// by calling skipped().
		///
		/// The timer thread is taken from a thread pool, so
		/// there is a limit to the number of available concurrent timers.
		class Timer : protected Runnable
		{
		public:
			Timer(long startInterval = 0, long periodicInterval = 0);
			/// Creates a new timer object. StartInterval and periodicInterval
			/// are given in milliseconds. If a periodicInterval of zero is 
			/// specified, the callback will only be called once, after the
			/// startInterval expires.
			/// To start the timer, call the Start() method.

			virtual ~Timer();
			/// Stops and destroys the timer.

			void start(const AbstractTimerCallback& method);
			/// Starts the timer.
			/// Create the TimerCallback as follows:
			///     TimerCallback<MyClass> callback(*this, &MyClass::onTimer);
			///     timer.start(callback);
			///
			/// The timer thread is taken from the global default thread pool.

			void start(const AbstractTimerCallback& method, Nequeo::Threading::Priority priority);
			/// Starts the timer in a thread with the given priority.
			/// Create the TimerCallback as follows:
			///     TimerCallback<MyClass> callback(*this, &MyClass::onTimer);
			///     timer.start(callback);
			///
			/// The timer thread is taken from the global default thread pool.

			void start(const AbstractTimerCallback& method, ThreadPool& threadPool);
			/// Starts the timer.
			/// Create the TimerCallback as follows:
			///     TimerCallback<MyClass> callback(*this, &MyClass::onTimer);
			///     timer.start(callback);

			void start(const AbstractTimerCallback& method, Nequeo::Threading::Priority priority, ThreadPool& threadPool);
			/// Starts the timer in a thread with the given priority.
			/// Create the TimerCallback as follows:
			///     TimerCallback<MyClass> callback(*this, &MyClass::onTimer);
			///     timer.start(callback);

			void stop();
			/// Stops the timer. If the callback method is currently running
			/// it will be allowed to finish first.
			/// WARNING: Never call this method from within the callback method,
			/// as a deadlock would result. To stop the timer from within the
			/// callback method, call restart(0).

			void restart();
			/// Restarts the periodic interval. If the callback method is already running,
			/// nothing will happen.

			void restart(long milliseconds);
			/// Sets a new periodic interval and restarts the timer.
			/// An interval of 0 will stop the timer.

			long getStartInterval() const;
			/// Returns the start interval.

			void setStartInterval(long milliseconds);
			/// Sets the start interval. Will only be 
			/// effective before start() is called.

			long getPeriodicInterval() const;
			/// Returns the periodic interval.

			void setPeriodicInterval(long milliseconds);
			/// Sets the periodic interval. If the timer is already running
			/// the new interval will be effective when the current interval
			/// expires.

			long skipped() const;
			/// Returns the number of skipped invocations since the last invocation.
			/// Skipped invocations happen if the timer callback function takes
			/// longer to execute than the timer interval.

		protected:
			void run();

		private:
			volatile long _startInterval;
			volatile long _periodicInterval;
			Event         _wakeUp;
			Event         _done;
			long          _skipped;
			AbstractTimerCallback* _pCallback;
			Nequeo::Timestamp  _nextInvocation;
			mutable FastMutex      _mutex;

			Timer(const Timer&);
			Timer& operator = (const Timer&);
		};

		/// This is the base class for all instantiations of
		/// the TimerCallback template.
		class AbstractTimerCallback
		{
		public:
			AbstractTimerCallback();
			AbstractTimerCallback(const AbstractTimerCallback& callback);
			virtual ~AbstractTimerCallback();

			AbstractTimerCallback& operator = (const AbstractTimerCallback& callback);

			virtual void invoke(Timer& timer) const = 0;
			virtual AbstractTimerCallback* clone() const = 0;
		};


		template <class C>
		class TimerCallback : public AbstractTimerCallback
			/// This template class implements an adapter that sits between
			/// a Timer and an object's method invoked by the timer.
			/// It is quite similar in concept to the RunnableAdapter, but provides 
			/// some Timer specific additional methods.
			/// See the Timer class for information on how
			/// to use this template class.
		{
		public:
			typedef void (C::*Callback)(Timer&);

			TimerCallback(C& object, Callback method) : _pObject(&object), _method(method)
			{
			}

			TimerCallback(const TimerCallback& callback) : _pObject(callback._pObject), _method(callback._method)
			{
			}

			~TimerCallback()
			{
			}

			TimerCallback& operator = (const TimerCallback& callback)
			{
				if (&callback != this)
				{
					_pObject = callback._pObject;
					_method = callback._method;
				}
				return *this;
			}

			void invoke(Timer& timer) const
			{
				(_pObject->*_method)(timer);
			}

			AbstractTimerCallback* clone() const
			{
				return new TimerCallback(*this);
			}

		private:
			TimerCallback();

			C*       _pObject;
			Callback _method;
		};
	}
}
#endif
