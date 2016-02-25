/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoPreview.cpp
*  Purpose :       SIP VideoPreview class.
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

#include "VideoPreview.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Video preview.
/// </summary>
/// <param name="captureDeviceID">The video capture device id.</param>
VideoPreview::VideoPreview(int captureDeviceID) :
	_pjVideoPreview(new pj::VideoPreview(captureDeviceID)), _disposed(false)
{
}

///	<summary>
///	Video preview deconstructor.
///	</summary>
VideoPreview::~VideoPreview()
{
	if (!_disposed)
	{
		// Cleanup the native classes.
		this->!VideoPreview();

		_disposed = true;
	}
}

///	<summary>
///	Video preview.
///	</summary>
VideoPreview::!VideoPreview()
{
	if (!_disposed)
	{
		// If the callback has been created.
		if (_pjVideoPreview != nullptr)
		{
			// Cleanup the native classes.
			delete _pjVideoPreview;
			_pjVideoPreview = nullptr;
		}
	}
}

/// <summary>
/// Determine if the specified video input device has built-in native
/// preview capability.This is a convenience function that is equal to
/// querying device's capability for PJMEDIA_VID_DEV_CAP_INPUT_PREVIEW
/// capability.
/// </summary>
/// <returns>True if it has.</returns>
bool VideoPreview::HasNative()
{
	return _pjVideoPreview->hasNative();
}

/// <summary>
/// Start video preview window for the specified capture device.
/// </summary>
/// <param name="param">Video preview parameters.</param>
void VideoPreview::Start(VideoPreviewOpParam^ param)
{
	pj::VideoPreviewOpParam prm;

	// If not null.
	if (param != nullptr)
	{
		prm.rendId = param->RenderID;
		prm.show = param->Show;
		prm.windowFlags = param->WindowFlags;

		if (param->Format != nullptr)
		{
			prm.format.type = MediaFormat::GetMediaType(param->Format->Type);
			prm.format.id = param->Format->Id;
		}

		if (param->Window != nullptr)
		{
			prm.window.type = VideoWindow::GeVideoDeviceHandleType(param->Window->Type);
		}
	}
	else
	{
		prm.rendId = pjmedia_vid_dev_std_index::PJMEDIA_VID_DEFAULT_RENDER_DEV;
		prm.show = true;
		prm.windowFlags = pjmedia_vid_dev_wnd_flag::PJMEDIA_VID_DEV_WND_BORDER | pjmedia_vid_dev_wnd_flag::PJMEDIA_VID_DEV_WND_RESIZABLE;
	}

	try
	{
		// Start the preview.
		_pjVideoPreview->start(prm);
	}
	catch (const pj::Error& pjError)
	{
		std::string reason = pjError.reason;
		String^ err = gcnew String(reason.c_str());
		throw gcnew System::Exception(err);
	}
	catch (const std::exception& ex) 
	{
		std::string reason = ex.what();
		String^ err = gcnew String(reason.c_str());
		throw gcnew System::Exception(err);
	}
}

/// <summary>
/// Stop video preview.
/// </summary>
void VideoPreview::Stop()
{
	_pjVideoPreview->stop();
}

/// <summary>
/// Get the preview window handle associated with the capture device, if any.
/// </summary>
/// <returns>Video window.</returns>
VideoWindow^ VideoPreview::GetVideoWindow()
{
	pj::VideoWindow pjWindow = _pjVideoPreview->getVideoWindow();
	VideoWindow^ window = gcnew VideoWindow(pjWindow);
	window->SetVideoPreviewRef(_pjVideoPreview);

	// Return the video window.
	return window;
}