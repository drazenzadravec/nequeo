/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          UnicodeConverter.cpp
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

#include "stdafx.h"

#include "UnicodeConverter.h"
#include "TextConverter.h"
#include "TextIterator.h"
#include "UTF8Encoding.h"
#include "UTF16Encoding.h"
#include <cstring>
#include <wchar.h>

#ifndef NEQUEO_NO_WSTRING

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			void UnicodeConverter::toUTF16(const std::string& utf8String, std::wstring& utf16String)
			{
				utf16String.clear();
				UTF8Encoding utf8Encoding;
				TextIterator it(utf8String, utf8Encoding);
				TextIterator end(utf8String);
				while (it != end)
				{
					int cc = *it++;
					if (cc <= 0xffff)
					{
						utf16String += (wchar_t)cc;
					}
					else
					{
						cc -= 0x10000;
						utf16String += (wchar_t)((cc >> 10) & 0x3ff) | 0xd800;
						utf16String += (wchar_t)(cc & 0x3ff) | 0xdc00;
					}
				}
			}


			void UnicodeConverter::toUTF16(const char* utf8String, int length, std::wstring& utf16String)
			{
				utf16String.clear();

				UTF8Encoding utf8Encoding;
				UTF16Encoding utf16Encoding;
				const unsigned char* it = (const unsigned char*)utf8String;
				const unsigned char* end = (const unsigned char*)utf8String + length;

				while (it < end)
				{
					int n = utf8Encoding.queryConvert(it, 1);
					int uc;
					int read = 1;

					while (-1 > n && (end - it) >= -n)
					{
						read = -n;
						n = utf8Encoding.queryConvert(it, read);
					}

					if (-1 > n)
					{
						it = end;
					}
					else
					{
						it += read;
					}

					if (-1 >= n)
					{
						uc = 0xfffd;	// Replacement Character (instead of '?')
					}
					else
					{
						uc = n;
					}

					if (uc > 0xffff)
					{
						uc -= 0x10000;
						utf16String += (wchar_t)((uc >> 10) & 0x3ff) | 0xd800;
						utf16String += (wchar_t)(uc & 0x3ff) | 0xdc00;
					}
					else
					{
						utf16String += (wchar_t)uc;
					}
				}
			}


			void UnicodeConverter::toUTF16(const char* utf8String, std::wstring& utf16String)
			{
				toUTF16(utf8String, (int)std::strlen(utf8String), utf16String);
			}


			void UnicodeConverter::toUTF8(const std::wstring& utf16String, std::string& utf8String)
			{
				utf8String.clear();
				UTF8Encoding utf8Encoding;
				UTF16Encoding utf16Encoding;
				TextConverter converter(utf16Encoding, utf8Encoding);
				converter.convert(utf16String.data(), (int)utf16String.length()*sizeof(wchar_t), utf8String);
			}


			void UnicodeConverter::toUTF8(const wchar_t* utf16String, int length, std::string& utf8String)
			{
				utf8String.clear();
				UTF8Encoding utf8Encoding;
				UTF16Encoding utf16Encoding;
				TextConverter converter(utf16Encoding, utf8Encoding);
				converter.convert(utf16String, (int)length*sizeof(wchar_t), utf8String);
			}


			void UnicodeConverter::toUTF8(const wchar_t* utf16String, std::string& utf8String)
			{
				toUTF8(utf16String, (int)wcslen(utf16String), utf8String);
			}
		}
	}
}
#endif
