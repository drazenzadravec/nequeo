/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          SipTxOption.cpp
*  Purpose :       SIP SipTxOption class.
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

#include "SipTxOption.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// This structure describes an incoming SIP message. It corresponds to the
/// rx data structure in SIP library.
/// </summary>
SipTxOption::SipTxOption()
{
}

/// <summary>
/// Gets or sets MIME type of the message body, if application specifies the messageBody
/// in this structure.
/// </summary>
String^ SipTxOption::ContentType::get()
{
	return _contentType;
}

/// <summary>
/// Gets or sets MIME type of the message body, if application specifies the messageBody
/// in this structure.
/// </summary>
void SipTxOption::ContentType::set(String^ value)
{
	_contentType = value;
}

/// <summary>
/// Gets or sets Optional message body to be added to the message, only when the
/// message doesn't have a body.
/// </summary>
String^ SipTxOption::MsgBody::get()
{
	return _msgBody;
}

/// <summary>
/// Gets or sets Optional message body to be added to the message, only when the
/// message doesn't have a body.
/// </summary>
void SipTxOption::MsgBody::set(String^ value)
{
	_msgBody = value;
}

/// <summary>
/// Gets or sets optional remote target URI (i.e. Target header). If empty (""), the
/// target will be set to the remote URI(To header). At the moment this
/// field is only used when sending initial INVITE and MESSAGE requests.
/// </summary>
String^ SipTxOption::TargetUri::get()
{
	return _targetUri;
}

/// <summary>
/// Gets or sets optional remote target URI (i.e. Target header). If empty (""), the
/// target will be set to the remote URI(To header). At the moment this
/// field is only used when sending initial INVITE and MESSAGE requests.
/// </summary>
void SipTxOption::TargetUri::set(String^ value)
{
	_targetUri = value;
}

/// <summary>
/// Gets or sets content type of the multipart body. If application wants to send
/// multipart message bodies, it puts the parts in multipartParts and set
/// the content type in multipartContentType.If the message already
/// contains a body, the body will be added to the multipart bodies.
/// </summary>
SipMediaType^ SipTxOption::MultipartContentType::get()
{
	return _multipartContentType;
}

/// <summary>
/// Gets or sets content type of the multipart body. If application wants to send
/// multipart message bodies, it puts the parts in multipartParts and set
/// the content type in multipartContentType.If the message already
/// contains a body, the body will be added to the multipart bodies.
/// </summary>
void SipTxOption::MultipartContentType::set(SipMediaType^ value)
{
	_multipartContentType = value;
}

/// <summary>
/// Gets or sets additional message headers to be included in the outgoing message.
/// </summary>
array<SipHeader^>^ SipTxOption::Headers::get()
{
	return _headers;
}

/// <summary>
/// Gets or sets additional message headers to be included in the outgoing message.
/// </summary>
void SipTxOption::Headers::set(array<SipHeader^>^ value)
{
	_headers = value;
}