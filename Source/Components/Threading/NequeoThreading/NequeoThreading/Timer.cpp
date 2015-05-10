/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Timer.cpp
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

#include "stdafx.h"

#include "Timer.h"
#include "ThreadPool.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"
#include "ErrorHandler.h"
#include "Base\Timestamp.h"

using Nequeo::Timestamp;

namespace Nequeo {
	namespace Threading
	{
		Timer::Timer(long startInterval, long periodicInterval) :
			_startInterval(startInterval),
			_periodicInterval(periodicInterval),
			_skipped(0),
			_pCallback(0)
		{
		}


		Timer::~Timer()
		{
			stop();
		}


		void Timer::start(const AbstractTimerCallback& method)
		{
			start(method, Nequeo::Threading::PRIO_NORMAL, ThreadPool::defaultPool());
		}


		void Timer::start(const AbstractTimerCallback& method, Nequeo::Threading::Priority priority)
		{
			start(method, priority, ThreadPool::defaultPool());
		}


		void Timer::start(const AbstractTimerCallback& method, ThreadPool& threadPool)
		{
			start(method, Nequeo::Threading::PRIO_NORMAL, threadPool);
		}


		void Timer::start(const AbstractTimerCallback& method, Nequeo::Threading::Priority priority, ThreadPool& threadPool)
		{
			Timestamp nextInvocation;
			nextInvocation += static_cast<Timestamp::TimeVal>(_startInterval)* 1000;

			if (_pCallback)
			{
				FastMutex::ScopedLock lock(_mutex);
				_nextInvocation = nextInvocation;
				_pCallback = method.clone();
				_wakeUp.reset();
				threadPool.startWithPriority(priority, *this);
			}
		}


		void Timer::stop()
		{
			FastMutex::ScopedLock lock(_mutex);
			if (_pCallback)
			{
				_periodicInterval = 0;
				_mutex.unlock();
				_wakeUp.set();
				_done.wait(); // warning: deadlock if called from timer callback
				_mutex.lock();
				delete _pCallback;
				_pCallback = 0;
			}
		}


		void Timer::restart()
		{
			FastMutex::ScopedLock lock(_mutex);
			if (_pCallback)
			{
				_wakeUp.set();
			}
		}


		void Timer::restart(long milliseconds)
		{
			if (milliseconds >= 0)
			{
				FastMutex::ScopedLock lock(_mutex);
				if (_pCallback)
				{
					_periodicInterval = milliseconds;
					_wakeUp.set();
				}
			}
		}


		long Timer::getStartInterval() const
		{
			FastMutex::ScopedLock lock(_mutex);
			return _startInterval;
		}


		void Timer::setStartInterval(long milliseconds)
		{
			if (milliseconds >= 0)
			{
				FastMutex::ScopedLock lock(_mutex);
				_startInterval = milliseconds;
			}
		}


		long Timer::getPeriodicInterval() const
		{
			FastMutex::ScopedLock lock(_mutex);
			return _periodicInterval;
		}


		void Timer::setPeriodicInterval(long milliseconds)
		{
			if (milliseconds >= 0)
			{
				FastMutex::ScopedLock lock(_mutex);
				_periodicInterval = milliseconds;
			}
		}


		void Timer::run()
		{
			Timestamp now;
			long interval(0);
			do
			{
				long sleep(0);
				do
				{
					now.update();
					sleep = static_cast<long>((_nextInvocation - now) / 1000);
					if (sleep < 0)
					{
						if (interval == 0)
						{
							sleep = 0;
							break;
						}
						_nextInvocation += static_cast<Timestamp::TimeVal>(interval)* 1000;
						++_skipped;
					}
				} while (sleep < 0);

				if (_wakeUp.tryWait(sleep))
				{
					FastMutex::ScopedLock lock(_mutex);
					_nextInvocation.update();
					interval = _periodicInterval;
				}
				else
				{
					try
					{
						_pCallback->invoke(*this);
					}
					catch (Exception& exc)
					{
						ErrorHandler::handle(exc);
					}
					catch (std::exception& exc)
					{
						ErrorHandler::handle(exc);
					}
					catch (...)
					{
						ErrorHandler::handle();
					}
					interval = _periodicInterval;
				}
				_nextInvocation += static_cast<Timestamp::TimeVal>(interval)* 1000;
				_skipped = 0;
			} while (interval > 0);
			_done.set();
		}


		long Timer::skipped() const
		{
			return _skipped;
		}


		AbstractTimerCallback::AbstractTimerCallback()
		{
		}


		AbstractTimerCallback::AbstractTimerCallback(const AbstractTimerCallback& callback)
		{
		}


		AbstractTimerCallback::~AbstractTimerCallback()
		{
		}


		AbstractTimerCallback& AbstractTimerCallback::operator = (const AbstractTimerCallback& callback)
		{
			return *this;
		}
	}
}