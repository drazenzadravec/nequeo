/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CMp3Encoder.cpp
*  Purpose :       CMp3Encoder class.
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

#include "fcntl.h"
#include "corecrt_io.h"

#define SLASH '\\'
#define PATH_MAX 1024
#define MAX_NOGAP 200
#define ID3TAGS_EXTENDED
#define MAX_U_32_NUM 0xFFFFFFFF
#define FLOAT_TO_UNSIGNED(f) ((unsigned long)(((long)((f) - 2147483648.0)) + 2147483647L + 1))
#define UNSIGNED_TO_FLOAT(u) (((double)((long)((u) - 2147483647L - 1))) + 2147483648.0)
#define dimension_of(array) (sizeof(array)/sizeof(array[0]))

/* Ugly, NOT final version */
#define T_IF(str)          if ( 0 == local_strcasecmp (token,str) ) {
#define T_ELIF(str)        } else if ( 0 == local_strcasecmp (token,str) ) {
#define T_ELIF_INTERNAL(str) } else if (internal_opts_enabled && (0 == local_strcasecmp (token,str)) ) {
#define T_ELIF2(str1,str2) } else if ( 0 == local_strcasecmp (token,str1)  ||  0 == local_strcasecmp (token,str2) ) {
#define T_ELSE             } else {
#define T_END              }

using namespace Nequeo::Media;

/// <summary>
/// Get the error list.
/// </summary>
/// <returns>The result.</returns>
std::vector<char*> GetErrorList()
{
	return errorList;
}

/// <summary>
/// Encode the file.
/// </summary>
/// <param name="pszInput">The input file to convert.</param>
/// <param name="pszOutput">The mp4 encoded file.</param>
/// <returns>The result.</returns>
int LameMp3EncodeFile(const char *pszInput, const char *pszOutput)
{
	lame_t gf;
	int ret;
	int i;
	FILE *outf;
	char **argv;

	// Clear the error list.
	errorList.clear();

	// Initialze lame MP3 encoder.
	gf = lame_init();
	if (gf != NULL)
	{
		char inPath[PATH_MAX + 1];
		char outPath[PATH_MAX + 1];
		char nogapdir[PATH_MAX + 1];

		int nogapout = 0;
		int max_nogap = MAX_NOGAP;
		char nogap_inPath_[MAX_NOGAP][PATH_MAX + 1];
		char *nogap_inPath[MAX_NOGAP];

		// Allocate memory.
		memset(inPath, 0, sizeof(inPath));
		memset(nogap_inPath_, 0, sizeof(nogap_inPath_));
		for (i = 0; i < MAX_NOGAP; ++i) {
			nogap_inPath[i] = &nogap_inPath_[i][0];
		}

		/* parse the command line arguments, setting various flags in the
		* struct 'gf'.  If you want to parse your own arguments,
		* or call libmp3lame from a program which uses a GUI to set arguments,
		* skip this call and set the values of interest in the gf struct.
		* (see the file API and lame.h for documentation about these parameters)
		*/
		{
			char *str = LameMp3GetEnvironment("LAMEOPT");
			ParseArgsFromString(gf, str, inPath, outPath);
			free(str);
		}

		// Assign the array.
		const int argc = 3;
		argv = new char*[argc];

		argv[0] = "Nequeo.MP3.Encoder";
		argv[1] = (char *)pszInput;
		argv[2] = (char *)pszOutput;

		// Parse any argument options.
		ret = ParseArgs(gf, argc, argv, inPath, outPath, nogap_inPath, &max_nogap);

		if (global_ui_config.update_interval < 0.)
			global_ui_config.update_interval = 2.;

		if (outPath[0] != '\0' && max_nogap > 0) {
			strncpy(nogapdir, outPath, PATH_MAX + 1);
			nogapout = 1;
		}

		/* initialize input file.  This also sets samplerate and as much
		other data on the input file as available in the headers */
		outf = InitFiles(gf, inPath, outPath);

		if (outf == NULL) {
			ret = -1;
		}

		// If all is ok.
		if (ret >= 0)
		{
			/* turn off automatic writing of ID3 tag data into mp3 stream
			* we have to call it before 'lame_init_params', because that
			* function would spit out ID3v2 tag data.
			*/
			lame_set_write_id3tag_automatic(gf, 0);

			/* Now that all the options are set, lame needs to analyze them and
			* set some more internal options and check for problems
			*/
			ret = lame_init_params(gf);
			if (ret < 0)
			{
				errorList.push_back("Fatal error during initialization.\r\n");
			}
			else
			{
				/* turn off VBR histogram */
				if (global_ui_config.silent > 0) {
					global_ui_config.brhist = 0; /* turn off VBR histogram */
				}

				/* encode a single input file */
				if (max_nogap == 0)
				{
					// Start encoding the mp3 file.
					ret = LameMp3EncodeLoop(gf, outf, max_nogap, inPath, outPath);

					/* close the output file */
					fclose(outf);

					/* close the input file */
					CloseInfile();
				}
			}
		}

		// Close the encoder.
		lame_close(gf);

		if (argv != NULL)
		{
			// Delete the top level array.
			delete[] argv;
		}
	}
	else
	{
		errorList.push_back("Fatal error during initialization.\r\n");
		ret = -1;
	}

	// Return the result.
	return ret;
}

/// <summary>
/// Encode the file.
/// </summary>
/// <param name="gf">The global lame struct.</param>
/// <param name="outf">The output file information.</param>
/// <param name="nogap">No gap equals zero then only encode one file.</param>
/// <param name="inPath">The input file to convert.</param>
/// <param name="outPath">The mp4 encoded file.</param>
/// <returns>The result.</returns>
int LameMp3EncodeLoop(lame_global_flags *gf, FILE *outf, int nogap, char *inPath, char *outPath)
{
	unsigned char mp3buffer[LAME_MAXMP3BUFFER];
	int     Buffer[2][1152];
	int     iread, imp3, owrite;
	size_t  id3v2_size;

	id3v2_size = lame_get_id3v2_tag(gf, 0, 0);
	if (id3v2_size > 0) {
		unsigned char *id3v2tag = (unsigned char *)malloc(id3v2_size);
		if (id3v2tag != 0) {
			imp3 = lame_get_id3v2_tag(gf, id3v2tag, id3v2_size);
			owrite = (int)fwrite(id3v2tag, 1, imp3, outf);
			free(id3v2tag);
			if (owrite != imp3) {
				errorList.push_back("Error writing ID3v2 tag.\r\n");
				return 1;
			}
		}
	}
	else {
		unsigned char* id3v2tag = getOldTag(gf);
		id3v2_size = sizeOfOldTag(gf);
		if (id3v2_size > 0) {
			size_t owrite = fwrite(id3v2tag, 1, id3v2_size, outf);
			if (owrite != id3v2_size) {
				errorList.push_back("Error writing ID3v2 tag.\r\n");
				return 1;
			}
		}
	}
	if (global_writer.flush_write == 1) {
		fflush(outf);
	}

	/* encode until we hit eof */
	do {
		/* read in 'iread' samples */
		iread = get_audio(gf, Buffer);

		if (iread >= 0) {

			/* encode */
			imp3 = lame_encode_buffer_int(gf, Buffer[0], Buffer[1], iread,
				mp3buffer, sizeof(mp3buffer));

			/* was our output buffer big enough? */
			if (imp3 < 0) {
				if (imp3 == -1)
					errorList.push_back("mp3 buffer is not big enough...\r\n");
				else
					errorList.push_back("mp3 internal error:  error code.\r\n");
				return 1;
			}
			owrite = (int)fwrite(mp3buffer, 1, imp3, outf);
			if (owrite != imp3) {
				errorList.push_back("Error writing mp3 output.\r\n");
				return 1;
			}
		}
		if (global_writer.flush_write == 1) {
			fflush(outf);
		}
	} while (iread > 0);

	if (nogap)
		imp3 = lame_encode_flush_nogap(gf, mp3buffer, sizeof(mp3buffer)); /* may return one more mp3 frame */
	else
		imp3 = lame_encode_flush(gf, mp3buffer, sizeof(mp3buffer)); /* may return one more mp3 frame */

	if (imp3 < 0) {
		if (imp3 == -1)
			errorList.push_back("mp3 buffer is not big enough...\r\n");
		else
			errorList.push_back("mp3 internal error:  error code.\r\n");
		return 1;

	}

	owrite = (int)fwrite(mp3buffer, 1, imp3, outf);
	if (owrite != imp3) {
		errorList.push_back("Error writing mp3 output.\r\n");
		return 1;
	}
	if (global_writer.flush_write == 1) {
		fflush(outf);
	}
	imp3 = write_id3v1_tag(gf, outf);
	if (global_writer.flush_write == 1) {
		fflush(outf);
	}
	if (imp3) {
		return 1;
	}
	write_xing_frame(gf, outf, id3v2_size);
	if (global_writer.flush_write == 1) {
		fflush(outf);
	}
	
	return 0;
}

/// <summary>
/// Close in file.
/// </summary>
void CloseInfile(void)
{
	// Close the input file
	CloseInputFile(global.music_in);

	// Free memory.
	FreePcmBuffer(&global.pcm32);
	FreePcmBuffer(&global.pcm16);
	global.music_in = 0;
	free(global.in_id3v2_tag);
	global.in_id3v2_tag = 0;
	global.in_id3v2_size = 0;
}

/// <summary>
/// Close input file.
/// </summary>
/// <param name="musicin">The input file information..</param>
int CloseInputFile(FILE *musicin)
{
	int     ret = 0;

	if (musicin != stdin && musicin != 0) {
		ret = fclose(musicin);
	}
	if (ret != 0) {
		if (global_ui_config.silent < 10) {
			errorList.push_back("Could not close audio input file.\r\n");
		}
	}
	return ret;
}

/// <summary>
/// Free the pcm memory buffer.
/// </summary>
void FreePcmBuffer(PcmBuffer * b)
{
	if (b != 0) {
		free(b->ch[0]);
		free(b->ch[1]);
		b->ch[0] = 0;
		b->ch[1] = 0;
		b->n = 0;
		b->u = 0;
	}
}

/// <summary>
/// UTF8 to unicode.
/// </summary>
/// <param name="var">The UFT8 value.</param>
/// <returns>The result.</returns>
wchar_t* UTF8ToUnicode(const char *mbstr)
{
	return MBSToUnicode(mbstr, CP_UTF8);
}

/// <summary>
/// Unicode to UTF8.
/// </summary>
/// <param name="var">The unicode value.</param>
/// <returns>The result.</returns>
char* UnicodeToUtf8(const wchar_t *wstr)
{
	return UnicodeToMbs(wstr, CP_UTF8);
}

/// <summary>
/// UTF8 to unicode.
/// </summary>
/// <param name="mbstr">The UFT8 value.</param>
/// <param name="code_page">The UFT8 code page.</param>
/// <returns>The result.</returns>
wchar_t* MBSToUnicode(const char *mbstr, int code_page)
{
	int n = MultiByteToWideChar(code_page, 0, mbstr, -1, NULL, 0);
	wchar_t* wstr;

	// Allocate the memory.
	wstr = (wchar_t*)malloc(n*sizeof(wstr[0]));

	if (wstr != 0) {
		n = MultiByteToWideChar(code_page, 0, mbstr, -1, wstr, n);
		if (n == 0) {
			free(wstr);
			wstr = 0;
		}
	}

	// Return the unicode string.
	return wstr;
}

/// <summary>
/// Unicode to UTF8.
/// </summary>
/// <param name="var">The unicode value.</param>
/// <param name="code_page">The UFT8 code page.</param>
/// <returns>The result.</returns>
char* UnicodeToMbs(const wchar_t *wstr, int code_page)
{
	int n = 1 + WideCharToMultiByte(code_page, 0, wstr, -1, 0, 0, 0, 0);
	char* mbstr;

	// Allocate the memory.
	mbstr = (char*)malloc(n*sizeof(mbstr[0]));

	if (mbstr != 0) {
		n = WideCharToMultiByte(code_page, 0, wstr, -1, mbstr, n, 0, 0);
		if (n == 0) {
			free(mbstr);
			mbstr = 0;
		}
	}

	// Return the UTF8 string.
	return mbstr;
}

/// <summary>
/// MP3 get environment.
/// </summary>
/// <param name="var">The the enviornment.</param>
/// <returns>The result.</returns>
char* LameMp3GetEnvironment(char const* var)
{
	char* str = 0;

	// Get the utf 8 to unicode.
	wchar_t* wvar = UTF8ToUnicode(var);
	wchar_t* wstr = 0;

	// Get the utf 8 to unicode.
	if (wvar != 0) {
		wstr = _wgetenv(wvar);
		str = UnicodeToUtf8(wstr);
	}

	// Free the memory.
	free(wvar);
	free(wstr);

	// Return the value.
	return str;
}

/// <summary>
/// Parse args from string.
/// </summary>
/// <param name="gfp">The global lame.</param>
/// <param name="p">The string.</param>
/// <param name="inPath">The in path.</param>
/// <param name="outPath">The out path.</param>
/// <returns>The result.</returns>
int ParseArgsFromString(lame_global_flags *const gfp, const char *p, char *inPath, char *outPath)
{
	char   *q;
	char   *f;
	char   *r[128];
	int     c = 0;
	int     ret;

	if (p == NULL || *p == '\0')
		return 0;

	// Allocate memory.
	q = (char*)malloc(strlen(p) + 1);
	f = q;

	// Copy the sttring.
	strcpy(q, p);

	r[c++] = "lhama";
	for (;;) {
		r[c++] = q;
		while (*q != ' ' && *q != '\0')
			q++;
		if (*q == '\0')
			break;
		*q++ = '\0';
	}
	r[c] = NULL;

	ret = ParseArgs(gfp, c, r, inPath, outPath, NULL, NULL);
	free(f);

	// Return the result.
	return ret;
}

/// <summary>
/// Convert to latin1.
/// </summary>
/// <param name="s">The value.</param>
/// <returns>The result.</returns>
char* ToLatin1(char const* s)
{
	return UTF8ToLatin1(s);
}

/// <summary>
/// Convert to UFT16.
/// </summary>
/// <param name="s">The value.</param>
/// <returns>The result.</returns>
unsigned short* ToUtf16(char const* s)
{
	return UTF8ToUtf16(s);
}

/// <summary>
/// Convert to latin1.
/// </summary>
/// <param name="str">The value.</param>
/// <returns>The result.</returns>
char* UTF8ToLatin1(char const* str)
{
	return MbsToMbs(str, CP_UTF8, 28591); /* Latin-1 is code page 28591 */
}

