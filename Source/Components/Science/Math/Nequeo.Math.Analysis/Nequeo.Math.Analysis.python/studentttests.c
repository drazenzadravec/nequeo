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


#include "stdafx.h"
#include "studentttests.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
One-sample t-test

This test checks three hypotheses about the mean of the given sample.  The
following tests are performed:
    * two-tailed test (null hypothesis - the mean is equal  to  the  given
      value)
    * left-tailed test (null hypothesis - the  mean  is  greater  than  or
      equal to the given value)
    * right-tailed test (null hypothesis - the mean is less than or  equal
      to the given value).

The test is based on the assumption that  a  given  sample  has  a  normal
distribution and  an  unknown  dispersion.  If  the  distribution  sharply
differs from normal, the test will work incorrectly.

INPUT PARAMETERS:
    X       -   sample. Array whose index goes from 0 to N-1.
    N       -   size of sample, N>=0
    Mean    -   assumed value of the mean.

OUTPUT PARAMETERS:
    BothTails   -   p-value for two-tailed test.
                    If BothTails is less than the given significance level
                    the null hypothesis is rejected.
    LeftTail    -   p-value for left-tailed test.
                    If LeftTail is less than the given significance level,
                    the null hypothesis is rejected.
    RightTail   -   p-value for right-tailed test.
                    If RightTail is less than the given significance level
                    the null hypothesis is rejected.

NOTE: this function correctly handles degenerate cases:
      * when N=0, all p-values are set to 1.0
      * when variance of X[] is exactly zero, p-values are set
        to 1.0 or 0.0, depending on difference between sample mean and
        value of mean being tested.


  -- ALGLIB --
     Copyright 08.09.2006 by Bochkanov Sergey
*************************************************************************/
void studentttest1(/* Real    */ ae_vector* x,
     ae_int_t n,
     double mean,
     double* bothtails,
     double* lefttail,
     double* righttail,
     ae_state *_state)
{
    ae_int_t i;
    double xmean;
    double x0;
    double v;
    ae_bool samex;
    double xvariance;
    double xstddev;
    double v1;
    double v2;
    double stat;
    double s;

    *bothtails = 0;
    *lefttail = 0;
    *righttail = 0;

    if( n<=0 )
    {
        *bothtails = 1.0;
        *lefttail = 1.0;
        *righttail = 1.0;
        return;
    }
    
    /*
     * Mean
     */
    xmean = (double)(0);
    x0 = x->ptr.p_double[0];
    samex = ae_true;
    for(i=0; i<=n-1; i++)
    {
        v = x->ptr.p_double[i];
        xmean = xmean+v;
        samex = samex&&ae_fp_eq(v,x0);
    }
    if( samex )
    {
        xmean = x0;
    }
    else
    {
        xmean = xmean/n;
    }
    
    /*
     * Variance (using corrected two-pass algorithm)
     */
    xvariance = (double)(0);
    xstddev = (double)(0);
    if( n!=1&&!samex )
    {
        v1 = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v1 = v1+ae_sqr(x->ptr.p_double[i]-xmean, _state);
        }
        v2 = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v2 = v2+(x->ptr.p_double[i]-xmean);
        }
        v2 = ae_sqr(v2, _state)/n;
        xvariance = (v1-v2)/(n-1);
        if( ae_fp_less(xvariance,(double)(0)) )
        {
            xvariance = (double)(0);
        }
        xstddev = ae_sqrt(xvariance, _state);
    }
    if( ae_fp_eq(xstddev,(double)(0)) )
    {
        if( ae_fp_eq(xmean,mean) )
        {
            *bothtails = 1.0;
        }
        else
        {
            *bothtails = 0.0;
        }
        if( ae_fp_greater_eq(xmean,mean) )
        {
            *lefttail = 1.0;
        }
        else
        {
            *lefttail = 0.0;
        }
        if( ae_fp_less_eq(xmean,mean) )
        {
            *righttail = 1.0;
        }
        else
        {
            *righttail = 0.0;
        }
        return;
    }
    
    /*
     * Statistic
     */
    stat = (xmean-mean)/(xstddev/ae_sqrt((double)(n), _state));
    s = studenttdistribution(n-1, stat, _state);
    *bothtails = 2*ae_minreal(s, 1-s, _state);
    *lefttail = s;
    *righttail = 1-s;
}


