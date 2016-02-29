/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoWindow.cpp
*  Purpose :       SIP VideoWindow class.
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

#include "VideoWindow.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Video window.
/// </summary>
/// <param name="pjVideoWindow">The video window.</param>
VideoWindow::VideoWindow(pj::VideoWindow& pjVideoWindow) :
	_pjVideoWindow(pjVideoWindow), _disposed(false), _isVideoPreview(false), _isCallMediaInfo(false), _videoWindowID(-1)
{
}

///	<summary>
///	Video window deconstructor.
///	</summary>
VideoWindow::~VideoWindow()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Set video preview.
/// </summary>
/// <param name="pjVideoPreview">The video preview.</param>
void VideoWindow::SetVideoPreviewRef(pj::VideoPreview* pjVideoPreview)
{
	_isVideoPreview = true;
	_pjVideoPreview = pjVideoPreview;
}

/// <summary>
/// Set call media info.
/// </summary>
/// <param name="pjCallMediaInfo">Thecall media info.</param>
void VideoWindow::SetCallMediaInfoRef(pj::CallMediaInfo* pjCallMediaInfo)
{
	_isCallMediaInfo = true;
	_pjCallMediaInfo = pjCallMediaInfo;
}

/// <summary>
/// Get window info.
/// </summary>
/// <returns>The video window info.</returns>
VideoWindowInfo^ VideoWindow::GetInfo()
{
	VideoWindowInfo^ videoInfo = nullptr;
	pj::VideoWindowInfo info;

	try
	{
		// If is video preview ref.
		if (_isVideoPreview)
		{
			pj::VideoWindow pjWindow = _pjVideoPreview->getVideoWindow();
			info = pjWindow.getInfo();
		}
		else if (_isCallMediaInfo)
		{
			pj::VideoWindow pjWindowCall = _pjCallMediaInfo->videoWindow;
			info = pjWindowCall.getInfo();
		}
		else
		{
			// Use default.
			info = _pjVideoWindow.getInfo();
		}

		// Create the video info.
		videoInfo = gcnew VideoWindowInfo();

		videoInfo->IsNative = info.isNative;
		videoInfo->RenderDeviceId = info.renderDeviceId;
		videoInfo->Show = info.show;

		videoInfo->WindowHandle = gcnew VideoWindowHandle();
		videoInfo->WindowHandle->Type = GeVideoDeviceHandleTypeEx(info.winHandle.type);

		videoInfo->WindowPosition = gcnew MediaCoordinate();
		videoInfo->WindowPosition->X = info.pos.x;
		videoInfo->WindowPosition->Y = info.pos.y;

		videoInfo->WindowSize = gcnew MediaSize();
		videoInfo->WindowSize->Height = info.size.h;
		videoInfo->WindowSize->Width = info.size.w;
	}
	catch (const pj::Error& pjError)
	{
		std::string reason = pjError.reason;
		videoInfo = nullptr;
	}
	catch (const std::exception& ex)
	{
		std::string reason = ex.what();
		videoInfo = nullptr;
	}

	// Return the video window info;
	return videoInfo;
}

/// <summary>
/// Show or hide window. This operation is not valid for native windows
/// (VideoWindowInfo.isNative = true), on which native windowing API
/// must be used instead.
/// </summary>
/// <param name="show">Set to true to show the window, false to hide the window.</param>
void VideoWindow::Show(bool show)
{
	// If is video preview ref.
	if (_isVideoPreview)
	{
		pj::VideoWindow pjWindow = _pjVideoPreview->getVideoWindow();
		pjWindow.Show(show);
	}
	else if (_isCallMediaInfo)
	{
		pj::VideoWindow pjWindowCall = _pjCallMediaInfo->videoWindow;
		pjWindowCall.Show(show);
	}
	else
	{
		// Use default.
		_pjVideoWindow.Show(show);
	}
}

/// <summary>
/// Set video window position. This operation is not valid for native windows
/// (VideoWindowInfo.isNative = true), on which native windowing API
/// must be used instead.
/// </summary>
/// <param name="position">The window position.</param>
void VideoWindow::SetPosition(MediaCoordinate^ position)
{
	pj::MediaCoordinate pos;

	pos.x = position->X;
	pos.y = position->Y;

	// If is video preview ref.
	if (_isVideoPreview)
	{
		pj::VideoWindow pjWindow = _pjVideoPreview->getVideoWindow();
		pjWindow.setPos(pos);
	}
	else if (_isCallMediaInfo)
	{
		pj::VideoWindow pjWindowCall = _pjCallMediaInfo->videoWindow;
		pjWindowCall.setPos(pos);
	}
	else
	{
		// Set the position.
		_pjVideoWindow.setPos(pos);
	}
}

/// <summary>
/// Resize window. This operation is not valid for native windows
/// (VideoWindowInfo.isNative = true), on which native windowing API
/// must be used instead.
/// </summary>
/// <param name="size">The new window size.</param>
void VideoWindow::SetSize(MediaSize^ size)
{
	pj::MediaSize pjSize;

	pjSize.h = size->Height;
	pjSize.w = size->Width;

	// If is video preview ref.
	if (_isVideoPreview)
	{
		pj::VideoWindow pjWindow = _pjVideoPreview->getVideoWindow();
		pjWindow.setSize(pjSize);
	}
	else if (_isCallMediaInfo)
	{
		pj::VideoWindow pjWindowCall = _pjCallMediaInfo->videoWindow;
		pjWindowCall.setSize(pjSize);
	}
	else
	{
		// Set the size.
		_pjVideoWindow.setSize(pjSize);
	}
}

