/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NativeProxy.cpp
*  Purpose :       NativeProxy class.
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

#include "stdafx.h"

#include "NativeProxy.h"

namespace Nequeo
{
	struct NativeProxy::impl
	{
		impl(const stdtstring &dllName, const stdtstring &className)
			: managedDll(dllName)
			, managedClass(className)
			, managedClassPtr(0)
		{
		}
		stdtstring managedDll, managedClass;
		void *managedClassPtr;
	};

	NativeProxy::NativeProxy(const stdtstring &dllName, const stdtstring &className)
		: _pimpl(new impl(dllName, className))
	{
		_pimpl->managedClassPtr = createManagedClass(dllName, className);
	}

	NativeProxy::NativeProxy(const stdtstring &dllName, const stdtstring &className, const AnyTypeArray &ctorParams)
		: _pimpl(new impl(dllName, className))
	{
		_pimpl->managedClassPtr = createManagedClass(dllName, className, ctorParams);
	}

	NativeProxy::~NativeProxy()
	{
		try
		{
			destroyManagedClass(_pimpl->managedClassPtr);
		}
		catch (...)
		{
		}
		delete _pimpl;
	}

	void NativeProxy::executeManaged(const stdtstring &function)
	{
		AnyTypeArray parameters;
		executeManaged(function, parameters);
	}

	void NativeProxy::executeManaged(const stdtstring &function, AnyTypeArray &parameters)
	{
		AnyType result;
		executeManaged(function, parameters, result);
	}

	void NativeProxy::executeManaged(const stdtstring &function, AnyType &result)
	{
		AnyTypeArray parameters;
		executeManaged(function, parameters, result);
	}

	void NativeProxy::executeManaged(const stdtstring &function, AnyTypeArray &parameters, AnyType &result)
	{
		execute(_pimpl->managedClassPtr, function, parameters, result);
	}

	void NativeProxy::operator()(const stdtstring &function)
	{
		executeManaged(function);
	}

	void NativeProxy::operator()(const stdtstring &function, AnyTypeArray &parameters)
	{
		executeManaged(function, parameters);
	}

	void NativeProxy::operator()(const stdtstring &function, AnyType &result)
	{
		executeManaged(function, result);
	}

	void NativeProxy::operator()(const stdtstring &function, AnyTypeArray &parameters, AnyType &result)
	{
		executeManaged(function, parameters, result);
	}

	void NativeProxy::set(const stdtstring &property, const AnyType &value)
	{
		setProperty(_pimpl->managedClassPtr, property, value);
	}

	void NativeProxy::get(const stdtstring &property, AnyType &value)
	{
		getProperty(_pimpl->managedClassPtr, property, value);
	}
}
