/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ActiveMethod.h
*  Purpose :       ActiveMethod class.
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

#ifndef _ACTIVEMETHOD_H
#define _ACTIVEMETHOD_H

#include "GlobalThreading.h"

#include "ActiveResult.h"
#include "ActiveRunnable.h"
#include "ActiveStarter.h"
#include "AutoPtr.h"

namespace Nequeo {
	namespace Threading
	{
		template <class ResultType, class ArgType, class OwnerType, class StarterType = ActiveStarter<OwnerType> >
		class ActiveMethod
			/// An active method is a method that, when called, executes
			/// in its own thread. ActiveMethod's take exactly one
			/// argument and can return a value. To pass more than one
			/// argument to the method, use a struct.
			/// The following example shows how to add an ActiveMethod
			/// to a class:
			///
			///     class ActiveObject
			///     {
			///     public:
			///         ActiveObject():
			///             exampleActiveMethod(this, &ActiveObject::exampleActiveMethodImpl)
			///         {
			///         }
			///
			///         ActiveMethod<std::string, std::string, ActiveObject> exampleActiveMethod;
			///
			///     protected:
			///         std::string exampleActiveMethodImpl(const std::string& arg)
			///         {
			///             ...
			///         }
			///     };
			///
			/// And following is an example that shows how to invoke an ActiveMethod.
			///
			///     ActiveObject myActiveObject;
			///     ActiveResult<std::string> result = myActiveObject.exampleActiveMethod("foo");
			///     ...
			///     result.wait();
			///     std::cout << result.data() << std::endl;
			///
			/// The way an ActiveMethod is started can be changed by passing a StarterType
			/// template argument with a corresponding class. The default ActiveStarter
			/// starts the method in its own thread, obtained from a thread pool.
			///
			/// For an alternative implementation of StarterType, see ActiveDispatcher.
			///
			/// For methods that do not require an argument or a return value, the Void
			/// class can be used.
		{
		public:
			typedef ResultType(OwnerType::*Callback)(const ArgType&);
			typedef ActiveResult<ResultType> ActiveResultType;
			typedef ActiveRunnable<ResultType, ArgType, OwnerType> ActiveRunnableType;

			ActiveMethod(OwnerType* pOwner, Callback method) :
				_pOwner(pOwner),
				_method(method)
				/// Creates an ActiveMethod object.
			{
				poco_check_ptr(pOwner);
			}

			ActiveResultType operator () (const ArgType& arg)
				/// Invokes the ActiveMethod.
			{
				ActiveResultType result(new ActiveResultHolder<ResultType>());
				ActiveRunnableBase::Ptr pRunnable(new ActiveRunnableType(_pOwner, _method, arg, result));
				StarterType::start(_pOwner, pRunnable);
				return result;
			}

			ActiveMethod(const ActiveMethod& other) :
				_pOwner(other._pOwner),
				_method(other._method)
			{
			}

			ActiveMethod& operator = (const ActiveMethod& other)
			{
				ActiveMethod tmp(other);
				swap(tmp);
				return *this;
			}

			void swap(ActiveMethod& other)
			{
				std::swap(_pOwner, other._pOwner);
				std::swap(_method, other._method);
			}

		private:
			ActiveMethod();

			OwnerType* _pOwner;
			Callback   _method;
		};



		template <class ResultType, class OwnerType, class StarterType>
		class ActiveMethod < ResultType, void, OwnerType, StarterType >
			/// An active method is a method that, when called, executes
			/// in its own thread. ActiveMethod's take exactly one
			/// argument and can return a value. To pass more than one
			/// argument to the method, use a struct.
			/// The following example shows how to add an ActiveMethod
			/// to a class:
			///
			///     class ActiveObject
			///     {
			///     public:
			///         ActiveObject():
			///             exampleActiveMethod(this, &ActiveObject::exampleActiveMethodImpl)
			///         {
			///         }
			///
			///         ActiveMethod<std::string, std::string, ActiveObject> exampleActiveMethod;
			///
			///     protected:
			///         std::string exampleActiveMethodImpl(const std::string& arg)
			///         {
			///             ...
			///         }
			///     };
			///
			/// And following is an example that shows how to invoke an ActiveMethod.
			///
			///     ActiveObject myActiveObject;
			///     ActiveResult<std::string> result = myActiveObject.exampleActiveMethod("foo");
			///     ...
			///     result.wait();
			///     std::cout << result.data() << std::endl;
			///
			/// The way an ActiveMethod is started can be changed by passing a StarterType
			/// template argument with a corresponding class. The default ActiveStarter
			/// starts the method in its own thread, obtained from a thread pool.
			///
			/// For an alternative implementation of StarterType, see ActiveDispatcher.
			///
			/// For methods that do not require an argument or a return value, simply use void.
		{
		public:
			typedef ResultType(OwnerType::*Callback)(void);
			typedef ActiveResult<ResultType> ActiveResultType;
			typedef ActiveRunnable<ResultType, void, OwnerType> ActiveRunnableType;

			ActiveMethod(OwnerType* pOwner, Callback method) :
				_pOwner(pOwner),
				_method(method)
				/// Creates an ActiveMethod object.
			{
				poco_check_ptr(pOwner);
			}

			ActiveResultType operator () (void)
				/// Invokes the ActiveMethod.
			{
				ActiveResultType result(new ActiveResultHolder<ResultType>());
				ActiveRunnableBase::Ptr pRunnable(new ActiveRunnableType(_pOwner, _method, result));
				StarterType::start(_pOwner, pRunnable);
				return result;
			}

			ActiveMethod(const ActiveMethod& other) :
				_pOwner(other._pOwner),
				_method(other._method)
			{
			}

			ActiveMethod& operator = (const ActiveMethod& other)
			{
				ActiveMethod tmp(other);
				swap(tmp);
				return *this;
			}

			void swap(ActiveMethod& other)
			{
				std::swap(_pOwner, other._pOwner);
				std::swap(_method, other._method);
			}

		private:
			ActiveMethod();

			OwnerType* _pOwner;
			Callback   _method;
		};
	}
}
#endif
