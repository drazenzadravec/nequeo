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

#ifndef _trfac_h
#define _trfac_h

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


/*$ Declarations $*/


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
     ae_state *_state);
void _pexec_rmatrixlu(/* Real    */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Integer */ ae_vector* pivots, ae_state *_state);


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
     ae_state *_state);
void _pexec_cmatrixlu(/* Complex */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Integer */ ae_vector* pivots, ae_state *_state);


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
     ae_state *_state);
ae_bool _pexec_hpdmatrixcholesky(/* Complex */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper, ae_state *_state);


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
     ae_state *_state);
ae_bool _pexec_spdmatrixcholesky(/* Real    */ ae_matrix* a,
    ae_int_t n,
    ae_bool isupper, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


void rmatrixlup(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state);


void cmatrixlup(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state);


void rmatrixplu(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state);


void cmatrixplu(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state);


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
     ae_state *_state);


/*$ End $*/
#endif

