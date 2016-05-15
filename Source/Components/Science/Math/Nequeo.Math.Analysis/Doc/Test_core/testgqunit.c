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
#include "testgqunit.h"


/*$ Declarations $*/
static double testgqunit_mapkind(ae_int_t k, ae_state *_state);
static void testgqunit_buildgausslegendrequadrature(ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state);
static void testgqunit_buildgaussjacobiquadrature(ae_int_t n,
     double alpha,
     double beta,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state);
static void testgqunit_buildgausslaguerrequadrature(ae_int_t n,
     double alpha,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state);
static void testgqunit_buildgausshermitequadrature(ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Test
*************************************************************************/
ae_bool testgq(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector alpha;
    ae_vector beta;
    ae_vector x;
    ae_vector w;
    ae_vector x2;
    ae_vector w2;
    double err;
    ae_int_t n;
    ae_int_t i;
    ae_int_t info;
    ae_int_t akind;
    ae_int_t bkind;
    double alphac;
    double betac;
    double errtol;
    double nonstricterrtol;
    double stricterrtol;
    ae_bool recerrors;
    ae_bool specerrors;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&alpha, 0, DT_REAL, _state);
    ae_vector_init(&beta, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);

    recerrors = ae_false;
    specerrors = ae_false;
    waserrors = ae_false;
    errtol = 1.0E-12;
    nonstricterrtol = 1.0E-6;
    stricterrtol = 1000*ae_machineepsilon;
    
    /*
     * Three tests for rec-based Gauss quadratures with known weights/nodes:
     * 1. Gauss-Legendre with N=2
     * 2. Gauss-Legendre with N=5
     * 3. Gauss-Chebyshev with N=1, 2, 4, 8, ..., 512
     */
    err = (double)(0);
    ae_vector_set_length(&alpha, 2, _state);
    ae_vector_set_length(&beta, 2, _state);
    alpha.ptr.p_double[0] = (double)(0);
    alpha.ptr.p_double[1] = (double)(0);
    beta.ptr.p_double[1] = (double)1/(double)(4*1*1-1);
    gqgeneraterec(&alpha, &beta, 2.0, 2, &info, &x, &w, _state);
    if( info>0 )
    {
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[0]+ae_sqrt((double)(3), _state)/3, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[1]-ae_sqrt((double)(3), _state)/3, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[0]-1, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[1]-1, _state), _state);
        for(i=0; i<=0; i++)
        {
            recerrors = recerrors||ae_fp_greater_eq(x.ptr.p_double[i],x.ptr.p_double[i+1]);
        }
    }
    else
    {
        recerrors = ae_true;
    }
    ae_vector_set_length(&alpha, 5, _state);
    ae_vector_set_length(&beta, 5, _state);
    alpha.ptr.p_double[0] = (double)(0);
    for(i=1; i<=4; i++)
    {
        alpha.ptr.p_double[i] = (double)(0);
        beta.ptr.p_double[i] = ae_sqr((double)(i), _state)/(4*ae_sqr((double)(i), _state)-1);
    }
    gqgeneraterec(&alpha, &beta, 2.0, 5, &info, &x, &w, _state);
    if( info>0 )
    {
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[0]+ae_sqrt(245+14*ae_sqrt((double)(70), _state), _state)/21, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[0]+x.ptr.p_double[4], _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[1]+ae_sqrt(245-14*ae_sqrt((double)(70), _state), _state)/21, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[1]+x.ptr.p_double[3], _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[2], _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[0]-(322-13*ae_sqrt((double)(70), _state))/900, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[0]-w.ptr.p_double[4], _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[1]-(322+13*ae_sqrt((double)(70), _state))/900, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[1]-w.ptr.p_double[3], _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[2]-(double)128/(double)225, _state), _state);
        for(i=0; i<=3; i++)
        {
            recerrors = recerrors||ae_fp_greater_eq(x.ptr.p_double[i],x.ptr.p_double[i+1]);
        }
    }
    else
    {
        recerrors = ae_true;
    }
    n = 1;
    while(n<=512)
    {
        ae_vector_set_length(&alpha, n, _state);
        ae_vector_set_length(&beta, n, _state);
        for(i=0; i<=n-1; i++)
        {
            alpha.ptr.p_double[i] = (double)(0);
            if( i==0 )
            {
                beta.ptr.p_double[i] = (double)(0);
            }
            if( i==1 )
            {
                beta.ptr.p_double[i] = (double)1/(double)2;
            }
            if( i>1 )
            {
                beta.ptr.p_double[i] = (double)1/(double)4;
            }
        }
        gqgeneraterec(&alpha, &beta, ae_pi, n, &info, &x, &w, _state);
        if( info>0 )
        {
            for(i=0; i<=n-1; i++)
            {
                err = ae_maxreal(err, ae_fabs(x.ptr.p_double[i]-ae_cos(ae_pi*(n-i-0.5)/n, _state), _state), _state);
                err = ae_maxreal(err, ae_fabs(w.ptr.p_double[i]-ae_pi/n, _state), _state);
            }
            for(i=0; i<=n-2; i++)
            {
                recerrors = recerrors||ae_fp_greater_eq(x.ptr.p_double[i],x.ptr.p_double[i+1]);
            }
        }
        else
        {
            recerrors = ae_true;
        }
        n = n*2;
    }
    recerrors = recerrors||ae_fp_greater(err,errtol);
    
    /*
     * Three tests for rec-based Gauss-Lobatto quadratures with known weights/nodes:
     * 1. Gauss-Lobatto with N=3
     * 2. Gauss-Lobatto with N=4
     * 3. Gauss-Lobatto with N=6
     */
    err = (double)(0);
    ae_vector_set_length(&alpha, 2, _state);
    ae_vector_set_length(&beta, 2, _state);
    alpha.ptr.p_double[0] = (double)(0);
    alpha.ptr.p_double[1] = (double)(0);
    beta.ptr.p_double[0] = (double)(0);
    beta.ptr.p_double[1] = (double)(1*1)/(double)(4*1*1-1);
    gqgenerategausslobattorec(&alpha, &beta, 2.0, (double)(-1), (double)(1), 3, &info, &x, &w, _state);
    if( info>0 )
    {
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[0]+1, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[1], _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[2]-1, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[0]-(double)1/(double)3, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[1]-(double)4/(double)3, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[2]-(double)1/(double)3, _state), _state);
        for(i=0; i<=1; i++)
        {
            recerrors = recerrors||ae_fp_greater_eq(x.ptr.p_double[i],x.ptr.p_double[i+1]);
        }
    }
    else
    {
        recerrors = ae_true;
    }
    ae_vector_set_length(&alpha, 3, _state);
    ae_vector_set_length(&beta, 3, _state);
    alpha.ptr.p_double[0] = (double)(0);
    alpha.ptr.p_double[1] = (double)(0);
    alpha.ptr.p_double[2] = (double)(0);
    beta.ptr.p_double[0] = (double)(0);
    beta.ptr.p_double[1] = (double)(1*1)/(double)(4*1*1-1);
    beta.ptr.p_double[2] = (double)(2*2)/(double)(4*2*2-1);
    gqgenerategausslobattorec(&alpha, &beta, 2.0, (double)(-1), (double)(1), 4, &info, &x, &w, _state);
    if( info>0 )
    {
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[0]+1, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[1]+ae_sqrt((double)(5), _state)/5, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[2]-ae_sqrt((double)(5), _state)/5, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[3]-1, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[0]-(double)1/(double)6, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[1]-(double)5/(double)6, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[2]-(double)5/(double)6, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[3]-(double)1/(double)6, _state), _state);
        for(i=0; i<=2; i++)
        {
            recerrors = recerrors||ae_fp_greater_eq(x.ptr.p_double[i],x.ptr.p_double[i+1]);
        }
    }
    else
    {
        recerrors = ae_true;
    }
    ae_vector_set_length(&alpha, 5, _state);
    ae_vector_set_length(&beta, 5, _state);
    alpha.ptr.p_double[0] = (double)(0);
    alpha.ptr.p_double[1] = (double)(0);
    alpha.ptr.p_double[2] = (double)(0);
    alpha.ptr.p_double[3] = (double)(0);
    alpha.ptr.p_double[4] = (double)(0);
    beta.ptr.p_double[0] = (double)(0);
    beta.ptr.p_double[1] = (double)(1*1)/(double)(4*1*1-1);
    beta.ptr.p_double[2] = (double)(2*2)/(double)(4*2*2-1);
    beta.ptr.p_double[3] = (double)(3*3)/(double)(4*3*3-1);
    beta.ptr.p_double[4] = (double)(4*4)/(double)(4*4*4-1);
    gqgenerategausslobattorec(&alpha, &beta, 2.0, (double)(-1), (double)(1), 6, &info, &x, &w, _state);
    if( info>0 )
    {
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[0]+1, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[1]+ae_sqrt((7+2*ae_sqrt((double)(7), _state))/21, _state), _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[2]+ae_sqrt((7-2*ae_sqrt((double)(7), _state))/21, _state), _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[3]-ae_sqrt((7-2*ae_sqrt((double)(7), _state))/21, _state), _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[4]-ae_sqrt((7+2*ae_sqrt((double)(7), _state))/21, _state), _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[5]-1, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[0]-(double)1/(double)15, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[1]-(14-ae_sqrt((double)(7), _state))/30, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[2]-(14+ae_sqrt((double)(7), _state))/30, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[3]-(14+ae_sqrt((double)(7), _state))/30, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[4]-(14-ae_sqrt((double)(7), _state))/30, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[5]-(double)1/(double)15, _state), _state);
        for(i=0; i<=4; i++)
        {
            recerrors = recerrors||ae_fp_greater_eq(x.ptr.p_double[i],x.ptr.p_double[i+1]);
        }
    }
    else
    {
        recerrors = ae_true;
    }
    recerrors = recerrors||ae_fp_greater(err,errtol);
    
    /*
     * Three tests for rec-based Gauss-Radau quadratures with known weights/nodes:
     * 1. Gauss-Radau with N=2
     * 2. Gauss-Radau with N=3
     * 3. Gauss-Radau with N=3 (another case)
     */
    err = (double)(0);
    ae_vector_set_length(&alpha, 1, _state);
    ae_vector_set_length(&beta, 2, _state);
    alpha.ptr.p_double[0] = (double)(0);
    beta.ptr.p_double[0] = (double)(0);
    beta.ptr.p_double[1] = (double)(1*1)/(double)(4*1*1-1);
    gqgenerategaussradaurec(&alpha, &beta, 2.0, (double)(-1), 2, &info, &x, &w, _state);
    if( info>0 )
    {
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[0]+1, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[1]-(double)1/(double)3, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[0]-0.5, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[1]-1.5, _state), _state);
        for(i=0; i<=0; i++)
        {
            recerrors = recerrors||ae_fp_greater_eq(x.ptr.p_double[i],x.ptr.p_double[i+1]);
        }
    }
    else
    {
        recerrors = ae_true;
    }
    ae_vector_set_length(&alpha, 2, _state);
    ae_vector_set_length(&beta, 3, _state);
    alpha.ptr.p_double[0] = (double)(0);
    alpha.ptr.p_double[1] = (double)(0);
    for(i=0; i<=2; i++)
    {
        beta.ptr.p_double[i] = ae_sqr((double)(i), _state)/(4*ae_sqr((double)(i), _state)-1);
    }
    gqgenerategaussradaurec(&alpha, &beta, 2.0, (double)(-1), 3, &info, &x, &w, _state);
    if( info>0 )
    {
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[0]+1, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[1]-(1-ae_sqrt((double)(6), _state))/5, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[2]-(1+ae_sqrt((double)(6), _state))/5, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[0]-(double)2/(double)9, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[1]-(16+ae_sqrt((double)(6), _state))/18, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[2]-(16-ae_sqrt((double)(6), _state))/18, _state), _state);
        for(i=0; i<=1; i++)
        {
            recerrors = recerrors||ae_fp_greater_eq(x.ptr.p_double[i],x.ptr.p_double[i+1]);
        }
    }
    else
    {
        recerrors = ae_true;
    }
    ae_vector_set_length(&alpha, 2, _state);
    ae_vector_set_length(&beta, 3, _state);
    alpha.ptr.p_double[0] = (double)(0);
    alpha.ptr.p_double[1] = (double)(0);
    for(i=0; i<=2; i++)
    {
        beta.ptr.p_double[i] = ae_sqr((double)(i), _state)/(4*ae_sqr((double)(i), _state)-1);
    }
    gqgenerategaussradaurec(&alpha, &beta, 2.0, (double)(1), 3, &info, &x, &w, _state);
    if( info>0 )
    {
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[2]-1, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[1]+(1-ae_sqrt((double)(6), _state))/5, _state), _state);
        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[0]+(1+ae_sqrt((double)(6), _state))/5, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[2]-(double)2/(double)9, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[1]-(16+ae_sqrt((double)(6), _state))/18, _state), _state);
        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[0]-(16-ae_sqrt((double)(6), _state))/18, _state), _state);
        for(i=0; i<=1; i++)
        {
            recerrors = recerrors||ae_fp_greater_eq(x.ptr.p_double[i],x.ptr.p_double[i+1]);
        }
    }
    else
    {
        recerrors = ae_true;
    }
    recerrors = recerrors||ae_fp_greater(err,errtol);
    
    /*
     * test recurrence-based special cases (Legendre, Jacobi, Hermite, ...)
     * against another implementation (polynomial root-finder)
     */
    for(n=1; n<=20; n++)
    {
        
        /*
         * test gauss-legendre
         */
        err = (double)(0);
        gqgenerategausslegendre(n, &info, &x, &w, _state);
        if( info>0 )
        {
            testgqunit_buildgausslegendrequadrature(n, &x2, &w2, _state);
            for(i=0; i<=n-1; i++)
            {
                err = ae_maxreal(err, ae_fabs(x.ptr.p_double[i]-x2.ptr.p_double[i], _state), _state);
                err = ae_maxreal(err, ae_fabs(w.ptr.p_double[i]-w2.ptr.p_double[i], _state), _state);
            }
        }
        else
        {
            specerrors = ae_true;
        }
        specerrors = specerrors||ae_fp_greater(err,errtol);
        
        /*
         * Test Gauss-Jacobi.
         * Since task is much more difficult we will use less strict
         * threshold.
         */
        err = (double)(0);
        for(akind=0; akind<=9; akind++)
        {
            for(bkind=0; bkind<=9; bkind++)
            {
                alphac = testgqunit_mapkind(akind, _state);
                betac = testgqunit_mapkind(bkind, _state);
                gqgenerategaussjacobi(n, alphac, betac, &info, &x, &w, _state);
                if( info>0 )
                {
                    testgqunit_buildgaussjacobiquadrature(n, alphac, betac, &x2, &w2, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        err = ae_maxreal(err, ae_fabs(x.ptr.p_double[i]-x2.ptr.p_double[i], _state), _state);
                        err = ae_maxreal(err, ae_fabs(w.ptr.p_double[i]-w2.ptr.p_double[i], _state), _state);
                    }
                }
                else
                {
                    specerrors = ae_true;
                }
            }
        }
        specerrors = specerrors||ae_fp_greater(err,nonstricterrtol);
        
        /*
         * special test for Gauss-Jacobi (Chebyshev weight
         * function with analytically known nodes/weights)
         */
        err = (double)(0);
        gqgenerategaussjacobi(n, -0.5, -0.5, &info, &x, &w, _state);
        if( info>0 )
        {
            for(i=0; i<=n-1; i++)
            {
                err = ae_maxreal(err, ae_fabs(x.ptr.p_double[i]+ae_cos(ae_pi*(i+0.5)/n, _state), _state), _state);
                err = ae_maxreal(err, ae_fabs(w.ptr.p_double[i]-ae_pi/n, _state), _state);
            }
        }
        else
        {
            specerrors = ae_true;
        }
        specerrors = specerrors||ae_fp_greater(err,stricterrtol);
        
        /*
         * Test Gauss-Laguerre
         */
        err = (double)(0);
        for(akind=0; akind<=9; akind++)
        {
            alphac = testgqunit_mapkind(akind, _state);
            gqgenerategausslaguerre(n, alphac, &info, &x, &w, _state);
            if( info>0 )
            {
                testgqunit_buildgausslaguerrequadrature(n, alphac, &x2, &w2, _state);
                for(i=0; i<=n-1; i++)
                {
                    err = ae_maxreal(err, ae_fabs(x.ptr.p_double[i]-x2.ptr.p_double[i], _state), _state);
                    err = ae_maxreal(err, ae_fabs(w.ptr.p_double[i]-w2.ptr.p_double[i], _state), _state);
                }
            }
            else
            {
                specerrors = ae_true;
            }
        }
        specerrors = specerrors||ae_fp_greater(err,nonstricterrtol);
        
        /*
         * Test Gauss-Hermite
         */
        err = (double)(0);
        gqgenerategausshermite(n, &info, &x, &w, _state);
        if( info>0 )
        {
            testgqunit_buildgausshermitequadrature(n, &x2, &w2, _state);
            for(i=0; i<=n-1; i++)
            {
                err = ae_maxreal(err, ae_fabs(x.ptr.p_double[i]-x2.ptr.p_double[i], _state), _state);
                err = ae_maxreal(err, ae_fabs(w.ptr.p_double[i]-w2.ptr.p_double[i], _state), _state);
            }
        }
        else
        {
            specerrors = ae_true;
        }
        specerrors = specerrors||ae_fp_greater(err,nonstricterrtol);
    }
    
    /*
     * end
     */
    waserrors = recerrors||specerrors;
    if( !silent )
    {
        printf("TESTING GAUSS QUADRATURES\n");
        printf("FINAL RESULT:                             ");
        if( waserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* SPECIAL CASES (LEGENDRE/JACOBI/..)      ");
        if( specerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* RECURRENCE-BASED:                       ");
        if( recerrors )
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
ae_bool _pexec_testgq(ae_bool silent, ae_state *_state)
{
    return testgq(silent, _state);
}


/*************************************************************************
Maps:
    0   =>  -0.9
    1   =>  -0.5
    2   =>  -0.1
    3   =>   0.0
    4   =>  +0.1
    5   =>  +0.5
    6   =>  +0.9
    7   =>  +1.0
    8   =>  +1.5
    9   =>  +2.0
*************************************************************************/
static double testgqunit_mapkind(ae_int_t k, ae_state *_state)
{
    double result;


    result = (double)(0);
    if( k==0 )
    {
        result = -0.9;
    }
    if( k==1 )
    {
        result = -0.5;
    }
    if( k==2 )
    {
        result = -0.1;
    }
    if( k==3 )
    {
        result = 0.0;
    }
    if( k==4 )
    {
        result = 0.1;
    }
    if( k==5 )
    {
        result = 0.5;
    }
    if( k==6 )
    {
        result = 0.9;
    }
    if( k==7 )
    {
        result = 1.0;
    }
    if( k==8 )
    {
        result = 1.5;
    }
    if( k==9 )
    {
        result = 2.0;
    }
    return result;
}


/*************************************************************************
Gauss-Legendre, another variant
*************************************************************************/
static void testgqunit_buildgausslegendrequadrature(ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double r;
    double r1;
    double p1;
    double p2;
    double p3;
    double dp3;
    double tmp;

    ae_vector_clear(x);
    ae_vector_clear(w);

    ae_vector_set_length(x, n-1+1, _state);
    ae_vector_set_length(w, n-1+1, _state);
    for(i=0; i<=(n+1)/2-1; i++)
    {
        r = ae_cos(ae_pi*(4*i+3)/(4*n+2), _state);
        do
        {
            p2 = (double)(0);
            p3 = (double)(1);
            for(j=0; j<=n-1; j++)
            {
                p1 = p2;
                p2 = p3;
                p3 = ((2*j+1)*r*p2-j*p1)/(j+1);
            }
            dp3 = n*(r*p3-p2)/(r*r-1);
            r1 = r;
            r = r-p3/dp3;
        }
        while(ae_fp_greater_eq(ae_fabs(r-r1, _state),ae_machineepsilon*(1+ae_fabs(r, _state))*100));
        x->ptr.p_double[i] = r;
        x->ptr.p_double[n-1-i] = -r;
        w->ptr.p_double[i] = 2/((1-r*r)*dp3*dp3);
        w->ptr.p_double[n-1-i] = 2/((1-r*r)*dp3*dp3);
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-2-i; j++)
        {
            if( ae_fp_greater_eq(x->ptr.p_double[j],x->ptr.p_double[j+1]) )
            {
                tmp = x->ptr.p_double[j];
                x->ptr.p_double[j] = x->ptr.p_double[j+1];
                x->ptr.p_double[j+1] = tmp;
                tmp = w->ptr.p_double[j];
                w->ptr.p_double[j] = w->ptr.p_double[j+1];
                w->ptr.p_double[j+1] = tmp;
            }
        }
    }
}


/*************************************************************************
Gauss-Jacobi, another variant
*************************************************************************/
static void testgqunit_buildgaussjacobiquadrature(ae_int_t n,
     double alpha,
     double beta,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double r;
    double r1;
    double t1;
    double t2;
    double t3;
    double p1;
    double p2;
    double p3;
    double pp;
    double an;
    double bn;
    double a;
    double b;
    double c;
    double tmpsgn;
    double tmp;
    double alfbet;
    double temp;

    ae_vector_clear(x);
    ae_vector_clear(w);

    ae_vector_set_length(x, n-1+1, _state);
    ae_vector_set_length(w, n-1+1, _state);
    r = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        if( i==0 )
        {
            an = alpha/n;
            bn = beta/n;
            t1 = (1+alpha)*(2.78/(4+n*n)+0.768*an/n);
            t2 = 1+1.48*an+0.96*bn+0.452*an*an+0.83*an*bn;
            r = (t2-t1)/t2;
        }
        else
        {
            if( i==1 )
            {
                t1 = (4.1+alpha)/((1+alpha)*(1+0.156*alpha));
                t2 = 1+0.06*(n-8)*(1+0.12*alpha)/n;
                t3 = 1+0.012*beta*(1+0.25*ae_fabs(alpha, _state))/n;
                r = r-t1*t2*t3*(1-r);
            }
            else
            {
                if( i==2 )
                {
                    t1 = (1.67+0.28*alpha)/(1+0.37*alpha);
                    t2 = 1+0.22*(n-8)/n;
                    t3 = 1+8*beta/((6.28+beta)*n*n);
                    r = r-t1*t2*t3*(x->ptr.p_double[0]-r);
                }
                else
                {
                    if( i<n-2 )
                    {
                        r = 3*x->ptr.p_double[i-1]-3*x->ptr.p_double[i-2]+x->ptr.p_double[i-3];
                    }
                    else
                    {
                        if( i==n-2 )
                        {
                            t1 = (1+0.235*beta)/(0.766+0.119*beta);
                            t2 = 1/(1+0.639*(n-4)/(1+0.71*(n-4)));
                            t3 = 1/(1+20*alpha/((7.5+alpha)*n*n));
                            r = r+t1*t2*t3*(r-x->ptr.p_double[i-2]);
                        }
                        else
                        {
                            if( i==n-1 )
                            {
                                t1 = (1+0.37*beta)/(1.67+0.28*beta);
                                t2 = 1/(1+0.22*(n-8)/n);
                                t3 = 1/(1+8*alpha/((6.28+alpha)*n*n));
                                r = r+t1*t2*t3*(r-x->ptr.p_double[i-2]);
                            }
                        }
                    }
                }
            }
        }
        alfbet = alpha+beta;
        do
        {
            temp = 2+alfbet;
            p1 = (alpha-beta+temp*r)*0.5;
            p2 = (double)(1);
            for(j=2; j<=n; j++)
            {
                p3 = p2;
                p2 = p1;
                temp = 2*j+alfbet;
                a = 2*j*(j+alfbet)*(temp-2);
                b = (temp-1)*(alpha*alpha-beta*beta+temp*(temp-2)*r);
                c = 2*(j-1+alpha)*(j-1+beta)*temp;
                p1 = (b*p2-c*p3)/a;
            }
            pp = (n*(alpha-beta-temp*r)*p1+2*(n+alpha)*(n+beta)*p2)/(temp*(1-r*r));
            r1 = r;
            r = r1-p1/pp;
        }
        while(ae_fp_greater_eq(ae_fabs(r-r1, _state),ae_machineepsilon*(1+ae_fabs(r, _state))*100));
        x->ptr.p_double[i] = r;
        w->ptr.p_double[i] = ae_exp(lngamma(alpha+n, &tmpsgn, _state)+lngamma(beta+n, &tmpsgn, _state)-lngamma((double)(n+1), &tmpsgn, _state)-lngamma(n+alfbet+1, &tmpsgn, _state), _state)*temp*ae_pow((double)(2), alfbet, _state)/(pp*p2);
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-2-i; j++)
        {
            if( ae_fp_greater_eq(x->ptr.p_double[j],x->ptr.p_double[j+1]) )
            {
                tmp = x->ptr.p_double[j];
                x->ptr.p_double[j] = x->ptr.p_double[j+1];
                x->ptr.p_double[j+1] = tmp;
                tmp = w->ptr.p_double[j];
                w->ptr.p_double[j] = w->ptr.p_double[j+1];
                w->ptr.p_double[j+1] = tmp;
            }
        }
    }
}


