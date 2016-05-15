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
#include "testsvdunit.h"


/*$ Declarations $*/
static void testsvdunit_fillsparsea(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double sparcity,
     ae_state *_state);
static void testsvdunit_getsvderror(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* u,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* vt,
     double* materr,
     double* orterr,
     ae_bool* wsorted,
     ae_state *_state);
static void testsvdunit_testsvdproblem(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double* materr,
     double* orterr,
     double* othererr,
     ae_bool* wsorted,
     ae_bool* wfailed,
     ae_int_t* failcount,
     ae_int_t* succcount,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Testing SVD decomposition subroutine
*************************************************************************/
ae_bool testsvd(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_int_t m;
    ae_int_t n;
    ae_int_t maxmn;
    ae_int_t i;
    ae_int_t j;
    ae_int_t gpass;
    ae_int_t pass;
    ae_bool waserrors;
    ae_bool wsorted;
    ae_bool wfailed;
    double materr;
    double orterr;
    double othererr;
    double threshold;
    double failr;
    ae_int_t failcount;
    ae_int_t succcount;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);

    failcount = 0;
    succcount = 0;
    materr = (double)(0);
    orterr = (double)(0);
    othererr = (double)(0);
    wsorted = ae_true;
    wfailed = ae_false;
    waserrors = ae_false;
    maxmn = 30;
    threshold = 5*100*ae_machineepsilon;
    ae_matrix_set_length(&a, maxmn-1+1, maxmn-1+1, _state);
    
    /*
     * TODO: div by zero fail, convergence fail
     */
    for(gpass=1; gpass<=1; gpass++)
    {
        
        /*
         * zero matrix, several cases
         */
        for(i=0; i<=maxmn-1; i++)
        {
            for(j=0; j<=maxmn-1; j++)
            {
                a.ptr.pp_double[i][j] = (double)(0);
            }
        }
        for(i=1; i<=ae_minint(5, maxmn, _state); i++)
        {
            for(j=1; j<=ae_minint(5, maxmn, _state); j++)
            {
                testsvdunit_testsvdproblem(&a, i, j, &materr, &orterr, &othererr, &wsorted, &wfailed, &failcount, &succcount, _state);
            }
        }
        
        /*
         * Long dense matrix
         */
        for(i=0; i<=maxmn-1; i++)
        {
            for(j=0; j<=ae_minint(5, maxmn, _state)-1; j++)
            {
                a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
        }
        for(i=1; i<=maxmn; i++)
        {
            for(j=1; j<=ae_minint(5, maxmn, _state); j++)
            {
                testsvdunit_testsvdproblem(&a, i, j, &materr, &orterr, &othererr, &wsorted, &wfailed, &failcount, &succcount, _state);
            }
        }
        for(i=0; i<=ae_minint(5, maxmn, _state)-1; i++)
        {
            for(j=0; j<=maxmn-1; j++)
            {
                a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
        }
        for(i=1; i<=ae_minint(5, maxmn, _state); i++)
        {
            for(j=1; j<=maxmn; j++)
            {
                testsvdunit_testsvdproblem(&a, i, j, &materr, &orterr, &othererr, &wsorted, &wfailed, &failcount, &succcount, _state);
            }
        }
        
        /*
         * Dense matrices
         */
        for(m=1; m<=ae_minint(10, maxmn, _state); m++)
        {
            for(n=1; n<=ae_minint(10, maxmn, _state); n++)
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    }
                }
                testsvdunit_testsvdproblem(&a, m, n, &materr, &orterr, &othererr, &wsorted, &wfailed, &failcount, &succcount, _state);
            }
        }
        
        /*
         * Sparse matrices, very sparse matrices, incredible sparse matrices
         */
        for(m=1; m<=10; m++)
        {
            for(n=1; n<=10; n++)
            {
                for(pass=1; pass<=2; pass++)
                {
                    testsvdunit_fillsparsea(&a, m, n, 0.8, _state);
                    testsvdunit_testsvdproblem(&a, m, n, &materr, &orterr, &othererr, &wsorted, &wfailed, &failcount, &succcount, _state);
                    testsvdunit_fillsparsea(&a, m, n, 0.9, _state);
                    testsvdunit_testsvdproblem(&a, m, n, &materr, &orterr, &othererr, &wsorted, &wfailed, &failcount, &succcount, _state);
                    testsvdunit_fillsparsea(&a, m, n, 0.95, _state);
                    testsvdunit_testsvdproblem(&a, m, n, &materr, &orterr, &othererr, &wsorted, &wfailed, &failcount, &succcount, _state);
                }
            }
        }
    }
    
    /*
     * report
     */
    failr = (double)failcount/(double)(succcount+failcount);
    waserrors = (((wfailed||ae_fp_greater(materr,threshold))||ae_fp_greater(orterr,threshold))||ae_fp_greater(othererr,threshold))||!wsorted;
    if( !silent )
    {
        printf("TESTING SVD DECOMPOSITION\n");
        printf("SVD decomposition error:                 %5.3e\n",
            (double)(materr));
        printf("SVD orthogonality error:                 %5.3e\n",
            (double)(orterr));
        printf("SVD with different parameters error:     %5.3e\n",
            (double)(othererr));
        printf("Singular values order:                   ");
        if( wsorted )
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
            printf("YES\n");
        }
        else
        {
            printf("NO\n");
            printf("Fail ratio:                              %5.3f\n",
                (double)(failr));
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
ae_bool _pexec_testsvd(ae_bool silent, ae_state *_state)
{
    return testsvd(silent, _state);
}


static void testsvdunit_fillsparsea(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double sparcity,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=m-1; i++)
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


static void testsvdunit_getsvderror(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* u,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* vt,
     double* materr,
     double* orterr,
     ae_bool* wsorted,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t minmn;
    double locerr;
    double sm;


    minmn = ae_minint(m, n, _state);
    
    /*
     * decomposition error
     */
    locerr = (double)(0);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            sm = (double)(0);
            for(k=0; k<=minmn-1; k++)
            {
                sm = sm+w->ptr.p_double[k]*u->ptr.pp_double[i][k]*vt->ptr.pp_double[k][j];
            }
            locerr = ae_maxreal(locerr, ae_fabs(a->ptr.pp_double[i][j]-sm, _state), _state);
        }
    }
    *materr = ae_maxreal(*materr, locerr, _state);
    
    /*
     * orthogonality error
     */
    locerr = (double)(0);
    for(i=0; i<=minmn-1; i++)
    {
        for(j=i; j<=minmn-1; j++)
        {
            sm = ae_v_dotproduct(&u->ptr.pp_double[0][i], u->stride, &u->ptr.pp_double[0][j], u->stride, ae_v_len(0,m-1));
            if( i!=j )
            {
                locerr = ae_maxreal(locerr, ae_fabs(sm, _state), _state);
            }
            else
            {
                locerr = ae_maxreal(locerr, ae_fabs(sm-1, _state), _state);
            }
            sm = ae_v_dotproduct(&vt->ptr.pp_double[i][0], 1, &vt->ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
            if( i!=j )
            {
                locerr = ae_maxreal(locerr, ae_fabs(sm, _state), _state);
            }
            else
            {
                locerr = ae_maxreal(locerr, ae_fabs(sm-1, _state), _state);
            }
        }
    }
    *orterr = ae_maxreal(*orterr, locerr, _state);
    
    /*
     * values order error
     */
    for(i=1; i<=minmn-1; i++)
    {
        if( ae_fp_greater(w->ptr.p_double[i],w->ptr.p_double[i-1]) )
        {
            *wsorted = ae_false;
        }
    }
}


