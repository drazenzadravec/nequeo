/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
*
*  File :          TextToSpeech.h
*  Purpose :       Text To Speech header.
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

#include "GlobalText.h"

#include <sapi.h>

namespace Nequeo {
	namespace IO
	{
		///	<summary>
		///	Text to speech.
		///	</summary>
		class TextToSpeech
		{
		public:
			///	<summary>
			///	Text to speech.
			///	</summary>
			TextToSpeech();

			///	<summary>
			///	Text to speech.
			///	</summary>
			~TextToSpeech();

			///	<summary>
			///	Speak the text.
			///	</summary>
			/// <param name="text">The text to speak.</param>
			void Speak(std::wstring text);

			///	<summary>
			///	Pause speak the text.
			///	</summary>
			void Pause();

			///	<summary>
			///	Resume speak the text.
			///	</summary>
			void Resume();

		private:
			bool _disposed;

			HRESULT _hr;
			ISpVoice* _pVoice;
		};
	}
}
