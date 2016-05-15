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
#include "trfac.h"


/*$ Declarations $*/
static void trfac_cmatrixluprec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Complex */ ae_vector* tmp,
     ae_state *_state);
static void trfac_rmatrixluprec(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Real    */ ae_vector* tmp,
     ae_state *_state);
static void trfac_cmatrixplurec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Complex */ ae_vector* tmp,
     ae_state *_state);
static void trfac_rmatrixplurec(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Real    */ ae_vector* tmp,
     ae_state *_state);
static void trfac_cmatrixlup2(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Complex */ ae_vector* tmp,
     ae_state *_state);
static void trfac_rmatrixlup2(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Real    */ ae_vector* tmp,
     ae_state *_state);
static void trfac_cmatrixplu2(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Complex */ ae_vector* tmp,
     ae_state *_state);
static void trfac_rmatrixplu2(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Real    */ ae_vector* tmp,
     ae_state *_state);
static ae_bool trfac_hpdmatrixcholeskyrec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* tmp,
     ae_state *_state);
static ae_bool trfac_hpdmatrixcholesky2(/* Complex */ ae_matrix* aaa,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* tmp,
     ae_state *_state);
static ae_bool trfac_spdmatrixcholesky2(/* Real    */ ae_matrix* aaa,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* tmp,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
LU decomposition of a general real matrix with row pivoting

A is represented as A = P*L*U, where:
* L is lower unitriangular matrix
* U is upper triangular matrix
* P = P0*P1*...*PK, K=min(M,N)-1,
  Pi - permutation matrix for I and Pivots[I]

This is cache-oblivous implementation of LU decomposition.
It is optimized for square matrices. As for rectangular matrices:
* best case - M>>N
* worst case - N>>M, small M, large N, matrix does not fit in CPU cache

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
    A       -   array[0..M-1, 0..N-1].
    M       -   number of rows in matrix A.
    N       -   number of columns in matrix A.


OUTPUT PARAMETERS:
    A       -   matrices L and U in compact form:
                * L is stored under main diagonal
                * U is stored on and above main diagonal
    Pivots  -   permutation matrix in compact form.
                array[0..Min(M-1,N-1)].

  -- ALGLIB routine --
     10.01.2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixlu(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state)
{

    ae_vector_clear(pivots);

    ae_assert(m>0, "RMatrixLU: incorrect M!", _state);
    ae_assert(n>0, "RMatrixLU: incorrect N!", _state);
    rmatrixplu(a, m, n, pivots, _state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixlu(/* Real    */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Integer */ ae_vector* pivots, ae_state *_state)
{
    rmatrixlu(a,m,n,pivots, _state);
}


/*************************************************************************
LU decomposition of a general complex matrix with row pivoting

A is represented as A = P*L*U, where:
* L is lower unitriangular matrix
* U is upper triangular matrix
* P = P0*P1*...*PK, K=min(M,N)-1,
  Pi - permutation matrix for I and Pivots[I]

This is cache-oblivous implementation of LU decomposition. It is optimized
for square matrices. As for rectangular matrices:
* best case - M>>N
* worst case - N>>M, small M, large N, matrix does not fit in CPU cache

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
    A       -   array[0..M-1, 0..N-1].
    M       -   number of rows in matrix A.
    N       -   number of columns in matrix A.


OUTPUT PARAMETERS:
    A       -   matrices L and U in compact form:
                * L is stored under main diagonal
                * U is stored on and above main diagonal
    Pivots  -   permutation matrix in compact form.
                array[0..Min(M-1,N-1)].

  -- ALGLIB routine --
     10.01.2010
     Bochkanov Sergey
*************************************************************************/
void cmatrixlu(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state)
{

    ae_vector_clear(pivots);

    ae_assert(m>0, "CMatrixLU: incorrect M!", _state);
    ae_assert(n>0, "CMatrixLU: incorrect N!", _state);
    cmatrixplu(a, m, n, pivots, _state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixlu(/* Complex */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Integer */ ae_vector* pivots, ae_state *_state)
{
    cmatrixlu(a,m,n,pivots, _state);
}


/*************************************************************************
Cache-oblivious Cholesky decomposition

The algorithm computes Cholesky decomposition  of  a  Hermitian  positive-
definite matrix. The result of an algorithm is a representation  of  A  as
A=U'*U  or A=L*L' (here X' detones conj(X^T)).

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

INPUT PARAMETERS:
    A       -   upper or lower triangle of a factorized matrix.
                array with elements [0..N-1, 0..N-1].
    N       -   size of matrix A.
    IsUpper -   if IsUpper=True, then A contains an upper triangle of
                a symmetric matrix, otherwise A contains a lower one.

OUTPUT PARAMETERS:
    A       -   the result of factorization. If IsUpper=True, then
                the upper triangle contains matrix U, so that A = U'*U,
                and the elements below the main diagonal are not modified.
                Similarly, if IsUpper = False.

RESULT:
    If  the  matrix  is  positive-definite,  the  function  returns  True.
    Otherwise, the function returns False. Contents of A is not determined
    in such case.

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
ae_bool hpdmatrixcholesky(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector tmp;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&tmp, 0, DT_COMPLEX, _state);

    if( n<1 )
    {
        result = ae_false;
        ae_frame_leave(_state);
        return result;
    }
    result = trfac_hpdmatrixcholeskyrec(a, 0, n, isupper, &tmp, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_hpdmatrixcholesky(/* Complex */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper, ae_state *_state)
{
    return hpdmatrixcholesky(a,n,isupper, _state);
}


/*************************************************************************
Cache-oblivious Cholesky decomposition

The algorithm computes Cholesky decomposition  of  a  symmetric  positive-
definite matrix. The result of an algorithm is a representation  of  A  as
A=U^T*U  or A=L*L^T

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

INPUT PARAMETERS:
    A       -   upper or lower triangle of a factorized matrix.
                array with elements [0..N-1, 0..N-1].
    N       -   size of matrix A.
    IsUpper -   if IsUpper=True, then A contains an upper triangle of
                a symmetric matrix, otherwise A contains a lower one.

OUTPUT PARAMETERS:
    A       -   the result of factorization. If IsUpper=True, then
                the upper triangle contains matrix U, so that A = U^T*U,
                and the elements below the main diagonal are not modified.
                Similarly, if IsUpper = False.

RESULT:
    If  the  matrix  is  positive-definite,  the  function  returns  True.
    Otherwise, the function returns False. Contents of A is not determined
    in such case.

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
ae_bool spdmatrixcholesky(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector tmp;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    if( n<1 )
    {
        result = ae_false;
        ae_frame_leave(_state);
        return result;
    }
    result = spdmatrixcholeskyrec(a, 0, n, isupper, &tmp, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_spdmatrixcholesky(/* Real    */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper, ae_state *_state)
{
    return spdmatrixcholesky(a,n,isupper, _state);
}


/*************************************************************************
Update of Cholesky decomposition: rank-1 update to original A.  "Buffered"
version which uses preallocated buffer which is saved  between  subsequent
function calls.

This function uses internally allocated buffer which is not saved  between
subsequent  calls.  So,  if  you  perform  a lot  of  subsequent  updates,
we  recommend   you   to   use   "buffered"   version   of  this function:
SPDMatrixCholeskyUpdateAdd1Buf().

INPUT PARAMETERS:
    A       -   upper or lower Cholesky factor.
                array with elements [0..N-1, 0..N-1].
                Exception is thrown if array size is too small.
    N       -   size of matrix A, N>0
    IsUpper -   if IsUpper=True, then A contains  upper  Cholesky  factor;
                otherwise A contains a lower one.
    U       -   array[N], rank-1 update to A: A_mod = A + u*u'
                Exception is thrown if array size is too small.
    BufR    -   possibly preallocated  buffer;  automatically  resized  if
                needed. It is recommended to  reuse  this  buffer  if  you
                perform a lot of subsequent decompositions.

OUTPUT PARAMETERS:
    A       -   updated factorization.  If  IsUpper=True,  then  the  upper
                triangle contains matrix U, and the elements below the main
                diagonal are not modified. Similarly, if IsUpper = False.
                
NOTE: this function always succeeds, so it does not return completion code

NOTE: this function checks sizes of input arrays, but it does  NOT  checks
      for presence of infinities or NAN's.

  -- ALGLIB --
     03.02.2014
     Sergey Bochkanov
*************************************************************************/
void spdmatrixcholeskyupdateadd1(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* u,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector bufr;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&bufr, 0, DT_REAL, _state);

    ae_assert(n>0, "SPDMatrixCholeskyUpdateAdd1: N<=0", _state);
    ae_assert(a->rows>=n, "SPDMatrixCholeskyUpdateAdd1: Rows(A)<N", _state);
    ae_assert(a->cols>=n, "SPDMatrixCholeskyUpdateAdd1: Cols(A)<N", _state);
    ae_assert(u->cnt>=n, "SPDMatrixCholeskyUpdateAdd1: Length(U)<N", _state);
    spdmatrixcholeskyupdateadd1buf(a, n, isupper, u, &bufr, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Update of Cholesky decomposition: "fixing" some variables.

This function uses internally allocated buffer which is not saved  between
subsequent  calls.  So,  if  you  perform  a lot  of  subsequent  updates,
we  recommend   you   to   use   "buffered"   version   of  this function:
SPDMatrixCholeskyUpdateFixBuf().

"FIXING" EXPLAINED:

    Suppose we have N*N positive definite matrix A. "Fixing" some variable
    means filling corresponding row/column of  A  by  zeros,  and  setting
    diagonal element to 1.
    
    For example, if we fix 2nd variable in 4*4 matrix A, it becomes Af:
    
        ( A00  A01  A02  A03 )      ( Af00  0   Af02 Af03 )
        ( A10  A11  A12  A13 )      (  0    1    0    0   )
        ( A20  A21  A22  A23 )  =>  ( Af20  0   Af22 Af23 )
        ( A30  A31  A32  A33 )      ( Af30  0   Af32 Af33 )
    
    If we have Cholesky decomposition of A, it must be recalculated  after
    variables were  fixed.  However,  it  is  possible  to  use  efficient
    algorithm, which needs O(K*N^2)  time  to  "fix"  K  variables,  given
    Cholesky decomposition of original, "unfixed" A.

INPUT PARAMETERS:
    A       -   upper or lower Cholesky factor.
                array with elements [0..N-1, 0..N-1].
                Exception is thrown if array size is too small.
    N       -   size of matrix A, N>0
    IsUpper -   if IsUpper=True, then A contains  upper  Cholesky  factor;
                otherwise A contains a lower one.
    Fix     -   array[N], I-th element is True if I-th  variable  must  be
                fixed. Exception is thrown if array size is too small.
    BufR    -   possibly preallocated  buffer;  automatically  resized  if
                needed. It is recommended to  reuse  this  buffer  if  you
                perform a lot of subsequent decompositions.

OUTPUT PARAMETERS:
    A       -   updated factorization.  If  IsUpper=True,  then  the  upper
                triangle contains matrix U, and the elements below the main
                diagonal are not modified. Similarly, if IsUpper = False.
                
NOTE: this function always succeeds, so it does not return completion code

NOTE: this function checks sizes of input arrays, but it does  NOT  checks
      for presence of infinities or NAN's.
      
NOTE: this  function  is  efficient  only  for  moderate amount of updated
      variables - say, 0.1*N or 0.3*N. For larger amount of  variables  it
      will  still  work,  but  you  may  get   better   performance   with
      straightforward Cholesky.

  -- ALGLIB --
     03.02.2014
     Sergey Bochkanov
*************************************************************************/
void spdmatrixcholeskyupdatefix(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Boolean */ ae_vector* fix,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector bufr;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&bufr, 0, DT_REAL, _state);

    ae_assert(n>0, "SPDMatrixCholeskyUpdateFix: N<=0", _state);
    ae_assert(a->rows>=n, "SPDMatrixCholeskyUpdateFix: Rows(A)<N", _state);
    ae_assert(a->cols>=n, "SPDMatrixCholeskyUpdateFix: Cols(A)<N", _state);
    ae_assert(fix->cnt>=n, "SPDMatrixCholeskyUpdateFix: Length(Fix)<N", _state);
    spdmatrixcholeskyupdatefixbuf(a, n, isupper, fix, &bufr, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Update of Cholesky decomposition: rank-1 update to original A.  "Buffered"
version which uses preallocated buffer which is saved  between  subsequent
function calls.

See comments for SPDMatrixCholeskyUpdateAdd1() for more information.

INPUT PARAMETERS:
    A       -   upper or lower Cholesky factor.
                array with elements [0..N-1, 0..N-1].
                Exception is thrown if array size is too small.
    N       -   size of matrix A, N>0
    IsUpper -   if IsUpper=True, then A contains  upper  Cholesky  factor;
                otherwise A contains a lower one.
    U       -   array[N], rank-1 update to A: A_mod = A + u*u'
                Exception is thrown if array size is too small.
    BufR    -   possibly preallocated  buffer;  automatically  resized  if
                needed. It is recommended to  reuse  this  buffer  if  you
                perform a lot of subsequent decompositions.

OUTPUT PARAMETERS:
    A       -   updated factorization.  If  IsUpper=True,  then  the  upper
                triangle contains matrix U, and the elements below the main
                diagonal are not modified. Similarly, if IsUpper = False.

  -- ALGLIB --
     03.02.2014
     Sergey Bochkanov
*************************************************************************/
void spdmatrixcholeskyupdateadd1buf(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* u,
     /* Real    */ ae_vector* bufr,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t nz;
    double cs;
    double sn;
    double v;
    double vv;


    ae_assert(n>0, "SPDMatrixCholeskyUpdateAdd1Buf: N<=0", _state);
    ae_assert(a->rows>=n, "SPDMatrixCholeskyUpdateAdd1Buf: Rows(A)<N", _state);
    ae_assert(a->cols>=n, "SPDMatrixCholeskyUpdateAdd1Buf: Cols(A)<N", _state);
    ae_assert(u->cnt>=n, "SPDMatrixCholeskyUpdateAdd1Buf: Length(U)<N", _state);
    
    /*
     * Find index of first non-zero entry in U
     */
    nz = n;
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_neq(u->ptr.p_double[i],(double)(0)) )
        {
            nz = i;
            break;
        }
    }
    if( nz==n )
    {
        
        /*
         * Nothing to update
         */
        return;
    }
    
    /*
     * If working with upper triangular matrix
     */
    if( isupper )
    {
        
        /*
         * Perform a sequence of updates which fix variables one by one.
         * This approach is different from one which is used when we work
         * with lower triangular matrix.
         */
        rvectorsetlengthatleast(bufr, n, _state);
        for(j=nz; j<=n-1; j++)
        {
            bufr->ptr.p_double[j] = u->ptr.p_double[j];
        }
        for(i=nz; i<=n-1; i++)
        {
            if( ae_fp_neq(bufr->ptr.p_double[i],(double)(0)) )
            {
                generaterotation(a->ptr.pp_double[i][i], bufr->ptr.p_double[i], &cs, &sn, &v, _state);
                a->ptr.pp_double[i][i] = v;
                bufr->ptr.p_double[i] = 0.0;
                for(j=i+1; j<=n-1; j++)
                {
                    v = a->ptr.pp_double[i][j];
                    vv = bufr->ptr.p_double[j];
                    a->ptr.pp_double[i][j] = cs*v+sn*vv;
                    bufr->ptr.p_double[j] = -sn*v+cs*vv;
                }
            }
        }
    }
    else
    {
        
        /*
         * Calculate rows of modified Cholesky factor, row-by-row
         * (updates performed during variable fixing are applied
         * simultaneously to each row)
         */
        rvectorsetlengthatleast(bufr, 3*n, _state);
        for(j=nz; j<=n-1; j++)
        {
            bufr->ptr.p_double[j] = u->ptr.p_double[j];
        }
        for(i=nz; i<=n-1; i++)
        {
            
            /*
             * Update all previous updates [Idx+1...I-1] to I-th row
             */
            vv = bufr->ptr.p_double[i];
            for(j=nz; j<=i-1; j++)
            {
                cs = bufr->ptr.p_double[n+2*j+0];
                sn = bufr->ptr.p_double[n+2*j+1];
                v = a->ptr.pp_double[i][j];
                a->ptr.pp_double[i][j] = cs*v+sn*vv;
                vv = -sn*v+cs*vv;
            }
            
            /*
             * generate rotation applied to I-th element of update vector
             */
            generaterotation(a->ptr.pp_double[i][i], vv, &cs, &sn, &v, _state);
            a->ptr.pp_double[i][i] = v;
            bufr->ptr.p_double[n+2*i+0] = cs;
            bufr->ptr.p_double[n+2*i+1] = sn;
        }
    }
}


/*************************************************************************
Update of Cholesky  decomposition:  "fixing"  some  variables.  "Buffered"
version which uses preallocated buffer which is saved  between  subsequent
function calls.

See comments for SPDMatrixCholeskyUpdateFix() for more information.

INPUT PARAMETERS:
    A       -   upper or lower Cholesky factor.
                array with elements [0..N-1, 0..N-1].
                Exception is thrown if array size is too small.
    N       -   size of matrix A, N>0
    IsUpper -   if IsUpper=True, then A contains  upper  Cholesky  factor;
                otherwise A contains a lower one.
    Fix     -   array[N], I-th element is True if I-th  variable  must  be
                fixed. Exception is thrown if array size is too small.
    BufR    -   possibly preallocated  buffer;  automatically  resized  if
                needed. It is recommended to  reuse  this  buffer  if  you
                perform a lot of subsequent decompositions.

OUTPUT PARAMETERS:
    A       -   updated factorization.  If  IsUpper=True,  then  the  upper
                triangle contains matrix U, and the elements below the main
                diagonal are not modified. Similarly, if IsUpper = False.

  -- ALGLIB --
     03.02.2014
     Sergey Bochkanov
*************************************************************************/
void spdmatrixcholeskyupdatefixbuf(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Boolean */ ae_vector* fix,
     /* Real    */ ae_vector* bufr,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t nfix;
    ae_int_t idx;
    double cs;
    double sn;
    double v;
    double vv;


    ae_assert(n>0, "SPDMatrixCholeskyUpdateFixBuf: N<=0", _state);
    ae_assert(a->rows>=n, "SPDMatrixCholeskyUpdateFixBuf: Rows(A)<N", _state);
    ae_assert(a->cols>=n, "SPDMatrixCholeskyUpdateFixBuf: Cols(A)<N", _state);
    ae_assert(fix->cnt>=n, "SPDMatrixCholeskyUpdateFixBuf: Length(Fix)<N", _state);
    
    /*
     * Count number of variables to fix.
     * Quick exit if NFix=0 or NFix=N
     */
    nfix = 0;
    for(i=0; i<=n-1; i++)
    {
        if( fix->ptr.p_bool[i] )
        {
            inc(&nfix, _state);
        }
    }
    if( nfix==0 )
    {
        
        /*
         * Nothing to fix
         */
        return;
    }
    if( nfix==n )
    {
        
        /*
         * All variables are fixed.
         * Set A to identity and exit.
         */
        if( isupper )
        {
            for(i=0; i<=n-1; i++)
            {
                a->ptr.pp_double[i][i] = (double)(1);
                for(j=i+1; j<=n-1; j++)
                {
                    a->ptr.pp_double[i][j] = (double)(0);
                }
            }
        }
        else
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    a->ptr.pp_double[i][j] = (double)(0);
                }
                a->ptr.pp_double[i][i] = (double)(1);
            }
        }
        return;
    }
    
    /*
     * If working with upper triangular matrix
     */
    if( isupper )
    {
        
        /*
         * Perform a sequence of updates which fix variables one by one.
         * This approach is different from one which is used when we work
         * with lower triangular matrix.
         */
        rvectorsetlengthatleast(bufr, n, _state);
        for(k=0; k<=n-1; k++)
        {
            if( fix->ptr.p_bool[k] )
            {
                idx = k;
                
                /*
                 * Quick exit if it is last variable
                 */
                if( idx==n-1 )
                {
                    for(i=0; i<=idx-1; i++)
                    {
                        a->ptr.pp_double[i][idx] = 0.0;
                    }
                    a->ptr.pp_double[idx][idx] = 1.0;
                    continue;
                }
                
                /*
                 * We have Cholesky decomposition of quadratic term in A,
                 * with upper triangle being stored as given below:
                 *
                 *         ( U00 u01 U02 )
                 *     U = (     u11 u12 )
                 *         (         U22 )
                 *
                 * Here u11 is diagonal element corresponding to variable K. We
                 * want to fix this variable, and we do so by modifying U as follows:
                 *
                 *             ( U00  0  U02 )
                 *     U_mod = (      1   0  )
                 *             (         U_m )
                 *
                 * with U_m = CHOLESKY [ (U22^T)*U22 + (u12^T)*u12 ]
                 *
                 * Of course, we can calculate U_m by calculating (U22^T)*U22 explicitly,
                 * modifying it and performing Cholesky decomposition of modified matrix.
                 * However, we can treat it as follows:
                 * * we already have CHOLESKY[(U22^T)*U22], which is equal to U22
                 * * we have rank-1 update (u12^T)*u12 applied to (U22^T)*U22
                 * * thus, we can calculate updated Cholesky with O(N^2) algorithm
                 *   instead of O(N^3) one
                 */
                for(j=idx+1; j<=n-1; j++)
                {
                    bufr->ptr.p_double[j] = a->ptr.pp_double[idx][j];
                }
                for(i=0; i<=idx-1; i++)
                {
                    a->ptr.pp_double[i][idx] = 0.0;
                }
                a->ptr.pp_double[idx][idx] = 1.0;
                for(i=idx+1; i<=n-1; i++)
                {
                    a->ptr.pp_double[idx][i] = 0.0;
                }
                for(i=idx+1; i<=n-1; i++)
                {
                    if( ae_fp_neq(bufr->ptr.p_double[i],(double)(0)) )
                    {
                        generaterotation(a->ptr.pp_double[i][i], bufr->ptr.p_double[i], &cs, &sn, &v, _state);
                        a->ptr.pp_double[i][i] = v;
                        bufr->ptr.p_double[i] = 0.0;
                        for(j=i+1; j<=n-1; j++)
                        {
                            v = a->ptr.pp_double[i][j];
                            vv = bufr->ptr.p_double[j];
                            a->ptr.pp_double[i][j] = cs*v+sn*vv;
                            bufr->ptr.p_double[j] = -sn*v+cs*vv;
                        }
                    }
                }
            }
        }
    }
    else
    {
        
        /*
         * Calculate rows of modified Cholesky factor, row-by-row
         * (updates performed during variable fixing are applied
         * simultaneously to each row)
         */
        rvectorsetlengthatleast(bufr, 3*n, _state);
        for(k=0; k<=n-1; k++)
        {
            if( fix->ptr.p_bool[k] )
            {
                idx = k;
                
                /*
                 * Quick exit if it is last variable
                 */
                if( idx==n-1 )
                {
                    for(i=0; i<=idx-1; i++)
                    {
                        a->ptr.pp_double[idx][i] = 0.0;
                    }
                    a->ptr.pp_double[idx][idx] = 1.0;
                    continue;
                }
                
                /*
                 * store column to buffer and clear row/column of A
                 */
                for(j=idx+1; j<=n-1; j++)
                {
                    bufr->ptr.p_double[j] = a->ptr.pp_double[j][idx];
                }
                for(i=0; i<=idx-1; i++)
                {
                    a->ptr.pp_double[idx][i] = 0.0;
                }
                a->ptr.pp_double[idx][idx] = 1.0;
                for(i=idx+1; i<=n-1; i++)
                {
                    a->ptr.pp_double[i][idx] = 0.0;
                }
                
                /*
                 * Apply update to rows of A
                 */
                for(i=idx+1; i<=n-1; i++)
                {
                    
                    /*
                     * Update all previous updates [Idx+1...I-1] to I-th row
                     */
                    vv = bufr->ptr.p_double[i];
                    for(j=idx+1; j<=i-1; j++)
                    {
                        cs = bufr->ptr.p_double[n+2*j+0];
                        sn = bufr->ptr.p_double[n+2*j+1];
                        v = a->ptr.pp_double[i][j];
                        a->ptr.pp_double[i][j] = cs*v+sn*vv;
                        vv = -sn*v+cs*vv;
                    }
                    
                    /*
                     * generate rotation applied to I-th element of update vector
                     */
                    generaterotation(a->ptr.pp_double[i][i], vv, &cs, &sn, &v, _state);
                    a->ptr.pp_double[i][i] = v;
                    bufr->ptr.p_double[n+2*i+0] = cs;
                    bufr->ptr.p_double[n+2*i+1] = sn;
                }
            }
        }
    }
}


