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
#include "testsblasunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testsblas(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix ua;
    ae_matrix la;
    ae_vector x;
    ae_vector y1;
    ae_vector y2;
    ae_vector y3;
    ae_int_t n;
    ae_int_t maxn;
    ae_int_t i;
    ae_int_t j;
    ae_int_t i1;
    ae_int_t i2;
    ae_bool waserrors;
    double mverr;
    double threshold;
    double alpha;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ua, 0, 0, DT_REAL, _state);
    ae_matrix_init(&la, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&y3, 0, DT_REAL, _state);

    mverr = (double)(0);
    waserrors = ae_false;
    maxn = 10;
    threshold = 1000*ae_machineepsilon;
    
    /*
     * Test MV
     */
    for(n=2; n<=maxn; n++)
    {
        ae_matrix_set_length(&a, n+1, n+1, _state);
        ae_matrix_set_length(&ua, n+1, n+1, _state);
        ae_matrix_set_length(&la, n+1, n+1, _state);
        ae_vector_set_length(&x, n+1, _state);
        ae_vector_set_length(&y1, n+1, _state);
        ae_vector_set_length(&y2, n+1, _state);
        ae_vector_set_length(&y3, n+1, _state);
        
        /*
         * fill A, UA, LA
         */
        for(i=1; i<=n; i++)
        {
            a.ptr.pp_double[i][i] = 2*ae_randomreal(_state)-1;
            for(j=i+1; j<=n; j++)
            {
                a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                a.ptr.pp_double[j][i] = a.ptr.pp_double[i][j];
            }
        }
        for(i=1; i<=n; i++)
        {
            for(j=1; j<=n; j++)
            {
                ua.ptr.pp_double[i][j] = (double)(0);
            }
        }
        for(i=1; i<=n; i++)
        {
            for(j=i; j<=n; j++)
            {
                ua.ptr.pp_double[i][j] = a.ptr.pp_double[i][j];
            }
        }
        for(i=1; i<=n; i++)
        {
            for(j=1; j<=n; j++)
            {
                la.ptr.pp_double[i][j] = (double)(0);
            }
        }
        for(i=1; i<=n; i++)
        {
            for(j=1; j<=i; j++)
            {
                la.ptr.pp_double[i][j] = a.ptr.pp_double[i][j];
            }
        }
        
        /*
         * test on different I1, I2
         */
        for(i1=1; i1<=n; i1++)
        {
            for(i2=i1; i2<=n; i2++)
            {
                
                /*
                 * Fill X, choose Alpha
                 */
                for(i=1; i<=i2-i1+1; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                alpha = 2*ae_randomreal(_state)-1;
                
                /*
                 * calculate A*x, UA*x, LA*x
                 */
                for(i=i1; i<=i2; i++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][i1], 1, &x.ptr.p_double[1], 1, ae_v_len(i1,i2));
                    y1.ptr.p_double[i-i1+1] = alpha*v;
                }
                symmetricmatrixvectormultiply(&ua, ae_true, i1, i2, &x, alpha, &y2, _state);
                symmetricmatrixvectormultiply(&la, ae_false, i1, i2, &x, alpha, &y3, _state);
                
                /*
                 * Calculate error
                 */
                ae_v_sub(&y2.ptr.p_double[1], 1, &y1.ptr.p_double[1], 1, ae_v_len(1,i2-i1+1));
                v = ae_v_dotproduct(&y2.ptr.p_double[1], 1, &y2.ptr.p_double[1], 1, ae_v_len(1,i2-i1+1));
                mverr = ae_maxreal(mverr, ae_sqrt(v, _state), _state);
                ae_v_sub(&y3.ptr.p_double[1], 1, &y1.ptr.p_double[1], 1, ae_v_len(1,i2-i1+1));
                v = ae_v_dotproduct(&y3.ptr.p_double[1], 1, &y3.ptr.p_double[1], 1, ae_v_len(1,i2-i1+1));
                mverr = ae_maxreal(mverr, ae_sqrt(v, _state), _state);
            }
        }
    }
    
    /*
     * report
     */
    waserrors = ae_fp_greater(mverr,threshold);
    if( !silent )
    {
        printf("TESTING SYMMETRIC BLAS\n");
        printf("MV error:                                %5.3e\n",
            (double)(mverr));
        printf("Threshold:                               %5.3e\n",
            (double)(threshold));
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
ae_bool _pexec_testsblas(ae_bool silent, ae_state *_state)
{
    return testsblas(silent, _state);
}


/*$ End $*/
