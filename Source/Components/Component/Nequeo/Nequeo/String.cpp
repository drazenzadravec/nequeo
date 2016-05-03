/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          String.cpp
*  Purpose :       String class.
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

#include "String.h"

namespace Nequeo
{
	/// <summary>
	/// Constructor for the current class.
	/// </summary>
	StringEx::StringEx() : _disposed(false)
	{
	}

	/// <summary>
	/// This destructor.
	/// </summary>
	StringEx::~StringEx()
	{
		// If not disposed.
		if (!_disposed)
		{
			// Indicate that dispose has been called.
			_disposed = true;
		}
	}

	/// <summary>
	/// Convert from wide string to string.
	/// </summary>
	/// <param name="wstr">The wide string.</param>
	/// <returns>The string.</returns>
	std::string StringEx::WideStringToString(std::wstring wstr)
	{
		string str(wstr.length(), ' ');
		copy(wstr.begin(), wstr.end(), str.begin());
		return str;
	}

	/// <summary>
	/// Convert from string to wide string.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns>The wide string.</returns>
	std::wstring StringEx::StringToWideString(std::string str)
	{
		wstring wstr(str.length(), L' ');
		copy(str.begin(), str.end(), wstr.begin());
		return wstr;
	}

	/// <summary>
	/// Convert from wide string to ASCII string.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns>The wide string.</returns>
	std::string StringEx::ToAsciiString(const std::wstring &source)
	{
		int sourceLen = static_cast<int>(source.size());
		int requiredCharacters = WideCharToMultiByte(GetACP(), 0, source.c_str(), sourceLen, 0, 0, 0, 0);
		char *buffer = new char[requiredCharacters + 1];
		int writtenCharacters = WideCharToMultiByte(GetACP(), 0, source.c_str(), sourceLen, buffer, requiredCharacters, 0, 0);
		buffer[writtenCharacters] = 0;
		std::string result(buffer);
		delete[] buffer;
		return result;
	}

	/// <summary>
	/// Convert from string to UNICODE wide string.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns>The wide string.</returns>
	std::wstring StringEx::ToUnicodeString(const std::string &source)
	{
		int sourceLen = static_cast<int>(source.size());
		int requiredCharacters = MultiByteToWideChar(GetACP(), 0, source.c_str(), sourceLen, 0, 0);
		wchar_t *buffer = new wchar_t[requiredCharacters + 1];
		int writtenCharacters = MultiByteToWideChar(GetACP(), 0, source.c_str(), sourceLen, buffer, requiredCharacters);
		buffer[writtenCharacters] = 0;
		std::wstring result(buffer);
		delete[] buffer;
		return result;
	}
}