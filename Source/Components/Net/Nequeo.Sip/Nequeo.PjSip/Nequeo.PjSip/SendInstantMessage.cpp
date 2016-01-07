/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          SendInstantMessage.cpp
*  Purpose :       SIP SendInstantMessage class.
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

#include "SendInstantMessage.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// This structure contains parameters for sending instance message methods,
/// e.g: Contact::SendInstantMessage(), Call:SendInstantMessage().
/// </summary>
SendInstantMessageParam::SendInstantMessageParam() :
	_disposed(false)
{
}

/// <summary>
/// This structure contains parameters for sending instance message methods,
/// e.g: Contact::SendInstantMessage(), Call:SendInstantMessage().
/// </summary>
SendInstantMessageParam::~SendInstantMessageParam()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Gets or sets the MIME type. Default is "text/plain".
///	</summary>
String^ SendInstantMessageParam::ContentType::get()
{
	return _contentType;
}

///	<summary>
///	Gets or sets the MIME type. Default is "text/plain".
///	</summary>
void SendInstantMessageParam::ContentType::set(String^ value)
{
	_contentType = value;
}

///	<summary>
///	Gets or sets the message content.
///	</summary>
String^ SendInstantMessageParam::Content::get()
{
	return _content;
}

///	<summary>
///	Gets or sets the message content.
///	</summary>
void SendInstantMessageParam::Content::set(String^ value)
{
	_content = value;
}

///	<summary>
///	Gets or sets the list of headers etc to be included in outgoing request.
///	</summary>
SipTxOption^ SendInstantMessageParam::TxOption::get()
{
	return _txOption;
}

///	<summary>
///	Gets or sets the list of headers etc to be included in outgoing request.
///	</summary>
void SendInstantMessageParam::TxOption::set(SipTxOption^ value)
{
	_txOption = value;
}
