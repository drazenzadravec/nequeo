/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          SipRxData.cpp
*  Purpose :       SIP SipRxData class.
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

#include "SipRxData.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	This structure describes an incoming SIP message. It corresponds to the
/// rx data structure in SIP library.
///	</summary>
SipRxData::SipRxData()
{
}

/// <summary>
/// Gets or sets a short info string describing the request, which normally contains
/// the request method and its CSeq.
/// </summary>
String^ SipRxData::Info::get()
{
	return _info;
}

/// <summary>
/// Gets or sets a short info string describing the request, which normally contains
/// the request method and its CSeq.
/// </summary>
void SipRxData::Info::set(String^ value)
{
	_info = value;
}

/// <summary>
/// Gets or sets the source address of the message.
/// </summary>
String^ SipRxData::SrcAddress::get()
{
	return _srcAddress;
}

/// <summary>
/// Gets or sets the source address of the message.
/// </summary>
void SipRxData::SrcAddress::set(String^ value)
{
	_srcAddress = value;
}

/// <summary>
/// Gets or sets the whole message data as a string, containing both the header section
/// and message body section.
/// </summary>
String^ SipRxData::WholeMsg::get()
{
	return _wholeMsg;
}

/// <summary>
/// Gets or sets the whole message data as a string, containing both the header section
/// and message body section.
/// </summary>
void SipRxData::WholeMsg::set(String^ value)
{
	_wholeMsg = value;
}