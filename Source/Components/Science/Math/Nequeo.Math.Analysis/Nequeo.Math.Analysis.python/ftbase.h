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

#ifndef _ftbase_h
#define _ftbase_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "ntheory.h"


/*$ Declarations $*/


/*************************************************************************
This record stores execution plan for the fast transformation  along  with
preallocated temporary buffers and precalculated values.

FIELDS:
    Entries         -   plan entries, one row = one entry (see below for
                        description).
    Buf0,Buf1,Buf2  -   global temporary buffers; some of them are allocated,
                        some of them are not (as decided by plan generation
                        subroutine).
    Buffer          -   global buffer whose size is equal to plan size.
                        There is one-to-one correspondence between elements
                        of global buffer and elements of array transformed.
                        Because of it global buffer can be used as temporary
                        thread-safe storage WITHOUT ACQUIRING LOCK - each
                        worker thread works with its part of input array,
                        and each part of input array corresponds to distinct
                        part of buffer.
    
FORMAT OF THE ENTRIES TABLE:

Entries table is 2D array which stores one entry per row. Row format is:
    row[0]      operation type:
                *  0 for "end of plan/subplan"
                * +1 for "reference O(N^2) complex FFT"
                * -1 for complex transposition
                * -2 for multiplication by twiddle factors of complex FFT
                * -3 for "start of plan/subplan"
    row[1]      repetition count, >=1
    row[2]      base operand size (number of microvectors), >=1
    row[3]      microvector size (measured in real numbers), >=1
    row[4]      parameter0, meaning depends on row[0]
    row[5]      parameter1, meaning depends on row[0]

FORMAT OF THE DATA:

Transformation plan works with row[1]*row[2]*row[3]  real  numbers,  which
are (in most cases) interpreted as sequence of complex numbers. These data
are grouped as follows:
* we have row[1] contiguous OPERANDS, which can be treated separately
* each operand includes row[2] contiguous MICROVECTORS
* each microvector includes row[3] COMPONENTS, which can be treated separately
* pair of components form complex number, so in most cases row[3] will be even

Say, if you want to perform complex FFT of length 3, then:
* you have 1 operand: row[1]=1
* operand consists of 3 microvectors:   row[2]=3
* each microvector has two components:  row[3]=2
* a pair of subsequent components is treated as complex number

if you want to perform TWO simultaneous complex FFT's of length 3, then you
can choose between two representations:
* 1 operand, 3 microvectors, 4 components; storage format is given below:
  [ A0X A0Y B0X B0Y A1X A1Y B1X B1Y ... ]
  (here A denotes first sequence, B - second one).
* 2 operands, 3 microvectors, 2 components; storage format is given below:
  [ A0X A0Y A1X A2Y ... B0X B0Y B1X B1Y ... ]
Most FFT operations are supported only for the second format, but you
should remember that first format sometimes can be used too.

SUPPORTED OPERATIONS:

row[0]=0:
* "end of plan/subplan"
* in case we meet entry with such type,  FFT  transformation  is  finished
  (or we return from recursive FFT subplan, in case it was subplan).

row[0]=+1:
* "reference 1D complex FFT"
* we perform reference O(N^2) complex FFT on input data, which are treated
  as row[1] arrays, each of row[2] complex numbers, and row[3] must be
  equal to 2
* transformation is performed using temporary buffer

row[0]=opBluesteinsFFT:
* input array is handled with Bluestein's algorithm (by zero-padding to
  Param0 complex numbers).
* this plan calls Param0-point subplan which is located at offset Param1
  (offset is measured with respect to location of the calling entry)
* this plan uses precomputed quantities stored in Plan.PrecR at
  offset Param2.
* transformation is performed using 4 temporary buffers, which are
  retrieved from Plan.BluesteinPool.

row[0]=+3:
* "optimized 1D complex FFT"
* this function supports only several operand sizes: from 1 to 5.
  These transforms are hard-coded and performed very efficiently

row[0]=opRadersFFT:
* input array is handled with Rader's algorithm (permutation and
  reduction to N-1-point FFT)
* this plan calls N-1-point subplan which is located at offset Param0
  (offset is measured with respect to location of the calling entry)
* this plan uses precomputed primitive root and its inverse (modulo N)
  which are stored in Param1 and Param2.
* Param3 stores offset of the precomputed data for the plan
* plan length must be prime, (N-1)*(N-1) must fit into integer variable

row[0]=-1
* "complex transposition"
* input data are treated as row[1] independent arrays, which are processed
  separately
* each of operands is treated as matrix with row[4] rows and row[2]/row[4]
  columns. Each element of the matrix is microvector with row[3] components.
* transposition is performed using temporary buffer

row[0]=-2
* "multiplication by twiddle factors of complex FFT"
* input data are treated as row[1] independent arrays, which are processed
  separately
* row[4] contains N1 - length of the "first FFT"  in  a  Cooley-Tukey  FFT
  algorithm
* this function does not require temporary buffers

row[0]=-3
* "start of the plan"
* each subplan must start from this entry
* param0 is ignored
* param1 stores approximate (optimistic) estimate of KFLOPs required to
  transform one operand of the plan. Total cost of the plan is approximately
  equal to row[1]*param1 KFLOPs.
* this function does not require temporary buffers

row[0]=-4
* "jump"
* param0 stores relative offset of the jump site
  (+1 corresponds to the next entry)

row[0]=-5
* "parallel call"
* input data are treated as row[1] independent arrays
* child subplan is applied independently for each of arrays - row[1] times
* subplan length must be equal to row[2]*row[3]
* param0 stores relative offset of the child subplan site
  (+1 corresponds to the next entry)
* param1 stores approximate total cost of plan, measured in UNITS
  (1 UNIT = 100 KFLOPs). Plan cost must be rounded DOWN to nearest integer.


    
TODO 
     2. from KFLOPs to UNITs, 1 UNIT = 100 000 FLOP!!!!!!!!!!!

     3. from IsRoot to TaskType = {0, -1, +1}; or maybe, add IsSeparatePlan
        to distinguish root of child subplan from global root which uses
        separate buffer
        
     4. child subplans in parallel call must NOT use buffer provided by parent plan;
        they must allocate their own local buffer
*************************************************************************/
typedef struct
{
    ae_matrix entries;
    ae_vector buffer;
    ae_vector precr;
    ae_vector preci;
    ae_shared_pool bluesteinpool;
} fasttransformplan;


