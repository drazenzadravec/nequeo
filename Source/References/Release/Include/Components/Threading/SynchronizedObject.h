/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          SynchronizedObject.h
*  Purpose :       SynchronizedObject class.
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

#ifndef _SYNCHRONIZEDOBJECT_H
#define _SYNCHRONIZEDOBJECT_H

#include "GlobalThreading.h"

#include "Mutex.h"
#include "Event.h"

namespace Nequeo {
	namespace Threading
	{
		/// This class aggregates a Mutex and an Event
		/// and can act as a base class for all objects
		/// requiring synchronization in a multithreaded
		/// scenario.
		class SynchronizedObject
		{
		public:
			typedef Nequeo::Threading::ScopedLock<SynchronizedObject> ScopedLock;

			SynchronizedObject();
			/// Creates the object.

			virtual ~SynchronizedObject();
			/// Destroys the object.

			void lock() const;
			/// Locks the object. Blocks if the object
			/// is locked by another thread.

			bool tryLock() const;
			/// Tries to lock the object. Returns false immediately
			/// if the object is already locked by another thread
			/// Returns true if the object was successfully locked.

			void unlock() const;
			/// Unlocks the object so that it can be locked by
			/// other threads.

			void notify() const;
			/// Signals the object. 
			/// Exactly only one thread waiting for the object 
			/// can resume execution.

			void wait() const;
			/// Waits for the object to become signalled.

			void wait(long milliseconds) const;
			/// Waits for the object to become signalled.
			/// Throws a TimeoutException if the object
			/// does not become signalled within the specified
			/// time interval.

			bool tryWait(long milliseconds) const;
			/// Waits for the object to become signalled.
			/// Returns true if the object
			/// became signalled within the specified
			/// time interval, false otherwise.

		private:
			mutable Mutex _mutex;
			mutable Event _event;
		};

		//
		inline void SynchronizedObject::lock() const
		{
			_mutex.lock();
		}


		inline bool SynchronizedObject::tryLock() const
		{
			return _mutex.tryLock();
		}


		inline void SynchronizedObject::unlock() const
		{
			_mutex.unlock();
		}


		inline void SynchronizedObject::notify() const
		{
			_event.set();
		}


		inline void SynchronizedObject::wait() const
		{
			_event.wait();
		}


		inline void SynchronizedObject::wait(long milliseconds) const
		{
			_event.wait(milliseconds);
		}


		inline bool SynchronizedObject::tryWait(long milliseconds) const
		{
			return _event.tryWait(milliseconds);
		}
	}
}
#endif
