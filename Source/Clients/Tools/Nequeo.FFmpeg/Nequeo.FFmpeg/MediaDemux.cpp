/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaDemux.cpp
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

#include "stdafx.h"

#include "MediaDemux.h"

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

/// <summary>
/// Open device with the specified name.
/// </summary>
/// <param name="deviceName">Capture device name to open.</param>
static libffmpeg::AVFormatContext* open_device(char* deviceName)
{
	// Allocate the context.
	libffmpeg::AVFormatContext* formatContext = libffmpeg::avformat_alloc_context();

	// Input capture format (direct show).
	libffmpeg::AVInputFormat* inputFormat = libffmpeg::av_find_input_format("dshow");

	// Open the file.
	if (libffmpeg::avformat_open_input(&formatContext, deviceName, inputFormat, NULL) != 0)
	{
		return NULL;
	}
	return formatContext;
}
#pragma managed(pop)

/// <summary>
/// Open the codec context.
/// </summary>
/// <param name="data">Media demux data.</param>
/// <param name="type">Media type.</param>
static int open_codec_context(MediaDemuxData^ data, enum libffmpeg::AVMediaType type);

/// <summary>
/// Decode the packet.
/// </summary>
/// <param name="data">Media demux data.</param>
/// <param name="got_frame">Has frame.</param>
/// <param name="cached">The cache.</param>
/// <param name="audio">The audio data.</param>
/// <param name="video">The video data.</param>
/// <param name="numberSamples">The number of samples per channel.</param>
/// <param name="bytesPerSample">The bytes per sample.</param>
/// <param name="audioCount">The number of audio bytes read.</param>
/// <param name="videoCount">The number of video bit maps.</param>
/// <returns>Data read.</returns>
static int decode_packet(MediaDemuxData^ data, int *got_frame, int cached, List<unsigned char>^ audio, List<Bitmap^>^ video,
	int *numberSamples, int bytesPerSample, int *audioCount, int *videoCount);

/// <summary>
/// Decode the packet.
/// </summary>
/// <param name="fmt">The format name.</param>
/// <param name="sample_fmt">The sample format.</param>
static int get_format_from_sample_fmt(const char **fmt, enum libffmpeg::AVSampleFormat sample_fmt);

/// <summary>
/// Audio video demultiplexer and decoder.
/// </summary>
MediaDemux::MediaDemux() :
	_data(nullptr), _disposed(false), _frameType(-1), _captureVideo(false), _captureAudio(false)
{
	libffmpeg::av_register_all();
}

/// <summary>
/// Disposes the object and frees its resources.
/// </summary>
MediaDemux::~MediaDemux()
{
	// If not disposed.
	if (!_disposed)
	{
		this->!MediaDemux();
		_disposed = true;
	}
}

/// <summary>
/// Object's finalizer.
/// </summary>
MediaDemux::!MediaDemux()
{
	// If not disposed.
	if (!_disposed)
	{
		// Release all resources.
		Close();
	}
}

