/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          VideoFileReader.h
*  Purpose :       SIP VideoFileReader class.
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

#ifndef _VIDEOFILEREADER_H
#define _VIDEOFILEREADER_H

#include "stdafx.h"

using namespace System;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;

using namespace AForge::Video;

namespace Nequeo
{
	namespace Media
	{
		namespace FFmpeg
		{
			/// <summary>
			/// A structure to encapsulate all FFMPEG related private variable.
			/// </summary>
			public ref struct ReaderPrivateData
			{
			public:
				libffmpeg::AVFormatContext*		FormatContext;
				libffmpeg::AVStream*			VideoStream;
				libffmpeg::AVCodecContext*		CodecContext;
				libffmpeg::AVFrame*				VideoFrame;
				struct libffmpeg::SwsContext*	ConvertContext;

				libffmpeg::AVPacket* Packet;
				int BytesRemaining;

				ReaderPrivateData()
				{
					FormatContext = NULL;
					VideoStream = NULL;
					CodecContext = NULL;
					VideoFrame = NULL;
					ConvertContext = NULL;

					Packet = NULL;
					BytesRemaining = 0;
				}
			};

			/// <summary>
			/// Class for reading video files utilizing FFmpeg library.
			/// </summary>
			/// 
			/// <remarks><para>The class allows to read video files using <a href="http://www.ffmpeg.org/">FFmpeg</a> library.</para>
			/// 
			/// <para><note>Make sure you have <b>FFmpeg</b> binaries (DLLs) in the output folder of your application in order
			/// to use this class successfully. <b>FFmpeg</b> binaries can be found in Externals folder provided with AForge.NET
			/// framework's distribution.</note></para>
			///
			/// <para>Sample usage:</para>
			/// <code>
			/// // create instance of video reader
			/// VideoFileReader reader = new VideoFileReader( );
			/// // open video file
			/// reader.Open( "test.avi" );
			/// // check some of its attributes
			/// Console.WriteLine( "width:  " + reader.Width );
			/// Console.WriteLine( "height: " + reader.Height );
			/// Console.WriteLine( "fps:    " + reader.FrameRate );
			/// Console.WriteLine( "codec:  " + reader.CodecName );
			/// // read 100 video frames out of it
			/// for ( int i = 0; i &lt; 100; i++ )
			/// {
			///     Bitmap videoFrame = reader.ReadVideoFrame( );
			///     // process the frame somehow
			///     // ...
			/// 
			///     // dispose the frame when it is no longer required
			///     videoFrame.Dispose( );
			/// }
			/// reader.Close( );
			/// </code>
			/// </remarks>
			///
			public ref class VideoFileReader : IDisposable
			{
			public:

				/// <summary>
				/// Frame width of the opened video file.
				/// </summary>
				///
				/// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
				///
				property int Width
				{
					int get()
					{
						CheckIfVideoFileIsOpen();
						return m_width;
					}
				}

				/// <summary>
				/// Frame height of the opened video file.
				/// </summary>
				///
				/// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
				///
				property int Height
				{
					int get()
					{
						CheckIfVideoFileIsOpen();
						return m_height;
					}
				}

				/// <summary>
				/// Frame rate of the opened video file.
				/// </summary>
				///
				/// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
				///
				property int FrameRate
				{
					int get()
					{
						CheckIfVideoFileIsOpen();
						return m_frameRate;
					}
				}

				/// <summary>
				/// Number of video frames in the opened video file.
				/// </summary>
				///
				/// <remarks><para><note><b>Warning</b>: some video file formats may report different value
				/// from the actual number of video frames in the file (subject to fix/investigate).</note></para>
				/// </remarks>
				///
				/// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
				///
				property Int64 FrameCount
				{
					Int64 get()
					{
						CheckIfVideoFileIsOpen();
						return m_framesCount;
					}
				}

				/// <summary>
				/// Name of codec used for encoding the opened video file.
				/// </summary>
				///
				/// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
				///
				property String^ CodecName
				{
					String^ get()
					{
						CheckIfVideoFileIsOpen();
						return m_codecName;
					}
				}

				/// <summary>
				/// The property specifies if a video file is opened or not by this instance of the class.
				/// </summary>
				property bool IsOpen
				{
					bool get()
					{
						return (data != nullptr);
					}
				}

			protected:

				/// <summary>
				/// Object's finalizer.
				/// </summary>
				/// 
				!VideoFileReader()
				{
					Close();
				}

			public:

				/// <summary>
				/// Initializes a new instance of the <see cref="VideoFileReader"/> class.
				/// </summary>
				/// 
				VideoFileReader(void);

				/// <summary>
				/// Disposes the object and frees its resources.
				/// </summary>
				/// 
				~VideoFileReader()
				{
					this->!VideoFileReader();
					disposed = true;
				}

				/// <summary>
				/// Open video file with the specified name.
				/// </summary>
				///
				/// <param name="fileName">Video file name to open.</param>
				///
				/// <exception cref="System::IO::IOException">Cannot open video file with the specified name.</exception>
				/// <exception cref="VideoException">A error occurred while opening the video file. See exception message.</exception>
				///
				void Open(String^ fileName);

				/// <summary>
				/// Read next video frame of the currently opened video file.
				/// </summary>
				/// 
				/// <returns>Returns next video frame of the opened file or <see langword="null"/> if end of
				/// file was reached. The returned video frame has 24 bpp color format.</returns>
				/// 
				/// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
				/// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
				/// 
				Bitmap^ ReadVideoFrame();

				/// <summary>
				/// Close currently opened video file if any.
				/// </summary>
				/// 
				void Close();

			private:

				int m_width;
				int m_height;
				int	m_frameRate;
				String^ m_codecName;
				Int64 m_framesCount;

			private:
				Bitmap^ DecodeVideoFrame();

				// Checks if video file was opened
				void CheckIfVideoFileIsOpen()
				{
					if (data == nullptr)
					{
						throw gcnew System::IO::IOException("Video file is not open, so can not access its properties.");
					}
				}

				// Check if the object was already disposed
				void CheckIfDisposed()
				{
					if (disposed)
					{
						throw gcnew System::ObjectDisposedException("The object was already disposed.");
					}
				}

			private:
				// private data of the class
				ReaderPrivateData^ data;
				bool disposed;
			};

		}
	}
}
#endif