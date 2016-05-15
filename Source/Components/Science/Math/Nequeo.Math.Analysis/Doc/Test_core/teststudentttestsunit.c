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
#include "teststudentttestsunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool teststudentttests(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    double eps;
    ae_vector x;
    ae_vector y;
    ae_vector xa;
    ae_vector ya;
    ae_vector xb;
    ae_vector yb;
    ae_int_t n;
    double taill;
    double tailr;
    double tailb;
    double taill1;
    double tailr1;
    double tailb1;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&xa, 0, DT_REAL, _state);
    ae_vector_init(&ya, 0, DT_REAL, _state);
    ae_vector_init(&xb, 0, DT_REAL, _state);
    ae_vector_init(&yb, 0, DT_REAL, _state);

    waserrors = ae_false;
    eps = 0.001;
    
    /*
     * 1-sample test
     */
    n = 8;
    ae_vector_set_length(&x, 8, _state);
    x.ptr.p_double[0] = -3.0;
    x.ptr.p_double[1] = -1.5;
    x.ptr.p_double[2] = -1.0;
    x.ptr.p_double[3] = -0.5;
    x.ptr.p_double[4] = 0.5;
    x.ptr.p_double[5] = 1.0;
    x.ptr.p_double[6] = 1.5;
    x.ptr.p_double[7] = 3.0;
    studentttest1(&x, n, 0.0, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-1.00000, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-0.50000, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-0.50000, _state),eps);
    studentttest1(&x, n, 1.0, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-0.17816, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-0.08908, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-0.91092, _state),eps);
    studentttest1(&x, n, -1.0, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-0.17816, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-0.91092, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-0.08908, _state),eps);
    x.ptr.p_double[0] = 1.1;
    x.ptr.p_double[1] = 1.1;
    x.ptr.p_double[2] = 1.1;
    x.ptr.p_double[3] = 1.1;
    x.ptr.p_double[4] = 1.1;
    x.ptr.p_double[5] = 1.1;
    x.ptr.p_double[6] = 1.1;
    x.ptr.p_double[7] = 1.1;
    studentttest1(&x, n, 1.1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(1));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest1(&x, n, 0.0, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(0));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(0));
    x.ptr.p_double[7] = 1.1;
    studentttest1(&x, 1, 1.1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(1));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest1(&x, 1, 0.0, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(0));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(0));
    
    /*
     * 2-sample pooled (equal variance) test
     */
    n = 8;
    ae_vector_set_length(&x, 8, _state);
    ae_vector_set_length(&y, 8, _state);
    x.ptr.p_double[0] = -3.0;
    x.ptr.p_double[1] = -1.5;
    x.ptr.p_double[2] = -1.0;
    x.ptr.p_double[3] = -0.5;
    x.ptr.p_double[4] = 0.5;
    x.ptr.p_double[5] = 1.0;
    x.ptr.p_double[6] = 1.5;
    x.ptr.p_double[7] = 3.0;
    y.ptr.p_double[0] = -2.0;
    y.ptr.p_double[1] = -0.5;
    y.ptr.p_double[2] = 0.0;
    y.ptr.p_double[3] = 0.5;
    y.ptr.p_double[4] = 1.5;
    y.ptr.p_double[5] = 2.0;
    y.ptr.p_double[6] = 2.5;
    y.ptr.p_double[7] = 4.0;
    studentttest2(&x, n, &y, n, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-0.30780, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-0.15390, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-0.84610, _state),eps);
    studentttest2(&x, n, &y, n-1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-0.53853, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-0.26927, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-0.73074, _state),eps);
    studentttest2(&x, n-1, &y, n, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-0.13829, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-0.06915, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-0.93086, _state),eps);
    x.ptr.p_double[0] = -1.0;
    x.ptr.p_double[1] = -1.0;
    x.ptr.p_double[2] = -1.0;
    x.ptr.p_double[3] = -1.0;
    x.ptr.p_double[4] = -1.0;
    x.ptr.p_double[5] = -1.0;
    x.ptr.p_double[6] = -1.0;
    x.ptr.p_double[7] = -1.0;
    y.ptr.p_double[0] = 1.0;
    y.ptr.p_double[1] = 1.0;
    y.ptr.p_double[2] = 1.0;
    y.ptr.p_double[3] = 1.0;
    y.ptr.p_double[4] = 1.0;
    y.ptr.p_double[5] = 1.0;
    y.ptr.p_double[6] = 1.0;
    y.ptr.p_double[7] = 1.0;
    studentttest2(&x, n, &y, n, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(0));
    waserrors = waserrors||ae_fp_neq(taill,(double)(0));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest2(&x, n, &y, n-1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(0));
    waserrors = waserrors||ae_fp_neq(taill,(double)(0));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest2(&x, n, &y, 1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(0));
    waserrors = waserrors||ae_fp_neq(taill,(double)(0));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest2(&x, n-1, &y, n, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(0));
    waserrors = waserrors||ae_fp_neq(taill,(double)(0));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest2(&x, 1, &y, n, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(0));
    waserrors = waserrors||ae_fp_neq(taill,(double)(0));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest2(&x, 1, &y, 1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(0));
    waserrors = waserrors||ae_fp_neq(taill,(double)(0));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest2(&y, 1, &x, 1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(0));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(0));
    x.ptr.p_double[0] = 1.1;
    x.ptr.p_double[1] = 1.1;
    x.ptr.p_double[2] = 1.1;
    x.ptr.p_double[3] = 1.1;
    x.ptr.p_double[4] = 1.1;
    x.ptr.p_double[5] = 1.1;
    x.ptr.p_double[6] = 1.1;
    x.ptr.p_double[7] = 1.1;
    y.ptr.p_double[0] = 1.1;
    y.ptr.p_double[1] = 1.1;
    y.ptr.p_double[2] = 1.1;
    y.ptr.p_double[3] = 1.1;
    y.ptr.p_double[4] = 1.1;
    y.ptr.p_double[5] = 1.1;
    y.ptr.p_double[6] = 1.1;
    y.ptr.p_double[7] = 1.1;
    studentttest2(&x, n, &y, n, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(1));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest2(&x, n, &y, n-1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(1));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest2(&x, n, &y, 1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(1));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest2(&x, n-1, &y, n, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(1));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest2(&x, 1, &y, n, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(1));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    studentttest2(&x, 1, &y, 1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(1));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    
    /*
     * 2-sample unpooled (unequal variance) test:
     * * test on two non-constant samples
     * * tests on different combinations of non-constant and constant samples
     */
    n = 8;
    ae_vector_set_length(&xa, 8, _state);
    ae_vector_set_length(&ya, 8, _state);
    ae_vector_set_length(&xb, 8, _state);
    ae_vector_set_length(&yb, 8, _state);
    xa.ptr.p_double[0] = -3.0;
    xa.ptr.p_double[1] = -1.5;
    xa.ptr.p_double[2] = -1.0;
    xa.ptr.p_double[3] = -0.5;
    xa.ptr.p_double[4] = 0.5;
    xa.ptr.p_double[5] = 1.0;
    xa.ptr.p_double[6] = 1.5;
    xa.ptr.p_double[7] = 3.0;
    ya.ptr.p_double[0] = -1.0;
    ya.ptr.p_double[1] = -0.5;
    ya.ptr.p_double[2] = 0.0;
    ya.ptr.p_double[3] = 0.5;
    ya.ptr.p_double[4] = 1.5;
    ya.ptr.p_double[5] = 2.0;
    ya.ptr.p_double[6] = 2.5;
    ya.ptr.p_double[7] = 3.0;
    xb.ptr.p_double[0] = -1.1;
    xb.ptr.p_double[1] = -1.1;
    xb.ptr.p_double[2] = -1.1;
    xb.ptr.p_double[3] = -1.1;
    xb.ptr.p_double[4] = -1.1;
    xb.ptr.p_double[5] = -1.1;
    xb.ptr.p_double[6] = -1.1;
    xb.ptr.p_double[7] = -1.1;
    yb.ptr.p_double[0] = 1.1;
    yb.ptr.p_double[1] = 1.1;
    yb.ptr.p_double[2] = 1.1;
    yb.ptr.p_double[3] = 1.1;
    yb.ptr.p_double[4] = 1.1;
    yb.ptr.p_double[5] = 1.1;
    yb.ptr.p_double[6] = 1.1;
    yb.ptr.p_double[7] = 1.1;
    unequalvariancettest(&xa, n, &ya, n, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-0.25791, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-0.12896, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-0.87105, _state),eps);
    unequalvariancettest(&xa, n, &yb, n, &tailb, &taill, &tailr, _state);
    studentttest1(&xa, n, 1.1, &tailb1, &taill1, &tailr1, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-tailb1, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-taill1, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-tailr1, _state),eps);
    unequalvariancettest(&xa, n, &yb, 1, &tailb, &taill, &tailr, _state);
    studentttest1(&xa, n, 1.1, &tailb1, &taill1, &tailr1, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-tailb1, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-taill1, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-tailr1, _state),eps);
    unequalvariancettest(&xb, n, &ya, n, &tailb, &taill, &tailr, _state);
    studentttest1(&ya, n, -1.1, &tailb1, &taill1, &tailr1, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-tailb1, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-tailr1, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-taill1, _state),eps);
    unequalvariancettest(&xb, 1, &ya, n, &tailb, &taill, &tailr, _state);
    studentttest1(&ya, n, -1.1, &tailb1, &taill1, &tailr1, _state);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailb-tailb1, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(taill-tailr1, _state),eps);
    waserrors = waserrors||ae_fp_greater(ae_fabs(tailr-taill1, _state),eps);
    unequalvariancettest(&xb, 1, &yb, 1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(0));
    waserrors = waserrors||ae_fp_neq(taill,(double)(0));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    unequalvariancettest(&yb, 1, &xb, 1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(0));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(0));
    unequalvariancettest(&xb, 1, &xb, 1, &tailb, &taill, &tailr, _state);
    waserrors = waserrors||ae_fp_neq(tailb,(double)(1));
    waserrors = waserrors||ae_fp_neq(taill,(double)(1));
    waserrors = waserrors||ae_fp_neq(tailr,(double)(1));
    
    /*
     *
     */
    if( !silent )
    {
        if( waserrors )
        {
            printf("TEST FAILED\n");
        }
        else
        {
            printf("TEST PASSED\n");
        }
    }
    result = !waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_teststudentttests(ae_bool silent, ae_state *_state)
{
    return teststudentttests(silent, _state);
}


/*$ End $*/
