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
#include "chebyshev.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Calculation of the value of the Chebyshev polynomials of the
first and second kinds.

Parameters:
    r   -   polynomial kind, either 1 or 2.
    n   -   degree, n>=0
    x   -   argument, -1 <= x <= 1

Result:
    the value of the Chebyshev polynomial at x
*************************************************************************/
double chebyshevcalculate(ae_int_t r,
     ae_int_t n,
     double x,
     ae_state *_state)
{
    ae_int_t i;
    double a;
    double b;
    double result;


    result = (double)(0);
    
    /*
     * Prepare A and B
     */
    if( r==1 )
    {
        a = (double)(1);
        b = x;
    }
    else
    {
        a = (double)(1);
        b = 2*x;
    }
    
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
        result = 2*x*b-a;
        a = b;
        b = result;
    }
    return result;
}


/*************************************************************************
Summation of Chebyshev polynomials using Clenshaw’s recurrence formula.

This routine calculates
    c[0]*T0(x) + c[1]*T1(x) + ... + c[N]*TN(x)
or
    c[0]*U0(x) + c[1]*U1(x) + ... + c[N]*UN(x)
depending on the R.

Parameters:
    r   -   polynomial kind, either 1 or 2.
    n   -   degree, n>=0
    x   -   argument

Result:
    the value of the Chebyshev polynomial at x
*************************************************************************/
double chebyshevsum(/* Real    */ ae_vector* c,
     ae_int_t r,
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
    for(i=n; i>=1; i--)
    {
        result = 2*x*b1-b2+c->ptr.p_double[i];
        b2 = b1;
        b1 = result;
    }
    if( r==1 )
    {
        result = -b2+x*b1+c->ptr.p_double[0];
    }
    else
    {
        result = -b2+2*x*b1+c->ptr.p_double[0];
    }
    return result;
}


/*************************************************************************
Representation of Tn as C[0] + C[1]*X + ... + C[N]*X^N

Input parameters:
    N   -   polynomial degree, n>=0

Output parameters:
    C   -   coefficients
*************************************************************************/
void chebyshevcoefficients(ae_int_t n,
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
    if( n==0||n==1 )
    {
        c->ptr.p_double[n] = (double)(1);
    }
    else
    {
        c->ptr.p_double[n] = ae_exp((n-1)*ae_log((double)(2), _state), _state);
        for(i=0; i<=n/2-1; i++)
        {
            c->ptr.p_double[n-2*(i+1)] = -c->ptr.p_double[n-2*i]*(n-2*i)*(n-2*i-1)/4/(i+1)/(n-i-1);
        }
    }
}


/*************************************************************************
Conversion of a series of Chebyshev polynomials to a power series.

Represents A[0]*T0(x) + A[1]*T1(x) + ... + A[N]*Tn(x) as
B[0] + B[1]*X + ... + B[N]*X^N.

Input parameters:
    A   -   Chebyshev series coefficients
    N   -   degree, N>=0
    
Output parameters
    B   -   power series coefficients
*************************************************************************/
void fromchebyshev(/* Real    */ ae_vector* a,
     ae_int_t n,
     /* Real    */ ae_vector* b,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    double e;
    double d;

    ae_vector_clear(b);

    ae_vector_set_length(b, n+1, _state);
    for(i=0; i<=n; i++)
    {
        b->ptr.p_double[i] = (double)(0);
    }
    d = (double)(0);
    i = 0;
    do
    {
        k = i;
        do
        {
            e = b->ptr.p_double[k];
            b->ptr.p_double[k] = (double)(0);
            if( i<=1&&k==i )
            {
                b->ptr.p_double[k] = (double)(1);
            }
            else
            {
                if( i!=0 )
                {
                    b->ptr.p_double[k] = 2*d;
                }
                if( k>i+1 )
                {
                    b->ptr.p_double[k] = b->ptr.p_double[k]-b->ptr.p_double[k-2];
                }
            }
            d = e;
            k = k+1;
        }
        while(k<=n);
        d = b->ptr.p_double[i];
        e = (double)(0);
        k = i;
        while(k<=n)
        {
            e = e+b->ptr.p_double[k]*a->ptr.p_double[k];
            k = k+2;
        }
        b->ptr.p_double[i] = e;
        i = i+1;
    }
    while(i<=n);
}


/*$ End $*/
