/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaMux.h
*  Purpose :       MediaMux class.
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

#ifndef _MEDIAMUX_H
#define _MEDIAMUX_H

#include "stdafx.h"

#include "VideoCodec.h"
#include "AudioCodec.h"
#include "MediaMuxException.h"
#include "MediaMuxData.h"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;

using namespace Nequeo::IO::Audio;

namespace Nequeo
{
	namespace Media
	{
		namespace FFmpeg
		{
			/// <summary>
			/// Audio video multiplexer and encoder.
			/// </summary>
			public ref class MediaMux
			{
			public:
				/// <summary>
				/// Audio video multiplexer and encoder.
				/// </summary>
				MediaMux();

				/// <summary>
				/// Disposes the object and frees its resources.
				/// </summary>
				~MediaMux();

				/// <summary>
				/// Object's finalizer.
				/// </summary>
				!MediaMux();

				/// <summary>
				/// Open audio video file with the specified name.
				/// </summary>
				/// <param name="fileName">Audio video file name to open.</param>
				/// <param name="videoWidth">Frame width of the video file.</param>
				/// <param name="videoHeight">Frame height of the video file.</param>
				/// <param name="audioHeader">The audio wave frame header.</param>
				void Open(String^ fileName, int videoWidth, int videoHeight, WaveStructure audioHeader);

				/// <summary>
				/// Open audio video file with the specified name.
				/// </summary>
				/// <param name="fileName">Audio video file name to open.</param>
				/// <param name="videoWidth">Frame width of the video file.</param>
				/// <param name="videoHeight">Frame height of the video file.</param>
				/// <param name="videoFrameRate">Frame rate of the video file.</param>
				/// <param name="audioHeader">The audio wave frame header.</param>
				void Open(String^ fileName, int videoWidth, int videoHeight, int videoFrameRate, WaveStructure audioHeader);

				/// <summary>
				/// Open audio video file with the specified name.
				/// </summary>
				/// <param name="fileName">Audio video file name to open.</param>
				/// <param name="videoWidth">Frame width of the video file.</param>
				/// <param name="videoHeight">Frame height of the video file.</param>
				/// <param name="videoFrameRate">Frame rate of the video file.</param>
				/// <param name="audioHeader">The audio wave frame header.</param>
				/// <param name="videoBitRate">Bit rate of the video stream.</param>
				void Open(String^ fileName, int videoWidth, int videoHeight, int videoFrameRate, WaveStructure audioHeader, int videoBitRate);

				/// <summary>
				/// Video frame to writer.
				/// </summary>
				/// <param name="frame">The video bitmap frame to write.</param>
				void WriteVideoFrame(Bitmap^ frame);

				/// <summary>
				/// Audio frame to writer.
				/// </summary>
				/// <param name="frame">The audio wave frame to write.</param>
				void WriteAudioFrame(array<unsigned char>^ frame);

				/// <summary>
				/// Close currently opened files if any.
				/// </summary>
				void Close();

				/// <summary>
				/// Frame width of the opened video file.
				/// </summary>
				property int Width
				{
					int get()
					{
						return _width;
					}
				}

				/// <summary>
				/// Frame height of the opened video file.
				/// </summary>
				property int Height
				{
					int get()
					{
						return _height;
					}
				}

				/// <summary>
				/// Frame rate of the opened video file.
				/// </summary>
				property int FrameRate
				{
					int get()
					{
						return _frameRate;
					}
				}

				/// <summary>
				/// Bit rate of the video stream.
				/// </summary>
				property int BitRate
				{
					int get()
					{
						return _bitRate;
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
				/// Gets the number of audio channels in each frame to be written.
				/// </summary>
				property int Channels
				{
					int get()
					{
						return _channels;
					}
				}

				/// <summary>
				/// Gets the number of audio bytes per sample in each channel.
				/// </summary>
				property int BytesPerSample
				{
					int get()
					{
						return _bytesPerSample;
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
				}

			private:
				bool _disposed;
				MediaMuxData^ _data;

				int _width;
				int _height;
				int	_frameRate;
				int _bitRate;

				int _channels;
				int _bytesPerSample;
				int _numberSamples;
				int _sampleRate;
				
			};
		}
	}
}
#endif