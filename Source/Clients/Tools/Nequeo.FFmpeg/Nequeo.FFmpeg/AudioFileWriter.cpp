/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AudioFileWriter.cpp
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

#include "stdafx.h"

#include "AudioFileWriter.h"

using namespace Nequeo::Media::FFmpeg;

/// <summary>
/// Write the audio frame.
/// </summary>
/// <param name="data">Audio write data.</param>
static void write_audio_frame(WriterAudioPrivateData^ data);

/// <summary>
/// Open the audio frame.
/// </summary>
/// <param name="data">Audio write data.</param>
static void open_audio(WriterAudioPrivateData^ data);

/// <summary>
/// Add audio stream.
/// </summary>
/// <param name="data">Audio write data.</param>
/// <param name="bitRate">Audio bit rate.</param>
/// <param name="sampleRate">Audio sample rate.</param>
/// <param name="channels">Audio channels ( 1 - mono, 2 - stereo).</param>
/// <param name="bitsPerSample">Bit depth (bits per sample). Typical values are 8, 16 (default 16).</param>
/// <param name="codecId">Audio codec id.</param>
static void add_audio_stream(WriterAudioPrivateData^ data, int64_t bitRate, int sampleRate, int channels, short bitsPerSample,
	enum libffmpeg::AVCodecID codecId, AudioCodec audioCodec);

/// <summary>
/// Audio file writer.
/// </summary>
AudioFileWriter::AudioFileWriter() :
	_data(nullptr), _disposed(false), _channels(0), _bytesPerSample(0)
{
	libffmpeg::av_register_all();
}

/// <summary>
/// Disposes the object and frees its resources.
/// </summary>
AudioFileWriter::~AudioFileWriter()
{
	// If not disposed.
	if (!_disposed)
	{
		this->!AudioFileWriter();
		_disposed = true;
	}
}

/// <summary>
/// Object's finalizer.
/// </summary>
AudioFileWriter::!AudioFileWriter()
{
	// If not disposed.
	if (!_disposed)
	{
		// Release all resources.
		Close();
	}
}

/// <summary>
/// Close currently opened audio file if any.
/// </summary>
void AudioFileWriter::Close()
{
	// If data exists.
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

			if (_data->AudioStream)
			{
				// Close the audio codec.
				libffmpeg::avcodec_close(_data->AudioStream->codec);
			}

			if (_data->AudioFrame)
			{
				// Free the frame data.
				libffmpeg::av_free(_data->AudioFrame->data[0]);
				libffmpeg::av_free(_data->AudioFrame);
			}

			if (_data->AudioOutputBuffer)
			{
				// Free the output buffer.
				libffmpeg::av_free(_data->AudioOutputBuffer);
			}

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

			if (_data->Packet != NULL)
			{
				// Free the packet.
				delete _data->Packet;
			}

			// Free the context.
			libffmpeg::av_free(_data->FormatContext);
		}

		// Set to null.
		_data = nullptr;
	}
}

/// <summary>
/// Audio file writer.
/// </summary>
/// <param name="fileName">Audio file name to create.</param>
/// <param name="header">The audio wave frame header.</param>
void AudioFileWriter::Open(String^ fileName, WaveStructure header)
{
	Open(fileName, header, AudioCodec::Default);
}

