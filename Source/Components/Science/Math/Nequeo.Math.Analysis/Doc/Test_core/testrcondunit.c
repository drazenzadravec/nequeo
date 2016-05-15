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


#include <stdafx.h>
#include <stdio.h>
#include "testrcondunit.h"


/*$ Declarations $*/
static double testrcondunit_threshold50 = 0.25;
static double testrcondunit_threshold90 = 0.10;
static void testrcondunit_rmatrixmakeacopy(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_state *_state);
static void testrcondunit_rmatrixdrophalf(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool droplower,
     ae_state *_state);
static void testrcondunit_cmatrixdrophalf(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool droplower,
     ae_state *_state);
static void testrcondunit_rmatrixgenzero(/* Real    */ ae_matrix* a0,
     ae_int_t n,
     ae_state *_state);
static ae_bool testrcondunit_rmatrixinvmattr(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunittriangular,
     ae_state *_state);
static ae_bool testrcondunit_rmatrixinvmatlu(/* Real    */ ae_matrix* a,
     /* Integer */ ae_vector* pivots,
     ae_int_t n,
     ae_state *_state);
static ae_bool testrcondunit_rmatrixinvmat(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state);
static void testrcondunit_rmatrixrefrcond(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double* rc1,
     double* rcinf,
     ae_state *_state);
static void testrcondunit_cmatrixmakeacopy(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_state *_state);
static void testrcondunit_cmatrixgenzero(/* Complex */ ae_matrix* a0,
     ae_int_t n,
     ae_state *_state);
static ae_bool testrcondunit_cmatrixinvmattr(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunittriangular,
     ae_state *_state);
static ae_bool testrcondunit_cmatrixinvmatlu(/* Complex */ ae_matrix* a,
     /* Integer */ ae_vector* pivots,
     ae_int_t n,
     ae_state *_state);
static ae_bool testrcondunit_cmatrixinvmat(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state);
static void testrcondunit_cmatrixrefrcond(/* Complex */ ae_matrix* a,
     ae_int_t n,
     double* rc1,
     double* rcinf,
     ae_state *_state);
static ae_bool testrcondunit_testrmatrixtrrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state);
static ae_bool testrcondunit_testcmatrixtrrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state);
static ae_bool testrcondunit_testrmatrixrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state);
static ae_bool testrcondunit_testspdmatrixrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state);
static ae_bool testrcondunit_testcmatrixrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state);
static ae_bool testrcondunit_testhpdmatrixrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state);


/*$ Body $*/


