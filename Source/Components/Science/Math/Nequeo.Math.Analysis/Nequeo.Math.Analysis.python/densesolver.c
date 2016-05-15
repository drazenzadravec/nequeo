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
#include "densesolver.h"


/*$ Declarations $*/
static void densesolver_rmatrixlusolveinternal(/* Real    */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_bool havea,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* x,
     ae_state *_state);
static void densesolver_spdmatrixcholeskysolveinternal(/* Real    */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_matrix* a,
     ae_bool havea,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* x,
     ae_state *_state);
static void densesolver_cmatrixlusolveinternal(/* Complex */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_bool havea,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* x,
     ae_state *_state);
static void densesolver_hpdmatrixcholeskysolveinternal(/* Complex */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_matrix* a,
     ae_bool havea,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* x,
     ae_state *_state);
static ae_int_t densesolver_densesolverrfsmax(ae_int_t n,
     double r1,
     double rinf,
     ae_state *_state);
static ae_int_t densesolver_densesolverrfsmaxv2(ae_int_t n,
     double r2,
     ae_state *_state);
static void densesolver_rbasiclusolve(/* Real    */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Real    */ ae_vector* xb,
     ae_state *_state);
static void densesolver_spdbasiccholeskysolve(/* Real    */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* xb,
     ae_state *_state);
static void densesolver_cbasiclusolve(/* Complex */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Complex */ ae_vector* xb,
     ae_state *_state);
