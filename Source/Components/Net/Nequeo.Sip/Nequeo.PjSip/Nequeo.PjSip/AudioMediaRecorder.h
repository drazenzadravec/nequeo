/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AudioMediaRecorder.h
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

#pragma once

#ifndef _AUDIOMEDIARECORDER_H
#define _AUDIOMEDIARECORDER_H

#include "stdafx.h"

#include "Media.h"
#include "AudioMedia.h"
#include "CallAudioMedia.h"

#include "pjsua2\media.hpp"
#include "pjsua2.hpp"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			/// <summary>
			/// Audio media recorder.
			/// </summary>
			public ref class AudioMediaRecorder
			{
			public:
				/// <summary>
				/// Audio media recorder.
				/// </summary>
				AudioMediaRecorder();

				///	<summary>
				///	Audio media recorder.
				///	</summary>
				~AudioMediaRecorder();

				///	<summary>
				///	Audio media recorder.
				///	</summary>
				!AudioMediaRecorder();

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
				void CreateRecorder(String^ filename, unsigned encoderType, long maxSize, unsigned options);

				/// <summary>
				/// Start recording.
				/// </summary>
				/// <param name="captureMedia">The audio capture media.</param>
				void Start(AudioMedia^ captureMedia);

				/// <summary>
				/// Stop recording.
				/// </summary>
				/// <param name="captureMedia">The audio capture media.</param>
				void Stop(AudioMedia^ captureMedia);

			private:
				bool _disposed;

				pj::AudioMediaRecorder* _pjAudioMediaRecorder;

				void MarshalString(String^ s, std::string& os);
				void MarshalString(String^ s, std::wstring& os);
			};
		}
	}
}
#endif