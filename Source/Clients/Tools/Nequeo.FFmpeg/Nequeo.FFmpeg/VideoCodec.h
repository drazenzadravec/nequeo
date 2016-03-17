/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          VideoCodec.h
*  Purpose :       SIP VideoCodec class.
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

#ifndef _VIDEOCODEC_H
#define _VIDEOCODEC_H

#include "stdafx.h"

using namespace System;

extern int video_codecs[];
extern int pixel_formats[];

extern int CODECS_COUNT;

namespace Nequeo
{
	namespace Media
	{
		namespace FFmpeg
		{
			/// <summary>
			/// Enumeration of some video codecs from FFmpeg library, which are available for writing video files.
			/// </summary>
			public enum class VideoCodec
			{
				/// <summary>
				/// Default video codec, which FFmpeg library selects for the specified file format.
				/// </summary>
				Default = -1,
				/// <summary>
				/// MPEG-4 part 2.
				/// </summary>
				MPEG4 = 0,
				/// <summary>
				/// Windows Media Video 7.
				/// </summary>
				WMV1,
				/// <summary>
				/// Windows Media Video 8.
				/// </summary>
				WMV2,
				/// <summary>
				/// MPEG-4 part 2 Microsoft variant version 2.
				/// </summary>
				MSMPEG4v2,
				/// <summary>
				/// MPEG-4 part 2 Microsoft variant version 3.
				/// </summary>
				MSMPEG4v3,
				/// <summary>
				/// H.263+ / H.263-1998 / H.263 version 2.
				/// </summary>
				H263P,
				/// <summary>
				/// Flash Video (FLV) / Sorenson Spark / Sorenson H.263.
				/// </summary>
				FLV1,
				/// <summary>
				/// MPEG-2 part 2.
				/// </summary>
				MPEG2,
				/// <summary>
				/// Raw (uncompressed) video.
				/// </summary>
				Raw,
				/// <summary>
				/// H.264.
				/// </summary>
				H264,
				/// <summary>
				/// MP4
				/// </summary>
				MP4,
			};
		}
	}
}
#endif