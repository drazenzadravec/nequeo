/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Thread.cpp
*  Purpose :       Thread class.
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

#include "Thread.h"
#include "Mutex.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"
#include "ThreadLocal.h"
#include <sstream>

namespace Nequeo {
	namespace Threading
	{
		Thread::Thread() :
			_id(uniqueId()),
			_name(makeName()),
			_pTLS(0)
		{
		}


		Thread::Thread(const std::string& name) :
			_id(uniqueId()),
			_name(name),
			_pTLS(0)
		{
		}


		Thread::~Thread()
		{
			delete _pTLS;
		}


		void Thread::setPriority(Priority prio)
		{
			setPriorityImpl(prio);
		}


		Nequeo::Threading::Priority Thread::getPriority() const
		{
			return Priority(getPriorityImpl());
		}


		void Thread::start(Runnable& target)
		{
			startImpl(target);
		}


		void Thread::start(Callable target, void* pData)
		{
			startImpl(target, pData);
		}


		void Thread::join()
		{
			joinImpl();
		}


		void Thread::join(long milliseconds)
		{
			if (!joinImpl(milliseconds))
				throw Nequeo::Exceptions::TimeoutException();
		}


		bool Thread::tryJoin(long milliseconds)
		{
			return joinImpl(milliseconds);
		}


		ThreadLocalStorage& Thread::tls()
		{
			if (!_pTLS)
				_pTLS = new ThreadLocalStorage;
			return *_pTLS;
		}


		void Thread::clearTLS()
		{
			if (_pTLS)
			{
				delete _pTLS;
				_pTLS = 0;
			}
		}


		std::string Thread::makeName()
		{
			std::ostringstream name;
			name << '#' << _id;
			return name.str();
		}


		namespace
		{
			static FastMutex uniqueIdMutex;
		}


		int Thread::uniqueId()
		{
			FastMutex::ScopedLock lock(uniqueIdMutex);

			static unsigned count = 0;
			++count;
			return count;
		}


		void Thread::setName(const std::string& name)
		{
			FastMutex::ScopedLock lock(_mutex);

			_name = name;
		}
	}
}