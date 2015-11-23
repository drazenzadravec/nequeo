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
   | Module: wincache_zvcache.c                                                                   |
   +----------------------------------------------------------------------------------------------+
   | Author: Kanwaljeet Singla <ksingla@microsoft.com>                                            |
   | Revised: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

#include "precomp.h"

/*
 * NOTES:
 *
 * copyin_* - Copies zend types into memory cache by performing a deep copy.
 * In the process, all pointers are converted to offsets from the beginning of
 * the memory cache segment.  This is done because the user cache is allocated
 * in shared memory at arbitrary addresses in each process.
 *
 * copyout_* - Creates new instances of zend types that were previously copied
 * into the memory cache.  This involves converting offsets into pointers which
 * are valid within the current process.  All objects should be marked as "copy-
 * on-write", such that the data that lives in the memory cache is not modified.
 *
 */

/*
 * zvcache uses pointers as HashTable index values.  Therefore, we need
 * zend_long's to be at least as big as size_t.
 */
C_ASSERT(SIZEOF_SIZE_T <= SIZEOF_ZEND_LONG);

#define PER_RUN_SCAVENGE_COUNT       16

#ifndef WINCACHE_MAX_ZVKEY_LENGTH
#define WINCACHE_MAX_ZVKEY_LENGTH    0xFFFF
#endif /* WINCACHE_MAX_ZVKEY_LENGTH */


#define ZMALLOC(pcopy, size)         (pcopy->fnmalloc(pcopy->palloc, pcopy->hoffset, size))
#define ZREALLOC(pcopy, addr, size)  (pcopy->fnrealloc(pcopy->palloc, pcopy->hoffset, addr, size))
#define ZSTRDUP(pcopy, pstr)         (pcopy->fnstrdup(pcopy->palloc, pcopy->hoffset, pstr))
#define ZFREE(pcopy, addr)           (pcopy->fnfree(pcopy->palloc, pcopy->hoffset, addr))

#define ZOFFSET(pcopy, pbuffer)      ((pbuffer == NULL) ? 0 : (((char *)pbuffer) - (pcopy)->pbaseadr))
#define ZVALUE(pcopy, offset)        ((offset == 0) ? NULL : ((pcopy)->pbaseadr + (offset)))
#define ZVCACHE_VALUE(p, o)          ((zvcache_value *)alloc_get_cachevalue(p, o))
#define ZVALUE_HT_GET_DATA_ADDR(pcopy, ht) \
  ((char*)(ZVALUE((pcopy),(size_t)(ht)->arData)) - HT_HASH_SIZE((ht)->nTableMask))

static int  copyin_memory(zvcopy_context * pcopy, HashTable * phtable, void * src, size_t size, void **ppdest, zend_uchar *pallocated);
static int  copyin_zval(zvcache_context * pcache, zvcopy_context * pcopy, HashTable * phtable, zval * poriginal, zval ** ppcopied);
static int  copyin_hashtable(zvcache_context * pcache, zvcopy_context * pcopy, HashTable * phtable, HashTable * poriginal, HashTable ** ppcopied);
static int  copyin_string(zvcopy_context * pcopy, HashTable *phxlate, zend_string * orig_string, zend_string ** new_string);
static int  copyin_reference(zvcache_context * pcache, zvcopy_context * pcopy, HashTable *phtable, zend_reference * orig_ref, zend_reference ** new_ref);

static int  copyout_zval(zvcache_context * pcache, zvcopy_context * pcopy, HashTable * phtable, zval * pcached, zval ** ppcopied);
static int  copyout_hashtable(zvcache_context * pcache, zvcopy_context * pcopy, HashTable * phtable, HashTable * pcached, HashTable ** ppcopied);
static int  copyout_reference(zvcache_context * pcache, zvcopy_context * pcopy, HashTable * phtable, zend_reference * pcached, zend_reference ** ppnew_ref);

static int  find_zvcache_entry(zvcache_context * pcache, const char * key, unsigned int index, zvcache_value ** ppvalue);
static int  create_zvcache_data(zvcache_context * pcache, const char * key, zval * pzval, unsigned int ttl, zvcache_value ** ppvalue);
static void destroy_zvcache_data(zvcache_context * pcache, zvcache_value * pvalue);
static void add_zvcache_entry(zvcache_context * pcache, unsigned int index, zvcache_value * pvalue);
static void remove_zvcache_entry(zvcache_context * pcache, unsigned int index, zvcache_value * pvalue);
static void run_zvcache_scavenger(zvcache_context * pcache);

/* Globals */
static unsigned short gzvcacheid = 1;
#pragma warning( push )
#pragma warning( disable : 4146 ) /* we do negative indexing with unsigneds */
static const uint32_t uninitialized_bucket[-HT_MIN_MASK] = {HT_INVALID_IDX, HT_INVALID_IDX};
#pragma warning( pop )

/* Private functions */
static int copyin_memory(zvcopy_context * pcopy, HashTable * phtable, void * src, size_t size, void **ppdest, zend_uchar *pallocated)
{
    int         result      = NONFATAL;
    void *      pdest       = NULL;

    _ASSERT(pallocated  != NULL);
    _ASSERT(ppdest      != NULL);
    _ASSERT(*ppdest     == NULL);

    *pallocated = 0;

    if(phtable != NULL && phtable->nTableSize)
    {
        /* Check if zval is already copied */
        if((pdest = zend_hash_index_find_ptr(phtable, (zend_ulong)src)) != NULL)
        {
            *ppdest = pdest;
            goto Finished;
        }
    }

    /* Allocate cache memory & copy the original into the cache. */
    pdest = (zend_string *)ZMALLOC(pcopy, size);
    if (pdest == NULL)
    {
        result = pcopy->oomcode;
        goto Finished;
    }

    *pallocated = 1;

    pcopy->allocsize += size;
    memcpy_s(pdest, size, src, size);

    /* update the table */
    if(phtable != NULL && phtable->nTableSize)
    {
        zend_hash_index_update_ptr(phtable, (zend_ulong)src, (void *)pdest);
    }

    *ppdest = pdest;

Finished:

    return result;
}

