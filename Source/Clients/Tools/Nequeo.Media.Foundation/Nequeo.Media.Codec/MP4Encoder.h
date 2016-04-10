/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MP4Encoder.h
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

#pragma once

#ifndef _MP4ENCODER_H
#define _MP4ENCODER_H

#include "stdafx.h"

#include "AACProfileType.h"
#include "H264ProfileType.h"
#include "AACProfileInfo.h"
#include "H264ProfileInfo.h"

using namespace System;

namespace Nequeo
{
	namespace Media
	{
		/// <summary>
		/// MP4 encoder.
		/// </summary>
		public ref class MP4Encoder sealed
		{
		public:
			///	<summary>
			///	MP4 encoder.
			///	</summary>
			MP4Encoder();

			///	<summary>
			///MP4 encoder deconstructor.
			///	</summary>
			~MP4Encoder();

			///	<summary>
			///	MP4 encoder finalizer.
			///	</summary>
			!MP4Encoder();

			/// <summary>
			/// Encode the file.
			/// </summary>
			/// <param name="inputFilename">The input file to convert.</param>
			/// <param name="outputFilename">The mp4 encoded file.</param>
			/// <param name="audioProfile">The mp4 encoded audio profile.</param>
			/// <param name="videoProfile">The mp4 encoded video profile.</param>
			void Encode(String^ inputFilename, String^ outputFilename, AACProfileType audioProfile, H264ProfileType videoProfile);

			/// <summary>
			/// Encode the file.
			/// </summary>
			/// <param name="inputFilename">The input file to convert.</param>
			/// <param name="outputFilename">The mp4 encoded file.</param>
			/// <param name="audioProfile">The mp4 encoded audio profile.</param>
			/// <param name="videoProfile">The mp4 encoded video profile.</param>
			void Encode(String^ inputFilename, String^ outputFilename, AACProfile^ audioProfile, H264Profile^ videoProfile);

		private:
			bool _disposed;

			void MarshalString(String^ s, std::string& os);
			void MarshalString(String^ s, std::wstring& os);

		};
	}
}
#endif