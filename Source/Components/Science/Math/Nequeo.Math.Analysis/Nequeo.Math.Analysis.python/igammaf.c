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
#include "igammaf.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Incomplete gamma integral

The function is defined by

                          x
                           -
                  1       | |  -t  a-1
 igam(a,x)  =   -----     |   e   t   dt.
                 -      | |
                | (a)    -
                          0


In this implementation both arguments must be positive.
The integral is evaluated by either a power series or
continued fraction expansion, depending on the relative
values of a and x.

ACCURACY:

                     Relative error:
arithmetic   domain     # trials      peak         rms
   IEEE      0,30       200000       3.6e-14     2.9e-15
   IEEE      0,100      300000       9.9e-14     1.5e-14

Cephes Math Library Release 2.8:  June, 2000
Copyright 1985, 1987, 2000 by Stephen L. Moshier
*************************************************************************/
double incompletegamma(double a, double x, ae_state *_state)
{
    double igammaepsilon;
    double ans;
    double ax;
    double c;
    double r;
    double tmp;
    double result;


    igammaepsilon = 0.000000000000001;
    if( ae_fp_less_eq(x,(double)(0))||ae_fp_less_eq(a,(double)(0)) )
    {
        result = (double)(0);
        return result;
    }
    if( ae_fp_greater(x,(double)(1))&&ae_fp_greater(x,a) )
    {
        result = 1-incompletegammac(a, x, _state);
        return result;
    }
    ax = a*ae_log(x, _state)-x-lngamma(a, &tmp, _state);
    if( ae_fp_less(ax,-709.78271289338399) )
    {
        result = (double)(0);
        return result;
    }
    ax = ae_exp(ax, _state);
    r = a;
    c = (double)(1);
    ans = (double)(1);
    do
    {
        r = r+1;
        c = c*x/r;
        ans = ans+c;
    }
    while(ae_fp_greater(c/ans,igammaepsilon));
    result = ans*ax/a;
    return result;
}


/*************************************************************************
Complemented incomplete gamma integral

The function is defined by


 igamc(a,x)   =   1 - igam(a,x)

                           inf.
                             -
                    1       | |  -t  a-1
              =   -----     |   e   t   dt.
                   -      | |
                  | (a)    -
                            x


In this implementation both arguments must be positive.
The integral is evaluated by either a power series or
continued fraction expansion, depending on the relative
values of a and x.

ACCURACY:

Tested at random a, x.
               a         x                      Relative error:
arithmetic   domain   domain     # trials      peak         rms
   IEEE     0.5,100   0,100      200000       1.9e-14     1.7e-15
   IEEE     0.01,0.5  0,100      200000       1.4e-13     1.6e-15

Cephes Math Library Release 2.8:  June, 2000
Copyright 1985, 1987, 2000 by Stephen L. Moshier
*************************************************************************/
double incompletegammac(double a, double x, ae_state *_state)
{
    double igammaepsilon;
    double igammabignumber;
    double igammabignumberinv;
    double ans;
    double ax;
    double c;
    double yc;
    double r;
    double t;
    double y;
    double z;
    double pk;
    double pkm1;
    double pkm2;
    double qk;
    double qkm1;
    double qkm2;
    double tmp;
    double result;


    igammaepsilon = 0.000000000000001;
    igammabignumber = 4503599627370496.0;
    igammabignumberinv = 2.22044604925031308085*0.0000000000000001;
    if( ae_fp_less_eq(x,(double)(0))||ae_fp_less_eq(a,(double)(0)) )
    {
        result = (double)(1);
        return result;
    }
    if( ae_fp_less(x,(double)(1))||ae_fp_less(x,a) )
    {
        result = 1-incompletegamma(a, x, _state);
        return result;
    }
    ax = a*ae_log(x, _state)-x-lngamma(a, &tmp, _state);
    if( ae_fp_less(ax,-709.78271289338399) )
    {
        result = (double)(0);
        return result;
    }
    ax = ae_exp(ax, _state);
    y = 1-a;
    z = x+y+1;
    c = (double)(0);
    pkm2 = (double)(1);
    qkm2 = x;
    pkm1 = x+1;
    qkm1 = z*x;
    ans = pkm1/qkm1;
    do
    {
        c = c+1;
        y = y+1;
        z = z+2;
        yc = y*c;
        pk = pkm1*z-pkm2*yc;
        qk = qkm1*z-qkm2*yc;
        if( ae_fp_neq(qk,(double)(0)) )
        {
            r = pk/qk;
            t = ae_fabs((ans-r)/r, _state);
            ans = r;
        }
        else
        {
            t = (double)(1);
        }
        pkm2 = pkm1;
        pkm1 = pk;
        qkm2 = qkm1;
        qkm1 = qk;
        if( ae_fp_greater(ae_fabs(pk, _state),igammabignumber) )
        {
            pkm2 = pkm2*igammabignumberinv;
            pkm1 = pkm1*igammabignumberinv;
            qkm2 = qkm2*igammabignumberinv;
            qkm1 = qkm1*igammabignumberinv;
        }
    }
    while(ae_fp_greater(t,igammaepsilon));
    result = ans*ax;
    return result;
}


