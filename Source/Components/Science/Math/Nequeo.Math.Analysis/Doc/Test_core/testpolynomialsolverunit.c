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
#include "testpolynomialsolverunit.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Test
*************************************************************************/
ae_bool testpolynomialsolver(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool wereerrors;
    ae_vector a;
    ae_vector x;
    double eps;
    ae_int_t n;
    polynomialsolverreport rep;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_COMPLEX, _state);
    _polynomialsolverreport_init(&rep, _state);

    wereerrors = ae_false;
    
    /*
     * Basic tests
     */
    eps = 1.0E-6;
    n = 1;
    ae_vector_set_length(&a, n+1, _state);
    a.ptr.p_double[0] = (double)(2);
    a.ptr.p_double[1] = (double)(3);
    polynomialsolve(&a, n, &x, &rep, _state);
    seterrorflag(&wereerrors, ae_fp_greater(ae_fabs(x.ptr.p_complex[0].x+(double)2/(double)3, _state),eps), _state);
    seterrorflag(&wereerrors, ae_fp_neq(x.ptr.p_complex[0].y,(double)(0)), _state);
    seterrorflag(&wereerrors, ae_fp_greater(rep.maxerr,100*ae_machineepsilon), _state);
    n = 2;
    ae_vector_set_length(&a, n+1, _state);
    a.ptr.p_double[0] = (double)(1);
    a.ptr.p_double[1] = (double)(-2);
    a.ptr.p_double[2] = (double)(1);
    polynomialsolve(&a, n, &x, &rep, _state);
    seterrorflag(&wereerrors, ae_fp_greater(ae_c_abs(ae_c_sub_d(x.ptr.p_complex[0],1), _state),eps), _state);
    seterrorflag(&wereerrors, ae_fp_greater(ae_c_abs(ae_c_sub_d(x.ptr.p_complex[1],1), _state),eps), _state);
    seterrorflag(&wereerrors, ae_fp_greater(rep.maxerr,100*ae_machineepsilon), _state);
    n = 2;
    ae_vector_set_length(&a, n+1, _state);
    a.ptr.p_double[0] = (double)(2);
    a.ptr.p_double[1] = (double)(-3);
    a.ptr.p_double[2] = (double)(1);
    polynomialsolve(&a, n, &x, &rep, _state);
    if( ae_fp_less(x.ptr.p_complex[0].x,x.ptr.p_complex[1].x) )
    {
        seterrorflag(&wereerrors, ae_fp_greater(ae_fabs(x.ptr.p_complex[0].x-1, _state),eps), _state);
        seterrorflag(&wereerrors, ae_fp_greater(ae_fabs(x.ptr.p_complex[1].x-2, _state),eps), _state);
    }
    else
    {
        seterrorflag(&wereerrors, ae_fp_greater(ae_fabs(x.ptr.p_complex[0].x-2, _state),eps), _state);
        seterrorflag(&wereerrors, ae_fp_greater(ae_fabs(x.ptr.p_complex[1].x-1, _state),eps), _state);
    }
    seterrorflag(&wereerrors, ae_fp_neq(x.ptr.p_complex[0].y,(double)(0)), _state);
    seterrorflag(&wereerrors, ae_fp_neq(x.ptr.p_complex[1].y,(double)(0)), _state);
    seterrorflag(&wereerrors, ae_fp_greater(rep.maxerr,100*ae_machineepsilon), _state);
    n = 2;
    ae_vector_set_length(&a, n+1, _state);
    a.ptr.p_double[0] = (double)(1);
    a.ptr.p_double[1] = (double)(0);
    a.ptr.p_double[2] = (double)(1);
    polynomialsolve(&a, n, &x, &rep, _state);
    seterrorflag(&wereerrors, ae_fp_greater(ae_c_abs(ae_c_add_d(ae_c_mul(x.ptr.p_complex[0],x.ptr.p_complex[0]),(double)(1)), _state),eps), _state);
    seterrorflag(&wereerrors, ae_fp_greater(rep.maxerr,100*ae_machineepsilon), _state);
    n = 4;
    ae_vector_set_length(&a, n+1, _state);
    a.ptr.p_double[0] = (double)(0);
    a.ptr.p_double[1] = (double)(0);
    a.ptr.p_double[2] = (double)(0);
    a.ptr.p_double[3] = (double)(0);
    a.ptr.p_double[4] = (double)(1);
    polynomialsolve(&a, n, &x, &rep, _state);
    seterrorflag(&wereerrors, ae_c_neq_d(x.ptr.p_complex[0],(double)(0)), _state);
    seterrorflag(&wereerrors, ae_c_neq_d(x.ptr.p_complex[1],(double)(0)), _state);
    seterrorflag(&wereerrors, ae_c_neq_d(x.ptr.p_complex[2],(double)(0)), _state);
    seterrorflag(&wereerrors, ae_c_neq_d(x.ptr.p_complex[3],(double)(0)), _state);
    seterrorflag(&wereerrors, ae_fp_greater(rep.maxerr,100*ae_machineepsilon), _state);
    n = 2;
    ae_vector_set_length(&a, n+1, _state);
    a.ptr.p_double[0] = (double)(0);
    a.ptr.p_double[1] = (double)(3);
    a.ptr.p_double[2] = (double)(2);
    polynomialsolve(&a, n, &x, &rep, _state);
    if( ae_fp_greater(x.ptr.p_complex[0].x,x.ptr.p_complex[1].x) )
    {
        seterrorflag(&wereerrors, ae_c_neq_d(x.ptr.p_complex[0],(double)(0)), _state);
        seterrorflag(&wereerrors, ae_fp_greater(ae_fabs(x.ptr.p_complex[1].x+(double)3/(double)2, _state),eps), _state);
        seterrorflag(&wereerrors, ae_fp_neq(x.ptr.p_complex[1].y,(double)(0)), _state);
    }
    else
    {
        seterrorflag(&wereerrors, ae_c_neq_d(x.ptr.p_complex[1],(double)(0)), _state);
        seterrorflag(&wereerrors, ae_fp_greater(ae_fabs(x.ptr.p_complex[0].x+(double)3/(double)2, _state),eps), _state);
        seterrorflag(&wereerrors, ae_fp_neq(x.ptr.p_complex[0].y,(double)(0)), _state);
    }
    seterrorflag(&wereerrors, ae_fp_greater(rep.maxerr,100*ae_machineepsilon), _state);
    if( !silent )
    {
        printf("TESTING POLYNOMIAL SOLVER\n");
        if( wereerrors )
        {
            printf("TEST FAILED\n");
        }
        else
        {
            printf("TEST PASSED\n");
        }
    }
    result = !wereerrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testpolynomialsolver(ae_bool silent, ae_state *_state)
{
    return testpolynomialsolver(silent, _state);
}


/*$ End $*/