/// <summary>
/// Convert to mbs.
/// </summary>
/// <param name="str">The value.</param>
/// <param name="cp_from">From.</param>
/// <param name="cp_to">To.</param>
/// <returns>The result.</returns>
char* MbsToMbs(const char* str, int cp_from, int cp_to)
{
	wchar_t* wstr = MBSToUnicode(str, cp_from);

	if (wstr != 0) {
		char* local8bit = UnicodeToMbs(wstr, cp_to);
		free(wstr);
		return local8bit;
	}
	return 0;
}

/// <summary>
/// Convert to UFT16.
/// </summary>
/// <param name="s">The value.</param>
/// <returns>The result.</returns>
unsigned short* UTF8ToUtf16(char const* mbstr)
{
	int n = MultiByteToWideChar(CP_UTF8, 0, mbstr, -1, NULL, 0);
	wchar_t* wstr;

	// Allocate memory.
	wstr = (wchar_t*)malloc((n + 1)*sizeof(wstr[0]));

	if (wstr != 0) {
		wstr[0] = 0xfeff; /* BOM */
		n = MultiByteToWideChar(CP_UTF8, 0, mbstr, -1, wstr + 1, n);
		if (n == 0) {
			free(wstr);
			wstr = 0;
		}
	}

	// Return the value.
	return (unsigned short*)wstr;
}

