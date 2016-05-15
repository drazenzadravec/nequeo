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

#ifndef _apserv_h
#define _apserv_h

#include "aenv.h"
#include "ialglib.h"


/*$ Declarations $*/


/*************************************************************************
Buffers for internal functions which need buffers:
* check for size of the buffer you want to use.
* if buffer is too small, resize it; leave unchanged, if it is larger than
  needed.
* use it.

We can pass this structure to multiple functions;  after first run through
functions buffer sizes will be finally determined,  and  on  a next run no
allocation will be required.
*************************************************************************/
typedef struct
{
    ae_vector ba0;
    ae_vector ia0;
    ae_vector ia1;
    ae_vector ia2;
    ae_vector ia3;
    ae_vector ra0;
    ae_vector ra1;
    ae_vector ra2;
    ae_vector ra3;
    ae_matrix rm0;
    ae_matrix rm1;
} apbuffers;


/*************************************************************************
Structure which is used to workaround limitations of ALGLIB parallellization
environment.

  -- ALGLIB --
     Copyright 12.04.2009 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_bool val;
} sboolean;


/*************************************************************************
Structure which is used to workaround limitations of ALGLIB parallellization
environment.

  -- ALGLIB --
     Copyright 12.04.2009 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_vector val;
} sbooleanarray;


/*************************************************************************
Structure which is used to workaround limitations of ALGLIB parallellization
environment.

  -- ALGLIB --
     Copyright 12.04.2009 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_int_t val;
} sinteger;


/*************************************************************************
Structure which is used to workaround limitations of ALGLIB parallellization
environment.

  -- ALGLIB --
     Copyright 12.04.2009 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_vector val;
} sintegerarray;


/*************************************************************************
Structure which is used to workaround limitations of ALGLIB parallellization
environment.

  -- ALGLIB --
     Copyright 12.04.2009 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    double val;
} sreal;


/*************************************************************************
Structure which is used to workaround limitations of ALGLIB parallellization
environment.

  -- ALGLIB --
     Copyright 12.04.2009 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_vector val;
} srealarray;


/*************************************************************************
Structure which is used to workaround limitations of ALGLIB parallellization
environment.

  -- ALGLIB --
     Copyright 12.04.2009 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_complex val;
} scomplex;


/*************************************************************************
Structure which is used to workaround limitations of ALGLIB parallellization
environment.

  -- ALGLIB --
     Copyright 12.04.2009 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_vector val;
} scomplexarray;


/*$ Body $*/


/*************************************************************************
This function is used to set error flags  during  unit  tests.  When  COND
parameter is True, FLAG variable is  set  to  True.  When  COND is  False,
FLAG is unchanged.

The purpose of this function is to have single  point  where  failures  of
unit tests can be detected.

This function returns value of COND.
*************************************************************************/
ae_bool seterrorflag(ae_bool* flag, ae_bool cond, ae_state *_state);


/*************************************************************************
Internally calls SetErrorFlag() with condition:

    Abs(Val-RefVal)>Tol*Max(Abs(RefVal),S)
    
This function is used to test relative error in Val against  RefVal,  with
relative error being replaced by absolute when scale  of  RefVal  is  less
than S.

This function returns value of COND.
*************************************************************************/
ae_bool seterrorflagdiff(ae_bool* flag,
     double val,
     double refval,
     double tol,
     double s,
     ae_state *_state);


/*************************************************************************
The function "touches" integer - it is used  to  avoid  compiler  messages
about unused variables (in rare cases when we do NOT want to remove  these
variables).

  -- ALGLIB --
     Copyright 17.09.2012 by Bochkanov Sergey
*************************************************************************/
void touchint(ae_int_t* a, ae_state *_state);


/*************************************************************************
The function "touches" real   -  it is used  to  avoid  compiler  messages
about unused variables (in rare cases when we do NOT want to remove  these
variables).

  -- ALGLIB --
     Copyright 17.09.2012 by Bochkanov Sergey
*************************************************************************/
void touchreal(double* a, ae_state *_state);


/*************************************************************************
The function performs zero-coalescing on real value.

NOTE: no check is performed for B<>0

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
double coalesce(double a, double b, ae_state *_state);


/*************************************************************************
The function convert integer value to real value.

  -- ALGLIB --
     Copyright 17.09.2012 by Bochkanov Sergey
*************************************************************************/
double inttoreal(ae_int_t a, ae_state *_state);


/*************************************************************************
The function calculates binary logarithm.

NOTE: it costs twice as much as Ln(x)

  -- ALGLIB --
     Copyright 17.09.2012 by Bochkanov Sergey
*************************************************************************/
double logbase2(double x, ae_state *_state);


