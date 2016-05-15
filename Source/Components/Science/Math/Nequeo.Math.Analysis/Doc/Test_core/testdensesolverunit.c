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
#include "testdensesolverunit.h"


/*$ Declarations $*/
static ae_bool testdensesolverunit_rmatrixchecksolutionm(/* Real    */ ae_matrix* xe,
     ae_int_t n,
     ae_int_t m,
     double threshold,
     ae_int_t info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_rmatrixchecksolutionmfast(/* Real    */ ae_matrix* xe,
     ae_int_t n,
     ae_int_t m,
     double threshold,
     ae_int_t info,
     /* Real    */ ae_matrix* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_rmatrixchecksolution(/* Real    */ ae_matrix* xe,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     densesolverreport* rep,
     /* Real    */ ae_vector* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_rmatrixchecksolutionfast(/* Real    */ ae_matrix* xe,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     /* Real    */ ae_vector* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_rmatrixchecksingularm(ae_int_t n,
     ae_int_t m,
     ae_int_t info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_rmatrixchecksingularmfast(ae_int_t n,
     ae_int_t m,
     ae_int_t info,
     /* Real    */ ae_matrix* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_rmatrixchecksingular(ae_int_t n,
     ae_int_t info,
     densesolverreport* rep,
     /* Real    */ ae_vector* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_rmatrixchecksingularfast(ae_int_t n,
     ae_int_t info,
     /* Real    */ ae_vector* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_cmatrixchecksolutionm(/* Complex */ ae_matrix* xe,
     ae_int_t n,
     ae_int_t m,
     double threshold,
     ae_int_t info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_cmatrixchecksolutionmfast(/* Complex */ ae_matrix* xe,
     ae_int_t n,
     ae_int_t m,
     double threshold,
     ae_int_t info,
     /* Complex */ ae_matrix* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_cmatrixchecksolution(/* Complex */ ae_matrix* xe,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     densesolverreport* rep,
     /* Complex */ ae_vector* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_cmatrixchecksolutionfast(/* Complex */ ae_matrix* xe,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     /* Complex */ ae_vector* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_cmatrixchecksingularm(ae_int_t n,
     ae_int_t m,
     ae_int_t info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_cmatrixchecksingularmfast(ae_int_t n,
     ae_int_t m,
     ae_int_t info,
     /* Complex */ ae_matrix* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_cmatrixchecksingular(ae_int_t n,
     ae_int_t info,
     densesolverreport* rep,
     /* Complex */ ae_vector* xs,
     ae_state *_state);
static ae_bool testdensesolverunit_cmatrixchecksingularfast(ae_int_t n,
     ae_int_t info,
     /* Complex */ ae_vector* xs,
     ae_state *_state);
static void testdensesolverunit_rmatrixmakeacopy(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_state *_state);
static void testdensesolverunit_cmatrixmakeacopy(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_state *_state);
static void testdensesolverunit_rmatrixdrophalf(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool droplower,
     ae_state *_state);
static void testdensesolverunit_cmatrixdrophalf(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool droplower,
     ae_state *_state);
static void testdensesolverunit_testrsolver(ae_int_t maxn,
     ae_int_t maxm,
     ae_int_t passcount,
     double threshold,
     ae_bool* rerrors,
     ae_bool* rfserrors,
     ae_state *_state);
static void testdensesolverunit_testspdsolver(ae_int_t maxn,
     ae_int_t maxm,
     ae_int_t passcount,
     double threshold,
     ae_bool* spderrors,
     ae_bool* rfserrors,
     ae_state *_state);
static void testdensesolverunit_testcsolver(ae_int_t maxn,
     ae_int_t maxm,
     ae_int_t passcount,
     double threshold,
     ae_bool* cerrors,
     ae_bool* rfserrors,
     ae_state *_state);
static void testdensesolverunit_testhpdsolver(ae_int_t maxn,
     ae_int_t maxm,
     ae_int_t passcount,
     double threshold,
     ae_bool* hpderrors,
     ae_bool* rfserrors,
     ae_state *_state);
static void testdensesolverunit_unset2d(/* Real    */ ae_matrix* x,
     ae_state *_state);
static void testdensesolverunit_unset1d(/* Real    */ ae_vector* x,
     ae_state *_state);
static void testdensesolverunit_cunset2d(/* Complex */ ae_matrix* x,
     ae_state *_state);
static void testdensesolverunit_cunset1d(/* Complex */ ae_vector* x,
     ae_state *_state);
static void testdensesolverunit_unsetrep(densesolverreport* r,
     ae_state *_state);
static void testdensesolverunit_unsetlsrep(densesolverlsreport* r,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Test
*************************************************************************/
ae_bool testdensesolver(ae_bool silent, ae_state *_state)
{
    ae_int_t maxn;
    ae_int_t maxm;
    ae_int_t passcount;
    double threshold;
    ae_bool rerrors;
    ae_bool cerrors;
    ae_bool spderrors;
    ae_bool hpderrors;
    ae_bool rfserrors;
    ae_bool waserrors;
    ae_bool result;


    maxn = 10;
    maxm = 5;
    passcount = 5;
    threshold = 10000*ae_machineepsilon;
    rfserrors = ae_false;
    rerrors = ae_false;
    cerrors = ae_false;
    spderrors = ae_false;
    hpderrors = ae_false;
    testdensesolverunit_testrsolver(maxn, maxm, passcount, threshold, &rerrors, &rfserrors, _state);
    testdensesolverunit_testspdsolver(maxn, maxm, passcount, threshold, &spderrors, &rfserrors, _state);
    testdensesolverunit_testcsolver(maxn, maxm, passcount, threshold, &cerrors, &rfserrors, _state);
    testdensesolverunit_testhpdsolver(maxn, maxm, passcount, threshold, &hpderrors, &rfserrors, _state);
    waserrors = (((rerrors||cerrors)||spderrors)||hpderrors)||rfserrors;
    if( !silent )
    {
        printf("TESTING DENSE SOLVER\n");
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
        printf("* ITERATIVE IMPROVEMENT:                  ");
        if( rfserrors )
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
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testdensesolver(ae_bool silent, ae_state *_state)
{
    return testdensesolver(silent, _state);
}


/*************************************************************************
Checks whether solver results are correct solution.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_rmatrixchecksolutionm(/* Real    */ ae_matrix* xe,
     ae_int_t n,
     ae_int_t m,
     double threshold,
     ae_int_t info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* xs,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
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
            for(j=0; j<=m-1; j++)
            {
                result = result&&ae_fp_less_eq(ae_fabs(xe->ptr.pp_double[i][j]-xs->ptr.pp_double[i][j], _state),threshold);
            }
        }
    }
    return result;
}


/*************************************************************************
Checks whether solver results are correct solution.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_rmatrixchecksolutionmfast(/* Real    */ ae_matrix* xe,
     ae_int_t n,
     ae_int_t m,
     double threshold,
     ae_int_t info,
     /* Real    */ ae_matrix* xs,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_bool result;


    result = ae_true;
    if( info<=0 )
    {
        result = ae_false;
    }
    else
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                result = result&&ae_fp_less_eq(ae_fabs(xe->ptr.pp_double[i][j]-xs->ptr.pp_double[i][j], _state),threshold);
            }
        }
    }
    return result;
}


/*************************************************************************
Checks whether solver results are correct solution.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_rmatrixchecksolution(/* Real    */ ae_matrix* xe,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     densesolverreport* rep,
     /* Real    */ ae_vector* xs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xsm;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xsm, 0, 0, DT_REAL, _state);

    ae_matrix_set_length(&xsm, n, 1, _state);
    ae_v_move(&xsm.ptr.pp_double[0][0], xsm.stride, &xs->ptr.p_double[0], 1, ae_v_len(0,n-1));
    result = testdensesolverunit_rmatrixchecksolutionm(xe, n, 1, threshold, info, rep, &xsm, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Checks whether solver results are correct solution.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_rmatrixchecksolutionfast(/* Real    */ ae_matrix* xe,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     /* Real    */ ae_vector* xs,
     ae_state *_state)
{
    ae_int_t i;
    ae_bool result;


    result = ae_true;
    if( info<=0 )
    {
        result = ae_false;
    }
    else
    {
        for(i=0; i<=n-1; i++)
        {
            result = result&&ae_fp_less_eq(ae_fabs(xe->ptr.pp_double[i][0]-xs->ptr.p_double[i], _state),threshold);
        }
    }
    return result;
}


/*************************************************************************
Checks whether solver results indicate singular matrix.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_rmatrixchecksingularm(ae_int_t n,
     ae_int_t m,
     ae_int_t info,
     densesolverreport* rep,
     /* Real    */ ae_matrix* xs,
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
                for(j=0; j<=m-1; j++)
                {
                    result = result&&ae_fp_eq(xs->ptr.pp_double[i][j],(double)(0));
                }
            }
        }
    }
    return result;
}


/*************************************************************************
Checks whether solver results indicate singular matrix.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_rmatrixchecksingularmfast(ae_int_t n,
     ae_int_t m,
     ae_int_t info,
     /* Real    */ ae_matrix* xs,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_bool result;


    result = ae_true;
    if( info!=-3 )
    {
        result = ae_false;
    }
    else
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                result = result&&ae_fp_eq(xs->ptr.pp_double[i][j],(double)(0));
            }
        }
    }
    return result;
}


/*************************************************************************
Checks whether solver results indicate singular matrix.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_rmatrixchecksingular(ae_int_t n,
     ae_int_t info,
     densesolverreport* rep,
     /* Real    */ ae_vector* xs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xsm;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xsm, 0, 0, DT_REAL, _state);

    ae_matrix_set_length(&xsm, n, 1, _state);
    ae_v_move(&xsm.ptr.pp_double[0][0], xsm.stride, &xs->ptr.p_double[0], 1, ae_v_len(0,n-1));
    result = testdensesolverunit_rmatrixchecksingularm(n, 1, info, rep, &xsm, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Checks whether solver results indicate singular matrix.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_rmatrixchecksingularfast(ae_int_t n,
     ae_int_t info,
     /* Real    */ ae_vector* xs,
     ae_state *_state)
{
    ae_int_t i;
    ae_bool result;


    result = ae_true;
    if( info!=-3 )
    {
        result = ae_false;
    }
    else
    {
        for(i=0; i<=n-1; i++)
        {
            result = result&&ae_fp_eq(xs->ptr.p_double[i],(double)(0));
        }
    }
    return result;
}


/*************************************************************************
Checks whether solver results are correct solution.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_cmatrixchecksolutionm(/* Complex */ ae_matrix* xe,
     ae_int_t n,
     ae_int_t m,
     double threshold,
     ae_int_t info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* xs,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
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
            for(j=0; j<=m-1; j++)
            {
                result = result&&ae_fp_less_eq(ae_c_abs(ae_c_sub(xe->ptr.pp_complex[i][j],xs->ptr.pp_complex[i][j]), _state),threshold);
            }
        }
    }
    return result;
}


