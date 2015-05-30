/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RWLockImpl.h
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

#pragma once

#ifndef _RWLOCKIMPL_H
#define _RWLOCKIMPL_H

#include "GlobalThreading.h"

#include "Exceptions\Exception.h"
#include "Base\UnWindows.h"

namespace Nequeo {
	namespace Threading
	{
		class RWLockImpl
		{
		protected:
			RWLockImpl();
			~RWLockImpl();
			void readLockImpl();
			bool tryReadLockImpl();
			void writeLockImpl();
			bool tryWriteLockImpl();
			void unlockImpl();

		private:
			void addWriter();
			void removeWriter();
			DWORD tryReadLockOnce();

			HANDLE   _mutex;
			HANDLE   _readEvent;
			HANDLE   _writeEvent;
			unsigned _readers;
			unsigned _writersWaiting;
			unsigned _writers;
		};
	}
}
#endif
