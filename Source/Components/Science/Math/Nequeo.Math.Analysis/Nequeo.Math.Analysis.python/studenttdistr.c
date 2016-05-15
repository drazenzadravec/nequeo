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
#include "studenttdistr.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Student's t distribution

Computes the integral from minus infinity to t of the Student
t distribution with integer k > 0 degrees of freedom:

                                     t
                                     -
                                    | |
             -                      |         2   -(k+1)/2
            | ( (k+1)/2 )           |  (     x   )
      ----------------------        |  ( 1 + --- )        dx
                    -               |  (      k  )
      sqrt( k pi ) | ( k/2 )        |
                                  | |
                                   -
                                  -inf.

Relation to incomplete beta integral:

       1 - stdtr(k,t) = 0.5 * incbet( k/2, 1/2, z )
where
       z = k/(k + t**2).

For t < -2, this is the method of computation.  For higher t,
a direct method is derived from integration by parts.
Since the function is symmetric about t=0, the area under the
right tail of the density is found by calling the function
with -t instead of t.

ACCURACY:

Tested at random 1 <= k <= 25.  The "domain" refers to t.
                     Relative error:
arithmetic   domain     # trials      peak         rms
   IEEE     -100,-2      50000       5.9e-15     1.4e-15
   IEEE     -2,100      500000       2.7e-15     4.9e-17

Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
*************************************************************************/
double studenttdistribution(ae_int_t k, double t, ae_state *_state)
{
    double x;
    double rk;
    double z;
    double f;
    double tz;
    double p;
    double xsqk;
    ae_int_t j;
    double result;


    ae_assert(k>0, "Domain error in StudentTDistribution", _state);
    if( ae_fp_eq(t,(double)(0)) )
    {
        result = 0.5;
        return result;
    }
    if( ae_fp_less(t,-2.0) )
    {
        rk = (double)(k);
        z = rk/(rk+t*t);
        result = 0.5*incompletebeta(0.5*rk, 0.5, z, _state);
        return result;
    }
    if( ae_fp_less(t,(double)(0)) )
    {
        x = -t;
    }
    else
    {
        x = t;
    }
    rk = (double)(k);
    z = 1.0+x*x/rk;
    if( k%2!=0 )
    {
        xsqk = x/ae_sqrt(rk, _state);
        p = ae_atan(xsqk, _state);
        if( k>1 )
        {
            f = 1.0;
            tz = 1.0;
            j = 3;
            while(j<=k-2&&ae_fp_greater(tz/f,ae_machineepsilon))
            {
                tz = tz*((j-1)/(z*j));
                f = f+tz;
                j = j+2;
            }
            p = p+f*xsqk/z;
        }
        p = p*2.0/ae_pi;
    }
    else
    {
        f = 1.0;
        tz = 1.0;
        j = 2;
        while(j<=k-2&&ae_fp_greater(tz/f,ae_machineepsilon))
        {
            tz = tz*((j-1)/(z*j));
            f = f+tz;
            j = j+2;
        }
        p = f*x/ae_sqrt(z*rk, _state);
    }
    if( ae_fp_less(t,(double)(0)) )
    {
        p = -p;
    }
    result = 0.5+0.5*p;
    return result;
}


/*************************************************************************
Functional inverse of Student's t distribution

Given probability p, finds the argument t such that stdtr(k,t)
is equal to p.

ACCURACY:

Tested at random 1 <= k <= 100.  The "domain" refers to p:
                     Relative error:
arithmetic   domain     # trials      peak         rms
   IEEE    .001,.999     25000       5.7e-15     8.0e-16
   IEEE    10^-6,.001    25000       2.0e-12     2.9e-14

Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
*************************************************************************/
double invstudenttdistribution(ae_int_t k, double p, ae_state *_state)
{
    double t;
    double rk;
    double z;
    ae_int_t rflg;
    double result;


    ae_assert((k>0&&ae_fp_greater(p,(double)(0)))&&ae_fp_less(p,(double)(1)), "Domain error in InvStudentTDistribution", _state);
    rk = (double)(k);
    if( ae_fp_greater(p,0.25)&&ae_fp_less(p,0.75) )
    {
        if( ae_fp_eq(p,0.5) )
        {
            result = (double)(0);
            return result;
        }
        z = 1.0-2.0*p;
        z = invincompletebeta(0.5, 0.5*rk, ae_fabs(z, _state), _state);
        t = ae_sqrt(rk*z/(1.0-z), _state);
        if( ae_fp_less(p,0.5) )
        {
            t = -t;
        }
        result = t;
        return result;
    }
    rflg = -1;
    if( ae_fp_greater_eq(p,0.5) )
    {
        p = 1.0-p;
        rflg = 1;
    }
    z = invincompletebeta(0.5*rk, 0.5, 2.0*p, _state);
    if( ae_fp_less(ae_maxrealnumber*z,rk) )
    {
        result = rflg*ae_maxrealnumber;
        return result;
    }
    t = ae_sqrt(rk/z-rk, _state);
    result = rflg*t;
    return result;
}


/*$ End $*/
