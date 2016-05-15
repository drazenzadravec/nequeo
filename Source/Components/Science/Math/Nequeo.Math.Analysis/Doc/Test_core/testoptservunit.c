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
#include "testoptservunit.h"


/*$ Declarations $*/
static void testoptservunit_testprec(ae_bool* wereerrors,
     ae_state *_state);


/*$ Body $*/


ae_bool testoptserv(ae_bool silent, ae_state *_state)
{
    ae_bool precerrors;
    ae_bool wereerrors;
    ae_bool result;


    precerrors = ae_false;
    testoptservunit_testprec(&precerrors, _state);
    
    /*
     * report
     */
    wereerrors = precerrors;
    if( !silent )
    {
        printf("TESTING OPTSERV\n");
        printf("TESTS:                                    \n");
        printf("* PRECONDITIONERS                         ");
        if( precerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        if( wereerrors )
        {
            printf("TEST FAILED\n");
        }
        else
        {
            printf("TEST PASSED\n");
        }
        printf("\n\n");
    }
    result = !wereerrors;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testoptserv(ae_bool silent, ae_state *_state)
{
    return testoptserv(silent, _state);
}


/*************************************************************************
This function checks preconditioning functions

On failure sets error flag.
*************************************************************************/
static void testoptservunit_testprec(ae_bool* wereerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t k;
    ae_int_t i;
    ae_int_t j;
    ae_int_t i0;
    ae_int_t j0;
    ae_int_t j1;
    double v;
    double rho;
    double theta;
    double tolg;
    ae_matrix va;
    ae_vector vc;
    ae_vector vd;
    ae_vector vb;
    ae_vector s0;
    ae_vector s1;
    ae_vector s2;
    ae_vector g;
    precbuflbfgs buf;
    precbuflowrank lowrankbuf;
    ae_vector norms;
    ae_matrix sk;
    ae_matrix yk;
    ae_matrix bk;
    ae_vector bksk;
    ae_vector tmp;
    matinvreport rep;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&va, 0, 0, DT_REAL, _state);
    ae_vector_init(&vc, 0, DT_REAL, _state);
    ae_vector_init(&vd, 0, DT_REAL, _state);
    ae_vector_init(&vb, 0, DT_REAL, _state);
    ae_vector_init(&s0, 0, DT_REAL, _state);
    ae_vector_init(&s1, 0, DT_REAL, _state);
    ae_vector_init(&s2, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    _precbuflbfgs_init(&buf, _state);
    _precbuflowrank_init(&lowrankbuf, _state);
    ae_vector_init(&norms, 0, DT_REAL, _state);
    ae_matrix_init(&sk, 0, 0, DT_REAL, _state);
    ae_matrix_init(&yk, 0, 0, DT_REAL, _state);
    ae_matrix_init(&bk, 0, 0, DT_REAL, _state);
    ae_vector_init(&bksk, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    _matinvreport_init(&rep, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Test for inexact L-BFGS preconditioner.
     *
     * We generate QP problem 0.5*x'*H*x, with random H=D+V'*C*V.
     * Different K's, from 0 to N, are tried. We test preconditioner
     * code which uses compact L-BFGS update against reference implementation
     * which uses non-compact BFGS scheme.
     *
     * For each K we perform two tests: first for KxN non-zero matrix V,
     * second one for NxN matrix V with last N-K rows set to zero. Last test
     * checks algorithm's ability to handle zero updates.
     */
    tolg = 1.0E-9;
    for(n=1; n<=10; n++)
    {
        for(k=0; k<=n; k++)
        {
            
            /*
             * Prepare problem:
             * * VD, VC, VA, with VC/VA reordered by ascending of VC[i]*norm(VA[i,...])^2
             * * trial vector S (copies are stored to S0,S1,S2)
             */
            ae_vector_set_length(&vd, n, _state);
            ae_vector_set_length(&s0, n, _state);
            ae_vector_set_length(&s1, n, _state);
            ae_vector_set_length(&s2, n, _state);
            for(i=0; i<=n-1; i++)
            {
                vd.ptr.p_double[i] = ae_exp(hqrndnormal(&rs, _state), _state);
                s0.ptr.p_double[i] = hqrndnormal(&rs, _state);
                s1.ptr.p_double[i] = s0.ptr.p_double[i];
                s2.ptr.p_double[i] = s0.ptr.p_double[i];
            }
            rmatrixrndcond(n, 1.0E2, &va, _state);
            rvectorsetlengthatleast(&vc, n, _state);
            for(i=0; i<=k-1; i++)
            {
                vc.ptr.p_double[i] = ae_exp(hqrndnormal(&rs, _state), _state);
            }
            for(i=k; i<=n-1; i++)
            {
                vc.ptr.p_double[i] = (double)(0);
                for(j=0; j<=n-1; j++)
                {
                    va.ptr.pp_double[i][j] = 0.0;
                }
            }
            ae_vector_set_length(&norms, k, _state);
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&va.ptr.pp_double[i][0], 1, &va.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                norms.ptr.p_double[i] = v*vc.ptr.p_double[i];
            }
            for(i=0; i<=k-1; i++)
            {
                for(j=0; j<=k-2; j++)
                {
                    if( ae_fp_greater(norms.ptr.p_double[j],norms.ptr.p_double[j+1]) )
                    {
                        
                        /*
                         * Swap elements J and J+1
                         */
                        v = norms.ptr.p_double[j];
                        norms.ptr.p_double[j] = norms.ptr.p_double[j+1];
                        norms.ptr.p_double[j+1] = v;
                        v = vc.ptr.p_double[j];
                        vc.ptr.p_double[j] = vc.ptr.p_double[j+1];
                        vc.ptr.p_double[j+1] = v;
                        for(j0=0; j0<=n-1; j0++)
                        {
                            v = va.ptr.pp_double[j][j0];
                            va.ptr.pp_double[j][j0] = va.ptr.pp_double[j+1][j0];
                            va.ptr.pp_double[j+1][j0] = v;
                        }
                    }
                }
            }
            
            /*
             * Generate reference model and apply it to S2:
             * * generate approximate Hessian Bk
             * * calculate inv(Bk)
             * * calculate inv(Bk)*S2, store to S2
             */
            rmatrixsetlengthatleast(&sk, k, n, _state);
            rmatrixsetlengthatleast(&yk, k, n, _state);
            ae_matrix_set_length(&bk, n, n, _state);
            ae_vector_set_length(&bksk, n, _state);
            ae_vector_set_length(&tmp, n, _state);
            for(i=0; i<=k-1; i++)
            {
                ae_v_move(&sk.ptr.pp_double[i][0], 1, &va.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                v = ae_v_dotproduct(&va.ptr.pp_double[i][0], 1, &sk.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                v = v*vc.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    yk.ptr.pp_double[i][j] = vd.ptr.p_double[j]*sk.ptr.pp_double[i][j]+va.ptr.pp_double[i][j]*v;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( i==j )
                    {
                        bk.ptr.pp_double[i][i] = vd.ptr.p_double[i];
                    }
                    else
                    {
                        bk.ptr.pp_double[i][j] = 0.0;
                    }
                }
            }
            for(i=0; i<=k-1; i++)
            {
                theta = 0.0;
                for(j0=0; j0<=n-1; j0++)
                {
                    bksk.ptr.p_double[j0] = (double)(0);
                    for(j1=0; j1<=n-1; j1++)
                    {
                        theta = theta+sk.ptr.pp_double[i][j0]*bk.ptr.pp_double[j0][j1]*sk.ptr.pp_double[i][j1];
                        bksk.ptr.p_double[j0] = bksk.ptr.p_double[j0]+bk.ptr.pp_double[j0][j1]*sk.ptr.pp_double[i][j1];
                    }
                }
                theta = 1/theta;
                rho = ae_v_dotproduct(&sk.ptr.pp_double[i][0], 1, &yk.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                rho = 1/rho;
                for(j0=0; j0<=n-1; j0++)
                {
                    for(j1=0; j1<=n-1; j1++)
                    {
                        bk.ptr.pp_double[j0][j1] = bk.ptr.pp_double[j0][j1]+rho*yk.ptr.pp_double[i][j0]*yk.ptr.pp_double[i][j1];
                    }
                }
                for(j0=0; j0<=n-1; j0++)
                {
                    for(j1=0; j1<=n-1; j1++)
                    {
                        bk.ptr.pp_double[j0][j1] = bk.ptr.pp_double[j0][j1]-theta*bksk.ptr.p_double[j0]*bksk.ptr.p_double[j1];
                    }
                }
            }
            rmatrixinverse(&bk, n, &j0, &rep, _state);
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&bk.ptr.pp_double[i][0], 1, &s2.ptr.p_double[0], 1, ae_v_len(0,n-1));
                tmp.ptr.p_double[i] = v;
            }
            for(i=0; i<=n-1; i++)
            {
                s2.ptr.p_double[i] = tmp.ptr.p_double[i];
            }
            
            /*
             * First test for non-zero V:
             * * apply preconditioner to X0
             * * compare reference model against implementation being tested
             */
            inexactlbfgspreconditioner(&s0, n, &vd, &vc, &va, k, &buf, _state);
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(wereerrors, ae_fp_greater(ae_fabs(s2.ptr.p_double[i]-s0.ptr.p_double[i], _state),tolg), _state);
            }
            
            /*
             * Second test - N-K zero rows appended to V and rows are
             * randomly reordered. Doing so should not change result,
             * algorithm must be able to order rows according to second derivative
             * and skip zero updates.
             */
            for(i=0; i<=n-1; i++)
            {
                i0 = i+hqrnduniformi(&rs, n-i, _state);
                v = vc.ptr.p_double[i];
                vc.ptr.p_double[i] = vc.ptr.p_double[i0];
                vc.ptr.p_double[i0] = v;
                for(j=0; j<=n-1; j++)
                {
                    v = va.ptr.pp_double[i][j];
                    va.ptr.pp_double[i][j] = va.ptr.pp_double[i0][j];
                    va.ptr.pp_double[i0][j] = v;
                }
            }
            inexactlbfgspreconditioner(&s1, n, &vd, &vc, &va, n, &buf, _state);
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(wereerrors, ae_fp_greater(ae_fabs(s2.ptr.p_double[i]-s1.ptr.p_double[i], _state),tolg), _state);
            }
        }
    }
    
    /*
     * Test for exact low-rank preconditioner.
     *
     * We generate QP problem 0.5*x'*H*x, with random H=D+V'*C*V.
     * Different K's, from 0 to N, are tried. We test preconditioner
     * code which uses Woodbury update against reference implementation
     * which performs straightforward matrix inversion.
     *
     * For each K we perform two tests: first for KxN non-zero matrix V,
     * second one for NxN matrix V with randomly appended N-K zero rows.
     * Last test checks algorithm's ability to handle zero updates.
     */
    tolg = 1.0E-9;
    for(n=1; n<=10; n++)
    {
        for(k=0; k<=n; k++)
        {
            
            /*
             * Prepare problem:
             * * VD, VC, VA
             * * trial vector S (copies are stored to S0,S1,S2)
             */
            ae_vector_set_length(&vd, n, _state);
            ae_vector_set_length(&s0, n, _state);
            ae_vector_set_length(&s1, n, _state);
            ae_vector_set_length(&s2, n, _state);
            for(i=0; i<=n-1; i++)
            {
                vd.ptr.p_double[i] = ae_exp(hqrndnormal(&rs, _state), _state);
                s0.ptr.p_double[i] = hqrndnormal(&rs, _state);
                s1.ptr.p_double[i] = s0.ptr.p_double[i];
                s2.ptr.p_double[i] = s0.ptr.p_double[i];
            }
            rmatrixrndcond(n, 1.0E2, &va, _state);
            rvectorsetlengthatleast(&vc, n, _state);
            for(i=0; i<=k-1; i++)
            {
                vc.ptr.p_double[i] = ae_exp(hqrndnormal(&rs, _state), _state);
            }
            for(i=k; i<=n-1; i++)
            {
                vc.ptr.p_double[i] = (double)(0);
                for(j=0; j<=n-1; j++)
                {
                    va.ptr.pp_double[i][j] = 0.0;
                }
            }
            
            /*
             * Generate reference model and apply it to S2
             */
            ae_matrix_set_length(&bk, n, n, _state);
            ae_vector_set_length(&tmp, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( i==j )
                    {
                        v = vd.ptr.p_double[i];
                    }
                    else
                    {
                        v = 0.0;
                    }
                    for(j1=0; j1<=k-1; j1++)
                    {
                        v = v+va.ptr.pp_double[j1][i]*vc.ptr.p_double[j1]*va.ptr.pp_double[j1][j];
                    }
                    bk.ptr.pp_double[i][j] = v;
                }
            }
            rmatrixinverse(&bk, n, &j, &rep, _state);
            ae_assert(j>0, "Assertion failed", _state);
            for(i=0; i<=n-1; i++)
            {
                v = 0.0;
                for(j=0; j<=n-1; j++)
                {
                    v = v+bk.ptr.pp_double[i][j]*s2.ptr.p_double[j];
                }
                tmp.ptr.p_double[i] = v;
            }
            for(i=0; i<=n-1; i++)
            {
                s2.ptr.p_double[i] = tmp.ptr.p_double[i];
            }
            
            /*
             * First test for non-zero V:
             * * apply preconditioner to X0
             * * compare reference model against implementation being tested
             */
            preparelowrankpreconditioner(&vd, &vc, &va, n, k, &lowrankbuf, _state);
            applylowrankpreconditioner(&s0, &lowrankbuf, _state);
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(wereerrors, ae_fp_greater(ae_fabs(s2.ptr.p_double[i]-s0.ptr.p_double[i], _state),tolg), _state);
            }
            
            /*
             * Second test - N-K zero rows appended to V and rows are
             * randomly reordered. Doing so should not change result,
             * algorithm must be able to order rows according to second derivative
             * and skip zero updates.
             */
            for(i=0; i<=n-1; i++)
            {
                i0 = i+hqrnduniformi(&rs, n-i, _state);
                v = vc.ptr.p_double[i];
                vc.ptr.p_double[i] = vc.ptr.p_double[i0];
                vc.ptr.p_double[i0] = v;
                for(j=0; j<=n-1; j++)
                {
                    v = va.ptr.pp_double[i][j];
                    va.ptr.pp_double[i][j] = va.ptr.pp_double[i0][j];
                    va.ptr.pp_double[i0][j] = v;
                }
            }
            preparelowrankpreconditioner(&vd, &vc, &va, n, n, &lowrankbuf, _state);
            applylowrankpreconditioner(&s1, &lowrankbuf, _state);
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(wereerrors, ae_fp_greater(ae_fabs(s2.ptr.p_double[i]-s1.ptr.p_double[i], _state),tolg), _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*$ End $*/
