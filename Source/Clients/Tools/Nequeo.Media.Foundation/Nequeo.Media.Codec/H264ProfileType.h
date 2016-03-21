/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          H264ProfileType.h
*  Purpose :       H264ProfileType class.
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

#ifndef _H264PROFILETYPE_H
#define _H264PROFILETYPE_H

#include "stdafx.h"

namespace Nequeo
{
	namespace Media
	{
		/// <summary>
		/// A enum of encapsulated H264 video types.
		/// </summary>
		public enum class H264ProfileType
		{
			/// <summary>
			/// Frames per second ratio { 15, 1 }, Frame size { 176, 144 }, Bit rate 128000.
			/// </summary>
			H264_1,
			/// <summary>
			/// Frames per second ratio { 15, 1 }, Frame size { 352, 288 }, Bit rate 384000 }
			/// </summary>
			H264_2,
			/// <summary>
			/// Frames per second ratio { 30, 1 }, Frame size { 352, 288 }, Bit rate 384000 }
			/// </summary>
			H264_3,
			/// <summary>
			/// Frames per second ratio { 29970, 1000 }, Frame size { 320, 240 }, Bit rate 528560 }
			/// </summary>
			H264_4,
			/// <summary>
			/// Frames per second ratio { 15, 1 }, Frame size { 720, 576 }, Bit rate 4000000 }
			/// </summary>
			H264_5,
			/// <summary>
			/// Frames per second ratio { 25, 1 }, Frame size { 720, 576 }, Bit rate 10000000 }
			/// </summary>
			H264_6,
			/// <summary>
			/// Frames per second ratio { 30, 1 }, Frame size { 352, 288 }, Bit rate 10000000 }
			/// </summary>
			H264_7,
		};
	}
}
#endif