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
#include "testlsfitunit.h"


/*$ Declarations $*/
static void testlsfitunit_testpolynomialfitting(ae_bool* fiterrors,
     ae_state *_state);
static void testlsfitunit_testrationalfitting(ae_bool* fiterrors,
     ae_state *_state);
static void testlsfitunit_testsplinefitting(ae_bool* fiterrors,
     ae_state *_state);
static void testlsfitunit_testgeneralfitting(ae_bool* llserrors,
     ae_bool* nlserrors,
     ae_state *_state);
static void testlsfitunit_testrdp(ae_bool* errorflag, ae_state *_state);
static void testlsfitunit_testlogisticfitting(ae_bool* fiterrors,
     ae_state *_state);
static ae_bool testlsfitunit_isglssolution(ae_int_t n,
     ae_int_t m,
     ae_int_t k,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* fmatrix,
     /* Real    */ ae_matrix* cmatrix,
     /* Real    */ ae_vector* c,
     ae_state *_state);
static double testlsfitunit_getglserror(ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* fmatrix,
     /* Real    */ ae_vector* c,
     ae_state *_state);
static void testlsfitunit_fitlinearnonlinear(ae_int_t m,
     ae_int_t deravailable,
     /* Real    */ ae_matrix* xy,
     lsfitstate* state,
     ae_bool* nlserrors,
     ae_state *_state);
static void testlsfitunit_testgradientcheck(ae_bool* testg,
     ae_state *_state);
static void testlsfitunit_funcderiv(/* Real    */ ae_vector* c,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* x0,
     ae_int_t k,
     ae_int_t m,
     ae_int_t functype,
     double* f,
     /* Real    */ ae_vector* g,
     ae_state *_state);


/*$ Body $*/


ae_bool testlsfit(ae_bool silent, ae_state *_state)
{
    ae_bool waserrors;
    ae_bool llserrors;
    ae_bool nlserrors;
    ae_bool polfiterrors;
    ae_bool ratfiterrors;
    ae_bool splfiterrors;
    ae_bool graderrors;
    ae_bool logisticerrors;
    ae_bool rdperrors;
    ae_bool result;


    waserrors = ae_false;
    polfiterrors = ae_false;
    ratfiterrors = ae_false;
    splfiterrors = ae_false;
    llserrors = ae_false;
    nlserrors = ae_false;
    graderrors = ae_false;
    logisticerrors = ae_false;
    rdperrors = ae_false;
    testlsfitunit_testrdp(&rdperrors, _state);
    testlsfitunit_testlogisticfitting(&logisticerrors, _state);
    testlsfitunit_testpolynomialfitting(&polfiterrors, _state);
    testlsfitunit_testrationalfitting(&ratfiterrors, _state);
    testlsfitunit_testsplinefitting(&splfiterrors, _state);
    testlsfitunit_testgeneralfitting(&llserrors, &nlserrors, _state);
    testlsfitunit_testgradientcheck(&graderrors, _state);
    
    /*
     * report
     */
    waserrors = ((((((llserrors||nlserrors)||polfiterrors)||ratfiterrors)||splfiterrors)||graderrors)||logisticerrors)||rdperrors;
    if( !silent )
    {
        printf("TESTING LEAST SQUARES\n");
        printf("POLYNOMIAL LEAST SQUARES:                ");
        if( polfiterrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("RATIONAL LEAST SQUARES:                  ");
        if( ratfiterrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("SPLINE LEAST SQUARES:                    ");
        if( splfiterrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("LINEAR LEAST SQUARES:                    ");
        if( llserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("NON-LINEAR LEAST SQUARES:                ");
        if( nlserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("TEST FOR VERIFICATION OF THE GRADIENT:   ");
        if( graderrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("LOGISTIC FITTING (4PL/5PL):              ");
        if( logisticerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("RDP ALGORITHM:                           ");
        if( rdperrors )
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
    
    /*
     * end
     */
    result = !waserrors;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testlsfit(ae_bool silent, ae_state *_state)
{
    return testlsfit(silent, _state);
}


/*************************************************************************
Unit test
*************************************************************************/
static void testlsfitunit_testpolynomialfitting(ae_bool* fiterrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    double threshold;
    ae_vector x;
    ae_vector y;
    ae_vector w;
    ae_vector x2;
    ae_vector y2;
    ae_vector w2;
    ae_vector xfull;
    ae_vector yfull;
    double t;
    ae_int_t i;
    ae_int_t k;
    ae_vector xc;
    ae_vector yc;
    ae_vector dc;
    ae_int_t info;
    ae_int_t info2;
    double v;
    double v0;
    double v1;
    double v2;
    double s;
    double xmin;
    double xmax;
    double refrms;
    double refavg;
    double refavgrel;
    double refmax;
    barycentricinterpolant p;
    barycentricinterpolant p1;
    barycentricinterpolant p2;
    polynomialfitreport rep;
    polynomialfitreport rep2;
    ae_int_t n;
    ae_int_t m;
    ae_int_t maxn;
    ae_int_t pass;
    ae_int_t passcount;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&xfull, 0, DT_REAL, _state);
    ae_vector_init(&yfull, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&yc, 0, DT_REAL, _state);
    ae_vector_init(&dc, 0, DT_INT, _state);
    _barycentricinterpolant_init(&p, _state);
    _barycentricinterpolant_init(&p1, _state);
    _barycentricinterpolant_init(&p2, _state);
    _polynomialfitreport_init(&rep, _state);
    _polynomialfitreport_init(&rep2, _state);

    *fiterrors = ae_false;
    maxn = 5;
    passcount = 20;
    threshold = 1.0E8*ae_machineepsilon;
    
    /*
     * Test polunomial fitting
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            
            /*
             * N=M+K fitting (i.e. interpolation)
             */
            for(k=0; k<=n-1; k++)
            {
                taskgenint1d((double)(-1), (double)(1), n, &xfull, &yfull, _state);
                ae_vector_set_length(&x, n-k, _state);
                ae_vector_set_length(&y, n-k, _state);
                ae_vector_set_length(&w, n-k, _state);
                if( k>0 )
                {
                    ae_vector_set_length(&xc, k, _state);
                    ae_vector_set_length(&yc, k, _state);
                    ae_vector_set_length(&dc, k, _state);
                }
                for(i=0; i<=n-k-1; i++)
                {
                    x.ptr.p_double[i] = xfull.ptr.p_double[i];
                    y.ptr.p_double[i] = yfull.ptr.p_double[i];
                    w.ptr.p_double[i] = 1+ae_randomreal(_state);
                }
                for(i=0; i<=k-1; i++)
                {
                    xc.ptr.p_double[i] = xfull.ptr.p_double[n-k+i];
                    yc.ptr.p_double[i] = yfull.ptr.p_double[n-k+i];
                    dc.ptr.p_int[i] = 0;
                }
                polynomialfitwc(&x, &y, &w, n-k, &xc, &yc, &dc, k, n, &info, &p1, &rep, _state);
                if( info<=0 )
                {
                    *fiterrors = ae_true;
                }
                else
                {
                    for(i=0; i<=n-k-1; i++)
                    {
                        *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(barycentriccalc(&p1, x.ptr.p_double[i], _state)-y.ptr.p_double[i], _state),threshold);
                    }
                    for(i=0; i<=k-1; i++)
                    {
                        *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(barycentriccalc(&p1, xc.ptr.p_double[i], _state)-yc.ptr.p_double[i], _state),threshold);
                    }
                }
            }
            
            /*
             * Testing constraints on derivatives.
             * Special tasks which will always have solution:
             * 1. P(0)=YC[0]
             * 2. P(0)=YC[0], P'(0)=YC[1]
             */
            if( n>1 )
            {
                for(m=3; m<=5; m++)
                {
                    for(k=1; k<=2; k++)
                    {
                        taskgenint1d((double)(-1), (double)(1), n, &x, &y, _state);
                        ae_vector_set_length(&w, n, _state);
                        ae_vector_set_length(&xc, 2, _state);
                        ae_vector_set_length(&yc, 2, _state);
                        ae_vector_set_length(&dc, 2, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            w.ptr.p_double[i] = 1+ae_randomreal(_state);
                        }
                        xc.ptr.p_double[0] = (double)(0);
                        yc.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
                        dc.ptr.p_int[0] = 0;
                        xc.ptr.p_double[1] = (double)(0);
                        yc.ptr.p_double[1] = 2*ae_randomreal(_state)-1;
                        dc.ptr.p_int[1] = 1;
                        polynomialfitwc(&x, &y, &w, n, &xc, &yc, &dc, k, m, &info, &p1, &rep, _state);
                        if( info<=0 )
                        {
                            *fiterrors = ae_true;
                        }
                        else
                        {
                            barycentricdiff1(&p1, 0.0, &v0, &v1, _state);
                            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(v0-yc.ptr.p_double[0], _state),threshold);
                            if( k==2 )
                            {
                                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(v1-yc.ptr.p_double[1], _state),threshold);
                            }
                        }
                    }
                }
            }
        }
    }
    for(m=2; m<=8; m++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * General fitting
             *
             * interpolating function through M nodes should have
             * greater RMS error than fitting it through the same M nodes
             */
            n = 100;
            ae_vector_set_length(&x2, n, _state);
            ae_vector_set_length(&y2, n, _state);
            ae_vector_set_length(&w2, n, _state);
            xmin = (double)(0);
            xmax = 2*ae_pi;
            for(i=0; i<=n-1; i++)
            {
                x2.ptr.p_double[i] = 2*ae_pi*ae_randomreal(_state);
                y2.ptr.p_double[i] = ae_sin(x2.ptr.p_double[i], _state);
                w2.ptr.p_double[i] = (double)(1);
            }
            ae_vector_set_length(&x, m, _state);
            ae_vector_set_length(&y, m, _state);
            for(i=0; i<=m-1; i++)
            {
                x.ptr.p_double[i] = xmin+(xmax-xmin)*i/(m-1);
                y.ptr.p_double[i] = ae_sin(x.ptr.p_double[i], _state);
            }
            polynomialbuild(&x, &y, m, &p1, _state);
            polynomialfitwc(&x2, &y2, &w2, n, &xc, &yc, &dc, 0, m, &info, &p2, &rep, _state);
            if( info<=0 )
            {
                *fiterrors = ae_true;
            }
            else
            {
                
                /*
                 * calculate P1 (interpolant) RMS error, compare with P2 error
                 */
                v1 = (double)(0);
                v2 = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    v1 = v1+ae_sqr(barycentriccalc(&p1, x2.ptr.p_double[i], _state)-y2.ptr.p_double[i], _state);
                    v2 = v2+ae_sqr(barycentriccalc(&p2, x2.ptr.p_double[i], _state)-y2.ptr.p_double[i], _state);
                }
                v1 = ae_sqrt(v1/n, _state);
                v2 = ae_sqrt(v2/n, _state);
                *fiterrors = *fiterrors||ae_fp_greater(v2,v1);
                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(v2-rep.rmserror, _state),threshold);
            }
            
            /*
             * compare weighted and non-weighted
             */
            n = 20;
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&y, n, _state);
            ae_vector_set_length(&w, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = i+(ae_randomreal(_state)-0.5);
                y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                w.ptr.p_double[i] = (double)(1);
            }
            polynomialfitwc(&x, &y, &w, n, &xc, &yc, &dc, 0, m, &info, &p1, &rep, _state);
            polynomialfit(&x, &y, n, m, &info2, &p2, &rep2, _state);
            if( info<=0||info2<=0 )
            {
                *fiterrors = ae_true;
            }
            else
            {
                
                /*
                 * calculate P1 (interpolant), compare with P2 error
                 * compare RMS errors
                 */
                t = 2*ae_randomreal(_state)-1;
                v1 = barycentriccalc(&p1, t, _state);
                v2 = barycentriccalc(&p2, t, _state);
                *fiterrors = *fiterrors||!approxequalrel(v2, v1, threshold, _state);
                *fiterrors = *fiterrors||!approxequalrel(rep.rmserror, rep2.rmserror, threshold, _state);
                *fiterrors = *fiterrors||!approxequalrel(rep.avgerror, rep2.avgerror, threshold, _state);
                *fiterrors = *fiterrors||!approxequalrel(rep.avgrelerror, rep2.avgrelerror, threshold, _state);
                *fiterrors = *fiterrors||!approxequalrel(rep.maxerror, rep2.maxerror, threshold, _state);
            }
        }
    }
    for(m=1; m<=maxn; m++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            ae_assert(passcount>=2, "PassCount should be 2 or greater!", _state);
            
            /*
             * solve simple task (all X[] are the same, Y[] are specially
             * calculated to ensure simple form of all types of errors)
             * and check correctness of the errors calculated by subroutines
             *
             * First pass is done with zero Y[], other passes - with random Y[].
             * It should test both ability to correctly calculate errors and
             * ability to not fail while working with zeros :)
             */
            n = 4*maxn;
            if( pass==1 )
            {
                v1 = (double)(0);
                v2 = (double)(0);
                v = (double)(0);
            }
            else
            {
                v1 = ae_randomreal(_state);
                v2 = ae_randomreal(_state);
                v = 1+ae_randomreal(_state);
            }
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&y, n, _state);
            ae_vector_set_length(&w, n, _state);
            for(i=0; i<=maxn-1; i++)
            {
                x.ptr.p_double[4*i+0] = (double)(i);
                y.ptr.p_double[4*i+0] = v-v2;
                w.ptr.p_double[4*i+0] = (double)(1);
                x.ptr.p_double[4*i+1] = (double)(i);
                y.ptr.p_double[4*i+1] = v-v1;
                w.ptr.p_double[4*i+1] = (double)(1);
                x.ptr.p_double[4*i+2] = (double)(i);
                y.ptr.p_double[4*i+2] = v+v1;
                w.ptr.p_double[4*i+2] = (double)(1);
                x.ptr.p_double[4*i+3] = (double)(i);
                y.ptr.p_double[4*i+3] = v+v2;
                w.ptr.p_double[4*i+3] = (double)(1);
            }
            refrms = ae_sqrt((ae_sqr(v1, _state)+ae_sqr(v2, _state))/2, _state);
            refavg = (ae_fabs(v1, _state)+ae_fabs(v2, _state))/2;
            if( pass==1 )
            {
                refavgrel = (double)(0);
            }
            else
            {
                refavgrel = 0.25*(ae_fabs(v2, _state)/ae_fabs(v-v2, _state)+ae_fabs(v1, _state)/ae_fabs(v-v1, _state)+ae_fabs(v1, _state)/ae_fabs(v+v1, _state)+ae_fabs(v2, _state)/ae_fabs(v+v2, _state));
            }
            refmax = ae_maxreal(v1, v2, _state);
            
            /*
             * Test errors correctness
             */
            polynomialfit(&x, &y, n, m, &info, &p, &rep, _state);
            if( info<=0 )
            {
                *fiterrors = ae_true;
            }
            else
            {
                s = barycentriccalc(&p, (double)(0), _state);
                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(s-v, _state),threshold);
                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.rmserror-refrms, _state),threshold);
                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.avgerror-refavg, _state),threshold);
                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.avgrelerror-refavgrel, _state),threshold);
                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.maxerror-refmax, _state),threshold);
            }
        }
    }
    ae_frame_leave(_state);
}