/*************************************************************************
Sparse Cholesky decomposition for skyline matrixm using in-place algorithm
without allocating additional storage.

The algorithm computes Cholesky decomposition  of  a  symmetric  positive-
definite sparse matrix. The result of an algorithm is a representation  of
A as A=U^T*U or A=L*L^T

This  function  is  a  more  efficient alternative to general, but  slower
SparseCholeskyX(), because it does not  create  temporary  copies  of  the
target. It performs factorization in-place, which gives  best  performance
on low-profile matrices. Its drawback, however, is that it can not perform
profile-reducing permutation of input matrix.

INPUT PARAMETERS:
    A       -   sparse matrix in skyline storage (SKS) format.
    N       -   size of matrix A (can be smaller than actual size of A)
    IsUpper -   if IsUpper=True, then factorization is performed on  upper
                triangle. Another triangle is ignored (it may contant some
                data, but it is not changed).
    

OUTPUT PARAMETERS:
    A       -   the result of factorization, stored in SKS. If IsUpper=True,
                then the upper  triangle  contains  matrix  U,  such  that
                A = U^T*U. Lower triangle is not changed.
                Similarly, if IsUpper = False. In this case L is returned,
                and we have A = L*(L^T).
                Note that THIS function does not  perform  permutation  of
                rows to reduce bandwidth.

RESULT:
    If  the  matrix  is  positive-definite,  the  function  returns  True.
    Otherwise, the function returns False. Contents of A is not determined
    in such case.

NOTE: for  performance  reasons  this  function  does NOT check that input
      matrix  includes  only  finite  values. It is your responsibility to
      make sure that there are no infinite or NAN values in the matrix.

  -- ALGLIB routine --
     16.01.2014
     Bochkanov Sergey
*************************************************************************/
ae_bool sparsecholeskyskyline(sparsematrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t jnz;
    ae_int_t jnza;
    ae_int_t jnzl;
    double v;
    double vv;
    double a12;
    ae_int_t nready;
    ae_int_t nadd;
    ae_int_t banda;
    ae_int_t offsa;
    ae_int_t offsl;
    ae_bool result;


    ae_assert(n>=0, "SparseCholeskySkyline: N<0", _state);
    ae_assert(sparsegetnrows(a, _state)>=n, "SparseCholeskySkyline: rows(A)<N", _state);
    ae_assert(sparsegetncols(a, _state)>=n, "SparseCholeskySkyline: cols(A)<N", _state);
    ae_assert(sparseissks(a, _state), "SparseCholeskySkyline: A is not stored in SKS format", _state);
    result = ae_false;
    
    /*
     * transpose if needed
     */
    if( isupper )
    {
        sparsetransposesks(a, _state);
    }
    
    /*
     * Perform Cholesky decomposition:
     * * we assume than leading NReady*NReady submatrix is done
     * * having Cholesky decomposition of NReady*NReady submatrix we
     *   obtain decomposition of larger (NReady+NAdd)*(NReady+NAdd) one.
     *
     * Here is algorithm. At the start we have
     *
     *     (      |   )
     *     (  L   |   )
     * S = (      |   )
     *     (----------)
     *     (  A   | B )
     *
     * with L being already computed Cholesky factor, A and B being
     * unprocessed parts of the matrix. Of course, L/A/B are stored
     * in SKS format.
     *
     * Then, we calculate A1:=(inv(L)*A')' and replace A with A1.
     * Then, we calculate B1:=B-A1*A1'     and replace B with B1
     *
     * Finally, we calculate small NAdd*NAdd Cholesky of B1 with
     * dense solver. Now, L/A1/B1 are Cholesky decomposition of the
     * larger (NReady+NAdd)*(NReady+NAdd) matrix.
     */
    nready = 0;
    nadd = 1;
    while(nready<n)
    {
        ae_assert(nadd==1, "SkylineCholesky: internal error", _state);
        
        /*
         * Calculate A1:=(inv(L)*A')'
         *
         * Elements are calculated row by row (example below is given
         * for NAdd=1):
         * * first, we solve L[0,0]*A1[0]=A[0]
         * * then, we solve  L[1,0]*A1[0]+L[1,1]*A1[1]=A[1]
         * * then, we move to next row and so on
         * * during calculation of A1 we update A12 - squared norm of A1
         *
         * We extensively use sparsity of both A/A1 and L:
         * * first, equations from 0 to BANDWIDTH(A1)-1 are completely zero
         * * second, for I>=BANDWIDTH(A1), I-th equation is reduced from
         *     L[I,0]*A1[0] + L[I,1]*A1[1] + ... + L[I,I]*A1[I] = A[I]
         *   to
         *     L[I,JNZ]*A1[JNZ] + ... + L[I,I]*A1[I] = A[I]
         *   where JNZ = max(NReady-BANDWIDTH(A1),I-BANDWIDTH(L[i]))
         *   (JNZ is an index of the firts column where both A and L become
         *   nonzero).
         *
         * NOTE: we rely on details of SparseMatrix internal storage format.
         *       This is allowed by SparseMatrix specification.
         */
        a12 = 0.0;
        if( a->didx.ptr.p_int[nready]>0 )
        {
            banda = a->didx.ptr.p_int[nready];
            for(i=nready-banda; i<=nready-1; i++)
            {
                
                /*
                 * Elements of A1[0:I-1] were computed:
                 * * A1[0:NReady-BandA-1] are zero (sparse)
                 * * A1[NReady-BandA:I-1] replaced corresponding elements of A
                 *
                 * Now it is time to get I-th one.
                 *
                 * First, we calculate:
                 * * JNZA  - index of the first column where A become nonzero
                 * * JNZL  - index of the first column where L become nonzero
                 * * JNZ   - index of the first column where both A and L become nonzero
                 * * OffsA - offset of A[JNZ] in A.Vals
                 * * OffsL - offset of L[I,JNZ] in A.Vals
                 *
                 * Then, we solve SUM(A1[j]*L[I,j],j=JNZ..I-1) + A1[I]*L[I,I] = A[I],
                 * with A1[JNZ..I-1] already known, and A1[I] unknown.
                 */
                jnza = nready-banda;
                jnzl = i-a->didx.ptr.p_int[i];
                jnz = ae_maxint(jnza, jnzl, _state);
                offsa = a->ridx.ptr.p_int[nready]+(jnz-jnza);
                offsl = a->ridx.ptr.p_int[i]+(jnz-jnzl);
                v = 0.0;
                k = i-1-jnz;
                for(j=0; j<=k; j++)
                {
                    v = v+a->vals.ptr.p_double[offsa+j]*a->vals.ptr.p_double[offsl+j];
                }
                vv = (a->vals.ptr.p_double[offsa+k+1]-v)/a->vals.ptr.p_double[offsl+k+1];
                a->vals.ptr.p_double[offsa+k+1] = vv;
                a12 = a12+vv*vv;
            }
        }
        
        /*
         * Calculate CHOLESKY(B-A1*A1')
         */
        offsa = a->ridx.ptr.p_int[nready]+a->didx.ptr.p_int[nready];
        v = a->vals.ptr.p_double[offsa];
        if( ae_fp_less_eq(v,a12) )
        {
            result = ae_false;
            return result;
        }
        a->vals.ptr.p_double[offsa] = ae_sqrt(v-a12, _state);
        
        /*
         * Increase size of the updated matrix
         */
        inc(&nready, _state);
    }
    
    /*
     * transpose if needed
     */
    if( isupper )
    {
        sparsetransposesks(a, _state);
    }
    result = ae_true;
    return result;
}


