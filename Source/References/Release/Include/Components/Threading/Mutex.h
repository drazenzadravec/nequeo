/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Mutex.h
*  Purpose :       Mutex class.
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

#ifndef _MUTEX_H
#define _MUTEX_H

#include "GlobalThreading.h"

#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"
#include "ScopedLock.h"
#include "Mutex_WIN32.h"

namespace Nequeo {
	namespace Threading
	{
		/// A Mutex (mutual exclusion) is a synchronization 
		/// mechanism used to control access to a shared resource
		/// in a concurrent (multithreaded) scenario.
		/// Mutexes are recursive, that is, the same mutex can be 
		/// locked multiple times by the same thread (but, of course,
		/// not by other threads).
		/// Using the ScopedLock class is the preferred way to automatically
		/// lock and unlock a mutex.
		class Mutex : private MutexImpl
		{
		public:
			typedef Nequeo::Threading::ScopedLock<Mutex> ScopedLock;

			Mutex();
			/// creates the Mutex.

			~Mutex();
			/// destroys the Mutex.

			void lock();
			/// Locks the mutex. Blocks if the mutex
			/// is held by another thread.

			void lock(long milliseconds);
			/// Locks the mutex. Blocks up to the given number of milliseconds
			/// if the mutex is held by another thread. Throws a TimeoutException
			/// if the mutex can not be locked within the given timeout.
			///
			/// Performance Note: On most platforms (including Windows), this member function is 
			/// implemented using a loop calling (the equivalent of) tryLock() and Thread::sleep().
			/// On POSIX platforms that support pthread_mutex_timedlock(), this is used.

			bool tryLock();
			/// Tries to lock the mutex. Returns false immediately
			/// if the mutex is already held by another thread.
			/// Returns true if the mutex was successfully locked.

			bool tryLock(long milliseconds);
			/// Locks the mutex. Blocks up to the given number of milliseconds
			/// if the mutex is held by another thread.
			/// Returns true if the mutex was successfully locked.
			///
			/// Performance Note: On most platforms (including Windows), this member function is 
			/// implemented using a loop calling (the equivalent of) tryLock() and Thread::sleep().
			/// On POSIX platforms that support pthread_mutex_timedlock(), this is used.

			void unlock();
			/// Unlocks the mutex so that it can be acquired by
			/// other threads.

		private:
			Mutex(const Mutex&);
			Mutex& operator = (const Mutex&);
		};


		class FastMutex : private FastMutexImpl
			/// A FastMutex (mutual exclusion) is similar to a Mutex.
			/// Unlike a Mutex, however, a FastMutex is not recursive,
			/// which means that a deadlock will occur if the same
			/// thread tries to lock a mutex it has already locked again.
			/// Locking a FastMutex is faster than locking a recursive Mutex.
			/// Using the ScopedLock class is the preferred way to automatically
			/// lock and unlock a mutex.
		{
		public:
			typedef Nequeo::Threading::ScopedLock<FastMutex> ScopedLock;

			FastMutex();
			/// creates the Mutex.

			~FastMutex();
			/// destroys the Mutex.

			void lock();
			/// Locks the mutex. Blocks if the mutex
			/// is held by another thread.

			void lock(long milliseconds);
			/// Locks the mutex. Blocks up to the given number of milliseconds
			/// if the mutex is held by another thread. Throws a TimeoutException
			/// if the mutex can not be locked within the given timeout.
			///
			/// Performance Note: On most platforms (including Windows), this member function is 
			/// implemented using a loop calling (the equivalent of) tryLock() and Thread::sleep().
			/// On POSIX platforms that support pthread_mutex_timedlock(), this is used.

			bool tryLock();
			/// Tries to lock the mutex. Returns false immediately
			/// if the mutex is already held by another thread.
			/// Returns true if the mutex was successfully locked.

			bool tryLock(long milliseconds);
			/// Locks the mutex. Blocks up to the given number of milliseconds
			/// if the mutex is held by another thread.
			/// Returns true if the mutex was successfully locked.
			///
			/// Performance Note: On most platforms (including Windows), this member function is 
			/// implemented using a loop calling (the equivalent of) tryLock() and Thread::sleep().
			/// On POSIX platforms that support pthread_mutex_timedlock(), this is used.

			void unlock();
			/// Unlocks the mutex so that it can be acquired by
			/// other threads.

		private:
			FastMutex(const FastMutex&);
			FastMutex& operator = (const FastMutex&);
		};


		class NullMutex
			/// A NullMutex is an simple mutex implementation
			/// which performs no locking at all. Useful in policy driven design
			/// where the type of mutex used can be now a template parameter allowing 
			/// to switch between thread-safe and not thread-safe implementations.
		{
		public:
			typedef Nequeo::Threading::ScopedLock<NullMutex> ScopedLock;

			NullMutex()
				/// Creates the NullMutex.
			{
			}

			~NullMutex()
				/// Destroys the NullMutex.
			{
			}

			void lock()
				/// Does nothing.
			{
			}

			void lock(long)
				/// Does nothing.
			{
			}

			bool tryLock()
				/// Does nothing and always returns true.
			{
				return true;
			}

			bool tryLock(long)
				/// Does nothing and always returns true.
			{
				return true;
			}

			void unlock()
				/// Does nothing.
			{
			}
		};


		//
		// inlines
		//
		inline void Mutex::lock()
		{
			lockImpl();
		}


		inline void Mutex::lock(long milliseconds)
		{
			if (!tryLockImpl(milliseconds))
				throw Nequeo::Exceptions::TimeoutException();
		}


		inline bool Mutex::tryLock()
		{
			return tryLockImpl();
		}


		inline bool Mutex::tryLock(long milliseconds)
		{
			return tryLockImpl(milliseconds);
		}


		inline void Mutex::unlock()
		{
			unlockImpl();
		}


		inline void FastMutex::lock()
		{
			lockImpl();
		}


		inline void FastMutex::lock(long milliseconds)
		{
			if (!tryLockImpl(milliseconds))
				throw Nequeo::Exceptions::TimeoutException();
		}


		inline bool FastMutex::tryLock()
		{
			return tryLockImpl();
		}


		inline bool FastMutex::tryLock(long milliseconds)
		{
			return tryLockImpl(milliseconds);
		}


		inline void FastMutex::unlock()
		{
			unlockImpl();
		}
	}
}
#endif
