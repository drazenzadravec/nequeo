/*
   +----------------------------------------------------------------------------------------------+
   | Windows Cache for PHP                                                                        |
   +----------------------------------------------------------------------------------------------+
   | Copyright (c) 2009, Microsoft Corporation. All rights reserved.                              |
   |                                                                                              |
   | Redistribution and use in source and binary forms, with or without modification, are         |
   | permitted provided that the following conditions are met:                                    |
   | - Redistributions of source code must retain the above copyright notice, this list of        |
   | conditions and the following disclaimer.                                                     |
   | - Redistributions in binary form must reproduce the above copyright notice, this list of     |
   | conditions and the following disclaimer in the documentation and/or other materials provided |
   | with the distribution.                                                                       |
   | - Neither the name of the Microsoft Corporation nor the names of its contributors may be     |
   | used to endorse or promote products derived from this software without specific prior written|
   | permission.                                                                                  |
   |                                                                                              |
   | THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS  |
   | OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF              |
   | MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE   |
   | COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,    |
   | EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE|
   | GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED   |
   | AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING    |
   | NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED |
   | OF THE POSSIBILITY OF SUCH DAMAGE.                                                           |
   +----------------------------------------------------------------------------------------------+
   | Module: wincache_utils.h                                                                     |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#ifndef _WINCACHE_UTILS_H_
#define _WINCACHE_UTILS_H_

extern uint32_t     utils_hashcalc(const char * str, size_t strlen);
extern uint32_t     utils_getindex(const char * filename, unsigned int numfiles);
extern const char * utils_filepath(zend_file_handle * file_handle);
extern char *       utils_fullpath(const char * filename, size_t filename_len);
extern int          utils_cwdcexec(char * buffer, size_t length);
extern int          utils_filefolder(const char * filepath, size_t flength, char * pbuffer, size_t length);
extern int          utils_apoolpid();
extern unsigned int utils_ticksdiff(unsigned int present, unsigned int past);
extern char *       utils_resolve_path(const char *filename, size_t filename_length, const char *path);
extern char *       utils_build_temp_filename(char * suffix);
extern void         utils_get_filename_and_line(const char **filename, uint *linenumber);
extern void         utils_wait_for_listener(const char * respath, unsigned int timeout);

extern int
utils_create_init_event(
    char * prefix,
    char * name,
    HANDLE *pinit_event,
    unsigned char *pisfirst
    );

#if (defined(_MSC_VER) && (_MSC_VER < 1500))
extern int wincache_php_snprintf_s(char *buf, size_t len, size_t len2, const char *format,...);
#endif

extern const char *
utils_get_apppool_name();

extern int
utils_set_apppool_acl(
    char * filename
    );

extern int          utils_revert_if_necessary(HANDLE *phOriginalToken);
extern void         utils_reimpersonate_if_necessary(HANDLE hOriginalToken);

extern const char * utils_get_typename(zend_uchar type);

#endif /* _WINCACHE_UTILS_H_ */