static void testlsfitunit_testrationalfitting(ae_bool* fiterrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    double threshold;
    ae_int_t maxn;
    ae_int_t passcount;
    barycentricinterpolant b1;
    barycentricinterpolant b2;
    ae_vector x;
    ae_vector x2;
    ae_vector y;
    ae_vector y2;
    ae_vector w;
    ae_vector w2;
    ae_vector xc;
    ae_vector yc;
    ae_vector dc;
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    ae_int_t k;
    ae_int_t pass;
    double t;
    double s;
    double v;
    double v0;
    double v1;
    double v2;
    ae_int_t info;
    ae_int_t info2;
    double xmin;
    double xmax;
    double refrms;
    double refavg;
    double refavgrel;
    double refmax;
    barycentricfitreport rep;
    barycentricfitreport rep2;

    ae_frame_make(_state, &_frame_block);
    _barycentricinterpolant_init(&b1, _state);
    _barycentricinterpolant_init(&b2, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&yc, 0, DT_REAL, _state);
    ae_vector_init(&dc, 0, DT_INT, _state);
    _barycentricfitreport_init(&rep, _state);
    _barycentricfitreport_init(&rep2, _state);

    *fiterrors = ae_false;
    
    /*
     * PassCount        number of repeated passes
     * Threshold        error tolerance
     * LipschitzTol     Lipschitz constant increase allowed
     *                  when calculating constant on a twice denser grid
     */
    passcount = 5;
    maxn = 15;
    threshold = 1000000*ae_machineepsilon;
    
    /*
     * Test rational fitting:
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=2; n<=maxn; n++)
        {
            
            /*
             * N=M+K fitting (i.e. interpolation)
             */
            for(k=0; k<=n-1; k++)
            {
                ae_vector_set_length(&x, n-k, _state);
                ae_vector_set_length(&y, n-k, _state);
                ae_vector_set_length(&w, n-k, _state);
                if( k>0 )
                {
                    ae_vector_set_length(&xc, k, _state);
                    ae_vector_set_length(&yc, k, _state);
                    ae_vector_set_length(&dc, k, _state);
                }
                for(i=0; i<=n-k-1; i++)
                {
                    x.ptr.p_double[i] = (double)i/(double)(n-1);
                    y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    w.ptr.p_double[i] = 1+ae_randomreal(_state);
                }
                for(i=0; i<=k-1; i++)
                {
                    xc.ptr.p_double[i] = (double)(n-k+i)/(double)(n-1);
                    yc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    dc.ptr.p_int[i] = 0;
                }
                barycentricfitfloaterhormannwc(&x, &y, &w, n-k, &xc, &yc, &dc, k, n, &info, &b1, &rep, _state);
                if( info<=0 )
                {
                    *fiterrors = ae_true;
                }
                else
                {
                    for(i=0; i<=n-k-1; i++)
                    {
                        *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(barycentriccalc(&b1, x.ptr.p_double[i], _state)-y.ptr.p_double[i], _state),threshold);
                    }
                    for(i=0; i<=k-1; i++)
                    {
                        *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(barycentriccalc(&b1, xc.ptr.p_double[i], _state)-yc.ptr.p_double[i], _state),threshold);
                    }
                }
            }
            
            /*
             * Testing constraints on derivatives:
             * * several M's are tried
             * * several K's are tried - 1, 2.
             * * constraints at the ends of the interval
             */
            for(m=3; m<=5; m++)
            {
                for(k=1; k<=2; k++)
                {
                    ae_vector_set_length(&x, n, _state);
                    ae_vector_set_length(&y, n, _state);
                    ae_vector_set_length(&w, n, _state);
                    ae_vector_set_length(&xc, 2, _state);
                    ae_vector_set_length(&yc, 2, _state);
                    ae_vector_set_length(&dc, 2, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        w.ptr.p_double[i] = 1+ae_randomreal(_state);
                    }
                    xc.ptr.p_double[0] = (double)(-1);
                    yc.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
                    dc.ptr.p_int[0] = 0;
                    xc.ptr.p_double[1] = (double)(1);
                    yc.ptr.p_double[1] = 2*ae_randomreal(_state)-1;
                    dc.ptr.p_int[1] = 0;
                    barycentricfitfloaterhormannwc(&x, &y, &w, n, &xc, &yc, &dc, k, m, &info, &b1, &rep, _state);
                    if( info<=0 )
                    {
                        *fiterrors = ae_true;
                    }
                    else
                    {
                        for(i=0; i<=k-1; i++)
                        {
                            barycentricdiff1(&b1, xc.ptr.p_double[i], &v0, &v1, _state);
                            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(v0-yc.ptr.p_double[i], _state),threshold);
                        }
                    }
                }
            }
        }
    }
    for(m=2; m<=8; m++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * General fitting
             *
             * interpolating function through M nodes should have
             * greater RMS error than fitting it through the same M nodes
             */
            n = 100;
            ae_vector_set_length(&x2, n, _state);
            ae_vector_set_length(&y2, n, _state);
            ae_vector_set_length(&w2, n, _state);
            xmin = ae_maxrealnumber;
            xmax = -ae_maxrealnumber;
            for(i=0; i<=n-1; i++)
            {
                x2.ptr.p_double[i] = 2*ae_pi*ae_randomreal(_state);
                y2.ptr.p_double[i] = ae_sin(x2.ptr.p_double[i], _state);
                w2.ptr.p_double[i] = (double)(1);
                xmin = ae_minreal(xmin, x2.ptr.p_double[i], _state);
                xmax = ae_maxreal(xmax, x2.ptr.p_double[i], _state);
            }
            ae_vector_set_length(&x, m, _state);
            ae_vector_set_length(&y, m, _state);
            for(i=0; i<=m-1; i++)
            {
                x.ptr.p_double[i] = xmin+(xmax-xmin)*i/(m-1);
                y.ptr.p_double[i] = ae_sin(x.ptr.p_double[i], _state);
            }
            barycentricbuildfloaterhormann(&x, &y, m, 3, &b1, _state);
            barycentricfitfloaterhormannwc(&x2, &y2, &w2, n, &xc, &yc, &dc, 0, m, &info, &b2, &rep, _state);
            if( info<=0 )
            {
                *fiterrors = ae_true;
            }
            else
            {
                
                /*
                 * calculate B1 (interpolant) RMS error, compare with B2 error
                 */
                v1 = (double)(0);
                v2 = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    v1 = v1+ae_sqr(barycentriccalc(&b1, x2.ptr.p_double[i], _state)-y2.ptr.p_double[i], _state);
                    v2 = v2+ae_sqr(barycentriccalc(&b2, x2.ptr.p_double[i], _state)-y2.ptr.p_double[i], _state);
                }
                v1 = ae_sqrt(v1/n, _state);
                v2 = ae_sqrt(v2/n, _state);
                *fiterrors = *fiterrors||ae_fp_greater(v2,v1);
                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(v2-rep.rmserror, _state),threshold);
            }
            
            /*
             * compare weighted and non-weighted
             */
            n = 20;
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&y, n, _state);
            ae_vector_set_length(&w, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = i+(ae_randomreal(_state)-0.5);
                y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                w.ptr.p_double[i] = (double)(1);
            }
            barycentricfitfloaterhormannwc(&x, &y, &w, n, &xc, &yc, &dc, 0, m, &info, &b1, &rep, _state);
            barycentricfitfloaterhormann(&x, &y, n, m, &info2, &b2, &rep2, _state);
            if( info<=0||info2<=0 )
            {
                *fiterrors = ae_true;
            }
            else
            {
                
                /*
                 * calculate B1 (interpolant), compare with B2
                 * compare RMS errors
                 */
                t = 2*ae_randomreal(_state)-1;
                v1 = barycentriccalc(&b1, t, _state);
                v2 = barycentriccalc(&b2, t, _state);
                *fiterrors = *fiterrors||!approxequalrel(v2, v1, threshold, _state);
                *fiterrors = *fiterrors||!approxequalrel(rep.rmserror, rep2.rmserror, threshold, _state);
                *fiterrors = *fiterrors||!approxequalrel(rep.avgerror, rep2.avgerror, threshold, _state);
                *fiterrors = *fiterrors||!approxequalrel(rep.avgrelerror, rep2.avgrelerror, threshold, _state);
                *fiterrors = *fiterrors||!approxequalrel(rep.maxerror, rep2.maxerror, threshold, _state);
            }
        }
    }
    for(pass=1; pass<=passcount; pass++)
    {
        ae_assert(passcount>=2, "PassCount should be 2 or greater!", _state);
        
        /*
         * solve simple task (all X[] are the same, Y[] are specially
         * calculated to ensure simple form of all types of errors)
         * and check correctness of the errors calculated by subroutines
         *
         * First pass is done with zero Y[], other passes - with random Y[].
         * It should test both ability to correctly calculate errors and
         * ability to not fail while working with zeros :)
         */
        n = 4;
        if( pass==1 )
        {
            v1 = (double)(0);
            v2 = (double)(0);
            v = (double)(0);
        }
        else
        {
            v1 = ae_randomreal(_state);
            v2 = ae_randomreal(_state);
            v = 1+ae_randomreal(_state);
        }
        ae_vector_set_length(&x, 4, _state);
        ae_vector_set_length(&y, 4, _state);
        ae_vector_set_length(&w, 4, _state);
        x.ptr.p_double[0] = (double)(0);
        y.ptr.p_double[0] = v-v2;
        w.ptr.p_double[0] = (double)(1);
        x.ptr.p_double[1] = (double)(0);
        y.ptr.p_double[1] = v-v1;
        w.ptr.p_double[1] = (double)(1);
        x.ptr.p_double[2] = (double)(0);
        y.ptr.p_double[2] = v+v1;
        w.ptr.p_double[2] = (double)(1);
        x.ptr.p_double[3] = (double)(0);
        y.ptr.p_double[3] = v+v2;
        w.ptr.p_double[3] = (double)(1);
        refrms = ae_sqrt((ae_sqr(v1, _state)+ae_sqr(v2, _state))/2, _state);
        refavg = (ae_fabs(v1, _state)+ae_fabs(v2, _state))/2;
        if( pass==1 )
        {
            refavgrel = (double)(0);
        }
        else
        {
            refavgrel = 0.25*(ae_fabs(v2, _state)/ae_fabs(v-v2, _state)+ae_fabs(v1, _state)/ae_fabs(v-v1, _state)+ae_fabs(v1, _state)/ae_fabs(v+v1, _state)+ae_fabs(v2, _state)/ae_fabs(v+v2, _state));
        }
        refmax = ae_maxreal(v1, v2, _state);
        
        /*
         * Test errors correctness
         */
        barycentricfitfloaterhormann(&x, &y, 4, 2, &info, &b1, &rep, _state);
        if( info<=0 )
        {
            *fiterrors = ae_true;
        }
        else
        {
            s = barycentriccalc(&b1, (double)(0), _state);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(s-v, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.rmserror-refrms, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.avgerror-refavg, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.avgrelerror-refavgrel, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.maxerror-refmax, _state),threshold);
        }
    }
    ae_frame_leave(_state);
}


