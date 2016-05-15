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
#include "testnormestimatorunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testnormestimator(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    double tol;
    ae_int_t maxmn;
    ae_int_t m;
    ae_int_t n;
    ae_int_t pass;
    ae_int_t passcount;
    ae_matrix a;
    ae_vector rowsizes;
    sparsematrix s;
    double snorm;
    double enorm;
    double enorm2;
    ae_int_t nbetter;
    double sigma;
    ae_int_t i;
    ae_int_t j;
    normestimatorstate e;
    normestimatorstate e2;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&rowsizes, 0, DT_INT, _state);
    _sparsematrix_init(&s, _state);
    _normestimatorstate_init(&e, _state);
    _normestimatorstate_init(&e2, _state);

    tol = 0.01;
    maxmn = 5;
    waserrors = ae_false;
    
    /*
     * First test: algorithm must correctly determine matrix norm
     */
    for(m=1; m<=maxmn; m++)
    {
        for(n=1; n<=maxmn; n++)
        {
            
            /*
             * Create estimator with quite large NStart and NIts.
             * It should guarantee that we converge to the correct solution.
             */
            normestimatorcreate(m, n, 15, 15, &e, _state);
            
            /*
             * Try with zero A first
             */
            sparsecreate(m, n, 1, &s, _state);
            sparseconverttocrs(&s, _state);
            normestimatorestimatesparse(&e, &s, _state);
            normestimatorresults(&e, &enorm, _state);
            waserrors = waserrors||ae_fp_neq(enorm,(double)(0));
            
            /*
             * Choose random norm, try with non-zero matrix
             * with specified norm.
             */
            snorm = ae_exp(10*ae_randomreal(_state)-5, _state);
            sparsecreate(m, n, 1, &s, _state);
            if( m>=n )
            {
                
                /*
                 * Generate random orthogonal M*M matrix,
                 * use N leading columns as columns of A
                 */
                rmatrixrndorthogonal(m, &a, _state);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        sparseset(&s, i, j, snorm*a.ptr.pp_double[i][j], _state);
                    }
                }
            }
            else
            {
                
                /*
                 * Generate random orthogonal N*N matrix,
                 * use M leading rows as rows of A
                 */
                rmatrixrndorthogonal(n, &a, _state);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        sparseset(&s, i, j, snorm*a.ptr.pp_double[i][j], _state);
                    }
                }
            }
            sparseconverttocrs(&s, _state);
            normestimatorestimatesparse(&e, &s, _state);
            normestimatorresults(&e, &enorm, _state);
            waserrors = (waserrors||ae_fp_greater(enorm,snorm*(1+tol)))||ae_fp_less(enorm,snorm*(1-tol));
        }
    }
    
    /*
     * NStart=10 should give statistically better results than NStart=1.
     * In order to test it we perform PassCount attempts to solve random
     * problem by means of two estimators: one with NStart=10 and another
     * one with NStart=1. Every time we compare two estimates and choose
     * better one.
     *
     * Random variable NBetter is a number of cases when NStart=10 was better.
     * Under null hypothesis (no difference) it is binomially distributed
     * with mean PassCount/2 and variance PassCount/4. However, we expect
     * to have significant deviation to the right, in the area of larger
     * values.
     *
     * NOTE: we use fixed N because this test is independent of problem size.
     */
    n = 3;
    normestimatorcreate(n, n, 1, 1, &e, _state);
    normestimatorcreate(n, n, 10, 1, &e2, _state);
    normestimatorsetseed(&e, 0, _state);
    normestimatorsetseed(&e2, 0, _state);
    nbetter = 0;
    passcount = 2000;
    sigma = 5.0;
    for(pass=1; pass<=passcount; pass++)
    {
        snorm = ae_pow(10.0, 2*ae_randomreal(_state)-1, _state);
        sparsecreate(n, n, 1, &s, _state);
        rmatrixrndcond(n, 2.0, &a, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                sparseset(&s, i, j, snorm*a.ptr.pp_double[i][j], _state);
            }
        }
        sparseconverttocrs(&s, _state);
        normestimatorestimatesparse(&e, &s, _state);
        normestimatorresults(&e, &enorm, _state);
        normestimatorestimatesparse(&e2, &s, _state);
        normestimatorresults(&e2, &enorm2, _state);
        if( ae_fp_less(ae_fabs(enorm2-snorm, _state),ae_fabs(enorm-snorm, _state)) )
        {
            nbetter = nbetter+1;
        }
    }
    waserrors = waserrors||ae_fp_less((double)(nbetter),0.5*passcount+sigma*ae_sqrt(0.25*passcount, _state));
    
    /*
     * Same as previous one (for NStart), but tests dependence on NIts.
     */
    n = 3;
    normestimatorcreate(n, n, 1, 1, &e, _state);
    normestimatorcreate(n, n, 1, 10, &e2, _state);
    normestimatorsetseed(&e, 0, _state);
    normestimatorsetseed(&e2, 0, _state);
    nbetter = 0;
    passcount = 2000;
    sigma = 5.0;
    for(pass=1; pass<=passcount; pass++)
    {
        snorm = ae_pow(10.0, 2*ae_randomreal(_state)-1, _state);
        sparsecreate(n, n, 1, &s, _state);
        rmatrixrndcond(n, 2.0, &a, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                sparseset(&s, i, j, snorm*a.ptr.pp_double[i][j], _state);
            }
        }
        sparseconverttocrs(&s, _state);
        normestimatorestimatesparse(&e, &s, _state);
        normestimatorresults(&e, &enorm, _state);
        normestimatorestimatesparse(&e2, &s, _state);
        normestimatorresults(&e2, &enorm2, _state);
        if( ae_fp_less(ae_fabs(enorm2-snorm, _state),ae_fabs(enorm-snorm, _state)) )
        {
            nbetter = nbetter+1;
        }
    }
    waserrors = waserrors||ae_fp_less((double)(nbetter),0.5*passcount+sigma*ae_sqrt(0.25*passcount, _state));
    
    /*
     * report
     */
    if( !silent )
    {
        printf("TESTING NORM ESTIMATOR\n");
        printf("TEST:                                    ");
        if( !waserrors )
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
ae_bool _pexec_testnormestimator(ae_bool silent, ae_state *_state)
{
    return testnormestimator(silent, _state);
}


/*$ End $*/