static int copyin_zval(zvcache_context * pcache, zvcopy_context * pcopy, HashTable * phtable, zval * poriginal, zval ** ppcopied)
{
    int                  result     = NONFATAL;
    zval *               pnewzv     = NULL;
    zval *               pfromht    = NULL;
    zend_uchar           allocated  = 0;
    char *               pbuffer    = NULL;
    size_t               length     = 0;
    HashTable *          phashtable = NULL;
    zvcopy_context *     phashcopy  = NULL;
    size_t               offset     = 0;
    php_serialize_data_t serdata    = {0};
    smart_str            smartstr   = {0};
    unsigned char        isfirst    = 0;
    zend_string *        pzstr      = NULL;
    zvcache_hashtable_pool_tracker * table_tracker = NULL;
    zend_reference *     pzref      = NULL;

    dprintverbose("start copyin_zval");

    _ASSERT(pcache    != NULL);
    _ASSERT(pcopy     != NULL);
    _ASSERT(poriginal != NULL);
    _ASSERT(ppcopied  != NULL);

    if (NULL == *ppcopied)
    {
        result = copyin_memory(pcopy, phtable, poriginal, sizeof(zval), &pnewzv, &allocated);
        if (FAILED(result))
        {
            goto Finished;
        }

        if (!allocated)
        {
            *ppcopied = pnewzv;
            goto Finished;
        }
    }
    else
    {
        /* copy into an already allocated zval */
        pnewzv = *ppcopied;
        if (poriginal != pnewzv)
        {
            /* struct copy to pick up flags and such.  Still need to fix up value pointer. */
            *pnewzv = *poriginal;
        }
    }

    switch(Z_TYPE_P(poriginal))
    {
        case IS_RESOURCE:
            _ASSERT(FALSE);
            result = FATAL_ZVCACHE_INVALID_ZVAL;
            goto Finished;

        case IS_UNDEF:
        case IS_NULL:
        case IS_TRUE:
        case IS_FALSE:
        case IS_LONG:
        case IS_DOUBLE:
            break;

        case IS_STRING:
        case IS_CONSTANT:
            result = copyin_string(pcopy, phtable, Z_STR_P(poriginal), &pzstr);
            if (FAILED(result))
            {
                goto Finished;
            }

            /* Fix up the zval type flags */
            Z_TYPE_FLAGS_P(pnewzv) &= ~(IS_TYPE_REFCOUNTED | IS_TYPE_COPYABLE);

            /* Use offset in cached zval pointer */
            Z_STR_P(pnewzv) = (zend_string *)ZOFFSET(pcopy, pzstr);
            break;

        case IS_ARRAY:
            /* Initialize zvcopied hashtable for direct call to copyin_zval */
            if(phtable == NULL)
            {
                phtable = WCG(zvcopied);
                isfirst = 1;
                zend_hash_init(phtable, 0, NULL, NULL, 0);
            }

            /*
             * Because we need to remember the offset of the memory pool, all
             * HashTables that are copied into shared memory will use the *ptr
             * field, instead of the *arr field.
             * The *ptr will point at a zvcache_hashtable_pool_tracker structure.
             */

            table_tracker = (zvcache_hashtable_pool_tracker *)ZMALLOC(pcopy, sizeof(zvcache_hashtable_pool_tracker));
            if (table_tracker == NULL)
            {
                result = pcopy->oomcode;
                goto Finished;
            }
            pcopy->allocsize += sizeof(zvcache_hashtable_pool_tracker);

            /* If pcopy is not set to use memory pool, use a different copy context */
            if(pcopy->hoffset == 0)
            {
                _ASSERT(pcache->incopy == pcopy);
                phashcopy = (zvcopy_context *)alloc_pemalloc(sizeof(zvcopy_context));
                if(phashcopy == NULL)
                {
                    result = FATAL_OUT_OF_LMEMORY;
                    goto Finished;
                }

                result = alloc_create_mpool(pcache->zvalloc, &offset);
                if(FAILED(result))
                {
                    goto Finished;
                }

                phashcopy->oomcode   = FATAL_OUT_OF_SMEMORY;
                phashcopy->palloc    = pcache->zvalloc;
                phashcopy->pbaseadr  = pcache->zvmemaddr;
                phashcopy->hoffset   = offset;
                phashcopy->allocsize = 0;
                phashcopy->fnmalloc  = alloc_ommalloc;
                phashcopy->fnrealloc = alloc_omrealloc;
                phashcopy->fnstrdup  = alloc_omstrdup;
                phashcopy->fnfree    = alloc_omfree;

                result = copyin_hashtable(pcache, phashcopy, phtable, Z_ARR_P(poriginal), &phashtable);
                pcopy->allocsize += phashcopy->allocsize;
            }
            else
            {
                result = copyin_hashtable(pcache, pcopy, phtable, Z_ARR_P(poriginal), &phashtable);
            }

            if(isfirst)
            {
                _ASSERT(phtable != NULL);

                zend_hash_destroy(phtable);
                phtable->nTableSize = 0;

                phtable = NULL;
            }

            if(FAILED(result))
            {
                goto Finished;
            }

            /* fix up the flags on the copied in zval */
            Z_TYPE_FLAGS_P(pnewzv) = IS_TYPE_IMMUTABLE;
            GC_REFCOUNT(Z_COUNTED_P(pnewzv)) = 2;
            GC_FLAGS(Z_COUNTED_P(pnewzv)) |= IS_ARRAY_IMMUTABLE;

            /* Keep offset so that freeing shared memory is easy */
            table_tracker->hoff = offset;
            table_tracker->val = ZOFFSET(pcopy, phashtable);
            phashtable = NULL;
            Z_PTR_P(pnewzv) = (void *)ZOFFSET(pcopy, table_tracker);
            table_tracker = NULL;
            break;

        case IS_REFERENCE:
            result = copyin_reference(pcache, pcopy, phtable, Z_REF_P(poriginal), &pzref);
            if(FAILED(result))
            {
                goto Finished;
            }
            Z_REF_P(pnewzv) = (zend_reference *)ZOFFSET(pcopy, pzref);
            break;

        case IS_OBJECT:
            /* Serialize object and store in string */
            PHP_VAR_SERIALIZE_INIT(serdata);
            php_var_serialize(&smartstr, poriginal, &serdata);
            PHP_VAR_SERIALIZE_DESTROY(serdata);

            if(smartstr.s && ZSTR_LEN(smartstr.s))
            {
                size_t size = _ZSTR_STRUCT_SIZE(ZSTR_LEN(smartstr.s));
                zend_string * pdest = NULL;
                pdest = (zend_string *)ZMALLOC(pcopy, size);
                if (pdest == NULL)
                {
                    result = pcopy->oomcode;
                    goto Finished;
                }

                /* Allocate the zend_string & copy the original into the cache. */
                pcopy->allocsize += size;
                memcpy_s(pdest, size, smartstr.s, size);

                /* Use Offset in copied-in zval */
                Z_PTR_P(pnewzv) = (zend_string *)ZOFFSET(pcopy, pdest);
                smart_str_free(&smartstr);
            }
            else
            {
                ZVAL_NULL(pnewzv);
            }

            break;

        default:
            result = FATAL_ZVCACHE_INVALID_ZVAL;
            goto Finished;
    }

    *ppcopied = pnewzv;
    _ASSERT(SUCCEEDED(result));

Finished:

    if(phashcopy != NULL)
    {
        alloc_pefree(phashcopy);
        phashcopy = NULL;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in copyin_zval", result);

        if(pnewzv != NULL)
        {
            if(allocated == 1 && offset == 0)
            {
                ZFREE(pcopy, pnewzv);
                pnewzv = NULL;
            }

            if(offset != 0)
            {
                alloc_free_mpool(pcache->zvalloc, offset);
                offset = 0;
            }
        }
    }

    dprintverbose("end copyin_zval");

    return result;
}

static int copyout_zval(zvcache_context * pcache, zvcopy_context * pcopy, HashTable * phtable, zval * pcached, zval ** ppcopied)
{
    int                    result     = NONFATAL;
    zval *                 pnewzv     = NULL;
    zval *                 pfromht    = NULL;
    zend_string *          ptmp_str   = NULL;
    int                    allocated  = 0;
    char *                 pbuffer    = NULL;
    HashTable *            phashtable = NULL;
    php_unserialize_data_t serdata    = {0};
    unsigned char          isfirst    = 0;
    zend_uchar             added_to_phtable = 0;
    zvcache_hashtable_pool_tracker * table_tracker = NULL;
    zend_reference *       pref       = NULL;

    dprintverbose("start copyout_zval");

    _ASSERT(pcache    != NULL);
    _ASSERT(pcopy     != NULL);
    _ASSERT(pcached   != NULL);
    _ASSERT(ppcopied  != NULL);

    if(phtable != NULL && phtable->nTableSize)
    {
        /* Check if zval is already copied */
        if((pfromht = zend_hash_index_find_ptr(phtable, (zend_ulong)pcached)) != NULL)
        {
            if (Z_REFCOUNTED_P(pfromht)) { Z_ADDREF_P(pfromht); }
            *ppcopied = pfromht;
            goto Finished;
        }
    }

    /* Allocate memory as required */
    if(*ppcopied == NULL)
    {
        pnewzv = (zval *)ZMALLOC(pcopy, sizeof(zval));
        if(pnewzv == NULL)
        {
            result = pcopy->oomcode;
            goto Finished;
        }

        allocated = 1;
    }
    else
    {
        pnewzv = *ppcopied;
    }

    _ASSERT(pnewzv != NULL);

    if(phtable != NULL && phtable->nTableSize)
    {
        zend_hash_index_update_ptr(phtable, (zend_ulong)pcached, (void *)pnewzv);
        added_to_phtable = 1;
    }

    /*
     * struct copy the zval - this picks up the flags and type info from the
     * cached zval.
     * However, the value field is still an offset into the cache.  Need to
     * convert to an actual pointer before doing anything else.
     */
    *pnewzv = *pcached;

    switch(Z_TYPE_P(pcached))
    {
        case IS_RESOURCE:
            /* We don't support caching of zend_resource types */
            _ASSERT(FALSE);
            result = FATAL_ZVCACHE_INVALID_ZVAL;
            goto Finished;

        case IS_TRUE:
        case IS_FALSE:
        case IS_UNDEF:
        case IS_LONG:
        case IS_DOUBLE:
        case IS_NULL:
            /* Nothing to do */
            break;

        case IS_STRING:
        case IS_CONSTANT:
            /* create a new, non-persistent string from the cached copy */
            ptmp_str = (zend_string *)ZVALUE(pcache->incopy, (size_t)Z_STR_P(pcached));
            Z_STR_P(pnewzv) = zend_string_alloc(ZSTR_LEN(ptmp_str), 0);
            memcpy(ZSTR_VAL(Z_STR_P(pnewzv)), ZSTR_VAL(ptmp_str), ZSTR_LEN(ptmp_str)+1);
            Z_TYPE_INFO_P(pnewzv) = IS_STRING_EX;
            break;

        case IS_ARRAY:
            /* Initialize zvcopied HashTable for first call to copyout_zval */
            if(phtable == NULL)
            {
                phtable = WCG(zvcopied);
                isfirst = 1;
                zend_hash_init(WCG(zvcopied), 0, NULL, NULL, 0);
            }

            table_tracker = (zvcache_hashtable_pool_tracker *)ZVALUE(pcache->incopy, (size_t)Z_PTR_P(pcached));

            result = copyout_hashtable(pcache, pcopy, phtable, (HashTable *)ZVALUE(pcache->incopy, table_tracker->val), &phashtable);

            if(isfirst)
            {
                _ASSERT(phtable != NULL);

                zend_hash_destroy(phtable);
                phtable->nTableSize = 0;

                phtable = NULL;
            }

            if(FAILED(result))
            {
                goto Finished;
            }

            Z_ARR_P(pnewzv) = phashtable;
            phashtable = NULL;
            break;

        case IS_REFERENCE:
            result = copyout_reference(pcache, pcopy, phtable, (zend_reference *)ZVALUE(pcache->incopy, (size_t)Z_REF_P(pcached)), &pref);
            if(FAILED(result))
            {
                goto Finished;
            }
            Z_REF_P(pnewzv) = pref;
            break;

        case IS_OBJECT:
            /* Deserialize stored data to produce object */
            ptmp_str = (zend_string *)ZVALUE(pcache->incopy, (size_t)Z_PTR_P(pcached));
            pbuffer = ZSTR_VAL(ptmp_str);

            PHP_VAR_UNSERIALIZE_INIT(serdata);
            if(!php_var_unserialize(pnewzv, (const unsigned char **)&pbuffer, (const unsigned char *)pbuffer + ZSTR_LEN(ptmp_str), &serdata))
            {
                /* Destroy zval and set return value to null */
                dprintimportant("Failed to unserialize cached object: %s", ZSTR_VAL(ptmp_str));
                ZVAL_NULL(pnewzv);
            }
            PHP_VAR_UNSERIALIZE_DESTROY(serdata);
            break;

        default:
            result = FATAL_ZVCACHE_INVALID_ZVAL;
            goto Finished;
    }

    *ppcopied = pnewzv;
    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in copyout_zval", result);

        if (added_to_phtable == 1)
        {
            zend_hash_index_del(phtable, (zend_ulong)pcached);
        }

        if(pbuffer != NULL)
        {
            ZFREE(pcopy, pbuffer);
            pbuffer = NULL;
        }

        if(pnewzv != NULL)
        {
            if(allocated == 1)
            {
                ZFREE(pcopy, pnewzv);
                pnewzv = NULL;
            }
        }
    }

    dprintverbose("end copyout_zval");

    return result;
}

