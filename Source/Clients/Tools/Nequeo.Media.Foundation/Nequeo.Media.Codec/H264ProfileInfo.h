/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          H264ProfileInfo.h
*  Purpose :       H264ProfileInfo class.
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

#ifndef _H264PROFILEINFO_H
#define _H264PROFILEINFO_H

#include "stdafx.h"

namespace Nequeo
{
	namespace Media
	{
		/// <summary>
		/// A structure to encapsulate H264 video.
		/// </summary>
		struct H264ProfileInfo
		{
			UINT32  profile;
			MFRatio fps;
			MFRatio frame_size;
			UINT32  bitrate;
		};

		/// <summary>
		/// A structure to encapsulate H264 video.
		/// </summary>
		public enum class AVEncH264VProfile
		{
			eAVEncH264VProfile_unknown = 0,
			eAVEncH264VProfile_Simple = 66,
			eAVEncH264VProfile_Base = 66,
			eAVEncH264VProfile_Main = 77,
			eAVEncH264VProfile_High = 100,
			eAVEncH264VProfile_422 = 122,
			eAVEncH264VProfile_High10 = 110,
			eAVEncH264VProfile_444 = 144,
			eAVEncH264VProfile_Extended = 88,
			// UVC 1.2 H.264 extension
			eAVEncH264VProfile_ScalableBase = 83,
			eAVEncH264VProfile_ScalableHigh = 86,
			eAVEncH264VProfile_MultiviewHigh = 118,
			eAVEncH264VProfile_StereoHigh = 128,
			eAVEncH264VProfile_ConstrainedBase = 256,
			eAVEncH264VProfile_UCConstrainedHigh = 257,
			eAVEncH264VProfile_UCScalableConstrainedBase = 258,
			eAVEncH264VProfile_UCScalableConstrainedHigh = 259
		};

		/// <summary>
		/// A structure to encapsulate H264 video frame per second.
		/// </summary>
		public ref struct FramePerSecondRatio
		{
			unsigned long Rate;
			unsigned long Base;
		};

		/// <summary>
		/// A structure to encapsulate H264 video frame size.
		/// </summary>
		public ref struct FrameSizeRatio
		{
			unsigned long Width;
			unsigned long Height;
		};

		/// <summary>
		/// A structure to encapsulate H264 video.
		/// </summary>
		public ref struct H264Profile
		{
			AVEncH264VProfile Profile;
			FramePerSecondRatio^ FramePerSecond;
			FrameSizeRatio^ FrameSize;
			unsigned int BitRate;
		};
	}
}
#endif