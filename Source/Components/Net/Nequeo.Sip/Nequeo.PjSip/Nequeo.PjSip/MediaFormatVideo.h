/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaFormatVideo.h
*  Purpose :       SIP MediaFormatVideo class.
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

#ifndef _MEDIAFORMATVIDEO_H
#define _MEDIAFORMATVIDEO_H

#include "stdafx.h"

#include "MediaType.h"
#include "MediaFormat.h"

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
			/// This structure describe detail information about an video media.
			/// </summary>
			public ref class MediaFormatVideo sealed : public MediaFormat
			{
			public:
				/// <summary>
				/// This structure describe detail information about an video media.
				/// </summary>
				MediaFormatVideo();

				/// <summary>
				/// Gets or sets the video width.
				/// </summary>
				property unsigned Width
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets the video height.
				/// </summary>
				property unsigned Height
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets the frames per second numerator.
				/// </summary>
				property int FpsNum
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets the frames per second denumerator.
				/// </summary>
				property int FpsDenum
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets the average bitrate.
				/// </summary>
				property unsigned AvgBps
				{
					unsigned get();
					void set(unsigned value);
				}

				/// <summary>
				/// Gets or sets the maximum bitrate.
				/// </summary>
				property unsigned MaxBps
				{
					unsigned get();
					void set(unsigned value);
				}

			private:
				unsigned _width;
				unsigned _height;
				int _fpsNum;
				int _fpsDenum;
				unsigned _avgBps;
				unsigned _maxBps;
			};
		}
	}
}
#endif