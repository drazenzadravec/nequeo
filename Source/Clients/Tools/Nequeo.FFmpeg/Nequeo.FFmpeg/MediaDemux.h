/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaDemux.h
*  Purpose :       MediaDemux class.
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

#ifndef _MEDIADEMUX_H
#define _MEDIADEMUX_H

#include "stdafx.h"

#include "MediaDemuxData.h"
#include "MediaDemuxException.h"
#include "SampleFormat.h"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;

namespace Nequeo
{
	namespace Media
	{
		namespace FFmpeg
		{
			/// <summary>
			/// Audio video demultiplexer and decoder.
			/// </summary>
			public ref class MediaDemux
			{
			public:
				/// <summary>
				/// Audio video demultiplexer and decoder.
				/// </summary>
				MediaDemux();

				/// <summary>
				/// Disposes the object and frees its resources.
				/// </summary>
				~MediaDemux();

				/// <summary>
				/// Object's finalizer.
				/// </summary>
				!MediaDemux();

				/// <summary>
				/// Open audio video file with the specified name.
				/// </summary>
				/// <param name="fileName">Audio video file name to open.</param>
				void Open(String^ fileName);

				/// <summary>
				/// Read the next frame of the opened file.
				/// </summary>
				/// <param name="audio">Audio frame data.</param>
				/// <param name="video">Video frame data.</param>
				/// <returns>The number of bytes read.</returns>
				int ReadFrame(
					[System::Runtime::InteropServices::OutAttribute] array<unsigned char>^% audio,
					[System::Runtime::InteropServices::OutAttribute] array<Bitmap^>^% video);

				/// <summary>
				/// Close currently opened files if any.
				/// </summary>
				void Close();

				/// <summary>
				/// Gets the frame width of the opened video file.
				/// </summary>
				property int Width
				{
					int get()
					{
						return _width;
					}
				}

				/// <summary>
				/// Gets the frame height of the opened video file.
				/// </summary>
				property int Height
				{
					int get()
					{
						return _height;
					}
				}

				/// <summary>
				/// Gets the frame rate of the opened video file.
				/// </summary>
				property int FrameRate
				{
					int get()
					{
						return _frameRate;
					}
				}

				/// <summary>
				/// Gets the number of audio channels in each frame to be written.
				/// </summary>
				property int Channels
				{
					int get()
					{
						return _channels;
					}
					void set(int value)
					{
						_channels = value;
					}
				}

				/// <summary>
				/// Gets the audio sample rate.
				/// </summary>
				property int SampleRate
				{
					int get()
					{
						return _sampleRate;
					}
				}

				/// <summary>
				/// Gets the number of audio bytes per sample in each channel.
				/// </summary>
				property int BytesPerSample
				{
					int get()
					{
						return _bitsPerSample;
					}
					void set(int value)
					{
						_bitsPerSample = value;
					}
				}

				/// <summary>
				/// Gets format average byte rate. Number of bytes per second that are used for all
				/// data. This value equals SampleRate * Channels * (BitsPerSample/8)
				/// </summary>
				property int AverageByteRate
				{
					int get()
					{
						return _averageByteRate;
					}
				}

				/// <summary>
				/// Gets the number of audio samples (per channel) described by this frame.
				/// </summary>
				property int NumberSamples
				{
					int get()
					{
						return _numberSamples;
					}
					void set(int value)
					{
						_numberSamples = value;
					}
				}

				/// <summary>
				/// Gets the sample format.
				/// </summary>
				property SampleFormat Sample
				{
					SampleFormat get()
					{
						return _sampleFormat;
					}
				}

			private:
				bool _disposed;
				int _frameType;
				MediaDemuxData^ _data;

				int _width;
				int _height;
				int _frameRate;
				int _numberSamples;
				libffmpeg::AVPixelFormat _pix_fmt;

				int _bitsPerSample;
				int _channels;
				int _sampleRate;
				int _averageByteRate;
				int _blockAlign;
				SampleFormat _sampleFormat;
			};
		}
	}
}
#endif