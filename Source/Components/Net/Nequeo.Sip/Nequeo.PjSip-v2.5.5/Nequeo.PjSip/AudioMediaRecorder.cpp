/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AudioMediaRecorder.cpp
*  Purpose :       SIP AudioMediaRecorder class.
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

#include "AudioMediaRecorder.h"
#include "MediaType.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Audio media recorder.
/// </summary>
AudioMediaRecorder::AudioMediaRecorder() :
	_disposed(false), _pjAudioMediaRecorder(new pj::AudioMediaRecorder())
{
}

///	<summary>
///	Audio media recorder.
///	</summary>
AudioMediaRecorder::~AudioMediaRecorder()
{
	if (!_disposed)
	{
		// Cleanup the native classes.
		this->!AudioMediaRecorder();

		_disposed = true;
	}
}

///	<summary>
///	Audio media recorder.
///	</summary>
AudioMediaRecorder::!AudioMediaRecorder()
{
	if (!_disposed)
	{
		// If the callback has been created.
		if (_pjAudioMediaRecorder != nullptr)
		{
			// Cleanup the native classes.
			delete _pjAudioMediaRecorder;
			_pjAudioMediaRecorder = nullptr;
		}
	}
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void AudioMediaRecorder::MarshalString(String^ s, std::string& os)
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
void AudioMediaRecorder::MarshalString(String^ s, std::wstring& os)
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
/// Create a file recorder, and automatically connect this recorder to
/// the conference bridge.The recorder currently supports recording WAV
/// file.The type of the recorder to use is determined by the extension of
/// the file(e.g. ".wav").
/// </summary>
/// <param name="filename">Output file name. The function will determine the
///	default format to be used based on the file extension.
///	Currently ".wav" is supported on all platforms.</param>
/// <param name="encoderType">Optionally specify the type of encoder to be used to
///	compress the media, if the file can support different
///	encodings.This value must be zero for now.</param>
/// <param name="maxSize">Maximum file size. Specify zero or -1 to remove size
///	limitation.This value must be zero or -1 for now.</param>
/// <param name="options">Optional options, which can be used to specify the
/// recording file format. Default is zero or PJMEDIA_FILE_WRITE_PCM.</param>
void AudioMediaRecorder::CreateRecorder(String^ filename, unsigned encoderType, long maxSize, unsigned options)
{
	std::string filenameN;
	MarshalString(filename, filenameN);

	// Create the audio file.
	_pjAudioMediaRecorder->createRecorder(filenameN, encoderType, maxSize, options);
}

/// <summary>
/// Start recording.
/// </summary>
/// <param name="captureMedia">The audio capture media.</param>
void AudioMediaRecorder::Start(AudioMedia^ captureMedia)
{
	pj::AudioMedia& media = captureMedia->GetAudioMedia();
	media.startTransmit(*_pjAudioMediaRecorder);
}

/// <summary>
/// Stop recording.
/// </summary>
/// <param name="captureMedia">The audio capture media.</param>
void AudioMediaRecorder::Stop(AudioMedia^ captureMedia)
{
	pj::AudioMedia& media = captureMedia->GetAudioMedia();
	media.stopTransmit(*_pjAudioMediaRecorder);
}

/// <summary>
/// Get the audio media recorder reference.
/// </summary>
/// <returns>The audio media recorder reference.</returns>
pj::AudioMediaRecorder& AudioMediaRecorder::GetAudioMediaRecorder()
{
	return *_pjAudioMediaRecorder;
}

/// <summary>
/// Start recoding a conversation between one or more calls.
/// </summary>
/// <param name="captureMedia">The capture media; e.g the local microphone.</param>
/// <param name="conferenceCalls">Array of remote conference calls.</param>
void AudioMediaRecorder::StartRecordingConversation(AudioMedia^ captureMedia, array<AudioMedia^>^ conferenceCalls)
{
	pj::AudioMedia& media = captureMedia->GetAudioMedia();
	media.startTransmit(*_pjAudioMediaRecorder);

	// For each call.
	for (int i = 0; i < conferenceCalls->Length; i++)
	{
		pj::AudioMedia& mediaCall = conferenceCalls[i]->GetAudioMedia();
		mediaCall.startTransmit(*_pjAudioMediaRecorder);
	}
}

/// <summary>
/// Stop recoding a conversation between one or more calls.
/// </summary>
/// <param name="captureMedia">The capture media; e.g the local microphone.</param>
/// <param name="conferenceCalls">Array of remote conference calls.</param>
void AudioMediaRecorder::StopRecordingConversation(AudioMedia^ captureMedia, array<AudioMedia^>^ conferenceCalls)
{
	pj::AudioMedia& media = captureMedia->GetAudioMedia();
	media.stopTransmit(*_pjAudioMediaRecorder);

	// For each call.
	for (int i = 0; i < conferenceCalls->Length; i++)
	{
		pj::AudioMedia& mediaCall = conferenceCalls[i]->GetAudioMedia();
		mediaCall.stopTransmit(*_pjAudioMediaRecorder);
	}
}