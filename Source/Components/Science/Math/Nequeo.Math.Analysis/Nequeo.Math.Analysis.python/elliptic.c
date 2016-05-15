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
#include "elliptic.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Complete elliptic integral of the first kind

Approximates the integral



           pi/2
            -
           | |
           |           dt
K(m)  =    |    ------------------
           |                   2
         | |    sqrt( 1 - m sin t )
          -
           0

using the approximation

    P(x)  -  log x Q(x).

ACCURACY:

                     Relative error:
arithmetic   domain     # trials      peak         rms
   IEEE       0,1        30000       2.5e-16     6.8e-17

Cephes Math Library, Release 2.8:  June, 2000
Copyright 1984, 1987, 2000 by Stephen L. Moshier
*************************************************************************/
double ellipticintegralk(double m, ae_state *_state)
{
    double result;


    result = ellipticintegralkhighprecision(1.0-m, _state);
    return result;
}


/*************************************************************************
Complete elliptic integral of the first kind

Approximates the integral



           pi/2
            -
           | |
           |           dt
K(m)  =    |    ------------------
           |                   2
         | |    sqrt( 1 - m sin t )
          -
           0

where m = 1 - m1, using the approximation

    P(x)  -  log x Q(x).

The argument m1 is used rather than m so that the logarithmic
singularity at m = 1 will be shifted to the origin; this
preserves maximum accuracy.

K(0) = pi/2.

ACCURACY:

                     Relative error:
arithmetic   domain     # trials      peak         rms
   IEEE       0,1        30000       2.5e-16     6.8e-17

Cephes Math Library, Release 2.8:  June, 2000
Copyright 1984, 1987, 2000 by Stephen L. Moshier
*************************************************************************/
double ellipticintegralkhighprecision(double m1, ae_state *_state)
{
    double p;
    double q;
    double result;


    if( ae_fp_less_eq(m1,ae_machineepsilon) )
    {
        result = 1.3862943611198906188E0-0.5*ae_log(m1, _state);
    }
    else
    {
        p = 1.37982864606273237150E-4;
        p = p*m1+2.28025724005875567385E-3;
        p = p*m1+7.97404013220415179367E-3;
        p = p*m1+9.85821379021226008714E-3;
        p = p*m1+6.87489687449949877925E-3;
        p = p*m1+6.18901033637687613229E-3;
        p = p*m1+8.79078273952743772254E-3;
        p = p*m1+1.49380448916805252718E-2;
        p = p*m1+3.08851465246711995998E-2;
        p = p*m1+9.65735902811690126535E-2;
        p = p*m1+1.38629436111989062502E0;
        q = 2.94078955048598507511E-5;
        q = q*m1+9.14184723865917226571E-4;
        q = q*m1+5.94058303753167793257E-3;
        q = q*m1+1.54850516649762399335E-2;
        q = q*m1+2.39089602715924892727E-2;
        q = q*m1+3.01204715227604046988E-2;
        q = q*m1+3.73774314173823228969E-2;
        q = q*m1+4.88280347570998239232E-2;
        q = q*m1+7.03124996963957469739E-2;
        q = q*m1+1.24999999999870820058E-1;
        q = q*m1+4.99999999999999999821E-1;
        result = p-q*ae_log(m1, _state);
    }
    return result;
}


