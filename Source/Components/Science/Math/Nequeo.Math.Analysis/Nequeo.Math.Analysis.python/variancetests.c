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
#include "variancetests.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Two-sample F-test

This test checks three hypotheses about dispersions of the given  samples.
The following tests are performed:
    * two-tailed test (null hypothesis - the dispersions are equal)
    * left-tailed test (null hypothesis  -  the  dispersion  of  the first
      sample is greater than or equal to  the  dispersion  of  the  second
      sample).
    * right-tailed test (null hypothesis - the  dispersion  of  the  first
      sample is less than or equal to the dispersion of the second sample)

The test is based on the following assumptions:
    * the given samples have normal distributions
    * the samples are independent.

Input parameters:
    X   -   sample 1. Array whose index goes from 0 to N-1.
    N   -   sample size.
    Y   -   sample 2. Array whose index goes from 0 to M-1.
    M   -   sample size.

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

  -- ALGLIB --
     Copyright 19.09.2006 by Bochkanov Sergey
*************************************************************************/
void ftest(/* Real    */ ae_vector* x,
     ae_int_t n,
     /* Real    */ ae_vector* y,
     ae_int_t m,
     double* bothtails,
     double* lefttail,
     double* righttail,
     ae_state *_state)
{
    ae_int_t i;
    double xmean;
    double ymean;
    double xvar;
    double yvar;
    ae_int_t df1;
    ae_int_t df2;
    double stat;

    *bothtails = 0;
    *lefttail = 0;
    *righttail = 0;

    if( n<=2||m<=2 )
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
    for(i=0; i<=n-1; i++)
    {
        xmean = xmean+x->ptr.p_double[i];
    }
    xmean = xmean/n;
    ymean = (double)(0);
    for(i=0; i<=m-1; i++)
    {
        ymean = ymean+y->ptr.p_double[i];
    }
    ymean = ymean/m;
    
    /*
     * Variance (using corrected two-pass algorithm)
     */
    xvar = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        xvar = xvar+ae_sqr(x->ptr.p_double[i]-xmean, _state);
    }
    xvar = xvar/(n-1);
    yvar = (double)(0);
    for(i=0; i<=m-1; i++)
    {
        yvar = yvar+ae_sqr(y->ptr.p_double[i]-ymean, _state);
    }
    yvar = yvar/(m-1);
    if( ae_fp_eq(xvar,(double)(0))||ae_fp_eq(yvar,(double)(0)) )
    {
        *bothtails = 1.0;
        *lefttail = 1.0;
        *righttail = 1.0;
        return;
    }
    
    /*
     * Statistic
     */
    df1 = n-1;
    df2 = m-1;
    stat = ae_minreal(xvar/yvar, yvar/xvar, _state);
    *bothtails = 1-(fdistribution(df1, df2, 1/stat, _state)-fdistribution(df1, df2, stat, _state));
    *lefttail = fdistribution(df1, df2, xvar/yvar, _state);
    *righttail = 1-(*lefttail);
}


/*************************************************************************
One-sample chi-square test

This test checks three hypotheses about the dispersion of the given sample
The following tests are performed:
    * two-tailed test (null hypothesis - the dispersion equals  the  given
      number)
    * left-tailed test (null hypothesis - the dispersion is  greater  than
      or equal to the given number)
    * right-tailed test (null hypothesis  -  dispersion is  less  than  or
      equal to the given number).

Test is based on the following assumptions:
    * the given sample has a normal distribution.

Input parameters:
    X           -   sample 1. Array whose index goes from 0 to N-1.
    N           -   size of the sample.
    Variance    -   dispersion value to compare with.

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

  -- ALGLIB --
     Copyright 19.09.2006 by Bochkanov Sergey
*************************************************************************/
void onesamplevariancetest(/* Real    */ ae_vector* x,
     ae_int_t n,
     double variance,
     double* bothtails,
     double* lefttail,
     double* righttail,
     ae_state *_state)
{
    ae_int_t i;
    double xmean;
    double xvar;
    double s;
    double stat;

    *bothtails = 0;
    *lefttail = 0;
    *righttail = 0;

    if( n<=1 )
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
    for(i=0; i<=n-1; i++)
    {
        xmean = xmean+x->ptr.p_double[i];
    }
    xmean = xmean/n;
    
    /*
     * Variance
     */
    xvar = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        xvar = xvar+ae_sqr(x->ptr.p_double[i]-xmean, _state);
    }
    xvar = xvar/(n-1);
    if( ae_fp_eq(xvar,(double)(0)) )
    {
        *bothtails = 1.0;
        *lefttail = 1.0;
        *righttail = 1.0;
        return;
    }
    
    /*
     * Statistic
     */
    stat = (n-1)*xvar/variance;
    s = chisquaredistribution((double)(n-1), stat, _state);
    *bothtails = 2*ae_minreal(s, 1-s, _state);
    *lefttail = s;
    *righttail = 1-(*lefttail);
}


/*$ End $*/