/*************************************************************************
Checks whether solver results are correct solution.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_cmatrixchecksolutionmfast(/* Complex */ ae_matrix* xe,
     ae_int_t n,
     ae_int_t m,
     double threshold,
     ae_int_t info,
     /* Complex */ ae_matrix* xs,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_bool result;


    result = ae_true;
    if( info<=0 )
    {
        result = ae_false;
        return result;
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            result = result&&ae_fp_less_eq(ae_c_abs(ae_c_sub(xe->ptr.pp_complex[i][j],xs->ptr.pp_complex[i][j]), _state),threshold);
        }
    }
    return result;
}


/*************************************************************************
Checks whether solver results are correct solution.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_cmatrixchecksolution(/* Complex */ ae_matrix* xe,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     densesolverreport* rep,
     /* Complex */ ae_vector* xs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xsm;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xsm, 0, 0, DT_COMPLEX, _state);

    ae_matrix_set_length(&xsm, n, 1, _state);
    ae_v_cmove(&xsm.ptr.pp_complex[0][0], xsm.stride, &xs->ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
    result = testdensesolverunit_cmatrixchecksolutionm(xe, n, 1, threshold, info, rep, &xsm, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Checks whether solver results are correct solution.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_cmatrixchecksolutionfast(/* Complex */ ae_matrix* xe,
     ae_int_t n,
     double threshold,
     ae_int_t info,
     /* Complex */ ae_vector* xs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xsm;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xsm, 0, 0, DT_COMPLEX, _state);

    ae_matrix_set_length(&xsm, n, 1, _state);
    ae_v_cmove(&xsm.ptr.pp_complex[0][0], xsm.stride, &xs->ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
    result = testdensesolverunit_cmatrixchecksolutionmfast(xe, n, 1, threshold, info, &xsm, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Checks whether solver results indicate singular matrix.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_cmatrixchecksingularm(ae_int_t n,
     ae_int_t m,
     ae_int_t info,
     densesolverreport* rep,
     /* Complex */ ae_matrix* xs,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_bool result;


    result = ae_true;
    if( info!=-3&&info!=1 )
    {
        result = ae_false;
        return result;
    }
    result = result&&!(ae_fp_less(rep->r1,(double)(0))||ae_fp_greater(rep->r1,1000*ae_machineepsilon));
    result = result&&!(ae_fp_less(rep->rinf,(double)(0))||ae_fp_greater(rep->rinf,1000*ae_machineepsilon));
    if( info==-3 )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                result = result&&ae_c_eq_d(xs->ptr.pp_complex[i][j],(double)(0));
            }
        }
    }
    return result;
}


/*************************************************************************
Checks whether solver results indicate singular matrix.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_cmatrixchecksingularmfast(ae_int_t n,
     ae_int_t m,
     ae_int_t info,
     /* Complex */ ae_matrix* xs,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_bool result;


    result = ae_true;
    if( info!=-3 )
    {
        result = ae_false;
        return result;
    }
    if( info==-3 )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                result = result&&ae_c_eq_d(xs->ptr.pp_complex[i][j],(double)(0));
            }
        }
    }
    return result;
}


