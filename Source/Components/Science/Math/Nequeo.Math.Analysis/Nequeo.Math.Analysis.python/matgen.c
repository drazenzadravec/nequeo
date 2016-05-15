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
#include "matgen.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Generation of a random uniformly distributed (Haar) orthogonal matrix

INPUT PARAMETERS:
    N   -   matrix size, N>=1
    
OUTPUT PARAMETERS:
    A   -   orthogonal NxN matrix, array[0..N-1,0..N-1]

NOTE: this function uses algorithm  described  in  Stewart, G. W.  (1980),
      "The Efficient Generation of  Random  Orthogonal  Matrices  with  an
      Application to Condition Estimators".
      
      Speaking short, to generate an (N+1)x(N+1) orthogonal matrix, it:
      * takes an NxN one
      * takes uniformly distributed unit vector of dimension N+1.
      * constructs a Householder reflection from the vector, then applies
        it to the smaller matrix (embedded in the larger size with a 1 at
        the bottom right corner).

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void rmatrixrndorthogonal(ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(a);

    ae_assert(n>=1, "RMatrixRndOrthogonal: N<1!", _state);
    ae_matrix_set_length(a, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i==j )
            {
                a->ptr.pp_double[i][j] = (double)(1);
            }
            else
            {
                a->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    rmatrixrndorthogonalfromtheright(a, n, n, _state);
}


/*************************************************************************
Generation of random NxN matrix with given condition number and norm2(A)=1

INPUT PARAMETERS:
    N   -   matrix size
    C   -   condition number (in 2-norm)

OUTPUT PARAMETERS:
    A   -   random matrix with norm2(A)=1 and cond(A)=C

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void rmatrixrndcond(ae_int_t n,
     double c,
     /* Real    */ ae_matrix* a,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double l1;
    double l2;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(a);
    _hqrndstate_init(&rs, _state);

    ae_assert(n>=1&&ae_fp_greater_eq(c,(double)(1)), "RMatrixRndCond: N<1 or C<1!", _state);
    ae_matrix_set_length(a, n, n, _state);
    if( n==1 )
    {
        
        /*
         * special case
         */
        a->ptr.pp_double[0][0] = (double)(2*ae_randominteger(2, _state)-1);
        ae_frame_leave(_state);
        return;
    }
    hqrndrandomize(&rs, _state);
    l1 = (double)(0);
    l2 = ae_log(1/c, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a->ptr.pp_double[i][j] = (double)(0);
        }
    }
    a->ptr.pp_double[0][0] = ae_exp(l1, _state);
    for(i=1; i<=n-2; i++)
    {
        a->ptr.pp_double[i][i] = ae_exp(hqrnduniformr(&rs, _state)*(l2-l1)+l1, _state);
    }
    a->ptr.pp_double[n-1][n-1] = ae_exp(l2, _state);
    rmatrixrndorthogonalfromtheleft(a, n, n, _state);
    rmatrixrndorthogonalfromtheright(a, n, n, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Generation of a random Haar distributed orthogonal complex matrix

INPUT PARAMETERS:
    N   -   matrix size, N>=1

OUTPUT PARAMETERS:
    A   -   orthogonal NxN matrix, array[0..N-1,0..N-1]

NOTE: this function uses algorithm  described  in  Stewart, G. W.  (1980),
      "The Efficient Generation of  Random  Orthogonal  Matrices  with  an
      Application to Condition Estimators".
      
      Speaking short, to generate an (N+1)x(N+1) orthogonal matrix, it:
      * takes an NxN one
      * takes uniformly distributed unit vector of dimension N+1.
      * constructs a Householder reflection from the vector, then applies
        it to the smaller matrix (embedded in the larger size with a 1 at
        the bottom right corner).

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void cmatrixrndorthogonal(ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(a);

    ae_assert(n>=1, "CMatrixRndOrthogonal: N<1!", _state);
    ae_matrix_set_length(a, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i==j )
            {
                a->ptr.pp_complex[i][j] = ae_complex_from_i(1);
            }
            else
            {
                a->ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
    }
    cmatrixrndorthogonalfromtheright(a, n, n, _state);
}


/*************************************************************************
Generation of random NxN complex matrix with given condition number C and
norm2(A)=1

INPUT PARAMETERS:
    N   -   matrix size
    C   -   condition number (in 2-norm)

OUTPUT PARAMETERS:
    A   -   random matrix with norm2(A)=1 and cond(A)=C

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void cmatrixrndcond(ae_int_t n,
     double c,
     /* Complex */ ae_matrix* a,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double l1;
    double l2;
    hqrndstate state;
    ae_complex v;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(a);
    _hqrndstate_init(&state, _state);

    ae_assert(n>=1&&ae_fp_greater_eq(c,(double)(1)), "CMatrixRndCond: N<1 or C<1!", _state);
    ae_matrix_set_length(a, n, n, _state);
    if( n==1 )
    {
        
        /*
         * special case
         */
        hqrndrandomize(&state, _state);
        hqrndunit2(&state, &v.x, &v.y, _state);
        a->ptr.pp_complex[0][0] = v;
        ae_frame_leave(_state);
        return;
    }
    hqrndrandomize(&state, _state);
    l1 = (double)(0);
    l2 = ae_log(1/c, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a->ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
    a->ptr.pp_complex[0][0] = ae_complex_from_d(ae_exp(l1, _state));
    for(i=1; i<=n-2; i++)
    {
        a->ptr.pp_complex[i][i] = ae_complex_from_d(ae_exp(hqrnduniformr(&state, _state)*(l2-l1)+l1, _state));
    }
    a->ptr.pp_complex[n-1][n-1] = ae_complex_from_d(ae_exp(l2, _state));
    cmatrixrndorthogonalfromtheleft(a, n, n, _state);
    cmatrixrndorthogonalfromtheright(a, n, n, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Generation of random NxN symmetric matrix with given condition number  and
norm2(A)=1

INPUT PARAMETERS:
    N   -   matrix size
    C   -   condition number (in 2-norm)

OUTPUT PARAMETERS:
    A   -   random matrix with norm2(A)=1 and cond(A)=C

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void smatrixrndcond(ae_int_t n,
     double c,
     /* Real    */ ae_matrix* a,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double l1;
    double l2;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(a);
    _hqrndstate_init(&rs, _state);

    ae_assert(n>=1&&ae_fp_greater_eq(c,(double)(1)), "SMatrixRndCond: N<1 or C<1!", _state);
    ae_matrix_set_length(a, n, n, _state);
    if( n==1 )
    {
        
        /*
         * special case
         */
        a->ptr.pp_double[0][0] = (double)(2*ae_randominteger(2, _state)-1);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Prepare matrix
     */
    hqrndrandomize(&rs, _state);
    l1 = (double)(0);
    l2 = ae_log(1/c, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a->ptr.pp_double[i][j] = (double)(0);
        }
    }
    a->ptr.pp_double[0][0] = ae_exp(l1, _state);
    for(i=1; i<=n-2; i++)
    {
        a->ptr.pp_double[i][i] = (2*hqrnduniformi(&rs, 2, _state)-1)*ae_exp(hqrnduniformr(&rs, _state)*(l2-l1)+l1, _state);
    }
    a->ptr.pp_double[n-1][n-1] = ae_exp(l2, _state);
    
    /*
     * Multiply
     */
    smatrixrndmultiply(a, n, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Generation of random NxN symmetric positive definite matrix with given
condition number and norm2(A)=1

INPUT PARAMETERS:
    N   -   matrix size
    C   -   condition number (in 2-norm)

OUTPUT PARAMETERS:
    A   -   random SPD matrix with norm2(A)=1 and cond(A)=C

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void spdmatrixrndcond(ae_int_t n,
     double c,
     /* Real    */ ae_matrix* a,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double l1;
    double l2;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(a);
    _hqrndstate_init(&rs, _state);

    
    /*
     * Special cases
     */
    if( n<=0||ae_fp_less(c,(double)(1)) )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(a, n, n, _state);
    if( n==1 )
    {
        a->ptr.pp_double[0][0] = (double)(1);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Prepare matrix
     */
    hqrndrandomize(&rs, _state);
    l1 = (double)(0);
    l2 = ae_log(1/c, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a->ptr.pp_double[i][j] = (double)(0);
        }
    }
    a->ptr.pp_double[0][0] = ae_exp(l1, _state);
    for(i=1; i<=n-2; i++)
    {
        a->ptr.pp_double[i][i] = ae_exp(hqrnduniformr(&rs, _state)*(l2-l1)+l1, _state);
    }
    a->ptr.pp_double[n-1][n-1] = ae_exp(l2, _state);
    
    /*
     * Multiply
     */
    smatrixrndmultiply(a, n, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Generation of random NxN Hermitian matrix with given condition number  and
norm2(A)=1

INPUT PARAMETERS:
    N   -   matrix size
    C   -   condition number (in 2-norm)

OUTPUT PARAMETERS:
    A   -   random matrix with norm2(A)=1 and cond(A)=C

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void hmatrixrndcond(ae_int_t n,
     double c,
     /* Complex */ ae_matrix* a,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double l1;
    double l2;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(a);
    _hqrndstate_init(&rs, _state);

    ae_assert(n>=1&&ae_fp_greater_eq(c,(double)(1)), "HMatrixRndCond: N<1 or C<1!", _state);
    ae_matrix_set_length(a, n, n, _state);
    if( n==1 )
    {
        
        /*
         * special case
         */
        a->ptr.pp_complex[0][0] = ae_complex_from_i(2*ae_randominteger(2, _state)-1);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Prepare matrix
     */
    hqrndrandomize(&rs, _state);
    l1 = (double)(0);
    l2 = ae_log(1/c, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a->ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
    a->ptr.pp_complex[0][0] = ae_complex_from_d(ae_exp(l1, _state));
    for(i=1; i<=n-2; i++)
    {
        a->ptr.pp_complex[i][i] = ae_complex_from_d((2*hqrnduniformi(&rs, 2, _state)-1)*ae_exp(hqrnduniformr(&rs, _state)*(l2-l1)+l1, _state));
    }
    a->ptr.pp_complex[n-1][n-1] = ae_complex_from_d(ae_exp(l2, _state));
    
    /*
     * Multiply
     */
    hmatrixrndmultiply(a, n, _state);
    
    /*
     * post-process to ensure that matrix diagonal is real
     */
    for(i=0; i<=n-1; i++)
    {
        a->ptr.pp_complex[i][i].y = (double)(0);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Generation of random NxN Hermitian positive definite matrix with given
condition number and norm2(A)=1

INPUT PARAMETERS:
    N   -   matrix size
    C   -   condition number (in 2-norm)

OUTPUT PARAMETERS:
    A   -   random HPD matrix with norm2(A)=1 and cond(A)=C

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void hpdmatrixrndcond(ae_int_t n,
     double c,
     /* Complex */ ae_matrix* a,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double l1;
    double l2;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(a);
    _hqrndstate_init(&rs, _state);

    
    /*
     * Special cases
     */
    if( n<=0||ae_fp_less(c,(double)(1)) )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(a, n, n, _state);
    if( n==1 )
    {
        a->ptr.pp_complex[0][0] = ae_complex_from_i(1);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Prepare matrix
     */
    hqrndrandomize(&rs, _state);
    l1 = (double)(0);
    l2 = ae_log(1/c, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a->ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
    a->ptr.pp_complex[0][0] = ae_complex_from_d(ae_exp(l1, _state));
    for(i=1; i<=n-2; i++)
    {
        a->ptr.pp_complex[i][i] = ae_complex_from_d(ae_exp(hqrnduniformr(&rs, _state)*(l2-l1)+l1, _state));
    }
    a->ptr.pp_complex[n-1][n-1] = ae_complex_from_d(ae_exp(l2, _state));
    
    /*
     * Multiply
     */
    hmatrixrndmultiply(a, n, _state);
    
    /*
     * post-process to ensure that matrix diagonal is real
     */
    for(i=0; i<=n-1; i++)
    {
        a->ptr.pp_complex[i][i].y = (double)(0);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Multiplication of MxN matrix by NxN random Haar distributed orthogonal matrix

INPUT PARAMETERS:
    A   -   matrix, array[0..M-1, 0..N-1]
    M, N-   matrix size

OUTPUT PARAMETERS:
    A   -   A*Q, where Q is random NxN orthogonal matrix

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void rmatrixrndorthogonalfromtheright(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    double tau;
    double lambdav;
    ae_int_t s;
    ae_int_t i;
    double u1;
    double u2;
    ae_vector w;
    ae_vector v;
    hqrndstate state;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&v, 0, DT_REAL, _state);
    _hqrndstate_init(&state, _state);

    ae_assert(n>=1&&m>=1, "RMatrixRndOrthogonalFromTheRight: N<1 or M<1!", _state);
    if( n==1 )
    {
        
        /*
         * Special case
         */
        tau = (double)(2*ae_randominteger(2, _state)-1);
        for(i=0; i<=m-1; i++)
        {
            a->ptr.pp_double[i][0] = a->ptr.pp_double[i][0]*tau;
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * General case.
     * First pass.
     */
    ae_vector_set_length(&w, m, _state);
    ae_vector_set_length(&v, n+1, _state);
    hqrndrandomize(&state, _state);
    for(s=2; s<=n; s++)
    {
        
        /*
         * Prepare random normal v
         */
        do
        {
            i = 1;
            while(i<=s)
            {
                hqrndnormal2(&state, &u1, &u2, _state);
                v.ptr.p_double[i] = u1;
                if( i+1<=s )
                {
                    v.ptr.p_double[i+1] = u2;
                }
                i = i+2;
            }
            lambdav = ae_v_dotproduct(&v.ptr.p_double[1], 1, &v.ptr.p_double[1], 1, ae_v_len(1,s));
        }
        while(ae_fp_eq(lambdav,(double)(0)));
        
        /*
         * Prepare and apply reflection
         */
        generatereflection(&v, s, &tau, _state);
        v.ptr.p_double[1] = (double)(1);
        applyreflectionfromtheright(a, tau, &v, 0, m-1, n-s, n-1, &w, _state);
    }
    
    /*
     * Second pass.
     */
    for(i=0; i<=n-1; i++)
    {
        tau = (double)(2*hqrnduniformi(&state, 2, _state)-1);
        ae_v_muld(&a->ptr.pp_double[0][i], a->stride, ae_v_len(0,m-1), tau);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Multiplication of MxN matrix by MxM random Haar distributed orthogonal matrix

INPUT PARAMETERS:
    A   -   matrix, array[0..M-1, 0..N-1]
    M, N-   matrix size

OUTPUT PARAMETERS:
    A   -   Q*A, where Q is random MxM orthogonal matrix

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void rmatrixrndorthogonalfromtheleft(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    double tau;
    double lambdav;
    ae_int_t s;
    ae_int_t i;
    ae_int_t j;
    double u1;
    double u2;
    ae_vector w;
    ae_vector v;
    hqrndstate state;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&v, 0, DT_REAL, _state);
    _hqrndstate_init(&state, _state);

    ae_assert(n>=1&&m>=1, "RMatrixRndOrthogonalFromTheRight: N<1 or M<1!", _state);
    if( m==1 )
    {
        
        /*
         * special case
         */
        tau = (double)(2*ae_randominteger(2, _state)-1);
        for(j=0; j<=n-1; j++)
        {
            a->ptr.pp_double[0][j] = a->ptr.pp_double[0][j]*tau;
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * General case.
     * First pass.
     */
    ae_vector_set_length(&w, n, _state);
    ae_vector_set_length(&v, m+1, _state);
    hqrndrandomize(&state, _state);
    for(s=2; s<=m; s++)
    {
        
        /*
         * Prepare random normal v
         */
        do
        {
            i = 1;
            while(i<=s)
            {
                hqrndnormal2(&state, &u1, &u2, _state);
                v.ptr.p_double[i] = u1;
                if( i+1<=s )
                {
                    v.ptr.p_double[i+1] = u2;
                }
                i = i+2;
            }
            lambdav = ae_v_dotproduct(&v.ptr.p_double[1], 1, &v.ptr.p_double[1], 1, ae_v_len(1,s));
        }
        while(ae_fp_eq(lambdav,(double)(0)));
        
        /*
         * Prepare and apply reflection
         */
        generatereflection(&v, s, &tau, _state);
        v.ptr.p_double[1] = (double)(1);
        applyreflectionfromtheleft(a, tau, &v, m-s, m-1, 0, n-1, &w, _state);
    }
    
    /*
     * Second pass.
     */
    for(i=0; i<=m-1; i++)
    {
        tau = (double)(2*hqrnduniformi(&state, 2, _state)-1);
        ae_v_muld(&a->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), tau);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Multiplication of MxN complex matrix by NxN random Haar distributed
complex orthogonal matrix

INPUT PARAMETERS:
    A   -   matrix, array[0..M-1, 0..N-1]
    M, N-   matrix size

OUTPUT PARAMETERS:
    A   -   A*Q, where Q is random NxN orthogonal matrix

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void cmatrixrndorthogonalfromtheright(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_complex lambdav;
    ae_complex tau;
    ae_int_t s;
    ae_int_t i;
    ae_vector w;
    ae_vector v;
    hqrndstate state;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&w, 0, DT_COMPLEX, _state);
    ae_vector_init(&v, 0, DT_COMPLEX, _state);
    _hqrndstate_init(&state, _state);

    ae_assert(n>=1&&m>=1, "CMatrixRndOrthogonalFromTheRight: N<1 or M<1!", _state);
    if( n==1 )
    {
        
        /*
         * Special case
         */
        hqrndrandomize(&state, _state);
        hqrndunit2(&state, &tau.x, &tau.y, _state);
        for(i=0; i<=m-1; i++)
        {
            a->ptr.pp_complex[i][0] = ae_c_mul(a->ptr.pp_complex[i][0],tau);
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * General case.
     * First pass.
     */
    ae_vector_set_length(&w, m, _state);
    ae_vector_set_length(&v, n+1, _state);
    hqrndrandomize(&state, _state);
    for(s=2; s<=n; s++)
    {
        
        /*
         * Prepare random normal v
         */
        do
        {
            for(i=1; i<=s; i++)
            {
                hqrndnormal2(&state, &tau.x, &tau.y, _state);
                v.ptr.p_complex[i] = tau;
            }
            lambdav = ae_v_cdotproduct(&v.ptr.p_complex[1], 1, "N", &v.ptr.p_complex[1], 1, "Conj", ae_v_len(1,s));
        }
        while(ae_c_eq_d(lambdav,(double)(0)));
        
        /*
         * Prepare and apply reflection
         */
        complexgeneratereflection(&v, s, &tau, _state);
        v.ptr.p_complex[1] = ae_complex_from_i(1);
        complexapplyreflectionfromtheright(a, tau, &v, 0, m-1, n-s, n-1, &w, _state);
    }
    
    /*
     * Second pass.
     */
    for(i=0; i<=n-1; i++)
    {
        hqrndunit2(&state, &tau.x, &tau.y, _state);
        ae_v_cmulc(&a->ptr.pp_complex[0][i], a->stride, ae_v_len(0,m-1), tau);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Multiplication of MxN complex matrix by MxM random Haar distributed
complex orthogonal matrix

INPUT PARAMETERS:
    A   -   matrix, array[0..M-1, 0..N-1]
    M, N-   matrix size

OUTPUT PARAMETERS:
    A   -   Q*A, where Q is random MxM orthogonal matrix

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void cmatrixrndorthogonalfromtheleft(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_complex tau;
    ae_complex lambdav;
    ae_int_t s;
    ae_int_t i;
    ae_int_t j;
    ae_vector w;
    ae_vector v;
    hqrndstate state;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&w, 0, DT_COMPLEX, _state);
    ae_vector_init(&v, 0, DT_COMPLEX, _state);
    _hqrndstate_init(&state, _state);

    ae_assert(n>=1&&m>=1, "CMatrixRndOrthogonalFromTheRight: N<1 or M<1!", _state);
    if( m==1 )
    {
        
        /*
         * special case
         */
        hqrndrandomize(&state, _state);
        hqrndunit2(&state, &tau.x, &tau.y, _state);
        for(j=0; j<=n-1; j++)
        {
            a->ptr.pp_complex[0][j] = ae_c_mul(a->ptr.pp_complex[0][j],tau);
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * General case.
     * First pass.
     */
    ae_vector_set_length(&w, n, _state);
    ae_vector_set_length(&v, m+1, _state);
    hqrndrandomize(&state, _state);
    for(s=2; s<=m; s++)
    {
        
        /*
         * Prepare random normal v
         */
        do
        {
            for(i=1; i<=s; i++)
            {
                hqrndnormal2(&state, &tau.x, &tau.y, _state);
                v.ptr.p_complex[i] = tau;
            }
            lambdav = ae_v_cdotproduct(&v.ptr.p_complex[1], 1, "N", &v.ptr.p_complex[1], 1, "Conj", ae_v_len(1,s));
        }
        while(ae_c_eq_d(lambdav,(double)(0)));
        
        /*
         * Prepare and apply reflection
         */
        complexgeneratereflection(&v, s, &tau, _state);
        v.ptr.p_complex[1] = ae_complex_from_i(1);
        complexapplyreflectionfromtheleft(a, tau, &v, m-s, m-1, 0, n-1, &w, _state);
    }
    
    /*
     * Second pass.
     */
    for(i=0; i<=m-1; i++)
    {
        hqrndunit2(&state, &tau.x, &tau.y, _state);
        ae_v_cmulc(&a->ptr.pp_complex[i][0], 1, ae_v_len(0,n-1), tau);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Symmetric multiplication of NxN matrix by random Haar distributed
orthogonal  matrix

INPUT PARAMETERS:
    A   -   matrix, array[0..N-1, 0..N-1]
    N   -   matrix size

OUTPUT PARAMETERS:
    A   -   Q'*A*Q, where Q is random NxN orthogonal matrix

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void smatrixrndmultiply(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    double tau;
    double lambdav;
    ae_int_t s;
    ae_int_t i;
    double u1;
    double u2;
    ae_vector w;
    ae_vector v;
    hqrndstate state;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&v, 0, DT_REAL, _state);
    _hqrndstate_init(&state, _state);

    
    /*
     * General case.
     */
    ae_vector_set_length(&w, n, _state);
    ae_vector_set_length(&v, n+1, _state);
    hqrndrandomize(&state, _state);
    for(s=2; s<=n; s++)
    {
        
        /*
         * Prepare random normal v
         */
        do
        {
            i = 1;
            while(i<=s)
            {
                hqrndnormal2(&state, &u1, &u2, _state);
                v.ptr.p_double[i] = u1;
                if( i+1<=s )
                {
                    v.ptr.p_double[i+1] = u2;
                }
                i = i+2;
            }
            lambdav = ae_v_dotproduct(&v.ptr.p_double[1], 1, &v.ptr.p_double[1], 1, ae_v_len(1,s));
        }
        while(ae_fp_eq(lambdav,(double)(0)));
        
        /*
         * Prepare and apply reflection
         */
        generatereflection(&v, s, &tau, _state);
        v.ptr.p_double[1] = (double)(1);
        applyreflectionfromtheright(a, tau, &v, 0, n-1, n-s, n-1, &w, _state);
        applyreflectionfromtheleft(a, tau, &v, n-s, n-1, 0, n-1, &w, _state);
    }
    
    /*
     * Second pass.
     */
    for(i=0; i<=n-1; i++)
    {
        tau = (double)(2*hqrnduniformi(&state, 2, _state)-1);
        ae_v_muld(&a->ptr.pp_double[0][i], a->stride, ae_v_len(0,n-1), tau);
        ae_v_muld(&a->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), tau);
    }
    
    /*
     * Copy upper triangle to lower
     */
    for(i=0; i<=n-2; i++)
    {
        ae_v_move(&a->ptr.pp_double[i+1][i], a->stride, &a->ptr.pp_double[i][i+1], 1, ae_v_len(i+1,n-1));
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Hermitian multiplication of NxN matrix by random Haar distributed
complex orthogonal matrix

INPUT PARAMETERS:
    A   -   matrix, array[0..N-1, 0..N-1]
    N   -   matrix size

OUTPUT PARAMETERS:
    A   -   Q^H*A*Q, where Q is random NxN orthogonal matrix

  -- ALGLIB routine --
     04.12.2009
     Bochkanov Sergey
*************************************************************************/
void hmatrixrndmultiply(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_complex tau;
    ae_complex lambdav;
    ae_int_t s;
    ae_int_t i;
    ae_vector w;
    ae_vector v;
    hqrndstate state;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&w, 0, DT_COMPLEX, _state);
    ae_vector_init(&v, 0, DT_COMPLEX, _state);
    _hqrndstate_init(&state, _state);

    
    /*
     * General case.
     */
    ae_vector_set_length(&w, n, _state);
    ae_vector_set_length(&v, n+1, _state);
    hqrndrandomize(&state, _state);
    for(s=2; s<=n; s++)
    {
        
        /*
         * Prepare random normal v
         */
        do
        {
            for(i=1; i<=s; i++)
            {
                hqrndnormal2(&state, &tau.x, &tau.y, _state);
                v.ptr.p_complex[i] = tau;
            }
            lambdav = ae_v_cdotproduct(&v.ptr.p_complex[1], 1, "N", &v.ptr.p_complex[1], 1, "Conj", ae_v_len(1,s));
        }
        while(ae_c_eq_d(lambdav,(double)(0)));
        
        /*
         * Prepare and apply reflection
         */
        complexgeneratereflection(&v, s, &tau, _state);
        v.ptr.p_complex[1] = ae_complex_from_i(1);
        complexapplyreflectionfromtheright(a, tau, &v, 0, n-1, n-s, n-1, &w, _state);
        complexapplyreflectionfromtheleft(a, ae_c_conj(tau, _state), &v, n-s, n-1, 0, n-1, &w, _state);
    }
    
    /*
     * Second pass.
     */
    for(i=0; i<=n-1; i++)
    {
        hqrndunit2(&state, &tau.x, &tau.y, _state);
        ae_v_cmulc(&a->ptr.pp_complex[0][i], a->stride, ae_v_len(0,n-1), tau);
        tau = ae_c_conj(tau, _state);
        ae_v_cmulc(&a->ptr.pp_complex[i][0], 1, ae_v_len(0,n-1), tau);
    }
    
    /*
     * Change all values from lower triangle by complex-conjugate values
     * from upper one
     */
    for(i=0; i<=n-2; i++)
    {
        ae_v_cmove(&a->ptr.pp_complex[i+1][i], a->stride, &a->ptr.pp_complex[i][i+1], 1, "N", ae_v_len(i+1,n-1));
    }
    for(s=0; s<=n-2; s++)
    {
        for(i=s+1; i<=n-1; i++)
        {
            a->ptr.pp_complex[i][s].y = -a->ptr.pp_complex[i][s].y;
        }
    }
    ae_frame_leave(_state);
}


/*$ End $*/
