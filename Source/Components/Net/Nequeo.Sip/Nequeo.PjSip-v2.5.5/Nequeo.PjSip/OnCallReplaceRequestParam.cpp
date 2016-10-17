/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnCallReplaceRequestParam.cpp
*  Purpose :       SIP OnCallReplaceRequestParam class.
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

#include "OnCallReplaceRequestParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	This structure contains parameters for Call::onCallReplaceRequest() callback.
///	</summary>
OnCallReplaceRequestParam::OnCallReplaceRequestParam()
{
}
/// <summary>
/// Gets or sets the current call.
/// </summary>
Call^ OnCallReplaceRequestParam::CurrentCall::get()
{
	return _currentCall;
}

/// <summary>
/// Gets or sets the current call.
/// </summary>
void OnCallReplaceRequestParam::CurrentCall::set(Call^ value)
{
	_currentCall = value;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
CallInfo^ OnCallReplaceRequestParam::Info::get()
{
	return _info;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
void OnCallReplaceRequestParam::Info::set(CallInfo^ value)
{
	_info = value;
}

/// <summary>
/// Gets or sets the incoming INVITE request to replace the call.
/// </summary>
SipRxData^ OnCallReplaceRequestParam::RxData::get()
{
	return _rxData;
}

/// <summary>
/// Gets or sets the incoming INVITE request to replace the call.
/// </summary>
void OnCallReplaceRequestParam::RxData::set(SipRxData^ value)
{
	_rxData = value;
}

/// <summary>
/// Gets or sets the current call setting, application can update this setting for the call being replaced.
/// </summary>
CallSetting^ OnCallReplaceRequestParam::Setting::get()
{
	return _setting;
}

/// <summary>
/// Gets or sets the current call setting, application can update this setting for the call being replaced.
/// </summary>
void OnCallReplaceRequestParam::Setting::set(CallSetting^ value)
{
	_setting = value;
}

/// <summary>
/// Gets or sets the optional status text to be set by application.
/// </summary>
String^ OnCallReplaceRequestParam::Reason::get()
{
	return _reason;
}

/// <summary>
/// Gets or sets the optional status text to be set by application.
/// </summary>
void OnCallReplaceRequestParam::Reason::set(String^ value)
{
	_reason = value;
}

/// <summary>
/// Gets or sets the Status code to be set by application. Application should only
/// return a final status(200 - 699).
/// </summary>
StatusCode OnCallReplaceRequestParam::Code::get()
{
	return _code;
}

/// <summary>
/// Gets or sets the Status code to be set by application. Application should only
/// return a final status(200 - 699).
/// </summary>
void OnCallReplaceRequestParam::Code::set(StatusCode value)
{
	_code = value;
}