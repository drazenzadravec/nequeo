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
#include "basestat.h"


/*$ Declarations $*/
static void basestat_rankdatarec(/* Real    */ ae_matrix* xy,
     ae_int_t i0,
     ae_int_t i1,
     ae_int_t nfeatures,
     ae_bool iscentered,
     ae_shared_pool* pool,
     ae_int_t basecasecost,
     ae_state *_state);
static void basestat_rankdatabasecase(/* Real    */ ae_matrix* xy,
     ae_int_t i0,
     ae_int_t i1,
     ae_int_t nfeatures,
     ae_bool iscentered,
     apbuffers* buf0,
     apbuffers* buf1,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Calculation of the distribution moments: mean, variance, skewness, kurtosis.

INPUT PARAMETERS:
    X       -   sample
    N       -   N>=0, sample size:
                * if given, only leading N elements of X are processed
                * if not given, automatically determined from size of X
    
OUTPUT PARAMETERS
    Mean    -   mean.
    Variance-   variance.
    Skewness-   skewness (if variance<>0; zero otherwise).
    Kurtosis-   kurtosis (if variance<>0; zero otherwise).

NOTE: variance is calculated by dividing sum of squares by N-1, not N.

  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
void samplemoments(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* mean,
     double* variance,
     double* skewness,
     double* kurtosis,
     ae_state *_state)
{
    ae_int_t i;
    double v;
    double v1;
    double v2;
    double stddev;

    *mean = 0;
    *variance = 0;
    *skewness = 0;
    *kurtosis = 0;

    ae_assert(n>=0, "SampleMoments: N<0", _state);
    ae_assert(x->cnt>=n, "SampleMoments: Length(X)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "SampleMoments: X is not finite vector", _state);
    
    /*
     * Init, special case 'N=0'
     */
    *mean = (double)(0);
    *variance = (double)(0);
    *skewness = (double)(0);
    *kurtosis = (double)(0);
    stddev = (double)(0);
    if( n<=0 )
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
        *variance = (v1-v2)/(n-1);
        if( ae_fp_less(*variance,(double)(0)) )
        {
            *variance = (double)(0);
        }
        stddev = ae_sqrt(*variance, _state);
    }
    
    /*
     * Skewness and kurtosis
     */
    if( ae_fp_neq(stddev,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            v = (x->ptr.p_double[i]-(*mean))/stddev;
            v2 = ae_sqr(v, _state);
            *skewness = *skewness+v2*v;
            *kurtosis = *kurtosis+ae_sqr(v2, _state);
        }
        *skewness = *skewness/n;
        *kurtosis = *kurtosis/n-3;
    }
}


/*************************************************************************
Calculation of the mean.

INPUT PARAMETERS:
    X       -   sample
    N       -   N>=0, sample size:
                * if given, only leading N elements of X are processed
                * if not given, automatically determined from size of X

NOTE:
                
This function return result  which calculated by 'SampleMoments' function
and stored at 'Mean' variable.


  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
double samplemean(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state)
{
    double mean;
    double tmp0;
    double tmp1;
    double tmp2;
    double result;


    samplemoments(x, n, &mean, &tmp0, &tmp1, &tmp2, _state);
    result = mean;
    return result;
}


/*************************************************************************
Calculation of the variance.

INPUT PARAMETERS:
    X       -   sample
    N       -   N>=0, sample size:
                * if given, only leading N elements of X are processed
                * if not given, automatically determined from size of X

NOTE:
                
This function return result  which calculated by 'SampleMoments' function
and stored at 'Variance' variable.


  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
double samplevariance(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state)
{
    double variance;
    double tmp0;
    double tmp1;
    double tmp2;
    double result;


    samplemoments(x, n, &tmp0, &variance, &tmp1, &tmp2, _state);
    result = variance;
    return result;
}


/*************************************************************************
Calculation of the skewness.

INPUT PARAMETERS:
    X       -   sample
    N       -   N>=0, sample size:
                * if given, only leading N elements of X are processed
                * if not given, automatically determined from size of X

NOTE:
                
This function return result  which calculated by 'SampleMoments' function
and stored at 'Skewness' variable.


  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
double sampleskewness(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state)
{
    double skewness;
    double tmp0;
    double tmp1;
    double tmp2;
    double result;


    samplemoments(x, n, &tmp0, &tmp1, &skewness, &tmp2, _state);
    result = skewness;
    return result;
}


/*************************************************************************
Calculation of the kurtosis.

INPUT PARAMETERS:
    X       -   sample
    N       -   N>=0, sample size:
                * if given, only leading N elements of X are processed
                * if not given, automatically determined from size of X

NOTE:
                
This function return result  which calculated by 'SampleMoments' function
and stored at 'Kurtosis' variable.


  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
double samplekurtosis(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state)
{
    double kurtosis;
    double tmp0;
    double tmp1;
    double tmp2;
    double result;


    samplemoments(x, n, &tmp0, &tmp1, &tmp2, &kurtosis, _state);
    result = kurtosis;
    return result;
}


/*************************************************************************
ADev

Input parameters:
    X   -   sample
    N   -   N>=0, sample size:
            * if given, only leading N elements of X are processed
            * if not given, automatically determined from size of X
    
Output parameters:
    ADev-   ADev

  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
void sampleadev(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* adev,
     ae_state *_state)
{
    ae_int_t i;
    double mean;

    *adev = 0;

    ae_assert(n>=0, "SampleADev: N<0", _state);
    ae_assert(x->cnt>=n, "SampleADev: Length(X)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "SampleADev: X is not finite vector", _state);
    
    /*
     * Init, handle N=0
     */
    mean = (double)(0);
    *adev = (double)(0);
    if( n<=0 )
    {
        return;
    }
    
    /*
     * Mean
     */
    for(i=0; i<=n-1; i++)
    {
        mean = mean+x->ptr.p_double[i];
    }
    mean = mean/n;
    
    /*
     * ADev
     */
    for(i=0; i<=n-1; i++)
    {
        *adev = *adev+ae_fabs(x->ptr.p_double[i]-mean, _state);
    }
    *adev = *adev/n;
}