/*************************************************************************
This function compares two numbers for approximate equality, with tolerance
to errors as large as max(|a|,|b|)*tol.


  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool approxequalrel(double a, double b, double tol, ae_state *_state);


/*************************************************************************
This  function  generates  1-dimensional  general  interpolation task with
moderate Lipshitz constant (close to 1.0)

If N=1 then suborutine generates only one point at the middle of [A,B]

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
void taskgenint1d(double a,
     double b,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state);


/*************************************************************************
This function generates  1-dimensional equidistant interpolation task with
moderate Lipshitz constant (close to 1.0)

If N=1 then suborutine generates only one point at the middle of [A,B]

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
void taskgenint1dequidist(double a,
     double b,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state);


/*************************************************************************
This function generates  1-dimensional Chebyshev-1 interpolation task with
moderate Lipshitz constant (close to 1.0)

If N=1 then suborutine generates only one point at the middle of [A,B]

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
void taskgenint1dcheb1(double a,
     double b,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state);


/*************************************************************************
This function generates  1-dimensional Chebyshev-2 interpolation task with
moderate Lipshitz constant (close to 1.0)

If N=1 then suborutine generates only one point at the middle of [A,B]

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
void taskgenint1dcheb2(double a,
     double b,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state);


/*************************************************************************
This function checks that all values from X[] are distinct. It does more
than just usual floating point comparison:
* first, it calculates max(X) and min(X)
* second, it maps X[] from [min,max] to [1,2]
* only at this stage actual comparison is done

The meaning of such check is to ensure that all values are "distinct enough"
and will not cause interpolation subroutine to fail.

NOTE:
    X[] must be sorted by ascending (subroutine ASSERT's it)

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool aredistinct(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
This function checks that two boolean values are the same (both  are  True 
or both are False).

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool aresameboolean(ae_bool v1, ae_bool v2, ae_state *_state);


/*************************************************************************
If Length(X)<N, resizes X

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void bvectorsetlengthatleast(/* Boolean */ ae_vector* x,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
If Length(X)<N, resizes X

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void ivectorsetlengthatleast(/* Integer */ ae_vector* x,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
If Length(X)<N, resizes X

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void rvectorsetlengthatleast(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
If Cols(X)<N or Rows(X)<M, resizes X

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void rmatrixsetlengthatleast(/* Real    */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Resizes X and:
* preserves old contents of X
* fills new elements by zeros

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void rmatrixresize(/* Real    */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Resizes X and:
* preserves old contents of X
* fills new elements by zeros

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void imatrixresize(/* Integer */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
This function checks that length(X) is at least N and first N values  from
X[] are finite

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool isfinitevector(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
This function checks that first N values from X[] are finite

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool isfinitecvector(/* Complex */ ae_vector* z,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
This function checks that size of X is at least MxN and values from
X[0..M-1,0..N-1] are finite.

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool apservisfinitematrix(/* Real    */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
This function checks that all values from X[0..M-1,0..N-1] are finite

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool apservisfinitecmatrix(/* Complex */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
This function checks that size of X is at least NxN and all values from
upper/lower triangle of X[0..N-1,0..N-1] are finite

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool isfinitertrmatrix(/* Real    */ ae_matrix* x,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state);


/*************************************************************************
This function checks that all values from upper/lower triangle of
X[0..N-1,0..N-1] are finite

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool apservisfinitectrmatrix(/* Complex */ ae_matrix* x,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state);


/*************************************************************************
This function checks that all values from X[0..M-1,0..N-1] are  finite  or
NaN's.

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool apservisfiniteornanmatrix(/* Real    */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Safe sqrt(x^2+y^2)

  -- ALGLIB --
     Copyright by Bochkanov Sergey
*************************************************************************/
double safepythag2(double x, double y, ae_state *_state);


/*************************************************************************
Safe sqrt(x^2+y^2)

  -- ALGLIB --
     Copyright by Bochkanov Sergey
*************************************************************************/
double safepythag3(double x, double y, double z, ae_state *_state);


/*************************************************************************
Safe division.

This function attempts to calculate R=X/Y without overflow.

It returns:
* +1, if abs(X/Y)>=MaxRealNumber or undefined - overflow-like situation
      (no overlfow is generated, R is either NAN, PosINF, NegINF)
*  0, if MinRealNumber<abs(X/Y)<MaxRealNumber or X=0, Y<>0
      (R contains result, may be zero)
* -1, if 0<abs(X/Y)<MinRealNumber - underflow-like situation
      (R contains zero; it corresponds to underflow)

No overflow is generated in any case.

  -- ALGLIB --
     Copyright by Bochkanov Sergey
*************************************************************************/
ae_int_t saferdiv(double x, double y, double* r, ae_state *_state);


/*************************************************************************
This function calculates "safe" min(X/Y,V) for positive finite X, Y, V.
No overflow is generated in any case.

  -- ALGLIB --
     Copyright by Bochkanov Sergey
*************************************************************************/
double safeminposrv(double x, double y, double v, ae_state *_state);


/*************************************************************************
This function makes periodic mapping of X to [A,B].

It accepts X, A, B (A>B). It returns T which lies in  [A,B] and integer K,
such that X = T + K*(B-A).

NOTES:
* K is represented as real value, although actually it is integer
* T is guaranteed to be in [A,B]
* T replaces X

  -- ALGLIB --
     Copyright by Bochkanov Sergey
*************************************************************************/
void apperiodicmap(double* x,
     double a,
     double b,
     double* k,
     ae_state *_state);


/*************************************************************************
Returns random normal number using low-quality system-provided generator

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
double randomnormal(ae_state *_state);


/*************************************************************************
Generates random unit vector using low-quality system-provided generator.
Reallocates array if its size is too short.

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void randomunit(ae_int_t n, /* Real    */ ae_vector* x, ae_state *_state);


/*************************************************************************
This function is used to swap two integer values
*************************************************************************/
void swapi(ae_int_t* v0, ae_int_t* v1, ae_state *_state);


/*************************************************************************
This function is used to swap two real values
*************************************************************************/
void swapr(double* v0, double* v1, ae_state *_state);


/*************************************************************************
This function is used to return maximum of three real values
*************************************************************************/
double maxreal3(double v0, double v1, double v2, ae_state *_state);


/*************************************************************************
This function is used to increment value of integer variable
*************************************************************************/
void inc(ae_int_t* v, ae_state *_state);


/*************************************************************************
This function is used to decrement value of integer variable
*************************************************************************/
void dec(ae_int_t* v, ae_state *_state);


/*************************************************************************
This function performs two operations:
1. decrements value of integer variable, if it is positive
2. explicitly sets variable to zero if it is non-positive
It is used by some algorithms to decrease value of internal counters.
*************************************************************************/
void countdown(ae_int_t* v, ae_state *_state);


/*************************************************************************
'bounds' value: maps X to [B1,B2]

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
double boundval(double x, double b1, double b2, ae_state *_state);


/*************************************************************************
Allocation of serializer: complex value
*************************************************************************/
void alloccomplex(ae_serializer* s, ae_complex v, ae_state *_state);


/*************************************************************************
Serialization: complex value
*************************************************************************/
void serializecomplex(ae_serializer* s, ae_complex v, ae_state *_state);


/*************************************************************************
Unserialization: complex value
*************************************************************************/
ae_complex unserializecomplex(ae_serializer* s, ae_state *_state);


/*************************************************************************
Allocation of serializer: real array
*************************************************************************/
void allocrealarray(ae_serializer* s,
     /* Real    */ ae_vector* v,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Serialization: complex value
*************************************************************************/
void serializerealarray(ae_serializer* s,
     /* Real    */ ae_vector* v,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Unserialization: complex value
*************************************************************************/
void unserializerealarray(ae_serializer* s,
     /* Real    */ ae_vector* v,
     ae_state *_state);


/*************************************************************************
Allocation of serializer: Integer array
*************************************************************************/
void allocintegerarray(ae_serializer* s,
     /* Integer */ ae_vector* v,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Serialization: Integer array
*************************************************************************/
void serializeintegerarray(ae_serializer* s,
     /* Integer */ ae_vector* v,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Unserialization: complex value
*************************************************************************/
void unserializeintegerarray(ae_serializer* s,
     /* Integer */ ae_vector* v,
     ae_state *_state);


/*************************************************************************
Allocation of serializer: real matrix
*************************************************************************/
void allocrealmatrix(ae_serializer* s,
     /* Real    */ ae_matrix* v,
     ae_int_t n0,
     ae_int_t n1,
     ae_state *_state);


/*************************************************************************
Serialization: complex value
*************************************************************************/
void serializerealmatrix(ae_serializer* s,
     /* Real    */ ae_matrix* v,
     ae_int_t n0,
     ae_int_t n1,
     ae_state *_state);


/*************************************************************************
Unserialization: complex value
*************************************************************************/
void unserializerealmatrix(ae_serializer* s,
     /* Real    */ ae_matrix* v,
     ae_state *_state);


/*************************************************************************
Copy integer array
*************************************************************************/
void copyintegerarray(/* Integer */ ae_vector* src,
     /* Integer */ ae_vector* dst,
     ae_state *_state);


/*************************************************************************
Copy real array
*************************************************************************/
void copyrealarray(/* Real    */ ae_vector* src,
     /* Real    */ ae_vector* dst,
     ae_state *_state);


/*************************************************************************
Copy real matrix
*************************************************************************/
void copyrealmatrix(/* Real    */ ae_matrix* src,
     /* Real    */ ae_matrix* dst,
     ae_state *_state);


/*************************************************************************
This function searches integer array. Elements in this array are actually
records, each NRec elements wide. Each record has unique header - NHeader
integer values, which identify it. Records are lexicographically sorted by
header.

Records are identified by their index, not offset (offset = NRec*index).

This function searches A (records with indices [I0,I1)) for a record with
header B. It returns index of this record (not offset!), or -1 on failure.

  -- ALGLIB --
     Copyright 28.03.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t recsearch(/* Integer */ ae_vector* a,
     ae_int_t nrec,
     ae_int_t nheader,
     ae_int_t i0,
     ae_int_t i1,
     /* Integer */ ae_vector* b,
     ae_state *_state);


/*************************************************************************
This function is used in parallel functions for recurrent division of large
task into two smaller tasks.

It has following properties:
* it works only for TaskSize>=2 (assertion is thrown otherwise)
* for TaskSize=2, it returns Task0=1, Task1=1
* in case TaskSize is odd,  Task0=TaskSize-1, Task1=1
* in case TaskSize is even, Task0 and Task1 are approximately TaskSize/2
  and both Task0 and Task1 are even, Task0>=Task1

  -- ALGLIB --
     Copyright 07.04.2013 by Bochkanov Sergey
*************************************************************************/
void splitlengtheven(ae_int_t tasksize,
     ae_int_t* task0,
     ae_int_t* task1,
     ae_state *_state);


/*************************************************************************
This function is used in parallel functions for recurrent division of large
task into two smaller tasks.

It has following properties:
* it works only for TaskSize>=2 and ChunkSize>=2
  (assertion is thrown otherwise)
* Task0+Task1=TaskSize, Task0>0, Task1>0
* Task0 and Task1 are close to each other
* in case TaskSize>ChunkSize, Task0 is always divisible by ChunkSize

  -- ALGLIB --
     Copyright 07.04.2013 by Bochkanov Sergey
*************************************************************************/
void splitlength(ae_int_t tasksize,
     ae_int_t chunksize,
     ae_int_t* task0,
     ae_int_t* task1,
     ae_state *_state);


/*************************************************************************
This function is used to calculate number of chunks (including partial,
non-complete chunks) in some set. It expects that ChunkSize>=1, TaskSize>=0.
Assertion is thrown otherwise.

Function result is equivalent to Ceil(TaskSize/ChunkSize), but with guarantees
that rounding errors won't ruin results.

  -- ALGLIB --
     Copyright 21.01.2015 by Bochkanov Sergey
*************************************************************************/
ae_int_t chunkscount(ae_int_t tasksize,
     ae_int_t chunksize,
     ae_state *_state);
void _apbuffers_init(void* _p, ae_state *_state);
void _apbuffers_init_copy(void* _dst, void* _src, ae_state *_state);
void _apbuffers_clear(void* _p);
void _apbuffers_destroy(void* _p);
void _sboolean_init(void* _p, ae_state *_state);
void _sboolean_init_copy(void* _dst, void* _src, ae_state *_state);
void _sboolean_clear(void* _p);
void _sboolean_destroy(void* _p);
void _sbooleanarray_init(void* _p, ae_state *_state);
void _sbooleanarray_init_copy(void* _dst, void* _src, ae_state *_state);
void _sbooleanarray_clear(void* _p);
void _sbooleanarray_destroy(void* _p);
void _sinteger_init(void* _p, ae_state *_state);
void _sinteger_init_copy(void* _dst, void* _src, ae_state *_state);
void _sinteger_clear(void* _p);
void _sinteger_destroy(void* _p);
void _sintegerarray_init(void* _p, ae_state *_state);
void _sintegerarray_init_copy(void* _dst, void* _src, ae_state *_state);
void _sintegerarray_clear(void* _p);
void _sintegerarray_destroy(void* _p);
void _sreal_init(void* _p, ae_state *_state);
void _sreal_init_copy(void* _dst, void* _src, ae_state *_state);
void _sreal_clear(void* _p);
void _sreal_destroy(void* _p);
void _srealarray_init(void* _p, ae_state *_state);
void _srealarray_init_copy(void* _dst, void* _src, ae_state *_state);
void _srealarray_clear(void* _p);
void _srealarray_destroy(void* _p);
void _scomplex_init(void* _p, ae_state *_state);
void _scomplex_init_copy(void* _dst, void* _src, ae_state *_state);
void _scomplex_clear(void* _p);
void _scomplex_destroy(void* _p);
void _scomplexarray_init(void* _p, ae_state *_state);
void _scomplexarray_init_copy(void* _dst, void* _src, ae_state *_state);
void _scomplexarray_clear(void* _p);
void _scomplexarray_destroy(void* _p);


/*$ End $*/
#endif

