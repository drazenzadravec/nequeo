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
#include "testevdunit.h"


/*$ Declarations $*/
static void testevdunit_rmatrixfillsparsea(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double sparcity,
     ae_state *_state);
static void testevdunit_cmatrixfillsparsea(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double sparcity,
     ae_state *_state);
static void testevdunit_rmatrixsymmetricsplit(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_matrix* al,
     /* Real    */ ae_matrix* au,
     ae_state *_state);
static void testevdunit_cmatrixhermitiansplit(/* Complex */ ae_matrix* a,
     ae_int_t n,
     /* Complex */ ae_matrix* al,
     /* Complex */ ae_matrix* au,
     ae_state *_state);
static void testevdunit_unset2d(/* Real    */ ae_matrix* a,
     ae_state *_state);
static void testevdunit_cunset2d(/* Complex */ ae_matrix* a,
     ae_state *_state);
static void testevdunit_unset1d(/* Real    */ ae_vector* a,
     ae_state *_state);
static double testevdunit_tdtestproduct(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     /* Real    */ ae_matrix* z,
     /* Real    */ ae_vector* lambdav,
     ae_state *_state);
static double testevdunit_testproduct(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_matrix* z,
     /* Real    */ ae_vector* lambdav,
     ae_state *_state);
static double testevdunit_testort(/* Real    */ ae_matrix* z,
     ae_int_t n,
     ae_state *_state);
static double testevdunit_testcproduct(/* Complex */ ae_matrix* a,
     ae_int_t n,
     /* Complex */ ae_matrix* z,
     /* Real    */ ae_vector* lambdav,
     ae_state *_state);
static double testevdunit_testcort(/* Complex */ ae_matrix* z,
     ae_int_t n,
     ae_state *_state);
static void testevdunit_testsevdproblem(/* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* al,
     /* Real    */ ae_matrix* au,
     ae_int_t n,
     double threshold,
     ae_bool* serrors,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_state *_state);
static void testevdunit_testhevdproblem(/* Complex */ ae_matrix* a,
     /* Complex */ ae_matrix* al,
     /* Complex */ ae_matrix* au,
     ae_int_t n,
     double threshold,
     ae_bool* herrors,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_state *_state);
static void testevdunit_testsevdbiproblem(/* Real    */ ae_matrix* afull,
     /* Real    */ ae_matrix* al,
     /* Real    */ ae_matrix* au,
     ae_int_t n,
     ae_bool distvals,
     double threshold,
     ae_bool* serrors,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_state *_state);
static void testevdunit_testhevdbiproblem(/* Complex */ ae_matrix* afull,
     /* Complex */ ae_matrix* al,
     /* Complex */ ae_matrix* au,
     ae_int_t n,
     ae_bool distvals,
     double threshold,
     ae_bool* herrors,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_state *_state);
static void testevdunit_testtdevdproblem(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     double threshold,
     ae_bool* tderrors,
     ae_state *_state);
static void testevdunit_testtdevdbiproblem(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_bool distvals,
     double threshold,
     ae_bool* serrors,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_state *_state);
static void testevdunit_testnsevdproblem(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double threshold,
     ae_bool* nserrors,
     ae_state *_state);
static void testevdunit_testevdset(ae_int_t n,
     double threshold,
     double bithreshold,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_bool* nserrors,
     ae_bool* serrors,
     ae_bool* herrors,
     ae_bool* tderrors,
     ae_bool* sbierrors,
     ae_bool* hbierrors,
     ae_bool* tdbierrors,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Testing symmetric EVD subroutine
*************************************************************************/
ae_bool testevd(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix ra;
    ae_int_t n;
    ae_int_t j;
    ae_int_t failc;
    ae_int_t runs;
    double failthreshold;
    double threshold;
    double bithreshold;
    ae_bool waserrors;
    ae_bool nserrors;
    ae_bool serrors;
    ae_bool herrors;
    ae_bool tderrors;
    ae_bool sbierrors;
    ae_bool hbierrors;
    ae_bool tdbierrors;
    ae_bool wfailed;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ra, 0, 0, DT_REAL, _state);

    failthreshold = 0.005;
    threshold = 1.0E-8;
    bithreshold = 1.0E-6;
    nserrors = ae_false;
    serrors = ae_false;
    herrors = ae_false;
    tderrors = ae_false;
    sbierrors = ae_false;
    hbierrors = ae_false;
    tdbierrors = ae_false;
    failc = 0;
    runs = 0;
    
    /*
     * Test problems
     */
    for(n=1; n<=ablasblocksize(&ra, _state); n++)
    {
        testevdunit_testevdset(n, threshold, bithreshold, &failc, &runs, &nserrors, &serrors, &herrors, &tderrors, &sbierrors, &hbierrors, &tdbierrors, _state);
    }
    for(j=2; j<=3; j++)
    {
        for(n=j*ablasblocksize(&ra, _state)-1; n<=j*ablasblocksize(&ra, _state)+1; n++)
        {
            testevdunit_testevdset(n, threshold, bithreshold, &failc, &runs, &nserrors, &serrors, &herrors, &tderrors, &sbierrors, &hbierrors, &tdbierrors, _state);
        }
    }
    
    /*
     * report
     */
    wfailed = ae_fp_greater((double)failc/(double)runs,failthreshold);
    waserrors = ((((((nserrors||serrors)||herrors)||tderrors)||sbierrors)||hbierrors)||tdbierrors)||wfailed;
    if( !silent )
    {
        printf("TESTING EVD UNIT\n");
        printf("NS ERRORS:                               ");
        if( !nserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("S ERRORS:                                ");
        if( !serrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("H ERRORS:                                ");
        if( !herrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("TD ERRORS:                               ");
        if( !tderrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("SBI ERRORS:                              ");
        if( !sbierrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("HBI ERRORS:                              ");
        if( !hbierrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("TDBI ERRORS:                             ");
        if( !tdbierrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("FAILURE THRESHOLD:                       ");
        if( !wfailed )
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
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testevd(ae_bool silent, ae_state *_state)
{
    return testevd(silent, _state);
}


/*************************************************************************
Sparse fill
*************************************************************************/
static void testevdunit_rmatrixfillsparsea(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double sparcity,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( ae_fp_greater_eq(ae_randomreal(_state),sparcity) )
            {
                a->ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            else
            {
                a->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
}


/*************************************************************************
Sparse fill
*************************************************************************/
static void testevdunit_cmatrixfillsparsea(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double sparcity,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( ae_fp_greater_eq(ae_randomreal(_state),sparcity) )
            {
                a->ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                a->ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
            }
            else
            {
                a->ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
    }
}


/*************************************************************************
Copies A to AL (lower half) and AU (upper half), filling unused parts by
random garbage.
*************************************************************************/
static void testevdunit_rmatrixsymmetricsplit(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_matrix* al,
     /* Real    */ ae_matrix* au,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=n-1; i++)
    {
        for(j=i+1; j<=n-1; j++)
        {
            al->ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            al->ptr.pp_double[j][i] = a->ptr.pp_double[i][j];
            au->ptr.pp_double[i][j] = a->ptr.pp_double[i][j];
            au->ptr.pp_double[j][i] = 2*ae_randomreal(_state)-1;
        }
        al->ptr.pp_double[i][i] = a->ptr.pp_double[i][i];
        au->ptr.pp_double[i][i] = a->ptr.pp_double[i][i];
    }
}


/*************************************************************************
Copies A to AL (lower half) and AU (upper half), filling unused parts by
random garbage.
*************************************************************************/
static void testevdunit_cmatrixhermitiansplit(/* Complex */ ae_matrix* a,
     ae_int_t n,
     /* Complex */ ae_matrix* al,
     /* Complex */ ae_matrix* au,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=n-1; i++)
    {
        for(j=i+1; j<=n-1; j++)
        {
            al->ptr.pp_complex[i][j] = ae_complex_from_d(2*ae_randomreal(_state)-1);
            al->ptr.pp_complex[j][i] = ae_c_conj(a->ptr.pp_complex[i][j], _state);
            au->ptr.pp_complex[i][j] = a->ptr.pp_complex[i][j];
            au->ptr.pp_complex[j][i] = ae_complex_from_d(2*ae_randomreal(_state)-1);
        }
        al->ptr.pp_complex[i][i] = a->ptr.pp_complex[i][i];
        au->ptr.pp_complex[i][i] = a->ptr.pp_complex[i][i];
    }
}


/*************************************************************************
Unsets 2D array.
*************************************************************************/
static void testevdunit_unset2d(/* Real    */ ae_matrix* a,
     ae_state *_state)
{

    ae_matrix_clear(a);

    if( a->rows*a->cols>0 )
    {
        ae_matrix_set_length(a, 1, 1, _state);
    }
}


/*************************************************************************
Unsets 2D array.
*************************************************************************/
static void testevdunit_cunset2d(/* Complex */ ae_matrix* a,
     ae_state *_state)
{


    ae_matrix_set_length(a, 0+1, 0+1, _state);
    a->ptr.pp_complex[0][0] = ae_complex_from_d(2*ae_randomreal(_state)-1);
}


/*************************************************************************
Unsets 1D array.
*************************************************************************/
static void testevdunit_unset1d(/* Real    */ ae_vector* a,
     ae_state *_state)
{

    ae_vector_clear(a);

    if( a->cnt>0 )
    {
        ae_vector_set_length(a, 1, _state);
    }
}


/*************************************************************************
Tests Z*Lambda*Z' against tridiag(D,E).
Returns relative error.
*************************************************************************/
static double testevdunit_tdtestproduct(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     /* Real    */ ae_matrix* z,
     /* Real    */ ae_vector* lambdav,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    double mx;
    double result;


    result = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            
            /*
             * Calculate V = A[i,j], A = Z*Lambda*Z'
             */
            v = (double)(0);
            for(k=0; k<=n-1; k++)
            {
                v = v+z->ptr.pp_double[i][k]*lambdav->ptr.p_double[k]*z->ptr.pp_double[j][k];
            }
            
            /*
             * Compare
             */
            if( ae_iabs(i-j, _state)==0 )
            {
                result = ae_maxreal(result, ae_fabs(v-d->ptr.p_double[i], _state), _state);
            }
            if( ae_iabs(i-j, _state)==1 )
            {
                result = ae_maxreal(result, ae_fabs(v-e->ptr.p_double[ae_minint(i, j, _state)], _state), _state);
            }
            if( ae_iabs(i-j, _state)>1 )
            {
                result = ae_maxreal(result, ae_fabs(v, _state), _state);
            }
        }
    }
    mx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        mx = ae_maxreal(mx, ae_fabs(d->ptr.p_double[i], _state), _state);
    }
    for(i=0; i<=n-2; i++)
    {
        mx = ae_maxreal(mx, ae_fabs(e->ptr.p_double[i], _state), _state);
    }
    if( ae_fp_eq(mx,(double)(0)) )
    {
        mx = (double)(1);
    }
    result = result/mx;
    return result;
}


/*************************************************************************
Tests Z*Lambda*Z' against A
Returns relative error.
*************************************************************************/
static double testevdunit_testproduct(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_matrix* z,
     /* Real    */ ae_vector* lambdav,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    double mx;
    double result;


    result = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            
            /*
             * Calculate V = A[i,j], A = Z*Lambda*Z'
             */
            v = (double)(0);
            for(k=0; k<=n-1; k++)
            {
                v = v+z->ptr.pp_double[i][k]*lambdav->ptr.p_double[k]*z->ptr.pp_double[j][k];
            }
            
            /*
             * Compare
             */
            result = ae_maxreal(result, ae_fabs(v-a->ptr.pp_double[i][j], _state), _state);
        }
    }
    mx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            mx = ae_maxreal(mx, ae_fabs(a->ptr.pp_double[i][j], _state), _state);
        }
    }
    if( ae_fp_eq(mx,(double)(0)) )
    {
        mx = (double)(1);
    }
    result = result/mx;
    return result;
}


/*************************************************************************
Tests Z*Z' against diag(1...1)
Returns absolute error.
*************************************************************************/
static double testevdunit_testort(/* Real    */ ae_matrix* z,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;
    double result;


    result = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&z->ptr.pp_double[0][i], z->stride, &z->ptr.pp_double[0][j], z->stride, ae_v_len(0,n-1));
            if( i==j )
            {
                v = v-1;
            }
            result = ae_maxreal(result, ae_fabs(v, _state), _state);
        }
    }
    return result;
}


