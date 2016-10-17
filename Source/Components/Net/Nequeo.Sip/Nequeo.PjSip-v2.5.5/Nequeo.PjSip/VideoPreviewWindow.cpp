/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoPreviewWindow.cpp
*  Purpose :       SIP VideoPreviewWindow class.
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

#include "VideoPreviewWindow.h"

#include <pjsua-lib/pjsua.h>
#include <pjsua-lib/pjsua_internal.h>

using namespace Nequeo::Net::PjSip::UI;

using namespace System::Runtime::InteropServices;

/// <summary>
/// Load form.
/// </summary>
/// <param name="sender">The object sender.</param>
/// <param name="e">The event.</param>
System::Void VideoPreviewWindow::VideoPreviewWindow_Load(System::Object^  sender, System::EventArgs^  e)
{
	try
	{
		// Create the preview.
		Create();
	}
	catch (const std::exception&) {}
}

/// <summary>
/// Closing form.
/// </summary>
/// <param name="sender">The object sender.</param>
/// <param name="e">The event.</param>
System::Void VideoPreviewWindow::VideoPreviewWindow_FormClosing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e)
{
	try
	{
		// Stop the catpure.
		pjsua_vid_preview_stop(_videoCaptureIndex);
	}
	catch (const std::exception&) {}

	// Send the form closing event.
	OnVideoPreviewClosing(this, gcnew EventArgs());
}

/// <summary>
/// Create the preview.
/// </summary>
void VideoPreviewWindow::Create()
{
	pjsua_vid_win_info wi;

	// Get the window id for the capture device.
	pjsua_vid_win_id wid = pjsua_vid_preview_get_win(_videoCaptureIndex);

	pj_status_t status;
	pjsua_vid_preview_param pre_param;

	// Set the postion.
	const pjmedia_coord pos = { 0, 0 };

	// Get the default preview parameters.
	pjsua_vid_preview_param_default(&pre_param);
	pre_param.show = PJ_FALSE;
	pre_param.rend_id = _videoRenderIndex;

	// Start the capture.
	status = pjsua_vid_preview_start(_videoCaptureIndex, &pre_param);

	// If all is not good.
	if (status != PJ_SUCCESS)
	{
		return;
	}

	// Get the preview window.
	wid = pjsua_vid_preview_get_win(_videoCaptureIndex);

	// Set the position.
	pjsua_vid_win_set_pos(wid, &pos);

	// Show the window.
	pjsua_vid_win_set_show(wid, PJ_TRUE);

	// Set the preview size.
	/*const pjmedia_rect_size size = { 320, 240 };
	pjsua_vid_win_set_size(wid, &size);*/

	// Get the window info.
	pjsua_vid_win_get_info(wid, &wi);

	// Set the window size.
	pjmedia_rect_size videoSize = wi.size;
	this->Width = videoSize.w + 16;
	this->Height = videoSize.h + 38;

	// Set the video catpure handle to this form.
	HWND videoHandle = (HWND)(wi.hwnd.info.win.hwnd);
	HWND parentHandle = (HWND)(this->Handle.ToPointer());

	// Assign the video handle to the parent.
	::SetParent(videoHandle, parentHandle);
}