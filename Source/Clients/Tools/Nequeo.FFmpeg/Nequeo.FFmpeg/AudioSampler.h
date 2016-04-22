/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AudioSampler.h
*  Purpose :       AudioSampler class.
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

#ifndef _AUDIOSAMPLER_H
#define _AUDIOSAMPLER_H

#include "stdafx.h"

#include "AudioSamplerData.h"
#include "SampleFormat.h"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

using namespace Nequeo::IO::Audio;

namespace Nequeo
{
	namespace Media
	{
		namespace FFmpeg
		{
			/// <summary>
			/// Audio sampler.
			/// </summary
			public ref class AudioSampler
			{
			public:
				/// <summary>
				/// Audio sampler.
				/// </summary>
				AudioSampler();

				/// <summary>
				/// Disposes the object and frees its resources.
				/// </summary>
				~AudioSampler();

				/// <summary>
				/// Object's finalizer.
				/// </summary>
				!AudioSampler();

				/// <summary>
				/// Open the re-sampler.
				/// </summary>
				/// <param name="sourceHeader">The audio source wave frame header.</param>
				/// <param name="sourceFormat">The audio source sample format.</param>
				/// <param name="sourceNumberSamples">The audio source number of samples per channel.</param>
				/// <param name="destinationHeader">The audio destination wave frame header.</param>
				/// <param name="destinationFormat">The audio destination sample format.</param>
				/// <param name="destinationNumberSamples">The audio destination number of samples per channel.</param>
				void Open(WaveStructure sourceHeader, SampleFormat sourceFormat, int sourceNumberSamples,
					WaveStructure destinationHeader, SampleFormat destinationFormat, [System::Runtime::InteropServices::OutAttribute] int% destinationNumberSamples);

				/// <summary>
				/// Re-sample the audio frame.
				/// </summary>
				/// <param name="frame">The audio source frame.</param>
				/// <returns>The re-sampled audio frame.</returns>
				array<unsigned char>^ Resample(array<unsigned char>^ frame);

				/// <summary>
				/// Close currently opened audio file if any.
				/// </summary>
				void Close();

			private:
				bool _disposed;
				AudioSamplerData^ _data;

				int _channels;
				int _bytesPerSample;
				int _numberSamples;
			};
		}
	}
}
#endif