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
#include "testratintunit.h"


/*$ Declarations $*/
static void testratintunit_poldiff2(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* f,
     ae_int_t n,
     double t,
     double* p,
     double* dp,
     double* d2p,
     ae_state *_state);
static void testratintunit_brcunset(barycentricinterpolant* b,
     ae_state *_state);


/*$ Body $*/


ae_bool testratint(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_bool bcerrors;
    ae_bool nperrors;
    double threshold;
    double lipschitztol;
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
    double h;
    double s1;
    double s2;
    ae_int_t n;
    ae_int_t n2;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t d;
    ae_int_t pass;
    double maxerr;
    double t;
    double a;
    double b;
    double s;
    double v0;
    double v1;
    double v2;
    double v3;
    double d0;
    double d1;
    double d2;
    ae_bool result;

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

    nperrors = ae_false;
    bcerrors = ae_false;
    waserrors = ae_false;
    
    /*
     * PassCount        number of repeated passes
     * Threshold        error tolerance
     * LipschitzTol     Lipschitz constant increase allowed
     *                  when calculating constant on a twice denser grid
     */
    passcount = 5;
    maxn = 15;
    threshold = 1000000*ae_machineepsilon;
    lipschitztol = 1.3;
    
    /*
     * Basic barycentric functions
     */
    for(n=1; n<=10; n++)
    {
        
        /*
         * randomized tests
         */
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * generate weights from polynomial interpolation
             */
            v0 = 1+0.4*ae_randomreal(_state)-0.2;
            v1 = 2*ae_randomreal(_state)-1;
            v2 = 2*ae_randomreal(_state)-1;
            v3 = 2*ae_randomreal(_state)-1;
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&y, n, _state);
            ae_vector_set_length(&w, n, _state);
            for(i=0; i<=n-1; i++)
            {
                if( n==1 )
                {
                    x.ptr.p_double[i] = (double)(0);
                }
                else
                {
                    x.ptr.p_double[i] = v0*ae_cos(i*ae_pi/(n-1), _state);
                }
                y.ptr.p_double[i] = ae_sin(v1*x.ptr.p_double[i], _state)+ae_cos(v2*x.ptr.p_double[i], _state)+ae_exp(v3*x.ptr.p_double[i], _state);
            }
            for(j=0; j<=n-1; j++)
            {
                w.ptr.p_double[j] = (double)(1);
                for(k=0; k<=n-1; k++)
                {
                    if( k!=j )
                    {
                        w.ptr.p_double[j] = w.ptr.p_double[j]/(x.ptr.p_double[j]-x.ptr.p_double[k]);
                    }
                }
            }
            barycentricbuildxyw(&x, &y, &w, n, &b1, _state);
            
            /*
             * unpack, then pack again and compare
             */
            testratintunit_brcunset(&b2, _state);
            barycentricunpack(&b1, &n2, &x2, &y2, &w2, _state);
            bcerrors = bcerrors||n2!=n;
            barycentricbuildxyw(&x2, &y2, &w2, n2, &b2, _state);
            t = 2*ae_randomreal(_state)-1;
            bcerrors = bcerrors||ae_fp_greater(ae_fabs(barycentriccalc(&b1, t, _state)-barycentriccalc(&b2, t, _state), _state),threshold);
            
            /*
             * copy, compare
             */
            testratintunit_brcunset(&b2, _state);
            barycentriccopy(&b1, &b2, _state);
            t = 2*ae_randomreal(_state)-1;
            bcerrors = bcerrors||ae_fp_greater(ae_fabs(barycentriccalc(&b1, t, _state)-barycentriccalc(&b2, t, _state), _state),threshold);
            
            /*
             * test interpolation properties
             */
            for(i=0; i<=n-1; i++)
            {
                
                /*
                 * test interpolation at nodes
                 */
                bcerrors = bcerrors||ae_fp_greater(ae_fabs(barycentriccalc(&b1, x.ptr.p_double[i], _state)-y.ptr.p_double[i], _state),threshold*ae_fabs(y.ptr.p_double[i], _state));
                
                /*
                 * compare with polynomial interpolation
                 */
                t = 2*ae_randomreal(_state)-1;
                testratintunit_poldiff2(&x, &y, n, t, &v0, &v1, &v2, _state);
                bcerrors = bcerrors||ae_fp_greater(ae_fabs(barycentriccalc(&b1, t, _state)-v0, _state),threshold*ae_maxreal(ae_fabs(v0, _state), (double)(1), _state));
                
                /*
                 * test continuity between nodes
                 * calculate Lipschitz constant on two grids -
                 * dense and even more dense. If Lipschitz constant
                 * on a denser grid is significantly increased,
                 * continuity test is failed
                 */
                t = 3.0;
                k = 100;
                s1 = (double)(0);
                for(j=0; j<=k-1; j++)
                {
                    v1 = x.ptr.p_double[i]+(t-x.ptr.p_double[i])*j/k;
                    v2 = x.ptr.p_double[i]+(t-x.ptr.p_double[i])*(j+1)/k;
                    s1 = ae_maxreal(s1, ae_fabs(barycentriccalc(&b1, v2, _state)-barycentriccalc(&b1, v1, _state), _state)/ae_fabs(v2-v1, _state), _state);
                }
                k = 2*k;
                s2 = (double)(0);
                for(j=0; j<=k-1; j++)
                {
                    v1 = x.ptr.p_double[i]+(t-x.ptr.p_double[i])*j/k;
                    v2 = x.ptr.p_double[i]+(t-x.ptr.p_double[i])*(j+1)/k;
                    s2 = ae_maxreal(s2, ae_fabs(barycentriccalc(&b1, v2, _state)-barycentriccalc(&b1, v1, _state), _state)/ae_fabs(v2-v1, _state), _state);
                }
                bcerrors = bcerrors||(ae_fp_greater(s2,lipschitztol*s1)&&ae_fp_greater(s1,threshold*k));
            }
            
            /*
             * test differentiation properties
             */
            for(i=0; i<=n-1; i++)
            {
                t = 2*ae_randomreal(_state)-1;
                testratintunit_poldiff2(&x, &y, n, t, &v0, &v1, &v2, _state);
                d0 = (double)(0);
                d1 = (double)(0);
                d2 = (double)(0);
                barycentricdiff1(&b1, t, &d0, &d1, _state);
                bcerrors = bcerrors||ae_fp_greater(ae_fabs(v0-d0, _state),threshold*ae_maxreal(ae_fabs(v0, _state), (double)(1), _state));
                bcerrors = bcerrors||ae_fp_greater(ae_fabs(v1-d1, _state),threshold*ae_maxreal(ae_fabs(v1, _state), (double)(1), _state));
                d0 = (double)(0);
                d1 = (double)(0);
                d2 = (double)(0);
                barycentricdiff2(&b1, t, &d0, &d1, &d2, _state);
                bcerrors = bcerrors||ae_fp_greater(ae_fabs(v0-d0, _state),threshold*ae_maxreal(ae_fabs(v0, _state), (double)(1), _state));
                bcerrors = bcerrors||ae_fp_greater(ae_fabs(v1-d1, _state),threshold*ae_maxreal(ae_fabs(v1, _state), (double)(1), _state));
                bcerrors = bcerrors||ae_fp_greater(ae_fabs(v2-d2, _state),ae_sqrt(threshold, _state)*ae_maxreal(ae_fabs(v2, _state), (double)(1), _state));
            }
            
            /*
             * test linear translation
             */
            t = 2*ae_randomreal(_state)-1;
            a = 2*ae_randomreal(_state)-1;
            b = 2*ae_randomreal(_state)-1;
            testratintunit_brcunset(&b2, _state);
            barycentriccopy(&b1, &b2, _state);
            barycentriclintransx(&b2, a, b, _state);
            bcerrors = bcerrors||ae_fp_greater(ae_fabs(barycentriccalc(&b1, a*t+b, _state)-barycentriccalc(&b2, t, _state), _state),threshold);
            a = (double)(0);
            b = 2*ae_randomreal(_state)-1;
            testratintunit_brcunset(&b2, _state);
            barycentriccopy(&b1, &b2, _state);
            barycentriclintransx(&b2, a, b, _state);
            bcerrors = bcerrors||ae_fp_greater(ae_fabs(barycentriccalc(&b1, a*t+b, _state)-barycentriccalc(&b2, t, _state), _state),threshold);
            a = 2*ae_randomreal(_state)-1;
            b = 2*ae_randomreal(_state)-1;
            testratintunit_brcunset(&b2, _state);
            barycentriccopy(&b1, &b2, _state);
            barycentriclintransy(&b2, a, b, _state);
            bcerrors = bcerrors||ae_fp_greater(ae_fabs(a*barycentriccalc(&b1, t, _state)+b-barycentriccalc(&b2, t, _state), _state),threshold);
        }
    }
    for(pass=0; pass<=3; pass++)
    {
        
        /*
         * Crash-test: small numbers, large numbers
         */
        ae_vector_set_length(&x, 4, _state);
        ae_vector_set_length(&y, 4, _state);
        ae_vector_set_length(&w, 4, _state);
        h = (double)(1);
        if( pass%2==0 )
        {
            h = 100*ae_minrealnumber;
        }
        if( pass%2==1 )
        {
            h = 0.01*ae_maxrealnumber;
        }
        x.ptr.p_double[0] = 0*h;
        x.ptr.p_double[1] = 1*h;
        x.ptr.p_double[2] = 2*h;
        x.ptr.p_double[3] = 3*h;
        y.ptr.p_double[0] = 0*h;
        y.ptr.p_double[1] = 1*h;
        y.ptr.p_double[2] = 2*h;
        y.ptr.p_double[3] = 3*h;
        w.ptr.p_double[0] = -1/(x.ptr.p_double[1]-x.ptr.p_double[0]);
        w.ptr.p_double[1] = 1*(1/(x.ptr.p_double[1]-x.ptr.p_double[0])+1/(x.ptr.p_double[2]-x.ptr.p_double[1]));
        w.ptr.p_double[2] = -1*(1/(x.ptr.p_double[2]-x.ptr.p_double[1])+1/(x.ptr.p_double[3]-x.ptr.p_double[2]));
        w.ptr.p_double[3] = 1/(x.ptr.p_double[3]-x.ptr.p_double[2]);
        v0 = (double)(0);
        if( pass/2==0 )
        {
            v0 = (double)(0);
        }
        if( pass/2==1 )
        {
            v0 = 0.6*h;
        }
        barycentricbuildxyw(&x, &y, &w, 4, &b1, _state);
        t = barycentriccalc(&b1, v0, _state);
        d0 = (double)(0);
        d1 = (double)(0);
        d2 = (double)(0);
        barycentricdiff1(&b1, v0, &d0, &d1, _state);
        bcerrors = bcerrors||ae_fp_greater(ae_fabs(t-v0, _state),threshold*v0);
        bcerrors = bcerrors||ae_fp_greater(ae_fabs(d0-v0, _state),threshold*v0);
        bcerrors = bcerrors||ae_fp_greater(ae_fabs(d1-1, _state),1000*threshold);
    }
    
    /*
     * crash test: large abscissas, small argument
     *
     * test for errors in D0 is not very strict
     * because renormalization used in Diff1()
     * destroys part of precision.
     */
    ae_vector_set_length(&x, 4, _state);
    ae_vector_set_length(&y, 4, _state);
    ae_vector_set_length(&w, 4, _state);
    h = 0.01*ae_maxrealnumber;
    x.ptr.p_double[0] = 0*h;
    x.ptr.p_double[1] = 1*h;
    x.ptr.p_double[2] = 2*h;
    x.ptr.p_double[3] = 3*h;
    y.ptr.p_double[0] = 0*h;
    y.ptr.p_double[1] = 1*h;
    y.ptr.p_double[2] = 2*h;
    y.ptr.p_double[3] = 3*h;
    w.ptr.p_double[0] = -1/(x.ptr.p_double[1]-x.ptr.p_double[0]);
    w.ptr.p_double[1] = 1*(1/(x.ptr.p_double[1]-x.ptr.p_double[0])+1/(x.ptr.p_double[2]-x.ptr.p_double[1]));
    w.ptr.p_double[2] = -1*(1/(x.ptr.p_double[2]-x.ptr.p_double[1])+1/(x.ptr.p_double[3]-x.ptr.p_double[2]));
    w.ptr.p_double[3] = 1/(x.ptr.p_double[3]-x.ptr.p_double[2]);
    v0 = 100*ae_minrealnumber;
    barycentricbuildxyw(&x, &y, &w, 4, &b1, _state);
    t = barycentriccalc(&b1, v0, _state);
    d0 = (double)(0);
    d1 = (double)(0);
    d2 = (double)(0);
    barycentricdiff1(&b1, v0, &d0, &d1, _state);
    bcerrors = bcerrors||ae_fp_greater(ae_fabs(t, _state),v0*(1+threshold));
    bcerrors = bcerrors||ae_fp_greater(ae_fabs(d0, _state),v0*(1+threshold));
    bcerrors = bcerrors||ae_fp_greater(ae_fabs(d1-1, _state),1000*threshold);
    
    /*
     * crash test: test safe barycentric formula
     */
    ae_vector_set_length(&x, 4, _state);
    ae_vector_set_length(&y, 4, _state);
    ae_vector_set_length(&w, 4, _state);
    h = 2*ae_minrealnumber;
    x.ptr.p_double[0] = 0*h;
    x.ptr.p_double[1] = 1*h;
    x.ptr.p_double[2] = 2*h;
    x.ptr.p_double[3] = 3*h;
    y.ptr.p_double[0] = 0*h;
    y.ptr.p_double[1] = 1*h;
    y.ptr.p_double[2] = 2*h;
    y.ptr.p_double[3] = 3*h;
    w.ptr.p_double[0] = -1/(x.ptr.p_double[1]-x.ptr.p_double[0]);
    w.ptr.p_double[1] = 1*(1/(x.ptr.p_double[1]-x.ptr.p_double[0])+1/(x.ptr.p_double[2]-x.ptr.p_double[1]));
    w.ptr.p_double[2] = -1*(1/(x.ptr.p_double[2]-x.ptr.p_double[1])+1/(x.ptr.p_double[3]-x.ptr.p_double[2]));
    w.ptr.p_double[3] = 1/(x.ptr.p_double[3]-x.ptr.p_double[2]);
    v0 = ae_minrealnumber;
    barycentricbuildxyw(&x, &y, &w, 4, &b1, _state);
    t = barycentriccalc(&b1, v0, _state);
    bcerrors = bcerrors||ae_fp_greater(ae_fabs(t-v0, _state)/v0,threshold);
    
    /*
     * Testing "No Poles" interpolation
     */
    maxerr = (double)(0);
    for(pass=1; pass<=passcount-1; pass++)
    {
        ae_vector_set_length(&x, 1, _state);
        ae_vector_set_length(&y, 1, _state);
        x.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
        y.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
        barycentricbuildfloaterhormann(&x, &y, 1, 1, &b1, _state);
        maxerr = ae_maxreal(maxerr, ae_fabs(barycentriccalc(&b1, 2*ae_randomreal(_state)-1, _state)-y.ptr.p_double[0], _state), _state);
    }
    for(n=2; n<=10; n++)
    {
        
        /*
         * compare interpolant built by subroutine
         * with interpolant built by hands
         */
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&y, n, _state);
        ae_vector_set_length(&w, n, _state);
        ae_vector_set_length(&w2, n, _state);
        
        /*
         * D=1, non-equidistant nodes
         */
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * Initialize X, Y, W
             */
            a = -1-1*ae_randomreal(_state);
            b = 1+1*ae_randomreal(_state);
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = ae_atan((b-a)*i/(n-1)+a, _state);
            }
            for(i=0; i<=n-1; i++)
            {
                y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            w.ptr.p_double[0] = -1/(x.ptr.p_double[1]-x.ptr.p_double[0]);
            s = (double)(1);
            for(i=1; i<=n-2; i++)
            {
                w.ptr.p_double[i] = s*(1/(x.ptr.p_double[i]-x.ptr.p_double[i-1])+1/(x.ptr.p_double[i+1]-x.ptr.p_double[i]));
                s = -s;
            }
            w.ptr.p_double[n-1] = s/(x.ptr.p_double[n-1]-x.ptr.p_double[n-2]);
            for(i=0; i<=n-1; i++)
            {
                k = ae_randominteger(n, _state);
                if( k!=i )
                {
                    t = x.ptr.p_double[i];
                    x.ptr.p_double[i] = x.ptr.p_double[k];
                    x.ptr.p_double[k] = t;
                    t = y.ptr.p_double[i];
                    y.ptr.p_double[i] = y.ptr.p_double[k];
                    y.ptr.p_double[k] = t;
                    t = w.ptr.p_double[i];
                    w.ptr.p_double[i] = w.ptr.p_double[k];
                    w.ptr.p_double[k] = t;
                }
            }
            
            /*
             * Build and test
             */
            barycentricbuildfloaterhormann(&x, &y, n, 1, &b1, _state);
            barycentricbuildxyw(&x, &y, &w, n, &b2, _state);
            for(i=1; i<=2*n; i++)
            {
                t = a+(b-a)*ae_randomreal(_state);
                maxerr = ae_maxreal(maxerr, ae_fabs(barycentriccalc(&b1, t, _state)-barycentriccalc(&b2, t, _state), _state), _state);
            }
        }
        
        /*
         * D = 0, 1, 2. Equidistant nodes.
         */
        for(d=0; d<=2; d++)
        {
            for(pass=1; pass<=passcount; pass++)
            {
                
                /*
                 * Skip incorrect (N,D) pairs
                 */
                if( n<2*d )
                {
                    continue;
                }
                
                /*
                 * Initialize X, Y, W
                 */
                a = -1-1*ae_randomreal(_state);
                b = 1+1*ae_randomreal(_state);
                for(i=0; i<=n-1; i++)
                {
                    x.ptr.p_double[i] = (b-a)*i/(n-1)+a;
                }
                for(i=0; i<=n-1; i++)
                {
                    y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                s = (double)(1);
                if( d==0 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        w.ptr.p_double[i] = s;
                        s = -s;
                    }
                }
                if( d==1 )
                {
                    w.ptr.p_double[0] = -s;
                    for(i=1; i<=n-2; i++)
                    {
                        w.ptr.p_double[i] = 2*s;
                        s = -s;
                    }
                    w.ptr.p_double[n-1] = s;
                }
                if( d==2 )
                {
                    w.ptr.p_double[0] = s;
                    w.ptr.p_double[1] = -3*s;
                    for(i=2; i<=n-3; i++)
                    {
                        w.ptr.p_double[i] = 4*s;
                        s = -s;
                    }
                    w.ptr.p_double[n-2] = 3*s;
                    w.ptr.p_double[n-1] = -s;
                }
                
                /*
                 * Mix
                 */
                for(i=0; i<=n-1; i++)
                {
                    k = ae_randominteger(n, _state);
                    if( k!=i )
                    {
                        t = x.ptr.p_double[i];
                        x.ptr.p_double[i] = x.ptr.p_double[k];
                        x.ptr.p_double[k] = t;
                        t = y.ptr.p_double[i];
                        y.ptr.p_double[i] = y.ptr.p_double[k];
                        y.ptr.p_double[k] = t;
                        t = w.ptr.p_double[i];
                        w.ptr.p_double[i] = w.ptr.p_double[k];
                        w.ptr.p_double[k] = t;
                    }
                }
                
                /*
                 * Build and test
                 */
                barycentricbuildfloaterhormann(&x, &y, n, d, &b1, _state);
                barycentricbuildxyw(&x, &y, &w, n, &b2, _state);
                for(i=1; i<=2*n; i++)
                {
                    t = a+(b-a)*ae_randomreal(_state);
                    maxerr = ae_maxreal(maxerr, ae_fabs(barycentriccalc(&b1, t, _state)-barycentriccalc(&b2, t, _state), _state), _state);
                }
            }
        }
    }
    if( ae_fp_greater(maxerr,threshold) )
    {
        nperrors = ae_true;
    }
    
    /*
     * report
     */
    waserrors = bcerrors||nperrors;
    if( !silent )
    {
        printf("TESTING RATIONAL INTERPOLATION\n");
        printf("BASIC BARYCENTRIC FUNCTIONS:             ");
        if( bcerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("FLOATER-HORMANN:                         ");
        if( nperrors )
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
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testratint(ae_bool silent, ae_state *_state)
{
    return testratint(silent, _state);
}


static void testratintunit_poldiff2(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* f,
     ae_int_t n,
     double t,
     double* p,
     double* dp,
     double* d2p,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _f;
    ae_int_t m;
    ae_int_t i;
    ae_vector df;
    ae_vector d2f;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_f, f, _state);
    f = &_f;
    *p = 0;
    *dp = 0;
    *d2p = 0;
    ae_vector_init(&df, 0, DT_REAL, _state);
    ae_vector_init(&d2f, 0, DT_REAL, _state);

    n = n-1;
    ae_vector_set_length(&df, n+1, _state);
    ae_vector_set_length(&d2f, n+1, _state);
    for(i=0; i<=n; i++)
    {
        d2f.ptr.p_double[i] = (double)(0);
        df.ptr.p_double[i] = (double)(0);
    }
    for(m=1; m<=n; m++)
    {
        for(i=0; i<=n-m; i++)
        {
            d2f.ptr.p_double[i] = ((t-x->ptr.p_double[i+m])*d2f.ptr.p_double[i]+(x->ptr.p_double[i]-t)*d2f.ptr.p_double[i+1]+2*df.ptr.p_double[i]-2*df.ptr.p_double[i+1])/(x->ptr.p_double[i]-x->ptr.p_double[i+m]);
            df.ptr.p_double[i] = ((t-x->ptr.p_double[i+m])*df.ptr.p_double[i]+f->ptr.p_double[i]+(x->ptr.p_double[i]-t)*df.ptr.p_double[i+1]-f->ptr.p_double[i+1])/(x->ptr.p_double[i]-x->ptr.p_double[i+m]);
            f->ptr.p_double[i] = ((t-x->ptr.p_double[i+m])*f->ptr.p_double[i]+(x->ptr.p_double[i]-t)*f->ptr.p_double[i+1])/(x->ptr.p_double[i]-x->ptr.p_double[i+m]);
        }
    }
    *p = f->ptr.p_double[0];
    *dp = df.ptr.p_double[0];
    *d2p = d2f.ptr.p_double[0];
    ae_frame_leave(_state);
}


static void testratintunit_brcunset(barycentricinterpolant* b,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_vector w;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);

    ae_vector_set_length(&x, 1, _state);
    ae_vector_set_length(&y, 1, _state);
    ae_vector_set_length(&w, 1, _state);
    x.ptr.p_double[0] = (double)(0);
    y.ptr.p_double[0] = (double)(0);
    w.ptr.p_double[0] = (double)(1);
    barycentricbuildxyw(&x, &y, &w, 1, b, _state);
    ae_frame_leave(_state);
}


/*$ End $*/
