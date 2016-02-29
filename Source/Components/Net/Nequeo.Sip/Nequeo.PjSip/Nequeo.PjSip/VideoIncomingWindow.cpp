/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoIncomingWindow.cpp
*  Purpose :       SIP VideoIncomingWindow class.
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

#include "VideoIncomingWindow.h"

#include <pjsua-lib/pjsua.h>
#include <pjsua-lib/pjsua_internal.h>

using namespace Nequeo::Net::PjSip::UI;

using namespace System::Runtime::InteropServices;

/// <summary>
/// Load form.
/// </summary>
/// <param name="sender">The object sender.</param>
/// <param name="e">The event.</param>
System::Void VideoIncomingWindow::VideoIncomingWindow_Load(System::Object^  sender, System::EventArgs^  e)
{
	try
	{
		// If video window id is not valid.
		if (_videoWindowID < 0)
		{
			throw gcnew Exception("The video window ID is not valid. The video window ID must be greater than or equal to zero.");
		}

		// Create the incoming.
		Create();
	}
	catch (const std::exception&) {}
}

/// <summary>
/// Closing form.
/// </summary>
/// <param name="sender">The object sender.</param>
/// <param name="e">The event.</param>
System::Void VideoIncomingWindow::VideoIncomingWindow_FormClosing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e)
{
	// Send the form closing event.
	OnVideoIncomingClosing(this, gcnew EventArgs());
}

/// <summary>
/// Show the video window.
/// </summary>
void VideoIncomingWindow::ShowVideoWindow()
{
	// Show the window.
	pjsua_vid_win_set_show(_videoWindowID, PJ_TRUE);
}

/// <summary>
/// Hide the video window.
/// </summary>
void VideoIncomingWindow::HideVideoWindow()
{
	// Hide the window.
	pjsua_vid_win_set_show(_videoWindowID, PJ_FALSE);
}

/// <summary>
/// Create the icoming video.
/// </summary>
void VideoIncomingWindow::Create()
{
	pjsua_vid_win_info wi;

	// Set the postion.
	const pjmedia_coord pos = { 0, 0 };

	// Get the info.
	pjsua_vid_win_get_info(_videoWindowID, &wi);

	// Set the position.
	pjsua_vid_win_set_pos(_videoWindowID, &pos);

	// Show the window.
	pjsua_vid_win_set_show(_videoWindowID, PJ_TRUE);

	// Set the preview size.
	/*const pjmedia_rect_size size = { 320, 240 };
	pjsua_vid_win_set_size(wid, &size);*/

	// Set the window size.
	pjmedia_rect_size videoSize = wi.size;
	this->Width = videoSize.w;
	this->Height = videoSize.h;

	// Set the video catpure handle to this form.
	HWND videoHandle = (HWND)(wi.hwnd.info.win.hwnd);
	HWND parentHandle = (HWND)(this->Handle.ToPointer());

	// Assign the video handle to the parent.
	::SetParent(videoHandle, parentHandle);
}