/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Exception.cpp
*  Purpose :       Exception class.
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

#include "Exception.h"

///
Nequeo::Exception::Exception(int code) : _pNested(0), _code(code)
{
}

///
Nequeo::Exception::Exception(const std::string& msg, int code) : _msg(msg), _pNested(0), _code(code)
{
}

///
Nequeo::Exception::Exception(const std::string& msg, const std::string& arg, int code) : _msg(msg), _pNested(0), _code(code)
{
	if (!arg.empty())
	{
		_msg.append(": ");
		_msg.append(arg);
	}
}

///
Nequeo::Exception::Exception(const std::string& msg, const Nequeo::Exception& nested, int code) : _msg(msg), _pNested(nested.clone()), _code(code)
{
}

///
Nequeo::Exception::Exception(const Nequeo::Exception& exc) : std::exception(exc), _msg(exc._msg), _code(exc._code)
{
	_pNested = exc._pNested ? exc._pNested->clone() : 0;
}

///
Nequeo::Exception::~Exception() throw()
{
	delete _pNested;
}

///
Nequeo::Exception& Nequeo::Exception::operator = (const Nequeo::Exception& exc)
{
	if (&exc != this)
	{
		delete _pNested;
		_msg = exc._msg;
		_pNested = exc._pNested ? exc._pNested->clone() : 0;
		_code = exc._code;
	}
	return *this;
}

///
const char* Nequeo::Exception::name() const throw()
{
	return "Exception";
}

///
const char* Nequeo::Exception::className() const throw()
{
	return typeid(*this).name();
}

///
const char* Nequeo::Exception::what() const throw()
{
	return name();
}

///
std::string Nequeo::Exception::displayText() const
{
	std::string txt = name();
	if (!_msg.empty())
	{
		txt.append(": ");
		txt.append(_msg);
	}
	return txt;
}

///
void Nequeo::Exception::extendedMessage(const std::string& arg)
{
	if (!arg.empty())
	{
		if (!_msg.empty()) _msg.append(": ");
		_msg.append(arg);
	}
}

///
Nequeo::Exception* Nequeo::Exception::clone() const
{
	return new Nequeo::Exception(*this);
}

///
void Nequeo::Exception::rethrow() const
{
	throw *this;
}

///
inline const Nequeo::Exception* Nequeo::Exception::nested() const
{
	return _pNested;
}

///
inline const std::string& Nequeo::Exception::message() const
{
	return _msg;
}

///
inline void Nequeo::Exception::message(const std::string& msg)
{
	_msg = msg;
}

///
inline int Nequeo::Exception::code() const
{
	return _code;
}