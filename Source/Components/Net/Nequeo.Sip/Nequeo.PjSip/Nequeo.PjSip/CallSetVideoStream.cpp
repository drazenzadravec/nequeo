/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallSetVideoStream.cpp
*  Purpose :       SIP CallSetVideoStream class.
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

#include "CallSetVideoStream.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// This structure contains parameters for Call::vidSetStream()
/// </summary>
CallSetVideoStreamParam::CallSetVideoStreamParam() :
	_disposed(false)
{
}

/// <summary>
/// This structure contains parameters for Call::vidSetStream()
/// </summary>
CallSetVideoStreamParam::~CallSetVideoStreamParam()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Gets or sets the specify the media stream index. This can be set to -1 to denote
/// the default video stream in the call, which is the first active
/// video stream or any first video stream if none is active.
/// This field is valid for all video stream operations, except
/// PJSUA_CALL_VID_STRM_ADD.
///
/// Default: -1 (first active video stream, or any first video stream
///             if none is active)
/// </summary>
int CallSetVideoStreamParam::MediaIndex::get()
{
	return _mediaIndex;
}

/// <summary>
/// Gets or sets the specify the media stream index. This can be set to -1 to denote
/// the default video stream in the call, which is the first active
/// video stream or any first video stream if none is active.
/// This field is valid for all video stream operations, except
/// PJSUA_CALL_VID_STRM_ADD.
///
/// Default: -1 (first active video stream, or any first video stream
///             if none is active)
/// </summary>
void CallSetVideoStreamParam::MediaIndex::set(int value)
{
	_mediaIndex = value;
}

/// <summary>
/// Gets or sets the Specify the media stream direction.
/// This field is valid for the following video stream operations :
/// PJSUA_CALL_VID_STRM_ADD and PJSUA_CALL_VID_STRM_CHANGE_DIR.
/// Default : PJMEDIA_DIR_ENCODING_DECODING
/// </summary>
MediaDirection CallSetVideoStreamParam::Direction::get()
{
	return _direction;
}

/// <summary>
/// Gets or sets the Specify the media stream direction.
/// This field is valid for the following video stream operations :
/// PJSUA_CALL_VID_STRM_ADD and PJSUA_CALL_VID_STRM_CHANGE_DIR.
/// Default : PJMEDIA_DIR_ENCODING_DECODING
/// </summary>
void CallSetVideoStreamParam::Direction::set(MediaDirection value)
{
	_direction = value;
}

/// <summary>
/// Gets or sets the specify the video capture device ID. This can be set to
/// PJMEDIA_VID_DEFAULT_CAPTURE_DEV to specify the default capture
/// device as configured in the account.
/// This field is valid for the following video stream operations :
/// PJSUA_CALL_VID_STRM_ADD and PJSUA_CALL_VID_STRM_CHANGE_CAP_DEV.
/// Default : PJMEDIA_VID_DEFAULT_CAPTURE_DEV.
/// </summary>
int CallSetVideoStreamParam::CaptureDevice::get()
{
	return _captureDevice;
}

/// <summary>
/// Gets or sets the specify the video capture device ID. This can be set to
/// PJMEDIA_VID_DEFAULT_CAPTURE_DEV to specify the default capture
/// device as configured in the account.
/// This field is valid for the following video stream operations :
/// PJSUA_CALL_VID_STRM_ADD and PJSUA_CALL_VID_STRM_CHANGE_CAP_DEV.
/// Default : PJMEDIA_VID_DEFAULT_CAPTURE_DEV.
/// </summary>
void CallSetVideoStreamParam::CaptureDevice::set(int value)
{
	_captureDevice = value;
}