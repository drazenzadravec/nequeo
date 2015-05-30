/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NumberParser.h
*  Purpose :       NumberParser class.
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

#ifndef _NUMBERPARSER_H
#define _NUMBERPARSER_H

#include "GlobalPrimitive.h"

#include "Base\Types.h"

namespace Nequeo {
	namespace Primitive
	{
		/// The NumberParser class provides static methods
		/// for parsing numbers out of strings.
		class NumberParser
		{
		public:
			static int parse(const std::string& s);
			/// Parses an integer value in decimal notation from the given string.
			/// Throws a SyntaxException if the string does not hold a number in decimal notation.

			static bool tryParse(const std::string& s, int& value);
			/// Parses an integer value in decimal notation from the given string.
			/// Returns true if a valid integer has been found, false otherwise. 

			static unsigned parseUnsigned(const std::string& s);
			/// Parses an unsigned integer value in decimal notation from the given string.
			/// Throws a SyntaxException if the string does not hold a number in decimal notation.

			static bool tryParseUnsigned(const std::string& s, unsigned& value);
			/// Parses an unsigned integer value in decimal notation from the given string.
			/// Returns true if a valid integer has been found, false otherwise. 

			static unsigned parseHex(const std::string& s);
			/// Parses an integer value in hexadecimal notation from the given string.
			/// Throws a SyntaxException if the string does not hold a number in
			/// hexadecimal notation.

			static bool tryParseHex(const std::string& s, unsigned& value);
			/// Parses an unsigned integer value in hexadecimal notation from the given string.
			/// Returns true if a valid integer has been found, false otherwise. 

#if defined(NEQUEO_HAVE_INT64)

			static Int64 parse64(const std::string& s);
			/// Parses a 64-bit integer value in decimal notation from the given string.
			/// Throws a SyntaxException if the string does not hold a number in decimal notation.

			static bool tryParse64(const std::string& s, Int64& value);
			/// Parses a 64-bit integer value in decimal notation from the given string.
			/// Returns true if a valid integer has been found, false otherwise. 

			static UInt64 parseUnsigned64(const std::string& s);
			/// Parses an unsigned 64-bit integer value in decimal notation from the given string.
			/// Throws a SyntaxException if the string does not hold a number in decimal notation.

			static bool tryParseUnsigned64(const std::string& s, UInt64& value);
			/// Parses an unsigned 64-bit integer value in decimal notation from the given string.
			/// Returns true if a valid integer has been found, false otherwise. 

			static UInt64 parseHex64(const std::string& s);
			/// Parses a 64 bit-integer value in hexadecimal notation from the given string.
			/// Throws a SyntaxException if the string does not hold a number in hexadecimal notation.

			static bool tryParseHex64(const std::string& s, UInt64& value);
			/// Parses an unsigned 64-bit integer value in hexadecimal notation from the given string.
			/// Returns true if a valid integer has been found, false otherwise. 

#endif

			static double parseFloat(const std::string& s);
			/// Parses a double value in decimal floating point notation
			/// from the given string. 
			/// Throws a SyntaxException if the string does not hold a floating-point 
			/// number in decimal notation.

			static bool tryParseFloat(const std::string& s, double& value);
			/// Parses a double value in decimal floating point notation
			/// from the given string. 
			/// Returns true if a valid floating point number has been found, 
			/// false otherwise. 
		};
	}
}
#endif