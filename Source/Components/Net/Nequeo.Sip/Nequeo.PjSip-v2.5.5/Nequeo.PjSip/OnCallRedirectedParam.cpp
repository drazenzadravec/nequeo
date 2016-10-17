/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnCallRedirectedParam.cpp
*  Purpose :       SIP OnCallRedirectedParam class.
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

#include "OnCallRedirectedParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	This structure contains parameters for Call::onCallRedirected() callback.
///	</summary>
OnCallRedirectedParam::OnCallRedirectedParam()
{
}
/// <summary>
/// Gets or sets the current call.
/// </summary>
Call^ OnCallRedirectedParam::CurrentCall::get()
{
	return _currentCall;
}

/// <summary>
/// Gets or sets the current call.
/// </summary>
void OnCallRedirectedParam::CurrentCall::set(Call^ value)
{
	_currentCall = value;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
CallInfo^ OnCallRedirectedParam::Info::get()
{
	return _info;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
void OnCallRedirectedParam::Info::set(CallInfo^ value)
{
	_info = value;
}

/// <summary>
/// Gets or sets the sip event type.
/// </summary>
SipEventType OnCallRedirectedParam::EventType::get()
{
	return _eventType;
}

/// <summary>
/// Gets or sets the sip event type.
/// </summary>
void OnCallRedirectedParam::EventType::set(SipEventType value)
{
	_eventType = value;
}

/// <summary>
/// Gets or sets the current target to be tried.
/// </summary>
String^ OnCallRedirectedParam::TargetUri::get()
{
	return _targetUri;
}

/// <summary>
///Gets or sets the current target to be tried.
/// </summary>
void OnCallRedirectedParam::TargetUri::set(String^ value)
{
	_targetUri = value;
}

/// <summary>
/// Gets or sets the redirect options.
/// </summary>
RedirectResponseType OnCallRedirectedParam::Redirect::get()
{
	return _redirect;
}

/// <summary>
/// Gets or sets the redirect options.
/// </summary>
void OnCallRedirectedParam::Redirect::set(RedirectResponseType value)
{
	_redirect = value;
}