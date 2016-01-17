/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaFormat.h
*  Purpose :       SIP MediaFormat class.
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

#ifndef _MEDIAFORMAT_H
#define _MEDIAFORMAT_H

#include "stdafx.h"

#include "MediaType.h"

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
			/// This structure contains all the information needed to completely describe a media.
			/// </summary>
			public ref class MediaFormat
			{
			public:
				/// <summary>
				/// This structure contains all the information needed to completely describe a media.
				/// </summary>
				MediaFormat();

				/// <summary>
				/// Gets or sets the media format id.
				/// </summary>
				property unsigned Id
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets the media type.
				/// </summary>
				property MediaType Type
				{
					MediaType get();
					void set(MediaType value);
				}

			internal:
				/// <summary>
				/// Get the media type.
				/// </summary>
				/// <param name="mediaType">The current media type.</param>
				/// <returns>The media type.</returns>
				static MediaType GetMediaTypeEx(pjmedia_type mediaType);

			private:
				unsigned _id;
				MediaType _type;
			};
		}
	}
}
#endif