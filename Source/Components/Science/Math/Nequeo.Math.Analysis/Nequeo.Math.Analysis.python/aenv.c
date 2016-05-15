/**************************************************************************
ALGLIB 3.10.0 (source code generated 2015-08-19)
Copyright (c) Sergey Bochkanov (ALGLIB project).

>>> SOURCE LICENSE >>>
This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation (www.fsf.org); either version 2 of the 
License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU General Public License is available at
http://www.fsf.org/licensing/licenses
>>> END OF LICENSE >>>
**************************************************************************/
#include "stdafx.h"
#include "aenv.h"

#if defined(AE_CPU)
#if AE_CPU==AE_INTEL
#if AE_COMPILER==AE_MSVC
#include <intrin.h>
#endif
#endif
#endif

/*$ Declarations $*/
/*
 * OS-specific includes
 */
#ifdef AE_USE_CPP
}
#endif
#if AE_OS==AE_WINDOWS
#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0501
#endif
#include <windows.h>
#include <process.h>
#elif AE_OS==AE_POSIX
#include <time.h>
#include <unistd.h>
#include <pthread.h>
#include <sched.h>
#endif
/* Debugging helpers for Windows */
#ifdef AE_DEBUG4WINDOWS
#include <windows.h>
#include <stdio.h>
#endif
#ifdef AE_USE_CPP
namespace alglib_impl
{
#endif

/*
 * local definitions
 */
#define x_nb 16
#define AE_DATA_ALIGN 64
#define AE_PTR_ALIGN sizeof(void*)
#define DYN_BOTTOM ((void*)1)
#define DYN_FRAME  ((void*)2)
#define AE_LITTLE_ENDIAN 1
#define AE_BIG_ENDIAN 2
#define AE_MIXED_ENDIAN 3
#define AE_SER_ENTRY_LENGTH 11
#define AE_SER_ENTRIES_PER_ROW 5

#define AE_SM_DEFAULT 0
#define AE_SM_ALLOC 1
#define AE_SM_READY2S 2
#define AE_SM_TO_STRING 10
#define AE_SM_FROM_STRING 20
#define AE_SM_TO_CPPSTRING 11

#define AE_LOCK_CYCLES 512
#define AE_LOCK_TESTS_BEFORE_YIELD 16
#define AE_CRITICAL_ASSERT(x) if( !(x) ) abort()

/*************************************************************************
Lock.

This is internal structure which implements lock functionality.
*************************************************************************/
typedef struct
{
#if AE_OS==AE_WINDOWS
    volatile ae_int_t * volatile p_lock;
    char buf[sizeof(ae_int_t)+AE_LOCK_ALIGNMENT];
#elif AE_OS==AE_POSIX
    pthread_mutex_t mutex;
#else
    ae_bool is_locked;
#endif
} _lock;

/*$ Body $*/
/*
 * alloc counter
 */
ae_int64_t _alloc_counter = 0;
ae_bool    _use_alloc_counter = ae_false;
#ifdef AE_SMP_DEBUGCOUNTERS
__declspec(align(AE_LOCK_ALIGNMENT)) volatile ae_int64_t _ae_dbg_lock_acquisitions = 0;
__declspec(align(AE_LOCK_ALIGNMENT)) volatile ae_int64_t _ae_dbg_lock_spinwaits = 0;
__declspec(align(AE_LOCK_ALIGNMENT)) volatile ae_int64_t _ae_dbg_lock_yields = 0;
#endif

/*
 * These declarations are used to ensure that
 * sizeof(ae_bool)=1, sizeof(ae_int32_t)==4, sizeof(ae_int64_t)==8, sizeof(ae_int_t)==sizeof(void*).
 * they will lead to syntax error otherwise (array size will be negative).
 *
 * you can remove them, if you want - they are not used anywhere.
 *
 */
static char     _ae_bool_must_be_8_bits_wide[1-2*((int)(sizeof(ae_bool))-1)*((int)(sizeof(ae_bool))-1)];
static char _ae_int32_t_must_be_32_bits_wide[1-2*((int)(sizeof(ae_int32_t))-4)*((int)(sizeof(ae_int32_t))-4)];
static char _ae_int64_t_must_be_64_bits_wide[1-2*((int)(sizeof(ae_int64_t))-8)*((int)(sizeof(ae_int64_t))-8)];
static char _ae_int_t_must_be_pointer_sized [1-2*((int)(sizeof(ae_int_t))-(int)sizeof(void*))*((int)(sizeof(ae_int_t))-(int)(sizeof(void*)))];  

/*
 * This variable is used to prevent some tricky optimizations which may degrade multithreaded performance.
 * It is touched once in the ae_init_pool() function from smp.c in order to prevent optimizations.
 *
 */
static volatile ae_int_t ae_never_change_it = 1;

ae_int_t ae_misalignment(const void *ptr, size_t alignment)
{
    union _u
    {
        const void *ptr;
        ae_int_t iptr;
    } u;
    u.ptr = ptr;
    return (ae_int_t)(u.iptr%alignment);
}

void* ae_align(void *ptr, size_t alignment)
{
    char *result = (char*)ptr;
    if( (result-(char*)0)%alignment!=0 )
        result += alignment - (result-(char*)0)%alignment;
    return result;
}

/*************************************************************************
This function abnormally aborts program, using one of several ways:

* for AE_USE_CPP_ERROR_HANDLING being NOT defined:
  * for state!=NULL and state->break_jump being initialized with  call  to
    ae_state_set_break_jump() - it performs longjmp() to return site.
  * otherwise, abort() is called
* for AE_USE_CPP_ERROR_HANDLING being DEFINED - an instance of ae_error_type()
  class is throw'ed.
  
In   all  cases,  for  state!=NULL  function  sets  state->last_error  and
state->error_msg fields. It also clears state with ae_state_clear().
  
If state is not NULL and state->thread_exception_handler  is  set,  it  is
called prior to handling error and clearing state.
*************************************************************************/
void ae_break(ae_state *state, ae_error_type error_type, const char *msg)
{
#ifndef AE_USE_CPP_ERROR_HANDLING
    if( state!=NULL )
    {
        if( state->thread_exception_handler!=NULL )
            state->thread_exception_handler(state);
        ae_state_clear(state);
        state->last_error = error_type;
        state->error_msg = msg;
        if( state->break_jump!=NULL )
            longjmp(*(state->break_jump), 1);
        else
            abort();
    }
    else
        abort();
#else
    if( state!=NULL )
    {
        if( state->thread_exception_handler!=NULL )
            state->thread_exception_handler(state);
        ae_state_clear(state);
        state->last_error = error_type;
        state->error_msg = msg;
    }
    throw error_type;
#endif
}

void* aligned_malloc(size_t size, size_t alignment)
{
    if( size==0 )
        return NULL;
    if( alignment<=1 )
    {
        /* no alignment, just call malloc */
        void *block;
        void **p; ;
        block = malloc(sizeof(void*)+size);
        if( block==NULL )
            return NULL;
        p = (void**)block;
        *p = block;
        if( _use_alloc_counter )
        {
#if AE_OS==AE_WINDOWS
            InterlockedIncrement((LONG volatile *)&_alloc_counter);
#else
#endif
        }
        return (void*)((char*)block+sizeof(void*));
    }
    else
    {
        /* align */
        void *block;
        char *result;
        block = malloc(alignment-1+sizeof(void*)+size);
        if( block==NULL )
            return NULL;
        result = (char*)block+sizeof(void*);
        /*if( (result-(char*)0)%alignment!=0 )
            result += alignment - (result-(char*)0)%alignment;*/
        result = (char*)ae_align(result, alignment);
        *((void**)(result-sizeof(void*))) = block;
        if( _use_alloc_counter )
        {
#if AE_OS==AE_WINDOWS
            InterlockedIncrement((LONG volatile *)&_alloc_counter);
#else
#endif
        }
        return result;
    }
}

void aligned_free(void *block)
{
    void *p;
    if( block==NULL )
        return;
    p = *((void**)((char*)block-sizeof(void*)));
    free(p);
    if( _use_alloc_counter )
    {
#if AE_OS==AE_WINDOWS
        InterlockedDecrement((LONG volatile *)&_alloc_counter);
#else
#endif
    }
}

/************************************************************************
Malloc's memory with automatic alignment.

Returns NULL when zero size is specified.

Error handling:
* if state is NULL, returns NULL on allocation error
* if state is not NULL, calls ae_break() on allocation error
************************************************************************/
void* ae_malloc(size_t size, ae_state *state)
{
    void *result;
    if( size==0 )
        return NULL;
    result = aligned_malloc(size,AE_DATA_ALIGN);
    if( result==NULL && state!=NULL)
        ae_break(state, ERR_OUT_OF_MEMORY, "ae_malloc(): out of memory");
    return result;
}

void ae_free(void *p)
{
    if( p!=NULL )
        aligned_free(p);
}

/************************************************************************
Sets pointers to the matrix rows.

* dst must be correctly initialized matrix
* dst->data.ptr points to the beginning of memory block  allocated  for  
  row pointers.
* dst->ptr - undefined (initialized during algorithm processing)
* storage parameter points to the beginning of actual storage
************************************************************************/
void ae_matrix_update_row_pointers(ae_matrix *dst, void *storage)
{
    char *p_base;
    void **pp_ptr;
    ae_int_t i;
    if( dst->rows>0 && dst->cols>0 )
    {
        p_base = (char*)storage;
        pp_ptr = (void**)dst->data.ptr;
        dst->ptr.pp_void = pp_ptr;
        for(i=0; i<dst->rows; i++, p_base+=dst->stride*ae_sizeof(dst->datatype))
            pp_ptr[i] = p_base;
    }
    else
        dst->ptr.pp_void = NULL;
}

/************************************************************************
Returns size of datatype.
Zero for dynamic types like strings or multiple precision types.
************************************************************************/
ae_int_t ae_sizeof(ae_datatype datatype)
{
    switch(datatype)
    {
        case DT_BOOL:       return (ae_int_t)sizeof(ae_bool);
        case DT_INT:        return (ae_int_t)sizeof(ae_int_t);
        case DT_REAL:       return (ae_int_t)sizeof(double);
        case DT_COMPLEX:    return 2*(ae_int_t)sizeof(double);
        default:            return 0;
    }
}


/************************************************************************
This  dummy  function  is  used to prevent compiler messages about unused
locals in automatically generated code.

It makes nothing - just accepts pointer, "touches" it - and that is  all.
It performs several tricky operations without side effects which  confuse
compiler so it does not compain about unused locals in THIS function.
************************************************************************/
void ae_touch_ptr(void *p)
{
    void * volatile fake_variable0 = p;
    void * volatile fake_variable1 = fake_variable0;
    fake_variable0 = fake_variable1;
}

/************************************************************************
This function initializes ALGLIB environment state.

NOTES:
* stacks contain no frames, so ae_make_frame() must be called before
  attaching dynamic blocks. Without it ae_leave_frame() will cycle
  forever (which is intended behavior).
************************************************************************/
void ae_state_init(ae_state *state)
{
    ae_int32_t *vp;

    /*
     * p_next points to itself because:
     * * correct program should be able to detect end of the list
     *   by looking at the ptr field.
     * * NULL p_next may be used to distinguish automatic blocks
     *   (in the list) from non-automatic (not in the list)
     */
    state->last_block.p_next = &(state->last_block);
    state->last_block.deallocator = NULL;
    state->last_block.ptr = DYN_BOTTOM;
    state->p_top_block = &(state->last_block);
#ifndef AE_USE_CPP_ERROR_HANDLING
    state->break_jump = NULL;
#endif
    state->error_msg = "";
    
    /*
     * determine endianness and initialize precomputed IEEE special quantities.
     */
    state->endianness = ae_get_endianness();
    if( state->endianness==AE_LITTLE_ENDIAN )
    {
        vp = (ae_int32_t*)(&state->v_nan);
        vp[0] = 0;
        vp[1] = (ae_int32_t)0x7FF80000;
        vp = (ae_int32_t*)(&state->v_posinf);
        vp[0] = 0;
        vp[1] = (ae_int32_t)0x7FF00000;
        vp = (ae_int32_t*)(&state->v_neginf);
        vp[0] = 0;
        vp[1] = (ae_int32_t)0xFFF00000;
    }
    else if( state->endianness==AE_BIG_ENDIAN )
    {
        vp = (ae_int32_t*)(&state->v_nan);
        vp[1] = 0;
        vp[0] = (ae_int32_t)0x7FF80000;
        vp = (ae_int32_t*)(&state->v_posinf);
        vp[1] = 0;
        vp[0] = (ae_int32_t)0x7FF00000;
        vp = (ae_int32_t*)(&state->v_neginf);
        vp[1] = 0;
        vp[0] = (ae_int32_t)0xFFF00000;
    }
    else
        abort();
    
    /*
     * set threading information
     */
    state->worker_thread = NULL;
    state->parent_task = NULL;
    state->thread_exception_handler = NULL;
}


/************************************************************************
This function clears ALGLIB environment state.
All dynamic data controlled by state are freed.
************************************************************************/
void ae_state_clear(ae_state *state)
{
    while( state->p_top_block->ptr!=DYN_BOTTOM )
        ae_frame_leave(state);
}


#ifndef AE_USE_CPP_ERROR_HANDLING
/************************************************************************
This function sets jump buffer for error handling.

buf may be NULL.
************************************************************************/
void ae_state_set_break_jump(ae_state *state, jmp_buf *buf)
{
    state->break_jump = buf;
}
#endif


/************************************************************************
This function makes new stack frame.

This function takes two parameters: environment state and pointer to  the
dynamic block which will be used as indicator  of  the  frame  beginning.
This dynamic block must be initialized by caller and mustn't  be changed/
deallocated/reused till ae_leave_frame called. It may be global or  local
variable (local is even better).
************************************************************************/
void ae_frame_make(ae_state *state, ae_frame *tmp)
{
    tmp->db_marker.p_next = state->p_top_block;
    tmp->db_marker.deallocator = NULL;
    tmp->db_marker.ptr = DYN_FRAME;
    state->p_top_block = &tmp->db_marker;
}


/************************************************************************
This function leaves current stack frame and deallocates all automatic
dynamic blocks which were attached to this frame.
************************************************************************/
void ae_frame_leave(ae_state *state)
{
    while( state->p_top_block->ptr!=DYN_FRAME && state->p_top_block->ptr!=DYN_BOTTOM)
    {
        if( state->p_top_block->ptr!=NULL && state->p_top_block->deallocator!=NULL)
            ((ae_deallocator)(state->p_top_block->deallocator))(state->p_top_block->ptr);
        state->p_top_block = state->p_top_block->p_next;
    }
    state->p_top_block = state->p_top_block->p_next;
}


/************************************************************************
This function attaches block to the dynamic block list

block               block
state               ALGLIB environment state

NOTES:
* never call it for special blocks which marks frame boundaries!
************************************************************************/
void ae_db_attach(ae_dyn_block *block, ae_state *state)
{
    block->p_next = state->p_top_block;
    state->p_top_block = block;
}


/************************************************************************
This function malloc's dynamic block:

block               destination block, assumed to be uninitialized
size                size (in bytes)
state               ALGLIB environment state. May be NULL.
make_automatic      if true, vector is added to the dynamic block list

block is assumed to be uninitialized, its fields are ignored.

Error handling:
* if state is NULL, returns ae_false on allocation error
* if state is not NULL, calls ae_break() on allocation error
* returns ae_true on success

NOTES:
* never call it for blocks which are already in the list
************************************************************************/
ae_bool ae_db_malloc(ae_dyn_block *block, ae_int_t size, ae_state *state, ae_bool make_automatic)
{
    /* ensure that size is >=0
       two ways to exit: 1) through ae_assert, if we have non-NULL state, 2) by returning ae_false */
    if( state!=NULL )
        ae_assert(size>=0, "ae_db_malloc(): negative size", state);
    if( size<0 )
        return ae_false;
    
    /* allocation */
    block->ptr = ae_malloc((size_t)size, state);
    if( block->ptr==NULL && size!=0 )
    {
        /* for state!=NULL exception is thrown from ae_malloc(), so
           we have to handle only situation when state is NULL */
        return ae_false;
    }
    if( make_automatic && state!=NULL )
        ae_db_attach(block, state);
    else
        block->p_next = NULL;
    block->deallocator = ae_free;
    return ae_true;
}


/************************************************************************
This function realloc's dynamic block:

block               destination block (initialized)
size                new size (in bytes)
state               ALGLIB environment state

block is assumed to be initialized.

This function:
* deletes old contents
* preserves automatic state

Error handling:
* if state is NULL, returns ae_false on allocation error
* if state is not NULL, calls ae_break() on allocation error
* returns ae_true on success

NOTES:
* never call it for special blocks which mark frame boundaries!
************************************************************************/
ae_bool ae_db_realloc(ae_dyn_block *block, ae_int_t size, ae_state *state)
{
    /* ensure that size is >=0
       two ways to exit: 1) through ae_assert, if we have non-NULL state, 2) by returning ae_false */
    if( state!=NULL )
        ae_assert(size>=0, "ae_db_realloc(): negative size", state);
    if( size<0 )
        return ae_false;
    
    /* realloc */
    if( block->ptr!=NULL )
        ((ae_deallocator)block->deallocator)(block->ptr);
    block->ptr = ae_malloc((size_t)size, state);
    if( block->ptr==NULL && size!=0 )
    {
        /* for state!=NULL exception is thrown from ae_malloc(), so
           we have to handle only situation when state is NULL */
        return ae_false;
    }
    block->deallocator = ae_free;
    return ae_true;
}


/************************************************************************
This function clears dynamic block (releases  all  dynamically  allocated
memory). Dynamic block may be in automatic management list - in this case
it will NOT be removed from list.

block               destination block (initialized)

NOTES:
* never call it for special blocks which marks frame boundaries!
************************************************************************/
void ae_db_free(ae_dyn_block *block)
{
    if( block->ptr!=NULL )
        ((ae_deallocator)block->deallocator)(block->ptr);
    block->ptr = NULL;
    block->deallocator = ae_free;
}

/************************************************************************
This function swaps contents of two dynamic blocks (pointers and 
deallocators) leaving other parameters (automatic management settings, 
etc.) unchanged.

NOTES:
* never call it for special blocks which marks frame boundaries!
************************************************************************/
void ae_db_swap(ae_dyn_block *block1, ae_dyn_block *block2)
{
    void (*deallocator)(void*) = NULL;
    void * volatile ptr;
    ptr = block1->ptr;
    deallocator = block1->deallocator;
    block1->ptr = block2->ptr;
    block1->deallocator = block2->deallocator;
    block2->ptr = ptr;
    block2->deallocator = deallocator;
}

/*************************************************************************
This function creates ae_vector.
Vector size may be zero. Vector contents is uninitialized.

dst                 destination vector, assumed to be  uninitialized,  its
                    fields are ignored.
size                vector size, may be zero
datatype            guess what...
state               this parameter can be:
                    * pointer to current instance of ae_state, if you want
                      to automatically destroy this object  after  leaving
                      current frame
                    * NULL,   if  you  do  NOT  want  this  vector  to  be
                      automatically managed (say, if it is field  of  some
                      object)

Error handling:
* on failure (size<0 or unable to allocate memory) - calls ae_break() with
  NULL state pointer. Usually it results in abort() call.

dst is 
*************************************************************************/
void ae_vector_init(ae_vector *dst, ae_int_t size, ae_datatype datatype, ae_state *state)
{
    /* ensure that size is >=0
       two ways to exit: 1) through ae_assert, if we have non-NULL state, 2) by returning ae_false */
    ae_assert(
        size>=0,
        "ae_vector_init(): negative size",
        NULL);

    /* init */
    dst->cnt = size;
    dst->datatype = datatype;
    ae_assert(
        ae_db_malloc(&dst->data, size*ae_sizeof(datatype), state, state!=NULL), /* TODO: change ae_db_malloc() */
        "ae_vector_init(): failed to allocate memory",
        NULL);
    dst->ptr.p_ptr = dst->data.ptr;
    dst->is_attached = ae_false;
}


/************************************************************************
This function creates copy of ae_vector. New copy of the data is created,
which is managed and owned by newly initialized vector.

dst                 destination vector
src                 well, it is source
state               this parameter can be:
                    * pointer to current instance of ae_state, if you want
                      to automatically destroy this object  after  leaving
                      current frame
                    * NULL,   if  you  do  NOT  want  this  vector  to  be
                      automatically managed (say, if it is field  of  some
                      object)
                      
Error handling:
* on failure calls ae_break() with NULL state pointer. Usually it  results
  in abort() call.

dst is assumed to be uninitialized, its fields are ignored.
************************************************************************/
void ae_vector_init_copy(ae_vector *dst, ae_vector *src, ae_state *state)
{
    ae_vector_init(dst, src->cnt, src->datatype, state);
    if( src->cnt!=0 )
        memcpy(dst->ptr.p_ptr, src->ptr.p_ptr, (size_t)(src->cnt*ae_sizeof(src->datatype)));
}

/************************************************************************
This function initializes ae_vector using X-structure as source. New copy
of data is created, which is owned/managed by ae_vector  structure.  Both
structures (source and destination) remain completely  independent  after
this call.

dst                 destination matrix
src                 well, it is source
state               this parameter can be:
                    * pointer to current instance of ae_state, if you want
                      to automatically destroy this object  after  leaving
                      current frame
                    * NULL,   if  you  do  NOT  want  this  vector  to  be
                      automatically managed (say, if it is field  of  some
                      object)
                      
Error handling:
* on failure calls ae_break() with NULL state pointer. Usually it  results
  in abort() call.

dst is assumed to be uninitialized, its fields are ignored.
************************************************************************/
void ae_vector_init_from_x(ae_vector *dst, x_vector *src, ae_state *state)
{
    ae_vector_init(dst, (ae_int_t)src->cnt, (ae_datatype)src->datatype, state);
    if( src->cnt>0 )
        memcpy(dst->ptr.p_ptr, src->ptr, (size_t)(((ae_int_t)src->cnt)*ae_sizeof((ae_datatype)src->datatype)));
}

/************************************************************************
This function initializes ae_vector using X-structure as source.

New vector is attached to source:
* DST shares memory with SRC
* both DST and SRC are writable - all writes to DST  change  elements  of
  SRC and vice versa.
* DST can be reallocated with ae_vector_set_length(), in  this  case  SRC
  remains untouched
* SRC, however, CAN NOT BE REALLOCATED AS LONG AS DST EXISTS

NOTE: is_attached field is set  to  ae_true  in  order  to  indicate  that
      vector does not own its memory.

dst                 destination vector
src                 well, it is source
state               this parameter can be:
                    * pointer to current instance of ae_state, if you want
                      to automatically destroy this object  after  leaving
                      current frame
                    * NULL,   if  you  do  NOT  want  this  vector  to  be
                      automatically managed (say, if it is field  of  some
                      object)
                      
Error handling:
* on failure calls ae_break() with NULL state pointer. Usually it  results
  in abort() call.

dst is assumed to be uninitialized, its fields are ignored.
************************************************************************/
void ae_vector_attach_to_x(ae_vector *dst, x_vector *src, ae_state *state)
{
    volatile ae_int_t cnt;
    
    cnt = (ae_int_t)src->cnt;
    
    /* ensure that size is correct */
    ae_assert(cnt==src->cnt,  "ae_vector_attach_to_x(): 32/64 overflow", NULL);
    ae_assert(cnt>=0,         "ae_vector_attach_to_x(): negative length", NULL);
    
    /* init */
    dst->cnt = cnt;
    dst->datatype = (ae_datatype)src->datatype;
    dst->ptr.p_ptr = src->ptr;
    dst->is_attached = ae_true;
    ae_assert(
        ae_db_malloc(&dst->data, 0, state, state!=NULL),
        "ae_vector_attach_to_x(): malloc error",
        NULL);
}

/************************************************************************
This function changes length of ae_vector.

dst                 destination vector
newsize             vector size, may be zero
state               ALGLIB environment state

Error handling:
* if state is NULL, returns ae_false on allocation error
* if state is not NULL, calls ae_break() on allocation error
* returns ae_true on success

NOTES:
* vector must be initialized
* all contents is destroyed during setlength() call
* new size may be zero.
************************************************************************/
ae_bool ae_vector_set_length(ae_vector *dst, ae_int_t newsize, ae_state *state)
{
    /* ensure that size is >=0
       two ways to exit: 1) through ae_assert, if we have non-NULL state, 2) by returning ae_false */
    if( state!=NULL )
        ae_assert(newsize>=0, "ae_vector_set_length(): negative size", state);
    if( newsize<0 )
        return ae_false;

    /* set length */
    if( dst->cnt==newsize )
        return ae_true;
    dst->cnt = newsize;
    if( !ae_db_realloc(&dst->data, newsize*ae_sizeof(dst->datatype), state) )
        return ae_false;
    dst->ptr.p_ptr = dst->data.ptr;
    return ae_true;
}


/************************************************************************
This  function  provides  "CLEAR"  functionality  for vector (contents is
cleared, but structure still left in valid state).

The  function clears vector contents (releases all dynamically  allocated
memory). Vector may be in automatic management list  -  in this  case  it
will NOT be removed from list.

IMPORTANT: this function does NOT invalidates dst; it just  releases  all
dynamically allocated storage, but dst still may be used  after  call  to
ae_vector_set_length().

dst                 destination vector
************************************************************************/
void ae_vector_clear(ae_vector *dst)
{
    dst->cnt = 0;
    ae_db_free(&dst->data);
    dst->ptr.p_ptr = 0;
    dst->is_attached = ae_false;
}


/************************************************************************
This  function  provides "DESTROY"  functionality for vector (contents is
cleared, all internal structures are destroyed). For vectors it  is  same
as CLEAR.

dst                 destination vector
************************************************************************/
void ae_vector_destroy(ae_vector *dst)
{
    ae_vector_clear(dst);
}


/************************************************************************
This function efficiently swaps contents of two vectors, leaving other
pararemeters (automatic management, etc.) unchanged.
************************************************************************/
void ae_swap_vectors(ae_vector *vec1, ae_vector *vec2)
{
    ae_int_t cnt;
    ae_datatype datatype;
    void *p_ptr;
    
    ae_assert(!vec1->is_attached, "ALGLIB: internal error, attempt to swap vectors attached to X-object", NULL);
    ae_assert(!vec2->is_attached, "ALGLIB: internal error, attempt to swap vectors attached to X-object", NULL);
    
    ae_db_swap(&vec1->data, &vec2->data);
    
    cnt = vec1->cnt;
    datatype = vec1->datatype;
    p_ptr = vec1->ptr.p_ptr;
    vec1->cnt = vec2->cnt;
    vec1->datatype = vec2->datatype;
    vec1->ptr.p_ptr = vec2->ptr.p_ptr;
    vec2->cnt = cnt;
    vec2->datatype = datatype;
    vec2->ptr.p_ptr = p_ptr;
}

/************************************************************************
This function creates ae_matrix.

Matrix size may be zero, in such cases both rows and cols are zero.
Matrix contents is uninitialized.

dst                 destination matrix, assumed  to  be  unitialized,  its
                    fields are ignored
rows                rows count
cols                cols count
datatype            element type
state               depending on your desire to  register  matrix  in  the
                    current frame:
                    * pointer to ALGLIB environment state, if you want the
                      matrix to be automatically managed
                    * NULL, if you do not want it to be automatically
                      managed

Error handling:
* calls ae_break() with NULL state; usually it results in abort() call.

dst is assumed to be uninitialized, its fields are ignored.
************************************************************************/
void ae_matrix_init(ae_matrix *dst, ae_int_t rows, ae_int_t cols, ae_datatype datatype, ae_state *state)
{
    ae_assert(rows>=0 && cols>=0, "ae_matrix_init(): negative length", NULL);

    /* if one of rows/cols is zero, another MUST be too */
    if( rows==0 || cols==0 )
    {
        rows = 0;
        cols = 0;
    }

    /* init */
    dst->is_attached = ae_false;
    dst->rows = rows;
    dst->cols = cols;
    dst->stride = cols;
    while( dst->stride*ae_sizeof(datatype)%AE_DATA_ALIGN!=0 )
        dst->stride++;
    dst->datatype = datatype;
    ae_assert(
        ae_db_malloc(&dst->data, dst->rows*((ae_int_t)sizeof(void*)+dst->stride*ae_sizeof(datatype))+AE_DATA_ALIGN-1, state, state!=NULL),  /* TODO: change ae_db_malloc() */
        "ae_matrix_init(): failed to allocate memory",
        NULL);
    ae_matrix_update_row_pointers(dst, ae_align((char*)dst->data.ptr+dst->rows*sizeof(void*),AE_DATA_ALIGN));
}


/************************************************************************
This function creates copy of ae_matrix. A new copy of the data is created.

dst                 destination matrix
src                 well, it is source
state               this parameter can be:
                    * pointer to current instance of ae_state, if you want
                      to automatically destroy this object  after  leaving
                      current frame
                    * NULL,   if  you  do  NOT  want  this  vector  to  be
                      automatically managed (say, if it is field  of  some
                      object)
                      
Error handling:
* on failure calls ae_break() with NULL state pointer. Usually it  results
  in abort() call.

dst is assumed to be uninitialized, its fields are ignored.
************************************************************************/
void ae_matrix_init_copy(ae_matrix *dst, ae_matrix *src, ae_state *state)
{
    ae_int_t i;
    ae_matrix_init(dst, src->rows, src->cols, src->datatype, state);
    if( src->rows!=0 && src->cols!=0 )
    {
        if( dst->stride==src->stride )
            memcpy(dst->ptr.pp_void[0], src->ptr.pp_void[0], (size_t)(src->rows*src->stride*ae_sizeof(src->datatype)));
        else
            for(i=0; i<dst->rows; i++)
                memcpy(dst->ptr.pp_void[i], src->ptr.pp_void[i], (size_t)(dst->cols*ae_sizeof(dst->datatype)));
    }
}


/************************************************************************
This function initializes ae_matrix using X-structure as source. New copy
of data is created, which is owned/managed by ae_matrix  structure.  Both
structures (source and destination) remain completely  independent  after
this call.

dst                 destination matrix
src                 well, it is source
state               this parameter can be:
                    * pointer to current instance of ae_state, if you want
                      to automatically destroy this object  after  leaving
                      current frame
                    * NULL,   if  you  do  NOT  want  this  vector  to  be
                      automatically managed (say, if it is field  of  some
                      object)
                      
Error handling:
* on failure calls ae_break() with NULL state pointer. Usually it  results
  in abort() call.

dst is assumed to be uninitialized, its fields are ignored.
************************************************************************/
void ae_matrix_init_from_x(ae_matrix *dst, x_matrix *src, ae_state *state)
{
    char *p_src_row;
    char *p_dst_row;
    ae_int_t row_size;
    ae_int_t i;
    ae_matrix_init(dst, (ae_int_t)src->rows, (ae_int_t)src->cols, (ae_datatype)src->datatype, state);
    if( src->rows!=0 && src->cols!=0 )
    {
        p_src_row = (char*)src->ptr;
        p_dst_row = (char*)(dst->ptr.pp_void[0]);
        row_size = ae_sizeof((ae_datatype)src->datatype)*(ae_int_t)src->cols;
        for(i=0; i<src->rows; i++, p_src_row+=src->stride*ae_sizeof((ae_datatype)src->datatype), p_dst_row+=dst->stride*ae_sizeof((ae_datatype)src->datatype))
            memcpy(p_dst_row, p_src_row, (size_t)(row_size));
    }
}


/************************************************************************
This function initializes ae_matrix using X-structure as source.

New matrix is attached to source:
* DST shares memory with SRC
* both DST and SRC are writable - all writes to DST  change  elements  of
  SRC and vice versa.
* DST can be reallocated with ae_matrix_set_length(), in  this  case  SRC
  remains untouched
* SRC, however, CAN NOT BE REALLOCATED AS LONG AS DST EXISTS

dst                 destination matrix
src                 well, it is source
state               this parameter can be:
                    * pointer to current instance of ae_state, if you want
                      to automatically destroy this object  after  leaving
                      current frame
                    * NULL,   if  you  do  NOT  want  this  vector  to  be
                      automatically managed (say, if it is field  of  some
                      object)
                      
Error handling:
* on failure calls ae_break() with NULL state pointer. Usually it  results
  in abort() call.

dst is assumed to be uninitialized, its fields are ignored.
************************************************************************/
void ae_matrix_attach_to_x(ae_matrix *dst, x_matrix *src, ae_state *state)
{
    ae_int_t rows, cols;
    
    rows = (ae_int_t)src->rows;
    cols = (ae_int_t)src->cols;
    
    /* ensure that size is correct */
    ae_assert(rows==src->rows,      "ae_matrix_attach_to_x(): 32/64 overflow", NULL);
    ae_assert(cols==src->cols,      "ae_matrix_attach_to_x(): 32/64 overflow", NULL);
    ae_assert(rows>=0 && cols>=0,   "ae_matrix_attach_to_x(): negative length", NULL);
    
    /* if one of rows/cols is zero, another MUST be too */
    if( rows==0 || cols==0 )
    {
        rows = 0;
        cols = 0;
    }

    /* init */
    dst->is_attached = ae_true;
    dst->rows = rows;
    dst->cols = cols;
    dst->stride = cols;
    dst->datatype = (ae_datatype)src->datatype;
    dst->ptr.pp_void = NULL;
    ae_assert(
        ae_db_malloc(&dst->data, dst->rows*(ae_int_t)sizeof(void*), state, state!=NULL),
        "ae_matrix_attach_to_x(): malloc error",
        NULL);
    if( dst->rows>0 && dst->cols>0 )
    {
        ae_int_t i, rowsize;
        char *p_row;
        void **pp_ptr;
        
        p_row = (char*)src->ptr;
        rowsize = dst->stride*ae_sizeof(dst->datatype);
        pp_ptr  = (void**)dst->data.ptr;
        dst->ptr.pp_void = pp_ptr;
        for(i=0; i<dst->rows; i++, p_row+=rowsize)
            pp_ptr[i] = p_row;
    }
}


/************************************************************************
This function changes length of ae_matrix.

dst                 destination matrix
rows                size, may be zero
cols                size, may be zero
state               ALGLIB environment state

Error handling:
* if state is NULL, returns ae_false on allocation error
* if state is not NULL, calls ae_break() on allocation error
* returns ae_true on success

NOTES:
* matrix must be initialized
* all contents is destroyed during setlength() call
* new size may be zero.
************************************************************************/
ae_bool ae_matrix_set_length(ae_matrix *dst, ae_int_t rows, ae_int_t cols, ae_state *state)
{
    /* ensure that size is >=0
       two ways to exit: 1) through ae_assert, if we have non-NULL state, 2) by returning ae_false */
    if( state!=NULL )
        ae_assert(rows>=0 && cols>=0, "ae_matrix_set_length(): negative length", state);
    if( rows<0 || cols<0 )
        return ae_false;

    if( dst->rows==rows && dst->cols==cols )
        return ae_true;    
    dst->rows = rows;
    dst->cols = cols;
    dst->stride = cols;
    while( dst->stride*ae_sizeof(dst->datatype)%AE_DATA_ALIGN!=0 )
        dst->stride++;
    if( !ae_db_realloc(&dst->data, dst->rows*((ae_int_t)sizeof(void*)+dst->stride*ae_sizeof(dst->datatype))+AE_DATA_ALIGN-1, state) )
        return ae_false;
    ae_matrix_update_row_pointers(dst, ae_align((char*)dst->data.ptr+dst->rows*sizeof(void*),AE_DATA_ALIGN));
    return ae_true;
}


/************************************************************************
This  function  provides  "CLEAR"  functionality  for vector (contents is
cleared, but structure still left in valid state).

The  function clears matrix contents (releases all dynamically  allocated
memory). Matrix may be in automatic management list  -  in this  case  it
will NOT be removed from list.

IMPORTANT: this function does NOT invalidates dst; it just  releases  all
dynamically allocated storage, but dst still may be used  after  call  to
ae_matrix_set_length().

dst                 destination matrix
************************************************************************/
void ae_matrix_clear(ae_matrix *dst)
{
    dst->rows = 0;
    dst->cols = 0;
    dst->stride = 0;
    ae_db_free(&dst->data);
    dst->ptr.p_ptr = 0;
    dst->is_attached = ae_false;
}


/************************************************************************
This  function  provides  "DESTROY" functionality for matrix (contents is
cleared, but structure still left in valid state).

For matrices it is same as CLEAR.

dst                 destination matrix
************************************************************************/
void ae_matrix_destroy(ae_matrix *dst)
{
    ae_matrix_clear(dst);
}


/************************************************************************
This function efficiently swaps contents of two vectors, leaving other
pararemeters (automatic management, etc.) unchanged.
************************************************************************/
void ae_swap_matrices(ae_matrix *mat1, ae_matrix *mat2)
{
    ae_int_t rows;
    ae_int_t cols;
    ae_int_t stride;
    ae_datatype datatype;
    void *p_ptr;

    ae_assert(!mat1->is_attached, "ALGLIB: internal error, attempt to swap matrices attached to X-object", NULL);
    ae_assert(!mat2->is_attached, "ALGLIB: internal error, attempt to swap matrices attached to X-object", NULL);
    
    ae_db_swap(&mat1->data, &mat2->data);
    
    rows = mat1->rows;
    cols = mat1->cols;
    stride = mat1->stride;
    datatype = mat1->datatype;
    p_ptr = mat1->ptr.p_ptr;

    mat1->rows = mat2->rows;
    mat1->cols = mat2->cols;
    mat1->stride = mat2->stride;
    mat1->datatype = mat2->datatype;
    mat1->ptr.p_ptr = mat2->ptr.p_ptr;

    mat2->rows = rows;
    mat2->cols = cols;
    mat2->stride = stride;
    mat2->datatype = datatype;
    mat2->ptr.p_ptr = p_ptr;
}


/************************************************************************
This function creates smart pointer structure.

dst                 destination smart pointer.
                    already allocated, but not initialized.
subscriber          pointer to pointer which receives updates in the
                    internal object stored in ae_smart_ptr. Any update to
                    dst->ptr is translated to subscriber. Can be NULL.
state               this parameter can be:
                    * pointer to current instance of ae_state, if you want
                      to automatically destroy this object  after  leaving
                      current frame
                    * NULL,   if  you  do  NOT  want  this  vector  to  be
                      automatically managed (say, if it is field  of  some
                      object)
                      
Error handling:
* on failure calls ae_break() with NULL state pointer. Usually it  results
  in abort() call.

After initialization, smart pointer stores NULL pointer.
************************************************************************/
void ae_smart_ptr_init(ae_smart_ptr *dst, void **subscriber, ae_state *state)
{
    dst->subscriber = subscriber;
    dst->ptr = NULL;
    if( dst->subscriber!=NULL )
        *(dst->subscriber) = dst->ptr;
    dst->is_owner = ae_false;
    dst->is_dynamic = ae_false;
    dst->frame_entry.deallocator = ae_smart_ptr_destroy;
    dst->frame_entry.ptr = dst;
    if( state!=NULL )
        ae_db_attach(&dst->frame_entry, state);
}


/************************************************************************
This function clears smart pointer structure.

dst                 destination smart pointer.

After call to this function smart pointer contains NULL reference,  which
is  propagated  to  its  subscriber  (in  cases  non-NULL  subscruber was
specified during pointer creation).
************************************************************************/
void ae_smart_ptr_clear(void *_dst)
{
    ae_smart_ptr *dst = (ae_smart_ptr*)_dst;
    if( dst->is_owner && dst->ptr!=NULL )
    {
        dst->destroy(dst->ptr);
        if( dst->is_dynamic )
            ae_free(dst->ptr);
    }
    dst->is_owner = ae_false;
    dst->is_dynamic = ae_false;
    dst->ptr = NULL;
    dst->destroy = NULL;
    if( dst->subscriber!=NULL )
        *(dst->subscriber) = NULL;
}


/************************************************************************
This function dstroys smart pointer structure (same as clearing it).

dst                 destination smart pointer.
************************************************************************/
void ae_smart_ptr_destroy(void *_dst)
{
    ae_smart_ptr_clear(_dst);
}


/************************************************************************
This function assigns pointer to ae_smart_ptr structure.

dst                 destination smart pointer.
new_ptr             new pointer to assign
is_owner            whether smart pointer owns new_ptr
is_dynamic          whether object is dynamic - clearing such object
                    requires BOTH calling destructor function AND calling
                    ae_free() for memory occupied by object.
destroy             destructor function

In case smart pointer already contains non-NULL value and owns this value,
it is freed before assigning new pointer.

Changes in pointer are propagated to its  subscriber  (in  case  non-NULL
subscriber was specified during pointer creation).

You can specify NULL new_ptr, in which case is_owner/destroy are ignored.
************************************************************************/
void ae_smart_ptr_assign(ae_smart_ptr *dst, void *new_ptr, ae_bool is_owner, ae_bool is_dynamic, void (*destroy)(void*))
{
    if( dst->is_owner && dst->ptr!=NULL )
        dst->destroy(dst->ptr);
    if( new_ptr!=NULL )
    {
        dst->ptr = new_ptr;
        dst->is_owner = is_owner;
        dst->is_dynamic = is_dynamic;
        dst->destroy = destroy;
    }
    else
    {
        dst->ptr = NULL;
        dst->is_owner = ae_false;
        dst->is_dynamic = ae_false;
        dst->destroy = NULL;
    }
    if( dst->subscriber!=NULL )
        *(dst->subscriber) = dst->ptr;
}


/************************************************************************
This function releases pointer owned by ae_smart_ptr structure:
* all internal fields are set to NULL
* destructor function for internal pointer is NOT called even when we own
  this pointer. After this call ae_smart_ptr releases  ownership  of  its
  pointer and passes it to caller.
* changes in pointer are propagated to its subscriber (in  case  non-NULL
  subscriber was specified during pointer creation).

dst                 destination smart pointer.
************************************************************************/
void ae_smart_ptr_release(ae_smart_ptr *dst)
{
    dst->is_owner = ae_false;
    dst->is_dynamic = ae_false;
    dst->ptr = NULL;
    dst->destroy = NULL;
    if( dst->subscriber!=NULL )
        *(dst->subscriber) = NULL;
}

/************************************************************************
This function copies contents of ae_vector (SRC) to x_vector (DST).

This function should not be called for  DST  which  is  attached  to  SRC
(opposite situation, when SRC is attached to DST, is possible).

Depending on situation, following actions are performed 
* for SRC attached to DST, this function performs no actions (no need  to
  do anything)
* for independent vectors of different sizes it allocates storage in  DST
  and copy contents of SRC  to  DST.  DST->last_action field  is  set  to
  ACT_NEW_LOCATION, and DST->owner is set to OWN_AE.
* for  independent  vectors   of  same  sizes  it does not perform memory
  (re)allocation.  It  just  copies  SRC  to  already   existing   place.
  DST->last_action   is   set   to    ACT_SAME_LOCATION  (unless  it  was
  ACT_NEW_LOCATION), DST->owner is unmodified.

dst                 destination vector
src                 source, vector in x-format
state               ALGLIB environment state

NOTES:
* dst is assumed to be initialized. Its contents is freed before  copying
  data  from src  (if  size / type  are  different)  or  overwritten  (if
  possible given destination size).
************************************************************************/
void ae_x_set_vector(x_vector *dst, ae_vector *src, ae_state *state)
{
    if( src->ptr.p_ptr == dst->ptr )
    {
        /* src->ptr points to the beginning of dst, attached matrices, no need to copy */
        return;
    }
    if( dst->cnt!=src->cnt || dst->datatype!=src->datatype )
    {
        if( dst->owner==OWN_AE )
            ae_free(dst->ptr);
        dst->ptr = ae_malloc((size_t)(src->cnt*ae_sizeof(src->datatype)), state);
        if( src->cnt!=0 && dst->ptr==NULL )
            ae_break(state, ERR_OUT_OF_MEMORY, "ae_malloc(): out of memory");
        dst->last_action = ACT_NEW_LOCATION;
        dst->cnt = src->cnt;
        dst->datatype = src->datatype;
        dst->owner = OWN_AE;
    }
    else
    {
        if( dst->last_action==ACT_UNCHANGED )
            dst->last_action = ACT_SAME_LOCATION;
        else if( dst->last_action==ACT_SAME_LOCATION )
            dst->last_action = ACT_SAME_LOCATION;
        else if( dst->last_action==ACT_NEW_LOCATION )
            dst->last_action = ACT_NEW_LOCATION;
        else
            ae_assert(ae_false, "ALGLIB: internal error in ae_x_set_vector()", state);
    }
    if( src->cnt )
        memcpy(dst->ptr, src->ptr.p_ptr, (size_t)(src->cnt*ae_sizeof(src->datatype)));
}

/************************************************************************
This function copies contents of ae_matrix to x_matrix.

This function should not be called for  DST  which  is  attached  to  SRC
(opposite situation, when SRC is attached to DST, is possible).

Depending on situation, following actions are performed 
* for SRC attached to DST, this function performs no actions (no need  to
  do anything)
* for independent matrices of different sizes it allocates storage in DST
  and copy contents of SRC  to  DST.  DST->last_action field  is  set  to
  ACT_NEW_LOCATION, and DST->owner is set to OWN_AE.
* for  independent  matrices  of  same  sizes  it does not perform memory
  (re)allocation.  It  just  copies  SRC  to  already   existing   place.
  DST->last_action   is   set   to    ACT_SAME_LOCATION  (unless  it  was
  ACT_NEW_LOCATION), DST->owner is unmodified.

dst                 destination vector
src                 source, matrix in x-format
state               ALGLIB environment state

NOTES:
* dst is assumed to be initialized. Its contents is freed before  copying
  data  from src  (if  size / type  are  different)  or  overwritten  (if
  possible given destination size).
************************************************************************/
void ae_x_set_matrix(x_matrix *dst, ae_matrix *src, ae_state *state)
{
    char *p_src_row;
    char *p_dst_row;
    ae_int_t i;
    ae_int_t row_size;
    if( src->ptr.pp_void!=NULL && src->ptr.pp_void[0] == dst->ptr )
    {
        /* src->ptr points to the beginning of dst, attached matrices, no need to copy */
        return;
    }
    if( dst->rows!=src->rows || dst->cols!=src->cols || dst->datatype!=src->datatype )
    {
        if( dst->owner==OWN_AE )
            ae_free(dst->ptr);
        dst->rows = src->rows;
        dst->cols = src->cols;
        dst->stride = src->cols;
        dst->datatype = src->datatype;
        dst->ptr = ae_malloc((size_t)(dst->rows*((ae_int_t)dst->stride)*ae_sizeof(src->datatype)), state);
        if( dst->rows!=0 && dst->stride!=0 && dst->ptr==NULL )
            ae_break(state, ERR_OUT_OF_MEMORY, "ae_malloc(): out of memory");
        dst->last_action = ACT_NEW_LOCATION;
        dst->owner = OWN_AE;
    }
    else
    {
        if( dst->last_action==ACT_UNCHANGED )
            dst->last_action = ACT_SAME_LOCATION;
        else if( dst->last_action==ACT_SAME_LOCATION )
            dst->last_action = ACT_SAME_LOCATION;
        else if( dst->last_action==ACT_NEW_LOCATION )
            dst->last_action = ACT_NEW_LOCATION;
        else
            ae_assert(ae_false, "ALGLIB: internal error in ae_x_set_vector()", state);
    }
    if( src->rows!=0 && src->cols!=0 )
    {
        p_src_row = (char*)(src->ptr.pp_void[0]);
        p_dst_row = (char*)dst->ptr;
        row_size = ae_sizeof(src->datatype)*src->cols;
        for(i=0; i<src->rows; i++, p_src_row+=src->stride*ae_sizeof(src->datatype), p_dst_row+=dst->stride*ae_sizeof(src->datatype))
            memcpy(p_dst_row, p_src_row, (size_t)(row_size));
    }
}

/************************************************************************
This function attaches x_vector to ae_vector's contents.
Ownership of memory allocated is not changed (it is still managed by
ae_matrix).

dst                 destination vector
src                 source, vector in x-format
state               ALGLIB environment state

NOTES:
* dst is assumed to be initialized. Its contents is freed before
  attaching to src.
* this function doesn't need ae_state parameter because it can't fail
  (assuming correctly initialized src)
************************************************************************/
void ae_x_attach_to_vector(x_vector *dst, ae_vector *src)
{
    if( dst->owner==OWN_AE )
        ae_free(dst->ptr);
    dst->ptr = src->ptr.p_ptr;
    dst->last_action = ACT_NEW_LOCATION;
    dst->cnt = src->cnt;
    dst->datatype = src->datatype;
    dst->owner = OWN_CALLER;
}

/************************************************************************
This function attaches x_matrix to ae_matrix's contents.
Ownership of memory allocated is not changed (it is still managed by
ae_matrix).

dst                 destination vector
src                 source, matrix in x-format
state               ALGLIB environment state

NOTES:
* dst is assumed to be initialized. Its contents is freed before
  attaching to src.
* this function doesn't need ae_state parameter because it can't fail
  (assuming correctly initialized src)
************************************************************************/
void ae_x_attach_to_matrix(x_matrix *dst, ae_matrix *src)
{
    if( dst->owner==OWN_AE )
            ae_free(dst->ptr);
    dst->rows = src->rows;
    dst->cols = src->cols;
    dst->stride = src->stride;
    dst->datatype = src->datatype;
    dst->ptr = &(src->ptr.pp_double[0][0]);
    dst->last_action = ACT_NEW_LOCATION;
    dst->owner = OWN_CALLER;
}

/************************************************************************
This function clears x_vector. It does nothing  if vector is not owned by
ALGLIB environment.

dst                 vector
************************************************************************/
void x_vector_clear(x_vector *dst)
{
    if( dst->owner==OWN_AE )
        aligned_free(dst->ptr);
    dst->ptr = NULL;
    dst->cnt = 0;
}

/************************************************************************
Assertion

For  non-NULL  state  it  allows  to  gracefully  leave  ALGLIB  session,
removing all frames and deallocating registered dynamic data structure.

For NULL state it just abort()'s program.
************************************************************************/
void ae_assert(ae_bool cond, const char *msg, ae_state *state)
{
    if( !cond )
        ae_break(state, ERR_ASSERTION_FAILED, msg);
}

/************************************************************************
CPUID

Returns information about features CPU and compiler support.

You must tell ALGLIB what CPU family is used by defining AE_CPU symbol
(without this hint zero will be returned).

Note: results of this function depend on both CPU and compiler;
if compiler doesn't support SSE intrinsics, function won't set 
corresponding flag.
************************************************************************/
static volatile ae_bool _ae_cpuid_initialized = ae_false;
static volatile ae_bool _ae_cpuid_has_sse2 = ae_false;
ae_int_t ae_cpuid()
{
    /*
     * to speed up CPU detection we cache results from previous attempts
     * there is no synchronization, but it is still thread safe.
     *
     * thread safety is guaranteed on all modern architectures which
     * have following property: simultaneous writes by different cores
     * to the same location will be executed in serial manner.
     *
     */
    ae_int_t result;
    
    /*
     * if not initialized, determine system properties
     */
    if( !_ae_cpuid_initialized )
    {
        /*
         * SSE2
         */
#if defined(AE_CPU)
#if (AE_CPU==AE_INTEL) && defined(AE_HAS_SSE2_INTRINSICS)
#if AE_COMPILER==AE_MSVC
        {
            int CPUInfo[4];
            __cpuid(CPUInfo, 1);
            if( (CPUInfo[3]&0x04000000)!=0 )
                _ae_cpuid_has_sse2 = ae_true;
        }
#elif AE_COMPILER==AE_GNUC
        {
            ae_int_t a,b,c,d;
            __asm__ __volatile__ ("cpuid": "=a" (a), "=b" (b), "=c" (c), "=d" (d) : "a" (1));
            if( (d&0x04000000)!=0 )
                _ae_cpuid_has_sse2 = ae_true;
        }
#elif AE_COMPILER==AE_SUNC
        {
            ae_int_t a,b,c,d;
            __asm__ __volatile__ ("cpuid": "=a" (a), "=b" (b), "=c" (c), "=d" (d) : "a" (1));
            if( (d&0x04000000)!=0 )
                _ae_cpuid_has_sse2 = ae_true;
        }
#else
#endif
#endif
#endif
        /*
         * Perform one more CPUID call to generate memory fence
         */
#if AE_CPU==AE_INTEL
#if AE_COMPILER==AE_MSVC
        { int CPUInfo[4]; __cpuid(CPUInfo, 1); }
#elif AE_COMPILER==AE_GNUC
        { ae_int_t a,b,c,d; __asm__ __volatile__ ("cpuid": "=a" (a), "=b" (b), "=c" (c), "=d" (d) : "a" (1)); }
#elif AE_COMPILER==AE_SUNC
        { ae_int_t a,b,c,d; __asm__ __volatile__ ("cpuid": "=a" (a), "=b" (b), "=c" (c), "=d" (d) : "a" (1)); }
#else
#endif
#endif
        
        /*
         * set initialization flag
         */
        _ae_cpuid_initialized = ae_true;
    }
    
    /*
     * return
     */
    result = 0;
    if( _ae_cpuid_has_sse2 )
        result = result|CPU_SSE2;
    return result;
}

/************************************************************************
Real math functions
************************************************************************/
ae_bool ae_fp_eq(double v1, double v2)
{
    /* IEEE-strict floating point comparison */
    volatile double x = v1;
    volatile double y = v2;
    return x==y;
}

ae_bool ae_fp_neq(double v1, double v2)
{
    /* IEEE-strict floating point comparison */
    return !ae_fp_eq(v1,v2);
}

ae_bool ae_fp_less(double v1, double v2)
{
    /* IEEE-strict floating point comparison */
    volatile double x = v1;
    volatile double y = v2;
    return x<y;
}

ae_bool ae_fp_less_eq(double v1, double v2)
{
    /* IEEE-strict floating point comparison */
    volatile double x = v1;
    volatile double y = v2;
    return x<=y;
}

ae_bool ae_fp_greater(double v1, double v2)
{
    /* IEEE-strict floating point comparison */
    volatile double x = v1;
    volatile double y = v2;
    return x>y;
}

ae_bool ae_fp_greater_eq(double v1, double v2)
{
    /* IEEE-strict floating point comparison */
    volatile double x = v1;
    volatile double y = v2;
    return x>=y;
}

ae_bool ae_isfinite_stateless(double x, ae_int_t endianness)
{
    union _u
    {
        double a;
        ae_int32_t p[2];
    } u;
    ae_int32_t high;
    u.a = x;
    if( endianness==AE_LITTLE_ENDIAN )
        high = u.p[1];
    else
        high = u.p[0];
    return (high & (ae_int32_t)0x7FF00000)!=(ae_int32_t)0x7FF00000;
}

ae_bool ae_isnan_stateless(double x,    ae_int_t endianness)
{
    union _u
    {
        double a;
        ae_int32_t p[2];
    } u;
    ae_int32_t high, low;
    u.a = x;
    if( endianness==AE_LITTLE_ENDIAN )
    {
        high = u.p[1];
        low =  u.p[0];
    }
    else
    {
        high = u.p[0];
        low =  u.p[1];
    }
    return ((high &0x7FF00000)==0x7FF00000) && (((high &0x000FFFFF)!=0) || (low!=0));
}

ae_bool ae_isinf_stateless(double x,    ae_int_t endianness)
{
    union _u
    {
        double a;
        ae_int32_t p[2];
    } u;
    ae_int32_t high, low;
    u.a = x;
    if( endianness==AE_LITTLE_ENDIAN )
    {
        high = u.p[1];
        low  = u.p[0];
    }
    else
    {
        high = u.p[0];
        low  = u.p[1];
    }
    
    /* 31 least significant bits of high are compared */
    return ((high&0x7FFFFFFF)==0x7FF00000) && (low==0); 
}

ae_bool ae_isposinf_stateless(double x, ae_int_t endianness)
{
    union _u
    {
        double a;
        ae_int32_t p[2];
    } u;
    ae_int32_t high, low;
    u.a = x;
    if( endianness==AE_LITTLE_ENDIAN )
    {
        high = u.p[1];
        low  = u.p[0];
    }
    else
    {
        high = u.p[0];
        low  = u.p[1];
    }
    
    /* all 32 bits of high are compared */
    return (high==(ae_int32_t)0x7FF00000) && (low==0); 
}

ae_bool ae_isneginf_stateless(double x, ae_int_t endianness)
{
    union _u
    {
        double a;
        ae_int32_t p[2];
    } u;
    ae_int32_t high, low;
    u.a = x;
    if( endianness==AE_LITTLE_ENDIAN )
    {
        high = u.p[1];
        low  = u.p[0];
    }
    else
    {
        high = u.p[0];
        low  = u.p[1];
    }
    
    /* this code is a bit tricky to avoid comparison of high with 0xFFF00000, which may be unsafe with some buggy compilers */
    return ((high&0x7FFFFFFF)==0x7FF00000) && (high!=(ae_int32_t)0x7FF00000) && (low==0);
}

ae_int_t ae_get_endianness()
{
    union
    {
        double a;
        ae_int32_t p[2];
    } u;
    
    /*
     * determine endianness
     * two types are supported: big-endian and little-endian.
     * mixed-endian hardware is NOT supported.
     *
     * 1983 is used as magic number because its non-periodic double 
     * representation allow us to easily distinguish between upper 
     * and lower halfs and to detect mixed endian hardware.
     *
     */
    u.a = 1.0/1983.0; 
    if( u.p[1]==(ae_int32_t)0x3f408642 )
        return AE_LITTLE_ENDIAN;
    if( u.p[0]==(ae_int32_t)0x3f408642 )
        return AE_BIG_ENDIAN;
    return AE_MIXED_ENDIAN;
}

ae_bool ae_isfinite(double x,ae_state *state)
{
    return ae_isfinite_stateless(x, state->endianness);
}

ae_bool ae_isnan(double x,   ae_state *state)
{
    return ae_isnan_stateless(x, state->endianness);
}

ae_bool ae_isinf(double x,   ae_state *state)
{
    return ae_isinf_stateless(x, state->endianness);
}

ae_bool ae_isposinf(double x,ae_state *state)
{
    return ae_isposinf_stateless(x, state->endianness);
}

ae_bool ae_isneginf(double x,ae_state *state)
{
    return ae_isneginf_stateless(x, state->endianness);
}

double ae_fabs(double x,  ae_state *state)
{
    return fabs(x);
}

ae_int_t ae_iabs(ae_int_t x, ae_state *state)
{
    return x>=0 ? x : -x;
}

double ae_sqr(double x,  ae_state *state)
{
    return x*x;
}

double ae_sqrt(double x, ae_state *state)
{
    return sqrt(x);
}

ae_int_t ae_sign(double x, ae_state *state)
{
    if( x>0 ) return  1;
    if( x<0 ) return -1;
    return 0;
}

ae_int_t ae_round(double x, ae_state *state)
{
    return (ae_int_t)(ae_ifloor(x+0.5,state));
}

ae_int_t ae_trunc(double x, ae_state *state)
{
    return (ae_int_t)(x>0 ? ae_ifloor(x,state) : ae_iceil(x,state));
}

ae_int_t ae_ifloor(double x, ae_state *state)
{
    return (ae_int_t)(floor(x));
}

ae_int_t ae_iceil(double x,  ae_state *state)
{
    return (ae_int_t)(ceil(x));
}

ae_int_t ae_maxint(ae_int_t m1, ae_int_t m2, ae_state *state)
{
    return m1>m2 ? m1 : m2;
}

ae_int_t ae_minint(ae_int_t m1, ae_int_t m2, ae_state *state)
{
    return m1>m2 ? m2 : m1;
}

double ae_maxreal(double m1, double m2, ae_state *state)
{
    return m1>m2 ? m1 : m2;
}

double ae_minreal(double m1, double m2, ae_state *state)
{
    return m1>m2 ? m2 : m1;
}

double ae_randomreal(ae_state *state)
{
    int i1 = rand();
    int i2 = rand();
    double mx = (double)(RAND_MAX)+1.0;
    volatile double tmp0 = i2/mx;
    volatile double tmp1 = i1+tmp0;
    return tmp1/mx;
}

ae_int_t ae_randominteger(ae_int_t maxv, ae_state *state)
{
    return rand()%maxv;
}

double   ae_sin(double x, ae_state *state)
{
    return sin(x);
}

double   ae_cos(double x, ae_state *state)
{
    return cos(x);
}

double   ae_tan(double x, ae_state *state)
{
    return tan(x);
}

double   ae_sinh(double x, ae_state *state)
{
    return sinh(x);
}

double   ae_cosh(double x, ae_state *state)
{
    return cosh(x);
}
double   ae_tanh(double x, ae_state *state)
{
    return tanh(x);
}

double   ae_asin(double x, ae_state *state)
{
    return asin(x);
}

double   ae_acos(double x, ae_state *state)
{
    return acos(x);
}

double   ae_atan(double x, ae_state *state)
{
    return atan(x);
}

double   ae_atan2(double y, double x, ae_state *state)
{
    return atan2(y,x);
}

double   ae_log(double x, ae_state *state)
{
    return log(x);
}

double   ae_pow(double x, double y, ae_state *state)
{
    return pow(x,y);
}

double   ae_exp(double x, ae_state *state)
{
    return exp(x);
}

/************************************************************************
Symmetric/Hermitian properties: check and force
************************************************************************/
static void x_split_length(ae_int_t n, ae_int_t nb, ae_int_t* n1, ae_int_t* n2)
{
    ae_int_t r;
    if( n<=nb )
    {
        *n1 = n;
        *n2 = 0;
    }
    else
    {
        if( n%nb!=0 )
        {
            *n2 = n%nb;
            *n1 = n-(*n2);
        }
        else
        {
            *n2 = n/2;
            *n1 = n-(*n2);
            if( *n1%nb==0 )
            {
                return;
            }
            r = nb-*n1%nb;
            *n1 = *n1+r;
            *n2 = *n2-r;
        }
    }
}
static double x_safepythag2(double x, double y)
{
    double w;
    double xabs;
    double yabs;
    double z;
    xabs = fabs(x);
    yabs = fabs(y);
    w = xabs>yabs ? xabs : yabs;
    z = xabs<yabs ? xabs : yabs;
    if( z==0 )
        return w;
    else
    {
        double t;
        t = z/w;
        return w*sqrt(1+t*t);
    }
}
/*
 * this function checks difference between offdiagonal blocks BL and BU
 * (see below). Block BL is specified by offsets (offset0,offset1)  and
 * sizes (len0,len1).
 *
 *     [ .          ]
 *     [   A0  BU   ]
 * A = [   BL  A1   ]
 *     [          . ]
 *
 *  this subroutine updates current values of:
 *  a) mx       maximum value of A[i,j] found so far
 *  b) err      componentwise difference between elements of BL and BU^T
 *
 */
static void is_symmetric_rec_off_stat(x_matrix *a, ae_int_t offset0, ae_int_t offset1, ae_int_t len0, ae_int_t len1, ae_bool *nonfinite, double *mx, double *err, ae_state *_state)
{
    /* try to split problem into two smaller ones */
    if( len0>x_nb || len1>x_nb )
    {
        ae_int_t n1, n2;
        if( len0>len1 )
        {
            x_split_length(len0, x_nb, &n1, &n2);
            is_symmetric_rec_off_stat(a, offset0,    offset1, n1, len1, nonfinite, mx, err, _state);
            is_symmetric_rec_off_stat(a, offset0+n1, offset1, n2, len1, nonfinite, mx, err, _state);
        }
        else
        {
            x_split_length(len1, x_nb, &n1, &n2);
            is_symmetric_rec_off_stat(a, offset0, offset1,    len0, n1, nonfinite, mx, err, _state);
            is_symmetric_rec_off_stat(a, offset0, offset1+n1, len0, n2, nonfinite, mx, err, _state);
        }
        return;
    }
    else
    {
        /* base case */
        double *p1, *p2, *prow, *pcol;
        double v;
        ae_int_t i, j;

        p1 = (double*)(a->ptr)+offset0*a->stride+offset1;
        p2 = (double*)(a->ptr)+offset1*a->stride+offset0;
        for(i=0; i<len0; i++)
        {
            pcol = p2+i;
            prow = p1+i*a->stride;
            for(j=0; j<len1; j++)
            {
                if( !ae_isfinite(*pcol,_state) || !ae_isfinite(*prow,_state) )
                {
                    *nonfinite = ae_true;
                }
                else
                {
                    v = fabs(*pcol);
                    *mx =  *mx>v ? *mx : v;
                    v = fabs(*prow);
                    *mx =  *mx>v ? *mx : v;
                    v = fabs(*pcol-*prow);
                    *err = *err>v ? *err : v;
                }                
                pcol += a->stride;
                prow++;
            }
        }
    }
}
/*
 * this function checks that diagonal block A0 is symmetric.
 * Block A0 is specified by its offset and size.
 *
 *     [ .          ]
 *     [   A0       ]
 * A = [       .    ]
 *     [          . ]
 *
 *  this subroutine updates current values of:
 *  a) mx       maximum value of A[i,j] found so far
 *  b) err      componentwise difference between A0 and A0^T
 *
 */
static void is_symmetric_rec_diag_stat(x_matrix *a, ae_int_t offset, ae_int_t len, ae_bool *nonfinite, double *mx, double *err, ae_state *_state)
{
    double *p, *prow, *pcol;
    double v;
    ae_int_t i, j;
    
    /* try to split problem into two smaller ones */
    if( len>x_nb )
    {
        ae_int_t n1, n2;
        x_split_length(len, x_nb, &n1, &n2);
        is_symmetric_rec_diag_stat(a, offset, n1, nonfinite, mx, err, _state);
        is_symmetric_rec_diag_stat(a, offset+n1, n2, nonfinite, mx, err, _state);
        is_symmetric_rec_off_stat(a, offset+n1, offset, n2, n1, nonfinite, mx, err, _state);
        return;
    }
    
    /* base case */
    p = (double*)(a->ptr)+offset*a->stride+offset;
    for(i=0; i<len; i++)
    {
        pcol = p+i;
        prow = p+i*a->stride;
        for(j=0; j<i; j++,pcol+=a->stride,prow++)
        {
            if( !ae_isfinite(*pcol,_state) || !ae_isfinite(*prow,_state) )
            {
                *nonfinite = ae_true;
            }
            else
            {
                v = fabs(*pcol);
                *mx =  *mx>v ? *mx : v;
                v = fabs(*prow);
                *mx =  *mx>v ? *mx : v;
                v = fabs(*pcol-*prow);
                *err = *err>v ? *err : v;
            }
        }
        v = fabs(p[i+i*a->stride]);
        *mx =  *mx>v ? *mx : v;
    }
}
/*
 * this function checks difference between offdiagonal blocks BL and BU
 * (see below). Block BL is specified by offsets (offset0,offset1)  and
 * sizes (len0,len1).
 *
 *     [ .          ]
 *     [   A0  BU   ]
 * A = [   BL  A1   ]
 *     [          . ]
 *
 *  this subroutine updates current values of:
 *  a) mx       maximum value of A[i,j] found so far
 *  b) err      componentwise difference between elements of BL and BU^H
 *
 */
static void is_hermitian_rec_off_stat(x_matrix *a, ae_int_t offset0, ae_int_t offset1, ae_int_t len0, ae_int_t len1, ae_bool *nonfinite, double *mx, double *err, ae_state *_state)
{
    /* try to split problem into two smaller ones */
    if( len0>x_nb || len1>x_nb )
    {
        ae_int_t n1, n2;
        if( len0>len1 )
        {
            x_split_length(len0, x_nb, &n1, &n2);
            is_hermitian_rec_off_stat(a, offset0,    offset1, n1, len1, nonfinite, mx, err, _state);
            is_hermitian_rec_off_stat(a, offset0+n1, offset1, n2, len1, nonfinite, mx, err, _state);
        }
        else
        {
            x_split_length(len1, x_nb, &n1, &n2);
            is_hermitian_rec_off_stat(a, offset0, offset1,    len0, n1, nonfinite, mx, err, _state);
            is_hermitian_rec_off_stat(a, offset0, offset1+n1, len0, n2, nonfinite, mx, err, _state);
        }
        return;
    }
    else
    {
        /* base case */
        ae_complex *p1, *p2, *prow, *pcol;
        double v;
        ae_int_t i, j;

        p1 = (ae_complex*)(a->ptr)+offset0*a->stride+offset1;
        p2 = (ae_complex*)(a->ptr)+offset1*a->stride+offset0;
        for(i=0; i<len0; i++)
        {
            pcol = p2+i;
            prow = p1+i*a->stride;
            for(j=0; j<len1; j++)
            {
                if( !ae_isfinite(pcol->x, _state) || !ae_isfinite(pcol->y, _state) || !ae_isfinite(prow->x, _state) || !ae_isfinite(prow->y, _state) )
                {
                    *nonfinite = ae_true;
                }
                else
                {
                    v = x_safepythag2(pcol->x, pcol->y);
                    *mx =  *mx>v ? *mx : v;
                    v = x_safepythag2(prow->x, prow->y);
                    *mx =  *mx>v ? *mx : v;
                    v = x_safepythag2(pcol->x-prow->x, pcol->y+prow->y);
                    *err = *err>v ? *err : v;
                }
                pcol += a->stride;
                prow++;
            }
        }
    }
}
/*
 * this function checks that diagonal block A0 is Hermitian.
 * Block A0 is specified by its offset and size.
 *
 *     [ .          ]
 *     [   A0       ]
 * A = [       .    ]
 *     [          . ]
 *
 *  this subroutine updates current values of:
 *  a) mx       maximum value of A[i,j] found so far
 *  b) err      componentwise difference between A0 and A0^H
 *
 */
static void is_hermitian_rec_diag_stat(x_matrix *a, ae_int_t offset, ae_int_t len, ae_bool *nonfinite, double *mx, double *err, ae_state *_state)
{
    ae_complex *p, *prow, *pcol;
    double v;
    ae_int_t i, j;
    
    /* try to split problem into two smaller ones */
    if( len>x_nb )
    {
        ae_int_t n1, n2;
        x_split_length(len, x_nb, &n1, &n2);
        is_hermitian_rec_diag_stat(a, offset, n1, nonfinite, mx, err, _state);
        is_hermitian_rec_diag_stat(a, offset+n1, n2, nonfinite, mx, err, _state);
        is_hermitian_rec_off_stat(a, offset+n1, offset, n2, n1, nonfinite, mx, err, _state);
        return;
    }
    
    /* base case */
    p = (ae_complex*)(a->ptr)+offset*a->stride+offset;
    for(i=0; i<len; i++)
    {
        pcol = p+i;
        prow = p+i*a->stride;
        for(j=0; j<i; j++,pcol+=a->stride,prow++)
        {
            if( !ae_isfinite(pcol->x, _state) || !ae_isfinite(pcol->y, _state) || !ae_isfinite(prow->x, _state) || !ae_isfinite(prow->y, _state) )
            {
                *nonfinite = ae_true;
            }
            else
            {
                v = x_safepythag2(pcol->x, pcol->y);
                *mx =  *mx>v ? *mx : v;
                v = x_safepythag2(prow->x, prow->y);
                *mx =  *mx>v ? *mx : v;
                v = x_safepythag2(pcol->x-prow->x, pcol->y+prow->y);
                *err = *err>v ? *err : v;
            }
        }
        if( !ae_isfinite(p[i+i*a->stride].x, _state) || !ae_isfinite(p[i+i*a->stride].y, _state) )
        {
            *nonfinite = ae_true;
        }
        else
        {
            v = fabs(p[i+i*a->stride].x);
            *mx =  *mx>v ? *mx : v;
            v = fabs(p[i+i*a->stride].y);
            *err =  *err>v ? *err : v;
        }
    }
}
/*
 * this function copies offdiagonal block BL to its symmetric counterpart
 * BU (see below). Block BL is specified by offsets (offset0,offset1)
 * and sizes (len0,len1).
 *
 *     [ .          ]
 *     [   A0  BU   ]
 * A = [   BL  A1   ]
 *     [          . ]
 *
 */
static void force_symmetric_rec_off_stat(x_matrix *a, ae_int_t offset0, ae_int_t offset1, ae_int_t len0, ae_int_t len1)
{
    /* try to split problem into two smaller ones */
    if( len0>x_nb || len1>x_nb )
    {
        ae_int_t n1, n2;
        if( len0>len1 )
        {
            x_split_length(len0, x_nb, &n1, &n2);
            force_symmetric_rec_off_stat(a, offset0,    offset1, n1, len1);
            force_symmetric_rec_off_stat(a, offset0+n1, offset1, n2, len1);
        }
        else
        {
            x_split_length(len1, x_nb, &n1, &n2);
            force_symmetric_rec_off_stat(a, offset0, offset1,    len0, n1);
            force_symmetric_rec_off_stat(a, offset0, offset1+n1, len0, n2);
        }
        return;
    }
    else
    {
        /* base case */
        double *p1, *p2, *prow, *pcol;
        ae_int_t i, j;

        p1 = (double*)(a->ptr)+offset0*a->stride+offset1;
        p2 = (double*)(a->ptr)+offset1*a->stride+offset0;
        for(i=0; i<len0; i++)
        {
            pcol = p2+i;
            prow = p1+i*a->stride;
            for(j=0; j<len1; j++)
            {
                *pcol = *prow;
                pcol += a->stride;
                prow++;
            }
        }
    }
}
/*
 * this function copies lower part of diagonal block A0 to its upper part
 * Block is specified by offset and size.
 *
 *     [ .          ]
 *     [   A0       ]
 * A = [       .    ]
 *     [          . ]
 *
 */
static void force_symmetric_rec_diag_stat(x_matrix *a, ae_int_t offset, ae_int_t len)
{
    double *p, *prow, *pcol;
    ae_int_t i, j;
    
    /* try to split problem into two smaller ones */
    if( len>x_nb )
    {
        ae_int_t n1, n2;
        x_split_length(len, x_nb, &n1, &n2);
        force_symmetric_rec_diag_stat(a, offset, n1);
        force_symmetric_rec_diag_stat(a, offset+n1, n2);
        force_symmetric_rec_off_stat(a, offset+n1, offset, n2, n1);
        return;
    }
    
    /* base case */
    p = (double*)(a->ptr)+offset*a->stride+offset;
    for(i=0; i<len; i++)
    {
        pcol = p+i;
        prow = p+i*a->stride;
        for(j=0; j<i; j++,pcol+=a->stride,prow++)
            *pcol = *prow;
    }
}
/*
 * this function copies Hermitian transpose of offdiagonal block BL to
 * its symmetric counterpart BU (see below). Block BL is specified by
 * offsets (offset0,offset1) and sizes (len0,len1).
 *
 *     [ .          ]
 *     [   A0  BU   ]
 * A = [   BL  A1   ]
 *     [          . ]
 */
static void force_hermitian_rec_off_stat(x_matrix *a, ae_int_t offset0, ae_int_t offset1, ae_int_t len0, ae_int_t len1)
{
    /* try to split problem into two smaller ones */
    if( len0>x_nb || len1>x_nb )
    {
        ae_int_t n1, n2;
        if( len0>len1 )
        {
            x_split_length(len0, x_nb, &n1, &n2);
            force_hermitian_rec_off_stat(a, offset0,    offset1, n1, len1);
            force_hermitian_rec_off_stat(a, offset0+n1, offset1, n2, len1);
        }
        else
        {
            x_split_length(len1, x_nb, &n1, &n2);
            force_hermitian_rec_off_stat(a, offset0, offset1,    len0, n1);
            force_hermitian_rec_off_stat(a, offset0, offset1+n1, len0, n2);
        }
        return;
    }
    else
    {
        /* base case */
        ae_complex *p1, *p2, *prow, *pcol;
        ae_int_t i, j;

        p1 = (ae_complex*)(a->ptr)+offset0*a->stride+offset1;
        p2 = (ae_complex*)(a->ptr)+offset1*a->stride+offset0;
        for(i=0; i<len0; i++)
        {
            pcol = p2+i;
            prow = p1+i*a->stride;
            for(j=0; j<len1; j++)
            {
                *pcol = *prow;
                pcol += a->stride;
                prow++;
            }
        }
    }
}
/*
 * this function copies Hermitian transpose of lower part of
 * diagonal block A0 to its upper part Block is specified by offset and size.
 *
 *     [ .          ]
 *     [   A0       ]
 * A = [       .    ]
 *     [          . ]
 *
 */
static void force_hermitian_rec_diag_stat(x_matrix *a, ae_int_t offset, ae_int_t len)
{
    ae_complex *p, *prow, *pcol;
    ae_int_t i, j;
    
    /* try to split problem into two smaller ones */
    if( len>x_nb )
    {
        ae_int_t n1, n2;
        x_split_length(len, x_nb, &n1, &n2);
        force_hermitian_rec_diag_stat(a, offset, n1);
        force_hermitian_rec_diag_stat(a, offset+n1, n2);
        force_hermitian_rec_off_stat(a, offset+n1, offset, n2, n1);
        return;
    }
    
    /* base case */
    p = (ae_complex*)(a->ptr)+offset*a->stride+offset;
    for(i=0; i<len; i++)
    {
        pcol = p+i;
        prow = p+i*a->stride;
        for(j=0; j<i; j++,pcol+=a->stride,prow++)
            *pcol = *prow;
    }
}
ae_bool x_is_symmetric(x_matrix *a)
{
    double mx, err;
    ae_bool nonfinite;
    ae_state _alglib_env_state;
    if( a->datatype!=DT_REAL )
        return ae_false;
    if( a->cols!=a->rows )
        return ae_false;
    if( a->cols==0 || a->rows==0 )
        return ae_true;
    ae_state_init(&_alglib_env_state);
    mx = 0;
    err = 0;
    nonfinite = ae_false;
    is_symmetric_rec_diag_stat(a, 0, (ae_int_t)a->rows, &nonfinite, &mx, &err, &_alglib_env_state);
    if( nonfinite )
        return ae_false;
    if( mx==0 )
        return ae_true;
    return err/mx<=1.0E-14;
}
ae_bool x_is_hermitian(x_matrix *a)
{
    double mx, err;
    ae_bool nonfinite;
    ae_state _alglib_env_state;
    if( a->datatype!=DT_COMPLEX )
        return ae_false;
    if( a->cols!=a->rows )
        return ae_false;
    if( a->cols==0 || a->rows==0 )
        return ae_true;
    ae_state_init(&_alglib_env_state);
    mx = 0;
    err = 0;
    nonfinite = ae_false;
    is_hermitian_rec_diag_stat(a, 0, (ae_int_t)a->rows, &nonfinite, &mx, &err, &_alglib_env_state);
    if( nonfinite )
        return ae_false;
    if( mx==0 )
        return ae_true;
    return err/mx<=1.0E-14;
}
ae_bool x_force_symmetric(x_matrix *a)
{
    if( a->datatype!=DT_REAL )
        return ae_false;
    if( a->cols!=a->rows )
        return ae_false;
    if( a->cols==0 || a->rows==0 )
        return ae_true;
    force_symmetric_rec_diag_stat(a, 0, (ae_int_t)a->rows);
    return ae_true;
}
ae_bool x_force_hermitian(x_matrix *a)
{
    if( a->datatype!=DT_COMPLEX )
        return ae_false;
    if( a->cols!=a->rows )
        return ae_false;
    if( a->cols==0 || a->rows==0 )
        return ae_true;
    force_hermitian_rec_diag_stat(a, 0, (ae_int_t)a->rows);
    return ae_true;
}

ae_bool ae_is_symmetric(ae_matrix *a)
{
    x_matrix x;
    x.owner = OWN_CALLER;
    ae_x_attach_to_matrix(&x, a);
    return x_is_symmetric(&x);
}

ae_bool ae_is_hermitian(ae_matrix *a)
{
    x_matrix x;
    x.owner = OWN_CALLER;
    ae_x_attach_to_matrix(&x, a);
    return x_is_hermitian(&x);
}

ae_bool ae_force_symmetric(ae_matrix *a)
{
    x_matrix x;
    x.owner = OWN_CALLER;
    ae_x_attach_to_matrix(&x, a);
    return x_force_symmetric(&x);
}

ae_bool ae_force_hermitian(ae_matrix *a)
{
    x_matrix x;
    x.owner = OWN_CALLER;
    ae_x_attach_to_matrix(&x, a);
    return x_force_hermitian(&x);
}

/************************************************************************
This function converts six-bit value (from 0 to 63)  to  character  (only
digits, lowercase and uppercase letters, minus and underscore are used).

If v is negative or greater than 63, this function returns '?'.
************************************************************************/
static char _sixbits2char_tbl[64] = { 
        '0', '1', '2', '3', '4', '5', '6', '7',
        '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
        'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
        'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
        'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 
        'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 
        'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 
        'u', 'v', 'w', 'x', 'y', 'z', '-', '_' };

char ae_sixbits2char(ae_int_t v)
{
    
    if( v<0 || v>63 )
        return '?';
    return _sixbits2char_tbl[v]; 
    
    /* v is correct, process it */
    /*if( v<10 )
        return '0'+v;
    v -= 10;
    if( v<26 )
        return 'A'+v;
    v -= 26;
    if( v<26 )
        return 'a'+v;
    v -= 26;
    return v==0 ? '-' : '_';*/
}

/************************************************************************
This function converts character to six-bit value (from 0 to 63).

This function is inverse of ae_sixbits2char()
If c is not correct character, this function returns -1.
************************************************************************/
static ae_int_t _ae_char2sixbits_tbl[] = {
    -1, -1, -1, -1, -1, -1, -1, -1,
    -1, -1, -1, -1, -1, -1, -1, -1,
    -1, -1, -1, -1, -1, -1, -1, -1,
    -1, -1, -1, -1, -1, -1, -1, -1,
    -1, -1, -1, -1, -1, -1, -1, -1,
    -1, -1, -1, -1, -1, 62, -1, -1,
     0,  1,  2,  3,  4,  5,  6,  7,
     8,  9, -1, -1, -1, -1, -1, -1,
    -1, 10, 11, 12, 13, 14, 15, 16,
    17, 18, 19, 20, 21, 22, 23, 24,
    25, 26, 27, 28, 29, 30, 31, 32,
    33, 34, 35, -1, -1, -1, -1, 63,
    -1, 36, 37, 38, 39, 40, 41, 42,
    43, 44, 45, 46, 47, 48, 49, 50,
    51, 52, 53, 54, 55, 56, 57, 58,
    59, 60, 61, -1, -1, -1, -1, -1 };
ae_int_t ae_char2sixbits(char c)
{
    return (c>=0 && c<127) ? _ae_char2sixbits_tbl[(int)c] : -1;
}

/************************************************************************
This function converts three bytes (24 bits) to four six-bit values 
(24 bits again).

src     pointer to three bytes
dst     pointer to four ints
************************************************************************/
void ae_threebytes2foursixbits(const unsigned char *src, ae_int_t *dst)
{
    dst[0] = src[0] & 0x3F;
    dst[1] = (src[0]>>6) | ((src[1]&0x0F)<<2);
    dst[2] = (src[1]>>4) | ((src[2]&0x03)<<4);
    dst[3] = src[2]>>2;
}

/************************************************************************
This function converts four six-bit values (24 bits) to three bytes
(24 bits again).

src     pointer to four ints
dst     pointer to three bytes
************************************************************************/
void ae_foursixbits2threebytes(const ae_int_t *src, unsigned char *dst)
{
    dst[0] = (unsigned char)(     src[0] | ((src[1]&0x03)<<6));
    dst[1] = (unsigned char)((src[1]>>2) | ((src[2]&0x0F)<<4));
    dst[2] = (unsigned char)((src[2]>>4) | (src[3]<<2));
}

/************************************************************************
This function serializes boolean value into buffer

v           boolean value to be serialized
buf         buffer, at least 12 characters wide 
            (11 chars for value, one for trailing zero)
state       ALGLIB environment state
************************************************************************/
void ae_bool2str(ae_bool v, char *buf, ae_state *state)
{
    char c = v ? '1' : '0';
    ae_int_t i;
    for(i=0; i<AE_SER_ENTRY_LENGTH; i++)
        buf[i] = c;
    buf[AE_SER_ENTRY_LENGTH] = 0;
}

/************************************************************************
This function unserializes boolean value from buffer

buf         buffer which contains value; leading spaces/tabs/newlines are 
            ignored, traling spaces/tabs/newlines are treated as  end  of
            the boolean value.
state       ALGLIB environment state

This function raises an error in case unexpected symbol is found
************************************************************************/
ae_bool ae_str2bool(const char *buf, ae_state *state, const char **pasttheend)
{
    ae_bool was0, was1;
    const char *emsg = "ALGLIB: unable to read boolean value from stream";
    
    was0 = ae_false;
    was1 = ae_false;
    while( *buf==' ' || *buf=='\t' || *buf=='\n' || *buf=='\r' )
        buf++;
    while( *buf!=' ' && *buf!='\t' && *buf!='\n' && *buf!='\r' && *buf!=0 )
    {
        if( *buf=='0' )
        {
            was0 = ae_true;
            buf++;
            continue;
        }
        if( *buf=='1' )
        {
            was1 = ae_true;
            buf++;
            continue;
        }
        ae_break(state, ERR_ASSERTION_FAILED, emsg);
    }
    *pasttheend = buf;
    if( (!was0) && (!was1) )
        ae_break(state, ERR_ASSERTION_FAILED, emsg);
    if( was0 && was1 )
        ae_break(state, ERR_ASSERTION_FAILED, emsg);
    return was1 ? ae_true : ae_false;
}

/************************************************************************
This function serializes integer value into buffer

v           integer value to be serialized
buf         buffer, at least 12 characters wide 
            (11 chars for value, one for trailing zero)
state       ALGLIB environment state
************************************************************************/
void ae_int2str(ae_int_t v, char *buf, ae_state *state)
{
    union _u
    {
        ae_int_t ival;
        unsigned char bytes[9];
    } u;
    ae_int_t i;
    ae_int_t sixbits[12];
    unsigned char c;
    
    /*
     * copy v to array of chars, sign extending it and 
     * converting to little endian order
     *
     * because we don't want to mention size of ae_int_t explicitly, 
     * we do it as follows:
     * 1. we fill u.bytes by zeros or ones (depending on sign of v)
     * 2. we copy v to u.ival
     * 3. if we run on big endian architecture, we reorder u.bytes
     * 4. now we have signed 64-bit representation of v stored in u.bytes
     * 5. additionally, we set 9th byte of u.bytes to zero in order to
     *    simplify conversion to six-bit representation
     */
    c = v<0 ? (unsigned char)0xFF : (unsigned char)0x00;
    u.ival = v;
    for(i=sizeof(ae_int_t); i<=8; i++) /* <=8 is preferred because it avoids unnecessary compiler warnings*/
        u.bytes[i] = c;
    u.bytes[8] = 0;
    if( state->endianness==AE_BIG_ENDIAN )
    {
        for(i=0; i<(ae_int_t)(sizeof(ae_int_t)/2); i++)
        {
            unsigned char tc;
            tc = u.bytes[i];
            u.bytes[i] = u.bytes[sizeof(ae_int_t)-1-i];
            u.bytes[sizeof(ae_int_t)-1-i] = tc;
        }
    }
    
    /*
     * convert to six-bit representation, output
     *
     * NOTE: last 12th element of sixbits is always zero, we do not output it
     */
    ae_threebytes2foursixbits(u.bytes+0, sixbits+0);
    ae_threebytes2foursixbits(u.bytes+3, sixbits+4);
    ae_threebytes2foursixbits(u.bytes+6, sixbits+8);        
    for(i=0; i<AE_SER_ENTRY_LENGTH; i++)
        buf[i] = ae_sixbits2char(sixbits[i]);
    buf[AE_SER_ENTRY_LENGTH] = 0x00;
}

/************************************************************************
This function unserializes integer value from string

buf         buffer which contains value; leading spaces/tabs/newlines are 
            ignored, traling spaces/tabs/newlines are treated as  end  of
            the boolean value.
state       ALGLIB environment state

This function raises an error in case unexpected symbol is found
************************************************************************/
ae_int_t ae_str2int(const char *buf, ae_state *state, const char **pasttheend)
{
    const char *emsg = "ALGLIB: unable to read integer value from stream";
    ae_int_t sixbits[12];
    ae_int_t sixbitsread, i;
    union _u
    {
        ae_int_t ival;
        unsigned char bytes[9];
    } u;
    /* 
     * 1. skip leading spaces
     * 2. read and decode six-bit digits
     * 3. set trailing digits to zeros
     * 4. convert to little endian 64-bit integer representation
     * 5. convert to big endian representation, if needed
     */
    while( *buf==' ' || *buf=='\t' || *buf=='\n' || *buf=='\r' )
        buf++;
    sixbitsread = 0;
    while( *buf!=' ' && *buf!='\t' && *buf!='\n' && *buf!='\r' && *buf!=0 )
    {
        ae_int_t d;
        d = ae_char2sixbits(*buf);
        if( d<0 || sixbitsread>=AE_SER_ENTRY_LENGTH )
            ae_break(state, ERR_ASSERTION_FAILED, emsg);
        sixbits[sixbitsread] = d;
        sixbitsread++;
        buf++;
    }
    *pasttheend = buf;
    if( sixbitsread==0 )
        ae_break(state, ERR_ASSERTION_FAILED, emsg);
    for(i=sixbitsread; i<12; i++)
        sixbits[i] = 0;
    ae_foursixbits2threebytes(sixbits+0, u.bytes+0);
    ae_foursixbits2threebytes(sixbits+4, u.bytes+3);
    ae_foursixbits2threebytes(sixbits+8, u.bytes+6);
    if( state->endianness==AE_BIG_ENDIAN )
    {
        for(i=0; i<(ae_int_t)(sizeof(ae_int_t)/2); i++)
        {
            unsigned char tc;
            tc = u.bytes[i];
            u.bytes[i] = u.bytes[sizeof(ae_int_t)-1-i];
            u.bytes[sizeof(ae_int_t)-1-i] = tc;
        }
    }
    return u.ival;
}


/************************************************************************
This function serializes double value into buffer

v           double value to be serialized
buf         buffer, at least 12 characters wide 
            (11 chars for value, one for trailing zero)
state       ALGLIB environment state
************************************************************************/
void ae_double2str(double v, char *buf, ae_state *state)
{
    union _u
    {
        double dval;
        unsigned char bytes[9];
    } u;
    ae_int_t i;
    ae_int_t sixbits[12];

    /*
     * handle special quantities
     */
    if( ae_isnan(v, state) )
    {
        const char *s = ".nan_______";
        memcpy(buf, s, strlen(s)+1);
        return;
    }
    if( ae_isposinf(v, state) )
    {
        const char *s = ".posinf____";
        memcpy(buf, s, strlen(s)+1);
        return;
    }
    if( ae_isneginf(v, state) )
    {
        const char *s = ".neginf____";
        memcpy(buf, s, strlen(s)+1);
        return;
    }
    
    /*
     * process general case:
     * 1. copy v to array of chars
     * 2. set 9th byte of u.bytes to zero in order to
     *    simplify conversion to six-bit representation
     * 3. convert to little endian (if needed)
     * 4. convert to six-bit representation
     *    (last 12th element of sixbits is always zero, we do not output it)
     */
    u.dval = v;
    u.bytes[8] = 0;
    if( state->endianness==AE_BIG_ENDIAN )
    {
        for(i=0; i<(ae_int_t)(sizeof(double)/2); i++)
        {
            unsigned char tc;
            tc = u.bytes[i];
            u.bytes[i] = u.bytes[sizeof(double)-1-i];
            u.bytes[sizeof(double)-1-i] = tc;
        }
    }
    ae_threebytes2foursixbits(u.bytes+0, sixbits+0);
    ae_threebytes2foursixbits(u.bytes+3, sixbits+4);
    ae_threebytes2foursixbits(u.bytes+6, sixbits+8);
    for(i=0; i<AE_SER_ENTRY_LENGTH; i++)
        buf[i] = ae_sixbits2char(sixbits[i]);
    buf[AE_SER_ENTRY_LENGTH] = 0x00;
}

/************************************************************************
This function unserializes double value from string

buf         buffer which contains value; leading spaces/tabs/newlines are 
            ignored, traling spaces/tabs/newlines are treated as  end  of
            the boolean value.
state       ALGLIB environment state

This function raises an error in case unexpected symbol is found
************************************************************************/
double ae_str2double(const char *buf, ae_state *state, const char **pasttheend)
{
    const char *emsg = "ALGLIB: unable to read double value from stream";
    ae_int_t sixbits[12];
    ae_int_t sixbitsread, i;
    union _u
    {
        double dval;
        unsigned char bytes[9];
    } u;
    
    
     /* 
      * skip leading spaces
      */
    while( *buf==' ' || *buf=='\t' || *buf=='\n' || *buf=='\r' )
        buf++;
      
    /*
     * Handle special cases
     */
    if( *buf=='.' )
    {
        const char *s_nan =    ".nan_______";
        const char *s_posinf = ".posinf____";
        const char *s_neginf = ".neginf____";
        if( strncmp(buf, s_nan, strlen(s_nan))==0 )
        {
            *pasttheend = buf+strlen(s_nan);
            return state->v_nan;
        }
        if( strncmp(buf, s_posinf, strlen(s_posinf))==0 )
        {
            *pasttheend = buf+strlen(s_posinf);
            return state->v_posinf;
        }
        if( strncmp(buf, s_neginf, strlen(s_neginf))==0 )
        {
            *pasttheend = buf+strlen(s_neginf);
            return state->v_neginf;
        }
        ae_break(state, ERR_ASSERTION_FAILED, emsg);
    }
    
    /* 
     * General case:
     * 1. read and decode six-bit digits
     * 2. check that all 11 digits were read
     * 3. set last 12th digit to zero (needed for simplicity of conversion)
     * 4. convert to 8 bytes
     * 5. convert to big endian representation, if needed
     */
    sixbitsread = 0;
    while( *buf!=' ' && *buf!='\t' && *buf!='\n' && *buf!='\r' && *buf!=0 )
    {
        ae_int_t d;
        d = ae_char2sixbits(*buf);
        if( d<0 || sixbitsread>=AE_SER_ENTRY_LENGTH )
            ae_break(state, ERR_ASSERTION_FAILED, emsg);
        sixbits[sixbitsread] = d;
        sixbitsread++;
        buf++;
    }
    *pasttheend = buf;
    if( sixbitsread!=AE_SER_ENTRY_LENGTH )
        ae_break(state, ERR_ASSERTION_FAILED, emsg);
    sixbits[AE_SER_ENTRY_LENGTH] = 0;
    ae_foursixbits2threebytes(sixbits+0, u.bytes+0);
    ae_foursixbits2threebytes(sixbits+4, u.bytes+3);
    ae_foursixbits2threebytes(sixbits+8, u.bytes+6);
    if( state->endianness==AE_BIG_ENDIAN )
    {
        for(i=0; i<(ae_int_t)(sizeof(double)/2); i++)
        {
            unsigned char tc;
            tc = u.bytes[i];
            u.bytes[i] = u.bytes[sizeof(double)-1-i];
            u.bytes[sizeof(double)-1-i] = tc;
        }
    }
    return u.dval;
}


/************************************************************************
This function performs given number of spin-wait iterations
************************************************************************/
void ae_spin_wait(ae_int_t cnt)
{
    /*
     * these strange operations with ae_never_change_it are necessary to
     * prevent compiler optimization of the loop.
     */
    volatile ae_int_t i;
    
    /* very unlikely because no one will wait for such amount of cycles */
    if( cnt>0x12345678 )
        ae_never_change_it = cnt%10;
    
    /* spin wait, test condition which will never be true */
    for(i=0; i<cnt; i++)
        if( ae_never_change_it>0 )
            ae_never_change_it--;
}


/************************************************************************
This function causes the calling thread to relinquish the CPU. The thread
is moved to the end of the queue and some other thread gets to run.

NOTE: this function should NOT be called when AE_OS is AE_UNKNOWN  -  the
      whole program will be abnormally terminated.
************************************************************************/
void ae_yield()
{
#if AE_OS==AE_WINDOWS
    if( !SwitchToThread() )
        Sleep(0);
#elif AE_OS==AE_POSIX
    sched_yield();
#else
    abort();
#endif
}

/************************************************************************
This function initializes ae_lock structure and sets lock in a free mode.
************************************************************************/
void ae_init_lock(ae_lock *lock)
{
    _lock *p;
    lock->ptr = malloc(sizeof(_lock));
    AE_CRITICAL_ASSERT(lock->ptr!=NULL);
    p = (_lock*)lock->ptr;
#if AE_OS==AE_WINDOWS
    p->p_lock = (ae_int_t*)ae_align((void*)(&p->buf),AE_LOCK_ALIGNMENT);
    p->p_lock[0] = 0;
#elif AE_OS==AE_POSIX
    pthread_mutex_init(&p->mutex, NULL);
#else
    p->is_locked = ae_false;
#endif
}


/************************************************************************
This function acquires lock. In case lock is busy, we perform several
iterations inside tight loop before trying again.
************************************************************************/
void ae_acquire_lock(ae_lock *lock)
{
#if AE_OS==AE_WINDOWS
    ae_int_t cnt = 0;
#endif
    _lock *p;
    p = (_lock*)lock->ptr;
#if AE_OS==AE_WINDOWS
#ifdef AE_SMP_DEBUGCOUNTERS
    InterlockedIncrement((LONG volatile *)&_ae_dbg_lock_acquisitions);
#endif
    for(;;)
    {
		if( InterlockedCompareExchange((LONG volatile *)p->p_lock, 1, 0)==0 )
		    return;
        ae_spin_wait(AE_LOCK_CYCLES);
#ifdef AE_SMP_DEBUGCOUNTERS
        InterlockedIncrement((LONG volatile *)&_ae_dbg_lock_spinwaits);
#endif
        cnt++;
        if( cnt%AE_LOCK_TESTS_BEFORE_YIELD==0 )
        {
#ifdef AE_SMP_DEBUGCOUNTERS
            InterlockedIncrement((LONG volatile *)&_ae_dbg_lock_yields);
#endif
            ae_yield();
        }
    }
#elif AE_OS==AE_POSIX
    ae_int_t cnt = 0;
    for(;;)
    {
		if(  pthread_mutex_trylock(&p->mutex)==0 )
		    return;
        ae_spin_wait(AE_LOCK_CYCLES);
        cnt++;
        if( cnt%AE_LOCK_TESTS_BEFORE_YIELD==0 )
            ae_yield();
    }
   ;
#else
    AE_CRITICAL_ASSERT(!p->is_locked);
    p->is_locked = ae_true;
#endif
}


/************************************************************************
This function releases lock.
************************************************************************/
void ae_release_lock(ae_lock *lock)
{
    _lock *p;
    p = (_lock*)lock->ptr;
#if AE_OS==AE_WINDOWS
    InterlockedExchange((LONG volatile *)p->p_lock, 0);
#elif AE_OS==AE_POSIX
    pthread_mutex_unlock(&p->mutex);
#else
    p->is_locked = ae_false;
#endif
}


/************************************************************************
This function frees ae_lock structure.
************************************************************************/
void ae_free_lock(ae_lock *lock)
{
    _lock *p;
    p = (_lock*)lock->ptr;
#if AE_OS==AE_POSIX
    pthread_mutex_destroy(&p->mutex);
#endif
    free(p);
}


/************************************************************************
This function creates ae_shared_pool structure.

dst                 destination shared pool;
                    already allocated, but not initialized.
state               this parameter can be:
                    * pointer to current instance of ae_state, if you want
                      to automatically destroy this object  after  leaving
                      current frame
                    * NULL,   if  you  do  NOT  want  this  vector  to  be
                      automatically managed (say, if it is field  of  some
                      object)
                      
Error handling:
* on failure calls ae_break() with NULL state pointer. Usually it  results
  in abort() call.

dst is assumed to be uninitialized, its fields are ignored.
************************************************************************/
void ae_shared_pool_init(void *_dst, ae_state *state)
{
    ae_shared_pool *dst;
    
    dst = (ae_shared_pool*)_dst;
    
    /* init */
    dst->seed_object = NULL;
    dst->recycled_objects = NULL;
    dst->recycled_entries = NULL;
    dst->enumeration_counter = NULL;
    dst->size_of_object = 0;
    dst->init = NULL;
    dst->init_copy = NULL;
    dst->destroy = NULL;
    dst->frame_entry.deallocator = ae_shared_pool_destroy;
    dst->frame_entry.ptr = dst;
    if( state!=NULL )
        ae_db_attach(&dst->frame_entry, state);
    ae_init_lock(&dst->pool_lock);
}


/************************************************************************
This function clears all dynamically allocated fields of the pool except
for the lock. It does NOT try to acquire pool_lock.

NOTE: this function is NOT thread-safe, it is not protected by lock.
************************************************************************/
static void ae_shared_pool_internalclear(ae_shared_pool *dst)
{
    ae_shared_pool_entry *ptr, *tmp;
    
    /* destroy seed */
    if( dst->seed_object!=NULL )
    {
        dst->destroy((void*)dst->seed_object);
        ae_free((void*)dst->seed_object);
        dst->seed_object = NULL;
    }
    
    /* destroy recycled objects */
    for(ptr=dst->recycled_objects; ptr!=NULL;)
    {
        tmp = (ae_shared_pool_entry*)ptr->next_entry;
        dst->destroy(ptr->obj);
        ae_free(ptr->obj);
        ae_free(ptr);
        ptr = tmp;
    }
    dst->recycled_objects = NULL;
    
    /* destroy recycled entries */
    for(ptr=dst->recycled_entries; ptr!=NULL;)
    {
        tmp = (ae_shared_pool_entry*)ptr->next_entry;
        ae_free(ptr);
        ptr = tmp;
    }
    dst->recycled_entries = NULL;
}


/************************************************************************
This function creates copy of ae_shared_pool.

dst                 destination pool, allocated but not initialized
src                 source pool
state               this parameter can be:
                    * pointer to current instance of ae_state, if you want
                      to automatically destroy this object  after  leaving
                      current frame
                    * NULL,   if  you  do  NOT  want  this  vector  to  be
                      automatically managed (say, if it is field  of  some
                      object)
                      
Error handling:
* on failure calls ae_break() with NULL state pointer. Usually it  results
  in abort() call.

dst is assumed to be uninitialized, its fields are ignored.

NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
      you should NOT call it when lock can be used by another thread.
************************************************************************/
void ae_shared_pool_init_copy(void *_dst, void *_src, ae_state *state)
{
    ae_shared_pool *dst, *src;
    ae_shared_pool_entry *ptr;
    
    /* state!=NULL, allocation errors result in exception */
    /* AE_CRITICAL_ASSERT(state!=NULL); */
    
    dst = (ae_shared_pool*)_dst;
    src = (ae_shared_pool*)_src;
    ae_shared_pool_init(dst, state);
    
    /* copy non-pointer fields */
    dst->size_of_object = src->size_of_object;
    dst->init = src->init;
    dst->init_copy = src->init_copy;
    dst->destroy = src->destroy;
    ae_init_lock(&dst->pool_lock);    
    
    /* copy seed object */
    if( src->seed_object!=NULL )
    {
        dst->seed_object = ae_malloc(dst->size_of_object, state);
        dst->init_copy(dst->seed_object, src->seed_object, NULL);
    }
    
    /* copy recycled objects */
    dst->recycled_objects = NULL;
    for(ptr=src->recycled_objects; ptr!=NULL; ptr=(ae_shared_pool_entry*)ptr->next_entry)
    {
        ae_shared_pool_entry *tmp;
        tmp = (ae_shared_pool_entry*)ae_malloc(sizeof(ae_shared_pool_entry), state);
        tmp->obj =  ae_malloc(dst->size_of_object, state);
        dst->init_copy(tmp->obj, ptr->obj, NULL);
        tmp->next_entry = dst->recycled_objects;
        dst->recycled_objects = tmp;
    }
    
    /* recycled entries are not copied because they do not store any information */
    dst->recycled_entries = NULL;
    
    /* enumeration counter is reset on copying */
    dst->enumeration_counter = NULL;
    
    /* initialize frame record */
    dst->frame_entry.deallocator = ae_shared_pool_destroy;
    dst->frame_entry.ptr = dst;
}


/************************************************************************
This function clears contents of the pool, but pool remain usable.

IMPORTANT: this function invalidates dst, it can not be used after it  is
           cleared.

NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
      you should NOT call it when lock can be used by another thread.
************************************************************************/
void ae_shared_pool_clear(void *_dst)
{
    ae_shared_pool *dst = (ae_shared_pool*)_dst;
    
    /* clear seed and lists */
    ae_shared_pool_internalclear(dst);
    
    /* clear fields */
    dst->seed_object = NULL;
    dst->recycled_objects = NULL;
    dst->recycled_entries = NULL;
    dst->enumeration_counter = NULL;
    dst->size_of_object = 0;
    dst->init = NULL;
    dst->init_copy = NULL;
    dst->destroy = NULL;
}


/************************************************************************
This function destroys  pool  (object  is  left  in  invalid  state,  all
dynamically allocated memory is freed).

NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
      you should NOT call it when lock can be used by another thread.
************************************************************************/
void ae_shared_pool_destroy(void *_dst)
{
    ae_shared_pool *dst = (ae_shared_pool*)_dst;
    ae_shared_pool_clear(_dst);
    ae_free_lock(&dst->pool_lock);
}


/************************************************************************
This function returns True, if internal seed object was set.  It  returns
False for un-seeded pool.

dst                 destination pool (initialized by constructor function)

NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
      you should NOT call it when lock can be used by another thread.
************************************************************************/
ae_bool ae_shared_pool_is_initialized(void *_dst)
{
    ae_shared_pool *dst = (ae_shared_pool*)_dst;
    return dst->seed_object!=NULL;
}


/************************************************************************
This function sets internal seed object. All objects owned by the pool
(current seed object, recycled objects) are automatically freed.

dst                 destination pool (initialized by constructor function)
seed_object         new seed object
size_of_object      sizeof(), used to allocate memory
init                constructor function
init_copy           copy constructor
clear               destructor function
state               ALGLIB environment state

NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
      you should NOT call it when lock can be used by another thread.
************************************************************************/
void ae_shared_pool_set_seed(
    ae_shared_pool  *dst,
    void            *seed_object,
    ae_int_t        size_of_object,
    void            (*init)(void* dst, ae_state* state),
    void            (*init_copy)(void* dst, void* src, ae_state* state),
    void            (*destroy)(void* ptr),
    ae_state        *state)
{
    /* state!=NULL, allocation errors result in exception */
    AE_CRITICAL_ASSERT(state!=NULL);
    
    /* destroy internal objects */
    ae_shared_pool_internalclear(dst);
    
    /* set non-pointer fields */
    dst->size_of_object = size_of_object;
    dst->init = init;
    dst->init_copy = init_copy;
    dst->destroy = destroy;
    
    /* set seed object */
    dst->seed_object = ae_malloc(size_of_object, state);
    init_copy(dst->seed_object, seed_object, NULL);
}


/************************************************************************
This  function  retrieves  a  copy  of  the seed object from the pool and
stores it to target smart pointer ptr.

In case target pointer owns non-NULL  value,  it  is  deallocated  before
storing value retrieved from pool. Target pointer becomes  owner  of  the
value which was retrieved from pool.

pool                pool
pptr                pointer to ae_smart_ptr structure
state               ALGLIB environment state

NOTE: this function IS thread-safe.  It  acquires  pool  lock  during its
      operation and can be used simultaneously from several threads.
************************************************************************/
void ae_shared_pool_retrieve(
    ae_shared_pool  *pool,
    ae_smart_ptr    *pptr,
    ae_state        *state)
{
    void *new_obj;
    
    /* state!=NULL, allocation errors are handled by throwing exception from ae_malloc() */
    AE_CRITICAL_ASSERT(state!=NULL);
    
    /* assert that pool was seeded */
    ae_assert(
        pool->seed_object!=NULL,
        "ALGLIB: shared pool is not seeded, PoolRetrieve() failed",
        state);
    
    /* acquire lock */
    ae_acquire_lock(&pool->pool_lock);
    
    /* try to reuse recycled objects */
    if( pool->recycled_objects!=NULL )
    {
        void *new_obj;
        ae_shared_pool_entry *result;
        
        /* retrieve entry/object from list of recycled objects */
        result = pool->recycled_objects;
        pool->recycled_objects = (ae_shared_pool_entry*)pool->recycled_objects->next_entry;
        new_obj = result->obj;
        result->obj = NULL;
        
        /* move entry to list of recycled entries */
        result->next_entry = pool->recycled_entries;
        pool->recycled_entries = result;
        
        /* release lock */
        ae_release_lock(&pool->pool_lock);
        
        /* assign object to smart pointer */
        ae_smart_ptr_assign(pptr, new_obj, ae_true, ae_true, pool->destroy);
        return;
    }
        
    /* release lock; we do not need it anymore because copy constructor does not modify source variable */
    ae_release_lock(&pool->pool_lock);
    
    /* create new object from seed */
    new_obj = ae_malloc(pool->size_of_object, state);
    pool->init_copy(new_obj, pool->seed_object, NULL);
        
    /* assign object to smart pointer and return */
    ae_smart_ptr_assign(pptr, new_obj, ae_true, ae_true, pool->destroy);
}


/************************************************************************
This function recycles object owned by smart  pointer  by  moving  it  to
internal storage of the shared pool.

Source pointer must own the object. After function is over, it owns NULL
pointer.

pool                pool
pptr                pointer to ae_smart_ptr structure
state               ALGLIB environment state

NOTE: this function IS thread-safe.  It  acquires  pool  lock  during its
      operation and can be used simultaneously from several threads.
************************************************************************/
void ae_shared_pool_recycle(
    ae_shared_pool  *pool,
    ae_smart_ptr    *pptr,
    ae_state        *state)
{
    ae_shared_pool_entry *new_entry;
    
    /* state!=NULL, allocation errors are handled by throwing exception from ae_malloc() */
    AE_CRITICAL_ASSERT(state!=NULL);
    
    /* assert that pool was seeded */
    ae_assert(
        pool->seed_object!=NULL,
        "ALGLIB: shared pool is not seeded, PoolRecycle() failed",
        state);
    
    /* assert that pointer non-null and owns the object */
    ae_assert(pptr->is_owner,  "ALGLIB: pptr in ae_shared_pool_recycle() does not own its pointer", state);
    ae_assert(pptr->ptr!=NULL, "ALGLIB: pptr in ae_shared_pool_recycle() is NULL", state);
    
    /* acquire lock */
    ae_acquire_lock(&pool->pool_lock);
    
    /* acquire shared pool entry (reuse one from recycled_entries or malloc new one) */
    if( pool->recycled_entries!=NULL )
    {
        /* reuse previously allocated entry */
        new_entry = pool->recycled_entries;
        pool->recycled_entries = (ae_shared_pool_entry*)new_entry->next_entry;
    }
    else
    {
        /*
         * Allocate memory for new entry.
         *
         * NOTE: we release pool lock during allocation because ae_malloc() may raise
         *       exception and we do not want our pool to be left in the locked state.
         */
        ae_release_lock(&pool->pool_lock);
        new_entry =  (ae_shared_pool_entry*)ae_malloc(sizeof(ae_shared_pool_entry), state);
        ae_acquire_lock(&pool->pool_lock);
    }
    
    /* add object to the list of recycled objects */
    new_entry->obj = pptr->ptr;
    new_entry->next_entry = pool->recycled_objects;
    pool->recycled_objects = new_entry;
    
    /* release lock object */
    ae_release_lock(&pool->pool_lock);
    
    /* release source pointer */
    ae_smart_ptr_release(pptr);
}


/************************************************************************
This function clears internal list of  recycled  objects,  but  does  not
change seed object managed by the pool.

pool                pool
state               ALGLIB environment state

NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
      you should NOT call it when lock can be used by another thread.
************************************************************************/
void ae_shared_pool_clear_recycled(
    ae_shared_pool  *pool,
    ae_state        *state)
{
    ae_shared_pool_entry *ptr, *tmp;
    
    /* clear recycled objects */
    for(ptr=pool->recycled_objects; ptr!=NULL;)
    {
        tmp = (ae_shared_pool_entry*)ptr->next_entry;
        pool->destroy(ptr->obj);
        ae_free(ptr->obj);
        ae_free(ptr);
        ptr = tmp;
    }
    pool->recycled_objects = NULL;
}


/************************************************************************
This function allows to enumerate recycled elements of the  shared  pool.
It stores pointer to the first recycled object in the smart pointer.

IMPORTANT:
* in case target pointer owns non-NULL  value,  it  is deallocated before
  storing value retrieved from pool.
* recycled object IS NOT removed from pool
* target pointer DOES NOT become owner of the new value
* this function IS NOT thread-safe
* you SHOULD NOT modify shared pool during enumeration (although you  can
  modify state of the objects retrieved from pool)
* in case there is no recycled objects in the pool, NULL is stored to pptr
* in case pool is not seeded, NULL is stored to pptr

pool                pool
pptr                pointer to ae_smart_ptr structure
state               ALGLIB environment state
************************************************************************/
void ae_shared_pool_first_recycled(
    ae_shared_pool  *pool,
    ae_smart_ptr    *pptr,
    ae_state        *state)
{   
    /* modify internal enumeration counter */
    pool->enumeration_counter = pool->recycled_objects;
    
    /* exit on empty list */
    if( pool->enumeration_counter==NULL )
    {
        ae_smart_ptr_assign(pptr, NULL, ae_false, ae_false, NULL);
        return;
    }
    
    /* assign object to smart pointer */
    ae_smart_ptr_assign(pptr, pool->enumeration_counter->obj, ae_false, ae_false, pool->destroy);
}


/************************************************************************
This function allows to enumerate recycled elements of the  shared  pool.
It stores pointer to the next recycled object in the smart pointer.

IMPORTANT:
* in case target pointer owns non-NULL  value,  it  is deallocated before
  storing value retrieved from pool.
* recycled object IS NOT removed from pool
* target pointer DOES NOT become owner of the new value
* this function IS NOT thread-safe
* you SHOULD NOT modify shared pool during enumeration (although you  can
  modify state of the objects retrieved from pool)
* in case there is no recycled objects left in the pool, NULL is stored.
* in case pool is not seeded, NULL is stored.

pool                pool
pptr                pointer to ae_smart_ptr structure
state               ALGLIB environment state
************************************************************************/
void ae_shared_pool_next_recycled(
    ae_shared_pool  *pool,
    ae_smart_ptr    *pptr,
    ae_state        *state)
{   
    /* exit on end of list */
    if( pool->enumeration_counter==NULL )
    {
        ae_smart_ptr_assign(pptr, NULL, ae_false, ae_false, NULL);
        return;
    }
    
    /* modify internal enumeration counter */
    pool->enumeration_counter = (ae_shared_pool_entry*)pool->enumeration_counter->next_entry;
    
    /* exit on empty list */
    if( pool->enumeration_counter==NULL )
    {
        ae_smart_ptr_assign(pptr, NULL, ae_false, ae_false, NULL);
        return;
    }
    
    /* assign object to smart pointer */
    ae_smart_ptr_assign(pptr, pool->enumeration_counter->obj, ae_false, ae_false, pool->destroy);
}



/************************************************************************
This function clears internal list of recycled objects and  seed  object.
However, pool still can be used (after initialization with another seed).

pool                pool
state               ALGLIB environment state

NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
      you should NOT call it when lock can be used by another thread.
************************************************************************/
void ae_shared_pool_reset(
    ae_shared_pool  *pool,
    ae_state        *state)
{
    /* clear seed and lists */
    ae_shared_pool_internalclear(pool);
    
    /* clear fields */
    pool->seed_object = NULL;
    pool->recycled_objects = NULL;
    pool->recycled_entries = NULL;
    pool->enumeration_counter = NULL;
    pool->size_of_object = 0;
    pool->init = NULL;
    pool->init_copy = NULL;
    pool->destroy = NULL;
}


/************************************************************************
This function initializes serializer
************************************************************************/
void ae_serializer_init(ae_serializer *serializer)
{
    serializer->mode = AE_SM_DEFAULT;
    serializer->entries_needed = 0;
    serializer->bytes_asked = 0;
}

void ae_serializer_clear(ae_serializer *serializer)
{
}

void ae_serializer_alloc_start(ae_serializer *serializer)
{
    serializer->entries_needed = 0;
    serializer->bytes_asked = 0;
    serializer->mode = AE_SM_ALLOC;
}

void ae_serializer_alloc_entry(ae_serializer *serializer)
{
    serializer->entries_needed++;
}

ae_int_t ae_serializer_get_alloc_size(ae_serializer *serializer)
{
    ae_int_t rows, lastrowsize, result;
    
    serializer->mode = AE_SM_READY2S;
    
    /* if no entries needes (degenerate case) */
    if( serializer->entries_needed==0 )
    {
        serializer->bytes_asked = 1;
        return serializer->bytes_asked;
    }
    
    /* non-degenerate case */
    rows = serializer->entries_needed/AE_SER_ENTRIES_PER_ROW;
    lastrowsize = AE_SER_ENTRIES_PER_ROW;
    if( serializer->entries_needed%AE_SER_ENTRIES_PER_ROW )
    {
        lastrowsize = serializer->entries_needed%AE_SER_ENTRIES_PER_ROW;
        rows++;
    }
    
    /* calculate result size */
    result  = ((rows-1)*AE_SER_ENTRIES_PER_ROW+lastrowsize)*AE_SER_ENTRY_LENGTH;
    result +=  (rows-1)*(AE_SER_ENTRIES_PER_ROW-1)+(lastrowsize-1);
    result += rows*2;
    serializer->bytes_asked = result;
    return result;
}

#ifdef AE_USE_CPP_SERIALIZATION
void ae_serializer_sstart_str(ae_serializer *serializer, std::string *buf)
{
    serializer->mode = AE_SM_TO_CPPSTRING;
    serializer->out_cppstr = buf;
    serializer->entries_saved = 0;
    serializer->bytes_written = 0;
}
#endif

#ifdef AE_USE_CPP_SERIALIZATION
void ae_serializer_ustart_str(ae_serializer *serializer, const std::string *buf)
{
    serializer->mode = AE_SM_FROM_STRING;
    serializer->in_str = buf->c_str();
}
#endif

void ae_serializer_sstart_str(ae_serializer *serializer, char *buf)
{
    serializer->mode = AE_SM_TO_STRING;
    serializer->out_str = buf;
    serializer->out_str[0] = 0;
    serializer->entries_saved = 0;
    serializer->bytes_written = 0;
}

void ae_serializer_ustart_str(ae_serializer *serializer, const char *buf)
{
    serializer->mode = AE_SM_FROM_STRING;
    serializer->in_str = buf;
}

void ae_serializer_serialize_bool(ae_serializer *serializer, ae_bool v, ae_state *state)
{
    char buf[AE_SER_ENTRY_LENGTH+2+1];
    const char *emsg = "ALGLIB: serialization integrity error";
    ae_int_t bytes_appended;
    
    /* prepare serialization, check consistency */
    ae_bool2str(v, buf, state);
    serializer->entries_saved++;
    if( serializer->entries_saved%AE_SER_ENTRIES_PER_ROW )
        strcat(buf, " ");
    else
        strcat(buf, "\r\n");
    bytes_appended = (ae_int_t)strlen(buf);
    if( serializer->bytes_written+bytes_appended > serializer->bytes_asked )
        ae_break(state, ERR_ASSERTION_FAILED, emsg);
    serializer->bytes_written += bytes_appended;
        
    /* append to buffer */
#ifdef AE_USE_CPP_SERIALIZATION
    if( serializer->mode==AE_SM_TO_CPPSTRING )
    {
        *(serializer->out_cppstr) += buf;
        return;
    }
#endif
    if( serializer->mode==AE_SM_TO_STRING )
    {
        strcat(serializer->out_str, buf);
        serializer->out_str += bytes_appended;
        return;
    }
    ae_break(state, ERR_ASSERTION_FAILED, emsg);
}

void ae_serializer_serialize_int(ae_serializer *serializer, ae_int_t v, ae_state *state)
{
    char buf[AE_SER_ENTRY_LENGTH+2+1];
    const char *emsg = "ALGLIB: serialization integrity error";
    ae_int_t bytes_appended;
    
    /* prepare serialization, check consistency */
    ae_int2str(v, buf, state);
    serializer->entries_saved++;
    if( serializer->entries_saved%AE_SER_ENTRIES_PER_ROW )
        strcat(buf, " ");
    else
        strcat(buf, "\r\n");
    bytes_appended = (ae_int_t)strlen(buf);
    if( serializer->bytes_written+bytes_appended > serializer->bytes_asked )
        ae_break(state, ERR_ASSERTION_FAILED, emsg);
    serializer->bytes_written += bytes_appended;
        
    /* append to buffer */
#ifdef AE_USE_CPP_SERIALIZATION
    if( serializer->mode==AE_SM_TO_CPPSTRING )
    {
        *(serializer->out_cppstr) += buf;
        return;
    }
#endif
    if( serializer->mode==AE_SM_TO_STRING )
    {
        strcat(serializer->out_str, buf);
        serializer->out_str += bytes_appended;
        return;
    }
    ae_break(state, ERR_ASSERTION_FAILED, emsg);
}

void ae_serializer_serialize_double(ae_serializer *serializer, double v, ae_state *state)
{
    char buf[AE_SER_ENTRY_LENGTH+2+1];
    const char *emsg = "ALGLIB: serialization integrity error";
    ae_int_t bytes_appended;
    
    /* prepare serialization, check consistency */
    ae_double2str(v, buf, state);
    serializer->entries_saved++;
    if( serializer->entries_saved%AE_SER_ENTRIES_PER_ROW )
        strcat(buf, " ");
    else
        strcat(buf, "\r\n");
    bytes_appended = (ae_int_t)strlen(buf);
    if( serializer->bytes_written+bytes_appended > serializer->bytes_asked )
        ae_break(state, ERR_ASSERTION_FAILED, emsg);
    serializer->bytes_written += bytes_appended;
        
    /* append to buffer */
#ifdef AE_USE_CPP_SERIALIZATION
    if( serializer->mode==AE_SM_TO_CPPSTRING )
    {
        *(serializer->out_cppstr) += buf;
        return;
    }
#endif
    if( serializer->mode==AE_SM_TO_STRING )
    {
        strcat(serializer->out_str, buf);
        serializer->out_str += bytes_appended;
        return;
    }
    ae_break(state, ERR_ASSERTION_FAILED, emsg);
}

void ae_serializer_unserialize_bool(ae_serializer *serializer, ae_bool *v, ae_state *state)
{
    *v = ae_str2bool(serializer->in_str, state, &serializer->in_str);
}

void ae_serializer_unserialize_int(ae_serializer *serializer, ae_int_t *v, ae_state *state)
{
    *v = ae_str2int(serializer->in_str, state, &serializer->in_str);
}

void ae_serializer_unserialize_double(ae_serializer *serializer, double *v, ae_state *state)
{
    *v = ae_str2double(serializer->in_str, state, &serializer->in_str);
}

void ae_serializer_stop(ae_serializer *serializer)
{
}


/************************************************************************
Complex math functions
************************************************************************/
ae_complex ae_complex_from_i(ae_int_t v)
{
    ae_complex r;
    r.x = (double)v;
    r.y = 0.0;
    return r;
}

ae_complex ae_complex_from_d(double v)
{
    ae_complex r;
    r.x = v;
    r.y = 0.0;
    return r;
}

ae_complex ae_c_neg(ae_complex lhs)
{
    ae_complex result;
    result.x = -lhs.x;
    result.y = -lhs.y;
    return result;
}

ae_complex ae_c_conj(ae_complex lhs, ae_state *state)
{
    ae_complex result;
    result.x = +lhs.x;
    result.y = -lhs.y;
    return result;
}

ae_complex ae_c_sqr(ae_complex lhs, ae_state *state)
{
    ae_complex result;
    result.x = lhs.x*lhs.x-lhs.y*lhs.y;
    result.y = 2*lhs.x*lhs.y;
    return result;
}

double ae_c_abs(ae_complex z, ae_state *state)
{
    double w;
    double xabs;
    double yabs;
    double v;

    xabs = fabs(z.x);
    yabs = fabs(z.y);
    w = xabs>yabs ? xabs : yabs;
    v = xabs<yabs ? xabs : yabs;
    if( v==0 )
        return w;
    else
    {
        double t = v/w;
        return w*sqrt(1+t*t);
    }
}

ae_bool ae_c_eq(ae_complex lhs,   ae_complex rhs)
{
    volatile double x1 = lhs.x;
    volatile double x2 = rhs.x;
    volatile double y1 = lhs.y;
    volatile double y2 = rhs.y;
    return x1==x2 && y1==y2;
}

ae_bool ae_c_neq(ae_complex lhs,  ae_complex rhs)
{
    volatile double x1 = lhs.x;
    volatile double x2 = rhs.x;
    volatile double y1 = lhs.y;
    volatile double y2 = rhs.y;
    return x1!=x2 || y1!=y2;
}

ae_complex ae_c_add(ae_complex lhs,  ae_complex rhs)
{
    ae_complex result;
    result.x = lhs.x+rhs.x;
    result.y = lhs.y+rhs.y;
    return result;
}

ae_complex ae_c_mul(ae_complex lhs,  ae_complex rhs)
{
    ae_complex result;
    result.x = lhs.x*rhs.x-lhs.y*rhs.y;
    result.y = lhs.x*rhs.y+lhs.y*rhs.x;
    return result;
}

ae_complex ae_c_sub(ae_complex lhs,   ae_complex rhs)
{
    ae_complex result;
    result.x = lhs.x-rhs.x;
    result.y = lhs.y-rhs.y;
    return result;
}

ae_complex ae_c_div(ae_complex lhs,   ae_complex rhs)
{
    ae_complex result;
    double e;
    double f;
    if( fabs(rhs.y)<fabs(rhs.x) )
    {
        e = rhs.y/rhs.x;
        f = rhs.x+rhs.y*e;
        result.x = (lhs.x+lhs.y*e)/f;
        result.y = (lhs.y-lhs.x*e)/f;
    }
    else
    {
        e = rhs.x/rhs.y;
        f = rhs.y+rhs.x*e;
        result.x = (lhs.y+lhs.x*e)/f;
        result.y = (-lhs.x+lhs.y*e)/f;
    }
    return result;
}

ae_bool ae_c_eq_d(ae_complex lhs,  double rhs)
{
    volatile double x1 = lhs.x;
    volatile double x2 = rhs;
    volatile double y1 = lhs.y;
    volatile double y2 = 0;
    return x1==x2 && y1==y2;
}

ae_bool ae_c_neq_d(ae_complex lhs, double rhs)
{
    volatile double x1 = lhs.x;
    volatile double x2 = rhs;
    volatile double y1 = lhs.y;
    volatile double y2 = 0;
    return x1!=x2 || y1!=y2;
}

ae_complex ae_c_add_d(ae_complex lhs, double rhs)
{
    ae_complex result;
    result.x = lhs.x+rhs;
    result.y = lhs.y;
    return result;
}

ae_complex ae_c_mul_d(ae_complex lhs, double rhs)
{
    ae_complex result;
    result.x = lhs.x*rhs;
    result.y = lhs.y*rhs;
    return result;
}

ae_complex ae_c_sub_d(ae_complex lhs, double rhs)
{
    ae_complex result;
    result.x = lhs.x-rhs;
    result.y = lhs.y;
    return result;
}

ae_complex ae_c_d_sub(double lhs,     ae_complex rhs)
{
    ae_complex result;
    result.x = lhs-rhs.x;
    result.y = -rhs.y;
    return result;
}

ae_complex ae_c_div_d(ae_complex lhs, double rhs)
{
    ae_complex result;
    result.x = lhs.x/rhs;
    result.y = lhs.y/rhs;
    return result;
}

ae_complex ae_c_d_div(double lhs,   ae_complex rhs)
{
    ae_complex result;
    double e;
    double f;
    if( fabs(rhs.y)<fabs(rhs.x) )
    {
        e = rhs.y/rhs.x;
        f = rhs.x+rhs.y*e;
        result.x = lhs/f;
        result.y = -lhs*e/f;
    }
    else
    {
        e = rhs.x/rhs.y;
        f = rhs.y+rhs.x*e;
        result.x = lhs*e/f;
        result.y = -lhs/f;
    }
    return result;
}


/************************************************************************
Complex BLAS operations
************************************************************************/
ae_complex ae_v_cdotproduct(const ae_complex *v0, ae_int_t stride0, const char *conj0, const ae_complex *v1, ae_int_t stride1, const char *conj1, ae_int_t n)
{
    double rx = 0, ry = 0; 
    ae_int_t i;
    ae_bool bconj0 = !((conj0[0]=='N') || (conj0[0]=='n'));
    ae_bool bconj1 = !((conj1[0]=='N') || (conj1[0]=='n'));
    ae_complex result;
    if( bconj0 && bconj1 )
    {
        double v0x, v0y, v1x, v1y;
        for(i=0; i<n; i++, v0+=stride0, v1+=stride1)
        {
            v0x = v0->x;
            v0y = -v0->y;
            v1x = v1->x;
            v1y = -v1->y;
            rx += v0x*v1x-v0y*v1y;
            ry += v0x*v1y+v0y*v1x;
        }
    }
    if( !bconj0 && bconj1 )
    {
        double v0x, v0y, v1x, v1y;
        for(i=0; i<n; i++, v0+=stride0, v1+=stride1)
        {
            v0x = v0->x;
            v0y = v0->y;
            v1x = v1->x;
            v1y = -v1->y;
            rx += v0x*v1x-v0y*v1y;
            ry += v0x*v1y+v0y*v1x;
        }
    }
    if( bconj0 && !bconj1 )
    {
        double v0x, v0y, v1x, v1y;
        for(i=0; i<n; i++, v0+=stride0, v1+=stride1)
        {
            v0x = v0->x;
            v0y = -v0->y;
            v1x = v1->x;
            v1y = v1->y;
            rx += v0x*v1x-v0y*v1y;
            ry += v0x*v1y+v0y*v1x;
        }
    }
    if( !bconj0 && !bconj1 )
    {
        double v0x, v0y, v1x, v1y;
        for(i=0; i<n; i++, v0+=stride0, v1+=stride1)
        {
            v0x = v0->x;
            v0y = v0->y;
            v1x = v1->x;
            v1y = v1->y;
            rx += v0x*v1x-v0y*v1y;
            ry += v0x*v1y+v0y*v1x;
        }
    }
    result.x = rx;
    result.y = ry;
    return result;
}

void ae_v_cmove(ae_complex *vdst, ae_int_t stride_dst, const ae_complex* vsrc, ae_int_t stride_src, const char *conj_src, ae_int_t n)
{
    ae_bool bconj = !((conj_src[0]=='N') || (conj_src[0]=='n'));
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x =  vsrc->x;
                vdst->y = -vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
                *vdst = *vsrc;
        }
    }
    else
    {
        /*
         * optimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x =  vsrc->x;
                vdst->y = -vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
                *vdst = *vsrc;
        }
    }
}

void ae_v_cmoveneg(ae_complex *vdst, ae_int_t stride_dst, const ae_complex* vsrc, ae_int_t stride_src, const char *conj_src, ae_int_t n)
{
    ae_bool bconj = !((conj_src[0]=='N') || (conj_src[0]=='n'));
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x = -vsrc->x;
                vdst->y =  vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x = -vsrc->x;
                vdst->y = -vsrc->y;
            }
        }
    }
    else
    {
        /*
         * optimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x = -vsrc->x;
                vdst->y =  vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x = -vsrc->x;
                vdst->y = -vsrc->y;
            }
        }
    }
}

void ae_v_cmoved(ae_complex *vdst, ae_int_t stride_dst, const ae_complex* vsrc, ae_int_t stride_src, const char *conj_src, ae_int_t n, double alpha)
{
    ae_bool bconj = !((conj_src[0]=='N') || (conj_src[0]=='n'));
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x =  alpha*vsrc->x;
                vdst->y = -alpha*vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x = alpha*vsrc->x;
                vdst->y = alpha*vsrc->y;
            }
        }
    }
    else
    {
        /*
         * optimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x =  alpha*vsrc->x;
                vdst->y = -alpha*vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x = alpha*vsrc->x;
                vdst->y = alpha*vsrc->y;
            }
        }
    }
}

void ae_v_cmovec(ae_complex *vdst, ae_int_t stride_dst, const ae_complex* vsrc, ae_int_t stride_src, const char *conj_src, ae_int_t n, ae_complex alpha)
{
    ae_bool bconj = !((conj_src[0]=='N') || (conj_src[0]=='n'));
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        if( bconj )
        {
            double ax = alpha.x, ay = alpha.y;
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x =  ax*vsrc->x+ay*vsrc->y;
                vdst->y = -ax*vsrc->y+ay*vsrc->x;
            }
        }
        else
        {
            double ax = alpha.x, ay = alpha.y;
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x = ax*vsrc->x-ay*vsrc->y;
                vdst->y = ax*vsrc->y+ay*vsrc->x;
            }
        }
    }
    else
    {
        /*
         * highly optimized case
         */
        if( bconj )
        {
            double ax = alpha.x, ay = alpha.y;
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x =  ax*vsrc->x+ay*vsrc->y;
                vdst->y = -ax*vsrc->y+ay*vsrc->x;
            }
        }
        else
        {
            double ax = alpha.x, ay = alpha.y;
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x = ax*vsrc->x-ay*vsrc->y;
                vdst->y = ax*vsrc->y+ay*vsrc->x;
            }
        }
    }
}

