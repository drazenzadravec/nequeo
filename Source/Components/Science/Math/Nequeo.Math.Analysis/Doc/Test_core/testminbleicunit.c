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
#include "testminbleicunit.h"


/*$ Declarations $*/
static void testminbleicunit_calciip2(minbleicstate* state,
     ae_int_t n,
     ae_int_t fk,
     ae_state *_state);
static void testminbleicunit_testfeasibility(ae_bool* feaserr,
     ae_bool* converr,
     ae_bool* interr,
     ae_state *_state);
static void testminbleicunit_testother(ae_bool* err, ae_state *_state);
static void testminbleicunit_testconv(ae_bool* err, ae_state *_state);
static void testminbleicunit_testpreconditioning(ae_bool* err,
     ae_state *_state);
static void testminbleicunit_setrandompreconditioner(minbleicstate* state,
     ae_int_t n,
     ae_int_t preckind,
     ae_state *_state);
static void testminbleicunit_testgradientcheck(ae_bool* testg,
     ae_state *_state);
static void testminbleicunit_testbugs(ae_bool* err, ae_state *_state);
static void testminbleicunit_funcderiv(double a,
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


ae_bool testminbleic(ae_bool silent, ae_state *_state)
{
    ae_bool waserrors;
    ae_bool feasibilityerrors;
    ae_bool othererrors;
    ae_bool precerrors;
    ae_bool interrors;
    ae_bool converrors;
    ae_bool graderrors;
    ae_bool bugs;
    ae_bool result;


    waserrors = ae_false;
    feasibilityerrors = ae_false;
    othererrors = ae_false;
    precerrors = ae_false;
    interrors = ae_false;
    converrors = ae_false;
    graderrors = ae_false;
    bugs = ae_false;
    testminbleicunit_testfeasibility(&feasibilityerrors, &converrors, &interrors, _state);
    testminbleicunit_testother(&othererrors, _state);
    testminbleicunit_testconv(&converrors, _state);
    testminbleicunit_testbugs(&bugs, _state);
    testminbleicunit_testpreconditioning(&precerrors, _state);
    testminbleicunit_testgradientcheck(&graderrors, _state);
    
    /*
     * end
     */
    waserrors = (((((feasibilityerrors||othererrors)||converrors)||interrors)||precerrors)||graderrors)||bugs;
    if( !silent )
    {
        printf("TESTING BLEIC OPTIMIZATION\n");
        printf("FEASIBILITY PROPERTIES:                   ");
        if( feasibilityerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("PRECONDITIONING:                          ");
        if( precerrors )
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
        printf("CONVERGENCE PROPERTIES:                   ");
        if( converrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("INTERNAL ERRORS:                          ");
        if( interrors )
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
        printf("FIXED BUGS:                               ");
        if( bugs )
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
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testminbleic(ae_bool silent, ae_state *_state)
{
    return testminbleic(silent, _state);
}


/*************************************************************************
Calculate test function IIP2

f(x) = sum( ((i*i+1)^FK*x[i])^2, i=0..N-1)

It has high condition number which makes fast convergence unlikely without
good preconditioner.

*************************************************************************/
static void testminbleicunit_calciip2(minbleicstate* state,
     ae_int_t n,
     ae_int_t fk,
     ae_state *_state)
{
    ae_int_t i;


    if( state->needfg )
    {
        state->f = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        if( state->needfg )
        {
            state->f = state->f+ae_pow((double)(i*i+1), (double)(2*fk), _state)*ae_sqr(state->x.ptr.p_double[i], _state);
            state->g.ptr.p_double[i] = ae_pow((double)(i*i+1), (double)(2*fk), _state)*2*state->x.ptr.p_double[i];
        }
    }
}


/*************************************************************************
This function test feasibility properties.
It launches a sequence of problems and examines their solutions.
Most of the attention is directed towards feasibility properties,
although we make some quick checks to ensure that actual solution is found.

On failure sets FeasErr (or ConvErr, depending on failure type) to True,
or leaves it unchanged otherwise.

IntErr is set to True on internal errors (errors in the control flow).
*************************************************************************/
static void testminbleicunit_testfeasibility(ae_bool* feaserr,
     ae_bool* converr,
     ae_bool* interr,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pkind;
    ae_int_t preckind;
    ae_int_t passcount;
    ae_int_t pass;
    ae_int_t n;
    ae_int_t nmax;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t p;
    double v;
    double v2;
    double v3;
    double vv;
    ae_vector bl;
    ae_vector bu;
    ae_vector x;
    ae_vector g;
    ae_vector x0;
    ae_vector xc;
    ae_vector xs;
    ae_vector svdw;
    ae_matrix c;
    ae_matrix svdu;
    ae_matrix svdvt;
    ae_vector ct;
    minbleicstate state;
    double epsx;
    double epsg;
    double epsfeas;
    double weakepsg;
    minbleicreport rep;
    ae_int_t dkind;
    double diffstep;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&xs, 0, DT_REAL, _state);
    ae_vector_init(&svdw, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&svdu, 0, 0, DT_REAL, _state);
    ae_matrix_init(&svdvt, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    _minbleicstate_init(&state, _state);
    _minbleicreport_init(&rep, _state);

    nmax = 5;
    epsg = 1.0E-8;
    weakepsg = 1.0E-4;
    epsx = 1.0E-4;
    epsfeas = 1.0E-6;
    passcount = 10;
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Test problem 1:
         * * no boundary and inequality constraints
         * * randomly generated plane as equality constraint
         * * random point (not necessarily on the plane)
         * * f = |x|^P, P = {2, 4} is used as target function
         * * preconditioner is chosen at random (we just want to be
         *   sure that preconditioning won't prevent us from converging
         *   to the feasible point):
         *   * unit preconditioner
         *   * random diagonal-based preconditioner
         *   * random scale-based preconditioner
         * * either analytic gradient or numerical differentiation are used
         * * we check that after work is over we are on the plane and
         *   that we are in the stationary point of constrained F
         */
        diffstep = 1.0E-6;
        for(dkind=0; dkind<=1; dkind++)
        {
            for(preckind=0; preckind<=2; preckind++)
            {
                for(pkind=1; pkind<=2; pkind++)
                {
                    for(n=1; n<=nmax; n++)
                    {
                        
                        /*
                         * Generate X, BL, BU, CT and left part of C.
                         *
                         * Right part of C is generated using somewhat complex algo:
                         * * we generate random vector and multiply it by C.
                         * * result is used as the right part.
                         * * calculations are done on the fly, vector itself is not stored
                         * We use such algo to be sure that our system is consistent.
                         */
                        p = 2*pkind;
                        ae_vector_set_length(&x, n, _state);
                        ae_vector_set_length(&g, n, _state);
                        ae_matrix_set_length(&c, 1, n+1, _state);
                        ae_vector_set_length(&ct, 1, _state);
                        c.ptr.pp_double[0][n] = (double)(0);
                        for(i=0; i<=n-1; i++)
                        {
                            x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                            c.ptr.pp_double[0][i] = 2*ae_randomreal(_state)-1;
                            v = 2*ae_randomreal(_state)-1;
                            c.ptr.pp_double[0][n] = c.ptr.pp_double[0][n]+c.ptr.pp_double[0][i]*v;
                        }
                        ct.ptr.p_int[0] = 0;
                        
                        /*
                         * Create and optimize
                         */
                        if( dkind==0 )
                        {
                            minbleiccreate(n, &x, &state, _state);
                        }
                        if( dkind==1 )
                        {
                            minbleiccreatef(n, &x, diffstep, &state, _state);
                        }
                        minbleicsetlc(&state, &c, &ct, 1, _state);
                        minbleicsetcond(&state, weakepsg, 0.0, 0.0, 0, _state);
                        testminbleicunit_setrandompreconditioner(&state, n, preckind, _state);
                        while(minbleiciteration(&state, _state))
                        {
                            if( state.needf||state.needfg )
                            {
                                state.f = (double)(0);
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                if( state.needf||state.needfg )
                                {
                                    state.f = state.f+ae_pow(state.x.ptr.p_double[i], (double)(p), _state);
                                }
                                if( state.needfg )
                                {
                                    state.g.ptr.p_double[i] = p*ae_pow(state.x.ptr.p_double[i], (double)(p-1), _state);
                                }
                            }
                        }
                        minbleicresults(&state, &x, &rep, _state);
                        if( rep.terminationtype<=0 )
                        {
                            *converr = ae_true;
                            ae_frame_leave(_state);
                            return;
                        }
                        
                        /*
                         * Test feasibility of solution
                         */
                        v = ae_v_dotproduct(&c.ptr.pp_double[0][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                        *feaserr = *feaserr||ae_fp_greater(ae_fabs(v-c.ptr.pp_double[0][n], _state),epsfeas);
                        
                        /*
                         * if C is nonzero, test that result is
                         * a stationary point of constrained F.
                         *
                         * NOTE: this check is done only if C is nonzero
                         */
                        vv = ae_v_dotproduct(&c.ptr.pp_double[0][0], 1, &c.ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
                        if( ae_fp_neq(vv,(double)(0)) )
                        {
                            
                            /*
                             * Calculate gradient at the result
                             * Project gradient into C
                             * Check projected norm
                             */
                            for(i=0; i<=n-1; i++)
                            {
                                g.ptr.p_double[i] = p*ae_pow(x.ptr.p_double[i], (double)(p-1), _state);
                            }
                            v2 = ae_v_dotproduct(&c.ptr.pp_double[0][0], 1, &c.ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
                            v = ae_v_dotproduct(&c.ptr.pp_double[0][0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,n-1));
                            vv = v/v2;
                            ae_v_subd(&g.ptr.p_double[0], 1, &c.ptr.pp_double[0][0], 1, ae_v_len(0,n-1), vv);
                            v3 = ae_v_dotproduct(&g.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,n-1));
                            *converr = *converr||ae_fp_greater(ae_sqrt(v3, _state),weakepsg);
                        }
                    }
                }
            }
        }
        
        /*
         * Test problem 2 (multiple equality constraints):
         * * 1<=N<=NMax, 1<=K<=N
         * * no boundary constraints
         * * N-dimensional space
         * * randomly generated point xs
         * * K randomly generated hyperplanes which all pass through xs
         *   define K equality constraints: (a[k],x)=b[k]
         * * equality constraints are checked for being well conditioned
         * * preconditioner is chosen at random (we just want to be
         *   sure that preconditioning won't prevent us from converging
         *   to the feasible point):
         *   * unit preconditioner
         *   * random diagonal-based preconditioner
         *   * random scale-based preconditioner
         * * f(x) = |x-x0|^2, x0 = xs+a[0]
         * * either analytic gradient or numerical differentiation are used
         * * extremum of f(x) is exactly xs because:
         *   * xs is the closest point in the plane defined by (a[0],x)=b[0]
         *   * xs is feasible by definition
         */
        diffstep = 1.0E-6;
        for(dkind=0; dkind<=1; dkind++)
        {
            for(preckind=0; preckind<=2; preckind++)
            {
                for(n=2; n<=nmax; n++)
                {
                    for(k=1; k<=n; k++)
                    {
                        
                        /*
                         * Generate X, X0, XS, BL, BU, CT and left part of C.
                         *
                         * Right part of C is generated using somewhat complex algo:
                         * * we generate random vector and multiply it by C.
                         * * result is used as the right part.
                         * * calculations are done on the fly, vector itself is not stored
                         * We use such algo to be sure that our system is consistent.
                         */
                        ae_vector_set_length(&x, n, _state);
                        ae_vector_set_length(&x0, n, _state);
                        ae_vector_set_length(&xs, n, _state);
                        ae_vector_set_length(&g, n, _state);
                        ae_matrix_set_length(&c, k, n+1, _state);
                        ae_vector_set_length(&ct, k, _state);
                        c.ptr.pp_double[0][n] = (double)(0);
                        for(i=0; i<=n-1; i++)
                        {
                            x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                            xs.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        }
                        do
                        {
                            for(i=0; i<=k-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                                }
                                v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &xs.ptr.p_double[0], 1, ae_v_len(0,n-1));
                                c.ptr.pp_double[i][n] = v;
                                ct.ptr.p_int[i] = 0;
                            }
                            seterrorflag(feaserr, !rmatrixsvd(&c, k, n, 0, 0, 0, &svdw, &svdu, &svdvt, _state), _state);
                        }
                        while(!(ae_fp_greater(svdw.ptr.p_double[0],(double)(0))&&ae_fp_greater(svdw.ptr.p_double[k-1],0.001*svdw.ptr.p_double[0])));
                        ae_v_move(&x0.ptr.p_double[0], 1, &xs.ptr.p_double[0], 1, ae_v_len(0,n-1));
                        ae_v_add(&x0.ptr.p_double[0], 1, &c.ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
                        
                        /*
                         * Create and optimize
                         */
                        if( dkind==0 )
                        {
                            minbleiccreate(n, &x, &state, _state);
                        }
                        if( dkind==1 )
                        {
                            minbleiccreatef(n, &x, diffstep, &state, _state);
                        }
                        minbleicsetlc(&state, &c, &ct, k, _state);
                        minbleicsetcond(&state, weakepsg, 0.0, 0.0, 0, _state);
                        testminbleicunit_setrandompreconditioner(&state, n, preckind, _state);
                        while(minbleiciteration(&state, _state))
                        {
                            if( state.needf||state.needfg )
                            {
                                state.f = (double)(0);
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                if( state.needf||state.needfg )
                                {
                                    state.f = state.f+ae_sqr(state.x.ptr.p_double[i]-x0.ptr.p_double[i], _state);
                                }
                                if( state.needfg )
                                {
                                    state.g.ptr.p_double[i] = 2*(state.x.ptr.p_double[i]-x0.ptr.p_double[i]);
                                }
                            }
                        }
                        minbleicresults(&state, &x, &rep, _state);
                        if( rep.terminationtype<=0 )
                        {
                            *converr = ae_true;
                            ae_frame_leave(_state);
                            return;
                        }
                        
                        /*
                         * check feasiblity properties
                         */
                        for(i=0; i<=k-1; i++)
                        {
                            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                            *feaserr = *feaserr||ae_fp_greater(ae_fabs(v-c.ptr.pp_double[i][n], _state),epsx);
                        }
                        
                        /*
                         * Compare with XS
                         */
                        v = (double)(0);
                        for(i=0; i<=n-1; i++)
                        {
                            v = v+ae_sqr(x.ptr.p_double[i]-xs.ptr.p_double[i], _state);
                        }
                        v = ae_sqrt(v, _state);
                        *converr = *converr||ae_fp_greater(ae_fabs(v, _state),0.001);
                    }
                }
            }
        }
        
        /*
         * Another simple problem:
         * * bound constraints 0 <= x[i] <= 1
         * * no linear constraints
         * * preconditioner is chosen at random (we just want to be
         *   sure that preconditioning won't prevent us from converging
         *   to the feasible point):
         *   * unit preconditioner
         *   * random diagonal-based preconditioner
         *   * random scale-based preconditioner
         * * F(x) = |x-x0|^P, where P={2,4} and x0 is randomly selected from [-1,+2]^N
         * * with such simple boundaries and function it is easy to find
         *   analytic form of solution: S[i] = bound(x0[i], 0, 1)
         * * we also check that both final solution and subsequent iterates
         *   are strictly feasible
         */
        diffstep = 1.0E-6;
        for(dkind=0; dkind<=1; dkind++)
        {
            for(preckind=0; preckind<=2; preckind++)
            {
                for(pkind=1; pkind<=2; pkind++)
                {
                    for(n=1; n<=nmax; n++)
                    {
                        
                        /*
                         * Generate X, BL, BU.
                         */
                        p = 2*pkind;
                        ae_vector_set_length(&bl, n, _state);
                        ae_vector_set_length(&bu, n, _state);
                        ae_vector_set_length(&x, n, _state);
                        ae_vector_set_length(&x0, n, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            bl.ptr.p_double[i] = (double)(0);
                            bu.ptr.p_double[i] = (double)(1);
                            x.ptr.p_double[i] = ae_randomreal(_state);
                            x0.ptr.p_double[i] = 3*ae_randomreal(_state)-1;
                        }
                        
                        /*
                         * Create and optimize
                         */
                        if( dkind==0 )
                        {
                            minbleiccreate(n, &x, &state, _state);
                        }
                        if( dkind==1 )
                        {
                            minbleiccreatef(n, &x, diffstep, &state, _state);
                        }
                        minbleicsetbc(&state, &bl, &bu, _state);
                        minbleicsetcond(&state, weakepsg, 0.0, 0.0, 0, _state);
                        testminbleicunit_setrandompreconditioner(&state, n, preckind, _state);
                        while(minbleiciteration(&state, _state))
                        {
                            if( state.needf||state.needfg )
                            {
                                state.f = (double)(0);
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                if( state.needf||state.needfg )
                                {
                                    state.f = state.f+ae_pow(state.x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p), _state);
                                }
                                if( state.needfg )
                                {
                                    state.g.ptr.p_double[i] = p*ae_pow(state.x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p-1), _state);
                                }
                                *feaserr = *feaserr||ae_fp_less(state.x.ptr.p_double[i],0.0);
                                *feaserr = *feaserr||ae_fp_greater(state.x.ptr.p_double[i],1.0);
                            }
                        }
                        minbleicresults(&state, &x, &rep, _state);
                        if( rep.terminationtype<=0 )
                        {
                            *converr = ae_true;
                            ae_frame_leave(_state);
                            return;
                        }
                        
                        /*
                         * * compare solution with analytic one
                         * * check feasibility
                         */
                        v = 0.0;
                        for(i=0; i<=n-1; i++)
                        {
                            if( ae_fp_greater(x.ptr.p_double[i],(double)(0))&&ae_fp_less(x.ptr.p_double[i],(double)(1)) )
                            {
                                v = v+ae_sqr(p*ae_pow(x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p-1), _state), _state);
                            }
                            *feaserr = *feaserr||ae_fp_less(x.ptr.p_double[i],0.0);
                            *feaserr = *feaserr||ae_fp_greater(x.ptr.p_double[i],1.0);
                        }
                        *converr = *converr||ae_fp_greater(ae_sqrt(v, _state),weakepsg);
                    }
                }
            }
        }
        
        /*
         * Same as previous problem, but with minor modifications:
         * * some bound constraints are 0<=x[i]<=1, some are Ci=x[i]=Ci
         * * no linear constraints
         * * preconditioner is chosen at random (we just want to be
         *   sure that preconditioning won't prevent us from converging
         *   to the feasible point):
         *   * unit preconditioner
         *   * random diagonal-based preconditioner
         *   * random scale-based preconditioner
         * * F(x) = |x-x0|^P, where P={2,4} and x0 is randomly selected from [-1,+2]^N
         * * with such simple boundaries and function it is easy to find
         *   analytic form of solution: S[i] = bound(x0[i], 0, 1)
         * * we also check that both final solution and subsequent iterates
         *   are strictly feasible
         */
        diffstep = 1.0E-6;
        for(dkind=0; dkind<=1; dkind++)
        {
            for(preckind=0; preckind<=2; preckind++)
            {
                for(pkind=1; pkind<=2; pkind++)
                {
                    for(n=1; n<=nmax; n++)
                    {
                        
                        /*
                         * Generate X, BL, BU.
                         */
                        p = 2*pkind;
                        ae_vector_set_length(&bl, n, _state);
                        ae_vector_set_length(&bu, n, _state);
                        ae_vector_set_length(&x, n, _state);
                        ae_vector_set_length(&x0, n, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            if( ae_fp_greater(ae_randomreal(_state),0.5) )
                            {
                                bl.ptr.p_double[i] = (double)(0);
                                bu.ptr.p_double[i] = (double)(1);
                            }
                            else
                            {
                                bl.ptr.p_double[i] = ae_randomreal(_state);
                                bu.ptr.p_double[i] = bl.ptr.p_double[i];
                            }
                            x.ptr.p_double[i] = ae_randomreal(_state);
                            x0.ptr.p_double[i] = 3*ae_randomreal(_state)-1;
                        }
                        
                        /*
                         * Create and optimize
                         */
                        if( dkind==0 )
                        {
                            minbleiccreate(n, &x, &state, _state);
                        }
                        if( dkind==1 )
                        {
                            minbleiccreatef(n, &x, diffstep, &state, _state);
                        }
                        minbleicsetbc(&state, &bl, &bu, _state);
                        minbleicsetcond(&state, weakepsg, 0.0, 0.0, 0, _state);
                        testminbleicunit_setrandompreconditioner(&state, n, preckind, _state);
                        while(minbleiciteration(&state, _state))
                        {
                            if( state.needf||state.needfg )
                            {
                                state.f = (double)(0);
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                if( state.needf||state.needfg )
                                {
                                    state.f = state.f+ae_pow(state.x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p), _state);
                                }
                                if( state.needfg )
                                {
                                    state.g.ptr.p_double[i] = p*ae_pow(state.x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p-1), _state);
                                }
                                *feaserr = *feaserr||ae_fp_less(state.x.ptr.p_double[i],bl.ptr.p_double[i]);
                                *feaserr = *feaserr||ae_fp_greater(state.x.ptr.p_double[i],bu.ptr.p_double[i]);
                            }
                        }
                        minbleicresults(&state, &x, &rep, _state);
                        if( rep.terminationtype<=0 )
                        {
                            *converr = ae_true;
                            ae_frame_leave(_state);
                            return;
                        }
                        
                        /*
                         * * compare solution with analytic one
                         * * check feasibility
                         */
                        v = 0.0;
                        for(i=0; i<=n-1; i++)
                        {
                            if( ae_fp_greater(x.ptr.p_double[i],bl.ptr.p_double[i])&&ae_fp_less(x.ptr.p_double[i],bu.ptr.p_double[i]) )
                            {
                                v = v+ae_sqr(p*ae_pow(x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p-1), _state), _state);
                            }
                            *feaserr = *feaserr||ae_fp_less(x.ptr.p_double[i],bl.ptr.p_double[i]);
                            *feaserr = *feaserr||ae_fp_greater(x.ptr.p_double[i],bu.ptr.p_double[i]);
                        }
                        *converr = *converr||ae_fp_greater(ae_sqrt(v, _state),weakepsg);
                    }
                }
            }
        }
        
        /*
         * Same as previous one, but with bound constraints posed
         * as general linear ones:
         * * no bound constraints
         * * 2*N linear constraints 0 <= x[i] <= 1
         * * preconditioner is chosen at random (we just want to be
         *   sure that preconditioning won't prevent us from converging
         *   to the feasible point):
         *   * unit preconditioner
         *   * random diagonal-based preconditioner
         *   * random scale-based preconditioner
         * * F(x) = |x-x0|^P, where P={2,4} and x0 is randomly selected from [-1,+2]^N
         * * with such simple constraints and function it is easy to find
         *   analytic form of solution: S[i] = bound(x0[i], 0, 1).
         * * however, we can't guarantee that solution is strictly feasible
         *   with respect to nonlinearity constraint, so we check
         *   for approximate feasibility.
         */
        for(preckind=0; preckind<=2; preckind++)
        {
            for(pkind=1; pkind<=2; pkind++)
            {
                for(n=1; n<=nmax; n++)
                {
                    
                    /*
                     * Generate X, BL, BU.
                     */
                    p = 2*pkind;
                    ae_vector_set_length(&x, n, _state);
                    ae_vector_set_length(&x0, n, _state);
                    ae_matrix_set_length(&c, 2*n, n+1, _state);
                    ae_vector_set_length(&ct, 2*n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        x.ptr.p_double[i] = ae_randomreal(_state);
                        x0.ptr.p_double[i] = 3*ae_randomreal(_state)-1;
                        for(j=0; j<=n; j++)
                        {
                            c.ptr.pp_double[2*i+0][j] = (double)(0);
                            c.ptr.pp_double[2*i+1][j] = (double)(0);
                        }
                        c.ptr.pp_double[2*i+0][i] = (double)(1);
                        c.ptr.pp_double[2*i+0][n] = (double)(0);
                        ct.ptr.p_int[2*i+0] = 1;
                        c.ptr.pp_double[2*i+1][i] = (double)(1);
                        c.ptr.pp_double[2*i+1][n] = (double)(1);
                        ct.ptr.p_int[2*i+1] = -1;
                    }
                    
                    /*
                     * Create and optimize
                     */
                    minbleiccreate(n, &x, &state, _state);
                    minbleicsetlc(&state, &c, &ct, 2*n, _state);
                    minbleicsetcond(&state, weakepsg, 0.0, 0.0, 0, _state);
                    testminbleicunit_setrandompreconditioner(&state, n, preckind, _state);
                    while(minbleiciteration(&state, _state))
                    {
                        if( state.needfg )
                        {
                            state.f = (double)(0);
                            for(i=0; i<=n-1; i++)
                            {
                                state.f = state.f+ae_pow(state.x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p), _state);
                                state.g.ptr.p_double[i] = p*ae_pow(state.x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p-1), _state);
                            }
                            continue;
                        }
                        
                        /*
                         * Unknown protocol specified
                         */
                        *interr = ae_true;
                        ae_frame_leave(_state);
                        return;
                    }
                    minbleicresults(&state, &x, &rep, _state);
                    if( rep.terminationtype<=0 )
                    {
                        *converr = ae_true;
                        ae_frame_leave(_state);
                        return;
                    }
                    
                    /*
                     * * compare solution with analytic one
                     * * check feasibility
                     */
                    v = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        if( ae_fp_greater(x.ptr.p_double[i],0.02)&&ae_fp_less(x.ptr.p_double[i],0.98) )
                        {
                            v = v+ae_sqr(p*ae_pow(x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p-1), _state), _state);
                        }
                        *feaserr = *feaserr||ae_fp_less(x.ptr.p_double[i],0.0-epsfeas);
                        *feaserr = *feaserr||ae_fp_greater(x.ptr.p_double[i],1.0+epsfeas);
                    }
                    *converr = *converr||ae_fp_greater(ae_sqrt(v, _state),weakepsg);
                }
            }
        }
        
        /*
         * Feasibility problem:
         * * bound constraints 0<=x[i]<=1
         * * starting point xs with xs[i] in [-1,+2]
         * * random point xc from [0,1] is used to generate K<=N
         *   random linear equality/inequality constraints of the form
         *   (c,x-xc)=0.0 (or, alternatively, >= or <=), where
         *   c is a random vector.
         * * preconditioner is chosen at random (we just want to be
         *   sure that preconditioning won't prevent us from converging
         *   to the feasible point):
         *   * unit preconditioner
         *   * random diagonal-based preconditioner
         *   * random scale-based preconditioner
         * * F(x) = |x-x0|^P, where P={2,4} and x0 is randomly selected from [-1,+2]^N
         * * we do not know analytic form of the solution, and, if fact, we do not
         *   check for solution correctness. We just check that algorithm converges
         *   to the feasible points.
         */
        for(preckind=0; preckind<=2; preckind++)
        {
            for(pkind=1; pkind<=2; pkind++)
            {
                for(n=1; n<=nmax; n++)
                {
                    for(k=1; k<=n; k++)
                    {
                        
                        /*
                         * Generate X, BL, BU.
                         */
                        p = 2*pkind;
                        ae_vector_set_length(&x0, n, _state);
                        ae_vector_set_length(&xc, n, _state);
                        ae_vector_set_length(&xs, n, _state);
                        ae_matrix_set_length(&c, k, n+1, _state);
                        ae_vector_set_length(&ct, k, _state);
                        ae_vector_set_length(&bl, n, _state);
                        ae_vector_set_length(&bu, n, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            x0.ptr.p_double[i] = 3*ae_randomreal(_state)-1;
                            xs.ptr.p_double[i] = 3*ae_randomreal(_state)-1;
                            xc.ptr.p_double[i] = 0.1+0.8*ae_randomreal(_state);
                            bl.ptr.p_double[i] = (double)(0);
                            bu.ptr.p_double[i] = (double)(1);
                        }
                        for(i=0; i<=k-1; i++)
                        {
                            c.ptr.pp_double[i][n] = (double)(0);
                            for(j=0; j<=n-1; j++)
                            {
                                c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                                c.ptr.pp_double[i][n] = c.ptr.pp_double[i][n]+c.ptr.pp_double[i][j]*xc.ptr.p_double[j];
                            }
                            ct.ptr.p_int[i] = ae_randominteger(3, _state)-1;
                        }
                        
                        /*
                         * Create and optimize
                         */
                        minbleiccreate(n, &xs, &state, _state);
                        minbleicsetbc(&state, &bl, &bu, _state);
                        minbleicsetlc(&state, &c, &ct, k, _state);
                        minbleicsetcond(&state, weakepsg, 0.0, 0.0, 0, _state);
                        testminbleicunit_setrandompreconditioner(&state, n, preckind, _state);
                        while(minbleiciteration(&state, _state))
                        {
                            if( state.needfg )
                            {
                                state.f = (double)(0);
                                for(i=0; i<=n-1; i++)
                                {
                                    state.f = state.f+ae_pow(state.x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p), _state);
                                    state.g.ptr.p_double[i] = p*ae_pow(state.x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p-1), _state);
                                }
                                continue;
                            }
                            
                            /*
                             * Unknown protocol specified
                             */
                            *interr = ae_true;
                            ae_frame_leave(_state);
                            return;
                        }
                        minbleicresults(&state, &x, &rep, _state);
                        if( rep.terminationtype<=0 )
                        {
                            *converr = ae_true;
                            ae_frame_leave(_state);
                            return;
                        }
                        
                        /*
                         * Check feasibility
                         */
                        for(i=0; i<=n-1; i++)
                        {
                            *feaserr = *feaserr||ae_fp_less(x.ptr.p_double[i],0.0);
                            *feaserr = *feaserr||ae_fp_greater(x.ptr.p_double[i],1.0);
                        }
                        for(i=0; i<=k-1; i++)
                        {
                            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                            v = v-c.ptr.pp_double[i][n];
                            if( ct.ptr.p_int[i]==0 )
                            {
                                *feaserr = *feaserr||ae_fp_greater(ae_fabs(v, _state),epsfeas);
                            }
                            if( ct.ptr.p_int[i]<0 )
                            {
                                *feaserr = *feaserr||ae_fp_greater(v,epsfeas);
                            }
                            if( ct.ptr.p_int[i]>0 )
                            {
                                *feaserr = *feaserr||ae_fp_less(v,-epsfeas);
                            }
                        }
                    }
                }
            }
        }
        
        /*
         * Infeasible problem:
         * * all bound constraints are 0 <= x[i] <= 1 except for one
         * * that one is 0 >= x[i] >= 1
         * * no linear constraints
         * * preconditioner is chosen at random (we just want to be
         *   sure that preconditioning won't prevent us from detecting
         *   infeasible point):
         *   * unit preconditioner
         *   * random diagonal-based preconditioner
         *   * random scale-based preconditioner
         * * F(x) = |x-x0|^P, where P={2,4} and x0 is randomly selected from [-1,+2]^N
         * * algorithm must return correct error code on such problem
         */
        for(preckind=0; preckind<=2; preckind++)
        {
            for(pkind=1; pkind<=2; pkind++)
            {
                for(n=1; n<=nmax; n++)
                {
                    
                    /*
                     * Generate X, BL, BU.
                     */
                    p = 2*pkind;
                    ae_vector_set_length(&bl, n, _state);
                    ae_vector_set_length(&bu, n, _state);
                    ae_vector_set_length(&x, n, _state);
                    ae_vector_set_length(&x0, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        bl.ptr.p_double[i] = (double)(0);
                        bu.ptr.p_double[i] = (double)(1);
                        x.ptr.p_double[i] = ae_randomreal(_state);
                        x0.ptr.p_double[i] = 3*ae_randomreal(_state)-1;
                    }
                    i = ae_randominteger(n, _state);
                    bl.ptr.p_double[i] = (double)(1);
                    bu.ptr.p_double[i] = (double)(0);
                    
                    /*
                     * Create and optimize
                     */
                    minbleiccreate(n, &x, &state, _state);
                    minbleicsetbc(&state, &bl, &bu, _state);
                    minbleicsetcond(&state, weakepsg, 0.0, 0.0, 0, _state);
                    testminbleicunit_setrandompreconditioner(&state, n, preckind, _state);
                    while(minbleiciteration(&state, _state))
                    {
                        if( state.needfg )
                        {
                            state.f = (double)(0);
                            for(i=0; i<=n-1; i++)
                            {
                                state.f = state.f+ae_pow(state.x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p), _state);
                                state.g.ptr.p_double[i] = p*ae_pow(state.x.ptr.p_double[i]-x0.ptr.p_double[i], (double)(p-1), _state);
                            }
                            continue;
                        }
                        
                        /*
                         * Unknown protocol specified
                         */
                        *interr = ae_true;
                        ae_frame_leave(_state);
                        return;
                    }
                    minbleicresults(&state, &x, &rep, _state);
                    *feaserr = *feaserr||rep.terminationtype!=-3;
                }
            }
        }
        
        /*
         * Infeasible problem (2):
         * * no bound and inequality constraints
         * * 1<=K<=N arbitrary equality constraints
         * * (K+1)th constraint which is equal to the first constraint a*x=c,
         *   but with c:=c+1. I.e. we have both a*x=c and a*x=c+1, which can't
         *   be true (other constraints may be inconsistent too, but we don't
         *   have to check it).
         * * preconditioner is chosen at random (we just want to be
         *   sure that preconditioning won't prevent us from detecting
         *   infeasible point):
         *   * unit preconditioner
         *   * random diagonal-based preconditioner
         *   * random scale-based preconditioner
         * * F(x) = |x|^P, where P={2,4}
         * * algorithm must return correct error code on such problem
         */
        for(preckind=0; preckind<=2; preckind++)
        {
            for(pkind=1; pkind<=2; pkind++)
            {
                for(n=1; n<=nmax; n++)
                {
                    for(k=1; k<=n; k++)
                    {
                        
                        /*
                         * Generate X, BL, BU.
                         */
                        p = 2*pkind;
                        ae_vector_set_length(&x, n, _state);
                        ae_matrix_set_length(&c, k+1, n+1, _state);
                        ae_vector_set_length(&ct, k+1, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            x.ptr.p_double[i] = ae_randomreal(_state);
                        }
                        for(i=0; i<=k-1; i++)
                        {
                            for(j=0; j<=n; j++)
                            {
                                c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                            }
                            ct.ptr.p_int[i] = 0;
                        }
                        ct.ptr.p_int[k] = 0;
                        ae_v_move(&c.ptr.pp_double[k][0], 1, &c.ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
                        c.ptr.pp_double[k][n] = c.ptr.pp_double[0][n]+1;
                        
                        /*
                         * Create and optimize
                         */
                        minbleiccreate(n, &x, &state, _state);
                        minbleicsetlc(&state, &c, &ct, k+1, _state);
                        minbleicsetcond(&state, weakepsg, 0.0, 0.0, 0, _state);
                        testminbleicunit_setrandompreconditioner(&state, n, preckind, _state);
                        while(minbleiciteration(&state, _state))
                        {
                            if( state.needfg )
                            {
                                state.f = (double)(0);
                                for(i=0; i<=n-1; i++)
                                {
                                    state.f = state.f+ae_pow(state.x.ptr.p_double[i], (double)(p), _state);
                                    state.g.ptr.p_double[i] = p*ae_pow(state.x.ptr.p_double[i], (double)(p-1), _state);
                                }
                                continue;
                            }
                            
                            /*
                             * Unknown protocol specified
                             */
                            *interr = ae_true;
                            ae_frame_leave(_state);
                            return;
                        }
                        minbleicresults(&state, &x, &rep, _state);
                        *feaserr = *feaserr||rep.terminationtype!=-3;
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function additional properties.

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testminbleicunit_testother(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t passcount;
    ae_int_t pass;
    ae_int_t n;
    ae_int_t nmax;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_vector bl;
    ae_vector bu;
    ae_vector x;
    ae_vector xf;
    ae_vector x0;
    ae_vector x1;
    ae_vector b;
    ae_vector xlast;
    ae_vector a;
    ae_vector s;
    ae_vector h;
    ae_matrix c;
    ae_matrix fulla;
    ae_vector ct;
    double fprev;
    double xprev;
    double stpmax;
    double v;
    ae_int_t pkind;
    ae_int_t ckind;
    ae_int_t mkind;
    double vc;
    double vm;
    minbleicstate state;
    double epsx;
    double epsg;
    double eps;
    double tmpeps;
    minbleicreport rep;
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
    ae_int_t stopcallidx;
    ae_int_t callidx;
    ae_int_t maxits;
    ae_bool terminationrequested;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&xf, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&xlast, 0, DT_REAL, _state);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&h, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&fulla, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    _minbleicstate_init(&state, _state);
    _minbleicreport_init(&rep, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    nmax = 5;
    epsx = 1.0E-4;
    epsg = 1.0E-8;
    passcount = 10;
    
    /*
     * Try to reproduce bug 570 (optimizer hangs on problems where it is required
     * to perform very small step - less than 1E-50 - in order to activate constraints).
     *
     * The problem being solved is:
     *
     *     min x[0]+x[1]+...+x[n-1]
     *
     * subject to
     *
     *     x[i]>=0, for i=0..n-1
     *
     * with initial point
     *
     *     x[0] = 1.0E-100, x[1]=x[2]=...=0.5
     *
     * We try to reproduce this problem in different settings:
     * * boundary-only constraints - we test that completion code is positive,
     *   and all x[] are EXACTLY zero
     * * boundary constraints posed as general linear ones - we test that
     *   completion code is positive, and all x[] are APPROXIMATELY zero.
     */
    n = 10;
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&bl, n, _state);
    ae_vector_set_length(&bu, n, _state);
    ae_matrix_set_length(&c, n, n+1, _state);
    ae_vector_set_length(&ct, n, _state);
    for(i=0; i<=n-1; i++)
    {
        x.ptr.p_double[i] = 0.5;
        bl.ptr.p_double[i] = 0.0;
        bu.ptr.p_double[i] = _state->v_posinf;
        ct.ptr.p_int[i] = 1;
        for(j=0; j<=n; j++)
        {
            c.ptr.pp_double[i][j] = 0.0;
        }
        c.ptr.pp_double[i][i] = 1.0;
    }
    x.ptr.p_double[0] = 1.0E-100;
    minbleiccreate(n, &x, &state, _state);
    minbleicsetbc(&state, &bl, &bu, _state);
    minbleicsetcond(&state, (double)(0), (double)(0), (double)(0), 2*n, _state);
    while(minbleiciteration(&state, _state))
    {
        if( state.needfg )
        {
            state.f = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                state.f = state.f+state.x.ptr.p_double[i];
                state.g.ptr.p_double[i] = 1.0;
            }
        }
    }
    minbleicresults(&state, &xf, &rep, _state);
    seterrorflag(err, rep.terminationtype<=0, _state);
    if( rep.terminationtype>0 )
    {
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(err, ae_fp_neq(xf.ptr.p_double[i],(double)(0)), _state);
        }
    }
    minbleiccreate(n, &x, &state, _state);
    minbleicsetlc(&state, &c, &ct, n, _state);
    minbleicsetcond(&state, 1.0E-64, (double)(0), (double)(0), 10, _state);
    while(minbleiciteration(&state, _state))
    {
        if( state.needfg )
        {
            state.f = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                state.f = state.f+state.x.ptr.p_double[i];
                state.g.ptr.p_double[i] = 1.0;
            }
        }
    }
    minbleicresults(&state, &xf, &rep, _state);
    seterrorflag(err, rep.terminationtype<=0, _state);
    if( rep.terminationtype>0 )
    {
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(err, ae_fp_greater(ae_fabs(xf.ptr.p_double[i], _state),1.0E-10), _state);
        }
    }
    
    /*
     * Test reports:
     * * first value must be starting point
     * * last value must be last point
     */
    for(pass=1; pass<=passcount; pass++)
    {
        n = 50;
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&xlast, n, _state);
        ae_vector_set_length(&bl, n, _state);
        ae_vector_set_length(&bu, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = (double)(10);
            bl.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            bu.ptr.p_double[i] = _state->v_posinf;
        }
        minbleiccreate(n, &x, &state, _state);
        minbleicsetbc(&state, &bl, &bu, _state);
        minbleicsetcond(&state, 1.0E-64, (double)(0), (double)(0), 10, _state);
        minbleicsetxrep(&state, ae_true, _state);
        fprev = ae_maxrealnumber;
        while(minbleiciteration(&state, _state))
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
        minbleicresults(&state, &x, &rep, _state);
        for(i=0; i<=n-1; i++)
        {
            *err = *err||ae_fp_neq(x.ptr.p_double[i],xlast.ptr.p_double[i]);
        }
    }
    
    /*
     * Test differentiation vs. analytic gradient
     * (first one issues NeedF requests, second one issues NeedFG requests)
     */
    for(pass=1; pass<=passcount; pass++)
    {
        n = 10;
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
                minbleiccreate(n, &x, &state, _state);
            }
            if( dkind==1 )
            {
                minbleiccreatef(n, &x, diffstep, &state, _state);
            }
            minbleicsetcond(&state, 1.0E-6, (double)(0), epsx, 0, _state);
            wasf = ae_false;
            wasfg = ae_false;
            while(minbleiciteration(&state, _state))
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
            minbleicresults(&state, &x, &rep, _state);
            if( dkind==0 )
            {
                *err = (*err||wasf)||!wasfg;
            }
            if( dkind==1 )
            {
                *err = (*err||!wasf)||wasfg;
            }
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
    for(pass=1; pass<=passcount; pass++)
    {
        ae_vector_set_length(&x, 1, _state);
        ae_vector_set_length(&s, 1, _state);
        diffstep = ae_randomreal(_state)*1.0E-6;
        s.ptr.p_double[0] = ae_exp(ae_randomreal(_state)*4-2, _state);
        x.ptr.p_double[0] = (double)(0);
        minbleiccreatef(1, &x, diffstep, &state, _state);
        minbleicsetcond(&state, 1.0E-6, (double)(0), epsx, 0, _state);
        minbleicsetscale(&state, &s, _state);
        v = (double)(0);
        while(minbleiciteration(&state, _state))
        {
            state.f = ae_sqr(state.x.ptr.p_double[0], _state);
            v = ae_maxreal(v, ae_fabs(state.x.ptr.p_double[0], _state), _state);
        }
        minbleicresults(&state, &x, &rep, _state);
        r = v/(s.ptr.p_double[0]*diffstep);
        *err = *err||ae_fp_greater(ae_fabs(ae_log(r, _state), _state),ae_log(1+1000*ae_machineepsilon, _state));
    }
    
    /*
     * Test stpmax
     */
    for(pass=1; pass<=passcount; pass++)
    {
        n = 1;
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&bl, n, _state);
        ae_vector_set_length(&bu, n, _state);
        x.ptr.p_double[0] = (double)(100);
        bl.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
        bu.ptr.p_double[0] = _state->v_posinf;
        stpmax = 0.05+0.05*ae_randomreal(_state);
        minbleiccreate(n, &x, &state, _state);
        minbleicsetbc(&state, &bl, &bu, _state);
        minbleicsetcond(&state, epsg, (double)(0), epsx, 0, _state);
        minbleicsetxrep(&state, ae_true, _state);
        minbleicsetstpmax(&state, stpmax, _state);
        xprev = x.ptr.p_double[0];
        while(minbleiciteration(&state, _state))
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
    }
    
    /*
     * Ability to solve problems with function which is unbounded from below
     */
    for(pass=1; pass<=passcount; pass++)
    {
        n = 1;
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&bl, n, _state);
        ae_vector_set_length(&bu, n, _state);
        bl.ptr.p_double[0] = 4*ae_randomreal(_state)+1;
        bu.ptr.p_double[0] = bl.ptr.p_double[0]+1;
        x.ptr.p_double[0] = 0.5*(bl.ptr.p_double[0]+bu.ptr.p_double[0]);
        minbleiccreate(n, &x, &state, _state);
        minbleicsetbc(&state, &bl, &bu, _state);
        minbleicsetcond(&state, epsg, (double)(0), epsx, 0, _state);
        while(minbleiciteration(&state, _state))
        {
            if( state.needfg )
            {
                state.f = -1.0E8*ae_sqr(state.x.ptr.p_double[0], _state);
                state.g.ptr.p_double[0] = -2.0E8*state.x.ptr.p_double[0];
            }
        }
        minbleicresults(&state, &x, &rep, _state);
        *err = *err||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-bu.ptr.p_double[0], _state),epsx);
    }
    
    /*
     * Test correctness of the scaling:
     * * initial point is random point from [+1,+2]^N
     * * f(x) = SUM(A[i]*x[i]^4), C[i] is random from [0.01,100]
     * * function is EFFECTIVELY unconstrained; it has formal constraints,
     *   but they are inactive at the solution; we try different variants
     *   in order to explore different control paths of the optimizer:
     *   0) absense of constraints
     *   1) bound constraints -100000<=x[i]<=100000
     *   2) one linear constraint 0*x=0
     *   3) combination of (1) and (2)
     * * we use random scaling matrix
     * * we test different variants of the preconditioning:
     *   0) unit preconditioner
     *   1) random diagonal from [0.01,100]
     *   2) scale preconditioner
     * * we set very stringent stopping conditions
     * * and we test that in the extremum stopping conditions are
     *   satisfied subject to the current scaling coefficients.
     */
    for(pass=1; pass<=passcount; pass++)
    {
        tmpeps = 1.0E-5;
        for(n=1; n<=10; n++)
        {
            for(ckind=0; ckind<=3; ckind++)
            {
                for(pkind=0; pkind<=2; pkind++)
                {
                    ae_vector_set_length(&x, n, _state);
                    ae_vector_set_length(&a, n, _state);
                    ae_vector_set_length(&s, n, _state);
                    ae_vector_set_length(&h, n, _state);
                    ae_vector_set_length(&bl, n, _state);
                    ae_vector_set_length(&bu, n, _state);
                    ae_matrix_set_length(&c, 1, n+1, _state);
                    ae_vector_set_length(&ct, 1, _state);
                    ct.ptr.p_int[0] = 0;
                    c.ptr.pp_double[0][n] = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        x.ptr.p_double[i] = ae_randomreal(_state)+1;
                        bl.ptr.p_double[i] = (double)(-100000);
                        bu.ptr.p_double[i] = (double)(100000);
                        c.ptr.pp_double[0][i] = (double)(0);
                        a.ptr.p_double[i] = ae_exp(ae_log((double)(10), _state)*(2*ae_randomreal(_state)-1), _state);
                        s.ptr.p_double[i] = ae_exp(ae_log((double)(10), _state)*(2*ae_randomreal(_state)-1), _state);
                        h.ptr.p_double[i] = ae_exp(ae_log((double)(10), _state)*(2*ae_randomreal(_state)-1), _state);
                    }
                    minbleiccreate(n, &x, &state, _state);
                    if( ckind==1||ckind==3 )
                    {
                        minbleicsetbc(&state, &bl, &bu, _state);
                    }
                    if( ckind==2||ckind==3 )
                    {
                        minbleicsetlc(&state, &c, &ct, 1, _state);
                    }
                    if( pkind==1 )
                    {
                        minbleicsetprecdiag(&state, &h, _state);
                    }
                    if( pkind==2 )
                    {
                        minbleicsetprecscale(&state, _state);
                    }
                    minbleicsetcond(&state, tmpeps, (double)(0), (double)(0), 0, _state);
                    minbleicsetscale(&state, &s, _state);
                    while(minbleiciteration(&state, _state))
                    {
                        if( state.needfg )
                        {
                            state.f = (double)(0);
                            for(i=0; i<=n-1; i++)
                            {
                                state.f = state.f+a.ptr.p_double[i]*ae_sqr(state.x.ptr.p_double[i], _state);
                                state.g.ptr.p_double[i] = 2*a.ptr.p_double[i]*state.x.ptr.p_double[i];
                            }
                        }
                    }
                    minbleicresults(&state, &x, &rep, _state);
                    if( rep.terminationtype<=0 )
                    {
                        *err = ae_true;
                        ae_frame_leave(_state);
                        return;
                    }
                    v = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        v = v+ae_sqr(s.ptr.p_double[i]*2*a.ptr.p_double[i]*x.ptr.p_double[i], _state);
                    }
                    v = ae_sqrt(v, _state);
                    seterrorflag(err, ae_fp_greater(v,tmpeps), _state);
                }
            }
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
     * where c is either 1.0 or 1.0E+4, M is either 1.0E8, 1.0E20 or +INF
     * (we try different combinations)
     */
    for(pass=1; pass<=passcount; pass++)
    {
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
                    vc = 1.0E+4;
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
                minbleiccreate(1, &x, &state, _state);
                minbleicsetcond(&state, epsg, (double)(0), (double)(0), 0, _state);
                while(minbleiciteration(&state, _state))
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
                            state.g.ptr.p_double[0] = (double)(0);
                        }
                    }
                }
                minbleicresults(&state, &x, &rep, _state);
                if( rep.terminationtype<=0 )
                {
                    *err = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                *err = *err||ae_fp_greater(ae_fabs(1/ae_sqr(1-x.ptr.p_double[0], _state)-1/ae_sqr(1+x.ptr.p_double[0], _state)+vc, _state),epsg);
            }
        }
    }
    
    /*
     * Test behaviour on noisy functions.
     *
     * Consider following problem:
     * * f(x,y) = (x+1)^2 + (y+1)^2 + 10000*MachineEpsilon*RandomReal()
     * * boundary constraints x>=0, y>=0
     * * starting point (x0,y0)=(10*MachineEpsilon,1.0)
     *
     * Such problem contains small numerical noise. Without noise its
     * solution is (xs,ys)=(0,0), which is easy to find. However, presence
     * of the noise makes it hard to solve:
     * * noisy f(x,y) is monotonically decreasing only when we perform
     *   steps orders of magnitude larger than 10000*MachineEpsilon
     * * at small scales f(x,y) is non-monotonic and non-convex
     * * however, our first step must be done towards
     *   (x1,y1) = (0,1-some_small_value), and length of such step is
     *   many times SMALLER than 10000*MachineEpsilon
     * * second step, from (x1,y1) to (xs,ys), will be large enough to
     *   ignore numerical noise, so the only problem is to perform
     *   first step
     *
     * Naive implementation of BLEIC should fail sometimes (sometimes -
     * due to non-deterministic nature of noise) on such problem. However,
     * our improved implementation should solve it correctly. We test
     * several variations of inner stopping criteria.
     */
    for(pass=1; pass<=passcount; pass++)
    {
        eps = 1.0E-9;
        ae_vector_set_length(&x, 2, _state);
        ae_vector_set_length(&bl, 2, _state);
        ae_vector_set_length(&bu, 2, _state);
        x.ptr.p_double[0] = 10*ae_machineepsilon;
        x.ptr.p_double[1] = 1.0;
        bl.ptr.p_double[0] = 0.0;
        bu.ptr.p_double[0] = _state->v_posinf;
        bl.ptr.p_double[1] = 0.0;
        bu.ptr.p_double[1] = _state->v_posinf;
        for(ckind=0; ckind<=2; ckind++)
        {
            minbleiccreate(2, &x, &state, _state);
            minbleicsetbc(&state, &bl, &bu, _state);
            if( ckind==0 )
            {
                minbleicsetcond(&state, eps, (double)(0), (double)(0), 0, _state);
            }
            if( ckind==1 )
            {
                minbleicsetcond(&state, (double)(0), eps, (double)(0), 0, _state);
            }
            if( ckind==2 )
            {
                minbleicsetcond(&state, (double)(0), (double)(0), eps, 0, _state);
            }
            while(minbleiciteration(&state, _state))
            {
                if( state.needfg )
                {
                    state.f = ae_sqr(state.x.ptr.p_double[0]+1, _state)+ae_sqr(state.x.ptr.p_double[1]+1, _state)+10000*ae_machineepsilon*ae_randomreal(_state);
                    state.g.ptr.p_double[0] = 2*(state.x.ptr.p_double[0]+1);
                    state.g.ptr.p_double[1] = 2*(state.x.ptr.p_double[1]+1);
                }
            }
            minbleicresults(&state, &xf, &rep, _state);
            if( (rep.terminationtype<=0||ae_fp_neq(xf.ptr.p_double[0],(double)(0)))||ae_fp_neq(xf.ptr.p_double[1],(double)(0)) )
            {
                *err = ae_true;
                ae_frame_leave(_state);
                return;
            }
        }
    }
    
    /*
     * Deterministic variation of the previous problem.
     *
     * Consider following problem:
     * * boundary constraints x>=0, y>=0
     * * starting point (x0,y0)=(10*MachineEpsilon,1.0)
     *            / (x+1)^2 + (y+1)^2,        for (x,y)<>(x0,y0)
     * * f(x,y) = |
     *            \ (x+1)^2 + (y+1)^2 - 0.1,  for (x,y)=(x0,y0)
     *
     * Such problem contains deterministic numerical noise (-0.1 at
     * starting point). Without noise its solution is easy to find.
     * However, presence of the noise makes it hard to solve:
     * * our first step must be done towards (x1,y1) = (0,1-some_small_value),
     *   but such step will increase function valye by approximately 0.1  -
     *   instead of decreasing it.
     *
     * Naive implementation of BLEIC should fail on such problem. However,
     * our improved implementation should solve it correctly. We test
     * several variations of inner stopping criteria.
     */
    for(pass=1; pass<=passcount; pass++)
    {
        eps = 1.0E-9;
        ae_vector_set_length(&x, 2, _state);
        ae_vector_set_length(&bl, 2, _state);
        ae_vector_set_length(&bu, 2, _state);
        x.ptr.p_double[0] = 10*ae_machineepsilon;
        x.ptr.p_double[1] = 1.0;
        bl.ptr.p_double[0] = 0.0;
        bu.ptr.p_double[0] = _state->v_posinf;
        bl.ptr.p_double[1] = 0.0;
        bu.ptr.p_double[1] = _state->v_posinf;
        for(ckind=0; ckind<=2; ckind++)
        {
            minbleiccreate(2, &x, &state, _state);
            minbleicsetbc(&state, &bl, &bu, _state);
            if( ckind==0 )
            {
                minbleicsetcond(&state, eps, (double)(0), (double)(0), 0, _state);
            }
            if( ckind==1 )
            {
                minbleicsetcond(&state, (double)(0), eps, (double)(0), 0, _state);
            }
            if( ckind==2 )
            {
                minbleicsetcond(&state, (double)(0), (double)(0), eps, 0, _state);
            }
            while(minbleiciteration(&state, _state))
            {
                if( state.needfg )
                {
                    state.f = ae_sqr(state.x.ptr.p_double[0]+1, _state)+ae_sqr(state.x.ptr.p_double[1]+1, _state);
                    if( ae_fp_eq(state.x.ptr.p_double[0],x.ptr.p_double[0])&&ae_fp_eq(state.x.ptr.p_double[1],x.ptr.p_double[1]) )
                    {
                        state.f = state.f-0.1;
                    }
                    state.g.ptr.p_double[0] = 2*(state.x.ptr.p_double[0]+1);
                    state.g.ptr.p_double[1] = 2*(state.x.ptr.p_double[1]+1);
                }
            }
            minbleicresults(&state, &xf, &rep, _state);
            if( (rep.terminationtype<=0||ae_fp_neq(xf.ptr.p_double[0],(double)(0)))||ae_fp_neq(xf.ptr.p_double[1],(double)(0)) )
            {
                *err = ae_true;
                ae_frame_leave(_state);
                return;
            }
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
        minbleiccreate(n, &x0, &state, _state);
        minbleicsetcond(&state, 0.0, 0.0, 0.0, stopiteration, _state);
        minbleicsetxrep(&state, ae_true, _state);
        k = -1;
        while(minbleiciteration(&state, _state))
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
        minbleicresults(&state, &x1, &rep, _state);
        seterrorflag(err, rep.terminationtype!=-8, _state);
    }
    
    /*
     * Check algorithm ability to handle request for termination:
     * * to terminate with correct return code = 8
     * * to return point which was "current" at the moment of termination
     *
     * NOTE: we solve problem with "corrupted" preconditioner which makes it hard
     *       to converge in less than StopCallIdx iterations
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
        ae_vector_set_length(&s, 3, _state);
        s.ptr.p_double[0] = 0.00001;
        s.ptr.p_double[1] = 0.00001;
        s.ptr.p_double[2] = 10000.0;
        stopcallidx = ae_randominteger(20, _state);
        maxits = 25;
        minbleiccreate(n, &x, &state, _state);
        minbleicsetcond(&state, (double)(0), (double)(0), (double)(0), maxits, _state);
        minbleicsetxrep(&state, ae_true, _state);
        minbleicsetprecdiag(&state, &s, _state);
        callidx = 0;
        terminationrequested = ae_false;
        ae_v_move(&xlast.ptr.p_double[0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
        while(minbleiciteration(&state, _state))
        {
            if( state.needfg )
            {
                state.f = ss*ae_sqr(ae_exp(state.x.ptr.p_double[0], _state)-2, _state)+ae_sqr(state.x.ptr.p_double[1], _state)+ae_sqr(state.x.ptr.p_double[2]-state.x.ptr.p_double[0], _state);
                state.g.ptr.p_double[0] = 2*ss*(ae_exp(state.x.ptr.p_double[0], _state)-2)*ae_exp(state.x.ptr.p_double[0], _state)+2*(state.x.ptr.p_double[2]-state.x.ptr.p_double[0])*(-1);
                state.g.ptr.p_double[1] = 2*state.x.ptr.p_double[1];
                state.g.ptr.p_double[2] = 2*(state.x.ptr.p_double[2]-state.x.ptr.p_double[0]);
                if( callidx==stopcallidx )
                {
                    minbleicrequesttermination(&state, _state);
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
        minbleicresults(&state, &x, &rep, _state);
        seterrorflag(err, rep.terminationtype!=8, _state);
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(err, ae_fp_neq(x.ptr.p_double[i],xlast.ptr.p_double[i]), _state);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests convergence properties.
We solve several simple problems with different combinations of constraints

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testminbleicunit_testconv(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t passcount;
    ae_int_t pass;
    ae_vector bl;
    ae_vector bu;
    ae_vector x;
    ae_vector b;
    ae_vector tmp;
    ae_vector g;
    ae_vector xf;
    ae_vector xs0;
    ae_vector xs1;
    ae_matrix a;
    ae_matrix c;
    ae_matrix ce;
    ae_vector ct;
    ae_vector nonnegative;
    minbleicstate state;
    double epsg;
    double epsfeas;
    double tol;
    minbleicreport rep;
    snnlssolver nnls;
    ae_int_t m;
    ae_int_t n;
    ae_int_t k;
    ae_int_t i;
    ae_int_t j;
    double v;
    double vv;
    ae_int_t preckind;
    ae_int_t akind;
    ae_int_t shiftkind;
    ae_int_t bscale;
    double tolconstr;
    double f0;
    double f1;
    ae_int_t ccnt;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&xf, 0, DT_REAL, _state);
    ae_vector_init(&xs0, 0, DT_REAL, _state);
    ae_vector_init(&xs1, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ce, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    ae_vector_init(&nonnegative, 0, DT_BOOL, _state);
    _minbleicstate_init(&state, _state);
    _minbleicreport_init(&rep, _state);
    _snnlssolver_init(&nnls, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    epsg = 1.0E-8;
    epsfeas = 1.0E-8;
    tol = 0.001;
    passcount = 10;
    
    /*
     * Three closely connected problems:
     * * 2-dimensional space
     * * octagonal area bounded by:
     *   * -1<=x<=+1
     *   * -1<=y<=+1
     *   * x+y<=1.5
     *   * x-y<=1.5
     *   * -x+y<=1.5
     *   * -x-y<=1.5
     * * several target functions:
     *   * f0=x+0.001*y, minimum at x=-1, y=-0.5
     *   * f1=(x+10)^2+y^2, minimum at x=-1, y=0
     *   * f2=(x+10)^2+(y-0.6)^2, minimum at x=-1, y=0.5
     */
    ae_vector_set_length(&x, 2, _state);
    ae_vector_set_length(&bl, 2, _state);
    ae_vector_set_length(&bu, 2, _state);
    ae_matrix_set_length(&c, 4, 3, _state);
    ae_vector_set_length(&ct, 4, _state);
    bl.ptr.p_double[0] = (double)(-1);
    bl.ptr.p_double[1] = (double)(-1);
    bu.ptr.p_double[0] = (double)(1);
    bu.ptr.p_double[1] = (double)(1);
    c.ptr.pp_double[0][0] = (double)(1);
    c.ptr.pp_double[0][1] = (double)(1);
    c.ptr.pp_double[0][2] = 1.5;
    ct.ptr.p_int[0] = -1;
    c.ptr.pp_double[1][0] = (double)(1);
    c.ptr.pp_double[1][1] = (double)(-1);
    c.ptr.pp_double[1][2] = 1.5;
    ct.ptr.p_int[1] = -1;
    c.ptr.pp_double[2][0] = (double)(-1);
    c.ptr.pp_double[2][1] = (double)(1);
    c.ptr.pp_double[2][2] = 1.5;
    ct.ptr.p_int[2] = -1;
    c.ptr.pp_double[3][0] = (double)(-1);
    c.ptr.pp_double[3][1] = (double)(-1);
    c.ptr.pp_double[3][2] = 1.5;
    ct.ptr.p_int[3] = -1;
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * f0
         */
        x.ptr.p_double[0] = 0.2*ae_randomreal(_state)-0.1;
        x.ptr.p_double[1] = 0.2*ae_randomreal(_state)-0.1;
        minbleiccreate(2, &x, &state, _state);
        minbleicsetbc(&state, &bl, &bu, _state);
        minbleicsetlc(&state, &c, &ct, 4, _state);
        minbleicsetcond(&state, epsg, 0.0, 0.0, 0, _state);
        while(minbleiciteration(&state, _state))
        {
            if( state.needfg )
            {
                state.f = state.x.ptr.p_double[0]+0.001*state.x.ptr.p_double[1];
                state.g.ptr.p_double[0] = (double)(1);
                state.g.ptr.p_double[1] = 0.001;
            }
        }
        minbleicresults(&state, &x, &rep, _state);
        if( rep.terminationtype>0 )
        {
            *err = *err||ae_fp_greater(ae_fabs(x.ptr.p_double[0]+1, _state),tol);
            *err = *err||ae_fp_greater(ae_fabs(x.ptr.p_double[1]+0.5, _state),tol);
        }
        else
        {
            *err = ae_true;
        }
        
        /*
         * f1
         */
        x.ptr.p_double[0] = 0.2*ae_randomreal(_state)-0.1;
        x.ptr.p_double[1] = 0.2*ae_randomreal(_state)-0.1;
        minbleiccreate(2, &x, &state, _state);
        minbleicsetbc(&state, &bl, &bu, _state);
        minbleicsetlc(&state, &c, &ct, 4, _state);
        minbleicsetcond(&state, epsg, 0.0, 0.0, 0, _state);
        while(minbleiciteration(&state, _state))
        {
            if( state.needfg )
            {
                state.f = ae_sqr(state.x.ptr.p_double[0]+10, _state)+ae_sqr(state.x.ptr.p_double[1], _state);
                state.g.ptr.p_double[0] = 2*(state.x.ptr.p_double[0]+10);
                state.g.ptr.p_double[1] = 2*state.x.ptr.p_double[1];
            }
        }
        minbleicresults(&state, &x, &rep, _state);
        if( rep.terminationtype>0 )
        {
            *err = *err||ae_fp_greater(ae_fabs(x.ptr.p_double[0]+1, _state),tol);
            *err = *err||ae_fp_greater(ae_fabs(x.ptr.p_double[1], _state),tol);
        }
        else
        {
            *err = ae_true;
        }
        
        /*
         * f2
         */
        x.ptr.p_double[0] = 0.2*ae_randomreal(_state)-0.1;
        x.ptr.p_double[1] = 0.2*ae_randomreal(_state)-0.1;
        minbleiccreate(2, &x, &state, _state);
        minbleicsetbc(&state, &bl, &bu, _state);
        minbleicsetlc(&state, &c, &ct, 4, _state);
        minbleicsetcond(&state, epsg, 0.0, 0.0, 0, _state);
        while(minbleiciteration(&state, _state))
        {
            if( state.needfg )
            {
                state.f = ae_sqr(state.x.ptr.p_double[0]+10, _state)+ae_sqr(state.x.ptr.p_double[1]-0.6, _state);
                state.g.ptr.p_double[0] = 2*(state.x.ptr.p_double[0]+10);
                state.g.ptr.p_double[1] = 2*(state.x.ptr.p_double[1]-0.6);
            }
        }
        minbleicresults(&state, &x, &rep, _state);
        if( rep.terminationtype>0 )
        {
            *err = *err||ae_fp_greater(ae_fabs(x.ptr.p_double[0]+1, _state),tol);
            *err = *err||ae_fp_greater(ae_fabs(x.ptr.p_double[1]-0.5, _state),tol);
        }
        else
        {
            *err = ae_true;
        }
    }
    
    /*
     * Degenerate optimization problem with excessive constraints.
     *
     * * N=3..10, M=N div 3, K = 2*N
     * * f(x) = 0.5*|A*x-b|^2, where A is MxN random matrix, b is Mx1 random vector
     * * bound constraint:
     *   a) Ci=x[i]=Ci  for i=0..M-1
     *   b) 0<=x[i]<=1  for i=M..N-1
     * * linear constraints (for fixed feasible xf and random ai):
     *   a) ai*x  = ai*xf                   for i=0..M-1
     *   b) ai*x <= ai*xf+random(0.1,1.0)   for i=M..K-1
     * * preconditioner is chosen at random (we just want to be
     *   sure that preconditioning won't prevent us from detecting
     *   infeasible point):
     *   a) unit preconditioner
     *   b) random diagonal-based preconditioner
     *   c) random scale-based preconditioner
     * * we choose two random initial points from interior of the area
     *   given by bound constraints.
     *
     * We do not know analytic solution of this problem, and we do not need
     * to solve it :) we just perform two restarts from two different initial
     * points and check that both solutions give approximately same function
     * value.
     */
    for(preckind=0; preckind<=2; preckind++)
    {
        for(n=3; n<=10; n++)
        {
            
            /*
             * Generate problem
             */
            m = n/3;
            k = 2*n;
            ae_vector_set_length(&bl, n, _state);
            ae_vector_set_length(&bu, n, _state);
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&xs0, n, _state);
            ae_vector_set_length(&xs1, n, _state);
            ae_vector_set_length(&xf, n, _state);
            for(i=0; i<=n-1; i++)
            {
                if( i<m )
                {
                    v = ae_randomreal(_state);
                    bl.ptr.p_double[i] = v;
                    bu.ptr.p_double[i] = v;
                    xf.ptr.p_double[i] = v;
                    xs0.ptr.p_double[i] = v;
                    xs1.ptr.p_double[i] = v;
                }
                else
                {
                    bl.ptr.p_double[i] = (double)(0);
                    bu.ptr.p_double[i] = (double)(1);
                    xf.ptr.p_double[i] = ae_randomreal(_state);
                    xs0.ptr.p_double[i] = ae_randomreal(_state);
                    xs1.ptr.p_double[i] = ae_randomreal(_state);
                }
                x.ptr.p_double[i] = ae_randomreal(_state);
            }
            ae_matrix_set_length(&c, k, n+1, _state);
            ae_vector_set_length(&ct, k, _state);
            for(i=0; i<=k-1; i++)
            {
                v = (double)(0);
                for(j=0; j<=n-1; j++)
                {
                    c.ptr.pp_double[i][j] = randomnormal(_state);
                    v = v+ae_sqr(c.ptr.pp_double[i][j], _state);
                }
                if( ae_fp_greater(v,(double)(0)) )
                {
                    for(j=0; j<=n-1; j++)
                    {
                        c.ptr.pp_double[i][j] = c.ptr.pp_double[i][j]/ae_sqrt(v, _state);
                    }
                }
                v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &xf.ptr.p_double[0], 1, ae_v_len(0,n-1));
                c.ptr.pp_double[i][n] = v;
                if( i<m )
                {
                    ct.ptr.p_int[i] = 0;
                }
                else
                {
                    ct.ptr.p_int[i] = -1;
                    c.ptr.pp_double[i][n] = c.ptr.pp_double[i][n]+0.1+0.9*ae_randomreal(_state);
                }
            }
            ae_matrix_set_length(&a, m, n+1, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n; j++)
                {
                    a.ptr.pp_double[i][j] = ae_randomreal(_state);
                }
            }
            
            /*
             * Create and optimize
             */
            minbleiccreate(n, &x, &state, _state);
            minbleicsetbc(&state, &bl, &bu, _state);
            minbleicsetlc(&state, &c, &ct, k, _state);
            minbleicsetcond(&state, epsg, 0.0, 0.0, 0, _state);
            testminbleicunit_setrandompreconditioner(&state, n, preckind, _state);
            
            /*
             * Solve problem 0:
             * * restart from XS0
             * * solve
             * * check convergence/feasibility
             * * calculate F0 - function value at solution
             */
            minbleicrestartfrom(&state, &xs0, _state);
            while(minbleiciteration(&state, _state))
            {
                state.f = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.g.ptr.p_double[i] = (double)(0);
                }
                for(i=0; i<=m-1; i++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    v = v-a.ptr.pp_double[i][n];
                    state.f = state.f+0.5*ae_sqr(v, _state);
                    ae_v_addd(&state.g.ptr.p_double[0], 1, &a.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
                }
            }
            minbleicresults(&state, &x, &rep, _state);
            if( rep.terminationtype<=0 )
            {
                *err = ae_true;
                ae_frame_leave(_state);
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( ae_fp_less(x.ptr.p_double[i],bl.ptr.p_double[i])||ae_fp_greater(x.ptr.p_double[i],bu.ptr.p_double[i]) )
                {
                    *err = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
            }
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                v = v-c.ptr.pp_double[i][n];
                if( ct.ptr.p_int[i]==0&&ae_fp_greater(ae_fabs(v, _state),epsfeas) )
                {
                    *err = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                if( ct.ptr.p_int[i]<0&&ae_fp_greater(v,epsfeas) )
                {
                    *err = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
            }
            f0 = (double)(0);
            for(i=0; i<=m-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                v = v-a.ptr.pp_double[i][n];
                f0 = f0+0.5*ae_sqr(v, _state);
            }
            
            /*
             * Solve problem 1:
             * * restart from XS1
             * * solve
             * * check convergence/feasibility
             * * calculate F1 - function value at solution
             */
            minbleicrestartfrom(&state, &xs1, _state);
            while(minbleiciteration(&state, _state))
            {
                state.f = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.g.ptr.p_double[i] = (double)(0);
                }
                for(i=0; i<=m-1; i++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    v = v-a.ptr.pp_double[i][n];
                    state.f = state.f+0.5*ae_sqr(v, _state);
                    ae_v_addd(&state.g.ptr.p_double[0], 1, &a.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
                }
            }
            minbleicresults(&state, &x, &rep, _state);
            if( rep.terminationtype<=0 )
            {
                *err = ae_true;
                ae_frame_leave(_state);
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( ae_fp_less(x.ptr.p_double[i],bl.ptr.p_double[i])||ae_fp_greater(x.ptr.p_double[i],bu.ptr.p_double[i]) )
                {
                    *err = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
            }
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                v = v-c.ptr.pp_double[i][n];
                if( ct.ptr.p_int[i]==0&&ae_fp_greater(ae_fabs(v, _state),epsfeas) )
                {
                    *err = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                if( ct.ptr.p_int[i]<0&&ae_fp_greater(v,epsfeas) )
                {
                    *err = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
            }
            f1 = (double)(0);
            for(i=0; i<=m-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                v = v-a.ptr.pp_double[i][n];
                f1 = f1+0.5*ae_sqr(v, _state);
            }
            
            /*
             * compare F0 and F1
             */
            seterrorflag(err, ae_fp_greater(ae_fabs(f0-f1, _state),1.0E-4), _state);
        }
    }
    
    /*
     * Convex/nonconvex optimization problem with excessive
     * (degenerate constraints):
     *
     * * N=2..8
     * * f = 0.5*x'*A*x+b'*x
     * * b has normally distributed entries with scale 10^BScale
     * * several kinds of A are tried: zero, well conditioned SPD, well conditioned indefinite, low rank
     * * box constraints: x[i] in [-1,+1]
     * * 2^N "excessive" general linear constraints (v_k,x)<=(v_k,v_k)+v_shift,
     *   where v_k is one of 2^N vertices of feasible hypercube, v_shift is
     *   a shift parameter:
     *   * with zero v_shift such constraints are degenerate (each vertex has
     *     N box constraints and one "redundant" linear constraint)
     *   * with positive v_shift linear constraint is always inactive
     *   * with small (about machine epsilon) but negative v_shift,
     *     constraint is close to degenerate - but not exactly
     *
     * We check that constrained gradient is close to zero at solution.
     * Box constraint is considered active if distance to boundary is less
     * than TolConstr.
     *
     * NOTE: TolConstr must be large enough so it won't conflict with
     *       perturbation introduced by v_shift
     */
    tolconstr = 1.0E-8;
    for(n=2; n<=8; n++)
    {
        for(akind=0; akind<=3; akind++)
        {
            for(shiftkind=-5; shiftkind<=1; shiftkind++)
            {
                for(bscale=0; bscale>=-2; bscale--)
                {
                    
                    /*
                     * Generate A, B and initial point
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    ae_vector_set_length(&b, n, _state);
                    ae_vector_set_length(&x, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        b.ptr.p_double[i] = ae_pow((double)(10), (double)(bscale), _state)*hqrndnormal(&rs, _state);
                        x.ptr.p_double[i] = hqrnduniformr(&rs, _state)-0.5;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 0.0;
                        }
                    }
                    if( akind==1 )
                    {
                        
                        /*
                         * Dense well conditioned SPD
                         */
                        spdmatrixrndcond(n, 50.0, &a, _state);
                    }
                    if( akind==2 )
                    {
                        
                        /*
                         * Dense well conditioned indefinite
                         */
                        smatrixrndcond(n, 50.0, &a, _state);
                    }
                    if( akind==3 )
                    {
                        
                        /*
                         * Low rank
                         */
                        ae_vector_set_length(&tmp, n, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                a.ptr.pp_double[i][j] = 0.0;
                            }
                        }
                        for(k=1; k<=ae_minint(3, n-1, _state); k++)
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                tmp.ptr.p_double[i] = hqrndnormal(&rs, _state);
                            }
                            v = hqrndnormal(&rs, _state);
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    a.ptr.pp_double[i][j] = a.ptr.pp_double[i][j]+v*tmp.ptr.p_double[i]*tmp.ptr.p_double[j];
                                }
                            }
                        }
                    }
                    
                    /*
                     * Generate constraints
                     */
                    ae_vector_set_length(&bl, n, _state);
                    ae_vector_set_length(&bu, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        bl.ptr.p_double[i] = -1.0;
                        bu.ptr.p_double[i] = 1.0;
                    }
                    ccnt = ae_round(ae_pow((double)(2), (double)(n), _state), _state);
                    ae_matrix_set_length(&c, ccnt, n+1, _state);
                    ae_vector_set_length(&ct, ccnt, _state);
                    for(i=0; i<=ccnt-1; i++)
                    {
                        ct.ptr.p_int[i] = -1;
                        k = i;
                        c.ptr.pp_double[i][n] = ae_sign((double)(shiftkind), _state)*ae_pow((double)(10), ae_fabs((double)(shiftkind), _state), _state)*ae_machineepsilon;
                        for(j=0; j<=n-1; j++)
                        {
                            c.ptr.pp_double[i][j] = (double)(2*(k%2)-1);
                            c.ptr.pp_double[i][n] = c.ptr.pp_double[i][n]+c.ptr.pp_double[i][j]*c.ptr.pp_double[i][j];
                            k = k/2;
                        }
                    }
                    
                    /*
                     * Create and optimize
                     */
                    minbleiccreate(n, &x, &state, _state);
                    minbleicsetbc(&state, &bl, &bu, _state);
                    minbleicsetlc(&state, &c, &ct, ccnt, _state);
                    minbleicsetcond(&state, 1.0E-9, 0.0, 0.0, 0, _state);
                    while(minbleiciteration(&state, _state))
                    {
                        ae_assert(state.needfg, "Assertion failed", _state);
                        state.f = (double)(0);
                        for(i=0; i<=n-1; i++)
                        {
                            state.f = state.f+state.x.ptr.p_double[i]*b.ptr.p_double[i];
                            state.g.ptr.p_double[i] = b.ptr.p_double[i];
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                            state.f = state.f+0.5*state.x.ptr.p_double[i]*v;
                            state.g.ptr.p_double[i] = state.g.ptr.p_double[i]+v;
                        }
                    }
                    minbleicresults(&state, &xs0, &rep, _state);
                    seterrorflag(err, rep.terminationtype<=0, _state);
                    if( *err )
                    {
                        ae_frame_leave(_state);
                        return;
                    }
                    
                    /*
                     * Evaluate gradient at solution and test
                     */
                    vv = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xs0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                        v = v+b.ptr.p_double[i];
                        if( ae_fp_less_eq(xs0.ptr.p_double[i],bl.ptr.p_double[i]+tolconstr)&&ae_fp_greater(v,(double)(0)) )
                        {
                            v = 0.0;
                        }
                        if( ae_fp_greater_eq(xs0.ptr.p_double[i],bu.ptr.p_double[i]-tolconstr)&&ae_fp_less(v,(double)(0)) )
                        {
                            v = 0.0;
                        }
                        vv = vv+ae_sqr(v, _state);
                    }
                    vv = ae_sqrt(vv, _state);
                    seterrorflag(err, ae_fp_greater(vv,1.0E-5), _state);
                }
            }
        }
    }
    
    /*
     * Convex/nonconvex optimization problem with combination of
     * box and linear constraints:
     *
     * * N=2..8
     * * f = 0.5*x'*A*x+b'*x
     * * b has normally distributed entries with scale 10^BScale
     * * several kinds of A are tried: zero, well conditioned SPD,
     *   well conditioned indefinite, low rank
     * * box constraints: x[i] in [-1,+1]
     * * initial point x0 = [0 0 ... 0 0]
     * * CCnt=min(3,N-1) general linear constraints of form (c,x)=0.
     *   random mix of equality/inequality constraints is tried.
     *   x0 is guaranteed to be feasible.
     *
     * We check that constrained gradient is close to zero at solution.
     * Inequality constraint is considered active if distance to boundary
     * is less than TolConstr. We use nonnegative least squares solver
     * in order to compute constrained gradient.
     */
    tolconstr = 1.0E-8;
    for(n=2; n<=8; n++)
    {
        for(akind=0; akind<=3; akind++)
        {
            for(bscale=0; bscale>=-2; bscale--)
            {
                
                /*
                 * Generate A, B and initial point
                 */
                ae_matrix_set_length(&a, n, n, _state);
                ae_vector_set_length(&b, n, _state);
                ae_vector_set_length(&x, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    b.ptr.p_double[i] = ae_pow((double)(10), (double)(bscale), _state)*hqrndnormal(&rs, _state);
                    x.ptr.p_double[i] = 0.0;
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a.ptr.pp_double[i][j] = 0.0;
                    }
                }
                if( akind==1 )
                {
                    
                    /*
                     * Dense well conditioned SPD
                     */
                    spdmatrixrndcond(n, 50.0, &a, _state);
                }
                if( akind==2 )
                {
                    
                    /*
                     * Dense well conditioned indefinite
                     */
                    smatrixrndcond(n, 50.0, &a, _state);
                }
                if( akind==3 )
                {
                    
                    /*
                     * Low rank
                     */
                    ae_vector_set_length(&tmp, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 0.0;
                        }
                    }
                    for(k=1; k<=ae_minint(3, n-1, _state); k++)
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            tmp.ptr.p_double[i] = hqrndnormal(&rs, _state);
                        }
                        v = hqrndnormal(&rs, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                a.ptr.pp_double[i][j] = a.ptr.pp_double[i][j]+v*tmp.ptr.p_double[i]*tmp.ptr.p_double[j];
                            }
                        }
                    }
                }
                
                /*
                 * Generate constraints
                 */
                ae_vector_set_length(&bl, n, _state);
                ae_vector_set_length(&bu, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    bl.ptr.p_double[i] = -1.0;
                    bu.ptr.p_double[i] = 1.0;
                }
                ccnt = ae_minint(3, n-1, _state);
                ae_matrix_set_length(&c, ccnt, n+1, _state);
                ae_vector_set_length(&ct, ccnt, _state);
                for(i=0; i<=ccnt-1; i++)
                {
                    ct.ptr.p_int[i] = hqrnduniformi(&rs, 3, _state)-1;
                    c.ptr.pp_double[i][n] = 0.0;
                    for(j=0; j<=n-1; j++)
                    {
                        c.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                    }
                }
                
                /*
                 * Create and optimize
                 */
                minbleiccreate(n, &x, &state, _state);
                minbleicsetbc(&state, &bl, &bu, _state);
                minbleicsetlc(&state, &c, &ct, ccnt, _state);
                minbleicsetcond(&state, 1.0E-9, 0.0, 0.0, 0, _state);
                while(minbleiciteration(&state, _state))
                {
                    ae_assert(state.needfg, "Assertion failed", _state);
                    state.f = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.f = state.f+state.x.ptr.p_double[i]*b.ptr.p_double[i];
                        state.g.ptr.p_double[i] = b.ptr.p_double[i];
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                        state.f = state.f+0.5*state.x.ptr.p_double[i]*v;
                        state.g.ptr.p_double[i] = state.g.ptr.p_double[i]+v;
                    }
                }
                minbleicresults(&state, &xs0, &rep, _state);
                seterrorflag(err, rep.terminationtype<=0, _state);
                if( *err )
                {
                    ae_frame_leave(_state);
                    return;
                }
                
                /*
                 * 1. evaluate unconstrained gradient at solution
                 *
                 * 2. calculate constrained gradient (NNLS solver is used
                 *    to evaluate gradient subject to active constraints).
                 *    In order to do this we form CE matrix, matrix of active
                 *    constraints (columns store constraint vectors). Then
                 *    we try to approximate gradient vector by columns of CE,
                 *    subject to non-negativity restriction placed on variables
                 *    corresponding to inequality constraints.
                 *
                 *    Residual from such regression is a constrained gradient vector.
                 */
                ae_vector_set_length(&g, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xs0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    g.ptr.p_double[i] = v+b.ptr.p_double[i];
                }
                ae_matrix_set_length(&ce, n, n+ccnt, _state);
                ae_vector_set_length(&nonnegative, n+ccnt, _state);
                k = 0;
                for(i=0; i<=n-1; i++)
                {
                    seterrorflag(err, ae_fp_less(xs0.ptr.p_double[i],bl.ptr.p_double[i]), _state);
                    seterrorflag(err, ae_fp_greater(xs0.ptr.p_double[i],bu.ptr.p_double[i]), _state);
                    if( ae_fp_less_eq(xs0.ptr.p_double[i],bl.ptr.p_double[i]+tolconstr) )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = 0.0;
                        }
                        ce.ptr.pp_double[i][k] = 1.0;
                        nonnegative.ptr.p_bool[k] = ae_true;
                        inc(&k, _state);
                        continue;
                    }
                    if( ae_fp_greater_eq(xs0.ptr.p_double[i],bu.ptr.p_double[i]-tolconstr) )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = 0.0;
                        }
                        ce.ptr.pp_double[i][k] = -1.0;
                        nonnegative.ptr.p_bool[k] = ae_true;
                        inc(&k, _state);
                        continue;
                    }
                }
                for(i=0; i<=ccnt-1; i++)
                {
                    v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &xs0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    v = v-c.ptr.pp_double[i][n];
                    seterrorflag(err, ct.ptr.p_int[i]==0&&ae_fp_greater(ae_fabs(v, _state),tolconstr), _state);
                    seterrorflag(err, ct.ptr.p_int[i]>0&&ae_fp_less(v,-tolconstr), _state);
                    seterrorflag(err, ct.ptr.p_int[i]<0&&ae_fp_greater(v,tolconstr), _state);
                    if( ct.ptr.p_int[i]==0 )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = c.ptr.pp_double[i][j];
                        }
                        nonnegative.ptr.p_bool[k] = ae_false;
                        inc(&k, _state);
                        continue;
                    }
                    if( (ct.ptr.p_int[i]>0&&ae_fp_less_eq(v,tolconstr))||(ct.ptr.p_int[i]<0&&ae_fp_greater_eq(v,-tolconstr)) )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = ae_sign((double)(ct.ptr.p_int[i]), _state)*c.ptr.pp_double[i][j];
                        }
                        nonnegative.ptr.p_bool[k] = ae_true;
                        inc(&k, _state);
                        continue;
                    }
                }
                snnlsinit(0, 0, 0, &nnls, _state);
                snnlssetproblem(&nnls, &ce, &g, 0, k, n, _state);
                for(i=0; i<=k-1; i++)
                {
                    if( !nonnegative.ptr.p_bool[i] )
                    {
                        snnlsdropnnc(&nnls, i, _state);
                    }
                }
                snnlssolve(&nnls, &tmp, _state);
                for(i=0; i<=k-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        g.ptr.p_double[j] = g.ptr.p_double[j]-tmp.ptr.p_double[i]*ce.ptr.pp_double[j][i];
                    }
                }
                vv = ae_v_dotproduct(&g.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,n-1));
                vv = ae_sqrt(vv, _state);
                seterrorflag(err, ae_fp_greater(vv,1.0E-5), _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests preconditioning

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testminbleicunit_testpreconditioning(ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pass;
    ae_int_t n;
    ae_vector x;
    ae_vector x0;
    ae_int_t i;
    ae_int_t k;
    ae_matrix v;
    ae_matrix c;
    ae_vector ct;
    ae_vector bl;
    ae_vector bu;
    ae_vector vd;
    ae_vector d;
    ae_vector units;
    ae_vector s;
    ae_int_t cntb1;
    ae_int_t cntb2;
    ae_int_t cntg1;
    ae_int_t cntg2;
    double epsg;
    ae_vector diagh;
    minbleicstate state;
    minbleicreport rep;
    ae_int_t ckind;
    ae_int_t fk;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);
    ae_vector_init(&vd, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&units, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&diagh, 0, DT_REAL, _state);
    _minbleicstate_init(&state, _state);
    _minbleicreport_init(&rep, _state);

    
    /*
     * Preconditioner test 1.
     *
     * If
     * * B1 is default preconditioner with unit scale
     * * G1 is diagonal preconditioner based on approximate diagonal of Hessian matrix
     * * B2 is default preconditioner with non-unit scale S[i]=1/sqrt(h[i])
     * * G2 is scale-based preconditioner with non-unit scale S[i]=1/sqrt(h[i])
     * then B1 is worse than G1, B2 is worse than G2.
     * "Worse" means more iterations to converge.
     *
     * Test problem setup:
     * * f(x) = sum( ((i*i+1)*x[i])^2, i=0..N-1)
     * * constraints:
     *   0) absent
     *   1) boundary only
     *   2) linear equality only
     *   3) combination of boundary and linear equality constraints
     *
     * N        - problem size
     * K        - number of repeated passes (should be large enough to average out random factors)
     */
    k = 100;
    epsg = 1.0E-8;
    for(n=10; n<=10; n++)
    {
        for(ckind=0; ckind<=3; ckind++)
        {
            fk = 1;
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&units, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = (double)(0);
                units.ptr.p_double[i] = (double)(1);
            }
            minbleiccreate(n, &x, &state, _state);
            minbleicsetcond(&state, epsg, 0.0, 0.0, 0, _state);
            if( ckind==1||ckind==3 )
            {
                ae_vector_set_length(&bl, n, _state);
                ae_vector_set_length(&bu, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    bl.ptr.p_double[i] = (double)(-1);
                    bu.ptr.p_double[i] = (double)(1);
                }
                minbleicsetbc(&state, &bl, &bu, _state);
            }
            if( ckind==2||ckind==3 )
            {
                ae_matrix_set_length(&c, 1, n+1, _state);
                ae_vector_set_length(&ct, 1, _state);
                ct.ptr.p_int[0] = ae_randominteger(3, _state)-1;
                for(i=0; i<=n-1; i++)
                {
                    c.ptr.pp_double[0][i] = 2*ae_randomreal(_state)-1;
                }
                c.ptr.pp_double[0][n] = (double)(0);
                minbleicsetlc(&state, &c, &ct, 1, _state);
            }
            
            /*
             * Test it with default preconditioner VS. perturbed diagonal preconditioner
             */
            minbleicsetprecdefault(&state, _state);
            minbleicsetscale(&state, &units, _state);
            cntb1 = 0;
            for(pass=0; pass<=k-1; pass++)
            {
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                minbleicrestartfrom(&state, &x, _state);
                while(minbleiciteration(&state, _state))
                {
                    testminbleicunit_calciip2(&state, n, fk, _state);
                }
                minbleicresults(&state, &x, &rep, _state);
                cntb1 = cntb1+rep.inneriterationscount;
                *err = *err||rep.terminationtype<=0;
            }
            ae_vector_set_length(&diagh, n, _state);
            for(i=0; i<=n-1; i++)
            {
                diagh.ptr.p_double[i] = 2*ae_pow((double)(i*i+1), (double)(2*fk), _state)*(0.8+0.4*ae_randomreal(_state));
            }
            minbleicsetprecdiag(&state, &diagh, _state);
            minbleicsetscale(&state, &units, _state);
            cntg1 = 0;
            for(pass=0; pass<=k-1; pass++)
            {
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                minbleicrestartfrom(&state, &x, _state);
                while(minbleiciteration(&state, _state))
                {
                    testminbleicunit_calciip2(&state, n, fk, _state);
                }
                minbleicresults(&state, &x, &rep, _state);
                cntg1 = cntg1+rep.inneriterationscount;
                *err = *err||rep.terminationtype<=0;
            }
            *err = *err||cntb1<cntg1;
            
            /*
             * Test it with scale-based preconditioner
             */
            ae_vector_set_length(&s, n, _state);
            for(i=0; i<=n-1; i++)
            {
                s.ptr.p_double[i] = 1/ae_sqrt(2*ae_pow((double)(i*i+1), (double)(2*fk), _state)*(0.8+0.4*ae_randomreal(_state)), _state);
            }
            minbleicsetprecdefault(&state, _state);
            minbleicsetscale(&state, &s, _state);
            cntb2 = 0;
            for(pass=0; pass<=k-1; pass++)
            {
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                minbleicrestartfrom(&state, &x, _state);
                while(minbleiciteration(&state, _state))
                {
                    testminbleicunit_calciip2(&state, n, fk, _state);
                }
                minbleicresults(&state, &x, &rep, _state);
                cntb2 = cntb2+rep.inneriterationscount;
                *err = *err||rep.terminationtype<=0;
            }
            minbleicsetprecscale(&state, _state);
            minbleicsetscale(&state, &s, _state);
            cntg2 = 0;
            for(pass=0; pass<=k-1; pass++)
            {
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                minbleicrestartfrom(&state, &x, _state);
                while(minbleiciteration(&state, _state))
                {
                    testminbleicunit_calciip2(&state, n, fk, _state);
                }
                minbleicresults(&state, &x, &rep, _state);
                cntg2 = cntg2+rep.inneriterationscount;
                *err = *err||rep.terminationtype<=0;
            }
            *err = *err||cntb2<cntg2;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function sets random preconditioner:
* unit one, for PrecKind=0
* diagonal-based one, for PrecKind=1
* scale-based one, for PrecKind=2
*************************************************************************/
static void testminbleicunit_setrandompreconditioner(minbleicstate* state,
     ae_int_t n,
     ae_int_t preckind,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector p;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&p, 0, DT_REAL, _state);

    if( preckind==1 )
    {
        ae_vector_set_length(&p, n, _state);
        for(i=0; i<=n-1; i++)
        {
            p.ptr.p_double[i] = ae_exp(6*ae_randomreal(_state)-3, _state);
        }
        minbleicsetprecdiag(state, &p, _state);
    }
    else
    {
        minbleicsetprecdefault(state, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests, that gradient verified correctly.
*************************************************************************/
static void testminbleicunit_testgradientcheck(ae_bool* testg,
     ae_state *_state)
{
    ae_frame _frame_block;
    minbleicstate state;
    minbleicreport rep;
    ae_int_t n;
    double a;
    double b;
    double c;
    double d;
    double x0;
    double x1;
    double x2;
    ae_vector x;
    ae_vector bl;
    ae_vector bu;
    ae_int_t infcomp;
    double teststep;
    double noise;
    ae_int_t nbrcomp;
    double spp;
    ae_int_t func;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *testg = ae_false;
    _minbleicstate_init(&state, _state);
    _minbleicreport_init(&rep, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);

    passcount = 35;
    spp = 1.0;
    teststep = 0.01;
    n = 3;
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&bl, n, _state);
    ae_vector_set_length(&bu, n, _state);
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
        
        /*
         * Prepare boundary parameters
         */
        for(i=0; i<=n-1; i++)
        {
            bl.ptr.p_double[i] = ae_randomreal(_state)-spp;
            bu.ptr.p_double[i] = ae_randomreal(_state)+spp-1;
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
        minbleiccreate(n, &x, &state, _state);
        minbleicsetgradientcheck(&state, teststep, _state);
        minbleicsetbc(&state, &bl, &bu, _state);
        
        /*
         * Check that the criterion passes a derivative if it is correct
         */
        while(minbleiciteration(&state, _state))
        {
            if( state.needfg )
            {
                
                /*
                 * Check that .X within the boundaries
                 */
                for(i=0; i<=n-1; i++)
                {
                    if( (ae_isfinite(bl.ptr.p_double[i], _state)&&ae_fp_less(state.x.ptr.p_double[i],bl.ptr.p_double[i]))||(ae_isfinite(bu.ptr.p_double[i], _state)&&ae_fp_greater(state.x.ptr.p_double[i],bu.ptr.p_double[i])) )
                    {
                        *testg = ae_true;
                        ae_frame_leave(_state);
                        return;
                    }
                }
                testminbleicunit_funcderiv(a, b, c, d, x0, x1, x2, &state.x, func, &state.f, &state.g, _state);
            }
        }
        minbleicresults(&state, &x, &rep, _state);
        
        /*
         * Check that error code does not equal to -7 and parameter .VarIdx
         * equal to -1.
         */
        if( rep.terminationtype==-7||rep.varidx!=-1 )
        {
            *testg = ae_true;
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = 5*randomnormal(_state);
        }
        minbleicrestartfrom(&state, &x, _state);
        
        /*
         * Check that the criterion does not miss a derivative if
         * it is incorrect
         */
        while(minbleiciteration(&state, _state))
        {
            if( state.needfg )
            {
                for(i=0; i<=n-1; i++)
                {
                    if( (ae_isfinite(bl.ptr.p_double[i], _state)&&ae_fp_less(state.x.ptr.p_double[i],bl.ptr.p_double[i]))||(ae_isfinite(bu.ptr.p_double[i], _state)&&ae_fp_greater(state.x.ptr.p_double[i],bu.ptr.p_double[i])) )
                    {
                        *testg = ae_true;
                        ae_frame_leave(_state);
                        return;
                    }
                }
                testminbleicunit_funcderiv(a, b, c, d, x0, x1, x2, &state.x, func, &state.f, &state.g, _state);
                state.g.ptr.p_double[nbrcomp] = state.g.ptr.p_double[nbrcomp]+noise;
            }
        }
        minbleicresults(&state, &x, &rep, _state);
        
        /*
         * Check that error code equal to -7 and parameter .VarIdx
         * equal to number of incorrect component.
         */
        if( rep.terminationtype!=-7||rep.varidx!=nbrcomp )
        {
            *testg = ae_true;
            ae_frame_leave(_state);
            return;
        }
    }
    *testg = ae_false;
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests problems which caused bugs in the past.

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testminbleicunit_testbugs(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    ae_vector bl;
    ae_vector bu;
    ae_vector x;
    ae_vector x1;
    ae_vector h;
    ae_vector prior;
    ae_vector w;
    ae_matrix a;
    ae_matrix c;
    ae_matrix xy;
    ae_vector ct;
    minbleicstate state;
    minbleicreport rep;
    hqrndstate rs;
    ae_int_t pass;
    double tolx;
    double regterm;
    ae_int_t n;
    ae_int_t ckind;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&h, 0, DT_REAL, _state);
    ae_vector_init(&prior, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    _minbleicstate_init(&state, _state);
    _minbleicreport_init(&rep, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Reproduce situation: optimizer sometimes hangs when starts with
     * gradient orthogonal to the only linear constraint. In most cases
     * it is solved successfully, but sometimes leads to infinite loop
     * in one of the early optimizer versions.
     *
     * The problem is:
     * * f(x)= x'*x + c'*x
     * * linear constraint c'*x=0
     * * initial point is x=0
     * * there are two ways to choose coefficient vector c:
     *   * its components can be long binary fractions
     *   * or they can be either 0 or 1
     *   both ways test different scenarios for accumulation of rounding errors
     *
     * If test fails, it usually hangs
     */
    tolx = 1.0E-10;
    for(pass=1; pass<=10; pass++)
    {
        for(ckind=0; ckind<=1; ckind++)
        {
            for(n=2; n<=10; n++)
            {
                ae_vector_set_length(&x, n, _state);
                ae_matrix_set_length(&c, 1, n+1, _state);
                ae_vector_set_length(&ct, 1, _state);
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = 0.0;
                    if( ckind==0 )
                    {
                        c.ptr.pp_double[0][i] = ae_sqrt(hqrnduniformr(&rs, _state), _state);
                    }
                    else
                    {
                        c.ptr.pp_double[0][i] = (double)(hqrnduniformi(&rs, 2, _state));
                    }
                }
                c.ptr.pp_double[0][n] = 0.0;
                ct.ptr.p_int[0] = 0;
                minbleiccreate(n, &x, &state, _state);
                minbleicsetlc(&state, &c, &ct, 1, _state);
                minbleicsetcond(&state, 0.0, 0.0, 0.0, 99, _state);
                while(minbleiciteration(&state, _state))
                {
                    ae_assert(state.needfg, "Assertion failed", _state);
                    if( state.needfg )
                    {
                        state.f = 0.0;
                        for(i=0; i<=n-1; i++)
                        {
                            state.f = state.f+ae_sqr(state.x.ptr.p_double[i], _state)+state.x.ptr.p_double[i]*c.ptr.pp_double[0][i];
                            state.g.ptr.p_double[i] = 2*state.x.ptr.p_double[i]+c.ptr.pp_double[0][i];
                        }
                    }
                }
                minbleicresultsbuf(&state, &x1, &rep, _state);
                seterrorflag(err, rep.terminationtype<=0, _state);
                for(i=0; i<=n-1; i++)
                {
                    seterrorflag(err, ae_fp_greater(ae_fabs(x1.ptr.p_double[i], _state),tolx), _state);
                }
            }
        }
    }
    
    /*
     * Reproduce optimization problem which caused bugs (optimizer hangs)
     * when BLEIC was used from MCPD unit. We perform test on specific
     * 9-dimensional problem, no need to try general-case methods.
     *
     * This test hangs if bug is present. Thus, we do not test completion
     * code returned by optimizer - we just test that it was returned :)
     */
    tolx = 1.0E-8;
    regterm = 1.0E-8;
    for(pass=1; pass<=1000; pass++)
    {
        
        /*
         * Prepare constraints:
         * * [0,1] box constraints on all variables
         * * 5 linear constraints, first one is random equality;
         *   second one is random inequality; other ones are "sum-to-one" constraints
         *   for x0-x2, x3-x5, x6-x8.
         */
        ae_vector_set_length(&bl, 9, _state);
        ae_vector_set_length(&bu, 9, _state);
        for(i=0; i<=9-1; i++)
        {
            bl.ptr.p_double[i] = 0.0;
            bu.ptr.p_double[i] = 1.0;
        }
        ae_matrix_set_length(&c, 5, 10, _state);
        ae_vector_set_length(&ct, 5, _state);
        for(i=0; i<=1; i++)
        {
            c.ptr.pp_double[i][9] = (double)(0);
            for(j=0; j<=9-1; j++)
            {
                c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                c.ptr.pp_double[i][9] = c.ptr.pp_double[i][9]+c.ptr.pp_double[i][j]*((double)1/(double)9);
            }
        }
        ct.ptr.p_int[0] = 0;
        ct.ptr.p_int[1] = 1;
        c.ptr.pp_double[1][9] = c.ptr.pp_double[1][9]-0.1;
        for(i=0; i<=3-1; i++)
        {
            for(k=0; k<=9-1; k++)
            {
                c.ptr.pp_double[2+i][k] = (double)(0);
            }
            for(k=0; k<=3-1; k++)
            {
                c.ptr.pp_double[2+i][k*3+i] = (double)(1);
            }
            c.ptr.pp_double[2+i][9] = 1.0;
            ct.ptr.p_int[2+i] = 0;
        }
        
        /*
         * Prepare weights
         */
        ae_vector_set_length(&w, 3, _state);
        for(i=0; i<=w.cnt-1; i++)
        {
            w.ptr.p_double[i] = 1.0;
        }
        
        /*
         * Prepare preconditioner H
         */
        ae_vector_set_length(&h, 9, _state);
        for(i=0; i<=h.cnt-1; i++)
        {
            h.ptr.p_double[i] = 1.0;
        }
        
        /*
         * Prepare prior value for regularization
         */
        ae_vector_set_length(&prior, 9, _state);
        for(i=0; i<=prior.cnt-1; i++)
        {
            prior.ptr.p_double[i] = (double)(0);
        }
        prior.ptr.p_double[0] = 1.0;
        prior.ptr.p_double[4] = 1.0;
        prior.ptr.p_double[8] = 1.0;
        
        /*
         * Prepare dataset XY
         */
        ae_matrix_set_length(&xy, 6, 3, _state);
        for(i=0; i<=xy.rows-1; i++)
        {
            for(j=0; j<=xy.cols-1; j++)
            {
                xy.ptr.pp_double[i][j] = ae_randomreal(_state);
            }
        }
        
        /*
         * Optimize
         */
        ae_vector_set_length(&x, 9, _state);
        for(i=0; i<=9-1; i++)
        {
            x.ptr.p_double[i] = (double)1/(double)9;
        }
        minbleiccreate(9, &x, &state, _state);
        minbleicsetbc(&state, &bl, &bu, _state);
        minbleicsetlc(&state, &c, &ct, 5, _state);
        minbleicsetcond(&state, 0.0, 0.0, tolx, 0, _state);
        minbleicsetprecdiag(&state, &h, _state);
        while(minbleiciteration(&state, _state))
        {
            ae_assert(state.needfg, "Assertion failed", _state);
            if( state.needfg )
            {
                
                /*
                 * Calculate regularization term
                 */
                state.f = 0.0;
                for(i=0; i<=9-1; i++)
                {
                    state.f = state.f+regterm*ae_sqr(state.x.ptr.p_double[i]-prior.ptr.p_double[i], _state);
                    state.g.ptr.p_double[i] = 2*regterm*(state.x.ptr.p_double[i]-prior.ptr.p_double[i]);
                }
                
                /*
                 * calculate prediction error/gradient for K-th pair
                 */
                for(k=0; k<=xy.rows-2; k++)
                {
                    for(i=0; i<=3-1; i++)
                    {
                        v = ae_v_dotproduct(&state.x.ptr.p_double[i*3], 1, &xy.ptr.pp_double[k][0], 1, ae_v_len(i*3,i*3+3-1));
                        state.f = state.f+ae_sqr(w.ptr.p_double[i]*(v-xy.ptr.pp_double[k+1][i]), _state);
                        for(j=0; j<=3-1; j++)
                        {
                            state.g.ptr.p_double[i*3+j] = state.g.ptr.p_double[i*3+j]+2*w.ptr.p_double[i]*w.ptr.p_double[i]*(v-xy.ptr.pp_double[k+1][i])*xy.ptr.pp_double[k][j];
                        }
                    }
                }
            }
        }
        minbleicresultsbuf(&state, &x, &rep, _state);
    }
    ae_frame_leave(_state);
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
static void testminbleicunit_funcderiv(double a,
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
