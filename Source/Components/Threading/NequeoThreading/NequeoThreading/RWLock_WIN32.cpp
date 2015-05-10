/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RWLockImpl.cpp
*  Purpose :       RWLockImpl class.
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

#include "RWLock_WIN32.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

using Nequeo::Exceptions::SystemException;

namespace Nequeo {
	namespace Threading
	{
		RWLockImpl::RWLockImpl() : _readers(0), _writersWaiting(0), _writers(0)
		{
			_mutex = CreateMutexW(NULL, FALSE, NULL);
			if (_mutex == NULL)
				throw SystemException("cannot create reader/writer lock");

			_readEvent = CreateEventW(NULL, TRUE, TRUE, NULL);
			if (_readEvent == NULL)
				throw SystemException("cannot create reader/writer lock");

			_writeEvent = CreateEventW(NULL, TRUE, TRUE, NULL);
			if (_writeEvent == NULL)
				throw SystemException("cannot create reader/writer lock");
		}


		RWLockImpl::~RWLockImpl()
		{
			CloseHandle(_mutex);
			CloseHandle(_readEvent);
			CloseHandle(_writeEvent);
		}


		inline void RWLockImpl::addWriter()
		{
			switch (WaitForSingleObject(_mutex, INFINITE))
			{
			case WAIT_OBJECT_0:
				if (++_writersWaiting == 1) ResetEvent(_readEvent);
				ReleaseMutex(_mutex);
				break;
			default:
				throw SystemException("cannot lock reader/writer lock");
			}
		}


		inline void RWLockImpl::removeWriter()
		{
			switch (WaitForSingleObject(_mutex, INFINITE))
			{
			case WAIT_OBJECT_0:
				if (--_writersWaiting == 0 && _writers == 0) SetEvent(_readEvent);
				ReleaseMutex(_mutex);
				break;
			default:
				throw SystemException("cannot lock reader/writer lock");
			}
		}


		void RWLockImpl::readLockImpl()
		{
			HANDLE h[2];
			h[0] = _mutex;
			h[1] = _readEvent;
			switch (WaitForMultipleObjects(2, h, TRUE, INFINITE))
			{
			case WAIT_OBJECT_0:
			case WAIT_OBJECT_0 + 1:
				++_readers;
				ResetEvent(_writeEvent);
				ReleaseMutex(_mutex);
				break;
			default:
				throw SystemException("cannot lock reader/writer lock");
			}
		}


		bool RWLockImpl::tryReadLockImpl()
		{
			for (;;)
			{
				if (_writers != 0 || _writersWaiting != 0)
					return false;

				DWORD result = tryReadLockOnce();
				switch (result)
				{
				case WAIT_OBJECT_0:
				case WAIT_OBJECT_0 + 1:
					return true;
				case WAIT_TIMEOUT:
					continue; // try again
				default:
					throw SystemException("cannot lock reader/writer lock");
				}
			}
		}


		void RWLockImpl::writeLockImpl()
		{
			addWriter();
			HANDLE h[2];
			h[0] = _mutex;
			h[1] = _writeEvent;
			switch (WaitForMultipleObjects(2, h, TRUE, INFINITE))
			{
			case WAIT_OBJECT_0:
			case WAIT_OBJECT_0 + 1:
				--_writersWaiting;
				++_readers;
				++_writers;
				ResetEvent(_readEvent);
				ResetEvent(_writeEvent);
				ReleaseMutex(_mutex);
				break;
			default:
				removeWriter();
				throw SystemException("cannot lock reader/writer lock");
			}
		}


		bool RWLockImpl::tryWriteLockImpl()
		{
			addWriter();
			HANDLE h[2];
			h[0] = _mutex;
			h[1] = _writeEvent;
			switch (WaitForMultipleObjects(2, h, TRUE, 1))
			{
			case WAIT_OBJECT_0:
			case WAIT_OBJECT_0 + 1:
				--_writersWaiting;
				++_readers;
				++_writers;
				ResetEvent(_readEvent);
				ResetEvent(_writeEvent);
				ReleaseMutex(_mutex);
				return true;
			case WAIT_TIMEOUT:
				removeWriter();
				return false;
			default:
				removeWriter();
				throw SystemException("cannot lock reader/writer lock");
			}
		}


		void RWLockImpl::unlockImpl()
		{
			switch (WaitForSingleObject(_mutex, INFINITE))
			{
			case WAIT_OBJECT_0:
				_writers = 0;
				if (_writersWaiting == 0) SetEvent(_readEvent);
				if (--_readers == 0) SetEvent(_writeEvent);
				ReleaseMutex(_mutex);
				break;
			default:
				throw SystemException("cannot unlock reader/writer lock");
			}
		}


		DWORD RWLockImpl::tryReadLockOnce()
		{
			HANDLE h[2];
			h[0] = _mutex;
			h[1] = _readEvent;
			DWORD result = WaitForMultipleObjects(2, h, TRUE, 1);
			switch (result)
			{
			case WAIT_OBJECT_0:
			case WAIT_OBJECT_0 + 1:
				++_readers;
				ResetEvent(_writeEvent);
				ReleaseMutex(_mutex);
				return result;
			case WAIT_TIMEOUT:
				return result;
			default:
				throw SystemException("cannot lock reader/writer lock");
			}
		}
	}
}