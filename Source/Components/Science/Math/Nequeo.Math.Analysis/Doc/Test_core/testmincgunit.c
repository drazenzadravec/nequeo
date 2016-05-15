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
#include "testmincgunit.h"


/*$ Declarations $*/
static void testmincgunit_testfunc2(mincgstate* state, ae_state *_state);
static void testmincgunit_testfunc3(mincgstate* state, ae_state *_state);
static void testmincgunit_calciip2(mincgstate* state,
     ae_int_t n,
     ae_state *_state);
static void testmincgunit_calclowrank(mincgstate* state,
     ae_int_t n,
     ae_int_t vcnt,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_matrix* v,
     /* Real    */ ae_vector* vd,
     /* Real    */ ae_vector* x0,
     ae_state *_state);
static void testmincgunit_testpreconditioning(ae_bool* err,
     ae_state *_state);
static ae_bool testmincgunit_gradientchecktest(ae_state *_state);
static void testmincgunit_funcderiv(double a,
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


ae_bool testmincg(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_bool referror;
    ae_bool eqerror;
    ae_bool linerror1;
    ae_bool linerror2;
    ae_bool restartserror;
    ae_bool precerror;
    ae_bool converror;
    ae_bool othererrors;
    ae_bool graderrors;
    ae_int_t n;
    ae_vector x;
    ae_vector xe;
    ae_vector b;
    ae_vector xlast;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_matrix a;
    ae_vector diagh;
    mincgstate state;
    mincgreport rep;
    ae_int_t cgtype;
    ae_int_t difftype;
    double diffstep;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&xe, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&xlast, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&diagh, 0, DT_REAL, _state);
    _mincgstate_init(&state, _state);
    _mincgreport_init(&rep, _state);

    waserrors = ae_false;
    referror = ae_false;
    linerror1 = ae_false;
    linerror2 = ae_false;
    eqerror = ae_false;
    converror = ae_false;
    restartserror = ae_false;
    othererrors = ae_false;
    precerror = ae_false;
    testmincgunit_testpreconditioning(&precerror, _state);
    testother(&othererrors, _state);
    for(difftype=0; difftype<=1; difftype++)
    {
        for(cgtype=-1; cgtype<=1; cgtype++)
        {
            
            /*
             * Reference problem
             */
            ae_vector_set_length(&x, 2+1, _state);
            n = 3;
            diffstep = 1.0E-6;
            x.ptr.p_double[0] = 100*ae_randomreal(_state)-50;
            x.ptr.p_double[1] = 100*ae_randomreal(_state)-50;
            x.ptr.p_double[2] = 100*ae_randomreal(_state)-50;
            if( difftype==0 )
            {
                mincgcreate(n, &x, &state, _state);
            }
            if( difftype==1 )
            {
                mincgcreatef(n, &x, diffstep, &state, _state);
            }
            mincgsetcgtype(&state, cgtype, _state);
            while(mincgiteration(&state, _state))
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
            mincgresults(&state, &x, &rep, _state);
            referror = (((referror||rep.terminationtype<=0)||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-2, _state),0.001))||ae_fp_greater(ae_fabs(x.ptr.p_double[1], _state),0.001))||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-2, _state),0.001);
            
            /*
             * F2 problem with restarts:
             * * make several iterations and restart BEFORE termination
             * * iterate and restart AFTER termination
             *
             * NOTE: step is bounded from above to avoid premature convergence
             */
            ae_vector_set_length(&x, 3, _state);
            n = 3;
            diffstep = 1.0E-6;
            x.ptr.p_double[0] = 10+10*ae_randomreal(_state);
            x.ptr.p_double[1] = 10+10*ae_randomreal(_state);
            x.ptr.p_double[2] = 10+10*ae_randomreal(_state);
            if( difftype==0 )
            {
                mincgcreate(n, &x, &state, _state);
            }
            if( difftype==1 )
            {
                mincgcreatef(n, &x, diffstep, &state, _state);
            }
            mincgsetcgtype(&state, cgtype, _state);
            mincgsetstpmax(&state, 0.1, _state);
            mincgsetcond(&state, 0.0000001, 0.0, 0.0, 0, _state);
            for(i=0; i<=10; i++)
            {
                if( !mincgiteration(&state, _state) )
                {
                    break;
                }
                testmincgunit_testfunc2(&state, _state);
            }
            x.ptr.p_double[0] = 10+10*ae_randomreal(_state);
            x.ptr.p_double[1] = 10+10*ae_randomreal(_state);
            x.ptr.p_double[2] = 10+10*ae_randomreal(_state);
            mincgrestartfrom(&state, &x, _state);
            while(mincgiteration(&state, _state))
            {
                testmincgunit_testfunc2(&state, _state);
            }
            mincgresults(&state, &x, &rep, _state);
            restartserror = (((restartserror||rep.terminationtype<=0)||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-ae_log((double)(2), _state), _state),0.01))||ae_fp_greater(ae_fabs(x.ptr.p_double[1], _state),0.01))||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-ae_log((double)(2), _state), _state),0.01);
            x.ptr.p_double[0] = 10+10*ae_randomreal(_state);
            x.ptr.p_double[1] = 10+10*ae_randomreal(_state);
            x.ptr.p_double[2] = 10+10*ae_randomreal(_state);
            mincgrestartfrom(&state, &x, _state);
            while(mincgiteration(&state, _state))
            {
                testmincgunit_testfunc2(&state, _state);
            }
            mincgresults(&state, &x, &rep, _state);
            restartserror = (((restartserror||rep.terminationtype<=0)||ae_fp_greater(ae_fabs(x.ptr.p_double[0]-ae_log((double)(2), _state), _state),0.01))||ae_fp_greater(ae_fabs(x.ptr.p_double[1], _state),0.01))||ae_fp_greater(ae_fabs(x.ptr.p_double[2]-ae_log((double)(2), _state), _state),0.01);
            
            /*
             * 1D problem #1
             */
            ae_vector_set_length(&x, 0+1, _state);
            n = 1;
            diffstep = 1.0E-6;
            x.ptr.p_double[0] = 100*ae_randomreal(_state)-50;
            if( difftype==0 )
            {
                mincgcreate(n, &x, &state, _state);
            }
            if( difftype==1 )
            {
                mincgcreatef(n, &x, diffstep, &state, _state);
            }
            mincgsetcgtype(&state, cgtype, _state);
            while(mincgiteration(&state, _state))
            {
                if( state.needf||state.needfg )
                {
                    state.f = -ae_cos(state.x.ptr.p_double[0], _state);
                }
                if( state.needfg )
                {
                    state.g.ptr.p_double[0] = ae_sin(state.x.ptr.p_double[0], _state);
                }
            }
            mincgresults(&state, &x, &rep, _state);
            linerror1 = (linerror1||rep.terminationtype<=0)||ae_fp_greater(ae_fabs(x.ptr.p_double[0]/ae_pi-ae_round(x.ptr.p_double[0]/ae_pi, _state), _state),0.001);
            
            /*
             * 1D problem #2
             */
            ae_vector_set_length(&x, 0+1, _state);
            n = 1;
            diffstep = 1.0E-6;
            x.ptr.p_double[0] = 100*ae_randomreal(_state)-50;
            if( difftype==0 )
            {
                mincgcreate(n, &x, &state, _state);
            }
            if( difftype==1 )
            {
                mincgcreatef(n, &x, diffstep, &state, _state);
            }
            mincgsetcgtype(&state, cgtype, _state);
            while(mincgiteration(&state, _state))
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
            mincgresults(&state, &x, &rep, _state);
            linerror2 = (linerror2||rep.terminationtype<=0)||ae_fp_greater(ae_fabs(x.ptr.p_double[0], _state),0.001);
            
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
                 * Solve task
                 */
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                if( difftype==0 )
                {
                    mincgcreate(n, &x, &state, _state);
                }
                if( difftype==1 )
                {
                    mincgcreatef(n, &x, diffstep, &state, _state);
                }
                mincgsetcgtype(&state, cgtype, _state);
                while(mincgiteration(&state, _state))
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
                mincgresults(&state, &x, &rep, _state);
                eqerror = eqerror||rep.terminationtype<=0;
                for(i=0; i<=n-1; i++)
                {
                    eqerror = eqerror||ae_fp_greater(ae_fabs(x.ptr.p_double[i]-xe.ptr.p_double[i], _state),0.001);
                }
            }
            
            /*
             * Testing convergence properties
             */
            diffstep = 1.0E-6;
            n = 3;
            ae_vector_set_length(&x, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 6*ae_randomreal(_state)-3;
            }
            if( difftype==0 )
            {
                mincgcreate(n, &x, &state, _state);
            }
            if( difftype==1 )
            {
                mincgcreatef(n, &x, diffstep, &state, _state);
            }
            mincgsetcond(&state, 0.001, 0.0, 0.0, 0, _state);
            mincgsetcgtype(&state, cgtype, _state);
            while(mincgiteration(&state, _state))
            {
                testmincgunit_testfunc3(&state, _state);
            }
            mincgresults(&state, &x, &rep, _state);
            converror = converror||rep.terminationtype!=4;
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 6*ae_randomreal(_state)-3;
            }
            if( difftype==0 )
            {
                mincgcreate(n, &x, &state, _state);
            }
            if( difftype==1 )
            {
                mincgcreatef(n, &x, diffstep, &state, _state);
            }
            mincgsetcond(&state, 0.0, 0.001, 0.0, 0, _state);
            mincgsetcgtype(&state, cgtype, _state);
            while(mincgiteration(&state, _state))
            {
                testmincgunit_testfunc3(&state, _state);
            }
            mincgresults(&state, &x, &rep, _state);
            converror = converror||rep.terminationtype!=1;
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 6*ae_randomreal(_state)-3;
            }
            if( difftype==0 )
            {
                mincgcreate(n, &x, &state, _state);
            }
            if( difftype==1 )
            {
                mincgcreatef(n, &x, diffstep, &state, _state);
            }
            mincgsetcond(&state, 0.0, 0.0, 0.001, 0, _state);
            mincgsetcgtype(&state, cgtype, _state);
            while(mincgiteration(&state, _state))
            {
                testmincgunit_testfunc3(&state, _state);
            }
            mincgresults(&state, &x, &rep, _state);
            converror = converror||rep.terminationtype!=2;
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            if( difftype==0 )
            {
                mincgcreate(n, &x, &state, _state);
            }
            if( difftype==1 )
            {
                mincgcreatef(n, &x, diffstep, &state, _state);
            }
            mincgsetcond(&state, 0.0, 0.0, 0.0, 10, _state);
            mincgsetcgtype(&state, cgtype, _state);
            while(mincgiteration(&state, _state))
            {
                testmincgunit_testfunc3(&state, _state);
            }
            mincgresults(&state, &x, &rep, _state);
            converror = converror||!((rep.terminationtype==5&&rep.iterationscount==10)||rep.terminationtype==7);
        }
    }
    
    /*
     *  Test for MinCGGradientCheck
     */
    graderrors = testmincgunit_gradientchecktest(_state);
    
    /*
     * end
     */
    waserrors = (((((((referror||eqerror)||linerror1)||linerror2)||converror)||othererrors)||restartserror)||precerror)||graderrors;
    if( !silent )
    {
        printf("TESTING CG OPTIMIZATION\n");
        printf("REFERENCE PROBLEM:                        ");
        if( referror )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("LIN-1 PROBLEM:                            ");
        if( linerror1 )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("LIN-2 PROBLEM:                            ");
        if( linerror2 )
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
        printf("PRECONDITIONING:                          ");
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
ae_bool _pexec_testmincg(ae_bool silent, ae_state *_state)
{
    return testmincg(silent, _state);
}


/*************************************************************************
Other properties
*************************************************************************/
void testother(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_vector x;
    ae_vector s;
    ae_vector a;
    ae_vector b;
    ae_vector h;
    ae_vector x0;
    ae_vector x1;
    ae_vector xlast;
    ae_matrix fulla;
    double fprev;
    double xprev;
    double stpmax;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    mincgstate state;
    mincgreport rep;
    ae_int_t cgtype;
    double tmpeps;
    double epsg;
    double v;
    double r;
    ae_bool hasxlast;
    double lastscaledstep;
    ae_int_t pkind;
    ae_int_t ckind;
    ae_int_t mkind;
    ae_int_t dkind;
    double diffstep;
    double vc;
    double vm;
    ae_bool wasf;
    ae_bool wasfg;
    hqrndstate rs;
    ae_int_t spoiliteration;
    ae_int_t stopiteration;
    ae_int_t spoilvar;
    double spoilval;
    ae_int_t pass;
    double ss;
    ae_int_t callidx;
    ae_int_t stopcallidx;
    ae_int_t maxits;
    ae_bool terminationrequested;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&h, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&xlast, 0, DT_REAL, _state);
    ae_matrix_init(&fulla, 0, 0, DT_REAL, _state);
    _mincgstate_init(&state, _state);
    _mincgreport_init(&rep, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    for(cgtype=-1; cgtype<=1; cgtype++)
    {
        
        /*
         * Test reports (F should form monotone sequence)
         */
        n = 50;
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&xlast, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = (double)(1);
        }
        mincgcreate(n, &x, &state, _state);
        mincgsetcond(&state, (double)(0), (double)(0), (double)(0), 100, _state);
        mincgsetxrep(&state, ae_true, _state);
        fprev = ae_maxrealnumber;
        while(mincgiteration(&state, _state))
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
        mincgresults(&state, &x, &rep, _state);
        for(i=0; i<=n-1; i++)
        {
            *err = *err||ae_fp_neq(x.ptr.p_double[i],xlast.ptr.p_double[i]);
        }
        
        /*
         * Test differentiation vs. analytic gradient
         * (first one issues NeedF requests, second one issues NeedFG requests)
         */
        n = 50;
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
                mincgcreate(n, &x, &state, _state);
            }
            if( dkind==1 )
            {
                mincgcreatef(n, &x, diffstep, &state, _state);
            }
            mincgsetcond(&state, (double)(0), (double)(0), (double)(0), n/2, _state);
            wasf = ae_false;
            wasfg = ae_false;
            while(mincgiteration(&state, _state))
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
            mincgresults(&state, &x, &rep, _state);
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
        mincgcreatef(1, &x, diffstep, &state, _state);
        mincgsetcond(&state, 1.0E-6, (double)(0), (double)(0), 0, _state);
        mincgsetscale(&state, &s, _state);
        v = (double)(0);
        while(mincgiteration(&state, _state))
        {
            state.f = ae_sqr(state.x.ptr.p_double[0], _state);
            v = ae_maxreal(v, ae_fabs(state.x.ptr.p_double[0], _state), _state);
        }
        mincgresults(&state, &x, &rep, _state);
        r = v/(s.ptr.p_double[0]*diffstep);
        *err = *err||ae_fp_greater(ae_fabs(ae_log(r, _state), _state),ae_log(1+1000*ae_machineepsilon, _state));
        
        /*
         * Test maximum step
         */
        n = 1;
        ae_vector_set_length(&x, n, _state);
        x.ptr.p_double[0] = (double)(100);
        stpmax = 0.05+0.05*ae_randomreal(_state);
        mincgcreate(n, &x, &state, _state);
        mincgsetcond(&state, 1.0E-9, (double)(0), (double)(0), 0, _state);
        mincgsetstpmax(&state, stpmax, _state);
        mincgsetxrep(&state, ae_true, _state);
        xprev = x.ptr.p_double[0];
        while(mincgiteration(&state, _state))
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
                mincgcreate(n, &x, &state, _state);
                mincgsetscale(&state, &s, _state);
                mincgsetxrep(&state, ae_true, _state);
                if( pkind==1 )
                {
                    mincgsetprecdiag(&state, &h, _state);
                }
                if( pkind==2 )
                {
                    mincgsetprecscale(&state, _state);
                }
                
                /*
                 * Test gradient-based stopping condition
                 */
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = ae_randomreal(_state)+1;
                }
                mincgsetcond(&state, tmpeps, (double)(0), (double)(0), 0, _state);
                mincgrestartfrom(&state, &x, _state);
                while(mincgiteration(&state, _state))
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
                mincgresults(&state, &x, &rep, _state);
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
                mincgsetcond(&state, (double)(0), (double)(0), tmpeps, 0, _state);
                mincgrestartfrom(&state, &x, _state);
                lastscaledstep = (double)(0);
                while(mincgiteration(&state, _state))
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
                mincgresults(&state, &x, &rep, _state);
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
                vc = 1.0;
                vm = 1.0E+8;
                if( ckind==1 )
                {
                    vc = 1.0E+6;
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
                mincgcreate(1, &x, &state, _state);
                mincgsetcond(&state, epsg, (double)(0), (double)(0), 0, _state);
                mincgsetcgtype(&state, cgtype, _state);
                while(mincgiteration(&state, _state))
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
                mincgresults(&state, &x, &rep, _state);
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
        mincgcreate(n, &x0, &state, _state);
        mincgsetcond(&state, 0.0, 0.0, 0.0, stopiteration, _state);
        mincgsetxrep(&state, ae_true, _state);
        k = -1;
        while(mincgiteration(&state, _state))
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
        mincgresults(&state, &x1, &rep, _state);
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
        mincgcreate(n, &x, &state, _state);
        mincgsetcond(&state, (double)(0), (double)(0), (double)(0), maxits, _state);
        mincgsetxrep(&state, ae_true, _state);
        callidx = 0;
        terminationrequested = ae_false;
        ae_v_move(&xlast.ptr.p_double[0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
        while(mincgiteration(&state, _state))
        {
            if( state.needfg )
            {
                state.f = ss*ae_sqr(ae_exp(state.x.ptr.p_double[0], _state)-2, _state)+ae_sqr(state.x.ptr.p_double[1], _state)+ae_sqr(state.x.ptr.p_double[2]-state.x.ptr.p_double[0], _state);
                state.g.ptr.p_double[0] = 2*ss*(ae_exp(state.x.ptr.p_double[0], _state)-2)*ae_exp(state.x.ptr.p_double[0], _state)+2*(state.x.ptr.p_double[2]-state.x.ptr.p_double[0])*(-1);
                state.g.ptr.p_double[1] = 2*state.x.ptr.p_double[1];
                state.g.ptr.p_double[2] = 2*(state.x.ptr.p_double[2]-state.x.ptr.p_double[0]);
                if( callidx==stopcallidx )
                {
                    mincgrequesttermination(&state, _state);
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
        mincgresults(&state, &x, &rep, _state);
        seterrorflag(err, rep.terminationtype!=8, _state);
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(err, ae_fp_neq(x.ptr.p_double[i],xlast.ptr.p_double[i]), _state);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Calculate test function #2

Simple variation of #1, much more nonlinear, which makes unlikely premature
convergence of algorithm .
*************************************************************************/
static void testmincgunit_testfunc2(mincgstate* state, ae_state *_state)
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
static void testmincgunit_testfunc3(mincgstate* state, ae_state *_state)
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
static void testmincgunit_calciip2(mincgstate* state,
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
Calculate test function f(x) = 0.5*(x-x0)'*A*(x-x0), A = D+V'*Vd*V
*************************************************************************/
static void testmincgunit_calclowrank(mincgstate* state,
     ae_int_t n,
     ae_int_t vcnt,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_matrix* v,
     /* Real    */ ae_vector* vd,
     /* Real    */ ae_vector* x0,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double dx;
    double t;
    double t2;


    state->f = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        state->g.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        dx = state->x.ptr.p_double[i]-x0->ptr.p_double[i];
        state->f = state->f+0.5*dx*d->ptr.p_double[i]*dx;
        state->g.ptr.p_double[i] = state->g.ptr.p_double[i]+d->ptr.p_double[i]*dx;
    }
    for(i=0; i<=vcnt-1; i++)
    {
        t = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            t = t+v->ptr.pp_double[i][j]*(state->x.ptr.p_double[j]-x0->ptr.p_double[j]);
        }
        state->f = state->f+0.5*t*vd->ptr.p_double[i]*t;
        t2 = t*vd->ptr.p_double[i];
        ae_v_addd(&state->g.ptr.p_double[0], 1, &v->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), t2);
    }
}


/*************************************************************************
This function tests preconditioning

On failure sets Err to True (leaves it unchanged otherwise)
*************************************************************************/
static void testmincgunit_testpreconditioning(ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pass;
    ae_int_t n;
    ae_vector x;
    ae_vector x0;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t vs;
    ae_matrix v;
    ae_vector vd;
    ae_vector d;
    ae_vector s;
    ae_int_t cntb1;
    ae_int_t cntg1;
    ae_int_t cntb2;
    ae_int_t cntg2;
    double epsg;
    ae_vector diagh;
    mincgstate state;
    mincgreport rep;
    ae_int_t cgtype;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);
    ae_vector_init(&vd, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&diagh, 0, DT_REAL, _state);
    _mincgstate_init(&state, _state);
    _mincgreport_init(&rep, _state);

    k = 50;
    epsg = 1.0E-10;
    for(cgtype=-1; cgtype<=1; cgtype++)
    {
        
        /*
         * Preconditioner test 1.
         *
         * If
         * * B1 is default preconditioner
         * * G1 is diagonal precomditioner based on approximate diagonal of Hessian matrix
         * then "bad" preconditioner is worse than "good" one.
         * "Worse" means more iterations to converge.
         *
         *
         * We test it using f(x) = sum( ((i*i+1)*x[i])^2, i=0..N-1).
         *
         * N        - problem size
         * K        - number of repeated passes (should be large enough to average out random factors)
         */
        for(n=10; n<=15; n++)
        {
            ae_vector_set_length(&x, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = (double)(0);
            }
            mincgcreate(n, &x, &state, _state);
            mincgsetcgtype(&state, cgtype, _state);
            
            /*
             * Test it with default preconditioner
             */
            mincgsetprecdefault(&state, _state);
            cntb1 = 0;
            for(pass=0; pass<=k-1; pass++)
            {
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                mincgrestartfrom(&state, &x, _state);
                while(mincgiteration(&state, _state))
                {
                    testmincgunit_calciip2(&state, n, _state);
                }
                mincgresults(&state, &x, &rep, _state);
                cntb1 = cntb1+rep.iterationscount;
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
            mincgsetprecdiag(&state, &diagh, _state);
            cntg1 = 0;
            for(pass=0; pass<=k-1; pass++)
            {
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                mincgrestartfrom(&state, &x, _state);
                while(mincgiteration(&state, _state))
                {
                    testmincgunit_calciip2(&state, n, _state);
                }
                mincgresults(&state, &x, &rep, _state);
                cntg1 = cntg1+rep.iterationscount;
                *err = *err||rep.terminationtype<=0;
            }
            
            /*
             * Compare
             */
            *err = *err||cntb1<cntg1;
        }
        
        /*
         * Preconditioner test 2.
         *
         * If
         * * B1 is default preconditioner
         * * G1 is low rank exact preconditioner
         * then "bad" preconditioner is worse than "good" one.
         * "Worse" means more iterations to converge.
         *
         * Target function is f(x) = 0.5*(x-x0)'*A*(x-x0), A = D+V'*Vd*V
         *
         * N        - problem size
         * K        - number of repeated passes (should be large enough to average out random factors)
         */
        for(n=10; n<=15; n++)
        {
            for(vs=0; vs<=5; vs++)
            {
                ae_vector_set_length(&x, n, _state);
                ae_vector_set_length(&x0, n, _state);
                ae_vector_set_length(&d, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = (double)(0);
                    x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    d.ptr.p_double[i] = ae_exp(2*ae_randomreal(_state), _state);
                }
                if( vs>0 )
                {
                    ae_matrix_set_length(&v, vs, n, _state);
                    ae_vector_set_length(&vd, vs, _state);
                    for(i=0; i<=vs-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            v.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                        vd.ptr.p_double[i] = ae_exp(2*ae_randomreal(_state), _state);
                    }
                }
                mincgcreate(n, &x, &state, _state);
                mincgsetcgtype(&state, cgtype, _state);
                
                /*
                 * Test it with default preconditioner
                 */
                mincgsetprecdefault(&state, _state);
                cntb1 = 0;
                for(pass=0; pass<=k-1; pass++)
                {
                    for(i=0; i<=n-1; i++)
                    {
                        x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    mincgrestartfrom(&state, &x, _state);
                    while(mincgiteration(&state, _state))
                    {
                        testmincgunit_calclowrank(&state, n, vs, &d, &v, &vd, &x0, _state);
                    }
                    mincgresults(&state, &x, &rep, _state);
                    cntb1 = cntb1+rep.iterationscount;
                    *err = *err||rep.terminationtype<=0;
                }
                
                /*
                 * Test it with low rank preconditioner
                 */
                mincgsetpreclowrankfast(&state, &d, &vd, &v, vs, _state);
                cntg1 = 0;
                for(pass=0; pass<=k-1; pass++)
                {
                    for(i=0; i<=n-1; i++)
                    {
                        x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    mincgrestartfrom(&state, &x, _state);
                    while(mincgiteration(&state, _state))
                    {
                        testmincgunit_calclowrank(&state, n, vs, &d, &v, &vd, &x0, _state);
                    }
                    mincgresults(&state, &x, &rep, _state);
                    cntg1 = cntg1+rep.iterationscount;
                    *err = *err||rep.terminationtype<=0;
                }
                
                /*
                 * Compare
                 */
                *err = *err||cntb1<cntg1;
            }
        }
        
        /*
         * Preconditioner test 3.
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
            mincgcreate(n, &x, &state, _state);
            ae_vector_set_length(&s, n, _state);
            for(i=0; i<=n-1; i++)
            {
                s.ptr.p_double[i] = 1/ae_sqrt(2*ae_pow((double)(i*i+1), (double)(2), _state)*(0.8+0.4*ae_randomreal(_state)), _state);
            }
            mincgsetprecdefault(&state, _state);
            mincgsetscale(&state, &s, _state);
            cntb2 = 0;
            for(pass=0; pass<=k-1; pass++)
            {
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                mincgrestartfrom(&state, &x, _state);
                while(mincgiteration(&state, _state))
                {
                    testmincgunit_calciip2(&state, n, _state);
                }
                mincgresults(&state, &x, &rep, _state);
                cntb2 = cntb2+rep.iterationscount;
                *err = *err||rep.terminationtype<=0;
            }
            mincgsetprecscale(&state, _state);
            mincgsetscale(&state, &s, _state);
            cntg2 = 0;
            for(pass=0; pass<=k-1; pass++)
            {
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                mincgrestartfrom(&state, &x, _state);
                while(mincgiteration(&state, _state))
                {
                    testmincgunit_calciip2(&state, n, _state);
                }
                mincgresults(&state, &x, &rep, _state);
                cntg2 = cntg2+rep.iterationscount;
                *err = *err||rep.terminationtype<=0;
            }
            *err = *err||cntb2<cntg2;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests, that gradient verified correctly.
*************************************************************************/
static ae_bool testmincgunit_gradientchecktest(ae_state *_state)
{
    ae_frame _frame_block;
    mincgstate state;
    mincgreport rep;
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
    _mincgstate_init(&state, _state);
    _mincgreport_init(&rep, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);

    passcount = 35;
    teststep = 0.01;
    n = 3;
    ae_vector_set_length(&x, n, _state);
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Prepare test's parameters
         */
        func = ae_randominteger(3, _state)+1;
        nbrcomp = ae_randominteger(n, _state);
        noise = (double)(2*ae_randominteger(2, _state)-1);
        
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
        mincgcreate(n, &x, &state, _state);
        mincgsetcond(&state, (double)(0), (double)(0), (double)(0), 0, _state);
        mincgsetgradientcheck(&state, teststep, _state);
        
        /*
         * Check that the criterion passes a derivative if it is correct
         */
        while(mincgiteration(&state, _state))
        {
            if( state.needfg )
            {
                testmincgunit_funcderiv(a, b, c, d, x0, x1, x2, &state.x, func, &state.f, &state.g, _state);
            }
        }
        mincgresults(&state, &x, &rep, _state);
        
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
        mincgrestartfrom(&state, &x, _state);
        
        /*
         * Check that the criterion does not miss a derivative if
         * it is incorrect
         */
        while(mincgiteration(&state, _state))
        {
            if( state.needfg )
            {
                testmincgunit_funcderiv(a, b, c, d, x0, x1, x2, &state.x, func, &state.f, &state.g, _state);
                state.g.ptr.p_double[nbrcomp] = state.g.ptr.p_double[nbrcomp]+noise;
            }
        }
        mincgresults(&state, &x, &rep, _state);
        
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
static void testmincgunit_funcderiv(double a,
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
