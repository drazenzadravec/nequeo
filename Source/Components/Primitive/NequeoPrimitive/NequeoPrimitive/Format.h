/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Format.h
*  Purpose :       Format class.
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

#ifndef _FORMAT_H
#define _FORMAT_H

#include "GlobalPrimitive.h"
#include "Any.h"
#include <vector>

namespace Nequeo {
	namespace Primitive
	{
		std::string Format(const std::string& fmt, const Any& value);
		/// This function implements sprintf-style formatting in a typesafe way.
		/// Various variants of the function are available, supporting a
		/// different number of arguments (up to six).
		///
		/// The formatting is controlled by the format string in fmt.
		/// Format strings are quite similar to those of the std::printf() function, but
		/// there are some minor differences.
		///
		/// The format string can consist of any sequence of characters; certain
		/// characters have a special meaning. Characters without a special meaning
		/// are copied verbatim to the result. A percent sign (%) marks the beginning
		/// of a format specification. Format specifications have the following syntax:
		///
		///   %[<index>][<flags>][<width>][.<precision>][<modifier>]<type>
		///
		/// Index, flags, width, precision and prefix are optional. The only required part of
		/// the format specification, apart from the percent sign, is the type.
		///
		/// The optional index argument has the format "[<n>]" and allows to
		/// address an argument by its zero-based position (see the example below).
		///
		/// Following are valid type specifications and their meaning:
		///
		///   * b boolean (true = 1, false = 0)
		///   * c character
		///   * d signed decimal integer
		///   * i signed decimal integer
		///   * o unsigned octal integer
		///   * u unsigned decimal integer
		///   * x unsigned hexadecimal integer (lower case)
		///   * X unsigned hexadecimal integer (upper case)
		///   * e signed floating-point value in the form [-]d.dddde[<sign>]dd[d]
		///   * E signed floating-point value in the form [-]d.ddddE[<sign>]dd[d]
		///   * f signed floating-point value in the form [-]dddd.dddd
		///   * s std::string
		///   * z std::size_t
		///
		/// The following flags are supported:
		///
		///   * - left align the result within the given field width
		///   * + prefix the output value with a sign (+ or -) if the output value is of a signed type
		///   * 0 if width is prefixed with 0, zeros are added until the minimum width is reached
		///   * # For o, x, X, the # flag prefixes any nonzero output value with 0, 0x, or 0X, respectively; 
		///     for e, E, f, the # flag forces the output value to contain a decimal point in all cases.
		///
		/// The following modifiers are supported:
		///
		///   * (none) argument is char (c), int (d, i), unsigned (o, u, x, X) double (e, E, f, g, G) or string (s)
		///   * l      argument is long (d, i), unsigned long (o, u, x, X) or long double (e, E, f, g, G)
		///   * L      argument is long long (d, i), unsigned long long (o, u, x, X)
		///   * h      argument is short (d, i), unsigned short (o, u, x, X) or float (e, E, f, g, G)
		///   * ?      argument is any signed or unsigned int, short, long, or 64-bit integer (d, i, o, x, X)
		///
		/// The width argument is a nonnegative decimal integer controlling the minimum number of characters printed.
		/// If the number of characters in the output value is less than the specified width, blanks or
		/// leading zeros are added, according to the specified flags (-, +, 0).
		///
		/// Precision is a nonnegative decimal integer, preceded by a period (.), which specifies the number of characters 
		/// to be printed, the number of decimal places, or the number of significant digits.
		///
		/// Throws an InvalidArgumentException if an argument index is out of range.
		///
		/// Starting with release 1.4.3, an argument that does not match the format
		/// specifier no longer results in a BadCastException. The string [ERRFMT] is 
		/// written to the result string instead.
		/// 
		/// If there are more format specifiers than values, the format specifiers without a corresponding value
		/// are copied verbatim to output.
		///
		/// If there are more values than format specifiers, the superfluous values are ignored.
		///
		/// Usage Examples:
		///     std::string s1 = format("The answer to life, the universe, and everything is %d", 42);
		///     std::string s2 = format("second: %[1]d, first: %[0]d", 1, 2);

		std::string Format(const std::string& fmt, const Any& value1, const Any& value2);
		std::string Format(const std::string& fmt, const Any& value1, const Any& value2, const Any& value3);
		std::string Format(const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4);
		std::string Format(const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4, const Any& value5);
		std::string Format(const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4, const Any& value5, const Any& value6);


		void Format(std::string& result, const std::string& fmt, const Any& value);
		/// Appends the formatted string to result.

		void Format(std::string& result, const std::string& fmt, const Any& value1, const Any& value2);
		void Format(std::string& result, const std::string& fmt, const Any& value1, const Any& value2, const Any& value3);
		void Format(std::string& result, const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4);
		void Format(std::string& result, const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4, const Any& value5);
		void Format(std::string& result, const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4, const Any& value5, const Any& value6);

		/// Supports a variable number of arguments and is used by
		/// all other variants of format().
		void Format(std::string& result, const std::string& fmt, const std::vector<Any>& values);
	}
}
#endif