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
#include "testlegendreunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testlegendre(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    double err;
    double sumerr;
    double cerr;
    double threshold;
    ae_int_t n;
    ae_int_t maxn;
    ae_int_t i;
    ae_int_t pass;
    ae_vector c;
    double x;
    double v;
    double t;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&c, 0, DT_REAL, _state);

    err = (double)(0);
    sumerr = (double)(0);
    cerr = (double)(0);
    threshold = 1.0E-9;
    waserrors = ae_false;
    
    /*
     * Testing Legendre polynomials values
     */
    for(n=0; n<=10; n++)
    {
        legendrecoefficients(n, &c, _state);
        for(pass=1; pass<=10; pass++)
        {
            x = 2*ae_randomreal(_state)-1;
            v = legendrecalculate(n, x, _state);
            t = (double)(1);
            for(i=0; i<=n; i++)
            {
                v = v-c.ptr.p_double[i]*t;
                t = t*x;
            }
            err = ae_maxreal(err, ae_fabs(v, _state), _state);
        }
    }
    
    /*
     * Testing Clenshaw summation
     */
    maxn = 20;
    ae_vector_set_length(&c, maxn+1, _state);
    for(pass=1; pass<=10; pass++)
    {
        x = 2*ae_randomreal(_state)-1;
        v = (double)(0);
        for(n=0; n<=maxn; n++)
        {
            c.ptr.p_double[n] = 2*ae_randomreal(_state)-1;
            v = v+legendrecalculate(n, x, _state)*c.ptr.p_double[n];
            sumerr = ae_maxreal(sumerr, ae_fabs(v-legendresum(&c, n, x, _state), _state), _state);
        }
    }
    
    /*
     * Testing coefficients
     */
    legendrecoefficients(0, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-1, _state), _state);
    legendrecoefficients(1, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-1, _state), _state);
    legendrecoefficients(2, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]+(double)1/(double)2, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-(double)3/(double)2, _state), _state);
    legendrecoefficients(3, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]+(double)3/(double)2, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]-(double)5/(double)2, _state), _state);
    legendrecoefficients(4, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-(double)3/(double)8, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]+(double)30/(double)8, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[4]-(double)35/(double)8, _state), _state);
    legendrecoefficients(9, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-(double)315/(double)128, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]+(double)4620/(double)128, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[4]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[5]-(double)18018/(double)128, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[6]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[7]+(double)25740/(double)128, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[8]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[9]-(double)12155/(double)128, _state), _state);
    
    /*
     * Reporting
     */
    waserrors = (ae_fp_greater(err,threshold)||ae_fp_greater(sumerr,threshold))||ae_fp_greater(cerr,threshold);
    if( !silent )
    {
        printf("TESTING CALCULATION OF THE LEGENDRE POLYNOMIALS\n");
        printf("Max error                                 %5.2e\n",
            (double)(err));
        printf("Summation error                           %5.2e\n",
            (double)(sumerr));
        printf("Coefficients error                        %5.2e\n",
            (double)(cerr));
        printf("Threshold                                 %5.2e\n",
            (double)(threshold));
        if( !waserrors )
        {
            printf("TEST PASSED\n");
        }
        else
        {
            printf("TEST FAILED\n");
        }
    }
    result = !waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testlegendre(ae_bool silent, ae_state *_state)
{
    return testlegendre(silent, _state);
}


/*$ End $*/