/*************************************************************************
Median calculation.

Input parameters:
    X   -   sample (array indexes: [0..N-1])
    N   -   N>=0, sample size:
            * if given, only leading N elements of X are processed
            * if not given, automatically determined from size of X

Output parameters:
    Median

  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
void samplemedian(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* median,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_int_t i;
    ae_int_t ir;
    ae_int_t j;
    ae_int_t l;
    ae_int_t midp;
    ae_int_t k;
    double a;
    double tval;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    *median = 0;

    ae_assert(n>=0, "SampleMedian: N<0", _state);
    ae_assert(x->cnt>=n, "SampleMedian: Length(X)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "SampleMedian: X is not finite vector", _state);
    
    /*
     * Some degenerate cases
     */
    *median = (double)(0);
    if( n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    if( n==1 )
    {
        *median = x->ptr.p_double[0];
        ae_frame_leave(_state);
        return;
    }
    if( n==2 )
    {
        *median = 0.5*(x->ptr.p_double[0]+x->ptr.p_double[1]);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Common case, N>=3.
     * Choose X[(N-1)/2]
     */
    l = 0;
    ir = n-1;
    k = (n-1)/2;
    for(;;)
    {
        if( ir<=l+1 )
        {
            
            /*
             * 1 or 2 elements in partition
             */
            if( ir==l+1&&ae_fp_less(x->ptr.p_double[ir],x->ptr.p_double[l]) )
            {
                tval = x->ptr.p_double[l];
                x->ptr.p_double[l] = x->ptr.p_double[ir];
                x->ptr.p_double[ir] = tval;
            }
            break;
        }
        else
        {
            midp = (l+ir)/2;
            tval = x->ptr.p_double[midp];
            x->ptr.p_double[midp] = x->ptr.p_double[l+1];
            x->ptr.p_double[l+1] = tval;
            if( ae_fp_greater(x->ptr.p_double[l],x->ptr.p_double[ir]) )
            {
                tval = x->ptr.p_double[l];
                x->ptr.p_double[l] = x->ptr.p_double[ir];
                x->ptr.p_double[ir] = tval;
            }
            if( ae_fp_greater(x->ptr.p_double[l+1],x->ptr.p_double[ir]) )
            {
                tval = x->ptr.p_double[l+1];
                x->ptr.p_double[l+1] = x->ptr.p_double[ir];
                x->ptr.p_double[ir] = tval;
            }
            if( ae_fp_greater(x->ptr.p_double[l],x->ptr.p_double[l+1]) )
            {
                tval = x->ptr.p_double[l];
                x->ptr.p_double[l] = x->ptr.p_double[l+1];
                x->ptr.p_double[l+1] = tval;
            }
            i = l+1;
            j = ir;
            a = x->ptr.p_double[l+1];
            for(;;)
            {
                do
                {
                    i = i+1;
                }
                while(ae_fp_less(x->ptr.p_double[i],a));
                do
                {
                    j = j-1;
                }
                while(ae_fp_greater(x->ptr.p_double[j],a));
                if( j<i )
                {
                    break;
                }
                tval = x->ptr.p_double[i];
                x->ptr.p_double[i] = x->ptr.p_double[j];
                x->ptr.p_double[j] = tval;
            }
            x->ptr.p_double[l+1] = x->ptr.p_double[j];
            x->ptr.p_double[j] = a;
            if( j>=k )
            {
                ir = j-1;
            }
            if( j<=k )
            {
                l = i;
            }
        }
    }
    
    /*
     * If N is odd, return result
     */
    if( n%2==1 )
    {
        *median = x->ptr.p_double[k];
        ae_frame_leave(_state);
        return;
    }
    a = x->ptr.p_double[n-1];
    for(i=k+1; i<=n-1; i++)
    {
        if( ae_fp_less(x->ptr.p_double[i],a) )
        {
            a = x->ptr.p_double[i];
        }
    }
    *median = 0.5*(x->ptr.p_double[k]+a);
    ae_frame_leave(_state);
}


/*************************************************************************
Percentile calculation.

Input parameters:
    X   -   sample (array indexes: [0..N-1])
    N   -   N>=0, sample size:
            * if given, only leading N elements of X are processed
            * if not given, automatically determined from size of X
    P   -   percentile (0<=P<=1)

Output parameters:
    V   -   percentile

  -- ALGLIB --
     Copyright 01.03.2008 by Bochkanov Sergey
*************************************************************************/
void samplepercentile(/* Real    */ ae_vector* x,
     ae_int_t n,
     double p,
     double* v,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_int_t i1;
    double t;
    ae_vector rbuf;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    *v = 0;
    ae_vector_init(&rbuf, 0, DT_REAL, _state);

    ae_assert(n>=0, "SamplePercentile: N<0", _state);
    ae_assert(x->cnt>=n, "SamplePercentile: Length(X)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "SamplePercentile: X is not finite vector", _state);
    ae_assert(ae_isfinite(p, _state), "SamplePercentile: incorrect P!", _state);
    ae_assert(ae_fp_greater_eq(p,(double)(0))&&ae_fp_less_eq(p,(double)(1)), "SamplePercentile: incorrect P!", _state);
    tagsortfast(x, &rbuf, n, _state);
    if( ae_fp_eq(p,(double)(0)) )
    {
        *v = x->ptr.p_double[0];
        ae_frame_leave(_state);
        return;
    }
    if( ae_fp_eq(p,(double)(1)) )
    {
        *v = x->ptr.p_double[n-1];
        ae_frame_leave(_state);
        return;
    }
    t = p*(n-1);
    i1 = ae_ifloor(t, _state);
    t = t-ae_ifloor(t, _state);
    *v = x->ptr.p_double[i1]*(1-t)+x->ptr.p_double[i1+1]*t;
    ae_frame_leave(_state);
}


/*************************************************************************
2-sample covariance

Input parameters:
    X       -   sample 1 (array indexes: [0..N-1])
    Y       -   sample 2 (array indexes: [0..N-1])
    N       -   N>=0, sample size:
                * if given, only N leading elements of X/Y are processed
                * if not given, automatically determined from input sizes

Result:
    covariance (zero for N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
double cov2(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    double xmean;
    double ymean;
    double v;
    double x0;
    double y0;
    double s;
    ae_bool samex;
    ae_bool samey;
    double result;


    ae_assert(n>=0, "Cov2: N<0", _state);
    ae_assert(x->cnt>=n, "Cov2: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Cov2: Length(Y)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "Cov2: X is not finite vector", _state);
    ae_assert(isfinitevector(y, n, _state), "Cov2: Y is not finite vector", _state);
    
    /*
     * Special case
     */
    if( n<=1 )
    {
        result = (double)(0);
        return result;
    }
    
    /*
     * Calculate mean.
     *
     *
     * Additonally we calculate SameX and SameY -
     * flag variables which are set to True when
     * all X[] (or Y[]) contain exactly same value.
     *
     * If at least one of them is True, we return zero
     * (othwerwise we risk to get nonzero covariation
     * because of roundoff).
     */
    xmean = (double)(0);
    ymean = (double)(0);
    samex = ae_true;
    samey = ae_true;
    x0 = x->ptr.p_double[0];
    y0 = y->ptr.p_double[0];
    v = (double)1/(double)n;
    for(i=0; i<=n-1; i++)
    {
        s = x->ptr.p_double[i];
        samex = samex&&ae_fp_eq(s,x0);
        xmean = xmean+s*v;
        s = y->ptr.p_double[i];
        samey = samey&&ae_fp_eq(s,y0);
        ymean = ymean+s*v;
    }
    if( samex||samey )
    {
        result = (double)(0);
        return result;
    }
    
    /*
     * covariance
     */
    v = (double)1/(double)(n-1);
    result = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        result = result+v*(x->ptr.p_double[i]-xmean)*(y->ptr.p_double[i]-ymean);
    }
    return result;
}


