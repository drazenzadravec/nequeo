/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnIncomingSubscribeParam.cpp
*  Purpose :       SIP OnIncomingSubscribeParam class.
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

#include "OnIncomingSubscribeParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	On incoming subscribe paramters.
///	</summary>
OnIncomingSubscribeParam::OnIncomingSubscribeParam()
{
}

/// <summary>
/// Gets or sets the SIP status code of the transaction.
/// </summary>
StatusCode OnIncomingSubscribeParam::Code::get()
{
	return _code;
}

/// <summary>
/// Gets or sets the SIP status code of the transaction.
/// </summary>
void OnIncomingSubscribeParam::Code::set(StatusCode value)
{
	_code = value;
}

/// <summary>
/// Gets or sets the sender URI.
/// </summary>
String^ OnIncomingSubscribeParam::FromUri::get()
{
	return _fromUri;
}

/// <summary>
/// Gets or sets the sender URI.
/// </summary>
void OnIncomingSubscribeParam::FromUri::set(String^ value)
{
	_fromUri = value;
}

/// <summary>
/// Gets or sets the reason phrase to respond to the request.
/// </summary>
String^ OnIncomingSubscribeParam::Reason::get()
{
	return _reason;
}

/// <summary>
/// Gets or sets the reason phrase to respond to the request.
/// </summary>
void OnIncomingSubscribeParam::Reason::set(String^ value)
{
	_reason = value;
}

/// <summary>
/// Gets or sets the incoming INVITE request.
/// </summary>
SipRxData^ OnIncomingSubscribeParam::RxData::get()
{
	return _rxData;
}

/// <summary>
/// Gets or sets the incoming INVITE request.
/// </summary>
void OnIncomingSubscribeParam::RxData::set(SipRxData^ value)
{
	_rxData = value;
}

/// <summary>
/// Gets or sets additional data to be sent with the response, if any.
/// </summary>
SipTxOption^ OnIncomingSubscribeParam::TxOption::get()
{
	return _txOption;
}

/// <summary>
/// Gets or sets additional data to be sent with the response, if any.
/// </summary>
void OnIncomingSubscribeParam::TxOption::set(SipTxOption^ value)
{
	_txOption = value;
}