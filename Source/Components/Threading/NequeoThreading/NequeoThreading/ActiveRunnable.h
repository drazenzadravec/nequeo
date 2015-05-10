/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Runnable.h
*  Purpose :       Runnable class.
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

#ifndef _ACTIVERUNNABLE_H
#define _ACTIVERUNNABLE_H

#include "GlobalThreading.h"

#include "ActiveResult.h"
#include "Runnable.h"
#include "Base\RefCountedObject.h"
#include "Base\AutoPtr.h"
#include "Exceptions\Exception.h"

namespace Nequeo {
	namespace Threading
	{
		class ActiveRunnableBase : public Runnable, public RefCountedObject
			/// The base class for all ActiveRunnable instantiations.
		{
		public:
			typedef Nequeo::AutoPtr<ActiveRunnableBase> Ptr;
		};


		template <class ResultType, class ArgType, class OwnerType>
		class ActiveRunnable : public ActiveRunnableBase
			/// This class is used by ActiveMethod.
			/// See the ActiveMethod class for more information.
		{
		public:
			typedef ResultType(OwnerType::*Callback)(const ArgType&);
			typedef ActiveResult<ResultType> ActiveResultType;

			ActiveRunnable(OwnerType* pOwner, Callback method, const ArgType& arg, const ActiveResultType& result) :
				_pOwner(pOwner),
				_method(method),
				_arg(arg),
				_result(result)
			{
				poco_check_ptr(pOwner);
			}

			void run()
			{
				ActiveRunnableBase::Ptr guard(this, false); // ensure automatic release when done
				try
				{
					_result.data(new ResultType((_pOwner->*_method)(_arg)));
				}
				catch (Exception& e)
				{
					_result.error(e);
				}
				catch (std::exception& e)
				{
					_result.error(e.what());
				}
				catch (...)
				{
					_result.error("unknown exception");
				}
				_result.notify();
			}

		private:
			OwnerType* _pOwner;
			Callback   _method;
			ArgType    _arg;
			ActiveResultType _result;
		};


		template <class ArgType, class OwnerType>
		class ActiveRunnable<void, ArgType, OwnerType> : public ActiveRunnableBase
			/// This class is used by ActiveMethod.
			/// See the ActiveMethod class for more information.
		{
		public:
			typedef void (OwnerType::*Callback)(const ArgType&);
			typedef ActiveResult<void> ActiveResultType;

			ActiveRunnable(OwnerType* pOwner, Callback method, const ArgType& arg, const ActiveResultType& result) :
				_pOwner(pOwner),
				_method(method),
				_arg(arg),
				_result(result)
			{
				poco_check_ptr(pOwner);
			}

			void run()
			{
				ActiveRunnableBase::Ptr guard(this, false); // ensure automatic release when done
				try
				{
					(_pOwner->*_method)(_arg);
				}
				catch (Exception& e)
				{
					_result.error(e);
				}
				catch (std::exception& e)
				{
					_result.error(e.what());
				}
				catch (...)
				{
					_result.error("unknown exception");
				}
				_result.notify();
			}

		private:
			OwnerType* _pOwner;
			Callback   _method;
			ArgType    _arg;
			ActiveResultType _result;
		};


		template <class ResultType, class OwnerType>
		class ActiveRunnable<ResultType, void, OwnerType> : public ActiveRunnableBase
			/// This class is used by ActiveMethod.
			/// See the ActiveMethod class for more information.
		{
		public:
			typedef ResultType(OwnerType::*Callback)();
			typedef ActiveResult<ResultType> ActiveResultType;

			ActiveRunnable(OwnerType* pOwner, Callback method, const ActiveResultType& result) :
				_pOwner(pOwner),
				_method(method),
				_result(result)
			{
				poco_check_ptr(pOwner);
			}

			void run()
			{
				ActiveRunnableBase::Ptr guard(this, false); // ensure automatic release when done
				try
				{
					_result.data(new ResultType((_pOwner->*_method)()));
				}
				catch (Exception& e)
				{
					_result.error(e);
				}
				catch (std::exception& e)
				{
					_result.error(e.what());
				}
				catch (...)
				{
					_result.error("unknown exception");
				}
				_result.notify();
			}

		private:
			OwnerType* _pOwner;
			Callback   _method;
			ActiveResultType _result;
		};


		template <class OwnerType>
		class ActiveRunnable<void, void, OwnerType> : public ActiveRunnableBase
			/// This class is used by ActiveMethod.
			/// See the ActiveMethod class for more information.
		{
		public:
			typedef void (OwnerType::*Callback)();
			typedef ActiveResult<void> ActiveResultType;

			ActiveRunnable(OwnerType* pOwner, Callback method, const ActiveResultType& result) :
				_pOwner(pOwner),
				_method(method),
				_result(result)
			{
				poco_check_ptr(pOwner);
			}

			void run()
			{
				ActiveRunnableBase::Ptr guard(this, false); // ensure automatic release when done
				try
				{
					(_pOwner->*_method)();
				}
				catch (Exception& e)
				{
					_result.error(e);
				}
				catch (std::exception& e)
				{
					_result.error(e.what());
				}
				catch (...)
				{
					_result.error("unknown exception");
				}
				_result.notify();
			}

		private:
			OwnerType* _pOwner;
			Callback   _method;
			ActiveResultType _result;
		};
	}
}
#endif
