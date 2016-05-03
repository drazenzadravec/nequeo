/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NativeProxyBase.cpp
*  Purpose :       NativeProxyBase class.
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

#include "NativeProxyBase.h"

namespace Nequeo
{
	const TCHAR BridgeDll[] = _T("NequeoNativeToManaged.dll");
	const TCHAR ModuleSubDirectory[] = _T("NequeoManagedModules");

	struct NativeProxyBase::Impl
	{
		Impl(const stdtstring &dllfilename);
		~Impl();

		void retrieveFunctionAddresses();
		void checkFunctionAddresses();
		void loadLibrary();
		void init();
		template<class FunctionPointer>
		FunctionPointer getAddress(const char * const functionName);

		stdtstring bridgeDllName;
		HMODULE bridgeDllHandle;
		CreateNetObject createNetObject;
		ReleaseNetObject releaseNetObject;
		CallNetObject callNetObject;
		GetNetObjectProperty getNetObjectProperty;
		SetNetObjectProperty setNetObjectProperty;
		CallNetClass callNetClass;
		GetNetClassProperty getNetClassProperty;
		SetNetClassProperty setNetClassProperty;
		bool valid;
	};

	NativeProxyBase::Impl::Impl(const stdtstring &dllfilename)
		: bridgeDllName(dllfilename)
		, bridgeDllHandle(0)
		, createNetObject(0)
		, releaseNetObject(0)
		, callNetObject(0)
		, getNetObjectProperty(0)
		, setNetObjectProperty(0)
		, callNetClass(0)
		, getNetClassProperty(0)
		, setNetClassProperty(0)
		, valid(false)
	{
		init();
	}

	NativeProxyBase::Impl::~Impl()
	{
		if (bridgeDllHandle != 0)
			FreeLibrary(bridgeDllHandle);
	}

	void NativeProxyBase::Impl::retrieveFunctionAddresses()
	{
		createNetObject = getAddress < CreateNetObject >(CREATENETOBJECT);
		releaseNetObject = getAddress < ReleaseNetObject >(RELEASENETOBJECT);
		callNetObject = getAddress < CallNetObject >(CALLNETOBJECT);
		getNetObjectProperty = getAddress < GetNetObjectProperty >(GETNETOBJECTPROPERTY);
		setNetObjectProperty = getAddress < SetNetObjectProperty >(SETNETOBJECTPROPERTY);
		callNetClass = getAddress < CallNetClass >(CALLNETCLASS);
		getNetClassProperty = getAddress < GetNetClassProperty >(GETNETCLASSPROPERTY);
		setNetClassProperty = getAddress < SetNetClassProperty >(SETNETCLASSPROPERTY);
	}

	template<class FunctionPointer>
	FunctionPointer NativeProxyBase::Impl::getAddress(const char * const functionName)
	{
		return reinterpret_cast<FunctionPointer>(GetProcAddress(bridgeDllHandle, functionName));
	}

	void NativeProxyBase::Impl::checkFunctionAddresses()
	{
		if (!createNetObject
			|| !releaseNetObject
			|| !callNetObject
			|| !getNetObjectProperty
			|| !setNetObjectProperty
			|| !callNetClass
			|| !getNetClassProperty
			|| !setNetClassProperty
			)
		{
			FreeLibrary(bridgeDllHandle);
			throw std::invalid_argument("Bridgefunction not found!");
		}
	}

	void NativeProxyBase::Impl::loadLibrary()
	{
		if (bridgeDllHandle != 0)
			throw std::invalid_argument("Bridge library alread loaded!");

		stdtstring fileName(ModuleSubDirectory);

		fileName += _T("\\");
		fileName += bridgeDllName;

		bridgeDllHandle = ::LoadLibrary(fileName.c_str());
		if (!bridgeDllHandle)
			bridgeDllHandle = ::LoadLibrary(bridgeDllName.c_str());

		if (bridgeDllHandle == 0)
			throw std::invalid_argument("Cannot load bridge library!");
	}

	void NativeProxyBase::Impl::init()
	{
		loadLibrary();
		retrieveFunctionAddresses();
		checkFunctionAddresses();
		valid = true;
	}

	NativeProxyBase::NativeProxyBase(void)
		: _pImpl(new Impl(BridgeDll))
	{
	}

	NativeProxyBase::NativeProxyBase(const stdtstring &bridgeDllName)
		: _pImpl(new Impl(bridgeDllName))
	{
	}