static int copyin_hashtable(zvcache_context * pcache, zvcopy_context * pcopy, HashTable * phtable, HashTable * poriginal, HashTable ** ppcopied)
{
    int             result     = NONFATAL;
    uint32_t        idx;
    uint32_t        nIndex;
    Bucket *        p          = NULL;
    zend_string *   pzstr      = NULL;
    zend_uchar      allocated  = 0;
    HashTable *     pnew_table = NULL;
    char *          ptemp      = NULL;
    size_t          temp_size  = 0;
    zval *          pzval      = NULL;

    _ASSERT(ppcopied);
    _ASSERT(*ppcopied == NULL);

    dprintverbose("begin copyin_hashtable");

    /* allocate a hashtable & copy over existing table stuff */
    result = copyin_memory(pcopy, phtable, poriginal, sizeof(zend_array), &pnew_table, &allocated);
    if (FAILED(result))
    {
        goto Finished;
    }

    *ppcopied = pnew_table;

    if (allocated == 0)
    {
        /* already copied */
        goto Finished;
    }

    /* Clear out the destructor, since we're taking ownership of the table */
    pnew_table->pDestructor = NULL;

    /* fix up the HashTable flags since we're persisting it */
    pnew_table->u.flags |= HASH_FLAG_STATIC_KEYS;
    pnew_table->u.flags &= ~HASH_FLAG_APPLY_PROTECTION;

    /* Uninitalized */
    if (!(pnew_table->u.flags & HASH_FLAG_INITIALIZED)) {
        HT_SET_DATA_ADDR(pnew_table, 0);
        goto Finished;
    }
    /* Packed (a.k.a. simple array) */
    if (pnew_table->u.flags & HASH_FLAG_PACKED) {
        result = copyin_memory(pcopy, phtable, HT_GET_DATA_ADDR(poriginal), HT_USED_SIZE(poriginal), &ptemp, &allocated);
        if (FAILED(result))
        {
            goto Finished;
        }
        HT_SET_DATA_ADDR(pnew_table, ptemp);
        ptemp = NULL;
    }
#pragma warning( push )
#pragma warning( disable : 4018 ) /* yeah, we do signed/unsigned comparisons */
    /* Sparsely Populated: Need to compress while copying in */
    else if (poriginal->nNumUsed < -(int32_t)poriginal->nTableMask / 2) {
        /* compact table */
        Bucket *old_buckets = poriginal->arData;
        int32_t hash_size = -(int32_t)poriginal->nTableMask;

        while (hash_size >> 1 > poriginal->nNumUsed) {
            hash_size >>= 1;
        }
        pnew_table->nTableMask = -hash_size;
        temp_size = (hash_size * sizeof(uint32_t)) + (poriginal->nNumUsed * sizeof(Bucket));
        ptemp = ZMALLOC(pcopy, temp_size);
        if (!ptemp)
        {
            result = pcopy->oomcode;
            goto Finished;
        }
        pcopy->allocsize += temp_size;

        HT_SET_DATA_ADDR(pnew_table, ptemp);
        ptemp = NULL;
        HT_HASH_RESET(pnew_table);
        memcpy(pnew_table->arData, old_buckets, poriginal->nNumUsed * sizeof(Bucket));

        for (idx = 0; idx < poriginal->nNumUsed; idx++) {
            p = pnew_table->arData + idx;
            if (Z_TYPE(p->val) == IS_UNDEF) continue;

            /* fix up the index table */
            nIndex = ((uint32_t)p->h) | pnew_table->nTableMask;
            Z_NEXT(p->val) = HT_HASH(pnew_table, nIndex);
            HT_HASH(pnew_table, nIndex) = HT_IDX_TO_HASH(idx);
        }
    }
#pragma warning( pop )
    /* Appropriately sized: Copy the whole thing in */
    else {
        result = copyin_memory(pcopy, phtable, HT_GET_DATA_ADDR(poriginal), HT_USED_SIZE(poriginal), &ptemp, &allocated);
        HT_SET_DATA_ADDR(pnew_table, ptemp);
    }

    /* Copy in Keys and Buckets */
    for (idx = 0; idx < poriginal->nNumUsed; idx++) {
        p = pnew_table->arData + idx;
        if (Z_TYPE(p->val) == IS_UNDEF) continue;

        /* persist bucket and key */
        if (p->key) {
            result = copyin_string(pcopy, phtable, p->key, &pzstr);
            if (FAILED(result))
            {
                goto Finished;
            }

            /* Use Offset in copied-in zval */
            p->key = (zend_string *)ZOFFSET(pcopy, pzstr);
            pzstr = NULL;
        }

        /* persist the data itself */
        pzval = &p->val;
        result = copyin_zval(pcache, pcopy, phtable, pzval, &pzval);
        if (FAILED(result))
        {
            goto Finished;
        }
    }

    /* ensure pointer is translated to cache memory offset */
    pnew_table->arData = (Bucket *)ZOFFSET(pcopy, pnew_table->arData);

    *ppcopied = pnew_table;

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in copyin_hashtable", result);
    }

    dprintverbose("end copyin_hashtable");

    return result;
}

