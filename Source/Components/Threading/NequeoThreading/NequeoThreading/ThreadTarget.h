/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ThreadTarget.h
*  Purpose :       ThreadTarget class.
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

#ifndef _THREADTARGET_H
#define _THREADTARGET_H

#include "GlobalThreading.h"

#include "Runnable.h"

namespace Nequeo {
	namespace Threading
	{
		/// This adapter simplifies using static member functions as well as 
		/// standalone functions as targets for threads.
		/// Note that it is possible to pass those entities directly to Thread::start().
		/// This adapter is provided as a convenience for higher abstraction level
		/// scenarios where Runnable abstract class is used.
		///
		/// For using a non-static member function as a thread target, please
		/// see the RunnableAdapter class.
		class ThreadTarget : public Runnable
			/// 
			/// Usage:
			///    class MyObject
			///    {
			///        static void doSomething() {}
			///    };
			///    ThreadTarget ra(&MyObject::doSomething);
			///    Thread thr;
			///    thr.start(ra);
			///
			/// or:
			/// 
			///    void doSomething() {}
			/// 
			///    ThreadTarget ra(doSomething);
			///    Thread thr;
			///    thr.start(ra);
		{
		public:
			typedef void(*Callback)();

			ThreadTarget(Callback method);

			ThreadTarget(const ThreadTarget& te);

			~ThreadTarget();

			ThreadTarget& operator = (const ThreadTarget& te);

			void run();

		private:
			ThreadTarget();

			Callback _method;
		};

		///
		inline void ThreadTarget::run()
		{
			_method();
		}
	}
}
#endif
