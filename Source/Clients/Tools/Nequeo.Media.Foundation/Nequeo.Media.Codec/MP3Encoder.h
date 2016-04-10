/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MP3Encoder.h
*  Purpose :       MP3Encoder class.
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

#ifndef _MP3ENCODER_H
#define _MP3ENCODER_H

#include "stdafx.h"

#include "lame.h"

using namespace System;
using namespace System::Text;
using namespace System::Collections;
using namespace System::Collections::Generic;

namespace Nequeo
{
	namespace Media
	{
		/// <summary>
		/// MP3 encoder.
		/// </summary>
		public ref class MP3Encoder sealed
		{
		public:
			///	<summary>
			///	MP3 encoder.
			///	</summary>
			MP3Encoder();

			///	<summary>
			///MP3 encoder deconstructor.
			///	</summary>
			~MP3Encoder();

			///	<summary>
			///	MP3 encoder finalizer.
			///	</summary>
			!MP3Encoder();

			/// <summary>
			/// Encode the file.
			/// </summary>
			/// <param name="inputFilename">The input file to convert.</param>
			/// <param name="outputFilename">The mp3 encoded file.</param>
			void Encode(String^ inputFilename, String^ outputFilename);

		private:
			bool _disposed;

			void MarshalString(String^ s, std::string& os);
			void MarshalString(String^ s, std::wstring& os);

			///	<summary>
			///	Convert from wide string to string.
			///	</summary>
			/// <param name="wstr">The wide string.</param>
			/// <returns>The result.</returns>
			std::string WstringToString(std::wstring wstr);

			///	<summary>
			///	Convert from string to wide string.
			///	</summary>
			/// <param name="str">The string.</param>
			/// <returns>The result.</returns>
			std::wstring StringToWstring(std::string str);

		};
	}
}
#endif