/*************************************************************************
Pearson product-moment correlation coefficient

Input parameters:
    X       -   sample 1 (array indexes: [0..N-1])
    Y       -   sample 2 (array indexes: [0..N-1])
    N       -   N>=0, sample size:
                * if given, only N leading elements of X/Y are processed
                * if not given, automatically determined from input sizes

Result:
    Pearson product-moment correlation coefficient
    (zero for N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
double pearsoncorr2(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    double xmean;
    double ymean;
    double v;
    double x0;
    double y0;
    double s;
    ae_bool samex;
    ae_bool samey;
    double xv;
    double yv;
    double t1;
    double t2;
    double result;


    ae_assert(n>=0, "PearsonCorr2: N<0", _state);
    ae_assert(x->cnt>=n, "PearsonCorr2: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "PearsonCorr2: Length(Y)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "PearsonCorr2: X is not finite vector", _state);
    ae_assert(isfinitevector(y, n, _state), "PearsonCorr2: Y is not finite vector", _state);
    
    /*
     * Special case
     */
    if( n<=1 )
    {
        result = (double)(0);
        return result;
    }
    
    /*
     * Calculate mean.
     *
     *
     * Additonally we calculate SameX and SameY -
     * flag variables which are set to True when
     * all X[] (or Y[]) contain exactly same value.
     *
     * If at least one of them is True, we return zero
     * (othwerwise we risk to get nonzero correlation
     * because of roundoff).
     */
    xmean = (double)(0);
    ymean = (double)(0);
    samex = ae_true;
    samey = ae_true;
    x0 = x->ptr.p_double[0];
    y0 = y->ptr.p_double[0];
    v = (double)1/(double)n;
    for(i=0; i<=n-1; i++)
    {
        s = x->ptr.p_double[i];
        samex = samex&&ae_fp_eq(s,x0);
        xmean = xmean+s*v;
        s = y->ptr.p_double[i];
        samey = samey&&ae_fp_eq(s,y0);
        ymean = ymean+s*v;
    }
    if( samex||samey )
    {
        result = (double)(0);
        return result;
    }
    
    /*
     * numerator and denominator
     */
    s = (double)(0);
    xv = (double)(0);
    yv = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        t1 = x->ptr.p_double[i]-xmean;
        t2 = y->ptr.p_double[i]-ymean;
        xv = xv+ae_sqr(t1, _state);
        yv = yv+ae_sqr(t2, _state);
        s = s+t1*t2;
    }
    if( ae_fp_eq(xv,(double)(0))||ae_fp_eq(yv,(double)(0)) )
    {
        result = (double)(0);
    }
    else
    {
        result = s/(ae_sqrt(xv, _state)*ae_sqrt(yv, _state));
    }
    return result;
}