/*************************************************************************
Two-sample pooled test

This test checks three hypotheses about the mean of the given samples. The
following tests are performed:
    * two-tailed test (null hypothesis - the means are equal)
    * left-tailed test (null hypothesis - the mean of the first sample  is
      greater than or equal to the mean of the second sample)
    * right-tailed test (null hypothesis - the mean of the first sample is
      less than or equal to the mean of the second sample).

Test is based on the following assumptions:
    * given samples have normal distributions
    * dispersions are equal
    * samples are independent.

Input parameters:
    X       -   sample 1. Array whose index goes from 0 to N-1.
    N       -   size of sample.
    Y       -   sample 2. Array whose index goes from 0 to M-1.
    M       -   size of sample.

Output parameters:
    BothTails   -   p-value for two-tailed test.
                    If BothTails is less than the given significance level
                    the null hypothesis is rejected.
    LeftTail    -   p-value for left-tailed test.
                    If LeftTail is less than the given significance level,
                    the null hypothesis is rejected.
    RightTail   -   p-value for right-tailed test.
                    If RightTail is less than the given significance level
                    the null hypothesis is rejected.

NOTE: this function correctly handles degenerate cases:
      * when N=0 or M=0, all p-values are set to 1.0
      * when both samples has exactly zero variance, p-values are set
        to 1.0 or 0.0, depending on difference between means.

  -- ALGLIB --
     Copyright 18.09.2006 by Bochkanov Sergey
*************************************************************************/
void studentttest2(/* Real    */ ae_vector* x,
     ae_int_t n,
     /* Real    */ ae_vector* y,
     ae_int_t m,
     double* bothtails,
     double* lefttail,
     double* righttail,
     ae_state *_state)
{
    ae_int_t i;
    ae_bool samex;
    ae_bool samey;
    double x0;
    double y0;
    double xmean;
    double ymean;
    double v;
    double stat;
    double s;
    double p;

    *bothtails = 0;
    *lefttail = 0;
    *righttail = 0;

    if( n<=0||m<=0 )
    {
        *bothtails = 1.0;
        *lefttail = 1.0;
        *righttail = 1.0;
        return;
    }
    
    /*
     * Mean
     */
    xmean = (double)(0);
    x0 = x->ptr.p_double[0];
    samex = ae_true;
    for(i=0; i<=n-1; i++)
    {
        v = x->ptr.p_double[i];
        xmean = xmean+v;
        samex = samex&&ae_fp_eq(v,x0);
    }
    if( samex )
    {
        xmean = x0;
    }
    else
    {
        xmean = xmean/n;
    }
    ymean = (double)(0);
    y0 = y->ptr.p_double[0];
    samey = ae_true;
    for(i=0; i<=m-1; i++)
    {
        v = y->ptr.p_double[i];
        ymean = ymean+v;
        samey = samey&&ae_fp_eq(v,y0);
    }
    if( samey )
    {
        ymean = y0;
    }
    else
    {
        ymean = ymean/m;
    }
    
    /*
     * S
     */
    s = (double)(0);
    if( n+m>2 )
    {
        for(i=0; i<=n-1; i++)
        {
            s = s+ae_sqr(x->ptr.p_double[i]-xmean, _state);
        }
        for(i=0; i<=m-1; i++)
        {
            s = s+ae_sqr(y->ptr.p_double[i]-ymean, _state);
        }
        s = ae_sqrt(s*((double)1/(double)n+(double)1/(double)m)/(n+m-2), _state);
    }
    if( ae_fp_eq(s,(double)(0)) )
    {
        if( ae_fp_eq(xmean,ymean) )
        {
            *bothtails = 1.0;
        }
        else
        {
            *bothtails = 0.0;
        }
        if( ae_fp_greater_eq(xmean,ymean) )
        {
            *lefttail = 1.0;
        }
        else
        {
            *lefttail = 0.0;
        }
        if( ae_fp_less_eq(xmean,ymean) )
        {
            *righttail = 1.0;
        }
        else
        {
            *righttail = 0.0;
        }
        return;
    }
    
    /*
     * Statistic
     */
    stat = (xmean-ymean)/s;
    p = studenttdistribution(n+m-2, stat, _state);
    *bothtails = 2*ae_minreal(p, 1-p, _state);
    *lefttail = p;
    *righttail = 1-p;
}