/*************************************************************************
Incomplete elliptic integral of the first kind F(phi|m)

Approximates the integral



               phi
                -
               | |
               |           dt
F(phi_\m)  =    |    ------------------
               |                   2
             | |    sqrt( 1 - m sin t )
              -
               0

of amplitude phi and modulus m, using the arithmetic -
geometric mean algorithm.




ACCURACY:

Tested at random points with m in [0, 1] and phi as indicated.

                     Relative error:
arithmetic   domain     # trials      peak         rms
   IEEE     -10,10       200000      7.4e-16     1.0e-16

Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 2000 by Stephen L. Moshier
*************************************************************************/
double incompleteellipticintegralk(double phi, double m, ae_state *_state)
{
    double a;
    double b;
    double c;
    double e;
    double temp;
    double pio2;
    double t;
    double k;
    ae_int_t d;
    ae_int_t md;
    ae_int_t s;
    ae_int_t npio2;
    double result;


    pio2 = 1.57079632679489661923;
    if( ae_fp_eq(m,(double)(0)) )
    {
        result = phi;
        return result;
    }
    a = 1-m;
    if( ae_fp_eq(a,(double)(0)) )
    {
        result = ae_log(ae_tan(0.5*(pio2+phi), _state), _state);
        return result;
    }
    npio2 = ae_ifloor(phi/pio2, _state);
    if( npio2%2!=0 )
    {
        npio2 = npio2+1;
    }
    if( npio2!=0 )
    {
        k = ellipticintegralk(1-a, _state);
        phi = phi-npio2*pio2;
    }
    else
    {
        k = (double)(0);
    }
    if( ae_fp_less(phi,(double)(0)) )
    {
        phi = -phi;
        s = -1;
    }
    else
    {
        s = 0;
    }
    b = ae_sqrt(a, _state);
    t = ae_tan(phi, _state);
    if( ae_fp_greater(ae_fabs(t, _state),(double)(10)) )
    {
        e = 1.0/(b*t);
        if( ae_fp_less(ae_fabs(e, _state),(double)(10)) )
        {
            e = ae_atan(e, _state);
            if( npio2==0 )
            {
                k = ellipticintegralk(1-a, _state);
            }
            temp = k-incompleteellipticintegralk(e, m, _state);
            if( s<0 )
            {
                temp = -temp;
            }
            result = temp+npio2*k;
            return result;
        }
    }
    a = 1.0;
    c = ae_sqrt(m, _state);
    d = 1;
    md = 0;
    while(ae_fp_greater(ae_fabs(c/a, _state),ae_machineepsilon))
    {
        temp = b/a;
        phi = phi+ae_atan(t*temp, _state)+md*ae_pi;
        md = ae_trunc((phi+pio2)/ae_pi, _state);
        t = t*(1.0+temp)/(1.0-temp*t*t);
        c = 0.5*(a-b);
        temp = ae_sqrt(a*b, _state);
        a = 0.5*(a+b);
        b = temp;
        d = d+d;
    }
    temp = (ae_atan(t, _state)+md*ae_pi)/(d*a);
    if( s<0 )
    {
        temp = -temp;
    }
    result = temp+npio2*k;
    return result;
}


/*************************************************************************
Complete elliptic integral of the second kind

Approximates the integral


           pi/2
            -
           | |                 2
E(m)  =    |    sqrt( 1 - m sin t ) dt
         | |
          -
           0

using the approximation

     P(x)  -  x log x Q(x).

ACCURACY:

                     Relative error:
arithmetic   domain     # trials      peak         rms
   IEEE       0, 1       10000       2.1e-16     7.3e-17

Cephes Math Library, Release 2.8: June, 2000
Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
*************************************************************************/
double ellipticintegrale(double m, ae_state *_state)
{
    double p;
    double q;
    double result;


    ae_assert(ae_fp_greater_eq(m,(double)(0))&&ae_fp_less_eq(m,(double)(1)), "Domain error in EllipticIntegralE: m<0 or m>1", _state);
    m = 1-m;
    if( ae_fp_eq(m,(double)(0)) )
    {
        result = (double)(1);
        return result;
    }
    p = 1.53552577301013293365E-4;
    p = p*m+2.50888492163602060990E-3;
    p = p*m+8.68786816565889628429E-3;
    p = p*m+1.07350949056076193403E-2;
    p = p*m+7.77395492516787092951E-3;
    p = p*m+7.58395289413514708519E-3;
    p = p*m+1.15688436810574127319E-2;
    p = p*m+2.18317996015557253103E-2;
    p = p*m+5.68051945617860553470E-2;
    p = p*m+4.43147180560990850618E-1;
    p = p*m+1.00000000000000000299E0;
    q = 3.27954898576485872656E-5;
    q = q*m+1.00962792679356715133E-3;
    q = q*m+6.50609489976927491433E-3;
    q = q*m+1.68862163993311317300E-2;
    q = q*m+2.61769742454493659583E-2;
    q = q*m+3.34833904888224918614E-2;
    q = q*m+4.27180926518931511717E-2;
    q = q*m+5.85936634471101055642E-2;
    q = q*m+9.37499997197644278445E-2;
    q = q*m+2.49999999999888314361E-1;
    result = p-q*m*ae_log(m, _state);
    return result;
}


