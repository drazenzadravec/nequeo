/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoPreviewOpParam.h
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

#pragma once

#ifndef _VIDEOPREVIEWOPPARAM_H
#define _VIDEOPREVIEWOPPARAM_H

#include "stdafx.h"

#include "MediaFormat.h"
#include "VideoWindowHandle.h"

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
			///	<summary>
			///	This structure contains parameters for VideoPreview::start()
			///	</summary>
			public ref class VideoPreviewOpParam sealed
			{
			public:
				///	<summary>
				///	This structure contains parameters for VideoPreview::start()
				///	</summary>
				VideoPreviewOpParam();

				/// <summary>
				/// Gets or sets the Device ID for the video renderer to be used for rendering the
				/// capture stream for preview.This parameter is ignored if native
				/// preview is being used.
				/// </summary>
				property int RenderID
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets the show window initially.
				/// </summary>
				property bool Show
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets the Window flags. The value is a bitmask combination of
				/// \a pjmedia_vid_dev_wnd_flag.
				/// </summary>
				property unsigned WindowFlags
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets the media format. If left unitialized, this parameter will not be used.
				/// </summary>
				property MediaFormat^ Format
				{
					MediaFormat^ get();
					void set(MediaFormat^ value);
				}

				/// <summary>
				/// Gets or sets the Optional output window to be used to display the video preview.
				/// This parameter will only be used if the video device supports
				/// PJMEDIA_VID_DEV_CAP_OUTPUT_WINDOW capability and the capability
				/// is not read - only.
				/// </summary>
				property VideoWindowHandle^ Window
				{
					VideoWindowHandle^ get();
					void set(VideoWindowHandle^ value);
				}

			private:
				int _renderID;
				bool _show;
				unsigned _windowFlags;
				MediaFormat^ _format;
				VideoWindowHandle^ _window;
			};
		}
	}
}
#endif