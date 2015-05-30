/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RunnableAdapter.h
*  Purpose :       RunnableAdapter class.
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

#ifndef _RUNNABLEADAPTER_H
#define _RUNNABLEADAPTER_H

#include "GlobalThreading.h"

#include "Runnable.h"

namespace Nequeo {
	namespace Threading
	{
		/// This adapter simplifies using ordinary methods as
		/// targets for threads.
		template <class C>
		class RunnableAdapter : public Runnable
			/// Usage:
			///    RunnableAdapter<MyClass> ra(myObject, &MyObject::doSomething));
			///    Thread thr;
			///    thr.Start(ra);
			///
			/// For using a freestanding or static member function as a thread
			/// target, please see the ThreadTarget class.
		{
		public:
			typedef void (C::*Callback)();

			RunnableAdapter(C& object, Callback method) : _pObject(&object), _method(method)
			{
			}

			RunnableAdapter(const RunnableAdapter& ra) : _pObject(ra._pObject), _method(ra._method)
			{
			}

			~RunnableAdapter()
			{
			}

			RunnableAdapter& operator = (const RunnableAdapter& ra)
			{
				_pObject = ra._pObject;
				_method = ra._method;
				return *this;
			}

			void run()
			{
				(_pObject->*_method)();
			}

		private:
			RunnableAdapter();

			C*       _pObject;
			Callback _method;
		};
	}
}
#endif
