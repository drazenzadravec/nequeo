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
#include "testbasestatunit.h"


/*$ Declarations $*/
static void testbasestatunit_testranking(ae_bool* err, ae_state *_state);


/*$ Body $*/


ae_bool testbasestat(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_bool s1errors;
    ae_bool covcorrerrors;
    ae_bool rankerrors;
    double threshold;
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;
    ae_int_t kx;
    ae_int_t ky;
    ae_int_t ctype;
    ae_int_t cidxx;
    ae_int_t cidxy;
    ae_vector x;
    ae_vector y;
    ae_matrix mx;
    ae_matrix my;
    ae_matrix cc;
    ae_matrix cp;
    ae_matrix cs;
    double mean;
    double variance;
    double skewness;
    double kurtosis;
    double adev;
    double median;
    double pv;
    double v;
    double tmean;
    double tvariance;
    double tskewness;
    double tkurtosis;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_matrix_init(&mx, 0, 0, DT_REAL, _state);
    ae_matrix_init(&my, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cc, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cp, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cs, 0, 0, DT_REAL, _state);

    
    /*
     * Primary settings
     */
    waserrors = ae_false;
    s1errors = ae_false;
    covcorrerrors = ae_false;
    rankerrors = ae_false;
    threshold = 1000*ae_machineepsilon;
    
    /*
     * Ranking
     */
    testbasestatunit_testranking(&rankerrors, _state);
    
    /*
     * * prepare X and Y - two test samples
     * * test 1-sample coefficients
     * * test for SampleMean, SampleVariance,
     *   SampleSkewness, SampleKurtosis.
     */
    n = 10;
    ae_vector_set_length(&x, n, _state);
    for(i=0; i<=n-1; i++)
    {
        x.ptr.p_double[i] = ae_sqr((double)(i), _state);
    }
    samplemoments(&x, n, &mean, &variance, &skewness, &kurtosis, _state);
    s1errors = s1errors||ae_fp_greater(ae_fabs(mean-28.5, _state),0.001);
    s1errors = s1errors||ae_fp_greater(ae_fabs(variance-801.1667, _state),0.001);
    s1errors = s1errors||ae_fp_greater(ae_fabs(skewness-0.5751, _state),0.001);
    s1errors = s1errors||ae_fp_greater(ae_fabs(kurtosis+1.2666, _state),0.001);
    tmean = samplemean(&x, n, _state);
    tvariance = samplevariance(&x, n, _state);
    tskewness = sampleskewness(&x, n, _state);
    tkurtosis = samplekurtosis(&x, n, _state);
    s1errors = s1errors||ae_fp_neq(mean-tmean,(double)(0));
    s1errors = s1errors||ae_fp_neq(variance-tvariance,(double)(0));
    s1errors = s1errors||ae_fp_neq(skewness-tskewness,(double)(0));
    s1errors = s1errors||ae_fp_neq(kurtosis-tkurtosis,(double)(0));
    sampleadev(&x, n, &adev, _state);
    s1errors = s1errors||ae_fp_greater(ae_fabs(adev-23.2000, _state),0.001);
    samplemedian(&x, n, &median, _state);
    s1errors = s1errors||ae_fp_greater(ae_fabs(median-0.5*(16+25), _state),0.001);
    for(i=0; i<=n-1; i++)
    {
        samplepercentile(&x, n, (double)i/(double)(n-1), &pv, _state);
        s1errors = s1errors||ae_fp_greater(ae_fabs(pv-x.ptr.p_double[i], _state),0.001);
    }
    samplepercentile(&x, n, 0.5, &pv, _state);
    s1errors = s1errors||ae_fp_greater(ae_fabs(pv-0.5*(16+25), _state),0.001);
    
    /*
     * test covariance/correlation:
     * * 2-sample coefficients
     *
     * We generate random matrices MX and MY
     */
    n = 10;
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&y, n, _state);
    for(i=0; i<=n-1; i++)
    {
        x.ptr.p_double[i] = ae_sqr((double)(i), _state);
        y.ptr.p_double[i] = (double)(i);
    }
    covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(pearsoncorr2(&x, &y, n, _state)-0.9627, _state),0.0001);
    covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(spearmancorr2(&x, &y, n, _state)-1.0000, _state),0.0001);
    covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(cov2(&x, &y, n, _state)-82.5000, _state),0.0001);
    for(i=0; i<=n-1; i++)
    {
        x.ptr.p_double[i] = ae_sqr(i-0.5*n, _state);
        y.ptr.p_double[i] = (double)(i);
    }
    covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(pearsoncorr2(&x, &y, n, _state)+0.3676, _state),0.0001);
    covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(spearmancorr2(&x, &y, n, _state)+0.2761, _state),0.0001);
    covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(cov2(&x, &y, n, _state)+9.1667, _state),0.0001);
    
    /*
     * test covariance/correlation:
     * * matrix covariance/correlation
     * * matrix cross-covariance/cross-correlation
     *
     * We generate random matrices MX and MY which contain KX (KY)
     * columns, all except one are random, one of them is constant.
     * We test that function (a) do not crash on constant column,
     * and (b) return variances/correlations that are exactly zero
     * for this column.
     *
     * CType control variable controls type of constant: 0 - no constant
     * column, 1 - zero column, 2 - nonzero column with value whose
     * binary representation contains many non-zero bits. Using such
     * type of constant column we are able to ensure than even in the
     * presense of roundoff error functions correctly detect constant
     * columns.
     */
    for(n=0; n<=10; n++)
    {
        if( n>0 )
        {
            ae_vector_set_length(&x, n, _state);
            ae_vector_set_length(&y, n, _state);
        }
        for(ctype=0; ctype<=2; ctype++)
        {
            for(kx=1; kx<=10; kx++)
            {
                for(ky=1; ky<=10; ky++)
                {
                    
                    /*
                     * Fill matrices, add constant column (when CType=1 or =2)
                     */
                    cidxx = -1;
                    cidxy = -1;
                    if( n>0 )
                    {
                        ae_matrix_set_length(&mx, n, kx, _state);
                        ae_matrix_set_length(&my, n, ky, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=kx-1; j++)
                            {
                                mx.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                            }
                            for(j=0; j<=ky-1; j++)
                            {
                                my.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                            }
                        }
                        if( ctype==1 )
                        {
                            cidxx = ae_randominteger(kx, _state);
                            cidxy = ae_randominteger(ky, _state);
                            for(i=0; i<=n-1; i++)
                            {
                                mx.ptr.pp_double[i][cidxx] = 0.0;
                                my.ptr.pp_double[i][cidxy] = 0.0;
                            }
                        }
                        if( ctype==2 )
                        {
                            cidxx = ae_randominteger(kx, _state);
                            cidxy = ae_randominteger(ky, _state);
                            v = ae_sqrt((double)(ae_randominteger(kx, _state)+1)/(double)kx, _state);
                            for(i=0; i<=n-1; i++)
                            {
                                mx.ptr.pp_double[i][cidxx] = v;
                                my.ptr.pp_double[i][cidxy] = v;
                            }
                        }
                    }
                    
                    /*
                     * test covariance/correlation matrix using
                     * 2-sample functions as reference point.
                     *
                     * We also test that coefficients for constant variables
                     * are exactly zero.
                     */
                    covm(&mx, n, kx, &cc, _state);
                    pearsoncorrm(&mx, n, kx, &cp, _state);
                    spearmancorrm(&mx, n, kx, &cs, _state);
                    for(i=0; i<=kx-1; i++)
                    {
                        for(j=0; j<=kx-1; j++)
                        {
                            if( n>0 )
                            {
                                ae_v_move(&x.ptr.p_double[0], 1, &mx.ptr.pp_double[0][i], mx.stride, ae_v_len(0,n-1));
                                ae_v_move(&y.ptr.p_double[0], 1, &mx.ptr.pp_double[0][j], mx.stride, ae_v_len(0,n-1));
                            }
                            covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(cov2(&x, &y, n, _state)-cc.ptr.pp_double[i][j], _state),threshold);
                            covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(pearsoncorr2(&x, &y, n, _state)-cp.ptr.pp_double[i][j], _state),threshold);
                            covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(spearmancorr2(&x, &y, n, _state)-cs.ptr.pp_double[i][j], _state),threshold);
                        }
                    }
                    if( ctype!=0&&n>0 )
                    {
                        for(i=0; i<=kx-1; i++)
                        {
                            covcorrerrors = covcorrerrors||ae_fp_neq(cc.ptr.pp_double[i][cidxx],(double)(0));
                            covcorrerrors = covcorrerrors||ae_fp_neq(cc.ptr.pp_double[cidxx][i],(double)(0));
                            covcorrerrors = covcorrerrors||ae_fp_neq(cp.ptr.pp_double[i][cidxx],(double)(0));
                            covcorrerrors = covcorrerrors||ae_fp_neq(cp.ptr.pp_double[cidxx][i],(double)(0));
                            covcorrerrors = covcorrerrors||ae_fp_neq(cs.ptr.pp_double[i][cidxx],(double)(0));
                            covcorrerrors = covcorrerrors||ae_fp_neq(cs.ptr.pp_double[cidxx][i],(double)(0));
                        }
                    }
                    
                    /*
                     * test cross-covariance/cross-correlation matrix using
                     * 2-sample functions as reference point.
                     *
                     * We also test that coefficients for constant variables
                     * are exactly zero.
                     */
                    covm2(&mx, &my, n, kx, ky, &cc, _state);
                    pearsoncorrm2(&mx, &my, n, kx, ky, &cp, _state);
                    spearmancorrm2(&mx, &my, n, kx, ky, &cs, _state);
                    for(i=0; i<=kx-1; i++)
                    {
                        for(j=0; j<=ky-1; j++)
                        {
                            if( n>0 )
                            {
                                ae_v_move(&x.ptr.p_double[0], 1, &mx.ptr.pp_double[0][i], mx.stride, ae_v_len(0,n-1));
                                ae_v_move(&y.ptr.p_double[0], 1, &my.ptr.pp_double[0][j], my.stride, ae_v_len(0,n-1));
                            }
                            covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(cov2(&x, &y, n, _state)-cc.ptr.pp_double[i][j], _state),threshold);
                            covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(pearsoncorr2(&x, &y, n, _state)-cp.ptr.pp_double[i][j], _state),threshold);
                            covcorrerrors = covcorrerrors||ae_fp_greater(ae_fabs(spearmancorr2(&x, &y, n, _state)-cs.ptr.pp_double[i][j], _state),threshold);
                        }
                    }
                    if( ctype!=0&&n>0 )
                    {
                        for(i=0; i<=kx-1; i++)
                        {
                            covcorrerrors = covcorrerrors||ae_fp_neq(cc.ptr.pp_double[i][cidxy],(double)(0));
                            covcorrerrors = covcorrerrors||ae_fp_neq(cp.ptr.pp_double[i][cidxy],(double)(0));
                            covcorrerrors = covcorrerrors||ae_fp_neq(cs.ptr.pp_double[i][cidxy],(double)(0));
                        }
                        for(j=0; j<=ky-1; j++)
                        {
                            covcorrerrors = covcorrerrors||ae_fp_neq(cc.ptr.pp_double[cidxx][j],(double)(0));
                            covcorrerrors = covcorrerrors||ae_fp_neq(cp.ptr.pp_double[cidxx][j],(double)(0));
                            covcorrerrors = covcorrerrors||ae_fp_neq(cs.ptr.pp_double[cidxx][j],(double)(0));
                        }
                    }
                }
            }
        }
    }
    
    /*
     * Final report
     */
    waserrors = (s1errors||covcorrerrors)||rankerrors;
    if( !silent )
    {
        printf("DESC.STAT TEST\n");
        printf("TOTAL RESULTS:                           ");
        if( !waserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* 1-SAMPLE FUNCTIONALITY:                ");
        if( !s1errors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* CORRELATION/COVARIATION:               ");
        if( !covcorrerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* RANKING:                               ");
        if( !rankerrors )
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
ae_bool _pexec_testbasestat(ae_bool silent, ae_state *_state)
{
    return testbasestat(silent, _state);
}


/*************************************************************************
This function tests ranking functionality. In case  of  failure  it  sets
Err parameter to True; this parameter is left unchanged otherwise.
*************************************************************************/
static void testbasestatunit_testranking(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t testk;
    ae_int_t npoints;
    ae_int_t nfeatures;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_matrix xy0;
    ae_matrix xy1;
    ae_matrix xy2;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy2, 0, 0, DT_REAL, _state);

    
    /*
     * Test 1 - large array, unique ranks, each row is obtained as follows:
     * * we generate X[i=0..N-1] = I
     * * we add random noise: X[i] := X[i] + 0.2*randomreal()-0.1
     * * we perform random permutation
     *
     * Such dataset has following properties:
     * * all data are unique within their rows
     * * rank(X[i]) = round(X[i])
     *
     * We perform several tests with different NPoints/NFeatures.
     */
    for(testk=0; testk<=1; testk++)
    {
        
        /*
         * Select problem size
         */
        if( testk==0 )
        {
            npoints = 200;
            nfeatures = 1000;
        }
        else
        {
            npoints = 1000;
            nfeatures = 200;
        }
        
        /*
         * Generate XY0, XY1, XY2
         */
        ae_matrix_set_length(&xy0, npoints, nfeatures, _state);
        ae_matrix_set_length(&xy1, npoints, nfeatures, _state);
        ae_matrix_set_length(&xy2, npoints, nfeatures, _state);
        for(i=0; i<=npoints-1; i++)
        {
            for(j=0; j<=nfeatures-1; j++)
            {
                xy0.ptr.pp_double[i][j] = j+0.2*ae_randomreal(_state)-0.1;
            }
            for(j=0; j<=nfeatures-2; j++)
            {
                k = ae_randominteger(nfeatures-j, _state);
                if( k!=0 )
                {
                    v = xy0.ptr.pp_double[i][j];
                    xy0.ptr.pp_double[i][j] = xy0.ptr.pp_double[i][j+k];
                    xy0.ptr.pp_double[i][j+k] = v;
                }
            }
            for(j=0; j<=nfeatures-1; j++)
            {
                xy1.ptr.pp_double[i][j] = xy0.ptr.pp_double[i][j];
                xy2.ptr.pp_double[i][j] = xy0.ptr.pp_double[i][j];
            }
        }
        
        /*
         * Test uncentered ranks
         */
        rankdata(&xy0, npoints, nfeatures, _state);
        for(i=0; i<=npoints-1; i++)
        {
            for(j=0; j<=nfeatures-1; j++)
            {
                if( ae_fp_neq(xy0.ptr.pp_double[i][j],(double)(ae_round(xy2.ptr.pp_double[i][j], _state))) )
                {
                    *err = ae_true;
                }
            }
        }
        
        /*
         * Test centered ranks:
         * they must be equal to uncentered ranks minus (NFeatures-1)/2
         */
        rankdatacentered(&xy1, npoints, nfeatures, _state);
        for(i=0; i<=npoints-1; i++)
        {
            for(j=0; j<=nfeatures-1; j++)
            {
                if( ae_fp_neq(xy1.ptr.pp_double[i][j],ae_round(xy2.ptr.pp_double[i][j], _state)-(double)(nfeatures-1)/(double)2) )
                {
                    *err = ae_true;
                }
            }
        }
    }
    
    /*
     * Test correct handling of tied ranks
     */
    npoints = 3;
    nfeatures = 4;
    ae_matrix_set_length(&xy0, npoints, nfeatures, _state);
    ae_matrix_set_length(&xy1, npoints, nfeatures, _state);
    xy0.ptr.pp_double[0][0] = 2.25;
    xy0.ptr.pp_double[0][1] = 3.75;
    xy0.ptr.pp_double[0][2] = 3.25;
    xy0.ptr.pp_double[0][3] = 2.25;
    xy0.ptr.pp_double[1][0] = (double)(2);
    xy0.ptr.pp_double[1][1] = (double)(2);
    xy0.ptr.pp_double[1][2] = (double)(2);
    xy0.ptr.pp_double[1][3] = (double)(7);
    xy0.ptr.pp_double[2][0] = (double)(9);
    xy0.ptr.pp_double[2][1] = (double)(9);
    xy0.ptr.pp_double[2][2] = (double)(9);
    xy0.ptr.pp_double[2][3] = (double)(9);
    for(i=0; i<=npoints-1; i++)
    {
        for(j=0; j<=nfeatures-1; j++)
        {
            xy1.ptr.pp_double[i][j] = xy0.ptr.pp_double[i][j];
        }
    }
    rankdata(&xy0, npoints, nfeatures, _state);
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[0][0]-0.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[0][1]-3.0, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[0][2]-2.0, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[0][3]-0.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[1][0]-1.0, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[1][1]-1.0, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[1][2]-1.0, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[1][3]-3.0, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[2][0]-1.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[2][1]-1.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[2][2]-1.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[2][3]-1.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    rankdatacentered(&xy1, npoints, nfeatures, _state);
    if( ae_fp_greater(ae_fabs(xy1.ptr.pp_double[0][0]+1.0, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy1.ptr.pp_double[0][1]-1.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy1.ptr.pp_double[0][2]-0.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy1.ptr.pp_double[0][3]+1.0, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy1.ptr.pp_double[1][0]+0.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy1.ptr.pp_double[1][1]+0.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy1.ptr.pp_double[1][2]+0.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_greater(ae_fabs(xy1.ptr.pp_double[1][3]-1.5, _state),10*ae_machineepsilon) )
    {
        *err = ae_true;
    }
    if( ae_fp_neq(xy1.ptr.pp_double[2][0],(double)(0)) )
    {
        *err = ae_true;
    }
    if( ae_fp_neq(xy1.ptr.pp_double[2][1],(double)(0)) )
    {
        *err = ae_true;
    }
    if( ae_fp_neq(xy1.ptr.pp_double[2][2],(double)(0)) )
    {
        *err = ae_true;
    }
    if( ae_fp_neq(xy1.ptr.pp_double[2][3],(double)(0)) )
    {
        *err = ae_true;
    }
    ae_frame_leave(_state);
}


/*$ End $*/
