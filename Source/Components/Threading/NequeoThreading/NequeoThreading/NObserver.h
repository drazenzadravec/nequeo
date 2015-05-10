/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NObserver.h
*  Purpose :       NObserver class.
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

#ifndef _NOBSERVER_H
#define _NOBSERVER_H

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
			/// This class template is quite similar to the Observer class
			/// template. The only difference is that the NObserver
			/// expects the callback function to accept a const AutoPtr& 
			/// instead of a plain pointer as argument, thus simplifying memory
			/// management.
			template <class C, class N>
			class NObserver : public AbstractObserver
			{
			public:
				typedef AutoPtr<N> NotificationPtr;
				typedef void (C::*Callback)(const NotificationPtr&);

				NObserver(C& object, Callback method) :
					_pObject(&object),
					_method(method)
				{
				}

				NObserver(const NObserver& observer) :
					AbstractObserver(observer),
					_pObject(observer._pObject),
					_method(observer._method)
				{
				}

				~NObserver()
				{
				}

				NObserver& operator = (const NObserver& observer)
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
							NotificationPtr ptr(pCastNf, true);
							(_pObject->*_method)(ptr);
						}
					}
				}

				bool equals(const AbstractObserver& abstractObserver) const
				{
					const NObserver* pObs = dynamic_cast<const NObserver*>(&abstractObserver);
					return pObs && pObs->_pObject == _pObject && pObs->_method == _method;
				}

				bool accepts(Notification* pNf) const
				{
					return dynamic_cast<N*>(pNf) != 0;
				}

				AbstractObserver* clone() const
				{
					return new NObserver(*this);
				}

				void disable()
				{
					Poco::Mutex::ScopedLock lock(_mutex);

					_pObject = 0;
				}

			private:
				NObserver();

				C*       _pObject;
				Callback _method;
				mutable Poco::Mutex _mutex;
			};
		}
	}
}
#endif
