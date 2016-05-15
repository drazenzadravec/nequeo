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
#include "testspline2dunit.h"


/*$ Declarations $*/
static void testspline2dunit_lconst(spline2dinterpolant* c,
     /* Real    */ ae_vector* lx,
     /* Real    */ ae_vector* ly,
     ae_int_t m,
     ae_int_t n,
     double lstep,
     double* lc,
     double* lcx,
     double* lcy,
     double* lcxy,
     ae_state *_state);
static void testspline2dunit_twodnumder(spline2dinterpolant* c,
     double x,
     double y,
     double h,
     double* f,
     double* fx,
     double* fy,
     double* fxy,
     ae_state *_state);
static ae_bool testspline2dunit_testunpack(spline2dinterpolant* c,
     /* Real    */ ae_vector* lx,
     /* Real    */ ae_vector* ly,
     ae_state *_state);
static ae_bool testspline2dunit_testlintrans(spline2dinterpolant* c,
     ae_int_t d,
     double ax,
     double bx,
     double ay,
     double by,
     ae_state *_state);
static void testspline2dunit_unsetspline2d(spline2dinterpolant* c,
     ae_state *_state);
static ae_bool testspline2dunit_testspline2dvf(ae_bool silent,
     ae_state *_state);


/*$ Body $*/


