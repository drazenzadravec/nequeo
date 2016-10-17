/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          VideoWindowInfo.h
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

#pragma once

#ifndef _VIDEOWINDOWINFO_H
#define _VIDEOWINDOWINFO_H

#include "stdafx.h"

#include "VideoWindowHandle.h"
#include "MediaCoordinate.h"
#include "MediaSize.h"

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
			/// This structure describes video window info.
			/// </summary>
			public ref class VideoWindowInfo sealed
			{
			public:
				/// <summary>
				/// This structure describes video window info.
				/// </summary>
				VideoWindowInfo();

				///	<summary>
				///	This structure describes video window info.
				///	</summary>
				~VideoWindowInfo();

				/// <summary>
				/// Gets or sets the flag to indicate whether this window is a native window,
				/// such as created by built - in preview device.If this field is
				/// true, only the video window handle field of this
				/// structure is valid.
				/// </summary>
				property bool IsNative
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets the video window handle.
				/// </summary>
				property VideoWindowHandle^ WindowHandle
				{
					VideoWindowHandle^ get();
					void set(VideoWindowHandle^ value);
				}

				/// <summary>
				/// Gets or sets the renderer device ID.
				/// </summary>
				property int RenderDeviceId
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets the window show status. The window is hidden if false.
				/// </summary>
				property bool Show
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets the window position.
				/// </summary>
				property MediaCoordinate^ WindowPosition
				{
					MediaCoordinate^ get();
					void set(MediaCoordinate^ value);
				}

				/// <summary>
				/// Gets or sets the window size.
				/// </summary>
				property MediaSize^ WindowSize
				{
					MediaSize^ get();
					void set(MediaSize^ value);
				}

			private:
				bool _disposed;
				bool _isNative;
				VideoWindowHandle^ _windowHandle;
				int _renderDeviceId;
				bool _show;
				MediaCoordinate^ _windowPosition;
				MediaSize^ _windowSize;
			};
		}
	}
}
#endif