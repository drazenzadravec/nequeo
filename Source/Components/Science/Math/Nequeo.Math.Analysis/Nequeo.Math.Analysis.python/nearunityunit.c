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
#include "nearunityunit.h"


/*$ Declarations $*/


/*$ Body $*/


double nulog1p(double x, ae_state *_state)
{
    double z;
    double lp;
    double lq;
    double result;


    z = 1.0+x;
    if( ae_fp_less(z,0.70710678118654752440)||ae_fp_greater(z,1.41421356237309504880) )
    {
        result = ae_log(z, _state);
        return result;
    }
    z = x*x;
    lp = 4.5270000862445199635215E-5;
    lp = lp*x+4.9854102823193375972212E-1;
    lp = lp*x+6.5787325942061044846969E0;
    lp = lp*x+2.9911919328553073277375E1;
    lp = lp*x+6.0949667980987787057556E1;
    lp = lp*x+5.7112963590585538103336E1;
    lp = lp*x+2.0039553499201281259648E1;
    lq = 1.0000000000000000000000E0;
    lq = lq*x+1.5062909083469192043167E1;
    lq = lq*x+8.3047565967967209469434E1;
    lq = lq*x+2.2176239823732856465394E2;
    lq = lq*x+3.0909872225312059774938E2;
    lq = lq*x+2.1642788614495947685003E2;
    lq = lq*x+6.0118660497603843919306E1;
    z = -0.5*z+x*(z*lp/lq);
    result = x+z;
    return result;
}


double nuexpm1(double x, ae_state *_state)
{
    double r;
    double xx;
    double ep;
    double eq;
    double result;


    if( ae_fp_less(x,-0.5)||ae_fp_greater(x,0.5) )
    {
        result = ae_exp(x, _state)-1.0;
        return result;
    }
    xx = x*x;
    ep = 1.2617719307481059087798E-4;
    ep = ep*xx+3.0299440770744196129956E-2;
    ep = ep*xx+9.9999999999999999991025E-1;
    eq = 3.0019850513866445504159E-6;
    eq = eq*xx+2.5244834034968410419224E-3;
    eq = eq*xx+2.2726554820815502876593E-1;
    eq = eq*xx+2.0000000000000000000897E0;
    r = x*ep;
    r = r/(eq-r);
    result = r+r;
    return result;
}


double nucosm1(double x, ae_state *_state)
{
    double xx;
    double c;
    double result;


    if( ae_fp_less(x,-0.25*ae_pi)||ae_fp_greater(x,0.25*ae_pi) )
    {
        result = ae_cos(x, _state)-1;
        return result;
    }
    xx = x*x;
    c = 4.7377507964246204691685E-14;
    c = c*xx-1.1470284843425359765671E-11;
    c = c*xx+2.0876754287081521758361E-9;
    c = c*xx-2.7557319214999787979814E-7;
    c = c*xx+2.4801587301570552304991E-5;
    c = c*xx-1.3888888888888872993737E-3;
    c = c*xx+4.1666666666666666609054E-2;
    result = -0.5*xx+xx*xx*c;
    return result;
}


/*$ End $*/
