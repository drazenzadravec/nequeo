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
#include "testbdsvdunit.h"


/*$ Declarations $*/
static void testbdsvdunit_fillidentity(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state);
static void testbdsvdunit_fillsparsede(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     double sparcity,
     ae_state *_state);
static void testbdsvdunit_getbdsvderror(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_matrix* u,
     /* Real    */ ae_matrix* c,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* vt,
     double* materr,
     double* orterr,
     ae_bool* wsorted,
     ae_state *_state);
static void testbdsvdunit_checksvdmultiplication(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_matrix* u,
     /* Real    */ ae_matrix* c,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* vt,
     double* err,
     ae_state *_state);
static void testbdsvdunit_testbdsvdproblem(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     double* materr,
     double* orterr,
     ae_bool* wsorted,
     ae_bool* wfailed,
     ae_int_t* failcount,
     ae_int_t* succcount,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Testing bidiagonal SVD decomposition subroutine
*************************************************************************/
ae_bool testbdsvd(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector d;
    ae_vector e;
    ae_matrix mempty;
    ae_int_t n;
    ae_int_t maxn;
    ae_int_t i;
    ae_int_t pass;
    ae_bool waserrors;
    ae_bool wsorted;
    ae_bool wfailed;
    double materr;
    double orterr;
    double threshold;
    double failr;
    ae_int_t failcount;
    ae_int_t succcount;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);
    ae_matrix_init(&mempty, 0, 0, DT_REAL, _state);

    failcount = 0;
    succcount = 0;
    materr = (double)(0);
    orterr = (double)(0);
    wsorted = ae_true;
    wfailed = ae_false;
    waserrors = ae_false;
    maxn = 15;
    threshold = 5*100*ae_machineepsilon;
    ae_vector_set_length(&d, maxn-1+1, _state);
    ae_vector_set_length(&e, maxn-2+1, _state);
    
    /*
     * special case: zero divide matrix
     * unfixed LAPACK routine should fail on this problem
     */
    n = 7;
    d.ptr.p_double[0] = -6.96462904751731892700e-01;
    d.ptr.p_double[1] = 0.00000000000000000000e+00;
    d.ptr.p_double[2] = -5.73827770385971991400e-01;
    d.ptr.p_double[3] = -6.62562624399371191700e-01;
    d.ptr.p_double[4] = 5.82737148001782223600e-01;
    d.ptr.p_double[5] = 3.84825263580925003300e-01;
    d.ptr.p_double[6] = 9.84087420830525472200e-01;
    e.ptr.p_double[0] = -7.30307931760612871800e-02;
    e.ptr.p_double[1] = -2.30079042939542843800e-01;
    e.ptr.p_double[2] = -6.87824621739351216300e-01;
    e.ptr.p_double[3] = -1.77306437707837570600e-02;
    e.ptr.p_double[4] = 1.78285126526551632000e-15;
    e.ptr.p_double[5] = -4.89434737751289969400e-02;
    rmatrixbdsvd(&d, &e, n, ae_true, ae_false, &mempty, 0, &mempty, 0, &mempty, 0, _state);
    
    /*
     * zero matrix, several cases
     */
    for(i=0; i<=maxn-1; i++)
    {
        d.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=maxn-2; i++)
    {
        e.ptr.p_double[i] = (double)(0);
    }
    for(n=1; n<=maxn; n++)
    {
        testbdsvdunit_testbdsvdproblem(&d, &e, n, &materr, &orterr, &wsorted, &wfailed, &failcount, &succcount, _state);
    }
    
    /*
     * Dense matrix
     */
    for(n=1; n<=maxn; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            for(i=0; i<=maxn-1; i++)
            {
                d.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=maxn-2; i++)
            {
                e.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            testbdsvdunit_testbdsvdproblem(&d, &e, n, &materr, &orterr, &wsorted, &wfailed, &failcount, &succcount, _state);
        }
    }
    
    /*
     * Sparse matrices, very sparse matrices, incredible sparse matrices
     */
    for(n=1; n<=maxn; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            testbdsvdunit_fillsparsede(&d, &e, n, 0.5, _state);
            testbdsvdunit_testbdsvdproblem(&d, &e, n, &materr, &orterr, &wsorted, &wfailed, &failcount, &succcount, _state);
            testbdsvdunit_fillsparsede(&d, &e, n, 0.8, _state);
            testbdsvdunit_testbdsvdproblem(&d, &e, n, &materr, &orterr, &wsorted, &wfailed, &failcount, &succcount, _state);
            testbdsvdunit_fillsparsede(&d, &e, n, 0.9, _state);
            testbdsvdunit_testbdsvdproblem(&d, &e, n, &materr, &orterr, &wsorted, &wfailed, &failcount, &succcount, _state);
            testbdsvdunit_fillsparsede(&d, &e, n, 0.95, _state);
            testbdsvdunit_testbdsvdproblem(&d, &e, n, &materr, &orterr, &wsorted, &wfailed, &failcount, &succcount, _state);
        }
    }
    
    /*
     * report
     */
    failr = (double)failcount/(double)(succcount+failcount);
    waserrors = ((wfailed||ae_fp_greater(materr,threshold))||ae_fp_greater(orterr,threshold))||!wsorted;
    if( !silent )
    {
        printf("TESTING BIDIAGONAL SVD DECOMPOSITION\n");
        printf("SVD decomposition error:                 %5.3e\n",
            (double)(materr));
        printf("SVD orthogonality error:                 %5.3e\n",
            (double)(orterr));
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
ae_bool _pexec_testbdsvd(ae_bool silent, ae_state *_state)
{
    return testbdsvd(silent, _state);
}


static void testbdsvdunit_fillidentity(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    ae_matrix_set_length(a, n-1+1, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i==j )
            {
                a->ptr.pp_double[i][j] = (double)(1);
            }
            else
            {
                a->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
}


static void testbdsvdunit_fillsparsede(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     double sparcity,
     ae_state *_state)
{
    ae_int_t i;


    ae_vector_set_length(d, n-1+1, _state);
    ae_vector_set_length(e, ae_maxint(0, n-2, _state)+1, _state);
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_greater_eq(ae_randomreal(_state),sparcity) )
        {
            d->ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        else
        {
            d->ptr.p_double[i] = (double)(0);
        }
    }
    for(i=0; i<=n-2; i++)
    {
        if( ae_fp_greater_eq(ae_randomreal(_state),sparcity) )
        {
            e->ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        else
        {
            e->ptr.p_double[i] = (double)(0);
        }
    }
}


static void testbdsvdunit_getbdsvderror(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_matrix* u,
     /* Real    */ ae_matrix* c,
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
    double locerr;
    double sm;


    
    /*
     * decomposition error
     */
    locerr = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            sm = (double)(0);
            for(k=0; k<=n-1; k++)
            {
                sm = sm+w->ptr.p_double[k]*u->ptr.pp_double[i][k]*vt->ptr.pp_double[k][j];
            }
            if( isupper )
            {
                if( i==j )
                {
                    locerr = ae_maxreal(locerr, ae_fabs(d->ptr.p_double[i]-sm, _state), _state);
                }
                else
                {
                    if( i==j-1 )
                    {
                        locerr = ae_maxreal(locerr, ae_fabs(e->ptr.p_double[i]-sm, _state), _state);
                    }
                    else
                    {
                        locerr = ae_maxreal(locerr, ae_fabs(sm, _state), _state);
                    }
                }
            }
            else
            {
                if( i==j )
                {
                    locerr = ae_maxreal(locerr, ae_fabs(d->ptr.p_double[i]-sm, _state), _state);
                }
                else
                {
                    if( i-1==j )
                    {
                        locerr = ae_maxreal(locerr, ae_fabs(e->ptr.p_double[j]-sm, _state), _state);
                    }
                    else
                    {
                        locerr = ae_maxreal(locerr, ae_fabs(sm, _state), _state);
                    }
                }
            }
        }
    }
    *materr = ae_maxreal(*materr, locerr, _state);
    
    /*
     * check for C = U'
     * we consider it as decomposition error
     */
    locerr = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            locerr = ae_maxreal(locerr, ae_fabs(u->ptr.pp_double[i][j]-c->ptr.pp_double[j][i], _state), _state);
        }
    }
    *materr = ae_maxreal(*materr, locerr, _state);
    
    /*
     * orthogonality error
     */
    locerr = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        for(j=i; j<=n-1; j++)
        {
            sm = ae_v_dotproduct(&u->ptr.pp_double[0][i], u->stride, &u->ptr.pp_double[0][j], u->stride, ae_v_len(0,n-1));
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
    for(i=1; i<=n-1; i++)
    {
        if( ae_fp_greater(w->ptr.p_double[i],w->ptr.p_double[i-1]) )
        {
            *wsorted = ae_false;
        }
    }
}