/*$ Body $*/


/*************************************************************************
This subroutine generates FFT plan for K complex FFT's with length N each.

INPUT PARAMETERS:
    N           -   FFT length (in complex numbers), N>=1
    K           -   number of repetitions, K>=1
    
OUTPUT PARAMETERS:
    Plan        -   plan

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
void ftcomplexfftplan(ae_int_t n,
     ae_int_t k,
     fasttransformplan* plan,
     ae_state *_state);


/*************************************************************************
This subroutine applies transformation plan to input/output array A.

INPUT PARAMETERS:
    Plan        -   transformation plan
    A           -   array, must be large enough for plan to work
    OffsA       -   offset of the subarray to process
    RepCnt      -   repetition count (transformation is repeatedly applied
                    to subsequent subarrays)
    
OUTPUT PARAMETERS:
    Plan        -   plan (temporary buffers can be modified, plan itself
                    is unchanged and can be reused)
    A           -   transformed array

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
void ftapplyplan(fasttransformplan* plan,
     /* Real    */ ae_vector* a,
     ae_int_t offsa,
     ae_int_t repcnt,
     ae_state *_state);


/*************************************************************************
Returns good factorization N=N1*N2.

Usually N1<=N2 (but not always - small N's may be exception).
if N1<>1 then N2<>1.

Factorization is chosen depending on task type and codelets we have.

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
void ftbasefactorize(ae_int_t n,
     ae_int_t tasktype,
     ae_int_t* n1,
     ae_int_t* n2,
     ae_state *_state);


/*************************************************************************
Is number smooth?

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool ftbaseissmooth(ae_int_t n, ae_state *_state);


/*************************************************************************
Returns smallest smooth (divisible only by 2, 3, 5) number that is greater
than or equal to max(N,2)

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
ae_int_t ftbasefindsmooth(ae_int_t n, ae_state *_state);


/*************************************************************************
Returns  smallest  smooth  (divisible only by 2, 3, 5) even number that is
greater than or equal to max(N,2)

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
ae_int_t ftbasefindsmootheven(ae_int_t n, ae_state *_state);


/*************************************************************************
Returns estimate of FLOP count for the FFT.

It is only an estimate based on operations count for the PERFECT FFT
and relative inefficiency of the algorithm actually used.

N should be power of 2, estimates are badly wrong for non-power-of-2 N's.

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
double ftbasegetflopestimate(ae_int_t n, ae_state *_state);
void _fasttransformplan_init(void* _p, ae_state *_state);
void _fasttransformplan_init_copy(void* _dst, void* _src, ae_state *_state);
void _fasttransformplan_clear(void* _p);
void _fasttransformplan_destroy(void* _p);


/*$ End $*/
#endif

