/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          FIFOEvent.h
*  Purpose :       FIFOEvent class.
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

#ifndef _FIFOEVENT_H
#define _FIFOEVENT_H

#include "GlobalThreading.h"

#include "AbstractEvent.h"
#include "FIFOStrategy.h"
#include "AbstractDelegate.h"

namespace Nequeo {
	namespace Threading {
		namespace Events
		{
			/// A FIFOEvent uses internally a FIFOStrategy which guarantees
			/// that delegates are invoked in the order they were added to
			/// the event.
			///
			/// Note that as of release 1.4.2, this is the default behavior
			/// implemented by BasicEvent, so this class is provided
			/// for backwards compatibility only.
			template <class TArgs, class TMutex = FastMutex>
			class FIFOEvent : public AbstractEvent <
				TArgs,
				FIFOStrategy<TArgs, AbstractDelegate<TArgs> >,
				AbstractDelegate<TArgs>,
				TMutex >

			{
			public:
				FIFOEvent()
				{
				}

				~FIFOEvent()
				{
				}

			private:
				FIFOEvent(const FIFOEvent& e);
				FIFOEvent& operator = (const FIFOEvent& e);
			};
		}
	}
}
#endif
