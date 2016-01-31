/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoPreviewOpParam.cpp
*  Purpose :       SIP VideoPreviewOpParam class.
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

#include "VideoPreviewOpParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	This structure contains parameters for VideoPreview::start()
///	</summary>
VideoPreviewOpParam::VideoPreviewOpParam()
{
}

/// <summary>
/// Gets or sets the Device ID for the video renderer to be used for rendering the
/// capture stream for preview.This parameter is ignored if native
/// preview is being used.
/// </summary>
int VideoPreviewOpParam::RenderID::get()
{
	return _renderID;
}

/// <summary>
/// Gets or sets the Device ID for the video renderer to be used for rendering the
/// capture stream for preview.This parameter is ignored if native
/// preview is being used.
/// </summary>
void VideoPreviewOpParam::RenderID::set(int value)
{
	_renderID = value;
}

/// <summary>
/// Gets or sets the show window initially.
/// </summary>
bool VideoPreviewOpParam::Show::get()
{
	return _show;
}

/// <summary>
/// Gets or sets the show window initially.
/// </summary>
void VideoPreviewOpParam::Show::set(bool value)
{
	_show = value;
}

/// <summary>
/// Gets or sets the Window flags. The value is a bitmask combination of
/// \a pjmedia_vid_dev_wnd_flag.
/// </summary>
unsigned VideoPreviewOpParam::WindowFlags::get()
{
	return _windowFlags;
}

/// <summary>
/// Gets or sets the Window flags. The value is a bitmask combination of
/// \a pjmedia_vid_dev_wnd_flag.
/// </summary>
void VideoPreviewOpParam::WindowFlags::set(unsigned value)
{
	_windowFlags = value;
}

/// <summary>
/// Gets or sets the media format. If left unitialized, this parameter will not be used.
/// </summary>
MediaFormat^ VideoPreviewOpParam::Format::get()
{
	return _format;
}

/// <summary>
/// Gets or sets the media format. If left unitialized, this parameter will not be used.
/// </summary>
void VideoPreviewOpParam::Format::set(MediaFormat^ value)
{
	_format = value;
}

/// <summary>
/// Gets or sets the Optional output window to be used to display the video preview.
/// This parameter will only be used if the video device supports
/// PJMEDIA_VID_DEV_CAP_OUTPUT_WINDOW capability and the capability
/// is not read - only.
/// </summary>
VideoWindowHandle^ VideoPreviewOpParam::Window::get()
{
	return _window;
}

/// <summary>
/// Gets or sets the Optional output window to be used to display the video preview.
/// This parameter will only be used if the video device supports
/// PJMEDIA_VID_DEV_CAP_OUTPUT_WINDOW capability and the capability
/// is not read - only.
/// </summary>
void VideoPreviewOpParam::Window::set(VideoWindowHandle^ value)
{
	_window = value;
}