/*************************************************************************
Tests Z*Lambda*Z' against A
Returns relative error.
*************************************************************************/
static double testevdunit_testcproduct(/* Complex */ ae_matrix* a,
     ae_int_t n,
     /* Complex */ ae_matrix* z,
     /* Real    */ ae_vector* lambdav,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_complex v;
    double mx;
    double result;


    result = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            
            /*
             * Calculate V = A[i,j], A = Z*Lambda*Z'
             */
            v = ae_complex_from_i(0);
            for(k=0; k<=n-1; k++)
            {
                v = ae_c_add(v,ae_c_mul(ae_c_mul_d(z->ptr.pp_complex[i][k],lambdav->ptr.p_double[k]),ae_c_conj(z->ptr.pp_complex[j][k], _state)));
            }
            
            /*
             * Compare
             */
            result = ae_maxreal(result, ae_c_abs(ae_c_sub(v,a->ptr.pp_complex[i][j]), _state), _state);
        }
    }
    mx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            mx = ae_maxreal(mx, ae_c_abs(a->ptr.pp_complex[i][j], _state), _state);
        }
    }
    if( ae_fp_eq(mx,(double)(0)) )
    {
        mx = (double)(1);
    }
    result = result/mx;
    return result;
}


/*************************************************************************
Tests Z*Z' against diag(1...1)
Returns absolute error.
*************************************************************************/
static double testevdunit_testcort(/* Complex */ ae_matrix* z,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_complex v;
    double result;


    result = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&z->ptr.pp_complex[0][i], z->stride, "N", &z->ptr.pp_complex[0][j], z->stride, "Conj", ae_v_len(0,n-1));
            if( i==j )
            {
                v = ae_c_sub_d(v,1);
            }
            result = ae_maxreal(result, ae_c_abs(v, _state), _state);
        }
    }
    return result;
}


