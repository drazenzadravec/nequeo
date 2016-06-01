/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NativeProxyBase.h
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

#pragma once

#ifndef _NATIVEPROXYBASE_H
#define _NATIVEPROXYBASE_H

#include "Global.h"
#include "Types.h"
#include "StringEx.h"
#include "NativeHandler.h"
#include "Exceptions\Exception.h"

using namespace Nequeo::System::Any;

namespace Nequeo
{
	/// <summary>
	/// Native library loader.
	/// </summary>
	class NativeProxyBase
	{
	public:
		/// <summary>
		/// Native library loader.
		/// </summary>
		NativeProxyBase();
		virtual ~NativeProxyBase();
		explicit NativeProxyBase(const stdtstring &bridgeDllName);

		bool isValid() const;
		void *createManagedClass(const stdtstring &assemblyFile, const stdtstring &className);
		void *createManagedClass(const stdtstring &assemblyFile, const stdtstring &className, const AnyTypeArray &parameters);
		void destroyManagedClass(void *classPointer);
		// instance 
		void execute(void *classPointer, const stdtstring &functionName, AnyTypeArray &parameters, AnyType &result);
		void setProperty(void *classPointer, const stdtstring &propertyName, const AnyType & value);
		void getProperty(void *classPointer, const stdtstring &propertyName, AnyType &value);
		// static 
		void executeStatic(const stdtstring &assemblyFile, const stdtstring &className, const stdtstring &functionName, AnyTypeArray &parameters, AnyType &result);
		void setPropertyStatic(const stdtstring &assemblyFile, const stdtstring &className, const stdtstring &propertyName, const AnyType & value);
		void getPropertyStatic(const stdtstring &assemblyFile, const stdtstring &className, const stdtstring &propertyName, AnyType &value);

	private:
		NativeProxyBase(const NativeProxyBase&);
		NativeProxyBase &operator=(const NativeProxyBase&);

		struct Impl;
		Impl *_pImpl;

	};
}
#endif