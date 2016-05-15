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
#include "svd.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Singular value decomposition of a rectangular matrix.

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

The algorithm calculates the singular value decomposition of a matrix of
size MxN: A = U * S * V^T

The algorithm finds the singular values and, optionally, matrices U and V^T.
The algorithm can find both first min(M,N) columns of matrix U and rows of
matrix V^T (singular vectors), and matrices U and V^T wholly (of sizes MxM
and NxN respectively).

Take into account that the subroutine does not return matrix V but V^T.

Input parameters:
    A           -   matrix to be decomposed.
                    Array whose indexes range within [0..M-1, 0..N-1].
    M           -   number of rows in matrix A.
    N           -   number of columns in matrix A.
    UNeeded     -   0, 1 or 2. See the description of the parameter U.
    VTNeeded    -   0, 1 or 2. See the description of the parameter VT.
    AdditionalMemory -
                    If the parameter:
                     * equals 0, the algorithm doesn’t use additional
                       memory (lower requirements, lower performance).
                     * equals 1, the algorithm uses additional
                       memory of size min(M,N)*min(M,N) of real numbers.
                       It often speeds up the algorithm.
                     * equals 2, the algorithm uses additional
                       memory of size M*min(M,N) of real numbers.
                       It allows to get a maximum performance.
                    The recommended value of the parameter is 2.

Output parameters:
    W           -   contains singular values in descending order.
    U           -   if UNeeded=0, U isn't changed, the left singular vectors
                    are not calculated.
                    if Uneeded=1, U contains left singular vectors (first
                    min(M,N) columns of matrix U). Array whose indexes range
                    within [0..M-1, 0..Min(M,N)-1].
                    if UNeeded=2, U contains matrix U wholly. Array whose
                    indexes range within [0..M-1, 0..M-1].
    VT          -   if VTNeeded=0, VT isn’t changed, the right singular vectors
                    are not calculated.
                    if VTNeeded=1, VT contains right singular vectors (first
                    min(M,N) rows of matrix V^T). Array whose indexes range
                    within [0..min(M,N)-1, 0..N-1].
                    if VTNeeded=2, VT contains matrix V^T wholly. Array whose
                    indexes range within [0..N-1, 0..N-1].

  -- ALGLIB --
     Copyright 2005 by Bochkanov Sergey