/// <summary>
/// Parse arguments.
/// </summary>
/// <param name="gfp">The value.</param>
/// <param name="argc">The value.</param>
/// <param name="argv">The value.</param>
/// <param name="inPath">The value.</param>
/// <param name="outPath">The value.</param>
/// <param name="nogap_inPath">The value.</param>
/// <param name="num_nogap">The value.</param>
/// <returns>The result.</returns>
int ParseArgs(lame_global_flags *gfp, int argc, char **argv, char *const inPath, char *const outPath, char **nogap_inPath, int *num_nogap)
{
	char    outDir[1024] = "";
	int     input_file = 0;  /* set to 1 if we parse an input file name  */
	int     i;
	int     autoconvert = 0;
	double  val;
	int     nogap = 0;
	int     nogap_tags = 0;  /* set to 1 to use VBR tags in NOGAP mode */
	const char *ProgramName = argv[0];
	int     count_nogap = 0;
	int     noreplaygain = 0; /* is RG explicitly disabled by the user */
	int     id3tag_mode = ID3TAG_MODE_DEFAULT;
	int     ignore_tag_errors = 0;  /* Ignore errors in values passed for tags */
#ifdef ID3TAGS_EXTENDED
	enum TextEncoding id3_tenc = TENC_UTF16;
#else
	enum TextEncoding id3_tenc = TENC_LATIN1;
#endif

	inPath[0] = '\0';
	outPath[0] = '\0';
	/* turn on display options. user settings may turn them off below */
	global_ui_config.silent = 0;
	global_ui_config.brhist = 1;
	global_decoder.mp3_delay = 0;
	global_decoder.mp3_delay_set = 0;
	global_decoder.disable_wav_header = 0;
	global_ui_config.print_clipping_info = 0;
	id3tag_init(gfp);

	/* process args */
	for (i = 0; ++i < argc;)
	{
		char    c;
		char   *token;
		char   *arg;
		char   *nextArg;
		int     argUsed;

		token = argv[i];
		if (*token++ == '-') {
			argUsed = 0;
			nextArg = i + 1 < argc ? argv[i + 1] : "";

			if (!*token) { /* The user wants to use stdin and/or stdout. */
				input_file = 1;
				if (inPath[0] == '\0')
					strncpy(inPath, argv[i], PATH_MAX + 1);
				else if (outPath[0] == '\0')
					strncpy(outPath, argv[i], PATH_MAX + 1);
			}
			if (*token == '-') { /* GNU style */
				token++;

				T_IF("resample")
					argUsed = 1;
				(void)lame_set_out_samplerate(gfp, resample_rate(atof(nextArg)));

				T_ELIF("vbr-old")
					lame_set_VBR(gfp, vbr_rh);

				T_ELIF("vbr-new")
					lame_set_VBR(gfp, vbr_mt);

				T_ELIF("vbr-mtrh")
					lame_set_VBR(gfp, vbr_mtrh);

				T_ELIF("cbr")
					lame_set_VBR(gfp, vbr_off);

				T_ELIF("abr")
					/* values larger than 8000 are bps (like Fraunhofer), so it's strange to get 320000 bps MP3 when specifying 8000 bps MP3 */
					int m = atoi(nextArg);
				argUsed = 1;
				if (m >= 8000) {
					m = (m + 500) / 1000;
				}
				if (m > 320) {
					m = 320;
				}
				if (m < 8) {
					m = 8;
				}
				lame_set_VBR(gfp, vbr_abr);
				lame_set_VBR_mean_bitrate_kbps(gfp, m);

				T_ELIF("r3mix")
					lame_set_preset(gfp, R3MIX);

				T_ELIF("bitwidth")
					argUsed = 1;
				global_raw_pcm.in_bitwidth = atoi(nextArg);

				T_ELIF("signed")
					global_raw_pcm.in_signed = 1;

				T_ELIF("unsigned")
					global_raw_pcm.in_signed = 0;

				T_ELIF("little-endian")
					global_raw_pcm.in_endian = ByteOrderLittleEndian;

				T_ELIF("big-endian")
					global_raw_pcm.in_endian = ByteOrderBigEndian;

				T_ELIF("mp1input")
					global_reader.input_format = sf_mp1;

				T_ELIF("mp2input")
					global_reader.input_format = sf_mp2;

				T_ELIF("mp3input")
					global_reader.input_format = sf_mp3;

				T_ELIF("ogginput")
					errorList.push_back("Sorry, vorbis support in LAME is deprecated.\r\n");
				return -1;
#if INTERNAL_OPTS
				T_ELIF_INTERNAL("noshort")
					(void) lame_set_no_short_blocks(gfp, 1);

				T_ELIF_INTERNAL("short")
					(void) lame_set_no_short_blocks(gfp, 0);

				T_ELIF_INTERNAL("allshort")
					(void) lame_set_force_short_blocks(gfp, 1);
#endif

				T_ELIF("decode")
					(void) lame_set_decode_only(gfp, 1);

				T_ELIF("flush")
					global_writer.flush_write = 1;

				T_ELIF("decode-mp3delay")
					global_decoder.mp3_delay = atoi(nextArg);
				global_decoder.mp3_delay_set = 1;
				argUsed = 1;

				T_ELIF("nores")
					lame_set_disable_reservoir(gfp, 1);

				T_ELIF("strictly-enforce-ISO")
					lame_set_strict_ISO(gfp, MDB_STRICT_ISO);

				T_ELIF("buffer-constraint")
					argUsed = 1;
				if (strcmp(nextArg, "default") == 0)
					(void) lame_set_strict_ISO(gfp, MDB_DEFAULT);
				else if (strcmp(nextArg, "strict") == 0)
					(void) lame_set_strict_ISO(gfp, MDB_STRICT_ISO);
				else if (strcmp(nextArg, "maximum") == 0)
					(void) lame_set_strict_ISO(gfp, MDB_MAXIMUM);
				else {
					errorList.push_back("unknown buffer constraint \r\n");
					return -1;
				}

				T_ELIF("scale")
					argUsed = 1;
				(void)lame_set_scale(gfp, (float)atof(nextArg));

				T_ELIF("scale-l")
					argUsed = 1;
				(void)lame_set_scale_left(gfp, (float)atof(nextArg));

				T_ELIF("scale-r")
					argUsed = 1;
				(void)lame_set_scale_right(gfp, (float)atof(nextArg));

				T_ELIF("noasm")
					argUsed = 1;
				if (!strcmp(nextArg, "mmx"))
					(void) lame_set_asm_optimizations(gfp, MMX, 0);
				if (!strcmp(nextArg, "3dnow"))
					(void) lame_set_asm_optimizations(gfp, AMD_3DNOW, 0);
				if (!strcmp(nextArg, "sse"))
					(void) lame_set_asm_optimizations(gfp, SSE, 0);

				T_ELIF("freeformat")
					lame_set_free_format(gfp, 1);

				T_ELIF("replaygain-fast")
					lame_set_findReplayGain(gfp, 1);

#ifdef DECODE_ON_THE_FLY
				T_ELIF("replaygain-accurate")
					lame_set_decode_on_the_fly(gfp, 1);
				lame_set_findReplayGain(gfp, 1);
#endif

				T_ELIF("noreplaygain")
					noreplaygain = 1;
				lame_set_findReplayGain(gfp, 0);


#ifdef DECODE_ON_THE_FLY
				T_ELIF("clipdetect")
					global_ui_config.print_clipping_info = 1;
				lame_set_decode_on_the_fly(gfp, 1);
#endif

				T_ELIF("nohist")
					global_ui_config.brhist = 0;

#if defined(__OS2__) || defined(WIN32)
				T_ELIF("priority")
					char   *endptr;
				int     priority = (int)strtol(nextArg, &endptr, 10);
				if (endptr != nextArg) {
					argUsed = 1;
				}
				setProcessPriority(priority);
#endif

				/* options for ID3 tag */
#ifdef ID3TAGS_EXTENDED
				T_ELIF2("id3v2-utf16", "id3v2-ucs2") /* id3v2-ucs2 for compatibility only */
					id3_tenc = TENC_UTF16;
				id3tag_add_v2(gfp);

				T_ELIF("id3v2-latin1")
					id3_tenc = TENC_LATIN1;
				id3tag_add_v2(gfp);
#endif

				T_ELIF("tt")
					argUsed = 1;
				id3_tag(gfp, 't', id3_tenc, nextArg);

				T_ELIF("ta")
					argUsed = 1;
				id3_tag(gfp, 'a', id3_tenc, nextArg);

				T_ELIF("tl")
					argUsed = 1;
				id3_tag(gfp, 'l', id3_tenc, nextArg);

				T_ELIF("ty")
					argUsed = 1;
				id3_tag(gfp, 'y', id3_tenc, nextArg);

				T_ELIF("tc")
					argUsed = 1;
				id3_tag(gfp, 'c', id3_tenc, nextArg);

				T_ELIF("tn")
					int ret = id3_tag(gfp, 'n', id3_tenc, nextArg);
				argUsed = 1;
				if (ret != 0) {
					if (0 == ignore_tag_errors) {
						if (id3tag_mode == ID3TAG_MODE_V1_ONLY) {
							if (global_ui_config.silent < 9) {
								errorList.push_back("The track number has to be between 1 and 255 for ID3v1.\r\n");
							}
							return -1;
						}
						else if (id3tag_mode == ID3TAG_MODE_V2_ONLY) {
							/* track will be stored as-is in ID3v2 case, so no problem here */
						}
						else {
							if (global_ui_config.silent < 9) {
								errorList.push_back("The track number has to be between 1 and 255 for ID3v1, ignored for ID3v1.\r\n");
							}
						}
					}
				}

				T_ELIF("tg")
					int ret = id3_tag(gfp, 'g', id3_tenc, nextArg);
				argUsed = 1;
				if (ret != 0) {
					if (0 == ignore_tag_errors) {
						if (ret == -1) {
							errorList.push_back("Unknown ID3v1 genre number: \r\n");
							return -1;
						}
						else if (ret == -2) {
							if (id3tag_mode == ID3TAG_MODE_V1_ONLY) {
								errorList.push_back("Unknown ID3v1 genre: \r\n");
								return -1;
							}
							else if (id3tag_mode == ID3TAG_MODE_V2_ONLY) {
								/* genre will be stored as-is in ID3v2 case, so no problem here */
							}
							else {
								if (global_ui_config.silent < 9) {
									errorList.push_back("Unknown ID3v1 genre:. Setting ID3v1 genre to 'Other'\r\n");
								}
							}
						}
						else {
							if (global_ui_config.silent < 10)
								errorList.push_back("Internal error.\r\n");
							return -1;
						}
					}
				}

				T_ELIF("tv")
					argUsed = 1;
				if (id3_tag(gfp, 'v', id3_tenc, nextArg)) {
					if (global_ui_config.silent < 9) {
						errorList.push_back("Invalid field value: Ignored\r\n");
					}
				}

				T_ELIF("ti")
					argUsed = 1;
				if (set_id3_albumart(gfp, nextArg) != 0) {
					if (!ignore_tag_errors) {
						return -1;
					}
				}

				T_ELIF("ignore-tag-errors")
					ignore_tag_errors = 1;

				T_ELIF("add-id3v2")
					id3tag_add_v2(gfp);

				T_ELIF("id3v1-only")
					id3tag_v1_only(gfp);
				id3tag_mode = ID3TAG_MODE_V1_ONLY;

				T_ELIF("id3v2-only")
					id3tag_v2_only(gfp);
				id3tag_mode = ID3TAG_MODE_V2_ONLY;

				T_ELIF("space-id3v1")
					id3tag_space_v1(gfp);

				T_ELIF("pad-id3v2")
					id3tag_pad_v2(gfp);

				T_ELIF("pad-id3v2-size")
					int n = atoi(nextArg);
				n = n <= 128000 ? n : 128000;
				n = n >= 0 ? n : 0;
				id3tag_set_pad(gfp, n);
				argUsed = 1;


				//T_ELIF("genre-list")
					//id3tag_genre_list(genre_list_handler, NULL);

				return -2;


				T_ELIF("lowpass")
					val = atof(nextArg);
				argUsed = 1;
				if (val < 0) {
					lame_set_lowpassfreq(gfp, -1);
				}
				else {
					/* useful are 0.001 kHz...50 kHz, 50 Hz...50000 Hz */
					if (val < 0.001 || val > 50000.) {
						errorList.push_back("Must specify lowpass with --lowpass freq, freq >= 0.001 kHz\r\n");
						return -1;
					}
					lame_set_lowpassfreq(gfp, (int)(val * (val < 50. ? 1.e3 : 1.e0) + 0.5));
				}

				T_ELIF("lowpass-width")
					val = atof(nextArg);
				argUsed = 1;
				/* useful are 0.001 kHz...16 kHz, 16 Hz...50000 Hz */
				if (val < 0.001 || val > 50000.) {
					errorList.push_back
						("Must specify lowpass width with --lowpass-width freq, freq >= 0.001 kHz\r\n");
					return -1;
				}
				lame_set_lowpasswidth(gfp, (int)(val * (val < 16. ? 1.e3 : 1.e0) + 0.5));

				T_ELIF("highpass")
					val = atof(nextArg);
				argUsed = 1;
				if (val < 0.0) {
					lame_set_highpassfreq(gfp, -1);
				}
				else {
					/* useful are 0.001 kHz...16 kHz, 16 Hz...50000 Hz */
					if (val < 0.001 || val > 50000.) {
						errorList.push_back("Must specify highpass with --highpass freq, freq >= 0.001 kHz\r\n");
						return -1;
					}
					lame_set_highpassfreq(gfp, (int)(val * (val < 16. ? 1.e3 : 1.e0) + 0.5));
				}

				T_ELIF("highpass-width")
					val = atof(nextArg);
				argUsed = 1;
				/* useful are 0.001 kHz...16 kHz, 16 Hz...50000 Hz */
				if (val < 0.001 || val > 50000.) {
					errorList.push_back
						("Must specify highpass width with --highpass-width freq, freq >= 0.001 kHz\r\n");
					return -1;
				}
				lame_set_highpasswidth(gfp, (int)val);

				T_ELIF("comp")
					argUsed = 1;
				val = atof(nextArg);
				if (val < 1.0) {
					errorList.push_back("Must specify compression ratio >= 1.0\r\n");
					return -1;
				}
				lame_set_compression_ratio(gfp, (float)val);
#if INTERNAL_OPTS
				T_ELIF_INTERNAL("notemp")
					(void) lame_set_useTemporal(gfp, 0);

				T_ELIF_INTERNAL("interch")
					argUsed = 1;
				(void)lame_set_interChRatio(gfp, (float)atof(nextArg));

				T_ELIF_INTERNAL("temporal-masking")
					argUsed = 1;
				(void)lame_set_useTemporal(gfp, atoi(nextArg) ? 1 : 0);

				T_ELIF_INTERNAL("nspsytune")
					;

				T_ELIF_INTERNAL("nssafejoint")
					lame_set_exp_nspsytune(gfp, lame_get_exp_nspsytune(gfp) | 2);

				T_ELIF_INTERNAL("nsmsfix")
					argUsed = 1;
				(void)lame_set_msfix(gfp, atof(nextArg));

				T_ELIF_INTERNAL("ns-bass")
					argUsed = 1;
				{
					double  d;
					int     k;
					d = atof(nextArg);
					k = (int)(d * 4);
					if (k < -32)
						k = -32;
					if (k > 31)
						k = 31;
					if (k < 0)
						k += 64;
					lame_set_exp_nspsytune(gfp, lame_get_exp_nspsytune(gfp) | (k << 2));
				}

				T_ELIF_INTERNAL("ns-alto")
					argUsed = 1;
				{
					double  d;
					int     k;
					d = atof(nextArg);
					k = (int)(d * 4);
					if (k < -32)
						k = -32;
					if (k > 31)
						k = 31;
					if (k < 0)
						k += 64;
					lame_set_exp_nspsytune(gfp, lame_get_exp_nspsytune(gfp) | (k << 8));
				}

				T_ELIF_INTERNAL("ns-treble")
					argUsed = 1;
				{
					double  d;
					int     k;
					d = atof(nextArg);
					k = (int)(d * 4);
					if (k < -32)
						k = -32;
					if (k > 31)
						k = 31;
					if (k < 0)
						k += 64;
					lame_set_exp_nspsytune(gfp, lame_get_exp_nspsytune(gfp) | (k << 14));
				}

				T_ELIF_INTERNAL("ns-sfb21")
					/*  to be compatible with Naoki's original code,
					*  ns-sfb21 specifies how to change ns-treble for sfb21 */
					argUsed = 1;
				{
					double  d;
					int     k;
					d = atof(nextArg);
					k = (int)(d * 4);
					if (k < -32)
						k = -32;
					if (k > 31)
						k = 31;
					if (k < 0)
						k += 64;
					lame_set_exp_nspsytune(gfp, lame_get_exp_nspsytune(gfp) | (k << 20));
				}
#endif
				/* some more GNU-ish options could be added
				* brief         => few messages on screen (name, status report)
				* o/output file => specifies output filename
				* O             => stdout
				* i/input file  => specifies input filename
				* I             => stdin
				*/
				T_ELIF("quiet")
					global_ui_config.silent = 10; /* on a scale from 1 to 10 be very silent */

				T_ELIF("silent")
					global_ui_config.silent = 9;

				T_ELIF("brief")
					global_ui_config.silent = -5; /* print few info on screen */

				T_ELIF("verbose")
					global_ui_config.silent = -10; /* print a lot on screen */

				//T_ELIF2("version", "license")
					//print_license(stdout);

				return -2;

				T_ELIF2("help", "usage")
					if (0 == local_strncasecmp(nextArg, "id3", 3)) {
						//help_id3tag(stdout);
					}
					else if (0 == local_strncasecmp(nextArg, "dev", 3)) {
						//help_developer_switches(stdout);
					}
					else {
						//short_help(gfp, stdout, ProgramName);
					}
					return -2;

					//T_ELIF("longhelp")
						//long_help(gfp, stdout, ProgramName, 0 /* lessmode=NO */);

					return -2;

					T_ELIF("?")
#ifdef __unix__
						FILE   *fp = popen("less -Mqc", "w");
					long_help(gfp, fp, ProgramName, 0 /* lessmode=NO */);
					pclose(fp);
#else
					// long_help(gfp, stdout, ProgramName, 1 /* lessmode=YES */);
#endif
					return -2;

					T_ELIF2("preset", "alt-preset")
						argUsed = 1;
					{
						int     fast = 0, cbr = 0;

						while ((strcmp(nextArg, "fast") == 0) || (strcmp(nextArg, "cbr") == 0)) {

							if ((strcmp(nextArg, "fast") == 0) && (fast < 1))
								fast = 1;
							if ((strcmp(nextArg, "cbr") == 0) && (cbr < 1))
								cbr = 1;

							argUsed++;
							nextArg = i + argUsed < argc ? argv[i + argUsed] : "";
						}

						if (presets_set(gfp, fast, cbr, nextArg, ProgramName) < 0)
							return -1;
					}

					T_ELIF("disptime")
						argUsed = 1;
					global_ui_config.update_interval = (float)atof(nextArg);

					T_ELIF("nogaptags")
						nogap_tags = 1;

					T_ELIF("nogapout")
						/* FIXME: replace strcpy by safer strncpy */
						strcpy(outPath, nextArg);
					argUsed = 1;

					T_ELIF("out-dir")
						/* FIXME: replace strcpy by safer strncpy */
						strcpy(outDir, nextArg);
					argUsed = 1;

					T_ELIF("nogap")
						nogap = 1;

					T_ELIF("swap-channel")
						global_reader.swap_channel = 1;
#if INTERNAL_OPTS
					T_ELIF_INTERNAL("tune") /*without helptext */
						argUsed = 1;
					lame_set_tune(gfp, (float)atof(nextArg));

					T_ELIF_INTERNAL("shortthreshold") {
						float   x, y;
						int     n = sscanf(nextArg, "%f,%f", &x, &y);
						if (n == 1) {
							y = x;
						}
						argUsed = 1;
						(void)lame_set_short_threshold(gfp, x, y);
					}

					T_ELIF_INTERNAL("maskingadjust") /*without helptext */
						argUsed = 1;
					(void)lame_set_maskingadjust(gfp, (float)atof(nextArg));

					T_ELIF_INTERNAL("maskingadjustshort") /*without helptext */
						argUsed = 1;
					(void)lame_set_maskingadjust_short(gfp, (float)atof(nextArg));

					T_ELIF_INTERNAL("athcurve") /*without helptext */
						argUsed = 1;
					(void)lame_set_ATHcurve(gfp, (float)atof(nextArg));

					T_ELIF_INTERNAL("no-preset-tune") /*without helptext */
						(void) lame_set_preset_notune(gfp, 0);

					T_ELIF_INTERNAL("substep")
						argUsed = 1;
					(void)lame_set_substep(gfp, atoi(nextArg));

					T_ELIF_INTERNAL("sbgain") /*without helptext */
						argUsed = 1;
					(void)lame_set_subblock_gain(gfp, atoi(nextArg));

					T_ELIF_INTERNAL("sfscale") /*without helptext */
						(void) lame_set_sfscale(gfp, 1);

					T_ELIF_INTERNAL("noath")
						(void) lame_set_noATH(gfp, 1);

					T_ELIF_INTERNAL("athonly")
						(void) lame_set_ATHonly(gfp, 1);

					T_ELIF_INTERNAL("athshort")
						(void) lame_set_ATHshort(gfp, 1);

					T_ELIF_INTERNAL("athlower")
						argUsed = 1;
					(void)lame_set_ATHlower(gfp, (float)atof(nextArg));

					T_ELIF_INTERNAL("athtype")
						argUsed = 1;
					(void)lame_set_ATHtype(gfp, atoi(nextArg));

					T_ELIF_INTERNAL("athaa-type") /*  switch for developing, no DOCU */
						argUsed = 1; /* once was 1:Gaby, 2:Robert, 3:Jon, else:off */
					lame_set_athaa_type(gfp, atoi(nextArg)); /* now: 0:off else:Jon */
#endif
					T_ELIF("athaa-sensitivity")
						argUsed = 1;
					lame_set_athaa_sensitivity(gfp, (float)atof(nextArg));

					T_ELIF_INTERNAL("debug-file") /* switch for developing, no DOCU */
						argUsed = 1; /* file name to print debug info into */
					{
						// set_debug_file(nextArg);
					}

					T_ELSE{
						errorList.push_back("Unrecognized option.\r\n");
					return -1;
					}
					T_END   i += argUsed;

			}
			else {
				while ((c = *token++) != '\0') {
					arg = *token ? token : nextArg;
					switch (c) {
					case 'm':
						argUsed = 1;

						switch (*arg) {
						case 's':
							(void)lame_set_mode(gfp, STEREO);
							break;
						case 'd':
							(void)lame_set_mode(gfp, DUAL_CHANNEL);
							break;
						case 'f':
							lame_set_force_ms(gfp, 1);
							/* FALLTHROUGH */
						case 'j':
							(void)lame_set_mode(gfp, JOINT_STEREO);
							break;
						case 'm':
							(void)lame_set_mode(gfp, MONO);
							break;
						case 'l':
							(void)lame_set_mode(gfp, MONO);
							(void)lame_set_scale_left(gfp, 2);
							(void)lame_set_scale_right(gfp, 0);
							break;
						case 'r':
							(void)lame_set_mode(gfp, MONO);
							(void)lame_set_scale_left(gfp, 0);
							(void)lame_set_scale_right(gfp, 2);
							break;
						case 'a':
							(void)lame_set_mode(gfp, JOINT_STEREO);
							break;
						default:
							errorList.push_back("-m mode must be s/d/j/f/m not.\r\n");
							return -1;
						}
						break;

					case 'V':
						argUsed = 1;
						/* to change VBR default look in lame.h */
						if (lame_get_VBR(gfp) == vbr_off)
							lame_set_VBR(gfp, vbr_default);
						lame_set_VBR_quality(gfp, (float)atof(arg));
						break;
					case 'v':
						/* to change VBR default look in lame.h */
						if (lame_get_VBR(gfp) == vbr_off)
							lame_set_VBR(gfp, vbr_default);
						break;

					case 'q':
						argUsed = 1;
						(void)lame_set_quality(gfp, atoi(arg));
						break;
					case 'f':
						(void)lame_set_quality(gfp, 7);
						break;
					case 'h':
						(void)lame_set_quality(gfp, 2);
						break;

					case 's':
						argUsed = 1;
						val = atof(arg);
						val = (int)(val * (val <= 192 ? 1.e3 : 1.e0) + 0.5);
						global_reader.input_samplerate = (int)val;
						(void)lame_set_in_samplerate(gfp, (int)val);
						break;
					case 'b':
						argUsed = 1;
						lame_set_brate(gfp, atoi(arg));
						lame_set_VBR_min_bitrate_kbps(gfp, lame_get_brate(gfp));
						break;
					case 'B':
						argUsed = 1;
						lame_set_VBR_max_bitrate_kbps(gfp, atoi(arg));
						break;
					case 'F':
						lame_set_VBR_hard_min(gfp, 1);
						break;
					case 't': /* dont write VBR tag */
						(void)lame_set_bWriteVbrTag(gfp, 0);
						global_decoder.disable_wav_header = 1;
						break;
					case 'T': /* do write VBR tag */
						(void)lame_set_bWriteVbrTag(gfp, 1);
						nogap_tags = 1;
						global_decoder.disable_wav_header = 0;
						break;
					case 'r': /* force raw pcm input file */
#if defined(LIBSNDFILE)
						error_printf
							("WARNING: libsndfile may ignore -r and perform fseek's on the input.\n"
								"Compile without libsndfile if this is a problem.\n");
#endif
						global_reader.input_format = sf_raw;
						break;
					case 'x': /* force byte swapping */
						global_reader.swapbytes = 1;
						break;
					case 'p': /* (jo) error_protection: add crc16 information to stream */
						lame_set_error_protection(gfp, 1);
						break;
					case 'a': /* autoconvert input file from stereo to mono - for mono mp3 encoding */
						autoconvert = 1;
						(void)lame_set_mode(gfp, MONO);
						break;
					case 'd':   /*(void) lame_set_allow_diff_short( gfp, 1 ); */
					case 'k':   /*lame_set_lowpassfreq(gfp, -1);
								lame_set_highpassfreq(gfp, -1); */
						errorList.push_back("WARNING: is obsolete.\r\n");
						break;
					case 'S':
						global_ui_config.silent = 5;
						break;
					case 'X':
						/*  experimental switch -X:
						the differnt types of quant compare are tough
						to communicate to endusers, so they shouldn't
						bother to toy around with them
						*/
					{
						int     x, y;
						int     n = sscanf(arg, "%d,%d", &x, &y);
						if (n == 1) {
							y = x;
						}
						argUsed = 1;
						if (internal_opts_enabled) {
							lame_set_quant_comp(gfp, x);
							lame_set_quant_comp_short(gfp, y);
						}
					}
					break;
					case 'Y':
						lame_set_experimentalY(gfp, 1);
						break;
					case 'Z':
						/*  experimental switch -Z:
						*/
					{
						int     n = 1;
						argUsed = sscanf(arg, "%d", &n);
						/*if (internal_opts_enabled)*/
						{
							lame_set_experimentalZ(gfp, n);
						}
					}
					break;
					case 'e':
						argUsed = 1;

						switch (*arg) {
						case 'n':
							lame_set_emphasis(gfp, 0);
							break;
						case '5':
							lame_set_emphasis(gfp, 1);
							break;
						case 'c':
							lame_set_emphasis(gfp, 3);
							break;
						default:
							errorList.push_back("-e emp must be n/5/c not.\r\n");
							return -1;
						}
						break;
					case 'c':
						lame_set_copyright(gfp, 1);
						break;
					case 'o':
						lame_set_original(gfp, 0);
						break;

					default:
						errorList.push_back("Unrecognized option.\r\n");
						return -1;
					}
					if (argUsed) {
						if (arg == token)
							token = ""; /* no more from token */
						else
							++i; /* skip arg we used */
						arg = "";
						argUsed = 0;
					}
				}
			}
		}
		else {
			if (nogap) {
				if ((num_nogap != NULL) && (count_nogap < *num_nogap)) {
					strncpy(nogap_inPath[count_nogap++], argv[i], PATH_MAX + 1);
					input_file = 1;
				}
				else {
					/* sorry, calling program did not allocate enough space */
					errorList.push_back
						("Error: 'nogap option'.  Calling program does not allow nogap option, or\r\n"
							"you have exceeded maximum number of input files for the nogap option.\r\n");
					*num_nogap = -1;
					return -1;
				}
			}
			else {
				/* normal options:   inputfile  [outputfile], and
				either one can be a '-' for stdin/stdout */
				if (inPath[0] == '\0') {
					strncpy(inPath, argv[i], PATH_MAX + 1);
					input_file = 1;
				}
				else {
					if (outPath[0] == '\0')
						strncpy(outPath, argv[i], PATH_MAX + 1);
					else {
						errorList.push_back("%s: excess arg.\r\n");
						return -1;
					}
				}
			}
		}
	}

	if (!input_file) {
		return -1;
	}

	if (inPath[0] == '-')
		global_ui_config.silent = (global_ui_config.silent <= 1 ? 1 : global_ui_config.silent);
#ifdef WIN32
	else
		dosToLongFileName(inPath);
#endif

	if (outPath[0] == '\0' && count_nogap == 0) {
		if (inPath[0] == '-') {
			/* if input is stdin, default output is stdout */
			strcpy(outPath, "-");
		}
		else {
			if (generateOutPath(gfp, inPath, outDir, outPath) != 0) {
				return -1;
			}
		}
	}

	/* RG is enabled by default */
	if (!noreplaygain)
		lame_set_findReplayGain(gfp, 1);

	/* disable VBR tags with nogap unless the VBR tags are forced */
	if (nogap && lame_get_bWriteVbrTag(gfp) && nogap_tags == 0) {
		lame_set_bWriteVbrTag(gfp, 0);
	}

	/* some file options not allowed with stdout */
	if (outPath[0] == '-') {
		(void)lame_set_bWriteVbrTag(gfp, 0); /* turn off VBR tag */
	}

	/* if user did not explicitly specify input is mp3, check file name */
	if (global_reader.input_format == sf_unknown)
		global_reader.input_format = (sound_file_format_e)filename_to_type(inPath);

#if !(defined HAVE_MPGLIB || defined AMIGA_MPEGA)
	if (is_mpeg_file_format(global_reader.input_format)) {
		error_printf("Error: libmp3lame not compiled with mpg123 *decoding* support \n");
		return -1;
	}
#endif

	/* default guess for number of channels */
	if (autoconvert)
		(void)lame_set_num_channels(gfp, 2);
	else if (MONO == lame_get_mode(gfp))
		(void) lame_set_num_channels(gfp, 1);
	else
		(void)lame_set_num_channels(gfp, 2);

	if (lame_get_free_format(gfp)) {
		if (lame_get_brate(gfp) < 8 || lame_get_brate(gfp) > 640) {
			errorList.push_back("For free format, specify a bitrate between 8 and 640 kbps.\r\n");
			errorList.push_back("with the -b <bitrate> option.\r\n");
			return -1;
		}
	}

	if (num_nogap != NULL)
		*num_nogap = count_nogap;

	return 0;
}