void ae_v_cadd(ae_complex *vdst,     ae_int_t stride_dst, const ae_complex *vsrc, ae_int_t stride_src, const char *conj_src, ae_int_t n)
{
    ae_bool bconj = !((conj_src[0]=='N') || (conj_src[0]=='n'));
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x += vsrc->x;
                vdst->y -= vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x += vsrc->x;
                vdst->y += vsrc->y;
            }
        }
    }
    else
    {
        /*
         * optimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x += vsrc->x;
                vdst->y -= vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x += vsrc->x;
                vdst->y += vsrc->y;
            }
        }
    }
}

void ae_v_caddd(ae_complex *vdst,    ae_int_t stride_dst, const ae_complex *vsrc, ae_int_t stride_src, const char *conj_src, ae_int_t n, double alpha)
{
    ae_bool bconj = !((conj_src[0]=='N') || (conj_src[0]=='n'));
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x += alpha*vsrc->x;
                vdst->y -= alpha*vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x += alpha*vsrc->x;
                vdst->y += alpha*vsrc->y;
            }
        }
    }
    else
    {
        /*
         * optimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x += alpha*vsrc->x;
                vdst->y -= alpha*vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x += alpha*vsrc->x;
                vdst->y += alpha*vsrc->y;
            }
        }
    }
}

void ae_v_caddc(ae_complex *vdst,    ae_int_t stride_dst, const ae_complex *vsrc, ae_int_t stride_src, const char *conj_src, ae_int_t n, ae_complex alpha)
{
    ae_bool bconj = !((conj_src[0]=='N') || (conj_src[0]=='n'));
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        double ax = alpha.x, ay = alpha.y;
        if( bconj )
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x += ax*vsrc->x+ay*vsrc->y;
                vdst->y -= ax*vsrc->y-ay*vsrc->x;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x += ax*vsrc->x-ay*vsrc->y;
                vdst->y += ax*vsrc->y+ay*vsrc->x;
            }
        }
    }
    else
    {
        /*
         * highly optimized case
         */
        double ax = alpha.x, ay = alpha.y;
        if( bconj )
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x += ax*vsrc->x+ay*vsrc->y;
                vdst->y -= ax*vsrc->y-ay*vsrc->x;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x += ax*vsrc->x-ay*vsrc->y;
                vdst->y += ax*vsrc->y+ay*vsrc->x;
            }
        }
    }
}

