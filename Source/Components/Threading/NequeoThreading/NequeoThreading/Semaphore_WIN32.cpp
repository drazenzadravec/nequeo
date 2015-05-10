/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          SemaphoreImpl.cpp
*  Purpose :       SemaphoreImpl class.
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

#include "Semaphore_WIN32.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

using Nequeo::Exceptions::SystemException;

namespace Nequeo {
	namespace Threading
	{
		SemaphoreImpl::SemaphoreImpl(int n, int max)
		{
			if (n >= 0 && max > 0 && n <= max)
			{
				_sema = CreateSemaphoreW(NULL, n, max, NULL);
				if (!_sema)
				{
					throw SystemException("cannot create semaphore");
				}
			}
		}


		SemaphoreImpl::~SemaphoreImpl()
		{
			CloseHandle(_sema);
		}


		void SemaphoreImpl::waitImpl()
		{
			switch (WaitForSingleObject(_sema, INFINITE))
			{
			case WAIT_OBJECT_0:
				return;
			default:
				throw SystemException("wait for semaphore failed");
			}
		}


		bool SemaphoreImpl::waitImpl(long milliseconds)
		{
			switch (WaitForSingleObject(_sema, milliseconds + 1))
			{
			case WAIT_TIMEOUT:
				return false;
			case WAIT_OBJECT_0:
				return true;
			default:
				throw SystemException("wait for semaphore failed");
			}
		}
	}
}