/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          VideoFileWriter.cpp
*  Purpose :       VideoFileWriter class.
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

#include "VideoFileWriter.h"

using namespace Nequeo::Media::FFmpeg;


/// <summary>
/// Write the video frame.
/// </summary>
/// <param name="data">Video write data.</param>
static void write_video_frame(WriterPrivateData^ data);

/// <summary>
/// Open the video frame.
/// </summary>
/// <param name="data">Video write data.</param>
static void open_video(WriterPrivateData^ data);

/// <summary>
/// Add video stream.
/// </summary>
/// <param name="data">Video write data.</param>
/// <param name="width">Video width.</param>
/// <param name="height">Video height.</param>
/// <param name="frameRate">Video frame rate.</param>
/// <param name="bitRate">Video bit rate.</param>
/// <param name="codecId">Video codec id.</param>
/// <param name="pixelFormat">Video pixel format.</param>
static void add_video_stream(WriterPrivateData^ data, int width, int height, int frameRate, int bitRate,
							  enum libffmpeg::AVCodecID codecId, enum libffmpeg::AVPixelFormat pixelFormat);



/// <summary>
/// Initializes a new instance of the <see cref="VideoFileWriter"/> class.
/// </summary>
VideoFileWriter::VideoFileWriter() :
    data( nullptr ), _disposed( false )
{
	libffmpeg::av_register_all( );
}

/// <summary>
/// Create video file with the specified name and attributes.
/// </summary>
///
/// <param name="fileName">Video file name to create.</param>
/// <param name="width">Frame width of the video file.</param>
/// <param name="height">Frame height of the video file.</param>
///
/// <remarks><para>See documentation to the <see cref="Open( String^, int, int, int, VideoCodec )" />
/// for more information and the list of possible exceptions.</para>
///
/// <para><note>The method opens the video file using <see cref="VideoCodec::Default" />
/// codec and 25 fps frame rate.</note></para>
/// </remarks>
void VideoFileWriter::Open( String^ fileName, int width, int height )
{
	Open( fileName, width, height, 25 );
}

/// <summary>
/// Create video file with the specified name and attributes.
/// </summary>
///
/// <param name="fileName">Video file name to create.</param>
/// <param name="width">Frame width of the video file.</param>
/// <param name="height">Frame height of the video file.</param>
/// <param name="frameRate">Frame rate of the video file.</param>
///
/// <remarks><para>See documentation to the <see cref="Open( String^, int, int, int, VideoCodec )" />
/// for more information and the list of possible exceptions.</para>
///
/// <para><note>The method opens the video file using <see cref="VideoCodec::Default" />
/// codec.</note></para>
/// </remarks>
void VideoFileWriter::Open( String^ fileName, int width, int height, int frameRate )
{
	Open( fileName, width, height, frameRate, VideoCodec::Default );
}

/// <summary>
/// Create video file with the specified name and attributes.
/// </summary>
///
/// <param name="fileName">Video file name to create.</param>
/// <param name="width">Frame width of the video file.</param>
/// <param name="height">Frame height of the video file.</param>
/// <param name="frameRate">Frame rate of the video file.</param>
/// <param name="codec">Video codec to use for compression.</param>
///
/// <remarks><para>The methods creates new video file with the specified name.
/// If a file with such name already exists in the file system, it will be overwritten.</para>
///
/// <para>When adding new video frames using <see cref="WriteVideoFrame(Bitmap^ frame)"/> method,
/// the video frame must have width and height as specified during file opening.</para>
/// </remarks>
///
/// <exception cref="ArgumentException">Video file resolution must be a multiple of two.</exception>
/// <exception cref="ArgumentException">Invalid video codec is specified.</exception>
/// <exception cref="VideoException">A error occurred while creating new video file. See exception message.</exception>
/// <exception cref="System::IO::IOException">Cannot open video file with the specified name.</exception>
void VideoFileWriter::Open( String^ fileName, int width, int height, int frameRate, VideoCodec codec )
{
	Open( fileName, width, height, frameRate, codec, 400000 );
}

