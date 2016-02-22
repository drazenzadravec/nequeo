/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnInstantMessageParam.cpp
*  Purpose :       SIP OnInstantMessageParam class.
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

#include "OnInstantMessageParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	On instant message paramters.
///	</summary>
OnInstantMessageParam::OnInstantMessageParam()
{
}

/// <summary>
/// Gets or sets the contact URI of the sender.
/// </summary>
String^ OnInstantMessageParam::ContactUri::get()
{
	return _contactUri;
}

/// <summary>
/// Gets or sets the contact URI of the sender.
/// </summary>
void OnInstantMessageParam::ContactUri::set(String^ value)
{
	_contactUri = value;
}

/// <summary>
/// Gets or sets the MIME type of the message body.
/// </summary>
String^ OnInstantMessageParam::ContentType::get()
{
	return _contentType;
}

/// <summary>
/// Gets or sets the MIME type of the message body.
/// </summary>
void OnInstantMessageParam::ContentType::set(String^ value)
{
	_contentType = value;
}

/// <summary>
/// Gets or sets the sender from URI.
/// </summary>
String^ OnInstantMessageParam::FromUri::get()
{
	return _fromUri;
}

/// <summary>
/// Gets or sets the sender from URI.
/// </summary>
void OnInstantMessageParam::FromUri::set(String^ value)
{
	_fromUri = value;
}

/// <summary>
/// Gets or sets the message body.
/// </summary>
String^ OnInstantMessageParam::MsgBody::get()
{
	return _msgBody;
}

/// <summary>
/// Gets or sets the message body.
/// </summary>
void OnInstantMessageParam::MsgBody::set(String^ value)
{
	_msgBody = value;
}

/// <summary>
/// Gets or sets the incoming INVITE request.
/// </summary>
SipRxData^ OnInstantMessageParam::RxData::get()
{
	return _rxData;
}

/// <summary>
/// Gets or sets the incoming INVITE request.
/// </summary>
void OnInstantMessageParam::RxData::set(SipRxData^ value)
{
	_rxData = value;
}

/// <summary>
/// Gets or sets the to URI of the request.
/// </summary>
String^ OnInstantMessageParam::ToUri::get()
{
	return _toUri;
}

/// <summary>
/// Gets or sets the to URI of the request.
/// </summary>
void OnInstantMessageParam::ToUri::set(String^ value)
{
	_toUri = value;
}