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
#include "jacobianelliptic.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Jacobian Elliptic Functions

Evaluates the Jacobian elliptic functions sn(u|m), cn(u|m),
and dn(u|m) of parameter m between 0 and 1, and real
argument u.

These functions are periodic, with quarter-period on the
real axis equal to the complete elliptic integral
ellpk(1.0-m).

Relation to incomplete elliptic integral:
If u = ellik(phi,m), then sn(u|m) = sin(phi),
and cn(u|m) = cos(phi).  Phi is called the amplitude of u.

Computation is by means of the arithmetic-geometric mean
algorithm, except when m is within 1e-9 of 0 or 1.  In the
latter case with m close to 1, the approximation applies
only for phi < pi/2.

ACCURACY:

Tested at random points with u between 0 and 10, m between
0 and 1.

           Absolute error (* = relative error):
arithmetic   function   # trials      peak         rms
   IEEE      phi         10000       9.2e-16*    1.4e-16*
   IEEE      sn          50000       4.1e-15     4.6e-16
   IEEE      cn          40000       3.6e-15     4.4e-16
   IEEE      dn          10000       1.3e-12     1.8e-14

 Peak error observed in consistency check using addition
theorem for sn(u+v) was 4e-16 (absolute).  Also tested by
the above relation to the incomplete elliptic integral.
Accuracy deteriorates when u is large.

Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 2000 by Stephen L. Moshier
*************************************************************************/
void jacobianellipticfunctions(double u,
     double m,
     double* sn,
     double* cn,
     double* dn,
     double* ph,
     ae_state *_state)
{
    ae_frame _frame_block;
    double ai;
    double b;
    double phi;
    double t;
    double twon;
    ae_vector a;
    ae_vector c;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *sn = 0;
    *cn = 0;
    *dn = 0;
    *ph = 0;
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&c, 0, DT_REAL, _state);

    ae_assert(ae_fp_greater_eq(m,(double)(0))&&ae_fp_less_eq(m,(double)(1)), "Domain error in JacobianEllipticFunctions: m<0 or m>1", _state);
    ae_vector_set_length(&a, 8+1, _state);
    ae_vector_set_length(&c, 8+1, _state);
    if( ae_fp_less(m,1.0e-9) )
    {
        t = ae_sin(u, _state);
        b = ae_cos(u, _state);
        ai = 0.25*m*(u-t*b);
        *sn = t-ai*b;
        *cn = b+ai*t;
        *ph = u-ai;
        *dn = 1.0-0.5*m*t*t;
        ae_frame_leave(_state);
        return;
    }
    if( ae_fp_greater_eq(m,0.9999999999) )
    {
        ai = 0.25*(1.0-m);
        b = ae_cosh(u, _state);
        t = ae_tanh(u, _state);
        phi = 1.0/b;
        twon = b*ae_sinh(u, _state);
        *sn = t+ai*(twon-u)/(b*b);
        *ph = 2.0*ae_atan(ae_exp(u, _state), _state)-1.57079632679489661923+ai*(twon-u)/b;
        ai = ai*t*phi;
        *cn = phi-ai*(twon-u);
        *dn = phi+ai*(twon+u);
        ae_frame_leave(_state);
        return;
    }
    a.ptr.p_double[0] = 1.0;
    b = ae_sqrt(1.0-m, _state);
    c.ptr.p_double[0] = ae_sqrt(m, _state);
    twon = 1.0;
    i = 0;
    while(ae_fp_greater(ae_fabs(c.ptr.p_double[i]/a.ptr.p_double[i], _state),ae_machineepsilon))
    {
        if( i>7 )
        {
            ae_assert(ae_false, "Overflow in JacobianEllipticFunctions", _state);
            break;
        }
        ai = a.ptr.p_double[i];
        i = i+1;
        c.ptr.p_double[i] = 0.5*(ai-b);
        t = ae_sqrt(ai*b, _state);
        a.ptr.p_double[i] = 0.5*(ai+b);
        b = t;
        twon = twon*2.0;
    }
    phi = twon*a.ptr.p_double[i]*u;
    do
    {
        t = c.ptr.p_double[i]*ae_sin(phi, _state)/a.ptr.p_double[i];
        b = phi;
        phi = (ae_asin(t, _state)+phi)/2.0;
        i = i-1;
    }
    while(i!=0);
    *sn = ae_sin(phi, _state);
    t = ae_cos(phi, _state);
    *cn = t;
    *dn = t/ae_cos(phi-b, _state);
    *ph = phi;
    ae_frame_leave(_state);
}


/*$ End $*/
