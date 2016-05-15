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
#include "testmcpdunit.h"


/*$ Declarations $*/
static void testmcpdunit_testsimple(ae_bool* err, ae_state *_state);
static void testmcpdunit_testentryexit(ae_bool* err, ae_state *_state);
static void testmcpdunit_testec(ae_bool* err, ae_state *_state);
static void testmcpdunit_testbc(ae_bool* err, ae_state *_state);
static void testmcpdunit_testlc(ae_bool* err, ae_state *_state);
static void testmcpdunit_createee(ae_int_t n,
     ae_int_t entrystate,
     ae_int_t exitstate,
     mcpdstate* s,
     ae_state *_state);


/*$ Body $*/


ae_bool testmcpd(ae_bool silent, ae_state *_state)
{
    ae_bool waserrors;
    ae_bool simpleerrors;
    ae_bool entryexiterrors;
    ae_bool ecerrors;
    ae_bool bcerrors;
    ae_bool lcerrors;
    ae_bool othererrors;
    ae_bool result;


    
    /*
     * Init
     */
    waserrors = ae_false;
    othererrors = ae_false;
    simpleerrors = ae_false;
    entryexiterrors = ae_false;
    ecerrors = ae_false;
    bcerrors = ae_false;
    lcerrors = ae_false;
    
    /*
     * Test
     */
    testmcpdunit_testsimple(&simpleerrors, _state);
    testmcpdunit_testentryexit(&entryexiterrors, _state);
    testmcpdunit_testec(&ecerrors, _state);
    testmcpdunit_testbc(&bcerrors, _state);
    testmcpdunit_testlc(&lcerrors, _state);
    
    /*
     * Final report
     */
    waserrors = ((((othererrors||simpleerrors)||entryexiterrors)||ecerrors)||bcerrors)||lcerrors;
    if( !silent )
    {
        printf("MCPD TEST\n");
        printf("TOTAL RESULTS:                           ");
        if( !waserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* SIMPLE:                                ");
        if( !simpleerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* ENTRY/EXIT:                            ");
        if( !entryexiterrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* EQUALITY CONSTRAINTS:                  ");
        if( !ecerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* BOUND CONSTRAINTS:                     ");
        if( !bcerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* LINEAR CONSTRAINTS:                    ");
        if( !lcerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* OTHER PROPERTIES:                      ");
        if( !othererrors )
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
ae_bool _pexec_testmcpd(ae_bool silent, ae_state *_state)
{
    return testmcpd(silent, _state);
}


/*************************************************************************
Simple test with no "entry"/"exit" states

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testmcpdunit_testsimple(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_matrix pexact;
    ae_matrix xy;
    double threshold;
    ae_int_t i;
    ae_int_t j;
    double v;
    double v0;
    ae_matrix p;
    mcpdstate s;
    mcpdreport rep;
    double offdiagonal;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&pexact, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p, 0, 0, DT_REAL, _state);
    _mcpdstate_init(&s, _state);
    _mcpdreport_init(&rep, _state);

    threshold = 1.0E-2;
    
    /*
     * First test:
     * * N-dimensional problem
     * * proportional data
     * * no "entry"/"exit" states
     * * N tracks, each includes only two states
     * * first record in I-th track is [0 ... 1 ... 0] with 1 is in I-th position
     * * all tracks are modelled using randomly generated transition matrix P
     */
    for(n=1; n<=5; n++)
    {
        
        /*
         * Initialize "exact" P:
         * * fill by random values
         * * make sure that each column sums to non-zero value
         * * normalize
         */
        ae_matrix_set_length(&pexact, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                pexact.ptr.pp_double[i][j] = ae_randomreal(_state);
            }
        }
        for(j=0; j<=n-1; j++)
        {
            i = ae_randominteger(n, _state);
            pexact.ptr.pp_double[i][j] = pexact.ptr.pp_double[i][j]+0.1;
        }
        for(j=0; j<=n-1; j++)
        {
            v = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                v = v+pexact.ptr.pp_double[i][j];
            }
            for(i=0; i<=n-1; i++)
            {
                pexact.ptr.pp_double[i][j] = pexact.ptr.pp_double[i][j]/v;
            }
        }
        
        /*
         * Initialize solver:
         * * create object
         * * add tracks
         */
        mcpdcreate(n, &s, _state);
        for(i=0; i<=n-1; i++)
        {
            ae_matrix_set_length(&xy, 2, n, _state);
            for(j=0; j<=n-1; j++)
            {
                xy.ptr.pp_double[0][j] = (double)(0);
            }
            xy.ptr.pp_double[0][i] = (double)(1);
            for(j=0; j<=n-1; j++)
            {
                xy.ptr.pp_double[1][j] = pexact.ptr.pp_double[j][i];
            }
            mcpdaddtrack(&s, &xy, 2, _state);
        }
        
        /*
         * Solve and test
         */
        mcpdsolve(&s, _state);
        mcpdresults(&s, &p, &rep, _state);
        if( rep.terminationtype>0 )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    *err = *err||ae_fp_greater(ae_fabs(p.ptr.pp_double[i][j]-pexact.ptr.pp_double[i][j], _state),threshold);
                }
            }
        }
        else
        {
            *err = ae_true;
        }
    }
    
    /*
     * Second test:
     * * N-dimensional problem
     * * proportional data
     * * no "entry"/"exit" states
     * * N tracks, each includes only two states
     * * first record in I-th track is [0 ...0.1 0.8 0.1 ... 0] with 0.8 is in I-th position
     * * all tracks are modelled using randomly generated transition matrix P
     */
    offdiagonal = 0.1;
    for(n=1; n<=5; n++)
    {
        
        /*
         * Initialize "exact" P:
         * * fill by random values
         * * make sure that each column sums to non-zero value
         * * normalize
         */
        ae_matrix_set_length(&pexact, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                pexact.ptr.pp_double[i][j] = ae_randomreal(_state);
            }
        }
        for(j=0; j<=n-1; j++)
        {
            i = ae_randominteger(n, _state);
            pexact.ptr.pp_double[i][j] = pexact.ptr.pp_double[i][j]+0.1;
        }
        for(j=0; j<=n-1; j++)
        {
            v = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                v = v+pexact.ptr.pp_double[i][j];
            }
            for(i=0; i<=n-1; i++)
            {
                pexact.ptr.pp_double[i][j] = pexact.ptr.pp_double[i][j]/v;
            }
        }
        
        /*
         * Initialize solver:
         * * create object
         * * add tracks
         */
        mcpdcreate(n, &s, _state);
        for(i=0; i<=n-1; i++)
        {
            ae_matrix_set_length(&xy, 2, n, _state);
            for(j=0; j<=n-1; j++)
            {
                xy.ptr.pp_double[0][j] = (double)(0);
            }
            
            /*
             * "main" element
             */
            xy.ptr.pp_double[0][i] = 1.0-2*offdiagonal;
            for(j=0; j<=n-1; j++)
            {
                xy.ptr.pp_double[1][j] = (1.0-2*offdiagonal)*pexact.ptr.pp_double[j][i];
            }
            
            /*
             * off-diagonal ones
             */
            if( i>0 )
            {
                xy.ptr.pp_double[0][i-1] = offdiagonal;
                for(j=0; j<=n-1; j++)
                {
                    xy.ptr.pp_double[1][j] = xy.ptr.pp_double[1][j]+offdiagonal*pexact.ptr.pp_double[j][i-1];
                }
            }
            if( i<n-1 )
            {
                xy.ptr.pp_double[0][i+1] = offdiagonal;
                for(j=0; j<=n-1; j++)
                {
                    xy.ptr.pp_double[1][j] = xy.ptr.pp_double[1][j]+offdiagonal*pexact.ptr.pp_double[j][i+1];
                }
            }
            mcpdaddtrack(&s, &xy, 2, _state);
        }
        
        /*
         * Solve and test
         */
        mcpdsolve(&s, _state);
        mcpdresults(&s, &p, &rep, _state);
        if( rep.terminationtype>0 )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    *err = *err||ae_fp_greater(ae_fabs(p.ptr.pp_double[i][j]-pexact.ptr.pp_double[i][j], _state),threshold);
                }
            }
        }
        else
        {
            *err = ae_true;
        }
    }
    
    /*
     * Third test:
     * * N-dimensional problem
     * * population data
     * * no "entry"/"exit" states
     * * N tracks, each includes only two states
     * * first record in I-th track is V*[0 ...0.1 0.8 0.1 ... 0] with 0.8 is in I-th position, V in [1,10]
     * * all tracks are modelled using randomly generated transition matrix P
     */
    offdiagonal = 0.1;
    for(n=1; n<=5; n++)
    {
        
        /*
         * Initialize "exact" P:
         * * fill by random values
         * * make sure that each column sums to non-zero value
         * * normalize
         */
        ae_matrix_set_length(&pexact, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                pexact.ptr.pp_double[i][j] = ae_randomreal(_state);
            }
        }
        for(j=0; j<=n-1; j++)
        {
            i = ae_randominteger(n, _state);
            pexact.ptr.pp_double[i][j] = pexact.ptr.pp_double[i][j]+0.1;
        }
        for(j=0; j<=n-1; j++)
        {
            v = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                v = v+pexact.ptr.pp_double[i][j];
            }
            for(i=0; i<=n-1; i++)
            {
                pexact.ptr.pp_double[i][j] = pexact.ptr.pp_double[i][j]/v;
            }
        }
        
        /*
         * Initialize solver:
         * * create object
         * * add tracks
         */
        mcpdcreate(n, &s, _state);
        for(i=0; i<=n-1; i++)
        {
            ae_matrix_set_length(&xy, 2, n, _state);
            for(j=0; j<=n-1; j++)
            {
                xy.ptr.pp_double[0][j] = (double)(0);
            }
            
            /*
             * "main" element
             */
            v0 = 9*ae_randomreal(_state)+1;
            xy.ptr.pp_double[0][i] = v0*(1.0-2*offdiagonal);
            for(j=0; j<=n-1; j++)
            {
                xy.ptr.pp_double[1][j] = v0*(1.0-2*offdiagonal)*pexact.ptr.pp_double[j][i];
            }
            
            /*
             * off-diagonal ones
             */
            if( i>0 )
            {
                xy.ptr.pp_double[0][i-1] = v0*offdiagonal;
                for(j=0; j<=n-1; j++)
                {
                    xy.ptr.pp_double[1][j] = xy.ptr.pp_double[1][j]+v0*offdiagonal*pexact.ptr.pp_double[j][i-1];
                }
            }
            if( i<n-1 )
            {
                xy.ptr.pp_double[0][i+1] = v0*offdiagonal;
                for(j=0; j<=n-1; j++)
                {
                    xy.ptr.pp_double[1][j] = xy.ptr.pp_double[1][j]+v0*offdiagonal*pexact.ptr.pp_double[j][i+1];
                }
            }
            mcpdaddtrack(&s, &xy, 2, _state);
        }
        
        /*
         * Solve and test
         */
        mcpdsolve(&s, _state);
        mcpdresults(&s, &p, &rep, _state);
        if( rep.terminationtype>0 )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    *err = *err||ae_fp_greater(ae_fabs(p.ptr.pp_double[i][j]-pexact.ptr.pp_double[i][j], _state),threshold);
                }
            }
        }
        else
        {
            *err = ae_true;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Test for different combinations of "entry"/"exit" models

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testmcpdunit_testentryexit(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_matrix p;
    ae_matrix pexact;
    ae_matrix xy;
    double threshold;
    ae_int_t entrystate;
    ae_int_t exitstate;
    ae_int_t entrykind;
    ae_int_t exitkind;
    ae_int_t popkind;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    mcpdstate s;
    mcpdreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&p, 0, 0, DT_REAL, _state);
    ae_matrix_init(&pexact, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _mcpdstate_init(&s, _state);
    _mcpdreport_init(&rep, _state);

    threshold = 1.0E-3;
    
    /*
     *
     */
    for(n=2; n<=5; n++)
    {
        for(entrykind=0; entrykind<=1; entrykind++)
        {
            for(exitkind=0; exitkind<=1; exitkind++)
            {
                for(popkind=0; popkind<=1; popkind++)
                {
                    
                    /*
                     * Generate EntryState/ExitState such that one of the following is True:
                     * * EntryState<>ExitState
                     * * EntryState=-1 or ExitState=-1
                     */
                    do
                    {
                        if( entrykind==0 )
                        {
                            entrystate = -1;
                        }
                        else
                        {
                            entrystate = ae_randominteger(n, _state);
                        }
                        if( exitkind==0 )
                        {
                            exitstate = -1;
                        }
                        else
                        {
                            exitstate = ae_randominteger(n, _state);
                        }
                    }
                    while(!((entrystate==-1||exitstate==-1)||entrystate!=exitstate));
                    
                    /*
                     * Generate transition matrix P such that:
                     * * columns corresponding to non-exit states sums to 1.0
                     * * columns corresponding to exit states sums to 0.0
                     * * rows corresponding to entry states are zero
                     */
                    ae_matrix_set_length(&pexact, n, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            pexact.ptr.pp_double[i][j] = (double)(1+ae_randominteger(5, _state));
                            if( i==entrystate )
                            {
                                pexact.ptr.pp_double[i][j] = 0.0;
                            }
                            if( j==exitstate )
                            {
                                pexact.ptr.pp_double[i][j] = 0.0;
                            }
                        }
                    }
                    for(j=0; j<=n-1; j++)
                    {
                        v = 0.0;
                        for(i=0; i<=n-1; i++)
                        {
                            v = v+pexact.ptr.pp_double[i][j];
                        }
                        if( ae_fp_neq(v,(double)(0)) )
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                pexact.ptr.pp_double[i][j] = pexact.ptr.pp_double[i][j]/v;
                            }
                        }
                    }
                    
                    /*
                     * Create MCPD solver
                     */
                    if( entrystate<0&&exitstate<0 )
                    {
                        mcpdcreate(n, &s, _state);
                    }
                    if( entrystate>=0&&exitstate<0 )
                    {
                        mcpdcreateentry(n, entrystate, &s, _state);
                    }
                    if( entrystate<0&&exitstate>=0 )
                    {
                        mcpdcreateexit(n, exitstate, &s, _state);
                    }
                    if( entrystate>=0&&exitstate>=0 )
                    {
                        mcpdcreateentryexit(n, entrystate, exitstate, &s, _state);
                    }
                    
                    /*
                     * Add N tracks.
                     *
                     * K-th track starts from vector with large value of
                     * K-th component and small random noise in other components.
                     *
                     * Track contains from 2 to 4 elements.
                     *
                     * Tracks contain proportional (normalized) or
                     * population data, depending on PopKind variable.
                     */
                    for(k=0; k<=n-1; k++)
                    {
                        
                        /*
                         * Generate track whose length is in 2..4
                         */
                        ae_matrix_set_length(&xy, 2+ae_randominteger(3, _state), n, _state);
                        for(j=0; j<=n-1; j++)
                        {
                            xy.ptr.pp_double[0][j] = 0.05*ae_randomreal(_state);
                        }
                        xy.ptr.pp_double[0][k] = 1+ae_randomreal(_state);
                        for(i=1; i<=xy.rows-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                if( j!=entrystate )
                                {
                                    v = ae_v_dotproduct(&pexact.ptr.pp_double[j][0], 1, &xy.ptr.pp_double[i-1][0], 1, ae_v_len(0,n-1));
                                    xy.ptr.pp_double[i][j] = v;
                                }
                                else
                                {
                                    xy.ptr.pp_double[i][j] = ae_randomreal(_state);
                                }
                            }
                        }
                        
                        /*
                         * Normalize, if needed
                         */
                        if( popkind==1 )
                        {
                            for(i=0; i<=xy.rows-1; i++)
                            {
                                v = 0.0;
                                for(j=0; j<=n-1; j++)
                                {
                                    v = v+xy.ptr.pp_double[i][j];
                                }
                                if( ae_fp_greater(v,(double)(0)) )
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        xy.ptr.pp_double[i][j] = xy.ptr.pp_double[i][j]/v;
                                    }
                                }
                            }
                        }
                        
                        /*
                         * Add track
                         */
                        mcpdaddtrack(&s, &xy, xy.rows, _state);
                    }
                    
                    /*
                     * Solve and test
                     */
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    if( rep.terminationtype>0 )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                *err = *err||ae_fp_greater(ae_fabs(p.ptr.pp_double[i][j]-pexact.ptr.pp_double[i][j], _state),threshold);
                            }
                        }
                    }
                    else
                    {
                        *err = ae_true;
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Test equality constraints.

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testmcpdunit_testec(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_matrix p;
    ae_matrix ec;
    ae_matrix xy;
    ae_int_t entrystate;
    ae_int_t exitstate;
    ae_int_t entrykind;
    ae_int_t exitkind;
    ae_int_t i;
    ae_int_t j;
    ae_int_t ic;
    ae_int_t jc;
    double vc;
    mcpdstate s;
    mcpdreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&p, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ec, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _mcpdstate_init(&s, _state);
    _mcpdreport_init(&rep, _state);

    
    /*
     * We try different problems with following properties:
     * * N is large enough - we won't have problems with inconsistent constraints
     * * first state is either "entry" or "normal"
     * * last state is either "exit" or "normal"
     * * we have one long random track
     *
     * We test several properties which are described in comments below
     */
    for(n=4; n<=6; n++)
    {
        for(entrykind=0; entrykind<=1; entrykind++)
        {
            for(exitkind=0; exitkind<=1; exitkind++)
            {
                
                /*
                 * Prepare problem
                 */
                if( entrykind==0 )
                {
                    entrystate = -1;
                }
                else
                {
                    entrystate = 0;
                }
                if( exitkind==0 )
                {
                    exitstate = -1;
                }
                else
                {
                    exitstate = n-1;
                }
                ae_matrix_set_length(&xy, 2*n, n, _state);
                for(i=0; i<=xy.rows-1; i++)
                {
                    for(j=0; j<=xy.cols-1; j++)
                    {
                        xy.ptr.pp_double[i][j] = ae_randomreal(_state);
                    }
                }
                
                /*
                 * Test that single equality constraint on non-entry
                 * non-exit elements of P is satisfied.
                 *
                 * NOTE: this test needs N>=4 because smaller values
                 * can give us inconsistent constraints
                 */
                ae_assert(n>=4, "TestEC: expectation failed", _state);
                ic = 1+ae_randominteger(n-2, _state);
                jc = 1+ae_randominteger(n-2, _state);
                vc = ae_randomreal(_state);
                testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                mcpdaddtrack(&s, &xy, xy.rows, _state);
                mcpdaddec(&s, ic, jc, vc, _state);
                mcpdsolve(&s, _state);
                mcpdresults(&s, &p, &rep, _state);
                if( rep.terminationtype>0 )
                {
                    *err = *err||ae_fp_neq(p.ptr.pp_double[ic][jc],vc);
                }
                else
                {
                    *err = ae_true;
                }
                
                /*
                 * Test interaction with default "sum-to-one" constraint
                 * on columns of P.
                 *
                 * We set N-1 equality constraints on random non-exit column
                 * of P, which are inconsistent with this default constraint
                 * (sum will be greater that 1.0).
                 *
                 * Algorithm must detect inconsistency.
                 *
                 * NOTE:
                 * 1. we do not set constraints for the first element of
                 *    the column, because this element may be constrained by
                 *    "exit state" constraint.
                 * 2. this test needs N>=3
                 */
                ae_assert(n>=3, "TestEC: expectation failed", _state);
                jc = ae_randominteger(n-1, _state);
                vc = 0.95;
                testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                mcpdaddtrack(&s, &xy, xy.rows, _state);
                for(i=1; i<=n-1; i++)
                {
                    mcpdaddec(&s, i, jc, vc, _state);
                }
                mcpdsolve(&s, _state);
                mcpdresults(&s, &p, &rep, _state);
                *err = *err||rep.terminationtype!=-3;
                
                /*
                 * Test interaction with constrains on entry states.
                 *
                 * When model has entry state, corresponding row of P
                 * must be zero. We try to set two kinds of constraints
                 * on random element of this row:
                 * * zero equality constraint, which must be consistent
                 * * non-zero equality constraint, which must be inconsistent
                 */
                if( entrystate>=0 )
                {
                    jc = ae_randominteger(n, _state);
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdaddec(&s, entrystate, jc, 0.0, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype<=0;
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdaddec(&s, entrystate, jc, 0.5, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype!=-3;
                }
                
                /*
                 * Test interaction with constrains on exit states.
                 *
                 * When model has exit state, corresponding column of P
                 * must be zero. We try to set two kinds of constraints
                 * on random element of this column:
                 * * zero equality constraint, which must be consistent
                 * * non-zero equality constraint, which must be inconsistent
                 */
                if( exitstate>=0 )
                {
                    ic = ae_randominteger(n, _state);
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdaddec(&s, ic, exitstate, 0.0, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype<=0;
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdaddec(&s, ic, exitstate, 0.5, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype!=-3;
                }
                
                /*
                 * Test SetEC() call - we constrain subset of non-entry
                 * non-exit elements and test it.
                 */
                ae_assert(n>=4, "TestEC: expectation failed", _state);
                ae_matrix_set_length(&ec, n, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        ec.ptr.pp_double[i][j] = _state->v_nan;
                    }
                }
                for(j=1; j<=n-2; j++)
                {
                    ec.ptr.pp_double[1+ae_randominteger(n-2, _state)][j] = 0.1+0.1*ae_randomreal(_state);
                }
                testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                mcpdaddtrack(&s, &xy, xy.rows, _state);
                mcpdsetec(&s, &ec, _state);
                mcpdsolve(&s, _state);
                mcpdresults(&s, &p, &rep, _state);
                if( rep.terminationtype>0 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            if( ae_isfinite(ec.ptr.pp_double[i][j], _state) )
                            {
                                *err = *err||ae_fp_neq(p.ptr.pp_double[i][j],ec.ptr.pp_double[i][j]);
                            }
                        }
                    }
                }
                else
                {
                    *err = ae_true;
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Test bound constraints.

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testmcpdunit_testbc(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_matrix p;
    ae_matrix bndl;
    ae_matrix bndu;
    ae_matrix xy;
    ae_int_t entrystate;
    ae_int_t exitstate;
    ae_int_t entrykind;
    ae_int_t exitkind;
    ae_int_t i;
    ae_int_t j;
    ae_int_t ic;
    ae_int_t jc;
    double vl;
    double vu;
    mcpdstate s;
    mcpdreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&p, 0, 0, DT_REAL, _state);
    ae_matrix_init(&bndl, 0, 0, DT_REAL, _state);
    ae_matrix_init(&bndu, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _mcpdstate_init(&s, _state);
    _mcpdreport_init(&rep, _state);

    
    /*
     * We try different problems with following properties:
     * * N is large enough - we won't have problems with inconsistent constraints
     * * first state is either "entry" or "normal"
     * * last state is either "exit" or "normal"
     * * we have one long random track
     *
     * We test several properties which are described in comments below
     */
    for(n=4; n<=6; n++)
    {
        for(entrykind=0; entrykind<=1; entrykind++)
        {
            for(exitkind=0; exitkind<=1; exitkind++)
            {
                
                /*
                 * Prepare problem
                 */
                if( entrykind==0 )
                {
                    entrystate = -1;
                }
                else
                {
                    entrystate = 0;
                }
                if( exitkind==0 )
                {
                    exitstate = -1;
                }
                else
                {
                    exitstate = n-1;
                }
                ae_matrix_set_length(&xy, 2*n, n, _state);
                for(i=0; i<=xy.rows-1; i++)
                {
                    for(j=0; j<=xy.cols-1; j++)
                    {
                        xy.ptr.pp_double[i][j] = ae_randomreal(_state);
                    }
                }
                
                /*
                 * Test that single bound constraint on non-entry
                 * non-exit elements of P is satisfied.
                 *
                 * NOTE 1: this test needs N>=4 because smaller values
                 * can give us inconsistent constraints
                 */
                ae_assert(n>=4, "TestBC: expectation failed", _state);
                ic = 1+ae_randominteger(n-2, _state);
                jc = 1+ae_randominteger(n-2, _state);
                if( ae_fp_greater(ae_randomreal(_state),0.5) )
                {
                    vl = 0.3*ae_randomreal(_state);
                }
                else
                {
                    vl = _state->v_neginf;
                }
                if( ae_fp_greater(ae_randomreal(_state),0.5) )
                {
                    vu = 0.5+0.3*ae_randomreal(_state);
                }
                else
                {
                    vu = _state->v_posinf;
                }
                testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                mcpdaddtrack(&s, &xy, xy.rows, _state);
                mcpdaddbc(&s, ic, jc, vl, vu, _state);
                mcpdsolve(&s, _state);
                mcpdresults(&s, &p, &rep, _state);
                if( rep.terminationtype>0 )
                {
                    *err = *err||ae_fp_less(p.ptr.pp_double[ic][jc],vl);
                    *err = *err||ae_fp_greater(p.ptr.pp_double[ic][jc],vu);
                }
                else
                {
                    *err = ae_true;
                }
                
                /*
                 * Test interaction with default "sum-to-one" constraint
                 * on columns of P.
                 *
                 * We set N-1 bound constraints on random non-exit column
                 * of P, which are inconsistent with this default constraint
                 * (sum will be greater that 1.0).
                 *
                 * Algorithm must detect inconsistency.
                 *
                 * NOTE:
                 * 1. we do not set constraints for the first element of
                 *    the column, because this element may be constrained by
                 *    "exit state" constraint.
                 * 2. this test needs N>=3
                 */
                ae_assert(n>=3, "TestEC: expectation failed", _state);
                jc = ae_randominteger(n-1, _state);
                vl = 0.85;
                vu = 0.95;
                testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                mcpdaddtrack(&s, &xy, xy.rows, _state);
                for(i=1; i<=n-1; i++)
                {
                    mcpdaddbc(&s, i, jc, vl, vu, _state);
                }
                mcpdsolve(&s, _state);
                mcpdresults(&s, &p, &rep, _state);
                *err = *err||rep.terminationtype!=-3;
                
                /*
                 * Test interaction with constrains on entry states.
                 *
                 * When model has entry state, corresponding row of P
                 * must be zero. We try to set two kinds of constraints
                 * on random element of this row:
                 * * bound constraint with zero lower bound, which must be consistent
                 * * bound constraint with non-zero lower bound, which must be inconsistent
                 */
                if( entrystate>=0 )
                {
                    jc = ae_randominteger(n, _state);
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdaddbc(&s, entrystate, jc, 0.0, 1.0, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype<=0;
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdaddbc(&s, entrystate, jc, 0.5, 1.0, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype!=-3;
                }
                
                /*
                 * Test interaction with constrains on exit states.
                 *
                 * When model has exit state, corresponding column of P
                 * must be zero. We try to set two kinds of constraints
                 * on random element of this column:
                 * * bound constraint with zero lower bound, which must be consistent
                 * * bound constraint with non-zero lower bound, which must be inconsistent
                 */
                if( exitstate>=0 )
                {
                    ic = ae_randominteger(n, _state);
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdaddbc(&s, ic, exitstate, 0.0, 1.0, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype<=0;
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdaddbc(&s, ic, exitstate, 0.5, 1.0, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype!=-3;
                }
                
                /*
                 * Test SetBC() call - we constrain subset of non-entry
                 * non-exit elements and test it.
                 */
                ae_assert(n>=4, "TestBC: expectation failed", _state);
                ae_matrix_set_length(&bndl, n, n, _state);
                ae_matrix_set_length(&bndu, n, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        bndl.ptr.pp_double[i][j] = _state->v_neginf;
                        bndu.ptr.pp_double[i][j] = _state->v_posinf;
                    }
                }
                for(j=1; j<=n-2; j++)
                {
                    i = 1+ae_randominteger(n-2, _state);
                    bndl.ptr.pp_double[i][j] = 0.5-0.1*ae_randomreal(_state);
                    bndu.ptr.pp_double[i][j] = 0.5+0.1*ae_randomreal(_state);
                }
                testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                mcpdaddtrack(&s, &xy, xy.rows, _state);
                mcpdsetbc(&s, &bndl, &bndu, _state);
                mcpdsolve(&s, _state);
                mcpdresults(&s, &p, &rep, _state);
                if( rep.terminationtype>0 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            *err = *err||ae_fp_less(p.ptr.pp_double[i][j],bndl.ptr.pp_double[i][j]);
                            *err = *err||ae_fp_greater(p.ptr.pp_double[i][j],bndu.ptr.pp_double[i][j]);
                        }
                    }
                }
                else
                {
                    *err = ae_true;
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Test bound constraints.

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testmcpdunit_testlc(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_matrix p;
    ae_matrix c;
    ae_matrix xy;
    ae_vector ct;
    ae_int_t entrystate;
    ae_int_t exitstate;
    ae_int_t entrykind;
    ae_int_t exitkind;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t t;
    ae_int_t jc;
    double v;
    double threshold;
    mcpdstate s;
    mcpdreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&p, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    _mcpdstate_init(&s, _state);
    _mcpdreport_init(&rep, _state);

    threshold = 1.0E5*ae_machineepsilon;
    
    /*
     * We try different problems with following properties:
     * * N is large enough - we won't have problems with inconsistent constraints
     * * first state is either "entry" or "normal"
     * * last state is either "exit" or "normal"
     * * we have one long random track
     *
     * We test several properties which are described in comments below
     */
    for(n=4; n<=6; n++)
    {
        for(entrykind=0; entrykind<=1; entrykind++)
        {
            for(exitkind=0; exitkind<=1; exitkind++)
            {
                
                /*
                 * Prepare problem
                 */
                if( entrykind==0 )
                {
                    entrystate = -1;
                }
                else
                {
                    entrystate = 0;
                }
                if( exitkind==0 )
                {
                    exitstate = -1;
                }
                else
                {
                    exitstate = n-1;
                }
                ae_matrix_set_length(&xy, 2*n, n, _state);
                for(i=0; i<=xy.rows-1; i++)
                {
                    for(j=0; j<=xy.cols-1; j++)
                    {
                        xy.ptr.pp_double[i][j] = ae_randomreal(_state);
                    }
                }
                
                /*
                 * Test that single linear equality/inequality constraint
                 * on non-entry non-exit elements of P is satisfied.
                 *
                 * NOTE 1: this test needs N>=4 because smaller values
                 *         can give us inconsistent constraints
                 * NOTE 2: Constraints are generated is such a way that P=(1/N ... 1/N)
                 *         is always feasible. It guarantees that there always exists
                 *         at least one feasible point
                 * NOTE 3: If we have inequality constraint, we "shift" right part
                 *         in order to make feasible some neighborhood of P=(1/N ... 1/N).
                 */
                ae_assert(n>=4, "TestLC: expectation failed", _state);
                ae_matrix_set_length(&c, 1, n*n+1, _state);
                ae_vector_set_length(&ct, 1, _state);
                v = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( ((i==0||i==n-1)||j==0)||j==n-1 )
                        {
                            c.ptr.pp_double[0][i*n+j] = (double)(0);
                        }
                        else
                        {
                            c.ptr.pp_double[0][i*n+j] = ae_randomreal(_state);
                            v = v+c.ptr.pp_double[0][i*n+j]*((double)1/(double)n);
                        }
                    }
                }
                c.ptr.pp_double[0][n*n] = v;
                ct.ptr.p_int[0] = ae_randominteger(3, _state)-1;
                if( ct.ptr.p_int[0]<0 )
                {
                    c.ptr.pp_double[0][n*n] = c.ptr.pp_double[0][n*n]+0.1;
                }
                if( ct.ptr.p_int[0]>0 )
                {
                    c.ptr.pp_double[0][n*n] = c.ptr.pp_double[0][n*n]-0.1;
                }
                testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                mcpdaddtrack(&s, &xy, xy.rows, _state);
                mcpdsetlc(&s, &c, &ct, 1, _state);
                mcpdsolve(&s, _state);
                mcpdresults(&s, &p, &rep, _state);
                if( rep.terminationtype>0 )
                {
                    v = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            v = v+p.ptr.pp_double[i][j]*c.ptr.pp_double[0][i*n+j];
                        }
                    }
                    if( ct.ptr.p_int[0]<0 )
                    {
                        *err = *err||ae_fp_greater_eq(v,c.ptr.pp_double[0][n*n]+threshold);
                    }
                    if( ct.ptr.p_int[0]==0 )
                    {
                        *err = *err||ae_fp_greater_eq(ae_fabs(v-c.ptr.pp_double[0][n*n], _state),threshold);
                    }
                    if( ct.ptr.p_int[0]>0 )
                    {
                        *err = *err||ae_fp_less_eq(v,c.ptr.pp_double[0][n*n]-threshold);
                    }
                }
                else
                {
                    *err = ae_true;
                }
                
                /*
                 * Test interaction with default "sum-to-one" constraint
                 * on columns of P.
                 *
                 * We set linear constraint which has for "sum-to-X" on
                 * on random non-exit column of P. This constraint can be
                 * either consistent (X=1.0) or inconsistent (X<>1.0) with
                 * this default constraint.
                 *
                 * Algorithm must detect inconsistency.
                 *
                 * NOTE:
                 * 1. this test needs N>=2
                 */
                ae_assert(n>=2, "TestLC: expectation failed", _state);
                jc = ae_randominteger(n-1, _state);
                ae_matrix_set_length(&c, 1, n*n+1, _state);
                ae_vector_set_length(&ct, 1, _state);
                for(i=0; i<=n*n-1; i++)
                {
                    c.ptr.pp_double[0][i] = 0.0;
                }
                for(i=0; i<=n-1; i++)
                {
                    c.ptr.pp_double[0][n*i+jc] = 1.0;
                }
                c.ptr.pp_double[0][n*n] = 1.0;
                ct.ptr.p_int[0] = 0;
                testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                mcpdaddtrack(&s, &xy, xy.rows, _state);
                mcpdsetlc(&s, &c, &ct, 1, _state);
                mcpdsolve(&s, _state);
                mcpdresults(&s, &p, &rep, _state);
                *err = *err||rep.terminationtype<=0;
                c.ptr.pp_double[0][n*n] = 2.0;
                testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                mcpdaddtrack(&s, &xy, xy.rows, _state);
                mcpdsetlc(&s, &c, &ct, 1, _state);
                mcpdsolve(&s, _state);
                mcpdresults(&s, &p, &rep, _state);
                *err = *err||rep.terminationtype!=-3;
                
                /*
                 * Test interaction with constrains on entry states.
                 *
                 * When model has entry state, corresponding row of P
                 * must be zero. We try to set two kinds of constraints
                 * on elements of this row:
                 * * sums-to-zero constraint, which must be consistent
                 * * sums-to-one constraint, which must be inconsistent
                 */
                if( entrystate>=0 )
                {
                    ae_matrix_set_length(&c, 1, n*n+1, _state);
                    ae_vector_set_length(&ct, 1, _state);
                    for(i=0; i<=n*n-1; i++)
                    {
                        c.ptr.pp_double[0][i] = 0.0;
                    }
                    for(j=0; j<=n-1; j++)
                    {
                        c.ptr.pp_double[0][n*entrystate+j] = 1.0;
                    }
                    ct.ptr.p_int[0] = 0;
                    c.ptr.pp_double[0][n*n] = 0.0;
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdsetlc(&s, &c, &ct, 1, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype<=0;
                    c.ptr.pp_double[0][n*n] = 1.0;
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdsetlc(&s, &c, &ct, 1, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype!=-3;
                }
                
                /*
                 * Test interaction with constrains on exit states.
                 *
                 * When model has exit state, corresponding column of P
                 * must be zero. We try to set two kinds of constraints
                 * on elements of this column:
                 * * sums-to-zero constraint, which must be consistent
                 * * sums-to-one constraint, which must be inconsistent
                 */
                if( exitstate>=0 )
                {
                    ae_matrix_set_length(&c, 1, n*n+1, _state);
                    ae_vector_set_length(&ct, 1, _state);
                    for(i=0; i<=n*n-1; i++)
                    {
                        c.ptr.pp_double[0][i] = 0.0;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        c.ptr.pp_double[0][n*i+exitstate] = 1.0;
                    }
                    ct.ptr.p_int[0] = 0;
                    c.ptr.pp_double[0][n*n] = 0.0;
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdsetlc(&s, &c, &ct, 1, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype<=0;
                    c.ptr.pp_double[0][n*n] = 1.0;
                    testmcpdunit_createee(n, entrystate, exitstate, &s, _state);
                    mcpdaddtrack(&s, &xy, xy.rows, _state);
                    mcpdsetlc(&s, &c, &ct, 1, _state);
                    mcpdsolve(&s, _state);
                    mcpdresults(&s, &p, &rep, _state);
                    *err = *err||rep.terminationtype!=-3;
                }
            }
        }
    }
    
    /*
     * Final test - we generate several random constraints and
     * test SetLC() function.
     *
     * NOTES:
     *
     * 1. Constraints are generated is such a way that P=(1/N ... 1/N)
     *    is always feasible. It guarantees that there always exists
     *    at least one feasible point
     * 2. For simplicity of the test we do not use entry/exit states
     *    in our model
     */
    for(n=1; n<=4; n++)
    {
        for(k=1; k<=2*n; k++)
        {
            
            /*
             * Generate track
             */
            ae_matrix_set_length(&xy, 2*n, n, _state);
            for(i=0; i<=xy.rows-1; i++)
            {
                for(j=0; j<=xy.cols-1; j++)
                {
                    xy.ptr.pp_double[i][j] = ae_randomreal(_state);
                }
            }
            
            /*
             * Generate random constraints
             */
            ae_matrix_set_length(&c, k, n*n+1, _state);
            ae_vector_set_length(&ct, k, _state);
            for(i=0; i<=k-1; i++)
            {
                
                /*
                 * Generate constraint and its right part
                 */
                c.ptr.pp_double[i][n*n] = (double)(0);
                for(j=0; j<=n*n-1; j++)
                {
                    c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    c.ptr.pp_double[i][n*n] = c.ptr.pp_double[i][n*n]+c.ptr.pp_double[i][j]*((double)1/(double)n);
                }
                ct.ptr.p_int[i] = ae_randominteger(3, _state)-1;
                
                /*
                 * If we have inequality constraint, we "shift" right part
                 * in order to make feasible some neighborhood of P=(1/N ... 1/N).
                 */
                if( ct.ptr.p_int[i]<0 )
                {
                    c.ptr.pp_double[i][n*n] = c.ptr.pp_double[i][n*n]+0.1;
                }
                if( ct.ptr.p_int[i]>0 )
                {
                    c.ptr.pp_double[i][n*n] = c.ptr.pp_double[i][n*n]-0.1;
                }
            }
            
            /*
             * Test
             */
            testmcpdunit_createee(n, -1, -1, &s, _state);
            mcpdaddtrack(&s, &xy, xy.rows, _state);
            mcpdsetlc(&s, &c, &ct, k, _state);
            mcpdsolve(&s, _state);
            mcpdresults(&s, &p, &rep, _state);
            if( rep.terminationtype>0 )
            {
                for(t=0; t<=k-1; t++)
                {
                    v = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            v = v+p.ptr.pp_double[i][j]*c.ptr.pp_double[t][i*n+j];
                        }
                    }
                    if( ct.ptr.p_int[t]<0 )
                    {
                        *err = *err||ae_fp_greater_eq(v,c.ptr.pp_double[t][n*n]+threshold);
                    }
                    if( ct.ptr.p_int[t]==0 )
                    {
                        *err = *err||ae_fp_greater_eq(ae_fabs(v-c.ptr.pp_double[t][n*n], _state),threshold);
                    }
                    if( ct.ptr.p_int[t]>0 )
                    {
                        *err = *err||ae_fp_less_eq(v,c.ptr.pp_double[t][n*n]-threshold);
                    }
                }
            }
            else
            {
                *err = ae_true;
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function is used to create MCPD object with arbitrary combination of
entry and exit states
*************************************************************************/
static void testmcpdunit_createee(ae_int_t n,
     ae_int_t entrystate,
     ae_int_t exitstate,
     mcpdstate* s,
     ae_state *_state)
{

    _mcpdstate_clear(s);

    if( entrystate<0&&exitstate<0 )
    {
        mcpdcreate(n, s, _state);
    }
    if( entrystate>=0&&exitstate<0 )
    {
        mcpdcreateentry(n, entrystate, s, _state);
    }
    if( entrystate<0&&exitstate>=0 )
    {
        mcpdcreateexit(n, exitstate, s, _state);
    }
    if( entrystate>=0&&exitstate>=0 )
    {
        mcpdcreateentryexit(n, entrystate, exitstate, s, _state);
    }
}


/*$ End $*/
