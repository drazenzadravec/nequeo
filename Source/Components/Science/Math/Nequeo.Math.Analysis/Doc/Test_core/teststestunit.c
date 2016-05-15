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
#include "teststestunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool teststest(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    double taill;
    double tailr;
    double tailb;
    ae_bool waserrors;
    double eps;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);

    waserrors = ae_false;
    eps = 1.0E-3;
    
    /*
     * Test 1
     */
    ae_vector_set_length(&x, 6, _state);
    x.ptr.p_double[0] = (double)(-3);
    x.ptr.p_double[1] = (double)(-2);
    x.ptr.p_double[2] = (double)(-1);
    x.ptr.p_double[3] = (double)(1);
    x.ptr.p_double[4] = (double)(2);
    x.ptr.p_double[5] = (double)(3);
    onesamplesigntest(&x, 6, 0.0, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-0.65625, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-0.65625, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-1.00000, _state),eps);
    onesamplesigntest(&x, 6, -1.0, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-0.81250, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-0.50000, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-1.00000, _state),eps);
    onesamplesigntest(&x, 6, -1.5, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-0.89062, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-0.34375, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-0.68750, _state),eps);
    onesamplesigntest(&x, 6, -3.0, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-1.00000, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-0.03125, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-0.06250, _state),eps);
    
    /*
     * Test 2
     */
    ae_vector_set_length(&x, 3, _state);
    x.ptr.p_double[0] = (double)(2);
    x.ptr.p_double[1] = (double)(2);
    x.ptr.p_double[2] = (double)(2);
    onesamplesigntest(&x, 3, 2.0, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailb,(double)(1));
    
    /*
     * Final report
     */
    if( !silent )
    {
        printf("SIGN TEST:                               ");
        if( !waserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        if( waserrors )
        {
            printf("TEST SUMMARY: FAILED\n");
        }
        else
        {
            printf("TEST SUMMARY: PASSED\n");
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
ae_bool _pexec_teststest(ae_bool silent, ae_state *_state)
{
    return teststest(silent, _state);
}


/*$ End $*/
