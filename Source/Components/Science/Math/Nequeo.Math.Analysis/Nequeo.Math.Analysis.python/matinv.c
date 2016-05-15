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
#include "matinv.h"


/*$ Declarations $*/
static ae_int_t matinv_parallelsize = 64;
static void matinv_rmatrixtrinverserec(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     /* Real    */ ae_vector* tmp,
     sinteger* info,
     matinvreport* rep,
     ae_state *_state);
static void matinv_cmatrixtrinverserec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     /* Complex */ ae_vector* tmp,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state);
static void matinv_rmatrixluinverserec(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     /* Real    */ ae_vector* work,
     sinteger* info,
     matinvreport* rep,
     ae_state *_state);
static void matinv_cmatrixluinverserec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     /* Complex */ ae_vector* work,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state);
static void matinv_hpdmatrixcholeskyinverserec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* tmp,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Inversion of a matrix given by its LU decomposition.

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
  ! of this function. We should note that matrix inversion  is  harder  to
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
    A       -   LU decomposition of the matrix
                (output of RMatrixLU subroutine).
    Pivots  -   table of permutations
                (the output of RMatrixLU subroutine).
    N       -   size of matrix A (optional) :
                * if given, only principal NxN submatrix is processed  and
                  overwritten. other elements are unchanged.
                * if not given,  size  is  automatically  determined  from
                  matrix size (A must be square matrix)

OUTPUT PARAMETERS:
    Info    -   return code:
                * -3    A is singular, or VERY close to singular.
                        it is filled by zeros in such cases.
                *  1    task is solved (but matrix A may be ill-conditioned,
                        check R1/RInf parameters for condition numbers).
    Rep     -   solver report, see below for more info
    A       -   inverse of matrix A.
                Array whose indexes range within [0..N-1, 0..N-1].

SOLVER REPORT

