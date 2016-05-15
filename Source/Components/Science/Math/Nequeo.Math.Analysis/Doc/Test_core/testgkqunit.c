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
#include "testgkqunit.h"


/*$ Declarations $*/
static double testgkqunit_mapkind(ae_int_t k, ae_state *_state);


/*$ Body $*/


/*************************************************************************
Test
*************************************************************************/
ae_bool testgkq(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pkind;
    double errtol;
    double eps;
    double nonstricterrtol;
    ae_int_t n;
    ae_int_t i;
    ae_int_t k;
    ae_int_t info;
    double err;
    ae_int_t akind;
    ae_int_t bkind;
    double alphac;
    double betac;
    ae_vector x1;
    ae_vector wg1;
    ae_vector wk1;
    ae_vector x2;
    ae_vector wg2;
    ae_vector wk2;
    ae_int_t info1;
    ae_int_t info2;
    ae_bool successatleastonce;
    ae_bool intblerrors;
    ae_bool vstblerrors;
    ae_bool generrors;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&wg1, 0, DT_REAL, _state);
    ae_vector_init(&wk1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&wg2, 0, DT_REAL, _state);
    ae_vector_init(&wk2, 0, DT_REAL, _state);

    intblerrors = ae_false;
    vstblerrors = ae_false;
    generrors = ae_false;
    waserrors = ae_false;
    errtol = 10000*ae_machineepsilon;
    nonstricterrtol = 1000*errtol;
    
    /*
     * test recurrence-based Legendre nodes against the precalculated table
     */
    for(pkind=0; pkind<=5; pkind++)
    {
        n = 0;
        if( pkind==0 )
        {
            n = 15;
        }
        if( pkind==1 )
        {
            n = 21;
        }
        if( pkind==2 )
        {
            n = 31;
        }
        if( pkind==3 )
        {
            n = 41;
        }
        if( pkind==4 )
        {
            n = 51;
        }
        if( pkind==5 )
        {
            n = 61;
        }
        gkqlegendrecalc(n, &info, &x1, &wk1, &wg1, _state);
        gkqlegendretbl(n, &x2, &wk2, &wg2, &eps, _state);
        if( info<=0 )
        {
            generrors = ae_true;
            break;
        }
        for(i=0; i<=n-1; i++)
        {
            vstblerrors = vstblerrors||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-x2.ptr.p_double[i], _state),errtol);
            vstblerrors = vstblerrors||ae_fp_greater(ae_fabs(wk1.ptr.p_double[i]-wk2.ptr.p_double[i], _state),errtol);
            vstblerrors = vstblerrors||ae_fp_greater(ae_fabs(wg1.ptr.p_double[i]-wg2.ptr.p_double[i], _state),errtol);
        }
    }
    
    /*
     * Test recurrence-baced Gauss-Kronrod nodes against Gauss-only nodes
     * calculated with subroutines from GQ unit.
     */
    for(k=1; k<=30; k++)
    {
        n = 2*k+1;
        
        /*
         * Gauss-Legendre
         */
        err = (double)(0);
        gkqgenerategausslegendre(n, &info1, &x1, &wk1, &wg1, _state);
        gqgenerategausslegendre(k, &info2, &x2, &wg2, _state);
        if( info1>0&&info2>0 )
        {
            for(i=0; i<=k-1; i++)
            {
                err = ae_maxreal(err, ae_fabs(x1.ptr.p_double[2*i+1]-x2.ptr.p_double[i], _state), _state);
                err = ae_maxreal(err, ae_fabs(wg1.ptr.p_double[2*i+1]-wg2.ptr.p_double[i], _state), _state);
            }
        }
        else
        {
            generrors = ae_true;
        }
        generrors = generrors||ae_fp_greater(err,errtol);
    }
    for(k=1; k<=15; k++)
    {
        n = 2*k+1;
        
        /*
         * Gauss-Jacobi
         */
        successatleastonce = ae_false;
        err = (double)(0);
        for(akind=0; akind<=9; akind++)
        {
            for(bkind=0; bkind<=9; bkind++)
            {
                alphac = testgkqunit_mapkind(akind, _state);
                betac = testgkqunit_mapkind(bkind, _state);
                gkqgenerategaussjacobi(n, alphac, betac, &info1, &x1, &wk1, &wg1, _state);
                gqgenerategaussjacobi(k, alphac, betac, &info2, &x2, &wg2, _state);
                if( info1>0&&info2>0 )
                {
                    successatleastonce = ae_true;
                    for(i=0; i<=k-1; i++)
                    {
                        err = ae_maxreal(err, ae_fabs(x1.ptr.p_double[2*i+1]-x2.ptr.p_double[i], _state), _state);
                        err = ae_maxreal(err, ae_fabs(wg1.ptr.p_double[2*i+1]-wg2.ptr.p_double[i], _state), _state);
                    }
                }
                else
                {
                    generrors = generrors||info1!=-5;
                }
            }
        }
        generrors = (generrors||ae_fp_greater(err,errtol))||!successatleastonce;
    }
    
    /*
     * end
     */
    waserrors = (intblerrors||vstblerrors)||generrors;
    if( !silent )
    {
        printf("TESTING GAUSS-KRONROD QUADRATURES\n");
        printf("FINAL RESULT:                             ");
        if( waserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* PRE-CALCULATED TABLE:                   ");
        if( intblerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* CALCULATED AGAINST THE TABLE:           ");
        if( vstblerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* GENERAL PROPERTIES:                     ");
        if( generrors )
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
ae_bool _pexec_testgkq(ae_bool silent, ae_state *_state)
{
    return testgkq(silent, _state);
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
static double testgkqunit_mapkind(ae_int_t k, ae_state *_state)
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


/*$ End $*/
