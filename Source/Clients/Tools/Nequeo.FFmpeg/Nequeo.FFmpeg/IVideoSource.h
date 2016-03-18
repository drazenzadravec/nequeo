/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          IVideoSource.h
*  Purpose :       IVideoSource class.
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

#ifndef _IVIDEOSOURCE_H
#define _IVIDEOSOURCE_H

#include "stdafx.h"

#include "ReasonToFinishPlaying.h"
#include "NewFrameEventArgs.h"
#include "VideoSourceErrorEventArgs.h"

using namespace System;

namespace Nequeo
{
	namespace Media
	{
		namespace FFmpeg
		{
			/// <summary>
			/// Delegate for new frame event handler.
			/// </summary>
			/// <param name="sender">Sender object.</param>
			/// <param name="eventArgs">Event arguments.</param>
			public delegate void NewFrameEventHandler(Object^ sender, NewFrameEventArgs^ eventArgs);

			/// <summary>
			/// Delegate for video source error event handler.
			/// </summary>
			/// <param name="sender">Sender object.</param>
			/// <param name="eventArgs">Event arguments.</param>
			public delegate void VideoSourceErrorEventHandler(Object^ sender, VideoSourceErrorEventArgs^ eventArgs);

			/// <summary>
			/// Delegate for playing finished event handler.
			/// </summary>
			/// <param name="sender">Sender object.</param>
			/// <param name="reason">Reason of finishing video playing.</param>
			public delegate void PlayingFinishedEventHandler(Object^ sender, ReasonToFinishPlaying reason);

			/// <summary>
			/// Video source interface.
			/// </summary>
			/// <remarks>The interface describes common methods for different type of video sources.</remarks>
			public interface class IVideoSource
			{
			public:
				/// <summary>
				/// New frame event.
				/// </summary>
				/// <remarks><para>This event is used to notify clients about new available video frame.</para>
				/// <para><note>Since video source may have multiple clients, each client is responsible for
				/// making a copy (cloning) of the passed video frame, but video source is responsible for
				/// disposing its own original copy after notifying of clients.</note></para>
				/// </remarks>
				event NewFrameEventHandler^ NewFrame;

				/// <summary>
				/// Video source error event.
				/// </summary>
				/// <remarks>This event is used to notify clients about any type of errors occurred in
				/// video source object, for example internal exceptions.</remarks>
				event VideoSourceErrorEventHandler^ VideoSourceError;

				/// <summary>
				/// Video playing finished event.
				/// </summary>
				/// <remarks><para>This event is used to notify clients that the video playing has finished.</para>
				/// </remarks>
				event PlayingFinishedEventHandler^ PlayingFinished;

				/// <summary>
				/// Video source.
				/// </summary>
				/// <remarks>The meaning of the property depends on particular video source.
				/// Depending on video source it may be a file name, URL or any other string
				/// describing the video source.</remarks>
				property String^ Source
				{
					String^ get();
					void set(String^ fileName);
				}

				/// <summary>
				/// Received frames count.
				/// </summary>
				/// <remarks>Number of frames the video source provided from the moment of the last
				/// access to the property.
				/// </remarks>
				property int FramesReceived
				{
					int get();
				}

				/// <summary>
				/// Received bytes count.
				/// </summary>
				/// <remarks>Number of bytes the video source provided from the moment of the last
				/// access to the property.
				/// </remarks>
				property long long BytesReceived
				{
					long long get();
				}

				/// <summary>
				/// State of the video source.
				/// </summary>
				/// <remarks>Current state of video source object - running or not.</remarks>
				property bool IsRunning
				{
					bool get();
				}

				/// <summary>
				/// Start video source.
				/// </summary>
				/// <remarks>Starts video source and return execution to caller. Video source
				/// object creates background thread and notifies about new frames with the
				/// help of <see cref="NewFrame"/> event.</remarks>
				void Start();

				/// <summary>
				/// Signal video source to stop its work.
				/// </summary>
				/// <remarks>Signals video source to stop its background thread, stop to
				/// provide new frames and free resources.</remarks>
				void SignalToStop();

				/// <summary>
				/// Wait for video source has stopped.
				/// </summary>
				/// <remarks>Waits for video source stopping after it was signalled to stop using
				/// <see cref="SignalToStop"/> method.</remarks>
				void WaitForStop();

				/// <summary>
				/// Stop video source.
				/// </summary>
				/// <remarks>Stops video source aborting its thread.</remarks>
				void Stop();
			};
		}
	}
}
#endif