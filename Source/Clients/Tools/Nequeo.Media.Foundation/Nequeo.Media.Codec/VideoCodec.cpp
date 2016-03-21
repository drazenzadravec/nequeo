/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          VideoCodec.cpp
*  Purpose :       VideoCodec class.
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

using namespace Nequeo::Media;

// MP4 Encoder Methods.
unsigned long h264_profiles_array_size = 7UL;
unsigned long acc_profiles_array_size = 4UL;

H264ProfileInfo h264_profiles[] =
{
	{ eAVEncH264VProfile_Base,{ 15, 1 },{ 176, 144 },   128000 },
	{ eAVEncH264VProfile_Base,{ 15, 1 },{ 352, 288 },   384000 },
	{ eAVEncH264VProfile_Base,{ 30, 1 },{ 352, 288 },   384000 },
	{ eAVEncH264VProfile_Base,{ 29970, 1000 },{ 320, 240 },   528560 },
	{ eAVEncH264VProfile_Base,{ 15, 1 },{ 720, 576 },  4000000 },
	{ eAVEncH264VProfile_Main,{ 25, 1 },{ 720, 576 }, 10000000 },
	{ eAVEncH264VProfile_Main,{ 30, 1 },{ 352, 288 }, 10000000 },
};

AACProfileInfo aac_profiles[] =
{
	{ 96000, 2, 16, 24000, 0x29 },
	{ 48000, 2, 16, 24000, 0x29 },
	{ 44100, 2, 16, 16000, 0x29 },
	{ 44100, 2, 16, 12000, 0x29 },
};

#if defined(__cplusplus)
extern "C" {
	// MP3 Encoder Methods.
	UiConfig global_ui_config = { 0,0,0,0 };
	ReaderConfig global_reader = { sf_unknown, 0, 0, 0 };
	WriterConfig global_writer = { 0 };

	int const IFF_ID_FORM = 0x464f524d; /* "FORM" */
	int const IFF_ID_AIFF = 0x41494646; /* "AIFF" */
	int const IFF_ID_AIFC = 0x41494643; /* "AIFC" */
	int const IFF_ID_COMM = 0x434f4d4d; /* "COMM" */
	int const IFF_ID_SSND = 0x53534e44; /* "SSND" */
	int const IFF_ID_MPEG = 0x4d504547; /* "MPEG" */

	int const IFF_ID_NONE = 0x4e4f4e45; /* "NONE" *//* AIFF-C data format */
	int const IFF_ID_2CBE = 0x74776f73; /* "twos" *//* AIFF-C data format */
	int const IFF_ID_2CLE = 0x736f7774; /* "sowt" *//* AIFF-C data format */

	int const WAV_ID_RIFF = 0x52494646; /* "RIFF" */
	int const WAV_ID_WAVE = 0x57415645; /* "WAVE" */
	int const WAV_ID_FMT = 0x666d7420; /* "fmt " */
	int const WAV_ID_DATA = 0x64617461; /* "data" */

	RawPCMConfig global_raw_pcm =
	{
		/* in_bitwidth */ 16,
		/* in_signed   */ -1,
		/* in_endian   */ ByteOrderLittleEndian
	};
}
#endif