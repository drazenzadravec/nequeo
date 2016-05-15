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
#include "testminlbfgsunit.h"


/*$ Declarations $*/
static void testminlbfgsunit_testfunc2(minlbfgsstate* state,
     ae_state *_state);
static void testminlbfgsunit_testfunc3(minlbfgsstate* state,
     ae_state *_state);
static void testminlbfgsunit_calciip2(minlbfgsstate* state,
     ae_int_t n,
     ae_state *_state);
static void testminlbfgsunit_testpreconditioning(ae_bool* err,
     ae_state *_state);
static void testminlbfgsunit_testother(ae_bool* err, ae_state *_state);
static ae_bool testminlbfgsunit_gradientchecktest(ae_state *_state);
static void testminlbfgsunit_funcderiv(double a,
     double b,
     double c,
     double d,
     double x0,
     double x1,
     double x2,
     /* Real    */ ae_vector* x,
     ae_int_t functype,
     double* f,
     /* Real    */ ae_vector* g,
     ae_state *_state);


/*$ Body $*/


ae_bool testminlbfgs(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_bool referror;
    ae_bool nonconverror;
    ae_bool eqerror;
    ae_bool converror;
    ae_bool crashtest;
    ae_bool othererrors;
    ae_bool restartserror;
    ae_bool precerror;
    ae_bool graderrors;
    ae_int_t n;
    ae_int_t m;
    ae_vector x;
    ae_vector xe;
    ae_vector b;
    ae_vector xlast;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_matrix a;
    ae_vector diagh;
    ae_int_t maxits;
    minlbfgsstate state;
    minlbfgsreport rep;
    double diffstep;
    ae_int_t dkind;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&xe, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&xlast, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&diagh, 0, DT_REAL, _state);
    _minlbfgsstate_init(&state, _state);
    _minlbfgsreport_init(&rep, _state);

    waserrors = ae_false;
    referror = ae_false;
    precerror = ae_false;
    nonconverror = ae_false;
    restartserror = ae_false;
    eqerror = ae_false;
    converror = ae_false;
    crashtest = ae_false;
    othererrors = ae_false;
    testminlbfgsunit_testpreconditioning(&precerror, _state);
    testminlbfgsunit_testother(&othererrors, _state);
    
    /*
     * Reference problem
     */
    diffstep = 1.0E-6;
    for(dkind=0; dkind<=1; dkind++)
    {
        ae_vector_set_length(&x, 3, _state);
        n = 3;
        m = 2;
        x.ptr.p_double[0] = 100*ae_randomreal(_state)-50;
        x.ptr.p_double[1] = 100*ae_randomreal(_state)-50;
        x.ptr.p_double[2] = 100*ae_randomreal(_state)-50;
        if( dkind==0 )
        {
            minlbfgscreate(n, m, &x, &state, _state);
        }
        if( dkind==1 )
        {
            minlbfgscreatef(n, m, &x, diffstep, &state, _state);
        }
        minlbfgssetcond(&state, (double)(0), (double)(0), (double)(0), 0, _state);
        while(minlbfgsiteration(&state, _state))
        {
            if( state.needf||state.needfg )
            {
                state.f = ae_sqr(state.x.ptr.p_double[0]-2, _state)+ae_sqr(state.x.ptr.p_double[1], _state)+ae_sqr(state.x.ptr.p_double[2]-state.x.ptr.p_double[0], _state);
            }
            if( state.needfg )
            {
                state.g.ptr.p_double[0] = 2*(state.x.ptr.p_double[0]-2)+2*(state.x.ptr.p_double[0]-state.x.ptr.p_double[2]);
                state.g.ptr.p_double[1] = 2*state.x.ptr.p_double[1];
                state.g.ptr.p_double[2] = 2*(state.x.ptr.p_double[2]-state.x.ptr.p_double[0]);
            }
        }
        minlbfgsresults(&state, &x, &rep, _state);
        referror = (((referror||rep.terminationtype<=0)||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-2, _state),0.001))||ae_fp_greater(ae_fabs(x.ptr.p_double[1], _state),0.001))||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-2, _state),0.001);
    }
    
    /*
     * nonconvex problems with complex surface: we start from point with very small
     * gradient, but we need ever smaller gradient in the next step due to
     * Wolfe conditions.
     */
    diffstep = 1.0E-6;
    for(dkind=0; dkind<=1; dkind++)
    {
        ae_vector_set_length(&x, 1, _state);
        n = 1;
        m = 1;
        v = (double)(-100);
        while(ae_fp_less(v,0.1))
        {
            x.ptr.p_double[0] = v;
            if( dkind==0 )
            {
                minlbfgscreate(n, m, &x, &state, _state);
            }
            if( dkind==1 )
            {
                minlbfgscreatef(n, m, &x, diffstep, &state, _state);
            }
            minlbfgssetcond(&state, 1.0E-9, (double)(0), (double)(0), 0, _state);
            while(minlbfgsiteration(&state, _state))
            {
                if( state.needf||state.needfg )
                {
                    state.f = ae_sqr(state.x.ptr.p_double[0], _state)/(1+ae_sqr(state.x.ptr.p_double[0], _state));
                }
                if( state.needfg )
                {
                    state.g.ptr.p_double[0] = (2*state.x.ptr.p_double[0]*(1+ae_sqr(state.x.ptr.p_double[0], _state))-ae_sqr(state.x.ptr.p_double[0], _state)*2*state.x.ptr.p_double[0])/ae_sqr(1+ae_sqr(state.x.ptr.p_double[0], _state), _state);
                }
            }
            minlbfgsresults(&state, &x, &rep, _state);
            nonconverror = (nonconverror||rep.terminationtype<=0)||ae_fp_greater(ae_fabs(x.ptr.p_double[0], _state),0.001);
            v = v+0.1;
        }
    }
    
    /*
     * F2 problem with restarts:
     * * make several iterations and restart BEFORE termination
     * * iterate and restart AFTER termination
     *
     * NOTE: step is bounded from above to avoid premature convergence
     */
    diffstep = 1.0E-6;
    for(dkind=0; dkind<=1; dkind++)
    {
        ae_vector_set_length(&x, 3, _state);
        n = 3;
        m = 2;
        x.ptr.p_double[0] = 10+10*ae_randomreal(_state);
        x.ptr.p_double[1] = 10+10*ae_randomreal(_state);
        x.ptr.p_double[2] = 10+10*ae_randomreal(_state);
        if( dkind==0 )
        {
            minlbfgscreate(n, m, &x, &state, _state);
        }
        if( dkind==1 )
        {
            minlbfgscreatef(n, m, &x, diffstep, &state, _state);
        }
        minlbfgssetstpmax(&state, 0.1, _state);
        minlbfgssetcond(&state, 0.0000001, 0.0, 0.0, 0, _state);
        for(i=0; i<=10; i++)
        {
            if( !minlbfgsiteration(&state, _state) )
            {
                break;
            }
            testminlbfgsunit_testfunc2(&state, _state);
        }
        x.ptr.p_double[0] = 10+10*ae_randomreal(_state);
        x.ptr.p_double[1] = 10+10*ae_randomreal(_state);
        x.ptr.p_double[2] = 10+10*ae_randomreal(_state);
        minlbfgsrestartfrom(&state, &x, _state);
        while(minlbfgsiteration(&state, _state))
        {
            testminlbfgsunit_testfunc2(&state, _state);
        }
        minlbfgsresults(&state, &x, &rep, _state);
        restartserror = (((restartserror||rep.terminationtype<=0)||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-ae_log((double)(2), _state), _state),0.01))||ae_fp_greater(ae_fabs(x.ptr.p_double[1], _state),0.01))||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-ae_log((double)(2), _state), _state),0.01);
        x.ptr.p_double[0] = 10+10*ae_randomreal(_state);
        x.ptr.p_double[1] = 10+10*ae_randomreal(_state);
        x.ptr.p_double[2] = 10+10*ae_randomreal(_state);
        minlbfgsrestartfrom(&state, &x, _state);
        while(minlbfgsiteration(&state, _state))
        {
            testminlbfgsunit_testfunc2(&state, _state);
        }
        minlbfgsresults(&state, &x, &rep, _state);
        restartserror = (((restartserror||rep.terminationtype<=0)||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-ae_log((double)(2), _state), _state),0.01))||ae_fp_greater(ae_fabs(x.ptr.p_double[1], _state),0.01))||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-ae_log((double)(2), _state), _state),0.01);
    }
    
    /*
     * Linear equations
     */
    diffstep = 1.0E-6;
    for(n=1; n<=10; n++)
    {
        
        /*
         * Prepare task
         */
        ae_matrix_set_length(&a, n-1+1, n-1+1, _state);
        ae_vector_set_length(&x, n-1+1, _state);
        ae_vector_set_length(&xe, n-1+1, _state);
        ae_vector_set_length(&b, n-1+1, _state);
        for(i=0; i<=n-1; i++)
        {
            xe.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            a.ptr.pp_double[i][i] = a.ptr.pp_double[i][i]+3*ae_sign(a.ptr.pp_double[i][i], _state);
        }
        for(i=0; i<=n-1; i++)
        {
            v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xe.ptr.p_double[0], 1, ae_v_len(0,n-1));
            b.ptr.p_double[i] = v;
        }
        
        /*
         * Test different M/DKind
         */
        for(m=1; m<=n; m++)
        {
            for(dkind=0; dkind<=1; dkind++)
            {
                
                /*
                 * Solve task
                 */
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                if( dkind==0 )
                {
                    minlbfgscreate(n, m, &x, &state, _state);
                }
                if( dkind==1 )
                {
                    minlbfgscreatef(n, m, &x, diffstep, &state, _state);
                }
                minlbfgssetcond(&state, (double)(0), (double)(0), (double)(0), 0, _state);
                while(minlbfgsiteration(&state, _state))
                {
                    if( state.needf||state.needfg )
                    {
                        state.f = (double)(0);
                    }
                    if( state.needfg )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            state.g.ptr.p_double[i] = (double)(0);
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                        if( state.needf||state.needfg )
                        {
                            state.f = state.f+ae_sqr(v-b.ptr.p_double[i], _state);
                        }
                        if( state.needfg )
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                state.g.ptr.p_double[j] = state.g.ptr.p_double[j]+2*(v-b.ptr.p_double[i])*a.ptr.pp_double[i][j];
                            }
                        }
                    }
                }
                minlbfgsresults(&state, &x, &rep, _state);
                eqerror = eqerror||rep.terminationtype<=0;
                for(i=0; i<=n-1; i++)
                {
                    eqerror = eqerror||ae_fp_greater(ae_fabs(x.ptr.p_double[i]-xe.ptr.p_double[i], _state),0.001);
                }
            }
        }
    }
    
    /*
     * Testing convergence properties
     */
    diffstep = 1.0E-6;
    for(dkind=0; dkind<=1; dkind++)
    {
        ae_vector_set_length(&x, 3, _state);
        n = 3;
        m = 2;
        for(i=0; i<=2; i++)
        {
            x.ptr.p_double[i] = 6*ae_randomreal(_state)-3;
        }
        if( dkind==0 )
        {
            minlbfgscreate(n, m, &x, &state, _state);
        }
        if( dkind==1 )
        {
            minlbfgscreatef(n, m, &x, diffstep, &state, _state);
        }
        minlbfgssetcond(&state, 0.001, (double)(0), (double)(0), 0, _state);
        while(minlbfgsiteration(&state, _state))
        {
            testminlbfgsunit_testfunc3(&state, _state);
        }
        minlbfgsresults(&state, &x, &rep, _state);
        converror = converror||rep.terminationtype!=4;
        for(i=0; i<=2; i++)
        {
            x.ptr.p_double[i] = 6*ae_randomreal(_state)-3;
        }
        if( dkind==0 )
        {
            minlbfgscreate(n, m, &x, &state, _state);
        }
        if( dkind==1 )
        {
            minlbfgscreatef(n, m, &x, diffstep, &state, _state);
        }
        minlbfgssetcond(&state, (double)(0), 0.001, (double)(0), 0, _state);
        while(minlbfgsiteration(&state, _state))
        {
            testminlbfgsunit_testfunc3(&state, _state);
        }
        minlbfgsresults(&state, &x, &rep, _state);
        converror = converror||rep.terminationtype!=1;
        for(i=0; i<=2; i++)
        {
            x.ptr.p_double[i] = 6*ae_randomreal(_state)-3;
        }
        if( dkind==0 )
        {
            minlbfgscreate(n, m, &x, &state, _state);
        }
        if( dkind==1 )
        {
            minlbfgscreatef(n, m, &x, diffstep, &state, _state);
        }
        minlbfgssetcond(&state, (double)(0), (double)(0), 0.001, 0, _state);
        while(minlbfgsiteration(&state, _state))
        {
            testminlbfgsunit_testfunc3(&state, _state);
        }
        minlbfgsresults(&state, &x, &rep, _state);
        converror = converror||rep.terminationtype!=2;
        for(i=0; i<=2; i++)
        {
            x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        if( dkind==0 )
        {
            minlbfgscreate(n, m, &x, &state, _state);
        }
        if( dkind==1 )
        {
            minlbfgscreatef(n, m, &x, diffstep, &state, _state);
        }
        minlbfgssetcond(&state, (double)(0), (double)(0), (double)(0), 10, _state);
        while(minlbfgsiteration(&state, _state))
        {
            testminlbfgsunit_testfunc3(&state, _state);
        }
        minlbfgsresults(&state, &x, &rep, _state);
        converror = (converror||rep.terminationtype!=5)||rep.iterationscount!=10;
    }
    
    /*
     * Crash test: too many iterations on a simple tasks
     * May fail when encounter zero step, underflow or something like that
     */
    ae_vector_set_length(&x, 2+1, _state);
    n = 3;
    m = 2;
    maxits = 10000;
    for(i=0; i<=2; i++)
    {
        x.ptr.p_double[i] = 6*ae_randomreal(_state)-3;
    }
    minlbfgscreate(n, m, &x, &state, _state);
    minlbfgssetcond(&state, (double)(0), (double)(0), (double)(0), maxits, _state);
    while(minlbfgsiteration(&state, _state))
    {
        state.f = ae_sqr(ae_exp(state.x.ptr.p_double[0], _state)-2, _state)+ae_sqr(state.x.ptr.p_double[1], _state)+ae_sqr(state.x.ptr.p_double[2]-state.x.ptr.p_double[0], _state);
        state.g.ptr.p_double[0] = 2*(ae_exp(state.x.ptr.p_double[0], _state)-2)*ae_exp(state.x.ptr.p_double[0], _state)+2*(state.x.ptr.p_double[0]-state.x.ptr.p_double[2]);
        state.g.ptr.p_double[1] = 2*state.x.ptr.p_double[1];
        state.g.ptr.p_double[2] = 2*(state.x.ptr.p_double[2]-state.x.ptr.p_double[0]);
    }
    minlbfgsresults(&state, &x, &rep, _state);
    crashtest = crashtest||rep.terminationtype<=0;
    
    /*
     *  Test for MinLBFGSGradientCheck
     */
    graderrors = testminlbfgsunit_gradientchecktest(_state);
    
    /*
     * end
     */
    waserrors = (((((((referror||nonconverror)||eqerror)||converror)||crashtest)||othererrors)||restartserror)||precerror)||graderrors;
    if( !silent )
    {
        printf("TESTING L-BFGS OPTIMIZATION\n");
        printf("REFERENCE PROBLEM:                        ");
        if( referror )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("NON-CONVEX PROBLEM:                       ");
        if( nonconverror )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("LINEAR EQUATIONS:                         ");
        if( eqerror )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("RESTARTS:                                 ");
        if( restartserror )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("PRECONDITIONER:                           ");
        if( precerror )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("CONVERGENCE PROPERTIES:                   ");
        if( converror )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("CRASH TEST:                               ");
        if( crashtest )
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
        printf("TEST FOR VERIFICATION OF THE GRADIENT:    ");
        if( graderrors )
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
ae_bool _pexec_testminlbfgs(ae_bool silent, ae_state *_state)
{
    return testminlbfgs(silent, _state);
}


/*************************************************************************
Calculate test function #2

Simple variation of #1, much more nonlinear, which makes unlikely premature
convergence of algorithm .
*************************************************************************/
static void testminlbfgsunit_testfunc2(minlbfgsstate* state,
     ae_state *_state)
{


    if( ae_fp_less(state->x.ptr.p_double[0],(double)(100)) )
    {
        if( state->needf||state->needfg )
        {
            state->f = ae_sqr(ae_exp(state->x.ptr.p_double[0], _state)-2, _state)+ae_sqr(ae_sqr(state->x.ptr.p_double[1], _state), _state)+ae_sqr(state->x.ptr.p_double[2]-state->x.ptr.p_double[0], _state);
        }
        if( state->needfg )
        {
            state->g.ptr.p_double[0] = 2*(ae_exp(state->x.ptr.p_double[0], _state)-2)*ae_exp(state->x.ptr.p_double[0], _state)+2*(state->x.ptr.p_double[0]-state->x.ptr.p_double[2]);
            state->g.ptr.p_double[1] = 4*state->x.ptr.p_double[1]*ae_sqr(state->x.ptr.p_double[1], _state);
            state->g.ptr.p_double[2] = 2*(state->x.ptr.p_double[2]-state->x.ptr.p_double[0]);
        }
    }
    else
    {
        if( state->needf||state->needfg )
        {
            state->f = ae_sqrt(ae_maxrealnumber, _state);
        }
        if( state->needfg )
        {
            state->g.ptr.p_double[0] = ae_sqrt(ae_maxrealnumber, _state);
            state->g.ptr.p_double[1] = (double)(0);
            state->g.ptr.p_double[2] = (double)(0);
        }
    }
}


/*************************************************************************
Calculate test function #3

Simple variation of #1, much more nonlinear, with non-zero value at minimum.
It achieve two goals:
* makes unlikely premature convergence of algorithm .
* solves some issues with EpsF stopping condition which arise when
  F(minimum) is zero

*************************************************************************/
static void testminlbfgsunit_testfunc3(minlbfgsstate* state,
     ae_state *_state)
{
    double s;


    s = 0.001;
    if( ae_fp_less(state->x.ptr.p_double[0],(double)(100)) )
    {
        if( state->needf||state->needfg )
        {
            state->f = ae_sqr(ae_exp(state->x.ptr.p_double[0], _state)-2, _state)+ae_sqr(ae_sqr(state->x.ptr.p_double[1], _state)+s, _state)+ae_sqr(state->x.ptr.p_double[2]-state->x.ptr.p_double[0], _state);
        }
        if( state->needfg )
        {
            state->g.ptr.p_double[0] = 2*(ae_exp(state->x.ptr.p_double[0], _state)-2)*ae_exp(state->x.ptr.p_double[0], _state)+2*(state->x.ptr.p_double[0]-state->x.ptr.p_double[2]);
            state->g.ptr.p_double[1] = 2*(ae_sqr(state->x.ptr.p_double[1], _state)+s)*2*state->x.ptr.p_double[1];
            state->g.ptr.p_double[2] = 2*(state->x.ptr.p_double[2]-state->x.ptr.p_double[0]);
        }
    }
    else
    {
        if( state->needf||state->needfg )
        {
            state->f = ae_sqrt(ae_maxrealnumber, _state);
        }
        if( state->needfg )
        {
            state->g.ptr.p_double[0] = ae_sqrt(ae_maxrealnumber, _state);
            state->g.ptr.p_double[1] = (double)(0);
            state->g.ptr.p_double[2] = (double)(0);
        }
    }
}


/*************************************************************************
Calculate test function IIP2

f(x) = sum( ((i*i+1)*x[i])^2, i=0..N-1)

It has high condition number which makes fast convergence unlikely without
good preconditioner.

*************************************************************************/
static void testminlbfgsunit_calciip2(minlbfgsstate* state,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;


    if( state->needf||state->needfg )
    {
        state->f = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        if( state->needf||state->needfg )
        {
            state->f = state->f+ae_sqr((double)(i*i+1), _state)*ae_sqr(state->x.ptr.p_double[i], _state);
        }
        if( state->needfg )
        {
            state->g.ptr.p_double[i] = ae_sqr((double)(i*i+1), _state)*2*state->x.ptr.p_double[i];
        }
    }
}


/*************************************************************************
This function tests preconditioning

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testminlbfgsunit_testpreconditioning(ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pass;
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t cntb1;
    ae_int_t cntb2;
    ae_int_t cntg1;
    ae_int_t cntg2;
    double epsg;
    ae_int_t pkind;
    minlbfgsstate state;
    minlbfgsreport rep;
    ae_vector x;
    ae_vector s;
    ae_matrix a;
    ae_vector diagh;

    ae_frame_make(_state, &_frame_block);
    _minlbfgsstate_init(&state, _state);
    _minlbfgsreport_init(&rep, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&diagh, 0, DT_REAL, _state);

    m = 1;
    k = 50;
    epsg = 1.0E-10;
    
    /*
     * Preconditioner test1.
     *
     * If
     * * B1 is default preconditioner
     * * B2 is Cholesky preconditioner with unit diagonal
     * * G1 is Cholesky preconditioner based on exact Hessian with perturbations
     * * G2 is diagonal precomditioner based on approximate diagonal of Hessian matrix
     * then "bad" preconditioners (B1/B2/..) are worse than "good" ones (G1/G2/..).
     * "Worse" means more iterations to converge.
     *
     * We test it using f(x) = sum( ((i*i+1)*x[i])^2, i=0..N-1) and L-BFGS
     * optimizer with deliberately small M=1.
     *
     * N        - problem size
     * PKind    - zero for upper triangular preconditioner, one for lower triangular.
     * K        - number of repeated passes (should be large enough to average out random factors)
     */
    for(n=10; n<=15; n++)
    {
        pkind = ae_randominteger(2, _state);
        ae_vector_set_length(&x, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = (double)(0);
        }
        minlbfgscreate(n, m, &x, &state, _state);
        
        /*
         * Test it with default preconditioner
         */
        minlbfgssetprecdefault(&state, _state);
        cntb1 = 0;
        for(pass=0; pass<=k-1; pass++)
        {
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            minlbfgsrestartfrom(&state, &x, _state);
            while(minlbfgsiteration(&state, _state))
            {
                testminlbfgsunit_calciip2(&state, n, _state);
            }
            minlbfgsresults(&state, &x, &rep, _state);
            cntb1 = cntb1+rep.iterationscount;
            *err = *err||rep.terminationtype<=0;
        }
        
        /*
         * Test it with unit preconditioner
         */
        ae_matrix_set_length(&a, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( i==j )
                {
                    a.ptr.pp_double[i][i] = (double)(1);
                }
                else
                {
                    a.ptr.pp_double[i][j] = (double)(0);
                }
            }
        }
        minlbfgssetpreccholesky(&state, &a, pkind==0, _state);
        cntb2 = 0;
        for(pass=0; pass<=k-1; pass++)
        {
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            minlbfgsrestartfrom(&state, &x, _state);
            while(minlbfgsiteration(&state, _state))
            {
                testminlbfgsunit_calciip2(&state, n, _state);
            }
            minlbfgsresults(&state, &x, &rep, _state);
            cntb2 = cntb2+rep.iterationscount;
            *err = *err||rep.terminationtype<=0;
        }
        
        /*
         * Test it with perturbed Hessian preconditioner
         */
        ae_matrix_set_length(&a, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( i==j )
                {
                    a.ptr.pp_double[i][i] = (i*i+1)*(0.8+0.4*ae_randomreal(_state));
                }
                else
                {
                    if( (pkind==0&&j>i)||(pkind==1&&j<i) )
                    {
                        a.ptr.pp_double[i][j] = 0.1*ae_randomreal(_state)-0.05;
                    }
                    else
                    {
                        a.ptr.pp_double[i][j] = _state->v_nan;
                    }
                }
            }
        }
        minlbfgssetpreccholesky(&state, &a, pkind==0, _state);
        cntg1 = 0;
        for(pass=0; pass<=k-1; pass++)
        {
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            minlbfgsrestartfrom(&state, &x, _state);
            while(minlbfgsiteration(&state, _state))
            {
                testminlbfgsunit_calciip2(&state, n, _state);
            }
            minlbfgsresults(&state, &x, &rep, _state);
            cntg1 = cntg1+rep.iterationscount;
            *err = *err||rep.terminationtype<=0;
        }
        
        /*
         * Test it with perturbed diagonal preconditioner
         */
        ae_vector_set_length(&diagh, n, _state);
        for(i=0; i<=n-1; i++)
        {
            diagh.ptr.p_double[i] = 2*ae_sqr((double)(i*i+1), _state)*(0.8+0.4*ae_randomreal(_state));
        }
        minlbfgssetprecdiag(&state, &diagh, _state);
        cntg2 = 0;
        for(pass=0; pass<=k-1; pass++)
        {
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            minlbfgsrestartfrom(&state, &x, _state);
            while(minlbfgsiteration(&state, _state))
            {
                testminlbfgsunit_calciip2(&state, n, _state);
            }
            minlbfgsresults(&state, &x, &rep, _state);
            cntg2 = cntg2+rep.iterationscount;
            *err = *err||rep.terminationtype<=0;
        }
        
        /*
         * Compare
         */
        *err = *err||cntb1<cntg1;
        *err = *err||cntb2<cntg1;
        *err = *err||cntb1<cntg2;
        *err = *err||cntb2<cntg2;
    }
    
    /*
     * Preconditioner test 2.
     *
     * If
     * * B2 is default preconditioner with non-unit scale S[i]=1/sqrt(h[i])
     * * G2 is scale-based preconditioner with non-unit scale S[i]=1/sqrt(h[i])
     * then B2 is worse than G2.
     * "Worse" means more iterations to converge.
     */
    for(n=10; n<=15; n++)
    {
        ae_vector_set_length(&x, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = (double)(0);
        }
        minlbfgscreate(n, m, &x, &state, _state);
        ae_vector_set_length(&s, n, _state);
        for(i=0; i<=n-1; i++)
        {
            s.ptr.p_double[i] = 1/ae_sqrt(2*ae_pow((double)(i*i+1), (double)(2), _state)*(0.8+0.4*ae_randomreal(_state)), _state);
        }
        minlbfgssetprecdefault(&state, _state);
        minlbfgssetscale(&state, &s, _state);
        cntb2 = 0;
        for(pass=0; pass<=k-1; pass++)
        {
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            minlbfgsrestartfrom(&state, &x, _state);
            while(minlbfgsiteration(&state, _state))
            {
                testminlbfgsunit_calciip2(&state, n, _state);
            }
            minlbfgsresults(&state, &x, &rep, _state);
            cntb2 = cntb2+rep.iterationscount;
            *err = *err||rep.terminationtype<=0;
        }
        minlbfgssetprecscale(&state, _state);
        minlbfgssetscale(&state, &s, _state);
        cntg2 = 0;
        for(pass=0; pass<=k-1; pass++)
        {
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            minlbfgsrestartfrom(&state, &x, _state);
            while(minlbfgsiteration(&state, _state))
            {
                testminlbfgsunit_calciip2(&state, n, _state);
            }
            minlbfgsresults(&state, &x, &rep, _state);
            cntg2 = cntg2+rep.iterationscount;
            *err = *err||rep.terminationtype<=0;
        }
        *err = *err||cntb2<cntg2;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests other properties

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testminlbfgsunit_testother(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t m;
    ae_vector x;
    ae_vector a;
    ae_vector b;
    ae_vector s;
    ae_vector h;
    ae_vector x0;
    ae_vector x1;
    ae_vector xlast;
    ae_matrix fulla;
    ae_bool hasxlast;
    double lastscaledstep;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    minlbfgsstate state;
    minlbfgsreport rep;
    double fprev;
    double xprev;
    double v;
    double stpmax;
    double tmpeps;
    double epsg;
    ae_int_t pkind;
    ae_int_t ckind;
    ae_int_t mkind;
    double vc;
    double vm;
    double diffstep;
    ae_int_t dkind;
    ae_bool wasf;
    ae_bool wasfg;
    double r;
    hqrndstate rs;
    ae_int_t spoiliteration;
    ae_int_t stopiteration;
    ae_int_t spoilvar;
    double spoilval;
    double ss;
    ae_bool terminationrequested;
    ae_int_t pass;
    ae_int_t stopcallidx;
    ae_int_t callidx;
    ae_int_t maxits;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&h, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&xlast, 0, DT_REAL, _state);
    ae_matrix_init(&fulla, 0, 0, DT_REAL, _state);
    _minlbfgsstate_init(&state, _state);
    _minlbfgsreport_init(&rep, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Test reports (F should form monotone sequence)
     */
    n = 50;
    m = 2;
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&xlast, n, _state);
    for(i=0; i<=n-1; i++)
    {
        x.ptr.p_double[i] = (double)(1);
    }
    minlbfgscreate(n, m, &x, &state, _state);
    minlbfgssetcond(&state, (double)(0), (double)(0), (double)(0), 100, _state);
    minlbfgssetxrep(&state, ae_true, _state);
    fprev = ae_maxrealnumber;
    while(minlbfgsiteration(&state, _state))
    {
        if( state.needfg )
        {
            state.f = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                state.f = state.f+ae_sqr((1+i)*state.x.ptr.p_double[i], _state);
                state.g.ptr.p_double[i] = 2*(1+i)*state.x.ptr.p_double[i];
            }
        }
        if( state.xupdated )
        {
            *err = *err||ae_fp_greater(state.f,fprev);
            if( ae_fp_eq(fprev,ae_maxrealnumber) )
            {
                for(i=0; i<=n-1; i++)
                {
                    *err = *err||ae_fp_neq(state.x.ptr.p_double[i],x.ptr.p_double[i]);
                }
            }
            fprev = state.f;
            ae_v_move(&xlast.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
        }
    }
    minlbfgsresults(&state, &x, &rep, _state);
    for(i=0; i<=n-1; i++)
    {
        *err = *err||ae_fp_neq(x.ptr.p_double[i],xlast.ptr.p_double[i]);
    }
    
    /*
     * Test differentiation vs. analytic gradient
     * (first one issues NeedF requests, second one issues NeedFG requests)
     */
    n = 50;
    m = 5;
    diffstep = 1.0E-6;
    for(dkind=0; dkind<=1; dkind++)
    {
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&xlast, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = (double)(1);
        }
        if( dkind==0 )
        {
            minlbfgscreate(n, m, &x, &state, _state);
        }
        if( dkind==1 )
        {
            minlbfgscreatef(n, m, &x, diffstep, &state, _state);
        }
        minlbfgssetcond(&state, (double)(0), (double)(0), (double)(0), n/2, _state);
        wasf = ae_false;
        wasfg = ae_false;
        while(minlbfgsiteration(&state, _state))
        {
            if( state.needf||state.needfg )
            {
                state.f = (double)(0);
            }
            for(i=0; i<=n-1; i++)
            {
                if( state.needf||state.needfg )
                {
                    state.f = state.f+ae_sqr((1+i)*state.x.ptr.p_double[i], _state);
                }
                if( state.needfg )
                {
                    state.g.ptr.p_double[i] = 2*(1+i)*state.x.ptr.p_double[i];
                }
            }
            wasf = wasf||state.needf;
            wasfg = wasfg||state.needfg;
        }
        minlbfgsresults(&state, &x, &rep, _state);
        if( dkind==0 )
        {
            *err = (*err||wasf)||!wasfg;
        }
        if( dkind==1 )
        {
            *err = (*err||!wasf)||wasfg;
        }
    }
    
    /*
     * Test that numerical differentiation uses scaling.
     *
     * In order to test that we solve simple optimization
     * problem: min(x^2) with initial x equal to 0.0.
     *
     * We choose random DiffStep and S, then we check that
     * optimizer evaluates function at +-DiffStep*S only.
     */
    ae_vector_set_length(&x, 1, _state);
    ae_vector_set_length(&s, 1, _state);
    diffstep = ae_randomreal(_state)*1.0E-6;
    s.ptr.p_double[0] = ae_exp(ae_randomreal(_state)*4-2, _state);
    x.ptr.p_double[0] = (double)(0);
    minlbfgscreatef(1, 1, &x, diffstep, &state, _state);
    minlbfgssetcond(&state, 1.0E-6, (double)(0), (double)(0), 0, _state);
    minlbfgssetscale(&state, &s, _state);
    v = (double)(0);
    while(minlbfgsiteration(&state, _state))
    {
        state.f = ae_sqr(state.x.ptr.p_double[0], _state);
        v = ae_maxreal(v, ae_fabs(state.x.ptr.p_double[0], _state), _state);
    }
    minlbfgsresults(&state, &x, &rep, _state);
    r = v/(s.ptr.p_double[0]*diffstep);
    *err = *err||ae_fp_greater(ae_fabs(ae_log(r, _state), _state),ae_log(1+1000*ae_machineepsilon, _state));
    
    /*
     * test maximum step
     */
    n = 1;
    m = 1;
    ae_vector_set_length(&x, n, _state);
    x.ptr.p_double[0] = (double)(100);
    stpmax = 0.05+0.05*ae_randomreal(_state);
    minlbfgscreate(n, m, &x, &state, _state);
    minlbfgssetcond(&state, 1.0E-9, (double)(0), (double)(0), 0, _state);
    minlbfgssetstpmax(&state, stpmax, _state);
    minlbfgssetxrep(&state, ae_true, _state);
    xprev = x.ptr.p_double[0];
    while(minlbfgsiteration(&state, _state))
    {
        if( state.needfg )
        {
            state.f = ae_exp(state.x.ptr.p_double[0], _state)+ae_exp(-state.x.ptr.p_double[0], _state);
            state.g.ptr.p_double[0] = ae_exp(state.x.ptr.p_double[0], _state)-ae_exp(-state.x.ptr.p_double[0], _state);
            *err = *err||ae_fp_greater(ae_fabs(state.x.ptr.p_double[0]-xprev, _state),(1+ae_sqrt(ae_machineepsilon, _state))*stpmax);
        }
        if( state.xupdated )
        {
            *err = *err||ae_fp_greater(ae_fabs(state.x.ptr.p_double[0]-xprev, _state),(1+ae_sqrt(ae_machineepsilon, _state))*stpmax);
            xprev = state.x.ptr.p_double[0];
        }
    }
    
    /*
     * Test correctness of the scaling:
     * * initial point is random point from [+1,+2]^N
     * * f(x) = SUM(A[i]*x[i]^4), C[i] is random from [0.01,100]
     * * we use random scaling matrix
     * * we test different variants of the preconditioning:
     *   0) unit preconditioner
     *   1) random diagonal from [0.01,100]
     *   2) scale preconditioner
     * * we set stringent stopping conditions (we try EpsG and EpsX)
     * * and we test that in the extremum stopping conditions are
     *   satisfied subject to the current scaling coefficients.
     */
    tmpeps = 1.0E-10;
    m = 1;
    for(n=1; n<=10; n++)
    {
        for(pkind=0; pkind<=2; pkind++)
        {
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&xlast, n, _state);
            ae_vector_set_length(&a, n, _state);
            ae_vector_set_length(&s, n, _state);
            ae_vector_set_length(&h, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = ae_randomreal(_state)+1;
                a.ptr.p_double[i] = ae_exp(ae_log((double)(100), _state)*(2*ae_randomreal(_state)-1), _state);
                s.ptr.p_double[i] = ae_exp(ae_log((double)(100), _state)*(2*ae_randomreal(_state)-1), _state);
                h.ptr.p_double[i] = ae_exp(ae_log((double)(100), _state)*(2*ae_randomreal(_state)-1), _state);
            }
            minlbfgscreate(n, m, &x, &state, _state);
            minlbfgssetscale(&state, &s, _state);
            minlbfgssetxrep(&state, ae_true, _state);
            if( pkind==1 )
            {
                minlbfgssetprecdiag(&state, &h, _state);
            }
            if( pkind==2 )
            {
                minlbfgssetprecscale(&state, _state);
            }
            
            /*
             * Test gradient-based stopping condition
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = ae_randomreal(_state)+1;
            }
            minlbfgssetcond(&state, tmpeps, (double)(0), (double)(0), 0, _state);
            minlbfgsrestartfrom(&state, &x, _state);
            while(minlbfgsiteration(&state, _state))
            {
                if( state.needfg )
                {
                    state.f = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.f = state.f+a.ptr.p_double[i]*ae_pow(state.x.ptr.p_double[i], (double)(4), _state);
                        state.g.ptr.p_double[i] = 4*a.ptr.p_double[i]*ae_pow(state.x.ptr.p_double[i], (double)(3), _state);
                    }
                }
            }
            minlbfgsresults(&state, &x, &rep, _state);
            if( rep.terminationtype<=0 )
            {
                *err = ae_true;
                ae_frame_leave(_state);
                return;
            }
            v = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                v = v+ae_sqr(s.ptr.p_double[i]*4*a.ptr.p_double[i]*ae_pow(x.ptr.p_double[i], (double)(3), _state), _state);
            }
            v = ae_sqrt(v, _state);
            *err = *err||ae_fp_greater(v,tmpeps);
            
            /*
             * Test step-based stopping condition
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = ae_randomreal(_state)+1;
            }
            hasxlast = ae_false;
            lastscaledstep = ae_maxrealnumber;
            minlbfgssetcond(&state, (double)(0), (double)(0), tmpeps, 0, _state);
            minlbfgsrestartfrom(&state, &x, _state);
            while(minlbfgsiteration(&state, _state))
            {
                if( state.needfg )
                {
                    state.f = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.f = state.f+a.ptr.p_double[i]*ae_pow(state.x.ptr.p_double[i], (double)(4), _state);
                        state.g.ptr.p_double[i] = 4*a.ptr.p_double[i]*ae_pow(state.x.ptr.p_double[i], (double)(3), _state);
                    }
                }
                if( state.xupdated )
                {
                    if( hasxlast )
                    {
                        lastscaledstep = (double)(0);
                        for(i=0; i<=n-1; i++)
                        {
                            lastscaledstep = lastscaledstep+ae_sqr(state.x.ptr.p_double[i]-xlast.ptr.p_double[i], _state)/ae_sqr(s.ptr.p_double[i], _state);
                        }
                        lastscaledstep = ae_sqrt(lastscaledstep, _state);
                    }
                    else
                    {
                        lastscaledstep = (double)(0);
                    }
                    ae_v_move(&xlast.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    hasxlast = ae_true;
                }
            }
            minlbfgsresults(&state, &x, &rep, _state);
            if( rep.terminationtype<=0 )
            {
                *err = ae_true;
                ae_frame_leave(_state);
                return;
            }
            *err = *err||ae_fp_greater(lastscaledstep,tmpeps);
        }
    }
    
    /*
     * Check correctness of the "trimming".
     *
     * Trimming is a technique which is used to help algorithm
     * cope with unbounded functions. In order to check this
     * technique we will try to solve following optimization
     * problem:
     *
     *     min f(x) subject to no constraints on X
     *            { 1/(1-x) + 1/(1+x) + c*x, if -0.999999<x<0.999999
     *     f(x) = {
     *            { M, if x<=-0.999999 or x>=0.999999
     *
     * where c is either 1.0 or 1.0E+6, M is either 1.0E8, 1.0E20 or +INF
     * (we try different combinations)
     */
    for(ckind=0; ckind<=1; ckind++)
    {
        for(mkind=0; mkind<=2; mkind++)
        {
            
            /*
             * Choose c and M
             */
            vc = (double)(1);
            vm = (double)(1);
            if( ckind==0 )
            {
                vc = 1.0;
            }
            if( ckind==1 )
            {
                vc = 1.0E+6;
            }
            if( mkind==0 )
            {
                vm = 1.0E+8;
            }
            if( mkind==1 )
            {
                vm = 1.0E+20;
            }
            if( mkind==2 )
            {
                vm = _state->v_posinf;
            }
            
            /*
             * Create optimizer, solve optimization problem
             */
            epsg = 1.0E-6*vc;
            ae_vector_set_length(&x, 1, _state);
            x.ptr.p_double[0] = 0.0;
            minlbfgscreate(1, 1, &x, &state, _state);
            minlbfgssetcond(&state, epsg, (double)(0), (double)(0), 0, _state);
            while(minlbfgsiteration(&state, _state))
            {
                if( state.needfg )
                {
                    if( ae_fp_less(-0.999999,state.x.ptr.p_double[0])&&ae_fp_less(state.x.ptr.p_double[0],0.999999) )
                    {
                        state.f = 1/(1-state.x.ptr.p_double[0])+1/(1+state.x.ptr.p_double[0])+vc*state.x.ptr.p_double[0];
                        state.g.ptr.p_double[0] = 1/ae_sqr(1-state.x.ptr.p_double[0], _state)-1/ae_sqr(1+state.x.ptr.p_double[0], _state)+vc;
                    }
                    else
                    {
                        state.f = vm;
                    }
                }
            }
            minlbfgsresults(&state, &x, &rep, _state);
            if( rep.terminationtype<=0 )
            {
                *err = ae_true;
                ae_frame_leave(_state);
                return;
            }
            *err = *err||ae_fp_greater(ae_fabs(1/ae_sqr(1-x.ptr.p_double[0], _state)-1/ae_sqr(1+x.ptr.p_double[0], _state)+vc, _state),epsg);
        }
    }
    
    /*
     * Test integrity checks for NAN/INF:
     * * algorithm solves optimization problem, which is normal for some time (quadratic)
     * * after 5-th step we choose random component of gradient and consistently spoil
     *   it by NAN or INF.
     * * we check that correct termination code is returned (-8)
     */
    n = 100;
    for(pass=1; pass<=10; pass++)
    {
        spoiliteration = 5;
        stopiteration = 8;
        if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
        {
            
            /*
             * Gradient can be spoiled by +INF, -INF, NAN
             */
            spoilvar = hqrnduniformi(&rs, n, _state);
            i = hqrnduniformi(&rs, 3, _state);
            spoilval = _state->v_nan;
            if( i==0 )
            {
                spoilval = _state->v_neginf;
            }
            if( i==1 )
            {
                spoilval = _state->v_posinf;
            }
        }
        else
        {
            
            /*
             * Function value can be spoiled only by NAN
             * (+INF can be recognized as legitimate value during optimization)
             */
            spoilvar = -1;
            spoilval = _state->v_nan;
        }
        spdmatrixrndcond(n, 1.0E5, &fulla, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&x0, n, _state);
        for(i=0; i<=n-1; i++)
        {
            b.ptr.p_double[i] = hqrndnormal(&rs, _state);
            x0.ptr.p_double[i] = hqrndnormal(&rs, _state);
        }
        minlbfgscreate(n, 1, &x0, &state, _state);
        minlbfgssetcond(&state, 0.0, 0.0, 0.0, stopiteration, _state);
        minlbfgssetxrep(&state, ae_true, _state);
        k = -1;
        while(minlbfgsiteration(&state, _state))
        {
            if( state.needfg )
            {
                state.f = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.f = state.f+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                    state.g.ptr.p_double[i] = b.ptr.p_double[i];
                    for(j=0; j<=n-1; j++)
                    {
                        state.f = state.f+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        state.g.ptr.p_double[i] = state.g.ptr.p_double[i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
                if( k>=spoiliteration )
                {
                    if( spoilvar<0 )
                    {
                        state.f = spoilval;
                    }
                    else
                    {
                        state.g.ptr.p_double[spoilvar] = spoilval;
                    }
                }
                continue;
            }
            if( state.xupdated )
            {
                inc(&k, _state);
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minlbfgsresults(&state, &x1, &rep, _state);
        seterrorflag(err, rep.terminationtype!=-8, _state);
    }
    
    /*
     * Check algorithm ability to handle request for termination:
     * * to terminate with correct return code = 8
     * * to return point which was "current" at the moment of termination
     */
    for(pass=1; pass<=50; pass++)
    {
        n = 3;
        ss = (double)(100);
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&xlast, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = 6+ae_randomreal(_state);
        }
        stopcallidx = ae_randominteger(20, _state);
        maxits = 25;
        minlbfgscreate(n, 1, &x, &state, _state);
        minlbfgssetcond(&state, (double)(0), (double)(0), (double)(0), maxits, _state);
        minlbfgssetxrep(&state, ae_true, _state);
        callidx = 0;
        terminationrequested = ae_false;
        ae_v_move(&xlast.ptr.p_double[0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
        while(minlbfgsiteration(&state, _state))
        {
            if( state.needfg )
            {
                state.f = ss*ae_sqr(ae_exp(state.x.ptr.p_double[0], _state)-2, _state)+ae_sqr(state.x.ptr.p_double[1], _state)+ae_sqr(state.x.ptr.p_double[2]-state.x.ptr.p_double[0], _state);
                state.g.ptr.p_double[0] = 2*ss*(ae_exp(state.x.ptr.p_double[0], _state)-2)*ae_exp(state.x.ptr.p_double[0], _state)+2*(state.x.ptr.p_double[2]-state.x.ptr.p_double[0])*(-1);
                state.g.ptr.p_double[1] = 2*state.x.ptr.p_double[1];
                state.g.ptr.p_double[2] = 2*(state.x.ptr.p_double[2]-state.x.ptr.p_double[0]);
                if( callidx==stopcallidx )
                {
                    minlbfgsrequesttermination(&state, _state);
                    terminationrequested = ae_true;
                }
                inc(&callidx, _state);
                continue;
            }
            if( state.xupdated )
            {
                if( !terminationrequested )
                {
                    ae_v_move(&xlast.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minlbfgsresults(&state, &x, &rep, _state);
        seterrorflag(err, rep.terminationtype!=8, _state);
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(err, ae_fp_neq(x.ptr.p_double[i],xlast.ptr.p_double[i]), _state);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests, that gradient verified correctly.
*************************************************************************/
static ae_bool testminlbfgsunit_gradientchecktest(ae_state *_state)
{
    ae_frame _frame_block;
    minlbfgsstate state;
    minlbfgsreport rep;
    ae_int_t m;
    ae_int_t n;
    double a;
    double b;
    double c;
    double d;
    double x0;
    double x1;
    double x2;
    ae_vector x;
    double teststep;
    double noise;
    ae_int_t nbrcomp;
    ae_int_t func;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t i;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _minlbfgsstate_init(&state, _state);
    _minlbfgsreport_init(&rep, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);

    passcount = 35;
    teststep = 0.01;
    n = 3;
    m = 2;
    ae_vector_set_length(&x, n, _state);
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Prepare test's parameters
         */
        func = ae_randominteger(3, _state)+1;
        nbrcomp = ae_randominteger(n, _state);
        noise = (double)(10*(2*ae_randominteger(2, _state)-1));
        
        /*
         * Prepare function's parameters
         */
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = 5*randomnormal(_state);
        }
        a = 5*ae_randomreal(_state)+1;
        b = 5*ae_randomreal(_state)+1;
        c = 5*ae_randomreal(_state)+1;
        d = 5*ae_randomreal(_state)+1;
        x0 = 5*(2*ae_randomreal(_state)-1);
        x1 = 5*(2*ae_randomreal(_state)-1);
        x2 = 5*(2*ae_randomreal(_state)-1);
        minlbfgscreate(n, m, &x, &state, _state);
        minlbfgssetcond(&state, (double)(0), (double)(0), (double)(0), 0, _state);
        minlbfgssetgradientcheck(&state, teststep, _state);
        
        /*
         * Check that the criterion passes a derivative if it is correct
         */
        while(minlbfgsiteration(&state, _state))
        {
            if( state.needfg )
            {
                testminlbfgsunit_funcderiv(a, b, c, d, x0, x1, x2, &state.x, func, &state.f, &state.g, _state);
            }
        }
        minlbfgsresults(&state, &x, &rep, _state);
        
        /*
         * Check that error code does not equal to -7 and parameter .VarIdx
         * equal to -1.
         */
        if( rep.terminationtype==-7||rep.varidx!=-1 )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = 5*randomnormal(_state);
        }
        minlbfgsrestartfrom(&state, &x, _state);
        
        /*
         * Check that the criterion does not miss a derivative if
         * it is incorrect
         */
        while(minlbfgsiteration(&state, _state))
        {
            if( state.needfg )
            {
                testminlbfgsunit_funcderiv(a, b, c, d, x0, x1, x2, &state.x, func, &state.f, &state.g, _state);
                state.g.ptr.p_double[nbrcomp] = state.g.ptr.p_double[nbrcomp]+noise;
            }
        }
        minlbfgsresults(&state, &x, &rep, _state);
        
        /*
         * Check that error code equal to -7 and parameter .VarIdx
         * equal to number of incorrect component.
         */
        if( rep.terminationtype!=-7||rep.varidx!=nbrcomp )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function return function value and it derivatives. Function dimension
is 3.
    Function's list:
        * funcType=1:
            F(X)=A*(X-X0)^2+B*(Y-Y0)^2+C*(Z-Z0)^2+D;
        * funcType=2:
            F(X)=A*sin(X-X0)^2+B*sin(Y-Y0)^2+C*sin(Z-Z0)^2+D;
        * funcType=3:
            F(X)=A*(X-X0)^2+B*(Y-Y0)^2+C*((Z-Z0)-(X-X0))^2+D.
*************************************************************************/
static void testminlbfgsunit_funcderiv(double a,
     double b,
     double c,
     double d,
     double x0,
     double x1,
     double x2,
     /* Real    */ ae_vector* x,
     ae_int_t functype,
     double* f,
     /* Real    */ ae_vector* g,
     ae_state *_state)
{


    ae_assert(((ae_isfinite(a, _state)&&ae_isfinite(b, _state))&&ae_isfinite(c, _state))&&ae_isfinite(d, _state), "FuncDeriv: A, B, C or D contains NaN or Infinite.", _state);
    ae_assert((ae_isfinite(x0, _state)&&ae_isfinite(x1, _state))&&ae_isfinite(x2, _state), "FuncDeriv: X0, X1 or X2 contains NaN or Infinite.", _state);
    ae_assert(functype>=1&&functype<=3, "FuncDeriv: incorrect funcType(funcType<1 or funcType>3).", _state);
    if( functype==1 )
    {
        *f = a*ae_sqr(x->ptr.p_double[0]-x0, _state)+b*ae_sqr(x->ptr.p_double[1]-x1, _state)+c*ae_sqr(x->ptr.p_double[2]-x2, _state)+d;
        g->ptr.p_double[0] = 2*a*(x->ptr.p_double[0]-x0);
        g->ptr.p_double[1] = 2*b*(x->ptr.p_double[1]-x1);
        g->ptr.p_double[2] = 2*c*(x->ptr.p_double[2]-x2);
        return;
    }
    if( functype==2 )
    {
        *f = a*ae_sqr(ae_sin(x->ptr.p_double[0]-x0, _state), _state)+b*ae_sqr(ae_sin(x->ptr.p_double[1]-x1, _state), _state)+c*ae_sqr(ae_sin(x->ptr.p_double[2]-x2, _state), _state)+d;
        g->ptr.p_double[0] = 2*a*ae_sin(x->ptr.p_double[0]-x0, _state)*ae_cos(x->ptr.p_double[0]-x0, _state);
        g->ptr.p_double[1] = 2*b*ae_sin(x->ptr.p_double[1]-x1, _state)*ae_cos(x->ptr.p_double[1]-x1, _state);
        g->ptr.p_double[2] = 2*c*ae_sin(x->ptr.p_double[2]-x2, _state)*ae_cos(x->ptr.p_double[2]-x2, _state);
        return;
    }
    if( functype==3 )
    {
        *f = a*ae_sqr(x->ptr.p_double[0]-x0, _state)+b*ae_sqr(x->ptr.p_double[1]-x1, _state)+c*ae_sqr(x->ptr.p_double[2]-x2-(x->ptr.p_double[0]-x0), _state)+d;
        g->ptr.p_double[0] = 2*a*(x->ptr.p_double[0]-x0)+2*c*(x->ptr.p_double[0]-x->ptr.p_double[2]-x0+x2);
        g->ptr.p_double[1] = 2*b*(x->ptr.p_double[1]-x1);
        g->ptr.p_double[2] = 2*c*(x->ptr.p_double[2]-x->ptr.p_double[0]-x2+x0);
        return;
    }
}


/*$ End $*/
