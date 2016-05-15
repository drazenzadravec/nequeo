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

#ifndef _filters_h
#define _filters_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "tsort.h"
#include "basicstatops.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "basestat.h"
#include "gammafunc.h"
#include "normaldistr.h"
#include "igammaf.h"
#include "hblas.h"
#include "reflections.h"
#include "creflections.h"
#include "sblas.h"
#include "ortfac.h"
#include "blas.h"
#include "rotations.h"
#include "hqrnd.h"
#include "bdsvd.h"
#include "svd.h"
#include "linreg.h"


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


/*$ End $*/
#endif

