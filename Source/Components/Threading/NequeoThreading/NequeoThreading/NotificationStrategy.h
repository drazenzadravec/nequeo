/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NotificationStrategy.h
*  Purpose :       NotificationStrategy class.
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

#ifndef _NOTIFICATIONSTRATEGY_H
#define _NOTIFICATIONSTRATEGY_H

#include "GlobalThreading.h"

namespace Nequeo {
	namespace Threading {
		namespace Events
		{
			/// The interface that all notification strategies must implement.
			/// 
			/// Note: Event is based on policy-driven design, so every strategy implementation
			/// must provide all the methods from this interface (otherwise: compile errors)
			/// but does not need to inherit from NotificationStrategy.
			template <class TArgs, class TDelegate>
			class NotificationStrategy
			{
			public:
				NotificationStrategy()
				{
				}

				virtual ~NotificationStrategy()
				{
				}

				virtual void notify(const void* sender, TArgs& arguments) = 0;
				/// Sends a notification to all registered delegates.

				virtual void add(const TDelegate& delegate) = 0;
				/// Adds a delegate to the strategy.

				virtual void remove(const TDelegate& delegate) = 0;
				/// Removes a delegate from the strategy, if found.
				/// Does nothing if the delegate has not been added.

				virtual void clear() = 0;
				/// Removes all delegates from the strategy.

				virtual bool empty() const = 0;
				/// Returns false if the strategy contains at least one delegate.
			};
		}
	}
}
#endif
