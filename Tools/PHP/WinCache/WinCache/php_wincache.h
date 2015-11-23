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
   | Module: php_wincache.h                                                                       |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#ifndef _PHP_WINCACHE_H_
#define _PHP_WINCACHE_H_

extern zend_module_entry wincache_module_entry;
#define phpext_wincache_ptr &wincache_module_entry

/* For static builds we need all the headers as this */
/* file is included in main/internal_functions.c */
#ifndef COMPILE_DL_WINCACHE
#include "precomp.h"
#endif

typedef struct wclock_context wclock_context;
struct wclock_context
{
    lock_context *           lockobj;     /* Internal lock context for this wclock */
    unsigned int             tcreate;     /* Tick count when this was created */
    unsigned int             tuse;        /* Tick count when this was last used */
};

/* Module globals */
ZEND_BEGIN_MODULE_GLOBALS(wincache)
    aplist_context *         lfcache;     /* Shared memory for fcache filelist */
    zvcache_context *        zvucache;    /* User controlled user cache */
    zvcache_context *        zvscache;    /* Zval cache used to store session data */
    HashTable *              phscache;    /* Hashtable for session caches for modified savepaths */
    unsigned int             numfiles;    /* Configured number of files to handle */
    unsigned int             fcchkfreq;   /* File change check frequency in seconds */
    unsigned int             ttlmax;      /* Seconds a cache entry can stay dormant */
                                          /* Pointer to the original rmdir function */
    void                    (*orig_rmdir)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original file_exists function */
    void                    (*orig_file_exists)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original file_get_contents function */
    void                    (*orig_file_get_contents)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original filesize function */
    void                    (*orig_filesize)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original is_dir function */
    void                    (*orig_is_dir)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original is_file function */
    void                    (*orig_is_file)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original is_readable function */
    void                    (*orig_is_readable)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original is_writable function */
    void                    (*orig_is_writable)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original is_writeable function */
    void                    (*orig_is_writeable)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original readfile function */
    void                    (*orig_readfile)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original realpath function */
    void                    (*orig_realpath)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original unlink function */
    void                    (*orig_unlink)(INTERNAL_FUNCTION_PARAMETERS);
                                          /* Pointer to the original rename function */
    void                    (*orig_rename)(INTERNAL_FUNCTION_PARAMETERS);
    zend_bool                enablecli;   /* Enable wincache for command line sapi */
    zend_bool                fcenabled;   /* File cache enabled or disabled */
    unsigned int             fcachesize;  /* File cache size in MBs */
    unsigned int             maxfilesize; /* Max file size (kb) allowed in fcache */
    zend_bool                ucenabled;   /* User cache enabled or disabled */
    unsigned int             ucachesize;  /* User cache size in MBs */
    unsigned int             scachesize;  /* Session cache size in MBs */
    unsigned int             debuglevel;  /* Debug dump level (0/101/201/301/401/501) */
    char *                   ignorelist;  /* Pipe-separated list of files to ignore */
    char *                   fcefilter;   /* Comma-separated sitelist having fcenabled toggled */
    char *                   namesalt;    /* Salt to use in all the names */
    zend_bool                fcndetect;   /* File change notication detection enabled */
    zend_bool                localheap;   /* Local heap is enabled or disabled */

    HashTable *              wclocks;     /* Locks created using wincache_lock call */
    HashTable *              zvcopied;    /* Copied zvals to make refcounting work */
    unsigned int             lasterror;   /* Last error value */
    unsigned int             uclasterror; /* Last error value encountered by user cache */
    unsigned int             parentpid;   /* Parent process identifier */
    filemap_global_context * fmapgdata;   /* Global data for filemap */
    zend_llist *             errmsglist;  /* List of errors generated by PHP */
    zend_ini_entry *         inifce;      /* fcenabled ini_entry in ini_directives */
    zend_ini_entry *         inisavepath; /* save_path ini_entry in ini_directives */
    unsigned char            dofctoggle;  /* Do toggle of fcenabled due to filter settigns */
                                          /* Enable wrapper functions around standard PHP functions */
    zend_bool                reroute_enabled;
    const char *             apppoolid;   /* The application id. */
    char *                   filemapdir;  /* Directory where temp filemap files should be created */
ZEND_END_MODULE_GLOBALS(wincache)

ZEND_EXTERN_MODULE_GLOBALS(wincache)

#ifdef ZTS
#define WCG(v) ZEND_TSRMG(wincache_globals_id, zend_wincache_globals *, v)
# ifdef COMPILE_DL_WINCACHE
ZEND_TSRMLS_CACHE_EXTERN();
# endif
#else
#define WCG(v) (wincache_globals.v)
#endif

typedef zend_string *(*fn_zend_resolve_path)(const char *filename, int filename_len);
typedef int (*fn_zend_stream_open_function)(const char * filename, zend_file_handle *handle);
typedef zend_op_array * (*fn_zend_compile_file)(zend_file_handle *, int);
typedef void (*fn_zend_error_cb)(int type, const char *error_filename, const uint error_lineno, const char *format, va_list args);

fn_zend_resolve_path original_resolve_path;
fn_zend_stream_open_function original_stream_open_function;

extern zend_string * wincache_resolve_path(const char * filename, int filename_len);
extern int wincache_stream_open_function(const char * filename, zend_file_handle * file_handle);

extern void wincache_intercept_functions_init();
extern void wincache_intercept_functions_shutdown();
extern void wincache_save_orig_functions();

#endif /* _PHP_WINCACHE_H_ */