	NativeProxyBase::~NativeProxyBase(void)
	{
		delete _pImpl;
	}

	bool NativeProxyBase::isValid() const
	{
		return _pImpl->valid;
	}

	void *NativeProxyBase::createManagedClass(const stdtstring &assemblyFile, const stdtstring &className)
	{
		AnyTypeArray args;
		return createManagedClass(assemblyFile, className, args);
	}

	void *NativeProxyBase::createManagedClass(const stdtstring &assemblyFile, const stdtstring &className, const AnyTypeArray &parameters)
	{
		if (isValid())
		{
			Nequeo::StringEx stringEx;
			std::wstring exceptionText;
			if (void *classPointer = _pImpl->createNetObject(assemblyFile, className, parameters, exceptionText))
				return classPointer;
			else
				throw Nequeo::Exception(stringEx.ToAsciiString(exceptionText));
		}
		else
			throw std::invalid_argument("Not initialized!");
	}

	void NativeProxyBase::destroyManagedClass(void *classPointer)
	{
		if (isValid())
		{
			Nequeo::StringEx stringEx;
			std::wstring exceptionText;
			if (!_pImpl->releaseNetObject(classPointer, exceptionText))
				throw Nequeo::Exception(stringEx.ToAsciiString(exceptionText));
		}
		else
			throw std::invalid_argument("Not initialized!");
	}

	void NativeProxyBase::execute(void *classPointer, const stdtstring &functionName, AnyTypeArray &parameters, AnyType &result)
	{
		if (isValid() && classPointer)
		{
			Nequeo::StringEx stringEx;
			std::wstring exceptionText;
			if (!_pImpl->callNetObject(classPointer, functionName, parameters, result, exceptionText))
				throw Nequeo::Exception(stringEx.ToAsciiString(exceptionText));
		}
		else
			throw std::invalid_argument("Not initialized!");
	}

	void NativeProxyBase::setProperty(void *classPointer, const stdtstring &propertyName, const AnyType & value)
	{
		if (isValid() && classPointer)
		{
			Nequeo::StringEx stringEx;
			std::wstring exceptionText;
			if (!_pImpl->setNetObjectProperty(classPointer, propertyName, value, exceptionText))
				throw Nequeo::Exception(stringEx.ToAsciiString(exceptionText));
		}
		else
			throw std::invalid_argument("Not initialized!");
	}

	void NativeProxyBase::getProperty(void *classPointer, const stdtstring &propertyName, AnyType & value)
	{
		if (isValid() && classPointer)
		{
			Nequeo::StringEx stringEx;
			std::wstring exceptionText;
			if (!_pImpl->getNetObjectProperty(classPointer, propertyName, value, exceptionText))
				throw Nequeo::Exception(stringEx.ToAsciiString(exceptionText));
		}
		else
			throw std::invalid_argument("Not initialized!");
	}

	void NativeProxyBase::executeStatic(const stdtstring &assemblyFile, const stdtstring &className, const stdtstring &functionName, AnyTypeArray &parameters, AnyType &result)
	{
		if (isValid())
		{
			Nequeo::StringEx stringEx;
			std::wstring exceptionText;
			if (!_pImpl->callNetClass(assemblyFile, className, functionName, parameters, result, exceptionText))
				throw Nequeo::Exception(stringEx.ToAsciiString(exceptionText));
		}
		else
			throw std::invalid_argument("Not initialized!");
	}

	void NativeProxyBase::setPropertyStatic(const stdtstring &assemblyFile, const stdtstring &className, const stdtstring &propertyName, const AnyType & value)
	{
		if (isValid())
		{
			Nequeo::StringEx stringEx;
			std::wstring exceptionText;
			if (!_pImpl->setNetClassProperty(assemblyFile, className, propertyName, value, exceptionText))
				throw Nequeo::Exception(stringEx.ToAsciiString(exceptionText));
		}
		else
			throw std::invalid_argument("Not initialized!");
	}

	void NativeProxyBase::getPropertyStatic(const stdtstring &assemblyFile, const stdtstring &className, const stdtstring &propertyName, AnyType &value)
	{
		if (isValid())
		{
			Nequeo::StringEx stringEx;
			std::wstring exceptionText;
			if (!_pImpl->getNetClassProperty(assemblyFile, className, propertyName, value, exceptionText))
				throw Nequeo::Exception(stringEx.ToAsciiString(exceptionText));
		}
		else
			throw std::invalid_argument("Not initialized!");
	}
}