/// <summary>
/// Create video file with the specified name and attributes.
/// </summary>
///
/// <param name="fileName">Video file name to create.</param>
/// <param name="width">Frame width of the video file.</param>
/// <param name="height">Frame height of the video file.</param>
/// <param name="frameRate">Frame rate of the video file.</param>
/// <param name="codec">Video codec to use for compression.</param>
/// <param name="bitRate">Bit rate of the video stream.</param>
///
/// <remarks><para>The methods creates new video file with the specified name.
/// If a file with such name already exists in the file system, it will be overwritten.</para>
///
/// <para>When adding new video frames using <see cref="WriteVideoFrame(Bitmap^ frame)"/> method,
/// the video frame must have width and height as specified during file opening.</para>
///
/// <para><note>The bit rate parameter represents a trade-off value between video quality
/// and video file size. Higher bit rate value increase video quality and result in larger
/// file size. Smaller values result in opposite – worse quality and small video files.</note></para>
/// </remarks>
///
/// <exception cref="ArgumentException">Video file resolution must be a multiple of two.</exception>
/// <exception cref="ArgumentException">Invalid video codec is specified.</exception>
/// <exception cref="VideoException">A error occurred while creating new video file. See exception message.</exception>
/// <exception cref="System::IO::IOException">Cannot open video file with the specified name.</exception>
void VideoFileWriter::Open( String^ fileName, int width, int height, int frameRate, VideoCodec codec, int bitRate )
{
	// Check if disposed.
    CheckIfDisposed( );

	// close previous file if any open
	Close( );

	// Create a new write data.
	data = gcnew WriterPrivateData( );
	bool success = false;

	// check width and height
	if ( ( ( width & 1 ) != 0 ) || ( ( height & 1 ) != 0 ) )
	{
		throw gcnew ArgumentException( "Video file resolution must be a multiple of two." );
	}

	// check video codec
	if ( ( (int) codec < -1 ) || ( (int) codec >= CODECS_COUNT ) )
	{
		throw gcnew ArgumentException( "Invalid video codec is specified." );
	}

	m_width  = width;
	m_height = height;
	m_codec  = codec;
	m_frameRate = frameRate;
	m_bitRate = bitRate;
	
	// convert specified managed String to unmanaged string
	IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalUni( fileName );
    wchar_t* nativeFileNameUnicode = (wchar_t*) ptr.ToPointer( );
    int utf8StringSize = WideCharToMultiByte( CP_UTF8, 0, nativeFileNameUnicode, -1, NULL, 0, NULL, NULL );
    char* nativeFileName = new char[utf8StringSize];
    WideCharToMultiByte( CP_UTF8, 0, nativeFileNameUnicode, -1, nativeFileName, utf8StringSize, NULL, NULL );

	try
	{
		// guess about destination file format from its file name
		libffmpeg::AVOutputFormat* outputFormat = libffmpeg::av_guess_format( NULL, nativeFileName, NULL );

		if ( !outputFormat )
		{
			// gues about destination file format from its short name
			outputFormat = libffmpeg::av_guess_format( "mpeg", NULL, NULL );

			if ( !outputFormat )
			{
				throw gcnew VideoException( "Cannot find suitable output format." );
			}
		}

		// prepare format context
		data->FormatContext = libffmpeg::avformat_alloc_context( );

		if ( !data->FormatContext )
		{
			throw gcnew VideoException( "Cannot allocate format context." );
		}
		data->FormatContext->oformat = outputFormat;

		// add video stream using the specified video codec
		add_video_stream( data, width, height, frameRate, bitRate,
			( codec == VideoCodec::Default ) ? outputFormat->video_codec : (libffmpeg::AVCodecID) video_codecs[(int) codec],
			( codec == VideoCodec::Default ) ? libffmpeg::AV_PIX_FMT_YUV420P : (libffmpeg::AVPixelFormat) pixel_formats[(int) codec] );

		// Open the video data.
		open_video( data );

		// open output file
		if ( !( outputFormat->flags & AVFMT_NOFILE ) )
		{
			if ( libffmpeg::avio_open( &data->FormatContext->pb, nativeFileName, AVIO_FLAG_WRITE ) < 0 )
			{
				throw gcnew System::IO::IOException( "Cannot open the video file." );
			}
		}

		// Write the header data.
		libffmpeg::avformat_write_header( data->FormatContext, NULL );

		success = true;
	}
	finally
	{
		System::Runtime::InteropServices::Marshal::FreeHGlobal( ptr );
        delete [] nativeFileName;

		if ( !success )
		{
			// Close the file.
			Close( );
		}
	}
}

