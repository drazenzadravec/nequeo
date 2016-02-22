/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnIncomingCallParam.cpp
*  Purpose :       SIP OnIncomingCallParam class.
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

#include "OnIncomingCallParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	On incoming call paramters.
///	</summary>
OnIncomingCallParam::OnIncomingCallParam()
{
}

/// <summary>
/// Gets or sets the library call ID allocated for the new call.
/// </summary>
int OnIncomingCallParam::CallId::get()
{
	return _callId;
}

/// <summary>
/// Gets or sets the library call ID allocated for the new call.
/// </summary>
void OnIncomingCallParam::CallId::set(int value)
{
	_callId = value;
}

/// <summary>
/// Gets or sets the incoming INVITE request.
/// </summary>
SipRxData^ OnIncomingCallParam::RxData::get()
{
	return _rxData;
}

/// <summary>
/// Gets or sets the incoming INVITE request.
/// </summary>
void OnIncomingCallParam::RxData::set(SipRxData^ value)
{
	_rxData = value;
}