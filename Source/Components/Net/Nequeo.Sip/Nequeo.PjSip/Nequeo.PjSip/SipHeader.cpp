/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallSetting.cpp
*  Purpose :       SIP CallSetting class.
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

#include "SipHeader.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Call settings.
///	</summary>
SipHeader::SipHeader()
{
}

///	<summary>
///	Gets or sets the header name.
///	</summary>
String^ SipHeader::Name::get()
{
	return _name;
}

///	<summary>
///	Gets or sets the header name.
///	</summary>
void SipHeader::Name::set(String^ value)
{
	_name = value;
}

///	<summary>
///	Gets or sets the header value.
///	</summary>
String^ SipHeader::Value::get()
{
	return _value;
}

///	<summary>
///	Gets or sets the header value.
///	</summary>
void SipHeader::Value::set(String^ value)
{
	_value = value;
}