/// <summary>
/// Rotate the video window. This function will change the video orientation
/// and also possibly the video window size(width and height get swapped).
/// This operation is not valid for native windows(VideoWindowInfo. 
/// isNative = true), on which native windowing API must be used instead.
/// </summary>
/// <param name="angle">The rotation angle in degrees, must be multiple of 90.
///	Specify positive value for clockwise rotation or
/// negative value for counter - clockwise rotation.</param>
void VideoWindow::Rotate(int angle)
{
	// If is video preview ref.
	if (_isVideoPreview)
	{
		pj::VideoWindow pjWindow = _pjVideoPreview->getVideoWindow();
		pjWindow.rotate(angle);
	}
	else if (_isCallMediaInfo)
	{
		pj::VideoWindow pjWindowCall = _pjCallMediaInfo->videoWindow;
		pjWindowCall.rotate(angle);
	}
	else
	{
		_pjVideoWindow.rotate(angle);
	}
}

/// <summary>
/// Set output window. This operation is valid only when the underlying
/// video device supports PJMEDIA_VIDEO_DEV_CAP_OUTPUT_WINDOW capability AND
/// allows the output window to be changed on - the - fly, otherwise Error will
/// be thrown.Currently it is only supported on Android.
/// </summary>
/// <param name="window">The new output window.</param>
void VideoWindow::SetWindow(VideoWindowHandle^ window)
{
	pj::VideoWindowHandle win;

	win.type = GeVideoDeviceHandleType(window->Type);

	// If is video preview ref.
	if (_isVideoPreview)
	{
		pj::VideoWindow pjWindow = _pjVideoPreview->getVideoWindow();
		pjWindow.setWindow(win);
	}
	else if (_isCallMediaInfo)
	{
		pj::VideoWindow pjWindowCall = _pjCallMediaInfo->videoWindow;
		pjWindowCall.setWindow(win);
	}
	else
	{
		// Set the window.
		_pjVideoWindow.setWindow(win);
	}
}

///	<summary>
///	Get the video window id.
///	</summary>
/// <returns>The video window id.</returns>
int VideoWindow::GetVideoWindowID()
{
	return _videoWindowID;
}

/// <summary>
/// Set the video window id.
/// </summary>
/// <param name="videoWindowID">The video window id.</param>
void VideoWindow::SetVideoWindowID(int videoWindowID)
{
	_videoWindowID = videoWindowID;
}

///	<summary>
///	Get the VideoDeviceHandleType.
///	</summary>
/// <param name="videoDeviceHandleType">The VideoDeviceHandleType.</param>
/// <returns>The VideoDeviceHandleType.</returns>
VideoDeviceHandleType VideoWindow::GeVideoDeviceHandleTypeEx(pjmedia_vid_dev_hwnd_type videoDeviceHandleType)
{
	switch (videoDeviceHandleType)
	{
	case PJMEDIA_VID_DEV_HWND_TYPE_NONE:
		return VideoDeviceHandleType::PJMEDIA_VID_DEV_HWND_TYPE_NONE;
	case PJMEDIA_VID_DEV_HWND_TYPE_WINDOWS:
		return VideoDeviceHandleType::PJMEDIA_VID_DEV_HWND_TYPE_WINDOWS;
	case PJMEDIA_VID_DEV_HWND_TYPE_IOS:
		return VideoDeviceHandleType::PJMEDIA_VID_DEV_HWND_TYPE_IOS;
	case PJMEDIA_VID_DEV_HWND_TYPE_ANDROID:
		return VideoDeviceHandleType::PJMEDIA_VID_DEV_HWND_TYPE_ANDROID;
	default:
		return VideoDeviceHandleType::PJMEDIA_VID_DEV_HWND_TYPE_NONE;
	}
}

///	<summary>
///	Get the VideoDeviceHandleType.
///	</summary>
/// <param name="videoDeviceHandleType">The VideoDeviceHandleType.</param>
/// <returns>The VideoDeviceHandleType.</returns>
pjmedia_vid_dev_hwnd_type VideoWindow::GeVideoDeviceHandleType(VideoDeviceHandleType videoDeviceHandleType)
{
	switch (videoDeviceHandleType)
	{
	case Nequeo::Net::PjSip::VideoDeviceHandleType::PJMEDIA_VID_DEV_HWND_TYPE_NONE:
		return pjmedia_vid_dev_hwnd_type::PJMEDIA_VID_DEV_HWND_TYPE_NONE;
	case Nequeo::Net::PjSip::VideoDeviceHandleType::PJMEDIA_VID_DEV_HWND_TYPE_WINDOWS:
		return pjmedia_vid_dev_hwnd_type::PJMEDIA_VID_DEV_HWND_TYPE_WINDOWS;
	case Nequeo::Net::PjSip::VideoDeviceHandleType::PJMEDIA_VID_DEV_HWND_TYPE_IOS:
		return pjmedia_vid_dev_hwnd_type::PJMEDIA_VID_DEV_HWND_TYPE_IOS;
	case Nequeo::Net::PjSip::VideoDeviceHandleType::PJMEDIA_VID_DEV_HWND_TYPE_ANDROID:
		return pjmedia_vid_dev_hwnd_type::PJMEDIA_VID_DEV_HWND_TYPE_ANDROID;
	default:
		return pjmedia_vid_dev_hwnd_type::PJMEDIA_VID_DEV_HWND_TYPE_NONE;
	}
}