/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaMux.cpp
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

#include "stdafx.h"

#include "MediaMux.h"

using namespace Nequeo::Media::FFmpeg;

/// <summary>
/// Close the output stream.
/// </summary>
static void close_stream(libffmpeg::AVFormatContext *oc, libffmpeg::OutputStream *ost);

/// <summary>
/// Add an output stream.
/// </summary>
static void add_stream(libffmpeg::OutputStream *ost, libffmpeg::AVFormatContext *oc, libffmpeg::AVCodec **codec, enum libffmpeg::AVCodecID codec_id,
	int videoWidth, int videoHeight, int videoFrameRate, int videoBitRate, int64_t bitRate, int sampleRate, int channels, short bitsPerSample);

/// <summary>
/// Open the video.
/// </summary>
static void open_video(MediaMuxData^ data, libffmpeg::AVFormatContext *oc, libffmpeg::AVCodec *codec, libffmpeg::OutputStream *ost, libffmpeg::AVDictionary *opt_arg);

/// <summary>
/// Open the audio.
/// </summary>
static void open_audio(libffmpeg::AVFormatContext *oc, libffmpeg::AVCodec *codec, libffmpeg::OutputStream *ost, libffmpeg::AVDictionary *opt_arg);

/// <summary>
/// Allocate audio frame.
/// </summary>
static libffmpeg::AVFrame *alloc_audio_frame(enum libffmpeg::AVSampleFormat sample_fmt, uint64_t channel_layout, int sample_rate, int nb_samples);

/// <summary>
/// Allocate video frame.
/// </summary>
static libffmpeg::AVFrame *alloc_picture(enum libffmpeg::AVPixelFormat pix_fmt, int width, int height);

/// <summary>
/// encode one video frame and send it to the muxer
/// return 1 when encoding is finished, 0 otherwise
/// </summary>
static int write_video_frame(MediaMuxData^ data);

/// <summary>
/// Write the audio frame.
/// </summary>
/// <param name="data">Audio write data.</param>
static int write_audio_frame(MediaMuxData^ data);

/// <summary>
/// Audio video multiplexer and encoder.
/// </summary>
MediaMux::MediaMux() :
	_disposed(false), _data(nullptr)
{
	libffmpeg::av_register_all();
}

/// <summary>
/// Disposes the object and frees its resources.
/// </summary>
MediaMux::~MediaMux()
{
	// If not disposed.
	if (!_disposed)
	{
		this->!MediaMux();
		_disposed = true;
	}
}

/// <summary>
/// Object's finalizer.
/// </summary>
MediaMux::!MediaMux()
{
	// If not disposed.
	if (!_disposed)
	{
		// Release all resources.
		Close();
	}
}

/// <summary>
/// Open audio video file with the specified name.
/// </summary>
/// <param name="fileName">Audio video file name to open.</param>
/// <param name="videoWidth">Frame width of the video file.</param>
/// <param name="videoHeight">Frame height of the video file.</param>
/// <param name="audioHeader">The audio wave frame header.</param>
void MediaMux::Open(String^ fileName, int videoWidth, int videoHeight, WaveStructure audioHeader)
{
	Open(fileName, videoWidth, videoHeight, 25, audioHeader);
}

/// <summary>
/// Open audio video file with the specified name.
/// </summary>
/// <param name="fileName">Audio video file name to open.</param>
/// <param name="videoWidth">Frame width of the video file.</param>
/// <param name="videoHeight">Frame height of the video file.</param>
/// <param name="videoFrameRate">Frame rate of the video file.</param>
/// <param name="audioHeader">The audio wave frame header.</param>
void MediaMux::Open(String^ fileName, int videoWidth, int videoHeight, int videoFrameRate, WaveStructure audioHeader)
{
	Open(fileName, videoWidth, videoHeight, videoFrameRate, audioHeader, 400000);
}

