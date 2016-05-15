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
#include "testcqmodelsunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testcqmodels(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool eval0errors;
    ae_bool eval1errors;
    ae_bool eval2errors;
    ae_bool newton0errors;
    ae_bool newton1errors;
    ae_bool newton2errors;
    ae_bool waserrors;
    convexquadraticmodel s;
    ae_int_t nkind;
    ae_int_t kmax;
    ae_int_t n;
    ae_int_t k;
    ae_int_t i;
    ae_int_t pass;
    ae_int_t j;
    double alpha;
    double theta;
    double tau;
    double v;
    double v2;
    double h;
    double f0;
    double mkind;
    double xtadx2;
    double noise;
    ae_matrix a;
    ae_matrix q;
    ae_vector b;
    ae_vector r;
    ae_vector x;
    ae_vector x0;
    ae_vector xc;
    ae_vector d;
    ae_vector ge;
    ae_vector gt;
    ae_vector tmp0;
    ae_vector adx;
    ae_vector adxe;
    ae_vector activeset;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _convexquadraticmodel_init(&s, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&q, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&r, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&ge, 0, DT_REAL, _state);
    ae_vector_init(&gt, 0, DT_REAL, _state);
    ae_vector_init(&tmp0, 0, DT_REAL, _state);
    ae_vector_init(&adx, 0, DT_REAL, _state);
    ae_vector_init(&adxe, 0, DT_REAL, _state);
    ae_vector_init(&activeset, 0, DT_BOOL, _state);

    waserrors = ae_false;
    
    /*
     * Eval0 test: unconstrained model evaluation
     */
    eval0errors = ae_false;
    for(n=1; n<=5; n++)
    {
        for(k=0; k<=2*n; k++)
        {
            
            /*
             * Allocate place
             */
            ae_matrix_set_length(&a, n, n, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&d, n, _state);
            ae_vector_set_length(&ge, n, _state);
            ae_vector_set_length(&gt, n, _state);
            if( k>0 )
            {
                ae_matrix_set_length(&q, k, n, _state);
                ae_vector_set_length(&r, k, _state);
            }
            
            /*
             * Generate problem
             */
            alpha = ae_randomreal(_state)+1.0;
            theta = ae_randomreal(_state)+1.0;
            tau = ae_randomreal(_state)*ae_randominteger(2, _state);
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_double[i][i] = 10*(1+ae_randomreal(_state));
                b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                d.ptr.p_double[i] = ae_randomreal(_state)+1;
                for(j=i+1; j<=n-1; j++)
                {
                    v = 0.1*ae_randomreal(_state)-0.05;
                    a.ptr.pp_double[i][j] = v;
                    a.ptr.pp_double[j][i] = v;
                }
                for(j=0; j<=k-1; j++)
                {
                    q.ptr.pp_double[j][i] = 2*ae_randomreal(_state)-1;
                }
            }
            for(i=0; i<=k-1; i++)
            {
                r.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            
            /*
             * Build model
             */
            cqminit(n, &s, _state);
            cqmseta(&s, &a, ae_fp_greater(ae_randomreal(_state),0.5), alpha, _state);
            cqmsetb(&s, &b, _state);
            cqmsetq(&s, &q, &r, k, theta, _state);
            cqmsetd(&s, &d, tau, _state);
            
            /*
             * Evaluate and compare:
             * * X          -   random point
             * * GE         -   "exact" gradient
             * * XTADX2     -   x'*(alpha*A+tau*D)*x/2
             * * ADXE       -   (alpha*A+tau*D)*x
             * * V          -   model value at X
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                ge.ptr.p_double[i] = 0.0;
            }
            v = 0.0;
            xtadx2 = 0.0;
            ae_vector_set_length(&adxe, n, _state);
            for(i=0; i<=n-1; i++)
            {
                adxe.ptr.p_double[i] = (double)(0);
            }
            for(i=0; i<=n-1; i++)
            {
                v = v+x.ptr.p_double[i]*b.ptr.p_double[i];
                ge.ptr.p_double[i] = ge.ptr.p_double[i]+b.ptr.p_double[i];
                v = v+0.5*ae_sqr(x.ptr.p_double[i], _state)*tau*d.ptr.p_double[i];
                ge.ptr.p_double[i] = ge.ptr.p_double[i]+x.ptr.p_double[i]*tau*d.ptr.p_double[i];
                adxe.ptr.p_double[i] = adxe.ptr.p_double[i]+x.ptr.p_double[i]*tau*d.ptr.p_double[i];
                xtadx2 = xtadx2+0.5*ae_sqr(x.ptr.p_double[i], _state)*tau*d.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    v = v+0.5*alpha*x.ptr.p_double[i]*a.ptr.pp_double[i][j]*x.ptr.p_double[j];
                    ge.ptr.p_double[i] = ge.ptr.p_double[i]+alpha*a.ptr.pp_double[i][j]*x.ptr.p_double[j];
                    adxe.ptr.p_double[i] = adxe.ptr.p_double[i]+alpha*a.ptr.pp_double[i][j]*x.ptr.p_double[j];
                    xtadx2 = xtadx2+0.5*alpha*x.ptr.p_double[i]*a.ptr.pp_double[i][j]*x.ptr.p_double[j];
                }
            }
            for(i=0; i<=k-1; i++)
            {
                v2 = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                v = v+0.5*theta*ae_sqr(v2-r.ptr.p_double[i], _state);
                for(j=0; j<=n-1; j++)
                {
                    ge.ptr.p_double[j] = ge.ptr.p_double[j]+theta*(v2-r.ptr.p_double[i])*q.ptr.pp_double[i][j];
                }
            }
            v2 = cqmeval(&s, &x, _state);
            eval0errors = eval0errors||ae_fp_greater(ae_fabs(v-v2, _state),10000*ae_machineepsilon);
            cqmevalx(&s, &x, &v2, &noise, _state);
            eval0errors = eval0errors||ae_fp_greater(ae_fabs(v-v2, _state),10000*ae_machineepsilon);
            eval0errors = (eval0errors||ae_fp_less(noise,(double)(0)))||ae_fp_greater(noise,10000*ae_machineepsilon);
            v2 = cqmxtadx2(&s, &x, _state);
            eval0errors = eval0errors||ae_fp_greater(ae_fabs(xtadx2-v2, _state),10000*ae_machineepsilon);
            cqmgradunconstrained(&s, &x, &gt, _state);
            for(i=0; i<=n-1; i++)
            {
                eval0errors = eval0errors||ae_fp_greater(ae_fabs(ge.ptr.p_double[i]-gt.ptr.p_double[i], _state),10000*ae_machineepsilon);
            }
            cqmadx(&s, &x, &adx, _state);
            for(i=0; i<=n-1; i++)
            {
                eval0errors = eval0errors||ae_fp_greater(ae_fabs(adx.ptr.p_double[i]-adxe.ptr.p_double[i], _state),10000*ae_machineepsilon);
            }
        }
    }
    waserrors = waserrors||eval0errors;
    
    /*
     * Eval1 test: constrained model evaluation
     */
    eval1errors = ae_false;
    for(n=1; n<=5; n++)
    {
        for(k=0; k<=2*n; k++)
        {
            
            /*
             * Allocate place
             */
            ae_matrix_set_length(&a, n, n, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&xc, n, _state);
            ae_vector_set_length(&activeset, n, _state);
            if( k>0 )
            {
                ae_matrix_set_length(&q, k, n, _state);
                ae_vector_set_length(&r, k, _state);
            }
            
            /*
             * Generate problem
             */
            alpha = ae_randomreal(_state)+1.0;
            theta = ae_randomreal(_state)+1.0;
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_double[i][i] = 10*(1+ae_randomreal(_state));
                b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                activeset.ptr.p_bool[i] = ae_fp_greater(ae_randomreal(_state),0.5);
                for(j=i+1; j<=n-1; j++)
                {
                    v = 0.1*ae_randomreal(_state)-0.05;
                    a.ptr.pp_double[i][j] = v;
                    a.ptr.pp_double[j][i] = v;
                }
                for(j=0; j<=k-1; j++)
                {
                    q.ptr.pp_double[j][i] = 2*ae_randomreal(_state)-1;
                }
            }
            for(i=0; i<=k-1; i++)
            {
                r.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            
            /*
             * Build model, evaluate at random point X, compare
             */
            cqminit(n, &s, _state);
            cqmseta(&s, &a, ae_fp_greater(ae_randomreal(_state),0.5), alpha, _state);
            cqmsetb(&s, &b, _state);
            cqmsetq(&s, &q, &r, k, theta, _state);
            cqmsetactiveset(&s, &xc, &activeset, _state);
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                if( !activeset.ptr.p_bool[i] )
                {
                    xc.ptr.p_double[i] = x.ptr.p_double[i];
                }
            }
            v = 0.0;
            for(i=0; i<=n-1; i++)
            {
                v = v+xc.ptr.p_double[i]*b.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    v = v+0.5*alpha*xc.ptr.p_double[i]*a.ptr.pp_double[i][j]*xc.ptr.p_double[j];
                }
            }
            for(i=0; i<=k-1; i++)
            {
                v2 = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
                v = v+0.5*theta*ae_sqr(v2-r.ptr.p_double[i], _state);
            }
            eval1errors = eval1errors||ae_fp_greater(ae_fabs(v-cqmeval(&s, &xc, _state), _state),10000*ae_machineepsilon);
            eval1errors = eval1errors||ae_fp_greater(ae_fabs(v-cqmdebugconstrainedevalt(&s, &x, _state), _state),10000*ae_machineepsilon);
            eval1errors = eval1errors||ae_fp_greater(ae_fabs(v-cqmdebugconstrainedevale(&s, &x, _state), _state),10000*ae_machineepsilon);
        }
    }
    waserrors = waserrors||eval1errors;
    
    /*
     * Eval2 test: we generate empty problem and apply sequence of random transformations,
     * re-evaluating and re-checking model after each modification.
     *
     * The purpose of such test is to ensure that our caching strategy works correctly.
     */
    eval2errors = ae_false;
    for(n=1; n<=5; n++)
    {
        kmax = 2*n;
        ae_matrix_set_length(&a, n, n, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&d, n, _state);
        ae_vector_set_length(&x, n, _state);
        ae_vector_set_length(&xc, n, _state);
        ae_matrix_set_length(&q, kmax, n, _state);
        ae_vector_set_length(&r, kmax, _state);
        ae_vector_set_length(&activeset, n, _state);
        ae_vector_set_length(&tmp0, n, _state);
        alpha = 0.0;
        theta = 0.0;
        k = 0;
        tau = 1.0+ae_randomreal(_state);
        for(i=0; i<=n-1; i++)
        {
            d.ptr.p_double[i] = 1.0;
            b.ptr.p_double[i] = 0.0;
            xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            for(j=0; j<=n-1; j++)
            {
                a.ptr.pp_double[i][j] = (double)(0);
            }
        }
        cqminit(n, &s, _state);
        cqmsetd(&s, &d, tau, _state);
        for(pass=1; pass<=100; pass++)
        {
            
            /*
             * Select random modification type, apply modification.
             *
             * MKind is a random integer in [0,7] - number of specific 
             * modification to apply.
             */
            mkind = (double)(ae_randominteger(8, _state));
            if( ae_fp_eq(mkind,(double)(0)) )
            {
                
                /*
                 * Set non-zero D
                 */
                tau = 1.0+ae_randomreal(_state);
                for(i=0; i<=n-1; i++)
                {
                    d.ptr.p_double[i] = 2*ae_randomreal(_state)+1;
                }
                cqmsetd(&s, &d, tau, _state);
            }
            else
            {
                if( ae_fp_eq(mkind,(double)(1)) )
                {
                    
                    /*
                     * Set zero D.
                     * In case Alpha=0, set non-zero A.
                     */
                    if( ae_fp_eq(alpha,(double)(0)) )
                    {
                        alpha = 1.0+ae_randomreal(_state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=i+1; j<=n-1; j++)
                            {
                                a.ptr.pp_double[i][j] = 0.2*ae_randomreal(_state)-0.1;
                                a.ptr.pp_double[j][i] = a.ptr.pp_double[i][j];
                            }
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            a.ptr.pp_double[i][i] = 4+2*ae_randomreal(_state);
                        }
                        cqmseta(&s, &a, ae_fp_greater(ae_randomreal(_state),0.5), alpha, _state);
                    }
                    tau = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        d.ptr.p_double[i] = (double)(0);
                    }
                    cqmsetd(&s, &d, 0.0, _state);
                }
                else
                {
                    if( ae_fp_eq(mkind,(double)(2)) )
                    {
                        
                        /*
                         * Set non-zero A
                         */
                        alpha = 1.0+ae_randomreal(_state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=i+1; j<=n-1; j++)
                            {
                                a.ptr.pp_double[i][j] = 0.2*ae_randomreal(_state)-0.1;
                                a.ptr.pp_double[j][i] = a.ptr.pp_double[i][j];
                            }
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            a.ptr.pp_double[i][i] = 4+2*ae_randomreal(_state);
                        }
                        cqmseta(&s, &a, ae_fp_greater(ae_randomreal(_state),0.5), alpha, _state);
                    }
                    else
                    {
                        if( ae_fp_eq(mkind,(double)(3)) )
                        {
                            
                            /*
                             * Set zero A.
                             * In case Tau=0, set non-zero D.
                             */
                            if( ae_fp_eq(tau,(double)(0)) )
                            {
                                tau = 1.0+ae_randomreal(_state);
                                for(i=0; i<=n-1; i++)
                                {
                                    d.ptr.p_double[i] = 2*ae_randomreal(_state)+1;
                                }
                                cqmsetd(&s, &d, tau, _state);
                            }
                            alpha = 0.0;
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    a.ptr.pp_double[i][j] = (double)(0);
                                }
                            }
                            cqmseta(&s, &a, ae_fp_greater(ae_randomreal(_state),0.5), alpha, _state);
                        }
                        else
                        {
                            if( ae_fp_eq(mkind,(double)(4)) )
                            {
                                
                                /*
                                 * Set B.
                                 */
                                for(i=0; i<=n-1; i++)
                                {
                                    b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                                }
                                cqmsetb(&s, &b, _state);
                            }
                            else
                            {
                                if( ae_fp_eq(mkind,(double)(5)) )
                                {
                                    
                                    /*
                                     * Set Q.
                                     */
                                    k = ae_randominteger(kmax+1, _state);
                                    theta = 1.0+ae_randomreal(_state);
                                    for(i=0; i<=k-1; i++)
                                    {
                                        r.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                                        for(j=0; j<=n-1; j++)
                                        {
                                            q.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                                        }
                                    }
                                    cqmsetq(&s, &q, &r, k, theta, _state);
                                }
                                else
                                {
                                    if( ae_fp_eq(mkind,(double)(6)) )
                                    {
                                        
                                        /*
                                         * Set active set
                                         */
                                        for(i=0; i<=n-1; i++)
                                        {
                                            activeset.ptr.p_bool[i] = ae_fp_greater(ae_randomreal(_state),0.5);
                                            xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                                        }
                                        cqmsetactiveset(&s, &xc, &activeset, _state);
                                    }
                                    else
                                    {
                                        if( ae_fp_eq(mkind,(double)(7)) )
                                        {
                                            
                                            /*
                                             * Rewrite main diagonal
                                             */
                                            if( ae_fp_eq(alpha,(double)(0)) )
                                            {
                                                alpha = 1.0;
                                            }
                                            for(i=0; i<=n-1; i++)
                                            {
                                                tmp0.ptr.p_double[i] = 1+ae_randomreal(_state);
                                                a.ptr.pp_double[i][i] = tmp0.ptr.p_double[i]/alpha;
                                            }
                                            cqmrewritedensediagonal(&s, &tmp0, _state);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            /*
             * generate random point with respect to constraints,
             * test model at this point
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                if( activeset.ptr.p_bool[i] )
                {
                    x.ptr.p_double[i] = xc.ptr.p_double[i];
                }
            }
            v = 0.0;
            for(i=0; i<=n-1; i++)
            {
                v = v+x.ptr.p_double[i]*b.ptr.p_double[i];
            }
            if( ae_fp_greater(tau,(double)(0)) )
            {
                for(i=0; i<=n-1; i++)
                {
                    v = v+0.5*tau*d.ptr.p_double[i]*ae_sqr(x.ptr.p_double[i], _state);
                }
            }
            if( ae_fp_greater(alpha,(double)(0)) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        v = v+0.5*alpha*x.ptr.p_double[i]*a.ptr.pp_double[i][j]*x.ptr.p_double[j];
                    }
                }
            }
            if( ae_fp_greater(theta,(double)(0)) )
            {
                for(i=0; i<=k-1; i++)
                {
                    v2 = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    v = v+0.5*theta*ae_sqr(v2-r.ptr.p_double[i], _state);
                }
            }
            v2 = cqmeval(&s, &x, _state);
            eval2errors = eval2errors||ae_fp_greater(ae_fabs(v-v2, _state),10000*ae_machineepsilon);
            v2 = cqmdebugconstrainedevalt(&s, &x, _state);
            eval2errors = eval2errors||ae_fp_greater(ae_fabs(v-v2, _state),10000*ae_machineepsilon);
            v2 = cqmdebugconstrainedevale(&s, &x, _state);
            eval2errors = eval2errors||ae_fp_greater(ae_fabs(v-v2, _state),10000*ae_machineepsilon);
        }
    }
    waserrors = waserrors||eval2errors;
    
    /*
     * Newton0 test: unconstrained optimization
     */
    newton0errors = ae_false;
    for(n=1; n<=5; n++)
    {
        for(k=0; k<=2*n; k++)
        {
            
            /*
             * Allocate place
             */
            ae_matrix_set_length(&a, n, n, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&x0, n, _state);
            if( k>0 )
            {
                ae_matrix_set_length(&q, k, n, _state);
                ae_vector_set_length(&r, k, _state);
            }
            
            /*
             * Generate problem with known solution x0:
             *   min f(x),
             *   f(x) = 0.5*(x-x0)'*A*(x-x0)
             *        = 0.5*x'*A*x + (-x0'*A)*x + 0.5*x0'*A*x0'
             */
            alpha = ae_randomreal(_state)+1.0;
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                a.ptr.pp_double[i][i] = 10*(1+ae_randomreal(_state));
                for(j=i+1; j<=n-1; j++)
                {
                    v = 0.1*ae_randomreal(_state)-0.05;
                    a.ptr.pp_double[i][j] = v;
                    a.ptr.pp_double[j][i] = v;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                b.ptr.p_double[i] = -alpha*v;
            }
            theta = ae_randomreal(_state)+1.0;
            for(i=0; i<=k-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    q.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                }
                v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                r.ptr.p_double[i] = v;
            }
            
            /*
             * Build model, evaluate at random point X, compare
             */
            cqminit(n, &s, _state);
            cqmseta(&s, &a, ae_fp_greater(ae_randomreal(_state),0.5), alpha, _state);
            cqmsetb(&s, &b, _state);
            cqmsetq(&s, &q, &r, k, theta, _state);
            cqmconstrainedoptimum(&s, &x, _state);
            for(i=0; i<=n-1; i++)
            {
                newton0errors = newton0errors||ae_fp_greater(ae_fabs(x.ptr.p_double[i]-x0.ptr.p_double[i], _state),1.0E6*ae_machineepsilon);
            }
        }
    }
    waserrors = waserrors||newton0errors;
    
    /*
     * Newton1 test: constrained optimization
     */
    newton1errors = ae_false;
    h = 1.0E-3;
    for(n=1; n<=5; n++)
    {
        for(k=0; k<=2*n; k++)
        {
            
            /*
             * Allocate place
             */
            ae_matrix_set_length(&a, n, n, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&xc, n, _state);
            ae_vector_set_length(&activeset, n, _state);
            if( k>0 )
            {
                ae_matrix_set_length(&q, k, n, _state);
                ae_vector_set_length(&r, k, _state);
            }
            
            /*
             * Generate test problem with unknown solution.
             */
            alpha = ae_randomreal(_state)+1.0;
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_double[i][i] = 10*(1+ae_randomreal(_state));
                b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                activeset.ptr.p_bool[i] = ae_fp_greater(ae_randomreal(_state),0.5);
                for(j=i+1; j<=n-1; j++)
                {
                    v = 0.1*ae_randomreal(_state)-0.05;
                    a.ptr.pp_double[i][j] = v;
                    a.ptr.pp_double[j][i] = v;
                }
            }
            theta = ae_randomreal(_state)+1.0;
            for(i=0; i<=k-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    q.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                }
                r.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            
            /*
             * Build model, find solution
             */
            cqminit(n, &s, _state);
            cqmseta(&s, &a, ae_fp_greater(ae_randomreal(_state),0.5), alpha, _state);
            cqmsetb(&s, &b, _state);
            cqmsetq(&s, &q, &r, k, theta, _state);
            cqmsetactiveset(&s, &xc, &activeset, _state);
            if( cqmconstrainedoptimum(&s, &x, _state) )
            {
                
                /*
                 * Check that constraints are satisfied,
                 * and that solution is true optimum
                 */
                f0 = cqmeval(&s, &x, _state);
                for(i=0; i<=n-1; i++)
                {
                    newton1errors = newton1errors||(activeset.ptr.p_bool[i]&&ae_fp_neq(x.ptr.p_double[i],xc.ptr.p_double[i]));
                    if( !activeset.ptr.p_bool[i] )
                    {
                        v = x.ptr.p_double[i];
                        x.ptr.p_double[i] = v+h;
                        v2 = cqmeval(&s, &x, _state);
                        newton1errors = newton1errors||ae_fp_less(v2,f0);
                        x.ptr.p_double[i] = v-h;
                        v2 = cqmeval(&s, &x, _state);
                        newton1errors = newton1errors||ae_fp_less(v2,f0);
                        x.ptr.p_double[i] = v;
                    }
                }
            }
            else
            {
                newton1errors = ae_true;
            }
        }
    }
    waserrors = waserrors||newton1errors;
    
    /*
     * Newton2 test: we test ability to work with diagonal matrices, including
     * very large ones (up to 100.000 elements). This test checks that:
     * a) we can work with Alpha=0, i.e. when we have strictly diagonal A
     * b) diagonal problems are handled efficiently, i.e. algorithm will
     *    successfully solve problem with N=100.000
     *
     * Test problem:
     * * diagonal term D and rank-K term Q
     * * known solution X0,
     * * about 50% of constraints are active and equal to components of X0
     */
    newton2errors = ae_false;
    for(nkind=0; nkind<=5; nkind++)
    {
        for(k=0; k<=5; k++)
        {
            n = ae_round(ae_pow((double)(n), (double)(nkind), _state), _state);
            
            /*
             * generate problem
             */
            ae_vector_set_length(&d, n, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&activeset, n, _state);
            if( k>0 )
            {
                ae_matrix_set_length(&q, k, n, _state);
                ae_vector_set_length(&r, k, _state);
            }
            tau = 1+ae_randomreal(_state);
            theta = 1+ae_randomreal(_state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                d.ptr.p_double[i] = 1+ae_randomreal(_state);
                b.ptr.p_double[i] = -x0.ptr.p_double[i]*tau*d.ptr.p_double[i];
                activeset.ptr.p_bool[i] = ae_fp_greater(ae_randomreal(_state),0.5);
            }
            for(i=0; i<=k-1; i++)
            {
                v = 0.0;
                for(j=0; j<=n-1; j++)
                {
                    q.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    v = v+q.ptr.pp_double[i][j]*x0.ptr.p_double[j];
                }
                r.ptr.p_double[i] = v;
            }
            
            /*
             * Solve, test
             */
            cqminit(n, &s, _state);
            cqmsetb(&s, &b, _state);
            cqmsetd(&s, &d, tau, _state);
            cqmsetq(&s, &q, &r, k, theta, _state);
            cqmsetactiveset(&s, &x0, &activeset, _state);
            if( cqmconstrainedoptimum(&s, &x, _state) )
            {
                
                /*
                 * Check that constraints are satisfied,
                 * and that solution is true optimum
                 */
                f0 = cqmeval(&s, &x, _state);
                for(i=0; i<=n-1; i++)
                {
                    newton2errors = newton2errors||(activeset.ptr.p_bool[i]&&ae_fp_neq(x.ptr.p_double[i],x0.ptr.p_double[i]));
                    newton2errors = newton2errors||(!activeset.ptr.p_bool[i]&&ae_fp_greater(ae_fabs(x.ptr.p_double[i]-x0.ptr.p_double[i], _state),1000*ae_machineepsilon));
                }
                
                /*
                 * Check that constrained evaluation at some point gives correct results
                 */
                for(i=0; i<=n-1; i++)
                {
                    if( activeset.ptr.p_bool[i] )
                    {
                        x.ptr.p_double[i] = x0.ptr.p_double[i];
                    }
                    else
                    {
                        x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                }
                v = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    v = v+0.5*tau*d.ptr.p_double[i]*ae_sqr(x.ptr.p_double[i], _state)+x.ptr.p_double[i]*b.ptr.p_double[i];
                }
                for(i=0; i<=k-1; i++)
                {
                    v2 = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    v = v+0.5*theta*ae_sqr(v2-r.ptr.p_double[i], _state);
                }
                v2 = cqmeval(&s, &x, _state);
                newton2errors = (newton2errors||!ae_isfinite(v2, _state))||ae_fp_greater(ae_fabs(v-v2, _state),10000*ae_machineepsilon);
                v2 = cqmdebugconstrainedevalt(&s, &x, _state);
                newton2errors = (newton2errors||!ae_isfinite(v2, _state))||ae_fp_greater(ae_fabs(v-v2, _state),10000*ae_machineepsilon);
                v2 = cqmdebugconstrainedevale(&s, &x, _state);
                newton2errors = (newton2errors||!ae_isfinite(v2, _state))||ae_fp_greater(ae_fabs(v-v2, _state),10000*ae_machineepsilon);
            }
            else
            {
                newton2errors = ae_true;
            }
        }
    }
    waserrors = waserrors||newton2errors;
    
    /*
     * report
     */
    if( !silent )
    {
        printf("TESTING CONVEX QUADRATIC MODELS\n");
        printf("Eval0 test:                               ");
        if( eval0errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Eval1 test:                               ");
        if( eval1errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Eval2 test:                               ");
        if( eval2errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Newton0 test:                             ");
        if( newton0errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Newton1 test:                             ");
        if( newton1errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Newton2 test:                             ");
        if( newton2errors )
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
ae_bool _pexec_testcqmodels(ae_bool silent, ae_state *_state)
{
    return testcqmodels(silent, _state);
}


/*$ End $*/