/*************************************************************************
Sparse Cholesky decomposition: "expert" function.

The algorithm computes Cholesky decomposition  of  a  symmetric  positive-
definite sparse matrix. The result is representation of A  as  A=U^T*U  or
A=L*L^T

Triangular factor L or U is written to separate SparseMatrix structure. If
output buffer already contrains enough memory to store L/U, this memory is
reused.

INPUT PARAMETERS:
    A       -   upper or lower triangle of sparse matrix.
                Matrix can be in any sparse storage format.
    N       -   size of matrix A (can be smaller than actual size of A)
    IsUpper -   if IsUpper=True, then A contains an upper triangle of
                a symmetric matrix, otherwise A contains a lower one.
                Another triangle is ignored.
    P0, P1  -   integer arrays:
                * for Ordering=-3  -  user-supplied permutation  of  rows/
                  columns, which complies  to  requirements stated  in the
                  "OUTPUT PARAMETERS" section.  Both  P0 and  P1  must  be
                  initialized by user.
                * for other values of  Ordering  -  possibly  preallocated
                  buffer,  which   is   filled   by  internally  generated
                  permutation. Automatically resized if its  size  is  too
                  small to store data.
    Ordering-   sparse matrix reordering algorithm which is used to reduce
                fill-in amount:
                * -3    use ordering supplied by user in P0/P1
                * -2    use random ordering
                * -1    use original order
                * 0     use best algorithm implemented so far
                If input matrix is  given  in  SKS  format,  factorization
                function ignores Ordering and uses original order  of  the
                columns. The idea is that if you already store  matrix  in
                SKS format, it is better not to perform costly reordering.
    Algo    -   type of algorithm which is used during factorization:
                * 0     use best  algorithm  (for  SKS  input  or   output
                        matrices Algo=2 is used; otherwise Algo=1 is used)
                * 1     use CRS-based algorithm
                * 2     use skyline-based factorization algorithm.
                        This algorithm is a  fastest  one  for low-profile
                        matrices,  but  requires  too  much of memory  for
                        matrices with large bandwidth.
    Fmt     -   desired storage format  of  the  output,  as  returned  by
                SparseGetMatrixType() function:
                * 0 for hash-based storage
                * 1 for CRS
                * 2 for SKS
                If you do not know what format to choose, use 1 (CRS).
    Buf     -   SparseBuffers structure which is used to store temporaries.
                This function may reuse previously allocated  storage,  so
                if you perform repeated factorizations it is beneficial to
                reuse Buf.
    C       -   SparseMatrix structure  which  can  be  just  some  random
                garbage. In  case  in  contains  enough  memory  to  store
                triangular factors, this memory will be reused. Othwerwise,
                algorithm will automatically allocate enough memory.
    

OUTPUT PARAMETERS:
    C       -   the result of factorization, stored in desired format.  If
                IsUpper=True, then the upper triangle  contains  matrix U,
                such  that  (P'*A*P) = U^T*U,  where  P  is  a permutation
                matrix (see below). The elements below the  main  diagonal
                are zero.
                Similarly, if IsUpper = False. In this case L is returned,
                and we have (P'*A*P) = L*(L^T).
    P0      -   permutation  (according   to   Ordering  parameter)  which
                minimizes amount of fill-in:
                * P0 is array[N]
                * permutation is applied to A before  factorization  takes
                  place, i.e. we have U'*U = L*L' = P'*A*P
                * P0[k]=j means that column/row j of A  is  moved  to k-th
                  position before starting factorization.
    P1      -   permutation P in another format, array[N]:
                * P1[k]=j means that k-th column/row of A is moved to j-th
                  position

RESULT:
    If  the  matrix  is  positive-definite,  the  function  returns  True.
    Otherwise, the function returns False. Contents of C is not determined
    in such case.

NOTE: for  performance  reasons  this  function  does NOT check that input
      matrix  includes  only  finite  values. It is your responsibility to
      make sure that there are no infinite or NAN values in the matrix.

  -- ALGLIB routine --
     16.01.2014
     Bochkanov Sergey
*************************************************************************/
ae_bool sparsecholeskyx(sparsematrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Integer */ ae_vector* p0,
     /* Integer */ ae_vector* p1,
     ae_int_t ordering,
     ae_int_t algo,
     ae_int_t fmt,
     sparsebuffers* buf,
     sparsematrix* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t t0;
    ae_int_t t1;
    double v;
    hqrndstate rs;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _hqrndstate_init(&rs, _state);

    ae_assert(n>=0, "SparseMatrixCholeskyBuf: N<0", _state);
    ae_assert(sparsegetnrows(a, _state)>=n, "SparseMatrixCholeskyBuf: rows(A)<N", _state);
    ae_assert(sparsegetncols(a, _state)>=n, "SparseMatrixCholeskyBuf: cols(A)<N", _state);
    ae_assert(ordering>=-3&&ordering<=0, "SparseMatrixCholeskyBuf: invalid Ordering parameter", _state);
    ae_assert(algo>=0&&algo<=2, "SparseMatrixCholeskyBuf: invalid Algo parameter", _state);
    hqrndrandomize(&rs, _state);
    
    /*
     * Perform some quick checks.
     * Because sparse matrices are expensive data structures, these
     * checks are better to perform during early stages of the factorization.
     */
    result = ae_false;
    if( n<1 )
    {
        ae_frame_leave(_state);
        return result;
    }
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_less_eq(sparsegetdiagonal(a, i, _state),(double)(0)) )
        {
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * First, determine appropriate ordering:
     * * for SKS inputs, Ordering=-1 is automatically chosen (overrides user settings)
     */
    if( ordering==0 )
    {
        ordering = -1;
    }
    if( sparseissks(a, _state) )
    {
        ordering = -1;
    }
    if( ordering==-3 )
    {
        
        /*
         * User-supplied ordering.
         * Check its correctness.
         */
        ae_assert(p0->cnt>=n, "SparseCholeskyX: user-supplied permutation is too short", _state);
        ae_assert(p1->cnt>=n, "SparseCholeskyX: user-supplied permutation is too short", _state);
        for(i=0; i<=n-1; i++)
        {
            ae_assert(p0->ptr.p_int[i]>=0&&p0->ptr.p_int[i]<n, "SparseCholeskyX: user-supplied permutation includes values outside of [0,N)", _state);
            ae_assert(p1->ptr.p_int[i]>=0&&p1->ptr.p_int[i]<n, "SparseCholeskyX: user-supplied permutation includes values outside of [0,N)", _state);
            ae_assert(p1->ptr.p_int[p0->ptr.p_int[i]]==i, "SparseCholeskyX: user-supplied permutation is inconsistent - P1 is not inverse of P0", _state);
        }
    }
    if( ordering==-2 )
    {
        
        /*
         * Use random ordering
         */
        ivectorsetlengthatleast(p0, n, _state);
        ivectorsetlengthatleast(p1, n, _state);
        for(i=0; i<=n-1; i++)
        {
            p0->ptr.p_int[i] = i;
        }
        for(i=0; i<=n-1; i++)
        {
            j = i+hqrnduniformi(&rs, n-i, _state);
            if( j!=i )
            {
                k = p0->ptr.p_int[i];
                p0->ptr.p_int[i] = p0->ptr.p_int[j];
                p0->ptr.p_int[j] = k;
            }
        }
        for(i=0; i<=n-1; i++)
        {
            p1->ptr.p_int[p0->ptr.p_int[i]] = i;
        }
    }
    if( ordering==-1 )
    {
        
        /*
         * Use initial ordering
         */
        ivectorsetlengthatleast(p0, n, _state);
        ivectorsetlengthatleast(p1, n, _state);
        for(i=0; i<=n-1; i++)
        {
            p0->ptr.p_int[i] = i;
            p1->ptr.p_int[i] = i;
        }
    }
    
    /*
     * Determine algorithm to use:
     * * for SKS input or output - use SKS solver (overrides user settings)
     * * default is to use Algo=1
     */
    if( algo==0 )
    {
        algo = 1;
    }
    if( sparseissks(a, _state)||fmt==2 )
    {
        algo = 2;
    }
    algo = 2;
    if( algo==2 )
    {
        
        /*
         * Skyline Cholesky with non-skyline output.
         *
         * Call CholeskyX() recursively with Buf.S as output matrix,
         * then perform conversion from SKS to desired format. We can
         * use Buf.S in reccurrent call because SKS-to-SKS CholeskyX()
         * does not uses this field.
         */
        if( fmt!=2 )
        {
            result = sparsecholeskyx(a, n, isupper, p0, p1, -3, algo, 2, buf, &buf->s, _state);
            if( result )
            {
                sparsecopytobuf(&buf->s, fmt, c, _state);
            }
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Skyline Cholesky with skyline output
         */
        if( sparseissks(a, _state)&&ordering==-1 )
        {
            
            /*
             * Non-permuted skyline matrix.
             *
             * Quickly copy matrix to output buffer without permutation.
             *
             * NOTE: Buf.D is used as dummy vector filled with zeros.
             */
            ivectorsetlengthatleast(&buf->d, n, _state);
            for(i=0; i<=n-1; i++)
            {
                buf->d.ptr.p_int[i] = 0;
            }
            if( isupper )
            {
                
                /*
                 * Create strictly upper-triangular matrix,
                 * copy upper triangle of input.
                 */
                sparsecreatesksbuf(n, n, &buf->d, &a->uidx, c, _state);
                for(i=0; i<=n-1; i++)
                {
                    t0 = a->ridx.ptr.p_int[i+1]-a->uidx.ptr.p_int[i]-1;
                    t1 = a->ridx.ptr.p_int[i+1]-1;
                    k = c->ridx.ptr.p_int[i+1]-c->uidx.ptr.p_int[i]-1;
                    for(j=t0; j<=t1; j++)
                    {
                        c->vals.ptr.p_double[k] = a->vals.ptr.p_double[j];
                        k = k+1;
                    }
                }
            }
            else
            {
                
                /*
                 * Create strictly lower-triangular matrix,
                 * copy lower triangle of input.
                 */
                sparsecreatesksbuf(n, n, &a->didx, &buf->d, c, _state);
                for(i=0; i<=n-1; i++)
                {
                    t0 = a->ridx.ptr.p_int[i];
                    t1 = a->ridx.ptr.p_int[i]+a->didx.ptr.p_int[i];
                    k = c->ridx.ptr.p_int[i];
                    for(j=t0; j<=t1; j++)
                    {
                        c->vals.ptr.p_double[k] = a->vals.ptr.p_double[j];
                        k = k+1;
                    }
                }
            }
        }
        else
        {
            
            /*
             * Non-identity permutations OR non-skyline input:
             * * investigate profile of permuted A
             * * create skyline matrix in output buffer
             * * copy input with permutation
             */
            ivectorsetlengthatleast(&buf->d, n, _state);
            ivectorsetlengthatleast(&buf->u, n, _state);
            for(i=0; i<=n-1; i++)
            {
                buf->d.ptr.p_int[i] = 0;
                buf->u.ptr.p_int[i] = 0;
            }
            t0 = 0;
            t1 = 0;
            while(sparseenumerate(a, &t0, &t1, &i, &j, &v, _state))
            {
                if( (isupper&&j>=i)||(!isupper&&j<=i) )
                {
                    i = p1->ptr.p_int[i];
                    j = p1->ptr.p_int[j];
                    if( (j<i&&isupper)||(j>i&&!isupper) )
                    {
                        swapi(&i, &j, _state);
                    }
                    if( i>j )
                    {
                        buf->d.ptr.p_int[i] = ae_maxint(buf->d.ptr.p_int[i], i-j, _state);
                    }
                    else
                    {
                        buf->u.ptr.p_int[j] = ae_maxint(buf->u.ptr.p_int[j], j-i, _state);
                    }
                }
            }
            sparsecreatesksbuf(n, n, &buf->d, &buf->u, c, _state);
            t0 = 0;
            t1 = 0;
            while(sparseenumerate(a, &t0, &t1, &i, &j, &v, _state))
            {
                if( (isupper&&j>=i)||(!isupper&&j<=i) )
                {
                    i = p1->ptr.p_int[i];
                    j = p1->ptr.p_int[j];
                    if( (j<i&&isupper)||(j>i&&!isupper) )
                    {
                        swapi(&j, &i, _state);
                    }
                    sparserewriteexisting(c, i, j, v, _state);
                }
            }
        }
        result = sparsecholeskyskyline(c, n, isupper, _state);
        ae_frame_leave(_state);
        return result;
    }
    ae_assert(ae_false, "SparseCholeskyX: internal error - unexpected algorithm", _state);
    ae_frame_leave(_state);
    return result;
}


void rmatrixlup(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector tmp;
    ae_int_t i;
    ae_int_t j;
    double mx;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(pivots);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    
    /*
     * Internal LU decomposition subroutine.
     * Never call it directly.
     */
    ae_assert(m>0, "RMatrixLUP: incorrect M!", _state);
    ae_assert(n>0, "RMatrixLUP: incorrect N!", _state);
    
    /*
     * Scale matrix to avoid overflows,
     * decompose it, then scale back.
     */
    mx = (double)(0);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            mx = ae_maxreal(mx, ae_fabs(a->ptr.pp_double[i][j], _state), _state);
        }
    }
    if( ae_fp_neq(mx,(double)(0)) )
    {
        v = 1/mx;
        for(i=0; i<=m-1; i++)
        {
            ae_v_muld(&a->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
        }
    }
    ae_vector_set_length(pivots, ae_minint(m, n, _state), _state);
    ae_vector_set_length(&tmp, 2*ae_maxint(m, n, _state), _state);
    trfac_rmatrixluprec(a, 0, m, n, pivots, &tmp, _state);
    if( ae_fp_neq(mx,(double)(0)) )
    {
        v = mx;
        for(i=0; i<=m-1; i++)
        {
            ae_v_muld(&a->ptr.pp_double[i][0], 1, ae_v_len(0,ae_minint(i, n-1, _state)), v);
        }
    }
    ae_frame_leave(_state);
}