/// <summary>
/// Open audio video device with the specified name.
/// </summary>
/// <param name="captureDeviceName">Audio video device name to open (video=[video device]:audio=[audio device]).</param>
/// <param name="captureVideo">True if capturing video.</param>
/// <param name="captureAudio">True if capturing audio.</param>
void MediaDemux::OpenDevice(String^ captureDeviceName, bool captureVideo, bool captureAudio)
{
	// convert specified managed String to UTF8 unmanaged string
	IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalUni(captureDeviceName);
	wchar_t* nativeFileNameUnicode = (wchar_t*)ptr.ToPointer();
	int utf8StringSize = WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, NULL, 0, NULL, NULL);
	char* nativeFileName = new char[utf8StringSize];
	WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, nativeFileName, utf8StringSize, NULL, NULL);

	// Register the device.
	libffmpeg::avdevice_register_all();

	int ret = 0;
	bool success = false;
	_captureVideo = false;

	try
	{
		// Create the reader data packet.
		_data = gcnew MediaDemuxData();
		_data->NativeData = new MediaDemuxDataNative();
		_data->Packet = new libffmpeg::AVPacket();

		// open the specified device.
		_data->FormatContext = open_device(nativeFileName);
		if (_data->FormatContext == NULL)
		{
			throw gcnew MediaDemuxException("Cannot open the file.");
		}

		// retrieve stream information
		if (libffmpeg::avformat_find_stream_info(_data->FormatContext, NULL) < 0)
		{
			throw gcnew MediaDemuxException("Cannot find stream information.");
		}

		// If capturing video.
		if (captureVideo)
		{
			// Open the video codec context.
			if (open_codec_context(_data, libffmpeg::AVMEDIA_TYPE_VIDEO) >= 0)
			{
				// Assign video stream.
				_data->VideoStream = _data->FormatContext->streams[_data->Video_Stream_Index];
				_data->VideoCodecContext = _data->VideoStream->codec;

				// Get video details.
				_width = _data->VideoCodecContext->width;
				_height = _data->VideoCodecContext->height;
				_pix_fmt = _data->VideoCodecContext->pix_fmt;
				_frameRate = _data->VideoStream->r_frame_rate.num / _data->VideoStream->r_frame_rate.den;

				// prepare scaling context to convert RGB image to video format
				_data->VideoConvertContext = libffmpeg::sws_getContext(_data->VideoCodecContext->width, _data->VideoCodecContext->height, _data->VideoCodecContext->pix_fmt,
					_data->VideoCodecContext->width, _data->VideoCodecContext->height, libffmpeg::AV_PIX_FMT_BGR24, SWS_BICUBIC, NULL, NULL, NULL);

				if (_data->VideoConvertContext == NULL)
				{
					throw gcnew MediaDemuxException("Cannot initialize frames conversion context.");
				}

				// Allocate image buffer.
				ret = libffmpeg::av_image_alloc(_data->NativeData->VideoData, _data->NativeData->VideoLineSize, _width, _height, _pix_fmt, 1);

				// Could not allocate video buffer.
				if (ret < 0)
				{
					throw gcnew MediaDemuxException("Could not allocate raw video buffer.");
				}

				// Allocated video data.
				_captureVideo = true;

				// Set the video buffer size.
				_data->Video_Buffer_Size = ret;

				// Allocate the video frame.
				_data->VideoFrame = libffmpeg::av_frame_alloc();
				if (!_data->VideoFrame)
				{
					throw gcnew MediaDemuxException("Could not allocate frame.");
				}
			}
		}

		// If capturing audio.
		if (captureAudio)
		{
			// Open the audio codec context.
			if (open_codec_context(_data, libffmpeg::AVMEDIA_TYPE_AUDIO) >= 0)
			{
				// Assign audio stream.
				_data->AudioStream = _data->FormatContext->streams[_data->Audio_Stream_Index];
				_data->AudioCodecContext = _data->AudioStream->codec;

				// get some properties of the audio file
				_bytesPerSample = libffmpeg::av_get_bytes_per_sample(_data->AudioCodecContext->sample_fmt);
				_bitsPerSample = libffmpeg::av_get_bytes_per_sample(_data->AudioCodecContext->sample_fmt) * 8;
				_channels = _data->AudioCodecContext->channels;
				_sampleRate = _data->AudioCodecContext->sample_rate;
				_averageByteRate = (_data->AudioCodecContext->sample_rate * _data->AudioCodecContext->channels) * (_bitsPerSample / 8);
				_blockAlign = (short)_data->AudioCodecContext->block_align;

				// Allocate the audio frame.
				_data->AudioFrame = libffmpeg::av_frame_alloc();
				if (!_data->AudioFrame)
				{
					throw gcnew MediaDemuxException("Could not allocate frame.");
				}

				// number of samples.
				_numberSamples = _data->AudioFrame->nb_samples;
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_U8;

				// Select the sample format.
				switch (_data->AudioCodecContext->sample_fmt)
				{
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_NONE:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_NONE;
					break;
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_U8:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_U8;
					break;
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_S16:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_S16;
					break;
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_S32:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_S32;
					break;
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_FLT:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_FLT;
					break;
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_DBL:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_DBL;
					break;
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_U8P:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_U8P;
					break;
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_S16P:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_S16P;
					break;
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_S32P:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_S32P;
					break;
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_FLTP:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_FLTP;
					break;
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_DBLP:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_DBLP;
					break;
				case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_NB:
					_sampleFormat = SampleFormat::AV_SAMPLE_FMT_NB;
					break;
				default:
					break;
				}
			}
		}

		// if no audo or video exists in the file.
		if (!_data->AudioStream && !_data->VideoStream)
		{
			throw gcnew MediaDemuxException("Could not find audio or video stream in the input.");
		}

		// Initialize packet, set data to NULL, let the demuxer fill it.
		libffmpeg::av_init_packet(_data->Packet);
		_data->Packet->data = NULL;
		_data->Packet->size = 0;

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
}

