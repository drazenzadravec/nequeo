/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallMediaInfo.cpp
*  Purpose :       SIP CallMediaInfo class.
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

#include "CallMediaInfo.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Call media information.
/// </summary>
CallMediaInfo::CallMediaInfo() :
	_disposed(false)
{
}

///	<summary>
///	Call media information deconstructor.
///	</summary>
CallMediaInfo::~CallMediaInfo()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Gets or sets the media index in SDP.
/// </summary>
unsigned CallMediaInfo::Index::get()
{
	return _index;
}

/// <summary>
/// Gets or sets the media index in SDP.
/// </summary>
void CallMediaInfo::Index::set(unsigned value)
{
	_index = value;
}

/// <summary>
/// Gets or sets the media type.
/// </summary>
MediaType CallMediaInfo::Type::get()
{
	return _type;
}

/// <summary>
/// Gets or sets the media type.
/// </summary>
void CallMediaInfo::Type::set(MediaType value)
{
	_type = value;
}

/// <summary>
/// Gets or sets the media direction.
/// </summary>
MediaDirection CallMediaInfo::Direction::get()
{
	return _direction;
}

/// <summary>
/// Gets or sets the media direction.
/// </summary>
void CallMediaInfo::Direction::set(MediaDirection value)
{
	_direction = value;
}

/// <summary>
/// Gets or sets the call media status.
/// </summary>
CallMediaStatus CallMediaInfo::Status::get()
{
	return _status;
}

/// <summary>
/// Gets or sets the call media status.
/// </summary>
void CallMediaInfo::Status::set(CallMediaStatus value)
{
	_status = value;
}

/// <summary>
/// Gets or sets the conference port number for the call. Only valid if the media type is audio.
/// </summary>
int CallMediaInfo::AudioConfSlot::get()
{
	return _audioConfSlot;
}

/// <summary>
/// Gets or sets the conference port number for the call. Only valid if the media type is audio.
/// </summary>
void CallMediaInfo::AudioConfSlot::set(int value)
{
	_audioConfSlot = value;
}

///	<summary>
///	Gets or sets the window id for incoming video, if any, or
/// PJSUA_INVALID_ID. Only valid if the media type is video.
///	</summary>
int CallMediaInfo::VideoIncomingWindowId::get()
{
	return _videoIncomingWindowId;
}

///	<summary>
///	Gets or sets the window id for incoming video, if any, or
/// PJSUA_INVALID_ID. Only valid if the media type is video.
///	</summary>
void CallMediaInfo::VideoIncomingWindowId::set(int value)
{
	_videoIncomingWindowId = value;
}

///	<summary>
///	Gets or sets the video capture device for outgoing transmission, if any,
/// or PJMEDIA_VID_INVALID_DEV.Only valid if the media type is video.
///	</summary>
int CallMediaInfo::VideoCapDev::get()
{
	return _videoCapDev;
}

///	<summary>
///	Gets or sets the video capture device for outgoing transmission, if any,
/// or PJMEDIA_VID_INVALID_DEV.Only valid if the media type is video.
///	</summary>
void CallMediaInfo::VideoCapDev::set(int value)
{
	_videoCapDev = value;
}