/*************************************************************************
Spearman's rank correlation coefficient

Input parameters:
    X       -   sample 1 (array indexes: [0..N-1])
    Y       -   sample 2 (array indexes: [0..N-1])
    N       -   N>=0, sample size:
                * if given, only N leading elements of X/Y are processed
                * if not given, automatically determined from input sizes

Result:
    Spearman's rank correlation coefficient
    (zero for N=0 or N=1)

  -- ALGLIB --
     Copyright 09.04.2007 by Bochkanov Sergey
*************************************************************************/
double spearmancorr2(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    apbuffers buf;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    _apbuffers_init(&buf, _state);

    ae_assert(n>=0, "SpearmanCorr2: N<0", _state);
    ae_assert(x->cnt>=n, "SpearmanCorr2: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "SpearmanCorr2: Length(Y)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "SpearmanCorr2: X is not finite vector", _state);
    ae_assert(isfinitevector(y, n, _state), "SpearmanCorr2: Y is not finite vector", _state);
    
    /*
     * Special case
     */
    if( n<=1 )
    {
        result = (double)(0);
        ae_frame_leave(_state);
        return result;
    }
    rankx(x, n, ae_false, &buf, _state);
    rankx(y, n, ae_false, &buf, _state);
    result = pearsoncorr2(x, y, n, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Covariance matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with covariance matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X are used
            * if not given, automatically determined from input size
    M   -   M>0, number of variables:
            * if given, only leading M columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M,M], covariance matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void covm(/* Real    */ ae_matrix* x,
     ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_matrix* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _x;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_vector t;
    ae_vector x0;
    ae_vector same;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_x, x, _state);
    x = &_x;
    ae_matrix_clear(c);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&same, 0, DT_BOOL, _state);

    ae_assert(n>=0, "CovM: N<0", _state);
    ae_assert(m>=1, "CovM: M<1", _state);
    ae_assert(x->rows>=n, "CovM: Rows(X)<N!", _state);
    ae_assert(x->cols>=m||n==0, "CovM: Cols(X)<M!", _state);
    ae_assert(apservisfinitematrix(x, n, m, _state), "CovM: X contains infinite/NAN elements", _state);
    
    /*
     * N<=1, return zero
     */
    if( n<=1 )
    {
        ae_matrix_set_length(c, m, m, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                c->ptr.pp_double[i][j] = (double)(0);
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Calculate means,
     * check for constant columns
     */
    ae_vector_set_length(&t, m, _state);
    ae_vector_set_length(&x0, m, _state);
    ae_vector_set_length(&same, m, _state);
    ae_matrix_set_length(c, m, m, _state);
    for(i=0; i<=m-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
        same.ptr.p_bool[i] = ae_true;
    }
    ae_v_move(&x0.ptr.p_double[0], 1, &x->ptr.pp_double[0][0], 1, ae_v_len(0,m-1));
    v = (double)1/(double)n;
    for(i=0; i<=n-1; i++)
    {
        ae_v_addd(&t.ptr.p_double[0], 1, &x->ptr.pp_double[i][0], 1, ae_v_len(0,m-1), v);
        for(j=0; j<=m-1; j++)
        {
            same.ptr.p_bool[j] = same.ptr.p_bool[j]&&ae_fp_eq(x->ptr.pp_double[i][j],x0.ptr.p_double[j]);
        }
    }
    
    /*
     * * center variables;
     * * if we have constant columns, these columns are
     *   artificially zeroed (they must be zero in exact arithmetics,
     *   but unfortunately floating point ops are not exact).
     * * calculate upper half of symmetric covariance matrix
     */
    for(i=0; i<=n-1; i++)
    {
        ae_v_sub(&x->ptr.pp_double[i][0], 1, &t.ptr.p_double[0], 1, ae_v_len(0,m-1));
        for(j=0; j<=m-1; j++)
        {
            if( same.ptr.p_bool[j] )
            {
                x->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    rmatrixsyrk(m, n, (double)1/(double)(n-1), x, 0, 0, 1, 0.0, c, 0, 0, ae_true, _state);
    rmatrixenforcesymmetricity(c, m, ae_true, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_covm(/* Real    */ ae_matrix* x,
    ae_int_t n,
    ae_int_t m,
    /* Real    */ ae_matrix* c, ae_state *_state)
{
    covm(x,n,m,c, _state);
}


/*************************************************************************
Pearson product-moment correlation matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with correlation matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X are used
            * if not given, automatically determined from input size
    M   -   M>0, number of variables:
            * if given, only leading M columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M,M], correlation matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void pearsoncorrm(/* Real    */ ae_matrix* x,
     ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_matrix* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector t;
    ae_int_t i;
    ae_int_t j;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(c);
    ae_vector_init(&t, 0, DT_REAL, _state);

    ae_assert(n>=0, "PearsonCorrM: N<0", _state);
    ae_assert(m>=1, "PearsonCorrM: M<1", _state);
    ae_assert(x->rows>=n, "PearsonCorrM: Rows(X)<N!", _state);
    ae_assert(x->cols>=m||n==0, "PearsonCorrM: Cols(X)<M!", _state);
    ae_assert(apservisfinitematrix(x, n, m, _state), "PearsonCorrM: X contains infinite/NAN elements", _state);
    ae_vector_set_length(&t, m, _state);
    covm(x, n, m, c, _state);
    for(i=0; i<=m-1; i++)
    {
        if( ae_fp_greater(c->ptr.pp_double[i][i],(double)(0)) )
        {
            t.ptr.p_double[i] = 1/ae_sqrt(c->ptr.pp_double[i][i], _state);
        }
        else
        {
            t.ptr.p_double[i] = 0.0;
        }
    }
    for(i=0; i<=m-1; i++)
    {
        v = t.ptr.p_double[i];
        for(j=0; j<=m-1; j++)
        {
            c->ptr.pp_double[i][j] = c->ptr.pp_double[i][j]*v*t.ptr.p_double[j];
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_pearsoncorrm(/* Real    */ ae_matrix* x,
    ae_int_t n,
    ae_int_t m,
    /* Real    */ ae_matrix* c, ae_state *_state)
{
    pearsoncorrm(x,n,m,c, _state);
}


/*************************************************************************
Spearman's rank correlation matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with correlation matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X are used
            * if not given, automatically determined from input size
    M   -   M>0, number of variables:
            * if given, only leading M columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M,M], correlation matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void spearmancorrm(/* Real    */ ae_matrix* x,
     ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_matrix* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    apbuffers buf;
    ae_matrix xc;
    ae_vector t;
    double v;
    double vv;
    double x0;
    ae_bool b;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(c);
    _apbuffers_init(&buf, _state);
    ae_matrix_init(&xc, 0, 0, DT_REAL, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);

    ae_assert(n>=0, "SpearmanCorrM: N<0", _state);
    ae_assert(m>=1, "SpearmanCorrM: M<1", _state);
    ae_assert(x->rows>=n, "SpearmanCorrM: Rows(X)<N!", _state);
    ae_assert(x->cols>=m||n==0, "SpearmanCorrM: Cols(X)<M!", _state);
    ae_assert(apservisfinitematrix(x, n, m, _state), "SpearmanCorrM: X contains infinite/NAN elements", _state);
    
    /*
     * N<=1, return zero
     */
    if( n<=1 )
    {
        ae_matrix_set_length(c, m, m, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                c->ptr.pp_double[i][j] = (double)(0);
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Allocate
     */
    ae_vector_set_length(&t, ae_maxint(n, m, _state), _state);
    ae_matrix_set_length(c, m, m, _state);
    
    /*
     * Replace data with ranks
     */
    ae_matrix_set_length(&xc, m, n, _state);
    rmatrixtranspose(n, m, x, 0, 0, &xc, 0, 0, _state);
    rankdata(&xc, m, n, _state);
    
    /*
     * 1. Calculate means, check for constant columns
     * 2. Center variables, constant  columns are
     *   artificialy zeroed (they must be zero in exact arithmetics,
     *   but unfortunately floating point is not exact).
     */
    for(i=0; i<=m-1; i++)
    {
        
        /*
         * Calculate:
         * * V - mean value of I-th variable
         * * B - True in case all variable values are same
         */
        v = (double)(0);
        b = ae_true;
        x0 = xc.ptr.pp_double[i][0];
        for(j=0; j<=n-1; j++)
        {
            vv = xc.ptr.pp_double[i][j];
            v = v+vv;
            b = b&&ae_fp_eq(vv,x0);
        }
        v = v/n;
        
        /*
         * Center/zero I-th variable
         */
        if( b )
        {
            
            /*
             * Zero
             */
            for(j=0; j<=n-1; j++)
            {
                xc.ptr.pp_double[i][j] = 0.0;
            }
        }
        else
        {
            
            /*
             * Center
             */
            for(j=0; j<=n-1; j++)
            {
                xc.ptr.pp_double[i][j] = xc.ptr.pp_double[i][j]-v;
            }
        }
    }
    
    /*
     * Calculate upper half of symmetric covariance matrix
     */
    rmatrixsyrk(m, n, (double)1/(double)(n-1), &xc, 0, 0, 0, 0.0, c, 0, 0, ae_true, _state);
    
    /*
     * Calculate Pearson coefficients (upper triangle)
     */
    for(i=0; i<=m-1; i++)
    {
        if( ae_fp_greater(c->ptr.pp_double[i][i],(double)(0)) )
        {
            t.ptr.p_double[i] = 1/ae_sqrt(c->ptr.pp_double[i][i], _state);
        }
        else
        {
            t.ptr.p_double[i] = 0.0;
        }
    }
    for(i=0; i<=m-1; i++)
    {
        v = t.ptr.p_double[i];
        for(j=i; j<=m-1; j++)
        {
            c->ptr.pp_double[i][j] = c->ptr.pp_double[i][j]*v*t.ptr.p_double[j];
        }
    }
    
    /*
     * force symmetricity
     */
    rmatrixenforcesymmetricity(c, m, ae_true, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spearmancorrm(/* Real    */ ae_matrix* x,
    ae_int_t n,
    ae_int_t m,
    /* Real    */ ae_matrix* c, ae_state *_state)
{
    spearmancorrm(x,n,m,c, _state);
}


/*************************************************************************
Cross-covariance matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with covariance matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M1], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    Y   -   array[N,M2], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X/Y are used
            * if not given, automatically determined from input sizes
    M1  -   M1>0, number of variables in X:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size
    M2  -   M2>0, number of variables in Y:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M1,M2], cross-covariance matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void covm2(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     ae_int_t n,
     ae_int_t m1,
     ae_int_t m2,
     /* Real    */ ae_matrix* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _x;
    ae_matrix _y;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_vector t;
    ae_vector x0;
    ae_vector y0;
    ae_vector samex;
    ae_vector samey;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_x, x, _state);
    x = &_x;
    ae_matrix_init_copy(&_y, y, _state);
    y = &_y;
    ae_matrix_clear(c);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&y0, 0, DT_REAL, _state);
    ae_vector_init(&samex, 0, DT_BOOL, _state);
    ae_vector_init(&samey, 0, DT_BOOL, _state);

    ae_assert(n>=0, "CovM2: N<0", _state);
    ae_assert(m1>=1, "CovM2: M1<1", _state);
    ae_assert(m2>=1, "CovM2: M2<1", _state);
    ae_assert(x->rows>=n, "CovM2: Rows(X)<N!", _state);
    ae_assert(x->cols>=m1||n==0, "CovM2: Cols(X)<M1!", _state);
    ae_assert(apservisfinitematrix(x, n, m1, _state), "CovM2: X contains infinite/NAN elements", _state);
    ae_assert(y->rows>=n, "CovM2: Rows(Y)<N!", _state);
    ae_assert(y->cols>=m2||n==0, "CovM2: Cols(Y)<M2!", _state);
    ae_assert(apservisfinitematrix(y, n, m2, _state), "CovM2: X contains infinite/NAN elements", _state);
    
    /*
     * N<=1, return zero
     */
    if( n<=1 )
    {
        ae_matrix_set_length(c, m1, m2, _state);
        for(i=0; i<=m1-1; i++)
        {
            for(j=0; j<=m2-1; j++)
            {
                c->ptr.pp_double[i][j] = (double)(0);
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Allocate
     */
    ae_vector_set_length(&t, ae_maxint(m1, m2, _state), _state);
    ae_vector_set_length(&x0, m1, _state);
    ae_vector_set_length(&y0, m2, _state);
    ae_vector_set_length(&samex, m1, _state);
    ae_vector_set_length(&samey, m2, _state);
    ae_matrix_set_length(c, m1, m2, _state);
    
    /*
     * * calculate means of X
     * * center X
     * * if we have constant columns, these columns are
     *   artificially zeroed (they must be zero in exact arithmetics,
     *   but unfortunately floating point ops are not exact).
     */
    for(i=0; i<=m1-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
        samex.ptr.p_bool[i] = ae_true;
    }
    ae_v_move(&x0.ptr.p_double[0], 1, &x->ptr.pp_double[0][0], 1, ae_v_len(0,m1-1));
    v = (double)1/(double)n;
    for(i=0; i<=n-1; i++)
    {
        ae_v_addd(&t.ptr.p_double[0], 1, &x->ptr.pp_double[i][0], 1, ae_v_len(0,m1-1), v);
        for(j=0; j<=m1-1; j++)
        {
            samex.ptr.p_bool[j] = samex.ptr.p_bool[j]&&ae_fp_eq(x->ptr.pp_double[i][j],x0.ptr.p_double[j]);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        ae_v_sub(&x->ptr.pp_double[i][0], 1, &t.ptr.p_double[0], 1, ae_v_len(0,m1-1));
        for(j=0; j<=m1-1; j++)
        {
            if( samex.ptr.p_bool[j] )
            {
                x->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    
    /*
     * Repeat same steps for Y
     */
    for(i=0; i<=m2-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
        samey.ptr.p_bool[i] = ae_true;
    }
    ae_v_move(&y0.ptr.p_double[0], 1, &y->ptr.pp_double[0][0], 1, ae_v_len(0,m2-1));
    v = (double)1/(double)n;
    for(i=0; i<=n-1; i++)
    {
        ae_v_addd(&t.ptr.p_double[0], 1, &y->ptr.pp_double[i][0], 1, ae_v_len(0,m2-1), v);
        for(j=0; j<=m2-1; j++)
        {
            samey.ptr.p_bool[j] = samey.ptr.p_bool[j]&&ae_fp_eq(y->ptr.pp_double[i][j],y0.ptr.p_double[j]);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        ae_v_sub(&y->ptr.pp_double[i][0], 1, &t.ptr.p_double[0], 1, ae_v_len(0,m2-1));
        for(j=0; j<=m2-1; j++)
        {
            if( samey.ptr.p_bool[j] )
            {
                y->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    
    /*
     * calculate cross-covariance matrix
     */
    rmatrixgemm(m1, m2, n, (double)1/(double)(n-1), x, 0, 0, 1, y, 0, 0, 0, 0.0, c, 0, 0, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_covm2(/* Real    */ ae_matrix* x,
    /* Real    */ ae_matrix* y,
    ae_int_t n,
    ae_int_t m1,
    ae_int_t m2,
    /* Real    */ ae_matrix* c, ae_state *_state)
{
    covm2(x,y,n,m1,m2,c, _state);
}


/*************************************************************************
Pearson product-moment cross-correlation matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with correlation matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M1], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    Y   -   array[N,M2], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X/Y are used
            * if not given, automatically determined from input sizes
    M1  -   M1>0, number of variables in X:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size
    M2  -   M2>0, number of variables in Y:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M1,M2], cross-correlation matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void pearsoncorrm2(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     ae_int_t n,
     ae_int_t m1,
     ae_int_t m2,
     /* Real    */ ae_matrix* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _x;
    ae_matrix _y;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_vector t;
    ae_vector x0;
    ae_vector y0;
    ae_vector sx;
    ae_vector sy;
    ae_vector samex;
    ae_vector samey;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_x, x, _state);
    x = &_x;
    ae_matrix_init_copy(&_y, y, _state);
    y = &_y;
    ae_matrix_clear(c);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&y0, 0, DT_REAL, _state);
    ae_vector_init(&sx, 0, DT_REAL, _state);
    ae_vector_init(&sy, 0, DT_REAL, _state);
    ae_vector_init(&samex, 0, DT_BOOL, _state);
    ae_vector_init(&samey, 0, DT_BOOL, _state);

    ae_assert(n>=0, "PearsonCorrM2: N<0", _state);
    ae_assert(m1>=1, "PearsonCorrM2: M1<1", _state);
    ae_assert(m2>=1, "PearsonCorrM2: M2<1", _state);
    ae_assert(x->rows>=n, "PearsonCorrM2: Rows(X)<N!", _state);
    ae_assert(x->cols>=m1||n==0, "PearsonCorrM2: Cols(X)<M1!", _state);
    ae_assert(apservisfinitematrix(x, n, m1, _state), "PearsonCorrM2: X contains infinite/NAN elements", _state);
    ae_assert(y->rows>=n, "PearsonCorrM2: Rows(Y)<N!", _state);
    ae_assert(y->cols>=m2||n==0, "PearsonCorrM2: Cols(Y)<M2!", _state);
    ae_assert(apservisfinitematrix(y, n, m2, _state), "PearsonCorrM2: X contains infinite/NAN elements", _state);
    
    /*
     * N<=1, return zero
     */
    if( n<=1 )
    {
        ae_matrix_set_length(c, m1, m2, _state);
        for(i=0; i<=m1-1; i++)
        {
            for(j=0; j<=m2-1; j++)
            {
                c->ptr.pp_double[i][j] = (double)(0);
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Allocate
     */
    ae_vector_set_length(&t, ae_maxint(m1, m2, _state), _state);
    ae_vector_set_length(&x0, m1, _state);
    ae_vector_set_length(&y0, m2, _state);
    ae_vector_set_length(&sx, m1, _state);
    ae_vector_set_length(&sy, m2, _state);
    ae_vector_set_length(&samex, m1, _state);
    ae_vector_set_length(&samey, m2, _state);
    ae_matrix_set_length(c, m1, m2, _state);
    
    /*
     * * calculate means of X
     * * center X
     * * if we have constant columns, these columns are
     *   artificially zeroed (they must be zero in exact arithmetics,
     *   but unfortunately floating point ops are not exact).
     * * calculate column variances
     */
    for(i=0; i<=m1-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
        samex.ptr.p_bool[i] = ae_true;
        sx.ptr.p_double[i] = (double)(0);
    }
    ae_v_move(&x0.ptr.p_double[0], 1, &x->ptr.pp_double[0][0], 1, ae_v_len(0,m1-1));
    v = (double)1/(double)n;
    for(i=0; i<=n-1; i++)
    {
        ae_v_addd(&t.ptr.p_double[0], 1, &x->ptr.pp_double[i][0], 1, ae_v_len(0,m1-1), v);
        for(j=0; j<=m1-1; j++)
        {
            samex.ptr.p_bool[j] = samex.ptr.p_bool[j]&&ae_fp_eq(x->ptr.pp_double[i][j],x0.ptr.p_double[j]);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        ae_v_sub(&x->ptr.pp_double[i][0], 1, &t.ptr.p_double[0], 1, ae_v_len(0,m1-1));
        for(j=0; j<=m1-1; j++)
        {
            if( samex.ptr.p_bool[j] )
            {
                x->ptr.pp_double[i][j] = (double)(0);
            }
            sx.ptr.p_double[j] = sx.ptr.p_double[j]+x->ptr.pp_double[i][j]*x->ptr.pp_double[i][j];
        }
    }
    for(j=0; j<=m1-1; j++)
    {
        sx.ptr.p_double[j] = ae_sqrt(sx.ptr.p_double[j]/(n-1), _state);
    }
    
    /*
     * Repeat same steps for Y
     */
    for(i=0; i<=m2-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
        samey.ptr.p_bool[i] = ae_true;
        sy.ptr.p_double[i] = (double)(0);
    }
    ae_v_move(&y0.ptr.p_double[0], 1, &y->ptr.pp_double[0][0], 1, ae_v_len(0,m2-1));
    v = (double)1/(double)n;
    for(i=0; i<=n-1; i++)
    {
        ae_v_addd(&t.ptr.p_double[0], 1, &y->ptr.pp_double[i][0], 1, ae_v_len(0,m2-1), v);
        for(j=0; j<=m2-1; j++)
        {
            samey.ptr.p_bool[j] = samey.ptr.p_bool[j]&&ae_fp_eq(y->ptr.pp_double[i][j],y0.ptr.p_double[j]);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        ae_v_sub(&y->ptr.pp_double[i][0], 1, &t.ptr.p_double[0], 1, ae_v_len(0,m2-1));
        for(j=0; j<=m2-1; j++)
        {
            if( samey.ptr.p_bool[j] )
            {
                y->ptr.pp_double[i][j] = (double)(0);
            }
            sy.ptr.p_double[j] = sy.ptr.p_double[j]+y->ptr.pp_double[i][j]*y->ptr.pp_double[i][j];
        }
    }
    for(j=0; j<=m2-1; j++)
    {
        sy.ptr.p_double[j] = ae_sqrt(sy.ptr.p_double[j]/(n-1), _state);
    }
    
    /*
     * calculate cross-covariance matrix
     */
    rmatrixgemm(m1, m2, n, (double)1/(double)(n-1), x, 0, 0, 1, y, 0, 0, 0, 0.0, c, 0, 0, _state);
    
    /*
     * Divide by standard deviations
     */
    for(i=0; i<=m1-1; i++)
    {
        if( ae_fp_neq(sx.ptr.p_double[i],(double)(0)) )
        {
            sx.ptr.p_double[i] = 1/sx.ptr.p_double[i];
        }
        else
        {
            sx.ptr.p_double[i] = 0.0;
        }
    }
    for(i=0; i<=m2-1; i++)
    {
        if( ae_fp_neq(sy.ptr.p_double[i],(double)(0)) )
        {
            sy.ptr.p_double[i] = 1/sy.ptr.p_double[i];
        }
        else
        {
            sy.ptr.p_double[i] = 0.0;
        }
    }
    for(i=0; i<=m1-1; i++)
    {
        v = sx.ptr.p_double[i];
        for(j=0; j<=m2-1; j++)
        {
            c->ptr.pp_double[i][j] = c->ptr.pp_double[i][j]*v*sy.ptr.p_double[j];
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_pearsoncorrm2(/* Real    */ ae_matrix* x,
    /* Real    */ ae_matrix* y,
    ae_int_t n,
    ae_int_t m1,
    ae_int_t m2,
    /* Real    */ ae_matrix* c, ae_state *_state)
{
    pearsoncorrm2(x,y,n,m1,m2,c, _state);
}


/*************************************************************************
Spearman's rank cross-correlation matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with correlation matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M1], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    Y   -   array[N,M2], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X/Y are used
            * if not given, automatically determined from input sizes
    M1  -   M1>0, number of variables in X:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size
    M2  -   M2>0, number of variables in Y:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M1,M2], cross-correlation matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void spearmancorrm2(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     ae_int_t n,
     ae_int_t m1,
     ae_int_t m2,
     /* Real    */ ae_matrix* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double v;
    double v2;
    double vv;
    ae_bool b;
    ae_vector t;
    double x0;
    double y0;
    ae_vector sx;
    ae_vector sy;
    ae_matrix xc;
    ae_matrix yc;
    apbuffers buf;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(c);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&sx, 0, DT_REAL, _state);
    ae_vector_init(&sy, 0, DT_REAL, _state);
    ae_matrix_init(&xc, 0, 0, DT_REAL, _state);
    ae_matrix_init(&yc, 0, 0, DT_REAL, _state);
    _apbuffers_init(&buf, _state);

    ae_assert(n>=0, "SpearmanCorrM2: N<0", _state);
    ae_assert(m1>=1, "SpearmanCorrM2: M1<1", _state);
    ae_assert(m2>=1, "SpearmanCorrM2: M2<1", _state);
    ae_assert(x->rows>=n, "SpearmanCorrM2: Rows(X)<N!", _state);
    ae_assert(x->cols>=m1||n==0, "SpearmanCorrM2: Cols(X)<M1!", _state);
    ae_assert(apservisfinitematrix(x, n, m1, _state), "SpearmanCorrM2: X contains infinite/NAN elements", _state);
    ae_assert(y->rows>=n, "SpearmanCorrM2: Rows(Y)<N!", _state);
    ae_assert(y->cols>=m2||n==0, "SpearmanCorrM2: Cols(Y)<M2!", _state);
    ae_assert(apservisfinitematrix(y, n, m2, _state), "SpearmanCorrM2: X contains infinite/NAN elements", _state);
    
    /*
     * N<=1, return zero
     */
    if( n<=1 )
    {
        ae_matrix_set_length(c, m1, m2, _state);
        for(i=0; i<=m1-1; i++)
        {
            for(j=0; j<=m2-1; j++)
            {
                c->ptr.pp_double[i][j] = (double)(0);
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Allocate
     */
    ae_vector_set_length(&t, ae_maxint(ae_maxint(m1, m2, _state), n, _state), _state);
    ae_vector_set_length(&sx, m1, _state);
    ae_vector_set_length(&sy, m2, _state);
    ae_matrix_set_length(c, m1, m2, _state);
    
    /*
     * Replace data with ranks
     */
    ae_matrix_set_length(&xc, m1, n, _state);
    ae_matrix_set_length(&yc, m2, n, _state);
    rmatrixtranspose(n, m1, x, 0, 0, &xc, 0, 0, _state);
    rmatrixtranspose(n, m2, y, 0, 0, &yc, 0, 0, _state);
    rankdata(&xc, m1, n, _state);
    rankdata(&yc, m2, n, _state);
    
    /*
     * 1. Calculate means, variances, check for constant columns
     * 2. Center variables, constant  columns are
     *   artificialy zeroed (they must be zero in exact arithmetics,
     *   but unfortunately floating point is not exact).
     *
     * Description of variables:
     * * V - mean value of I-th variable
     * * V2- variance
     * * VV-temporary
     * * B - True in case all variable values are same
     */
    for(i=0; i<=m1-1; i++)
    {
        v = (double)(0);
        v2 = 0.0;
        b = ae_true;
        x0 = xc.ptr.pp_double[i][0];
        for(j=0; j<=n-1; j++)
        {
            vv = xc.ptr.pp_double[i][j];
            v = v+vv;
            b = b&&ae_fp_eq(vv,x0);
        }
        v = v/n;
        if( b )
        {
            for(j=0; j<=n-1; j++)
            {
                xc.ptr.pp_double[i][j] = 0.0;
            }
        }
        else
        {
            for(j=0; j<=n-1; j++)
            {
                vv = xc.ptr.pp_double[i][j];
                xc.ptr.pp_double[i][j] = vv-v;
                v2 = v2+(vv-v)*(vv-v);
            }
        }
        sx.ptr.p_double[i] = ae_sqrt(v2/(n-1), _state);
    }
    for(i=0; i<=m2-1; i++)
    {
        v = (double)(0);
        v2 = 0.0;
        b = ae_true;
        y0 = yc.ptr.pp_double[i][0];
        for(j=0; j<=n-1; j++)
        {
            vv = yc.ptr.pp_double[i][j];
            v = v+vv;
            b = b&&ae_fp_eq(vv,y0);
        }
        v = v/n;
        if( b )
        {
            for(j=0; j<=n-1; j++)
            {
                yc.ptr.pp_double[i][j] = 0.0;
            }
        }
        else
        {
            for(j=0; j<=n-1; j++)
            {
                vv = yc.ptr.pp_double[i][j];
                yc.ptr.pp_double[i][j] = vv-v;
                v2 = v2+(vv-v)*(vv-v);
            }
        }
        sy.ptr.p_double[i] = ae_sqrt(v2/(n-1), _state);
    }
    
    /*
     * calculate cross-covariance matrix
     */
    rmatrixgemm(m1, m2, n, (double)1/(double)(n-1), &xc, 0, 0, 0, &yc, 0, 0, 1, 0.0, c, 0, 0, _state);
    
    /*
     * Divide by standard deviations
     */
    for(i=0; i<=m1-1; i++)
    {
        if( ae_fp_neq(sx.ptr.p_double[i],(double)(0)) )
        {
            sx.ptr.p_double[i] = 1/sx.ptr.p_double[i];
        }
        else
        {
            sx.ptr.p_double[i] = 0.0;
        }
    }
    for(i=0; i<=m2-1; i++)
    {
        if( ae_fp_neq(sy.ptr.p_double[i],(double)(0)) )
        {
            sy.ptr.p_double[i] = 1/sy.ptr.p_double[i];
        }
        else
        {
            sy.ptr.p_double[i] = 0.0;
        }
    }
    for(i=0; i<=m1-1; i++)
    {
        v = sx.ptr.p_double[i];
        for(j=0; j<=m2-1; j++)
        {
            c->ptr.pp_double[i][j] = c->ptr.pp_double[i][j]*v*sy.ptr.p_double[j];
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spearmancorrm2(/* Real    */ ae_matrix* x,
    /* Real    */ ae_matrix* y,
    ae_int_t n,
    ae_int_t m1,
    ae_int_t m2,
    /* Real    */ ae_matrix* c, ae_state *_state)
{
    spearmancorrm2(x,y,n,m1,m2,c, _state);
}


/*************************************************************************
This function replaces data in XY by their ranks:
* XY is processed row-by-row
* rows are processed separately
* tied data are correctly handled (tied ranks are calculated)
* ranking starts from 0, ends at NFeatures-1
* sum of within-row values is equal to (NFeatures-1)*NFeatures/2

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! ones where expected operations count is less than 100.000

INPUT PARAMETERS:
    XY      -   array[NPoints,NFeatures], dataset
    NPoints -   number of points
    NFeatures-  number of features

OUTPUT PARAMETERS:
    XY      -   data are replaced by their within-row ranks;
                ranking starts from 0, ends at NFeatures-1

  -- ALGLIB --
     Copyright 18.04.2013 by Bochkanov Sergey
*************************************************************************/
void rankdata(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nfeatures,
     ae_state *_state)
{
    ae_frame _frame_block;
    apbuffers buf0;
    apbuffers buf1;
    ae_int_t basecasecost;
    ae_shared_pool pool;

    ae_frame_make(_state, &_frame_block);
    _apbuffers_init(&buf0, _state);
    _apbuffers_init(&buf1, _state);
    ae_shared_pool_init(&pool, _state);

    ae_assert(npoints>=0, "RankData: NPoints<0", _state);
    ae_assert(nfeatures>=1, "RankData: NFeatures<1", _state);
    ae_assert(xy->rows>=npoints, "RankData: Rows(XY)<NPoints", _state);
    ae_assert(xy->cols>=nfeatures||npoints==0, "RankData: Cols(XY)<NFeatures", _state);
    ae_assert(apservisfinitematrix(xy, npoints, nfeatures, _state), "RankData: XY contains infinite/NAN elements", _state);
    
    /*
     * Basecase cost is a maximum cost of basecase problems.
     * Problems harded than that cost will be split.
     *
     * Problem cost is assumed to be NPoints*NFeatures*log2(NFeatures),
     * which is proportional, but NOT equal to number of FLOPs required
     * to solve problem.
     */
    basecasecost = 10000;
    
    /*
     * Try to use serial code, no SMP functionality, no shared pools.
     */
    if( ae_fp_less(inttoreal(npoints, _state)*inttoreal(nfeatures, _state)*logbase2((double)(nfeatures), _state),(double)(basecasecost)) )
    {
        basestat_rankdatabasecase(xy, 0, npoints, nfeatures, ae_false, &buf0, &buf1, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Parallel code
     */
    ae_shared_pool_set_seed(&pool, &buf0, sizeof(buf0), _apbuffers_init, _apbuffers_init_copy, _apbuffers_destroy, _state);
    basestat_rankdatarec(xy, 0, npoints, nfeatures, ae_false, &pool, basecasecost, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rankdata(/* Real    */ ae_matrix* xy,
    ae_int_t npoints,
    ae_int_t nfeatures, ae_state *_state)
{
    rankdata(xy,npoints,nfeatures, _state);
}


/*************************************************************************
This function replaces data in XY by their CENTERED ranks:
* XY is processed row-by-row
* rows are processed separately
* tied data are correctly handled (tied ranks are calculated)
* centered ranks are just usual ranks, but centered in such way  that  sum
  of within-row values is equal to 0.0.
* centering is performed by subtracting mean from each row, i.e it changes
  mean value, but does NOT change higher moments

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! ones where expected operations count is less than 100.000

INPUT PARAMETERS:
    XY      -   array[NPoints,NFeatures], dataset
    NPoints -   number of points
    NFeatures-  number of features

OUTPUT PARAMETERS:
    XY      -   data are replaced by their within-row ranks;
                ranking starts from 0, ends at NFeatures-1

  -- ALGLIB --
     Copyright 18.04.2013 by Bochkanov Sergey
*************************************************************************/
void rankdatacentered(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nfeatures,
     ae_state *_state)
{
    ae_frame _frame_block;
    apbuffers buf0;
    apbuffers buf1;
    ae_int_t basecasecost;
    ae_shared_pool pool;

    ae_frame_make(_state, &_frame_block);
    _apbuffers_init(&buf0, _state);
    _apbuffers_init(&buf1, _state);
    ae_shared_pool_init(&pool, _state);

    ae_assert(npoints>=0, "RankData: NPoints<0", _state);
    ae_assert(nfeatures>=1, "RankData: NFeatures<1", _state);
    ae_assert(xy->rows>=npoints, "RankData: Rows(XY)<NPoints", _state);
    ae_assert(xy->cols>=nfeatures||npoints==0, "RankData: Cols(XY)<NFeatures", _state);
    ae_assert(apservisfinitematrix(xy, npoints, nfeatures, _state), "RankData: XY contains infinite/NAN elements", _state);
    
    /*
     * Basecase cost is a maximum cost of basecase problems.
     * Problems harded than that cost will be split.
     *
     * Problem cost is assumed to be NPoints*NFeatures*log2(NFeatures),
     * which is proportional, but NOT equal to number of FLOPs required
     * to solve problem.
     */
    basecasecost = 10000;
    
    /*
     * Try to use serial code, no SMP functionality, no shared pools.
     */
    if( ae_fp_less(inttoreal(npoints, _state)*inttoreal(nfeatures, _state)*logbase2((double)(nfeatures), _state),(double)(basecasecost)) )
    {
        basestat_rankdatabasecase(xy, 0, npoints, nfeatures, ae_true, &buf0, &buf1, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Parallel code
     */
    ae_shared_pool_set_seed(&pool, &buf0, sizeof(buf0), _apbuffers_init, _apbuffers_init_copy, _apbuffers_destroy, _state);
    basestat_rankdatarec(xy, 0, npoints, nfeatures, ae_true, &pool, basecasecost, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rankdatacentered(/* Real    */ ae_matrix* xy,
    ae_int_t npoints,
    ae_int_t nfeatures, ae_state *_state)
{
    rankdatacentered(xy,npoints,nfeatures, _state);
}


/*************************************************************************
Obsolete function, we recommend to use PearsonCorr2().

  -- ALGLIB --
     Copyright 09.04.2007 by Bochkanov Sergey
*************************************************************************/
double pearsoncorrelation(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state)
{
    double result;


    result = pearsoncorr2(x, y, n, _state);
    return result;
}


/*************************************************************************
Obsolete function, we recommend to use SpearmanCorr2().

    -- ALGLIB --
    Copyright 09.04.2007 by Bochkanov Sergey
*************************************************************************/
double spearmanrankcorrelation(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state)
{
    double result;


    result = spearmancorr2(x, y, n, _state);
    return result;
}


/*************************************************************************
Recurrent code for RankData(), splits problem into  subproblems  or  calls
basecase code (depending on problem complexity).

INPUT PARAMETERS:
    XY      -   array[NPoints,NFeatures], dataset
    I0      -   index of first row to process
    I1      -   index of past-the-last row to process;
                this function processes half-interval [I0,I1).
    NFeatures-  number of features
    IsCentered- whether ranks are centered or not:
                * True      -   ranks are centered in such way that  their
                                within-row sum is zero
                * False     -   ranks are not centered
    Pool    -   shared pool which holds temporary buffers
                (APBuffers structure)
    BasecaseCost-minimum cost of the problem which will be split

OUTPUT PARAMETERS:
    XY      -   data in [I0,I1) are replaced by their within-row ranks;
                ranking starts from 0, ends at NFeatures-1

  -- ALGLIB --
     Copyright 18.04.2013 by Bochkanov Sergey
*************************************************************************/
static void basestat_rankdatarec(/* Real    */ ae_matrix* xy,
     ae_int_t i0,
     ae_int_t i1,
     ae_int_t nfeatures,
     ae_bool iscentered,
     ae_shared_pool* pool,
     ae_int_t basecasecost,
     ae_state *_state)
{
    ae_frame _frame_block;
    apbuffers *buf0;
    ae_smart_ptr _buf0;
    apbuffers *buf1;
    ae_smart_ptr _buf1;
    double problemcost;
    ae_int_t im;

    ae_frame_make(_state, &_frame_block);
    ae_smart_ptr_init(&_buf0, (void**)&buf0, _state);
    ae_smart_ptr_init(&_buf1, (void**)&buf1, _state);

    ae_assert(i1>=i0, "RankDataRec: internal error", _state);
    
    /*
     * Recursively split problem, if it is too large
     */
    problemcost = inttoreal(i1-i0, _state)*inttoreal(nfeatures, _state)*logbase2((double)(nfeatures), _state);
    if( i1-i0>=2&&ae_fp_greater(problemcost,(double)(basecasecost)) )
    {
        im = (i1+i0)/2;
        basestat_rankdatarec(xy, i0, im, nfeatures, iscentered, pool, basecasecost, _state);
        basestat_rankdatarec(xy, im, i1, nfeatures, iscentered, pool, basecasecost, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Retrieve buffers from pool, call serial code, return buffers to pool
     */
    ae_shared_pool_retrieve(pool, &_buf0, _state);
    ae_shared_pool_retrieve(pool, &_buf1, _state);
    basestat_rankdatabasecase(xy, i0, i1, nfeatures, iscentered, buf0, buf1, _state);
    ae_shared_pool_recycle(pool, &_buf0, _state);
    ae_shared_pool_recycle(pool, &_buf1, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Basecase code for RankData(), performs actual work on subset of data using
temporary buffer passed as parameter.

INPUT PARAMETERS:
    XY      -   array[NPoints,NFeatures], dataset
    I0      -   index of first row to process
    I1      -   index of past-the-last row to process;
                this function processes half-interval [I0,I1).
    NFeatures-  number of features
    IsCentered- whether ranks are centered or not:
                * True      -   ranks are centered in such way that  their
                                within-row sum is zero
                * False     -   ranks are not centered
    Buf0    -   temporary buffers, may be empty (this function automatically
                allocates/reuses buffers).
    Buf1    -   temporary buffers, may be empty (this function automatically
                allocates/reuses buffers).

OUTPUT PARAMETERS:
    XY      -   data in [I0,I1) are replaced by their within-row ranks;
                ranking starts from 0, ends at NFeatures-1

  -- ALGLIB --
     Copyright 18.04.2013 by Bochkanov Sergey
*************************************************************************/
static void basestat_rankdatabasecase(/* Real    */ ae_matrix* xy,
     ae_int_t i0,
     ae_int_t i1,
     ae_int_t nfeatures,
     ae_bool iscentered,
     apbuffers* buf0,
     apbuffers* buf1,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(i1>=i0, "RankDataBasecase: internal error", _state);
    if( buf1->ra0.cnt<nfeatures )
    {
        ae_vector_set_length(&buf1->ra0, nfeatures, _state);
    }
    for(i=i0; i<=i1-1; i++)
    {
        ae_v_move(&buf1->ra0.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nfeatures-1));
        rankx(&buf1->ra0, nfeatures, iscentered, buf0, _state);
        ae_v_move(&xy->ptr.pp_double[i][0], 1, &buf1->ra0.ptr.p_double[0], 1, ae_v_len(0,nfeatures-1));
    }
}


/*$ End $*/