/// <summary>
/// Init files.
/// </summary>
/// <param name="gf">The value.</param>
/// <param name="inPath">The value.</param>
/// <param name="outPath">The value.</param>
/// <returns>The result.</returns>
FILE * InitFiles(lame_global_flags *gf, char const *inPath, char const *outPath)
{
	FILE *outf;

	/* Mostly it is not useful to use the same input and output name.
	This test is very easy and buggy and don't recognize different names
	assigning the same file
	*/
	if (0 != strcmp("-", outPath) && 0 == strcmp(inPath, outPath)) {
		errorList.push_back("Input file and Output file are the same. Abort.\r\n");
		return NULL;
	}

	/* open the wav/aiff/raw pcm or mp3 input file.  This call will
	* open the file, try to parse the headers and
	* set gf.samplerate, gf.num_channels, gf.num_samples.
	* if you want to do your own file input, skip this call and set
	* samplerate, num_channels and num_samples yourself.
	*/
	if (InitInputfile(gf, inPath) < 0) {
		errorList.push_back("Can't init input file.\r\n");
		return NULL;
	}

	if ((outf = InitOutputFile(outPath, lame_get_decode_only(gf))) == NULL) {
		errorList.push_back("Can't init output file.\r\n");
		return NULL;
	}

	// Return the file information.
	return outf;
}

/// <summary>
/// Init input file.
/// </summary>
/// <param name="gfp">The value.</param>
/// <param name="inPath">The value.</param>
/// <returns>The result.</returns>
int InitInputfile(lame_t gfp, char const *inPath)
{
	int enc_delay = 0, enc_padding = 0;

	/* open the input file */
	global.count_samples_carefully = 0;
	global.num_samples_read = 0;
	global.pcmbitwidth = global_raw_pcm.in_bitwidth;
	global.pcmswapbytes = global_reader.swapbytes;
	global.pcm_is_unsigned_8bit = global_raw_pcm.in_signed == 1 ? 0 : 1;
	global.pcm_is_ieee_float = 0;
	global.hip = 0;
	global.music_in = 0;
	global.snd_file = 0;
	global.in_id3v2_size = 0;
	global.in_id3v2_tag = 0;

	if (is_mpeg_file_format(global_reader.input_format)) {
		global.music_in = OpenMpegFile(gfp, inPath, &enc_delay, &enc_padding);
	}
	else {
#ifdef LIBSNDFILE
		if (strcmp(inPath, "-") != 0) { /* not for stdin */
			global.snd_file = open_snd_file(gfp, inPath);
		}
#endif
		if (global.snd_file == 0) {
			global.music_in = OpenWaveFile(gfp, inPath);
		}
	}

	InitPcmBuffer(&global.pcm32, sizeof(int));
	InitPcmBuffer(&global.pcm16, sizeof(short));
	SetSkipStartAndEnd(gfp, enc_delay, enc_padding);
	{
		unsigned long n = lame_get_num_samples(gfp);
		if (n != MAX_U_32_NUM) {
			unsigned long const discard = global.pcm32.skip_start + global.pcm32.skip_end;
			lame_set_num_samples(gfp, n > discard ? n - discard : 0);
		}
	}

	// Return the result.
	return (global.snd_file != NULL || global.music_in != NULL) ? 1 : -1;
}

/// <summary>
/// Open mpeg file.
/// </summary>
/// <param name="gfp">The value.</param>
/// <param name="inPath">The value.</param>
/// <param name="enc_delay">The value.</param>
/// <param name="enc_padding">The value.</param>
/// <returns>The result.</returns>
FILE* OpenMpegFile(lame_t gfp, char const *inPath, int *enc_delay, int *enc_padding)
{
	FILE   *musicin;

	/* set the defaults from info incase we cannot determine them from file */
	lame_set_num_samples(gfp, MAX_U_32_NUM);

	if (strcmp(inPath, "-") == 0) {
		musicin = stdin;
		LameMp3SetStreamBinaryMode(musicin); /* Read from standard input. */
	}
	else {
		musicin = LameMp3OpenFile(inPath, "rb");
		if (musicin == NULL) {
			if (global_ui_config.silent < 10) {
				errorList.push_back("Could not find input path.\r\n");
			}
			return 0;
		}
	}
#ifdef AMIGA_MPEGA
	if (-1 == lame_decode_initfile(inPath, &global_decoder.mp3input_data)) {
		if (global_ui_config.silent < 10) {
			error_printf("Error reading headers in mp3 input file %s.\n", inPath);
		}
		close_input_file(musicin);
		return 0;
	}
#endif
#ifdef HAVE_MPGLIB
	if (-1 == lame_decode_initfile(musicin, &global_decoder.mp3input_data, enc_delay, enc_padding)) {
		if (global_ui_config.silent < 10) {
			errorList.push_back("Error reading headers in mp3 input file.\r\n");
		}
		CloseInputFile(musicin);
		return 0;
	}
#endif
	if (-1 == lame_set_num_channels(gfp, global_decoder.mp3input_data.stereo)) {
		if (global_ui_config.silent < 10) {
			errorList.push_back("Unsupported number of channels.\r\n");
		}
		CloseInputFile(musicin);
		return 0;
	}
	if (global_reader.input_samplerate == 0) {
		(void)lame_set_in_samplerate(gfp, global_decoder.mp3input_data.samplerate);
	}
	else {
		(void)lame_set_in_samplerate(gfp, global_reader.input_samplerate);
	}
	(void)lame_set_num_samples(gfp, global_decoder.mp3input_data.nsamp);

	if (lame_get_num_samples(gfp) == MAX_U_32_NUM && musicin != stdin) {
		double  flen = lame_get_file_size(musicin); /* try to figure out num_samples */
		if (flen >= 0) {
			/* try file size, assume 2 bytes per sample */
			if (global_decoder.mp3input_data.bitrate > 0) {
				double  totalseconds =
					(flen * 8.0 / (1000.0 * global_decoder.mp3input_data.bitrate));
				unsigned long tmp_num_samples =
					(unsigned long)(totalseconds * lame_get_in_samplerate(gfp));

				(void)lame_set_num_samples(gfp, tmp_num_samples);
				global_decoder.mp3input_data.nsamp = tmp_num_samples;
			}
		}
	}
	return musicin;
}

/// <summary>
/// Open wave file.
/// </summary>
/// <param name="gfp">The value.</param>
/// <param name="inPath">The value.</param>
/// <returns>The result.</returns>
FILE* OpenWaveFile(lame_t gfp, char const *inPath)
{
	FILE   *musicin;

	/* set the defaults from info incase we cannot determine them from file */
	lame_set_num_samples(gfp, MAX_U_32_NUM);

	if (!strcmp(inPath, "-")) {
		LameMp3SetStreamBinaryMode(musicin = stdin); /* Read from standard input. */
	}
	else {
		if ((musicin = LameMp3OpenFile(inPath, "rb")) == NULL) {
			if (global_ui_config.silent < 10) {
				errorList.push_back("Could not find input path.\r\n");
			}
			errorList.push_back("Could not open file.\r\n");
		}
	}

	if (global_reader.input_format == sf_ogg) {
		if (global_ui_config.silent < 10) {
			errorList.push_back("Sorry, vorbis support in LAME is deprecated.\r\n");
		}
		errorList.push_back("Input format is unknown.\r\n");
	}
	else if (global_reader.input_format == sf_raw) {
		/* assume raw PCM */
		if (global_ui_config.silent < 9) {
			
		}
		global.pcmswapbytes = global_reader.swapbytes;
	}
	else {
		global_reader.input_format = (sound_file_format)ParseFileHeader(gfp, musicin);
	}
	if (global_reader.input_format == sf_unknown) {
		errorList.push_back("Unsupported data format.\r\n");
	}

	if (lame_get_num_samples(gfp) == MAX_U_32_NUM && musicin != stdin) {
		double const flen = LameMp3GetFileSize(musicin); /* try to figure out num_samples */
		if (flen >= 0) {
			/* try file size, assume 2 bytes per sample */
			unsigned long fsize = (unsigned long)(flen / (2 * lame_get_num_channels(gfp)));
			(void)lame_set_num_samples(gfp, fsize);
		}
	}
	return musicin;
}

/// <summary>
/// Parse file header.
/// </summary>
/// <param name="gfp">The value.</param>
/// <param name="sf">The value.</param>
/// <returns>The result.</returns>
int ParseFileHeader(lame_global_flags *gfp, FILE *sf)
{
	int     type = read_32_bits_high_low(sf);

	/*
	DEBUGF(
	"First word of input stream: %08x '%4.4s'\n", type, (char*) &type);
	*/

	global.count_samples_carefully = 0;
	global.pcm_is_unsigned_8bit = global_raw_pcm.in_signed == 1 ? 0 : 1;
	/*global_reader.input_format = sf_raw; commented out, because it is better to fail
	here as to encode some hundreds of input files not supported by LAME
	If you know you have RAW PCM data, use the -r switch
	*/

	if (type == WAV_ID_RIFF) {
		/* It's probably a WAV file */
		int const ret = ParseWaveHeader(gfp, sf);
		if (ret > 0) {
			global.count_samples_carefully = 1;
			return sf_wave;
		}
		if (ret < 0) {
			if (global_ui_config.silent < 10) {
				errorList.push_back("Warning: corrupt or unsupported WAVE format.\r\n");
			}
		}
	}
	else if (type == IFF_ID_FORM) {
		/* It's probably an AIFF file */
		int const ret = ParseAiffHeader(gfp, sf);
		if (ret > 0) {
			global.count_samples_carefully = 1;
			return sf_aiff;
		}
		if (ret < 0) {
			if (global_ui_config.silent < 10) {
				errorList.push_back("Warning: corrupt or unsupported AIFF format.\r\n");
			}
		}
	}
	else {
		if (global_ui_config.silent < 10) {
			errorList.push_back("Warning: unsupported audio format.\r\n");
		}
	}
	return sf_unknown;
}