/// <summary>
/// Audio file writer.
/// </summary>
/// <param name="fileName">Audio file name to create.</param>
/// <param name="header">The audio wave frame header.</param>
/// <param name="codec">Audio codec to use for compression.</param>
void AudioFileWriter::Open(String^ fileName, WaveStructure header, AudioCodec codec)
{
	// convert specified managed String to unmanaged string
	IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalUni(fileName);
	wchar_t* nativeFileNameUnicode = (wchar_t*)ptr.ToPointer();
	int utf8StringSize = WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, NULL, 0, NULL, NULL);
	char* nativeFileName = new char[utf8StringSize];
	WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, nativeFileName, utf8StringSize, NULL, NULL);

	bool success = false;

	try
	{
		// Create a new write data.
		_data = gcnew WriterAudioPrivateData();
		_data->Packet = new libffmpeg::AVPacket();
		_data->Packet->data = NULL;

		// guess about destination file format from its file name
		libffmpeg::AVOutputFormat* outputFormat = libffmpeg::av_guess_format(NULL, nativeFileName, NULL);

		// prepare format context
		_data->FormatContext = libffmpeg::avformat_alloc_context();

		// If the format could not be allocated.
		if (!_data->FormatContext)
		{
			throw gcnew AudioException("Cannot allocate format context.");
		}

		// Set the output format from the guessed filename.
		_data->FormatContext->oformat = outputFormat;
		
		// add audio stream using the specified audio codec
		add_audio_stream(_data, (int64_t)header.FmtAverageByteRate * 8, header.SampleRate, (int)header.Channels, header.BitsPerSample,
			(codec == AudioCodec::Default) ? outputFormat->audio_codec : (libffmpeg::AVCodecID) audio_codecs[(int)codec], codec);

		// Open the audio data.
		open_audio(_data);

		// open output file
		if (!(outputFormat->flags & AVFMT_NOFILE))
		{
			if (libffmpeg::avio_open(&_data->FormatContext->pb, nativeFileName, AVIO_FLAG_WRITE) < 0)
			{
				throw gcnew System::IO::IOException("Cannot open the audio file.");
			}
		}

		int data_size = 0;

		// If the block size has be specified.
		if (header.FmtBlockAlign > 0)
		{
			// Set the bytes per sample.
			data_size = header.FmtBlockAlign;
		}
		else
		{
			// if a frame has been decoded, output it
			data_size = libffmpeg::av_get_bytes_per_sample(_data->AudioStream->codec->sample_fmt);
			if (data_size < 0)
			{
				throw gcnew System::IO::IOException("Cannot get the bytes per sample from the sample format.");
			}
		}
		
		// Assign the final audio parameters.
		_channels = _data->AudioStream->codec->channels;
		_bytesPerSample = data_size;
		_numberSamples = _data->AudioFrame->nb_samples;

		// allocate output buffer 
		_data->AudioOutputBufferSize = _numberSamples * _channels * _bytesPerSample;
		_data->AudioOutputBuffer = (uint8_t*)libffmpeg::av_malloc(_data->AudioOutputBufferSize);

		// Write the header data.
		libffmpeg::avformat_write_header(_data->FormatContext, NULL);

		// All is ok.
		success = true;
	}
	finally
	{
		System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr);
		delete[] nativeFileName;

		if (!success)
		{
			// Close the file.
			Close();
		}
	}
}

/// <summary>
/// Audio frame to writer.
/// </summary>
/// <param name="frame">The audio wave frame to write.</param>
void AudioFileWriter::WriteAudioFrame(array<unsigned char>^ frame)
{
	WriteAudioFrame(frame, TimeSpan::MinValue);
}

/// <summary>
/// Audio frame to writer.
/// </summary>
/// <param name="frame">The audio wave frame to write.</param>
/// <param name="timestamp">Frame timestamp, total time since recording started.</param>
/// <remarks><note>The <paramref name="timestamp"/> parameter allows user to specify presentation
/// time of the frame being saved. However, it is user's responsibility to make sure the value is increasing
/// over time.</note></para>
/// </remarks>
void AudioFileWriter::WriteAudioFrame(array<unsigned char>^ frame, TimeSpan timestamp)
{
	// Audio frame to encoder.
	EncodeAudioFrame(frame);

	// Add a time stamp.
	if (timestamp.Ticks >= 0)
	{
		const int audioRate = _data->AudioFrame->nb_samples * _channels;
		const double frameNumber = timestamp.TotalSeconds * audioRate;
		_data->AudioFrame->pts = static_cast<int64_t>(frameNumber);
	}

	// Write the converted frame to the audio file.
	write_audio_frame(_data);
}

/// <summary>
/// Audio frame to writer.
/// </summary>
/// <param name="frame">The audio wave frame to write.</param>
/// <param name="position">The audio frame position.</param>
void AudioFileWriter::WriteAudioFrame(array<unsigned char>^ frame, signed long long position)
{
	// Audio frame to encoder.
	EncodeAudioFrame(frame);

	// Count next frame.
	_data->AudioFrame->pts = position;

	// Write the converted frame to the audio file.
	write_audio_frame(_data);
}

/// <summary>
/// Audio frame to encoder.
/// </summary>
/// <param name="frame">The audio wave frame to write.</param>
void AudioFileWriter::EncodeAudioFrame(array<unsigned char>^ frame)
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
			uint8_t *sample = _data->AudioFrame->data[ch] + (_bytesPerSample * i);

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
}

