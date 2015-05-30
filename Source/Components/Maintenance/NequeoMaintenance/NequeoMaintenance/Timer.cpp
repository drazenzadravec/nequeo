/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Timer.cpp
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

#include "stdafx.h"

#include "Timer.h"
#include "Threading\Notification.h"
#include "Threading\ErrorHandler.h"
#include "Threading\Event.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

namespace Nequeo {
	namespace Maintenance
	{
		class TimerNotification : public Nequeo::Threading::Notifications::Notification
		{
		public:
			TimerNotification(Nequeo::Threading::Notifications::TimedNotificationQueue& queue) :
				_queue(queue)
			{
			}

			~TimerNotification()
			{
			}

			virtual bool execute() = 0;

			Nequeo::Threading::Notifications::TimedNotificationQueue& queue()
			{
				return _queue;
			}

		private:
			Nequeo::Threading::Notifications::TimedNotificationQueue& _queue;
		};


		class StopNotification : public TimerNotification
		{
		public:
			StopNotification(Nequeo::Threading::Notifications::TimedNotificationQueue& queue) :
				TimerNotification(queue)
			{
			}

			~StopNotification()
			{
			}

			bool execute()
			{
				queue().clear();
				return false;
			}
		};


		class CancelNotification : public TimerNotification
		{
		public:
			CancelNotification(Nequeo::Threading::Notifications::TimedNotificationQueue& queue) :
				TimerNotification(queue)
			{
			}

			~CancelNotification()
			{
			}

			bool execute()
			{
				queue().clear();
				_finished.set();
				return true;
			}

			void wait()
			{
				_finished.wait();
			}

		private:
			Nequeo::Threading::Event _finished;
		};


		class TaskNotification : public TimerNotification
		{
		public:
			TaskNotification(Nequeo::Threading::Notifications::TimedNotificationQueue& queue, TimerTask::Ptr pTask) :
				TimerNotification(queue),
				_pTask(pTask)
			{
			}

			~TaskNotification()
			{
			}

			TimerTask::Ptr task()
			{
				return _pTask;
			}

			bool execute()
			{
				if (!_pTask->isCancelled())
				{
					try
					{
						_pTask->_lastExecution.update();
						_pTask->run();
					}
					catch (Exception& exc)
					{
						Nequeo::Threading::ErrorHandler::handle(exc);
					}
					catch (std::exception& exc)
					{
						Nequeo::Threading::ErrorHandler::handle(exc);
					}
					catch (...)
					{
						Nequeo::Threading::ErrorHandler::handle();
					}
				}
				return true;
			}

		private:
			TimerTask::Ptr _pTask;
		};


		class PeriodicTaskNotification : public TaskNotification
		{
		public:
			PeriodicTaskNotification(Nequeo::Threading::Notifications::TimedNotificationQueue& queue, TimerTask::Ptr pTask, long interval) :
				TaskNotification(queue, pTask),
				_interval(interval)
			{
			}

			~PeriodicTaskNotification()
			{
			}

			bool execute()
			{
				TaskNotification::execute();

				if (!task()->isCancelled())
				{
					Nequeo::Timestamp now;
					Nequeo::Timestamp nextExecution;
					nextExecution += static_cast<Nequeo::Timestamp::TimeDiff>(_interval)* 1000;
					if (nextExecution < now) nextExecution = now;
					queue().enqueueNotification(this, nextExecution);
					duplicate();
				}
				return true;
			}

		private:
			long _interval;
		};


		class FixedRateTaskNotification : public TaskNotification
		{
		public:
			FixedRateTaskNotification(Nequeo::Threading::Notifications::TimedNotificationQueue& queue, TimerTask::Ptr pTask, long interval, Nequeo::Timestamp time) :
				TaskNotification(queue, pTask),
				_interval(interval),
				_nextExecution(time)
			{
			}

			~FixedRateTaskNotification()
			{
			}

			bool execute()
			{
				TaskNotification::execute();

				if (!task()->isCancelled())
				{
					Nequeo::Timestamp now;
					_nextExecution += static_cast<Nequeo::Timestamp::TimeDiff>(_interval)* 1000;
					if (_nextExecution < now) _nextExecution = now;
					queue().enqueueNotification(this, _nextExecution);
					duplicate();
				}
				return true;
			}

		private:
			long _interval;
			Nequeo::Timestamp _nextExecution;
		};


		Timer::Timer()
		{
			_thread.start(*this);
		}


		Timer::Timer(Nequeo::Threading::Priority priority)
		{
			_thread.setPriority(priority);
			_thread.start(*this);
		}


		Timer::~Timer()
		{
			_queue.enqueueNotification(new StopNotification(_queue), 0);
			_thread.join();
		}


		void Timer::cancel(bool wait)
		{
			Nequeo::AutoPtr<CancelNotification> pNf = new CancelNotification(_queue);
			_queue.enqueueNotification(pNf, 0);
			if (wait)
			{
				pNf->wait();
			}
		}


		void Timer::schedule(TimerTask::Ptr pTask, Nequeo::Timestamp time)
		{
			_queue.enqueueNotification(new TaskNotification(_queue, pTask), time);
		}


		void Timer::schedule(TimerTask::Ptr pTask, long delay, long interval)
		{
			Nequeo::Timestamp time;
			time += static_cast<Nequeo::Timestamp::TimeDiff>(delay)* 1000;
			schedule(pTask, time, interval);
		}


		void Timer::schedule(TimerTask::Ptr pTask, Nequeo::Timestamp time, long interval)
		{
			_queue.enqueueNotification(new PeriodicTaskNotification(_queue, pTask, interval), time);
		}


		void Timer::scheduleAtFixedRate(TimerTask::Ptr pTask, long delay, long interval)
		{
			Nequeo::Timestamp time;
			time += static_cast<Nequeo::Timestamp::TimeDiff>(delay)* 1000;
			scheduleAtFixedRate(pTask, time, interval);
		}


		void Timer::scheduleAtFixedRate(TimerTask::Ptr pTask, Nequeo::Timestamp time, long interval)
		{
			_queue.enqueueNotification(new FixedRateTaskNotification(_queue, pTask, interval, time), time);
		}


		void Timer::run()
		{
			bool cont = true;
			while (cont)
			{
				Nequeo::AutoPtr<TimerNotification> pNf = static_cast<TimerNotification*>(_queue.waitDequeueNotification());
				cont = pNf->execute();
			}
		}
	}
}