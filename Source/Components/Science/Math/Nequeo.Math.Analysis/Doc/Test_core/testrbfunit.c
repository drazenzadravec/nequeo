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
#include "testrbfunit.h"


/*$ Declarations $*/
static ae_int_t testrbfunit_mxnx = 3;
static double testrbfunit_eps = 1.0E-6;
static double testrbfunit_tol = 1.0E-10;
static ae_int_t testrbfunit_mxits = 0;
static double testrbfunit_heps = 1.0E-12;
static ae_bool testrbfunit_specialtest(ae_state *_state);
static ae_bool testrbfunit_basicrbftest(ae_state *_state);
static ae_bool testrbfunit_irregularrbftest(ae_state *_state);
static ae_bool testrbfunit_linearitymodelrbftest(ae_state *_state);
static ae_bool testrbfunit_serializationtest(ae_state *_state);
static ae_bool testrbfunit_searcherr(/* Real    */ ae_matrix* y0,
     /* Real    */ ae_matrix* y1,
     ae_int_t n,
     ae_int_t ny,
     ae_int_t errtype,
     /* Real    */ ae_vector* b1,
     /* Real    */ ae_vector* delta,
     ae_state *_state);
static ae_bool testrbfunit_basicmultilayerrbftest(ae_state *_state);


/*$ Body $*/


