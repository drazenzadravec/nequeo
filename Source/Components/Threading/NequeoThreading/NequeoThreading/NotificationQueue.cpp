/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NotificationQueue.cpp
*  Purpose :       NotificationQueue class.
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

#include "NotificationQueue.h"
#include "NotificationCenter.h"
#include "Notification.h"
#include "SingletonHolder.h"

namespace Nequeo {
	namespace Threading {
		namespace Notifications
		{
			NotificationQueue::NotificationQueue()
			{
			}


			NotificationQueue::~NotificationQueue()
			{
				clear();
			}


			void NotificationQueue::enqueueNotification(Notification::Ptr pNotification)
			{
				FastMutex::ScopedLock lock(_mutex);
				if (_waitQueue.empty())
				{
					_nfQueue.push_back(pNotification);
				}
				else
				{
					WaitInfo* pWI = _waitQueue.front();
					_waitQueue.pop_front();
					pWI->pNf = pNotification;
					pWI->nfAvailable.set();
				}
			}


			void NotificationQueue::enqueueUrgentNotification(Notification::Ptr pNotification)
			{
				FastMutex::ScopedLock lock(_mutex);
				if (_waitQueue.empty())
				{
					_nfQueue.push_front(pNotification);
				}
				else
				{
					WaitInfo* pWI = _waitQueue.front();
					_waitQueue.pop_front();
					pWI->pNf = pNotification;
					pWI->nfAvailable.set();
				}
			}


			Notification* NotificationQueue::dequeueNotification()
			{
				FastMutex::ScopedLock lock(_mutex);
				return dequeueOne().duplicate();
			}


			Notification* NotificationQueue::waitDequeueNotification()
			{
				Notification::Ptr pNf;
				WaitInfo* pWI = 0;
				{
					FastMutex::ScopedLock lock(_mutex);
					pNf = dequeueOne();
					if (pNf) return pNf.duplicate();
					pWI = new WaitInfo;
					_waitQueue.push_back(pWI);
				}
				pWI->nfAvailable.wait();
				pNf = pWI->pNf;
				delete pWI;
				return pNf.duplicate();
			}


			Notification* NotificationQueue::waitDequeueNotification(long milliseconds)
			{
				Notification::Ptr pNf;
				WaitInfo* pWI = 0;
				{
					FastMutex::ScopedLock lock(_mutex);
					pNf = dequeueOne();
					if (pNf) return pNf.duplicate();
					pWI = new WaitInfo;
					_waitQueue.push_back(pWI);
				}
				if (pWI->nfAvailable.tryWait(milliseconds))
				{
					pNf = pWI->pNf;
				}
				else
				{
					FastMutex::ScopedLock lock(_mutex);
					pNf = pWI->pNf;
					for (WaitQueue::iterator it = _waitQueue.begin(); it != _waitQueue.end(); ++it)
					{
						if (*it == pWI)
						{
							_waitQueue.erase(it);
							break;
						}
					}
				}
				delete pWI;
				return pNf.duplicate();
			}


			void NotificationQueue::dispatch(NotificationCenter& notificationCenter)
			{
				FastMutex::ScopedLock lock(_mutex);
				Notification::Ptr pNf = dequeueOne();
				while (pNf)
				{
					notificationCenter.postNotification(pNf);
					pNf = dequeueOne();
				}
			}


			void NotificationQueue::wakeUpAll()
			{
				FastMutex::ScopedLock lock(_mutex);
				for (WaitQueue::iterator it = _waitQueue.begin(); it != _waitQueue.end(); ++it)
				{
					(*it)->nfAvailable.set();
				}
				_waitQueue.clear();
			}


			bool NotificationQueue::empty() const
			{
				FastMutex::ScopedLock lock(_mutex);
				return _nfQueue.empty();
			}


			int NotificationQueue::size() const
			{
				FastMutex::ScopedLock lock(_mutex);
				return static_cast<int>(_nfQueue.size());
			}


			void NotificationQueue::clear()
			{
				FastMutex::ScopedLock lock(_mutex);
				_nfQueue.clear();
			}


			bool NotificationQueue::hasIdleThreads() const
			{
				FastMutex::ScopedLock lock(_mutex);
				return !_waitQueue.empty();
			}


			Notification::Ptr NotificationQueue::dequeueOne()
			{
				Notification::Ptr pNf;
				if (!_nfQueue.empty())
				{
					pNf = _nfQueue.front();
					_nfQueue.pop_front();
				}
				return pNf;
			}


			namespace
			{
				static SingletonHolder<NotificationQueue> sh;
			}


			NotificationQueue& NotificationQueue::defaultQueue()
			{
				return *sh.get();
			}
		}
	}
}