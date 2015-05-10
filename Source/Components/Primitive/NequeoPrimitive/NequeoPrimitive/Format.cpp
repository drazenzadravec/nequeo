/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Format.cpp
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

#include "stdafx.h"

#include "Format.h"
#include "Base\Ascii.h"
#include "Base\Types.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"
#include <sstream>
#if !defined(NEQUEO_NO_LOCALE)
#include <locale>
#endif
#include <cstddef>

using Nequeo::Primitive::Any;
using Nequeo::Exceptions::BadCastException;

namespace Nequeo {
	namespace Primitive
	{
		void parseFlags(std::ostream& str, std::string::const_iterator& itFmt, const std::string::const_iterator& endFmt)
		{
			bool isFlag = true;
			while (isFlag && itFmt != endFmt)
			{
				switch (*itFmt)
				{
				case '-': str.setf(std::ios::left); ++itFmt; break;
				case '+': str.setf(std::ios::showpos); ++itFmt; break;
				case '0': str.fill('0'); ++itFmt; break;
				case '#': str.setf(std::ios::showpoint | std::ios_base::showbase); ++itFmt; break;
				default:  isFlag = false; break;
				}
			}
		}


		void parseWidth(std::ostream& str, std::string::const_iterator& itFmt, const std::string::const_iterator& endFmt)
		{
			int width = 0;
			while (itFmt != endFmt && Ascii::isDigit(*itFmt))
			{
				width = 10 * width + *itFmt - '0';
				++itFmt;
			}
			if (width != 0) str.width(width);
		}


		void parsePrec(std::ostream& str, std::string::const_iterator& itFmt, const std::string::const_iterator& endFmt)
		{
			if (itFmt != endFmt && *itFmt == '.')
			{
				++itFmt;
				int prec = 0;
				while (itFmt != endFmt && Ascii::isDigit(*itFmt))
				{
					prec = 10 * prec + *itFmt - '0';
					++itFmt;
				}
				if (prec >= 0) str.precision(prec);
			}
		}

		char parseMod(std::string::const_iterator& itFmt, const std::string::const_iterator& endFmt)
		{
			char mod = 0;
			if (itFmt != endFmt)
			{
				switch (*itFmt)
				{
				case 'l':
				case 'h':
				case 'L':
				case '?': mod = *itFmt++; break;
				}
			}
			return mod;
		}

		std::size_t parseIndex(std::string::const_iterator& itFmt, const std::string::const_iterator& endFmt)
		{
			int index = 0;
			while (itFmt != endFmt && Ascii::isDigit(*itFmt))
			{
				index = 10 * index + *itFmt - '0';
				++itFmt;
			}
			if (itFmt != endFmt && *itFmt == ']') ++itFmt;
			return index;
		}

		void prepareFormat(std::ostream& str, char type)
		{
			switch (type)
			{
			case 'd':
			case 'i': str << std::dec; break;
			case 'o': str << std::oct; break;
			case 'x': str << std::hex; break;
			case 'X': str << std::hex << std::uppercase; break;
			case 'e': str << std::scientific; break;
			case 'E': str << std::scientific << std::uppercase; break;
			case 'f': str << std::fixed; break;
			}
		}


		void writeAnyInt(std::ostream& str, const Any& any)
		{
			if (any.type() == typeid(char))
				str << static_cast<int>(AnyCast<char>(any));
			else if (any.type() == typeid(signed char))
				str << static_cast<int>(AnyCast<signed char>(any));
			else if (any.type() == typeid(unsigned char))
				str << static_cast<unsigned>(AnyCast<unsigned char>(any));
			else if (any.type() == typeid(short))
				str << AnyCast<short>(any);
			else if (any.type() == typeid(unsigned short))
				str << AnyCast<unsigned short>(any);
			else if (any.type() == typeid(int))
				str << AnyCast<int>(any);
			else if (any.type() == typeid(unsigned int))
				str << AnyCast<unsigned int>(any);
			else if (any.type() == typeid(long))
				str << AnyCast<long>(any);
			else if (any.type() == typeid(unsigned long))
				str << AnyCast<unsigned long>(any);
			else if (any.type() == typeid(Int64))
				str << AnyCast<Int64>(any);
			else if (any.type() == typeid(UInt64))
				str << AnyCast<UInt64>(any);
			else if (any.type() == typeid(bool))
				str << AnyCast<bool>(any);
		}