void ae_v_csub(ae_complex *vdst,     ae_int_t stride_dst, const ae_complex *vsrc, ae_int_t stride_src, const char *conj_src, ae_int_t n)
{
    ae_bool bconj = !((conj_src[0]=='N') || (conj_src[0]=='n'));
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x -= vsrc->x;
                vdst->y += vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            {
                vdst->x -= vsrc->x;
                vdst->y -= vsrc->y;
            }
        }
    }
    else
    {
        /*
         * highly optimized case
         */
        if( bconj )
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x -= vsrc->x;
                vdst->y += vsrc->y;
            }
        }
        else
        {
            for(i=0; i<n; i++, vdst++, vsrc++)
            {
                vdst->x -= vsrc->x;
                vdst->y -= vsrc->y;
            }
        }
    }
}

void ae_v_csubd(ae_complex *vdst, ae_int_t stride_dst, const ae_complex *vsrc, ae_int_t stride_src, const char *conj_src, ae_int_t n, double alpha)
{
    ae_v_caddd(vdst, stride_dst, vsrc, stride_src, conj_src, n, -alpha);
}

void ae_v_csubc(ae_complex *vdst, ae_int_t stride_dst, const ae_complex *vsrc, ae_int_t stride_src, const char *conj_src, ae_int_t n, ae_complex alpha)
{
    alpha.x = -alpha.x;
    alpha.y = -alpha.y;
    ae_v_caddc(vdst, stride_dst, vsrc, stride_src, conj_src, n, alpha);
}

