/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          PriorityDelegate.h
*  Purpose :       PriorityDelegate class.
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

#ifndef _PRIORITYDELEGATE_H
#define _PRIORITYDELEGATE_H

#include "GlobalThreading.h"

#include "AbstractPriorityDelegate.h"
#include "PriorityExpire.h"
#include "FunctionPriorityDelegate.h"
#include "Mutex.h"
#include "Timestamp.h"

namespace Nequeo {
	namespace Threading {
		namespace Events
		{
			template <class TObj, class TArgs, bool useSender = true>
			class PriorityDelegate : public AbstractPriorityDelegate < TArgs >
			{
			public:
				typedef void (TObj::*NotifyMethod)(const void*, TArgs&);

				PriorityDelegate(TObj* obj, NotifyMethod method, int prio) :
					AbstractPriorityDelegate<TArgs>(prio),
					_receiverObject(obj),
					_receiverMethod(method)
				{
				}

				PriorityDelegate(const PriorityDelegate& delegate) :
					AbstractPriorityDelegate<TArgs>(delegate),
					_receiverObject(delegate._receiverObject),
					_receiverMethod(delegate._receiverMethod)
				{
				}

				PriorityDelegate& operator = (const PriorityDelegate& delegate)
				{
					if (&delegate != this)
					{
						this->_pTarget = delegate._pTarget;
						this->_receiverObject = delegate._receiverObject;
						this->_receiverMethod = delegate._receiverMethod;
						this->_priority = delegate._priority;
					}
					return *this;
				}

				~PriorityDelegate()
				{
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
					const PriorityDelegate* pOtherDelegate = dynamic_cast<const PriorityDelegate*>(other.unwrap());
					return pOtherDelegate && this->priority() == pOtherDelegate->priority() && _receiverObject == pOtherDelegate->_receiverObject && _receiverMethod == pOtherDelegate->_receiverMethod;
				}

				AbstractDelegate<TArgs>* clone() const
				{
					return new PriorityDelegate(*this);
				}

				void disable()
				{
					Mutex::ScopedLock lock(_mutex);
					_receiverObject = 0;
				}

			protected:
				TObj*        _receiverObject;
				NotifyMethod _receiverMethod;
				Mutex _mutex;

			private:
				PriorityDelegate();
			};


			template <class TObj, class TArgs>
			class PriorityDelegate<TObj, TArgs, false> : public AbstractPriorityDelegate < TArgs >
			{
			public:
				typedef void (TObj::*NotifyMethod)(TArgs&);

				PriorityDelegate(TObj* obj, NotifyMethod method, int prio) :
					AbstractPriorityDelegate<TArgs>(prio),
					_receiverObject(obj),
					_receiverMethod(method)
				{
				}

				PriorityDelegate(const PriorityDelegate& delegate) :
					AbstractPriorityDelegate<TArgs>(delegate),
					_receiverObject(delegate._receiverObject),
					_receiverMethod(delegate._receiverMethod)
				{
				}

				PriorityDelegate& operator = (const PriorityDelegate& delegate)
				{
					if (&delegate != this)
					{
						this->_pTarget = delegate._pTarget;
						this->_receiverObject = delegate._receiverObject;
						this->_receiverMethod = delegate._receiverMethod;
						this->_priority = delegate._priority;
					}
					return *this;
				}

				~PriorityDelegate()
				{
				}

				bool notify(const void* sender, TArgs& arguments)
				{
					Mutex::ScopedLock lock(_mutex);
					if (_receiverObject)
					{
						(_receiverObject->*_receiverMethod)(arguments);
						return true;
					}
					return false;
				}

				bool equals(const AbstractDelegate<TArgs>& other) const
				{
					const PriorityDelegate* pOtherDelegate = dynamic_cast<const PriorityDelegate*>(other.unwrap());
					return pOtherDelegate && this->priority() == pOtherDelegate->priority() && _receiverObject == pOtherDelegate->_receiverObject && _receiverMethod == pOtherDelegate->_receiverMethod;
				}

