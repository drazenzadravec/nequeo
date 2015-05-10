/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ByteOrder.h
*  Purpose :       ByteOrder class.
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

#ifndef _BYTEORDER_H
#define _BYTEORDER_H

#include "Global.h"
#include "Types.h"

namespace Nequeo
{
	/// This class contains a number of static methods
	/// to convert between big-endian and little-endian
	/// integers of various sizes.
	class ByteOrder
	{
	public:
		static Int16 flipBytes(Int16 value);
		static UInt16 flipBytes(UInt16 value);
		static Int32 flipBytes(Int32 value);
		static UInt32 flipBytes(UInt32 value);
#if defined(NEQUEO_HAVE_INT64)
		static Int64 flipBytes(Int64 value);
		static UInt64 flipBytes(UInt64 value);
#endif

		static Int16 toBigEndian(Int16 value);
		static UInt16 toBigEndian(UInt16 value);
		static Int32 toBigEndian(Int32 value);
		static UInt32 toBigEndian(UInt32 value);
#if defined(NEQUEO_HAVE_INT64)
		static Int64 toBigEndian(Int64 value);
		static UInt64 toBigEndian(UInt64 value);
#endif

		static Int16 fromBigEndian(Int16 value);
		static UInt16 fromBigEndian(UInt16 value);
		static Int32 fromBigEndian(Int32 value);
		static UInt32 fromBigEndian(UInt32 value);
#if defined(NEQUEO_HAVE_INT64)
		static Int64 fromBigEndian(Int64 value);
		static UInt64 fromBigEndian(UInt64 value);
#endif

		static Int16 toLittleEndian(Int16 value);
		static UInt16 toLittleEndian(UInt16 value);
		static Int32 toLittleEndian(Int32 value);
		static UInt32 toLittleEndian(UInt32 value);
#if defined(NEQUEO_HAVE_INT64)
		static Int64 toLittleEndian(Int64 value);
		static UInt64 toLittleEndian(UInt64 value);
#endif

		static Int16 fromLittleEndian(Int16 value);
		static UInt16 fromLittleEndian(UInt16 value);
		static Int32 fromLittleEndian(Int32 value);
		static UInt32 fromLittleEndian(UInt32 value);
#if defined(NEQUEO_HAVE_INT64)
		static Int64 fromLittleEndian(Int64 value);
		static UInt64 fromLittleEndian(UInt64 value);
#endif

		static Int16 toNetwork(Int16 value);
		static UInt16 toNetwork(UInt16 value);
		static Int32 toNetwork(Int32 value);
		static UInt32 toNetwork(UInt32 value);
#if defined(NEQUEO_HAVE_INT64)
		static Int64 toNetwork(Int64 value);
		static UInt64 toNetwork(UInt64 value);
#endif

		static Int16 fromNetwork(Int16 value);
		static UInt16 fromNetwork(UInt16 value);
		static Int32 fromNetwork(Int32 value);
		static UInt32 fromNetwork(UInt32 value);
#if defined(NEQUEO_HAVE_INT64)
		static Int64 fromNetwork(Int64 value);
		static UInt64 fromNetwork(UInt64 value);
#endif
	};


	//
	// inlines
	//
	inline UInt16 ByteOrder::flipBytes(UInt16 value)
	{
		return ((value >> 8) & 0x00FF) | ((value << 8) & 0xFF00);
	}


	inline Int16 ByteOrder::flipBytes(Int16 value)
	{
		return Int16(flipBytes(UInt16(value)));
	}


	inline UInt32 ByteOrder::flipBytes(UInt32 value)
	{
		return ((value >> 24) & 0x000000FF) | ((value >> 8) & 0x0000FF00)
			| ((value << 8) & 0x00FF0000) | ((value << 24) & 0xFF000000);
	}


	inline Int32 ByteOrder::flipBytes(Int32 value)
	{
		return Int32(flipBytes(UInt32(value)));
	}


#if defined(NEQUEO_HAVE_INT64)
	inline UInt64 ByteOrder::flipBytes(UInt64 value)
	{
		UInt32 hi = UInt32(value >> 32);
		UInt32 lo = UInt32(value & 0xFFFFFFFF);
		return UInt64(flipBytes(hi)) | (UInt64(flipBytes(lo)) << 32);
	}