void cmatrixlup(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector tmp;
    ae_int_t i;
    ae_int_t j;
    double mx;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(pivots);
    ae_vector_init(&tmp, 0, DT_COMPLEX, _state);

    
    /*
     * Internal LU decomposition subroutine.
     * Never call it directly.
     */
    ae_assert(m>0, "CMatrixLUP: incorrect M!", _state);
    ae_assert(n>0, "CMatrixLUP: incorrect N!", _state);
    
    /*
     * Scale matrix to avoid overflows,
     * decompose it, then scale back.
     */
    mx = (double)(0);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            mx = ae_maxreal(mx, ae_c_abs(a->ptr.pp_complex[i][j], _state), _state);
        }
    }
    if( ae_fp_neq(mx,(double)(0)) )
    {
        v = 1/mx;
        for(i=0; i<=m-1; i++)
        {
            ae_v_cmuld(&a->ptr.pp_complex[i][0], 1, ae_v_len(0,n-1), v);
        }
    }
    ae_vector_set_length(pivots, ae_minint(m, n, _state), _state);
    ae_vector_set_length(&tmp, 2*ae_maxint(m, n, _state), _state);
    trfac_cmatrixluprec(a, 0, m, n, pivots, &tmp, _state);
    if( ae_fp_neq(mx,(double)(0)) )
    {
        v = mx;
        for(i=0; i<=m-1; i++)
        {
            ae_v_cmuld(&a->ptr.pp_complex[i][0], 1, ae_v_len(0,ae_minint(i, n-1, _state)), v);
        }
    }
    ae_frame_leave(_state);
}