/*************************************************************************
Gauss-Laguerre, another variant
*************************************************************************/
static void testgqunit_buildgausslaguerrequadrature(ae_int_t n,
     double alpha,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double r;
    double r1;
    double p1;
    double p2;
    double p3;
    double dp3;
    double tsg;
    double tmp;

    ae_vector_clear(x);
    ae_vector_clear(w);

    ae_vector_set_length(x, n-1+1, _state);
    ae_vector_set_length(w, n-1+1, _state);
    r = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        if( i==0 )
        {
            r = (1+alpha)*(3+0.92*alpha)/(1+2.4*n+1.8*alpha);
        }
        else
        {
            if( i==1 )
            {
                r = r+(15+6.25*alpha)/(1+0.9*alpha+2.5*n);
            }
            else
            {
                r = r+((1+2.55*(i-1))/(1.9*(i-1))+1.26*(i-1)*alpha/(1+3.5*(i-1)))/(1+0.3*alpha)*(r-x->ptr.p_double[i-2]);
            }
        }
        do
        {
            p2 = (double)(0);
            p3 = (double)(1);
            for(j=0; j<=n-1; j++)
            {
                p1 = p2;
                p2 = p3;
                p3 = ((-r+2*j+alpha+1)*p2-(j+alpha)*p1)/(j+1);
            }
            dp3 = (n*p3-(n+alpha)*p2)/r;
            r1 = r;
            r = r-p3/dp3;
        }
        while(ae_fp_greater_eq(ae_fabs(r-r1, _state),ae_machineepsilon*(1+ae_fabs(r, _state))*100));
        x->ptr.p_double[i] = r;
        w->ptr.p_double[i] = -ae_exp(lngamma(alpha+n, &tsg, _state)-lngamma((double)(n), &tsg, _state), _state)/(dp3*n*p2);
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-2-i; j++)
        {
            if( ae_fp_greater_eq(x->ptr.p_double[j],x->ptr.p_double[j+1]) )
            {
                tmp = x->ptr.p_double[j];
                x->ptr.p_double[j] = x->ptr.p_double[j+1];
                x->ptr.p_double[j+1] = tmp;
                tmp = w->ptr.p_double[j];
                w->ptr.p_double[j] = w->ptr.p_double[j+1];
                w->ptr.p_double[j+1] = tmp;
            }
        }
    }
}


