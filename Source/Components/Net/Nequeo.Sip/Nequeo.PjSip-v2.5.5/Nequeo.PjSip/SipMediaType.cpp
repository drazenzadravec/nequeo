/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          SipMediaType.cpp
*  Purpose :       SIP SipMediaType class.
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

#include "SipMediaType.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// SIP media type containing type and subtype. For example, for
/// "application/sdp", the type is "application" and the subtype is "sdp".
/// </summary>
SipMediaType::SipMediaType()
{
}

/// <summary>
/// Gets or sets the media subtype.
/// </summary>
String^ SipMediaType::SubType::get()
{
	return _subType;
}

/// <summary>
/// Gets or sets the media subtype.
/// </summary>
void SipMediaType::SubType::set(String^ value)
{
	_subType = value;
}

/// <summary>
/// Gets or sets the media type.
/// </summary>
String^ SipMediaType::Type::get()
{
	return _type;
}

/// <summary>
/// Gets or sets the media type.
/// </summary>
void SipMediaType::Type::set(String^ value)
{
	_type = value;
}