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
#include "testfiltersunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testfilters(ae_bool silent, ae_state *_state)
{
    ae_bool waserrors;
    ae_bool smaerrors;
    ae_bool emaerrors;
    ae_bool lrmaerrors;
    ae_bool result;


    smaerrors = testsma(ae_true, _state);
    emaerrors = testema(ae_true, _state);
    lrmaerrors = testlrma(ae_true, _state);
    
    /*
     * Final report
     */
    waserrors = (smaerrors||emaerrors)||lrmaerrors;
    if( !silent )
    {
        printf("FILTERS TEST\n");
        printf("* SMA:                                   ");
        if( !smaerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* EMA:                                   ");
        if( !emaerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* LRMA:                                  ");
        if( !lrmaerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        if( waserrors )
        {
            printf("TEST SUMMARY: FAILED\n");
        }
        else
        {
            printf("TEST SUMMARY: PASSED\n");
        }
        printf("\n\n");
    }
    result = !waserrors;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testfilters(ae_bool silent, ae_state *_state)
{
    return testfilters(silent, _state);
}


/*************************************************************************
This function tests SMA(k) filter. It returns True on error.

Additional IsSilent parameter controls detailed error reporting.
*************************************************************************/
ae_bool testsma(ae_bool issilent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_bool precomputederrors;
    ae_bool zerohandlingerrors;
    double threshold;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);

    threshold = 1000*ae_machineepsilon;
    if( !issilent )
    {
        printf("SMA(K) TEST\n");
    }
    
    /*
     * Test several pre-computed problems.
     *
     * NOTE: tests below rely on the fact that floating point
     *       additions and subtractions are exact when dealing
     *       with integer values.
     */
    precomputederrors = ae_false;
    ae_vector_set_length(&x, 1, _state);
    x.ptr.p_double[0] = (double)(7);
    filtersma(&x, 1, 1, _state);
    precomputederrors = precomputederrors||ae_fp_neq(x.ptr.p_double[0],(double)(7));
    ae_vector_set_length(&x, 3, _state);
    x.ptr.p_double[0] = (double)(7);
    x.ptr.p_double[1] = (double)(8);
    x.ptr.p_double[2] = (double)(9);
    filtersma(&x, 3, 1, _state);
    precomputederrors = ((precomputederrors||ae_fp_neq(x.ptr.p_double[0],(double)(7)))||ae_fp_neq(x.ptr.p_double[1],(double)(8)))||ae_fp_neq(x.ptr.p_double[2],(double)(9));
    filtersma(&x, 3, 2, _state);
    precomputederrors = ((precomputederrors||ae_fp_neq(x.ptr.p_double[0],(double)(7)))||ae_fp_neq(x.ptr.p_double[1],7.5))||ae_fp_neq(x.ptr.p_double[2],8.5);
    ae_vector_set_length(&x, 3, _state);
    x.ptr.p_double[0] = (double)(7);
    x.ptr.p_double[1] = (double)(8);
    x.ptr.p_double[2] = (double)(9);
    filtersma(&x, 3, 4, _state);
    precomputederrors = ((precomputederrors||ae_fp_neq(x.ptr.p_double[0],(double)(7)))||ae_fp_neq(x.ptr.p_double[1],7.5))||ae_fp_neq(x.ptr.p_double[2],(double)(8));
    
    /*
     * Test zero-handling:
     * a) when we have non-zero sequence (N1 elements) followed by zero sequence
     *    (N2 elements), then first N1+K-1 elements of the processed sequence are
     *    non-zero, but elements since (N1+K)th must be exactly zero.
     * b) similar property holds for zero sequence followed by non-zero one
     *
     * Naive implementation of SMA does not have such property.
     *
     * NOTE: it is important to initialize X with non-integer elements with long
     * binary mantissas, because this test tries to test behaviour in the presence
     * of roundoff errors, and it will be useless when used with integer inputs.
     */
    zerohandlingerrors = ae_false;
    ae_vector_set_length(&x, 10, _state);
    x.ptr.p_double[0] = ae_sqrt((double)(2), _state);
    x.ptr.p_double[1] = ae_sqrt((double)(3), _state);
    x.ptr.p_double[2] = ae_sqrt((double)(5), _state);
    x.ptr.p_double[3] = ae_sqrt((double)(6), _state);
    x.ptr.p_double[4] = ae_sqrt((double)(7), _state);
    x.ptr.p_double[5] = (double)(0);
    x.ptr.p_double[6] = (double)(0);
    x.ptr.p_double[7] = (double)(0);
    x.ptr.p_double[8] = (double)(0);
    x.ptr.p_double[9] = (double)(0);
    filtersma(&x, 10, 3, _state);
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-ae_sqrt((double)(2), _state), _state),threshold);
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[1]-(ae_sqrt((double)(2), _state)+ae_sqrt((double)(3), _state))/2, _state),threshold);
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-(ae_sqrt((double)(2), _state)+ae_sqrt((double)(3), _state)+ae_sqrt((double)(5), _state))/3, _state),threshold);
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[3]-(ae_sqrt((double)(3), _state)+ae_sqrt((double)(5), _state)+ae_sqrt((double)(6), _state))/3, _state),threshold);
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[4]-(ae_sqrt((double)(5), _state)+ae_sqrt((double)(6), _state)+ae_sqrt((double)(7), _state))/3, _state),threshold);
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[5]-(ae_sqrt((double)(6), _state)+ae_sqrt((double)(7), _state))/3, _state),threshold);
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[6]-ae_sqrt((double)(7), _state)/3, _state),threshold);
    zerohandlingerrors = zerohandlingerrors||ae_fp_neq(x.ptr.p_double[7],(double)(0));
    zerohandlingerrors = zerohandlingerrors||ae_fp_neq(x.ptr.p_double[8],(double)(0));
    zerohandlingerrors = zerohandlingerrors||ae_fp_neq(x.ptr.p_double[9],(double)(0));
    x.ptr.p_double[0] = (double)(0);
    x.ptr.p_double[1] = (double)(0);
    x.ptr.p_double[2] = (double)(0);
    x.ptr.p_double[3] = (double)(0);
    x.ptr.p_double[4] = (double)(0);
    x.ptr.p_double[5] = ae_sqrt((double)(2), _state);
    x.ptr.p_double[6] = ae_sqrt((double)(3), _state);
    x.ptr.p_double[7] = ae_sqrt((double)(5), _state);
    x.ptr.p_double[8] = ae_sqrt((double)(6), _state);
    x.ptr.p_double[9] = ae_sqrt((double)(7), _state);
    filtersma(&x, 10, 3, _state);
    zerohandlingerrors = zerohandlingerrors||ae_fp_neq(x.ptr.p_double[0],(double)(0));
    zerohandlingerrors = zerohandlingerrors||ae_fp_neq(x.ptr.p_double[1],(double)(0));
    zerohandlingerrors = zerohandlingerrors||ae_fp_neq(x.ptr.p_double[2],(double)(0));
    zerohandlingerrors = zerohandlingerrors||ae_fp_neq(x.ptr.p_double[3],(double)(0));
    zerohandlingerrors = zerohandlingerrors||ae_fp_neq(x.ptr.p_double[4],(double)(0));
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[5]-ae_sqrt((double)(2), _state)/3, _state),threshold);
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[6]-(ae_sqrt((double)(2), _state)+ae_sqrt((double)(3), _state))/3, _state),threshold);
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[7]-(ae_sqrt((double)(2), _state)+ae_sqrt((double)(3), _state)+ae_sqrt((double)(5), _state))/3, _state),threshold);
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[8]-(ae_sqrt((double)(3), _state)+ae_sqrt((double)(5), _state)+ae_sqrt((double)(6), _state))/3, _state),threshold);
    zerohandlingerrors = zerohandlingerrors||ae_fp_greater(ae_fabs(x.ptr.p_double[9]-(ae_sqrt((double)(5), _state)+ae_sqrt((double)(6), _state)+ae_sqrt((double)(7), _state))/3, _state),threshold);
    
    /*
     * Final result
     */
    result = precomputederrors||zerohandlingerrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function tests EMA(alpha) filter. It returns True on error.

