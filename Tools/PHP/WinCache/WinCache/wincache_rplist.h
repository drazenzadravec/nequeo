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
   | Module: wincache_rplist.h                                                                    |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   +----------------------------------------------------------------------------------------------+
*/

#ifndef _WINCACHE_RPLIST_H_
#define _WINCACHE_RPLIST_H_

/* rplist_value   - SHARED */
/* rplist_header  - SHARED */
/* rplist_context - LOCAL  */

#define VERIFICATION_NOTDONE 0
#define VERIFICATION_PASSED  1
#define VERIFICATION_FAILED  2

typedef struct rplist_value rplist_value;
struct rplist_value
{
    size_t             file_path;  /* resolve file path */
    size_t             cwd_cexec;  /* cwdir|cexec */
    size_t             inc_path;   /* current include path */
    size_t             open_based; /* open_basedir set */

    unsigned int       is_deleted; /* If set to 1, entry is marked deleted */
    unsigned int       is_verified;/* entry passed openbased check status */
    size_t             absentry;   /* offset of entry in aplist */
    size_t             same_value; /* rplist entry pointing to same absentry */
    size_t             prev_value; /* previous rplist_value offset */
    size_t             next_value; /* next rplist_value offset */
};

typedef struct rplist_header rplist_header;
struct rplist_header
{
    unsigned int       itemcount;  /* Number of valid items */
    unsigned int       last_owner; /* PID of last owner of lock */
    unsigned int       valuecount; /* Total values starting from last entry */
    size_t             values[1];  /* valuecount rplist_value offsets */
};

typedef struct rplist_context rplist_context;
struct rplist_context
{
    char *             rpmemaddr;  /* base memory address of rplist */
    rplist_header *    rpheader;   /* rplist cache header */
    filemap_context *  rpfilemap;  /* filemap where rplist is kept */
    lock_context *     rplock;     /* lock for rplist_header */
    alloc_context *    rpalloc;    /* alloc context for rplist segment */
};

typedef struct rplist_entry_info rplist_entry_info;
struct rplist_entry_info
{
    char *              pathkey;   /* resolve file path */
    char *              subkey;    /* cwdir|cexec subkey entry */
    char *              abspath;   /* absolute path to use */
    rplist_entry_info * next;      /* next entry */
};

typedef struct rplist_info rplist_info;
struct rplist_info
{
    unsigned int        itemcount; /* Total number of items in subcache */
    rplist_entry_info * entries;   /* Individual entries */
};

extern int  rplist_create(rplist_context ** ppcache);
extern void rplist_destroy(rplist_context * pcache);
extern int  rplist_initialize(rplist_context * pcache, unsigned short islocal, unsigned char isfirst, unsigned short cachekey, unsigned int filecount);
extern void rplist_initheader(rplist_context * pcache, unsigned int filecount);
extern void rplist_terminate(rplist_context * pcache);

extern int  rplist_getentry(rplist_context * pcache, const char * filename, rplist_value ** ppvalue, size_t * poffset);
extern void rplist_setabsval(rplist_context * pcache, rplist_value * pvalue, size_t absentry, size_t prevsame);
extern void rplist_deleteval(rplist_context * pcache, size_t valoffset);
extern void rplist_markdeleted(rplist_context * pcache, size_t valoffset);
extern int  rplist_getinfo(rplist_context * pcache, zend_bool summaryonly, rplist_info ** ppinfo);
extern void rplist_freeinfo(rplist_info * pinfo);

extern void rplist_runtest();

#endif /* _WINCACHE_RPLIST_H_ */
