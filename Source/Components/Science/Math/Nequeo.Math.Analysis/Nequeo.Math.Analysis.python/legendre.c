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
#include "legendre.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Calculation of the value of the Legendre polynomial Pn.

Parameters:
    n   -   degree, n>=0
    x   -   argument

Result:
    the value of the Legendre polynomial Pn at x
*************************************************************************/
double legendrecalculate(ae_int_t n, double x, ae_state *_state)
{
    double a;
    double b;
    ae_int_t i;
    double result;


    result = (double)(1);
    a = (double)(1);
    b = x;
    if( n==0 )
    {
        result = a;
        return result;
    }
    if( n==1 )
    {
        result = b;
        return result;
    }
    for(i=2; i<=n; i++)
    {
        result = ((2*i-1)*x*b-(i-1)*a)/i;
        a = b;
        b = result;
    }
    return result;
}


/*************************************************************************
Summation of Legendre polynomials using Clenshaw’s recurrence formula.

This routine calculates
    c[0]*P0(x) + c[1]*P1(x) + ... + c[N]*PN(x)

Parameters:
    n   -   degree, n>=0
    x   -   argument

Result:
    the value of the Legendre polynomial at x
*************************************************************************/
double legendresum(/* Real    */ ae_vector* c,
     ae_int_t n,
     double x,
     ae_state *_state)
{
    double b1;
    double b2;
    ae_int_t i;
    double result;


    b1 = (double)(0);
    b2 = (double)(0);
    result = (double)(0);
    for(i=n; i>=0; i--)
    {
        result = (2*i+1)*x*b1/(i+1)-(i+1)*b2/(i+2)+c->ptr.p_double[i];
        b2 = b1;
        b1 = result;
    }
    return result;
}


/*************************************************************************
Representation of Pn as C[0] + C[1]*X + ... + C[N]*X^N

Input parameters:
    N   -   polynomial degree, n>=0

Output parameters:
    C   -   coefficients
*************************************************************************/
void legendrecoefficients(ae_int_t n,
     /* Real    */ ae_vector* c,
     ae_state *_state)
{
    ae_int_t i;

    ae_vector_clear(c);

    ae_vector_set_length(c, n+1, _state);
    for(i=0; i<=n; i++)
    {
        c->ptr.p_double[i] = (double)(0);
    }
    c->ptr.p_double[n] = (double)(1);
    for(i=1; i<=n; i++)
    {
        c->ptr.p_double[n] = c->ptr.p_double[n]*(n+i)/2/i;
    }
    for(i=0; i<=n/2-1; i++)
    {
        c->ptr.p_double[n-2*(i+1)] = -c->ptr.p_double[n-2*i]*(n-2*i)*(n-2*i-1)/2/(i+1)/(2*(n-i)-1);
    }
}


/*$ End $*/
