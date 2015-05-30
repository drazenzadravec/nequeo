/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NumberFormatter.h
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

#pragma once

#ifndef _NUMBERFORMATTER_H
#define _NUMBERFORMATTER_H

#include "GlobalPrimitive.h"

#include "Base\Types.h"

namespace Nequeo {
	namespace Primitive
	{
		/// The NumberFormatter class provides static methods
		/// for formatting numeric values into strings.
		///
		/// There are two kind of static member functions:
		///    * format* functions return a std::string containing
		///      the formatted value.
		///    * append* functions append the formatted value to
		///      an existing string.
		///
		/// Internally, std::sprintf() is used to do the actual
		/// formatting.
		class NumberFormatter
		{
		public:
			static std::string format(int value);
			/// Formats an integer value in decimal notation.

			static std::string format(int value, int width);
			/// Formats an integer value in decimal notation,
			/// right justified in a field having at least
			/// the specified width.

			static std::string format0(int value, int width);
			/// Formats an integer value in decimal notation, 
			/// right justified and zero-padded in a field
			/// having at least the specified width.

			static std::string formatHex(int value);
			/// Formats an int value in hexadecimal notation.
			/// The value is treated as unsigned.

			static std::string formatHex(int value, int width);
			/// Formats a int value in hexadecimal notation,
			/// right justified and zero-padded in
			/// a field having at least the specified width.
			/// The value is treated as unsigned.

			static std::string format(unsigned value);
			/// Formats an unsigned int value in decimal notation.

			static std::string format(unsigned value, int width);
			/// Formats an unsigned long int in decimal notation,
			/// right justified in a field having at least the
			/// specified width.

			static std::string format0(unsigned int value, int width);
			/// Formats an unsigned int value in decimal notation, 
			/// right justified and zero-padded in a field having at 
			/// least the specified width.

			static std::string formatHex(unsigned value);
			/// Formats an unsigned int value in hexadecimal notation.

			static std::string formatHex(unsigned value, int width);
			/// Formats a int value in hexadecimal notation,
			/// right justified and zero-padded in
			/// a field having at least the specified width.

			static std::string format(long value);
			/// Formats a long value in decimal notation.

			static std::string format(long value, int width);
			/// Formats a long value in decimal notation,
			/// right justified in a field having at least the 
			/// specified width.

			static std::string format0(long value, int width);
			/// Formats a long value in decimal notation, 
			/// right justified and zero-padded in a field
			/// having at least the specified width.

			static std::string formatHex(long value);
			/// Formats an unsigned long value in hexadecimal notation.
			/// The value is treated as unsigned.

			static std::string formatHex(long value, int width);
			/// Formats an unsigned long value in hexadecimal notation,
			/// right justified and zero-padded in a field having at least the 
			/// specified width.
			/// The value is treated as unsigned.

			static std::string format(unsigned long value);
			/// Formats an unsigned long value in decimal notation.

			static std::string format(unsigned long value, int width);
			/// Formats an unsigned long value in decimal notation,
			/// right justified in a field having at least the specified 
			/// width.

			static std::string format0(unsigned long value, int width);
			/// Formats an unsigned long value in decimal notation, 
			/// right justified and zero-padded
			/// in a field having at least the specified width.

			static std::string formatHex(unsigned long value);
			/// Formats an unsigned long value in hexadecimal notation.

			static std::string formatHex(unsigned long value, int width);
			/// Formats an unsigned long value in hexadecimal notation,
			/// right justified and zero-padded in a field having at least the 
			/// specified width.

#if defined(NEQUEO_HAVE_INT64) && !defined(NEQUEO_LONG_IS_64_BIT)

			static std::string format(Int64 value);
			/// Formats a 64-bit integer value in decimal notation.

			static std::string format(Int64 value, int width);
			/// Formats a 64-bit integer value in decimal notation,
			/// right justified in a field having at least the specified width.

