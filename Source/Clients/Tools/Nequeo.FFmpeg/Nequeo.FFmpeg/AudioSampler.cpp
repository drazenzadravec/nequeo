/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AudioSampler.cpp
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

#include "stdafx.h"

#include "AudioSampler.h"

using namespace Nequeo::Media::FFmpeg;

static int get_format_from_sample_fmt(const char **fmt, enum libffmpeg::AVSampleFormat sample_fmt);

/// <summary>
/// Audio sampler.
/// </summary>
AudioSampler::AudioSampler() :
	_disposed(false), _data(nullptr)
{
	libffmpeg::av_register_all();
}

/// <summary>
/// Disposes the object and frees its resources.
/// </summary>
AudioSampler::~AudioSampler()
{
	// If not disposed.
	if (!_disposed)
	{
		this->!AudioSampler();
		_disposed = true;
	}
}

/// <summary>
/// Object's finalizer.
/// </summary>
AudioSampler::!AudioSampler()
{
	// If not disposed.
	if (!_disposed)
	{
		// Release all resources.
		Close();
	}
}

/// <summary>
/// Open the re-sampler.
/// </summary>
/// <param name="sourceHeader">The audio source wave frame header.</param>
/// <param name="sourceFormat">The audio source sample format.</param>
/// <param name="sourceNumberSamples">The audio source number of samples per channel.</param>
/// <param name="destinationHeader">The audio destination wave frame header.</param>
/// <param name="destinationFormat">The audio destination sample format.</param>
/// <param name="destinationNumberSamples">The audio destination number of samples per channel.</param>
void AudioSampler::Open(WaveStructure sourceHeader, SampleFormat sourceFormat, int sourceNumberSamples, 
	WaveStructure destinationHeader, SampleFormat destinationFormat, [System::Runtime::InteropServices::OutAttribute] int% destinationNumberSamples)
{
	int ret;
	bool success = false;

	try
	{
		// Create the reader data packet.
		_data = gcnew AudioSamplerData(sourceHeader.SampleRate, destinationHeader.SampleRate, sourceNumberSamples);
		_data->Native = new libffmpeg::AudioSamplerDataNative();

		// create resampler context.
		_data->Native->swr_ctx = libffmpeg::swr_alloc();
		if (!_data->Native->swr_ctx) 
		{
			throw gcnew Exception("Could not allocate resampler context.");
		}

		// Set the sources channel.
		switch (sourceHeader.Channels)
		{
		case 1:
			_data->src_ch_layout = AV_CH_LAYOUT_MONO;
			break;
		case 2:
			_data->src_ch_layout = AV_CH_LAYOUT_STEREO;
			break;
		default:
			break;
		}

		// Set the destination channel.
		switch (destinationHeader.Channels)
		{
		case 1:
			_data->dst_ch_layout = AV_CH_LAYOUT_MONO;
			break;
		case 2:
			_data->dst_ch_layout = AV_CH_LAYOUT_STEREO;
			break;
		default:
			break;
		}

		// Set the source sample format.
		switch (sourceFormat)
		{
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_NONE:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_NONE;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_U8:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_U8;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_S16:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_S32:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_S32;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_FLT:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_FLT;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_DBL:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_DBL;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_U8P:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_U8P;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_S16P:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16P;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_S32P:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_S32P;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_FLTP:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_FLTP;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_DBLP:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_DBLP;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_NB:
			_data->src_sample_fmt = libffmpeg::AV_SAMPLE_FMT_NB;
			break;
		default:
			break;
		}

		// Set the destination sample format.
		switch (destinationFormat)
		{
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_NONE:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_NONE;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_U8:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_U8;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_S16:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_S32:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_S32;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_FLT:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_FLT;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_DBL:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_DBL;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_U8P:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_U8P;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_S16P:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_S16P;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_S32P:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_S32P;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_FLTP:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_FLTP;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_DBLP:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_DBLP;
			break;
		case Nequeo::Media::FFmpeg::SampleFormat::AV_SAMPLE_FMT_NB:
			_data->dst_sample_fmt = libffmpeg::AV_SAMPLE_FMT_NB;
			break;
		default:
			break;
		}

		/* set options */
		libffmpeg::av_opt_set_int(_data->Native->swr_ctx, "in_channel_layout", _data->src_ch_layout, 0);
		libffmpeg::av_opt_set_int(_data->Native->swr_ctx, "in_sample_rate", _data->src_rate, 0);
		libffmpeg::av_opt_set_sample_fmt(_data->Native->swr_ctx, "in_sample_fmt", _data->src_sample_fmt, 0);

		libffmpeg::av_opt_set_int(_data->Native->swr_ctx, "out_channel_layout", _data->dst_ch_layout, 0);
		libffmpeg::av_opt_set_int(_data->Native->swr_ctx, "out_sample_rate", _data->dst_rate, 0);
		libffmpeg::av_opt_set_sample_fmt(_data->Native->swr_ctx, "out_sample_fmt", _data->dst_sample_fmt, 0);

		/* initialize the resampling context */
		if ((ret = libffmpeg::swr_init(_data->Native->swr_ctx)) < 0)
		{
			throw gcnew Exception("Failed to initialize the resampling context.");
		}

		uint8_t **src_data = _data->SourceData;
		uint8_t **dst_data = _data->DestinationData;
		int src_linesize = _data->src_linesize;
		int dst_linesize = _data->dst_linesize;

		/* allocate source and destination samples buffers */
		_data->src_nb_channels = libffmpeg::av_get_channel_layout_nb_channels(_data->src_ch_layout);
		ret = libffmpeg::av_samples_alloc_array_and_samples(&src_data, &src_linesize, _data->src_nb_channels, _data->src_nb_samples, _data->src_sample_fmt, 0);
		if (ret < 0) 
		{
			throw gcnew Exception("Could not allocate source samples.");
		}

		/* compute the number of converted samples: buffering is avoided
		* ensuring that the output buffer will contain at least all the
		* converted input samples */
		_data->dst_nb_samples = libffmpeg::av_rescale_rnd(_data->src_nb_samples, _data->dst_rate, _data->src_rate, libffmpeg::AV_ROUND_UP);
		_data->max_dst_nb_samples = _data->dst_nb_samples;
		destinationNumberSamples = _data->dst_nb_samples;

		/* buffer is going to be directly written to a rawaudio file, no alignment */
		_data->dst_nb_channels = libffmpeg::av_get_channel_layout_nb_channels(_data->dst_ch_layout);
		ret = libffmpeg::av_samples_alloc_array_and_samples(&dst_data, &dst_linesize, _data->dst_nb_channels, _data->dst_nb_samples, _data->dst_sample_fmt, 0);

		if (ret < 0) 
		{
			throw gcnew Exception("Could not allocate destination samples.");
		}

		// if a frame has been decoded, output it
		int data_size = libffmpeg::av_get_bytes_per_sample(_data->src_sample_fmt);
		if (data_size < 0)
		{
			throw gcnew Exception("Cannot get the bytes per sample from the sample format.");
		}

		// Assign the source and destination data store.
		_data->SourceData = src_data;
		_data->DestinationData = dst_data;
		_data->src_linesize = src_linesize;
		_data->dst_linesize = dst_linesize;

		_numberSamples = sourceNumberSamples;
		_channels = _data->src_nb_channels;
		_bytesPerSample = data_size;

		// All is ok.
		success = true;
	}
	finally
	{
		if (!success)
		{
			// Close the file.
			Close();
		}
	}
}

