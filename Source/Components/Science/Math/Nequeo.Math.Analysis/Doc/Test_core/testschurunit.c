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
#include "testschurunit.h"


/*$ Declarations $*/
static void testschurunit_fillsparsea(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double sparcity,
     ae_state *_state);
static void testschurunit_testschurproblem(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double* materr,
     double* orterr,
     ae_bool* errstruct,
     ae_bool* wfailed,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Testing Schur decomposition subroutine
*************************************************************************/
ae_bool testschur(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_int_t n;
    ae_int_t maxn;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_int_t passcount;
    ae_bool waserrors;
    ae_bool errstruct;
    ae_bool wfailed;
    double materr;
    double orterr;
    double threshold;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);

    materr = (double)(0);
    orterr = (double)(0);
    errstruct = ae_false;
    wfailed = ae_false;
    waserrors = ae_false;
    maxn = 70;
    passcount = 1;
    threshold = 5*100*ae_machineepsilon;
    ae_matrix_set_length(&a, maxn-1+1, maxn-1+1, _state);
    
    /*
     * zero matrix, several cases
     */
    for(i=0; i<=maxn-1; i++)
    {
        for(j=0; j<=maxn-1; j++)
        {
            a.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(n=1; n<=maxn; n++)
    {
        if( n>30&&n%2==0 )
        {
            continue;
        }
        testschurunit_testschurproblem(&a, n, &materr, &orterr, &errstruct, &wfailed, _state);
    }
    
    /*
     * Dense matrix
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            if( n>30&&n%2==0 )
            {
                continue;
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                }
            }
            testschurunit_testschurproblem(&a, n, &materr, &orterr, &errstruct, &wfailed, _state);
        }
    }
    
    /*
     * Sparse matrices, very sparse matrices, incredible sparse matrices
     */
    for(pass=1; pass<=1; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            if( n>30&&n%3!=0 )
            {
                continue;
            }
            testschurunit_fillsparsea(&a, n, 0.8, _state);
            testschurunit_testschurproblem(&a, n, &materr, &orterr, &errstruct, &wfailed, _state);
            testschurunit_fillsparsea(&a, n, 0.9, _state);
            testschurunit_testschurproblem(&a, n, &materr, &orterr, &errstruct, &wfailed, _state);
            testschurunit_fillsparsea(&a, n, 0.95, _state);
            testschurunit_testschurproblem(&a, n, &materr, &orterr, &errstruct, &wfailed, _state);
            testschurunit_fillsparsea(&a, n, 0.997, _state);
            testschurunit_testschurproblem(&a, n, &materr, &orterr, &errstruct, &wfailed, _state);
        }
    }
    
    /*
     * report
     */
    waserrors = ((ae_fp_greater(materr,threshold)||ae_fp_greater(orterr,threshold))||errstruct)||wfailed;
    if( !silent )
    {
        printf("TESTING SCHUR DECOMPOSITION\n");
        printf("Schur decomposition error:               %5.3e\n",
            (double)(materr));
        printf("Schur orthogonality error:               %5.3e\n",
            (double)(orterr));
        printf("T matrix structure:                      ");
        if( !errstruct )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("Always converged:                        ");
        if( !wfailed )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
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
ae_bool _pexec_testschur(ae_bool silent, ae_state *_state)
{
    return testschur(silent, _state);
}


static void testschurunit_fillsparsea(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double sparcity,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( ae_fp_greater_eq(ae_randomreal(_state),sparcity) )
            {
                a->ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            else
            {
                a->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
}


static void testschurunit_testschurproblem(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double* materr,
     double* orterr,
     ae_bool* errstruct,
     ae_bool* wfailed,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix s;
    ae_matrix t;
    ae_vector sr;
    ae_vector astc;
    ae_vector sastc;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    double locerr;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&s, 0, 0, DT_REAL, _state);
    ae_matrix_init(&t, 0, 0, DT_REAL, _state);
    ae_vector_init(&sr, 0, DT_REAL, _state);
    ae_vector_init(&astc, 0, DT_REAL, _state);
    ae_vector_init(&sastc, 0, DT_REAL, _state);

    ae_vector_set_length(&sr, n-1+1, _state);
    ae_vector_set_length(&astc, n-1+1, _state);
    ae_vector_set_length(&sastc, n-1+1, _state);
    
    /*
     * Schur decomposition, convergence test
     */
    ae_matrix_set_length(&t, n-1+1, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            t.ptr.pp_double[i][j] = a->ptr.pp_double[i][j];
        }
    }
    if( !rmatrixschur(&t, n, &s, _state) )
    {
        *wfailed = ae_true;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * decomposition error
     */
    locerr = (double)(0);
    for(j=0; j<=n-1; j++)
    {
        ae_v_move(&sr.ptr.p_double[0], 1, &s.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
        for(k=0; k<=n-1; k++)
        {
            v = ae_v_dotproduct(&t.ptr.pp_double[k][0], 1, &sr.ptr.p_double[0], 1, ae_v_len(0,n-1));
            astc.ptr.p_double[k] = v;
        }
        for(k=0; k<=n-1; k++)
        {
            v = ae_v_dotproduct(&s.ptr.pp_double[k][0], 1, &astc.ptr.p_double[0], 1, ae_v_len(0,n-1));
            sastc.ptr.p_double[k] = v;
        }
        for(k=0; k<=n-1; k++)
        {
            locerr = ae_maxreal(locerr, ae_fabs(sastc.ptr.p_double[k]-a->ptr.pp_double[k][j], _state), _state);
        }
    }
    *materr = ae_maxreal(*materr, locerr, _state);
    
    /*
     * orthogonality error
     */
    locerr = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&s.ptr.pp_double[0][i], s.stride, &s.ptr.pp_double[0][j], s.stride, ae_v_len(0,n-1));
            if( i!=j )
            {
                locerr = ae_maxreal(locerr, ae_fabs(v, _state), _state);
            }
            else
            {
                locerr = ae_maxreal(locerr, ae_fabs(v-1, _state), _state);
            }
        }
    }
    *orterr = ae_maxreal(*orterr, locerr, _state);
    
    /*
     * T matrix structure
     */
    for(j=0; j<=n-1; j++)
    {
        for(i=j+2; i<=n-1; i++)
        {
            if( ae_fp_neq(t.ptr.pp_double[i][j],(double)(0)) )
            {
                *errstruct = ae_true;
            }
        }
    }
    ae_frame_leave(_state);
}


/*$ End $*/