			static std::string format0(Int64 value, int width);
			/// Formats a 64-bit integer value in decimal notation, 
			/// right justified and zero-padded in a field having at least 
			/// the specified width.

			static std::string formatHex(Int64 value);
			/// Formats a 64-bit integer value in hexadecimal notation.
			/// The value is treated as unsigned.

			static std::string formatHex(Int64 value, int width);
			/// Formats a 64-bit integer value in hexadecimal notation,
			/// right justified and zero-padded in a field having at least 
			/// the specified width.
			/// The value is treated as unsigned.

			static std::string format(UInt64 value);
			/// Formats an unsigned 64-bit integer value in decimal notation.

			static std::string format(UInt64 value, int width);
			/// Formats an unsigned 64-bit integer value in decimal notation,
			/// right justified in a field having at least the specified width.

			static std::string format0(UInt64 value, int width);
			/// Formats an unsigned 64-bit integer value in decimal notation, 
			/// right justified and zero-padded in a field having at least the 
			/// specified width.

			static std::string formatHex(UInt64 value);
			/// Formats a 64-bit integer value in hexadecimal notation.

			static std::string formatHex(UInt64 value, int width);
			/// Formats a 64-bit integer value in hexadecimal notation,
			/// right justified and zero-padded in a field having at least 
			/// the specified width.

#endif

			static std::string format(float value);
			/// Formats a float value in decimal floating-point notation,
			/// according to std::printf's %g format with a precision of 8 fractional digits.

			static std::string format(double value);
			/// Formats a double value in decimal floating-point notation,
			/// according to std::printf's %g format with a precision of 16 fractional digits.

			static std::string format(double value, int precision);
			/// Formats a double value in decimal floating-point notation,
			/// according to std::printf's %f format with the given precision.

			static std::string format(double value, int width, int precision);
			/// Formats a double value in decimal floating-point notation,
			/// right justified in a field of the specified width,
			/// with the number of fractional digits given in precision.

			static std::string format(const void* ptr);
			/// Formats a pointer in an eight (32-bit architectures) or
			/// sixteen (64-bit architectures) characters wide
			/// field in hexadecimal notation.

			static void append(std::string& str, int value);
			/// Formats an integer value in decimal notation.

			static void append(std::string& str, int value, int width);
			/// Formats an integer value in decimal notation,
			/// right justified in a field having at least
			/// the specified width.

			static void append0(std::string& str, int value, int width);
			/// Formats an integer value in decimal notation, 
			/// right justified and zero-padded in a field
			/// having at least the specified width.

			static void appendHex(std::string& str, int value);
			/// Formats an int value in hexadecimal notation.
			/// The value is treated as unsigned.

			static void appendHex(std::string& str, int value, int width);
			/// Formats a int value in hexadecimal notation,
			/// right justified and zero-padded in
			/// a field having at least the specified width.
			/// The value is treated as unsigned.

			static void append(std::string& str, unsigned value);
			/// Formats an unsigned int value in decimal notation.

			static void append(std::string& str, unsigned value, int width);
			/// Formats an unsigned long int in decimal notation,
			/// right justified in a field having at least the
			/// specified width.

			static void append0(std::string& str, unsigned int value, int width);
			/// Formats an unsigned int value in decimal notation, 
			/// right justified and zero-padded in a field having at 
			/// least the specified width.

			static void appendHex(std::string& str, unsigned value);
			/// Formats an unsigned int value in hexadecimal notation.

			static void appendHex(std::string& str, unsigned value, int width);
			/// Formats a int value in hexadecimal notation,
			/// right justified and zero-padded in
			/// a field having at least the specified width.

			static void append(std::string& str, long value);
			/// Formats a long value in decimal notation.

			static void append(std::string& str, long value, int width);
			/// Formats a long value in decimal notation,
			/// right justified in a field having at least the 
			/// specified width.

