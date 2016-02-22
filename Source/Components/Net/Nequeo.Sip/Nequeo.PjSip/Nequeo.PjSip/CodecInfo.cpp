/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AccountConnection.cpp
*  Purpose :       SIP Account Connection class.
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

#include "CodecInfo.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Codec information.
///	</summary>
CodecInfo::CodecInfo()
{
}

/// <summary>
/// Gets or sets the codec id.
/// </summary>
String^ CodecInfo::CodecId::get()
{
	return _codecId;
}

/// <summary>
/// Gets or sets the codec id.
/// </summary>
void CodecInfo::CodecId::set(String^ value)
{
	_codecId = value;
}

/// <summary>
/// Gets or sets the codec description.
/// </summary>
String^ CodecInfo::Description::get()
{
	return _description;
}

/// <summary>
/// Gets or sets the codec description.
/// </summary>
void CodecInfo::Description::set(String^ value)
{
	_description = value;
}

/// <summary>
/// Gets or sets the codec priority (integer 0-255).
/// </summary>
byte CodecInfo::Priority::get()
{
	return _priority;
}

/// <summary>
/// Gets or sets the codec priority (integer 0-255).
/// </summary>
void CodecInfo::Priority::set(byte value)
{
	_priority = value;
}