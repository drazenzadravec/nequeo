/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnCallTransferStatusParam.cpp
*  Purpose :       SIP OnCallTransferStatusParam class.
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

#include "OnCallTransferStatusParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	This structure contains parameters for Call::onCallTransferStatus() callback.
///	</summary>
OnCallTransferStatusParam::OnCallTransferStatusParam()
{
}
/// <summary>
/// Gets or sets the current call.
/// </summary>
Call^ OnCallTransferStatusParam::CurrentCall::get()
{
	return _currentCall;
}

/// <summary>
/// Gets or sets the current call.
/// </summary>
void OnCallTransferStatusParam::CurrentCall::set(Call^ value)
{
	_currentCall = value;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
CallInfo^ OnCallTransferStatusParam::Info::get()
{
	return _info;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
void OnCallTransferStatusParam::Info::set(CallInfo^ value)
{
	_info = value;
}

/// <summary>
/// Gets or sets the status progress of the transfer request.
/// </summary>
StatusCode OnCallTransferStatusParam::Code::get()
{
	return _code;
}

/// <summary>
/// Gets or sets the status progress of the transfer request.
/// </summary>
void OnCallTransferStatusParam::Code::set(StatusCode value)
{
	_code = value;
}

/// <summary>
/// Gets or sets the status progress reason.
/// </summary>
String^ OnCallTransferStatusParam::Reason::get()
{
	return _reason;
}

/// <summary>
/// Gets or sets the status progress reason.
/// </summary>
void OnCallTransferStatusParam::Reason::set(String^ value)
{
	_reason = value;
}

/// <summary>
/// Gets or sets the If true, no further notification will be reported. The statusCode
/// specified in this callback is the final status.
/// </summary>
bool OnCallTransferStatusParam::FinalNotify::get()
{
	return _finalNotify;
}

/// <summary>
/// Gets or sets the If true, no further notification will be reported. The statusCode
/// specified in this callback is the final status.
/// </summary>
void OnCallTransferStatusParam::FinalNotify::set(bool value)
{
	_finalNotify = value;
}

/// <summary>
/// Gets or sets the Initially will be set to true, application can set this to false
/// if it no longer wants to receive further notification (for example,
/// after it hangs up the call).
/// </summary>
bool OnCallTransferStatusParam::Continue::get()
{
	return _continue;
}

/// <summary>
/// Gets or sets the Initially will be set to true, application can set this to false
/// if it no longer wants to receive further notification (for example,
/// after it hangs up the call).
/// </summary>
void OnCallTransferStatusParam::Continue::set(bool value)
{
	_continue = value;
}