/*************************************************************************
Checks whether solver results indicate singular matrix.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_cmatrixchecksingular(ae_int_t n,
     ae_int_t info,
     densesolverreport* rep,
     /* Complex */ ae_vector* xs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xsm;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xsm, 0, 0, DT_COMPLEX, _state);

    ae_matrix_set_length(&xsm, n, 1, _state);
    ae_v_cmove(&xsm.ptr.pp_complex[0][0], xsm.stride, &xs->ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
    result = testdensesolverunit_cmatrixchecksingularm(n, 1, info, rep, &xsm, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Checks whether solver results indicate singular matrix.
Returns True on success.
*************************************************************************/
static ae_bool testdensesolverunit_cmatrixchecksingularfast(ae_int_t n,
     ae_int_t info,
     /* Complex */ ae_vector* xs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xsm;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xsm, 0, 0, DT_COMPLEX, _state);

    ae_matrix_set_length(&xsm, n, 1, _state);
    ae_v_cmove(&xsm.ptr.pp_complex[0][0], xsm.stride, &xs->ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
    result = testdensesolverunit_cmatrixchecksingularmfast(n, 1, info, &xsm, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Copy
*************************************************************************/
static void testdensesolverunit_rmatrixmakeacopy(/* Real    */ ae_matrix* a,
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
static void testdensesolverunit_cmatrixmakeacopy(/* Complex */ ae_matrix* a,
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
Drops upper or lower half of the matrix - fills it by special pattern
which may be used later to ensure that this part wasn't changed
*************************************************************************/
static void testdensesolverunit_rmatrixdrophalf(/* Real    */ ae_matrix* a,
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
static void testdensesolverunit_cmatrixdrophalf(/* Complex */ ae_matrix* a,
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
Real test
*************************************************************************/
static void testdensesolverunit_testrsolver(ae_int_t maxn,
     ae_int_t maxm,
     ae_int_t passcount,
     double threshold,
     ae_bool* rerrors,
     ae_bool* rfserrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix lua;
    ae_matrix atmp;
    ae_vector p;
    ae_matrix xe;
    ae_matrix b;
    ae_vector bv;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t n;
    ae_int_t m;
    ae_int_t pass;
    ae_int_t taskkind;
    double v;
    double verr;
    ae_int_t info;
    densesolverreport rep;
    densesolverlsreport repls;
    ae_matrix x;
    ae_vector xv;
    ae_vector y;
    ae_vector tx;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&lua, 0, 0, DT_REAL, _state);
    ae_matrix_init(&atmp, 0, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_matrix_init(&xe, 0, 0, DT_REAL, _state);
    ae_matrix_init(&b, 0, 0, DT_REAL, _state);
    ae_vector_init(&bv, 0, DT_REAL, _state);
    _densesolverreport_init(&rep, _state);
    _densesolverlsreport_init(&repls, _state);
    ae_matrix_init(&x, 0, 0, DT_REAL, _state);
    ae_vector_init(&xv, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&tx, 0, DT_REAL, _state);

    
    /*
     * General square matrices:
     * * test general solvers
     * * test least squares solver
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            for(m=1; m<=maxm; m++)
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
                testdensesolverunit_rmatrixmakeacopy(&a, n, n, &lua, _state);
                rmatrixlu(&lua, n, n, &p, _state);
                ae_matrix_set_length(&xe, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        xe.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    }
                }
                ae_matrix_set_length(&b, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xe.ptr.pp_double[0][j], xe.stride, ae_v_len(0,n-1));
                        b.ptr.pp_double[i][j] = v;
                    }
                }
                
                /*
                 * Test solvers
                 */
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_unset2d(&x, _state);
                rmatrixsolvem(&a, n, &b, m, ae_fp_greater(ae_randomreal(_state),0.5), &info, &rep, &x, _state);
                *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksolutionm(&xe, n, m, threshold, info, &rep, &x, _state);
                info = 0;
                ae_matrix_set_length(&x, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x.ptr.pp_double[i][j] = b.ptr.pp_double[i][j];
                    }
                }
                rmatrixsolvemfast(&a, n, &x, m, &info, _state);
                *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksolutionmfast(&xe, n, m, threshold, info, &x, _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_unset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                rmatrixsolve(&a, n, &bv, &info, &rep, &xv, _state);
                *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksolution(&xe, n, threshold, info, &rep, &xv, _state);
                info = 0;
                ae_vector_set_length(&bv, n, _state);
                ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                rmatrixsolvefast(&a, n, &bv, &info, _state);
                *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksolutionfast(&xe, n, threshold, info, &bv, _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_unset2d(&x, _state);
                rmatrixlusolvem(&lua, &p, n, &b, m, &info, &rep, &x, _state);
                *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksolutionm(&xe, n, m, threshold, info, &rep, &x, _state);
                info = 0;
                ae_matrix_set_length(&x, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x.ptr.pp_double[i][j] = b.ptr.pp_double[i][j];
                    }
                }
                rmatrixlusolvemfast(&lua, &p, n, &x, m, &info, _state);
                seterrorflag(rerrors, !testdensesolverunit_rmatrixchecksolutionmfast(&xe, n, m, threshold, info, &x, _state), _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_unset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                rmatrixlusolve(&lua, &p, n, &bv, &info, &rep, &xv, _state);
                *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksolution(&xe, n, threshold, info, &rep, &xv, _state);
                info = 0;
                ae_vector_set_length(&bv, n, _state);
                ae_v_move(&xv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                rmatrixlusolvefast(&lua, &p, n, &xv, &info, _state);
                seterrorflag(rerrors, !testdensesolverunit_rmatrixchecksolutionfast(&xe, n, threshold, info, &xv, _state), _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_unset2d(&x, _state);
                rmatrixmixedsolvem(&a, &lua, &p, n, &b, m, &info, &rep, &x, _state);
                *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksolutionm(&xe, n, m, threshold, info, &rep, &x, _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_unset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                rmatrixmixedsolve(&a, &lua, &p, n, &bv, &info, &rep, &xv, _state);
                *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksolution(&xe, n, threshold, info, &rep, &xv, _state);
                
                /*
                 * Test DenseSolverRLS():
                 * * test on original system A*x = b
                 * * test on overdetermined system with the same solution: (A' A')'*x = (b' b')'
                 * * test on underdetermined system with the same solution: (A 0 0 0 ) * z = b
                 */
                info = 0;
                testdensesolverunit_unsetlsrep(&repls, _state);
                testdensesolverunit_unset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                rmatrixsolvels(&a, n, n, &bv, 0.0, &info, &repls, &xv, _state);
                if( info<=0 )
                {
                    *rerrors = ae_true;
                }
                else
                {
                    *rerrors = (*rerrors||ae_fp_less(repls.r2,100*ae_machineepsilon))||ae_fp_greater(repls.r2,1+1000*ae_machineepsilon);
                    *rerrors = (*rerrors||repls.n!=n)||repls.k!=0;
                    for(i=0; i<=n-1; i++)
                    {
                        *rerrors = *rerrors||ae_fp_greater(ae_fabs(xe.ptr.pp_double[i][0]-xv.ptr.p_double[i], _state),threshold);
                    }
                }
                info = 0;
                testdensesolverunit_unsetlsrep(&repls, _state);
                testdensesolverunit_unset1d(&xv, _state);
                ae_vector_set_length(&bv, 2*n, _state);
                ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                ae_v_move(&bv.ptr.p_double[n], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(n,2*n-1));
                ae_matrix_set_length(&atmp, 2*n, n, _state);
                copymatrix(&a, 0, n-1, 0, n-1, &atmp, 0, n-1, 0, n-1, _state);
                copymatrix(&a, 0, n-1, 0, n-1, &atmp, n, 2*n-1, 0, n-1, _state);
                rmatrixsolvels(&atmp, 2*n, n, &bv, 0.0, &info, &repls, &xv, _state);
                if( info<=0 )
                {
                    *rerrors = ae_true;
                }
                else
                {
                    *rerrors = (*rerrors||ae_fp_less(repls.r2,100*ae_machineepsilon))||ae_fp_greater(repls.r2,1+1000*ae_machineepsilon);
                    *rerrors = (*rerrors||repls.n!=n)||repls.k!=0;
                    for(i=0; i<=n-1; i++)
                    {
                        *rerrors = *rerrors||ae_fp_greater(ae_fabs(xe.ptr.pp_double[i][0]-xv.ptr.p_double[i], _state),threshold);
                    }
                }
                info = 0;
                testdensesolverunit_unsetlsrep(&repls, _state);
                testdensesolverunit_unset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                ae_matrix_set_length(&atmp, n, 2*n, _state);
                copymatrix(&a, 0, n-1, 0, n-1, &atmp, 0, n-1, 0, n-1, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=n; j<=2*n-1; j++)
                    {
                        atmp.ptr.pp_double[i][j] = (double)(0);
                    }
                }
                rmatrixsolvels(&atmp, n, 2*n, &bv, 0.0, &info, &repls, &xv, _state);
                if( info<=0 )
                {
                    *rerrors = ae_true;
                }
                else
                {
                    *rerrors = *rerrors||ae_fp_neq(repls.r2,(double)(0));
                    *rerrors = (*rerrors||repls.n!=2*n)||repls.k!=n;
                    for(i=0; i<=n-1; i++)
                    {
                        *rerrors = *rerrors||ae_fp_greater(ae_fabs(xe.ptr.pp_double[i][0]-xv.ptr.p_double[i], _state),threshold);
                    }
                    for(i=n; i<=2*n-1; i++)
                    {
                        *rerrors = *rerrors||ae_fp_greater(ae_fabs(xv.ptr.p_double[i], _state),threshold);
                    }
                }
                
                /*
                 * ********************************************************
                 * EXACTLY SINGULAR MATRICES
                 * ability to detect singularity is tested
                 * ********************************************************
                 *
                 * 1. generate different types of singular matrices:
                 *    * zero (TaskKind=0)
                 *    * with zero columns (TaskKind=1)
                 *    * with zero rows (TaskKind=2)
                 *    * with equal rows/columns (TaskKind=2 or 3)
                 * 2. generate random solution vector xe
                 * 3. generate right part b=A*xe
                 * 4. test different methods
                 */
                for(taskkind=0; taskkind<=4; taskkind++)
                {
                    testdensesolverunit_unset2d(&a, _state);
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
                    ae_matrix_set_length(&xe, n, m, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            xe.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                    ae_matrix_set_length(&b, n, m, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xe.ptr.pp_double[0][j], xe.stride, ae_v_len(0,n-1));
                            b.ptr.pp_double[i][j] = v;
                        }
                    }
                    testdensesolverunit_rmatrixmakeacopy(&a, n, n, &lua, _state);
                    rmatrixlu(&lua, n, n, &p, _state);
                    
                    /*
                     * Test RMatrixSolveM()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_unset2d(&x, _state);
                    rmatrixsolvem(&a, n, &b, m, ae_fp_greater(ae_randomreal(_state),0.5), &info, &rep, &x, _state);
                    *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksingularm(n, m, info, &rep, &x, _state);
                    
                    /*
                     * Test RMatrixSolveMFast(); performed only for matrices
                     * with zero rows or columns
                     */
                    if( (taskkind==0||taskkind==1)||taskkind==2 )
                    {
                        info = 0;
                        ae_matrix_set_length(&x, n, m, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                x.ptr.pp_double[i][j] = b.ptr.pp_double[i][j];
                            }
                        }
                        rmatrixsolvemfast(&a, n, &x, m, &info, _state);
                        *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksingularmfast(n, m, info, &x, _state);
                    }
                    
                    /*
                     * Test RMatrixSolve()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_unset2d(&x, _state);
                    ae_vector_set_length(&bv, n, _state);
                    ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                    rmatrixsolve(&a, n, &bv, &info, &rep, &xv, _state);
                    *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksingular(n, info, &rep, &xv, _state);
                    
                    /*
                     * Test RMatrixSolveFast(); performed only for matrices
                     * with zero rows or columns
                     */
                    if( (taskkind==0||taskkind==1)||taskkind==2 )
                    {
                        info = 0;
                        ae_vector_set_length(&bv, n, _state);
                        ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                        rmatrixsolvefast(&a, n, &bv, &info, _state);
                        seterrorflag(rerrors, !testdensesolverunit_rmatrixchecksingularfast(n, info, &bv, _state), _state);
                    }
                    
                    /*
                     * Test RMatrixLUSolveM()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_unset2d(&x, _state);
                    rmatrixlusolvem(&lua, &p, n, &b, m, &info, &rep, &x, _state);
                    *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksingularm(n, m, info, &rep, &x, _state);
                    
                    /*
                     * Test RMatrixLUSolveMFast(); performed only for matrices
                     * with zero rows or columns
                     */
                    if( (taskkind==0||taskkind==1)||taskkind==2 )
                    {
                        info = 0;
                        ae_matrix_set_length(&x, n, m, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                x.ptr.pp_double[i][j] = b.ptr.pp_double[i][j];
                            }
                        }
                        rmatrixlusolvemfast(&lua, &p, n, &x, m, &info, _state);
                        seterrorflag(rerrors, !testdensesolverunit_rmatrixchecksingularmfast(n, m, info, &x, _state), _state);
                    }
                    
                    /*
                     * Test RMatrixLUSolve()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_unset2d(&x, _state);
                    ae_vector_set_length(&bv, n, _state);
                    ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                    rmatrixlusolve(&lua, &p, n, &bv, &info, &rep, &xv, _state);
                    *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksingular(n, info, &rep, &xv, _state);
                    
                    /*
                     * Test RMatrixLUSolveFast(); performed only for matrices
                     * with zero rows or columns
                     */
                    if( (taskkind==0||taskkind==1)||taskkind==2 )
                    {
                        info = 0;
                        ae_vector_set_length(&bv, n, _state);
                        ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                        rmatrixlusolvefast(&lua, &p, n, &bv, &info, _state);
                        seterrorflag(rerrors, !testdensesolverunit_rmatrixchecksingularfast(n, info, &bv, _state), _state);
                    }
                    
                    /*
                     * Test RMatrixMixedSolveM()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_unset2d(&x, _state);
                    rmatrixmixedsolvem(&a, &lua, &p, n, &b, m, &info, &rep, &x, _state);
                    *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksingularm(n, m, info, &rep, &x, _state);
                    
                    /*
                     * Test RMatrixMixedSolve()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_unset2d(&x, _state);
                    ae_vector_set_length(&bv, n, _state);
                    ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                    rmatrixmixedsolve(&a, &lua, &p, n, &bv, &info, &rep, &xv, _state);
                    *rerrors = *rerrors||!testdensesolverunit_rmatrixchecksingular(n, info, &rep, &xv, _state);
                }
            }
        }
    }
    
    /*
     * test iterative improvement
     */
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Test iterative improvement matrices
         *
         * A matrix/right part are constructed such that both matrix
         * and solution components are within (-1,+1). Such matrix/right part
         * have nice properties - system can be solved using iterative
         * improvement with |A*x-b| about several ulps of max(1,|b|).
         */
        n = 100;
        ae_matrix_set_length(&a, n, n, _state);
        ae_matrix_set_length(&b, n, 1, _state);
        ae_vector_set_length(&bv, n, _state);
        ae_vector_set_length(&tx, n, _state);
        ae_vector_set_length(&xv, n, _state);
        ae_vector_set_length(&y, n, _state);
        for(i=0; i<=n-1; i++)
        {
            xv.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            ae_v_move(&y.ptr.p_double[0], 1, &a.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
            xdot(&y, &xv, n, &tx, &v, &verr, _state);
            bv.ptr.p_double[i] = v;
        }
        ae_v_move(&b.ptr.pp_double[0][0], b.stride, &bv.ptr.p_double[0], 1, ae_v_len(0,n-1));
        
        /*
         * Test RMatrixSolveM()
         */
        testdensesolverunit_unset2d(&x, _state);
        rmatrixsolvem(&a, n, &b, 1, ae_true, &info, &rep, &x, _state);
        if( info<=0 )
        {
            *rfserrors = ae_true;
        }
        else
        {
            ae_vector_set_length(&xv, n, _state);
            ae_v_move(&xv.ptr.p_double[0], 1, &x.ptr.pp_double[0][0], x.stride, ae_v_len(0,n-1));
            for(i=0; i<=n-1; i++)
            {
                ae_v_move(&y.ptr.p_double[0], 1, &a.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                xdot(&y, &xv, n, &tx, &v, &verr, _state);
                *rfserrors = *rfserrors||ae_fp_greater(ae_fabs(v-b.ptr.pp_double[i][0], _state),8*ae_machineepsilon*ae_maxreal((double)(1), ae_fabs(b.ptr.pp_double[i][0], _state), _state));
            }
        }
        
        /*
         * Test RMatrixSolve()
         */
        testdensesolverunit_unset1d(&xv, _state);
        rmatrixsolve(&a, n, &bv, &info, &rep, &xv, _state);
        if( info<=0 )
        {
            *rfserrors = ae_true;
        }
        else
        {
            for(i=0; i<=n-1; i++)
            {
                ae_v_move(&y.ptr.p_double[0], 1, &a.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                xdot(&y, &xv, n, &tx, &v, &verr, _state);
                *rfserrors = *rfserrors||ae_fp_greater(ae_fabs(v-bv.ptr.p_double[i], _state),8*ae_machineepsilon*ae_maxreal((double)(1), ae_fabs(bv.ptr.p_double[i], _state), _state));
            }
        }
        
        /*
         * Test LS-solver on the same matrix
         */
        rmatrixsolvels(&a, n, n, &bv, 0.0, &info, &repls, &xv, _state);
        if( info<=0 )
        {
            *rfserrors = ae_true;
        }
        else
        {
            for(i=0; i<=n-1; i++)
            {
                ae_v_move(&y.ptr.p_double[0], 1, &a.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                xdot(&y, &xv, n, &tx, &v, &verr, _state);
                *rfserrors = *rfserrors||ae_fp_greater(ae_fabs(v-bv.ptr.p_double[i], _state),8*ae_machineepsilon*ae_maxreal((double)(1), ae_fabs(bv.ptr.p_double[i], _state), _state));
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
SPD test
*************************************************************************/
static void testdensesolverunit_testspdsolver(ae_int_t maxn,
     ae_int_t maxm,
     ae_int_t passcount,
     double threshold,
     ae_bool* spderrors,
     ae_bool* rfserrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix cha;
    ae_matrix atmp;
    ae_vector p;
    ae_matrix xe;
    ae_matrix b;
    ae_vector bv;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t n;
    ae_int_t m;
    ae_int_t pass;
    ae_int_t taskkind;
    double v;
    ae_bool isupper;
    ae_int_t info;
    densesolverreport rep;
    densesolverlsreport repls;
    ae_matrix x;
    ae_vector xv;
    ae_vector y;
    ae_vector tx;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cha, 0, 0, DT_REAL, _state);
    ae_matrix_init(&atmp, 0, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_matrix_init(&xe, 0, 0, DT_REAL, _state);
    ae_matrix_init(&b, 0, 0, DT_REAL, _state);
    ae_vector_init(&bv, 0, DT_REAL, _state);
    _densesolverreport_init(&rep, _state);
    _densesolverlsreport_init(&repls, _state);
    ae_matrix_init(&x, 0, 0, DT_REAL, _state);
    ae_vector_init(&xv, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&tx, 0, DT_REAL, _state);

    
    /*
     * General square matrices:
     * * test general solvers
     * * test least squares solver
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            for(m=1; m<=maxm; m++)
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
                isupper = ae_fp_greater(ae_randomreal(_state),0.5);
                spdmatrixrndcond(n, (double)(1000), &a, _state);
                testdensesolverunit_rmatrixmakeacopy(&a, n, n, &cha, _state);
                if( !spdmatrixcholesky(&cha, n, isupper, _state) )
                {
                    *spderrors = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                ae_matrix_set_length(&xe, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        xe.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    }
                }
                ae_matrix_set_length(&b, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xe.ptr.pp_double[0][j], xe.stride, ae_v_len(0,n-1));
                        b.ptr.pp_double[i][j] = v;
                    }
                }
                testdensesolverunit_rmatrixdrophalf(&a, n, isupper, _state);
                testdensesolverunit_rmatrixdrophalf(&cha, n, isupper, _state);
                
                /*
                 * Test solvers
                 */
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_unset2d(&x, _state);
                spdmatrixsolvem(&a, n, isupper, &b, m, &info, &rep, &x, _state);
                *spderrors = *spderrors||!testdensesolverunit_rmatrixchecksolutionm(&xe, n, m, threshold, info, &rep, &x, _state);
                info = 0;
                ae_matrix_set_length(&x, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x.ptr.pp_double[i][j] = b.ptr.pp_double[i][j];
                    }
                }
                spdmatrixsolvemfast(&a, n, isupper, &x, m, &info, _state);
                seterrorflag(spderrors, !testdensesolverunit_rmatrixchecksolutionmfast(&xe, n, m, threshold, info, &x, _state), _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_unset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                spdmatrixsolve(&a, n, isupper, &bv, &info, &rep, &xv, _state);
                *spderrors = *spderrors||!testdensesolverunit_rmatrixchecksolution(&xe, n, threshold, info, &rep, &xv, _state);
                info = 0;
                ae_vector_set_length(&bv, n, _state);
                ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                spdmatrixsolvefast(&a, n, isupper, &bv, &info, _state);
                seterrorflag(spderrors, !testdensesolverunit_rmatrixchecksolutionfast(&xe, n, threshold, info, &bv, _state), _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_unset2d(&x, _state);
                spdmatrixcholeskysolvem(&cha, n, isupper, &b, m, &info, &rep, &x, _state);
                *spderrors = *spderrors||!testdensesolverunit_rmatrixchecksolutionm(&xe, n, m, threshold, info, &rep, &x, _state);
                info = 0;
                ae_matrix_set_length(&x, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x.ptr.pp_double[i][j] = b.ptr.pp_double[i][j];
                    }
                }
                spdmatrixcholeskysolvemfast(&cha, n, isupper, &x, m, &info, _state);
                seterrorflag(spderrors, !testdensesolverunit_rmatrixchecksolutionmfast(&xe, n, m, threshold, info, &x, _state), _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_unset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                spdmatrixcholeskysolve(&cha, n, isupper, &bv, &info, &rep, &xv, _state);
                *spderrors = *spderrors||!testdensesolverunit_rmatrixchecksolution(&xe, n, threshold, info, &rep, &xv, _state);
                info = 0;
                ae_vector_set_length(&bv, n, _state);
                ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                spdmatrixcholeskysolvefast(&cha, n, isupper, &bv, &info, _state);
                seterrorflag(spderrors, !testdensesolverunit_rmatrixchecksolutionfast(&xe, n, threshold, info, &bv, _state), _state);
                
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
                 * 2. generate random solution vector xe
                 * 3. generate right part b=A*xe
                 * 4. test different methods
                 */
                for(taskkind=0; taskkind<=3; taskkind++)
                {
                    testdensesolverunit_unset2d(&a, _state);
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
                            for(j=i; j<=n-1; j++)
                            {
                                a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                                a.ptr.pp_double[j][i] = a.ptr.pp_double[i][j];
                            }
                        }
                        k = ae_randominteger(n, _state);
                        ae_v_muld(&a.ptr.pp_double[0][k], a.stride, ae_v_len(0,n-1), 0);
                        ae_v_muld(&a.ptr.pp_double[k][0], 1, ae_v_len(0,n-1), 0);
                    }
                    if( taskkind==2 )
                    {
                        
                        /*
                         * there is zero row
                         */
                        ae_matrix_set_length(&a, n, n, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=i; j<=n-1; j++)
                            {
                                a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                                a.ptr.pp_double[j][i] = a.ptr.pp_double[i][j];
                            }
                        }
                        k = ae_randominteger(n, _state);
                        ae_v_muld(&a.ptr.pp_double[k][0], 1, ae_v_len(0,n-1), 0);
                        ae_v_muld(&a.ptr.pp_double[0][k], a.stride, ae_v_len(0,n-1), 0);
                    }
                    if( taskkind==3 )
                    {
                        
                        /*
                         * equal columns/rows
                         */
                        if( n<2 )
                        {
                            continue;
                        }
                        ae_matrix_set_length(&a, n, n, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=i; j<=n-1; j++)
                            {
                                a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                                a.ptr.pp_double[j][i] = a.ptr.pp_double[i][j];
                            }
                        }
                        k = 1+ae_randominteger(n-1, _state);
                        ae_v_move(&a.ptr.pp_double[0][0], a.stride, &a.ptr.pp_double[0][k], a.stride, ae_v_len(0,n-1));
                        ae_v_move(&a.ptr.pp_double[0][0], 1, &a.ptr.pp_double[k][0], 1, ae_v_len(0,n-1));
                    }
                    ae_matrix_set_length(&xe, n, m, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            xe.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                    ae_matrix_set_length(&b, n, m, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xe.ptr.pp_double[0][j], xe.stride, ae_v_len(0,n-1));
                            b.ptr.pp_double[i][j] = v;
                        }
                    }
                    testdensesolverunit_rmatrixmakeacopy(&a, n, n, &cha, _state);
                    testdensesolverunit_rmatrixdrophalf(&a, n, isupper, _state);
                    testdensesolverunit_rmatrixdrophalf(&cha, n, isupper, _state);
                    
                    /*
                     * Test SPDMatrixSolveM()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_unset2d(&x, _state);
                    spdmatrixsolvem(&a, n, isupper, &b, m, &info, &rep, &x, _state);
                    *spderrors = *spderrors||!testdensesolverunit_rmatrixchecksingularm(n, m, info, &rep, &x, _state);
                    
                    /*
                     * Test SPDMatrixSolveMFast()
                     */
                    if( (taskkind==0||taskkind==1)||taskkind==2 )
                    {
                        info = 0;
                        ae_matrix_set_length(&x, n, m, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                x.ptr.pp_double[i][j] = b.ptr.pp_double[i][j];
                            }
                        }
                        spdmatrixsolvemfast(&a, n, isupper, &x, m, &info, _state);
                        seterrorflag(spderrors, !testdensesolverunit_rmatrixchecksingularmfast(n, m, info, &x, _state), _state);
                    }
                    
                    /*
                     * Test SPDMatrixSolve()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_unset2d(&x, _state);
                    ae_vector_set_length(&bv, n, _state);
                    ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                    spdmatrixsolve(&a, n, isupper, &bv, &info, &rep, &xv, _state);
                    *spderrors = *spderrors||!testdensesolverunit_rmatrixchecksingular(n, info, &rep, &xv, _state);
                    
                    /*
                     * Test SPDMatrixSolveFast()
                     */
                    info = 0;
                    ae_vector_set_length(&bv, n, _state);
                    ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                    spdmatrixsolvefast(&a, n, isupper, &bv, &info, _state);
                    seterrorflag(spderrors, !testdensesolverunit_rmatrixchecksingular(n, info, &rep, &bv, _state), _state);
                    
                    /*
                     * 'equal columns/rows' are degenerate, but
                     * Cholesky matrix with equal columns/rows IS NOT degenerate,
                     * so it is not used for testing purposes.
                     */
                    if( taskkind!=3 )
                    {
                        
                        /*
                         * Test SPDMatrixLUSolveM() (and fast version)
                         */
                        info = 0;
                        testdensesolverunit_unsetrep(&rep, _state);
                        testdensesolverunit_unset2d(&x, _state);
                        spdmatrixcholeskysolvem(&cha, n, isupper, &b, m, &info, &rep, &x, _state);
                        *spderrors = *spderrors||!testdensesolverunit_rmatrixchecksingularm(n, m, info, &rep, &x, _state);
                        if( (taskkind==0||taskkind==1)||taskkind==2 )
                        {
                            info = 0;
                            ae_matrix_set_length(&x, n, m, _state);
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=m-1; j++)
                                {
                                    x.ptr.pp_double[i][j] = b.ptr.pp_double[i][j];
                                }
                            }
                            spdmatrixcholeskysolvemfast(&a, n, isupper, &x, m, &info, _state);
                            seterrorflag(spderrors, !testdensesolverunit_rmatrixchecksingularmfast(n, m, info, &x, _state), _state);
                        }
                        
                        /*
                         * Test SPDMatrixLUSolve()
                         */
                        info = 0;
                        testdensesolverunit_unsetrep(&rep, _state);
                        testdensesolverunit_unset2d(&x, _state);
                        ae_vector_set_length(&bv, n, _state);
                        ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                        spdmatrixcholeskysolve(&cha, n, isupper, &bv, &info, &rep, &xv, _state);
                        *spderrors = *spderrors||!testdensesolverunit_rmatrixchecksingular(n, info, &rep, &xv, _state);
                        if( (taskkind==0||taskkind==1)||taskkind==2 )
                        {
                            info = 0;
                            ae_vector_set_length(&bv, n, _state);
                            ae_v_move(&bv.ptr.p_double[0], 1, &b.ptr.pp_double[0][0], b.stride, ae_v_len(0,n-1));
                            spdmatrixcholeskysolvefast(&a, n, isupper, &bv, &info, _state);
                            seterrorflag(spderrors, !testdensesolverunit_rmatrixchecksingularfast(n, info, &bv, _state), _state);
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
static void testdensesolverunit_testcsolver(ae_int_t maxn,
     ae_int_t maxm,
     ae_int_t passcount,
     double threshold,
     ae_bool* cerrors,
     ae_bool* rfserrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix lua;
    ae_matrix atmp;
    ae_vector p;
    ae_matrix xe;
    ae_matrix b;
    ae_vector bv;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t n;
    ae_int_t m;
    ae_int_t pass;
    ae_int_t taskkind;
    double verr;
    ae_complex v;
    ae_int_t info;
    densesolverreport rep;
    densesolverlsreport repls;
    ae_matrix x;
    ae_vector xv;
    ae_vector y;
    ae_vector tx;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&lua, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&atmp, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_matrix_init(&xe, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&b, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&bv, 0, DT_COMPLEX, _state);
    _densesolverreport_init(&rep, _state);
    _densesolverlsreport_init(&repls, _state);
    ae_matrix_init(&x, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&xv, 0, DT_COMPLEX, _state);
    ae_vector_init(&y, 0, DT_COMPLEX, _state);
    ae_vector_init(&tx, 0, DT_REAL, _state);

    
    /*
     * General square matrices:
     * * test general solvers
     * * test least squares solver
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            for(m=1; m<=maxm; m++)
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
                testdensesolverunit_cmatrixmakeacopy(&a, n, n, &lua, _state);
                cmatrixlu(&lua, n, n, &p, _state);
                ae_matrix_set_length(&xe, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        xe.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                        xe.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                    }
                }
                ae_matrix_set_length(&b, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        v = ae_v_cdotproduct(&a.ptr.pp_complex[i][0], 1, "N", &xe.ptr.pp_complex[0][j], xe.stride, "N", ae_v_len(0,n-1));
                        b.ptr.pp_complex[i][j] = v;
                    }
                }
                
                /*
                 * Test solvers
                 */
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_cunset2d(&x, _state);
                cmatrixsolvem(&a, n, &b, m, ae_fp_greater(ae_randomreal(_state),0.5), &info, &rep, &x, _state);
                *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksolutionm(&xe, n, m, threshold, info, &rep, &x, _state);
                info = 0;
                ae_matrix_set_length(&x, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x.ptr.pp_complex[i][j] = b.ptr.pp_complex[i][j];
                    }
                }
                cmatrixsolvemfast(&a, n, &x, m, &info, _state);
                *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksolutionmfast(&xe, n, m, threshold, info, &x, _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_cunset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                cmatrixsolve(&a, n, &bv, &info, &rep, &xv, _state);
                *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksolution(&xe, n, threshold, info, &rep, &xv, _state);
                info = 0;
                ae_vector_set_length(&bv, n, _state);
                ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                cmatrixsolvefast(&a, n, &bv, &info, _state);
                *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksolutionfast(&xe, n, threshold, info, &bv, _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_cunset2d(&x, _state);
                cmatrixlusolvem(&lua, &p, n, &b, m, &info, &rep, &x, _state);
                *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksolutionm(&xe, n, m, threshold, info, &rep, &x, _state);
                info = 0;
                ae_matrix_set_length(&x, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x.ptr.pp_complex[i][j] = b.ptr.pp_complex[i][j];
                    }
                }
                cmatrixlusolvemfast(&lua, &p, n, &x, m, &info, _state);
                seterrorflag(cerrors, !testdensesolverunit_cmatrixchecksolutionmfast(&xe, n, m, threshold, info, &x, _state), _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_cunset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                cmatrixlusolve(&lua, &p, n, &bv, &info, &rep, &xv, _state);
                *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksolution(&xe, n, threshold, info, &rep, &xv, _state);
                info = 0;
                ae_vector_set_length(&bv, n, _state);
                ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                cmatrixlusolvefast(&lua, &p, n, &bv, &info, _state);
                seterrorflag(cerrors, !testdensesolverunit_cmatrixchecksolutionfast(&xe, n, threshold, info, &bv, _state), _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_cunset2d(&x, _state);
                cmatrixmixedsolvem(&a, &lua, &p, n, &b, m, &info, &rep, &x, _state);
                *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksolutionm(&xe, n, m, threshold, info, &rep, &x, _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_cunset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                cmatrixmixedsolve(&a, &lua, &p, n, &bv, &info, &rep, &xv, _state);
                *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksolution(&xe, n, threshold, info, &rep, &xv, _state);
                
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
                 * 2. generate random solution vector xe
                 * 3. generate right part b=A*xe
                 * 4. test different methods
                 */
                for(taskkind=0; taskkind<=4; taskkind++)
                {
                    testdensesolverunit_cunset2d(&a, _state);
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
                    ae_matrix_set_length(&xe, n, m, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            xe.ptr.pp_complex[i][j] = ae_complex_from_d(2*ae_randomreal(_state)-1);
                        }
                    }
                    ae_matrix_set_length(&b, n, m, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            v = ae_v_cdotproduct(&a.ptr.pp_complex[i][0], 1, "N", &xe.ptr.pp_complex[0][j], xe.stride, "N", ae_v_len(0,n-1));
                            b.ptr.pp_complex[i][j] = v;
                        }
                    }
                    testdensesolverunit_cmatrixmakeacopy(&a, n, n, &lua, _state);
                    cmatrixlu(&lua, n, n, &p, _state);
                    
                    /*
                     * Test CMatrixSolveM()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_cunset2d(&x, _state);
                    cmatrixsolvem(&a, n, &b, m, ae_fp_greater(ae_randomreal(_state),0.5), &info, &rep, &x, _state);
                    *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksingularm(n, m, info, &rep, &x, _state);
                    
                    /*
                     * Test CMatrixSolveMFast(); performed only for matrices
                     * with zero rows or columns
                     */
                    if( (taskkind==0||taskkind==1)||taskkind==2 )
                    {
                        info = 0;
                        ae_matrix_set_length(&x, n, m, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                x.ptr.pp_complex[i][j] = b.ptr.pp_complex[i][j];
                            }
                        }
                        cmatrixsolvemfast(&a, n, &x, m, &info, _state);
                        seterrorflag(cerrors, !testdensesolverunit_cmatrixchecksingularmfast(n, m, info, &x, _state), _state);
                    }
                    
                    /*
                     * Test CMatrixSolve()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_cunset2d(&x, _state);
                    ae_vector_set_length(&bv, n, _state);
                    ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                    cmatrixsolve(&a, n, &bv, &info, &rep, &xv, _state);
                    *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksingular(n, info, &rep, &xv, _state);
                    if( (taskkind==0||taskkind==1)||taskkind==2 )
                    {
                        info = 0;
                        ae_vector_set_length(&bv, n, _state);
                        ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                        cmatrixsolvefast(&a, n, &bv, &info, _state);
                        seterrorflag(cerrors, !testdensesolverunit_cmatrixchecksingularfast(n, info, &bv, _state), _state);
                    }
                    
                    /*
                     * Test CMatrixLUSolveM()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_cunset2d(&x, _state);
                    cmatrixlusolvem(&lua, &p, n, &b, m, &info, &rep, &x, _state);
                    *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksingularm(n, m, info, &rep, &x, _state);
                    
                    /*
                     * Test CMatrixLUSolveMFast()
                     */
                    if( (taskkind==0||taskkind==1)||taskkind==2 )
                    {
                        info = 0;
                        ae_matrix_set_length(&x, n, m, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                x.ptr.pp_complex[i][j] = b.ptr.pp_complex[i][j];
                            }
                        }
                        cmatrixlusolvemfast(&lua, &p, n, &x, m, &info, _state);
                        seterrorflag(cerrors, !testdensesolverunit_cmatrixchecksingularmfast(n, m, info, &x, _state), _state);
                    }
                    
                    /*
                     * Test CMatrixLUSolve()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_cunset2d(&x, _state);
                    ae_vector_set_length(&bv, n, _state);
                    ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                    cmatrixlusolve(&lua, &p, n, &bv, &info, &rep, &xv, _state);
                    *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksingular(n, info, &rep, &xv, _state);
                    
                    /*
                     * Test CMatrixLUSolveFast()
                     */
                    if( (taskkind==0||taskkind==1)||taskkind==2 )
                    {
                        info = 0;
                        ae_vector_set_length(&bv, n, _state);
                        ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                        cmatrixlusolvefast(&lua, &p, n, &bv, &info, _state);
                        seterrorflag(cerrors, !testdensesolverunit_cmatrixchecksingularfast(n, info, &bv, _state), _state);
                    }
                    
                    /*
                     * Test CMatrixMixedSolveM()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_cunset2d(&x, _state);
                    cmatrixmixedsolvem(&a, &lua, &p, n, &b, m, &info, &rep, &x, _state);
                    *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksingularm(n, m, info, &rep, &x, _state);
                    
                    /*
                     * Test CMatrixMixedSolve()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_cunset2d(&x, _state);
                    ae_vector_set_length(&bv, n, _state);
                    ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                    cmatrixmixedsolve(&a, &lua, &p, n, &bv, &info, &rep, &xv, _state);
                    *cerrors = *cerrors||!testdensesolverunit_cmatrixchecksingular(n, info, &rep, &xv, _state);
                }
            }
        }
    }
    
    /*
     * test iterative improvement
     */
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Test iterative improvement matrices
         *
         * A matrix/right part are constructed such that both matrix
         * and solution components magnitudes are within (-1,+1).
         * Such matrix/right part have nice properties - system can
         * be solved using iterative improvement with |A*x-b| about
         * several ulps of max(1,|b|).
         */
        n = 100;
        ae_matrix_set_length(&a, n, n, _state);
        ae_matrix_set_length(&b, n, 1, _state);
        ae_vector_set_length(&bv, n, _state);
        ae_vector_set_length(&tx, 2*n, _state);
        ae_vector_set_length(&xv, n, _state);
        ae_vector_set_length(&y, n, _state);
        for(i=0; i<=n-1; i++)
        {
            xv.ptr.p_complex[i].x = 2*ae_randomreal(_state)-1;
            xv.ptr.p_complex[i].y = 2*ae_randomreal(_state)-1;
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                a.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
            }
            ae_v_cmove(&y.ptr.p_complex[0], 1, &a.ptr.pp_complex[i][0], 1, "N", ae_v_len(0,n-1));
            xcdot(&y, &xv, n, &tx, &v, &verr, _state);
            bv.ptr.p_complex[i] = v;
        }
        ae_v_cmove(&b.ptr.pp_complex[0][0], b.stride, &bv.ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
        
        /*
         * Test CMatrixSolveM()
         */
        testdensesolverunit_cunset2d(&x, _state);
        cmatrixsolvem(&a, n, &b, 1, ae_true, &info, &rep, &x, _state);
        if( info<=0 )
        {
            *rfserrors = ae_true;
        }
        else
        {
            ae_vector_set_length(&xv, n, _state);
            ae_v_cmove(&xv.ptr.p_complex[0], 1, &x.ptr.pp_complex[0][0], x.stride, "N", ae_v_len(0,n-1));
            for(i=0; i<=n-1; i++)
            {
                ae_v_cmove(&y.ptr.p_complex[0], 1, &a.ptr.pp_complex[i][0], 1, "N", ae_v_len(0,n-1));
                xcdot(&y, &xv, n, &tx, &v, &verr, _state);
                *rfserrors = *rfserrors||ae_fp_greater(ae_c_abs(ae_c_sub(v,b.ptr.pp_complex[i][0]), _state),8*ae_machineepsilon*ae_maxreal((double)(1), ae_c_abs(b.ptr.pp_complex[i][0], _state), _state));
            }
        }
        
        /*
         * Test CMatrixSolve()
         */
        testdensesolverunit_cunset1d(&xv, _state);
        cmatrixsolve(&a, n, &bv, &info, &rep, &xv, _state);
        if( info<=0 )
        {
            *rfserrors = ae_true;
        }
        else
        {
            for(i=0; i<=n-1; i++)
            {
                ae_v_cmove(&y.ptr.p_complex[0], 1, &a.ptr.pp_complex[i][0], 1, "N", ae_v_len(0,n-1));
                xcdot(&y, &xv, n, &tx, &v, &verr, _state);
                *rfserrors = *rfserrors||ae_fp_greater(ae_c_abs(ae_c_sub(v,bv.ptr.p_complex[i]), _state),8*ae_machineepsilon*ae_maxreal((double)(1), ae_c_abs(bv.ptr.p_complex[i], _state), _state));
            }
        }
        
        /*
         * TODO: Test LS-solver on the same matrix
         */
    }
    ae_frame_leave(_state);
}


/*************************************************************************
HPD test
*************************************************************************/
static void testdensesolverunit_testhpdsolver(ae_int_t maxn,
     ae_int_t maxm,
     ae_int_t passcount,
     double threshold,
     ae_bool* hpderrors,
     ae_bool* rfserrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix cha;
    ae_matrix atmp;
    ae_vector p;
    ae_matrix xe;
    ae_matrix b;
    ae_vector bv;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t n;
    ae_int_t m;
    ae_int_t pass;
    ae_int_t taskkind;
    ae_complex v;
    ae_bool isupper;
    ae_int_t info;
    densesolverreport rep;
    densesolverlsreport repls;
    ae_matrix x;
    ae_vector xv;
    ae_vector y;
    ae_vector tx;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cha, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&atmp, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_matrix_init(&xe, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&b, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&bv, 0, DT_COMPLEX, _state);
    _densesolverreport_init(&rep, _state);
    _densesolverlsreport_init(&repls, _state);
    ae_matrix_init(&x, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&xv, 0, DT_COMPLEX, _state);
    ae_vector_init(&y, 0, DT_COMPLEX, _state);
    ae_vector_init(&tx, 0, DT_COMPLEX, _state);

    
    /*
     * General square matrices:
     * * test general solvers
     * * test least squares solver
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            for(m=1; m<=maxm; m++)
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
                isupper = ae_fp_greater(ae_randomreal(_state),0.5);
                hpdmatrixrndcond(n, (double)(1000), &a, _state);
                testdensesolverunit_cmatrixmakeacopy(&a, n, n, &cha, _state);
                if( !hpdmatrixcholesky(&cha, n, isupper, _state) )
                {
                    *hpderrors = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                ae_matrix_set_length(&xe, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        xe.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                        xe.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                    }
                }
                ae_matrix_set_length(&b, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        v = ae_v_cdotproduct(&a.ptr.pp_complex[i][0], 1, "N", &xe.ptr.pp_complex[0][j], xe.stride, "N", ae_v_len(0,n-1));
                        b.ptr.pp_complex[i][j] = v;
                    }
                }
                testdensesolverunit_cmatrixdrophalf(&a, n, isupper, _state);
                testdensesolverunit_cmatrixdrophalf(&cha, n, isupper, _state);
                
                /*
                 * Test solvers
                 */
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_cunset2d(&x, _state);
                hpdmatrixsolvem(&a, n, isupper, &b, m, &info, &rep, &x, _state);
                *hpderrors = *hpderrors||!testdensesolverunit_cmatrixchecksolutionm(&xe, n, m, threshold, info, &rep, &x, _state);
                info = 0;
                ae_matrix_set_length(&x, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x.ptr.pp_complex[i][j] = b.ptr.pp_complex[i][j];
                    }
                }
                hpdmatrixsolvemfast(&a, n, isupper, &x, m, &info, _state);
                seterrorflag(hpderrors, !testdensesolverunit_cmatrixchecksolutionmfast(&xe, n, m, threshold, info, &x, _state), _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_cunset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                hpdmatrixsolve(&a, n, isupper, &bv, &info, &rep, &xv, _state);
                *hpderrors = *hpderrors||!testdensesolverunit_cmatrixchecksolution(&xe, n, threshold, info, &rep, &xv, _state);
                info = 0;
                ae_vector_set_length(&bv, n, _state);
                ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                hpdmatrixsolvefast(&a, n, isupper, &bv, &info, _state);
                *hpderrors = *hpderrors||!testdensesolverunit_cmatrixchecksolution(&xe, n, threshold, info, &rep, &bv, _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_cunset2d(&x, _state);
                hpdmatrixcholeskysolvem(&cha, n, isupper, &b, m, &info, &rep, &x, _state);
                *hpderrors = *hpderrors||!testdensesolverunit_cmatrixchecksolutionm(&xe, n, m, threshold, info, &rep, &x, _state);
                info = 0;
                ae_matrix_set_length(&x, n, m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x.ptr.pp_complex[i][j] = b.ptr.pp_complex[i][j];
                    }
                }
                hpdmatrixcholeskysolvemfast(&cha, n, isupper, &x, m, &info, _state);
                seterrorflag(hpderrors, !testdensesolverunit_cmatrixchecksolutionmfast(&xe, n, m, threshold, info, &x, _state), _state);
                info = 0;
                testdensesolverunit_unsetrep(&rep, _state);
                testdensesolverunit_cunset1d(&xv, _state);
                ae_vector_set_length(&bv, n, _state);
                ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                hpdmatrixcholeskysolve(&cha, n, isupper, &bv, &info, &rep, &xv, _state);
                *hpderrors = *hpderrors||!testdensesolverunit_cmatrixchecksolution(&xe, n, threshold, info, &rep, &xv, _state);
                info = 0;
                ae_vector_set_length(&bv, n, _state);
                ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                hpdmatrixcholeskysolvefast(&cha, n, isupper, &bv, &info, _state);
                seterrorflag(hpderrors, !testdensesolverunit_cmatrixchecksolutionfast(&xe, n, threshold, info, &bv, _state), _state);
                
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
                 * 2. generate random solution vector xe
                 * 3. generate right part b=A*xe
                 * 4. test different methods
                 */
                for(taskkind=0; taskkind<=3; taskkind++)
                {
                    testdensesolverunit_cunset2d(&a, _state);
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
                            for(j=i; j<=n-1; j++)
                            {
                                a.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                                a.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                                if( i==j )
                                {
                                    a.ptr.pp_complex[i][j].y = (double)(0);
                                }
                                a.ptr.pp_complex[j][i] = a.ptr.pp_complex[i][j];
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
                            for(j=i; j<=n-1; j++)
                            {
                                a.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                                a.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                                if( i==j )
                                {
                                    a.ptr.pp_complex[i][j].y = (double)(0);
                                }
                                a.ptr.pp_complex[j][i] = a.ptr.pp_complex[i][j];
                            }
                        }
                        k = ae_randominteger(n, _state);
                        ae_v_cmuld(&a.ptr.pp_complex[k][0], 1, ae_v_len(0,n-1), 0);
                        ae_v_cmuld(&a.ptr.pp_complex[0][k], a.stride, ae_v_len(0,n-1), 0);
                    }
                    if( taskkind==3 )
                    {
                        
                        /*
                         * equal columns/rows
                         */
                        if( n<2 )
                        {
                            continue;
                        }
                        ae_matrix_set_length(&a, n, n, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=i; j<=n-1; j++)
                            {
                                a.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                                a.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                                if( i==j )
                                {
                                    a.ptr.pp_complex[i][j].y = (double)(0);
                                }
                                a.ptr.pp_complex[j][i] = a.ptr.pp_complex[i][j];
                            }
                        }
                        k = 1+ae_randominteger(n-1, _state);
                        ae_v_cmove(&a.ptr.pp_complex[0][0], a.stride, &a.ptr.pp_complex[0][k], a.stride, "N", ae_v_len(0,n-1));
                        ae_v_cmove(&a.ptr.pp_complex[0][0], 1, &a.ptr.pp_complex[k][0], 1, "N", ae_v_len(0,n-1));
                    }
                    ae_matrix_set_length(&xe, n, m, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            xe.ptr.pp_complex[i][j] = ae_complex_from_d(2*ae_randomreal(_state)-1);
                        }
                    }
                    ae_matrix_set_length(&b, n, m, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            v = ae_v_cdotproduct(&a.ptr.pp_complex[i][0], 1, "N", &xe.ptr.pp_complex[0][j], xe.stride, "N", ae_v_len(0,n-1));
                            b.ptr.pp_complex[i][j] = v;
                        }
                    }
                    testdensesolverunit_cmatrixmakeacopy(&a, n, n, &cha, _state);
                    testdensesolverunit_cmatrixdrophalf(&a, n, isupper, _state);
                    testdensesolverunit_cmatrixdrophalf(&cha, n, isupper, _state);
                    
                    /*
                     * Test SPDMatrixSolveM() (and fast version)
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_cunset2d(&x, _state);
                    hpdmatrixsolvem(&a, n, isupper, &b, m, &info, &rep, &x, _state);
                    *hpderrors = *hpderrors||!testdensesolverunit_cmatrixchecksingularm(n, m, info, &rep, &x, _state);
                    if( (taskkind==0||taskkind==1)||taskkind==2 )
                    {
                        info = 0;
                        ae_matrix_set_length(&x, n, m, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                x.ptr.pp_complex[i][j] = b.ptr.pp_complex[i][j];
                            }
                        }
                        hpdmatrixsolvemfast(&a, n, isupper, &x, m, &info, _state);
                        seterrorflag(hpderrors, !testdensesolverunit_cmatrixchecksingularmfast(n, m, info, &x, _state), _state);
                    }
                    
                    /*
                     * Test SPDMatrixSolve()
                     */
                    info = 0;
                    testdensesolverunit_unsetrep(&rep, _state);
                    testdensesolverunit_cunset2d(&x, _state);
                    ae_vector_set_length(&bv, n, _state);
                    ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                    hpdmatrixsolve(&a, n, isupper, &bv, &info, &rep, &xv, _state);
                    *hpderrors = *hpderrors||!testdensesolverunit_cmatrixchecksingular(n, info, &rep, &xv, _state);
                    if( (taskkind==0||taskkind==1)||taskkind==2 )
                    {
                        info = 0;
                        ae_vector_set_length(&bv, n, _state);
                        ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                        hpdmatrixsolvefast(&a, n, isupper, &bv, &info, _state);
                        seterrorflag(hpderrors, !testdensesolverunit_cmatrixchecksingularfast(n, info, &bv, _state), _state);
                    }
                    
                    /*
                     * 'equal columns/rows' are degenerate, but
                     * Cholesky matrix with equal columns/rows IS NOT degenerate,
                     * so it is not used for testing purposes.
                     */
                    if( taskkind!=3 )
                    {
                        
                        /*
                         * Test SPDMatrixCholeskySolveM()/fast
                         */
                        info = 0;
                        testdensesolverunit_unsetrep(&rep, _state);
                        testdensesolverunit_cunset2d(&x, _state);
                        hpdmatrixcholeskysolvem(&cha, n, isupper, &b, m, &info, &rep, &x, _state);
                        *hpderrors = *hpderrors||!testdensesolverunit_cmatrixchecksingularm(n, m, info, &rep, &x, _state);
                        if( (taskkind==0||taskkind==1)||taskkind==2 )
                        {
                            info = 0;
                            ae_matrix_set_length(&x, n, m, _state);
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=m-1; j++)
                                {
                                    x.ptr.pp_complex[i][j] = b.ptr.pp_complex[i][j];
                                }
                            }
                            hpdmatrixcholeskysolvemfast(&cha, n, isupper, &x, m, &info, _state);
                            seterrorflag(hpderrors, !testdensesolverunit_cmatrixchecksingularmfast(n, m, info, &x, _state), _state);
                        }
                        
                        /*
                         * Test HPDMatrixCholeskySolve() (fast)
                         */
                        info = 0;
                        testdensesolverunit_unsetrep(&rep, _state);
                        testdensesolverunit_cunset2d(&x, _state);
                        ae_vector_set_length(&bv, n, _state);
                        ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                        hpdmatrixcholeskysolve(&cha, n, isupper, &bv, &info, &rep, &xv, _state);
                        *hpderrors = *hpderrors||!testdensesolverunit_cmatrixchecksingular(n, info, &rep, &xv, _state);
                        if( (taskkind==0||taskkind==1)||taskkind==2 )
                        {
                            ae_vector_set_length(&bv, n, _state);
                            ae_v_cmove(&bv.ptr.p_complex[0], 1, &b.ptr.pp_complex[0][0], b.stride, "N", ae_v_len(0,n-1));
                            hpdmatrixcholeskysolvefast(&cha, n, isupper, &bv, &info, _state);
                            seterrorflag(hpderrors, !testdensesolverunit_cmatrixchecksingularfast(n, info, &bv, _state), _state);
                        }
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unsets real matrix
*************************************************************************/
static void testdensesolverunit_unset2d(/* Real    */ ae_matrix* x,
     ae_state *_state)
{


    ae_matrix_set_length(x, 1, 1, _state);
    x->ptr.pp_double[0][0] = 2*ae_randomreal(_state)-1;
}


/*************************************************************************
Unsets real vector
*************************************************************************/
static void testdensesolverunit_unset1d(/* Real    */ ae_vector* x,
     ae_state *_state)
{


    ae_vector_set_length(x, 1, _state);
    x->ptr.p_double[0] = 2*ae_randomreal(_state)-1;
}


/*************************************************************************
Unsets real matrix
*************************************************************************/
static void testdensesolverunit_cunset2d(/* Complex */ ae_matrix* x,
     ae_state *_state)
{


    ae_matrix_set_length(x, 1, 1, _state);
    x->ptr.pp_complex[0][0] = ae_complex_from_d(2*ae_randomreal(_state)-1);
}


/*************************************************************************
Unsets real vector
*************************************************************************/
static void testdensesolverunit_cunset1d(/* Complex */ ae_vector* x,
     ae_state *_state)
{


    ae_vector_set_length(x, 1, _state);
    x->ptr.p_complex[0] = ae_complex_from_d(2*ae_randomreal(_state)-1);
}


/*************************************************************************
Unsets report
*************************************************************************/
static void testdensesolverunit_unsetrep(densesolverreport* r,
     ae_state *_state)
{


    r->r1 = (double)(-1);
    r->rinf = (double)(-1);
}


/*************************************************************************
Unsets report
*************************************************************************/
static void testdensesolverunit_unsetlsrep(densesolverlsreport* r,
     ae_state *_state)
{


    r->r2 = (double)(-1);
    r->n = -1;
    r->k = -1;
    testdensesolverunit_unset2d(&r->cx, _state);
}


/*$ End $*/
