/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          StringUtils.cpp
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

#include "stdafx.h"

#include "StringUtils.h"

#include <algorithm>
#include <iomanip>
#include <cstdlib>
#include <cstring>
#include <functional>

namespace Nequeo
{
	void StringUtils::Replace(Nequeo::String& s, const char* search, const char* replace)
	{
		if (!search || !replace)
		{
			return;
		}

		size_t replaceLength = strlen(replace);
		size_t searchLength = strlen(search);

		for (std::size_t pos = 0;; pos += replaceLength)
		{
			pos = s.find(search, pos);
			if (pos == Nequeo::String::npos)
				break;

			s.erase(pos, searchLength);
			s.insert(pos, replace);
		}
	}


	Nequeo::String StringUtils::ToLower(const char* source)
	{
		Nequeo::String copy;
		size_t sourceLength = strlen(source);
		copy.resize(sourceLength);
		std::transform(source, source + sourceLength, copy.begin(), ::tolower);

		return copy;
	}


	Nequeo::String StringUtils::ToUpper(const char* source)
	{
		Nequeo::String copy;
		size_t sourceLength = strlen(source);
		copy.resize(sourceLength);
		std::transform(source, source + sourceLength, copy.begin(), ::toupper);

		return copy;
	}


	bool StringUtils::CaselessCompare(const char* value1, const char* value2)
	{
		Nequeo::String value1Lower = ToLower(value1);
		Nequeo::String value2Lower = ToLower(value2);

		return value1Lower == value2Lower;
	}


	Nequeo::Vector<Nequeo::String> StringUtils::Split(const Nequeo::String& toSplit, char splitOn)
	{
		Nequeo::StringStream input(toSplit);
		Nequeo::Vector<Nequeo::String> returnValues;
		Nequeo::String item;

		while (std::getline(input, item, splitOn))
		{
			if (item.size() > 0)
			{
				returnValues.push_back(item);
			}
		}

		return returnValues;
	}


	Nequeo::Vector<Nequeo::String> StringUtils::SplitOnLine(const Nequeo::String& toSplit)
	{
		Nequeo::StringStream input(toSplit);
		Nequeo::Vector<Nequeo::String> returnValues;
		Nequeo::String item;

		while (std::getline(input, item))
		{
			if (item.size() > 0)
			{
				returnValues.push_back(item);
			}
		}

		return returnValues;
	}


	Nequeo::String StringUtils::URLEncode(const char* unsafe)
	{
		Nequeo::StringStream escaped;
		escaped.fill('0');
		escaped << std::hex << std::uppercase;

		size_t unsafeLength = strlen(unsafe);
		for (auto i = unsafe, n = unsafe + unsafeLength; i != n; ++i)
		{
			int c = *i;
			//MSVC 2015 has an assertion that c is positive in isalnum(). This breaks unicode support.
			//bypass that with the first check.
			if (c >= 0 && (isalnum(c) || c == '-' || c == '_' || c == '.' || c == '~'))
			{
				escaped << (char)c;
			}
			else
			{
				//this unsigned char cast allows us to handle unicode characters.
				escaped << '%' << std::setw(2) << int((unsigned char)c) << std::setw(0);
			}
		}

		return escaped.str();
	}

	Nequeo::String StringUtils::UTF8Escape(const char* unicodeString, const char* delimiter)
	{
		Nequeo::StringStream escaped;
		escaped.fill('0');
		escaped << std::hex << std::uppercase;

		size_t unsafeLength = strlen(unicodeString);
		for (auto i = unicodeString, n = unicodeString + unsafeLength; i != n; ++i)
		{
			int c = *i;
			//MSVC 2015 has an assertion that c is positive in isalnum(). This breaks unicode support.
			//bypass that with the first check.
			if (c >= ' ' && c < 127)
			{
				escaped << (char)c;
			}
			else
			{
				//this unsigned char cast allows us to handle unicode characters.
				escaped << delimiter << std::setw(2) << int((unsigned char)c) << std::setw(0);
			}
		}

		return escaped.str();
	}

