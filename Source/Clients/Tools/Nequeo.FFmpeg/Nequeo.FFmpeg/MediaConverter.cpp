/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaConverter.cpp
*  Purpose :       MediaConverter class.
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

#include "MediaConverter.h"

using namespace Nequeo::Media::FFmpeg;

static int open_input_file(MediaConverterData^ data, const char *filename);
static int open_output_file(MediaConverterData^ data, const char *filename);
static int init_filters(MediaConverterData^ data);
static int init_filter(MediaConverterData^ data, MediaConverterDataNative* fctx, libffmpeg::AVCodecContext *dec_ctx, 
	libffmpeg::AVCodecContext *enc_ctx, const char *filter_spec);
static int encode_write_frame(MediaConverterData^ data, libffmpeg::AVFrame *filt_frame, unsigned int stream_index, int *got_frame);
static int filter_encode_write_frame(MediaConverterData^ data, libffmpeg::AVFrame *frame, unsigned int stream_index);
static int flush_encoder(MediaConverterData^ data, unsigned int stream_index);

/// <summary>
/// Audio video converter.
/// </summary>
MediaConverter::MediaConverter() :
	_disposed(false), _data(nullptr)
{
	libffmpeg::av_register_all();
	libffmpeg::avfilter_register_all();
}

/// <summary>
/// Disposes the object and frees its resources.
/// </summary>
MediaConverter::~MediaConverter()
{
	// If not disposed.
	if (!_disposed)
	{
		this->!MediaConverter();
		_disposed = true;
	}
}

/// <summary>
/// Object's finalizer.
/// </summary>
MediaConverter::!MediaConverter()
{
	// If not disposed.
	if (!_disposed)
	{
		// Release all resources.
		Close();
	}
}

