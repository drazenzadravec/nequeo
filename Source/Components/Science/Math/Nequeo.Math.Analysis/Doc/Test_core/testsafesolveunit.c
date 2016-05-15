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
#include "testsafesolveunit.h"


/*$ Declarations $*/
static void testsafesolveunit_rmatrixmakeacopy(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_state *_state);
static void testsafesolveunit_cmatrixmakeacopy(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Main unittest subroutine
*************************************************************************/
ae_bool testsafesolve(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t maxmn;
    double threshold;
    ae_bool rerrors;
    ae_bool cerrors;
    ae_bool waserrors;
    ae_bool isupper;
    ae_int_t trans;
    ae_bool isunit;
    double scalea;
    double growth;
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;
    ae_int_t j1;
    ae_int_t j2;
    ae_complex cv;
    ae_matrix ca;
    ae_matrix cea;
    ae_matrix ctmpa;
    ae_vector cxs;
    ae_vector cxe;
    double rv;
    ae_matrix ra;
    ae_matrix rea;
    ae_matrix rtmpa;
    ae_vector rxs;
    ae_vector rxe;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cea, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&ctmpa, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&cxs, 0, DT_COMPLEX, _state);
    ae_vector_init(&cxe, 0, DT_COMPLEX, _state);
    ae_matrix_init(&ra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&rea, 0, 0, DT_REAL, _state);
    ae_matrix_init(&rtmpa, 0, 0, DT_REAL, _state);
    ae_vector_init(&rxs, 0, DT_REAL, _state);
    ae_vector_init(&rxe, 0, DT_REAL, _state);

    maxmn = 30;
    threshold = 100000*ae_machineepsilon;
    rerrors = ae_false;
    cerrors = ae_false;
    waserrors = ae_false;
    
    /*
     * Different problems: general tests
     */
    for(n=1; n<=maxmn; n++)
    {
        
        /*
         * test complex solver with well-conditioned matrix:
         * 1. generate A: fill off-diagonal elements with small values,
         *    diagonal elements are filled with larger values
         * 2. generate 'effective' A
         * 3. prepare task (exact X is stored in CXE, right part - in CXS),
         *    solve and compare CXS and CXE
         */
        isupper = ae_fp_greater(ae_randomreal(_state),0.5);
        trans = ae_randominteger(3, _state);
        isunit = ae_fp_greater(ae_randomreal(_state),0.5);
        scalea = ae_randomreal(_state)+0.5;
        ae_matrix_set_length(&ca, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( i==j )
                {
                    ca.ptr.pp_complex[i][j].x = (2*ae_randominteger(2, _state)-1)*(5+ae_randomreal(_state));
                    ca.ptr.pp_complex[i][j].y = (2*ae_randominteger(2, _state)-1)*(5+ae_randomreal(_state));
                }
                else
                {
                    ca.ptr.pp_complex[i][j].x = 0.2*ae_randomreal(_state)-0.1;
                    ca.ptr.pp_complex[i][j].y = 0.2*ae_randomreal(_state)-0.1;
                }
            }
        }
        testsafesolveunit_cmatrixmakeacopy(&ca, n, n, &ctmpa, _state);
        for(i=0; i<=n-1; i++)
        {
            if( isupper )
            {
                j1 = 0;
                j2 = i-1;
            }
            else
            {
                j1 = i+1;
                j2 = n-1;
            }
            for(j=j1; j<=j2; j++)
            {
                ctmpa.ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
            if( isunit )
            {
                ctmpa.ptr.pp_complex[i][i] = ae_complex_from_i(1);
            }
        }
        ae_matrix_set_length(&cea, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            if( trans==0 )
            {
                ae_v_cmoved(&cea.ptr.pp_complex[i][0], 1, &ctmpa.ptr.pp_complex[i][0], 1, "N", ae_v_len(0,n-1), scalea);
            }
            if( trans==1 )
            {
                ae_v_cmoved(&cea.ptr.pp_complex[0][i], cea.stride, &ctmpa.ptr.pp_complex[i][0], 1, "N", ae_v_len(0,n-1), scalea);
            }
            if( trans==2 )
            {
                ae_v_cmoved(&cea.ptr.pp_complex[0][i], cea.stride, &ctmpa.ptr.pp_complex[i][0], 1, "Conj", ae_v_len(0,n-1), scalea);
            }
        }
        ae_vector_set_length(&cxe, n, _state);
        for(i=0; i<=n-1; i++)
        {
            cxe.ptr.p_complex[i].x = 2*ae_randomreal(_state)-1;
            cxe.ptr.p_complex[i].y = 2*ae_randomreal(_state)-1;
        }
        ae_vector_set_length(&cxs, n, _state);
        for(i=0; i<=n-1; i++)
        {
            cv = ae_v_cdotproduct(&cea.ptr.pp_complex[i][0], 1, "N", &cxe.ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
            cxs.ptr.p_complex[i] = cv;
        }
        if( cmatrixscaledtrsafesolve(&ca, scalea, n, &cxs, isupper, trans, isunit, ae_sqrt(ae_maxrealnumber, _state), _state) )
        {
            for(i=0; i<=n-1; i++)
            {
                cerrors = cerrors||ae_fp_greater(ae_c_abs(ae_c_sub(cxs.ptr.p_complex[i],cxe.ptr.p_complex[i]), _state),threshold);
            }
        }
        else
        {
            cerrors = ae_true;
        }
        
        /*
         * same with real
         */
        isupper = ae_fp_greater(ae_randomreal(_state),0.5);
        trans = ae_randominteger(2, _state);
        isunit = ae_fp_greater(ae_randomreal(_state),0.5);
        scalea = ae_randomreal(_state)+0.5;
        ae_matrix_set_length(&ra, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( i==j )
                {
                    ra.ptr.pp_double[i][j] = (2*ae_randominteger(2, _state)-1)*(5+ae_randomreal(_state));
                }
                else
                {
                    ra.ptr.pp_double[i][j] = 0.2*ae_randomreal(_state)-0.1;
                }
            }
        }
        testsafesolveunit_rmatrixmakeacopy(&ra, n, n, &rtmpa, _state);
        for(i=0; i<=n-1; i++)
        {
            if( isupper )
            {
                j1 = 0;
                j2 = i-1;
            }
            else
            {
                j1 = i+1;
                j2 = n-1;
            }
            for(j=j1; j<=j2; j++)
            {
                rtmpa.ptr.pp_double[i][j] = (double)(0);
            }
            if( isunit )
            {
                rtmpa.ptr.pp_double[i][i] = (double)(1);
            }
        }
        ae_matrix_set_length(&rea, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            if( trans==0 )
            {
                ae_v_moved(&rea.ptr.pp_double[i][0], 1, &rtmpa.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), scalea);
            }
            if( trans==1 )
            {
                ae_v_moved(&rea.ptr.pp_double[0][i], rea.stride, &rtmpa.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), scalea);
            }
        }
        ae_vector_set_length(&rxe, n, _state);
        for(i=0; i<=n-1; i++)
        {
            rxe.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        ae_vector_set_length(&rxs, n, _state);
        for(i=0; i<=n-1; i++)
        {
            rv = ae_v_dotproduct(&rea.ptr.pp_double[i][0], 1, &rxe.ptr.p_double[0], 1, ae_v_len(0,n-1));
            rxs.ptr.p_double[i] = rv;
        }
        if( rmatrixscaledtrsafesolve(&ra, scalea, n, &rxs, isupper, trans, isunit, ae_sqrt(ae_maxrealnumber, _state), _state) )
        {
            for(i=0; i<=n-1; i++)
            {
                rerrors = rerrors||ae_fp_greater(ae_fabs(rxs.ptr.p_double[i]-rxe.ptr.p_double[i], _state),threshold);
            }
        }
        else
        {
            rerrors = ae_true;
        }
    }
    
    /*
     * Special test with diagonal ill-conditioned matrix:
     * * ability to solve it when resulting growth is less than threshold
     * * ability to stop solve when resulting growth is greater than threshold
     *
     * A = diag(1, 1/growth)
     * b = (1, 0.5)
     */
    n = 2;
    growth = (double)(10);
    ae_matrix_set_length(&ca, n, n, _state);
    ca.ptr.pp_complex[0][0] = ae_complex_from_i(1);
    ca.ptr.pp_complex[0][1] = ae_complex_from_i(0);
    ca.ptr.pp_complex[1][0] = ae_complex_from_i(0);
    ca.ptr.pp_complex[1][1] = ae_complex_from_d(1/growth);
    ae_vector_set_length(&cxs, n, _state);
    cxs.ptr.p_complex[0] = ae_complex_from_d(1.0);
    cxs.ptr.p_complex[1] = ae_complex_from_d(0.5);
    cerrors = cerrors||!cmatrixscaledtrsafesolve(&ca, 1.0, n, &cxs, ae_fp_greater(ae_randomreal(_state),0.5), ae_randominteger(3, _state), ae_false, 1.05*ae_maxreal(ae_c_abs(cxs.ptr.p_complex[1], _state)*growth, 1.0, _state), _state);
    cerrors = cerrors||!cmatrixscaledtrsafesolve(&ca, 1.0, n, &cxs, ae_fp_greater(ae_randomreal(_state),0.5), ae_randominteger(3, _state), ae_false, 0.95*ae_maxreal(ae_c_abs(cxs.ptr.p_complex[1], _state)*growth, 1.0, _state), _state);
    ae_matrix_set_length(&ra, n, n, _state);
    ra.ptr.pp_double[0][0] = (double)(1);
    ra.ptr.pp_double[0][1] = (double)(0);
    ra.ptr.pp_double[1][0] = (double)(0);
    ra.ptr.pp_double[1][1] = 1/growth;
    ae_vector_set_length(&rxs, n, _state);
    rxs.ptr.p_double[0] = 1.0;
    rxs.ptr.p_double[1] = 0.5;
    rerrors = rerrors||!rmatrixscaledtrsafesolve(&ra, 1.0, n, &rxs, ae_fp_greater(ae_randomreal(_state),0.5), ae_randominteger(2, _state), ae_false, 1.05*ae_maxreal(ae_fabs(rxs.ptr.p_double[1], _state)*growth, 1.0, _state), _state);
    rerrors = rerrors||!rmatrixscaledtrsafesolve(&ra, 1.0, n, &rxs, ae_fp_greater(ae_randomreal(_state),0.5), ae_randominteger(2, _state), ae_false, 0.95*ae_maxreal(ae_fabs(rxs.ptr.p_double[1], _state)*growth, 1.0, _state), _state);
    
    /*
     * Special test with diagonal degenerate matrix:
     * * ability to solve it when resulting growth is less than threshold
     * * ability to stop solve when resulting growth is greater than threshold
     *
     * A = diag(1, 0)
     * b = (1, 0.5)
     */
    n = 2;
    ae_matrix_set_length(&ca, n, n, _state);
    ca.ptr.pp_complex[0][0] = ae_complex_from_i(1);
    ca.ptr.pp_complex[0][1] = ae_complex_from_i(0);
    ca.ptr.pp_complex[1][0] = ae_complex_from_i(0);
    ca.ptr.pp_complex[1][1] = ae_complex_from_i(0);
    ae_vector_set_length(&cxs, n, _state);
    cxs.ptr.p_complex[0] = ae_complex_from_d(1.0);
    cxs.ptr.p_complex[1] = ae_complex_from_d(0.5);
    cerrors = cerrors||cmatrixscaledtrsafesolve(&ca, 1.0, n, &cxs, ae_fp_greater(ae_randomreal(_state),0.5), ae_randominteger(3, _state), ae_false, ae_sqrt(ae_maxrealnumber, _state), _state);
    ae_matrix_set_length(&ra, n, n, _state);
    ra.ptr.pp_double[0][0] = (double)(1);
    ra.ptr.pp_double[0][1] = (double)(0);
    ra.ptr.pp_double[1][0] = (double)(0);
    ra.ptr.pp_double[1][1] = (double)(0);
    ae_vector_set_length(&rxs, n, _state);
    rxs.ptr.p_double[0] = 1.0;
    rxs.ptr.p_double[1] = 0.5;
    rerrors = rerrors||rmatrixscaledtrsafesolve(&ra, 1.0, n, &rxs, ae_fp_greater(ae_randomreal(_state),0.5), ae_randominteger(2, _state), ae_false, ae_sqrt(ae_maxrealnumber, _state), _state);
    
    /*
     * report
     */
    waserrors = rerrors||cerrors;
    if( !silent )
    {
        printf("TESTING SAFE TR SOLVER\n");
        printf("REAL:                                    ");
        if( !rerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("COMPLEX:                                 ");
        if( !cerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
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
ae_bool _pexec_testsafesolve(ae_bool silent, ae_state *_state)
{
    return testsafesolve(silent, _state);
}


/*************************************************************************
Copy
*************************************************************************/
static void testsafesolveunit_rmatrixmakeacopy(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(b);

    ae_matrix_set_length(b, m-1+1, n-1+1, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            b->ptr.pp_double[i][j] = a->ptr.pp_double[i][j];
        }
    }
}


/*************************************************************************
Copy
*************************************************************************/
static void testsafesolveunit_cmatrixmakeacopy(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(b);

    ae_matrix_set_length(b, m-1+1, n-1+1, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            b->ptr.pp_complex[i][j] = a->ptr.pp_complex[i][j];
        }
    }
}


/*$ End $*/