/// <summary>
/// Write the audio frame.
/// </summary>
/// <param name="data">Audio write data.</param>
void write_audio_frame(WriterAudioPrivateData^ data)
{
	libffmpeg::AVCodecContext* codecContext = data->AudioStream->codec;
	int retEncode, ret = 0;

	// Create the packet.
	libffmpeg::AVPacket* packet = data->Packet;
	libffmpeg::av_init_packet(packet);
	packet->data = data->AudioOutputBuffer;
	packet->size = data->AudioOutputBufferSize;
	packet->stream_index = data->AudioStream->index;

	// If the packet has a key frame.
	if (codecContext->coded_frame->key_frame)
	{
		packet->flags |= AV_PKT_FLAG_KEY;
	}

	int got_packet = 0;

	// encode the audio
	retEncode = libffmpeg::avcodec_encode_audio2(codecContext, packet, data->AudioFrame, &got_packet);

	// if zero size, it means the sound was buffered
	if (retEncode == 0 && got_packet == 1)
	{
		// write the compressed frame to the media file
		ret = libffmpeg::av_interleaved_write_frame(data->FormatContext, packet);
		libffmpeg::av_packet_unref(packet);
	}

	// if the sound was not written.
	if (ret != 0)
	{
		throw gcnew AudioException("Error while writing audio frame.");
	}
}

/// <summary>
/// Allocate sound of the specified format and size.
/// </summary>
/// <param name="data">Audio write data.</param>
static libffmpeg::AVFrame* alloc_sound(WriterAudioPrivateData^ data)
{
	libffmpeg::AVFrame* sound;
	void* sound_buf;
	int size;

	// Get the current context.
	libffmpeg::AVCodecContext* codecContext = data->AudioStream->codec;

	// Allocate frame memory.
	sound = libffmpeg::av_frame_alloc();
	if (!sound)
	{
		return NULL;
	}

	sound->nb_samples = codecContext->frame_size;
	sound->format = codecContext->sample_fmt;
	sound->channel_layout = codecContext->channel_layout;

	// Allocate the picture memory.
	size = libffmpeg::av_samples_get_buffer_size(NULL, codecContext->channels, codecContext->frame_size, codecContext->sample_fmt, 0);
	sound_buf = libffmpeg::av_malloc(size);
	if (!sound_buf)
	{
		// Free if failed.
		libffmpeg::av_free(sound);
		return NULL;
	}

	// setup the data pointers in the AVFrame.
	int ret = libffmpeg::avcodec_fill_audio_frame(sound, codecContext->channels, codecContext->sample_fmt, (uint8_t*)sound_buf, size, 0);
	if (ret < 0)
	{
		return NULL;
	}

	// Return the sound.
	return sound;
}

/// <summary>
/// Select the highest supported samplerate.
/// </summary>
/// <param name="codec">Audio codec.</param>
static int select_sample_rate(libffmpeg::AVCodec *codec)
{
	const int *p;
	int best_samplerate = 0;

	if (!codec->supported_samplerates)
		return 44100;

	p = codec->supported_samplerates;
	while (*p) {
		best_samplerate = FFMAX(*p, best_samplerate);
		p++;
	}
	return best_samplerate;
}

/// <summary>
/// Select layout with the highest channel count
/// </summary>
/// <param name="codec">Audio codec.</param>
static int select_channel_layout(libffmpeg::AVCodec *codec)
{
	const uint64_t *p;
	uint64_t best_ch_layout = 0;
	int best_nb_channels = 0;

	if (!codec->channel_layouts)
		return AV_CH_LAYOUT_STEREO;

	p = codec->channel_layouts;
	while (*p)
	{
		int nb_channels = libffmpeg::av_get_channel_layout_nb_channels(*p);

		if (nb_channels > best_nb_channels) {
			best_ch_layout = *p;
			best_nb_channels = nb_channels;
		}
		p++;
	}

	return best_ch_layout;
}

/// <summary>
/// Open the audio frame.
/// </summary>
/// <param name="data">Audio write data.</param>
void open_audio(WriterAudioPrivateData^ data)
{
	libffmpeg::AVCodecContext* codecContext = data->AudioStream->codec;
	libffmpeg::AVCodec* codec = libffmpeg::avcodec_find_encoder(codecContext->codec_id);

	if (!codec)
	{
		throw gcnew AudioException("Cannot find audio codec.");
	}

	// open the codec 
	if (avcodec_open2(codecContext, codec, NULL) < 0)
	{
		throw gcnew AudioException("Cannot open audio codec.");
	}

	// allocate the encoded sound
	data->AudioFrame = alloc_sound(data);

	if (!data->AudioFrame)
	{
		throw gcnew AudioException("Cannot allocate audio sound.");
	}
}

