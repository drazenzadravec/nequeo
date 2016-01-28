/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AudioMediaPlayer.cpp
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

#include "stdafx.h"

#include "AudioMediaPlayer.h"
#include "MediaType.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Audio media player.
/// </summary>
AudioMediaPlayer::AudioMediaPlayer() : 
_disposed(false), _pjAudioMediaPlayer(new pj::AudioMediaPlayer())
{
}

///	<summary>
///	Audio media player.
///	</summary>
AudioMediaPlayer::~AudioMediaPlayer()
{
	if (!_disposed)
	{
		// Cleanup the native classes.
		this->!AudioMediaPlayer();

		_disposed = true;
	}
}

///	<summary>
///	Audio media player.
///	</summary>
AudioMediaPlayer::!AudioMediaPlayer()
{
	if (!_disposed)
	{
		// If the callback has been created.
		if (_pjAudioMediaPlayer != nullptr)
		{
			// Cleanup the native classes.
			delete _pjAudioMediaPlayer;
			_pjAudioMediaPlayer = nullptr;
		}
	}
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void AudioMediaPlayer::MarshalString(String^ s, std::string& os)
{
	if (!String::IsNullOrEmpty(s))
	{
		using namespace Runtime::InteropServices;
		const char* chars = (const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
		os = chars;
		Marshal::FreeHGlobal(IntPtr((void*)chars));
	}
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void AudioMediaPlayer::MarshalString(String^ s, std::wstring& os)
{
	if (!String::IsNullOrEmpty(s))
	{
		using namespace Runtime::InteropServices;
		const wchar_t* chars = (const wchar_t*)(Marshal::StringToHGlobalUni(s)).ToPointer();
		os = chars;
		Marshal::FreeHGlobal(IntPtr((void*)chars));
	}
}

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
void AudioMediaPlayer::CreatePlayer(String^ filename, unsigned options)
{
	std::string filenameN;
	MarshalString(filename, filenameN);

	// Create the player.
	_pjAudioMediaPlayer->createPlayer(filenameN, options);
}

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
void AudioMediaPlayer::CreatePlaylist(array<String^>^ filenames, String^ label, unsigned options)
{
	pj::StringVector audioFiles;

	// For each file name.
	for (int i = 0; i < filenames->Length; i++)
	{
		std::string filenameN;
		MarshalString(filenames[i], filenameN);

		// Add the file.
		audioFiles.push_back(filenameN);
	}

	std::string labelN;
	MarshalString(label, labelN);

	// Create the player.
	_pjAudioMediaPlayer->createPlaylist(audioFiles, labelN, options);
}

/// <summary>
/// Get current playback position in samples. This operation is not valid for playlist.
/// </summary>
/// <returns>The current playback position, in samples..</returns>
unsigned int AudioMediaPlayer::GetPosition()
{
	return _pjAudioMediaPlayer->getPos();
}

/// <summary>
/// Set playback position in samples. This operation is not valid for playlist.
/// </summary>
/// <param name="samples">The desired playback position, in samples.</param>
void AudioMediaPlayer::SetPosition(unsigned int samples)
{
	_pjAudioMediaPlayer->setPos(samples);
}

/// <summary>
/// Start playback.
/// </summary>
/// <param name="playbackMedia">The audio playback media.</param>
void AudioMediaPlayer::Start(AudioMedia^ playbackMedia)
{
	pj::AudioMedia& media = playbackMedia->GetAudioMedia();
	_pjAudioMediaPlayer->startTransmit(media);
}

/// <summary>
/// Stop playback.
/// </summary>
/// <param name="playbackMedia">The audio playback media.</param>
void AudioMediaPlayer::Stop(AudioMedia^ playbackMedia)
{
	pj::AudioMedia& media = playbackMedia->GetAudioMedia();
	_pjAudioMediaPlayer->stopTransmit(media);
}

/// <summary>
/// Start playing audio to each call.
/// </summary>
/// <param name="conferenceCalls">Array of remote conference calls.</param>
void AudioMediaPlayer::StartPlayingConversation(array<AudioMedia^>^ conferenceCalls)
{
	// For each call.
	for (int i = 0; i < conferenceCalls->Length; i++)
	{
		pj::AudioMedia& media = conferenceCalls[i]->GetAudioMedia();
		_pjAudioMediaPlayer->startTransmit(media);
	}
}

/// <summary>
/// Stop playing audio to each call.
/// </summary>
/// <param name="conferenceCalls">Array of remote conference calls.</param>
void AudioMediaPlayer::StoptPlayingConversation(array<AudioMedia^>^ conferenceCalls)
{
	// For each call.
	for (int i = 0; i < conferenceCalls->Length; i++)
	{
		pj::AudioMedia& media = conferenceCalls[i]->GetAudioMedia();
		_pjAudioMediaPlayer->stopTransmit(media);
	}
}