static int copyout_hashtable(zvcache_context * pcache, zvcopy_context * pcopy, HashTable * phtable, HashTable * pcached, HashTable ** ppcopied)
{
    int             result     = NONFATAL;
    uint32_t        idx;
    Bucket *        p          = NULL;
    HashTable *     pnew_table = NULL;
    zend_uchar      allocated  = 0;
    char *          ptemp      = NULL;
    zval *          pnewzv     = NULL;
    size_t          tmp_size;
    char *          pdata_temp;
    zend_string *   ptmp_str   = NULL;

    dprintverbose("start copyout_hashtable");

    _ASSERT(pcache    != NULL);
    _ASSERT(pcopy     != NULL);
    _ASSERT(pcached   != NULL);
    _ASSERT(ppcopied  != NULL);

    /* Allocate memory for HashTable as required */
    if(*ppcopied == NULL)
    {
        pnew_table = (HashTable *)ZMALLOC(pcopy, sizeof(HashTable));
        if(pnew_table == NULL)
        {
            result = pcopy->oomcode;
            goto Finished;
        }
        *ppcopied = pnew_table;

        allocated = 1;
    }
    else
    {
        pnew_table = *ppcopied;
    }

    /* initalize from cached entry */
    *pnew_table = *pcached;

    /* Uninitalized */
    if (!(pnew_table->u.flags & HASH_FLAG_INITIALIZED)) {
        HT_SET_DATA_ADDR(pnew_table, &uninitialized_bucket);
        goto Finished;
    }

    tmp_size = HT_USED_SIZE(pcached);
    /* Allocate memory for buckets */
    ptemp = ZMALLOC(pcopy, tmp_size);
    if (!ptemp)
    {
        result = pcopy->oomcode;
        goto Finished;
    }

    pdata_temp = ZVALUE_HT_GET_DATA_ADDR(pcache->incopy,pcached);
    memcpy(ptemp, pdata_temp, tmp_size);
    HT_SET_DATA_ADDR(pnew_table, ptemp);

    /* Copy out Keys and Buckets */
    for (idx = 0; idx < pcached->nNumUsed; idx++) {
        p = pnew_table->arData + idx;
        if (Z_TYPE(p->val) == IS_UNDEF) continue;

        /* copy out key */
        if (p->key) {
            /* Use pointer to copied-in string */
            ptmp_str = (zend_string *)ZVALUE(pcache->incopy, (size_t)p->key);
            p->key = zend_string_alloc(ZSTR_LEN(ptmp_str), 0);
            memcpy(ZSTR_VAL(p->key), ZSTR_VAL(ptmp_str), ZSTR_LEN(ptmp_str)+1);
        }

        /* copy out the data itself */
        pnewzv = &p->val;
        copyout_zval(pcache, pcopy, phtable, pnewzv, &pnewzv);
    }

    /* After copying out all the values, reset the destructor */
    pnew_table->pDestructor = ZVAL_DESTRUCTOR;

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in copyout_hashtable", result);

        if(pnew_table != NULL)
        {
            if(allocated != 0)
            {
                ZFREE(pcopy, pnew_table);
                pnew_table = NULL;
            }
        }

        if (ptemp)
        {
            ZFREE(pcopy, ptemp);
            ptemp = NULL;
        }
    }

    dprintverbose("end copyout_hashtable");

    return result;
}

static int copyin_string(zvcopy_context * pcopy, HashTable *phtable, zend_string * orig_string, zend_string ** new_string)
{
    int                  result     = NONFATAL;
    size_t               size       = 0;
    zend_string *        pzstr      = NULL;
    zend_uchar           allocated  = 0;

    _ASSERT(orig_string != NULL);
    _ASSERT(new_string  != NULL);
    _ASSERT(*new_string == NULL);

    size = _ZSTR_STRUCT_SIZE(ZSTR_LEN(orig_string));
    result = copyin_memory(pcopy, phtable, orig_string, size, &pzstr, &allocated);
    if (FAILED(result))
    {
        goto Finished;
    }

    if (allocated)
    {
        *(((char *)pzstr) + size - 1) = 0;
        zend_string_hash_val(pzstr);
        /*
         * Fetched values will point at the cached zend_string, so we must
         * mark the string as read only, with copy-on-write semantics.
         */
        GC_FLAGS(pzstr) = IS_STR_INTERNED | IS_STR_PERMANENT;
    }

    *new_string = pzstr;

Finished:

    return result;

}

static int copyin_reference(zvcache_context * pcache, zvcopy_context * pcopy, HashTable *phtable, zend_reference * orig_ref, zend_reference ** new_ref)
{
    int                  result     = NONFATAL;
    size_t               size       = 0;
    zend_reference *     pzref      = NULL;
    zval *               pnew_zval  = NULL;
    zend_uchar           allocated  = 0;

    _ASSERT(orig_ref != NULL);
    _ASSERT(new_ref  != NULL);
    _ASSERT(*new_ref == NULL);

    size = sizeof(zend_reference);
    result = copyin_memory(pcopy, phtable, orig_ref, size, &pzref, &allocated);
    if (FAILED(result))
    {
        goto Finished;
    }

    if (allocated)
    {
        pnew_zval = &pzref->val;
        result = copyin_zval(pcache, pcopy, phtable, pnew_zval, &pnew_zval);
        GC_REFCOUNT(pzref) = 2;
    }

    *new_ref = pzref;

Finished:

    return result;

}

static int copyout_reference(zvcache_context * pcache, zvcopy_context * pcopy, HashTable * phtable, zend_reference * pcached, zend_reference ** ppnew_ref)
{
    int                  result     = NONFATAL;
    size_t               size       = 0;
    zend_reference *     pzref      = NULL;
    zval *               pnew_zval  = NULL;

    _ASSERT(pcache    != NULL);
    _ASSERT(pcopy     != NULL);
    _ASSERT(pcached   != NULL);
    _ASSERT(ppnew_ref != NULL);

    /* check table to see if we've already copied this item out*/
    if(phtable != NULL && phtable->nTableSize)
    {
        if((pzref = zend_hash_index_find_ptr(phtable, (zend_ulong)pcached)) != NULL)
        {
            *ppnew_ref = pzref;
            goto Finished;
        }
    }

    /* Allocate memory as required */
    if(*ppnew_ref == NULL)
    {
        pzref = (zend_reference *)ZMALLOC(pcopy, sizeof(zend_reference));
        if(pzref == NULL)
        {
            result = pcopy->oomcode;
            goto Finished;
        }
    }
    else
    {
        pzref = *ppnew_ref;
    }

    *pzref = *pcached;

    /* copy out zval */
    pnew_zval = &pzref->val;
    result = copyout_zval(pcache, pcopy, phtable, pnew_zval, &pnew_zval);
    if (FAILED(result))
    {
        goto Finished;
    }

    /* update the table */
    if(phtable != NULL && phtable->nTableSize)
    {
        zend_hash_index_update_ptr(phtable, (zend_ulong)pcached, (void *)pzref);
    }

    *ppnew_ref = pzref;

Finished:

    return result;

}

/* Call this method atleast under a read lock */
static int find_zvcache_entry(zvcache_context * pcache, const char * key, unsigned int index, zvcache_value ** ppvalue)
{
    int              result = NONFATAL;
    zvcache_header * header = NULL;
    zvcache_value *  pvalue = NULL;
    unsigned int     tdiff  = 0;
    unsigned int     ticks  = 0;

    dprintverbose("start find_zvcache_entry");

    _ASSERT(pcache  != NULL);
    _ASSERT(key     != NULL);
    _ASSERT(ppvalue != NULL);

    *ppvalue  = NULL;

    header = pcache->zvheader;
    pvalue = ZVCACHE_VALUE(pcache->zvalloc, header->values[index]);

    while(pvalue != NULL)
    {
        if(strcmp(pcache->zvmemaddr + pvalue->keystr, key) == 0)
        {
            if(pvalue->ttlive > 0)
            {
                /* NOTE: Session cache calculates ttl from last use, whereas
                 * User cache calculates from the time the entry was added
                 */
                ticks = (pcache->issession ? pvalue->use_ticks : pvalue->add_ticks);

                /* Check if the entry is not expired and */
                /* Stop looking only if entry is not stale */
                tdiff = utils_ticksdiff(0, ticks) / 1000;
                if(tdiff <= pvalue->ttlive)
                {
                    break;
                }
            }
            else
            {
                /* ttlive 0 means entry is valid unless deleted */
                break;
            }
        }

        pvalue = ZVCACHE_VALUE(pcache->zvalloc, pvalue->next_value);
    }

    *ppvalue = pvalue;

    dprintverbose("end find_zvcache_entry");

    return result;
}