static void densesolver_hpdbasiccholeskysolve(/* Complex */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* xb,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Dense solver for A*x=b with N*N real matrix A and N*1 real vectorx  x  and
b. This is "slow-but-feature rich" version of the  linear  solver.  Faster
version is RMatrixSolveFast() function.

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* iterative refinement
* O(N^3) complexity

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear  system
           ! and  performs  iterative   refinement,   which   results   in
           ! significant performance penalty  when  compared  with  "fast"
           ! version  which  just  performs  LU  decomposition  and  calls
           ! triangular solver.
           !
           ! This  performance  penalty  is  especially  visible  in   the
           ! multithreaded mode, because both condition number  estimation
           ! and   iterative    refinement   are   inherently   sequential
           ! calculations. It also very significant on small matrices.
           !
           ! Thus, if you need high performance and if you are pretty sure
           ! that your system is well conditioned, we  strongly  recommend
           ! you to use faster solver, RMatrixSolveFast() function.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that LU decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=1024,  achieving
  ! near-linear speedup for N=4096 or higher.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or exactly singular.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void rmatrixsolve(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_vector* b,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix bm;
    ae_matrix xm;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_vector_clear(x);
    ae_matrix_init(&bm, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xm, 0, 0, DT_REAL, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&bm, n, 1, _state);
    ae_v_move(&bm.ptr.pp_double[0][0], bm.stride, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
    rmatrixsolvem(a, n, &bm, 1, ae_true, info, rep, &xm, _state);
    ae_vector_set_length(x, n, _state);
    ae_v_move(&x->ptr.p_double[0], 1, &xm.ptr.pp_double[0][0], xm.stride, ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixsolve(/* Real    */ ae_matrix* a,
    ae_int_t n,
    /* Real    */ ae_vector* b,
    ae_int_t* info,
    densesolverreport* rep,
    /* Real    */ ae_vector* x, ae_state *_state)
{
    rmatrixsolve(a,n,b,info,rep,x, _state);
}


/*************************************************************************
Dense solver.

This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
real matrix, x  and  b  are  vectors.  This is a "fast" version of  linear
solver which does NOT provide  any  additional  functions  like  condition
number estimation or iterative refinement.

Algorithm features:
* efficient algorithm O(N^3) complexity
* no performance overhead from additional functionality

If you need condition number estimation or iterative refinement, use  more
feature-rich version - RMatrixSolve().

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that LU decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=1024,  achieving
  ! near-linear speedup for N=4096 or higher.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is exactly singular (ill conditioned matrices
                        are not recognized).
                * -1    N<=0 was passed
                *  1    task is solved 
    B       -   array[N]:
                * info>0    =>  overwritten by solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 16.03.2015 by Bochkanov Sergey
*************************************************************************/
void rmatrixsolvefast(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_vector* b,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;
    ae_int_t j;
    ae_vector p;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    *info = 0;
    ae_vector_init(&p, 0, DT_INT, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    rmatrixlu(a, n, n, &p, _state);
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_eq(a->ptr.pp_double[i][i],(double)(0)) )
        {
            for(j=0; j<=n-1; j++)
            {
                b->ptr.p_double[j] = 0.0;
            }
            *info = -3;
            ae_frame_leave(_state);
            return;
        }
    }
    densesolver_rbasiclusolve(a, &p, n, b, _state);
    *info = 1;
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixsolvefast(/* Real    */ ae_matrix* a,
    ae_int_t n,
    /* Real    */ ae_vector* b,
    ae_int_t* info, ae_state *_state)
{
    rmatrixsolvefast(a,n,b,info, _state);
}


/*************************************************************************
Dense solver.

Similar to RMatrixSolve() but solves task with multiple right parts (where
b and x are NxM matrices). This is  "slow-but-robust"  version  of  linear
solver with additional functionality  like  condition  number  estimation.
There also exists faster version - RMatrixSolveMFast().

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* optional iterative refinement
* O(N^3+M*N^2) complexity

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear  system
           ! and  performs  iterative   refinement,   which   results   in
           ! significant performance penalty  when  compared  with  "fast"
           ! version  which  just  performs  LU  decomposition  and  calls
           ! triangular solver.
           !
           ! This  performance  penalty  is  especially  visible  in   the
           ! multithreaded mode, because both condition number  estimation
           ! and   iterative    refinement   are   inherently   sequential
           ! calculations. It also very significant on small matrices.
           !
           ! Thus, if you need high performance and if you are pretty sure
           ! that your system is well conditioned, we  strongly  recommend
           ! you to use faster solver, RMatrixSolveMFast() function.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that LU decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=1024,  achieving
  ! near-linear speedup for N=4096 or higher.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size
    RFS     -   iterative refinement switch:
                * True - refinement is used.
                  Less performance, more precision.
                * False - refinement is not used.
                  More performance, less precision.

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    A is ill conditioned or singular.
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros


  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void rmatrixsolvem(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_bool rfs,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix da;
    ae_matrix emptya;
    ae_vector p;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);
    ae_matrix_init(&da, 0, 0, DT_REAL, _state);
    ae_matrix_init(&emptya, 0, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&da, n, n, _state);
    
    /*
     * 1. factorize matrix
     * 3. solve
     */
    for(i=0; i<=n-1; i++)
    {
        ae_v_move(&da.ptr.pp_double[i][0], 1, &a->ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
    }
    rmatrixlu(&da, n, n, &p, _state);
    if( rfs )
    {
        densesolver_rmatrixlusolveinternal(&da, &p, n, a, ae_true, b, m, info, rep, x, _state);
    }
    else
    {
        densesolver_rmatrixlusolveinternal(&da, &p, n, &emptya, ae_false, b, m, info, rep, x, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixsolvem(/* Real    */ ae_matrix* a,
    ae_int_t n,
    /* Real    */ ae_matrix* b,
    ae_int_t m,
    ae_bool rfs,
    ae_int_t* info,
    densesolverreport* rep,
    /* Real    */ ae_matrix* x, ae_state *_state)
{
    rmatrixsolvem(a,n,b,m,rfs,info,rep,x, _state);
}


/*************************************************************************
Dense solver.

Similar to RMatrixSolve() but solves task with multiple right parts (where
b and x are NxM matrices). This is "fast" version of linear  solver  which
does NOT offer additional functions like condition  number  estimation  or
iterative refinement.

Algorithm features:
* O(N^3+M*N^2) complexity
* no additional functionality, highest performance

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that LU decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=1024,  achieving
  ! near-linear speedup for N=4096 or higher.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size
    RFS     -   iterative refinement switch:
                * True - refinement is used.
                  Less performance, more precision.
                * False - refinement is not used.
                  More performance, less precision.

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is exactly singular (ill conditioned matrices
                        are not recognized).
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task is solved
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    B       -   array[N]:
                * info>0    =>  overwritten by solution
                * info=-3   =>  filled by zeros


  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void rmatrixsolvemfast(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    double v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_vector p;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    *info = 0;
    ae_vector_init(&p, 0, DT_INT, _state);

    
    /*
     * Check for exact degeneracy
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    rmatrixlu(a, n, n, &p, _state);
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_eq(a->ptr.pp_double[i][i],(double)(0)) )
        {
            for(j=0; j<=n-1; j++)
            {
                for(k=0; k<=m-1; k++)
                {
                    b->ptr.pp_double[j][k] = 0.0;
                }
            }
            *info = -3;
            ae_frame_leave(_state);
            return;
        }
    }
    
    /*
     * Solve with TRSM()
     */
    for(i=0; i<=n-1; i++)
    {
        if( p.ptr.p_int[i]!=i )
        {
            for(j=0; j<=m-1; j++)
            {
                v = b->ptr.pp_double[i][j];
                b->ptr.pp_double[i][j] = b->ptr.pp_double[p.ptr.p_int[i]][j];
                b->ptr.pp_double[p.ptr.p_int[i]][j] = v;
            }
        }
    }
    rmatrixlefttrsm(n, m, a, 0, 0, ae_false, ae_true, 0, b, 0, 0, _state);
    rmatrixlefttrsm(n, m, a, 0, 0, ae_true, ae_false, 0, b, 0, 0, _state);
    *info = 1;
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixsolvemfast(/* Real    */ ae_matrix* a,
    ae_int_t n,
    /* Real    */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info, ae_state *_state)
{
    rmatrixsolvemfast(a,n,b,m,info, _state);
}


/*************************************************************************
Dense solver.

This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
real matrix given by its LU decomposition, x and b are real vectors.  This
is "slow-but-robust" version of the linear LU-based solver. Faster version
is RMatrixLUSolveFast() function.

Algorithm features:
* automatic detection of degenerate cases
* O(N^2) complexity
* condition number estimation

No iterative refinement  is provided because exact form of original matrix
is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which results in 10-15x  performance  penalty  when  compared
           ! with "fast" version which just calls triangular solver.
           !
           ! This performance penalty is insignificant  when compared with
           ! cost of large LU decomposition.  However,  if you  call  this
           ! function many times for the same  left  side,  this  overhead
           ! BECOMES significant. It  also  becomes significant for small-
           ! scale problems.
           !
           ! In such cases we strongly recommend you to use faster solver,
           ! RMatrixLUSolveFast() function.

INPUT PARAMETERS
    LUA     -   array[N,N], LU decomposition, RMatrixLU result
    P       -   array[N], pivots array, RMatrixLU result
    N       -   size of A
    B       -   array[N], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or exactly singular.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

    
  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void rmatrixlusolve(/* Real    */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Real    */ ae_vector* b,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix bm;
    ae_matrix xm;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_vector_clear(x);
    ae_matrix_init(&bm, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xm, 0, 0, DT_REAL, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&bm, n, 1, _state);
    ae_v_move(&bm.ptr.pp_double[0][0], bm.stride, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
    rmatrixlusolvem(lua, p, n, &bm, 1, info, rep, &xm, _state);
    ae_vector_set_length(x, n, _state);
    ae_v_move(&x->ptr.p_double[0], 1, &xm.ptr.pp_double[0][0], xm.stride, ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Dense solver.

This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
real matrix given by its LU decomposition, x and b are real vectors.  This
is "fast-without-any-checks" version of the linear LU-based solver. Slower
but more robust version is RMatrixLUSolve() function.

Algorithm features:
* O(N^2) complexity
* fast algorithm without ANY additional checks, just triangular solver

INPUT PARAMETERS
    LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
    P       -   array[0..N-1], pivots array, RMatrixLU result
    N       -   size of A
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is exactly singular (ill conditioned matrices
                        are not recognized).
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task is solved 
    B       -   array[N]:
                * info>0    =>  overwritten by solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 18.03.2015 by Bochkanov Sergey
*************************************************************************/
void rmatrixlusolvefast(/* Real    */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Real    */ ae_vector* b,
     ae_int_t* info,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    *info = 0;

    if( n<=0 )
    {
        *info = -1;
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_eq(lua->ptr.pp_double[i][i],(double)(0)) )
        {
            for(j=0; j<=n-1; j++)
            {
                b->ptr.p_double[j] = 0.0;
            }
            *info = -3;
            return;
        }
    }
    densesolver_rbasiclusolve(lua, p, n, b, _state);
    *info = 1;
}


/*************************************************************************
Dense solver.

Similar to RMatrixLUSolve() but solves  task  with  multiple  right  parts
(where b and x are NxM matrices). This  is  "robust-but-slow"  version  of
LU-based solver which performs additional  checks  for  non-degeneracy  of
inputs (condition number estimation). If you need  best  performance,  use
"fast-without-any-checks" version, RMatrixLUSolveMFast().

Algorithm features:
* automatic detection of degenerate cases
* O(M*N^2) complexity
* condition number estimation

No iterative refinement  is provided because exact form of original matrix
is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which  results  in  significant  performance   penalty   when
           ! compared with "fast"  version  which  just  calls  triangular
           ! solver.
           !
           ! This performance penalty is especially apparent when you  use
           ! ALGLIB parallel capabilities (condition number estimation  is
           ! inherently  sequential).  It   also   becomes significant for
           ! small-scale problems.
           !
           ! In such cases we strongly recommend you to use faster solver,
           ! RMatrixLUSolveMFast() function.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. Triangular solver is relatively easy to parallelize.
  ! However, parallelization will be efficient  only for  large number  of
  ! right parts M.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.
  
INPUT PARAMETERS
    LUA     -   array[N,N], LU decomposition, RMatrixLU result
    P       -   array[N], pivots array, RMatrixLU result
    N       -   size of A
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or exactly singular.
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N,M], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros


  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void rmatrixlusolvem(/* Real    */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix emptya;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);
    ae_matrix_init(&emptya, 0, 0, DT_REAL, _state);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * solve
     */
    densesolver_rmatrixlusolveinternal(lua, p, n, &emptya, ae_false, b, m, info, rep, x, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixlusolvem(/* Real    */ ae_matrix* lua,
    /* Integer */ ae_vector* p,
    ae_int_t n,
    /* Real    */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info,
    densesolverreport* rep,
    /* Real    */ ae_matrix* x, ae_state *_state)
{
    rmatrixlusolvem(lua,p,n,b,m,info,rep,x, _state);
}


/*************************************************************************
Dense solver.

Similar to RMatrixLUSolve() but solves  task  with  multiple  right parts,
where b and x are NxM matrices.  This is "fast-without-any-checks" version
of LU-based solver. It does not estimate  condition number  of  a  system,
so it is extremely fast. If you need better detection  of  near-degenerate
cases, use RMatrixLUSolveM() function.

Algorithm features:
* O(M*N^2) complexity
* fast algorithm without ANY additional checks, just triangular solver

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. Triangular solver is relatively easy to parallelize.
  ! However, parallelization will be efficient  only for  large number  of
  ! right parts M.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
    P       -   array[0..N-1], pivots array, RMatrixLU result
    N       -   size of A
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS:
    Info    -   return code:
                * -3    matrix is exactly singular (ill conditioned matrices
                        are not recognized).
                * -1    N<=0 was passed
                *  1    task is solved
    B       -   array[N,M]:
                * info>0    =>  overwritten by solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 18.03.2015 by Bochkanov Sergey
*************************************************************************/
void rmatrixlusolvemfast(/* Real    */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     ae_state *_state)
{
    double v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;

    *info = 0;

    
    /*
     * Check for exact degeneracy
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_eq(lua->ptr.pp_double[i][i],(double)(0)) )
        {
            for(j=0; j<=n-1; j++)
            {
                for(k=0; k<=m-1; k++)
                {
                    b->ptr.pp_double[j][k] = 0.0;
                }
            }
            *info = -3;
            return;
        }
    }
    
    /*
     * Solve with TRSM()
     */
    for(i=0; i<=n-1; i++)
    {
        if( p->ptr.p_int[i]!=i )
        {
            for(j=0; j<=m-1; j++)
            {
                v = b->ptr.pp_double[i][j];
                b->ptr.pp_double[i][j] = b->ptr.pp_double[p->ptr.p_int[i]][j];
                b->ptr.pp_double[p->ptr.p_int[i]][j] = v;
            }
        }
    }
    rmatrixlefttrsm(n, m, lua, 0, 0, ae_false, ae_true, 0, b, 0, 0, _state);
    rmatrixlefttrsm(n, m, lua, 0, 0, ae_true, ae_false, 0, b, 0, 0, _state);
    *info = 1;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixlusolvemfast(/* Real    */ ae_matrix* lua,
    /* Integer */ ae_vector* p,
    ae_int_t n,
    /* Real    */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info, ae_state *_state)
{
    rmatrixlusolvemfast(lua,p,n,b,m,info, _state);
}


/*************************************************************************
Dense solver.

This  subroutine  solves  a  system  A*x=b,  where BOTH ORIGINAL A AND ITS
LU DECOMPOSITION ARE KNOWN. You can use it if for some  reasons  you  have
both A and its LU decomposition.

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* iterative refinement
* O(N^2) complexity

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
    P       -   array[0..N-1], pivots array, RMatrixLU result
    N       -   size of A
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or exactly singular.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void rmatrixmixedsolve(/* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Real    */ ae_vector* b,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix bm;
    ae_matrix xm;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_vector_clear(x);
    ae_matrix_init(&bm, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xm, 0, 0, DT_REAL, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&bm, n, 1, _state);
    ae_v_move(&bm.ptr.pp_double[0][0], bm.stride, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
    rmatrixmixedsolvem(a, lua, p, n, &bm, 1, info, rep, &xm, _state);
    ae_vector_set_length(x, n, _state);
    ae_v_move(&x->ptr.p_double[0], 1, &xm.ptr.pp_double[0][0], xm.stride, ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Dense solver.

Similar to RMatrixMixedSolve() but  solves task with multiple right  parts
(where b and x are NxM matrices).

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* iterative refinement
* O(M*N^2) complexity

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
    P       -   array[0..N-1], pivots array, RMatrixLU result
    N       -   size of A
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or exactly singular.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N,M], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void rmatrixmixedsolvem(/* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* x,
     ae_state *_state)
{

    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        return;
    }
    
    /*
     * solve
     */
    densesolver_rmatrixlusolveinternal(lua, p, n, a, ae_true, b, m, info, rep, x, _state);
}


/*************************************************************************
Complex dense solver for A*X=B with N*N  complex  matrix  A,  N*M  complex
matrices  X  and  B.  "Slow-but-feature-rich"   version   which   provides
additional functions, at the cost of slower  performance.  Faster  version
may be invoked with CMatrixSolveMFast() function.

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* iterative refinement
* O(N^3+M*N^2) complexity

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear  system
           ! and  performs  iterative   refinement,   which   results   in
           ! significant performance penalty  when  compared  with  "fast"
           ! version  which  just  performs  LU  decomposition  and  calls
           ! triangular solver.
           !
           ! This  performance  penalty  is  especially  visible  in   the
           ! multithreaded mode, because both condition number  estimation
           ! and   iterative    refinement   are   inherently   sequential
           ! calculations.
           !
           ! Thus, if you need high performance and if you are pretty sure
           ! that your system is well conditioned, we  strongly  recommend
           ! you to use faster solver, CMatrixSolveMFast() function.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that LU decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=1024,  achieving
  ! near-linear speedup for N=4096 or higher.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size
    RFS     -   iterative refinement switch:
                * True - refinement is used.
                  Less performance, more precision.
                * False - refinement is not used.
                  More performance, less precision.

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or exactly singular.
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N,M], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void cmatrixsolvem(/* Complex */ ae_matrix* a,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_bool rfs,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix da;
    ae_matrix emptya;
    ae_vector p;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);
    ae_matrix_init(&da, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&emptya, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&p, 0, DT_INT, _state);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&da, n, n, _state);
    
    /*
     * factorize, solve
     */
    for(i=0; i<=n-1; i++)
    {
        ae_v_cmove(&da.ptr.pp_complex[i][0], 1, &a->ptr.pp_complex[i][0], 1, "N", ae_v_len(0,n-1));
    }
    cmatrixlu(&da, n, n, &p, _state);
    if( rfs )
    {
        densesolver_cmatrixlusolveinternal(&da, &p, n, a, ae_true, b, m, info, rep, x, _state);
    }
    else
    {
        densesolver_cmatrixlusolveinternal(&da, &p, n, &emptya, ae_false, b, m, info, rep, x, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixsolvem(/* Complex */ ae_matrix* a,
    ae_int_t n,
    /* Complex */ ae_matrix* b,
    ae_int_t m,
    ae_bool rfs,
    ae_int_t* info,
    densesolverreport* rep,
    /* Complex */ ae_matrix* x, ae_state *_state)
{
    cmatrixsolvem(a,n,b,m,rfs,info,rep,x, _state);
}


/*************************************************************************
Complex dense solver for A*X=B with N*N  complex  matrix  A,  N*M  complex
matrices  X  and  B.  "Fast-but-lightweight" version which  provides  just
triangular solver - and no additional functions like iterative  refinement
or condition number estimation.

Algorithm features:
* O(N^3+M*N^2) complexity
* no additional time consuming functions

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that LU decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=1024,  achieving
  ! near-linear speedup for N=4096 or higher.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS:
    Info    -   return code:
                * -3    matrix is exactly singular (ill conditioned matrices
                        are not recognized).
                * -1    N<=0 was passed
                *  1    task is solved 
    B       -   array[N,M]:
                * info>0    =>  overwritten by solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 16.03.2015 by Bochkanov Sergey
*************************************************************************/
void cmatrixsolvemfast(/* Complex */ ae_matrix* a,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_complex v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_vector p;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    *info = 0;
    ae_vector_init(&p, 0, DT_INT, _state);

    
    /*
     * Check for exact degeneracy
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    cmatrixlu(a, n, n, &p, _state);
    for(i=0; i<=n-1; i++)
    {
        if( ae_c_eq_d(a->ptr.pp_complex[i][i],(double)(0)) )
        {
            for(j=0; j<=n-1; j++)
            {
                for(k=0; k<=m-1; k++)
                {
                    b->ptr.pp_complex[j][k] = ae_complex_from_d(0.0);
                }
            }
            *info = -3;
            ae_frame_leave(_state);
            return;
        }
    }
    
    /*
     * Solve with TRSM()
     */
    for(i=0; i<=n-1; i++)
    {
        if( p.ptr.p_int[i]!=i )
        {
            for(j=0; j<=m-1; j++)
            {
                v = b->ptr.pp_complex[i][j];
                b->ptr.pp_complex[i][j] = b->ptr.pp_complex[p.ptr.p_int[i]][j];
                b->ptr.pp_complex[p.ptr.p_int[i]][j] = v;
            }
        }
    }
    cmatrixlefttrsm(n, m, a, 0, 0, ae_false, ae_true, 0, b, 0, 0, _state);
    cmatrixlefttrsm(n, m, a, 0, 0, ae_true, ae_false, 0, b, 0, 0, _state);
    *info = 1;
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixsolvemfast(/* Complex */ ae_matrix* a,
    ae_int_t n,
    /* Complex */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info, ae_state *_state)
{
    cmatrixsolvemfast(a,n,b,m,info, _state);
}


/*************************************************************************
Complex dense solver for A*x=B with N*N complex matrix A and  N*1  complex
vectors x and b. "Slow-but-feature-rich" version of the solver.

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* iterative refinement
* O(N^3) complexity

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear  system
           ! and  performs  iterative   refinement,   which   results   in
           ! significant performance penalty  when  compared  with  "fast"
           ! version  which  just  performs  LU  decomposition  and  calls
           ! triangular solver.
           !
           ! This  performance  penalty  is  especially  visible  in   the
           ! multithreaded mode, because both condition number  estimation
           ! and   iterative    refinement   are   inherently   sequential
           ! calculations.
           !
           ! Thus, if you need high performance and if you are pretty sure
           ! that your system is well conditioned, we  strongly  recommend
           ! you to use faster solver, CMatrixSolveFast() function.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that LU decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=1024,  achieving
  ! near-linear speedup for N=4096 or higher.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or exactly singular.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void cmatrixsolve(/* Complex */ ae_matrix* a,
     ae_int_t n,
     /* Complex */ ae_vector* b,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix bm;
    ae_matrix xm;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_vector_clear(x);
    ae_matrix_init(&bm, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&xm, 0, 0, DT_COMPLEX, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&bm, n, 1, _state);
    ae_v_cmove(&bm.ptr.pp_complex[0][0], bm.stride, &b->ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
    cmatrixsolvem(a, n, &bm, 1, ae_true, info, rep, &xm, _state);
    ae_vector_set_length(x, n, _state);
    ae_v_cmove(&x->ptr.p_complex[0], 1, &xm.ptr.pp_complex[0][0], xm.stride, "N", ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixsolve(/* Complex */ ae_matrix* a,
    ae_int_t n,
    /* Complex */ ae_vector* b,
    ae_int_t* info,
    densesolverreport* rep,
    /* Complex */ ae_vector* x, ae_state *_state)
{
    cmatrixsolve(a,n,b,info,rep,x, _state);
}


/*************************************************************************
Complex dense solver for A*x=B with N*N complex matrix A and  N*1  complex
vectors x and b. "Fast-but-lightweight" version of the solver.

Algorithm features:
* O(N^3) complexity
* no additional time consuming features, just triangular solver

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that LU decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=1024,  achieving
  ! near-linear speedup for N=4096 or higher.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS:
    Info    -   return code:
                * -3    matrix is exactly singular (ill conditioned matrices
                        are not recognized).
                * -1    N<=0 was passed
                *  1    task is solved 
    B       -   array[N]:
                * info>0    =>  overwritten by solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void cmatrixsolvefast(/* Complex */ ae_matrix* a,
     ae_int_t n,
     /* Complex */ ae_vector* b,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;
    ae_int_t j;
    ae_vector p;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    *info = 0;
    ae_vector_init(&p, 0, DT_INT, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    cmatrixlu(a, n, n, &p, _state);
    for(i=0; i<=n-1; i++)
    {
        if( ae_c_eq_d(a->ptr.pp_complex[i][i],(double)(0)) )
        {
            for(j=0; j<=n-1; j++)
            {
                b->ptr.p_complex[j] = ae_complex_from_d(0.0);
            }
            *info = -3;
            ae_frame_leave(_state);
            return;
        }
    }
    densesolver_cbasiclusolve(a, &p, n, b, _state);
    *info = 1;
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixsolvefast(/* Complex */ ae_matrix* a,
    ae_int_t n,
    /* Complex */ ae_vector* b,
    ae_int_t* info, ae_state *_state)
{
    cmatrixsolvefast(a,n,b,info, _state);
}


/*************************************************************************
Dense solver for A*X=B with N*N complex A given by its  LU  decomposition,
and N*M matrices X and B (multiple right sides).   "Slow-but-feature-rich"
version of the solver.

Algorithm features:
* automatic detection of degenerate cases
* O(M*N^2) complexity
* condition number estimation

No iterative refinement  is provided because exact form of original matrix
is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which  results  in  significant  performance   penalty   when
           ! compared with "fast"  version  which  just  calls  triangular
           ! solver.
           !
           ! This performance penalty is especially apparent when you  use
           ! ALGLIB parallel capabilities (condition number estimation  is
           ! inherently  sequential).  It   also   becomes significant for
           ! small-scale problems.
           !
           ! In such cases we strongly recommend you to use faster solver,
           ! CMatrixLUSolveMFast() function.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. Triangular solver is relatively easy to parallelize.
  ! However, parallelization will be efficient  only for  large number  of
  ! right parts M.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
    P       -   array[0..N-1], pivots array, RMatrixLU result
    N       -   size of A
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or exactly singular.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N,M], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void cmatrixlusolvem(/* Complex */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix emptya;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);
    ae_matrix_init(&emptya, 0, 0, DT_COMPLEX, _state);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * solve
     */
    densesolver_cmatrixlusolveinternal(lua, p, n, &emptya, ae_false, b, m, info, rep, x, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixlusolvem(/* Complex */ ae_matrix* lua,
    /* Integer */ ae_vector* p,
    ae_int_t n,
    /* Complex */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info,
    densesolverreport* rep,
    /* Complex */ ae_matrix* x, ae_state *_state)
{
    cmatrixlusolvem(lua,p,n,b,m,info,rep,x, _state);
}


/*************************************************************************
Dense solver for A*X=B with N*N complex A given by its  LU  decomposition,
and N*M matrices X and B (multiple  right  sides).  "Fast-but-lightweight"
version of the solver.

Algorithm features:
* O(M*N^2) complexity
* no additional time-consuming features

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. Triangular solver is relatively easy to parallelize.
  ! However, parallelization will be efficient  only for  large number  of
  ! right parts M.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
    P       -   array[0..N-1], pivots array, RMatrixLU result
    N       -   size of A
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is exactly singular (ill conditioned matrices
                        are not recognized).
                * -1    N<=0 was passed
                *  1    task is solved 
    B       -   array[N,M]:
                * info>0    =>  overwritten by solution
                * info=-3   =>  filled by zeros


  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void cmatrixlusolvemfast(/* Complex */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     ae_state *_state)
{
    ae_complex v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;

    *info = 0;

    
    /*
     * Check for exact degeneracy
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        if( ae_c_eq_d(lua->ptr.pp_complex[i][i],(double)(0)) )
        {
            for(j=0; j<=n-1; j++)
            {
                for(k=0; k<=m-1; k++)
                {
                    b->ptr.pp_complex[j][k] = ae_complex_from_d(0.0);
                }
            }
            *info = -3;
            return;
        }
    }
    
    /*
     * Solve with TRSM()
     */
    for(i=0; i<=n-1; i++)
    {
        if( p->ptr.p_int[i]!=i )
        {
            for(j=0; j<=m-1; j++)
            {
                v = b->ptr.pp_complex[i][j];
                b->ptr.pp_complex[i][j] = b->ptr.pp_complex[p->ptr.p_int[i]][j];
                b->ptr.pp_complex[p->ptr.p_int[i]][j] = v;
            }
        }
    }
    cmatrixlefttrsm(n, m, lua, 0, 0, ae_false, ae_true, 0, b, 0, 0, _state);
    cmatrixlefttrsm(n, m, lua, 0, 0, ae_true, ae_false, 0, b, 0, 0, _state);
    *info = 1;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixlusolvemfast(/* Complex */ ae_matrix* lua,
    /* Integer */ ae_vector* p,
    ae_int_t n,
    /* Complex */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info, ae_state *_state)
{
    cmatrixlusolvemfast(lua,p,n,b,m,info, _state);
}


/*************************************************************************
Complex dense linear solver for A*x=b with complex N*N A  given  by its LU
decomposition and N*1 vectors x and b. This is  "slow-but-robust"  version
of  the  complex  linear  solver  with  additional  features   which   add
significant performance overhead. Faster version  is  CMatrixLUSolveFast()
function.

Algorithm features:
* automatic detection of degenerate cases
* O(N^2) complexity
* condition number estimation

No iterative refinement is provided because exact form of original matrix
is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which results in 10-15x  performance  penalty  when  compared
           ! with "fast" version which just calls triangular solver.
           !
           ! This performance penalty is insignificant  when compared with
           ! cost of large LU decomposition.  However,  if you  call  this
           ! function many times for the same  left  side,  this  overhead
           ! BECOMES significant. It  also  becomes significant for small-
           ! scale problems.
           !
           ! In such cases we strongly recommend you to use faster solver,
           ! CMatrixLUSolveFast() function.

INPUT PARAMETERS
    LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
    P       -   array[0..N-1], pivots array, CMatrixLU result
    N       -   size of A
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or exactly singular.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void cmatrixlusolve(/* Complex */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Complex */ ae_vector* b,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix bm;
    ae_matrix xm;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_vector_clear(x);
    ae_matrix_init(&bm, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&xm, 0, 0, DT_COMPLEX, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&bm, n, 1, _state);
    ae_v_cmove(&bm.ptr.pp_complex[0][0], bm.stride, &b->ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
    cmatrixlusolvem(lua, p, n, &bm, 1, info, rep, &xm, _state);
    ae_vector_set_length(x, n, _state);
    ae_v_cmove(&x->ptr.p_complex[0], 1, &xm.ptr.pp_complex[0][0], xm.stride, "N", ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Complex dense linear solver for A*x=b with N*N complex A given by  its  LU
decomposition and N*1 vectors x and b. This is  fast  lightweight  version
of solver, which is significantly faster than CMatrixLUSolve(),  but  does
not provide additional information (like condition numbers).

Algorithm features:
* O(N^2) complexity
* no additional time-consuming features, just triangular solver

INPUT PARAMETERS
    LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
    P       -   array[0..N-1], pivots array, CMatrixLU result
    N       -   size of A
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is exactly singular (ill conditioned matrices
                        are not recognized).
                * -1    N<=0 was passed
                *  1    task is solved 
    B       -   array[N]:
                * info>0    =>  overwritten by solution
                * info=-3   =>  filled by zeros
    
NOTE: unlike  CMatrixLUSolve(),  this   function   does   NOT   check  for
      near-degeneracy of input matrix. It  checks  for  EXACT  degeneracy,
      because this check is easy to do. However,  very  badly  conditioned
      matrices may went unnoticed.


  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void cmatrixlusolvefast(/* Complex */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Complex */ ae_vector* b,
     ae_int_t* info,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    *info = 0;

    if( n<=0 )
    {
        *info = -1;
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        if( ae_c_eq_d(lua->ptr.pp_complex[i][i],(double)(0)) )
        {
            for(j=0; j<=n-1; j++)
            {
                b->ptr.p_complex[j] = ae_complex_from_d(0.0);
            }
            *info = -3;
            return;
        }
    }
    densesolver_cbasiclusolve(lua, p, n, b, _state);
    *info = 1;
}


/*************************************************************************
Dense solver. Same as RMatrixMixedSolveM(), but for complex matrices.

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* iterative refinement
* O(M*N^2) complexity

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
    P       -   array[0..N-1], pivots array, CMatrixLU result
    N       -   size of A
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or exactly singular.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N,M], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void cmatrixmixedsolvem(/* Complex */ ae_matrix* a,
     /* Complex */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* x,
     ae_state *_state)
{

    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        return;
    }
    
    /*
     * solve
     */
    densesolver_cmatrixlusolveinternal(lua, p, n, a, ae_true, b, m, info, rep, x, _state);
}


/*************************************************************************
Dense solver. Same as RMatrixMixedSolve(), but for complex matrices.

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* iterative refinement
* O(N^2) complexity

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
    P       -   array[0..N-1], pivots array, CMatrixLU result
    N       -   size of A
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or exactly singular.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void cmatrixmixedsolve(/* Complex */ ae_matrix* a,
     /* Complex */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Complex */ ae_vector* b,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix bm;
    ae_matrix xm;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_vector_clear(x);
    ae_matrix_init(&bm, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&xm, 0, 0, DT_COMPLEX, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&bm, n, 1, _state);
    ae_v_cmove(&bm.ptr.pp_complex[0][0], bm.stride, &b->ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
    cmatrixmixedsolvem(a, lua, p, n, &bm, 1, info, rep, &xm, _state);
    ae_vector_set_length(x, n, _state);
    ae_v_cmove(&x->ptr.p_complex[0], 1, &xm.ptr.pp_complex[0][0], xm.stride, "N", ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Dense solver for A*X=B with N*N symmetric positive definite matrix A,  and
N*M vectors X and B. It is "slow-but-feature-rich" version of the solver.

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* O(N^3+M*N^2) complexity
* matrix is represented by its upper or lower triangle

No iterative refinement is provided because such partial representation of
matrix does not allow efficient calculation of extra-precise  matrix-vector
products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which  results  in  significant   performance   penalty  when
           ! compared with "fast" version  which  just  performs  Cholesky
           ! decomposition and calls triangular solver.
           !
           ! This  performance  penalty  is  especially  visible  in   the
           ! multithreaded mode, because both condition number  estimation
           ! and   iterative    refinement   are   inherently   sequential
           ! calculations.
           !
           ! Thus, if you need high performance and if you are pretty sure
           ! that your system is well conditioned, we  strongly  recommend
           ! you to use faster solver, SPDMatrixSolveMFast() function.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that Cholesky decomposition is harder
  ! to parallelize than, say, matrix-matrix product - this  algorithm  has
  ! several synchronization points which  can  not  be  avoided.  However,
  ! parallelism starts to be profitable starting from N=500.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    IsUpper -   what half of A is provided
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or non-SPD.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N,M], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void spdmatrixsolvem(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix da;
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t j2;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);
    ae_matrix_init(&da, 0, 0, DT_REAL, _state);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&da, n, n, _state);
    
    /*
     * factorize
     * solve
     */
    for(i=0; i<=n-1; i++)
    {
        if( isupper )
        {
            j1 = i;
            j2 = n-1;
        }
        else
        {
            j1 = 0;
            j2 = i;
        }
        ae_v_move(&da.ptr.pp_double[i][j1], 1, &a->ptr.pp_double[i][j1], 1, ae_v_len(j1,j2));
    }
    if( !spdmatrixcholesky(&da, n, isupper, _state) )
    {
        ae_matrix_set_length(x, n, m, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                x->ptr.pp_double[i][j] = (double)(0);
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    densesolver_spdmatrixcholeskysolveinternal(&da, n, isupper, a, ae_true, b, m, info, rep, x, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spdmatrixsolvem(/* Real    */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    /* Real    */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info,
    densesolverreport* rep,
    /* Real    */ ae_matrix* x, ae_state *_state)
{
    spdmatrixsolvem(a,n,isupper,b,m,info,rep,x, _state);
}


/*************************************************************************
Dense solver for A*X=B with N*N symmetric positive definite matrix A,  and
N*M vectors X and B. It is "fast-but-lightweight" version of the solver.

Algorithm features:
* O(N^3+M*N^2) complexity
* matrix is represented by its upper or lower triangle
* no additional time consuming features

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that Cholesky decomposition is harder
  ! to parallelize than, say, matrix-matrix product - this  algorithm  has
  ! several synchronization points which  can  not  be  avoided.  However,
  ! parallelism starts to be profitable starting from N=500.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    IsUpper -   what half of A is provided
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    A is is exactly singular
                * -1    N<=0 was passed
                *  1    task was solved
    B       -   array[N,M], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 17.03.2015 by Bochkanov Sergey
*************************************************************************/
void spdmatrixsolvemfast(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    *info = 0;

    *info = 1;
    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( !spdmatrixcholesky(a, n, isupper, _state) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                b->ptr.pp_double[i][j] = 0.0;
            }
        }
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    if( isupper )
    {
        rmatrixlefttrsm(n, m, a, 0, 0, ae_true, ae_false, 1, b, 0, 0, _state);
        rmatrixlefttrsm(n, m, a, 0, 0, ae_true, ae_false, 0, b, 0, 0, _state);
    }
    else
    {
        rmatrixlefttrsm(n, m, a, 0, 0, ae_false, ae_false, 0, b, 0, 0, _state);
        rmatrixlefttrsm(n, m, a, 0, 0, ae_false, ae_false, 1, b, 0, 0, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spdmatrixsolvemfast(/* Real    */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    /* Real    */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info, ae_state *_state)
{
    spdmatrixsolvemfast(a,n,isupper,b,m,info, _state);
}


/*************************************************************************
Dense linear solver for A*x=b with N*N real  symmetric  positive  definite
matrix A,  N*1 vectors x and b.  "Slow-but-feature-rich"  version  of  the
solver.

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* O(N^3) complexity
* matrix is represented by its upper or lower triangle

No iterative refinement is provided because such partial representation of
matrix does not allow efficient calculation of extra-precise  matrix-vector
products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which  results  in  significant   performance   penalty  when
           ! compared with "fast" version  which  just  performs  Cholesky
           ! decomposition and calls triangular solver.
           !
           ! This  performance  penalty  is  especially  visible  in   the
           ! multithreaded mode, because both condition number  estimation
           ! and   iterative    refinement   are   inherently   sequential
           ! calculations.
           !
           ! Thus, if you need high performance and if you are pretty sure
           ! that your system is well conditioned, we  strongly  recommend
           ! you to use faster solver, SPDMatrixSolveFast() function.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that Cholesky decomposition is harder
  ! to parallelize than, say, matrix-matrix product - this  algorithm  has
  ! several synchronization points which  can  not  be  avoided.  However,
  ! parallelism starts to be profitable starting from N=500.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    IsUpper -   what half of A is provided
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    matrix is very badly conditioned or non-SPD.
                * -1    N<=0 was passed
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void spdmatrixsolve(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* b,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix bm;
    ae_matrix xm;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_vector_clear(x);
    ae_matrix_init(&bm, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xm, 0, 0, DT_REAL, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&bm, n, 1, _state);
    ae_v_move(&bm.ptr.pp_double[0][0], bm.stride, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
    spdmatrixsolvem(a, n, isupper, &bm, 1, info, rep, &xm, _state);
    ae_vector_set_length(x, n, _state);
    ae_v_move(&x->ptr.p_double[0], 1, &xm.ptr.pp_double[0][0], xm.stride, ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spdmatrixsolve(/* Real    */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    /* Real    */ ae_vector* b,
    ae_int_t* info,
    densesolverreport* rep,
    /* Real    */ ae_vector* x, ae_state *_state)
{
    spdmatrixsolve(a,n,isupper,b,info,rep,x, _state);
}


/*************************************************************************
Dense linear solver for A*x=b with N*N real  symmetric  positive  definite
matrix A,  N*1 vectors x and  b.  "Fast-but-lightweight"  version  of  the
solver.

Algorithm features:
* O(N^3) complexity
* matrix is represented by its upper or lower triangle
* no additional time consuming features like condition number estimation

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that Cholesky decomposition is harder
  ! to parallelize than, say, matrix-matrix product - this  algorithm  has
  ! several synchronization points which  can  not  be  avoided.  However,
  ! parallelism starts to be profitable starting from N=500.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    IsUpper -   what half of A is provided
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    A is is exactly singular or non-SPD
                * -1    N<=0 was passed
                *  1    task was solved
    B       -   array[N], it contains:
                * info>0    =>  solution
                * info=-3   =>  filled by zeros

  -- ALGLIB --
     Copyright 17.03.2015 by Bochkanov Sergey
*************************************************************************/
void spdmatrixsolvefast(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* b,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    *info = 0;

    *info = 1;
    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( !spdmatrixcholesky(a, n, isupper, _state) )
    {
        for(i=0; i<=n-1; i++)
        {
            b->ptr.p_double[i] = 0.0;
        }
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    densesolver_spdbasiccholeskysolve(a, n, isupper, b, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spdmatrixsolvefast(/* Real    */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    /* Real    */ ae_vector* b,
    ae_int_t* info, ae_state *_state)
{
    spdmatrixsolvefast(a,n,isupper,b,info, _state);
}


/*************************************************************************
Dense solver for A*X=B with N*N symmetric positive definite matrix A given
by its Cholesky decomposition, and N*M vectors X and B. It  is  "slow-but-
feature-rich" version of the solver which estimates  condition  number  of
the system.

Algorithm features:
* automatic detection of degenerate cases
* O(M*N^2) complexity
* condition number estimation
* matrix is represented by its upper or lower triangle

No iterative refinement is provided because such partial representation of
matrix does not allow efficient calculation of extra-precise  matrix-vector
products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which  results  in  significant  performance   penalty   when
           ! compared with "fast"  version  which  just  calls  triangular
           ! solver. Amount of  overhead  introduced  depends  on  M  (the
           ! larger - the more efficient).
           !
           ! This performance penalty is insignificant  when compared with
           ! cost of large LU decomposition.  However,  if you  call  this
           ! function many times for the same  left  side,  this  overhead
           ! BECOMES significant. It  also  becomes significant for small-
           ! scale problems (N<50).
           !
           ! In such cases we strongly recommend you to use faster solver,
           ! SPDMatrixCholeskySolveMFast() function.

INPUT PARAMETERS
    CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                SPDMatrixCholesky result
    N       -   size of CHA
    IsUpper -   what half of CHA is provided
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    A is is exactly singular or badly conditioned
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task was solved
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N]:
                * for info>0 contains solution
                * for info=-3 filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void spdmatrixcholeskysolvem(/* Real    */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix emptya;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);
    ae_matrix_init(&emptya, 0, 0, DT_REAL, _state);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * solve
     */
    densesolver_spdmatrixcholeskysolveinternal(cha, n, isupper, &emptya, ae_false, b, m, info, rep, x, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spdmatrixcholeskysolvem(/* Real    */ ae_matrix* cha,
    ae_int_t n,
    ae_bool isupper,
    /* Real    */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info,
    densesolverreport* rep,
    /* Real    */ ae_matrix* x, ae_state *_state)
{
    spdmatrixcholeskysolvem(cha,n,isupper,b,m,info,rep,x, _state);
}


/*************************************************************************
Dense solver for A*X=B with N*N symmetric positive definite matrix A given
by its Cholesky decomposition, and N*M vectors X and B. It  is  "fast-but-
lightweight" version of  the  solver  which  just  solves  linear  system,
without any additional functions.

Algorithm features:
* O(M*N^2) complexity
* matrix is represented by its upper or lower triangle
* no additional functionality

INPUT PARAMETERS
    CHA     -   array[N,N], Cholesky decomposition,
                SPDMatrixCholesky result
    N       -   size of CHA
    IsUpper -   what half of CHA is provided
    B       -   array[N,M], right part
    M       -   right part size

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    A is is exactly singular or badly conditioned
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task was solved
    B       -   array[N]:
                * for info>0 overwritten by solution
                * for info=-3 filled by zeros

  -- ALGLIB --
     Copyright 18.03.2015 by Bochkanov Sergey
*************************************************************************/
void spdmatrixcholeskysolvemfast(/* Real    */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;

    *info = 0;

    *info = 1;
    if( n<=0 )
    {
        *info = -1;
        return;
    }
    for(k=0; k<=n-1; k++)
    {
        if( ae_fp_eq(cha->ptr.pp_double[k][k],0.0) )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    b->ptr.pp_double[i][j] = 0.0;
                }
            }
            *info = -3;
            return;
        }
    }
    if( isupper )
    {
        rmatrixlefttrsm(n, m, cha, 0, 0, ae_true, ae_false, 1, b, 0, 0, _state);
        rmatrixlefttrsm(n, m, cha, 0, 0, ae_true, ae_false, 0, b, 0, 0, _state);
    }
    else
    {
        rmatrixlefttrsm(n, m, cha, 0, 0, ae_false, ae_false, 0, b, 0, 0, _state);
        rmatrixlefttrsm(n, m, cha, 0, 0, ae_false, ae_false, 1, b, 0, 0, _state);
    }
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spdmatrixcholeskysolvemfast(/* Real    */ ae_matrix* cha,
    ae_int_t n,
    ae_bool isupper,
    /* Real    */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info, ae_state *_state)
{
    spdmatrixcholeskysolvemfast(cha,n,isupper,b,m,info, _state);
}


/*************************************************************************
Dense solver for A*x=b with N*N symmetric positive definite matrix A given
by its Cholesky decomposition, and N*1 real vectors x and b. This is "slow-
but-feature-rich"  version  of  the  solver  which,  in  addition  to  the
solution, performs condition number estimation.

Algorithm features:
* automatic detection of degenerate cases
* O(N^2) complexity
* condition number estimation
* matrix is represented by its upper or lower triangle

No iterative refinement is provided because such partial representation of
matrix does not allow efficient calculation of extra-precise  matrix-vector
products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which results in 10-15x  performance  penalty  when  compared
           ! with "fast" version which just calls triangular solver.
           !
           ! This performance penalty is insignificant  when compared with
           ! cost of large LU decomposition.  However,  if you  call  this
           ! function many times for the same  left  side,  this  overhead
           ! BECOMES significant. It  also  becomes significant for small-
           ! scale problems (N<50).
           !
           ! In such cases we strongly recommend you to use faster solver,
           ! SPDMatrixCholeskySolveFast() function.

INPUT PARAMETERS
    CHA     -   array[N,N], Cholesky decomposition,
                SPDMatrixCholesky result
    N       -   size of A
    IsUpper -   what half of CHA is provided
    B       -   array[N], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    A is is exactly singular or ill conditioned
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task is solved
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N]:
                * for info>0  - solution
                * for info=-3 - filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void spdmatrixcholeskysolve(/* Real    */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* b,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix bm;
    ae_matrix xm;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_vector_clear(x);
    ae_matrix_init(&bm, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xm, 0, 0, DT_REAL, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&bm, n, 1, _state);
    ae_v_move(&bm.ptr.pp_double[0][0], bm.stride, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
    spdmatrixcholeskysolvem(cha, n, isupper, &bm, 1, info, rep, &xm, _state);
    ae_vector_set_length(x, n, _state);
    ae_v_move(&x->ptr.p_double[0], 1, &xm.ptr.pp_double[0][0], xm.stride, ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Dense solver for A*x=b with N*N symmetric positive definite matrix A given
by its Cholesky decomposition, and N*1 real vectors x and b. This is "fast-
but-lightweight" version of the solver.

Algorithm features:
* O(N^2) complexity
* matrix is represented by its upper or lower triangle
* no additional features

INPUT PARAMETERS
    CHA     -   array[N,N], Cholesky decomposition,
                SPDMatrixCholesky result
    N       -   size of A
    IsUpper -   what half of CHA is provided
    B       -   array[N], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    A is is exactly singular or ill conditioned
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task is solved
    B       -   array[N]:
                * for info>0  - overwritten by solution
                * for info=-3 - filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void spdmatrixcholeskysolvefast(/* Real    */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* b,
     ae_int_t* info,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;

    *info = 0;

    *info = 1;
    if( n<=0 )
    {
        *info = -1;
        return;
    }
    for(k=0; k<=n-1; k++)
    {
        if( ae_fp_eq(cha->ptr.pp_double[k][k],0.0) )
        {
            for(i=0; i<=n-1; i++)
            {
                b->ptr.p_double[i] = 0.0;
            }
            *info = -3;
            return;
        }
    }
    densesolver_spdbasiccholeskysolve(cha, n, isupper, b, _state);
}


/*************************************************************************
Dense solver for A*X=B, with N*N Hermitian positive definite matrix A  and
N*M  complex  matrices  X  and  B.  "Slow-but-feature-rich" version of the
solver.

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* O(N^3+M*N^2) complexity
* matrix is represented by its upper or lower triangle

No iterative refinement is provided because such partial representation of
matrix does not allow efficient calculation of extra-precise  matrix-vector
products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which  results  in  significant  performance   penalty   when
           ! compared with "fast"  version  which  just  calls  triangular
           ! solver.
           !
           ! This performance penalty is especially apparent when you  use
           ! ALGLIB parallel capabilities (condition number estimation  is
           ! inherently  sequential).  It   also   becomes significant for
           ! small-scale problems (N<100).
           !
           ! In such cases we strongly recommend you to use faster solver,
           ! HPDMatrixSolveMFast() function.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that Cholesky decomposition is harder
  ! to parallelize than, say, matrix-matrix product - this  algorithm  has
  ! several synchronization points which  can  not  be  avoided.  However,
  ! parallelism starts to be profitable starting from N=500.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    IsUpper -   what half of A is provided
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS
    Info    -   same as in RMatrixSolve.
                Returns -3 for non-HPD matrices.
    Rep     -   same as in RMatrixSolve
    X       -   same as in RMatrixSolve

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void hpdmatrixsolvem(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix da;
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t j2;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);
    ae_matrix_init(&da, 0, 0, DT_COMPLEX, _state);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&da, n, n, _state);
    
    /*
     * factorize matrix, solve
     */
    for(i=0; i<=n-1; i++)
    {
        if( isupper )
        {
            j1 = i;
            j2 = n-1;
        }
        else
        {
            j1 = 0;
            j2 = i;
        }
        ae_v_cmove(&da.ptr.pp_complex[i][j1], 1, &a->ptr.pp_complex[i][j1], 1, "N", ae_v_len(j1,j2));
    }
    if( !hpdmatrixcholesky(&da, n, isupper, _state) )
    {
        ae_matrix_set_length(x, n, m, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                x->ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    densesolver_hpdmatrixcholeskysolveinternal(&da, n, isupper, a, ae_true, b, m, info, rep, x, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_hpdmatrixsolvem(/* Complex */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    /* Complex */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info,
    densesolverreport* rep,
    /* Complex */ ae_matrix* x, ae_state *_state)
{
    hpdmatrixsolvem(a,n,isupper,b,m,info,rep,x, _state);
}


/*************************************************************************
Dense solver for A*X=B, with N*N Hermitian positive definite matrix A  and
N*M complex matrices X and B. "Fast-but-lightweight" version of the solver.

Algorithm features:
* O(N^3+M*N^2) complexity
* matrix is represented by its upper or lower triangle
* no additional time consuming features like condition number estimation

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that Cholesky decomposition is harder
  ! to parallelize than, say, matrix-matrix product - this  algorithm  has
  ! several synchronization points which  can  not  be  avoided.  However,
  ! parallelism starts to be profitable starting from N=500.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    IsUpper -   what half of A is provided
    B       -   array[0..N-1,0..M-1], right part
    M       -   right part size

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    A is is exactly  singular or is not positive definite.
                        B is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task is solved
    B       -   array[0..N-1]:
                * overwritten by solution
                * zeros, if problem was not solved

  -- ALGLIB --
     Copyright 17.03.2015 by Bochkanov Sergey
*************************************************************************/
void hpdmatrixsolvemfast(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    *info = 0;

    *info = 1;
    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( !hpdmatrixcholesky(a, n, isupper, _state) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                b->ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
            }
        }
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    if( isupper )
    {
        cmatrixlefttrsm(n, m, a, 0, 0, ae_true, ae_false, 2, b, 0, 0, _state);
        cmatrixlefttrsm(n, m, a, 0, 0, ae_true, ae_false, 0, b, 0, 0, _state);
    }
    else
    {
        cmatrixlefttrsm(n, m, a, 0, 0, ae_false, ae_false, 0, b, 0, 0, _state);
        cmatrixlefttrsm(n, m, a, 0, 0, ae_false, ae_false, 2, b, 0, 0, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_hpdmatrixsolvemfast(/* Complex */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    /* Complex */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info, ae_state *_state)
{
    hpdmatrixsolvemfast(a,n,isupper,b,m,info, _state);
}


/*************************************************************************
Dense solver for A*x=b, with N*N Hermitian positive definite matrix A, and
N*1 complex vectors  x  and  b.  "Slow-but-feature-rich"  version  of  the
solver.

Algorithm features:
* automatic detection of degenerate cases
* condition number estimation
* O(N^3) complexity
* matrix is represented by its upper or lower triangle

No iterative refinement is provided because such partial representation of
matrix does not allow efficient calculation of extra-precise  matrix-vector
products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which  results  in  significant   performance   penalty  when
           ! compared with "fast" version  which  just  performs  Cholesky
           ! decomposition and calls triangular solver.
           !
           ! This  performance  penalty  is  especially  visible  in   the
           ! multithreaded mode, because both condition number  estimation
           ! and   iterative    refinement   are   inherently   sequential
           ! calculations.
           !
           ! Thus, if you need high performance and if you are pretty sure
           ! that your system is well conditioned, we  strongly  recommend
           ! you to use faster solver, HPDMatrixSolveFast() function.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that Cholesky decomposition is harder
  ! to parallelize than, say, matrix-matrix product - this  algorithm  has
  ! several synchronization points which  can  not  be  avoided.  However,
  ! parallelism starts to be profitable starting from N=500.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    IsUpper -   what half of A is provided
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   same as in RMatrixSolve
                Returns -3 for non-HPD matrices.
    Rep     -   same as in RMatrixSolve
    X       -   same as in RMatrixSolve

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void hpdmatrixsolve(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* b,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix bm;
    ae_matrix xm;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_vector_clear(x);
    ae_matrix_init(&bm, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&xm, 0, 0, DT_COMPLEX, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&bm, n, 1, _state);
    ae_v_cmove(&bm.ptr.pp_complex[0][0], bm.stride, &b->ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
    hpdmatrixsolvem(a, n, isupper, &bm, 1, info, rep, &xm, _state);
    ae_vector_set_length(x, n, _state);
    ae_v_cmove(&x->ptr.p_complex[0], 1, &xm.ptr.pp_complex[0][0], xm.stride, "N", ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_hpdmatrixsolve(/* Complex */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    /* Complex */ ae_vector* b,
    ae_int_t* info,
    densesolverreport* rep,
    /* Complex */ ae_vector* x, ae_state *_state)
{
    hpdmatrixsolve(a,n,isupper,b,info,rep,x, _state);
}


/*************************************************************************
Dense solver for A*x=b, with N*N Hermitian positive definite matrix A, and
N*1 complex vectors  x  and  b.  "Fast-but-lightweight"  version  of   the
solver without additional functions.

Algorithm features:
* O(N^3) complexity
* matrix is represented by its upper or lower triangle
* no additional time consuming functions

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions. Generally,  MKL  accelerates  any
  ! problem whose size is at least 128, with best  efficiency achieved for
  ! N's larger than 512.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of this function. We should note that Cholesky decomposition is harder
  ! to parallelize than, say, matrix-matrix product - this  algorithm  has
  ! several synchronization points which  can  not  be  avoided.  However,
  ! parallelism starts to be profitable starting from N=500.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..N-1,0..N-1], system matrix
    N       -   size of A
    IsUpper -   what half of A is provided
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    A is is exactly singular or not positive definite
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task was solved 
    B       -   array[0..N-1]:
                * overwritten by solution
                * zeros, if A is exactly singular (diagonal of its LU
                  decomposition has exact zeros).

  -- ALGLIB --
     Copyright 17.03.2015 by Bochkanov Sergey
*************************************************************************/
void hpdmatrixsolvefast(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* b,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    *info = 0;

    *info = 1;
    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( !hpdmatrixcholesky(a, n, isupper, _state) )
    {
        for(i=0; i<=n-1; i++)
        {
            b->ptr.p_complex[i] = ae_complex_from_d(0.0);
        }
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    densesolver_hpdbasiccholeskysolve(a, n, isupper, b, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_hpdmatrixsolvefast(/* Complex */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    /* Complex */ ae_vector* b,
    ae_int_t* info, ae_state *_state)
{
    hpdmatrixsolvefast(a,n,isupper,b,info, _state);
}


/*************************************************************************
Dense solver for A*X=B with N*N Hermitian positive definite matrix A given
by its Cholesky decomposition and N*M complex matrices X  and  B.  This is
"slow-but-feature-rich" version of the solver which, in  addition  to  the
solution, estimates condition number of the system.

Algorithm features:
* automatic detection of degenerate cases
* O(M*N^2) complexity
* condition number estimation
* matrix is represented by its upper or lower triangle

No iterative refinement is provided because such partial representation of
matrix does not allow efficient calculation of extra-precise  matrix-vector
products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which  results  in  significant  performance   penalty   when
           ! compared with "fast"  version  which  just  calls  triangular
           ! solver. Amount of  overhead  introduced  depends  on  M  (the
           ! larger - the more efficient).
           !
           ! This performance penalty is insignificant  when compared with
           ! cost of large Cholesky decomposition.  However,  if  you call
           ! this  function  many  times  for  the same  left  side,  this
           ! overhead BECOMES significant. It  also   becomes  significant
           ! for small-scale problems (N<50).
           !
           ! In such cases we strongly recommend you to use faster solver,
           ! HPDMatrixCholeskySolveMFast() function.


INPUT PARAMETERS
    CHA     -   array[N,N], Cholesky decomposition,
                HPDMatrixCholesky result
    N       -   size of CHA
    IsUpper -   what half of CHA is provided
    B       -   array[N,M], right part
    M       -   right part size

OUTPUT PARAMETERS:
    Info    -   return code:
                * -3    A is singular, or VERY close to singular.
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task was solved
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N]:
                * for info>0 contains solution
                * for info=-3 filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void hpdmatrixcholeskysolvem(/* Complex */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix emptya;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);
    ae_matrix_init(&emptya, 0, 0, DT_COMPLEX, _state);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * 1. scale matrix, max(|U[i,j]|)
     * 2. factorize scaled matrix
     * 3. solve
     */
    densesolver_hpdmatrixcholeskysolveinternal(cha, n, isupper, &emptya, ae_false, b, m, info, rep, x, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_hpdmatrixcholeskysolvem(/* Complex */ ae_matrix* cha,
    ae_int_t n,
    ae_bool isupper,
    /* Complex */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info,
    densesolverreport* rep,
    /* Complex */ ae_matrix* x, ae_state *_state)
{
    hpdmatrixcholeskysolvem(cha,n,isupper,b,m,info,rep,x, _state);
}


/*************************************************************************
Dense solver for A*X=B with N*N Hermitian positive definite matrix A given
by its Cholesky decomposition and N*M complex matrices X  and  B.  This is
"fast-but-lightweight" version of the solver.

Algorithm features:
* O(M*N^2) complexity
* matrix is represented by its upper or lower triangle
* no additional time-consuming features

INPUT PARAMETERS
    CHA     -   array[N,N], Cholesky decomposition,
                HPDMatrixCholesky result
    N       -   size of CHA
    IsUpper -   what half of CHA is provided
    B       -   array[N,M], right part
    M       -   right part size

OUTPUT PARAMETERS:
    Info    -   return code:
                * -3    A is singular, or VERY close to singular.
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task was solved
    B       -   array[N]:
                * for info>0 overwritten by solution
                * for info=-3 filled by zeros

  -- ALGLIB --
     Copyright 18.03.2015 by Bochkanov Sergey
*************************************************************************/
void hpdmatrixcholeskysolvemfast(/* Complex */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;

    *info = 0;

    *info = 1;
    if( n<=0 )
    {
        *info = -1;
        return;
    }
    for(k=0; k<=n-1; k++)
    {
        if( ae_fp_eq(cha->ptr.pp_complex[k][k].x,0.0)&&ae_fp_eq(cha->ptr.pp_complex[k][k].y,0.0) )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    b->ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
                }
            }
            *info = -3;
            return;
        }
    }
    if( isupper )
    {
        cmatrixlefttrsm(n, m, cha, 0, 0, ae_true, ae_false, 2, b, 0, 0, _state);
        cmatrixlefttrsm(n, m, cha, 0, 0, ae_true, ae_false, 0, b, 0, 0, _state);
    }
    else
    {
        cmatrixlefttrsm(n, m, cha, 0, 0, ae_false, ae_false, 0, b, 0, 0, _state);
        cmatrixlefttrsm(n, m, cha, 0, 0, ae_false, ae_false, 2, b, 0, 0, _state);
    }
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_hpdmatrixcholeskysolvemfast(/* Complex */ ae_matrix* cha,
    ae_int_t n,
    ae_bool isupper,
    /* Complex */ ae_matrix* b,
    ae_int_t m,
    ae_int_t* info, ae_state *_state)
{
    hpdmatrixcholeskysolvemfast(cha,n,isupper,b,m,info, _state);
}


/*************************************************************************
Dense solver for A*x=b with N*N Hermitian positive definite matrix A given
by its Cholesky decomposition, and N*1 complex vectors x and  b.  This  is
"slow-but-feature-rich" version of the solver  which  estimates  condition
number of the system.

Algorithm features:
* automatic detection of degenerate cases
* O(N^2) complexity
* condition number estimation
* matrix is represented by its upper or lower triangle

No iterative refinement is provided because such partial representation of
matrix does not allow efficient calculation of extra-precise  matrix-vector
products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
need iterative refinement.

IMPORTANT: ! this function is NOT the most efficient linear solver provided
           ! by ALGLIB. It estimates condition  number  of  linear system,
           ! which results in 10-15x  performance  penalty  when  compared
           ! with "fast" version which just calls triangular solver.
           !
           ! This performance penalty is insignificant  when compared with
           ! cost of large LU decomposition.  However,  if you  call  this
           ! function many times for the same  left  side,  this  overhead
           ! BECOMES significant. It  also  becomes significant for small-
           ! scale problems (N<50).
           !
           ! In such cases we strongly recommend you to use faster solver,
           ! HPDMatrixCholeskySolveFast() function.

INPUT PARAMETERS
    CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                SPDMatrixCholesky result
    N       -   size of A
    IsUpper -   what half of CHA is provided
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    A is is exactly singular or ill conditioned
                        X is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task is solved
    Rep     -   additional report, following fields are set:
                * rep.r1    condition number in 1-norm
                * rep.rinf  condition number in inf-norm
    X       -   array[N]:
                * for info>0  - solution
                * for info=-3 - filled by zeros

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
void hpdmatrixcholeskysolve(/* Complex */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* b,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix bm;
    ae_matrix xm;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_vector_clear(x);
    ae_matrix_init(&bm, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&xm, 0, 0, DT_COMPLEX, _state);

    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&bm, n, 1, _state);
    ae_v_cmove(&bm.ptr.pp_complex[0][0], bm.stride, &b->ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
    hpdmatrixcholeskysolvem(cha, n, isupper, &bm, 1, info, rep, &xm, _state);
    ae_vector_set_length(x, n, _state);
    ae_v_cmove(&x->ptr.p_complex[0], 1, &xm.ptr.pp_complex[0][0], xm.stride, "N", ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Dense solver for A*x=b with N*N Hermitian positive definite matrix A given
by its Cholesky decomposition, and N*1 complex vectors x and  b.  This  is
"fast-but-lightweight" version of the solver.

Algorithm features:
* O(N^2) complexity
* matrix is represented by its upper or lower triangle
* no additional time-consuming features

INPUT PARAMETERS
    CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                SPDMatrixCholesky result
    N       -   size of A
    IsUpper -   what half of CHA is provided
    B       -   array[0..N-1], right part

OUTPUT PARAMETERS
    Info    -   return code:
                * -3    A is is exactly singular or ill conditioned
                        B is filled by zeros in such cases.
                * -1    N<=0 was passed
                *  1    task is solved
    B       -   array[N]:
                * for info>0  - overwritten by solution
                * for info=-3 - filled by zeros

  -- ALGLIB --
     Copyright 18.03.2015 by Bochkanov Sergey
*************************************************************************/
void hpdmatrixcholeskysolvefast(/* Complex */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* b,
     ae_int_t* info,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;

    *info = 0;

    *info = 1;
    if( n<=0 )
    {
        *info = -1;
        return;
    }
    for(k=0; k<=n-1; k++)
    {
        if( ae_fp_eq(cha->ptr.pp_complex[k][k].x,0.0)&&ae_fp_eq(cha->ptr.pp_complex[k][k].y,0.0) )
        {
            for(i=0; i<=n-1; i++)
            {
                b->ptr.p_complex[i] = ae_complex_from_d(0.0);
            }
            *info = -3;
            return;
        }
    }
    densesolver_hpdbasiccholeskysolve(cha, n, isupper, b, _state);
}


/*************************************************************************
Dense solver.

This subroutine finds solution of the linear system A*X=B with non-square,
possibly degenerate A.  System  is  solved in the least squares sense, and
general least squares solution  X = X0 + CX*y  which  minimizes |A*X-B| is
returned. If A is non-degenerate, solution in the usual sense is returned.

Algorithm features:
* automatic detection (and correct handling!) of degenerate cases
* iterative refinement
* O(N^3) complexity

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes one  important  improvement   of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! Multithreaded acceleration is only partially supported (some parts are
  ! optimized, but most - are not).
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    A       -   array[0..NRows-1,0..NCols-1], system matrix
    NRows   -   vertical size of A
    NCols   -   horizontal size of A
    B       -   array[0..NCols-1], right part
    Threshold-  a number in [0,1]. Singular values  beyond  Threshold  are
                considered  zero.  Set  it to 0.0, if you don't understand
                what it means, so the solver will choose good value on its
                own.
                
OUTPUT PARAMETERS
    Info    -   return code:
                * -4    SVD subroutine failed
                * -1    if NRows<=0 or NCols<=0 or Threshold<0 was passed
                *  1    if task is solved
    Rep     -   solver report, see below for more info
    X       -   array[0..N-1,0..M-1], it contains:
                * solution of A*X=B (even for singular A)
                * zeros, if SVD subroutine failed

SOLVER REPORT

Subroutine sets following fields of the Rep structure:
* R2        reciprocal of condition number: 1/cond(A), 2-norm.
* N         = NCols
* K         dim(Null(A))
* CX        array[0..N-1,0..K-1], kernel of A.
            Columns of CX store such vectors that A*CX[i]=0.

  -- ALGLIB --
     Copyright 24.08.2009 by Bochkanov Sergey
*************************************************************************/
void rmatrixsolvels(/* Real    */ ae_matrix* a,
     ae_int_t nrows,
     ae_int_t ncols,
     /* Real    */ ae_vector* b,
     double threshold,
     ae_int_t* info,
     densesolverlsreport* rep,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector sv;
    ae_matrix u;
    ae_matrix vt;
    ae_vector rp;
    ae_vector utb;
    ae_vector sutb;
    ae_vector tmp;
    ae_vector ta;
    ae_vector tx;
    ae_vector buf;
    ae_vector w;
    ae_int_t i;
    ae_int_t j;
    ae_int_t nsv;
    ae_int_t kernelidx;
    double v;
    double verr;
    ae_bool svdfailed;
    ae_bool zeroa;
    ae_int_t rfs;
    ae_int_t nrfs;
    ae_bool terminatenexttime;
    ae_bool smallerr;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverlsreport_clear(rep);
    ae_vector_clear(x);
    ae_vector_init(&sv, 0, DT_REAL, _state);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt, 0, 0, DT_REAL, _state);
    ae_vector_init(&rp, 0, DT_REAL, _state);
    ae_vector_init(&utb, 0, DT_REAL, _state);
    ae_vector_init(&sutb, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&ta, 0, DT_REAL, _state);
    ae_vector_init(&tx, 0, DT_REAL, _state);
    ae_vector_init(&buf, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);

    if( (nrows<=0||ncols<=0)||ae_fp_less(threshold,(double)(0)) )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( ae_fp_eq(threshold,(double)(0)) )
    {
        threshold = 1000*ae_machineepsilon;
    }
    
    /*
     * Factorize A first
     */
    svdfailed = !rmatrixsvd(a, nrows, ncols, 1, 2, 2, &sv, &u, &vt, _state);
    zeroa = ae_fp_eq(sv.ptr.p_double[0],(double)(0));
    if( svdfailed||zeroa )
    {
        if( svdfailed )
        {
            *info = -4;
        }
        else
        {
            *info = 1;
        }
        ae_vector_set_length(x, ncols, _state);
        for(i=0; i<=ncols-1; i++)
        {
            x->ptr.p_double[i] = (double)(0);
        }
        rep->n = ncols;
        rep->k = ncols;
        ae_matrix_set_length(&rep->cx, ncols, ncols, _state);
        for(i=0; i<=ncols-1; i++)
        {
            for(j=0; j<=ncols-1; j++)
            {
                if( i==j )
                {
                    rep->cx.ptr.pp_double[i][j] = (double)(1);
                }
                else
                {
                    rep->cx.ptr.pp_double[i][j] = (double)(0);
                }
            }
        }
        rep->r2 = (double)(0);
        ae_frame_leave(_state);
        return;
    }
    nsv = ae_minint(ncols, nrows, _state);
    if( nsv==ncols )
    {
        rep->r2 = sv.ptr.p_double[nsv-1]/sv.ptr.p_double[0];
    }
    else
    {
        rep->r2 = (double)(0);
    }
    rep->n = ncols;
    *info = 1;
    
    /*
     * Iterative refinement of xc combined with solution:
     * 1. xc = 0
     * 2. calculate r = bc-A*xc using extra-precise dot product
     * 3. solve A*y = r
     * 4. update x:=x+r
     * 5. goto 2
     *
     * This cycle is executed until one of two things happens:
     * 1. maximum number of iterations reached
     * 2. last iteration decreased error to the lower limit
     */
    ae_vector_set_length(&utb, nsv, _state);
    ae_vector_set_length(&sutb, nsv, _state);
    ae_vector_set_length(x, ncols, _state);
    ae_vector_set_length(&tmp, ncols, _state);
    ae_vector_set_length(&ta, ncols+1, _state);
    ae_vector_set_length(&tx, ncols+1, _state);
    ae_vector_set_length(&buf, ncols+1, _state);
    for(i=0; i<=ncols-1; i++)
    {
        x->ptr.p_double[i] = (double)(0);
    }
    kernelidx = nsv;
    for(i=0; i<=nsv-1; i++)
    {
        if( ae_fp_less_eq(sv.ptr.p_double[i],threshold*sv.ptr.p_double[0]) )
        {
            kernelidx = i;
            break;
        }
    }
    rep->k = ncols-kernelidx;
    nrfs = densesolver_densesolverrfsmaxv2(ncols, rep->r2, _state);
    terminatenexttime = ae_false;
    ae_vector_set_length(&rp, nrows, _state);
    for(rfs=0; rfs<=nrfs; rfs++)
    {
        if( terminatenexttime )
        {
            break;
        }
        
        /*
         * calculate right part
         */
        if( rfs==0 )
        {
            ae_v_move(&rp.ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,nrows-1));
        }
        else
        {
            smallerr = ae_true;
            for(i=0; i<=nrows-1; i++)
            {
                ae_v_move(&ta.ptr.p_double[0], 1, &a->ptr.pp_double[i][0], 1, ae_v_len(0,ncols-1));
                ta.ptr.p_double[ncols] = (double)(-1);
                ae_v_move(&tx.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,ncols-1));
                tx.ptr.p_double[ncols] = b->ptr.p_double[i];
                xdot(&ta, &tx, ncols+1, &buf, &v, &verr, _state);
                rp.ptr.p_double[i] = -v;
                smallerr = smallerr&&ae_fp_less(ae_fabs(v, _state),4*verr);
            }
            if( smallerr )
            {
                terminatenexttime = ae_true;
            }
        }
        
        /*
         * solve A*dx = rp
         */
        for(i=0; i<=ncols-1; i++)
        {
            tmp.ptr.p_double[i] = (double)(0);
        }
        for(i=0; i<=nsv-1; i++)
        {
            utb.ptr.p_double[i] = (double)(0);
        }
        for(i=0; i<=nrows-1; i++)
        {
            v = rp.ptr.p_double[i];
            ae_v_addd(&utb.ptr.p_double[0], 1, &u.ptr.pp_double[i][0], 1, ae_v_len(0,nsv-1), v);
        }
        for(i=0; i<=nsv-1; i++)
        {
            if( i<kernelidx )
            {
                sutb.ptr.p_double[i] = utb.ptr.p_double[i]/sv.ptr.p_double[i];
            }
            else
            {
                sutb.ptr.p_double[i] = (double)(0);
            }
        }
        for(i=0; i<=nsv-1; i++)
        {
            v = sutb.ptr.p_double[i];
            ae_v_addd(&tmp.ptr.p_double[0], 1, &vt.ptr.pp_double[i][0], 1, ae_v_len(0,ncols-1), v);
        }
        
        /*
         * update x:  x:=x+dx
         */
        ae_v_add(&x->ptr.p_double[0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,ncols-1));
    }
    
    /*
     * fill CX
     */
    if( rep->k>0 )
    {
        ae_matrix_set_length(&rep->cx, ncols, rep->k, _state);
        for(i=0; i<=rep->k-1; i++)
        {
            ae_v_move(&rep->cx.ptr.pp_double[0][i], rep->cx.stride, &vt.ptr.pp_double[kernelidx+i][0], 1, ae_v_len(0,ncols-1));
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixsolvels(/* Real    */ ae_matrix* a,
    ae_int_t nrows,
    ae_int_t ncols,
    /* Real    */ ae_vector* b,
    double threshold,
    ae_int_t* info,
    densesolverlsreport* rep,
    /* Real    */ ae_vector* x, ae_state *_state)
{
    rmatrixsolvels(a,nrows,ncols,b,threshold,info,rep,x, _state);
}


/*************************************************************************
Internal LU solver

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
static void densesolver_rmatrixlusolveinternal(/* Real    */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_bool havea,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t rfs;
    ae_int_t nrfs;
    ae_vector xc;
    ae_vector y;
    ae_vector bc;
    ae_vector xa;
    ae_vector xb;
    ae_vector tx;
    double v;
    double verr;
    double mxb;
    ae_bool smallerr;
    ae_bool terminatenexttime;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&bc, 0, DT_REAL, _state);
    ae_vector_init(&xa, 0, DT_REAL, _state);
    ae_vector_init(&xb, 0, DT_REAL, _state);
    ae_vector_init(&tx, 0, DT_REAL, _state);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        if( p->ptr.p_int[i]>n-1||p->ptr.p_int[i]<i )
        {
            *info = -1;
            ae_frame_leave(_state);
            return;
        }
    }
    ae_matrix_set_length(x, n, m, _state);
    ae_vector_set_length(&y, n, _state);
    ae_vector_set_length(&xc, n, _state);
    ae_vector_set_length(&bc, n, _state);
    ae_vector_set_length(&tx, n+1, _state);
    ae_vector_set_length(&xa, n+1, _state);
    ae_vector_set_length(&xb, n+1, _state);
    
    /*
     * estimate condition number, test for near singularity
     */
    rep->r1 = rmatrixlurcond1(lua, n, _state);
    rep->rinf = rmatrixlurcondinf(lua, n, _state);
    if( ae_fp_less(rep->r1,rcondthreshold(_state))||ae_fp_less(rep->rinf,rcondthreshold(_state)) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                x->ptr.pp_double[i][j] = (double)(0);
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    
    /*
     * First stage of solution: rough solution with TRSM()
     */
    mxb = 0.0;
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            v = b->ptr.pp_double[i][j];
            mxb = ae_maxreal(mxb, ae_fabs(v, _state), _state);
            x->ptr.pp_double[i][j] = v;
        }
    }
    for(i=0; i<=n-1; i++)
    {
        if( p->ptr.p_int[i]!=i )
        {
            for(j=0; j<=m-1; j++)
            {
                v = x->ptr.pp_double[i][j];
                x->ptr.pp_double[i][j] = x->ptr.pp_double[p->ptr.p_int[i]][j];
                x->ptr.pp_double[p->ptr.p_int[i]][j] = v;
            }
        }
    }
    rmatrixlefttrsm(n, m, lua, 0, 0, ae_false, ae_true, 0, x, 0, 0, _state);
    rmatrixlefttrsm(n, m, lua, 0, 0, ae_true, ae_false, 0, x, 0, 0, _state);
    
    /*
     * Second stage: iterative refinement
     */
    if( havea )
    {
        for(k=0; k<=m-1; k++)
        {
            nrfs = densesolver_densesolverrfsmax(n, rep->r1, rep->rinf, _state);
            terminatenexttime = ae_false;
            for(rfs=0; rfs<=nrfs-1; rfs++)
            {
                if( terminatenexttime )
                {
                    break;
                }
                
                /*
                 * generate right part
                 */
                smallerr = ae_true;
                ae_v_move(&xb.ptr.p_double[0], 1, &x->ptr.pp_double[0][k], x->stride, ae_v_len(0,n-1));
                for(i=0; i<=n-1; i++)
                {
                    ae_v_move(&xa.ptr.p_double[0], 1, &a->ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                    xa.ptr.p_double[n] = (double)(-1);
                    xb.ptr.p_double[n] = b->ptr.pp_double[i][k];
                    xdot(&xa, &xb, n+1, &tx, &v, &verr, _state);
                    y.ptr.p_double[i] = -v;
                    smallerr = smallerr&&ae_fp_less(ae_fabs(v, _state),4*verr);
                }
                if( smallerr )
                {
                    terminatenexttime = ae_true;
                }
                
                /*
                 * solve and update
                 */
                densesolver_rbasiclusolve(lua, p, n, &y, _state);
                ae_v_add(&x->ptr.pp_double[0][k], x->stride, &y.ptr.p_double[0], 1, ae_v_len(0,n-1));
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal Cholesky solver

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
static void densesolver_spdmatrixcholeskysolveinternal(/* Real    */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_matrix* a,
     ae_bool havea,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* x,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        return;
    }
    ae_matrix_set_length(x, n, m, _state);
    
    /*
     * estimate condition number, test for near singularity
     */
    rep->r1 = spdmatrixcholeskyrcond(cha, n, isupper, _state);
    rep->rinf = rep->r1;
    if( ae_fp_less(rep->r1,rcondthreshold(_state)) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                x->ptr.pp_double[i][j] = (double)(0);
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        return;
    }
    *info = 1;
    
    /*
     * Solve with TRSM()
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            x->ptr.pp_double[i][j] = b->ptr.pp_double[i][j];
        }
    }
    if( isupper )
    {
        rmatrixlefttrsm(n, m, cha, 0, 0, ae_true, ae_false, 1, x, 0, 0, _state);
        rmatrixlefttrsm(n, m, cha, 0, 0, ae_true, ae_false, 0, x, 0, 0, _state);
    }
    else
    {
        rmatrixlefttrsm(n, m, cha, 0, 0, ae_false, ae_false, 0, x, 0, 0, _state);
        rmatrixlefttrsm(n, m, cha, 0, 0, ae_false, ae_false, 1, x, 0, 0, _state);
    }
}


/*************************************************************************
Internal LU solver

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
static void densesolver_cmatrixlusolveinternal(/* Complex */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_bool havea,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t rfs;
    ae_int_t nrfs;
    ae_vector xc;
    ae_vector y;
    ae_vector bc;
    ae_vector xa;
    ae_vector xb;
    ae_vector tx;
    ae_vector tmpbuf;
    ae_complex v;
    double verr;
    ae_bool smallerr;
    ae_bool terminatenexttime;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);
    ae_vector_init(&xc, 0, DT_COMPLEX, _state);
    ae_vector_init(&y, 0, DT_COMPLEX, _state);
    ae_vector_init(&bc, 0, DT_COMPLEX, _state);
    ae_vector_init(&xa, 0, DT_COMPLEX, _state);
    ae_vector_init(&xb, 0, DT_COMPLEX, _state);
    ae_vector_init(&tx, 0, DT_COMPLEX, _state);
    ae_vector_init(&tmpbuf, 0, DT_REAL, _state);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        if( p->ptr.p_int[i]>n-1||p->ptr.p_int[i]<i )
        {
            *info = -1;
            ae_frame_leave(_state);
            return;
        }
    }
    ae_matrix_set_length(x, n, m, _state);
    ae_vector_set_length(&y, n, _state);
    ae_vector_set_length(&xc, n, _state);
    ae_vector_set_length(&bc, n, _state);
    ae_vector_set_length(&tx, n, _state);
    ae_vector_set_length(&xa, n+1, _state);
    ae_vector_set_length(&xb, n+1, _state);
    ae_vector_set_length(&tmpbuf, 2*n+2, _state);
    
    /*
     * estimate condition number, test for near singularity
     */
    rep->r1 = cmatrixlurcond1(lua, n, _state);
    rep->rinf = cmatrixlurcondinf(lua, n, _state);
    if( ae_fp_less(rep->r1,rcondthreshold(_state))||ae_fp_less(rep->rinf,rcondthreshold(_state)) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                x->ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    
    /*
     * First phase: solve with TRSM()
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            x->ptr.pp_complex[i][j] = b->ptr.pp_complex[i][j];
        }
    }
    for(i=0; i<=n-1; i++)
    {
        if( p->ptr.p_int[i]!=i )
        {
            for(j=0; j<=m-1; j++)
            {
                v = x->ptr.pp_complex[i][j];
                x->ptr.pp_complex[i][j] = x->ptr.pp_complex[p->ptr.p_int[i]][j];
                x->ptr.pp_complex[p->ptr.p_int[i]][j] = v;
            }
        }
    }
    cmatrixlefttrsm(n, m, lua, 0, 0, ae_false, ae_true, 0, x, 0, 0, _state);
    cmatrixlefttrsm(n, m, lua, 0, 0, ae_true, ae_false, 0, x, 0, 0, _state);
    
    /*
     * solve
     */
    for(k=0; k<=m-1; k++)
    {
        ae_v_cmove(&bc.ptr.p_complex[0], 1, &b->ptr.pp_complex[0][k], b->stride, "N", ae_v_len(0,n-1));
        ae_v_cmove(&xc.ptr.p_complex[0], 1, &x->ptr.pp_complex[0][k], x->stride, "N", ae_v_len(0,n-1));
        
        /*
         * Iterative refinement of xc:
         * * calculate r = bc-A*xc using extra-precise dot product
         * * solve A*y = r
         * * update x:=x+r
         *
         * This cycle is executed until one of two things happens:
         * 1. maximum number of iterations reached
         * 2. last iteration decreased error to the lower limit
         */
        if( havea )
        {
            nrfs = densesolver_densesolverrfsmax(n, rep->r1, rep->rinf, _state);
            terminatenexttime = ae_false;
            for(rfs=0; rfs<=nrfs-1; rfs++)
            {
                if( terminatenexttime )
                {
                    break;
                }
                
                /*
                 * generate right part
                 */
                smallerr = ae_true;
                ae_v_cmove(&xb.ptr.p_complex[0], 1, &xc.ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
                for(i=0; i<=n-1; i++)
                {
                    ae_v_cmove(&xa.ptr.p_complex[0], 1, &a->ptr.pp_complex[i][0], 1, "N", ae_v_len(0,n-1));
                    xa.ptr.p_complex[n] = ae_complex_from_i(-1);
                    xb.ptr.p_complex[n] = bc.ptr.p_complex[i];
                    xcdot(&xa, &xb, n+1, &tmpbuf, &v, &verr, _state);
                    y.ptr.p_complex[i] = ae_c_neg(v);
                    smallerr = smallerr&&ae_fp_less(ae_c_abs(v, _state),4*verr);
                }
                if( smallerr )
                {
                    terminatenexttime = ae_true;
                }
                
                /*
                 * solve and update
                 */
                densesolver_cbasiclusolve(lua, p, n, &y, _state);
                ae_v_cadd(&xc.ptr.p_complex[0], 1, &y.ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
            }
        }
        
        /*
         * Store xc.
         * Post-scale result.
         */
        ae_v_cmove(&x->ptr.pp_complex[0][k], x->stride, &xc.ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal Cholesky solver

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
static void densesolver_hpdmatrixcholeskysolveinternal(/* Complex */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_matrix* a,
     ae_bool havea,
     /* Complex */ ae_matrix* b,
     ae_int_t m,
     ae_int_t* info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector xc;
    ae_vector y;
    ae_vector bc;
    ae_vector xa;
    ae_vector xb;
    ae_vector tx;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _densesolverreport_clear(rep);
    ae_matrix_clear(x);
    ae_vector_init(&xc, 0, DT_COMPLEX, _state);
    ae_vector_init(&y, 0, DT_COMPLEX, _state);
    ae_vector_init(&bc, 0, DT_COMPLEX, _state);
    ae_vector_init(&xa, 0, DT_COMPLEX, _state);
    ae_vector_init(&xb, 0, DT_COMPLEX, _state);
    ae_vector_init(&tx, 0, DT_COMPLEX, _state);

    
    /*
     * prepare: check inputs, allocate space...
     */
    if( n<=0||m<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(x, n, m, _state);
    ae_vector_set_length(&y, n, _state);
    ae_vector_set_length(&xc, n, _state);
    ae_vector_set_length(&bc, n, _state);
    ae_vector_set_length(&tx, n+1, _state);
    ae_vector_set_length(&xa, n+1, _state);
    ae_vector_set_length(&xb, n+1, _state);
    
    /*
     * estimate condition number, test for near singularity
     */
    rep->r1 = hpdmatrixcholeskyrcond(cha, n, isupper, _state);
    rep->rinf = rep->r1;
    if( ae_fp_less(rep->r1,rcondthreshold(_state)) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                x->ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    
    /*
     * solve
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            x->ptr.pp_complex[i][j] = b->ptr.pp_complex[i][j];
        }
    }
    if( isupper )
    {
        cmatrixlefttrsm(n, m, cha, 0, 0, ae_true, ae_false, 2, x, 0, 0, _state);
        cmatrixlefttrsm(n, m, cha, 0, 0, ae_true, ae_false, 0, x, 0, 0, _state);
    }
    else
    {
        cmatrixlefttrsm(n, m, cha, 0, 0, ae_false, ae_false, 0, x, 0, 0, _state);
        cmatrixlefttrsm(n, m, cha, 0, 0, ae_false, ae_false, 2, x, 0, 0, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine.
Returns maximum count of RFS iterations as function of:
1. machine epsilon
2. task size.
3. condition number

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
static ae_int_t densesolver_densesolverrfsmax(ae_int_t n,
     double r1,
     double rinf,
     ae_state *_state)
{
    ae_int_t result;


    result = 5;
    return result;
}


/*************************************************************************
Internal subroutine.
Returns maximum count of RFS iterations as function of:
1. machine epsilon
2. task size.
3. norm-2 condition number

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
static ae_int_t densesolver_densesolverrfsmaxv2(ae_int_t n,
     double r2,
     ae_state *_state)
{
    ae_int_t result;


    result = densesolver_densesolverrfsmax(n, (double)(0), (double)(0), _state);
    return result;
}


/*************************************************************************
Basic LU solver for PLU*x = y.

This subroutine assumes that:
* A=PLU is well-conditioned, so no zero divisions or overflow may occur

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
static void densesolver_rbasiclusolve(/* Real    */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Real    */ ae_vector* xb,
     ae_state *_state)
{
    ae_int_t i;
    double v;


    for(i=0; i<=n-1; i++)
    {
        if( p->ptr.p_int[i]!=i )
        {
            v = xb->ptr.p_double[i];
            xb->ptr.p_double[i] = xb->ptr.p_double[p->ptr.p_int[i]];
            xb->ptr.p_double[p->ptr.p_int[i]] = v;
        }
    }
    for(i=1; i<=n-1; i++)
    {
        v = ae_v_dotproduct(&lua->ptr.pp_double[i][0], 1, &xb->ptr.p_double[0], 1, ae_v_len(0,i-1));
        xb->ptr.p_double[i] = xb->ptr.p_double[i]-v;
    }
    xb->ptr.p_double[n-1] = xb->ptr.p_double[n-1]/lua->ptr.pp_double[n-1][n-1];
    for(i=n-2; i>=0; i--)
    {
        v = ae_v_dotproduct(&lua->ptr.pp_double[i][i+1], 1, &xb->ptr.p_double[i+1], 1, ae_v_len(i+1,n-1));
        xb->ptr.p_double[i] = (xb->ptr.p_double[i]-v)/lua->ptr.pp_double[i][i];
    }
}


/*************************************************************************
Basic Cholesky solver for ScaleA*Cholesky(A)'*x = y.

This subroutine assumes that:
* A*ScaleA is well scaled
* A is well-conditioned, so no zero divisions or overflow may occur

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
static void densesolver_spdbasiccholeskysolve(/* Real    */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* xb,
     ae_state *_state)
{
    ae_int_t i;
    double v;


    
    /*
     * A = L*L' or A=U'*U
     */
    if( isupper )
    {
        
        /*
         * Solve U'*y=b first.
         */
        for(i=0; i<=n-1; i++)
        {
            xb->ptr.p_double[i] = xb->ptr.p_double[i]/cha->ptr.pp_double[i][i];
            if( i<n-1 )
            {
                v = xb->ptr.p_double[i];
                ae_v_subd(&xb->ptr.p_double[i+1], 1, &cha->ptr.pp_double[i][i+1], 1, ae_v_len(i+1,n-1), v);
            }
        }
        
        /*
         * Solve U*x=y then.
         */
        for(i=n-1; i>=0; i--)
        {
            if( i<n-1 )
            {
                v = ae_v_dotproduct(&cha->ptr.pp_double[i][i+1], 1, &xb->ptr.p_double[i+1], 1, ae_v_len(i+1,n-1));
                xb->ptr.p_double[i] = xb->ptr.p_double[i]-v;
            }
            xb->ptr.p_double[i] = xb->ptr.p_double[i]/cha->ptr.pp_double[i][i];
        }
    }
    else
    {
        
        /*
         * Solve L*y=b first
         */
        for(i=0; i<=n-1; i++)
        {
            if( i>0 )
            {
                v = ae_v_dotproduct(&cha->ptr.pp_double[i][0], 1, &xb->ptr.p_double[0], 1, ae_v_len(0,i-1));
                xb->ptr.p_double[i] = xb->ptr.p_double[i]-v;
            }
            xb->ptr.p_double[i] = xb->ptr.p_double[i]/cha->ptr.pp_double[i][i];
        }
        
        /*
         * Solve L'*x=y then.
         */
        for(i=n-1; i>=0; i--)
        {
            xb->ptr.p_double[i] = xb->ptr.p_double[i]/cha->ptr.pp_double[i][i];
            if( i>0 )
            {
                v = xb->ptr.p_double[i];
                ae_v_subd(&xb->ptr.p_double[0], 1, &cha->ptr.pp_double[i][0], 1, ae_v_len(0,i-1), v);
            }
        }
    }
}


/*************************************************************************
Basic LU solver for ScaleA*PLU*x = y.

This subroutine assumes that:
* L is well-scaled, and it is U which needs scaling by ScaleA.
* A=PLU is well-conditioned, so no zero divisions or overflow may occur

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
static void densesolver_cbasiclusolve(/* Complex */ ae_matrix* lua,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     /* Complex */ ae_vector* xb,
     ae_state *_state)
{
    ae_int_t i;
    ae_complex v;


    for(i=0; i<=n-1; i++)
    {
        if( p->ptr.p_int[i]!=i )
        {
            v = xb->ptr.p_complex[i];
            xb->ptr.p_complex[i] = xb->ptr.p_complex[p->ptr.p_int[i]];
            xb->ptr.p_complex[p->ptr.p_int[i]] = v;
        }
    }
    for(i=1; i<=n-1; i++)
    {
        v = ae_v_cdotproduct(&lua->ptr.pp_complex[i][0], 1, "N", &xb->ptr.p_complex[0], 1, "N", ae_v_len(0,i-1));
        xb->ptr.p_complex[i] = ae_c_sub(xb->ptr.p_complex[i],v);
    }
    xb->ptr.p_complex[n-1] = ae_c_div(xb->ptr.p_complex[n-1],lua->ptr.pp_complex[n-1][n-1]);
    for(i=n-2; i>=0; i--)
    {
        v = ae_v_cdotproduct(&lua->ptr.pp_complex[i][i+1], 1, "N", &xb->ptr.p_complex[i+1], 1, "N", ae_v_len(i+1,n-1));
        xb->ptr.p_complex[i] = ae_c_div(ae_c_sub(xb->ptr.p_complex[i],v),lua->ptr.pp_complex[i][i]);
    }
}


/*************************************************************************
Basic Cholesky solver for ScaleA*Cholesky(A)'*x = y.

This subroutine assumes that:
* A*ScaleA is well scaled
* A is well-conditioned, so no zero divisions or overflow may occur

  -- ALGLIB --
     Copyright 27.01.2010 by Bochkanov Sergey
*************************************************************************/
static void densesolver_hpdbasiccholeskysolve(/* Complex */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* xb,
     ae_state *_state)
{
    ae_int_t i;
    ae_complex v;


    
    /*
     * A = L*L' or A=U'*U
     */
    if( isupper )
    {
        
        /*
         * Solve U'*y=b first.
         */
        for(i=0; i<=n-1; i++)
        {
            xb->ptr.p_complex[i] = ae_c_div(xb->ptr.p_complex[i],ae_c_conj(cha->ptr.pp_complex[i][i], _state));
            if( i<n-1 )
            {
                v = xb->ptr.p_complex[i];
                ae_v_csubc(&xb->ptr.p_complex[i+1], 1, &cha->ptr.pp_complex[i][i+1], 1, "Conj", ae_v_len(i+1,n-1), v);
            }
        }
        
        /*
         * Solve U*x=y then.
         */
        for(i=n-1; i>=0; i--)
        {
            if( i<n-1 )
            {
                v = ae_v_cdotproduct(&cha->ptr.pp_complex[i][i+1], 1, "N", &xb->ptr.p_complex[i+1], 1, "N", ae_v_len(i+1,n-1));
                xb->ptr.p_complex[i] = ae_c_sub(xb->ptr.p_complex[i],v);
            }
            xb->ptr.p_complex[i] = ae_c_div(xb->ptr.p_complex[i],cha->ptr.pp_complex[i][i]);
        }
    }
    else
    {
        
        /*
         * Solve L*y=b first
         */
        for(i=0; i<=n-1; i++)
        {
            if( i>0 )
            {
                v = ae_v_cdotproduct(&cha->ptr.pp_complex[i][0], 1, "N", &xb->ptr.p_complex[0], 1, "N", ae_v_len(0,i-1));
                xb->ptr.p_complex[i] = ae_c_sub(xb->ptr.p_complex[i],v);
            }
            xb->ptr.p_complex[i] = ae_c_div(xb->ptr.p_complex[i],cha->ptr.pp_complex[i][i]);
        }
        
        /*
         * Solve L'*x=y then.
         */
        for(i=n-1; i>=0; i--)
        {
            xb->ptr.p_complex[i] = ae_c_div(xb->ptr.p_complex[i],ae_c_conj(cha->ptr.pp_complex[i][i], _state));
            if( i>0 )
            {
                v = xb->ptr.p_complex[i];
                ae_v_csubc(&xb->ptr.p_complex[0], 1, &cha->ptr.pp_complex[i][0], 1, "Conj", ae_v_len(0,i-1), v);
            }
        }
    }
}


void _densesolverreport_init(void* _p, ae_state *_state)
{
    densesolverreport *p = (densesolverreport*)_p;
    ae_touch_ptr((void*)p);
}


void _densesolverreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    densesolverreport *dst = (densesolverreport*)_dst;
    densesolverreport *src = (densesolverreport*)_src;
    dst->r1 = src->r1;
    dst->rinf = src->rinf;
}


void _densesolverreport_clear(void* _p)
{
    densesolverreport *p = (densesolverreport*)_p;
    ae_touch_ptr((void*)p);
}


void _densesolverreport_destroy(void* _p)
{
    densesolverreport *p = (densesolverreport*)_p;
    ae_touch_ptr((void*)p);
}


void _densesolverlsreport_init(void* _p, ae_state *_state)
{
    densesolverlsreport *p = (densesolverlsreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->cx, 0, 0, DT_REAL, _state);
}


void _densesolverlsreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    densesolverlsreport *dst = (densesolverlsreport*)_dst;
    densesolverlsreport *src = (densesolverlsreport*)_src;
    dst->r2 = src->r2;
    ae_matrix_init_copy(&dst->cx, &src->cx, _state);
    dst->n = src->n;
    dst->k = src->k;
}


void _densesolverlsreport_clear(void* _p)
{
    densesolverlsreport *p = (densesolverlsreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->cx);
}


void _densesolverlsreport_destroy(void* _p)
{
    densesolverlsreport *p = (densesolverlsreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->cx);
}


/*$ End $*/
