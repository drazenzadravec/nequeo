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
#include "testmatinvunit.h"


/*$ Declarations $*/
static void testmatinvunit_rmatrixmakeacopy(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_state *_state);
static void testmatinvunit_cmatrixmakeacopy(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_state *_state);
static ae_bool testmatinvunit_rmatrixcheckinverse(/* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* inva,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state);
static ae_bool testmatinvunit_spdmatrixcheckinverse(/* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* inva,
     ae_bool isupper,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state);
static ae_bool testmatinvunit_hpdmatrixcheckinverse(/* Complex */ ae_matrix* a,
     /* Complex */ ae_matrix* inva,
     ae_bool isupper,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state);
static ae_bool testmatinvunit_rmatrixcheckinversesingular(/* Real    */ ae_matrix* inva,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state);
static ae_bool testmatinvunit_cmatrixcheckinverse(/* Complex */ ae_matrix* a,
     /* Complex */ ae_matrix* inva,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state);
static ae_bool testmatinvunit_cmatrixcheckinversesingular(/* Complex */ ae_matrix* inva,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state);
static void testmatinvunit_rmatrixdrophalf(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool droplower,
     ae_state *_state);
static void testmatinvunit_cmatrixdrophalf(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool droplower,
     ae_state *_state);
static void testmatinvunit_testrtrinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* rtrerrors,
     ae_state *_state);
static void testmatinvunit_testctrinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* ctrerrors,
     ae_state *_state);
static void testmatinvunit_testrinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* rerrors,
     ae_state *_state);
static void testmatinvunit_testcinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* cerrors,
     ae_state *_state);
static void testmatinvunit_testspdinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* spderrors,
     ae_state *_state);
static void testmatinvunit_testhpdinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* hpderrors,
     ae_state *_state);
static void testmatinvunit_unset2d(/* Real    */ ae_matrix* x,
     ae_state *_state);
static void testmatinvunit_cunset2d(/* Complex */ ae_matrix* x,
     ae_state *_state);
static void testmatinvunit_unsetrep(matinvreport* r, ae_state *_state);


/*$ Body $*/


