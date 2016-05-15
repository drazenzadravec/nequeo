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
#include "testidwintunit.h"


/*$ Declarations $*/
static void testidwintunit_testxy(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t d,
     ae_int_t nq,
     ae_int_t nw,
     ae_bool* idwerrors,
     ae_state *_state);
static void testidwintunit_testdegree(ae_int_t n,
     ae_int_t nx,
     ae_int_t d,
     ae_int_t dtask,
     ae_bool* idwerrors,
     ae_state *_state);
static void testidwintunit_testnoisy(ae_bool* idwerrors, ae_state *_state);


/*$ Body $*/


/*************************************************************************
Testing IDW interpolation
*************************************************************************/
ae_bool testidwint(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xy;
    ae_int_t i;
    ae_int_t j;
    double vx;
    double vy;
    double vz;
    ae_int_t d;
    ae_int_t dtask;
    ae_int_t nx;
    ae_int_t nq;
    ae_int_t nw;
    ae_int_t smalln;
    ae_int_t largen;
    ae_bool waserrors;
    ae_bool idwerrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);

    idwerrors = ae_false;
    smalln = 256;
    largen = 1024;
    nq = 10;
    nw = 18;
    
    /*
     * Simple test:
     * * F = x^3 + sin(pi*y)*z^2 - (x+y)^2
     * * space is either R1=[-1,+1] (other dimensions are
     *   fixed at 0), R1^2 or R1^3.
     ** D = -1, 0, 1, 2
     */
    for(nx=1; nx<=2; nx++)
    {
        ae_matrix_set_length(&xy, largen, nx+1, _state);
        for(i=0; i<=largen-1; i++)
        {
            for(j=0; j<=nx-1; j++)
            {
                xy.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            if( nx>=1 )
            {
                vx = xy.ptr.pp_double[i][0];
            }
            else
            {
                vx = (double)(0);
            }
            if( nx>=2 )
            {
                vy = xy.ptr.pp_double[i][1];
            }
            else
            {
                vy = (double)(0);
            }
            if( nx>=3 )
            {
                vz = xy.ptr.pp_double[i][2];
            }
            else
            {
                vz = (double)(0);
            }
            xy.ptr.pp_double[i][nx] = vx*vx*vx+ae_sin(ae_pi*vy, _state)*ae_sqr(vz, _state)-ae_sqr(vx+vy, _state);
        }
        for(d=-1; d<=2; d++)
        {
            testidwintunit_testxy(&xy, largen, nx, d, nq, nw, &idwerrors, _state);
        }
    }
    
    /*
     * Another simple test:
     * * five points in 2D - (0,0), (0,1), (1,0), (-1,0) (0,-1)
     * * F is random
     * * D = -1, 0, 1, 2
     */
    nx = 2;
    ae_matrix_set_length(&xy, 5, nx+1, _state);
    xy.ptr.pp_double[0][0] = (double)(0);
    xy.ptr.pp_double[0][1] = (double)(0);
    xy.ptr.pp_double[0][2] = 2*ae_randomreal(_state)-1;
    xy.ptr.pp_double[1][0] = (double)(1);
    xy.ptr.pp_double[1][1] = (double)(0);
    xy.ptr.pp_double[1][2] = 2*ae_randomreal(_state)-1;
    xy.ptr.pp_double[2][0] = (double)(0);
    xy.ptr.pp_double[2][1] = (double)(1);
    xy.ptr.pp_double[2][2] = 2*ae_randomreal(_state)-1;
    xy.ptr.pp_double[3][0] = (double)(-1);
    xy.ptr.pp_double[3][1] = (double)(0);
    xy.ptr.pp_double[3][2] = 2*ae_randomreal(_state)-1;
    xy.ptr.pp_double[4][0] = (double)(0);
    xy.ptr.pp_double[4][1] = (double)(-1);
    xy.ptr.pp_double[4][2] = 2*ae_randomreal(_state)-1;
    for(d=-1; d<=2; d++)
    {
        testidwintunit_testxy(&xy, 5, nx, d, nq, nw, &idwerrors, _state);
    }
    
    /*
     * Degree test.
     *
     * F is either:
     * * constant (DTask=0)
     * * linear (DTask=1)
     * * quadratic (DTask=2)
     *
     * Nodal functions are either
     * * constant (D=0)
     * * linear (D=1)
     * * quadratic (D=2)
     *
     * When DTask<=D, we can interpolate without errors.
     * When DTask>D, we MUST have errors.
     */
    for(nx=1; nx<=3; nx++)
    {
        for(d=0; d<=2; d++)
        {
            for(dtask=0; dtask<=2; dtask++)
            {
                testidwintunit_testdegree(smalln, nx, d, dtask, &idwerrors, _state);
            }
        }
    }
    
    /*
     * Noisy test
     */
    testidwintunit_testnoisy(&idwerrors, _state);
    
    /*
     * report
     */
    waserrors = idwerrors;
    if( !silent )
    {
        printf("TESTING INVERSE DISTANCE WEIGHTING\n");
        printf("* IDW:                                   ");
        if( !idwerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
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
ae_bool _pexec_testidwint(ae_bool silent, ae_state *_state)
{
    return testidwint(silent, _state);
}


/*************************************************************************
Testing IDW:
* generate model using N/NX/D/NQ/NW
* test basic properties
*************************************************************************/
static void testidwintunit_testxy(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t d,
     ae_int_t nq,
     ae_int_t nw,
     ae_bool* idwerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    double threshold;
    double lipschitzstep;
    ae_int_t i;
    ae_int_t i1;
    ae_int_t i2;
    double v;
    double v1;
    double v2;
    double t;
    double l1;
    double l2;
    idwinterpolant z1;
    ae_vector x;

    ae_frame_make(_state, &_frame_block);
    _idwinterpolant_init(&z1, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);

    threshold = 1000*ae_machineepsilon;
    lipschitzstep = 0.001;
    ae_vector_set_length(&x, nx, _state);
    
    /*
     * build
     */
    idwbuildmodifiedshepard(xy, n, nx, d, nq, nw, &z1, _state);
    
    /*
     * first, test interpolation properties at nodes
     */
    for(i=0; i<=n-1; i++)
    {
        ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
        *idwerrors = *idwerrors||ae_fp_neq(idwcalc(&z1, &x, _state),xy->ptr.pp_double[i][nx]);
    }
    
    /*
     * test Lipschitz continuity
     */
    i1 = ae_randominteger(n, _state);
    do
    {
        i2 = ae_randominteger(n, _state);
    }
    while(i2==i1);
    l1 = (double)(0);
    t = (double)(0);
    while(ae_fp_less(t,(double)(1)))
    {
        v = 1-t;
        ae_v_moved(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i1][0], 1, ae_v_len(0,nx-1), v);
        v = t;
        ae_v_addd(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i2][0], 1, ae_v_len(0,nx-1), v);
        v1 = idwcalc(&z1, &x, _state);
        v = 1-(t+lipschitzstep);
        ae_v_moved(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i1][0], 1, ae_v_len(0,nx-1), v);
        v = t+lipschitzstep;
        ae_v_addd(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i2][0], 1, ae_v_len(0,nx-1), v);
        v2 = idwcalc(&z1, &x, _state);
        l1 = ae_maxreal(l1, ae_fabs(v2-v1, _state)/lipschitzstep, _state);
        t = t+lipschitzstep;
    }
    l2 = (double)(0);
    t = (double)(0);
    while(ae_fp_less(t,(double)(1)))
    {
        v = 1-t;
        ae_v_moved(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i1][0], 1, ae_v_len(0,nx-1), v);
        v = t;
        ae_v_addd(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i2][0], 1, ae_v_len(0,nx-1), v);
        v1 = idwcalc(&z1, &x, _state);
        v = 1-(t+lipschitzstep/3);
        ae_v_moved(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i1][0], 1, ae_v_len(0,nx-1), v);
        v = t+lipschitzstep/3;
        ae_v_addd(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i2][0], 1, ae_v_len(0,nx-1), v);
        v2 = idwcalc(&z1, &x, _state);
        l2 = ae_maxreal(l2, ae_fabs(v2-v1, _state)/(lipschitzstep/3), _state);
        t = t+lipschitzstep/3;
    }
    *idwerrors = *idwerrors||ae_fp_greater(l2,2.0*l1);
    ae_frame_leave(_state);
}


