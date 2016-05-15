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
#include "binomialdistr.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Binomial distribution

Returns the sum of the terms 0 through k of the Binomial
probability density:

  k
  --  ( n )   j      n-j
  >   (   )  p  (1-p)
  --  ( j )
 j=0

The terms are not summed directly; instead the incomplete
beta integral is employed, according to the formula

y = bdtr( k, n, p ) = incbet( n-k, k+1, 1-p ).

The arguments must be positive, with p ranging from 0 to 1.

ACCURACY:

Tested at random points (a,b,p), with p between 0 and 1.

              a,b                     Relative error:
arithmetic  domain     # trials      peak         rms
 For p between 0.001 and 1:
   IEEE     0,100       100000      4.3e-15     2.6e-16

Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
*************************************************************************/
double binomialdistribution(ae_int_t k,
     ae_int_t n,
     double p,
     ae_state *_state)
{
    double dk;
    double dn;
    double result;


    ae_assert(ae_fp_greater_eq(p,(double)(0))&&ae_fp_less_eq(p,(double)(1)), "Domain error in BinomialDistribution", _state);
    ae_assert(k>=-1&&k<=n, "Domain error in BinomialDistribution", _state);
    if( k==-1 )
    {
        result = (double)(0);
        return result;
    }
    if( k==n )
    {
        result = (double)(1);
        return result;
    }
    dn = (double)(n-k);
    if( k==0 )
    {
        dk = ae_pow(1.0-p, dn, _state);
    }
    else
    {
        dk = (double)(k+1);
        dk = incompletebeta(dn, dk, 1.0-p, _state);
    }
    result = dk;
    return result;
}


/*************************************************************************
Complemented binomial distribution

Returns the sum of the terms k+1 through n of the Binomial
probability density:

  n
  --  ( n )   j      n-j
  >   (   )  p  (1-p)
  --  ( j )
 j=k+1

The terms are not summed directly; instead the incomplete
beta integral is employed, according to the formula

y = bdtrc( k, n, p ) = incbet( k+1, n-k, p ).

The arguments must be positive, with p ranging from 0 to 1.

ACCURACY:

Tested at random points (a,b,p).

              a,b                     Relative error:
arithmetic  domain     # trials      peak         rms
 For p between 0.001 and 1:
   IEEE     0,100       100000      6.7e-15     8.2e-16
 For p between 0 and .001:
   IEEE     0,100       100000      1.5e-13     2.7e-15

Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
*************************************************************************/
double binomialcdistribution(ae_int_t k,
     ae_int_t n,
     double p,
     ae_state *_state)
{
    double dk;
    double dn;
    double result;


    ae_assert(ae_fp_greater_eq(p,(double)(0))&&ae_fp_less_eq(p,(double)(1)), "Domain error in BinomialDistributionC", _state);
    ae_assert(k>=-1&&k<=n, "Domain error in BinomialDistributionC", _state);
    if( k==-1 )
    {
        result = (double)(1);
        return result;
    }
    if( k==n )
    {
        result = (double)(0);
        return result;
    }
    dn = (double)(n-k);
    if( k==0 )
    {
        if( ae_fp_less(p,0.01) )
        {
            dk = -nuexpm1(dn*nulog1p(-p, _state), _state);
        }
        else
        {
            dk = 1.0-ae_pow(1.0-p, dn, _state);
        }
    }
    else
    {
        dk = (double)(k+1);
        dk = incompletebeta(dk, dn, p, _state);
    }
    result = dk;
    return result;
}


/*************************************************************************
Inverse binomial distribution

Finds the event probability p such that the sum of the
terms 0 through k of the Binomial probability density
is equal to the given cumulative probability y.

This is accomplished using the inverse beta integral
function and the relation

1 - p = incbi( n-k, k+1, y ).

ACCURACY:

Tested at random points (a,b,p).

              a,b                     Relative error:
arithmetic  domain     # trials      peak         rms
 For p between 0.001 and 1:
   IEEE     0,100       100000      2.3e-14     6.4e-16
   IEEE     0,10000     100000      6.6e-12     1.2e-13
 For p between 10^-6 and 0.001:
   IEEE     0,100       100000      2.0e-12     1.3e-14
   IEEE     0,10000     100000      1.5e-12     3.2e-14

Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
*************************************************************************/
double invbinomialdistribution(ae_int_t k,
     ae_int_t n,
     double y,
     ae_state *_state)
{
    double dk;
    double dn;
    double p;
    double result;


    ae_assert(k>=0&&k<n, "Domain error in InvBinomialDistribution", _state);
    dn = (double)(n-k);
    if( k==0 )
    {
        if( ae_fp_greater(y,0.8) )
        {
            p = -nuexpm1(nulog1p(y-1.0, _state)/dn, _state);
        }
        else
        {
            p = 1.0-ae_pow(y, 1.0/dn, _state);
        }
    }
    else
    {
        dk = (double)(k+1);
        p = incompletebeta(dn, dk, 0.5, _state);
        if( ae_fp_greater(p,0.5) )
        {
            p = invincompletebeta(dk, dn, 1.0-y, _state);
        }
        else
        {
            p = 1.0-invincompletebeta(dn, dk, y, _state);
        }
    }
    result = p;
    return result;
}


/*$ End $*/