static void testbdsvdunit_checksvdmultiplication(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_matrix* u,
     /* Real    */ ae_matrix* c,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* vt,
     double* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector wt;
    ae_matrix u2;
    ae_matrix c2;
    ae_matrix vt2;
    ae_matrix u1;
    ae_matrix c1;
    ae_matrix vt1;
    ae_int_t nru;
    ae_int_t ncc;
    ae_int_t ncvt;
    ae_int_t pass;
    hqrndstate rs;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&wt, 0, DT_REAL, _state);
    ae_matrix_init(&u2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&u1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt1, 0, 0, DT_REAL, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    ae_vector_set_length(&wt, n, _state);
    
    /*
     * Perform nonsquare SVD
     */
    for(pass=1; pass<=20; pass++)
    {
        
        /*
         * Problem size
         */
        nru = hqrnduniformi(&rs, 2*n, _state);
        ncc = hqrnduniformi(&rs, 2*n, _state);
        ncvt = hqrnduniformi(&rs, 2*n, _state);
        
        /*
         * Reference matrices (copy 1) and working matrices (copy 2)
         */
        for(i=0; i<=n-1; i++)
        {
            wt.ptr.p_double[i] = d->ptr.p_double[i];
        }
        if( nru>0 )
        {
            
            /*
             * init U1/U2
             */
            ae_matrix_set_length(&u1, nru, n, _state);
            ae_matrix_set_length(&u2, nru, n, _state);
            for(i=0; i<=u1.rows-1; i++)
            {
                for(j=0; j<=u1.cols-1; j++)
                {
                    u1.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                    u2.ptr.pp_double[i][j] = u1.ptr.pp_double[i][j];
                }
            }
        }
        else
        {
            
            /*
             * Set U1/U2 to 1x1 matrices; working with 1x1 matrices allows
             * to test correctness of code which passes them to MKL.
             */
            ae_matrix_set_length(&u1, 1, 1, _state);
            ae_matrix_set_length(&u2, 1, 1, _state);
        }
        if( ncc>0 )
        {
            ae_matrix_set_length(&c1, n, ncc, _state);
            ae_matrix_set_length(&c2, n, ncc, _state);
            for(i=0; i<=c1.rows-1; i++)
            {
                for(j=0; j<=c1.cols-1; j++)
                {
                    c1.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                    c2.ptr.pp_double[i][j] = c1.ptr.pp_double[i][j];
                }
            }
        }
        else
        {
            
            /*
             * Set C1/C1 to 1x1 matrices; working with 1x1 matrices allows
             * to test correctness of code which passes them to MKL.
             */
            ae_matrix_set_length(&c1, 1, 1, _state);
            ae_matrix_set_length(&c2, 1, 1, _state);
        }
        if( ncvt>0 )
        {
            ae_matrix_set_length(&vt1, n, ncvt, _state);
            ae_matrix_set_length(&vt2, n, ncvt, _state);
            for(i=0; i<=vt1.rows-1; i++)
            {
                for(j=0; j<=vt1.cols-1; j++)
                {
                    vt1.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                    vt2.ptr.pp_double[i][j] = vt1.ptr.pp_double[i][j];
                }
            }
        }
        else
        {
            
            /*
             * Set VT1/VT1 to 1x1 matrices; working with 1x1 matrices allows
             * to test correctness of code which passes them to MKL.
             */
            ae_matrix_set_length(&vt1, 1, 1, _state);
            ae_matrix_set_length(&vt2, 1, 1, _state);
        }
        
        /*
         * SVD with non-square U/C/VT
         */
        if( !rmatrixbdsvd(&wt, e, n, isupper, ae_fp_greater(hqrnduniformr(&rs, _state),(double)(0)), &u2, nru, &c2, ncc, &vt2, ncvt, _state) )
        {
            *err = 1.0;
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=nru-1; i++)
        {
            for(j=0; j<=u2.cols-1; j++)
            {
                v = ae_v_dotproduct(&u1.ptr.pp_double[i][0], 1, &u->ptr.pp_double[0][j], u->stride, ae_v_len(0,n-1));
                *err = ae_maxreal(*err, ae_fabs(v-u2.ptr.pp_double[i][j], _state), _state);
            }
        }
        for(i=0; i<=c2.rows-1; i++)
        {
            for(j=0; j<=ncc-1; j++)
            {
                v = ae_v_dotproduct(&c->ptr.pp_double[i][0], 1, &c1.ptr.pp_double[0][j], c1.stride, ae_v_len(0,n-1));
                *err = ae_maxreal(*err, ae_fabs(v-c2.ptr.pp_double[i][j], _state), _state);
            }
        }
        for(i=0; i<=vt2.rows-1; i++)
        {
            for(j=0; j<=ncvt-1; j++)
            {
                v = ae_v_dotproduct(&vt->ptr.pp_double[i][0], 1, &vt1.ptr.pp_double[0][j], vt1.stride, ae_v_len(0,n-1));
                *err = ae_maxreal(*err, ae_fabs(v-vt2.ptr.pp_double[i][j], _state), _state);
            }
        }
    }
    ae_frame_leave(_state);
}


