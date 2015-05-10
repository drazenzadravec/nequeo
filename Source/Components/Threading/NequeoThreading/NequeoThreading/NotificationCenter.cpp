/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NotificationCenter.cpp
*  Purpose :       NotificationCenter class.
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

#include "NotificationCenter.h"
#include "Notification.h"
#include "Observer.h"
#include "Base\AutoPtr.h"
#include "SingletonHolder.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

namespace Nequeo {
	namespace Threading {
		namespace Notifications
		{
			NotificationCenter::NotificationCenter()
			{
			}


			NotificationCenter::~NotificationCenter()
			{
			}


			void NotificationCenter::addObserver(const AbstractObserver& observer)
			{
				Mutex::ScopedLock lock(_mutex);
				_observers.push_back(observer.clone());
			}


			void NotificationCenter::removeObserver(const AbstractObserver& observer)
			{
				Mutex::ScopedLock lock(_mutex);
				for (ObserverList::iterator it = _observers.begin(); it != _observers.end(); ++it)
				{
					if (observer.equals(**it))
					{
						(*it)->disable();
						_observers.erase(it);
						return;
					}
				}
			}


			void NotificationCenter::postNotification(Notification::Ptr pNotification)
			{
				ScopedLockWithUnlock<Mutex> lock(_mutex);
				ObserverList observersToNotify(_observers);
				lock.unlock();
				for (ObserverList::iterator it = observersToNotify.begin(); it != observersToNotify.end(); ++it)
				{
					(*it)->notify(pNotification);
				}
			}


			bool NotificationCenter::hasObservers() const
			{
				Mutex::ScopedLock lock(_mutex);

				return !_observers.empty();
			}


			std::size_t NotificationCenter::countObservers() const
			{
				Mutex::ScopedLock lock(_mutex);

				return _observers.size();
			}


			namespace
			{
				static SingletonHolder<NotificationCenter> sh;
			}


			NotificationCenter& NotificationCenter::defaultCenter()
			{
				return *sh.get();
			}
		}
	}
}