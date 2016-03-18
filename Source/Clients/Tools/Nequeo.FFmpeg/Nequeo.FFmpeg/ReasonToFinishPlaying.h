/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          ReasonToFinishPlaying.h
*  Purpose :       ReasonToFinishPlaying class.
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

#ifndef _REASONTOFINISHPLAYING_H
#define _REASONTOFINISHPLAYING_H

#include "stdafx.h"

using namespace System;

namespace Nequeo
{
	namespace Media
	{
		namespace FFmpeg
		{
			/// <summary>
			/// Reason of finishing video playing.
			/// </summary>
			/// <remarks><para>When video source class fire the <see cref="IVideoSource.PlayingFinished"/> event, they
			/// need to specify reason of finishing video playing. For example, it may be end of stream reached.</para></remarks>
			public enum class ReasonToFinishPlaying
			{
				/// <summary>
				/// Video playing has finished because it end was reached.
				/// </summary>
				EndOfStreamReached,
				/// <summary>
				/// Video playing has finished because it was stopped by user.
				/// </summary>
				StoppedByUser,
				/// <summary>
				/// Video playing has finished because the device was lost (unplugged).
				/// </summary>
				DeviceLost,
				/// <summary>
				/// Video playing has finished because of some error happened the video source (camera, stream, file, etc.).
				/// A error reporting event usually is fired to provide error information.
				/// </summary>
				VideoSourceError
			};
		}
	}
}
#endif