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
#include "testnlequnit.h"


/*$ Declarations $*/
static void testnlequnit_testfunchbm(nleqstate* state, ae_state *_state);
static void testnlequnit_testfunchb1(nleqstate* state, ae_state *_state);
static void testnlequnit_testfuncshbm(nleqstate* state, ae_state *_state);


/*$ Body $*/


ae_bool testnleq(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_bool basicserrors;
    ae_bool converror;
    ae_bool othererrors;
    ae_int_t n;
    ae_vector x;
    ae_int_t i;
    ae_int_t k;
    double v;
    double flast;
    ae_vector xlast;
    ae_bool firstrep;
    ae_int_t nfunc;
    ae_int_t njac;
    ae_int_t itcnt;
    nleqstate state;
    nleqreport rep;
    ae_int_t pass;
    ae_int_t passcount;
    double epsf;
    double stpmax;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&xlast, 0, DT_REAL, _state);
    _nleqstate_init(&state, _state);
    _nleqreport_init(&rep, _state);

    waserrors = ae_false;
    basicserrors = ae_false;
    converror = ae_false;
    othererrors = ae_false;
    
    /*
     * Basic tests
     *
     * Test with Himmelblau's function (M):
     * * ability to find correct result
     * * ability to work after soft restart (restart after finish)
     * * ability to work after hard restart (restart in the middle of optimization)
     */
    passcount = 100;
    for(pass=0; pass<=passcount-1; pass++)
    {
        
        /*
         * Ability to find correct result
         */
        ae_vector_set_length(&x, 2, _state);
        x.ptr.p_double[0] = 20*ae_randomreal(_state)-10;
        x.ptr.p_double[1] = 20*ae_randomreal(_state)-10;
        nleqcreatelm(2, 2, &x, &state, _state);
        epsf = 1.0E-9;
        nleqsetcond(&state, epsf, 0, _state);
        while(nleqiteration(&state, _state))
        {
            testnlequnit_testfunchbm(&state, _state);
        }
        nleqresults(&state, &x, &rep, _state);
        if( rep.terminationtype>0 )
        {
            basicserrors = basicserrors||ae_fp_greater(ae_sqr(x.ptr.p_double[0]*x.ptr.p_double[0]+x.ptr.p_double[1]-11, _state)+ae_sqr(x.ptr.p_double[0]+x.ptr.p_double[1]*x.ptr.p_double[1]-7, _state),ae_sqr(epsf, _state));
        }
        else
        {
            basicserrors = ae_true;
        }
        
        /*
         * Ability to work after soft restart
         */
        ae_vector_set_length(&x, 2, _state);
        x.ptr.p_double[0] = 20*ae_randomreal(_state)-10;
        x.ptr.p_double[1] = 20*ae_randomreal(_state)-10;
        nleqcreatelm(2, 2, &x, &state, _state);
        epsf = 1.0E-9;
        nleqsetcond(&state, epsf, 0, _state);
        while(nleqiteration(&state, _state))
        {
            testnlequnit_testfunchbm(&state, _state);
        }
        nleqresults(&state, &x, &rep, _state);
        ae_vector_set_length(&x, 2, _state);
        x.ptr.p_double[0] = 20*ae_randomreal(_state)-10;
        x.ptr.p_double[1] = 20*ae_randomreal(_state)-10;
        nleqrestartfrom(&state, &x, _state);
        while(nleqiteration(&state, _state))
        {
            testnlequnit_testfunchbm(&state, _state);
        }
        nleqresults(&state, &x, &rep, _state);
        if( rep.terminationtype>0 )
        {
            basicserrors = basicserrors||ae_fp_greater(ae_sqr(x.ptr.p_double[0]*x.ptr.p_double[0]+x.ptr.p_double[1]-11, _state)+ae_sqr(x.ptr.p_double[0]+x.ptr.p_double[1]*x.ptr.p_double[1]-7, _state),ae_sqr(epsf, _state));
        }
        else
        {
            basicserrors = ae_true;
        }
        
        /*
         * Ability to work after hard restart:
         * * stopping condition: small F
         * * StpMax is so small that we need about 10000 iterations to
         *   find solution (steps are small)
         * * choose random K significantly less that 9999
         * * iterate for some time, then break, restart optimization
         */
        ae_vector_set_length(&x, 2, _state);
        x.ptr.p_double[0] = (double)(100);
        x.ptr.p_double[1] = (double)(100);
        nleqcreatelm(2, 2, &x, &state, _state);
        epsf = 1.0E-9;
        nleqsetcond(&state, epsf, 0, _state);
        nleqsetstpmax(&state, 0.01, _state);
        k = 1+ae_randominteger(100, _state);
        for(i=0; i<=k-1; i++)
        {
            if( !nleqiteration(&state, _state) )
            {
                break;
            }
            testnlequnit_testfunchbm(&state, _state);
        }
        ae_vector_set_length(&x, 2, _state);
        x.ptr.p_double[0] = 20*ae_randomreal(_state)-10;
        x.ptr.p_double[1] = 20*ae_randomreal(_state)-10;
        nleqrestartfrom(&state, &x, _state);
        while(nleqiteration(&state, _state))
        {
            testnlequnit_testfunchbm(&state, _state);
        }
        nleqresults(&state, &x, &rep, _state);
        if( rep.terminationtype>0 )
        {
            basicserrors = basicserrors||ae_fp_greater(ae_sqr(x.ptr.p_double[0]*x.ptr.p_double[0]+x.ptr.p_double[1]-11, _state)+ae_sqr(x.ptr.p_double[0]+x.ptr.p_double[1]*x.ptr.p_double[1]-7, _state),ae_sqr(epsf, _state));
        }
        else
        {
            basicserrors = ae_true;
        }
    }
    
    /*
     * Basic tests
     *
     * Test with Himmelblau's function (1):
     * * ability to find correct result
     */
    passcount = 100;
    for(pass=0; pass<=passcount-1; pass++)
    {
        
        /*
         * Ability to find correct result
         */
        ae_vector_set_length(&x, 2, _state);
        x.ptr.p_double[0] = 20*ae_randomreal(_state)-10;
        x.ptr.p_double[1] = 20*ae_randomreal(_state)-10;
        nleqcreatelm(2, 1, &x, &state, _state);
        epsf = 1.0E-9;
        nleqsetcond(&state, epsf, 0, _state);
        while(nleqiteration(&state, _state))
        {
            testnlequnit_testfunchb1(&state, _state);
        }
        nleqresults(&state, &x, &rep, _state);
        if( rep.terminationtype>0 )
        {
            basicserrors = basicserrors||ae_fp_greater(ae_sqr(x.ptr.p_double[0]*x.ptr.p_double[0]+x.ptr.p_double[1]-11, _state)+ae_sqr(x.ptr.p_double[0]+x.ptr.p_double[1]*x.ptr.p_double[1]-7, _state),epsf);
        }
        else
        {
            basicserrors = ae_true;
        }
    }
    
    /*
     * Basic tests
     *
     * Ability to detect situation when we can't find minimum
     */
    passcount = 100;
    for(pass=0; pass<=passcount-1; pass++)
    {
        ae_vector_set_length(&x, 2, _state);
        x.ptr.p_double[0] = 20*ae_randomreal(_state)-10;
        x.ptr.p_double[1] = 20*ae_randomreal(_state)-10;
        nleqcreatelm(2, 3, &x, &state, _state);
        epsf = 1.0E-9;
        nleqsetcond(&state, epsf, 0, _state);
        while(nleqiteration(&state, _state))
        {
            testnlequnit_testfuncshbm(&state, _state);
        }
        nleqresults(&state, &x, &rep, _state);
        basicserrors = basicserrors||rep.terminationtype!=-4;
    }
    
    /*
     * Test correctness of intermediate reports and final report:
     * * first report is starting point
     * * function value decreases on subsequent reports
     * * function value is correctly reported
     * * last report is final point
     * * NFunc and NJac are compared with values counted directly
     * * IterationsCount is compared with value counter directly
     */
    n = 2;
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&xlast, n, _state);
    x.ptr.p_double[0] = 20*ae_randomreal(_state)-10;
    x.ptr.p_double[1] = 20*ae_randomreal(_state)-10;
    xlast.ptr.p_double[0] = ae_maxrealnumber;
    xlast.ptr.p_double[1] = ae_maxrealnumber;
    nleqcreatelm(n, 2, &x, &state, _state);
    nleqsetcond(&state, 1.0E-6, 0, _state);
    nleqsetxrep(&state, ae_true, _state);
    firstrep = ae_true;
    flast = ae_maxrealnumber;
    nfunc = 0;
    njac = 0;
    itcnt = 0;
    while(nleqiteration(&state, _state))
    {
        if( state.xupdated )
        {
            
            /*
             * first report must be starting point
             */
            if( firstrep )
            {
                for(i=0; i<=n-1; i++)
                {
                    othererrors = othererrors||ae_fp_neq(state.x.ptr.p_double[i],x.ptr.p_double[i]);
                }
                firstrep = ae_false;
            }
            
            /*
             * function value must decrease
             */
            othererrors = othererrors||ae_fp_greater(state.f,flast);
            
            /*
             * check correctness of function value
             */
            v = ae_sqr(state.x.ptr.p_double[0]*state.x.ptr.p_double[0]+state.x.ptr.p_double[1]-11, _state)+ae_sqr(state.x.ptr.p_double[0]+state.x.ptr.p_double[1]*state.x.ptr.p_double[1]-7, _state);
            othererrors = othererrors||ae_fp_greater(ae_fabs(v-state.f, _state)/ae_maxreal(v, (double)(1), _state),100*ae_machineepsilon);
            
            /*
             * update info and continue
             */
            ae_v_move(&xlast.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
            flast = state.f;
            itcnt = itcnt+1;
            continue;
        }
        if( state.needf )
        {
            nfunc = nfunc+1;
        }
        if( state.needfij )
        {
            nfunc = nfunc+1;
            njac = njac+1;
        }
        testnlequnit_testfunchbm(&state, _state);
    }
    nleqresults(&state, &x, &rep, _state);
    if( rep.terminationtype>0 )
    {
        othererrors = (othererrors||ae_fp_neq(xlast.ptr.p_double[0],x.ptr.p_double[0]))||ae_fp_neq(xlast.ptr.p_double[1],x.ptr.p_double[1]);
        v = ae_sqr(x.ptr.p_double[0]*x.ptr.p_double[0]+x.ptr.p_double[1]-11, _state)+ae_sqr(x.ptr.p_double[0]+x.ptr.p_double[1]*x.ptr.p_double[1]-7, _state);
        othererrors = othererrors||ae_fp_greater(ae_fabs(flast-v, _state)/ae_maxreal(v, (double)(1), _state),100*ae_machineepsilon);
    }
    else
    {
        converror = ae_true;
    }
    othererrors = othererrors||rep.nfunc!=nfunc;
    othererrors = othererrors||rep.njac!=njac;
    othererrors = othererrors||rep.iterationscount!=itcnt-1;
    
    /*
     * Test ability to set limit on algorithm steps
     */
    ae_vector_set_length(&x, 2, _state);
    ae_vector_set_length(&xlast, 2, _state);
    x.ptr.p_double[0] = 20*ae_randomreal(_state)+20;
    x.ptr.p_double[1] = 20*ae_randomreal(_state)+20;
    xlast.ptr.p_double[0] = x.ptr.p_double[0];
    xlast.ptr.p_double[1] = x.ptr.p_double[1];
    stpmax = 0.1+0.1*ae_randomreal(_state);
    epsf = 1.0E-9;
    nleqcreatelm(2, 3, &x, &state, _state);
    nleqsetstpmax(&state, stpmax, _state);
    nleqsetcond(&state, epsf, 0, _state);
    nleqsetxrep(&state, ae_true, _state);
    while(nleqiteration(&state, _state))
    {
        if( state.needf||state.needfij )
        {
            testnlequnit_testfunchbm(&state, _state);
        }
        if( (state.needf||state.needfij)||state.xupdated )
        {
            othererrors = othererrors||ae_fp_greater(ae_sqrt(ae_sqr(state.x.ptr.p_double[0]-xlast.ptr.p_double[0], _state)+ae_sqr(state.x.ptr.p_double[1]-xlast.ptr.p_double[1], _state), _state),1.00001*stpmax);
        }
        if( state.xupdated )
        {
            xlast.ptr.p_double[0] = state.x.ptr.p_double[0];
            xlast.ptr.p_double[1] = state.x.ptr.p_double[1];
        }
    }
    
    /*
     * end
     */
    waserrors = (basicserrors||converror)||othererrors;
    if( !silent )
    {
        printf("TESTING NLEQ SOLVER\n");
        printf("BASIC FUNCTIONALITY:                      ");
        if( basicserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("CONVERGENCE:                              ");
        if( converror )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("OTHER PROPERTIES:                         ");
        if( othererrors )
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
        printf("\n\n");
    }
    result = !waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testnleq(ae_bool silent, ae_state *_state)
{
    return testnleq(silent, _state);
}


/*************************************************************************
Himmelblau's function

    F = (x^2+y-11)^2 + (x+y^2-7)^2

posed as system of M functions:

    f0 = x^2+y-11
    f1 = x+y^2-7

*************************************************************************/
static void testnlequnit_testfunchbm(nleqstate* state, ae_state *_state)
{
    double x;
    double y;


    ae_assert(state->needf||state->needfij, "TestNLEQUnit: internal error!", _state);
    x = state->x.ptr.p_double[0];
    y = state->x.ptr.p_double[1];
    if( state->needf )
    {
        state->f = ae_sqr(x*x+y-11, _state)+ae_sqr(x+y*y-7, _state);
        return;
    }
    if( state->needfij )
    {
        state->fi.ptr.p_double[0] = x*x+y-11;
        state->fi.ptr.p_double[1] = x+y*y-7;
        state->j.ptr.pp_double[0][0] = 2*x;
        state->j.ptr.pp_double[0][1] = (double)(1);
        state->j.ptr.pp_double[1][0] = (double)(1);
        state->j.ptr.pp_double[1][1] = 2*y;
        return;
    }
}


/*************************************************************************
Himmelblau's function

    F = (x^2+y-11)^2 + (x+y^2-7)^2

posed as system of 1 function
*************************************************************************/
static void testnlequnit_testfunchb1(nleqstate* state, ae_state *_state)
{
    double x;
    double y;


    ae_assert(state->needf||state->needfij, "TestNLEQUnit: internal error!", _state);
    x = state->x.ptr.p_double[0];
    y = state->x.ptr.p_double[1];
    if( state->needf )
    {
        state->f = ae_sqr(ae_sqr(x*x+y-11, _state)+ae_sqr(x+y*y-7, _state), _state);
        return;
    }
    if( state->needfij )
    {
        state->fi.ptr.p_double[0] = ae_sqr(x*x+y-11, _state)+ae_sqr(x+y*y-7, _state);
        state->j.ptr.pp_double[0][0] = 2*(x*x+y-11)*2*x+2*(x+y*y-7);
        state->j.ptr.pp_double[0][1] = 2*(x*x+y-11)+2*(x+y*y-7)*2*y;
        return;
    }
}


/*************************************************************************
Shifted Himmelblau's function

    F = (x^2+y-11)^2 + (x+y^2-7)^2 + 1

posed as system of M functions:

    f0 = x^2+y-11
    f1 = x+y^2-7
    f2 = 1

This function is used to test algorithm on problem which has no solution.
*************************************************************************/
static void testnlequnit_testfuncshbm(nleqstate* state, ae_state *_state)
{
    double x;
    double y;


    ae_assert(state->needf||state->needfij, "TestNLEQUnit: internal error!", _state);
    x = state->x.ptr.p_double[0];
    y = state->x.ptr.p_double[1];
    if( state->needf )
    {
        state->f = ae_sqr(x*x+y-11, _state)+ae_sqr(x+y*y-7, _state)+1;
        return;
    }
    if( state->needfij )
    {
        state->fi.ptr.p_double[0] = x*x+y-11;
        state->fi.ptr.p_double[1] = x+y*y-7;
        state->fi.ptr.p_double[2] = (double)(1);
        state->j.ptr.pp_double[0][0] = 2*x;
        state->j.ptr.pp_double[0][1] = (double)(1);
        state->j.ptr.pp_double[1][0] = (double)(1);
        state->j.ptr.pp_double[1][1] = 2*y;
        state->j.ptr.pp_double[2][0] = (double)(0);
        state->j.ptr.pp_double[2][1] = (double)(0);
        return;
    }
}


/*$ End $*/
