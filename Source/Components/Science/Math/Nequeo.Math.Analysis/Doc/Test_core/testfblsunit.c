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
#include "testfblsunit.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Testing
*************************************************************************/
ae_bool testfbls(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t m;
    ae_int_t mx;
    ae_int_t i;
    ae_int_t j;
    ae_bool waserrors;
    ae_bool cgerrors;
    ae_bool lserrors;
    double eps;
    double v;
    double v1;
    double v2;
    ae_vector tmp0;
    ae_vector tmp1;
    ae_vector tmp2;
    ae_matrix a;
    ae_vector b;
    ae_vector x;
    ae_vector xe;
    ae_vector buf;
    double alpha;
    double e1;
    double e2;
    fblslincgstate cgstate;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&tmp0, 0, DT_REAL, _state);
    ae_vector_init(&tmp1, 0, DT_REAL, _state);
    ae_vector_init(&tmp2, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&xe, 0, DT_REAL, _state);
    ae_vector_init(&buf, 0, DT_REAL, _state);
    _fblslincgstate_init(&cgstate, _state);

    mx = 10;
    waserrors = ae_false;
    cgerrors = ae_false;
    lserrors = ae_false;
    
    /*
     * Test CG solver:
     * * generate problem (A, B, Alpha, XE - exact solution) and initial approximation X
     * * E1 = ||A'A*x-b||
     * * solve
     * * E2 = ||A'A*x-b||
     * * test that E2<0.001*E1
     */
    for(n=1; n<=mx; n++)
    {
        for(m=1; m<=mx; m++)
        {
            ae_matrix_set_length(&a, m, n, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&xe, n, _state);
            ae_vector_set_length(&tmp1, m, _state);
            ae_vector_set_length(&tmp2, n, _state);
            
            /*
             * init A, alpha, B, X (initial approximation), XE (exact solution)
             * X is initialized in such way that is has no chances to be equal to XE.
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                }
            }
            alpha = ae_randomreal(_state)+0.1;
            for(i=0; i<=n-1; i++)
            {
                b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xe.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                x.ptr.p_double[i] = (2*ae_randominteger(2, _state)-1)*(2+ae_randomreal(_state));
            }
            
            /*
             * Test dense CG (which solves A'A*x=b and accepts dense A)
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = (2*ae_randominteger(2, _state)-1)*(2+ae_randomreal(_state));
            }
            rmatrixmv(m, n, &a, 0, 0, 0, &x, 0, &tmp1, 0, _state);
            rmatrixmv(n, m, &a, 0, 0, 1, &tmp1, 0, &tmp2, 0, _state);
            ae_v_addd(&tmp2.ptr.p_double[0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1), alpha);
            ae_v_sub(&tmp2.ptr.p_double[0], 1, &b.ptr.p_double[0], 1, ae_v_len(0,n-1));
            v = ae_v_dotproduct(&tmp2.ptr.p_double[0], 1, &tmp2.ptr.p_double[0], 1, ae_v_len(0,n-1));
            e1 = ae_sqrt(v, _state);
            fblssolvecgx(&a, m, n, alpha, &b, &x, &buf, _state);
            rmatrixmv(m, n, &a, 0, 0, 0, &x, 0, &tmp1, 0, _state);
            rmatrixmv(n, m, &a, 0, 0, 1, &tmp1, 0, &tmp2, 0, _state);
            ae_v_addd(&tmp2.ptr.p_double[0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1), alpha);
            ae_v_sub(&tmp2.ptr.p_double[0], 1, &b.ptr.p_double[0], 1, ae_v_len(0,n-1));
            v = ae_v_dotproduct(&tmp2.ptr.p_double[0], 1, &tmp2.ptr.p_double[0], 1, ae_v_len(0,n-1));
            e2 = ae_sqrt(v, _state);
            cgerrors = cgerrors||ae_fp_greater(e2,0.001*e1);
            
            /*
             * Test sparse CG (which relies on reverse communication)
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = (2*ae_randominteger(2, _state)-1)*(2+ae_randomreal(_state));
            }
            rmatrixmv(m, n, &a, 0, 0, 0, &x, 0, &tmp1, 0, _state);
            rmatrixmv(n, m, &a, 0, 0, 1, &tmp1, 0, &tmp2, 0, _state);
            ae_v_addd(&tmp2.ptr.p_double[0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1), alpha);
            ae_v_sub(&tmp2.ptr.p_double[0], 1, &b.ptr.p_double[0], 1, ae_v_len(0,n-1));
            v = ae_v_dotproduct(&tmp2.ptr.p_double[0], 1, &tmp2.ptr.p_double[0], 1, ae_v_len(0,n-1));
            e1 = ae_sqrt(v, _state);
            fblscgcreate(&x, &b, n, &cgstate, _state);
            while(fblscgiteration(&cgstate, _state))
            {
                rmatrixmv(m, n, &a, 0, 0, 0, &cgstate.x, 0, &tmp1, 0, _state);
                rmatrixmv(n, m, &a, 0, 0, 1, &tmp1, 0, &cgstate.ax, 0, _state);
                ae_v_addd(&cgstate.ax.ptr.p_double[0], 1, &cgstate.x.ptr.p_double[0], 1, ae_v_len(0,n-1), alpha);
                v1 = ae_v_dotproduct(&tmp1.ptr.p_double[0], 1, &tmp1.ptr.p_double[0], 1, ae_v_len(0,m-1));
                v2 = ae_v_dotproduct(&cgstate.x.ptr.p_double[0], 1, &cgstate.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                cgstate.xax = v1+alpha*v2;
            }
            rmatrixmv(m, n, &a, 0, 0, 0, &cgstate.xk, 0, &tmp1, 0, _state);
            rmatrixmv(n, m, &a, 0, 0, 1, &tmp1, 0, &tmp2, 0, _state);
            ae_v_addd(&tmp2.ptr.p_double[0], 1, &cgstate.xk.ptr.p_double[0], 1, ae_v_len(0,n-1), alpha);
            ae_v_sub(&tmp2.ptr.p_double[0], 1, &b.ptr.p_double[0], 1, ae_v_len(0,n-1));
            v = ae_v_dotproduct(&tmp2.ptr.p_double[0], 1, &tmp2.ptr.p_double[0], 1, ae_v_len(0,n-1));
            e2 = ae_sqrt(v, _state);
            cgerrors = cgerrors||ae_fp_greater(ae_fabs(e1-cgstate.e1, _state),100*ae_machineepsilon*e1);
            cgerrors = cgerrors||ae_fp_greater(ae_fabs(e2-cgstate.e2, _state),100*ae_machineepsilon*e1);
            cgerrors = cgerrors||ae_fp_greater(e2,0.001*e1);
        }
    }
    
    /*
     * Test linear least squares:
     * * try N=1..5, M=N..2*N
     *                           [ B ]
     * * generate MxN matrix A = [   ], where (M-N)xN submatrix B contains
     *                           [ C ]
     *   random values from [-1,+1], and NxN submatrix C is diagonally dominant
     *   (diagonal of C is equal to 1.0, and magnitude of off-diagonal elements
     *   is smaller than 0.01). Such matrix is guaranteed to be non-degenerate.
     * * generate random known solution xe, set right part b=A*xe
     * * check that results of FBLSSolveLS agree with xe
     */
    eps = 1.0E-6;
    for(n=1; n<=5; n++)
    {
        for(m=n; m<=2*n; m++)
        {
            ae_matrix_set_length(&a, m, n, _state);
            for(i=0; i<=m-n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                }
            }
            for(i=m-n; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 0.01*(2*ae_randomreal(_state)-1);
                }
                a.ptr.pp_double[i][i-(m-n)] = 1.0;
            }
            ae_vector_set_length(&xe, n, _state);
            for(i=0; i<=n-1; i++)
            {
                xe.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            ae_vector_set_length(&b, m, _state);
            for(i=0; i<=m-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xe.ptr.p_double[0], 1, ae_v_len(0,n-1));
                b.ptr.p_double[i] = v;
            }
            fblssolvels(&a, &b, m, n, &tmp0, &tmp1, &tmp2, _state);
            for(i=0; i<=n-1; i++)
            {
                lserrors = lserrors||ae_fp_greater(ae_fabs(b.ptr.p_double[i]-xe.ptr.p_double[i], _state),eps);
            }
        }
    }
    
    /*
     * report
     */
    waserrors = cgerrors||lserrors;
    if( !silent )
    {
        printf("TESTING FBLS\n");
        printf("CG ERRORS:                               ");
        if( cgerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("LS ERRORS:                               ");
        if( lserrors )
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
    result = !waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testfbls(ae_bool silent, ae_state *_state)
{
    return testfbls(silent, _state);
}


/*$ End $*/