Additional IsSilent parameter controls detailed error reporting.
*************************************************************************/
ae_bool testema(ae_bool issilent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_bool precomputederrors;
    double threshold;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);

    threshold = 1000*ae_machineepsilon;
    if( !issilent )
    {
        printf("EMA(alpha) TEST\n");
    }
    
    /*
     * Test several pre-computed problems.
     *
     * NOTE: tests below rely on the fact that floating point
     *       additions and subtractions are exact when dealing
     *       with integer values.
     */
    precomputederrors = ae_false;
    ae_vector_set_length(&x, 1, _state);
    x.ptr.p_double[0] = (double)(7);
    filterema(&x, 1, 1.0, _state);
    precomputederrors = precomputederrors||ae_fp_neq(x.ptr.p_double[0],(double)(7));
    filterema(&x, 1, 0.5, _state);
    precomputederrors = precomputederrors||ae_fp_neq(x.ptr.p_double[0],(double)(7));
    ae_vector_set_length(&x, 3, _state);
    x.ptr.p_double[0] = (double)(7);
    x.ptr.p_double[1] = (double)(8);
    x.ptr.p_double[2] = (double)(9);
    filterema(&x, 3, 1.0, _state);
    precomputederrors = ((precomputederrors||ae_fp_neq(x.ptr.p_double[0],(double)(7)))||ae_fp_neq(x.ptr.p_double[1],(double)(8)))||ae_fp_neq(x.ptr.p_double[2],(double)(9));
    filterema(&x, 3, 0.5, _state);
    precomputederrors = ((precomputederrors||ae_fp_neq(x.ptr.p_double[0],(double)(7)))||ae_fp_neq(x.ptr.p_double[1],7.5))||ae_fp_neq(x.ptr.p_double[2],8.25);
    
    /*
     * Final result
     */
    result = precomputederrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function tests LRMA(k) filter. It returns True on error.

