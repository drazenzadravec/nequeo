/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Timer.h
*  Purpose :       Timer header.
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

#include "GlobalMaintenance.h"
#include "TimerTask.h"
#include "Base\Timestamp.h"
#include "Threading\Runnable.h"
#include "Threading\Thread.h"
#include "Threading\TimedNotificationQueue.h"
#include "Threading\Priority.h"

namespace Nequeo {
	namespace Maintenance
	{
		/// A Timer allows to schedule tasks (TimerTask objects) for future execution 
		/// in a background thread. Tasks may be scheduled for one-time execution, 
		/// or for repeated execution at regular intervals. 
		///
		/// The Timer object creates a thread that executes all scheduled tasks
		/// sequentially. Therefore, tasks should complete their work as quickly
		/// as possible, otherwise subsequent tasks may be delayed.
		///
		/// Timer is save for multithreaded use - multiple threads can schedule
		/// new tasks simultaneously.
		class Timer : protected Nequeo::Threading::Runnable
		{
		public:
			Timer();
			/// Creates the Timer.

			explicit Timer(Nequeo::Threading::Priority priority);
			/// Creates the Timer, using a timer thread with
			/// the given priority.

			~Timer();
			/// Destroys the Timer, cancelling all pending tasks.

			void cancel(bool wait = false);
			/// Cancels all pending tasks.
			///
			/// If a task is currently running, it is allowed to finish.
			///
			/// Task cancellation is done asynchronously. If wait
			/// is false, cancel() returns immediately and the
			/// task queue will be purged as soon as the currently
			/// running task finishes. If wait is true, waits
			/// until the queue has been purged.

			void schedule(TimerTask::Ptr pTask, Nequeo::Timestamp time);
			/// Schedules a task for execution at the specified time.
			///
			/// If the time lies in the past, the task is executed
			/// immediately.

			void schedule(TimerTask::Ptr pTask, long delay, long interval);
			/// Schedules a task for periodic execution.
			///
			/// The task is first executed after the given delay.
			/// Subsequently, the task is executed periodically with
			/// the given interval in milliseconds between invocations.

			void schedule(TimerTask::Ptr pTask, Nequeo::Timestamp time, long interval);
			/// Schedules a task for periodic execution.
			///
			/// The task is first executed at the given time.
			/// Subsequently, the task is executed periodically with
			/// the given interval in milliseconds between invocations.

			void scheduleAtFixedRate(TimerTask::Ptr pTask, long delay, long interval);
			/// Schedules a task for periodic execution at a fixed rate.
			///
			/// The task is first executed after the given delay.
			/// Subsequently, the task is executed periodically 
			/// every number of milliseconds specified by interval.
			///
			/// If task execution takes longer than the given interval,
			/// further executions are delayed.

			void scheduleAtFixedRate(TimerTask::Ptr pTask, Nequeo::Timestamp time, long interval);
			/// Schedules a task for periodic execution at a fixed rate.
			///
			/// The task is first executed at the given time.
			/// Subsequently, the task is executed periodically 
			/// every number of milliseconds specified by interval.
			///
			/// If task execution takes longer than the given interval,
			/// further executions are delayed.

		protected:
			void run();

		private:
			Timer(const Timer&);
			Timer& operator = (const Timer&);

			Nequeo::Threading::Notifications::TimedNotificationQueue _queue;
			Nequeo::Threading::Thread _thread;
		};
	}
}
#endif