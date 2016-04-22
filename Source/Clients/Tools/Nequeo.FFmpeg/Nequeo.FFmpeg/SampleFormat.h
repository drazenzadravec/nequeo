/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          SampleFormat.h
*  Purpose :       SampleFormat class.
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

#ifndef _SAMPLEFORMAT_H
#define _SAMPLEFORMAT_H

#include "stdafx.h"

using namespace System;

namespace Nequeo
{
	namespace Media
	{
		namespace FFmpeg
		{
			/// <summary>
			/// Enumeration of the audio sample formats.
			/// </summary>
			public enum class SampleFormat
			{
				/// <summary>
				/// Default none.
				/// </summary>
				AV_SAMPLE_FMT_NONE = -1,
				/// <summary>
				/// Unsigned 8 bits.
				/// </summary>
				AV_SAMPLE_FMT_U8,
				/// <summary>
				/// Signed 16 bits.
				/// </summary>
				AV_SAMPLE_FMT_S16,
				/// <summary>
				/// Signed 32 bits.
				/// </summary>
				AV_SAMPLE_FMT_S32,
				/// <summary>
				/// Float.
				/// </summary>
				AV_SAMPLE_FMT_FLT,
				/// <summary>
				/// Double.
				/// </summary>
				AV_SAMPLE_FMT_DBL,
				/// <summary>
				/// Unsigned 8 bits, planar.
				/// </summary>
				AV_SAMPLE_FMT_U8P,
				/// <summary>
				/// Signed 16 bits, planar.
				/// </summary>
				AV_SAMPLE_FMT_S16P,
				/// <summary>
				/// Signed 32 bits, planar.
				/// </summary>
				AV_SAMPLE_FMT_S32P,
				/// <summary>
				/// Float, planar.
				/// </summary>
				AV_SAMPLE_FMT_FLTP,
				/// <summary>
				/// Double, planar.
				/// </summary>
				AV_SAMPLE_FMT_DBLP,
				/// <summary>
				/// Number of sample formats. DO NOT USE if linking dynamically.
				/// </summary>
				AV_SAMPLE_FMT_NB,

			};
		}
	}
}
#endif