/// <summary>
/// Convert source file to destination file.
/// </summary>
/// <param name="sourceFile">Audio video file name to convert.</param>
/// <param name="destinationFile">Converted audio video file name.</param>
void MediaConverter::Open(String^ sourceFile, String^ destinationFile)
{
	int ret;
	bool success = false;

	// convert specified managed String to unmanaged string
	IntPtr ptrSourceFile = System::Runtime::InteropServices::Marshal::StringToHGlobalUni(sourceFile);
	wchar_t* nativeSourceFileNameUnicode = (wchar_t*)ptrSourceFile.ToPointer();
	int utf8StringSizeSource = WideCharToMultiByte(CP_UTF8, 0, nativeSourceFileNameUnicode, -1, NULL, 0, NULL, NULL);
	char* nativeSourceFileName = new char[utf8StringSizeSource];
	WideCharToMultiByte(CP_UTF8, 0, nativeSourceFileNameUnicode, -1, nativeSourceFileName, utf8StringSizeSource, NULL, NULL);

	// convert specified managed String to unmanaged string
	IntPtr ptrDestinationFile = System::Runtime::InteropServices::Marshal::StringToHGlobalUni(destinationFile);
	wchar_t* nativeDestinationFileNameUnicode = (wchar_t*)ptrDestinationFile.ToPointer();
	int utf8StringSizeDestination = WideCharToMultiByte(CP_UTF8, 0, nativeDestinationFileNameUnicode, -1, NULL, 0, NULL, NULL);
	char* nativeDestinationFileName = new char[utf8StringSizeDestination];
	WideCharToMultiByte(CP_UTF8, 0, nativeDestinationFileNameUnicode, -1, nativeDestinationFileName, utf8StringSizeDestination, NULL, NULL);

	try
	{
		// Create the reader data packet.
		_data = gcnew MediaConverterData();
		_data->Packet = new libffmpeg::AVPacket();
		_data->Packet->data = NULL;
		_data->Packet->size = 0;

		// Open input file.
		if ((ret = open_input_file(_data, nativeSourceFileName)) < 0)
			throw gcnew MediaConverterException("Could not open input file.");

		// Open output file.
		if ((ret = open_output_file(_data, nativeDestinationFileName)) < 0)
			throw gcnew MediaConverterException("Could not create output file.");

		// Open filters.
		if ((ret = init_filters(_data)) < 0)
			throw gcnew MediaConverterException("Could not create filters.");

		libffmpeg::AVMediaType type;
		unsigned int stream_index;
		unsigned int i;
		int got_frame;

		// Set the encoding function handler.
		dec_func decodingFuncHandler;

		/* read all packets */
		while (1) 
		{
			if ((ret = libffmpeg::av_read_frame(_data->FormatContextIn, _data->Packet)) < 0)
				break;

			stream_index = _data->Packet->stream_index;
			type = (libffmpeg::AVMediaType)(_data->FormatContextIn->streams[_data->Packet->stream_index]->codec->codec_type);

			if (_data->NativeData[stream_index].filter_graph)
			{
				_data->Frame = libffmpeg::av_frame_alloc();
				if (!_data->Frame)
				{
					ret = AVERROR(ENOMEM);
					break;
				}

				libffmpeg::av_packet_rescale_ts(_data->Packet,
					_data->FormatContextIn->streams[stream_index]->time_base,
					_data->FormatContextIn->streams[stream_index]->codec->time_base);

				decodingFuncHandler = (type == libffmpeg::AVMEDIA_TYPE_VIDEO) ? libffmpeg::avcodec_decode_video2 : libffmpeg::avcodec_decode_audio4;
				ret = decodingFuncHandler(_data->FormatContextIn->streams[stream_index]->codec, _data->Frame, &got_frame, _data->Packet);

				if (ret < 0) 
				{
					libffmpeg::AVFrame* frame = _data->Frame;
					libffmpeg::av_frame_free(&frame);
					throw gcnew MediaConverterException("Decoding failed.");
					break;
				}

				if (got_frame) 
				{
					_data->Frame->pts = libffmpeg::av_frame_get_best_effort_timestamp(_data->Frame);
					ret = filter_encode_write_frame(_data, _data->Frame, stream_index);

					libffmpeg::AVFrame* frame = _data->Frame;
					libffmpeg::av_frame_free(&frame);
					if (ret < 0)
						goto end;
				}
				else 
				{
					libffmpeg::AVFrame* frame = _data->Frame;
					libffmpeg::av_frame_free(&frame);
				}
			}
			else 
			{
				/* remux this frame without reencoding */
				libffmpeg::av_packet_rescale_ts(_data->Packet,
					_data->FormatContextIn->streams[stream_index]->time_base,
					_data->FormatContextOut->streams[stream_index]->time_base);

				ret = libffmpeg::av_interleaved_write_frame(_data->FormatContextOut, _data->Packet);
				if (ret < 0)
					goto end;
			}

			libffmpeg::av_packet_unref(_data->Packet);
		}

		/* flush filters and encoders */
		for (i = 0; i < _data->FormatContextIn->nb_streams; i++)
		{
			/* flush filter */
			if (!_data->NativeData[i].filter_graph)
				continue;

			ret = filter_encode_write_frame(_data, NULL, i);
			if (ret < 0) 
			{
				throw gcnew MediaConverterException("Flushing filter failed.");
				goto end;
			}

			/* flush encoder */
			ret = flush_encoder(_data, i);
			if (ret < 0) 
			{
				throw gcnew MediaConverterException("Flushing encoder failed.");
				goto end;
			}
		}

		// If context exists.
		if (_data->FormatContextOut)
		{
			// Write last bit of data.
			libffmpeg::av_write_trailer(_data->FormatContextOut);
		}

	end:
		// All is good.
		success = true;
	}
	finally
	{
		System::Runtime::InteropServices::Marshal::FreeHGlobal(ptrSourceFile);
		delete[] nativeSourceFileName;

		System::Runtime::InteropServices::Marshal::FreeHGlobal(ptrDestinationFile);
		delete[] nativeDestinationFileName;

		if (!success)
		{
			// Close the stream.
			Close();
		}
	}
}