/// <summary>
/// Parse Aiff header.
/// </summary>
/// <param name="gfp">The value.</param>
/// <param name="sf">The value.</param>
/// <returns>The result.</returns>
int ParseAiffHeader(lame_global_flags * gfp, FILE * sf)
{
	long    chunkSize = 0, subSize = 0, typeID = 0, dataType = IFF_ID_NONE;
	IFF_AIFF aiff_info;
	int     seen_comm_chunk = 0, seen_ssnd_chunk = 0;
	long    pcm_data_pos = -1;

	memset(&aiff_info, 0, sizeof(aiff_info));
	chunkSize = read_32_bits_high_low(sf);

	typeID = read_32_bits_high_low(sf);
	if ((typeID != IFF_ID_AIFF) && (typeID != IFF_ID_AIFC))
		return -1;

	while (chunkSize > 0) {
		long    ckSize;
		int     type = read_32_bits_high_low(sf);
		chunkSize -= 4;

		/* DEBUGF(
		"found chunk type %08x '%4.4s'\n", type, (char*)&type); */

		/* don't use a switch here to make it easier to use 'break' for SSND */
		if (type == IFF_ID_COMM) {
			seen_comm_chunk = seen_ssnd_chunk + 1;
			subSize = read_32_bits_high_low(sf);
			ckSize = make_even_number_of_bytes_in_length(subSize);
			chunkSize -= ckSize;

			aiff_info.numChannels = (short)read_16_bits_high_low(sf);
			ckSize -= 2;
			aiff_info.numSampleFrames = read_32_bits_high_low(sf);
			ckSize -= 4;
			aiff_info.sampleSize = (short)read_16_bits_high_low(sf);
			ckSize -= 2;
			aiff_info.sampleRate = read_ieee_extended_high_low(sf);
			ckSize -= 10;
			if (typeID == IFF_ID_AIFC) {
				dataType = read_32_bits_high_low(sf);
				ckSize -= 4;
			}
			if (fskip(sf, ckSize, SEEK_CUR) != 0)
				return -1;
		}
		else if (type == IFF_ID_SSND) {
			seen_ssnd_chunk = 1;
			subSize = read_32_bits_high_low(sf);
			ckSize = make_even_number_of_bytes_in_length(subSize);
			chunkSize -= ckSize;

			aiff_info.blkAlgn.offset = read_32_bits_high_low(sf);
			ckSize -= 4;
			aiff_info.blkAlgn.blockSize = read_32_bits_high_low(sf);
			ckSize -= 4;

			aiff_info.sampleType = IFF_ID_SSND;

			if (seen_comm_chunk > 0) {
				if (fskip(sf, (long)aiff_info.blkAlgn.offset, SEEK_CUR) != 0)
					return -1;
				/* We've found the audio data. Read no further! */
				break;
			}
			pcm_data_pos = ftell(sf);
			if (pcm_data_pos >= 0) {
				pcm_data_pos += aiff_info.blkAlgn.offset;
			}
			if (fskip(sf, ckSize, SEEK_CUR) != 0)
				return -1;
		}
		else {
			subSize = read_32_bits_high_low(sf);
			ckSize = make_even_number_of_bytes_in_length(subSize);
			chunkSize -= ckSize;

			if (fskip(sf, ckSize, SEEK_CUR) != 0)
				return -1;
		}
	}
	if (dataType == IFF_ID_2CLE) {
		global.pcmswapbytes = global_reader.swapbytes;
	}
	else if (dataType == IFF_ID_2CBE) {
		global.pcmswapbytes = !global_reader.swapbytes;
	}
	else if (dataType == IFF_ID_NONE) {
		global.pcmswapbytes = !global_reader.swapbytes;
	}
	else {
		return -1;
	}

	/* DEBUGF("Parsed AIFF %d\n", is_aiff); */
	if (seen_comm_chunk && (seen_ssnd_chunk > 0 || aiff_info.numSampleFrames == 0)) {
		/* make sure the header is sane */
		if (0 != aiff_check2(&aiff_info))
			return 0;
		if (-1 == lame_set_num_channels(gfp, aiff_info.numChannels)) {
			if (global_ui_config.silent < 10) {
				errorList.push_back("Unsupported number of channels.\r\n");
			}
			return 0;
		}
		if (global_reader.input_samplerate == 0) {
			(void)lame_set_in_samplerate(gfp, (int)aiff_info.sampleRate);
		}
		else {
			(void)lame_set_in_samplerate(gfp, global_reader.input_samplerate);
		}
		(void)lame_set_num_samples(gfp, aiff_info.numSampleFrames);
		global.pcmbitwidth = aiff_info.sampleSize;
		global.pcm_is_unsigned_8bit = 0;
		global.pcm_is_ieee_float = 0; /* FIXME: possible ??? */
		if (pcm_data_pos >= 0) {
			if (fseek(sf, pcm_data_pos, SEEK_SET) != 0) {
				if (global_ui_config.silent < 10) {
					errorList.push_back("Can't rewind stream to audio data position.\r\n");
				}
				return 0;
			}
		}

		return 1;
	}
	return -1;
}

/// <summary>
/// Parse Wave header.
/// </summary>
/// <param name="gfp">The value.</param>
/// <param name="sf">The value.</param>
/// <returns>The result.</returns>
int ParseWaveHeader(lame_global_flags * gfp, FILE * sf)
{
	int     format_tag = 0;
	int     channels = 0;
	int     block_align = 0;
	int     bits_per_sample = 0;
	int     samples_per_sec = 0;
	int     avg_bytes_per_sec = 0;


	int     is_wav = 0;
	long    data_length = 0, file_length, subSize = 0;
	int     loop_sanity = 0;

	file_length = read_32_bits_high_low(sf);
	if (read_32_bits_high_low(sf) != WAV_ID_WAVE)
		return -1;

	for (loop_sanity = 0; loop_sanity < 20; ++loop_sanity) {
		int     type = read_32_bits_high_low(sf);

		if (type == WAV_ID_FMT) {
			subSize = read_32_bits_low_high(sf);
			subSize = make_even_number_of_bytes_in_length(subSize);
			if (subSize < 16) {
				/*DEBUGF(
				"'fmt' chunk too short (only %ld bytes)!", subSize);  */
				return -1;
			}

			format_tag = read_16_bits_low_high(sf);
			subSize -= 2;
			channels = read_16_bits_low_high(sf);
			subSize -= 2;
			samples_per_sec = read_32_bits_low_high(sf);
			subSize -= 4;
			avg_bytes_per_sec = read_32_bits_low_high(sf);
			subSize -= 4;
			block_align = read_16_bits_low_high(sf);
			subSize -= 2;
			bits_per_sample = read_16_bits_low_high(sf);
			subSize -= 2;

			/* WAVE_FORMAT_EXTENSIBLE support */
			if ((subSize > 9) && (format_tag == WAVE_FORMAT_EXTENSIBLE)) {
				read_16_bits_low_high(sf); /* cbSize */
				read_16_bits_low_high(sf); /* ValidBitsPerSample */
				read_32_bits_low_high(sf); /* ChannelMask */
										   /* SubType coincident with format_tag for PCM int or float */
				format_tag = read_16_bits_low_high(sf);
				subSize -= 10;
			}

			/* DEBUGF("   skipping %d bytes\n", subSize); */

			if (subSize > 0) {
				if (fskip(sf, (long)subSize, SEEK_CUR) != 0)
					return -1;
			};

		}
		else if (type == WAV_ID_DATA) {
			subSize = read_32_bits_low_high(sf);
			data_length = subSize;
			is_wav = 1;
			/* We've found the audio data. Read no further! */
			break;

		}
		else {
			subSize = read_32_bits_low_high(sf);
			subSize = make_even_number_of_bytes_in_length(subSize);
			if (fskip(sf, (long)subSize, SEEK_CUR) != 0) {
				return -1;
			}
		}
	}
	if (is_wav) {
		if (format_tag != WAVE_FORMAT_PCM && format_tag != WAVE_FORMAT_IEEE_FLOAT) {
			if (global_ui_config.silent < 10) {
				errorList.push_back("Unsupported data format.\r\n");
			}
			return 0;   /* oh no! non-supported format  */
		}


		/* make sure the header is sane */
		if (-1 == lame_set_num_channels(gfp, channels)) {
			if (global_ui_config.silent < 10) {
				errorList.push_back("Unsupported number of channels.\r\n");
			}
			return 0;
		}
		if (global_reader.input_samplerate == 0) {
			(void)lame_set_in_samplerate(gfp, samples_per_sec);
		}
		else {
			(void)lame_set_in_samplerate(gfp, global_reader.input_samplerate);
		}
		global.pcmbitwidth = bits_per_sample;
		global.pcm_is_unsigned_8bit = 1;
		global.pcm_is_ieee_float = (format_tag == WAVE_FORMAT_IEEE_FLOAT ? 1 : 0);
		(void)lame_set_num_samples(gfp, data_length / (channels * ((bits_per_sample + 7) / 8)));
		return 1;
	}
	return -1;
}

/// <summary>
/// Get file size.
/// </summary>
/// <param name="fp">The value.</param>
/// <returns>The result.</returns>
off_t LameMp3GetFileSize(FILE *fp)
{
	struct stat sb;
	int     fd = fileno(fp);

	if (0 == fstat(fd, &sb))
		return sb.st_size;
	return (off_t)-1;
}

/// <summary>
/// Init PCM buffer.
/// </summary>
/// <param name="b">The value.</param>
/// <param name="w">The value.</param>
/// <returns>The result.</returns>
void InitPcmBuffer(PcmBuffer *b, int w)
{
	b->ch[0] = 0;
	b->ch[1] = 0;
	b->w = w;
	b->n = 0;
	b->u = 0;
	b->skip_start = 0;
	b->skip_end = 0;
}

/// <summary>
/// Init input file.
/// </summary>
/// <param name="gfp">The value.</param>
/// <param name="enc_delay">The value.</param>
/// <param name="enc_padding">The value.</param>
/// <returns>The result.</returns>
void SetSkipStartAndEnd(lame_t gfp, int enc_delay, int enc_padding)
{
	int     skip_start = 0, skip_end = 0;

	if (global_decoder.mp3_delay_set)
		skip_start = global_decoder.mp3_delay;

	switch (global_reader.input_format) {
	case sf_mp123:
		break;

	case sf_mp3:
		if (skip_start == 0) {
			if (enc_delay > -1 || enc_padding > -1) {
				if (enc_delay > -1)
					skip_start = enc_delay + 528 + 1;
				if (enc_padding > -1)
					skip_end = enc_padding - (528 + 1);
			}
			else
				skip_start = lame_get_encoder_delay(gfp) + 528 + 1;
		}
		else {
			/* user specified a value of skip. just add for decoder */
			skip_start += 528 + 1; /* mp3 decoder has a 528 sample delay, plus user supplied "skip" */
		}
		break;
	case sf_mp2:
		skip_start += 240 + 1;
		break;
	case sf_mp1:
		skip_start += 240 + 1;
		break;
	case sf_raw:
		skip_start += 0; /* other formats have no delay *//* is += 0 not better ??? */
		break;
	case sf_wave:
		skip_start += 0; /* other formats have no delay *//* is += 0 not better ??? */
		break;
	case sf_aiff:
		skip_start += 0; /* other formats have no delay *//* is += 0 not better ??? */
		break;
	default:
		skip_start += 0; /* other formats have no delay *//* is += 0 not better ??? */
		break;
	}

	skip_start = skip_start < 0 ? 0 : skip_start;
	skip_end = skip_end < 0 ? 0 : skip_end;
	global.pcm16.skip_start = global.pcm32.skip_start = skip_start;
	global.pcm16.skip_end = global.pcm32.skip_end = skip_end;
}

/// <summary>
/// Init output file.
/// </summary>
/// <param name="outPath">The value.</param>
/// <param name="decode">The value.</param>
/// <returns>The result.</returns>
FILE* InitOutputFile(char const *outPath, int decode)
{
	FILE   *outf;

	/* open the output file */
	if (0 == strcmp(outPath, "-")) {
		outf = stdout;
		LameMp3SetStreamBinaryMode(outf);
	}
	else {
		outf = LameMp3OpenFile(outPath, "w+b");
#ifdef __riscos__
		/* Assign correct file type */
		if (outf != NULL) {
			char   *p, *out_path = strdup(outPath);
			for (p = out_path; *p; p++) { /* ugly, ugly to modify a string */
				switch (*p) {
				case '.':
					*p = '/';
					break;
				case '/':
					*p = '.';
					break;
				}
			}
			SetFiletype(out_path, decode ? 0xFB1 /*WAV*/ : 0x1AD /*AMPEG*/);
			free(out_path);
		}
#else
		(void)decode;
#endif
	}
	return outf;
}

/// <summary>
/// Open file.
/// </summary>
/// <param name="file">The value.</param>
/// <param name="mode">The value.</param>
/// <returns>The result.</returns>
FILE* LameMp3OpenFile(char const* file, char const* mode)
{
	FILE* fh = 0;
	wchar_t* wfile = UTF8ToUnicode(file);
	wchar_t* wmode = UTF8ToUnicode(mode);
	if (wfile != 0 && wmode != 0) {
		fh = _wfopen(wfile, wmode);
	}
	else {
		fh = fopen(file, mode);
	}
	free(wfile);
	free(wmode);
	return fh;
}

/// <summary>
/// Open binary stream mode.
/// </summary>
/// <param name="fp">The value.</param>
/// <returns>The result.</returns>
int LameMp3SetStreamBinaryMode(FILE * const fp)
{
	_setmode(_fileno(fp), _O_BINARY);
	return 0;
}

int read_32_bits_high_low(FILE * fp)
{
	unsigned char bytes[4] = { 0, 0, 0, 0 };
	fread(bytes, 1, 4, fp);
	{
		int32_t const low = bytes[3];
		int32_t const medl = bytes[2];
		int32_t const medh = bytes[1];
		int32_t const high = (signed char)(bytes[0]);
		return (high << 24) | (medh << 16) | (medl << 8) | low;
	}
}

int read_16_bits_high_low(FILE * fp)
{
	unsigned char bytes[2] = { 0, 0 };
	fread(bytes, 1, 2, fp);
	{
		int32_t const low = bytes[1];
		int32_t const high = (signed char)(bytes[0]);
		return (high << 8) | low;
	}
}

void write_16_bits_low_high(FILE * fp, int val)
{
	unsigned char bytes[2];
	bytes[0] = (val & 0xff);
	bytes[1] = ((val >> 8) & 0xff);
	fwrite(bytes, 1, 2, fp);
}

void write_32_bits_low_high(FILE * fp, int val)
{
	unsigned char bytes[4];
	bytes[0] = (val & 0xff);
	bytes[1] = ((val >> 8) & 0xff);
	bytes[2] = ((val >> 16) & 0xff);
	bytes[3] = ((val >> 24) & 0xff);
	fwrite(bytes, 1, 4, fp);
}

int read_32_bits_low_high(FILE * fp)
{
	unsigned char bytes[4] = { 0, 0, 0, 0 };
	fread(bytes, 1, 4, fp);
	{
		int32_t const low = bytes[0];
		int32_t const medl = bytes[1];
		int32_t const medh = bytes[2];
		int32_t const high = (signed char)(bytes[3]);
		return (high << 24) | (medh << 16) | (medl << 8) | low;
	}
}

int read_16_bits_low_high(FILE * fp)
{
	unsigned char bytes[2] = { 0, 0 };
	fread(bytes, 1, 2, fp);
	{
		int32_t const low = bytes[0];
		int32_t const high = (signed char)(bytes[1]);
		return (high << 8) | low;
	}
}

