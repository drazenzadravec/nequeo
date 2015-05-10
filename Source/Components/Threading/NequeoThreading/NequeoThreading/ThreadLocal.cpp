/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TLSAbstractSlot.cpp
*  Purpose :       TLSAbstractSlot class.
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

#include "ThreadLocal.h"
#include "SingletonHolder.h"
#include "Thread.h"

namespace Nequeo {
	namespace Threading
	{
		TLSAbstractSlot::TLSAbstractSlot()
		{
		}


		TLSAbstractSlot::~TLSAbstractSlot()
		{
		}


		ThreadLocalStorage::ThreadLocalStorage()
		{
		}


		ThreadLocalStorage::~ThreadLocalStorage()
		{
			for (TLSMap::iterator it = _map.begin(); it != _map.end(); ++it)
			{
				delete it->second;
			}
		}


		TLSAbstractSlot*& ThreadLocalStorage::get(const void* key)
		{
			TLSMap::iterator it = _map.find(key);
			if (it == _map.end())
				return _map.insert(TLSMap::value_type(key, reinterpret_cast<TLSAbstractSlot*>(0))).first->second;
			else
				return it->second;
		}


		namespace
		{
			static SingletonHolder<ThreadLocalStorage> sh;
		}


		ThreadLocalStorage& ThreadLocalStorage::current()
		{
			Thread* pThread = Thread::current();
			if (pThread)
			{
				return pThread->tls();
			}
			else
			{
				return *sh.get();
			}
		}


		void ThreadLocalStorage::clear()
		{
			Thread* pThread = Thread::current();
			if (pThread)
				pThread->clearTLS();
		}
	}
}