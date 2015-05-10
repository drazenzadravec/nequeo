/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          UnicodeConverter.h
*  Purpose :       UnicodeConverter header.
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

#ifndef _UNICODECONVERTER_H
#define _UNICODECONVERTER_H

#include "GlobalText.h"

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			class UnicodeConverter
				/// A convenience class that converts strings from
				/// UTF-8 encoded std::strings to UTF-16 encoded std::wstrings 
				/// and vice-versa.
				///
				/// This class is mainly used for working with the Unicode Windows APIs
				/// and probably won't be of much use anywhere else.
			{
			public:
				static void toUTF16(const std::string& utf8String, std::wstring& utf16String);
				/// Converts the given UTF-8 encoded string into an UTF-16 encoded wstring.

				static void toUTF16(const char* utf8String, int length, std::wstring& utf16String);
				/// Converts the given UTF-8 encoded character sequence into an UTF-16 encoded string.

				static void toUTF16(const char* utf8String, std::wstring& utf16String);
				/// Converts the given zero-terminated UTF-8 encoded character sequence into an UTF-16 encoded wstring.

				static void toUTF8(const std::wstring& utf16String, std::string& utf8String);
				/// Converts the given UTF-16 encoded wstring into an UTF-8 encoded string.

				static void toUTF8(const wchar_t* utf16String, int length, std::string& utf8String);
				/// Converts the given zero-terminated UTF-16 encoded wide character sequence into an UTF-8 encoded wstring.

				static void toUTF8(const wchar_t* utf16String, std::string& utf8String);
				/// Converts the given UTF-16 encoded zero terminated character sequence into an UTF-8 encoded string.
			};
		}
	}
}
#endif