/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Types.h
*  Purpose :       Data type definitions.
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

#ifndef _TYPES_H
#define _TYPES_H

#include "Global.h"

namespace Nequeo
{
	typedef signed char				Int8;
	typedef unsigned char			UInt8;
	typedef signed short			Int16;
	typedef unsigned short			UInt16;
	typedef signed int				Int32;
	typedef unsigned int			UInt32;
	typedef signed __int64			Int64;
	typedef unsigned __int64		UInt64;
#if defined(_WIN64)
#define NEQUEO_PTR_IS_64_BIT 1
	typedef signed __int64			IntPtr;
	typedef unsigned __int64		UIntPtr;
#else
	typedef signed long				IntPtr;
	typedef unsigned long			UIntPtr;
#endif
#define NEQUEO_HAVE_INT64 1
}

namespace Nequeo {
	namespace System {
		typedef unsigned char byte;
		typedef signed char sbyte;
	}
}
#endif