void ae_v_cmuld(ae_complex *vdst, ae_int_t stride_dst, ae_int_t n, double alpha)
{
    ae_int_t i;
    if( stride_dst!=1 )
    {
        /*
         * general unoptimized case
         */
        for(i=0; i<n; i++, vdst+=stride_dst)
        {
            vdst->x *= alpha;
            vdst->y *= alpha;
        }
    }
    else
    {
        /*
         * optimized case
         */
        for(i=0; i<n; i++, vdst++)
        {
            vdst->x *= alpha;
            vdst->y *= alpha;
        }
    }
}

void ae_v_cmulc(ae_complex *vdst, ae_int_t stride_dst, ae_int_t n, ae_complex alpha)
{
    ae_int_t i;
    if( stride_dst!=1 )
    {
        /*
         * general unoptimized case
         */
        double ax = alpha.x, ay = alpha.y;
        for(i=0; i<n; i++, vdst+=stride_dst)
        {
            double  dstx = vdst->x, dsty = vdst->y;
            vdst->x = ax*dstx-ay*dsty;
            vdst->y = ax*dsty+ay*dstx;
        }
    }
    else
    {
        /*
         * highly optimized case
         */
        double ax = alpha.x, ay = alpha.y;
        for(i=0; i<n; i++, vdst++)
        {
            double  dstx = vdst->x, dsty = vdst->y;
            vdst->x = ax*dstx-ay*dsty;
            vdst->y = ax*dsty+ay*dstx;
        }
    }
}