			static void append0(std::string& str, long value, int width);
			/// Formats a long value in decimal notation, 
			/// right justified and zero-padded in a field
			/// having at least the specified width.

			static void appendHex(std::string& str, long value);
			/// Formats an unsigned long value in hexadecimal notation.
			/// The value is treated as unsigned.

			static void appendHex(std::string& str, long value, int width);
			/// Formats an unsigned long value in hexadecimal notation,
			/// right justified and zero-padded in a field having at least the 
			/// specified width.
			/// The value is treated as unsigned.

			static void append(std::string& str, unsigned long value);
			/// Formats an unsigned long value in decimal notation.

			static void append(std::string& str, unsigned long value, int width);
			/// Formats an unsigned long value in decimal notation,
			/// right justified in a field having at least the specified 
			/// width.

			static void append0(std::string& str, unsigned long value, int width);
			/// Formats an unsigned long value in decimal notation, 
			/// right justified and zero-padded
			/// in a field having at least the specified width.

			static void appendHex(std::string& str, unsigned long value);
			/// Formats an unsigned long value in hexadecimal notation.

			static void appendHex(std::string& str, unsigned long value, int width);
			/// Formats an unsigned long value in hexadecimal notation,
			/// right justified and zero-padded in a field having at least the 
			/// specified width.

#if defined(NEQUEO_HAVE_INT64) && !defined(NEQUEO_LONG_IS_64_BIT)

			static void append(std::string& str, Int64 value);
			/// Formats a 64-bit integer value in decimal notation.

			static void append(std::string& str, Int64 value, int width);
			/// Formats a 64-bit integer value in decimal notation,
			/// right justified in a field having at least the specified width.

			static void append0(std::string& str, Int64 value, int width);
			/// Formats a 64-bit integer value in decimal notation, 
			/// right justified and zero-padded in a field having at least 
			/// the specified width.

			static void appendHex(std::string& str, Int64 value);
			/// Formats a 64-bit integer value in hexadecimal notation.
			/// The value is treated as unsigned.

			static void appendHex(std::string& str, Int64 value, int width);
			/// Formats a 64-bit integer value in hexadecimal notation,
			/// right justified and zero-padded in a field having at least 
			/// the specified width.
			/// The value is treated as unsigned.

			static void append(std::string& str, UInt64 value);
			/// Formats an unsigned 64-bit integer value in decimal notation.

			static void append(std::string& str, UInt64 value, int width);
			/// Formats an unsigned 64-bit integer value in decimal notation,
			/// right justified in a field having at least the specified width.

			static void append0(std::string& str, UInt64 value, int width);
			/// Formats an unsigned 64-bit integer value in decimal notation, 
			/// right justified and zero-padded in a field having at least the 
			/// specified width.

			static void appendHex(std::string& str, UInt64 value);
			/// Formats a 64-bit integer value in hexadecimal notation.

			static void appendHex(std::string& str, UInt64 value, int width);
			/// Formats a 64-bit integer value in hexadecimal notation,
			/// right justified and zero-padded in a field having at least 
			/// the specified width.

#endif

			static void append(std::string& str, float value);
			/// Formats a float value in decimal floating-point notation,
			/// according to std::printf's %g format with a precision of 8 fractional digits.

			static void append(std::string& str, double value);
			/// Formats a double value in decimal floating-point notation,
			/// according to std::printf's %g format with a precision of 16 fractional digits.

			static void append(std::string& str, double value, int precision);
			/// Formats a double value in decimal floating-point notation,
			/// according to std::printf's %f format with the given precision.

			static void append(std::string& str, double value, int width, int precision);
			/// Formats a double value in decimal floating-point notation,
			/// right justified in a field of the specified width,
			/// with the number of fractional digits given in precision.

			static void append(std::string& str, const void* ptr);
			/// Formats a pointer in an eight (32-bit architectures) or
			/// sixteen (64-bit architectures) characters wide
			/// field in hexadecimal notation.
		};
	}
}
#endif