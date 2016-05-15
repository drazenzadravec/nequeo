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
#include "filters.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Filters: simple moving averages (unsymmetric).

This filter replaces array by results of SMA(K) filter. SMA(K) is defined
as filter which averages at most K previous points (previous - not points
AROUND central point) - or less, in case of the first K-1 points.

INPUT PARAMETERS:
    X           -   array[N], array to process. It can be larger than N,
                    in this case only first N points are processed.
    N           -   points count, N>=0
    K           -   K>=1 (K can be larger than N ,  such  cases  will  be
                    correctly handled). Window width. K=1 corresponds  to
                    identity transformation (nothing changes).

OUTPUT PARAMETERS:
    X           -   array, whose first N elements were processed with SMA(K)

NOTE 1: this function uses efficient in-place  algorithm  which  does not
        allocate temporary arrays.

NOTE 2: this algorithm makes only one pass through array and uses running
        sum  to speed-up calculation of the averages. Additional measures
        are taken to ensure that running sum on a long sequence  of  zero
        elements will be correctly reset to zero even in the presence  of
        round-off error.

NOTE 3: this  is  unsymmetric version of the algorithm,  which  does  NOT
        averages points after the current one. Only X[i], X[i-1], ... are
        used when calculating new value of X[i]. We should also note that
        this algorithm uses BOTH previous points and  current  one,  i.e.
        new value of X[i] depends on BOTH previous point and X[i] itself.

  -- ALGLIB --
     Copyright 25.10.2011 by Bochkanov Sergey
