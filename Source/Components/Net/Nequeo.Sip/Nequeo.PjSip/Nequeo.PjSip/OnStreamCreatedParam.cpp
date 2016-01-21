/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          OnStreamCreatedParam.cpp
*  Purpose :       SIP OnStreamCreatedParam class.
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

#include "OnStreamCreatedParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	This structure contains parameters for Call::onStreamCreated() callback.
///	</summary>
OnStreamCreatedParam::OnStreamCreatedParam()
{
}
/// <summary>
/// Gets or sets the current call.
/// </summary>
Call^ OnStreamCreatedParam::CurrentCall::get()
{
	return _currentCall;
}

/// <summary>
/// Gets or sets the current call.
/// </summary>
void OnStreamCreatedParam::CurrentCall::set(Call^ value)
{
	_currentCall = value;
}

/// <summary>
/// Gets or sets the stream index in the media session.
/// </summary>
unsigned OnStreamCreatedParam::StreamIndex::get()
{
	return _streamIndex;
}

/// <summary>
/// Gets or sets the stream index in the media session.
/// </summary>
void OnStreamCreatedParam::StreamIndex::set(unsigned value)
{
	_streamIndex = value;
}