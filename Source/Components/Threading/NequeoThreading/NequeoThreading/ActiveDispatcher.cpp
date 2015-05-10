/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ActiveDispatcher.cpp
*  Purpose :       ActiveDispatcher class.
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

#include "ActiveDispatcher.h"
#include "Notification.h"
#include "Base\AutoPtr.h"

using Nequeo::AutoPtr;
using Nequeo::Threading::Notifications::Notification;

namespace Nequeo {
	namespace Threading
	{
		class MethodNotification : public Notification
		{
		public:
			MethodNotification(ActiveRunnableBase::Ptr pRunnable) :
				_pRunnable(pRunnable)
			{
			}

			ActiveRunnableBase::Ptr runnable() const
			{
				return _pRunnable;
			}

		private:
			ActiveRunnableBase::Ptr _pRunnable;
		};

		class StopNotification : public Notification
		{
		};

		ActiveDispatcher::ActiveDispatcher()
		{
			_thread.start(*this);
		}


		ActiveDispatcher::ActiveDispatcher(Nequeo::Threading::Priority prio)
		{
			_thread.setPriority(prio);
			_thread.start(*this);
		}


		ActiveDispatcher::~ActiveDispatcher()
		{
			try
			{
				stop();
			}
			catch (...)
			{
			}
		}


		void ActiveDispatcher::start(ActiveRunnableBase::Ptr pRunnable)
		{
			_queue.enqueueNotification(new MethodNotification(pRunnable));
		}


		void ActiveDispatcher::cancel()
		{
			_queue.clear();
		}


		void ActiveDispatcher::run()
		{
			AutoPtr<Notification> pNf = _queue.waitDequeueNotification();
			while (pNf && !dynamic_cast<StopNotification*>(pNf.get()))
			{
				MethodNotification* pMethodNf = dynamic_cast<MethodNotification*>(pNf.get());
				
				if (pMethodNf != nullptr)
				{
					ActiveRunnableBase::Ptr pRunnable = pMethodNf->runnable();
					pRunnable->duplicate(); // run will release
					pRunnable->run();
					pNf = _queue.waitDequeueNotification();
				}
			}
		}


		void ActiveDispatcher::stop()
		{
			_queue.clear();
			_queue.wakeUpAll();
			_queue.enqueueNotification(new StopNotification);
			_thread.join();
		}
	}
}