/// <summary>
/// Open audio video file with the specified name.
/// </summary>
/// <param name="fileName">Audio video file name to open.</param>
/// <param name="videoWidth">Frame width of the video file.</param>
/// <param name="videoHeight">Frame height of the video file.</param>
/// <param name="videoFrameRate">Frame rate of the video file.</param>
/// <param name="audioHeader">The audio wave frame header.</param>
/// <param name="videoBitRate">Bit rate of the video stream.</param>
void MediaMux::Open(String^ fileName, int videoWidth, int videoHeight, int videoFrameRate, WaveStructure audioHeader, int videoBitRate)
{
	int ret;
	bool success = false;

	// check width and height
	if (((videoWidth & 1) != 0) || ((videoHeight & 1) != 0))
	{
		throw gcnew ArgumentException("Video file resolution must be a multiple of two.");
	}

	// Get the video parameters.
	_width = videoWidth;
	_height = videoHeight;
	_frameRate = videoFrameRate;
	_bitRate = videoBitRate;

	// Get the audio parameters.
	_channels = audioHeader.Channels;
	_sampleRate = audioHeader.SampleRate;
	_bytesPerSample = audioHeader.BitsPerSample / 8;

	// convert specified managed String to unmanaged string
	IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalUni(fileName);
	wchar_t* nativeFileNameUnicode = (wchar_t*)ptr.ToPointer();
	int utf8StringSize = WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, NULL, 0, NULL, NULL);
	char* nativeFileName = new char[utf8StringSize];
	WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, nativeFileName, utf8StringSize, NULL, NULL);

	try
	{
		// Create the reader data packet.
		_data = gcnew MediaMuxData();
		_data->AudioStream = new libffmpeg::OutputStream();
		_data->VideoStream = new libffmpeg::OutputStream();

		/* allocate the output media context */
		libffmpeg::AVFormatContext* avFormatContext = _data->FormatContext;
		libffmpeg::avformat_alloc_output_context2(&avFormatContext, NULL, NULL, nativeFileName);
		_data->FormatContext = avFormatContext;

		if (!_data->FormatContext)
		{
			// Use mpeg as the default.
			libffmpeg::avformat_alloc_output_context2(&avFormatContext, NULL, "mpeg", nativeFileName);
			_data->FormatContext = avFormatContext;
		}

		if (!_data->FormatContext)
			throw gcnew MediaMuxException("Could not deduce output format from file extension.");

		// Set the format type.
		_data->OutputFormat = _data->FormatContext->oformat;

		/* Add the audio and video streams using the default format codecs
		* and initialize the codecs. */
		if (_data->OutputFormat->video_codec != libffmpeg::AV_CODEC_ID_NONE) 
		{
			// Get the video codec.
			libffmpeg::AVCodec* videoCodec = _data->VideoCodec;

			add_stream(_data->VideoStream, _data->FormatContext, &videoCodec, _data->OutputFormat->video_codec, 
				videoWidth, videoHeight, videoFrameRate, videoBitRate, 
				(int64_t)audioHeader.FmtAverageByteRate * 8, audioHeader.SampleRate, (int)audioHeader.Channels, audioHeader.BitsPerSample);

			_data->VideoCodec = videoCodec;
			_data->Have_Video = 1;
			_data->Encode_Video = 1;
		}

		if (_data->OutputFormat->audio_codec != libffmpeg::AV_CODEC_ID_NONE) 
		{
			// Get the video codec.
			libffmpeg::AVCodec* audioCodec = _data->AudioCodec;

			add_stream(_data->AudioStream, _data->FormatContext, &audioCodec, _data->OutputFormat->audio_codec, 
				videoWidth, videoHeight, videoFrameRate, videoBitRate,
				(int64_t)audioHeader.FmtAverageByteRate * 8, audioHeader.SampleRate, (int)audioHeader.Channels, audioHeader.BitsPerSample);

			_data->AudioCodec = audioCodec;
			_data->Have_Audio = 1;
			_data->Encode_Audio = 1;
		}

		/* Now that all the parameters are set, we can open the audio and
		* video codecs and allocate the necessary encode buffers. */
		if (_data->Have_Video)
		{
			// Open the video.
			open_video(_data, _data->FormatContext, _data->VideoCodec, _data->VideoStream, _data->Dictionary);
		}

		// Has audio.
		if (_data->Have_Audio)
		{
			// Open the audio.
			open_audio(_data->FormatContext, _data->AudioCodec, _data->AudioStream, _data->Dictionary);

			// Get the number of samples.
			_numberSamples = _data->AudioStream->frame->nb_samples;
		}

		/* open the output file, if needed */
		if (!(_data->OutputFormat->flags & AVFMT_NOFILE))
		{
			ret = libffmpeg::avio_open(&_data->FormatContext->pb, nativeFileName, AVIO_FLAG_WRITE);
			if (ret < 0) 
			{
				throw gcnew MediaMuxException("Could not open the output file.");
			}
		}

		// Write the header data.
		libffmpeg::AVDictionary* dictionary = _data->Dictionary;
		ret = libffmpeg::avformat_write_header(_data->FormatContext, &dictionary);
		if (ret < 0)
		{
			throw gcnew MediaMuxException("Error occurred when opening output file.");
		}
		else
		{
			// Set the dictionary.
			_data->Dictionary = dictionary;
		}

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
/// Video frame to writer.
/// </summary>
/// <param name="frame">The video bitmap frame to write.</param>
void MediaMux::WriteVideoFrame(Bitmap^ frame)
{
	if (_data == nullptr)
	{
		throw gcnew System::IO::IOException("A video file was not opened yet.");
	}

	// lock the bitmap to allocate memory.
	BitmapData^ bitmapData = frame->LockBits(System::Drawing::Rectangle(0, 0, _width, _height),
		ImageLockMode::ReadOnly, (frame->PixelFormat == PixelFormat::Format8bppIndexed) ? PixelFormat::Format8bppIndexed : PixelFormat::Format24bppRgb);

	// Get the pointer to the first scan pixel
	uint8_t* ptr = reinterpret_cast<uint8_t*>(static_cast<void*>(bitmapData->Scan0));

	// Get the stride (scan) pointer to the first pixel
	uint8_t* srcData[4] = { ptr, NULL, NULL, NULL };
	int srcLinesize[4] = { bitmapData->Stride, 0, 0, 0 };

	// Get the current context.
	libffmpeg::AVCodecContext *c = _data->VideoStream->st->codec;

	// convert source image to the format of the video file, write the image to the bitmap data memory block.
	if (frame->PixelFormat == PixelFormat::Format8bppIndexed)
	{
		// Gray scale.
		libffmpeg::sws_scale(_data->VideoStream->sws_ctx_Gray, srcData, srcLinesize, 0, _height, _data->VideoStream->frame->data, _data->VideoStream->frame->linesize);
	}
	else
	{
		if (frame->PixelFormat == PixelFormat::Format24bppRgb)
		{
			// 24 bit rgb
			libffmpeg::sws_scale(_data->VideoStream->sws_ctx_BGR24, srcData, srcLinesize, 0, _height, _data->VideoStream->frame->data, _data->VideoStream->frame->linesize);
		}
		else
		{
			// 420p YUV.
			libffmpeg::sws_scale(_data->VideoStream->sws_ctx_YUV420P, srcData, srcLinesize, 0, _height, _data->VideoStream->frame->data, _data->VideoStream->frame->linesize);
		}
	}

	// Unlick the memory where the bitmap is stored.
	frame->UnlockBits(bitmapData);

	// Write the video frame.
	_data->Encode_Video = !write_video_frame(_data);
}

/// <summary>
/// Audio frame to writer.
/// </summary>
/// <param name="frame">The audio wave frame to write.</param>
void MediaMux::WriteAudioFrame(array<unsigned char>^ frame)
{
	if (_data == nullptr)
	{
		throw gcnew System::IO::IOException("An audio file was not opened yet.");
	}

	// Contains the samples per channel.
	uint64_t frameByteIndex = 0;

	// For sample.
	for (uint64_t i = 0; i < _numberSamples; i++)
	{
		// For each channel.
		for (int ch = 0; ch < _channels; ch++)
		{
			// Get the pointer to this set of data.
			uint8_t *sample = _data->AudioStream->frame->data[ch] + (_bytesPerSample * i);

			// Write the data size
			for (int j = 0; j < _bytesPerSample; j++)
			{
				// Do not go beyond the last index.
				if (frameByteIndex < frame->Length)
				{
					// Write the sound data.
					sample[j] = (uint8_t)frame[frameByteIndex];
					frameByteIndex++;
				}
			}
		}
	}

	// Write the audio frame.
	_data->Encode_Audio = !write_audio_frame(_data);
}

/// <summary>
/// Close currently opened files if any.
/// </summary>
void MediaMux::Close()
{
	// If data is not null.
	if (_data != nullptr)
	{
		// If context exists.
		if (_data->FormatContext)
		{
			if (_data->FormatContext->pb != NULL)
			{
				// Write last bit of data.
				libffmpeg::av_write_trailer(_data->FormatContext);
			}

			if (_data->VideoStream != NULL)
			{
				/* Close each codec. */
				if (_data->Have_Video)
					close_stream(_data->FormatContext, _data->VideoStream);

				// Delete the native data.
				delete _data->VideoStream;
			}

			if (_data->AudioStream != NULL)
			{
				if (_data->Have_Audio)
					close_stream(_data->FormatContext, _data->AudioStream);

				// Delete the native data.
				delete _data->AudioStream;
			}
		}

		// If context exists.
		if (_data->FormatContext)
		{
			for (unsigned int i = 0; i < _data->FormatContext->nb_streams; i++)
			{
				// Free the stream data, using the stream pointer.
				libffmpeg::av_freep(&_data->FormatContext->streams[i]->codec);
				libffmpeg::av_freep(&_data->FormatContext->streams[i]);
			}

			if (_data->FormatContext->pb != NULL)
			{
				// Close the stream.
				libffmpeg::avio_close(_data->FormatContext->pb);
			}

			// Free the context.
			libffmpeg::av_free(_data->FormatContext);
		}

		_data = nullptr;
	}
}

/// <summary>
/// Close the output stream.
/// </summary>
void close_stream(libffmpeg::AVFormatContext *oc, libffmpeg::OutputStream *ost)
{
	libffmpeg::avcodec_close(ost->st->codec);
	libffmpeg::av_frame_free(&ost->frame);
	libffmpeg::av_frame_free(&ost->tmp_frame);
	libffmpeg::sws_freeContext(ost->sws_ctx_BGR24);
	libffmpeg::sws_freeContext(ost->sws_ctx_YUV420P);
	libffmpeg::sws_freeContext(ost->sws_ctx_Gray);
	libffmpeg::swr_free(&ost->swr_ctx);
}

/// <summary>
/// Add an output stream.
/// </summary>
void add_stream(libffmpeg::OutputStream *ost, libffmpeg::AVFormatContext *oc, libffmpeg::AVCodec **codec, enum libffmpeg::AVCodecID codec_id,
	int videoWidth, int videoHeight, int videoFrameRate, int videoBitRate, int64_t bitRate, int sampleRate, int channels, short bitsPerSample)
{
	libffmpeg::AVCodecContext *c;
	int i;

	/* find the encoder */
	*codec = avcodec_find_encoder(codec_id);
	if (!(*codec)) 
	{
		throw gcnew MediaMuxException("Could not find encoder.");
	}

	ost->st = avformat_new_stream(oc, *codec);
	if (!ost->st) 
	{
		throw gcnew MediaMuxException("Could not allocate stream.");
	}
	ost->st->id = oc->nb_streams - 1;
	c = ost->st->codec;

	switch ((*codec)->type) 
	{
	case libffmpeg::AVMEDIA_TYPE_AUDIO:
		c->sample_fmt = (*codec)->sample_fmts ? (*codec)->sample_fmts[0] : libffmpeg::AV_SAMPLE_FMT_FLTP;
		c->bit_rate = bitRate;
		c->sample_rate = sampleRate;
		if ((*codec)->supported_samplerates) 
		{
			c->sample_rate = (*codec)->supported_samplerates[0];
			for (i = 0; (*codec)->supported_samplerates[i]; i++) 
			{
				if ((*codec)->supported_samplerates[i] == sampleRate)
					c->sample_rate = sampleRate;
			}
		}
		c->channels = libffmpeg::av_get_channel_layout_nb_channels(c->channel_layout);
		c->channel_layout = AV_CH_LAYOUT_STEREO;
		if ((*codec)->channel_layouts) 
		{
			c->channel_layout = (*codec)->channel_layouts[0];
			for (i = 0; (*codec)->channel_layouts[i]; i++) 
			{
				if ((*codec)->channel_layouts[i] == AV_CH_LAYOUT_STEREO)
					c->channel_layout = AV_CH_LAYOUT_STEREO;
			}
		}
		c->channels = libffmpeg::av_get_channel_layout_nb_channels(c->channel_layout);

		libffmpeg::AVRational audioRational;
		audioRational.num = 1;
		audioRational.den = c->sample_rate;
		ost->st->time_base = audioRational;
		break;

	case libffmpeg::AVMEDIA_TYPE_VIDEO:
		c->codec_id = codec_id;

		c->bit_rate = videoBitRate;
		/* Resolution must be a multiple of two. */
		c->width = videoWidth;
		c->height = videoHeight;

		/* timebase: This is the fundamental unit of time (in seconds) in terms
		* of which frame timestamps are represented. For fixed-fps content,
		* timebase should be 1/framerate and timestamp increments should be
		* identical to 1. */
		libffmpeg::AVRational videoRational;
		videoRational.num = 1;
		videoRational.den = videoFrameRate;
		ost->st->time_base = videoRational;
		c->time_base = ost->st->time_base;

		c->gop_size = 12; /* emit one intra frame every twelve frames at most */
		c->pix_fmt = libffmpeg::AV_PIX_FMT_YUV420P;
		if (c->codec_id == libffmpeg::AV_CODEC_ID_MPEG2VIDEO) 
		{
			/* just for testing, we also add B frames */
			c->max_b_frames = 2;
		}
		if (c->codec_id == libffmpeg::AV_CODEC_ID_MPEG1VIDEO)
		{
			/* Needed to avoid using macroblocks in which some coeffs overflow.
			* This does not happen with normal video, it just happens here as
			* the motion of the chroma plane does not match the luma plane. */
			c->mb_decision = 2;
		}
		break;

	default:
		break;
	}

	/* Some formats want stream headers to be separate. */
	if (oc->oformat->flags & AVFMT_GLOBALHEADER)
		c->flags |= AV_CODEC_FLAG_GLOBAL_HEADER;
}

/// <summary>
/// Open the video.
/// </summary>
void open_video(MediaMuxData^ data, libffmpeg::AVFormatContext *oc, libffmpeg::AVCodec *codec, libffmpeg::OutputStream *ost, libffmpeg::AVDictionary *opt_arg)
{
	int ret;
	libffmpeg::AVCodecContext *c = ost->st->codec;
	libffmpeg::AVDictionary *opt = NULL;

	av_dict_copy(&opt, opt_arg, 0);

	/* open the codec */
	ret = avcodec_open2(c, codec, &opt);
	libffmpeg::av_dict_free(&opt);
	if (ret < 0) 
	{
		throw gcnew MediaMuxException("Could not open video codec.");
	}

	/* allocate and init a re-usable frame */
	ost->frame = alloc_picture(c->pix_fmt, c->width, c->height);
	if (!ost->frame) 
	{
		throw gcnew MediaMuxException("Could not allocate video frame.");
	}

	/* If the output format is not YUV420P, then a temporary YUV420P
	* picture is needed too. It is then converted to the required
	* output format. */
	if (c->pix_fmt != libffmpeg::AV_PIX_FMT_YUV420P)
	{
		ost->tmp_frame = NULL;
		ost->tmp_frame = alloc_picture(libffmpeg::AV_PIX_FMT_YUV420P, c->width, c->height);
		if (!ost->tmp_frame) 
		{
			throw gcnew MediaMuxException("Could not allocate temporary picture.");
		}
	}

	if (!ost->frame)
	{
		throw gcnew MediaMuxException("Cannot allocate video picture.");
	}

	// prepare scaling context to convert RGB image to video format
	data->VideoStream->sws_ctx_BGR24 = libffmpeg::sws_getContext(c->width, c->height, libffmpeg::AV_PIX_FMT_BGR24,
		c->width, c->height, c->pix_fmt, SWS_BICUBIC, NULL, NULL, NULL);

	// prepare scaling context to convert grayscale image to video format
	data->VideoStream->sws_ctx_YUV420P = libffmpeg::sws_getContext(c->width, c->height, libffmpeg::AV_PIX_FMT_YUV420P,
		c->width, c->height, c->pix_fmt, SWS_BICUBIC, NULL, NULL, NULL);

	// prepare scaling context to convert grayscale image to video format
	data->VideoStream->sws_ctx_Gray = libffmpeg::sws_getContext(c->width, c->height, libffmpeg::AV_PIX_FMT_GRAY8,
		c->width, c->height, c->pix_fmt, SWS_BICUBIC, NULL, NULL, NULL);

	if ((data->VideoStream->sws_ctx_BGR24 == NULL) || (data->VideoStream->sws_ctx_YUV420P == NULL) || (data->VideoStream->sws_ctx_Gray == NULL))
	{
		throw gcnew MediaMuxException("Cannot initialize frames conversion context.");
	}
}

/// <summary>
/// Allocate video frame.
/// </summary>
libffmpeg::AVFrame *alloc_picture(enum libffmpeg::AVPixelFormat pix_fmt, int width, int height)
{
	libffmpeg::AVFrame* picture;
	void* picture_buf;
	int size;

	// Allocate frame memory.
	picture = libffmpeg::av_frame_alloc();
	if (!picture)
	{
		return NULL;
	}

	// Allocate the picture memory.
	size = libffmpeg::avpicture_get_size(pix_fmt, width, height);
	picture_buf = libffmpeg::av_malloc(size);
	if (!picture_buf)
	{
		// Free if failed.
		libffmpeg::av_free(picture);
		return NULL;
	}

	// Write the picture data into the allocated memory.
	libffmpeg::avpicture_fill((libffmpeg::AVPicture *) picture, (uint8_t *)picture_buf, pix_fmt, width, height);

	// Return the picture.
	return picture;
}

/// <summary>
/// Open the audio.
/// </summary>
void open_audio(libffmpeg::AVFormatContext *oc, libffmpeg::AVCodec *codec, libffmpeg::OutputStream *ost, libffmpeg::AVDictionary *opt_arg)
{
	libffmpeg::AVCodecContext *c;
	int nb_samples;
	int ret;
	libffmpeg::AVDictionary *opt = NULL;

	c = ost->st->codec;

	/* open it */
	libffmpeg::av_dict_copy(&opt, opt_arg, 0);
	ret = libffmpeg::avcodec_open2(c, codec, &opt);
	libffmpeg::av_dict_free(&opt);

	if (ret < 0) 
	{
		throw gcnew MediaMuxException("Could not open audio codec.");
	}

	if (c->codec->capabilities & AV_CODEC_CAP_VARIABLE_FRAME_SIZE)
		nb_samples = 10000;
	else
		nb_samples = c->frame_size;

	ost->frame = alloc_audio_frame(c->sample_fmt, c->channel_layout, c->sample_rate, nb_samples);
	ost->tmp_frame = alloc_audio_frame(c->sample_fmt, c->channel_layout, c->sample_rate, nb_samples);

	/* create resampler context */
	ost->swr_ctx = libffmpeg::swr_alloc();
	if (!ost->swr_ctx) 
	{
		throw gcnew MediaMuxException("Could not allocate resampler context.");
	}

	/* set options */
	av_opt_set_int(ost->swr_ctx, "in_channel_count", c->channels, 0);
	av_opt_set_int(ost->swr_ctx, "in_sample_rate", c->sample_rate, 0);
	av_opt_set_sample_fmt(ost->swr_ctx, "in_sample_fmt", c->sample_fmt, 0);
	av_opt_set_int(ost->swr_ctx, "out_channel_count", c->channels, 0);
	av_opt_set_int(ost->swr_ctx, "out_sample_rate", c->sample_rate, 0);
	av_opt_set_sample_fmt(ost->swr_ctx, "out_sample_fmt", c->sample_fmt, 0);

	/* initialize the resampling context */
	if ((ret = swr_init(ost->swr_ctx)) < 0) 
	{
		throw gcnew MediaMuxException("Failed to initialize the resampling context.");
	}
}

/// <summary>
/// Allocate audio frame.
/// </summary>
libffmpeg::AVFrame *alloc_audio_frame(enum libffmpeg::AVSampleFormat sample_fmt, uint64_t channel_layout, int sample_rate, int nb_samples)
{
	libffmpeg::AVFrame *frame = libffmpeg::av_frame_alloc();
	int ret;

	if (!frame) 
	{
		throw gcnew MediaMuxException("Error allocating an audio frame.");
	}

	frame->format = sample_fmt;
	frame->channel_layout = channel_layout;
	frame->sample_rate = sample_rate;
	frame->nb_samples = nb_samples;

	if (nb_samples) {
		ret = libffmpeg::av_frame_get_buffer(frame, 0);
		if (ret < 0) 
		{
			throw gcnew MediaMuxException("Error allocating an audio buffer.");
		}
	}

	return frame;
}

/// <summary>
/// encode one video frame and send it to the muxer
/// return 1 when encoding is finished, 0 otherwise
/// </summary>
int write_video_frame(MediaMuxData^ data)
{
	int ret;
	libffmpeg::AVCodecContext *c;
	libffmpeg::AVFrame *frame;
	int got_packet = 0;
	libffmpeg::AVPacket pkt = { 0 };

	// Count next frame.
	data->VideoStream->frame->pts = data->VideoStream->next_pts++;

	c = data->VideoStream->st->codec;
	frame = data->VideoStream->frame;
	av_init_packet(&pkt);

	/* encode the image */
	ret = avcodec_encode_video2(c, &pkt, frame, &got_packet);
	if (ret < 0) 
	{
		throw gcnew MediaMuxException("Error encoding video frame.");
	}

	if (got_packet) 
	{
		/* rescale output packet timestamp values from codec to stream timebase */
		libffmpeg::av_packet_rescale_ts(&pkt, c->time_base, data->VideoStream->st->time_base);
		pkt.stream_index = data->VideoStream->st->index;

		/* Write the compressed frame to the media file. */
		ret = av_interleaved_write_frame(data->FormatContext, &pkt);
		libffmpeg::av_packet_unref(&pkt);
	}
	else 
	{
		ret = 0;
	}

	if (ret < 0) 
	{
		throw gcnew MediaMuxException("Error while writing video frame.");
	}

	// Return the state.
	return (frame || got_packet) ? 0 : 1;
}

/// <summary>
/// Write the audio frame.
/// </summary>
/// <param name="data">Audio write data.</param>
int write_audio_frame(MediaMuxData^ data)
{
	libffmpeg::AVCodecContext *c;
	libffmpeg::AVPacket pkt = { 0 }; // data and size must be 0;
	libffmpeg::AVFrame *frame;
	int ret;
	int got_packet;
	int dst_nb_samples;

	frame = data->AudioStream->frame;
	frame->pts = data->AudioStream->next_pts;
	data->AudioStream->next_pts += frame->nb_samples;

	av_init_packet(&pkt);
	c = data->AudioStream->st->codec;
	av_init_packet(&pkt);

	if (frame) {
		/* convert samples from native format to destination codec format, using the resampler */
		/* compute destination number of samples */
		dst_nb_samples = libffmpeg::av_rescale_rnd(libffmpeg::swr_get_delay(data->AudioStream->swr_ctx, c->sample_rate) + frame->nb_samples,
			c->sample_rate, c->sample_rate, libffmpeg::AV_ROUND_UP);

		/* when we pass a frame to the encoder, it may keep a reference to it
		* internally;
		* make sure we do not overwrite it here
		*/
		ret = libffmpeg::av_frame_make_writable(data->AudioStream->frame);
		if (ret < 0)
			exit(1);

		/* convert to destination format */
		ret = libffmpeg::swr_convert(data->AudioStream->swr_ctx, data->AudioStream->frame->data, dst_nb_samples,
			(const uint8_t **)frame->data, frame->nb_samples);

		if (ret < 0) 
		{
			throw gcnew MediaMuxException("Error while converting.");
		}

		frame = data->AudioStream->frame;
		libffmpeg::AVRational audioRational;
		audioRational.num = 1;
		audioRational.den = c->sample_rate;

		frame->pts = libffmpeg::av_rescale_q(data->AudioStream->samples_count, audioRational, c->time_base);
		data->AudioStream->samples_count += dst_nb_samples;
	}

	ret = avcodec_encode_audio2(c, &pkt, frame, &got_packet);
	if (ret < 0) 
	{
		throw gcnew MediaMuxException("Error encoding audio frame.");
	}

	if (got_packet) 
	{
		/* rescale output packet timestamp values from codec to stream timebase */
		libffmpeg::av_packet_rescale_ts(&pkt, c->time_base, data->AudioStream->st->time_base);
		pkt.stream_index = data->AudioStream->st->index;

		/* Write the compressed frame to the media file. */
		ret = av_interleaved_write_frame(data->FormatContext, &pkt);
		libffmpeg::av_packet_unref(&pkt);

		if (ret < 0) 
		{
			throw gcnew MediaMuxException("Error while writing audio frame.");
		}
	}

	// Return the state.
	return (frame || got_packet) ? 0 : 1;
}