/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ThreadImpl.cpp
*  Purpose :       ThreadImpl class.
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

#include "Thread_WIN32.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"
#include "ErrorHandler.h"
#include <process.h>

using Nequeo::Exceptions::SystemException;

namespace Nequeo {
	namespace Threading
	{
		ThreadImpl::CurrentThreadHolder ThreadImpl::_currentThreadHolder;


		ThreadImpl::ThreadImpl() :
			_pRunnableTarget(0),
			_thread(0),
			_threadId(0),
			_prio(PRIO_NORMAL),
			_stackSize(NEQUEO_THREAD_STACK_SIZE)
		{
		}


		ThreadImpl::~ThreadImpl()
		{
			if (_thread) CloseHandle(_thread);
		}


		void ThreadImpl::setPriorityImpl(int prio)
		{
			if (prio != _prio)
			{
				_prio = prio;
				if (_thread)
				{
					if (SetThreadPriority(_thread, _prio) == 0)
						throw SystemException("cannot set thread priority");
				}
			}
		}


		void ThreadImpl::setOSPriorityImpl(int prio, int /* policy */)
		{
			setPriorityImpl(prio);
		}


		void ThreadImpl::startImpl(Runnable& target)
		{
			if (isRunningImpl())
				throw SystemException("thread already running");

			_pRunnableTarget = &target;

			createImpl(runnableEntry, this);
		}


		void ThreadImpl::startImpl(Callable target, void* pData)
		{
			if (isRunningImpl())
				throw SystemException("thread already running");

			threadCleanup();
			_callbackTarget.callback = target;
			_callbackTarget.pData = pData;

			createImpl(callableEntry, this);
		}


		void ThreadImpl::createImpl(Entry ent, void* pData)
		{
#if defined(_DLL)
			_thread = CreateThread(NULL, _stackSize, ent, pData, 0, &_threadId);
#else
			unsigned threadId;
			_thread = (HANDLE) _beginthreadex(NULL, _stackSize, ent, this, 0, &threadId);
			_threadId = static_cast<DWORD>(threadId);
#endif
			if (!_thread)
				throw SystemException("cannot create thread");
			if (_prio != PRIO_NORMAL && !SetThreadPriority(_thread, _prio))
				throw SystemException("cannot set thread priority");
		}


		void ThreadImpl::joinImpl()
		{
			if (!_thread) return;

			switch (WaitForSingleObject(_thread, INFINITE))
			{
			case WAIT_OBJECT_0:
				threadCleanup();
				return;
			default:
				throw SystemException("cannot join thread");
			}
		}


		bool ThreadImpl::joinImpl(long milliseconds)
		{
			if (!_thread) return true;

			switch (WaitForSingleObject(_thread, milliseconds + 1))
			{
			case WAIT_TIMEOUT:
				return false;
			case WAIT_OBJECT_0:
				threadCleanup();
				return true;
			default:
				throw SystemException("cannot join thread");
			}
		}


		bool ThreadImpl::isRunningImpl() const
		{
			if (_thread)
			{
				DWORD ec = 0;
				return GetExitCodeThread(_thread, &ec) && ec == STILL_ACTIVE;
			}
			return false;
		}


		void ThreadImpl::threadCleanup()
		{
			if (!_thread) return;
			if (CloseHandle(_thread)) _thread = 0;
		}


		ThreadImpl* ThreadImpl::currentImpl()
		{
			return _currentThreadHolder.get();
		}


		ThreadImpl::TIDImpl ThreadImpl::currentTidImpl()
		{
			return GetCurrentThreadId();
		}


#if defined(_DLL)
		DWORD WINAPI ThreadImpl::runnableEntry(LPVOID pThread)
#else
		unsigned __stdcall ThreadImpl::runnableEntry(void* pThread)
#endif
		{
			_currentThreadHolder.set(reinterpret_cast<ThreadImpl*>(pThread));
#if defined(_DEBUG) && defined(POCO_WIN32_DEBUGGER_THREAD_NAMES)
			setThreadName(-1, reinterpret_cast<Thread*>(pThread)->getName().c_str());
#endif
			try
			{
				reinterpret_cast<ThreadImpl*>(pThread)->_pRunnableTarget->run();
			}
			catch (Exception& exc)
			{
				ErrorHandler::handle(exc);
			}
			catch (std::exception& exc)
			{
				ErrorHandler::handle(exc);
			}
			catch (...)
			{
				ErrorHandler::handle();
			}
			return 0;
		}


#if defined(_DLL)
		DWORD WINAPI ThreadImpl::callableEntry(LPVOID pThread)
#else
		unsigned __stdcall ThreadImpl::callableEntry(void* pThread)
#endif
		{
			_currentThreadHolder.set(reinterpret_cast<ThreadImpl*>(pThread));
#if defined(_DEBUG) && defined(POCO_WIN32_DEBUGGER_THREAD_NAMES)
			setThreadName(-1, reinterpret_cast<Thread*>(pThread)->getName().c_str());
#endif
			try
			{
				ThreadImpl* pTI = reinterpret_cast<ThreadImpl*>(pThread);
				pTI->_callbackTarget.callback(pTI->_callbackTarget.pData);
			}
			catch (Exception& exc)
			{
				ErrorHandler::handle(exc);
			}
			catch (std::exception& exc)
			{
				ErrorHandler::handle(exc);
			}
			catch (...)
			{
				ErrorHandler::handle();
			}
			return 0;
		}
	}
}