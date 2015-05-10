/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          MutexImpl.cpp
*  Purpose :       MutexImpl class.
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

#include "Mutex_WIN32.h"
#include "Base\Timestamp.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

using Nequeo::Timestamp;

namespace Nequeo {
	namespace Threading
	{
		MutexImpl::MutexImpl()
		{
			// the fct has a boolean return value under WInnNt/2000/XP but not on Win98
			// the return only checks if the input address of &_cs was valid, so it is safe to omit it
			InitializeCriticalSectionAndSpinCount(&_cs, 4000);
		}


		MutexImpl::~MutexImpl()
		{
			DeleteCriticalSection(&_cs);
		}


		bool MutexImpl::tryLockImpl(long milliseconds)
		{
			const int sleepMillis = 5;
			Timestamp now;
			Timestamp::TimeDiff diff(Timestamp::TimeDiff(milliseconds) * 1000);
			do
			{
				try
				{
					if (TryEnterCriticalSection(&_cs) == TRUE)
						return true;
				}
				catch (...)
				{
					throw Nequeo::Exceptions::SystemException("cannot lock mutex");
				}
				Sleep(sleepMillis);
			} while (!now.isElapsed(diff));
			return false;
		}
	}
}