		void formatOne(std::string& result, std::string::const_iterator& itFmt, const std::string::const_iterator& endFmt, std::vector<Any>::const_iterator& itVal)
		{
			std::ostringstream str;
#if !defined(NEQUEO_NO_LOCALE)
			str.imbue(std::locale::classic());
#endif
			try
			{
				parseFlags(str, itFmt, endFmt);
				parseWidth(str, itFmt, endFmt);
				parsePrec(str, itFmt, endFmt);
				char mod = parseMod(itFmt, endFmt);
				if (itFmt != endFmt)
				{
					char type = *itFmt++;
					prepareFormat(str, type);
					switch (type)
					{
					case 'b':
						str << AnyCast<bool>(*itVal++);
						break;
					case 'c':
						str << AnyCast<char>(*itVal++);
						break;
					case 'd':
					case 'i':
						switch (mod)
						{
						case 'l': str << AnyCast<long>(*itVal++); break;
						case 'L': str << AnyCast<Int64>(*itVal++); break;
						case 'h': str << AnyCast<short>(*itVal++); break;
						case '?': writeAnyInt(str, *itVal++); break;
						default:  str << AnyCast<int>(*itVal++); break;
						}
						break;
					case 'o':
					case 'u':
					case 'x':
					case 'X':
						switch (mod)
						{
						case 'l': str << AnyCast<unsigned long>(*itVal++); break;
						case 'L': str << AnyCast<UInt64>(*itVal++); break;
						case 'h': str << AnyCast<unsigned short>(*itVal++); break;
						case '?': writeAnyInt(str, *itVal++); break;
						default:  str << AnyCast<unsigned>(*itVal++); break;
						}
						break;
					case 'e':
					case 'E':
					case 'f':
						switch (mod)
						{
						case 'l': str << AnyCast<long double>(*itVal++); break;
						case 'L': str << AnyCast<long double>(*itVal++); break;
						case 'h': str << AnyCast<float>(*itVal++); break;
						default:  str << AnyCast<double>(*itVal++); break;
						}
						break;
					case 's':
						str << RefAnyCast<std::string>(*itVal++);
						break;
					case 'z':
						str << AnyCast<std::size_t>(*itVal++);
						break;
					case 'I':
					case 'D':
					default:
						str << type;
					}
				}
			}
			catch (BadCastException&)
			{
				str << "[ERRFMT]";
			}
			result.append(str.str());
		}
	}


	std::string Format(const std::string& fmt, const Any& value)
	{
		std::string result;
		Nequeo::Primitive::Format(result, fmt, value);
		return result;
	}


	std::string Format(const std::string& fmt, const Any& value1, const Any& value2)
	{
		std::string result;
		Nequeo::Primitive::Format(result, fmt, value1, value2);
		return result;
	}


	std::string Format(const std::string& fmt, const Any& value1, const Any& value2, const Any& value3)
	{
		std::string result;
		Nequeo::Primitive::Format(result, fmt, value1, value2, value3);
		return result;
	}


	std::string Format(const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4)
	{
		std::string result;
		Nequeo::Primitive::Format(result, fmt, value1, value2, value3, value4);
		return result;
	}


	std::string Format(const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4, const Any& value5)
	{
		std::string result;
		Nequeo::Primitive::Format(result, fmt, value1, value2, value3, value4, value5);
		return result;
	}


	std::string Format(const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4, const Any& value5, const Any& value6)
	{
		std::string result;
		Nequeo::Primitive::Format(result, fmt, value1, value2, value3, value4, value5, value6);
		return result;
	}


	void Format(std::string& result, const std::string& fmt, const Any& value)
	{
		std::vector<Any> args;
		args.push_back(value);
		Nequeo::Primitive::Format(result, fmt, args);
	}


	void Format(std::string& result, const std::string& fmt, const Any& value1, const Any& value2)
	{
		std::vector<Any> args;
		args.push_back(value1);
		args.push_back(value2);
		Nequeo::Primitive::Format(result, fmt, args);
	}


	void Format(std::string& result, const std::string& fmt, const Any& value1, const Any& value2, const Any& value3)
	{
		std::vector<Any> args;
		args.push_back(value1);
		args.push_back(value2);
		args.push_back(value3);
		Nequeo::Primitive::Format(result, fmt, args);
	}


	void Format(std::string& result, const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4)
	{
		std::vector<Any> args;
		args.push_back(value1);
		args.push_back(value2);
		args.push_back(value3);
		args.push_back(value4);
		Nequeo::Primitive::Format(result, fmt, args);
	}


	void Format(std::string& result, const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4, const Any& value5)
	{
		std::vector<Any> args;
		args.push_back(value1);
		args.push_back(value2);
		args.push_back(value3);
		args.push_back(value4);
		args.push_back(value5);
		Nequeo::Primitive::Format(result, fmt, args);
	}


	void Format(std::string& result, const std::string& fmt, const Any& value1, const Any& value2, const Any& value3, const Any& value4, const Any& value5, const Any& value6)
	{
		std::vector<Any> args;
		args.push_back(value1);
		args.push_back(value2);
		args.push_back(value3);
		args.push_back(value4);
		args.push_back(value5);
		args.push_back(value6);
		Nequeo::Primitive::Format(result, fmt, args);
	}


	void Format(std::string& result, const std::string& fmt, const std::vector<Any>& values)
	{
		std::string::const_iterator itFmt = fmt.begin();
		std::string::const_iterator endFmt = fmt.end();
		std::vector<Any>::const_iterator itVal = values.begin();
		std::vector<Any>::const_iterator endVal = values.end();
		while (itFmt != endFmt)
		{
			switch (*itFmt)
			{
			case '%':
				++itFmt;
				if (itFmt != endFmt && itVal != endVal)
				{
					if (*itFmt == '[')
					{
						++itFmt;
						std::size_t index = Nequeo::Primitive::parseIndex(itFmt, endFmt);
						if (index < values.size())
						{
							std::vector<Any>::const_iterator it = values.begin() + index;
							formatOne(result, itFmt, endFmt, it);
						}
						else throw Nequeo::Exceptions::InvalidArgumentException("format argument index out of range", fmt);
					}
					else
					{
						formatOne(result, itFmt, endFmt, itVal);
					}
				}
				else if (itFmt != endFmt)
				{
					result += *itFmt++;
				}
				break;
			default:
				result += *itFmt;
				++itFmt;
			}
		}
	}
}