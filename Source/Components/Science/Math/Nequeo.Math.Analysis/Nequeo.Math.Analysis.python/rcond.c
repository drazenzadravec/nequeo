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
#include "rcond.h"


/*$ Declarations $*/
static void rcond_rmatrixrcondtrinternal(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     ae_bool onenorm,
     double anorm,
     double* rc,
     ae_state *_state);
static void rcond_cmatrixrcondtrinternal(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     ae_bool onenorm,
     double anorm,
     double* rc,
     ae_state *_state);
static void rcond_spdmatrixrcondcholeskyinternal(/* Real    */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isnormprovided,
     double anorm,
     double* rc,
     ae_state *_state);
static void rcond_hpdmatrixrcondcholeskyinternal(/* Complex */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isnormprovided,
     double anorm,
     double* rc,
     ae_state *_state);
static void rcond_rmatrixrcondluinternal(/* Real    */ ae_matrix* lua,
     ae_int_t n,
     ae_bool onenorm,
     ae_bool isanormprovided,
     double anorm,
     double* rc,
     ae_state *_state);
static void rcond_cmatrixrcondluinternal(/* Complex */ ae_matrix* lua,
     ae_int_t n,
     ae_bool onenorm,
     ae_bool isanormprovided,
     double anorm,
     double* rc,
     ae_state *_state);
static void rcond_rmatrixestimatenorm(ae_int_t n,
     /* Real    */ ae_vector* v,
     /* Real    */ ae_vector* x,
     /* Integer */ ae_vector* isgn,
     double* est,
     ae_int_t* kase,
     ae_state *_state);
static void rcond_cmatrixestimatenorm(ae_int_t n,
     /* Complex */ ae_vector* v,
     /* Complex */ ae_vector* x,
     double* est,
     ae_int_t* kase,
     /* Integer */ ae_vector* isave,
     /* Real    */ ae_vector* rsave,
     ae_state *_state);
static double rcond_internalcomplexrcondscsum1(/* Complex */ ae_vector* x,
     ae_int_t n,
     ae_state *_state);
static ae_int_t rcond_internalcomplexrcondicmax1(/* Complex */ ae_vector* x,
     ae_int_t n,
     ae_state *_state);
static void rcond_internalcomplexrcondsaveall(/* Integer */ ae_vector* isave,
     /* Real    */ ae_vector* rsave,
     ae_int_t* i,
     ae_int_t* iter,
     ae_int_t* j,
     ae_int_t* jlast,
     ae_int_t* jump,
     double* absxi,
     double* altsgn,
     double* estold,
     double* temp,
     ae_state *_state);
static void rcond_internalcomplexrcondloadall(/* Integer */ ae_vector* isave,
     /* Real    */ ae_vector* rsave,
     ae_int_t* i,
     ae_int_t* iter,
     ae_int_t* j,
     ae_int_t* jlast,
     ae_int_t* jump,
     double* absxi,
     double* altsgn,
     double* estold,
     double* temp,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Estimate of a matrix condition number (1-norm)

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
    N   -   size of matrix A.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double rmatrixrcond1(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;
    ae_int_t j;
    double v;
    double nrm;
    ae_vector pivots;
    ae_vector t;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init(&pivots, 0, DT_INT, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);

    ae_assert(n>=1, "RMatrixRCond1: N<1!", _state);
    ae_vector_set_length(&t, n, _state);
    for(i=0; i<=n-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            t.ptr.p_double[j] = t.ptr.p_double[j]+ae_fabs(a->ptr.pp_double[i][j], _state);
        }
    }
    nrm = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        nrm = ae_maxreal(nrm, t.ptr.p_double[i], _state);
    }
    rmatrixlu(a, n, n, &pivots, _state);
    rcond_rmatrixrcondluinternal(a, n, ae_true, ae_true, nrm, &v, _state);
    result = v;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Estimate of a matrix condition number (infinity-norm).

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
    N   -   size of matrix A.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double rmatrixrcondinf(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;
    ae_int_t j;
    double v;
    double nrm;
    ae_vector pivots;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init(&pivots, 0, DT_INT, _state);

    ae_assert(n>=1, "RMatrixRCondInf: N<1!", _state);
    nrm = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        v = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            v = v+ae_fabs(a->ptr.pp_double[i][j], _state);
        }
        nrm = ae_maxreal(nrm, v, _state);
    }
    rmatrixlu(a, n, n, &pivots, _state);
    rcond_rmatrixrcondluinternal(a, n, ae_false, ae_true, nrm, &v, _state);
    result = v;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Condition number estimate of a symmetric positive definite matrix.

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

It should be noted that 1-norm and inf-norm of condition numbers of symmetric
matrices are equal, so the algorithm doesn't take into account the
differences between these types of norms.

Input parameters:
    A       -   symmetric positive definite matrix which is given by its
                upper or lower triangle depending on the value of
                IsUpper. Array with elements [0..N-1, 0..N-1].
    N       -   size of matrix A.
    IsUpper -   storage format.

Result:
    1/LowerBound(cond(A)), if matrix A is positive definite,
   -1, if matrix A is not positive definite, and its condition number
    could not be found by this algorithm.

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double spdmatrixrcond(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t j2;
    double v;
    double nrm;
    ae_vector t;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init(&t, 0, DT_REAL, _state);

    ae_vector_set_length(&t, n, _state);
    for(i=0; i<=n-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
    }
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
        for(j=j1; j<=j2; j++)
        {
            if( i==j )
            {
                t.ptr.p_double[i] = t.ptr.p_double[i]+ae_fabs(a->ptr.pp_double[i][i], _state);
            }
            else
            {
                t.ptr.p_double[i] = t.ptr.p_double[i]+ae_fabs(a->ptr.pp_double[i][j], _state);
                t.ptr.p_double[j] = t.ptr.p_double[j]+ae_fabs(a->ptr.pp_double[i][j], _state);
            }
        }
    }
    nrm = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        nrm = ae_maxreal(nrm, t.ptr.p_double[i], _state);
    }
    if( spdmatrixcholesky(a, n, isupper, _state) )
    {
        rcond_spdmatrixrcondcholeskyinternal(a, n, isupper, ae_true, nrm, &v, _state);
        result = v;
    }
    else
    {
        result = (double)(-1);
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Triangular matrix: estimate of a condition number (1-norm)

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    A       -   matrix. Array[0..N-1, 0..N-1].
    N       -   size of A.
    IsUpper -   True, if the matrix is upper triangular.
    IsUnit  -   True, if the matrix has a unit diagonal.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double rmatrixtrrcond1(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double v;
    double nrm;
    ae_vector pivots;
    ae_vector t;
    ae_int_t j1;
    ae_int_t j2;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&pivots, 0, DT_INT, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);

    ae_assert(n>=1, "RMatrixTRRCond1: N<1!", _state);
    ae_vector_set_length(&t, n, _state);
    for(i=0; i<=n-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        if( isupper )
        {
            j1 = i+1;
            j2 = n-1;
        }
        else
        {
            j1 = 0;
            j2 = i-1;
        }
        for(j=j1; j<=j2; j++)
        {
            t.ptr.p_double[j] = t.ptr.p_double[j]+ae_fabs(a->ptr.pp_double[i][j], _state);
        }
        if( isunit )
        {
            t.ptr.p_double[i] = t.ptr.p_double[i]+1;
        }
        else
        {
            t.ptr.p_double[i] = t.ptr.p_double[i]+ae_fabs(a->ptr.pp_double[i][i], _state);
        }
    }
    nrm = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        nrm = ae_maxreal(nrm, t.ptr.p_double[i], _state);
    }
    rcond_rmatrixrcondtrinternal(a, n, isupper, isunit, ae_true, nrm, &v, _state);
    result = v;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Triangular matrix: estimate of a matrix condition number (infinity-norm).

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
    N   -   size of matrix A.
    IsUpper -   True, if the matrix is upper triangular.
    IsUnit  -   True, if the matrix has a unit diagonal.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double rmatrixtrrcondinf(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double v;
    double nrm;
    ae_vector pivots;
    ae_int_t j1;
    ae_int_t j2;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&pivots, 0, DT_INT, _state);

    ae_assert(n>=1, "RMatrixTRRCondInf: N<1!", _state);
    nrm = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        if( isupper )
        {
            j1 = i+1;
            j2 = n-1;
        }
        else
        {
            j1 = 0;
            j2 = i-1;
        }
        v = (double)(0);
        for(j=j1; j<=j2; j++)
        {
            v = v+ae_fabs(a->ptr.pp_double[i][j], _state);
        }
        if( isunit )
        {
            v = v+1;
        }
        else
        {
            v = v+ae_fabs(a->ptr.pp_double[i][i], _state);
        }
        nrm = ae_maxreal(nrm, v, _state);
    }
    rcond_rmatrixrcondtrinternal(a, n, isupper, isunit, ae_false, nrm, &v, _state);
    result = v;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Condition number estimate of a Hermitian positive definite matrix.

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