int fskip(FILE * fp, long offset, int whence)
{
#ifndef PIPE_BUF
	char    buffer[4096];
#else
	char    buffer[PIPE_BUF];
#endif

	/* S_ISFIFO macro is defined on newer Linuxes */
#ifndef S_ISFIFO
# ifdef _S_IFIFO
	/* _S_IFIFO is defined on Win32 and Cygwin */
#  define S_ISFIFO(m) (((m)&_S_IFIFO) == _S_IFIFO)
# endif
#endif

#ifdef S_ISFIFO
	/* fseek is known to fail on pipes with several C-Library implementations
	workaround: 1) test for pipe
	2) for pipes, only relatvie seeking is possible
	3)            and only in forward direction!
	else fallback to old code
	*/
	{
		int const fd = fileno(fp);
		struct stat file_stat;

		if (fstat(fd, &file_stat) == 0) {
			if (S_ISFIFO(file_stat.st_mode)) {
				if (whence != SEEK_CUR || offset < 0) {
					return -1;
				}
				while (offset > 0) {
					size_t const bytes_to_skip = min_size_t(sizeof(buffer), offset);
					size_t const read = fread(buffer, 1, bytes_to_skip, fp);
					if (read < 1) {
						return -1;
					}
					offset -= read;
				}
				return 0;
			}
		}
	}
#endif
	if (0 == fseek(fp, offset, whence)) {
		return 0;
	}

	if (whence != SEEK_CUR || offset < 0) {
		if (global_ui_config.silent < 10) {
			errorList.push_back
				("Mostly the return status of functions is not evaluate so it is more secure to polute.\r\n");
		}
		return -1;
	}

	while (offset > 0) {
		size_t const bytes_to_skip = min_size_t(sizeof(buffer), offset);
		size_t const read = fread(buffer, 1, bytes_to_skip, fp);
		if (read < 1) {
			return -1;
		}
		offset -= read;
	}

	return 0;
}

size_t min_size_t(size_t a, size_t b)
{
	if (a < b) {
		return a;
	}
	return b;
}

long make_even_number_of_bytes_in_length(long x)
{
	if ((x & 0x01) != 0) {
		return x + 1;
	}
	return x;
}

int aiff_check2(IFF_AIFF * const pcm_aiff_data)
{
	if (pcm_aiff_data->sampleType != (unsigned long)IFF_ID_SSND) {
		if (global_ui_config.silent < 10) {
			errorList.push_back("ERROR: input sound data is not PCM.\r\n");
		}
		return 1;
	}
	switch (pcm_aiff_data->sampleSize) {
	case 32:
	case 24:
	case 16:
	case 8:
		break;
	default:
		if (global_ui_config.silent < 10) {
			errorList.push_back("ERROR: input sound data is not 8, 16, 24 or 32 bits.\r\n");
		}
		return 1;
	}
	if (pcm_aiff_data->numChannels != 1 && pcm_aiff_data->numChannels != 2) {
		if (global_ui_config.silent < 10) {
			errorList.push_back("ERROR: input sound data is not mono or stereo.\r\n");
		}
		return 1;
	}
	if (pcm_aiff_data->blkAlgn.blockSize != 0) {
		if (global_ui_config.silent < 10) {
			errorList.push_back("ERROR: block size of input sound data is not 0 bytes.\r\n");
		}
		return 1;
	}
	/* A bug, since we correctly skip the offset earlier in the code.
	if (pcm_aiff_data->blkAlgn.offset != 0) {
	error_printf("Block offset is not 0 bytes in '%s'\n", file_name);
	return 1;
	} */

	return 0;
}

double read_ieee_extended_high_low(FILE * fp)
{
	unsigned char bytes[10];
	memset(bytes, 0, 10);
	fread(bytes, 1, 10, fp);
	{
		int32_t const s = (bytes[0] & 0x80);
		int32_t const e_h = (bytes[0] & 0x7F);
		int32_t const e_l = bytes[1];
		int32_t e = (e_h << 8) | e_l;
		uint32_t const hm = uint32_high_low(bytes + 2);
		uint32_t const lm = uint32_high_low(bytes + 6);
		double  result = 0;
		if (e != 0 || hm != 0 || lm != 0) {
			if (e == 0x7fff) {
				result = HUGE_VAL;
			}
			else {
				double  mantissa_h = UNSIGNED_TO_FLOAT(hm);
				double  mantissa_l = UNSIGNED_TO_FLOAT(lm);
				e -= 0x3fff;
				e -= 31;
				result = ldexp(mantissa_h, e);
				e -= 32;
				result += ldexp(mantissa_l, e);
			}
		}
		return s ? -result : result;
	}
}

unsigned int uint32_high_low(unsigned char *bytes)
{
	uint32_t const hh = bytes[0];
	uint32_t const hl = bytes[1];
	uint32_t const lh = bytes[2];
	uint32_t const ll = bytes[3];
	return (hh << 24) | (hl << 16) | (lh << 8) | ll;
}

int is_mpeg_file_format(int input_file_format)
{
	switch (input_file_format) {
	case sf_mp1:
		return 1;
	case sf_mp2:
		return 2;
	case sf_mp3:
		return 3;
	case sf_mp123:
		return -1;
	default:
		break;
	}
	return 0;
}

off_t lame_get_file_size(FILE * fp)
{
	struct stat sb;
	int     fd = fileno(fp);

	if (0 == fstat(fd, &sb))
		return sb.st_size;
	return (off_t)-1;
}

unsigned char* getOldTag(lame_t gf)
{
	(void)gf;
	return global.in_id3v2_tag;
}

hip_t get_hip(void)
{
	return global.hip;
}

size_t sizeOfOldTag(lame_t gf)
{
	(void)gf;
	return global.in_id3v2_size;
}

int get_audio(lame_t gfp, int buffer[2][1152])
{
	int     used = 0, read = 0;
	do {
		read = get_audio_common(gfp, buffer, NULL);
		used = addPcmBuffer(&global.pcm32, buffer[0], buffer[1], read);
	} while (used <= 0 && read > 0);
	if (read < 0) {
		return read;
	}
	if (global_reader.swap_channel == 0)
		return takePcmBuffer(&global.pcm32, buffer[0], buffer[1], used, 1152);
	else
		return takePcmBuffer(&global.pcm32, buffer[1], buffer[0], used, 1152);
}

int get_audio_common(lame_t gfp, int buffer[2][1152], short buffer16[2][1152])
{
	int     num_channels = lame_get_num_channels(gfp);
	int     insamp[2 * 1152];
	short   buf_tmp16[2][1152];
	int     samples_read;
	int     framesize;
	int     samples_to_read;
	unsigned int remaining, tmp_num_samples;
	int     i;
	int    *p;

	/*
	* NOTE: LAME can now handle arbritray size input data packets,
	* so there is no reason to read the input data in chuncks of
	* size "framesize".  EXCEPT:  the LAME graphical frame analyzer
	* will get out of sync if we read more than framesize worth of data.
	*/

	samples_to_read = framesize = lame_get_framesize(gfp);

	/* get num_samples */
	if (is_mpeg_file_format(global_reader.input_format)) {
		tmp_num_samples = global_decoder.mp3input_data.nsamp;
	}
	else {
		tmp_num_samples = lame_get_num_samples(gfp);
	}

	/* if this flag has been set, then we are carefull to read
	* exactly num_samples and no more.  This is useful for .wav and .aiff
	* files which have id3 or other tags at the end.  Note that if you
	* are using LIBSNDFILE, this is not necessary
	*/
	if (global.count_samples_carefully) {
		if (global.num_samples_read < tmp_num_samples) {
			remaining = tmp_num_samples - global.num_samples_read;
		}
		else {
			remaining = 0;
		}
		if (remaining < (unsigned int)framesize && 0 != tmp_num_samples)
			/* in case the input is a FIFO (at least it's reproducible with
			a FIFO) tmp_num_samples may be 0 and therefore remaining
			would be 0, but we need to read some samples, so don't
			change samples_to_read to the wrong value in this case */
			samples_to_read = remaining;
	}

	if (is_mpeg_file_format(global_reader.input_format)) {
		if (buffer != NULL)
			samples_read = read_samples_mp3(gfp, global.music_in, buf_tmp16);
		else
			samples_read = read_samples_mp3(gfp, global.music_in, buffer16);
		if (samples_read < 0) {
			return samples_read;
		}
	}
	else {
		if (global.snd_file) {
#ifdef LIBSNDFILE
			samples_read = sf_read_int(global.snd_file, insamp, num_channels * samples_to_read);
#else
			samples_read = 0;
#endif
		}
		else {
			samples_read =
				read_samples_pcm(global.music_in, insamp, num_channels * samples_to_read);
		}
		if (samples_read < 0) {
			return samples_read;
		}
		p = insamp + samples_read;
		samples_read /= num_channels;
		if (buffer != NULL) { /* output to int buffer */
			if (num_channels == 2) {
				for (i = samples_read; --i >= 0;) {
					buffer[1][i] = *--p;
					buffer[0][i] = *--p;
				}
			}
			else if (num_channels == 1) {
				memset(buffer[1], 0, samples_read * sizeof(int));
				for (i = samples_read; --i >= 0;) {
					buffer[0][i] = *--p;
				}
			}
		}
		else {          /* convert from int; output to 16-bit buffer */
			if (num_channels == 2) {
				for (i = samples_read; --i >= 0;) {
					buffer16[1][i] = *--p >> (8 * sizeof(int) - 16);
					buffer16[0][i] = *--p >> (8 * sizeof(int) - 16);
				}
			}
			else if (num_channels == 1) {
				memset(buffer16[1], 0, samples_read * sizeof(short));
				for (i = samples_read; --i >= 0;) {
					buffer16[0][i] = *--p >> (8 * sizeof(int) - 16);
				}
			}
		}
	}

	/* LAME mp3 output 16bit -  convert to int, if necessary */
	if (is_mpeg_file_format(global_reader.input_format)) {
		if (buffer != NULL) {
			for (i = samples_read; --i >= 0;)
				buffer[0][i] = buf_tmp16[0][i] << (8 * sizeof(int) - 16);
			if (num_channels == 2) {
				for (i = samples_read; --i >= 0;)
					buffer[1][i] = buf_tmp16[1][i] << (8 * sizeof(int) - 16);
			}
			else if (num_channels == 1) {
				memset(buffer[1], 0, samples_read * sizeof(int));
			}
		}
	}


	/* if num_samples = MAX_U_32_NUM, then it is considered infinitely long.
	Don't count the samples */
	if (tmp_num_samples != MAX_U_32_NUM)
		global.num_samples_read += samples_read;

	return samples_read;
}

int addPcmBuffer(PcmBuffer * b, void *a0, void *a1, int read)
{
	int     a_n;

	if (b == 0) {
		return 0;
	}
	if (read < 0) {
		return b->u - b->skip_end;
	}
	if (b->skip_start >= read) {
		b->skip_start -= read;
		return b->u - b->skip_end;
	}
	a_n = read - b->skip_start;

	if (b != 0 && a_n > 0) {
		int const a_skip = b->w * b->skip_start;
		int const a_want = b->w * a_n;
		int const b_used = b->w * b->u;
		int const b_have = b->w * b->n;
		int const b_need = b->w * (b->u + a_n);
		if (b_have < b_need) {
			b->n = b->u + a_n;
			b->ch[0] = realloc(b->ch[0], b_need);
			b->ch[1] = realloc(b->ch[1], b_need);
		}
		b->u += a_n;
		if (b->ch[0] != 0 && a0 != 0) {
			char   *src = (char *)a0;
			char   *dst = (char *)b->ch[0];
			memcpy(dst + b_used, src + a_skip, a_want);
		}
		if (b->ch[1] != 0 && a1 != 0) {
			char   *src = (char *)a1;
			char   *dst = (char *)b->ch[1];
			memcpy(dst + b_used, src + a_skip, a_want);
		}
	}
	b->skip_start = 0;
	return b->u - b->skip_end;
}

int takePcmBuffer(PcmBuffer * b, void *a0, void *a1, int a_n, int mm)
{
	if (a_n > mm) {
		a_n = mm;
	}
	if (b != 0 && a_n > 0) {
		int const a_take = b->w * a_n;
		if (a0 != 0 && b->ch[0] != 0) {
			memcpy(a0, b->ch[0], a_take);
		}
		if (a1 != 0 && b->ch[1] != 0) {
			memcpy(a1, b->ch[1], a_take);
		}
		b->u -= a_n;
		if (b->u < 0) {
			b->u = 0;
			return a_n;
		}
		if (b->ch[0] != 0) {
			memmove(b->ch[0], (char *)b->ch[0] + a_take, b->w * b->u);
		}
		if (b->ch[1] != 0) {
			memmove(b->ch[1], (char *)b->ch[1] + a_take, b->w * b->u);
		}
	}
	return a_n;
}

int read_samples_mp3(lame_t gfp, FILE * musicin, short int mpg123pcm[2][1152])
{
	int     out;
#if defined(AMIGA_MPEGA)  ||  defined(HAVE_MPGLIB)
	int     samplerate;
	static const char type_name[] = "MP3 file";

	out = lame_decode_fromfile(musicin, mpg123pcm[0], mpg123pcm[1], &global_decoder.mp3input_data);
	/*
	* out < 0:  error, probably EOF
	* out = 0:  not possible with lame_decode_fromfile() ???
	* out > 0:  number of output samples
	*/
	if (out < 0) {
		memset(mpg123pcm, 0, sizeof(**mpg123pcm) * 2 * 1152);
		return 0;
	}

	if (lame_get_num_channels(gfp) != global_decoder.mp3input_data.stereo) {
		if (global_ui_config.silent < 10) {
			errorList.push_back("Error: number of channels has changed in %s - not supported.\r\n");
		}
		out = -1;
	}
	samplerate = global_reader.input_samplerate;
	if (samplerate == 0) {
		samplerate = global_decoder.mp3input_data.samplerate;
	}
	if (lame_get_in_samplerate(gfp) != samplerate) {
		if (global_ui_config.silent < 10) {
			errorList.push_back("Error: sample frequency has changed in %s - not supported.\r\n");
		}
		out = -1;
	}
#else
	out = -1;
#endif
	return out;
}

int lame_decode_fromfile(FILE * fd, short pcm_l[], short pcm_r[], mp3data_struct * mp3data)
{
	int     ret = 0;
	size_t  len = 0;
	unsigned char buf[1024];

	/* first see if we still have data buffered in the decoder: */
	ret = hip_decode1_headers(global.hip, buf, len, pcm_l, pcm_r, mp3data);
	if (ret != 0)
		return ret;


	/* read until we get a valid output frame */
	for (;;) {
		len = fread(buf, 1, 1024, fd);
		if (len == 0) {
			/* we are done reading the file, but check for buffered data */
			ret = hip_decode1_headers(global.hip, buf, len, pcm_l, pcm_r, mp3data);
			if (ret <= 0) {
				hip_decode_exit(global.hip); /* release mp3decoder memory */
				global.hip = 0;
				return -1; /* done with file */
			}
			break;
		}

		ret = hip_decode1_headers(global.hip, buf, len, pcm_l, pcm_r, mp3data);
		if (ret == -1) {
			hip_decode_exit(global.hip); /* release mp3decoder memory */
			global.hip = 0;
			return -1;
		}
		if (ret > 0)
			break;
	}
	return ret;
}

