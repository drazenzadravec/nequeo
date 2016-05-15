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
#include "testspline1dunit.h"


/*$ Declarations $*/
static void testspline1dunit_lconst(double a,
     double b,
     spline1dinterpolant* c,
     double lstep,
     double* l0,
     double* l1,
     double* l2,
     ae_state *_state);
static ae_bool testspline1dunit_enumerateallsplines(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t* splineindex,
     spline1dinterpolant* s,
     ae_state *_state);
static ae_bool testspline1dunit_testunpack(spline1dinterpolant* c,
     /* Real    */ ae_vector* x,
     ae_state *_state);
static void testspline1dunit_unsetspline1d(spline1dinterpolant* c,
     ae_state *_state);
static ae_bool testspline1dunit_testmonotonespline(ae_state *_state);


/*$ Body $*/


ae_bool testspline1d(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_bool crserrors;
    ae_bool cserrors;
    ae_bool hserrors;
    ae_bool aserrors;
    ae_bool lserrors;
    ae_bool dserrors;
    ae_bool uperrors;
    ae_bool cperrors;
    ae_bool lterrors;
    ae_bool ierrors;
    ae_bool monotoneerr;
    double nonstrictthreshold;
    double threshold;
    ae_int_t passcount;
    double lstep;
    double h;
    ae_int_t maxn;
    ae_int_t bltype;
    ae_int_t brtype;
    ae_bool periodiccond;
    ae_int_t n;
    ae_int_t i;
    ae_int_t k;
    ae_int_t pass;
    ae_vector x;
    ae_vector y;
    ae_vector yp;
    ae_vector w;
    ae_vector w2;
    ae_vector y2;
    ae_vector d;
    ae_vector xc;
    ae_vector yc;
    ae_vector xtest;
    ae_int_t n2;
    ae_vector tmp0;
    ae_vector tmp1;
    ae_vector tmp2;
    ae_vector tmpx;
    ae_vector dc;
    spline1dinterpolant c;
    spline1dinterpolant c2;
    double a;
    double b;
    double bl;
    double br;
    double t;
    double sa;
    double sb;
    double v;
    double l10;
    double l11;
    double l12;
    double l20;
    double l21;
    double l22;
    double p0;
    double p1;
    double p2;
    double s;
    double ds;
    double d2s;
    double s2;
    double ds2;
    double d2s2;
    double vl;
    double vm;
    double vr;
    double err;
    double tension;
    double intab;
    ae_int_t splineindex;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&yp, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&yc, 0, DT_REAL, _state);
    ae_vector_init(&xtest, 0, DT_REAL, _state);
    ae_vector_init(&tmp0, 0, DT_REAL, _state);
    ae_vector_init(&tmp1, 0, DT_REAL, _state);
    ae_vector_init(&tmp2, 0, DT_REAL, _state);
    ae_vector_init(&tmpx, 0, DT_REAL, _state);
    ae_vector_init(&dc, 0, DT_INT, _state);
    _spline1dinterpolant_init(&c, _state);
    _spline1dinterpolant_init(&c2, _state);

    waserrors = ae_false;
    passcount = 20;
    lstep = 0.005;
    h = 0.00001;
    maxn = 10;
    threshold = 10000*ae_machineepsilon;
    nonstrictthreshold = 0.00001;
    lserrors = ae_false;
    cserrors = ae_false;
    crserrors = ae_false;
    hserrors = ae_false;
    aserrors = ae_false;
    dserrors = ae_false;
    cperrors = ae_false;
    uperrors = ae_false;
    lterrors = ae_false;
    ierrors = ae_false;
    
    /*
     * General test: linear, cubic, Hermite, Akima
     */
    for(n=2; n<=maxn; n++)
    {
        ae_vector_set_length(&x, n-1+1, _state);
        ae_vector_set_length(&y, n-1+1, _state);
        ae_vector_set_length(&yp, n-1+1, _state);
        ae_vector_set_length(&d, n-1+1, _state);
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * Prepare task:
             * * X contains abscissas from [A,B]
             * * Y contains function values
             * * YP contains periodic function values
             */
            a = -1-ae_randomreal(_state);
            b = 1+ae_randomreal(_state);
            bl = 2*ae_randomreal(_state)-1;
            br = 2*ae_randomreal(_state)-1;
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 0.5*(b+a)+0.5*(b-a)*ae_cos(ae_pi*(2*i+1)/(2*n), _state);
                if( i==0 )
                {
                    x.ptr.p_double[i] = a;
                }
                if( i==n-1 )
                {
                    x.ptr.p_double[i] = b;
                }
                y.ptr.p_double[i] = ae_cos(1.3*ae_pi*x.ptr.p_double[i]+0.4, _state);
                yp.ptr.p_double[i] = y.ptr.p_double[i];
                d.ptr.p_double[i] = -1.3*ae_pi*ae_sin(1.3*ae_pi*x.ptr.p_double[i]+0.4, _state);
            }
            yp.ptr.p_double[n-1] = yp.ptr.p_double[0];
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
                    t = yp.ptr.p_double[i];
                    yp.ptr.p_double[i] = yp.ptr.p_double[k];
                    yp.ptr.p_double[k] = t;
                    t = d.ptr.p_double[i];
                    d.ptr.p_double[i] = d.ptr.p_double[k];
                    d.ptr.p_double[k] = t;
                }
            }
            
            /*
             * Build linear spline
             * Test for general interpolation scheme properties:
             * * values at nodes
             * * continuous function
             * Test for specific properties is implemented below.
             */
            spline1dbuildlinear(&x, &y, n, &c, _state);
            err = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                err = ae_maxreal(err, ae_fabs(y.ptr.p_double[i]-spline1dcalc(&c, x.ptr.p_double[i], _state), _state), _state);
            }
            lserrors = lserrors||ae_fp_greater(err,threshold);
            testspline1dunit_lconst(a, b, &c, lstep, &l10, &l11, &l12, _state);
            testspline1dunit_lconst(a, b, &c, lstep/3, &l20, &l21, &l22, _state);
            lserrors = lserrors||ae_fp_greater(l20/l10,1.2);
            
            /*
             * Build cubic spline.
             * Test for interpolation scheme properties:
             * * values at nodes
             * * boundary conditions
             * * continuous function
             * * continuous first derivative
             * * continuous second derivative
             * * periodicity properties
             * * Spline1DGridDiff(), Spline1DGridDiff2() and Spline1DDiff()
             *   calls must return same results
             */
            for(bltype=-1; bltype<=2; bltype++)
            {
                for(brtype=-1; brtype<=2; brtype++)
                {
                    
                    /*
                     * skip meaningless combination of boundary conditions
                     * (one condition is periodic, another is not)
                     */
                    periodiccond = bltype==-1||brtype==-1;
                    if( periodiccond&&bltype!=brtype )
                    {
                        continue;
                    }
                    
                    /*
                     * build
                     */
                    if( periodiccond )
                    {
                        spline1dbuildcubic(&x, &yp, n, bltype, bl, brtype, br, &c, _state);
                    }
                    else
                    {
                        spline1dbuildcubic(&x, &y, n, bltype, bl, brtype, br, &c, _state);
                    }
                    
                    /*
                     * interpolation properties
                     */
                    err = (double)(0);
                    if( periodiccond )
                    {
                        
                        /*
                         * * check values at nodes; spline is periodic so
                         *   we add random number of periods to nodes
                         * * we also test for periodicity of derivatives
                         */
                        for(i=0; i<=n-1; i++)
                        {
                            v = x.ptr.p_double[i];
                            vm = v+(b-a)*(ae_randominteger(5, _state)-2);
                            t = yp.ptr.p_double[i]-spline1dcalc(&c, vm, _state);
                            err = ae_maxreal(err, ae_fabs(t, _state), _state);
                            spline1ddiff(&c, v, &s, &ds, &d2s, _state);
                            spline1ddiff(&c, vm, &s2, &ds2, &d2s2, _state);
                            err = ae_maxreal(err, ae_fabs(s-s2, _state), _state);
                            err = ae_maxreal(err, ae_fabs(ds-ds2, _state), _state);
                            err = ae_maxreal(err, ae_fabs(d2s-d2s2, _state), _state);
                        }
                        
                        /*
                         * periodicity between nodes
                         */
                        v = a+(b-a)*ae_randomreal(_state);
                        vm = v+(b-a)*(ae_randominteger(5, _state)-2);
                        err = ae_maxreal(err, ae_fabs(spline1dcalc(&c, v, _state)-spline1dcalc(&c, vm, _state), _state), _state);
                        spline1ddiff(&c, v, &s, &ds, &d2s, _state);
                        spline1ddiff(&c, vm, &s2, &ds2, &d2s2, _state);
                        err = ae_maxreal(err, ae_fabs(s-s2, _state), _state);
                        err = ae_maxreal(err, ae_fabs(ds-ds2, _state), _state);
                        err = ae_maxreal(err, ae_fabs(d2s-d2s2, _state), _state);
                    }
                    else
                    {
                        
                        /*
                         * * check values at nodes
                         */
                        for(i=0; i<=n-1; i++)
                        {
                            err = ae_maxreal(err, ae_fabs(y.ptr.p_double[i]-spline1dcalc(&c, x.ptr.p_double[i], _state), _state), _state);
                        }
                    }
                    cserrors = cserrors||ae_fp_greater(err,threshold);
                    
                    /*
                     * check boundary conditions
                     */
                    err = (double)(0);
                    if( bltype==0 )
                    {
                        spline1ddiff(&c, a-h, &s, &ds, &d2s, _state);
                        spline1ddiff(&c, a+h, &s2, &ds2, &d2s2, _state);
                        t = (d2s2-d2s)/(2*h);
                        err = ae_maxreal(err, ae_fabs(t, _state), _state);
                    }
                    if( bltype==1 )
                    {
                        t = (spline1dcalc(&c, a+h, _state)-spline1dcalc(&c, a-h, _state))/(2*h);
                        err = ae_maxreal(err, ae_fabs(bl-t, _state), _state);
                    }
                    if( bltype==2 )
                    {
                        t = (spline1dcalc(&c, a+h, _state)-2*spline1dcalc(&c, a, _state)+spline1dcalc(&c, a-h, _state))/ae_sqr(h, _state);
                        err = ae_maxreal(err, ae_fabs(bl-t, _state), _state);
                    }
                    if( brtype==0 )
                    {
                        spline1ddiff(&c, b-h, &s, &ds, &d2s, _state);
                        spline1ddiff(&c, b+h, &s2, &ds2, &d2s2, _state);
                        t = (d2s2-d2s)/(2*h);
                        err = ae_maxreal(err, ae_fabs(t, _state), _state);
                    }
                    if( brtype==1 )
                    {
                        t = (spline1dcalc(&c, b+h, _state)-spline1dcalc(&c, b-h, _state))/(2*h);
                        err = ae_maxreal(err, ae_fabs(br-t, _state), _state);
                    }
                    if( brtype==2 )
                    {
                        t = (spline1dcalc(&c, b+h, _state)-2*spline1dcalc(&c, b, _state)+spline1dcalc(&c, b-h, _state))/ae_sqr(h, _state);
                        err = ae_maxreal(err, ae_fabs(br-t, _state), _state);
                    }
                    if( bltype==-1||brtype==-1 )
                    {
                        spline1ddiff(&c, a+100*ae_machineepsilon, &s, &ds, &d2s, _state);
                        spline1ddiff(&c, b-100*ae_machineepsilon, &s2, &ds2, &d2s2, _state);
                        err = ae_maxreal(err, ae_fabs(s-s2, _state), _state);
                        err = ae_maxreal(err, ae_fabs(ds-ds2, _state), _state);
                        err = ae_maxreal(err, ae_fabs(d2s-d2s2, _state), _state);
                    }
                    cserrors = cserrors||ae_fp_greater(err,1.0E-3);
                    
                    /*
                     * Check Lipschitz continuity
                     */
                    testspline1dunit_lconst(a, b, &c, lstep, &l10, &l11, &l12, _state);
                    testspline1dunit_lconst(a, b, &c, lstep/3, &l20, &l21, &l22, _state);
                    if( ae_fp_greater(l10,1.0E-6) )
                    {
                        cserrors = cserrors||ae_fp_greater(l20/l10,1.2);
                    }
                    if( ae_fp_greater(l11,1.0E-6) )
                    {
                        cserrors = cserrors||ae_fp_greater(l21/l11,1.2);
                    }
                    if( ae_fp_greater(l12,1.0E-6) )
                    {
                        cserrors = cserrors||ae_fp_greater(l22/l12,1.2);
                    }
                    
                    /*
                     * compare spline1dgriddiff() and spline1ddiff() results
                     */
                    err = (double)(0);
                    if( periodiccond )
                    {
                        spline1dgriddiffcubic(&x, &yp, n, bltype, bl, brtype, br, &tmp1, _state);
                    }
                    else
                    {
                        spline1dgriddiffcubic(&x, &y, n, bltype, bl, brtype, br, &tmp1, _state);
                    }
                    ae_assert(tmp1.cnt>=n, "Assertion failed", _state);
                    for(i=0; i<=n-1; i++)
                    {
                        spline1ddiff(&c, x.ptr.p_double[i], &s, &ds, &d2s, _state);
                        err = ae_maxreal(err, ae_fabs(ds-tmp1.ptr.p_double[i], _state), _state);
                    }
                    if( periodiccond )
                    {
                        spline1dgriddiff2cubic(&x, &yp, n, bltype, bl, brtype, br, &tmp1, &tmp2, _state);
                    }
                    else
                    {
                        spline1dgriddiff2cubic(&x, &y, n, bltype, bl, brtype, br, &tmp1, &tmp2, _state);
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        spline1ddiff(&c, x.ptr.p_double[i], &s, &ds, &d2s, _state);
                        err = ae_maxreal(err, ae_fabs(ds-tmp1.ptr.p_double[i], _state), _state);
                        err = ae_maxreal(err, ae_fabs(d2s-tmp2.ptr.p_double[i], _state), _state);
                    }
                    cserrors = cserrors||ae_fp_greater(err,threshold);
                    
                    /*
                     * compare spline1dconv()/convdiff()/convdiff2() and spline1ddiff() results
                     */
                    n2 = 2+ae_randominteger(2*n, _state);
                    ae_vector_set_length(&tmpx, n2, _state);
                    for(i=0; i<=n2-1; i++)
                    {
                        tmpx.ptr.p_double[i] = 0.5*(a+b)+(a-b)*(2*ae_randomreal(_state)-1);
                    }
                    err = (double)(0);
                    if( periodiccond )
                    {
                        spline1dconvcubic(&x, &yp, n, bltype, bl, brtype, br, &tmpx, n2, &tmp0, _state);
                    }
                    else
                    {
                        spline1dconvcubic(&x, &y, n, bltype, bl, brtype, br, &tmpx, n2, &tmp0, _state);
                    }
                    for(i=0; i<=n2-1; i++)
                    {
                        spline1ddiff(&c, tmpx.ptr.p_double[i], &s, &ds, &d2s, _state);
                        err = ae_maxreal(err, ae_fabs(s-tmp0.ptr.p_double[i], _state), _state);
                    }
                    if( periodiccond )
                    {
                        spline1dconvdiffcubic(&x, &yp, n, bltype, bl, brtype, br, &tmpx, n2, &tmp0, &tmp1, _state);
                    }
                    else
                    {
                        spline1dconvdiffcubic(&x, &y, n, bltype, bl, brtype, br, &tmpx, n2, &tmp0, &tmp1, _state);
                    }
                    for(i=0; i<=n2-1; i++)
                    {
                        spline1ddiff(&c, tmpx.ptr.p_double[i], &s, &ds, &d2s, _state);
                        err = ae_maxreal(err, ae_fabs(s-tmp0.ptr.p_double[i], _state), _state);
                        err = ae_maxreal(err, ae_fabs(ds-tmp1.ptr.p_double[i], _state), _state);
                    }
                    if( periodiccond )
                    {
                        spline1dconvdiff2cubic(&x, &yp, n, bltype, bl, brtype, br, &tmpx, n2, &tmp0, &tmp1, &tmp2, _state);
                    }
                    else
                    {
                        spline1dconvdiff2cubic(&x, &y, n, bltype, bl, brtype, br, &tmpx, n2, &tmp0, &tmp1, &tmp2, _state);
                    }
                    for(i=0; i<=n2-1; i++)
                    {
                        spline1ddiff(&c, tmpx.ptr.p_double[i], &s, &ds, &d2s, _state);
                        err = ae_maxreal(err, ae_fabs(s-tmp0.ptr.p_double[i], _state), _state);
                        err = ae_maxreal(err, ae_fabs(ds-tmp1.ptr.p_double[i], _state), _state);
                        err = ae_maxreal(err, ae_fabs(d2s-tmp2.ptr.p_double[i], _state), _state);
                    }
                    cserrors = cserrors||ae_fp_greater(err,threshold);
                }
            }
            
            /*
             * Build Catmull-Rom spline.
             * Test for interpolation scheme properties:
             * * values at nodes
             * * boundary conditions
             * * continuous function
             * * continuous first derivative
             * * periodicity properties
             */
            for(bltype=-1; bltype<=0; bltype++)
            {
                periodiccond = bltype==-1;
                
                /*
                 * select random tension value, then build
                 */
                if( ae_fp_greater(ae_randomreal(_state),0.5) )
                {
                    if( ae_fp_greater(ae_randomreal(_state),0.5) )
                    {
                        tension = (double)(0);
                    }
                    else
                    {
                        tension = (double)(1);
                    }
                }
                else
                {
                    tension = ae_randomreal(_state);
                }
                if( periodiccond )
                {
                    spline1dbuildcatmullrom(&x, &yp, n, bltype, tension, &c, _state);
                }
                else
                {
                    spline1dbuildcatmullrom(&x, &y, n, bltype, tension, &c, _state);
                }
                
                /*
                 * interpolation properties
                 */
                err = (double)(0);
                if( periodiccond )
                {
                    
                    /*
                     * * check values at nodes; spline is periodic so
                     *   we add random number of periods to nodes
                     * * we also test for periodicity of first derivative
                     */
                    for(i=0; i<=n-1; i++)
                    {
                        v = x.ptr.p_double[i];
                        vm = v+(b-a)*(ae_randominteger(5, _state)-2);
                        t = yp.ptr.p_double[i]-spline1dcalc(&c, vm, _state);
                        err = ae_maxreal(err, ae_fabs(t, _state), _state);
                        spline1ddiff(&c, v, &s, &ds, &d2s, _state);
                        spline1ddiff(&c, vm, &s2, &ds2, &d2s2, _state);
                        err = ae_maxreal(err, ae_fabs(s-s2, _state), _state);
                        err = ae_maxreal(err, ae_fabs(ds-ds2, _state), _state);
                    }
                    
                    /*
                     * periodicity between nodes
                     */
                    v = a+(b-a)*ae_randomreal(_state);
                    vm = v+(b-a)*(ae_randominteger(5, _state)-2);
                    err = ae_maxreal(err, ae_fabs(spline1dcalc(&c, v, _state)-spline1dcalc(&c, vm, _state), _state), _state);
                    spline1ddiff(&c, v, &s, &ds, &d2s, _state);
                    spline1ddiff(&c, vm, &s2, &ds2, &d2s2, _state);
                    err = ae_maxreal(err, ae_fabs(s-s2, _state), _state);
                    err = ae_maxreal(err, ae_fabs(ds-ds2, _state), _state);
                }
                else
                {
                    
                    /*
                     * * check values at nodes
                     */
                    for(i=0; i<=n-1; i++)
                    {
                        err = ae_maxreal(err, ae_fabs(y.ptr.p_double[i]-spline1dcalc(&c, x.ptr.p_double[i], _state), _state), _state);
                    }
                }
                crserrors = crserrors||ae_fp_greater(err,threshold);
                
                /*
                 * check boundary conditions
                 */
                err = (double)(0);
                if( bltype==0 )
                {
                    spline1ddiff(&c, a-h, &s, &ds, &d2s, _state);
                    spline1ddiff(&c, a+h, &s2, &ds2, &d2s2, _state);
                    t = (d2s2-d2s)/(2*h);
                    err = ae_maxreal(err, ae_fabs(t, _state), _state);
                    spline1ddiff(&c, b-h, &s, &ds, &d2s, _state);
                    spline1ddiff(&c, b+h, &s2, &ds2, &d2s2, _state);
                    t = (d2s2-d2s)/(2*h);
                    err = ae_maxreal(err, ae_fabs(t, _state), _state);
                }
                if( bltype==-1 )
                {
                    spline1ddiff(&c, a+100*ae_machineepsilon, &s, &ds, &d2s, _state);
                    spline1ddiff(&c, b-100*ae_machineepsilon, &s2, &ds2, &d2s2, _state);
                    err = ae_maxreal(err, ae_fabs(s-s2, _state), _state);
                    err = ae_maxreal(err, ae_fabs(ds-ds2, _state), _state);
                }
                crserrors = crserrors||ae_fp_greater(err,1.0E-3);
                
                /*
                 * Check Lipschitz continuity
                 */
                testspline1dunit_lconst(a, b, &c, lstep, &l10, &l11, &l12, _state);
                testspline1dunit_lconst(a, b, &c, lstep/3, &l20, &l21, &l22, _state);
                if( ae_fp_greater(l10,1.0E-6) )
                {
                    crserrors = crserrors||ae_fp_greater(l20/l10,1.2);
                }
                if( ae_fp_greater(l11,1.0E-6) )
                {
                    crserrors = crserrors||ae_fp_greater(l21/l11,1.2);
                }
            }
            
            /*
             * Build Hermite spline.
             * Test for interpolation scheme properties:
             * * values and derivatives at nodes
             * * continuous function
             * * continuous first derivative
             */
            spline1dbuildhermite(&x, &y, &d, n, &c, _state);
            err = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                err = ae_maxreal(err, ae_fabs(y.ptr.p_double[i]-spline1dcalc(&c, x.ptr.p_double[i], _state), _state), _state);
            }
            hserrors = hserrors||ae_fp_greater(err,threshold);
            err = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                t = (spline1dcalc(&c, x.ptr.p_double[i]+h, _state)-spline1dcalc(&c, x.ptr.p_double[i]-h, _state))/(2*h);
                err = ae_maxreal(err, ae_fabs(d.ptr.p_double[i]-t, _state), _state);
            }
            hserrors = hserrors||ae_fp_greater(err,1.0E-3);
            testspline1dunit_lconst(a, b, &c, lstep, &l10, &l11, &l12, _state);
            testspline1dunit_lconst(a, b, &c, lstep/3, &l20, &l21, &l22, _state);
            hserrors = hserrors||ae_fp_greater(l20/l10,1.2);
            hserrors = hserrors||ae_fp_greater(l21/l11,1.2);
            
            /*
             * Build Akima spline
             * Test for general interpolation scheme properties:
             * * values at nodes
             * * continuous function
             * * continuous first derivative
             * Test for Akima-specific properties is implemented below.
             */
            spline1dbuildakima(&x, &y, n, &c, _state);
            err = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                err = ae_maxreal(err, ae_fabs(y.ptr.p_double[i]-spline1dcalc(&c, x.ptr.p_double[i], _state), _state), _state);
            }
            aserrors = aserrors||ae_fp_greater(err,threshold);
            testspline1dunit_lconst(a, b, &c, lstep, &l10, &l11, &l12, _state);
            testspline1dunit_lconst(a, b, &c, lstep/3, &l20, &l21, &l22, _state);
            hserrors = hserrors||(ae_fp_greater(l10,1.0E-10)&&ae_fp_greater(l20/l10,1.2));
            hserrors = hserrors||(ae_fp_greater(l11,1.0E-10)&&ae_fp_greater(l21/l11,1.2));
        }
    }
    
    /*
     * Special linear spline test:
     * test for linearity between x[i] and x[i+1]
     */
    for(n=2; n<=maxn; n++)
    {
        ae_vector_set_length(&x, n-1+1, _state);
        ae_vector_set_length(&y, n-1+1, _state);
        
        /*
         * Prepare task
         */
        a = (double)(-1);
        b = (double)(1);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = a+(b-a)*i/(n-1);
            y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        spline1dbuildlinear(&x, &y, n, &c, _state);
        
        /*
         * Test
         */
        err = (double)(0);
        for(k=0; k<=n-2; k++)
        {
            a = x.ptr.p_double[k];
            b = x.ptr.p_double[k+1];
            for(pass=1; pass<=passcount; pass++)
            {
                t = a+(b-a)*ae_randomreal(_state);
                v = y.ptr.p_double[k]+(t-a)/(b-a)*(y.ptr.p_double[k+1]-y.ptr.p_double[k]);
                err = ae_maxreal(err, ae_fabs(spline1dcalc(&c, t, _state)-v, _state), _state);
            }
        }
        lserrors = lserrors||ae_fp_greater(err,threshold);
    }
    
    /*
     * Special Akima test: test outlier sensitivity
     * Spline value at (x[i], x[i+1]) should depend from
     * f[i-2], f[i-1], f[i], f[i+1], f[i+2], f[i+3] only.
     */
    for(n=5; n<=maxn; n++)
    {
        ae_vector_set_length(&x, n-1+1, _state);
        ae_vector_set_length(&y, n-1+1, _state);
        ae_vector_set_length(&y2, n-1+1, _state);
        
        /*
         * Prepare unperturbed Akima spline
         */
        a = (double)(-1);
        b = (double)(1);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = a+(b-a)*i/(n-1);
            y.ptr.p_double[i] = ae_cos(1.3*ae_pi*x.ptr.p_double[i]+0.4, _state);
        }
        spline1dbuildakima(&x, &y, n, &c, _state);
        
        /*
         * Process perturbed tasks
         */
        err = (double)(0);
        for(k=0; k<=n-1; k++)
        {
            ae_v_move(&y2.ptr.p_double[0], 1, &y.ptr.p_double[0], 1, ae_v_len(0,n-1));
            y2.ptr.p_double[k] = (double)(5);
            spline1dbuildakima(&x, &y2, n, &c2, _state);
            
            /*
             * Test left part independence
             */
            if( k-3>=1 )
            {
                a = (double)(-1);
                b = x.ptr.p_double[k-3];
                for(pass=1; pass<=passcount; pass++)
                {
                    t = a+(b-a)*ae_randomreal(_state);
                    err = ae_maxreal(err, ae_fabs(spline1dcalc(&c, t, _state)-spline1dcalc(&c2, t, _state), _state), _state);
                }
            }
            
            /*
             * Test right part independence
             */
            if( k+3<=n-2 )
            {
                a = x.ptr.p_double[k+3];
                b = (double)(1);
                for(pass=1; pass<=passcount; pass++)
                {
                    t = a+(b-a)*ae_randomreal(_state);
                    err = ae_maxreal(err, ae_fabs(spline1dcalc(&c, t, _state)-spline1dcalc(&c2, t, _state), _state), _state);
                }
            }
        }
        aserrors = aserrors||ae_fp_greater(err,threshold);
    }
    
    /*
     * Differentiation, copy/unpack test
     */
    for(n=2; n<=maxn; n++)
    {
        ae_vector_set_length(&x, n-1+1, _state);
        ae_vector_set_length(&y, n-1+1, _state);
        
        /*
         * Prepare cubic spline
         */
        a = -1-ae_randomreal(_state);
        b = 1+ae_randomreal(_state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = a+(b-a)*i/(n-1);
            y.ptr.p_double[i] = ae_cos(1.3*ae_pi*x.ptr.p_double[i]+0.4, _state);
        }
        spline1dbuildcubic(&x, &y, n, 2, 0.0, 2, 0.0, &c, _state);
        
        /*
         * Test diff
         */
        err = (double)(0);
        for(pass=1; pass<=passcount; pass++)
        {
            t = a+(b-a)*ae_randomreal(_state);
            spline1ddiff(&c, t, &s, &ds, &d2s, _state);
            vl = spline1dcalc(&c, t-h, _state);
            vm = spline1dcalc(&c, t, _state);
            vr = spline1dcalc(&c, t+h, _state);
            err = ae_maxreal(err, ae_fabs(s-vm, _state), _state);
            err = ae_maxreal(err, ae_fabs(ds-(vr-vl)/(2*h), _state), _state);
            err = ae_maxreal(err, ae_fabs(d2s-(vr-2*vm+vl)/ae_sqr(h, _state), _state), _state);
        }
        dserrors = dserrors||ae_fp_greater(err,0.001);
        
        /*
         * Test copy
         */
        testspline1dunit_unsetspline1d(&c2, _state);
        spline1dcopy(&c, &c2, _state);
        err = (double)(0);
        for(pass=1; pass<=passcount; pass++)
        {
            t = a+(b-a)*ae_randomreal(_state);
            err = ae_maxreal(err, ae_fabs(spline1dcalc(&c, t, _state)-spline1dcalc(&c2, t, _state), _state), _state);
        }
        cperrors = cperrors||ae_fp_greater(err,threshold);
        
        /*
         * Test unpack
         */
        uperrors = uperrors||!testspline1dunit_testunpack(&c, &x, _state);
    }
    
    /*
     * Linear translation errors
     */
    for(n=2; n<=maxn; n++)
    {
        
        /*
         * Prepare:
         * * X, Y - grid points
         * * XTest - test points
         */
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&y, n, _state);
        a = -1-ae_randomreal(_state);
        b = 1+ae_randomreal(_state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = a+(b-a)*(i+0.2*ae_randomreal(_state)-0.1)/(n-1);
            y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        ae_vector_set_length(&xtest, 5*n+2, _state);
        for(i=0; i<=xtest.cnt-1; i++)
        {
            xtest.ptr.p_double[i] = a+(b-a)*(i-1)/(xtest.cnt-3);
        }
        splineindex = 0;
        while(testspline1dunit_enumerateallsplines(&x, &y, n, &splineindex, &c, _state))
        {
            
            /*
             * LinTransX, general A
             */
            sa = 4*ae_randomreal(_state)-2;
            sb = 2*ae_randomreal(_state)-1;
            spline1dcopy(&c, &c2, _state);
            spline1dlintransx(&c2, sa, sb, _state);
            for(i=0; i<=xtest.cnt-1; i++)
            {
                lterrors = lterrors||ae_fp_greater(ae_fabs(spline1dcalc(&c, xtest.ptr.p_double[i], _state)-spline1dcalc(&c2, (xtest.ptr.p_double[i]-sb)/sa, _state), _state),threshold);
            }
            
            /*
             * LinTransX, special case: A=0
             */
            sb = 2*ae_randomreal(_state)-1;
            spline1dcopy(&c, &c2, _state);
            spline1dlintransx(&c2, (double)(0), sb, _state);
            for(i=0; i<=xtest.cnt-1; i++)
            {
                lterrors = lterrors||ae_fp_greater(ae_fabs(spline1dcalc(&c, sb, _state)-spline1dcalc(&c2, xtest.ptr.p_double[i], _state), _state),threshold);
            }
            
            /*
             * LinTransY
             */
            sa = 2*ae_randomreal(_state)-1;
            sb = 2*ae_randomreal(_state)-1;
            spline1dcopy(&c, &c2, _state);
            spline1dlintransy(&c2, sa, sb, _state);
            for(i=0; i<=xtest.cnt-1; i++)
            {
                lterrors = lterrors||ae_fp_greater(ae_fabs(sa*spline1dcalc(&c, xtest.ptr.p_double[i], _state)+sb-spline1dcalc(&c2, xtest.ptr.p_double[i], _state), _state),threshold);
            }
        }
    }
    
    /*
     * Testing integration.
     * Three tests are performed:
     *
     * * approximate test (well behaved smooth function, many points,
     *   integration inside [a,b]), non-periodic spline
     *
     * * exact test (integration of parabola, outside of [a,b], non-periodic spline
     *
     * * approximate test for periodic splines. F(x)=cos(2*pi*x)+1.
     *   Period length is equals to 1.0, so all operations with
     *   multiples of period are done exactly. For each value of PERIOD
     *   we calculate and test integral at four points:
     *   -   0 < t0 < PERIOD
     *   -   t1 = PERIOD-eps
     *   -   t2 = PERIOD
     *   -   t3 = PERIOD+eps
     */
    err = (double)(0);
    for(n=20; n<=35; n++)
    {
        ae_vector_set_length(&x, n-1+1, _state);
        ae_vector_set_length(&y, n-1+1, _state);
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * Prepare cubic spline
             */
            a = -1-0.2*ae_randomreal(_state);
            b = 1+0.2*ae_randomreal(_state);
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = a+(b-a)*i/(n-1);
                y.ptr.p_double[i] = ae_sin(ae_pi*x.ptr.p_double[i]+0.4, _state)+ae_exp(x.ptr.p_double[i], _state);
            }
            bl = ae_pi*ae_cos(ae_pi*a+0.4, _state)+ae_exp(a, _state);
            br = ae_pi*ae_cos(ae_pi*b+0.4, _state)+ae_exp(b, _state);
            spline1dbuildcubic(&x, &y, n, 1, bl, 1, br, &c, _state);
            
            /*
             * Test
             */
            t = a+(b-a)*ae_randomreal(_state);
            v = -ae_cos(ae_pi*a+0.4, _state)/ae_pi+ae_exp(a, _state);
            v = -ae_cos(ae_pi*t+0.4, _state)/ae_pi+ae_exp(t, _state)-v;
            v = v-spline1dintegrate(&c, t, _state);
            err = ae_maxreal(err, ae_fabs(v, _state), _state);
        }
    }
    ierrors = ierrors||ae_fp_greater(err,0.001);
    p0 = 2*ae_randomreal(_state)-1;
    p1 = 2*ae_randomreal(_state)-1;
    p2 = 2*ae_randomreal(_state)-1;
    a = -ae_randomreal(_state)-0.5;
    b = ae_randomreal(_state)+0.5;
    n = 2;
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&y, n, _state);
    ae_vector_set_length(&d, n, _state);
    x.ptr.p_double[0] = a;
    y.ptr.p_double[0] = p0+p1*a+p2*ae_sqr(a, _state);
    d.ptr.p_double[0] = p1+2*p2*a;
    x.ptr.p_double[1] = b;
    y.ptr.p_double[1] = p0+p1*b+p2*ae_sqr(b, _state);
    d.ptr.p_double[1] = p1+2*p2*b;
    spline1dbuildhermite(&x, &y, &d, n, &c, _state);
    bl = ae_minreal(a, b, _state)-ae_fabs(b-a, _state);
    br = ae_minreal(a, b, _state)+ae_fabs(b-a, _state);
    err = (double)(0);
    for(pass=1; pass<=100; pass++)
    {
        t = bl+(br-bl)*ae_randomreal(_state);
        v = p0*t+p1*ae_sqr(t, _state)/2+p2*ae_sqr(t, _state)*t/3-(p0*a+p1*ae_sqr(a, _state)/2+p2*ae_sqr(a, _state)*a/3);
        v = v-spline1dintegrate(&c, t, _state);
        err = ae_maxreal(err, ae_fabs(v, _state), _state);
    }
    ierrors = ierrors||ae_fp_greater(err,threshold);
    n = 100;
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&y, n, _state);
    for(i=0; i<=n-1; i++)
    {
        x.ptr.p_double[i] = (double)i/(double)(n-1);
        y.ptr.p_double[i] = ae_cos(2*ae_pi*x.ptr.p_double[i], _state)+1;
    }
    y.ptr.p_double[0] = (double)(2);
    y.ptr.p_double[n-1] = (double)(2);
    spline1dbuildcubic(&x, &y, n, -1, 0.0, -1, 0.0, &c, _state);
    intab = spline1dintegrate(&c, 1.0, _state);
    v = ae_randomreal(_state);
    vr = spline1dintegrate(&c, v, _state);
    ierrors = ierrors||ae_fp_greater(ae_fabs(intab-1, _state),0.001);
    for(i=-10; i<=10; i++)
    {
        ierrors = ierrors||ae_fp_greater(ae_fabs(spline1dintegrate(&c, i+v, _state)-(i*intab+vr), _state),0.001);
        ierrors = ierrors||ae_fp_greater(ae_fabs(spline1dintegrate(&c, i-1000*ae_machineepsilon, _state)-i*intab, _state),0.001);
        ierrors = ierrors||ae_fp_greater(ae_fabs(spline1dintegrate(&c, (double)(i), _state)-i*intab, _state),0.001);
        ierrors = ierrors||ae_fp_greater(ae_fabs(spline1dintegrate(&c, i+1000*ae_machineepsilon, _state)-i*intab, _state),0.001);
    }
    
    /*
     * Test fo monotone cubic Hermit interpolation
     */
    monotoneerr = testspline1dunit_testmonotonespline(_state);
    
    /*
     * report
     */
    waserrors = (((((((((lserrors||cserrors)||crserrors)||hserrors)||aserrors)||dserrors)||cperrors)||uperrors)||lterrors)||ierrors)||monotoneerr;
    if( !silent )
    {
        printf("TESTING SPLINE INTERPOLATION\n");
        
        /*
         * Normal tests
         */
        printf("LINEAR SPLINE TEST:                      ");
        if( lserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("CUBIC SPLINE TEST:                       ");
        if( cserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("CATMULL-ROM SPLINE TEST:                 ");
        if( crserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("HERMITE SPLINE TEST:                     ");
        if( hserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("AKIMA SPLINE TEST:                       ");
        if( aserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("DIFFERENTIATION TEST:                    ");
        if( dserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("COPY/SERIALIZATION TEST:                 ");
        if( cperrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("UNPACK TEST:                             ");
        if( uperrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("LIN.TRANS. TEST:                         ");
        if( lterrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("INTEGRATION TEST:                        ");
        if( ierrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("TEST MONOTONE CUBIC HERMITE SPLINE:      ");
        if( monotoneerr )
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
ae_bool _pexec_testspline1d(ae_bool silent, ae_state *_state)
{
    return testspline1d(silent, _state);
}


/*************************************************************************
Lipschitz constants for spline inself, first and second derivatives.
*************************************************************************/
static void testspline1dunit_lconst(double a,
     double b,
     spline1dinterpolant* c,
     double lstep,
     double* l0,
     double* l1,
     double* l2,
     ae_state *_state)
{
    double t;
    double vl;
    double vm;
    double vr;
    double prevf;
    double prevd;
    double prevd2;
    double f;
    double d;
    double d2;

    *l0 = 0;
    *l1 = 0;
    *l2 = 0;

    *l0 = (double)(0);
    *l1 = (double)(0);
    *l2 = (double)(0);
    t = a-0.1;
    vl = spline1dcalc(c, t-2*lstep, _state);
    vm = spline1dcalc(c, t-lstep, _state);
    vr = spline1dcalc(c, t, _state);
    f = vm;
    d = (vr-vl)/(2*lstep);
    d2 = (vr-2*vm+vl)/ae_sqr(lstep, _state);
    while(ae_fp_less_eq(t,b+0.1))
    {
        prevf = f;
        prevd = d;
        prevd2 = d2;
        vl = vm;
        vm = vr;
        vr = spline1dcalc(c, t+lstep, _state);
        f = vm;
        d = (vr-vl)/(2*lstep);
        d2 = (vr-2*vm+vl)/ae_sqr(lstep, _state);
        *l0 = ae_maxreal(*l0, ae_fabs((f-prevf)/lstep, _state), _state);
        *l1 = ae_maxreal(*l1, ae_fabs((d-prevd)/lstep, _state), _state);
        *l2 = ae_maxreal(*l2, ae_fabs((d2-prevd2)/lstep, _state), _state);
        t = t+lstep;
    }
}


/*************************************************************************
This function is used to enumerate all spline types  which  can  be  built
from given dataset. It should be used as follows:

>
> init X, Y, N
> SplineIndex:=0;
> while EnumerateAllSplines(X, Y, N, SplineIndex, S) do
> begin
>     do something with S
> end;
>

On initial call EnumerateAllSplines accepts:
* dataset X, Y, number of points N (N>=2)
* SplineIndex, equal to 0

It returns:
* True, in case there is a spline type which corresponds to SplineIndex.
  In this case S contains spline which was built using X/Y and spline type,
  as specified by input value of SplineIndex. SplineIndex is advanced to
  the next value.
* False, in case SplineIndex contains past-the-end value, spline is not built.

This function tries different variants of linear/cubic, periodic/nonperiodic
splines.
*************************************************************************/
static ae_bool testspline1dunit_enumerateallsplines(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t* splineindex,
     spline1dinterpolant* s,
     ae_state *_state)
{
    ae_int_t idxoffs;
    ae_bool result;

    _spline1dinterpolant_clear(s);

    ae_assert(*splineindex>=0, "Assertion failed", _state);
    result = ae_false;
    if( *splineindex==0 )
    {
        
        /*
         * Linear spline
         */
        spline1dbuildlinear(x, y, n, s, _state);
        *splineindex = *splineindex+1;
        result = ae_true;
        return result;
    }
    else
    {
        if( *splineindex>=1&&*splineindex<11 )
        {
            
            /*
             * Cubic spline, either periodic or non-periodic
             */
            idxoffs = *splineindex-1;
            if( idxoffs==9 )
            {
                
                /*
                 * Periodic spline
                 */
                spline1dbuildcubic(x, y, n, -1, 0.0, -1, 0.0, s, _state);
            }
            else
            {
                
                /*
                 * Non-periodic spline
                 */
                spline1dbuildcubic(x, y, n, idxoffs/3, 2*ae_randomreal(_state)-1, idxoffs%3, 2*ae_randomreal(_state)-1, s, _state);
            }
            *splineindex = *splineindex+1;
            result = ae_true;
            return result;
        }
    }
    return result;
}


/*************************************************************************
Unpack testing
*************************************************************************/
static ae_bool testspline1dunit_testunpack(spline1dinterpolant* c,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t n;
    double err;
    double t;
    double v1;
    double v2;
    ae_int_t pass;
    ae_int_t passcount;
    ae_matrix tbl;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&tbl, 0, 0, DT_REAL, _state);

    passcount = 20;
    err = (double)(0);
    spline1dunpack(c, &n, &tbl, _state);
    for(i=0; i<=n-2; i++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            t = ae_randomreal(_state)*(tbl.ptr.pp_double[i][1]-tbl.ptr.pp_double[i][0]);
            v1 = tbl.ptr.pp_double[i][2]+t*tbl.ptr.pp_double[i][3]+ae_sqr(t, _state)*tbl.ptr.pp_double[i][4]+t*ae_sqr(t, _state)*tbl.ptr.pp_double[i][5];
            v2 = spline1dcalc(c, tbl.ptr.pp_double[i][0]+t, _state);
            err = ae_maxreal(err, ae_fabs(v1-v2, _state), _state);
        }
    }
    for(i=0; i<=n-2; i++)
    {
        err = ae_maxreal(err, ae_fabs(x->ptr.p_double[i]-tbl.ptr.pp_double[i][0], _state), _state);
    }
    for(i=0; i<=n-2; i++)
    {
        err = ae_maxreal(err, ae_fabs(x->ptr.p_double[i+1]-tbl.ptr.pp_double[i][1], _state), _state);
    }
    result = ae_fp_less(err,100*ae_machineepsilon);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Unset spline, i.e. initialize it with random garbage
*************************************************************************/
static void testspline1dunit_unsetspline1d(spline1dinterpolant* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_vector d;

    ae_frame_make(_state, &_frame_block);
    _spline1dinterpolant_clear(c);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);

    ae_vector_set_length(&x, 2, _state);
    ae_vector_set_length(&y, 2, _state);
    ae_vector_set_length(&d, 2, _state);
    x.ptr.p_double[0] = (double)(-1);
    y.ptr.p_double[0] = ae_randomreal(_state);
    d.ptr.p_double[0] = ae_randomreal(_state);
    x.ptr.p_double[1] = (double)(1);
    y.ptr.p_double[1] = ae_randomreal(_state);
    d.ptr.p_double[1] = ae_randomreal(_state);
    spline1dbuildhermite(&x, &y, &d, 2, c, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Tests that built spline is monotone.
*************************************************************************/
static ae_bool testspline1dunit_testmonotonespline(ae_state *_state)
{
    ae_frame _frame_block;
    spline1dinterpolant c;
    spline1dinterpolant s2;
    double c0;
    double c1;
    ae_vector x;
    ae_vector y;
    ae_vector d;
    ae_int_t m;
    ae_vector n;
    ae_int_t alln;
    ae_int_t shift;
    double sign0;
    double sign1;
    double r;
    double st;
    double eps;
    double delta;
    double v;
    double dv;
    double d2v;
    ae_int_t nseg;
    ae_int_t npoints;
    ae_int_t tp;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t kmax;
    ae_int_t l;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _spline1dinterpolant_init(&c, _state);
    _spline1dinterpolant_init(&s2, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&n, 0, DT_INT, _state);

    eps = 100*ae_machineepsilon;
    
    /*
     * Special test - N=2.
     *
     * Following properties are tested:
     * * monotone spline must be equal to the Hermite spline with
     *   zero derivative at the ends
     * * monotone spline is constant beyond left/right boundaries
     */
    ae_vector_set_length(&x, 2, _state);
    ae_vector_set_length(&y, 2, _state);
    ae_vector_set_length(&d, 2, _state);
    x.ptr.p_double[0] = -0.1-ae_randomreal(_state);
    y.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
    d.ptr.p_double[0] = 0.0;
    x.ptr.p_double[1] = 0.1+ae_randomreal(_state);
    y.ptr.p_double[1] = y.ptr.p_double[0];
    d.ptr.p_double[1] = 0.0;
    spline1dbuildmonotone(&x, &y, 2, &c, _state);
    spline1dbuildhermite(&x, &y, &d, 2, &s2, _state);
    v = 2*ae_randomreal(_state)-1;
    if( ae_fp_greater(ae_fabs(spline1dcalc(&c, v, _state)-spline1dcalc(&s2, v, _state), _state),eps) )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    if( ae_fp_neq(spline1dcalc(&c, (double)(-5), _state),y.ptr.p_double[0]) )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    if( ae_fp_neq(spline1dcalc(&c, (double)(5), _state),y.ptr.p_double[0]) )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Special test - N=3, x=[0,1,2], y=[0,1,0].
     * Monotone spline must be equal to the Hermite spline with
     * zero derivative at all points.
     */
    ae_vector_set_length(&x, 3, _state);
    ae_vector_set_length(&y, 3, _state);
    ae_vector_set_length(&d, 3, _state);
    x.ptr.p_double[0] = 0.0;
    y.ptr.p_double[0] = 0.0;
    d.ptr.p_double[0] = 0.0;
    x.ptr.p_double[1] = 1.0;
    y.ptr.p_double[1] = 1.0;
    d.ptr.p_double[1] = 0.0;
    x.ptr.p_double[2] = 2.0;
    y.ptr.p_double[2] = 0.0;
    d.ptr.p_double[2] = 0.0;
    spline1dbuildmonotone(&x, &y, 3, &c, _state);
    spline1dbuildhermite(&x, &y, &d, 3, &s2, _state);
    for(i=0; i<=10; i++)
    {
        v = x.ptr.p_double[0]+(double)i/(double)10*(x.ptr.p_double[2]-x.ptr.p_double[0]);
        if( ae_fp_greater(ae_fabs(spline1dcalc(&c, v, _state)-spline1dcalc(&s2, v, _state), _state),eps) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * Special test - N=5, x=[0,1,2,3,4], y=[0,1,1,2,3].
     *
     * 1) spline passes through all prescribed points
     * 2) spline derivative at all points except x=3 is exactly zero
     * 3) spline derivative at x=3 is 1.0 (within machine epsilon)
     */
    ae_vector_set_length(&x, 5, _state);
    ae_vector_set_length(&y, 5, _state);
    x.ptr.p_double[0] = 0.0;
    y.ptr.p_double[0] = 0.0;
    x.ptr.p_double[1] = 1.0;
    y.ptr.p_double[1] = 1.0;
    x.ptr.p_double[2] = 2.0;
    y.ptr.p_double[2] = 1.0;
    x.ptr.p_double[3] = 3.0;
    y.ptr.p_double[3] = 2.0;
    x.ptr.p_double[4] = 4.0;
    y.ptr.p_double[4] = 3.0;
    spline1dbuildmonotone(&x, &y, 5, &c, _state);
    for(i=0; i<=4; i++)
    {
        spline1ddiff(&c, x.ptr.p_double[i], &v, &dv, &d2v, _state);
        if( ae_fp_greater(ae_fabs(v-y.ptr.p_double[i], _state),eps) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        if( (ae_fp_eq(x.ptr.p_double[i],3.0)&&ae_fp_greater(ae_fabs(dv-1.0, _state),eps))||(ae_fp_neq(x.ptr.p_double[i],3.0)&&ae_fp_neq(dv,(double)(0))) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * Special test:
     * * N=4
     * * three fixed points - (0,0), (1,1), (2,0)
     * * one special point (x,y) with x in [0.1,0.9], y in [0.1,0.9]
     * * monotonicity of the interpolant at [0,1] is checked with very small step 1/KMax
     */
    ae_vector_set_length(&x, 4, _state);
    ae_vector_set_length(&y, 4, _state);
    x.ptr.p_double[0] = 0.0;
    y.ptr.p_double[0] = 0.0;
    x.ptr.p_double[2] = 1.0;
    y.ptr.p_double[2] = 1.0;
    x.ptr.p_double[3] = 2.0;
    y.ptr.p_double[3] = 0.0;
    for(i=1; i<=9; i++)
    {
        for(j=1; j<=9; j++)
        {
            x.ptr.p_double[1] = (double)i/(double)10;
            y.ptr.p_double[1] = (double)j/(double)10;
            spline1dbuildmonotone(&x, &y, 4, &c, _state);
            kmax = 1000;
            for(k=0; k<=kmax-1; k++)
            {
                if( ae_fp_greater(spline1dcalc(&c, (double)k/(double)kmax, _state),spline1dcalc(&c, (double)(k+1)/(double)kmax, _state)) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
    }
    
    /*
     * General case
     */
    delta = (double)(0);
    nseg = 10;
    npoints = 15;
    passcount = 30;
    for(pass=1; pass<=passcount; pass++)
    {
        tp = ae_randominteger(6, _state)+4;
        r = (double)(ae_randominteger(76, _state)+25);
        m = ae_randominteger(nseg, _state)+1;
        ae_vector_set_length(&n, m, _state);
        alln = 0;
        for(i=0; i<=m-1; i++)
        {
            n.ptr.p_int[i] = ae_randominteger(npoints, _state)+2;
            alln = alln+n.ptr.p_int[i];
        }
        ae_vector_set_length(&x, alln, _state);
        ae_vector_set_length(&y, alln, _state);
        x.ptr.p_double[0] = r*(2*ae_randomreal(_state)-1);
        y.ptr.p_double[0] = r*(2*ae_randomreal(_state)-1);
        
        /*
         * Builds monotone function
         */
        st = 0.1+0.7*ae_randomreal(_state);
        shift = 0;
        sign0 = ae_pow((double)(-1), (double)(0), _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=1; j<=n.ptr.p_int[i]-1; j++)
            {
                x.ptr.p_double[shift+j] = x.ptr.p_double[shift+j-1]+st+ae_randomreal(_state);
                delta = ae_maxreal(delta, x.ptr.p_double[shift+j]-x.ptr.p_double[shift+j-1], _state);
                y.ptr.p_double[shift+j] = y.ptr.p_double[shift+j-1]+sign0*(st+ae_randomreal(_state));
            }
            shift = shift+n.ptr.p_int[i];
            if( i!=m-1 )
            {
                sign0 = ae_pow((double)(-1), (double)(i+1), _state);
                x.ptr.p_double[shift] = x.ptr.p_double[shift-1]+st+ae_randomreal(_state);
                y.ptr.p_double[shift] = y.ptr.p_double[shift-1]+sign0*ae_randomreal(_state);
            }
        }
        delta = 3*delta;
        spline1dbuildmonotone(&x, &y, alln, &c, _state);
        
        /*
         * Check that built function is monotone
         */
        shift = 0;
        for(i=0; i<=m-1; i++)
        {
            for(j=1; j<=n.ptr.p_int[i]-1; j++)
            {
                st = (x.ptr.p_double[shift+j]-x.ptr.p_double[shift+j-1])/tp;
                sign0 = y.ptr.p_double[shift+j]-y.ptr.p_double[shift+j-1];
                if( ae_fp_neq(sign0,(double)(0)) )
                {
                    sign0 = sign0/ae_fabs(sign0, _state);
                }
                for(l=0; l<=tp-1; l++)
                {
                    c0 = spline1dcalc(&c, x.ptr.p_double[shift+j-1]+l*st, _state);
                    c1 = spline1dcalc(&c, x.ptr.p_double[shift+j-1]+(l+1)*st, _state);
                    sign1 = c1-c0;
                    if( ae_fp_neq(sign1,(double)(0)) )
                    {
                        sign1 = sign1/ae_fabs(sign1, _state);
                    }
                    if( ae_fp_less(sign0*sign1,(double)(0)) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
        }
        c0 = spline1dcalc(&c, x.ptr.p_double[0]-delta, _state);
        c1 = spline1dcalc(&c, x.ptr.p_double[0], _state);
        if( ae_fp_greater(ae_fabs(c0-c1, _state),eps) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        c0 = spline1dcalc(&c, x.ptr.p_double[alln-1], _state);
        c1 = spline1dcalc(&c, x.ptr.p_double[alln-1]+delta, _state);
        if( ae_fp_greater(ae_fabs(c0-c1, _state),eps) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Builds constant function
         */
        y.ptr.p_double[0] = r*(2*ae_randomreal(_state)-1);
        for(i=1; i<=alln-1; i++)
        {
            y.ptr.p_double[i] = y.ptr.p_double[0];
        }
        spline1dbuildmonotone(&x, &y, alln, &c, _state);
        shift = 0;
        for(i=0; i<=m-1; i++)
        {
            for(j=1; j<=n.ptr.p_int[i]-1; j++)
            {
                st = (x.ptr.p_double[shift+j]-x.ptr.p_double[shift+j-1])/tp;
                sign0 = y.ptr.p_double[shift+j]-y.ptr.p_double[shift+j-1];
                for(l=0; l<=tp-1; l++)
                {
                    c0 = spline1dcalc(&c, x.ptr.p_double[shift+j-1]+l*st, _state);
                    c1 = spline1dcalc(&c, x.ptr.p_double[shift+j-1]+(l+1)*st, _state);
                    sign1 = c1-c0;
                    if( ae_fp_greater(sign0,eps)||ae_fp_greater(sign1,eps) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*$ End $*/