/// <summary>
/// Close currently opened video file if any.
/// </summary>
void VideoFileWriter::Close( )
{
	// if data exists.
	if ( data != nullptr )
	{
		// If context exists.
		if ( data->FormatContext )
		{
			if ( data->FormatContext->pb != NULL )
			{
				// Write last bit of data.
				libffmpeg::av_write_trailer( data->FormatContext );
			}

			if ( data->VideoStream )
			{
				// Close the video codec.
				libffmpeg::avcodec_close( data->VideoStream->codec );
			}

			if ( data->VideoFrame )
			{
				// Free the frame data.
				libffmpeg::av_free( data->VideoFrame->data[0] );
				libffmpeg::av_free( data->VideoFrame );
			}

			if ( data->VideoOutputBuffer )
			{
				// Free the output buffer.
				libffmpeg::av_free( data->VideoOutputBuffer );
			}

			for ( unsigned int i = 0; i < data->FormatContext->nb_streams; i++ )
			{
				// Free the stream data, using the stream pointer.
				libffmpeg::av_freep( &data->FormatContext->streams[i]->codec );
				libffmpeg::av_freep( &data->FormatContext->streams[i] );
			}

			if ( data->FormatContext->pb != NULL )
			{
				// Close the stream.
				libffmpeg::avio_close( data->FormatContext->pb );
			}
			
			// Free the context.
			libffmpeg::av_free( data->FormatContext );
		}

		if ( data->ConvertContext != NULL )
		{
			// Free the converter.
			libffmpeg::sws_freeContext( data->ConvertContext );
		}

		if ( data->ConvertContextGrayscale != NULL )
		{
			// Free the grayscale converter.
			libffmpeg::sws_freeContext( data->ConvertContextGrayscale );
		}

		// Set to null.
		data = nullptr;
	}

	m_width  = 0;
	m_height = 0;
}

/// <summary>
/// Write new video frame into currently opened video file.
/// </summary>
///
/// <param name="frame">Bitmap to add as a new video frame.</param>
///
/// <remarks><para>The specified bitmap must be either color 24 or 32 bpp image or grayscale 8 bpp (indexed) image.</para>
/// </remarks>
///
/// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
/// <exception cref="ArgumentException">The provided bitmap must be 24 or 32 bpp color image or 8 bpp grayscale image.</exception>
/// <exception cref="ArgumentException">Bitmap size must be of the same as video size, which was specified on opening video file.</exception>
/// <exception cref="VideoException">A error occurred while writing new video frame. See exception message.</exception>
void VideoFileWriter::WriteVideoFrame( Bitmap^ frame )
{
	WriteVideoFrame( frame, TimeSpan::MinValue );
}

/// <summary>
/// Write new video frame with a specific timestamp into currently opened video file.
/// </summary>
///
/// <param name="frame">Bitmap to add as a new video frame.</param>
/// <param name="timestamp">Frame timestamp, total time since recording started.</param>
///
/// <remarks><para>The specified bitmap must be either color 24 or 32 bpp image or grayscale 8 bpp (indexed) image.</para>
/// 
/// <para><note>The <paramref name="timestamp"/> parameter allows user to specify presentation
/// time of the frame being saved. However, it is user's responsibility to make sure the value is increasing
/// over time.</note></para>
/// </remarks>
///
/// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
/// <exception cref="ArgumentException">The provided bitmap must be 24 or 32 bpp color image or 8 bpp grayscale image.</exception>
/// <exception cref="ArgumentException">Bitmap size must be of the same as video size, which was specified on opening video file.</exception>
/// <exception cref="VideoException">A error occurred while writing new video frame. See exception message.</exception>
void VideoFileWriter::WriteVideoFrame( Bitmap^ frame, TimeSpan timestamp )
{
    CheckIfDisposed( );

	if ( data == nullptr )
	{
		throw gcnew System::IO::IOException( "A video file was not opened yet." );
	}

	if ( ( frame->PixelFormat != PixelFormat::Format24bppRgb ) &&
	     ( frame->PixelFormat != PixelFormat::Format32bppArgb ) &&
		 ( frame->PixelFormat != PixelFormat::Format32bppPArgb ) &&
	 	 ( frame->PixelFormat != PixelFormat::Format32bppRgb ) &&
		 ( frame->PixelFormat != PixelFormat::Format8bppIndexed ) )
	{
		throw gcnew ArgumentException( "The provided bitmap must be 24 or 32 bpp color image or 8 bpp grayscale image." );
	}

	if ( ( frame->Width != m_width ) || ( frame->Height != m_height ) )
	{
		throw gcnew ArgumentException( "Bitmap size must be of the same as video size, which was specified on opening video file." );
	}

	// lock the bitmap to allocate memory.
	BitmapData^ bitmapData = frame->LockBits( System::Drawing::Rectangle( 0, 0, m_width, m_height ),
		ImageLockMode::ReadOnly, ( frame->PixelFormat == PixelFormat::Format8bppIndexed ) ? PixelFormat::Format8bppIndexed : PixelFormat::Format24bppRgb );

	// Get the pointer to the first scan pixel
	libffmpeg::uint8_t* ptr = reinterpret_cast<libffmpeg::uint8_t*>( static_cast<void*>( bitmapData->Scan0 ) );

	// Get the stride (scan) pointer to the first pixel
	libffmpeg::uint8_t* srcData[4] = { ptr, NULL, NULL, NULL };
	int srcLinesize[4] = { bitmapData->Stride, 0, 0, 0 };

	// convert source image to the format of the video file, write the image to the bitmap data memory block.
	if ( frame->PixelFormat == PixelFormat::Format8bppIndexed )
	{
		libffmpeg::sws_scale( data->ConvertContextGrayscale, srcData, srcLinesize, 0, m_height, data->VideoFrame->data, data->VideoFrame->linesize );
	}
	else
	{
		libffmpeg::sws_scale( data->ConvertContext, srcData, srcLinesize, 0, m_height, data->VideoFrame->data, data->VideoFrame->linesize );
	}

	// Unlick the memory where the bitmap is stored.
	frame->UnlockBits( bitmapData );

	if ( timestamp.Ticks >= 0 )
	{
		const double frameNumber = timestamp.TotalSeconds * m_frameRate;
		data->VideoFrame->pts = static_cast<libffmpeg::int64_t>( frameNumber );
	}

	// write the converted frame to the video file
	write_video_frame( data );
}