It should be noted that 1-norm and inf-norm of condition numbers of symmetric
matrices are equal, so the algorithm doesn't take into account the
differences between these types of norms.

Input parameters:
    A       -   Hermitian positive definite matrix which is given by its
                upper or lower triangle depending on the value of
                IsUpper. Array with elements [0..N-1, 0..N-1].
    N       -   size of matrix A.
    IsUpper -   storage format.

Result:
    1/LowerBound(cond(A)), if matrix A is positive definite,
   -1, if matrix A is not positive definite, and its condition number
    could not be found by this algorithm.

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double hpdmatrixrcond(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t j2;
    double v;
    double nrm;
    ae_vector t;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init(&t, 0, DT_REAL, _state);

    ae_vector_set_length(&t, n, _state);
    for(i=0; i<=n-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
    }
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
        for(j=j1; j<=j2; j++)
        {
            if( i==j )
            {
                t.ptr.p_double[i] = t.ptr.p_double[i]+ae_c_abs(a->ptr.pp_complex[i][i], _state);
            }
            else
            {
                t.ptr.p_double[i] = t.ptr.p_double[i]+ae_c_abs(a->ptr.pp_complex[i][j], _state);
                t.ptr.p_double[j] = t.ptr.p_double[j]+ae_c_abs(a->ptr.pp_complex[i][j], _state);
            }
        }
    }
    nrm = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        nrm = ae_maxreal(nrm, t.ptr.p_double[i], _state);
    }
    if( hpdmatrixcholesky(a, n, isupper, _state) )
    {
        rcond_hpdmatrixrcondcholeskyinternal(a, n, isupper, ae_true, nrm, &v, _state);
        result = v;
    }
    else
    {
        result = (double)(-1);
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Estimate of a matrix condition number (1-norm)

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
    N   -   size of matrix A.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double cmatrixrcond1(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;
    ae_int_t j;
    double v;
    double nrm;
    ae_vector pivots;
    ae_vector t;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init(&pivots, 0, DT_INT, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);

    ae_assert(n>=1, "CMatrixRCond1: N<1!", _state);
    ae_vector_set_length(&t, n, _state);
    for(i=0; i<=n-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            t.ptr.p_double[j] = t.ptr.p_double[j]+ae_c_abs(a->ptr.pp_complex[i][j], _state);
        }
    }
    nrm = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        nrm = ae_maxreal(nrm, t.ptr.p_double[i], _state);
    }
    cmatrixlu(a, n, n, &pivots, _state);
    rcond_cmatrixrcondluinternal(a, n, ae_true, ae_true, nrm, &v, _state);
    result = v;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Estimate of a matrix condition number (infinity-norm).

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
    N   -   size of matrix A.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double cmatrixrcondinf(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;
    ae_int_t j;
    double v;
    double nrm;
    ae_vector pivots;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init(&pivots, 0, DT_INT, _state);

    ae_assert(n>=1, "CMatrixRCondInf: N<1!", _state);
    nrm = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        v = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            v = v+ae_c_abs(a->ptr.pp_complex[i][j], _state);
        }
        nrm = ae_maxreal(nrm, v, _state);
    }
    cmatrixlu(a, n, n, &pivots, _state);
    rcond_cmatrixrcondluinternal(a, n, ae_false, ae_true, nrm, &v, _state);
    result = v;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Estimate of the condition number of a matrix given by its LU decomposition (1-norm)

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    LUA         -   LU decomposition of a matrix in compact form. Output of
                    the RMatrixLU subroutine.
    N           -   size of matrix A.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double rmatrixlurcond1(/* Real    */ ae_matrix* lua,
     ae_int_t n,
     ae_state *_state)
{
    double v;
    double result;


    rcond_rmatrixrcondluinternal(lua, n, ae_true, ae_false, (double)(0), &v, _state);
    result = v;
    return result;
}


/*************************************************************************
Estimate of the condition number of a matrix given by its LU decomposition
(infinity norm).

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    LUA     -   LU decomposition of a matrix in compact form. Output of
                the RMatrixLU subroutine.
    N       -   size of matrix A.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double rmatrixlurcondinf(/* Real    */ ae_matrix* lua,
     ae_int_t n,
     ae_state *_state)
{
    double v;
    double result;


    rcond_rmatrixrcondluinternal(lua, n, ae_false, ae_false, (double)(0), &v, _state);
    result = v;
    return result;
}


/*************************************************************************
Condition number estimate of a symmetric positive definite matrix given by
Cholesky decomposition.

The algorithm calculates a lower bound of the condition number. In this
case, the algorithm does not return a lower bound of the condition number,
but an inverse number (to avoid an overflow in case of a singular matrix).

It should be noted that 1-norm and inf-norm condition numbers of symmetric
matrices are equal, so the algorithm doesn't take into account the
differences between these types of norms.

Input parameters:
    CD  - Cholesky decomposition of matrix A,
          output of SMatrixCholesky subroutine.
    N   - size of matrix A.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double spdmatrixcholeskyrcond(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    double v;
    double result;


    rcond_spdmatrixrcondcholeskyinternal(a, n, isupper, ae_false, (double)(0), &v, _state);
    result = v;
    return result;
}


/*************************************************************************
Condition number estimate of a Hermitian positive definite matrix given by
Cholesky decomposition.

The algorithm calculates a lower bound of the condition number. In this
case, the algorithm does not return a lower bound of the condition number,
but an inverse number (to avoid an overflow in case of a singular matrix).

It should be noted that 1-norm and inf-norm condition numbers of symmetric
matrices are equal, so the algorithm doesn't take into account the
differences between these types of norms.

Input parameters:
    CD  - Cholesky decomposition of matrix A,
          output of SMatrixCholesky subroutine.
    N   - size of matrix A.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double hpdmatrixcholeskyrcond(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    double v;
    double result;


    rcond_hpdmatrixrcondcholeskyinternal(a, n, isupper, ae_false, (double)(0), &v, _state);
    result = v;
    return result;
}


/*************************************************************************
Estimate of the condition number of a matrix given by its LU decomposition (1-norm)

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    LUA         -   LU decomposition of a matrix in compact form. Output of
                    the CMatrixLU subroutine.
    N           -   size of matrix A.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double cmatrixlurcond1(/* Complex */ ae_matrix* lua,
     ae_int_t n,
     ae_state *_state)
{
    double v;
    double result;


    ae_assert(n>=1, "CMatrixLURCond1: N<1!", _state);
    rcond_cmatrixrcondluinternal(lua, n, ae_true, ae_false, 0.0, &v, _state);
    result = v;
    return result;
}


/*************************************************************************
Estimate of the condition number of a matrix given by its LU decomposition
(infinity norm).

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    LUA     -   LU decomposition of a matrix in compact form. Output of
                the CMatrixLU subroutine.
    N       -   size of matrix A.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double cmatrixlurcondinf(/* Complex */ ae_matrix* lua,
     ae_int_t n,
     ae_state *_state)
{
    double v;
    double result;


    ae_assert(n>=1, "CMatrixLURCondInf: N<1!", _state);
    rcond_cmatrixrcondluinternal(lua, n, ae_false, ae_false, 0.0, &v, _state);
    result = v;
    return result;
}


/*************************************************************************
Triangular matrix: estimate of a condition number (1-norm)

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    A       -   matrix. Array[0..N-1, 0..N-1].
    N       -   size of A.
    IsUpper -   True, if the matrix is upper triangular.
    IsUnit  -   True, if the matrix has a unit diagonal.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double cmatrixtrrcond1(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double v;
    double nrm;
    ae_vector pivots;
    ae_vector t;
    ae_int_t j1;
    ae_int_t j2;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&pivots, 0, DT_INT, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);

    ae_assert(n>=1, "RMatrixTRRCond1: N<1!", _state);
    ae_vector_set_length(&t, n, _state);
    for(i=0; i<=n-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        if( isupper )
        {
            j1 = i+1;
            j2 = n-1;
        }
        else
        {
            j1 = 0;
            j2 = i-1;
        }
        for(j=j1; j<=j2; j++)
        {
            t.ptr.p_double[j] = t.ptr.p_double[j]+ae_c_abs(a->ptr.pp_complex[i][j], _state);
        }
        if( isunit )
        {
            t.ptr.p_double[i] = t.ptr.p_double[i]+1;
        }
        else
        {
            t.ptr.p_double[i] = t.ptr.p_double[i]+ae_c_abs(a->ptr.pp_complex[i][i], _state);
        }
    }
    nrm = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        nrm = ae_maxreal(nrm, t.ptr.p_double[i], _state);
    }
    rcond_cmatrixrcondtrinternal(a, n, isupper, isunit, ae_true, nrm, &v, _state);
    result = v;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Triangular matrix: estimate of a matrix condition number (infinity-norm).

The algorithm calculates a lower bound of the condition number. In this case,
the algorithm does not return a lower bound of the condition number, but an
inverse number (to avoid an overflow in case of a singular matrix).

Input parameters:
    A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
    N   -   size of matrix A.
    IsUpper -   True, if the matrix is upper triangular.
    IsUnit  -   True, if the matrix has a unit diagonal.

Result: 1/LowerBound(cond(A))

NOTE:
    if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
    0.0 is returned in such cases.
*************************************************************************/
double cmatrixtrrcondinf(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double v;
    double nrm;
    ae_vector pivots;
    ae_int_t j1;
    ae_int_t j2;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&pivots, 0, DT_INT, _state);

    ae_assert(n>=1, "RMatrixTRRCondInf: N<1!", _state);
    nrm = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        if( isupper )
        {
            j1 = i+1;
            j2 = n-1;
        }
        else
        {
            j1 = 0;
            j2 = i-1;
        }
        v = (double)(0);
        for(j=j1; j<=j2; j++)
        {
            v = v+ae_c_abs(a->ptr.pp_complex[i][j], _state);
        }
        if( isunit )
        {
            v = v+1;
        }
        else
        {
            v = v+ae_c_abs(a->ptr.pp_complex[i][i], _state);
        }
        nrm = ae_maxreal(nrm, v, _state);
    }
    rcond_cmatrixrcondtrinternal(a, n, isupper, isunit, ae_false, nrm, &v, _state);
    result = v;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Threshold for rcond: matrices with condition number beyond this  threshold
