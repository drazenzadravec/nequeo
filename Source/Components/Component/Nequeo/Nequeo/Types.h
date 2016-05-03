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

#include "boost\any.hpp"

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

namespace Nequeo {
	namespace System {
		namespace Any
		{
			typedef boost::any AnyType;
			typedef std::vector<boost::any> AnyTypeArray;
			typedef std::basic_string<TCHAR> stdtstring;

			typedef bool AnyBoolean;
			typedef __int64 AnyInt64;
			typedef __int32 AnyInt32;
			typedef __int16 AnyInt16;
			typedef __int8  AnyInt8;
			typedef unsigned __int64 AnyUInt64;
			typedef unsigned __int32 AnyUInt32;
			typedef unsigned __int16 AnyUInt16;
			typedef unsigned __int8  AnyUInt8;
			typedef long double AnyFloat128;
			typedef double AnyFloat64;
			typedef float AnyFloat32;
			typedef tm AnyDateTime;
			typedef char AnyAsciiChar;
			typedef wchar_t AnyUnicodeChar;
			typedef std::string AnyAsciiString;
			typedef std::wstring AnyUnicodeString;
			typedef std::vector<AnyUInt8> AnyByteArray;
			typedef std::vector<std::string> AnyAsciiStringArray;
			typedef std::vector<std::wstring> AnyUnicodeStringArray;
			typedef TCHAR AnyTChar;
			typedef std::vector<stdtstring> AnyTString;
			typedef std::vector<stdtstring> AnyTStringArray;

			inline AnyType anyFromBoolean(AnyBoolean init) { return AnyType(init); }
			inline AnyType anyFromInt64(AnyInt64 init) { return AnyType(init); }
			inline AnyType anyFromInt32(AnyInt32 init) { return AnyType(init); }
			inline AnyType anyFromInt16(AnyInt16 init) { return AnyType(init); }
			inline AnyType anyFromInt8(AnyInt8 init) { return AnyType(init); }
			inline AnyType anyFromUInt64(AnyUInt64 init) { return AnyType(init); }
			inline AnyType anyFromUInt32(AnyUInt32 init) { return AnyType(init); }
			inline AnyType anyFromUInt16(AnyUInt16 init) { return AnyType(init); }
			inline AnyType anyFromUInt8(AnyUInt8 init) { return AnyType(init); }
			inline AnyType anyFromFloat128(AnyFloat128 init) { return AnyType(init); }
			inline AnyType anyFromFloat64(AnyFloat64 init) { return AnyType(init); }
			inline AnyType anyFromFloat32(AnyFloat32 init) { return AnyType(init); }
			inline AnyType anyFromDateTime(const AnyDateTime &init) { return AnyType(init); }
			inline AnyType anyFromAsciiChar(AnyAsciiChar init) { return AnyType(init); }
			inline AnyType anyFromUnicodeChar(AnyUnicodeChar init) { return AnyType(init); }
			inline AnyType anyFromAsciiString(const AnyAsciiString &init) { return AnyType(init); }
			inline AnyType anyFromUnicodeString(const AnyUnicodeString &init) { return AnyType(init); }
			inline AnyType anyFromByteArray(const AnyByteArray &init) { return AnyType(init); }
			inline AnyType anyFromAsciiStringArray(const AnyAsciiStringArray &init) { return AnyType(init); }
			inline AnyType anyFromUnicodeStringArray(const AnyUnicodeStringArray &init) { return AnyType(init); }
			inline AnyType anyFromTString(const stdtstring &source) { return AnyType(source); }
			inline AnyType anyFromTChar(TCHAR source) { return AnyType(source); }
			inline AnyType anyFromTStringArray(const AnyTStringArray &source) { return AnyType(source); }

			inline AnyBoolean booleanFromAny(const AnyType &anyValue) { return boost::any_cast<AnyBoolean>(anyValue); }
			inline AnyInt64 int64FromAny(const AnyType &anyValue) { return boost::any_cast<AnyInt64>(anyValue); }
			inline AnyInt32 int32FromAny(const AnyType &anyValue) { return boost::any_cast<AnyInt32>(anyValue); }
			inline AnyInt16 int16FromAny(const AnyType &anyValue) { return boost::any_cast<AnyInt16>(anyValue); }
			inline AnyInt8 int8FromAny(const AnyType &anyValue) { return boost::any_cast<AnyInt8>(anyValue); }
			inline AnyUInt64 uInt64FromAny(const AnyType &anyValue) { return boost::any_cast<AnyUInt64>(anyValue); }
			inline AnyUInt32 uInt32FromAny(const AnyType &anyValue) { return boost::any_cast<AnyUInt32>(anyValue); }
			inline AnyUInt16 uInt16FromAny(const AnyType &anyValue) { return boost::any_cast<AnyUInt16>(anyValue); }
			inline AnyUInt8 uInt8FromAny(const AnyType &anyValue) { return boost::any_cast<AnyUInt8>(anyValue); }
			inline AnyFloat128 float128FromAny(const AnyType &anyValue) { return boost::any_cast<AnyFloat128>(anyValue); }
			inline AnyFloat64 float64FromAny(const AnyType &anyValue) { return boost::any_cast<AnyFloat64>(anyValue); }
			inline AnyFloat32 float32FromAny(const AnyType &anyValue) { return boost::any_cast<AnyFloat32>(anyValue); }
			inline const AnyDateTime dateTimeFromAny(const AnyType &anyValue) { return boost::any_cast<AnyDateTime>(anyValue); }
			inline AnyAsciiChar asciiCharFromAny(const AnyType &anyValue) { return boost::any_cast<AnyAsciiChar>(anyValue); }
			inline AnyUnicodeChar unicodeCharFromAny(const AnyType &anyValue) { return boost::any_cast<AnyUnicodeChar>(anyValue); }
			inline const AnyAsciiString asciiStringFromAny(const AnyType &anyValue) { return boost::any_cast<AnyAsciiString>(anyValue); }
			inline const AnyUnicodeString unicodeStringFromAny(const AnyType &anyValue) { return boost::any_cast<AnyUnicodeString>(anyValue); }
			inline const AnyByteArray byteArrayFromAny(const AnyType &anyValue) { return boost::any_cast<AnyByteArray>(anyValue); }
			inline const AnyUnicodeStringArray unicodeStringArrayFromAny(const AnyType &anyValue) { return boost::any_cast<AnyUnicodeStringArray>(anyValue); }
			inline const AnyAsciiStringArray asciiStringArrayFromAny(const AnyType &anyValue) { return boost::any_cast<AnyAsciiStringArray>(anyValue); }
			inline const AnyTString tStringFromAny(const AnyType &anyValue) { return boost::any_cast<AnyTString>(anyValue); }
			inline TCHAR tCharFromAny(const AnyType &anyValue) { return boost::any_cast<TCHAR>(anyValue); }
			inline const AnyTStringArray tStringArrayFromAny(const AnyType &anyValue) { return boost::any_cast<AnyTStringArray>(anyValue); }
			inline bool isAnyNull(const AnyType &data) { return data.empty(); }
		}
	}
}
#endif