Subroutine sets following fields of the Rep structure:
* R1        reciprocal of condition number: 1/cond(A), 1-norm.
* RInf      reciprocal of condition number: 1/cond(A), inf-norm.

  -- ALGLIB routine --
     05.02.2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixluinverse(/* Real    */ ae_matrix* a,
     /* Integer */ ae_vector* pivots,
     ae_int_t n,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    sinteger sinfo;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _matinvreport_clear(rep);
    ae_vector_init(&work, 0, DT_REAL, _state);
    _sinteger_init(&sinfo, _state);

    ae_assert(n>0, "RMatrixLUInverse: N<=0!", _state);
    ae_assert(a->cols>=n, "RMatrixLUInverse: cols(A)<N!", _state);
    ae_assert(a->rows>=n, "RMatrixLUInverse: rows(A)<N!", _state);
    ae_assert(pivots->cnt>=n, "RMatrixLUInverse: len(Pivots)<N!", _state);
    ae_assert(apservisfinitematrix(a, n, n, _state), "RMatrixLUInverse: A contains infinite or NaN values!", _state);
    *info = 1;
    for(i=0; i<=n-1; i++)
    {
        if( pivots->ptr.p_int[i]>n-1||pivots->ptr.p_int[i]<i )
        {
            *info = -1;
        }
    }
    ae_assert(*info>0, "RMatrixLUInverse: incorrect Pivots array!", _state);
    
    /*
     * calculate condition numbers
     */
    rep->r1 = rmatrixlurcond1(a, n, _state);
    rep->rinf = rmatrixlurcondinf(a, n, _state);
    if( ae_fp_less(rep->r1,rcondthreshold(_state))||ae_fp_less(rep->rinf,rcondthreshold(_state)) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a->ptr.pp_double[i][j] = (double)(0);
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Call cache-oblivious code
     */
    ae_vector_set_length(&work, n, _state);
    sinfo.val = 1;
    matinv_rmatrixluinverserec(a, 0, n, &work, &sinfo, rep, _state);
    *info = sinfo.val;
    
    /*
     * apply permutations
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=n-2; j>=0; j--)
        {
            k = pivots->ptr.p_int[j];
            v = a->ptr.pp_double[i][j];
            a->ptr.pp_double[i][j] = a->ptr.pp_double[i][k];
            a->ptr.pp_double[i][k] = v;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixluinverse(/* Real    */ ae_matrix* a,
    /* Integer */ ae_vector* pivots,
    ae_int_t n,
    ae_int_t* info,
    matinvreport* rep, ae_state *_state)
{
    rmatrixluinverse(a,pivots,n,info,rep, _state);
}


/*************************************************************************
Inversion of a general matrix.

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
  ! of this function. We should note that matrix inversion  is  harder  to
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

Input parameters:
    A       -   matrix.
    N       -   size of matrix A (optional) :
                * if given, only principal NxN submatrix is processed  and
                  overwritten. other elements are unchanged.
                * if not given,  size  is  automatically  determined  from
                  matrix size (A must be square matrix)

Output parameters:
    Info    -   return code, same as in RMatrixLUInverse
    Rep     -   solver report, same as in RMatrixLUInverse
    A       -   inverse of matrix A, same as in RMatrixLUInverse

Result:
    True, if the matrix is not singular.
    False, if the matrix is singular.

  -- ALGLIB --
     Copyright 2005-2010 by Bochkanov Sergey
*************************************************************************/
void rmatrixinverse(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector pivots;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _matinvreport_clear(rep);
    ae_vector_init(&pivots, 0, DT_INT, _state);

    ae_assert(n>0, "RMatrixInverse: N<=0!", _state);
    ae_assert(a->cols>=n, "RMatrixInverse: cols(A)<N!", _state);
    ae_assert(a->rows>=n, "RMatrixInverse: rows(A)<N!", _state);
    ae_assert(apservisfinitematrix(a, n, n, _state), "RMatrixInverse: A contains infinite or NaN values!", _state);
    rmatrixlu(a, n, n, &pivots, _state);
    rmatrixluinverse(a, &pivots, n, info, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixinverse(/* Real    */ ae_matrix* a,
    ae_int_t n,
    ae_int_t* info,
    matinvreport* rep, ae_state *_state)
{
    rmatrixinverse(a,n,info,rep, _state);
}


/*************************************************************************
Inversion of a matrix given by its LU decomposition.

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
  ! of this function. We should note that matrix inversion  is  harder  to
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
    A       -   LU decomposition of the matrix
                (output of CMatrixLU subroutine).
    Pivots  -   table of permutations
                (the output of CMatrixLU subroutine).
    N       -   size of matrix A (optional) :
                * if given, only principal NxN submatrix is processed  and
                  overwritten. other elements are unchanged.
                * if not given,  size  is  automatically  determined  from
                  matrix size (A must be square matrix)

OUTPUT PARAMETERS:
    Info    -   return code, same as in RMatrixLUInverse
    Rep     -   solver report, same as in RMatrixLUInverse
    A       -   inverse of matrix A, same as in RMatrixLUInverse

  -- ALGLIB routine --
     05.02.2010
     Bochkanov Sergey
*************************************************************************/
void cmatrixluinverse(/* Complex */ ae_matrix* a,
     /* Integer */ ae_vector* pivots,
     ae_int_t n,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_complex v;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _matinvreport_clear(rep);
    ae_vector_init(&work, 0, DT_COMPLEX, _state);

    ae_assert(n>0, "CMatrixLUInverse: N<=0!", _state);
    ae_assert(a->cols>=n, "CMatrixLUInverse: cols(A)<N!", _state);
    ae_assert(a->rows>=n, "CMatrixLUInverse: rows(A)<N!", _state);
    ae_assert(pivots->cnt>=n, "CMatrixLUInverse: len(Pivots)<N!", _state);
    ae_assert(apservisfinitecmatrix(a, n, n, _state), "CMatrixLUInverse: A contains infinite or NaN values!", _state);
    *info = 1;
    for(i=0; i<=n-1; i++)
    {
        if( pivots->ptr.p_int[i]>n-1||pivots->ptr.p_int[i]<i )
        {
            *info = -1;
        }
    }
    ae_assert(*info>0, "CMatrixLUInverse: incorrect Pivots array!", _state);
    
    /*
     * calculate condition numbers
     */
    rep->r1 = cmatrixlurcond1(a, n, _state);
    rep->rinf = cmatrixlurcondinf(a, n, _state);
    if( ae_fp_less(rep->r1,rcondthreshold(_state))||ae_fp_less(rep->rinf,rcondthreshold(_state)) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a->ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Call cache-oblivious code
     */
    ae_vector_set_length(&work, n, _state);
    matinv_cmatrixluinverserec(a, 0, n, &work, info, rep, _state);
    
    /*
     * apply permutations
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=n-2; j>=0; j--)
        {
            k = pivots->ptr.p_int[j];
            v = a->ptr.pp_complex[i][j];
            a->ptr.pp_complex[i][j] = a->ptr.pp_complex[i][k];
            a->ptr.pp_complex[i][k] = v;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixluinverse(/* Complex */ ae_matrix* a,
    /* Integer */ ae_vector* pivots,
    ae_int_t n,
    ae_int_t* info,
    matinvreport* rep, ae_state *_state)
{
    cmatrixluinverse(a,pivots,n,info,rep, _state);
}


/*************************************************************************
Inversion of a general matrix.

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
  ! of this function. We should note that matrix inversion  is  harder  to
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

Input parameters:
    A       -   matrix
    N       -   size of matrix A (optional) :
                * if given, only principal NxN submatrix is processed  and
                  overwritten. other elements are unchanged.
                * if not given,  size  is  automatically  determined  from
                  matrix size (A must be square matrix)

Output parameters:
    Info    -   return code, same as in RMatrixLUInverse
    Rep     -   solver report, same as in RMatrixLUInverse
    A       -   inverse of matrix A, same as in RMatrixLUInverse

  -- ALGLIB --
     Copyright 2005 by Bochkanov Sergey
*************************************************************************/
void cmatrixinverse(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector pivots;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _matinvreport_clear(rep);
    ae_vector_init(&pivots, 0, DT_INT, _state);

    ae_assert(n>0, "CRMatrixInverse: N<=0!", _state);
    ae_assert(a->cols>=n, "CRMatrixInverse: cols(A)<N!", _state);
    ae_assert(a->rows>=n, "CRMatrixInverse: rows(A)<N!", _state);
    ae_assert(apservisfinitecmatrix(a, n, n, _state), "CMatrixInverse: A contains infinite or NaN values!", _state);
    cmatrixlu(a, n, n, &pivots, _state);
    cmatrixluinverse(a, &pivots, n, info, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixinverse(/* Complex */ ae_matrix* a,
    ae_int_t n,
    ae_int_t* info,
    matinvreport* rep, ae_state *_state)
{
    cmatrixinverse(a,n,info,rep, _state);
}


/*************************************************************************
Inversion of a symmetric positive definite matrix which is given
by Cholesky decomposition.

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
  ! of  this  function.  However,  Cholesky  inversion  is  a  "difficult"
  ! algorithm  -  it  has  lots  of  internal synchronization points which
  ! prevents efficient  parallelization  of  algorithm.  Only  very  large
  ! problems (N=thousands) can be efficiently parallelized.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

Input parameters:
    A       -   Cholesky decomposition of the matrix to be inverted:
                A=U’*U or A = L*L'.
                Output of  SPDMatrixCholesky subroutine.
    N       -   size of matrix A (optional) :
                * if given, only principal NxN submatrix is processed  and
                  overwritten. other elements are unchanged.
                * if not given,  size  is  automatically  determined  from
                  matrix size (A must be square matrix)
    IsUpper -   storage type (optional):
                * if True, symmetric  matrix  A  is  given  by  its  upper
                  triangle, and the lower triangle isn’t  used/changed  by
                  function
                * if False,  symmetric matrix  A  is  given  by  its lower
                  triangle, and the  upper triangle isn’t used/changed  by
                  function
                * if not given, lower half is used.

Output parameters:
    Info    -   return code, same as in RMatrixLUInverse
    Rep     -   solver report, same as in RMatrixLUInverse
    A       -   inverse of matrix A, same as in RMatrixLUInverse

  -- ALGLIB routine --
     10.02.2010
     Bochkanov Sergey
*************************************************************************/
void spdmatrixcholeskyinverse(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector tmp;
    matinvreport rep2;
    ae_bool f;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _matinvreport_clear(rep);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    _matinvreport_init(&rep2, _state);

    ae_assert(n>0, "SPDMatrixCholeskyInverse: N<=0!", _state);
    ae_assert(a->cols>=n, "SPDMatrixCholeskyInverse: cols(A)<N!", _state);
    ae_assert(a->rows>=n, "SPDMatrixCholeskyInverse: rows(A)<N!", _state);
    *info = 1;
    f = ae_true;
    for(i=0; i<=n-1; i++)
    {
        f = f&&ae_isfinite(a->ptr.pp_double[i][i], _state);
    }
    ae_assert(f, "SPDMatrixCholeskyInverse: A contains infinite or NaN values!", _state);
    
    /*
     * calculate condition numbers
     */
    rep->r1 = spdmatrixcholeskyrcond(a, n, isupper, _state);
    rep->rinf = rep->r1;
    if( ae_fp_less(rep->r1,rcondthreshold(_state))||ae_fp_less(rep->rinf,rcondthreshold(_state)) )
    {
        if( isupper )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=i; j<=n-1; j++)
                {
                    a->ptr.pp_double[i][j] = (double)(0);
                }
            }
        }
        else
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i; j++)
                {
                    a->ptr.pp_double[i][j] = (double)(0);
                }
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Inverse
     */
    ae_vector_set_length(&tmp, n, _state);
    spdmatrixcholeskyinverserec(a, 0, n, isupper, &tmp, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spdmatrixcholeskyinverse(/* Real    */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    ae_int_t* info,
    matinvreport* rep, ae_state *_state)
{
    spdmatrixcholeskyinverse(a,n,isupper,info,rep, _state);
}


/*************************************************************************
Inversion of a symmetric positive definite matrix.

Given an upper or lower triangle of a symmetric positive definite matrix,
the algorithm generates matrix A^-1 and saves the upper or lower triangle
depending on the input.

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
  ! of  this  function.  However,  Cholesky  inversion  is  a  "difficult"
  ! algorithm  -  it  has  lots  of  internal synchronization points which
  ! prevents efficient  parallelization  of  algorithm.  Only  very  large
  ! problems (N=thousands) can be efficiently parallelized.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

Input parameters:
    A       -   matrix to be inverted (upper or lower triangle).
                Array with elements [0..N-1,0..N-1].
    N       -   size of matrix A (optional) :
                * if given, only principal NxN submatrix is processed  and
                  overwritten. other elements are unchanged.
                * if not given,  size  is  automatically  determined  from
                  matrix size (A must be square matrix)
    IsUpper -   storage type (optional):
                * if True, symmetric  matrix  A  is  given  by  its  upper
                  triangle, and the lower triangle isn’t  used/changed  by
                  function
                * if False,  symmetric matrix  A  is  given  by  its lower
                  triangle, and the  upper triangle isn’t used/changed  by
                  function
                * if not given,  both lower and upper  triangles  must  be
                  filled.

Output parameters:
    Info    -   return code, same as in RMatrixLUInverse
    Rep     -   solver report, same as in RMatrixLUInverse
    A       -   inverse of matrix A, same as in RMatrixLUInverse

  -- ALGLIB routine --
     10.02.2010
     Bochkanov Sergey
*************************************************************************/
void spdmatrixinverse(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{

    *info = 0;
    _matinvreport_clear(rep);

    ae_assert(n>0, "SPDMatrixInverse: N<=0!", _state);
    ae_assert(a->cols>=n, "SPDMatrixInverse: cols(A)<N!", _state);
    ae_assert(a->rows>=n, "SPDMatrixInverse: rows(A)<N!", _state);
    ae_assert(isfinitertrmatrix(a, n, isupper, _state), "SPDMatrixInverse: A contains infinite or NaN values!", _state);
    *info = 1;
    if( spdmatrixcholesky(a, n, isupper, _state) )
    {
        spdmatrixcholeskyinverse(a, n, isupper, info, rep, _state);
    }
    else
    {
        *info = -3;
    }
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spdmatrixinverse(/* Real    */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    ae_int_t* info,
    matinvreport* rep, ae_state *_state)
{
    spdmatrixinverse(a,n,isupper,info,rep, _state);
}


/*************************************************************************
Inversion of a Hermitian positive definite matrix which is given
by Cholesky decomposition.

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
  ! of  this  function.  However,  Cholesky  inversion  is  a  "difficult"
  ! algorithm  -  it  has  lots  of  internal synchronization points which
  ! prevents efficient  parallelization  of  algorithm.  Only  very  large
  ! problems (N=thousands) can be efficiently parallelized.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

Input parameters:
    A       -   Cholesky decomposition of the matrix to be inverted:
                A=U’*U or A = L*L'.
                Output of  HPDMatrixCholesky subroutine.
    N       -   size of matrix A (optional) :
                * if given, only principal NxN submatrix is processed  and
                  overwritten. other elements are unchanged.
                * if not given,  size  is  automatically  determined  from
                  matrix size (A must be square matrix)
    IsUpper -   storage type (optional):
                * if True, symmetric  matrix  A  is  given  by  its  upper
                  triangle, and the lower triangle isn’t  used/changed  by
                  function
                * if False,  symmetric matrix  A  is  given  by  its lower
                  triangle, and the  upper triangle isn’t used/changed  by
                  function
                * if not given, lower half is used.

Output parameters:
    Info    -   return code, same as in RMatrixLUInverse
    Rep     -   solver report, same as in RMatrixLUInverse
    A       -   inverse of matrix A, same as in RMatrixLUInverse

  -- ALGLIB routine --
     10.02.2010
     Bochkanov Sergey
*************************************************************************/
void hpdmatrixcholeskyinverse(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    matinvreport rep2;
    ae_vector tmp;
    ae_bool f;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _matinvreport_clear(rep);
    _matinvreport_init(&rep2, _state);
    ae_vector_init(&tmp, 0, DT_COMPLEX, _state);

    ae_assert(n>0, "HPDMatrixCholeskyInverse: N<=0!", _state);
    ae_assert(a->cols>=n, "HPDMatrixCholeskyInverse: cols(A)<N!", _state);
    ae_assert(a->rows>=n, "HPDMatrixCholeskyInverse: rows(A)<N!", _state);
    f = ae_true;
    for(i=0; i<=n-1; i++)
    {
        f = (f&&ae_isfinite(a->ptr.pp_complex[i][i].x, _state))&&ae_isfinite(a->ptr.pp_complex[i][i].y, _state);
    }
    ae_assert(f, "HPDMatrixCholeskyInverse: A contains infinite or NaN values!", _state);
    *info = 1;
    
    /*
     * calculate condition numbers
     */
    rep->r1 = hpdmatrixcholeskyrcond(a, n, isupper, _state);
    rep->rinf = rep->r1;
    if( ae_fp_less(rep->r1,rcondthreshold(_state))||ae_fp_less(rep->rinf,rcondthreshold(_state)) )
    {
        if( isupper )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=i; j<=n-1; j++)
                {
                    a->ptr.pp_complex[i][j] = ae_complex_from_i(0);
                }
            }
        }
        else
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i; j++)
                {
                    a->ptr.pp_complex[i][j] = ae_complex_from_i(0);
                }
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Inverse
     */
    ae_vector_set_length(&tmp, n, _state);
    matinv_hpdmatrixcholeskyinverserec(a, 0, n, isupper, &tmp, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_hpdmatrixcholeskyinverse(/* Complex */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    ae_int_t* info,
    matinvreport* rep, ae_state *_state)
{
    hpdmatrixcholeskyinverse(a,n,isupper,info,rep, _state);
}


/*************************************************************************
Inversion of a Hermitian positive definite matrix.

Given an upper or lower triangle of a Hermitian positive definite matrix,
the algorithm generates matrix A^-1 and saves the upper or lower triangle
depending on the input.

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
  ! of  this  function.  However,  Cholesky  inversion  is  a  "difficult"
  ! algorithm  -  it  has  lots  of  internal synchronization points which
  ! prevents efficient  parallelization  of  algorithm.  Only  very  large
  ! problems (N=thousands) can be efficiently parallelized.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.
  
Input parameters:
    A       -   matrix to be inverted (upper or lower triangle).
                Array with elements [0..N-1,0..N-1].
    N       -   size of matrix A (optional) :
                * if given, only principal NxN submatrix is processed  and
                  overwritten. other elements are unchanged.
                * if not given,  size  is  automatically  determined  from
                  matrix size (A must be square matrix)
    IsUpper -   storage type (optional):
                * if True, symmetric  matrix  A  is  given  by  its  upper
                  triangle, and the lower triangle isn’t  used/changed  by
                  function
                * if False,  symmetric matrix  A  is  given  by  its lower
                  triangle, and the  upper triangle isn’t used/changed  by
                  function
                * if not given,  both lower and upper  triangles  must  be
                  filled.

Output parameters:
    Info    -   return code, same as in RMatrixLUInverse
    Rep     -   solver report, same as in RMatrixLUInverse
    A       -   inverse of matrix A, same as in RMatrixLUInverse

  -- ALGLIB routine --
     10.02.2010
     Bochkanov Sergey
*************************************************************************/
void hpdmatrixinverse(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{

    *info = 0;
    _matinvreport_clear(rep);

    ae_assert(n>0, "HPDMatrixInverse: N<=0!", _state);
    ae_assert(a->cols>=n, "HPDMatrixInverse: cols(A)<N!", _state);
    ae_assert(a->rows>=n, "HPDMatrixInverse: rows(A)<N!", _state);
    ae_assert(apservisfinitectrmatrix(a, n, isupper, _state), "HPDMatrixInverse: A contains infinite or NaN values!", _state);
    *info = 1;
    if( hpdmatrixcholesky(a, n, isupper, _state) )
    {
        hpdmatrixcholeskyinverse(a, n, isupper, info, rep, _state);
    }
    else
    {
        *info = -3;
    }
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_hpdmatrixinverse(/* Complex */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    ae_int_t* info,
    matinvreport* rep, ae_state *_state)
{
    hpdmatrixinverse(a,n,isupper,info,rep, _state);
}


/*************************************************************************
Triangular matrix inverse (real)

The subroutine inverts the following types of matrices:
    * upper triangular
    * upper triangular with unit diagonal
    * lower triangular
    * lower triangular with unit diagonal

In case of an upper (lower) triangular matrix,  the  inverse  matrix  will
also be upper (lower) triangular, and after the end of the algorithm,  the
inverse matrix replaces the source matrix. The elements  below (above) the
main diagonal are not changed by the algorithm.

If  the matrix  has a unit diagonal, the inverse matrix also  has  a  unit
diagonal, and the diagonal elements are not passed to the algorithm.

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
  ! of this function. We should note that triangular inverse is harder  to
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
  
Input parameters:
    A       -   matrix, array[0..N-1, 0..N-1].
    N       -   size of matrix A (optional) :
                * if given, only principal NxN submatrix is processed  and
                  overwritten. other elements are unchanged.
                * if not given,  size  is  automatically  determined  from
                  matrix size (A must be square matrix)
    IsUpper -   True, if the matrix is upper triangular.
    IsUnit  -   diagonal type (optional):
                * if True, matrix has unit diagonal (a[i,i] are NOT used)
                * if False, matrix diagonal is arbitrary
                * if not given, False is assumed

Output parameters:
    Info    -   same as for RMatrixLUInverse
    Rep     -   same as for RMatrixLUInverse
    A       -   same as for RMatrixLUInverse.

  -- ALGLIB --
     Copyright 05.02.2010 by Bochkanov Sergey
*************************************************************************/
void rmatrixtrinverse(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector tmp;
    sinteger sinfo;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _matinvreport_clear(rep);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    _sinteger_init(&sinfo, _state);

    ae_assert(n>0, "RMatrixTRInverse: N<=0!", _state);
    ae_assert(a->cols>=n, "RMatrixTRInverse: cols(A)<N!", _state);
    ae_assert(a->rows>=n, "RMatrixTRInverse: rows(A)<N!", _state);
    ae_assert(isfinitertrmatrix(a, n, isupper, _state), "RMatrixTRInverse: A contains infinite or NaN values!", _state);
    
    /*
     * calculate condition numbers
     */
    rep->r1 = rmatrixtrrcond1(a, n, isupper, isunit, _state);
    rep->rinf = rmatrixtrrcondinf(a, n, isupper, isunit, _state);
    if( ae_fp_less(rep->r1,rcondthreshold(_state))||ae_fp_less(rep->rinf,rcondthreshold(_state)) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a->ptr.pp_double[i][j] = (double)(0);
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Invert
     */
    ae_vector_set_length(&tmp, n, _state);
    sinfo.val = 1;
    matinv_rmatrixtrinverserec(a, 0, n, isupper, isunit, &tmp, &sinfo, rep, _state);
    *info = sinfo.val;
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixtrinverse(/* Real    */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    ae_bool isunit,
    ae_int_t* info,
    matinvreport* rep, ae_state *_state)
{
    rmatrixtrinverse(a,n,isupper,isunit,info,rep, _state);
}


/*************************************************************************
Triangular matrix inverse (complex)

The subroutine inverts the following types of matrices:
    * upper triangular
    * upper triangular with unit diagonal
    * lower triangular
    * lower triangular with unit diagonal

In case of an upper (lower) triangular matrix,  the  inverse  matrix  will
also be upper (lower) triangular, and after the end of the algorithm,  the
inverse matrix replaces the source matrix. The elements  below (above) the
main diagonal are not changed by the algorithm.

If  the matrix  has a unit diagonal, the inverse matrix also  has  a  unit
diagonal, and the diagonal elements are not passed to the algorithm.

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
  ! of this function. We should note that triangular inverse is harder  to
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

Input parameters:
    A       -   matrix, array[0..N-1, 0..N-1].
    N       -   size of matrix A (optional) :
                * if given, only principal NxN submatrix is processed  and
                  overwritten. other elements are unchanged.
                * if not given,  size  is  automatically  determined  from
                  matrix size (A must be square matrix)
    IsUpper -   True, if the matrix is upper triangular.
    IsUnit  -   diagonal type (optional):
                * if True, matrix has unit diagonal (a[i,i] are NOT used)
                * if False, matrix diagonal is arbitrary
                * if not given, False is assumed

Output parameters:
    Info    -   same as for RMatrixLUInverse
    Rep     -   same as for RMatrixLUInverse
    A       -   same as for RMatrixLUInverse.

  -- ALGLIB --
     Copyright 05.02.2010 by Bochkanov Sergey
*************************************************************************/
void cmatrixtrinverse(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector tmp;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _matinvreport_clear(rep);
    ae_vector_init(&tmp, 0, DT_COMPLEX, _state);

    ae_assert(n>0, "CMatrixTRInverse: N<=0!", _state);
    ae_assert(a->cols>=n, "CMatrixTRInverse: cols(A)<N!", _state);
    ae_assert(a->rows>=n, "CMatrixTRInverse: rows(A)<N!", _state);
    ae_assert(apservisfinitectrmatrix(a, n, isupper, _state), "CMatrixTRInverse: A contains infinite or NaN values!", _state);
    *info = 1;
    
    /*
     * calculate condition numbers
     */
    rep->r1 = cmatrixtrrcond1(a, n, isupper, isunit, _state);
    rep->rinf = cmatrixtrrcondinf(a, n, isupper, isunit, _state);
    if( ae_fp_less(rep->r1,rcondthreshold(_state))||ae_fp_less(rep->rinf,rcondthreshold(_state)) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a->ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
        rep->r1 = (double)(0);
        rep->rinf = (double)(0);
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Invert
     */
    ae_vector_set_length(&tmp, n, _state);
    matinv_cmatrixtrinverserec(a, 0, n, isupper, isunit, &tmp, info, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixtrinverse(/* Complex */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper,
    ae_bool isunit,
    ae_int_t* info,
    matinvreport* rep, ae_state *_state)
{
    cmatrixtrinverse(a,n,isupper,isunit,info,rep, _state);
}


/*************************************************************************
Recursive subroutine for SPD inversion.

NOTE: this function expects that matris is strictly positive-definite.

  -- ALGLIB routine --
     10.02.2010
     Bochkanov Sergey
*************************************************************************/
void spdmatrixcholeskyinverserec(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_int_t n1;
    ae_int_t n2;
    sinteger sinfo2;
    matinvreport rep2;

    ae_frame_make(_state, &_frame_block);
    _sinteger_init(&sinfo2, _state);
    _matinvreport_init(&rep2, _state);

    if( n<1 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Base case
     */
    if( n<=ablasblocksize(a, _state) )
    {
        sinfo2.val = 1;
        matinv_rmatrixtrinverserec(a, offs, n, isupper, ae_false, tmp, &sinfo2, &rep2, _state);
        ae_assert(sinfo2.val>0, "SPDMatrixCholeskyInverseRec: integrity check failed", _state);
        if( isupper )
        {
            
            /*
             * Compute the product U * U'.
             * NOTE: we never assume that diagonal of U is real
             */
            for(i=0; i<=n-1; i++)
            {
                if( i==0 )
                {
                    
                    /*
                     * 1x1 matrix
                     */
                    a->ptr.pp_double[offs+i][offs+i] = ae_sqr(a->ptr.pp_double[offs+i][offs+i], _state);
                }
                else
                {
                    
                    /*
                     * (I+1)x(I+1) matrix,
                     *
                     * ( A11  A12 )   ( A11^H        )   ( A11*A11^H+A12*A12^H  A12*A22^H )
                     * (          ) * (              ) = (                                )
                     * (      A22 )   ( A12^H  A22^H )   ( A22*A12^H            A22*A22^H )
                     *
                     * A11 is IxI, A22 is 1x1.
                     */
                    ae_v_move(&tmp->ptr.p_double[0], 1, &a->ptr.pp_double[offs][offs+i], a->stride, ae_v_len(0,i-1));
                    for(j=0; j<=i-1; j++)
                    {
                        v = a->ptr.pp_double[offs+j][offs+i];
                        ae_v_addd(&a->ptr.pp_double[offs+j][offs+j], 1, &tmp->ptr.p_double[j], 1, ae_v_len(offs+j,offs+i-1), v);
                    }
                    v = a->ptr.pp_double[offs+i][offs+i];
                    ae_v_muld(&a->ptr.pp_double[offs][offs+i], a->stride, ae_v_len(offs,offs+i-1), v);
                    a->ptr.pp_double[offs+i][offs+i] = ae_sqr(a->ptr.pp_double[offs+i][offs+i], _state);
                }
            }
        }
        else
        {
            
            /*
             * Compute the product L' * L
             * NOTE: we never assume that diagonal of L is real
             */
            for(i=0; i<=n-1; i++)
            {
                if( i==0 )
                {
                    
                    /*
                     * 1x1 matrix
                     */
                    a->ptr.pp_double[offs+i][offs+i] = ae_sqr(a->ptr.pp_double[offs+i][offs+i], _state);
                }
                else
                {
                    
                    /*
                     * (I+1)x(I+1) matrix,
                     *
                     * ( A11^H  A21^H )   ( A11      )   ( A11^H*A11+A21^H*A21  A21^H*A22 )
                     * (              ) * (          ) = (                                )
                     * (        A22^H )   ( A21  A22 )   ( A22^H*A21            A22^H*A22 )
                     *
                     * A11 is IxI, A22 is 1x1.
                     */
                    ae_v_move(&tmp->ptr.p_double[0], 1, &a->ptr.pp_double[offs+i][offs], 1, ae_v_len(0,i-1));
                    for(j=0; j<=i-1; j++)
                    {
                        v = a->ptr.pp_double[offs+i][offs+j];
                        ae_v_addd(&a->ptr.pp_double[offs+j][offs], 1, &tmp->ptr.p_double[0], 1, ae_v_len(offs,offs+j), v);
                    }
                    v = a->ptr.pp_double[offs+i][offs+i];
                    ae_v_muld(&a->ptr.pp_double[offs+i][offs], 1, ae_v_len(offs,offs+i-1), v);
                    a->ptr.pp_double[offs+i][offs+i] = ae_sqr(a->ptr.pp_double[offs+i][offs+i], _state);
                }
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Recursive code: triangular factor inversion merged with
     * UU' or L'L multiplication
     */
    ablassplitlength(a, n, &n1, &n2, _state);
    
    /*
     * form off-diagonal block of trangular inverse
     */
    if( isupper )
    {
        for(i=0; i<=n1-1; i++)
        {
            ae_v_muld(&a->ptr.pp_double[offs+i][offs+n1], 1, ae_v_len(offs+n1,offs+n-1), -1);
        }
        rmatrixlefttrsm(n1, n2, a, offs, offs, isupper, ae_false, 0, a, offs, offs+n1, _state);
        rmatrixrighttrsm(n1, n2, a, offs+n1, offs+n1, isupper, ae_false, 0, a, offs, offs+n1, _state);
    }
    else
    {
        for(i=0; i<=n2-1; i++)
        {
            ae_v_muld(&a->ptr.pp_double[offs+n1+i][offs], 1, ae_v_len(offs,offs+n1-1), -1);
        }
        rmatrixrighttrsm(n2, n1, a, offs, offs, isupper, ae_false, 0, a, offs+n1, offs, _state);
        rmatrixlefttrsm(n2, n1, a, offs+n1, offs+n1, isupper, ae_false, 0, a, offs+n1, offs, _state);
    }
    
    /*
     * invert first diagonal block
     */
    spdmatrixcholeskyinverserec(a, offs, n1, isupper, tmp, _state);
    
    /*
     * update first diagonal block with off-diagonal block,
     * update off-diagonal block
     */
    if( isupper )
    {
        rmatrixsyrk(n1, n2, 1.0, a, offs, offs+n1, 0, 1.0, a, offs, offs, isupper, _state);
        rmatrixrighttrsm(n1, n2, a, offs+n1, offs+n1, isupper, ae_false, 1, a, offs, offs+n1, _state);
    }
    else
    {
        rmatrixsyrk(n1, n2, 1.0, a, offs+n1, offs, 1, 1.0, a, offs, offs, isupper, _state);
        rmatrixlefttrsm(n2, n1, a, offs+n1, offs+n1, isupper, ae_false, 1, a, offs+n1, offs, _state);
    }
    
    /*
     * invert second diagonal block
     */
    spdmatrixcholeskyinverserec(a, offs+n1, n2, isupper, tmp, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Triangular matrix inversion, recursive subroutine

NOTE: this function sets Info on failure, leaves it unchanged on success.

NOTE: only Tmp[Offs:Offs+N-1] is modified, other entries of the temporary array are not modified

  -- ALGLIB --
     05.02.2010, Bochkanov Sergey.
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     February 29, 1992.
*************************************************************************/
static void matinv_rmatrixtrinverserec(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     /* Real    */ ae_vector* tmp,
     sinteger* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t i;
    ae_int_t j;
    double v;
    double ajj;


    if( n<1 )
    {
        info->val = -1;
        return;
    }
    
    /*
     * Base case
     */
    if( n<=ablasblocksize(a, _state) )
    {
        if( isupper )
        {
            
            /*
             * Compute inverse of upper triangular matrix.
             */
            for(j=0; j<=n-1; j++)
            {
                if( !isunit )
                {
                    if( ae_fp_eq(a->ptr.pp_double[offs+j][offs+j],(double)(0)) )
                    {
                        info->val = -3;
                        return;
                    }
                    a->ptr.pp_double[offs+j][offs+j] = 1/a->ptr.pp_double[offs+j][offs+j];
                    ajj = -a->ptr.pp_double[offs+j][offs+j];
                }
                else
                {
                    ajj = (double)(-1);
                }
                
                /*
                 * Compute elements 1:j-1 of j-th column.
                 */
                if( j>0 )
                {
                    ae_v_move(&tmp->ptr.p_double[offs+0], 1, &a->ptr.pp_double[offs+0][offs+j], a->stride, ae_v_len(offs+0,offs+j-1));
                    for(i=0; i<=j-1; i++)
                    {
                        if( i<j-1 )
                        {
                            v = ae_v_dotproduct(&a->ptr.pp_double[offs+i][offs+i+1], 1, &tmp->ptr.p_double[offs+i+1], 1, ae_v_len(offs+i+1,offs+j-1));
                        }
                        else
                        {
                            v = (double)(0);
                        }
                        if( !isunit )
                        {
                            a->ptr.pp_double[offs+i][offs+j] = v+a->ptr.pp_double[offs+i][offs+i]*tmp->ptr.p_double[offs+i];
                        }
                        else
                        {
                            a->ptr.pp_double[offs+i][offs+j] = v+tmp->ptr.p_double[offs+i];
                        }
                    }
                    ae_v_muld(&a->ptr.pp_double[offs+0][offs+j], a->stride, ae_v_len(offs+0,offs+j-1), ajj);
                }
            }
        }
        else
        {
            
            /*
             * Compute inverse of lower triangular matrix.
             */
            for(j=n-1; j>=0; j--)
            {
                if( !isunit )
                {
                    if( ae_fp_eq(a->ptr.pp_double[offs+j][offs+j],(double)(0)) )
                    {
                        info->val = -3;
                        return;
                    }
                    a->ptr.pp_double[offs+j][offs+j] = 1/a->ptr.pp_double[offs+j][offs+j];
                    ajj = -a->ptr.pp_double[offs+j][offs+j];
                }
                else
                {
                    ajj = (double)(-1);
                }
                if( j<n-1 )
                {
                    
                    /*
                     * Compute elements j+1:n of j-th column.
                     */
                    ae_v_move(&tmp->ptr.p_double[offs+j+1], 1, &a->ptr.pp_double[offs+j+1][offs+j], a->stride, ae_v_len(offs+j+1,offs+n-1));
                    for(i=j+1; i<=n-1; i++)
                    {
                        if( i>j+1 )
                        {
                            v = ae_v_dotproduct(&a->ptr.pp_double[offs+i][offs+j+1], 1, &tmp->ptr.p_double[offs+j+1], 1, ae_v_len(offs+j+1,offs+i-1));
                        }
                        else
                        {
                            v = (double)(0);
                        }
                        if( !isunit )
                        {
                            a->ptr.pp_double[offs+i][offs+j] = v+a->ptr.pp_double[offs+i][offs+i]*tmp->ptr.p_double[offs+i];
                        }
                        else
                        {
                            a->ptr.pp_double[offs+i][offs+j] = v+tmp->ptr.p_double[offs+i];
                        }
                    }
                    ae_v_muld(&a->ptr.pp_double[offs+j+1][offs+j], a->stride, ae_v_len(offs+j+1,offs+n-1), ajj);
                }
            }
        }
        return;
    }
    
    /*
     * Recursive case
     */
    ablassplitlength(a, n, &n1, &n2, _state);
    if( n2>0 )
    {
        if( isupper )
        {
            for(i=0; i<=n1-1; i++)
            {
                ae_v_muld(&a->ptr.pp_double[offs+i][offs+n1], 1, ae_v_len(offs+n1,offs+n-1), -1);
            }
            rmatrixrighttrsm(n1, n2, a, offs+n1, offs+n1, isupper, isunit, 0, a, offs, offs+n1, _state);
            rmatrixlefttrsm(n1, n2, a, offs, offs, isupper, isunit, 0, a, offs, offs+n1, _state);
            matinv_rmatrixtrinverserec(a, offs+n1, n2, isupper, isunit, tmp, info, rep, _state);
        }
        else
        {
            for(i=0; i<=n2-1; i++)
            {
                ae_v_muld(&a->ptr.pp_double[offs+n1+i][offs], 1, ae_v_len(offs,offs+n1-1), -1);
            }
            rmatrixlefttrsm(n2, n1, a, offs+n1, offs+n1, isupper, isunit, 0, a, offs+n1, offs, _state);
            rmatrixrighttrsm(n2, n1, a, offs, offs, isupper, isunit, 0, a, offs+n1, offs, _state);
            matinv_rmatrixtrinverserec(a, offs+n1, n2, isupper, isunit, tmp, info, rep, _state);
        }
    }
    matinv_rmatrixtrinverserec(a, offs, n1, isupper, isunit, tmp, info, rep, _state);
}


/*************************************************************************
Triangular matrix inversion, recursive subroutine

  -- ALGLIB --
     05.02.2010, Bochkanov Sergey.
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     February 29, 1992.
*************************************************************************/
static void matinv_cmatrixtrinverserec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     /* Complex */ ae_vector* tmp,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t i;
    ae_int_t j;
    ae_complex v;
    ae_complex ajj;


    if( n<1 )
    {
        *info = -1;
        return;
    }
    
    /*
     * Base case
     */
    if( n<=ablascomplexblocksize(a, _state) )
    {
        if( isupper )
        {
            
            /*
             * Compute inverse of upper triangular matrix.
             */
            for(j=0; j<=n-1; j++)
            {
                if( !isunit )
                {
                    if( ae_c_eq_d(a->ptr.pp_complex[offs+j][offs+j],(double)(0)) )
                    {
                        *info = -3;
                        return;
                    }
                    a->ptr.pp_complex[offs+j][offs+j] = ae_c_d_div(1,a->ptr.pp_complex[offs+j][offs+j]);
                    ajj = ae_c_neg(a->ptr.pp_complex[offs+j][offs+j]);
                }
                else
                {
                    ajj = ae_complex_from_i(-1);
                }
                
                /*
                 * Compute elements 1:j-1 of j-th column.
                 */
                if( j>0 )
                {
                    ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs+0][offs+j], a->stride, "N", ae_v_len(0,j-1));
                    for(i=0; i<=j-1; i++)
                    {
                        if( i<j-1 )
                        {
                            v = ae_v_cdotproduct(&a->ptr.pp_complex[offs+i][offs+i+1], 1, "N", &tmp->ptr.p_complex[i+1], 1, "N", ae_v_len(offs+i+1,offs+j-1));
                        }
                        else
                        {
                            v = ae_complex_from_i(0);
                        }
                        if( !isunit )
                        {
                            a->ptr.pp_complex[offs+i][offs+j] = ae_c_add(v,ae_c_mul(a->ptr.pp_complex[offs+i][offs+i],tmp->ptr.p_complex[i]));
                        }
                        else
                        {
                            a->ptr.pp_complex[offs+i][offs+j] = ae_c_add(v,tmp->ptr.p_complex[i]);
                        }
                    }
                    ae_v_cmulc(&a->ptr.pp_complex[offs+0][offs+j], a->stride, ae_v_len(offs+0,offs+j-1), ajj);
                }
            }
        }
        else
        {
            
            /*
             * Compute inverse of lower triangular matrix.
             */
            for(j=n-1; j>=0; j--)
            {
                if( !isunit )
                {
                    if( ae_c_eq_d(a->ptr.pp_complex[offs+j][offs+j],(double)(0)) )
                    {
                        *info = -3;
                        return;
                    }
                    a->ptr.pp_complex[offs+j][offs+j] = ae_c_d_div(1,a->ptr.pp_complex[offs+j][offs+j]);
                    ajj = ae_c_neg(a->ptr.pp_complex[offs+j][offs+j]);
                }
                else
                {
                    ajj = ae_complex_from_i(-1);
                }
                if( j<n-1 )
                {
                    
                    /*
                     * Compute elements j+1:n of j-th column.
                     */
                    ae_v_cmove(&tmp->ptr.p_complex[j+1], 1, &a->ptr.pp_complex[offs+j+1][offs+j], a->stride, "N", ae_v_len(j+1,n-1));
                    for(i=j+1; i<=n-1; i++)
                    {
                        if( i>j+1 )
                        {
                            v = ae_v_cdotproduct(&a->ptr.pp_complex[offs+i][offs+j+1], 1, "N", &tmp->ptr.p_complex[j+1], 1, "N", ae_v_len(offs+j+1,offs+i-1));
                        }
                        else
                        {
                            v = ae_complex_from_i(0);
                        }
                        if( !isunit )
                        {
                            a->ptr.pp_complex[offs+i][offs+j] = ae_c_add(v,ae_c_mul(a->ptr.pp_complex[offs+i][offs+i],tmp->ptr.p_complex[i]));
                        }
                        else
                        {
                            a->ptr.pp_complex[offs+i][offs+j] = ae_c_add(v,tmp->ptr.p_complex[i]);
                        }
                    }
                    ae_v_cmulc(&a->ptr.pp_complex[offs+j+1][offs+j], a->stride, ae_v_len(offs+j+1,offs+n-1), ajj);
                }
            }
        }
        return;
    }
    
    /*
     * Recursive case
     */
    ablascomplexsplitlength(a, n, &n1, &n2, _state);
    if( n2>0 )
    {
        if( isupper )
        {
            for(i=0; i<=n1-1; i++)
            {
                ae_v_cmuld(&a->ptr.pp_complex[offs+i][offs+n1], 1, ae_v_len(offs+n1,offs+n-1), -1);
            }
            cmatrixlefttrsm(n1, n2, a, offs, offs, isupper, isunit, 0, a, offs, offs+n1, _state);
            cmatrixrighttrsm(n1, n2, a, offs+n1, offs+n1, isupper, isunit, 0, a, offs, offs+n1, _state);
        }
        else
        {
            for(i=0; i<=n2-1; i++)
            {
                ae_v_cmuld(&a->ptr.pp_complex[offs+n1+i][offs], 1, ae_v_len(offs,offs+n1-1), -1);
            }
            cmatrixrighttrsm(n2, n1, a, offs, offs, isupper, isunit, 0, a, offs+n1, offs, _state);
            cmatrixlefttrsm(n2, n1, a, offs+n1, offs+n1, isupper, isunit, 0, a, offs+n1, offs, _state);
        }
        matinv_cmatrixtrinverserec(a, offs+n1, n2, isupper, isunit, tmp, info, rep, _state);
    }
    matinv_cmatrixtrinverserec(a, offs, n1, isupper, isunit, tmp, info, rep, _state);
}


static void matinv_rmatrixluinverserec(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     /* Real    */ ae_vector* work,
     sinteger* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_int_t n1;
    ae_int_t n2;


    if( n<1 )
    {
        info->val = -1;
        return;
    }
    
    /*
     * Base case
     */
    if( n<=ablasblocksize(a, _state) )
    {
        
        /*
         * Form inv(U)
         */
        matinv_rmatrixtrinverserec(a, offs, n, ae_true, ae_false, work, info, rep, _state);
        if( info->val<=0 )
        {
            return;
        }
        
        /*
         * Solve the equation inv(A)*L = inv(U) for inv(A).
         */
        for(j=n-1; j>=0; j--)
        {
            
            /*
             * Copy current column of L to WORK and replace with zeros.
             */
            for(i=j+1; i<=n-1; i++)
            {
                work->ptr.p_double[i] = a->ptr.pp_double[offs+i][offs+j];
                a->ptr.pp_double[offs+i][offs+j] = (double)(0);
            }
            
            /*
             * Compute current column of inv(A).
             */
            if( j<n-1 )
            {
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&a->ptr.pp_double[offs+i][offs+j+1], 1, &work->ptr.p_double[j+1], 1, ae_v_len(offs+j+1,offs+n-1));
                    a->ptr.pp_double[offs+i][offs+j] = a->ptr.pp_double[offs+i][offs+j]-v;
                }
            }
        }
        return;
    }
    
    /*
     * Recursive code:
     *
     *         ( L1      )   ( U1  U12 )
     * A    =  (         ) * (         )
     *         ( L12  L2 )   (     U2  )
     *
     *         ( W   X )
     * A^-1 =  (       )
     *         ( Y   Z )
     */
    ablassplitlength(a, n, &n1, &n2, _state);
    ae_assert(n2>0, "LUInverseRec: internal error!", _state);
    
    /*
     * X := inv(U1)*U12*inv(U2)
     */
    rmatrixlefttrsm(n1, n2, a, offs, offs, ae_true, ae_false, 0, a, offs, offs+n1, _state);
    rmatrixrighttrsm(n1, n2, a, offs+n1, offs+n1, ae_true, ae_false, 0, a, offs, offs+n1, _state);
    
    /*
     * Y := inv(L2)*L12*inv(L1)
     */
    rmatrixlefttrsm(n2, n1, a, offs+n1, offs+n1, ae_false, ae_true, 0, a, offs+n1, offs, _state);
    rmatrixrighttrsm(n2, n1, a, offs, offs, ae_false, ae_true, 0, a, offs+n1, offs, _state);
    
    /*
     * W := inv(L1*U1)+X*Y
     */
    matinv_rmatrixluinverserec(a, offs, n1, work, info, rep, _state);
    if( info->val<=0 )
    {
        return;
    }
    rmatrixgemm(n1, n1, n2, 1.0, a, offs, offs+n1, 0, a, offs+n1, offs, 0, 1.0, a, offs, offs, _state);
    
    /*
     * X := -X*inv(L2)
     * Y := -inv(U2)*Y
     */
    rmatrixrighttrsm(n1, n2, a, offs+n1, offs+n1, ae_false, ae_true, 0, a, offs, offs+n1, _state);
    for(i=0; i<=n1-1; i++)
    {
        ae_v_muld(&a->ptr.pp_double[offs+i][offs+n1], 1, ae_v_len(offs+n1,offs+n-1), -1);
    }
    rmatrixlefttrsm(n2, n1, a, offs+n1, offs+n1, ae_true, ae_false, 0, a, offs+n1, offs, _state);
    for(i=0; i<=n2-1; i++)
    {
        ae_v_muld(&a->ptr.pp_double[offs+n1+i][offs], 1, ae_v_len(offs,offs+n1-1), -1);
    }
    
    /*
     * Z := inv(L2*U2)
     */
    matinv_rmatrixluinverserec(a, offs+n1, n2, work, info, rep, _state);
}


static void matinv_cmatrixluinverserec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     /* Complex */ ae_vector* work,
     ae_int_t* info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_complex v;
    ae_int_t n1;
    ae_int_t n2;


    if( n<1 )
    {
        *info = -1;
        return;
    }
    
    /*
     * Base case
     */
    if( n<=ablascomplexblocksize(a, _state) )
    {
        
        /*
         * Form inv(U)
         */
        matinv_cmatrixtrinverserec(a, offs, n, ae_true, ae_false, work, info, rep, _state);
        if( *info<=0 )
        {
            return;
        }
        
        /*
         * Solve the equation inv(A)*L = inv(U) for inv(A).
         */
        for(j=n-1; j>=0; j--)
        {
            
            /*
             * Copy current column of L to WORK and replace with zeros.
             */
            for(i=j+1; i<=n-1; i++)
            {
                work->ptr.p_complex[i] = a->ptr.pp_complex[offs+i][offs+j];
                a->ptr.pp_complex[offs+i][offs+j] = ae_complex_from_i(0);
            }
            
            /*
             * Compute current column of inv(A).
             */
            if( j<n-1 )
            {
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_cdotproduct(&a->ptr.pp_complex[offs+i][offs+j+1], 1, "N", &work->ptr.p_complex[j+1], 1, "N", ae_v_len(offs+j+1,offs+n-1));
                    a->ptr.pp_complex[offs+i][offs+j] = ae_c_sub(a->ptr.pp_complex[offs+i][offs+j],v);
                }
            }
        }
        return;
    }
    
    /*
     * Recursive code:
     *
     *         ( L1      )   ( U1  U12 )
     * A    =  (         ) * (         )
     *         ( L12  L2 )   (     U2  )
     *
     *         ( W   X )
     * A^-1 =  (       )
     *         ( Y   Z )
     */
    ablascomplexsplitlength(a, n, &n1, &n2, _state);
    ae_assert(n2>0, "LUInverseRec: internal error!", _state);
    
    /*
     * X := inv(U1)*U12*inv(U2)
     */
    cmatrixlefttrsm(n1, n2, a, offs, offs, ae_true, ae_false, 0, a, offs, offs+n1, _state);
    cmatrixrighttrsm(n1, n2, a, offs+n1, offs+n1, ae_true, ae_false, 0, a, offs, offs+n1, _state);
    
    /*
     * Y := inv(L2)*L12*inv(L1)
     */
    cmatrixlefttrsm(n2, n1, a, offs+n1, offs+n1, ae_false, ae_true, 0, a, offs+n1, offs, _state);
    cmatrixrighttrsm(n2, n1, a, offs, offs, ae_false, ae_true, 0, a, offs+n1, offs, _state);
    
    /*
     * W := inv(L1*U1)+X*Y
     */
    matinv_cmatrixluinverserec(a, offs, n1, work, info, rep, _state);
    if( *info<=0 )
    {
        return;
    }
    cmatrixgemm(n1, n1, n2, ae_complex_from_d(1.0), a, offs, offs+n1, 0, a, offs+n1, offs, 0, ae_complex_from_d(1.0), a, offs, offs, _state);
    
    /*
     * X := -X*inv(L2)
     * Y := -inv(U2)*Y
     */
    cmatrixrighttrsm(n1, n2, a, offs+n1, offs+n1, ae_false, ae_true, 0, a, offs, offs+n1, _state);
    for(i=0; i<=n1-1; i++)
    {
        ae_v_cmuld(&a->ptr.pp_complex[offs+i][offs+n1], 1, ae_v_len(offs+n1,offs+n-1), -1);
    }
    cmatrixlefttrsm(n2, n1, a, offs+n1, offs+n1, ae_true, ae_false, 0, a, offs+n1, offs, _state);
    for(i=0; i<=n2-1; i++)
    {
        ae_v_cmuld(&a->ptr.pp_complex[offs+n1+i][offs], 1, ae_v_len(offs,offs+n1-1), -1);
    }
    
    /*
     * Z := inv(L2*U2)
     */
    matinv_cmatrixluinverserec(a, offs+n1, n2, work, info, rep, _state);
}


/*************************************************************************
Recursive subroutine for HPD inversion.

  -- ALGLIB routine --
     10.02.2010
     Bochkanov Sergey
*************************************************************************/
static void matinv_hpdmatrixcholeskyinverserec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* tmp,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_complex v;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t info2;
    matinvreport rep2;

    ae_frame_make(_state, &_frame_block);
    _matinvreport_init(&rep2, _state);

    if( n<1 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Base case
     */
    if( n<=ablascomplexblocksize(a, _state) )
    {
        matinv_cmatrixtrinverserec(a, offs, n, isupper, ae_false, tmp, &info2, &rep2, _state);
        if( isupper )
        {
            
            /*
             * Compute the product U * U'.
             * NOTE: we never assume that diagonal of U is real
             */
            for(i=0; i<=n-1; i++)
            {
                if( i==0 )
                {
                    
                    /*
                     * 1x1 matrix
                     */
                    a->ptr.pp_complex[offs+i][offs+i] = ae_complex_from_d(ae_sqr(a->ptr.pp_complex[offs+i][offs+i].x, _state)+ae_sqr(a->ptr.pp_complex[offs+i][offs+i].y, _state));
                }
                else
                {
                    
                    /*
                     * (I+1)x(I+1) matrix,
                     *
                     * ( A11  A12 )   ( A11^H        )   ( A11*A11^H+A12*A12^H  A12*A22^H )
                     * (          ) * (              ) = (                                )
                     * (      A22 )   ( A12^H  A22^H )   ( A22*A12^H            A22*A22^H )
                     *
                     * A11 is IxI, A22 is 1x1.
                     */
                    ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs][offs+i], a->stride, "Conj", ae_v_len(0,i-1));
                    for(j=0; j<=i-1; j++)
                    {
                        v = a->ptr.pp_complex[offs+j][offs+i];
                        ae_v_caddc(&a->ptr.pp_complex[offs+j][offs+j], 1, &tmp->ptr.p_complex[j], 1, "N", ae_v_len(offs+j,offs+i-1), v);
                    }
                    v = ae_c_conj(a->ptr.pp_complex[offs+i][offs+i], _state);
                    ae_v_cmulc(&a->ptr.pp_complex[offs][offs+i], a->stride, ae_v_len(offs,offs+i-1), v);
                    a->ptr.pp_complex[offs+i][offs+i] = ae_complex_from_d(ae_sqr(a->ptr.pp_complex[offs+i][offs+i].x, _state)+ae_sqr(a->ptr.pp_complex[offs+i][offs+i].y, _state));
                }
            }
        }
        else
        {
            
            /*
             * Compute the product L' * L
             * NOTE: we never assume that diagonal of L is real
             */
            for(i=0; i<=n-1; i++)
            {
                if( i==0 )
                {
                    
                    /*
                     * 1x1 matrix
                     */
                    a->ptr.pp_complex[offs+i][offs+i] = ae_complex_from_d(ae_sqr(a->ptr.pp_complex[offs+i][offs+i].x, _state)+ae_sqr(a->ptr.pp_complex[offs+i][offs+i].y, _state));
                }
                else
                {
                    
                    /*
                     * (I+1)x(I+1) matrix,
                     *
                     * ( A11^H  A21^H )   ( A11      )   ( A11^H*A11+A21^H*A21  A21^H*A22 )
                     * (              ) * (          ) = (                                )
                     * (        A22^H )   ( A21  A22 )   ( A22^H*A21            A22^H*A22 )
                     *
                     * A11 is IxI, A22 is 1x1.
                     */
                    ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs+i][offs], 1, "N", ae_v_len(0,i-1));
                    for(j=0; j<=i-1; j++)
                    {
                        v = ae_c_conj(a->ptr.pp_complex[offs+i][offs+j], _state);
                        ae_v_caddc(&a->ptr.pp_complex[offs+j][offs], 1, &tmp->ptr.p_complex[0], 1, "N", ae_v_len(offs,offs+j), v);
                    }
                    v = ae_c_conj(a->ptr.pp_complex[offs+i][offs+i], _state);
                    ae_v_cmulc(&a->ptr.pp_complex[offs+i][offs], 1, ae_v_len(offs,offs+i-1), v);
                    a->ptr.pp_complex[offs+i][offs+i] = ae_complex_from_d(ae_sqr(a->ptr.pp_complex[offs+i][offs+i].x, _state)+ae_sqr(a->ptr.pp_complex[offs+i][offs+i].y, _state));
                }
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Recursive code: triangular factor inversion merged with
     * UU' or L'L multiplication
     */
    ablascomplexsplitlength(a, n, &n1, &n2, _state);
    
    /*
     * form off-diagonal block of trangular inverse
     */
    if( isupper )
    {
        for(i=0; i<=n1-1; i++)
        {
            ae_v_cmuld(&a->ptr.pp_complex[offs+i][offs+n1], 1, ae_v_len(offs+n1,offs+n-1), -1);
        }
        cmatrixlefttrsm(n1, n2, a, offs, offs, isupper, ae_false, 0, a, offs, offs+n1, _state);
        cmatrixrighttrsm(n1, n2, a, offs+n1, offs+n1, isupper, ae_false, 0, a, offs, offs+n1, _state);
    }
    else
    {
        for(i=0; i<=n2-1; i++)
        {
            ae_v_cmuld(&a->ptr.pp_complex[offs+n1+i][offs], 1, ae_v_len(offs,offs+n1-1), -1);
        }
        cmatrixrighttrsm(n2, n1, a, offs, offs, isupper, ae_false, 0, a, offs+n1, offs, _state);
        cmatrixlefttrsm(n2, n1, a, offs+n1, offs+n1, isupper, ae_false, 0, a, offs+n1, offs, _state);
    }
    
    /*
     * invert first diagonal block
     */
    matinv_hpdmatrixcholeskyinverserec(a, offs, n1, isupper, tmp, _state);
    
    /*
     * update first diagonal block with off-diagonal block,
     * update off-diagonal block
     */
    if( isupper )
    {
        cmatrixherk(n1, n2, 1.0, a, offs, offs+n1, 0, 1.0, a, offs, offs, isupper, _state);
        cmatrixrighttrsm(n1, n2, a, offs+n1, offs+n1, isupper, ae_false, 2, a, offs, offs+n1, _state);
    }
    else
    {
        cmatrixherk(n1, n2, 1.0, a, offs+n1, offs, 2, 1.0, a, offs, offs, isupper, _state);
        cmatrixlefttrsm(n2, n1, a, offs+n1, offs+n1, isupper, ae_false, 2, a, offs+n1, offs, _state);
    }
    
    /*
     * invert second diagonal block
     */
    matinv_hpdmatrixcholeskyinverserec(a, offs+n1, n2, isupper, tmp, _state);
    ae_frame_leave(_state);
}


void _matinvreport_init(void* _p, ae_state *_state)
{
    matinvreport *p = (matinvreport*)_p;
    ae_touch_ptr((void*)p);
}


void _matinvreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    matinvreport *dst = (matinvreport*)_dst;
    matinvreport *src = (matinvreport*)_src;
    dst->r1 = src->r1;
    dst->rinf = src->rinf;
}


void _matinvreport_clear(void* _p)
{
    matinvreport *p = (matinvreport*)_p;
    ae_touch_ptr((void*)p);
}


void _matinvreport_destroy(void* _p)
{
    matinvreport *p = (matinvreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
