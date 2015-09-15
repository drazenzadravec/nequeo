/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Delegate.h
*  Purpose :       Delegate class.
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

#ifndef _DELEGATE_H
#define _DELEGATE_H

#include "GlobalThreading.h"

#include "AbstractDelegate.h"
#include "FunctionDelegate.h"
#include "Expire.h"
#include "Mutex.h"
#include "Base\Timestamp.h"

namespace Nequeo {
	namespace Threading {
		namespace Events
		{
			template <class TObj, class TArgs, bool withSender = true>
			class Delegate : public AbstractDelegate < TArgs >
			{
			public:
				typedef void (TObj::*NotifyMethod)(const void*, TArgs&);

				Delegate(TObj* obj, NotifyMethod method) :
					_receiverObject(obj),
					_receiverMethod(method)
				{
				}

				Delegate(const Delegate& delegate) :
					AbstractDelegate<TArgs>(delegate),
					_receiverObject(delegate._receiverObject),
					_receiverMethod(delegate._receiverMethod)
				{
				}

				~Delegate()
				{
				}

				Delegate& operator = (const Delegate& delegate)
				{
					if (&delegate != this)
					{
						this->_receiverObject = delegate._receiverObject;
						this->_receiverMethod = delegate._receiverMethod;
					}
					return *this;
				}

				bool notify(const void* sender, TArgs& arguments)
				{
					Mutex::ScopedLock lock(_mutex);
					if (_receiverObject)
					{
						(_receiverObject->*_receiverMethod)(sender, arguments);
						return true;
					}
					else return false;
				}

				bool equals(const AbstractDelegate<TArgs>& other) const
				{
					const Delegate* pOtherDelegate = reinterpret_cast<const Delegate*>(other.unwrap());
					return pOtherDelegate && _receiverObject == pOtherDelegate->_receiverObject && _receiverMethod == pOtherDelegate->_receiverMethod;
				}

				AbstractDelegate<TArgs>* clone() const
				{
					return new Delegate(*this);
				}

				void disable()
				{
					Mutex::ScopedLock lock(_mutex);
					_receiverObject = 0;
				}

			protected:
				TObj*        _receiverObject;
				NotifyMethod _receiverMethod;
				Mutex        _mutex;

			private:
				Delegate();
			};


			template <class TObj, class TArgs>
			class Delegate<TObj, TArgs, false> : public AbstractDelegate < TArgs >
			{
			public:
				typedef void (TObj::*NotifyMethod)(TArgs&);

				Delegate(TObj* obj, NotifyMethod method) :
					_receiverObject(obj),
					_receiverMethod(method)
				{
				}

				Delegate(const Delegate& delegate) :
					AbstractDelegate<TArgs>(delegate),
					_receiverObject(delegate._receiverObject),
					_receiverMethod(delegate._receiverMethod)
				{
				}

				~Delegate()
				{
				}

				Delegate& operator = (const Delegate& delegate)
				{
					if (&delegate != this)
					{
						this->_pTarget = delegate._pTarget;
						this->_receiverObject = delegate._receiverObject;
						this->_receiverMethod = delegate._receiverMethod;
					}
					return *this;
				}

				bool notify(const void*, TArgs& arguments)
				{
					Mutex::ScopedLock lock(_mutex);
					if (_receiverObject)
					{
						(_receiverObject->*_receiverMethod)(arguments);
						return true;
					}
					else return false;
				}

				bool equals(const AbstractDelegate<TArgs>& other) const
				{
					const Delegate* pOtherDelegate = reinterpret_cast<const Delegate*>(other.unwrap());
					return pOtherDelegate && _receiverObject == pOtherDelegate->_receiverObject && _receiverMethod == pOtherDelegate->_receiverMethod;
				}

				AbstractDelegate<TArgs>* clone() const
				{
					return new Delegate(*this);
				}

				void disable()
				{
					Mutex::ScopedLock lock(_mutex);
					_receiverObject = 0;
				}

			protected:
				TObj*        _receiverObject;
				NotifyMethod _receiverMethod;
				Mutex        _mutex;

			private:
				Delegate();
			};


			template <class TObj, class TArgs>
			static Delegate<TObj, TArgs, true> delegate(TObj* pObj, void (TObj::*NotifyMethod)(const void*, TArgs&))
			{
				return Delegate<TObj, TArgs, true>(pObj, NotifyMethod);
			}


			template <class TObj, class TArgs>
			static Delegate<TObj, TArgs, false> delegate(TObj* pObj, void (TObj::*NotifyMethod)(TArgs&))
			{
				return Delegate<TObj, TArgs, false>(pObj, NotifyMethod);
			}


			template <class TObj, class TArgs>
			static Expire<TArgs> delegate(TObj* pObj, void (TObj::*NotifyMethod)(const void*, TArgs&), Nequeo::Primitive::Timestamp::TimeDiff expireMillisecs)
			{
				return Expire<TArgs>(Delegate<TObj, TArgs, true>(pObj, NotifyMethod), expireMillisecs);
			}


			template <class TObj, class TArgs>
			static Expire<TArgs> delegate(TObj* pObj, void (TObj::*NotifyMethod)(TArgs&), Nequeo::Primitive::Timestamp::TimeDiff expireMillisecs)
			{
				return Expire<TArgs>(Delegate<TObj, TArgs, false>(pObj, NotifyMethod), expireMillisecs);
			}


			template <class TArgs>
			static Expire<TArgs> delegate(void(*NotifyMethod)(const void*, TArgs&), Nequeo::Primitive::Timestamp::TimeDiff expireMillisecs)
			{
				return Expire<TArgs>(FunctionDelegate<TArgs, true, true>(NotifyMethod), expireMillisecs);
			}


			template <class TArgs>
			static Expire<TArgs> delegate(void(*NotifyMethod)(void*, TArgs&), Nequeo::Primitive::Timestamp::TimeDiff expireMillisecs)
			{
				return Expire<TArgs>(FunctionDelegate<TArgs, true, false>(NotifyMethod), expireMillisecs);
			}


			template <class TArgs>
			static Expire<TArgs> delegate(void(*NotifyMethod)(TArgs&), Nequeo::Primitive::Timestamp::TimeDiff expireMillisecs)
			{
				return Expire<TArgs>(FunctionDelegate<TArgs, false>(NotifyMethod), expireMillisecs);
			}


			template <class TArgs>
			static FunctionDelegate<TArgs, true, true> delegate(void(*NotifyMethod)(const void*, TArgs&))
			{
				return FunctionDelegate<TArgs, true, true>(NotifyMethod);
			}


			template <class TArgs>
			static FunctionDelegate<TArgs, true, false> delegate(void(*NotifyMethod)(void*, TArgs&))
			{
				return FunctionDelegate<TArgs, true, false>(NotifyMethod);
			}


			template <class TArgs>
			static FunctionDelegate<TArgs, false> delegate(void(*NotifyMethod)(TArgs&))
			{
				return FunctionDelegate<TArgs, false>(NotifyMethod);
			}
		}
	}
}
#endif