int read_samples_pcm(FILE * musicin, int sample_buffer[2304], int samples_to_read)
{
	int     samples_read;
	int     bytes_per_sample = global.pcmbitwidth / 8;
	int     swap_byte_order; /* byte order of input stream */

	switch (global.pcmbitwidth) {
	case 32:
	case 24:
	case 16:
		if (global_raw_pcm.in_signed == 0) {
			if (global_ui_config.silent < 10) {
				errorList.push_back("Unsigned input only supported with bitwidth 8.\r\n");
			}
			return -1;
		}
		swap_byte_order = (global_raw_pcm.in_endian != ByteOrderLittleEndian) ? 1 : 0;
		if (global.pcmswapbytes) {
			swap_byte_order = !swap_byte_order;
		}
		break;

	case 8:
		swap_byte_order = global.pcm_is_unsigned_8bit;
		break;

	default:
		if (global_ui_config.silent < 10) {
			errorList.push_back("Only 8, 16, 24 and 32 bit input files supported.\r\n");
		}
		return -1;
	}
	samples_read = unpack_read_samples(samples_to_read, bytes_per_sample, swap_byte_order,
		sample_buffer, musicin);
	if (ferror(musicin)) {
		if (global_ui_config.silent < 10) {
			errorList.push_back("Error reading input file.\r\n");
		}
		return -1;
	}

	return samples_read;
}

int unpack_read_samples(const int samples_to_read, const int bytes_per_sample,
	const int swap_order, int *sample_buffer, FILE * pcm_in)
{
	size_t  samples_read;
	int     i;
	int    *op;              /* output pointer */
	unsigned char *ip = (unsigned char *)sample_buffer; /* input pointer */
	const int b = sizeof(int) * 8;

#define GA_URS_IFLOOP( ga_urs_bps ) \
    if( bytes_per_sample == ga_urs_bps ) \
      for( i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >=0;)

	samples_read = fread(sample_buffer, bytes_per_sample, samples_to_read, pcm_in);
	op = sample_buffer + samples_read;

	if (swap_order == 0) {
		GA_URS_IFLOOP(1)
			* --op = ip[i] << (b - 8);
		GA_URS_IFLOOP(2)
			* --op = ip[i] << (b - 16) | ip[i + 1] << (b - 8);
		GA_URS_IFLOOP(3)
			* --op = ip[i] << (b - 24) | ip[i + 1] << (b - 16) | ip[i + 2] << (b - 8);
		GA_URS_IFLOOP(4)
			* --op =
			ip[i] << (b - 32) | ip[i + 1] << (b - 24) | ip[i + 2] << (b - 16) | ip[i + 3] << (b -
				8);
	}
	else {
		GA_URS_IFLOOP(1)
			* --op = (ip[i] ^ 0x80) << (b - 8) | 0x7f << (b - 16); /* convert from unsigned */
		GA_URS_IFLOOP(2)
			* --op = ip[i] << (b - 8) | ip[i + 1] << (b - 16);
		GA_URS_IFLOOP(3)
			* --op = ip[i] << (b - 8) | ip[i + 1] << (b - 16) | ip[i + 2] << (b - 24);
		GA_URS_IFLOOP(4)
			* --op =
			ip[i] << (b - 8) | ip[i + 1] << (b - 16) | ip[i + 2] << (b - 24) | ip[i + 3] << (b -
				32);
	}
#undef GA_URS_IFLOOP
	if (global.pcm_is_ieee_float) {
		ieee754_float32_t const m_max = INT_MAX;
		ieee754_float32_t const m_min = -(ieee754_float32_t)INT_MIN;
		ieee754_float32_t *x = (ieee754_float32_t *)sample_buffer;
		
		for (i = 0; i < samples_to_read; ++i) {
			ieee754_float32_t const u = x[i];
			int     v;
			if (u >= 1) {
				v = INT_MAX;
			}
			else if (u <= -1) {
				v = INT_MIN;
			}
			else if (u >= 0) {
				v = (int)(u * m_max + 0.5f);
			}
			else {
				v = (int)(u * m_min - 0.5f);
			}
			sample_buffer[i] = v;
		}
	}
	return (samples_read);
}

int lame_decode_initfile(FILE * fd, mp3data_struct * mp3data, int *enc_delay, int *enc_padding)
{
	/*  VBRTAGDATA pTagData; */
	/* int xing_header,len2,num_frames; */
	unsigned char buf[100];
	int     ret;
	size_t  len;
	int     aid_header;
	short int pcm_l[1152], pcm_r[1152];
	int     freeformat = 0;

	memset(mp3data, 0, sizeof(mp3data_struct));
	if (global.hip) {
		hip_decode_exit(global.hip);
	}

	global.hip = hip_decode_init();
	/*hip_set_msgf(global.hip, global_ui_config.silent < 10 ? &frontend_msgf : 0);
	hip_set_errorf(global.hip, global_ui_config.silent < 10 ? &frontend_errorf : 0);
	hip_set_debugf(global.hip, &frontend_debugf);*/

	len = 4;
	if (fread(buf, 1, len, fd) != len)
		return -1;      /* failed */
	while (buf[0] == 'I' && buf[1] == 'D' && buf[2] == '3') {
		len = 6;
		if (fread(&buf[4], 1, len, fd) != len)
			return -1;  /* failed */
		len = lenOfId3v2Tag(&buf[6]);
		if (global.in_id3v2_size < 1) {
			global.in_id3v2_size = 10 + len;
			global.in_id3v2_tag = (unsigned char*)malloc(global.in_id3v2_size);
			if (global.in_id3v2_tag) {
				memcpy(global.in_id3v2_tag, buf, 10);
				if (fread(&global.in_id3v2_tag[10], 1, len, fd) != len)
					return -1;  /* failed */
				len = 0; /* copied, nothing to skip */
			}
			else {
				global.in_id3v2_size = 0;
			}
		}
		fskip(fd, len, SEEK_CUR);
		len = 4;
		if (fread(&buf, 1, len, fd) != len)
			return -1;  /* failed */
	}
	aid_header = check_aid(buf);
	if (aid_header) {
		if (fread(&buf, 1, 2, fd) != 2)
			return -1;  /* failed */
		aid_header = (unsigned char)buf[0] + 256 * (unsigned char)buf[1];
		if (global_ui_config.silent < 9) {
			errorList.push_back("Album ID found.\r\n");
		}
		/* skip rest of AID, except for 6 bytes we have already read */
		fskip(fd, aid_header - 6, SEEK_CUR);

		/* read 4 more bytes to set up buffer for MP3 header check */
		if (fread(&buf, 1, len, fd) != len)
			return -1;  /* failed */
	}
	len = 4;
	while (!is_syncword_mp123(buf)) {
		unsigned int i;
		for (i = 0; i < len - 1; i++)
			buf[i] = buf[i + 1];
		if (fread(buf + len - 1, 1, 1, fd) != 1)
			return -1;  /* failed */
	}

	if ((buf[2] & 0xf0) == 0) {
		if (global_ui_config.silent < 9) {
			errorList.push_back("Input file is freeformat.\r\n");
		}
		freeformat = 1;
	}
	/* now parse the current buffer looking for MP3 headers.    */
	/* (as of 11/00: mpglib modified so that for the first frame where  */
	/* headers are parsed, no data will be decoded.   */
	/* However, for freeformat, we need to decode an entire frame, */
	/* so mp3data->bitrate will be 0 until we have decoded the first */
	/* frame.  Cannot decode first frame here because we are not */
	/* yet prepared to handle the output. */
	ret = hip_decode1_headersB(global.hip, buf, len, pcm_l, pcm_r, mp3data, enc_delay, enc_padding);
	if (-1 == ret)
		return -1;

	/* repeat until we decode a valid mp3 header.  */
	while (!mp3data->header_parsed) {
		len = fread(buf, 1, sizeof(buf), fd);
		if (len != sizeof(buf))
			return -1;
		ret =
			hip_decode1_headersB(global.hip, buf, len, pcm_l, pcm_r, mp3data, enc_delay,
				enc_padding);
		if (-1 == ret)
			return -1;
	}

	if (mp3data->bitrate == 0 && !freeformat) {
		if (global_ui_config.silent < 10) {
			errorList.push_back("fail to sync...\r\n");
		}
		return lame_decode_initfile(fd, mp3data, enc_delay, enc_padding);
	}

	if (mp3data->totalframes > 0) {
		/* mpglib found a Xing VBR header and computed nsamp & totalframes */
	}
	else {
		/* set as unknown.  Later, we will take a guess based on file size
		* ant bitrate */
		mp3data->nsamp = MAX_U_32_NUM;
	}

	/*
	report_printf("ret = %i NEED_MORE=%i \n",ret,MP3_NEED_MORE);
	report_printf("stereo = %i \n",mp.fr.stereo);
	report_printf("samp = %i  \n",freqs[mp.fr.sampling_frequency]);
	report_printf("framesize = %i  \n",framesize);
	report_printf("bitrate = %i  \n",mp3data->bitrate);
	report_printf("num frames = %ui  \n",num_frames);
	report_printf("num samp = %ui  \n",mp3data->nsamp);
	report_printf("mode     = %i  \n",mp.fr.mode);
	*/

	return 0;
}

size_t lenOfId3v2Tag(unsigned char const* buf)
{
	unsigned int b0 = buf[0] & 127;
	unsigned int b1 = buf[1] & 127;
	unsigned int b2 = buf[2] & 127;
	unsigned int b3 = buf[3] & 127;
	return (((((b0 << 7) + b1) << 7) + b2) << 7) + b3;
}

int check_aid(const unsigned char *header)
{
	return 0 == memcmp(header, "AiD\1", 4);
}

int is_syncword_mp123(const void *const headerptr)
{
	const unsigned char *const p = (const unsigned char *const)headerptr;
	static const char abl2[16] = { 0, 7, 7, 7, 0, 7, 0, 0, 0, 0, 0, 8, 8, 8, 8, 8 };

	if ((p[0] & 0xFF) != 0xFF)
		return 0;       /* first 8 bits must be '1' */
	if ((p[1] & 0xE0) != 0xE0)
		return 0;       /* next 3 bits are also */
	if ((p[1] & 0x18) == 0x08)
		return 0;       /* no MPEG-1, -2 or -2.5 */
	switch (p[1] & 0x06) {
	default:
	case 0x00:         /* illegal Layer */
		return 0;

	case 0x02:         /* Layer3 */
		if (global_reader.input_format != sf_mp3 && global_reader.input_format != sf_mp123) {
			return 0;
		}
		global_reader.input_format = sf_mp3;
		break;

	case 0x04:         /* Layer2 */
		if (global_reader.input_format != sf_mp2 && global_reader.input_format != sf_mp123) {
			return 0;
		}
		global_reader.input_format = sf_mp2;
		break;

	case 0x06:         /* Layer1 */
		if (global_reader.input_format != sf_mp1 && global_reader.input_format != sf_mp123) {
			return 0;
		}
		global_reader.input_format = sf_mp1;
		break;
	}
	if ((p[1] & 0x06) == 0x00)
		return 0;       /* no Layer I, II and III */
	if ((p[2] & 0xF0) == 0xF0)
		return 0;       /* bad bitrate */
	if ((p[2] & 0x0C) == 0x0C)
		return 0;       /* no sample frequency with (32,44.1,48)/(1,2,4)     */
	if ((p[1] & 0x18) == 0x18 && (p[1] & 0x06) == 0x04 && abl2[p[2] >> 4] & (1 << (p[3] >> 6)))
		return 0;
	if ((p[3] & 3) == 2)
		return 0;       /* reserved enphasis mode */
	return 1;
}

int write_id3v1_tag(lame_t gf, FILE * outf)
{
	unsigned char mp3buffer[128];
	int     imp3, owrite;

	imp3 = lame_get_id3v1_tag(gf, mp3buffer, sizeof(mp3buffer));
	if (imp3 <= 0) {
		return 0;
	}
	if ((size_t)imp3 > sizeof(mp3buffer)) {
		errorList.push_back("Error writing ID3v1 tag: buffer too small.\r\n");
		return 0;       /* not critical */
	}
	owrite = (int)fwrite(mp3buffer, 1, imp3, outf);
	if (owrite != imp3) {
		errorList.push_back("Error writing ID3v1 tag.\r\n");
		return 1;
	}
	return 0;
}

int write_xing_frame(lame_global_flags * gf, FILE * outf, size_t offset)
{
	unsigned char mp3buffer[LAME_MAXMP3BUFFER];
	size_t  imp3, owrite;

	imp3 = lame_get_lametag_frame(gf, mp3buffer, sizeof(mp3buffer));
	if (imp3 <= 0) {
		return 0;       /* nothing to do */
	}
	
	if (imp3 > sizeof(mp3buffer)) {
		errorList.push_back
			("Error writing LAME-tag frame: buffer too small.\r\n");
		return -1;
	}
	if (fseek(outf, offset, SEEK_SET) != 0) {
		errorList.push_back("fatal error: can't update LAME-tag frame!\r\n");
		return -1;
	}
	owrite = (int)fwrite(mp3buffer, 1, imp3, outf);
	if (owrite != imp3) {
		errorList.push_back("Error writing LAME-tag.\r\n");
		return -1;
	}
	
	return imp3;
}

/* LAME is a simple frontend which just uses the file extension */
/* to determine the file type.  Trying to analyze the file */
/* contents is well beyond the scope of LAME and should not be added. */
int filename_to_type(const char *FileName)
{
	size_t  len = strlen(FileName);

	if (len < 4)
		return sf_unknown;

	FileName += len - 4;
	if (0 == local_strcasecmp(FileName, ".mpg"))
		return sf_mp123;
	if (0 == local_strcasecmp(FileName, ".mp1"))
		return sf_mp123;
	if (0 == local_strcasecmp(FileName, ".mp2"))
		return sf_mp123;
	if (0 == local_strcasecmp(FileName, ".mp3"))
		return sf_mp123;
	if (0 == local_strcasecmp(FileName, ".wav"))
		return sf_wave;
	if (0 == local_strcasecmp(FileName, ".aif"))
		return sf_aiff;
	if (0 == local_strcasecmp(FileName, ".raw"))
		return sf_raw;
	if (0 == local_strcasecmp(FileName, ".ogg"))
		return sf_ogg;
	return sf_unknown;
}

