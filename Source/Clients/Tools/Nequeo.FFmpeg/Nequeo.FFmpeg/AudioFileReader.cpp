/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AudioFileReader.cpp
*  Purpose :       AudioFileReader class.
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

#include "stdafx.h"

#include "AudioFileReader.h"

using namespace Nequeo::Media::FFmpeg;

#pragma managed(push, off)
/// <summary>
/// Open video file with the specified name.
/// </summary>
/// <param name="fileName">Video file name to open.</param>
static libffmpeg::AVFormatContext* open_file(char* fileName)
{
	// Allocate the context.
	libffmpeg::AVFormatContext* formatContext = libffmpeg::avformat_alloc_context();

	// Open the file.
	if (libffmpeg::avformat_open_input(&formatContext, fileName, NULL, 0) != 0)
	{
		return NULL;
	}
	return formatContext;
}
#pragma managed(pop)

/// <summary>
/// Audio file reader.
/// </summary>
AudioFileReader::AudioFileReader() :
	_data(nullptr), _disposed(false)
{
	libffmpeg::av_register_all();
}

/// <summary>
/// Disposes the object and frees its resources.
/// </summary>
AudioFileReader::~AudioFileReader()
{
	// If not disposed.
	if (!_disposed)
	{
		this->!AudioFileReader();
		_disposed = true;
	}
}

/// <summary>
/// Object's finalizer.
/// </summary>
AudioFileReader::!AudioFileReader()
{
	// If not disposed.
	if (!_disposed)
	{
		// Release all resources.
		Close();
	}
}

/// <summary>
/// Audio file reader.
/// </summary>
/// <param name="fileName">Audio file name to open.</param>
WaveStructure AudioFileReader::Open(String^ fileName)
{
	// convert specified managed String to UTF8 unmanaged string
	IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalUni(fileName);
	wchar_t* nativeFileNameUnicode = (wchar_t*)ptr.ToPointer();
	int utf8StringSize = WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, NULL, 0, NULL, NULL);
	char* nativeFileName = new char[utf8StringSize];
	WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, nativeFileName, utf8StringSize, NULL, NULL);

	WaveStructure wave;
	bool success = false;

	try
	{
		// Create the reader data packet.
		_data = gcnew ReaderAudioPrivateData();
		_data->Packet = new libffmpeg::AVPacket();
		_data->Packet->data = NULL;

		// open the specified audio file
		_data->FormatContext = open_file(nativeFileName);
		if (_data->FormatContext == NULL)
		{
			throw gcnew System::IO::IOException("Cannot open the audio file.");
		}

		// retrieve stream information
		if (libffmpeg::avformat_find_stream_info(_data->FormatContext, NULL) < 0)
		{
			throw gcnew AudioException("Cannot find stream information.");
		}

		// search for the first audio stream
		for (unsigned int i = 0; i < _data->FormatContext->nb_streams; i++)
		{
			if (_data->FormatContext->streams[i]->codec->codec_type == libffmpeg::AVMEDIA_TYPE_AUDIO)
			{
				// get the pointer to the codec context for the audio stream
				_data->CodecContext = _data->FormatContext->streams[i]->codec;
				_data->AudioStream = _data->FormatContext->streams[i];
				break;
			}
		}
		if (_data->AudioStream == NULL)
		{
			throw gcnew AudioException("Cannot find audio stream in the specified file.");
		}

		// find decoder for the audio stream
		libffmpeg::AVCodec* codec = libffmpeg::avcodec_find_decoder(_data->CodecContext->codec_id);
		if (codec == NULL)
		{
			throw gcnew AudioException("Cannot find codec to decode the audio stream.");
		}

		// open the codec.
		if (libffmpeg::avcodec_open2(_data->CodecContext, codec, NULL) < 0)
		{
			throw gcnew AudioException("Cannot open audio codec.");
		}

		// allocate audio frame.
		_data->AudioFrame = libffmpeg::av_frame_alloc();
		if (_data->AudioFrame == NULL)
		{
			throw gcnew AudioException("Cannot initialize audio frames.");
		}

		// get some properties of the audio file
		wave.BitsPerSample = _data->CodecContext->bits_per_coded_sample;
		wave.Channels = _data->CodecContext->channels;
		wave.SampleRate = _data->CodecContext->sample_rate;
		wave.FmtAverageByteRate = (_data->CodecContext->sample_rate * _data->CodecContext->channels) * (wave.BitsPerSample / 8);
		wave.FmtBlockAlign = (short)_data->CodecContext->block_align;

		// All is good.
		success = true;
	}
	finally
	{
		System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr);
		delete[] nativeFileName;

		if (!success)
		{
			// Close the stream.
			Close();
		}
	}

	// Return wave file details.
	return wave;
}

