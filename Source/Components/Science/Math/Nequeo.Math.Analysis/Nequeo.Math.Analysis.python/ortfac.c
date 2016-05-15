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
#include "ortfac.h"


/*$ Declarations $*/
static void ortfac_cmatrixqrbasecase(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_vector* work,
     /* Complex */ ae_vector* t,
     /* Complex */ ae_vector* tau,
     ae_state *_state);
static void ortfac_cmatrixlqbasecase(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_vector* work,
     /* Complex */ ae_vector* t,
     /* Complex */ ae_vector* tau,
     ae_state *_state);
static void ortfac_rmatrixblockreflector(/* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* tau,
     ae_bool columnwisea,
     ae_int_t lengtha,
     ae_int_t blocksize,
     /* Real    */ ae_matrix* t,
     /* Real    */ ae_vector* work,
     ae_state *_state);
static void ortfac_cmatrixblockreflector(/* Complex */ ae_matrix* a,
     /* Complex */ ae_vector* tau,
     ae_bool columnwisea,
     ae_int_t lengtha,
     ae_int_t blocksize,
     /* Complex */ ae_matrix* t,
     /* Complex */ ae_vector* work,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
QR decomposition of a rectangular matrix of size MxN

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
  ! of this function. We should note that QP decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=512,   achieving
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
    A   -   matrix A whose indexes range within [0..M-1, 0..N-1].
    M   -   number of rows in matrix A.
    N   -   number of columns in matrix A.

Output parameters:
    A   -   matrices Q and R in compact form (see below).
    Tau -   array of scalar factors which are used to form
            matrix Q. Array whose index ranges within [0.. Min(M-1,N-1)].

Matrix A is represented as A = QR, where Q is an orthogonal matrix of size
MxM, R - upper triangular (or upper trapezoid) matrix of size M x N.

The elements of matrix R are located on and above the main diagonal of
matrix A. The elements which are located in Tau array and below the main
diagonal of matrix A are used to form matrix Q as follows:

Matrix Q is represented as a product of elementary reflections

Q = H(0)*H(2)*...*H(k-1),

where k = min(m,n), and each H(i) is in the form

H(i) = 1 - tau * v * (v^T)

where tau is a scalar stored in Tau[I]; v - real vector,
so that v(0:i-1) = 0, v(i) = 1, v(i+1:m-1) stored in A(i+1:m-1,i).

  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixqr(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* tau,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_vector t;
    ae_vector taubuf;
    ae_int_t minmn;
    ae_matrix tmpa;
    ae_matrix tmpt;
    ae_matrix tmpr;
    ae_int_t blockstart;
    ae_int_t blocksize;
    ae_int_t rowscount;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(tau);
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&taubuf, 0, DT_REAL, _state);
    ae_matrix_init(&tmpa, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tmpt, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tmpr, 0, 0, DT_REAL, _state);

    if( m<=0||n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    minmn = ae_minint(m, n, _state);
    ae_vector_set_length(&work, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(&t, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(tau, minmn, _state);
    ae_vector_set_length(&taubuf, minmn, _state);
    ae_matrix_set_length(&tmpa, m, ablasblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpt, ablasblocksize(a, _state), 2*ablasblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpr, 2*ablasblocksize(a, _state), n, _state);
    
    /*
     * Blocked code
     */
    blockstart = 0;
    while(blockstart!=minmn)
    {
        
        /*
         * Determine block size
         */
        blocksize = minmn-blockstart;
        if( blocksize>ablasblocksize(a, _state) )
        {
            blocksize = ablasblocksize(a, _state);
        }
        rowscount = m-blockstart;
        
        /*
         * QR decomposition of submatrix.
         * Matrix is copied to temporary storage to solve
         * some TLB issues arising from non-contiguous memory
         * access pattern.
         */
        rmatrixcopy(rowscount, blocksize, a, blockstart, blockstart, &tmpa, 0, 0, _state);
        rmatrixqrbasecase(&tmpa, rowscount, blocksize, &work, &t, &taubuf, _state);
        rmatrixcopy(rowscount, blocksize, &tmpa, 0, 0, a, blockstart, blockstart, _state);
        ae_v_move(&tau->ptr.p_double[blockstart], 1, &taubuf.ptr.p_double[0], 1, ae_v_len(blockstart,blockstart+blocksize-1));
        
        /*
         * Update the rest, choose between:
         * a) Level 2 algorithm (when the rest of the matrix is small enough)
         * b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
         *    representation for products of Householder transformations',
         *    by R. Schreiber and C. Van Loan.
         */
        if( blockstart+blocksize<=n-1 )
        {
            if( n-blockstart-blocksize>=2*ablasblocksize(a, _state)||rowscount>=4*ablasblocksize(a, _state) )
            {
                
                /*
                 * Prepare block reflector
                 */
                ortfac_rmatrixblockreflector(&tmpa, &taubuf, ae_true, rowscount, blocksize, &tmpt, &work, _state);
                
                /*
                 * Multiply the rest of A by Q'.
                 *
                 * Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                 * Q' = E + Y*T'*Y' = E + TmpA*TmpT'*TmpA'
                 */
                rmatrixgemm(blocksize, n-blockstart-blocksize, rowscount, 1.0, &tmpa, 0, 0, 1, a, blockstart, blockstart+blocksize, 0, 0.0, &tmpr, 0, 0, _state);
                rmatrixgemm(blocksize, n-blockstart-blocksize, blocksize, 1.0, &tmpt, 0, 0, 1, &tmpr, 0, 0, 0, 0.0, &tmpr, blocksize, 0, _state);
                rmatrixgemm(rowscount, n-blockstart-blocksize, blocksize, 1.0, &tmpa, 0, 0, 0, &tmpr, blocksize, 0, 0, 1.0, a, blockstart, blockstart+blocksize, _state);
            }
            else
            {
                
                /*
                 * Level 2 algorithm
                 */
                for(i=0; i<=blocksize-1; i++)
                {
                    ae_v_move(&t.ptr.p_double[1], 1, &tmpa.ptr.pp_double[i][i], tmpa.stride, ae_v_len(1,rowscount-i));
                    t.ptr.p_double[1] = (double)(1);
                    applyreflectionfromtheleft(a, taubuf.ptr.p_double[i], &t, blockstart+i, m-1, blockstart+blocksize, n-1, &work, _state);
                }
            }
        }
        
        /*
         * Advance
         */
        blockstart = blockstart+blocksize;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixqr(/* Real    */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Real    */ ae_vector* tau, ae_state *_state)
{
    rmatrixqr(a,m,n,tau, _state);
}


/*************************************************************************
LQ decomposition of a rectangular matrix of size MxN

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
  ! of this function. We should note that QP decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=512,   achieving
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
    A   -   matrix A whose indexes range within [0..M-1, 0..N-1].
    M   -   number of rows in matrix A.
    N   -   number of columns in matrix A.

Output parameters:
    A   -   matrices L and Q in compact form (see below)
    Tau -   array of scalar factors which are used to form
            matrix Q. Array whose index ranges within [0..Min(M,N)-1].

Matrix A is represented as A = LQ, where Q is an orthogonal matrix of size
MxM, L - lower triangular (or lower trapezoid) matrix of size M x N.

The elements of matrix L are located on and below  the  main  diagonal  of
matrix A. The elements which are located in Tau array and above  the  main
diagonal of matrix A are used to form matrix Q as follows:

Matrix Q is represented as a product of elementary reflections

Q = H(k-1)*H(k-2)*...*H(1)*H(0),

where k = min(m,n), and each H(i) is of the form

H(i) = 1 - tau * v * (v^T)

where tau is a scalar stored in Tau[I]; v - real vector, so that v(0:i-1)=0,
v(i) = 1, v(i+1:n-1) stored in A(i,i+1:n-1).

  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixlq(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* tau,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_vector t;
    ae_vector taubuf;
    ae_int_t minmn;
    ae_matrix tmpa;
    ae_matrix tmpt;
    ae_matrix tmpr;
    ae_int_t blockstart;
    ae_int_t blocksize;
    ae_int_t columnscount;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(tau);
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&taubuf, 0, DT_REAL, _state);
    ae_matrix_init(&tmpa, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tmpt, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tmpr, 0, 0, DT_REAL, _state);

    if( m<=0||n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    minmn = ae_minint(m, n, _state);
    ae_vector_set_length(&work, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(&t, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(tau, minmn, _state);
    ae_vector_set_length(&taubuf, minmn, _state);
    ae_matrix_set_length(&tmpa, ablasblocksize(a, _state), n, _state);
    ae_matrix_set_length(&tmpt, ablasblocksize(a, _state), 2*ablasblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpr, m, 2*ablasblocksize(a, _state), _state);
    
    /*
     * Blocked code
     */
    blockstart = 0;
    while(blockstart!=minmn)
    {
        
        /*
         * Determine block size
         */
        blocksize = minmn-blockstart;
        if( blocksize>ablasblocksize(a, _state) )
        {
            blocksize = ablasblocksize(a, _state);
        }
        columnscount = n-blockstart;
        
        /*
         * LQ decomposition of submatrix.
         * Matrix is copied to temporary storage to solve
         * some TLB issues arising from non-contiguous memory
         * access pattern.
         */
        rmatrixcopy(blocksize, columnscount, a, blockstart, blockstart, &tmpa, 0, 0, _state);
        rmatrixlqbasecase(&tmpa, blocksize, columnscount, &work, &t, &taubuf, _state);
        rmatrixcopy(blocksize, columnscount, &tmpa, 0, 0, a, blockstart, blockstart, _state);
        ae_v_move(&tau->ptr.p_double[blockstart], 1, &taubuf.ptr.p_double[0], 1, ae_v_len(blockstart,blockstart+blocksize-1));
        
        /*
         * Update the rest, choose between:
         * a) Level 2 algorithm (when the rest of the matrix is small enough)
         * b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
         *    representation for products of Householder transformations',
         *    by R. Schreiber and C. Van Loan.
         */
        if( blockstart+blocksize<=m-1 )
        {
            if( m-blockstart-blocksize>=2*ablasblocksize(a, _state) )
            {
                
                /*
                 * Prepare block reflector
                 */
                ortfac_rmatrixblockreflector(&tmpa, &taubuf, ae_false, columnscount, blocksize, &tmpt, &work, _state);
                
                /*
                 * Multiply the rest of A by Q.
                 *
                 * Q  = E + Y*T*Y'  = E + TmpA'*TmpT*TmpA
                 */
                rmatrixgemm(m-blockstart-blocksize, blocksize, columnscount, 1.0, a, blockstart+blocksize, blockstart, 0, &tmpa, 0, 0, 1, 0.0, &tmpr, 0, 0, _state);
                rmatrixgemm(m-blockstart-blocksize, blocksize, blocksize, 1.0, &tmpr, 0, 0, 0, &tmpt, 0, 0, 0, 0.0, &tmpr, 0, blocksize, _state);
                rmatrixgemm(m-blockstart-blocksize, columnscount, blocksize, 1.0, &tmpr, 0, blocksize, 0, &tmpa, 0, 0, 0, 1.0, a, blockstart+blocksize, blockstart, _state);
            }
            else
            {
                
                /*
                 * Level 2 algorithm
                 */
                for(i=0; i<=blocksize-1; i++)
                {
                    ae_v_move(&t.ptr.p_double[1], 1, &tmpa.ptr.pp_double[i][i], 1, ae_v_len(1,columnscount-i));
                    t.ptr.p_double[1] = (double)(1);
                    applyreflectionfromtheright(a, taubuf.ptr.p_double[i], &t, blockstart+blocksize, m-1, blockstart+i, n-1, &work, _state);
                }
            }
        }
        
        /*
         * Advance
         */
        blockstart = blockstart+blocksize;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixlq(/* Real    */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Real    */ ae_vector* tau, ae_state *_state)
{
    rmatrixlq(a,m,n,tau, _state);
}


/*************************************************************************
QR decomposition of a rectangular complex matrix of size MxN

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
  ! of this function. We should note that QP decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=512,   achieving
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
    A   -   matrix A whose indexes range within [0..M-1, 0..N-1]
    M   -   number of rows in matrix A.
    N   -   number of columns in matrix A.

Output parameters:
    A   -   matrices Q and R in compact form
    Tau -   array of scalar factors which are used to form matrix Q. Array
            whose indexes range within [0.. Min(M,N)-1]

Matrix A is represented as A = QR, where Q is an orthogonal matrix of size
MxM, R - upper triangular (or upper trapezoid) matrix of size MxN.

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994
*************************************************************************/
void cmatrixqr(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_vector* tau,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_vector t;
    ae_vector taubuf;
    ae_int_t minmn;
    ae_matrix tmpa;
    ae_matrix tmpt;
    ae_matrix tmpr;
    ae_int_t blockstart;
    ae_int_t blocksize;
    ae_int_t rowscount;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(tau);
    ae_vector_init(&work, 0, DT_COMPLEX, _state);
    ae_vector_init(&t, 0, DT_COMPLEX, _state);
    ae_vector_init(&taubuf, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpa, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpt, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpr, 0, 0, DT_COMPLEX, _state);

    if( m<=0||n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    minmn = ae_minint(m, n, _state);
    ae_vector_set_length(&work, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(&t, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(tau, minmn, _state);
    ae_vector_set_length(&taubuf, minmn, _state);
    ae_matrix_set_length(&tmpa, m, ablascomplexblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpt, ablascomplexblocksize(a, _state), ablascomplexblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpr, 2*ablascomplexblocksize(a, _state), n, _state);
    
    /*
     * Blocked code
     */
    blockstart = 0;
    while(blockstart!=minmn)
    {
        
        /*
         * Determine block size
         */
        blocksize = minmn-blockstart;
        if( blocksize>ablascomplexblocksize(a, _state) )
        {
            blocksize = ablascomplexblocksize(a, _state);
        }
        rowscount = m-blockstart;
        
        /*
         * QR decomposition of submatrix.
         * Matrix is copied to temporary storage to solve
         * some TLB issues arising from non-contiguous memory
         * access pattern.
         */
        cmatrixcopy(rowscount, blocksize, a, blockstart, blockstart, &tmpa, 0, 0, _state);
        ortfac_cmatrixqrbasecase(&tmpa, rowscount, blocksize, &work, &t, &taubuf, _state);
        cmatrixcopy(rowscount, blocksize, &tmpa, 0, 0, a, blockstart, blockstart, _state);
        ae_v_cmove(&tau->ptr.p_complex[blockstart], 1, &taubuf.ptr.p_complex[0], 1, "N", ae_v_len(blockstart,blockstart+blocksize-1));
        
        /*
         * Update the rest, choose between:
         * a) Level 2 algorithm (when the rest of the matrix is small enough)
         * b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
         *    representation for products of Householder transformations',
         *    by R. Schreiber and C. Van Loan.
         */
        if( blockstart+blocksize<=n-1 )
        {
            if( n-blockstart-blocksize>=2*ablascomplexblocksize(a, _state) )
            {
                
                /*
                 * Prepare block reflector
                 */
                ortfac_cmatrixblockreflector(&tmpa, &taubuf, ae_true, rowscount, blocksize, &tmpt, &work, _state);
                
                /*
                 * Multiply the rest of A by Q'.
                 *
                 * Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                 * Q' = E + Y*T'*Y' = E + TmpA*TmpT'*TmpA'
                 */
                cmatrixgemm(blocksize, n-blockstart-blocksize, rowscount, ae_complex_from_d(1.0), &tmpa, 0, 0, 2, a, blockstart, blockstart+blocksize, 0, ae_complex_from_d(0.0), &tmpr, 0, 0, _state);
                cmatrixgemm(blocksize, n-blockstart-blocksize, blocksize, ae_complex_from_d(1.0), &tmpt, 0, 0, 2, &tmpr, 0, 0, 0, ae_complex_from_d(0.0), &tmpr, blocksize, 0, _state);
                cmatrixgemm(rowscount, n-blockstart-blocksize, blocksize, ae_complex_from_d(1.0), &tmpa, 0, 0, 0, &tmpr, blocksize, 0, 0, ae_complex_from_d(1.0), a, blockstart, blockstart+blocksize, _state);
            }
            else
            {
                
                /*
                 * Level 2 algorithm
                 */
                for(i=0; i<=blocksize-1; i++)
                {
                    ae_v_cmove(&t.ptr.p_complex[1], 1, &tmpa.ptr.pp_complex[i][i], tmpa.stride, "N", ae_v_len(1,rowscount-i));
                    t.ptr.p_complex[1] = ae_complex_from_i(1);
                    complexapplyreflectionfromtheleft(a, ae_c_conj(taubuf.ptr.p_complex[i], _state), &t, blockstart+i, m-1, blockstart+blocksize, n-1, &work, _state);
                }
            }
        }
        
        /*
         * Advance
         */
        blockstart = blockstart+blocksize;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixqr(/* Complex */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Complex */ ae_vector* tau, ae_state *_state)
{
    cmatrixqr(a,m,n,tau, _state);
}


/*************************************************************************
LQ decomposition of a rectangular complex matrix of size MxN

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
  ! of this function. We should note that QP decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=512,   achieving
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
    A   -   matrix A whose indexes range within [0..M-1, 0..N-1]
    M   -   number of rows in matrix A.
    N   -   number of columns in matrix A.

Output parameters:
    A   -   matrices Q and L in compact form
    Tau -   array of scalar factors which are used to form matrix Q. Array
            whose indexes range within [0.. Min(M,N)-1]

Matrix A is represented as A = LQ, where Q is an orthogonal matrix of size
MxM, L - lower triangular (or lower trapezoid) matrix of size MxN.

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994
*************************************************************************/
void cmatrixlq(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_vector* tau,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_vector t;
    ae_vector taubuf;
    ae_int_t minmn;
    ae_matrix tmpa;
    ae_matrix tmpt;
    ae_matrix tmpr;
    ae_int_t blockstart;
    ae_int_t blocksize;
    ae_int_t columnscount;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(tau);
    ae_vector_init(&work, 0, DT_COMPLEX, _state);
    ae_vector_init(&t, 0, DT_COMPLEX, _state);
    ae_vector_init(&taubuf, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpa, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpt, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpr, 0, 0, DT_COMPLEX, _state);

    if( m<=0||n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    minmn = ae_minint(m, n, _state);
    ae_vector_set_length(&work, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(&t, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(tau, minmn, _state);
    ae_vector_set_length(&taubuf, minmn, _state);
    ae_matrix_set_length(&tmpa, ablascomplexblocksize(a, _state), n, _state);
    ae_matrix_set_length(&tmpt, ablascomplexblocksize(a, _state), ablascomplexblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpr, m, 2*ablascomplexblocksize(a, _state), _state);
    
    /*
     * Blocked code
     */
    blockstart = 0;
    while(blockstart!=minmn)
    {
        
        /*
         * Determine block size
         */
        blocksize = minmn-blockstart;
        if( blocksize>ablascomplexblocksize(a, _state) )
        {
            blocksize = ablascomplexblocksize(a, _state);
        }
        columnscount = n-blockstart;
        
        /*
         * LQ decomposition of submatrix.
         * Matrix is copied to temporary storage to solve
         * some TLB issues arising from non-contiguous memory
         * access pattern.
         */
        cmatrixcopy(blocksize, columnscount, a, blockstart, blockstart, &tmpa, 0, 0, _state);
        ortfac_cmatrixlqbasecase(&tmpa, blocksize, columnscount, &work, &t, &taubuf, _state);
        cmatrixcopy(blocksize, columnscount, &tmpa, 0, 0, a, blockstart, blockstart, _state);
        ae_v_cmove(&tau->ptr.p_complex[blockstart], 1, &taubuf.ptr.p_complex[0], 1, "N", ae_v_len(blockstart,blockstart+blocksize-1));
        
        /*
         * Update the rest, choose between:
         * a) Level 2 algorithm (when the rest of the matrix is small enough)
         * b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
         *    representation for products of Householder transformations',
         *    by R. Schreiber and C. Van Loan.
         */
        if( blockstart+blocksize<=m-1 )
        {
            if( m-blockstart-blocksize>=2*ablascomplexblocksize(a, _state) )
            {
                
                /*
                 * Prepare block reflector
                 */
                ortfac_cmatrixblockreflector(&tmpa, &taubuf, ae_false, columnscount, blocksize, &tmpt, &work, _state);
                
                /*
                 * Multiply the rest of A by Q.
                 *
                 * Q  = E + Y*T*Y'  = E + TmpA'*TmpT*TmpA
                 */
                cmatrixgemm(m-blockstart-blocksize, blocksize, columnscount, ae_complex_from_d(1.0), a, blockstart+blocksize, blockstart, 0, &tmpa, 0, 0, 2, ae_complex_from_d(0.0), &tmpr, 0, 0, _state);
                cmatrixgemm(m-blockstart-blocksize, blocksize, blocksize, ae_complex_from_d(1.0), &tmpr, 0, 0, 0, &tmpt, 0, 0, 0, ae_complex_from_d(0.0), &tmpr, 0, blocksize, _state);
                cmatrixgemm(m-blockstart-blocksize, columnscount, blocksize, ae_complex_from_d(1.0), &tmpr, 0, blocksize, 0, &tmpa, 0, 0, 0, ae_complex_from_d(1.0), a, blockstart+blocksize, blockstart, _state);
            }
            else
            {
                
                /*
                 * Level 2 algorithm
                 */
                for(i=0; i<=blocksize-1; i++)
                {
                    ae_v_cmove(&t.ptr.p_complex[1], 1, &tmpa.ptr.pp_complex[i][i], 1, "Conj", ae_v_len(1,columnscount-i));
                    t.ptr.p_complex[1] = ae_complex_from_i(1);
                    complexapplyreflectionfromtheright(a, taubuf.ptr.p_complex[i], &t, blockstart+blocksize, m-1, blockstart+i, n-1, &work, _state);
                }
            }
        }
        
        /*
         * Advance
         */
        blockstart = blockstart+blocksize;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixlq(/* Complex */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Complex */ ae_vector* tau, ae_state *_state)
{
    cmatrixlq(a,m,n,tau, _state);
}


/*************************************************************************
Partial unpacking of matrix Q from the QR decomposition of a matrix A

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
  ! of this function. We should note that QP decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=512,   achieving
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
    A       -   matrices Q and R in compact form.
                Output of RMatrixQR subroutine.
    M       -   number of rows in given matrix A. M>=0.
    N       -   number of columns in given matrix A. N>=0.
    Tau     -   scalar factors which are used to form Q.
                Output of the RMatrixQR subroutine.
    QColumns -  required number of columns of matrix Q. M>=QColumns>=0.

Output parameters:
    Q       -   first QColumns columns of matrix Q.
                Array whose indexes range within [0..M-1, 0..QColumns-1].
                If QColumns=0, the array remains unchanged.

  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixqrunpackq(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* tau,
     ae_int_t qcolumns,
     /* Real    */ ae_matrix* q,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_vector t;
    ae_vector taubuf;
    ae_int_t minmn;
    ae_int_t refcnt;
    ae_matrix tmpa;
    ae_matrix tmpt;
    ae_matrix tmpr;
    ae_int_t blockstart;
    ae_int_t blocksize;
    ae_int_t rowscount;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(q);
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&taubuf, 0, DT_REAL, _state);
    ae_matrix_init(&tmpa, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tmpt, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tmpr, 0, 0, DT_REAL, _state);

    ae_assert(qcolumns<=m, "UnpackQFromQR: QColumns>M!", _state);
    if( (m<=0||n<=0)||qcolumns<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * init
     */
    minmn = ae_minint(m, n, _state);
    refcnt = ae_minint(minmn, qcolumns, _state);
    ae_matrix_set_length(q, m, qcolumns, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=qcolumns-1; j++)
        {
            if( i==j )
            {
                q->ptr.pp_double[i][j] = (double)(1);
            }
            else
            {
                q->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    ae_vector_set_length(&work, ae_maxint(m, qcolumns, _state)+1, _state);
    ae_vector_set_length(&t, ae_maxint(m, qcolumns, _state)+1, _state);
    ae_vector_set_length(&taubuf, minmn, _state);
    ae_matrix_set_length(&tmpa, m, ablasblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpt, ablasblocksize(a, _state), 2*ablasblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpr, 2*ablasblocksize(a, _state), qcolumns, _state);
    
    /*
     * Blocked code
     */
    blockstart = ablasblocksize(a, _state)*(refcnt/ablasblocksize(a, _state));
    blocksize = refcnt-blockstart;
    while(blockstart>=0)
    {
        rowscount = m-blockstart;
        if( blocksize>0 )
        {
            
            /*
             * Copy current block
             */
            rmatrixcopy(rowscount, blocksize, a, blockstart, blockstart, &tmpa, 0, 0, _state);
            ae_v_move(&taubuf.ptr.p_double[0], 1, &tau->ptr.p_double[blockstart], 1, ae_v_len(0,blocksize-1));
            
            /*
             * Update, choose between:
             * a) Level 2 algorithm (when the rest of the matrix is small enough)
             * b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
             *    representation for products of Householder transformations',
             *    by R. Schreiber and C. Van Loan.
             */
            if( qcolumns>=2*ablasblocksize(a, _state) )
            {
                
                /*
                 * Prepare block reflector
                 */
                ortfac_rmatrixblockreflector(&tmpa, &taubuf, ae_true, rowscount, blocksize, &tmpt, &work, _state);
                
                /*
                 * Multiply matrix by Q.
                 *
                 * Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                 */
                rmatrixgemm(blocksize, qcolumns, rowscount, 1.0, &tmpa, 0, 0, 1, q, blockstart, 0, 0, 0.0, &tmpr, 0, 0, _state);
                rmatrixgemm(blocksize, qcolumns, blocksize, 1.0, &tmpt, 0, 0, 0, &tmpr, 0, 0, 0, 0.0, &tmpr, blocksize, 0, _state);
                rmatrixgemm(rowscount, qcolumns, blocksize, 1.0, &tmpa, 0, 0, 0, &tmpr, blocksize, 0, 0, 1.0, q, blockstart, 0, _state);
            }
            else
            {
                
                /*
                 * Level 2 algorithm
                 */
                for(i=blocksize-1; i>=0; i--)
                {
                    ae_v_move(&t.ptr.p_double[1], 1, &tmpa.ptr.pp_double[i][i], tmpa.stride, ae_v_len(1,rowscount-i));
                    t.ptr.p_double[1] = (double)(1);
                    applyreflectionfromtheleft(q, taubuf.ptr.p_double[i], &t, blockstart+i, m-1, 0, qcolumns-1, &work, _state);
                }
            }
        }
        
        /*
         * Advance
         */
        blockstart = blockstart-ablasblocksize(a, _state);
        blocksize = ablasblocksize(a, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixqrunpackq(/* Real    */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Real    */ ae_vector* tau,
    ae_int_t qcolumns,
    /* Real    */ ae_matrix* q, ae_state *_state)
{
    rmatrixqrunpackq(a,m,n,tau,qcolumns,q, _state);
}


/*************************************************************************
Unpacking of matrix R from the QR decomposition of a matrix A

Input parameters:
    A       -   matrices Q and R in compact form.
                Output of RMatrixQR subroutine.
    M       -   number of rows in given matrix A. M>=0.
    N       -   number of columns in given matrix A. N>=0.

Output parameters:
    R       -   matrix R, array[0..M-1, 0..N-1].

  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixqrunpackr(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* r,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;

    ae_matrix_clear(r);

    if( m<=0||n<=0 )
    {
        return;
    }
    k = ae_minint(m, n, _state);
    ae_matrix_set_length(r, m, n, _state);
    for(i=0; i<=n-1; i++)
    {
        r->ptr.pp_double[0][i] = (double)(0);
    }
    for(i=1; i<=m-1; i++)
    {
        ae_v_move(&r->ptr.pp_double[i][0], 1, &r->ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
    }
    for(i=0; i<=k-1; i++)
    {
        ae_v_move(&r->ptr.pp_double[i][i], 1, &a->ptr.pp_double[i][i], 1, ae_v_len(i,n-1));
    }
}


/*************************************************************************
Partial unpacking of matrix Q from the LQ decomposition of a matrix A

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
  ! of this function. We should note that QP decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=512,   achieving
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
    A       -   matrices L and Q in compact form.
                Output of RMatrixLQ subroutine.
    M       -   number of rows in given matrix A. M>=0.
    N       -   number of columns in given matrix A. N>=0.
    Tau     -   scalar factors which are used to form Q.
                Output of the RMatrixLQ subroutine.
    QRows   -   required number of rows in matrix Q. N>=QRows>=0.

Output parameters:
    Q       -   first QRows rows of matrix Q. Array whose indexes range
                within [0..QRows-1, 0..N-1]. If QRows=0, the array remains
                unchanged.

  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixlqunpackq(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* tau,
     ae_int_t qrows,
     /* Real    */ ae_matrix* q,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_vector t;
    ae_vector taubuf;
    ae_int_t minmn;
    ae_int_t refcnt;
    ae_matrix tmpa;
    ae_matrix tmpt;
    ae_matrix tmpr;
    ae_int_t blockstart;
    ae_int_t blocksize;
    ae_int_t columnscount;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(q);
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&taubuf, 0, DT_REAL, _state);
    ae_matrix_init(&tmpa, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tmpt, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tmpr, 0, 0, DT_REAL, _state);

    ae_assert(qrows<=n, "RMatrixLQUnpackQ: QRows>N!", _state);
    if( (m<=0||n<=0)||qrows<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * init
     */
    minmn = ae_minint(m, n, _state);
    refcnt = ae_minint(minmn, qrows, _state);
    ae_vector_set_length(&work, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(&t, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(&taubuf, minmn, _state);
    ae_matrix_set_length(&tmpa, ablasblocksize(a, _state), n, _state);
    ae_matrix_set_length(&tmpt, ablasblocksize(a, _state), 2*ablasblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpr, qrows, 2*ablasblocksize(a, _state), _state);
    ae_matrix_set_length(q, qrows, n, _state);
    for(i=0; i<=qrows-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i==j )
            {
                q->ptr.pp_double[i][j] = (double)(1);
            }
            else
            {
                q->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    
    /*
     * Blocked code
     */
    blockstart = ablasblocksize(a, _state)*(refcnt/ablasblocksize(a, _state));
    blocksize = refcnt-blockstart;
    while(blockstart>=0)
    {
        columnscount = n-blockstart;
        if( blocksize>0 )
        {
            
            /*
             * Copy submatrix
             */
            rmatrixcopy(blocksize, columnscount, a, blockstart, blockstart, &tmpa, 0, 0, _state);
            ae_v_move(&taubuf.ptr.p_double[0], 1, &tau->ptr.p_double[blockstart], 1, ae_v_len(0,blocksize-1));
            
            /*
             * Update matrix, choose between:
             * a) Level 2 algorithm (when the rest of the matrix is small enough)
             * b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
             *    representation for products of Householder transformations',
             *    by R. Schreiber and C. Van Loan.
             */
            if( qrows>=2*ablasblocksize(a, _state) )
            {
                
                /*
                 * Prepare block reflector
                 */
                ortfac_rmatrixblockreflector(&tmpa, &taubuf, ae_false, columnscount, blocksize, &tmpt, &work, _state);
                
                /*
                 * Multiply the rest of A by Q'.
                 *
                 * Q'  = E + Y*T'*Y'  = E + TmpA'*TmpT'*TmpA
                 */
                rmatrixgemm(qrows, blocksize, columnscount, 1.0, q, 0, blockstart, 0, &tmpa, 0, 0, 1, 0.0, &tmpr, 0, 0, _state);
                rmatrixgemm(qrows, blocksize, blocksize, 1.0, &tmpr, 0, 0, 0, &tmpt, 0, 0, 1, 0.0, &tmpr, 0, blocksize, _state);
                rmatrixgemm(qrows, columnscount, blocksize, 1.0, &tmpr, 0, blocksize, 0, &tmpa, 0, 0, 0, 1.0, q, 0, blockstart, _state);
            }
            else
            {
                
                /*
                 * Level 2 algorithm
                 */
                for(i=blocksize-1; i>=0; i--)
                {
                    ae_v_move(&t.ptr.p_double[1], 1, &tmpa.ptr.pp_double[i][i], 1, ae_v_len(1,columnscount-i));
                    t.ptr.p_double[1] = (double)(1);
                    applyreflectionfromtheright(q, taubuf.ptr.p_double[i], &t, 0, qrows-1, blockstart+i, n-1, &work, _state);
                }
            }
        }
        
        /*
         * Advance
         */
        blockstart = blockstart-ablasblocksize(a, _state);
        blocksize = ablasblocksize(a, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixlqunpackq(/* Real    */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Real    */ ae_vector* tau,
    ae_int_t qrows,
    /* Real    */ ae_matrix* q, ae_state *_state)
{
    rmatrixlqunpackq(a,m,n,tau,qrows,q, _state);
}


/*************************************************************************
Unpacking of matrix L from the LQ decomposition of a matrix A

Input parameters:
    A       -   matrices Q and L in compact form.
                Output of RMatrixLQ subroutine.
    M       -   number of rows in given matrix A. M>=0.
    N       -   number of columns in given matrix A. N>=0.

Output parameters:
    L       -   matrix L, array[0..M-1, 0..N-1].

  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixlqunpackl(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* l,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;

    ae_matrix_clear(l);

    if( m<=0||n<=0 )
    {
        return;
    }
    ae_matrix_set_length(l, m, n, _state);
    for(i=0; i<=n-1; i++)
    {
        l->ptr.pp_double[0][i] = (double)(0);
    }
    for(i=1; i<=m-1; i++)
    {
        ae_v_move(&l->ptr.pp_double[i][0], 1, &l->ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
    }
    for(i=0; i<=m-1; i++)
    {
        k = ae_minint(i, n-1, _state);
        ae_v_move(&l->ptr.pp_double[i][0], 1, &a->ptr.pp_double[i][0], 1, ae_v_len(0,k));
    }
}


/*************************************************************************
Partial unpacking of matrix Q from QR decomposition of a complex matrix A.

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
  ! of this function. We should note that QP decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=512,   achieving
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
    A           -   matrices Q and R in compact form.
                    Output of CMatrixQR subroutine .
    M           -   number of rows in matrix A. M>=0.
    N           -   number of columns in matrix A. N>=0.
    Tau         -   scalar factors which are used to form Q.
                    Output of CMatrixQR subroutine .
    QColumns    -   required number of columns in matrix Q. M>=QColumns>=0.

Output parameters:
    Q           -   first QColumns columns of matrix Q.
                    Array whose index ranges within [0..M-1, 0..QColumns-1].
                    If QColumns=0, array isn't changed.

  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
void cmatrixqrunpackq(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_vector* tau,
     ae_int_t qcolumns,
     /* Complex */ ae_matrix* q,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_vector t;
    ae_vector taubuf;
    ae_int_t minmn;
    ae_int_t refcnt;
    ae_matrix tmpa;
    ae_matrix tmpt;
    ae_matrix tmpr;
    ae_int_t blockstart;
    ae_int_t blocksize;
    ae_int_t rowscount;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(q);
    ae_vector_init(&work, 0, DT_COMPLEX, _state);
    ae_vector_init(&t, 0, DT_COMPLEX, _state);
    ae_vector_init(&taubuf, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpa, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpt, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpr, 0, 0, DT_COMPLEX, _state);

    ae_assert(qcolumns<=m, "UnpackQFromQR: QColumns>M!", _state);
    if( m<=0||n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * init
     */
    minmn = ae_minint(m, n, _state);
    refcnt = ae_minint(minmn, qcolumns, _state);
    ae_vector_set_length(&work, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(&t, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(&taubuf, minmn, _state);
    ae_matrix_set_length(&tmpa, m, ablascomplexblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpt, ablascomplexblocksize(a, _state), ablascomplexblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpr, 2*ablascomplexblocksize(a, _state), qcolumns, _state);
    ae_matrix_set_length(q, m, qcolumns, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=qcolumns-1; j++)
        {
            if( i==j )
            {
                q->ptr.pp_complex[i][j] = ae_complex_from_i(1);
            }
            else
            {
                q->ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
    }
    
    /*
     * Blocked code
     */
    blockstart = ablascomplexblocksize(a, _state)*(refcnt/ablascomplexblocksize(a, _state));
    blocksize = refcnt-blockstart;
    while(blockstart>=0)
    {
        rowscount = m-blockstart;
        if( blocksize>0 )
        {
            
            /*
             * QR decomposition of submatrix.
             * Matrix is copied to temporary storage to solve
             * some TLB issues arising from non-contiguous memory
             * access pattern.
             */
            cmatrixcopy(rowscount, blocksize, a, blockstart, blockstart, &tmpa, 0, 0, _state);
            ae_v_cmove(&taubuf.ptr.p_complex[0], 1, &tau->ptr.p_complex[blockstart], 1, "N", ae_v_len(0,blocksize-1));
            
            /*
             * Update matrix, choose between:
             * a) Level 2 algorithm (when the rest of the matrix is small enough)
             * b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
             *    representation for products of Householder transformations',
             *    by R. Schreiber and C. Van Loan.
             */
            if( qcolumns>=2*ablascomplexblocksize(a, _state) )
            {
                
                /*
                 * Prepare block reflector
                 */
                ortfac_cmatrixblockreflector(&tmpa, &taubuf, ae_true, rowscount, blocksize, &tmpt, &work, _state);
                
                /*
                 * Multiply the rest of A by Q.
                 *
                 * Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                 */
                cmatrixgemm(blocksize, qcolumns, rowscount, ae_complex_from_d(1.0), &tmpa, 0, 0, 2, q, blockstart, 0, 0, ae_complex_from_d(0.0), &tmpr, 0, 0, _state);
                cmatrixgemm(blocksize, qcolumns, blocksize, ae_complex_from_d(1.0), &tmpt, 0, 0, 0, &tmpr, 0, 0, 0, ae_complex_from_d(0.0), &tmpr, blocksize, 0, _state);
                cmatrixgemm(rowscount, qcolumns, blocksize, ae_complex_from_d(1.0), &tmpa, 0, 0, 0, &tmpr, blocksize, 0, 0, ae_complex_from_d(1.0), q, blockstart, 0, _state);
            }
            else
            {
                
                /*
                 * Level 2 algorithm
                 */
                for(i=blocksize-1; i>=0; i--)
                {
                    ae_v_cmove(&t.ptr.p_complex[1], 1, &tmpa.ptr.pp_complex[i][i], tmpa.stride, "N", ae_v_len(1,rowscount-i));
                    t.ptr.p_complex[1] = ae_complex_from_i(1);
                    complexapplyreflectionfromtheleft(q, taubuf.ptr.p_complex[i], &t, blockstart+i, m-1, 0, qcolumns-1, &work, _state);
                }
            }
        }
        
        /*
         * Advance
         */
        blockstart = blockstart-ablascomplexblocksize(a, _state);
        blocksize = ablascomplexblocksize(a, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixqrunpackq(/* Complex */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Complex */ ae_vector* tau,
    ae_int_t qcolumns,
    /* Complex */ ae_matrix* q, ae_state *_state)
{
    cmatrixqrunpackq(a,m,n,tau,qcolumns,q, _state);
}


/*************************************************************************
Unpacking of matrix R from the QR decomposition of a matrix A

Input parameters:
    A       -   matrices Q and R in compact form.
                Output of CMatrixQR subroutine.
    M       -   number of rows in given matrix A. M>=0.
    N       -   number of columns in given matrix A. N>=0.

Output parameters:
    R       -   matrix R, array[0..M-1, 0..N-1].

  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
void cmatrixqrunpackr(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* r,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;

    ae_matrix_clear(r);

    if( m<=0||n<=0 )
    {
        return;
    }
    k = ae_minint(m, n, _state);
    ae_matrix_set_length(r, m, n, _state);
    for(i=0; i<=n-1; i++)
    {
        r->ptr.pp_complex[0][i] = ae_complex_from_i(0);
    }
    for(i=1; i<=m-1; i++)
    {
        ae_v_cmove(&r->ptr.pp_complex[i][0], 1, &r->ptr.pp_complex[0][0], 1, "N", ae_v_len(0,n-1));
    }
    for(i=0; i<=k-1; i++)
    {
        ae_v_cmove(&r->ptr.pp_complex[i][i], 1, &a->ptr.pp_complex[i][i], 1, "N", ae_v_len(i,n-1));
    }
}


/*************************************************************************
Partial unpacking of matrix Q from LQ decomposition of a complex matrix A.

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
  ! of this function. We should note that QP decomposition  is  harder  to
  ! parallelize than, say, matrix-matrix  product  -  this  algorithm  has
  ! many internal synchronization points which can not be avoided. However
  ! parallelism starts to be profitable starting  from  N=512,   achieving
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
    A           -   matrices Q and R in compact form.
                    Output of CMatrixLQ subroutine .
    M           -   number of rows in matrix A. M>=0.
    N           -   number of columns in matrix A. N>=0.
    Tau         -   scalar factors which are used to form Q.
                    Output of CMatrixLQ subroutine .
    QRows       -   required number of rows in matrix Q. N>=QColumns>=0.

Output parameters:
    Q           -   first QRows rows of matrix Q.
                    Array whose index ranges within [0..QRows-1, 0..N-1].
                    If QRows=0, array isn't changed.

  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
void cmatrixlqunpackq(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_vector* tau,
     ae_int_t qrows,
     /* Complex */ ae_matrix* q,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_vector t;
    ae_vector taubuf;
    ae_int_t minmn;
    ae_int_t refcnt;
    ae_matrix tmpa;
    ae_matrix tmpt;
    ae_matrix tmpr;
    ae_int_t blockstart;
    ae_int_t blocksize;
    ae_int_t columnscount;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(q);
    ae_vector_init(&work, 0, DT_COMPLEX, _state);
    ae_vector_init(&t, 0, DT_COMPLEX, _state);
    ae_vector_init(&taubuf, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpa, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpt, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&tmpr, 0, 0, DT_COMPLEX, _state);

    if( m<=0||n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Init
     */
    minmn = ae_minint(m, n, _state);
    refcnt = ae_minint(minmn, qrows, _state);
    ae_vector_set_length(&work, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(&t, ae_maxint(m, n, _state)+1, _state);
    ae_vector_set_length(&taubuf, minmn, _state);
    ae_matrix_set_length(&tmpa, ablascomplexblocksize(a, _state), n, _state);
    ae_matrix_set_length(&tmpt, ablascomplexblocksize(a, _state), ablascomplexblocksize(a, _state), _state);
    ae_matrix_set_length(&tmpr, qrows, 2*ablascomplexblocksize(a, _state), _state);
    ae_matrix_set_length(q, qrows, n, _state);
    for(i=0; i<=qrows-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i==j )
            {
                q->ptr.pp_complex[i][j] = ae_complex_from_i(1);
            }
            else
            {
                q->ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
    }
    
    /*
     * Blocked code
     */
    blockstart = ablascomplexblocksize(a, _state)*(refcnt/ablascomplexblocksize(a, _state));
    blocksize = refcnt-blockstart;
    while(blockstart>=0)
    {
        columnscount = n-blockstart;
        if( blocksize>0 )
        {
            
            /*
             * LQ decomposition of submatrix.
             * Matrix is copied to temporary storage to solve
             * some TLB issues arising from non-contiguous memory
             * access pattern.
             */
            cmatrixcopy(blocksize, columnscount, a, blockstart, blockstart, &tmpa, 0, 0, _state);
            ae_v_cmove(&taubuf.ptr.p_complex[0], 1, &tau->ptr.p_complex[blockstart], 1, "N", ae_v_len(0,blocksize-1));
            
            /*
             * Update matrix, choose between:
             * a) Level 2 algorithm (when the rest of the matrix is small enough)
             * b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
             *    representation for products of Householder transformations',
             *    by R. Schreiber and C. Van Loan.
             */
            if( qrows>=2*ablascomplexblocksize(a, _state) )
            {
                
                /*
                 * Prepare block reflector
                 */
                ortfac_cmatrixblockreflector(&tmpa, &taubuf, ae_false, columnscount, blocksize, &tmpt, &work, _state);
                
                /*
                 * Multiply the rest of A by Q'.
                 *
                 * Q'  = E + Y*T'*Y'  = E + TmpA'*TmpT'*TmpA
                 */
                cmatrixgemm(qrows, blocksize, columnscount, ae_complex_from_d(1.0), q, 0, blockstart, 0, &tmpa, 0, 0, 2, ae_complex_from_d(0.0), &tmpr, 0, 0, _state);
                cmatrixgemm(qrows, blocksize, blocksize, ae_complex_from_d(1.0), &tmpr, 0, 0, 0, &tmpt, 0, 0, 2, ae_complex_from_d(0.0), &tmpr, 0, blocksize, _state);
                cmatrixgemm(qrows, columnscount, blocksize, ae_complex_from_d(1.0), &tmpr, 0, blocksize, 0, &tmpa, 0, 0, 0, ae_complex_from_d(1.0), q, 0, blockstart, _state);
            }
            else
            {
                
                /*
                 * Level 2 algorithm
                 */
                for(i=blocksize-1; i>=0; i--)
                {
                    ae_v_cmove(&t.ptr.p_complex[1], 1, &tmpa.ptr.pp_complex[i][i], 1, "Conj", ae_v_len(1,columnscount-i));
                    t.ptr.p_complex[1] = ae_complex_from_i(1);
                    complexapplyreflectionfromtheright(q, ae_c_conj(taubuf.ptr.p_complex[i], _state), &t, 0, qrows-1, blockstart+i, n-1, &work, _state);
                }
            }
        }
        
        /*
         * Advance
         */
        blockstart = blockstart-ablascomplexblocksize(a, _state);
        blocksize = ablascomplexblocksize(a, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixlqunpackq(/* Complex */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    /* Complex */ ae_vector* tau,
    ae_int_t qrows,
    /* Complex */ ae_matrix* q, ae_state *_state)
{
    cmatrixlqunpackq(a,m,n,tau,qrows,q, _state);
}


/*************************************************************************
Unpacking of matrix L from the LQ decomposition of a matrix A

Input parameters:
    A       -   matrices Q and L in compact form.
                Output of CMatrixLQ subroutine.
    M       -   number of rows in given matrix A. M>=0.
    N       -   number of columns in given matrix A. N>=0.

Output parameters:
    L       -   matrix L, array[0..M-1, 0..N-1].

  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
void cmatrixlqunpackl(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* l,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;

    ae_matrix_clear(l);

    if( m<=0||n<=0 )
    {
        return;
    }
    ae_matrix_set_length(l, m, n, _state);
    for(i=0; i<=n-1; i++)
    {
        l->ptr.pp_complex[0][i] = ae_complex_from_i(0);
    }
    for(i=1; i<=m-1; i++)
    {
        ae_v_cmove(&l->ptr.pp_complex[i][0], 1, &l->ptr.pp_complex[0][0], 1, "N", ae_v_len(0,n-1));
    }
    for(i=0; i<=m-1; i++)
    {
        k = ae_minint(i, n-1, _state);
        ae_v_cmove(&l->ptr.pp_complex[i][0], 1, &a->ptr.pp_complex[i][0], 1, "N", ae_v_len(0,k));
    }
}


/*************************************************************************
Base case for real QR

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994.
     Sergey Bochkanov, ALGLIB project, translation from FORTRAN to
     pseudocode, 2007-2010.
*************************************************************************/
void rmatrixqrbasecase(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* work,
     /* Real    */ ae_vector* t,
     /* Real    */ ae_vector* tau,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    ae_int_t minmn;
    double tmp;


    minmn = ae_minint(m, n, _state);
    
    /*
     * Test the input arguments
     */
    k = minmn;
    for(i=0; i<=k-1; i++)
    {
        
        /*
         * Generate elementary reflector H(i) to annihilate A(i+1:m,i)
         */
        ae_v_move(&t->ptr.p_double[1], 1, &a->ptr.pp_double[i][i], a->stride, ae_v_len(1,m-i));
        generatereflection(t, m-i, &tmp, _state);
        tau->ptr.p_double[i] = tmp;
        ae_v_move(&a->ptr.pp_double[i][i], a->stride, &t->ptr.p_double[1], 1, ae_v_len(i,m-1));
        t->ptr.p_double[1] = (double)(1);
        if( i<n )
        {
            
            /*
             * Apply H(i) to A(i:m-1,i+1:n-1) from the left
             */
            applyreflectionfromtheleft(a, tau->ptr.p_double[i], t, i, m-1, i+1, n-1, work, _state);
        }
    }
}


/*************************************************************************
Base case for real LQ

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994.
     Sergey Bochkanov, ALGLIB project, translation from FORTRAN to
     pseudocode, 2007-2010.
*************************************************************************/
void rmatrixlqbasecase(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* work,
     /* Real    */ ae_vector* t,
     /* Real    */ ae_vector* tau,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    double tmp;


    k = ae_minint(m, n, _state);
    for(i=0; i<=k-1; i++)
    {
        
        /*
         * Generate elementary reflector H(i) to annihilate A(i,i+1:n-1)
         */
        ae_v_move(&t->ptr.p_double[1], 1, &a->ptr.pp_double[i][i], 1, ae_v_len(1,n-i));
        generatereflection(t, n-i, &tmp, _state);
        tau->ptr.p_double[i] = tmp;
        ae_v_move(&a->ptr.pp_double[i][i], 1, &t->ptr.p_double[1], 1, ae_v_len(i,n-1));
        t->ptr.p_double[1] = (double)(1);
        if( i<n )
        {
            
            /*
             * Apply H(i) to A(i+1:m,i:n) from the right
             */
            applyreflectionfromtheright(a, tau->ptr.p_double[i], t, i+1, m-1, i, n-1, work, _state);
        }
    }
}


/*************************************************************************
Reduction of a rectangular matrix to  bidiagonal form

The algorithm reduces the rectangular matrix A to  bidiagonal form by
orthogonal transformations P and Q: A = Q*B*(P^T).

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
  ! Multithreaded acceleration is NOT supported for this function  because
  ! bidiagonal decompostion is inherently sequential in nature.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

Input parameters:
    A       -   source matrix. array[0..M-1, 0..N-1]
    M       -   number of rows in matrix A.
    N       -   number of columns in matrix A.

Output parameters:
    A       -   matrices Q, B, P in compact form (see below).
    TauQ    -   scalar factors which are used to form matrix Q.
    TauP    -   scalar factors which are used to form matrix P.

The main diagonal and one of the  secondary  diagonals  of  matrix  A  are
replaced with bidiagonal  matrix  B.  Other  elements  contain  elementary
reflections which form MxM matrix Q and NxN matrix P, respectively.

If M>=N, B is the upper  bidiagonal  MxN  matrix  and  is  stored  in  the
corresponding  elements  of  matrix  A.  Matrix  Q  is  represented  as  a
product   of   elementary   reflections   Q = H(0)*H(1)*...*H(n-1),  where
H(i) = 1-tau*v*v'. Here tau is a scalar which is stored  in  TauQ[i],  and
vector v has the following  structure:  v(0:i-1)=0, v(i)=1, v(i+1:m-1)  is
stored   in   elements   A(i+1:m-1,i).   Matrix   P  is  as  follows:  P =
G(0)*G(1)*...*G(n-2), where G(i) = 1 - tau*u*u'. Tau is stored in TauP[i],
u(0:i)=0, u(i+1)=1, u(i+2:n-1) is stored in elements A(i,i+2:n-1).

If M<N, B is the  lower  bidiagonal  MxN  matrix  and  is  stored  in  the
corresponding   elements  of  matrix  A.  Q = H(0)*H(1)*...*H(m-2),  where
H(i) = 1 - tau*v*v', tau is stored in TauQ, v(0:i)=0, v(i+1)=1, v(i+2:m-1)
is    stored    in   elements   A(i+2:m-1,i).    P = G(0)*G(1)*...*G(m-1),
G(i) = 1-tau*u*u', tau is stored in  TauP,  u(0:i-1)=0, u(i)=1, u(i+1:n-1)
is stored in A(i,i+1:n-1).

EXAMPLE:

m=6, n=5 (m > n):               m=5, n=6 (m < n):

(  d   e   u1  u1  u1 )         (  d   u1  u1  u1  u1  u1 )
(  v1  d   e   u2  u2 )         (  e   d   u2  u2  u2  u2 )
(  v1  v2  d   e   u3 )         (  v1  e   d   u3  u3  u3 )
(  v1  v2  v3  d   e  )         (  v1  v2  e   d   u4  u4 )
(  v1  v2  v3  v4  d  )         (  v1  v2  v3  e   d   u5 )
(  v1  v2  v3  v4  v5 )

Here vi and ui are vectors which form H(i) and G(i), and d and e -
are the diagonal and off-diagonal elements of matrix B.

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994.
     Sergey Bochkanov, ALGLIB project, translation from FORTRAN to
     pseudocode, 2007-2010.
*************************************************************************/
void rmatrixbd(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* tauq,
     /* Real    */ ae_vector* taup,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_vector t;
    ae_int_t maxmn;
    ae_int_t i;
    double ltau;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(tauq);
    ae_vector_clear(taup);
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);

    
    /*
     * Prepare
     */
    if( n<=0||m<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    maxmn = ae_maxint(m, n, _state);
    ae_vector_set_length(&work, maxmn+1, _state);
    ae_vector_set_length(&t, maxmn+1, _state);
    if( m>=n )
    {
        ae_vector_set_length(tauq, n, _state);
        ae_vector_set_length(taup, n, _state);
        for(i=0; i<=n-1; i++)
        {
            tauq->ptr.p_double[i] = 0.0;
            taup->ptr.p_double[i] = 0.0;
        }
    }
    else
    {
        ae_vector_set_length(tauq, m, _state);
        ae_vector_set_length(taup, m, _state);
        for(i=0; i<=m-1; i++)
        {
            tauq->ptr.p_double[i] = 0.0;
            taup->ptr.p_double[i] = 0.0;
        }
    }
    
    /*
     * Try to use MKL code
     *
     * NOTE: buffers Work[] and T[] are used for temporary storage of diagonals;
     * because they are present in A[], we do not use them.
     */
    if( rmatrixbdmkl(a, m, n, &work, &t, tauq, taup, _state) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * ALGLIB code
     */
    if( m>=n )
    {
        
        /*
         * Reduce to upper bidiagonal form
         */
        for(i=0; i<=n-1; i++)
        {
            
            /*
             * Generate elementary reflector H(i) to annihilate A(i+1:m-1,i)
             */
            ae_v_move(&t.ptr.p_double[1], 1, &a->ptr.pp_double[i][i], a->stride, ae_v_len(1,m-i));
            generatereflection(&t, m-i, &ltau, _state);
            tauq->ptr.p_double[i] = ltau;
            ae_v_move(&a->ptr.pp_double[i][i], a->stride, &t.ptr.p_double[1], 1, ae_v_len(i,m-1));
            t.ptr.p_double[1] = (double)(1);
            
            /*
             * Apply H(i) to A(i:m-1,i+1:n-1) from the left
             */
            applyreflectionfromtheleft(a, ltau, &t, i, m-1, i+1, n-1, &work, _state);
            if( i<n-1 )
            {
                
                /*
                 * Generate elementary reflector G(i) to annihilate
                 * A(i,i+2:n-1)
                 */
                ae_v_move(&t.ptr.p_double[1], 1, &a->ptr.pp_double[i][i+1], 1, ae_v_len(1,n-i-1));
                generatereflection(&t, n-1-i, &ltau, _state);
                taup->ptr.p_double[i] = ltau;
                ae_v_move(&a->ptr.pp_double[i][i+1], 1, &t.ptr.p_double[1], 1, ae_v_len(i+1,n-1));
                t.ptr.p_double[1] = (double)(1);
                
                /*
                 * Apply G(i) to A(i+1:m-1,i+1:n-1) from the right
                 */
                applyreflectionfromtheright(a, ltau, &t, i+1, m-1, i+1, n-1, &work, _state);
            }
            else
            {
                taup->ptr.p_double[i] = (double)(0);
            }
        }
    }
    else
    {
        
        /*
         * Reduce to lower bidiagonal form
         */
        for(i=0; i<=m-1; i++)
        {
            
            /*
             * Generate elementary reflector G(i) to annihilate A(i,i+1:n-1)
             */
            ae_v_move(&t.ptr.p_double[1], 1, &a->ptr.pp_double[i][i], 1, ae_v_len(1,n-i));
            generatereflection(&t, n-i, &ltau, _state);
            taup->ptr.p_double[i] = ltau;
            ae_v_move(&a->ptr.pp_double[i][i], 1, &t.ptr.p_double[1], 1, ae_v_len(i,n-1));
            t.ptr.p_double[1] = (double)(1);
            
            /*
             * Apply G(i) to A(i+1:m-1,i:n-1) from the right
             */
            applyreflectionfromtheright(a, ltau, &t, i+1, m-1, i, n-1, &work, _state);
            if( i<m-1 )
            {
                
                /*
                 * Generate elementary reflector H(i) to annihilate
                 * A(i+2:m-1,i)
                 */
                ae_v_move(&t.ptr.p_double[1], 1, &a->ptr.pp_double[i+1][i], a->stride, ae_v_len(1,m-1-i));
                generatereflection(&t, m-1-i, &ltau, _state);
                tauq->ptr.p_double[i] = ltau;
                ae_v_move(&a->ptr.pp_double[i+1][i], a->stride, &t.ptr.p_double[1], 1, ae_v_len(i+1,m-1));
                t.ptr.p_double[1] = (double)(1);
                
                /*
                 * Apply H(i) to A(i+1:m-1,i+1:n-1) from the left
                 */
                applyreflectionfromtheleft(a, ltau, &t, i+1, m-1, i+1, n-1, &work, _state);
            }
            else
            {
                tauq->ptr.p_double[i] = (double)(0);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unpacking matrix Q which reduces a matrix to bidiagonal form.

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
  ! Multithreaded acceleration is NOT supported for this function.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.
  
Input parameters:
    QP          -   matrices Q and P in compact form.
                    Output of ToBidiagonal subroutine.
    M           -   number of rows in matrix A.
    N           -   number of columns in matrix A.
    TAUQ        -   scalar factors which are used to form Q.
                    Output of ToBidiagonal subroutine.
    QColumns    -   required number of columns in matrix Q.
                    M>=QColumns>=0.

Output parameters:
    Q           -   first QColumns columns of matrix Q.
                    Array[0..M-1, 0..QColumns-1]
                    If QColumns=0, the array is not modified.

  -- ALGLIB --
     2005-2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixbdunpackq(/* Real    */ ae_matrix* qp,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* tauq,
     ae_int_t qcolumns,
     /* Real    */ ae_matrix* q,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(q);

    ae_assert(qcolumns<=m, "RMatrixBDUnpackQ: QColumns>M!", _state);
    ae_assert(qcolumns>=0, "RMatrixBDUnpackQ: QColumns<0!", _state);
    if( (m==0||n==0)||qcolumns==0 )
    {
        return;
    }
    
    /*
     * prepare Q
     */
    ae_matrix_set_length(q, m, qcolumns, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=qcolumns-1; j++)
        {
            if( i==j )
            {
                q->ptr.pp_double[i][j] = (double)(1);
            }
            else
            {
                q->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    
    /*
     * Calculate
     */
    rmatrixbdmultiplybyq(qp, m, n, tauq, q, m, qcolumns, ae_false, ae_false, _state);
}


/*************************************************************************
Multiplication by matrix Q which reduces matrix A to  bidiagonal form.

The algorithm allows pre- or post-multiply by Q or Q'.

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
  ! Multithreaded acceleration is NOT supported for this function.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.
  
Input parameters:
    QP          -   matrices Q and P in compact form.
                    Output of ToBidiagonal subroutine.
    M           -   number of rows in matrix A.
    N           -   number of columns in matrix A.
    TAUQ        -   scalar factors which are used to form Q.
                    Output of ToBidiagonal subroutine.
    Z           -   multiplied matrix.
                    array[0..ZRows-1,0..ZColumns-1]
    ZRows       -   number of rows in matrix Z. If FromTheRight=False,
                    ZRows=M, otherwise ZRows can be arbitrary.
    ZColumns    -   number of columns in matrix Z. If FromTheRight=True,
                    ZColumns=M, otherwise ZColumns can be arbitrary.
    FromTheRight -  pre- or post-multiply.
    DoTranspose -   multiply by Q or Q'.

Output parameters:
    Z           -   product of Z and Q.
                    Array[0..ZRows-1,0..ZColumns-1]
                    If ZRows=0 or ZColumns=0, the array is not modified.

  -- ALGLIB --
     2005-2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixbdmultiplybyq(/* Real    */ ae_matrix* qp,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* tauq,
     /* Real    */ ae_matrix* z,
     ae_int_t zrows,
     ae_int_t zcolumns,
     ae_bool fromtheright,
     ae_bool dotranspose,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t istep;
    ae_vector v;
    ae_vector work;
    ae_vector dummy;
    ae_int_t mx;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&v, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_vector_init(&dummy, 0, DT_REAL, _state);

    if( ((m<=0||n<=0)||zrows<=0)||zcolumns<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_assert((fromtheright&&zcolumns==m)||(!fromtheright&&zrows==m), "RMatrixBDMultiplyByQ: incorrect Z size!", _state);
    
    /*
     * Try to use MKL code
     */
    if( rmatrixbdmultiplybymkl(qp, m, n, tauq, &dummy, z, zrows, zcolumns, ae_true, fromtheright, dotranspose, _state) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * init
     */
    mx = ae_maxint(m, n, _state);
    mx = ae_maxint(mx, zrows, _state);
    mx = ae_maxint(mx, zcolumns, _state);
    ae_vector_set_length(&v, mx+1, _state);
    ae_vector_set_length(&work, mx+1, _state);
    if( m>=n )
    {
        
        /*
         * setup
         */
        if( fromtheright )
        {
            i1 = 0;
            i2 = n-1;
            istep = 1;
        }
        else
        {
            i1 = n-1;
            i2 = 0;
            istep = -1;
        }
        if( dotranspose )
        {
            i = i1;
            i1 = i2;
            i2 = i;
            istep = -istep;
        }
        
        /*
         * Process
         */
        i = i1;
        do
        {
            ae_v_move(&v.ptr.p_double[1], 1, &qp->ptr.pp_double[i][i], qp->stride, ae_v_len(1,m-i));
            v.ptr.p_double[1] = (double)(1);
            if( fromtheright )
            {
                applyreflectionfromtheright(z, tauq->ptr.p_double[i], &v, 0, zrows-1, i, m-1, &work, _state);
            }
            else
            {
                applyreflectionfromtheleft(z, tauq->ptr.p_double[i], &v, i, m-1, 0, zcolumns-1, &work, _state);
            }
            i = i+istep;
        }
        while(i!=i2+istep);
    }
    else
    {
        
        /*
         * setup
         */
        if( fromtheright )
        {
            i1 = 0;
            i2 = m-2;
            istep = 1;
        }
        else
        {
            i1 = m-2;
            i2 = 0;
            istep = -1;
        }
        if( dotranspose )
        {
            i = i1;
            i1 = i2;
            i2 = i;
            istep = -istep;
        }
        
        /*
         * Process
         */
        if( m-1>0 )
        {
            i = i1;
            do
            {
                ae_v_move(&v.ptr.p_double[1], 1, &qp->ptr.pp_double[i+1][i], qp->stride, ae_v_len(1,m-i-1));
                v.ptr.p_double[1] = (double)(1);
                if( fromtheright )
                {
                    applyreflectionfromtheright(z, tauq->ptr.p_double[i], &v, 0, zrows-1, i+1, m-1, &work, _state);
                }
                else
                {
                    applyreflectionfromtheleft(z, tauq->ptr.p_double[i], &v, i+1, m-1, 0, zcolumns-1, &work, _state);
                }
                i = i+istep;
            }
            while(i!=i2+istep);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unpacking matrix P which reduces matrix A to bidiagonal form.
The subroutine returns transposed matrix P.

Input parameters:
    QP      -   matrices Q and P in compact form.
                Output of ToBidiagonal subroutine.
    M       -   number of rows in matrix A.
    N       -   number of columns in matrix A.
    TAUP    -   scalar factors which are used to form P.
                Output of ToBidiagonal subroutine.
    PTRows  -   required number of rows of matrix P^T. N >= PTRows >= 0.

Output parameters:
    PT      -   first PTRows columns of matrix P^T
                Array[0..PTRows-1, 0..N-1]
                If PTRows=0, the array is not modified.

  -- ALGLIB --
     2005-2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixbdunpackpt(/* Real    */ ae_matrix* qp,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* taup,
     ae_int_t ptrows,
     /* Real    */ ae_matrix* pt,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(pt);

    ae_assert(ptrows<=n, "RMatrixBDUnpackPT: PTRows>N!", _state);
    ae_assert(ptrows>=0, "RMatrixBDUnpackPT: PTRows<0!", _state);
    if( (m==0||n==0)||ptrows==0 )
    {
        return;
    }
    
    /*
     * prepare PT
     */
    ae_matrix_set_length(pt, ptrows, n, _state);
    for(i=0; i<=ptrows-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i==j )
            {
                pt->ptr.pp_double[i][j] = (double)(1);
            }
            else
            {
                pt->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    
    /*
     * Calculate
     */
    rmatrixbdmultiplybyp(qp, m, n, taup, pt, ptrows, n, ae_true, ae_true, _state);
}


/*************************************************************************
Multiplication by matrix P which reduces matrix A to  bidiagonal form.

The algorithm allows pre- or post-multiply by P or P'.

Input parameters:
    QP          -   matrices Q and P in compact form.
                    Output of RMatrixBD subroutine.
    M           -   number of rows in matrix A.
    N           -   number of columns in matrix A.
    TAUP        -   scalar factors which are used to form P.
                    Output of RMatrixBD subroutine.
    Z           -   multiplied matrix.
                    Array whose indexes range within [0..ZRows-1,0..ZColumns-1].
    ZRows       -   number of rows in matrix Z. If FromTheRight=False,
                    ZRows=N, otherwise ZRows can be arbitrary.
    ZColumns    -   number of columns in matrix Z. If FromTheRight=True,
                    ZColumns=N, otherwise ZColumns can be arbitrary.
    FromTheRight -  pre- or post-multiply.
    DoTranspose -   multiply by P or P'.

Output parameters:
    Z - product of Z and P.
                Array whose indexes range within [0..ZRows-1,0..ZColumns-1].
                If ZRows=0 or ZColumns=0, the array is not modified.

  -- ALGLIB --
     2005-2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixbdmultiplybyp(/* Real    */ ae_matrix* qp,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* taup,
     /* Real    */ ae_matrix* z,
     ae_int_t zrows,
     ae_int_t zcolumns,
     ae_bool fromtheright,
     ae_bool dotranspose,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector v;
    ae_vector work;
    ae_vector dummy;
    ae_int_t mx;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t istep;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&v, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_vector_init(&dummy, 0, DT_REAL, _state);

    if( ((m<=0||n<=0)||zrows<=0)||zcolumns<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_assert((fromtheright&&zcolumns==n)||(!fromtheright&&zrows==n), "RMatrixBDMultiplyByP: incorrect Z size!", _state);
    
    /*
     * init
     */
    mx = ae_maxint(m, n, _state);
    mx = ae_maxint(mx, zrows, _state);
    mx = ae_maxint(mx, zcolumns, _state);
    ae_vector_set_length(&v, mx+1, _state);
    ae_vector_set_length(&work, mx+1, _state);
    if( m>=n )
    {
        
        /*
         * setup
         */
        if( fromtheright )
        {
            i1 = n-2;
            i2 = 0;
            istep = -1;
        }
        else
        {
            i1 = 0;
            i2 = n-2;
            istep = 1;
        }
        if( !dotranspose )
        {
            i = i1;
            i1 = i2;
            i2 = i;
            istep = -istep;
        }
        
        /*
         * Process
         */
        if( n-1>0 )
        {
            i = i1;
            do
            {
                ae_v_move(&v.ptr.p_double[1], 1, &qp->ptr.pp_double[i][i+1], 1, ae_v_len(1,n-1-i));
                v.ptr.p_double[1] = (double)(1);
                if( fromtheright )
                {
                    applyreflectionfromtheright(z, taup->ptr.p_double[i], &v, 0, zrows-1, i+1, n-1, &work, _state);
                }
                else
                {
                    applyreflectionfromtheleft(z, taup->ptr.p_double[i], &v, i+1, n-1, 0, zcolumns-1, &work, _state);
                }
                i = i+istep;
            }
            while(i!=i2+istep);
        }
    }
    else
    {
        
        /*
         * setup
         */
        if( fromtheright )
        {
            i1 = m-1;
            i2 = 0;
            istep = -1;
        }
        else
        {
            i1 = 0;
            i2 = m-1;
            istep = 1;
        }
        if( !dotranspose )
        {
            i = i1;
            i1 = i2;
            i2 = i;
            istep = -istep;
        }
        
        /*
         * Process
         */
        i = i1;
        do
        {
            ae_v_move(&v.ptr.p_double[1], 1, &qp->ptr.pp_double[i][i], 1, ae_v_len(1,n-i));
            v.ptr.p_double[1] = (double)(1);
            if( fromtheright )
            {
                applyreflectionfromtheright(z, taup->ptr.p_double[i], &v, 0, zrows-1, i, n-1, &work, _state);
            }
            else
            {
                applyreflectionfromtheleft(z, taup->ptr.p_double[i], &v, i, n-1, 0, zcolumns-1, &work, _state);
            }
            i = i+istep;
        }
        while(i!=i2+istep);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unpacking of the main and secondary diagonals of bidiagonal decomposition
of matrix A.

Input parameters:
    B   -   output of RMatrixBD subroutine.
    M   -   number of rows in matrix B.
    N   -   number of columns in matrix B.

Output parameters:
    IsUpper -   True, if the matrix is upper bidiagonal.
                otherwise IsUpper is False.
    D       -   the main diagonal.
                Array whose index ranges within [0..Min(M,N)-1].
    E       -   the secondary diagonal (upper or lower, depending on
                the value of IsUpper).
                Array index ranges within [0..Min(M,N)-1], the last
                element is not used.

  -- ALGLIB --
     2005-2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixbdunpackdiagonals(/* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t n,
     ae_bool* isupper,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_state *_state)
{
    ae_int_t i;

    *isupper = ae_false;
    ae_vector_clear(d);
    ae_vector_clear(e);

    *isupper = m>=n;
    if( m<=0||n<=0 )
    {
        return;
    }
    if( *isupper )
    {
        ae_vector_set_length(d, n, _state);
        ae_vector_set_length(e, n, _state);
        for(i=0; i<=n-2; i++)
        {
            d->ptr.p_double[i] = b->ptr.pp_double[i][i];
            e->ptr.p_double[i] = b->ptr.pp_double[i][i+1];
        }
        d->ptr.p_double[n-1] = b->ptr.pp_double[n-1][n-1];
    }
    else
    {
        ae_vector_set_length(d, m, _state);
        ae_vector_set_length(e, m, _state);
        for(i=0; i<=m-2; i++)
        {
            d->ptr.p_double[i] = b->ptr.pp_double[i][i];
            e->ptr.p_double[i] = b->ptr.pp_double[i+1][i];
        }
        d->ptr.p_double[m-1] = b->ptr.pp_double[m-1][m-1];
    }
}


/*************************************************************************
Reduction of a square matrix to  upper Hessenberg form: Q'*A*Q = H,
where Q is an orthogonal matrix, H - Hessenberg matrix.

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
  ! Multithreaded acceleration is NOT supported for this function.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

Input parameters:
    A       -   matrix A with elements [0..N-1, 0..N-1]
    N       -   size of matrix A.

Output parameters:
    A       -   matrices Q and P in  compact form (see below).
    Tau     -   array of scalar factors which are used to form matrix Q.
                Array whose index ranges within [0..N-2]

Matrix H is located on the main diagonal, on the lower secondary  diagonal
and above the main diagonal of matrix A. The elements which are used to
form matrix Q are situated in array Tau and below the lower secondary
diagonal of matrix A as follows:

Matrix Q is represented as a product of elementary reflections

Q = H(0)*H(2)*...*H(n-2),

where each H(i) is given by

H(i) = 1 - tau * v * (v^T)

where tau is a scalar stored in Tau[I]; v - is a real vector,
so that v(0:i) = 0, v(i+1) = 1, v(i+2:n-1) stored in A(i+2:n-1,i).

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     October 31, 1992
*************************************************************************/
void rmatrixhessenberg(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_vector* tau,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    double v;
    ae_vector t;
    ae_vector work;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(tau);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);

    ae_assert(n>=0, "RMatrixHessenberg: incorrect N!", _state);
    
    /*
     * Quick return if possible
     */
    if( n<=1 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Allocate place
     */
    ae_vector_set_length(tau, n-2+1, _state);
    ae_vector_set_length(&t, n+1, _state);
    ae_vector_set_length(&work, n-1+1, _state);
    
    /*
     * MKL version
     */
    if( rmatrixhessenbergmkl(a, n, tau, _state) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * ALGLIB version
     */
    for(i=0; i<=n-2; i++)
    {
        
        /*
         * Compute elementary reflector H(i) to annihilate A(i+2:ihi,i)
         */
        ae_v_move(&t.ptr.p_double[1], 1, &a->ptr.pp_double[i+1][i], a->stride, ae_v_len(1,n-i-1));
        generatereflection(&t, n-i-1, &v, _state);
        ae_v_move(&a->ptr.pp_double[i+1][i], a->stride, &t.ptr.p_double[1], 1, ae_v_len(i+1,n-1));
        tau->ptr.p_double[i] = v;
        t.ptr.p_double[1] = (double)(1);
        
        /*
         * Apply H(i) to A(1:ihi,i+1:ihi) from the right
         */
        applyreflectionfromtheright(a, v, &t, 0, n-1, i+1, n-1, &work, _state);
        
        /*
         * Apply H(i) to A(i+1:ihi,i+1:n) from the left
         */
        applyreflectionfromtheleft(a, v, &t, i+1, n-1, i+1, n-1, &work, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unpacking matrix Q which reduces matrix A to upper Hessenberg form

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
  ! Multithreaded acceleration is NOT supported for this function.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

Input parameters:
    A   -   output of RMatrixHessenberg subroutine.
    N   -   size of matrix A.
    Tau -   scalar factors which are used to form Q.
            Output of RMatrixHessenberg subroutine.

Output parameters:
    Q   -   matrix Q.
            Array whose indexes range within [0..N-1, 0..N-1].

  -- ALGLIB --
     2005-2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixhessenbergunpackq(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_vector* tau,
     /* Real    */ ae_matrix* q,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector v;
    ae_vector work;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(q);
    ae_vector_init(&v, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);

    if( n==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * init
     */
    ae_matrix_set_length(q, n-1+1, n-1+1, _state);
    ae_vector_set_length(&v, n-1+1, _state);
    ae_vector_set_length(&work, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i==j )
            {
                q->ptr.pp_double[i][j] = (double)(1);
            }
            else
            {
                q->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    
    /*
     * MKL version
     */
    if( rmatrixhessenbergunpackqmkl(a, n, tau, q, _state) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * ALGLIB version: unpack Q
     */
    for(i=0; i<=n-2; i++)
    {
        
        /*
         * Apply H(i)
         */
        ae_v_move(&v.ptr.p_double[1], 1, &a->ptr.pp_double[i+1][i], a->stride, ae_v_len(1,n-i-1));
        v.ptr.p_double[1] = (double)(1);
        applyreflectionfromtheright(q, tau->ptr.p_double[i], &v, 0, n-1, i+1, n-1, &work, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unpacking matrix H (the result of matrix A reduction to upper Hessenberg form)

Input parameters:
    A   -   output of RMatrixHessenberg subroutine.
    N   -   size of matrix A.

Output parameters:
    H   -   matrix H. Array whose indexes range within [0..N-1, 0..N-1].

  -- ALGLIB --
     2005-2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixhessenbergunpackh(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_matrix* h,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector v;
    ae_vector work;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(h);
    ae_vector_init(&v, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);

    if( n==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(h, n-1+1, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=i-2; j++)
        {
            h->ptr.pp_double[i][j] = (double)(0);
        }
        j = ae_maxint(0, i-1, _state);
        ae_v_move(&h->ptr.pp_double[i][j], 1, &a->ptr.pp_double[i][j], 1, ae_v_len(j,n-1));
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reduction of a symmetric matrix which is given by its higher or lower
triangular part to a tridiagonal matrix using orthogonal similarity
transformation: Q'*A*Q=T.

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
  ! Multithreaded acceleration is NOT supported for this function.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

Input parameters:
    A       -   matrix to be transformed
                array with elements [0..N-1, 0..N-1].
    N       -   size of matrix A.
    IsUpper -   storage format. If IsUpper = True, then matrix A is given
                by its upper triangle, and the lower triangle is not used
                and not modified by the algorithm, and vice versa
                if IsUpper = False.

Output parameters:
    A       -   matrices T and Q in  compact form (see lower)
    Tau     -   array of factors which are forming matrices H(i)
                array with elements [0..N-2].
    D       -   main diagonal of symmetric matrix T.
                array with elements [0..N-1].
    E       -   secondary diagonal of symmetric matrix T.
                array with elements [0..N-2].


  If IsUpper=True, the matrix Q is represented as a product of elementary
  reflectors

     Q = H(n-2) . . . H(2) H(0).

  Each H(i) has the form

     H(i) = I - tau * v * v'

  where tau is a real scalar, and v is a real vector with
  v(i+1:n-1) = 0, v(i) = 1, v(0:i-1) is stored on exit in
  A(0:i-1,i+1), and tau in TAU(i).

  If IsUpper=False, the matrix Q is represented as a product of elementary
  reflectors

     Q = H(0) H(2) . . . H(n-2).

  Each H(i) has the form

     H(i) = I - tau * v * v'

  where tau is a real scalar, and v is a real vector with
  v(0:i) = 0, v(i+1) = 1, v(i+2:n-1) is stored on exit in A(i+2:n-1,i),
  and tau in TAU(i).

  The contents of A on exit are illustrated by the following examples
  with n = 5:

  if UPLO = 'U':                       if UPLO = 'L':

    (  d   e   v1  v2  v3 )              (  d                  )
    (      d   e   v2  v3 )              (  e   d              )
    (          d   e   v3 )              (  v0  e   d          )
    (              d   e  )              (  v0  v1  e   d      )
    (                  d  )              (  v0  v1  v2  e   d  )

  where d and e denote diagonal and off-diagonal elements of T, and vi
  denotes an element of the vector defining H(i).

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     October 31, 1992
*************************************************************************/
void smatrixtd(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* tau,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    double alpha;
    double taui;
    double v;
    ae_vector t;
    ae_vector t2;
    ae_vector t3;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(tau);
    ae_vector_clear(d);
    ae_vector_clear(e);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&t2, 0, DT_REAL, _state);
    ae_vector_init(&t3, 0, DT_REAL, _state);

    if( n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(&t, n+1, _state);
    ae_vector_set_length(&t2, n+1, _state);
    ae_vector_set_length(&t3, n+1, _state);
    if( n>1 )
    {
        ae_vector_set_length(tau, n-2+1, _state);
    }
    ae_vector_set_length(d, n-1+1, _state);
    if( n>1 )
    {
        ae_vector_set_length(e, n-2+1, _state);
    }
    
    /*
     * Try to use MKL
     */
    if( smatrixtdmkl(a, n, isupper, tau, d, e, _state) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * ALGLIB version
     */
    if( isupper )
    {
        
        /*
         * Reduce the upper triangle of A
         */
        for(i=n-2; i>=0; i--)
        {
            
            /*
             * Generate elementary reflector H() = E - tau * v * v'
             */
            if( i>=1 )
            {
                ae_v_move(&t.ptr.p_double[2], 1, &a->ptr.pp_double[0][i+1], a->stride, ae_v_len(2,i+1));
            }
            t.ptr.p_double[1] = a->ptr.pp_double[i][i+1];
            generatereflection(&t, i+1, &taui, _state);
            if( i>=1 )
            {
                ae_v_move(&a->ptr.pp_double[0][i+1], a->stride, &t.ptr.p_double[2], 1, ae_v_len(0,i-1));
            }
            a->ptr.pp_double[i][i+1] = t.ptr.p_double[1];
            e->ptr.p_double[i] = a->ptr.pp_double[i][i+1];
            if( ae_fp_neq(taui,(double)(0)) )
            {
                
                /*
                 * Apply H from both sides to A
                 */
                a->ptr.pp_double[i][i+1] = (double)(1);
                
                /*
                 * Compute  x := tau * A * v  storing x in TAU
                 */
                ae_v_move(&t.ptr.p_double[1], 1, &a->ptr.pp_double[0][i+1], a->stride, ae_v_len(1,i+1));
                symmetricmatrixvectormultiply(a, isupper, 0, i, &t, taui, &t3, _state);
                ae_v_move(&tau->ptr.p_double[0], 1, &t3.ptr.p_double[1], 1, ae_v_len(0,i));
                
                /*
                 * Compute  w := x - 1/2 * tau * (x'*v) * v
                 */
                v = ae_v_dotproduct(&tau->ptr.p_double[0], 1, &a->ptr.pp_double[0][i+1], a->stride, ae_v_len(0,i));
                alpha = -0.5*taui*v;
                ae_v_addd(&tau->ptr.p_double[0], 1, &a->ptr.pp_double[0][i+1], a->stride, ae_v_len(0,i), alpha);
                
                /*
                 * Apply the transformation as a rank-2 update:
                 *    A := A - v * w' - w * v'
                 */
                ae_v_move(&t.ptr.p_double[1], 1, &a->ptr.pp_double[0][i+1], a->stride, ae_v_len(1,i+1));
                ae_v_move(&t3.ptr.p_double[1], 1, &tau->ptr.p_double[0], 1, ae_v_len(1,i+1));
                symmetricrank2update(a, isupper, 0, i, &t, &t3, &t2, (double)(-1), _state);
                a->ptr.pp_double[i][i+1] = e->ptr.p_double[i];
            }
            d->ptr.p_double[i+1] = a->ptr.pp_double[i+1][i+1];
            tau->ptr.p_double[i] = taui;
        }
        d->ptr.p_double[0] = a->ptr.pp_double[0][0];
    }
    else
    {
        
        /*
         * Reduce the lower triangle of A
         */
        for(i=0; i<=n-2; i++)
        {
            
            /*
             * Generate elementary reflector H = E - tau * v * v'
             */
            ae_v_move(&t.ptr.p_double[1], 1, &a->ptr.pp_double[i+1][i], a->stride, ae_v_len(1,n-i-1));
            generatereflection(&t, n-i-1, &taui, _state);
            ae_v_move(&a->ptr.pp_double[i+1][i], a->stride, &t.ptr.p_double[1], 1, ae_v_len(i+1,n-1));
            e->ptr.p_double[i] = a->ptr.pp_double[i+1][i];
            if( ae_fp_neq(taui,(double)(0)) )
            {
                
                /*
                 * Apply H from both sides to A
                 */
                a->ptr.pp_double[i+1][i] = (double)(1);
                
                /*
                 * Compute  x := tau * A * v  storing y in TAU
                 */
                ae_v_move(&t.ptr.p_double[1], 1, &a->ptr.pp_double[i+1][i], a->stride, ae_v_len(1,n-i-1));
                symmetricmatrixvectormultiply(a, isupper, i+1, n-1, &t, taui, &t2, _state);
                ae_v_move(&tau->ptr.p_double[i], 1, &t2.ptr.p_double[1], 1, ae_v_len(i,n-2));
                
                /*
                 * Compute  w := x - 1/2 * tau * (x'*v) * v
                 */
                v = ae_v_dotproduct(&tau->ptr.p_double[i], 1, &a->ptr.pp_double[i+1][i], a->stride, ae_v_len(i,n-2));
                alpha = -0.5*taui*v;
                ae_v_addd(&tau->ptr.p_double[i], 1, &a->ptr.pp_double[i+1][i], a->stride, ae_v_len(i,n-2), alpha);
                
                /*
                 * Apply the transformation as a rank-2 update:
                 *     A := A - v * w' - w * v'
                 *
                 */
                ae_v_move(&t.ptr.p_double[1], 1, &a->ptr.pp_double[i+1][i], a->stride, ae_v_len(1,n-i-1));
                ae_v_move(&t2.ptr.p_double[1], 1, &tau->ptr.p_double[i], 1, ae_v_len(1,n-i-1));
                symmetricrank2update(a, isupper, i+1, n-1, &t, &t2, &t3, (double)(-1), _state);
                a->ptr.pp_double[i+1][i] = e->ptr.p_double[i];
            }
            d->ptr.p_double[i] = a->ptr.pp_double[i][i];
            tau->ptr.p_double[i] = taui;
        }
        d->ptr.p_double[n-1] = a->ptr.pp_double[n-1][n-1];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unpacking matrix Q which reduces symmetric matrix to a tridiagonal
form.


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
  ! Multithreaded acceleration is NOT supported for this function.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

Input parameters:
    A       -   the result of a SMatrixTD subroutine
    N       -   size of matrix A.
    IsUpper -   storage format (a parameter of SMatrixTD subroutine)
    Tau     -   the result of a SMatrixTD subroutine

Output parameters:
    Q       -   transformation matrix.
                array with elements [0..N-1, 0..N-1].

  -- ALGLIB --
     Copyright 2005-2010 by Bochkanov Sergey
*************************************************************************/
void smatrixtdunpackq(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* tau,
     /* Real    */ ae_matrix* q,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector v;
    ae_vector work;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(q);
    ae_vector_init(&v, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);

    if( n==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * init
     */
    ae_matrix_set_length(q, n-1+1, n-1+1, _state);
    ae_vector_set_length(&v, n+1, _state);
    ae_vector_set_length(&work, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i==j )
            {
                q->ptr.pp_double[i][j] = (double)(1);
            }
            else
            {
                q->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    
    /*
     * MKL version
     */
    if( smatrixtdunpackqmkl(a, n, isupper, tau, q, _state) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * ALGLIB version: unpack Q
     */
    if( isupper )
    {
        for(i=0; i<=n-2; i++)
        {
            
            /*
             * Apply H(i)
             */
            ae_v_move(&v.ptr.p_double[1], 1, &a->ptr.pp_double[0][i+1], a->stride, ae_v_len(1,i+1));
            v.ptr.p_double[i+1] = (double)(1);
            applyreflectionfromtheleft(q, tau->ptr.p_double[i], &v, 0, i, 0, n-1, &work, _state);
        }
    }
    else
    {
        for(i=n-2; i>=0; i--)
        {
            
            /*
             * Apply H(i)
             */
            ae_v_move(&v.ptr.p_double[1], 1, &a->ptr.pp_double[i+1][i], a->stride, ae_v_len(1,n-i-1));
            v.ptr.p_double[1] = (double)(1);
            applyreflectionfromtheleft(q, tau->ptr.p_double[i], &v, i+1, n-1, 0, n-1, &work, _state);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reduction of a Hermitian matrix which is given  by  its  higher  or  lower
triangular part to a real  tridiagonal  matrix  using  unitary  similarity
transformation: Q'*A*Q = T.


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
  ! Multithreaded acceleration is NOT supported for this function.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

Input parameters:
    A       -   matrix to be transformed
                array with elements [0..N-1, 0..N-1].
    N       -   size of matrix A.
    IsUpper -   storage format. If IsUpper = True, then matrix A is  given
                by its upper triangle, and the lower triangle is not  used
                and not modified by the algorithm, and vice versa
                if IsUpper = False.

Output parameters:
    A       -   matrices T and Q in  compact form (see lower)
    Tau     -   array of factors which are forming matrices H(i)
                array with elements [0..N-2].
    D       -   main diagonal of real symmetric matrix T.
                array with elements [0..N-1].
    E       -   secondary diagonal of real symmetric matrix T.
                array with elements [0..N-2].


  If IsUpper=True, the matrix Q is represented as a product of elementary
  reflectors

     Q = H(n-2) . . . H(2) H(0).

  Each H(i) has the form

     H(i) = I - tau * v * v'

  where tau is a complex scalar, and v is a complex vector with
  v(i+1:n-1) = 0, v(i) = 1, v(0:i-1) is stored on exit in
  A(0:i-1,i+1), and tau in TAU(i).

  If IsUpper=False, the matrix Q is represented as a product of elementary
  reflectors

     Q = H(0) H(2) . . . H(n-2).

  Each H(i) has the form

     H(i) = I - tau * v * v'

  where tau is a complex scalar, and v is a complex vector with
  v(0:i) = 0, v(i+1) = 1, v(i+2:n-1) is stored on exit in A(i+2:n-1,i),
  and tau in TAU(i).

  The contents of A on exit are illustrated by the following examples
  with n = 5:

  if UPLO = 'U':                       if UPLO = 'L':

    (  d   e   v1  v2  v3 )              (  d                  )
    (      d   e   v2  v3 )              (  e   d              )
    (          d   e   v3 )              (  v0  e   d          )
    (              d   e  )              (  v0  v1  e   d      )
    (                  d  )              (  v0  v1  v2  e   d  )

where d and e denote diagonal and off-diagonal elements of T, and vi
denotes an element of the vector defining H(i).

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     October 31, 1992
*************************************************************************/
void hmatrixtd(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* tau,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_complex alpha;
    ae_complex taui;
    ae_complex v;
    ae_vector t;
    ae_vector t2;
    ae_vector t3;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(tau);
    ae_vector_clear(d);
    ae_vector_clear(e);
    ae_vector_init(&t, 0, DT_COMPLEX, _state);
    ae_vector_init(&t2, 0, DT_COMPLEX, _state);
    ae_vector_init(&t3, 0, DT_COMPLEX, _state);

    
    /*
     * Init and test
     */
    if( n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        ae_assert(ae_fp_eq(a->ptr.pp_complex[i][i].y,(double)(0)), "Assertion failed", _state);
    }
    if( n>1 )
    {
        ae_vector_set_length(tau, n-2+1, _state);
        ae_vector_set_length(e, n-2+1, _state);
    }
    ae_vector_set_length(d, n-1+1, _state);
    ae_vector_set_length(&t, n-1+1, _state);
    ae_vector_set_length(&t2, n-1+1, _state);
    ae_vector_set_length(&t3, n-1+1, _state);
    
    /*
     * MKL version
     */
    if( hmatrixtdmkl(a, n, isupper, tau, d, e, _state) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * ALGLIB version
     */
    if( isupper )
    {
        
        /*
         * Reduce the upper triangle of A
         */
        a->ptr.pp_complex[n-1][n-1] = ae_complex_from_d(a->ptr.pp_complex[n-1][n-1].x);
        for(i=n-2; i>=0; i--)
        {
            
            /*
             * Generate elementary reflector H = I+1 - tau * v * v'
             */
            alpha = a->ptr.pp_complex[i][i+1];
            t.ptr.p_complex[1] = alpha;
            if( i>=1 )
            {
                ae_v_cmove(&t.ptr.p_complex[2], 1, &a->ptr.pp_complex[0][i+1], a->stride, "N", ae_v_len(2,i+1));
            }
            complexgeneratereflection(&t, i+1, &taui, _state);
            if( i>=1 )
            {
                ae_v_cmove(&a->ptr.pp_complex[0][i+1], a->stride, &t.ptr.p_complex[2], 1, "N", ae_v_len(0,i-1));
            }
            alpha = t.ptr.p_complex[1];
            e->ptr.p_double[i] = alpha.x;
            if( ae_c_neq_d(taui,(double)(0)) )
            {
                
                /*
                 * Apply H(I+1) from both sides to A
                 */
                a->ptr.pp_complex[i][i+1] = ae_complex_from_i(1);
                
                /*
                 * Compute  x := tau * A * v  storing x in TAU
                 */
                ae_v_cmove(&t.ptr.p_complex[1], 1, &a->ptr.pp_complex[0][i+1], a->stride, "N", ae_v_len(1,i+1));
                hermitianmatrixvectormultiply(a, isupper, 0, i, &t, taui, &t2, _state);
                ae_v_cmove(&tau->ptr.p_complex[0], 1, &t2.ptr.p_complex[1], 1, "N", ae_v_len(0,i));
                
                /*
                 * Compute  w := x - 1/2 * tau * (x'*v) * v
                 */
                v = ae_v_cdotproduct(&tau->ptr.p_complex[0], 1, "Conj", &a->ptr.pp_complex[0][i+1], a->stride, "N", ae_v_len(0,i));
                alpha = ae_c_neg(ae_c_mul(ae_c_mul_d(taui,0.5),v));
                ae_v_caddc(&tau->ptr.p_complex[0], 1, &a->ptr.pp_complex[0][i+1], a->stride, "N", ae_v_len(0,i), alpha);
                
                /*
                 * Apply the transformation as a rank-2 update:
                 *    A := A - v * w' - w * v'
                 */
                ae_v_cmove(&t.ptr.p_complex[1], 1, &a->ptr.pp_complex[0][i+1], a->stride, "N", ae_v_len(1,i+1));
                ae_v_cmove(&t3.ptr.p_complex[1], 1, &tau->ptr.p_complex[0], 1, "N", ae_v_len(1,i+1));
                hermitianrank2update(a, isupper, 0, i, &t, &t3, &t2, ae_complex_from_i(-1), _state);
            }
            else
            {
                a->ptr.pp_complex[i][i] = ae_complex_from_d(a->ptr.pp_complex[i][i].x);
            }
            a->ptr.pp_complex[i][i+1] = ae_complex_from_d(e->ptr.p_double[i]);
            d->ptr.p_double[i+1] = a->ptr.pp_complex[i+1][i+1].x;
            tau->ptr.p_complex[i] = taui;
        }
        d->ptr.p_double[0] = a->ptr.pp_complex[0][0].x;
    }
    else
    {
        
        /*
         * Reduce the lower triangle of A
         */
        a->ptr.pp_complex[0][0] = ae_complex_from_d(a->ptr.pp_complex[0][0].x);
        for(i=0; i<=n-2; i++)
        {
            
            /*
             * Generate elementary reflector H = I - tau * v * v'
             */
            ae_v_cmove(&t.ptr.p_complex[1], 1, &a->ptr.pp_complex[i+1][i], a->stride, "N", ae_v_len(1,n-i-1));
            complexgeneratereflection(&t, n-i-1, &taui, _state);
            ae_v_cmove(&a->ptr.pp_complex[i+1][i], a->stride, &t.ptr.p_complex[1], 1, "N", ae_v_len(i+1,n-1));
            e->ptr.p_double[i] = a->ptr.pp_complex[i+1][i].x;
            if( ae_c_neq_d(taui,(double)(0)) )
            {
                
                /*
                 * Apply H(i) from both sides to A(i+1:n,i+1:n)
                 */
                a->ptr.pp_complex[i+1][i] = ae_complex_from_i(1);
                
                /*
                 * Compute  x := tau * A * v  storing y in TAU
                 */
                ae_v_cmove(&t.ptr.p_complex[1], 1, &a->ptr.pp_complex[i+1][i], a->stride, "N", ae_v_len(1,n-i-1));
                hermitianmatrixvectormultiply(a, isupper, i+1, n-1, &t, taui, &t2, _state);
                ae_v_cmove(&tau->ptr.p_complex[i], 1, &t2.ptr.p_complex[1], 1, "N", ae_v_len(i,n-2));
                
                /*
                 * Compute  w := x - 1/2 * tau * (x'*v) * v
                 */
                v = ae_v_cdotproduct(&tau->ptr.p_complex[i], 1, "Conj", &a->ptr.pp_complex[i+1][i], a->stride, "N", ae_v_len(i,n-2));
                alpha = ae_c_neg(ae_c_mul(ae_c_mul_d(taui,0.5),v));
                ae_v_caddc(&tau->ptr.p_complex[i], 1, &a->ptr.pp_complex[i+1][i], a->stride, "N", ae_v_len(i,n-2), alpha);
                
                /*
                 * Apply the transformation as a rank-2 update:
                 * A := A - v * w' - w * v'
                 */
                ae_v_cmove(&t.ptr.p_complex[1], 1, &a->ptr.pp_complex[i+1][i], a->stride, "N", ae_v_len(1,n-i-1));
                ae_v_cmove(&t2.ptr.p_complex[1], 1, &tau->ptr.p_complex[i], 1, "N", ae_v_len(1,n-i-1));
                hermitianrank2update(a, isupper, i+1, n-1, &t, &t2, &t3, ae_complex_from_i(-1), _state);
            }
            else
            {
                a->ptr.pp_complex[i+1][i+1] = ae_complex_from_d(a->ptr.pp_complex[i+1][i+1].x);
            }
            a->ptr.pp_complex[i+1][i] = ae_complex_from_d(e->ptr.p_double[i]);
            d->ptr.p_double[i] = a->ptr.pp_complex[i][i].x;
            tau->ptr.p_complex[i] = taui;
        }
        d->ptr.p_double[n-1] = a->ptr.pp_complex[n-1][n-1].x;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unpacking matrix Q which reduces a Hermitian matrix to a real  tridiagonal
form.


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
  ! Multithreaded acceleration is NOT supported for this function.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

Input parameters:
    A       -   the result of a HMatrixTD subroutine
    N       -   size of matrix A.
    IsUpper -   storage format (a parameter of HMatrixTD subroutine)
    Tau     -   the result of a HMatrixTD subroutine

Output parameters:
    Q       -   transformation matrix.
                array with elements [0..N-1, 0..N-1].

  -- ALGLIB --
     Copyright 2005-2010 by Bochkanov Sergey
*************************************************************************/
void hmatrixtdunpackq(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     /* Complex */ ae_vector* tau,
     /* Complex */ ae_matrix* q,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector v;
    ae_vector work;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(q);
    ae_vector_init(&v, 0, DT_COMPLEX, _state);
    ae_vector_init(&work, 0, DT_COMPLEX, _state);

    if( n==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * init
     */
    ae_matrix_set_length(q, n-1+1, n-1+1, _state);
    ae_vector_set_length(&v, n+1, _state);
    ae_vector_set_length(&work, n-1+1, _state);
    
    /*
     * MKL version
     */
    if( hmatrixtdunpackqmkl(a, n, isupper, tau, q, _state) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * ALGLIB version
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i==j )
            {
                q->ptr.pp_complex[i][j] = ae_complex_from_i(1);
            }
            else
            {
                q->ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
    }
    if( isupper )
    {
        for(i=0; i<=n-2; i++)
        {
            
            /*
             * Apply H(i)
             */
            ae_v_cmove(&v.ptr.p_complex[1], 1, &a->ptr.pp_complex[0][i+1], a->stride, "N", ae_v_len(1,i+1));
            v.ptr.p_complex[i+1] = ae_complex_from_i(1);
            complexapplyreflectionfromtheleft(q, tau->ptr.p_complex[i], &v, 0, i, 0, n-1, &work, _state);
        }
    }
    else
    {
        for(i=n-2; i>=0; i--)
        {
            
            /*
             * Apply H(i)
             */
            ae_v_cmove(&v.ptr.p_complex[1], 1, &a->ptr.pp_complex[i+1][i], a->stride, "N", ae_v_len(1,n-i-1));
            v.ptr.p_complex[1] = ae_complex_from_i(1);
            complexapplyreflectionfromtheleft(q, tau->ptr.p_complex[i], &v, i+1, n-1, 0, n-1, &work, _state);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Base case for complex QR

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994.
     Sergey Bochkanov, ALGLIB project, translation from FORTRAN to
     pseudocode, 2007-2010.
*************************************************************************/
static void ortfac_cmatrixqrbasecase(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_vector* work,
     /* Complex */ ae_vector* t,
     /* Complex */ ae_vector* tau,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    ae_int_t mmi;
    ae_int_t minmn;
    ae_complex tmp;


    minmn = ae_minint(m, n, _state);
    if( minmn<=0 )
    {
        return;
    }
    
    /*
     * Test the input arguments
     */
    k = ae_minint(m, n, _state);
    for(i=0; i<=k-1; i++)
    {
        
        /*
         * Generate elementary reflector H(i) to annihilate A(i+1:m,i)
         */
        mmi = m-i;
        ae_v_cmove(&t->ptr.p_complex[1], 1, &a->ptr.pp_complex[i][i], a->stride, "N", ae_v_len(1,mmi));
        complexgeneratereflection(t, mmi, &tmp, _state);
        tau->ptr.p_complex[i] = tmp;
        ae_v_cmove(&a->ptr.pp_complex[i][i], a->stride, &t->ptr.p_complex[1], 1, "N", ae_v_len(i,m-1));
        t->ptr.p_complex[1] = ae_complex_from_i(1);
        if( i<n-1 )
        {
            
            /*
             * Apply H'(i) to A(i:m,i+1:n) from the left
             */
            complexapplyreflectionfromtheleft(a, ae_c_conj(tau->ptr.p_complex[i], _state), t, i, m-1, i+1, n-1, work, _state);
        }
    }
}


/*************************************************************************
Base case for complex LQ

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994.
     Sergey Bochkanov, ALGLIB project, translation from FORTRAN to
     pseudocode, 2007-2010.
*************************************************************************/
static void ortfac_cmatrixlqbasecase(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_vector* work,
     /* Complex */ ae_vector* t,
     /* Complex */ ae_vector* tau,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t minmn;
    ae_complex tmp;


    minmn = ae_minint(m, n, _state);
    if( minmn<=0 )
    {
        return;
    }
    
    /*
     * Test the input arguments
     */
    for(i=0; i<=minmn-1; i++)
    {
        
        /*
         * Generate elementary reflector H(i)
         *
         * NOTE: ComplexGenerateReflection() generates left reflector,
         * i.e. H which reduces x by applyiong from the left, but we
         * need RIGHT reflector. So we replace H=E-tau*v*v' by H^H,
         * which changes v to conj(v).
         */
        ae_v_cmove(&t->ptr.p_complex[1], 1, &a->ptr.pp_complex[i][i], 1, "Conj", ae_v_len(1,n-i));
        complexgeneratereflection(t, n-i, &tmp, _state);
        tau->ptr.p_complex[i] = tmp;
        ae_v_cmove(&a->ptr.pp_complex[i][i], 1, &t->ptr.p_complex[1], 1, "Conj", ae_v_len(i,n-1));
        t->ptr.p_complex[1] = ae_complex_from_i(1);
        if( i<m-1 )
        {
            
            /*
             * Apply H'(i)
             */
            complexapplyreflectionfromtheright(a, tau->ptr.p_complex[i], t, i+1, m-1, i, n-1, work, _state);
        }
    }
}


/*************************************************************************
Generate block reflector:
* fill unused parts of reflectors matrix by zeros
* fill diagonal of reflectors matrix by ones
* generate triangular factor T

PARAMETERS:
    A           -   either LengthA*BlockSize (if ColumnwiseA) or
                    BlockSize*LengthA (if not ColumnwiseA) matrix of
                    elementary reflectors.
                    Modified on exit.
    Tau         -   scalar factors
    ColumnwiseA -   reflectors are stored in rows or in columns
    LengthA     -   length of largest reflector
    BlockSize   -   number of reflectors
    T           -   array[BlockSize,2*BlockSize]. Left BlockSize*BlockSize
                    submatrix stores triangular factor on exit.
    WORK        -   array[BlockSize]
    
  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
static void ortfac_rmatrixblockreflector(/* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* tau,
     ae_bool columnwisea,
     ae_int_t lengtha,
     ae_int_t blocksize,
     /* Real    */ ae_matrix* t,
     /* Real    */ ae_vector* work,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;


    
    /*
     * fill beginning of new column with zeros,
     * load 1.0 in the first non-zero element
     */
    for(k=0; k<=blocksize-1; k++)
    {
        if( columnwisea )
        {
            for(i=0; i<=k-1; i++)
            {
                a->ptr.pp_double[i][k] = (double)(0);
            }
        }
        else
        {
            for(i=0; i<=k-1; i++)
            {
                a->ptr.pp_double[k][i] = (double)(0);
            }
        }
        a->ptr.pp_double[k][k] = (double)(1);
    }
    
    /*
     * Calculate Gram matrix of A
     */
    for(i=0; i<=blocksize-1; i++)
    {
        for(j=0; j<=blocksize-1; j++)
        {
            t->ptr.pp_double[i][blocksize+j] = (double)(0);
        }
    }
    for(k=0; k<=lengtha-1; k++)
    {
        for(j=1; j<=blocksize-1; j++)
        {
            if( columnwisea )
            {
                v = a->ptr.pp_double[k][j];
                if( ae_fp_neq(v,(double)(0)) )
                {
                    ae_v_addd(&t->ptr.pp_double[j][blocksize], 1, &a->ptr.pp_double[k][0], 1, ae_v_len(blocksize,blocksize+j-1), v);
                }
            }
            else
            {
                v = a->ptr.pp_double[j][k];
                if( ae_fp_neq(v,(double)(0)) )
                {
                    ae_v_addd(&t->ptr.pp_double[j][blocksize], 1, &a->ptr.pp_double[0][k], a->stride, ae_v_len(blocksize,blocksize+j-1), v);
                }
            }
        }
    }
    
    /*
     * Prepare Y (stored in TmpA) and T (stored in TmpT)
     */
    for(k=0; k<=blocksize-1; k++)
    {
        
        /*
         * fill non-zero part of T, use pre-calculated Gram matrix
         */
        ae_v_move(&work->ptr.p_double[0], 1, &t->ptr.pp_double[k][blocksize], 1, ae_v_len(0,k-1));
        for(i=0; i<=k-1; i++)
        {
            v = ae_v_dotproduct(&t->ptr.pp_double[i][i], 1, &work->ptr.p_double[i], 1, ae_v_len(i,k-1));
            t->ptr.pp_double[i][k] = -tau->ptr.p_double[k]*v;
        }
        t->ptr.pp_double[k][k] = -tau->ptr.p_double[k];
        
        /*
         * Rest of T is filled by zeros
         */
        for(i=k+1; i<=blocksize-1; i++)
        {
            t->ptr.pp_double[i][k] = (double)(0);
        }
    }
}


/*************************************************************************
Generate block reflector (complex):
* fill unused parts of reflectors matrix by zeros
* fill diagonal of reflectors matrix by ones
* generate triangular factor T


  -- ALGLIB routine --
     17.02.2010
     Bochkanov Sergey
*************************************************************************/
static void ortfac_cmatrixblockreflector(/* Complex */ ae_matrix* a,
     /* Complex */ ae_vector* tau,
     ae_bool columnwisea,
     ae_int_t lengtha,
     ae_int_t blocksize,
     /* Complex */ ae_matrix* t,
     /* Complex */ ae_vector* work,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    ae_complex v;


    
    /*
     * Prepare Y (stored in TmpA) and T (stored in TmpT)
     */
    for(k=0; k<=blocksize-1; k++)
    {
        
        /*
         * fill beginning of new column with zeros,
         * load 1.0 in the first non-zero element
         */
        if( columnwisea )
        {
            for(i=0; i<=k-1; i++)
            {
                a->ptr.pp_complex[i][k] = ae_complex_from_i(0);
            }
        }
        else
        {
            for(i=0; i<=k-1; i++)
            {
                a->ptr.pp_complex[k][i] = ae_complex_from_i(0);
            }
        }
        a->ptr.pp_complex[k][k] = ae_complex_from_i(1);
        
        /*
         * fill non-zero part of T,
         */
        for(i=0; i<=k-1; i++)
        {
            if( columnwisea )
            {
                v = ae_v_cdotproduct(&a->ptr.pp_complex[k][i], a->stride, "Conj", &a->ptr.pp_complex[k][k], a->stride, "N", ae_v_len(k,lengtha-1));
            }
            else
            {
                v = ae_v_cdotproduct(&a->ptr.pp_complex[i][k], 1, "N", &a->ptr.pp_complex[k][k], 1, "Conj", ae_v_len(k,lengtha-1));
            }
            work->ptr.p_complex[i] = v;
        }
        for(i=0; i<=k-1; i++)
        {
            v = ae_v_cdotproduct(&t->ptr.pp_complex[i][i], 1, "N", &work->ptr.p_complex[i], 1, "N", ae_v_len(i,k-1));
            t->ptr.pp_complex[i][k] = ae_c_neg(ae_c_mul(tau->ptr.p_complex[k],v));
        }
        t->ptr.pp_complex[k][k] = ae_c_neg(tau->ptr.p_complex[k]);
        
        /*
         * Rest of T is filled by zeros
         */
        for(i=k+1; i<=blocksize-1; i++)
        {
            t->ptr.pp_complex[i][k] = ae_complex_from_i(0);
        }
    }
}


/*$ End $*/
