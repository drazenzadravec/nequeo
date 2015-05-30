/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          PriorityExpire.h
*  Purpose :       PriorityExpire class.
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

#ifndef _PRIORITYEXPIRE_H
#define _PRIORITYEXPIRE_H

#include "GlobalThreading.h"

#include "Timestamp.h"
#include "AbstractPriorityDelegate.h"

namespace Nequeo {
	namespace Threading {
		namespace Events
		{
			/// Decorator for AbstractPriorityDelegate adding automatic 
			/// expiring of registrations to AbstractPriorityDelegate.
			template <class TArgs>
			class PriorityExpire : public AbstractPriorityDelegate < TArgs >
			{
			public:
				PriorityExpire(const AbstractPriorityDelegate<TArgs>& p, Timestamp::TimeDiff expireMilliSec) :
					AbstractPriorityDelegate<TArgs>(p),
					_pDelegate(static_cast<AbstractPriorityDelegate<TArgs>*>(p.clone())),
					_expire(expireMilliSec * 1000)
				{
				}

				PriorityExpire(const PriorityExpire& expire) :
					AbstractPriorityDelegate<TArgs>(expire),
					_pDelegate(static_cast<AbstractPriorityDelegate<TArgs>*>(expire._pDelegate->clone())),
					_expire(expire._expire),
					_creationTime(expire._creationTime)
				{
				}

				~PriorityExpire()
				{
					delete _pDelegate;
				}

				PriorityExpire& operator = (const PriorityExpire& expire)
				{
					if (&expire != this)
					{
						delete this->_pDelegate;
						this->_pTarget = expire._pTarget;
						this->_pDelegate = expire._pDelegate->clone();
						this->_expire = expire._expire;
						this->_creationTime = expire._creationTime;
					}
					return *this;
				}

				bool notify(const void* sender, TArgs& arguments)
				{
					if (!expired())
						return this->_pDelegate->notify(sender, arguments);
					else
						return false;
				}

				bool equals(const AbstractDelegate<TArgs>& other) const
				{
					return other.equals(*_pDelegate);
				}

				AbstractPriorityDelegate<TArgs>* clone() const
				{
					return new PriorityExpire(*this);
				}

				void disable()
				{
					_pDelegate->disable();
				}

				const AbstractPriorityDelegate<TArgs>* unwrap() const
				{
					return this->_pDelegate;
				}

			protected:
				bool expired() const
				{
					return _creationTime.isElapsed(_expire);
				}

				AbstractPriorityDelegate<TArgs>* _pDelegate;
				Timestamp::TimeDiff _expire;
				Timestamp _creationTime;

			private:
				PriorityExpire();
			};
		}
	}
}
#endif