/// <summary>
/// Add audio stream.
/// </summary>
/// <param name="data">Audio write data.</param>
/// <param name="bitRate">Audio bit rate.</param>
/// <param name="sampleRate">Audio sample rate.</param>
/// <param name="channels">Audio channels ( 1 - mono, 2 - stereo).</param>
/// <param name="bitsPerSample">Bit depth (bits per sample). Typical values are 8, 16 (default 16).</param>
/// <param name="codecId">Audio codec id.</param>
void add_audio_stream(WriterAudioPrivateData^ data, int64_t bitRate, int sampleRate, int channels, 
	short bitsPerSample, enum libffmpeg::AVCodecID codecId, AudioCodec audioCodec)
{
	libffmpeg::AVCodecContext *codecContext;
	libffmpeg::AVCodec *codec;

	// create new stream
	data->AudioStream = libffmpeg::avformat_new_stream(data->FormatContext, NULL);

	if (!data->AudioStream)
	{
		throw gcnew AudioException("Failed creating new audio stream.");
	}

	data->AudioStream->id = 0;
	codecContext = data->AudioStream->codec;
	codecContext->codec_id = codecId;
	codecContext->codec_type = libffmpeg::AVMEDIA_TYPE_AUDIO;

	// Get the audo codec.
	codec = libffmpeg::avcodec_find_encoder(codecContext->codec_id);

	// put sample parameters.
	codecContext->bit_rate = bitRate;
	codecContext->sample_rate = sampleRate;
	codecContext->channels = channels;

	// Select the sample frame.
	switch (audioCodec)
	{
	case Nequeo::Media::FFmpeg::AudioCodec::MP2:
		/* select other audio parameters supported by the encoder */
		codecContext->sample_rate = select_sample_rate(codec);
		codecContext->channel_layout = select_channel_layout(codec);
		codecContext->channels = libffmpeg::av_get_channel_layout_nb_channels(codecContext->channel_layout);

		// Select the sample format.
		switch (bitsPerSample)
		{
		case 8:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_U8;
			break;
		case 16:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16;
			break;
		case 32:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S32;
			break;
		default:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16;
			break;
		}

		// Calculate the bit rate.
		// From the sample format S16 (16/8 = 2) or (8/8 = 1)
		codecContext->bit_rate = ((codecContext->sample_rate * codecContext->channels) * (codecContext->bits_per_coded_sample / 8)) * 8;
		break;
	case Nequeo::Media::FFmpeg::AudioCodec::MP3:
		// Select the sample format.
		switch (bitsPerSample)
		{
		case 8:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_U8P;
			break;
		case 16:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16P;
			break;
		case 32:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S32P;
			break;
		default:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16P;
			break;
		}
		break;
	case Nequeo::Media::FFmpeg::AudioCodec::AAC:
		codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_FLTP;
		break;
	case Nequeo::Media::FFmpeg::AudioCodec::WMA_v1:
		codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_FLTP;
		break;
	case Nequeo::Media::FFmpeg::AudioCodec::WMA_v2:
		codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_FLTP;
		break;
	default:
		switch (codecId)
		{
		case libffmpeg::AV_CODEC_ID_MP2:
			/* select other audio parameters supported by the encoder */
			codecContext->sample_rate = select_sample_rate(codec);
			codecContext->channel_layout = select_channel_layout(codec);
			codecContext->channels = libffmpeg::av_get_channel_layout_nb_channels(codecContext->channel_layout);

			// Select the sample format.
			switch (bitsPerSample)
			{
			case 8:
				codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_U8;
				break;
			case 16:
				codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16;
				break;
			case 32:
				codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S32;
				break;
			default:
				codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16;
				break;
			}

			// Calculate the bit rate.
			// From the sample format S16 (16/8 = 2) or (8/8 = 1)
			codecContext->bit_rate = ((codecContext->sample_rate * codecContext->channels) * (codecContext->bits_per_coded_sample / 8)) * 8;
			break;
		case libffmpeg::AV_CODEC_ID_MP3:
			// Select the sample format.
			switch (bitsPerSample)
			{
			case 8:
				codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_U8P;
				break;
			case 16:
				codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16P;
				break;
			case 32:
				codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S32P;
				break;
			default:
				codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16P;
				break;
			}
			break;
		case libffmpeg::AV_CODEC_ID_AAC:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_FLTP;
			break;
		case libffmpeg::AV_CODEC_ID_WMAV1:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_FLTP;
			break;
		case libffmpeg::AV_CODEC_ID_WMAV2:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_FLTP;
			break;
		default:
			codecContext->sample_fmt = libffmpeg::AV_SAMPLE_FMT_NONE;
			break;
		}
		break;
	}
}
