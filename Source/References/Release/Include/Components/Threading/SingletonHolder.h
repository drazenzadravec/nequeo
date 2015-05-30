/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          SingletonHolder.h
*  Purpose :       SingletonHolder class.
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

#ifndef _SINGLETONHOLDER_H
#define _SINGLETONHOLDER_H

#include "GlobalThreading.h"

#include "Mutex.h"

namespace Nequeo {
	namespace Threading
	{
		/// This is a helper template class for managing
		/// singleton objects allocated on the heap.
		/// The class ensures proper deletion (including
		/// calling of the destructor) of singleton objects
		/// when the application that created them terminates.
		template <class S>
		class SingletonHolder
		{
		public:
			SingletonHolder() :
				_pS(0)
				/// Creates the SingletonHolder.
			{
			}

			~SingletonHolder()
				/// Destroys the SingletonHolder and the singleton
				/// object that it holds.
			{
				delete _pS;
			}

			S* get()
				/// Returns a pointer to the singleton object
				/// hold by the SingletonHolder. The first call
				/// to get will create the singleton.
			{
				FastMutex::ScopedLock lock(_m);
				if (!_pS) _pS = new S;
				return _pS;
			}

		private:
			S* _pS;
			FastMutex _m;
		};
	}
}
#endif