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

#ifndef _snnls_h
#define _snnls_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "reflections.h"
#include "creflections.h"
#include "hqrnd.h"
#include "matgen.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "tsort.h"
#include "sparse.h"
#include "rotations.h"
#include "trfac.h"
#include "hblas.h"
#include "sblas.h"
#include "ortfac.h"
#include "fbls.h"


/*$ Declarations $*/


/*************************************************************************
This structure is a SNNLS (Specialized Non-Negative Least Squares) solver.

It solves problems of the form |A*x-b|^2 => min subject to  non-negativity
constraints on SOME components of x, with structured A (first  NS  columns
are just unit matrix, next ND columns store dense part).

This solver is suited for solution of many sequential NNLS  subproblems  -
it keeps track of previously allocated memory and reuses  it  as  much  as
possible.
*************************************************************************/
typedef struct
{
    ae_int_t ns;
    ae_int_t nd;
    ae_int_t nr;
    ae_matrix densea;
    ae_vector b;
    ae_vector nnc;
    double debugflops;
    ae_int_t debugmaxinnerits;
    ae_vector xn;
    ae_vector xp;
    ae_matrix tmpz;
    ae_matrix tmpca;
    ae_matrix tmplq;
    ae_matrix trda;
    ae_vector trdd;
    ae_vector crb;
    ae_vector g;
    ae_vector d;
    ae_vector dx;
    ae_vector diagaa;
    ae_vector cb;
    ae_vector cx;
    ae_vector cborg;
    ae_vector tmpcholesky;
    ae_vector r;
    ae_vector regdiag;
    ae_vector tmp0;
    ae_vector tmp1;
    ae_vector tmp2;
    ae_vector rdtmprowmap;
} snnlssolver;


/*$ Body $*/


/*************************************************************************
This subroutine is used to initialize SNNLS solver.

By default, empty NNLS problem is produced, but we allocated enough  space
to store problems with NSMax+NDMax columns and  NRMax  rows.  It  is  good
place to provide algorithm with initial estimate of the space requirements,
although you may underestimate problem size or even pass zero estimates  -
in this case buffer variables will be resized automatically  when  you set
NNLS problem.

Previously allocated buffer variables are reused as much as possible. This
function does not clear structure completely, it tries to preserve as much
dynamically allocated memory as possible.

  -- ALGLIB --
     Copyright 10.10.2012 by Bochkanov Sergey
*************************************************************************/
void snnlsinit(ae_int_t nsmax,
     ae_int_t ndmax,
     ae_int_t nrmax,
     snnlssolver* s,
     ae_state *_state);


/*************************************************************************
This subroutine is used to set NNLS problem:

        ( [ 1     |      ]   [   ]   [   ] )^2
        ( [   1   |      ]   [   ]   [   ] )
    min ( [     1 |  Ad  ] * [ x ] - [ b ] )    s.t. x>=0
        ( [       |      ]   [   ]   [   ] )
        ( [       |      ]   [   ]   [   ] )

where:
* identity matrix has NS*NS size (NS<=NR, NS can be zero)
* dense matrix Ad has NR*ND size
* b is NR*1 vector
* x is (NS+ND)*1 vector
* all elements of x are non-negative (this constraint can be removed later
  by calling SNNLSDropNNC() function)

Previously allocated buffer variables are reused as much as possible.
After you set problem, you can solve it with SNNLSSolve().

INPUT PARAMETERS:
    S   -   SNNLS solver, must be initialized with SNNLSInit() call
    A   -   array[NR,ND], dense part of the system
    B   -   array[NR], right part
    NS  -   size of the sparse part of the system, 0<=NS<=NR
    ND  -   size of the dense part of the system, ND>=0
    NR  -   rows count, NR>0

NOTE:
    1. You can have NS+ND=0, solver will correctly accept such combination
       and return empty array as problem solution.
    
  -- ALGLIB --
     Copyright 10.10.2012 by Bochkanov Sergey
*************************************************************************/
void snnlssetproblem(snnlssolver* s,
     /* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* b,
     ae_int_t ns,
     ae_int_t nd,
     ae_int_t nr,
     ae_state *_state);


/*************************************************************************
This subroutine drops non-negativity constraint from the  problem  set  by
SNNLSSetProblem() call. This function must be called AFTER problem is set,
because each SetProblem() call resets constraints to their  default  state
(all constraints are present).

INPUT PARAMETERS:
    S   -   SNNLS solver, must be initialized with SNNLSInit() call,
            problem must be set with SNNLSSetProblem() call.
    Idx -   constraint index, 0<=IDX<NS+ND
    
  -- ALGLIB --
     Copyright 10.10.2012 by Bochkanov Sergey
*************************************************************************/
void snnlsdropnnc(snnlssolver* s, ae_int_t idx, ae_state *_state);


/*************************************************************************
This subroutine is used to solve NNLS problem.

INPUT PARAMETERS:
    S   -   SNNLS solver, must be initialized with SNNLSInit() call and
            problem must be set up with SNNLSSetProblem() call.
    X   -   possibly preallocated buffer, automatically resized if needed

OUTPUT PARAMETERS:
    X   -   array[NS+ND], solution
    
NOTE:
    1. You can have NS+ND=0, solver will correctly accept such combination
       and return empty array as problem solution.
    
    2. Internal field S.DebugFLOPS contains rough estimate of  FLOPs  used
       to solve problem. It can be used for debugging purposes. This field
       is real-valued.
    
  -- ALGLIB --
     Copyright 10.10.2012 by Bochkanov Sergey
*************************************************************************/
void snnlssolve(snnlssolver* s,
     /* Real    */ ae_vector* x,
     ae_state *_state);
void _snnlssolver_init(void* _p, ae_state *_state);
void _snnlssolver_init_copy(void* _dst, void* _src, ae_state *_state);
void _snnlssolver_clear(void* _p);
void _snnlssolver_destroy(void* _p);


/*$ End $*/
#endif

