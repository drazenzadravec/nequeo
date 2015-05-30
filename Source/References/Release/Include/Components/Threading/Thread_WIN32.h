/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ThreadImpl.h
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

#pragma once

#ifndef _THREADIMPL_H
#define _THREADIMPL_H

#include "GlobalThreading.h"

#include "Runnable.h"
#include "Priority.h"
#include "Base\UnWindows.h"
#include "Exceptions\ExceptionCode.h"

namespace Nequeo {
	namespace Threading
	{
		class ThreadImpl
		{
		public:
			typedef DWORD TIDImpl;
			typedef void(*Callable)(void*);

#if defined(_DLL)
			typedef DWORD(WINAPI *Entry)(LPVOID);
#else
			typedef unsigned (__stdcall *Entry)(void*);
#endif

			struct CallbackData
			{
				CallbackData() : callback(0), pData(0)
				{
				}

				Callable  callback;
				void*     pData;
			};

			ThreadImpl();
			~ThreadImpl();

			TIDImpl tidImpl() const;
			void setPriorityImpl(int prio);
			int getPriorityImpl() const;
			void setOSPriorityImpl(int prio, int policy = 0);
			int getOSPriorityImpl() const;
			static int getMinOSPriorityImpl(int policy);
			static int getMaxOSPriorityImpl(int policy);
			void setStackSizeImpl(int size);
			int getStackSizeImpl() const;
			void startImpl(Runnable& target);
			void startImpl(Callable target, void* pData = 0);

			void joinImpl();
			bool joinImpl(long milliseconds);
			bool isRunningImpl() const;
			static void sleepImpl(long milliseconds);
			static void yieldImpl();
			static ThreadImpl* currentImpl();
			static TIDImpl currentTidImpl();

		protected:
#if defined(_DLL)
			static DWORD WINAPI runnableEntry(LPVOID pThread);
#else
			static unsigned __stdcall runnableEntry(void* pThread);
#endif

#if defined(_DLL)
			static DWORD WINAPI callableEntry(LPVOID pThread);
#else
			static unsigned __stdcall callableEntry(void* pThread);
#endif

			void createImpl(Entry ent, void* pData);
			void threadCleanup();

		private:
			class CurrentThreadHolder
			{
			public:
				CurrentThreadHolder() : _slot(TlsAlloc())
				{
					if (_slot == TLS_OUT_OF_INDEXES)
						throw Nequeo::Exceptions::SystemException("cannot allocate thread context key");
				}
				~CurrentThreadHolder()
				{
					TlsFree(_slot);
				}
				ThreadImpl* get() const
				{
					return reinterpret_cast<ThreadImpl*>(TlsGetValue(_slot));
				}
				void set(ThreadImpl* pThread)
				{
					TlsSetValue(_slot, pThread);
				}

			private:
				DWORD _slot;
			};

			Runnable*    _pRunnableTarget;
			CallbackData _callbackTarget;
			HANDLE       _thread;
			DWORD        _threadId;
			int          _prio;
			int          _stackSize;

			static CurrentThreadHolder _currentThreadHolder;
		};


		//
		// inlines
		//
		inline int ThreadImpl::getPriorityImpl() const
		{
			return _prio;
		}


		inline int ThreadImpl::getOSPriorityImpl() const
		{
			return _prio;
		}


		inline int ThreadImpl::getMinOSPriorityImpl(int /* policy */)
		{
			return PRIO_LOWEST;
		}


		inline int ThreadImpl::getMaxOSPriorityImpl(int /* policy */)
		{
			return PRIO_HIGHEST;
		}


		inline void ThreadImpl::sleepImpl(long milliseconds)
		{
			Sleep(DWORD(milliseconds));
		}


		inline void ThreadImpl::yieldImpl()
		{
			Sleep(0);
		}


		inline void ThreadImpl::setStackSizeImpl(int size)
		{
			_stackSize = size;
		}


		inline int ThreadImpl::getStackSizeImpl() const
		{
			return _stackSize;
		}


		inline ThreadImpl::TIDImpl ThreadImpl::tidImpl() const
		{
			return _threadId;
		}
	}
}
#endif