/// <summary>
/// Close currently opened files if any.
/// </summary>
void MediaConverter::Close()
{
	// If data is not null.
	if (_data != nullptr)
	{
		// Cleanup the packet.
		if (_data->Packet != NULL)
		{
			// Deref the packet.
			libffmpeg::AVPacket* packet = _data->Packet;
			libffmpeg::av_packet_unref(packet);

			// Free the packet.
			libffmpeg::av_free_packet(_data->Packet);
		}

		if (_data->Frame != NULL)
		{
			// Free audio frame.
			libffmpeg::AVFrame* frame = _data->Frame;
			libffmpeg::av_frame_free(&frame);
		}

		// If context exists.
		if (_data->FormatContextOut && _data->FormatContextIn)
		{
			// For each stream.
			for (int i = 0; i < _data->FormatContextIn->nb_streams; i++)
			{
				libffmpeg::avcodec_close(_data->FormatContextIn->streams[i]->codec);

				if (_data->FormatContextOut && _data->FormatContextOut->nb_streams > i && _data->FormatContextOut->streams[i] && _data->FormatContextOut->streams[i]->codec)
					libffmpeg::avcodec_close(_data->FormatContextOut->streams[i]->codec);

				if (_data->NativeData && _data->NativeData[i].filter_graph)
					libffmpeg::avfilter_graph_free(&_data->NativeData[i].filter_graph);
			}
		}

		// If native data.
		if (_data->NativeData != NULL)
		{
			libffmpeg::av_free(_data->NativeData);
		}

		// If context exists.
		if (_data->FormatContextIn)
		{
			// Close the format context.
			libffmpeg::AVFormatContext* avFormatContext = _data->FormatContextIn;
			libffmpeg::avformat_close_input(&avFormatContext);
		}

		// If context exists.
		if (_data->FormatContextOut)
		{
			if (_data->FormatContextOut && !(_data->FormatContextOut->oformat->flags & AVFMT_NOFILE))
				libffmpeg::avio_closep(&_data->FormatContextOut->pb);

			// Free the context.
			libffmpeg::avformat_free_context(_data->FormatContextOut);
		}

		_data = nullptr;
	}
}

/// <summary>
/// Open input file.
/// </summary>
/// <param name="data">Media data.</param>
/// <param name="filename">Input file name.</param>
/// <return>The return value.</return>
int open_input_file(MediaConverterData^ data, const char *filename)
{
	int ret;
	unsigned int i;

	libffmpeg::AVFormatContext* avFormatContext = data->FormatContextIn;
	if ((ret = avformat_open_input(&avFormatContext, filename, NULL, NULL)) < 0)
	{
		throw gcnew MediaConverterException("Cannot open input file.");
		return ret;
	}

	// Assign the new format.
	data->FormatContextIn = avFormatContext;

	if ((ret = avformat_find_stream_info(avFormatContext, NULL)) < 0)
	{
		throw gcnew MediaConverterException("Cannot find stream information.");
		return ret;
	}

	for (i = 0; i < avFormatContext->nb_streams; i++)
	{
		libffmpeg::AVStream *stream;
		libffmpeg::AVCodecContext *codec_ctx;

		// Assign the stream and codec context.
		stream = avFormatContext->streams[i];
		codec_ctx = stream->codec;

		/* Reencode video & audio and remux subtitles etc. */
		if (codec_ctx->codec_type == libffmpeg::AVMEDIA_TYPE_VIDEO
			|| codec_ctx->codec_type == libffmpeg::AVMEDIA_TYPE_AUDIO)
		{
			/* Open decoder */
			ret = avcodec_open2(codec_ctx, avcodec_find_decoder(codec_ctx->codec_id), NULL);
			if (ret < 0) 
			{
				throw gcnew MediaConverterException("Failed to open decoder for stream.");
				return ret;
			}
		}
	}

	// All is good.
	return 0;
}

