/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallSetting.cpp
*  Purpose :       SIP CallSetting class.
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

#include "CallSetting.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Call settings.
///	</summary>
CallSetting::CallSetting() :
	_disposed(false), _useDefaultValues(false)
{
}

///	<summary>
///	Call settings.
///	</summary>
/// <param name="useDefaultValues">Use default values.</param>
CallSetting::CallSetting(bool useDefaultValues) :
	_disposed(false)
{
	_useDefaultValues = useDefaultValues;
}

///	<summary>
///	Call settings.
///	</summary>
CallSetting::~CallSetting()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Gets or sets the bitmask of CallFlag constants.
///	</summary>
CallFlag CallSetting::Flag::get()
{
	return _flag;
}

///	<summary>
///	Gets or sets the bitmask of CallFlag constants.
///	</summary>
void CallSetting::Flag::set(CallFlag value)
{
	_flag = value;
}

///	<summary>
///	Gets or sets this flag controls what methods to request keyframe are allowed on
/// the call. Value is bitmask of VidReqKeyframeMethod.
///	</summary>
VidReqKeyframeMethod CallSetting::ReqKeyframeMethod::get()
{
	return _reqKeyframeMethod;
}

///	<summary>
///	Gets or sets this flag controls what methods to request keyframe are allowed on
/// the call. Value is bitmask of VidReqKeyframeMethod.
///	</summary>
void CallSetting::ReqKeyframeMethod::set(VidReqKeyframeMethod value)
{
	_reqKeyframeMethod = value;
}

///	<summary>
///	Gets or sets the number of simultaneous active audio streams for this call. Setting
/// this to zero will disable audio in this call.
///	</summary>
unsigned CallSetting::AudioCount::get()
{
	return _audioCount;
}

///	<summary>
///	Gets or sets the number of simultaneous active audio streams for this call. Setting
/// this to zero will disable audio in this call.
///	</summary>
void CallSetting::AudioCount::set(unsigned value)
{
	_audioCount = value;
}

///	<summary>
///	Gets or sets the number of simultaneous active video streams for this call. Setting
/// this to zero will disable video in this call.
///	</summary>
unsigned CallSetting::VideoCount::get()
{
	return _videoCount;
}

///	<summary>
///	Gets or sets the number of simultaneous active video streams for this call. Setting
/// this to zero will disable video in this call.
///	</summary>
void CallSetting::VideoCount::set(unsigned value)
{
	_videoCount = value;
}

///	<summary>
///	Gets the use default values.
///	</summary>
bool CallSetting::UseDefaultValues::get()
{
	return _useDefaultValues;
}