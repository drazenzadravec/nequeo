/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          PlayerState.h
*  Purpose :       PlayerState class.
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

#ifndef _PLAYERSTATE_H
#define _PLAYERSTATE_H

#include "MediaGlobal.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Media player current state.
			/// </summary>
			enum PlayerState
			{
				/// <summary>
				/// Ready.
				/// </summary>
				Ready = 0,
				/// <summary>
				/// Open pending.
				/// </summary>
				OpenPending = 1,
				/// <summary>
				/// Started.
				/// </summary>
				Started = 2,
				/// <summary>
				/// Pause pending.
				/// </summary>
				PausePending = 3,
				/// <summary>
				/// Paused.
				/// </summary>
				Paused = 4,
				/// <summary>
				/// Start pending.
				/// </summary>
				StartPending = 5,
				/// <summary>
				/// Stopped.
				/// </summary>
				Stopped = 6,
				/// <summary>
				/// Stopped.
				/// </summary>
				Closed = 7,
				/// <summary>
				/// Ended.
				/// </summary>
				Ended = 8,
			};

			/// <summary>
			/// Media player enabler state.
			/// </summary>
			enum EnablerState
			{
				/// <summary>
				/// Enabler ready.
				/// </summary>
				Enabler_Ready,
				/// <summary>
				/// Enabler silent in progress.
				/// </summary>
				Enabler_SilentInProgress,
				/// <summary>
				/// Enabler non silent in progress.
				/// </summary>
				Enabler_NonSilentInProgress,
				/// <summary>
				/// Enabler complete.
				/// </summary>
				Enabler_Complete,
			};

			/// <summary>
			/// Media player enabler flag.
			/// </summary>
			enum EnablerFlags
			{
				/// <summary>
				/// Use silent if supported, otherwise use non-silent.
				/// </summary>
				SilentOrNonSilent = 0,
				/// <summary>
				/// // Use non-silent.
				/// </summary>
				ForceNonSilent = 1,
			};

			/// <summary>
			/// Media preview current state.
			/// </summary>
			enum PreviewState
			{
				/// <summary>
				/// Ready.
				/// </summary>
				PreviewReady = 0,
				
			};

			/// <summary>
			/// Media capture current state.
			/// </summary>
			enum CaptureState
			{
				/// <summary>
				/// Not Ready.
				/// </summary>
				CaptureNotReady = 0,
				/// <summary>
				/// Ready.
				/// </summary>
				CaptureReady = 1,
				/// <summary>
				/// Capturing video.
				/// </summary>
				CapturingVideo = 2,
				/// <summary>
				/// Not capturing video.
				/// </summary>
				NotCapturingVideo = 3,
				/// <summary>
				/// Capturing audio.
				/// </summary>
				CapturingAudio = 4,
				/// <summary>
				/// Not capturing audio.
				/// </summary>
				NotCapturingAudio = 5,
				/// <summary>
				/// Capturing.
				/// </summary>
				Capturing = 6,
				/// <summary>
				/// Not capturing.
				/// </summary>
				NotCapturing = 7,

			};

			/// <summary>
			/// Media capture audio state.
			/// </summary>
			enum CaptureAudioState
			{
				/// <summary>
				/// Allow audio capture.
				/// </summary>
				AllowAudioCapture = 0,

				/// <summary>
				/// Disallow audio capture.
				/// </summary>
				DisallowAudioCapture = 1,
			};

			/// <summary>
			/// Media capture video state.
			/// </summary>
			enum CaptureVideoState
			{
				/// <summary>
				/// Allow video capture.
				/// </summary>
				AllowVideoCapture = 0,

				/// <summary>
				/// Disallow video capture.
				/// </summary>
				DisallowVideoCapture = 1,
			};

			/// <summary>
			/// Media capture screen state.
			/// </summary>
			enum CaptureScreenState
			{
				/// <summary>
				/// Allow screen capture.
				/// </summary>
				AllowScreenCapture = 0,

				/// <summary>
				/// Disallow screen capture.
				/// </summary>
				DisallowScreenCapture = 1,
			};

			/// <summary>
			/// Media source current state.
			/// </summary>
			enum MediaSourceState
			{
				/// <summary>
				/// Read source.
				/// </summary>
				MediaClosed = 0,
				/// <summary>
				/// Read source.
				/// </summary>
				ReadSourceReady = 1,
				/// <summary>
				/// Read source.
				/// </summary>
				WriteSourceReady = 2,
			};
		}
	}
}
#endif