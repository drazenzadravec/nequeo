/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          VideoCodec.h
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

#pragma once

#ifndef _VIDEOCODEC_H
#define _VIDEOCODEC_H

#include "stdafx.h"

#include "lame.h"

#include "H264ProfileInfo.h"
#include "AACProfileInfo.h"
#include "CSession.h"

using namespace System;
using namespace Nequeo::Media;

// MP4 Encoder Methods.
extern unsigned long h264_profiles_array_size;
extern unsigned long acc_profiles_array_size;

extern H264ProfileInfo h264_profiles[];
extern AACProfileInfo aac_profiles[];

extern HRESULT EncodeFile(PCWSTR pszInput, PCWSTR pszOutput, int videoProfile, int audioProfile);
extern HRESULT CreateMediaSource(PCWSTR pszURL, IMFMediaSource **ppSource);
extern HRESULT GetSourceDuration(IMFMediaSource *pSource, MFTIME *pDuration);
extern HRESULT CreateTranscodeProfile(IMFTranscodeProfile **ppProfile, int videoProfile, int audioProfile);
extern HRESULT CreateH264Profile(DWORD index, IMFAttributes **ppAttributes);
extern HRESULT CreateAACProfile(DWORD index, IMFAttributes **ppAttributes);
extern HRESULT RunEncodingSession(CSession *pSession, MFTIME duration);

