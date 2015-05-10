/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Activity.h
*  Purpose :       Activity class.
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

#ifndef _ACTIVITY_H
#define _ACTIVITY_H

#include "GlobalThreading.h"

#include "RunnableAdapter.h"
#include "ThreadPool.h"
#include "Event.h"
#include "Mutex.h"

namespace Nequeo {
	namespace Threading
	{
		/// This template class helps to implement active objects.
		/// An active object uses threads to decouple method
		/// execution from method invocation, or to perform tasks
		/// autonomously, without intervention of a caller.
		template <class C>
		class Activity : public Runnable
			/// An activity is a (typically longer running) method
			/// that executes within its own task. Activities can
			/// be started automatically (upon object construction)
			/// or manually at a later time. Activities can also
			/// be stopped at any time. However, to make stopping
			/// an activity work, the method implementing the
			/// activity has to check periodically whether it
			/// has been requested to stop, and if so, return. 
			/// Activities are stopped before the object they belong to is
			/// destroyed. Methods implementing activities cannot have arguments
			/// or return values. 
			///
			/// Activity objects are used as follows:
			///
			///     class ActiveObject
			///     {
			///     public:
			///         ActiveObject(): 
			///             _activity(this, &ActiveObject::runActivity)
			///         {
			///             ...
			///         }
			///   
			///         ...
			///  
			///     protected:
			///         void runActivity()
			///         {
			///             while (!_activity.isStopped())
			///             {
			///                 ...
			///             }
			///         }
			///
			///     private:
			///         Activity<ActiveObject> _activity;
			///     };
		{
		public:
			typedef RunnableAdapter<C> RunnableAdapterType;
			typedef typename RunnableAdapterType::Callback Callback;

			Activity(C* pOwner, Callback method) :
				_pOwner(pOwner),
				_runnable(*pOwner, method),
				_stopped(true),
				_running(false),
				_done(false)
				/// Creates the activity. Call start() to
				/// start it.
			{
				poco_check_ptr(pOwner);
			}

			~Activity()
				/// Stops and destroys the activity.
			{
				stop();
				wait();
			}

			void start()
				/// Starts the activity by acquiring a
				/// thread for it from the default thread pool.
			{
				FastMutex::ScopedLock lock(_mutex);

				if (!_running)
				{
					_done.reset();
					_stopped = false;
					_running = true;
					try
					{
						ThreadPool::defaultPool().start(*this);
					}
					catch (...)
					{
						_running = false;
						throw;
					}
				}
			}

			void stop()
				/// Requests to stop the activity.
			{
				FastMutex::ScopedLock lock(_mutex);

				_stopped = true;
			}

			void wait()
				/// Waits for the activity to complete.
			{
				if (_running)
				{
					_done.wait();
				}
			}

			void wait(long milliseconds)
				/// Waits the given interval for the activity to complete.
				/// An TimeoutException is thrown if the activity does not
				/// complete within the given interval.
			{
				if (_running)
				{
					_done.wait(milliseconds);
				}
			}

			bool isStopped() const
				/// Returns true if the activity has been requested to stop.
			{
				return _stopped;
			}

			bool isRunning() const
				/// Returns true if the activity is running.
			{
				return _running;
			}

		protected:
			void run()
			{
				try
				{
					_runnable.run();
				}
				catch (...)
				{
					_running = false;
					_done.set();
					throw;
				}
				_running = false;
				_done.set();
			}

		private:
			Activity();
			Activity(const Activity&);
			Activity& operator = (const Activity&);

			C*                  _pOwner;
			RunnableAdapterType _runnable;
			volatile bool       _stopped;
			volatile bool       _running;
			Event               _done;
			FastMutex           _mutex;
		};
	}
}
#endif
