/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallMediaInfo.h
*  Purpose :       SIP CallMediaInfo class.
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

#ifndef _CALLMEDIAINFO_H
#define _CALLMEDIAINFO_H

#include "stdafx.h"

#include "MediaType.h"
#include "MediaDirection.h"
#include "CallMediaStatus.h"

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
			///	Call media information.
			///	</summary>
			public ref class CallMediaInfo sealed
			{
			public:
				///	<summary>
				///	Call media information.
				///	</summary>
				CallMediaInfo();

				///	<summary>
				///	Call media information.
				///	</summary>
				~CallMediaInfo();

				///	<summary>
				///	Gets or sets the media index in SDP.
				///	</summary>
				property unsigned Index
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the media type.
				///	</summary>
				property MediaType Type
				{
					MediaType get();
					void set(MediaType value);
				}

				///	<summary>
				///	Gets or sets the media direction.
				///	</summary>
				property MediaDirection Direction
				{
					MediaDirection get();
					void set(MediaDirection value);
				}

				///	<summary>
				///	Gets or sets the call media status.
				///	</summary>
				property CallMediaStatus Status
				{
					CallMediaStatus get();
					void set(CallMediaStatus value);
				}

				///	<summary>
				///	Gets or sets the conference port number for the call. Only valid if the media type is audio.
				///	</summary>
				property int AudioConfSlot
				{
					int get();
					void set(int value);
				}

				///	<summary>
				///	Gets or sets the window id for incoming video, if any, or
				/// PJSUA_INVALID_ID. Only valid if the media type is video.
				///	</summary>
				property int VideoIncomingWindowId
				{
					int get();
					void set(int value);
				}

				///	<summary>
				///	Gets or sets the video capture device for outgoing transmission, if any,
				/// or PJMEDIA_VID_INVALID_DEV.Only valid if the media type is video.
				///	</summary>
				property int VideoCapDev
				{
					int get();
					void set(int value);
				}

				/**
				* The video window instance for incoming video. Only valid if
				* videoIncomingWindowId is not PJSUA_INVALID_ID and
				* the media type is video.
				*/
				/// TODO : Implement video window latter.
				///VideoWindow	videoWindow;

			private:
				bool _disposed;

				unsigned _index;
				MediaType _type;
				MediaDirection _direction;
				CallMediaStatus _status;
				int _audioConfSlot;
				int _videoIncomingWindowId;
				int _videoCapDev;
			};
		}
	}
}
#endif