*************************************************************************/
ae_bool rmatrixsvd(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     ae_int_t uneeded,
     ae_int_t vtneeded,
     ae_int_t additionalmemory,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* u,
     /* Real    */ ae_matrix* vt,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_vector tauq;
    ae_vector taup;
    ae_vector tau;
    ae_vector e;
    ae_vector work;
    ae_matrix t2;
    ae_bool isupper;
    ae_int_t minmn;
    ae_int_t ncu;
    ae_int_t nrvt;
    ae_int_t nru;
    ae_int_t ncvt;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_clear(w);
    ae_matrix_clear(u);
    ae_matrix_clear(vt);
    ae_vector_init(&tauq, 0, DT_REAL, _state);
    ae_vector_init(&taup, 0, DT_REAL, _state);
    ae_vector_init(&tau, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_matrix_init(&t2, 0, 0, DT_REAL, _state);

    result = ae_true;
    if( m==0||n==0 )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_assert(uneeded>=0&&uneeded<=2, "SVDDecomposition: wrong parameters!", _state);
    ae_assert(vtneeded>=0&&vtneeded<=2, "SVDDecomposition: wrong parameters!", _state);
    ae_assert(additionalmemory>=0&&additionalmemory<=2, "SVDDecomposition: wrong parameters!", _state);
    
    /*
     * initialize
     */
    minmn = ae_minint(m, n, _state);
    ae_vector_set_length(w, minmn+1, _state);
    ncu = 0;
    nru = 0;
    if( uneeded==1 )
    {
        nru = m;
        ncu = minmn;
        ae_matrix_set_length(u, nru-1+1, ncu-1+1, _state);
    }
    if( uneeded==2 )
    {
        nru = m;
        ncu = m;
        ae_matrix_set_length(u, nru-1+1, ncu-1+1, _state);
    }
    nrvt = 0;
    ncvt = 0;
    if( vtneeded==1 )
    {
        nrvt = minmn;
        ncvt = n;
        ae_matrix_set_length(vt, nrvt-1+1, ncvt-1+1, _state);
    }
    if( vtneeded==2 )
    {
        nrvt = n;
        ncvt = n;
        ae_matrix_set_length(vt, nrvt-1+1, ncvt-1+1, _state);
    }
    
    /*
     * M much larger than N
     * Use bidiagonal reduction with QR-decomposition
     */
    if( ae_fp_greater((double)(m),1.6*n) )
    {
        if( uneeded==0 )
        {
            
            /*
             * No left singular vectors to be computed
             */
            rmatrixqr(a, m, n, &tau, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    a->ptr.pp_double[i][j] = (double)(0);
                }
            }
            rmatrixbd(a, n, n, &tauq, &taup, _state);
            rmatrixbdunpackpt(a, n, n, &taup, nrvt, vt, _state);
            rmatrixbdunpackdiagonals(a, n, n, &isupper, w, &e, _state);
            result = rmatrixbdsvd(w, &e, n, isupper, ae_false, u, 0, a, 0, vt, ncvt, _state);
            ae_frame_leave(_state);
            return result;
        }
        else
        {
            
            /*
             * Left singular vectors (may be full matrix U) to be computed
             */
            rmatrixqr(a, m, n, &tau, _state);
            rmatrixqrunpackq(a, m, n, &tau, ncu, u, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    a->ptr.pp_double[i][j] = (double)(0);
                }
            }
            rmatrixbd(a, n, n, &tauq, &taup, _state);
            rmatrixbdunpackpt(a, n, n, &taup, nrvt, vt, _state);
            rmatrixbdunpackdiagonals(a, n, n, &isupper, w, &e, _state);
            if( additionalmemory<1 )
            {
                
                /*
                 * No additional memory can be used
                 */
                rmatrixbdmultiplybyq(a, n, n, &tauq, u, m, n, ae_true, ae_false, _state);
                result = rmatrixbdsvd(w, &e, n, isupper, ae_false, u, m, a, 0, vt, ncvt, _state);
            }
            else
            {
                
                /*
                 * Large U. Transforming intermediate matrix T2
                 */
                ae_vector_set_length(&work, ae_maxint(m, n, _state)+1, _state);
                rmatrixbdunpackq(a, n, n, &tauq, n, &t2, _state);
                copymatrix(u, 0, m-1, 0, n-1, a, 0, m-1, 0, n-1, _state);
                inplacetranspose(&t2, 0, n-1, 0, n-1, &work, _state);
                result = rmatrixbdsvd(w, &e, n, isupper, ae_false, u, 0, &t2, n, vt, ncvt, _state);
                rmatrixgemm(m, n, n, 1.0, a, 0, 0, 0, &t2, 0, 0, 1, 0.0, u, 0, 0, _state);
            }
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * N much larger than M
     * Use bidiagonal reduction with LQ-decomposition
     */
    if( ae_fp_greater((double)(n),1.6*m) )
    {
        if( vtneeded==0 )
        {
            
            /*
             * No right singular vectors to be computed
             */
            rmatrixlq(a, m, n, &tau, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=i+1; j<=m-1; j++)
                {
                    a->ptr.pp_double[i][j] = (double)(0);
                }
            }
            rmatrixbd(a, m, m, &tauq, &taup, _state);
            rmatrixbdunpackq(a, m, m, &tauq, ncu, u, _state);
            rmatrixbdunpackdiagonals(a, m, m, &isupper, w, &e, _state);
            ae_vector_set_length(&work, m+1, _state);
            inplacetranspose(u, 0, nru-1, 0, ncu-1, &work, _state);
            result = rmatrixbdsvd(w, &e, m, isupper, ae_false, a, 0, u, nru, vt, 0, _state);
            inplacetranspose(u, 0, nru-1, 0, ncu-1, &work, _state);
            ae_frame_leave(_state);
            return result;
        }
        else
        {
            
            /*
             * Right singular vectors (may be full matrix VT) to be computed
             */
            rmatrixlq(a, m, n, &tau, _state);
            rmatrixlqunpackq(a, m, n, &tau, nrvt, vt, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=i+1; j<=m-1; j++)
                {
                    a->ptr.pp_double[i][j] = (double)(0);
                }
            }
            rmatrixbd(a, m, m, &tauq, &taup, _state);
            rmatrixbdunpackq(a, m, m, &tauq, ncu, u, _state);
            rmatrixbdunpackdiagonals(a, m, m, &isupper, w, &e, _state);
            ae_vector_set_length(&work, ae_maxint(m, n, _state)+1, _state);
            inplacetranspose(u, 0, nru-1, 0, ncu-1, &work, _state);
            if( additionalmemory<1 )
            {
                
                /*
                 * No additional memory available
                 */
                rmatrixbdmultiplybyp(a, m, m, &taup, vt, m, n, ae_false, ae_true, _state);
                result = rmatrixbdsvd(w, &e, m, isupper, ae_false, a, 0, u, nru, vt, n, _state);
            }
            else
            {
                
                /*
                 * Large VT. Transforming intermediate matrix T2
                 */
                rmatrixbdunpackpt(a, m, m, &taup, m, &t2, _state);
                result = rmatrixbdsvd(w, &e, m, isupper, ae_false, a, 0, u, nru, &t2, m, _state);
                copymatrix(vt, 0, m-1, 0, n-1, a, 0, m-1, 0, n-1, _state);
                rmatrixgemm(m, n, m, 1.0, &t2, 0, 0, 0, a, 0, 0, 0, 0.0, vt, 0, 0, _state);
            }
            inplacetranspose(u, 0, nru-1, 0, ncu-1, &work, _state);
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * M<=N
     * We can use inplace transposition of U to get rid of columnwise operations
     */
    if( m<=n )
    {
        rmatrixbd(a, m, n, &tauq, &taup, _state);
        rmatrixbdunpackq(a, m, n, &tauq, ncu, u, _state);
        rmatrixbdunpackpt(a, m, n, &taup, nrvt, vt, _state);
        rmatrixbdunpackdiagonals(a, m, n, &isupper, w, &e, _state);
        ae_vector_set_length(&work, m+1, _state);
        inplacetranspose(u, 0, nru-1, 0, ncu-1, &work, _state);
        result = rmatrixbdsvd(w, &e, minmn, isupper, ae_false, a, 0, u, nru, vt, ncvt, _state);
        inplacetranspose(u, 0, nru-1, 0, ncu-1, &work, _state);
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Simple bidiagonal reduction
     */
    rmatrixbd(a, m, n, &tauq, &taup, _state);
    rmatrixbdunpackq(a, m, n, &tauq, ncu, u, _state);
    rmatrixbdunpackpt(a, m, n, &taup, nrvt, vt, _state);
    rmatrixbdunpackdiagonals(a, m, n, &isupper, w, &e, _state);
    if( additionalmemory<2||uneeded==0 )
    {
        
        /*
         * We cant use additional memory or there is no need in such operations
         */
        result = rmatrixbdsvd(w, &e, minmn, isupper, ae_false, u, nru, a, 0, vt, ncvt, _state);
    }
    else
    {
        
        /*
         * We can use additional memory
         */
        ae_matrix_set_length(&t2, minmn-1+1, m-1+1, _state);
        copyandtranspose(u, 0, m-1, 0, minmn-1, &t2, 0, minmn-1, 0, m-1, _state);
        result = rmatrixbdsvd(w, &e, minmn, isupper, ae_false, u, 0, &t2, m, vt, ncvt, _state);
        copyandtranspose(&t2, 0, minmn-1, 0, m-1, u, 0, m-1, 0, minmn-1, _state);
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_rmatrixsvd(/* Real    */ ae_matrix* a,
    ae_int_t m,
    ae_int_t n,
    ae_int_t uneeded,
    ae_int_t vtneeded,
    ae_int_t additionalmemory,
    /* Real    */ ae_vector* w,
    /* Real    */ ae_matrix* u,
    /* Real    */ ae_matrix* vt, ae_state *_state)
{
    return rmatrixsvd(a,m,n,uneeded,vtneeded,additionalmemory,w,u,vt, _state);
}


/*$ End $*/
