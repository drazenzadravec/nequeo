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
#include "testxblasunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testxblas(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool approxerrors;
    ae_bool exactnesserrors;
    ae_bool waserrors;
    double approxthreshold;
    ae_int_t maxn;
    ae_int_t passcount;
    ae_int_t n;
    ae_int_t i;
    ae_int_t pass;
    double rv1;
    double rv2;
    double rv2err;
    ae_complex cv1;
    ae_complex cv2;
    double cv2err;
    ae_vector rx;
    ae_vector ry;
    ae_vector cx;
    ae_vector cy;
    ae_vector temp;
    double s;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&rx, 0, DT_REAL, _state);
    ae_vector_init(&ry, 0, DT_REAL, _state);
    ae_vector_init(&cx, 0, DT_COMPLEX, _state);
    ae_vector_init(&cy, 0, DT_COMPLEX, _state);
    ae_vector_init(&temp, 0, DT_REAL, _state);

    approxerrors = ae_false;
    exactnesserrors = ae_false;
    waserrors = ae_false;
    approxthreshold = 1000*ae_machineepsilon;
    maxn = 1000;
    passcount = 10;
    
    /*
     * tests:
     * 1. ability to calculate dot product
     * 2. higher precision
     */
    for(n=1; n<=maxn; n++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             *  ability to approximately calculate real dot product
             */
            ae_vector_set_length(&rx, n, _state);
            ae_vector_set_length(&ry, n, _state);
            ae_vector_set_length(&temp, n, _state);
            for(i=0; i<=n-1; i++)
            {
                if( ae_fp_greater(ae_randomreal(_state),0.2) )
                {
                    rx.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                else
                {
                    rx.ptr.p_double[i] = (double)(0);
                }
                if( ae_fp_greater(ae_randomreal(_state),0.2) )
                {
                    ry.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                else
                {
                    ry.ptr.p_double[i] = (double)(0);
                }
            }
            rv1 = ae_v_dotproduct(&rx.ptr.p_double[0], 1, &ry.ptr.p_double[0], 1, ae_v_len(0,n-1));
            xdot(&rx, &ry, n, &temp, &rv2, &rv2err, _state);
            approxerrors = approxerrors||ae_fp_greater(ae_fabs(rv1-rv2, _state),approxthreshold);
            
            /*
             *  ability to approximately calculate complex dot product
             */
            ae_vector_set_length(&cx, n, _state);
            ae_vector_set_length(&cy, n, _state);
            ae_vector_set_length(&temp, 2*n, _state);
            for(i=0; i<=n-1; i++)
            {
                if( ae_fp_greater(ae_randomreal(_state),0.2) )
                {
                    cx.ptr.p_complex[i].x = 2*ae_randomreal(_state)-1;
                    cx.ptr.p_complex[i].y = 2*ae_randomreal(_state)-1;
                }
                else
                {
                    cx.ptr.p_complex[i] = ae_complex_from_i(0);
                }
                if( ae_fp_greater(ae_randomreal(_state),0.2) )
                {
                    cy.ptr.p_complex[i].x = 2*ae_randomreal(_state)-1;
                    cy.ptr.p_complex[i].y = 2*ae_randomreal(_state)-1;
                }
                else
                {
                    cy.ptr.p_complex[i] = ae_complex_from_i(0);
                }
            }
            cv1 = ae_v_cdotproduct(&cx.ptr.p_complex[0], 1, "N", &cy.ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
            xcdot(&cx, &cy, n, &temp, &cv2, &cv2err, _state);
            approxerrors = approxerrors||ae_fp_greater(ae_c_abs(ae_c_sub(cv1,cv2), _state),approxthreshold);
        }
    }
    
    /*
     * test of precision: real
     */
    n = 50000;
    ae_vector_set_length(&rx, n, _state);
    ae_vector_set_length(&ry, n, _state);
    ae_vector_set_length(&temp, n, _state);
    for(pass=0; pass<=passcount-1; pass++)
    {
        ae_assert(n%2==0, "Assertion failed", _state);
        
        /*
         * First test: X + X + ... + X - X - X - ... - X = 1*X
         */
        s = ae_exp((double)(ae_maxint(pass, 50, _state)), _state);
        if( pass==passcount-1&&pass>1 )
        {
            s = ae_maxrealnumber;
        }
        ry.ptr.p_double[0] = (2*ae_randomreal(_state)-1)*s*ae_sqrt(2*ae_randomreal(_state), _state);
        for(i=1; i<=n-1; i++)
        {
            ry.ptr.p_double[i] = ry.ptr.p_double[0];
        }
        for(i=0; i<=n/2-1; i++)
        {
            rx.ptr.p_double[i] = (double)(1);
        }
        for(i=n/2; i<=n-2; i++)
        {
            rx.ptr.p_double[i] = (double)(-1);
        }
        rx.ptr.p_double[n-1] = (double)(0);
        xdot(&rx, &ry, n, &temp, &rv2, &rv2err, _state);
        exactnesserrors = exactnesserrors||ae_fp_less(rv2err,(double)(0));
        exactnesserrors = exactnesserrors||ae_fp_greater(rv2err,4*ae_machineepsilon*ae_fabs(ry.ptr.p_double[0], _state));
        exactnesserrors = exactnesserrors||ae_fp_greater(ae_fabs(rv2-ry.ptr.p_double[0], _state),rv2err);
        
        /*
         * First test: X + X + ... + X = N*X
         */
        s = ae_exp((double)(ae_maxint(pass, 50, _state)), _state);
        if( pass==passcount-1&&pass>1 )
        {
            s = ae_maxrealnumber;
        }
        ry.ptr.p_double[0] = (2*ae_randomreal(_state)-1)*s*ae_sqrt(2*ae_randomreal(_state), _state);
        for(i=1; i<=n-1; i++)
        {
            ry.ptr.p_double[i] = ry.ptr.p_double[0];
        }
        for(i=0; i<=n-1; i++)
        {
            rx.ptr.p_double[i] = (double)(1);
        }
        xdot(&rx, &ry, n, &temp, &rv2, &rv2err, _state);
        exactnesserrors = exactnesserrors||ae_fp_less(rv2err,(double)(0));
        exactnesserrors = exactnesserrors||ae_fp_greater(rv2err,4*ae_machineepsilon*ae_fabs(ry.ptr.p_double[0], _state)*n);
        exactnesserrors = exactnesserrors||ae_fp_greater(ae_fabs(rv2-n*ry.ptr.p_double[0], _state),rv2err);
    }
    
    /*
     * test of precision: complex
     */
    n = 50000;
    ae_vector_set_length(&cx, n, _state);
    ae_vector_set_length(&cy, n, _state);
    ae_vector_set_length(&temp, 2*n, _state);
    for(pass=0; pass<=passcount-1; pass++)
    {
        ae_assert(n%2==0, "Assertion failed", _state);
        
        /*
         * First test: X + X + ... + X - X - X - ... - X = 1*X
         */
        s = ae_exp((double)(ae_maxint(pass, 50, _state)), _state);
        if( pass==passcount-1&&pass>1 )
        {
            s = ae_maxrealnumber;
        }
        cy.ptr.p_complex[0].x = (2*ae_randomreal(_state)-1)*s*ae_sqrt(2*ae_randomreal(_state), _state);
        cy.ptr.p_complex[0].y = (2*ae_randomreal(_state)-1)*s*ae_sqrt(2*ae_randomreal(_state), _state);
        for(i=1; i<=n-1; i++)
        {
            cy.ptr.p_complex[i] = cy.ptr.p_complex[0];
        }
        for(i=0; i<=n/2-1; i++)
        {
            cx.ptr.p_complex[i] = ae_complex_from_i(1);
        }
        for(i=n/2; i<=n-2; i++)
        {
            cx.ptr.p_complex[i] = ae_complex_from_i(-1);
        }
        cx.ptr.p_complex[n-1] = ae_complex_from_i(0);
        xcdot(&cx, &cy, n, &temp, &cv2, &cv2err, _state);
        exactnesserrors = exactnesserrors||ae_fp_less(cv2err,(double)(0));
        exactnesserrors = exactnesserrors||ae_fp_greater(cv2err,4*ae_machineepsilon*ae_c_abs(cy.ptr.p_complex[0], _state));
        exactnesserrors = exactnesserrors||ae_fp_greater(ae_c_abs(ae_c_sub(cv2,cy.ptr.p_complex[0]), _state),cv2err);
        
        /*
         * First test: X + X + ... + X = N*X
         */
        s = ae_exp((double)(ae_maxint(pass, 50, _state)), _state);
        if( pass==passcount-1&&pass>1 )
        {
            s = ae_maxrealnumber;
        }
        cy.ptr.p_complex[0] = ae_complex_from_d((2*ae_randomreal(_state)-1)*s*ae_sqrt(2*ae_randomreal(_state), _state));
        for(i=1; i<=n-1; i++)
        {
            cy.ptr.p_complex[i] = cy.ptr.p_complex[0];
        }
        for(i=0; i<=n-1; i++)
        {
            cx.ptr.p_complex[i] = ae_complex_from_i(1);
        }
        xcdot(&cx, &cy, n, &temp, &cv2, &cv2err, _state);
        exactnesserrors = exactnesserrors||ae_fp_less(cv2err,(double)(0));
        exactnesserrors = exactnesserrors||ae_fp_greater(cv2err,4*ae_machineepsilon*ae_c_abs(cy.ptr.p_complex[0], _state)*n);
        exactnesserrors = exactnesserrors||ae_fp_greater(ae_c_abs(ae_c_sub(cv2,ae_c_mul_d(cy.ptr.p_complex[0],1.0*n)), _state),cv2err);
    }
    
    /*
     * report
     */
    waserrors = approxerrors||exactnesserrors;
    if( !silent )
    {
        printf("TESTING XBLAS\n");
        printf("APPROX.TESTS:                            ");
        if( approxerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("EXACT TESTS:                             ");
        if( exactnesserrors )
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
    
    /*
     * end
     */
    result = !waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testxblas(ae_bool silent, ae_state *_state)
{
    return testxblas(silent, _state);
}


/*$ End $*/
