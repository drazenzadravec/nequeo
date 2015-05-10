/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          AbstractObserver.h
*  Purpose :       AbstractObserver class.
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

#ifndef _ABSTRACTOBSERVER_H
#define _ABSTRACTOBSERVER_H

#include "GlobalThreading.h"

#include "Notification.h"

namespace Nequeo {
	namespace Threading {
		namespace Notifications
		{
			/// The base class for all instantiations of
			/// the Observer and NObserver template classes.
			class AbstractObserver
			{
			public:
				AbstractObserver();
				AbstractObserver(const AbstractObserver& observer);
				virtual ~AbstractObserver();

				AbstractObserver& operator = (const AbstractObserver& observer);

				virtual void notify(Notification* pNf) const = 0;
				virtual bool equals(const AbstractObserver& observer) const = 0;
				virtual bool accepts(Notification* pNf) const = 0;
				virtual AbstractObserver* clone() const = 0;
				virtual void disable() = 0;
			};
		}
	}
}
#endif
