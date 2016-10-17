/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnCreateMediaTransportParam.cpp
*  Purpose :       SIP OnCreateMediaTransportParam class.
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

#include "OnCreateMediaTransportParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	This structure contains parameters for Call::onCreateMediaTransport() callback.
///	</summary>
OnCreateMediaTransportParam::OnCreateMediaTransportParam()
{
}
/// <summary>
/// Gets or sets the current call.
/// </summary>
Call^ OnCreateMediaTransportParam::CurrentCall::get()
{
	return _currentCall;
}

/// <summary>
/// Gets or sets the current call.
/// </summary>
void OnCreateMediaTransportParam::CurrentCall::set(Call^ value)
{
	_currentCall = value;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
CallInfo^ OnCreateMediaTransportParam::Info::get()
{
	return _info;
}

/// <summary>
/// Gets or sets the call information.
/// </summary>
void OnCreateMediaTransportParam::Info::set(CallInfo^ value)
{
	_info = value;
}

/// <summary>
/// Gets or sets the media index in the SDP for which this media transport will be used.
/// </summary>
unsigned OnCreateMediaTransportParam::MediaIndex::get()
{
	return _mediaIndex;
}

/// <summary>
/// Gets or sets the media index in the SDP for which this media transport will be used.
/// </summary>
void OnCreateMediaTransportParam::MediaIndex::set(unsigned value)
{
	_mediaIndex = value;
}

/// <summary>
/// Gets or sets the bitmask from pjsua_create_media_transport_flag.
/// </summary>
unsigned OnCreateMediaTransportParam::Flags::get()
{
	return _flags;
}

/// <summary>
/// Gets or sets the bitmask from pjsua_create_media_transport_flag.
/// </summary>
void OnCreateMediaTransportParam::Flags::set(unsigned value)
{
	_flags = value;
}