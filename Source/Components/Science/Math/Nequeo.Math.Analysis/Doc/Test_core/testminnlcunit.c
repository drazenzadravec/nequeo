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
#include "testminnlcunit.h"


/*$ Declarations $*/
static double testminnlcunit_barriertolerance = 0.05;
static void testminnlcunit_testbc(ae_bool* wereerrors, ae_state *_state);
static void testminnlcunit_testlc(ae_bool* wereerrors, ae_state *_state);
static void testminnlcunit_testnlc(ae_bool* wereerrors, ae_state *_state);
static void testminnlcunit_testother(ae_bool* wereerrors,
     ae_state *_state);
static void testminnlcunit_testbugs(ae_bool* wereerrors, ae_state *_state);


/*$ Body $*/


ae_bool testminnlc(ae_bool silent, ae_state *_state)
{
    ae_bool waserrors;
    ae_bool bcerr;
    ae_bool lcerr;
    ae_bool nlcerr;
    ae_bool othererr;
    ae_bool bugs;
    ae_bool result;


    waserrors = ae_false;
    bcerr = ae_false;
    lcerr = ae_false;
    nlcerr = ae_false;
    othererr = ae_false;
    bugs = ae_false;
    testminnlcunit_testbugs(&bugs, _state);
    testminnlcunit_testbc(&bcerr, _state);
    testminnlcunit_testlc(&lcerr, _state);
    testminnlcunit_testnlc(&nlcerr, _state);
    testminnlcunit_testother(&othererr, _state);
    
    /*
     * end
     */
    waserrors = (((bcerr||lcerr)||nlcerr)||othererr)||bugs;
    if( !silent )
    {
        printf("TESTING MINNLC OPTIMIZATION\n");
        printf("TESTS:\n");
        printf("* BOUND CONSTRAINED                       ");
        if( bcerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* LINEARLY CONSTRAINED                    ");
        if( lcerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* NONLINEARLY CONSTRAINED                 ");
        if( nlcerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* OTHER PROPERTIES                        ");
        if( othererr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* FIXED BUGS:                             ");
        if( bugs )
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
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testminnlc(ae_bool silent, ae_state *_state)
{
    return testminnlc(silent, _state);
}


/*************************************************************************
This function tests bound constrained quadratic programming algorithm.

On failure sets error flag.
*************************************************************************/
static void testminnlcunit_testbc(ae_bool* wereerrors, ae_state *_state)
{
    ae_frame _frame_block;
    minnlcstate state;
    minnlcreport rep;
    ae_int_t n;
    ae_int_t pass;
    ae_int_t i;
    ae_int_t j;
    ae_int_t aulits;
    double tolx;
    double tolg;
    ae_int_t scaletype;
    double rho;
    ae_matrix fulla;
    ae_vector b;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector s;
    ae_vector x0;
    ae_vector x1;
    double gnorm;
    double g;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    _minnlcstate_init(&state, _state);
    _minnlcreport_init(&rep, _state);
    ae_matrix_init(&fulla, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Convex test:
     * * N dimensions
     * * random number (0..N) of random boundary constraints
     * * positive-definite quadratic programming problem
     * * initial point is random (maybe infeasible!)
     * * random scale (unit or non-unit)
     */
    aulits = 50;
    rho = 200.0;
    tolx = 0.0005;
    tolg = 0.01;
    for(n=1; n<=10; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate well-conditioned problem with unit scale
             */
            spdmatrixrndcond(n, 1.0E2, &fulla, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                bndl.ptr.p_double[i] = _state->v_neginf;
                bndu.ptr.p_double[i] = _state->v_posinf;
                x0.ptr.p_double[i] = hqrndnormal(&rs, _state);
                j = hqrnduniformi(&rs, 5, _state);
                if( j==0 )
                {
                    bndl.ptr.p_double[i] = (double)(0);
                }
                if( j==1 )
                {
                    bndu.ptr.p_double[i] = (double)(0);
                }
                if( j==2 )
                {
                    bndl.ptr.p_double[i] = hqrndnormal(&rs, _state);
                    bndu.ptr.p_double[i] = bndl.ptr.p_double[i];
                }
                if( j==3 )
                {
                    bndl.ptr.p_double[i] = -0.1;
                    bndu.ptr.p_double[i] = 0.1;
                }
            }
            
            /*
             * Apply scaling to quadratic/linear term, so problem becomes
             * well-conditioned in the scaled coordinates.
             */
            scaletype = hqrnduniformi(&rs, 2, _state);
            ae_vector_set_length(&s, n, _state);
            for(i=0; i<=n-1; i++)
            {
                if( scaletype==0 )
                {
                    s.ptr.p_double[i] = (double)(1);
                }
                else
                {
                    s.ptr.p_double[i] = ae_exp(20*hqrndnormal(&rs, _state), _state);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = x0.ptr.p_double[i]*s.ptr.p_double[i];
                bndl.ptr.p_double[i] = bndl.ptr.p_double[i]*s.ptr.p_double[i];
                bndu.ptr.p_double[i] = bndu.ptr.p_double[i]*s.ptr.p_double[i];
                b.ptr.p_double[i] = b.ptr.p_double[i]/s.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    fulla.ptr.pp_double[i][j] = fulla.ptr.pp_double[i][j]/(s.ptr.p_double[i]*s.ptr.p_double[j]);
                }
            }
            
            /*
             * Solve problem
             */
            minnlccreate(n, &x0, &state, _state);
            minnlcsetalgoaul(&state, rho, aulits, _state);
            if( scaletype!=0 )
            {
                minnlcsetscale(&state, &s, _state);
            }
            minnlcsetbc(&state, &bndl, &bndu, _state);
            minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
            while(minnlciteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                        state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                        for(j=0; j<=n-1; j++)
                        {
                            state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                            state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        }
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnlcresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( *wereerrors )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Check feasibility properties
             */
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(wereerrors, ae_isfinite(bndl.ptr.p_double[i], _state)&&ae_fp_less_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i]-tolx*s.ptr.p_double[i]), _state);
                seterrorflag(wereerrors, ae_isfinite(bndu.ptr.p_double[i], _state)&&ae_fp_greater_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i]+tolx*s.ptr.p_double[i]), _state);
            }
            
            /*
             * Test - calculate scaled constrained gradient at solution,
             * check its norm.
             */
            gnorm = 0.0;
            for(i=0; i<=n-1; i++)
            {
                g = b.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    g = g+fulla.ptr.pp_double[i][j]*x1.ptr.p_double[j];
                }
                g = s.ptr.p_double[i]*g;
                if( (ae_isfinite(bndl.ptr.p_double[i], _state)&&ae_fp_less(ae_fabs(x1.ptr.p_double[i]-bndl.ptr.p_double[i], _state),tolx*s.ptr.p_double[i]))&&ae_fp_greater(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                if( (ae_isfinite(bndu.ptr.p_double[i], _state)&&ae_fp_less(ae_fabs(x1.ptr.p_double[i]-bndu.ptr.p_double[i], _state),tolx*s.ptr.p_double[i]))&&ae_fp_less(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                gnorm = gnorm+ae_sqr(g, _state);
            }
            gnorm = ae_sqrt(gnorm, _state);
            seterrorflag(wereerrors, ae_fp_greater(gnorm,tolg), _state);
        }
    }
    
    /*
     * Non-convex test:
     * * N dimensions, N>=2
     * * box constraints, x[i] in [-1,+1]
     * * A is symmetric indefinite with condition number 50.0
     * * random B with normal entries
     * * initial point is random, feasible
     * * scale is always unit
     *
     * We check that constrained problem can be successfully solved.
     * We do not check ability to detect unboundedness of unconstrained
     * problem because there is such functionality in MinNLC.
     */
    aulits = 50;
    rho = 200.0;
    tolx = 0.0005;
    tolg = 0.01;
    for(n=2; n<=10; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate problem
             */
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    fulla.ptr.pp_double[i][j] = 0.0;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                fulla.ptr.pp_double[i][i] = -1-hqrnduniformr(&rs, _state);
            }
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                b.ptr.p_double[i] = 0.05*hqrndnormal(&rs, _state);
                bndl.ptr.p_double[i] = (double)(-1);
                bndu.ptr.p_double[i] = (double)(1);
                x0.ptr.p_double[i] = 2*hqrnduniformr(&rs, _state)-1;
            }
            
            /*
             * Solve problem:
             * * without constraints we expect failure
             * * with constraints algorithm must succeed
             */
            minnlccreate(n, &x0, &state, _state);
            minnlcsetalgoaul(&state, rho, aulits, _state);
            minnlcsetbc(&state, &bndl, &bndu, _state);
            minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
            while(minnlciteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                        state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                        for(j=0; j<=n-1; j++)
                        {
                            state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                            state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        }
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnlcresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( *wereerrors )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Check feasibility properties
             */
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(wereerrors, ae_isfinite(bndl.ptr.p_double[i], _state)&&ae_fp_less_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i]-tolx), _state);
                seterrorflag(wereerrors, ae_isfinite(bndu.ptr.p_double[i], _state)&&ae_fp_greater_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i]+tolx), _state);
            }
            
            /*
             * Test - calculate scaled constrained gradient at solution,
             * check its norm.
             */
            gnorm = 0.0;
            for(i=0; i<=n-1; i++)
            {
                g = b.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    g = g+fulla.ptr.pp_double[i][j]*x1.ptr.p_double[j];
                }
                g = g;
                if( (ae_isfinite(bndl.ptr.p_double[i], _state)&&ae_fp_less(ae_fabs(x1.ptr.p_double[i]-bndl.ptr.p_double[i], _state),tolx))&&ae_fp_greater(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                if( (ae_isfinite(bndu.ptr.p_double[i], _state)&&ae_fp_less(ae_fabs(x1.ptr.p_double[i]-bndu.ptr.p_double[i], _state),tolx))&&ae_fp_less(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                gnorm = gnorm+ae_sqr(g, _state);
            }
            gnorm = ae_sqrt(gnorm, _state);
            seterrorflag(wereerrors, ae_fp_greater(gnorm,tolg), _state);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests linearly constrained quadratic programming algorithm.

Sets error flag on failure.
*************************************************************************/
static void testminnlcunit_testlc(ae_bool* wereerrors, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t k;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_matrix q;
    ae_matrix fulla;
    double v;
    double vv;
    ae_vector tmp;
    ae_vector bl;
    ae_vector bu;
    ae_vector b;
    ae_vector xs0;
    ae_vector xstart;
    ae_vector x;
    ae_vector x0;
    ae_vector x1;
    ae_vector x2;
    ae_vector xm;
    ae_vector s;
    ae_vector g;
    ae_vector bndl;
    ae_vector bndu;
    ae_matrix a;
    ae_matrix c;
    ae_matrix ce;
    ae_vector ct;
    ae_vector nonnegative;
    double tolx;
    double tolg;
    double tolf;
    ae_int_t aulits;
    double rho;
    minnlcstate state;
    minnlcreport rep;
    ae_int_t scaletype;
    double f0;
    double f1;
    double tolconstr;
    ae_int_t bscale;
    ae_int_t akind;
    ae_int_t ccnt;
    ae_int_t shiftkind;
    hqrndstate rs;
    snnlssolver nnls;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&q, 0, 0, DT_REAL, _state);
    ae_matrix_init(&fulla, 0, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&xs0, 0, DT_REAL, _state);
    ae_vector_init(&xstart, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&xm, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ce, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    ae_vector_init(&nonnegative, 0, DT_BOOL, _state);
    _minnlcstate_init(&state, _state);
    _minnlcreport_init(&rep, _state);
    _hqrndstate_init(&rs, _state);
    _snnlssolver_init(&nnls, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * First test:
     * * K<N equality constraints Q*x = Q*x0, where Q is random
     *   orthogonal K*N matrix, x0 is some random vector
     * * quadratic programming problem with identity quadratic term A and
     *   linear term equal to xm*A, where xm is some random vector such
     *   that Q*xm=0. It is always possible to find such xm, because K<N
     *   Thus, optimization problem has form 0.5*x'*I*x-xm'*x.
     * * exact solution must be equal to x0
     *
     * NOTE: this test is important because it is the only linearly constrained one
     *       which uses non-unit scaling!
     */
    rho = 200.0;
    tolx = 0.0005;
    aulits = 50;
    for(n=2; n<=6; n++)
    {
        for(k=1; k<=n-1; k++)
        {
            
            /*
             * Generate problem: A, b, CMatrix, x0, XStart
             */
            rmatrixrndorthogonal(n, &q, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&xm, n, _state);
            ae_vector_set_length(&xstart, n, _state);
            ae_matrix_set_length(&c, k, n+1, _state);
            ae_vector_set_length(&ct, k, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xm.ptr.p_double[i] = x0.ptr.p_double[i];
                xstart.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=k-1; i++)
            {
                ae_v_move(&c.ptr.pp_double[i][0], 1, &q.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                c.ptr.pp_double[i][n] = v;
                ct.ptr.p_int[i] = 0;
                v = 2*ae_randomreal(_state)-1;
                ae_v_addd(&xm.ptr.p_double[0], 1, &q.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
            }
            for(i=0; i<=n-1; i++)
            {
                b.ptr.p_double[i] = -xm.ptr.p_double[i];
            }
            
            /*
             * Apply scaling to linear term and known solution,
             * so problem becomes well-conditioned in the scaled coordinates.
             */
            scaletype = hqrnduniformi(&rs, 2, _state);
            ae_vector_set_length(&s, n, _state);
            for(i=0; i<=n-1; i++)
            {
                if( scaletype==0 )
                {
                    s.ptr.p_double[i] = (double)(1);
                }
                else
                {
                    s.ptr.p_double[i] = ae_exp(20*hqrndnormal(&rs, _state), _state);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = x0.ptr.p_double[i]*s.ptr.p_double[i];
                xstart.ptr.p_double[i] = xstart.ptr.p_double[i]*s.ptr.p_double[i];
                b.ptr.p_double[i] = b.ptr.p_double[i]/s.ptr.p_double[i];
            }
            for(i=0; i<=k-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    c.ptr.pp_double[i][j] = c.ptr.pp_double[i][j]/s.ptr.p_double[j];
                }
            }
            
            /*
             * Create optimizer, solve
             */
            minnlccreate(n, &xstart, &state, _state);
            minnlcsetalgoaul(&state, rho, aulits, _state);
            minnlcsetscale(&state, &s, _state);
            minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
            minnlcsetlc(&state, &c, &ct, k, _state);
            while(minnlciteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i]+0.5*ae_sqr(state.x.ptr.p_double[i], _state)/ae_sqr(s.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = b.ptr.p_double[i]+state.x.ptr.p_double[i]/ae_sqr(s.ptr.p_double[i], _state);
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnlcresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( *wereerrors )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Compare with analytic solution
             */
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-x0.ptr.p_double[i], _state),tolx*s.ptr.p_double[i]), _state);
            }
        }
    }
    
    /*
     * Inequality constrained problem:
     * * N*N diagonal A
     * * one inequality constraint q'*x>=0, where q is random unit vector
     * * optimization problem has form 0.5*x'*A*x-(x1*A)*x,
     *   where x1 is some random vector
     * * either:
     *   a) x1 is feasible => we must stop at x1
     *   b) x1 is infeasible => we must stop at the boundary q'*x=0 and
     *      projection of gradient onto q*x=0 must be zero
     *
     * NOTE: we make several passes because some specific kind of errors is rarely
     *       caught by this test, so we need several repetitions.
     */
    rho = 200.0;
    tolx = 0.0005;
    tolg = 0.01;
    aulits = 50;
    for(n=2; n<=6; n++)
    {
        for(pass=0; pass<=4; pass++)
        {
            
            /*
             * Generate problem: A, b, CMatrix, x0, XStart
             */
            spdmatrixrndcond(n, 1.0E2, &fulla, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&xm, n, _state);
            ae_vector_set_length(&xstart, n, _state);
            ae_matrix_set_length(&c, 1, n+1, _state);
            ae_vector_set_length(&ct, 1, _state);
            for(i=0; i<=n-1; i++)
            {
                xm.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xstart.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            do
            {
                v = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    c.ptr.pp_double[0][i] = 2*ae_randomreal(_state)-1;
                    v = v+ae_sqr(c.ptr.pp_double[0][i], _state);
                }
                v = ae_sqrt(v, _state);
            }
            while(ae_fp_eq(v,(double)(0)));
            for(i=0; i<=n-1; i++)
            {
                c.ptr.pp_double[0][i] = c.ptr.pp_double[0][i]/v;
            }
            c.ptr.pp_double[0][n] = (double)(0);
            ct.ptr.p_int[0] = 1;
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&fulla.ptr.pp_double[i][0], 1, &xm.ptr.p_double[0], 1, ae_v_len(0,n-1));
                b.ptr.p_double[i] = -v;
            }
            
            /*
             * Apply scaling to linear term and known solution,
             * so problem becomes well-conditioned in the scaled coordinates.
             */
            scaletype = hqrnduniformi(&rs, 2, _state);
            ae_vector_set_length(&s, n, _state);
            for(i=0; i<=n-1; i++)
            {
                if( scaletype==0 )
                {
                    s.ptr.p_double[i] = (double)(1);
                }
                else
                {
                    s.ptr.p_double[i] = ae_exp(20*hqrndnormal(&rs, _state), _state);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                xm.ptr.p_double[i] = xm.ptr.p_double[i]*s.ptr.p_double[i];
                xstart.ptr.p_double[i] = xstart.ptr.p_double[i]*s.ptr.p_double[i];
                b.ptr.p_double[i] = b.ptr.p_double[i]/s.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    fulla.ptr.pp_double[i][j] = fulla.ptr.pp_double[i][j]/(s.ptr.p_double[i]*s.ptr.p_double[j]);
                }
            }
            for(j=0; j<=n-1; j++)
            {
                c.ptr.pp_double[0][j] = c.ptr.pp_double[0][j]/s.ptr.p_double[j];
            }
            
            /*
             * Create optimizer, solve
             */
            minnlccreate(n, &xstart, &state, _state);
            minnlcsetalgoaul(&state, rho, aulits, _state);
            minnlcsetlc(&state, &c, &ct, 1, _state);
            minnlcsetscale(&state, &s, _state);
            while(minnlciteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                        state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                        for(j=0; j<=n-1; j++)
                        {
                            state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                            state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        }
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnlcresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( *wereerrors )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Test solution
             */
            ae_vector_set_length(&g, n, _state);
            ae_v_move(&g.ptr.p_double[0], 1, &b.ptr.p_double[0], 1, ae_v_len(0,n-1));
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&fulla.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                g.ptr.p_double[i] = g.ptr.p_double[i]+v;
            }
            v = ae_v_dotproduct(&x1.ptr.p_double[0], 1, &c.ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
            seterrorflag(wereerrors, ae_fp_less(v,-tolx), _state);
            if( ae_fp_less(v,tolx) )
            {
                
                /*
                 * Point at the boundary, project gradient into
                 * equality-constrained subspace.
                 */
                v = 0.0;
                vv = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    v = v+g.ptr.p_double[i]*c.ptr.pp_double[0][i];
                    vv = vv+c.ptr.pp_double[0][i]*c.ptr.pp_double[0][i];
                }
                v = v/vv;
                ae_v_subd(&g.ptr.p_double[0], 1, &c.ptr.pp_double[0][0], 1, ae_v_len(0,n-1), v);
            }
            v = 0.0;
            for(i=0; i<=n-1; i++)
            {
                v = v+ae_sqr(g.ptr.p_double[i]*s.ptr.p_double[i], _state);
            }
            seterrorflag(wereerrors, ae_fp_greater(ae_sqrt(v, _state),tolg), _state);
        }
    }
    
    /*
     * Equality-constrained test:
     * * N*N SPD A
     * * K<N equality constraints Q*x = Q*x0, where Q is random
     *   orthogonal K*N matrix, x0 is some random vector
     * * optimization problem has form 0.5*x'*A*x-(xm*A)*x,
     *   where xm is some random vector
     * * we check feasibility properties of the solution
     * * we do not know analytic form of the exact solution,
     *   but we know that projection of gradient into equality constrained
     *   subspace must be zero at the solution
     */
    rho = 200.0;
    tolx = 0.0005;
    tolg = 0.01;
    aulits = 50;
    for(n=2; n<=6; n++)
    {
        for(k=1; k<=n-1; k++)
        {
            
            /*
             * Generate problem: A, b, CMatrix, x0, XStart
             */
            rmatrixrndorthogonal(n, &q, _state);
            spdmatrixrndcond(n, 1.0E2, &fulla, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&xm, n, _state);
            ae_vector_set_length(&xstart, n, _state);
            ae_matrix_set_length(&c, k, n+1, _state);
            ae_vector_set_length(&ct, k, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xm.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xstart.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=k-1; i++)
            {
                ae_v_move(&c.ptr.pp_double[i][0], 1, &q.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                c.ptr.pp_double[i][n] = v;
                ct.ptr.p_int[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&fulla.ptr.pp_double[i][0], 1, &xm.ptr.p_double[0], 1, ae_v_len(0,n-1));
                b.ptr.p_double[i] = -v;
            }
            
            /*
             * Create optimizer, solve
             */
            minnlccreate(n, &xstart, &state, _state);
            minnlcsetalgoaul(&state, rho, aulits, _state);
            minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
            minnlcsetlc(&state, &c, &ct, k, _state);
            while(minnlciteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                        state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                        for(j=0; j<=n-1; j++)
                        {
                            state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                            state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        }
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnlcresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( *wereerrors )
            {
                ae_frame_leave(_state);
                return;
            }
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&x1.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                seterrorflag(wereerrors, ae_fp_greater(ae_fabs(v-c.ptr.pp_double[i][n], _state),tolx), _state);
            }
            ae_vector_set_length(&g, n, _state);
            ae_v_move(&g.ptr.p_double[0], 1, &b.ptr.p_double[0], 1, ae_v_len(0,n-1));
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&fulla.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                g.ptr.p_double[i] = g.ptr.p_double[i]+v;
            }
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&g.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                ae_v_subd(&g.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
            }
            v = ae_v_dotproduct(&g.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,n-1));
            seterrorflag(wereerrors, ae_fp_greater(ae_sqrt(v, _state),tolg), _state);
        }
    }
    
    /*
     * Boundary constraints vs linear ones:
     * * N*N SPD A
     * * optimization problem has form 0.5*x'*A*x-(xm*A)*x,
     *   where xm is some random vector from [-1,+1]
     * * K=2*N constraints of the form ai<=x[i] or x[i]<=b[i],
     *   with ai in [-1.0,-0.1], bi in [+0.1,+1.0]
     * * initial point xstart is from [-1,+2]
     * * we solve two related QP problems:
     *   a) one with constraints posed as boundary ones
     *   b) another one with same constraints posed as general linear ones
     * both problems must have same solution.
     * Here we test that boundary constrained and linear inequality constrained
     * solvers give same results.
     */
    rho = 200.0;
    tolx = 0.0005;
    aulits = 50;
    for(n=1; n<=6; n++)
    {
        
        /*
         * Generate problem: A, b, x0, XStart, C, CT
         */
        spdmatrixrndcond(n, 1.0E2, &fulla, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&xm, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_matrix_set_length(&c, 2*n, n+1, _state);
        ae_vector_set_length(&ct, 2*n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        for(i=0; i<=n-1; i++)
        {
            xm.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            x0.ptr.p_double[i] = 3*ae_randomreal(_state)-1;
            bndl.ptr.p_double[i] = -(0.1+0.9*ae_randomreal(_state));
            bndu.ptr.p_double[i] = 0.1+0.9*ae_randomreal(_state);
            for(j=0; j<=n-1; j++)
            {
                c.ptr.pp_double[2*i+0][j] = (double)(0);
                c.ptr.pp_double[2*i+1][j] = (double)(0);
            }
            c.ptr.pp_double[2*i+0][i] = (double)(1);
            c.ptr.pp_double[2*i+0][n] = bndl.ptr.p_double[i];
            ct.ptr.p_int[2*i+0] = 1;
            c.ptr.pp_double[2*i+1][i] = (double)(1);
            c.ptr.pp_double[2*i+1][n] = bndu.ptr.p_double[i];
            ct.ptr.p_int[2*i+1] = -1;
        }
        for(i=0; i<=n-1; i++)
        {
            v = ae_v_dotproduct(&fulla.ptr.pp_double[i][0], 1, &xm.ptr.p_double[0], 1, ae_v_len(0,n-1));
            b.ptr.p_double[i] = -v;
        }
        
        /*
         * Solve linear inequality constrained problem
         */
        minnlccreate(n, &x0, &state, _state);
        minnlcsetalgoaul(&state, rho, aulits, _state);
        minnlcsetlc(&state, &c, &ct, 2*n, _state);
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                    state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                    for(j=0; j<=n-1; j++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x1, &rep, _state);
        seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Solve boundary constrained problem
         */
        minnlccreate(n, &x0, &state, _state);
        minnlcsetalgoaul(&state, rho, aulits, _state);
        minnlcsetbc(&state, &bndl, &bndu, _state);
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                    state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                    for(j=0; j<=n-1; j++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x2, &rep, _state);
        seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Compare solutions
         */
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-x2.ptr.p_double[i], _state),tolx), _state);
        }
    }
    
    /*
     * Boundary and linear equality constrained QP problem with excessive
     * equality constraints:
     * * N*N SPD A with moderate condtion number (up to 100)
     * * boundary constraints 0<=x[i]<=1
     * * K=2*N equality constraints Q*x = Q*x0, where Q is random matrix,
     *   x0 is some random vector from the feasible hypercube (0.1<=x0[i]<=0.9)
     * * optimization problem has form 0.5*x'*A*x-b*x,
     *   where b is some random vector
     * * because constraints are excessive, the main problem is to find
     *   feasible point; the only existing feasible point is solution,
     *   so we have to check only feasibility
     */
    rho = 200.0;
    tolx = 0.0005;
    aulits = 50;
    for(n=1; n<=6; n++)
    {
        
        /*
         * Generate problem: A, b, BndL, BndU, CMatrix, x0, xm, XStart
         */
        k = 2*n;
        spdmatrixrndcond(n, 1.0E2, &fulla, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&xm, n, _state);
        ae_vector_set_length(&xstart, n, _state);
        ae_matrix_set_length(&c, k, n+1, _state);
        ae_vector_set_length(&ct, k, _state);
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 0.1+0.8*ae_randomreal(_state);
            xm.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            bndl.ptr.p_double[i] = 0.0;
            bndu.ptr.p_double[i] = 1.0;
            xstart.ptr.p_double[i] = (double)(ae_randominteger(2, _state));
        }
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
            c.ptr.pp_double[i][n] = v;
            ct.ptr.p_int[i] = 0;
        }
        for(i=0; i<=n-1; i++)
        {
            b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        
        /*
         * Create optimizer, solve
         */
        minnlccreate(n, &xstart, &state, _state);
        minnlcsetalgoaul(&state, rho, aulits, _state);
        minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
        minnlcsetlc(&state, &c, &ct, k, _state);
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                    state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                    for(j=0; j<=n-1; j++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x1, &rep, _state);
        seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=k-1; i++)
        {
            v = ae_v_dotproduct(&x1.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
            seterrorflag(wereerrors, ae_fp_greater(ae_fabs(v-c.ptr.pp_double[i][n], _state),tolx), _state);
        }
    }
    
    /*
     * Boundary and linear equality/inequality constrained QP problem with
     * excessive constraints:
     * * N*N SPD A with moderate condtion number (up to 100)
     * * boundary constraints 0<=x[i]<=1
     * * K=2*N equality/inequality constraints Q*x = Q*xm, where Q is random matrix,
     *   xm is some random vector from the feasible hypercube (0.1<=xm[i]<=0.9)
     * * optimization problem has form 0.5*x'*A*x-b*x,
     *   where b is some random vector
     * * because constraints are excessive, the main problem is to find
     *   feasible point; usually, the only existing feasible point is solution,
     *   so we have to check only feasibility
     *
     * NOTE: this problem is difficult one (estimates of Lagrange multipliers converge
     *       slowly), so we use relaxed tolerances - 0.01 instead of 0.0005
     */
    rho = 200.0;
    tolx = 0.01;
    aulits = 50;
    for(n=1; n<=6; n++)
    {
        
        /*
         * Generate problem: A, b, BndL, BndU, CMatrix, xm, x1, XStart
         */
        k = 2*n;
        spdmatrixrndcond(n, 1.0E2, &fulla, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        ae_vector_set_length(&xm, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_matrix_set_length(&c, k, n+1, _state);
        ae_vector_set_length(&ct, k, _state);
        for(i=0; i<=n-1; i++)
        {
            xm.ptr.p_double[i] = 0.1+0.8*ae_randomreal(_state);
            bndl.ptr.p_double[i] = 0.0;
            bndu.ptr.p_double[i] = 1.0;
            x0.ptr.p_double[i] = (double)(ae_randominteger(2, _state));
            b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &xm.ptr.p_double[0], 1, ae_v_len(0,n-1));
            ct.ptr.p_int[i] = ae_randominteger(3, _state)-1;
            if( ct.ptr.p_int[i]==0 )
            {
                c.ptr.pp_double[i][n] = v;
            }
            if( ct.ptr.p_int[i]>0 )
            {
                c.ptr.pp_double[i][n] = v;
            }
            if( ct.ptr.p_int[i]<0 )
            {
                c.ptr.pp_double[i][n] = v;
            }
        }
        
        /*
         * Create optimizer, solve
         */
        minnlccreate(n, &x0, &state, _state);
        minnlcsetalgoaul(&state, rho, aulits, _state);
        minnlcsetbc(&state, &bndl, &bndu, _state);
        minnlcsetlc(&state, &c, &ct, k, _state);
        minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                    state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                    for(j=0; j<=n-1; j++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x1, &rep, _state);
        seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=k-1; i++)
        {
            v = ae_v_dotproduct(&x1.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
            if( ct.ptr.p_int[i]==0 )
            {
                seterrorflag(wereerrors, ae_fp_greater(ae_fabs(v-c.ptr.pp_double[i][n], _state),tolx), _state);
            }
            if( ct.ptr.p_int[i]>0 )
            {
                seterrorflag(wereerrors, ae_fp_less(v,c.ptr.pp_double[i][n]-tolx), _state);
            }
            if( ct.ptr.p_int[i]<0 )
            {
                seterrorflag(wereerrors, ae_fp_greater(v,c.ptr.pp_double[i][n]+tolx), _state);
            }
        }
    }
    
    /*
     * Boundary and linear equality constrained QP problem,
     * test checks that different starting points yield same final point:
     * * random N from [1..6], random K from [1..N-1]
     * * N*N SPD A with moderate condtion number (important!)
     * * boundary constraints 0<=x[i]<=1
     * * K<N random linear equality constraints C*x = C*x0,
     *   where x0 is some random vector from the inner area of the
     *   feasible hypercube (0.1<=x0[i]<=0.9)
     * * optimization problem has form 0.5*x'*A*x+b*x,
     *   where b is some random vector with -5<=b[i]<=+5
     *
     * We solve this problem two times:
     * * each time from different initial point XStart in [-2,+2]
     * * we compare values of the target function (although final points
     *   may be slightly different, function values should match each other)
     * 
     * Both points should give same results; any significant difference is
     * evidence of some error in the QP implementation.
     */
    rho = 1000.0;
    tolf = 0.01;
    aulits = 50;
    for(pass=1; pass<=50; pass++)
    {
        
        /*
         * Generate problem: N, K, A, b, BndL, BndU, CMatrix, x0, xm, XStart.
         */
        n = ae_randominteger(5, _state)+2;
        k = 1+ae_randominteger(n-1, _state);
        spdmatrixrndcond(n, 1.0E2, &fulla, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&xstart, n, _state);
        ae_matrix_set_length(&c, k, n+1, _state);
        ae_vector_set_length(&ct, k, _state);
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 0.1+0.8*ae_randomreal(_state);
            b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            bndl.ptr.p_double[i] = 0.0;
            bndu.ptr.p_double[i] = 1.0;
        }
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
            c.ptr.pp_double[i][n] = v;
            ct.ptr.p_int[i] = 0;
        }
        
        /*
         * Start from first point
         */
        for(i=0; i<=n-1; i++)
        {
            xstart.ptr.p_double[i] = 4*ae_randomreal(_state)-2;
        }
        minnlccreate(n, &xstart, &state, _state);
        minnlcsetalgoaul(&state, rho, aulits, _state);
        minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
        minnlcsetbc(&state, &bndl, &bndu, _state);
        minnlcsetlc(&state, &c, &ct, k, _state);
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                    state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                    for(j=0; j<=n-1; j++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x0, &rep, _state);
        seterrorflag(wereerrors, !isfinitevector(&x0, n, _state), _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Start from another point
         */
        for(i=0; i<=n-1; i++)
        {
            xstart.ptr.p_double[i] = 4*ae_randomreal(_state)-2;
        }
        minnlcrestartfrom(&state, &xstart, _state);
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                    state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                    for(j=0; j<=n-1; j++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x1, &rep, _state);
        seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Calculate function value at X0 and X1, compare solutions
         */
        f0 = 0.0;
        f1 = 0.0;
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                f0 = f0+0.5*x0.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*x0.ptr.p_double[j];
                f1 = f1+0.5*x1.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*x1.ptr.p_double[j];
            }
            f0 = f0+x0.ptr.p_double[i]*b.ptr.p_double[i];
            f1 = f1+x1.ptr.p_double[i]*b.ptr.p_double[i];
        }
        seterrorflag(wereerrors, ae_fp_greater(ae_fabs(f0-f1, _state),tolf), _state);
    }
    
    /*
     * Convex/nonconvex optimization problem with excessive
     * (degenerate constraints):
     *
     * * N=2..5
     * * f = 0.5*x'*A*x+b'*x
     * * b has normally distributed entries with scale 10^BScale
     * * several kinds of A are tried: zero, well conditioned SPD, well conditioned indefinite, low rank
     * * box constraints: x[i] in [-1,+1]
     * * 2^N "excessive" general linear constraints (v_k,x)<=(v_k,v_k)+v_shift,
     *   where v_k is one of 2^N vertices of feasible hypercube, v_shift is
     *   a shift parameter:
     *   * with zero v_shift such constraints are degenerate (each vertex has
     *     N box constraints and one "redundant" linear constraint)
     *   * with positive v_shift linear constraint is always inactive
     *   * with small (about machine epsilon) but negative v_shift,
     *     constraint is close to degenerate - but not exactly
     *
     * We check that constrained gradient is close to zero at solution.
     * Box constraint is considered active if distance to boundary is less
     * than TolConstr.
     *
     * NOTE: because AUL algorithm is less exact than its active set counterparts,
     *       VERY loose tolerances are used for this test.
     */
    tolconstr = 1.0E-2;
    for(n=2; n<=8; n++)
    {
        for(akind=0; akind<=3; akind++)
        {
            for(shiftkind=-5; shiftkind<=1; shiftkind++)
            {
                for(bscale=0; bscale>=-2; bscale--)
                {
                    
                    /*
                     * Generate A, B and initial point
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    ae_vector_set_length(&b, n, _state);
                    ae_vector_set_length(&x, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        b.ptr.p_double[i] = ae_pow((double)(10), (double)(bscale), _state)*hqrndnormal(&rs, _state);
                        x.ptr.p_double[i] = hqrnduniformr(&rs, _state)-0.5;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 0.0;
                        }
                    }
                    if( akind==1 )
                    {
                        
                        /*
                         * Dense well conditioned SPD
                         */
                        spdmatrixrndcond(n, 50.0, &a, _state);
                    }
                    if( akind==2 )
                    {
                        
                        /*
                         * Dense well conditioned indefinite
                         */
                        smatrixrndcond(n, 50.0, &a, _state);
                    }
                    if( akind==3 )
                    {
                        
                        /*
                         * Low rank
                         */
                        ae_vector_set_length(&tmp, n, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                a.ptr.pp_double[i][j] = 0.0;
                            }
                        }
                        for(k=1; k<=ae_minint(3, n-1, _state); k++)
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                tmp.ptr.p_double[i] = hqrndnormal(&rs, _state);
                            }
                            v = hqrndnormal(&rs, _state);
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    a.ptr.pp_double[i][j] = a.ptr.pp_double[i][j]+v*tmp.ptr.p_double[i]*tmp.ptr.p_double[j];
                                }
                            }
                        }
                    }
                    
                    /*
                     * Generate constraints
                     */
                    ae_vector_set_length(&bl, n, _state);
                    ae_vector_set_length(&bu, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        bl.ptr.p_double[i] = -1.0;
                        bu.ptr.p_double[i] = 1.0;
                    }
                    ccnt = ae_round(ae_pow((double)(2), (double)(n), _state), _state);
                    ae_matrix_set_length(&c, ccnt, n+1, _state);
                    ae_vector_set_length(&ct, ccnt, _state);
                    for(i=0; i<=ccnt-1; i++)
                    {
                        ct.ptr.p_int[i] = -1;
                        k = i;
                        c.ptr.pp_double[i][n] = ae_sign((double)(shiftkind), _state)*ae_pow((double)(10), ae_fabs((double)(shiftkind), _state), _state)*ae_machineepsilon;
                        for(j=0; j<=n-1; j++)
                        {
                            c.ptr.pp_double[i][j] = (double)(2*(k%2)-1);
                            c.ptr.pp_double[i][n] = c.ptr.pp_double[i][n]+c.ptr.pp_double[i][j]*c.ptr.pp_double[i][j];
                            k = k/2;
                        }
                    }
                    
                    /*
                     * Create and optimize
                     */
                    minnlccreate(n, &x, &state, _state);
                    minnlcsetbc(&state, &bl, &bu, _state);
                    minnlcsetlc(&state, &c, &ct, ccnt, _state);
                    minnlcsetcond(&state, 1.0E-12, 0.0, 0.0, 0, _state);
                    minnlcsetalgoaul(&state, 1000.0, 10, _state);
                    while(minnlciteration(&state, _state))
                    {
                        ae_assert(state.needfij, "Assertion failed", _state);
                        state.fi.ptr.p_double[0] = (double)(0);
                        for(i=0; i<=n-1; i++)
                        {
                            state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+state.x.ptr.p_double[i]*b.ptr.p_double[i];
                            state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                            state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*v;
                            state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+v;
                        }
                    }
                    minnlcresults(&state, &xs0, &rep, _state);
                    seterrorflag(wereerrors, rep.terminationtype<=0, _state);
                    if( *wereerrors )
                    {
                        ae_frame_leave(_state);
                        return;
                    }
                    
                    /*
                     * Evaluate gradient at solution and test
                     */
                    vv = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xs0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                        v = v+b.ptr.p_double[i];
                        if( ae_fp_less_eq(xs0.ptr.p_double[i],bl.ptr.p_double[i]+tolconstr)&&ae_fp_greater(v,(double)(0)) )
                        {
                            v = 0.0;
                        }
                        if( ae_fp_greater_eq(xs0.ptr.p_double[i],bu.ptr.p_double[i]-tolconstr)&&ae_fp_less(v,(double)(0)) )
                        {
                            v = 0.0;
                        }
                        vv = vv+ae_sqr(v, _state);
                    }
                    vv = ae_sqrt(vv, _state);
                    seterrorflag(wereerrors, ae_fp_greater(vv,1.0E-3), _state);
                }
            }
        }
    }
    
    /*
     * Linear/convex optimization problem with combination of
     * box and linear constraints:
     *
     * * N=2..8
     * * f = 0.5*x'*A*x+b'*x
     * * b has normally distributed entries with scale 10^BScale
     * * several kinds of A are tried: zero, well conditioned SPD
     * * box constraints: x[i] in [-1,+1]
     * * initial point x0 = [0 0 ... 0 0]
     * * CCnt=min(3,N-1) general linear constraints of form (c,x)=0.
     *   random mix of equality/inequality constraints is tried.
     *   x0 is guaranteed to be feasible.
     *
     * We check that constrained gradient is close to zero at solution.
     * Inequality constraint is considered active if distance to boundary
     * is less than TolConstr. We use nonnegative least squares solver
     * in order to compute constrained gradient.
     */
    tolconstr = 1.0E-2;
    for(n=2; n<=8; n++)
    {
        for(akind=0; akind<=1; akind++)
        {
            for(bscale=0; bscale>=-2; bscale--)
            {
                
                /*
                 * Generate A, B and initial point
                 */
                ae_matrix_set_length(&a, n, n, _state);
                ae_vector_set_length(&b, n, _state);
                ae_vector_set_length(&x, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    b.ptr.p_double[i] = ae_pow((double)(10), (double)(bscale), _state)*hqrndnormal(&rs, _state);
                    x.ptr.p_double[i] = 0.0;
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a.ptr.pp_double[i][j] = 0.0;
                    }
                }
                if( akind==1 )
                {
                    
                    /*
                     * Dense well conditioned SPD
                     */
                    spdmatrixrndcond(n, 50.0, &a, _state);
                }
                
                /*
                 * Generate constraints
                 */
                ae_vector_set_length(&bl, n, _state);
                ae_vector_set_length(&bu, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    bl.ptr.p_double[i] = -1.0;
                    bu.ptr.p_double[i] = 1.0;
                }
                ccnt = ae_minint(3, n-1, _state);
                ae_matrix_set_length(&c, ccnt, n+1, _state);
                ae_vector_set_length(&ct, ccnt, _state);
                for(i=0; i<=ccnt-1; i++)
                {
                    ct.ptr.p_int[i] = hqrnduniformi(&rs, 3, _state)-1;
                    c.ptr.pp_double[i][n] = 0.0;
                    for(j=0; j<=n-1; j++)
                    {
                        c.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                    }
                }
                
                /*
                 * Create and optimize
                 */
                minnlccreate(n, &x, &state, _state);
                minnlcsetbc(&state, &bl, &bu, _state);
                minnlcsetlc(&state, &c, &ct, ccnt, _state);
                minnlcsetcond(&state, 1.0E-9, 0.0, 0.0, 0, _state);
                minnlcsetalgoaul(&state, 1000.0, 10, _state);
                while(minnlciteration(&state, _state))
                {
                    ae_assert(state.needfij, "Assertion failed", _state);
                    state.fi.ptr.p_double[0] = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+state.x.ptr.p_double[i]*b.ptr.p_double[i];
                        state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*v;
                        state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+v;
                    }
                }
                minnlcresults(&state, &xs0, &rep, _state);
                seterrorflag(wereerrors, rep.terminationtype<=0, _state);
                if( *wereerrors )
                {
                    ae_frame_leave(_state);
                    return;
                }
                
                /*
                 * 1. evaluate unconstrained gradient at solution
                 *
                 * 2. calculate constrained gradient (NNLS solver is used
                 *    to evaluate gradient subject to active constraints).
                 *    In order to do this we form CE matrix, matrix of active
                 *    constraints (columns store constraint vectors). Then
                 *    we try to approximate gradient vector by columns of CE,
                 *    subject to non-negativity restriction placed on variables
                 *    corresponding to inequality constraints.
                 *
                 *    Residual from such regression is a constrained gradient vector.
                 */
                ae_vector_set_length(&g, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xs0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    g.ptr.p_double[i] = v+b.ptr.p_double[i];
                }
                ae_matrix_set_length(&ce, n, n+ccnt, _state);
                ae_vector_set_length(&nonnegative, n+ccnt, _state);
                k = 0;
                for(i=0; i<=n-1; i++)
                {
                    seterrorflag(wereerrors, ae_fp_less(xs0.ptr.p_double[i],bl.ptr.p_double[i]-tolconstr), _state);
                    seterrorflag(wereerrors, ae_fp_greater(xs0.ptr.p_double[i],bu.ptr.p_double[i]+tolconstr), _state);
                    if( ae_fp_less_eq(xs0.ptr.p_double[i],bl.ptr.p_double[i]+tolconstr) )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = 0.0;
                        }
                        ce.ptr.pp_double[i][k] = 1.0;
                        nonnegative.ptr.p_bool[k] = ae_true;
                        inc(&k, _state);
                        continue;
                    }
                    if( ae_fp_greater_eq(xs0.ptr.p_double[i],bu.ptr.p_double[i]-tolconstr) )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = 0.0;
                        }
                        ce.ptr.pp_double[i][k] = -1.0;
                        nonnegative.ptr.p_bool[k] = ae_true;
                        inc(&k, _state);
                        continue;
                    }
                }
                for(i=0; i<=ccnt-1; i++)
                {
                    v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &xs0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    v = v-c.ptr.pp_double[i][n];
                    seterrorflag(wereerrors, ct.ptr.p_int[i]==0&&ae_fp_greater(ae_fabs(v, _state),tolconstr), _state);
                    seterrorflag(wereerrors, ct.ptr.p_int[i]>0&&ae_fp_less(v,-tolconstr), _state);
                    seterrorflag(wereerrors, ct.ptr.p_int[i]<0&&ae_fp_greater(v,tolconstr), _state);
                    if( ct.ptr.p_int[i]==0 )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = c.ptr.pp_double[i][j];
                        }
                        nonnegative.ptr.p_bool[k] = ae_false;
                        inc(&k, _state);
                        continue;
                    }
                    if( (ct.ptr.p_int[i]>0&&ae_fp_less_eq(v,tolconstr))||(ct.ptr.p_int[i]<0&&ae_fp_greater_eq(v,-tolconstr)) )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = ae_sign((double)(ct.ptr.p_int[i]), _state)*c.ptr.pp_double[i][j];
                        }
                        nonnegative.ptr.p_bool[k] = ae_true;
                        inc(&k, _state);
                        continue;
                    }
                }
                snnlsinit(0, 0, 0, &nnls, _state);
                snnlssetproblem(&nnls, &ce, &g, 0, k, n, _state);
                for(i=0; i<=k-1; i++)
                {
                    if( !nonnegative.ptr.p_bool[i] )
                    {
                        snnlsdropnnc(&nnls, i, _state);
                    }
                }
                snnlssolve(&nnls, &tmp, _state);
                for(i=0; i<=k-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        g.ptr.p_double[j] = g.ptr.p_double[j]-tmp.ptr.p_double[i]*ce.ptr.pp_double[j][i];
                    }
                }
                vv = ae_v_dotproduct(&g.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,n-1));
                vv = ae_sqrt(vv, _state);
                seterrorflag(wereerrors, ae_fp_greater(vv,1.0E-3), _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests nonlinearly constrained quadratic programming algorithm.

Sets error flag on failure.
*************************************************************************/
static void testminnlcunit_testnlc(ae_bool* wereerrors, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t n2;
    double tolx;
    double tolg;
    ae_int_t aulits;
    double rho;
    minnlcstate state;
    minnlcreport rep;
    ae_int_t scaletype;
    ae_vector x0;
    ae_vector x1;
    ae_vector b;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector s;
    ae_vector g;
    ae_vector ckind;
    ae_matrix fulla;
    ae_matrix c;
    ae_vector ct;
    ae_int_t cntbc;
    ae_int_t cntlc;
    ae_int_t cntnlec;
    ae_int_t cntnlic;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t pass;
    ae_int_t klc;
    ae_int_t knlc;
    ae_int_t knlec;
    ae_int_t knlic;
    double v;
    double vv;
    double vx;
    double vy;
    double gnorm;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    _minnlcstate_init(&state, _state);
    _minnlcreport_init(&rep, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&ckind, 0, DT_INT, _state);
    ae_matrix_init(&fulla, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Basic test:
     * * 2-dimensional problem
     * * target function F(x0,x1) = (x0-1)^2 + (x1-1)^2
     * * one nonlinear constraint Z(x0,x1) = x0^2+x1^2-1,
     *   which is tried as equality and inequality one
     */
    rho = 200.0;
    tolx = 0.0005;
    aulits = 50;
    n = 2;
    ae_vector_set_length(&x0, n, _state);
    for(i=0; i<=n-1; i++)
    {
        x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
    }
    minnlccreate(n, &x0, &state, _state);
    minnlcsetalgoaul(&state, rho, aulits, _state);
    minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
    minnlcsetnlc(&state, 0, 1, _state);
    while(minnlciteration(&state, _state))
    {
        if( state.needfij )
        {
            state.fi.ptr.p_double[0] = ae_sqr(state.x.ptr.p_double[0]-1, _state)+ae_sqr(state.x.ptr.p_double[1]-1, _state);
            state.j.ptr.pp_double[0][0] = 2*(state.x.ptr.p_double[0]-1);
            state.j.ptr.pp_double[0][1] = 2*(state.x.ptr.p_double[1]-1);
            state.fi.ptr.p_double[1] = ae_sqr(state.x.ptr.p_double[0], _state)+ae_sqr(state.x.ptr.p_double[1], _state)-1;
            state.j.ptr.pp_double[1][0] = 2*state.x.ptr.p_double[0];
            state.j.ptr.pp_double[1][1] = 2*state.x.ptr.p_double[1];
            continue;
        }
        ae_assert(ae_false, "Assertion failed", _state);
    }
    minnlcresults(&state, &x1, &rep, _state);
    seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
    seterrorflag(wereerrors, rep.terminationtype<=0, _state);
    if( *wereerrors )
    {
        ae_frame_leave(_state);
        return;
    }
    seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[0]-ae_sqrt((double)(2), _state)/2, _state),tolx), _state);
    seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[1]-ae_sqrt((double)(2), _state)/2, _state),tolx), _state);
    minnlcsetnlc(&state, 1, 0, _state);
    minnlcrestartfrom(&state, &x0, _state);
    while(minnlciteration(&state, _state))
    {
        if( state.needfij )
        {
            state.fi.ptr.p_double[0] = ae_sqr(state.x.ptr.p_double[0]-1, _state)+ae_sqr(state.x.ptr.p_double[1]-1, _state);
            state.j.ptr.pp_double[0][0] = 2*(state.x.ptr.p_double[0]-1);
            state.j.ptr.pp_double[0][1] = 2*(state.x.ptr.p_double[1]-1);
            state.fi.ptr.p_double[1] = ae_sqr(state.x.ptr.p_double[0], _state)+ae_sqr(state.x.ptr.p_double[1], _state)-1;
            state.j.ptr.pp_double[1][0] = 2*state.x.ptr.p_double[0];
            state.j.ptr.pp_double[1][1] = 2*state.x.ptr.p_double[1];
            continue;
        }
        ae_assert(ae_false, "Assertion failed", _state);
    }
    minnlcresults(&state, &x1, &rep, _state);
    seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
    seterrorflag(wereerrors, rep.terminationtype<=0, _state);
    if( *wereerrors )
    {
        ae_frame_leave(_state);
        return;
    }
    seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[0]-ae_sqrt((double)(2), _state)/2, _state),tolx), _state);
    seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[1]-ae_sqrt((double)(2), _state)/2, _state),tolx), _state);
    
    /*
     * This test checks correctness of scaling being applied to nonlinear
     * constraints. We solve bound constrained scaled problem and check
     * that solution is correct.
     */
    aulits = 50;
    rho = 200.0;
    tolx = 0.0005;
    tolg = 0.01;
    for(n=1; n<=10; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate well-conditioned problem with unit scale
             */
            spdmatrixrndcond(n, 1.0E2, &fulla, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                x0.ptr.p_double[i] = hqrndnormal(&rs, _state);
            }
            cntnlec = hqrnduniformi(&rs, n, _state);
            cntnlic = n-cntnlec;
            for(i=0; i<=cntnlec-1; i++)
            {
                bndl.ptr.p_double[i] = hqrndnormal(&rs, _state);
                bndu.ptr.p_double[i] = bndl.ptr.p_double[i];
            }
            for(i=cntnlec; i<=n-1; i++)
            {
                bndl.ptr.p_double[i] = hqrndnormal(&rs, _state);
                bndu.ptr.p_double[i] = bndl.ptr.p_double[i]+0.5;
            }
            
            /*
             * Apply scaling to quadratic/linear term, so problem becomes
             * well-conditioned in the scaled coordinates.
             */
            scaletype = hqrnduniformi(&rs, 2, _state);
            ae_vector_set_length(&s, n, _state);
            for(i=0; i<=n-1; i++)
            {
                if( scaletype==0 )
                {
                    s.ptr.p_double[i] = (double)(1);
                }
                else
                {
                    s.ptr.p_double[i] = ae_exp(20*hqrndnormal(&rs, _state), _state);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = x0.ptr.p_double[i]*s.ptr.p_double[i];
                bndl.ptr.p_double[i] = bndl.ptr.p_double[i]*s.ptr.p_double[i];
                bndu.ptr.p_double[i] = bndu.ptr.p_double[i]*s.ptr.p_double[i];
                b.ptr.p_double[i] = b.ptr.p_double[i]/s.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    fulla.ptr.pp_double[i][j] = fulla.ptr.pp_double[i][j]/(s.ptr.p_double[i]*s.ptr.p_double[j]);
                }
            }
            
            /*
             * Solve problem with boundary constraints posed as nonlinear ones
             */
            minnlccreate(n, &x0, &state, _state);
            minnlcsetalgoaul(&state, rho, aulits, _state);
            minnlcsetscale(&state, &s, _state);
            minnlcsetnlc(&state, cntnlec, 2*cntnlic, _state);
            minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
            while(minnlciteration(&state, _state))
            {
                if( state.needfij )
                {
                    for(i=0; i<=cntnlec+2*cntnlic; i++)
                    {
                        state.fi.ptr.p_double[i] = (double)(0);
                        for(j=0; j<=n-1; j++)
                        {
                            state.j.ptr.pp_double[i][j] = (double)(0);
                        }
                    }
                    
                    /*
                     * Function itself
                     */
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                        state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                        for(j=0; j<=n-1; j++)
                        {
                            state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                            state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        }
                    }
                    
                    /*
                     * Equality constraints
                     */
                    for(i=0; i<=cntnlec-1; i++)
                    {
                        state.fi.ptr.p_double[1+i] = (state.x.ptr.p_double[i]-bndl.ptr.p_double[i])/s.ptr.p_double[i];
                        state.j.ptr.pp_double[1+i][i] = 1/s.ptr.p_double[i];
                    }
                    
                    /*
                     * Inequality constraints
                     */
                    for(i=0; i<=cntnlic-1; i++)
                    {
                        k = cntnlec+i;
                        state.fi.ptr.p_double[1+cntnlec+2*i+0] = (bndl.ptr.p_double[k]-state.x.ptr.p_double[k])/s.ptr.p_double[k];
                        state.j.ptr.pp_double[1+cntnlec+2*i+0][k] = -1/s.ptr.p_double[k];
                        state.fi.ptr.p_double[1+cntnlec+2*i+1] = (state.x.ptr.p_double[k]-bndu.ptr.p_double[k])/s.ptr.p_double[k];
                        state.j.ptr.pp_double[1+cntnlec+2*i+1][k] = 1/s.ptr.p_double[k];
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnlcresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( *wereerrors )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Check feasibility properties
             */
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(wereerrors, ae_isfinite(bndl.ptr.p_double[i], _state)&&ae_fp_less_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i]-tolx*s.ptr.p_double[i]), _state);
                seterrorflag(wereerrors, ae_isfinite(bndu.ptr.p_double[i], _state)&&ae_fp_greater_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i]+tolx*s.ptr.p_double[i]), _state);
            }
            
            /*
             * Test - calculate scaled constrained gradient at solution,
             * check its norm.
             */
            ae_vector_set_length(&g, n, _state);
            gnorm = 0.0;
            for(i=0; i<=n-1; i++)
            {
                g.ptr.p_double[i] = b.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    g.ptr.p_double[i] = g.ptr.p_double[i]+fulla.ptr.pp_double[i][j]*x1.ptr.p_double[j];
                }
                g.ptr.p_double[i] = s.ptr.p_double[i]*g.ptr.p_double[i];
                if( (ae_isfinite(bndl.ptr.p_double[i], _state)&&ae_fp_less(ae_fabs(x1.ptr.p_double[i]-bndl.ptr.p_double[i], _state),tolx*s.ptr.p_double[i]))&&ae_fp_greater(g.ptr.p_double[i],(double)(0)) )
                {
                    g.ptr.p_double[i] = (double)(0);
                }
                if( (ae_isfinite(bndu.ptr.p_double[i], _state)&&ae_fp_less(ae_fabs(x1.ptr.p_double[i]-bndu.ptr.p_double[i], _state),tolx*s.ptr.p_double[i]))&&ae_fp_less(g.ptr.p_double[i],(double)(0)) )
                {
                    g.ptr.p_double[i] = (double)(0);
                }
                gnorm = gnorm+ae_sqr(g.ptr.p_double[i], _state);
            }
            gnorm = ae_sqrt(gnorm, _state);
            seterrorflag(wereerrors, ae_fp_greater(gnorm,tolg), _state);
        }
    }
    
    /*
     * Complex problem with mix of boundary, linear and nonlinear constraints:
     * * quadratic target function f(x) = 0.5*x'*A*x + b'*x
     * * unit scaling is used
     * * problem size N is even
     * * all variables are divided into pairs: x[0] and x[1], x[2] and x[3], ...
     * * constraints are set for pairs of variables, i.e. each constraint involves
     *   only pair of adjacent variables (x0/x1, x2/x3, x4/x5 and so on), and each
     *   pair of variables has at most one constraint which binds them
     * * for variables u and v following kinds of constraints can be randomly set:
     *   * CKind=0      no constraint
     *   * CKind=1      boundary equality constraint:    u=a, v=b
     *   * CKind=2      boundary inequality constraint:  a0<=u<=b0, a1<=v<=b1
     *   * CKind=3      linear equality constraint:      a*u+b*v  = c
     *   * CKind=4      linear inequality constraint:    a*u+b*v <= c
     *   * CKind=5      nonlinear equality constraint:   u^2+v^2  = 1
     *   * CKind=6      nonlinear inequality constraint: u^2+v^2 <= 1
     * * it is relatively easy to calculated projected gradient for such problem
     */
    aulits = 50;
    rho = 200.0;
    tolx = 0.0005;
    tolg = 0.01;
    n = 20;
    n2 = n/2;
    for(pass=1; pass<=50; pass++)
    {
        
        /*
         * Generate well-conditioned problem with unit scale
         */
        spdmatrixrndcond(n, 1.0E2, &fulla, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        ae_matrix_set_length(&c, n, n+1, _state);
        ae_vector_set_length(&ct, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&ckind, n2, _state);
        cntbc = 0;
        cntlc = 0;
        cntnlec = 0;
        cntnlic = 0;
        for(i=0; i<=n-1; i++)
        {
            bndl.ptr.p_double[i] = _state->v_neginf;
            bndu.ptr.p_double[i] = _state->v_posinf;
            x0.ptr.p_double[i] = hqrndnormal(&rs, _state);
            b.ptr.p_double[i] = hqrndnormal(&rs, _state);
        }
        for(i=0; i<=n2-1; i++)
        {
            ckind.ptr.p_int[i] = hqrnduniformi(&rs, 7, _state);
            if( ckind.ptr.p_int[i]==0 )
            {
                
                /*
                 * Unconstrained
                 */
                continue;
            }
            if( ckind.ptr.p_int[i]==1 )
            {
                
                /*
                 * Bound equality constrained
                 */
                bndl.ptr.p_double[2*i+0] = hqrnduniformr(&rs, _state)-0.5;
                bndu.ptr.p_double[2*i+0] = bndl.ptr.p_double[2*i+0];
                bndl.ptr.p_double[2*i+1] = hqrnduniformr(&rs, _state)-0.5;
                bndu.ptr.p_double[2*i+1] = bndl.ptr.p_double[2*i+1];
                inc(&cntbc, _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==2 )
            {
                
                /*
                 * Bound inequality constrained
                 */
                bndl.ptr.p_double[2*i+0] = hqrnduniformr(&rs, _state)-0.5;
                bndu.ptr.p_double[2*i+0] = bndl.ptr.p_double[2*i+0]+0.5;
                bndl.ptr.p_double[2*i+1] = hqrnduniformr(&rs, _state)-0.5;
                bndu.ptr.p_double[2*i+1] = bndl.ptr.p_double[2*i+1]+0.5;
                inc(&cntbc, _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==3 )
            {
                
                /*
                 * Linear equality constrained
                 */
                for(j=0; j<=n; j++)
                {
                    c.ptr.pp_double[cntlc][j] = 0.0;
                }
                vx = hqrnduniformr(&rs, _state)-0.5;
                vy = hqrnduniformr(&rs, _state)-0.5;
                c.ptr.pp_double[cntlc][2*i+0] = vx;
                c.ptr.pp_double[cntlc][2*i+1] = vy;
                c.ptr.pp_double[cntlc][n] = hqrnduniformr(&rs, _state)-0.5;
                ct.ptr.p_int[cntlc] = 0;
                inc(&cntlc, _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==4 )
            {
                
                /*
                 * Linear inequality constrained
                 */
                for(j=0; j<=n; j++)
                {
                    c.ptr.pp_double[cntlc][j] = 0.0;
                }
                vx = hqrnduniformr(&rs, _state)-0.5;
                vy = hqrnduniformr(&rs, _state)-0.5;
                c.ptr.pp_double[cntlc][2*i+0] = vx;
                c.ptr.pp_double[cntlc][2*i+1] = vy;
                c.ptr.pp_double[cntlc][n] = hqrnduniformr(&rs, _state)-0.5;
                ct.ptr.p_int[cntlc] = -1;
                inc(&cntlc, _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==5 )
            {
                
                /*
                 * Nonlinear equality constrained
                 */
                inc(&cntnlec, _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==6 )
            {
                
                /*
                 * Nonlinear inequality constrained
                 */
                inc(&cntnlic, _state);
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        
        /*
         * Solve problem
         */
        minnlccreate(n, &x0, &state, _state);
        minnlcsetalgoaul(&state, rho, aulits, _state);
        minnlcsetbc(&state, &bndl, &bndu, _state);
        minnlcsetlc(&state, &c, &ct, cntlc, _state);
        minnlcsetnlc(&state, cntnlec, cntnlic, _state);
        minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                
                /*
                 * Evaluate target function
                 */
                state.fi.ptr.p_double[0] = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                    state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                    for(j=0; j<=n-1; j++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
                
                /*
                 * Evaluate constraint functions
                 */
                knlec = 1;
                knlic = 1+cntnlec;
                for(i=0; i<=n2-1; i++)
                {
                    if( ckind.ptr.p_int[i]==5 )
                    {
                        state.fi.ptr.p_double[knlec] = (double)(0);
                        for(j=0; j<=n-1; j++)
                        {
                            state.j.ptr.pp_double[knlec][j] = 0.0;
                        }
                        state.fi.ptr.p_double[knlec] = ae_sqr(state.x.ptr.p_double[2*i+0], _state)+ae_sqr(state.x.ptr.p_double[2*i+1], _state)-1;
                        state.j.ptr.pp_double[knlec][2*i+0] = 2*state.x.ptr.p_double[2*i+0];
                        state.j.ptr.pp_double[knlec][2*i+1] = 2*state.x.ptr.p_double[2*i+1];
                        inc(&knlec, _state);
                        continue;
                    }
                    if( ckind.ptr.p_int[i]==6 )
                    {
                        state.fi.ptr.p_double[knlic] = (double)(0);
                        for(j=0; j<=n-1; j++)
                        {
                            state.j.ptr.pp_double[knlic][j] = 0.0;
                        }
                        state.fi.ptr.p_double[knlic] = ae_sqr(state.x.ptr.p_double[2*i+0], _state)+ae_sqr(state.x.ptr.p_double[2*i+1], _state)-1;
                        state.j.ptr.pp_double[knlic][2*i+0] = 2*state.x.ptr.p_double[2*i+0];
                        state.j.ptr.pp_double[knlic][2*i+1] = 2*state.x.ptr.p_double[2*i+1];
                        inc(&knlic, _state);
                        continue;
                    }
                }
                ae_assert(knlec==1+cntnlec, "Assertion failed", _state);
                ae_assert(knlic==1+cntnlec+cntnlic, "Assertion failed", _state);
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x1, &rep, _state);
        seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Check feasibility properties
         */
        klc = 0;
        for(i=0; i<=n2-1; i++)
        {
            if( ckind.ptr.p_int[i]==0 )
            {
                
                /*
                 * Unconstrained
                 */
                continue;
            }
            if( ckind.ptr.p_int[i]==1 )
            {
                
                /*
                 * Bound equality constrained
                 */
                seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[2*i+0]-bndl.ptr.p_double[2*i+0], _state),tolx), _state);
                seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[2*i+1]-bndl.ptr.p_double[2*i+1], _state),tolx), _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==2 )
            {
                
                /*
                 * Bound inequality constrained
                 */
                seterrorflag(wereerrors, ae_fp_less(x1.ptr.p_double[2*i+0],bndl.ptr.p_double[2*i+0]-tolx), _state);
                seterrorflag(wereerrors, ae_fp_greater(x1.ptr.p_double[2*i+0],bndu.ptr.p_double[2*i+0]+tolx), _state);
                seterrorflag(wereerrors, ae_fp_less(x1.ptr.p_double[2*i+1],bndl.ptr.p_double[2*i+1]-tolx), _state);
                seterrorflag(wereerrors, ae_fp_greater(x1.ptr.p_double[2*i+1],bndu.ptr.p_double[2*i+1]+tolx), _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==3 )
            {
                
                /*
                 * Linear equality constrained
                 */
                v = x1.ptr.p_double[2*i+0]*c.ptr.pp_double[klc][2*i+0]+x1.ptr.p_double[2*i+1]*c.ptr.pp_double[klc][2*i+1]-c.ptr.pp_double[klc][n];
                seterrorflag(wereerrors, ae_fp_greater(ae_fabs(v, _state),tolx), _state);
                inc(&klc, _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==4 )
            {
                
                /*
                 * Linear inequality constrained
                 */
                v = x1.ptr.p_double[2*i+0]*c.ptr.pp_double[klc][2*i+0]+x1.ptr.p_double[2*i+1]*c.ptr.pp_double[klc][2*i+1]-c.ptr.pp_double[klc][n];
                seterrorflag(wereerrors, ae_fp_greater(v,tolx), _state);
                inc(&klc, _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==5 )
            {
                
                /*
                 * Nonlinear equality constrained
                 */
                v = ae_sqr(x1.ptr.p_double[2*i+0], _state)+ae_sqr(x1.ptr.p_double[2*i+1], _state)-1;
                seterrorflag(wereerrors, ae_fp_greater(ae_fabs(v, _state),tolx), _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==6 )
            {
                
                /*
                 * Nonlinear inequality constrained
                 */
                v = ae_sqr(x1.ptr.p_double[2*i+0], _state)+ae_sqr(x1.ptr.p_double[2*i+1], _state)-1;
                seterrorflag(wereerrors, ae_fp_greater(v,tolx), _state);
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        
        /*
         * Test - calculate scaled constrained gradient at solution,
         * check its norm.
         */
        ae_vector_set_length(&g, n, _state);
        for(i=0; i<=n-1; i++)
        {
            v = b.ptr.p_double[i];
            for(j=0; j<=n-1; j++)
            {
                v = v+fulla.ptr.pp_double[i][j]*x1.ptr.p_double[j];
            }
            g.ptr.p_double[i] = v;
        }
        klc = 0;
        knlc = 0;
        for(i=0; i<=n2-1; i++)
        {
            if( ckind.ptr.p_int[i]==0 )
            {
                
                /*
                 * Unconstrained
                 */
                continue;
            }
            if( ckind.ptr.p_int[i]==1 )
            {
                
                /*
                 * Bound equality constrained, unconditionally set gradient to zero
                 */
                g.ptr.p_double[2*i+0] = 0.0;
                g.ptr.p_double[2*i+1] = 0.0;
                continue;
            }
            if( ckind.ptr.p_int[i]==2 )
            {
                
                /*
                 * Bound inequality constrained, conditionally set gradient to zero
                 * (when constraint is active)
                 */
                if( ae_fp_less(x1.ptr.p_double[2*i+0],bndl.ptr.p_double[2*i+0]+tolx)||ae_fp_greater(x1.ptr.p_double[2*i+0],bndu.ptr.p_double[2*i+0]-tolx) )
                {
                    g.ptr.p_double[2*i+0] = 0.0;
                }
                if( ae_fp_less(x1.ptr.p_double[2*i+1],bndl.ptr.p_double[2*i+1]+tolx)||ae_fp_greater(x1.ptr.p_double[2*i+1],bndu.ptr.p_double[2*i+1]-tolx) )
                {
                    g.ptr.p_double[2*i+1] = 0.0;
                }
                continue;
            }
            if( ckind.ptr.p_int[i]==3 )
            {
                
                /*
                 * Linear equality constrained, unconditionally project gradient into
                 * equality constrained subspace
                 */
                v = g.ptr.p_double[2*i+0]*c.ptr.pp_double[klc][2*i+0]+g.ptr.p_double[2*i+1]*c.ptr.pp_double[klc][2*i+1];
                vv = ae_sqr(c.ptr.pp_double[klc][2*i+0], _state)+ae_sqr(c.ptr.pp_double[klc][2*i+1], _state);
                g.ptr.p_double[2*i+0] = g.ptr.p_double[2*i+0]-c.ptr.pp_double[klc][2*i+0]*(v/vv);
                g.ptr.p_double[2*i+1] = g.ptr.p_double[2*i+1]-c.ptr.pp_double[klc][2*i+1]*(v/vv);
                inc(&klc, _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==4 )
            {
                
                /*
                 * Linear inequality constrained, conditionally project gradient
                 * (when constraint is active)
                 */
                v = x1.ptr.p_double[2*i+0]*c.ptr.pp_double[klc][2*i+0]+x1.ptr.p_double[2*i+1]*c.ptr.pp_double[klc][2*i+1]-c.ptr.pp_double[klc][n];
                if( ae_fp_greater(v,-tolx) )
                {
                    v = g.ptr.p_double[2*i+0]*c.ptr.pp_double[klc][2*i+0]+g.ptr.p_double[2*i+1]*c.ptr.pp_double[klc][2*i+1];
                    vv = ae_sqr(c.ptr.pp_double[klc][2*i+0], _state)+ae_sqr(c.ptr.pp_double[klc][2*i+1], _state);
                    g.ptr.p_double[2*i+0] = g.ptr.p_double[2*i+0]-c.ptr.pp_double[klc][2*i+0]*(v/vv);
                    g.ptr.p_double[2*i+1] = g.ptr.p_double[2*i+1]-c.ptr.pp_double[klc][2*i+1]*(v/vv);
                }
                inc(&klc, _state);
                continue;
            }
            if( ckind.ptr.p_int[i]==5 )
            {
                
                /*
                 * Nonlinear equality constrained, unconditionally project gradient
                 *
                 * NOTE: here we rely on the fact that corresponding components of X
                 *       sum to one.
                 */
                v = g.ptr.p_double[2*i+0]*x1.ptr.p_double[2*i+0]+g.ptr.p_double[2*i+1]*x1.ptr.p_double[2*i+1];
                g.ptr.p_double[2*i+0] = g.ptr.p_double[2*i+0]-x1.ptr.p_double[2*i+0]*v;
                g.ptr.p_double[2*i+1] = g.ptr.p_double[2*i+1]-x1.ptr.p_double[2*i+1]*v;
                continue;
            }
            if( ckind.ptr.p_int[i]==6 )
            {
                
                /*
                 * Nonlinear inequality constrained, conditionally project gradient
                 * (when constraint is active)
                 *
                 * NOTE: here we rely on the fact that corresponding components of X
                 *       sum to one.
                 */
                v = ae_sqr(x1.ptr.p_double[2*i+0], _state)+ae_sqr(x1.ptr.p_double[2*i+1], _state)-1;
                if( ae_fp_greater(v,-tolx) )
                {
                    v = g.ptr.p_double[2*i+0]*x1.ptr.p_double[2*i+0]+g.ptr.p_double[2*i+1]*x1.ptr.p_double[2*i+1];
                    g.ptr.p_double[2*i+0] = g.ptr.p_double[2*i+0]-x1.ptr.p_double[2*i+0]*v;
                    g.ptr.p_double[2*i+1] = g.ptr.p_double[2*i+1]-x1.ptr.p_double[2*i+1]*v;
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        gnorm = 0.0;
        for(i=0; i<=n-1; i++)
        {
            gnorm = gnorm+ae_sqr(g.ptr.p_double[i], _state);
        }
        gnorm = ae_sqrt(gnorm, _state);
        seterrorflag(wereerrors, ae_fp_greater(gnorm,tolg), _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function performs additional tests

On failure sets error flag.
*************************************************************************/
static void testminnlcunit_testother(ae_bool* wereerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    hqrndstate rs;
    double v;
    double h;
    double fl;
    double fr;
    double fl2;
    double fr2;
    double dfl;
    double dfr;
    double dfl2;
    double dfr2;
    double d2fl;
    double d2fr;
    double d2fl2;
    double d2fr2;
    double f0;
    double df;
    double d2f;
    double ndf;
    double nd2f;
    double dtol;
    double diffstep;
    minnlcstate state;
    minnlcreport rep;
    double rho;
    ae_int_t aulits;
    double tolx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t n;
    ae_vector b;
    ae_vector x0;
    ae_vector x1;
    ae_vector x2;
    ae_vector x3;
    ae_vector xlast;
    ae_vector bndl;
    ae_vector bndu;
    double condv;
    ae_matrix a;
    ae_matrix c;
    ae_matrix fulla;
    ae_vector ct;
    ae_int_t nlbfgs;
    ae_int_t nexact;
    ae_int_t nnone;
    ae_int_t prectype;
    ae_int_t ctype;
    ae_int_t trialidx;
    ae_int_t blocksize;
    ae_int_t blockcnt;
    ae_int_t maxits;
    ae_int_t spoiliteration;
    ae_int_t stopiteration;
    ae_int_t spoilvar;
    double spoilval;
    ae_int_t pass;

    ae_frame_make(_state, &_frame_block);
    _hqrndstate_init(&rs, _state);
    _minnlcstate_init(&state, _state);
    _minnlcreport_init(&rep, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&x3, 0, DT_REAL, _state);
    ae_vector_init(&xlast, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&fulla, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Test equality penalty function (correctly calculated and smooth)
     */
    h = 1.0E-4;
    v = -0.98;
    dtol = 1.0E-3;
    while(ae_fp_less_eq(v,0.98))
    {
        
        /*
         * Test numerical derivative; this test also checks continuity of the
         * function
         */
        minnlcequalitypenaltyfunction(v-2*h, &fl2, &dfl2, &d2fl2, _state);
        minnlcequalitypenaltyfunction(v-h, &fl, &dfl, &d2fl, _state);
        minnlcequalitypenaltyfunction(v+h, &fr, &dfr, &d2fr, _state);
        minnlcequalitypenaltyfunction(v+2*h, &fr2, &dfr2, &d2fr2, _state);
        minnlcequalitypenaltyfunction(v, &f0, &df, &d2f, _state);
        ndf = (-fr2+8*fr-8*fl+fl2)/(12*h);
        seterrorflag(wereerrors, ae_fp_greater(ae_fabs(ndf-df, _state),dtol*ae_maxreal(ae_fabs(ndf, _state), (double)(1), _state)), _state);
        nd2f = (-dfr2+8*dfr-8*dfl+dfl2)/(12*h);
        seterrorflag(wereerrors, ae_fp_greater(ae_fabs(nd2f-d2f, _state),dtol*ae_maxreal(ae_fabs(nd2f, _state), (double)(1), _state)), _state);
        
        /*
         * Next point
         */
        v = v+h;
    }
    minnlcequalitypenaltyfunction(0.0, &f0, &df, &d2f, _state);
    seterrorflag(wereerrors, ae_fp_neq(f0,(double)(0)), _state);
    seterrorflag(wereerrors, ae_fp_neq(df,(double)(0)), _state);
    
    /*
     * Test inequality penalty function (correctly calculated and smooth)
     */
    h = 1.0E-4;
    v = 0.02;
    dtol = 1.0E-3;
    while(ae_fp_less_eq(v,2.00))
    {
        
        /*
         * Test numerical derivative; this test also checks continuity of the
         * function
         */
        minnlcinequalityshiftfunction(v-2*h, &fl2, &dfl2, &d2fl2, _state);
        minnlcinequalityshiftfunction(v-h, &fl, &dfl, &d2fl, _state);
        minnlcinequalityshiftfunction(v+h, &fr, &dfr, &d2fr, _state);
        minnlcinequalityshiftfunction(v+2*h, &fr2, &dfr2, &d2fr2, _state);
        minnlcinequalityshiftfunction(v, &f0, &df, &d2f, _state);
        ndf = (-fr2+8*fr-8*fl+fl2)/(12*h);
        seterrorflag(wereerrors, ae_fp_greater(ae_fabs(ndf-df, _state),dtol*ae_maxreal(ae_fabs(ndf, _state), (double)(1), _state)), _state);
        nd2f = (-dfr2+8*dfr-8*dfl+dfl2)/(12*h);
        seterrorflag(wereerrors, ae_fp_greater(ae_fabs(nd2f-d2f, _state),dtol*ae_maxreal(ae_fabs(nd2f, _state), (double)(1), _state)), _state);
        
        /*
         * Next point
         */
        v = v+h;
    }
    minnlcinequalityshiftfunction(1.0, &f0, &df, &d2f, _state);
    seterrorflag(wereerrors, ae_fp_greater(ae_fabs(f0, _state),1.0E-6), _state);
    seterrorflag(wereerrors, ae_fp_greater(ae_fabs(df+1, _state),1.0E-6), _state);
    
    /*
     * Test location reports
     */
    aulits = 50;
    rho = 200.0;
    tolx = 0.0005;
    n = 2;
    ae_vector_set_length(&x0, n, _state);
    ae_vector_set_length(&xlast, n, _state);
    x0.ptr.p_double[0] = 0.1;
    x0.ptr.p_double[1] = 0.2;
    xlast.ptr.p_double[0] = (double)(0);
    xlast.ptr.p_double[1] = (double)(0);
    minnlccreate(n, &x0, &state, _state);
    minnlcsetalgoaul(&state, rho, aulits, _state);
    minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
    minnlcsetnlc(&state, 0, 1, _state);
    minnlcsetxrep(&state, ae_true, _state);
    while(minnlciteration(&state, _state))
    {
        if( state.needfij )
        {
            state.fi.ptr.p_double[0] = ae_sqr(state.x.ptr.p_double[0]-1, _state)+ae_sqr(state.x.ptr.p_double[1]-1, _state);
            state.j.ptr.pp_double[0][0] = 2*(state.x.ptr.p_double[0]-1);
            state.j.ptr.pp_double[0][1] = 2*(state.x.ptr.p_double[1]-1);
            state.fi.ptr.p_double[1] = ae_sqr(state.x.ptr.p_double[0], _state)+ae_sqr(state.x.ptr.p_double[1], _state)-1;
            state.j.ptr.pp_double[1][0] = 2*state.x.ptr.p_double[0];
            state.j.ptr.pp_double[1][1] = 2*state.x.ptr.p_double[1];
            continue;
        }
        if( state.xupdated )
        {
            
            /*
             * Save last point
             */
            xlast.ptr.p_double[0] = state.x.ptr.p_double[0];
            xlast.ptr.p_double[1] = state.x.ptr.p_double[1];
            continue;
        }
        ae_assert(ae_false, "Assertion failed", _state);
    }
    minnlcresults(&state, &x1, &rep, _state);
    seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
    seterrorflag(wereerrors, rep.terminationtype<=0, _state);
    if( *wereerrors )
    {
        ae_frame_leave(_state);
        return;
    }
    seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[0]-xlast.ptr.p_double[0], _state),1.0E4*ae_machineepsilon), _state);
    seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[1]-xlast.ptr.p_double[1], _state),1.0E4*ae_machineepsilon), _state);
    
    /*
     * Test numerical differentiation
     */
    aulits = 50;
    rho = 200.0;
    tolx = 0.0001;
    diffstep = 0.001;
    n = 2;
    ae_vector_set_length(&x0, n, _state);
    x0.ptr.p_double[0] = 0.1;
    x0.ptr.p_double[1] = 0.2;
    minnlccreatef(n, &x0, diffstep, &state, _state);
    minnlcsetalgoaul(&state, rho, aulits, _state);
    minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
    minnlcsetnlc(&state, 0, 1, _state);
    while(minnlciteration(&state, _state))
    {
        if( state.needfi )
        {
            state.fi.ptr.p_double[0] = ae_sqr(state.x.ptr.p_double[0]-1, _state)+ae_sqr(state.x.ptr.p_double[1]-1, _state);
            state.fi.ptr.p_double[1] = ae_sqr(state.x.ptr.p_double[0], _state)+ae_sqr(state.x.ptr.p_double[1], _state)-1;
            continue;
        }
        ae_assert(ae_false, "Assertion failed", _state);
    }
    minnlcresults(&state, &x1, &rep, _state);
    seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
    seterrorflag(wereerrors, rep.terminationtype<=0, _state);
    if( *wereerrors )
    {
        ae_frame_leave(_state);
        return;
    }
    seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[0]-ae_sqrt((double)(2), _state)/2, _state),tolx), _state);
    seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[1]-ae_sqrt((double)(2), _state)/2, _state),tolx), _state);
    
    /*
     * Test gradient checking
     */
    aulits = 50;
    rho = 200.0;
    tolx = 0.0001;
    diffstep = 0.001;
    n = 2;
    ae_vector_set_length(&x0, n, _state);
    x0.ptr.p_double[0] = 0.1;
    x0.ptr.p_double[1] = 0.2;
    minnlccreate(n, &x0, &state, _state);
    minnlcsetalgoaul(&state, rho, aulits, _state);
    minnlcsetgradientcheck(&state, diffstep, _state);
    minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
    minnlcsetnlc(&state, 0, 1, _state);
    while(minnlciteration(&state, _state))
    {
        if( state.needfij )
        {
            state.fi.ptr.p_double[0] = ae_sqr(state.x.ptr.p_double[0]-1, _state)+ae_sqr(state.x.ptr.p_double[1]-1, _state);
            state.j.ptr.pp_double[0][0] = 2*(state.x.ptr.p_double[0]-1);
            state.j.ptr.pp_double[0][1] = 2*(state.x.ptr.p_double[1]-1);
            state.fi.ptr.p_double[1] = ae_sqr(state.x.ptr.p_double[0], _state)+ae_sqr(state.x.ptr.p_double[1], _state)-1;
            state.j.ptr.pp_double[1][0] = 2*state.x.ptr.p_double[0];
            state.j.ptr.pp_double[1][1] = (double)(0);
            continue;
        }
        ae_assert(ae_false, "Assertion failed", _state);
    }
    minnlcresults(&state, &x1, &rep, _state);
    seterrorflag(wereerrors, rep.terminationtype!=-7, _state);
    seterrorflag(wereerrors, rep.varidx!=1, _state);
    seterrorflag(wereerrors, rep.funcidx!=1, _state);
    
    /*
     * Check handling of general linear constraints: solve linearly 
     * constrained twice, first time with constraints posed as linear
     * ones, second time with constraints posed as nonlinear ones.
     *
     * Linear constraints are normalized because we know that optimizer
     * normalizes them internally.
     *
     * We perform small amount of inner iterations - just 3 steps.
     * Only one outer iteration is performed. Such small number of
     * iterations allows to reduce influence of round-off errors
     * and compare results returned by different control paths within
     * optimizer (control path for linear constraints and one for
     * nonlinear constraints).
     *
     * We test following kinds of preconditioners:
     * * "none"
     * * "exact low rank", restart frequency is 1
     * Inexact LBFGS-based preconditioner is not tested because its
     * behavior greatly depends on order of equations.
     */
    n = 30;
    k = 5;
    rho = 1.0E3;
    aulits = 1;
    maxits = 3;
    ae_vector_set_length(&x0, n, _state);
    ae_matrix_set_length(&c, k, n+1, _state);
    ae_vector_set_length(&ct, k, _state);
    for(prectype=0; prectype<=1; prectype++)
    {
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = hqrndnormal(&rs, _state);
        }
        for(i=0; i<=k-1; i++)
        {
            v = 0.0;
            for(j=0; j<=n-1; j++)
            {
                c.ptr.pp_double[i][j] = hqrndnormal(&rs, _state);
                v = v+ae_sqr(c.ptr.pp_double[i][j], _state);
            }
            v = 1/ae_sqrt(v, _state);
            ae_v_muld(&c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
            c.ptr.pp_double[i][n] = v;
            ct.ptr.p_int[i] = 0;
        }
        minnlccreate(n, &x0, &state, _state);
        minnlcsetalgoaul(&state, rho, aulits, _state);
        minnlcsetcond(&state, 0.0, 0.0, 0.0, maxits, _state);
        if( prectype==0 )
        {
            minnlcsetprecnone(&state, _state);
        }
        if( prectype==1 )
        {
            minnlcsetprecexactlowrank(&state, 1, _state);
        }
        minnlcsetlc(&state, &c, &ct, k, _state);
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+ae_sqr(state.x.ptr.p_double[i], _state);
                    state.j.ptr.pp_double[0][i] = 2*state.x.ptr.p_double[i];
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x1, &rep, _state);
        seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        minnlccreate(n, &x0, &state, _state);
        minnlcsetalgoaul(&state, rho, aulits, _state);
        minnlcsetcond(&state, 0.0, 0.0, 0.0, maxits, _state);
        if( prectype==0 )
        {
            minnlcsetprecnone(&state, _state);
        }
        if( prectype==1 )
        {
            minnlcsetprecexactlowrank(&state, 1, _state);
        }
        minnlcsetnlc(&state, k, 0, _state);
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+ae_sqr(state.x.ptr.p_double[i], _state);
                    state.j.ptr.pp_double[0][i] = 2*state.x.ptr.p_double[i];
                }
                for(i=0; i<=k-1; i++)
                {
                    v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    state.fi.ptr.p_double[1+i] = v-c.ptr.pp_double[i][n];
                    ae_v_move(&state.j.ptr.pp_double[1+i][0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x2, &rep, _state);
        seterrorflag(wereerrors, !isfinitevector(&x2, n, _state), _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(wereerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-x2.ptr.p_double[i], _state),1.0E-4), _state);
        }
    }
    
    /*
     * Test preconditioning:
     * * compare number of iterations required to solve problem with
     *   different preconditioners (LBFGS, exact, none)
     * * a set of trials is performed (100 trials)
     * * each trial is a solution of boundary/linearly constrained problem
     *   (linear constraints may be posed as nonlinear ones) with normalized
     *   constraint matrix. Normalization is essential for reproducibility
     *   of results .
     *
     * Outer loop checks handling of different types of constraints
     * (posed as linear or nonlinear ones)
     */
    n = 30;
    blocksize = 3;
    blockcnt = 3;
    rho = 1.0E3;
    aulits = 5;
    condv = 1.0E2;
    ae_vector_set_length(&x0, n, _state);
    ae_vector_set_length(&bndl, n, _state);
    ae_vector_set_length(&bndu, n, _state);
    ae_matrix_set_length(&c, blocksize*blockcnt, n+1, _state);
    ae_vector_set_length(&ct, blocksize*blockcnt, _state);
    for(ctype=0; ctype<=1; ctype++)
    {
        
        /*
         * First, initialize iteration counters
         */
        nlbfgs = 0;
        nexact = 0;
        nnone = 0;
        
        /*
         * Perform trials
         */
        for(trialidx=0; trialidx<=99; trialidx++)
        {
            
            /*
             * Generate:
             * * boundary constraints BndL/BndU and initial point X0
             * * block-diagonal matrix of linear constraints C such
             *   that X0 is feasible w.r.t. constraints given by C
             */
            for(i=0; i<=n-1; i++)
            {
                if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
                {
                    bndl.ptr.p_double[i] = (double)(0);
                    bndu.ptr.p_double[i] = _state->v_posinf;
                    x0.ptr.p_double[i] = hqrnduniformr(&rs, _state);
                }
                else
                {
                    bndl.ptr.p_double[i] = (double)(0);
                    bndu.ptr.p_double[i] = (double)(0);
                    x0.ptr.p_double[i] = (double)(0);
                }
            }
            for(i=0; i<=blocksize*blockcnt-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    c.ptr.pp_double[i][j] = 0.0;
                }
            }
            for(k=0; k<=blockcnt-1; k++)
            {
                rmatrixrndcond(blocksize, condv, &a, _state);
                for(i=0; i<=blocksize-1; i++)
                {
                    for(j=0; j<=blocksize-1; j++)
                    {
                        c.ptr.pp_double[k*blocksize+i][k*blocksize+j] = a.ptr.pp_double[i][j];
                    }
                }
            }
            for(i=0; i<=blocksize*blockcnt-1; i++)
            {
                v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                v = 1/ae_sqrt(v, _state);
                ae_v_muld(&c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
                v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                c.ptr.pp_double[i][n] = v;
                ct.ptr.p_int[i] = hqrnduniformi(&rs, 3, _state)-1;
            }
            
            /*
             * Test unpreconditioned iteration
             */
            minnlccreate(n, &x0, &state, _state);
            minnlcsetalgoaul(&state, rho, aulits, _state);
            minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
            if( ctype==0 )
            {
                minnlcsetlc(&state, &c, &ct, blocksize*blockcnt, _state);
            }
            else
            {
                minnlcsetnlc(&state, blocksize*blockcnt, 0, _state);
            }
            minnlcsetprecnone(&state, _state);
            while(minnlciteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+ae_sqr(state.x.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = 2*state.x.ptr.p_double[i];
                    }
                    if( ctype==1 )
                    {
                        for(i=0; i<=blocksize*blockcnt-1; i++)
                        {
                            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                            state.fi.ptr.p_double[1+i] = v-c.ptr.pp_double[i][n];
                            ae_v_move(&state.j.ptr.pp_double[1+i][0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                        }
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnlcresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( *wereerrors )
            {
                ae_frame_leave(_state);
                return;
            }
            nnone = nnone+rep.iterationscount;
            
            /*
             * Test LBFGS preconditioned iteration
             */
            minnlccreate(n, &x0, &state, _state);
            minnlcsetalgoaul(&state, rho, aulits, _state);
            minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
            if( ctype==0 )
            {
                minnlcsetlc(&state, &c, &ct, blocksize*blockcnt, _state);
            }
            else
            {
                minnlcsetnlc(&state, blocksize*blockcnt, 0, _state);
            }
            minnlcsetprecinexact(&state, _state);
            while(minnlciteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+ae_sqr(state.x.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = 2*state.x.ptr.p_double[i];
                    }
                    if( ctype==1 )
                    {
                        for(i=0; i<=blocksize*blockcnt-1; i++)
                        {
                            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                            state.fi.ptr.p_double[1+i] = v-c.ptr.pp_double[i][n];
                            ae_v_move(&state.j.ptr.pp_double[1+i][0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                        }
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnlcresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( *wereerrors )
            {
                ae_frame_leave(_state);
                return;
            }
            nlbfgs = nlbfgs+rep.iterationscount;
            
            /*
             * Test exact preconditioner
             */
            minnlccreate(n, &x0, &state, _state);
            minnlcsetalgoaul(&state, rho, aulits, _state);
            minnlcsetcond(&state, 0.0, 0.0, 1.0E-9, 0, _state);
            if( ctype==0 )
            {
                minnlcsetlc(&state, &c, &ct, blocksize*blockcnt, _state);
            }
            else
            {
                minnlcsetnlc(&state, blocksize*blockcnt, 0, _state);
            }
            minnlcsetprecexactlowrank(&state, 3, _state);
            while(minnlciteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+ae_sqr(state.x.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = 2*state.x.ptr.p_double[i];
                    }
                    if( ctype==1 )
                    {
                        for(i=0; i<=blocksize*blockcnt-1; i++)
                        {
                            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                            state.fi.ptr.p_double[1+i] = v-c.ptr.pp_double[i][n];
                            ae_v_move(&state.j.ptr.pp_double[1+i][0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                        }
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnlcresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, !isfinitevector(&x1, n, _state), _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( *wereerrors )
            {
                ae_frame_leave(_state);
                return;
            }
            nexact = nexact+rep.iterationscount;
        }
        
        /*
         * Compare.
         *
         * Preconditioners must be significantly different,
         * with exact being best one, inexact being second,
         * "none" being worst option.
         */
        seterrorflag(wereerrors, !ae_fp_less((double)(nexact),0.9*nlbfgs), _state);
        seterrorflag(wereerrors, !ae_fp_less((double)(nlbfgs),0.9*nnone), _state);
    }
    
    /*
     * Test integrity checks for NAN/INF:
     * * algorithm solves optimization problem, which is normal for some time (quadratic)
     * * after 5-th step we choose random component of gradient and consistently spoil
     *   it by NAN or INF.
     * * we check that correct termination code is returned (-8)
     */
    n = 100;
    for(pass=1; pass<=10; pass++)
    {
        spoiliteration = 5;
        stopiteration = 8;
        if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
        {
            
            /*
             * Gradient can be spoiled by +INF, -INF, NAN
             */
            spoilvar = hqrnduniformi(&rs, n, _state);
            i = hqrnduniformi(&rs, 3, _state);
            spoilval = _state->v_nan;
            if( i==0 )
            {
                spoilval = _state->v_neginf;
            }
            if( i==1 )
            {
                spoilval = _state->v_posinf;
            }
        }
        else
        {
            
            /*
             * Function value can be spoiled only by NAN
             * (+INF can be recognized as legitimate value during optimization)
             */
            spoilvar = -1;
            spoilval = _state->v_nan;
        }
        spdmatrixrndcond(n, 1.0E5, &fulla, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&x0, n, _state);
        for(i=0; i<=n-1; i++)
        {
            b.ptr.p_double[i] = hqrndnormal(&rs, _state);
            x0.ptr.p_double[i] = hqrndnormal(&rs, _state);
        }
        minnlccreate(n, &x0, &state, _state);
        minnlcsetcond(&state, 0.0, 0.0, 0.0, stopiteration, _state);
        minnlcsetxrep(&state, ae_true, _state);
        k = -1;
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                    state.j.ptr.pp_double[0][i] = b.ptr.p_double[i];
                    for(j=0; j<=n-1; j++)
                    {
                        state.fi.ptr.p_double[0] = state.f+0.5*state.x.ptr.p_double[i]*fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                        state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+fulla.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
                if( k>=spoiliteration )
                {
                    if( spoilvar<0 )
                    {
                        state.fi.ptr.p_double[0] = spoilval;
                    }
                    else
                    {
                        state.j.ptr.pp_double[0][spoilvar] = spoilval;
                    }
                }
                continue;
            }
            if( state.xupdated )
            {
                inc(&k, _state);
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x1, &rep, _state);
        seterrorflag(wereerrors, rep.terminationtype!=-8, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function performs tests for fixed bugs

On failure sets error flag.
*************************************************************************/
static void testminnlcunit_testbugs(ae_bool* wereerrors, ae_state *_state)
{
    ae_frame _frame_block;
    hqrndstate rs;
    ae_int_t n;
    ae_int_t aulits;
    ae_int_t maxits;
    double rho;
    ae_int_t ckind;
    minnlcstate state;
    minnlcreport rep;
    ae_vector x0;
    ae_vector x1;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector ct;
    ae_matrix c;

    ae_frame_make(_state, &_frame_block);
    _hqrndstate_init(&rs, _state);
    _minnlcstate_init(&state, _state);
    _minnlcreport_init(&rep, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Bug description (fixed): sometimes on non-convex problems, when
     * Lagrange coefficient for inequality constraint becomes small,
     * algorithm performs VERY deep step into infeasible area (step is 1E50),
     * which de-stabilizes it and prevents from converging back to feasible area.
     *
     * Very rare situation, but must be fixed with additional "convexifying" term.
     * This test reproduces situation with convexified term turned off, then
     * checks that introduction of term solves issue.
     *
     * We perform three kinds of tests:
     * * with box inequality constraint
     * * with linear inequality constraint
     * * with nonlinear inequality constraint
     *
     * In all three cases we:
     * * first time solve non-convex problem with artificially moved stabilizing
     *   point and decreased initial value of Lagrange multiplier.
     * * second time we solve problem with good stabilizing point, but zero Lagrange multiplier
     * * last time solve same problem, but with default settings
     */
    aulits = 1;
    maxits = 1;
    rho = 100.0;
    n = 1;
    ae_vector_set_length(&x0, n, _state);
    x0.ptr.p_double[0] = 0.0;
    ae_vector_set_length(&bndl, n, _state);
    ae_vector_set_length(&bndu, n, _state);
    bndl.ptr.p_double[0] = 0.0;
    bndu.ptr.p_double[0] = _state->v_posinf;
    ae_matrix_set_length(&c, 1, 2, _state);
    ae_vector_set_length(&ct, 1, _state);
    c.ptr.pp_double[0][0] = 1.0;
    c.ptr.pp_double[0][1] = 0.0;
    ct.ptr.p_int[0] = 1;
    for(ckind=0; ckind<=2; ckind++)
    {
        minnlccreate(n, &x0, &state, _state);
        state.stabilizingpoint = -1.0E300;
        state.initialinequalitymultiplier = 1.0E-12;
        minnlcsetalgoaul(&state, rho, aulits, _state);
        minnlcsetcond(&state, 0.0, 0.0, 0.0, maxits, _state);
        if( ckind==0 )
        {
            minnlcsetbc(&state, &bndl, &bndu, _state);
        }
        if( ckind==1 )
        {
            minnlcsetlc(&state, &c, &ct, 1, _state);
        }
        if( ckind==2 )
        {
            minnlcsetnlc(&state, 0, 1, _state);
        }
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = state.x.ptr.p_double[0]-ae_sqr(state.x.ptr.p_double[0], _state);
                state.j.ptr.pp_double[0][0] = 1-2*state.x.ptr.p_double[0];
                if( ckind==2 )
                {
                    state.fi.ptr.p_double[1] = -state.x.ptr.p_double[0];
                    state.j.ptr.pp_double[1][0] = (double)(-1);
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x1, &rep, _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        seterrorflag(wereerrors, ae_fp_greater(x1.ptr.p_double[0],-1.0E6), _state);
        minnlccreate(n, &x0, &state, _state);
        state.stabilizingpoint = -1.0E2;
        state.initialinequalitymultiplier = 1.0E-12;
        minnlcsetalgoaul(&state, rho, aulits, _state);
        minnlcsetcond(&state, 0.0, 0.0, 0.0, maxits, _state);
        if( ckind==0 )
        {
            minnlcsetbc(&state, &bndl, &bndu, _state);
        }
        if( ckind==1 )
        {
            minnlcsetlc(&state, &c, &ct, 1, _state);
        }
        if( ckind==2 )
        {
            minnlcsetnlc(&state, 0, 1, _state);
        }
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = state.x.ptr.p_double[0]-ae_sqr(state.x.ptr.p_double[0], _state);
                state.j.ptr.pp_double[0][0] = 1-2*state.x.ptr.p_double[0];
                if( ckind==2 )
                {
                    state.fi.ptr.p_double[1] = -state.x.ptr.p_double[0];
                    state.j.ptr.pp_double[1][0] = (double)(-1);
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x1, &rep, _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        seterrorflag(wereerrors, ae_fp_less(x1.ptr.p_double[0],3*state.stabilizingpoint), _state);
        minnlccreate(n, &x0, &state, _state);
        minnlcsetalgoaul(&state, rho, aulits, _state);
        minnlcsetcond(&state, 0.0, 0.0, 0.0, maxits, _state);
        if( ckind==0 )
        {
            minnlcsetbc(&state, &bndl, &bndu, _state);
        }
        if( ckind==1 )
        {
            minnlcsetlc(&state, &c, &ct, 1, _state);
        }
        if( ckind==2 )
        {
            minnlcsetnlc(&state, 0, 1, _state);
        }
        while(minnlciteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = state.x.ptr.p_double[0]-ae_sqr(state.x.ptr.p_double[0], _state);
                state.j.ptr.pp_double[0][0] = 1-2*state.x.ptr.p_double[0];
                if( ckind==2 )
                {
                    state.fi.ptr.p_double[1] = -state.x.ptr.p_double[0];
                    state.j.ptr.pp_double[1][0] = (double)(-1);
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnlcresults(&state, &x1, &rep, _state);
        seterrorflag(wereerrors, rep.terminationtype<=0, _state);
        if( *wereerrors )
        {
            ae_frame_leave(_state);
            return;
        }
        seterrorflag(wereerrors, ae_fp_less(x1.ptr.p_double[0],3*state.stabilizingpoint), _state);
    }
    ae_frame_leave(_state);
}


/*$ End $*/