Additional IsSilent parameter controls detailed error reporting.
*************************************************************************/
ae_bool testlrma(ae_bool issilent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_bool precomputederrors;
    double threshold;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);

    threshold = 1000*ae_machineepsilon;
    if( !issilent )
    {
        printf("LRMA(K) TEST\n");
    }
    precomputederrors = ae_false;
    
    /*
     * First, check that filter does not changes points for K=1 or K=2
     */
    ae_vector_set_length(&x, 1, _state);
    x.ptr.p_double[0] = (double)(7);
    filterlrma(&x, 1, 1, _state);
    precomputederrors = precomputederrors||ae_fp_neq(x.ptr.p_double[0],(double)(7));
    ae_vector_set_length(&x, 6, _state);
    x.ptr.p_double[0] = (double)(7);
    x.ptr.p_double[1] = (double)(8);
    x.ptr.p_double[2] = (double)(9);
    x.ptr.p_double[3] = (double)(10);
    x.ptr.p_double[4] = (double)(11);
    x.ptr.p_double[5] = (double)(12);
    filterlrma(&x, 6, 1, _state);
    precomputederrors = (((((precomputederrors||ae_fp_neq(x.ptr.p_double[0],(double)(7)))||ae_fp_neq(x.ptr.p_double[1],(double)(8)))||ae_fp_neq(x.ptr.p_double[2],(double)(9)))||ae_fp_neq(x.ptr.p_double[3],(double)(10)))||ae_fp_neq(x.ptr.p_double[4],(double)(11)))||ae_fp_neq(x.ptr.p_double[5],(double)(12));
    filterlrma(&x, 6, 2, _state);
    precomputederrors = (((((precomputederrors||ae_fp_neq(x.ptr.p_double[0],(double)(7)))||ae_fp_neq(x.ptr.p_double[1],(double)(8)))||ae_fp_neq(x.ptr.p_double[2],(double)(9)))||ae_fp_neq(x.ptr.p_double[3],(double)(10)))||ae_fp_neq(x.ptr.p_double[4],(double)(11)))||ae_fp_neq(x.ptr.p_double[5],(double)(12));
    
    /*
     * Check several precomputed problems
     */
    ae_vector_set_length(&x, 6, _state);
    x.ptr.p_double[0] = (double)(7);
    x.ptr.p_double[1] = (double)(8);
    x.ptr.p_double[2] = (double)(9);
    x.ptr.p_double[3] = (double)(10);
    x.ptr.p_double[4] = (double)(11);
    x.ptr.p_double[5] = (double)(12);
    filterlrma(&x, 6, 3, _state);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-7, _state),threshold);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[1]-8, _state),threshold);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-9, _state),threshold);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[3]-10, _state),threshold);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[4]-11, _state),threshold);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[5]-12, _state),threshold);
    ae_vector_set_length(&x, 6, _state);
    x.ptr.p_double[0] = (double)(7);
    x.ptr.p_double[1] = (double)(8);
    x.ptr.p_double[2] = (double)(8);
    x.ptr.p_double[3] = (double)(9);
    x.ptr.p_double[4] = (double)(12);
    x.ptr.p_double[5] = (double)(12);
    filterlrma(&x, 6, 3, _state);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-7.0000000000, _state),1.0E-5);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[1]-8.0000000000, _state),1.0E-5);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-8.1666666667, _state),1.0E-5);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[3]-8.8333333333, _state),1.0E-5);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[4]-11.6666666667, _state),1.0E-5);
    precomputederrors = precomputederrors||ae_fp_greater(ae_fabs(x.ptr.p_double[5]-12.5000000000, _state),1.0E-5);
    
    /*
     * Final result
     */
    result = precomputederrors;
    ae_frame_leave(_state);
    return result;
}


/*$ End $*/
