/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoWindowInfo.cpp
*  Purpose :       SIP VideoWindowInfo class.
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

#include "VideoWindowInfo.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// This structure describes video window info.
/// </summary>
VideoWindowInfo::VideoWindowInfo() :  _disposed(false)
{
}

///	<summary>
///	This structure describes video window info.
///	</summary>
VideoWindowInfo::~VideoWindowInfo()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Gets or sets the flag to indicate whether this window is a native window,
/// such as created by built - in preview device.If this field is
/// true, only the video window handle field of this
/// structure is valid.
/// </summary>
bool VideoWindowInfo::IsNative::get()
{
	return _isNative;
}

/// <summary>
/// Gets or sets the flag to indicate whether this window is a native window,
/// such as created by built - in preview device.If this field is
/// true, only the video window handle field of this
/// structure is valid.
/// </summary>
void VideoWindowInfo::IsNative::set(bool value)
{
	_isNative = value;
}

/// <summary>
/// Gets or sets the video window handle.
/// </summary>
VideoWindowHandle^ VideoWindowInfo::WindowHandle::get()
{
	return _windowHandle;
}

/// <summary>
/// Gets or sets the video window handle.
/// </summary>
void VideoWindowInfo::WindowHandle::set(VideoWindowHandle^ value)
{
	_windowHandle = value;
}

/// <summary>
/// Gets or sets the renderer device ID.
/// </summary>
int VideoWindowInfo::RenderDeviceId::get()
{
	return _renderDeviceId;
}

/// <summary>
/// Gets or sets the renderer device ID.
/// </summary>
void VideoWindowInfo::RenderDeviceId::set(int value)
{
	_renderDeviceId = value;
}

/// <summary>
/// Gets or sets the window show status. The window is hidden if false.
/// </summary>
bool VideoWindowInfo::Show::get()
{
	return _show;
}

/// <summary>
/// Gets or sets the window show status. The window is hidden if false.
/// </summary>
void VideoWindowInfo::Show::set(bool value)
{
	_show = value;
}

/// <summary>
/// Gets or sets the window position.
/// </summary>
MediaCoordinate^ VideoWindowInfo::WindowPosition::get()
{
	return _windowPosition;
}

/// <summary>
/// Gets or sets the window position.
/// </summary>
void VideoWindowInfo::WindowPosition::set(MediaCoordinate^ value)
{
	_windowPosition = value;
}

/// <summary>
/// Gets or sets the window size.
/// </summary>
MediaSize^ VideoWindowInfo::WindowSize::get()
{
	return _windowSize;
}

/// <summary>
/// Gets or sets the window size.
/// </summary>
void VideoWindowInfo::WindowSize::set(MediaSize^ value)
{
	_windowSize = value;
}