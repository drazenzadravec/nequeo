/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          PriorityNotificationQueue.cpp
*  Purpose :       PriorityNotificationQueue class.
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

#include "PriorityNotificationQueue.h"
#include "NotificationCenter.h"
#include "Notification.h"
#include "SingletonHolder.h"

namespace Nequeo {
	namespace Threading {
		namespace Notifications
		{
			PriorityNotificationQueue::PriorityNotificationQueue()
			{
			}


			PriorityNotificationQueue::~PriorityNotificationQueue()
			{
				clear();
			}


			void PriorityNotificationQueue::enqueueNotification(Notification::Ptr pNotification, int priority)
			{
				FastMutex::ScopedLock lock(_mutex);
				if (_waitQueue.empty())
				{
					_nfQueue.insert(NfQueue::value_type(priority, pNotification));
				}
				else
				{
					WaitInfo* pWI = _waitQueue.front();
					_waitQueue.pop_front();
					pWI->pNf = pNotification;
					pWI->nfAvailable.set();
				}
			}


			Notification* PriorityNotificationQueue::dequeueNotification()
			{
				FastMutex::ScopedLock lock(_mutex);
				return dequeueOne().duplicate();
			}


			Notification* PriorityNotificationQueue::waitDequeueNotification()
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


			Notification* PriorityNotificationQueue::waitDequeueNotification(long milliseconds)
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


			void PriorityNotificationQueue::dispatch(NotificationCenter& notificationCenter)
			{
				FastMutex::ScopedLock lock(_mutex);
				Notification::Ptr pNf = dequeueOne();
				while (pNf)
				{
					notificationCenter.postNotification(pNf);
					pNf = dequeueOne();
				}
			}


			void PriorityNotificationQueue::wakeUpAll()
			{
				FastMutex::ScopedLock lock(_mutex);
				for (WaitQueue::iterator it = _waitQueue.begin(); it != _waitQueue.end(); ++it)
				{
					(*it)->nfAvailable.set();
				}
				_waitQueue.clear();
			}


			bool PriorityNotificationQueue::empty() const
			{
				FastMutex::ScopedLock lock(_mutex);
				return _nfQueue.empty();
			}


			int PriorityNotificationQueue::size() const
			{
				FastMutex::ScopedLock lock(_mutex);
				return static_cast<int>(_nfQueue.size());
			}


			void PriorityNotificationQueue::clear()
			{
				FastMutex::ScopedLock lock(_mutex);
				_nfQueue.clear();
			}


			bool PriorityNotificationQueue::hasIdleThreads() const
			{
				FastMutex::ScopedLock lock(_mutex);
				return !_waitQueue.empty();
			}


			Notification::Ptr PriorityNotificationQueue::dequeueOne()
			{
				Notification::Ptr pNf;
				NfQueue::iterator it = _nfQueue.begin();
				if (it != _nfQueue.end())
				{
					pNf = it->second;
					_nfQueue.erase(it);
				}
				return pNf;
			}


			namespace
			{
				static SingletonHolder<PriorityNotificationQueue> sh;
			}


			PriorityNotificationQueue& PriorityNotificationQueue::defaultQueue()
			{
				return *sh.get();
			}
		}
	}
}