/// <summary>
/// Open output file.
/// </summary>
/// <param name="data">Media data.</param>
/// <param name="filename">Output file name.</param>
/// <return>The return value.</return>
int open_output_file(MediaConverterData^ data, const char *filename)
{
	libffmpeg::AVStream *out_stream;
	libffmpeg::AVStream *in_stream;
	libffmpeg::AVCodecContext *dec_ctx, *enc_ctx;
	libffmpeg::AVCodec *encoder;
	int ret;
	unsigned int i;

	libffmpeg::AVFormatContext* avFormatContext = data->FormatContextOut;
	libffmpeg::avformat_alloc_output_context2(&avFormatContext, NULL, NULL, filename);
	if (!avFormatContext)
	{
		throw gcnew MediaConverterException("Could not create output context.");
		return AVERROR_UNKNOWN;
	}

	// Assign the new format.
	data->FormatContextOut = avFormatContext;

	for (i = 0; i < data->FormatContextIn->nb_streams; i++)
	{
		out_stream = libffmpeg::avformat_new_stream(avFormatContext, NULL);
		if (!out_stream) 
		{
			throw gcnew MediaConverterException("Failed allocating output stream.");
			return AVERROR_UNKNOWN;
		}

		in_stream = data->FormatContextIn->streams[i];
		dec_ctx = in_stream->codec;
		enc_ctx = out_stream->codec;

		if (dec_ctx->codec_type == libffmpeg::AVMEDIA_TYPE_VIDEO
			|| dec_ctx->codec_type == libffmpeg::AVMEDIA_TYPE_AUDIO) 
		{
			/* in this example, we choose transcoding to same codec */
			encoder = libffmpeg::avcodec_find_encoder(dec_ctx->codec_id);
			if (!encoder) 
			{
				throw gcnew MediaConverterException("Necessary encoder not found.");
				return AVERROR_INVALIDDATA;
			}

			/* In this example, we transcode to same properties (picture size,
			* sample rate etc.). These properties can be changed for output
			* streams easily using filters */
			if (dec_ctx->codec_type == libffmpeg::AVMEDIA_TYPE_VIDEO)
			{
				enc_ctx->height = dec_ctx->height;
				enc_ctx->width = dec_ctx->width;
				enc_ctx->sample_aspect_ratio = dec_ctx->sample_aspect_ratio;
				/* take first format from list of supported formats */
				enc_ctx->pix_fmt = encoder->pix_fmts[0];
				/* video time_base can be set to whatever is handy and supported by encoder */
				enc_ctx->time_base = dec_ctx->time_base;
			}
			else 
			{
				enc_ctx->sample_rate = dec_ctx->sample_rate;
				enc_ctx->channel_layout = dec_ctx->channel_layout;
				enc_ctx->channels = libffmpeg::av_get_channel_layout_nb_channels(enc_ctx->channel_layout);
				/* take first format from list of supported formats */
				enc_ctx->sample_fmt = encoder->sample_fmts[0];

				libffmpeg::AVRational audioRational;
				audioRational.num = 1;
				audioRational.den = enc_ctx->sample_rate;
				enc_ctx->time_base = audioRational;
			}

			/* Third parameter can be used to pass settings to encoder */
			ret = avcodec_open2(enc_ctx, encoder, NULL);
			if (ret < 0) {
				throw gcnew MediaConverterException("Cannot open video encoder for stream.");
				return ret;
			}
		}
		else if (dec_ctx->codec_type == libffmpeg::AVMEDIA_TYPE_UNKNOWN) 
		{
			throw gcnew MediaConverterException("Elementary stream is of unknown type, cannot proceed.");
			return AVERROR_INVALIDDATA;
		}
		else 
		{
			/* if this stream must be remuxed */
			ret = libffmpeg::avcodec_copy_context(avFormatContext->streams[i]->codec, data->FormatContextIn->streams[i]->codec);
			if (ret < 0) 
			{
				throw gcnew MediaConverterException("Copying stream context failed.");
				return ret;
			}
		}

		if (avFormatContext->oformat->flags & AVFMT_GLOBALHEADER)
			enc_ctx->flags |= AV_CODEC_FLAG_GLOBAL_HEADER;

	}
	
	if (!(avFormatContext->oformat->flags & AVFMT_NOFILE))
	{
		ret = libffmpeg::avio_open(&avFormatContext->pb, filename, AVIO_FLAG_WRITE);
		if (ret < 0) 
		{
			throw gcnew MediaConverterException("Could not open output file");
			return ret;
		}
	}

	/* init muxer, write output file header */
	ret = libffmpeg::avformat_write_header(avFormatContext, NULL);
	if (ret < 0) 
	{
		throw gcnew MediaConverterException("Error occurred when opening output file.");
		return ret;
	}

	return 0;
}