/*************************************************************************
Test
*************************************************************************/
ae_bool testmatinv(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t maxrn;
    ae_int_t maxcn;
    ae_int_t largen;
    ae_int_t passcount;
    double threshold;
    double rcondtol;
    ae_bool rtrerrors;
    ae_bool ctrerrors;
    ae_bool rerrors;
    ae_bool cerrors;
    ae_bool spderrors;
    ae_bool hpderrors;
    ae_bool waserrors;
    ae_matrix emptyra;
    ae_matrix emptyca;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&emptyra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&emptyca, 0, 0, DT_REAL, _state);

    maxrn = 3*ablasblocksize(&emptyra, _state)+1;
    maxcn = 3*ablasblocksize(&emptyca, _state)+1;
    largen = 256;
    passcount = 1;
    threshold = 10000*ae_machineepsilon;
    rcondtol = 0.01;
    rtrerrors = ae_false;
    ctrerrors = ae_false;
    rerrors = ae_false;
    cerrors = ae_false;
    spderrors = ae_false;
    hpderrors = ae_false;
    testmatinvunit_testrtrinv(1, maxrn, passcount, threshold, &rtrerrors, _state);
    testmatinvunit_testctrinv(1, maxcn, passcount, threshold, &ctrerrors, _state);
    testmatinvunit_testrinv(1, maxrn, passcount, threshold, &rerrors, _state);
    testmatinvunit_testspdinv(1, maxrn, passcount, threshold, &spderrors, _state);
    testmatinvunit_testcinv(1, maxcn, passcount, threshold, &cerrors, _state);
    testmatinvunit_testhpdinv(1, maxcn, passcount, threshold, &hpderrors, _state);
    testmatinvunit_testrtrinv(largen, largen, passcount, threshold, &rtrerrors, _state);
    testmatinvunit_testctrinv(largen, largen, passcount, threshold, &ctrerrors, _state);
    testmatinvunit_testrinv(largen, largen, passcount, threshold, &rerrors, _state);
    testmatinvunit_testspdinv(largen, largen, passcount, threshold, &spderrors, _state);
    testmatinvunit_testcinv(largen, largen, passcount, threshold, &cerrors, _state);
    testmatinvunit_testhpdinv(largen, largen, passcount, threshold, &hpderrors, _state);
    waserrors = ((((rtrerrors||ctrerrors)||rerrors)||cerrors)||spderrors)||hpderrors;
    if( !silent )
    {
        printf("TESTING MATINV\n");
        printf("* REAL TRIANGULAR:                        ");
        if( rtrerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* COMPLEX TRIANGULAR:                     ");
        if( ctrerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* REAL:                                   ");
        if( rerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* COMPLEX:                                ");
        if( cerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* SPD:                                    ");
        if( spderrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* HPD:                                    ");
        if( hpderrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        if( waserrors )
        {
            printf("TEST FAILED\n");
        }
        else
        {
            printf("TEST PASSED\n");
        }
    }
    result = !waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testmatinv(ae_bool silent, ae_state *_state)
{
    return testmatinv(silent, _state);
}


/*************************************************************************
Copy
*************************************************************************/
static void testmatinvunit_rmatrixmakeacopy(/* Real    */ ae_matrix* a,
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
Copy
*************************************************************************/
static void testmatinvunit_cmatrixmakeacopy(/* Complex */ ae_matrix* a,
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
Checks whether inverse is correct
Returns True on success.
*************************************************************************/
static ae_bool testmatinvunit_rmatrixcheckinverse(/* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* inva,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_bool result;


    result = ae_true;
    if( info<=0 )
    {
        result = ae_false;
    }
    else
    {
        result = result&&!(ae_fp_less(rep->r1,100*ae_machineepsilon)||ae_fp_greater(rep->r1,1+1000*ae_machineepsilon));
        result = result&&!(ae_fp_less(rep->rinf,100*ae_machineepsilon)||ae_fp_greater(rep->rinf,1+1000*ae_machineepsilon));
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                v = ae_v_dotproduct(&a->ptr.pp_double[i][0], 1, &inva->ptr.pp_double[0][j], inva->stride, ae_v_len(0,n-1));
                if( i==j )
                {
                    v = v-1;
                }
                result = result&&ae_fp_less_eq(ae_fabs(v, _state),threshold);
            }
        }
    }
    return result;
}


/*************************************************************************
Checks whether inverse is correct
Returns True on success.
*************************************************************************/
static ae_bool testmatinvunit_spdmatrixcheckinverse(/* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* inva,
     ae_bool isupper,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_matrix _inva;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_matrix_init_copy(&_inva, inva, _state);
    inva = &_inva;

    for(i=0; i<=n-2; i++)
    {
        if( isupper )
        {
            ae_v_move(&a->ptr.pp_double[i+1][i], a->stride, &a->ptr.pp_double[i][i+1], 1, ae_v_len(i+1,n-1));
            ae_v_move(&inva->ptr.pp_double[i+1][i], inva->stride, &inva->ptr.pp_double[i][i+1], 1, ae_v_len(i+1,n-1));
        }
        else
        {
            ae_v_move(&a->ptr.pp_double[i][i+1], 1, &a->ptr.pp_double[i+1][i], a->stride, ae_v_len(i+1,n-1));
            ae_v_move(&inva->ptr.pp_double[i][i+1], 1, &inva->ptr.pp_double[i+1][i], inva->stride, ae_v_len(i+1,n-1));
        }
    }
    result = ae_true;
    if( info<=0 )
    {
        result = ae_false;
    }
    else
    {
        result = result&&!(ae_fp_less(rep->r1,100*ae_machineepsilon)||ae_fp_greater(rep->r1,1+1000*ae_machineepsilon));
        result = result&&!(ae_fp_less(rep->rinf,100*ae_machineepsilon)||ae_fp_greater(rep->rinf,1+1000*ae_machineepsilon));
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                v = ae_v_dotproduct(&a->ptr.pp_double[i][0], 1, &inva->ptr.pp_double[0][j], inva->stride, ae_v_len(0,n-1));
                if( i==j )
                {
                    v = v-1;
                }
                result = result&&ae_fp_less_eq(ae_fabs(v, _state),threshold);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Checks whether inverse is correct
Returns True on success.
*************************************************************************/
static ae_bool testmatinvunit_hpdmatrixcheckinverse(/* Complex */ ae_matrix* a,
     /* Complex */ ae_matrix* inva,
     ae_bool isupper,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_matrix _inva;
    ae_int_t i;
    ae_int_t j;
    ae_complex v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_matrix_init_copy(&_inva, inva, _state);
    inva = &_inva;

    for(i=0; i<=n-2; i++)
    {
        if( isupper )
        {
            ae_v_cmove(&a->ptr.pp_complex[i+1][i], a->stride, &a->ptr.pp_complex[i][i+1], 1, "Conj", ae_v_len(i+1,n-1));
            ae_v_cmove(&inva->ptr.pp_complex[i+1][i], inva->stride, &inva->ptr.pp_complex[i][i+1], 1, "Conj", ae_v_len(i+1,n-1));
        }
        else
        {
            ae_v_cmove(&a->ptr.pp_complex[i][i+1], 1, &a->ptr.pp_complex[i+1][i], a->stride, "Conj", ae_v_len(i+1,n-1));
            ae_v_cmove(&inva->ptr.pp_complex[i][i+1], 1, &inva->ptr.pp_complex[i+1][i], inva->stride, "Conj", ae_v_len(i+1,n-1));
        }
    }
    result = ae_true;
    if( info<=0 )
    {
        result = ae_false;
    }
    else
    {
        result = result&&!(ae_fp_less(rep->r1,100*ae_machineepsilon)||ae_fp_greater(rep->r1,1+1000*ae_machineepsilon));
        result = result&&!(ae_fp_less(rep->rinf,100*ae_machineepsilon)||ae_fp_greater(rep->rinf,1+1000*ae_machineepsilon));
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                v = ae_v_cdotproduct(&a->ptr.pp_complex[i][0], 1, "N", &inva->ptr.pp_complex[0][j], inva->stride, "N", ae_v_len(0,n-1));
                if( i==j )
                {
                    v = ae_c_sub_d(v,1);
                }
                result = result&&ae_fp_less_eq(ae_c_abs(v, _state),threshold);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Checks whether inversion result indicate singular matrix
Returns True on success.
*************************************************************************/
static ae_bool testmatinvunit_rmatrixcheckinversesingular(/* Real    */ ae_matrix* inva,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_bool result;


    result = ae_true;
    if( info!=-3&&info!=1 )
    {
        result = ae_false;
    }
    else
    {
        result = result&&!(ae_fp_less(rep->r1,(double)(0))||ae_fp_greater(rep->r1,1000*ae_machineepsilon));
        result = result&&!(ae_fp_less(rep->rinf,(double)(0))||ae_fp_greater(rep->rinf,1000*ae_machineepsilon));
        if( info==-3 )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    result = result&&ae_fp_eq(inva->ptr.pp_double[i][j],(double)(0));
                }
            }
        }
    }
    return result;
}


/*************************************************************************
Checks whether inverse is correct
Returns True on success.
*************************************************************************/
static ae_bool testmatinvunit_cmatrixcheckinverse(/* Complex */ ae_matrix* a,
     /* Complex */ ae_matrix* inva,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_complex v;
    ae_bool result;


    result = ae_true;
    if( info<=0 )
    {
        result = ae_false;
    }
    else
    {
        result = result&&!(ae_fp_less(rep->r1,100*ae_machineepsilon)||ae_fp_greater(rep->r1,1+1000*ae_machineepsilon));
        result = result&&!(ae_fp_less(rep->rinf,100*ae_machineepsilon)||ae_fp_greater(rep->rinf,1+1000*ae_machineepsilon));
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                v = ae_v_cdotproduct(&a->ptr.pp_complex[i][0], 1, "N", &inva->ptr.pp_complex[0][j], inva->stride, "N", ae_v_len(0,n-1));
                if( i==j )
                {
                    v = ae_c_sub_d(v,1);
                }
                result = result&&ae_fp_less_eq(ae_c_abs(v, _state),threshold);
            }
        }
    }
    return result;
}


/*************************************************************************
Checks whether inversion result indicate singular matrix
Returns True on success.
*************************************************************************/
static ae_bool testmatinvunit_cmatrixcheckinversesingular(/* Complex */ ae_matrix* inva,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     matinvreport* rep,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_bool result;


    result = ae_true;
    if( info!=-3&&info!=1 )
    {
        result = ae_false;
    }
    else
    {
        result = result&&!(ae_fp_less(rep->r1,(double)(0))||ae_fp_greater(rep->r1,1000*ae_machineepsilon));
        result = result&&!(ae_fp_less(rep->rinf,(double)(0))||ae_fp_greater(rep->rinf,1000*ae_machineepsilon));
        if( info==-3 )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    result = result&&ae_c_eq_d(inva->ptr.pp_complex[i][j],(double)(0));
                }
            }
        }
    }
    return result;
}


/*************************************************************************
Drops upper or lower half of the matrix - fills it by special pattern
which may be used later to ensure that this part wasn't changed
*************************************************************************/
static void testmatinvunit_rmatrixdrophalf(/* Real    */ ae_matrix* a,
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
static void testmatinvunit_cmatrixdrophalf(/* Complex */ ae_matrix* a,
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
Real TR inverse
*************************************************************************/
static void testmatinvunit_testrtrinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* rtrerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix b;
    ae_int_t n;
    ae_int_t pass;
    ae_int_t i;
    ae_int_t j;
    ae_int_t task;
    ae_bool isupper;
    ae_bool isunit;
    double v;
    ae_bool waserrors;
    ae_int_t info;
    matinvreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&b, 0, 0, DT_REAL, _state);
    _matinvreport_init(&rep, _state);

    waserrors = ae_false;
    
    /*
     * Test
     */
    for(n=minn; n<=maxn; n++)
    {
        ae_matrix_set_length(&a, n, n, _state);
        ae_matrix_set_length(&b, n, n, _state);
        for(task=0; task<=3; task++)
        {
            for(pass=1; pass<=passcount; pass++)
            {
                
                /*
                 * Determine task
                 */
                isupper = task%2==0;
                isunit = task/2%2==0;
                
                /*
                 * Generate matrix
                 */
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( i==j )
                        {
                            a.ptr.pp_double[i][i] = 1+ae_randomreal(_state);
                        }
                        else
                        {
                            a.ptr.pp_double[i][j] = 0.2*ae_randomreal(_state)-0.1;
                        }
                        b.ptr.pp_double[i][j] = a.ptr.pp_double[i][j];
                    }
                }
                
                /*
                 * Inverse
                 */
                rmatrixtrinverse(&b, n, isupper, isunit, &info, &rep, _state);
                if( info<=0 )
                {
                    *rtrerrors = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                
                /*
                 * Structural test
                 */
                if( isunit )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        *rtrerrors = *rtrerrors||ae_fp_neq(a.ptr.pp_double[i][i],b.ptr.pp_double[i][i]);
                    }
                }
                if( isupper )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=i-1; j++)
                        {
                            *rtrerrors = *rtrerrors||ae_fp_neq(a.ptr.pp_double[i][j],b.ptr.pp_double[i][j]);
                        }
                    }
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=i+1; j<=n-1; j++)
                        {
                            *rtrerrors = *rtrerrors||ae_fp_neq(a.ptr.pp_double[i][j],b.ptr.pp_double[i][j]);
                        }
                    }
                }
                
                /*
                 * Inverse test
                 */
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( (j<i&&isupper)||(j>i&&!isupper) )
                        {
                            a.ptr.pp_double[i][j] = (double)(0);
                            b.ptr.pp_double[i][j] = (double)(0);
                        }
                    }
                }
                if( isunit )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        a.ptr.pp_double[i][i] = (double)(1);
                        b.ptr.pp_double[i][i] = (double)(1);
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &b.ptr.pp_double[0][j], b.stride, ae_v_len(0,n-1));
                        if( j!=i )
                        {
                            *rtrerrors = *rtrerrors||ae_fp_greater(ae_fabs(v, _state),threshold);
                        }
                        else
                        {
                            *rtrerrors = *rtrerrors||ae_fp_greater(ae_fabs(v-1, _state),threshold);
                        }
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Complex TR inverse
*************************************************************************/
static void testmatinvunit_testctrinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* ctrerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix b;
    ae_int_t n;
    ae_int_t pass;
    ae_int_t i;
    ae_int_t j;
    ae_int_t task;
    ae_bool isupper;
    ae_bool isunit;
    ae_complex v;
    ae_bool waserrors;
    ae_int_t info;
    matinvreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&b, 0, 0, DT_COMPLEX, _state);
    _matinvreport_init(&rep, _state);

    waserrors = ae_false;
    
    /*
     * Test
     */
    for(n=minn; n<=maxn; n++)
    {
        ae_matrix_set_length(&a, n, n, _state);
        ae_matrix_set_length(&b, n, n, _state);
        for(task=0; task<=3; task++)
        {
            for(pass=1; pass<=passcount; pass++)
            {
                
                /*
                 * Determine task
                 */
                isupper = task%2==0;
                isunit = task/2%2==0;
                
                /*
                 * Generate matrix
                 */
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( i==j )
                        {
                            a.ptr.pp_complex[i][i].x = 1+ae_randomreal(_state);
                            a.ptr.pp_complex[i][i].y = 1+ae_randomreal(_state);
                        }
                        else
                        {
                            a.ptr.pp_complex[i][j].x = 0.2*ae_randomreal(_state)-0.1;
                            a.ptr.pp_complex[i][j].y = 0.2*ae_randomreal(_state)-0.1;
                        }
                        b.ptr.pp_complex[i][j] = a.ptr.pp_complex[i][j];
                    }
                }
                
                /*
                 * Inverse
                 */
                cmatrixtrinverse(&b, n, isupper, isunit, &info, &rep, _state);
                if( info<=0 )
                {
                    *ctrerrors = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                
                /*
                 * Structural test
                 */
                if( isunit )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        *ctrerrors = *ctrerrors||ae_c_neq(a.ptr.pp_complex[i][i],b.ptr.pp_complex[i][i]);
                    }
                }
                if( isupper )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=i-1; j++)
                        {
                            *ctrerrors = *ctrerrors||ae_c_neq(a.ptr.pp_complex[i][j],b.ptr.pp_complex[i][j]);
                        }
                    }
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=i+1; j<=n-1; j++)
                        {
                            *ctrerrors = *ctrerrors||ae_c_neq(a.ptr.pp_complex[i][j],b.ptr.pp_complex[i][j]);
                        }
                    }
                }
                
                /*
                 * Inverse test
                 */
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( (j<i&&isupper)||(j>i&&!isupper) )
                        {
                            a.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                            b.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                        }
                    }
                }
                if( isunit )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        a.ptr.pp_complex[i][i] = ae_complex_from_i(1);
                        b.ptr.pp_complex[i][i] = ae_complex_from_i(1);
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        v = ae_v_cdotproduct(&a.ptr.pp_complex[i][0], 1, "N", &b.ptr.pp_complex[0][j], b.stride, "N", ae_v_len(0,n-1));
                        if( j!=i )
                        {
                            *ctrerrors = *ctrerrors||ae_fp_greater(ae_c_abs(v, _state),threshold);
                        }
                        else
                        {
                            *ctrerrors = *ctrerrors||ae_fp_greater(ae_c_abs(ae_c_sub_d(v,1), _state),threshold);
                        }
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Real test
*************************************************************************/
static void testmatinvunit_testrinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* rerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix lua;
    ae_matrix inva;
    ae_matrix invlua;
    ae_vector p;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t n;
    ae_int_t pass;
    ae_int_t taskkind;
    ae_int_t info;
    matinvreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&lua, 0, 0, DT_REAL, _state);
    ae_matrix_init(&inva, 0, 0, DT_REAL, _state);
    ae_matrix_init(&invlua, 0, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    _matinvreport_init(&rep, _state);

    
    /*
     * General square matrices:
     * * test general solvers
     * * test least squares solver
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=minn; n<=maxn; n++)
        {
            
            /*
             * ********************************************************
             * WELL CONDITIONED TASKS
             * ability to find correct solution is tested
             * ********************************************************
             *
             * 1. generate random well conditioned matrix A.
             * 2. generate random solution vector xe
             * 3. generate right part b=A*xe
             * 4. test different methods on original A
             */
            rmatrixrndcond(n, (double)(1000), &a, _state);
            testmatinvunit_rmatrixmakeacopy(&a, n, n, &lua, _state);
            rmatrixlu(&lua, n, n, &p, _state);
            testmatinvunit_rmatrixmakeacopy(&a, n, n, &inva, _state);
            testmatinvunit_rmatrixmakeacopy(&lua, n, n, &invlua, _state);
            info = 0;
            testmatinvunit_unsetrep(&rep, _state);
            rmatrixinverse(&inva, n, &info, &rep, _state);
            *rerrors = *rerrors||!testmatinvunit_rmatrixcheckinverse(&a, &inva, n, threshold, info, &rep, _state);
            info = 0;
            testmatinvunit_unsetrep(&rep, _state);
            rmatrixluinverse(&invlua, &p, n, &info, &rep, _state);
            *rerrors = *rerrors||!testmatinvunit_rmatrixcheckinverse(&a, &invlua, n, threshold, info, &rep, _state);
            
            /*
             * ********************************************************
             * EXACTLY SINGULAR MATRICES
             * ability to detect singularity is tested
             * ********************************************************
             *
             * 1. generate different types of singular matrices:
             *    * zero
             *    * with zero columns
             *    * with zero rows
             *    * with equal rows/columns
             * 2. test different methods
             */
            for(taskkind=0; taskkind<=4; taskkind++)
            {
                testmatinvunit_unset2d(&a, _state);
                if( taskkind==0 )
                {
                    
                    /*
                     * all zeros
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = (double)(0);
                        }
                    }
                }
                if( taskkind==1 )
                {
                    
                    /*
                     * there is zero column
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = ae_randominteger(n, _state);
                    ae_v_muld(&a.ptr.pp_double[0][k], a.stride, ae_v_len(0,n-1), 0);
                }
                if( taskkind==2 )
                {
                    
                    /*
                     * there is zero row
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = ae_randominteger(n, _state);
                    ae_v_muld(&a.ptr.pp_double[k][0], 1, ae_v_len(0,n-1), 0);
                }
                if( taskkind==3 )
                {
                    
                    /*
                     * equal columns
                     */
                    if( n<2 )
                    {
                        continue;
                    }
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = 1+ae_randominteger(n-1, _state);
                    ae_v_move(&a.ptr.pp_double[0][0], a.stride, &a.ptr.pp_double[0][k], a.stride, ae_v_len(0,n-1));
                }
                if( taskkind==4 )
                {
                    
                    /*
                     * equal rows
                     */
                    if( n<2 )
                    {
                        continue;
                    }
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = 1+ae_randominteger(n-1, _state);
                    ae_v_move(&a.ptr.pp_double[0][0], 1, &a.ptr.pp_double[k][0], 1, ae_v_len(0,n-1));
                }
                testmatinvunit_rmatrixmakeacopy(&a, n, n, &lua, _state);
                rmatrixlu(&lua, n, n, &p, _state);
                info = 0;
                testmatinvunit_unsetrep(&rep, _state);
                rmatrixinverse(&a, n, &info, &rep, _state);
                *rerrors = *rerrors||!testmatinvunit_rmatrixcheckinversesingular(&a, n, threshold, info, &rep, _state);
                info = 0;
                testmatinvunit_unsetrep(&rep, _state);
                rmatrixluinverse(&lua, &p, n, &info, &rep, _state);
                *rerrors = *rerrors||!testmatinvunit_rmatrixcheckinversesingular(&lua, n, threshold, info, &rep, _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Complex test
*************************************************************************/
static void testmatinvunit_testcinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* cerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix lua;
    ae_matrix inva;
    ae_matrix invlua;
    ae_vector p;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t n;
    ae_int_t pass;
    ae_int_t taskkind;
    ae_int_t info;
    matinvreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&lua, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&inva, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&invlua, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    _matinvreport_init(&rep, _state);

    
    /*
     * General square matrices:
     * * test general solvers
     * * test least squares solver
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=minn; n<=maxn; n++)
        {
            
            /*
             * ********************************************************
             * WELL CONDITIONED TASKS
             * ability to find correct solution is tested
             * ********************************************************
             *
             * 1. generate random well conditioned matrix A.
             * 2. generate random solution vector xe
             * 3. generate right part b=A*xe
             * 4. test different methods on original A
             */
            cmatrixrndcond(n, (double)(1000), &a, _state);
            testmatinvunit_cmatrixmakeacopy(&a, n, n, &lua, _state);
            cmatrixlu(&lua, n, n, &p, _state);
            testmatinvunit_cmatrixmakeacopy(&a, n, n, &inva, _state);
            testmatinvunit_cmatrixmakeacopy(&lua, n, n, &invlua, _state);
            info = 0;
            testmatinvunit_unsetrep(&rep, _state);
            cmatrixinverse(&inva, n, &info, &rep, _state);
            *cerrors = *cerrors||!testmatinvunit_cmatrixcheckinverse(&a, &inva, n, threshold, info, &rep, _state);
            info = 0;
            testmatinvunit_unsetrep(&rep, _state);
            cmatrixluinverse(&invlua, &p, n, &info, &rep, _state);
            *cerrors = *cerrors||!testmatinvunit_cmatrixcheckinverse(&a, &invlua, n, threshold, info, &rep, _state);
            
            /*
             * ********************************************************
             * EXACTLY SINGULAR MATRICES
             * ability to detect singularity is tested
             * ********************************************************
             *
             * 1. generate different types of singular matrices:
             *    * zero
             *    * with zero columns
             *    * with zero rows
             *    * with equal rows/columns
             * 2. test different methods
             */
            for(taskkind=0; taskkind<=4; taskkind++)
            {
                testmatinvunit_cunset2d(&a, _state);
                if( taskkind==0 )
                {
                    
                    /*
                     * all zeros
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                        }
                    }
                }
                if( taskkind==1 )
                {
                    
                    /*
                     * there is zero column
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                            a.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = ae_randominteger(n, _state);
                    ae_v_cmuld(&a.ptr.pp_complex[0][k], a.stride, ae_v_len(0,n-1), 0);
                }
                if( taskkind==2 )
                {
                    
                    /*
                     * there is zero row
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                            a.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = ae_randominteger(n, _state);
                    ae_v_cmuld(&a.ptr.pp_complex[k][0], 1, ae_v_len(0,n-1), 0);
                }
                if( taskkind==3 )
                {
                    
                    /*
                     * equal columns
                     */
                    if( n<2 )
                    {
                        continue;
                    }
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                            a.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = 1+ae_randominteger(n-1, _state);
                    ae_v_cmove(&a.ptr.pp_complex[0][0], a.stride, &a.ptr.pp_complex[0][k], a.stride, "N", ae_v_len(0,n-1));
                }
                if( taskkind==4 )
                {
                    
                    /*
                     * equal rows
                     */
                    if( n<2 )
                    {
                        continue;
                    }
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                            a.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = 1+ae_randominteger(n-1, _state);
                    ae_v_cmove(&a.ptr.pp_complex[0][0], 1, &a.ptr.pp_complex[k][0], 1, "N", ae_v_len(0,n-1));
                }
                testmatinvunit_cmatrixmakeacopy(&a, n, n, &lua, _state);
                cmatrixlu(&lua, n, n, &p, _state);
                info = 0;
                testmatinvunit_unsetrep(&rep, _state);
                cmatrixinverse(&a, n, &info, &rep, _state);
                *cerrors = *cerrors||!testmatinvunit_cmatrixcheckinversesingular(&a, n, threshold, info, &rep, _state);
                info = 0;
                testmatinvunit_unsetrep(&rep, _state);
                cmatrixluinverse(&lua, &p, n, &info, &rep, _state);
                *cerrors = *cerrors||!testmatinvunit_cmatrixcheckinversesingular(&lua, n, threshold, info, &rep, _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
SPD test
*************************************************************************/
static void testmatinvunit_testspdinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* spderrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix cha;
    ae_matrix inva;
    ae_matrix invcha;
    ae_bool isupper;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t n;
    ae_int_t pass;
    ae_int_t taskkind;
    ae_int_t info;
    matinvreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cha, 0, 0, DT_REAL, _state);
    ae_matrix_init(&inva, 0, 0, DT_REAL, _state);
    ae_matrix_init(&invcha, 0, 0, DT_REAL, _state);
    _matinvreport_init(&rep, _state);

    
    /*
     * General square matrices:
     * * test general solvers
     * * test least squares solver
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=minn; n<=maxn; n++)
        {
            isupper = ae_fp_greater(ae_randomreal(_state),0.5);
            
            /*
             * ********************************************************
             * WELL CONDITIONED TASKS
             * ability to find correct solution is tested
             * ********************************************************
             *
             * 1. generate random well conditioned matrix A.
             * 2. generate random solution vector xe
             * 3. generate right part b=A*xe
             * 4. test different methods on original A
             */
            spdmatrixrndcond(n, (double)(1000), &a, _state);
            testmatinvunit_rmatrixdrophalf(&a, n, isupper, _state);
            testmatinvunit_rmatrixmakeacopy(&a, n, n, &cha, _state);
            if( !spdmatrixcholesky(&cha, n, isupper, _state) )
            {
                continue;
            }
            testmatinvunit_rmatrixmakeacopy(&a, n, n, &inva, _state);
            testmatinvunit_rmatrixmakeacopy(&cha, n, n, &invcha, _state);
            info = 0;
            testmatinvunit_unsetrep(&rep, _state);
            spdmatrixinverse(&inva, n, isupper, &info, &rep, _state);
            *spderrors = *spderrors||!testmatinvunit_spdmatrixcheckinverse(&a, &inva, isupper, n, threshold, info, &rep, _state);
            info = 0;
            testmatinvunit_unsetrep(&rep, _state);
            spdmatrixcholeskyinverse(&invcha, n, isupper, &info, &rep, _state);
            *spderrors = *spderrors||!testmatinvunit_spdmatrixcheckinverse(&a, &invcha, isupper, n, threshold, info, &rep, _state);
            
            /*
             * ********************************************************
             * EXACTLY SINGULAR MATRICES
             * ability to detect singularity is tested
             * ********************************************************
             *
             * 1. generate different types of singular matrices:
             *    * zero
             *    * with zero columns
             *    * with zero rows
             * 2. test different methods
             */
            for(taskkind=0; taskkind<=2; taskkind++)
            {
                testmatinvunit_unset2d(&a, _state);
                if( taskkind==0 )
                {
                    
                    /*
                     * all zeros
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = (double)(0);
                        }
                    }
                }
                if( taskkind==1 )
                {
                    
                    /*
                     * there is zero column
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = ae_randominteger(n, _state);
                    ae_v_muld(&a.ptr.pp_double[0][k], a.stride, ae_v_len(0,n-1), 0);
                }
                if( taskkind==2 )
                {
                    
                    /*
                     * there is zero row
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = ae_randominteger(n, _state);
                    ae_v_muld(&a.ptr.pp_double[k][0], 1, ae_v_len(0,n-1), 0);
                }
                info = 0;
                testmatinvunit_unsetrep(&rep, _state);
                spdmatrixcholeskyinverse(&a, n, isupper, &info, &rep, _state);
                if( info!=-3&&info!=1 )
                {
                    *spderrors = ae_true;
                }
                else
                {
                    *spderrors = (*spderrors||ae_fp_less(rep.r1,(double)(0)))||ae_fp_greater(rep.r1,1000*ae_machineepsilon);
                    *spderrors = (*spderrors||ae_fp_less(rep.rinf,(double)(0)))||ae_fp_greater(rep.rinf,1000*ae_machineepsilon);
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
HPD test
*************************************************************************/
static void testmatinvunit_testhpdinv(ae_int_t minn,
     ae_int_t maxn,
     ae_int_t passcount,
     double threshold,
     ae_bool* hpderrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix cha;
    ae_matrix inva;
    ae_matrix invcha;
    ae_bool isupper;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t n;
    ae_int_t pass;
    ae_int_t taskkind;
    ae_int_t info;
    matinvreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cha, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&inva, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&invcha, 0, 0, DT_COMPLEX, _state);
    _matinvreport_init(&rep, _state);

    
    /*
     * General square matrices:
     * * test general solvers
     * * test least squares solver
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=minn; n<=maxn; n++)
        {
            isupper = ae_fp_greater(ae_randomreal(_state),0.5);
            
            /*
             * ********************************************************
             * WELL CONDITIONED TASKS
             * ability to find correct solution is tested
             * ********************************************************
             *
             * 1. generate random well conditioned matrix A.
             * 2. generate random solution vector xe
             * 3. generate right part b=A*xe
             * 4. test different methods on original A
             */
            hpdmatrixrndcond(n, (double)(1000), &a, _state);
            testmatinvunit_cmatrixdrophalf(&a, n, isupper, _state);
            testmatinvunit_cmatrixmakeacopy(&a, n, n, &cha, _state);
            if( !hpdmatrixcholesky(&cha, n, isupper, _state) )
            {
                continue;
            }
            testmatinvunit_cmatrixmakeacopy(&a, n, n, &inva, _state);
            testmatinvunit_cmatrixmakeacopy(&cha, n, n, &invcha, _state);
            info = 0;
            testmatinvunit_unsetrep(&rep, _state);
            hpdmatrixinverse(&inva, n, isupper, &info, &rep, _state);
            *hpderrors = *hpderrors||!testmatinvunit_hpdmatrixcheckinverse(&a, &inva, isupper, n, threshold, info, &rep, _state);
            info = 0;
            testmatinvunit_unsetrep(&rep, _state);
            hpdmatrixcholeskyinverse(&invcha, n, isupper, &info, &rep, _state);
            *hpderrors = *hpderrors||!testmatinvunit_hpdmatrixcheckinverse(&a, &invcha, isupper, n, threshold, info, &rep, _state);
            
            /*
             * ********************************************************
             * EXACTLY SINGULAR MATRICES
             * ability to detect singularity is tested
             * ********************************************************
             *
             * 1. generate different types of singular matrices:
             *    * zero
             *    * with zero columns
             *    * with zero rows
             * 2. test different methods
             */
            for(taskkind=0; taskkind<=2; taskkind++)
            {
                testmatinvunit_cunset2d(&a, _state);
                if( taskkind==0 )
                {
                    
                    /*
                     * all zeros
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                        }
                    }
                }
                if( taskkind==1 )
                {
                    
                    /*
                     * there is zero column
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                            a.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = ae_randominteger(n, _state);
                    ae_v_cmuld(&a.ptr.pp_complex[0][k], a.stride, ae_v_len(0,n-1), 0);
                    ae_v_cmuld(&a.ptr.pp_complex[k][0], 1, ae_v_len(0,n-1), 0);
                }
                if( taskkind==2 )
                {
                    
                    /*
                     * there is zero row
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                            a.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                        }
                    }
                    k = ae_randominteger(n, _state);
                    ae_v_cmuld(&a.ptr.pp_complex[k][0], 1, ae_v_len(0,n-1), 0);
                    ae_v_cmuld(&a.ptr.pp_complex[0][k], a.stride, ae_v_len(0,n-1), 0);
                }
                info = 0;
                testmatinvunit_unsetrep(&rep, _state);
                hpdmatrixcholeskyinverse(&a, n, isupper, &info, &rep, _state);
                if( info!=-3&&info!=1 )
                {
                    *hpderrors = ae_true;
                }
                else
                {
                    *hpderrors = (*hpderrors||ae_fp_less(rep.r1,(double)(0)))||ae_fp_greater(rep.r1,1000*ae_machineepsilon);
                    *hpderrors = (*hpderrors||ae_fp_less(rep.rinf,(double)(0)))||ae_fp_greater(rep.rinf,1000*ae_machineepsilon);
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unsets real matrix
*************************************************************************/
static void testmatinvunit_unset2d(/* Real    */ ae_matrix* x,
     ae_state *_state)
{


    ae_matrix_set_length(x, 1, 1, _state);
    x->ptr.pp_double[0][0] = 2*ae_randomreal(_state)-1;
}


/*************************************************************************
Unsets real matrix
*************************************************************************/
static void testmatinvunit_cunset2d(/* Complex */ ae_matrix* x,
     ae_state *_state)
{


    ae_matrix_set_length(x, 1, 1, _state);
    x->ptr.pp_complex[0][0] = ae_complex_from_d(2*ae_randomreal(_state)-1);
}


/*************************************************************************
Unsets report
*************************************************************************/
static void testmatinvunit_unsetrep(matinvreport* r, ae_state *_state)
{


    r->r1 = (double)(-1);
    r->rinf = (double)(-1);
}


/*$ End $*/
