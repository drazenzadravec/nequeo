/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaMuxDataNative.h
*  Purpose :       MediaMuxDataNative class.
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

#ifndef _MEDIAMUXDATANATIVE_H
#define _MEDIAMUXDATANATIVE_H

#include "stdafx.h"

namespace libffmpeg
{
	/// <summary>
	/// A wrapper around a single output AVStream
	/// </summary>
	typedef struct OutputStream
	{
		AVStream *st;

		/* pts of the next frame that will be generated */
		int64_t next_pts;
		int samples_count;

		AVFrame *frame;
		AVFrame *tmp_frame;

		float t, tincr, tincr2;

		struct SwsContext *sws_ctx_BGR24;
		struct SwsContext *sws_ctx_YUV420P;
		struct SwsContext *sws_ctx_Gray;
		struct SwrContext *swr_ctx;

	} OutputStream;
}
#endif