void rmatrixplu(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector tmp;
    ae_int_t i;
    ae_int_t j;
    double mx;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(pivots);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    
    /*
     * Internal LU decomposition subroutine.
     * Never call it directly.
     */
    ae_assert(m>0, "RMatrixPLU: incorrect M!", _state);
    ae_assert(n>0, "RMatrixPLU: incorrect N!", _state);
    ae_vector_set_length(&tmp, 2*ae_maxint(m, n, _state), _state);
    ae_vector_set_length(pivots, ae_minint(m, n, _state), _state);
    
    /*
     * Scale matrix to avoid overflows,
     * decompose it, then scale back.
     */
    mx = (double)(0);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            mx = ae_maxreal(mx, ae_fabs(a->ptr.pp_double[i][j], _state), _state);
        }
    }
    if( ae_fp_neq(mx,(double)(0)) )
    {
        v = 1/mx;
        for(i=0; i<=m-1; i++)
        {
            ae_v_muld(&a->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
        }
    }
    trfac_rmatrixplurec(a, 0, m, n, pivots, &tmp, _state);
    if( ae_fp_neq(mx,(double)(0)) )
    {
        v = mx;
        for(i=0; i<=ae_minint(m, n, _state)-1; i++)
        {
            ae_v_muld(&a->ptr.pp_double[i][i], 1, ae_v_len(i,n-1), v);
        }
    }
    ae_frame_leave(_state);
}


void cmatrixplu(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector tmp;
    ae_int_t i;
    ae_int_t j;
    double mx;
    ae_complex v;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(pivots);
    ae_vector_init(&tmp, 0, DT_COMPLEX, _state);

    
    /*
     * Internal LU decomposition subroutine.
     * Never call it directly.
     */
    ae_assert(m>0, "CMatrixPLU: incorrect M!", _state);
    ae_assert(n>0, "CMatrixPLU: incorrect N!", _state);
    ae_vector_set_length(&tmp, 2*ae_maxint(m, n, _state), _state);
    ae_vector_set_length(pivots, ae_minint(m, n, _state), _state);
    
    /*
     * Scale matrix to avoid overflows,
     * decompose it, then scale back.
     */
    mx = (double)(0);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            mx = ae_maxreal(mx, ae_c_abs(a->ptr.pp_complex[i][j], _state), _state);
        }
    }
    if( ae_fp_neq(mx,(double)(0)) )
    {
        v = ae_complex_from_d(1/mx);
        for(i=0; i<=m-1; i++)
        {
            ae_v_cmulc(&a->ptr.pp_complex[i][0], 1, ae_v_len(0,n-1), v);
        }
    }
    trfac_cmatrixplurec(a, 0, m, n, pivots, &tmp, _state);
    if( ae_fp_neq(mx,(double)(0)) )
    {
        v = ae_complex_from_d(mx);
        for(i=0; i<=ae_minint(m, n, _state)-1; i++)
        {
            ae_v_cmulc(&a->ptr.pp_complex[i][i], 1, ae_v_len(i,n-1), v);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Recursive computational subroutine for SPDMatrixCholesky.

INPUT PARAMETERS:
    A       -   matrix given by upper or lower triangle
    Offs    -   offset of diagonal block to decompose
    N       -   diagonal block size
    IsUpper -   what half is given
    Tmp     -   temporary array; allocated by function, if its size is too
                small; can be reused on subsequent calls.
                
OUTPUT PARAMETERS:
    A       -   upper (or lower) triangle contains Cholesky decomposition

RESULT:
    True, on success
    False, on failure

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
ae_bool spdmatrixcholeskyrec(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t n1;
    ae_int_t n2;
    ae_bool result;


    
    /*
     * check N
     */
    if( n<1 )
    {
        result = ae_false;
        return result;
    }
    
    /*
     * Prepare buffer
     */
    if( tmp->cnt<2*n )
    {
        ae_vector_set_length(tmp, 2*n, _state);
    }
    
    /*
     * special cases
     *
     * NOTE: we do not use MKL to accelerate Cholesky basecase
     *       because basecase cost is negligible when compared to
     *       the cost of entire decomposition (most time is spent
     *       in GEMM snd SYRK).
     */
    if( n==1 )
    {
        if( ae_fp_greater(a->ptr.pp_double[offs][offs],(double)(0)) )
        {
            a->ptr.pp_double[offs][offs] = ae_sqrt(a->ptr.pp_double[offs][offs], _state);
            result = ae_true;
        }
        else
        {
            result = ae_false;
        }
        return result;
    }
    if( n<=ablasblocksize(a, _state) )
    {
        result = trfac_spdmatrixcholesky2(a, offs, n, isupper, tmp, _state);
        return result;
    }
    
    /*
     * general case: split task in cache-oblivious manner
     */
    result = ae_true;
    ablassplitlength(a, n, &n1, &n2, _state);
    result = spdmatrixcholeskyrec(a, offs, n1, isupper, tmp, _state);
    if( !result )
    {
        return result;
    }
    if( n2>0 )
    {
        if( isupper )
        {
            rmatrixlefttrsm(n1, n2, a, offs, offs, isupper, ae_false, 1, a, offs, offs+n1, _state);
            rmatrixsyrk(n2, n1, -1.0, a, offs, offs+n1, 1, 1.0, a, offs+n1, offs+n1, isupper, _state);
        }
        else
        {
            rmatrixrighttrsm(n2, n1, a, offs, offs, isupper, ae_false, 1, a, offs+n1, offs, _state);
            rmatrixsyrk(n2, n1, -1.0, a, offs+n1, offs, 0, 1.0, a, offs+n1, offs+n1, isupper, _state);
        }
        result = spdmatrixcholeskyrec(a, offs+n1, n2, isupper, tmp, _state);
        if( !result )
        {
            return result;
        }
    }
    return result;
}


/*************************************************************************
Recurrent complex LU subroutine.
Never call it directly.

  -- ALGLIB routine --
     04.01.2010
     Bochkanov Sergey
*************************************************************************/
static void trfac_cmatrixluprec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Complex */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t m1;
    ae_int_t m2;


    
    /*
     * Kernel case
     */
    if( ae_minint(m, n, _state)<=ablascomplexblocksize(a, _state) )
    {
        trfac_cmatrixlup2(a, offs, m, n, pivots, tmp, _state);
        return;
    }
    
    /*
     * Preliminary step, make N>=M
     *
     *     ( A1 )
     * A = (    ), where A1 is square
     *     ( A2 )
     *
     * Factorize A1, update A2
     */
    if( m>n )
    {
        trfac_cmatrixluprec(a, offs, n, n, pivots, tmp, _state);
        for(i=0; i<=n-1; i++)
        {
            ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs+n][offs+i], a->stride, "N", ae_v_len(0,m-n-1));
            ae_v_cmove(&a->ptr.pp_complex[offs+n][offs+i], a->stride, &a->ptr.pp_complex[offs+n][pivots->ptr.p_int[offs+i]], a->stride, "N", ae_v_len(offs+n,offs+m-1));
            ae_v_cmove(&a->ptr.pp_complex[offs+n][pivots->ptr.p_int[offs+i]], a->stride, &tmp->ptr.p_complex[0], 1, "N", ae_v_len(offs+n,offs+m-1));
        }
        cmatrixrighttrsm(m-n, n, a, offs, offs, ae_true, ae_true, 0, a, offs+n, offs, _state);
        return;
    }
    
    /*
     * Non-kernel case
     */
    ablascomplexsplitlength(a, m, &m1, &m2, _state);
    trfac_cmatrixluprec(a, offs, m1, n, pivots, tmp, _state);
    if( m2>0 )
    {
        for(i=0; i<=m1-1; i++)
        {
            if( offs+i!=pivots->ptr.p_int[offs+i] )
            {
                ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs+m1][offs+i], a->stride, "N", ae_v_len(0,m2-1));
                ae_v_cmove(&a->ptr.pp_complex[offs+m1][offs+i], a->stride, &a->ptr.pp_complex[offs+m1][pivots->ptr.p_int[offs+i]], a->stride, "N", ae_v_len(offs+m1,offs+m-1));
                ae_v_cmove(&a->ptr.pp_complex[offs+m1][pivots->ptr.p_int[offs+i]], a->stride, &tmp->ptr.p_complex[0], 1, "N", ae_v_len(offs+m1,offs+m-1));
            }
        }
        cmatrixrighttrsm(m2, m1, a, offs, offs, ae_true, ae_true, 0, a, offs+m1, offs, _state);
        cmatrixgemm(m-m1, n-m1, m1, ae_complex_from_d(-1.0), a, offs+m1, offs, 0, a, offs, offs+m1, 0, ae_complex_from_d(1.0), a, offs+m1, offs+m1, _state);
        trfac_cmatrixluprec(a, offs+m1, m-m1, n-m1, pivots, tmp, _state);
        for(i=0; i<=m2-1; i++)
        {
            if( offs+m1+i!=pivots->ptr.p_int[offs+m1+i] )
            {
                ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs][offs+m1+i], a->stride, "N", ae_v_len(0,m1-1));
                ae_v_cmove(&a->ptr.pp_complex[offs][offs+m1+i], a->stride, &a->ptr.pp_complex[offs][pivots->ptr.p_int[offs+m1+i]], a->stride, "N", ae_v_len(offs,offs+m1-1));
                ae_v_cmove(&a->ptr.pp_complex[offs][pivots->ptr.p_int[offs+m1+i]], a->stride, &tmp->ptr.p_complex[0], 1, "N", ae_v_len(offs,offs+m1-1));
            }
        }
    }
}