/*************************************************************************
Incomplete elliptic integral of the second kind

Approximates the integral


               phi
                -
               | |
               |                   2
E(phi_\m)  =    |    sqrt( 1 - m sin t ) dt
               |
             | |
              -
               0

of amplitude phi and modulus m, using the arithmetic -
geometric mean algorithm.

ACCURACY:

Tested at random arguments with phi in [-10, 10] and m in
[0, 1].
                     Relative error:
arithmetic   domain     # trials      peak         rms
   IEEE     -10,10      150000       3.3e-15     1.4e-16

Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 1993, 2000 by Stephen L. Moshier
*************************************************************************/
double incompleteellipticintegrale(double phi, double m, ae_state *_state)
{
    double pio2;
    double a;
    double b;
    double c;
    double e;
    double temp;
    double lphi;
    double t;
    double ebig;
    ae_int_t d;
    ae_int_t md;
    ae_int_t npio2;
    ae_int_t s;
    double result;


    pio2 = 1.57079632679489661923;
    if( ae_fp_eq(m,(double)(0)) )
    {
        result = phi;
        return result;
    }
    lphi = phi;
    npio2 = ae_ifloor(lphi/pio2, _state);
    if( npio2%2!=0 )
    {
        npio2 = npio2+1;
    }
    lphi = lphi-npio2*pio2;
    if( ae_fp_less(lphi,(double)(0)) )
    {
        lphi = -lphi;
        s = -1;
    }
    else
    {
        s = 1;
    }
    a = 1.0-m;
    ebig = ellipticintegrale(m, _state);
    if( ae_fp_eq(a,(double)(0)) )
    {
        temp = ae_sin(lphi, _state);
        if( s<0 )
        {
            temp = -temp;
        }
        result = temp+npio2*ebig;
        return result;
    }
    t = ae_tan(lphi, _state);
    b = ae_sqrt(a, _state);
    
    /*
     * Thanks to Brian Fitzgerald <fitzgb@mml0.meche.rpi.edu>
     * for pointing out an instability near odd multiples of pi/2
     */
    if( ae_fp_greater(ae_fabs(t, _state),(double)(10)) )
    {
        
        /*
         * Transform the amplitude
         */
        e = 1.0/(b*t);
        
        /*
         * ... but avoid multiple recursions.
         */
        if( ae_fp_less(ae_fabs(e, _state),(double)(10)) )
        {
            e = ae_atan(e, _state);
            temp = ebig+m*ae_sin(lphi, _state)*ae_sin(e, _state)-incompleteellipticintegrale(e, m, _state);
            if( s<0 )
            {
                temp = -temp;
            }
            result = temp+npio2*ebig;
            return result;
        }
    }
    c = ae_sqrt(m, _state);
    a = 1.0;
    d = 1;
    e = 0.0;
    md = 0;
    while(ae_fp_greater(ae_fabs(c/a, _state),ae_machineepsilon))
    {
        temp = b/a;
        lphi = lphi+ae_atan(t*temp, _state)+md*ae_pi;
        md = ae_trunc((lphi+pio2)/ae_pi, _state);
        t = t*(1.0+temp)/(1.0-temp*t*t);
        c = 0.5*(a-b);
        temp = ae_sqrt(a*b, _state);
        a = 0.5*(a+b);
        b = temp;
        d = d+d;
        e = e+c*ae_sin(lphi, _state);
    }
    temp = ebig/ellipticintegralk(m, _state);
    temp = temp*((ae_atan(t, _state)+md*ae_pi)/(d*a));
    temp = temp+e;
    if( s<0 )
    {
        temp = -temp;
    }
    result = temp+npio2*ebig;
    return result;
}


/*$ End $*/