/// <summary>
/// Open audio video file with the specified name.
/// </summary>
/// <param name="fileName">Audio video file name to open.</param>
void MediaDemux::Open(String^ fileName)
{
	// convert specified managed String to UTF8 unmanaged string
	IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalUni(fileName);
	wchar_t* nativeFileNameUnicode = (wchar_t*)ptr.ToPointer();
	int utf8StringSize = WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, NULL, 0, NULL, NULL);
	char* nativeFileName = new char[utf8StringSize];
	WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, nativeFileName, utf8StringSize, NULL, NULL);

	int ret = 0;
	bool success = false;
	_captureVideo = false;

	try
	{
		// Create the reader data packet.
		_data = gcnew MediaDemuxData();
		_data->NativeData = new MediaDemuxDataNative();
		_data->Packet = new libffmpeg::AVPacket();

		// open the specified audio file
		_data->FormatContext = open_file(nativeFileName);
		if (_data->FormatContext == NULL)
		{
			throw gcnew MediaDemuxException("Cannot open the file.");
		}

		// retrieve stream information
		if (libffmpeg::avformat_find_stream_info(_data->FormatContext, NULL) < 0)
		{
			throw gcnew MediaDemuxException("Cannot find stream information.");
		}

		// Open the video codec context.
		if (open_codec_context(_data, libffmpeg::AVMEDIA_TYPE_VIDEO) >= 0)
		{
			// Assign video stream.
			_data->VideoStream = _data->FormatContext->streams[_data->Video_Stream_Index];
			_data->VideoCodecContext = _data->VideoStream->codec;

			// Get video details.
			_width = _data->VideoCodecContext->width;
			_height = _data->VideoCodecContext->height;
			_pix_fmt = _data->VideoCodecContext->pix_fmt;
			_frameRate = _data->VideoStream->r_frame_rate.num / _data->VideoStream->r_frame_rate.den;

			// prepare scaling context to convert RGB image to video format
			_data->VideoConvertContext = libffmpeg::sws_getContext(_data->VideoCodecContext->width, _data->VideoCodecContext->height, _data->VideoCodecContext->pix_fmt,
				_data->VideoCodecContext->width, _data->VideoCodecContext->height, libffmpeg::AV_PIX_FMT_BGR24, SWS_BICUBIC, NULL, NULL, NULL);

			if (_data->VideoConvertContext == NULL)
			{
				throw gcnew MediaDemuxException("Cannot initialize frames conversion context.");
			}

			// Allocate image buffer.
			ret = libffmpeg::av_image_alloc(_data->NativeData->VideoData, _data->NativeData->VideoLineSize, _width, _height, _pix_fmt, 1);

			// Could not allocate video buffer.
			if (ret < 0) 
			{
				throw gcnew MediaDemuxException("Could not allocate raw video buffer.");
			}

			// Allocated video data.
			_captureVideo = true;

			// Set the video buffer size.
			_data->Video_Buffer_Size = ret;

			// Allocate the video frame.
			_data->VideoFrame = libffmpeg::av_frame_alloc();
			if (!_data->VideoFrame)
			{
				throw gcnew MediaDemuxException("Could not allocate frame.");
			}
		}

		// Open the audio codec context.
		if (open_codec_context(_data, libffmpeg::AVMEDIA_TYPE_AUDIO) >= 0)
		{
			// Assign audio stream.
			_data->AudioStream = _data->FormatContext->streams[_data->Audio_Stream_Index];
			_data->AudioCodecContext = _data->AudioStream->codec;

			// get some properties of the audio file
			_bytesPerSample = libffmpeg::av_get_bytes_per_sample(_data->AudioCodecContext->sample_fmt);
			_bitsPerSample = libffmpeg::av_get_bytes_per_sample(_data->AudioCodecContext->sample_fmt) * 8;
			_channels = _data->AudioCodecContext->channels;
			_sampleRate = _data->AudioCodecContext->sample_rate;
			_averageByteRate = (_data->AudioCodecContext->sample_rate * _data->AudioCodecContext->channels) * (_bitsPerSample / 8);
			_blockAlign = (short)_data->AudioCodecContext->block_align;

			// Allocate the audio frame.
			_data->AudioFrame = libffmpeg::av_frame_alloc();
			if (!_data->AudioFrame)
			{
				throw gcnew MediaDemuxException("Could not allocate frame.");
			}

			// number of samples.
			_numberSamples = _data->AudioFrame->nb_samples;
			_sampleFormat = SampleFormat::AV_SAMPLE_FMT_U8;

			// Select the sample format.
			switch (_data->AudioCodecContext->sample_fmt)
			{
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_NONE:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_NONE;
				break;
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_U8:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_U8;
				break;
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_S16:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_S16;
				break;
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_S32:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_S32;
				break;
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_FLT:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_FLT;
				break;
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_DBL:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_DBL;
				break;
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_U8P:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_U8P;
				break;
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_S16P:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_S16P;
				break;
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_S32P:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_S32P;
				break;
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_FLTP:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_FLTP;
				break;
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_DBLP:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_DBLP;
				break;
			case libffmpeg::AVSampleFormat::AV_SAMPLE_FMT_NB:
				_sampleFormat = SampleFormat::AV_SAMPLE_FMT_NB;
				break;
			default:
				break;
			}
		}

		// if no audo or video exists in the file.
		if (!_data->AudioStream && !_data->VideoStream)
		{
			throw gcnew MediaDemuxException("Could not find audio or video stream in the input.");
		}

		// Initialize packet, set data to NULL, let the demuxer fill it.
		libffmpeg::av_init_packet(_data->Packet);
		_data->Packet->data = NULL;
		_data->Packet->size = 0;

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
}

