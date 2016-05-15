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
#include "testhermiteunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testhermite(ae_bool silent, ae_state *_state)
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
     * Testing Hermite polynomials
     */
    n = 0;
    err = ae_maxreal(err, ae_fabs(hermitecalculate(n, (double)(1), _state)-1, _state), _state);
    n = 1;
    err = ae_maxreal(err, ae_fabs(hermitecalculate(n, (double)(1), _state)-2, _state), _state);
    n = 2;
    err = ae_maxreal(err, ae_fabs(hermitecalculate(n, (double)(1), _state)-2, _state), _state);
    n = 3;
    err = ae_maxreal(err, ae_fabs(hermitecalculate(n, (double)(1), _state)+4, _state), _state);
    n = 4;
    err = ae_maxreal(err, ae_fabs(hermitecalculate(n, (double)(1), _state)+20, _state), _state);
    n = 5;
    err = ae_maxreal(err, ae_fabs(hermitecalculate(n, (double)(1), _state)+8, _state), _state);
    n = 6;
    err = ae_maxreal(err, ae_fabs(hermitecalculate(n, (double)(1), _state)-184, _state), _state);
    n = 7;
    err = ae_maxreal(err, ae_fabs(hermitecalculate(n, (double)(1), _state)-464, _state), _state);
    n = 11;
    err = ae_maxreal(err, ae_fabs(hermitecalculate(n, (double)(1), _state)-230848, _state), _state);
    n = 12;
    err = ae_maxreal(err, ae_fabs(hermitecalculate(n, (double)(1), _state)-280768, _state), _state);
    
    /*
     * Testing Clenshaw summation
     */
    maxn = 10;
    ae_vector_set_length(&c, maxn+1, _state);
    for(pass=1; pass<=10; pass++)
    {
        x = 2*ae_randomreal(_state)-1;
        v = (double)(0);
        for(n=0; n<=maxn; n++)
        {
            c.ptr.p_double[n] = 2*ae_randomreal(_state)-1;
            v = v+hermitecalculate(n, x, _state)*c.ptr.p_double[n];
            sumerr = ae_maxreal(sumerr, ae_fabs(v-hermitesum(&c, n, x, _state), _state), _state);
        }
    }
    
    /*
     * Testing coefficients
     */
    hermitecoefficients(0, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-1, _state), _state);
    hermitecoefficients(1, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-2, _state), _state);
    hermitecoefficients(2, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]+2, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-4, _state), _state);
    hermitecoefficients(3, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]+12, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]-8, _state), _state);
    hermitecoefficients(4, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-12, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]+48, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[4]-16, _state), _state);
    hermitecoefficients(5, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-120, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]+160, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[4]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[5]-32, _state), _state);
    hermitecoefficients(6, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]+120, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-720, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[4]+480, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[5]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[6]-64, _state), _state);
    
    /*
     * Reporting
     */
    waserrors = (ae_fp_greater(err,threshold)||ae_fp_greater(sumerr,threshold))||ae_fp_greater(cerr,threshold);
    if( !silent )
    {
        printf("TESTING CALCULATION OF THE HERMITE POLYNOMIALS\n");
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
ae_bool _pexec_testhermite(ae_bool silent, ae_state *_state)
{
    return testhermite(silent, _state);
}


/*$ End $*/
