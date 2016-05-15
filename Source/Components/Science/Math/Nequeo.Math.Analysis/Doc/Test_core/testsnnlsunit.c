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
#include "testsnnlsunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testsnnls(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool test0errors;
    ae_bool test1errors;
    ae_bool test2errors;
    ae_bool testnewtonerrors;
    ae_bool waserrors;
    double eps;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    ae_int_t ns;
    ae_int_t nd;
    ae_int_t nr;
    ae_matrix densea;
    ae_matrix effectivea;
    ae_vector isconstrained;
    ae_vector g;
    ae_vector b;
    ae_vector x;
    ae_vector xs;
    snnlssolver s;
    hqrndstate rs;
    double rho;
    double xtol;
    ae_int_t nmax;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&densea, 0, 0, DT_REAL, _state);
    ae_matrix_init(&effectivea, 0, 0, DT_REAL, _state);
    ae_vector_init(&isconstrained, 0, DT_BOOL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&xs, 0, DT_REAL, _state);
    _snnlssolver_init(&s, _state);
    _hqrndstate_init(&rs, _state);

    test0errors = ae_false;
    test1errors = ae_false;
    test2errors = ae_false;
    testnewtonerrors = ae_false;
    waserrors = ae_false;
    hqrndrandomize(&rs, _state);
    nmax = 10;
    
    /*
     * Test 2 (comes first because it is very basic):
     * * NS=0
     * * ND in [1,NMAX]
     * * NR=ND
     * * DenseA is diagonal with positive entries
     * * B is random
     * * random constraints
     * Exact solution is known and can be tested
     */
    eps = 1.0E-12;
    for(nd=1; nd<=nmax; nd++)
    {
        
        /*
         * Generate problem
         */
        ns = 0;
        nr = nd;
        ae_matrix_set_length(&densea, nd, nd, _state);
        ae_vector_set_length(&b, nd, _state);
        ae_vector_set_length(&isconstrained, nd, _state);
        for(i=0; i<=nd-1; i++)
        {
            for(j=0; j<=nd-1; j++)
            {
                densea.ptr.pp_double[i][j] = (double)(0);
            }
            densea.ptr.pp_double[i][i] = (double)(1+hqrnduniformi(&rs, 2, _state));
            b.ptr.p_double[i] = (double)((1+hqrnduniformi(&rs, 2, _state))*(2*hqrnduniformi(&rs, 2, _state)-1));
            isconstrained.ptr.p_bool[i] = ae_fp_greater(hqrnduniformr(&rs, _state),0.5);
        }
        
        /*
         * Solve with SNNLS solver
         */
        snnlsinit(0, 0, 0, &s, _state);
        snnlssetproblem(&s, &densea, &b, 0, nd, nd, _state);
        for(i=0; i<=nd-1; i++)
        {
            if( !isconstrained.ptr.p_bool[i] )
            {
                snnlsdropnnc(&s, i, _state);
            }
        }
        snnlssolve(&s, &x, _state);
        
        /*
         * Check
         */
        for(i=0; i<=nd-1; i++)
        {
            if( isconstrained.ptr.p_bool[i] )
            {
                seterrorflag(&test2errors, ae_fp_greater(ae_fabs(x.ptr.p_double[i]-ae_maxreal(b.ptr.p_double[i]/densea.ptr.pp_double[i][i], 0.0, _state), _state),eps), _state);
                seterrorflag(&test2errors, ae_fp_less(x.ptr.p_double[i],0.0), _state);
            }
            else
            {
                seterrorflag(&test2errors, ae_fp_greater(ae_fabs(x.ptr.p_double[i]-b.ptr.p_double[i]/densea.ptr.pp_double[i][i], _state),eps), _state);
            }
        }
    }
    
    /*
     * Test 0:
     * * NS in [0,NMAX]
     * * ND in [0,NMAX]
     * * NR in [NS,NS+ND+NMAX]
     * * NS+ND>0, NR>0
     * * about 50% of variables are constrained
     * * we check that constrained gradient is small at the solution
     */
    eps = 1.0E-5;
    for(ns=0; ns<=nmax; ns++)
    {
        for(nd=0; nd<=nmax; nd++)
        {
            for(nr=ns; nr<=ns+nd+nmax; nr++)
            {
                
                /*
                 * Skip NS+ND=0, NR=0
                 */
                if( ns+nd==0 )
                {
                    continue;
                }
                if( nr==0 )
                {
                    continue;
                }
                
                /*
                 * Generate problem:
                 * * DenseA, array[NR,ND]
                 * * EffectiveA, array[NR,NS+ND]
                 * * B, array[NR]
                 * * IsConstrained, array[NS+ND]
                 */
                if( nd>0 )
                {
                    ae_matrix_set_length(&densea, nr, nd, _state);
                    for(i=0; i<=nr-1; i++)
                    {
                        for(j=0; j<=nd-1; j++)
                        {
                            densea.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                }
                ae_matrix_set_length(&effectivea, nr, ns+nd, _state);
                for(i=0; i<=nr-1; i++)
                {
                    for(j=0; j<=ns+nd-1; j++)
                    {
                        effectivea.ptr.pp_double[i][j] = 0.0;
                    }
                }
                for(i=0; i<=ns-1; i++)
                {
                    effectivea.ptr.pp_double[i][i] = 1.0;
                }
                for(i=0; i<=nr-1; i++)
                {
                    for(j=0; j<=nd-1; j++)
                    {
                        effectivea.ptr.pp_double[i][ns+j] = densea.ptr.pp_double[i][j];
                    }
                }
                ae_vector_set_length(&b, nr, _state);
                for(i=0; i<=nr-1; i++)
                {
                    b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                ae_vector_set_length(&isconstrained, ns+nd, _state);
                for(i=0; i<=ns+nd-1; i++)
                {
                    isconstrained.ptr.p_bool[i] = ae_fp_greater(ae_randomreal(_state),0.5);
                }
                
                /*
                 * Solve with SNNLS solver
                 */
                snnlsinit(0, 0, 0, &s, _state);
                snnlssetproblem(&s, &densea, &b, ns, nd, nr, _state);
                for(i=0; i<=ns+nd-1; i++)
                {
                    if( !isconstrained.ptr.p_bool[i] )
                    {
                        snnlsdropnnc(&s, i, _state);
                    }
                }
                snnlssolve(&s, &x, _state);
                
                /*
                 * Check non-negativity
                 */
                for(i=0; i<=ns+nd-1; i++)
                {
                    seterrorflag(&test0errors, isconstrained.ptr.p_bool[i]&&ae_fp_less(x.ptr.p_double[i],(double)(0)), _state);
                }
                
                /*
                 * Calculate gradient A'*A*x-b'*A.
                 * Check projected gradient (each component must be less than Eps).
                 */
                ae_vector_set_length(&g, ns+nd, _state);
                for(i=0; i<=ns+nd-1; i++)
                {
                    v = ae_v_dotproduct(&b.ptr.p_double[0], 1, &effectivea.ptr.pp_double[0][i], effectivea.stride, ae_v_len(0,nr-1));
                    g.ptr.p_double[i] = -v;
                }
                for(i=0; i<=nr-1; i++)
                {
                    v = ae_v_dotproduct(&effectivea.ptr.pp_double[i][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,ns+nd-1));
                    ae_v_addd(&g.ptr.p_double[0], 1, &effectivea.ptr.pp_double[i][0], 1, ae_v_len(0,ns+nd-1), v);
                }
                for(i=0; i<=ns+nd-1; i++)
                {
                    if( !isconstrained.ptr.p_bool[i]||ae_fp_greater(x.ptr.p_double[i],(double)(0)) )
                    {
                        seterrorflag(&test0errors, ae_fp_greater(ae_fabs(g.ptr.p_double[i], _state),eps), _state);
                    }
                    else
                    {
                        seterrorflag(&test0errors, ae_fp_less(g.ptr.p_double[i],-eps), _state);
                    }
                }
            }
        }
    }
    
    /*
     * Test 1: ability of the solver to take very short steps.
     *
     * We solve problem similar to one solver in test 0, but with
     * progressively decreased magnitude of variables. We generate
     * problem with already-known solution and compare results against it.
     */
    xtol = 1.0E6*ae_machineepsilon;
    for(ns=0; ns<=nmax; ns++)
    {
        for(nd=0; nd<=nmax; nd++)
        {
            for(nr=ns; nr<=ns+nd+nmax; nr++)
            {
                for(k=0; k<=20; k++)
                {
                    
                    /*
                     * Skip NS+ND=0, NR=0
                     *
                     * Skip degenerate problems (NR<NS+ND) - important for this particular test.
                     */
                    if( ns+nd==0 )
                    {
                        continue;
                    }
                    if( nr==0 )
                    {
                        continue;
                    }
                    if( nr<ns+nd )
                    {
                        continue;
                    }
                    
                    /*
                     * Generate problem:
                     * * DenseA, array[NR,ND]
                     * * EffectiveA, array[NR,NS+ND]
                     * * B, array[NR]
                     * * IsConstrained, array[NS+ND]
                     */
                    rho = ae_pow((double)(10), (double)(-k), _state);
                    if( nd>0 )
                    {
                        ae_matrix_set_length(&densea, nr, nd, _state);
                        for(i=0; i<=nr-1; i++)
                        {
                            for(j=0; j<=nd-1; j++)
                            {
                                densea.ptr.pp_double[i][j] = 2*hqrnduniformr(&rs, _state)-1;
                            }
                        }
                    }
                    ae_matrix_set_length(&effectivea, nr, ns+nd, _state);
                    for(i=0; i<=nr-1; i++)
                    {
                        for(j=0; j<=ns+nd-1; j++)
                        {
                            effectivea.ptr.pp_double[i][j] = 0.0;
                        }
                    }
                    for(i=0; i<=ns-1; i++)
                    {
                        effectivea.ptr.pp_double[i][i] = 1.0;
                    }
                    for(i=0; i<=nr-1; i++)
                    {
                        for(j=0; j<=nd-1; j++)
                        {
                            effectivea.ptr.pp_double[i][ns+j] = densea.ptr.pp_double[i][j];
                        }
                    }
                    ae_vector_set_length(&xs, ns+nd, _state);
                    ae_vector_set_length(&isconstrained, ns+nd, _state);
                    for(i=0; i<=ns+nd-1; i++)
                    {
                        xs.ptr.p_double[i] = rho*(hqrnduniformr(&rs, _state)-0.5);
                        isconstrained.ptr.p_bool[i] = ae_fp_greater(xs.ptr.p_double[i],0.0);
                    }
                    ae_vector_set_length(&b, nr, _state);
                    for(i=0; i<=nr-1; i++)
                    {
                        v = 0.0;
                        for(j=0; j<=ns+nd-1; j++)
                        {
                            v = v+effectivea.ptr.pp_double[i][j]*xs.ptr.p_double[j];
                        }
                        b.ptr.p_double[i] = v;
                    }
                    
                    /*
                     * Solve with SNNLS solver
                     */
                    snnlsinit(0, 0, 0, &s, _state);
                    snnlssetproblem(&s, &densea, &b, ns, nd, nr, _state);
                    for(i=0; i<=ns+nd-1; i++)
                    {
                        if( !isconstrained.ptr.p_bool[i] )
                        {
                            snnlsdropnnc(&s, i, _state);
                        }
                    }
                    snnlssolve(&s, &x, _state);
                    
                    /*
                     * Check non-negativity
                     */
                    for(i=0; i<=ns+nd-1; i++)
                    {
                        seterrorflag(&test1errors, isconstrained.ptr.p_bool[i]&&ae_fp_less(x.ptr.p_double[i],(double)(0)), _state);
                    }
                    
                    /*
                     * Compare with true solution
                     */
                    for(i=0; i<=ns+nd-1; i++)
                    {
                        seterrorflag(&test1errors, ae_fp_greater(ae_fabs(xs.ptr.p_double[i]-x.ptr.p_double[i], _state),rho*xtol), _state);
                    }
                }
            }
        }
    }
    
    /*
     * Test for Newton phase:
     * * NS in [0,NMAX]
     * * ND in [0,NMAX]
     * * NR in [NS,NS+ND+NMAX]
     * * NS+ND>0, NR>0
     * * all variables are unconstrained
     * * S.DebugMaxNewton is set to 1, S.RefinementIts is set to 1,
     *   i.e. algorithm is terminated after one Newton iteration, and no
     *   iterative refinement is used.
     * * we test that gradient is small at solution, i.e. one Newton iteration
     *   on unconstrained problem is enough to find solution. In case of buggy
     *   Newton solver one iteration won't move us to the solution - it may
     *   decrease function value, but won't find exact solution.
     *
     * This test is intended to catch subtle bugs in the Newton solver which
     * do NOT prevent algorithm from converging to the solution, but slow it
     * down (convergence becomes linear or even slower).
     */
    eps = 1.0E-4;
    for(ns=0; ns<=nmax; ns++)
    {
        for(nd=0; nd<=nmax; nd++)
        {
            for(nr=ns; nr<=ns+nd+nmax; nr++)
            {
                
                /*
                 * Skip NS+ND=0, NR=0
                 */
                if( ns+nd==0 )
                {
                    continue;
                }
                if( nr==0 )
                {
                    continue;
                }
                
                /*
                 * Generate problem:
                 * * DenseA, array[NR,ND]
                 * * EffectiveA, array[NR,NS+ND]
                 * * B, array[NR]
                 * * IsConstrained, array[NS+ND]
                 */
                if( nd>0 )
                {
                    ae_matrix_set_length(&densea, nr, nd, _state);
                    for(i=0; i<=nr-1; i++)
                    {
                        for(j=0; j<=nd-1; j++)
                        {
                            densea.ptr.pp_double[i][j] = hqrndnormal(&rs, _state);
                        }
                    }
                }
                ae_matrix_set_length(&effectivea, nr, ns+nd, _state);
                for(i=0; i<=nr-1; i++)
                {
                    for(j=0; j<=ns+nd-1; j++)
                    {
                        effectivea.ptr.pp_double[i][j] = 0.0;
                    }
                }
                for(i=0; i<=ns-1; i++)
                {
                    effectivea.ptr.pp_double[i][i] = 1.0;
                }
                for(i=0; i<=nr-1; i++)
                {
                    for(j=0; j<=nd-1; j++)
                    {
                        effectivea.ptr.pp_double[i][ns+j] = densea.ptr.pp_double[i][j];
                    }
                }
                ae_vector_set_length(&b, nr, _state);
                for(i=0; i<=nr-1; i++)
                {
                    b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                }
                
                /*
                 * Solve with SNNLS solver
                 */
                snnlsinit(0, 0, 0, &s, _state);
                snnlssetproblem(&s, &densea, &b, ns, nd, nr, _state);
                for(i=0; i<=ns+nd-1; i++)
                {
                    snnlsdropnnc(&s, i, _state);
                }
                s.debugmaxinnerits = 1;
                snnlssolve(&s, &x, _state);
                
                /*
                 * Calculate gradient A'*A*x-b'*A.
                 * Check projected gradient (each component must be less than Eps).
                 */
                ae_vector_set_length(&g, ns+nd, _state);
                for(i=0; i<=ns+nd-1; i++)
                {
                    v = ae_v_dotproduct(&b.ptr.p_double[0], 1, &effectivea.ptr.pp_double[0][i], effectivea.stride, ae_v_len(0,nr-1));
                    g.ptr.p_double[i] = -v;
                }
                for(i=0; i<=nr-1; i++)
                {
                    v = ae_v_dotproduct(&effectivea.ptr.pp_double[i][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,ns+nd-1));
                    ae_v_addd(&g.ptr.p_double[0], 1, &effectivea.ptr.pp_double[i][0], 1, ae_v_len(0,ns+nd-1), v);
                }
                for(i=0; i<=ns+nd-1; i++)
                {
                    seterrorflag(&testnewtonerrors, ae_fp_greater(ae_fabs(g.ptr.p_double[i], _state),eps), _state);
                }
            }
        }
    }
    
    /*
     * report
     */
    waserrors = ((test0errors||test1errors)||test2errors)||testnewtonerrors;
    if( !silent )
    {
        printf("TESTING SPECIAL NNLS SOLVER\n");
        printf("TEST 0:                                   ");
        if( test0errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("TEST 1:                                   ");
        if( test1errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("TEST 2:                                   ");
        if( test2errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("NEWTON PHASE:                             ");
        if( testnewtonerrors )
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
ae_bool _pexec_testsnnls(ae_bool silent, ae_state *_state)
{
    return testsnnls(silent, _state);
}


/*$ End $*/