	inline Int64 ByteOrder::flipBytes(Int64 value)
	{
		return Int64(flipBytes(UInt64(value)));
	}
#endif // POCO_HAVE_INT64


	//
	// some macro trickery to automate the method implementation
	//
#define NEQUEO_IMPLEMENT_BYTEORDER_NOOP_(op, type) \
	inline type ByteOrder::op(type value)		\
			{											\
		return value;							\
			}
#define NEQUEO_IMPLEMENT_BYTEORDER_FLIP_(op, type) \
	inline type ByteOrder::op(type value)		\
			{											\
		return flipBytes(value);				\
			}


#if defined(NEQUEO_HAVE_INT64)
#define NEQUEO_IMPLEMENT_BYTEORDER_NOOP(op) \
		NEQUEO_IMPLEMENT_BYTEORDER_NOOP_(op, Int16)	\
		NEQUEO_IMPLEMENT_BYTEORDER_NOOP_(op, UInt16)	\
		NEQUEO_IMPLEMENT_BYTEORDER_NOOP_(op, Int32)	\
		NEQUEO_IMPLEMENT_BYTEORDER_NOOP_(op, UInt32)	\
		NEQUEO_IMPLEMENT_BYTEORDER_NOOP_(op, Int64)	\
		NEQUEO_IMPLEMENT_BYTEORDER_NOOP_(op, UInt64)
#define NEQUEO_IMPLEMENT_BYTEORDER_FLIP(op) \
		NEQUEO_IMPLEMENT_BYTEORDER_FLIP_(op, Int16)	\
		NEQUEO_IMPLEMENT_BYTEORDER_FLIP_(op, UInt16)	\
		NEQUEO_IMPLEMENT_BYTEORDER_FLIP_(op, Int32)	\
		NEQUEO_IMPLEMENT_BYTEORDER_FLIP_(op, UInt32)	\
		NEQUEO_IMPLEMENT_BYTEORDER_FLIP_(op, Int64)	\
		NEQUEO_IMPLEMENT_BYTEORDER_FLIP_(op, UInt64)
#else
#define NEQUEO_IMPLEMENT_BYTEORDER_NOOP(op) \
		NEQUEO_IMPLEMENT_BYTEORDER_NOOP_(op, Int16)	\
		NEQUEO_IMPLEMENT_BYTEORDER_NOOP_(op, UInt16)	\
		NEQUEO_IMPLEMENT_BYTEORDER_NOOP_(op, Int32)	\
		NEQUEO_IMPLEMENT_BYTEORDER_NOOP_(op, UInt32)
#define NEQUEO_IMPLEMENT_BYTEORDER_FLIP(op) \
		NEQUEO_IMPLEMENT_BYTEORDER_FLIP_(op, Int16)	\
		NEQUEO_IMPLEMENT_BYTEORDER_FLIP_(op, UInt16)	\
		NEQUEO_IMPLEMENT_BYTEORDER_FLIP_(op, Int32)	\
		NEQUEO_IMPLEMENT_BYTEORDER_FLIP_(op, UInt32)
#endif


#if defined(NEQUEO_ARCH_BIG_ENDIAN)
#define NEQUEO_IMPLEMENT_BYTEORDER_BIG NEQUEO_IMPLEMENT_BYTEORDER_NOOP
#define NEQUEO_IMPLEMENT_BYTEORDER_LIT NEQUEO_IMPLEMENT_BYTEORDER_FLIP
#else
#define NEQUEO_IMPLEMENT_BYTEORDER_BIG NEQUEO_IMPLEMENT_BYTEORDER_FLIP
#define NEQUEO_IMPLEMENT_BYTEORDER_LIT NEQUEO_IMPLEMENT_BYTEORDER_NOOP
#endif

	NEQUEO_IMPLEMENT_BYTEORDER_BIG(toBigEndian)
	NEQUEO_IMPLEMENT_BYTEORDER_BIG(fromBigEndian)
	NEQUEO_IMPLEMENT_BYTEORDER_BIG(toNetwork)
	NEQUEO_IMPLEMENT_BYTEORDER_BIG(fromNetwork)
	NEQUEO_IMPLEMENT_BYTEORDER_LIT(toLittleEndian)
	NEQUEO_IMPLEMENT_BYTEORDER_LIT(fromLittleEndian)
}
#endif