/************************************************************************
Real BLAS operations
************************************************************************/
double ae_v_dotproduct(const double *v0, ae_int_t stride0, const double *v1, ae_int_t stride1, ae_int_t n)
{
    double result = 0;
    ae_int_t i;
    if( stride0!=1 || stride1!=1 )
    {
        /*
         * slow general code
         */
        for(i=0; i<n; i++, v0+=stride0, v1+=stride1)
            result += (*v0)*(*v1);
    }
    else
    {
        /*
         * optimized code for stride=1
         */
        ae_int_t n4 = n/4;
        ae_int_t nleft = n%4;
        for(i=0; i<n4; i++, v0+=4, v1+=4)
            result += v0[0]*v1[0]+v0[1]*v1[1]+v0[2]*v1[2]+v0[3]*v1[3];
        for(i=0; i<nleft; i++, v0++, v1++)
            result += v0[0]*v1[0];
    }
    return result;
}

void ae_v_move(double *vdst,  ae_int_t stride_dst, const double* vsrc,  ae_int_t stride_src, ae_int_t n)
{
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            *vdst = *vsrc;
    }
    else
    {
        /*
         * optimized case
         */
        ae_int_t n2 = n/2;
        for(i=0; i<n2; i++, vdst+=2, vsrc+=2)
        {
            vdst[0] = vsrc[0];
            vdst[1] = vsrc[1];
        }
        if( n%2!=0 )
            vdst[0] = vsrc[0];
    }
}

