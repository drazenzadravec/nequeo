/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaDirection.h
*  Purpose :       SIP MediaDirection class.
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

#ifndef _MEDIADIRECTION_H
#define _MEDIADIRECTION_H

#include "stdafx.h"

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
			/// Media direction.
			/// </summary>
			public enum class MediaDirection : unsigned
			{
				///	<summary>
				/// None.
				///	</summary>
				PJMEDIA_DIR_NONE = 0,
				///	<summary>
				/// Encoding (outgoing to network) stream, also known as capture.
				///	</summary>
				PJMEDIA_DIR_ENCODING = 1,
				///	<summary>
				/// Same as encoding direction.
				///	</summary>
				PJMEDIA_DIR_CAPTURE = PJMEDIA_DIR_ENCODING,
				///	<summary>
				/// Decoding (incoming from network) stream, also known as playback.
				///	</summary>
				PJMEDIA_DIR_DECODING = 2,
				///	<summary>
				/// Same as decoding.
				///	</summary>
				PJMEDIA_DIR_PLAYBACK = PJMEDIA_DIR_DECODING,
				///	<summary>
				/// Same as decoding.
				///	</summary>
				PJMEDIA_DIR_RENDER = PJMEDIA_DIR_DECODING,
				///	<summary>
				/// Incoming and outgoing stream, same as PJMEDIA_DIR_CAPTURE_PLAYBACK.
				///	</summary>
				PJMEDIA_DIR_ENCODING_DECODING = 3,
				///	<summary>
				/// Same as ENCODING_DECODING.
				///	</summary>
				PJMEDIA_DIR_CAPTURE_PLAYBACK = PJMEDIA_DIR_ENCODING_DECODING,
				///	<summary>
				/// Same as ENCODING_DECODING.
				///	</summary>
				PJMEDIA_DIR_CAPTURE_RENDER = PJMEDIA_DIR_ENCODING_DECODING

			};
		}
	}
}
#endif