/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Expire.h
*  Purpose :       Expire class.
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

#ifndef _EXPIRE_H
#define _EXPIRE_H

#include "GlobalThreading.h"

#include "AbstractDelegate.h"
#include "Base\Timestamp.h"

namespace Nequeo {
	namespace Threading {
		namespace Events
		{
			/// Decorator for AbstractDelegate adding automatic 
			/// expiration of registrations to AbstractDelegate's.
			template <class TArgs>
			class Expire : public AbstractDelegate < TArgs >
			{
			public:
				Expire(const AbstractDelegate<TArgs>& p, Timestamp::TimeDiff expireMillisecs) :
					_pDelegate(p.clone()),
					_expire(expireMillisecs * 1000)
				{
				}

				Expire(const Expire& expire) :
					AbstractDelegate<TArgs>(expire),
					_pDelegate(expire._pDelegate->clone()),
					_expire(expire._expire),
					_creationTime(expire._creationTime)
				{
				}

				~Expire()
				{
					delete _pDelegate;
				}

				Expire& operator = (const Expire& expire)
				{
					if (&expire != this)
					{
						delete this->_pDelegate;
						this->_pDelegate = expire._pDelegate->clone();
						this->_expire = expire._expire;
						this->_creationTime = expire._creationTime;
						this->_pTarget = expire._pTarget;
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

				AbstractDelegate<TArgs>* clone() const
				{
					return new Expire(*this);
				}

				void disable()
				{
					_pDelegate->disable();
				}

				const AbstractDelegate<TArgs>* unwrap() const
				{
					return this->_pDelegate;
				}

			protected:
				bool expired() const
				{
					return _creationTime.isElapsed(_expire);
				}

				AbstractDelegate<TArgs>* _pDelegate;
				Timestamp::TimeDiff _expire;
				Timestamp _creationTime;

			private:
				Expire();
			};
		}
	}
}
#endif