static int create_zvcache_data(zvcache_context * pcache, const char * key, zval * pzval, unsigned int ttl, zvcache_value ** ppvalue)
{
    int              result  = NONFATAL;
    zvcache_value *  pvalue  = NULL;
    size_t           keylen  = 0;
    zval *           pcopied = NULL;
    zvcopy_context * pcopy   = NULL;
    unsigned int     cticks  = 0;

    dprintverbose("start create_zvcache_data");

    _ASSERT(pcache   != NULL);
    _ASSERT(key      != NULL);
    _ASSERT(pzval    != NULL);
    _ASSERT(ppvalue  != NULL);
    _ASSERT(*ppvalue == NULL);

    *ppvalue = NULL;
    pcopy = pcache->incopy;

    keylen = strlen(key) + 1;
    _ASSERT(keylen < 4098);  /* NOTE: No clue why 4098 */
    if (keylen > WINCACHE_MAX_ZVKEY_LENGTH)
    {
        result = FATAL_ZVCACHE_INVALID_KEY_LENGTH;
        goto Finished;
    }

    pvalue = (zvcache_value *)ZMALLOC(pcopy, sizeof(zvcache_value) + keylen);
    if(pvalue == NULL)
    {
        result = pcopy->oomcode;
        goto Finished;
    }

    /* Only set zvcache_value to 0. Set keyname after zvcache_value */
    memset(pvalue, 0, sizeof(zvcache_value));
    memcpy_s((char *)pvalue + sizeof(zvcache_value), keylen, key, keylen);

    /* Reset allocsize before calling copyin */
    pcopy->allocsize = 0;

    result = copyin_zval(pcache, pcopy, NULL, pzval, &pcopied);
    if(FAILED(result))
    {
        goto Finished;
    }

    pvalue->keystr     = ZOFFSET(pcopy, pvalue) + sizeof(zvcache_value);
    pvalue->keylen     = (unsigned short)(keylen - 1);
    pvalue->sizeb      = pcopy->allocsize;
    pvalue->zvalue     = ZOFFSET(pcopy, pcopied);

    cticks = GetTickCount();
    pvalue->add_ticks  = cticks;
    pvalue->use_ticks  = cticks;

    pvalue->ttlive     = ttl;
    pvalue->hitcount   = 0;
    pvalue->next_value = 0;
    pvalue->prev_value = 0;

    *ppvalue = pvalue;
    _ASSERT(SUCCEEDED(result));

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in create_zvcache_data", result);

        if(pvalue != NULL)
        {
            ZFREE(pcopy, pvalue);
            pvalue = NULL;
        }
    }

    dprintverbose("end create_zvcache_data");

    return result;
}

static void destroy_zvcache_data(zvcache_context * pcache, zvcache_value * pvalue)
{
    zval * pzval;
    zvcache_hashtable_pool_tracker * table_tracker = NULL;


    dprintverbose("start destroy_zvcache_data");

    if(pvalue != NULL)
    {
        if(pvalue->zvalue != 0)
        {
            pzval = (zval *)ZVALUE(pcache->incopy, pvalue->zvalue);

            switch(Z_TYPE_P(pzval))
            {
                case IS_UNDEF:
                case IS_NULL:
                case IS_TRUE:
                case IS_FALSE:
                case IS_LONG:
                case IS_DOUBLE:
                    break;

                case IS_STRING:
                case IS_CONSTANT:
                    ZFREE(pcache->incopy, ZVALUE(pcache->incopy, (size_t)Z_STR_P(pzval)));
                    Z_STR_P(pzval) = NULL;
                    break;

                case IS_ARRAY:
                    table_tracker = (zvcache_hashtable_pool_tracker *)ZVALUE(pcache->incopy, (size_t)Z_PTR_P(pzval));
                    alloc_free_mpool(pcache->zvalloc, table_tracker->hoff);
                    table_tracker->hoff = 0;
                    table_tracker->val = 0;
                    ZFREE(pcache->incopy, ZVALUE(pcache->incopy, (size_t)Z_PTR_P(pzval)));
                    Z_PTR_P(pzval) = NULL;
                    table_tracker = NULL;
                    break;

                case IS_REFERENCE:
                    ZFREE(pcache->incopy, ZVALUE(pcache->incopy, (size_t)Z_REF_P(pzval)));
                    Z_REF_P(pzval) = NULL;
                    break;

                case IS_OBJECT:
                    ZFREE(pcache->incopy, ZVALUE(pcache->incopy, (size_t)Z_PTR_P(pzval)));
                    Z_PTR_P(pzval) = NULL;
                    break;

                default:
                    dprintimportant("destroy_zvcache_data: Can't free unsupported type %d", Z_TYPE_P(pzval));
            }

            ZFREE(pcache->incopy, pzval);
            pvalue->zvalue = 0;
            pzval = NULL;
        }

        /* No need to call free for keystr as value and keystr are allocated together */
        ZFREE(pcache->incopy, pvalue);
        pvalue = NULL;
    }

    dprintverbose("end destroy_zvcache_data");

    return;
}

static void add_zvcache_entry(zvcache_context * pcache, unsigned int index, zvcache_value * pvalue)
{
    zvcache_header * header  = NULL;
    zvcache_value *  pcheck  = NULL;

    dprintverbose("start add_zvcache_entry");

    _ASSERT(pcache         != NULL);
    _ASSERT(pvalue         != NULL);
    _ASSERT(pvalue->keystr != 0);

    header = pcache->zvheader;
    pcheck = ZVCACHE_VALUE(pcache->zvalloc, header->values[index]);

    while(pcheck != NULL)
    {
        if(pcheck->next_value == 0)
        {
            break;
        }

        pcheck = ZVCACHE_VALUE(pcache->zvalloc, pcheck->next_value);
    }

    if(pcheck != NULL)
    {
        pcheck->next_value = alloc_get_valueoffset(pcache->zvalloc, pvalue);
        pvalue->next_value = 0;
        pvalue->prev_value = alloc_get_valueoffset(pcache->zvalloc, pcheck);
    }
    else
    {
        header->values[index] = alloc_get_valueoffset(pcache->zvalloc, pvalue);
        pvalue->next_value = 0;
        pvalue->prev_value = 0;
    }

    header->itemcount++;

    dprintverbose("end add_zvcache_entry");

    return;
}

/* Call this method under write lock */
static void remove_zvcache_entry(zvcache_context * pcache, unsigned int index, zvcache_value * pvalue)
{
    alloc_context *   apalloc = NULL;
    zvcache_header *  header  = NULL;
    zvcache_value *   ptemp   = NULL;

    dprintverbose("start remove_zvcache_entry");

    _ASSERT(pcache         != NULL);
    _ASSERT(pvalue         != NULL);
    _ASSERT(pvalue->keystr != 0);

    apalloc = pcache->zvalloc;
    header  = pcache->zvheader;

    /* Decrement itemcount and remove entry from hashtable */
    header->itemcount--;

    if(pvalue->prev_value == 0)
    {
        header->values[index] = pvalue->next_value;
        if(pvalue->next_value != 0)
        {
            ptemp = ZVCACHE_VALUE(apalloc, pvalue->next_value);
            ptemp->prev_value = 0;
        }
    }
    else
    {
        ptemp = ZVCACHE_VALUE(apalloc, pvalue->prev_value);
        ptemp->next_value = pvalue->next_value;

        if(pvalue->next_value != 0)
        {
            ptemp = ZVCACHE_VALUE(apalloc, pvalue->next_value);
            ptemp->prev_value = pvalue->prev_value;
        }
    }

    /* Destroy aplist data now that fcache is deleted */
    destroy_zvcache_data(pcache, pvalue);
    pvalue = NULL;

    dprintverbose("end remove_zvcache_entry");
    return;
}

/* Call this method under lock */
static void run_zvcache_scavenger(zvcache_context * pcache)
{
    unsigned int sindex   = 0;
    unsigned int eindex   = 0;
    unsigned int ticks    = 0;
    unsigned int tickdiff = 0;

    zvcache_header * pheader = NULL;
    alloc_context *  palloc  = NULL;
    zvcache_value *  pvalue  = NULL;
    zvcache_value *  ptemp   = NULL;

    dprintverbose("start run_zvcache_scavenger");

    _ASSERT(pcache != NULL);

    pheader = pcache->zvheader;
    palloc  = pcache->zvalloc;
    ticks   = GetTickCount();

    /* Only run if lscavenge happened atleast 10 seconds ago */
    if(utils_ticksdiff(ticks, pheader->lscavenge) < 10000)
    {
        goto Finished;
    }

    pheader->lscavenge = ticks;

    /* Run scavenger starting from start */
    sindex = pheader->scstart;
    eindex = sindex + PER_RUN_SCAVENGE_COUNT;
    pheader->scstart = eindex;

    if(eindex >= pheader->valuecount)
    {
        eindex = pheader->valuecount;
        pheader->scstart = 0;
    }

    dprintimportant("zvcache scavenger sindex = %d, eindex = %d", sindex, eindex);
    for( ;sindex < eindex; sindex++)
    {
        if(pheader->values[sindex] == 0)
        {
            continue;
        }

        pvalue = ZVCACHE_VALUE(palloc, pheader->values[sindex]);
        while(pvalue != NULL)
        {
            ptemp = pvalue;
            pvalue = ZVCACHE_VALUE(palloc, pvalue->next_value);

            /* If ttlive is 0, entry will stay unless deleted */
            if(ptemp->ttlive == 0)
            {
                continue;
            }

            /* Remove the entry from cache if its expired */
            tickdiff = utils_ticksdiff(ticks, ptemp->use_ticks) / 1000;
            if(tickdiff >= ptemp->ttlive)
            {
                remove_zvcache_entry(pcache, sindex, ptemp);
                ptemp = NULL;
            }
        }
    }

Finished:

    dprintverbose("end run_aplist_scavenger");
    return;
}