void ae_v_moveneg(double *vdst,  ae_int_t stride_dst, const double* vsrc,  ae_int_t stride_src, ae_int_t n)
{
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            *vdst = -*vsrc;
    }
    else
    {
        /*
         * optimized case
         */
        ae_int_t n2 = n/2;
        for(i=0; i<n2; i++, vdst+=2, vsrc+=2)
        {
            vdst[0] = -vsrc[0];
            vdst[1] = -vsrc[1];
        }
        if( n%2!=0 )
            vdst[0] = -vsrc[0];
    }
}

void ae_v_moved(double *vdst,  ae_int_t stride_dst, const double* vsrc,  ae_int_t stride_src, ae_int_t n, double alpha)
{
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            *vdst = alpha*(*vsrc);
    }
    else
    {
        /*
         * optimized case
         */
        ae_int_t n2 = n/2;
        for(i=0; i<n2; i++, vdst+=2, vsrc+=2)
        {
            vdst[0] = alpha*vsrc[0];
            vdst[1] = alpha*vsrc[1];
        }
        if( n%2!=0 )
            vdst[0] = alpha*vsrc[0];
    }
}

void ae_v_add(double *vdst,     ae_int_t stride_dst, const double *vsrc,  ae_int_t stride_src, ae_int_t n)
{
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            *vdst += *vsrc;
    }
    else
    {
        /*
         * optimized case
         */
        ae_int_t n2 = n/2;
        for(i=0; i<n2; i++, vdst+=2, vsrc+=2)
        {
            vdst[0] += vsrc[0];
            vdst[1] += vsrc[1];
        }
        if( n%2!=0 )
            vdst[0] += vsrc[0];
    }
}

