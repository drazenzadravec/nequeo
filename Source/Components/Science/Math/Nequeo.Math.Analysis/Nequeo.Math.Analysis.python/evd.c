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
#include "evd.h"


/*$ Declarations $*/
static ae_bool evd_tridiagonalevd(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_int_t zneeded,
     /* Real    */ ae_matrix* z,
     ae_state *_state);
static void evd_tdevde2(double a,
     double b,
     double c,
     double* rt1,
     double* rt2,
     ae_state *_state);
static void evd_tdevdev2(double a,
     double b,
     double c,
     double* rt1,
     double* rt2,
     double* cs1,
     double* sn1,
     ae_state *_state);
static double evd_tdevdpythag(double a, double b, ae_state *_state);
static double evd_tdevdextsign(double a, double b, ae_state *_state);
static ae_bool evd_internalbisectioneigenvalues(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_int_t irange,
     ae_int_t iorder,
     double vl,
     double vu,
     ae_int_t il,
     ae_int_t iu,
     double abstol,
     /* Real    */ ae_vector* w,
     ae_int_t* m,
     ae_int_t* nsplit,
     /* Integer */ ae_vector* iblock,
     /* Integer */ ae_vector* isplit,
     ae_int_t* errorcode,
     ae_state *_state);
static void evd_internaldstein(ae_int_t n,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t m,
     /* Real    */ ae_vector* w,
     /* Integer */ ae_vector* iblock,
     /* Integer */ ae_vector* isplit,
     /* Real    */ ae_matrix* z,
     /* Integer */ ae_vector* ifail,
     ae_int_t* info,
     ae_state *_state);
static void evd_tdininternaldlagtf(ae_int_t n,
     /* Real    */ ae_vector* a,
     double lambdav,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* c,
     double tol,
     /* Real    */ ae_vector* d,
     /* Integer */ ae_vector* iin,
     ae_int_t* info,
     ae_state *_state);
static void evd_tdininternaldlagts(ae_int_t n,
     /* Real    */ ae_vector* a,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_vector* d,
     /* Integer */ ae_vector* iin,
     /* Real    */ ae_vector* y,
     double* tol,
     ae_int_t* info,
     ae_state *_state);
static void evd_internaldlaebz(ae_int_t ijob,
     ae_int_t nitmax,
     ae_int_t n,
     ae_int_t mmax,
     ae_int_t minp,
     double abstol,
     double reltol,
     double pivmin,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     /* Real    */ ae_vector* e2,
     /* Integer */ ae_vector* nval,
     /* Real    */ ae_matrix* ab,
     /* Real    */ ae_vector* c,
     ae_int_t* mout,
     /* Integer */ ae_matrix* nab,
     /* Real    */ ae_vector* work,
     /* Integer */ ae_vector* iwork,
     ae_int_t* info,
     ae_state *_state);
static void evd_rmatrixinternaltrevc(/* Real    */ ae_matrix* t,
     ae_int_t n,
     ae_int_t side,
     ae_int_t howmny,
     /* Boolean */ ae_vector* vselect,
     /* Real    */ ae_matrix* vl,
     /* Real    */ ae_matrix* vr,
     ae_int_t* m,
     ae_int_t* info,
     ae_state *_state);
static void evd_internaltrevc(/* Real    */ ae_matrix* t,
     ae_int_t n,
     ae_int_t side,
     ae_int_t howmny,
     /* Boolean */ ae_vector* vselect,
     /* Real    */ ae_matrix* vl,
     /* Real    */ ae_matrix* vr,
     ae_int_t* m,
     ae_int_t* info,
     ae_state *_state);
static void evd_internalhsevdlaln2(ae_bool ltrans,
     ae_int_t na,
     ae_int_t nw,
     double smin,
     double ca,
     /* Real    */ ae_matrix* a,
     double d1,
     double d2,
     /* Real    */ ae_matrix* b,
     double wr,
     double wi,
     /* Boolean */ ae_vector* rswap4,
     /* Boolean */ ae_vector* zswap4,
     /* Integer */ ae_matrix* ipivot44,
     /* Real    */ ae_vector* civ4,
     /* Real    */ ae_vector* crv4,
     /* Real    */ ae_matrix* x,
     double* scl,
     double* xnorm,
     ae_int_t* info,
     ae_state *_state);