/*************************************************************************
Two-sample unpooled test

This test checks three hypotheses about the mean of the given samples. The
following tests are performed:
    * two-tailed test (null hypothesis - the means are equal)
    * left-tailed test (null hypothesis - the mean of the first sample  is
      greater than or equal to the mean of the second sample)
    * right-tailed test (null hypothesis - the mean of the first sample is
      less than or equal to the mean of the second sample).

Test is based on the following assumptions:
    * given samples have normal distributions
    * samples are independent.
Equality of variances is NOT required.

Input parameters:
    X - sample 1. Array whose index goes from 0 to N-1.
    N - size of the sample.
    Y - sample 2. Array whose index goes from 0 to M-1.
    M - size of the sample.

Output parameters:
    BothTails   -   p-value for two-tailed test.
                    If BothTails is less than the given significance level
                    the null hypothesis is rejected.
    LeftTail    -   p-value for left-tailed test.
                    If LeftTail is less than the given significance level,
                    the null hypothesis is rejected.
    RightTail   -   p-value for right-tailed test.
                    If RightTail is less than the given significance level
                    the null hypothesis is rejected.

NOTE: this function correctly handles degenerate cases:
      * when N=0 or M=0, all p-values are set to 1.0
      * when both samples has zero variance, p-values are set
        to 1.0 or 0.0, depending on difference between means.
      * when only one sample has zero variance, test reduces to 1-sample
        version.

  -- ALGLIB --
     Copyright 18.09.2006 by Bochkanov Sergey
*************************************************************************/
void unequalvariancettest(/* Real    */ ae_vector* x,
     ae_int_t n,
     /* Real    */ ae_vector* y,
     ae_int_t m,
     double* bothtails,
     double* lefttail,
     double* righttail,
     ae_state *_state)
{
    ae_int_t i;
    ae_bool samex;
    ae_bool samey;
    double x0;
    double y0;
    double xmean;
    double ymean;
    double xvar;
    double yvar;
    double v;
    double df;
    double p;
    double stat;
    double c;

    *bothtails = 0;
    *lefttail = 0;
    *righttail = 0;

    if( n<=0||m<=0 )
    {
        *bothtails = 1.0;
        *lefttail = 1.0;
        *righttail = 1.0;
        return;
    }
    
    /*
     * Mean
     */
    xmean = (double)(0);
    x0 = x->ptr.p_double[0];
    samex = ae_true;
    for(i=0; i<=n-1; i++)
    {
        v = x->ptr.p_double[i];
        xmean = xmean+v;
        samex = samex&&ae_fp_eq(v,x0);
    }
    if( samex )
    {
        xmean = x0;
    }
    else
    {
        xmean = xmean/n;
    }
    ymean = (double)(0);
    y0 = y->ptr.p_double[0];
    samey = ae_true;
    for(i=0; i<=m-1; i++)
    {
        v = y->ptr.p_double[i];
        ymean = ymean+v;
        samey = samey&&ae_fp_eq(v,y0);
    }
    if( samey )
    {
        ymean = y0;
    }
    else
    {
        ymean = ymean/m;
    }
    
    /*
     * Variance (using corrected two-pass algorithm)
     */
    xvar = (double)(0);
    if( n>=2&&!samex )
    {
        for(i=0; i<=n-1; i++)
        {
            xvar = xvar+ae_sqr(x->ptr.p_double[i]-xmean, _state);
        }
        xvar = xvar/(n-1);
    }
    yvar = (double)(0);
    if( m>=2&&!samey )
    {
        for(i=0; i<=m-1; i++)
        {
            yvar = yvar+ae_sqr(y->ptr.p_double[i]-ymean, _state);
        }
        yvar = yvar/(m-1);
    }
    
    /*
     * Handle different special cases
     * (one or both variances are zero).
     */
    if( ae_fp_eq(xvar,(double)(0))&&ae_fp_eq(yvar,(double)(0)) )
    {
        if( ae_fp_eq(xmean,ymean) )
        {
            *bothtails = 1.0;
        }
        else
        {
            *bothtails = 0.0;
        }
        if( ae_fp_greater_eq(xmean,ymean) )
        {
            *lefttail = 1.0;
        }
        else
        {
            *lefttail = 0.0;
        }
        if( ae_fp_less_eq(xmean,ymean) )
        {
            *righttail = 1.0;
        }
        else
        {
            *righttail = 0.0;
        }
        return;
    }
    if( ae_fp_eq(xvar,(double)(0)) )
    {
        
        /*
         * X is constant, unpooled 2-sample test reduces to 1-sample test.
         *
         * NOTE: right-tail and left-tail must be passed to 1-sample
         *       t-test in reverse order because we reverse order of
         *       of samples.
         */
        studentttest1(y, m, xmean, bothtails, righttail, lefttail, _state);
        return;
    }
    if( ae_fp_eq(yvar,(double)(0)) )
    {
        
        /*
         * Y is constant, unpooled 2-sample test reduces to 1-sample test.
         */
        studentttest1(x, n, ymean, bothtails, lefttail, righttail, _state);
        return;
    }
    
    /*
     * Statistic
     */
    stat = (xmean-ymean)/ae_sqrt(xvar/n+yvar/m, _state);
    c = xvar/n/(xvar/n+yvar/m);
    df = (n-1)*(m-1)/((m-1)*ae_sqr(c, _state)+(n-1)*ae_sqr(1-c, _state));
    if( ae_fp_greater(stat,(double)(0)) )
    {
        p = 1-0.5*incompletebeta(df/2, 0.5, df/(df+ae_sqr(stat, _state)), _state);
    }
    else
    {
        p = 0.5*incompletebeta(df/2, 0.5, df/(df+ae_sqr(stat, _state)), _state);
    }
    *bothtails = 2*ae_minreal(p, 1-p, _state);
    *lefttail = p;
    *righttail = 1-p;
}


/*$ End $*/