/// <summary>
/// Open filter.
/// </summary>
/// <param name="data">Media data.</param>
/// <return>The return value.</return>
int init_filters(MediaConverterData^ data)
{
	const char *filter_spec;
	unsigned int i;
	int ret;

	data->NativeData = (MediaConverterDataNative*)libffmpeg::av_malloc_array(data->FormatContextIn->nb_streams, sizeof(*data->NativeData));
	if (!data->NativeData)
		return AVERROR(ENOMEM);

	for (i = 0; i < data->FormatContextIn->nb_streams; i++) 
	{
		data->NativeData[i].buffersrc_ctx = NULL;
		data->NativeData[i].buffersink_ctx = NULL;
		data->NativeData[i].filter_graph = NULL;

		if (!(data->FormatContextIn->streams[i]->codec->codec_type == libffmpeg::AVMEDIA_TYPE_AUDIO
			|| data->FormatContextIn->streams[i]->codec->codec_type == libffmpeg::AVMEDIA_TYPE_VIDEO))
			continue;

		if (data->FormatContextIn->streams[i]->codec->codec_type == libffmpeg::AVMEDIA_TYPE_VIDEO)
			filter_spec = "null"; /* passthrough (dummy) filter for video */
		else
			filter_spec = "anull"; /* passthrough (dummy) filter for audio */

		ret = init_filter(data, &data->NativeData[i], data->FormatContextIn->streams[i]->codec,
			data->FormatContextOut->streams[i]->codec, filter_spec);

		if (ret)
			return ret;
	}
	return 0;
}