*************************************************************************/
void filtersma(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_int_t k,
     ae_state *_state)
{
    ae_int_t i;
    double runningsum;
    double termsinsum;
    ae_int_t zeroprefix;
    double v;


    ae_assert(n>=0, "FilterSMA: N<0", _state);
    ae_assert(x->cnt>=n, "FilterSMA: Length(X)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "FilterSMA: X contains INF or NAN", _state);
    ae_assert(k>=1, "FilterSMA: K<1", _state);
    
    /*
     * Quick exit, if necessary
     */
    if( n<=1||k==1 )
    {
        return;
    }
    
    /*
     * Prepare variables (see below for explanation)
     */
    runningsum = 0.0;
    termsinsum = (double)(0);
    for(i=ae_maxint(n-k, 0, _state); i<=n-1; i++)
    {
        runningsum = runningsum+x->ptr.p_double[i];
        termsinsum = termsinsum+1;
    }
    i = ae_maxint(n-k, 0, _state);
    zeroprefix = 0;
    while(i<=n-1&&ae_fp_eq(x->ptr.p_double[i],(double)(0)))
    {
        zeroprefix = zeroprefix+1;
        i = i+1;
    }
    
    /*
     * General case: we assume that N>1 and K>1
     *
     * Make one pass through all elements. At the beginning of
     * the iteration we have:
     * * I              element being processed
     * * RunningSum     current value of the running sum
     *                  (including I-th element)
     * * TermsInSum     number of terms in sum, 0<=TermsInSum<=K
     * * ZeroPrefix     length of the sequence of zero elements
     *                  which starts at X[I-K+1] and continues towards X[I].
     *                  Equal to zero in case X[I-K+1] is non-zero.
     *                  This value is used to make RunningSum exactly zero
     *                  when it follows from the problem properties.
     */
    for(i=n-1; i>=0; i--)
    {
        
        /*
         * Store new value of X[i], save old value in V
         */
        v = x->ptr.p_double[i];
        x->ptr.p_double[i] = runningsum/termsinsum;
        
        /*
         * Update RunningSum and TermsInSum
         */
        if( i-k>=0 )
        {
            runningsum = runningsum-v+x->ptr.p_double[i-k];
        }
        else
        {
            runningsum = runningsum-v;
            termsinsum = termsinsum-1;
        }
        
        /*
         * Update ZeroPrefix.
         * In case we have ZeroPrefix=TermsInSum,
         * RunningSum is reset to zero.
         */
        if( i-k>=0 )
        {
            if( ae_fp_neq(x->ptr.p_double[i-k],(double)(0)) )
            {
                zeroprefix = 0;
            }
            else
            {
                zeroprefix = ae_minint(zeroprefix+1, k, _state);
            }
        }
        else
        {
            zeroprefix = ae_minint(zeroprefix, i+1, _state);
        }
        if( ae_fp_eq((double)(zeroprefix),termsinsum) )
        {
            runningsum = (double)(0);
        }
    }
}


/*************************************************************************
Filters: exponential moving averages.

This filter replaces array by results of EMA(alpha) filter. EMA(alpha) is
defined as filter which replaces X[] by S[]:
    S[0] = X[0]
    S[t] = alpha*X[t] + (1-alpha)*S[t-1]

INPUT PARAMETERS:
    X           -   array[N], array to process. It can be larger than N,
                    in this case only first N points are processed.
    N           -   points count, N>=0
    alpha       -   0<alpha<=1, smoothing parameter.

OUTPUT PARAMETERS:
    X           -   array, whose first N elements were processed
                    with EMA(alpha)

NOTE 1: this function uses efficient in-place  algorithm  which  does not
        allocate temporary arrays.

NOTE 2: this algorithm uses BOTH previous points and  current  one,  i.e.
        new value of X[i] depends on BOTH previous point and X[i] itself.

NOTE 3: technical analytis users quite often work  with  EMA  coefficient
        expressed in DAYS instead of fractions. If you want to  calculate
        EMA(N), where N is a number of days, you can use alpha=2/(N+1).

  -- ALGLIB --
     Copyright 25.10.2011 by Bochkanov Sergey
*************************************************************************/
void filterema(/* Real    */ ae_vector* x,
     ae_int_t n,
     double alpha,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(n>=0, "FilterEMA: N<0", _state);
    ae_assert(x->cnt>=n, "FilterEMA: Length(X)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "FilterEMA: X contains INF or NAN", _state);
    ae_assert(ae_fp_greater(alpha,(double)(0)), "FilterEMA: Alpha<=0", _state);
    ae_assert(ae_fp_less_eq(alpha,(double)(1)), "FilterEMA: Alpha>1", _state);
    
    /*
     * Quick exit, if necessary
     */
    if( n<=1||ae_fp_eq(alpha,(double)(1)) )
    {
        return;
    }
    
    /*
     * Process
     */
    for(i=1; i<=n-1; i++)
    {
        x->ptr.p_double[i] = alpha*x->ptr.p_double[i]+(1-alpha)*x->ptr.p_double[i-1];
    }
}


/*************************************************************************
Filters: linear regression moving averages.

This filter replaces array by results of LRMA(K) filter.

LRMA(K) is defined as filter which, for each data  point,  builds  linear
regression  model  using  K  prevous  points (point itself is included in
these K points) and calculates value of this linear model at the point in
question.

INPUT PARAMETERS:
    X           -   array[N], array to process. It can be larger than N,
                    in this case only first N points are processed.
    N           -   points count, N>=0
    K           -   K>=1 (K can be larger than N ,  such  cases  will  be
                    correctly handled). Window width. K=1 corresponds  to
                    identity transformation (nothing changes).

OUTPUT PARAMETERS:
    X           -   array, whose first N elements were processed with SMA(K)

NOTE 1: this function uses efficient in-place  algorithm  which  does not
        allocate temporary arrays.

NOTE 2: this algorithm makes only one pass through array and uses running
        sum  to speed-up calculation of the averages. Additional measures
        are taken to ensure that running sum on a long sequence  of  zero
        elements will be correctly reset to zero even in the presence  of
        round-off error.

NOTE 3: this  is  unsymmetric version of the algorithm,  which  does  NOT
        averages points after the current one. Only X[i], X[i-1], ... are
        used when calculating new value of X[i]. We should also note that
        this algorithm uses BOTH previous points and  current  one,  i.e.
        new value of X[i] depends on BOTH previous point and X[i] itself.

  -- ALGLIB --
     Copyright 25.10.2011 by Bochkanov Sergey
*************************************************************************/
void filterlrma(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_int_t k,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t m;
    ae_matrix xy;
    ae_vector s;
    ae_int_t info;
    double a;
    double b;
    double vara;
    double varb;
    double covab;
    double corrab;
    double p;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);

    ae_assert(n>=0, "FilterLRMA: N<0", _state);
    ae_assert(x->cnt>=n, "FilterLRMA: Length(X)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "FilterLRMA: X contains INF or NAN", _state);
    ae_assert(k>=1, "FilterLRMA: K<1", _state);
    
    /*
     * Quick exit, if necessary:
     * * either N is equal to 1 (nothing to average)
     * * or K is 1 (only point itself is used) or 2 (model is too simple,
     *   we will always get identity transformation)
     */
    if( n<=1||k<=2 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * General case: K>2, N>1.
     * We do not process points with I<2 because first two points (I=0 and I=1) will be
     * left unmodified by LRMA filter in any case.
     */
    ae_matrix_set_length(&xy, k, 2, _state);
    ae_vector_set_length(&s, k, _state);
    for(i=0; i<=k-1; i++)
    {
        xy.ptr.pp_double[i][0] = (double)(i);
        s.ptr.p_double[i] = 1.0;
    }
    for(i=n-1; i>=2; i--)
    {
        m = ae_minint(i+1, k, _state);
        ae_v_move(&xy.ptr.pp_double[0][1], xy.stride, &x->ptr.p_double[i-m+1], 1, ae_v_len(0,m-1));
        lrlines(&xy, &s, m, &info, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
        ae_assert(info==1, "FilterLRMA: internal error", _state);
        x->ptr.p_double[i] = a+b*(m-1);
    }
    ae_frame_leave(_state);
}


/*$ End $*/
