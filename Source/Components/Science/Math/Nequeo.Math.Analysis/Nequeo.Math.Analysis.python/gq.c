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
#include "gq.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Computation of nodes and weights for a Gauss quadrature formula

The algorithm generates the N-point Gauss quadrature formula  with  weight
function given by coefficients alpha and beta  of  a  recurrence  relation
which generates a system of orthogonal polynomials:

P-1(x)   =  0
P0(x)    =  1
Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

and zeroth moment Mu0

Mu0 = integral(W(x)dx,a,b)

INPUT PARAMETERS:
    Alpha   –   array[0..N-1], alpha coefficients
    Beta    –   array[0..N-1], beta coefficients
                Zero-indexed element is not used and may be arbitrary.
                Beta[I]>0.
    Mu0     –   zeroth moment of the weight function.
    N       –   number of nodes of the quadrature formula, N>=1

OUTPUT PARAMETERS:
    Info    -   error code:
                * -3    internal eigenproblem solver hasn't converged
                * -2    Beta[i]<=0
                * -1    incorrect N was passed
                *  1    OK
    X       -   array[0..N-1] - array of quadrature nodes,
                in ascending order.
    W       -   array[0..N-1] - array of quadrature weights.

  -- ALGLIB --
     Copyright 2005-2009 by Bochkanov Sergey
