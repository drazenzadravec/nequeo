/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AudioFileWriter.h
*  Purpose :       AudioFileWriter class.
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

#ifndef _AUDIOFILEWRITER_H
#define _AUDIOFILEWRITER_H

#include "stdafx.h"

#include "WriterAudioPrivateData.h"
#include "AudioException.h"
#include "AudioCodec.h"

using namespace System;
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
			/// Audio file writer.
			/// </summary>
			public ref class AudioFileWriter
			{
			public:
				/// <summary>
				/// Audio file writer.
				/// </summary>
				AudioFileWriter();

				/// <summary>
				/// Disposes the object and frees its resources.
				/// </summary>
				~AudioFileWriter();

				/// <summary>
				/// Object's finalizer.
				/// </summary>
				!AudioFileWriter();

				/// <summary>
				/// Audio file writer.
				/// </summary>
				/// <param name="fileName">Audio file name to create.</param>
				/// <param name="header">The audio wave frame header.</param>
				void Open(String^ fileName, WaveStructure header);

				/// <summary>
				/// Audio file writer.
				/// </summary>
				/// <param name="fileName">Audio file name to create.</param>
				/// <param name="header">The audio wave frame header.</param>
				/// <param name="codec">Audio codec to use for compression.</param>
				void Open(String^ fileName, WaveStructure header, AudioCodec codec);

				/// <summary>
				/// Audio frame to writer.
				/// </summary>
				/// <param name="frame">The audio wave frame to write.</param>
				void WriteAudioFrame(array<unsigned char>^ frame);

				/// <summary>
				/// Audio frame to writer.
				/// </summary>
				/// <param name="frame">The audio wave frame to write.</param>
				/// <param name="timestamp">Frame timestamp, total time since recording started.</param>
				/// <remarks><note>The <paramref name="timestamp"/> parameter allows user to specify presentation
				/// time of the frame being saved. However, it is user's responsibility to make sure the value is increasing
				/// over time.</note></para>
				/// </remarks>
				void WriteAudioFrame(array<unsigned char>^ frame, TimeSpan timestamp);

				/// <summary>
				/// Audio frame to writer.
				/// </summary>
				/// <param name="frame">The audio wave frame to write.</param>
				/// <param name="position">The audio frame position.</param>
				void WriteAudioFrame(array<unsigned char>^ frame, signed long long position);

				/// <summary>
				/// Close currently opened audio file if any.
				/// </summary>
				void Close();

				/// <summary>
				/// Gets the number of channels in each frame to be written.
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
				/// Gets the number of bytes per sample in each channel.
				/// </summary>
				property int BytesPerSample
				{
					int get()
					{
						return _bytesPerSample;
					}
					void set(int value)
					{
						_bytesPerSample = value;
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

			private:
				bool _disposed;
				int _channels;
				int _bytesPerSample;
				int _numberSamples;

				WriterAudioPrivateData^ _data;

				/// <summary>
				/// Audio frame to encoder.
				/// </summary>
				/// <param name="frame">The audio wave frame to write.</param>
				void EncodeAudioFrame(array<unsigned char>^ frame);
			};
		}
	}
}
#endif