static void testbdsvdunit_testbdsvdproblem(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* e,
     ae_int_t n,
     double* materr,
     double* orterr,
     ae_bool* wsorted,
     ae_bool* wfailed,
     ae_int_t* failcount,
     ae_int_t* succcount,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix u;
    ae_matrix vt;
    ae_matrix c;
    ae_vector w;
    ae_int_t i;
    double mx;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);

    mx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_greater(ae_fabs(d->ptr.p_double[i], _state),mx) )
        {
            mx = ae_fabs(d->ptr.p_double[i], _state);
        }
    }
    for(i=0; i<=n-2; i++)
    {
        if( ae_fp_greater(ae_fabs(e->ptr.p_double[i], _state),mx) )
        {
            mx = ae_fabs(e->ptr.p_double[i], _state);
        }
    }
    if( ae_fp_eq(mx,(double)(0)) )
    {
        mx = (double)(1);
    }
    
    /*
     * Upper BDSVD tests
     */
    ae_vector_set_length(&w, n-1+1, _state);
    testbdsvdunit_fillidentity(&u, n, _state);
    testbdsvdunit_fillidentity(&vt, n, _state);
    testbdsvdunit_fillidentity(&c, n, _state);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = d->ptr.p_double[i];
    }
    if( !rmatrixbdsvd(&w, e, n, ae_true, ae_false, &u, n, &c, n, &vt, n, _state) )
    {
        *failcount = *failcount+1;
        *wfailed = ae_true;
        ae_frame_leave(_state);
        return;
    }
    testbdsvdunit_getbdsvderror(d, e, n, ae_true, &u, &c, &w, &vt, materr, orterr, wsorted, _state);
    testbdsvdunit_checksvdmultiplication(d, e, n, ae_true, &u, &c, &w, &vt, materr, _state);
    testbdsvdunit_fillidentity(&u, n, _state);
    testbdsvdunit_fillidentity(&vt, n, _state);
    testbdsvdunit_fillidentity(&c, n, _state);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = d->ptr.p_double[i];
    }
    if( !rmatrixbdsvd(&w, e, n, ae_true, ae_true, &u, n, &c, n, &vt, n, _state) )
    {
        *failcount = *failcount+1;
        *wfailed = ae_true;
        ae_frame_leave(_state);
        return;
    }
    testbdsvdunit_getbdsvderror(d, e, n, ae_true, &u, &c, &w, &vt, materr, orterr, wsorted, _state);
    testbdsvdunit_checksvdmultiplication(d, e, n, ae_true, &u, &c, &w, &vt, materr, _state);
    
    /*
     * Lower BDSVD tests
     */
    ae_vector_set_length(&w, n-1+1, _state);
    testbdsvdunit_fillidentity(&u, n, _state);
    testbdsvdunit_fillidentity(&vt, n, _state);
    testbdsvdunit_fillidentity(&c, n, _state);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = d->ptr.p_double[i];
    }
    if( !rmatrixbdsvd(&w, e, n, ae_false, ae_false, &u, n, &c, n, &vt, n, _state) )
    {
        *failcount = *failcount+1;
        *wfailed = ae_true;
        ae_frame_leave(_state);
        return;
    }
    testbdsvdunit_getbdsvderror(d, e, n, ae_false, &u, &c, &w, &vt, materr, orterr, wsorted, _state);
    testbdsvdunit_checksvdmultiplication(d, e, n, ae_false, &u, &c, &w, &vt, materr, _state);
    testbdsvdunit_fillidentity(&u, n, _state);
    testbdsvdunit_fillidentity(&vt, n, _state);
    testbdsvdunit_fillidentity(&c, n, _state);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = d->ptr.p_double[i];
    }
    if( !rmatrixbdsvd(&w, e, n, ae_false, ae_true, &u, n, &c, n, &vt, n, _state) )
    {
        *failcount = *failcount+1;
        *wfailed = ae_true;
        ae_frame_leave(_state);
        return;
    }
    testbdsvdunit_getbdsvderror(d, e, n, ae_false, &u, &c, &w, &vt, materr, orterr, wsorted, _state);
    testbdsvdunit_checksvdmultiplication(d, e, n, ae_false, &u, &c, &w, &vt, materr, _state);
    
    /*
     * update counter
     */
    *succcount = *succcount+1;
    ae_frame_leave(_state);
}


/*$ End $*/