/*************************************************************************
Testing degree properties

F is either:
* constant (DTask=0)
* linear (DTask=1)
* quadratic (DTask=2)

Nodal functions are either
* constant (D=0)
* linear (D=1)
* quadratic (D=2)

When DTask<=D, we can interpolate without errors.
When DTask>D, we MUST have errors.
*************************************************************************/
static void testidwintunit_testdegree(ae_int_t n,
     ae_int_t nx,
     ae_int_t d,
     ae_int_t dtask,
     ae_bool* idwerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    double threshold;
    ae_int_t nq;
    ae_int_t nw;
    ae_int_t i;
    ae_int_t j;
    double v;
    double c0;
    ae_vector c1;
    ae_matrix c2;
    ae_vector x;
    ae_matrix xy;
    idwinterpolant z1;
    double v1;
    double v2;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&c1, 0, DT_REAL, _state);
    ae_matrix_init(&c2, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _idwinterpolant_init(&z1, _state);

    threshold = 1.0E6*ae_machineepsilon;
    nq = 2*(nx*nx+nx+1);
    nw = 10;
    ae_assert(nq<=n, "TestDegree: internal error", _state);
    
    /*
     * prepare model
     */
    c0 = 2*ae_randomreal(_state)-1;
    ae_vector_set_length(&c1, nx, _state);
    for(i=0; i<=nx-1; i++)
    {
        c1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
    }
    ae_matrix_set_length(&c2, nx, nx, _state);
    for(i=0; i<=nx-1; i++)
    {
        for(j=i+1; j<=nx-1; j++)
        {
            c2.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            c2.ptr.pp_double[j][i] = c2.ptr.pp_double[i][j];
        }
        do
        {
            c2.ptr.pp_double[i][i] = 2*ae_randomreal(_state)-1;
        }
        while(ae_fp_less_eq(ae_fabs(c2.ptr.pp_double[i][i], _state),0.3));
    }
    
    /*
     * prepare points
     */
    ae_matrix_set_length(&xy, n, nx+1, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=nx-1; j++)
        {
            xy.ptr.pp_double[i][j] = 4*ae_randomreal(_state)-2;
        }
        xy.ptr.pp_double[i][nx] = c0;
        if( dtask>=1 )
        {
            v = ae_v_dotproduct(&c1.ptr.p_double[0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
            xy.ptr.pp_double[i][nx] = xy.ptr.pp_double[i][nx]+v;
        }
        if( dtask==2 )
        {
            for(j=0; j<=nx-1; j++)
            {
                v = ae_v_dotproduct(&c2.ptr.pp_double[j][0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
                xy.ptr.pp_double[i][nx] = xy.ptr.pp_double[i][nx]+xy.ptr.pp_double[i][j]*v;
            }
        }
    }
    
    /*
     * build interpolant, calculate value at random point
     */
    idwbuildmodifiedshepard(&xy, n, nx, d, nq, nw, &z1, _state);
    ae_vector_set_length(&x, nx, _state);
    for(i=0; i<=nx-1; i++)
    {
        x.ptr.p_double[i] = 4*ae_randomreal(_state)-2;
    }
    v1 = idwcalc(&z1, &x, _state);
    
    /*
     * calculate model value at the same point
     */
    v2 = c0;
    if( dtask>=1 )
    {
        v = ae_v_dotproduct(&c1.ptr.p_double[0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,nx-1));
        v2 = v2+v;
    }
    if( dtask==2 )
    {
        for(j=0; j<=nx-1; j++)
        {
            v = ae_v_dotproduct(&c2.ptr.pp_double[j][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,nx-1));
            v2 = v2+x.ptr.p_double[j]*v;
        }
    }
    
    /*
     * Compare
     */
    if( dtask<=d )
    {
        *idwerrors = *idwerrors||ae_fp_greater(ae_fabs(v2-v1, _state),threshold);
    }
    else
    {
        *idwerrors = *idwerrors||ae_fp_less(ae_fabs(v2-v1, _state),threshold);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Noisy test:
 * F = x^2 + y^2 + z^2 + noise on [-1,+1]^3
 * space is either R1=[-1,+1] (other dimensions are
   fixed at 0), R1^2 or R1^3.
 * D = 1, 2
 * 4096 points is used for function generation,
   4096 points - for testing
 * RMS error of "noisy" model on test set must be
   lower than RMS error of interpolation model.
*************************************************************************/
static void testidwintunit_testnoisy(ae_bool* idwerrors, ae_state *_state)
{
    ae_frame _frame_block;
    double noiselevel;
    ae_int_t nq;
    ae_int_t nw;
    ae_int_t d;
    ae_int_t nx;
    ae_int_t ntrn;
    ae_int_t ntst;
    ae_int_t i;
    ae_int_t j;
    double v;
    double t;
    double v1;
    double v2;
    double ve;
    ae_matrix xy;
    ae_vector x;
    idwinterpolant z1;
    idwinterpolant z2;
    double rms1;
    double rms2;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    _idwinterpolant_init(&z1, _state);
    _idwinterpolant_init(&z2, _state);

    nq = 20;
    nw = 40;
    noiselevel = 0.2;
    ntrn = 256;
    ntst = 1024;
    for(d=1; d<=2; d++)
    {
        for(nx=1; nx<=2; nx++)
        {
            
            /*
             * prepare dataset
             */
            ae_matrix_set_length(&xy, ntrn, nx+1, _state);
            for(i=0; i<=ntrn-1; i++)
            {
                v = noiselevel*(2*ae_randomreal(_state)-1);
                for(j=0; j<=nx-1; j++)
                {
                    t = 2*ae_randomreal(_state)-1;
                    v = v+ae_sqr(t, _state);
                    xy.ptr.pp_double[i][j] = t;
                }
                xy.ptr.pp_double[i][nx] = v;
            }
            
            /*
             * build interpolants
             */
            idwbuildmodifiedshepard(&xy, ntrn, nx, d, nq, nw, &z1, _state);
            idwbuildnoisy(&xy, ntrn, nx, d, nq, nw, &z2, _state);
            
            /*
             * calculate RMS errors
             */
            ae_vector_set_length(&x, nx, _state);
            rms1 = (double)(0);
            rms2 = (double)(0);
            for(i=0; i<=ntst-1; i++)
            {
                ve = (double)(0);
                for(j=0; j<=nx-1; j++)
                {
                    t = 2*ae_randomreal(_state)-1;
                    x.ptr.p_double[j] = t;
                    ve = ve+ae_sqr(t, _state);
                }
                v1 = idwcalc(&z1, &x, _state);
                v2 = idwcalc(&z2, &x, _state);
                rms1 = rms1+ae_sqr(v1-ve, _state);
                rms2 = rms2+ae_sqr(v2-ve, _state);
            }
            *idwerrors = *idwerrors||ae_fp_greater(rms2,rms1);
        }
    }
    ae_frame_leave(_state);
}


/*$ End $*/