ae_bool testspline2d(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_bool blerrors;
    ae_bool bcerrors;
    ae_bool dserrors;
    ae_bool cperrors;
    ae_bool uperrors;
    ae_bool lterrors;
    ae_bool syerrors;
    ae_bool rlerrors;
    ae_bool rcerrors;
    ae_bool vferrors;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t jobtype;
    double lstep;
    double h;
    ae_vector x;
    ae_vector y;
    spline2dinterpolant c;
    spline2dinterpolant c2;
    ae_vector lx;
    ae_vector ly;
    ae_vector fv;
    ae_matrix f;
    ae_matrix fr;
    ae_matrix ft;
    double ax;
    double ay;
    double bx;
    double by;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t n;
    ae_int_t m;
    ae_int_t d;
    ae_int_t n2;
    ae_int_t m2;
    double err;
    double t;
    double t1;
    double t2;
    double l1;
    double l1x;
    double l1y;
    double l1xy;
    double l2;
    double l2x;
    double l2y;
    double l2xy;
    double fm;
    double f1;
    double f2;
    double f3;
    double f4;
    double v1;
    double v1x;
    double v1y;
    double v1xy;
    double v2;
    double v2x;
    double v2y;
    double v2xy;
    double mf;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    _spline2dinterpolant_init(&c, _state);
    _spline2dinterpolant_init(&c2, _state);
    ae_vector_init(&lx, 0, DT_REAL, _state);
    ae_vector_init(&ly, 0, DT_REAL, _state);
    ae_vector_init(&fv, 0, DT_REAL, _state);
    ae_matrix_init(&f, 0, 0, DT_REAL, _state);
    ae_matrix_init(&fr, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ft, 0, 0, DT_REAL, _state);

    waserrors = ae_false;
    passcount = 10;
    h = 0.00001;
    lstep = 0.001;
    blerrors = ae_false;
    bcerrors = ae_false;
    dserrors = ae_false;
    cperrors = ae_false;
    uperrors = ae_false;
    lterrors = ae_false;
    syerrors = ae_false;
    rlerrors = ae_false;
    rcerrors = ae_false;
    vferrors = ae_false;
    
    /*
     * Test: bilinear, bicubic
     */
    for(n=2; n<=7; n++)
    {
        for(m=2; m<=7; m++)
        {
            d = ae_randominteger(2, _state)+2;
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&y, m, _state);
            ae_vector_set_length(&lx, 2*n-1, _state);
            ae_vector_set_length(&ly, 2*m-1, _state);
            ae_matrix_set_length(&f, m, n, _state);
            ae_vector_set_length(&fv, m*n*d, _state);
            ae_matrix_set_length(&ft, n, m, _state);
            for(pass=1; pass<=passcount; pass++)
            {
                
                /*
                 * Prepare task:
                 * * X and Y stores grid
                 * * F stores function values
                 * * LX and LY stores twice dense grid (for Lipschitz testing)
                 */
                ax = -1-ae_randomreal(_state);
                bx = 1+ae_randomreal(_state);
                ay = -1-ae_randomreal(_state);
                by = 1+ae_randomreal(_state);
                for(j=0; j<=n-1; j++)
                {
                    x.ptr.p_double[j] = 0.5*(bx+ax)-0.5*(bx-ax)*ae_cos(ae_pi*(2*j+1)/(2*n), _state);
                    if( j==0 )
                    {
                        x.ptr.p_double[j] = ax;
                    }
                    if( j==n-1 )
                    {
                        x.ptr.p_double[j] = bx;
                    }
                    lx.ptr.p_double[2*j] = x.ptr.p_double[j];
                    if( j>0 )
                    {
                        lx.ptr.p_double[2*j-1] = 0.5*(x.ptr.p_double[j]+x.ptr.p_double[j-1]);
                    }
                }
                for(j=0; j<=n-1; j++)
                {
                    k = ae_randominteger(n, _state);
                    if( k!=j )
                    {
                        t = x.ptr.p_double[j];
                        x.ptr.p_double[j] = x.ptr.p_double[k];
                        x.ptr.p_double[k] = t;
                    }
                }
                for(i=0; i<=m-1; i++)
                {
                    y.ptr.p_double[i] = 0.5*(by+ay)-0.5*(by-ay)*ae_cos(ae_pi*(2*i+1)/(2*m), _state);
                    if( i==0 )
                    {
                        y.ptr.p_double[i] = ay;
                    }
                    if( i==m-1 )
                    {
                        y.ptr.p_double[i] = by;
                    }
                    ly.ptr.p_double[2*i] = y.ptr.p_double[i];
                    if( i>0 )
                    {
                        ly.ptr.p_double[2*i-1] = 0.5*(y.ptr.p_double[i]+y.ptr.p_double[i-1]);
                    }
                }
                for(i=0; i<=m-1; i++)
                {
                    k = ae_randominteger(m, _state);
                    if( k!=i )
                    {
                        t = y.ptr.p_double[i];
                        y.ptr.p_double[i] = y.ptr.p_double[k];
                        y.ptr.p_double[k] = t;
                    }
                }
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        f.ptr.pp_double[i][j] = ae_exp(0.6*x.ptr.p_double[j], _state)-ae_exp(-0.3*y.ptr.p_double[i]+0.08*x.ptr.p_double[j], _state)+2*ae_cos(ae_pi*(x.ptr.p_double[j]+1.2*y.ptr.p_double[i]), _state)+0.1*ae_cos(20*x.ptr.p_double[j]+15*y.ptr.p_double[i], _state);
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        for(k=0; k<=d-1; k++)
                        {
                            fv.ptr.p_double[d*(n*j+i)+k] = ae_exp(0.6*x.ptr.p_double[i], _state)-ae_exp(-0.3*y.ptr.p_double[j]+0.08*x.ptr.p_double[i], _state)+2*ae_cos(ae_pi*(x.ptr.p_double[i]+1.2*y.ptr.p_double[j]+k), _state)+0.1*ae_cos(20*x.ptr.p_double[i]+15*y.ptr.p_double[j]+k, _state);
                        }
                    }
                }
                
                /*
                 * Test bilinear interpolation:
                 * * interpolation at the nodes
                 * * linearity
                 * * continuity
                 * * differentiation in the inner points
                 */
                spline2dbuildbilinear(&x, &y, &f, m, n, &c, _state);
                err = (double)(0);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        err = ae_maxreal(err, ae_fabs(f.ptr.pp_double[i][j]-spline2dcalc(&c, x.ptr.p_double[j], y.ptr.p_double[i], _state), _state), _state);
                    }
                }
                blerrors = blerrors||ae_fp_greater(err,10000*ae_machineepsilon);
                err = (double)(0);
                for(i=0; i<=m-2; i++)
                {
                    for(j=0; j<=n-2; j++)
                    {
                        
                        /*
                         * Test for linearity between grid points
                         * (test point - geometric center of the cell)
                         */
                        fm = spline2dcalc(&c, lx.ptr.p_double[2*j+1], ly.ptr.p_double[2*i+1], _state);
                        f1 = spline2dcalc(&c, lx.ptr.p_double[2*j], ly.ptr.p_double[2*i], _state);
                        f2 = spline2dcalc(&c, lx.ptr.p_double[2*j+2], ly.ptr.p_double[2*i], _state);
                        f3 = spline2dcalc(&c, lx.ptr.p_double[2*j+2], ly.ptr.p_double[2*i+2], _state);
                        f4 = spline2dcalc(&c, lx.ptr.p_double[2*j], ly.ptr.p_double[2*i+2], _state);
                        err = ae_maxreal(err, ae_fabs(0.25*(f1+f2+f3+f4)-fm, _state), _state);
                    }
                }
                blerrors = blerrors||ae_fp_greater(err,10000*ae_machineepsilon);
                testspline2dunit_lconst(&c, &lx, &ly, m, n, lstep, &l1, &l1x, &l1y, &l1xy, _state);
                testspline2dunit_lconst(&c, &lx, &ly, m, n, lstep/3, &l2, &l2x, &l2y, &l2xy, _state);
                blerrors = blerrors||ae_fp_greater(l2/l1,1.2);
                err = (double)(0);
                for(i=0; i<=m-2; i++)
                {
                    for(j=0; j<=n-2; j++)
                    {
                        spline2ddiff(&c, lx.ptr.p_double[2*j+1], ly.ptr.p_double[2*i+1], &v1, &v1x, &v1y, &v1xy, _state);
                        testspline2dunit_twodnumder(&c, lx.ptr.p_double[2*j+1], ly.ptr.p_double[2*i+1], h, &v2, &v2x, &v2y, &v2xy, _state);
                        err = ae_maxreal(err, ae_fabs(v1-v2, _state), _state);
                        err = ae_maxreal(err, ae_fabs(v1x-v2x, _state), _state);
                        err = ae_maxreal(err, ae_fabs(v1y-v2y, _state), _state);
                        err = ae_maxreal(err, ae_fabs(v1xy-v2xy, _state), _state);
                    }
                }
                dserrors = dserrors||ae_fp_greater(err,1.0E-3);
                uperrors = uperrors||!testspline2dunit_testunpack(&c, &lx, &ly, _state);
                lterrors = lterrors||!testspline2dunit_testlintrans(&c, 1, ax, bx, ay, by, _state);
                
                /*
                 * Lin.Trans. test for vector-function
                 */
                spline2dbuildbilinearv(&x, n, &y, m, &fv, d, &c, _state);
                lterrors = lterrors||!testspline2dunit_testlintrans(&c, d, ax, bx, ay, by, _state);
                
                /*
                 * Test bicubic interpolation.
                 * * interpolation at the nodes
                 * * smoothness
                 * * differentiation
                 */
                spline2dbuildbicubic(&x, &y, &f, m, n, &c, _state);
                err = (double)(0);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        err = ae_maxreal(err, ae_fabs(f.ptr.pp_double[i][j]-spline2dcalc(&c, x.ptr.p_double[j], y.ptr.p_double[i], _state), _state), _state);
                    }
                }
                bcerrors = bcerrors||ae_fp_greater(err,10000*ae_machineepsilon);
                testspline2dunit_lconst(&c, &lx, &ly, m, n, lstep, &l1, &l1x, &l1y, &l1xy, _state);
                testspline2dunit_lconst(&c, &lx, &ly, m, n, lstep/3, &l2, &l2x, &l2y, &l2xy, _state);
                bcerrors = bcerrors||ae_fp_greater(l2/l1,1.2);
                bcerrors = bcerrors||ae_fp_greater(l2x/l1x,1.2);
                bcerrors = bcerrors||ae_fp_greater(l2y/l1y,1.2);
                if( ae_fp_greater(l2xy,0.01)&&ae_fp_greater(l1xy,0.01) )
                {
                    
                    /*
                     * Cross-derivative continuity is tested only when
                     * bigger than 0.01. When the task size is too
                     * small, the d2F/dXdY is nearly zero and Lipschitz
                     * constant ratio is ill-conditioned.
                     */
                    bcerrors = bcerrors||ae_fp_greater(l2xy/l1xy,1.2);
                }
                err = (double)(0);
                for(i=0; i<=2*m-2; i++)
                {
                    for(j=0; j<=2*n-2; j++)
                    {
                        spline2ddiff(&c, lx.ptr.p_double[j], ly.ptr.p_double[i], &v1, &v1x, &v1y, &v1xy, _state);
                        testspline2dunit_twodnumder(&c, lx.ptr.p_double[j], ly.ptr.p_double[i], h, &v2, &v2x, &v2y, &v2xy, _state);
                        err = ae_maxreal(err, ae_fabs(v1-v2, _state), _state);
                        err = ae_maxreal(err, ae_fabs(v1x-v2x, _state), _state);
                        err = ae_maxreal(err, ae_fabs(v1y-v2y, _state), _state);
                        err = ae_maxreal(err, ae_fabs(v1xy-v2xy, _state), _state);
                    }
                }
                dserrors = dserrors||ae_fp_greater(err,1.0E-3);
                uperrors = uperrors||!testspline2dunit_testunpack(&c, &lx, &ly, _state);
                lterrors = lterrors||!testspline2dunit_testlintrans(&c, 1, ax, bx, ay, by, _state);
                
                /*
                 * Lin.Trans. test for vector-function
                 */
                spline2dbuildbicubicv(&x, n, &y, m, &fv, d, &c, _state);
                lterrors = lterrors||!testspline2dunit_testlintrans(&c, d, ax, bx, ay, by, _state);
                
                /*
                 * Copy test
                 */
                if( ae_fp_greater(ae_randomreal(_state),0.5) )
                {
                    spline2dbuildbicubic(&x, &y, &f, m, n, &c, _state);
                }
                else
                {
                    spline2dbuildbilinear(&x, &y, &f, m, n, &c, _state);
                }
                testspline2dunit_unsetspline2d(&c2, _state);
                spline2dcopy(&c, &c2, _state);
                err = (double)(0);
                for(i=1; i<=5; i++)
                {
                    t1 = ax+(bx-ax)*ae_randomreal(_state);
                    t2 = ay+(by-ay)*ae_randomreal(_state);
                    err = ae_maxreal(err, ae_fabs(spline2dcalc(&c, t1, t2, _state)-spline2dcalc(&c2, t1, t2, _state), _state), _state);
                }
                cperrors = cperrors||ae_fp_greater(err,10000*ae_machineepsilon);
                
                /*
                 * Special symmetry test
                 */
                err = (double)(0);
                for(jobtype=0; jobtype<=1; jobtype++)
                {
                    
                    /*
                     * Prepare
                     */
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ft.ptr.pp_double[j][i] = f.ptr.pp_double[i][j];
                        }
                    }
                    if( jobtype==0 )
                    {
                        spline2dbuildbilinear(&x, &y, &f, m, n, &c, _state);
                        spline2dbuildbilinear(&y, &x, &ft, n, m, &c2, _state);
                    }
                    else
                    {
                        spline2dbuildbicubic(&x, &y, &f, m, n, &c, _state);
                        spline2dbuildbicubic(&y, &x, &ft, n, m, &c2, _state);
                    }
                    
                    /*
                     * Test
                     */
                    for(i=1; i<=10; i++)
                    {
                        t1 = ax+(bx-ax)*ae_randomreal(_state);
                        t2 = ay+(by-ay)*ae_randomreal(_state);
                        err = ae_maxreal(err, ae_fabs(spline2dcalc(&c, t1, t2, _state)-spline2dcalc(&c2, t2, t1, _state), _state), _state);
                    }
                }
                syerrors = syerrors||ae_fp_greater(err,10000*ae_machineepsilon);
            }
        }
    }
    
    /*
     * Test resample
     */
    for(m=2; m<=6; m++)
    {
        for(n=2; n<=6; n++)
        {
            ae_matrix_set_length(&f, m-1+1, n-1+1, _state);
            ae_vector_set_length(&x, n-1+1, _state);
            ae_vector_set_length(&y, m-1+1, _state);
            for(j=0; j<=n-1; j++)
            {
                x.ptr.p_double[j] = (double)j/(double)(n-1);
            }
            for(i=0; i<=m-1; i++)
            {
                y.ptr.p_double[i] = (double)i/(double)(m-1);
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    f.ptr.pp_double[i][j] = ae_exp(0.6*x.ptr.p_double[j], _state)-ae_exp(-0.3*y.ptr.p_double[i]+0.08*x.ptr.p_double[j], _state)+2*ae_cos(ae_pi*(x.ptr.p_double[j]+1.2*y.ptr.p_double[i]), _state)+0.1*ae_cos(20*x.ptr.p_double[j]+15*y.ptr.p_double[i], _state);
                }
            }
            for(m2=2; m2<=6; m2++)
            {
                for(n2=2; n2<=6; n2++)
                {
                    for(pass=1; pass<=passcount; pass++)
                    {
                        for(jobtype=0; jobtype<=1; jobtype++)
                        {
                            if( jobtype==0 )
                            {
                                spline2dresamplebilinear(&f, m, n, &fr, m2, n2, _state);
                                spline2dbuildbilinear(&x, &y, &f, m, n, &c, _state);
                            }
                            if( jobtype==1 )
                            {
                                spline2dresamplebicubic(&f, m, n, &fr, m2, n2, _state);
                                spline2dbuildbicubic(&x, &y, &f, m, n, &c, _state);
                            }
                            err = (double)(0);
                            mf = (double)(0);
                            for(i=0; i<=m2-1; i++)
                            {
                                for(j=0; j<=n2-1; j++)
                                {
                                    v1 = spline2dcalc(&c, (double)j/(double)(n2-1), (double)i/(double)(m2-1), _state);
                                    v2 = fr.ptr.pp_double[i][j];
                                    err = ae_maxreal(err, ae_fabs(v1-v2, _state), _state);
                                    mf = ae_maxreal(mf, ae_fabs(v1, _state), _state);
                                }
                            }
                            if( jobtype==0 )
                            {
                                rlerrors = rlerrors||ae_fp_greater(err/mf,10000*ae_machineepsilon);
                            }
                            if( jobtype==1 )
                            {
                                rcerrors = rcerrors||ae_fp_greater(err/mf,10000*ae_machineepsilon);
                            }
                        }
                    }
                }
            }
        }
    }
    
    /*
     * Test for vector-function
     */
    vferrors = testspline2dunit_testspline2dvf(ae_true, _state);
    
    /*
     * Report
     */
    waserrors = ((((((((blerrors||bcerrors)||dserrors)||cperrors)||uperrors)||lterrors)||syerrors)||rlerrors)||rcerrors)||vferrors;
    if( !silent )
    {
        printf("TESTING 2D SPLINE\n");
        
        /*
         * Normal tests
         */
        printf("BILINEAR TEST:                           ");
        if( blerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("BICUBIC TEST:                            ");
        if( bcerrors )
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
        printf("COPY/SERIALIZE TEST:                     ");
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
        printf("SPECIAL SYMMETRY TEST:                   ");
        if( syerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("BILINEAR RESAMPLING TEST:                ");
        if( rlerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("BICUBIC RESAMPLING TEST:                 ");
        if( rcerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("VECTOR FUNCTION TEST:                    ");
        if( vferrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        
        /*
         * Summary
         */
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
ae_bool _pexec_testspline2d(ae_bool silent, ae_state *_state)
{
    return testspline2d(silent, _state);
}


/*************************************************************************
Lipschitz constants for spline inself, first and second derivatives.
*************************************************************************/
static void testspline2dunit_lconst(spline2dinterpolant* c,
     /* Real    */ ae_vector* lx,
     /* Real    */ ae_vector* ly,
     ae_int_t m,
     ae_int_t n,
     double lstep,
     double* lc,
     double* lcx,
     double* lcy,
     double* lcxy,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double f1;
    double f2;
    double f3;
    double f4;
    double fx1;
    double fx2;
    double fx3;
    double fx4;
    double fy1;
    double fy2;
    double fy3;
    double fy4;
    double fxy1;
    double fxy2;
    double fxy3;
    double fxy4;
    double s2lstep;

    *lc = 0;
    *lcx = 0;
    *lcy = 0;
    *lcxy = 0;

    *lc = (double)(0);
    *lcx = (double)(0);
    *lcy = (double)(0);
    *lcxy = (double)(0);
    s2lstep = ae_sqrt((double)(2), _state)*lstep;
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            
            /*
             * Calculate
             */
            testspline2dunit_twodnumder(c, lx->ptr.p_double[j]-lstep/2, ly->ptr.p_double[i]-lstep/2, lstep/4, &f1, &fx1, &fy1, &fxy1, _state);
            testspline2dunit_twodnumder(c, lx->ptr.p_double[j]+lstep/2, ly->ptr.p_double[i]-lstep/2, lstep/4, &f2, &fx2, &fy2, &fxy2, _state);
            testspline2dunit_twodnumder(c, lx->ptr.p_double[j]+lstep/2, ly->ptr.p_double[i]+lstep/2, lstep/4, &f3, &fx3, &fy3, &fxy3, _state);
            testspline2dunit_twodnumder(c, lx->ptr.p_double[j]-lstep/2, ly->ptr.p_double[i]+lstep/2, lstep/4, &f4, &fx4, &fy4, &fxy4, _state);
            
            /*
             * Lipschitz constant for the function itself
             */
            *lc = ae_maxreal(*lc, ae_fabs((f1-f2)/lstep, _state), _state);
            *lc = ae_maxreal(*lc, ae_fabs((f2-f3)/lstep, _state), _state);
            *lc = ae_maxreal(*lc, ae_fabs((f3-f4)/lstep, _state), _state);
            *lc = ae_maxreal(*lc, ae_fabs((f4-f1)/lstep, _state), _state);
            *lc = ae_maxreal(*lc, ae_fabs((f1-f3)/s2lstep, _state), _state);
            *lc = ae_maxreal(*lc, ae_fabs((f2-f4)/s2lstep, _state), _state);
            
            /*
             * Lipschitz constant for the first derivative
             */
            *lcx = ae_maxreal(*lcx, ae_fabs((fx1-fx2)/lstep, _state), _state);
            *lcx = ae_maxreal(*lcx, ae_fabs((fx2-fx3)/lstep, _state), _state);
            *lcx = ae_maxreal(*lcx, ae_fabs((fx3-fx4)/lstep, _state), _state);
            *lcx = ae_maxreal(*lcx, ae_fabs((fx4-fx1)/lstep, _state), _state);
            *lcx = ae_maxreal(*lcx, ae_fabs((fx1-fx3)/s2lstep, _state), _state);
            *lcx = ae_maxreal(*lcx, ae_fabs((fx2-fx4)/s2lstep, _state), _state);
            
            /*
             * Lipschitz constant for the first derivative
             */
            *lcy = ae_maxreal(*lcy, ae_fabs((fy1-fy2)/lstep, _state), _state);
            *lcy = ae_maxreal(*lcy, ae_fabs((fy2-fy3)/lstep, _state), _state);
            *lcy = ae_maxreal(*lcy, ae_fabs((fy3-fy4)/lstep, _state), _state);
            *lcy = ae_maxreal(*lcy, ae_fabs((fy4-fy1)/lstep, _state), _state);
            *lcy = ae_maxreal(*lcy, ae_fabs((fy1-fy3)/s2lstep, _state), _state);
            *lcy = ae_maxreal(*lcy, ae_fabs((fy2-fy4)/s2lstep, _state), _state);
            
            /*
             * Lipschitz constant for the cross-derivative
             */
            *lcxy = ae_maxreal(*lcxy, ae_fabs((fxy1-fxy2)/lstep, _state), _state);
            *lcxy = ae_maxreal(*lcxy, ae_fabs((fxy2-fxy3)/lstep, _state), _state);
            *lcxy = ae_maxreal(*lcxy, ae_fabs((fxy3-fxy4)/lstep, _state), _state);
            *lcxy = ae_maxreal(*lcxy, ae_fabs((fxy4-fxy1)/lstep, _state), _state);
            *lcxy = ae_maxreal(*lcxy, ae_fabs((fxy1-fxy3)/s2lstep, _state), _state);
            *lcxy = ae_maxreal(*lcxy, ae_fabs((fxy2-fxy4)/s2lstep, _state), _state);
        }
    }
}


/*************************************************************************
Numerical differentiation.
*************************************************************************/
static void testspline2dunit_twodnumder(spline2dinterpolant* c,
     double x,
     double y,
     double h,
     double* f,
     double* fx,
     double* fy,
     double* fxy,
     ae_state *_state)
{

    *f = 0;
    *fx = 0;
    *fy = 0;
    *fxy = 0;

    *f = spline2dcalc(c, x, y, _state);
    *fx = (spline2dcalc(c, x+h, y, _state)-spline2dcalc(c, x-h, y, _state))/(2*h);
    *fy = (spline2dcalc(c, x, y+h, _state)-spline2dcalc(c, x, y-h, _state))/(2*h);
    *fxy = (spline2dcalc(c, x+h, y+h, _state)-spline2dcalc(c, x-h, y+h, _state)-spline2dcalc(c, x+h, y-h, _state)+spline2dcalc(c, x-h, y-h, _state))/ae_sqr(2*h, _state);
}


/*************************************************************************
Unpack test
*************************************************************************/
static ae_bool testspline2dunit_testunpack(spline2dinterpolant* c,
     /* Real    */ ae_vector* lx,
     /* Real    */ ae_vector* ly,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;
    ae_int_t m;
    ae_int_t ci;
    ae_int_t cj;
    ae_int_t p;
    double err;
    double tx;
    double ty;
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
    spline2dunpack(c, &m, &n, &tbl, _state);
    for(i=0; i<=m-2; i++)
    {
        for(j=0; j<=n-2; j++)
        {
            for(pass=1; pass<=passcount; pass++)
            {
                p = (n-1)*i+j;
                tx = (0.001+0.999*ae_randomreal(_state))*(tbl.ptr.pp_double[p][1]-tbl.ptr.pp_double[p][0]);
                ty = (0.001+0.999*ae_randomreal(_state))*(tbl.ptr.pp_double[p][3]-tbl.ptr.pp_double[p][2]);
                
                /*
                 * Interpolation properties
                 */
                v1 = (double)(0);
                for(ci=0; ci<=3; ci++)
                {
                    for(cj=0; cj<=3; cj++)
                    {
                        v1 = v1+tbl.ptr.pp_double[p][4+ci*4+cj]*ae_pow(tx, (double)(ci), _state)*ae_pow(ty, (double)(cj), _state);
                    }
                }
                v2 = spline2dcalc(c, tbl.ptr.pp_double[p][0]+tx, tbl.ptr.pp_double[p][2]+ty, _state);
                err = ae_maxreal(err, ae_fabs(v1-v2, _state), _state);
                
                /*
                 * Grid correctness
                 */
                err = ae_maxreal(err, ae_fabs(lx->ptr.p_double[2*j]-tbl.ptr.pp_double[p][0], _state), _state);
                err = ae_maxreal(err, ae_fabs(lx->ptr.p_double[2*(j+1)]-tbl.ptr.pp_double[p][1], _state), _state);
                err = ae_maxreal(err, ae_fabs(ly->ptr.p_double[2*i]-tbl.ptr.pp_double[p][2], _state), _state);
                err = ae_maxreal(err, ae_fabs(ly->ptr.p_double[2*(i+1)]-tbl.ptr.pp_double[p][3], _state), _state);
            }
        }
    }
    result = ae_fp_less(err,10000*ae_machineepsilon);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
LinTrans test for scalar
*************************************************************************/
static ae_bool testspline2dunit_testlintrans(spline2dinterpolant* c,
     ae_int_t d,
     double ax,
     double bx,
     double ay,
     double by,
     ae_state *_state)
{
    ae_frame _frame_block;
    double err;
    double a1;
    double a2;
    double b1;
    double b2;
    double tx;
    double ty;
    double vx;
    double vy;
    ae_vector v1;
    ae_vector v2;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t xjob;
    ae_int_t yjob;
    spline2dinterpolant c2;
    ae_int_t di;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&v1, 0, DT_REAL, _state);
    ae_vector_init(&v2, 0, DT_REAL, _state);
    _spline2dinterpolant_init(&c2, _state);

    passcount = 5;
    err = (double)(0);
    for(xjob=0; xjob<=1; xjob++)
    {
        for(yjob=0; yjob<=1; yjob++)
        {
            for(pass=1; pass<=passcount; pass++)
            {
                
                /*
                 * Prepare
                 */
                do
                {
                    a1 = 2*ae_randomreal(_state)-1;
                }
                while(ae_fp_eq(a1,(double)(0)));
                a1 = a1*xjob;
                b1 = 2*ae_randomreal(_state)-1;
                do
                {
                    a2 = 2*ae_randomreal(_state)-1;
                }
                while(ae_fp_eq(a2,(double)(0)));
                a2 = a2*yjob;
                b2 = 2*ae_randomreal(_state)-1;
                
                /*
                 * Test XY
                 */
                spline2dcopy(c, &c2, _state);
                spline2dlintransxy(&c2, a1, b1, a2, b2, _state);
                tx = ax+ae_randomreal(_state)*(bx-ax);
                ty = ay+ae_randomreal(_state)*(by-ay);
                if( xjob==0 )
                {
                    tx = b1;
                    vx = ax+ae_randomreal(_state)*(bx-ax);
                }
                else
                {
                    vx = (tx-b1)/a1;
                }
                if( yjob==0 )
                {
                    ty = b2;
                    vy = ay+ae_randomreal(_state)*(by-ay);
                }
                else
                {
                    vy = (ty-b2)/a2;
                }
                spline2dcalcv(c, tx, ty, &v1, _state);
                spline2dcalcv(&c2, vx, vy, &v2, _state);
                for(di=0; di<=d-1; di++)
                {
                    err = ae_maxreal(err, ae_fabs(v1.ptr.p_double[di]-v2.ptr.p_double[di], _state), _state);
                }
                
                /*
                 * Test F
                 */
                spline2dcopy(c, &c2, _state);
                spline2dlintransf(&c2, a1, b1, _state);
                tx = ax+ae_randomreal(_state)*(bx-ax);
                ty = ay+ae_randomreal(_state)*(by-ay);
                spline2dcalcv(c, tx, ty, &v1, _state);
                spline2dcalcv(&c2, tx, ty, &v2, _state);
                for(di=0; di<=d-1; di++)
                {
                    err = ae_maxreal(err, ae_fabs(a1*v1.ptr.p_double[di]+b1-v2.ptr.p_double[di], _state), _state);
                }
            }
        }
    }
    result = ae_fp_less(err,10000*ae_machineepsilon);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Unset spline, i.e. initialize it with random garbage
*************************************************************************/
static void testspline2dunit_unsetspline2d(spline2dinterpolant* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_matrix f;

    ae_frame_make(_state, &_frame_block);
    _spline2dinterpolant_clear(c);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_matrix_init(&f, 0, 0, DT_REAL, _state);

    ae_vector_set_length(&x, 2, _state);
    ae_vector_set_length(&y, 2, _state);
    ae_matrix_set_length(&f, 2, 2, _state);
    x.ptr.p_double[0] = (double)(-1);
    x.ptr.p_double[1] = (double)(1);
    y.ptr.p_double[0] = (double)(-1);
    y.ptr.p_double[1] = (double)(1);
    f.ptr.pp_double[0][0] = (double)(0);
    f.ptr.pp_double[0][1] = (double)(0);
    f.ptr.pp_double[1][0] = (double)(0);
    f.ptr.pp_double[1][1] = (double)(0);
    spline2dbuildbilinear(&x, &y, &f, 2, 2, c, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
The function check, that follow functions works correctly:
    Spline2DBilinearV, Spline2DBicubicV, Spline2DCalcV
    and Spline2DUnpackV.
*************************************************************************/
static ae_bool testspline2dunit_testspline2dvf(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    spline2dinterpolant vc;
    spline2dinterpolant sc;
    double range;
    ae_vector x;
    ae_vector y;
    ae_vector f;
    double rndx;
    double rndy;
    ae_int_t nrnd;
    ae_vector resf;
    ae_matrix ef;
    double resef;
    ae_int_t m;
    ae_int_t n;
    ae_int_t d;
    ae_int_t tstn;
    ae_int_t tstm;
    ae_int_t tstd;
    ae_matrix tsttbl0;
    ae_matrix tsttbl1;
    double eps;
    double st;
    ae_int_t p0;
    ae_int_t p1;
    ae_int_t variant;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t i0;
    ae_int_t j0;
    double xmin;
    double xmax;
    double ymin;
    double ymax;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _spline2dinterpolant_init(&vc, _state);
    _spline2dinterpolant_init(&sc, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&f, 0, DT_REAL, _state);
    ae_vector_init(&resf, 0, DT_REAL, _state);
    ae_matrix_init(&ef, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tsttbl0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tsttbl1, 0, 0, DT_REAL, _state);

    eps = 10000.0*ae_machineepsilon;
    st = 0.1;
    passcount = 5;
    for(pass=1; pass<=passcount; pass++)
    {
        for(variant=1; variant<=2; variant++)
        {
            range = ae_randominteger(71, _state)+30.0;
            nrnd = ae_randominteger(26, _state)+25;
            range = (double)(ae_randominteger(71, _state)+30);
            m = ae_randominteger(4, _state)+2;
            n = ae_randominteger(4, _state)+2;
            d = ae_randominteger(3, _state)+1;
            rvectorsetlengthatleast(&x, n, _state);
            rvectorsetlengthatleast(&y, m, _state);
            rvectorsetlengthatleast(&f, n*m*d, _state);
            rmatrixsetlengthatleast(&ef, m, n, _state);
            
            /*
             * Build a grid for spline
             */
            x.ptr.p_double[0] = range*(2*ae_randomreal(_state)-1);
            y.ptr.p_double[0] = range*(2*ae_randomreal(_state)-1);
            for(i=1; i<=n-1; i++)
            {
                x.ptr.p_double[i] = x.ptr.p_double[i-1]+st+ae_randomreal(_state);
            }
            for(i=1; i<=m-1; i++)
            {
                y.ptr.p_double[i] = y.ptr.p_double[i-1]+st+ae_randomreal(_state);
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    for(k=0; k<=d-1; k++)
                    {
                        f.ptr.p_double[d*(n*j+i)+k] = range*(2*ae_randomreal(_state)-1);
                    }
                }
            }
            xmin = x.ptr.p_double[0];
            xmax = x.ptr.p_double[n-1];
            ymin = y.ptr.p_double[0];
            ymax = y.ptr.p_double[m-1];
            
            /*
             * Build a spline
             */
            if( variant==1 )
            {
                spline2dbuildbilinearv(&x, n, &y, m, &f, d, &vc, _state);
            }
            if( variant==2 )
            {
                spline2dbuildbicubicv(&x, n, &y, m, &f, d, &vc, _state);
            }
            
            /*
             * Part of test, which shows that Spline2DBuildBilinearV function
             * works correctly.
             * And there is test for Spline2DUnpackV.
             */
            spline2dunpackv(&vc, &tstm, &tstn, &tstd, &tsttbl1, _state);
            if( (tstm!=m||tstn!=n)||tstd!=d )
            {
                if( !silent )
                {
                    printf("TestSpline2DVF fail Spline2DUnpack:\n");
                    printf("    TstM=%0d; M=%0d;\n    TstN=%0d; N=%0d;\n    TstD=%0d; D=%0d.\n",
                        (int)(tstm),
                        (int)(m),
                        (int)(tstn),
                        (int)(n),
                        (int)(tstd),
                        (int)(d));
                }
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(k=0; k<=d-1; k++)
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        ef.ptr.pp_double[i][j] = f.ptr.p_double[d*(i*n+j)+k];
                    }
                }
                if( variant==1 )
                {
                    spline2dbuildbilinear(&x, &y, &ef, m, n, &sc, _state);
                }
                if( variant==2 )
                {
                    spline2dbuildbicubic(&x, &y, &ef, m, n, &sc, _state);
                }
                spline2dunpack(&sc, &tstm, &tstn, &tsttbl0, _state);
                if( tstm!=m||tstn!=n )
                {
                    if( !silent )
                    {
                        printf("TestSpline2DVF fail Spline2DUnpack:\n");
                        printf("    TstM=%0d; M=%0d;\n    TstN=%0d; N=%0d.\n",
                            (int)(tstm),
                            (int)(m),
                            (int)(tstn),
                            (int)(n));
                    }
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=m-2; i++)
                {
                    for(j=0; j<=n-2; j++)
                    {
                        p0 = i*(n-1)+j;
                        p1 = d*p0;
                        for(i0=0; i0<=19; i0++)
                        {
                            if( ae_fp_neq(tsttbl1.ptr.pp_double[p1+k][i0],tsttbl0.ptr.pp_double[p0][i0]) )
                            {
                                if( !silent )
                                {
                                    printf("TestSpline2DVF: Tbl error\n");
                                }
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                }
            }
            
            /*
             * Part of test, which shows that functions Spline2DCalcVBuf and Spline2DCalcV   
             * works correctly
             */
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    
                    /*
                     * Test for grid points
                     */
                    spline2dcalcv(&vc, x.ptr.p_double[i], y.ptr.p_double[j], &resf, _state);
                    for(k=0; k<=d-1; k++)
                    {
                        for(i0=0; i0<=m-1; i0++)
                        {
                            for(j0=0; j0<=n-1; j0++)
                            {
                                ef.ptr.pp_double[i0][j0] = f.ptr.p_double[d*(i0*n+j0)+k];
                            }
                        }
                        if( variant==1 )
                        {
                            spline2dbuildbilinear(&x, &y, &ef, m, n, &sc, _state);
                        }
                        if( variant==2 )
                        {
                            spline2dbuildbicubic(&x, &y, &ef, m, n, &sc, _state);
                        }
                        resef = spline2dcalc(&sc, x.ptr.p_double[i], y.ptr.p_double[j], _state);
                        if( ae_fp_greater(ae_fabs(resf.ptr.p_double[k]-resef, _state),eps) )
                        {
                            if( !silent )
                            {
                                printf("TestSpline2DVF fail Spline2DCalcV:\n");
                                printf("    %0.5f=|resF[%0d]-resEF|=|%0.5f-%0.5f|>Eps=%0.2e;\n",
                                    (double)(ae_fabs(resf.ptr.p_double[k]-resef, _state)),
                                    (int)(k),
                                    (double)(resf.ptr.p_double[k]),
                                    (double)(resef),
                                    (double)(eps));
                                printf("    resF[%0d]=%0.5f;\n",
                                    (int)(k),
                                    (double)(resf.ptr.p_double[k]));
                                printf("    resEF=%0.5f.\n",
                                    (double)(resef));
                            }
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                    }
                }
            }
            
            /*
             * Test for random points
             */
            for(i=1; i<=nrnd; i++)
            {
                rndx = xmin+(xmax-xmin)*ae_randomreal(_state);
                rndy = ymin+(ymax-ymin)*ae_randomreal(_state);
                
                /*
                 * Calculate value for vector-function in random point
                 */
                spline2dcalcv(&vc, rndx, rndy, &resf, _state);
                for(k=0; k<=d-1; k++)
                {
                    
                    /*
                     * Build spline for scalar-function, each of which correspond
                     * to one of vector-function's components.
                     */
                    for(i0=0; i0<=m-1; i0++)
                    {
                        for(j0=0; j0<=n-1; j0++)
                        {
                            ef.ptr.pp_double[i0][j0] = f.ptr.p_double[d*(i0*n+j0)+k];
                        }
                    }
                    if( variant==1 )
                    {
                        spline2dbuildbilinear(&x, &y, &ef, m, n, &sc, _state);
                    }
                    if( variant==2 )
                    {
                        spline2dbuildbicubic(&x, &y, &ef, m, n, &sc, _state);
                    }
                    resef = spline2dcalc(&sc, rndx, rndy, _state);
                    if( ae_fp_greater(ae_fabs(resf.ptr.p_double[k]-resef, _state),eps) )
                    {
                        if( !silent )
                        {
                            printf("TestSpline2DVF fail Spline2DCalcV:\n");
                            printf("    %0.5f=|resF[%0d]-resEF|=|%0.5f-%0.5f|>Eps=%0.2e;\n",
                                (double)(ae_fabs(resf.ptr.p_double[k]-resef, _state)),
                                (int)(k),
                                (double)(resf.ptr.p_double[k]),
                                (double)(resef),
                                (double)(eps));
                            printf("    resF[%0d]=%0.5f;\n",
                                (int)(k),
                                (double)(resf.ptr.p_double[k]));
                            printf("    resEF=%0.5f.\n",
                                (double)(resef));
                        }
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
        }
    }
    if( !silent )
    {
        printf("TestSpline2DVF: OK\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*$ End $*/