/// <summary>
/// Read the next frame of the opened file.
/// </summary>
/// <param name="audio">Audio frame data.</param>
/// <param name="video">Video frame data.</param>
/// <returns>The number of bytes read.</returns>
int MediaDemux::ReadFrame(
	[System::Runtime::InteropServices::OutAttribute] array<unsigned char>^% audio,
	[System::Runtime::InteropServices::OutAttribute] array<Bitmap^>^% video)
{
	int ret = 0;
	int got_frame;
	int numberSamples;
	int audioCount = 0;
	int videoCount = 0;
	libffmpeg::AVPacket* packet = _data->Packet;

	// Read some frame data.
	if (libffmpeg::av_read_frame(_data->FormatContext, packet) >= 0)
	{
		// Set up the packet data store.
		List<unsigned char>^ audioData = gcnew List<unsigned char>();
		List<Bitmap^>^ videoData = gcnew List<Bitmap^>();

		// Get the original packet.
		libffmpeg::AVPacket orig_pkt = *packet;

		do 
		{
			// Decode the packet data.
			ret = decode_packet(_data, &got_frame, 0, audioData, videoData, &numberSamples, _bytesPerSample, &audioCount, &videoCount);
			_numberSamples = numberSamples;

			if (ret < 0)
				break;

			packet->data += ret;
			packet->size -= ret;

		} while (packet->size > 0);

		// Unref the current packet.
		libffmpeg::av_packet_unref(&orig_pkt);

		// If audio exists.
		if (audioCount > 0)
		{
			// Create audio container.
			audio = gcnew array<unsigned char>(audioCount);

			// Copy the data.
			for (int i = 0; i < audioCount; i++)
			{
				// Assign.
				audio[i] = audioData[i];
			}

			// Clear the data store.
			audioData = nullptr;
		}
		else
		{
			// No audio.
			audio = nullptr;
			audioData = nullptr;
		}

		// If video exists.
		if (videoCount > 0)
		{
			// Create video container.
			video = gcnew array<Bitmap^>(videoCount);

			// Copy the data.
			for (int i = 0; i < videoCount; i++)
			{
				// Assign.
				video[i] = videoData[i];
			}

			// Clear the data store.
			videoData = nullptr;
		}
		else
		{
			// No video.
			video = nullptr;
			videoData = nullptr;
		}
	}

	// Return the result.
	return ret;
}

