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
#include "gkq.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Computation of nodes and weights of a Gauss-Kronrod quadrature formula

The algorithm generates the N-point Gauss-Kronrod quadrature formula  with
weight  function  given  by  coefficients  alpha  and beta of a recurrence
relation which generates a system of orthogonal polynomials:

    P-1(x)   =  0
    P0(x)    =  1
    Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

and zero moment Mu0

    Mu0 = integral(W(x)dx,a,b)


INPUT PARAMETERS:
    Alpha       –   alpha coefficients, array[0..floor(3*K/2)].
    Beta        –   beta coefficients,  array[0..ceil(3*K/2)].
                    Beta[0] is not used and may be arbitrary.
                    Beta[I]>0.
    Mu0         –   zeroth moment of the weight function.
    N           –   number of nodes of the Gauss-Kronrod quadrature formula,
                    N >= 3,
                    N =  2*K+1.

OUTPUT PARAMETERS:
    Info        -   error code:
                    * -5    no real and positive Gauss-Kronrod formula can
                            be created for such a weight function  with  a
                            given number of nodes.
                    * -4    N is too large, task may be ill  conditioned -
                            x[i]=x[i+1] found.
                    * -3    internal eigenproblem solver hasn't converged
                    * -2    Beta[i]<=0
                    * -1    incorrect N was passed
                    * +1    OK
    X           -   array[0..N-1] - array of quadrature nodes,
                    in ascending order.
    WKronrod    -   array[0..N-1] - Kronrod weights
    WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                    corresponding to extended Kronrod nodes).

  -- ALGLIB --
     Copyright 08.05.2009 by Bochkanov Sergey
