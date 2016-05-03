/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NativeProxy.h
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

#pragma once

#ifndef _NATIVEPROXY_H
#define _NATIVEPROXY_H

#include "Global.h"

#include "NativeProxyBase.h"
#include "Types.h"
#include "String.h"
#include "Exceptions\Exception.h"

using namespace Nequeo::System::Any;

namespace Nequeo
{
	/// <summary>
	/// Native library loader.
	/// </summary>
	class NativeProxy : public NativeProxyBase
	{
	public:
		/// <summary>
		/// Native library loader.
		/// </summary>
		NativeProxy(const stdtstring &dllName, const stdtstring &className);
		NativeProxy(const stdtstring &dllName, const stdtstring &className, const AnyTypeArray &ctorParams);
		virtual ~NativeProxy();

		void executeManaged(const stdtstring &function);
		void executeManaged(const stdtstring &function, AnyTypeArray &parameters);
		void executeManaged(const stdtstring &function, AnyType &result);
		void executeManaged(const stdtstring &function, AnyTypeArray &parameters, AnyType &result);

		void operator()(const stdtstring &function);
		void operator()(const stdtstring &function, AnyTypeArray &parameters);
		void operator()(const stdtstring &function, AnyType &result);
		void operator()(const stdtstring &function, AnyTypeArray &parameters, AnyType &result);

		void set(const stdtstring &property, const AnyType &value);
		void get(const stdtstring &property, AnyType &value);

	private:
		NativeProxy(const NativeProxy &);
		NativeProxy &operator=(const NativeProxy &);
		struct impl;
		impl *_pimpl;

	};
}
#endif