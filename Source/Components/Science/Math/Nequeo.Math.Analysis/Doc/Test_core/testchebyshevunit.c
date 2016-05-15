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
#include "testchebyshevunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testchebyshev(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    double err;
    double sumerr;
    double cerr;
    double ferr;
    double threshold;
    double x;
    double v;
    ae_int_t pass;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t n;
    ae_int_t maxn;
    ae_vector c;
    ae_vector p1;
    ae_vector p2;
    ae_matrix a;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&c, 0, DT_REAL, _state);
    ae_vector_init(&p1, 0, DT_REAL, _state);
    ae_vector_init(&p2, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);

    err = (double)(0);
    sumerr = (double)(0);
    cerr = (double)(0);
    ferr = (double)(0);
    threshold = 1.0E-9;
    waserrors = ae_false;
    
    /*
     * Testing Chebyshev polynomials of the first kind
     */
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 0, 0.00, _state)-1, _state), _state);
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 0, 0.33, _state)-1, _state), _state);
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 0, -0.42, _state)-1, _state), _state);
    x = 0.2;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 1, x, _state)-0.2, _state), _state);
    x = 0.4;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 1, x, _state)-0.4, _state), _state);
    x = 0.6;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 1, x, _state)-0.6, _state), _state);
    x = 0.8;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 1, x, _state)-0.8, _state), _state);
    x = 1.0;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 1, x, _state)-1.0, _state), _state);
    x = 0.2;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 2, x, _state)+0.92, _state), _state);
    x = 0.4;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 2, x, _state)+0.68, _state), _state);
    x = 0.6;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 2, x, _state)+0.28, _state), _state);
    x = 0.8;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 2, x, _state)-0.28, _state), _state);
    x = 1.0;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, 2, x, _state)-1.00, _state), _state);
    n = 10;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, n, 0.2, _state)-0.4284556288, _state), _state);
    n = 11;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, n, 0.2, _state)+0.7996160205, _state), _state);
    n = 12;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(1, n, 0.2, _state)+0.7483020370, _state), _state);
    
    /*
     * Testing Chebyshev polynomials of the second kind
     */
    n = 0;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(2, n, 0.2, _state)-1.0000000000, _state), _state);
    n = 1;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(2, n, 0.2, _state)-0.4000000000, _state), _state);
    n = 2;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(2, n, 0.2, _state)+0.8400000000, _state), _state);
    n = 3;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(2, n, 0.2, _state)+0.7360000000, _state), _state);
    n = 4;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(2, n, 0.2, _state)-0.5456000000, _state), _state);
    n = 10;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(2, n, 0.2, _state)-0.6128946176, _state), _state);
    n = 11;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(2, n, 0.2, _state)+0.6770370970, _state), _state);
    n = 12;
    err = ae_maxreal(err, ae_fabs(chebyshevcalculate(2, n, 0.2, _state)+0.8837094564, _state), _state);
    
    /*
     * Testing Clenshaw summation
     */
    maxn = 20;
    ae_vector_set_length(&c, maxn+1, _state);
    for(k=1; k<=2; k++)
    {
        for(pass=1; pass<=10; pass++)
        {
            x = 2*ae_randomreal(_state)-1;
            v = (double)(0);
            for(n=0; n<=maxn; n++)
            {
                c.ptr.p_double[n] = 2*ae_randomreal(_state)-1;
                v = v+chebyshevcalculate(k, n, x, _state)*c.ptr.p_double[n];
                sumerr = ae_maxreal(sumerr, ae_fabs(v-chebyshevsum(&c, k, n, x, _state), _state), _state);
            }
        }
    }
    
    /*
     * Testing coefficients
     */
    chebyshevcoefficients(0, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-1, _state), _state);
    chebyshevcoefficients(1, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-1, _state), _state);
    chebyshevcoefficients(2, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]+1, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-2, _state), _state);
    chebyshevcoefficients(3, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]+3, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]-4, _state), _state);
    chebyshevcoefficients(4, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-1, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]+8, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[4]-8, _state), _state);
    chebyshevcoefficients(9, &c, _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[0]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[1]-9, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[2]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[3]+120, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[4]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[5]-432, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[6]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[7]+576, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[8]-0, _state), _state);
    cerr = ae_maxreal(cerr, ae_fabs(c.ptr.p_double[9]-256, _state), _state);
    
    /*
     * Testing FromChebyshev
     */
    maxn = 10;
    ae_matrix_set_length(&a, maxn+1, maxn+1, _state);
    for(i=0; i<=maxn; i++)
    {
        for(j=0; j<=maxn; j++)
        {
            a.ptr.pp_double[i][j] = (double)(0);
        }
        chebyshevcoefficients(i, &c, _state);
        ae_v_move(&a.ptr.pp_double[i][0], 1, &c.ptr.p_double[0], 1, ae_v_len(0,i));
    }
    ae_vector_set_length(&c, maxn+1, _state);
    ae_vector_set_length(&p1, maxn+1, _state);
    for(n=0; n<=maxn; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            for(i=0; i<=n; i++)
            {
                p1.ptr.p_double[i] = (double)(0);
            }
            for(i=0; i<=n; i++)
            {
                c.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                v = c.ptr.p_double[i];
                ae_v_addd(&p1.ptr.p_double[0], 1, &a.ptr.pp_double[i][0], 1, ae_v_len(0,i), v);
            }
            fromchebyshev(&c, n, &p2, _state);
            for(i=0; i<=n; i++)
            {
                ferr = ae_maxreal(ferr, ae_fabs(p1.ptr.p_double[i]-p2.ptr.p_double[i], _state), _state);
            }
        }
    }
    
    /*
     * Reporting
     */
    waserrors = ((ae_fp_greater(err,threshold)||ae_fp_greater(sumerr,threshold))||ae_fp_greater(cerr,threshold))||ae_fp_greater(ferr,threshold);
    if( !silent )
    {
        printf("TESTING CALCULATION OF THE CHEBYSHEV POLYNOMIALS\n");
        printf("Max error against table                   %5.2e\n",
            (double)(err));
        printf("Summation error                           %5.2e\n",
            (double)(sumerr));
        printf("Coefficients error                        %5.2e\n",
            (double)(cerr));
        printf("FrobChebyshev error                       %5.2e\n",
            (double)(ferr));
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
ae_bool _pexec_testchebyshev(ae_bool silent, ae_state *_state)
{
    return testchebyshev(silent, _state);
}


/*$ End $*/
