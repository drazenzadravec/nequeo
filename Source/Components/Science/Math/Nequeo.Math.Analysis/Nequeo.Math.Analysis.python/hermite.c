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
#include "hermite.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Calculation of the value of the Hermite polynomial.

Parameters:
    n   -   degree, n>=0
    x   -   argument

Result:
    the value of the Hermite polynomial Hn at x
*************************************************************************/
double hermitecalculate(ae_int_t n, double x, ae_state *_state)
{
    ae_int_t i;
    double a;
    double b;
    double result;


    result = (double)(0);
    
    /*
     * Prepare A and B
     */
    a = (double)(1);
    b = 2*x;
    
    /*
     * Special cases: N=0 or N=1
     */
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
    
    /*
     * General case: N>=2
     */
    for(i=2; i<=n; i++)
    {
        result = 2*x*b-2*(i-1)*a;
        a = b;
        b = result;
    }
    return result;
}


/*************************************************************************
Summation of Hermite polynomials using Clenshaw’s recurrence formula.

This routine calculates
    c[0]*H0(x) + c[1]*H1(x) + ... + c[N]*HN(x)

Parameters:
    n   -   degree, n>=0
    x   -   argument

Result:
    the value of the Hermite polynomial at x
*************************************************************************/
double hermitesum(/* Real    */ ae_vector* c,
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
        result = 2*(x*b1-(i+1)*b2)+c->ptr.p_double[i];
        b2 = b1;
        b1 = result;
    }
    return result;
}


/*************************************************************************
Representation of Hn as C[0] + C[1]*X + ... + C[N]*X^N

Input parameters:
    N   -   polynomial degree, n>=0

Output parameters:
    C   -   coefficients
*************************************************************************/
void hermitecoefficients(ae_int_t n,
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
    c->ptr.p_double[n] = ae_exp(n*ae_log((double)(2), _state), _state);
    for(i=0; i<=n/2-1; i++)
    {
        c->ptr.p_double[n-2*(i+1)] = -c->ptr.p_double[n-2*i]*(n-2*i)*(n-2*i-1)/4/(i+1);
    }
}


/*$ End $*/
