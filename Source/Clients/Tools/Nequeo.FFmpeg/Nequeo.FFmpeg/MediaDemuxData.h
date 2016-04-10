/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaDemuxData.h
*  Purpose :       MediaDemuxData class.
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

#ifndef _MEDIADEMUXDATA_H
#define _MEDIADEMUXDATA_H

#include "stdafx.h"

#include "MediaDemuxDataNative.h"

using namespace System;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;

namespace Nequeo
{
	namespace Media
	{
		namespace FFmpeg
		{
			/// <summary>
			/// A structure to encapsulate all FFMPEG related private variable.
			/// </summary>
			public ref struct MediaDemuxData
			{
			public:
				/// <summary>
				/// A structure to encapsulate all FFMPEG related private variable.
				/// </summary>
				MediaDemuxData()
				{
					FormatContext = NULL;
					AudioStream = NULL;
					VideoStream = NULL;
					AudioCodecContext = NULL;
					VideoCodecContext = NULL;
					AudioFrame = NULL;
					VideoFrame = NULL;
					Packet = NULL;
					NativeData = NULL;
					VideoConvertContext = NULL;

					Video_Stream_Index = -1;
					Audio_Stream_Index = -1;

					FrameType = -1;
				}

				MediaDemuxDataNative* NativeData;

				libffmpeg::AVFrame* AudioFrame;
				libffmpeg::AVFrame* VideoFrame;
				libffmpeg::AVFormatContext* FormatContext;
				libffmpeg::AVStream* AudioStream;
				libffmpeg::AVStream* VideoStream;
				libffmpeg::AVCodecContext* AudioCodecContext;
				libffmpeg::AVCodecContext* VideoCodecContext;
				libffmpeg::AVPacket* Packet;
				struct libffmpeg::SwsContext* VideoConvertContext;

				int Video_Stream_Index;
				int Audio_Stream_Index;
				int Video_Buffer_Size;
				int Audio_Buffer_Size;

				int FrameType;
			};
		}
	}
}
#endif