/*************************************************************************
Recurrent real LU subroutine.
Never call it directly.

  -- ALGLIB routine --
     04.01.2010
     Bochkanov Sergey
*************************************************************************/
static void trfac_rmatrixluprec(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t m1;
    ae_int_t m2;


    
    /*
     * Kernel case
     */
    if( ae_minint(m, n, _state)<=ablasblocksize(a, _state) )
    {
        trfac_rmatrixlup2(a, offs, m, n, pivots, tmp, _state);
        return;
    }
    
    /*
     * Preliminary step, make N>=M
     *
     *     ( A1 )
     * A = (    ), where A1 is square
     *     ( A2 )
     *
     * Factorize A1, update A2
     */
    if( m>n )
    {
        trfac_rmatrixluprec(a, offs, n, n, pivots, tmp, _state);
        for(i=0; i<=n-1; i++)
        {
            if( offs+i!=pivots->ptr.p_int[offs+i] )
            {
                ae_v_move(&tmp->ptr.p_double[0], 1, &a->ptr.pp_double[offs+n][offs+i], a->stride, ae_v_len(0,m-n-1));
                ae_v_move(&a->ptr.pp_double[offs+n][offs+i], a->stride, &a->ptr.pp_double[offs+n][pivots->ptr.p_int[offs+i]], a->stride, ae_v_len(offs+n,offs+m-1));
                ae_v_move(&a->ptr.pp_double[offs+n][pivots->ptr.p_int[offs+i]], a->stride, &tmp->ptr.p_double[0], 1, ae_v_len(offs+n,offs+m-1));
            }
        }
        rmatrixrighttrsm(m-n, n, a, offs, offs, ae_true, ae_true, 0, a, offs+n, offs, _state);
        return;
    }
    
    /*
     * Non-kernel case
     */
    ablassplitlength(a, m, &m1, &m2, _state);
    trfac_rmatrixluprec(a, offs, m1, n, pivots, tmp, _state);
    if( m2>0 )
    {
        for(i=0; i<=m1-1; i++)
        {
            if( offs+i!=pivots->ptr.p_int[offs+i] )
            {
                ae_v_move(&tmp->ptr.p_double[0], 1, &a->ptr.pp_double[offs+m1][offs+i], a->stride, ae_v_len(0,m2-1));
                ae_v_move(&a->ptr.pp_double[offs+m1][offs+i], a->stride, &a->ptr.pp_double[offs+m1][pivots->ptr.p_int[offs+i]], a->stride, ae_v_len(offs+m1,offs+m-1));
                ae_v_move(&a->ptr.pp_double[offs+m1][pivots->ptr.p_int[offs+i]], a->stride, &tmp->ptr.p_double[0], 1, ae_v_len(offs+m1,offs+m-1));
            }
        }
        rmatrixrighttrsm(m2, m1, a, offs, offs, ae_true, ae_true, 0, a, offs+m1, offs, _state);
        rmatrixgemm(m-m1, n-m1, m1, -1.0, a, offs+m1, offs, 0, a, offs, offs+m1, 0, 1.0, a, offs+m1, offs+m1, _state);
        trfac_rmatrixluprec(a, offs+m1, m-m1, n-m1, pivots, tmp, _state);
        for(i=0; i<=m2-1; i++)
        {
            if( offs+m1+i!=pivots->ptr.p_int[offs+m1+i] )
            {
                ae_v_move(&tmp->ptr.p_double[0], 1, &a->ptr.pp_double[offs][offs+m1+i], a->stride, ae_v_len(0,m1-1));
                ae_v_move(&a->ptr.pp_double[offs][offs+m1+i], a->stride, &a->ptr.pp_double[offs][pivots->ptr.p_int[offs+m1+i]], a->stride, ae_v_len(offs,offs+m1-1));
                ae_v_move(&a->ptr.pp_double[offs][pivots->ptr.p_int[offs+m1+i]], a->stride, &tmp->ptr.p_double[0], 1, ae_v_len(offs,offs+m1-1));
            }
        }
    }
}


/*************************************************************************
Recurrent complex LU subroutine.
Never call it directly.

  -- ALGLIB routine --
     04.01.2010
     Bochkanov Sergey
*************************************************************************/
static void trfac_cmatrixplurec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Complex */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t n1;
    ae_int_t n2;


    
    /*
     * Kernel case
     */
    if( ae_minint(m, n, _state)<=ablascomplexblocksize(a, _state) )
    {
        trfac_cmatrixplu2(a, offs, m, n, pivots, tmp, _state);
        return;
    }
    
    /*
     * Preliminary step, make M>=N.
     *
     * A = (A1 A2), where A1 is square
     * Factorize A1, update A2
     */
    if( n>m )
    {
        trfac_cmatrixplurec(a, offs, m, m, pivots, tmp, _state);
        for(i=0; i<=m-1; i++)
        {
            ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs+i][offs+m], 1, "N", ae_v_len(0,n-m-1));
            ae_v_cmove(&a->ptr.pp_complex[offs+i][offs+m], 1, &a->ptr.pp_complex[pivots->ptr.p_int[offs+i]][offs+m], 1, "N", ae_v_len(offs+m,offs+n-1));
            ae_v_cmove(&a->ptr.pp_complex[pivots->ptr.p_int[offs+i]][offs+m], 1, &tmp->ptr.p_complex[0], 1, "N", ae_v_len(offs+m,offs+n-1));
        }
        cmatrixlefttrsm(m, n-m, a, offs, offs, ae_false, ae_true, 0, a, offs, offs+m, _state);
        return;
    }
    
    /*
     * Non-kernel case
     */
    ablascomplexsplitlength(a, n, &n1, &n2, _state);
    trfac_cmatrixplurec(a, offs, m, n1, pivots, tmp, _state);
    if( n2>0 )
    {
        for(i=0; i<=n1-1; i++)
        {
            if( offs+i!=pivots->ptr.p_int[offs+i] )
            {
                ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs+i][offs+n1], 1, "N", ae_v_len(0,n2-1));
                ae_v_cmove(&a->ptr.pp_complex[offs+i][offs+n1], 1, &a->ptr.pp_complex[pivots->ptr.p_int[offs+i]][offs+n1], 1, "N", ae_v_len(offs+n1,offs+n-1));
                ae_v_cmove(&a->ptr.pp_complex[pivots->ptr.p_int[offs+i]][offs+n1], 1, &tmp->ptr.p_complex[0], 1, "N", ae_v_len(offs+n1,offs+n-1));
            }
        }
        cmatrixlefttrsm(n1, n2, a, offs, offs, ae_false, ae_true, 0, a, offs, offs+n1, _state);
        cmatrixgemm(m-n1, n-n1, n1, ae_complex_from_d(-1.0), a, offs+n1, offs, 0, a, offs, offs+n1, 0, ae_complex_from_d(1.0), a, offs+n1, offs+n1, _state);
        trfac_cmatrixplurec(a, offs+n1, m-n1, n-n1, pivots, tmp, _state);
        for(i=0; i<=n2-1; i++)
        {
            if( offs+n1+i!=pivots->ptr.p_int[offs+n1+i] )
            {
                ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs+n1+i][offs], 1, "N", ae_v_len(0,n1-1));
                ae_v_cmove(&a->ptr.pp_complex[offs+n1+i][offs], 1, &a->ptr.pp_complex[pivots->ptr.p_int[offs+n1+i]][offs], 1, "N", ae_v_len(offs,offs+n1-1));
                ae_v_cmove(&a->ptr.pp_complex[pivots->ptr.p_int[offs+n1+i]][offs], 1, &tmp->ptr.p_complex[0], 1, "N", ae_v_len(offs,offs+n1-1));
            }
        }
    }
}


