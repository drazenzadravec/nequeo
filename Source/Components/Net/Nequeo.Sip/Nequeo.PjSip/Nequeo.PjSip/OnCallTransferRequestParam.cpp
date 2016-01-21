/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnCallTransferRequestParam.cpp
*  Purpose :       SIP OnCallTransferRequestParam class.
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

#include "OnCallTransferRequestParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	This structure contains parameters for Call::onCallTransferRequest() callback.
///	</summary>
OnCallTransferRequestParam::OnCallTransferRequestParam()
{
}
/// <summary>
/// Gets or sets the current call.
/// </summary>
Call^ OnCallTransferRequestParam::CurrentCall::get()
{
	return _currentCall;
}

/// <summary>
/// Gets or sets the current call.
/// </summary>
void OnCallTransferRequestParam::CurrentCall::set(Call^ value)
{
	_currentCall = value;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
CallInfo^ OnCallTransferRequestParam::Info::get()
{
	return _info;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
void OnCallTransferRequestParam::Info::set(CallInfo^ value)
{
	_info = value;
}

/// <summary>
/// Gets or sets the status progress of the transfer request.
/// </summary>
StatusCode OnCallTransferRequestParam::Code::get()
{
	return _code;
}

/// <summary>
/// Gets or sets the status progress of the transfer request.
/// </summary>
void OnCallTransferRequestParam::Code::set(StatusCode value)
{
	_code = value;
}

/// <summary>
/// Gets or sets the destination where the call will be transferred to.
/// </summary>
String^ OnCallTransferRequestParam::DestinationUri::get()
{
	return _destinationUri;
}

/// <summary>
/// Gets or sets the destination where the call will be transferred to.
/// </summary>
void OnCallTransferRequestParam::DestinationUri::set(String^ value)
{
	_destinationUri = value;
}

/// <summary>
/// Gets or sets the current call setting, application can update this setting
/// for the call being transferred.
/// </summary>
CallSetting^ OnCallTransferRequestParam::Setting::get()
{
	return _setting;
}

/// <summary>
/// Gets or sets the current call setting, application can update this setting
/// for the call being transferred.
/// </summary>
void OnCallTransferRequestParam::Setting::set(CallSetting^ value)
{
	_setting = value;
}