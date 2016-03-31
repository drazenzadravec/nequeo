/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AudioCodec.h
*  Purpose :       AudioCodec class.
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

#ifndef _AUDIOCODEC_H
#define _AUDIOCODEC_H

#include "stdafx.h"

using namespace System;

extern int audio_codecs[];
extern int AUDIO_CODECS_COUNT;

namespace Nequeo
{
	namespace Media
	{
		namespace FFmpeg
		{
			/// <summary>
			/// Enumeration of some audio codecs from FFmpeg library, which are available for writing audio files.
			/// </summary>
			public enum class AudioCodec
			{
				/// <summary>
				/// Default audio codec, which FFmpeg library selects for the specified file format.
				/// </summary>
				Default = -1,
				/// <summary>
				/// MP2
				/// </summary>
				MP2,
				/// <summary>
				/// MP3
				/// </summary>
				MP3,
				/// <summary>
				/// AAC
				/// </summary>
				AAC,
				/// <summary>
				/// WMA
				/// </summary>
				WMA_v1,
				/// <summary>
				/// WMA
				/// </summary>
				WMA_v2,
			};
		}
	}
}
#endif