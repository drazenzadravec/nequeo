/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AudioFileReader.h
*  Purpose :       AudioFileReader class.
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

#ifndef _AUDIOFILEREADER_H
#define _AUDIOFILEREADER_H

#include "stdafx.h"

#include "ReaderAudioPrivateData.h"
#include "AudioException.h"

using namespace System;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;
using namespace System::Collections;
using namespace System::Collections::Generic;

using namespace Nequeo::IO::Audio;

namespace Nequeo
{
	namespace Media
	{
		namespace FFmpeg
		{
			/// <summary>
			/// Audio file reader.
			/// </summary>
			public ref class AudioFileReader
			{
			public:
				/// <summary>
				/// Audio file reader.
				/// </summary>
				AudioFileReader();

				/// <summary>
				/// Disposes the object and frees its resources.
				/// </summary>
				~AudioFileReader();

				/// <summary>
				/// Object's finalizer.
				/// </summary>
				!AudioFileReader();

				/// <summary>
				/// Audio file reader.
				/// </summary>
				/// <param name="fileName">Audio file name to open.</param>
				/// <return>The audio file details.</return>
				WaveStructure Open(String^ fileName);

				/// <summary>
				/// Read next audio frame of the currently opened audio file.
				/// </summary>
				/// <returns>The array of audio frame data.</returns>
				array<unsigned char>^ ReadAudioFrame();

				/// <summary>
				/// Close currently opened audio file if any.
				/// </summary>
				void Close();

			private:
				bool _disposed;
				ReaderAudioPrivateData^ _data;

				/// <summary>
				/// Decode next audio frame of the currently opened audio file.
				/// </summary>
				/// <param name="bytesDecoded">The number of bytes decoded.</param>
				/// <returns>The array of audio frame data.</returns>
				array<unsigned char>^ DecodeAudioFrame(int bytesDecoded);
			};
		}
	}
}
#endif