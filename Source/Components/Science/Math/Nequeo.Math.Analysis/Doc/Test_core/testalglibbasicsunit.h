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

#ifndef _testalglibbasicsunit_h
#define _testalglibbasicsunit_h

#include "aenv.h"
#include "ialglib.h"
#include "alglibbasics.h"


/*$ Declarations $*/


typedef struct
{
    ae_bool bfield;
    double rfield;
    ae_int_t ifield;
    ae_complex cfield;
    ae_vector b1field;
    ae_vector r1field;
    ae_vector i1field;
    ae_vector c1field;
    ae_matrix b2field;
    ae_matrix r2field;
    ae_matrix i2field;
    ae_matrix c2field;
} rec1;


typedef struct
{
    ae_vector b;
    ae_vector i;
    ae_vector r;
} rec4serialization;


typedef struct
{
    ae_complex cval;
    double rval;
    ae_int_t ival;
    ae_bool bval;
    ae_vector i1val;
} poolrec1;


typedef struct
{
    ae_bool bval;
    poolrec1 recval;
    ae_shared_pool pool;
} poolrec2;


typedef struct
{
    ae_int_t val;
} poolsummand;


/*$ Body $*/


void rec4serializationalloc(ae_serializer* s,
     rec4serialization* v,
     ae_state *_state);


void rec4serializationserialize(ae_serializer* s,
     rec4serialization* v,
     ae_state *_state);


void rec4serializationunserialize(ae_serializer* s,
     rec4serialization* v,
     ae_state *_state);


ae_bool testalglibbasics(ae_bool silent, ae_state *_state);
ae_bool _pexec_testalglibbasics(ae_bool silent, ae_state *_state);
void _rec1_init(void* _p, ae_state *_state);
void _rec1_init_copy(void* _dst, void* _src, ae_state *_state);
void _rec1_clear(void* _p);
void _rec1_destroy(void* _p);
void _rec4serialization_init(void* _p, ae_state *_state);
void _rec4serialization_init_copy(void* _dst, void* _src, ae_state *_state);
void _rec4serialization_clear(void* _p);
void _rec4serialization_destroy(void* _p);
void _poolrec1_init(void* _p, ae_state *_state);
void _poolrec1_init_copy(void* _dst, void* _src, ae_state *_state);
void _poolrec1_clear(void* _p);
void _poolrec1_destroy(void* _p);
void _poolrec2_init(void* _p, ae_state *_state);
void _poolrec2_init_copy(void* _dst, void* _src, ae_state *_state);
void _poolrec2_clear(void* _p);
void _poolrec2_destroy(void* _p);
void _poolsummand_init(void* _p, ae_state *_state);
void _poolsummand_init_copy(void* _dst, void* _src, ae_state *_state);
void _poolsummand_clear(void* _p);
void _poolsummand_destroy(void* _p);


/*$ End $*/
#endif

