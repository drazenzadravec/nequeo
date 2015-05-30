/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          FunctionPriorityDelegate.h
*  Purpose :       FunctionPriorityDelegate class.
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

#ifndef _FUNCTIONPRIORITYDELEGATE_H
#define _FUNCTIONPRIORITYDELEGATE_H

#include "GlobalThreading.h"

#include "AbstractPriorityDelegate.h"
#include "Mutex.h"

namespace Nequeo {
	namespace Threading {
		namespace Events
		{
			/// Wraps a freestanding function or static member function 
			/// for use as a PriorityDelegate.
			template <class TArgs, bool useSender = true, bool senderIsConst = true>
			class FunctionPriorityDelegate : public AbstractPriorityDelegate < TArgs >
			{
			public:
				typedef void(*NotifyMethod)(const void*, TArgs&);

				FunctionPriorityDelegate(NotifyMethod method, int prio) :
					AbstractPriorityDelegate<TArgs>(prio),
					_receiverMethod(method)
				{
				}

				FunctionPriorityDelegate(const FunctionPriorityDelegate& delegate) :
					AbstractPriorityDelegate<TArgs>(delegate),
					_receiverMethod(delegate._receiverMethod)
				{
				}

				FunctionPriorityDelegate& operator = (const FunctionPriorityDelegate& delegate)
				{
					if (&delegate != this)
					{
						this->_pTarget = delegate._pTarget;
						this->_receiverMethod = delegate._receiverMethod;
						this->_priority = delegate._priority;
					}
					return *this;
				}

				~FunctionPriorityDelegate()
				{
				}

				bool notify(const void* sender, TArgs& arguments)
				{
					Mutex::ScopedLock lock(_mutex);
					if (_receiverMethod)
					{
						(*_receiverMethod)(sender, arguments);
						return true;
					}
					else return false;
				}

				bool equals(const AbstractDelegate<TArgs>& other) const
				{
					const FunctionPriorityDelegate* pOtherDelegate = dynamic_cast<const FunctionPriorityDelegate*>(other.unwrap());
					return pOtherDelegate && this->priority() == pOtherDelegate->priority() && _receiverMethod == pOtherDelegate->_receiverMethod;
				}

				AbstractDelegate<TArgs>* clone() const
				{
					return new FunctionPriorityDelegate(*this);
				}

				void disable()
				{
					Mutex::ScopedLock lock(_mutex);
					_receiverMethod = 0;
				}

			protected:
				NotifyMethod _receiverMethod;
				Mutex _mutex;

			private:
				FunctionPriorityDelegate();
			};


			template <class TArgs>
			class FunctionPriorityDelegate<TArgs, true, false> : public AbstractPriorityDelegate < TArgs >
			{
			public:
				typedef void(*NotifyMethod)(void*, TArgs&);

				FunctionPriorityDelegate(NotifyMethod method, int prio) :
					AbstractPriorityDelegate<TArgs>(prio),
					_receiverMethod(method)
				{
				}

				FunctionPriorityDelegate(const FunctionPriorityDelegate& delegate) :
					AbstractPriorityDelegate<TArgs>(delegate),
					_receiverMethod(delegate._receiverMethod)
				{
				}

				FunctionPriorityDelegate& operator = (const FunctionPriorityDelegate& delegate)
				{
					if (&delegate != this)
					{
						this->_pTarget = delegate._pTarget;
						this->_receiverMethod = delegate._receiverMethod;
						this->_priority = delegate._priority;
					}
					return *this;
				}

				~FunctionPriorityDelegate()
				{
				}

				bool notify(const void* sender, TArgs& arguments)
				{
					Mutex::ScopedLock lock(_mutex);
					if (_receiverMethod)
					{
						(*_receiverMethod)(const_cast<void*>(sender), arguments);
						return true;
					}
					else return false;
				}

				bool equals(const AbstractDelegate<TArgs>& other) const
				{
					const FunctionPriorityDelegate* pOtherDelegate = dynamic_cast<const FunctionPriorityDelegate*>(other.unwrap());
					return pOtherDelegate && this->priority() == pOtherDelegate->priority() && _receiverMethod == pOtherDelegate->_receiverMethod;
				}

				AbstractDelegate<TArgs>* clone() const
				{
					return new FunctionPriorityDelegate(*this);
				}

				void disable()
				{
					Mutex::ScopedLock lock(_mutex);
					_receiverMethod = 0;
				}

			protected:
				NotifyMethod _receiverMethod;
				Mutex _mutex;

			private:
				FunctionPriorityDelegate();
			};


			template <class TArgs>
			class FunctionPriorityDelegate<TArgs, false> : public AbstractPriorityDelegate < TArgs >
			{
			public:
				typedef void(*NotifyMethod)(TArgs&);

				FunctionPriorityDelegate(NotifyMethod method, int prio) :
					AbstractPriorityDelegate<TArgs>(prio),
					_receiverMethod(method)
				{
				}

				FunctionPriorityDelegate(const FunctionPriorityDelegate& delegate) :
					AbstractPriorityDelegate<TArgs>(delegate),
					_receiverMethod(delegate._receiverMethod)
				{
				}

				FunctionPriorityDelegate& operator = (const FunctionPriorityDelegate& delegate)
				{
					if (&delegate != this)
					{
						this->_pTarget = delegate._pTarget;
						this->_receiverMethod = delegate._receiverMethod;
						this->_priority = delegate._priority;
					}
					return *this;
				}

				~FunctionPriorityDelegate()
				{
				}

				bool notify(const void* sender, TArgs& arguments)
				{
					Mutex::ScopedLock lock(_mutex);
					if (_receiverMethod)
					{
						(*_receiverMethod)(arguments);
						return true;
					}
					else return false;
				}

				bool equals(const AbstractDelegate<TArgs>& other) const
				{
					const FunctionPriorityDelegate* pOtherDelegate = dynamic_cast<const FunctionPriorityDelegate*>(other.unwrap());
					return pOtherDelegate && this->priority() == pOtherDelegate->priority() && _receiverMethod == pOtherDelegate->_receiverMethod;
				}

				AbstractDelegate<TArgs>* clone() const
				{
					return new FunctionPriorityDelegate(*this);
				}

				void disable()
				{
					Mutex::ScopedLock lock(_mutex);
					_receiverMethod = 0;
				}

			protected:
				NotifyMethod _receiverMethod;
				Mutex _mutex;

			private:
				FunctionPriorityDelegate();
			};
		}
	}
}
#endif