static void evd_internalhsevdladiv(double a,
     double b,
     double c,
     double d,
     double* p,
     double* q,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Finding the eigenvalues and eigenvectors of a symmetric matrix

The algorithm finds eigen pairs of a symmetric matrix by reducing it to
tridiagonal form and using the QL/QR algorithm.

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
    A       -   symmetric matrix which is given by its upper or lower
                triangular part.
                Array whose indexes range within [0..N-1, 0..N-1].
    N       -   size of matrix A.
    ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                If ZNeeded is equal to:
                 * 0, the eigenvectors are not returned;
                 * 1, the eigenvectors are returned.
    IsUpper -   storage format.

Output parameters:
    D       -   eigenvalues in ascending order.
                Array whose index ranges within [0..N-1].
    Z       -   if ZNeeded is equal to:
                 * 0, Z hasn’t changed;
                 * 1, Z contains the eigenvectors.
                Array whose indexes range within [0..N-1, 0..N-1].
                The eigenvectors are stored in the matrix columns.

Result:
    True, if the algorithm has converged.
    False, if the algorithm hasn't converged (rare case).

  -- ALGLIB --
     Copyright 2005-2008 by Bochkanov Sergey
*************************************************************************/
ae_bool smatrixevd(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_int_t zneeded,
     ae_bool isupper,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_matrix* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_vector tau;
    ae_vector e;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_clear(d);
    ae_matrix_clear(z);
    ae_vector_init(&tau, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);

    ae_assert(zneeded==0||zneeded==1, "SMatrixEVD: incorrect ZNeeded", _state);
    smatrixtd(a, n, isupper, &tau, d, &e, _state);
    if( zneeded==1 )
    {
        smatrixtdunpackq(a, n, isupper, &tau, z, _state);
    }
    result = smatrixtdevd(d, &e, n, zneeded, z, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Subroutine for finding the eigenvalues (and eigenvectors) of  a  symmetric
matrix  in  a  given half open interval (A, B] by using  a  bisection  and
inverse iteration

Input parameters:
    A       -   symmetric matrix which is given by its upper or lower
                triangular part. Array [0..N-1, 0..N-1].
    N       -   size of matrix A.
    ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                If ZNeeded is equal to:
                 * 0, the eigenvectors are not returned;
                 * 1, the eigenvectors are returned.
    IsUpperA -  storage format of matrix A.
    B1, B2 -    half open interval (B1, B2] to search eigenvalues in.

Output parameters:
    M       -   number of eigenvalues found in a given half-interval (M>=0).
    W       -   array of the eigenvalues found.
                Array whose index ranges within [0..M-1].
    Z       -   if ZNeeded is equal to:
                 * 0, Z hasn’t changed;
                 * 1, Z contains eigenvectors.
                Array whose indexes range within [0..N-1, 0..M-1].
                The eigenvectors are stored in the matrix columns.

Result:
    True, if successful. M contains the number of eigenvalues in the given
    half-interval (could be equal to 0), W contains the eigenvalues,
    Z contains the eigenvectors (if needed).

    False, if the bisection method subroutine wasn't able to find the
    eigenvalues in the given interval or if the inverse iteration subroutine
    wasn't able to find all the corresponding eigenvectors.
    In that case, the eigenvalues and eigenvectors are not returned,
    M is equal to 0.

  -- ALGLIB --
     Copyright 07.01.2006 by Bochkanov Sergey
*************************************************************************/
ae_bool smatrixevdr(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_int_t zneeded,
     ae_bool isupper,
     double b1,
     double b2,
     ae_int_t* m,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_vector tau;
    ae_vector e;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    *m = 0;
    ae_vector_clear(w);
    ae_matrix_clear(z);
    ae_vector_init(&tau, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);

    ae_assert(zneeded==0||zneeded==1, "SMatrixTDEVDR: incorrect ZNeeded", _state);
    smatrixtd(a, n, isupper, &tau, w, &e, _state);
    if( zneeded==1 )
    {
        smatrixtdunpackq(a, n, isupper, &tau, z, _state);
    }
    result = smatrixtdevdr(w, &e, n, zneeded, b1, b2, m, z, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Subroutine for finding the eigenvalues and  eigenvectors  of  a  symmetric
matrix with given indexes by using bisection and inverse iteration methods.

Input parameters:
    A       -   symmetric matrix which is given by its upper or lower
                triangular part. Array whose indexes range within [0..N-1, 0..N-1].
    N       -   size of matrix A.
    ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                If ZNeeded is equal to:
                 * 0, the eigenvectors are not returned;
                 * 1, the eigenvectors are returned.
    IsUpperA -  storage format of matrix A.
    I1, I2 -    index interval for searching (from I1 to I2).
                0 <= I1 <= I2 <= N-1.

Output parameters:
    W       -   array of the eigenvalues found.
                Array whose index ranges within [0..I2-I1].
    Z       -   if ZNeeded is equal to:
                 * 0, Z hasn’t changed;
                 * 1, Z contains eigenvectors.
                Array whose indexes range within [0..N-1, 0..I2-I1].
                In that case, the eigenvectors are stored in the matrix columns.

Result:
    True, if successful. W contains the eigenvalues, Z contains the
    eigenvectors (if needed).

    False, if the bisection method subroutine wasn't able to find the
    eigenvalues in the given interval or if the inverse iteration subroutine
    wasn't able to find all the corresponding eigenvectors.
    In that case, the eigenvalues and eigenvectors are not returned.

  -- ALGLIB --
     Copyright 07.01.2006 by Bochkanov Sergey
*************************************************************************/
ae_bool smatrixevdi(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_int_t zneeded,
     ae_bool isupper,
     ae_int_t i1,
     ae_int_t i2,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_vector tau;
    ae_vector e;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_clear(w);
    ae_matrix_clear(z);
    ae_vector_init(&tau, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);

    ae_assert(zneeded==0||zneeded==1, "SMatrixEVDI: incorrect ZNeeded", _state);
    smatrixtd(a, n, isupper, &tau, w, &e, _state);
    if( zneeded==1 )
    {
        smatrixtdunpackq(a, n, isupper, &tau, z, _state);
    }
    result = smatrixtdevdi(w, &e, n, zneeded, i1, i2, z, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Finding the eigenvalues and eigenvectors of a Hermitian matrix

The algorithm finds eigen pairs of a Hermitian matrix by  reducing  it  to
real tridiagonal form and using the QL/QR algorithm.

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
    A       -   Hermitian matrix which is given  by  its  upper  or  lower
                triangular part.
                Array whose indexes range within [0..N-1, 0..N-1].
    N       -   size of matrix A.
    IsUpper -   storage format.
    ZNeeded -   flag controlling whether the eigenvectors  are  needed  or
                not. If ZNeeded is equal to:
                 * 0, the eigenvectors are not returned;
                 * 1, the eigenvectors are returned.

Output parameters:
    D       -   eigenvalues in ascending order.
                Array whose index ranges within [0..N-1].
    Z       -   if ZNeeded is equal to:
                 * 0, Z hasn’t changed;
                 * 1, Z contains the eigenvectors.
                Array whose indexes range within [0..N-1, 0..N-1].
                The eigenvectors are stored in the matrix columns.

Result:
    True, if the algorithm has converged.
    False, if the algorithm hasn't converged (rare case).

Note:
    eigenvectors of Hermitian matrix are defined up to  multiplication  by
    a complex number L, such that |L|=1.

  -- ALGLIB --
     Copyright 2005, 23 March 2007 by Bochkanov Sergey
*************************************************************************/
ae_bool hmatrixevd(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_int_t zneeded,
     ae_bool isupper,
     /* Real    */ ae_vector* d,
     /* Complex */ ae_matrix* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_vector tau;
    ae_vector e;
    ae_matrix t;
    ae_matrix qz;
    ae_matrix q;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_clear(d);
    ae_matrix_clear(z);
    ae_vector_init(&tau, 0, DT_COMPLEX, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);
    ae_matrix_init(&t, 0, 0, DT_REAL, _state);
    ae_matrix_init(&qz, 0, 0, DT_REAL, _state);
    ae_matrix_init(&q, 0, 0, DT_COMPLEX, _state);

    ae_assert(zneeded==0||zneeded==1, "HermitianEVD: incorrect ZNeeded", _state);
    
    /*
     * Reduce to tridiagonal form
     */
    hmatrixtd(a, n, isupper, &tau, d, &e, _state);
    if( zneeded==1 )
    {
        hmatrixtdunpackq(a, n, isupper, &tau, &q, _state);
        zneeded = 2;
    }
    
    /*
     * TDEVD
     */
    result = smatrixtdevd(d, &e, n, zneeded, &t, _state);
    
    /*
     * Eigenvectors are needed
     * Calculate Z = Q*T = Re(Q)*T + i*Im(Q)*T
     */
    if( result&&zneeded!=0 )
    {
        ae_matrix_set_length(z, n, n, _state);
        ae_matrix_set_length(&qz, n, 2*n, _state);
        
        /*
         * Calculate Re(Q)*T
         */
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                qz.ptr.pp_double[i][j] = q.ptr.pp_complex[i][j].x;
            }
        }
        rmatrixgemm(n, n, n, 1.0, &qz, 0, 0, 0, &t, 0, 0, 0, 0.0, &qz, 0, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                z->ptr.pp_complex[i][j].x = qz.ptr.pp_double[i][n+j];
            }
        }
        
        /*
         * Calculate Im(Q)*T
         */
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                qz.ptr.pp_double[i][j] = q.ptr.pp_complex[i][j].y;
            }
        }
        rmatrixgemm(n, n, n, 1.0, &qz, 0, 0, 0, &t, 0, 0, 0, 0.0, &qz, 0, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                z->ptr.pp_complex[i][j].y = qz.ptr.pp_double[i][n+j];
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Subroutine for finding the eigenvalues (and eigenvectors) of  a  Hermitian
matrix  in  a  given half-interval (A, B] by using a bisection and inverse
iteration

Input parameters:
    A       -   Hermitian matrix which is given  by  its  upper  or  lower
                triangular  part.  Array  whose   indexes   range   within
                [0..N-1, 0..N-1].
    N       -   size of matrix A.
    ZNeeded -   flag controlling whether the eigenvectors  are  needed  or
                not. If ZNeeded is equal to:
                 * 0, the eigenvectors are not returned;
                 * 1, the eigenvectors are returned.
    IsUpperA -  storage format of matrix A.
    B1, B2 -    half-interval (B1, B2] to search eigenvalues in.

Output parameters:
    M       -   number of eigenvalues found in a given half-interval, M>=0
    W       -   array of the eigenvalues found.
                Array whose index ranges within [0..M-1].
    Z       -   if ZNeeded is equal to:
                 * 0, Z hasn’t changed;
                 * 1, Z contains eigenvectors.
                Array whose indexes range within [0..N-1, 0..M-1].
                The eigenvectors are stored in the matrix columns.

Result:
    True, if successful. M contains the number of eigenvalues in the given
    half-interval (could be equal to 0), W contains the eigenvalues,
    Z contains the eigenvectors (if needed).

    False, if the bisection method subroutine  wasn't  able  to  find  the
    eigenvalues  in  the  given  interval  or  if  the  inverse  iteration
    subroutine  wasn't  able  to  find all the corresponding eigenvectors.
    In that case, the eigenvalues and eigenvectors are not returned, M  is
    equal to 0.

Note:
    eigen vectors of Hermitian matrix are defined up to multiplication  by
    a complex number L, such as |L|=1.

  -- ALGLIB --
     Copyright 07.01.2006, 24.03.2007 by Bochkanov Sergey.
*************************************************************************/
ae_bool hmatrixevdr(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_int_t zneeded,
     ae_bool isupper,
     double b1,
     double b2,
     ae_int_t* m,
     /* Real    */ ae_vector* w,
     /* Complex */ ae_matrix* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_matrix q;
    ae_matrix t;
    ae_vector tau;
    ae_vector e;
    ae_vector work;
    ae_int_t i;
    ae_int_t k;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    *m = 0;
    ae_vector_clear(w);
    ae_matrix_clear(z);
    ae_matrix_init(&q, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&t, 0, 0, DT_REAL, _state);
    ae_vector_init(&tau, 0, DT_COMPLEX, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);

    ae_assert(zneeded==0||zneeded==1, "HermitianEigenValuesAndVectorsInInterval: incorrect ZNeeded", _state);
    
    /*
     * Reduce to tridiagonal form
     */
    hmatrixtd(a, n, isupper, &tau, w, &e, _state);
    if( zneeded==1 )
    {
        hmatrixtdunpackq(a, n, isupper, &tau, &q, _state);
        zneeded = 2;
    }
    
    /*
     * Bisection and inverse iteration
     */
    result = smatrixtdevdr(w, &e, n, zneeded, b1, b2, m, &t, _state);
    
    /*
     * Eigenvectors are needed
     * Calculate Z = Q*T = Re(Q)*T + i*Im(Q)*T
     */
    if( (result&&zneeded!=0)&&*m!=0 )
    {
        ae_vector_set_length(&work, *m-1+1, _state);
        ae_matrix_set_length(z, n-1+1, *m-1+1, _state);
        for(i=0; i<=n-1; i++)
        {
            
            /*
             * Calculate real part
             */
            for(k=0; k<=*m-1; k++)
            {
                work.ptr.p_double[k] = (double)(0);
            }
            for(k=0; k<=n-1; k++)
            {
                v = q.ptr.pp_complex[i][k].x;
                ae_v_addd(&work.ptr.p_double[0], 1, &t.ptr.pp_double[k][0], 1, ae_v_len(0,*m-1), v);
            }
            for(k=0; k<=*m-1; k++)
            {
                z->ptr.pp_complex[i][k].x = work.ptr.p_double[k];
            }
            
            /*
             * Calculate imaginary part
             */
            for(k=0; k<=*m-1; k++)
            {
                work.ptr.p_double[k] = (double)(0);
            }
            for(k=0; k<=n-1; k++)
            {
                v = q.ptr.pp_complex[i][k].y;
                ae_v_addd(&work.ptr.p_double[0], 1, &t.ptr.pp_double[k][0], 1, ae_v_len(0,*m-1), v);
            }
            for(k=0; k<=*m-1; k++)
            {
                z->ptr.pp_complex[i][k].y = work.ptr.p_double[k];
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Subroutine for finding the eigenvalues and  eigenvectors  of  a  Hermitian
matrix with given indexes by using bisection and inverse iteration methods

Input parameters:
    A       -   Hermitian matrix which is given  by  its  upper  or  lower
                triangular part.
                Array whose indexes range within [0..N-1, 0..N-1].
    N       -   size of matrix A.
    ZNeeded -   flag controlling whether the eigenvectors  are  needed  or
                not. If ZNeeded is equal to:
                 * 0, the eigenvectors are not returned;
                 * 1, the eigenvectors are returned.
    IsUpperA -  storage format of matrix A.
    I1, I2 -    index interval for searching (from I1 to I2).
                0 <= I1 <= I2 <= N-1.

Output parameters:
    W       -   array of the eigenvalues found.
                Array whose index ranges within [0..I2-I1].
    Z       -   if ZNeeded is equal to:
                 * 0, Z hasn’t changed;
                 * 1, Z contains eigenvectors.
                Array whose indexes range within [0..N-1, 0..I2-I1].
                In  that  case,  the eigenvectors are stored in the matrix
                columns.

Result:
    True, if successful. W contains the eigenvalues, Z contains the
    eigenvectors (if needed).

    False, if the bisection method subroutine  wasn't  able  to  find  the
    eigenvalues  in  the  given  interval  or  if  the  inverse  iteration
    subroutine wasn't able to find  all  the  corresponding  eigenvectors.
    In that case, the eigenvalues and eigenvectors are not returned.

Note:
    eigen vectors of Hermitian matrix are defined up to multiplication  by
    a complex number L, such as |L|=1.

  -- ALGLIB --
     Copyright 07.01.2006, 24.03.2007 by Bochkanov Sergey.
*************************************************************************/
ae_bool hmatrixevdi(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_int_t zneeded,
     ae_bool isupper,
     ae_int_t i1,
     ae_int_t i2,
     /* Real    */ ae_vector* w,
     /* Complex */ ae_matrix* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_matrix q;
    ae_matrix t;
    ae_vector tau;
    ae_vector e;
    ae_vector work;
    ae_int_t i;
    ae_int_t k;
    double v;
    ae_int_t m;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_clear(w);
    ae_matrix_clear(z);
    ae_matrix_init(&q, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&t, 0, 0, DT_REAL, _state);
    ae_vector_init(&tau, 0, DT_COMPLEX, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);

    ae_assert(zneeded==0||zneeded==1, "HermitianEigenValuesAndVectorsByIndexes: incorrect ZNeeded", _state);
    
    /*
     * Reduce to tridiagonal form
     */
    hmatrixtd(a, n, isupper, &tau, w, &e, _state);
    if( zneeded==1 )
    {
        hmatrixtdunpackq(a, n, isupper, &tau, &q, _state);
        zneeded = 2;
    }
    
    /*
     * Bisection and inverse iteration
     */
    result = smatrixtdevdi(w, &e, n, zneeded, i1, i2, &t, _state);
    
    /*
     * Eigenvectors are needed
     * Calculate Z = Q*T = Re(Q)*T + i*Im(Q)*T
     */
    m = i2-i1+1;
    if( result&&zneeded!=0 )
    {
        ae_vector_set_length(&work, m-1+1, _state);
        ae_matrix_set_length(z, n-1+1, m-1+1, _state);
        for(i=0; i<=n-1; i++)
        {
            
            /*
             * Calculate real part
             */
            for(k=0; k<=m-1; k++)
            {
                work.ptr.p_double[k] = (double)(0);
            }
            for(k=0; k<=n-1; k++)
            {
                v = q.ptr.pp_complex[i][k].x;
                ae_v_addd(&work.ptr.p_double[0], 1, &t.ptr.pp_double[k][0], 1, ae_v_len(0,m-1), v);
            }
            for(k=0; k<=m-1; k++)
            {
                z->ptr.pp_complex[i][k].x = work.ptr.p_double[k];
            }
            
            /*
             * Calculate imaginary part
             */
            for(k=0; k<=m-1; k++)
            {
                work.ptr.p_double[k] = (double)(0);
            }
            for(k=0; k<=n-1; k++)
            {
                v = q.ptr.pp_complex[i][k].y;
                ae_v_addd(&work.ptr.p_double[0], 1, &t.ptr.pp_double[k][0], 1, ae_v_len(0,m-1), v);
            }
            for(k=0; k<=m-1; k++)
            {
                z->ptr.pp_complex[i][k].y = work.ptr.p_double[k];
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Finding the eigenvalues and eigenvectors of a tridiagonal symmetric matrix

The algorithm finds the eigen pairs of a tridiagonal symmetric matrix by
using an QL/QR algorithm with implicit shifts.

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
    D       -   the main diagonal of a tridiagonal matrix.
                Array whose index ranges within [0..N-1].
    E       -   the secondary diagonal of a tridiagonal matrix.
                Array whose index ranges within [0..N-2].
    N       -   size of matrix A.
    ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                If ZNeeded is equal to:
                 * 0, the eigenvectors are not needed;
                 * 1, the eigenvectors of a tridiagonal matrix
                   are multiplied by the square matrix Z. It is used if the
                   tridiagonal matrix is obtained by the similarity
                   transformation of a symmetric matrix;
                 * 2, the eigenvectors of a tridiagonal matrix replace the
                   square matrix Z;
                 * 3, matrix Z contains the first row of the eigenvectors
                   matrix.
    Z       -   if ZNeeded=1, Z contains the square matrix by which the
                eigenvectors are multiplied.
                Array whose indexes range within [0..N-1, 0..N-1].

Output parameters:
    D       -   eigenvalues in ascending order.
                Array whose index ranges within [0..N-1].
    Z       -   if ZNeeded is equal to:
                 * 0, Z hasn’t changed;
                 * 1, Z contains the product of a given matrix (from the left)
                   and the eigenvectors matrix (from the right);
                 * 2, Z contains the eigenvectors.
                 * 3, Z contains the first row of the eigenvectors matrix.
                If ZNeeded<3, Z is the array whose indexes range within [0..N-1, 0..N-1].
                In that case, the eigenvectors are stored in the matrix columns.
                If ZNeeded=3, Z is the array whose indexes range within [0..0, 0..N-1].

Result:
    True, if the algorithm has converged.
    False, if the algorithm hasn't converged.

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994
*************************************************************************/
ae_bool smatrixtdevd(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_int_t zneeded,
     /* Real    */ ae_matrix* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _e;
    ae_vector d1;
    ae_vector e1;
    ae_vector ex;
    ae_matrix z1;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_e, e, _state);
    e = &_e;
    ae_vector_init(&d1, 0, DT_REAL, _state);
    ae_vector_init(&e1, 0, DT_REAL, _state);
    ae_vector_init(&ex, 0, DT_REAL, _state);
    ae_matrix_init(&z1, 0, 0, DT_REAL, _state);

    ae_assert(n>=1, "SMatrixTDEVD: N<=0", _state);
    ae_assert(zneeded>=0&&zneeded<=3, "SMatrixTDEVD: incorrect ZNeeded", _state);
    result = ae_false;
    
    /*
     * Preprocess Z: make ZNeeded equal to 0, 1 or 3.
     * Ensure that memory for Z is allocated.
     */
    if( zneeded==2 )
    {
        
        /*
         * Load identity to Z
         */
        rmatrixsetlengthatleast(z, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                z->ptr.pp_double[i][j] = 0.0;
            }
            z->ptr.pp_double[i][i] = 1.0;
        }
        zneeded = 1;
    }
    if( zneeded==3 )
    {
        
        /*
         * Allocate memory
         */
        rmatrixsetlengthatleast(z, 1, n, _state);
    }
    
    /*
     * Try to solve problem with MKL
     */
    ae_vector_set_length(&ex, n, _state);
    for(i=0; i<=n-2; i++)
    {
        ex.ptr.p_double[i] = e->ptr.p_double[i];
    }
    if( smatrixtdevdmkl(d, &ex, n, zneeded, z, &result, _state) )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Prepare 1-based task
     */
    ae_vector_set_length(&d1, n+1, _state);
    ae_vector_set_length(&e1, n+1, _state);
    ae_v_move(&d1.ptr.p_double[1], 1, &d->ptr.p_double[0], 1, ae_v_len(1,n));
    if( n>1 )
    {
        ae_v_move(&e1.ptr.p_double[1], 1, &e->ptr.p_double[0], 1, ae_v_len(1,n-1));
    }
    if( zneeded==1 )
    {
        ae_matrix_set_length(&z1, n+1, n+1, _state);
        for(i=1; i<=n; i++)
        {
            ae_v_move(&z1.ptr.pp_double[i][1], 1, &z->ptr.pp_double[i-1][0], 1, ae_v_len(1,n));
        }
    }
    
    /*
     * Solve 1-based task
     */
    result = evd_tridiagonalevd(&d1, &e1, n, zneeded, &z1, _state);
    if( !result )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Convert back to 0-based result
     */
    ae_v_move(&d->ptr.p_double[0], 1, &d1.ptr.p_double[1], 1, ae_v_len(0,n-1));
    if( zneeded!=0 )
    {
        if( zneeded==1 )
        {
            for(i=1; i<=n; i++)
            {
                ae_v_move(&z->ptr.pp_double[i-1][0], 1, &z1.ptr.pp_double[i][1], 1, ae_v_len(0,n-1));
            }
            ae_frame_leave(_state);
            return result;
        }
        if( zneeded==2 )
        {
            ae_matrix_set_length(z, n-1+1, n-1+1, _state);
            for(i=1; i<=n; i++)
            {
                ae_v_move(&z->ptr.pp_double[i-1][0], 1, &z1.ptr.pp_double[i][1], 1, ae_v_len(0,n-1));
            }
            ae_frame_leave(_state);
            return result;
        }
        if( zneeded==3 )
        {
            ae_matrix_set_length(z, 0+1, n-1+1, _state);
            ae_v_move(&z->ptr.pp_double[0][0], 1, &z1.ptr.pp_double[1][1], 1, ae_v_len(0,n-1));
            ae_frame_leave(_state);
            return result;
        }
        ae_assert(ae_false, "SMatrixTDEVD: Incorrect ZNeeded!", _state);
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Subroutine for finding the tridiagonal matrix eigenvalues/vectors in a
given half-interval (A, B] by using bisection and inverse iteration.

Input parameters:
    D       -   the main diagonal of a tridiagonal matrix.
                Array whose index ranges within [0..N-1].
    E       -   the secondary diagonal of a tridiagonal matrix.
                Array whose index ranges within [0..N-2].
    N       -   size of matrix, N>=0.
    ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                If ZNeeded is equal to:
                 * 0, the eigenvectors are not needed;
                 * 1, the eigenvectors of a tridiagonal matrix are multiplied
                   by the square matrix Z. It is used if the tridiagonal
                   matrix is obtained by the similarity transformation
                   of a symmetric matrix.
                 * 2, the eigenvectors of a tridiagonal matrix replace matrix Z.
    A, B    -   half-interval (A, B] to search eigenvalues in.
    Z       -   if ZNeeded is equal to:
                 * 0, Z isn't used and remains unchanged;
                 * 1, Z contains the square matrix (array whose indexes range
                   within [0..N-1, 0..N-1]) which reduces the given symmetric
                   matrix to tridiagonal form;
                 * 2, Z isn't used (but changed on the exit).

Output parameters:
    D       -   array of the eigenvalues found.
                Array whose index ranges within [0..M-1].
    M       -   number of eigenvalues found in the given half-interval (M>=0).
    Z       -   if ZNeeded is equal to:
                 * 0, doesn't contain any information;
                 * 1, contains the product of a given NxN matrix Z (from the
                   left) and NxM matrix of the eigenvectors found (from the
                   right). Array whose indexes range within [0..N-1, 0..M-1].
                 * 2, contains the matrix of the eigenvectors found.
                   Array whose indexes range within [0..N-1, 0..M-1].

Result:

    True, if successful. In that case, M contains the number of eigenvalues
    in the given half-interval (could be equal to 0), D contains the eigenvalues,
    Z contains the eigenvectors (if needed).
    It should be noted that the subroutine changes the size of arrays D and Z.

    False, if the bisection method subroutine wasn't able to find the
    eigenvalues in the given interval or if the inverse iteration subroutine
    wasn't able to find all the corresponding eigenvectors. In that case,
    the eigenvalues and eigenvectors are not returned, M is equal to 0.

  -- ALGLIB --
     Copyright 31.03.2008 by Bochkanov Sergey
*************************************************************************/
ae_bool smatrixtdevdr(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_int_t zneeded,
     double a,
     double b,
     ae_int_t* m,
     /* Real    */ ae_matrix* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t errorcode;
    ae_int_t nsplit;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t cr;
    ae_vector iblock;
    ae_vector isplit;
    ae_vector ifail;
    ae_vector d1;
    ae_vector e1;
    ae_vector w;
    ae_matrix z2;
    ae_matrix z3;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    *m = 0;
    ae_vector_init(&iblock, 0, DT_INT, _state);
    ae_vector_init(&isplit, 0, DT_INT, _state);
    ae_vector_init(&ifail, 0, DT_INT, _state);
    ae_vector_init(&d1, 0, DT_REAL, _state);
    ae_vector_init(&e1, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_matrix_init(&z2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&z3, 0, 0, DT_REAL, _state);

    ae_assert(zneeded>=0&&zneeded<=2, "SMatrixTDEVDR: incorrect ZNeeded!", _state);
    
    /*
     * Special cases
     */
    if( ae_fp_less_eq(b,a) )
    {
        *m = 0;
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    if( n<=0 )
    {
        *m = 0;
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Copy D,E to D1, E1
     */
    ae_vector_set_length(&d1, n+1, _state);
    ae_v_move(&d1.ptr.p_double[1], 1, &d->ptr.p_double[0], 1, ae_v_len(1,n));
    if( n>1 )
    {
        ae_vector_set_length(&e1, n-1+1, _state);
        ae_v_move(&e1.ptr.p_double[1], 1, &e->ptr.p_double[0], 1, ae_v_len(1,n-1));
    }
    
    /*
     * No eigen vectors
     */
    if( zneeded==0 )
    {
        result = evd_internalbisectioneigenvalues(&d1, &e1, n, 2, 1, a, b, 0, 0, (double)(-1), &w, m, &nsplit, &iblock, &isplit, &errorcode, _state);
        if( !result||*m==0 )
        {
            *m = 0;
            ae_frame_leave(_state);
            return result;
        }
        ae_vector_set_length(d, *m-1+1, _state);
        ae_v_move(&d->ptr.p_double[0], 1, &w.ptr.p_double[1], 1, ae_v_len(0,*m-1));
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Eigen vectors are multiplied by Z
     */
    if( zneeded==1 )
    {
        
        /*
         * Find eigen pairs
         */
        result = evd_internalbisectioneigenvalues(&d1, &e1, n, 2, 2, a, b, 0, 0, (double)(-1), &w, m, &nsplit, &iblock, &isplit, &errorcode, _state);
        if( !result||*m==0 )
        {
            *m = 0;
            ae_frame_leave(_state);
            return result;
        }
        evd_internaldstein(n, &d1, &e1, *m, &w, &iblock, &isplit, &z2, &ifail, &cr, _state);
        if( cr!=0 )
        {
            *m = 0;
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Sort eigen values and vectors
         */
        for(i=1; i<=*m; i++)
        {
            k = i;
            for(j=i; j<=*m; j++)
            {
                if( ae_fp_less(w.ptr.p_double[j],w.ptr.p_double[k]) )
                {
                    k = j;
                }
            }
            v = w.ptr.p_double[i];
            w.ptr.p_double[i] = w.ptr.p_double[k];
            w.ptr.p_double[k] = v;
            for(j=1; j<=n; j++)
            {
                v = z2.ptr.pp_double[j][i];
                z2.ptr.pp_double[j][i] = z2.ptr.pp_double[j][k];
                z2.ptr.pp_double[j][k] = v;
            }
        }
        
        /*
         * Transform Z2 and overwrite Z
         */
        ae_matrix_set_length(&z3, *m+1, n+1, _state);
        for(i=1; i<=*m; i++)
        {
            ae_v_move(&z3.ptr.pp_double[i][1], 1, &z2.ptr.pp_double[1][i], z2.stride, ae_v_len(1,n));
        }
        for(i=1; i<=n; i++)
        {
            for(j=1; j<=*m; j++)
            {
                v = ae_v_dotproduct(&z->ptr.pp_double[i-1][0], 1, &z3.ptr.pp_double[j][1], 1, ae_v_len(0,n-1));
                z2.ptr.pp_double[i][j] = v;
            }
        }
        ae_matrix_set_length(z, n-1+1, *m-1+1, _state);
        for(i=1; i<=*m; i++)
        {
            ae_v_move(&z->ptr.pp_double[0][i-1], z->stride, &z2.ptr.pp_double[1][i], z2.stride, ae_v_len(0,n-1));
        }
        
        /*
         * Store W
         */
        ae_vector_set_length(d, *m-1+1, _state);
        for(i=1; i<=*m; i++)
        {
            d->ptr.p_double[i-1] = w.ptr.p_double[i];
        }
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Eigen vectors are stored in Z
     */
    if( zneeded==2 )
    {
        
        /*
         * Find eigen pairs
         */
        result = evd_internalbisectioneigenvalues(&d1, &e1, n, 2, 2, a, b, 0, 0, (double)(-1), &w, m, &nsplit, &iblock, &isplit, &errorcode, _state);
        if( !result||*m==0 )
        {
            *m = 0;
            ae_frame_leave(_state);
            return result;
        }
        evd_internaldstein(n, &d1, &e1, *m, &w, &iblock, &isplit, &z2, &ifail, &cr, _state);
        if( cr!=0 )
        {
            *m = 0;
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Sort eigen values and vectors
         */
        for(i=1; i<=*m; i++)
        {
            k = i;
            for(j=i; j<=*m; j++)
            {
                if( ae_fp_less(w.ptr.p_double[j],w.ptr.p_double[k]) )
                {
                    k = j;
                }
            }
            v = w.ptr.p_double[i];
            w.ptr.p_double[i] = w.ptr.p_double[k];
            w.ptr.p_double[k] = v;
            for(j=1; j<=n; j++)
            {
                v = z2.ptr.pp_double[j][i];
                z2.ptr.pp_double[j][i] = z2.ptr.pp_double[j][k];
                z2.ptr.pp_double[j][k] = v;
            }
        }
        
        /*
         * Store W
         */
        ae_vector_set_length(d, *m-1+1, _state);
        for(i=1; i<=*m; i++)
        {
            d->ptr.p_double[i-1] = w.ptr.p_double[i];
        }
        ae_matrix_set_length(z, n-1+1, *m-1+1, _state);
        for(i=1; i<=*m; i++)
        {
            ae_v_move(&z->ptr.pp_double[0][i-1], z->stride, &z2.ptr.pp_double[1][i], z2.stride, ae_v_len(0,n-1));
        }
        ae_frame_leave(_state);
        return result;
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Subroutine for finding tridiagonal matrix eigenvalues/vectors with given
indexes (in ascending order) by using the bisection and inverse iteraion.

Input parameters:
    D       -   the main diagonal of a tridiagonal matrix.
                Array whose index ranges within [0..N-1].
    E       -   the secondary diagonal of a tridiagonal matrix.
                Array whose index ranges within [0..N-2].
    N       -   size of matrix. N>=0.
    ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                If ZNeeded is equal to:
                 * 0, the eigenvectors are not needed;
                 * 1, the eigenvectors of a tridiagonal matrix are multiplied
                   by the square matrix Z. It is used if the
                   tridiagonal matrix is obtained by the similarity transformation
                   of a symmetric matrix.
                 * 2, the eigenvectors of a tridiagonal matrix replace
                   matrix Z.
    I1, I2  -   index interval for searching (from I1 to I2).
                0 <= I1 <= I2 <= N-1.
    Z       -   if ZNeeded is equal to:
                 * 0, Z isn't used and remains unchanged;
                 * 1, Z contains the square matrix (array whose indexes range within [0..N-1, 0..N-1])
                   which reduces the given symmetric matrix to  tridiagonal form;
                 * 2, Z isn't used (but changed on the exit).

Output parameters:
    D       -   array of the eigenvalues found.
                Array whose index ranges within [0..I2-I1].
    Z       -   if ZNeeded is equal to:
                 * 0, doesn't contain any information;
                 * 1, contains the product of a given NxN matrix Z (from the left) and
                   Nx(I2-I1) matrix of the eigenvectors found (from the right).
                   Array whose indexes range within [0..N-1, 0..I2-I1].
                 * 2, contains the matrix of the eigenvalues found.
                   Array whose indexes range within [0..N-1, 0..I2-I1].


Result:

    True, if successful. In that case, D contains the eigenvalues,
    Z contains the eigenvectors (if needed).
    It should be noted that the subroutine changes the size of arrays D and Z.

    False, if the bisection method subroutine wasn't able to find the eigenvalues
    in the given interval or if the inverse iteration subroutine wasn't able
    to find all the corresponding eigenvectors. In that case, the eigenvalues
    and eigenvectors are not returned.

  -- ALGLIB --
     Copyright 25.12.2005 by Bochkanov Sergey
*************************************************************************/
ae_bool smatrixtdevdi(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_int_t zneeded,
     ae_int_t i1,
     ae_int_t i2,
     /* Real    */ ae_matrix* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t errorcode;
    ae_int_t nsplit;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t m;
    ae_int_t cr;
    ae_vector iblock;
    ae_vector isplit;
    ae_vector ifail;
    ae_vector w;
    ae_vector d1;
    ae_vector e1;
    ae_matrix z2;
    ae_matrix z3;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&iblock, 0, DT_INT, _state);
    ae_vector_init(&isplit, 0, DT_INT, _state);
    ae_vector_init(&ifail, 0, DT_INT, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&d1, 0, DT_REAL, _state);
    ae_vector_init(&e1, 0, DT_REAL, _state);
    ae_matrix_init(&z2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&z3, 0, 0, DT_REAL, _state);

    ae_assert((0<=i1&&i1<=i2)&&i2<n, "SMatrixTDEVDI: incorrect I1/I2!", _state);
    
    /*
     * Copy D,E to D1, E1
     */
    ae_vector_set_length(&d1, n+1, _state);
    ae_v_move(&d1.ptr.p_double[1], 1, &d->ptr.p_double[0], 1, ae_v_len(1,n));
    if( n>1 )
    {
        ae_vector_set_length(&e1, n-1+1, _state);
        ae_v_move(&e1.ptr.p_double[1], 1, &e->ptr.p_double[0], 1, ae_v_len(1,n-1));
    }
    
    /*
     * No eigen vectors
     */
    if( zneeded==0 )
    {
        result = evd_internalbisectioneigenvalues(&d1, &e1, n, 3, 1, (double)(0), (double)(0), i1+1, i2+1, (double)(-1), &w, &m, &nsplit, &iblock, &isplit, &errorcode, _state);
        if( !result )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( m!=i2-i1+1 )
        {
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
        ae_vector_set_length(d, m-1+1, _state);
        for(i=1; i<=m; i++)
        {
            d->ptr.p_double[i-1] = w.ptr.p_double[i];
        }
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Eigen vectors are multiplied by Z
     */
    if( zneeded==1 )
    {
        
        /*
         * Find eigen pairs
         */
        result = evd_internalbisectioneigenvalues(&d1, &e1, n, 3, 2, (double)(0), (double)(0), i1+1, i2+1, (double)(-1), &w, &m, &nsplit, &iblock, &isplit, &errorcode, _state);
        if( !result )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( m!=i2-i1+1 )
        {
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
        evd_internaldstein(n, &d1, &e1, m, &w, &iblock, &isplit, &z2, &ifail, &cr, _state);
        if( cr!=0 )
        {
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Sort eigen values and vectors
         */
        for(i=1; i<=m; i++)
        {
            k = i;
            for(j=i; j<=m; j++)
            {
                if( ae_fp_less(w.ptr.p_double[j],w.ptr.p_double[k]) )
                {
                    k = j;
                }
            }
            v = w.ptr.p_double[i];
            w.ptr.p_double[i] = w.ptr.p_double[k];
            w.ptr.p_double[k] = v;
            for(j=1; j<=n; j++)
            {
                v = z2.ptr.pp_double[j][i];
                z2.ptr.pp_double[j][i] = z2.ptr.pp_double[j][k];
                z2.ptr.pp_double[j][k] = v;
            }
        }
        
        /*
         * Transform Z2 and overwrite Z
         */
        ae_matrix_set_length(&z3, m+1, n+1, _state);
        for(i=1; i<=m; i++)
        {
            ae_v_move(&z3.ptr.pp_double[i][1], 1, &z2.ptr.pp_double[1][i], z2.stride, ae_v_len(1,n));
        }
        for(i=1; i<=n; i++)
        {
            for(j=1; j<=m; j++)
            {
                v = ae_v_dotproduct(&z->ptr.pp_double[i-1][0], 1, &z3.ptr.pp_double[j][1], 1, ae_v_len(0,n-1));
                z2.ptr.pp_double[i][j] = v;
            }
        }
        ae_matrix_set_length(z, n-1+1, m-1+1, _state);
        for(i=1; i<=m; i++)
        {
            ae_v_move(&z->ptr.pp_double[0][i-1], z->stride, &z2.ptr.pp_double[1][i], z2.stride, ae_v_len(0,n-1));
        }
        
        /*
         * Store W
         */
        ae_vector_set_length(d, m-1+1, _state);
        for(i=1; i<=m; i++)
        {
            d->ptr.p_double[i-1] = w.ptr.p_double[i];
        }
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Eigen vectors are stored in Z
     */
    if( zneeded==2 )
    {
        
        /*
         * Find eigen pairs
         */
        result = evd_internalbisectioneigenvalues(&d1, &e1, n, 3, 2, (double)(0), (double)(0), i1+1, i2+1, (double)(-1), &w, &m, &nsplit, &iblock, &isplit, &errorcode, _state);
        if( !result )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( m!=i2-i1+1 )
        {
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
        evd_internaldstein(n, &d1, &e1, m, &w, &iblock, &isplit, &z2, &ifail, &cr, _state);
        if( cr!=0 )
        {
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Sort eigen values and vectors
         */
        for(i=1; i<=m; i++)
        {
            k = i;
            for(j=i; j<=m; j++)
            {
                if( ae_fp_less(w.ptr.p_double[j],w.ptr.p_double[k]) )
                {
                    k = j;
                }
            }
            v = w.ptr.p_double[i];
            w.ptr.p_double[i] = w.ptr.p_double[k];
            w.ptr.p_double[k] = v;
            for(j=1; j<=n; j++)
            {
                v = z2.ptr.pp_double[j][i];
                z2.ptr.pp_double[j][i] = z2.ptr.pp_double[j][k];
                z2.ptr.pp_double[j][k] = v;
            }
        }
        
        /*
         * Store Z
         */
        ae_matrix_set_length(z, n-1+1, m-1+1, _state);
        for(i=1; i<=m; i++)
        {
            ae_v_move(&z->ptr.pp_double[0][i-1], z->stride, &z2.ptr.pp_double[1][i], z2.stride, ae_v_len(0,n-1));
        }
        
        /*
         * Store W
         */
        ae_vector_set_length(d, m-1+1, _state);
        for(i=1; i<=m; i++)
        {
            d->ptr.p_double[i-1] = w.ptr.p_double[i];
        }
        ae_frame_leave(_state);
        return result;
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Finding eigenvalues and eigenvectors of a general (unsymmetric) matrix

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes one  important  improvement   of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison. Speed-up provided by MKL for this particular problem (EVD)
  ! is really high, because  MKL  uses combination of (a) better low-level
  ! optimizations, and (b) better EVD algorithms.
  !
  ! On one particular SSE-capable  machine  for  N=1024,  commercial  MKL-
  ! -capable ALGLIB was:
  ! * 7-10 times faster than open source "generic C" version
  ! * 15-18 times faster than "pure C#" version
  !
  ! Multithreaded acceleration is NOT supported for this function.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

The algorithm finds eigenvalues and eigenvectors of a general matrix by
using the QR algorithm with multiple shifts. The algorithm can find
eigenvalues and both left and right eigenvectors.

The right eigenvector is a vector x such that A*x = w*x, and the left
eigenvector is a vector y such that y'*A = w*y' (here y' implies a complex
conjugate transposition of vector y).

Input parameters:
    A       -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
    N       -   size of matrix A.
    VNeeded -   flag controlling whether eigenvectors are needed or not.
                If VNeeded is equal to:
                 * 0, eigenvectors are not returned;
                 * 1, right eigenvectors are returned;
                 * 2, left eigenvectors are returned;
                 * 3, both left and right eigenvectors are returned.

Output parameters:
    WR      -   real parts of eigenvalues.
                Array whose index ranges within [0..N-1].
    WR      -   imaginary parts of eigenvalues.
                Array whose index ranges within [0..N-1].
    VL, VR  -   arrays of left and right eigenvectors (if they are needed).
                If WI[i]=0, the respective eigenvalue is a real number,
                and it corresponds to the column number I of matrices VL/VR.
                If WI[i]>0, we have a pair of complex conjugate numbers with
                positive and negative imaginary parts:
                    the first eigenvalue WR[i] + sqrt(-1)*WI[i];
                    the second eigenvalue WR[i+1] + sqrt(-1)*WI[i+1];
                    WI[i]>0
                    WI[i+1] = -WI[i] < 0
                In that case, the eigenvector  corresponding to the first
                eigenvalue is located in i and i+1 columns of matrices
                VL/VR (the column number i contains the real part, and the
                column number i+1 contains the imaginary part), and the vector
                corresponding to the second eigenvalue is a complex conjugate to
                the first vector.
                Arrays whose indexes range within [0..N-1, 0..N-1].

Result:
    True, if the algorithm has converged.
    False, if the algorithm has not converged.

Note 1:
    Some users may ask the following question: what if WI[N-1]>0?
    WI[N] must contain an eigenvalue which is complex conjugate to the
    N-th eigenvalue, but the array has only size N?
    The answer is as follows: such a situation cannot occur because the
    algorithm finds a pairs of eigenvalues, therefore, if WI[i]>0, I is
    strictly less than N-1.

Note 2:
    The algorithm performance depends on the value of the internal parameter
    NS of the InternalSchurDecomposition subroutine which defines the number
    of shifts in the QR algorithm (similarly to the block width in block-matrix
    algorithms of linear algebra). If you require maximum performance
    on your machine, it is recommended to adjust this parameter manually.


See also the InternalTREVC subroutine.

The algorithm is based on the LAPACK 3.0 library.
*************************************************************************/
ae_bool rmatrixevd(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_int_t vneeded,
     /* Real    */ ae_vector* wr,
     /* Real    */ ae_vector* wi,
     /* Real    */ ae_matrix* vl,
     /* Real    */ ae_matrix* vr,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_matrix a1;
    ae_matrix vl1;
    ae_matrix vr1;
    ae_matrix s1;
    ae_matrix s;
    ae_matrix dummy;
    ae_vector wr1;
    ae_vector wi1;
    ae_vector tau;
    ae_int_t i;
    ae_int_t info;
    ae_vector sel1;
    ae_int_t m1;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_clear(wr);
    ae_vector_clear(wi);
    ae_matrix_clear(vl);
    ae_matrix_clear(vr);
    ae_matrix_init(&a1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vl1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vr1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&s1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&s, 0, 0, DT_REAL, _state);
    ae_matrix_init(&dummy, 0, 0, DT_REAL, _state);
    ae_vector_init(&wr1, 0, DT_REAL, _state);
    ae_vector_init(&wi1, 0, DT_REAL, _state);
    ae_vector_init(&tau, 0, DT_REAL, _state);
    ae_vector_init(&sel1, 0, DT_BOOL, _state);

    ae_assert(vneeded>=0&&vneeded<=3, "RMatrixEVD: incorrect VNeeded!", _state);
    if( vneeded==0 )
    {
        
        /*
         * Eigen values only
         */
        rmatrixhessenberg(a, n, &tau, _state);
        rmatrixinternalschurdecomposition(a, n, 0, 0, wr, wi, &dummy, &info, _state);
        result = info==0;
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Eigen values and vectors
     */
    rmatrixhessenberg(a, n, &tau, _state);
    rmatrixhessenbergunpackq(a, n, &tau, &s, _state);
    rmatrixinternalschurdecomposition(a, n, 1, 1, wr, wi, &s, &info, _state);
    result = info==0;
    if( !result )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( vneeded==1||vneeded==3 )
    {
        ae_matrix_set_length(vr, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            ae_v_move(&vr->ptr.pp_double[i][0], 1, &s.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
        }
    }
    if( vneeded==2||vneeded==3 )
    {
        ae_matrix_set_length(vl, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            ae_v_move(&vl->ptr.pp_double[i][0], 1, &s.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
        }
    }
    evd_rmatrixinternaltrevc(a, n, vneeded, 1, &sel1, vl, vr, &m1, &info, _state);
    result = info==0;
    ae_frame_leave(_state);
    return result;
}


static ae_bool evd_tridiagonalevd(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_int_t zneeded,
     /* Real    */ ae_matrix* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _e;
    ae_int_t maxit;
    ae_int_t i;
    ae_int_t ii;
    ae_int_t iscale;
    ae_int_t j;
    ae_int_t jtot;
    ae_int_t k;
    ae_int_t t;
    ae_int_t l;
    ae_int_t l1;
    ae_int_t lend;
    ae_int_t lendm1;
    ae_int_t lendp1;
    ae_int_t lendsv;
    ae_int_t lm1;
    ae_int_t lsv;
    ae_int_t m;
    ae_int_t mm1;
    ae_int_t nm1;
    ae_int_t nmaxit;
    ae_int_t tmpint;
    double anorm;
    double b;
    double c;
    double eps;
    double eps2;
    double f;
    double g;
    double p;
    double r;
    double rt1;
    double rt2;
    double s;
    double safmax;
    double safmin;
    double ssfmax;
    double ssfmin;
    double tst;
    double tmp;
    ae_vector work1;
    ae_vector work2;
    ae_vector workc;
    ae_vector works;
    ae_vector wtemp;
    ae_bool gotoflag;
    ae_int_t zrows;
    ae_bool wastranspose;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_e, e, _state);
    e = &_e;
    ae_vector_init(&work1, 0, DT_REAL, _state);
    ae_vector_init(&work2, 0, DT_REAL, _state);
    ae_vector_init(&workc, 0, DT_REAL, _state);
    ae_vector_init(&works, 0, DT_REAL, _state);
    ae_vector_init(&wtemp, 0, DT_REAL, _state);

    ae_assert(zneeded>=0&&zneeded<=3, "TridiagonalEVD: Incorrent ZNeeded", _state);
    
    /*
     * Quick return if possible
     */
    if( zneeded<0||zneeded>3 )
    {
        result = ae_false;
        ae_frame_leave(_state);
        return result;
    }
    result = ae_true;
    if( n==0 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( n==1 )
    {
        if( zneeded==2||zneeded==3 )
        {
            ae_matrix_set_length(z, 1+1, 1+1, _state);
            z->ptr.pp_double[1][1] = (double)(1);
        }
        ae_frame_leave(_state);
        return result;
    }
    maxit = 30;
    
    /*
     * Initialize arrays
     */
    ae_vector_set_length(&wtemp, n+1, _state);
    ae_vector_set_length(&work1, n-1+1, _state);
    ae_vector_set_length(&work2, n-1+1, _state);
    ae_vector_set_length(&workc, n+1, _state);
    ae_vector_set_length(&works, n+1, _state);
    
    /*
     * Determine the unit roundoff and over/underflow thresholds.
     */
    eps = ae_machineepsilon;
    eps2 = ae_sqr(eps, _state);
    safmin = ae_minrealnumber;
    safmax = ae_maxrealnumber;
    ssfmax = ae_sqrt(safmax, _state)/3;
    ssfmin = ae_sqrt(safmin, _state)/eps2;
    
    /*
     * Prepare Z
     *
     * Here we are using transposition to get rid of column operations
     *
     */
    wastranspose = ae_false;
    zrows = 0;
    if( zneeded==1 )
    {
        zrows = n;
    }
    if( zneeded==2 )
    {
        zrows = n;
    }
    if( zneeded==3 )
    {
        zrows = 1;
    }
    if( zneeded==1 )
    {
        wastranspose = ae_true;
        inplacetranspose(z, 1, n, 1, n, &wtemp, _state);
    }
    if( zneeded==2 )
    {
        wastranspose = ae_true;
        ae_matrix_set_length(z, n+1, n+1, _state);
        for(i=1; i<=n; i++)
        {
            for(j=1; j<=n; j++)
            {
                if( i==j )
                {
                    z->ptr.pp_double[i][j] = (double)(1);
                }
                else
                {
                    z->ptr.pp_double[i][j] = (double)(0);
                }
            }
        }
    }
    if( zneeded==3 )
    {
        wastranspose = ae_false;
        ae_matrix_set_length(z, 1+1, n+1, _state);
        for(j=1; j<=n; j++)
        {
            if( j==1 )
            {
                z->ptr.pp_double[1][j] = (double)(1);
            }
            else
            {
                z->ptr.pp_double[1][j] = (double)(0);
            }
        }
    }
    nmaxit = n*maxit;
    jtot = 0;
    
    /*
     * Determine where the matrix splits and choose QL or QR iteration
     * for each block, according to whether top or bottom diagonal
     * element is smaller.
     */
    l1 = 1;
    nm1 = n-1;
    for(;;)
    {
        if( l1>n )
        {
            break;
        }
        if( l1>1 )
        {
            e->ptr.p_double[l1-1] = (double)(0);
        }
        gotoflag = ae_false;
        m = l1;
        if( l1<=nm1 )
        {
            for(m=l1; m<=nm1; m++)
            {
                tst = ae_fabs(e->ptr.p_double[m], _state);
                if( ae_fp_eq(tst,(double)(0)) )
                {
                    gotoflag = ae_true;
                    break;
                }
                if( ae_fp_less_eq(tst,ae_sqrt(ae_fabs(d->ptr.p_double[m], _state), _state)*ae_sqrt(ae_fabs(d->ptr.p_double[m+1], _state), _state)*eps) )
                {
                    e->ptr.p_double[m] = (double)(0);
                    gotoflag = ae_true;
                    break;
                }
            }
        }
        if( !gotoflag )
        {
            m = n;
        }
        
        /*
         * label 30:
         */
        l = l1;
        lsv = l;
        lend = m;
        lendsv = lend;
        l1 = m+1;
        if( lend==l )
        {
            continue;
        }
        
        /*
         * Scale submatrix in rows and columns L to LEND
         */
        if( l==lend )
        {
            anorm = ae_fabs(d->ptr.p_double[l], _state);
        }
        else
        {
            anorm = ae_maxreal(ae_fabs(d->ptr.p_double[l], _state)+ae_fabs(e->ptr.p_double[l], _state), ae_fabs(e->ptr.p_double[lend-1], _state)+ae_fabs(d->ptr.p_double[lend], _state), _state);
            for(i=l+1; i<=lend-1; i++)
            {
                anorm = ae_maxreal(anorm, ae_fabs(d->ptr.p_double[i], _state)+ae_fabs(e->ptr.p_double[i], _state)+ae_fabs(e->ptr.p_double[i-1], _state), _state);
            }
        }
        iscale = 0;
        if( ae_fp_eq(anorm,(double)(0)) )
        {
            continue;
        }
        if( ae_fp_greater(anorm,ssfmax) )
        {
            iscale = 1;
            tmp = ssfmax/anorm;
            tmpint = lend-1;
            ae_v_muld(&d->ptr.p_double[l], 1, ae_v_len(l,lend), tmp);
            ae_v_muld(&e->ptr.p_double[l], 1, ae_v_len(l,tmpint), tmp);
        }
        if( ae_fp_less(anorm,ssfmin) )
        {
            iscale = 2;
            tmp = ssfmin/anorm;
            tmpint = lend-1;
            ae_v_muld(&d->ptr.p_double[l], 1, ae_v_len(l,lend), tmp);
            ae_v_muld(&e->ptr.p_double[l], 1, ae_v_len(l,tmpint), tmp);
        }
        
        /*
         * Choose between QL and QR iteration
         */
        if( ae_fp_less(ae_fabs(d->ptr.p_double[lend], _state),ae_fabs(d->ptr.p_double[l], _state)) )
        {
            lend = lsv;
            l = lendsv;
        }
        if( lend>l )
        {
            
            /*
             * QL Iteration
             *
             * Look for small subdiagonal element.
             */
            for(;;)
            {
                gotoflag = ae_false;
                if( l!=lend )
                {
                    lendm1 = lend-1;
                    for(m=l; m<=lendm1; m++)
                    {
                        tst = ae_sqr(ae_fabs(e->ptr.p_double[m], _state), _state);
                        if( ae_fp_less_eq(tst,eps2*ae_fabs(d->ptr.p_double[m], _state)*ae_fabs(d->ptr.p_double[m+1], _state)+safmin) )
                        {
                            gotoflag = ae_true;
                            break;
                        }
                    }
                }
                if( !gotoflag )
                {
                    m = lend;
                }
                if( m<lend )
                {
                    e->ptr.p_double[m] = (double)(0);
                }
                p = d->ptr.p_double[l];
                if( m!=l )
                {
                    
                    /*
                     * If remaining matrix is 2-by-2, use DLAE2 or SLAEV2
                     * to compute its eigensystem.
                     */
                    if( m==l+1 )
                    {
                        if( zneeded>0 )
                        {
                            evd_tdevdev2(d->ptr.p_double[l], e->ptr.p_double[l], d->ptr.p_double[l+1], &rt1, &rt2, &c, &s, _state);
                            work1.ptr.p_double[l] = c;
                            work2.ptr.p_double[l] = s;
                            workc.ptr.p_double[1] = work1.ptr.p_double[l];
                            works.ptr.p_double[1] = work2.ptr.p_double[l];
                            if( !wastranspose )
                            {
                                applyrotationsfromtheright(ae_false, 1, zrows, l, l+1, &workc, &works, z, &wtemp, _state);
                            }
                            else
                            {
                                applyrotationsfromtheleft(ae_false, l, l+1, 1, zrows, &workc, &works, z, &wtemp, _state);
                            }
                        }
                        else
                        {
                            evd_tdevde2(d->ptr.p_double[l], e->ptr.p_double[l], d->ptr.p_double[l+1], &rt1, &rt2, _state);
                        }
                        d->ptr.p_double[l] = rt1;
                        d->ptr.p_double[l+1] = rt2;
                        e->ptr.p_double[l] = (double)(0);
                        l = l+2;
                        if( l<=lend )
                        {
                            continue;
                        }
                        
                        /*
                         * GOTO 140
                         */
                        break;
                    }
                    if( jtot==nmaxit )
                    {
                        
                        /*
                         * GOTO 140
                         */
                        break;
                    }
                    jtot = jtot+1;
                    
                    /*
                     * Form shift.
                     */
                    g = (d->ptr.p_double[l+1]-p)/(2*e->ptr.p_double[l]);
                    r = evd_tdevdpythag(g, (double)(1), _state);
                    g = d->ptr.p_double[m]-p+e->ptr.p_double[l]/(g+evd_tdevdextsign(r, g, _state));
                    s = (double)(1);
                    c = (double)(1);
                    p = (double)(0);
                    
                    /*
                     * Inner loop
                     */
                    mm1 = m-1;
                    for(i=mm1; i>=l; i--)
                    {
                        f = s*e->ptr.p_double[i];
                        b = c*e->ptr.p_double[i];
                        generaterotation(g, f, &c, &s, &r, _state);
                        if( i!=m-1 )
                        {
                            e->ptr.p_double[i+1] = r;
                        }
                        g = d->ptr.p_double[i+1]-p;
                        r = (d->ptr.p_double[i]-g)*s+2*c*b;
                        p = s*r;
                        d->ptr.p_double[i+1] = g+p;
                        g = c*r-b;
                        
                        /*
                         * If eigenvectors are desired, then save rotations.
                         */
                        if( zneeded>0 )
                        {
                            work1.ptr.p_double[i] = c;
                            work2.ptr.p_double[i] = -s;
                        }
                    }
                    
                    /*
                     * If eigenvectors are desired, then apply saved rotations.
                     */
                    if( zneeded>0 )
                    {
                        for(i=l; i<=m-1; i++)
                        {
                            workc.ptr.p_double[i-l+1] = work1.ptr.p_double[i];
                            works.ptr.p_double[i-l+1] = work2.ptr.p_double[i];
                        }
                        if( !wastranspose )
                        {
                            applyrotationsfromtheright(ae_false, 1, zrows, l, m, &workc, &works, z, &wtemp, _state);
                        }
                        else
                        {
                            applyrotationsfromtheleft(ae_false, l, m, 1, zrows, &workc, &works, z, &wtemp, _state);
                        }
                    }
                    d->ptr.p_double[l] = d->ptr.p_double[l]-p;
                    e->ptr.p_double[l] = g;
                    continue;
                }
                
                /*
                 * Eigenvalue found.
                 */
                d->ptr.p_double[l] = p;
                l = l+1;
                if( l<=lend )
                {
                    continue;
                }
                break;
            }
        }
        else
        {
            
            /*
             * QR Iteration
             *
             * Look for small superdiagonal element.
             */
            for(;;)
            {
                gotoflag = ae_false;
                if( l!=lend )
                {
                    lendp1 = lend+1;
                    for(m=l; m>=lendp1; m--)
                    {
                        tst = ae_sqr(ae_fabs(e->ptr.p_double[m-1], _state), _state);
                        if( ae_fp_less_eq(tst,eps2*ae_fabs(d->ptr.p_double[m], _state)*ae_fabs(d->ptr.p_double[m-1], _state)+safmin) )
                        {
                            gotoflag = ae_true;
                            break;
                        }
                    }
                }
                if( !gotoflag )
                {
                    m = lend;
                }
                if( m>lend )
                {
                    e->ptr.p_double[m-1] = (double)(0);
                }
                p = d->ptr.p_double[l];
                if( m!=l )
                {
                    
                    /*
                     * If remaining matrix is 2-by-2, use DLAE2 or SLAEV2
                     * to compute its eigensystem.
                     */
                    if( m==l-1 )
                    {
                        if( zneeded>0 )
                        {
                            evd_tdevdev2(d->ptr.p_double[l-1], e->ptr.p_double[l-1], d->ptr.p_double[l], &rt1, &rt2, &c, &s, _state);
                            work1.ptr.p_double[m] = c;
                            work2.ptr.p_double[m] = s;
                            workc.ptr.p_double[1] = c;
                            works.ptr.p_double[1] = s;
                            if( !wastranspose )
                            {
                                applyrotationsfromtheright(ae_true, 1, zrows, l-1, l, &workc, &works, z, &wtemp, _state);
                            }
                            else
                            {
                                applyrotationsfromtheleft(ae_true, l-1, l, 1, zrows, &workc, &works, z, &wtemp, _state);
                            }
                        }
                        else
                        {
                            evd_tdevde2(d->ptr.p_double[l-1], e->ptr.p_double[l-1], d->ptr.p_double[l], &rt1, &rt2, _state);
                        }
                        d->ptr.p_double[l-1] = rt1;
                        d->ptr.p_double[l] = rt2;
                        e->ptr.p_double[l-1] = (double)(0);
                        l = l-2;
                        if( l>=lend )
                        {
                            continue;
                        }
                        break;
                    }
                    if( jtot==nmaxit )
                    {
                        break;
                    }
                    jtot = jtot+1;
                    
                    /*
                     * Form shift.
                     */
                    g = (d->ptr.p_double[l-1]-p)/(2*e->ptr.p_double[l-1]);
                    r = evd_tdevdpythag(g, (double)(1), _state);
                    g = d->ptr.p_double[m]-p+e->ptr.p_double[l-1]/(g+evd_tdevdextsign(r, g, _state));
                    s = (double)(1);
                    c = (double)(1);
                    p = (double)(0);
                    
                    /*
                     * Inner loop
                     */
                    lm1 = l-1;
                    for(i=m; i<=lm1; i++)
                    {
                        f = s*e->ptr.p_double[i];
                        b = c*e->ptr.p_double[i];
                        generaterotation(g, f, &c, &s, &r, _state);
                        if( i!=m )
                        {
                            e->ptr.p_double[i-1] = r;
                        }
                        g = d->ptr.p_double[i]-p;
                        r = (d->ptr.p_double[i+1]-g)*s+2*c*b;
                        p = s*r;
                        d->ptr.p_double[i] = g+p;
                        g = c*r-b;
                        
                        /*
                         * If eigenvectors are desired, then save rotations.
                         */
                        if( zneeded>0 )
                        {
                            work1.ptr.p_double[i] = c;
                            work2.ptr.p_double[i] = s;
                        }
                    }
                    
                    /*
                     * If eigenvectors are desired, then apply saved rotations.
                     */
                    if( zneeded>0 )
                    {
                        for(i=m; i<=l-1; i++)
                        {
                            workc.ptr.p_double[i-m+1] = work1.ptr.p_double[i];
                            works.ptr.p_double[i-m+1] = work2.ptr.p_double[i];
                        }
                        if( !wastranspose )
                        {
                            applyrotationsfromtheright(ae_true, 1, zrows, m, l, &workc, &works, z, &wtemp, _state);
                        }
                        else
                        {
                            applyrotationsfromtheleft(ae_true, m, l, 1, zrows, &workc, &works, z, &wtemp, _state);
                        }
                    }
                    d->ptr.p_double[l] = d->ptr.p_double[l]-p;
                    e->ptr.p_double[lm1] = g;
                    continue;
                }
                
                /*
                 * Eigenvalue found.
                 */
                d->ptr.p_double[l] = p;
                l = l-1;
                if( l>=lend )
                {
                    continue;
                }
                break;
            }
        }
        
        /*
         * Undo scaling if necessary
         */
        if( iscale==1 )
        {
            tmp = anorm/ssfmax;
            tmpint = lendsv-1;
            ae_v_muld(&d->ptr.p_double[lsv], 1, ae_v_len(lsv,lendsv), tmp);
            ae_v_muld(&e->ptr.p_double[lsv], 1, ae_v_len(lsv,tmpint), tmp);
        }
        if( iscale==2 )
        {
            tmp = anorm/ssfmin;
            tmpint = lendsv-1;
            ae_v_muld(&d->ptr.p_double[lsv], 1, ae_v_len(lsv,lendsv), tmp);
            ae_v_muld(&e->ptr.p_double[lsv], 1, ae_v_len(lsv,tmpint), tmp);
        }
        
        /*
         * Check for no convergence to an eigenvalue after a total
         * of N*MAXIT iterations.
         */
        if( jtot>=nmaxit )
        {
            result = ae_false;
            if( wastranspose )
            {
                inplacetranspose(z, 1, n, 1, n, &wtemp, _state);
            }
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * Order eigenvalues and eigenvectors.
     */
    if( zneeded==0 )
    {
        
        /*
         * Sort
         */
        if( n==1 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( n==2 )
        {
            if( ae_fp_greater(d->ptr.p_double[1],d->ptr.p_double[2]) )
            {
                tmp = d->ptr.p_double[1];
                d->ptr.p_double[1] = d->ptr.p_double[2];
                d->ptr.p_double[2] = tmp;
            }
            ae_frame_leave(_state);
            return result;
        }
        i = 2;
        do
        {
            t = i;
            while(t!=1)
            {
                k = t/2;
                if( ae_fp_greater_eq(d->ptr.p_double[k],d->ptr.p_double[t]) )
                {
                    t = 1;
                }
                else
                {
                    tmp = d->ptr.p_double[k];
                    d->ptr.p_double[k] = d->ptr.p_double[t];
                    d->ptr.p_double[t] = tmp;
                    t = k;
                }
            }
            i = i+1;
        }
        while(i<=n);
        i = n-1;
        do
        {
            tmp = d->ptr.p_double[i+1];
            d->ptr.p_double[i+1] = d->ptr.p_double[1];
            d->ptr.p_double[1] = tmp;
            t = 1;
            while(t!=0)
            {
                k = 2*t;
                if( k>i )
                {
                    t = 0;
                }
                else
                {
                    if( k<i )
                    {
                        if( ae_fp_greater(d->ptr.p_double[k+1],d->ptr.p_double[k]) )
                        {
                            k = k+1;
                        }
                    }
                    if( ae_fp_greater_eq(d->ptr.p_double[t],d->ptr.p_double[k]) )
                    {
                        t = 0;
                    }
                    else
                    {
                        tmp = d->ptr.p_double[k];
                        d->ptr.p_double[k] = d->ptr.p_double[t];
                        d->ptr.p_double[t] = tmp;
                        t = k;
                    }
                }
            }
            i = i-1;
        }
        while(i>=1);
    }
    else
    {
        
        /*
         * Use Selection Sort to minimize swaps of eigenvectors
         */
        for(ii=2; ii<=n; ii++)
        {
            i = ii-1;
            k = i;
            p = d->ptr.p_double[i];
            for(j=ii; j<=n; j++)
            {
                if( ae_fp_less(d->ptr.p_double[j],p) )
                {
                    k = j;
                    p = d->ptr.p_double[j];
                }
            }
            if( k!=i )
            {
                d->ptr.p_double[k] = d->ptr.p_double[i];
                d->ptr.p_double[i] = p;
                if( wastranspose )
                {
                    ae_v_move(&wtemp.ptr.p_double[1], 1, &z->ptr.pp_double[i][1], 1, ae_v_len(1,n));
                    ae_v_move(&z->ptr.pp_double[i][1], 1, &z->ptr.pp_double[k][1], 1, ae_v_len(1,n));
                    ae_v_move(&z->ptr.pp_double[k][1], 1, &wtemp.ptr.p_double[1], 1, ae_v_len(1,n));
                }
                else
                {
                    ae_v_move(&wtemp.ptr.p_double[1], 1, &z->ptr.pp_double[1][i], z->stride, ae_v_len(1,zrows));
                    ae_v_move(&z->ptr.pp_double[1][i], z->stride, &z->ptr.pp_double[1][k], z->stride, ae_v_len(1,zrows));
                    ae_v_move(&z->ptr.pp_double[1][k], z->stride, &wtemp.ptr.p_double[1], 1, ae_v_len(1,zrows));
                }
            }
        }
        if( wastranspose )
        {
            inplacetranspose(z, 1, n, 1, n, &wtemp, _state);
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
DLAE2  computes the eigenvalues of a 2-by-2 symmetric matrix
   [  A   B  ]
   [  B   C  ].
On return, RT1 is the eigenvalue of larger absolute value, and RT2
is the eigenvalue of smaller absolute value.

  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     October 31, 1992
*************************************************************************/
static void evd_tdevde2(double a,
     double b,
     double c,
     double* rt1,
     double* rt2,
     ae_state *_state)
{
    double ab;
    double acmn;
    double acmx;
    double adf;
    double df;
    double rt;
    double sm;
    double tb;

    *rt1 = 0;
    *rt2 = 0;

    sm = a+c;
    df = a-c;
    adf = ae_fabs(df, _state);
    tb = b+b;
    ab = ae_fabs(tb, _state);
    if( ae_fp_greater(ae_fabs(a, _state),ae_fabs(c, _state)) )
    {
        acmx = a;
        acmn = c;
    }
    else
    {
        acmx = c;
        acmn = a;
    }
    if( ae_fp_greater(adf,ab) )
    {
        rt = adf*ae_sqrt(1+ae_sqr(ab/adf, _state), _state);
    }
    else
    {
        if( ae_fp_less(adf,ab) )
        {
            rt = ab*ae_sqrt(1+ae_sqr(adf/ab, _state), _state);
        }
        else
        {
            
            /*
             * Includes case AB=ADF=0
             */
            rt = ab*ae_sqrt((double)(2), _state);
        }
    }
    if( ae_fp_less(sm,(double)(0)) )
    {
        *rt1 = 0.5*(sm-rt);
        
        /*
         * Order of execution important.
         * To get fully accurate smaller eigenvalue,
         * next line needs to be executed in higher precision.
         */
        *rt2 = acmx/(*rt1)*acmn-b/(*rt1)*b;
    }
    else
    {
        if( ae_fp_greater(sm,(double)(0)) )
        {
            *rt1 = 0.5*(sm+rt);
            
            /*
             * Order of execution important.
             * To get fully accurate smaller eigenvalue,
             * next line needs to be executed in higher precision.
             */
            *rt2 = acmx/(*rt1)*acmn-b/(*rt1)*b;
        }
        else
        {
            
            /*
             * Includes case RT1 = RT2 = 0
             */
            *rt1 = 0.5*rt;
            *rt2 = -0.5*rt;
        }
    }
}


/*************************************************************************
DLAEV2 computes the eigendecomposition of a 2-by-2 symmetric matrix

   [  A   B  ]
   [  B   C  ].

On return, RT1 is the eigenvalue of larger absolute value, RT2 is the
eigenvalue of smaller absolute value, and (CS1,SN1) is the unit right
eigenvector for RT1, giving the decomposition

   [ CS1  SN1 ] [  A   B  ] [ CS1 -SN1 ]  =  [ RT1  0  ]
   [-SN1  CS1 ] [  B   C  ] [ SN1  CS1 ]     [  0  RT2 ].


  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     October 31, 1992
*************************************************************************/
static void evd_tdevdev2(double a,
     double b,
     double c,
     double* rt1,
     double* rt2,
     double* cs1,
     double* sn1,
     ae_state *_state)
{
    ae_int_t sgn1;
    ae_int_t sgn2;
    double ab;
    double acmn;
    double acmx;
    double acs;
    double adf;
    double cs;
    double ct;
    double df;
    double rt;
    double sm;
    double tb;
    double tn;

    *rt1 = 0;
    *rt2 = 0;
    *cs1 = 0;
    *sn1 = 0;

    
    /*
     * Compute the eigenvalues
     */
    sm = a+c;
    df = a-c;
    adf = ae_fabs(df, _state);
    tb = b+b;
    ab = ae_fabs(tb, _state);
    if( ae_fp_greater(ae_fabs(a, _state),ae_fabs(c, _state)) )
    {
        acmx = a;
        acmn = c;
    }
    else
    {
        acmx = c;
        acmn = a;
    }
    if( ae_fp_greater(adf,ab) )
    {
        rt = adf*ae_sqrt(1+ae_sqr(ab/adf, _state), _state);
    }
    else
    {
        if( ae_fp_less(adf,ab) )
        {
            rt = ab*ae_sqrt(1+ae_sqr(adf/ab, _state), _state);
        }
        else
        {
            
            /*
             * Includes case AB=ADF=0
             */
            rt = ab*ae_sqrt((double)(2), _state);
        }
    }
    if( ae_fp_less(sm,(double)(0)) )
    {
        *rt1 = 0.5*(sm-rt);
        sgn1 = -1;
        
        /*
         * Order of execution important.
         * To get fully accurate smaller eigenvalue,
         * next line needs to be executed in higher precision.
         */
        *rt2 = acmx/(*rt1)*acmn-b/(*rt1)*b;
    }
    else
    {
        if( ae_fp_greater(sm,(double)(0)) )
        {
            *rt1 = 0.5*(sm+rt);
            sgn1 = 1;
            
            /*
             * Order of execution important.
             * To get fully accurate smaller eigenvalue,
             * next line needs to be executed in higher precision.
             */
            *rt2 = acmx/(*rt1)*acmn-b/(*rt1)*b;
        }
        else
        {
            
            /*
             * Includes case RT1 = RT2 = 0
             */
            *rt1 = 0.5*rt;
            *rt2 = -0.5*rt;
            sgn1 = 1;
        }
    }
    
    /*
     * Compute the eigenvector
     */
    if( ae_fp_greater_eq(df,(double)(0)) )
    {
        cs = df+rt;
        sgn2 = 1;
    }
    else
    {
        cs = df-rt;
        sgn2 = -1;
    }
    acs = ae_fabs(cs, _state);
    if( ae_fp_greater(acs,ab) )
    {
        ct = -tb/cs;
        *sn1 = 1/ae_sqrt(1+ct*ct, _state);
        *cs1 = ct*(*sn1);
    }
    else
    {
        if( ae_fp_eq(ab,(double)(0)) )
        {
            *cs1 = (double)(1);
            *sn1 = (double)(0);
        }
        else
        {
            tn = -cs/tb;
            *cs1 = 1/ae_sqrt(1+tn*tn, _state);
            *sn1 = tn*(*cs1);
        }
    }
    if( sgn1==sgn2 )
    {
        tn = *cs1;
        *cs1 = -*sn1;
        *sn1 = tn;
    }
}


/*************************************************************************
Internal routine
*************************************************************************/
static double evd_tdevdpythag(double a, double b, ae_state *_state)
{
    double result;


    if( ae_fp_less(ae_fabs(a, _state),ae_fabs(b, _state)) )
    {
        result = ae_fabs(b, _state)*ae_sqrt(1+ae_sqr(a/b, _state), _state);
    }
    else
    {
        result = ae_fabs(a, _state)*ae_sqrt(1+ae_sqr(b/a, _state), _state);
    }
    return result;
}


/*************************************************************************
Internal routine
*************************************************************************/
static double evd_tdevdextsign(double a, double b, ae_state *_state)
{
    double result;


    if( ae_fp_greater_eq(b,(double)(0)) )
    {
        result = ae_fabs(a, _state);
    }
    else
    {
        result = -ae_fabs(a, _state);
    }
    return result;
}


static ae_bool evd_internalbisectioneigenvalues(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_int_t irange,
     ae_int_t iorder,
     double vl,
     double vu,
     ae_int_t il,
     ae_int_t iu,
     double abstol,
     /* Real    */ ae_vector* w,
     ae_int_t* m,
     ae_int_t* nsplit,
     /* Integer */ ae_vector* iblock,
     /* Integer */ ae_vector* isplit,
     ae_int_t* errorcode,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _d;
    ae_vector _e;
    double fudge;
    double relfac;
    ae_bool ncnvrg;
    ae_bool toofew;
    ae_int_t ib;
    ae_int_t ibegin;
    ae_int_t idiscl;
    ae_int_t idiscu;
    ae_int_t ie;
    ae_int_t iend;
    ae_int_t iinfo;
    ae_int_t im;
    ae_int_t iin;
    ae_int_t ioff;
    ae_int_t iout;
    ae_int_t itmax;
    ae_int_t iw;
    ae_int_t iwoff;
    ae_int_t j;
    ae_int_t itmp1;
    ae_int_t jb;
    ae_int_t jdisc;
    ae_int_t je;
    ae_int_t nwl;
    ae_int_t nwu;
    double atoli;
    double bnorm;
    double gl;
    double gu;
    double pivmin;
    double rtoli;
    double safemn;
    double tmp1;
    double tmp2;
    double tnorm;
    double ulp;
    double wkill;
    double wl;
    double wlu;
    double wu;
    double wul;
    double scalefactor;
    double t;
    ae_vector idumma;
    ae_vector work;
    ae_vector iwork;
    ae_vector ia1s2;
    ae_vector ra1s2;
    ae_matrix ra1s2x2;
    ae_matrix ia1s2x2;
    ae_vector ra1siin;
    ae_vector ra2siin;
    ae_vector ra3siin;
    ae_vector ra4siin;
    ae_matrix ra1siinx2;
    ae_matrix ia1siinx2;
    ae_vector iworkspace;
    ae_vector rworkspace;
    ae_int_t tmpi;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_d, d, _state);
    d = &_d;
    ae_vector_init_copy(&_e, e, _state);
    e = &_e;
    ae_vector_clear(w);
    *m = 0;
    *nsplit = 0;
    ae_vector_clear(iblock);
    ae_vector_clear(isplit);
    *errorcode = 0;
    ae_vector_init(&idumma, 0, DT_INT, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_vector_init(&iwork, 0, DT_INT, _state);
    ae_vector_init(&ia1s2, 0, DT_INT, _state);
    ae_vector_init(&ra1s2, 0, DT_REAL, _state);
    ae_matrix_init(&ra1s2x2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ia1s2x2, 0, 0, DT_INT, _state);
    ae_vector_init(&ra1siin, 0, DT_REAL, _state);
    ae_vector_init(&ra2siin, 0, DT_REAL, _state);
    ae_vector_init(&ra3siin, 0, DT_REAL, _state);
    ae_vector_init(&ra4siin, 0, DT_REAL, _state);
    ae_matrix_init(&ra1siinx2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ia1siinx2, 0, 0, DT_INT, _state);
    ae_vector_init(&iworkspace, 0, DT_INT, _state);
    ae_vector_init(&rworkspace, 0, DT_REAL, _state);

    
    /*
     * Quick return if possible
     */
    *m = 0;
    if( n==0 )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Get machine constants
     * NB is the minimum vector length for vector bisection, or 0
     * if only scalar is to be done.
     */
    fudge = (double)(2);
    relfac = (double)(2);
    safemn = ae_minrealnumber;
    ulp = 2*ae_machineepsilon;
    rtoli = ulp*relfac;
    ae_vector_set_length(&idumma, 1+1, _state);
    ae_vector_set_length(&work, 4*n+1, _state);
    ae_vector_set_length(&iwork, 3*n+1, _state);
    ae_vector_set_length(w, n+1, _state);
    ae_vector_set_length(iblock, n+1, _state);
    ae_vector_set_length(isplit, n+1, _state);
    ae_vector_set_length(&ia1s2, 2+1, _state);
    ae_vector_set_length(&ra1s2, 2+1, _state);
    ae_matrix_set_length(&ra1s2x2, 2+1, 2+1, _state);
    ae_matrix_set_length(&ia1s2x2, 2+1, 2+1, _state);
    ae_vector_set_length(&ra1siin, n+1, _state);
    ae_vector_set_length(&ra2siin, n+1, _state);
    ae_vector_set_length(&ra3siin, n+1, _state);
    ae_vector_set_length(&ra4siin, n+1, _state);
    ae_matrix_set_length(&ra1siinx2, n+1, 2+1, _state);
    ae_matrix_set_length(&ia1siinx2, n+1, 2+1, _state);
    ae_vector_set_length(&iworkspace, n+1, _state);
    ae_vector_set_length(&rworkspace, n+1, _state);
    
    /*
     * these initializers are not really necessary,
     * but without them compiler complains about uninitialized locals
     */
    wlu = (double)(0);
    wul = (double)(0);
    
    /*
     * Check for Errors
     */
    result = ae_false;
    *errorcode = 0;
    if( irange<=0||irange>=4 )
    {
        *errorcode = -4;
    }
    if( iorder<=0||iorder>=3 )
    {
        *errorcode = -5;
    }
    if( n<0 )
    {
        *errorcode = -3;
    }
    if( irange==2&&ae_fp_greater_eq(vl,vu) )
    {
        *errorcode = -6;
    }
    if( irange==3&&(il<1||il>ae_maxint(1, n, _state)) )
    {
        *errorcode = -8;
    }
    if( irange==3&&(iu<ae_minint(n, il, _state)||iu>n) )
    {
        *errorcode = -9;
    }
    if( *errorcode!=0 )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Initialize error flags
     */
    ncnvrg = ae_false;
    toofew = ae_false;
    
    /*
     * Simplifications:
     */
    if( (irange==3&&il==1)&&iu==n )
    {
        irange = 1;
    }
    
    /*
     * Special Case when N=1
     */
    if( n==1 )
    {
        *nsplit = 1;
        isplit->ptr.p_int[1] = 1;
        if( irange==2&&(ae_fp_greater_eq(vl,d->ptr.p_double[1])||ae_fp_less(vu,d->ptr.p_double[1])) )
        {
            *m = 0;
        }
        else
        {
            w->ptr.p_double[1] = d->ptr.p_double[1];
            iblock->ptr.p_int[1] = 1;
            *m = 1;
        }
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Scaling
     */
    t = ae_fabs(d->ptr.p_double[n], _state);
    for(j=1; j<=n-1; j++)
    {
        t = ae_maxreal(t, ae_fabs(d->ptr.p_double[j], _state), _state);
        t = ae_maxreal(t, ae_fabs(e->ptr.p_double[j], _state), _state);
    }
    scalefactor = (double)(1);
    if( ae_fp_neq(t,(double)(0)) )
    {
        if( ae_fp_greater(t,ae_sqrt(ae_sqrt(ae_minrealnumber, _state), _state)*ae_sqrt(ae_maxrealnumber, _state)) )
        {
            scalefactor = t;
        }
        if( ae_fp_less(t,ae_sqrt(ae_sqrt(ae_maxrealnumber, _state), _state)*ae_sqrt(ae_minrealnumber, _state)) )
        {
            scalefactor = t;
        }
        for(j=1; j<=n-1; j++)
        {
            d->ptr.p_double[j] = d->ptr.p_double[j]/scalefactor;
            e->ptr.p_double[j] = e->ptr.p_double[j]/scalefactor;
        }
        d->ptr.p_double[n] = d->ptr.p_double[n]/scalefactor;
    }
    
    /*
     * Compute Splitting Points
     */
    *nsplit = 1;
    work.ptr.p_double[n] = (double)(0);
    pivmin = (double)(1);
    for(j=2; j<=n; j++)
    {
        tmp1 = ae_sqr(e->ptr.p_double[j-1], _state);
        if( ae_fp_greater(ae_fabs(d->ptr.p_double[j]*d->ptr.p_double[j-1], _state)*ae_sqr(ulp, _state)+safemn,tmp1) )
        {
            isplit->ptr.p_int[*nsplit] = j-1;
            *nsplit = *nsplit+1;
            work.ptr.p_double[j-1] = (double)(0);
        }
        else
        {
            work.ptr.p_double[j-1] = tmp1;
            pivmin = ae_maxreal(pivmin, tmp1, _state);
        }
    }
    isplit->ptr.p_int[*nsplit] = n;
    pivmin = pivmin*safemn;
    
    /*
     * Compute Interval and ATOLI
     */
    if( irange==3 )
    {
        
        /*
         * RANGE='I': Compute the interval containing eigenvalues
         *     IL through IU.
         *
         * Compute Gershgorin interval for entire (split) matrix
         * and use it as the initial interval
         */
        gu = d->ptr.p_double[1];
        gl = d->ptr.p_double[1];
        tmp1 = (double)(0);
        for(j=1; j<=n-1; j++)
        {
            tmp2 = ae_sqrt(work.ptr.p_double[j], _state);
            gu = ae_maxreal(gu, d->ptr.p_double[j]+tmp1+tmp2, _state);
            gl = ae_minreal(gl, d->ptr.p_double[j]-tmp1-tmp2, _state);
            tmp1 = tmp2;
        }
        gu = ae_maxreal(gu, d->ptr.p_double[n]+tmp1, _state);
        gl = ae_minreal(gl, d->ptr.p_double[n]-tmp1, _state);
        tnorm = ae_maxreal(ae_fabs(gl, _state), ae_fabs(gu, _state), _state);
        gl = gl-fudge*tnorm*ulp*n-fudge*2*pivmin;
        gu = gu+fudge*tnorm*ulp*n+fudge*pivmin;
        
        /*
         * Compute Iteration parameters
         */
        itmax = ae_iceil((ae_log(tnorm+pivmin, _state)-ae_log(pivmin, _state))/ae_log((double)(2), _state), _state)+2;
        if( ae_fp_less_eq(abstol,(double)(0)) )
        {
            atoli = ulp*tnorm;
        }
        else
        {
            atoli = abstol;
        }
        work.ptr.p_double[n+1] = gl;
        work.ptr.p_double[n+2] = gl;
        work.ptr.p_double[n+3] = gu;
        work.ptr.p_double[n+4] = gu;
        work.ptr.p_double[n+5] = gl;
        work.ptr.p_double[n+6] = gu;
        iwork.ptr.p_int[1] = -1;
        iwork.ptr.p_int[2] = -1;
        iwork.ptr.p_int[3] = n+1;
        iwork.ptr.p_int[4] = n+1;
        iwork.ptr.p_int[5] = il-1;
        iwork.ptr.p_int[6] = iu;
        
        /*
         * Calling DLAEBZ
         *
         * DLAEBZ( 3, ITMAX, N, 2, 2, NB, ATOLI, RTOLI, PIVMIN, D, E,
         *    WORK, IWORK( 5 ), WORK( N+1 ), WORK( N+5 ), IOUT,
         *    IWORK, W, IBLOCK, IINFO )
         */
        ia1s2.ptr.p_int[1] = iwork.ptr.p_int[5];
        ia1s2.ptr.p_int[2] = iwork.ptr.p_int[6];
        ra1s2.ptr.p_double[1] = work.ptr.p_double[n+5];
        ra1s2.ptr.p_double[2] = work.ptr.p_double[n+6];
        ra1s2x2.ptr.pp_double[1][1] = work.ptr.p_double[n+1];
        ra1s2x2.ptr.pp_double[2][1] = work.ptr.p_double[n+2];
        ra1s2x2.ptr.pp_double[1][2] = work.ptr.p_double[n+3];
        ra1s2x2.ptr.pp_double[2][2] = work.ptr.p_double[n+4];
        ia1s2x2.ptr.pp_int[1][1] = iwork.ptr.p_int[1];
        ia1s2x2.ptr.pp_int[2][1] = iwork.ptr.p_int[2];
        ia1s2x2.ptr.pp_int[1][2] = iwork.ptr.p_int[3];
        ia1s2x2.ptr.pp_int[2][2] = iwork.ptr.p_int[4];
        evd_internaldlaebz(3, itmax, n, 2, 2, atoli, rtoli, pivmin, d, e, &work, &ia1s2, &ra1s2x2, &ra1s2, &iout, &ia1s2x2, w, iblock, &iinfo, _state);
        iwork.ptr.p_int[5] = ia1s2.ptr.p_int[1];
        iwork.ptr.p_int[6] = ia1s2.ptr.p_int[2];
        work.ptr.p_double[n+5] = ra1s2.ptr.p_double[1];
        work.ptr.p_double[n+6] = ra1s2.ptr.p_double[2];
        work.ptr.p_double[n+1] = ra1s2x2.ptr.pp_double[1][1];
        work.ptr.p_double[n+2] = ra1s2x2.ptr.pp_double[2][1];
        work.ptr.p_double[n+3] = ra1s2x2.ptr.pp_double[1][2];
        work.ptr.p_double[n+4] = ra1s2x2.ptr.pp_double[2][2];
        iwork.ptr.p_int[1] = ia1s2x2.ptr.pp_int[1][1];
        iwork.ptr.p_int[2] = ia1s2x2.ptr.pp_int[2][1];
        iwork.ptr.p_int[3] = ia1s2x2.ptr.pp_int[1][2];
        iwork.ptr.p_int[4] = ia1s2x2.ptr.pp_int[2][2];
        if( iwork.ptr.p_int[6]==iu )
        {
            wl = work.ptr.p_double[n+1];
            wlu = work.ptr.p_double[n+3];
            nwl = iwork.ptr.p_int[1];
            wu = work.ptr.p_double[n+4];
            wul = work.ptr.p_double[n+2];
            nwu = iwork.ptr.p_int[4];
        }
        else
        {
            wl = work.ptr.p_double[n+2];
            wlu = work.ptr.p_double[n+4];
            nwl = iwork.ptr.p_int[2];
            wu = work.ptr.p_double[n+3];
            wul = work.ptr.p_double[n+1];
            nwu = iwork.ptr.p_int[3];
        }
        if( ((nwl<0||nwl>=n)||nwu<1)||nwu>n )
        {
            *errorcode = 4;
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
    }
    else
    {
        
        /*
         * RANGE='A' or 'V' -- Set ATOLI
         */
        tnorm = ae_maxreal(ae_fabs(d->ptr.p_double[1], _state)+ae_fabs(e->ptr.p_double[1], _state), ae_fabs(d->ptr.p_double[n], _state)+ae_fabs(e->ptr.p_double[n-1], _state), _state);
        for(j=2; j<=n-1; j++)
        {
            tnorm = ae_maxreal(tnorm, ae_fabs(d->ptr.p_double[j], _state)+ae_fabs(e->ptr.p_double[j-1], _state)+ae_fabs(e->ptr.p_double[j], _state), _state);
        }
        if( ae_fp_less_eq(abstol,(double)(0)) )
        {
            atoli = ulp*tnorm;
        }
        else
        {
            atoli = abstol;
        }
        if( irange==2 )
        {
            wl = vl;
            wu = vu;
        }
        else
        {
            wl = (double)(0);
            wu = (double)(0);
        }
    }
    
    /*
     * Find Eigenvalues -- Loop Over Blocks and recompute NWL and NWU.
     * NWL accumulates the number of eigenvalues .le. WL,
     * NWU accumulates the number of eigenvalues .le. WU
     */
    *m = 0;
    iend = 0;
    *errorcode = 0;
    nwl = 0;
    nwu = 0;
    for(jb=1; jb<=*nsplit; jb++)
    {
        ioff = iend;
        ibegin = ioff+1;
        iend = isplit->ptr.p_int[jb];
        iin = iend-ioff;
        if( iin==1 )
        {
            
            /*
             * Special Case -- IIN=1
             */
            if( irange==1||ae_fp_greater_eq(wl,d->ptr.p_double[ibegin]-pivmin) )
            {
                nwl = nwl+1;
            }
            if( irange==1||ae_fp_greater_eq(wu,d->ptr.p_double[ibegin]-pivmin) )
            {
                nwu = nwu+1;
            }
            if( irange==1||(ae_fp_less(wl,d->ptr.p_double[ibegin]-pivmin)&&ae_fp_greater_eq(wu,d->ptr.p_double[ibegin]-pivmin)) )
            {
                *m = *m+1;
                w->ptr.p_double[*m] = d->ptr.p_double[ibegin];
                iblock->ptr.p_int[*m] = jb;
            }
        }
        else
        {
            
            /*
             * General Case -- IIN > 1
             *
             * Compute Gershgorin Interval
             * and use it as the initial interval
             */
            gu = d->ptr.p_double[ibegin];
            gl = d->ptr.p_double[ibegin];
            tmp1 = (double)(0);
            for(j=ibegin; j<=iend-1; j++)
            {
                tmp2 = ae_fabs(e->ptr.p_double[j], _state);
                gu = ae_maxreal(gu, d->ptr.p_double[j]+tmp1+tmp2, _state);
                gl = ae_minreal(gl, d->ptr.p_double[j]-tmp1-tmp2, _state);
                tmp1 = tmp2;
            }
            gu = ae_maxreal(gu, d->ptr.p_double[iend]+tmp1, _state);
            gl = ae_minreal(gl, d->ptr.p_double[iend]-tmp1, _state);
            bnorm = ae_maxreal(ae_fabs(gl, _state), ae_fabs(gu, _state), _state);
            gl = gl-fudge*bnorm*ulp*iin-fudge*pivmin;
            gu = gu+fudge*bnorm*ulp*iin+fudge*pivmin;
            
            /*
             * Compute ATOLI for the current submatrix
             */
            if( ae_fp_less_eq(abstol,(double)(0)) )
            {
                atoli = ulp*ae_maxreal(ae_fabs(gl, _state), ae_fabs(gu, _state), _state);
            }
            else
            {
                atoli = abstol;
            }
            if( irange>1 )
            {
                if( ae_fp_less(gu,wl) )
                {
                    nwl = nwl+iin;
                    nwu = nwu+iin;
                    continue;
                }
                gl = ae_maxreal(gl, wl, _state);
                gu = ae_minreal(gu, wu, _state);
                if( ae_fp_greater_eq(gl,gu) )
                {
                    continue;
                }
            }
            
            /*
             * Set Up Initial Interval
             */
            work.ptr.p_double[n+1] = gl;
            work.ptr.p_double[n+iin+1] = gu;
            
            /*
             * Calling DLAEBZ
             *
             * CALL DLAEBZ( 1, 0, IN, IN, 1, NB, ATOLI, RTOLI, PIVMIN,
             *    D( IBEGIN ), E( IBEGIN ), WORK( IBEGIN ),
             *    IDUMMA, WORK( N+1 ), WORK( N+2*IN+1 ), IM,
             *    IWORK, W( M+1 ), IBLOCK( M+1 ), IINFO )
             */
            for(tmpi=1; tmpi<=iin; tmpi++)
            {
                ra1siin.ptr.p_double[tmpi] = d->ptr.p_double[ibegin-1+tmpi];
                if( ibegin-1+tmpi<n )
                {
                    ra2siin.ptr.p_double[tmpi] = e->ptr.p_double[ibegin-1+tmpi];
                }
                ra3siin.ptr.p_double[tmpi] = work.ptr.p_double[ibegin-1+tmpi];
                ra1siinx2.ptr.pp_double[tmpi][1] = work.ptr.p_double[n+tmpi];
                ra1siinx2.ptr.pp_double[tmpi][2] = work.ptr.p_double[n+tmpi+iin];
                ra4siin.ptr.p_double[tmpi] = work.ptr.p_double[n+2*iin+tmpi];
                rworkspace.ptr.p_double[tmpi] = w->ptr.p_double[*m+tmpi];
                iworkspace.ptr.p_int[tmpi] = iblock->ptr.p_int[*m+tmpi];
                ia1siinx2.ptr.pp_int[tmpi][1] = iwork.ptr.p_int[tmpi];
                ia1siinx2.ptr.pp_int[tmpi][2] = iwork.ptr.p_int[tmpi+iin];
            }
            evd_internaldlaebz(1, 0, iin, iin, 1, atoli, rtoli, pivmin, &ra1siin, &ra2siin, &ra3siin, &idumma, &ra1siinx2, &ra4siin, &im, &ia1siinx2, &rworkspace, &iworkspace, &iinfo, _state);
            for(tmpi=1; tmpi<=iin; tmpi++)
            {
                work.ptr.p_double[n+tmpi] = ra1siinx2.ptr.pp_double[tmpi][1];
                work.ptr.p_double[n+tmpi+iin] = ra1siinx2.ptr.pp_double[tmpi][2];
                work.ptr.p_double[n+2*iin+tmpi] = ra4siin.ptr.p_double[tmpi];
                w->ptr.p_double[*m+tmpi] = rworkspace.ptr.p_double[tmpi];
                iblock->ptr.p_int[*m+tmpi] = iworkspace.ptr.p_int[tmpi];
                iwork.ptr.p_int[tmpi] = ia1siinx2.ptr.pp_int[tmpi][1];
                iwork.ptr.p_int[tmpi+iin] = ia1siinx2.ptr.pp_int[tmpi][2];
            }
            nwl = nwl+iwork.ptr.p_int[1];
            nwu = nwu+iwork.ptr.p_int[iin+1];
            iwoff = *m-iwork.ptr.p_int[1];
            
            /*
             * Compute Eigenvalues
             */
            itmax = ae_iceil((ae_log(gu-gl+pivmin, _state)-ae_log(pivmin, _state))/ae_log((double)(2), _state), _state)+2;
            
            /*
             * Calling DLAEBZ
             *
             *CALL DLAEBZ( 2, ITMAX, IN, IN, 1, NB, ATOLI, RTOLI, PIVMIN,
             *    D( IBEGIN ), E( IBEGIN ), WORK( IBEGIN ),
             *    IDUMMA, WORK( N+1 ), WORK( N+2*IN+1 ), IOUT,
             *    IWORK, W( M+1 ), IBLOCK( M+1 ), IINFO )
             */
            for(tmpi=1; tmpi<=iin; tmpi++)
            {
                ra1siin.ptr.p_double[tmpi] = d->ptr.p_double[ibegin-1+tmpi];
                if( ibegin-1+tmpi<n )
                {
                    ra2siin.ptr.p_double[tmpi] = e->ptr.p_double[ibegin-1+tmpi];
                }
                ra3siin.ptr.p_double[tmpi] = work.ptr.p_double[ibegin-1+tmpi];
                ra1siinx2.ptr.pp_double[tmpi][1] = work.ptr.p_double[n+tmpi];
                ra1siinx2.ptr.pp_double[tmpi][2] = work.ptr.p_double[n+tmpi+iin];
                ra4siin.ptr.p_double[tmpi] = work.ptr.p_double[n+2*iin+tmpi];
                rworkspace.ptr.p_double[tmpi] = w->ptr.p_double[*m+tmpi];
                iworkspace.ptr.p_int[tmpi] = iblock->ptr.p_int[*m+tmpi];
                ia1siinx2.ptr.pp_int[tmpi][1] = iwork.ptr.p_int[tmpi];
                ia1siinx2.ptr.pp_int[tmpi][2] = iwork.ptr.p_int[tmpi+iin];
            }
            evd_internaldlaebz(2, itmax, iin, iin, 1, atoli, rtoli, pivmin, &ra1siin, &ra2siin, &ra3siin, &idumma, &ra1siinx2, &ra4siin, &iout, &ia1siinx2, &rworkspace, &iworkspace, &iinfo, _state);
            for(tmpi=1; tmpi<=iin; tmpi++)
            {
                work.ptr.p_double[n+tmpi] = ra1siinx2.ptr.pp_double[tmpi][1];
                work.ptr.p_double[n+tmpi+iin] = ra1siinx2.ptr.pp_double[tmpi][2];
                work.ptr.p_double[n+2*iin+tmpi] = ra4siin.ptr.p_double[tmpi];
                w->ptr.p_double[*m+tmpi] = rworkspace.ptr.p_double[tmpi];
                iblock->ptr.p_int[*m+tmpi] = iworkspace.ptr.p_int[tmpi];
                iwork.ptr.p_int[tmpi] = ia1siinx2.ptr.pp_int[tmpi][1];
                iwork.ptr.p_int[tmpi+iin] = ia1siinx2.ptr.pp_int[tmpi][2];
            }
            
            /*
             * Copy Eigenvalues Into W and IBLOCK
             * Use -JB for block number for unconverged eigenvalues.
             */
            for(j=1; j<=iout; j++)
            {
                tmp1 = 0.5*(work.ptr.p_double[j+n]+work.ptr.p_double[j+iin+n]);
                
                /*
                 * Flag non-convergence.
                 */
                if( j>iout-iinfo )
                {
                    ncnvrg = ae_true;
                    ib = -jb;
                }
                else
                {
                    ib = jb;
                }
                for(je=iwork.ptr.p_int[j]+1+iwoff; je<=iwork.ptr.p_int[j+iin]+iwoff; je++)
                {
                    w->ptr.p_double[je] = tmp1;
                    iblock->ptr.p_int[je] = ib;
                }
            }
            *m = *m+im;
        }
    }
    
    /*
     * If RANGE='I', then (WL,WU) contains eigenvalues NWL+1,...,NWU
     * If NWL+1 < IL or NWU > IU, discard extra eigenvalues.
     */
    if( irange==3 )
    {
        im = 0;
        idiscl = il-1-nwl;
        idiscu = nwu-iu;
        if( idiscl>0||idiscu>0 )
        {
            for(je=1; je<=*m; je++)
            {
                if( ae_fp_less_eq(w->ptr.p_double[je],wlu)&&idiscl>0 )
                {
                    idiscl = idiscl-1;
                }
                else
                {
                    if( ae_fp_greater_eq(w->ptr.p_double[je],wul)&&idiscu>0 )
                    {
                        idiscu = idiscu-1;
                    }
                    else
                    {
                        im = im+1;
                        w->ptr.p_double[im] = w->ptr.p_double[je];
                        iblock->ptr.p_int[im] = iblock->ptr.p_int[je];
                    }
                }
            }
            *m = im;
        }
        if( idiscl>0||idiscu>0 )
        {
            
            /*
             * Code to deal with effects of bad arithmetic:
             * Some low eigenvalues to be discarded are not in (WL,WLU],
             * or high eigenvalues to be discarded are not in (WUL,WU]
             * so just kill off the smallest IDISCL/largest IDISCU
             * eigenvalues, by simply finding the smallest/largest
             * eigenvalue(s).
             *
             * (If N(w) is monotone non-decreasing, this should never
             *  happen.)
             */
            if( idiscl>0 )
            {
                wkill = wu;
                for(jdisc=1; jdisc<=idiscl; jdisc++)
                {
                    iw = 0;
                    for(je=1; je<=*m; je++)
                    {
                        if( iblock->ptr.p_int[je]!=0&&(ae_fp_less(w->ptr.p_double[je],wkill)||iw==0) )
                        {
                            iw = je;
                            wkill = w->ptr.p_double[je];
                        }
                    }
                    iblock->ptr.p_int[iw] = 0;
                }
            }
            if( idiscu>0 )
            {
                wkill = wl;
                for(jdisc=1; jdisc<=idiscu; jdisc++)
                {
                    iw = 0;
                    for(je=1; je<=*m; je++)
                    {
                        if( iblock->ptr.p_int[je]!=0&&(ae_fp_greater(w->ptr.p_double[je],wkill)||iw==0) )
                        {
                            iw = je;
                            wkill = w->ptr.p_double[je];
                        }
                    }
                    iblock->ptr.p_int[iw] = 0;
                }
            }
            im = 0;
            for(je=1; je<=*m; je++)
            {
                if( iblock->ptr.p_int[je]!=0 )
                {
                    im = im+1;
                    w->ptr.p_double[im] = w->ptr.p_double[je];
                    iblock->ptr.p_int[im] = iblock->ptr.p_int[je];
                }
            }
            *m = im;
        }
        if( idiscl<0||idiscu<0 )
        {
            toofew = ae_true;
        }
    }
    
    /*
     * If ORDER='B', do nothing -- the eigenvalues are already sorted
     *    by block.
     * If ORDER='E', sort the eigenvalues from smallest to largest
     */
    if( iorder==1&&*nsplit>1 )
    {
        for(je=1; je<=*m-1; je++)
        {
            ie = 0;
            tmp1 = w->ptr.p_double[je];
            for(j=je+1; j<=*m; j++)
            {
                if( ae_fp_less(w->ptr.p_double[j],tmp1) )
                {
                    ie = j;
                    tmp1 = w->ptr.p_double[j];
                }
            }
            if( ie!=0 )
            {
                itmp1 = iblock->ptr.p_int[ie];
                w->ptr.p_double[ie] = w->ptr.p_double[je];
                iblock->ptr.p_int[ie] = iblock->ptr.p_int[je];
                w->ptr.p_double[je] = tmp1;
                iblock->ptr.p_int[je] = itmp1;
            }
        }
    }
    for(j=1; j<=*m; j++)
    {
        w->ptr.p_double[j] = w->ptr.p_double[j]*scalefactor;
    }
    *errorcode = 0;
    if( ncnvrg )
    {
        *errorcode = *errorcode+1;
    }
    if( toofew )
    {
        *errorcode = *errorcode+2;
    }
    result = *errorcode==0;
    ae_frame_leave(_state);
    return result;
}


static void evd_internaldstein(ae_int_t n,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t m,
     /* Real    */ ae_vector* w,
     /* Integer */ ae_vector* iblock,
     /* Integer */ ae_vector* isplit,
     /* Real    */ ae_matrix* z,
     /* Integer */ ae_vector* ifail,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _e;
    ae_vector _w;
    ae_int_t maxits;
    ae_int_t extra;
    ae_int_t b1;
    ae_int_t blksiz;
    ae_int_t bn;
    ae_int_t gpind;
    ae_int_t i;
    ae_int_t iinfo;
    ae_int_t its;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t jblk;
    ae_int_t jmax;
    ae_int_t nblk;
    ae_int_t nrmchk;
    double dtpcrt;
    double eps;
    double eps1;
    double nrm;
    double onenrm;
    double ortol;
    double pertol;
    double scl;
    double sep;
    double tol;
    double xj;
    double xjm;
    double ztr;
    ae_vector work1;
    ae_vector work2;
    ae_vector work3;
    ae_vector work4;
    ae_vector work5;
    ae_vector iwork;
    ae_bool tmpcriterion;
    ae_int_t ti;
    ae_int_t i1;
    ae_int_t i2;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_e, e, _state);
    e = &_e;
    ae_vector_init_copy(&_w, w, _state);
    w = &_w;
    ae_matrix_clear(z);
    ae_vector_clear(ifail);
    *info = 0;
    ae_vector_init(&work1, 0, DT_REAL, _state);
    ae_vector_init(&work2, 0, DT_REAL, _state);
    ae_vector_init(&work3, 0, DT_REAL, _state);
    ae_vector_init(&work4, 0, DT_REAL, _state);
    ae_vector_init(&work5, 0, DT_REAL, _state);
    ae_vector_init(&iwork, 0, DT_INT, _state);

    maxits = 5;
    extra = 2;
    ae_vector_set_length(&work1, ae_maxint(n, 1, _state)+1, _state);
    ae_vector_set_length(&work2, ae_maxint(n-1, 1, _state)+1, _state);
    ae_vector_set_length(&work3, ae_maxint(n, 1, _state)+1, _state);
    ae_vector_set_length(&work4, ae_maxint(n, 1, _state)+1, _state);
    ae_vector_set_length(&work5, ae_maxint(n, 1, _state)+1, _state);
    ae_vector_set_length(&iwork, ae_maxint(n, 1, _state)+1, _state);
    ae_vector_set_length(ifail, ae_maxint(m, 1, _state)+1, _state);
    ae_matrix_set_length(z, ae_maxint(n, 1, _state)+1, ae_maxint(m, 1, _state)+1, _state);
    
    /*
     * these initializers are not really necessary,
     * but without them compiler complains about uninitialized locals
     */
    gpind = 0;
    onenrm = (double)(0);
    ortol = (double)(0);
    dtpcrt = (double)(0);
    xjm = (double)(0);
    
    /*
     * Test the input parameters.
     */
    *info = 0;
    for(i=1; i<=m; i++)
    {
        ifail->ptr.p_int[i] = 0;
    }
    if( n<0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( m<0||m>n )
    {
        *info = -4;
        ae_frame_leave(_state);
        return;
    }
    for(j=2; j<=m; j++)
    {
        if( iblock->ptr.p_int[j]<iblock->ptr.p_int[j-1] )
        {
            *info = -6;
            break;
        }
        if( iblock->ptr.p_int[j]==iblock->ptr.p_int[j-1]&&ae_fp_less(w->ptr.p_double[j],w->ptr.p_double[j-1]) )
        {
            *info = -5;
            break;
        }
    }
    if( *info!=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Quick return if possible
     */
    if( n==0||m==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    if( n==1 )
    {
        z->ptr.pp_double[1][1] = (double)(1);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Some preparations
     */
    ti = n-1;
    ae_v_move(&work1.ptr.p_double[1], 1, &e->ptr.p_double[1], 1, ae_v_len(1,ti));
    ae_vector_set_length(e, n+1, _state);
    ae_v_move(&e->ptr.p_double[1], 1, &work1.ptr.p_double[1], 1, ae_v_len(1,ti));
    ae_v_move(&work1.ptr.p_double[1], 1, &w->ptr.p_double[1], 1, ae_v_len(1,m));
    ae_vector_set_length(w, n+1, _state);
    ae_v_move(&w->ptr.p_double[1], 1, &work1.ptr.p_double[1], 1, ae_v_len(1,m));
    
    /*
     * Get machine constants.
     */
    eps = ae_machineepsilon;
    
    /*
     * Compute eigenvectors of matrix blocks.
     */
    j1 = 1;
    for(nblk=1; nblk<=iblock->ptr.p_int[m]; nblk++)
    {
        
        /*
         * Find starting and ending indices of block nblk.
         */
        if( nblk==1 )
        {
            b1 = 1;
        }
        else
        {
            b1 = isplit->ptr.p_int[nblk-1]+1;
        }
        bn = isplit->ptr.p_int[nblk];
        blksiz = bn-b1+1;
        if( blksiz!=1 )
        {
            
            /*
             * Compute reorthogonalization criterion and stopping criterion.
             */
            gpind = b1;
            onenrm = ae_fabs(d->ptr.p_double[b1], _state)+ae_fabs(e->ptr.p_double[b1], _state);
            onenrm = ae_maxreal(onenrm, ae_fabs(d->ptr.p_double[bn], _state)+ae_fabs(e->ptr.p_double[bn-1], _state), _state);
            for(i=b1+1; i<=bn-1; i++)
            {
                onenrm = ae_maxreal(onenrm, ae_fabs(d->ptr.p_double[i], _state)+ae_fabs(e->ptr.p_double[i-1], _state)+ae_fabs(e->ptr.p_double[i], _state), _state);
            }
            ortol = 0.001*onenrm;
            dtpcrt = ae_sqrt(0.1/blksiz, _state);
        }
        
        /*
         * Loop through eigenvalues of block nblk.
         */
        jblk = 0;
        for(j=j1; j<=m; j++)
        {
            if( iblock->ptr.p_int[j]!=nblk )
            {
                j1 = j;
                break;
            }
            jblk = jblk+1;
            xj = w->ptr.p_double[j];
            if( blksiz==1 )
            {
                
                /*
                 * Skip all the work if the block size is one.
                 */
                work1.ptr.p_double[1] = (double)(1);
            }
            else
            {
                
                /*
                 * If eigenvalues j and j-1 are too close, add a relatively
                 * small perturbation.
                 */
                if( jblk>1 )
                {
                    eps1 = ae_fabs(eps*xj, _state);
                    pertol = 10*eps1;
                    sep = xj-xjm;
                    if( ae_fp_less(sep,pertol) )
                    {
                        xj = xjm+pertol;
                    }
                }
                its = 0;
                nrmchk = 0;
                
                /*
                 * Get random starting vector.
                 */
                for(ti=1; ti<=blksiz; ti++)
                {
                    work1.ptr.p_double[ti] = 2*ae_randomreal(_state)-1;
                }
                
                /*
                 * Copy the matrix T so it won't be destroyed in factorization.
                 */
                for(ti=1; ti<=blksiz-1; ti++)
                {
                    work2.ptr.p_double[ti] = e->ptr.p_double[b1+ti-1];
                    work3.ptr.p_double[ti] = e->ptr.p_double[b1+ti-1];
                    work4.ptr.p_double[ti] = d->ptr.p_double[b1+ti-1];
                }
                work4.ptr.p_double[blksiz] = d->ptr.p_double[b1+blksiz-1];
                
                /*
                 * Compute LU factors with partial pivoting  ( PT = LU )
                 */
                tol = (double)(0);
                evd_tdininternaldlagtf(blksiz, &work4, xj, &work2, &work3, tol, &work5, &iwork, &iinfo, _state);
                
                /*
                 * Update iteration count.
                 */
                do
                {
                    its = its+1;
                    if( its>maxits )
                    {
                        
                        /*
                         * If stopping criterion was not satisfied, update info and
                         * store eigenvector number in array ifail.
                         */
                        *info = *info+1;
                        ifail->ptr.p_int[*info] = j;
                        break;
                    }
                    
                    /*
                     * Normalize and scale the righthand side vector Pb.
                     */
                    v = (double)(0);
                    for(ti=1; ti<=blksiz; ti++)
                    {
                        v = v+ae_fabs(work1.ptr.p_double[ti], _state);
                    }
                    scl = blksiz*onenrm*ae_maxreal(eps, ae_fabs(work4.ptr.p_double[blksiz], _state), _state)/v;
                    ae_v_muld(&work1.ptr.p_double[1], 1, ae_v_len(1,blksiz), scl);
                    
                    /*
                     * Solve the system LU = Pb.
                     */
                    evd_tdininternaldlagts(blksiz, &work4, &work2, &work3, &work5, &iwork, &work1, &tol, &iinfo, _state);
                    
                    /*
                     * Reorthogonalize by modified Gram-Schmidt if eigenvalues are
                     * close enough.
                     */
                    if( jblk!=1 )
                    {
                        if( ae_fp_greater(ae_fabs(xj-xjm, _state),ortol) )
                        {
                            gpind = j;
                        }
                        if( gpind!=j )
                        {
                            for(i=gpind; i<=j-1; i++)
                            {
                                i1 = b1;
                                i2 = b1+blksiz-1;
                                ztr = ae_v_dotproduct(&work1.ptr.p_double[1], 1, &z->ptr.pp_double[i1][i], z->stride, ae_v_len(1,blksiz));
                                ae_v_subd(&work1.ptr.p_double[1], 1, &z->ptr.pp_double[i1][i], z->stride, ae_v_len(1,blksiz), ztr);
                                touchint(&i2, _state);
                            }
                        }
                    }
                    
                    /*
                     * Check the infinity norm of the iterate.
                     */
                    jmax = vectoridxabsmax(&work1, 1, blksiz, _state);
                    nrm = ae_fabs(work1.ptr.p_double[jmax], _state);
                    
                    /*
                     * Continue for additional iterations after norm reaches
                     * stopping criterion.
                     */
                    tmpcriterion = ae_false;
                    if( ae_fp_less(nrm,dtpcrt) )
                    {
                        tmpcriterion = ae_true;
                    }
                    else
                    {
                        nrmchk = nrmchk+1;
                        if( nrmchk<extra+1 )
                        {
                            tmpcriterion = ae_true;
                        }
                    }
                }
                while(tmpcriterion);
                
                /*
                 * Accept iterate as jth eigenvector.
                 */
                scl = 1/vectornorm2(&work1, 1, blksiz, _state);
                jmax = vectoridxabsmax(&work1, 1, blksiz, _state);
                if( ae_fp_less(work1.ptr.p_double[jmax],(double)(0)) )
                {
                    scl = -scl;
                }
                ae_v_muld(&work1.ptr.p_double[1], 1, ae_v_len(1,blksiz), scl);
            }
            for(i=1; i<=n; i++)
            {
                z->ptr.pp_double[i][j] = (double)(0);
            }
            for(i=1; i<=blksiz; i++)
            {
                z->ptr.pp_double[b1+i-1][j] = work1.ptr.p_double[i];
            }
            
            /*
             * Save the shift to check eigenvalue spacing at next
             * iteration.
             */
            xjm = xj;
        }
    }
    ae_frame_leave(_state);
}


static void evd_tdininternaldlagtf(ae_int_t n,
     /* Real    */ ae_vector* a,
     double lambdav,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* c,
     double tol,
     /* Real    */ ae_vector* d,
     /* Integer */ ae_vector* iin,
     ae_int_t* info,
     ae_state *_state)
{
    ae_int_t k;
    double eps;
    double mult;
    double piv1;
    double piv2;
    double scale1;
    double scale2;
    double temp;
    double tl;

    *info = 0;

    *info = 0;
    if( n<0 )
    {
        *info = -1;
        return;
    }
    if( n==0 )
    {
        return;
    }
    a->ptr.p_double[1] = a->ptr.p_double[1]-lambdav;
    iin->ptr.p_int[n] = 0;
    if( n==1 )
    {
        if( ae_fp_eq(a->ptr.p_double[1],(double)(0)) )
        {
            iin->ptr.p_int[1] = 1;
        }
        return;
    }
    eps = ae_machineepsilon;
    tl = ae_maxreal(tol, eps, _state);
    scale1 = ae_fabs(a->ptr.p_double[1], _state)+ae_fabs(b->ptr.p_double[1], _state);
    for(k=1; k<=n-1; k++)
    {
        a->ptr.p_double[k+1] = a->ptr.p_double[k+1]-lambdav;
        scale2 = ae_fabs(c->ptr.p_double[k], _state)+ae_fabs(a->ptr.p_double[k+1], _state);
        if( k<n-1 )
        {
            scale2 = scale2+ae_fabs(b->ptr.p_double[k+1], _state);
        }
        if( ae_fp_eq(a->ptr.p_double[k],(double)(0)) )
        {
            piv1 = (double)(0);
        }
        else
        {
            piv1 = ae_fabs(a->ptr.p_double[k], _state)/scale1;
        }
        if( ae_fp_eq(c->ptr.p_double[k],(double)(0)) )
        {
            iin->ptr.p_int[k] = 0;
            piv2 = (double)(0);
            scale1 = scale2;
            if( k<n-1 )
            {
                d->ptr.p_double[k] = (double)(0);
            }
        }
        else
        {
            piv2 = ae_fabs(c->ptr.p_double[k], _state)/scale2;
            if( ae_fp_less_eq(piv2,piv1) )
            {
                iin->ptr.p_int[k] = 0;
                scale1 = scale2;
                c->ptr.p_double[k] = c->ptr.p_double[k]/a->ptr.p_double[k];
                a->ptr.p_double[k+1] = a->ptr.p_double[k+1]-c->ptr.p_double[k]*b->ptr.p_double[k];
                if( k<n-1 )
                {
                    d->ptr.p_double[k] = (double)(0);
                }
            }
            else
            {
                iin->ptr.p_int[k] = 1;
                mult = a->ptr.p_double[k]/c->ptr.p_double[k];
                a->ptr.p_double[k] = c->ptr.p_double[k];
                temp = a->ptr.p_double[k+1];
                a->ptr.p_double[k+1] = b->ptr.p_double[k]-mult*temp;
                if( k<n-1 )
                {
                    d->ptr.p_double[k] = b->ptr.p_double[k+1];
                    b->ptr.p_double[k+1] = -mult*d->ptr.p_double[k];
                }
                b->ptr.p_double[k] = temp;
                c->ptr.p_double[k] = mult;
            }
        }
        if( ae_fp_less_eq(ae_maxreal(piv1, piv2, _state),tl)&&iin->ptr.p_int[n]==0 )
        {
            iin->ptr.p_int[n] = k;
        }
    }
    if( ae_fp_less_eq(ae_fabs(a->ptr.p_double[n], _state),scale1*tl)&&iin->ptr.p_int[n]==0 )
    {
        iin->ptr.p_int[n] = n;
    }
}


static void evd_tdininternaldlagts(ae_int_t n,
     /* Real    */ ae_vector* a,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_vector* d,
     /* Integer */ ae_vector* iin,
     /* Real    */ ae_vector* y,
     double* tol,
     ae_int_t* info,
     ae_state *_state)
{
    ae_int_t k;
    double absak;
    double ak;
    double bignum;
    double eps;
    double pert;
    double sfmin;
    double temp;

    *info = 0;

    *info = 0;
    if( n<0 )
    {
        *info = -1;
        return;
    }
    if( n==0 )
    {
        return;
    }
    eps = ae_machineepsilon;
    sfmin = ae_minrealnumber;
    bignum = 1/sfmin;
    if( ae_fp_less_eq(*tol,(double)(0)) )
    {
        *tol = ae_fabs(a->ptr.p_double[1], _state);
        if( n>1 )
        {
            *tol = ae_maxreal(*tol, ae_maxreal(ae_fabs(a->ptr.p_double[2], _state), ae_fabs(b->ptr.p_double[1], _state), _state), _state);
        }
        for(k=3; k<=n; k++)
        {
            *tol = ae_maxreal(*tol, ae_maxreal(ae_fabs(a->ptr.p_double[k], _state), ae_maxreal(ae_fabs(b->ptr.p_double[k-1], _state), ae_fabs(d->ptr.p_double[k-2], _state), _state), _state), _state);
        }
        *tol = *tol*eps;
        if( ae_fp_eq(*tol,(double)(0)) )
        {
            *tol = eps;
        }
    }
    for(k=2; k<=n; k++)
    {
        if( iin->ptr.p_int[k-1]==0 )
        {
            y->ptr.p_double[k] = y->ptr.p_double[k]-c->ptr.p_double[k-1]*y->ptr.p_double[k-1];
        }
        else
        {
            temp = y->ptr.p_double[k-1];
            y->ptr.p_double[k-1] = y->ptr.p_double[k];
            y->ptr.p_double[k] = temp-c->ptr.p_double[k-1]*y->ptr.p_double[k];
        }
    }
    for(k=n; k>=1; k--)
    {
        if( k<=n-2 )
        {
            temp = y->ptr.p_double[k]-b->ptr.p_double[k]*y->ptr.p_double[k+1]-d->ptr.p_double[k]*y->ptr.p_double[k+2];
        }
        else
        {
            if( k==n-1 )
            {
                temp = y->ptr.p_double[k]-b->ptr.p_double[k]*y->ptr.p_double[k+1];
            }
            else
            {
                temp = y->ptr.p_double[k];
            }
        }
        ak = a->ptr.p_double[k];
        pert = ae_fabs(*tol, _state);
        if( ae_fp_less(ak,(double)(0)) )
        {
            pert = -pert;
        }
        for(;;)
        {
            absak = ae_fabs(ak, _state);
            if( ae_fp_less(absak,(double)(1)) )
            {
                if( ae_fp_less(absak,sfmin) )
                {
                    if( ae_fp_eq(absak,(double)(0))||ae_fp_greater(ae_fabs(temp, _state)*sfmin,absak) )
                    {
                        ak = ak+pert;
                        pert = 2*pert;
                        continue;
                    }
                    else
                    {
                        temp = temp*bignum;
                        ak = ak*bignum;
                    }
                }
                else
                {
                    if( ae_fp_greater(ae_fabs(temp, _state),absak*bignum) )
                    {
                        ak = ak+pert;
                        pert = 2*pert;
                        continue;
                    }
                }
            }
            break;
        }
        y->ptr.p_double[k] = temp/ak;
    }
}


static void evd_internaldlaebz(ae_int_t ijob,
     ae_int_t nitmax,
     ae_int_t n,
     ae_int_t mmax,
     ae_int_t minp,
     double abstol,
     double reltol,
     double pivmin,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     /* Real    */ ae_vector* e2,
     /* Integer */ ae_vector* nval,
     /* Real    */ ae_matrix* ab,
     /* Real    */ ae_vector* c,
     ae_int_t* mout,
     /* Integer */ ae_matrix* nab,
     /* Real    */ ae_vector* work,
     /* Integer */ ae_vector* iwork,
     ae_int_t* info,
     ae_state *_state)
{
    ae_int_t itmp1;
    ae_int_t itmp2;
    ae_int_t j;
    ae_int_t ji;
    ae_int_t jit;
    ae_int_t jp;
    ae_int_t kf;
    ae_int_t kfnew;
    ae_int_t kl;
    ae_int_t klnew;
    double tmp1;
    double tmp2;

    *mout = 0;
    *info = 0;

    *info = 0;
    if( ijob<1||ijob>3 )
    {
        *info = -1;
        return;
    }
    
    /*
     * Initialize NAB
     */
    if( ijob==1 )
    {
        
        /*
         * Compute the number of eigenvalues in the initial intervals.
         */
        *mout = 0;
        
        /*
         *DIR$ NOVECTOR
         */
        for(ji=1; ji<=minp; ji++)
        {
            for(jp=1; jp<=2; jp++)
            {
                tmp1 = d->ptr.p_double[1]-ab->ptr.pp_double[ji][jp];
                if( ae_fp_less(ae_fabs(tmp1, _state),pivmin) )
                {
                    tmp1 = -pivmin;
                }
                nab->ptr.pp_int[ji][jp] = 0;
                if( ae_fp_less_eq(tmp1,(double)(0)) )
                {
                    nab->ptr.pp_int[ji][jp] = 1;
                }
                for(j=2; j<=n; j++)
                {
                    tmp1 = d->ptr.p_double[j]-e2->ptr.p_double[j-1]/tmp1-ab->ptr.pp_double[ji][jp];
                    if( ae_fp_less(ae_fabs(tmp1, _state),pivmin) )
                    {
                        tmp1 = -pivmin;
                    }
                    if( ae_fp_less_eq(tmp1,(double)(0)) )
                    {
                        nab->ptr.pp_int[ji][jp] = nab->ptr.pp_int[ji][jp]+1;
                    }
                }
            }
            *mout = *mout+nab->ptr.pp_int[ji][2]-nab->ptr.pp_int[ji][1];
        }
        return;
    }
    
    /*
     * Initialize for loop
     *
     * KF and KL have the following meaning:
     *   Intervals 1,...,KF-1 have converged.
     *   Intervals KF,...,KL  still need to be refined.
     */
    kf = 1;
    kl = minp;
    
    /*
     * If IJOB=2, initialize C.
     * If IJOB=3, use the user-supplied starting point.
     */
    if( ijob==2 )
    {
        for(ji=1; ji<=minp; ji++)
        {
            c->ptr.p_double[ji] = 0.5*(ab->ptr.pp_double[ji][1]+ab->ptr.pp_double[ji][2]);
        }
    }
    
    /*
     * Iteration loop
     */
    for(jit=1; jit<=nitmax; jit++)
    {
        
        /*
         * Loop over intervals
         *
         *
         * Serial Version of the loop
         */
        klnew = kl;
        for(ji=kf; ji<=kl; ji++)
        {
            
            /*
             * Compute N(w), the number of eigenvalues less than w
             */
            tmp1 = c->ptr.p_double[ji];
            tmp2 = d->ptr.p_double[1]-tmp1;
            itmp1 = 0;
            if( ae_fp_less_eq(tmp2,pivmin) )
            {
                itmp1 = 1;
                tmp2 = ae_minreal(tmp2, -pivmin, _state);
            }
            
            /*
             * A series of compiler directives to defeat vectorization
             * for the next loop
             *
             **$PL$ CMCHAR=' '
             *CDIR$          NEXTSCALAR
             *C$DIR          SCALAR
             *CDIR$          NEXT SCALAR
             *CVD$L          NOVECTOR
             *CDEC$          NOVECTOR
             *CVD$           NOVECTOR
             **VDIR          NOVECTOR
             **VOCL          LOOP,SCALAR
             *CIBM           PREFER SCALAR
             **$PL$ CMCHAR='*'
             */
            for(j=2; j<=n; j++)
            {
                tmp2 = d->ptr.p_double[j]-e2->ptr.p_double[j-1]/tmp2-tmp1;
                if( ae_fp_less_eq(tmp2,pivmin) )
                {
                    itmp1 = itmp1+1;
                    tmp2 = ae_minreal(tmp2, -pivmin, _state);
                }
            }
            if( ijob<=2 )
            {
                
                /*
                 * IJOB=2: Choose all intervals containing eigenvalues.
                 *
                 * Insure that N(w) is monotone
                 */
                itmp1 = ae_minint(nab->ptr.pp_int[ji][2], ae_maxint(nab->ptr.pp_int[ji][1], itmp1, _state), _state);
                
                /*
                 * Update the Queue -- add intervals if both halves
                 * contain eigenvalues.
                 */
                if( itmp1==nab->ptr.pp_int[ji][2] )
                {
                    
                    /*
                     * No eigenvalue in the upper interval:
                     * just use the lower interval.
                     */
                    ab->ptr.pp_double[ji][2] = tmp1;
                }
                else
                {
                    if( itmp1==nab->ptr.pp_int[ji][1] )
                    {
                        
                        /*
                         * No eigenvalue in the lower interval:
                         * just use the upper interval.
                         */
                        ab->ptr.pp_double[ji][1] = tmp1;
                    }
                    else
                    {
                        if( klnew<mmax )
                        {
                            
                            /*
                             * Eigenvalue in both intervals -- add upper to queue.
                             */
                            klnew = klnew+1;
                            ab->ptr.pp_double[klnew][2] = ab->ptr.pp_double[ji][2];
                            nab->ptr.pp_int[klnew][2] = nab->ptr.pp_int[ji][2];
                            ab->ptr.pp_double[klnew][1] = tmp1;
                            nab->ptr.pp_int[klnew][1] = itmp1;
                            ab->ptr.pp_double[ji][2] = tmp1;
                            nab->ptr.pp_int[ji][2] = itmp1;
                        }
                        else
                        {
                            *info = mmax+1;
                            return;
                        }
                    }
                }
            }
            else
            {
                
                /*
                 * IJOB=3: Binary search.  Keep only the interval
                 * containing  w  s.t. N(w) = NVAL
                 */
                if( itmp1<=nval->ptr.p_int[ji] )
                {
                    ab->ptr.pp_double[ji][1] = tmp1;
                    nab->ptr.pp_int[ji][1] = itmp1;
                }
                if( itmp1>=nval->ptr.p_int[ji] )
                {
                    ab->ptr.pp_double[ji][2] = tmp1;
                    nab->ptr.pp_int[ji][2] = itmp1;
                }
            }
        }
        kl = klnew;
        
        /*
         * Check for convergence
         */
        kfnew = kf;
        for(ji=kf; ji<=kl; ji++)
        {
            tmp1 = ae_fabs(ab->ptr.pp_double[ji][2]-ab->ptr.pp_double[ji][1], _state);
            tmp2 = ae_maxreal(ae_fabs(ab->ptr.pp_double[ji][2], _state), ae_fabs(ab->ptr.pp_double[ji][1], _state), _state);
            if( ae_fp_less(tmp1,ae_maxreal(abstol, ae_maxreal(pivmin, reltol*tmp2, _state), _state))||nab->ptr.pp_int[ji][1]>=nab->ptr.pp_int[ji][2] )
            {
                
                /*
                 * Converged -- Swap with position KFNEW,
                 * then increment KFNEW
                 */
                if( ji>kfnew )
                {
                    tmp1 = ab->ptr.pp_double[ji][1];
                    tmp2 = ab->ptr.pp_double[ji][2];
                    itmp1 = nab->ptr.pp_int[ji][1];
                    itmp2 = nab->ptr.pp_int[ji][2];
                    ab->ptr.pp_double[ji][1] = ab->ptr.pp_double[kfnew][1];
                    ab->ptr.pp_double[ji][2] = ab->ptr.pp_double[kfnew][2];
                    nab->ptr.pp_int[ji][1] = nab->ptr.pp_int[kfnew][1];
                    nab->ptr.pp_int[ji][2] = nab->ptr.pp_int[kfnew][2];
                    ab->ptr.pp_double[kfnew][1] = tmp1;
                    ab->ptr.pp_double[kfnew][2] = tmp2;
                    nab->ptr.pp_int[kfnew][1] = itmp1;
                    nab->ptr.pp_int[kfnew][2] = itmp2;
                    if( ijob==3 )
                    {
                        itmp1 = nval->ptr.p_int[ji];
                        nval->ptr.p_int[ji] = nval->ptr.p_int[kfnew];
                        nval->ptr.p_int[kfnew] = itmp1;
                    }
                }
                kfnew = kfnew+1;
            }
        }
        kf = kfnew;
        
        /*
         * Choose Midpoints
         */
        for(ji=kf; ji<=kl; ji++)
        {
            c->ptr.p_double[ji] = 0.5*(ab->ptr.pp_double[ji][1]+ab->ptr.pp_double[ji][2]);
        }
        
        /*
         * If no more intervals to refine, quit.
         */
        if( kf>kl )
        {
            break;
        }
    }
    
    /*
     * Converged
     */
    *info = ae_maxint(kl+1-kf, 0, _state);
    *mout = kl;
}


/*************************************************************************
Internal subroutine

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     June 30, 1999
*************************************************************************/
static void evd_rmatrixinternaltrevc(/* Real    */ ae_matrix* t,
     ae_int_t n,
     ae_int_t side,
     ae_int_t howmny,
     /* Boolean */ ae_vector* vselect,
     /* Real    */ ae_matrix* vl,
     /* Real    */ ae_matrix* vr,
     ae_int_t* m,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _vselect;
    ae_int_t i;
    ae_int_t j;
    ae_matrix t1;
    ae_matrix vl1;
    ae_matrix vr1;
    ae_vector vselect1;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_vselect, vselect, _state);
    vselect = &_vselect;
    *m = 0;
    *info = 0;
    ae_matrix_init(&t1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vl1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vr1, 0, 0, DT_REAL, _state);
    ae_vector_init(&vselect1, 0, DT_BOOL, _state);

    
    /*
     * Allocate VL/VR, if needed
     */
    if( howmny==2||howmny==3 )
    {
        if( side==1||side==3 )
        {
            rmatrixsetlengthatleast(vr, n, n, _state);
        }
        if( side==2||side==3 )
        {
            rmatrixsetlengthatleast(vl, n, n, _state);
        }
    }
    
    /*
     * Try to use MKL kernel
     */
    if( rmatrixinternaltrevcmkl(t, n, side, howmny, vl, vr, m, info, _state) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * ALGLIB version
     */
    ae_matrix_set_length(&t1, n+1, n+1, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            t1.ptr.pp_double[i+1][j+1] = t->ptr.pp_double[i][j];
        }
    }
    if( howmny==3 )
    {
        ae_vector_set_length(&vselect1, n+1, _state);
        for(i=0; i<=n-1; i++)
        {
            vselect1.ptr.p_bool[1+i] = vselect->ptr.p_bool[i];
        }
    }
    if( (side==2||side==3)&&howmny==1 )
    {
        ae_matrix_set_length(&vl1, n+1, n+1, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                vl1.ptr.pp_double[i+1][j+1] = vl->ptr.pp_double[i][j];
            }
        }
    }
    if( (side==1||side==3)&&howmny==1 )
    {
        ae_matrix_set_length(&vr1, n+1, n+1, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                vr1.ptr.pp_double[i+1][j+1] = vr->ptr.pp_double[i][j];
            }
        }
    }
    evd_internaltrevc(&t1, n, side, howmny, &vselect1, &vl1, &vr1, m, info, _state);
    if( side!=1 )
    {
        rmatrixsetlengthatleast(vl, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                vl->ptr.pp_double[i][j] = vl1.ptr.pp_double[i+1][j+1];
            }
        }
    }
    if( side!=2 )
    {
        rmatrixsetlengthatleast(vr, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                vr->ptr.pp_double[i][j] = vr1.ptr.pp_double[i+1][j+1];
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     June 30, 1999
*************************************************************************/
static void evd_internaltrevc(/* Real    */ ae_matrix* t,
     ae_int_t n,
     ae_int_t side,
     ae_int_t howmny,
     /* Boolean */ ae_vector* vselect,
     /* Real    */ ae_matrix* vl,
     /* Real    */ ae_matrix* vr,
     ae_int_t* m,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _vselect;
    ae_bool allv;
    ae_bool bothv;
    ae_bool leftv;
    ae_bool over;
    ae_bool pair;
    ae_bool rightv;
    ae_bool somev;
    ae_int_t i;
    ae_int_t ierr;
    ae_int_t ii;
    ae_int_t ip;
    ae_int_t iis;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t j2;
    ae_int_t jnxt;
    ae_int_t k;
    ae_int_t ki;
    ae_int_t n2;
    double beta;
    double bignum;
    double emax;
    double rec;
    double remax;
    double scl;
    double smin;
    double smlnum;
    double ulp;
    double unfl;
    double vcrit;
    double vmax;
    double wi;
    double wr;
    double xnorm;
    ae_matrix x;
    ae_vector work;
    ae_vector temp;
    ae_matrix temp11;
    ae_matrix temp22;
    ae_matrix temp11b;
    ae_matrix temp21b;
    ae_matrix temp12b;
    ae_matrix temp22b;
    ae_bool skipflag;
    ae_int_t k1;
    ae_int_t k2;
    ae_int_t k3;
    ae_int_t k4;
    double vt;
    ae_vector rswap4;
    ae_vector zswap4;
    ae_matrix ipivot44;
    ae_vector civ4;
    ae_vector crv4;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_vselect, vselect, _state);
    vselect = &_vselect;
    *m = 0;
    *info = 0;
    ae_matrix_init(&x, 0, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_vector_init(&temp, 0, DT_REAL, _state);
    ae_matrix_init(&temp11, 0, 0, DT_REAL, _state);
    ae_matrix_init(&temp22, 0, 0, DT_REAL, _state);
    ae_matrix_init(&temp11b, 0, 0, DT_REAL, _state);
    ae_matrix_init(&temp21b, 0, 0, DT_REAL, _state);
    ae_matrix_init(&temp12b, 0, 0, DT_REAL, _state);
    ae_matrix_init(&temp22b, 0, 0, DT_REAL, _state);
    ae_vector_init(&rswap4, 0, DT_BOOL, _state);
    ae_vector_init(&zswap4, 0, DT_BOOL, _state);
    ae_matrix_init(&ipivot44, 0, 0, DT_INT, _state);
    ae_vector_init(&civ4, 0, DT_REAL, _state);
    ae_vector_init(&crv4, 0, DT_REAL, _state);

    ae_matrix_set_length(&x, 2+1, 2+1, _state);
    ae_matrix_set_length(&temp11, 1+1, 1+1, _state);
    ae_matrix_set_length(&temp11b, 1+1, 1+1, _state);
    ae_matrix_set_length(&temp21b, 2+1, 1+1, _state);
    ae_matrix_set_length(&temp12b, 1+1, 2+1, _state);
    ae_matrix_set_length(&temp22b, 2+1, 2+1, _state);
    ae_matrix_set_length(&temp22, 2+1, 2+1, _state);
    ae_vector_set_length(&work, 3*n+1, _state);
    ae_vector_set_length(&temp, n+1, _state);
    ae_vector_set_length(&rswap4, 4+1, _state);
    ae_vector_set_length(&zswap4, 4+1, _state);
    ae_matrix_set_length(&ipivot44, 4+1, 4+1, _state);
    ae_vector_set_length(&civ4, 4+1, _state);
    ae_vector_set_length(&crv4, 4+1, _state);
    if( howmny!=1 )
    {
        if( side==1||side==3 )
        {
            ae_matrix_set_length(vr, n+1, n+1, _state);
        }
        if( side==2||side==3 )
        {
            ae_matrix_set_length(vl, n+1, n+1, _state);
        }
    }
    
    /*
     * Decode and test the input parameters
     */
    bothv = side==3;
    rightv = side==1||bothv;
    leftv = side==2||bothv;
    allv = howmny==2;
    over = howmny==1;
    somev = howmny==3;
    *info = 0;
    if( n<0 )
    {
        *info = -2;
        ae_frame_leave(_state);
        return;
    }
    if( !rightv&&!leftv )
    {
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    if( (!allv&&!over)&&!somev )
    {
        *info = -4;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Set M to the number of columns required to store the selected
     * eigenvectors, standardize the array SELECT if necessary, and
     * test MM.
     */
    if( somev )
    {
        *m = 0;
        pair = ae_false;
        for(j=1; j<=n; j++)
        {
            if( pair )
            {
                pair = ae_false;
                vselect->ptr.p_bool[j] = ae_false;
            }
            else
            {
                if( j<n )
                {
                    if( ae_fp_eq(t->ptr.pp_double[j+1][j],(double)(0)) )
                    {
                        if( vselect->ptr.p_bool[j] )
                        {
                            *m = *m+1;
                        }
                    }
                    else
                    {
                        pair = ae_true;
                        if( vselect->ptr.p_bool[j]||vselect->ptr.p_bool[j+1] )
                        {
                            vselect->ptr.p_bool[j] = ae_true;
                            *m = *m+2;
                        }
                    }
                }
                else
                {
                    if( vselect->ptr.p_bool[n] )
                    {
                        *m = *m+1;
                    }
                }
            }
        }
    }
    else
    {
        *m = n;
    }
    
    /*
     * Quick return if possible.
     */
    if( n==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Set the constants to control overflow.
     */
    unfl = ae_minrealnumber;
    ulp = ae_machineepsilon;
    smlnum = unfl*(n/ulp);
    bignum = (1-ulp)/smlnum;
    
    /*
     * Compute 1-norm of each column of strictly upper triangular
     * part of T to control overflow in triangular solver.
     */
    work.ptr.p_double[1] = (double)(0);
    for(j=2; j<=n; j++)
    {
        work.ptr.p_double[j] = (double)(0);
        for(i=1; i<=j-1; i++)
        {
            work.ptr.p_double[j] = work.ptr.p_double[j]+ae_fabs(t->ptr.pp_double[i][j], _state);
        }
    }
    
    /*
     * Index IP is used to specify the real or complex eigenvalue:
     * IP = 0, real eigenvalue,
     *      1, first of conjugate complex pair: (wr,wi)
     *     -1, second of conjugate complex pair: (wr,wi)
     */
    n2 = 2*n;
    if( rightv )
    {
        
        /*
         * Compute right eigenvectors.
         */
        ip = 0;
        iis = *m;
        for(ki=n; ki>=1; ki--)
        {
            skipflag = ae_false;
            if( ip==1 )
            {
                skipflag = ae_true;
            }
            else
            {
                if( ki!=1 )
                {
                    if( ae_fp_neq(t->ptr.pp_double[ki][ki-1],(double)(0)) )
                    {
                        ip = -1;
                    }
                }
                if( somev )
                {
                    if( ip==0 )
                    {
                        if( !vselect->ptr.p_bool[ki] )
                        {
                            skipflag = ae_true;
                        }
                    }
                    else
                    {
                        if( !vselect->ptr.p_bool[ki-1] )
                        {
                            skipflag = ae_true;
                        }
                    }
                }
            }
            if( !skipflag )
            {
                
                /*
                 * Compute the KI-th eigenvalue (WR,WI).
                 */
                wr = t->ptr.pp_double[ki][ki];
                wi = (double)(0);
                if( ip!=0 )
                {
                    wi = ae_sqrt(ae_fabs(t->ptr.pp_double[ki][ki-1], _state), _state)*ae_sqrt(ae_fabs(t->ptr.pp_double[ki-1][ki], _state), _state);
                }
                smin = ae_maxreal(ulp*(ae_fabs(wr, _state)+ae_fabs(wi, _state)), smlnum, _state);
                if( ip==0 )
                {
                    
                    /*
                     * Real right eigenvector
                     */
                    work.ptr.p_double[ki+n] = (double)(1);
                    
                    /*
                     * Form right-hand side
                     */
                    for(k=1; k<=ki-1; k++)
                    {
                        work.ptr.p_double[k+n] = -t->ptr.pp_double[k][ki];
                    }
                    
                    /*
                     * Solve the upper quasi-triangular system:
                     *   (T(1:KI-1,1:KI-1) - WR)*X = SCALE*WORK.
                     */
                    jnxt = ki-1;
                    for(j=ki-1; j>=1; j--)
                    {
                        if( j>jnxt )
                        {
                            continue;
                        }
                        j1 = j;
                        j2 = j;
                        jnxt = j-1;
                        if( j>1 )
                        {
                            if( ae_fp_neq(t->ptr.pp_double[j][j-1],(double)(0)) )
                            {
                                j1 = j-1;
                                jnxt = j-2;
                            }
                        }
                        if( j1==j2 )
                        {
                            
                            /*
                             * 1-by-1 diagonal block
                             */
                            temp11.ptr.pp_double[1][1] = t->ptr.pp_double[j][j];
                            temp11b.ptr.pp_double[1][1] = work.ptr.p_double[j+n];
                            evd_internalhsevdlaln2(ae_false, 1, 1, smin, (double)(1), &temp11, 1.0, 1.0, &temp11b, wr, 0.0, &rswap4, &zswap4, &ipivot44, &civ4, &crv4, &x, &scl, &xnorm, &ierr, _state);
                            
                            /*
                             * Scale X(1,1) to avoid overflow when updating
                             * the right-hand side.
                             */
                            if( ae_fp_greater(xnorm,(double)(1)) )
                            {
                                if( ae_fp_greater(work.ptr.p_double[j],bignum/xnorm) )
                                {
                                    x.ptr.pp_double[1][1] = x.ptr.pp_double[1][1]/xnorm;
                                    scl = scl/xnorm;
                                }
                            }
                            
                            /*
                             * Scale if necessary
                             */
                            if( ae_fp_neq(scl,(double)(1)) )
                            {
                                k1 = n+1;
                                k2 = n+ki;
                                ae_v_muld(&work.ptr.p_double[k1], 1, ae_v_len(k1,k2), scl);
                            }
                            work.ptr.p_double[j+n] = x.ptr.pp_double[1][1];
                            
                            /*
                             * Update right-hand side
                             */
                            k1 = 1+n;
                            k2 = j-1+n;
                            k3 = j-1;
                            vt = -x.ptr.pp_double[1][1];
                            ae_v_addd(&work.ptr.p_double[k1], 1, &t->ptr.pp_double[1][j], t->stride, ae_v_len(k1,k2), vt);
                        }
                        else
                        {
                            
                            /*
                             * 2-by-2 diagonal block
                             */
                            temp22.ptr.pp_double[1][1] = t->ptr.pp_double[j-1][j-1];
                            temp22.ptr.pp_double[1][2] = t->ptr.pp_double[j-1][j];
                            temp22.ptr.pp_double[2][1] = t->ptr.pp_double[j][j-1];
                            temp22.ptr.pp_double[2][2] = t->ptr.pp_double[j][j];
                            temp21b.ptr.pp_double[1][1] = work.ptr.p_double[j-1+n];
                            temp21b.ptr.pp_double[2][1] = work.ptr.p_double[j+n];
                            evd_internalhsevdlaln2(ae_false, 2, 1, smin, 1.0, &temp22, 1.0, 1.0, &temp21b, wr, (double)(0), &rswap4, &zswap4, &ipivot44, &civ4, &crv4, &x, &scl, &xnorm, &ierr, _state);
                            
                            /*
                             * Scale X(1,1) and X(2,1) to avoid overflow when
                             * updating the right-hand side.
                             */
                            if( ae_fp_greater(xnorm,(double)(1)) )
                            {
                                beta = ae_maxreal(work.ptr.p_double[j-1], work.ptr.p_double[j], _state);
                                if( ae_fp_greater(beta,bignum/xnorm) )
                                {
                                    x.ptr.pp_double[1][1] = x.ptr.pp_double[1][1]/xnorm;
                                    x.ptr.pp_double[2][1] = x.ptr.pp_double[2][1]/xnorm;
                                    scl = scl/xnorm;
                                }
                            }
                            
                            /*
                             * Scale if necessary
                             */
                            if( ae_fp_neq(scl,(double)(1)) )
                            {
                                k1 = 1+n;
                                k2 = ki+n;
                                ae_v_muld(&work.ptr.p_double[k1], 1, ae_v_len(k1,k2), scl);
                            }
                            work.ptr.p_double[j-1+n] = x.ptr.pp_double[1][1];
                            work.ptr.p_double[j+n] = x.ptr.pp_double[2][1];
                            
                            /*
                             * Update right-hand side
                             */
                            k1 = 1+n;
                            k2 = j-2+n;
                            k3 = j-2;
                            k4 = j-1;
                            vt = -x.ptr.pp_double[1][1];
                            ae_v_addd(&work.ptr.p_double[k1], 1, &t->ptr.pp_double[1][k4], t->stride, ae_v_len(k1,k2), vt);
                            vt = -x.ptr.pp_double[2][1];
                            ae_v_addd(&work.ptr.p_double[k1], 1, &t->ptr.pp_double[1][j], t->stride, ae_v_len(k1,k2), vt);
                        }
                    }
                    
                    /*
                     * Copy the vector x or Q*x to VR and normalize.
                     */
                    if( !over )
                    {
                        k1 = 1+n;
                        k2 = ki+n;
                        ae_v_move(&vr->ptr.pp_double[1][iis], vr->stride, &work.ptr.p_double[k1], 1, ae_v_len(1,ki));
                        ii = columnidxabsmax(vr, 1, ki, iis, _state);
                        remax = 1/ae_fabs(vr->ptr.pp_double[ii][iis], _state);
                        ae_v_muld(&vr->ptr.pp_double[1][iis], vr->stride, ae_v_len(1,ki), remax);
                        for(k=ki+1; k<=n; k++)
                        {
                            vr->ptr.pp_double[k][iis] = (double)(0);
                        }
                    }
                    else
                    {
                        if( ki>1 )
                        {
                            ae_v_move(&temp.ptr.p_double[1], 1, &vr->ptr.pp_double[1][ki], vr->stride, ae_v_len(1,n));
                            matrixvectormultiply(vr, 1, n, 1, ki-1, ae_false, &work, 1+n, ki-1+n, 1.0, &temp, 1, n, work.ptr.p_double[ki+n], _state);
                            ae_v_move(&vr->ptr.pp_double[1][ki], vr->stride, &temp.ptr.p_double[1], 1, ae_v_len(1,n));
                        }
                        ii = columnidxabsmax(vr, 1, n, ki, _state);
                        remax = 1/ae_fabs(vr->ptr.pp_double[ii][ki], _state);
                        ae_v_muld(&vr->ptr.pp_double[1][ki], vr->stride, ae_v_len(1,n), remax);
                    }
                }
                else
                {
                    
                    /*
                     * Complex right eigenvector.
                     *
                     * Initial solve
                     *     [ (T(KI-1,KI-1) T(KI-1,KI) ) - (WR + I* WI)]*X = 0.
                     *     [ (T(KI,KI-1)   T(KI,KI)   )               ]
                     */
                    if( ae_fp_greater_eq(ae_fabs(t->ptr.pp_double[ki-1][ki], _state),ae_fabs(t->ptr.pp_double[ki][ki-1], _state)) )
                    {
                        work.ptr.p_double[ki-1+n] = (double)(1);
                        work.ptr.p_double[ki+n2] = wi/t->ptr.pp_double[ki-1][ki];
                    }
                    else
                    {
                        work.ptr.p_double[ki-1+n] = -wi/t->ptr.pp_double[ki][ki-1];
                        work.ptr.p_double[ki+n2] = (double)(1);
                    }
                    work.ptr.p_double[ki+n] = (double)(0);
                    work.ptr.p_double[ki-1+n2] = (double)(0);
                    
                    /*
                     * Form right-hand side
                     */
                    for(k=1; k<=ki-2; k++)
                    {
                        work.ptr.p_double[k+n] = -work.ptr.p_double[ki-1+n]*t->ptr.pp_double[k][ki-1];
                        work.ptr.p_double[k+n2] = -work.ptr.p_double[ki+n2]*t->ptr.pp_double[k][ki];
                    }
                    
                    /*
                     * Solve upper quasi-triangular system:
                     * (T(1:KI-2,1:KI-2) - (WR+i*WI))*X = SCALE*(WORK+i*WORK2)
                     */
                    jnxt = ki-2;
                    for(j=ki-2; j>=1; j--)
                    {
                        if( j>jnxt )
                        {
                            continue;
                        }
                        j1 = j;
                        j2 = j;
                        jnxt = j-1;
                        if( j>1 )
                        {
                            if( ae_fp_neq(t->ptr.pp_double[j][j-1],(double)(0)) )
                            {
                                j1 = j-1;
                                jnxt = j-2;
                            }
                        }
                        if( j1==j2 )
                        {
                            
                            /*
                             * 1-by-1 diagonal block
                             */
                            temp11.ptr.pp_double[1][1] = t->ptr.pp_double[j][j];
                            temp12b.ptr.pp_double[1][1] = work.ptr.p_double[j+n];
                            temp12b.ptr.pp_double[1][2] = work.ptr.p_double[j+n+n];
                            evd_internalhsevdlaln2(ae_false, 1, 2, smin, 1.0, &temp11, 1.0, 1.0, &temp12b, wr, wi, &rswap4, &zswap4, &ipivot44, &civ4, &crv4, &x, &scl, &xnorm, &ierr, _state);
                            
                            /*
                             * Scale X(1,1) and X(1,2) to avoid overflow when
                             * updating the right-hand side.
                             */
                            if( ae_fp_greater(xnorm,(double)(1)) )
                            {
                                if( ae_fp_greater(work.ptr.p_double[j],bignum/xnorm) )
                                {
                                    x.ptr.pp_double[1][1] = x.ptr.pp_double[1][1]/xnorm;
                                    x.ptr.pp_double[1][2] = x.ptr.pp_double[1][2]/xnorm;
                                    scl = scl/xnorm;
                                }
                            }
                            
                            /*
                             * Scale if necessary
                             */
                            if( ae_fp_neq(scl,(double)(1)) )
                            {
                                k1 = 1+n;
                                k2 = ki+n;
                                ae_v_muld(&work.ptr.p_double[k1], 1, ae_v_len(k1,k2), scl);
                                k1 = 1+n2;
                                k2 = ki+n2;
                                ae_v_muld(&work.ptr.p_double[k1], 1, ae_v_len(k1,k2), scl);
                            }
                            work.ptr.p_double[j+n] = x.ptr.pp_double[1][1];
                            work.ptr.p_double[j+n2] = x.ptr.pp_double[1][2];
                            
                            /*
                             * Update the right-hand side
                             */
                            k1 = 1+n;
                            k2 = j-1+n;
                            k3 = 1;
                            k4 = j-1;
                            vt = -x.ptr.pp_double[1][1];
                            ae_v_addd(&work.ptr.p_double[k1], 1, &t->ptr.pp_double[k3][j], t->stride, ae_v_len(k1,k2), vt);
                            k1 = 1+n2;
                            k2 = j-1+n2;
                            k3 = 1;
                            k4 = j-1;
                            vt = -x.ptr.pp_double[1][2];
                            ae_v_addd(&work.ptr.p_double[k1], 1, &t->ptr.pp_double[k3][j], t->stride, ae_v_len(k1,k2), vt);
                        }
                        else
                        {
                            
                            /*
                             * 2-by-2 diagonal block
                             */
                            temp22.ptr.pp_double[1][1] = t->ptr.pp_double[j-1][j-1];
                            temp22.ptr.pp_double[1][2] = t->ptr.pp_double[j-1][j];
                            temp22.ptr.pp_double[2][1] = t->ptr.pp_double[j][j-1];
                            temp22.ptr.pp_double[2][2] = t->ptr.pp_double[j][j];
                            temp22b.ptr.pp_double[1][1] = work.ptr.p_double[j-1+n];
                            temp22b.ptr.pp_double[1][2] = work.ptr.p_double[j-1+n+n];
                            temp22b.ptr.pp_double[2][1] = work.ptr.p_double[j+n];
                            temp22b.ptr.pp_double[2][2] = work.ptr.p_double[j+n+n];
                            evd_internalhsevdlaln2(ae_false, 2, 2, smin, 1.0, &temp22, 1.0, 1.0, &temp22b, wr, wi, &rswap4, &zswap4, &ipivot44, &civ4, &crv4, &x, &scl, &xnorm, &ierr, _state);
                            
                            /*
                             * Scale X to avoid overflow when updating
                             * the right-hand side.
                             */
                            if( ae_fp_greater(xnorm,(double)(1)) )
                            {
                                beta = ae_maxreal(work.ptr.p_double[j-1], work.ptr.p_double[j], _state);
                                if( ae_fp_greater(beta,bignum/xnorm) )
                                {
                                    rec = 1/xnorm;
                                    x.ptr.pp_double[1][1] = x.ptr.pp_double[1][1]*rec;
                                    x.ptr.pp_double[1][2] = x.ptr.pp_double[1][2]*rec;
                                    x.ptr.pp_double[2][1] = x.ptr.pp_double[2][1]*rec;
                                    x.ptr.pp_double[2][2] = x.ptr.pp_double[2][2]*rec;
                                    scl = scl*rec;
                                }
                            }
                            
                            /*
                             * Scale if necessary
                             */
                            if( ae_fp_neq(scl,(double)(1)) )
                            {
                                ae_v_muld(&work.ptr.p_double[1+n], 1, ae_v_len(1+n,ki+n), scl);
                                ae_v_muld(&work.ptr.p_double[1+n2], 1, ae_v_len(1+n2,ki+n2), scl);
                            }
                            work.ptr.p_double[j-1+n] = x.ptr.pp_double[1][1];
                            work.ptr.p_double[j+n] = x.ptr.pp_double[2][1];
                            work.ptr.p_double[j-1+n2] = x.ptr.pp_double[1][2];
                            work.ptr.p_double[j+n2] = x.ptr.pp_double[2][2];
                            
                            /*
                             * Update the right-hand side
                             */
                            vt = -x.ptr.pp_double[1][1];
                            ae_v_addd(&work.ptr.p_double[n+1], 1, &t->ptr.pp_double[1][j-1], t->stride, ae_v_len(n+1,n+j-2), vt);
                            vt = -x.ptr.pp_double[2][1];
                            ae_v_addd(&work.ptr.p_double[n+1], 1, &t->ptr.pp_double[1][j], t->stride, ae_v_len(n+1,n+j-2), vt);
                            vt = -x.ptr.pp_double[1][2];
                            ae_v_addd(&work.ptr.p_double[n2+1], 1, &t->ptr.pp_double[1][j-1], t->stride, ae_v_len(n2+1,n2+j-2), vt);
                            vt = -x.ptr.pp_double[2][2];
                            ae_v_addd(&work.ptr.p_double[n2+1], 1, &t->ptr.pp_double[1][j], t->stride, ae_v_len(n2+1,n2+j-2), vt);
                        }
                    }
                    
                    /*
                     * Copy the vector x or Q*x to VR and normalize.
                     */
                    if( !over )
                    {
                        ae_v_move(&vr->ptr.pp_double[1][iis-1], vr->stride, &work.ptr.p_double[n+1], 1, ae_v_len(1,ki));
                        ae_v_move(&vr->ptr.pp_double[1][iis], vr->stride, &work.ptr.p_double[n2+1], 1, ae_v_len(1,ki));
                        emax = (double)(0);
                        for(k=1; k<=ki; k++)
                        {
                            emax = ae_maxreal(emax, ae_fabs(vr->ptr.pp_double[k][iis-1], _state)+ae_fabs(vr->ptr.pp_double[k][iis], _state), _state);
                        }
                        remax = 1/emax;
                        ae_v_muld(&vr->ptr.pp_double[1][iis-1], vr->stride, ae_v_len(1,ki), remax);
                        ae_v_muld(&vr->ptr.pp_double[1][iis], vr->stride, ae_v_len(1,ki), remax);
                        for(k=ki+1; k<=n; k++)
                        {
                            vr->ptr.pp_double[k][iis-1] = (double)(0);
                            vr->ptr.pp_double[k][iis] = (double)(0);
                        }
                    }
                    else
                    {
                        if( ki>2 )
                        {
                            ae_v_move(&temp.ptr.p_double[1], 1, &vr->ptr.pp_double[1][ki-1], vr->stride, ae_v_len(1,n));
                            matrixvectormultiply(vr, 1, n, 1, ki-2, ae_false, &work, 1+n, ki-2+n, 1.0, &temp, 1, n, work.ptr.p_double[ki-1+n], _state);
                            ae_v_move(&vr->ptr.pp_double[1][ki-1], vr->stride, &temp.ptr.p_double[1], 1, ae_v_len(1,n));
                            ae_v_move(&temp.ptr.p_double[1], 1, &vr->ptr.pp_double[1][ki], vr->stride, ae_v_len(1,n));
                            matrixvectormultiply(vr, 1, n, 1, ki-2, ae_false, &work, 1+n2, ki-2+n2, 1.0, &temp, 1, n, work.ptr.p_double[ki+n2], _state);
                            ae_v_move(&vr->ptr.pp_double[1][ki], vr->stride, &temp.ptr.p_double[1], 1, ae_v_len(1,n));
                        }
                        else
                        {
                            vt = work.ptr.p_double[ki-1+n];
                            ae_v_muld(&vr->ptr.pp_double[1][ki-1], vr->stride, ae_v_len(1,n), vt);
                            vt = work.ptr.p_double[ki+n2];
                            ae_v_muld(&vr->ptr.pp_double[1][ki], vr->stride, ae_v_len(1,n), vt);
                        }
                        emax = (double)(0);
                        for(k=1; k<=n; k++)
                        {
                            emax = ae_maxreal(emax, ae_fabs(vr->ptr.pp_double[k][ki-1], _state)+ae_fabs(vr->ptr.pp_double[k][ki], _state), _state);
                        }
                        remax = 1/emax;
                        ae_v_muld(&vr->ptr.pp_double[1][ki-1], vr->stride, ae_v_len(1,n), remax);
                        ae_v_muld(&vr->ptr.pp_double[1][ki], vr->stride, ae_v_len(1,n), remax);
                    }
                }
                iis = iis-1;
                if( ip!=0 )
                {
                    iis = iis-1;
                }
            }
            if( ip==1 )
            {
                ip = 0;
            }
            if( ip==-1 )
            {
                ip = 1;
            }
        }
    }
    if( leftv )
    {
        
        /*
         * Compute left eigenvectors.
         */
        ip = 0;
        iis = 1;
        for(ki=1; ki<=n; ki++)
        {
            skipflag = ae_false;
            if( ip==-1 )
            {
                skipflag = ae_true;
            }
            else
            {
                if( ki!=n )
                {
                    if( ae_fp_neq(t->ptr.pp_double[ki+1][ki],(double)(0)) )
                    {
                        ip = 1;
                    }
                }
                if( somev )
                {
                    if( !vselect->ptr.p_bool[ki] )
                    {
                        skipflag = ae_true;
                    }
                }
            }
            if( !skipflag )
            {
                
                /*
                 * Compute the KI-th eigenvalue (WR,WI).
                 */
                wr = t->ptr.pp_double[ki][ki];
                wi = (double)(0);
                if( ip!=0 )
                {
                    wi = ae_sqrt(ae_fabs(t->ptr.pp_double[ki][ki+1], _state), _state)*ae_sqrt(ae_fabs(t->ptr.pp_double[ki+1][ki], _state), _state);
                }
                smin = ae_maxreal(ulp*(ae_fabs(wr, _state)+ae_fabs(wi, _state)), smlnum, _state);
                if( ip==0 )
                {
                    
                    /*
                     * Real left eigenvector.
                     */
                    work.ptr.p_double[ki+n] = (double)(1);
                    
                    /*
                     * Form right-hand side
                     */
                    for(k=ki+1; k<=n; k++)
                    {
                        work.ptr.p_double[k+n] = -t->ptr.pp_double[ki][k];
                    }
                    
                    /*
                     * Solve the quasi-triangular system:
                     * (T(KI+1:N,KI+1:N) - WR)'*X = SCALE*WORK
                     */
                    vmax = (double)(1);
                    vcrit = bignum;
                    jnxt = ki+1;
                    for(j=ki+1; j<=n; j++)
                    {
                        if( j<jnxt )
                        {
                            continue;
                        }
                        j1 = j;
                        j2 = j;
                        jnxt = j+1;
                        if( j<n )
                        {
                            if( ae_fp_neq(t->ptr.pp_double[j+1][j],(double)(0)) )
                            {
                                j2 = j+1;
                                jnxt = j+2;
                            }
                        }
                        if( j1==j2 )
                        {
                            
                            /*
                             * 1-by-1 diagonal block
                             *
                             * Scale if necessary to avoid overflow when forming
                             * the right-hand side.
                             */
                            if( ae_fp_greater(work.ptr.p_double[j],vcrit) )
                            {
                                rec = 1/vmax;
                                ae_v_muld(&work.ptr.p_double[ki+n], 1, ae_v_len(ki+n,n+n), rec);
                                vmax = (double)(1);
                                vcrit = bignum;
                            }
                            vt = ae_v_dotproduct(&t->ptr.pp_double[ki+1][j], t->stride, &work.ptr.p_double[ki+1+n], 1, ae_v_len(ki+1,j-1));
                            work.ptr.p_double[j+n] = work.ptr.p_double[j+n]-vt;
                            
                            /*
                             * Solve (T(J,J)-WR)'*X = WORK
                             */
                            temp11.ptr.pp_double[1][1] = t->ptr.pp_double[j][j];
                            temp11b.ptr.pp_double[1][1] = work.ptr.p_double[j+n];
                            evd_internalhsevdlaln2(ae_false, 1, 1, smin, 1.0, &temp11, 1.0, 1.0, &temp11b, wr, (double)(0), &rswap4, &zswap4, &ipivot44, &civ4, &crv4, &x, &scl, &xnorm, &ierr, _state);
                            
                            /*
                             * Scale if necessary
                             */
                            if( ae_fp_neq(scl,(double)(1)) )
                            {
                                ae_v_muld(&work.ptr.p_double[ki+n], 1, ae_v_len(ki+n,n+n), scl);
                            }
                            work.ptr.p_double[j+n] = x.ptr.pp_double[1][1];
                            vmax = ae_maxreal(ae_fabs(work.ptr.p_double[j+n], _state), vmax, _state);
                            vcrit = bignum/vmax;
                        }
                        else
                        {
                            
                            /*
                             * 2-by-2 diagonal block
                             *
                             * Scale if necessary to avoid overflow when forming
                             * the right-hand side.
                             */
                            beta = ae_maxreal(work.ptr.p_double[j], work.ptr.p_double[j+1], _state);
                            if( ae_fp_greater(beta,vcrit) )
                            {
                                rec = 1/vmax;
                                ae_v_muld(&work.ptr.p_double[ki+n], 1, ae_v_len(ki+n,n+n), rec);
                                vmax = (double)(1);
                                vcrit = bignum;
                            }
                            vt = ae_v_dotproduct(&t->ptr.pp_double[ki+1][j], t->stride, &work.ptr.p_double[ki+1+n], 1, ae_v_len(ki+1,j-1));
                            work.ptr.p_double[j+n] = work.ptr.p_double[j+n]-vt;
                            vt = ae_v_dotproduct(&t->ptr.pp_double[ki+1][j+1], t->stride, &work.ptr.p_double[ki+1+n], 1, ae_v_len(ki+1,j-1));
                            work.ptr.p_double[j+1+n] = work.ptr.p_double[j+1+n]-vt;
                            
                            /*
                             * Solve
                             *    [T(J,J)-WR   T(J,J+1)     ]'* X = SCALE*( WORK1 )
                             *    [T(J+1,J)    T(J+1,J+1)-WR]             ( WORK2 )
                             */
                            temp22.ptr.pp_double[1][1] = t->ptr.pp_double[j][j];
                            temp22.ptr.pp_double[1][2] = t->ptr.pp_double[j][j+1];
                            temp22.ptr.pp_double[2][1] = t->ptr.pp_double[j+1][j];
                            temp22.ptr.pp_double[2][2] = t->ptr.pp_double[j+1][j+1];
                            temp21b.ptr.pp_double[1][1] = work.ptr.p_double[j+n];
                            temp21b.ptr.pp_double[2][1] = work.ptr.p_double[j+1+n];
                            evd_internalhsevdlaln2(ae_true, 2, 1, smin, 1.0, &temp22, 1.0, 1.0, &temp21b, wr, (double)(0), &rswap4, &zswap4, &ipivot44, &civ4, &crv4, &x, &scl, &xnorm, &ierr, _state);
                            
                            /*
                             * Scale if necessary
                             */
                            if( ae_fp_neq(scl,(double)(1)) )
                            {
                                ae_v_muld(&work.ptr.p_double[ki+n], 1, ae_v_len(ki+n,n+n), scl);
                            }
                            work.ptr.p_double[j+n] = x.ptr.pp_double[1][1];
                            work.ptr.p_double[j+1+n] = x.ptr.pp_double[2][1];
                            vmax = ae_maxreal(ae_fabs(work.ptr.p_double[j+n], _state), ae_maxreal(ae_fabs(work.ptr.p_double[j+1+n], _state), vmax, _state), _state);
                            vcrit = bignum/vmax;
                        }
                    }
                    
                    /*
                     * Copy the vector x or Q*x to VL and normalize.
                     */
                    if( !over )
                    {
                        ae_v_move(&vl->ptr.pp_double[ki][iis], vl->stride, &work.ptr.p_double[ki+n], 1, ae_v_len(ki,n));
                        ii = columnidxabsmax(vl, ki, n, iis, _state);
                        remax = 1/ae_fabs(vl->ptr.pp_double[ii][iis], _state);
                        ae_v_muld(&vl->ptr.pp_double[ki][iis], vl->stride, ae_v_len(ki,n), remax);
                        for(k=1; k<=ki-1; k++)
                        {
                            vl->ptr.pp_double[k][iis] = (double)(0);
                        }
                    }
                    else
                    {
                        if( ki<n )
                        {
                            ae_v_move(&temp.ptr.p_double[1], 1, &vl->ptr.pp_double[1][ki], vl->stride, ae_v_len(1,n));
                            matrixvectormultiply(vl, 1, n, ki+1, n, ae_false, &work, ki+1+n, n+n, 1.0, &temp, 1, n, work.ptr.p_double[ki+n], _state);
                            ae_v_move(&vl->ptr.pp_double[1][ki], vl->stride, &temp.ptr.p_double[1], 1, ae_v_len(1,n));
                        }
                        ii = columnidxabsmax(vl, 1, n, ki, _state);
                        remax = 1/ae_fabs(vl->ptr.pp_double[ii][ki], _state);
                        ae_v_muld(&vl->ptr.pp_double[1][ki], vl->stride, ae_v_len(1,n), remax);
                    }
                }
                else
                {
                    
                    /*
                     * Complex left eigenvector.
                     *
                     * Initial solve:
                     *   ((T(KI,KI)    T(KI,KI+1) )' - (WR - I* WI))*X = 0.
                     *   ((T(KI+1,KI) T(KI+1,KI+1))                )
                     */
                    if( ae_fp_greater_eq(ae_fabs(t->ptr.pp_double[ki][ki+1], _state),ae_fabs(t->ptr.pp_double[ki+1][ki], _state)) )
                    {
                        work.ptr.p_double[ki+n] = wi/t->ptr.pp_double[ki][ki+1];
                        work.ptr.p_double[ki+1+n2] = (double)(1);
                    }
                    else
                    {
                        work.ptr.p_double[ki+n] = (double)(1);
                        work.ptr.p_double[ki+1+n2] = -wi/t->ptr.pp_double[ki+1][ki];
                    }
                    work.ptr.p_double[ki+1+n] = (double)(0);
                    work.ptr.p_double[ki+n2] = (double)(0);
                    
                    /*
                     * Form right-hand side
                     */
                    for(k=ki+2; k<=n; k++)
                    {
                        work.ptr.p_double[k+n] = -work.ptr.p_double[ki+n]*t->ptr.pp_double[ki][k];
                        work.ptr.p_double[k+n2] = -work.ptr.p_double[ki+1+n2]*t->ptr.pp_double[ki+1][k];
                    }
                    
                    /*
                     * Solve complex quasi-triangular system:
                     * ( T(KI+2,N:KI+2,N) - (WR-i*WI) )*X = WORK1+i*WORK2
                     */
                    vmax = (double)(1);
                    vcrit = bignum;
                    jnxt = ki+2;
                    for(j=ki+2; j<=n; j++)
                    {
                        if( j<jnxt )
                        {
                            continue;
                        }
                        j1 = j;
                        j2 = j;
                        jnxt = j+1;
                        if( j<n )
                        {
                            if( ae_fp_neq(t->ptr.pp_double[j+1][j],(double)(0)) )
                            {
                                j2 = j+1;
                                jnxt = j+2;
                            }
                        }
                        if( j1==j2 )
                        {
                            
                            /*
                             * 1-by-1 diagonal block
                             *
                             * Scale if necessary to avoid overflow when
                             * forming the right-hand side elements.
                             */
                            if( ae_fp_greater(work.ptr.p_double[j],vcrit) )
                            {
                                rec = 1/vmax;
                                ae_v_muld(&work.ptr.p_double[ki+n], 1, ae_v_len(ki+n,n+n), rec);
                                ae_v_muld(&work.ptr.p_double[ki+n2], 1, ae_v_len(ki+n2,n+n2), rec);
                                vmax = (double)(1);
                                vcrit = bignum;
                            }
                            vt = ae_v_dotproduct(&t->ptr.pp_double[ki+2][j], t->stride, &work.ptr.p_double[ki+2+n], 1, ae_v_len(ki+2,j-1));
                            work.ptr.p_double[j+n] = work.ptr.p_double[j+n]-vt;
                            vt = ae_v_dotproduct(&t->ptr.pp_double[ki+2][j], t->stride, &work.ptr.p_double[ki+2+n2], 1, ae_v_len(ki+2,j-1));
                            work.ptr.p_double[j+n2] = work.ptr.p_double[j+n2]-vt;
                            
                            /*
                             * Solve (T(J,J)-(WR-i*WI))*(X11+i*X12)= WK+I*WK2
                             */
                            temp11.ptr.pp_double[1][1] = t->ptr.pp_double[j][j];
                            temp12b.ptr.pp_double[1][1] = work.ptr.p_double[j+n];
                            temp12b.ptr.pp_double[1][2] = work.ptr.p_double[j+n+n];
                            evd_internalhsevdlaln2(ae_false, 1, 2, smin, 1.0, &temp11, 1.0, 1.0, &temp12b, wr, -wi, &rswap4, &zswap4, &ipivot44, &civ4, &crv4, &x, &scl, &xnorm, &ierr, _state);
                            
                            /*
                             * Scale if necessary
                             */
                            if( ae_fp_neq(scl,(double)(1)) )
                            {
                                ae_v_muld(&work.ptr.p_double[ki+n], 1, ae_v_len(ki+n,n+n), scl);
                                ae_v_muld(&work.ptr.p_double[ki+n2], 1, ae_v_len(ki+n2,n+n2), scl);
                            }
                            work.ptr.p_double[j+n] = x.ptr.pp_double[1][1];
                            work.ptr.p_double[j+n2] = x.ptr.pp_double[1][2];
                            vmax = ae_maxreal(ae_fabs(work.ptr.p_double[j+n], _state), ae_maxreal(ae_fabs(work.ptr.p_double[j+n2], _state), vmax, _state), _state);
                            vcrit = bignum/vmax;
                        }
                        else
                        {
                            
                            /*
                             * 2-by-2 diagonal block
                             *
                             * Scale if necessary to avoid overflow when forming
                             * the right-hand side elements.
                             */
                            beta = ae_maxreal(work.ptr.p_double[j], work.ptr.p_double[j+1], _state);
                            if( ae_fp_greater(beta,vcrit) )
                            {
                                rec = 1/vmax;
                                ae_v_muld(&work.ptr.p_double[ki+n], 1, ae_v_len(ki+n,n+n), rec);
                                ae_v_muld(&work.ptr.p_double[ki+n2], 1, ae_v_len(ki+n2,n+n2), rec);
                                vmax = (double)(1);
                                vcrit = bignum;
                            }
                            vt = ae_v_dotproduct(&t->ptr.pp_double[ki+2][j], t->stride, &work.ptr.p_double[ki+2+n], 1, ae_v_len(ki+2,j-1));
                            work.ptr.p_double[j+n] = work.ptr.p_double[j+n]-vt;
                            vt = ae_v_dotproduct(&t->ptr.pp_double[ki+2][j], t->stride, &work.ptr.p_double[ki+2+n2], 1, ae_v_len(ki+2,j-1));
                            work.ptr.p_double[j+n2] = work.ptr.p_double[j+n2]-vt;
                            vt = ae_v_dotproduct(&t->ptr.pp_double[ki+2][j+1], t->stride, &work.ptr.p_double[ki+2+n], 1, ae_v_len(ki+2,j-1));
                            work.ptr.p_double[j+1+n] = work.ptr.p_double[j+1+n]-vt;
                            vt = ae_v_dotproduct(&t->ptr.pp_double[ki+2][j+1], t->stride, &work.ptr.p_double[ki+2+n2], 1, ae_v_len(ki+2,j-1));
                            work.ptr.p_double[j+1+n2] = work.ptr.p_double[j+1+n2]-vt;
                            
                            /*
                             * Solve 2-by-2 complex linear equation
                             *   ([T(j,j)   T(j,j+1)  ]'-(wr-i*wi)*I)*X = SCALE*B
                             *   ([T(j+1,j) T(j+1,j+1)]             )
                             */
                            temp22.ptr.pp_double[1][1] = t->ptr.pp_double[j][j];
                            temp22.ptr.pp_double[1][2] = t->ptr.pp_double[j][j+1];
                            temp22.ptr.pp_double[2][1] = t->ptr.pp_double[j+1][j];
                            temp22.ptr.pp_double[2][2] = t->ptr.pp_double[j+1][j+1];
                            temp22b.ptr.pp_double[1][1] = work.ptr.p_double[j+n];
                            temp22b.ptr.pp_double[1][2] = work.ptr.p_double[j+n+n];
                            temp22b.ptr.pp_double[2][1] = work.ptr.p_double[j+1+n];
                            temp22b.ptr.pp_double[2][2] = work.ptr.p_double[j+1+n+n];
                            evd_internalhsevdlaln2(ae_true, 2, 2, smin, 1.0, &temp22, 1.0, 1.0, &temp22b, wr, -wi, &rswap4, &zswap4, &ipivot44, &civ4, &crv4, &x, &scl, &xnorm, &ierr, _state);
                            
                            /*
                             * Scale if necessary
                             */
                            if( ae_fp_neq(scl,(double)(1)) )
                            {
                                ae_v_muld(&work.ptr.p_double[ki+n], 1, ae_v_len(ki+n,n+n), scl);
                                ae_v_muld(&work.ptr.p_double[ki+n2], 1, ae_v_len(ki+n2,n+n2), scl);
                            }
                            work.ptr.p_double[j+n] = x.ptr.pp_double[1][1];
                            work.ptr.p_double[j+n2] = x.ptr.pp_double[1][2];
                            work.ptr.p_double[j+1+n] = x.ptr.pp_double[2][1];
                            work.ptr.p_double[j+1+n2] = x.ptr.pp_double[2][2];
                            vmax = ae_maxreal(ae_fabs(x.ptr.pp_double[1][1], _state), vmax, _state);
                            vmax = ae_maxreal(ae_fabs(x.ptr.pp_double[1][2], _state), vmax, _state);
                            vmax = ae_maxreal(ae_fabs(x.ptr.pp_double[2][1], _state), vmax, _state);
                            vmax = ae_maxreal(ae_fabs(x.ptr.pp_double[2][2], _state), vmax, _state);
                            vcrit = bignum/vmax;
                        }
                    }
                    
                    /*
                     * Copy the vector x or Q*x to VL and normalize.
                     */
                    if( !over )
                    {
                        ae_v_move(&vl->ptr.pp_double[ki][iis], vl->stride, &work.ptr.p_double[ki+n], 1, ae_v_len(ki,n));
                        ae_v_move(&vl->ptr.pp_double[ki][iis+1], vl->stride, &work.ptr.p_double[ki+n2], 1, ae_v_len(ki,n));
                        emax = (double)(0);
                        for(k=ki; k<=n; k++)
                        {
                            emax = ae_maxreal(emax, ae_fabs(vl->ptr.pp_double[k][iis], _state)+ae_fabs(vl->ptr.pp_double[k][iis+1], _state), _state);
                        }
                        remax = 1/emax;
                        ae_v_muld(&vl->ptr.pp_double[ki][iis], vl->stride, ae_v_len(ki,n), remax);
                        ae_v_muld(&vl->ptr.pp_double[ki][iis+1], vl->stride, ae_v_len(ki,n), remax);
                        for(k=1; k<=ki-1; k++)
                        {
                            vl->ptr.pp_double[k][iis] = (double)(0);
                            vl->ptr.pp_double[k][iis+1] = (double)(0);
                        }
                    }
                    else
                    {
                        if( ki<n-1 )
                        {
                            ae_v_move(&temp.ptr.p_double[1], 1, &vl->ptr.pp_double[1][ki], vl->stride, ae_v_len(1,n));
                            matrixvectormultiply(vl, 1, n, ki+2, n, ae_false, &work, ki+2+n, n+n, 1.0, &temp, 1, n, work.ptr.p_double[ki+n], _state);
                            ae_v_move(&vl->ptr.pp_double[1][ki], vl->stride, &temp.ptr.p_double[1], 1, ae_v_len(1,n));
                            ae_v_move(&temp.ptr.p_double[1], 1, &vl->ptr.pp_double[1][ki+1], vl->stride, ae_v_len(1,n));
                            matrixvectormultiply(vl, 1, n, ki+2, n, ae_false, &work, ki+2+n2, n+n2, 1.0, &temp, 1, n, work.ptr.p_double[ki+1+n2], _state);
                            ae_v_move(&vl->ptr.pp_double[1][ki+1], vl->stride, &temp.ptr.p_double[1], 1, ae_v_len(1,n));
                        }
                        else
                        {
                            vt = work.ptr.p_double[ki+n];
                            ae_v_muld(&vl->ptr.pp_double[1][ki], vl->stride, ae_v_len(1,n), vt);
                            vt = work.ptr.p_double[ki+1+n2];
                            ae_v_muld(&vl->ptr.pp_double[1][ki+1], vl->stride, ae_v_len(1,n), vt);
                        }
                        emax = (double)(0);
                        for(k=1; k<=n; k++)
                        {
                            emax = ae_maxreal(emax, ae_fabs(vl->ptr.pp_double[k][ki], _state)+ae_fabs(vl->ptr.pp_double[k][ki+1], _state), _state);
                        }
                        remax = 1/emax;
                        ae_v_muld(&vl->ptr.pp_double[1][ki], vl->stride, ae_v_len(1,n), remax);
                        ae_v_muld(&vl->ptr.pp_double[1][ki+1], vl->stride, ae_v_len(1,n), remax);
                    }
                }
                iis = iis+1;
                if( ip!=0 )
                {
                    iis = iis+1;
                }
            }
            if( ip==-1 )
            {
                ip = 0;
            }
            if( ip==1 )
            {
                ip = -1;
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
DLALN2 solves a system of the form  (ca A - w D ) X = s B
or (ca A' - w D) X = s B   with possible scaling ("s") and
perturbation of A.  (A' means A-transpose.)

A is an NA x NA real matrix, ca is a real scalar, D is an NA x NA
real diagonal matrix, w is a real or complex value, and X and B are
NA x 1 matrices -- real if w is real, complex if w is complex.  NA
may be 1 or 2.

If w is complex, X and B are represented as NA x 2 matrices,
the first column of each being the real part and the second
being the imaginary part.

"s" is a scaling factor (.LE. 1), computed by DLALN2, which is
so chosen that X can be computed without overflow.  X is further
scaled if necessary to assure that norm(ca A - w D)*norm(X) is less
than overflow.

If both singular values of (ca A - w D) are less than SMIN,
SMIN*identity will be used instead of (ca A - w D).  If only one
singular value is less than SMIN, one element of (ca A - w D) will be
perturbed enough to make the smallest singular value roughly SMIN.
If both singular values are at least SMIN, (ca A - w D) will not be
perturbed.  In any case, the perturbation will be at most some small
multiple of max( SMIN, ulp*norm(ca A - w D) ).  The singular values
are computed by infinity-norm approximations, and thus will only be
correct to a factor of 2 or so.

Note: all input quantities are assumed to be smaller than overflow
by a reasonable factor.  (See BIGNUM.)

  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     October 31, 1992
*************************************************************************/
static void evd_internalhsevdlaln2(ae_bool ltrans,
     ae_int_t na,
     ae_int_t nw,
     double smin,
     double ca,
     /* Real    */ ae_matrix* a,
     double d1,
     double d2,
     /* Real    */ ae_matrix* b,
     double wr,
     double wi,
     /* Boolean */ ae_vector* rswap4,
     /* Boolean */ ae_vector* zswap4,
     /* Integer */ ae_matrix* ipivot44,
     /* Real    */ ae_vector* civ4,
     /* Real    */ ae_vector* crv4,
     /* Real    */ ae_matrix* x,
     double* scl,
     double* xnorm,
     ae_int_t* info,
     ae_state *_state)
{
    ae_int_t icmax;
    ae_int_t j;
    double bbnd;
    double bi1;
    double bi2;
    double bignum;
    double bnorm;
    double br1;
    double br2;
    double ci21;
    double ci22;
    double cmax;
    double cnorm;
    double cr21;
    double cr22;
    double csi;
    double csr;
    double li21;
    double lr21;
    double smini;
    double smlnum;
    double temp;
    double u22abs;
    double ui11;
    double ui11r;
    double ui12;
    double ui12s;
    double ui22;
    double ur11;
    double ur11r;
    double ur12;
    double ur12s;
    double ur22;
    double xi1;
    double xi2;
    double xr1;
    double xr2;
    double tmp1;
    double tmp2;

    *scl = 0;
    *xnorm = 0;
    *info = 0;

    zswap4->ptr.p_bool[1] = ae_false;
    zswap4->ptr.p_bool[2] = ae_false;
    zswap4->ptr.p_bool[3] = ae_true;
    zswap4->ptr.p_bool[4] = ae_true;
    rswap4->ptr.p_bool[1] = ae_false;
    rswap4->ptr.p_bool[2] = ae_true;
    rswap4->ptr.p_bool[3] = ae_false;
    rswap4->ptr.p_bool[4] = ae_true;
    ipivot44->ptr.pp_int[1][1] = 1;
    ipivot44->ptr.pp_int[2][1] = 2;
    ipivot44->ptr.pp_int[3][1] = 3;
    ipivot44->ptr.pp_int[4][1] = 4;
    ipivot44->ptr.pp_int[1][2] = 2;
    ipivot44->ptr.pp_int[2][2] = 1;
    ipivot44->ptr.pp_int[3][2] = 4;
    ipivot44->ptr.pp_int[4][2] = 3;
    ipivot44->ptr.pp_int[1][3] = 3;
    ipivot44->ptr.pp_int[2][3] = 4;
    ipivot44->ptr.pp_int[3][3] = 1;
    ipivot44->ptr.pp_int[4][3] = 2;
    ipivot44->ptr.pp_int[1][4] = 4;
    ipivot44->ptr.pp_int[2][4] = 3;
    ipivot44->ptr.pp_int[3][4] = 2;
    ipivot44->ptr.pp_int[4][4] = 1;
    smlnum = 2*ae_minrealnumber;
    bignum = 1/smlnum;
    smini = ae_maxreal(smin, smlnum, _state);
    
    /*
     * Don't check for input errors
     */
    *info = 0;
    
    /*
     * Standard Initializations
     */
    *scl = (double)(1);
    if( na==1 )
    {
        
        /*
         * 1 x 1  (i.e., scalar) system   C X = B
         */
        if( nw==1 )
        {
            
            /*
             * Real 1x1 system.
             *
             * C = ca A - w D
             */
            csr = ca*a->ptr.pp_double[1][1]-wr*d1;
            cnorm = ae_fabs(csr, _state);
            
            /*
             * If | C | < SMINI, use C = SMINI
             */
            if( ae_fp_less(cnorm,smini) )
            {
                csr = smini;
                cnorm = smini;
                *info = 1;
            }
            
            /*
             * Check scaling for  X = B / C
             */
            bnorm = ae_fabs(b->ptr.pp_double[1][1], _state);
            if( ae_fp_less(cnorm,(double)(1))&&ae_fp_greater(bnorm,(double)(1)) )
            {
                if( ae_fp_greater(bnorm,bignum*cnorm) )
                {
                    *scl = 1/bnorm;
                }
            }
            
            /*
             * Compute X
             */
            x->ptr.pp_double[1][1] = b->ptr.pp_double[1][1]*(*scl)/csr;
            *xnorm = ae_fabs(x->ptr.pp_double[1][1], _state);
        }
        else
        {
            
            /*
             * Complex 1x1 system (w is complex)
             *
             * C = ca A - w D
             */
            csr = ca*a->ptr.pp_double[1][1]-wr*d1;
            csi = -wi*d1;
            cnorm = ae_fabs(csr, _state)+ae_fabs(csi, _state);
            
            /*
             * If | C | < SMINI, use C = SMINI
             */
            if( ae_fp_less(cnorm,smini) )
            {
                csr = smini;
                csi = (double)(0);
                cnorm = smini;
                *info = 1;
            }
            
            /*
             * Check scaling for  X = B / C
             */
            bnorm = ae_fabs(b->ptr.pp_double[1][1], _state)+ae_fabs(b->ptr.pp_double[1][2], _state);
            if( ae_fp_less(cnorm,(double)(1))&&ae_fp_greater(bnorm,(double)(1)) )
            {
                if( ae_fp_greater(bnorm,bignum*cnorm) )
                {
                    *scl = 1/bnorm;
                }
            }
            
            /*
             * Compute X
             */
            evd_internalhsevdladiv(*scl*b->ptr.pp_double[1][1], *scl*b->ptr.pp_double[1][2], csr, csi, &tmp1, &tmp2, _state);
            x->ptr.pp_double[1][1] = tmp1;
            x->ptr.pp_double[1][2] = tmp2;
            *xnorm = ae_fabs(x->ptr.pp_double[1][1], _state)+ae_fabs(x->ptr.pp_double[1][2], _state);
        }
    }
    else
    {
        
        /*
         * 2x2 System
         *
         * Compute the real part of  C = ca A - w D  (or  ca A' - w D )
         */
        crv4->ptr.p_double[1+0] = ca*a->ptr.pp_double[1][1]-wr*d1;
        crv4->ptr.p_double[2+2] = ca*a->ptr.pp_double[2][2]-wr*d2;
        if( ltrans )
        {
            crv4->ptr.p_double[1+2] = ca*a->ptr.pp_double[2][1];
            crv4->ptr.p_double[2+0] = ca*a->ptr.pp_double[1][2];
        }
        else
        {
            crv4->ptr.p_double[2+0] = ca*a->ptr.pp_double[2][1];
            crv4->ptr.p_double[1+2] = ca*a->ptr.pp_double[1][2];
        }
        if( nw==1 )
        {
            
            /*
             * Real 2x2 system  (w is real)
             *
             * Find the largest element in C
             */
            cmax = (double)(0);
            icmax = 0;
            for(j=1; j<=4; j++)
            {
                if( ae_fp_greater(ae_fabs(crv4->ptr.p_double[j], _state),cmax) )
                {
                    cmax = ae_fabs(crv4->ptr.p_double[j], _state);
                    icmax = j;
                }
            }
            
            /*
             * If norm(C) < SMINI, use SMINI*identity.
             */
            if( ae_fp_less(cmax,smini) )
            {
                bnorm = ae_maxreal(ae_fabs(b->ptr.pp_double[1][1], _state), ae_fabs(b->ptr.pp_double[2][1], _state), _state);
                if( ae_fp_less(smini,(double)(1))&&ae_fp_greater(bnorm,(double)(1)) )
                {
                    if( ae_fp_greater(bnorm,bignum*smini) )
                    {
                        *scl = 1/bnorm;
                    }
                }
                temp = *scl/smini;
                x->ptr.pp_double[1][1] = temp*b->ptr.pp_double[1][1];
                x->ptr.pp_double[2][1] = temp*b->ptr.pp_double[2][1];
                *xnorm = temp*bnorm;
                *info = 1;
                return;
            }
            
            /*
             * Gaussian elimination with complete pivoting.
             */
            ur11 = crv4->ptr.p_double[icmax];
            cr21 = crv4->ptr.p_double[ipivot44->ptr.pp_int[2][icmax]];
            ur12 = crv4->ptr.p_double[ipivot44->ptr.pp_int[3][icmax]];
            cr22 = crv4->ptr.p_double[ipivot44->ptr.pp_int[4][icmax]];
            ur11r = 1/ur11;
            lr21 = ur11r*cr21;
            ur22 = cr22-ur12*lr21;
            
            /*
             * If smaller pivot < SMINI, use SMINI
             */
            if( ae_fp_less(ae_fabs(ur22, _state),smini) )
            {
                ur22 = smini;
                *info = 1;
            }
            if( rswap4->ptr.p_bool[icmax] )
            {
                br1 = b->ptr.pp_double[2][1];
                br2 = b->ptr.pp_double[1][1];
            }
            else
            {
                br1 = b->ptr.pp_double[1][1];
                br2 = b->ptr.pp_double[2][1];
            }
            br2 = br2-lr21*br1;
            bbnd = ae_maxreal(ae_fabs(br1*(ur22*ur11r), _state), ae_fabs(br2, _state), _state);
            if( ae_fp_greater(bbnd,(double)(1))&&ae_fp_less(ae_fabs(ur22, _state),(double)(1)) )
            {
                if( ae_fp_greater_eq(bbnd,bignum*ae_fabs(ur22, _state)) )
                {
                    *scl = 1/bbnd;
                }
            }
            xr2 = br2*(*scl)/ur22;
            xr1 = *scl*br1*ur11r-xr2*(ur11r*ur12);
            if( zswap4->ptr.p_bool[icmax] )
            {
                x->ptr.pp_double[1][1] = xr2;
                x->ptr.pp_double[2][1] = xr1;
            }
            else
            {
                x->ptr.pp_double[1][1] = xr1;
                x->ptr.pp_double[2][1] = xr2;
            }
            *xnorm = ae_maxreal(ae_fabs(xr1, _state), ae_fabs(xr2, _state), _state);
            
            /*
             * Further scaling if  norm(A) norm(X) > overflow
             */
            if( ae_fp_greater(*xnorm,(double)(1))&&ae_fp_greater(cmax,(double)(1)) )
            {
                if( ae_fp_greater(*xnorm,bignum/cmax) )
                {
                    temp = cmax/bignum;
                    x->ptr.pp_double[1][1] = temp*x->ptr.pp_double[1][1];
                    x->ptr.pp_double[2][1] = temp*x->ptr.pp_double[2][1];
                    *xnorm = temp*(*xnorm);
                    *scl = temp*(*scl);
                }
            }
        }
        else
        {
            
            /*
             * Complex 2x2 system  (w is complex)
             *
             * Find the largest element in C
             */
            civ4->ptr.p_double[1+0] = -wi*d1;
            civ4->ptr.p_double[2+0] = (double)(0);
            civ4->ptr.p_double[1+2] = (double)(0);
            civ4->ptr.p_double[2+2] = -wi*d2;
            cmax = (double)(0);
            icmax = 0;
            for(j=1; j<=4; j++)
            {
                if( ae_fp_greater(ae_fabs(crv4->ptr.p_double[j], _state)+ae_fabs(civ4->ptr.p_double[j], _state),cmax) )
                {
                    cmax = ae_fabs(crv4->ptr.p_double[j], _state)+ae_fabs(civ4->ptr.p_double[j], _state);
                    icmax = j;
                }
            }
            
            /*
             * If norm(C) < SMINI, use SMINI*identity.
             */
            if( ae_fp_less(cmax,smini) )
            {
                bnorm = ae_maxreal(ae_fabs(b->ptr.pp_double[1][1], _state)+ae_fabs(b->ptr.pp_double[1][2], _state), ae_fabs(b->ptr.pp_double[2][1], _state)+ae_fabs(b->ptr.pp_double[2][2], _state), _state);
                if( ae_fp_less(smini,(double)(1))&&ae_fp_greater(bnorm,(double)(1)) )
                {
                    if( ae_fp_greater(bnorm,bignum*smini) )
                    {
                        *scl = 1/bnorm;
                    }
                }
                temp = *scl/smini;
                x->ptr.pp_double[1][1] = temp*b->ptr.pp_double[1][1];
                x->ptr.pp_double[2][1] = temp*b->ptr.pp_double[2][1];
                x->ptr.pp_double[1][2] = temp*b->ptr.pp_double[1][2];
                x->ptr.pp_double[2][2] = temp*b->ptr.pp_double[2][2];
                *xnorm = temp*bnorm;
                *info = 1;
                return;
            }
            
            /*
             * Gaussian elimination with complete pivoting.
             */
            ur11 = crv4->ptr.p_double[icmax];
            ui11 = civ4->ptr.p_double[icmax];
            cr21 = crv4->ptr.p_double[ipivot44->ptr.pp_int[2][icmax]];
            ci21 = civ4->ptr.p_double[ipivot44->ptr.pp_int[2][icmax]];
            ur12 = crv4->ptr.p_double[ipivot44->ptr.pp_int[3][icmax]];
            ui12 = civ4->ptr.p_double[ipivot44->ptr.pp_int[3][icmax]];
            cr22 = crv4->ptr.p_double[ipivot44->ptr.pp_int[4][icmax]];
            ci22 = civ4->ptr.p_double[ipivot44->ptr.pp_int[4][icmax]];
            if( icmax==1||icmax==4 )
            {
                
                /*
                 * Code when off-diagonals of pivoted C are real
                 */
                if( ae_fp_greater(ae_fabs(ur11, _state),ae_fabs(ui11, _state)) )
                {
                    temp = ui11/ur11;
                    ur11r = 1/(ur11*(1+ae_sqr(temp, _state)));
                    ui11r = -temp*ur11r;
                }
                else
                {
                    temp = ur11/ui11;
                    ui11r = -1/(ui11*(1+ae_sqr(temp, _state)));
                    ur11r = -temp*ui11r;
                }
                lr21 = cr21*ur11r;
                li21 = cr21*ui11r;
                ur12s = ur12*ur11r;
                ui12s = ur12*ui11r;
                ur22 = cr22-ur12*lr21;
                ui22 = ci22-ur12*li21;
            }
            else
            {
                
                /*
                 * Code when diagonals of pivoted C are real
                 */
                ur11r = 1/ur11;
                ui11r = (double)(0);
                lr21 = cr21*ur11r;
                li21 = ci21*ur11r;
                ur12s = ur12*ur11r;
                ui12s = ui12*ur11r;
                ur22 = cr22-ur12*lr21+ui12*li21;
                ui22 = -ur12*li21-ui12*lr21;
            }
            u22abs = ae_fabs(ur22, _state)+ae_fabs(ui22, _state);
            
            /*
             * If smaller pivot < SMINI, use SMINI
             */
            if( ae_fp_less(u22abs,smini) )
            {
                ur22 = smini;
                ui22 = (double)(0);
                *info = 1;
            }
            if( rswap4->ptr.p_bool[icmax] )
            {
                br2 = b->ptr.pp_double[1][1];
                br1 = b->ptr.pp_double[2][1];
                bi2 = b->ptr.pp_double[1][2];
                bi1 = b->ptr.pp_double[2][2];
            }
            else
            {
                br1 = b->ptr.pp_double[1][1];
                br2 = b->ptr.pp_double[2][1];
                bi1 = b->ptr.pp_double[1][2];
                bi2 = b->ptr.pp_double[2][2];
            }
            br2 = br2-lr21*br1+li21*bi1;
            bi2 = bi2-li21*br1-lr21*bi1;
            bbnd = ae_maxreal((ae_fabs(br1, _state)+ae_fabs(bi1, _state))*(u22abs*(ae_fabs(ur11r, _state)+ae_fabs(ui11r, _state))), ae_fabs(br2, _state)+ae_fabs(bi2, _state), _state);
            if( ae_fp_greater(bbnd,(double)(1))&&ae_fp_less(u22abs,(double)(1)) )
            {
                if( ae_fp_greater_eq(bbnd,bignum*u22abs) )
                {
                    *scl = 1/bbnd;
                    br1 = *scl*br1;
                    bi1 = *scl*bi1;
                    br2 = *scl*br2;
                    bi2 = *scl*bi2;
                }
            }
            evd_internalhsevdladiv(br2, bi2, ur22, ui22, &xr2, &xi2, _state);
            xr1 = ur11r*br1-ui11r*bi1-ur12s*xr2+ui12s*xi2;
            xi1 = ui11r*br1+ur11r*bi1-ui12s*xr2-ur12s*xi2;
            if( zswap4->ptr.p_bool[icmax] )
            {
                x->ptr.pp_double[1][1] = xr2;
                x->ptr.pp_double[2][1] = xr1;
                x->ptr.pp_double[1][2] = xi2;
                x->ptr.pp_double[2][2] = xi1;
            }
            else
            {
                x->ptr.pp_double[1][1] = xr1;
                x->ptr.pp_double[2][1] = xr2;
                x->ptr.pp_double[1][2] = xi1;
                x->ptr.pp_double[2][2] = xi2;
            }
            *xnorm = ae_maxreal(ae_fabs(xr1, _state)+ae_fabs(xi1, _state), ae_fabs(xr2, _state)+ae_fabs(xi2, _state), _state);
            
            /*
             * Further scaling if  norm(A) norm(X) > overflow
             */
            if( ae_fp_greater(*xnorm,(double)(1))&&ae_fp_greater(cmax,(double)(1)) )
            {
                if( ae_fp_greater(*xnorm,bignum/cmax) )
                {
                    temp = cmax/bignum;
                    x->ptr.pp_double[1][1] = temp*x->ptr.pp_double[1][1];
                    x->ptr.pp_double[2][1] = temp*x->ptr.pp_double[2][1];
                    x->ptr.pp_double[1][2] = temp*x->ptr.pp_double[1][2];
                    x->ptr.pp_double[2][2] = temp*x->ptr.pp_double[2][2];
                    *xnorm = temp*(*xnorm);
                    *scl = temp*(*scl);
                }
            }
        }
    }
}


/*************************************************************************
performs complex division in  real arithmetic

                        a + i*b
             p + i*q = ---------
                        c + i*d

The algorithm is due to Robert L. Smith and can be found
in D. Knuth, The art of Computer Programming, Vol.2, p.195

  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     October 31, 1992
*************************************************************************/
static void evd_internalhsevdladiv(double a,
     double b,
     double c,
     double d,
     double* p,
     double* q,
     ae_state *_state)
{
    double e;
    double f;

    *p = 0;
    *q = 0;

    if( ae_fp_less(ae_fabs(d, _state),ae_fabs(c, _state)) )
    {
        e = d/c;
        f = c+d*e;
        *p = (a+b*e)/f;
        *q = (b-a*e)/f;
    }
    else
    {
        e = c/d;
        f = d+c*e;
        *p = (b+a*e)/f;
        *q = (-a+b*e)/f;
    }
}


/*$ End $*/
