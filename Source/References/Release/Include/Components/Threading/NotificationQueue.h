/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NotificationQueue.h
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

#pragma once

#ifndef _NOTIFICATIONQUEUE_H
#define _NOTIFICATIONQUEUE_H

#include "GlobalThreading.h"

#include "Notification.h"
#include "Mutex.h"
#include "Event.h"
#include <deque>

namespace Nequeo {
	namespace Threading {
		namespace Notifications
		{
			class NotificationCenter;

			/// A NotificationQueue object provides a way to implement asynchronous
			/// notifications. This is especially useful for sending notifications
			/// from one thread to another, for example from a background thread to 
			/// the main (user interface) thread. 
			/// 
			/// The NotificationQueue can also be used to distribute work from
			/// a controlling thread to one or more worker threads. Each worker thread
			/// repeatedly calls waitDequeueNotification() and processes the
			/// returned notification. Special care must be taken when shutting
			/// down a queue with worker threads waiting for notifications.
			/// The recommended sequence to shut down and destroy the queue is to
			///   1. set a termination flag for every worker thread
			///   2. call the wakeUpAll() method
			///   3. join each worker thread
			///   4. destroy the notification queue.
			class NotificationQueue
			{
			public:
				NotificationQueue();
				/// Creates the NotificationQueue.

				~NotificationQueue();
				/// Destroys the NotificationQueue.

				void enqueueNotification(Notification::Ptr pNotification);
				/// Enqueues the given notification by adding it to
				/// the end of the queue (FIFO).
				/// The queue takes ownership of the notification, thus
				/// a call like
				///     notificationQueue.enqueueNotification(new MyNotification);
				/// does not result in a memory leak.

				void enqueueUrgentNotification(Notification::Ptr pNotification);
				/// Enqueues the given notification by adding it to
				/// the front of the queue (LIFO). The event therefore gets processed
				/// before all other events already in the queue.
				/// The queue takes ownership of the notification, thus
				/// a call like
				///     notificationQueue.enqueueUrgentNotification(new MyNotification);
				/// does not result in a memory leak.

				Notification* dequeueNotification();
				/// Dequeues the next pending notification.
				/// Returns 0 (null) if no notification is available.
				/// The caller gains ownership of the notification and
				/// is expected to release it when done with it.
				///
				/// It is highly recommended that the result is immediately
				/// assigned to a Notification::Ptr, to avoid potential
				/// memory management issues.

				Notification* waitDequeueNotification();
				/// Dequeues the next pending notification.
				/// If no notification is available, waits for a notification
				/// to be enqueued. 
				/// The caller gains ownership of the notification and
				/// is expected to release it when done with it.
				/// This method returns 0 (null) if wakeUpWaitingThreads()
				/// has been called by another thread.
				///
				/// It is highly recommended that the result is immediately
				/// assigned to a Notification::Ptr, to avoid potential
				/// memory management issues.

				Notification* waitDequeueNotification(long milliseconds);
				/// Dequeues the next pending notification.
				/// If no notification is available, waits for a notification
				/// to be enqueued up to the specified time.
				/// Returns 0 (null) if no notification is available.
				/// The caller gains ownership of the notification and
				/// is expected to release it when done with it.
				///
				/// It is highly recommended that the result is immediately
				/// assigned to a Notification::Ptr, to avoid potential
				/// memory management issues.

				void dispatch(NotificationCenter& notificationCenter);
				/// Dispatches all queued notifications to the given
				/// notification center.

				void wakeUpAll();
				/// Wakes up all threads that wait for a notification.

				bool empty() const;
				/// Returns true iff the queue is empty.

				int size() const;
				/// Returns the number of notifications in the queue.

				void clear();
				/// Removes all notifications from the queue.

				bool hasIdleThreads() const;
				/// Returns true if the queue has at least one thread waiting 
				/// for a notification.

				static NotificationQueue& defaultQueue();
				/// Returns a reference to the default
				/// NotificationQueue.

			protected:
				Notification::Ptr dequeueOne();

			private:
				typedef std::deque<Notification::Ptr> NfQueue;
				struct WaitInfo
				{
					Notification::Ptr pNf;
					Event             nfAvailable;
				};
				typedef std::deque<WaitInfo*> WaitQueue;

				NfQueue           _nfQueue;
				WaitQueue         _waitQueue;
				mutable FastMutex _mutex;
			};
		}
	}
}
#endif