are considered singular.

Threshold must be far enough from underflow, at least Sqr(Threshold)  must
be greater than underflow.
*************************************************************************/
double rcondthreshold(ae_state *_state)
{
    double result;


    result = ae_sqrt(ae_sqrt(ae_minrealnumber, _state), _state);
    return result;
}


/*************************************************************************
Internal subroutine for condition number estimation

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     February 29, 1992
*************************************************************************/
static void rcond_rmatrixrcondtrinternal(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     ae_bool onenorm,
     double anorm,
     double* rc,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector ex;
    ae_vector ev;
    ae_vector iwork;
    ae_vector tmp;
    ae_int_t i;
    ae_int_t j;
    ae_int_t kase;
    ae_int_t kase1;
    ae_int_t j1;
    ae_int_t j2;
    double ainvnm;
    double maxgrowth;
    double s;

    ae_frame_make(_state, &_frame_block);
    *rc = 0;
    ae_vector_init(&ex, 0, DT_REAL, _state);
    ae_vector_init(&ev, 0, DT_REAL, _state);
    ae_vector_init(&iwork, 0, DT_INT, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    
    /*
     * RC=0 if something happens
     */
    *rc = (double)(0);
    
    /*
     * init
     */
    if( onenorm )
    {
        kase1 = 1;
    }
    else
    {
        kase1 = 2;
    }
    ae_vector_set_length(&iwork, n+1, _state);
    ae_vector_set_length(&tmp, n, _state);
    
    /*
     * prepare parameters for triangular solver
     */
    maxgrowth = 1/rcondthreshold(_state);
    s = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        if( isupper )
        {
            j1 = i+1;
            j2 = n-1;
        }
        else
        {
            j1 = 0;
            j2 = i-1;
        }
        for(j=j1; j<=j2; j++)
        {
            s = ae_maxreal(s, ae_fabs(a->ptr.pp_double[i][j], _state), _state);
        }
        if( isunit )
        {
            s = ae_maxreal(s, (double)(1), _state);
        }
        else
        {
            s = ae_maxreal(s, ae_fabs(a->ptr.pp_double[i][i], _state), _state);
        }
    }
    if( ae_fp_eq(s,(double)(0)) )
    {
        s = (double)(1);
    }
    s = 1/s;
    
    /*
     * Scale according to S
     */
    anorm = anorm*s;
    
    /*
     * Quick return if possible
     * We assume that ANORM<>0 after this block
     */
    if( ae_fp_eq(anorm,(double)(0)) )
    {
        ae_frame_leave(_state);
        return;
    }
    if( n==1 )
    {
        *rc = (double)(1);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Estimate the norm of inv(A).
     */
    ainvnm = (double)(0);
    kase = 0;
    for(;;)
    {
        rcond_rmatrixestimatenorm(n, &ev, &ex, &iwork, &ainvnm, &kase, _state);
        if( kase==0 )
        {
            break;
        }
        
        /*
         * from 1-based array to 0-based
         */
        for(i=0; i<=n-1; i++)
        {
            ex.ptr.p_double[i] = ex.ptr.p_double[i+1];
        }
        
        /*
         * multiply by inv(A) or inv(A')
         */
        if( kase==kase1 )
        {
            
            /*
             * multiply by inv(A)
             */
            if( !rmatrixscaledtrsafesolve(a, s, n, &ex, isupper, 0, isunit, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
        }
        else
        {
            
            /*
             * multiply by inv(A')
             */
            if( !rmatrixscaledtrsafesolve(a, s, n, &ex, isupper, 1, isunit, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
        }
        
        /*
         * from 0-based array to 1-based
         */
        for(i=n-1; i>=0; i--)
        {
            ex.ptr.p_double[i+1] = ex.ptr.p_double[i];
        }
    }
    
    /*
     * Compute the estimate of the reciprocal condition number.
     */
    if( ae_fp_neq(ainvnm,(double)(0)) )
    {
        *rc = 1/ainvnm;
        *rc = *rc/anorm;
        if( ae_fp_less(*rc,rcondthreshold(_state)) )
        {
            *rc = (double)(0);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Condition number estimation

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     March 31, 1993
*************************************************************************/
static void rcond_cmatrixrcondtrinternal(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunit,
     ae_bool onenorm,
     double anorm,
     double* rc,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector ex;
    ae_vector cwork2;
    ae_vector cwork3;
    ae_vector cwork4;
    ae_vector isave;
    ae_vector rsave;
    ae_int_t kase;
    ae_int_t kase1;
    double ainvnm;
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t j2;
    double s;
    double maxgrowth;

    ae_frame_make(_state, &_frame_block);
    *rc = 0;
    ae_vector_init(&ex, 0, DT_COMPLEX, _state);
    ae_vector_init(&cwork2, 0, DT_COMPLEX, _state);
    ae_vector_init(&cwork3, 0, DT_COMPLEX, _state);
    ae_vector_init(&cwork4, 0, DT_COMPLEX, _state);
    ae_vector_init(&isave, 0, DT_INT, _state);
    ae_vector_init(&rsave, 0, DT_REAL, _state);

    
    /*
     * RC=0 if something happens
     */
    *rc = (double)(0);
    
    /*
     * init
     */
    if( n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    if( n==0 )
    {
        *rc = (double)(1);
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(&cwork2, n+1, _state);
    
    /*
     * prepare parameters for triangular solver
     */
    maxgrowth = 1/rcondthreshold(_state);
    s = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        if( isupper )
        {
            j1 = i+1;
            j2 = n-1;
        }
        else
        {
            j1 = 0;
            j2 = i-1;
        }
        for(j=j1; j<=j2; j++)
        {
            s = ae_maxreal(s, ae_c_abs(a->ptr.pp_complex[i][j], _state), _state);
        }
        if( isunit )
        {
            s = ae_maxreal(s, (double)(1), _state);
        }
        else
        {
            s = ae_maxreal(s, ae_c_abs(a->ptr.pp_complex[i][i], _state), _state);
        }
    }
    if( ae_fp_eq(s,(double)(0)) )
    {
        s = (double)(1);
    }
    s = 1/s;
    
    /*
     * Scale according to S
     */
    anorm = anorm*s;
    
    /*
     * Quick return if possible
     */
    if( ae_fp_eq(anorm,(double)(0)) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Estimate the norm of inv(A).
     */
    ainvnm = (double)(0);
    if( onenorm )
    {
        kase1 = 1;
    }
    else
    {
        kase1 = 2;
    }
    kase = 0;
    for(;;)
    {
        rcond_cmatrixestimatenorm(n, &cwork4, &ex, &ainvnm, &kase, &isave, &rsave, _state);
        if( kase==0 )
        {
            break;
        }
        
        /*
         * From 1-based to 0-based
         */
        for(i=0; i<=n-1; i++)
        {
            ex.ptr.p_complex[i] = ex.ptr.p_complex[i+1];
        }
        
        /*
         * multiply by inv(A) or inv(A')
         */
        if( kase==kase1 )
        {
            
            /*
             * multiply by inv(A)
             */
            if( !cmatrixscaledtrsafesolve(a, s, n, &ex, isupper, 0, isunit, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
        }
        else
        {
            
            /*
             * multiply by inv(A')
             */
            if( !cmatrixscaledtrsafesolve(a, s, n, &ex, isupper, 2, isunit, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
        }
        
        /*
         * from 0-based to 1-based
         */
        for(i=n-1; i>=0; i--)
        {
            ex.ptr.p_complex[i+1] = ex.ptr.p_complex[i];
        }
    }
    
    /*
     * Compute the estimate of the reciprocal condition number.
     */
    if( ae_fp_neq(ainvnm,(double)(0)) )
    {
        *rc = 1/ainvnm;
        *rc = *rc/anorm;
        if( ae_fp_less(*rc,rcondthreshold(_state)) )
        {
            *rc = (double)(0);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine for condition number estimation

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     February 29, 1992
*************************************************************************/
static void rcond_spdmatrixrcondcholeskyinternal(/* Real    */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isnormprovided,
     double anorm,
     double* rc,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t kase;
    double ainvnm;
    ae_vector ex;
    ae_vector ev;
    ae_vector tmp;
    ae_vector iwork;
    double sa;
    double v;
    double maxgrowth;

    ae_frame_make(_state, &_frame_block);
    *rc = 0;
    ae_vector_init(&ex, 0, DT_REAL, _state);
    ae_vector_init(&ev, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&iwork, 0, DT_INT, _state);

    ae_assert(n>=1, "Assertion failed", _state);
    ae_vector_set_length(&tmp, n, _state);
    
    /*
     * RC=0 if something happens
     */
    *rc = (double)(0);
    
    /*
     * prepare parameters for triangular solver
     */
    maxgrowth = 1/rcondthreshold(_state);
    sa = (double)(0);
    if( isupper )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=i; j<=n-1; j++)
            {
                sa = ae_maxreal(sa, ae_c_abs(ae_complex_from_d(cha->ptr.pp_double[i][j]), _state), _state);
            }
        }
    }
    else
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=i; j++)
            {
                sa = ae_maxreal(sa, ae_c_abs(ae_complex_from_d(cha->ptr.pp_double[i][j]), _state), _state);
            }
        }
    }
    if( ae_fp_eq(sa,(double)(0)) )
    {
        sa = (double)(1);
    }
    sa = 1/sa;
    
    /*
     * Estimate the norm of A.
     */
    if( !isnormprovided )
    {
        kase = 0;
        anorm = (double)(0);
        for(;;)
        {
            rcond_rmatrixestimatenorm(n, &ev, &ex, &iwork, &anorm, &kase, _state);
            if( kase==0 )
            {
                break;
            }
            if( isupper )
            {
                
                /*
                 * Multiply by U
                 */
                for(i=1; i<=n; i++)
                {
                    v = ae_v_dotproduct(&cha->ptr.pp_double[i-1][i-1], 1, &ex.ptr.p_double[i], 1, ae_v_len(i-1,n-1));
                    ex.ptr.p_double[i] = v;
                }
                ae_v_muld(&ex.ptr.p_double[1], 1, ae_v_len(1,n), sa);
                
                /*
                 * Multiply by U'
                 */
                for(i=0; i<=n-1; i++)
                {
                    tmp.ptr.p_double[i] = (double)(0);
                }
                for(i=0; i<=n-1; i++)
                {
                    v = ex.ptr.p_double[i+1];
                    ae_v_addd(&tmp.ptr.p_double[i], 1, &cha->ptr.pp_double[i][i], 1, ae_v_len(i,n-1), v);
                }
                ae_v_move(&ex.ptr.p_double[1], 1, &tmp.ptr.p_double[0], 1, ae_v_len(1,n));
                ae_v_muld(&ex.ptr.p_double[1], 1, ae_v_len(1,n), sa);
            }
            else
            {
                
                /*
                 * Multiply by L'
                 */
                for(i=0; i<=n-1; i++)
                {
                    tmp.ptr.p_double[i] = (double)(0);
                }
                for(i=0; i<=n-1; i++)
                {
                    v = ex.ptr.p_double[i+1];
                    ae_v_addd(&tmp.ptr.p_double[0], 1, &cha->ptr.pp_double[i][0], 1, ae_v_len(0,i), v);
                }
                ae_v_move(&ex.ptr.p_double[1], 1, &tmp.ptr.p_double[0], 1, ae_v_len(1,n));
                ae_v_muld(&ex.ptr.p_double[1], 1, ae_v_len(1,n), sa);
                
                /*
                 * Multiply by L
                 */
                for(i=n; i>=1; i--)
                {
                    v = ae_v_dotproduct(&cha->ptr.pp_double[i-1][0], 1, &ex.ptr.p_double[1], 1, ae_v_len(0,i-1));
                    ex.ptr.p_double[i] = v;
                }
                ae_v_muld(&ex.ptr.p_double[1], 1, ae_v_len(1,n), sa);
            }
        }
    }
    
    /*
     * Quick return if possible
     */
    if( ae_fp_eq(anorm,(double)(0)) )
    {
        ae_frame_leave(_state);
        return;
    }
    if( n==1 )
    {
        *rc = (double)(1);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Estimate the 1-norm of inv(A).
     */
    kase = 0;
    for(;;)
    {
        rcond_rmatrixestimatenorm(n, &ev, &ex, &iwork, &ainvnm, &kase, _state);
        if( kase==0 )
        {
            break;
        }
        for(i=0; i<=n-1; i++)
        {
            ex.ptr.p_double[i] = ex.ptr.p_double[i+1];
        }
        if( isupper )
        {
            
            /*
             * Multiply by inv(U').
             */
            if( !rmatrixscaledtrsafesolve(cha, sa, n, &ex, isupper, 1, ae_false, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Multiply by inv(U).
             */
            if( !rmatrixscaledtrsafesolve(cha, sa, n, &ex, isupper, 0, ae_false, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
        }
        else
        {
            
            /*
             * Multiply by inv(L).
             */
            if( !rmatrixscaledtrsafesolve(cha, sa, n, &ex, isupper, 0, ae_false, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Multiply by inv(L').
             */
            if( !rmatrixscaledtrsafesolve(cha, sa, n, &ex, isupper, 1, ae_false, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
        }
        for(i=n-1; i>=0; i--)
        {
            ex.ptr.p_double[i+1] = ex.ptr.p_double[i];
        }
    }
    
    /*
     * Compute the estimate of the reciprocal condition number.
     */
    if( ae_fp_neq(ainvnm,(double)(0)) )
    {
        v = 1/ainvnm;
        *rc = v/anorm;
        if( ae_fp_less(*rc,rcondthreshold(_state)) )
        {
            *rc = (double)(0);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine for condition number estimation

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     February 29, 1992
*************************************************************************/
static void rcond_hpdmatrixrcondcholeskyinternal(/* Complex */ ae_matrix* cha,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isnormprovided,
     double anorm,
     double* rc,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector isave;
    ae_vector rsave;
    ae_vector ex;
    ae_vector ev;
    ae_vector tmp;
    ae_int_t kase;
    double ainvnm;
    ae_complex v;
    ae_int_t i;
    ae_int_t j;
    double sa;
    double maxgrowth;

    ae_frame_make(_state, &_frame_block);
    *rc = 0;
    ae_vector_init(&isave, 0, DT_INT, _state);
    ae_vector_init(&rsave, 0, DT_REAL, _state);
    ae_vector_init(&ex, 0, DT_COMPLEX, _state);
    ae_vector_init(&ev, 0, DT_COMPLEX, _state);
    ae_vector_init(&tmp, 0, DT_COMPLEX, _state);

    ae_assert(n>=1, "Assertion failed", _state);
    ae_vector_set_length(&tmp, n, _state);
    
    /*
     * RC=0 if something happens
     */
    *rc = (double)(0);
    
    /*
     * prepare parameters for triangular solver
     */
    maxgrowth = 1/rcondthreshold(_state);
    sa = (double)(0);
    if( isupper )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=i; j<=n-1; j++)
            {
                sa = ae_maxreal(sa, ae_c_abs(cha->ptr.pp_complex[i][j], _state), _state);
            }
        }
    }
    else
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=i; j++)
            {
                sa = ae_maxreal(sa, ae_c_abs(cha->ptr.pp_complex[i][j], _state), _state);
            }
        }
    }
    if( ae_fp_eq(sa,(double)(0)) )
    {
        sa = (double)(1);
    }
    sa = 1/sa;
    
    /*
     * Estimate the norm of A
     */
    if( !isnormprovided )
    {
        anorm = (double)(0);
        kase = 0;
        for(;;)
        {
            rcond_cmatrixestimatenorm(n, &ev, &ex, &anorm, &kase, &isave, &rsave, _state);
            if( kase==0 )
            {
                break;
            }
            if( isupper )
            {
                
                /*
                 * Multiply by U
                 */
                for(i=1; i<=n; i++)
                {
                    v = ae_v_cdotproduct(&cha->ptr.pp_complex[i-1][i-1], 1, "N", &ex.ptr.p_complex[i], 1, "N", ae_v_len(i-1,n-1));
                    ex.ptr.p_complex[i] = v;
                }
                ae_v_cmuld(&ex.ptr.p_complex[1], 1, ae_v_len(1,n), sa);
                
                /*
                 * Multiply by U'
                 */
                for(i=0; i<=n-1; i++)
                {
                    tmp.ptr.p_complex[i] = ae_complex_from_i(0);
                }
                for(i=0; i<=n-1; i++)
                {
                    v = ex.ptr.p_complex[i+1];
                    ae_v_caddc(&tmp.ptr.p_complex[i], 1, &cha->ptr.pp_complex[i][i], 1, "Conj", ae_v_len(i,n-1), v);
                }
                ae_v_cmove(&ex.ptr.p_complex[1], 1, &tmp.ptr.p_complex[0], 1, "N", ae_v_len(1,n));
                ae_v_cmuld(&ex.ptr.p_complex[1], 1, ae_v_len(1,n), sa);
            }
            else
            {
                
                /*
                 * Multiply by L'
                 */
                for(i=0; i<=n-1; i++)
                {
                    tmp.ptr.p_complex[i] = ae_complex_from_i(0);
                }
                for(i=0; i<=n-1; i++)
                {
                    v = ex.ptr.p_complex[i+1];
                    ae_v_caddc(&tmp.ptr.p_complex[0], 1, &cha->ptr.pp_complex[i][0], 1, "Conj", ae_v_len(0,i), v);
                }
                ae_v_cmove(&ex.ptr.p_complex[1], 1, &tmp.ptr.p_complex[0], 1, "N", ae_v_len(1,n));
                ae_v_cmuld(&ex.ptr.p_complex[1], 1, ae_v_len(1,n), sa);
                
                /*
                 * Multiply by L
                 */
                for(i=n; i>=1; i--)
                {
                    v = ae_v_cdotproduct(&cha->ptr.pp_complex[i-1][0], 1, "N", &ex.ptr.p_complex[1], 1, "N", ae_v_len(0,i-1));
                    ex.ptr.p_complex[i] = v;
                }
                ae_v_cmuld(&ex.ptr.p_complex[1], 1, ae_v_len(1,n), sa);
            }
        }
    }
    
    /*
     * Quick return if possible
     * After this block we assume that ANORM<>0
     */
    if( ae_fp_eq(anorm,(double)(0)) )
    {
        ae_frame_leave(_state);
        return;
    }
    if( n==1 )
    {
        *rc = (double)(1);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Estimate the norm of inv(A).
     */
    ainvnm = (double)(0);
    kase = 0;
    for(;;)
    {
        rcond_cmatrixestimatenorm(n, &ev, &ex, &ainvnm, &kase, &isave, &rsave, _state);
        if( kase==0 )
        {
            break;
        }
        for(i=0; i<=n-1; i++)
        {
            ex.ptr.p_complex[i] = ex.ptr.p_complex[i+1];
        }
        if( isupper )
        {
            
            /*
             * Multiply by inv(U').
             */
            if( !cmatrixscaledtrsafesolve(cha, sa, n, &ex, isupper, 2, ae_false, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Multiply by inv(U).
             */
            if( !cmatrixscaledtrsafesolve(cha, sa, n, &ex, isupper, 0, ae_false, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
        }
        else
        {
            
            /*
             * Multiply by inv(L).
             */
            if( !cmatrixscaledtrsafesolve(cha, sa, n, &ex, isupper, 0, ae_false, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Multiply by inv(L').
             */
            if( !cmatrixscaledtrsafesolve(cha, sa, n, &ex, isupper, 2, ae_false, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
        }
        for(i=n-1; i>=0; i--)
        {
            ex.ptr.p_complex[i+1] = ex.ptr.p_complex[i];
        }
    }
    
    /*
     * Compute the estimate of the reciprocal condition number.
     */
    if( ae_fp_neq(ainvnm,(double)(0)) )
    {
        *rc = 1/ainvnm;
        *rc = *rc/anorm;
        if( ae_fp_less(*rc,rcondthreshold(_state)) )
        {
            *rc = (double)(0);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine for condition number estimation

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     February 29, 1992
*************************************************************************/
static void rcond_rmatrixrcondluinternal(/* Real    */ ae_matrix* lua,
     ae_int_t n,
     ae_bool onenorm,
     ae_bool isanormprovided,
     double anorm,
     double* rc,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector ex;
    ae_vector ev;
    ae_vector iwork;
    ae_vector tmp;
    double v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t kase;
    ae_int_t kase1;
    double ainvnm;
    double maxgrowth;
    double su;
    double sl;
    ae_bool mupper;
    ae_bool munit;

    ae_frame_make(_state, &_frame_block);
    *rc = 0;
    ae_vector_init(&ex, 0, DT_REAL, _state);
    ae_vector_init(&ev, 0, DT_REAL, _state);
    ae_vector_init(&iwork, 0, DT_INT, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    
    /*
     * RC=0 if something happens
     */
    *rc = (double)(0);
    
    /*
     * init
     */
    if( onenorm )
    {
        kase1 = 1;
    }
    else
    {
        kase1 = 2;
    }
    mupper = ae_true;
    munit = ae_true;
    ae_vector_set_length(&iwork, n+1, _state);
    ae_vector_set_length(&tmp, n, _state);
    
    /*
     * prepare parameters for triangular solver
     */
    maxgrowth = 1/rcondthreshold(_state);
    su = (double)(0);
    sl = (double)(1);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=i-1; j++)
        {
            sl = ae_maxreal(sl, ae_fabs(lua->ptr.pp_double[i][j], _state), _state);
        }
        for(j=i; j<=n-1; j++)
        {
            su = ae_maxreal(su, ae_fabs(lua->ptr.pp_double[i][j], _state), _state);
        }
    }
    if( ae_fp_eq(su,(double)(0)) )
    {
        su = (double)(1);
    }
    su = 1/su;
    sl = 1/sl;
    
    /*
     * Estimate the norm of A.
     */
    if( !isanormprovided )
    {
        kase = 0;
        anorm = (double)(0);
        for(;;)
        {
            rcond_rmatrixestimatenorm(n, &ev, &ex, &iwork, &anorm, &kase, _state);
            if( kase==0 )
            {
                break;
            }
            if( kase==kase1 )
            {
                
                /*
                 * Multiply by U
                 */
                for(i=1; i<=n; i++)
                {
                    v = ae_v_dotproduct(&lua->ptr.pp_double[i-1][i-1], 1, &ex.ptr.p_double[i], 1, ae_v_len(i-1,n-1));
                    ex.ptr.p_double[i] = v;
                }
                
                /*
                 * Multiply by L
                 */
                for(i=n; i>=1; i--)
                {
                    if( i>1 )
                    {
                        v = ae_v_dotproduct(&lua->ptr.pp_double[i-1][0], 1, &ex.ptr.p_double[1], 1, ae_v_len(0,i-2));
                    }
                    else
                    {
                        v = (double)(0);
                    }
                    ex.ptr.p_double[i] = ex.ptr.p_double[i]+v;
                }
            }
            else
            {
                
                /*
                 * Multiply by L'
                 */
                for(i=0; i<=n-1; i++)
                {
                    tmp.ptr.p_double[i] = (double)(0);
                }
                for(i=0; i<=n-1; i++)
                {
                    v = ex.ptr.p_double[i+1];
                    if( i>=1 )
                    {
                        ae_v_addd(&tmp.ptr.p_double[0], 1, &lua->ptr.pp_double[i][0], 1, ae_v_len(0,i-1), v);
                    }
                    tmp.ptr.p_double[i] = tmp.ptr.p_double[i]+v;
                }
                ae_v_move(&ex.ptr.p_double[1], 1, &tmp.ptr.p_double[0], 1, ae_v_len(1,n));
                
                /*
                 * Multiply by U'
                 */
                for(i=0; i<=n-1; i++)
                {
                    tmp.ptr.p_double[i] = (double)(0);
                }
                for(i=0; i<=n-1; i++)
                {
                    v = ex.ptr.p_double[i+1];
                    ae_v_addd(&tmp.ptr.p_double[i], 1, &lua->ptr.pp_double[i][i], 1, ae_v_len(i,n-1), v);
                }
                ae_v_move(&ex.ptr.p_double[1], 1, &tmp.ptr.p_double[0], 1, ae_v_len(1,n));
            }
        }
    }
    
    /*
     * Scale according to SU/SL
     */
    anorm = anorm*su*sl;
    
    /*
     * Quick return if possible
     * We assume that ANORM<>0 after this block
     */
    if( ae_fp_eq(anorm,(double)(0)) )
    {
        ae_frame_leave(_state);
        return;
    }
    if( n==1 )
    {
        *rc = (double)(1);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Estimate the norm of inv(A).
     */
    ainvnm = (double)(0);
    kase = 0;
    for(;;)
    {
        rcond_rmatrixestimatenorm(n, &ev, &ex, &iwork, &ainvnm, &kase, _state);
        if( kase==0 )
        {
            break;
        }
        
        /*
         * from 1-based array to 0-based
         */
        for(i=0; i<=n-1; i++)
        {
            ex.ptr.p_double[i] = ex.ptr.p_double[i+1];
        }
        
        /*
         * multiply by inv(A) or inv(A')
         */
        if( kase==kase1 )
        {
            
            /*
             * Multiply by inv(L).
             */
            if( !rmatrixscaledtrsafesolve(lua, sl, n, &ex, !mupper, 0, munit, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Multiply by inv(U).
             */
            if( !rmatrixscaledtrsafesolve(lua, su, n, &ex, mupper, 0, !munit, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
        }
        else
        {
            
            /*
             * Multiply by inv(U').
             */
            if( !rmatrixscaledtrsafesolve(lua, su, n, &ex, mupper, 1, !munit, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Multiply by inv(L').
             */
            if( !rmatrixscaledtrsafesolve(lua, sl, n, &ex, !mupper, 1, munit, maxgrowth, _state) )
            {
                ae_frame_leave(_state);
                return;
            }
        }
        
        /*
         * from 0-based array to 1-based
         */
        for(i=n-1; i>=0; i--)
        {
            ex.ptr.p_double[i+1] = ex.ptr.p_double[i];
        }
    }
    
    /*
     * Compute the estimate of the reciprocal condition number.
     */
    if( ae_fp_neq(ainvnm,(double)(0)) )
    {
        *rc = 1/ainvnm;
        *rc = *rc/anorm;
        if( ae_fp_less(*rc,rcondthreshold(_state)) )
        {
            *rc = (double)(0);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Condition number estimation

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     March 31, 1993
*************************************************************************/
static void rcond_cmatrixrcondluinternal(/* Complex */ ae_matrix* lua,
     ae_int_t n,
     ae_bool onenorm,
     ae_bool isanormprovided,
     double anorm,
     double* rc,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector ex;
    ae_vector cwork2;
    ae_vector cwork3;
    ae_vector cwork4;
    ae_vector isave;
    ae_vector rsave;
    ae_int_t kase;
    ae_int_t kase1;
    double ainvnm;
    ae_complex v;
    ae_int_t i;
    ae_int_t j;
    double su;
    double sl;
    double maxgrowth;

    ae_frame_make(_state, &_frame_block);
    *rc = 0;
    ae_vector_init(&ex, 0, DT_COMPLEX, _state);
    ae_vector_init(&cwork2, 0, DT_COMPLEX, _state);
    ae_vector_init(&cwork3, 0, DT_COMPLEX, _state);
    ae_vector_init(&cwork4, 0, DT_COMPLEX, _state);
    ae_vector_init(&isave, 0, DT_INT, _state);
    ae_vector_init(&rsave, 0, DT_REAL, _state);

    if( n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(&cwork2, n+1, _state);
    *rc = (double)(0);
    if( n==0 )
    {
        *rc = (double)(1);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * prepare parameters for triangular solver
     */
    maxgrowth = 1/rcondthreshold(_state);
    su = (double)(0);
    sl = (double)(1);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=i-1; j++)
        {
            sl = ae_maxreal(sl, ae_c_abs(lua->ptr.pp_complex[i][j], _state), _state);
        }
        for(j=i; j<=n-1; j++)
        {
            su = ae_maxreal(su, ae_c_abs(lua->ptr.pp_complex[i][j], _state), _state);
        }
    }
    if( ae_fp_eq(su,(double)(0)) )
    {
        su = (double)(1);
    }
    su = 1/su;
    sl = 1/sl;
    
    /*
     * Estimate the norm of SU*SL*A.
     */
    if( !isanormprovided )
    {
        anorm = (double)(0);
        if( onenorm )
        {
            kase1 = 1;
        }
        else
        {
            kase1 = 2;
        }
        kase = 0;
        do
        {
            rcond_cmatrixestimatenorm(n, &cwork4, &ex, &anorm, &kase, &isave, &rsave, _state);
            if( kase!=0 )
            {
                if( kase==kase1 )
                {
                    
                    /*
                     * Multiply by U
                     */
                    for(i=1; i<=n; i++)
                    {
                        v = ae_v_cdotproduct(&lua->ptr.pp_complex[i-1][i-1], 1, "N", &ex.ptr.p_complex[i], 1, "N", ae_v_len(i-1,n-1));
                        ex.ptr.p_complex[i] = v;
                    }
                    
                    /*
                     * Multiply by L
                     */
                    for(i=n; i>=1; i--)
                    {
                        v = ae_complex_from_i(0);
                        if( i>1 )
                        {
                            v = ae_v_cdotproduct(&lua->ptr.pp_complex[i-1][0], 1, "N", &ex.ptr.p_complex[1], 1, "N", ae_v_len(0,i-2));
                        }
                        ex.ptr.p_complex[i] = ae_c_add(v,ex.ptr.p_complex[i]);
                    }
                }
                else
                {
                    
                    /*
                     * Multiply by L'
                     */
                    for(i=1; i<=n; i++)
                    {
                        cwork2.ptr.p_complex[i] = ae_complex_from_i(0);
                    }
                    for(i=1; i<=n; i++)
                    {
                        v = ex.ptr.p_complex[i];
                        if( i>1 )
                        {
                            ae_v_caddc(&cwork2.ptr.p_complex[1], 1, &lua->ptr.pp_complex[i-1][0], 1, "Conj", ae_v_len(1,i-1), v);
                        }
                        cwork2.ptr.p_complex[i] = ae_c_add(cwork2.ptr.p_complex[i],v);
                    }
                    
                    /*
                     * Multiply by U'
                     */
                    for(i=1; i<=n; i++)
                    {
                        ex.ptr.p_complex[i] = ae_complex_from_i(0);
                    }
                    for(i=1; i<=n; i++)
                    {
                        v = cwork2.ptr.p_complex[i];
                        ae_v_caddc(&ex.ptr.p_complex[i], 1, &lua->ptr.pp_complex[i-1][i-1], 1, "Conj", ae_v_len(i,n), v);
                    }
                }
            }
        }
        while(kase!=0);
    }
    
    /*
     * Scale according to SU/SL
     */
    anorm = anorm*su*sl;
    
    /*
     * Quick return if possible
     */
    if( ae_fp_eq(anorm,(double)(0)) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Estimate the norm of inv(A).
     */
    ainvnm = (double)(0);
    if( onenorm )
    {
        kase1 = 1;
    }
    else
    {
        kase1 = 2;
    }
    kase = 0;
    for(;;)
    {
        rcond_cmatrixestimatenorm(n, &cwork4, &ex, &ainvnm, &kase, &isave, &rsave, _state);
        if( kase==0 )
        {
            break;
        }
        
        /*
         * From 1-based to 0-based
         */
        for(i=0; i<=n-1; i++)
        {
            ex.ptr.p_complex[i] = ex.ptr.p_complex[i+1];
        }
        
        /*
         * multiply by inv(A) or inv(A')
         */
        if( kase==kase1 )
        {
            
            /*
             * Multiply by inv(L).
             */
            if( !cmatrixscaledtrsafesolve(lua, sl, n, &ex, ae_false, 0, ae_true, maxgrowth, _state) )
            {
                *rc = (double)(0);
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Multiply by inv(U).
             */
            if( !cmatrixscaledtrsafesolve(lua, su, n, &ex, ae_true, 0, ae_false, maxgrowth, _state) )
            {
                *rc = (double)(0);
                ae_frame_leave(_state);
                return;
            }
        }
        else
        {
            
            /*
             * Multiply by inv(U').
             */
            if( !cmatrixscaledtrsafesolve(lua, su, n, &ex, ae_true, 2, ae_false, maxgrowth, _state) )
            {
                *rc = (double)(0);
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Multiply by inv(L').
             */
            if( !cmatrixscaledtrsafesolve(lua, sl, n, &ex, ae_false, 2, ae_true, maxgrowth, _state) )
            {
                *rc = (double)(0);
                ae_frame_leave(_state);
                return;
            }
        }
        
        /*
         * from 0-based to 1-based
         */
        for(i=n-1; i>=0; i--)
        {
            ex.ptr.p_complex[i+1] = ex.ptr.p_complex[i];
        }
    }
    
    /*
     * Compute the estimate of the reciprocal condition number.
     */
    if( ae_fp_neq(ainvnm,(double)(0)) )
    {
        *rc = 1/ainvnm;
        *rc = *rc/anorm;
        if( ae_fp_less(*rc,rcondthreshold(_state)) )
        {
            *rc = (double)(0);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine for matrix norm estimation

  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     February 29, 1992
*************************************************************************/
static void rcond_rmatrixestimatenorm(ae_int_t n,
     /* Real    */ ae_vector* v,
     /* Real    */ ae_vector* x,
     /* Integer */ ae_vector* isgn,
     double* est,
     ae_int_t* kase,
     ae_state *_state)
{
    ae_int_t itmax;
    ae_int_t i;
    double t;
    ae_bool flg;
    ae_int_t positer;
    ae_int_t posj;
    ae_int_t posjlast;
    ae_int_t posjump;
    ae_int_t posaltsgn;
    ae_int_t posestold;
    ae_int_t postemp;


    itmax = 5;
    posaltsgn = n+1;
    posestold = n+2;
    postemp = n+3;
    positer = n+1;
    posj = n+2;
    posjlast = n+3;
    posjump = n+4;
    if( *kase==0 )
    {
        ae_vector_set_length(v, n+4, _state);
        ae_vector_set_length(x, n+1, _state);
        ae_vector_set_length(isgn, n+5, _state);
        t = (double)1/(double)n;
        for(i=1; i<=n; i++)
        {
            x->ptr.p_double[i] = t;
        }
        *kase = 1;
        isgn->ptr.p_int[posjump] = 1;
        return;
    }
    
    /*
     *     ................ ENTRY   (JUMP = 1)
     *     FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY A*X.
     */
    if( isgn->ptr.p_int[posjump]==1 )
    {
        if( n==1 )
        {
            v->ptr.p_double[1] = x->ptr.p_double[1];
            *est = ae_fabs(v->ptr.p_double[1], _state);
            *kase = 0;
            return;
        }
        *est = (double)(0);
        for(i=1; i<=n; i++)
        {
            *est = *est+ae_fabs(x->ptr.p_double[i], _state);
        }
        for(i=1; i<=n; i++)
        {
            if( ae_fp_greater_eq(x->ptr.p_double[i],(double)(0)) )
            {
                x->ptr.p_double[i] = (double)(1);
            }
            else
            {
                x->ptr.p_double[i] = (double)(-1);
            }
            isgn->ptr.p_int[i] = ae_sign(x->ptr.p_double[i], _state);
        }
        *kase = 2;
        isgn->ptr.p_int[posjump] = 2;
        return;
    }
    
    /*
     *     ................ ENTRY   (JUMP = 2)
     *     FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY TRANDPOSE(A)*X.
     */
    if( isgn->ptr.p_int[posjump]==2 )
    {
        isgn->ptr.p_int[posj] = 1;
        for(i=2; i<=n; i++)
        {
            if( ae_fp_greater(ae_fabs(x->ptr.p_double[i], _state),ae_fabs(x->ptr.p_double[isgn->ptr.p_int[posj]], _state)) )
            {
                isgn->ptr.p_int[posj] = i;
            }
        }
        isgn->ptr.p_int[positer] = 2;
        
        /*
         * MAIN LOOP - ITERATIONS 2,3,...,ITMAX.
         */
        for(i=1; i<=n; i++)
        {
            x->ptr.p_double[i] = (double)(0);
        }
        x->ptr.p_double[isgn->ptr.p_int[posj]] = (double)(1);
        *kase = 1;
        isgn->ptr.p_int[posjump] = 3;
        return;
    }
    
    /*
     *     ................ ENTRY   (JUMP = 3)
     *     X HAS BEEN OVERWRITTEN BY A*X.
     */
    if( isgn->ptr.p_int[posjump]==3 )
    {
        ae_v_move(&v->ptr.p_double[1], 1, &x->ptr.p_double[1], 1, ae_v_len(1,n));
        v->ptr.p_double[posestold] = *est;
        *est = (double)(0);
        for(i=1; i<=n; i++)
        {
            *est = *est+ae_fabs(v->ptr.p_double[i], _state);
        }
        flg = ae_false;
        for(i=1; i<=n; i++)
        {
            if( (ae_fp_greater_eq(x->ptr.p_double[i],(double)(0))&&isgn->ptr.p_int[i]<0)||(ae_fp_less(x->ptr.p_double[i],(double)(0))&&isgn->ptr.p_int[i]>=0) )
            {
                flg = ae_true;
            }
        }
        
        /*
         * REPEATED SIGN VECTOR DETECTED, HENCE ALGORITHM HAS CONVERGED.
         * OR MAY BE CYCLING.
         */
        if( !flg||ae_fp_less_eq(*est,v->ptr.p_double[posestold]) )
        {
            v->ptr.p_double[posaltsgn] = (double)(1);
            for(i=1; i<=n; i++)
            {
                x->ptr.p_double[i] = v->ptr.p_double[posaltsgn]*(1+(double)(i-1)/(double)(n-1));
                v->ptr.p_double[posaltsgn] = -v->ptr.p_double[posaltsgn];
            }
            *kase = 1;
            isgn->ptr.p_int[posjump] = 5;
            return;
        }
        for(i=1; i<=n; i++)
        {
            if( ae_fp_greater_eq(x->ptr.p_double[i],(double)(0)) )
            {
                x->ptr.p_double[i] = (double)(1);
                isgn->ptr.p_int[i] = 1;
            }
            else
            {
                x->ptr.p_double[i] = (double)(-1);
                isgn->ptr.p_int[i] = -1;
            }
        }
        *kase = 2;
        isgn->ptr.p_int[posjump] = 4;
        return;
    }
    
    /*
     *     ................ ENTRY   (JUMP = 4)
     *     X HAS BEEN OVERWRITTEN BY TRANDPOSE(A)*X.
     */
    if( isgn->ptr.p_int[posjump]==4 )
    {
        isgn->ptr.p_int[posjlast] = isgn->ptr.p_int[posj];
        isgn->ptr.p_int[posj] = 1;
        for(i=2; i<=n; i++)
        {
            if( ae_fp_greater(ae_fabs(x->ptr.p_double[i], _state),ae_fabs(x->ptr.p_double[isgn->ptr.p_int[posj]], _state)) )
            {
                isgn->ptr.p_int[posj] = i;
            }
        }
        if( ae_fp_neq(x->ptr.p_double[isgn->ptr.p_int[posjlast]],ae_fabs(x->ptr.p_double[isgn->ptr.p_int[posj]], _state))&&isgn->ptr.p_int[positer]<itmax )
        {
            isgn->ptr.p_int[positer] = isgn->ptr.p_int[positer]+1;
            for(i=1; i<=n; i++)
            {
                x->ptr.p_double[i] = (double)(0);
            }
            x->ptr.p_double[isgn->ptr.p_int[posj]] = (double)(1);
            *kase = 1;
            isgn->ptr.p_int[posjump] = 3;
            return;
        }
        
        /*
         * ITERATION COMPLETE.  FINAL STAGE.
         */
        v->ptr.p_double[posaltsgn] = (double)(1);
        for(i=1; i<=n; i++)
        {
            x->ptr.p_double[i] = v->ptr.p_double[posaltsgn]*(1+(double)(i-1)/(double)(n-1));
            v->ptr.p_double[posaltsgn] = -v->ptr.p_double[posaltsgn];
        }
        *kase = 1;
        isgn->ptr.p_int[posjump] = 5;
        return;
    }
    
    /*
     *     ................ ENTRY   (JUMP = 5)
     *     X HAS BEEN OVERWRITTEN BY A*X.
     */
    if( isgn->ptr.p_int[posjump]==5 )
    {
        v->ptr.p_double[postemp] = (double)(0);
        for(i=1; i<=n; i++)
        {
            v->ptr.p_double[postemp] = v->ptr.p_double[postemp]+ae_fabs(x->ptr.p_double[i], _state);
        }
        v->ptr.p_double[postemp] = 2*v->ptr.p_double[postemp]/(3*n);
        if( ae_fp_greater(v->ptr.p_double[postemp],*est) )
        {
            ae_v_move(&v->ptr.p_double[1], 1, &x->ptr.p_double[1], 1, ae_v_len(1,n));
            *est = v->ptr.p_double[postemp];
        }
        *kase = 0;
        return;
    }
}


static void rcond_cmatrixestimatenorm(ae_int_t n,
     /* Complex */ ae_vector* v,
     /* Complex */ ae_vector* x,
     double* est,
     ae_int_t* kase,
     /* Integer */ ae_vector* isave,
     /* Real    */ ae_vector* rsave,
     ae_state *_state)
{
    ae_int_t itmax;
    ae_int_t i;
    ae_int_t iter;
    ae_int_t j;
    ae_int_t jlast;
    ae_int_t jump;
    double absxi;
    double altsgn;
    double estold;
    double safmin;
    double temp;


    
    /*
     *Executable Statements ..
     */
    itmax = 5;
    safmin = ae_minrealnumber;
    if( *kase==0 )
    {
        ae_vector_set_length(v, n+1, _state);
        ae_vector_set_length(x, n+1, _state);
        ae_vector_set_length(isave, 5, _state);
        ae_vector_set_length(rsave, 4, _state);
        for(i=1; i<=n; i++)
        {
            x->ptr.p_complex[i] = ae_complex_from_d((double)1/(double)n);
        }
        *kase = 1;
        jump = 1;
        rcond_internalcomplexrcondsaveall(isave, rsave, &i, &iter, &j, &jlast, &jump, &absxi, &altsgn, &estold, &temp, _state);
        return;
    }
    rcond_internalcomplexrcondloadall(isave, rsave, &i, &iter, &j, &jlast, &jump, &absxi, &altsgn, &estold, &temp, _state);
    
    /*
     * ENTRY   (JUMP = 1)
     * FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY A*X.
     */
    if( jump==1 )
    {
        if( n==1 )
        {
            v->ptr.p_complex[1] = x->ptr.p_complex[1];
            *est = ae_c_abs(v->ptr.p_complex[1], _state);
            *kase = 0;
            rcond_internalcomplexrcondsaveall(isave, rsave, &i, &iter, &j, &jlast, &jump, &absxi, &altsgn, &estold, &temp, _state);
            return;
        }
        *est = rcond_internalcomplexrcondscsum1(x, n, _state);
        for(i=1; i<=n; i++)
        {
            absxi = ae_c_abs(x->ptr.p_complex[i], _state);
            if( ae_fp_greater(absxi,safmin) )
            {
                x->ptr.p_complex[i] = ae_c_div_d(x->ptr.p_complex[i],absxi);
            }
            else
            {
                x->ptr.p_complex[i] = ae_complex_from_i(1);
            }
        }
        *kase = 2;
        jump = 2;
        rcond_internalcomplexrcondsaveall(isave, rsave, &i, &iter, &j, &jlast, &jump, &absxi, &altsgn, &estold, &temp, _state);
        return;
    }
    
    /*
     * ENTRY   (JUMP = 2)
     * FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY CTRANS(A)*X.
     */
    if( jump==2 )
    {
        j = rcond_internalcomplexrcondicmax1(x, n, _state);
        iter = 2;
        
        /*
         * MAIN LOOP - ITERATIONS 2,3,...,ITMAX.
         */
        for(i=1; i<=n; i++)
        {
            x->ptr.p_complex[i] = ae_complex_from_i(0);
        }
        x->ptr.p_complex[j] = ae_complex_from_i(1);
        *kase = 1;
        jump = 3;
        rcond_internalcomplexrcondsaveall(isave, rsave, &i, &iter, &j, &jlast, &jump, &absxi, &altsgn, &estold, &temp, _state);
        return;
    }
    
    /*
     * ENTRY   (JUMP = 3)
     * X HAS BEEN OVERWRITTEN BY A*X.
     */
    if( jump==3 )
    {
        ae_v_cmove(&v->ptr.p_complex[1], 1, &x->ptr.p_complex[1], 1, "N", ae_v_len(1,n));
        estold = *est;
        *est = rcond_internalcomplexrcondscsum1(v, n, _state);
        
        /*
         * TEST FOR CYCLING.
         */
        if( ae_fp_less_eq(*est,estold) )
        {
            
            /*
             * ITERATION COMPLETE.  FINAL STAGE.
             */
            altsgn = (double)(1);
            for(i=1; i<=n; i++)
            {
                x->ptr.p_complex[i] = ae_complex_from_d(altsgn*(1+(double)(i-1)/(double)(n-1)));
                altsgn = -altsgn;
            }
            *kase = 1;
            jump = 5;
            rcond_internalcomplexrcondsaveall(isave, rsave, &i, &iter, &j, &jlast, &jump, &absxi, &altsgn, &estold, &temp, _state);
            return;
        }
        for(i=1; i<=n; i++)
        {
            absxi = ae_c_abs(x->ptr.p_complex[i], _state);
            if( ae_fp_greater(absxi,safmin) )
            {
                x->ptr.p_complex[i] = ae_c_div_d(x->ptr.p_complex[i],absxi);
            }
            else
            {
                x->ptr.p_complex[i] = ae_complex_from_i(1);
            }
        }
        *kase = 2;
        jump = 4;
        rcond_internalcomplexrcondsaveall(isave, rsave, &i, &iter, &j, &jlast, &jump, &absxi, &altsgn, &estold, &temp, _state);
        return;
    }
    
    /*
     * ENTRY   (JUMP = 4)
     * X HAS BEEN OVERWRITTEN BY CTRANS(A)*X.
     */
    if( jump==4 )
    {
        jlast = j;
        j = rcond_internalcomplexrcondicmax1(x, n, _state);
        if( ae_fp_neq(ae_c_abs(x->ptr.p_complex[jlast], _state),ae_c_abs(x->ptr.p_complex[j], _state))&&iter<itmax )
        {
            iter = iter+1;
            
            /*
             * MAIN LOOP - ITERATIONS 2,3,...,ITMAX.
             */
            for(i=1; i<=n; i++)
            {
                x->ptr.p_complex[i] = ae_complex_from_i(0);
            }
            x->ptr.p_complex[j] = ae_complex_from_i(1);
            *kase = 1;
            jump = 3;
            rcond_internalcomplexrcondsaveall(isave, rsave, &i, &iter, &j, &jlast, &jump, &absxi, &altsgn, &estold, &temp, _state);
            return;
        }
        
        /*
         * ITERATION COMPLETE.  FINAL STAGE.
         */
        altsgn = (double)(1);
        for(i=1; i<=n; i++)
        {
            x->ptr.p_complex[i] = ae_complex_from_d(altsgn*(1+(double)(i-1)/(double)(n-1)));
            altsgn = -altsgn;
        }
        *kase = 1;
        jump = 5;
        rcond_internalcomplexrcondsaveall(isave, rsave, &i, &iter, &j, &jlast, &jump, &absxi, &altsgn, &estold, &temp, _state);
        return;
    }
    
    /*
     * ENTRY   (JUMP = 5)
     * X HAS BEEN OVERWRITTEN BY A*X.
     */
    if( jump==5 )
    {
        temp = 2*(rcond_internalcomplexrcondscsum1(x, n, _state)/(3*n));
        if( ae_fp_greater(temp,*est) )
        {
            ae_v_cmove(&v->ptr.p_complex[1], 1, &x->ptr.p_complex[1], 1, "N", ae_v_len(1,n));
            *est = temp;
        }
        *kase = 0;
        rcond_internalcomplexrcondsaveall(isave, rsave, &i, &iter, &j, &jlast, &jump, &absxi, &altsgn, &estold, &temp, _state);
        return;
    }
}


static double rcond_internalcomplexrcondscsum1(/* Complex */ ae_vector* x,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    double result;


    result = (double)(0);
    for(i=1; i<=n; i++)
    {
        result = result+ae_c_abs(x->ptr.p_complex[i], _state);
    }
    return result;
}


static ae_int_t rcond_internalcomplexrcondicmax1(/* Complex */ ae_vector* x,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    double m;
    ae_int_t result;


    result = 1;
    m = ae_c_abs(x->ptr.p_complex[1], _state);
    for(i=2; i<=n; i++)
    {
        if( ae_fp_greater(ae_c_abs(x->ptr.p_complex[i], _state),m) )
        {
            result = i;
            m = ae_c_abs(x->ptr.p_complex[i], _state);
        }
    }
    return result;
}


static void rcond_internalcomplexrcondsaveall(/* Integer */ ae_vector* isave,
     /* Real    */ ae_vector* rsave,
     ae_int_t* i,
     ae_int_t* iter,
     ae_int_t* j,
     ae_int_t* jlast,
     ae_int_t* jump,
     double* absxi,
     double* altsgn,
     double* estold,
     double* temp,
     ae_state *_state)
{


    isave->ptr.p_int[0] = *i;
    isave->ptr.p_int[1] = *iter;
    isave->ptr.p_int[2] = *j;
    isave->ptr.p_int[3] = *jlast;
    isave->ptr.p_int[4] = *jump;
    rsave->ptr.p_double[0] = *absxi;
    rsave->ptr.p_double[1] = *altsgn;
    rsave->ptr.p_double[2] = *estold;
    rsave->ptr.p_double[3] = *temp;
}


static void rcond_internalcomplexrcondloadall(/* Integer */ ae_vector* isave,
     /* Real    */ ae_vector* rsave,
     ae_int_t* i,
     ae_int_t* iter,
     ae_int_t* j,
     ae_int_t* jlast,
     ae_int_t* jump,
     double* absxi,
     double* altsgn,
     double* estold,
     double* temp,
     ae_state *_state)
{


    *i = isave->ptr.p_int[0];
    *iter = isave->ptr.p_int[1];
    *j = isave->ptr.p_int[2];
    *jlast = isave->ptr.p_int[3];
    *jump = isave->ptr.p_int[4];
    *absxi = rsave->ptr.p_double[0];
    *altsgn = rsave->ptr.p_double[1];
    *estold = rsave->ptr.p_double[2];
    *temp = rsave->ptr.p_double[3];
}


/*$ End $*/
