/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TimedNotificationQueue.h
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

#pragma once

#ifndef _TIMEDNOTIFICATIONQUEUE_H
#define _TIMEDNOTIFICATIONQUEUE_H

#include "GlobalThreading.h"

#include "Notification.h"
#include "Mutex.h"
#include "Event.h"
#include "Base\Timestamp.h"
#include <map>

namespace Nequeo {
	namespace Threading {
		namespace Notifications
		{
			/// A TimedNotificationQueue object provides a way to implement timed, asynchronous
			/// notifications. This is especially useful for sending notifications
			/// from one thread to another, for example from a background thread to 
			/// the main (user interface) thread. 
			///
			/// The TimedNotificationQueue is quite similar to the NotificationQueue class.
			/// The only difference to NotificationQueue is that each Notification is tagged
			/// with a Timestamp. When inserting a Notification into the queue, the
			/// Notification is inserted according to the given Timestamp, with 
			/// lower Timestamp values being inserted before higher ones.
			///
			/// Notifications are dequeued in order of their timestamps.
			///
			/// TimedNotificationQueue has some restrictions regarding multithreaded use.
			/// While multiple threads may enqueue notifications, only one thread at a
			/// time may dequeue notifications from the queue.
			///
			/// If two threads try to dequeue a notification simultaneously, the results
			/// are undefined.
			class TimedNotificationQueue
			{
			public:
				TimedNotificationQueue();
				/// Creates the TimedNotificationQueue.

				~TimedNotificationQueue();
				/// Destroys the TimedNotificationQueue.

				void enqueueNotification(Notification::Ptr pNotification, Nequeo::Timestamp timestamp);
				/// Enqueues the given notification by adding it to
				/// the queue according to the given timestamp.
				/// Lower timestamp values are inserted before higher ones.
				/// The queue takes ownership of the notification, thus
				/// a call like
				///     notificationQueue.enqueueNotification(new MyNotification, someTime);
				/// does not result in a memory leak.

				Notification* dequeueNotification();
				/// Dequeues the next pending notification with a timestamp
				/// less than or equal to the current time.
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

				bool empty() const;
				/// Returns true iff the queue is empty.

				int size() const;
				/// Returns the number of notifications in the queue.

				void clear();
				/// Removes all notifications from the queue.
				///
				/// Calling clear() while another thread executes one of
				/// the dequeue member functions will result in undefined
				/// behavior.

			protected:
				typedef std::multimap<Nequeo::Timestamp, Notification::Ptr> NfQueue;
				Notification::Ptr dequeueOne(NfQueue::iterator& it);
				bool wait(Nequeo::Timestamp::TimeDiff interval);

			private:
				NfQueue _nfQueue;
				Event   _nfAvailable;
				mutable FastMutex _mutex;
			};
		}
	}
}
#endif
