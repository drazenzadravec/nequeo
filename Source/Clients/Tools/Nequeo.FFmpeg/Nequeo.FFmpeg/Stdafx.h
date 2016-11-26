// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

#include <stdlib.h>
#include <windows.h>
#include <exception>
#include <vector>
#include <memory>

#define __STDC_CONSTANT_MACROS

namespace libffmpeg
{
	#if defined(__cplusplus)
	extern "C"
	{
		// disable warnings about badly formed documentation from FFmpeg, which we don't need at all
		#pragma warning(disable:4635)
		// disable warning about conversion int64 to int32
		#pragma warning(disable:4244)

		#define INT64_C(x) (x ## LL)
		#define UINT64_C(x) (x ## ULL)

		#include "inttypes.h";
		#include "stdint.h";

		#include "libavutil\channel_layout.h"
		#include "libavutil\common.h"
		#include "libavutil\intreadwrite.h"
		#include "libavutil\log.h"
		#include "libavutil\opt.h"
		#include "libavcodec\audio_frame_queue.h"
		#include "libavcodec\mpegaudio.h"
		#include "libavcodec\mpegaudiodecheader.h"
		#include "libavformat\avformat.h"
		#include "libavformat\avio.h"
		#include "libavcodec\avcodec.h"
		#include "libswscale\swscale.h"
		#include <libavutil\imgutils.h>
		#include <libavutil\samplefmt.h>
		#include <libswresample\swresample.h>
		#include <libavfilter/avfiltergraph.h>
		#include <libavfilter/buffersink.h>
		#include <libavfilter/buffersrc.h>
		#include <libavutil/pixdesc.h>
		#include <libavdevice\avdevice.h>

		#include "lame.h"
	}
	#endif
}