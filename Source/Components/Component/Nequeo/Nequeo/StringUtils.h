/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          StringUtils.h
*  Purpose :       StringUtils class.
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

#include "Global.h"
#include "Allocator.h"

namespace Nequeo
{
	/**
	* All the things the c++ stdlib is missing for string operations that I needed.
	*/
	class StringUtils
	{
	public:
		static void Replace(Nequeo::String& s, const char* search, const char* replace);


		/**
		* Converts a string to lower case.
		*/
		static Nequeo::String ToLower(const char* source);


		/**
		* Converts a string to upper case.
		*/
		static Nequeo::String ToUpper(const char* source);


		/**
		* Does a caseless comparison of two strings.
		*/
		static bool CaselessCompare(const char* value1, const char* value2);


		/**
		* URL encodes a string (uses %20 not + for spaces).
		*/
		static Nequeo::String URLEncode(const char* unsafe);

		/**
		* Http Clients tend to escape some characters but not all. Escaping all of them causes problems, because the client
		* will also try to escape them.
		* So this only escapes non-ascii characters and the + character
		*/
		static Nequeo::String UTF8Escape(const char* unicodeString, const char* delimiter);

		/**
		* URL encodes a double (if it ends up going to scientific notation) otherwise it just returns it as a string.
		*/
		static Nequeo::String URLEncode(double unsafe);


		/**
		* Decodes a URL encoded string (will handle both encoding schemes for spaces).
		*/
		static Nequeo::String URLDecode(const char* safe);


		/**
		* Splits a string on a delimiter (empty items are excluded).
		*/
		static Nequeo::Vector<Nequeo::String> Split(const Nequeo::String& toSplit, char splitOn);


		/**
		* Splits a string on new line characters.
		*/
		static Nequeo::Vector<Nequeo::String> SplitOnLine(const Nequeo::String& toSplit);


		/** static Aws::Vector<Aws::String> SplitOnRegex(Aws::String regex);
		*  trim from start
		*/
		static Nequeo::String LTrim(const char* source);


		/**
		* trim from end
		*/
		static Nequeo::String RTrim(const char* source);

		/**
		* trim from both ends
		*/
		static Nequeo::String Trim(const char* source);

		/** static Nequeo::Vector<Nequeo::String> SplitOnRegex(Nequeo::String regex);
		*  trim from start
		*/
		static Nequeo::String LTrim(const char* source, const char* trim);


		/**
		* trim from end
		*/
		static Nequeo::String RTrim(const char* source, const char* trim);

		/**
		* trim from both ends
		*/
		static Nequeo::String Trim(const char* source, const char* trim);

		/**
		* convert to int 64
		*/
		static long long ConvertToInt64(const char* source);


		/**
		* convert to int 32
		*/
		static long ConvertToInt32(const char* source);


		/**
		* convert to bool
		*/
		static bool ConvertToBool(const char* source);


		/**
		* convert to double
		*/
		static double ConvertToDouble(const char* source);


#ifdef _WIN32
		/**
		* Converts a string to wstring.
		*/
		static Nequeo::WString ToWString(const char* source);

		/**
		* Converts a wstring to string.
		*/
		static Nequeo::String FromWString(const wchar_t* source);
#endif

		/**
		* not all platforms (Android) have std::to_string
		*/
		template< typename T >
		static Nequeo::String to_string(T value)
		{
			Nequeo::OStringStream os;
			os << value;
			return os.str();
		}
	};
}