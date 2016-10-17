/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnMwiInfoParam.cpp
*  Purpose :       SIP OnMwiInfoParam class.
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

#include "OnMwiInfoParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	On MWI info paramters.
///	</summary>
OnMwiInfoParam::OnMwiInfoParam()
{
}

/// <summary>
/// Gets or sets the incoming response that causes this callback to be called.
/// If the transaction fails because of time out or transport error,
/// the content will be empty.
/// </summary>
SipRxData^ OnMwiInfoParam::RxData::get()
{
	return _rxData;
}

/// <summary>
/// Gets or sets the incoming response that causes this callback to be called.
/// If the transaction fails because of time out or transport error,
/// the content will be empty.
/// </summary>
void OnMwiInfoParam::RxData::set(SipRxData^ value)
{
	_rxData = value;
}

/// <summary>
/// Gets or sets the MWI subscription state.
/// </summary>
SubscriptionState OnMwiInfoParam::State::get()
{
	return _state;
}

/// <summary>
/// Gets or sets the MWI subscription state.
/// </summary>
void OnMwiInfoParam::State::set(SubscriptionState value)
{
	_state = value;
}