#if defined(__cplusplus)
extern "C" {

	#define INTERNAL_OPTS 0

	// MP3 Encoder Methods.
	extern int const IFF_ID_FORM;
	extern int const IFF_ID_AIFF;
	extern int const IFF_ID_AIFC;
	extern int const IFF_ID_COMM;
	extern int const IFF_ID_SSND;
	extern int const IFF_ID_MPEG;

	extern int const IFF_ID_NONE;
	extern int const IFF_ID_2CBE;
	extern int const IFF_ID_2CLE;

	extern int const WAV_ID_RIFF;
	extern int const WAV_ID_WAVE;
	extern int const WAV_ID_FMT;
	extern int const WAV_ID_DATA;

	typedef long double ieee854_float80_t;
	typedef double      ieee754_float64_t;
	typedef float       ieee754_float32_t;

	typedef struct blockAlign_struct {
		unsigned long offset;
		unsigned long blockSize;
	} blockAlign;

	typedef struct IFF_AIFF_struct {
		short   numChannels;
		unsigned long numSampleFrames;
		short   sampleSize;
		double  sampleRate;
		unsigned long sampleType;
		blockAlign blkAlgn;
	} IFF_AIFF;

	typedef struct UiConfig
	{
		int   silent;                   /* Verbosity */
		int   brhist;
		int   print_clipping_info;      /* print info whether waveform clips */
		float update_interval;          /* to use Frank's time status display */
	} UiConfig;

	struct PcmBuffer {
		void   *ch[2];           /* buffer for each channel */
		int     w;               /* sample width */
		int     n;               /* number samples allocated */
		int     u;               /* number samples used */
		int     skip_start;      /* number samples to ignore at the beginning */
		int     skip_end;        /* number samples to ignore at the end */
	};

	typedef void SNDFILE;
	typedef struct PcmBuffer PcmBuffer;

	typedef enum sound_file_format_e {
		sf_unknown,
		sf_raw,
		sf_wave,
		sf_aiff,
		sf_mp1,                  /* MPEG Layer 1, aka mpg */
		sf_mp2,                  /* MPEG Layer 2 */
		sf_mp3,                  /* MPEG Layer 3 */
		sf_mp123,                /* MPEG Layer 1,2 or 3; whatever .mp3, .mp2, .mp1 or .mpg contains */
		sf_ogg
	} sound_file_format;

	/* GLOBAL VARIABLES used by parse.c and main.c.
	instantiated in parce.c.  ugly, ugly */

	typedef struct ReaderConfig
	{
		sound_file_format input_format;
		int   swapbytes;                /* force byte swapping   default=0 */
		int   swap_channel;             /* 0: no-op, 1: swaps input channels */
		int   input_samplerate;
	} ReaderConfig;

	typedef struct WriterConfig
	{
		int   flush_write;
	} WriterConfig;

	typedef struct DecoderConfig
	{
		int   mp3_delay;                /* to adjust the number of samples truncated during decode */
		int   mp3_delay_set;            /* user specified the value of the mp3 encoder delay to assume for decoding */
		int   disable_wav_header;
		mp3data_struct mp3input_data;
	} DecoderConfig;

	typedef enum ByteOrder { ByteOrderLittleEndian, ByteOrderBigEndian } ByteOrder;

	enum ID3TAG_MODE
	{
		ID3TAG_MODE_DEFAULT,
		ID3TAG_MODE_V1_ONLY,
		ID3TAG_MODE_V2_ONLY
	};

	/* possible text encodings */
	typedef enum TextEncoding
	{
		TENC_RAW,     /* bytes will be stored as-is into ID3 tags, which are Latin1 per definition */
		TENC_LATIN1,  /* text will be converted from local encoding to Latin1, as ID3 needs it */
		TENC_UTF16   /* text will be converted from local encoding to Unicode, as ID3v2 wants it */
	} TextEncoding;

	typedef struct RawPCMConfig
	{
		int     in_bitwidth;
		int     in_signed;
		ByteOrder in_endian;
	} RawPCMConfig;

	typedef struct get_audio_global_data_struct {
		int     count_samples_carefully;
		int     pcmbitwidth;
		int     pcmswapbytes;
		int     pcm_is_unsigned_8bit;
		int     pcm_is_ieee_float;
		unsigned int num_samples_read;
		FILE   *music_in;
		SNDFILE *snd_file;
		hip_t   hip;
		PcmBuffer pcm32;
		PcmBuffer pcm16;
		size_t  in_id3v2_size;
		unsigned char* in_id3v2_tag;
	} get_audio_global_data;

	static DecoderConfig global_decoder;
	static get_audio_global_data global;
	static std::vector<char*> errorList;
	static int const internal_opts_enabled = INTERNAL_OPTS;

	extern UiConfig global_ui_config;
	extern ReaderConfig global_reader;
	extern WriterConfig global_writer;
	extern RawPCMConfig global_raw_pcm;

	extern std::vector<char*> GetErrorList();
	extern int LameMp3EncodeFile(const char *pszInput, const char *pszOutput);
	extern wchar_t* UTF8ToUnicode(const char *mbstr);
	extern char* UnicodeToUtf8(const wchar_t *wstr);
	extern char* LameMp3GetEnvironment(char const *var);
	extern wchar_t* MBSToUnicode(const char *mbstr, int code_page);
	extern char* UnicodeToMbs(const wchar_t *wstr, int code_page);
	extern int ParseArgsFromString(lame_global_flags *const gfp, const char *p, char *inPath, char *outPath);
	extern char* ToLatin1(char const* s);
	extern unsigned short* ToUtf16(char const* s);
	extern char* UTF8ToLatin1(char const* str);
	extern char* MbsToMbs(const char* str, int cp_from, int cp_to);
	extern unsigned short* UTF8ToUtf16(char const* mbstr);
	extern int ParseArgs(lame_global_flags * gfp, int argc, char **argv, char *const inPath, char *const outPath, char **nogap_inPath, int *num_nogap);
	extern void CloseInfile(void);
	extern int CloseInputFile(FILE *musicin);
	extern void FreePcmBuffer(PcmBuffer * b);
	extern int LameMp3EncodeLoop(lame_global_flags *gf, FILE *outf, int nogap, char *inPath, char *outPath);
	extern FILE* InitFiles(lame_global_flags *gf, char const *inPath, char const *outPath);
	extern int InitInputfile(lame_t gfp, char const *inPath);
	extern FILE* InitOutputFile(char const *outPath, int decode);
	extern FILE* LameMp3OpenFile(char const* file, char const* mode);
	extern int LameMp3SetStreamBinaryMode(FILE * const fp);
	extern void SetSkipStartAndEnd(lame_t gfp, int enc_delay, int enc_padding);
	extern void InitPcmBuffer(PcmBuffer *b, int w);
	extern FILE* OpenWaveFile(lame_t gfp, char const *inPath);
	extern FILE* OpenMpegFile(lame_t gfp, char const *inPath, int *enc_delay, int *enc_padding);
	extern off_t LameMp3GetFileSize(FILE *fp);
	extern int ParseFileHeader(lame_global_flags * gfp, FILE * sf);
	extern int ParseAiffHeader(lame_global_flags * gfp, FILE * sf);
	extern int ParseWaveHeader(lame_global_flags * gfp, FILE * sf);
	extern int read_32_bits_high_low(FILE * fp);
	extern int read_16_bits_high_low(FILE * fp);
	extern void write_16_bits_low_high(FILE * fp, int val);
	extern void write_32_bits_low_high(FILE * fp, int val);
	extern int read_32_bits_low_high(FILE * fp);
	extern int read_16_bits_low_high(FILE * fp);
	extern int fskip(FILE * fp, long offset, int whence);
	extern size_t min_size_t(size_t a, size_t b);
	extern long make_even_number_of_bytes_in_length(long x);
	extern int aiff_check2(IFF_AIFF * const pcm_aiff_data);
	extern double read_ieee_extended_high_low(FILE * fp);
	extern unsigned int uint32_high_low(unsigned char *bytes);
	extern int is_mpeg_file_format(int input_file_format);
	extern off_t lame_get_file_size(FILE * fp);
	extern unsigned char* getOldTag(lame_t gf);
	extern hip_t get_hip(void);
	extern size_t sizeOfOldTag(lame_t gf);
	extern int get_audio(lame_t gfp, int buffer[2][1152]);
	extern int get_audio_common(lame_t gfp, int buffer[2][1152], short buffer16[2][1152]);
	extern int addPcmBuffer(PcmBuffer * b, void *a0, void *a1, int read);
	extern int takePcmBuffer(PcmBuffer * b, void *a0, void *a1, int a_n, int mm);
	extern int read_samples_mp3(lame_t gfp, FILE * musicin, short int mpg123pcm[2][1152]);
	extern int lame_decode_fromfile(FILE * fd, short pcm_l[], short pcm_r[], mp3data_struct * mp3data);
	extern int read_samples_pcm(FILE * musicin, int sample_buffer[2304], int samples_to_read);
	extern int unpack_read_samples(const int samples_to_read, const int bytes_per_sample, const int swap_order, int *sample_buffer, FILE * pcm_in);
	extern int lame_decode_initfile(FILE * fd, mp3data_struct * mp3data, int *enc_delay, int *enc_padding);
	extern size_t lenOfId3v2Tag(unsigned char const* buf);
	extern int check_aid(const unsigned char *header);
	extern int is_syncword_mp123(const void *const headerptr);
	extern int write_id3v1_tag(lame_t gf, FILE * outf);
	extern int write_xing_frame(lame_global_flags * gf, FILE * outf, size_t offset);
	extern int filename_to_type(const char *FileName);
	extern int local_strcasecmp(const char *s1, const char *s2);
	extern int generateOutPath(lame_t gfp, char const* inPath, char const* outDir, char* outPath);
	extern size_t scanPath(char const* s, char const** a, char const** b);
	extern size_t scanBasename(char const* s, char const** a, char const** b);
	extern int isCommonSuffix(char const* s_ext);
	extern void dosToLongFileName(char *fn);
	extern int presets_set(lame_t gfp, int fast, int cbr, const char *preset_name, const char *ProgramName);
	extern int local_strncasecmp(const char *s1, const char *s2, int n);
	extern int resample_rate(double freq);
	extern void setProcessPriority(int Priority);
	extern BOOL SetPriorityClassMacro(DWORD p);
	extern int id3_tag(lame_global_flags* gfp, int type, TextEncoding enc, char* str);
	extern int set_id3v2tag(lame_global_flags* gfp, int type, unsigned short const* str);
	extern int set_id3tag(lame_global_flags* gfp, int type, char const* str);
	extern FILE* lame_fopen(char const* file, char const* mode);
	extern int set_id3_albumart(lame_t gfp, char const* file_name);
}
#endif
#endif