/* Public functions */
int zvcache_create(zvcache_context ** ppcache)
{
    int               result = NONFATAL;
    zvcache_context * pcache = NULL;

    dprintverbose("start zvcache_create");

    _ASSERT(ppcache != NULL);
    *ppcache = NULL;

    pcache = (zvcache_context *)alloc_pemalloc(sizeof(zvcache_context));
    if(pcache == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    pcache->id          = gzvcacheid++;
    pcache->islocal     = 0;
    pcache->cachekey    = 0;
    pcache->hinitdone   = NULL;
    pcache->issession   = 0;

    pcache->incopy      = NULL;
    pcache->outcopy     = NULL;

    pcache->zvmemaddr   = NULL;
    pcache->zvheader    = NULL;
    pcache->zvfilemap   = NULL;
    pcache->zvlock      = NULL;
    pcache->zvalloc     = NULL;

    *ppcache = pcache;

Finished:

    if(FAILED(result))
    {
        dprintimportant("failure %d in zvcache_create", result);
    }

    dprintverbose("end zvcache_create %p", pcache);

    return result;
}

void zvcache_destroy(zvcache_context * pcache)
{
    dprintverbose("start zvcache_destroy %p", pcache);

    if(pcache != NULL)
    {
        alloc_pefree(pcache);
        pcache = NULL;
    }

    dprintverbose("end zvcache_destroy");

    return;
}

int zvcache_initialize(zvcache_context * pcache, unsigned int issession, unsigned short islocal, unsigned short cachekey, unsigned int zvcount, unsigned int cachesize, char * shmfilepath)
{
    int              result    = NONFATAL;
    size_t           segsize   = 0;
    zvcache_header * header    = NULL;
    unsigned int     msize     = 0;

    unsigned int    cticks     = 0;
    unsigned short  mapclass   = FILEMAP_MAP_SRANDOM;
    unsigned short  locktype   = LOCK_TYPE_SHARED;
    unsigned char   isfirst    = 0;
    unsigned char   islocked   = 0;
    unsigned int    initmemory = 0;
    DWORD           ret        = 0;
    char *          lockname   = ((issession) ? "SESSZVALS_CACHE" : "USERZVALS_CACHE");
    char *          prefix     = NULL;
    size_t          cchprefix  = 0;

    dprintverbose("start zvcache_initialize %p", pcache);

    _ASSERT(pcache    != NULL);
    _ASSERT(zvcount   >= 128 && zvcount   <= 1024);
    _ASSERT(cachesize >= 2   && cachesize <= 64);

    pcache->issession = issession;

    /* Initialize memory map to store file list */
    result = filemap_create(&pcache->zvfilemap);
    if(FAILED(result))
    {
        goto Finished;
    }

    pcache->cachekey = cachekey;

    if(islocal)
    {
        mapclass = FILEMAP_MAP_LRANDOM;
        locktype = LOCK_TYPE_LOCAL;

        pcache->islocal = islocal;
    }

    /* get object name prefix */
    result = lock_get_nameprefix(lockname, cachekey, locktype, &prefix, &cchprefix);
    if (FAILED(result))
    {
        goto Finished;
    }

    result = utils_create_init_event(prefix, "ZVCACHE_INIT", &pcache->hinitdone, &isfirst);
    if (FAILED(result))
    {
        result = FATAL_ZVCACHE_INIT_EVENT;
        goto Finished;
    }

    islocked = 1;

    /* If a shmfilepath is passed, use that to create filemap */
    result = filemap_initialize(pcache->zvfilemap, ((issession) ? FILEMAP_TYPE_SESSZVALS : FILEMAP_TYPE_USERZVALS), cachekey, mapclass, cachesize, isfirst, shmfilepath);
    if(FAILED(result))
    {
        goto Finished;
    }

    pcache->zvmemaddr = (char *)pcache->zvfilemap->mapaddr;
    segsize = filemap_getsize(pcache->zvfilemap);
    initmemory = (pcache->zvfilemap->existing == 0);

    /* Create allocator for file list segment */
    result = alloc_create(&pcache->zvalloc);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = alloc_initialize(pcache->zvalloc, islocal, ((issession) ? "SESSZVALS_SEGMENT" : "USERZVALS_SEGMENT"), cachekey, pcache->zvfilemap->mapaddr, segsize, initmemory);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* Get memory for cache header */
    msize = sizeof(zvcache_header) + ((zvcount - 1) * sizeof(size_t));
    pcache->zvheader = (zvcache_header *)alloc_get_cacheheader(pcache->zvalloc, msize, ((issession) ? CACHE_TYPE_SESSZVALS : CACHE_TYPE_USERZVALS));
    if(pcache->zvheader == NULL)
    {
        result = FATAL_ZVCACHE_INITIALIZE;
        goto Finished;
    }

    header = pcache->zvheader;

    /* Create reader writer locks for the zvcache */
    result = lock_create(&pcache->zvlock);
    if(FAILED(result))
    {
        goto Finished;
    }

    result = lock_initialize(pcache->zvlock, lockname, cachekey, locktype, &header->last_owner);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* Initialize the zvcache_header if this is the first process */
    if(islocal || isfirst)
    {
        cticks = GetTickCount();

        if(initmemory)
        {
            /* No need to get a write lock as other processes */
            /* are blocked waiting for hinitdone event */

            /* We can set rdcount to 0 safely as other processes are */
            /* blocked and this process is right now not using lock */
            header->hitcount     = 0;
            header->misscount    = 0;
            header->last_owner   = 0;

            header->lscavenge    = cticks;
            header->scstart      = 0;
            header->itemcount    = 0;
            header->valuecount   = zvcount;

            memset((void *)header->values, 0, sizeof(size_t) * zvcount);
        }

        header->init_ticks = cticks;
        header->mapcount   = 1;

        ReleaseMutex(pcache->hinitdone);
        islocked = 0;
    }
    else
    {
        /* Increment the mapcount */
        InterlockedIncrement(&header->mapcount);
    }

    /* Create incopy and outcopy zvcopy contexts */
    pcache->incopy = (zvcopy_context *)alloc_pemalloc(sizeof(zvcopy_context));
    if(pcache->incopy == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    pcache->incopy->oomcode   = FATAL_OUT_OF_SMEMORY;
    pcache->incopy->palloc    = pcache->zvalloc;
    pcache->incopy->pbaseadr  = pcache->zvmemaddr;
    pcache->incopy->hoffset   = 0;
    pcache->incopy->allocsize = 0;
    pcache->incopy->fnmalloc  = alloc_osmalloc;
    pcache->incopy->fnrealloc = alloc_osrealloc;
    pcache->incopy->fnstrdup  = alloc_osstrdup;
    pcache->incopy->fnfree    = alloc_osfree;

    pcache->outcopy = (zvcopy_context *)alloc_pemalloc(sizeof(zvcopy_context));
    if(pcache->outcopy == NULL)
    {
        result = FATAL_OUT_OF_LMEMORY;
        goto Finished;
    }

    pcache->outcopy->oomcode   = FATAL_OUT_OF_LMEMORY;
    pcache->outcopy->palloc    = NULL;
    pcache->outcopy->pbaseadr  = pcache->zvmemaddr;
    pcache->outcopy->hoffset   = 0;
    pcache->outcopy->allocsize = 0;
    pcache->outcopy->fnmalloc  = alloc_oemalloc;
    pcache->outcopy->fnrealloc = alloc_oerealloc;
    pcache->outcopy->fnstrdup  = alloc_oestrdup;
    pcache->outcopy->fnfree    = alloc_oefree;

Finished:

    if (islocked)
    {
        ReleaseMutex(pcache->hinitdone);
        islocked = 0;
    }

    if (prefix != NULL)
    {
        alloc_pefree(prefix);
        prefix = NULL;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in zvcache_initialize", result);

        if(pcache->zvalloc != NULL)
        {
            alloc_terminate(pcache->zvalloc);
            alloc_destroy(pcache->zvalloc);

            pcache->zvalloc = NULL;
        }

        if(pcache->zvfilemap != NULL)
        {
            filemap_terminate(pcache->zvfilemap);
            filemap_destroy(pcache->zvfilemap);

            pcache->zvfilemap = NULL;
        }

        if(pcache->zvlock != NULL)
        {
            lock_terminate(pcache->zvlock);
            lock_destroy(pcache->zvlock);

            pcache->zvlock = NULL;
        }

        if(pcache->hinitdone != NULL)
        {
            CloseHandle(pcache->hinitdone);
            pcache->hinitdone = NULL;
        }

        pcache->zvheader = NULL;
    }

    dprintverbose("end zvcache_initialize");

    return result;
}

void zvcache_terminate(zvcache_context * pcache)
{
    dprintverbose("start zvcache_terminate %p", pcache);

    if(pcache != NULL)
    {
        if(pcache->zvheader != NULL)
        {
            InterlockedDecrement(&pcache->zvheader->mapcount);
            pcache->zvheader = NULL;
        }

        if(pcache->zvalloc != NULL)
        {
            alloc_terminate(pcache->zvalloc);
            alloc_destroy(pcache->zvalloc);

            pcache->zvalloc = NULL;
        }

        if(pcache->zvfilemap != NULL)
        {
            filemap_terminate(pcache->zvfilemap);
            filemap_destroy(pcache->zvfilemap);

            pcache->zvfilemap = NULL;
        }

        if(pcache->zvlock != NULL)
        {
            lock_terminate(pcache->zvlock);
            lock_destroy(pcache->zvlock);

            pcache->zvlock = NULL;
        }

        if(pcache->hinitdone != NULL)
        {
            CloseHandle(pcache->hinitdone);
            pcache->hinitdone = NULL;
        }
    }

    dprintverbose("end zvcache_terminate");

    return;
}

int zvcache_get(zvcache_context * pcache, const char * key, zval ** pvalue)
{
    int              result  = NONFATAL;
    unsigned int     index   = 0;
    unsigned char    flock   = 0;
    zvcache_value *  pentry  = NULL;
    zvcache_header * header  = NULL;
    zval *           pcached = NULL;

    dprintverbose("start zvcache_get");

    _ASSERT(pcache != NULL);
    _ASSERT(key    != NULL);
    _ASSERT(pvalue != NULL);

    header = pcache->zvheader;
    index = utils_getindex(key, pcache->zvheader->valuecount);

    lock_lock(pcache->zvlock);
    flock = 1;

    result = find_zvcache_entry(pcache, key, index, &pentry);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* Adjust overall cache hitcount/misscount and entry hitcount */
    if(pentry == NULL)
    {
        InterlockedIncrement(&header->misscount);

        result = WARNING_ZVCACHE_EMISSING;
        goto Finished;
    }
    else
    {
        InterlockedIncrement(&pentry->hitcount);
        InterlockedIncrement(&header->hitcount);
    }

    /* Entry found. copyout to local memory */
    _ASSERT(pentry->zvalue  != 0);
    _ASSERT(pcache->outcopy != NULL);

    InterlockedExchange(&pentry->use_ticks, GetTickCount());
    pcached = (zval *)ZVALUE(pcache->incopy, pentry->zvalue);

    result = copyout_zval(pcache, pcache->outcopy, NULL, pcached, pvalue);
    if(FAILED(result))
    {
        goto Finished;
    }

    _ASSERT(SUCCEEDED(result));

Finished:

    if(flock)
    {
        lock_unlock(pcache->zvlock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in zvcache_get", result);
    }

    dprintverbose("end zvcache_get");

    return result;
}

int zvcache_set(zvcache_context * pcache, const char * key, zval * pzval, unsigned int ttl, unsigned char isadd)
{
    int             result  = NONFATAL;
    unsigned int    index   = 0;
    unsigned char   flock   = 0;
    zvcache_value * pentry  = NULL;
    zvcache_value * pnewval = NULL;
    zval *          pcopied = NULL;
    char *          pchar   = NULL;
    zend_string *   zstr    = NULL;

    dprintverbose("start zvcache_set");

    _ASSERT(pcache != NULL);
    _ASSERT(key    != NULL);
    _ASSERT(pzval  != NULL);

    index = utils_getindex(key, pcache->zvheader->valuecount);

    lock_lock(pcache->zvlock);
    flock = 1;

    result = find_zvcache_entry(pcache, key, index, &pentry);
    if(FAILED(result))
    {
        goto Finished;
    }

    if(pentry != NULL)
    {
        /* If key already exists, throw error for add */
        if(isadd == 1)
        {
            _ASSERT(pentry->zvalue != 0);

            result = WARNING_ZVCACHE_EXISTS;
            goto Finished;
        }

        InterlockedExchange(&pentry->use_ticks, GetTickCount());
    }

    /* Only update the session entry if the value changed */
    if(pcache->issession)
    {
        _ASSERT((Z_TYPE_P(pzval) == IS_STRING));

        if(pentry != NULL)
        {
            pcopied = (zval *)ZVALUE(pcache->incopy, pentry->zvalue);
            _ASSERT(Z_TYPE_P(pcopied) == IS_STRING);

            zstr = (zend_string *)ZVALUE(pcache->incopy, (size_t)Z_STR_P(pcopied));

            if(Z_STRLEN_P(pzval) == ZSTR_LEN(zstr) && zend_string_equals(Z_STR_P(pzval), zstr))
            {
                /* Shortcircuit set as value stored in existing entry is the same */
                goto Finished;
            }
        }
    }

    /* If entry wasn't found or set was called, create new data */
    result = create_zvcache_data(pcache, key, pzval, ttl, &pnewval);
    if(FAILED(result))
    {
        goto Finished;
    }

    run_zvcache_scavenger(pcache);

    /* Check again after scavenging */
    result = find_zvcache_entry(pcache, key, index, &pentry);
    if(FAILED(result))
    {
        goto Finished;
    }

    /* some other process already added this entry, destroy cache data */
    if(pentry != NULL)
    {
        if(isadd == 1)
        {
            result = WARNING_ZVCACHE_EXISTS;
            goto Finished;
        }
        else
        {
            /* Delete the existing entry */
            remove_zvcache_entry(pcache, index, pentry);
            pentry = NULL;
        }
    }

    pentry = pnewval;
    pnewval = NULL;

    add_zvcache_entry(pcache, index, pentry);
    _ASSERT(pentry != NULL);

Finished:

    if(flock)
    {
        lock_unlock(pcache->zvlock);
        flock = 0;
    }

    if(pnewval != NULL)
    {
        destroy_zvcache_data(pcache, pnewval);
        pnewval = NULL;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in zvcache_set", result);
    }

    dprintverbose("end zvcache_set");

    return result;
}

int zvcache_delete(zvcache_context * pcache, const char * key)
{
    int             result = NONFATAL;
    unsigned int    index     = 0;
    unsigned char   flock     = 0;
    zvcache_value * pentry    = NULL;

    dprintverbose("start zvcache_delete");

    _ASSERT(pcache != NULL);
    _ASSERT(key    != NULL);

    index = utils_getindex(key, pcache->zvheader->valuecount);

    lock_lock(pcache->zvlock);
    flock = 1;

    run_zvcache_scavenger(pcache);

    result = find_zvcache_entry(pcache, key, index, &pentry);
    if(FAILED(result))
    {
        goto Finished;
    }

    if(pentry == NULL)
    {
        result = WARNING_ZVCACHE_EMISSING;
        goto Finished;
    }

    remove_zvcache_entry(pcache, index, pentry);

    _ASSERT(SUCCEEDED(result));

Finished:

    if(flock)
    {
        lock_unlock(pcache->zvlock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in zvcache_delete", result);
    }

    dprintverbose("end zvcache_delete");

    return result;
}

int zvcache_clear(zvcache_context * pcache)
{
    int              result = NONFATAL;
    unsigned int     index  = 0;
    zvcache_header * header = NULL;
    alloc_context *  palloc = NULL;
    zvcache_value *  pvalue = NULL;
    zvcache_value *  ptemp  = NULL;

    dprintverbose("start zvcache_clear %p", pcache);

    _ASSERT(pcache != NULL);

    lock_lock(pcache->zvlock);

    /* No point running the scavenger on call to clear */

    header = pcache->zvheader;
    palloc = pcache->zvalloc;

    for(index = 0; index < header->valuecount; index++)
    {
        if(header->values[index] == 0)
        {
            continue;
        }

        pvalue = ZVCACHE_VALUE(palloc, header->values[index]);

        while(pvalue != NULL)
        {
            ptemp = ZVCACHE_VALUE(palloc, pvalue->next_value);
            remove_zvcache_entry(pcache, index, pvalue);
            pvalue = ptemp;
        }

        ptemp = NULL;
    }

    _ASSERT(SUCCEEDED(result));

    lock_unlock(pcache->zvlock);

    dprintverbose("end zvcache_clear");

    return result;
}

int zvcache_exists(zvcache_context * pcache, const char * key, unsigned char * pexists)
{
    int             result = NONFATAL;
    unsigned int    index  = 0;
    unsigned char   flock  = 0;
    zvcache_value * pentry = NULL;

    dprintverbose("start zvcache_exists");

    _ASSERT(pcache  != NULL);
    _ASSERT(key     != NULL);
    _ASSERT(pexists != NULL);

    *pexists = 0;
    index = utils_getindex(key, pcache->zvheader->valuecount);

    lock_lock(pcache->zvlock);
    flock = 1;

    result = find_zvcache_entry(pcache, key, index, &pentry);
    if(FAILED(result))
    {
        goto Finished;
    }

    if(pentry == NULL)
    {
        *pexists = 0;
    }
    else
    {
        *pexists = 1;
    }

    _ASSERT(SUCCEEDED(result));

Finished:

    if(flock)
    {
        lock_unlock(pcache->zvlock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in zvcache_exists", result);
    }

    dprintverbose("end zvcache_exists");

    return result;
}

static void zvcache_info_entry_dtor(void * pvoid)
{
    zvcache_info_entry * pinfo = NULL;

    _ASSERT(pvoid != NULL);
    pinfo = (zvcache_info_entry *)pvoid;

    if(pinfo->key != NULL)
    {
        alloc_efree(pinfo->key);
        pinfo->key = NULL;
    }

    return;
}

int zvcache_list(zvcache_context * pcache, zend_bool summaryonly, char * pkey, zvcache_info * pcinfo, zend_llist * plist)
{
    int                  result   = NONFATAL;
    unsigned char        flock    = 0;
    zvcache_header *     header   = NULL;
    alloc_context *      palloc   = NULL;
    zvcache_value *      pvalue   = NULL;
    unsigned int         index    = 0;
    zvcache_info_entry   pinfo    = {0};
    unsigned int         cticks   = 0;
    unsigned int         count    = 0;

    dprintverbose("start zvcache_list");

    _ASSERT(pcache != NULL);
    _ASSERT(pcinfo != NULL);
    _ASSERT(plist  != NULL);

    header = pcache->zvheader;
    palloc = pcache->zvalloc;
    cticks  = GetTickCount();

    lock_lock(pcache->zvlock);
    flock = 1;

    pcinfo->hitcount  = header->hitcount;
    pcinfo->misscount = header->misscount;
    pcinfo->itemcount = header->itemcount;
    pcinfo->islocal   = pcache->islocal;

    zend_llist_init(plist, sizeof(zvcache_info_entry), zvcache_info_entry_dtor, 0);

    pcinfo->initage = utils_ticksdiff(cticks, header->init_ticks) / 1000;

    if(pkey != NULL)
    {
        index = utils_getindex(pkey, header->valuecount);

        result = find_zvcache_entry(pcache, pkey, index, &pvalue);
        if(FAILED(result))
        {
            goto Finished;
        }

        if(pvalue != NULL)
        {
            pinfo.key = alloc_estrdup(ZVALUE(pcache->incopy, pvalue->keystr));
            if(pinfo.key == NULL)
            {
                result = FATAL_OUT_OF_LMEMORY;
                goto Finished;
            }

            pinfo.ttl      = pvalue->ttlive;
            pinfo.age      = utils_ticksdiff(cticks, pvalue->add_ticks) / 1000;
            pinfo.type     = Z_TYPE_P((zval *)ZVALUE(pcache->incopy, pvalue->zvalue));
            pinfo.sizeb    = pvalue->sizeb;
            pinfo.hitcount = pvalue->hitcount;

            zend_llist_add_element(plist, &pinfo);
        }
    }
    else
    {
        /* Leave count to 0 if only summary is needed */
        if(!summaryonly)
        {
            count = header->valuecount;
        }

        for(index = 0; index < count; index++)
        {
            if(header->values[index] == 0)
            {
                continue;
            }

            pvalue = ZVCACHE_VALUE(palloc, header->values[index]);
            while(pvalue != NULL)
            {
                pinfo.key = alloc_estrdup(ZVALUE(pcache->incopy, pvalue->keystr));
                if(pinfo.key == NULL)
                {
                    result = FATAL_OUT_OF_LMEMORY;
                    goto Finished;
                }

                pinfo.ttl      = pvalue->ttlive;
                pinfo.age      = utils_ticksdiff(cticks, pvalue->add_ticks) / 1000;
                pinfo.type     = Z_TYPE_P((zval *)ZVALUE(pcache->incopy, pvalue->zvalue));
                pinfo.sizeb    = pvalue->sizeb;
                pinfo.hitcount = pvalue->hitcount;

                zend_llist_add_element(plist, &pinfo);
                pvalue = ZVCACHE_VALUE(palloc, pvalue->next_value);
            }
        }
    }

Finished:

    if(flock)
    {
        lock_unlock(pcache->zvlock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in zvcache_list", result);

        zend_llist_destroy(plist);
    }

    dprintverbose("end zvcache_list");

    return result;
}

int zvcache_change(zvcache_context * pcache, const char * key, zend_long delta, zend_long * newvalue)
{
    int             result = NONFATAL;
    unsigned char   flock  = 0;
    unsigned int    index  = 0;
    zvcache_value * pvalue = NULL;
    zval *          pzval  = NULL;

    dprintverbose("start zvcache_change");

    _ASSERT(pcache   != NULL);
    _ASSERT(key      != NULL);
    _ASSERT(newvalue != NULL);

    *newvalue = 0;
    index = utils_getindex(key, pcache->zvheader->valuecount);

    lock_lock(pcache->zvlock);
    flock = 1;

    run_zvcache_scavenger(pcache);

    result = find_zvcache_entry(pcache, key, index, &pvalue);
    if(FAILED(result))
    {
        goto Finished;
    }

    if(pvalue == NULL)
    {
        result = WARNING_ZVCACHE_EMISSING;
        goto Finished;
    }

    _ASSERT(pvalue->zvalue != 0);
    pzval = (zval *)ZVALUE(pcache->incopy, pvalue->zvalue);

    if(Z_TYPE_P(pzval) != IS_LONG)
    {
        result = WARNING_ZVCACHE_NOTLONG;
        goto Finished;
    }

    Z_LVAL_P(pzval) += delta;
    *newvalue = Z_LVAL_P(pzval);

Finished:

    if(flock)
    {
        lock_unlock(pcache->zvlock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in zvcache_change", result);
    }

    dprintverbose("end zvcache_change");

    return result;
}

int zvcache_compswitch(zvcache_context * pcache, const char * key, zend_long oldvalue, zend_long newvalue)
{
    int             result = NONFATAL;
    unsigned char   flock  = 0;
    unsigned int    index  = 0;
    zvcache_value * pentry = NULL;
    zval *          pzval  = NULL;

    dprintverbose("start zvcache_compswitch");

    _ASSERT(pcache != NULL);
    _ASSERT(key    != NULL);

    index = utils_getindex(key, pcache->zvheader->valuecount);

    lock_lock(pcache->zvlock);
    flock = 1;

    run_zvcache_scavenger(pcache);

    result = find_zvcache_entry(pcache, key, index, &pentry);
    if(FAILED(result))
    {
        goto Finished;
    }

    if(pentry == NULL)
    {
        result = WARNING_ZVCACHE_EMISSING;
        goto Finished;
    }

    _ASSERT(pentry->zvalue != 0);
    pzval = (zval *)ZVALUE(pcache->incopy, pentry->zvalue);

    if(Z_TYPE_P(pzval) != IS_LONG)
    {
        result = WARNING_ZVCACHE_NOTLONG;
        goto Finished;
    }

    if(Z_LVAL_P(pzval) == oldvalue)
    {
        Z_LVAL_P(pzval) = newvalue;
    }
    else
    {
        result = WARNING_ZVCACHE_CASNEQ;
        goto Finished;
    }

    _ASSERT(SUCCEEDED(result));

Finished:

    if(flock)
    {
        lock_unlock(pcache->zvlock);
        flock = 0;
    }

    if(FAILED(result))
    {
        dprintimportant("failure %d in zvcache_compswitch", result);
    }

    dprintverbose("end zvcache_compswitch");

    return result;
}
