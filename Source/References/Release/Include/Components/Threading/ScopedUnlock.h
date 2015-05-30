/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ScopedUnlock.h
*  Purpose :       ScopedUnlock class.
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

#ifndef _SCOPEDUNLOCK_H
#define _SCOPEDUNLOCK_H

#include "GlobalThreading.h"

namespace Nequeo {
	namespace Threading
	{
		/// A class that simplifies thread synchronization
		/// with a mutex.
		/// The constructor accepts a Mutex and unlocks it.
		/// The destructor locks the mutex.
		template <class M>
		class ScopedUnlock
		{
		public:
			inline ScopedUnlock(M& mutex, bool unlockNow = true) : _mutex(mutex)
			{
				if (unlockNow)
					_mutex.unlock();
			}
			inline ~ScopedUnlock()
			{
				_mutex.lock();
			}

		private:
			M& _mutex;

			ScopedUnlock();
			ScopedUnlock(const ScopedUnlock&);
			ScopedUnlock& operator = (const ScopedUnlock&);
		};
	}
}
#endif