/*************************************************************************
Recurrent real LU subroutine.
Never call it directly.

  -- ALGLIB routine --
     04.01.2010
     Bochkanov Sergey
*************************************************************************/
static void trfac_rmatrixplurec(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t n1;
    ae_int_t n2;


    
    /*
     * Basecases
     */
    if( rmatrixplumkl(a, offs, m, n, pivots, _state) )
    {
        return;
    }
    if( ae_minint(m, n, _state)<=ablasblocksize(a, _state) )
    {
        trfac_rmatrixplu2(a, offs, m, n, pivots, tmp, _state);
        return;
    }
    
    /*
     * Preliminary step, make M>=N.
     *
     * A = (A1 A2), where A1 is square
     * Factorize A1, update A2
     */
    if( n>m )
    {
        trfac_rmatrixplurec(a, offs, m, m, pivots, tmp, _state);
        for(i=0; i<=m-1; i++)
        {
            ae_v_move(&tmp->ptr.p_double[0], 1, &a->ptr.pp_double[offs+i][offs+m], 1, ae_v_len(0,n-m-1));
            ae_v_move(&a->ptr.pp_double[offs+i][offs+m], 1, &a->ptr.pp_double[pivots->ptr.p_int[offs+i]][offs+m], 1, ae_v_len(offs+m,offs+n-1));
            ae_v_move(&a->ptr.pp_double[pivots->ptr.p_int[offs+i]][offs+m], 1, &tmp->ptr.p_double[0], 1, ae_v_len(offs+m,offs+n-1));
        }
        rmatrixlefttrsm(m, n-m, a, offs, offs, ae_false, ae_true, 0, a, offs, offs+m, _state);
        return;
    }
    
    /*
     * Non-kernel case
     */
    ablassplitlength(a, n, &n1, &n2, _state);
    trfac_rmatrixplurec(a, offs, m, n1, pivots, tmp, _state);
    if( n2>0 )
    {
        for(i=0; i<=n1-1; i++)
        {
            if( offs+i!=pivots->ptr.p_int[offs+i] )
            {
                ae_v_move(&tmp->ptr.p_double[0], 1, &a->ptr.pp_double[offs+i][offs+n1], 1, ae_v_len(0,n2-1));
                ae_v_move(&a->ptr.pp_double[offs+i][offs+n1], 1, &a->ptr.pp_double[pivots->ptr.p_int[offs+i]][offs+n1], 1, ae_v_len(offs+n1,offs+n-1));
                ae_v_move(&a->ptr.pp_double[pivots->ptr.p_int[offs+i]][offs+n1], 1, &tmp->ptr.p_double[0], 1, ae_v_len(offs+n1,offs+n-1));
            }
        }
        rmatrixlefttrsm(n1, n2, a, offs, offs, ae_false, ae_true, 0, a, offs, offs+n1, _state);
        rmatrixgemm(m-n1, n-n1, n1, -1.0, a, offs+n1, offs, 0, a, offs, offs+n1, 0, 1.0, a, offs+n1, offs+n1, _state);
        trfac_rmatrixplurec(a, offs+n1, m-n1, n-n1, pivots, tmp, _state);
        for(i=0; i<=n2-1; i++)
        {
            if( offs+n1+i!=pivots->ptr.p_int[offs+n1+i] )
            {
                ae_v_move(&tmp->ptr.p_double[0], 1, &a->ptr.pp_double[offs+n1+i][offs], 1, ae_v_len(0,n1-1));
                ae_v_move(&a->ptr.pp_double[offs+n1+i][offs], 1, &a->ptr.pp_double[pivots->ptr.p_int[offs+n1+i]][offs], 1, ae_v_len(offs,offs+n1-1));
                ae_v_move(&a->ptr.pp_double[pivots->ptr.p_int[offs+n1+i]][offs], 1, &tmp->ptr.p_double[0], 1, ae_v_len(offs,offs+n1-1));
            }
        }
    }
}


/*************************************************************************
Complex LUP kernel

  -- ALGLIB routine --
     10.01.2010
     Bochkanov Sergey
*************************************************************************/
static void trfac_cmatrixlup2(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Complex */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t jp;
    ae_complex s;


    
    /*
     * Quick return if possible
     */
    if( m==0||n==0 )
    {
        return;
    }
    
    /*
     * main cycle
     */
    for(j=0; j<=ae_minint(m-1, n-1, _state); j++)
    {
        
        /*
         * Find pivot, swap columns
         */
        jp = j;
        for(i=j+1; i<=n-1; i++)
        {
            if( ae_fp_greater(ae_c_abs(a->ptr.pp_complex[offs+j][offs+i], _state),ae_c_abs(a->ptr.pp_complex[offs+j][offs+jp], _state)) )
            {
                jp = i;
            }
        }
        pivots->ptr.p_int[offs+j] = offs+jp;
        if( jp!=j )
        {
            ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs][offs+j], a->stride, "N", ae_v_len(0,m-1));
            ae_v_cmove(&a->ptr.pp_complex[offs][offs+j], a->stride, &a->ptr.pp_complex[offs][offs+jp], a->stride, "N", ae_v_len(offs,offs+m-1));
            ae_v_cmove(&a->ptr.pp_complex[offs][offs+jp], a->stride, &tmp->ptr.p_complex[0], 1, "N", ae_v_len(offs,offs+m-1));
        }
        
        /*
         * LU decomposition of 1x(N-J) matrix
         */
        if( ae_c_neq_d(a->ptr.pp_complex[offs+j][offs+j],(double)(0))&&j+1<=n-1 )
        {
            s = ae_c_d_div(1,a->ptr.pp_complex[offs+j][offs+j]);
            ae_v_cmulc(&a->ptr.pp_complex[offs+j][offs+j+1], 1, ae_v_len(offs+j+1,offs+n-1), s);
        }
        
        /*
         * Update trailing (M-J-1)x(N-J-1) matrix
         */
        if( j<ae_minint(m-1, n-1, _state) )
        {
            ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs+j+1][offs+j], a->stride, "N", ae_v_len(0,m-j-2));
            ae_v_cmoveneg(&tmp->ptr.p_complex[m], 1, &a->ptr.pp_complex[offs+j][offs+j+1], 1, "N", ae_v_len(m,m+n-j-2));
            cmatrixrank1(m-j-1, n-j-1, a, offs+j+1, offs+j+1, tmp, 0, tmp, m, _state);
        }
    }
}


/*************************************************************************
Real LUP kernel

  -- ALGLIB routine --
     10.01.2010
     Bochkanov Sergey
*************************************************************************/
static void trfac_rmatrixlup2(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t jp;
    double s;


    
    /*
     * Quick return if possible
     */
    if( m==0||n==0 )
    {
        return;
    }
    
    /*
     * main cycle
     */
    for(j=0; j<=ae_minint(m-1, n-1, _state); j++)
    {
        
        /*
         * Find pivot, swap columns
         */
        jp = j;
        for(i=j+1; i<=n-1; i++)
        {
            if( ae_fp_greater(ae_fabs(a->ptr.pp_double[offs+j][offs+i], _state),ae_fabs(a->ptr.pp_double[offs+j][offs+jp], _state)) )
            {
                jp = i;
            }
        }
        pivots->ptr.p_int[offs+j] = offs+jp;
        if( jp!=j )
        {
            ae_v_move(&tmp->ptr.p_double[0], 1, &a->ptr.pp_double[offs][offs+j], a->stride, ae_v_len(0,m-1));
            ae_v_move(&a->ptr.pp_double[offs][offs+j], a->stride, &a->ptr.pp_double[offs][offs+jp], a->stride, ae_v_len(offs,offs+m-1));
            ae_v_move(&a->ptr.pp_double[offs][offs+jp], a->stride, &tmp->ptr.p_double[0], 1, ae_v_len(offs,offs+m-1));
        }
        
        /*
         * LU decomposition of 1x(N-J) matrix
         */
        if( ae_fp_neq(a->ptr.pp_double[offs+j][offs+j],(double)(0))&&j+1<=n-1 )
        {
            s = 1/a->ptr.pp_double[offs+j][offs+j];
            ae_v_muld(&a->ptr.pp_double[offs+j][offs+j+1], 1, ae_v_len(offs+j+1,offs+n-1), s);
        }
        
        /*
         * Update trailing (M-J-1)x(N-J-1) matrix
         */
        if( j<ae_minint(m-1, n-1, _state) )
        {
            ae_v_move(&tmp->ptr.p_double[0], 1, &a->ptr.pp_double[offs+j+1][offs+j], a->stride, ae_v_len(0,m-j-2));
            ae_v_moveneg(&tmp->ptr.p_double[m], 1, &a->ptr.pp_double[offs+j][offs+j+1], 1, ae_v_len(m,m+n-j-2));
            rmatrixrank1(m-j-1, n-j-1, a, offs+j+1, offs+j+1, tmp, 0, tmp, m, _state);
        }
    }
}


/*************************************************************************
Complex PLU kernel

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     June 30, 1992
*************************************************************************/
static void trfac_cmatrixplu2(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Complex */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t jp;
    ae_complex s;


    
    /*
     * Quick return if possible
     */
    if( m==0||n==0 )
    {
        return;
    }
    for(j=0; j<=ae_minint(m-1, n-1, _state); j++)
    {
        
        /*
         * Find pivot and test for singularity.
         */
        jp = j;
        for(i=j+1; i<=m-1; i++)
        {
            if( ae_fp_greater(ae_c_abs(a->ptr.pp_complex[offs+i][offs+j], _state),ae_c_abs(a->ptr.pp_complex[offs+jp][offs+j], _state)) )
            {
                jp = i;
            }
        }
        pivots->ptr.p_int[offs+j] = offs+jp;
        if( ae_c_neq_d(a->ptr.pp_complex[offs+jp][offs+j],(double)(0)) )
        {
            
            /*
             *Apply the interchange to rows
             */
            if( jp!=j )
            {
                for(i=0; i<=n-1; i++)
                {
                    s = a->ptr.pp_complex[offs+j][offs+i];
                    a->ptr.pp_complex[offs+j][offs+i] = a->ptr.pp_complex[offs+jp][offs+i];
                    a->ptr.pp_complex[offs+jp][offs+i] = s;
                }
            }
            
            /*
             *Compute elements J+1:M of J-th column.
             */
            if( j+1<=m-1 )
            {
                s = ae_c_d_div(1,a->ptr.pp_complex[offs+j][offs+j]);
                ae_v_cmulc(&a->ptr.pp_complex[offs+j+1][offs+j], a->stride, ae_v_len(offs+j+1,offs+m-1), s);
            }
        }
        if( j<ae_minint(m, n, _state)-1 )
        {
            
            /*
             *Update trailing submatrix.
             */
            ae_v_cmove(&tmp->ptr.p_complex[0], 1, &a->ptr.pp_complex[offs+j+1][offs+j], a->stride, "N", ae_v_len(0,m-j-2));
            ae_v_cmoveneg(&tmp->ptr.p_complex[m], 1, &a->ptr.pp_complex[offs+j][offs+j+1], 1, "N", ae_v_len(m,m+n-j-2));
            cmatrixrank1(m-j-1, n-j-1, a, offs+j+1, offs+j+1, tmp, 0, tmp, m, _state);
        }
    }
}


/*************************************************************************
Real PLU kernel

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     June 30, 1992
*************************************************************************/
static void trfac_rmatrixplu2(/* Real    */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t jp;
    double s;


    
    /*
     * Quick return if possible
     */
    if( m==0||n==0 )
    {
        return;
    }
    for(j=0; j<=ae_minint(m-1, n-1, _state); j++)
    {
        
        /*
         * Find pivot and test for singularity.
         */
        jp = j;
        for(i=j+1; i<=m-1; i++)
        {
            if( ae_fp_greater(ae_fabs(a->ptr.pp_double[offs+i][offs+j], _state),ae_fabs(a->ptr.pp_double[offs+jp][offs+j], _state)) )
            {
                jp = i;
            }
        }
        pivots->ptr.p_int[offs+j] = offs+jp;
        if( ae_fp_neq(a->ptr.pp_double[offs+jp][offs+j],(double)(0)) )
        {
            
            /*
             *Apply the interchange to rows
             */
            if( jp!=j )
            {
                for(i=0; i<=n-1; i++)
                {
                    s = a->ptr.pp_double[offs+j][offs+i];
                    a->ptr.pp_double[offs+j][offs+i] = a->ptr.pp_double[offs+jp][offs+i];
                    a->ptr.pp_double[offs+jp][offs+i] = s;
                }
            }
            
            /*
             *Compute elements J+1:M of J-th column.
             */
            if( j+1<=m-1 )
            {
                s = 1/a->ptr.pp_double[offs+j][offs+j];
                ae_v_muld(&a->ptr.pp_double[offs+j+1][offs+j], a->stride, ae_v_len(offs+j+1,offs+m-1), s);
            }
        }
        if( j<ae_minint(m, n, _state)-1 )
        {
            
            /*
             *Update trailing submatrix.
             */
            ae_v_move(&tmp->ptr.p_double[0], 1, &a->ptr.pp_double[offs+j+1][offs+j], a->stride, ae_v_len(0,m-j-2));
            ae_v_moveneg(&tmp->ptr.p_double[m], 1, &a->ptr.pp_double[offs+j][offs+j+1], 1, ae_v_len(m,m+n-j-2));
            rmatrixrank1(m-j-1, n-j-1, a, offs+j+1, offs+j+1, tmp, 0, tmp, m, _state);
        }
    }
}