static void testlsfitunit_testsplinefitting(ae_bool* fiterrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    double threshold;
    double nonstrictthreshold;
    ae_int_t passcount;
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    ae_int_t k;
    ae_int_t pass;
    ae_vector x;
    ae_vector y;
    ae_vector w;
    ae_vector w2;
    ae_vector xc;
    ae_vector yc;
    ae_vector d;
    ae_vector dc;
    double sa;
    double sb;
    ae_int_t info;
    ae_int_t info1;
    ae_int_t info2;
    spline1dinterpolant c;
    spline1dinterpolant c2;
    spline1dfitreport rep;
    spline1dfitreport rep2;
    double s;
    double ds;
    double d2s;
    ae_int_t stype;
    double t;
    double v;
    double v1;
    double v2;
    double refrms;
    double refavg;
    double refavgrel;
    double refmax;
    double rho;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&yc, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&dc, 0, DT_INT, _state);
    _spline1dinterpolant_init(&c, _state);
    _spline1dinterpolant_init(&c2, _state);
    _spline1dfitreport_init(&rep, _state);
    _spline1dfitreport_init(&rep2, _state);

    
    /*
     * Valyes:
     * * pass count
     * * threshold - for tests which must be satisfied exactly
     * * nonstrictthreshold - for approximate tests
     */
    passcount = 20;
    threshold = 10000*ae_machineepsilon;
    nonstrictthreshold = 1.0E-4;
    *fiterrors = ae_false;
    
    /*
     * Test fitting by Cubic and Hermite splines (obsolete, but still supported)
     */
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Cubic splines
         * Ability to handle boundary constraints (1-4 constraints on F, dF/dx).
         */
        for(m=4; m<=8; m++)
        {
            for(k=1; k<=4; k++)
            {
                if( k>=m )
                {
                    continue;
                }
                n = 100;
                ae_vector_set_length(&x, n, _state);
                ae_vector_set_length(&y, n, _state);
                ae_vector_set_length(&w, n, _state);
                ae_vector_set_length(&xc, 4, _state);
                ae_vector_set_length(&yc, 4, _state);
                ae_vector_set_length(&dc, 4, _state);
                sa = 1+ae_randomreal(_state);
                sb = 2*ae_randomreal(_state)-1;
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = sa*ae_randomreal(_state)+sb;
                    y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    w.ptr.p_double[i] = 1+ae_randomreal(_state);
                }
                xc.ptr.p_double[0] = sb;
                yc.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
                dc.ptr.p_int[0] = 0;
                xc.ptr.p_double[1] = sb;
                yc.ptr.p_double[1] = 2*ae_randomreal(_state)-1;
                dc.ptr.p_int[1] = 1;
                xc.ptr.p_double[2] = sa+sb;
                yc.ptr.p_double[2] = 2*ae_randomreal(_state)-1;
                dc.ptr.p_int[2] = 0;
                xc.ptr.p_double[3] = sa+sb;
                yc.ptr.p_double[3] = 2*ae_randomreal(_state)-1;
                dc.ptr.p_int[3] = 1;
                spline1dfitcubicwc(&x, &y, &w, n, &xc, &yc, &dc, k, m, &info, &c, &rep, _state);
                if( info<=0 )
                {
                    *fiterrors = ae_true;
                }
                else
                {
                    
                    /*
                     * Check that constraints are satisfied
                     */
                    for(i=0; i<=k-1; i++)
                    {
                        spline1ddiff(&c, xc.ptr.p_double[i], &s, &ds, &d2s, _state);
                        if( dc.ptr.p_int[i]==0 )
                        {
                            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(s-yc.ptr.p_double[i], _state),threshold);
                        }
                        if( dc.ptr.p_int[i]==1 )
                        {
                            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(ds-yc.ptr.p_double[i], _state),threshold);
                        }
                        if( dc.ptr.p_int[i]==2 )
                        {
                            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(d2s-yc.ptr.p_double[i], _state),threshold);
                        }
                    }
                }
            }
        }
        
        /*
         * Cubic splines
         * Ability to handle one internal constraint
         */
        for(m=4; m<=8; m++)
        {
            n = 100;
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&y, n, _state);
            ae_vector_set_length(&w, n, _state);
            ae_vector_set_length(&xc, 1, _state);
            ae_vector_set_length(&yc, 1, _state);
            ae_vector_set_length(&dc, 1, _state);
            sa = 1+ae_randomreal(_state);
            sb = 2*ae_randomreal(_state)-1;
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = sa*ae_randomreal(_state)+sb;
                y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                w.ptr.p_double[i] = 1+ae_randomreal(_state);
            }
            xc.ptr.p_double[0] = sa*ae_randomreal(_state)+sb;
            yc.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
            dc.ptr.p_int[0] = ae_randominteger(2, _state);
            spline1dfitcubicwc(&x, &y, &w, n, &xc, &yc, &dc, 1, m, &info, &c, &rep, _state);
            if( info<=0 )
            {
                *fiterrors = ae_true;
            }
            else
            {
                
                /*
                 * Check that constraints are satisfied
                 */
                spline1ddiff(&c, xc.ptr.p_double[0], &s, &ds, &d2s, _state);
                if( dc.ptr.p_int[0]==0 )
                {
                    *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(s-yc.ptr.p_double[0], _state),threshold);
                }
                if( dc.ptr.p_int[0]==1 )
                {
                    *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(ds-yc.ptr.p_double[0], _state),threshold);
                }
                if( dc.ptr.p_int[0]==2 )
                {
                    *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(d2s-yc.ptr.p_double[0], _state),threshold);
                }
            }
        }
        
        /*
         * Hermite splines
         * Ability to handle boundary constraints (1-4 constraints on F, dF/dx).
         */
        for(m=4; m<=8; m++)
        {
            for(k=1; k<=4; k++)
            {
                if( k>=m )
                {
                    continue;
                }
                if( m%2!=0 )
                {
                    continue;
                }
                n = 100;
                ae_vector_set_length(&x, n, _state);
                ae_vector_set_length(&y, n, _state);
                ae_vector_set_length(&w, n, _state);
                ae_vector_set_length(&xc, 4, _state);
                ae_vector_set_length(&yc, 4, _state);
                ae_vector_set_length(&dc, 4, _state);
                sa = 1+ae_randomreal(_state);
                sb = 2*ae_randomreal(_state)-1;
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = sa*ae_randomreal(_state)+sb;
                    y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    w.ptr.p_double[i] = 1+ae_randomreal(_state);
                }
                xc.ptr.p_double[0] = sb;
                yc.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
                dc.ptr.p_int[0] = 0;
                xc.ptr.p_double[1] = sb;
                yc.ptr.p_double[1] = 2*ae_randomreal(_state)-1;
                dc.ptr.p_int[1] = 1;
                xc.ptr.p_double[2] = sa+sb;
                yc.ptr.p_double[2] = 2*ae_randomreal(_state)-1;
                dc.ptr.p_int[2] = 0;
                xc.ptr.p_double[3] = sa+sb;
                yc.ptr.p_double[3] = 2*ae_randomreal(_state)-1;
                dc.ptr.p_int[3] = 1;
                spline1dfithermitewc(&x, &y, &w, n, &xc, &yc, &dc, k, m, &info, &c, &rep, _state);
                if( info<=0 )
                {
                    *fiterrors = ae_true;
                }
                else
                {
                    
                    /*
                     * Check that constraints are satisfied
                     */
                    for(i=0; i<=k-1; i++)
                    {
                        spline1ddiff(&c, xc.ptr.p_double[i], &s, &ds, &d2s, _state);
                        if( dc.ptr.p_int[i]==0 )
                        {
                            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(s-yc.ptr.p_double[i], _state),threshold);
                        }
                        if( dc.ptr.p_int[i]==1 )
                        {
                            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(ds-yc.ptr.p_double[i], _state),threshold);
                        }
                        if( dc.ptr.p_int[i]==2 )
                        {
                            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(d2s-yc.ptr.p_double[i], _state),threshold);
                        }
                    }
                }
            }
        }
        
        /*
         * Hermite splines
         * Ability to handle one internal constraint
         */
        for(m=4; m<=8; m++)
        {
            if( m%2!=0 )
            {
                continue;
            }
            n = 100;
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&y, n, _state);
            ae_vector_set_length(&w, n, _state);
            ae_vector_set_length(&xc, 1, _state);
            ae_vector_set_length(&yc, 1, _state);
            ae_vector_set_length(&dc, 1, _state);
            sa = 1+ae_randomreal(_state);
            sb = 2*ae_randomreal(_state)-1;
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = sa*ae_randomreal(_state)+sb;
                y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                w.ptr.p_double[i] = 1+ae_randomreal(_state);
            }
            xc.ptr.p_double[0] = sa*ae_randomreal(_state)+sb;
            yc.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
            dc.ptr.p_int[0] = ae_randominteger(2, _state);
            spline1dfithermitewc(&x, &y, &w, n, &xc, &yc, &dc, 1, m, &info, &c, &rep, _state);
            if( info<=0 )
            {
                *fiterrors = ae_true;
            }
            else
            {
                
                /*
                 * Check that constraints are satisfied
                 */
                spline1ddiff(&c, xc.ptr.p_double[0], &s, &ds, &d2s, _state);
                if( dc.ptr.p_int[0]==0 )
                {
                    *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(s-yc.ptr.p_double[0], _state),threshold);
                }
                if( dc.ptr.p_int[0]==1 )
                {
                    *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(ds-yc.ptr.p_double[0], _state),threshold);
                }
                if( dc.ptr.p_int[0]==2 )
                {
                    *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(d2s-yc.ptr.p_double[0], _state),threshold);
                }
            }
        }
    }
    for(m=4; m<=8; m++)
    {
        for(stype=0; stype<=1; stype++)
        {
            for(pass=1; pass<=passcount; pass++)
            {
                if( stype==1&&m%2!=0 )
                {
                    continue;
                }
                
                /*
                 * cubic/Hermite spline fitting:
                 * * generate "template spline" C2
                 * * generate 2*N points from C2, such that result of
                 *   ideal fit should be equal to C2
                 * * fit, store in C
                 * * compare C and C2
                 */
                sa = 1+ae_randomreal(_state);
                sb = 2*ae_randomreal(_state)-1;
                if( stype==0 )
                {
                    ae_vector_set_length(&x, m-2, _state);
                    ae_vector_set_length(&y, m-2, _state);
                    for(i=0; i<=m-2-1; i++)
                    {
                        x.ptr.p_double[i] = sa*i/(m-2-1)+sb;
                        y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    spline1dbuildcubic(&x, &y, m-2, 1, 2*ae_randomreal(_state)-1, 1, 2*ae_randomreal(_state)-1, &c2, _state);
                }
                if( stype==1 )
                {
                    ae_vector_set_length(&x, m/2, _state);
                    ae_vector_set_length(&y, m/2, _state);
                    ae_vector_set_length(&d, m/2, _state);
                    for(i=0; i<=m/2-1; i++)
                    {
                        x.ptr.p_double[i] = sa*i/(m/2-1)+sb;
                        y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        d.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    spline1dbuildhermite(&x, &y, &d, m/2, &c2, _state);
                }
                n = 50;
                ae_vector_set_length(&x, 2*n, _state);
                ae_vector_set_length(&y, 2*n, _state);
                ae_vector_set_length(&w, 2*n, _state);
                for(i=0; i<=n-1; i++)
                {
                    
                    /*
                     * "if i=0" and "if i=1" are needed to
                     * synchronize interval size for C2 and
                     * spline being fitted (i.e. C).
                     */
                    t = ae_randomreal(_state);
                    x.ptr.p_double[i] = sa*ae_randomreal(_state)+sb;
                    if( i==0 )
                    {
                        x.ptr.p_double[i] = sb;
                    }
                    if( i==1 )
                    {
                        x.ptr.p_double[i] = sa+sb;
                    }
                    v = spline1dcalc(&c2, x.ptr.p_double[i], _state);
                    y.ptr.p_double[i] = v+t;
                    w.ptr.p_double[i] = 1+ae_randomreal(_state);
                    x.ptr.p_double[n+i] = x.ptr.p_double[i];
                    y.ptr.p_double[n+i] = v-t;
                    w.ptr.p_double[n+i] = w.ptr.p_double[i];
                }
                info = -1;
                if( stype==0 )
                {
                    spline1dfitcubicwc(&x, &y, &w, 2*n, &xc, &yc, &dc, 0, m, &info, &c, &rep, _state);
                }
                if( stype==1 )
                {
                    spline1dfithermitewc(&x, &y, &w, 2*n, &xc, &yc, &dc, 0, m, &info, &c, &rep, _state);
                }
                if( info<=0 )
                {
                    *fiterrors = ae_true;
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        v = sa*ae_randomreal(_state)+sb;
                        *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(spline1dcalc(&c, v, _state)-spline1dcalc(&c2, v, _state), _state),threshold);
                    }
                }
            }
        }
    }
    for(m=4; m<=8; m++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * prepare points/weights
             */
            n = 10+ae_randominteger(10, _state);
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&y, n, _state);
            ae_vector_set_length(&w, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = i+(ae_randomreal(_state)-0.5);
                y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                w.ptr.p_double[i] = (double)(1);
            }
            
            /*
             * Fit cubic with unity weights, without weights, then compare
             */
            if( m>=4 )
            {
                spline1dfitcubicwc(&x, &y, &w, n, &xc, &yc, &dc, 0, m, &info1, &c, &rep, _state);
                spline1dfitcubic(&x, &y, n, m, &info2, &c2, &rep2, _state);
                if( info1<=0||info2<=0 )
                {
                    *fiterrors = ae_true;
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        v = ae_randomreal(_state)*(n-1);
                        *fiterrors = *fiterrors||!approxequalrel(spline1dcalc(&c, v, _state), spline1dcalc(&c2, v, _state), threshold, _state);
                        *fiterrors = *fiterrors||!approxequalrel(rep.taskrcond, rep2.taskrcond, threshold, _state);
                        *fiterrors = *fiterrors||!approxequalrel(rep.rmserror, rep2.rmserror, threshold, _state);
                        *fiterrors = *fiterrors||!approxequalrel(rep.avgerror, rep2.avgerror, threshold, _state);
                        *fiterrors = *fiterrors||!approxequalrel(rep.avgrelerror, rep2.avgrelerror, threshold, _state);
                        *fiterrors = *fiterrors||!approxequalrel(rep.maxerror, rep2.maxerror, threshold, _state);
                    }
                }
            }
            
            /*
             * Fit Hermite with unity weights, without weights, then compare
             */
            if( m>=4&&m%2==0 )
            {
                spline1dfithermitewc(&x, &y, &w, n, &xc, &yc, &dc, 0, m, &info1, &c, &rep, _state);
                spline1dfithermite(&x, &y, n, m, &info2, &c2, &rep2, _state);
                if( info1<=0||info2<=0 )
                {
                    *fiterrors = ae_true;
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        v = ae_randomreal(_state)*(n-1);
                        *fiterrors = *fiterrors||!approxequalrel(spline1dcalc(&c, v, _state), spline1dcalc(&c2, v, _state), threshold, _state);
                        *fiterrors = *fiterrors||!approxequalrel(rep.taskrcond, rep2.taskrcond, threshold, _state);
                        *fiterrors = *fiterrors||!approxequalrel(rep.rmserror, rep2.rmserror, threshold, _state);
                        *fiterrors = *fiterrors||!approxequalrel(rep.avgerror, rep2.avgerror, threshold, _state);
                        *fiterrors = *fiterrors||!approxequalrel(rep.avgrelerror, rep2.avgrelerror, threshold, _state);
                        *fiterrors = *fiterrors||!approxequalrel(rep.maxerror, rep2.maxerror, threshold, _state);
                    }
                }
            }
        }
    }
    
    /*
     * check basic properties of penalized splines which are
     * preserved independently of Rho parameter.
     */
    for(m=4; m<=10; m++)
    {
        for(k=-5; k<=5; k++)
        {
            rho = (double)(k);
            
            /*
             * when we have two points (even with different weights),
             * resulting spline must be equal to the straight line
             */
            ae_vector_set_length(&x, 2, _state);
            ae_vector_set_length(&y, 2, _state);
            ae_vector_set_length(&w, 2, _state);
            x.ptr.p_double[0] = -0.5-ae_randomreal(_state);
            y.ptr.p_double[0] = 0.5+ae_randomreal(_state);
            w.ptr.p_double[0] = 1+ae_randomreal(_state);
            x.ptr.p_double[1] = 0.5+ae_randomreal(_state);
            y.ptr.p_double[1] = 0.5+ae_randomreal(_state);
            w.ptr.p_double[1] = 1+ae_randomreal(_state);
            spline1dfitpenalized(&x, &y, 2, m, rho, &info, &c, &rep, _state);
            if( info>0 )
            {
                v = 2*ae_randomreal(_state)-1;
                v1 = (v-x.ptr.p_double[0])/(x.ptr.p_double[1]-x.ptr.p_double[0])*y.ptr.p_double[1]+(v-x.ptr.p_double[1])/(x.ptr.p_double[0]-x.ptr.p_double[1])*y.ptr.p_double[0];
                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(v1-spline1dcalc(&c, v, _state), _state),nonstrictthreshold);
            }
            else
            {
                *fiterrors = ae_true;
            }
            spline1dfitpenalizedw(&x, &y, &w, 2, m, rho, &info, &c, &rep, _state);
            if( info>0 )
            {
                v = 2*ae_randomreal(_state)-1;
                v1 = (v-x.ptr.p_double[0])/(x.ptr.p_double[1]-x.ptr.p_double[0])*y.ptr.p_double[1]+(v-x.ptr.p_double[1])/(x.ptr.p_double[0]-x.ptr.p_double[1])*y.ptr.p_double[0];
                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(v1-spline1dcalc(&c, v, _state), _state),nonstrictthreshold);
            }
            else
            {
                *fiterrors = ae_true;
            }
            
            /*
             * spline fitting is invariant with respect to
             * scaling of weights (of course, ANY fitting algorithm
             * must be invariant, but we want to test this property
             * just to be sure that it is correctly implemented)
             */
            for(n=2; n<=2*m; n++)
            {
                ae_vector_set_length(&x, n, _state);
                ae_vector_set_length(&y, n, _state);
                ae_vector_set_length(&w, n, _state);
                ae_vector_set_length(&w2, n, _state);
                s = 1+ae_exp(10*ae_randomreal(_state), _state);
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = (double)i/(double)(n-1);
                    y.ptr.p_double[i] = ae_randomreal(_state);
                    w.ptr.p_double[i] = 0.1+ae_randomreal(_state);
                    w2.ptr.p_double[i] = w.ptr.p_double[i]*s;
                }
                spline1dfitpenalizedw(&x, &y, &w, n, m, rho, &info, &c, &rep, _state);
                spline1dfitpenalizedw(&x, &y, &w2, n, m, rho, &info2, &c2, &rep2, _state);
                if( info>0&&info2>0 )
                {
                    v = ae_randomreal(_state);
                    v1 = spline1dcalc(&c, v, _state);
                    v2 = spline1dcalc(&c2, v, _state);
                    *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(v1-v2, _state),nonstrictthreshold);
                }
                else
                {
                    *fiterrors = ae_true;
                }
            }
        }
    }
    
    /*
     * Advanced proprties:
     * * penalized spline with M about 5*N and sufficiently small Rho
     *   must pass through all points on equidistant grid
     */
    for(n=2; n<=10; n++)
    {
        m = 5*n;
        rho = (double)(-5);
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&y, n, _state);
        ae_vector_set_length(&w, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = (double)i/(double)(n-1);
            y.ptr.p_double[i] = ae_randomreal(_state);
            w.ptr.p_double[i] = 0.1+ae_randomreal(_state);
        }
        spline1dfitpenalized(&x, &y, n, m, rho, &info, &c, &rep, _state);
        if( info>0 )
        {
            for(i=0; i<=n-1; i++)
            {
                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(y.ptr.p_double[i]-spline1dcalc(&c, x.ptr.p_double[i], _state), _state),nonstrictthreshold);
            }
        }
        else
        {
            *fiterrors = ae_true;
        }
        spline1dfitpenalizedw(&x, &y, &w, n, m, rho, &info, &c, &rep, _state);
        if( info>0 )
        {
            for(i=0; i<=n-1; i++)
            {
                *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(y.ptr.p_double[i]-spline1dcalc(&c, x.ptr.p_double[i], _state), _state),nonstrictthreshold);
            }
        }
        else
        {
            *fiterrors = ae_true;
        }
    }
    
    /*
     * Check correctness of error reports
     */
    for(pass=1; pass<=passcount; pass++)
    {
        ae_assert(passcount>=2, "PassCount should be 2 or greater!", _state);
        
        /*
         * solve simple task (all X[] are the same, Y[] are specially
         * calculated to ensure simple form of all types of errors)
         * and check correctness of the errors calculated by subroutines
         *
         * First pass is done with zero Y[], other passes - with random Y[].
         * It should test both ability to correctly calculate errors and
         * ability to not fail while working with zeros :)
         */
        n = 4;
        if( pass==1 )
        {
            v1 = (double)(0);
            v2 = (double)(0);
            v = (double)(0);
        }
        else
        {
            v1 = ae_randomreal(_state);
            v2 = ae_randomreal(_state);
            v = 1+ae_randomreal(_state);
        }
        ae_vector_set_length(&x, 4, _state);
        ae_vector_set_length(&y, 4, _state);
        ae_vector_set_length(&w, 4, _state);
        x.ptr.p_double[0] = (double)(0);
        y.ptr.p_double[0] = v-v2;
        w.ptr.p_double[0] = (double)(1);
        x.ptr.p_double[1] = (double)(0);
        y.ptr.p_double[1] = v-v1;
        w.ptr.p_double[1] = (double)(1);
        x.ptr.p_double[2] = (double)(0);
        y.ptr.p_double[2] = v+v1;
        w.ptr.p_double[2] = (double)(1);
        x.ptr.p_double[3] = (double)(0);
        y.ptr.p_double[3] = v+v2;
        w.ptr.p_double[3] = (double)(1);
        refrms = ae_sqrt((ae_sqr(v1, _state)+ae_sqr(v2, _state))/2, _state);
        refavg = (ae_fabs(v1, _state)+ae_fabs(v2, _state))/2;
        if( pass==1 )
        {
            refavgrel = (double)(0);
        }
        else
        {
            refavgrel = 0.25*(ae_fabs(v2, _state)/ae_fabs(v-v2, _state)+ae_fabs(v1, _state)/ae_fabs(v-v1, _state)+ae_fabs(v1, _state)/ae_fabs(v+v1, _state)+ae_fabs(v2, _state)/ae_fabs(v+v2, _state));
        }
        refmax = ae_maxreal(v1, v2, _state);
        
        /*
         * Test penalized spline
         */
        spline1dfitpenalizedw(&x, &y, &w, 4, 4, 0.0, &info, &c, &rep, _state);
        if( info<=0 )
        {
            *fiterrors = ae_true;
        }
        else
        {
            s = spline1dcalc(&c, (double)(0), _state);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(s-v, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.rmserror-refrms, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.avgerror-refavg, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.avgrelerror-refavgrel, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.maxerror-refmax, _state),threshold);
        }
        
        /*
         * Test cubic fitting
         */
        spline1dfitcubic(&x, &y, 4, 4, &info, &c, &rep, _state);
        if( info<=0 )
        {
            *fiterrors = ae_true;
        }
        else
        {
            s = spline1dcalc(&c, (double)(0), _state);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(s-v, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.rmserror-refrms, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.avgerror-refavg, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.avgrelerror-refavgrel, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.maxerror-refmax, _state),threshold);
        }
        
        /*
         * Test Hermite fitting
         */
        spline1dfithermite(&x, &y, 4, 4, &info, &c, &rep, _state);
        if( info<=0 )
        {
            *fiterrors = ae_true;
        }
        else
        {
            s = spline1dcalc(&c, (double)(0), _state);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(s-v, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.rmserror-refrms, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.avgerror-refavg, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.avgrelerror-refavgrel, _state),threshold);
            *fiterrors = *fiterrors||ae_fp_greater(ae_fabs(rep.maxerror-refmax, _state),threshold);
        }
    }
    ae_frame_leave(_state);
}


