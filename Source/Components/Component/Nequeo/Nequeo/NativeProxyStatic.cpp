/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NativeProxyStatic.cpp
*  Purpose :       NativeProxyStatic class.
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

#include "NativeProxyStatic.h"

namespace Nequeo
{
	struct NativeProxyStatic::Impl
	{
		Impl(const stdtstring &dllName, const stdtstring &className)
			: managedDll(dllName)
			, managedClass(className)
		{
		}
		stdtstring managedDll, managedClass;
	};

	NativeProxyStatic::NativeProxyStatic(const stdtstring &dllName, const stdtstring &className)
		: _pImpl(new Impl(dllName, className))
	{
	}

	NativeProxyStatic::~NativeProxyStatic(void)
	{
		delete _pImpl;
	}

	void NativeProxyStatic::executeManaged(const stdtstring &function)
	{
		AnyTypeArray parameters;
		executeManaged(function, parameters);
	}

	void NativeProxyStatic::executeManaged(const stdtstring &function, AnyTypeArray &parameters)
	{
		AnyType result;
		executeManaged(function, parameters, result);
	}

	void NativeProxyStatic::executeManaged(const stdtstring &function, AnyType &result)
	{
		AnyTypeArray parameters;
		executeManaged(function, parameters, result);
	}

	void NativeProxyStatic::executeManaged(const stdtstring &function, AnyTypeArray &parameters, AnyType &result)
	{
		executeStatic(_pImpl->managedDll, _pImpl->managedClass, function, parameters, result);
	}

	void NativeProxyStatic::operator()(const stdtstring &function)
	{
		executeManaged(function);
	}

	void NativeProxyStatic::operator()(const stdtstring &function, AnyTypeArray &parameters)
	{
		executeManaged(function, parameters);
	}

	void NativeProxyStatic::operator()(const stdtstring &function, AnyType &result)
	{
		executeManaged(function, result);
	}

	void NativeProxyStatic::operator()(const stdtstring &function, AnyTypeArray &parameters, AnyType &result)
	{
		executeManaged(function, parameters, result);
	}

	void NativeProxyStatic::set(const stdtstring &property, const AnyType &value)
	{
		setPropertyStatic(_pImpl->managedDll, _pImpl->managedClass, property, value);
	}

	void NativeProxyStatic::get(const stdtstring &property, AnyType &value)
	{
		getPropertyStatic(_pImpl->managedDll, _pImpl->managedClass, property, value);
	}
}