/// <summary>
/// Close currently opened files if any.
/// </summary>
void MediaDemux::Close()
{
	// If data is not null.
	if (_data != nullptr)
	{
		if (_data->AudioFrame != NULL)
		{
			// Free audio frame.
			libffmpeg::av_free(_data->AudioFrame);
		}

		if (_data->VideoFrame != NULL)
		{
			// Free video frame.
			libffmpeg::av_free(_data->VideoFrame);
		}

		if (_data->AudioStream)
		{
			// Close the audio codec.
			libffmpeg::avcodec_close(_data->AudioStream->codec);
		}

		if (_data->VideoStream)
		{
			// Close the video codec.
			libffmpeg::avcodec_close(_data->VideoStream->codec);
		}

		if (_data->AudioCodecContext != NULL)
		{
			// Close the codec context.
			libffmpeg::avcodec_close(_data->AudioCodecContext);
		}

		if (_data->VideoCodecContext != NULL)
		{
			// Close the codec context.
			libffmpeg::avcodec_close(_data->VideoCodecContext);
		}

		if (_data->FormatContext != NULL)
		{
			// Close the format context.
			libffmpeg::AVFormatContext* avFormatContext = _data->FormatContext;
			libffmpeg::avformat_close_input(&avFormatContext);
		}

		if (_data->VideoConvertContext != NULL)
		{
			// Free the convert context.
			libffmpeg::sws_freeContext(_data->VideoConvertContext);
		}

		if (_data->Packet != NULL)
		{
			// Free the packet.
			delete _data->Packet;
		}

		if (_data->NativeData != NULL)
		{
			// Has video data been allocated.
			if (_captureVideo)
			{
				if (_data->NativeData->VideoData != NULL)
				{
					// Free the video data.
					libffmpeg::av_free(_data->NativeData->VideoData[0]);
				}
			}

			// Delete the native data.
			delete _data->NativeData;
		}

		_data = nullptr;
	}
}

/// <summary>
/// Open the codec context.
/// </summary>
/// <param name="data">Media demux data.</param>
/// <param name="type">Media type.</param>
int open_codec_context(MediaDemuxData^ data, enum libffmpeg::AVMediaType type)
{
	int ret, stream_index;
	libffmpeg::AVStream *st;
	libffmpeg::AVCodecContext *dec_ctx = NULL;
	libffmpeg::AVCodec *dec = NULL;
	libffmpeg::AVDictionary *opts = NULL;

	// Find the best stream.
	ret = libffmpeg::av_find_best_stream(data->FormatContext, type, -1, -1, NULL, 0);

	// No stream found.
	if (ret < 0) 
	{
		throw gcnew MediaDemuxException("Could not find a stream in input file.");
	}
	else 
	{
		stream_index = ret;
		st = data->FormatContext->streams[stream_index];

		/* find decoder for the stream */
		dec_ctx = st->codec;
		dec = libffmpeg::avcodec_find_decoder(dec_ctx->codec_id);
		if (!dec) 
		{
			throw gcnew MediaDemuxException("Failed to find codec.");
		}

		/* Init the decoders, with or without reference counting */
		libffmpeg::av_dict_set(&opts, "refcounted_frames", 1 ? "1" : "0", 0);
		if ((ret = libffmpeg::avcodec_open2(dec_ctx, dec, &opts)) < 0)
		{
			throw gcnew MediaDemuxException("Failed to open codec.");
		}

		// Select the stream.
		switch (type)
		{
		case libffmpeg::AVMEDIA_TYPE_UNKNOWN:
			break;
		case libffmpeg::AVMEDIA_TYPE_VIDEO:
			data->Video_Stream_Index = stream_index;
			break;
		case libffmpeg::AVMEDIA_TYPE_AUDIO:
			data->Audio_Stream_Index = stream_index;
			break;
		case libffmpeg::AVMEDIA_TYPE_DATA:
			break;
		case libffmpeg::AVMEDIA_TYPE_SUBTITLE:
			break;
		case libffmpeg::AVMEDIA_TYPE_ATTACHMENT:
			break;
		case libffmpeg::AVMEDIA_TYPE_NB:
			break;
		default:
			break;
		}
	}

	return 0;
}