/// <summary>
/// Write the video frame.
/// </summary>
/// <param name="data">Video write data.</param>
void write_video_frame(WriterPrivateData^ data)
{
	libffmpeg::AVCodecContext* codecContext = data->VideoStream->codec;
	int retEncode, ret = 0;

	if ( data->FormatContext->oformat->flags & AVFMT_RAWPICTURE )
	{
		Console::WriteLine( "raw picture must be written" );
	}
	else
	{
		// Create the packet.
		libffmpeg::AVPacket packet;
		libffmpeg::av_init_packet(&packet);
		packet.data = data->VideoOutputBuffer;
		packet.size = data->VideoOutputBufferSize;
		packet.stream_index = data->VideoStream->index;

		// Get the value.
		libffmpeg::int64_t _av_nopts_value = ((libffmpeg::int64_t)UINT64_C(0x8000000000000000));

		// If has no options.
		if (codecContext->coded_frame->pts != _av_nopts_value)
		{
			packet.pts = libffmpeg::av_rescale_q(codecContext->coded_frame->pts, codecContext->time_base, data->VideoStream->time_base);
		}

		// If the packet has a key frame.
		if (codecContext->coded_frame->key_frame)
		{
			packet.flags |= AV_PKT_FLAG_KEY;
		}

		int got_packet = 0;

		// encode the image
		retEncode = libffmpeg::avcodec_encode_video2(codecContext, &packet, data->VideoFrame, &got_packet);

		// if zero size, it means the image was buffered
		if (retEncode == 0  && got_packet == 1)
		{
			// write the compressed frame to the media file
			ret = libffmpeg::av_interleaved_write_frame( data->FormatContext, &packet );
		}
		else
		{
			// image was buffered
		}
	}

	// if the image was not written.
	if ( ret != 0 )
	{
		throw gcnew VideoException( "Error while writing video frame." );
	}
}

/// <summary>
/// Allocate picture of the specified format and size.
/// </summary>
/// <param name="data">The fram data.</param>
/// <param name="width">The frame width.</param>
/// <param name="height">The frame height.</param>
static libffmpeg::AVFrame* alloc_picture(enum libffmpeg::AVPixelFormat pix_fmt, int width, int height )
{
	libffmpeg::AVFrame* picture;
	void* picture_buf;
	int size;

	// Allocate frame memory.
	picture = libffmpeg::av_frame_alloc( );
	if ( !picture )
	{
		return NULL;
	}

	// Allocate the picture memory.
	size = libffmpeg::avpicture_get_size( pix_fmt, width, height );
	picture_buf = libffmpeg::av_malloc( size );
	if ( !picture_buf )
	{
		// Free if failed.
		libffmpeg::av_free( picture );
		return NULL;
	}

	// Write the picture data into the allocated memory.
	libffmpeg::avpicture_fill( (libffmpeg::AVPicture *) picture, (libffmpeg::uint8_t *) picture_buf, pix_fmt, width, height );

	// Return the picture.
	return picture;
}

