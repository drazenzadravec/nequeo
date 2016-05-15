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
#include "testhqrndunit.h"


/*$ Declarations $*/
static void testhqrndunit_calculatemv(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* mean,
     double* means,
     double* stddev,
     double* stddevs,
     ae_state *_state);
static void testhqrndunit_unsetstate(hqrndstate* state, ae_state *_state);


/*$ Body $*/


ae_bool testhqrnd(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_int_t samplesize;
    double sigmathreshold;
    ae_int_t passcount;
    ae_int_t n;
    ae_int_t i;
    ae_int_t k;
    ae_int_t pass;
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t i1;
    ae_int_t i2;
    double r1;
    double r2;
    ae_vector x;
    ae_vector bins;
    double mean;
    double means;
    double stddev;
    double stddevs;
    double lambdav;
    ae_bool seederrors;
    ae_bool urerrors;
    double ursigmaerr;
    ae_bool uierrors;
    double uisigmaerr;
    ae_bool normerrors;
    double normsigmaerr;
    ae_bool unit2errors;
    ae_bool experrors;
    double expsigmaerr;
    ae_bool discreteerr;
    ae_bool continuouserr;
    hqrndstate state;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&bins, 0, DT_INT, _state);
    _hqrndstate_init(&state, _state);

    waserrors = ae_false;
    sigmathreshold = (double)(7);
    samplesize = 100000;
    passcount = 50;
    seederrors = ae_false;
    urerrors = ae_false;
    uierrors = ae_false;
    normerrors = ae_false;
    experrors = ae_false;
    unit2errors = ae_false;
    ae_vector_set_length(&x, samplesize-1+1, _state);
    
    /*
     * Test seed errors
     */
    for(pass=1; pass<=passcount; pass++)
    {
        s1 = 1+ae_randominteger(32000, _state);
        s2 = 1+ae_randominteger(32000, _state);
        testhqrndunit_unsetstate(&state, _state);
        hqrndseed(s1, s2, &state, _state);
        i1 = hqrnduniformi(&state, 100, _state);
        testhqrndunit_unsetstate(&state, _state);
        hqrndseed(s1, s2, &state, _state);
        i2 = hqrnduniformi(&state, 100, _state);
        seederrors = seederrors||i1!=i2;
        testhqrndunit_unsetstate(&state, _state);
        hqrndseed(s1, s2, &state, _state);
        r1 = hqrnduniformr(&state, _state);
        testhqrndunit_unsetstate(&state, _state);
        hqrndseed(s1, s2, &state, _state);
        r2 = hqrnduniformr(&state, _state);
        seederrors = seederrors||ae_fp_neq(r1,r2);
    }
    
    /*
     * Test HQRNDRandomize() and real uniform generator
     */
    testhqrndunit_unsetstate(&state, _state);
    hqrndrandomize(&state, _state);
    ursigmaerr = (double)(0);
    for(i=0; i<=samplesize-1; i++)
    {
        x.ptr.p_double[i] = hqrnduniformr(&state, _state);
    }
    for(i=0; i<=samplesize-1; i++)
    {
        urerrors = (urerrors||ae_fp_less_eq(x.ptr.p_double[i],(double)(0)))||ae_fp_greater_eq(x.ptr.p_double[i],(double)(1));
    }
    testhqrndunit_calculatemv(&x, samplesize, &mean, &means, &stddev, &stddevs, _state);
    if( ae_fp_neq(means,(double)(0)) )
    {
        ursigmaerr = ae_maxreal(ursigmaerr, ae_fabs((mean-0.5)/means, _state), _state);
    }
    else
    {
        urerrors = ae_true;
    }
    if( ae_fp_neq(stddevs,(double)(0)) )
    {
        ursigmaerr = ae_maxreal(ursigmaerr, ae_fabs((stddev-ae_sqrt((double)1/(double)12, _state))/stddevs, _state), _state);
    }
    else
    {
        urerrors = ae_true;
    }
    urerrors = urerrors||ae_fp_greater(ursigmaerr,sigmathreshold);
    
    /*
     * Test HQRNDRandomize() and integer uniform
     */
    testhqrndunit_unsetstate(&state, _state);
    hqrndrandomize(&state, _state);
    uisigmaerr = (double)(0);
    for(n=2; n<=10; n++)
    {
        for(i=0; i<=samplesize-1; i++)
        {
            x.ptr.p_double[i] = (double)(hqrnduniformi(&state, n, _state));
        }
        for(i=0; i<=samplesize-1; i++)
        {
            uierrors = (uierrors||ae_fp_less(x.ptr.p_double[i],(double)(0)))||ae_fp_greater_eq(x.ptr.p_double[i],(double)(n));
        }
        testhqrndunit_calculatemv(&x, samplesize, &mean, &means, &stddev, &stddevs, _state);
        if( ae_fp_neq(means,(double)(0)) )
        {
            uisigmaerr = ae_maxreal(uisigmaerr, ae_fabs((mean-0.5*(n-1))/means, _state), _state);
        }
        else
        {
            uierrors = ae_true;
        }
        if( ae_fp_neq(stddevs,(double)(0)) )
        {
            uisigmaerr = ae_maxreal(uisigmaerr, ae_fabs((stddev-ae_sqrt((ae_sqr((double)(n), _state)-1)/12, _state))/stddevs, _state), _state);
        }
        else
        {
            uierrors = ae_true;
        }
    }
    uierrors = uierrors||ae_fp_greater(uisigmaerr,sigmathreshold);
    
    /*
     * Special 'close-to-limit' test on uniformity of integers
     * (straightforward implementation like 'RND mod N' will return
     *  non-uniform numbers for N=2/3*LIMIT)
     */
    testhqrndunit_unsetstate(&state, _state);
    hqrndrandomize(&state, _state);
    uisigmaerr = (double)(0);
    n = 1431655708;
    for(i=0; i<=samplesize-1; i++)
    {
        x.ptr.p_double[i] = (double)(hqrnduniformi(&state, n, _state));
    }
    for(i=0; i<=samplesize-1; i++)
    {
        uierrors = (uierrors||ae_fp_less(x.ptr.p_double[i],(double)(0)))||ae_fp_greater_eq(x.ptr.p_double[i],(double)(n));
    }
    testhqrndunit_calculatemv(&x, samplesize, &mean, &means, &stddev, &stddevs, _state);
    if( ae_fp_neq(means,(double)(0)) )
    {
        uisigmaerr = ae_maxreal(uisigmaerr, ae_fabs((mean-0.5*(n-1))/means, _state), _state);
    }
    else
    {
        uierrors = ae_true;
    }
    if( ae_fp_neq(stddevs,(double)(0)) )
    {
        uisigmaerr = ae_maxreal(uisigmaerr, ae_fabs((stddev-ae_sqrt((ae_sqr((double)(n), _state)-1)/12, _state))/stddevs, _state), _state);
    }
    else
    {
        uierrors = ae_true;
    }
    uierrors = uierrors||ae_fp_greater(uisigmaerr,sigmathreshold);
    
    /*
     * Test normal
     */
    testhqrndunit_unsetstate(&state, _state);
    hqrndrandomize(&state, _state);
    normsigmaerr = (double)(0);
    i = 0;
    while(i<samplesize)
    {
        hqrndnormal2(&state, &r1, &r2, _state);
        x.ptr.p_double[i] = r1;
        if( i+1<samplesize )
        {
            x.ptr.p_double[i+1] = r2;
        }
        i = i+2;
    }
    testhqrndunit_calculatemv(&x, samplesize, &mean, &means, &stddev, &stddevs, _state);
    if( ae_fp_neq(means,(double)(0)) )
    {
        normsigmaerr = ae_maxreal(normsigmaerr, ae_fabs((mean-0)/means, _state), _state);
    }
    else
    {
        normerrors = ae_true;
    }
    if( ae_fp_neq(stddevs,(double)(0)) )
    {
        normsigmaerr = ae_maxreal(normsigmaerr, ae_fabs((stddev-1)/stddevs, _state), _state);
    }
    else
    {
        normerrors = ae_true;
    }
    normerrors = normerrors||ae_fp_greater(normsigmaerr,sigmathreshold);
    
    /*
     * Test unit2
     */
    testhqrndunit_unsetstate(&state, _state);
    hqrndrandomize(&state, _state);
    n = 1000000;
    ae_vector_set_length(&bins, 10, _state);
    for(i=0; i<=bins.cnt-1; i++)
    {
        bins.ptr.p_int[i] = 0;
    }
    for(pass=0; pass<=n-1; pass++)
    {
        hqrndunit2(&state, &r1, &r2, _state);
        seterrorflag(&unit2errors, ae_fp_greater(ae_fabs(r1*r1+r2*r2-1, _state),100*ae_machineepsilon), _state);
        k = ae_ifloor((ae_atan2(r1, r2, _state)+ae_pi)/(2*ae_pi)*bins.cnt, _state);
        if( k<0 )
        {
            k = 0;
        }
        if( k>=bins.cnt )
        {
            k = bins.cnt-1;
        }
        bins.ptr.p_int[k] = bins.ptr.p_int[k]+1;
    }
    for(i=0; i<=bins.cnt-1; i++)
    {
        seterrorflag(&unit2errors, ae_fp_less((double)(bins.ptr.p_int[i]),0.9*n/bins.cnt)||ae_fp_greater((double)(bins.ptr.p_int[i]),1.1*n/bins.cnt), _state);
    }
    
    /*
     * Test exponential
     */
    testhqrndunit_unsetstate(&state, _state);
    hqrndrandomize(&state, _state);
    expsigmaerr = (double)(0);
    lambdav = 2+5*ae_randomreal(_state);
    for(i=0; i<=samplesize-1; i++)
    {
        x.ptr.p_double[i] = hqrndexponential(&state, lambdav, _state);
    }
    for(i=0; i<=samplesize-1; i++)
    {
        uierrors = uierrors||ae_fp_less(x.ptr.p_double[i],(double)(0));
    }
    testhqrndunit_calculatemv(&x, samplesize, &mean, &means, &stddev, &stddevs, _state);
    if( ae_fp_neq(means,(double)(0)) )
    {
        expsigmaerr = ae_maxreal(expsigmaerr, ae_fabs((mean-1.0/lambdav)/means, _state), _state);
    }
    else
    {
        experrors = ae_true;
    }
    if( ae_fp_neq(stddevs,(double)(0)) )
    {
        expsigmaerr = ae_maxreal(expsigmaerr, ae_fabs((stddev-1.0/lambdav)/stddevs, _state), _state);
    }
    else
    {
        experrors = ae_true;
    }
    experrors = experrors||ae_fp_greater(expsigmaerr,sigmathreshold);
    
    /*
     *Discrete/Continuous tests
     */
    discreteerr = hqrnddiscretetest(ae_true, _state);
    continuouserr = hqrndcontinuoustest(ae_true, _state);
    
    /*
     * Final report
     */
    waserrors = ((((((seederrors||urerrors)||uierrors)||normerrors)||unit2errors)||experrors)||discreteerr)||continuouserr;
    if( !silent )
    {
        printf("RNG TEST\n");
        printf("SEED TEST:                               ");
        if( !seederrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("UNIFORM CONTINUOUS:                      ");
        if( !urerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("UNIFORM INTEGER:                         ");
        if( !uierrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("NORMAL:                                  ");
        if( !normerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("UNIT2:                                   ");
        if( !unit2errors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("EXPONENTIAL:                             ");
        if( !experrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("DISCRETE:                                ");
        if( !discreteerr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("CONTINUOUS:                              ");
        if( !continuouserr )
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
ae_bool _pexec_testhqrnd(ae_bool silent, ae_state *_state)
{
    return testhqrnd(silent, _state);
}


/*************************************************************************
Function for test HQRNDContinuous function
*************************************************************************/
ae_bool hqrndcontinuoustest(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector sample;
    ae_vector bins;
    ae_vector binbounds;
    ae_int_t nb;
    ae_int_t samplesize;
    hqrndstate state;
    ae_int_t xp;
    ae_int_t i;
    ae_int_t j;
    double v;
    double sigma;
    double sigmamax;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&sample, 0, DT_REAL, _state);
    ae_vector_init(&bins, 0, DT_INT, _state);
    ae_vector_init(&binbounds, 0, DT_REAL, _state);
    _hqrndstate_init(&state, _state);

    result = ae_false;
    
    /*
     * Test for sample size equal to 1
     */
    ae_vector_set_length(&sample, 1, _state);
    sample.ptr.p_double[0] = ae_randomreal(_state);
    hqrndrandomize(&state, _state);
    result = result||ae_fp_neq(hqrndcontinuous(&state, &sample, 1, _state),sample.ptr.p_double[0]);
    
    /*
     * Test for larger samples
     */
    xp = 100000;
    sigmamax = 10.0;
    for(samplesize=2; samplesize<=5; samplesize++)
    {
        
        /*
         * 1. Generate random sample with SampleSize points
         * 2. Generate NB=3*(SampleSize-1) bins, with bounds as prescribed by (BinBounds[I],BinBounds[I+1]).
         *    Bin bounds are generated in such a way that value can fall into any bin with same probability
         * 3. Generate many random values
         * 4. Calculate number of values which fall into each bin
         * 5. Bins[I] should have binomial distribution with mean XP/NB and 
         *    variance XP*(1/NB)*(1-1/NB)
         */
        nb = 3*(samplesize-1);
        sigma = ae_sqrt(xp*((double)1/(double)nb)*(1-(double)1/(double)nb), _state);
        ae_vector_set_length(&sample, samplesize, _state);
        sample.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
        for(i=0; i<=samplesize-2; i++)
        {
            sample.ptr.p_double[i+1] = sample.ptr.p_double[i]+0.1+ae_randomreal(_state);
        }
        ae_vector_set_length(&bins, nb, _state);
        ae_vector_set_length(&binbounds, nb+1, _state);
        for(i=0; i<=samplesize-2; i++)
        {
            bins.ptr.p_int[3*i+0] = 0;
            bins.ptr.p_int[3*i+1] = 0;
            bins.ptr.p_int[3*i+2] = 0;
            binbounds.ptr.p_double[3*i+0] = sample.ptr.p_double[i];
            binbounds.ptr.p_double[3*i+1] = sample.ptr.p_double[i]+(sample.ptr.p_double[i+1]-sample.ptr.p_double[i])/3;
            binbounds.ptr.p_double[3*i+2] = sample.ptr.p_double[i]+(sample.ptr.p_double[i+1]-sample.ptr.p_double[i])*2/3;
        }
        binbounds.ptr.p_double[nb] = sample.ptr.p_double[samplesize-1];
        hqrndrandomize(&state, _state);
        for(i=0; i<=xp-1; i++)
        {
            v = hqrndcontinuous(&state, &sample, samplesize, _state);
            for(j=0; j<=nb-1; j++)
            {
                if( ae_fp_greater(v,binbounds.ptr.p_double[j])&&ae_fp_less(v,binbounds.ptr.p_double[j+1]) )
                {
                    bins.ptr.p_int[j] = bins.ptr.p_int[j]+1;
                    break;
                }
            }
        }
        for(i=0; i<=nb-1; i++)
        {
            result = result||ae_fp_greater(ae_fabs(bins.ptr.p_int[i]-(double)xp/(double)nb, _state),sigma*sigmamax);
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for test HQRNDContinuous function
*************************************************************************/
ae_bool hqrnddiscretetest(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector sample;
    double sigma;
    double sigmathreshold;
    double tsample;
    double max;
    double min;
    ae_int_t i;
    ae_int_t j;
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t binscount;
    ae_int_t xp;
    ae_vector nn;
    hqrndstate state;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&sample, 0, DT_REAL, _state);
    ae_vector_init(&nn, 0, DT_INT, _state);
    _hqrndstate_init(&state, _state);

    
    /*
     * We test that all values from discrete sample are generated with same probability.
     * To do this, we generate random values many times, then we calculate actual probabilities
     * and compare them with theoretical ones.
     */
    max = (double)(100);
    min = (double)(-100);
    xp = 100000;
    sigmathreshold = 10.0;
    for(binscount=1; binscount<=5; binscount++)
    {
        sigma = ae_sqrt(xp*((double)1/(double)binscount)*(1-(double)1/(double)binscount), _state);
        ae_vector_set_length(&nn, binscount, _state);
        for(i=0; i<=binscount-1; i++)
        {
            nn.ptr.p_int[i] = 0;
        }
        ae_vector_set_length(&sample, binscount, _state);
        sample.ptr.p_double[0] = (max-min)*ae_randomreal(_state)+min;
        for(i=1; i<=binscount-1; i++)
        {
            sample.ptr.p_double[i] = sample.ptr.p_double[i-1]+max*ae_randomreal(_state)+0.001;
        }
        s1 = 1+ae_randominteger(32000, _state);
        s2 = 1+ae_randominteger(32000, _state);
        hqrndseed(s1, s2, &state, _state);
        for(i=0; i<=xp-1; i++)
        {
            tsample = hqrnddiscrete(&state, &sample, binscount, _state);
            for(j=0; j<=binscount-1; j++)
            {
                if( ae_fp_eq(tsample,sample.ptr.p_double[j]) )
                {
                    nn.ptr.p_int[j] = nn.ptr.p_int[j]+1;
                    break;
                }
            }
        }
        for(i=0; i<=binscount-1; i++)
        {
            if( ae_fp_less((double)(nn.ptr.p_int[i]),(double)xp/(double)binscount-sigmathreshold*sigma)||ae_fp_greater((double)(nn.ptr.p_int[i]),(double)xp/(double)binscount+sigmathreshold*sigma) )
            {
                if( !silent )
                {
                    printf("HQRNDDiscreteTest::ErrorReport::\n");
                    printf("nn[%0d]=%0d;\n   xp/BinsCount=%0.5f;\n   C*sigma=%0.5f\n",
                        (int)(i),
                        (int)(nn.ptr.p_int[i]),
                        (double)((double)xp/(double)binscount),
                        (double)(sigmathreshold*sigma));
                    printf("HQRNDDiscreteTest: test is FAILED!\n");
                }
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
        if( !silent )
        {
            printf("HQRNDDiscreteTest: test is OK.\n");
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


static void testhqrndunit_calculatemv(/* Real    */ ae_vector* x,
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
        variance = (v1-v2)/(n-1);
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


/*************************************************************************
Unsets HQRNDState structure
*************************************************************************/
static void testhqrndunit_unsetstate(hqrndstate* state, ae_state *_state)
{


    state->s1 = 0;
    state->s2 = 0;
    state->magicv = 0;
}


/*$ End $*/
