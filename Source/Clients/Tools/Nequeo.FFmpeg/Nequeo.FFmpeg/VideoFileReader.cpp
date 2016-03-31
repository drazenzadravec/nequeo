/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          VideoFileReader.cpp
*  Purpose :       VideoFileReader class.
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

#include "VideoFileReader.h"

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
/// Initializes a new instance of the <see cref="VideoFileReader"/> class.
/// </summary>
VideoFileReader::VideoFileReader() :
    data( nullptr ), _disposed( false )
{	
	libffmpeg::av_register_all( );
}

/// <summary>
/// Open video file with the specified name.
/// </summary>
/// <param name="fileName">Video file name to open.</param>
/// <exception cref="System::IO::IOException">Cannot open video file with the specified name.</exception>
/// <exception cref="VideoException">A error occurred while opening the video file. See exception message.</exception>
void VideoFileReader::Open(String^ fileName )
{
	// Check if disposed.
    CheckIfDisposed( );

	// close previous file if any was open
	Close( );

	// Create the reader data packet.
	data = gcnew ReaderVideoPrivateData();
	data->Packet = new libffmpeg::AVPacket( );
	data->Packet->data = NULL;

	bool success = false;

	// convert specified managed String to UTF8 unmanaged string
	IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalUni( fileName );
    wchar_t* nativeFileNameUnicode = (wchar_t*) ptr.ToPointer( );
    int utf8StringSize = WideCharToMultiByte( CP_UTF8, 0, nativeFileNameUnicode, -1, NULL, 0, NULL, NULL );
    char* nativeFileName = new char[utf8StringSize];
    WideCharToMultiByte( CP_UTF8, 0, nativeFileNameUnicode, -1, nativeFileName, utf8StringSize, NULL, NULL );

	try
	{
		// open the specified video file
		data->FormatContext = open_file( nativeFileName );
		if ( data->FormatContext == NULL )
		{
			throw gcnew System::IO::IOException( "Cannot open the video file." );
		}

		// retrieve stream information
		if ( libffmpeg::avformat_find_stream_info( data->FormatContext, NULL ) < 0 )
		{
			throw gcnew VideoException( "Cannot find stream information." );
		}

		// search for the first video stream
		for ( unsigned int i = 0; i < data->FormatContext->nb_streams; i++ )
		{
			if( data->FormatContext->streams[i]->codec->codec_type == libffmpeg::AVMEDIA_TYPE_VIDEO )
			{
				// get the pointer to the codec context for the video stream
				data->CodecContext = data->FormatContext->streams[i]->codec;
				data->VideoStream  = data->FormatContext->streams[i];
				break;
			}
		}
		if ( data->VideoStream == NULL )
		{
			throw gcnew VideoException( "Cannot find video stream in the specified file." );
		}

		// find decoder for the video stream
		libffmpeg::AVCodec* codec = libffmpeg::avcodec_find_decoder( data->CodecContext->codec_id );
		if ( codec == NULL )
		{
			throw gcnew VideoException( "Cannot find codec to decode the video stream." );
		}

		// open the codec
		if ( libffmpeg::avcodec_open2( data->CodecContext, codec, NULL ) < 0 )
		{
			throw gcnew VideoException( "Cannot open video codec." );
		}

		// allocate video frame
		data->VideoFrame = libffmpeg::av_frame_alloc( );
		if (data->VideoFrame == NULL)
		{
			throw gcnew VideoException("Cannot initialize video frames.");
		}

		// prepare scaling context to convert RGB image to video format
		data->ConvertContext = libffmpeg::sws_getContext( data->CodecContext->width, data->CodecContext->height, data->CodecContext->pix_fmt,
				data->CodecContext->width, data->CodecContext->height, libffmpeg::AV_PIX_FMT_BGR24,
				SWS_BICUBIC, NULL, NULL, NULL );

		if ( data->ConvertContext == NULL )
		{
			throw gcnew VideoException( "Cannot initialize frames conversion context." );
		}

		// get some properties of the video file
		m_width  = data->CodecContext->width;
		m_height = data->CodecContext->height;
		m_frameRate = data->VideoStream->r_frame_rate.num / data->VideoStream->r_frame_rate.den;
		m_codecName = gcnew String( data->CodecContext->codec->name );
		m_framesCount = data->VideoStream->nb_frames;

		success = true;
	}
	finally
	{
		System::Runtime::InteropServices::Marshal::FreeHGlobal( ptr );
        delete [] nativeFileName;

		if ( !success )
		{
			// Close the stream.
			Close( );
		}
	}
}