ae_bool testrcond(ae_bool silent, ae_state *_state)
{
    ae_int_t maxn;
    ae_int_t passcount;
    ae_bool waserrors;
    ae_bool rtrerr;
    ae_bool ctrerr;
    ae_bool rerr;
    ae_bool cerr;
    ae_bool spderr;
    ae_bool hpderr;
    ae_bool result;


    maxn = 10;
    passcount = 100;
    
    /*
     * report
     */
    rtrerr = !testrcondunit_testrmatrixtrrcond(maxn, passcount, _state);
    ctrerr = !testrcondunit_testcmatrixtrrcond(maxn, passcount, _state);
    rerr = !testrcondunit_testrmatrixrcond(maxn, passcount, _state);
    cerr = !testrcondunit_testcmatrixrcond(maxn, passcount, _state);
    spderr = !testrcondunit_testspdmatrixrcond(maxn, passcount, _state);
    hpderr = !testrcondunit_testhpdmatrixrcond(maxn, passcount, _state);
    waserrors = ((((rtrerr||ctrerr)||rerr)||cerr)||spderr)||hpderr;
    if( !silent )
    {
        printf("TESTING RCOND\n");
        printf("REAL TRIANGULAR:                         ");
        if( !rtrerr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("COMPLEX TRIANGULAR:                      ");
        if( !ctrerr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("REAL:                                    ");
        if( !rerr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("SPD:                                     ");
        if( !spderr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("HPD:                                     ");
        if( !hpderr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("COMPLEX:                                 ");
        if( !cerr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        if( waserrors )
        {
            printf("TEST FAILED\n");
        }
        else
        {
            printf("TEST PASSED\n");
        }
        printf("\n\n");
    }
    result = !waserrors;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testrcond(ae_bool silent, ae_state *_state)
{
    return testrcond(silent, _state);
}


/*************************************************************************
Copy
*************************************************************************/
static void testrcondunit_rmatrixmakeacopy(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(b);

    ae_matrix_set_length(b, m-1+1, n-1+1, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            b->ptr.pp_double[i][j] = a->ptr.pp_double[i][j];
        }
    }
}


/*************************************************************************
Drops upper or lower half of the matrix - fills it by special pattern
which may be used later to ensure that this part wasn't changed
*************************************************************************/
static void testrcondunit_rmatrixdrophalf(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool droplower,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( (droplower&&i>j)||(!droplower&&i<j) )
            {
                a->ptr.pp_double[i][j] = (double)(1+2*i+3*j);
            }
        }
    }
}


/*************************************************************************
Drops upper or lower half of the matrix - fills it by special pattern
which may be used later to ensure that this part wasn't changed
*************************************************************************/
static void testrcondunit_cmatrixdrophalf(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool droplower,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( (droplower&&i>j)||(!droplower&&i<j) )
            {
                a->ptr.pp_complex[i][j] = ae_complex_from_i(1+2*i+3*j);
            }
        }
    }
}


/*************************************************************************
Generate matrix with given condition number C (2-norm)
*************************************************************************/
static void testrcondunit_rmatrixgenzero(/* Real    */ ae_matrix* a0,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    ae_matrix_set_length(a0, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a0->ptr.pp_double[i][j] = (double)(0);
        }
    }
}


/*************************************************************************
triangular inverse
*************************************************************************/
static ae_bool testrcondunit_rmatrixinvmattr(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunittriangular,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool nounit;
    ae_int_t i;
    ae_int_t j;
    double v;
    double ajj;
    ae_vector t;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&t, 0, DT_REAL, _state);

    result = ae_true;
    ae_vector_set_length(&t, n-1+1, _state);
    
    /*
     * Test the input parameters.
     */
    nounit = !isunittriangular;
    if( isupper )
    {
        
        /*
         * Compute inverse of upper triangular matrix.
         */
        for(j=0; j<=n-1; j++)
        {
            if( nounit )
            {
                if( ae_fp_eq(a->ptr.pp_double[j][j],(double)(0)) )
                {
                    result = ae_false;
                    ae_frame_leave(_state);
                    return result;
                }
                a->ptr.pp_double[j][j] = 1/a->ptr.pp_double[j][j];
                ajj = -a->ptr.pp_double[j][j];
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
                ae_v_move(&t.ptr.p_double[0], 1, &a->ptr.pp_double[0][j], a->stride, ae_v_len(0,j-1));
                for(i=0; i<=j-1; i++)
                {
                    if( i<j-1 )
                    {
                        v = ae_v_dotproduct(&a->ptr.pp_double[i][i+1], 1, &t.ptr.p_double[i+1], 1, ae_v_len(i+1,j-1));
                    }
                    else
                    {
                        v = (double)(0);
                    }
                    if( nounit )
                    {
                        a->ptr.pp_double[i][j] = v+a->ptr.pp_double[i][i]*t.ptr.p_double[i];
                    }
                    else
                    {
                        a->ptr.pp_double[i][j] = v+t.ptr.p_double[i];
                    }
                }
                ae_v_muld(&a->ptr.pp_double[0][j], a->stride, ae_v_len(0,j-1), ajj);
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
            if( nounit )
            {
                if( ae_fp_eq(a->ptr.pp_double[j][j],(double)(0)) )
                {
                    result = ae_false;
                    ae_frame_leave(_state);
                    return result;
                }
                a->ptr.pp_double[j][j] = 1/a->ptr.pp_double[j][j];
                ajj = -a->ptr.pp_double[j][j];
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
                ae_v_move(&t.ptr.p_double[j+1], 1, &a->ptr.pp_double[j+1][j], a->stride, ae_v_len(j+1,n-1));
                for(i=j+1; i<=n-1; i++)
                {
                    if( i>j+1 )
                    {
                        v = ae_v_dotproduct(&a->ptr.pp_double[i][j+1], 1, &t.ptr.p_double[j+1], 1, ae_v_len(j+1,i-1));
                    }
                    else
                    {
                        v = (double)(0);
                    }
                    if( nounit )
                    {
                        a->ptr.pp_double[i][j] = v+a->ptr.pp_double[i][i]*t.ptr.p_double[i];
                    }
                    else
                    {
                        a->ptr.pp_double[i][j] = v+t.ptr.p_double[i];
                    }
                }
                ae_v_muld(&a->ptr.pp_double[j+1][j], a->stride, ae_v_len(j+1,n-1), ajj);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
LU inverse
*************************************************************************/
static ae_bool testrcondunit_rmatrixinvmatlu(/* Real    */ ae_matrix* a,
     /* Integer */ ae_vector* pivots,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_int_t i;
    ae_int_t j;
    ae_int_t jp;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&work, 0, DT_REAL, _state);

    result = ae_true;
    
    /*
     * Quick return if possible
     */
    if( n==0 )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_vector_set_length(&work, n-1+1, _state);
    
    /*
     * Form inv(U)
     */
    if( !testrcondunit_rmatrixinvmattr(a, n, ae_true, ae_false, _state) )
    {
        result = ae_false;
        ae_frame_leave(_state);
        return result;
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
            work.ptr.p_double[i] = a->ptr.pp_double[i][j];
            a->ptr.pp_double[i][j] = (double)(0);
        }
        
        /*
         * Compute current column of inv(A).
         */
        if( j<n-1 )
        {
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&a->ptr.pp_double[i][j+1], 1, &work.ptr.p_double[j+1], 1, ae_v_len(j+1,n-1));
                a->ptr.pp_double[i][j] = a->ptr.pp_double[i][j]-v;
            }
        }
    }
    
    /*
     * Apply column interchanges.
     */
    for(j=n-2; j>=0; j--)
    {
        jp = pivots->ptr.p_int[j];
        if( jp!=j )
        {
            ae_v_move(&work.ptr.p_double[0], 1, &a->ptr.pp_double[0][j], a->stride, ae_v_len(0,n-1));
            ae_v_move(&a->ptr.pp_double[0][j], a->stride, &a->ptr.pp_double[0][jp], a->stride, ae_v_len(0,n-1));
            ae_v_move(&a->ptr.pp_double[0][jp], a->stride, &work.ptr.p_double[0], 1, ae_v_len(0,n-1));
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Matrix inverse
*************************************************************************/
static ae_bool testrcondunit_rmatrixinvmat(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector pivots;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&pivots, 0, DT_INT, _state);

    rmatrixlu(a, n, n, &pivots, _state);
    result = testrcondunit_rmatrixinvmatlu(a, &pivots, n, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
reference RCond
*************************************************************************/
static void testrcondunit_rmatrixrefrcond(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double* rc1,
     double* rcinf,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix inva;
    double nrm1a;
    double nrminfa;
    double nrm1inva;
    double nrminfinva;
    double v;
    ae_int_t k;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *rc1 = 0;
    *rcinf = 0;
    ae_matrix_init(&inva, 0, 0, DT_REAL, _state);

    
    /*
     * inv A
     */
    testrcondunit_rmatrixmakeacopy(a, n, n, &inva, _state);
    if( !testrcondunit_rmatrixinvmat(&inva, n, _state) )
    {
        *rc1 = (double)(0);
        *rcinf = (double)(0);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * norm A
     */
    nrm1a = (double)(0);
    nrminfa = (double)(0);
    for(k=0; k<=n-1; k++)
    {
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v = v+ae_fabs(a->ptr.pp_double[i][k], _state);
        }
        nrm1a = ae_maxreal(nrm1a, v, _state);
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v = v+ae_fabs(a->ptr.pp_double[k][i], _state);
        }
        nrminfa = ae_maxreal(nrminfa, v, _state);
    }
    
    /*
     * norm inv A
     */
    nrm1inva = (double)(0);
    nrminfinva = (double)(0);
    for(k=0; k<=n-1; k++)
    {
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v = v+ae_fabs(inva.ptr.pp_double[i][k], _state);
        }
        nrm1inva = ae_maxreal(nrm1inva, v, _state);
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v = v+ae_fabs(inva.ptr.pp_double[k][i], _state);
        }
        nrminfinva = ae_maxreal(nrminfinva, v, _state);
    }
    
    /*
     * result
     */
    *rc1 = nrm1inva*nrm1a;
    *rcinf = nrminfinva*nrminfa;
    ae_frame_leave(_state);
}


/*************************************************************************
Copy
*************************************************************************/
static void testrcondunit_cmatrixmakeacopy(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(b);

    ae_matrix_set_length(b, m-1+1, n-1+1, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            b->ptr.pp_complex[i][j] = a->ptr.pp_complex[i][j];
        }
    }
}


/*************************************************************************
Generate matrix with given condition number C (2-norm)
*************************************************************************/
static void testrcondunit_cmatrixgenzero(/* Complex */ ae_matrix* a0,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    ae_matrix_set_length(a0, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a0->ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
}


/*************************************************************************
triangular inverse
*************************************************************************/
static ae_bool testrcondunit_cmatrixinvmattr(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunittriangular,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool nounit;
    ae_int_t i;
    ae_int_t j;
    ae_complex v;
    ae_complex ajj;
    ae_vector t;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&t, 0, DT_COMPLEX, _state);

    result = ae_true;
    ae_vector_set_length(&t, n-1+1, _state);
    
    /*
     * Test the input parameters.
     */
    nounit = !isunittriangular;
    if( isupper )
    {
        
        /*
         * Compute inverse of upper triangular matrix.
         */
        for(j=0; j<=n-1; j++)
        {
            if( nounit )
            {
                if( ae_c_eq_d(a->ptr.pp_complex[j][j],(double)(0)) )
                {
                    result = ae_false;
                    ae_frame_leave(_state);
                    return result;
                }
                a->ptr.pp_complex[j][j] = ae_c_d_div(1,a->ptr.pp_complex[j][j]);
                ajj = ae_c_neg(a->ptr.pp_complex[j][j]);
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
                ae_v_cmove(&t.ptr.p_complex[0], 1, &a->ptr.pp_complex[0][j], a->stride, "N", ae_v_len(0,j-1));
                for(i=0; i<=j-1; i++)
                {
                    if( i<j-1 )
                    {
                        v = ae_v_cdotproduct(&a->ptr.pp_complex[i][i+1], 1, "N", &t.ptr.p_complex[i+1], 1, "N", ae_v_len(i+1,j-1));
                    }
                    else
                    {
                        v = ae_complex_from_i(0);
                    }
                    if( nounit )
                    {
                        a->ptr.pp_complex[i][j] = ae_c_add(v,ae_c_mul(a->ptr.pp_complex[i][i],t.ptr.p_complex[i]));
                    }
                    else
                    {
                        a->ptr.pp_complex[i][j] = ae_c_add(v,t.ptr.p_complex[i]);
                    }
                }
                ae_v_cmulc(&a->ptr.pp_complex[0][j], a->stride, ae_v_len(0,j-1), ajj);
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
            if( nounit )
            {
                if( ae_c_eq_d(a->ptr.pp_complex[j][j],(double)(0)) )
                {
                    result = ae_false;
                    ae_frame_leave(_state);
                    return result;
                }
                a->ptr.pp_complex[j][j] = ae_c_d_div(1,a->ptr.pp_complex[j][j]);
                ajj = ae_c_neg(a->ptr.pp_complex[j][j]);
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
                ae_v_cmove(&t.ptr.p_complex[j+1], 1, &a->ptr.pp_complex[j+1][j], a->stride, "N", ae_v_len(j+1,n-1));
                for(i=j+1; i<=n-1; i++)
                {
                    if( i>j+1 )
                    {
                        v = ae_v_cdotproduct(&a->ptr.pp_complex[i][j+1], 1, "N", &t.ptr.p_complex[j+1], 1, "N", ae_v_len(j+1,i-1));
                    }
                    else
                    {
                        v = ae_complex_from_i(0);
                    }
                    if( nounit )
                    {
                        a->ptr.pp_complex[i][j] = ae_c_add(v,ae_c_mul(a->ptr.pp_complex[i][i],t.ptr.p_complex[i]));
                    }
                    else
                    {
                        a->ptr.pp_complex[i][j] = ae_c_add(v,t.ptr.p_complex[i]);
                    }
                }
                ae_v_cmulc(&a->ptr.pp_complex[j+1][j], a->stride, ae_v_len(j+1,n-1), ajj);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
LU inverse
*************************************************************************/
static ae_bool testrcondunit_cmatrixinvmatlu(/* Complex */ ae_matrix* a,
     /* Integer */ ae_vector* pivots,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_int_t i;
    ae_int_t j;
    ae_int_t jp;
    ae_complex v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&work, 0, DT_COMPLEX, _state);

    result = ae_true;
    
    /*
     * Quick return if possible
     */
    if( n==0 )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_vector_set_length(&work, n-1+1, _state);
    
    /*
     * Form inv(U)
     */
    if( !testrcondunit_cmatrixinvmattr(a, n, ae_true, ae_false, _state) )
    {
        result = ae_false;
        ae_frame_leave(_state);
        return result;
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
            work.ptr.p_complex[i] = a->ptr.pp_complex[i][j];
            a->ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
        
        /*
         * Compute current column of inv(A).
         */
        if( j<n-1 )
        {
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_cdotproduct(&a->ptr.pp_complex[i][j+1], 1, "N", &work.ptr.p_complex[j+1], 1, "N", ae_v_len(j+1,n-1));
                a->ptr.pp_complex[i][j] = ae_c_sub(a->ptr.pp_complex[i][j],v);
            }
        }
    }
    
    /*
     * Apply column interchanges.
     */
    for(j=n-2; j>=0; j--)
    {
        jp = pivots->ptr.p_int[j];
        if( jp!=j )
        {
            ae_v_cmove(&work.ptr.p_complex[0], 1, &a->ptr.pp_complex[0][j], a->stride, "N", ae_v_len(0,n-1));
            ae_v_cmove(&a->ptr.pp_complex[0][j], a->stride, &a->ptr.pp_complex[0][jp], a->stride, "N", ae_v_len(0,n-1));
            ae_v_cmove(&a->ptr.pp_complex[0][jp], a->stride, &work.ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Matrix inverse
*************************************************************************/
static ae_bool testrcondunit_cmatrixinvmat(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector pivots;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&pivots, 0, DT_INT, _state);

    cmatrixlu(a, n, n, &pivots, _state);
    result = testrcondunit_cmatrixinvmatlu(a, &pivots, n, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
reference RCond
*************************************************************************/
static void testrcondunit_cmatrixrefrcond(/* Complex */ ae_matrix* a,
     ae_int_t n,
     double* rc1,
     double* rcinf,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix inva;
    double nrm1a;
    double nrminfa;
    double nrm1inva;
    double nrminfinva;
    double v;
    ae_int_t k;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *rc1 = 0;
    *rcinf = 0;
    ae_matrix_init(&inva, 0, 0, DT_COMPLEX, _state);

    
    /*
     * inv A
     */
    testrcondunit_cmatrixmakeacopy(a, n, n, &inva, _state);
    if( !testrcondunit_cmatrixinvmat(&inva, n, _state) )
    {
        *rc1 = (double)(0);
        *rcinf = (double)(0);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * norm A
     */
    nrm1a = (double)(0);
    nrminfa = (double)(0);
    for(k=0; k<=n-1; k++)
    {
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v = v+ae_c_abs(a->ptr.pp_complex[i][k], _state);
        }
        nrm1a = ae_maxreal(nrm1a, v, _state);
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v = v+ae_c_abs(a->ptr.pp_complex[k][i], _state);
        }
        nrminfa = ae_maxreal(nrminfa, v, _state);
    }
    
    /*
     * norm inv A
     */
    nrm1inva = (double)(0);
    nrminfinva = (double)(0);
    for(k=0; k<=n-1; k++)
    {
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v = v+ae_c_abs(inva.ptr.pp_complex[i][k], _state);
        }
        nrm1inva = ae_maxreal(nrm1inva, v, _state);
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v = v+ae_c_abs(inva.ptr.pp_complex[k][i], _state);
        }
        nrminfinva = ae_maxreal(nrminfinva, v, _state);
    }
    
    /*
     * result
     */
    *rc1 = nrm1inva*nrm1a;
    *rcinf = nrminfinva*nrminfa;
    ae_frame_leave(_state);
}


/*************************************************************************
Returns True for successful test, False - for failed test
*************************************************************************/
static ae_bool testrcondunit_testrmatrixtrrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix ea;
    ae_vector p;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t j2;
    ae_int_t pass;
    ae_bool err50;
    ae_bool err90;
    ae_bool errspec;
    ae_bool errless;
    double erc1;
    double ercinf;
    ae_vector q50;
    ae_vector q90;
    double v;
    ae_bool isupper;
    ae_bool isunit;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ea, 0, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_vector_init(&q50, 0, DT_REAL, _state);
    ae_vector_init(&q90, 0, DT_REAL, _state);

    err50 = ae_false;
    err90 = ae_false;
    errless = ae_false;
    errspec = ae_false;
    ae_vector_set_length(&q50, 2, _state);
    ae_vector_set_length(&q90, 2, _state);
    for(n=1; n<=maxn; n++)
    {
        
        /*
         * special test for zero matrix
         */
        testrcondunit_rmatrixgenzero(&a, n, _state);
        errspec = errspec||ae_fp_neq(rmatrixtrrcond1(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
        errspec = errspec||ae_fp_neq(rmatrixtrrcondinf(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
        
        /*
         * general test
         */
        ae_matrix_set_length(&a, n, n, _state);
        for(i=0; i<=1; i++)
        {
            q50.ptr.p_double[i] = (double)(0);
            q90.ptr.p_double[i] = (double)(0);
        }
        for(pass=1; pass<=passcount; pass++)
        {
            isupper = ae_fp_greater(ae_randomreal(_state),0.5);
            isunit = ae_fp_greater(ae_randomreal(_state),0.5);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = ae_randomreal(_state)-0.5;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_double[i][i] = 1+ae_randomreal(_state);
            }
            testrcondunit_rmatrixmakeacopy(&a, n, n, &ea, _state);
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = 0;
                    j2 = i-1;
                }
                else
                {
                    j1 = i+1;
                    j2 = n-1;
                }
                for(j=j1; j<=j2; j++)
                {
                    ea.ptr.pp_double[i][j] = (double)(0);
                }
                if( isunit )
                {
                    ea.ptr.pp_double[i][i] = (double)(1);
                }
            }
            testrcondunit_rmatrixrefrcond(&ea, n, &erc1, &ercinf, _state);
            
            /*
             * 1-norm
             */
            v = 1/rmatrixtrrcond1(&a, n, isupper, isunit, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*erc1) )
            {
                q50.ptr.p_double[0] = q50.ptr.p_double[0]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*erc1) )
            {
                q90.ptr.p_double[0] = q90.ptr.p_double[0]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,erc1*1.001);
            
            /*
             * Inf-norm
             */
            v = 1/rmatrixtrrcondinf(&a, n, isupper, isunit, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*ercinf) )
            {
                q50.ptr.p_double[1] = q50.ptr.p_double[1]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*ercinf) )
            {
                q90.ptr.p_double[1] = q90.ptr.p_double[1]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,ercinf*1.001);
        }
        for(i=0; i<=1; i++)
        {
            err50 = err50||ae_fp_less(q50.ptr.p_double[i],0.50);
            err90 = err90||ae_fp_less(q90.ptr.p_double[i],0.90);
        }
        
        /*
         * degenerate matrix test
         */
        if( n>=3 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 0.0;
                }
            }
            a.ptr.pp_double[0][0] = (double)(1);
            a.ptr.pp_double[n-1][n-1] = (double)(1);
            errspec = errspec||ae_fp_neq(rmatrixtrrcond1(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
            errspec = errspec||ae_fp_neq(rmatrixtrrcondinf(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
        }
        
        /*
         * near-degenerate matrix test
         */
        if( n>=2 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 0.0;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_double[i][i] = (double)(1);
            }
            i = ae_randominteger(n, _state);
            a.ptr.pp_double[i][i] = 0.1*ae_maxrealnumber;
            errspec = errspec||ae_fp_neq(rmatrixtrrcond1(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
            errspec = errspec||ae_fp_neq(rmatrixtrrcondinf(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
        }
    }
    
    /*
     * report
     */
    result = !(((err50||err90)||errless)||errspec);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Returns True for successful test, False - for failed test
*************************************************************************/
static ae_bool testrcondunit_testcmatrixtrrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix ea;
    ae_vector p;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t j2;
    ae_int_t pass;
    ae_bool err50;
    ae_bool err90;
    ae_bool errspec;
    ae_bool errless;
    double erc1;
    double ercinf;
    ae_vector q50;
    ae_vector q90;
    double v;
    ae_bool isupper;
    ae_bool isunit;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&ea, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_vector_init(&q50, 0, DT_REAL, _state);
    ae_vector_init(&q90, 0, DT_REAL, _state);

    err50 = ae_false;
    err90 = ae_false;
    errless = ae_false;
    errspec = ae_false;
    ae_vector_set_length(&q50, 2, _state);
    ae_vector_set_length(&q90, 2, _state);
    for(n=1; n<=maxn; n++)
    {
        
        /*
         * special test for zero matrix
         */
        testrcondunit_cmatrixgenzero(&a, n, _state);
        errspec = errspec||ae_fp_neq(cmatrixtrrcond1(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
        errspec = errspec||ae_fp_neq(cmatrixtrrcondinf(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
        
        /*
         * general test
         */
        ae_matrix_set_length(&a, n, n, _state);
        for(i=0; i<=1; i++)
        {
            q50.ptr.p_double[i] = (double)(0);
            q90.ptr.p_double[i] = (double)(0);
        }
        for(pass=1; pass<=passcount; pass++)
        {
            isupper = ae_fp_greater(ae_randomreal(_state),0.5);
            isunit = ae_fp_greater(ae_randomreal(_state),0.5);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_complex[i][j].x = ae_randomreal(_state)-0.5;
                    a.ptr.pp_complex[i][j].y = ae_randomreal(_state)-0.5;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_complex[i][i].x = 1+ae_randomreal(_state);
                a.ptr.pp_complex[i][i].y = 1+ae_randomreal(_state);
            }
            testrcondunit_cmatrixmakeacopy(&a, n, n, &ea, _state);
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = 0;
                    j2 = i-1;
                }
                else
                {
                    j1 = i+1;
                    j2 = n-1;
                }
                for(j=j1; j<=j2; j++)
                {
                    ea.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                }
                if( isunit )
                {
                    ea.ptr.pp_complex[i][i] = ae_complex_from_i(1);
                }
            }
            testrcondunit_cmatrixrefrcond(&ea, n, &erc1, &ercinf, _state);
            
            /*
             * 1-norm
             */
            v = 1/cmatrixtrrcond1(&a, n, isupper, isunit, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*erc1) )
            {
                q50.ptr.p_double[0] = q50.ptr.p_double[0]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*erc1) )
            {
                q90.ptr.p_double[0] = q90.ptr.p_double[0]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,erc1*1.001);
            
            /*
             * Inf-norm
             */
            v = 1/cmatrixtrrcondinf(&a, n, isupper, isunit, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*ercinf) )
            {
                q50.ptr.p_double[1] = q50.ptr.p_double[1]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*ercinf) )
            {
                q90.ptr.p_double[1] = q90.ptr.p_double[1]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,ercinf*1.001);
        }
        for(i=0; i<=1; i++)
        {
            err50 = err50||ae_fp_less(q50.ptr.p_double[i],0.50);
            err90 = err90||ae_fp_less(q90.ptr.p_double[i],0.90);
        }
        
        /*
         * degenerate matrix test
         */
        if( n>=3 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
                }
            }
            a.ptr.pp_complex[0][0] = ae_complex_from_i(1);
            a.ptr.pp_complex[n-1][n-1] = ae_complex_from_i(1);
            errspec = errspec||ae_fp_neq(cmatrixtrrcond1(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
            errspec = errspec||ae_fp_neq(cmatrixtrrcondinf(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
        }
        
        /*
         * near-degenerate matrix test
         */
        if( n>=2 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_complex[i][i] = ae_complex_from_i(1);
            }
            i = ae_randominteger(n, _state);
            a.ptr.pp_complex[i][i] = ae_complex_from_d(0.1*ae_maxrealnumber);
            errspec = errspec||ae_fp_neq(cmatrixtrrcond1(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
            errspec = errspec||ae_fp_neq(cmatrixtrrcondinf(&a, n, ae_fp_greater(ae_randomreal(_state),0.5), ae_false, _state),(double)(0));
        }
    }
    
    /*
     * report
     */
    result = !(((err50||err90)||errless)||errspec);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Returns True for successful test, False - for failed test
*************************************************************************/
static ae_bool testrcondunit_testrmatrixrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix lua;
    ae_vector p;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_bool err50;
    ae_bool err90;
    ae_bool errspec;
    ae_bool errless;
    double erc1;
    double ercinf;
    ae_vector q50;
    ae_vector q90;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&lua, 0, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_vector_init(&q50, 0, DT_REAL, _state);
    ae_vector_init(&q90, 0, DT_REAL, _state);

    err50 = ae_false;
    err90 = ae_false;
    errless = ae_false;
    errspec = ae_false;
    ae_vector_set_length(&q50, 3+1, _state);
    ae_vector_set_length(&q90, 3+1, _state);
    for(n=1; n<=maxn; n++)
    {
        
        /*
         * special test for zero matrix
         */
        testrcondunit_rmatrixgenzero(&a, n, _state);
        testrcondunit_rmatrixmakeacopy(&a, n, n, &lua, _state);
        rmatrixlu(&lua, n, n, &p, _state);
        errspec = errspec||ae_fp_neq(rmatrixrcond1(&a, n, _state),(double)(0));
        errspec = errspec||ae_fp_neq(rmatrixrcondinf(&a, n, _state),(double)(0));
        errspec = errspec||ae_fp_neq(rmatrixlurcond1(&lua, n, _state),(double)(0));
        errspec = errspec||ae_fp_neq(rmatrixlurcondinf(&lua, n, _state),(double)(0));
        
        /*
         * general test
         */
        ae_matrix_set_length(&a, n-1+1, n-1+1, _state);
        for(i=0; i<=3; i++)
        {
            q50.ptr.p_double[i] = (double)(0);
            q90.ptr.p_double[i] = (double)(0);
        }
        for(pass=1; pass<=passcount; pass++)
        {
            rmatrixrndcond(n, ae_exp(ae_randomreal(_state)*ae_log((double)(1000), _state), _state), &a, _state);
            testrcondunit_rmatrixmakeacopy(&a, n, n, &lua, _state);
            rmatrixlu(&lua, n, n, &p, _state);
            testrcondunit_rmatrixrefrcond(&a, n, &erc1, &ercinf, _state);
            
            /*
             * 1-norm, normal
             */
            v = 1/rmatrixrcond1(&a, n, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*erc1) )
            {
                q50.ptr.p_double[0] = q50.ptr.p_double[0]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*erc1) )
            {
                q90.ptr.p_double[0] = q90.ptr.p_double[0]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,erc1*1.001);
            
            /*
             * 1-norm, LU
             */
            v = 1/rmatrixlurcond1(&lua, n, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*erc1) )
            {
                q50.ptr.p_double[1] = q50.ptr.p_double[1]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*erc1) )
            {
                q90.ptr.p_double[1] = q90.ptr.p_double[1]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,erc1*1.001);
            
            /*
             * Inf-norm, normal
             */
            v = 1/rmatrixrcondinf(&a, n, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*ercinf) )
            {
                q50.ptr.p_double[2] = q50.ptr.p_double[2]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*ercinf) )
            {
                q90.ptr.p_double[2] = q90.ptr.p_double[2]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,ercinf*1.001);
            
            /*
             * Inf-norm, LU
             */
            v = 1/rmatrixlurcondinf(&lua, n, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*ercinf) )
            {
                q50.ptr.p_double[3] = q50.ptr.p_double[3]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*ercinf) )
            {
                q90.ptr.p_double[3] = q90.ptr.p_double[3]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,ercinf*1.001);
        }
        for(i=0; i<=3; i++)
        {
            err50 = err50||ae_fp_less(q50.ptr.p_double[i],0.50);
            err90 = err90||ae_fp_less(q90.ptr.p_double[i],0.90);
        }
        
        /*
         * degenerate matrix test
         */
        if( n>=3 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 0.0;
                }
            }
            a.ptr.pp_double[0][0] = (double)(1);
            a.ptr.pp_double[n-1][n-1] = (double)(1);
            errspec = errspec||ae_fp_neq(rmatrixrcond1(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(rmatrixrcondinf(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(rmatrixlurcond1(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(rmatrixlurcondinf(&a, n, _state),(double)(0));
        }
        
        /*
         * near-degenerate matrix test
         */
        if( n>=2 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 0.0;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_double[i][i] = (double)(1);
            }
            i = ae_randominteger(n, _state);
            a.ptr.pp_double[i][i] = 0.1*ae_maxrealnumber;
            errspec = errspec||ae_fp_neq(rmatrixrcond1(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(rmatrixrcondinf(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(rmatrixlurcond1(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(rmatrixlurcondinf(&a, n, _state),(double)(0));
        }
    }
    
    /*
     * report
     */
    result = !(((err50||err90)||errless)||errspec);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Returns True for successful test, False - for failed test
*************************************************************************/
static ae_bool testrcondunit_testspdmatrixrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix cha;
    ae_vector p;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_bool err50;
    ae_bool err90;
    ae_bool errspec;
    ae_bool errless;
    ae_bool isupper;
    double erc1;
    double ercinf;
    ae_vector q50;
    ae_vector q90;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cha, 0, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_vector_init(&q50, 0, DT_REAL, _state);
    ae_vector_init(&q90, 0, DT_REAL, _state);

    err50 = ae_false;
    err90 = ae_false;
    errless = ae_false;
    errspec = ae_false;
    ae_vector_set_length(&q50, 2, _state);
    ae_vector_set_length(&q90, 2, _state);
    for(n=1; n<=maxn; n++)
    {
        isupper = ae_fp_greater(ae_randomreal(_state),0.5);
        
        /*
         * general test
         */
        ae_matrix_set_length(&a, n, n, _state);
        for(i=0; i<=1; i++)
        {
            q50.ptr.p_double[i] = (double)(0);
            q90.ptr.p_double[i] = (double)(0);
        }
        for(pass=1; pass<=passcount; pass++)
        {
            spdmatrixrndcond(n, ae_exp(ae_randomreal(_state)*ae_log((double)(1000), _state), _state), &a, _state);
            testrcondunit_rmatrixrefrcond(&a, n, &erc1, &ercinf, _state);
            testrcondunit_rmatrixdrophalf(&a, n, isupper, _state);
            testrcondunit_rmatrixmakeacopy(&a, n, n, &cha, _state);
            spdmatrixcholesky(&cha, n, isupper, _state);
            
            /*
             * normal
             */
            v = 1/spdmatrixrcond(&a, n, isupper, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*erc1) )
            {
                q50.ptr.p_double[0] = q50.ptr.p_double[0]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*erc1) )
            {
                q90.ptr.p_double[0] = q90.ptr.p_double[0]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,erc1*1.001);
            
            /*
             * Cholesky
             */
            v = 1/spdmatrixcholeskyrcond(&cha, n, isupper, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*erc1) )
            {
                q50.ptr.p_double[1] = q50.ptr.p_double[1]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*erc1) )
            {
                q90.ptr.p_double[1] = q90.ptr.p_double[1]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,erc1*1.001);
        }
        for(i=0; i<=1; i++)
        {
            err50 = err50||ae_fp_less(q50.ptr.p_double[i],0.50);
            err90 = err90||ae_fp_less(q90.ptr.p_double[i],0.90);
        }
        
        /*
         * degenerate matrix test
         */
        if( n>=3 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 0.0;
                }
            }
            a.ptr.pp_double[0][0] = (double)(1);
            a.ptr.pp_double[n-1][n-1] = (double)(1);
            errspec = errspec||ae_fp_neq(spdmatrixrcond(&a, n, isupper, _state),(double)(-1));
            errspec = errspec||ae_fp_neq(spdmatrixcholeskyrcond(&a, n, isupper, _state),(double)(0));
        }
        
        /*
         * near-degenerate matrix test
         */
        if( n>=2 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 0.0;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_double[i][i] = (double)(1);
            }
            i = ae_randominteger(n, _state);
            a.ptr.pp_double[i][i] = 0.1*ae_maxrealnumber;
            errspec = errspec||ae_fp_neq(spdmatrixrcond(&a, n, isupper, _state),(double)(0));
            errspec = errspec||ae_fp_neq(spdmatrixcholeskyrcond(&a, n, isupper, _state),(double)(0));
        }
    }
    
    /*
     * report
     */
    result = !(((err50||err90)||errless)||errspec);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Returns True for successful test, False - for failed test
*************************************************************************/
static ae_bool testrcondunit_testcmatrixrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix lua;
    ae_vector p;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_bool err50;
    ae_bool err90;
    ae_bool errless;
    ae_bool errspec;
    double erc1;
    double ercinf;
    ae_vector q50;
    ae_vector q90;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&lua, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_vector_init(&q50, 0, DT_REAL, _state);
    ae_vector_init(&q90, 0, DT_REAL, _state);

    ae_vector_set_length(&q50, 3+1, _state);
    ae_vector_set_length(&q90, 3+1, _state);
    err50 = ae_false;
    err90 = ae_false;
    errless = ae_false;
    errspec = ae_false;
    
    /*
     * process
     */
    for(n=1; n<=maxn; n++)
    {
        
        /*
         * special test for zero matrix
         */
        testrcondunit_cmatrixgenzero(&a, n, _state);
        testrcondunit_cmatrixmakeacopy(&a, n, n, &lua, _state);
        cmatrixlu(&lua, n, n, &p, _state);
        errspec = errspec||ae_fp_neq(cmatrixrcond1(&a, n, _state),(double)(0));
        errspec = errspec||ae_fp_neq(cmatrixrcondinf(&a, n, _state),(double)(0));
        errspec = errspec||ae_fp_neq(cmatrixlurcond1(&lua, n, _state),(double)(0));
        errspec = errspec||ae_fp_neq(cmatrixlurcondinf(&lua, n, _state),(double)(0));
        
        /*
         * general test
         */
        ae_matrix_set_length(&a, n-1+1, n-1+1, _state);
        for(i=0; i<=3; i++)
        {
            q50.ptr.p_double[i] = (double)(0);
            q90.ptr.p_double[i] = (double)(0);
        }
        for(pass=1; pass<=passcount; pass++)
        {
            cmatrixrndcond(n, ae_exp(ae_randomreal(_state)*ae_log((double)(1000), _state), _state), &a, _state);
            testrcondunit_cmatrixmakeacopy(&a, n, n, &lua, _state);
            cmatrixlu(&lua, n, n, &p, _state);
            testrcondunit_cmatrixrefrcond(&a, n, &erc1, &ercinf, _state);
            
            /*
             * 1-norm, normal
             */
            v = 1/cmatrixrcond1(&a, n, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*erc1) )
            {
                q50.ptr.p_double[0] = q50.ptr.p_double[0]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*erc1) )
            {
                q90.ptr.p_double[0] = q90.ptr.p_double[0]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,erc1*1.001);
            
            /*
             * 1-norm, LU
             */
            v = 1/cmatrixlurcond1(&lua, n, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*erc1) )
            {
                q50.ptr.p_double[1] = q50.ptr.p_double[1]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*erc1) )
            {
                q90.ptr.p_double[1] = q90.ptr.p_double[1]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,erc1*1.001);
            
            /*
             * Inf-norm, normal
             */
            v = 1/cmatrixrcondinf(&a, n, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*ercinf) )
            {
                q50.ptr.p_double[2] = q50.ptr.p_double[2]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*ercinf) )
            {
                q90.ptr.p_double[2] = q90.ptr.p_double[2]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,ercinf*1.001);
            
            /*
             * Inf-norm, LU
             */
            v = 1/cmatrixlurcondinf(&lua, n, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*ercinf) )
            {
                q50.ptr.p_double[3] = q50.ptr.p_double[3]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*ercinf) )
            {
                q90.ptr.p_double[3] = q90.ptr.p_double[3]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,ercinf*1.001);
        }
        for(i=0; i<=3; i++)
        {
            err50 = err50||ae_fp_less(q50.ptr.p_double[i],0.50);
            err90 = err90||ae_fp_less(q90.ptr.p_double[i],0.90);
        }
        
        /*
         * degenerate matrix test
         */
        if( n>=3 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
                }
            }
            a.ptr.pp_complex[0][0] = ae_complex_from_i(1);
            a.ptr.pp_complex[n-1][n-1] = ae_complex_from_i(1);
            errspec = errspec||ae_fp_neq(cmatrixrcond1(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(cmatrixrcondinf(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(cmatrixlurcond1(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(cmatrixlurcondinf(&a, n, _state),(double)(0));
        }
        
        /*
         * near-degenerate matrix test
         */
        if( n>=2 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_complex[i][i] = ae_complex_from_i(1);
            }
            i = ae_randominteger(n, _state);
            a.ptr.pp_complex[i][i] = ae_complex_from_d(0.1*ae_maxrealnumber);
            errspec = errspec||ae_fp_neq(cmatrixrcond1(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(cmatrixrcondinf(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(cmatrixlurcond1(&a, n, _state),(double)(0));
            errspec = errspec||ae_fp_neq(cmatrixlurcondinf(&a, n, _state),(double)(0));
        }
    }
    
    /*
     * report
     */
    result = !(((err50||err90)||errless)||errspec);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Returns True for successful test, False - for failed test
*************************************************************************/
static ae_bool testrcondunit_testhpdmatrixrcond(ae_int_t maxn,
     ae_int_t passcount,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix cha;
    ae_vector p;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_bool err50;
    ae_bool err90;
    ae_bool errspec;
    ae_bool errless;
    ae_bool isupper;
    double erc1;
    double ercinf;
    ae_vector q50;
    ae_vector q90;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cha, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_vector_init(&q50, 0, DT_REAL, _state);
    ae_vector_init(&q90, 0, DT_REAL, _state);

    err50 = ae_false;
    err90 = ae_false;
    errless = ae_false;
    errspec = ae_false;
    ae_vector_set_length(&q50, 2, _state);
    ae_vector_set_length(&q90, 2, _state);
    for(n=1; n<=maxn; n++)
    {
        isupper = ae_fp_greater(ae_randomreal(_state),0.5);
        
        /*
         * general test
         */
        ae_matrix_set_length(&a, n, n, _state);
        for(i=0; i<=1; i++)
        {
            q50.ptr.p_double[i] = (double)(0);
            q90.ptr.p_double[i] = (double)(0);
        }
        for(pass=1; pass<=passcount; pass++)
        {
            hpdmatrixrndcond(n, ae_exp(ae_randomreal(_state)*ae_log((double)(1000), _state), _state), &a, _state);
            testrcondunit_cmatrixrefrcond(&a, n, &erc1, &ercinf, _state);
            testrcondunit_cmatrixdrophalf(&a, n, isupper, _state);
            testrcondunit_cmatrixmakeacopy(&a, n, n, &cha, _state);
            hpdmatrixcholesky(&cha, n, isupper, _state);
            
            /*
             * normal
             */
            v = 1/hpdmatrixrcond(&a, n, isupper, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*erc1) )
            {
                q50.ptr.p_double[0] = q50.ptr.p_double[0]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*erc1) )
            {
                q90.ptr.p_double[0] = q90.ptr.p_double[0]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,erc1*1.001);
            
            /*
             * Cholesky
             */
            v = 1/hpdmatrixcholeskyrcond(&cha, n, isupper, _state);
            if( ae_fp_greater_eq(v,testrcondunit_threshold50*erc1) )
            {
                q50.ptr.p_double[1] = q50.ptr.p_double[1]+(double)1/(double)passcount;
            }
            if( ae_fp_greater_eq(v,testrcondunit_threshold90*erc1) )
            {
                q90.ptr.p_double[1] = q90.ptr.p_double[1]+(double)1/(double)passcount;
            }
            errless = errless||ae_fp_greater(v,erc1*1.001);
        }
        for(i=0; i<=1; i++)
        {
            err50 = err50||ae_fp_less(q50.ptr.p_double[i],0.50);
            err90 = err90||ae_fp_less(q90.ptr.p_double[i],0.90);
        }
        
        /*
         * degenerate matrix test
         */
        if( n>=3 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
                }
            }
            a.ptr.pp_complex[0][0] = ae_complex_from_i(1);
            a.ptr.pp_complex[n-1][n-1] = ae_complex_from_i(1);
            errspec = errspec||ae_fp_neq(hpdmatrixrcond(&a, n, isupper, _state),(double)(-1));
            errspec = errspec||ae_fp_neq(hpdmatrixcholeskyrcond(&a, n, isupper, _state),(double)(0));
        }
        
        /*
         * near-degenerate matrix test
         */
        if( n>=2 )
        {
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_complex[i][i] = ae_complex_from_i(1);
            }
            i = ae_randominteger(n, _state);
            a.ptr.pp_complex[i][i] = ae_complex_from_d(0.1*ae_maxrealnumber);
            errspec = errspec||ae_fp_neq(hpdmatrixrcond(&a, n, isupper, _state),(double)(0));
            errspec = errspec||ae_fp_neq(hpdmatrixcholeskyrcond(&a, n, isupper, _state),(double)(0));
        }
    }
    
    /*
     * report
     */
    result = !(((err50||err90)||errless)||errspec);
    ae_frame_leave(_state);
    return result;
}


/*$ End $*/
