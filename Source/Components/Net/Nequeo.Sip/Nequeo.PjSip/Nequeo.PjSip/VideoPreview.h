/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoPreview.h
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

#pragma once

#ifndef _VIDEOPREVIEW_H
#define _VIDEOPREVIEW_H

#include "stdafx.h"

#include "VideoWindow.h"
#include "VideoPreviewOpParam.h"
#include "MediaFormat.h"

#include "pjsua2.hpp"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			/// <summary>
			/// Video preview.
			/// </summary>
			public ref class VideoPreview sealed
			{
			public:
				/// <summary>
				/// Video preview.
				/// </summary>
				/// <param name="captureDeviceID">The video capture device id.</param>
				VideoPreview(int captureDeviceID);

				///	<summary>
				///	Video preview. deconstructor.
				///	</summary>
				~VideoPreview();

				///	<summary>
				///	Video preview.
				///	</summary>
				!VideoPreview();

				/// <summary>
				/// Determine if the specified video input device has built-in native
				/// preview capability.This is a convenience function that is equal to
				/// querying device's capability for PJMEDIA_VID_DEV_CAP_INPUT_PREVIEW
				/// capability.
				/// </summary>
				/// <returns>True if it has.</returns>
				bool HasNative();

				/// <summary>
				/// Start video preview window for the specified capture device.
				/// </summary>
				/// <param name="param">Video preview parameters.</param>
				void Start(VideoPreviewOpParam^ param);

				/// <summary>
				/// Stop video preview.
				/// </summary>
				void Stop();

				/// <summary>
				/// Get the preview window handle associated with the capture device, if any.
				/// </summary>
				/// <returns>Video window.</returns>
				VideoWindow^ GetVideoWindow();

			private:
				bool _disposed;

				pj::VideoPreview* _pjVideoPreview;
			};
		}
	}
}
#endif