/// <summary>
/// Add video stream.
/// </summary>
/// <param name="data">Video write data.</param>
/// <param name="width">Video width.</param>
/// <param name="height">Video height.</param>
/// <param name="frameRate">Video frame rate.</param>
/// <param name="bitRate">Video bit rate.</param>
/// <param name="codecId">Video codec id.</param>
/// <param name="pixelFormat">Video pixel format.</param>
void add_video_stream(WriterPrivateData^ data, int width, int height, int frameRate, int bitRate,
					  enum libffmpeg::AVCodecID codecId, enum libffmpeg::AVPixelFormat pixelFormat )
{
	libffmpeg::AVCodecContext* codecContex;

	// create new stream
	data->VideoStream = libffmpeg::avformat_new_stream( data->FormatContext, NULL );

	if ( !data->VideoStream )
	{
		throw gcnew VideoException( "Failed creating new video stream." );
	}

	data->VideoStream->id = 0;
	codecContex = data->VideoStream->codec;
	codecContex->codec_id   = codecId;
	codecContex->codec_type = libffmpeg::AVMEDIA_TYPE_VIDEO;

	// put sample parameters
	codecContex->bit_rate = bitRate;
	codecContex->width    = width;
	codecContex->height   = height;

	// time base: this is the fundamental unit of time (in seconds) in terms
	// of which frame timestamps are represented. for fixed-fps content,
	// timebase should be 1/framerate and timestamp increments should be
	// identically 1.
	codecContex->time_base.den = frameRate;
	codecContex->time_base.num = 1;

	codecContex->gop_size = 12; // emit one intra frame every twelve frames at most
	codecContex->pix_fmt  = pixelFormat;

	if ( codecContex->codec_id == libffmpeg::AV_CODEC_ID_MPEG1VIDEO )
	{
		// Needed to avoid using macroblocks in which some coeffs overflow.
		// This does not happen with normal video, it just happens here as
		// the motion of the chroma plane does not match the luma plane.
		codecContex->mb_decision = 2;
	}

	// some formats want stream headers to be separate
	if( data->FormatContext->oformat->flags & AVFMT_GLOBALHEADER )
	{
		codecContex->flags |= CODEC_FLAG_GLOBAL_HEADER;
	}
}

/// <summary>
/// Open the video frame.
/// </summary>
/// <param name="data">Video write data.</param>
void open_video(WriterPrivateData^ data)
{
	libffmpeg::AVCodecContext* codecContext = data->VideoStream->codec;
	libffmpeg::AVCodec* codec = avcodec_find_encoder( codecContext->codec_id );

	if ( !codec )
	{
		throw gcnew VideoException( "Cannot find video codec." );
	}

	// open the codec 
	if ( avcodec_open2( codecContext, codec, NULL ) < 0 )
	{
		throw gcnew VideoException( "Cannot open video codec." );
	}

	data->VideoOutputBuffer = NULL;
	if ( !( data->FormatContext->oformat->flags & AVFMT_RAWPICTURE ) )
	{
         // allocate output buffer 
         data->VideoOutputBufferSize = 6 * codecContext->width * codecContext->height; // more than enough even for raw video
		 data->VideoOutputBuffer = (libffmpeg::uint8_t*) libffmpeg::av_malloc( data->VideoOutputBufferSize );
	}

	// allocate the encoded raw picture
	data->VideoFrame = alloc_picture( codecContext->pix_fmt, codecContext->width, codecContext->height );

	if ( !data->VideoFrame )
	{
		throw gcnew VideoException( "Cannot allocate video picture." );
	}

	// prepare scaling context to convert RGB image to video format
	data->ConvertContext = libffmpeg::sws_getContext( codecContext->width, codecContext->height, libffmpeg::AV_PIX_FMT_BGR24,
			codecContext->width, codecContext->height, codecContext->pix_fmt,
			SWS_BICUBIC, NULL, NULL, NULL );

	// prepare scaling context to convert grayscale image to video format
	data->ConvertContextGrayscale = libffmpeg::sws_getContext( codecContext->width, codecContext->height, libffmpeg::AV_PIX_FMT_GRAY8,
			codecContext->width, codecContext->height, codecContext->pix_fmt,
			SWS_BICUBIC, NULL, NULL, NULL );

	if ( ( data->ConvertContext == NULL ) || ( data->ConvertContextGrayscale == NULL ) )
	{
		throw gcnew VideoException( "Cannot initialize frames conversion context." );
	}
}

