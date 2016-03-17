/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          VideoFileSource.cpp
*  Purpose :       SIP VideoFileSource class.
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

#include "VideoFileSource.h"
#include "VideoFileReader.h"

using namespace Nequeo::Media::FFmpeg;

VideoFileSource::VideoFileSource( String^ fileName )
{
	m_fileName = fileName;
	m_workerThread = nullptr;
	m_needToStop = nullptr;

	m_frameIntervalFromSource = true;
	m_frameInterval = 0;
}

void VideoFileSource::Start( )
{
	if ( !IsRunning )
	{
        // check source
		if ( String::IsNullOrEmpty( m_fileName ) )
		{
            throw gcnew ArgumentException( "Video file name is not specified." );
		}

		m_framesReceived = 0;
        m_bytesReceived = 0;

		// create events
		m_needToStop = gcnew ManualResetEvent( false );
		
		// create and start new thread
		m_workerThread = gcnew Thread( gcnew ThreadStart( this, &VideoFileSource::WorkerThreadHandler ) );
		m_workerThread->Name = m_fileName; // just for debugging
		m_workerThread->Start( );
	}
}

void VideoFileSource::SignalToStop( )
{
	if ( m_workerThread != nullptr )
	{
		// signal to stop
		m_needToStop->Set( );
	}
}

void VideoFileSource::WaitForStop( )
{
	if ( m_workerThread != nullptr )
	{
		// wait for thread stop
		m_workerThread->Join( );

		Free( );
	}
}

void VideoFileSource::Stop( )
{
	if ( IsRunning )
	{
		m_workerThread->Abort( );
		WaitForStop( );
	}
}

void VideoFileSource::Free( )
{
	m_workerThread = nullptr;

	// release events
	m_needToStop->Close( );
	m_needToStop = nullptr;
}

void VideoFileSource::WorkerThreadHandler( )
{
	ReasonToFinishPlaying reasonToStop = ReasonToFinishPlaying::StoppedByUser;
	VideoFileReader^ videoReader = gcnew VideoFileReader( );

	try
	{
		videoReader->Open( m_fileName );

        // frame interval
        int interval = ( m_frameIntervalFromSource ) ?
			(int) ( 1000 / ( ( videoReader->FrameRate == 0 ) ? 25 : videoReader->FrameRate ) ) :
			m_frameInterval;

        while ( !m_needToStop->WaitOne( 0, false ) )
		{
			// start time
			DateTime start = DateTime::Now;

			// get next video frame
			Bitmap^ bitmap = videoReader->ReadVideoFrame( );

			if ( bitmap == nullptr )
			{
				reasonToStop = ReasonToFinishPlaying::EndOfStreamReached;
                break;
			}

			m_framesReceived++;
            m_bytesReceived += bitmap->Width * bitmap->Height *
                ( Bitmap::GetPixelFormatSize( bitmap->PixelFormat ) >> 3 );

			// notify clients about the new video frame
			NewFrame( this, gcnew NewFrameEventArgs( bitmap ) );

			// dispose the frame since we no longer need it
			delete bitmap;

            // wait for a while ?
            if ( interval > 0 )
            {
                // get frame extract duration
				TimeSpan^ span = DateTime::Now.Subtract( start );

                // miliseconds to sleep
                int msec = interval - (int) span->TotalMilliseconds;

                if ( ( msec > 0 ) && ( m_needToStop->WaitOne( msec, false ) == true ) )
					break;
            }
		}
	}
	catch ( Exception^ exception )
	{
        VideoSourceError( this, gcnew VideoSourceErrorEventArgs( exception->Message ) );
	}

	videoReader->Close( );
	PlayingFinished( this, reasonToStop );
}