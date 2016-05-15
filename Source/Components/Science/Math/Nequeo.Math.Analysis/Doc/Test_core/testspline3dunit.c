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
#include "testspline3dunit.h"


/*$ Declarations $*/
static ae_bool testspline3dunit_basictest(ae_state *_state);
static ae_bool testspline3dunit_testunpack(ae_state *_state);
static ae_bool testspline3dunit_testlintrans(ae_state *_state);
static ae_bool testspline3dunit_testtrilinearresample(ae_state *_state);
static void testspline3dunit_buildrndgrid(ae_bool isvect,
     ae_bool reorder,
     ae_int_t* n,
     ae_int_t* m,
     ae_int_t* l,
     ae_int_t* d,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* z,
     /* Real    */ ae_vector* f,
     ae_state *_state);


/*$ Body $*/


ae_bool testspline3d(ae_bool silence, ae_state *_state)
{
    ae_bool waserrors;
    ae_bool basicerr;
    ae_bool unpackerr;
    ae_bool lintransferr;
    ae_bool trilinreserr;
    ae_bool result;


    basicerr = testspline3dunit_basictest(_state);
    unpackerr = testspline3dunit_testunpack(_state);
    lintransferr = testspline3dunit_testlintrans(_state);
    trilinreserr = testspline3dunit_testtrilinearresample(_state);
    waserrors = ((basicerr||unpackerr)||lintransferr)||trilinreserr;
    if( !silence )
    {
        printf("TESTING 3D SPLINE\n");
        printf("BASIC TEST:                              ");
        if( basicerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("UNPACK TEST:                             ");
        if( unpackerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("LIN_TRANSF TEST:                         ");
        if( lintransferr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("TRILINEAR RESAMPLING TEST:               ");
        if( trilinreserr )
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
            printf("TEST FAILED");
        }
        else
        {
            printf("TEST PASSED");
        }
        printf("\n\n");
    }
    result = !waserrors;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testspline3d(ae_bool silence, ae_state *_state)
{
    return testspline3d(silence, _state);
}


/*************************************************************************
The function does test basic functionality.
*************************************************************************/
static ae_bool testspline3dunit_basictest(ae_state *_state)
{
    ae_frame _frame_block;
    spline3dinterpolant c;
    spline3dinterpolant cc;
    ae_vector vvf;
    double vsf;
    ae_int_t d;
    ae_int_t m;
    ae_int_t n;
    ae_int_t l;
    ae_vector x;
    ae_vector y;
    ae_vector z;
    ae_vector sf;
    ae_vector vf;
    double eps;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t offs;
    ae_int_t di;
    double ax;
    double ay;
    double az;
    double axy;
    double ayz;
    double vx;
    double vy;
    double vz;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _spline3dinterpolant_init(&c, _state);
    _spline3dinterpolant_init(&cc, _state);
    ae_vector_init(&vvf, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&z, 0, DT_REAL, _state);
    ae_vector_init(&sf, 0, DT_REAL, _state);
    ae_vector_init(&vf, 0, DT_REAL, _state);

    eps = 1000*ae_machineepsilon;
    
    /*
     * Test spline ability to reproduce D-dimensional vector function
     *     f[idx](x,y,z) = idx+AX*x + AY*y + AZ*z + AXY*x*y + AYZ*y*z
     * with random AX/AY/...
     *
     * We generate random test function, build spline, then evaluate
     * it in the random test point.
     */
    for(d=1; d<=3; d++)
    {
        n = 2+ae_randominteger(4, _state);
        m = 2+ae_randominteger(4, _state);
        l = 2+ae_randominteger(4, _state);
        ae_vector_set_length(&x, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = (double)(i);
        }
        ae_vector_set_length(&y, m, _state);
        for(i=0; i<=m-1; i++)
        {
            y.ptr.p_double[i] = (double)(i);
        }
        ae_vector_set_length(&z, l, _state);
        for(i=0; i<=l-1; i++)
        {
            z.ptr.p_double[i] = (double)(i);
        }
        ae_vector_set_length(&vf, l*m*n*d, _state);
        offs = 0;
        ax = 2*ae_randomreal(_state)-1;
        ay = 2*ae_randomreal(_state)-1;
        az = 2*ae_randomreal(_state)-1;
        axy = 2*ae_randomreal(_state)-1;
        ayz = 2*ae_randomreal(_state)-1;
        for(k=0; k<=l-1; k++)
        {
            for(j=0; j<=m-1; j++)
            {
                for(i=0; i<=n-1; i++)
                {
                    for(di=0; di<=d-1; di++)
                    {
                        vf.ptr.p_double[offs] = di+ax*i+ay*j+az*k+axy*i*j+ayz*j*k;
                        offs = offs+1;
                    }
                }
            }
        }
        spline3dbuildtrilinearv(&x, n, &y, m, &z, l, &vf, d, &c, _state);
        vx = ae_randomreal(_state)*n;
        vy = ae_randomreal(_state)*m;
        vz = ae_randomreal(_state)*l;
        spline3dcalcv(&c, vx, vy, vz, &vf, _state);
        for(di=0; di<=d-1; di++)
        {
            if( ae_fp_greater(ae_fabs(di+ax*vx+ay*vy+az*vz+axy*vx*vy+ayz*vy*vz-vf.ptr.p_double[di], _state),eps) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
        if( d==1 )
        {
            vsf = spline3dcalc(&c, vx, vy, vz, _state);
            if( ae_fp_greater(ae_fabs(ax*vx+ay*vy+az*vz+axy*vx*vy+ayz*vy*vz-vsf, _state),eps) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
    }
    
    /*
     * Generate random grid and test function.
     * Test spline ability to reproduce function values at grid nodes.
     */
    passcount = 20;
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Prepare a model and check that functions (Spline3DBuildTrilinear,
         * Spline3DCalc,Spline3DCalcV) work correctly and
         */
        testspline3dunit_buildrndgrid(ae_true, ae_true, &n, &m, &l, &d, &x, &y, &z, &vf, _state);
        rvectorsetlengthatleast(&sf, n*m*l, _state);
        
        /*
         * Check that the model's values are equal to the function's values
         * in grid points
         */
        spline3dbuildtrilinearv(&x, n, &y, m, &z, l, &vf, d, &c, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                for(k=0; k<=l-1; k++)
                {
                    spline3dcalcv(&c, x.ptr.p_double[i], y.ptr.p_double[j], z.ptr.p_double[k], &vvf, _state);
                    for(di=0; di<=d-1; di++)
                    {
                        if( ae_fp_greater(ae_fabs(vf.ptr.p_double[d*(n*(m*k+j)+i)+di]-vvf.ptr.p_double[di], _state),eps) )
                        {
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                    }
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Unpack/UnpackV test
*************************************************************************/
static ae_bool testspline3dunit_testunpack(ae_state *_state)
{
    ae_frame _frame_block;
    spline3dinterpolant c;
    ae_matrix tbl0;
    ae_matrix tbl1;
    ae_int_t n;
    ae_int_t m;
    ae_int_t l;
    ae_int_t d;
    ae_int_t sz;
    ae_int_t un;
    ae_int_t um;
    ae_int_t ul;
    ae_int_t ud;
    ae_int_t ust;
    ae_int_t uvn;
    ae_int_t uvm;
    ae_int_t uvl;
    ae_int_t uvd;
    ae_int_t uvst;
    ae_int_t ci;
    ae_int_t cj;
    ae_int_t ck;
    ae_vector x;
    ae_vector y;
    ae_vector z;
    ae_vector sf;
    ae_vector vf;
    ae_int_t p0;
    ae_int_t p1;
    double tx;
    double ty;
    double tz;
    double v1;
    double v2;
    double err;
    ae_int_t pass;
    ae_int_t passcount;
    ae_bool bperr;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t di;
    ae_int_t i0;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _spline3dinterpolant_init(&c, _state);
    ae_matrix_init(&tbl0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tbl1, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&z, 0, DT_REAL, _state);
    ae_vector_init(&sf, 0, DT_REAL, _state);
    ae_vector_init(&vf, 0, DT_REAL, _state);

    passcount = 20;
    err = (double)(0);
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * generate random grid.
         * NOTE: for this test we need ordered grid, i.e. grid
         *       with nodes in ascending order
         */
        testspline3dunit_buildrndgrid(ae_true, ae_false, &n, &m, &l, &d, &x, &y, &z, &vf, _state);
        sz = n*m*l;
        rvectorsetlengthatleast(&sf, sz, _state);
        spline3dbuildtrilinearv(&x, n, &y, m, &z, l, &vf, d, &c, _state);
        spline3dunpackv(&c, &uvn, &uvm, &uvl, &uvd, &uvst, &tbl0, _state);
        for(di=0; di<=d-1; di++)
        {
            
            /*
             * DI-th component copy of a vector-function to
             * a scalar function
             */
            for(i=0; i<=sz-1; i++)
            {
                sf.ptr.p_double[i] = vf.ptr.p_double[d*i+di];
            }
            spline3dbuildtrilinearv(&x, n, &y, m, &z, l, &sf, 1, &c, _state);
            spline3dunpackv(&c, &un, &um, &ul, &ud, &ust, &tbl1, _state);
            for(i=0; i<=n-2; i++)
            {
                for(j=0; j<=m-2; j++)
                {
                    for(k=0; k<=l-2; k++)
                    {
                        p1 = (n-1)*((m-1)*k+j)+i;
                        p0 = d*p1+di;
                        
                        /*
                         * Check that all components are correct:
                         *  *first check, that unpacked componets are equal
                         *   to packed components;
                         */
                        bperr = (((((((((((((((((un!=n||um!=m)||ul!=l)||ae_fp_neq(tbl1.ptr.pp_double[p1][0],x.ptr.p_double[i]))||ae_fp_neq(tbl1.ptr.pp_double[p1][1],x.ptr.p_double[i+1]))||ae_fp_neq(tbl1.ptr.pp_double[p1][2],y.ptr.p_double[j]))||ae_fp_neq(tbl1.ptr.pp_double[p1][3],y.ptr.p_double[j+1]))||ae_fp_neq(tbl1.ptr.pp_double[p1][4],z.ptr.p_double[k]))||ae_fp_neq(tbl1.ptr.pp_double[p1][5],z.ptr.p_double[k+1]))||uvn!=n)||uvm!=m)||uvl!=l)||uvd!=d)||ae_fp_neq(tbl0.ptr.pp_double[p0][0],x.ptr.p_double[i]))||ae_fp_neq(tbl0.ptr.pp_double[p0][1],x.ptr.p_double[i+1]))||ae_fp_neq(tbl0.ptr.pp_double[p0][2],y.ptr.p_double[j]))||ae_fp_neq(tbl0.ptr.pp_double[p0][3],y.ptr.p_double[j+1]))||ae_fp_neq(tbl0.ptr.pp_double[p0][4],z.ptr.p_double[k]))||ae_fp_neq(tbl0.ptr.pp_double[p0][5],z.ptr.p_double[k+1]);
                        
                        /*
                         *  *check, that all components unpacked by Unpack
                         *   function are equal to all components unpacked
                         *   by UnpackV function.
                         */
                        for(i0=0; i0<=13; i0++)
                        {
                            bperr = bperr||ae_fp_neq(tbl0.ptr.pp_double[p0][i0],tbl1.ptr.pp_double[p1][i0]);
                        }
                        if( bperr )
                        {
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                        tx = (0.001+0.999*ae_randomreal(_state))*(tbl1.ptr.pp_double[p1][1]-tbl1.ptr.pp_double[p1][0]);
                        ty = (0.001+0.999*ae_randomreal(_state))*(tbl1.ptr.pp_double[p1][3]-tbl1.ptr.pp_double[p1][2]);
                        tz = (0.001+0.999*ae_randomreal(_state))*(tbl1.ptr.pp_double[p1][5]-tbl1.ptr.pp_double[p1][4]);
                        
                        /*
                         * Interpolation properties for:
                         *  *scalar function;
                         */
                        v1 = (double)(0);
                        for(ci=0; ci<=1; ci++)
                        {
                            for(cj=0; cj<=1; cj++)
                            {
                                for(ck=0; ck<=1; ck++)
                                {
                                    v1 = v1+tbl1.ptr.pp_double[p1][6+2*(2*ck+cj)+ci]*ae_pow(tx, (double)(ci), _state)*ae_pow(ty, (double)(cj), _state)*ae_pow(tz, (double)(ck), _state);
                                }
                            }
                        }
                        v2 = spline3dcalc(&c, tbl1.ptr.pp_double[p1][0]+tx, tbl1.ptr.pp_double[p1][2]+ty, tbl1.ptr.pp_double[p1][4]+tz, _state);
                        err = ae_maxreal(err, ae_fabs(v1-v2, _state), _state);
                        
                        /*
                         *  *component of vector function.
                         */
                        v1 = (double)(0);
                        for(ci=0; ci<=1; ci++)
                        {
                            for(cj=0; cj<=1; cj++)
                            {
                                for(ck=0; ck<=1; ck++)
                                {
                                    v1 = v1+tbl0.ptr.pp_double[p0][6+2*(2*ck+cj)+ci]*ae_pow(tx, (double)(ci), _state)*ae_pow(ty, (double)(cj), _state)*ae_pow(tz, (double)(ck), _state);
                                }
                            }
                        }
                        v2 = spline3dcalc(&c, tbl0.ptr.pp_double[p0][0]+tx, tbl0.ptr.pp_double[p0][2]+ty, tbl0.ptr.pp_double[p0][4]+tz, _state);
                        err = ae_maxreal(err, ae_fabs(v1-v2, _state), _state);
                    }
                }
            }
        }
    }
    result = ae_fp_greater(err,1.0E+5*ae_machineepsilon);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
LinTrans test
*************************************************************************/
static ae_bool testspline3dunit_testlintrans(ae_state *_state)
{
    ae_frame _frame_block;
    spline3dinterpolant c;
    spline3dinterpolant c2;
    ae_int_t m;
    ae_int_t n;
    ae_int_t l;
    ae_int_t d;
    ae_vector x;
    ae_vector y;
    ae_vector z;
    ae_vector f;
    double a1;
    double a2;
    double a3;
    double b1;
    double b2;
    double b3;
    double tx;
    double ty;
    double tz;
    double vx;
    double vy;
    double vz;
    ae_vector v1;
    ae_vector v2;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t xjob;
    ae_int_t yjob;
    ae_int_t zjob;
    double err;
    ae_int_t i;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _spline3dinterpolant_init(&c, _state);
    _spline3dinterpolant_init(&c2, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&z, 0, DT_REAL, _state);
    ae_vector_init(&f, 0, DT_REAL, _state);
    ae_vector_init(&v1, 0, DT_REAL, _state);
    ae_vector_init(&v2, 0, DT_REAL, _state);

    err = (double)(0);
    passcount = 15;
    for(pass=1; pass<=passcount; pass++)
    {
        testspline3dunit_buildrndgrid(ae_true, ae_false, &n, &m, &l, &d, &x, &y, &z, &f, _state);
        spline3dbuildtrilinearv(&x, n, &y, m, &z, l, &f, d, &c, _state);
        for(xjob=0; xjob<=1; xjob++)
        {
            for(yjob=0; yjob<=1; yjob++)
            {
                for(zjob=0; zjob<=1; zjob++)
                {
                    
                    /*
                     * Prepare
                     */
                    do
                    {
                        a1 = 2.0*ae_randomreal(_state)-1.0;
                    }
                    while(ae_fp_eq(a1,(double)(0)));
                    a1 = a1*xjob;
                    b1 = x.ptr.p_double[0]+ae_randomreal(_state)*(x.ptr.p_double[n-1]-x.ptr.p_double[0]+2.0)-1.0;
                    do
                    {
                        a2 = 2.0*ae_randomreal(_state)-1.0;
                    }
                    while(ae_fp_eq(a2,(double)(0)));
                    a2 = a2*yjob;
                    b2 = y.ptr.p_double[0]+ae_randomreal(_state)*(y.ptr.p_double[m-1]-y.ptr.p_double[0]+2.0)-1.0;
                    do
                    {
                        a3 = 2.0*ae_randomreal(_state)-1.0;
                    }
                    while(ae_fp_eq(a3,(double)(0)));
                    a3 = a3*zjob;
                    b3 = z.ptr.p_double[0]+ae_randomreal(_state)*(z.ptr.p_double[l-1]-z.ptr.p_double[0]+2.0)-1.0;
                    
                    /*
                     * Test XYZ
                     */
                    spline3dcopy(&c, &c2, _state);
                    spline3dlintransxyz(&c2, a1, b1, a2, b2, a3, b3, _state);
                    tx = x.ptr.p_double[0]+ae_randomreal(_state)*(x.ptr.p_double[n-1]-x.ptr.p_double[0]);
                    ty = y.ptr.p_double[0]+ae_randomreal(_state)*(y.ptr.p_double[m-1]-y.ptr.p_double[0]);
                    tz = z.ptr.p_double[0]+ae_randomreal(_state)*(z.ptr.p_double[l-1]-z.ptr.p_double[0]);
                    if( xjob==0 )
                    {
                        tx = b1;
                        vx = x.ptr.p_double[0]+ae_randomreal(_state)*(x.ptr.p_double[n-1]-x.ptr.p_double[0]);
                    }
                    else
                    {
                        vx = (tx-b1)/a1;
                    }
                    if( yjob==0 )
                    {
                        ty = b2;
                        vy = y.ptr.p_double[0]+ae_randomreal(_state)*(y.ptr.p_double[m-1]-y.ptr.p_double[0]);
                    }
                    else
                    {
                        vy = (ty-b2)/a2;
                    }
                    if( zjob==0 )
                    {
                        tz = b3;
                        vz = z.ptr.p_double[0]+ae_randomreal(_state)*(z.ptr.p_double[l-1]-z.ptr.p_double[0]);
                    }
                    else
                    {
                        vz = (tz-b3)/a3;
                    }
                    spline3dcalcv(&c, tx, ty, tz, &v1, _state);
                    spline3dcalcv(&c2, vx, vy, vz, &v2, _state);
                    for(i=0; i<=d-1; i++)
                    {
                        err = ae_maxreal(err, ae_fabs(v1.ptr.p_double[i]-v2.ptr.p_double[i], _state), _state);
                    }
                    if( ae_fp_greater(err,1.0E+4*ae_machineepsilon) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                    
                    /*
                     * Test F
                     */
                    spline3dcopy(&c, &c2, _state);
                    spline3dlintransf(&c2, a1, b1, _state);
                    tx = x.ptr.p_double[0]+ae_randomreal(_state)*(x.ptr.p_double[n-1]-x.ptr.p_double[0]);
                    ty = y.ptr.p_double[0]+ae_randomreal(_state)*(y.ptr.p_double[m-1]-y.ptr.p_double[0]);
                    tz = z.ptr.p_double[0]+ae_randomreal(_state)*(z.ptr.p_double[l-1]-z.ptr.p_double[0]);
                    spline3dcalcv(&c, tx, ty, tz, &v1, _state);
                    spline3dcalcv(&c2, tx, ty, tz, &v2, _state);
                    for(i=0; i<=d-1; i++)
                    {
                        err = ae_maxreal(err, ae_fabs(a1*v1.ptr.p_double[i]+b1-v2.ptr.p_double[i], _state), _state);
                    }
                }
            }
        }
    }
    result = ae_fp_greater(err,1.0E+4*ae_machineepsilon);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Resample test
*************************************************************************/
static ae_bool testspline3dunit_testtrilinearresample(ae_state *_state)
{
    ae_frame _frame_block;
    spline3dinterpolant c;
    ae_int_t n;
    ae_int_t m;
    ae_int_t l;
    ae_int_t n2;
    ae_int_t m2;
    ae_int_t l2;
    ae_vector x;
    ae_vector y;
    ae_vector z;
    ae_vector f;
    ae_vector fr;
    double v1;
    double v2;
    double err;
    double mf;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _spline3dinterpolant_init(&c, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&z, 0, DT_REAL, _state);
    ae_vector_init(&f, 0, DT_REAL, _state);
    ae_vector_init(&fr, 0, DT_REAL, _state);

    result = ae_false;
    passcount = 20;
    for(pass=1; pass<=passcount; pass++)
    {
        n = ae_randominteger(4, _state)+2;
        m = ae_randominteger(4, _state)+2;
        l = ae_randominteger(4, _state)+2;
        n2 = ae_randominteger(4, _state)+2;
        m2 = ae_randominteger(4, _state)+2;
        l2 = ae_randominteger(4, _state)+2;
        rvectorsetlengthatleast(&x, n, _state);
        rvectorsetlengthatleast(&y, m, _state);
        rvectorsetlengthatleast(&z, l, _state);
        rvectorsetlengthatleast(&f, n*m*l, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = (double)i/(double)(n-1);
        }
        for(i=0; i<=m-1; i++)
        {
            y.ptr.p_double[i] = (double)i/(double)(m-1);
        }
        for(i=0; i<=l-1; i++)
        {
            z.ptr.p_double[i] = (double)i/(double)(l-1);
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                for(k=0; k<=l-1; k++)
                {
                    f.ptr.p_double[n*(m*k+j)+i] = 2*ae_randomreal(_state)-1;
                }
            }
        }
        spline3dresampletrilinear(&f, l, m, n, l2, m2, n2, &fr, _state);
        spline3dbuildtrilinearv(&x, n, &y, m, &z, l, &f, 1, &c, _state);
        err = (double)(0);
        mf = (double)(0);
        for(i=0; i<=n2-1; i++)
        {
            for(j=0; j<=m2-1; j++)
            {
                for(k=0; k<=l2-1; k++)
                {
                    v1 = spline3dcalc(&c, (double)i/(double)(n2-1), (double)j/(double)(m2-1), (double)k/(double)(l2-1), _state);
                    v2 = fr.ptr.p_double[n2*(m2*k+j)+i];
                    err = ae_maxreal(err, ae_fabs(v1-v2, _state), _state);
                    mf = ae_maxreal(mf, ae_fabs(v1, _state), _state);
                }
            }
        }
        result = result||ae_fp_greater(err/mf,1.0E+4*ae_machineepsilon);
        if( result )
        {
            ae_frame_leave(_state);
            return result;
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
The function does build random function on random grid with random number
of points:
* N, M, K   -   random from 2 to 5
* D         -   1 in case IsVect=False, 1..3 in case IsVect=True
* X, Y, Z   -   each variable spans from MinV to MaxV, with MinV is random
                number from [-1.5,0.5] and MaxV is random number from
                [0.5,1.5]. All nodes are well separated. All nodes are
                randomly reordered in case Reorder=False. When Reorder=True,
                nodes are returned in ascending order.
* F         -   random values from [-1,+1]
*************************************************************************/
static void testspline3dunit_buildrndgrid(ae_bool isvect,
     ae_bool reorder,
     ae_int_t* n,
     ae_int_t* m,
     ae_int_t* l,
     ae_int_t* d,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* z,
     /* Real    */ ae_vector* f,
     ae_state *_state)
{
    double st;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t di;
    double v;
    double mx;
    double maxv;
    double minv;

    *n = 0;
    *m = 0;
    *l = 0;
    *d = 0;
    ae_vector_clear(x);
    ae_vector_clear(y);
    ae_vector_clear(z);
    ae_vector_clear(f);

    st = 0.3;
    *m = ae_randominteger(4, _state)+2;
    *n = ae_randominteger(4, _state)+2;
    *l = ae_randominteger(4, _state)+2;
    if( isvect )
    {
        *d = ae_randominteger(3, _state)+1;
    }
    else
    {
        *d = 1;
    }
    rvectorsetlengthatleast(x, *n, _state);
    rvectorsetlengthatleast(y, *m, _state);
    rvectorsetlengthatleast(z, *l, _state);
    rvectorsetlengthatleast(f, *n*(*m)*(*l)*(*d), _state);
    
    /*
     * Fill X
     */
    x->ptr.p_double[0] = (double)(0);
    for(i=1; i<=*n-1; i++)
    {
        x->ptr.p_double[i] = x->ptr.p_double[i-1]+st+ae_randomreal(_state);
    }
    minv = -0.5-ae_randomreal(_state);
    maxv = 0.5+ae_randomreal(_state);
    mx = x->ptr.p_double[*n-1];
    for(i=0; i<=*n-1; i++)
    {
        x->ptr.p_double[i] = x->ptr.p_double[i]/mx*(maxv-minv)+minv;
    }
    if( reorder )
    {
        for(i=0; i<=*n-1; i++)
        {
            k = ae_randominteger(*n, _state);
            v = x->ptr.p_double[i];
            x->ptr.p_double[i] = x->ptr.p_double[k];
            x->ptr.p_double[k] = v;
        }
    }
    
    /*
     * Fill Y
     */
    y->ptr.p_double[0] = (double)(0);
    for(i=1; i<=*m-1; i++)
    {
        y->ptr.p_double[i] = y->ptr.p_double[i-1]+st+ae_randomreal(_state);
    }
    minv = -0.5-ae_randomreal(_state);
    maxv = 0.5+ae_randomreal(_state);
    mx = y->ptr.p_double[*m-1];
    for(i=0; i<=*m-1; i++)
    {
        y->ptr.p_double[i] = y->ptr.p_double[i]/mx*(maxv-minv)+minv;
    }
    if( reorder )
    {
        for(i=0; i<=*m-1; i++)
        {
            k = ae_randominteger(*m, _state);
            v = y->ptr.p_double[i];
            y->ptr.p_double[i] = y->ptr.p_double[k];
            y->ptr.p_double[k] = v;
        }
    }
    
    /*
     * Fill Z
     */
    z->ptr.p_double[0] = (double)(0);
    for(i=1; i<=*l-1; i++)
    {
        z->ptr.p_double[i] = z->ptr.p_double[i-1]+st+ae_randomreal(_state);
    }
    minv = -0.5-ae_randomreal(_state);
    maxv = 0.5+ae_randomreal(_state);
    mx = z->ptr.p_double[*l-1];
    for(i=0; i<=*l-1; i++)
    {
        z->ptr.p_double[i] = z->ptr.p_double[i]/mx*(maxv-minv)+minv;
    }
    if( reorder )
    {
        for(i=0; i<=*l-1; i++)
        {
            k = ae_randominteger(*l, _state);
            v = z->ptr.p_double[i];
            z->ptr.p_double[i] = z->ptr.p_double[k];
            z->ptr.p_double[k] = v;
        }
    }
    
    /*
     * Fill F
     */
    for(i=0; i<=*n-1; i++)
    {
        for(j=0; j<=*m-1; j++)
        {
            for(k=0; k<=*l-1; k++)
            {
                for(di=0; di<=*d-1; di++)
                {
                    f->ptr.p_double[*d*(*n*(*m*k+j)+i)+di] = 2*ae_randomreal(_state)-1;
                }
            }
        }
    }
}


/*$ End $*/