/// <summary>
/// Decode the packet.
/// </summary>
/// <param name="data">Media demux data.</param>
/// <param name="got_frame">Has frame.</param>
/// <param name="cached">The cache.</param>
/// <param name="audio">The audio data.</param>
/// <param name="video">The video data.</param>
/// <param name="numberSamples">The number of samples per channel.</param>
/// <param name="bytesPerSample">The bytes per sample.</param>
/// <param name="audioCount">The number of audio bytes read.</param>
/// <param name="videoCount">The number of video bit maps.</param>
/// <returns>Data read.</returns>
int decode_packet(MediaDemuxData^ data, int *got_frame, int cached, List<unsigned char>^ audio, List<Bitmap^>^ video,
	int *numberSamples, int bytesPerSample, int *audioCount, int *videoCount)
{
	int ret = 0;

	libffmpeg::AVPacket* packet = data->Packet;
	libffmpeg::AVFrame* audioFrame = data->AudioFrame;
	libffmpeg::AVFrame* videoFrame = data->VideoFrame;

	int decoded = packet->size;

	// Got new frame.
	*got_frame = 0;

	// No frame has been read.
	data->FrameType = -1;

	// If is video packet.
	if (packet->stream_index == data->Video_Stream_Index)
	{
		// decode video frame
		ret = libffmpeg::avcodec_decode_video2(data->VideoCodecContext, videoFrame, got_frame, packet);
		if (ret < 0) 
		{
			throw gcnew MediaDemuxException("Error decoding video frame.");
		}

		// If we have a frame.
		if (*got_frame)
		{
			int videoIndex = 0;

			// If the video is not correct.
			if (videoFrame->width != data->VideoCodecContext->width || videoFrame->height != data->VideoCodecContext->height ||
				videoFrame->format != data->VideoCodecContext->pix_fmt)
			{
				// To handle this change, one could call av_image_alloc again and
				// decode the following frames into another rawvideo file.
				throw gcnew MediaDemuxException(
					"Error: Width, height and pixel format have to be "
					"constant in a raw video file, but the width, height or "
					"pixel format of the input video changed.");
			}

			// Create a new bitmap.
			Bitmap^ bitmap = gcnew Bitmap(data->VideoCodecContext->width, data->VideoCodecContext->height, PixelFormat::Format24bppRgb);

			// lock the bitmap size into memory, allocate space.
			BitmapData^ bitmapData = bitmap->LockBits(System::Drawing::Rectangle(0, 0, data->VideoCodecContext->width, data->VideoCodecContext->height),
				ImageLockMode::ReadOnly, PixelFormat::Format24bppRgb);

			// Get the memory address of the first bitmap pixel
			uint8_t* ptr = reinterpret_cast<uint8_t*>(static_cast<void*>(bitmapData->Scan0));

			// Assign the pointer of the first pixel.
			uint8_t* srcData[4] = { ptr, NULL, NULL, NULL };

			// Get the stride width (scan width).
			int srcLinesize[4] = { bitmapData->Stride, 0, 0, 0 };

			// Convert video frame to the RGB bitmap.
			// Write the video frame image to the bitmap memory location set above.
			libffmpeg::sws_scale(data->VideoConvertContext, data->VideoFrame->data, data->VideoFrame->linesize, 0,
				data->VideoCodecContext->height, srcData, srcLinesize);

			// Unlock the bitmap from memory.
			bitmap->UnlockBits(bitmapData);

			// Add the bitmap.
			video->Add(bitmap);
			videoIndex++;

			// Video bitmaps read.
			*videoCount = videoIndex;

			// Video frame has been read.
			data->FrameType = 1;
		}

		// If we use frame reference counting, we own the data and need
		// to de-reference it when we don't use it anymore.
		if (*got_frame && 1)
			libffmpeg::av_frame_unref(videoFrame);

	}
	// If is audio packet.
	else if (packet->stream_index == data->Audio_Stream_Index)
	{
		// decode audio frame
		ret = libffmpeg::avcodec_decode_audio4(data->AudioCodecContext, audioFrame, got_frame, packet);
		if (ret < 0) 
		{
			throw gcnew MediaDemuxException("Error decoding audio frame.");
		}

		// Some audio decoders decode only part of the packet, and have to be
		// called again with the remainder of the packet data.
		// Sample: fate-suite/lossless-audio/luckynight-partial.shn
		// Also, some decoders might over-read the packet.
		decoded = FFMIN(ret, packet->size);

		// If we have a frame.
		if (*got_frame)
		{
			int data_size = 0;
			int audioIndex = 0;

			if (bytesPerSample > 0)
			{
				// Specific bytes per sample.
				data_size = bytesPerSample;
			}
			else
			{
				// The number of audio bytes read.
				data_size = libffmpeg::av_get_bytes_per_sample(data->AudioCodecContext->sample_fmt);
			}
			
			// Audio frame has been read.
			data->FrameType = 0;

			// Get the number of samples per channel.
			*numberSamples = data->AudioFrame->nb_samples;

			// For the number of samples per channel.
			for (int i = 0; i < data->AudioFrame->nb_samples; i++)
			{
				// For the number of channels.
				for (int ch = 0; ch < data->AudioCodecContext->channels; ch++)
				{
					// If channel exists.
					if (data->AudioFrame->data[ch])
					{
						// Get the point to this set of data.
						uint8_t *sample = data->AudioFrame->data[ch] + (data_size * i);

						// Write the data size
						for (int j = 0; j < data_size; j++)
						{
							// Write the sound data.
							audio->Add((unsigned char)(sample[j]));
							audioIndex++;
						}
					}
				}
			}

			// Audio bytes read.
			*audioCount = audioIndex;
		}

		// If we use frame reference counting, we own the data and need
		// to de-reference it when we don't use it anymore.
		if (*got_frame && 1)
			libffmpeg::av_frame_unref(audioFrame);
	}

	// Bytes decoded.
	return decoded;
}

