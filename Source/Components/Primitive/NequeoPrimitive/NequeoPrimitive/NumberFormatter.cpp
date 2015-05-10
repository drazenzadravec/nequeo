/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NumberFormatter.cpp
*  Purpose :       NumberFormatter class.
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

#include "NumberFormatter.h"
#include "IO\MemoryOutputStream.h"

void Nequeo::Primitive::NumberFormatter::append(std::string& str, int value)
{
	char buffer[64];
	sprintf_s(buffer, "%d", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, int value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%*d", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::append0(std::string& str, int value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*d", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, int value)
{
	char buffer[64];
	sprintf_s(buffer, "%X", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, int value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*X", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, unsigned value)
{
	char buffer[64];
	sprintf_s(buffer, "%u", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, unsigned value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%*u", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::append0(std::string& str, unsigned int value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*u", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, unsigned value)
{
	char buffer[64];
	sprintf_s(buffer, "%X", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, unsigned value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*X", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, long value)
{
	char buffer[64];
	sprintf_s(buffer, "%ld", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, long value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%*ld", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::append0(std::string& str, long value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*ld", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, long value)
{
	char buffer[64];
	sprintf_s(buffer, "%lX", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, long value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*lX", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, unsigned long value)
{
	char buffer[64];
	sprintf_s(buffer, "%lu", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, unsigned long value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%*lu", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::append0(std::string& str, unsigned long value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*lu", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, unsigned long value)
{
	char buffer[64];
	sprintf_s(buffer, "%lX", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, unsigned long value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*lX", width, value);
		str.append(buffer);
	}
}


#if defined(NEQUEO_HAVE_INT64) && !defined(NEQUEO_LONG_IS_64_BIT)


void Nequeo::Primitive::NumberFormatter::append(std::string& str, Int64 value)
{
	char buffer[64];
	sprintf_s(buffer, "%I64d", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, Int64 value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%*I64d", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::append0(std::string& str, Int64 value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*I64d", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, Int64 value)
{
	char buffer[64];
	sprintf_s(buffer, "%I64X", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, Int64 value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*I64X", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, UInt64 value)
{
	char buffer[64];
	sprintf_s(buffer, "%I64u", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, UInt64 value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%*I64u", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::append0(std::string& str, UInt64 value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*I64u", width, value);
		str.append(buffer);
	}
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, UInt64 value)
{
	char buffer[64];
	sprintf_s(buffer, "%I64X", value);
	str.append(buffer);
}


void Nequeo::Primitive::NumberFormatter::appendHex(std::string& str, UInt64 value, int width)
{
	if (width > 0 && width < 64)
	{
		char buffer[64];
		sprintf_s(buffer, "%0*I64X", width, value);
		str.append(buffer);
	}
}


#endif


void Nequeo::Primitive::NumberFormatter::append(std::string& str, float value)
{
	char buffer[64];
	Nequeo::IO::MemoryOutputStream ostr(buffer, sizeof(buffer));
	#if !defined(NEQUEO_NO_LOCALE)
		ostr.imbue(std::locale::classic());
	#endif
	ostr << std::setprecision(8) << value;
	str.append(buffer, static_cast<std::string::size_type>(ostr.charsWritten()));
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, double value)
{
	char buffer[64];
	Nequeo::IO::MemoryOutputStream ostr(buffer, sizeof(buffer));
	#if !defined(NEQUEO_NO_LOCALE)
		ostr.imbue(std::locale::classic());
	#endif
	ostr << std::setprecision(16) << value;
	str.append(buffer, static_cast<std::string::size_type>(ostr.charsWritten()));
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, double value, int precision)
{
	if (precision >= 0 && precision < 32)
	{
		char buffer[64];
		Nequeo::IO::MemoryOutputStream ostr(buffer, sizeof(buffer));
		#if !defined(NEQUEO_NO_LOCALE)
			ostr.imbue(std::locale::classic());
		#endif
		ostr << std::fixed << std::showpoint << std::setprecision(precision) << value;
		str.append(buffer, static_cast<std::string::size_type>(ostr.charsWritten()));
	}
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, double value, int width, int precision)
{
	if (width > 0 && width < 64 && precision >= 0 && precision < width)
	{
		char buffer[64];
		Nequeo::IO::MemoryOutputStream ostr(buffer, sizeof(buffer));
		#if !defined(NEQUEO_NO_LOCALE)
			ostr.imbue(std::locale::classic());
		#endif
		ostr << std::fixed << std::showpoint << std::setw(width) << std::setprecision(precision) << value;
		str.append(buffer, static_cast<std::string::size_type>(ostr.charsWritten()));
	}
}


void Nequeo::Primitive::NumberFormatter::append(std::string& str, const void* ptr)
{
	char buffer[24];
	#if defined(NEQUEO_PTR_IS_64_BIT)
		#if defined(NEQUEO_LONG_IS_64_BIT)
			sprintf_s(buffer, "%016lX", (UIntPtr)ptr);
		#else
			sprintf_s(buffer, "%016I64X", (UIntPtr)ptr);
		#endif
	#else
		sprintf_s(buffer, "%08lX", (UIntPtr)ptr);
	#endif
	str.append(buffer);
}

//
// inlines
//
inline std::string Nequeo::Primitive::NumberFormatter::format(int value)
{
	std::string result;
	append(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(int value, int width)
{
	std::string result;
	append(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format0(int value, int width)
{
	std::string result;
	append0(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(int value)
{
	std::string result;
	appendHex(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(int value, int width)
{
	std::string result;
	appendHex(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(unsigned value)
{
	std::string result;
	append(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(unsigned value, int width)
{
	std::string result;
	append(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format0(unsigned int value, int width)
{
	std::string result;
	append0(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(unsigned value)
{
	std::string result;
	appendHex(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(unsigned value, int width)
{
	std::string result;
	appendHex(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(long value)
{
	std::string result;
	append(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(long value, int width)
{
	std::string result;
	append(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format0(long value, int width)
{
	std::string result;
	append0(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(long value)
{
	std::string result;
	appendHex(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(long value, int width)
{
	std::string result;
	appendHex(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(unsigned long value)
{
	std::string result;
	append(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(unsigned long value, int width)
{
	std::string result;
	append(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format0(unsigned long value, int width)
{
	std::string result;
	append0(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(unsigned long value)
{
	std::string result;
	appendHex(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(unsigned long value, int width)
{
	std::string result;
	appendHex(result, value, width);
	return result;
}


#if defined(NEQUEO_HAVE_INT64) && !defined(NEQUEO_LONG_IS_64_BIT)


inline std::string Nequeo::Primitive::NumberFormatter::format(Int64 value)
{
	std::string result;
	append(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(Int64 value, int width)
{
	std::string result;
	append(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format0(Int64 value, int width)
{
	std::string result;
	append0(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(Int64 value)
{
	std::string result;
	appendHex(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(Int64 value, int width)
{
	std::string result;
	appendHex(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(UInt64 value)
{
	std::string result;
	append(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(UInt64 value, int width)
{
	std::string result;
	append(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format0(UInt64 value, int width)
{
	std::string result;
	append0(result, value, width);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(UInt64 value)
{
	std::string result;
	appendHex(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::formatHex(UInt64 value, int width)
{
	std::string result;
	appendHex(result, value, width);
	return result;
}


#endif


inline std::string Nequeo::Primitive::NumberFormatter::format(float value)
{
	std::string result;
	append(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(double value)
{
	std::string result;
	append(result, value);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(double value, int precision)
{
	std::string result;
	append(result, value, precision);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(double value, int width, int precision)
{
	std::string result;
	append(result, value, width, precision);
	return result;
}


inline std::string Nequeo::Primitive::NumberFormatter::format(const void* ptr)
{
	std::string result;
	append(result, ptr);
	return result;
}