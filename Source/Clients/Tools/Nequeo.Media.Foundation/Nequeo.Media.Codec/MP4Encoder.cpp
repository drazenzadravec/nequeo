/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MP4Encoder.cpp
*  Purpose :       MP4Encoder class.
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

#include "MP4Encoder.h"
#include "VideoCodec.h"

#include <vcclr.h>
#include <msclr\auto_handle.h>

using namespace Nequeo::Media;

///	<summary>
///	MP4 encoder.
///	</summary>
MP4Encoder::MP4Encoder() : _disposed(false)
{
}

///	<summary>
///	MP4 encoder deconstructor.
///	</summary>
MP4Encoder::~MP4Encoder()
{
	if (!_disposed)
	{
		// Cleanup the native classes.
		this->!MP4Encoder();

		_disposed = true;
	}
}

///	<summary>
///	MP4 encoder finalizer.
///	</summary>
MP4Encoder::!MP4Encoder()
{
	if (!_disposed)
	{

	}
}

/// <summary>
/// Encode the file.
/// </summary>
/// <param name="inputFilename">The input file to convert.</param>
/// <param name="outputFilename">The mp4 encoded file.</param>
/// <param name="audioProfile">The mp4 encoded audio profile.</param>
/// <param name="videoProfile">The mp4 encoded video profile.</param>
void MP4Encoder::Encode(String^ inputFilename, String^ outputFilename, AACProfileType audioProfile, H264ProfileType videoProfile)
{
	HRESULT hr;
	HRESULT mfhr;
	HRESULT encodeHR;

	int video_profile = 0;
	int audio_profile = 0;

	// Select the AAC audio profile.
	switch (audioProfile)
	{
	case Nequeo::Media::AACProfileType::ACC_1:
		audio_profile = 0;
		break;
	case Nequeo::Media::AACProfileType::ACC_2:
		audio_profile = 1;
		break;
	case Nequeo::Media::AACProfileType::ACC_3:
		audio_profile = 2;
		break;
	case Nequeo::Media::AACProfileType::ACC_4:
		audio_profile = 3;
		break;
	default:
		audio_profile = 0;
		break;
	}

	// Select the H264 video profile.
	switch (videoProfile)
	{
	case Nequeo::Media::H264ProfileType::H264_1:
		video_profile = 0;
		break;
	case Nequeo::Media::H264ProfileType::H264_2:
		video_profile = 1;
		break;
	case Nequeo::Media::H264ProfileType::H264_3:
		video_profile = 2;
		break;
	case Nequeo::Media::H264ProfileType::H264_4:
		video_profile = 3;
		break;
	case Nequeo::Media::H264ProfileType::H264_5:
		video_profile = 4;
		break;
	case Nequeo::Media::H264ProfileType::H264_6:
		video_profile = 5;
		break;
	case Nequeo::Media::H264ProfileType::H264_7:
		video_profile = 6;
		break;
	default:
		video_profile = 0;
		break;
	}

	pin_ptr<const wchar_t> inputFilenameN = PtrToStringChars(inputFilename);
	pin_ptr<const wchar_t> outputFilenameN = PtrToStringChars(outputFilename);

	try
	{
		// Set heap information.
		HeapSetInformation(NULL, HeapEnableTerminationOnCorruption, NULL, 0);

		// Start apartment thread.
		hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
		if (SUCCEEDED(hr))
		{
			// Start the media foundation.
			mfhr = MFStartup(MF_VERSION);
			if (SUCCEEDED(mfhr))
			{
				// Encode the file.
				encodeHR = EncodeFile(inputFilenameN, outputFilenameN, video_profile, audio_profile);

				// if encoding did not succeed.
				if (!SUCCEEDED(encodeHR))
				{
					// Throw exception.
					throw std::exception("Encoding has encounted an error.");
				}
			}
		}
	}
	catch (const std::exception& ex)
	{
		System::String^ errorMessage = gcnew System::String(ex.what());
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Encoding error.", innerException);
	}
	finally
	{
		if (SUCCEEDED(mfhr))
		{
			// Shitdown the media foundation.
			MFShutdown();
		}

		if (SUCCEEDED(hr))
		{
			// Stop apartment thread.
			CoUninitialize();
		}
	}
}

///	<summary>
///	MarshalString
///	</summary>
/// <param name="s">The string.</param>
/// <param name="os">The native string.</param>
void MP4Encoder::MarshalString(String^ s, std::string& os)
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
void MP4Encoder::MarshalString(String^ s, std::wstring& os)
{
	if (!String::IsNullOrEmpty(s))
	{
		using namespace Runtime::InteropServices;
		const wchar_t* chars = (const wchar_t*)(Marshal::StringToHGlobalUni(s)).ToPointer();
		os = chars;
		Marshal::FreeHGlobal(IntPtr((void*)chars));
	}
}