/// <summary>
/// Open filter.
/// </summary>
/// <param name="data">Media data.</param>
/// <return>The return value.</return>
int init_filter(MediaConverterData^ data, MediaConverterDataNative* fctx, libffmpeg::AVCodecContext *dec_ctx, libffmpeg::AVCodecContext *enc_ctx, const char *filter_spec)
{
	char args[512];
	int ret = 0;
	const char *error = NULL;

	libffmpeg::AVFilter *buffersrc = NULL;
	libffmpeg::AVFilter *buffersink = NULL;
	libffmpeg::AVFilterContext *buffersrc_ctx = NULL;
	libffmpeg::AVFilterContext *buffersink_ctx = NULL;
	libffmpeg::AVFilterInOut *outputs = libffmpeg::avfilter_inout_alloc();
	libffmpeg::AVFilterInOut *inputs = libffmpeg::avfilter_inout_alloc();
	libffmpeg::AVFilterGraph *filter_graph = libffmpeg::avfilter_graph_alloc();

	if (!outputs || !inputs || !filter_graph) 
	{
		ret = AVERROR(ENOMEM);
		goto end;
	}

	if (dec_ctx->codec_type == libffmpeg::AVMEDIA_TYPE_VIDEO)
	{
		buffersrc = libffmpeg::avfilter_get_by_name("buffer");
		buffersink = libffmpeg::avfilter_get_by_name("buffersink");
		if (!buffersrc || !buffersink) 
		{
			error = "filtering source or sink element not found.";
			ret = AVERROR_UNKNOWN;
			goto end;
		}

		snprintf(args, sizeof(args),
			"video_size=%dx%d:pix_fmt=%d:time_base=%d/%d:pixel_aspect=%d/%d",
			dec_ctx->width, dec_ctx->height, dec_ctx->pix_fmt,
			dec_ctx->time_base.num, dec_ctx->time_base.den,
			dec_ctx->sample_aspect_ratio.num,
			dec_ctx->sample_aspect_ratio.den);

		ret = avfilter_graph_create_filter(&buffersrc_ctx, buffersrc, "in", args, NULL, filter_graph);
		if (ret < 0) 
		{
			error = "Cannot create buffer source.";
			goto end;
		}

		ret = avfilter_graph_create_filter(&buffersink_ctx, buffersink, "out", NULL, NULL, filter_graph);
		if (ret < 0) 
		{
			error = "Cannot create buffer sink.";
			goto end;
		}

		ret = av_opt_set_bin(buffersink_ctx, "pix_fmts", (uint8_t*)&enc_ctx->pix_fmt, sizeof(enc_ctx->pix_fmt), AV_OPT_SEARCH_CHILDREN);
		if (ret < 0) 
		{
			error = "Cannot set output pixel format.";
			goto end;
		}
	}
	else if (dec_ctx->codec_type == libffmpeg::AVMEDIA_TYPE_AUDIO)
	{
		buffersrc = libffmpeg::avfilter_get_by_name("abuffer");
		buffersink = libffmpeg::avfilter_get_by_name("abuffersink");

		if (!buffersrc || !buffersink) 
		{
			error = "filtering source or sink element not found.";
			ret = AVERROR_UNKNOWN;
			goto end;
		}

		if (!dec_ctx->channel_layout)
			dec_ctx->channel_layout = libffmpeg::av_get_default_channel_layout(dec_ctx->channels);

		snprintf(args, sizeof(args),
			"time_base=%d/%d:sample_rate=%d:sample_fmt=%s:channel_layout=0x%",
			dec_ctx->time_base.num, dec_ctx->time_base.den, dec_ctx->sample_rate,
			av_get_sample_fmt_name(dec_ctx->sample_fmt),
			dec_ctx->channel_layout);

		ret = avfilter_graph_create_filter(&buffersrc_ctx, buffersrc, "in", args, NULL, filter_graph);
		if (ret < 0) 
		{
			error = "Cannot create audio buffer source.";
			goto end;
		}

		ret = avfilter_graph_create_filter(&buffersink_ctx, buffersink, "out", NULL, NULL, filter_graph);
		if (ret < 0) 
		{
			error = "Cannot create audio buffer sink.";
			goto end;
		}

		ret = av_opt_set_bin(buffersink_ctx, "sample_fmts", (uint8_t*)&enc_ctx->sample_fmt, sizeof(enc_ctx->sample_fmt), AV_OPT_SEARCH_CHILDREN);
		if (ret < 0) 
		{
			error = "Cannot set output sample format.";
			goto end;
		}

		ret = av_opt_set_bin(buffersink_ctx, "channel_layouts", (uint8_t*)&enc_ctx->channel_layout, sizeof(enc_ctx->channel_layout), AV_OPT_SEARCH_CHILDREN);
		if (ret < 0) 
		{
			error = "Cannot set output channel layout.";
			goto end;
		}

		ret = av_opt_set_bin(buffersink_ctx, "sample_rates", (uint8_t*)&enc_ctx->sample_rate, sizeof(enc_ctx->sample_rate), AV_OPT_SEARCH_CHILDREN);
		if (ret < 0) 
		{
			error = "Cannot set output sample rate.";
			goto end;
		}
	}
	else {
		ret = AVERROR_UNKNOWN;
		goto end;
	}

	/* Endpoints for the filter graph. */
	outputs->name = libffmpeg::av_strdup("in");
	outputs->filter_ctx = buffersrc_ctx;
	outputs->pad_idx = 0;
	outputs->next = NULL;

	inputs->name = libffmpeg::av_strdup("out");
	inputs->filter_ctx = buffersink_ctx;
	inputs->pad_idx = 0;
	inputs->next = NULL;

	if (!outputs->name || !inputs->name) {
		ret = AVERROR(ENOMEM);
		goto end;
	}

	if ((ret = avfilter_graph_parse_ptr(filter_graph, filter_spec,
		&inputs, &outputs, NULL)) < 0)
		goto end;

	if ((ret = avfilter_graph_config(filter_graph, NULL)) < 0)
		goto end;

	/* Fill FilteringContext */
	fctx->buffersrc_ctx = buffersrc_ctx;
	fctx->buffersink_ctx = buffersink_ctx;
	fctx->filter_graph = filter_graph;

end:
	avfilter_inout_free(&inputs);
	avfilter_inout_free(&outputs);

	// If an error has occured.
	if (error)
	{
		// Throw the error.
		System::String^ errorMessage = gcnew System::String(error);
		throw gcnew MediaConverterException(errorMessage);
	}

	// Return the result.
	return ret;
}