				AbstractDelegate<TArgs>* clone() const
				{
					return new PriorityDelegate(*this);
				}

				void disable()
				{
					Mutex::ScopedLock lock(_mutex);
					_receiverObject = 0;
				}

			protected:
				TObj*        _receiverObject;
				NotifyMethod _receiverMethod;
				Mutex _mutex;

			private:
				PriorityDelegate();
			};


			template <class TObj, class TArgs>
			static PriorityDelegate<TObj, TArgs, true> priorityDelegate(TObj* pObj, void (TObj::*NotifyMethod)(const void*, TArgs&), int prio)
			{
				return PriorityDelegate<TObj, TArgs, true>(pObj, NotifyMethod, prio);
			}


			template <class TObj, class TArgs>
			static PriorityDelegate<TObj, TArgs, false> priorityDelegate(TObj* pObj, void (TObj::*NotifyMethod)(TArgs&), int prio)
			{
				return PriorityDelegate<TObj, TArgs, false>(pObj, NotifyMethod, prio);
			}


			template <class TObj, class TArgs>
			static PriorityExpire<TArgs> priorityDelegate(TObj* pObj, void (TObj::*NotifyMethod)(const void*, TArgs&), int prio, Nequeo::Primitive::Timestamp::TimeDiff expireMilliSec)
			{
				return PriorityExpire<TArgs>(PriorityDelegate<TObj, TArgs, true>(pObj, NotifyMethod, prio), expireMilliSec);
			}


			template <class TObj, class TArgs>
			static PriorityExpire<TArgs> priorityDelegate(TObj* pObj, void (TObj::*NotifyMethod)(TArgs&), int prio, Nequeo::Primitive::Timestamp::TimeDiff expireMilliSec)
			{
				return PriorityExpire<TArgs>(PriorityDelegate<TObj, TArgs, false>(pObj, NotifyMethod, prio), expireMilliSec);
			}


			template <class TArgs>
			static PriorityExpire<TArgs> priorityDelegate(void(*NotifyMethod)(const void*, TArgs&), int prio, Nequeo::Primitive::Timestamp::TimeDiff expireMilliSec)
			{
				return PriorityExpire<TArgs>(FunctionPriorityDelegate<TArgs, true, true>(NotifyMethod, prio), expireMilliSec);
			}


			template <class TArgs>
			static PriorityExpire<TArgs> priorityDelegate(void(*NotifyMethod)(void*, TArgs&), int prio, Nequeo::Primitive::Timestamp::TimeDiff expireMilliSec)
			{
				return PriorityExpire<TArgs>(FunctionPriorityDelegate<TArgs, true, false>(NotifyMethod, prio), expireMilliSec);
			}


			template <class TArgs>
			static PriorityExpire<TArgs> priorityDelegate(void(*NotifyMethod)(TArgs&), int prio, Nequeo::Primitive::Timestamp::TimeDiff expireMilliSec)
			{
				return PriorityExpire<TArgs>(FunctionPriorityDelegate<TArgs, false>(NotifyMethod, prio), expireMilliSec);
			}


			template <class TArgs>
			static FunctionPriorityDelegate<TArgs, true, true> priorityDelegate(void(*NotifyMethod)(const void*, TArgs&), int prio)
			{
				return FunctionPriorityDelegate<TArgs, true, true>(NotifyMethod, prio);
			}


			template <class TArgs>
			static FunctionPriorityDelegate<TArgs, true, false> priorityDelegate(void(*NotifyMethod)(void*, TArgs&), int prio)
			{
				return FunctionPriorityDelegate<TArgs, true, false>(NotifyMethod, prio);
			}


			template <class TArgs>
			static FunctionPriorityDelegate<TArgs, false> priorityDelegate(void(*NotifyMethod)(TArgs&), int prio)
			{
				return FunctionPriorityDelegate<TArgs, false>(NotifyMethod, prio);
			}
		}
	}
}
#endif