static void testlsfitunit_testgeneralfitting(ae_bool* llserrors,
     ae_bool* nlserrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    double threshold;
    double nlthreshold;
    ae_int_t maxn;
    ae_int_t maxm;
    ae_int_t skind;
    ae_int_t pkind;
    ae_int_t passcount;
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t pass;
    double xscale;
    double cscale;
    double wscale;
    double noiselevel;
    double tol;
    double diffstep;
    ae_vector x;
    ae_vector y;
    ae_vector y2;
    ae_vector w;
    ae_vector w2;
    ae_vector s;
    ae_vector c;
    ae_vector cstart;
    ae_vector cend;
    ae_vector c2;
    ae_matrix a;
    ae_matrix a2;
    ae_matrix cm;
    double v;
    double v1;
    double v2;
    lsfitreport rep;
    lsfitreport rep2;
    ae_int_t info;
    ae_int_t info2;
    double refrms;
    double refavg;
    double refavgrel;
    double refmax;
    double avgdeviationpar;
    double avgdeviationcurve;
    double avgdeviationnoise;
    double adccnt;
    double adpcnt;
    double adncnt;
    lsfitstate state;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&c, 0, DT_REAL, _state);
    ae_vector_init(&cstart, 0, DT_REAL, _state);
    ae_vector_init(&cend, 0, DT_REAL, _state);
    ae_vector_init(&c2, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cm, 0, 0, DT_REAL, _state);
    _lsfitreport_init(&rep, _state);
    _lsfitreport_init(&rep2, _state);
    _lsfitstate_init(&state, _state);

    *llserrors = ae_false;
    *nlserrors = ae_false;
    threshold = 10000*ae_machineepsilon;
    nlthreshold = 0.00001;
    diffstep = 0.0001;
    maxn = 6;
    maxm = 6;
    passcount = 4;
    
    /*
     * Testing unconstrained least squares (linear/nonlinear)
     */
    for(n=1; n<=maxn; n++)
    {
        for(m=1; m<=maxm; m++)
        {
            for(pass=1; pass<=passcount; pass++)
            {
                
                /*
                 * Solve non-degenerate linear least squares task
                 * Use Chebyshev basis. Its condition number is very good.
                 */
                ae_matrix_set_length(&a, n, m, _state);
                ae_vector_set_length(&x, n, _state);
                ae_vector_set_length(&y, n, _state);
                ae_vector_set_length(&w, n, _state);
                xscale = 0.9+0.1*ae_randomreal(_state);
                for(i=0; i<=n-1; i++)
                {
                    if( n==1 )
                    {
                        x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    else
                    {
                        x.ptr.p_double[i] = xscale*((double)(2*i)/(double)(n-1)-1);
                    }
                    y.ptr.p_double[i] = 3*x.ptr.p_double[i]+ae_exp(x.ptr.p_double[i], _state);
                    w.ptr.p_double[i] = 1+ae_randomreal(_state);
                    a.ptr.pp_double[i][0] = (double)(1);
                    if( m>1 )
                    {
                        a.ptr.pp_double[i][1] = x.ptr.p_double[i];
                    }
                    for(j=2; j<=m-1; j++)
                    {
                        a.ptr.pp_double[i][j] = 2*x.ptr.p_double[i]*a.ptr.pp_double[i][j-1]-a.ptr.pp_double[i][j-2];
                    }
                }
                
                /*
                 * 1. test weighted fitting (optimality)
                 * 2. Solve degenerate least squares task built on the basis
                 *    of previous task
                 */
                lsfitlinearw(&y, &w, &a, n, m, &info, &c, &rep, _state);
                if( info<=0 )
                {
                    *llserrors = ae_true;
                }
                else
                {
                    *llserrors = *llserrors||!testlsfitunit_isglssolution(n, m, 0, &y, &w, &a, &cm, &c, _state);
                }
                ae_matrix_set_length(&a2, n, 2*m, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        a2.ptr.pp_double[i][2*j+0] = a.ptr.pp_double[i][j];
                        a2.ptr.pp_double[i][2*j+1] = a.ptr.pp_double[i][j];
                    }
                }
                lsfitlinearw(&y, &w, &a2, n, 2*m, &info, &c2, &rep, _state);
                if( info<=0 )
                {
                    *llserrors = ae_true;
                }
                else
                {
                    
                    /*
                     * test answer correctness using design matrix properties
                     * and previous task solution
                     */
                    for(j=0; j<=m-1; j++)
                    {
                        *llserrors = *llserrors||ae_fp_greater(ae_fabs(c2.ptr.p_double[2*j+0]+c2.ptr.p_double[2*j+1]-c.ptr.p_double[j], _state),threshold);
                    }
                }
                
                /*
                 * test non-weighted fitting
                 */
                ae_vector_set_length(&w2, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    w2.ptr.p_double[i] = (double)(1);
                }
                lsfitlinearw(&y, &w2, &a, n, m, &info, &c, &rep, _state);
                lsfitlinear(&y, &a, n, m, &info2, &c2, &rep2, _state);
                if( info<=0||info2<=0 )
                {
                    *llserrors = ae_true;
                }
                else
                {
                    
                    /*
                     * test answer correctness
                     */
                    for(j=0; j<=m-1; j++)
                    {
                        *llserrors = *llserrors||ae_fp_greater(ae_fabs(c.ptr.p_double[j]-c2.ptr.p_double[j], _state),threshold);
                    }
                    *llserrors = *llserrors||ae_fp_greater(ae_fabs(rep.taskrcond-rep2.taskrcond, _state),threshold);
                }
                
                /*
                 * test nonlinear fitting on the linear task
                 * (only non-degenerate tasks are tested)
                 * and compare with answer from linear fitting subroutine
                 */
                if( n>=m )
                {
                    ae_vector_set_length(&c2, m, _state);
                    
                    /*
                     * test function/gradient/Hessian-based weighted fitting
                     */
                    lsfitlinearw(&y, &w, &a, n, m, &info, &c, &rep, _state);
                    for(i=0; i<=m-1; i++)
                    {
                        c2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    lsfitcreatewf(&a, &y, &w, &c2, n, m, m, diffstep, &state, _state);
                    lsfitsetcond(&state, 0.0, nlthreshold, 0, _state);
                    testlsfitunit_fitlinearnonlinear(m, 0, &a, &state, nlserrors, _state);
                    lsfitresults(&state, &info, &c2, &rep2, _state);
                    if( info<=0 )
                    {
                        *nlserrors = ae_true;
                    }
                    else
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(c.ptr.p_double[i]-c2.ptr.p_double[i], _state),100*nlthreshold);
                        }
                    }
                    for(i=0; i<=m-1; i++)
                    {
                        c2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    lsfitcreatewfg(&a, &y, &w, &c2, n, m, m, ae_fp_greater(ae_randomreal(_state),0.5), &state, _state);
                    lsfitsetcond(&state, 0.0, nlthreshold, 0, _state);
                    testlsfitunit_fitlinearnonlinear(m, 1, &a, &state, nlserrors, _state);
                    lsfitresults(&state, &info, &c2, &rep2, _state);
                    if( info<=0 )
                    {
                        *nlserrors = ae_true;
                    }
                    else
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(c.ptr.p_double[i]-c2.ptr.p_double[i], _state),100*nlthreshold);
                        }
                    }
                    for(i=0; i<=m-1; i++)
                    {
                        c2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    lsfitcreatewfgh(&a, &y, &w, &c2, n, m, m, &state, _state);
                    lsfitsetcond(&state, 0.0, nlthreshold, 0, _state);
                    testlsfitunit_fitlinearnonlinear(m, 2, &a, &state, nlserrors, _state);
                    lsfitresults(&state, &info, &c2, &rep2, _state);
                    if( info<=0 )
                    {
                        *nlserrors = ae_true;
                    }
                    else
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(c.ptr.p_double[i]-c2.ptr.p_double[i], _state),100*nlthreshold);
                        }
                    }
                    
                    /*
                     * test gradient-only or Hessian-based fitting without weights
                     */
                    lsfitlinear(&y, &a, n, m, &info, &c, &rep, _state);
                    for(i=0; i<=m-1; i++)
                    {
                        c2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    lsfitcreatef(&a, &y, &c2, n, m, m, diffstep, &state, _state);
                    lsfitsetcond(&state, 0.0, nlthreshold, 0, _state);
                    testlsfitunit_fitlinearnonlinear(m, 0, &a, &state, nlserrors, _state);
                    lsfitresults(&state, &info, &c2, &rep2, _state);
                    if( info<=0 )
                    {
                        *nlserrors = ae_true;
                    }
                    else
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(c.ptr.p_double[i]-c2.ptr.p_double[i], _state),100*nlthreshold);
                        }
                    }
                    for(i=0; i<=m-1; i++)
                    {
                        c2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    lsfitcreatefg(&a, &y, &c2, n, m, m, ae_fp_greater(ae_randomreal(_state),0.5), &state, _state);
                    lsfitsetcond(&state, 0.0, nlthreshold, 0, _state);
                    testlsfitunit_fitlinearnonlinear(m, 1, &a, &state, nlserrors, _state);
                    lsfitresults(&state, &info, &c2, &rep2, _state);
                    if( info<=0 )
                    {
                        *nlserrors = ae_true;
                    }
                    else
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(c.ptr.p_double[i]-c2.ptr.p_double[i], _state),100*nlthreshold);
                        }
                    }
                    for(i=0; i<=m-1; i++)
                    {
                        c2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    lsfitcreatefgh(&a, &y, &c2, n, m, m, &state, _state);
                    lsfitsetcond(&state, 0.0, nlthreshold, 0, _state);
                    testlsfitunit_fitlinearnonlinear(m, 2, &a, &state, nlserrors, _state);
                    lsfitresults(&state, &info, &c2, &rep2, _state);
                    if( info<=0 )
                    {
                        *nlserrors = ae_true;
                    }
                    else
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(c.ptr.p_double[i]-c2.ptr.p_double[i], _state),100*nlthreshold);
                        }
                    }
                }
            }
        }
        
        /*
         * test correctness of the RCond field
         */
        ae_matrix_set_length(&a, n-1+1, n-1+1, _state);
        ae_vector_set_length(&x, n-1+1, _state);
        ae_vector_set_length(&y, n-1+1, _state);
        ae_vector_set_length(&w, n-1+1, _state);
        v1 = ae_maxrealnumber;
        v2 = ae_minrealnumber;
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = 0.1+0.9*ae_randomreal(_state);
            y.ptr.p_double[i] = 0.1+0.9*ae_randomreal(_state);
            w.ptr.p_double[i] = (double)(1);
            for(j=0; j<=n-1; j++)
            {
                if( i==j )
                {
                    a.ptr.pp_double[i][i] = 0.1+0.9*ae_randomreal(_state);
                    v1 = ae_minreal(v1, a.ptr.pp_double[i][i], _state);
                    v2 = ae_maxreal(v2, a.ptr.pp_double[i][i], _state);
                }
                else
                {
                    a.ptr.pp_double[i][j] = (double)(0);
                }
            }
        }
        lsfitlinearw(&y, &w, &a, n, n, &info, &c, &rep, _state);
        if( info<=0 )
        {
            *llserrors = ae_true;
        }
        else
        {
            *llserrors = *llserrors||ae_fp_greater(ae_fabs(rep.taskrcond-v1/v2, _state),threshold);
        }
    }
    
    /*
     * Test constrained least squares
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            for(m=1; m<=maxm; m++)
            {
                
                /*
                 * test for K<>0
                 */
                for(k=1; k<=m-1; k++)
                {
                    
                    /*
                     * Prepare Chebyshev basis. Its condition number is very good.
                     * Prepare constraints (random numbers)
                     */
                    ae_matrix_set_length(&a, n, m, _state);
                    ae_vector_set_length(&x, n, _state);
                    ae_vector_set_length(&y, n, _state);
                    ae_vector_set_length(&w, n, _state);
                    xscale = 0.9+0.1*ae_randomreal(_state);
                    for(i=0; i<=n-1; i++)
                    {
                        if( n==1 )
                        {
                            x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        }
                        else
                        {
                            x.ptr.p_double[i] = xscale*((double)(2*i)/(double)(n-1)-1);
                        }
                        y.ptr.p_double[i] = 3*x.ptr.p_double[i]+ae_exp(x.ptr.p_double[i], _state);
                        w.ptr.p_double[i] = 1+ae_randomreal(_state);
                        a.ptr.pp_double[i][0] = (double)(1);
                        if( m>1 )
                        {
                            a.ptr.pp_double[i][1] = x.ptr.p_double[i];
                        }
                        for(j=2; j<=m-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 2*x.ptr.p_double[i]*a.ptr.pp_double[i][j-1]-a.ptr.pp_double[i][j-2];
                        }
                    }
                    ae_matrix_set_length(&cm, k, m+1, _state);
                    for(i=0; i<=k-1; i++)
                    {
                        for(j=0; j<=m; j++)
                        {
                            cm.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                    
                    /*
                     * Solve constrained task
                     */
                    lsfitlinearwc(&y, &w, &a, &cm, n, m, k, &info, &c, &rep, _state);
                    if( info<=0 )
                    {
                        *llserrors = ae_true;
                    }
                    else
                    {
                        *llserrors = *llserrors||!testlsfitunit_isglssolution(n, m, k, &y, &w, &a, &cm, &c, _state);
                    }
                    
                    /*
                     * test non-weighted fitting
                     */
                    ae_vector_set_length(&w2, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        w2.ptr.p_double[i] = (double)(1);
                    }
                    lsfitlinearwc(&y, &w2, &a, &cm, n, m, k, &info, &c, &rep, _state);
                    lsfitlinearc(&y, &a, &cm, n, m, k, &info2, &c2, &rep2, _state);
                    if( info<=0||info2<=0 )
                    {
                        *llserrors = ae_true;
                    }
                    else
                    {
                        
                        /*
                         * test answer correctness
                         */
                        for(j=0; j<=m-1; j++)
                        {
                            *llserrors = *llserrors||ae_fp_greater(ae_fabs(c.ptr.p_double[j]-c2.ptr.p_double[j], _state),threshold);
                        }
                        *llserrors = *llserrors||ae_fp_greater(ae_fabs(rep.taskrcond-rep2.taskrcond, _state),threshold);
                    }
                }
            }
        }
    }
    
    /*
     * nonlinear task for nonlinear fitting:
     *
     *     f(X,C) = 1/(1+C*X^2),
     *     C(true) = 2.
     */
    n = 100;
    ae_vector_set_length(&c, 1, _state);
    c.ptr.p_double[0] = 1+2*ae_randomreal(_state);
    ae_matrix_set_length(&a, n, 1, _state);
    ae_vector_set_length(&y, n, _state);
    for(i=0; i<=n-1; i++)
    {
        a.ptr.pp_double[i][0] = 4*ae_randomreal(_state)-2;
        y.ptr.p_double[i] = 1/(1+2*ae_sqr(a.ptr.pp_double[i][0], _state));
    }
    lsfitcreatefg(&a, &y, &c, n, 1, 1, ae_true, &state, _state);
    lsfitsetcond(&state, 0.0, nlthreshold, 0, _state);
    while(lsfititeration(&state, _state))
    {
        if( state.needf )
        {
            state.f = 1/(1+state.c.ptr.p_double[0]*ae_sqr(state.x.ptr.p_double[0], _state));
        }
        if( state.needfg )
        {
            state.f = 1/(1+state.c.ptr.p_double[0]*ae_sqr(state.x.ptr.p_double[0], _state));
            state.g.ptr.p_double[0] = -ae_sqr(state.x.ptr.p_double[0], _state)/ae_sqr(1+state.c.ptr.p_double[0]*ae_sqr(state.x.ptr.p_double[0], _state), _state);
        }
    }
    lsfitresults(&state, &info, &c, &rep, _state);
    if( info<=0 )
    {
        *nlserrors = ae_true;
    }
    else
    {
        *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(c.ptr.p_double[0]-2, _state),100*nlthreshold);
    }
    
    /*
     * solve simple task (fitting by constant function) and check
     * correctness of the errors calculated by subroutines
     */
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * test on task with non-zero Yi
         */
        n = 4;
        v1 = ae_randomreal(_state);
        v2 = ae_randomreal(_state);
        v = 1+ae_randomreal(_state);
        ae_vector_set_length(&c, 1, _state);
        c.ptr.p_double[0] = 1+2*ae_randomreal(_state);
        ae_matrix_set_length(&a, 4, 1, _state);
        ae_vector_set_length(&y, 4, _state);
        a.ptr.pp_double[0][0] = (double)(1);
        y.ptr.p_double[0] = v-v2;
        a.ptr.pp_double[1][0] = (double)(1);
        y.ptr.p_double[1] = v-v1;
        a.ptr.pp_double[2][0] = (double)(1);
        y.ptr.p_double[2] = v+v1;
        a.ptr.pp_double[3][0] = (double)(1);
        y.ptr.p_double[3] = v+v2;
        refrms = ae_sqrt((ae_sqr(v1, _state)+ae_sqr(v2, _state))/2, _state);
        refavg = (ae_fabs(v1, _state)+ae_fabs(v2, _state))/2;
        refavgrel = 0.25*(ae_fabs(v2, _state)/ae_fabs(v-v2, _state)+ae_fabs(v1, _state)/ae_fabs(v-v1, _state)+ae_fabs(v1, _state)/ae_fabs(v+v1, _state)+ae_fabs(v2, _state)/ae_fabs(v+v2, _state));
        refmax = ae_maxreal(v1, v2, _state);
        
        /*
         * Test LLS
         */
        lsfitlinear(&y, &a, 4, 1, &info, &c, &rep, _state);
        if( info<=0 )
        {
            *llserrors = ae_true;
        }
        else
        {
            *llserrors = *llserrors||ae_fp_greater(ae_fabs(c.ptr.p_double[0]-v, _state),threshold);
            *llserrors = *llserrors||ae_fp_greater(ae_fabs(rep.rmserror-refrms, _state),threshold);
            *llserrors = *llserrors||ae_fp_greater(ae_fabs(rep.avgerror-refavg, _state),threshold);
            *llserrors = *llserrors||ae_fp_greater(ae_fabs(rep.avgrelerror-refavgrel, _state),threshold);
            *llserrors = *llserrors||ae_fp_greater(ae_fabs(rep.maxerror-refmax, _state),threshold);
        }
        
        /*
         * Test NLS
         */
        lsfitcreatefg(&a, &y, &c, 4, 1, 1, ae_true, &state, _state);
        lsfitsetcond(&state, 0.0, nlthreshold, 0, _state);
        while(lsfititeration(&state, _state))
        {
            if( state.needf )
            {
                state.f = state.c.ptr.p_double[0];
            }
            if( state.needfg )
            {
                state.f = state.c.ptr.p_double[0];
                state.g.ptr.p_double[0] = (double)(1);
            }
        }
        lsfitresults(&state, &info, &c, &rep, _state);
        if( info<=0 )
        {
            *nlserrors = ae_true;
        }
        else
        {
            *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(c.ptr.p_double[0]-v, _state),threshold);
            *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(rep.rmserror-refrms, _state),threshold);
            *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(rep.avgerror-refavg, _state),threshold);
            *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(rep.avgrelerror-refavgrel, _state),threshold);
            *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(rep.maxerror-refmax, _state),threshold);
        }
    }
    
    /*
     * Check covariance matrix, errors-in-parameters.
     *
     * We test three different solvers:
     * * nonlinear solver
     * * unconstrained linear solver
     * * constrained linear solver with empty set of constrains
     * on two random problems:
     * * problem with known prior, noise, unit weights
     * * problem with known prior, noise, non-unit weights
     *
     * We test that:
     * * rep.ErrPar=sqrt(diag(Rep.CovPar))
     * * Rep.ErrPar is not too optimistic  - average value of ratio
     *   between  |c_fit-c_prior| and ErrPar[] is less than TOL
     * * Rep.ErrPar is not too pessimistic - average value of ratio
     *   is larger than 1/TOL
     * * similarly, Rep.ErrCurve gives good estimate of |A*c_fit - A*c_prior|
     *   - not optimistic, not pessimistic.
     * * similarly, per-point noise estimates are good enough (we use
     *   slightly different tolerances, though)
     * In order to have these estimates we perform many different tests
     * and calculate average deviation divided by ErrPar/ErrCurve. Then
     * we perform test.
     *
     * Due to stochastic nature of the test it is not good idea to
     * consider each case individually - it is better to average over
     * many runs.
     * 
     */
    tol = 10.0;
    for(n=1; n<=10; n++)
    {
        for(skind=0; skind<=2; skind++)
        {
            for(pkind=0; pkind<=1; pkind++)
            {
                
                /*
                 * Generate problem:
                 * * PKind=0 - unit weights
                 * * PKind=1 - non-unit weights, exact estimate of noise at I-th point
                 *
                 * We generate:
                 * * C      -   prior values of parameters
                 * * CStart -   random initial point
                 * * A      -   function matrix
                 * * Y      -   noisy version of A*C
                 * * W      -   weights vector
                 * * S      -   vector of per-point estimates of noise
                 */
                cscale = ae_pow(10.0, 2*randomnormal(_state), _state);
                xscale = ae_pow(10.0, 2*randomnormal(_state), _state);
                noiselevel = 0.01*cscale*xscale;
                ae_vector_set_length(&c, n, _state);
                ae_vector_set_length(&cstart, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    c.ptr.p_double[i] = cscale*randomnormal(_state);
                    cstart.ptr.p_double[i] = cscale*randomnormal(_state);
                }
                ae_matrix_set_length(&a, 1000, n, _state);
                ae_vector_set_length(&y, a.rows, _state);
                ae_vector_set_length(&w, a.rows, _state);
                ae_vector_set_length(&s, a.rows, _state);
                for(i=0; i<=a.rows-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a.ptr.pp_double[i][j] = xscale*randomnormal(_state);
                    }
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &c.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    if( pkind==0 )
                    {
                        w.ptr.p_double[i] = (double)(1);
                        s.ptr.p_double[i] = noiselevel;
                        y.ptr.p_double[i] = v+s.ptr.p_double[i]*randomnormal(_state);
                    }
                    if( pkind==1 )
                    {
                        w.ptr.p_double[i] = 1/noiselevel;
                        s.ptr.p_double[i] = noiselevel;
                        y.ptr.p_double[i] = v+s.ptr.p_double[i]*randomnormal(_state);
                    }
                }
                
                /*
                 * Test different solvers:
                 * * SKind=0 - nonlinear solver
                 * * SKind=1 - linear unconstrained
                 * * SKind=2 - linear constrained with empty set of constraints
                 */
                info = -1;
                if( skind==0 )
                {
                    if( ae_fp_greater(ae_randomreal(_state),0.5) )
                    {
                        lsfitcreatefg(&a, &y, &cstart, a.rows, n, n, ae_true, &state, _state);
                    }
                    else
                    {
                        lsfitcreatef(&a, &y, &cstart, a.rows, n, n, 0.001*cscale, &state, _state);
                    }
                    lsfitsetcond(&state, 0.0, 0.0, 10, _state);
                    while(lsfititeration(&state, _state))
                    {
                        if( state.needf )
                        {
                            state.f = (double)(0);
                            for(i=0; i<=n-1; i++)
                            {
                                state.f = state.f+state.c.ptr.p_double[i]*state.x.ptr.p_double[i];
                            }
                        }
                        if( state.needfg )
                        {
                            state.f = (double)(0);
                            for(i=0; i<=n-1; i++)
                            {
                                state.f = state.f+state.c.ptr.p_double[i]*state.x.ptr.p_double[i];
                                state.g.ptr.p_double[i] = state.x.ptr.p_double[i];
                            }
                        }
                    }
                    lsfitresults(&state, &info, &cend, &rep, _state);
                }
                if( skind==1 )
                {
                    if( pkind==0 )
                    {
                        lsfitlinear(&y, &a, a.rows, n, &info, &cend, &rep, _state);
                    }
                    else
                    {
                        lsfitlinearw(&y, &w, &a, a.rows, n, &info, &cend, &rep, _state);
                    }
                }
                if( skind==2 )
                {
                    if( pkind==0 )
                    {
                        lsfitlinearc(&y, &a, &a2, a.rows, n, 0, &info, &cend, &rep, _state);
                    }
                    else
                    {
                        lsfitlinearwc(&y, &w, &a, &a2, a.rows, n, 0, &info, &cend, &rep, _state);
                    }
                }
                
                /*
                 * Tests:
                 * * check relation between CovPar and ErrPar
                 * * accumulate average deviation in parameters
                 * * accumulate average deviation in curve fit
                 * * accumulate average deviation in noise estimate
                 */
                avgdeviationpar = (double)(0);
                adpcnt = (double)(0);
                avgdeviationcurve = (double)(0);
                adccnt = (double)(0);
                avgdeviationnoise = (double)(0);
                adncnt = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    seterrorflag(llserrors, ae_fp_greater(ae_fabs(rep.covpar.ptr.pp_double[i][i]-ae_sqr(rep.errpar.ptr.p_double[i], _state), _state),100*ae_machineepsilon*ae_maxreal(ae_sqr(rep.errpar.ptr.p_double[i], _state), rep.covpar.ptr.pp_double[i][i], _state)), _state);
                }
                for(i=0; i<=n-1; i++)
                {
                    avgdeviationpar = (avgdeviationpar*adpcnt+ae_fabs(c.ptr.p_double[i]-cend.ptr.p_double[i], _state)/rep.errpar.ptr.p_double[i])/(adpcnt+1);
                    adpcnt = adpcnt+1;
                }
                for(i=0; i<=a.rows-1; i++)
                {
                    v1 = ae_v_dotproduct(&c.ptr.p_double[0], 1, &a.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                    v2 = ae_v_dotproduct(&cend.ptr.p_double[0], 1, &a.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                    avgdeviationcurve = (avgdeviationcurve*adccnt+ae_fabs(v1-v2, _state)/rep.errcurve.ptr.p_double[i])/(adccnt+1);
                    adccnt = adccnt+1;
                    avgdeviationnoise = (avgdeviationnoise*adncnt+rep.noise.ptr.p_double[i]/s.ptr.p_double[i])/(adncnt+1);
                    adncnt = adncnt+1;
                }
                
                /*
                 * Check that estimates are not too optimistic.
                 * This test is performed always.
                 */
                seterrorflag(llserrors, ae_fp_greater(avgdeviationpar,tol), _state);
                seterrorflag(llserrors, ae_fp_greater(avgdeviationcurve,tol), _state);
                seterrorflag(llserrors, ae_fp_greater(avgdeviationnoise,1.50), _state);
                seterrorflag(llserrors, ae_fp_less(avgdeviationnoise,0.66), _state);
                
                /*
                 * Test for estimates being too pessimistic is performed only
                 * when we have more than 4 parameters.
                 */
                seterrorflag(llserrors, n>=5&&ae_fp_less(avgdeviationcurve,0.01), _state);
                seterrorflag(llserrors, n>=5&&ae_fp_less(avgdeviationpar,0.01), _state);
            }
        }
    }
    
    /*
     * Check special property of the LSFit solver: it does not include points with
     * zero weight in the estimate of the noise level. Such property seems to be
     * quite natural, but in fact it requires some additional code in order to
     * ignore such points.
     *
     * In order to test it we solve two problems: one 300xN, with 150 non-zero
     * weights and 150 zero weights - and another one with only 150 points with
     * non-zero weights. Both problems should give us same covariance matrix.
     */
    tol = (double)(10);
    for(n=1; n<=10; n++)
    {
        
        /*
         * Generate N-dimensional linear problem with 300 points:
         * * y = c'*x + noise
         * * prior values of coefficients C has scale CScale
         * * coordinates X has scale XScale
         * * noise in I-th point has magnitude 0.1*CScale*XScale*WScale/W[i]
         */
        cscale = ae_pow(10.0, 2*randomnormal(_state), _state);
        xscale = ae_pow(10.0, 2*randomnormal(_state), _state);
        wscale = ae_pow(10.0, 2*randomnormal(_state), _state);
        noiselevel = 0.1*cscale*xscale;
        ae_vector_set_length(&c, n, _state);
        ae_vector_set_length(&cstart, n, _state);
        for(i=0; i<=n-1; i++)
        {
            c.ptr.p_double[i] = cscale*randomnormal(_state);
            cstart.ptr.p_double[i] = cscale*randomnormal(_state);
        }
        ae_matrix_set_length(&a, 300, n, _state);
        ae_vector_set_length(&y, a.rows, _state);
        ae_vector_set_length(&w, a.rows, _state);
        for(i=0; i<=a.rows-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a.ptr.pp_double[i][j] = xscale*randomnormal(_state);
            }
            v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &c.ptr.p_double[0], 1, ae_v_len(0,n-1));
            if( i<a.rows/2 )
            {
                w.ptr.p_double[i] = ae_pow((double)(10), randomnormal(_state), _state)*wscale;
                y.ptr.p_double[i] = v+noiselevel/w.ptr.p_double[i]*randomnormal(_state);
            }
            else
            {
                w.ptr.p_double[i] = (double)(0);
                y.ptr.p_double[i] = v+noiselevel*randomnormal(_state);
            }
        }
        
        /*
         * Solve problem #1 (with zero weights).
         * We randomly choose between analytic gradient and numerical differentiation.
         */
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            lsfitcreatewfg(&a, &y, &w, &cstart, a.rows, n, n, ae_true, &state, _state);
        }
        else
        {
            lsfitcreatewf(&a, &y, &w, &cstart, a.rows, n, n, 0.001*cscale, &state, _state);
        }
        lsfitsetcond(&state, 0.0, 0.0, 10, _state);
        while(lsfititeration(&state, _state))
        {
            if( state.needf )
            {
                state.f = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.f = state.f+state.c.ptr.p_double[i]*state.x.ptr.p_double[i];
                }
            }
            if( state.needfg )
            {
                state.f = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.f = state.f+state.c.ptr.p_double[i]*state.x.ptr.p_double[i];
                    state.g.ptr.p_double[i] = state.x.ptr.p_double[i];
                }
            }
        }
        lsfitresults(&state, &info, &c2, &rep, _state);
        
        /*
         * Solve problem #2 (only points with non-zero weights).
         * We randomly choose between analytic gradient and numerical differentiation.
         */
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            lsfitcreatewfg(&a, &y, &w, &cstart, a.rows/2, n, n, ae_true, &state, _state);
        }
        else
        {
            lsfitcreatewf(&a, &y, &w, &cstart, a.rows/2, n, n, 0.001*cscale, &state, _state);
        }
        lsfitsetcond(&state, 0.0, 0.0, 10, _state);
        while(lsfititeration(&state, _state))
        {
            if( state.needf )
            {
                state.f = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.f = state.f+state.c.ptr.p_double[i]*state.x.ptr.p_double[i];
                }
            }
            if( state.needfg )
            {
                state.f = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.f = state.f+state.c.ptr.p_double[i]*state.x.ptr.p_double[i];
                    state.g.ptr.p_double[i] = state.x.ptr.p_double[i];
                }
            }
        }
        lsfitresults(&state, &info, &c2, &rep2, _state);
        
        /*
         * Compare covariance matrices, it should be enough to test algorithm
         */
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                *nlserrors = *nlserrors||ae_fp_greater(ae_fabs(rep.covpar.ptr.pp_double[i][j]-rep2.covpar.ptr.pp_double[i][j], _state),1.0E-6*ae_maxreal(rep.covpar.ptr.pp_double[i][i], rep.covpar.ptr.pp_double[j][j], _state));
            }
        }
    }
    
    /*
     * Check correctness of Rep.R2
     * Solve several problems with different values of R2.
     *
     * NOTE: we check only LSFitLinear() because other functions should use same algorithm
     *       for calculation of Rep.R2
     */
    n = 4;
    ae_matrix_set_length(&a, 4, 2, _state);
    ae_vector_set_length(&y, 4, _state);
    a.ptr.pp_double[0][0] = (double)(1);
    a.ptr.pp_double[0][1] = (double)(-2);
    y.ptr.p_double[0] = (double)(-2);
    a.ptr.pp_double[1][0] = (double)(1);
    a.ptr.pp_double[1][1] = (double)(-1);
    y.ptr.p_double[1] = (double)(-1);
    a.ptr.pp_double[2][0] = (double)(1);
    a.ptr.pp_double[2][1] = (double)(1);
    y.ptr.p_double[2] = (double)(1);
    a.ptr.pp_double[3][0] = (double)(1);
    a.ptr.pp_double[3][1] = (double)(2);
    y.ptr.p_double[3] = (double)(2);
    lsfitlinear(&y, &a, 4, 1, &info, &c, &rep, _state);
    *llserrors = (*llserrors||info<=0)||ae_fp_greater(ae_fabs(rep.r2-0, _state),threshold);
    lsfitlinear(&y, &a, 4, 2, &info, &c, &rep, _state);
    *llserrors = (*llserrors||info<=0)||ae_fp_greater(ae_fabs(rep.r2-1, _state),threshold);
    a.ptr.pp_double[0][0] = (double)(1);
    a.ptr.pp_double[0][1] = (double)(-1);
    y.ptr.p_double[0] = (double)(-1);
    a.ptr.pp_double[1][0] = (double)(1);
    a.ptr.pp_double[1][1] = (double)(-1);
    y.ptr.p_double[1] = (double)(0);
    a.ptr.pp_double[2][0] = (double)(1);
    a.ptr.pp_double[2][1] = (double)(1);
    y.ptr.p_double[2] = (double)(1);
    a.ptr.pp_double[3][0] = (double)(1);
    a.ptr.pp_double[3][1] = (double)(1);
    y.ptr.p_double[3] = (double)(0);
    lsfitlinear(&y, &a, 4, 2, &info, &c, &rep, _state);
    *llserrors = (*llserrors||info<=0)||ae_fp_greater(ae_fabs(rep.r2-0.5, _state),threshold);
    n = 3;
    ae_matrix_set_length(&a, 3, 1, _state);
    ae_vector_set_length(&y, 3, _state);
    a.ptr.pp_double[0][0] = (double)(0);
    y.ptr.p_double[0] = (double)(0);
    a.ptr.pp_double[1][0] = (double)(0);
    y.ptr.p_double[1] = (double)(0);
    a.ptr.pp_double[2][0] = (double)(0);
    y.ptr.p_double[2] = (double)(0);
    lsfitlinear(&y, &a, 3, 1, &info, &c, &rep, _state);
    *llserrors = ((*llserrors||info<=0)||!ae_isfinite(rep.r2, _state))||ae_fp_neq(rep.r2,(double)(1));
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests RDP functionality. On error sets FitErrors flag variable;
on success - flag is not changed.
*************************************************************************/
static void testlsfitunit_testrdp(ae_bool* errorflag, ae_state *_state)
{
    ae_frame _frame_block;
    hqrndstate rs;
    ae_vector x;
    ae_vector y;
    ae_vector e;
    ae_vector x2;
    ae_vector y2;
    ae_vector x3;
    ae_vector y3;
    ae_matrix xy;
    ae_matrix xy2;
    ae_matrix xy3;
    ae_vector idx2;
    ae_vector idx3;
    ae_int_t nsections;
    ae_int_t nsections3;
    double eps;
    double v;
    ae_int_t i;
    ae_int_t k;
    ae_int_t n;
    spline1dinterpolant s;

    ae_frame_make(_state, &_frame_block);
    _hqrndstate_init(&rs, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&x3, 0, DT_REAL, _state);
    ae_vector_init(&y3, 0, DT_REAL, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy3, 0, 0, DT_REAL, _state);
    ae_vector_init(&idx2, 0, DT_INT, _state);
    ae_vector_init(&idx3, 0, DT_INT, _state);
    _spline1dinterpolant_init(&s, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Non-parametric, single section basic test (fixed)
     */
    ae_vector_set_length(&x, 2, _state);
    ae_vector_set_length(&y, 2, _state);
    x.ptr.p_double[0] = (double)(0);
    x.ptr.p_double[1] = 1.5;
    y.ptr.p_double[0] = (double)(2);
    y.ptr.p_double[1] = (double)(3);
    lstfitpiecewiselinearrdpfixed(&x, &y, 2, 1, &x2, &y2, &nsections, _state);
    seterrorflag(errorflag, nsections!=1, _state);
    if( nsections==1 )
    {
        seterrorflag(errorflag, ae_fp_neq(x2.ptr.p_double[0],(double)(0)), _state);
        seterrorflag(errorflag, ae_fp_neq(x2.ptr.p_double[1],1.5), _state);
        seterrorflag(errorflag, ae_fp_neq(y2.ptr.p_double[0],(double)(2)), _state);
        seterrorflag(errorflag, ae_fp_neq(y2.ptr.p_double[1],(double)(3)), _state);
    }
    
    /*
     * Non-parametric, single section tied test (fixed)
     */
    ae_vector_set_length(&x, 5, _state);
    ae_vector_set_length(&y, 5, _state);
    x.ptr.p_double[0] = (double)(0);
    x.ptr.p_double[1] = 1.5;
    x.ptr.p_double[2] = (double)(0);
    x.ptr.p_double[3] = (double)(0);
    x.ptr.p_double[4] = 1.5;
    y.ptr.p_double[0] = (double)(2);
    y.ptr.p_double[1] = (double)(1);
    y.ptr.p_double[2] = (double)(3);
    y.ptr.p_double[3] = (double)(1);
    y.ptr.p_double[4] = (double)(5);
    lstfitpiecewiselinearrdpfixed(&x, &y, 5, 1, &x2, &y2, &nsections, _state);
    seterrorflag(errorflag, nsections!=1, _state);
    if( nsections==1 )
    {
        seterrorflag(errorflag, ae_fp_neq(x2.ptr.p_double[0],(double)(0)), _state);
        seterrorflag(errorflag, ae_fp_neq(x2.ptr.p_double[1],1.5), _state);
        seterrorflag(errorflag, ae_fp_neq(y2.ptr.p_double[0],(double)(2)), _state);
        seterrorflag(errorflag, ae_fp_neq(y2.ptr.p_double[1],(double)(3)), _state);
    }
    
    /*
     * Non-parametric, two-section test (fixed)
     */
    ae_vector_set_length(&x, 5, _state);
    ae_vector_set_length(&y, 5, _state);
    x.ptr.p_double[0] = (double)(0);
    x.ptr.p_double[1] = 0.5;
    x.ptr.p_double[2] = (double)(1);
    x.ptr.p_double[3] = 1.75;
    x.ptr.p_double[4] = (double)(2);
    y.ptr.p_double[0] = (double)(1);
    y.ptr.p_double[1] = 2.1;
    y.ptr.p_double[2] = (double)(3);
    y.ptr.p_double[3] = 5.21;
    y.ptr.p_double[4] = (double)(6);
    lstfitpiecewiselinearrdpfixed(&x, &y, 5, 2, &x2, &y2, &nsections, _state);
    seterrorflag(errorflag, nsections!=2, _state);
    if( nsections==2 )
    {
        seterrorflag(errorflag, ae_fp_neq(x2.ptr.p_double[0],(double)(0)), _state);
        seterrorflag(errorflag, ae_fp_neq(x2.ptr.p_double[1],(double)(1)), _state);
        seterrorflag(errorflag, ae_fp_neq(x2.ptr.p_double[2],(double)(2)), _state);
        seterrorflag(errorflag, ae_fp_neq(y2.ptr.p_double[0],(double)(1)), _state);
        seterrorflag(errorflag, ae_fp_neq(y2.ptr.p_double[1],(double)(3)), _state);
        seterrorflag(errorflag, ae_fp_neq(y2.ptr.p_double[2],(double)(6)), _state);
    }
    
    /*
     * Non-parametric, variable precision test (non-fixed), results are compared against fixed-section test
     */
    eps = 10.0;
    n = 100;
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&y, n, _state);
    while(ae_fp_greater_eq(eps,0.0001))
    {
        
        /*
         * Generate set of randomly rearranged points
         */
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = ae_pi*i/(n-1);
            y.ptr.p_double[i] = ae_sin(x.ptr.p_double[i], _state)+0.01*(hqrnduniformr(&rs, _state)-0.5);
        }
        for(i=0; i<=n-2; i++)
        {
            k = i+hqrnduniformi(&rs, n-i, _state);
            v = x.ptr.p_double[i];
            x.ptr.p_double[i] = x.ptr.p_double[k];
            x.ptr.p_double[k] = v;
            v = y.ptr.p_double[i];
            y.ptr.p_double[i] = y.ptr.p_double[k];
            y.ptr.p_double[k] = v;
        }
        
        /*
         * Perform run of eps-based RDP algorithm
         */
        lstfitpiecewiselinearrdp(&x, &y, n, eps, &x2, &y2, &nsections, _state);
        seterrorflag(errorflag, nsections==0, _state);
        if( nsections==0 )
        {
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Check properties
         */
        for(i=0; i<=nsections-1; i++)
        {
            seterrorflag(errorflag, ae_fp_greater(x2.ptr.p_double[i],x2.ptr.p_double[i+1]), _state);
        }
        spline1dbuildlinear(&x2, &y2, nsections+1, &s, _state);
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(errorflag, ae_fp_greater(ae_fabs(spline1dcalc(&s, x.ptr.p_double[i], _state)-y.ptr.p_double[i], _state),eps), _state);
        }
        
        /*
         * compare results with values returned by section-based algorithm
         */
        lstfitpiecewiselinearrdpfixed(&x, &y, n, nsections, &x3, &y3, &nsections3, _state);
        seterrorflag(errorflag, nsections3!=nsections, _state);
        if( *errorflag )
        {
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=nsections-1; i++)
        {
            seterrorflag(errorflag, ae_fp_greater(ae_fabs(x2.ptr.p_double[i]-x3.ptr.p_double[i], _state),1000*ae_machineepsilon), _state);
            seterrorflag(errorflag, ae_fp_greater(ae_fabs(y2.ptr.p_double[i]-y3.ptr.p_double[i], _state),1000*ae_machineepsilon), _state);
        }
        
        /*
         * Next epsilon
         */
        eps = eps*0.5;
    }
    
    /*
     * Test that non-parametric RDP correctly handles requests for more than N-1 section
     */
    n = 100;
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&y, n, _state);
    for(i=0; i<=n-1; i++)
    {
        x.ptr.p_double[i] = ae_pi*hqrnduniformr(&rs, _state);
        y.ptr.p_double[i] = ae_sin(x.ptr.p_double[i], _state)+0.01*(hqrnduniformr(&rs, _state)-0.5);
    }
    lstfitpiecewiselinearrdpfixed(&x, &y, n, n, &x2, &y2, &nsections, _state);
    seterrorflag(errorflag, nsections!=n-1, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests 4PL/5PL fitting. On error sets FitErrors flag variable;
on success - flag is not changed.
*************************************************************************/
static void testlsfitunit_testlogisticfitting(ae_bool* fiterrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_int_t n;
    ae_int_t nz;
    ae_int_t ntotal;
    ae_int_t i;
    ae_int_t k;
    double v;
    double vv;
    ae_int_t k0;
    ae_int_t k1;
    double v0;
    double v1;
    ae_int_t pass;
    ae_int_t idxa;
    ae_int_t idxb;
    ae_int_t idxc;
    ae_int_t idxd;
    ae_int_t idxg;
    ae_int_t idxx;
    double a;
    double b;
    double c;
    double d;
    double g;
    double ae;
    double be;
    double ce;
    double de;
    double ge;
    double scalex;
    double scaley;
    double noise;
    double tol;
    lsfitreport rep;
    hqrndstate rs;
    double er2;
    double erms;
    double eavg;
    double eavgrel;
    double emax;
    double rss;
    double tss;
    double meany;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    _lsfitreport_init(&rep, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * 4PL/5PL calculation
     */
    tol = 1.0E-6;
    for(idxa=-5; idxa<=5; idxa++)
    {
        for(idxb=-3; idxb<=3; idxb++)
        {
            for(idxc=-5; idxc<=5; idxc++)
            {
                for(idxd=-5; idxd<=5; idxd++)
                {
                    for(idxg=-5; idxg<=5; idxg++)
                    {
                        for(idxx=-5; idxx<=5; idxx++)
                        {
                            
                            /*
                             * Convert Idx* to corresponding parameter value
                             */
                            a = (double)(idxa);
                            b = (double)(idxb);
                            c = ae_pow((double)(2), (double)(idxc), _state);
                            d = (double)(idxd);
                            g = ae_pow((double)(2), (double)(idxg), _state);
                            if( idxx!=0 )
                            {
                                v = ae_pow((double)(2), (double)(idxx), _state);
                            }
                            else
                            {
                                v = (double)(0);
                            }
                            
                            /*
                             * Test 4PL calculation
                             */
                            vv = 0.0;
                            if( ae_fp_neq(v,(double)(0)) )
                            {
                                vv = d+(a-d)/(1+ae_pow(v/c, b, _state));
                            }
                            else
                            {
                                if( ae_fp_greater(b,(double)(0)) )
                                {
                                    vv = a;
                                }
                                if( ae_fp_less(b,(double)(0)) )
                                {
                                    vv = d;
                                }
                                if( ae_fp_eq(b,(double)(0)) )
                                {
                                    vv = 0.5*(a+d);
                                }
                            }
                            v0 = logisticcalc4(v, a, b, c, d, _state);
                            seterrorflag(fiterrors, ae_fp_greater(ae_fabs(vv-v0, _state),tol), _state);
                            
                            /*
                             * Test 5PL calculation
                             */
                            if( ae_fp_neq(v,(double)(0)) )
                            {
                                vv = d+(a-d)/ae_pow(1+ae_pow(v/c, b, _state), g, _state);
                            }
                            else
                            {
                                if( ae_fp_greater(b,(double)(0)) )
                                {
                                    vv = a;
                                }
                                if( ae_fp_less(b,(double)(0)) )
                                {
                                    vv = d;
                                }
                                if( ae_fp_eq(b,(double)(0)) )
                                {
                                    vv = d+(a-d)/ae_pow((double)(2), g, _state);
                                }
                            }
                            v0 = logisticcalc5(v, a, b, c, d, g, _state);
                            seterrorflag(fiterrors, ae_fp_greater(ae_fabs(vv-v0, _state),tol), _state);
                        }
                    }
                }
            }
        }
    }
    
    /*
     * 4PL fitting
     *
     * Generate random AE/BE/CE/DE, generate random set of points and for
     * each point generate two function values: F(x)+eps and F(x)-eps.
     * Such problem has solution which is exactly AE/BE/CE/DE which were
     * used to generate points.
     *
     * This test checks both unconstrained and constrained fitting (latter
     * one is performed with A constrained to AE, B constrained to BE).
     */
    tol = 0.15;
    for(pass=1; pass<=100; pass++)
    {
        
        /*
         * Generate 2*N points with non-zero X and 2*NZ points with
         * zero X. In most cases we choose N<>0 and NZ<>0, but in
         * some cases either N or NZ (but not both) is zero.
         *
         * X-values have scale equal to ScaleX
         */
        scalex = ae_pow((double)(10), 30*hqrnduniformr(&rs, _state)-15, _state);
        n = 40+hqrnduniformi(&rs, 40, _state);
        nz = 4+hqrnduniformi(&rs, 4, _state);
        if( ae_fp_less(hqrnduniformr(&rs, _state),0.1) )
        {
            if( ae_fp_less(hqrnduniformr(&rs, _state),0.5) )
            {
                n = 0;
            }
            else
            {
                nz = 0;
            }
        }
        ntotal = 2*(n+nz);
        ae_vector_set_length(&x, ntotal, _state);
        for(i=0; i<=n-1; i++)
        {
            v = scalex*ae_exp(ae_log((double)(5), _state)*(2*hqrnduniformr(&rs, _state)-1), _state);
            x.ptr.p_double[2*i+0] = v;
            x.ptr.p_double[2*i+1] = v;
        }
        for(i=0; i<=nz-1; i++)
        {
            x.ptr.p_double[2*n+2*i+0] = (double)(0);
            x.ptr.p_double[2*n+2*i+1] = (double)(0);
        }
        
        /*
         * Fenerate A/B/C/D:
         * * A/D are random with scale equal to ScaleY
         * * B is in [0.25,4.0] (always positive)
         * * for C we choose one of X[], if N>0;
         *   if N=0, we set C=1.
         */
        scaley = ae_pow((double)(10), 30*hqrnduniformr(&rs, _state)-15, _state);
        ae = scaley*(hqrnduniformr(&rs, _state)-0.5);
        be = ae_exp(ae_log((double)(4), _state)*(2*hqrnduniformr(&rs, _state)-1), _state);
        ce = scalex*ae_exp(ae_log((double)(2), _state)*(2*hqrnduniformr(&rs, _state)-1), _state);
        de = ae+scaley*(2*hqrnduniformi(&rs, 2, _state)-1)*(hqrnduniformr(&rs, _state)+0.5);
        
        /*
         * Choose noise level and generate Y[].
         */
        noise = 0.05*scaley;
        ae_vector_set_length(&y, ntotal, _state);
        for(i=0; i<=ntotal/2-1; i++)
        {
            if( ae_fp_neq(x.ptr.p_double[2*i+0],(double)(0)) )
            {
                v = de+(ae-de)/(1.0+ae_pow(x.ptr.p_double[2*i+0]/ce, be, _state));
            }
            else
            {
                v = ae;
            }
            y.ptr.p_double[2*i+0] = v+noise;
            y.ptr.p_double[2*i+1] = v-noise;
        }
        
        /*
         * Unconstrained fit and test
         *
         * NOTE: we test that B>=0 is returned. If BE<0, we use
         *       symmetry property of 4PL model.
         */
        logisticfit4(&x, &y, ntotal, &a, &b, &c, &d, &rep, _state);
        seterrorflag(fiterrors, !ae_isfinite(a, _state), _state);
        seterrorflag(fiterrors, !ae_isfinite(b, _state), _state);
        seterrorflag(fiterrors, !ae_isfinite(c, _state), _state);
        seterrorflag(fiterrors, !ae_isfinite(d, _state), _state);
        seterrorflag(fiterrors, ae_fp_less(b,(double)(0)), _state);
        v = 0.0;
        for(i=0; i<=ntotal-1; i++)
        {
            if( ae_fp_neq(x.ptr.p_double[i],(double)(0)) )
            {
                vv = d+(a-d)/(1.0+ae_pow(x.ptr.p_double[i]/c, b, _state));
            }
            else
            {
                vv = a;
            }
            v = v+ae_sqr(y.ptr.p_double[i]-vv, _state);
        }
        v = ae_sqrt(v/ntotal, _state);
        seterrorflag(fiterrors, ae_fp_greater(v,(1+tol)*noise), _state);
        
        /*
         * Constrained fit and test
         *
         * NOTE: we test that B>=0 is returned. If BE<0, we use
         *       symmetry property of 4PL model.
         */
        for(k0=0; k0<=1; k0++)
        {
            for(k1=0; k1<=1; k1++)
            {
                
                /*
                 * Choose constraints.
                 */
                if( k0==0 )
                {
                    v0 = _state->v_nan;
                }
                else
                {
                    v0 = ae;
                }
                if( k1==0 )
                {
                    v1 = _state->v_nan;
                }
                else
                {
                    v1 = de;
                }
                
                /*
                 * Fit
                 */
                logisticfit4ec(&x, &y, ntotal, v0, v1, &a, &b, &c, &d, &rep, _state);
                
                /*
                 * Check
                 */
                seterrorflag(fiterrors, !ae_isfinite(a, _state), _state);
                seterrorflag(fiterrors, !ae_isfinite(b, _state), _state);
                seterrorflag(fiterrors, !ae_isfinite(c, _state), _state);
                seterrorflag(fiterrors, !ae_isfinite(d, _state), _state);
                seterrorflag(fiterrors, ae_fp_less(b,(double)(0)), _state);
                seterrorflag(fiterrors, k0!=0&&ae_fp_neq(a,v0), _state);
                seterrorflag(fiterrors, k1!=0&&ae_fp_neq(d,v1), _state);
                v = 0.0;
                for(i=0; i<=ntotal-1; i++)
                {
                    if( ae_fp_neq(x.ptr.p_double[i],(double)(0)) )
                    {
                        vv = d+(a-d)/(1.0+ae_pow(x.ptr.p_double[i]/c, b, _state));
                    }
                    else
                    {
                        if( ae_fp_greater_eq(b,(double)(0)) )
                        {
                            vv = a;
                        }
                        else
                        {
                            vv = d;
                        }
                    }
                    v = v+ae_sqr(y.ptr.p_double[i]-vv, _state);
                }
                v = ae_sqrt(v/ntotal, _state);
                seterrorflag(fiterrors, ae_fp_greater(v,(1+tol)*noise), _state);
            }
        }
    }
    
    /*
     * 5PL fitting
     *
     * Generate random AE/BE/CE/DE/GE, generate random set of points and for
     * each point generate two function values: F(x)+eps and F(x)-eps.
     * Such problem has solution which is exactly AE/BE/CE/DE which were
     * used to generate points.
     *
     * NOTE: because problem has higher condition number, we use lower
     *       tolerance for power parameters B and G.
     *
     * This test checks both unconstrained and constrained fitting.
     */
    tol = 0.01;
    for(pass=1; pass<=100; pass++)
    {
        
        /*
         * Generate N points, N-1 of them with non-zero X and
         * last one with zero X.
         * X-values have scale equal to ScaleX
         */
        scalex = ae_pow((double)(10), 30*hqrnduniformr(&rs, _state)-15, _state);
        n = 22;
        ae_vector_set_length(&x, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = scalex*ae_pow((double)(2), (double)(i-10), _state);
        }
        x.ptr.p_double[n-1] = (double)(0);
        
        /*
         * Generate A/B/C/D/G:
         * * A/D are random with scale equal to ScaleY
         * * B is in +-[0.25,4.0]
         * * G is in   [0.25,4.0]
         * * C is in   [0.25,4.0]*ScaleX
         *   if N=0, we set C=1.
         * Generate Y[].
         */
        scaley = ae_pow((double)(10), 30*hqrnduniformr(&rs, _state)-15, _state);
        ae = scaley*(hqrnduniformr(&rs, _state)-0.5);
        be = (2*hqrnduniformi(&rs, 2, _state)-1)*ae_exp(ae_log((double)(4), _state)*(2*hqrnduniformr(&rs, _state)-1), _state);
        ce = scalex*ae_exp(ae_log((double)(5), _state)*(2*hqrnduniformr(&rs, _state)-1), _state);
        de = ae+scaley*(2*hqrnduniformi(&rs, 2, _state)-1)*(hqrnduniformr(&rs, _state)+0.5);
        ge = ae_exp(ae_log(1.5, _state)*(2*hqrnduniformr(&rs, _state)-1), _state);
        ae_vector_set_length(&y, n, _state);
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_neq(x.ptr.p_double[i],(double)(0)) )
            {
                v = de+(ae-de)/ae_pow(1.0+ae_pow(x.ptr.p_double[i]/ce, be, _state), ge, _state);
            }
            else
            {
                if( ae_fp_greater_eq(be,(double)(0)) )
                {
                    v = ae;
                }
                else
                {
                    v = de;
                }
            }
            y.ptr.p_double[i] = v;
        }
        
        /*
         * Unconstrained fit and test
         *
         * NOTE: we test that B>=0 is returned. If BE<0, we use
         *       symmetry property of 4PL model.
         */
        logisticfit5(&x, &y, n, &a, &b, &c, &d, &g, &rep, _state);
        v = 0.0;
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_neq(x.ptr.p_double[i],(double)(0)) )
            {
                vv = d+(a-d)/ae_pow(1.0+ae_pow(x.ptr.p_double[i]/c, b, _state), g, _state);
            }
            else
            {
                if( ae_fp_greater_eq(b,(double)(0)) )
                {
                    vv = a;
                }
                else
                {
                    vv = d;
                }
            }
            v = v+ae_sqr(y.ptr.p_double[i]-vv, _state);
        }
        v = ae_sqrt(v/n, _state);
        seterrorflag(fiterrors, ae_fp_greater(v,scaley*1.0E-6), _state);
        
        /*
         * Constrained fit and test
         */
        for(k0=0; k0<=1; k0++)
        {
            for(k1=0; k1<=1; k1++)
            {
                
                /*
                 * Choose constraints.
                 */
                if( k0==0 )
                {
                    v0 = _state->v_nan;
                }
                else
                {
                    if( ae_fp_greater_eq(be,(double)(0)) )
                    {
                        v0 = ae;
                    }
                    else
                    {
                        v0 = de;
                    }
                }
                if( k1==0 )
                {
                    v1 = _state->v_nan;
                }
                else
                {
                    if( ae_fp_greater_eq(be,(double)(0)) )
                    {
                        v1 = de;
                    }
                    else
                    {
                        v1 = ae;
                    }
                }
                
                /*
                 * Fit
                 */
                logisticfit5ec(&x, &y, n, v0, v1, &a, &b, &c, &d, &g, &rep, _state);
                seterrorflag(fiterrors, !ae_isfinite(a, _state), _state);
                seterrorflag(fiterrors, !ae_isfinite(b, _state), _state);
                seterrorflag(fiterrors, !ae_isfinite(c, _state), _state);
                seterrorflag(fiterrors, !ae_isfinite(d, _state), _state);
                if( ae_fp_greater(b,(double)(0)) )
                {
                    seterrorflag(fiterrors, k0!=0&&ae_fp_neq(a,v0), _state);
                    seterrorflag(fiterrors, k1!=0&&ae_fp_neq(d,v1), _state);
                }
                else
                {
                    seterrorflag(fiterrors, k0!=0&&ae_fp_neq(d,v0), _state);
                    seterrorflag(fiterrors, k1!=0&&ae_fp_neq(a,v1), _state);
                }
                v = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    if( ae_fp_neq(x.ptr.p_double[i],(double)(0)) )
                    {
                        vv = d+(a-d)/ae_pow(1.0+ae_pow(x.ptr.p_double[i]/c, b, _state), g, _state);
                    }
                    else
                    {
                        if( ae_fp_greater_eq(b,(double)(0)) )
                        {
                            vv = a;
                        }
                        else
                        {
                            vv = d;
                        }
                    }
                    v = v+ae_sqr(y.ptr.p_double[i]-vv, _state);
                }
                v = ae_sqrt(v/n, _state);
                seterrorflag(fiterrors, ae_fp_greater(v,scaley*1.0E-6), _state);
            }
        }
    }
    
    /*
     * Test correctness of errors
     */
    tol = 1.0E-6;
    for(pass=1; pass<=50; pass++)
    {
        n = 10;
        meany = 0.0;
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&y, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = (double)(i);
            y.ptr.p_double[i] = hqrnduniformr(&rs, _state)-0.5;
            meany = meany+y.ptr.p_double[i];
        }
        meany = meany/n;
        x.ptr.p_double[1] = (double)(0);
        
        /*
         * Choose model fitting function to test
         */
        k = hqrnduniformi(&rs, 4, _state);
        a = (double)(0);
        d = (double)(0);
        c = (double)(1);
        b = (double)(1);
        g = (double)(1);
        if( k==0 )
        {
            logisticfit4(&x, &y, n, &a, &b, &c, &d, &rep, _state);
            g = 1.0;
        }
        if( k==1 )
        {
            logisticfit4ec(&x, &y, n, hqrnduniformr(&rs, _state)-0.5, hqrnduniformr(&rs, _state)-0.5, &a, &b, &c, &d, &rep, _state);
            g = 1.0;
        }
        if( k==2 )
        {
            logisticfit5(&x, &y, n, &a, &b, &c, &d, &g, &rep, _state);
        }
        if( k==3 )
        {
            logisticfit5ec(&x, &y, n, hqrnduniformr(&rs, _state)-0.5, hqrnduniformr(&rs, _state)-0.5, &a, &b, &c, &d, &g, &rep, _state);
        }
        k = 0;
        erms = (double)(0);
        eavg = (double)(0);
        eavgrel = (double)(0);
        emax = (double)(0);
        rss = 0.0;
        tss = 0.0;
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_neq(x.ptr.p_double[i],(double)(0)) )
            {
                v = d+(a-d)/ae_pow(1.0+ae_pow(x.ptr.p_double[i]/c, b, _state), g, _state);
            }
            else
            {
                if( ae_fp_greater_eq(b,(double)(0)) )
                {
                    v = a;
                }
                else
                {
                    v = d;
                }
            }
            v = v-y.ptr.p_double[i];
            rss = rss+v*v;
            tss = tss+ae_sqr(y.ptr.p_double[i]-meany, _state);
            erms = erms+ae_sqr(v, _state);
            eavg = eavg+ae_fabs(v, _state);
            if( ae_fp_neq(y.ptr.p_double[i],(double)(0)) )
            {
                eavgrel = eavgrel+ae_fabs(v/y.ptr.p_double[i], _state);
                k = k+1;
            }
            emax = ae_maxreal(emax, ae_fabs(v, _state), _state);
        }
        er2 = 1.0-rss/tss;
        erms = ae_sqrt(erms/n, _state);
        eavg = eavg/n;
        if( k>0 )
        {
            eavgrel = eavgrel/k;
        }
        seterrorflag(fiterrors, ae_fp_greater(ae_fabs(erms-rep.rmserror, _state),tol), _state);
        seterrorflag(fiterrors, ae_fp_greater(ae_fabs(eavg-rep.avgerror, _state),tol), _state);
        seterrorflag(fiterrors, ae_fp_greater(ae_fabs(emax-rep.maxerror, _state),tol), _state);
        seterrorflag(fiterrors, ae_fp_greater(ae_fabs(eavgrel-rep.avgrelerror, _state),tol), _state);
        seterrorflag(fiterrors, ae_fp_greater(ae_fabs(er2-rep.r2, _state),tol), _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Tests whether C is solution of (possibly) constrained LLS problem
*************************************************************************/
static ae_bool testlsfitunit_isglssolution(ae_int_t n,
     ae_int_t m,
     ae_int_t k,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* fmatrix,
     /* Real    */ ae_matrix* cmatrix,
     /* Real    */ ae_vector* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _c;
    ae_int_t i;
    ae_int_t j;
    ae_vector c2;
    ae_vector sv;
    ae_vector deltac;
    ae_vector deltaproj;
    ae_matrix u;
    ae_matrix vt;
    double v;
    double s1;
    double s2;
    double s3;
    double delta;
    double threshold;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_c, c, _state);
    c = &_c;
    ae_vector_init(&c2, 0, DT_REAL, _state);
    ae_vector_init(&sv, 0, DT_REAL, _state);
    ae_vector_init(&deltac, 0, DT_REAL, _state);
    ae_vector_init(&deltaproj, 0, DT_REAL, _state);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt, 0, 0, DT_REAL, _state);

    
    /*
     * Setup.
     * Threshold is small because CMatrix may be ill-conditioned
     */
    delta = 0.001;
    threshold = ae_sqrt(ae_machineepsilon, _state);
    ae_vector_set_length(&c2, m, _state);
    ae_vector_set_length(&deltac, m, _state);
    ae_vector_set_length(&deltaproj, m, _state);
    
    /*
     * test whether C is feasible point or not (projC must be close to C)
     */
    for(i=0; i<=k-1; i++)
    {
        v = ae_v_dotproduct(&cmatrix->ptr.pp_double[i][0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,m-1));
        if( ae_fp_greater(ae_fabs(v-cmatrix->ptr.pp_double[i][m], _state),threshold) )
        {
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * find orthogonal basis of Null(CMatrix) (stored in rows from K to M-1)
     */
    if( k>0 )
    {
        rmatrixsvd(cmatrix, k, m, 0, 2, 2, &sv, &u, &vt, _state);
    }
    
    /*
     * Test result
     */
    result = ae_true;
    s1 = testlsfitunit_getglserror(n, m, y, w, fmatrix, c, _state);
    for(j=0; j<=m-1; j++)
    {
        
        /*
         * prepare modification of C which leave us in the feasible set.
         *
         * let deltaC be increment on Jth coordinate, then project
         * deltaC in the Null(CMatrix) and store result in DeltaProj
         */
        ae_v_move(&c2.ptr.p_double[0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,m-1));
        for(i=0; i<=m-1; i++)
        {
            if( i==j )
            {
                deltac.ptr.p_double[i] = delta;
            }
            else
            {
                deltac.ptr.p_double[i] = (double)(0);
            }
        }
        if( k==0 )
        {
            ae_v_move(&deltaproj.ptr.p_double[0], 1, &deltac.ptr.p_double[0], 1, ae_v_len(0,m-1));
        }
        else
        {
            for(i=0; i<=m-1; i++)
            {
                deltaproj.ptr.p_double[i] = (double)(0);
            }
            for(i=k; i<=m-1; i++)
            {
                v = ae_v_dotproduct(&vt.ptr.pp_double[i][0], 1, &deltac.ptr.p_double[0], 1, ae_v_len(0,m-1));
                ae_v_addd(&deltaproj.ptr.p_double[0], 1, &vt.ptr.pp_double[i][0], 1, ae_v_len(0,m-1), v);
            }
        }
        
        /*
         * now we have DeltaProj such that if C is feasible,
         * then C+DeltaProj is feasible too
         */
        ae_v_move(&c2.ptr.p_double[0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,m-1));
        ae_v_add(&c2.ptr.p_double[0], 1, &deltaproj.ptr.p_double[0], 1, ae_v_len(0,m-1));
        s2 = testlsfitunit_getglserror(n, m, y, w, fmatrix, &c2, _state);
        ae_v_move(&c2.ptr.p_double[0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,m-1));
        ae_v_sub(&c2.ptr.p_double[0], 1, &deltaproj.ptr.p_double[0], 1, ae_v_len(0,m-1));
        s3 = testlsfitunit_getglserror(n, m, y, w, fmatrix, &c2, _state);
        result = (result&&ae_fp_greater_eq(s2,s1/(1+threshold)))&&ae_fp_greater_eq(s3,s1/(1+threshold));
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Tests whether C is solution of LLS problem
*************************************************************************/
static double testlsfitunit_getglserror(ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* fmatrix,
     /* Real    */ ae_vector* c,
     ae_state *_state)
{
    ae_int_t i;
    double v;
    double result;


    result = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        v = ae_v_dotproduct(&fmatrix->ptr.pp_double[i][0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,m-1));
        result = result+ae_sqr(w->ptr.p_double[i]*(v-y->ptr.p_double[i]), _state);
    }
    return result;
}


/*************************************************************************
Subroutine for nonlinear fitting of linear problem

DerAvailable:
* 0     when only function value should be used
* 1     when we can provide gradient/function
* 2     when we can provide Hessian/gradient/function

When something which is not permitted by DerAvailable is requested,
this function sets NLSErrors to True.
*************************************************************************/
static void testlsfitunit_fitlinearnonlinear(ae_int_t m,
     ae_int_t deravailable,
     /* Real    */ ae_matrix* xy,
     lsfitstate* state,
     ae_bool* nlserrors,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;


    while(lsfititeration(state, _state))
    {
        
        /*
         * assume that one and only one of flags is set
         * test that we didn't request hessian in hessian-free setting
         */
        if( deravailable<1&&state->needfg )
        {
            *nlserrors = ae_true;
        }
        if( deravailable<2&&state->needfgh )
        {
            *nlserrors = ae_true;
        }
        i = 0;
        if( state->needf )
        {
            i = i+1;
        }
        if( state->needfg )
        {
            i = i+1;
        }
        if( state->needfgh )
        {
            i = i+1;
        }
        if( i!=1 )
        {
            *nlserrors = ae_true;
        }
        
        /*
         * test that PointIndex is consistent with actual point passed
         */
        for(i=0; i<=m-1; i++)
        {
            *nlserrors = *nlserrors||ae_fp_neq(xy->ptr.pp_double[state->pointindex][i],state->x.ptr.p_double[i]);
        }
        
        /*
         * calculate
         */
        if( state->needf )
        {
            v = ae_v_dotproduct(&state->x.ptr.p_double[0], 1, &state->c.ptr.p_double[0], 1, ae_v_len(0,m-1));
            state->f = v;
            continue;
        }
        if( state->needfg )
        {
            v = ae_v_dotproduct(&state->x.ptr.p_double[0], 1, &state->c.ptr.p_double[0], 1, ae_v_len(0,m-1));
            state->f = v;
            ae_v_move(&state->g.ptr.p_double[0], 1, &state->x.ptr.p_double[0], 1, ae_v_len(0,m-1));
            continue;
        }
        if( state->needfgh )
        {
            v = ae_v_dotproduct(&state->x.ptr.p_double[0], 1, &state->c.ptr.p_double[0], 1, ae_v_len(0,m-1));
            state->f = v;
            ae_v_move(&state->g.ptr.p_double[0], 1, &state->x.ptr.p_double[0], 1, ae_v_len(0,m-1));
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    state->h.ptr.pp_double[i][j] = (double)(0);
                }
            }
            continue;
        }
    }
}