/*************************************************************************
Recursive computational subroutine for HPDMatrixCholesky

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
static ae_bool trfac_hpdmatrixcholeskyrec(/* Complex */ ae_matrix* a,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t n1;
    ae_int_t n2;
    ae_bool result;


    
    /*
     * check N
     */
    if( n<1 )
    {
        result = ae_false;
        return result;
    }
    
    /*
     * Prepare buffer
     */
    if( tmp->cnt<2*n )
    {
        ae_vector_set_length(tmp, 2*n, _state);
    }
    
    /*
     * special cases
     *
     * NOTE: we do not use MKL for basecases because their price is only
     *       minor part of overall running time for N>256.
     */
    if( n==1 )
    {
        if( ae_fp_greater(a->ptr.pp_complex[offs][offs].x,(double)(0)) )
        {
            a->ptr.pp_complex[offs][offs] = ae_complex_from_d(ae_sqrt(a->ptr.pp_complex[offs][offs].x, _state));
            result = ae_true;
        }
        else
        {
            result = ae_false;
        }
        return result;
    }
    if( n<=ablascomplexblocksize(a, _state) )
    {
        result = trfac_hpdmatrixcholesky2(a, offs, n, isupper, tmp, _state);
        return result;
    }
    
    /*
     * general case: split task in cache-oblivious manner
     */
    result = ae_true;
    ablascomplexsplitlength(a, n, &n1, &n2, _state);
    result = trfac_hpdmatrixcholeskyrec(a, offs, n1, isupper, tmp, _state);
    if( !result )
    {
        return result;
    }
    if( n2>0 )
    {
        if( isupper )
        {
            cmatrixlefttrsm(n1, n2, a, offs, offs, isupper, ae_false, 2, a, offs, offs+n1, _state);
            cmatrixherk(n2, n1, -1.0, a, offs, offs+n1, 2, 1.0, a, offs+n1, offs+n1, isupper, _state);
        }
        else
        {
            cmatrixrighttrsm(n2, n1, a, offs, offs, isupper, ae_false, 2, a, offs+n1, offs, _state);
            cmatrixherk(n2, n1, -1.0, a, offs+n1, offs, 0, 1.0, a, offs+n1, offs+n1, isupper, _state);
        }
        result = trfac_hpdmatrixcholeskyrec(a, offs+n1, n2, isupper, tmp, _state);
        if( !result )
        {
            return result;
        }
    }
    return result;
}


/*************************************************************************
Level-2 Hermitian Cholesky subroutine.

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     February 29, 1992
*************************************************************************/
static ae_bool trfac_hpdmatrixcholesky2(/* Complex */ ae_matrix* aaa,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double ajj;
    ae_complex v;
    double r;
    ae_bool result;


    result = ae_true;
    if( n<0 )
    {
        result = ae_false;
        return result;
    }
    
    /*
     * Quick return if possible
     */
    if( n==0 )
    {
        return result;
    }
    if( isupper )
    {
        
        /*
         * Compute the Cholesky factorization A = U'*U.
         */
        for(j=0; j<=n-1; j++)
        {
            
            /*
             * Compute U(J,J) and test for non-positive-definiteness.
             */
            v = ae_v_cdotproduct(&aaa->ptr.pp_complex[offs][offs+j], aaa->stride, "Conj", &aaa->ptr.pp_complex[offs][offs+j], aaa->stride, "N", ae_v_len(offs,offs+j-1));
            ajj = ae_c_sub(aaa->ptr.pp_complex[offs+j][offs+j],v).x;
            if( ae_fp_less_eq(ajj,(double)(0)) )
            {
                aaa->ptr.pp_complex[offs+j][offs+j] = ae_complex_from_d(ajj);
                result = ae_false;
                return result;
            }
            ajj = ae_sqrt(ajj, _state);
            aaa->ptr.pp_complex[offs+j][offs+j] = ae_complex_from_d(ajj);
            
            /*
             * Compute elements J+1:N-1 of row J.
             */
            if( j<n-1 )
            {
                if( j>0 )
                {
                    ae_v_cmoveneg(&tmp->ptr.p_complex[0], 1, &aaa->ptr.pp_complex[offs][offs+j], aaa->stride, "Conj", ae_v_len(0,j-1));
                    cmatrixmv(n-j-1, j, aaa, offs, offs+j+1, 1, tmp, 0, tmp, n, _state);
                    ae_v_cadd(&aaa->ptr.pp_complex[offs+j][offs+j+1], 1, &tmp->ptr.p_complex[n], 1, "N", ae_v_len(offs+j+1,offs+n-1));
                }
                r = 1/ajj;
                ae_v_cmuld(&aaa->ptr.pp_complex[offs+j][offs+j+1], 1, ae_v_len(offs+j+1,offs+n-1), r);
            }
        }
    }
    else
    {
        
        /*
         * Compute the Cholesky factorization A = L*L'.
         */
        for(j=0; j<=n-1; j++)
        {
            
            /*
             * Compute L(J+1,J+1) and test for non-positive-definiteness.
             */
            v = ae_v_cdotproduct(&aaa->ptr.pp_complex[offs+j][offs], 1, "Conj", &aaa->ptr.pp_complex[offs+j][offs], 1, "N", ae_v_len(offs,offs+j-1));
            ajj = ae_c_sub(aaa->ptr.pp_complex[offs+j][offs+j],v).x;
            if( ae_fp_less_eq(ajj,(double)(0)) )
            {
                aaa->ptr.pp_complex[offs+j][offs+j] = ae_complex_from_d(ajj);
                result = ae_false;
                return result;
            }
            ajj = ae_sqrt(ajj, _state);
            aaa->ptr.pp_complex[offs+j][offs+j] = ae_complex_from_d(ajj);
            
            /*
             * Compute elements J+1:N of column J.
             */
            if( j<n-1 )
            {
                if( j>0 )
                {
                    ae_v_cmove(&tmp->ptr.p_complex[0], 1, &aaa->ptr.pp_complex[offs+j][offs], 1, "Conj", ae_v_len(0,j-1));
                    cmatrixmv(n-j-1, j, aaa, offs+j+1, offs, 0, tmp, 0, tmp, n, _state);
                    for(i=0; i<=n-j-2; i++)
                    {
                        aaa->ptr.pp_complex[offs+j+1+i][offs+j] = ae_c_div_d(ae_c_sub(aaa->ptr.pp_complex[offs+j+1+i][offs+j],tmp->ptr.p_complex[n+i]),ajj);
                    }
                }
                else
                {
                    for(i=0; i<=n-j-2; i++)
                    {
                        aaa->ptr.pp_complex[offs+j+1+i][offs+j] = ae_c_div_d(aaa->ptr.pp_complex[offs+j+1+i][offs+j],ajj);
                    }
                }
            }
        }
    }
    return result;
}


/*************************************************************************
Level-2 Cholesky subroutine

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     February 29, 1992
*************************************************************************/
static ae_bool trfac_spdmatrixcholesky2(/* Real    */ ae_matrix* aaa,
     ae_int_t offs,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double ajj;
    double v;
    double r;
    ae_bool result;


    result = ae_true;
    if( n<0 )
    {
        result = ae_false;
        return result;
    }
    
    /*
     * Quick return if possible
     */
    if( n==0 )
    {
        return result;
    }
    if( isupper )
    {
        
        /*
         * Compute the Cholesky factorization A = U'*U.
         */
        for(j=0; j<=n-1; j++)
        {
            
            /*
             * Compute U(J,J) and test for non-positive-definiteness.
             */
            v = ae_v_dotproduct(&aaa->ptr.pp_double[offs][offs+j], aaa->stride, &aaa->ptr.pp_double[offs][offs+j], aaa->stride, ae_v_len(offs,offs+j-1));
            ajj = aaa->ptr.pp_double[offs+j][offs+j]-v;
            if( ae_fp_less_eq(ajj,(double)(0)) )
            {
                aaa->ptr.pp_double[offs+j][offs+j] = ajj;
                result = ae_false;
                return result;
            }
            ajj = ae_sqrt(ajj, _state);
            aaa->ptr.pp_double[offs+j][offs+j] = ajj;
            
            /*
             * Compute elements J+1:N-1 of row J.
             */
            if( j<n-1 )
            {
                if( j>0 )
                {
                    ae_v_moveneg(&tmp->ptr.p_double[0], 1, &aaa->ptr.pp_double[offs][offs+j], aaa->stride, ae_v_len(0,j-1));
                    rmatrixmv(n-j-1, j, aaa, offs, offs+j+1, 1, tmp, 0, tmp, n, _state);
                    ae_v_add(&aaa->ptr.pp_double[offs+j][offs+j+1], 1, &tmp->ptr.p_double[n], 1, ae_v_len(offs+j+1,offs+n-1));
                }
                r = 1/ajj;
                ae_v_muld(&aaa->ptr.pp_double[offs+j][offs+j+1], 1, ae_v_len(offs+j+1,offs+n-1), r);
            }
        }
    }
    else
    {
        
        /*
         * Compute the Cholesky factorization A = L*L'.
         */
        for(j=0; j<=n-1; j++)
        {
            
            /*
             * Compute L(J+1,J+1) and test for non-positive-definiteness.
             */
            v = ae_v_dotproduct(&aaa->ptr.pp_double[offs+j][offs], 1, &aaa->ptr.pp_double[offs+j][offs], 1, ae_v_len(offs,offs+j-1));
            ajj = aaa->ptr.pp_double[offs+j][offs+j]-v;
            if( ae_fp_less_eq(ajj,(double)(0)) )
            {
                aaa->ptr.pp_double[offs+j][offs+j] = ajj;
                result = ae_false;
                return result;
            }
            ajj = ae_sqrt(ajj, _state);
            aaa->ptr.pp_double[offs+j][offs+j] = ajj;
            
            /*
             * Compute elements J+1:N of column J.
             */
            if( j<n-1 )
            {
                if( j>0 )
                {
                    ae_v_move(&tmp->ptr.p_double[0], 1, &aaa->ptr.pp_double[offs+j][offs], 1, ae_v_len(0,j-1));
                    rmatrixmv(n-j-1, j, aaa, offs+j+1, offs, 0, tmp, 0, tmp, n, _state);
                    for(i=0; i<=n-j-2; i++)
                    {
                        aaa->ptr.pp_double[offs+j+1+i][offs+j] = (aaa->ptr.pp_double[offs+j+1+i][offs+j]-tmp->ptr.p_double[n+i])/ajj;
                    }
                }
                else
                {
                    for(i=0; i<=n-j-2; i++)
                    {
                        aaa->ptr.pp_double[offs+j+1+i][offs+j] = aaa->ptr.pp_double[offs+j+1+i][offs+j]/ajj;
                    }
                }
            }
        }
    }
    return result;
}


/*$ End $*/