/*************************************************************************
Inverse of complemented imcomplete gamma integral

Given p, the function finds x such that

 igamc( a, x ) = p.

Starting with the approximate value

        3
 x = a t

 where

 t = 1 - d - ndtri(p) sqrt(d)

and

 d = 1/9a,

the routine performs up to 10 Newton iterations to find the
root of igamc(a,x) - p = 0.

ACCURACY:

Tested at random a, p in the intervals indicated.

               a        p                      Relative error:
arithmetic   domain   domain     # trials      peak         rms
   IEEE     0.5,100   0,0.5       100000       1.0e-14     1.7e-15
   IEEE     0.01,0.5  0,0.5       100000       9.0e-14     3.4e-15
   IEEE    0.5,10000  0,0.5        20000       2.3e-13     3.8e-14

Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
*************************************************************************/
double invincompletegammac(double a, double y0, ae_state *_state)
{
    double igammaepsilon;
    double iinvgammabignumber;
    double x0;
    double x1;
    double x;
    double yl;
    double yh;
    double y;
    double d;
    double lgm;
    double dithresh;
    ae_int_t i;
    ae_int_t dir;
    double tmp;
    double result;


    igammaepsilon = 0.000000000000001;
    iinvgammabignumber = 4503599627370496.0;
    x0 = iinvgammabignumber;
    yl = (double)(0);
    x1 = (double)(0);
    yh = (double)(1);
    dithresh = 5*igammaepsilon;
    d = 1/(9*a);
    y = 1-d-invnormaldistribution(y0, _state)*ae_sqrt(d, _state);
    x = a*y*y*y;
    lgm = lngamma(a, &tmp, _state);
    i = 0;
    while(i<10)
    {
        if( ae_fp_greater(x,x0)||ae_fp_less(x,x1) )
        {
            d = 0.0625;
            break;
        }
        y = incompletegammac(a, x, _state);
        if( ae_fp_less(y,yl)||ae_fp_greater(y,yh) )
        {
            d = 0.0625;
            break;
        }
        if( ae_fp_less(y,y0) )
        {
            x0 = x;
            yl = y;
        }
        else
        {
            x1 = x;
            yh = y;
        }
        d = (a-1)*ae_log(x, _state)-x-lgm;
        if( ae_fp_less(d,-709.78271289338399) )
        {
            d = 0.0625;
            break;
        }
        d = -ae_exp(d, _state);
        d = (y-y0)/d;
        if( ae_fp_less(ae_fabs(d/x, _state),igammaepsilon) )
        {
            result = x;
            return result;
        }
        x = x-d;
        i = i+1;
    }
    if( ae_fp_eq(x0,iinvgammabignumber) )
    {
        if( ae_fp_less_eq(x,(double)(0)) )
        {
            x = (double)(1);
        }
        while(ae_fp_eq(x0,iinvgammabignumber))
        {
            x = (1+d)*x;
            y = incompletegammac(a, x, _state);
            if( ae_fp_less(y,y0) )
            {
                x0 = x;
                yl = y;
                break;
            }
            d = d+d;
        }
    }
    d = 0.5;
    dir = 0;
    i = 0;
    while(i<400)
    {
        x = x1+d*(x0-x1);
        y = incompletegammac(a, x, _state);
        lgm = (x0-x1)/(x1+x0);
        if( ae_fp_less(ae_fabs(lgm, _state),dithresh) )
        {
            break;
        }
        lgm = (y-y0)/y0;
        if( ae_fp_less(ae_fabs(lgm, _state),dithresh) )
        {
            break;
        }
        if( ae_fp_less_eq(x,0.0) )
        {
            break;
        }
        if( ae_fp_greater_eq(y,y0) )
        {
            x1 = x;
            yh = y;
            if( dir<0 )
            {
                dir = 0;
                d = 0.5;
            }
            else
            {
                if( dir>1 )
                {
                    d = 0.5*d+0.5;
                }
                else
                {
                    d = (y0-yl)/(yh-yl);
                }
            }
            dir = dir+1;
        }
        else
        {
            x0 = x;
            yl = y;
            if( dir>0 )
            {
                dir = 0;
                d = 0.5;
            }
            else
            {
                if( dir<-1 )
                {
                    d = 0.5*d;
                }
                else
                {
                    d = (y0-yl)/(yh-yl);
                }
            }
            dir = dir-1;
        }
        i = i+1;
    }
    result = x;
    return result;
}


/*$ End $*/