void ae_v_addd(double *vdst,    ae_int_t stride_dst, const double *vsrc,  ae_int_t stride_src, ae_int_t n, double alpha)
{
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            *vdst += alpha*(*vsrc);
    }
    else
    {
        /*
         * optimized case
         */
        ae_int_t n2 = n/2;
        for(i=0; i<n2; i++, vdst+=2, vsrc+=2)
        {
            vdst[0] += alpha*vsrc[0];
            vdst[1] += alpha*vsrc[1];
        }
        if( n%2!=0 )
            vdst[0] += alpha*vsrc[0];
    }
}

void ae_v_sub(double *vdst,     ae_int_t stride_dst, const double *vsrc,  ae_int_t stride_src, ae_int_t n)
{
    ae_int_t i;
    if( stride_dst!=1 || stride_src!=1 )
    {
        /*
         * general unoptimized case
         */
        for(i=0; i<n; i++, vdst+=stride_dst, vsrc+=stride_src)
            *vdst -= *vsrc;
    }
    else
    {
        /*
         * highly optimized case
         */
        ae_int_t n2 = n/2;
        for(i=0; i<n2; i++, vdst+=2, vsrc+=2)
        {
            vdst[0] -= vsrc[0];
            vdst[1] -= vsrc[1];
        }
        if( n%2!=0 )
            vdst[0] -= vsrc[0];
    }
}

