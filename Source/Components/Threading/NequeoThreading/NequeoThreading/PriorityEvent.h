/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          PriorityEvent.h
*  Purpose :       PriorityEvent class.
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

#ifndef _PRIORITYEVENT_H
#define _PRIORITYEVENT_H

#include "GlobalThreading.h"

#include "AbstractEvent.h"
#include "PriorityStrategy.h"
#include "AbstractPriorityDelegate.h"

namespace Nequeo {
	namespace Threading {
		namespace Events
		{
			/// A PriorityEvent uses internally a PriorityStrategy which 
			/// invokes delegates in order of priority (lower priorities first).
			/// PriorityEvent's can only be used together with PriorityDelegate's.
			/// PriorityDelegate's are sorted according to the priority value, when
			/// two delegates have the same priority, they are invoked in
			/// an arbitrary manner.
			template <class TArgs, class TMutex = FastMutex>
			class PriorityEvent : public AbstractEvent <
				TArgs,
				PriorityStrategy<TArgs, AbstractPriorityDelegate<TArgs> >,
				AbstractPriorityDelegate<TArgs>,
				TMutex >
			{
			public:
				PriorityEvent()
				{
				}

				~PriorityEvent()
				{
				}

			private:
				PriorityEvent(const PriorityEvent&);
				PriorityEvent& operator = (const PriorityEvent&);
			};
		}
	}
}
#endif