ae_bool testrbf(ae_bool silent, ae_state *_state)
{
    ae_bool specialerrors;
    ae_bool basicrbferrors;
    ae_bool irregularrbferrors;
    ae_bool linearitymodelrbferr;
    ae_bool sqrdegmatrixrbferr;
    ae_bool sererrors;
    ae_bool multilayerrbf1derrors;
    ae_bool multilayerrbferrors;
    ae_bool waserrors;
    ae_bool result;


    specialerrors = testrbfunit_specialtest(_state);
    basicrbferrors = testrbfunit_basicrbftest(_state);
    irregularrbferrors = testrbfunit_irregularrbftest(_state);
    linearitymodelrbferr = testrbfunit_linearitymodelrbftest(_state);
    sqrdegmatrixrbferr = sqrdegmatrixrbftest(ae_true, _state);
    multilayerrbf1derrors = ae_false;
    multilayerrbferrors = testrbfunit_basicmultilayerrbftest(_state);
    sererrors = testrbfunit_serializationtest(_state);
    
    /*
     * report
     */
    waserrors = ((((((specialerrors||basicrbferrors)||irregularrbferrors)||linearitymodelrbferr)||sqrdegmatrixrbferr)||sererrors)||multilayerrbf1derrors)||multilayerrbferrors;
    if( !silent )
    {
        printf("TESTING RBF\n");
        printf("Special cases:                                     ");
        if( specialerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("BasicRBFTest:                                      ");
        if( basicrbferrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("IrregularRBFTest:                                  ");
        if( irregularrbferrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("LinearityModelRBFTest:                             ");
        if( linearitymodelrbferr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("SqrDegMatrixRBFTest:                               ");
        if( sqrdegmatrixrbferr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Serialization test:                                ");
        if( sererrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("MultiLayerRBFErrors in 1D test:                    ");
        if( multilayerrbf1derrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("MultiLayerRBFErrors in 2-3D test:                  ");
        if( multilayerrbferrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        
        /*
         * was errors?
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
    result = !waserrors;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testrbf(ae_bool silent, ae_state *_state)
{
    return testrbf(silent, _state);
}


/*************************************************************************
The test  has  to  check, that  algorithm can solve problems of matrix are
degenerate.
    * used model with linear term;
    * points locate in a subspace of dimension less than an original space.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool sqrdegmatrixrbftest(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    rbfmodel s;
    rbfreport rep;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t k0;
    ae_int_t k1;
    ae_int_t np;
    double sx;
    double sy;
    double zx;
    double px;
    double zy;
    double py;
    double q;
    double z;
    ae_vector point;
    ae_matrix a;
    ae_vector d0;
    ae_vector d1;
    ae_int_t gen;
    ae_vector pvd0;
    ae_vector pvd1;
    double pvdnorm;
    double vnorm;
    double dd0;
    double dd1;
    ae_matrix gp;
    ae_vector x;
    ae_vector y;
    ae_int_t unx;
    ae_int_t uny;
    ae_matrix xwr;
    ae_matrix v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _rbfmodel_init(&s, _state);
    _rbfreport_init(&rep, _state);
    ae_vector_init(&point, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&d0, 0, DT_REAL, _state);
    ae_vector_init(&d1, 0, DT_REAL, _state);
    ae_vector_init(&pvd0, 0, DT_REAL, _state);
    ae_vector_init(&pvd1, 0, DT_REAL, _state);
    ae_matrix_init(&gp, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_matrix_init(&xwr, 0, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);

    zx = (double)(10);
    px = (double)(15);
    zy = (double)(10);
    py = (double)(15);
    ny = 1;
    for(nx=2; nx<=3; nx++)
    {
        
        /*
         * prepare test problem
         */
        sx = ae_pow(zx, px*(ae_randominteger(3, _state)-1), _state);
        sy = ae_pow(zy, py*(ae_randominteger(3, _state)-1), _state);
        ae_vector_set_length(&x, nx, _state);
        ae_vector_set_length(&y, ny, _state);
        ae_vector_set_length(&point, nx, _state);
        rbfcreate(nx, ny, &s, _state);
        rbfsetcond(&s, testrbfunit_heps, testrbfunit_heps, testrbfunit_mxits, _state);
        q = 0.25+ae_randomreal(_state);
        z = 4.5+ae_randomreal(_state);
        rbfsetalgoqnn(&s, q, z, _state);
        
        /*
         * start points for grid
         */
        for(i=0; i<=nx-1; i++)
        {
            point.ptr.p_double[i] = sx*(2*ae_randomreal(_state)-1);
        }
        if( nx==2 )
        {
            for(k0=2; k0<=4; k0++)
            {
                rmatrixrndorthogonal(nx, &a, _state);
                ae_vector_set_length(&d0, nx, _state);
                ae_v_move(&d0.ptr.p_double[0], 1, &a.ptr.pp_double[0][0], a.stride, ae_v_len(0,nx-1));
                np = k0;
                ae_matrix_set_length(&gp, np, nx+ny, _state);
                
                /*
                 * create grid
                 */
                for(i=0; i<=k0-1; i++)
                {
                    gp.ptr.pp_double[i][0] = point.ptr.p_double[0]+sx*i*d0.ptr.p_double[0];
                    gp.ptr.pp_double[i][1] = point.ptr.p_double[1]+sx*i*d0.ptr.p_double[1];
                    for(k=0; k<=ny-1; k++)
                    {
                        gp.ptr.pp_double[i][nx+k] = sy*(2*ae_randomreal(_state)-1);
                    }
                }
                rbfsetpoints(&s, &gp, np, _state);
                rbfbuildmodel(&s, &rep, _state);
                for(i=0; i<=np-1; i++)
                {
                    x.ptr.p_double[0] = gp.ptr.pp_double[i][0];
                    x.ptr.p_double[1] = gp.ptr.pp_double[i][1];
                    rbfcalc(&s, &x, &y, _state);
                    for(j=0; j<=ny-1; j++)
                    {
                        if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),sy*testrbfunit_eps) )
                        {
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                    }
                }
            }
        }
        if( nx==3 )
        {
            for(k0=2; k0<=4; k0++)
            {
                for(k1=2; k1<=4; k1++)
                {
                    for(gen=1; gen<=2; gen++)
                    {
                        rmatrixrndorthogonal(nx, &a, _state);
                        ae_vector_set_length(&d0, nx, _state);
                        ae_v_move(&d0.ptr.p_double[0], 1, &a.ptr.pp_double[0][0], a.stride, ae_v_len(0,nx-1));
                        
                        /*
                         * create grid
                         */
                        np = -1;
                        if( gen==1 )
                        {
                            np = k0;
                            ae_matrix_set_length(&gp, np, nx+ny, _state);
                            for(i=0; i<=k0-1; i++)
                            {
                                gp.ptr.pp_double[i][0] = point.ptr.p_double[0]+sx*i*d0.ptr.p_double[0];
                                gp.ptr.pp_double[i][1] = point.ptr.p_double[1]+sx*i*d0.ptr.p_double[1];
                                gp.ptr.pp_double[i][2] = point.ptr.p_double[2]+sx*i*d0.ptr.p_double[2];
                                for(k=0; k<=ny-1; k++)
                                {
                                    gp.ptr.pp_double[i][nx+k] = sy*(2*ae_randomreal(_state)-1);
                                }
                            }
                        }
                        if( gen==2 )
                        {
                            ae_vector_set_length(&d1, nx, _state);
                            ae_v_move(&d1.ptr.p_double[0], 1, &a.ptr.pp_double[0][1], a.stride, ae_v_len(0,nx-1));
                            np = k0*k1;
                            ae_matrix_set_length(&gp, np, nx+ny, _state);
                            for(i=0; i<=k0-1; i++)
                            {
                                for(j=0; j<=k1-1; j++)
                                {
                                    gp.ptr.pp_double[i*k1+j][0] = sx*i*d0.ptr.p_double[0]+sx*j*d1.ptr.p_double[0];
                                    gp.ptr.pp_double[i*k1+j][1] = sx*i*d0.ptr.p_double[1]+sx*j*d1.ptr.p_double[1];
                                    gp.ptr.pp_double[i*k1+j][2] = sx*i*d0.ptr.p_double[2]+sx*j*d1.ptr.p_double[2];
                                    for(k=0; k<=ny-1; k++)
                                    {
                                        gp.ptr.pp_double[i*k1+j][nx+k] = sy*(2*ae_randomreal(_state)-1);
                                    }
                                }
                            }
                        }
                        ae_assert(np>=0, "rbf test: integrity error", _state);
                        rbfsetpoints(&s, &gp, np, _state);
                        rbfbuildmodel(&s, &rep, _state);
                        for(i=0; i<=np-1; i++)
                        {
                            x.ptr.p_double[0] = gp.ptr.pp_double[i][0];
                            x.ptr.p_double[1] = gp.ptr.pp_double[i][1];
                            x.ptr.p_double[2] = gp.ptr.pp_double[i][2];
                            rbfcalc(&s, &x, &y, _state);
                            for(j=0; j<=ny-1; j++)
                            {
                                if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),sy*testrbfunit_eps) )
                                {
                                    result = ae_true;
                                    ae_frame_leave(_state);
                                    return result;
                                }
                            }
                        }
                        if( gen==2 )
                        {
                            rbfunpack(&s, &unx, &uny, &xwr, &np, &v, _state);
                            dd0 = (d0.ptr.p_double[0]*v.ptr.pp_double[0][0]+d0.ptr.p_double[1]*v.ptr.pp_double[0][1]+d0.ptr.p_double[2]*v.ptr.pp_double[0][2])/(d0.ptr.p_double[0]*d0.ptr.p_double[0]+d0.ptr.p_double[1]*d0.ptr.p_double[1]+d0.ptr.p_double[2]*d0.ptr.p_double[2]);
                            dd1 = (d1.ptr.p_double[0]*v.ptr.pp_double[0][0]+d1.ptr.p_double[1]*v.ptr.pp_double[0][1]+d1.ptr.p_double[2]*v.ptr.pp_double[0][2])/(d1.ptr.p_double[0]*d1.ptr.p_double[0]+d1.ptr.p_double[1]*d1.ptr.p_double[1]+d1.ptr.p_double[2]*d1.ptr.p_double[2]);
                            ae_vector_set_length(&pvd0, nx, _state);
                            ae_vector_set_length(&pvd1, nx, _state);
                            for(i=0; i<=nx-1; i++)
                            {
                                pvd0.ptr.p_double[i] = dd0*d0.ptr.p_double[i];
                                pvd1.ptr.p_double[i] = dd1*d1.ptr.p_double[i];
                            }
                            pvdnorm = ae_sqrt(ae_sqr(v.ptr.pp_double[0][0]-pvd0.ptr.p_double[0]-pvd1.ptr.p_double[0], _state)+ae_sqr(v.ptr.pp_double[0][1]-pvd0.ptr.p_double[1]-pvd1.ptr.p_double[1], _state)+ae_sqr(v.ptr.pp_double[0][2]-pvd0.ptr.p_double[2]-pvd1.ptr.p_double[2], _state), _state);
                            vnorm = ae_sqrt(ae_sqr(v.ptr.pp_double[0][0], _state)+ae_sqr(v.ptr.pp_double[0][1], _state)+ae_sqr(v.ptr.pp_double[0][2], _state), _state);
                            if( ae_fp_greater(pvdnorm,vnorm*testrbfunit_tol) )
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
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing basic functionality of RBF module on regular grids with
multi-layer algorithm in 1D.

  -- ALGLIB --
     Copyright 2.03.2012 by Bochkanov Sergey
*************************************************************************/
ae_bool basicmultilayerrbf1dtest(ae_state *_state)
{
    ae_frame _frame_block;
    rbfmodel s;
    rbfreport rep;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t linterm;
    ae_int_t n;
    double q;
    double r;
    ae_int_t errtype;
    ae_vector delta;
    ae_int_t nlayers;
    double a;
    double b;
    double f1;
    double f2;
    ae_vector a1;
    ae_vector b1;
    ae_matrix gp;
    ae_vector x;
    ae_vector y;
    ae_matrix mody0;
    ae_matrix mody1;
    ae_matrix gy;
    ae_vector gpgx0;
    ae_vector gpgx1;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _rbfmodel_init(&s, _state);
    _rbfreport_init(&rep, _state);
    ae_vector_init(&delta, 0, DT_REAL, _state);
    ae_vector_init(&a1, 0, DT_REAL, _state);
    ae_vector_init(&b1, 0, DT_REAL, _state);
    ae_matrix_init(&gp, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_matrix_init(&mody0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&mody1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&gy, 0, 0, DT_REAL, _state);
    ae_vector_init(&gpgx0, 0, DT_REAL, _state);
    ae_vector_init(&gpgx1, 0, DT_REAL, _state);

    a = 1.0;
    b = (double)1/(double)9;
    f1 = 1.0;
    f2 = 10.0;
    passcount = 5;
    n = 100;
    ae_vector_set_length(&gpgx0, n, _state);
    ae_vector_set_length(&gpgx1, n, _state);
    for(i=0; i<=n-1; i++)
    {
        gpgx0.ptr.p_double[i] = (double)i/(double)n;
        gpgx1.ptr.p_double[i] = (double)(0);
    }
    r = (double)(1);
    for(pass=0; pass<=passcount-1; pass++)
    {
        nx = ae_randominteger(2, _state)+2;
        ny = ae_randominteger(3, _state)+1;
        linterm = ae_randominteger(3, _state)+1;
        ae_vector_set_length(&x, nx, _state);
        ae_vector_set_length(&y, ny, _state);
        ae_vector_set_length(&a1, ny, _state);
        ae_vector_set_length(&b1, ny, _state);
        ae_vector_set_length(&delta, ny, _state);
        ae_matrix_set_length(&mody0, n, ny, _state);
        ae_matrix_set_length(&mody1, n, ny, _state);
        for(i=0; i<=ny-1; i++)
        {
            a1.ptr.p_double[i] = a+0.01*a*(2*ae_randomreal(_state)-1);
            b1.ptr.p_double[i] = b+0.01*b*(2*ae_randomreal(_state)-1);
            delta.ptr.p_double[i] = 0.35*b1.ptr.p_double[i];
        }
        ae_matrix_set_length(&gp, n, nx+ny, _state);
        
        /*
         * create grid
         */
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=nx-1; j++)
            {
                gp.ptr.pp_double[i][j] = (double)(0);
            }
            gp.ptr.pp_double[i][0] = (double)i/(double)n;
            for(j=0; j<=ny-1; j++)
            {
                gp.ptr.pp_double[i][nx+j] = a1.ptr.p_double[j]*ae_cos(f1*2*ae_pi*gp.ptr.pp_double[i][0], _state)+b1.ptr.p_double[j]*ae_cos(f2*2*ae_pi*gp.ptr.pp_double[i][0], _state);
                mody0.ptr.pp_double[i][j] = gp.ptr.pp_double[i][nx+j];
            }
        }
        q = (double)(1);
        nlayers = 1;
        errtype = 1;
        
        /*
         * test multilayer algorithm with different parameters
         */
        while(ae_fp_greater_eq(q,1/(2*f2)))
        {
            rbfcreate(nx, ny, &s, _state);
            rbfsetalgomultilayer(&s, r, nlayers, 0.0, _state);
            if( linterm==1 )
            {
                rbfsetlinterm(&s, _state);
            }
            if( linterm==2 )
            {
                rbfsetconstterm(&s, _state);
            }
            if( linterm==3 )
            {
                rbfsetzeroterm(&s, _state);
            }
            rbfsetpoints(&s, &gp, n, _state);
            rbfbuildmodel(&s, &rep, _state);
            if( ny==1 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=nx-1; j++)
                    {
                        x.ptr.p_double[j] = gp.ptr.pp_double[i][j];
                    }
                    if( nx==2 )
                    {
                        mody1.ptr.pp_double[i][0] = rbfcalc2(&s, x.ptr.p_double[0], x.ptr.p_double[1], _state);
                    }
                    else
                    {
                        if( nx==3 )
                        {
                            mody1.ptr.pp_double[i][0] = rbfcalc3(&s, x.ptr.p_double[0], x.ptr.p_double[1], x.ptr.p_double[2], _state);
                        }
                        else
                        {
                            ae_assert(ae_false, "BasicMultiLayerRBFTest1D: Invalid variable NX(NX neither 2 nor 3)", _state);
                        }
                    }
                }
                if( testrbfunit_searcherr(&mody0, &mody1, n, ny, errtype, &b1, &delta, _state) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                if( nx==2 )
                {
                    rbfgridcalc2(&s, &gpgx0, n, &gpgx1, n, &gy, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        mody1.ptr.pp_double[i][0] = gy.ptr.pp_double[i][0];
                    }
                }
                if( testrbfunit_searcherr(&mody0, &mody1, n, ny, errtype, &b1, &delta, _state) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=nx-1; j++)
                {
                    x.ptr.p_double[j] = gp.ptr.pp_double[i][j];
                }
                rbfcalc(&s, &x, &y, _state);
                for(j=0; j<=ny-1; j++)
                {
                    mody1.ptr.pp_double[i][j] = y.ptr.p_double[j];
                }
            }
            if( testrbfunit_searcherr(&mody0, &mody1, n, ny, errtype, &b1, &delta, _state) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=nx-1; j++)
                {
                    x.ptr.p_double[j] = gp.ptr.pp_double[i][j];
                }
                rbfcalcbuf(&s, &x, &y, _state);
                for(j=0; j<=ny-1; j++)
                {
                    mody1.ptr.pp_double[i][j] = y.ptr.p_double[j];
                }
            }
            if( testrbfunit_searcherr(&mody0, &mody1, n, ny, errtype, &b1, &delta, _state) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            q = q/2;
            nlayers = nlayers+1;
            if( errtype==1&&ae_fp_less_eq(q,1/f2) )
            {
                errtype = 2;
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function tests special cases:
* uninitialized RBF model will correctly return zero values
* RBF correctly handles 1 or 2 distinct points
* when  we have many uniformly spaced points and one outlier, filter which
  is applied to radii, makes all radii equal.
* RBF with NLayers=0 gives linear model

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testrbfunit_specialtest(ae_state *_state)
{
    ae_frame _frame_block;
    rbfmodel s;
    rbfreport rep;
    ae_int_t n;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t t;
    ae_matrix xy;
    ae_vector x;
    ae_vector y;
    ae_int_t termtype;
    double errtol;
    ae_int_t tmpnx;
    ae_int_t tmpny;
    ae_int_t tmpnc;
    ae_matrix xwr;
    ae_matrix v;
    double sx;
    double z;
    double va;
    double vb;
    double vc;
    double vd;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _rbfmodel_init(&s, _state);
    _rbfreport_init(&rep, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_matrix_init(&xwr, 0, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);

    errtol = 1.0E-9;
    result = ae_false;
    
    /*
     * Create model in the default state (no parameters/points specified).
     * With probability 0.5 we do one of the following:
     * * test that default state of the model is a zero model (all Calc()
     *   functions return zero)
     * * call RBFBuildModel() (without specifying anything) and test  that
     *   all Calc() functions return zero.
     */
    for(nx=2; nx<=3; nx++)
    {
        for(ny=1; ny<=3; ny++)
        {
            rbfcreate(nx, ny, &s, _state);
            if( ae_fp_greater(ae_randomreal(_state),0.5) )
            {
                rbfbuildmodel(&s, &rep, _state);
                if( rep.terminationtype<=0 )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
            ae_vector_set_length(&x, nx, _state);
            for(i=0; i<=nx-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            rbfcalc(&s, &x, &y, _state);
            if( y.cnt!=ny )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(i=0; i<=ny-1; i++)
            {
                if( ae_fp_neq(y.ptr.p_double[i],(double)(0)) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
    }
    
    /*
     * Create model with 1 point and different types of linear term.
     * Test algorithm on such dataset.
     */
    for(nx=2; nx<=3; nx++)
    {
        for(ny=1; ny<=3; ny++)
        {
            rbfcreate(nx, ny, &s, _state);
            for(termtype=0; termtype<=2; termtype++)
            {
                if( termtype==0 )
                {
                    rbfsetlinterm(&s, _state);
                }
                if( termtype==1 )
                {
                    rbfsetconstterm(&s, _state);
                }
                if( termtype==2 )
                {
                    rbfsetzeroterm(&s, _state);
                }
                ae_matrix_set_length(&xy, 1, nx+ny, _state);
                for(i=0; i<=nx+ny-1; i++)
                {
                    xy.ptr.pp_double[0][i] = 2*ae_randomreal(_state)-1;
                }
                rbfsetpoints(&s, &xy, 1, _state);
                rbfbuildmodel(&s, &rep, _state);
                if( rep.terminationtype<=0 )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                
                /*
                 * First, test that model exactly reproduces our dataset 
                 */
                ae_vector_set_length(&x, nx, _state);
                for(i=0; i<=nx-1; i++)
                {
                    x.ptr.p_double[i] = xy.ptr.pp_double[0][i];
                }
                rbfcalc(&s, &x, &y, _state);
                if( y.cnt!=ny )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=ny-1; i++)
                {
                    if( ae_fp_greater(ae_fabs(y.ptr.p_double[i]-xy.ptr.pp_double[0][nx+i], _state),errtol) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
                
                /*
                 * Second, test that model is constant unless it has zero polynomial term
                 * (in the latter case we have small "bump" around lone interpolation center)
                 */
                if( termtype==0||termtype==1 )
                {
                    for(i=0; i<=nx-1; i++)
                    {
                        x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    rbfcalc(&s, &x, &y, _state);
                    if( y.cnt!=ny )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                    for(i=0; i<=ny-1; i++)
                    {
                        if( ae_fp_greater(ae_fabs(y.ptr.p_double[i]-xy.ptr.pp_double[0][nx+i], _state),errtol) )
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
    
    /*
     * Create model with 2 points and different types of linear term.
     * Test algorithm on such dataset.
     */
    for(nx=2; nx<=3; nx++)
    {
        for(ny=1; ny<=3; ny++)
        {
            rbfcreate(nx, ny, &s, _state);
            for(termtype=0; termtype<=2; termtype++)
            {
                if( termtype==0 )
                {
                    rbfsetlinterm(&s, _state);
                }
                if( termtype==1 )
                {
                    rbfsetconstterm(&s, _state);
                }
                if( termtype==2 )
                {
                    rbfsetzeroterm(&s, _state);
                }
                ae_matrix_set_length(&xy, 2, nx+ny, _state);
                for(i=0; i<=nx+ny-1; i++)
                {
                    xy.ptr.pp_double[0][i] = 2*ae_randomreal(_state)-1;
                }
                for(i=0; i<=nx+ny-1; i++)
                {
                    xy.ptr.pp_double[1][i] = xy.ptr.pp_double[0][i]+1.0;
                }
                rbfsetpoints(&s, &xy, 2, _state);
                rbfbuildmodel(&s, &rep, _state);
                if( rep.terminationtype<=0 )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                for(j=0; j<=1; j++)
                {
                    ae_vector_set_length(&x, nx, _state);
                    for(i=0; i<=nx-1; i++)
                    {
                        x.ptr.p_double[i] = xy.ptr.pp_double[j][i];
                    }
                    rbfcalc(&s, &x, &y, _state);
                    if( y.cnt!=ny )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                    for(i=0; i<=ny-1; i++)
                    {
                        if( ae_fp_greater(ae_fabs(y.ptr.p_double[i]-xy.ptr.pp_double[j][nx+i], _state),errtol) )
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
    
    /*
     * Generate a set of points (xi,yi) = (SX*i,0), and one
     * outlier (x_far,y_far)=(-1000*SX,0).
     *
     * Radii filtering should place a bound on the radius of outlier.
     */
    for(nx=2; nx<=3; nx++)
    {
        for(ny=1; ny<=3; ny++)
        {
            sx = ae_exp(-5+10*ae_randomreal(_state), _state);
            rbfcreate(nx, ny, &s, _state);
            ae_matrix_set_length(&xy, 20, nx+ny, _state);
            for(i=0; i<=xy.rows-1; i++)
            {
                xy.ptr.pp_double[i][0] = sx*i;
                for(j=1; j<=nx-1; j++)
                {
                    xy.ptr.pp_double[i][j] = (double)(0);
                }
                for(j=0; j<=ny-1; j++)
                {
                    xy.ptr.pp_double[i][nx+j] = ae_randomreal(_state);
                }
            }
            xy.ptr.pp_double[xy.rows-1][0] = -1000*sx;
            rbfsetpoints(&s, &xy, xy.rows, _state);
            
            /*
             * Try random Z from [1,5]
             */
            z = 1+ae_randomreal(_state)*4;
            rbfsetalgoqnn(&s, 1.0, z, _state);
            rbfbuildmodel(&s, &rep, _state);
            if( rep.terminationtype<=0 )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            rbfunpack(&s, &tmpnx, &tmpny, &xwr, &tmpnc, &v, _state);
            if( (((tmpnx!=nx||tmpny!=ny)||tmpnc!=xy.rows)||xwr.cols!=nx+ny+1)||xwr.rows!=tmpnc )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(i=0; i<=tmpnc-2; i++)
            {
                if( ae_fp_greater(ae_fabs(xwr.ptr.pp_double[i][nx+ny]-sx, _state),errtol) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
            if( ae_fp_greater(ae_fabs(xwr.ptr.pp_double[tmpnc-1][nx+ny]-z*sx, _state),errtol) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
    }
    
    /*
     * RBF with NLayers=0 gives us linear model.
     *
     * In order to perform this test, we use test function which
     * is perfectly linear and see whether RBF model is able to
     * reproduce such function.
     */
    n = 5;
    for(ny=1; ny<=3; ny++)
    {
        va = 2*ae_randomreal(_state)-1;
        vb = 2*ae_randomreal(_state)-1;
        vc = 2*ae_randomreal(_state)-1;
        vd = 2*ae_randomreal(_state)-1;
        
        /*
         * Test NX=2.
         * Generate linear function using random coefficients VA/VB/VC.
         * Function is K-dimensional vector-valued, each component has slightly
         * different coefficients.
         */
        ae_matrix_set_length(&xy, n*n, 2+ny, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                xy.ptr.pp_double[n*i+j][0] = (double)(i);
                xy.ptr.pp_double[n*i+j][1] = (double)(j);
                for(k=0; k<=ny-1; k++)
                {
                    xy.ptr.pp_double[n*i+j][2+k] = (va+0.1*k)*i+(vb+0.2*k)*j+(vc+0.3*k);
                }
            }
        }
        rbfcreate(2, ny, &s, _state);
        rbfsetpoints(&s, &xy, n*n, _state);
        rbfsetalgomultilayer(&s, 1.0, 0, 0.01, _state);
        rbfbuildmodel(&s, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        ae_vector_set_length(&x, 2, _state);
        x.ptr.p_double[0] = (n-1)*ae_randomreal(_state);
        x.ptr.p_double[1] = (n-1)*ae_randomreal(_state);
        if( ny==1&&ae_fp_greater(ae_fabs(rbfcalc2(&s, x.ptr.p_double[0], x.ptr.p_double[1], _state)-(va*x.ptr.p_double[0]+vb*x.ptr.p_double[1]+vc), _state),errtol) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        rbfcalc(&s, &x, &y, _state);
        for(k=0; k<=ny-1; k++)
        {
            if( ae_fp_greater(ae_fabs(y.ptr.p_double[k]-((va+0.1*k)*x.ptr.p_double[0]+(vb+0.2*k)*x.ptr.p_double[1]+(vc+0.3*k)), _state),errtol) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
        
        /*
         * Test NX=3.
         * Generate linear function using random coefficients VA/VB/VC/VC.
         * Function is K-dimensional vector-valued, each component has slightly
         * different coefficients.
         */
        ae_matrix_set_length(&xy, n*n*n, 3+ny, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                for(t=0; t<=n-1; t++)
                {
                    xy.ptr.pp_double[n*n*i+n*j+t][0] = (double)(i);
                    xy.ptr.pp_double[n*n*i+n*j+t][1] = (double)(j);
                    xy.ptr.pp_double[n*n*i+n*j+t][2] = (double)(t);
                    for(k=0; k<=ny-1; k++)
                    {
                        xy.ptr.pp_double[n*n*i+n*j+t][3+k] = (va+0.1*k)*i+(vb+0.2*k)*j+(vc+0.3*k)*t+(vd+0.4*k);
                    }
                }
            }
        }
        rbfcreate(3, ny, &s, _state);
        rbfsetpoints(&s, &xy, n*n*n, _state);
        rbfsetalgomultilayer(&s, 1.0, 0, 0.01, _state);
        rbfbuildmodel(&s, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        ae_vector_set_length(&x, 3, _state);
        x.ptr.p_double[0] = (n-1)*ae_randomreal(_state);
        x.ptr.p_double[1] = (n-1)*ae_randomreal(_state);
        x.ptr.p_double[2] = (n-1)*ae_randomreal(_state);
        if( ny==1&&ae_fp_greater(ae_fabs(rbfcalc3(&s, x.ptr.p_double[0], x.ptr.p_double[1], x.ptr.p_double[2], _state)-(va*x.ptr.p_double[0]+vb*x.ptr.p_double[1]+vc*x.ptr.p_double[2]+vd), _state),errtol) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        rbfcalc(&s, &x, &y, _state);
        for(k=0; k<=ny-1; k++)
        {
            if( ae_fp_greater(ae_fabs(y.ptr.p_double[k]-((va+0.1*k)*x.ptr.p_double[0]+(vb+0.2*k)*x.ptr.p_double[1]+(vc+0.3*k)*x.ptr.p_double[2]+(vd+0.4*k)), _state),errtol) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing basic functionality of RBF module on regular grids.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testrbfunit_basicrbftest(ae_state *_state)
{
    ae_frame _frame_block;
    rbfmodel s;
    rbfreport rep;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t k0;
    ae_int_t k1;
    ae_int_t k2;
    ae_int_t linterm;
    ae_int_t np;
    double sx;
    double sy;
    double zx;
    double px;
    double zy;
    double py;
    double q;
    double z;
    ae_vector point;
    ae_matrix gp;
    ae_vector x;
    ae_vector y;
    ae_matrix gy;
    ae_int_t unx;
    ae_int_t uny;
    ae_matrix xwr;
    ae_matrix v;
    ae_vector gpgx0;
    ae_vector gpgx1;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t l;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _rbfmodel_init(&s, _state);
    _rbfreport_init(&rep, _state);
    ae_vector_init(&point, 0, DT_REAL, _state);
    ae_matrix_init(&gp, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_matrix_init(&gy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xwr, 0, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);
    ae_vector_init(&gpgx0, 0, DT_REAL, _state);
    ae_vector_init(&gpgx1, 0, DT_REAL, _state);

    zx = (double)(10);
    px = (double)(15);
    zy = (double)(10);
    py = (double)(15);
    
    /*
     * Problem types:
     * * 2 and 3-dimensional problems
     * * problems with zero, constant, linear terms
     * * different scalings of X and Y values (1.0, 1E-15, 1E+15)
     * * regular grids different grid sizes (from 2 to 4 points for each dimension)
     *
     * We check that:
     * * RBF model correctly reproduces function value (testes with different Calc() functions)
     * * unpacked model containt correct radii
     * * linear term has correct form
     */
    for(nx=2; nx<=3; nx++)
    {
        for(ny=1; ny<=3; ny++)
        {
            for(linterm=1; linterm<=3; linterm++)
            {
                
                /*
                 * prepare test problem
                 */
                sx = ae_pow(zx, px*(ae_randominteger(3, _state)-1), _state);
                sy = ae_pow(zy, py*(ae_randominteger(3, _state)-1), _state);
                ae_vector_set_length(&x, nx, _state);
                ae_vector_set_length(&y, ny, _state);
                ae_vector_set_length(&point, nx, _state);
                rbfcreate(nx, ny, &s, _state);
                rbfsetcond(&s, testrbfunit_heps, testrbfunit_heps, testrbfunit_mxits, _state);
                q = 0.25+ae_randomreal(_state);
                z = 4.5+ae_randomreal(_state);
                rbfsetalgoqnn(&s, q, z, _state);
                if( linterm==1 )
                {
                    rbfsetlinterm(&s, _state);
                }
                if( linterm==2 )
                {
                    rbfsetconstterm(&s, _state);
                }
                if( linterm==3 )
                {
                    rbfsetzeroterm(&s, _state);
                }
                
                /*
                 * start points for grid
                 */
                for(i=0; i<=nx-1; i++)
                {
                    point.ptr.p_double[i] = sx*(2*ae_randomreal(_state)-1);
                }
                
                /*
                 * 2-dimensional test problem
                 */
                if( nx==2 )
                {
                    for(k0=2; k0<=4; k0++)
                    {
                        for(k1=2; k1<=4; k1++)
                        {
                            np = k0*k1;
                            ae_matrix_set_length(&gp, np, nx+ny, _state);
                            
                            /*
                             * create grid
                             */
                            for(i=0; i<=k0-1; i++)
                            {
                                for(j=0; j<=k1-1; j++)
                                {
                                    gp.ptr.pp_double[i*k1+j][0] = point.ptr.p_double[0]+sx*i;
                                    gp.ptr.pp_double[i*k1+j][1] = point.ptr.p_double[1]+sx*j;
                                    for(k=0; k<=ny-1; k++)
                                    {
                                        gp.ptr.pp_double[i*k1+j][nx+k] = sy*(2*ae_randomreal(_state)-1);
                                    }
                                }
                            }
                            rbfsetpoints(&s, &gp, np, _state);
                            rbfbuildmodel(&s, &rep, _state);
                            if( ny==1 )
                            {
                                ae_vector_set_length(&gpgx0, k0, _state);
                                ae_vector_set_length(&gpgx1, k1, _state);
                                for(i=0; i<=k0-1; i++)
                                {
                                    gpgx0.ptr.p_double[i] = point.ptr.p_double[0]+sx*i;
                                }
                                for(i=0; i<=k1-1; i++)
                                {
                                    gpgx1.ptr.p_double[i] = point.ptr.p_double[1]+sx*i;
                                }
                                rbfgridcalc2(&s, &gpgx0, k0, &gpgx1, k1, &gy, _state);
                                for(i=0; i<=k0-1; i++)
                                {
                                    for(j=0; j<=k1-1; j++)
                                    {
                                        if( ae_fp_greater(ae_fabs(gy.ptr.pp_double[i][j]-gp.ptr.pp_double[i*k1+j][nx], _state),sy*testrbfunit_eps) )
                                        {
                                            result = ae_true;
                                            ae_frame_leave(_state);
                                            return result;
                                        }
                                    }
                                }
                            }
                            for(i=0; i<=np-1; i++)
                            {
                                x.ptr.p_double[0] = gp.ptr.pp_double[i][0];
                                x.ptr.p_double[1] = gp.ptr.pp_double[i][1];
                                if( ny==1 )
                                {
                                    y.ptr.p_double[0] = rbfcalc2(&s, x.ptr.p_double[0], x.ptr.p_double[1], _state);
                                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx]-y.ptr.p_double[0], _state),sy*testrbfunit_eps) )
                                    {
                                        result = ae_true;
                                        ae_frame_leave(_state);
                                        return result;
                                    }
                                }
                                rbfcalc(&s, &x, &y, _state);
                                for(j=0; j<=ny-1; j++)
                                {
                                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),sy*testrbfunit_eps) )
                                    {
                                        result = ae_true;
                                        ae_frame_leave(_state);
                                        return result;
                                    }
                                }
                                rbfcalcbuf(&s, &x, &y, _state);
                                for(j=0; j<=ny-1; j++)
                                {
                                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),sy*testrbfunit_eps) )
                                    {
                                        result = ae_true;
                                        ae_frame_leave(_state);
                                        return result;
                                    }
                                }
                            }
                            
                            /*
                             * test for RBFUnpack
                             */
                            rbfunpack(&s, &unx, &uny, &xwr, &np, &v, _state);
                            if( ((((nx!=unx||ny!=uny)||xwr.rows!=np)||xwr.cols!=nx+ny+1)||v.rows!=ny)||v.cols!=nx+1 )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                            for(i=0; i<=np-1; i++)
                            {
                                if( ae_fp_greater(ae_fabs(xwr.ptr.pp_double[i][unx+uny]-q*sx, _state),sx*testrbfunit_eps) )
                                {
                                    result = ae_true;
                                    ae_frame_leave(_state);
                                    return result;
                                }
                            }
                            if( linterm==2 )
                            {
                                for(i=0; i<=unx-1; i++)
                                {
                                    for(j=0; j<=uny-1; j++)
                                    {
                                        if( ae_fp_neq(v.ptr.pp_double[j][i],(double)(0)) )
                                        {
                                            result = ae_true;
                                            ae_frame_leave(_state);
                                            return result;
                                        }
                                    }
                                }
                            }
                            if( linterm==3 )
                            {
                                for(i=0; i<=unx; i++)
                                {
                                    for(j=0; j<=uny-1; j++)
                                    {
                                        if( ae_fp_neq(v.ptr.pp_double[j][i],(double)(0)) )
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
                }
                
                /*
                 * 3-dimensional test problems
                 */
                if( nx==3 )
                {
                    for(k0=2; k0<=4; k0++)
                    {
                        for(k1=2; k1<=4; k1++)
                        {
                            for(k2=2; k2<=4; k2++)
                            {
                                np = k0*k1*k2;
                                ae_matrix_set_length(&gp, np, nx+ny, _state);
                                
                                /*
                                 * create grid
                                 */
                                for(i=0; i<=k0-1; i++)
                                {
                                    for(j=0; j<=k1-1; j++)
                                    {
                                        for(k=0; k<=k2-1; k++)
                                        {
                                            gp.ptr.pp_double[(i*k1+j)*k2+k][0] = point.ptr.p_double[0]+sx*i;
                                            gp.ptr.pp_double[(i*k1+j)*k2+k][1] = point.ptr.p_double[1]+sx*j;
                                            gp.ptr.pp_double[(i*k1+j)*k2+k][2] = point.ptr.p_double[2]+sx*k;
                                            for(l=0; l<=ny-1; l++)
                                            {
                                                gp.ptr.pp_double[(i*k1+j)*k2+k][nx+l] = sy*(2*ae_randomreal(_state)-1);
                                            }
                                        }
                                    }
                                }
                                rbfsetpoints(&s, &gp, np, _state);
                                rbfbuildmodel(&s, &rep, _state);
                                for(i=0; i<=np-1; i++)
                                {
                                    x.ptr.p_double[0] = gp.ptr.pp_double[i][0];
                                    x.ptr.p_double[1] = gp.ptr.pp_double[i][1];
                                    x.ptr.p_double[2] = gp.ptr.pp_double[i][2];
                                    if( ny==1 )
                                    {
                                        y.ptr.p_double[0] = rbfcalc3(&s, x.ptr.p_double[0], x.ptr.p_double[1], x.ptr.p_double[2], _state);
                                        if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx]-y.ptr.p_double[0], _state),sy*testrbfunit_eps) )
                                        {
                                            result = ae_true;
                                            ae_frame_leave(_state);
                                            return result;
                                        }
                                    }
                                    rbfcalc(&s, &x, &y, _state);
                                    for(j=0; j<=ny-1; j++)
                                    {
                                        if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),sy*testrbfunit_eps) )
                                        {
                                            result = ae_true;
                                            ae_frame_leave(_state);
                                            return result;
                                        }
                                    }
                                    rbfcalcbuf(&s, &x, &y, _state);
                                    for(j=0; j<=ny-1; j++)
                                    {
                                        if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),sy*testrbfunit_eps) )
                                        {
                                            result = ae_true;
                                            ae_frame_leave(_state);
                                            return result;
                                        }
                                    }
                                }
                                
                                /*
                                 * test for RBFUnpack
                                 */
                                rbfunpack(&s, &unx, &uny, &xwr, &np, &v, _state);
                                if( ((((nx!=unx||ny!=uny)||xwr.rows!=np)||xwr.cols!=nx+ny+1)||v.rows!=ny)||v.cols!=nx+1 )
                                {
                                    result = ae_true;
                                    ae_frame_leave(_state);
                                    return result;
                                }
                                for(i=0; i<=np-1; i++)
                                {
                                    if( ae_fp_greater(ae_fabs(xwr.ptr.pp_double[i][unx+uny]-q*sx, _state),sx*testrbfunit_eps) )
                                    {
                                        result = ae_true;
                                        ae_frame_leave(_state);
                                        return result;
                                    }
                                }
                                if( linterm==2 )
                                {
                                    for(i=0; i<=unx-1; i++)
                                    {
                                        for(j=0; j<=uny-1; j++)
                                        {
                                            if( ae_fp_neq(v.ptr.pp_double[j][i],(double)(0)) )
                                            {
                                                result = ae_true;
                                                ae_frame_leave(_state);
                                                return result;
                                            }
                                        }
                                    }
                                }
                                if( linterm==3 )
                                {
                                    for(i=0; i<=unx; i++)
                                    {
                                        for(j=0; j<=uny-1; j++)
                                        {
                                            if( ae_fp_neq(v.ptr.pp_double[j][i],(double)(0)) )
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
Function for testing RBF module on irregular grids.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testrbfunit_irregularrbftest(ae_state *_state)
{
    ae_frame _frame_block;
    rbfmodel s;
    rbfreport rep;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t k0;
    ae_int_t k1;
    ae_int_t k2;
    ae_int_t linterm;
    ae_int_t np;
    double sx;
    double sy;
    double zx;
    double px;
    double zy;
    double py;
    double q;
    double z;
    ae_vector point;
    ae_matrix gp;
    ae_vector x;
    ae_vector y;
    ae_matrix gy;
    double noiselevel;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t l;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _rbfmodel_init(&s, _state);
    _rbfreport_init(&rep, _state);
    ae_vector_init(&point, 0, DT_REAL, _state);
    ae_matrix_init(&gp, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_matrix_init(&gy, 0, 0, DT_REAL, _state);

    zx = (double)(10);
    px = (double)(15);
    zy = (double)(10);
    py = (double)(15);
    noiselevel = 0.1;
    
    /*
     * Problem types:
     * * 2 and 3-dimensional problems
     * * problems with zero, constant, linear terms
     * * different scalings of X and Y values (1.0, 1E-15, 1E+15)
     * * noisy grids, which are just regular grids with different grid sizes
     *   (from 2 to 4 points for each dimension) and moderate amount of random
     *   noise added to all node positions.
     *
     * We check that:
     * * RBF model correctly reproduces function value (testes with different Calc() functions)
     */
    for(nx=2; nx<=3; nx++)
    {
        for(ny=1; ny<=3; ny++)
        {
            for(linterm=1; linterm<=3; linterm++)
            {
                
                /*
                 * prepare test problem
                 */
                sx = ae_pow(zx, px*(ae_randominteger(3, _state)-1), _state);
                sy = ae_pow(zy, py*(ae_randominteger(3, _state)-1), _state);
                ae_vector_set_length(&x, nx, _state);
                ae_vector_set_length(&y, ny, _state);
                ae_vector_set_length(&point, nx, _state);
                rbfcreate(nx, ny, &s, _state);
                rbfsetcond(&s, testrbfunit_heps, testrbfunit_heps, testrbfunit_mxits, _state);
                q = 0.25+ae_randomreal(_state);
                z = 4.5+ae_randomreal(_state);
                rbfsetalgoqnn(&s, q, z, _state);
                if( linterm==1 )
                {
                    rbfsetlinterm(&s, _state);
                }
                if( linterm==2 )
                {
                    rbfsetconstterm(&s, _state);
                }
                if( linterm==3 )
                {
                    rbfsetzeroterm(&s, _state);
                }
                
                /*
                 * start points for grid
                 */
                for(i=0; i<=nx-1; i++)
                {
                    point.ptr.p_double[i] = sx*(2*ae_randomreal(_state)-1);
                }
                
                /*
                 * 2-dimensional test problems
                 */
                if( nx==2 )
                {
                    for(k0=2; k0<=4; k0++)
                    {
                        for(k1=2; k1<=4; k1++)
                        {
                            np = k0*k1;
                            ae_matrix_set_length(&gp, np, nx+ny, _state);
                            
                            /*
                             * create grid
                             */
                            for(i=0; i<=k0-1; i++)
                            {
                                for(j=0; j<=k1-1; j++)
                                {
                                    gp.ptr.pp_double[i*k1+j][0] = point.ptr.p_double[0]+sx*i+noiselevel*sx*(2*ae_randomreal(_state)-1);
                                    gp.ptr.pp_double[i*k1+j][1] = point.ptr.p_double[1]+sx*j+noiselevel*sx*(2*ae_randomreal(_state)-1);
                                    for(k=0; k<=ny-1; k++)
                                    {
                                        gp.ptr.pp_double[i*k1+j][nx+k] = sy*(2*ae_randomreal(_state)-1);
                                    }
                                }
                            }
                            rbfsetpoints(&s, &gp, np, _state);
                            rbfbuildmodel(&s, &rep, _state);
                            for(i=0; i<=np-1; i++)
                            {
                                x.ptr.p_double[0] = gp.ptr.pp_double[i][0];
                                x.ptr.p_double[1] = gp.ptr.pp_double[i][1];
                                if( ny==1 )
                                {
                                    y.ptr.p_double[0] = rbfcalc2(&s, x.ptr.p_double[0], x.ptr.p_double[1], _state);
                                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx]-y.ptr.p_double[0], _state),sy*testrbfunit_eps) )
                                    {
                                        result = ae_true;
                                        ae_frame_leave(_state);
                                        return result;
                                    }
                                }
                                rbfcalc(&s, &x, &y, _state);
                                for(j=0; j<=ny-1; j++)
                                {
                                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),sy*testrbfunit_eps) )
                                    {
                                        result = ae_true;
                                        ae_frame_leave(_state);
                                        return result;
                                    }
                                }
                                rbfcalcbuf(&s, &x, &y, _state);
                                for(j=0; j<=ny-1; j++)
                                {
                                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),sy*testrbfunit_eps) )
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
                
                /*
                 * 2-dimensional test problems
                 */
                if( nx==3 )
                {
                    for(k0=2; k0<=4; k0++)
                    {
                        for(k1=2; k1<=4; k1++)
                        {
                            for(k2=2; k2<=4; k2++)
                            {
                                np = k0*k1*k2;
                                ae_matrix_set_length(&gp, np, nx+ny, _state);
                                
                                /*
                                 * create grid
                                 */
                                for(i=0; i<=k0-1; i++)
                                {
                                    for(j=0; j<=k1-1; j++)
                                    {
                                        for(k=0; k<=k2-1; k++)
                                        {
                                            gp.ptr.pp_double[(i*k1+j)*k2+k][0] = point.ptr.p_double[0]+sx*i+noiselevel*sx*(2*ae_randomreal(_state)-1);
                                            gp.ptr.pp_double[(i*k1+j)*k2+k][1] = point.ptr.p_double[1]+sx*j+noiselevel*sx*(2*ae_randomreal(_state)-1);
                                            gp.ptr.pp_double[(i*k1+j)*k2+k][2] = point.ptr.p_double[2]+sx*k+noiselevel*sx*(2*ae_randomreal(_state)-1);
                                            for(l=0; l<=ny-1; l++)
                                            {
                                                gp.ptr.pp_double[(i*k1+j)*k2+k][nx+l] = sy*(2*ae_randomreal(_state)-1);
                                            }
                                        }
                                    }
                                }
                                rbfsetpoints(&s, &gp, np, _state);
                                rbfbuildmodel(&s, &rep, _state);
                                for(i=0; i<=np-1; i++)
                                {
                                    x.ptr.p_double[0] = gp.ptr.pp_double[i][0];
                                    x.ptr.p_double[1] = gp.ptr.pp_double[i][1];
                                    x.ptr.p_double[2] = gp.ptr.pp_double[i][2];
                                    if( ny==1 )
                                    {
                                        y.ptr.p_double[0] = rbfcalc3(&s, x.ptr.p_double[0], x.ptr.p_double[1], x.ptr.p_double[2], _state);
                                        if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx]-y.ptr.p_double[0], _state),sy*testrbfunit_eps) )
                                        {
                                            result = ae_true;
                                            ae_frame_leave(_state);
                                            return result;
                                        }
                                    }
                                    rbfcalc(&s, &x, &y, _state);
                                    for(j=0; j<=ny-1; j++)
                                    {
                                        if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),sy*testrbfunit_eps) )
                                        {
                                            result = ae_true;
                                            ae_frame_leave(_state);
                                            return result;
                                        }
                                    }
                                    rbfcalcbuf(&s, &x, &y, _state);
                                    for(j=0; j<=ny-1; j++)
                                    {
                                        if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),sy*testrbfunit_eps) )
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
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
The test does  check, that  algorithm  can build linear model for the data
sets, when Y depends on X linearly.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testrbfunit_linearitymodelrbftest(ae_state *_state)
{
    ae_frame _frame_block;
    rbfmodel s;
    rbfreport rep;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t k0;
    ae_int_t k1;
    ae_int_t k2;
    ae_int_t linterm;
    ae_int_t np;
    double sx;
    double sy;
    double zx;
    double px;
    double zy;
    double py;
    double q;
    double z;
    ae_vector point;
    ae_vector a;
    ae_matrix gp;
    ae_vector x;
    ae_vector y;
    ae_int_t unx;
    ae_int_t uny;
    ae_matrix xwr;
    ae_matrix v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t l;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _rbfmodel_init(&s, _state);
    _rbfreport_init(&rep, _state);
    ae_vector_init(&point, 0, DT_REAL, _state);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_matrix_init(&gp, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_matrix_init(&xwr, 0, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);

    zx = (double)(10);
    px = (double)(15);
    zy = (double)(10);
    py = (double)(15);
    ny = 1;
    for(nx=2; nx<=3; nx++)
    {
        for(linterm=1; linterm<=3; linterm++)
        {
            
            /*
             * prepare test problem
             */
            sx = ae_pow(zx, px*(ae_randominteger(3, _state)-1), _state);
            sy = ae_pow(zy, py*(ae_randominteger(3, _state)-1), _state);
            ae_vector_set_length(&x, nx, _state);
            ae_vector_set_length(&y, ny, _state);
            ae_vector_set_length(&point, nx, _state);
            rbfcreate(nx, ny, &s, _state);
            q = 0.25+ae_randomreal(_state);
            z = 4.5+ae_randomreal(_state);
            rbfsetalgoqnn(&s, q, z, _state);
            ae_vector_set_length(&a, nx+1, _state);
            if( linterm==1 )
            {
                rbfsetlinterm(&s, _state);
                for(i=0; i<=nx-1; i++)
                {
                    a.ptr.p_double[i] = sy*(2*ae_randomreal(_state)-1)/sx;
                }
                a.ptr.p_double[nx] = sy*(2*ae_randomreal(_state)-1);
            }
            if( linterm==2 )
            {
                rbfsetconstterm(&s, _state);
                for(i=0; i<=nx-1; i++)
                {
                    a.ptr.p_double[i] = (double)(0);
                }
                a.ptr.p_double[nx] = sy*(2*ae_randomreal(_state)-1);
            }
            if( linterm==3 )
            {
                rbfsetzeroterm(&s, _state);
                for(i=0; i<=nx; i++)
                {
                    a.ptr.p_double[i] = (double)(0);
                }
            }
            
            /*
             * start points for grid
             */
            for(i=0; i<=nx-1; i++)
            {
                point.ptr.p_double[i] = sx*(2*ae_randomreal(_state)-1);
            }
            if( nx==2 )
            {
                for(k0=2; k0<=4; k0++)
                {
                    for(k1=2; k1<=4; k1++)
                    {
                        np = k0*k1;
                        ae_matrix_set_length(&gp, np, nx+ny, _state);
                        
                        /*
                         * create grid
                         */
                        for(i=0; i<=k0-1; i++)
                        {
                            for(j=0; j<=k1-1; j++)
                            {
                                gp.ptr.pp_double[i*k1+j][0] = point.ptr.p_double[0]+sx*i;
                                gp.ptr.pp_double[i*k1+j][1] = point.ptr.p_double[1]+sx*j;
                                gp.ptr.pp_double[i*k1+j][nx] = a.ptr.p_double[nx];
                                for(k=0; k<=nx-1; k++)
                                {
                                    gp.ptr.pp_double[i*k1+j][nx] = gp.ptr.pp_double[i*k1+j][nx]+gp.ptr.pp_double[i*k1+j][k]*a.ptr.p_double[k];
                                }
                            }
                        }
                        rbfsetpoints(&s, &gp, np, _state);
                        rbfbuildmodel(&s, &rep, _state);
                        
                        /*
                         * test for RBFUnpack
                         */
                        rbfunpack(&s, &unx, &uny, &xwr, &np, &v, _state);
                        if( ((((nx!=unx||ny!=uny)||xwr.rows!=np)||xwr.cols!=nx+ny+1)||v.rows!=ny)||v.cols!=nx+1 )
                        {
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                        for(i=0; i<=nx-1; i++)
                        {
                            if( ae_fp_greater(ae_fabs(v.ptr.pp_double[0][i]-a.ptr.p_double[i], _state),sy/sx*testrbfunit_tol) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                        if( ae_fp_greater(ae_fabs(v.ptr.pp_double[0][nx]-a.ptr.p_double[nx], _state),sy*testrbfunit_tol) )
                        {
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                        for(i=0; i<=np-1; i++)
                        {
                            if( ae_fp_greater(ae_fabs(xwr.ptr.pp_double[i][unx], _state),sy*testrbfunit_tol) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                }
            }
            if( nx==3 )
            {
                for(k0=2; k0<=4; k0++)
                {
                    for(k1=2; k1<=4; k1++)
                    {
                        for(k2=2; k2<=4; k2++)
                        {
                            np = k0*k1*k2;
                            ae_matrix_set_length(&gp, np, nx+ny, _state);
                            
                            /*
                             * create grid
                             */
                            for(i=0; i<=k0-1; i++)
                            {
                                for(j=0; j<=k1-1; j++)
                                {
                                    for(k=0; k<=k2-1; k++)
                                    {
                                        gp.ptr.pp_double[(i*k1+j)*k2+k][0] = point.ptr.p_double[0]+sx*i;
                                        gp.ptr.pp_double[(i*k1+j)*k2+k][1] = point.ptr.p_double[1]+sx*j;
                                        gp.ptr.pp_double[(i*k1+j)*k2+k][2] = point.ptr.p_double[2]+sx*k;
                                        gp.ptr.pp_double[(i*k1+j)*k2+k][nx] = a.ptr.p_double[nx];
                                        for(l=0; l<=nx-1; l++)
                                        {
                                            gp.ptr.pp_double[(i*k1+j)*k2+k][nx] = gp.ptr.pp_double[(i*k1+j)*k2+k][nx]+gp.ptr.pp_double[(i*k1+j)*k2+k][l]*a.ptr.p_double[l];
                                        }
                                    }
                                }
                            }
                            rbfsetpoints(&s, &gp, np, _state);
                            rbfbuildmodel(&s, &rep, _state);
                            
                            /*
                             * test for RBFUnpack
                             */
                            rbfunpack(&s, &unx, &uny, &xwr, &np, &v, _state);
                            if( ((((nx!=unx||ny!=uny)||xwr.rows!=np)||xwr.cols!=nx+ny+1)||v.rows!=ny)||v.cols!=nx+1 )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                            for(i=0; i<=nx-1; i++)
                            {
                                if( ae_fp_greater(ae_fabs(v.ptr.pp_double[0][i]-a.ptr.p_double[i], _state),sy/sx*testrbfunit_tol) )
                                {
                                    result = ae_true;
                                    ae_frame_leave(_state);
                                    return result;
                                }
                            }
                            if( ae_fp_greater(ae_fabs(v.ptr.pp_double[0][nx]-a.ptr.p_double[nx], _state),sy*testrbfunit_tol) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                            for(i=0; i<=np-1; i++)
                            {
                                if( ae_fp_greater(ae_fabs(xwr.ptr.pp_double[i][unx], _state),sy*testrbfunit_tol) )
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
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function tests serialization

  -- ALGLIB --
     Copyright 02.02.2012 by Bochkanov Sergey
*************************************************************************/
static ae_bool testrbfunit_serializationtest(ae_state *_state)
{
    ae_frame _frame_block;
    rbfmodel s;
    rbfmodel s2;
    rbfreport rep;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t k0;
    ae_int_t k1;
    ae_int_t k2;
    ae_int_t i0;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t j;
    ae_matrix xy;
    ae_vector testpoint;
    ae_vector y0;
    ae_vector y1;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _rbfmodel_init(&s, _state);
    _rbfmodel_init(&s2, _state);
    _rbfreport_init(&rep, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&testpoint, 0, DT_REAL, _state);
    ae_vector_init(&y0, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);

    result = ae_false;
    
    /*
     * This function generates random 2 or 3 dimensional problem,
     * builds RBF model (QNN is used), serializes/unserializes it, then compares
     * models by calculating model value at some random point.
     *
     * Additionally we test that new model (one which was restored
     * after serialization) has lost all model construction settings,
     * i.e. if we call RBFBuildModel() on a NEW model, we will get
     * empty (zero) model.
     */
    for(nx=2; nx<=3; nx++)
    {
        for(ny=1; ny<=2; ny++)
        {
            
            /*
             * prepare test problem
             */
            rbfcreate(nx, ny, &s, _state);
            rbfsetalgoqnn(&s, 1.0, 5.0, _state);
            rbfsetlinterm(&s, _state);
            if( nx==2 )
            {
                
                /*
                 * 2-dimensional problem
                 */
                k0 = 2+ae_randominteger(4, _state);
                k1 = 2+ae_randominteger(4, _state);
                ae_matrix_set_length(&xy, k0*k1, nx+ny, _state);
                for(i0=0; i0<=k0-1; i0++)
                {
                    for(i1=0; i1<=k1-1; i1++)
                    {
                        xy.ptr.pp_double[i0*k1+i1][0] = i0+0.1*(2*ae_randomreal(_state)-1);
                        xy.ptr.pp_double[i0*k1+i1][1] = i1+0.1*(2*ae_randomreal(_state)-1);
                        for(j=0; j<=ny-1; j++)
                        {
                            xy.ptr.pp_double[i0*k1+i1][nx+j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                }
                ae_vector_set_length(&testpoint, nx, _state);
                testpoint.ptr.p_double[0] = ae_randomreal(_state)*(k0-1);
                testpoint.ptr.p_double[1] = ae_randomreal(_state)*(k1-1);
            }
            else
            {
                
                /*
                 * 3-dimensional problem
                 */
                k0 = 2+ae_randominteger(4, _state);
                k1 = 2+ae_randominteger(4, _state);
                k2 = 2+ae_randominteger(4, _state);
                ae_matrix_set_length(&xy, k0*k1*k2, nx+ny, _state);
                for(i0=0; i0<=k0-1; i0++)
                {
                    for(i1=0; i1<=k1-1; i1++)
                    {
                        for(i2=0; i2<=k2-1; i2++)
                        {
                            xy.ptr.pp_double[i0*k1*k2+i1*k2+i2][0] = i0+0.1*(2*ae_randomreal(_state)-1);
                            xy.ptr.pp_double[i0*k1*k2+i1*k2+i2][1] = i1+0.1*(2*ae_randomreal(_state)-1);
                            xy.ptr.pp_double[i0*k1*k2+i1*k2+i2][2] = i2+0.1*(2*ae_randomreal(_state)-1);
                            for(j=0; j<=ny-1; j++)
                            {
                                xy.ptr.pp_double[i0*k1*k2+i1*k2+i2][nx+j] = 2*ae_randomreal(_state)-1;
                            }
                        }
                    }
                }
                ae_vector_set_length(&testpoint, nx, _state);
                testpoint.ptr.p_double[0] = ae_randomreal(_state)*(k0-1);
                testpoint.ptr.p_double[1] = ae_randomreal(_state)*(k1-1);
                testpoint.ptr.p_double[2] = ae_randomreal(_state)*(k2-1);
            }
            rbfsetpoints(&s, &xy, xy.rows, _state);
            
            /*
             * Build model, serialize, compare
             */
            rbfbuildmodel(&s, &rep, _state);
            {
                /*
                 * This code passes data structure through serializers
                 * (serializes it to string and loads back)
                 */
                ae_serializer _local_serializer;
                ae_int_t _local_ssize;
                ae_frame _local_frame_block;
                ae_dyn_block _local_dynamic_block;
                
                ae_frame_make(_state, &_local_frame_block);
                
                ae_serializer_init(&_local_serializer);
                ae_serializer_alloc_start(&_local_serializer);
                rbfalloc(&_local_serializer, &s, _state);
                _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                rbfserialize(&_local_serializer, &s, _state);
                ae_serializer_stop(&_local_serializer);
                ae_serializer_clear(&_local_serializer);
                
                ae_serializer_init(&_local_serializer);
                ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                rbfunserialize(&_local_serializer, &s2, _state);
                ae_serializer_stop(&_local_serializer);
                ae_serializer_clear(&_local_serializer);
                
                ae_frame_leave(_state);
            }
            rbfcalc(&s, &testpoint, &y0, _state);
            rbfcalc(&s2, &testpoint, &y1, _state);
            if( y0.cnt!=ny||y1.cnt!=ny )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(j=0; j<=ny-1; j++)
            {
                if( ae_fp_neq(y0.ptr.p_double[j],y1.ptr.p_double[j]) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
            
            /*
             * Check that calling RBFBuildModel() on S2 (new model)
             * will result in construction of zero model, i.e. test
             * that serialization restores model, but not dataset
             * which was used to build model.
             */
            rbfbuildmodel(&s2, &rep, _state);
            rbfcalc(&s2, &testpoint, &y1, _state);
            if( y1.cnt!=ny )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(j=0; j<=ny-1; j++)
            {
                if( ae_fp_neq(y1.ptr.p_double[j],(double)(0)) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
    }
    
    /*
     * This function generates random 2 or 3 dimensional problem,
     * builds model using RBF-NN algo, serializes/unserializes it,
     * then compares models by calculating model value at some
     * random point.
     *
     * Additionally we test that new model (one which was restored
     * after serialization) has lost all model construction settings,
     * i.e. if we call RBFBuildModel() on a NEW model, we will get
     * empty (zero) model.
     */
    for(nx=2; nx<=3; nx++)
    {
        for(ny=1; ny<=2; ny++)
        {
            
            /*
             * prepare test problem
             */
            rbfcreate(nx, ny, &s, _state);
            rbfsetalgomultilayer(&s, 5.0, 5, 1.0E-3, _state);
            rbfsetlinterm(&s, _state);
            if( nx==2 )
            {
                
                /*
                 * 2-dimensional problem
                 */
                k0 = 2+ae_randominteger(4, _state);
                k1 = 2+ae_randominteger(4, _state);
                ae_matrix_set_length(&xy, k0*k1, nx+ny, _state);
                for(i0=0; i0<=k0-1; i0++)
                {
                    for(i1=0; i1<=k1-1; i1++)
                    {
                        xy.ptr.pp_double[i0*k1+i1][0] = i0+0.1*(2*ae_randomreal(_state)-1);
                        xy.ptr.pp_double[i0*k1+i1][1] = i1+0.1*(2*ae_randomreal(_state)-1);
                        for(j=0; j<=ny-1; j++)
                        {
                            xy.ptr.pp_double[i0*k1+i1][nx+j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                }
                ae_vector_set_length(&testpoint, nx, _state);
                testpoint.ptr.p_double[0] = ae_randomreal(_state)*(k0-1);
                testpoint.ptr.p_double[1] = ae_randomreal(_state)*(k1-1);
            }
            else
            {
                
                /*
                 * 3-dimensional problem
                 */
                k0 = 2+ae_randominteger(4, _state);
                k1 = 2+ae_randominteger(4, _state);
                k2 = 2+ae_randominteger(4, _state);
                ae_matrix_set_length(&xy, k0*k1*k2, nx+ny, _state);
                for(i0=0; i0<=k0-1; i0++)
                {
                    for(i1=0; i1<=k1-1; i1++)
                    {
                        for(i2=0; i2<=k2-1; i2++)
                        {
                            xy.ptr.pp_double[i0*k1*k2+i1*k2+i2][0] = i0+0.1*(2*ae_randomreal(_state)-1);
                            xy.ptr.pp_double[i0*k1*k2+i1*k2+i2][1] = i1+0.1*(2*ae_randomreal(_state)-1);
                            xy.ptr.pp_double[i0*k1*k2+i1*k2+i2][2] = i2+0.1*(2*ae_randomreal(_state)-1);
                            for(j=0; j<=ny-1; j++)
                            {
                                xy.ptr.pp_double[i0*k1*k2+i1*k2+i2][nx+j] = 2*ae_randomreal(_state)-1;
                            }
                        }
                    }
                }
                ae_vector_set_length(&testpoint, nx, _state);
                testpoint.ptr.p_double[0] = ae_randomreal(_state)*(k0-1);
                testpoint.ptr.p_double[1] = ae_randomreal(_state)*(k1-1);
                testpoint.ptr.p_double[2] = ae_randomreal(_state)*(k2-1);
            }
            rbfsetpoints(&s, &xy, xy.rows, _state);
            
            /*
             * Build model, serialize, compare
             */
            rbfbuildmodel(&s, &rep, _state);
            {
                /*
                 * This code passes data structure through serializers
                 * (serializes it to string and loads back)
                 */
                ae_serializer _local_serializer;
                ae_int_t _local_ssize;
                ae_frame _local_frame_block;
                ae_dyn_block _local_dynamic_block;
                
                ae_frame_make(_state, &_local_frame_block);
                
                ae_serializer_init(&_local_serializer);
                ae_serializer_alloc_start(&_local_serializer);
                rbfalloc(&_local_serializer, &s, _state);
                _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                rbfserialize(&_local_serializer, &s, _state);
                ae_serializer_stop(&_local_serializer);
                ae_serializer_clear(&_local_serializer);
                
                ae_serializer_init(&_local_serializer);
                ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                rbfunserialize(&_local_serializer, &s2, _state);
                ae_serializer_stop(&_local_serializer);
                ae_serializer_clear(&_local_serializer);
                
                ae_frame_leave(_state);
            }
            rbfcalc(&s, &testpoint, &y0, _state);
            rbfcalc(&s2, &testpoint, &y1, _state);
            if( y0.cnt!=ny||y1.cnt!=ny )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(j=0; j<=ny-1; j++)
            {
                if( ae_fp_neq(y0.ptr.p_double[j],y1.ptr.p_double[j]) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
            
            /*
             * Check that calling RBFBuildModel() on S2 (new model)
             * will result in construction of zero model, i.e. test
             * that serialization restores model, but not dataset
             * which was used to build model.
             */
            rbfbuildmodel(&s2, &rep, _state);
            rbfcalc(&s2, &testpoint, &y1, _state);
            if( y1.cnt!=ny )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(j=0; j<=ny-1; j++)
            {
                if( ae_fp_neq(y1.ptr.p_double[j],(double)(0)) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


static ae_bool testrbfunit_searcherr(/* Real    */ ae_matrix* y0,
     /* Real    */ ae_matrix* y1,
     ae_int_t n,
     ae_int_t ny,
     ae_int_t errtype,
     /* Real    */ ae_vector* b1,
     /* Real    */ ae_vector* delta,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _y0;
    ae_matrix _y1;
    ae_vector _b1;
    ae_vector _delta;
    double oralerr;
    double iralerr;
    ae_vector irerr;
    ae_vector orerr;
    ae_int_t lb;
    ae_int_t rb;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_y0, y0, _state);
    y0 = &_y0;
    ae_matrix_init_copy(&_y1, y1, _state);
    y1 = &_y1;
    ae_vector_init_copy(&_b1, b1, _state);
    b1 = &_b1;
    ae_vector_init_copy(&_delta, delta, _state);
    delta = &_delta;
    ae_vector_init(&irerr, 0, DT_REAL, _state);
    ae_vector_init(&orerr, 0, DT_REAL, _state);

    ae_assert(n>0, "SearchErr: invalid parameter N(N<=0).", _state);
    ae_assert(ny>0, "SearchErr: invalid parameter NY(NY<=0).", _state);
    oralerr = 1.0E-1;
    iralerr = 1.0E-2;
    lb = 25;
    rb = 75;
    ae_vector_set_length(&orerr, ny, _state);
    ae_vector_set_length(&irerr, ny, _state);
    for(j=0; j<=ny-1; j++)
    {
        orerr.ptr.p_double[j] = (double)(0);
        irerr.ptr.p_double[j] = (double)(0);
    }
    if( errtype==1 )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=ny-1; j++)
            {
                if( ae_fp_less(orerr.ptr.p_double[j],ae_fabs(y0->ptr.pp_double[i][j]-y1->ptr.pp_double[i][j], _state)) )
                {
                    orerr.ptr.p_double[j] = ae_fabs(y0->ptr.pp_double[i][j]-y1->ptr.pp_double[i][j], _state);
                }
            }
        }
        for(i=0; i<=ny-1; i++)
        {
            if( ae_fp_greater(orerr.ptr.p_double[i],b1->ptr.p_double[i]+delta->ptr.p_double[i])||ae_fp_less(orerr.ptr.p_double[i],b1->ptr.p_double[i]-delta->ptr.p_double[i]) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
    }
    else
    {
        if( errtype==2 )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=ny-1; j++)
                {
                    if( i>lb&&i<rb )
                    {
                        if( ae_fp_less(irerr.ptr.p_double[j],ae_fabs(y0->ptr.pp_double[i][j]-y1->ptr.pp_double[i][j], _state)) )
                        {
                            irerr.ptr.p_double[j] = ae_fabs(y0->ptr.pp_double[i][j]-y1->ptr.pp_double[i][j], _state);
                        }
                    }
                    else
                    {
                        if( ae_fp_less(orerr.ptr.p_double[j],ae_fabs(y0->ptr.pp_double[i][j]-y1->ptr.pp_double[i][j], _state)) )
                        {
                            orerr.ptr.p_double[j] = ae_fabs(y0->ptr.pp_double[i][j]-y1->ptr.pp_double[i][j], _state);
                        }
                    }
                }
            }
            for(i=0; i<=ny-1; i++)
            {
                if( ae_fp_greater(orerr.ptr.p_double[i],oralerr)||ae_fp_greater(irerr.ptr.p_double[i],iralerr) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
        else
        {
            ae_assert(ae_false, "SearchErr: invalid argument ErrType(ErrType neither 1 nor 2)", _state);
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing basic functionality of RBF module on regular grids with
multi-layer algorithm in 2-3D.

  -- ALGLIB --
     Copyright 2.03.2012 by Bochkanov Sergey
*************************************************************************/
static ae_bool testrbfunit_basicmultilayerrbftest(ae_state *_state)
{
    ae_frame _frame_block;
    rbfmodel s;
    rbfreport rep;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t k0;
    ae_int_t k1;
    ae_int_t k2;
    ae_int_t linterm;
    ae_int_t np;
    double q;
    ae_int_t layers;
    ae_vector epss;
    ae_int_t range;
    double s1;
    double s2;
    double gstep;
    ae_vector point;
    ae_matrix gp;
    ae_vector x;
    ae_vector y;
    ae_matrix gy;
    ae_vector gpgx0;
    ae_vector gpgx1;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t l;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _rbfmodel_init(&s, _state);
    _rbfreport_init(&rep, _state);
    ae_vector_init(&epss, 0, DT_REAL, _state);
    ae_vector_init(&point, 0, DT_REAL, _state);
    ae_matrix_init(&gp, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_matrix_init(&gy, 0, 0, DT_REAL, _state);
    ae_vector_init(&gpgx0, 0, DT_REAL, _state);
    ae_vector_init(&gpgx1, 0, DT_REAL, _state);

    range = 10;
    k0 = 6;
    k1 = 6;
    k2 = 6;
    passcount = 10;
    ae_vector_set_length(&epss, 2, _state);
    epss.ptr.p_double[0] = 0.05;
    epss.ptr.p_double[1] = 1.0E-6;
    for(pass=0; pass<=passcount-1; pass++)
    {
        
        /*
         * prepare test problem
         */
        s1 = ae_pow((double)(range), (double)(ae_randominteger(3, _state)-1), _state);
        s2 = ae_pow((double)(range), (double)(ae_randominteger(3, _state)-1), _state);
        nx = ae_randominteger(2, _state)+2;
        ny = ae_randominteger(2, _state)+1;
        linterm = ae_randominteger(3, _state)+1;
        layers = ae_randominteger(2, _state);
        gstep = s1/6;
        ae_vector_set_length(&x, nx, _state);
        ae_vector_set_length(&y, ny, _state);
        ae_vector_set_length(&point, nx, _state);
        rbfcreate(nx, ny, &s, _state);
        q = s1;
        rbfsetalgomultilayer(&s, q, layers+5, 0.0, _state);
        if( linterm==1 )
        {
            rbfsetlinterm(&s, _state);
        }
        if( linterm==2 )
        {
            rbfsetconstterm(&s, _state);
        }
        if( linterm==3 )
        {
            rbfsetzeroterm(&s, _state);
        }
        
        /*
         * start points for grid
         */
        for(i=0; i<=nx-1; i++)
        {
            point.ptr.p_double[i] = s1*(2*ae_randomreal(_state)-1);
        }
        
        /*
         * 2-dimensional test problem
         */
        if( nx==2 )
        {
            np = k0*k1;
            ae_matrix_set_length(&gp, np, nx+ny, _state);
            
            /*
             * create grid
             */
            for(i=0; i<=k0-1; i++)
            {
                for(j=0; j<=k1-1; j++)
                {
                    gp.ptr.pp_double[i*k1+j][0] = point.ptr.p_double[0]+gstep*i;
                    gp.ptr.pp_double[i*k1+j][1] = point.ptr.p_double[1]+gstep*j;
                    for(k=0; k<=ny-1; k++)
                    {
                        gp.ptr.pp_double[i*k1+j][nx+k] = s2*(2*ae_randomreal(_state)-1);
                    }
                }
            }
            rbfsetpoints(&s, &gp, np, _state);
            rbfbuildmodel(&s, &rep, _state);
            if( ny==1 )
            {
                ae_vector_set_length(&gpgx0, k0, _state);
                ae_vector_set_length(&gpgx1, k1, _state);
                for(i=0; i<=k0-1; i++)
                {
                    gpgx0.ptr.p_double[i] = point.ptr.p_double[0]+gstep*i;
                }
                for(i=0; i<=k1-1; i++)
                {
                    gpgx1.ptr.p_double[i] = point.ptr.p_double[1]+gstep*i;
                }
                rbfgridcalc2(&s, &gpgx0, k0, &gpgx1, k1, &gy, _state);
                for(i=0; i<=k0-1; i++)
                {
                    for(j=0; j<=k1-1; j++)
                    {
                        if( ae_fp_greater(ae_fabs(gy.ptr.pp_double[i][j]-gp.ptr.pp_double[i*k1+j][nx], _state),s2*epss.ptr.p_double[layers]) )
                        {
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                    }
                }
            }
            for(i=0; i<=np-1; i++)
            {
                x.ptr.p_double[0] = gp.ptr.pp_double[i][0];
                x.ptr.p_double[1] = gp.ptr.pp_double[i][1];
                if( ny==1 )
                {
                    y.ptr.p_double[0] = rbfcalc2(&s, x.ptr.p_double[0], x.ptr.p_double[1], _state);
                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx]-y.ptr.p_double[0], _state),s2*epss.ptr.p_double[layers]) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
                rbfcalc(&s, &x, &y, _state);
                for(j=0; j<=ny-1; j++)
                {
                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),s2*epss.ptr.p_double[layers]) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
                rbfcalcbuf(&s, &x, &y, _state);
                for(j=0; j<=ny-1; j++)
                {
                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),s2*epss.ptr.p_double[layers]) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
        }
        
        /*
         * 3-dimensional test problems
         */
        if( nx==3 )
        {
            np = k0*k1*k2;
            ae_matrix_set_length(&gp, np, nx+ny, _state);
            
            /*
             * create grid
             */
            for(i=0; i<=k0-1; i++)
            {
                for(j=0; j<=k1-1; j++)
                {
                    for(k=0; k<=k2-1; k++)
                    {
                        gp.ptr.pp_double[(i*k1+j)*k2+k][0] = point.ptr.p_double[0]+gstep*i;
                        gp.ptr.pp_double[(i*k1+j)*k2+k][1] = point.ptr.p_double[1]+gstep*j;
                        gp.ptr.pp_double[(i*k1+j)*k2+k][2] = point.ptr.p_double[2]+gstep*k;
                        for(l=0; l<=ny-1; l++)
                        {
                            gp.ptr.pp_double[(i*k1+j)*k2+k][nx+l] = s2*(2*ae_randomreal(_state)-1);
                        }
                    }
                }
            }
            rbfsetpoints(&s, &gp, np, _state);
            rbfbuildmodel(&s, &rep, _state);
            for(i=0; i<=np-1; i++)
            {
                x.ptr.p_double[0] = gp.ptr.pp_double[i][0];
                x.ptr.p_double[1] = gp.ptr.pp_double[i][1];
                x.ptr.p_double[2] = gp.ptr.pp_double[i][2];
                if( ny==1 )
                {
                    y.ptr.p_double[0] = rbfcalc3(&s, x.ptr.p_double[0], x.ptr.p_double[1], x.ptr.p_double[2], _state);
                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx]-y.ptr.p_double[0], _state),s2*epss.ptr.p_double[layers]) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
                rbfcalc(&s, &x, &y, _state);
                for(j=0; j<=ny-1; j++)
                {
                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),s2*epss.ptr.p_double[layers]) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
                rbfcalcbuf(&s, &x, &y, _state);
                for(j=0; j<=ny-1; j++)
                {
                    if( ae_fp_greater(ae_fabs(gp.ptr.pp_double[i][nx+j]-y.ptr.p_double[j], _state),s2*epss.ptr.p_double[layers]) )
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