/*************************************************************************
Tests SEVD problem
*************************************************************************/
static void testevdunit_testsevdproblem(/* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* al,
     /* Real    */ ae_matrix* au,
     ae_int_t n,
     double threshold,
     ae_bool* serrors,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lambdav;
    ae_vector lambdaref;
    ae_matrix z;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&lambdav, 0, DT_REAL, _state);
    ae_vector_init(&lambdaref, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_REAL, _state);

    
    /*
     * Test simple EVD: values and full vectors, lower A
     */
    testevdunit_unset1d(&lambdaref, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevd(al, n, 1, ae_false, &lambdaref, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    *serrors = *serrors||ae_fp_greater(testevdunit_testproduct(a, n, &z, &lambdaref, _state),threshold);
    *serrors = *serrors||ae_fp_greater(testevdunit_testort(&z, n, _state),threshold);
    for(i=0; i<=n-2; i++)
    {
        if( ae_fp_less(lambdaref.ptr.p_double[i+1],lambdaref.ptr.p_double[i]) )
        {
            *serrors = ae_true;
            ae_frame_leave(_state);
            return;
        }
    }
    
    /*
     * Test simple EVD: values and full vectors, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevd(au, n, 1, ae_true, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    *serrors = *serrors||ae_fp_greater(testevdunit_testproduct(a, n, &z, &lambdav, _state),threshold);
    *serrors = *serrors||ae_fp_greater(testevdunit_testort(&z, n, _state),threshold);
    for(i=0; i<=n-2; i++)
    {
        if( ae_fp_less(lambdav.ptr.p_double[i+1],lambdav.ptr.p_double[i]) )
        {
            *serrors = ae_true;
            ae_frame_leave(_state);
            return;
        }
    }
    
    /*
     * Test simple EVD: values only, lower A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevd(al, n, 0, ae_false, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[i]-lambdaref.ptr.p_double[i], _state),threshold);
    }
    
    /*
     * Test simple EVD: values only, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevd(au, n, 0, ae_true, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[i]-lambdaref.ptr.p_double[i], _state),threshold);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Tests SEVD problem
*************************************************************************/
static void testevdunit_testhevdproblem(/* Complex */ ae_matrix* a,
     /* Complex */ ae_matrix* al,
     /* Complex */ ae_matrix* au,
     ae_int_t n,
     double threshold,
     ae_bool* herrors,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lambdav;
    ae_vector lambdaref;
    ae_matrix z;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&lambdav, 0, DT_REAL, _state);
    ae_vector_init(&lambdaref, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_COMPLEX, _state);

    
    /*
     * Test simple EVD: values and full vectors, lower A
     */
    testevdunit_unset1d(&lambdaref, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevd(al, n, 1, ae_false, &lambdaref, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    *herrors = *herrors||ae_fp_greater(testevdunit_testcproduct(a, n, &z, &lambdaref, _state),threshold);
    *herrors = *herrors||ae_fp_greater(testevdunit_testcort(&z, n, _state),threshold);
    for(i=0; i<=n-2; i++)
    {
        if( ae_fp_less(lambdaref.ptr.p_double[i+1],lambdaref.ptr.p_double[i]) )
        {
            *herrors = ae_true;
            ae_frame_leave(_state);
            return;
        }
    }
    
    /*
     * Test simple EVD: values and full vectors, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevd(au, n, 1, ae_true, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    *herrors = *herrors||ae_fp_greater(testevdunit_testcproduct(a, n, &z, &lambdav, _state),threshold);
    *herrors = *herrors||ae_fp_greater(testevdunit_testcort(&z, n, _state),threshold);
    for(i=0; i<=n-2; i++)
    {
        if( ae_fp_less(lambdav.ptr.p_double[i+1],lambdav.ptr.p_double[i]) )
        {
            *herrors = ae_true;
            ae_frame_leave(_state);
            return;
        }
    }
    
    /*
     * Test simple EVD: values only, lower A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevd(al, n, 0, ae_false, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        *herrors = *herrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[i]-lambdaref.ptr.p_double[i], _state),threshold);
    }
    
    /*
     * Test simple EVD: values only, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevd(au, n, 0, ae_true, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        *herrors = *herrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[i]-lambdaref.ptr.p_double[i], _state),threshold);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Tests EVD problem

DistVals    -   is True, when eigenvalues are distinct. Is False, when we
                are solving sparse task with  lots  of  zero  eigenvalues.
                In such cases some tests related to the  eigenvectors  are
                not performed.
*************************************************************************/
static void testevdunit_testsevdbiproblem(/* Real    */ ae_matrix* afull,
     /* Real    */ ae_matrix* al,
     /* Real    */ ae_matrix* au,
     ae_int_t n,
     ae_bool distvals,
     double threshold,
     ae_bool* serrors,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lambdav;
    ae_vector lambdaref;
    ae_matrix z;
    ae_matrix zref;
    ae_matrix a1;
    ae_matrix a2;
    ae_matrix ar;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t m;
    ae_int_t i1;
    ae_int_t i2;
    double v;
    double a;
    double b;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&lambdav, 0, DT_REAL, _state);
    ae_vector_init(&lambdaref, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_REAL, _state);
    ae_matrix_init(&zref, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ar, 0, 0, DT_REAL, _state);

    ae_vector_set_length(&lambdaref, n-1+1, _state);
    ae_matrix_set_length(&zref, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&a1, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&a2, n-1+1, n-1+1, _state);
    
    /*
     * Reference EVD
     */
    *runs = *runs+1;
    if( !smatrixevd(afull, n, 1, ae_true, &lambdaref, &zref, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Select random interval boundaries.
     * If there are non-distinct eigenvalues at the boundaries,
     * we move indexes further until values splits. It is done to
     * avoid situations where we can't get definite answer.
     */
    i1 = ae_randominteger(n, _state);
    i2 = i1+ae_randominteger(n-i1, _state);
    while(i1>0)
    {
        if( ae_fp_greater(ae_fabs(lambdaref.ptr.p_double[i1-1]-lambdaref.ptr.p_double[i1], _state),10*threshold) )
        {
            break;
        }
        i1 = i1-1;
    }
    while(i2<n-1)
    {
        if( ae_fp_greater(ae_fabs(lambdaref.ptr.p_double[i2+1]-lambdaref.ptr.p_double[i2], _state),10*threshold) )
        {
            break;
        }
        i2 = i2+1;
    }
    
    /*
     * Select A, B
     */
    if( i1>0 )
    {
        a = 0.5*(lambdaref.ptr.p_double[i1]+lambdaref.ptr.p_double[i1-1]);
    }
    else
    {
        a = lambdaref.ptr.p_double[0]-1;
    }
    if( i2<n-1 )
    {
        b = 0.5*(lambdaref.ptr.p_double[i2]+lambdaref.ptr.p_double[i2+1]);
    }
    else
    {
        b = lambdaref.ptr.p_double[n-1]+1;
    }
    
    /*
     * Test interval, no vectors, lower A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevdr(al, n, 0, ae_false, a, b, &m, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    if( m!=i2-i1+1 )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    
    /*
     * Test interval, no vectors, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevdr(au, n, 0, ae_true, a, b, &m, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    if( m!=i2-i1+1 )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    
    /*
     * Test indexes, no vectors, lower A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevdi(al, n, 0, ae_false, i1, i2, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    m = i2-i1+1;
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    
    /*
     * Test indexes, no vectors, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevdi(au, n, 0, ae_true, i1, i2, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    m = i2-i1+1;
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    
    /*
     * Test interval, vectors, lower A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevdr(al, n, 1, ae_false, a, b, &m, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    if( m!=i2-i1+1 )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        
        /*
         * Distinct eigenvalues, test vectors
         */
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_dotproduct(&z.ptr.pp_double[0][j], z.stride, &zref.ptr.pp_double[0][i1+j], zref.stride, ae_v_len(0,n-1));
            if( ae_fp_less(v,(double)(0)) )
            {
                ae_v_muld(&z.ptr.pp_double[0][j], z.stride, ae_v_len(0,n-1), -1);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *serrors = *serrors||ae_fp_greater(ae_fabs(z.ptr.pp_double[i][j]-zref.ptr.pp_double[i][i1+j], _state),threshold);
            }
        }
    }
    
    /*
     * Test interval, vectors, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevdr(au, n, 1, ae_true, a, b, &m, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    if( m!=i2-i1+1 )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        
        /*
         * Distinct eigenvalues, test vectors
         */
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_dotproduct(&z.ptr.pp_double[0][j], z.stride, &zref.ptr.pp_double[0][i1+j], zref.stride, ae_v_len(0,n-1));
            if( ae_fp_less(v,(double)(0)) )
            {
                ae_v_muld(&z.ptr.pp_double[0][j], z.stride, ae_v_len(0,n-1), -1);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *serrors = *serrors||ae_fp_greater(ae_fabs(z.ptr.pp_double[i][j]-zref.ptr.pp_double[i][i1+j], _state),threshold);
            }
        }
    }
    
    /*
     * Test indexes, vectors, lower A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevdi(al, n, 1, ae_false, i1, i2, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    m = i2-i1+1;
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        
        /*
         * Distinct eigenvalues, test vectors
         */
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_dotproduct(&z.ptr.pp_double[0][j], z.stride, &zref.ptr.pp_double[0][i1+j], zref.stride, ae_v_len(0,n-1));
            if( ae_fp_less(v,(double)(0)) )
            {
                ae_v_muld(&z.ptr.pp_double[0][j], z.stride, ae_v_len(0,n-1), -1);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *serrors = *serrors||ae_fp_greater(ae_fabs(z.ptr.pp_double[i][j]-zref.ptr.pp_double[i][i1+j], _state),threshold);
            }
        }
    }
    
    /*
     * Test indexes, vectors, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_unset2d(&z, _state);
    *runs = *runs+1;
    if( !smatrixevdi(au, n, 1, ae_true, i1, i2, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    m = i2-i1+1;
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        
        /*
         * Distinct eigenvalues, test vectors
         */
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_dotproduct(&z.ptr.pp_double[0][j], z.stride, &zref.ptr.pp_double[0][i1+j], zref.stride, ae_v_len(0,n-1));
            if( ae_fp_less(v,(double)(0)) )
            {
                ae_v_muld(&z.ptr.pp_double[0][j], z.stride, ae_v_len(0,n-1), -1);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *serrors = *serrors||ae_fp_greater(ae_fabs(z.ptr.pp_double[i][j]-zref.ptr.pp_double[i][i1+j], _state),threshold);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Tests EVD problem

DistVals    -   is True, when eigenvalues are distinct. Is False, when we
                are solving sparse task with  lots  of  zero  eigenvalues.
                In such cases some tests related to the  eigenvectors  are
                not performed.
*************************************************************************/
static void testevdunit_testhevdbiproblem(/* Complex */ ae_matrix* afull,
     /* Complex */ ae_matrix* al,
     /* Complex */ ae_matrix* au,
     ae_int_t n,
     ae_bool distvals,
     double threshold,
     ae_bool* herrors,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lambdav;
    ae_vector lambdaref;
    ae_matrix z;
    ae_matrix zref;
    ae_matrix a1;
    ae_matrix a2;
    ae_matrix ar;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t m;
    ae_int_t i1;
    ae_int_t i2;
    ae_complex v;
    double a;
    double b;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&lambdav, 0, DT_REAL, _state);
    ae_vector_init(&lambdaref, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&zref, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&a1, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&a2, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&ar, 0, 0, DT_COMPLEX, _state);

    ae_vector_set_length(&lambdaref, n-1+1, _state);
    ae_matrix_set_length(&zref, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&a1, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&a2, n-1+1, n-1+1, _state);
    
    /*
     * Reference EVD
     */
    *runs = *runs+1;
    if( !hmatrixevd(afull, n, 1, ae_true, &lambdaref, &zref, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Select random interval boundaries.
     * If there are non-distinct eigenvalues at the boundaries,
     * we move indexes further until values splits. It is done to
     * avoid situations where we can't get definite answer.
     */
    i1 = ae_randominteger(n, _state);
    i2 = i1+ae_randominteger(n-i1, _state);
    while(i1>0)
    {
        if( ae_fp_greater(ae_fabs(lambdaref.ptr.p_double[i1-1]-lambdaref.ptr.p_double[i1], _state),10*threshold) )
        {
            break;
        }
        i1 = i1-1;
    }
    while(i2<n-1)
    {
        if( ae_fp_greater(ae_fabs(lambdaref.ptr.p_double[i2+1]-lambdaref.ptr.p_double[i2], _state),10*threshold) )
        {
            break;
        }
        i2 = i2+1;
    }
    
    /*
     * Select A, B
     */
    if( i1>0 )
    {
        a = 0.5*(lambdaref.ptr.p_double[i1]+lambdaref.ptr.p_double[i1-1]);
    }
    else
    {
        a = lambdaref.ptr.p_double[0]-1;
    }
    if( i2<n-1 )
    {
        b = 0.5*(lambdaref.ptr.p_double[i2]+lambdaref.ptr.p_double[i2+1]);
    }
    else
    {
        b = lambdaref.ptr.p_double[n-1]+1;
    }
    
    /*
     * Test interval, no vectors, lower A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevdr(al, n, 0, ae_false, a, b, &m, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    if( m!=i2-i1+1 )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(k=0; k<=m-1; k++)
    {
        *herrors = *herrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    
    /*
     * Test interval, no vectors, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevdr(au, n, 0, ae_true, a, b, &m, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    if( m!=i2-i1+1 )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(k=0; k<=m-1; k++)
    {
        *herrors = *herrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    
    /*
     * Test indexes, no vectors, lower A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevdi(al, n, 0, ae_false, i1, i2, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    m = i2-i1+1;
    for(k=0; k<=m-1; k++)
    {
        *herrors = *herrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    
    /*
     * Test indexes, no vectors, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevdi(au, n, 0, ae_true, i1, i2, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    m = i2-i1+1;
    for(k=0; k<=m-1; k++)
    {
        *herrors = *herrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    
    /*
     * Test interval, vectors, lower A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevdr(al, n, 1, ae_false, a, b, &m, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    if( m!=i2-i1+1 )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(k=0; k<=m-1; k++)
    {
        *herrors = *herrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        
        /*
         * Distinct eigenvalues, test vectors
         */
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_cdotproduct(&z.ptr.pp_complex[0][j], z.stride, "N", &zref.ptr.pp_complex[0][i1+j], zref.stride, "Conj", ae_v_len(0,n-1));
            v = ae_c_conj(ae_c_div_d(v,ae_c_abs(v, _state)), _state);
            ae_v_cmulc(&z.ptr.pp_complex[0][j], z.stride, ae_v_len(0,n-1), v);
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *herrors = *herrors||ae_fp_greater(ae_c_abs(ae_c_sub(z.ptr.pp_complex[i][j],zref.ptr.pp_complex[i][i1+j]), _state),threshold);
            }
        }
    }
    
    /*
     * Test interval, vectors, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevdr(au, n, 1, ae_true, a, b, &m, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    if( m!=i2-i1+1 )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(k=0; k<=m-1; k++)
    {
        *herrors = *herrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        
        /*
         * Distinct eigenvalues, test vectors
         */
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_cdotproduct(&z.ptr.pp_complex[0][j], z.stride, "N", &zref.ptr.pp_complex[0][i1+j], zref.stride, "Conj", ae_v_len(0,n-1));
            v = ae_c_conj(ae_c_div_d(v,ae_c_abs(v, _state)), _state);
            ae_v_cmulc(&z.ptr.pp_complex[0][j], z.stride, ae_v_len(0,n-1), v);
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *herrors = *herrors||ae_fp_greater(ae_c_abs(ae_c_sub(z.ptr.pp_complex[i][j],zref.ptr.pp_complex[i][i1+j]), _state),threshold);
            }
        }
    }
    
    /*
     * Test indexes, vectors, lower A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevdi(al, n, 1, ae_false, i1, i2, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    m = i2-i1+1;
    for(k=0; k<=m-1; k++)
    {
        *herrors = *herrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        
        /*
         * Distinct eigenvalues, test vectors
         */
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_cdotproduct(&z.ptr.pp_complex[0][j], z.stride, "N", &zref.ptr.pp_complex[0][i1+j], zref.stride, "Conj", ae_v_len(0,n-1));
            v = ae_c_conj(ae_c_div_d(v,ae_c_abs(v, _state)), _state);
            ae_v_cmulc(&z.ptr.pp_complex[0][j], z.stride, ae_v_len(0,n-1), v);
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *herrors = *herrors||ae_fp_greater(ae_c_abs(ae_c_sub(z.ptr.pp_complex[i][j],zref.ptr.pp_complex[i][i1+j]), _state),threshold);
            }
        }
    }
    
    /*
     * Test indexes, vectors, upper A
     */
    testevdunit_unset1d(&lambdav, _state);
    testevdunit_cunset2d(&z, _state);
    *runs = *runs+1;
    if( !hmatrixevdi(au, n, 1, ae_true, i1, i2, &lambdav, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    m = i2-i1+1;
    for(k=0; k<=m-1; k++)
    {
        *herrors = *herrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        
        /*
         * Distinct eigenvalues, test vectors
         */
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_cdotproduct(&z.ptr.pp_complex[0][j], z.stride, "N", &zref.ptr.pp_complex[0][i1+j], zref.stride, "Conj", ae_v_len(0,n-1));
            v = ae_c_conj(ae_c_div_d(v,ae_c_abs(v, _state)), _state);
            ae_v_cmulc(&z.ptr.pp_complex[0][j], z.stride, ae_v_len(0,n-1), v);
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *herrors = *herrors||ae_fp_greater(ae_c_abs(ae_c_sub(z.ptr.pp_complex[i][j],zref.ptr.pp_complex[i][i1+j]), _state),threshold);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Tests EVD problem
*************************************************************************/
static void testevdunit_testtdevdproblem(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     double threshold,
     ae_bool* tderrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lambdav;
    ae_vector ee;
    ae_vector lambda2;
    ae_matrix z;
    ae_matrix zref;
    ae_matrix a1;
    ae_matrix a2;
    ae_bool wsucc;
    ae_int_t i;
    ae_int_t j;
    double v;
    double worstseparation;
    double requiredseparation;
    double specialthreshold;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&lambdav, 0, DT_REAL, _state);
    ae_vector_init(&ee, 0, DT_REAL, _state);
    ae_vector_init(&lambda2, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_REAL, _state);
    ae_matrix_init(&zref, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a2, 0, 0, DT_REAL, _state);

    ae_vector_set_length(&lambdav, n-1+1, _state);
    ae_vector_set_length(&lambda2, n-1+1, _state);
    ae_matrix_set_length(&zref, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&a1, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&a2, n-1+1, n-1+1, _state);
    if( n>1 )
    {
        ae_vector_set_length(&ee, n-2+1, _state);
    }
    
    /*
     * Test simple EVD: values and full vectors
     */
    for(i=0; i<=n-1; i++)
    {
        lambdav.ptr.p_double[i] = d->ptr.p_double[i];
    }
    for(i=0; i<=n-2; i++)
    {
        ee.ptr.p_double[i] = e->ptr.p_double[i];
    }
    testevdunit_unset2d(&z, _state);
    wsucc = smatrixtdevd(&lambdav, &ee, n, 2, &z, _state);
    if( !wsucc )
    {
        seterrorflag(tderrors, ae_true, _state);
        ae_frame_leave(_state);
        return;
    }
    seterrorflag(tderrors, ae_fp_greater(testevdunit_tdtestproduct(d, e, n, &z, &lambdav, _state),threshold), _state);
    seterrorflag(tderrors, ae_fp_greater(testevdunit_testort(&z, n, _state),threshold), _state);
    for(i=0; i<=n-2; i++)
    {
        if( ae_fp_less(lambdav.ptr.p_double[i+1],lambdav.ptr.p_double[i]) )
        {
            seterrorflag(tderrors, ae_true, _state);
            ae_frame_leave(_state);
            return;
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            zref.ptr.pp_double[i][j] = z.ptr.pp_double[i][j];
        }
    }
    
    /*
     * Test values only variant
     */
    for(i=0; i<=n-1; i++)
    {
        lambda2.ptr.p_double[i] = d->ptr.p_double[i];
    }
    for(i=0; i<=n-2; i++)
    {
        ee.ptr.p_double[i] = e->ptr.p_double[i];
    }
    testevdunit_unset2d(&z, _state);
    wsucc = smatrixtdevd(&lambda2, &ee, n, 0, &z, _state);
    if( !wsucc )
    {
        seterrorflag(tderrors, ae_true, _state);
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        seterrorflag(tderrors, ae_fp_greater(ae_fabs(lambda2.ptr.p_double[i]-lambdav.ptr.p_double[i], _state),threshold), _state);
    }
    
    /*
     * Test multiplication variant
     */
    for(i=0; i<=n-1; i++)
    {
        lambda2.ptr.p_double[i] = d->ptr.p_double[i];
    }
    for(i=0; i<=n-2; i++)
    {
        ee.ptr.p_double[i] = e->ptr.p_double[i];
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a1.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            a2.ptr.pp_double[i][j] = a1.ptr.pp_double[i][j];
        }
    }
    wsucc = smatrixtdevd(&lambda2, &ee, n, 1, &a1, _state);
    if( !wsucc )
    {
        seterrorflag(tderrors, ae_true, _state);
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        seterrorflag(tderrors, ae_fp_greater(ae_fabs(lambda2.ptr.p_double[i]-lambdav.ptr.p_double[i], _state),threshold), _state);
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&a2.ptr.pp_double[i][0], 1, &zref.ptr.pp_double[0][j], zref.stride, ae_v_len(0,n-1));
            
            /*
             * next line is a bit complicated because
             * depending on algorithm used we can get either
             * z or -z as eigenvector. so we compare result
             * with both A*ZRef and -A*ZRef
             */
            seterrorflag(tderrors, ae_fp_greater(ae_fabs(v-a1.ptr.pp_double[i][j], _state),threshold)&&ae_fp_greater(ae_fabs(v+a1.ptr.pp_double[i][j], _state),threshold), _state);
        }
    }
    
    /*
     * Test first row variant.
     *
     * NOTE: this test is special because ZNeeded=3 is ALGLIB-specific feature
     *       which is NOT supported by Intel MKL. Thus, MKL-capable version of
     *       ALGLIB will use different algorithms for ZNeeded=3 and for ZNeeded<3.
     *
     *       In most cases it is OK, but when problem happened to be degenerate
     *       (two close eigenvalues), Z computed by ALGLIB may be different from
     *       Z computed by MKL (up to arbitrary rotation), which will lead to
     *       failure of the test, because ZNeeded=2 is used as reference value
     *       for ZNeeded=3.
     *
     *       That's why this test is performed only for well-separated matrices,
     *       and with custom threshold.
     */
    requiredseparation = 1.0E-6;
    specialthreshold = 1.0E-6;
    worstseparation = ae_maxrealnumber;
    for(i=0; i<=n-2; i++)
    {
        worstseparation = ae_minreal(worstseparation, ae_fabs(lambdav.ptr.p_double[i+1]-lambdav.ptr.p_double[i], _state), _state);
    }
    if( ae_fp_greater(worstseparation,requiredseparation) )
    {
        for(i=0; i<=n-1; i++)
        {
            lambda2.ptr.p_double[i] = d->ptr.p_double[i];
        }
        for(i=0; i<=n-2; i++)
        {
            ee.ptr.p_double[i] = e->ptr.p_double[i];
        }
        testevdunit_unset2d(&z, _state);
        wsucc = smatrixtdevd(&lambda2, &ee, n, 3, &z, _state);
        if( !wsucc )
        {
            seterrorflag(tderrors, ae_true, _state);
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(tderrors, ae_fp_greater(ae_fabs(lambda2.ptr.p_double[i]-lambdav.ptr.p_double[i], _state),threshold), _state);
            
            /*
             * next line is a bit complicated because
             * depending on algorithm used we can get either
             * z or -z as eigenvector. so we compare result
             * with both z and -z
             */
            seterrorflag(tderrors, ae_fp_greater(ae_fabs(z.ptr.pp_double[0][i]-zref.ptr.pp_double[0][i], _state),specialthreshold)&&ae_fp_greater(ae_fabs(z.ptr.pp_double[0][i]+zref.ptr.pp_double[0][i], _state),specialthreshold), _state);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Tests EVD problem

DistVals    -   is True, when eigenvalues are distinct. Is False, when we
                are solving sparse task with  lots  of  zero  eigenvalues.
                In such cases some tests related to the  eigenvectors  are
                not performed.
*************************************************************************/
static void testevdunit_testtdevdbiproblem(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_bool distvals,
     double threshold,
     ae_bool* serrors,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lambdav;
    ae_vector lambdaref;
    ae_matrix z;
    ae_matrix zref;
    ae_matrix a1;
    ae_matrix a2;
    ae_matrix ar;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t m;
    ae_int_t i1;
    ae_int_t i2;
    double v;
    double a;
    double b;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&lambdav, 0, DT_REAL, _state);
    ae_vector_init(&lambdaref, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_REAL, _state);
    ae_matrix_init(&zref, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ar, 0, 0, DT_REAL, _state);

    ae_vector_set_length(&lambdaref, n-1+1, _state);
    ae_matrix_set_length(&zref, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&a1, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&a2, n-1+1, n-1+1, _state);
    
    /*
     * Reference EVD
     */
    ae_vector_set_length(&lambdaref, n, _state);
    ae_v_move(&lambdaref.ptr.p_double[0], 1, &d->ptr.p_double[0], 1, ae_v_len(0,n-1));
    *runs = *runs+1;
    if( !smatrixtdevd(&lambdaref, e, n, 2, &zref, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Select random interval boundaries.
     * If there are non-distinct eigenvalues at the boundaries,
     * we move indexes further until values splits. It is done to
     * avoid situations where we can't get definite answer.
     */
    i1 = ae_randominteger(n, _state);
    i2 = i1+ae_randominteger(n-i1, _state);
    while(i1>0)
    {
        if( ae_fp_greater(ae_fabs(lambdaref.ptr.p_double[i1-1]-lambdaref.ptr.p_double[i1], _state),10*threshold) )
        {
            break;
        }
        i1 = i1-1;
    }
    while(i2<n-1)
    {
        if( ae_fp_greater(ae_fabs(lambdaref.ptr.p_double[i2+1]-lambdaref.ptr.p_double[i2], _state),10*threshold) )
        {
            break;
        }
        i2 = i2+1;
    }
    
    /*
     * Test different combinations
     */
    
    /*
     * Select A, B
     */
    if( i1>0 )
    {
        a = 0.5*(lambdaref.ptr.p_double[i1]+lambdaref.ptr.p_double[i1-1]);
    }
    else
    {
        a = lambdaref.ptr.p_double[0]-1;
    }
    if( i2<n-1 )
    {
        b = 0.5*(lambdaref.ptr.p_double[i2]+lambdaref.ptr.p_double[i2+1]);
    }
    else
    {
        b = lambdaref.ptr.p_double[n-1]+1;
    }
    
    /*
     * Test interval, no vectors
     */
    ae_vector_set_length(&lambdav, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        lambdav.ptr.p_double[i] = d->ptr.p_double[i];
    }
    *runs = *runs+1;
    if( !smatrixtdevdr(&lambdav, e, n, 0, a, b, &m, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    if( m!=i2-i1+1 )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    
    /*
     * Test indexes, no vectors
     */
    ae_vector_set_length(&lambdav, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        lambdav.ptr.p_double[i] = d->ptr.p_double[i];
    }
    *runs = *runs+1;
    if( !smatrixtdevdi(&lambdav, e, n, 0, i1, i2, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    m = i2-i1+1;
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    
    /*
     * Test interval, transform vectors
     */
    ae_vector_set_length(&lambdav, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        lambdav.ptr.p_double[i] = d->ptr.p_double[i];
    }
    ae_matrix_set_length(&a1, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&a2, n-1+1, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a1.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            a2.ptr.pp_double[i][j] = a1.ptr.pp_double[i][j];
        }
    }
    *runs = *runs+1;
    if( !smatrixtdevdr(&lambdav, e, n, 1, a, b, &m, &a1, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    if( m!=i2-i1+1 )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        ae_matrix_set_length(&ar, n-1+1, m-1+1, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                v = ae_v_dotproduct(&a2.ptr.pp_double[i][0], 1, &zref.ptr.pp_double[0][i1+j], zref.stride, ae_v_len(0,n-1));
                ar.ptr.pp_double[i][j] = v;
            }
        }
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_dotproduct(&a1.ptr.pp_double[0][j], a1.stride, &ar.ptr.pp_double[0][j], ar.stride, ae_v_len(0,n-1));
            if( ae_fp_less(v,(double)(0)) )
            {
                ae_v_muld(&ar.ptr.pp_double[0][j], ar.stride, ae_v_len(0,n-1), -1);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *serrors = *serrors||ae_fp_greater(ae_fabs(a1.ptr.pp_double[i][j]-ar.ptr.pp_double[i][j], _state),threshold);
            }
        }
    }
    
    /*
     * Test indexes, transform vectors
     */
    ae_vector_set_length(&lambdav, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        lambdav.ptr.p_double[i] = d->ptr.p_double[i];
    }
    ae_matrix_set_length(&a1, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&a2, n-1+1, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a1.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            a2.ptr.pp_double[i][j] = a1.ptr.pp_double[i][j];
        }
    }
    *runs = *runs+1;
    if( !smatrixtdevdi(&lambdav, e, n, 1, i1, i2, &a1, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    m = i2-i1+1;
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        ae_matrix_set_length(&ar, n-1+1, m-1+1, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                v = ae_v_dotproduct(&a2.ptr.pp_double[i][0], 1, &zref.ptr.pp_double[0][i1+j], zref.stride, ae_v_len(0,n-1));
                ar.ptr.pp_double[i][j] = v;
            }
        }
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_dotproduct(&a1.ptr.pp_double[0][j], a1.stride, &ar.ptr.pp_double[0][j], ar.stride, ae_v_len(0,n-1));
            if( ae_fp_less(v,(double)(0)) )
            {
                ae_v_muld(&ar.ptr.pp_double[0][j], ar.stride, ae_v_len(0,n-1), -1);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *serrors = *serrors||ae_fp_greater(ae_fabs(a1.ptr.pp_double[i][j]-ar.ptr.pp_double[i][j], _state),threshold);
            }
        }
    }
    
    /*
     * Test interval, do not transform vectors
     */
    ae_vector_set_length(&lambdav, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        lambdav.ptr.p_double[i] = d->ptr.p_double[i];
    }
    ae_matrix_set_length(&z, 0+1, 0+1, _state);
    *runs = *runs+1;
    if( !smatrixtdevdr(&lambdav, e, n, 2, a, b, &m, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    if( m!=i2-i1+1 )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_dotproduct(&z.ptr.pp_double[0][j], z.stride, &zref.ptr.pp_double[0][i1+j], zref.stride, ae_v_len(0,n-1));
            if( ae_fp_less(v,(double)(0)) )
            {
                ae_v_muld(&z.ptr.pp_double[0][j], z.stride, ae_v_len(0,n-1), -1);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *serrors = *serrors||ae_fp_greater(ae_fabs(z.ptr.pp_double[i][j]-zref.ptr.pp_double[i][i1+j], _state),threshold);
            }
        }
    }
    
    /*
     * Test indexes, do not transform vectors
     */
    ae_vector_set_length(&lambdav, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        lambdav.ptr.p_double[i] = d->ptr.p_double[i];
    }
    ae_matrix_set_length(&z, 0+1, 0+1, _state);
    *runs = *runs+1;
    if( !smatrixtdevdi(&lambdav, e, n, 2, i1, i2, &z, _state) )
    {
        *failc = *failc+1;
        ae_frame_leave(_state);
        return;
    }
    m = i2-i1+1;
    for(k=0; k<=m-1; k++)
    {
        *serrors = *serrors||ae_fp_greater(ae_fabs(lambdav.ptr.p_double[k]-lambdaref.ptr.p_double[i1+k], _state),threshold);
    }
    if( distvals )
    {
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_dotproduct(&z.ptr.pp_double[0][j], z.stride, &zref.ptr.pp_double[0][i1+j], zref.stride, ae_v_len(0,n-1));
            if( ae_fp_less(v,(double)(0)) )
            {
                ae_v_muld(&z.ptr.pp_double[0][j], z.stride, ae_v_len(0,n-1), -1);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                *serrors = *serrors||ae_fp_greater(ae_fabs(z.ptr.pp_double[i][j]-zref.ptr.pp_double[i][i1+j], _state),threshold);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Non-symmetric problem
*************************************************************************/
static void testevdunit_testnsevdproblem(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double threshold,
     ae_bool* nserrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    double mx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t vjob;
    ae_bool needl;
    ae_bool needr;
    ae_vector wr0;
    ae_vector wi0;
    ae_vector wr1;
    ae_vector wi1;
    ae_vector wr0s;
    ae_vector wi0s;
    ae_vector wr1s;
    ae_vector wi1s;
    ae_matrix vl;
    ae_matrix vr;
    ae_vector vec1r;
    ae_vector vec1i;
    ae_vector vec2r;
    ae_vector vec2i;
    ae_vector vec3r;
    ae_vector vec3i;
    double curwr;
    double curwi;
    double vt;
    double tmp;
    double vnorm;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&wr0, 0, DT_REAL, _state);
    ae_vector_init(&wi0, 0, DT_REAL, _state);
    ae_vector_init(&wr1, 0, DT_REAL, _state);
    ae_vector_init(&wi1, 0, DT_REAL, _state);
    ae_vector_init(&wr0s, 0, DT_REAL, _state);
    ae_vector_init(&wi0s, 0, DT_REAL, _state);
    ae_vector_init(&wr1s, 0, DT_REAL, _state);
    ae_vector_init(&wi1s, 0, DT_REAL, _state);
    ae_matrix_init(&vl, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vr, 0, 0, DT_REAL, _state);
    ae_vector_init(&vec1r, 0, DT_REAL, _state);
    ae_vector_init(&vec1i, 0, DT_REAL, _state);
    ae_vector_init(&vec2r, 0, DT_REAL, _state);
    ae_vector_init(&vec2i, 0, DT_REAL, _state);
    ae_vector_init(&vec3r, 0, DT_REAL, _state);
    ae_vector_init(&vec3i, 0, DT_REAL, _state);

    ae_vector_set_length(&vec1r, n-1+1, _state);
    ae_vector_set_length(&vec2r, n-1+1, _state);
    ae_vector_set_length(&vec3r, n-1+1, _state);
    ae_vector_set_length(&vec1i, n-1+1, _state);
    ae_vector_set_length(&vec2i, n-1+1, _state);
    ae_vector_set_length(&vec3i, n-1+1, _state);
    ae_vector_set_length(&wr0s, n-1+1, _state);
    ae_vector_set_length(&wr1s, n-1+1, _state);
    ae_vector_set_length(&wi0s, n-1+1, _state);
    ae_vector_set_length(&wi1s, n-1+1, _state);
    mx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( ae_fp_greater(ae_fabs(a->ptr.pp_double[i][j], _state),mx) )
            {
                mx = ae_fabs(a->ptr.pp_double[i][j], _state);
            }
        }
    }
    if( ae_fp_eq(mx,(double)(0)) )
    {
        mx = (double)(1);
    }
    
    /*
     * Load values-only
     */
    if( !rmatrixevd(a, n, 0, &wr0, &wi0, &vl, &vr, _state) )
    {
        seterrorflag(nserrors, ae_true, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Test different jobs
     */
    for(vjob=1; vjob<=3; vjob++)
    {
        needr = vjob==1||vjob==3;
        needl = vjob==2||vjob==3;
        if( !rmatrixevd(a, n, vjob, &wr1, &wi1, &vl, &vr, _state) )
        {
            seterrorflag(nserrors, ae_true, _state);
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Test values:
         * 1. sort by real part
         * 2. test
         */
        ae_v_move(&wr0s.ptr.p_double[0], 1, &wr0.ptr.p_double[0], 1, ae_v_len(0,n-1));
        ae_v_move(&wi0s.ptr.p_double[0], 1, &wi0.ptr.p_double[0], 1, ae_v_len(0,n-1));
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-2-i; j++)
            {
                if( ae_fp_greater(wr0s.ptr.p_double[j],wr0s.ptr.p_double[j+1]) )
                {
                    tmp = wr0s.ptr.p_double[j];
                    wr0s.ptr.p_double[j] = wr0s.ptr.p_double[j+1];
                    wr0s.ptr.p_double[j+1] = tmp;
                    tmp = wi0s.ptr.p_double[j];
                    wi0s.ptr.p_double[j] = wi0s.ptr.p_double[j+1];
                    wi0s.ptr.p_double[j+1] = tmp;
                }
            }
        }
        ae_v_move(&wr1s.ptr.p_double[0], 1, &wr1.ptr.p_double[0], 1, ae_v_len(0,n-1));
        ae_v_move(&wi1s.ptr.p_double[0], 1, &wi1.ptr.p_double[0], 1, ae_v_len(0,n-1));
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-2-i; j++)
            {
                if( ae_fp_greater(wr1s.ptr.p_double[j],wr1s.ptr.p_double[j+1]) )
                {
                    tmp = wr1s.ptr.p_double[j];
                    wr1s.ptr.p_double[j] = wr1s.ptr.p_double[j+1];
                    wr1s.ptr.p_double[j+1] = tmp;
                    tmp = wi1s.ptr.p_double[j];
                    wi1s.ptr.p_double[j] = wi1s.ptr.p_double[j+1];
                    wi1s.ptr.p_double[j+1] = tmp;
                }
            }
        }
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(nserrors, ae_fp_greater(ae_fabs(wr0s.ptr.p_double[i]-wr1s.ptr.p_double[i], _state),threshold), _state);
            seterrorflag(nserrors, ae_fp_greater(ae_fabs(wi0s.ptr.p_double[i]-wi1s.ptr.p_double[i], _state),threshold), _state);
        }
        
        /*
         * Test right vectors
         */
        if( needr )
        {
            k = 0;
            while(k<=n-1)
            {
                curwr = (double)(0);
                curwi = (double)(0);
                if( ae_fp_eq(wi1.ptr.p_double[k],(double)(0)) )
                {
                    ae_v_move(&vec1r.ptr.p_double[0], 1, &vr.ptr.pp_double[0][k], vr.stride, ae_v_len(0,n-1));
                    for(i=0; i<=n-1; i++)
                    {
                        vec1i.ptr.p_double[i] = (double)(0);
                    }
                    curwr = wr1.ptr.p_double[k];
                    curwi = (double)(0);
                }
                if( ae_fp_greater(wi1.ptr.p_double[k],(double)(0)) )
                {
                    ae_v_move(&vec1r.ptr.p_double[0], 1, &vr.ptr.pp_double[0][k], vr.stride, ae_v_len(0,n-1));
                    ae_v_move(&vec1i.ptr.p_double[0], 1, &vr.ptr.pp_double[0][k+1], vr.stride, ae_v_len(0,n-1));
                    curwr = wr1.ptr.p_double[k];
                    curwi = wi1.ptr.p_double[k];
                }
                if( ae_fp_less(wi1.ptr.p_double[k],(double)(0)) )
                {
                    ae_v_move(&vec1r.ptr.p_double[0], 1, &vr.ptr.pp_double[0][k-1], vr.stride, ae_v_len(0,n-1));
                    ae_v_moveneg(&vec1i.ptr.p_double[0], 1, &vr.ptr.pp_double[0][k], vr.stride, ae_v_len(0,n-1));
                    curwr = wr1.ptr.p_double[k];
                    curwi = wi1.ptr.p_double[k];
                }
                vnorm = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    vt = ae_v_dotproduct(&a->ptr.pp_double[i][0], 1, &vec1r.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    vec2r.ptr.p_double[i] = vt;
                    vt = ae_v_dotproduct(&a->ptr.pp_double[i][0], 1, &vec1i.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    vec2i.ptr.p_double[i] = vt;
                    vnorm = vnorm+ae_sqr(vec1r.ptr.p_double[i], _state)+ae_sqr(vec1i.ptr.p_double[i], _state);
                }
                vnorm = ae_sqrt(vnorm, _state);
                ae_v_moved(&vec3r.ptr.p_double[0], 1, &vec1r.ptr.p_double[0], 1, ae_v_len(0,n-1), curwr);
                ae_v_subd(&vec3r.ptr.p_double[0], 1, &vec1i.ptr.p_double[0], 1, ae_v_len(0,n-1), curwi);
                ae_v_moved(&vec3i.ptr.p_double[0], 1, &vec1r.ptr.p_double[0], 1, ae_v_len(0,n-1), curwi);
                ae_v_addd(&vec3i.ptr.p_double[0], 1, &vec1i.ptr.p_double[0], 1, ae_v_len(0,n-1), curwr);
                seterrorflag(nserrors, ae_fp_less(vnorm,1.0E-3)||!ae_isfinite(vnorm, _state), _state);
                for(i=0; i<=n-1; i++)
                {
                    seterrorflag(nserrors, ae_fp_greater(ae_fabs(vec2r.ptr.p_double[i]-vec3r.ptr.p_double[i], _state),threshold), _state);
                    seterrorflag(nserrors, ae_fp_greater(ae_fabs(vec2i.ptr.p_double[i]-vec3i.ptr.p_double[i], _state),threshold), _state);
                }
                k = k+1;
            }
        }
        
        /*
         * Test left vectors
         */
        curwr = (double)(0);
        curwi = (double)(0);
        if( needl )
        {
            k = 0;
            while(k<=n-1)
            {
                if( ae_fp_eq(wi1.ptr.p_double[k],(double)(0)) )
                {
                    ae_v_move(&vec1r.ptr.p_double[0], 1, &vl.ptr.pp_double[0][k], vl.stride, ae_v_len(0,n-1));
                    for(i=0; i<=n-1; i++)
                    {
                        vec1i.ptr.p_double[i] = (double)(0);
                    }
                    curwr = wr1.ptr.p_double[k];
                    curwi = (double)(0);
                }
                if( ae_fp_greater(wi1.ptr.p_double[k],(double)(0)) )
                {
                    ae_v_move(&vec1r.ptr.p_double[0], 1, &vl.ptr.pp_double[0][k], vl.stride, ae_v_len(0,n-1));
                    ae_v_move(&vec1i.ptr.p_double[0], 1, &vl.ptr.pp_double[0][k+1], vl.stride, ae_v_len(0,n-1));
                    curwr = wr1.ptr.p_double[k];
                    curwi = wi1.ptr.p_double[k];
                }
                if( ae_fp_less(wi1.ptr.p_double[k],(double)(0)) )
                {
                    ae_v_move(&vec1r.ptr.p_double[0], 1, &vl.ptr.pp_double[0][k-1], vl.stride, ae_v_len(0,n-1));
                    ae_v_moveneg(&vec1i.ptr.p_double[0], 1, &vl.ptr.pp_double[0][k], vl.stride, ae_v_len(0,n-1));
                    curwr = wr1.ptr.p_double[k];
                    curwi = wi1.ptr.p_double[k];
                }
                vnorm = 0.0;
                for(j=0; j<=n-1; j++)
                {
                    vt = ae_v_dotproduct(&vec1r.ptr.p_double[0], 1, &a->ptr.pp_double[0][j], a->stride, ae_v_len(0,n-1));
                    vec2r.ptr.p_double[j] = vt;
                    vt = ae_v_dotproduct(&vec1i.ptr.p_double[0], 1, &a->ptr.pp_double[0][j], a->stride, ae_v_len(0,n-1));
                    vec2i.ptr.p_double[j] = -vt;
                    vnorm = vnorm+ae_sqr(vec1r.ptr.p_double[j], _state)+ae_sqr(vec1i.ptr.p_double[j], _state);
                }
                vnorm = ae_sqrt(vnorm, _state);
                ae_v_moved(&vec3r.ptr.p_double[0], 1, &vec1r.ptr.p_double[0], 1, ae_v_len(0,n-1), curwr);
                ae_v_addd(&vec3r.ptr.p_double[0], 1, &vec1i.ptr.p_double[0], 1, ae_v_len(0,n-1), curwi);
                ae_v_moved(&vec3i.ptr.p_double[0], 1, &vec1r.ptr.p_double[0], 1, ae_v_len(0,n-1), curwi);
                ae_v_addd(&vec3i.ptr.p_double[0], 1, &vec1i.ptr.p_double[0], 1, ae_v_len(0,n-1), -curwr);
                seterrorflag(nserrors, ae_fp_less(vnorm,1.0E-3)||!ae_isfinite(vnorm, _state), _state);
                for(i=0; i<=n-1; i++)
                {
                    seterrorflag(nserrors, ae_fp_greater(ae_fabs(vec2r.ptr.p_double[i]-vec3r.ptr.p_double[i], _state),threshold), _state);
                    seterrorflag(nserrors, ae_fp_greater(ae_fabs(vec2i.ptr.p_double[i]-vec3i.ptr.p_double[i], _state),threshold), _state);
                }
                k = k+1;
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Testing EVD subroutines for one N

NOTES:
* BIThreshold is a threshold for bisection-and-inverse-iteration subroutines.
  special threshold is needed because these subroutines may have much more
  larger error than QR-based algorithms.
*************************************************************************/
static void testevdunit_testevdset(ae_int_t n,
     double threshold,
     double bithreshold,
     ae_int_t* failc,
     ae_int_t* runs,
     ae_bool* nserrors,
     ae_bool* serrors,
     ae_bool* herrors,
     ae_bool* tderrors,
     ae_bool* sbierrors,
     ae_bool* hbierrors,
     ae_bool* tdbierrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix ra;
    ae_matrix ral;
    ae_matrix rau;
    ae_matrix ca;
    ae_matrix cal;
    ae_matrix cau;
    ae_vector d;
    ae_vector e;
    ae_int_t i;
    ae_int_t j;
    ae_int_t mkind;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ral, 0, 0, DT_REAL, _state);
    ae_matrix_init(&rau, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cal, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cau, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);

    
    /*
     * Test symmetric problems
     */
    ae_matrix_set_length(&ra, n, n, _state);
    ae_matrix_set_length(&ral, n, n, _state);
    ae_matrix_set_length(&rau, n, n, _state);
    ae_matrix_set_length(&ca, n, n, _state);
    ae_matrix_set_length(&cal, n, n, _state);
    ae_matrix_set_length(&cau, n, n, _state);
    
    /*
     * Zero matrices
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            ra.ptr.pp_double[i][j] = (double)(0);
            ca.ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
    testevdunit_rmatrixsymmetricsplit(&ra, n, &ral, &rau, _state);
    testevdunit_cmatrixhermitiansplit(&ca, n, &cal, &cau, _state);
    testevdunit_testsevdproblem(&ra, &ral, &rau, n, threshold, serrors, failc, runs, _state);
    testevdunit_testhevdproblem(&ca, &cal, &cau, n, threshold, herrors, failc, runs, _state);
    testevdunit_testsevdbiproblem(&ra, &ral, &rau, n, ae_false, bithreshold, sbierrors, failc, runs, _state);
    testevdunit_testhevdbiproblem(&ca, &cal, &cau, n, ae_false, bithreshold, hbierrors, failc, runs, _state);
    
    /*
     * Random matrix
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=i+1; j<=n-1; j++)
        {
            ra.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            ca.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
            ca.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
            ra.ptr.pp_double[j][i] = ra.ptr.pp_double[i][j];
            ca.ptr.pp_complex[j][i] = ae_c_conj(ca.ptr.pp_complex[i][j], _state);
        }
        ra.ptr.pp_double[i][i] = 2*ae_randomreal(_state)-1;
        ca.ptr.pp_complex[i][i] = ae_complex_from_d(2*ae_randomreal(_state)-1);
    }
    testevdunit_rmatrixsymmetricsplit(&ra, n, &ral, &rau, _state);
    testevdunit_cmatrixhermitiansplit(&ca, n, &cal, &cau, _state);
    testevdunit_testsevdproblem(&ra, &ral, &rau, n, threshold, serrors, failc, runs, _state);
    testevdunit_testhevdproblem(&ca, &cal, &cau, n, threshold, herrors, failc, runs, _state);
    
    /*
     * Random diagonally dominant matrix with distinct eigenvalues
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=i+1; j<=n-1; j++)
        {
            ra.ptr.pp_double[i][j] = 0.1*(2*ae_randomreal(_state)-1)/n;
            ca.ptr.pp_complex[i][j].x = 0.1*(2*ae_randomreal(_state)-1)/n;
            ca.ptr.pp_complex[i][j].y = 0.1*(2*ae_randomreal(_state)-1)/n;
            ra.ptr.pp_double[j][i] = ra.ptr.pp_double[i][j];
            ca.ptr.pp_complex[j][i] = ae_c_conj(ca.ptr.pp_complex[i][j], _state);
        }
        ra.ptr.pp_double[i][i] = 0.1*(2*ae_randomreal(_state)-1)+i;
        ca.ptr.pp_complex[i][i] = ae_complex_from_d(0.1*(2*ae_randomreal(_state)-1)+i);
    }
    testevdunit_rmatrixsymmetricsplit(&ra, n, &ral, &rau, _state);
    testevdunit_cmatrixhermitiansplit(&ca, n, &cal, &cau, _state);
    testevdunit_testsevdproblem(&ra, &ral, &rau, n, threshold, serrors, failc, runs, _state);
    testevdunit_testhevdproblem(&ca, &cal, &cau, n, threshold, herrors, failc, runs, _state);
    testevdunit_testsevdbiproblem(&ra, &ral, &rau, n, ae_true, bithreshold, sbierrors, failc, runs, _state);
    testevdunit_testhevdbiproblem(&ca, &cal, &cau, n, ae_true, bithreshold, hbierrors, failc, runs, _state);
    
    /*
     * Sparse matrices
     */
    testevdunit_rmatrixfillsparsea(&ra, n, n, 0.995, _state);
    testevdunit_cmatrixfillsparsea(&ca, n, n, 0.995, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=i+1; j<=n-1; j++)
        {
            ra.ptr.pp_double[j][i] = ra.ptr.pp_double[i][j];
            ca.ptr.pp_complex[j][i] = ae_c_conj(ca.ptr.pp_complex[i][j], _state);
        }
        ca.ptr.pp_complex[i][i].y = (double)(0);
    }
    testevdunit_rmatrixsymmetricsplit(&ra, n, &ral, &rau, _state);
    testevdunit_cmatrixhermitiansplit(&ca, n, &cal, &cau, _state);
    testevdunit_testsevdproblem(&ra, &ral, &rau, n, threshold, serrors, failc, runs, _state);
    testevdunit_testhevdproblem(&ca, &cal, &cau, n, threshold, herrors, failc, runs, _state);
    testevdunit_testsevdbiproblem(&ra, &ral, &rau, n, ae_false, bithreshold, sbierrors, failc, runs, _state);
    testevdunit_testhevdbiproblem(&ca, &cal, &cau, n, ae_false, bithreshold, hbierrors, failc, runs, _state);
    
    /*
     * testing tridiagonal problems
     */
    for(mkind=0; mkind<=7; mkind++)
    {
        ae_vector_set_length(&d, n, _state);
        if( n>1 )
        {
            ae_vector_set_length(&e, n-1, _state);
        }
        if( mkind==0 )
        {
            
            /*
             * Zero matrix
             */
            for(i=0; i<=n-1; i++)
            {
                d.ptr.p_double[i] = (double)(0);
            }
            for(i=0; i<=n-2; i++)
            {
                e.ptr.p_double[i] = (double)(0);
            }
        }
        if( mkind==1 )
        {
            
            /*
             * Diagonal matrix
             */
            for(i=0; i<=n-1; i++)
            {
                d.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=n-2; i++)
            {
                e.ptr.p_double[i] = (double)(0);
            }
        }
        if( mkind==2 )
        {
            
            /*
             * Off-diagonal matrix
             */
            for(i=0; i<=n-1; i++)
            {
                d.ptr.p_double[i] = (double)(0);
            }
            for(i=0; i<=n-2; i++)
            {
                e.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
        }
        if( mkind==3 )
        {
            
            /*
             * Dense matrix with blocks
             */
            for(i=0; i<=n-1; i++)
            {
                d.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=n-2; i++)
            {
                e.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            j = 1;
            i = 2;
            while(j<=n-2)
            {
                e.ptr.p_double[j] = (double)(0);
                j = j+i;
                i = i+1;
            }
        }
        if( mkind==4 )
        {
            
            /*
             * dense matrix
             */
            for(i=0; i<=n-1; i++)
            {
                d.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=n-2; i++)
            {
                e.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
        }
        if( mkind==5 )
        {
            
            /*
             * Diagonal matrix with distinct eigenvalues
             */
            for(i=0; i<=n-1; i++)
            {
                d.ptr.p_double[i] = 0.1*(2*ae_randomreal(_state)-1)+i;
            }
            for(i=0; i<=n-2; i++)
            {
                e.ptr.p_double[i] = (double)(0);
            }
        }
        if( mkind==6 )
        {
            
            /*
             * Off-diagonal matrix with distinct eigenvalues
             */
            for(i=0; i<=n-1; i++)
            {
                d.ptr.p_double[i] = (double)(0);
            }
            for(i=0; i<=n-2; i++)
            {
                e.ptr.p_double[i] = 0.1*(2*ae_randomreal(_state)-1)+i+1;
            }
        }
        if( mkind==7 )
        {
            
            /*
             * dense matrix with distinct eigenvalues
             */
            for(i=0; i<=n-1; i++)
            {
                d.ptr.p_double[i] = 0.1*(2*ae_randomreal(_state)-1)+i+1;
            }
            for(i=0; i<=n-2; i++)
            {
                e.ptr.p_double[i] = 0.1*(2*ae_randomreal(_state)-1);
            }
        }
        testevdunit_testtdevdproblem(&d, &e, n, threshold, tderrors, _state);
        testevdunit_testtdevdbiproblem(&d, &e, n, (mkind==5||mkind==6)||mkind==7, bithreshold, tdbierrors, failc, runs, _state);
    }
    
    /*
     * Test non-symmetric problems
     */
    
    /*
     * Test non-symmetric problems: zero, random, sparse matrices.
     */
    ae_matrix_set_length(&ra, n, n, _state);
    ae_matrix_set_length(&ca, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            ra.ptr.pp_double[i][j] = (double)(0);
            ca.ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
    testevdunit_testnsevdproblem(&ra, n, threshold, nserrors, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            ra.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            ca.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
            ca.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
        }
    }
    testevdunit_testnsevdproblem(&ra, n, threshold, nserrors, _state);
    testevdunit_rmatrixfillsparsea(&ra, n, n, 0.995, _state);
    testevdunit_cmatrixfillsparsea(&ca, n, n, 0.995, _state);
    testevdunit_testnsevdproblem(&ra, n, threshold, nserrors, _state);
    ae_frame_leave(_state);
}


/*$ End $*/
