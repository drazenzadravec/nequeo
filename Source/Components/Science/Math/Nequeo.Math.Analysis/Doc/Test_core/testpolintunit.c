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
#include "testpolintunit.h"


/*$ Declarations $*/
static double testpolintunit_internalpolint(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* f,
     ae_int_t n,
     double t,
     ae_state *_state);
static void testpolintunit_brcunset(barycentricinterpolant* b,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Unit test
*************************************************************************/
ae_bool testpolint(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_bool interrors;
    double threshold;
    ae_vector x;
    ae_vector y;
    ae_vector w;
    ae_vector c;
    ae_vector c0;
    ae_vector c1;
    ae_vector c2;
    ae_vector x2;
    ae_vector y2;
    ae_vector w2;
    ae_vector xfull;
    ae_vector yfull;
    double a;
    double b;
    double t;
    ae_int_t i;
    ae_int_t k;
    ae_vector xc;
    ae_vector yc;
    ae_vector dc;
    double v;
    double v0;
    double v1;
    double v2;
    double v3;
    double v4;
    double pscale;
    double poffset;
    double eps;
    barycentricinterpolant p;
    barycentricinterpolant p1;
    barycentricinterpolant p2;
    ae_int_t n;
    ae_int_t maxn;
    ae_int_t pass;
    ae_int_t passcount;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&c, 0, DT_REAL, _state);
    ae_vector_init(&c0, 0, DT_REAL, _state);
    ae_vector_init(&c1, 0, DT_REAL, _state);
    ae_vector_init(&c2, 0, DT_REAL, _state);
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

    waserrors = ae_false;
    interrors = ae_false;
    maxn = 5;
    passcount = 20;
    threshold = 1.0E8*ae_machineepsilon;
    
    /*
     * Test equidistant interpolation
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            
            /*
             * prepare task:
             * * equidistant points
             * * random Y
             * * T in [A,B] or near (within 10% of its width)
             */
            do
            {
                a = 2*ae_randomreal(_state)-1;
                b = 2*ae_randomreal(_state)-1;
            }
            while(ae_fp_less_eq(ae_fabs(a-b, _state),0.2));
            t = a+(1.2*ae_randomreal(_state)-0.1)*(b-a);
            taskgenint1dequidist(a, b, n, &x, &y, _state);
            
            /*
             * test "fast" equidistant interpolation (no barycentric model)
             */
            interrors = interrors||ae_fp_greater(ae_fabs(polynomialcalceqdist(a, b, &y, n, t, _state)-testpolintunit_internalpolint(&x, &y, n, t, _state), _state),threshold);
            
            /*
             * test "slow" equidistant interpolation (create barycentric model)
             */
            testpolintunit_brcunset(&p, _state);
            polynomialbuild(&x, &y, n, &p, _state);
            interrors = interrors||ae_fp_greater(ae_fabs(barycentriccalc(&p, t, _state)-testpolintunit_internalpolint(&x, &y, n, t, _state), _state),threshold);
            
            /*
             * test "fast" interpolation (create "fast" barycentric model)
             */
            testpolintunit_brcunset(&p, _state);
            polynomialbuildeqdist(a, b, &y, n, &p, _state);
            interrors = interrors||ae_fp_greater(ae_fabs(barycentriccalc(&p, t, _state)-testpolintunit_internalpolint(&x, &y, n, t, _state), _state),threshold);
        }
    }
    
    /*
     * Test Chebyshev-1 interpolation
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            
            /*
             * prepare task:
             * * equidistant points
             * * random Y
             * * T in [A,B] or near (within 10% of its width)
             */
            do
            {
                a = 2*ae_randomreal(_state)-1;
                b = 2*ae_randomreal(_state)-1;
            }
            while(ae_fp_less_eq(ae_fabs(a-b, _state),0.2));
            t = a+(1.2*ae_randomreal(_state)-0.1)*(b-a);
            taskgenint1dcheb1(a, b, n, &x, &y, _state);
            
            /*
             * test "fast" interpolation (no barycentric model)
             */
            interrors = interrors||ae_fp_greater(ae_fabs(polynomialcalccheb1(a, b, &y, n, t, _state)-testpolintunit_internalpolint(&x, &y, n, t, _state), _state),threshold);
            
            /*
             * test "slow" interpolation (create barycentric model)
             */
            testpolintunit_brcunset(&p, _state);
            polynomialbuild(&x, &y, n, &p, _state);
            interrors = interrors||ae_fp_greater(ae_fabs(barycentriccalc(&p, t, _state)-testpolintunit_internalpolint(&x, &y, n, t, _state), _state),threshold);
            
            /*
             * test "fast" interpolation (create "fast" barycentric model)
             */
            testpolintunit_brcunset(&p, _state);
            polynomialbuildcheb1(a, b, &y, n, &p, _state);
            interrors = interrors||ae_fp_greater(ae_fabs(barycentriccalc(&p, t, _state)-testpolintunit_internalpolint(&x, &y, n, t, _state), _state),threshold);
        }
    }
    
    /*
     * Test Chebyshev-2 interpolation
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            
            /*
             * prepare task:
             * * equidistant points
             * * random Y
             * * T in [A,B] or near (within 10% of its width)
             */
            do
            {
                a = 2*ae_randomreal(_state)-1;
                b = 2*ae_randomreal(_state)-1;
            }
            while(ae_fp_less_eq(ae_fabs(a-b, _state),0.2));
            t = a+(1.2*ae_randomreal(_state)-0.1)*(b-a);
            taskgenint1dcheb2(a, b, n, &x, &y, _state);
            
            /*
             * test "fast" interpolation (no barycentric model)
             */
            interrors = interrors||ae_fp_greater(ae_fabs(polynomialcalccheb2(a, b, &y, n, t, _state)-testpolintunit_internalpolint(&x, &y, n, t, _state), _state),threshold);
            
            /*
             * test "slow" interpolation (create barycentric model)
             */
            testpolintunit_brcunset(&p, _state);
            polynomialbuild(&x, &y, n, &p, _state);
            interrors = interrors||ae_fp_greater(ae_fabs(barycentriccalc(&p, t, _state)-testpolintunit_internalpolint(&x, &y, n, t, _state), _state),threshold);
            
            /*
             * test "fast" interpolation (create "fast" barycentric model)
             */
            testpolintunit_brcunset(&p, _state);
            polynomialbuildcheb2(a, b, &y, n, &p, _state);
            interrors = interrors||ae_fp_greater(ae_fabs(barycentriccalc(&p, t, _state)-testpolintunit_internalpolint(&x, &y, n, t, _state), _state),threshold);
        }
    }
    
    /*
     * Testing conversion Barycentric<->Chebyshev
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(k=1; k<=3; k++)
        {
            
            /*
             * Allocate
             */
            ae_vector_set_length(&x, k, _state);
            ae_vector_set_length(&y, k, _state);
            
            /*
             * Generate problem
             */
            a = 2*ae_randomreal(_state)-1;
            b = a+(0.1+ae_randomreal(_state))*(2*ae_randominteger(2, _state)-1);
            v0 = 2*ae_randomreal(_state)-1;
            v1 = 2*ae_randomreal(_state)-1;
            v2 = 2*ae_randomreal(_state)-1;
            if( k==1 )
            {
                x.ptr.p_double[0] = 0.5*(a+b);
                y.ptr.p_double[0] = v0;
            }
            if( k==2 )
            {
                x.ptr.p_double[0] = a;
                y.ptr.p_double[0] = v0-v1;
                x.ptr.p_double[1] = b;
                y.ptr.p_double[1] = v0+v1;
            }
            if( k==3 )
            {
                x.ptr.p_double[0] = a;
                y.ptr.p_double[0] = v0-v1+v2;
                x.ptr.p_double[1] = 0.5*(a+b);
                y.ptr.p_double[1] = v0-v2;
                x.ptr.p_double[2] = b;
                y.ptr.p_double[2] = v0+v1+v2;
            }
            
            /*
             * Test forward conversion
             */
            polynomialbuild(&x, &y, k, &p, _state);
            ae_vector_set_length(&c, 1, _state);
            polynomialbar2cheb(&p, a, b, &c, _state);
            interrors = interrors||c.cnt!=k;
            if( k>=1 )
            {
                interrors = interrors||ae_fp_greater(ae_fabs(c.ptr.p_double[0]-v0, _state),threshold);
            }
            if( k>=2 )
            {
                interrors = interrors||ae_fp_greater(ae_fabs(c.ptr.p_double[1]-v1, _state),threshold);
            }
            if( k>=3 )
            {
                interrors = interrors||ae_fp_greater(ae_fabs(c.ptr.p_double[2]-v2, _state),threshold);
            }
            
            /*
             * Test backward conversion
             */
            polynomialcheb2bar(&c, k, a, b, &p2, _state);
            v = a+ae_randomreal(_state)*(b-a);
            interrors = interrors||ae_fp_greater(ae_fabs(barycentriccalc(&p, v, _state)-barycentriccalc(&p2, v, _state), _state),threshold);
        }
    }
    
    /*
     * Testing conversion Barycentric<->Power
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(k=1; k<=5; k++)
        {
            
            /*
             * Allocate
             */
            ae_vector_set_length(&x, k, _state);
            ae_vector_set_length(&y, k, _state);
            
            /*
             * Generate problem
             */
            poffset = 2*ae_randomreal(_state)-1;
            pscale = (0.1+ae_randomreal(_state))*(2*ae_randominteger(2, _state)-1);
            v0 = 2*ae_randomreal(_state)-1;
            v1 = 2*ae_randomreal(_state)-1;
            v2 = 2*ae_randomreal(_state)-1;
            v3 = 2*ae_randomreal(_state)-1;
            v4 = 2*ae_randomreal(_state)-1;
            if( k==1 )
            {
                x.ptr.p_double[0] = poffset;
                y.ptr.p_double[0] = v0;
            }
            if( k==2 )
            {
                x.ptr.p_double[0] = poffset-pscale;
                y.ptr.p_double[0] = v0-v1;
                x.ptr.p_double[1] = poffset+pscale;
                y.ptr.p_double[1] = v0+v1;
            }
            if( k==3 )
            {
                x.ptr.p_double[0] = poffset-pscale;
                y.ptr.p_double[0] = v0-v1+v2;
                x.ptr.p_double[1] = poffset;
                y.ptr.p_double[1] = v0;
                x.ptr.p_double[2] = poffset+pscale;
                y.ptr.p_double[2] = v0+v1+v2;
            }
            if( k==4 )
            {
                x.ptr.p_double[0] = poffset-pscale;
                y.ptr.p_double[0] = v0-v1+v2-v3;
                x.ptr.p_double[1] = poffset-0.5*pscale;
                y.ptr.p_double[1] = v0-0.5*v1+0.25*v2-0.125*v3;
                x.ptr.p_double[2] = poffset+0.5*pscale;
                y.ptr.p_double[2] = v0+0.5*v1+0.25*v2+0.125*v3;
                x.ptr.p_double[3] = poffset+pscale;
                y.ptr.p_double[3] = v0+v1+v2+v3;
            }
            if( k==5 )
            {
                x.ptr.p_double[0] = poffset-pscale;
                y.ptr.p_double[0] = v0-v1+v2-v3+v4;
                x.ptr.p_double[1] = poffset-0.5*pscale;
                y.ptr.p_double[1] = v0-0.5*v1+0.25*v2-0.125*v3+0.0625*v4;
                x.ptr.p_double[2] = poffset;
                y.ptr.p_double[2] = v0;
                x.ptr.p_double[3] = poffset+0.5*pscale;
                y.ptr.p_double[3] = v0+0.5*v1+0.25*v2+0.125*v3+0.0625*v4;
                x.ptr.p_double[4] = poffset+pscale;
                y.ptr.p_double[4] = v0+v1+v2+v3+v4;
            }
            
            /*
             * Test forward conversion
             */
            polynomialbuild(&x, &y, k, &p, _state);
            ae_vector_set_length(&c, 1, _state);
            polynomialbar2pow(&p, poffset, pscale, &c, _state);
            interrors = interrors||c.cnt!=k;
            if( k>=1 )
            {
                interrors = interrors||ae_fp_greater(ae_fabs(c.ptr.p_double[0]-v0, _state),threshold);
            }
            if( k>=2 )
            {
                interrors = interrors||ae_fp_greater(ae_fabs(c.ptr.p_double[1]-v1, _state),threshold);
            }
            if( k>=3 )
            {
                interrors = interrors||ae_fp_greater(ae_fabs(c.ptr.p_double[2]-v2, _state),threshold);
            }
            if( k>=4 )
            {
                interrors = interrors||ae_fp_greater(ae_fabs(c.ptr.p_double[3]-v3, _state),threshold);
            }
            if( k>=5 )
            {
                interrors = interrors||ae_fp_greater(ae_fabs(c.ptr.p_double[4]-v4, _state),threshold);
            }
            
            /*
             * Test backward conversion
             */
            polynomialpow2bar(&c, k, poffset, pscale, &p2, _state);
            v = poffset+(2*ae_randomreal(_state)-1)*pscale;
            interrors = interrors||ae_fp_greater(ae_fabs(barycentriccalc(&p, v, _state)-barycentriccalc(&p2, v, _state), _state),threshold);
        }
    }
    
    /*
     * crash-test: ability to solve tasks which will overflow/underflow
     * weights with straightforward implementation
     */
    for(n=1; n<=20; n++)
    {
        a = -0.1*ae_maxrealnumber;
        b = 0.1*ae_maxrealnumber;
        taskgenint1dequidist(a, b, n, &x, &y, _state);
        polynomialbuild(&x, &y, n, &p, _state);
        for(i=0; i<=n-1; i++)
        {
            interrors = interrors||ae_fp_eq(p.w.ptr.p_double[i],(double)(0));
        }
    }
    
    /*
     * Test issue #634: instability in PolynomialBar2Pow().
     *
     * Function returns incorrect coefficients when called with
     * approximately-unit scale for data which have significantly
     * non-unit scale.
     */
    n = 7;
    eps = 1.0E-8;
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&x2, n, _state);
    ae_vector_set_length(&y, n, _state);
    x.ptr.p_double[0] = ae_randomreal(_state)-0.5;
    y.ptr.p_double[0] = ae_randomreal(_state)-0.5;
    for(i=1; i<=n-1; i++)
    {
        x.ptr.p_double[i] = x.ptr.p_double[i-1]+ae_randomreal(_state)+0.1;
        y.ptr.p_double[i] = ae_randomreal(_state)-0.5;
    }
    polynomialbuild(&x, &y, n, &p, _state);
    polynomialbar2pow(&p, 0.0, 1.0, &c0, _state);
    pscale = 1.0E-10;
    for(i=0; i<=n-1; i++)
    {
        x2.ptr.p_double[i] = x.ptr.p_double[i]*pscale;
    }
    polynomialbuild(&x2, &y, n, &p, _state);
    polynomialbar2pow(&p, 0.0, 1.0, &c1, _state);
    for(i=0; i<=n-1; i++)
    {
        seterrorflag(&interrors, ae_fp_greater(ae_fabs(c0.ptr.p_double[i]-c1.ptr.p_double[i]*ae_pow(pscale, (double)(i), _state), _state),eps), _state);
    }
    pscale = 1.0E10;
    for(i=0; i<=n-1; i++)
    {
        x2.ptr.p_double[i] = x.ptr.p_double[i]*pscale;
    }
    polynomialbuild(&x2, &y, n, &p, _state);
    polynomialbar2pow(&p, 0.0, 1.0, &c2, _state);
    for(i=0; i<=n-1; i++)
    {
        seterrorflag(&interrors, ae_fp_greater(ae_fabs(c0.ptr.p_double[i]-c2.ptr.p_double[i]*ae_pow(pscale, (double)(i), _state), _state),eps), _state);
    }
    
    /*
     * report
     */
    waserrors = interrors;
    if( !silent )
    {
        printf("TESTING POLYNOMIAL INTERPOLATION\n");
        
        /*
         * Normal tests
         */
        printf("INTERPOLATION TEST:                      ");
        if( interrors )
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
ae_bool _pexec_testpolint(ae_bool silent, ae_state *_state)
{
    return testpolint(silent, _state);
}


static double testpolintunit_internalpolint(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* f,
     ae_int_t n,
     double t,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _f;
    ae_int_t i;
    ae_int_t j;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_f, f, _state);
    f = &_f;

    n = n-1;
    for(j=0; j<=n-1; j++)
    {
        for(i=j+1; i<=n; i++)
        {
            f->ptr.p_double[i] = ((t-x->ptr.p_double[j])*f->ptr.p_double[i]-(t-x->ptr.p_double[i])*f->ptr.p_double[j])/(x->ptr.p_double[i]-x->ptr.p_double[j]);
        }
    }
    result = f->ptr.p_double[n];
    ae_frame_leave(_state);
    return result;
}


static void testpolintunit_brcunset(barycentricinterpolant* b,
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