*************************************************************************/
void gqgeneraterec(/* Real    */ ae_vector* alpha,
     /* Real    */ ae_vector* beta,
     double mu0,
     ae_int_t n,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector d;
    ae_vector e;
    ae_matrix z;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_clear(x);
    ae_vector_clear(w);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_REAL, _state);

    if( n<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    
    /*
     * Initialize
     */
    ae_vector_set_length(&d, n, _state);
    ae_vector_set_length(&e, n, _state);
    for(i=1; i<=n-1; i++)
    {
        d.ptr.p_double[i-1] = alpha->ptr.p_double[i-1];
        if( ae_fp_less_eq(beta->ptr.p_double[i],(double)(0)) )
        {
            *info = -2;
            ae_frame_leave(_state);
            return;
        }
        e.ptr.p_double[i-1] = ae_sqrt(beta->ptr.p_double[i], _state);
    }
    d.ptr.p_double[n-1] = alpha->ptr.p_double[n-1];
    
    /*
     * EVD
     */
    if( !smatrixtdevd(&d, &e, n, 3, &z, _state) )
    {
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Generate
     */
    ae_vector_set_length(x, n, _state);
    ae_vector_set_length(w, n, _state);
    for(i=1; i<=n; i++)
    {
        x->ptr.p_double[i-1] = d.ptr.p_double[i-1];
        w->ptr.p_double[i-1] = mu0*ae_sqr(z.ptr.pp_double[0][i-1], _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Computation of nodes and weights for a Gauss-Lobatto quadrature formula

The algorithm generates the N-point Gauss-Lobatto quadrature formula  with
weight function given by coefficients alpha and beta of a recurrence which
generates a system of orthogonal polynomials.

P-1(x)   =  0
P0(x)    =  1
Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

and zeroth moment Mu0

Mu0 = integral(W(x)dx,a,b)

INPUT PARAMETERS:
    Alpha   –   array[0..N-2], alpha coefficients
    Beta    –   array[0..N-2], beta coefficients.
                Zero-indexed element is not used, may be arbitrary.
                Beta[I]>0
    Mu0     –   zeroth moment of the weighting function.
    A       –   left boundary of the integration interval.
    B       –   right boundary of the integration interval.
    N       –   number of nodes of the quadrature formula, N>=3
                (including the left and right boundary nodes).

OUTPUT PARAMETERS:
    Info    -   error code:
                * -3    internal eigenproblem solver hasn't converged
                * -2    Beta[i]<=0
                * -1    incorrect N was passed
                *  1    OK
    X       -   array[0..N-1] - array of quadrature nodes,
                in ascending order.
    W       -   array[0..N-1] - array of quadrature weights.

  -- ALGLIB --
     Copyright 2005-2009 by Bochkanov Sergey
*************************************************************************/
void gqgenerategausslobattorec(/* Real    */ ae_vector* alpha,
     /* Real    */ ae_vector* beta,
     double mu0,
     double a,
     double b,
     ae_int_t n,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _alpha;
    ae_vector _beta;
    ae_int_t i;
    ae_vector d;
    ae_vector e;
    ae_matrix z;
    double pim1a;
    double pia;
    double pim1b;
    double pib;
    double t;
    double a11;
    double a12;
    double a21;
    double a22;
    double b1;
    double b2;
    double alph;
    double bet;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_alpha, alpha, _state);
    alpha = &_alpha;
    ae_vector_init_copy(&_beta, beta, _state);
    beta = &_beta;
    *info = 0;
    ae_vector_clear(x);
    ae_vector_clear(w);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_REAL, _state);

    if( n<=2 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    
    /*
     * Initialize, D[1:N+1], E[1:N]
     */
    n = n-2;
    ae_vector_set_length(&d, n+2, _state);
    ae_vector_set_length(&e, n+1, _state);
    for(i=1; i<=n+1; i++)
    {
        d.ptr.p_double[i-1] = alpha->ptr.p_double[i-1];
    }
    for(i=1; i<=n; i++)
    {
        if( ae_fp_less_eq(beta->ptr.p_double[i],(double)(0)) )
        {
            *info = -2;
            ae_frame_leave(_state);
            return;
        }
        e.ptr.p_double[i-1] = ae_sqrt(beta->ptr.p_double[i], _state);
    }
    
    /*
     * Caclulate Pn(a), Pn+1(a), Pn(b), Pn+1(b)
     */
    beta->ptr.p_double[0] = (double)(0);
    pim1a = (double)(0);
    pia = (double)(1);
    pim1b = (double)(0);
    pib = (double)(1);
    for(i=1; i<=n+1; i++)
    {
        
        /*
         * Pi(a)
         */
        t = (a-alpha->ptr.p_double[i-1])*pia-beta->ptr.p_double[i-1]*pim1a;
        pim1a = pia;
        pia = t;
        
        /*
         * Pi(b)
         */
        t = (b-alpha->ptr.p_double[i-1])*pib-beta->ptr.p_double[i-1]*pim1b;
        pim1b = pib;
        pib = t;
    }
    
    /*
     * Calculate alpha'(n+1), beta'(n+1)
     */
    a11 = pia;
    a12 = pim1a;
    a21 = pib;
    a22 = pim1b;
    b1 = a*pia;
    b2 = b*pib;
    if( ae_fp_greater(ae_fabs(a11, _state),ae_fabs(a21, _state)) )
    {
        a22 = a22-a12*a21/a11;
        b2 = b2-b1*a21/a11;
        bet = b2/a22;
        alph = (b1-bet*a12)/a11;
    }
    else
    {
        a12 = a12-a22*a11/a21;
        b1 = b1-b2*a11/a21;
        bet = b1/a12;
        alph = (b2-bet*a22)/a21;
    }
    if( ae_fp_less(bet,(double)(0)) )
    {
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    d.ptr.p_double[n+1] = alph;
    e.ptr.p_double[n] = ae_sqrt(bet, _state);
    
    /*
     * EVD
     */
    if( !smatrixtdevd(&d, &e, n+2, 3, &z, _state) )
    {
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Generate
     */
    ae_vector_set_length(x, n+2, _state);
    ae_vector_set_length(w, n+2, _state);
    for(i=1; i<=n+2; i++)
    {
        x->ptr.p_double[i-1] = d.ptr.p_double[i-1];
        w->ptr.p_double[i-1] = mu0*ae_sqr(z.ptr.pp_double[0][i-1], _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Computation of nodes and weights for a Gauss-Radau quadrature formula

The algorithm generates the N-point Gauss-Radau  quadrature  formula  with
weight function given by the coefficients alpha and  beta  of a recurrence
which generates a system of orthogonal polynomials.

P-1(x)   =  0
P0(x)    =  1
Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

and zeroth moment Mu0

Mu0 = integral(W(x)dx,a,b)

INPUT PARAMETERS:
    Alpha   –   array[0..N-2], alpha coefficients.
    Beta    –   array[0..N-1], beta coefficients
                Zero-indexed element is not used.
                Beta[I]>0
    Mu0     –   zeroth moment of the weighting function.
    A       –   left boundary of the integration interval.
    N       –   number of nodes of the quadrature formula, N>=2
                (including the left boundary node).

OUTPUT PARAMETERS:
    Info    -   error code:
                * -3    internal eigenproblem solver hasn't converged
                * -2    Beta[i]<=0
                * -1    incorrect N was passed
                *  1    OK
    X       -   array[0..N-1] - array of quadrature nodes,
                in ascending order.
    W       -   array[0..N-1] - array of quadrature weights.


  -- ALGLIB --
     Copyright 2005-2009 by Bochkanov Sergey
*************************************************************************/
void gqgenerategaussradaurec(/* Real    */ ae_vector* alpha,
     /* Real    */ ae_vector* beta,
     double mu0,
     double a,
     ae_int_t n,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _alpha;
    ae_vector _beta;
    ae_int_t i;
    ae_vector d;
    ae_vector e;
    ae_matrix z;
    double polim1;
    double poli;
    double t;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_alpha, alpha, _state);
    alpha = &_alpha;
    ae_vector_init_copy(&_beta, beta, _state);
    beta = &_beta;
    *info = 0;
    ae_vector_clear(x);
    ae_vector_clear(w);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_REAL, _state);

    if( n<2 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    
    /*
     * Initialize, D[1:N], E[1:N]
     */
    n = n-1;
    ae_vector_set_length(&d, n+1, _state);
    ae_vector_set_length(&e, n, _state);
    for(i=1; i<=n; i++)
    {
        d.ptr.p_double[i-1] = alpha->ptr.p_double[i-1];
        if( ae_fp_less_eq(beta->ptr.p_double[i],(double)(0)) )
        {
            *info = -2;
            ae_frame_leave(_state);
            return;
        }
        e.ptr.p_double[i-1] = ae_sqrt(beta->ptr.p_double[i], _state);
    }
    
    /*
     * Caclulate Pn(a), Pn-1(a), and D[N+1]
     */
    beta->ptr.p_double[0] = (double)(0);
    polim1 = (double)(0);
    poli = (double)(1);
    for(i=1; i<=n; i++)
    {
        t = (a-alpha->ptr.p_double[i-1])*poli-beta->ptr.p_double[i-1]*polim1;
        polim1 = poli;
        poli = t;
    }
    d.ptr.p_double[n] = a-beta->ptr.p_double[n]*polim1/poli;
    
    /*
     * EVD
     */
    if( !smatrixtdevd(&d, &e, n+1, 3, &z, _state) )
    {
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Generate
     */
    ae_vector_set_length(x, n+1, _state);
    ae_vector_set_length(w, n+1, _state);
    for(i=1; i<=n+1; i++)
    {
        x->ptr.p_double[i-1] = d.ptr.p_double[i-1];
        w->ptr.p_double[i-1] = mu0*ae_sqr(z.ptr.pp_double[0][i-1], _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Returns nodes/weights for Gauss-Legendre quadrature on [-1,1] with N
nodes.

INPUT PARAMETERS:
    N           -   number of nodes, >=1

OUTPUT PARAMETERS:
    Info        -   error code:
                    * -4    an  error   was   detected   when  calculating
                            weights/nodes.  N  is  too  large   to  obtain
                            weights/nodes  with  high   enough   accuracy.
                            Try  to   use   multiple   precision  version.
                    * -3    internal eigenproblem solver hasn't  converged
                    * -1    incorrect N was passed
                    * +1    OK
    X           -   array[0..N-1] - array of quadrature nodes,
                    in ascending order.
    W           -   array[0..N-1] - array of quadrature weights.


  -- ALGLIB --
     Copyright 12.05.2009 by Bochkanov Sergey
*************************************************************************/
void gqgenerategausslegendre(ae_int_t n,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector alpha;
    ae_vector beta;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_clear(x);
    ae_vector_clear(w);
    ae_vector_init(&alpha, 0, DT_REAL, _state);
    ae_vector_init(&beta, 0, DT_REAL, _state);

    if( n<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(&alpha, n, _state);
    ae_vector_set_length(&beta, n, _state);
    for(i=0; i<=n-1; i++)
    {
        alpha.ptr.p_double[i] = (double)(0);
    }
    beta.ptr.p_double[0] = (double)(2);
    for(i=1; i<=n-1; i++)
    {
        beta.ptr.p_double[i] = 1/(4-1/ae_sqr((double)(i), _state));
    }
    gqgeneraterec(&alpha, &beta, beta.ptr.p_double[0], n, info, x, w, _state);
    
    /*
     * test basic properties to detect errors
     */
    if( *info>0 )
    {
        if( ae_fp_less(x->ptr.p_double[0],(double)(-1))||ae_fp_greater(x->ptr.p_double[n-1],(double)(1)) )
        {
            *info = -4;
        }
        for(i=0; i<=n-2; i++)
        {
            if( ae_fp_greater_eq(x->ptr.p_double[i],x->ptr.p_double[i+1]) )
            {
                *info = -4;
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Returns  nodes/weights  for  Gauss-Jacobi quadrature on [-1,1] with weight
function W(x)=Power(1-x,Alpha)*Power(1+x,Beta).

INPUT PARAMETERS:
    N           -   number of nodes, >=1
    Alpha       -   power-law coefficient, Alpha>-1
    Beta        -   power-law coefficient, Beta>-1

OUTPUT PARAMETERS:
    Info        -   error code:
                    * -4    an  error  was   detected   when   calculating
                            weights/nodes. Alpha or  Beta  are  too  close
                            to -1 to obtain weights/nodes with high enough
                            accuracy, or, may be, N is too large.  Try  to
                            use multiple precision version.
                    * -3    internal eigenproblem solver hasn't converged
                    * -1    incorrect N/Alpha/Beta was passed
                    * +1    OK
    X           -   array[0..N-1] - array of quadrature nodes,
                    in ascending order.
    W           -   array[0..N-1] - array of quadrature weights.


  -- ALGLIB --
     Copyright 12.05.2009 by Bochkanov Sergey
*************************************************************************/
void gqgenerategaussjacobi(ae_int_t n,
     double alpha,
     double beta,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector a;
    ae_vector b;
    double alpha2;
    double beta2;
    double apb;
    double t;
    ae_int_t i;
    double s;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_clear(x);
    ae_vector_clear(w);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);

    if( (n<1||ae_fp_less_eq(alpha,(double)(-1)))||ae_fp_less_eq(beta,(double)(-1)) )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(&a, n, _state);
    ae_vector_set_length(&b, n, _state);
    apb = alpha+beta;
    a.ptr.p_double[0] = (beta-alpha)/(apb+2);
    t = (apb+1)*ae_log((double)(2), _state)+lngamma(alpha+1, &s, _state)+lngamma(beta+1, &s, _state)-lngamma(apb+2, &s, _state);
    if( ae_fp_greater(t,ae_log(ae_maxrealnumber, _state)) )
    {
        *info = -4;
        ae_frame_leave(_state);
        return;
    }
    b.ptr.p_double[0] = ae_exp(t, _state);
    if( n>1 )
    {
        alpha2 = ae_sqr(alpha, _state);
        beta2 = ae_sqr(beta, _state);
        a.ptr.p_double[1] = (beta2-alpha2)/((apb+2)*(apb+4));
        b.ptr.p_double[1] = 4*(alpha+1)*(beta+1)/((apb+3)*ae_sqr(apb+2, _state));
        for(i=2; i<=n-1; i++)
        {
            a.ptr.p_double[i] = 0.25*(beta2-alpha2)/(i*i*(1+0.5*apb/i)*(1+0.5*(apb+2)/i));
            b.ptr.p_double[i] = 0.25*(1+alpha/i)*(1+beta/i)*(1+apb/i)/((1+0.5*(apb+1)/i)*(1+0.5*(apb-1)/i)*ae_sqr(1+0.5*apb/i, _state));
        }
    }
    gqgeneraterec(&a, &b, b.ptr.p_double[0], n, info, x, w, _state);
    
    /*
     * test basic properties to detect errors
     */
    if( *info>0 )
    {
        if( ae_fp_less(x->ptr.p_double[0],(double)(-1))||ae_fp_greater(x->ptr.p_double[n-1],(double)(1)) )
        {
            *info = -4;
        }
        for(i=0; i<=n-2; i++)
        {
            if( ae_fp_greater_eq(x->ptr.p_double[i],x->ptr.p_double[i+1]) )
            {
                *info = -4;
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Returns  nodes/weights  for  Gauss-Laguerre  quadrature  on  [0,+inf) with
weight function W(x)=Power(x,Alpha)*Exp(-x)

INPUT PARAMETERS:
    N           -   number of nodes, >=1
    Alpha       -   power-law coefficient, Alpha>-1

OUTPUT PARAMETERS:
    Info        -   error code:
                    * -4    an  error  was   detected   when   calculating
                            weights/nodes. Alpha is too  close  to  -1  to
                            obtain weights/nodes with high enough accuracy
                            or, may  be,  N  is  too  large.  Try  to  use
                            multiple precision version.
                    * -3    internal eigenproblem solver hasn't converged
                    * -1    incorrect N/Alpha was passed
                    * +1    OK
    X           -   array[0..N-1] - array of quadrature nodes,
                    in ascending order.
    W           -   array[0..N-1] - array of quadrature weights.


  -- ALGLIB --
     Copyright 12.05.2009 by Bochkanov Sergey
*************************************************************************/
void gqgenerategausslaguerre(ae_int_t n,
     double alpha,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector a;
    ae_vector b;
    double t;
    ae_int_t i;
    double s;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_clear(x);
    ae_vector_clear(w);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);

    if( n<1||ae_fp_less_eq(alpha,(double)(-1)) )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(&a, n, _state);
    ae_vector_set_length(&b, n, _state);
    a.ptr.p_double[0] = alpha+1;
    t = lngamma(alpha+1, &s, _state);
    if( ae_fp_greater_eq(t,ae_log(ae_maxrealnumber, _state)) )
    {
        *info = -4;
        ae_frame_leave(_state);
        return;
    }
    b.ptr.p_double[0] = ae_exp(t, _state);
    if( n>1 )
    {
        for(i=1; i<=n-1; i++)
        {
            a.ptr.p_double[i] = 2*i+alpha+1;
            b.ptr.p_double[i] = i*(i+alpha);
        }
    }
    gqgeneraterec(&a, &b, b.ptr.p_double[0], n, info, x, w, _state);
    
    /*
     * test basic properties to detect errors
     */
    if( *info>0 )
    {
        if( ae_fp_less(x->ptr.p_double[0],(double)(0)) )
        {
            *info = -4;
        }
        for(i=0; i<=n-2; i++)
        {
            if( ae_fp_greater_eq(x->ptr.p_double[i],x->ptr.p_double[i+1]) )
            {
                *info = -4;
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Returns  nodes/weights  for  Gauss-Hermite  quadrature on (-inf,+inf) with
weight function W(x)=Exp(-x*x)

INPUT PARAMETERS:
    N           -   number of nodes, >=1

OUTPUT PARAMETERS:
    Info        -   error code:
                    * -4    an  error  was   detected   when   calculating
                            weights/nodes.  May be, N is too large. Try to
                            use multiple precision version.
                    * -3    internal eigenproblem solver hasn't converged
                    * -1    incorrect N/Alpha was passed
                    * +1    OK
    X           -   array[0..N-1] - array of quadrature nodes,
                    in ascending order.
    W           -   array[0..N-1] - array of quadrature weights.


  -- ALGLIB --
     Copyright 12.05.2009 by Bochkanov Sergey
*************************************************************************/
void gqgenerategausshermite(ae_int_t n,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector a;
    ae_vector b;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_clear(x);
    ae_vector_clear(w);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);

    if( n<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(&a, n, _state);
    ae_vector_set_length(&b, n, _state);
    for(i=0; i<=n-1; i++)
    {
        a.ptr.p_double[i] = (double)(0);
    }
    b.ptr.p_double[0] = ae_sqrt(4*ae_atan((double)(1), _state), _state);
    if( n>1 )
    {
        for(i=1; i<=n-1; i++)
        {
            b.ptr.p_double[i] = 0.5*i;
        }
    }
    gqgeneraterec(&a, &b, b.ptr.p_double[0], n, info, x, w, _state);
    
    /*
     * test basic properties to detect errors
     */
    if( *info>0 )
    {
        for(i=0; i<=n-2; i++)
        {
            if( ae_fp_greater_eq(x->ptr.p_double[i],x->ptr.p_double[i+1]) )
            {
                *info = -4;
            }
        }
    }
    ae_frame_leave(_state);
}


/*$ End $*/
