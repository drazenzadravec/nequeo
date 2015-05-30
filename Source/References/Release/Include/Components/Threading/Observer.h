/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Observer.h
*  Purpose :       Observer class.
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

#ifndef _OBSERVER_H
#define _OBSERVER_H

#include "GlobalThreading.h"

#include "AbstractObserver.h"
#include "Mutex.h"

namespace Nequeo {
	namespace Threading {
		namespace Notifications
		{
			/// This template class implements an adapter that sits between
			/// a NotificationCenter and an object receiving notifications
			/// from it. It is quite similar in concept to the 
			/// RunnableAdapter, but provides some NotificationCenter
			/// specific additional methods.
			/// See the NotificationCenter class for information on how
			/// to use this template class.
			///
			/// Instead of the Observer class template, you might want to
			/// use the NObserver class template, which uses an AutoPtr to
			/// pass the Notification to the callback function, thus freeing
			/// you from memory management issues.
			template <class C, class N>
			class Observer : public AbstractObserver
			{
			public:
				typedef void (C::*Callback)(N*);

				Observer(C& object, Callback method) :
					_pObject(&object),
					_method(method)
				{
				}

				Observer(const Observer& observer) :
					AbstractObserver(observer),
					_pObject(observer._pObject),
					_method(observer._method)
				{
				}

				~Observer()
				{
				}

				Observer& operator = (const Observer& observer)
				{
					if (&observer != this)
					{
						_pObject = observer._pObject;
						_method = observer._method;
					}
					return *this;
				}

				void notify(Notification* pNf) const
				{
					Poco::Mutex::ScopedLock lock(_mutex);

					if (_pObject)
					{
						N* pCastNf = dynamic_cast<N*>(pNf);
						if (pCastNf)
						{
							pCastNf->duplicate();
							(_pObject->*_method)(pCastNf);
						}
					}
				}

				bool equals(const AbstractObserver& abstractObserver) const
				{
					const Observer* pObs = dynamic_cast<const Observer*>(&abstractObserver);
					return pObs && pObs->_pObject == _pObject && pObs->_method == _method;
				}

				bool accepts(Notification* pNf) const
				{
					return dynamic_cast<N*>(pNf) != 0;
				}

				AbstractObserver* clone() const
				{
					return new Observer(*this);
				}

				void disable()
				{
					Poco::Mutex::ScopedLock lock(_mutex);

					_pObject = 0;
				}

			private:
				Observer();

				C*       _pObject;
				Callback _method;
				mutable Mutex _mutex;
			};
		}
	}
}
#endif