/*************************************************************************
This function tests, that gradient verified correctly.
*************************************************************************/
static void testlsfitunit_testgradientcheck(ae_bool* testg,
     ae_state *_state)
{
    ae_frame _frame_block;
    lsfitstate state;
    lsfitreport rep;
    ae_int_t n;
    ae_int_t m;
    ae_int_t k;
    ae_vector c;
    ae_vector cres;
    ae_matrix x;
    ae_vector y;
    ae_vector x0;
    ae_int_t info;
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
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    *testg = ae_false;
    _lsfitstate_init(&state, _state);
    _lsfitreport_init(&rep, _state);
    ae_vector_init(&c, 0, DT_REAL, _state);
    ae_vector_init(&cres, 0, DT_REAL, _state);
    ae_matrix_init(&x, 0, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);

    passcount = 35;
    spp = 1.0;
    teststep = 0.01;
    for(pass=1; pass<=passcount; pass++)
    {
        m = ae_randominteger(5, _state)+1;
        ae_vector_set_length(&x0, m, _state);
        k = ae_randominteger(5, _state)+1;
        ae_vector_set_length(&c, k, _state);
        ae_vector_set_length(&bl, k, _state);
        ae_vector_set_length(&bu, k, _state);
        
        /*
         * Prepare test's parameters
         */
        func = ae_randominteger(3, _state)+1;
        n = ae_randominteger(8, _state)+3;
        ae_matrix_set_length(&x, n, m, _state);
        ae_vector_set_length(&y, n, _state);
        nbrcomp = ae_randominteger(k, _state);
        noise = (double)(2*ae_randominteger(2, _state)-1);
        
        /*
         * Prepare function's parameters
         */
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                x.ptr.pp_double[i][j] = spp*(2*ae_randomreal(_state)-1);
            }
            y.ptr.p_double[i] = spp*(2*ae_randomreal(_state)-1);
        }
        for(i=0; i<=k-1; i++)
        {
            c.ptr.p_double[i] = spp*(2*ae_randomreal(_state)-1);
        }
        for(i=0; i<=m-1; i++)
        {
            x0.ptr.p_double[i] = 10*(2*ae_randomreal(_state)-1);
        }
        
        /*
         * Prepare boundary parameters
         */
        for(i=0; i<=k-1; i++)
        {
            bl.ptr.p_double[i] = ae_randomreal(_state)-spp;
            bu.ptr.p_double[i] = ae_randomreal(_state)+spp-1;
        }
        infcomp = ae_randominteger(k+1, _state);
        if( infcomp<k )
        {
            bl.ptr.p_double[infcomp] = _state->v_neginf;
        }
        infcomp = ae_randominteger(k+1, _state);
        if( infcomp<k )
        {
            bu.ptr.p_double[infcomp] = _state->v_posinf;
        }
        lsfitcreatefg(&x, &y, &c, n, m, k, ae_true, &state, _state);
        lsfitsetgradientcheck(&state, teststep, _state);
        lsfitsetcond(&state, 0.0, 0.0, 100, _state);
        lsfitsetbc(&state, &bl, &bu, _state);
        
        /*
         * Check that the criterion passes a derivative if it is correct
         */
        while(lsfititeration(&state, _state))
        {
            if( state.needfg )
            {
                testlsfitunit_funcderiv(&state.c, &state.x, &x0, k, m, func, &state.f, &state.g, _state);
            }
        }
        lsfitresults(&state, &info, &cres, &rep, _state);
        
        /*
         * Check that error code does not equal to -7 and parameter .VarIdx
         * equal to -1.
         */
        if( info==-7||rep.varidx!=-1 )
        {
            *testg = ae_true;
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Create again and...
         */
        lsfitcreatefg(&x, &y, &c, n, m, k, ae_true, &state, _state);
        lsfitsetgradientcheck(&state, teststep, _state);
        lsfitsetcond(&state, 0.0, 0.0, 100, _state);
        lsfitsetbc(&state, &bl, &bu, _state);
        
        /*
         * Check that the criterion does not miss a derivative if
         * it is incorrect
         */
        while(lsfititeration(&state, _state))
        {
            if( state.needfg )
            {
                testlsfitunit_funcderiv(&state.c, &state.x, &x0, k, m, func, &state.f, &state.g, _state);
                state.g.ptr.p_double[nbrcomp] = state.g.ptr.p_double[nbrcomp]+noise;
            }
        }
        lsfitresults(&state, &info, &cres, &rep, _state);
        
        /*
         * Check that error code equal to -7 and parameter .VarIdx
         * equal to number of incorrect component.
         */
        if( info!=-7||rep.varidx!=nbrcomp )
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
This function return function's value(F=F(X,C)) and it derivatives(DF=dF/dC).
Function dimension is M. Length(C) is K.
    Function's list:
        * funcType=1:
            K>M:
            F(X)=C0^2*(X0-CX0)^2+C1^2*(X1-CX1)^2+...+CM^2*(XM-CXM)^2
                +C(M+1)^2+...+CK^2;
            K<M:
            F(X)=C0^2*(X0-CX0)^2+C1^2*(X1-CX1)^2+...+CK^2*(XK-CXK)^2
                +(X(K+1)-CX(K+1))^2+...+(XM-CXM)^2;
        * funcType=2:
            K>M:
            F(X)=C0*sin(X0-CX0)^2+C1*sin(X1-CX1)^2+...+CM*sin(XM-CXM)^2
                +C(M+1)^3+...+CK^3;
            K<M
            F(X)=C0*sin(X0-CX0)^2+C1*sin(X1-CX1)^2+...+CK*sin(XK-CXK)^2
                +sin(X(K+1)-CX(K+1))^2+...+sin(XM-CXM)^2;
        * funcType=3:
            F(X)=C0^2+C1^2+...+CK^2+(X0-CX0)^2+(X1-CX1)^2+...+(XM-CXM)^2.
*************************************************************************/
static void testlsfitunit_funcderiv(/* Real    */ ae_vector* c,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* x0,
     ae_int_t k,
     ae_int_t m,
     ae_int_t functype,
     double* f,
     /* Real    */ ae_vector* g,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(functype>=1&&functype<=3, "FuncDeriv: incorrect funcType(funcType<1 or funcType>3).", _state);
    ae_assert(k>0, "FuncDeriv: K<=0", _state);
    ae_assert(m>0, "FuncDeriv: M<=0", _state);
    ae_assert(x->cnt>=m, "FuncDeriv: Length(X)<M", _state);
    ae_assert(isfinitevector(x, m, _state), "FuncDeriv: X contains NaN or Infinite.", _state);
    ae_assert(x0->cnt>=m, "FuncDeriv: Length(X0)<M", _state);
    ae_assert(isfinitevector(x0, m, _state), "FuncDeriv: X0 contains NaN or Infinite.", _state);
    ae_assert(c->cnt>=k, "FuncDeriv: Length(X)<K", _state);
    ae_assert(isfinitevector(c, k, _state), "FuncDeriv: C contains NaN or Infinite.", _state);
    if( functype==1 )
    {
        *f = (double)(0);
        for(i=0; i<=ae_minint(m, k, _state)-1; i++)
        {
            *f = *f+ae_sqr(c->ptr.p_double[i]*(x->ptr.p_double[i]-x0->ptr.p_double[i]), _state);
            g->ptr.p_double[i] = 2*c->ptr.p_double[i]*ae_sqr(x->ptr.p_double[i]-x0->ptr.p_double[i], _state);
        }
        if( k>m )
        {
            for(i=m; i<=k-1; i++)
            {
                *f = *f+ae_sqr(c->ptr.p_double[i], _state);
                g->ptr.p_double[i] = 2*c->ptr.p_double[i];
            }
        }
        if( k<m )
        {
            for(i=k; i<=m-1; i++)
            {
                *f = *f+ae_sqr(x->ptr.p_double[i]-x0->ptr.p_double[i], _state);
            }
        }
        return;
    }
    if( functype==2 )
    {
        *f = (double)(0);
        for(i=0; i<=ae_minint(m, k, _state)-1; i++)
        {
            *f = *f+c->ptr.p_double[i]*ae_sqr(ae_sin(x->ptr.p_double[i]-x0->ptr.p_double[i], _state), _state);
            g->ptr.p_double[i] = ae_sqr(ae_sin(x->ptr.p_double[i]-x0->ptr.p_double[i], _state), _state);
        }
        if( k>m )
        {
            for(i=m; i<=k-1; i++)
            {
                *f = *f+c->ptr.p_double[i]*c->ptr.p_double[i]*c->ptr.p_double[i];
                g->ptr.p_double[i] = 3*ae_sqr(c->ptr.p_double[i], _state);
            }
        }
        if( k<m )
        {
            for(i=k; i<=m-1; i++)
            {
                *f = *f+ae_sqr(ae_sin(x->ptr.p_double[i]-x0->ptr.p_double[i], _state), _state);
            }
        }
        return;
    }
    if( functype==3 )
    {
        *f = (double)(0);
        for(i=0; i<=m-1; i++)
        {
            *f = *f+ae_sqr(x->ptr.p_double[i]-x0->ptr.p_double[i], _state);
        }
        for(i=0; i<=k-1; i++)
        {
            *f = *f+c->ptr.p_double[i]*c->ptr.p_double[i];
        }
        for(i=0; i<=k-1; i++)
        {
            g->ptr.p_double[i] = 2*c->ptr.p_double[i];
        }
        return;
    }
}


/*$ End $*/
