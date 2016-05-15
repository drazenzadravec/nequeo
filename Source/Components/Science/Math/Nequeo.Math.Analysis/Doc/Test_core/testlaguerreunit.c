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
#include "testlaguerreunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testlaguerre(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    double err;
    double sumerr;
    double cerr;
    double threshold;
    ae_int_t n;
    ae_int_t maxn;
    ae_int_t pass;
    ae_vector c;
    double x;
    double v;
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
     * Testing Laguerre polynomials
     */
    n = 0;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)-1.0000000000, _state), _state);
    n = 1;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)-0.5000000000, _state), _state);
    n = 2;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)-0.1250000000, _state), _state);
    n = 3;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)+0.1458333333, _state), _state);
    n = 4;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)+0.3307291667, _state), _state);
    n = 5;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)+0.4455729167, _state), _state);
    n = 6;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)+0.5041449653, _state), _state);
    n = 7;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)+0.5183392237, _state), _state);
    n = 8;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)+0.4983629984, _state), _state);
    n = 9;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)+0.4529195204, _state), _state);
    n = 10;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)+0.3893744141, _state), _state);
    n = 11;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)+0.3139072988, _state), _state);
    n = 12;
    err = ae_maxreal(err, ae_fabs(laguerrecalculate(n, 0.5, _state)+0.2316496389, _state), _state);
    
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
            v = v+laguerrecalculate(n, x, _state)*c.ptr.p_double[n];
            sumerr = ae_maxreal(sumerr, ae_fabs(v-laguerresum(&c, n, x, _state), _state), _state);
        }
    }
    
    /*
     * Testing coefficients
     */
    laguerrecoefficients(0, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-1, _state), _state);
    laguerrecoefficients(1, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-1, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]+1, _state), _state);
    laguerrecoefficients(2, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-(double)2/(double)2, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]+(double)4/(double)2, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-(double)1/(double)2, _state), _state);
    laguerrecoefficients(3, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-(double)6/(double)6, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]+(double)18/(double)6, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-(double)9/(double)6, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]+(double)1/(double)6, _state), _state);
    laguerrecoefficients(4, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-(double)24/(double)24, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]+(double)96/(double)24, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-(double)72/(double)24, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]+(double)16/(double)24, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[4]-(double)1/(double)24, _state), _state);
    laguerrecoefficients(5, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-(double)120/(double)120, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]+(double)600/(double)120, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-(double)600/(double)120, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]+(double)200/(double)120, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[4]-(double)25/(double)120, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[5]+(double)1/(double)120, _state), _state);
    laguerrecoefficients(6, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-(double)720/(double)720, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]+(double)4320/(double)720, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-(double)5400/(double)720, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]+(double)2400/(double)720, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[4]-(double)450/(double)720, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[5]+(double)36/(double)720, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[6]-(double)1/(double)720, _state), _state);
    
    /*
     * Reporting
     */
    waserrors = (ae_fp_greater(err,threshold)||ae_fp_greater(sumerr,threshold))||ae_fp_greater(cerr,threshold);
    if( !silent )
    {
        printf("TESTING CALCULATION OF THE LAGUERRE POLYNOMIALS\n");
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
ae_bool _pexec_testlaguerre(ae_bool silent, ae_state *_state)
{
    return testlaguerre(silent, _state);
}


/*$ End $*/
