/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TimerTask.h
*  Purpose :       TimerTask header.
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

#ifndef _TIMERTASK_H
#define _TIMERTASK_H

#include "GlobalMaintenance.h"
#include "Base\AutoPtr.h"
#include "Base\RefCountedObject.h"
#include "Base\Timestamp.h"
#include "Threading\Runnable.h"

namespace Nequeo {
	namespace Maintenance
	{
		/// A task that can be scheduled for one-time or 
		/// repeated execution by a Timer.
		///
		/// This is an abstract class. Subclasses must override the run() member
		/// function to implement the actual task logic.
		class TimerTask : public Nequeo::RefCountedObject, public Nequeo::Threading::Runnable
		{
		public:
			typedef Nequeo::AutoPtr<TimerTask> Ptr;

			TimerTask();
			/// Creates the TimerTask.

			void cancel();
			/// Cancels the execution of the timer.
			/// If the task has been scheduled for one-time execution and has 
			/// not yet run, or has not yet been scheduled, it will never run. 
			/// If the task has been scheduled for repeated execution, it will never 
			/// run again. If the task is running when this call occurs, the task 
			/// will run to completion, but will never run again.	

			bool isCancelled() const;
			/// Returns true iff the TimerTask has been cancelled by a call
			/// to cancel().

			Nequeo::Timestamp lastExecution() const;
			/// Returns the time of the last execution of the timer task.
			///
			/// Returns 0 if the timer has never been executed.

		protected:
			~TimerTask();
			/// Destroys the TimerTask.

		private:
			TimerTask(const TimerTask&);
			TimerTask& operator = (const TimerTask&);

			Nequeo::Timestamp _lastExecution;
			bool _isCancelled;

			friend class TaskNotification;
		};


		///
		inline bool TimerTask::isCancelled() const
		{
			return _isCancelled;
		}

		///
		inline Nequeo::Timestamp TimerTask::lastExecution() const
		{
			return _lastExecution;
		}
	}
}
#endif