/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoPreviewForm.cpp
*  Purpose :       SIP VideoPreviewForm class.
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

#include "VideoPreviewForm.h"
#include "VideoPreviewOpParam.h"
#include "VideoWindowHandle.h"
#include "MediaFormat.h"
#include "MediaCoordinate.h"

using namespace Nequeo::Net::PjSip::UI;

/// <summary>
/// Load form.
/// </summary>
/// <param name="sender">The object sender.</param>
/// <param name="e">The event.</param>
System::Void VideoPreviewForm::VideoPreviewForm_Load(System::Object^  sender, System::EventArgs^  e)
{
	panel1->Width = (int)_window->GetInfo()->WindowSize->Width;
	panel1->Height = (int)_window->GetInfo()->WindowSize->Height;

	this->Width = panel1->Width + 43;
	this->Height = panel1->Height + 60;

	try
	{
		// Show the preview.
		_window->Show(true);
	}
	catch (const std::exception&) {}
}

/// <summary>
/// Closing form.
/// </summary>
/// <param name="sender">The object sender.</param>
/// <param name="e">The event.</param>
System::Void VideoPreviewForm::VideoPreviewForm_FormClosing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e)
{
	try
	{
		_window->Show(false);
		_videoPreview->Stop();
	}
	catch (const std::exception&) {}

	// Send the form closing event.
	OnVideoPreviewClosing(this, gcnew EventArgs());
}

/// <summary>
/// Moving form.
/// </summary>
/// <param name="sender">The object sender.</param>
/// <param name="e">The event.</param>
System::Void VideoPreviewForm::VideoPreviewForm_Move(System::Object^  sender, System::EventArgs^  e)
{
	int top = this->Top + 43;
	int left = this->Left + 20;

	try
	{
		MediaCoordinate^ mediaCor = gcnew MediaCoordinate();
		mediaCor->X = left;
		mediaCor->Y = top;

		// Set the new position.
		_window->SetPosition(mediaCor);
	}
	catch (const std::exception&) {}
}

/// <summary>
/// Create the preview.
/// </summary>
void VideoPreviewForm::Create()
{
	try
	{
		// Create.
		_videoPreview = gcnew VideoPreview(_videoCaptureIndex);

		// Configure.
		VideoPreviewOpParam^ parm = gcnew VideoPreviewOpParam();

		VideoWindowHandle^ handle = gcnew VideoWindowHandle();
		handle->Type = VideoDeviceHandleType::PJMEDIA_VID_DEV_HWND_TYPE_WINDOWS;

		MediaFormat^ format = gcnew MediaFormat();
		format->Type = MediaType::PJMEDIA_TYPE_VIDEO;

		// Assign.
		parm->Show = false;
		parm->Format = format;
		parm->WindowFlags = 0;
		parm->RenderID = _videoRenderIndex;
		parm->Window = handle;

		// Show.
		_videoPreview->Start(parm);
		_window = _videoPreview->GetVideoWindow();
	}
	catch (const std::exception&) {}
}