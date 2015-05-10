/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TimedNotificationQueue.cpp
*  Purpose :       TimedNotificationQueue class.
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

#include "TimedNotificationQueue.h"
#include "Notification.h"
#include "Base\Timestamp.h"
#include <limits>

namespace Nequeo {
	namespace Threading {
		namespace Notifications
		{
			TimedNotificationQueue::TimedNotificationQueue()
			{
			}


			TimedNotificationQueue::~TimedNotificationQueue()
			{
				clear();
			}


			void TimedNotificationQueue::enqueueNotification(Notification::Ptr pNotification, Nequeo::Timestamp timestamp)
			{
				FastMutex::ScopedLock lock(_mutex);
				_nfQueue.insert(NfQueue::value_type(timestamp, pNotification));
				_nfAvailable.set();
			}


			Notification* TimedNotificationQueue::dequeueNotification()
			{
				FastMutex::ScopedLock lock(_mutex);

				NfQueue::iterator it = _nfQueue.begin();
				if (it != _nfQueue.end())
				{
					Nequeo::Timestamp::TimeDiff sleep = -it->first.elapsed();
					if (sleep <= 0)
					{
						Notification::Ptr pNf = it->second;
						_nfQueue.erase(it);
						return pNf.duplicate();
					}
				}
				return 0;
			}


			Notification* TimedNotificationQueue::waitDequeueNotification()
			{
				for (;;)
				{
					_mutex.lock();
					NfQueue::iterator it = _nfQueue.begin();
					if (it != _nfQueue.end())
					{
						_mutex.unlock();
						Nequeo::Timestamp::TimeDiff sleep = -it->first.elapsed();
						if (sleep <= 0)
						{
							return dequeueOne(it).duplicate();
						}
						else if (!wait(sleep))
						{
							return dequeueOne(it).duplicate();
						}
						else continue;
					}
					else
					{
						_mutex.unlock();
					}
					_nfAvailable.wait();
				}
			}


			Notification* TimedNotificationQueue::waitDequeueNotification(long milliseconds)
			{
				while (milliseconds >= 0)
				{
					_mutex.lock();
					NfQueue::iterator it = _nfQueue.begin();
					if (it != _nfQueue.end())
					{
						_mutex.unlock();
						Nequeo::Timestamp now;
						Nequeo::Timestamp::TimeDiff sleep = it->first - now;
						if (sleep <= 0)
						{
							return dequeueOne(it).duplicate();
						}
						else if (sleep <= 1000 * Nequeo::Timestamp::TimeDiff(milliseconds))
						{
							if (!wait(sleep))
							{
								return dequeueOne(it).duplicate();
							}
							else
							{
								milliseconds -= static_cast<long>((now.elapsed() + 999) / 1000);
								continue;
							}
						}
					}
					else
					{
						_mutex.unlock();
					}
					if (milliseconds > 0)
					{
						Nequeo::Timestamp now;
						_nfAvailable.tryWait(milliseconds);
						milliseconds -= static_cast<long>((now.elapsed() + 999) / 1000);
					}
					else return 0;
				}
				return 0;
			}


			bool TimedNotificationQueue::wait(Nequeo::Timestamp::TimeDiff interval)
			{
				const Nequeo::Timestamp::TimeDiff MAX_SLEEP = 8 * 60 * 60 * Nequeo::Timestamp::TimeDiff(1000000); // sleep at most 8 hours at a time
				while (interval > 0)
				{
					Nequeo::Timestamp now;
					Nequeo::Timestamp::TimeDiff sleep = interval <= MAX_SLEEP ? interval : MAX_SLEEP;
					if (_nfAvailable.tryWait(static_cast<long>((sleep + 999) / 1000)))
						return true;
					interval -= now.elapsed();
				}
				return false;
			}


			bool TimedNotificationQueue::empty() const
			{
				FastMutex::ScopedLock lock(_mutex);
				return _nfQueue.empty();
			}


			int TimedNotificationQueue::size() const
			{
				FastMutex::ScopedLock lock(_mutex);
				return static_cast<int>(_nfQueue.size());
			}


			void TimedNotificationQueue::clear()
			{
				FastMutex::ScopedLock lock(_mutex);
				_nfQueue.clear();
			}


			Notification::Ptr TimedNotificationQueue::dequeueOne(NfQueue::iterator& it)
			{
				FastMutex::ScopedLock lock(_mutex);
				Notification::Ptr pNf = it->second;
				_nfQueue.erase(it);
				return pNf;
			}
		}
	}
}