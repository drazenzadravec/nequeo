/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          VideoCodec.cpp
*  Purpose :       SIP VideoCodec class.
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

#include "stdafx.h"

#include "VideoCodec.h"

using namespace Nequeo::Media::FFmpeg;

int video_codecs[] =
{
	libffmpeg::AV_CODEC_ID_MPEG4,
	libffmpeg::AV_CODEC_ID_WMV1,
	libffmpeg::AV_CODEC_ID_WMV2,
	libffmpeg::AV_CODEC_ID_MSMPEG4V2,
	libffmpeg::AV_CODEC_ID_MSMPEG4V3,
	libffmpeg::AV_CODEC_ID_H263P,
	libffmpeg::AV_CODEC_ID_FLV1,
	libffmpeg::AV_CODEC_ID_MPEG2VIDEO,
	libffmpeg::AV_CODEC_ID_RAWVIDEO,
	libffmpeg::AV_CODEC_ID_H264,
	libffmpeg::AV_CODEC_ID_MP4ALS,
};

int pixel_formats[] =
{
	libffmpeg::AV_PIX_FMT_YUV420P,
	libffmpeg::AV_PIX_FMT_YUV420P,
	libffmpeg::AV_PIX_FMT_YUV420P,
	libffmpeg::AV_PIX_FMT_YUV420P,
	libffmpeg::AV_PIX_FMT_YUV420P,
	libffmpeg::AV_PIX_FMT_YUV420P,
	libffmpeg::AV_PIX_FMT_YUV420P,
	libffmpeg::AV_PIX_FMT_YUV420P,
	libffmpeg::AV_PIX_FMT_BGR24,
};

int CODECS_COUNT ( sizeof( video_codecs ) / sizeof( libffmpeg::AVCodecID) );