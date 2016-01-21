/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnCallMediaEventParam.cpp
*  Purpose :       SIP OnCallMediaEventParam class.
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

#include "OnCallMediaEventParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	This structure contains parameters for Call::onCallMediaEvent() callback.
///	</summary>
OnCallMediaEventParam::OnCallMediaEventParam()
{
}

/// <summary>
/// Gets or sets the current call.
/// </summary>
Call^ OnCallMediaEventParam::CurrentCall::get()
{
	return _currentCall;
}

/// <summary>
/// Gets or sets the current call.
/// </summary>
void OnCallMediaEventParam::CurrentCall::set(Call^ value)
{
	_currentCall = value;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
CallInfo^ OnCallMediaEventParam::Info::get()
{
	return _info;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
void OnCallMediaEventParam::Info::set(CallInfo^ value)
{
	_info = value;
}

/// <summary>
/// Gets or sets the media stream index.
/// </summary>
unsigned OnCallMediaEventParam::MediaIndex::get()
{
	return _mediaIndex;
}

/// <summary>
/// Gets or sets the media stream index.
/// </summary>
void OnCallMediaEventParam::MediaIndex::set(unsigned value)
{
	_mediaIndex = value;
}

/// <summary>
/// Gets or sets the media event.
/// </summary>
MediaEvent^ OnCallMediaEventParam::Event::get()
{
	return _event;
}

/// <summary>
/// Gets or sets the media event.
/// </summary>
void OnCallMediaEventParam::Event::set(MediaEvent^ value)
{
	_event = value;
}