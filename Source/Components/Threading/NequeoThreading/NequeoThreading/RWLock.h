/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RWLock.h
*  Purpose :       RWLock class.
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

#ifndef _RWLOCK_H
#define _RWLOCK_H

#include "GlobalThreading.h"

#include "Exceptions\Exception.h"
#include "RWLock_WIN32.h"

namespace Nequeo {
	namespace Threading
	{
		class ScopedRWLock;
		class ScopedReadRWLock;
		class ScopedWriteRWLock;

		/// A reader writer lock allows multiple concurrent
		/// readers or one exclusive writer.
		class RWLock : private RWLockImpl
		{
		public:
			typedef ScopedRWLock ScopedLock;
			typedef ScopedReadRWLock ScopedReadLock;
			typedef ScopedWriteRWLock ScopedWriteLock;

			RWLock();
			/// Creates the Reader/Writer lock.

			~RWLock();
			/// Destroys the Reader/Writer lock.

			void readLock();
			/// Acquires a read lock. If another thread currently holds a write lock,
			/// waits until the write lock is released.

			bool tryReadLock();
			/// Tries to acquire a read lock. Immediately returns true if successful, or
			/// false if another thread currently holds a write lock.

			void writeLock();
			/// Acquires a write lock. If one or more other threads currently hold 
			/// locks, waits until all locks are released. The results are undefined
			/// if the same thread already holds a read or write lock

			bool tryWriteLock();
			/// Tries to acquire a write lock. Immediately returns true if successful,
			/// or false if one or more other threads currently hold 
			/// locks. The result is undefined if the same thread already
			/// holds a read or write lock.

			void unlock();
			/// Releases the read or write lock.

		private:
			RWLock(const RWLock&);
			RWLock& operator = (const RWLock&);
		};


		class ScopedRWLock
			/// A variant of ScopedLock for reader/writer locks.
		{
		public:
			ScopedRWLock(RWLock& rwl, bool write = false);
			~ScopedRWLock();

		private:
			RWLock& _rwl;

			ScopedRWLock();
			ScopedRWLock(const ScopedRWLock&);
			ScopedRWLock& operator = (const ScopedRWLock&);
		};


		class ScopedReadRWLock : public ScopedRWLock
			/// A variant of ScopedLock for reader locks.
		{
		public:
			ScopedReadRWLock(RWLock& rwl);
			~ScopedReadRWLock();
		};


		class ScopedWriteRWLock : public ScopedRWLock
			/// A variant of ScopedLock for writer locks.
		{
		public:
			ScopedWriteRWLock(RWLock& rwl);
			~ScopedWriteRWLock();
		};


		//
		// inlines
		//
		inline void RWLock::readLock()
		{
			readLockImpl();
		}


		inline bool RWLock::tryReadLock()
		{
			return tryReadLockImpl();
		}


		inline void RWLock::writeLock()
		{
			writeLockImpl();
		}


		inline bool RWLock::tryWriteLock()
		{
			return tryWriteLockImpl();
		}


		inline void RWLock::unlock()
		{
			unlockImpl();
		}


		inline ScopedRWLock::ScopedRWLock(RWLock& rwl, bool write) : _rwl(rwl)
		{
			if (write)
				_rwl.writeLock();
			else
				_rwl.readLock();
		}


		inline ScopedRWLock::~ScopedRWLock()
		{
			_rwl.unlock();
		}


		inline ScopedReadRWLock::ScopedReadRWLock(RWLock& rwl) : ScopedRWLock(rwl, false)
		{
		}


		inline ScopedReadRWLock::~ScopedReadRWLock()
		{
		}


		inline ScopedWriteRWLock::ScopedWriteRWLock(RWLock& rwl) : ScopedRWLock(rwl, true)
		{
		}


		inline ScopedWriteRWLock::~ScopedWriteRWLock()
		{
		}
	}
}
#endif
