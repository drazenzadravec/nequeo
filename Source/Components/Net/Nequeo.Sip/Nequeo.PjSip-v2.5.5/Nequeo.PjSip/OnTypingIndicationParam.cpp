/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnTypingIndicationParam.cpp
*  Purpose :       SIP OnTypingIndicationParam class.
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

#include "OnTypingIndicationParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	On typing indication paramters.
///	</summary>
OnTypingIndicationParam::OnTypingIndicationParam()
{
}

/// <summary>
/// Gets or sets Contact URI.
/// </summary>
String^ OnTypingIndicationParam::ContactUri::get()
{
	return _contactUri;
}

/// <summary>
/// Gets or sets Contact URI.
/// </summary>
void OnTypingIndicationParam::ContactUri::set(String^ value)
{
	_contactUri = value;
}

/// <summary>
/// Gets or sets sender from URI.
/// </summary>
String^ OnTypingIndicationParam::FromUri::get()
{
	return _fromUri;
}

/// <summary>
/// Gets or sets sender from URI.
/// </summary>
void OnTypingIndicationParam::FromUri::set(String^ value)
{
	_fromUri = value;
}

/// <summary>
/// Gets or sets an indicator if sender is typing.
/// </summary>
bool OnTypingIndicationParam::IsTyping::get()
{
	return _isTyping;
}

/// <summary>
/// Gets or sets an indicator if sender is typing.
/// </summary>
void OnTypingIndicationParam::IsTyping::set(bool value)
{
	_isTyping = value;
}

/// <summary>
/// Gets or sets to URI.
/// </summary>
String^ OnTypingIndicationParam::ToUri::get()
{
	return _toUri;
}

/// <summary>
/// Gets or sets to URI.
/// </summary>
void OnTypingIndicationParam::ToUri::set(String^ value)
{
	_toUri = value;
}

/// <summary>
/// Gets or sets the incoming response that causes this callback to be called.
/// If the transaction fails because of time out or transport error,
/// the content will be empty.
/// </summary>
SipRxData^ OnTypingIndicationParam::RxData::get()
{
	return _rxData;
}

/// <summary>
/// Gets or sets the incoming response that causes this callback to be called.
/// If the transaction fails because of time out or transport error,
/// the content will be empty.
/// </summary>
void OnTypingIndicationParam::RxData::set(SipRxData^ value)
{
	_rxData = value;
}