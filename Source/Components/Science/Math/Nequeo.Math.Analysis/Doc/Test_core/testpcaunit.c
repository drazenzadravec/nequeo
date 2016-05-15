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
#include "testpcaunit.h"


/*$ Declarations $*/
static void testpcaunit_calculatemv(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* mean,
     double* means,
     double* stddev,
     double* stddevs,
     ae_state *_state);


/*$ Body $*/


ae_bool testpca(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t passcount;
    ae_int_t maxn;
    ae_int_t maxm;
    double threshold;
    ae_int_t m;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t info;
    ae_vector means;
    ae_vector s;
    ae_vector t2;
    ae_vector t3;
    ae_matrix v;
    ae_matrix x;
    double t;
    double h;
    double tmean;
    double tmeans;
    double tstddev;
    double tstddevs;
    double tmean2;
    double tmeans2;
    double tstddev2;
    double tstddevs2;
    ae_bool pcaconverrors;
    ae_bool pcaorterrors;
    ae_bool pcavarerrors;
    ae_bool pcaopterrors;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&means, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&t2, 0, DT_REAL, _state);
    ae_vector_init(&t3, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);
    ae_matrix_init(&x, 0, 0, DT_REAL, _state);

    
    /*
     * Primary settings
     */
    maxm = 10;
    maxn = 100;
    passcount = 1;
    threshold = 1000*ae_machineepsilon;
    waserrors = ae_false;
    pcaconverrors = ae_false;
    pcaorterrors = ae_false;
    pcavarerrors = ae_false;
    pcaopterrors = ae_false;
    
    /*
     * Test 1: N random points in M-dimensional space
     */
    for(m=1; m<=maxm; m++)
    {
        for(n=1; n<=maxn; n++)
        {
            
            /*
             * Generate task
             */
            ae_matrix_set_length(&x, n-1+1, m-1+1, _state);
            ae_vector_set_length(&means, m-1+1, _state);
            for(j=0; j<=m-1; j++)
            {
                means.ptr.p_double[j] = 1.5*ae_randomreal(_state)-0.75;
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    x.ptr.pp_double[i][j] = means.ptr.p_double[j]+(2*ae_randomreal(_state)-1);
                }
            }
            
            /*
             * Solve
             */
            pcabuildbasis(&x, n, m, &info, &s, &v, _state);
            if( info!=1 )
            {
                pcaconverrors = ae_true;
                continue;
            }
            
            /*
             * Orthogonality test
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    t = ae_v_dotproduct(&v.ptr.pp_double[0][i], v.stride, &v.ptr.pp_double[0][j], v.stride, ae_v_len(0,m-1));
                    if( i==j )
                    {
                        t = t-1;
                    }
                    pcaorterrors = pcaorterrors||ae_fp_greater(ae_fabs(t, _state),threshold);
                }
            }
            
            /*
             * Variance test
             */
            ae_vector_set_length(&t2, n-1+1, _state);
            for(k=0; k<=m-1; k++)
            {
                for(i=0; i<=n-1; i++)
                {
                    t = ae_v_dotproduct(&x.ptr.pp_double[i][0], 1, &v.ptr.pp_double[0][k], v.stride, ae_v_len(0,m-1));
                    t2.ptr.p_double[i] = t;
                }
                testpcaunit_calculatemv(&t2, n, &tmean, &tmeans, &tstddev, &tstddevs, _state);
                if( n!=1 )
                {
                    t = ae_sqr(tstddev, _state)*n/(n-1);
                }
                else
                {
                    t = (double)(0);
                }
                pcavarerrors = pcavarerrors||ae_fp_greater(ae_fabs(t-s.ptr.p_double[k], _state),threshold);
            }
            for(k=0; k<=m-2; k++)
            {
                pcavarerrors = pcavarerrors||ae_fp_less(s.ptr.p_double[k],s.ptr.p_double[k+1]);
            }
            
            /*
             * Optimality: different perturbations in V[..,0] can't
             * increase variance of projection - can only decrease.
             */
            ae_vector_set_length(&t2, n-1+1, _state);
            ae_vector_set_length(&t3, n-1+1, _state);
            for(i=0; i<=n-1; i++)
            {
                t = ae_v_dotproduct(&x.ptr.pp_double[i][0], 1, &v.ptr.pp_double[0][0], v.stride, ae_v_len(0,m-1));
                t2.ptr.p_double[i] = t;
            }
            testpcaunit_calculatemv(&t2, n, &tmean, &tmeans, &tstddev, &tstddevs, _state);
            for(k=0; k<=2*m-1; k++)
            {
                h = 0.001;
                if( k%2!=0 )
                {
                    h = -h;
                }
                ae_v_move(&t3.ptr.p_double[0], 1, &t2.ptr.p_double[0], 1, ae_v_len(0,n-1));
                ae_v_addd(&t3.ptr.p_double[0], 1, &x.ptr.pp_double[0][k/2], x.stride, ae_v_len(0,n-1), h);
                t = (double)(0);
                for(j=0; j<=m-1; j++)
                {
                    if( j!=k/2 )
                    {
                        t = t+ae_sqr(v.ptr.pp_double[j][0], _state);
                    }
                    else
                    {
                        t = t+ae_sqr(v.ptr.pp_double[j][0]+h, _state);
                    }
                }
                t = 1/ae_sqrt(t, _state);
                ae_v_muld(&t3.ptr.p_double[0], 1, ae_v_len(0,n-1), t);
                testpcaunit_calculatemv(&t3, n, &tmean2, &tmeans2, &tstddev2, &tstddevs2, _state);
                pcaopterrors = pcaopterrors||ae_fp_greater(tstddev2,tstddev+threshold);
            }
        }
    }
    
    /*
     * Special test for N=0
     */
    for(m=1; m<=maxm; m++)
    {
        
        /*
         * Solve
         */
        pcabuildbasis(&x, 0, m, &info, &s, &v, _state);
        if( info!=1 )
        {
            pcaconverrors = ae_true;
            continue;
        }
        
        /*
         * Orthogonality test
         */
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                t = ae_v_dotproduct(&v.ptr.pp_double[0][i], v.stride, &v.ptr.pp_double[0][j], v.stride, ae_v_len(0,m-1));
                if( i==j )
                {
                    t = t-1;
                }
                pcaorterrors = pcaorterrors||ae_fp_greater(ae_fabs(t, _state),threshold);
            }
        }
    }
    
    /*
     * Final report
     */
    waserrors = ((pcaconverrors||pcaorterrors)||pcavarerrors)||pcaopterrors;
    if( !silent )
    {
        printf("PCA TEST\n");
        printf("TOTAL RESULTS:                           ");
        if( !waserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* CONVERGENCE                            ");
        if( !pcaconverrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* ORTOGONALITY                           ");
        if( !pcaorterrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* VARIANCE REPORT                        ");
        if( !pcavarerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* OPTIMALITY                             ");
        if( !pcaopterrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        if( waserrors )
        {
            printf("TEST SUMMARY: FAILED\n");
        }
        else
        {
            printf("TEST SUMMARY: PASSED\n");
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
ae_bool _pexec_testpca(ae_bool silent, ae_state *_state)
{
    return testpca(silent, _state);
}


/*************************************************************************
Moments estimates and their errors
*************************************************************************/
static void testpcaunit_calculatemv(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* mean,
     double* means,
     double* stddev,
     double* stddevs,
     ae_state *_state)
{
    ae_int_t i;
    double v1;
    double v2;
    double variance;

    *mean = 0;
    *means = 0;
    *stddev = 0;
    *stddevs = 0;

    *mean = (double)(0);
    *means = (double)(1);
    *stddev = (double)(0);
    *stddevs = (double)(1);
    variance = (double)(0);
    if( n<=1 )
    {
        return;
    }
    
    /*
     * Mean
     */
    for(i=0; i<=n-1; i++)
    {
        *mean = *mean+x->ptr.p_double[i];
    }
    *mean = *mean/n;
    
    /*
     * Variance (using corrected two-pass algorithm)
     */
    if( n!=1 )
    {
        v1 = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v1 = v1+ae_sqr(x->ptr.p_double[i]-(*mean), _state);
        }
        v2 = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v2 = v2+(x->ptr.p_double[i]-(*mean));
        }
        v2 = ae_sqr(v2, _state)/n;
        variance = (v1-v2)/n;
        if( ae_fp_less(variance,(double)(0)) )
        {
            variance = (double)(0);
        }
        *stddev = ae_sqrt(variance, _state);
    }
    
    /*
     * Errors
     */
    *means = *stddev/ae_sqrt((double)(n), _state);
    *stddevs = *stddev*ae_sqrt((double)(2), _state)/ae_sqrt((double)(n-1), _state);
}


/*$ End $*/