/*************************************************************************
Gauss-Hermite, another variant
*************************************************************************/
static void testgqunit_buildgausshermitequadrature(ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double r;
    double r1;
    double p1;
    double p2;
    double p3;
    double dp3;
    double pipm4;
    double tmp;

    ae_vector_clear(x);
    ae_vector_clear(w);

    ae_vector_set_length(x, n-1+1, _state);
    ae_vector_set_length(w, n-1+1, _state);
    pipm4 = ae_pow(ae_pi, -0.25, _state);
    r = (double)(0);
    for(i=0; i<=(n+1)/2-1; i++)
    {
        if( i==0 )
        {
            r = ae_sqrt((double)(2*n+1), _state)-1.85575*ae_pow((double)(2*n+1), -(double)1/(double)6, _state);
        }
        else
        {
            if( i==1 )
            {
                r = r-1.14*ae_pow((double)(n), 0.426, _state)/r;
            }
            else
            {
                if( i==2 )
                {
                    r = 1.86*r-0.86*x->ptr.p_double[0];
                }
                else
                {
                    if( i==3 )
                    {
                        r = 1.91*r-0.91*x->ptr.p_double[1];
                    }
                    else
                    {
                        r = 2*r-x->ptr.p_double[i-2];
                    }
                }
            }
        }
        do
        {
            p2 = (double)(0);
            p3 = pipm4;
            for(j=0; j<=n-1; j++)
            {
                p1 = p2;
                p2 = p3;
                p3 = p2*r*ae_sqrt((double)2/(double)(j+1), _state)-p1*ae_sqrt((double)j/(double)(j+1), _state);
            }
            dp3 = ae_sqrt((double)(2*j), _state)*p2;
            r1 = r;
            r = r-p3/dp3;
        }
        while(ae_fp_greater_eq(ae_fabs(r-r1, _state),ae_machineepsilon*(1+ae_fabs(r, _state))*100));
        x->ptr.p_double[i] = r;
        w->ptr.p_double[i] = 2/(dp3*dp3);
        x->ptr.p_double[n-1-i] = -x->ptr.p_double[i];
        w->ptr.p_double[n-1-i] = w->ptr.p_double[i];
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-2-i; j++)
        {
            if( ae_fp_greater_eq(x->ptr.p_double[j],x->ptr.p_double[j+1]) )
            {
                tmp = x->ptr.p_double[j];
                x->ptr.p_double[j] = x->ptr.p_double[j+1];
                x->ptr.p_double[j+1] = tmp;
                tmp = w->ptr.p_double[j];
                w->ptr.p_double[j] = w->ptr.p_double[j+1];
                w->ptr.p_double[j+1] = tmp;
            }
        }
    }
}


/*$ End $*/
