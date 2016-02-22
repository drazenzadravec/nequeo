/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AudioMediaPlayer.h
*  Purpose :       SIP AudioMediaPlayer class.
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

#ifndef _AUDIOMEDIAPLAYER_H
#define _AUDIOMEDIAPLAYER_H

#include "stdafx.h"

#include "Media.h"
#include "AudioMedia.h"
#include "CallAudioMedia.h"
#include "AudioMediaPlayerCallback.h"

#include "pjsua2\media.hpp"
#include "pjsua2.hpp"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			delegate bool OnPlayerEndOfFileCallback();

			/// <summary>
			/// Audio media player.
			/// </summary>
			public ref class AudioMediaPlayer
			{
			public:
				/// <summary>
				/// Audio media player.
				/// </summary>
				AudioMediaPlayer();

				///	<summary>
				///	Audio media player.
				///	</summary>
				~AudioMediaPlayer();

				///	<summary>
				///	Audio media player.
				///	</summary>
				!AudioMediaPlayer();

				///	<summary>
				///	Register a callback to be called when the file player reading has
				/// reached the end of file, or when the file reading has reached the
				/// end of file of the last file for a playlist.If the file or playlist
				/// is set to play repeatedly, then the callback will be called multiple
				/// times.
				///	</summary>
				event System::EventHandler<bool>^ OnPlayerEndOfFile;

				/// <summary>
				/// Create a file player, and automatically add this 
				/// player to the conference bridge.
				/// </summary>
				/// <param name="filename">The filename to be played. Currently only
				///	WAV files are supported, and the WAV file MUST be
				///	formatted as 16bit PCM mono / single channel(any
				///	clock rate is supported).</param>
				/// <param name="options">Optional option flag. Application may specify
				/// PJMEDIA_FILE_NO_LOOP to prevent playback loop; default is zero.</param>
				void CreatePlayer(String^ filename, unsigned options);

				/// <summary>
				/// Create a file playlist media port, and automatically add the port
				/// to the conference bridge.
				/// </summary>
				/// <param name="filenames">The Array of file names to be added to the play list.
				///	Note that the files must have the same clock rate,
				/// number of channels, and number of bits per sample.</param>
				/// <param name="label">Optional label to be set for the media port; default is empty string.</param>
				/// <param name="options">Optional option flag. Application may specify
				/// PJMEDIA_FILE_NO_LOOP to prevent playback loop; default is zero.</param>
				void CreatePlaylist(array<String^>^ filenames, String^ label, unsigned options);

				/// <summary>
				/// Get current playback position in samples. This operation is not valid for playlist.
				/// </summary>
				/// <returns>The current playback position, in samples..</returns>
				unsigned int GetPosition();

				/// <summary>
				/// Set playback position in samples. This operation is not valid for playlist.
				/// </summary>
				/// <param name="samples">The desired playback position, in samples.</param>
				void SetPosition(unsigned int samples);

				/// <summary>
				/// Start playback.
				/// </summary>
				/// <param name="playbackMedia">The audio playback media.</param>
				void Start(AudioMedia^ playbackMedia);

				/// <summary>
				/// Stop playback.
				/// </summary>
				/// <param name="playbackMedia">The audio playback media.</param>
				void Stop(AudioMedia^ playbackMedia);

				/// <summary>
				/// Start playing audio to each call.
				/// </summary>
				/// <param name="conferenceCalls">Array of remote conference calls.</param>
				void StartPlayingConversation(array<AudioMedia^>^ conferenceCalls);

				/// <summary>
				/// Stop playing audio to each call.
				/// </summary>
				/// <param name="conferenceCalls">Array of remote conference calls.</param>
				void StoptPlayingConversation(array<AudioMedia^>^ conferenceCalls);

			private:
				bool _disposed;

				AudioMediaPlayerCallback* _pjAudioMediaPlayer;

				/// <summary>
				/// Create the player.
				/// </summary>
				void Create();

				void MarshalString(String^ s, std::string& os);
				void MarshalString(String^ s, std::wstring& os);

				GCHandle _gchPlayerEndOfFile;
				bool OnPlayerEndOfFile_Handler();
			};
		}
	}
}
#endif