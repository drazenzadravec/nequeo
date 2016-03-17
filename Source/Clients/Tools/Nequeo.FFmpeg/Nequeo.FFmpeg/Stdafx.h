// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

#include <windows.h>

#define __STDC_CONSTANT_MACROS

namespace libffmpeg
{
	extern "C"
	{
		// disable warnings about badly formed documentation from FFmpeg, which we don't need at all
		#pragma warning(disable:4635) 
		// disable warning about conversion int64 to int32
		#pragma warning(disable:4244) 

		#include "inttypes.h";
		#include "stdint.h";

		#include "libavformat\avformat.h"
		#include "libavformat\avio.h"
		#include "libavcodec\avcodec.h"
		#include "libswscale\swscale.h"
	}
}