/// <summary>
/// Read next audio frame of the currently opened audio file.
/// </summary>
/// <returns>The array of audio frame data.</returns>
array<unsigned char>^ AudioFileReader::ReadAudioFrame()
{
	// Make sure data exists.
	if (_data == nullptr)
	{
		throw gcnew System::IO::IOException("Cannot read audio frames since audio file is not open.");
	}

	int frameFinished;
	array<unsigned char>^ sound = nullptr;

	int bytesDecoded;
	bool exit = false;

	// keep reading frames.
	while (true)
	{
		// work on the current packet until we have decoded all of it
		while (_data->BytesRemaining > 0)
		{
			// decode the next chunk of data
			bytesDecoded = libffmpeg::avcodec_decode_audio4(_data->CodecContext, _data->AudioFrame, &frameFinished, _data->Packet);

			// was there an error?
			if (bytesDecoded < 0)
			{
				throw gcnew AudioException("Error while decoding frame.");
			}

			// Count down bytes remaing.
			_data->BytesRemaining -= bytesDecoded;

			// did we finish the current frame? Then we can return
			if (frameFinished)
			{
				// Return the frame.
				return DecodeAudioFrame(bytesDecoded);
			}
		}

		// read the next packet, skipping all packets that aren't
		// for this stream
		do
		{
			// free old packet if any
			if (_data->Packet->data != NULL)
			{
				libffmpeg::av_free_packet(_data->Packet);
				_data->Packet->data = NULL;
			}

			// read new packet
			if (libffmpeg::av_read_frame(_data->FormatContext, _data->Packet) < 0)
			{
				exit = true;
				break;
			}
		} while (_data->Packet->stream_index != _data->AudioStream->index);

		// exit ?
		if (exit)
			break;

		// Get number of pagket remaining.
		_data->BytesRemaining = _data->Packet->size;
	}

	// decode the rest of the last frame
	bytesDecoded = libffmpeg::avcodec_decode_audio4(
		_data->CodecContext, _data->AudioFrame, &frameFinished, _data->Packet);

	// free last packet
	if (_data->Packet->data != NULL)
	{
		libffmpeg::av_free_packet(_data->Packet);
		_data->Packet->data = NULL;
	}

	// is there a frame
	if (frameFinished)
	{
		// Decode the frame into a sound.
		sound = DecodeAudioFrame(bytesDecoded);
	}

	// Return the frame sound.
	return sound;
}

/// <summary>
/// Close currently opened audio file if any.
/// </summary>
void AudioFileReader::Close()
{
	// If data is not null.
	if (_data != nullptr)
	{
		if (_data->AudioFrame != NULL)
		{
			// Free audio frame.
			libffmpeg::av_free(_data->AudioFrame);
		}

		if (_data->AudioStream)
		{
			// Close the audio codec.
			libffmpeg::avcodec_close(_data->AudioStream->codec);
		}

		if (_data->CodecContext != NULL)
		{
			// Close the codec context.
			libffmpeg::avcodec_close(_data->CodecContext);
		}

		if (_data->FormatContext != NULL)
		{
			// Close the format context.
			libffmpeg::AVFormatContext* avFormatContext = _data->FormatContext;
			libffmpeg::avformat_close_input(&avFormatContext);
		}

		if (_data->Packet->data != NULL)
		{
			// Free the packet.
			libffmpeg::av_free_packet(_data->Packet);
		}

		_data = nullptr;
	}
}

/// <summary>
/// Decode next audio frame of the currently opened audio file.
/// </summary>
/// <param name="bytesDecoded">The number of bytes decoded.</param>
/// <returns>The array of audio frame data.</returns>
array<unsigned char>^ AudioFileReader::DecodeAudioFrame(int bytesDecoded)
{
	array<unsigned char>^ sound = nullptr;

	// if a frame has been decoded, output it
	int data_size = libffmpeg::av_get_bytes_per_sample(_data->CodecContext->sample_fmt);
	if (data_size < 0)
	{
		return nullptr;
	}
	else
	{
		// Create a new sound data collection.
		List<unsigned char>^ soundData = gcnew List<unsigned char>();

		// For the number of samples per channel.
		for (int i = 0; i < _data->AudioFrame->nb_samples; i++)
		{
			// For the number of channels.
			for (int ch = 0; ch < _data->CodecContext->channels; ch++)
			{
				// If channel exists.
				if (_data->AudioFrame->data[ch])
				{
					// Get the point to this set of data.
					uint8_t *sample = _data->AudioFrame->data[ch] + (data_size * i);

					// Write the data size
					for (int j = 0; j < data_size; j++)
					{
						// Write the sound data.
						soundData->Add((unsigned char)(sample[j]));
					}
				}
			}
		}

		// Set the sound.
		sound = soundData->ToArray();
	}

	// Return the frame sound.
	return sound;
}