/// <summary>
/// Encode frame.
/// </summary>
/// <param name="data">Media data.</param>
/// <param name="filt_frame">Filter frame.</param>
/// <param name="stream_index">Stream index.</param>
/// <param name="got_frame">Has frame.</param>
/// <return>The return value.</return>
int encode_write_frame(MediaConverterData^ data, libffmpeg::AVFrame *filt_frame, unsigned int stream_index, int *got_frame)
{
	int ret;
	int got_frame_local;

	libffmpeg::AVPacket enc_pkt;
	libffmpeg::AVFormatContext* formatContextIn = data->FormatContextIn;

	// Set the encoding function handler.
	enc_func encodingFuncHandler = (formatContextIn->streams[stream_index]->codec->codec_type == libffmpeg::AVMEDIA_TYPE_VIDEO) ? 
		libffmpeg::avcodec_encode_video2 : libffmpeg::avcodec_encode_audio2;

	if (!got_frame)
		got_frame = &got_frame_local;

	/* encode filtered frame */
	enc_pkt.data = NULL;
	enc_pkt.size = 0;
	av_init_packet(&enc_pkt);
	ret = encodingFuncHandler(data->FormatContextOut->streams[stream_index]->codec, &enc_pkt,
		filt_frame, got_frame);

	av_frame_free(&filt_frame);
	if (ret < 0)
		return ret;
	if (!(*got_frame))
		return 0;

	/* prepare packet for muxing */
	enc_pkt.stream_index = stream_index;
	av_packet_rescale_ts(&enc_pkt,
		data->FormatContextOut->streams[stream_index]->codec->time_base,
		data->FormatContextOut->streams[stream_index]->time_base);

	/* mux encoded frame */
	ret = libffmpeg::av_interleaved_write_frame(data->FormatContextOut, &enc_pkt);
	return ret;
}

/// <summary>
/// Encode filter frame.
/// </summary>
/// <param name="data">Media data.</param>
/// <param name="filt_frame">Filter frame.</param>
/// <param name="stream_index">Stream index.</param>
/// <return>The return value.</return>
int filter_encode_write_frame(MediaConverterData^ data, libffmpeg::AVFrame *frame, unsigned int stream_index)
{
	int ret;
	libffmpeg::AVFrame *filt_frame;

	/* push the decoded frame into the filtergraph */
	ret = av_buffersrc_add_frame_flags(data->NativeData[stream_index].buffersrc_ctx, frame, 0);
	if (ret < 0) 
	{
		throw gcnew MediaConverterException("Error while feeding the filtergraph.");
		return ret;
	}

	/* pull filtered frames from the filtergraph */
	while (1) 
	{
		filt_frame = libffmpeg::av_frame_alloc();
		if (!filt_frame) 
		{
			ret = AVERROR(ENOMEM);
			break;
		}
		
		ret = libffmpeg::av_buffersink_get_frame(data->NativeData[stream_index].buffersink_ctx, filt_frame);
		if (ret < 0) 
		{
			/* if no more frames for output - returns AVERROR(EAGAIN)
			* if flushed and no more frames for output - returns AVERROR_EOF
			* rewrite retcode to 0 to show it as normal procedure completion
			*/
			if (ret == AVERROR(EAGAIN) || ret == AVERROR_EOF)
				ret = 0;

			libffmpeg::av_frame_free(&filt_frame);
			break;
		}

		filt_frame->pict_type = libffmpeg::AV_PICTURE_TYPE_NONE;
		ret = encode_write_frame(data, filt_frame, stream_index, NULL);
		if (ret < 0)
			break;
	}

	return ret;
}

/// <summary>
/// Flush encoder.
/// </summary>
/// <param name="data">Media data.</param>
/// <param name="stream_index">Stream index.</param>
/// <return>The return value.</return>
int flush_encoder(MediaConverterData^ data, unsigned int stream_index)
{
	int ret;
	int got_frame;

	if (!(data->FormatContextOut->streams[stream_index]->codec->codec->capabilities & AV_CODEC_CAP_DELAY))
		return 0;

	while (1) 
	{
		ret = encode_write_frame(data, NULL, stream_index, &got_frame);

		if (ret < 0)
			break;
		if (!got_frame)
			return 0;
	}
	return ret;
}