*************************************************************************/
void gkqgeneraterec(/* Real    */ ae_vector* alpha,
     /* Real    */ ae_vector* beta,
     double mu0,
     ae_int_t n,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* wkronrod,
     /* Real    */ ae_vector* wgauss,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _alpha;
    ae_vector _beta;
    ae_vector ta;
    ae_int_t i;
    ae_int_t j;
    ae_vector t;
    ae_vector s;
    ae_int_t wlen;
    ae_int_t woffs;
    double u;
    ae_int_t m;
    ae_int_t l;
    ae_int_t k;
    ae_vector xgtmp;
    ae_vector wgtmp;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_alpha, alpha, _state);
    alpha = &_alpha;
    ae_vector_init_copy(&_beta, beta, _state);
    beta = &_beta;
    *info = 0;
    ae_vector_clear(x);
    ae_vector_clear(wkronrod);
    ae_vector_clear(wgauss);
    ae_vector_init(&ta, 0, DT_REAL, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&xgtmp, 0, DT_REAL, _state);
    ae_vector_init(&wgtmp, 0, DT_REAL, _state);

    if( n%2!=1||n<3 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=ae_iceil((double)(3*(n/2))/(double)2, _state); i++)
    {
        if( ae_fp_less_eq(beta->ptr.p_double[i],(double)(0)) )
        {
            *info = -2;
            ae_frame_leave(_state);
            return;
        }
    }
    *info = 1;
    
    /*
     * from external conventions about N/Beta/Mu0 to internal
     */
    n = n/2;
    beta->ptr.p_double[0] = mu0;
    
    /*
     * Calculate Gauss nodes/weights, save them for later processing
     */
    gqgeneraterec(alpha, beta, mu0, n, info, &xgtmp, &wgtmp, _state);
    if( *info<0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Resize:
     * * A from 0..floor(3*n/2) to 0..2*n
     * * B from 0..ceil(3*n/2)  to 0..2*n
     */
    ae_vector_set_length(&ta, ae_ifloor((double)(3*n)/(double)2, _state)+1, _state);
    ae_v_move(&ta.ptr.p_double[0], 1, &alpha->ptr.p_double[0], 1, ae_v_len(0,ae_ifloor((double)(3*n)/(double)2, _state)));
    ae_vector_set_length(alpha, 2*n+1, _state);
    ae_v_move(&alpha->ptr.p_double[0], 1, &ta.ptr.p_double[0], 1, ae_v_len(0,ae_ifloor((double)(3*n)/(double)2, _state)));
    for(i=ae_ifloor((double)(3*n)/(double)2, _state)+1; i<=2*n; i++)
    {
        alpha->ptr.p_double[i] = (double)(0);
    }
    ae_vector_set_length(&ta, ae_iceil((double)(3*n)/(double)2, _state)+1, _state);
    ae_v_move(&ta.ptr.p_double[0], 1, &beta->ptr.p_double[0], 1, ae_v_len(0,ae_iceil((double)(3*n)/(double)2, _state)));
    ae_vector_set_length(beta, 2*n+1, _state);
    ae_v_move(&beta->ptr.p_double[0], 1, &ta.ptr.p_double[0], 1, ae_v_len(0,ae_iceil((double)(3*n)/(double)2, _state)));
    for(i=ae_iceil((double)(3*n)/(double)2, _state)+1; i<=2*n; i++)
    {
        beta->ptr.p_double[i] = (double)(0);
    }
    
    /*
     * Initialize T, S
     */
    wlen = 2+n/2;
    ae_vector_set_length(&t, wlen, _state);
    ae_vector_set_length(&s, wlen, _state);
    ae_vector_set_length(&ta, wlen, _state);
    woffs = 1;
    for(i=0; i<=wlen-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
        s.ptr.p_double[i] = (double)(0);
    }
    
    /*
     * Algorithm from Dirk P. Laurie, "Calculation of Gauss-Kronrod quadrature rules", 1997.
     */
    t.ptr.p_double[woffs+0] = beta->ptr.p_double[n+1];
    for(m=0; m<=n-2; m++)
    {
        u = (double)(0);
        for(k=(m+1)/2; k>=0; k--)
        {
            l = m-k;
            u = u+(alpha->ptr.p_double[k+n+1]-alpha->ptr.p_double[l])*t.ptr.p_double[woffs+k]+beta->ptr.p_double[k+n+1]*s.ptr.p_double[woffs+k-1]-beta->ptr.p_double[l]*s.ptr.p_double[woffs+k];
            s.ptr.p_double[woffs+k] = u;
        }
        ae_v_move(&ta.ptr.p_double[0], 1, &t.ptr.p_double[0], 1, ae_v_len(0,wlen-1));
        ae_v_move(&t.ptr.p_double[0], 1, &s.ptr.p_double[0], 1, ae_v_len(0,wlen-1));
        ae_v_move(&s.ptr.p_double[0], 1, &ta.ptr.p_double[0], 1, ae_v_len(0,wlen-1));
    }
    for(j=n/2; j>=0; j--)
    {
        s.ptr.p_double[woffs+j] = s.ptr.p_double[woffs+j-1];
    }
    for(m=n-1; m<=2*n-3; m++)
    {
        u = (double)(0);
        for(k=m+1-n; k<=(m-1)/2; k++)
        {
            l = m-k;
            j = n-1-l;
            u = u-(alpha->ptr.p_double[k+n+1]-alpha->ptr.p_double[l])*t.ptr.p_double[woffs+j]-beta->ptr.p_double[k+n+1]*s.ptr.p_double[woffs+j]+beta->ptr.p_double[l]*s.ptr.p_double[woffs+j+1];
            s.ptr.p_double[woffs+j] = u;
        }
        if( m%2==0 )
        {
            k = m/2;
            alpha->ptr.p_double[k+n+1] = alpha->ptr.p_double[k]+(s.ptr.p_double[woffs+j]-beta->ptr.p_double[k+n+1]*s.ptr.p_double[woffs+j+1])/t.ptr.p_double[woffs+j+1];
        }
        else
        {
            k = (m+1)/2;
            beta->ptr.p_double[k+n+1] = s.ptr.p_double[woffs+j]/s.ptr.p_double[woffs+j+1];
        }
        ae_v_move(&ta.ptr.p_double[0], 1, &t.ptr.p_double[0], 1, ae_v_len(0,wlen-1));
        ae_v_move(&t.ptr.p_double[0], 1, &s.ptr.p_double[0], 1, ae_v_len(0,wlen-1));
        ae_v_move(&s.ptr.p_double[0], 1, &ta.ptr.p_double[0], 1, ae_v_len(0,wlen-1));
    }
    alpha->ptr.p_double[2*n] = alpha->ptr.p_double[n-1]-beta->ptr.p_double[2*n]*s.ptr.p_double[woffs+0]/t.ptr.p_double[woffs+0];
    
    /*
     * calculation of Kronrod nodes and weights, unpacking of Gauss weights
     */
    gqgeneraterec(alpha, beta, mu0, 2*n+1, info, x, wkronrod, _state);
    if( *info==-2 )
    {
        *info = -5;
    }
    if( *info<0 )
    {
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=2*n-1; i++)
    {
        if( ae_fp_greater_eq(x->ptr.p_double[i],x->ptr.p_double[i+1]) )
        {
            *info = -4;
        }
    }
    if( *info<0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(wgauss, 2*n+1, _state);
    for(i=0; i<=2*n; i++)
    {
        wgauss->ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        wgauss->ptr.p_double[2*i+1] = wgtmp.ptr.p_double[i];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Returns   Gauss   and   Gauss-Kronrod   nodes/weights  for  Gauss-Legendre
quadrature with N points.

GKQLegendreCalc (calculation) or  GKQLegendreTbl  (precomputed  table)  is
used depending on machine precision and number of nodes.

INPUT PARAMETERS:
    N           -   number of Kronrod nodes, must be odd number, >=3.

OUTPUT PARAMETERS:
    Info        -   error code:
                    * -4    an  error   was   detected   when  calculating
                            weights/nodes.  N  is  too  large   to  obtain
                            weights/nodes  with  high   enough   accuracy.
                            Try  to   use   multiple   precision  version.
                    * -3    internal eigenproblem solver hasn't converged
                    * -1    incorrect N was passed
                    * +1    OK
    X           -   array[0..N-1] - array of quadrature nodes, ordered in
                    ascending order.
    WKronrod    -   array[0..N-1] - Kronrod weights
    WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                    corresponding to extended Kronrod nodes).


  -- ALGLIB --
     Copyright 12.05.2009 by Bochkanov Sergey
*************************************************************************/
void gkqgenerategausslegendre(ae_int_t n,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* wkronrod,
     /* Real    */ ae_vector* wgauss,
     ae_state *_state)
{
    double eps;

    *info = 0;
    ae_vector_clear(x);
    ae_vector_clear(wkronrod);
    ae_vector_clear(wgauss);

    if( ae_fp_greater(ae_machineepsilon,1.0E-32)&&(((((n==15||n==21)||n==31)||n==41)||n==51)||n==61) )
    {
        *info = 1;
        gkqlegendretbl(n, x, wkronrod, wgauss, &eps, _state);
    }
    else
    {
        gkqlegendrecalc(n, info, x, wkronrod, wgauss, _state);
    }
}


/*************************************************************************
Returns   Gauss   and   Gauss-Kronrod   nodes/weights   for   Gauss-Jacobi
quadrature on [-1,1] with weight function

    W(x)=Power(1-x,Alpha)*Power(1+x,Beta).

INPUT PARAMETERS:
    N           -   number of Kronrod nodes, must be odd number, >=3.
    Alpha       -   power-law coefficient, Alpha>-1
    Beta        -   power-law coefficient, Beta>-1

OUTPUT PARAMETERS:
    Info        -   error code:
                    * -5    no real and positive Gauss-Kronrod formula can
                            be created for such a weight function  with  a
                            given number of nodes.
                    * -4    an  error  was   detected   when   calculating
                            weights/nodes. Alpha or  Beta  are  too  close
                            to -1 to obtain weights/nodes with high enough
                            accuracy, or, may be, N is too large.  Try  to
                            use multiple precision version.
                    * -3    internal eigenproblem solver hasn't converged
                    * -1    incorrect N was passed
                    * +1    OK
                    * +2    OK, but quadrature rule have exterior  nodes,
                            x[0]<-1 or x[n-1]>+1
    X           -   array[0..N-1] - array of quadrature nodes, ordered in
                    ascending order.
    WKronrod    -   array[0..N-1] - Kronrod weights
    WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                    corresponding to extended Kronrod nodes).


  -- ALGLIB --
     Copyright 12.05.2009 by Bochkanov Sergey
*************************************************************************/
void gkqgenerategaussjacobi(ae_int_t n,
     double alpha,
     double beta,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* wkronrod,
     /* Real    */ ae_vector* wgauss,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t clen;
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
    ae_vector_clear(wkronrod);
    ae_vector_clear(wgauss);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);

    if( n%2!=1||n<3 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( ae_fp_less_eq(alpha,(double)(-1))||ae_fp_less_eq(beta,(double)(-1)) )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    clen = ae_iceil((double)(3*(n/2))/(double)2, _state)+1;
    ae_vector_set_length(&a, clen, _state);
    ae_vector_set_length(&b, clen, _state);
    for(i=0; i<=clen-1; i++)
    {
        a.ptr.p_double[i] = (double)(0);
    }
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
    if( clen>1 )
    {
        alpha2 = ae_sqr(alpha, _state);
        beta2 = ae_sqr(beta, _state);
        a.ptr.p_double[1] = (beta2-alpha2)/((apb+2)*(apb+4));
        b.ptr.p_double[1] = 4*(alpha+1)*(beta+1)/((apb+3)*ae_sqr(apb+2, _state));
        for(i=2; i<=clen-1; i++)
        {
            a.ptr.p_double[i] = 0.25*(beta2-alpha2)/(i*i*(1+0.5*apb/i)*(1+0.5*(apb+2)/i));
            b.ptr.p_double[i] = 0.25*(1+alpha/i)*(1+beta/i)*(1+apb/i)/((1+0.5*(apb+1)/i)*(1+0.5*(apb-1)/i)*ae_sqr(1+0.5*apb/i, _state));
        }
    }
    gkqgeneraterec(&a, &b, b.ptr.p_double[0], n, info, x, wkronrod, wgauss, _state);
    
    /*
     * test basic properties to detect errors
     */
    if( *info>0 )
    {
        if( ae_fp_less(x->ptr.p_double[0],(double)(-1))||ae_fp_greater(x->ptr.p_double[n-1],(double)(1)) )
        {
            *info = 2;
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
Returns Gauss and Gauss-Kronrod nodes for quadrature with N points.

Reduction to tridiagonal eigenproblem is used.

INPUT PARAMETERS:
    N           -   number of Kronrod nodes, must be odd number, >=3.

OUTPUT PARAMETERS:
    Info        -   error code:
                    * -4    an  error   was   detected   when  calculating
                            weights/nodes.  N  is  too  large   to  obtain
                            weights/nodes  with  high   enough   accuracy.
                            Try  to   use   multiple   precision  version.
                    * -3    internal eigenproblem solver hasn't converged
                    * -1    incorrect N was passed
                    * +1    OK
    X           -   array[0..N-1] - array of quadrature nodes, ordered in
                    ascending order.
    WKronrod    -   array[0..N-1] - Kronrod weights
    WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                    corresponding to extended Kronrod nodes).

  -- ALGLIB --
     Copyright 12.05.2009 by Bochkanov Sergey
*************************************************************************/
void gkqlegendrecalc(ae_int_t n,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* wkronrod,
     /* Real    */ ae_vector* wgauss,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector alpha;
    ae_vector beta;
    ae_int_t alen;
    ae_int_t blen;
    double mu0;
    ae_int_t k;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_clear(x);
    ae_vector_clear(wkronrod);
    ae_vector_clear(wgauss);
    ae_vector_init(&alpha, 0, DT_REAL, _state);
    ae_vector_init(&beta, 0, DT_REAL, _state);

    if( n%2!=1||n<3 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    mu0 = (double)(2);
    alen = ae_ifloor((double)(3*(n/2))/(double)2, _state)+1;
    blen = ae_iceil((double)(3*(n/2))/(double)2, _state)+1;
    ae_vector_set_length(&alpha, alen, _state);
    ae_vector_set_length(&beta, blen, _state);
    for(k=0; k<=alen-1; k++)
    {
        alpha.ptr.p_double[k] = (double)(0);
    }
    beta.ptr.p_double[0] = (double)(2);
    for(k=1; k<=blen-1; k++)
    {
        beta.ptr.p_double[k] = 1/(4-1/ae_sqr((double)(k), _state));
    }
    gkqgeneraterec(&alpha, &beta, mu0, n, info, x, wkronrod, wgauss, _state);
    
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
Returns Gauss and Gauss-Kronrod nodes for quadrature with N  points  using
pre-calculated table. Nodes/weights were  computed  with  accuracy  up  to
1.0E-32 (if MPFR version of ALGLIB is used). In standard double  precision
accuracy reduces to something about 2.0E-16 (depending  on your compiler's
handling of long floating point constants).

INPUT PARAMETERS:
    N           -   number of Kronrod nodes.
                    N can be 15, 21, 31, 41, 51, 61.

OUTPUT PARAMETERS:
    X           -   array[0..N-1] - array of quadrature nodes, ordered in
                    ascending order.
    WKronrod    -   array[0..N-1] - Kronrod weights
    WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                    corresponding to extended Kronrod nodes).


  -- ALGLIB --
     Copyright 12.05.2009 by Bochkanov Sergey
*************************************************************************/
void gkqlegendretbl(ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* wkronrod,
     /* Real    */ ae_vector* wgauss,
     double* eps,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t ng;
    ae_vector p1;
    ae_vector p2;
    double tmp;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(x);
    ae_vector_clear(wkronrod);
    ae_vector_clear(wgauss);
    *eps = 0;
    ae_vector_init(&p1, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);

    
    /*
     * these initializers are not really necessary,
     * but without them compiler complains about uninitialized locals
     */
    ng = 0;
    
    /*
     * Process
     */
    ae_assert(((((n==15||n==21)||n==31)||n==41)||n==51)||n==61, "GKQNodesTbl: incorrect N!", _state);
    ae_vector_set_length(x, n, _state);
    ae_vector_set_length(wkronrod, n, _state);
    ae_vector_set_length(wgauss, n, _state);
    for(i=0; i<=n-1; i++)
    {
        x->ptr.p_double[i] = (double)(0);
        wkronrod->ptr.p_double[i] = (double)(0);
        wgauss->ptr.p_double[i] = (double)(0);
    }
    *eps = ae_maxreal(ae_machineepsilon, 1.0E-32, _state);
    if( n==15 )
    {
        ng = 4;
        wgauss->ptr.p_double[0] = 0.129484966168869693270611432679082;
        wgauss->ptr.p_double[1] = 0.279705391489276667901467771423780;
        wgauss->ptr.p_double[2] = 0.381830050505118944950369775488975;
        wgauss->ptr.p_double[3] = 0.417959183673469387755102040816327;
        x->ptr.p_double[0] = 0.991455371120812639206854697526329;
        x->ptr.p_double[1] = 0.949107912342758524526189684047851;
        x->ptr.p_double[2] = 0.864864423359769072789712788640926;
        x->ptr.p_double[3] = 0.741531185599394439863864773280788;
        x->ptr.p_double[4] = 0.586087235467691130294144838258730;
        x->ptr.p_double[5] = 0.405845151377397166906606412076961;
        x->ptr.p_double[6] = 0.207784955007898467600689403773245;
        x->ptr.p_double[7] = 0.000000000000000000000000000000000;
        wkronrod->ptr.p_double[0] = 0.022935322010529224963732008058970;
        wkronrod->ptr.p_double[1] = 0.063092092629978553290700663189204;
        wkronrod->ptr.p_double[2] = 0.104790010322250183839876322541518;
        wkronrod->ptr.p_double[3] = 0.140653259715525918745189590510238;
        wkronrod->ptr.p_double[4] = 0.169004726639267902826583426598550;
        wkronrod->ptr.p_double[5] = 0.190350578064785409913256402421014;
        wkronrod->ptr.p_double[6] = 0.204432940075298892414161999234649;
        wkronrod->ptr.p_double[7] = 0.209482141084727828012999174891714;
    }
    if( n==21 )
    {
        ng = 5;
        wgauss->ptr.p_double[0] = 0.066671344308688137593568809893332;
        wgauss->ptr.p_double[1] = 0.149451349150580593145776339657697;
        wgauss->ptr.p_double[2] = 0.219086362515982043995534934228163;
        wgauss->ptr.p_double[3] = 0.269266719309996355091226921569469;
        wgauss->ptr.p_double[4] = 0.295524224714752870173892994651338;
        x->ptr.p_double[0] = 0.995657163025808080735527280689003;
        x->ptr.p_double[1] = 0.973906528517171720077964012084452;
        x->ptr.p_double[2] = 0.930157491355708226001207180059508;
        x->ptr.p_double[3] = 0.865063366688984510732096688423493;
        x->ptr.p_double[4] = 0.780817726586416897063717578345042;
        x->ptr.p_double[5] = 0.679409568299024406234327365114874;
        x->ptr.p_double[6] = 0.562757134668604683339000099272694;
        x->ptr.p_double[7] = 0.433395394129247190799265943165784;
        x->ptr.p_double[8] = 0.294392862701460198131126603103866;
        x->ptr.p_double[9] = 0.148874338981631210884826001129720;
        x->ptr.p_double[10] = 0.000000000000000000000000000000000;
        wkronrod->ptr.p_double[0] = 0.011694638867371874278064396062192;
        wkronrod->ptr.p_double[1] = 0.032558162307964727478818972459390;
        wkronrod->ptr.p_double[2] = 0.054755896574351996031381300244580;
        wkronrod->ptr.p_double[3] = 0.075039674810919952767043140916190;
        wkronrod->ptr.p_double[4] = 0.093125454583697605535065465083366;
        wkronrod->ptr.p_double[5] = 0.109387158802297641899210590325805;
        wkronrod->ptr.p_double[6] = 0.123491976262065851077958109831074;
        wkronrod->ptr.p_double[7] = 0.134709217311473325928054001771707;
        wkronrod->ptr.p_double[8] = 0.142775938577060080797094273138717;
        wkronrod->ptr.p_double[9] = 0.147739104901338491374841515972068;
        wkronrod->ptr.p_double[10] = 0.149445554002916905664936468389821;
    }
    if( n==31 )
    {
        ng = 8;
        wgauss->ptr.p_double[0] = 0.030753241996117268354628393577204;
        wgauss->ptr.p_double[1] = 0.070366047488108124709267416450667;
        wgauss->ptr.p_double[2] = 0.107159220467171935011869546685869;
        wgauss->ptr.p_double[3] = 0.139570677926154314447804794511028;
        wgauss->ptr.p_double[4] = 0.166269205816993933553200860481209;
        wgauss->ptr.p_double[5] = 0.186161000015562211026800561866423;
        wgauss->ptr.p_double[6] = 0.198431485327111576456118326443839;
        wgauss->ptr.p_double[7] = 0.202578241925561272880620199967519;
        x->ptr.p_double[0] = 0.998002298693397060285172840152271;
        x->ptr.p_double[1] = 0.987992518020485428489565718586613;
        x->ptr.p_double[2] = 0.967739075679139134257347978784337;
        x->ptr.p_double[3] = 0.937273392400705904307758947710209;
        x->ptr.p_double[4] = 0.897264532344081900882509656454496;
        x->ptr.p_double[5] = 0.848206583410427216200648320774217;
        x->ptr.p_double[6] = 0.790418501442465932967649294817947;
        x->ptr.p_double[7] = 0.724417731360170047416186054613938;
        x->ptr.p_double[8] = 0.650996741297416970533735895313275;
        x->ptr.p_double[9] = 0.570972172608538847537226737253911;
        x->ptr.p_double[10] = 0.485081863640239680693655740232351;
        x->ptr.p_double[11] = 0.394151347077563369897207370981045;
        x->ptr.p_double[12] = 0.299180007153168812166780024266389;
        x->ptr.p_double[13] = 0.201194093997434522300628303394596;
        x->ptr.p_double[14] = 0.101142066918717499027074231447392;
        x->ptr.p_double[15] = 0.000000000000000000000000000000000;
        wkronrod->ptr.p_double[0] = 0.005377479872923348987792051430128;
        wkronrod->ptr.p_double[1] = 0.015007947329316122538374763075807;
        wkronrod->ptr.p_double[2] = 0.025460847326715320186874001019653;
        wkronrod->ptr.p_double[3] = 0.035346360791375846222037948478360;
        wkronrod->ptr.p_double[4] = 0.044589751324764876608227299373280;
        wkronrod->ptr.p_double[5] = 0.053481524690928087265343147239430;
        wkronrod->ptr.p_double[6] = 0.062009567800670640285139230960803;
        wkronrod->ptr.p_double[7] = 0.069854121318728258709520077099147;
        wkronrod->ptr.p_double[8] = 0.076849680757720378894432777482659;
        wkronrod->ptr.p_double[9] = 0.083080502823133021038289247286104;
        wkronrod->ptr.p_double[10] = 0.088564443056211770647275443693774;
        wkronrod->ptr.p_double[11] = 0.093126598170825321225486872747346;
        wkronrod->ptr.p_double[12] = 0.096642726983623678505179907627589;
        wkronrod->ptr.p_double[13] = 0.099173598721791959332393173484603;
        wkronrod->ptr.p_double[14] = 0.100769845523875595044946662617570;
        wkronrod->ptr.p_double[15] = 0.101330007014791549017374792767493;
    }
    if( n==41 )
    {
        ng = 10;
        wgauss->ptr.p_double[0] = 0.017614007139152118311861962351853;
        wgauss->ptr.p_double[1] = 0.040601429800386941331039952274932;
        wgauss->ptr.p_double[2] = 0.062672048334109063569506535187042;
        wgauss->ptr.p_double[3] = 0.083276741576704748724758143222046;
        wgauss->ptr.p_double[4] = 0.101930119817240435036750135480350;
        wgauss->ptr.p_double[5] = 0.118194531961518417312377377711382;
        wgauss->ptr.p_double[6] = 0.131688638449176626898494499748163;
        wgauss->ptr.p_double[7] = 0.142096109318382051329298325067165;
        wgauss->ptr.p_double[8] = 0.149172986472603746787828737001969;
        wgauss->ptr.p_double[9] = 0.152753387130725850698084331955098;
        x->ptr.p_double[0] = 0.998859031588277663838315576545863;
        x->ptr.p_double[1] = 0.993128599185094924786122388471320;
        x->ptr.p_double[2] = 0.981507877450250259193342994720217;
        x->ptr.p_double[3] = 0.963971927277913791267666131197277;
        x->ptr.p_double[4] = 0.940822633831754753519982722212443;
        x->ptr.p_double[5] = 0.912234428251325905867752441203298;
        x->ptr.p_double[6] = 0.878276811252281976077442995113078;
        x->ptr.p_double[7] = 0.839116971822218823394529061701521;
        x->ptr.p_double[8] = 0.795041428837551198350638833272788;
        x->ptr.p_double[9] = 0.746331906460150792614305070355642;
        x->ptr.p_double[10] = 0.693237656334751384805490711845932;
        x->ptr.p_double[11] = 0.636053680726515025452836696226286;
        x->ptr.p_double[12] = 0.575140446819710315342946036586425;
        x->ptr.p_double[13] = 0.510867001950827098004364050955251;
        x->ptr.p_double[14] = 0.443593175238725103199992213492640;
        x->ptr.p_double[15] = 0.373706088715419560672548177024927;
        x->ptr.p_double[16] = 0.301627868114913004320555356858592;
        x->ptr.p_double[17] = 0.227785851141645078080496195368575;
        x->ptr.p_double[18] = 0.152605465240922675505220241022678;
        x->ptr.p_double[19] = 0.076526521133497333754640409398838;
        x->ptr.p_double[20] = 0.000000000000000000000000000000000;
        wkronrod->ptr.p_double[0] = 0.003073583718520531501218293246031;
        wkronrod->ptr.p_double[1] = 0.008600269855642942198661787950102;
        wkronrod->ptr.p_double[2] = 0.014626169256971252983787960308868;
        wkronrod->ptr.p_double[3] = 0.020388373461266523598010231432755;
        wkronrod->ptr.p_double[4] = 0.025882133604951158834505067096153;
        wkronrod->ptr.p_double[5] = 0.031287306777032798958543119323801;
        wkronrod->ptr.p_double[6] = 0.036600169758200798030557240707211;
        wkronrod->ptr.p_double[7] = 0.041668873327973686263788305936895;
        wkronrod->ptr.p_double[8] = 0.046434821867497674720231880926108;
        wkronrod->ptr.p_double[9] = 0.050944573923728691932707670050345;
        wkronrod->ptr.p_double[10] = 0.055195105348285994744832372419777;
        wkronrod->ptr.p_double[11] = 0.059111400880639572374967220648594;
        wkronrod->ptr.p_double[12] = 0.062653237554781168025870122174255;
        wkronrod->ptr.p_double[13] = 0.065834597133618422111563556969398;
        wkronrod->ptr.p_double[14] = 0.068648672928521619345623411885368;
        wkronrod->ptr.p_double[15] = 0.071054423553444068305790361723210;
        wkronrod->ptr.p_double[16] = 0.073030690332786667495189417658913;
        wkronrod->ptr.p_double[17] = 0.074582875400499188986581418362488;
        wkronrod->ptr.p_double[18] = 0.075704497684556674659542775376617;
        wkronrod->ptr.p_double[19] = 0.076377867672080736705502835038061;
        wkronrod->ptr.p_double[20] = 0.076600711917999656445049901530102;
    }
    if( n==51 )
    {
        ng = 13;
        wgauss->ptr.p_double[0] = 0.011393798501026287947902964113235;
        wgauss->ptr.p_double[1] = 0.026354986615032137261901815295299;
        wgauss->ptr.p_double[2] = 0.040939156701306312655623487711646;
        wgauss->ptr.p_double[3] = 0.054904695975835191925936891540473;
        wgauss->ptr.p_double[4] = 0.068038333812356917207187185656708;
        wgauss->ptr.p_double[5] = 0.080140700335001018013234959669111;
        wgauss->ptr.p_double[6] = 0.091028261982963649811497220702892;
        wgauss->ptr.p_double[7] = 0.100535949067050644202206890392686;
        wgauss->ptr.p_double[8] = 0.108519624474263653116093957050117;
        wgauss->ptr.p_double[9] = 0.114858259145711648339325545869556;
        wgauss->ptr.p_double[10] = 0.119455763535784772228178126512901;
        wgauss->ptr.p_double[11] = 0.122242442990310041688959518945852;
        wgauss->ptr.p_double[12] = 0.123176053726715451203902873079050;
        x->ptr.p_double[0] = 0.999262104992609834193457486540341;
        x->ptr.p_double[1] = 0.995556969790498097908784946893902;
        x->ptr.p_double[2] = 0.988035794534077247637331014577406;
        x->ptr.p_double[3] = 0.976663921459517511498315386479594;
        x->ptr.p_double[4] = 0.961614986425842512418130033660167;
        x->ptr.p_double[5] = 0.942974571228974339414011169658471;
        x->ptr.p_double[6] = 0.920747115281701561746346084546331;
        x->ptr.p_double[7] = 0.894991997878275368851042006782805;
        x->ptr.p_double[8] = 0.865847065293275595448996969588340;
        x->ptr.p_double[9] = 0.833442628760834001421021108693570;
        x->ptr.p_double[10] = 0.797873797998500059410410904994307;
        x->ptr.p_double[11] = 0.759259263037357630577282865204361;
        x->ptr.p_double[12] = 0.717766406813084388186654079773298;
        x->ptr.p_double[13] = 0.673566368473468364485120633247622;
        x->ptr.p_double[14] = 0.626810099010317412788122681624518;
        x->ptr.p_double[15] = 0.577662930241222967723689841612654;
        x->ptr.p_double[16] = 0.526325284334719182599623778158010;
        x->ptr.p_double[17] = 0.473002731445714960522182115009192;
        x->ptr.p_double[18] = 0.417885382193037748851814394594572;
        x->ptr.p_double[19] = 0.361172305809387837735821730127641;
        x->ptr.p_double[20] = 0.303089538931107830167478909980339;
        x->ptr.p_double[21] = 0.243866883720988432045190362797452;
        x->ptr.p_double[22] = 0.183718939421048892015969888759528;
        x->ptr.p_double[23] = 0.122864692610710396387359818808037;
        x->ptr.p_double[24] = 0.061544483005685078886546392366797;
        x->ptr.p_double[25] = 0.000000000000000000000000000000000;
        wkronrod->ptr.p_double[0] = 0.001987383892330315926507851882843;
        wkronrod->ptr.p_double[1] = 0.005561932135356713758040236901066;
        wkronrod->ptr.p_double[2] = 0.009473973386174151607207710523655;
        wkronrod->ptr.p_double[3] = 0.013236229195571674813656405846976;
        wkronrod->ptr.p_double[4] = 0.016847817709128298231516667536336;
        wkronrod->ptr.p_double[5] = 0.020435371145882835456568292235939;
        wkronrod->ptr.p_double[6] = 0.024009945606953216220092489164881;
        wkronrod->ptr.p_double[7] = 0.027475317587851737802948455517811;
        wkronrod->ptr.p_double[8] = 0.030792300167387488891109020215229;
        wkronrod->ptr.p_double[9] = 0.034002130274329337836748795229551;
        wkronrod->ptr.p_double[10] = 0.037116271483415543560330625367620;
        wkronrod->ptr.p_double[11] = 0.040083825504032382074839284467076;
        wkronrod->ptr.p_double[12] = 0.042872845020170049476895792439495;
        wkronrod->ptr.p_double[13] = 0.045502913049921788909870584752660;
        wkronrod->ptr.p_double[14] = 0.047982537138836713906392255756915;
        wkronrod->ptr.p_double[15] = 0.050277679080715671963325259433440;
        wkronrod->ptr.p_double[16] = 0.052362885806407475864366712137873;
        wkronrod->ptr.p_double[17] = 0.054251129888545490144543370459876;
        wkronrod->ptr.p_double[18] = 0.055950811220412317308240686382747;
        wkronrod->ptr.p_double[19] = 0.057437116361567832853582693939506;
        wkronrod->ptr.p_double[20] = 0.058689680022394207961974175856788;
        wkronrod->ptr.p_double[21] = 0.059720340324174059979099291932562;
        wkronrod->ptr.p_double[22] = 0.060539455376045862945360267517565;
        wkronrod->ptr.p_double[23] = 0.061128509717053048305859030416293;
        wkronrod->ptr.p_double[24] = 0.061471189871425316661544131965264;
        wkronrod->ptr.p_double[25] = 0.061580818067832935078759824240055;
    }
    if( n==61 )
    {
        ng = 15;
        wgauss->ptr.p_double[0] = 0.007968192496166605615465883474674;
        wgauss->ptr.p_double[1] = 0.018466468311090959142302131912047;
        wgauss->ptr.p_double[2] = 0.028784707883323369349719179611292;
        wgauss->ptr.p_double[3] = 0.038799192569627049596801936446348;
        wgauss->ptr.p_double[4] = 0.048402672830594052902938140422808;
        wgauss->ptr.p_double[5] = 0.057493156217619066481721689402056;
        wgauss->ptr.p_double[6] = 0.065974229882180495128128515115962;
        wgauss->ptr.p_double[7] = 0.073755974737705206268243850022191;
        wgauss->ptr.p_double[8] = 0.080755895229420215354694938460530;
        wgauss->ptr.p_double[9] = 0.086899787201082979802387530715126;
        wgauss->ptr.p_double[10] = 0.092122522237786128717632707087619;
        wgauss->ptr.p_double[11] = 0.096368737174644259639468626351810;
        wgauss->ptr.p_double[12] = 0.099593420586795267062780282103569;
        wgauss->ptr.p_double[13] = 0.101762389748405504596428952168554;
        wgauss->ptr.p_double[14] = 0.102852652893558840341285636705415;
        x->ptr.p_double[0] = 0.999484410050490637571325895705811;
        x->ptr.p_double[1] = 0.996893484074649540271630050918695;
        x->ptr.p_double[2] = 0.991630996870404594858628366109486;
        x->ptr.p_double[3] = 0.983668123279747209970032581605663;
        x->ptr.p_double[4] = 0.973116322501126268374693868423707;
        x->ptr.p_double[5] = 0.960021864968307512216871025581798;
        x->ptr.p_double[6] = 0.944374444748559979415831324037439;
        x->ptr.p_double[7] = 0.926200047429274325879324277080474;
        x->ptr.p_double[8] = 0.905573307699907798546522558925958;
        x->ptr.p_double[9] = 0.882560535792052681543116462530226;
        x->ptr.p_double[10] = 0.857205233546061098958658510658944;
        x->ptr.p_double[11] = 0.829565762382768397442898119732502;
        x->ptr.p_double[12] = 0.799727835821839083013668942322683;
        x->ptr.p_double[13] = 0.767777432104826194917977340974503;
        x->ptr.p_double[14] = 0.733790062453226804726171131369528;
        x->ptr.p_double[15] = 0.697850494793315796932292388026640;
        x->ptr.p_double[16] = 0.660061064126626961370053668149271;
        x->ptr.p_double[17] = 0.620526182989242861140477556431189;
        x->ptr.p_double[18] = 0.579345235826361691756024932172540;
        x->ptr.p_double[19] = 0.536624148142019899264169793311073;
        x->ptr.p_double[20] = 0.492480467861778574993693061207709;
        x->ptr.p_double[21] = 0.447033769538089176780609900322854;
        x->ptr.p_double[22] = 0.400401254830394392535476211542661;
        x->ptr.p_double[23] = 0.352704725530878113471037207089374;
        x->ptr.p_double[24] = 0.304073202273625077372677107199257;
        x->ptr.p_double[25] = 0.254636926167889846439805129817805;
        x->ptr.p_double[26] = 0.204525116682309891438957671002025;
        x->ptr.p_double[27] = 0.153869913608583546963794672743256;
        x->ptr.p_double[28] = 0.102806937966737030147096751318001;
        x->ptr.p_double[29] = 0.051471842555317695833025213166723;
        x->ptr.p_double[30] = 0.000000000000000000000000000000000;
        wkronrod->ptr.p_double[0] = 0.001389013698677007624551591226760;
        wkronrod->ptr.p_double[1] = 0.003890461127099884051267201844516;
        wkronrod->ptr.p_double[2] = 0.006630703915931292173319826369750;
        wkronrod->ptr.p_double[3] = 0.009273279659517763428441146892024;
        wkronrod->ptr.p_double[4] = 0.011823015253496341742232898853251;
        wkronrod->ptr.p_double[5] = 0.014369729507045804812451432443580;
        wkronrod->ptr.p_double[6] = 0.016920889189053272627572289420322;
        wkronrod->ptr.p_double[7] = 0.019414141193942381173408951050128;
        wkronrod->ptr.p_double[8] = 0.021828035821609192297167485738339;
        wkronrod->ptr.p_double[9] = 0.024191162078080601365686370725232;
        wkronrod->ptr.p_double[10] = 0.026509954882333101610601709335075;
        wkronrod->ptr.p_double[11] = 0.028754048765041292843978785354334;
        wkronrod->ptr.p_double[12] = 0.030907257562387762472884252943092;
        wkronrod->ptr.p_double[13] = 0.032981447057483726031814191016854;
        wkronrod->ptr.p_double[14] = 0.034979338028060024137499670731468;
        wkronrod->ptr.p_double[15] = 0.036882364651821229223911065617136;
        wkronrod->ptr.p_double[16] = 0.038678945624727592950348651532281;
        wkronrod->ptr.p_double[17] = 0.040374538951535959111995279752468;
        wkronrod->ptr.p_double[18] = 0.041969810215164246147147541285970;
        wkronrod->ptr.p_double[19] = 0.043452539701356069316831728117073;
        wkronrod->ptr.p_double[20] = 0.044814800133162663192355551616723;
        wkronrod->ptr.p_double[21] = 0.046059238271006988116271735559374;
        wkronrod->ptr.p_double[22] = 0.047185546569299153945261478181099;
        wkronrod->ptr.p_double[23] = 0.048185861757087129140779492298305;
        wkronrod->ptr.p_double[24] = 0.049055434555029778887528165367238;
        wkronrod->ptr.p_double[25] = 0.049795683427074206357811569379942;
        wkronrod->ptr.p_double[26] = 0.050405921402782346840893085653585;
        wkronrod->ptr.p_double[27] = 0.050881795898749606492297473049805;
        wkronrod->ptr.p_double[28] = 0.051221547849258772170656282604944;
        wkronrod->ptr.p_double[29] = 0.051426128537459025933862879215781;
        wkronrod->ptr.p_double[30] = 0.051494729429451567558340433647099;
    }
    
    /*
     * copy nodes
     */
    for(i=n-1; i>=n/2; i--)
    {
        x->ptr.p_double[i] = -x->ptr.p_double[n-1-i];
    }
    
    /*
     * copy Kronrod weights
     */
    for(i=n-1; i>=n/2; i--)
    {
        wkronrod->ptr.p_double[i] = wkronrod->ptr.p_double[n-1-i];
    }
    
    /*
     * copy Gauss weights
     */
    for(i=ng-1; i>=0; i--)
    {
        wgauss->ptr.p_double[n-2-2*i] = wgauss->ptr.p_double[i];
        wgauss->ptr.p_double[1+2*i] = wgauss->ptr.p_double[i];
    }
    for(i=0; i<=n/2; i++)
    {
        wgauss->ptr.p_double[2*i] = (double)(0);
    }
    
    /*
     * reorder
     */
    tagsort(x, n, &p1, &p2, _state);
    for(i=0; i<=n-1; i++)
    {
        tmp = wkronrod->ptr.p_double[i];
        wkronrod->ptr.p_double[i] = wkronrod->ptr.p_double[p2.ptr.p_int[i]];
        wkronrod->ptr.p_double[p2.ptr.p_int[i]] = tmp;
        tmp = wgauss->ptr.p_double[i];
        wgauss->ptr.p_double[i] = wgauss->ptr.p_double[p2.ptr.p_int[i]];
        wgauss->ptr.p_double[p2.ptr.p_int[i]] = tmp;
    }
    ae_frame_leave(_state);
}


/*$ End $*/