/// <summary>
/// Re-sample the audio frame.
/// </summary>
/// <param name="frame">The audio source frame.</param>
/// <returns>The re-sampled audio frame.</returns>
array<unsigned char>^ AudioSampler::Resample(array<unsigned char>^ frame)
{
	int ret;
	int dst_bufsize;
	array<unsigned char>^ sound = nullptr;

	// Create a new sound data collection.
	List<unsigned char>^ soundData = gcnew List<unsigned char>();

	if (_data == nullptr)
	{
		throw gcnew Exception("An audio sample has not been set yet.");
	}

	// Write to the source buffer.
	uint8_t **src_data = _data->SourceData;
	uint8_t **dst_data = _data->DestinationData;
	int src_linesize = _data->src_linesize;
	int dst_linesize = _data->dst_linesize;

	// Contains the samples per channel.
	uint64_t frameByteIndex = 0;

	// For sample.
	for (uint64_t i = 0; i < _numberSamples; i++)
	{
		// For each channel.
		for (int ch = 0; ch < _channels; ch++)
		{
			// Get the pointer to this set of data.
			uint8_t *sample = src_data[ch] + (_bytesPerSample * i);

			// Write the data size.
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

	/* compute destination number of samples */
	_data->dst_nb_samples = libffmpeg::av_rescale_rnd(libffmpeg::swr_get_delay(_data->Native->swr_ctx, _data->src_rate) + _data->src_nb_samples, _data->dst_rate, _data->src_rate, libffmpeg::AV_ROUND_UP);
	if (_data->dst_nb_samples > _data->max_dst_nb_samples)
	{
		libffmpeg::av_freep(&dst_data[0]);
		ret = libffmpeg::av_samples_alloc(dst_data, &dst_linesize, _data->dst_nb_channels, _data->dst_nb_samples, _data->dst_sample_fmt, 1);

		if (ret < 0)
			return nullptr;

		_data->max_dst_nb_samples = _data->dst_nb_samples;
	}

	/* convert to destination format */
	ret = libffmpeg::swr_convert(_data->Native->swr_ctx, dst_data, _data->dst_nb_samples, (const uint8_t **)src_data, _data->src_nb_samples);
	if (ret < 0) 
	{
		throw gcnew Exception("Error while converting.");
	}

	dst_bufsize = libffmpeg::av_samples_get_buffer_size(&dst_linesize, _data->dst_nb_channels, ret, _data->dst_sample_fmt, 1);
	if (dst_bufsize < 0) 
	{
		throw gcnew Exception("Could not get sample buffer size.");
	}

	// if a frame has been decoded, output it
	int data_size = libffmpeg::av_get_bytes_per_sample(_data->dst_sample_fmt);
	if (data_size < 0)
	{
		return nullptr;
	}
	else
	{
		// For the number of samples per channel.
		for (int i = 0; i < _data->dst_nb_samples; i++)
		{
			// For the number of channels.
			for (int ch = 0; ch < _data->dst_nb_channels; ch++)
			{
				// If channel exists.
				if (dst_data[ch])
				{
					// Get the point to this set of data.
					uint8_t *sample = dst_data[ch] + (data_size * i);

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

/// <summary>
/// Close currently opened audio file if any.
/// </summary>
void AudioSampler::Close()
{
	// If data is not null.
	if (_data != nullptr)
	{
		if (_data->SourceData)
		{
			libffmpeg::av_freep(&_data->SourceData[0]);

			// Release the source data.
			uint8_t **src_data = _data->SourceData;
			libffmpeg::av_freep(&src_data);
		}

		if (_data->DestinationData)
		{
			libffmpeg::av_freep(&_data->DestinationData[0]);

			// Release the destination data.
			uint8_t **dst_data = _data->DestinationData;
			libffmpeg::av_freep(&dst_data);
		}

		if (_data->Native != NULL)
		{
			if (_data->Native->swr_ctx != NULL)
			{
				// Free the swr context.
				libffmpeg::swr_free(&_data->Native->swr_ctx);
			}

			// Delete the native data.
			delete _data->Native;
		}

		_data = nullptr;
	}
}

/// <summary>
/// Get the new format type.
/// </summary>
/// <param name="fmt">The audio format.</param>
/// <param name="sample_fmt">The sample foramt.</param>
/// <returns>The result.</returns>
int get_format_from_sample_fmt(const char **fmt, enum libffmpeg::AVSampleFormat sample_fmt)
{
	int i;
	struct sample_fmt_entry 
	{
		enum libffmpeg::AVSampleFormat sample_fmt; 
		const char *fmt_be;
		const char *fmt_le;

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

	throw gcnew Exception("Sample format is not supported as output format.");
	return AVERROR(EINVAL);
}
