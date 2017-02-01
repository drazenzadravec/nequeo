/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CodecType.h
*  Purpose :       CodecType class.
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

#ifndef _CODECTYPE_H
#define _CODECTYPE_H

#include "MediaGlobal.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Media decoder type.
			/// </summary>
			enum class DecoderType
			{
				/// <summary>
				/// H.264 Video Decoder.
				/// </summary>
				H264 = 0,
				/// <summary>
				/// H.265 HEVC Video Decoder.
				/// </summary>
				H265 = 1,
				/// <summary>
				/// Advanced Audio Coding Decoder.
				/// </summary>
				AAC = 2,
				/// <summary>
				/// MPEG-1 layer 3 Audio Decoder.
				/// </summary>
				MP3 = 3,
				/// <summary>
				/// MPEG-4 Part 2 Video Decoder.
				/// </summary>
				MPEG4 = 4,
			};

			/// <summary>
			/// Media encoder type.
			/// </summary>
			enum class EncoderType
			{
				/// <summary>
				/// H.264 Video Encoder.
				/// </summary>
				H264 = 0,
				/// <summary>
				/// H.265 HEVC Video Encoder.
				/// </summary>
				H265 = 1,
				/// <summary>
				/// Advanced Audio Coding Encoder.
				/// </summary>
				AAC = 2,
				/// <summary>
				/// MPEG-1 layer 3 Audio Encoder.
				/// </summary>
				MP3 = 3,
			};
		}
	}
}
#endif