static void testsvdunit_testsvdproblem(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double* materr,
     double* orterr,
     double* othererr,
     ae_bool* wsorted,
     ae_bool* wfailed,
     ae_int_t* failcount,
     ae_int_t* succcount,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix u;
    ae_matrix vt;
    ae_matrix u2;
    ae_matrix vt2;
    ae_vector w;
    ae_vector w2;
    ae_int_t i;
    ae_int_t j;
    ae_int_t ujob;
    ae_int_t vtjob;
    ae_int_t memjob;
    ae_int_t ucheck;
    ae_int_t vtcheck;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt, 0, 0, DT_REAL, _state);
    ae_matrix_init(&u2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt2, 0, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);

    
    /*
     * Main SVD test
     */
    if( !rmatrixsvd(a, m, n, 2, 2, 2, &w, &u, &vt, _state) )
    {
        *failcount = *failcount+1;
        *wfailed = ae_true;
        ae_frame_leave(_state);
        return;
    }
    testsvdunit_getsvderror(a, m, n, &u, &w, &vt, materr, orterr, wsorted, _state);
    
    /*
     * Additional SVD tests
     */
    for(ujob=0; ujob<=2; ujob++)
    {
        for(vtjob=0; vtjob<=2; vtjob++)
        {
            for(memjob=0; memjob<=2; memjob++)
            {
                if( !rmatrixsvd(a, m, n, ujob, vtjob, memjob, &w2, &u2, &vt2, _state) )
                {
                    *failcount = *failcount+1;
                    *wfailed = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                ucheck = 0;
                if( ujob==1 )
                {
                    ucheck = ae_minint(m, n, _state);
                }
                if( ujob==2 )
                {
                    ucheck = m;
                }
                vtcheck = 0;
                if( vtjob==1 )
                {
                    vtcheck = ae_minint(m, n, _state);
                }
                if( vtjob==2 )
                {
                    vtcheck = n;
                }
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=ucheck-1; j++)
                    {
                        *othererr = ae_maxreal(*othererr, ae_fabs(u.ptr.pp_double[i][j]-u2.ptr.pp_double[i][j], _state), _state);
                    }
                }
                for(i=0; i<=vtcheck-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        *othererr = ae_maxreal(*othererr, ae_fabs(vt.ptr.pp_double[i][j]-vt2.ptr.pp_double[i][j], _state), _state);
                    }
                }
                for(i=0; i<=ae_minint(m, n, _state)-1; i++)
                {
                    *othererr = ae_maxreal(*othererr, ae_fabs(w.ptr.p_double[i]-w2.ptr.p_double[i], _state), _state);
                }
            }
        }
    }
    
    /*
     * update counter
     */
    *succcount = *succcount+1;
    ae_frame_leave(_state);
}


/*$ End $*/