/// <summary>
/// Decode the packet.
/// </summary>
/// <param name="fmt">The format name.</param>
/// <param name="sample_fmt">The sample format.</param>
int get_format_from_sample_fmt(const char **fmt, enum libffmpeg::AVSampleFormat sample_fmt)
{
	int i;
	struct sample_fmt_entry {
		enum libffmpeg::AVSampleFormat sample_fmt; const char *fmt_be, *fmt_le;
	} sample_fmt_entries[] = {
		{ libffmpeg::AV_SAMPLE_FMT_U8,  "u8",    "u8" },
		{ libffmpeg::AV_SAMPLE_FMT_S16, "s16be", "s16le" },
		{ libffmpeg::AV_SAMPLE_FMT_S32, "s32be", "s32le" },
		{ libffmpeg::AV_SAMPLE_FMT_FLT, "f32be", "f32le" },
		{ libffmpeg::AV_SAMPLE_FMT_DBL, "f64be", "f64le" },
		{ libffmpeg::AV_SAMPLE_FMT_U8P,  "u8",    "u8" },
		{ libffmpeg::AV_SAMPLE_FMT_S16P, "s16be", "s16le" },
		{ libffmpeg::AV_SAMPLE_FMT_S32P, "s32be", "s32le" },
		{ libffmpeg::AV_SAMPLE_FMT_FLTP, "f32be", "f32le" },
		{ libffmpeg::AV_SAMPLE_FMT_DBLP, "f64be", "f64le" },
	};
	*fmt = NULL;

	for (i = 0; i < FF_ARRAY_ELEMS(sample_fmt_entries); i++) 
	{
		struct sample_fmt_entry *entry = &sample_fmt_entries[i];
		if (sample_fmt == entry->sample_fmt) 
		{
			*fmt = AV_NE(entry->fmt_be, entry->fmt_le);
			return 0;
		}
	}

	// Error sample format is not supported.
	return -1;
}