int local_strcasecmp(const char *s1, const char *s2)
{
	unsigned char c1;
	unsigned char c2;

	do {
		c1 = (unsigned char)tolower(*s1);
		c2 = (unsigned char)tolower(*s2);
		if (!c1) {
			break;
		}
		++s1;
		++s2;
	} while (c1 == c2);
	return c1 - c2;
}

int generateOutPath(lame_t gfp, char const* inPath, char const* outDir, char* outPath)
{
	size_t const max_path = PATH_MAX;
	char const* s_ext = lame_get_decode_only(gfp) ? ".wav" : ".mp3";
#if 1
	size_t i = 0;
	int out_dir_used = 0;

	if (outDir != 0 && outDir[0] != 0) {
		out_dir_used = 1;
		while (*outDir) {
			outPath[i++] = *outDir++;
			if (i >= max_path) {
				goto err_generateOutPath;
			}
		}
		if (i > 0 && outPath[i - 1] != SLASH) {
			outPath[i++] = SLASH;
			if (i >= max_path) {
				goto err_generateOutPath;
			}
		}
		outPath[i] = 0;
	}
	else {
		char const* pa;
		char const* pb;
		size_t j, n = scanPath(inPath, &pa, &pb);
		if (i + n >= max_path) {
			goto err_generateOutPath;
		}
		for (j = 0; j < n; ++j) {
			outPath[i++] = pa[j];
		}
		if (n > 0) {
			outPath[i++] = SLASH;
			if (i >= max_path) {
				goto err_generateOutPath;
			}
		}
		outPath[i] = 0;
	}
	{
		int replace_suffix = 0;
		char const* na;
		char const* nb;
		size_t j, n = scanBasename(inPath, &na, &nb);
		if (i + n >= max_path) {
			goto err_generateOutPath;
		}
		for (j = 0; j < n; ++j) {
			outPath[i++] = na[j];
		}
		outPath[i] = 0;
		if (isCommonSuffix(nb) == 1) {
			replace_suffix = 1;
			if (out_dir_used == 0) {
				if (local_strcasecmp(nb, s_ext) == 0) {
					replace_suffix = 0;
				}
			}
		}
		if (replace_suffix == 0) {
			while (*nb) {
				outPath[i++] = *nb++;
				if (i >= max_path) {
					goto err_generateOutPath;
				}
			}
			outPath[i] = 0;
		}
	}
	if (i + 5 >= max_path) {
		goto err_generateOutPath;
	}
	while (*s_ext) {
		outPath[i++] = *s_ext++;
	}
	outPath[i] = 0;
	return 0;
err_generateOutPath:
	errorList.push_back("error: output file name too long.\r\n");
	return 1;
#else
	strncpy(outPath, inPath, PATH_MAX + 1 - 4);
	strncat(outPath, s_ext, 4);
	return 0;
#endif
}

size_t scanPath(char const* s, char const** a, char const** b)
{
	char const* s1 = s;
	char const* s2 = s;
	if (s != 0) {
		for (; *s; ++s) {
			switch (*s) {
			case SLASH:
			case ':':
				s2 = s;
				break;
			}
		}
		if (*s2 == ':') {
			++s2;
		}
	}
	if (a) {
		*a = s1;
	}
	if (b) {
		*b = s2;
	}
	return s2 - s1;
}

size_t scanBasename(char const* s, char const** a, char const** b)
{
	char const* s1 = s;
	char const* s2 = s;
	if (s != 0) {
		for (; *s; ++s) {
			switch (*s) {
			case SLASH:
			case ':':
				s1 = s2 = s;
				break;
			case '.':
				s2 = s;
				break;
			}
		}
		if (s2 == s1) {
			s2 = s;
		}
		if (*s1 == SLASH || *s1 == ':') {
			++s1;
		}
	}
	if (a != 0) {
		*a = s1;
	}
	if (b != 0) {
		*b = s2;
	}
	return s2 - s1;
}

int isCommonSuffix(char const* s_ext)
{
	char const* suffixes[] =
	{ ".WAV", ".RAW", ".MP1", ".MP2"
		, ".MP3", ".MPG", ".MPA", ".CDA"
		, ".OGG", ".AIF", ".AIFF", ".AU"
		, ".SND", ".FLAC", ".WV", ".OFR"
		, ".TAK", ".MP4", ".M4A", ".PCM"
		, ".W64"
	};
	size_t i;
	for (i = 0; i < dimension_of(suffixes); ++i) {
		if (local_strcasecmp(s_ext, suffixes[i]) == 0) {
			return 1;
		}
	}
	return 0;
}

void dosToLongFileName(char *fn)
{
	const int MSIZE = PATH_MAX + 1 - 4; /*  we wanna add ".mp3" later */
	WIN32_FIND_DATAA lpFindFileData;
	HANDLE  h = FindFirstFileA(fn, &lpFindFileData);
	if (h != INVALID_HANDLE_VALUE) {
		int     a;
		char   *q, *p;
		FindClose(h);
		for (a = 0; a < MSIZE; a++) {
			if ('\0' == lpFindFileData.cFileName[a])
				break;
		}
		if (a >= MSIZE || a == 0)
			return;
		q = strrchr(fn, '\\');
		p = strrchr(fn, '/');
		if (p - q > 0)
			q = p;
		if (q == NULL)
			q = strrchr(fn, ':');
		if (q == NULL)
			strncpy(fn, lpFindFileData.cFileName, a);
		else {
			a += q - fn + 1;
			if (a >= MSIZE)
				return;
			strncpy(++q, lpFindFileData.cFileName, MSIZE - a);
		}
	}
}

int presets_set(lame_t gfp, int fast, int cbr, const char *preset_name, const char *ProgramName)
{
	int     mono = 0;

	if ((strcmp(preset_name, "help") == 0) && (fast < 1)
		&& (cbr < 1)) {
		return -1;
	}

	/*aliases for compatibility with old presets */

	if (strcmp(preset_name, "phone") == 0) {
		preset_name = "16";
		mono = 1;
	}
	if ((strcmp(preset_name, "phon+") == 0) ||
		(strcmp(preset_name, "lw") == 0) ||
		(strcmp(preset_name, "mw-eu") == 0) || (strcmp(preset_name, "sw") == 0)) {
		preset_name = "24";
		mono = 1;
	}
	if (strcmp(preset_name, "mw-us") == 0) {
		preset_name = "40";
		mono = 1;
	}
	if (strcmp(preset_name, "voice") == 0) {
		preset_name = "56";
		mono = 1;
	}
	if (strcmp(preset_name, "fm") == 0) {
		preset_name = "112";
	}
	if ((strcmp(preset_name, "radio") == 0) || (strcmp(preset_name, "tape") == 0)) {
		preset_name = "112";
	}
	if (strcmp(preset_name, "hifi") == 0) {
		preset_name = "160";
	}
	if (strcmp(preset_name, "cd") == 0) {
		preset_name = "192";
	}
	if (strcmp(preset_name, "studio") == 0) {
		preset_name = "256";
	}

	if (strcmp(preset_name, "medium") == 0) {
		lame_set_VBR_q(gfp, 4);
		lame_set_VBR(gfp, vbr_default);
		return 0;
	}

	if (strcmp(preset_name, "standard") == 0) {
		lame_set_VBR_q(gfp, 2);
		lame_set_VBR(gfp, vbr_default);
		return 0;
	}

	else if (strcmp(preset_name, "extreme") == 0) {
		lame_set_VBR_q(gfp, 0);
		lame_set_VBR(gfp, vbr_default);
		return 0;
	}

	else if ((strcmp(preset_name, "insane") == 0) && (fast < 1)) {

		lame_set_preset(gfp, INSANE);

		return 0;
	}

	/* Generic ABR Preset */
	if (((atoi(preset_name)) > 0) && (fast < 1)) {
		if ((atoi(preset_name)) >= 8 && (atoi(preset_name)) <= 320) {
			lame_set_preset(gfp, atoi(preset_name));

			if (cbr == 1)
				lame_set_VBR(gfp, vbr_off);

			if (mono == 1) {
				lame_set_mode(gfp, MONO);
			}

			return 0;

		}
		else {
			errorList.push_back("Error: The bitrate specified is out of the valid range for this preset\n"
				"\n"
				"When using this mode you must enter a value between \"32\" and \"320\"\n"
				"\n" "For further information try: --preset help\"\r\n");
			return -1;
		}
	}

	errorList.push_back("Error: You did not enter a valid profile and/or options with --preset\n"
		"\n"
		"Available profiles are:\n"
		"\n"
		"                 medium\n"
		"                 standard\n"
		"                 extreme\n"
		"                 insane\n"
		"          <cbr> (ABR Mode) - The ABR Mode is implied. To use it,\n"
		"                             simply specify a bitrate. For example:\n"
		"                             \"--preset 185\" activates this\n"
		"                             preset and uses 185 as an average kbps.\n" "\r\n");
	return -1;
}

int local_strncasecmp(const char *s1, const char *s2, int n)
{
	unsigned char c1 = 0;
	unsigned char c2 = 0;
	int     cnt = 0;

	do {
		if (cnt == n) {
			break;
		}
		c1 = (unsigned char)tolower(*s1);
		c2 = (unsigned char)tolower(*s2);
		if (!c1) {
			break;
		}
		++s1;
		++s2;
		++cnt;
	} while (c1 == c2);
	return c1 - c2;
}

int resample_rate(double freq)
{
	if (freq >= 1.e3)
		freq *= 1.e-3;

	switch ((int)freq) {
	case 8:
		return 8000;
	case 11:
		return 11025;
	case 12:
		return 12000;
	case 16:
		return 16000;
	case 22:
		return 22050;
	case 24:
		return 24000;
	case 32:
		return 32000;
	case 44:
		return 44100;
	case 48:
		return 48000;
	default:
		errorList.push_back("Illegal resample frequency: %.3f kHz\r\n");
		return 0;
	}
}

void setProcessPriority(int Priority)
{
	switch (Priority) {
	case 0:
	case 1:
		SetPriorityClassMacro(IDLE_PRIORITY_CLASS);
		break;
	default:
	case 2:
		SetPriorityClassMacro(NORMAL_PRIORITY_CLASS);
		break;
	case 3:
	case 4:
		SetPriorityClassMacro(HIGH_PRIORITY_CLASS);
		break;
	}
}

BOOL SetPriorityClassMacro(DWORD p)
{
	HANDLE  op = GetCurrentProcess();
	return SetPriorityClass(op, p);
}

int id3_tag(lame_global_flags* gfp, int type, TextEncoding enc, char* str)
{
	void* x = 0;
	int result;
	if (enc == TENC_UTF16 && type != 'v') {
		id3_tag(gfp, type, TENC_LATIN1, str); /* for id3v1 */
	}
	switch (enc)
	{
	default:
#ifdef ID3TAGS_EXTENDED
	case TENC_LATIN1: x = ToLatin1(str); break;
	case TENC_UTF16:  x = ToUtf16(str);   break;
#else
	case TENC_RAW:    x = strdup(str);   break;
#endif
	}
	switch (enc)
	{
	default:
#ifdef ID3TAGS_EXTENDED
	case TENC_LATIN1: result = set_id3tag(gfp, type, (char const*)x);   break;
	case TENC_UTF16:  result = set_id3v2tag(gfp, type, (unsigned short const*)x); break;
#else
	case TENC_RAW:    result = set_id3tag(gfp, type, x);   break;
#endif
	}
	free(x);
	return result;
}

int set_id3v2tag(lame_global_flags* gfp, int type, unsigned short const* str)
{
	switch (type)
	{
	case 'a': return id3tag_set_textinfo_utf16(gfp, "TPE1", str);
	case 't': return id3tag_set_textinfo_utf16(gfp, "TIT2", str);
	case 'l': return id3tag_set_textinfo_utf16(gfp, "TALB", str);
	case 'g': return id3tag_set_textinfo_utf16(gfp, "TCON", str);
	case 'c': return id3tag_set_comment_utf16(gfp, 0, 0, str);
	case 'n': return id3tag_set_textinfo_utf16(gfp, "TRCK", str);
	case 'y': return id3tag_set_textinfo_utf16(gfp, "TYER", str);
	case 'v': return id3tag_set_fieldvalue_utf16(gfp, str);
	}
	return 0;
}

int set_id3tag(lame_global_flags* gfp, int type, char const* str)
{
	switch (type)
	{
	case 'a': return id3tag_set_artist(gfp, str), 0;
	case 't': return id3tag_set_title(gfp, str), 0;
	case 'l': return id3tag_set_album(gfp, str), 0;
	case 'g': return id3tag_set_genre(gfp, str);
	case 'c': return id3tag_set_comment(gfp, str), 0;
	case 'n': return id3tag_set_track(gfp, str);
	case 'y': return id3tag_set_year(gfp, str), 0;
	case 'v': return id3tag_set_fieldvalue(gfp, str);
	}
	return 0;
}

int set_id3_albumart(lame_t gfp, char const* file_name)
{
	int ret = -1;
	FILE *fpi = 0;
	char *albumart = 0;

	if (file_name == 0) {
		return 0;
	}
	fpi = lame_fopen(file_name, "rb");
	if (!fpi) {
		ret = 1;
	}
	else {
		size_t size;

		fseek(fpi, 0, SEEK_END);
		size = ftell(fpi);
		fseek(fpi, 0, SEEK_SET);
		albumart = (char *)malloc(size);
		if (!albumart) {
			ret = 2;
		}
		else {
			if (fread(albumart, 1, size, fpi) != size) {
				ret = 3;
			}
			else {
				ret = id3tag_set_albumart(gfp, albumart, size) ? 4 : 0;
			}
			free(albumart);
		}
		fclose(fpi);
	}
	switch (ret) {
	case 1: errorList.push_back("Could not find.\r\n"); break;
	case 2: errorList.push_back("Insufficient memory for reading the albumart.\r\n"); break;
	case 3: errorList.push_back("Read error.\r\n"); break;
	case 4: errorList.push_back("Unsupported image.\nSpecify JPEG/PNG/GIF image\r\n"); break;
	default: break;
	}
	return ret;
}

FILE* lame_fopen(char const* file, char const* mode)
{
	FILE* fh = 0;
	wchar_t* wfile = UTF8ToUnicode(file);
	wchar_t* wmode = UTF8ToUnicode(mode);
	if (wfile != 0 && wmode != 0) {
		fh = _wfopen(wfile, wmode);
	}
	else {
		fh = fopen(file, mode);
	}
	free(wfile);
	free(wmode);
	return fh;
}