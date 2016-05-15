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
#include "testminlmunit.h"


/*$ Declarations $*/
static ae_bool testminlmunit_rkindvsstatecheck(ae_int_t rkind,
     minlmstate* state,
     ae_state *_state);
static void testminlmunit_axmb(minlmstate* state,
     /* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* b,
     ae_int_t n,
     ae_state *_state);
static void testminlmunit_tryreproducefixedbugs(ae_bool* err,
     ae_state *_state);
static ae_bool testminlmunit_gradientchecktest(ae_state *_state);
static void testminlmunit_funcderiv(/* Real    */ ae_vector* a,
     /* Real    */ ae_vector* x0,
     /* Real    */ ae_vector* x,
     ae_int_t m,
     ae_int_t n,
     double anyconst,
     ae_int_t functype,
     /* Real    */ ae_vector* f,
     /* Real    */ ae_matrix* j,
     ae_state *_state);


/*$ Body $*/


ae_bool testminlm(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_bool referror;
    ae_bool lin1error;
    ae_bool lin2error;
    ae_bool eqerror;
    ae_bool converror;
    ae_bool scerror;
    ae_bool restartserror;
    ae_bool othererrors;
    ae_bool graderrors;
    ae_int_t rkind;
    ae_int_t ckind;
    ae_int_t tmpkind;
    double epsf;
    double epsx;
    double epsg;
    ae_int_t maxits;
    ae_int_t n;
    ae_int_t m;
    ae_vector x;
    ae_vector xe;
    ae_vector b;
    ae_vector bl;
    ae_vector bu;
    ae_vector xlast;
    ae_int_t i;
    ae_int_t j;
    double v;
    double s;
    double stpmax;
    double h;
    ae_matrix a;
    double fprev;
    double xprev;
    minlmstate state;
    minlmreport rep;
    ae_int_t stopcallidx;
    ae_int_t callidx;
    ae_bool terminationrequested;
    ae_int_t pass;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&xe, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);
    ae_vector_init(&xlast, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    _minlmstate_init(&state, _state);
    _minlmreport_init(&rep, _state);

    waserrors = ae_false;
    referror = ae_false;
    lin1error = ae_false;
    lin2error = ae_false;
    eqerror = ae_false;
    converror = ae_false;
    scerror = ae_false;
    othererrors = ae_false;
    restartserror = ae_false;
    
    /*
     * Try to reproduce previously fixed bugs
     */
    testminlmunit_tryreproducefixedbugs(&othererrors, _state);
    
    /*
     * Reference problem.
     * See comments for RKindVsStateCheck() for more info about RKind.
     *
     * NOTES: we also test negative RKind's corresponding to "inexact" schemes
     * which use approximate finite difference Jacobian.
     */
    ae_vector_set_length(&x, 3, _state);
    n = 3;
    m = 3;
    h = 0.0001;
    for(rkind=-2; rkind<=5; rkind++)
    {
        x.ptr.p_double[0] = 100*ae_randomreal(_state)-50;
        x.ptr.p_double[1] = 100*ae_randomreal(_state)-50;
        x.ptr.p_double[2] = 100*ae_randomreal(_state)-50;
        if( rkind==-2 )
        {
            minlmcreatev(n, m, &x, h, &state, _state);
            minlmsetacctype(&state, 1, _state);
        }
        if( rkind==-1 )
        {
            minlmcreatev(n, m, &x, h, &state, _state);
            minlmsetacctype(&state, 0, _state);
        }
        if( rkind==0 )
        {
            minlmcreatefj(n, m, &x, &state, _state);
        }
        if( rkind==1 )
        {
            minlmcreatefgj(n, m, &x, &state, _state);
        }
        if( rkind==2 )
        {
            minlmcreatefgh(n, &x, &state, _state);
        }
        if( rkind==3 )
        {
            minlmcreatevj(n, m, &x, &state, _state);
            minlmsetacctype(&state, 0, _state);
        }
        if( rkind==4 )
        {
            minlmcreatevj(n, m, &x, &state, _state);
            minlmsetacctype(&state, 1, _state);
        }
        if( rkind==5 )
        {
            minlmcreatevj(n, m, &x, &state, _state);
            minlmsetacctype(&state, 2, _state);
        }
        while(minlmiteration(&state, _state))
        {
            
            /*
             * (x-2)^2 + y^2 + (z-x)^2
             */
            if( state.needfi )
            {
                state.fi.ptr.p_double[0] = state.x.ptr.p_double[0]-2;
                state.fi.ptr.p_double[1] = state.x.ptr.p_double[1];
                state.fi.ptr.p_double[2] = state.x.ptr.p_double[2]-state.x.ptr.p_double[0];
            }
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = state.x.ptr.p_double[0]-2;
                state.fi.ptr.p_double[1] = state.x.ptr.p_double[1];
                state.fi.ptr.p_double[2] = state.x.ptr.p_double[2]-state.x.ptr.p_double[0];
                state.j.ptr.pp_double[0][0] = (double)(1);
                state.j.ptr.pp_double[0][1] = (double)(0);
                state.j.ptr.pp_double[0][2] = (double)(0);
                state.j.ptr.pp_double[1][0] = (double)(0);
                state.j.ptr.pp_double[1][1] = (double)(1);
                state.j.ptr.pp_double[1][2] = (double)(0);
                state.j.ptr.pp_double[2][0] = (double)(-1);
                state.j.ptr.pp_double[2][1] = (double)(0);
                state.j.ptr.pp_double[2][2] = (double)(1);
            }
            if( (state.needf||state.needfg)||state.needfgh )
            {
                state.f = ae_sqr(state.x.ptr.p_double[0]-2, _state)+ae_sqr(state.x.ptr.p_double[1], _state)+ae_sqr(state.x.ptr.p_double[2]-state.x.ptr.p_double[0], _state);
            }
            if( state.needfg||state.needfgh )
            {
                state.g.ptr.p_double[0] = 2*(state.x.ptr.p_double[0]-2)+2*(state.x.ptr.p_double[0]-state.x.ptr.p_double[2]);
                state.g.ptr.p_double[1] = 2*state.x.ptr.p_double[1];
                state.g.ptr.p_double[2] = 2*(state.x.ptr.p_double[2]-state.x.ptr.p_double[0]);
            }
            if( state.needfgh )
            {
                state.h.ptr.pp_double[0][0] = (double)(4);
                state.h.ptr.pp_double[0][1] = (double)(0);
                state.h.ptr.pp_double[0][2] = (double)(-2);
                state.h.ptr.pp_double[1][0] = (double)(0);
                state.h.ptr.pp_double[1][1] = (double)(2);
                state.h.ptr.pp_double[1][2] = (double)(0);
                state.h.ptr.pp_double[2][0] = (double)(-2);
                state.h.ptr.pp_double[2][1] = (double)(0);
                state.h.ptr.pp_double[2][2] = (double)(2);
            }
            scerror = scerror||!testminlmunit_rkindvsstatecheck(rkind, &state, _state);
        }
        minlmresults(&state, &x, &rep, _state);
        referror = (((referror||rep.terminationtype<=0)||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-2, _state),0.001))||ae_fp_greater(ae_fabs(x.ptr.p_double[1], _state),0.001))||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-2, _state),0.001);
    }
    
    /*
     * Reference bound constrained problem:
     *
     *     min sum((x[i]-xe[i])^4) subject to 0<=x[i]<=1
     *
     * NOTES:
     * 1. we test only two optimization modes - V and FGH,
     *    because from algorithm internals we can assume that actual
     *    mode being used doesn't matter for bound constrained optimization
     *    process.
     */
    for(tmpkind=0; tmpkind<=1; tmpkind++)
    {
        for(n=1; n<=5; n++)
        {
            ae_vector_set_length(&bl, n, _state);
            ae_vector_set_length(&bu, n, _state);
            ae_vector_set_length(&xe, n, _state);
            ae_vector_set_length(&x, n, _state);
            for(i=0; i<=n-1; i++)
            {
                bl.ptr.p_double[i] = (double)(0);
                bu.ptr.p_double[i] = (double)(1);
                xe.ptr.p_double[i] = 3*ae_randomreal(_state)-1;
                x.ptr.p_double[i] = ae_randomreal(_state);
            }
            if( tmpkind==0 )
            {
                minlmcreatefgh(n, &x, &state, _state);
            }
            if( tmpkind==1 )
            {
                minlmcreatev(n, n, &x, 1.0E-3, &state, _state);
            }
            minlmsetcond(&state, 1.0E-6, (double)(0), (double)(0), 0, _state);
            minlmsetbc(&state, &bl, &bu, _state);
            while(minlmiteration(&state, _state))
            {
                if( state.needfi )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[i] = ae_pow(state.x.ptr.p_double[i]-xe.ptr.p_double[i], (double)(2), _state);
                    }
                }
                if( (state.needf||state.needfg)||state.needfgh )
                {
                    state.f = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.f = state.f+ae_pow(state.x.ptr.p_double[i]-xe.ptr.p_double[i], (double)(4), _state);
                    }
                }
                if( state.needfg||state.needfgh )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        state.g.ptr.p_double[i] = 4*ae_pow(state.x.ptr.p_double[i]-xe.ptr.p_double[i], (double)(3), _state);
                    }
                }
                if( state.needfgh )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            state.h.ptr.pp_double[i][j] = (double)(0);
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        state.h.ptr.pp_double[i][i] = 12*ae_pow(state.x.ptr.p_double[i]-xe.ptr.p_double[i], (double)(2), _state);
                    }
                }
            }
            minlmresults(&state, &x, &rep, _state);
            if( rep.terminationtype==4 )
            {
                for(i=0; i<=n-1; i++)
                {
                    referror = referror||ae_fp_greater(ae_fabs(x.ptr.p_double[i]-boundval(xe.ptr.p_double[i], bl.ptr.p_double[i], bu.ptr.p_double[i], _state), _state),5.0E-2);
                }
            }
            else
            {
                referror = ae_true;
            }
        }
    }
    
    /*
     * 1D problem #1
     *
     * NOTES: we also test negative RKind's corresponding to "inexact" schemes
     * which use approximate finite difference Jacobian.
     */
    for(rkind=-2; rkind<=5; rkind++)
    {
        ae_vector_set_length(&x, 1, _state);
        n = 1;
        m = 1;
        h = 0.00001;
        x.ptr.p_double[0] = 100*ae_randomreal(_state)-50;
        if( rkind==-2 )
        {
            minlmcreatev(n, m, &x, h, &state, _state);
            minlmsetacctype(&state, 1, _state);
        }
        if( rkind==-1 )
        {
            minlmcreatev(n, m, &x, h, &state, _state);
            minlmsetacctype(&state, 0, _state);
        }
        if( rkind==0 )
        {
            minlmcreatefj(n, m, &x, &state, _state);
        }
        if( rkind==1 )
        {
            minlmcreatefgj(n, m, &x, &state, _state);
        }
        if( rkind==2 )
        {
            minlmcreatefgh(n, &x, &state, _state);
        }
        if( rkind==3 )
        {
            minlmcreatevj(n, m, &x, &state, _state);
            minlmsetacctype(&state, 0, _state);
        }
        if( rkind==4 )
        {
            minlmcreatevj(n, m, &x, &state, _state);
            minlmsetacctype(&state, 1, _state);
        }
        if( rkind==5 )
        {
            minlmcreatevj(n, m, &x, &state, _state);
            minlmsetacctype(&state, 2, _state);
        }
        while(minlmiteration(&state, _state))
        {
            if( state.needfi )
            {
                state.fi.ptr.p_double[0] = ae_sin(state.x.ptr.p_double[0], _state);
            }
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = ae_sin(state.x.ptr.p_double[0], _state);
                state.j.ptr.pp_double[0][0] = ae_cos(state.x.ptr.p_double[0], _state);
            }
            if( (state.needf||state.needfg)||state.needfgh )
            {
                state.f = ae_sqr(ae_sin(state.x.ptr.p_double[0], _state), _state);
            }
            if( state.needfg||state.needfgh )
            {
                state.g.ptr.p_double[0] = 2*ae_sin(state.x.ptr.p_double[0], _state)*ae_cos(state.x.ptr.p_double[0], _state);
            }
            if( state.needfgh )
            {
                state.h.ptr.pp_double[0][0] = 2*(ae_cos(state.x.ptr.p_double[0], _state)*ae_cos(state.x.ptr.p_double[0], _state)-ae_sin(state.x.ptr.p_double[0], _state)*ae_sin(state.x.ptr.p_double[0], _state));
            }
            scerror = scerror||!testminlmunit_rkindvsstatecheck(rkind, &state, _state);
        }
        minlmresults(&state, &x, &rep, _state);
        lin1error = rep.terminationtype<=0||ae_fp_greater(ae_fabs(x.ptr.p_double[0]/ae_pi-ae_round(x.ptr.p_double[0]/ae_pi, _state), _state),0.001);
    }
    
    /*
     * Linear equations: test normal optimization and optimization with restarts
     */
    for(n=1; n<=10; n++)
    {
        
        /*
         * Prepare task
         */
        h = 0.00001;
        rmatrixrndcond(n, (double)(100), &a, _state);
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&xe, n, _state);
        ae_vector_set_length(&b, n, _state);
        for(i=0; i<=n-1; i++)
        {
            xe.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        for(i=0; i<=n-1; i++)
        {
            v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xe.ptr.p_double[0], 1, ae_v_len(0,n-1));
            b.ptr.p_double[i] = v;
        }
        
        /*
         * Test different RKind
         *
         * NOTES: we also test negative RKind's corresponding to "inexact" schemes
         * which use approximate finite difference Jacobian.
         */
        for(rkind=-2; rkind<=5; rkind++)
        {
            
            /*
             * Solve task (first attempt)
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            if( rkind==-2 )
            {
                minlmcreatev(n, n, &x, h, &state, _state);
                minlmsetacctype(&state, 1, _state);
            }
            if( rkind==-1 )
            {
                minlmcreatev(n, n, &x, h, &state, _state);
                minlmsetacctype(&state, 0, _state);
            }
            if( rkind==0 )
            {
                minlmcreatefj(n, n, &x, &state, _state);
            }
            if( rkind==1 )
            {
                minlmcreatefgj(n, n, &x, &state, _state);
            }
            if( rkind==2 )
            {
                minlmcreatefgh(n, &x, &state, _state);
            }
            if( rkind==3 )
            {
                minlmcreatevj(n, n, &x, &state, _state);
                minlmsetacctype(&state, 0, _state);
            }
            if( rkind==4 )
            {
                minlmcreatevj(n, n, &x, &state, _state);
                minlmsetacctype(&state, 1, _state);
            }
            if( rkind==5 )
            {
                minlmcreatevj(n, n, &x, &state, _state);
                minlmsetacctype(&state, 2, _state);
            }
            while(minlmiteration(&state, _state))
            {
                testminlmunit_axmb(&state, &a, &b, n, _state);
                scerror = scerror||!testminlmunit_rkindvsstatecheck(rkind, &state, _state);
            }
            minlmresults(&state, &x, &rep, _state);
            eqerror = eqerror||rep.terminationtype<=0;
            for(i=0; i<=n-1; i++)
            {
                eqerror = eqerror||ae_fp_greater(ae_fabs(x.ptr.p_double[i]-xe.ptr.p_double[i], _state),0.001);
            }
            
            /*
             * Now we try to restart algorithm from new point
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            minlmrestartfrom(&state, &x, _state);
            while(minlmiteration(&state, _state))
            {
                testminlmunit_axmb(&state, &a, &b, n, _state);
                scerror = scerror||!testminlmunit_rkindvsstatecheck(rkind, &state, _state);
            }
            minlmresults(&state, &x, &rep, _state);
            restartserror = restartserror||rep.terminationtype<=0;
            for(i=0; i<=n-1; i++)
            {
                restartserror = restartserror||ae_fp_greater(ae_fabs(x.ptr.p_double[i]-xe.ptr.p_double[i], _state),0.001);
            }
        }
    }
    
    /*
     * Testing convergence properties using
     * different optimizer types and different conditions.
     *
     * Only limited subset of optimizers is tested because some
     * optimizers converge too quickly.
     */
    s = (double)(100);
    for(rkind=0; rkind<=5; rkind++)
    {
        
        /*
         * Skip FGH optimizer - it converges too quickly
         */
        if( rkind==2 )
        {
            continue;
        }
        
        /*
         * Test
         */
        for(ckind=0; ckind<=3; ckind++)
        {
            epsg = (double)(0);
            epsf = (double)(0);
            epsx = (double)(0);
            maxits = 0;
            if( ckind==0 )
            {
                epsf = 0.000001;
            }
            if( ckind==1 )
            {
                epsx = 0.000001;
            }
            if( ckind==2 )
            {
                maxits = 2;
            }
            if( ckind==3 )
            {
                epsg = 0.0001;
            }
            ae_vector_set_length(&x, 3, _state);
            n = 3;
            m = 3;
            for(i=0; i<=2; i++)
            {
                x.ptr.p_double[i] = (double)(6);
            }
            if( rkind==0 )
            {
                minlmcreatefj(n, m, &x, &state, _state);
            }
            if( rkind==1 )
            {
                minlmcreatefgj(n, m, &x, &state, _state);
            }
            ae_assert(rkind!=2, "Assertion failed", _state);
            if( rkind==3 )
            {
                minlmcreatevj(n, m, &x, &state, _state);
                minlmsetacctype(&state, 0, _state);
            }
            if( rkind==4 )
            {
                minlmcreatevj(n, m, &x, &state, _state);
                minlmsetacctype(&state, 1, _state);
            }
            if( rkind==5 )
            {
                minlmcreatevj(n, m, &x, &state, _state);
                minlmsetacctype(&state, 2, _state);
            }
            minlmsetcond(&state, epsg, epsf, epsx, maxits, _state);
            while(minlmiteration(&state, _state))
            {
                if( state.needfi||state.needfij )
                {
                    state.fi.ptr.p_double[0] = s*(ae_exp(state.x.ptr.p_double[0], _state)-2);
                    state.fi.ptr.p_double[1] = ae_sqr(state.x.ptr.p_double[1], _state)+1;
                    state.fi.ptr.p_double[2] = state.x.ptr.p_double[2]-state.x.ptr.p_double[0];
                }
                if( state.needfij )
                {
                    state.j.ptr.pp_double[0][0] = s*ae_exp(state.x.ptr.p_double[0], _state);
                    state.j.ptr.pp_double[0][1] = (double)(0);
                    state.j.ptr.pp_double[0][2] = (double)(0);
                    state.j.ptr.pp_double[1][0] = (double)(0);
                    state.j.ptr.pp_double[1][1] = 2*state.x.ptr.p_double[1];
                    state.j.ptr.pp_double[1][2] = (double)(0);
                    state.j.ptr.pp_double[2][0] = (double)(-1);
                    state.j.ptr.pp_double[2][1] = (double)(0);
                    state.j.ptr.pp_double[2][2] = (double)(1);
                }
                if( (state.needf||state.needfg)||state.needfgh )
                {
                    state.f = s*ae_sqr(ae_exp(state.x.ptr.p_double[0], _state)-2, _state)+ae_sqr(ae_sqr(state.x.ptr.p_double[1], _state)+1, _state)+ae_sqr(state.x.ptr.p_double[2]-state.x.ptr.p_double[0], _state);
                }
                if( state.needfg||state.needfgh )
                {
                    state.g.ptr.p_double[0] = s*2*(ae_exp(state.x.ptr.p_double[0], _state)-2)*ae_exp(state.x.ptr.p_double[0], _state)+2*(state.x.ptr.p_double[0]-state.x.ptr.p_double[2]);
                    state.g.ptr.p_double[1] = 2*(ae_sqr(state.x.ptr.p_double[1], _state)+1)*2*state.x.ptr.p_double[1];
                    state.g.ptr.p_double[2] = 2*(state.x.ptr.p_double[2]-state.x.ptr.p_double[0]);
                }
                if( state.needfgh )
                {
                    state.h.ptr.pp_double[0][0] = s*(4*ae_sqr(ae_exp(state.x.ptr.p_double[0], _state), _state)-4*ae_exp(state.x.ptr.p_double[0], _state))+2;
                    state.h.ptr.pp_double[0][1] = (double)(0);
                    state.h.ptr.pp_double[0][2] = (double)(-2);
                    state.h.ptr.pp_double[1][0] = (double)(0);
                    state.h.ptr.pp_double[1][1] = 12*ae_sqr(state.x.ptr.p_double[1], _state)+4;
                    state.h.ptr.pp_double[1][2] = (double)(0);
                    state.h.ptr.pp_double[2][0] = (double)(-2);
                    state.h.ptr.pp_double[2][1] = (double)(0);
                    state.h.ptr.pp_double[2][2] = (double)(2);
                }
                scerror = scerror||!testminlmunit_rkindvsstatecheck(rkind, &state, _state);
            }
            minlmresults(&state, &x, &rep, _state);
            if( ckind==0 )
            {
                converror = converror||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-ae_log((double)(2), _state), _state),0.05);
                converror = converror||ae_fp_greater(ae_fabs(x.ptr.p_double[1], _state),0.05);
                converror = converror||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-ae_log((double)(2), _state), _state),0.05);
                converror = converror||rep.terminationtype!=1;
            }
            if( ckind==1 )
            {
                converror = converror||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-ae_log((double)(2), _state), _state),0.05);
                converror = converror||ae_fp_greater(ae_fabs(x.ptr.p_double[1], _state),0.05);
                converror = converror||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-ae_log((double)(2), _state), _state),0.05);
                converror = converror||rep.terminationtype!=2;
            }
            if( ckind==2 )
            {
                converror = (converror||rep.terminationtype!=5)||rep.iterationscount!=maxits;
            }
            if( ckind==3 )
            {
                converror = converror||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-ae_log((double)(2), _state), _state),0.05);
                converror = converror||ae_fp_greater(ae_fabs(x.ptr.p_double[1], _state),0.05);
                converror = converror||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-ae_log((double)(2), _state), _state),0.05);
                converror = converror||rep.terminationtype!=4;
            }
        }
    }
    
    /*
     * Other properties:
     * 1. test reports (F should form monotone sequence)
     * 2. test maximum step
     */
    for(rkind=0; rkind<=5; rkind++)
    {
        
        /*
         * reports:
         * * check that first report is initial point
         * * check that F is monotone decreasing
         * * check that last report is final result
         */
        n = 3;
        m = 3;
        s = (double)(100);
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&xlast, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = (double)(6);
        }
        if( rkind==0 )
        {
            minlmcreatefj(n, m, &x, &state, _state);
        }
        if( rkind==1 )
        {
            minlmcreatefgj(n, m, &x, &state, _state);
        }
        if( rkind==2 )
        {
            minlmcreatefgh(n, &x, &state, _state);
        }
        if( rkind==3 )
        {
            minlmcreatevj(n, m, &x, &state, _state);
            minlmsetacctype(&state, 0, _state);
        }
        if( rkind==4 )
        {
            minlmcreatevj(n, m, &x, &state, _state);
            minlmsetacctype(&state, 1, _state);
        }
        if( rkind==5 )
        {
            minlmcreatevj(n, m, &x, &state, _state);
            minlmsetacctype(&state, 2, _state);
        }
        minlmsetcond(&state, (double)(0), (double)(0), (double)(0), 4, _state);
        minlmsetxrep(&state, ae_true, _state);
        fprev = ae_maxrealnumber;
        while(minlmiteration(&state, _state))
        {
            if( state.needfi||state.needfij )
            {
                state.fi.ptr.p_double[0] = ae_sqrt(s, _state)*(ae_exp(state.x.ptr.p_double[0], _state)-2);
                state.fi.ptr.p_double[1] = state.x.ptr.p_double[1];
                state.fi.ptr.p_double[2] = state.x.ptr.p_double[2]-state.x.ptr.p_double[0];
            }
            if( state.needfij )
            {
                state.j.ptr.pp_double[0][0] = ae_sqrt(s, _state)*ae_exp(state.x.ptr.p_double[0], _state);
                state.j.ptr.pp_double[0][1] = (double)(0);
                state.j.ptr.pp_double[0][2] = (double)(0);
                state.j.ptr.pp_double[1][0] = (double)(0);
                state.j.ptr.pp_double[1][1] = (double)(1);
                state.j.ptr.pp_double[1][2] = (double)(0);
                state.j.ptr.pp_double[2][0] = (double)(-1);
                state.j.ptr.pp_double[2][1] = (double)(0);
                state.j.ptr.pp_double[2][2] = (double)(1);
            }
            if( (state.needf||state.needfg)||state.needfgh )
            {
                state.f = s*ae_sqr(ae_exp(state.x.ptr.p_double[0], _state)-2, _state)+ae_sqr(state.x.ptr.p_double[1], _state)+ae_sqr(state.x.ptr.p_double[2]-state.x.ptr.p_double[0], _state);
            }
            if( state.needfg||state.needfgh )
            {
                state.g.ptr.p_double[0] = s*2*(ae_exp(state.x.ptr.p_double[0], _state)-2)*ae_exp(state.x.ptr.p_double[0], _state)+2*(state.x.ptr.p_double[0]-state.x.ptr.p_double[2]);
                state.g.ptr.p_double[1] = 2*state.x.ptr.p_double[1];
                state.g.ptr.p_double[2] = 2*(state.x.ptr.p_double[2]-state.x.ptr.p_double[0]);
            }
            if( state.needfgh )
            {
                state.h.ptr.pp_double[0][0] = s*(4*ae_sqr(ae_exp(state.x.ptr.p_double[0], _state), _state)-4*ae_exp(state.x.ptr.p_double[0], _state))+2;
                state.h.ptr.pp_double[0][1] = (double)(0);
                state.h.ptr.pp_double[0][2] = (double)(-2);
                state.h.ptr.pp_double[1][0] = (double)(0);
                state.h.ptr.pp_double[1][1] = (double)(2);
                state.h.ptr.pp_double[1][2] = (double)(0);
                state.h.ptr.pp_double[2][0] = (double)(-2);
                state.h.ptr.pp_double[2][1] = (double)(0);
                state.h.ptr.pp_double[2][2] = (double)(2);
            }
            scerror = scerror||!testminlmunit_rkindvsstatecheck(rkind, &state, _state);
            if( state.xupdated )
            {
                othererrors = othererrors||ae_fp_greater(state.f,fprev);
                if( ae_fp_eq(fprev,ae_maxrealnumber) )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        othererrors = othererrors||ae_fp_neq(state.x.ptr.p_double[i],x.ptr.p_double[i]);
                    }
                }
                fprev = state.f;
                ae_v_move(&xlast.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
            }
        }
        minlmresults(&state, &x, &rep, _state);
        for(i=0; i<=n-1; i++)
        {
            othererrors = othererrors||ae_fp_neq(x.ptr.p_double[i],xlast.ptr.p_double[i]);
        }
    }
    n = 1;
    ae_vector_set_length(&x, n, _state);
    x.ptr.p_double[0] = (double)(100);
    stpmax = 0.05+0.05*ae_randomreal(_state);
    minlmcreatefgh(n, &x, &state, _state);
    minlmsetcond(&state, 1.0E-9, (double)(0), (double)(0), 0, _state);
    minlmsetstpmax(&state, stpmax, _state);
    minlmsetxrep(&state, ae_true, _state);
    xprev = x.ptr.p_double[0];
    while(minlmiteration(&state, _state))
    {
        if( (state.needf||state.needfg)||state.needfgh )
        {
            state.f = ae_exp(state.x.ptr.p_double[0], _state)+ae_exp(-state.x.ptr.p_double[0], _state);
        }
        if( state.needfg||state.needfgh )
        {
            state.g.ptr.p_double[0] = ae_exp(state.x.ptr.p_double[0], _state)-ae_exp(-state.x.ptr.p_double[0], _state);
        }
        if( state.needfgh )
        {
            state.h.ptr.pp_double[0][0] = ae_exp(state.x.ptr.p_double[0], _state)+ae_exp(-state.x.ptr.p_double[0], _state);
        }
        othererrors = othererrors||ae_fp_greater(ae_fabs(state.x.ptr.p_double[0]-xprev, _state),(1+ae_sqrt(ae_machineepsilon, _state))*stpmax);
        if( state.xupdated )
        {
            xprev = state.x.ptr.p_double[0];
        }
    }
    
    /*
     * Check algorithm ability to handle request for termination:
     * * to terminate with correct return code = 8
     * * to return point which was "current" at the moment of termination
     */
    for(pass=1; pass<=50; pass++)
    {
        n = 3;
        m = 3;
        s = (double)(100);
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&xlast, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = 6+ae_randomreal(_state);
        }
        stopcallidx = ae_randominteger(20, _state);
        maxits = 25;
        minlmcreatevj(n, m, &x, &state, _state);
        minlmsetcond(&state, (double)(0), (double)(0), (double)(0), maxits, _state);
        minlmsetxrep(&state, ae_true, _state);
        callidx = 0;
        terminationrequested = ae_false;
        ae_v_move(&xlast.ptr.p_double[0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
        while(minlmiteration(&state, _state))
        {
            if( state.needfi||state.needfij )
            {
                state.fi.ptr.p_double[0] = ae_sqrt(s, _state)*(ae_exp(state.x.ptr.p_double[0], _state)-2);
                state.fi.ptr.p_double[1] = state.x.ptr.p_double[1];
                state.fi.ptr.p_double[2] = state.x.ptr.p_double[2]-state.x.ptr.p_double[0];
                if( state.needfij )
                {
                    state.j.ptr.pp_double[0][0] = ae_sqrt(s, _state)*ae_exp(state.x.ptr.p_double[0], _state);
                    state.j.ptr.pp_double[0][1] = (double)(0);
                    state.j.ptr.pp_double[0][2] = (double)(0);
                    state.j.ptr.pp_double[1][0] = (double)(0);
                    state.j.ptr.pp_double[1][1] = (double)(1);
                    state.j.ptr.pp_double[1][2] = (double)(0);
                    state.j.ptr.pp_double[2][0] = (double)(-1);
                    state.j.ptr.pp_double[2][1] = (double)(0);
                    state.j.ptr.pp_double[2][2] = (double)(1);
                }
                if( callidx==stopcallidx )
                {
                    minlmrequesttermination(&state, _state);
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
        minlmresults(&state, &x, &rep, _state);
        seterrorflag(&othererrors, rep.terminationtype!=8, _state);
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(&othererrors, ae_fp_neq(x.ptr.p_double[i],xlast.ptr.p_double[i]), _state);
        }
    }
    
    /*
     *  Test for MinLMGradientCheck
     */
    graderrors = testminlmunit_gradientchecktest(_state);
    
    /*
     * end
     */
    waserrors = (((((((referror||lin1error)||lin2error)||eqerror)||converror)||scerror)||othererrors)||restartserror)||graderrors;
    if( !silent )
    {
        printf("TESTING LEVENBERG-MARQUARDT OPTIMIZATION\n");
        printf("REFERENCE PROBLEMS:                       ");
        if( referror )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("1-D PROBLEM #1:                           ");
        if( lin1error )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("1-D PROBLEM #2:                           ");
        if( lin2error )
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
        printf("CONVERGENCE PROPERTIES:                   ");
        if( converror )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("STATE FIELDS CONSISTENCY:                 ");
        if( scerror )
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
        printf("TEST FOR VERIFICATION OF DERIVATIVES:     ");
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
ae_bool _pexec_testminlm(ae_bool silent, ae_state *_state)
{
    return testminlm(silent, _state);
}


/*************************************************************************
Asserts that State fields are consistent with RKind.
Returns False otherwise.

RKind is an algorithm selector:
* -2 = V, AccType=1
* -1 = V, AccType=0
*  0 = FJ
*  1 = FGJ
*  2 = FGH
*  3 = VJ, AccType=0
*  4 = VJ, AccType=1
*  5 = VJ, AccType=2

*************************************************************************/
static ae_bool testminlmunit_rkindvsstatecheck(ae_int_t rkind,
     minlmstate* state,
     ae_state *_state)
{
    ae_int_t nset;
    ae_bool result;


    nset = 0;
    if( state->needfi )
    {
        nset = nset+1;
    }
    if( state->needf )
    {
        nset = nset+1;
    }
    if( state->needfg )
    {
        nset = nset+1;
    }
    if( state->needfij )
    {
        nset = nset+1;
    }
    if( state->needfgh )
    {
        nset = nset+1;
    }
    if( state->xupdated )
    {
        nset = nset+1;
    }
    if( nset!=1 )
    {
        result = ae_false;
        return result;
    }
    if( rkind==-2 )
    {
        result = state->needfi||state->xupdated;
        return result;
    }
    if( rkind==-1 )
    {
        result = state->needfi||state->xupdated;
        return result;
    }
    if( rkind==0 )
    {
        result = (state->needf||state->needfij)||state->xupdated;
        return result;
    }
    if( rkind==1 )
    {
        result = ((state->needf||state->needfij)||state->needfg)||state->xupdated;
        return result;
    }
    if( rkind==2 )
    {
        result = ((state->needf||state->needfg)||state->needfgh)||state->xupdated;
        return result;
    }
    if( rkind==3 )
    {
        result = (state->needfi||state->needfij)||state->xupdated;
        return result;
    }
    if( rkind==4 )
    {
        result = (state->needfi||state->needfij)||state->xupdated;
        return result;
    }
    if( rkind==5 )
    {
        result = (state->needfi||state->needfij)||state->xupdated;
        return result;
    }
    result = ae_false;
    return result;
}


/*************************************************************************
Calculates FI/F/G/H for problem min(||Ax-b||)
*************************************************************************/
static void testminlmunit_axmb(minlmstate* state,
     /* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* b,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;


    if( (state->needf||state->needfg)||state->needfgh )
    {
        state->f = (double)(0);
    }
    if( state->needfg||state->needfgh )
    {
        for(i=0; i<=n-1; i++)
        {
            state->g.ptr.p_double[i] = (double)(0);
        }
    }
    if( state->needfgh )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                state->h.ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    for(i=0; i<=n-1; i++)
    {
        v = ae_v_dotproduct(&a->ptr.pp_double[i][0], 1, &state->x.ptr.p_double[0], 1, ae_v_len(0,n-1));
        if( (state->needf||state->needfg)||state->needfgh )
        {
            state->f = state->f+ae_sqr(v-b->ptr.p_double[i], _state);
        }
        if( state->needfg||state->needfgh )
        {
            for(j=0; j<=n-1; j++)
            {
                state->g.ptr.p_double[j] = state->g.ptr.p_double[j]+2*(v-b->ptr.p_double[i])*a->ptr.pp_double[i][j];
            }
        }
        if( state->needfgh )
        {
            for(j=0; j<=n-1; j++)
            {
                for(k=0; k<=n-1; k++)
                {
                    state->h.ptr.pp_double[j][k] = state->h.ptr.pp_double[j][k]+2*a->ptr.pp_double[i][j]*a->ptr.pp_double[i][k];
                }
            }
        }
        if( state->needfi )
        {
            state->fi.ptr.p_double[i] = v-b->ptr.p_double[i];
        }
        if( state->needfij )
        {
            state->fi.ptr.p_double[i] = v-b->ptr.p_double[i];
            ae_v_move(&state->j.ptr.pp_double[i][0], 1, &a->ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
        }
    }
}


/*************************************************************************
This function tries to reproduce previously fixed bugs; in case of bug
being present sets Err to True;
*************************************************************************/
static void testminlmunit_tryreproducefixedbugs(ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    minlmstate s;
    minlmreport rep;
    ae_vector bl;
    ae_vector bu;
    ae_vector x;

    ae_frame_make(_state, &_frame_block);
    _minlmstate_init(&s, _state);
    _minlmreport_init(&rep, _state);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);

    
    /*
     * Reproduce bug reported by ISS:
     * when solving bound constrained problem with numerical differentiation
     * and starting from infeasible point, we won't stop at the feasible point
     */
    ae_vector_set_length(&x, 2, _state);
    ae_vector_set_length(&bl, 2, _state);
    ae_vector_set_length(&bu, 2, _state);
    x.ptr.p_double[0] = 2.0;
    bl.ptr.p_double[0] = -1.0;
    bu.ptr.p_double[0] = 1.0;
    x.ptr.p_double[1] = 2.0;
    bl.ptr.p_double[1] = -1.0;
    bu.ptr.p_double[1] = 1.0;
    minlmcreatev(2, 2, &x, 0.001, &s, _state);
    minlmsetbc(&s, &bl, &bu, _state);
    while(minlmiteration(&s, _state))
    {
        if( s.needfi )
        {
            s.fi.ptr.p_double[0] = ae_sqr(s.x.ptr.p_double[0], _state);
            s.fi.ptr.p_double[1] = ae_sqr(s.x.ptr.p_double[1], _state);
        }
    }
    minlmresults(&s, &x, &rep, _state);
    *err = ((ae_fp_less(x.ptr.p_double[0],bl.ptr.p_double[0])||ae_fp_greater(x.ptr.p_double[0],bu.ptr.p_double[0]))||ae_fp_less(x.ptr.p_double[1],bl.ptr.p_double[1]))||ae_fp_greater(x.ptr.p_double[1],bu.ptr.p_double[1]);
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests, that gradient verified correctly.
*************************************************************************/
static ae_bool testminlmunit_gradientchecktest(ae_state *_state)
{
    ae_frame _frame_block;
    minlmstate state;
    minlmreport rep;
    ae_int_t n;
    ae_int_t m;
    ae_vector a;
    ae_vector x0;
    ae_vector x;
    ae_vector bl;
    ae_vector bu;
    ae_int_t infcomp;
    double teststep;
    double noise;
    double rndconst;
    ae_int_t nbrfunc;
    ae_int_t nbrcomp;
    double spp;
    ae_int_t func;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t i;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _minlmstate_init(&state, _state);
    _minlmreport_init(&rep, _state);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);

    passcount = 35;
    spp = 1.0;
    teststep = 0.01;
    for(pass=1; pass<=passcount; pass++)
    {
        n = ae_randominteger(10, _state)+1;
        m = ae_randominteger(10, _state)+1;
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&a, n, _state);
        ae_vector_set_length(&bl, n, _state);
        ae_vector_set_length(&bu, n, _state);
        
        /*
         * Prepare test's parameters
         */
        func = ae_randominteger(3, _state)+1;
        nbrfunc = ae_randominteger(m, _state);
        nbrcomp = ae_randominteger(n, _state);
        noise = (double)(2*ae_randominteger(2, _state)-1);
        rndconst = 2*ae_randomreal(_state)-1;
        
        /*
         * Prepare function's parameters
         */
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = 5*randomnormal(_state);
            a.ptr.p_double[i] = 5*ae_randomreal(_state)+1;
            x0.ptr.p_double[i] = 5*(2*ae_randomreal(_state)-1);
        }
        
        /*
         * Prepare boundary parameters
         */
        for(i=0; i<=n-1; i++)
        {
            bl.ptr.p_double[i] = -3*ae_randomreal(_state)-0.1;
            bu.ptr.p_double[i] = 3*ae_randomreal(_state)+0.1;
        }
        infcomp = ae_randominteger(n+1, _state);
        if( infcomp<n )
        {
            bl.ptr.p_double[infcomp] = _state->v_neginf;
        }
        infcomp = ae_randominteger(n+1, _state);
        if( infcomp<n )
        {
            bu.ptr.p_double[infcomp] = _state->v_posinf;
        }
        minlmcreatevj(n, m, &x, &state, _state);
        minlmsetcond(&state, (double)(0), (double)(0), (double)(0), 0, _state);
        minlmsetgradientcheck(&state, teststep, _state);
        minlmsetbc(&state, &bl, &bu, _state);
        
        /*
         * Check that the criterion passes a derivative if it is correct
         */
        while(minlmiteration(&state, _state))
        {
            if( state.needfij )
            {
                
                /*
                 * Check hat .X within the boundaries
                 */
                for(i=0; i<=n-1; i++)
                {
                    if( (ae_isfinite(bl.ptr.p_double[i], _state)&&ae_fp_less(state.x.ptr.p_double[i],bl.ptr.p_double[i]))||(ae_isfinite(bu.ptr.p_double[i], _state)&&ae_fp_greater(state.x.ptr.p_double[i],bu.ptr.p_double[i])) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
                testminlmunit_funcderiv(&a, &x0, &state.x, m, n, rndconst, func, &state.fi, &state.j, _state);
            }
        }
        minlmresults(&state, &x, &rep, _state);
        
        /*
         * Check that error code does not equal to -7 and parameter .VarIdx
         * equal to -1.
         */
        if( (rep.terminationtype==-7||rep.funcidx!=-1)||rep.varidx!=-1 )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = 5*randomnormal(_state);
        }
        minlmrestartfrom(&state, &x, _state);
        
        /*
         * Check that the criterion does not miss a derivative if
         * it is incorrect
         */
        while(minlmiteration(&state, _state))
        {
            if( state.needfij )
            {
                for(i=0; i<=n-1; i++)
                {
                    if( (ae_isfinite(bl.ptr.p_double[i], _state)&&ae_fp_less(state.x.ptr.p_double[i],bl.ptr.p_double[i]))||(ae_isfinite(bu.ptr.p_double[i], _state)&&ae_fp_greater(state.x.ptr.p_double[i],bu.ptr.p_double[i])) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
                testminlmunit_funcderiv(&a, &x0, &state.x, m, n, rndconst, func, &state.fi, &state.j, _state);
                state.j.ptr.pp_double[nbrfunc][nbrcomp] = state.j.ptr.pp_double[nbrfunc][nbrcomp]+noise;
            }
        }
        minlmresults(&state, &x, &rep, _state);
        
        /*
         * Check that error code equal to -7 and parameter .VarIdx
         * equal to number of incorrect component.
         */
        if( (rep.terminationtype!=-7||rep.funcidx!=nbrfunc)||rep.varidx!=nbrcomp )
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
This function return function value and it derivatives. The number of
functions is M, dimension for each of functions is N.
F(XI)=SUM(fi(XI)); (XI={x,y,z}; i=0..M-1);
    Function's list:
        * funcType=1:
            fi(X)=(Aj*Xj-X0j)^2;
        * funcType=2:
            fi(X)=Aj*sin(Xj-X0j);
        * funcType=3:
            fi(X)=Aj*Xj-X0j;
            fM-1(X)=A(M-1)*((X(M-1)-X0(M-1))-(X0-X00)).
*************************************************************************/
static void testminlmunit_funcderiv(/* Real    */ ae_vector* a,
     /* Real    */ ae_vector* x0,
     /* Real    */ ae_vector* x,
     ae_int_t m,
     ae_int_t n,
     double anyconst,
     ae_int_t functype,
     /* Real    */ ae_vector* f,
     /* Real    */ ae_matrix* j,
     ae_state *_state)
{
    ae_int_t i0;
    ae_int_t j0;


    ae_assert(functype>=1&&functype<=3, "FuncDeriv: incorrect funcType(funcType<1 or funcType>3).", _state);
    ae_assert(n>0, "FuncDeriv: N<=0", _state);
    ae_assert(m>0, "FuncDeriv: M<=0", _state);
    ae_assert(x->cnt>=n, "FuncDeriv: Length(X)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "FuncDeriv: X contains NaN or Infinite.", _state);
    ae_assert(x0->cnt>=n, "FuncDeriv: Length(X0)<N", _state);
    ae_assert(isfinitevector(x0, n, _state), "FuncDeriv: X0 contains NaN or Infinite.", _state);
    ae_assert(a->cnt>=n, "FuncDeriv: Length(X)<N", _state);
    ae_assert(isfinitevector(a, n, _state), "FuncDeriv: A contains NaN or Infinite.", _state);
    if( functype==1 )
    {
        for(i0=0; i0<=m-1; i0++)
        {
            if( i0<n )
            {
                f->ptr.p_double[i0] = a->ptr.p_double[i0]*x->ptr.p_double[i0]-x0->ptr.p_double[i0];
            }
            else
            {
                f->ptr.p_double[i0] = anyconst;
            }
        }
        for(i0=0; i0<=m-1; i0++)
        {
            for(j0=0; j0<=n-1; j0++)
            {
                if( i0==j0 )
                {
                    j->ptr.pp_double[i0][j0] = a->ptr.p_double[j0];
                }
                else
                {
                    j->ptr.pp_double[i0][j0] = (double)(0);
                }
            }
        }
        return;
    }
    if( functype==2 )
    {
        for(i0=0; i0<=m-1; i0++)
        {
            if( i0<n )
            {
                f->ptr.p_double[i0] = a->ptr.p_double[i0]*ae_sin(x->ptr.p_double[i0]-x0->ptr.p_double[i0], _state);
            }
            else
            {
                f->ptr.p_double[i0] = anyconst;
            }
        }
        for(i0=0; i0<=m-1; i0++)
        {
            for(j0=0; j0<=n-1; j0++)
            {
                if( i0==j0 )
                {
                    j->ptr.pp_double[i0][j0] = a->ptr.p_double[j0]*ae_cos(x->ptr.p_double[j0]-x0->ptr.p_double[j0], _state);
                }
                else
                {
                    j->ptr.pp_double[i0][j0] = (double)(0);
                }
            }
        }
        return;
    }
    if( functype==3 )
    {
        for(i0=0; i0<=m-1; i0++)
        {
            if( i0<n )
            {
                f->ptr.p_double[i0] = a->ptr.p_double[i0]*x->ptr.p_double[i0]-x0->ptr.p_double[i0];
            }
            else
            {
                f->ptr.p_double[i0] = anyconst;
            }
        }
        for(i0=0; i0<=m-1; i0++)
        {
            for(j0=0; j0<=n-1; j0++)
            {
                if( i0==j0 )
                {
                    j->ptr.pp_double[i0][j0] = a->ptr.p_double[j0];
                }
                else
                {
                    j->ptr.pp_double[i0][j0] = (double)(0);
                }
            }
        }
        if( m>n&&n>1 )
        {
            f->ptr.p_double[n-1] = a->ptr.p_double[n-1]*(x->ptr.p_double[n-1]-x0->ptr.p_double[n-1]-(x->ptr.p_double[0]-x0->ptr.p_double[0]));
            j->ptr.pp_double[n-1][0] = -a->ptr.p_double[n-1];
            j->ptr.pp_double[n-1][n-1] = a->ptr.p_double[n-1];
            for(i0=1; i0<=n-2; i0++)
            {
                j->ptr.pp_double[n-1][i0] = (double)(0);
            }
        }
        return;
    }
}


/*$ End $*/
