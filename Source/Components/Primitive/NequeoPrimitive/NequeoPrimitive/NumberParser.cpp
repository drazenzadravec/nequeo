/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NumberParser.cpp
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

#include "stdafx.h"

#include "NumberParser.h"
#include "IO\MemoryInputStream.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

int Nequeo::Primitive::NumberParser::parse(const std::string& s)
{
	int result;
	if (tryParse(s, result))
		return result;
	else
		throw Nequeo::Exceptions::SyntaxException("Not a valid integer", s);
}


bool Nequeo::Primitive::NumberParser::tryParse(const std::string& s, int& value)
{
	char temp;
	return sscanf_s(s.c_str(), "%d%c", &value, &temp) == 1;
}


unsigned Nequeo::Primitive::NumberParser::parseUnsigned(const std::string& s)
{
	unsigned result;
	if (tryParseUnsigned(s, result))
		return result;
	else
		throw Nequeo::Exceptions::SyntaxException("Not a valid unsigned integer", s);
}


bool Nequeo::Primitive::NumberParser::tryParseUnsigned(const std::string& s, unsigned& value)
{
	char temp;
	return sscanf_s(s.c_str(), "%u%c", &value, &temp) == 1;
}


unsigned Nequeo::Primitive::NumberParser::parseHex(const std::string& s)
{
	unsigned result;
	if (tryParseHex(s, result))
		return result;
	else
		throw Nequeo::Exceptions::SyntaxException("Not a valid hexadecimal integer", s);
}


bool Nequeo::Primitive::NumberParser::tryParseHex(const std::string& s, unsigned& value)
{
	char temp;
	return sscanf_s(s.c_str(), "%x%c", &value, &temp) == 1;
}


#if defined(NEQUEO_HAVE_INT64)


Nequeo::Int64 Nequeo::Primitive::NumberParser::parse64(const std::string& s)
{
	Int64 result;
	if (tryParse64(s, result))
		return result;
	else
		throw Nequeo::Exceptions::SyntaxException("Not a valid integer", s);
}


bool Nequeo::Primitive::NumberParser::tryParse64(const std::string& s, Int64& value)
{
	char temp;
	return sscanf_s(s.c_str(), "%I64d%c", &value, &temp) == 1;
}


Nequeo::UInt64 Nequeo::Primitive::NumberParser::parseUnsigned64(const std::string& s)
{
	UInt64 result;
	if (tryParseUnsigned64(s, result))
		return result;
	else
		throw Nequeo::Exceptions::SyntaxException("Not a valid unsigned integer", s);
}


bool Nequeo::Primitive::NumberParser::tryParseUnsigned64(const std::string& s, UInt64& value)
{
	char temp;
	return sscanf_s(s.c_str(), "%I64u%c", &value, &temp) == 1;
}


Nequeo::UInt64 Nequeo::Primitive::NumberParser::parseHex64(const std::string& s)
{
	UInt64 result;
	if (tryParseHex64(s, result))
		return result;
	else
		throw Nequeo::Exceptions::SyntaxException("Not a valid hexadecimal integer", s);
}


bool Nequeo::Primitive::NumberParser::tryParseHex64(const std::string& s, UInt64& value)
{
	char temp;
	return sscanf_s(s.c_str(), "%I64x%c", &value, &temp) == 1;
}


#endif

double Nequeo::Primitive::NumberParser::parseFloat(const std::string& s)
{
	double result;
	if (tryParseFloat(s, result))
		return result;
	else
		throw Nequeo::Exceptions::SyntaxException("Not a valid floating-point number", s);
}


bool Nequeo::Primitive::NumberParser::tryParseFloat(const std::string& s, double& value)
{
	Nequeo::IO::MemoryInputStream istr(s.data(), s.size());
#if !defined(NEQUEO_NO_LOCALE)
	istr.imbue(std::locale::classic());
#endif
	istr >> value;
	return istr.eof() && !istr.fail();
}