/// <summary>
/// Close currently opened video file if any.
/// </summary>
void VideoFileReader::Close(  )
{
	// If data is not null.
	if ( data != nullptr )
	{
		if ( data->VideoFrame != NULL )
		{
			// Free video frame.
			libffmpeg::av_free( data->VideoFrame );
		}

		if (data->VideoStream)
		{
			// Close the video codec.
			libffmpeg::avcodec_close(data->VideoStream->codec);
		}

		if ( data->CodecContext != NULL )
		{
			// Close the codec context.
			libffmpeg::avcodec_close( data->CodecContext );
		}

		if ( data->FormatContext != NULL )
		{
			// Close the format context.
			libffmpeg::AVFormatContext* avFormatContext = data->FormatContext;
			libffmpeg::avformat_close_input(&avFormatContext);
		}

		if ( data->ConvertContext != NULL )
		{
			// Free the convert context.
			libffmpeg::sws_freeContext( data->ConvertContext );
		}

		if ( data->Packet->data != NULL )
		{
			// Free the packet.
			libffmpeg::av_free_packet( data->Packet );
		}

		data = nullptr;
	}
}

/// <summary>
/// Read next video frame of the currently opened video file.
/// </summary>
/// <returns>Returns next video frame of the opened file or <see langword="null"/> if end of
/// file was reached. The returned video frame has 24 bpp color format.</returns>
/// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
/// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
Bitmap^ VideoFileReader::ReadVideoFrame(  )
{
	// Check if disposed.
    CheckIfDisposed( );

	// Make sure data exists.
	if ( data == nullptr )
	{
		throw gcnew System::IO::IOException( "Cannot read video frames since video file is not open." );
	}

	int frameFinished;
	Bitmap^ bitmap = nullptr;

	int bytesDecoded;
	bool exit = false;

	// keep reading frames.
	while ( true )
	{
		// work on the current packet until we have decoded all of it
		while ( data->BytesRemaining > 0 )
		{
			// decode the next chunk of data
			bytesDecoded = libffmpeg::avcodec_decode_video2( data->CodecContext, data->VideoFrame, &frameFinished, data->Packet );

			// was there an error?
			if ( bytesDecoded < 0 )
			{
				throw gcnew VideoException( "Error while decoding frame." );
			}

			// Count down bytes remaing.
			data->BytesRemaining -= bytesDecoded;
					 
			// did we finish the current frame? Then we can return
			if ( frameFinished )
			{
				// Return the frame.
				return DecodeVideoFrame( );
			}
		}

		// read the next packet, skipping all packets that aren't
		// for this stream
		do
		{
			// free old packet if any
			if ( data->Packet->data != NULL )
			{
				libffmpeg::av_free_packet( data->Packet );
				data->Packet->data = NULL;
			}

			// read new packet
			if ( libffmpeg::av_read_frame( data->FormatContext, data->Packet ) < 0)
			{
				exit = true;
				break;
			}
		}
		while ( data->Packet->stream_index != data->VideoStream->index );

		// exit ?
		if ( exit )
			break;

		// Get number of pagket remaining.
		data->BytesRemaining = data->Packet->size;
	}

	// decode the rest of the last frame
	bytesDecoded = libffmpeg::avcodec_decode_video2(
		data->CodecContext, data->VideoFrame, &frameFinished, data->Packet );

	// free last packet
	if ( data->Packet->data != NULL )
	{
		libffmpeg::av_free_packet( data->Packet );
		data->Packet->data = NULL;
	}

	// is there a frame
	if ( frameFinished )
	{
		// Decode the frame into a bitmap.
		bitmap = DecodeVideoFrame( );
	}

	// Return the frame image.
	return bitmap;
}

/// <summary>
/// Decodes video frame into managed Bitmap.
/// </summary>
Bitmap^ VideoFileReader::DecodeVideoFrame( )
{
	// Create a new bitmap.
	Bitmap^ bitmap = gcnew Bitmap( data->CodecContext->width, data->CodecContext->height, PixelFormat::Format24bppRgb );
	
	// lock the bitmap size into memory, allocate space.
	BitmapData^ bitmapData = bitmap->LockBits( System::Drawing::Rectangle( 0, 0, data->CodecContext->width, data->CodecContext->height ),
		ImageLockMode::ReadOnly, PixelFormat::Format24bppRgb );

	// Get the memory address of the first bitmap pixel
	uint8_t* ptr = reinterpret_cast<uint8_t*>( static_cast<void*>( bitmapData->Scan0 ) );

	// Assign the pointer of the first pixel.
	uint8_t* srcData[4] = { ptr, NULL, NULL, NULL };

	// Get the stride width (scan width).
	int srcLinesize[4] = { bitmapData->Stride, 0, 0, 0 };

	// Convert video frame to the RGB bitmap.
	// Write the video fram image to the bitmap memory location set above.
	libffmpeg::sws_scale( data->ConvertContext, data->VideoFrame->data, data->VideoFrame->linesize, 0,
		data->CodecContext->height, srcData, srcLinesize );

	// Unlock the bitmap from memory.
	bitmap->UnlockBits( bitmapData );

	// Return the bitmap.
	return bitmap;
}
