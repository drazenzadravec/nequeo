/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallCallback.cpp
*  Purpose :       SIP Call Callback class.
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

#include "CallCallback.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Call callbacks.
///	</summary>
/// <param name="account">The Sip account.</param>
/// <param name="callId">An index call id (0 - 3).</param>
CallCallback::CallCallback(AccountCallback& account, int callId) : pj::Call(account, callId),
	_disposed(false)
{
}

///	<summary>
///	Contact callbacks.
///	</summary>
CallCallback::~CallCallback()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Notify application when call state has changed.
/// Application may then query the call info to get the
/// detail call states by calling getInfo() function.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallState(pj::OnCallStateParam &prm)
{
	_onCallState_function_internal(prm);
}

/// <summary>
/// Notify application when media state in the call has changed.
/// Normal application would need to implement this callback, e.g.
/// to connect the call's media to sound device. When ICE is used,
/// this callback will also be called to report ICE negotiation failure.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallMediaState(pj::OnCallMediaStateParam &prm)
{
	_onCallMediaState_function_internal(prm);
}

/// <summary>
/// This is a general notification callback which is called whenever
/// a transaction within the call has changed state.Application can
/// implement this callback for example to monitor the state of
/// outgoing requests, or to answer unhandled incoming requests
/// (such as INFO) with a final response.
/// </summary>
/// <param name="prm">Callback parameter.</param>
void CallCallback::onCallTsxState(pj::OnCallTsxStateParam &prm)
{
	_onCallTsxState_function_internal(prm);
}

///	<summary>
///	Set the on call state function callback.
///	</summary>
/// <param name="onCallMediaStateCallBack">The on call state function callback.</param>
void CallCallback::Set_OnCallState_Function(OnCallState_Function onCallStateCallBack)
{
	_onCallState_function_internal = onCallStateCallBack;
}

///	<summary>
///	Set the on call media state function callback.
///	</summary>
/// <param name="onCallMediaStateCallBack">The on call media state function callback.</param>
void CallCallback::Set_OnCallMediaState_Function(OnCallMediaState_Function onCallMediaStateCallBack)
{
	_onCallMediaState_function_internal = onCallMediaStateCallBack;
}

///	<summary>
///	Set the on call tsx state function callback.
///	</summary>
/// <param name="onCallTsxStateCallBack">The on call tsx state function callback.</param>
void CallCallback::Set_OnCallTsxState_Function(OnCallTsxState_Function onCallTsxStateCallBack)
{
	_onCallTsxState_function_internal = onCallTsxStateCallBack;
}