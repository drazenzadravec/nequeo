/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AudioSamplerData.h
*  Purpose :       AudioSamplerData class.
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

#ifndef _AUDIOSAMPLERDATA_H
#define _AUDIOSAMPLERDATA_H

#include "stdafx.h"

#include "AudioSamplerDataNative.h"

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
			public ref struct AudioSamplerData
			{
			public:
				/// <summary>
				/// A structure to encapsulate all FFMPEG related private variable.
				/// </summary>
				AudioSamplerData(int sourceSampleRate, int destinationSampleRate, int sourceNumberSamples)
				{
					Native = NULL;

					SourceData = NULL;
					DestinationData = NULL;

					src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_DBL;
					dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_DBL;

					src_ch_layout = AV_CH_LAYOUT_STEREO;
					dst_ch_layout = AV_CH_LAYOUT_STEREO;

					src_rate = sourceSampleRate;
					dst_rate = destinationSampleRate;

					src_nb_channels = 0;
					dst_nb_channels = 0;

					src_nb_samples = sourceNumberSamples;
				}

				libffmpeg::AudioSamplerDataNative* Native;

				uint8_t **SourceData;
				uint8_t **DestinationData;

				libffmpeg::AVSampleFormat src_sample_fmt;
				libffmpeg::AVSampleFormat dst_sample_fmt;

				int64_t src_ch_layout;
				int64_t dst_ch_layout;

				int src_rate;
				int dst_rate;

				int src_nb_channels;
				int dst_nb_channels;

				int src_linesize;
				int dst_linesize;

				int src_nb_samples;
				int dst_nb_samples;
				int max_dst_nb_samples;
			};
		}
	}
}
#endif