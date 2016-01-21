/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnCallRxOfferParam.cpp
*  Purpose :       SIP OnCallRxOfferParam class.
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

#include "OnCallRxOfferParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	This structure contains parameters for Call::onCallRxOffer() callback.
///	</summary>
OnCallRxOfferParam::OnCallRxOfferParam()
{
}
/// <summary>
/// Gets or sets the current call.
/// </summary>
Call^ OnCallRxOfferParam::CurrentCall::get()
{
	return _currentCall;
}

/// <summary>
/// Gets or sets the current call.
/// </summary>
void OnCallRxOfferParam::CurrentCall::set(Call^ value)
{
	_currentCall = value;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
CallInfo^ OnCallRxOfferParam::Info::get()
{
	return _info;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
void OnCallRxOfferParam::Info::set(CallInfo^ value)
{
	_info = value;
}

/// <summary>
/// Gets or sets the new offer received.
/// </summary>
SdpSession^ OnCallRxOfferParam::Offer::get()
{
	return _offer;
}

/// <summary>
/// Gets or sets the new offer received.
/// </summary>
void OnCallRxOfferParam::Offer::set(SdpSession^ value)
{
	_offer = value;
}

/// <summary>
/// Gets or sets the current call setting, application can update this setting for answering the offer.
/// </summary>
CallSetting^ OnCallRxOfferParam::Setting::get()
{
	return _setting;
}

/// <summary>
/// Gets or sets the current call setting, application can update this setting for answering the offer.
/// </summary>
void OnCallRxOfferParam::Setting::set(CallSetting^ value)
{
	_setting = value;
}

/// <summary>
/// Gets or sets the Status code to be returned for answering the offer. On input,
/// it contains status code 200. Currently, valid values are only 200 and 488.
/// </summary>
StatusCode OnCallRxOfferParam::Code::get()
{
	return _code;
}

/// <summary>
/// Gets or sets the Status code to be returned for answering the offer. On input,
/// it contains status code 200. Currently, valid values are only 200 and 488.
/// </summary>
void OnCallRxOfferParam::Code::set(StatusCode value)
{
	_code = value;
}