	Nequeo::String StringUtils::URLEncode(double unsafe)
	{
		char buffer[32];
#if defined(_MSC_VER) && _MSC_VER < 1900
		_snprintf_s(buffer, sizeof(buffer), _TRUNCATE, "%g", unsafe);
#else
		snprintf(buffer, sizeof(buffer), "%g", unsafe);
#endif

		return StringUtils::URLEncode(buffer);
	}


	Nequeo::String StringUtils::URLDecode(const char* safe)
	{
		Nequeo::StringStream unescaped;
		unescaped.fill('0');
		unescaped << std::hex;

		size_t safeLength = strlen(safe);
		for (auto i = safe, n = safe + safeLength; i != n; ++i)
		{
			char c = *i;
			if (c == '%')
			{
				char hex[3];
				hex[0] = *(i + 1);
				hex[1] = *(i + 2);
				hex[2] = 0;
				i += 2;
				auto hexAsInteger = strtol(hex, nullptr, 16);
				unescaped << (char)hexAsInteger;
			}
			else
			{
				unescaped << *i;
			}
		}

		return unescaped.str();
	}

	Nequeo::String StringUtils::LTrim(const char* source)
	{
		Nequeo::String copy(source);
		copy.erase(copy.begin(), std::find_if(copy.begin(), copy.end(), std::not1(std::ptr_fun<int, int>(::isspace))));
		return copy;
	}

	// trim from end
	Nequeo::String StringUtils::RTrim(const char* source)
	{
		Nequeo::String copy(source);
		copy.erase(std::find_if(copy.rbegin(), copy.rend(), std::not1(std::ptr_fun<int, int>(::isspace))).base(), copy.end());
		return copy;
	}

	// trim from both ends
	Nequeo::String StringUtils::Trim(const char* source)
	{
		return LTrim(RTrim(source).c_str());
	}

	/** static Aws::Vector<Aws::String> SplitOnRegex(Aws::String regex);
	*  trim from start
	*/
	Nequeo::String StringUtils::LTrim(const char* source, const char* trim)
	{
		std::string s(source);
		s.erase(0, s.find_first_not_of(trim));
		return Nequeo::String(s.c_str());
	}

	/**
	* trim from end
	*/
	Nequeo::String StringUtils::RTrim(const char* source, const char* trim)
	{
		std::string s(source);
		s.erase(s.find_last_not_of(trim) + 1);
		return Nequeo::String(s.c_str());
	}

	/**
	* trim from both ends
	*/
	Nequeo::String StringUtils::Trim(const char* source, const char* trim)
	{
		return LTrim(RTrim(source, trim).c_str(), trim);
	}

	long long StringUtils::ConvertToInt64(const char* source)
	{
		if (!source)
		{
			return 0;
		}

#ifdef __ANDROID__
		return atoll(source);
#else
		return std::atoll(source);
#endif // __ANDROID__
	}


	long StringUtils::ConvertToInt32(const char* source)
	{
		if (!source)
		{
			return 0;
		}

		return std::atol(source);
	}


	bool StringUtils::ConvertToBool(const char* source)
	{
		if (!source)
		{
			return false;
		}

		Nequeo::String strValue = ToLower(source);
		if (strValue == "true" || strValue == "1")
		{
			return true;
		}

		return false;
	}


	double StringUtils::ConvertToDouble(const char* source)
	{
		if (!source)
		{
			return 0.0;
		}

		return std::strtod(source, NULL);
	}

#ifdef _WIN32

	Nequeo::WString StringUtils::ToWString(const char* source)
	{
		Nequeo::WString outString;

		outString.resize(std::strlen(source));
		std::copy(source, source + std::strlen(source), outString.begin());
		return outString;
	}

	Nequeo::String StringUtils::FromWString(const wchar_t* source)
	{
		Nequeo::WString inWString(source);

		Nequeo::String outString(inWString.begin(), inWString.end());
		return outString;
	}

#endif
}