void ae_v_subd(double *vdst,  ae_int_t stride_dst, const double *vsrc,  ae_int_t stride_src, ae_int_t n, double alpha)
{
    ae_v_addd(vdst, stride_dst, vsrc, stride_src, n, -alpha);
}

void ae_v_muld(double *vdst,  ae_int_t stride_dst, ae_int_t n, double alpha)
{
    ae_int_t i;
    if( stride_dst!=1 )
    {
        /*
         * general unoptimized case
         */
        for(i=0; i<n; i++, vdst+=stride_dst)
            *vdst *= alpha;
    }
    else
    {
        /*
         * highly optimized case
         */
        for(i=0; i<n; i++, vdst++)
            *vdst *= alpha;
    }
}

/************************************************************************
Other functions
************************************************************************/
ae_int_t ae_v_len(ae_int_t a, ae_int_t b)
{
    return b-a+1;
}

/************************************************************************
RComm functions
************************************************************************/
void _rcommstate_init(rcommstate* p, ae_state *_state)
{
    ae_vector_init(&p->ba, 0, DT_BOOL,    _state);
    ae_vector_init(&p->ia, 0, DT_INT,     _state);
    ae_vector_init(&p->ra, 0, DT_REAL,    _state);
    ae_vector_init(&p->ca, 0, DT_COMPLEX, _state);
}

void _rcommstate_init_copy(rcommstate* dst, rcommstate* src, ae_state *_state)
{
    ae_vector_init_copy(&dst->ba, &src->ba, _state);
    ae_vector_init_copy(&dst->ia, &src->ia, _state);
    ae_vector_init_copy(&dst->ra, &src->ra, _state);
    ae_vector_init_copy(&dst->ca, &src->ca, _state);
    dst->stage = src->stage;
}

void _rcommstate_clear(rcommstate* p)
{
    ae_vector_clear(&p->ba);
    ae_vector_clear(&p->ia);
    ae_vector_clear(&p->ra);
    ae_vector_clear(&p->ca);
}

void _rcommstate_destroy(rcommstate* p)
{
    _rcommstate_clear(p);
}

#ifdef AE_DEBUG4WINDOWS
int _tickcount()
{
    return GetTickCount();
}
#endif

#ifdef AE_DEBUG4POSIX
#include <sys/time.h>
int _tickcount()
{
    struct timeval now;
    ae_int64_t r, v;
    gettimeofday(&now, NULL);
    v = now.tv_sec;
    r = v*1000;
    v = now.tv_usec/1000;
    r = r+v;
    return r;
    /*struct timespec now;
    if (clock_gettime(CLOCK_MONOTONIC, &now) )
        return 0;
    return now.tv_sec * 1000.0 + now.tv_nsec / 1000000.0;*/
}
#endif

/*$ End $*/
