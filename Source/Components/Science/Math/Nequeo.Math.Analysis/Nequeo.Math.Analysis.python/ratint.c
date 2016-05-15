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
#include "ratint.h"


/*$ Declarations $*/
static void ratint_barycentricnormalize(barycentricinterpolant* b,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Rational interpolation using barycentric formula

F(t) = SUM(i=0,n-1,w[i]*f[i]/(t-x[i])) / SUM(i=0,n-1,w[i]/(t-x[i]))

Input parameters:
    B   -   barycentric interpolant built with one of model building
            subroutines.
    T   -   interpolation point

Result:
    barycentric interpolant F(t)

  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
double barycentriccalc(barycentricinterpolant* b,
     double t,
     ae_state *_state)
{
    double s1;
    double s2;
    double s;
    double v;
    ae_int_t i;
    double result;


    ae_assert(!ae_isinf(t, _state), "BarycentricCalc: infinite T!", _state);
    
    /*
     * special case: NaN
     */
    if( ae_isnan(t, _state) )
    {
        result = _state->v_nan;
        return result;
    }
    
    /*
     * special case: N=1
     */
    if( b->n==1 )
    {
        result = b->sy*b->y.ptr.p_double[0];
        return result;
    }
    
    /*
     * Here we assume that task is normalized, i.e.:
     * 1. abs(Y[i])<=1
     * 2. abs(W[i])<=1
     * 3. X[] is ordered
     */
    s = ae_fabs(t-b->x.ptr.p_double[0], _state);
    for(i=0; i<=b->n-1; i++)
    {
        v = b->x.ptr.p_double[i];
        if( ae_fp_eq(v,t) )
        {
            result = b->sy*b->y.ptr.p_double[i];
            return result;
        }
        v = ae_fabs(t-v, _state);
        if( ae_fp_less(v,s) )
        {
            s = v;
        }
    }
    s1 = (double)(0);
    s2 = (double)(0);
    for(i=0; i<=b->n-1; i++)
    {
        v = s/(t-b->x.ptr.p_double[i]);
        v = v*b->w.ptr.p_double[i];
        s1 = s1+v*b->y.ptr.p_double[i];
        s2 = s2+v;
    }
    result = b->sy*s1/s2;
    return result;
}


/*************************************************************************
Differentiation of barycentric interpolant: first derivative.

Algorithm used in this subroutine is very robust and should not fail until
provided with values too close to MaxRealNumber  (usually  MaxRealNumber/N
or greater will overflow).

INPUT PARAMETERS:
    B   -   barycentric interpolant built with one of model building
            subroutines.
    T   -   interpolation point

OUTPUT PARAMETERS:
    F   -   barycentric interpolant at T
    DF  -   first derivative
    
NOTE


  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void barycentricdiff1(barycentricinterpolant* b,
     double t,
     double* f,
     double* df,
     ae_state *_state)
{
    double v;
    double vv;
    ae_int_t i;
    ae_int_t k;
    double n0;
    double n1;
    double d0;
    double d1;
    double s0;
    double s1;
    double xk;
    double xi;
    double xmin;
    double xmax;
    double xscale1;
    double xoffs1;
    double xscale2;
    double xoffs2;
    double xprev;

    *f = 0;
    *df = 0;

    ae_assert(!ae_isinf(t, _state), "BarycentricDiff1: infinite T!", _state);
    
    /*
     * special case: NaN
     */
    if( ae_isnan(t, _state) )
    {
        *f = _state->v_nan;
        *df = _state->v_nan;
        return;
    }
    
    /*
     * special case: N=1
     */
    if( b->n==1 )
    {
        *f = b->sy*b->y.ptr.p_double[0];
        *df = (double)(0);
        return;
    }
    if( ae_fp_eq(b->sy,(double)(0)) )
    {
        *f = (double)(0);
        *df = (double)(0);
        return;
    }
    ae_assert(ae_fp_greater(b->sy,(double)(0)), "BarycentricDiff1: internal error", _state);
    
    /*
     * We assume than N>1 and B.SY>0. Find:
     * 1. pivot point (X[i] closest to T)
     * 2. width of interval containing X[i]
     */
    v = ae_fabs(b->x.ptr.p_double[0]-t, _state);
    k = 0;
    xmin = b->x.ptr.p_double[0];
    xmax = b->x.ptr.p_double[0];
    for(i=1; i<=b->n-1; i++)
    {
        vv = b->x.ptr.p_double[i];
        if( ae_fp_less(ae_fabs(vv-t, _state),v) )
        {
            v = ae_fabs(vv-t, _state);
            k = i;
        }
        xmin = ae_minreal(xmin, vv, _state);
        xmax = ae_maxreal(xmax, vv, _state);
    }
    
    /*
     * pivot point found, calculate dNumerator and dDenominator
     */
    xscale1 = 1/(xmax-xmin);
    xoffs1 = -xmin/(xmax-xmin)+1;
    xscale2 = (double)(2);
    xoffs2 = (double)(-3);
    t = t*xscale1+xoffs1;
    t = t*xscale2+xoffs2;
    xk = b->x.ptr.p_double[k];
    xk = xk*xscale1+xoffs1;
    xk = xk*xscale2+xoffs2;
    v = t-xk;
    n0 = (double)(0);
    n1 = (double)(0);
    d0 = (double)(0);
    d1 = (double)(0);
    xprev = (double)(-2);
    for(i=0; i<=b->n-1; i++)
    {
        xi = b->x.ptr.p_double[i];
        xi = xi*xscale1+xoffs1;
        xi = xi*xscale2+xoffs2;
        ae_assert(ae_fp_greater(xi,xprev), "BarycentricDiff1: points are too close!", _state);
        xprev = xi;
        if( i!=k )
        {
            vv = ae_sqr(t-xi, _state);
            s0 = (t-xk)/(t-xi);
            s1 = (xk-xi)/vv;
        }
        else
        {
            s0 = (double)(1);
            s1 = (double)(0);
        }
        vv = b->w.ptr.p_double[i]*b->y.ptr.p_double[i];
        n0 = n0+s0*vv;
        n1 = n1+s1*vv;
        vv = b->w.ptr.p_double[i];
        d0 = d0+s0*vv;
        d1 = d1+s1*vv;
    }
    *f = b->sy*n0/d0;
    *df = (n1*d0-n0*d1)/ae_sqr(d0, _state);
    if( ae_fp_neq(*df,(double)(0)) )
    {
        *df = ae_sign(*df, _state)*ae_exp(ae_log(ae_fabs(*df, _state), _state)+ae_log(b->sy, _state)+ae_log(xscale1, _state)+ae_log(xscale2, _state), _state);
    }
}


/*************************************************************************
Differentiation of barycentric interpolant: first/second derivatives.

INPUT PARAMETERS:
    B   -   barycentric interpolant built with one of model building
            subroutines.
    T   -   interpolation point

OUTPUT PARAMETERS:
    F   -   barycentric interpolant at T
    DF  -   first derivative
    D2F -   second derivative

NOTE: this algorithm may fail due to overflow/underflor if  used  on  data
whose values are close to MaxRealNumber or MinRealNumber.  Use more robust
BarycentricDiff1() subroutine in such cases.


  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void barycentricdiff2(barycentricinterpolant* b,
     double t,
     double* f,
     double* df,
     double* d2f,
     ae_state *_state)
{
    double v;
    double vv;
    ae_int_t i;
    ae_int_t k;
    double n0;
    double n1;
    double n2;
    double d0;
    double d1;
    double d2;
    double s0;
    double s1;
    double s2;
    double xk;
    double xi;

    *f = 0;
    *df = 0;
    *d2f = 0;

    ae_assert(!ae_isinf(t, _state), "BarycentricDiff1: infinite T!", _state);
    
    /*
     * special case: NaN
     */
    if( ae_isnan(t, _state) )
    {
        *f = _state->v_nan;
        *df = _state->v_nan;
        *d2f = _state->v_nan;
        return;
    }
    
    /*
     * special case: N=1
     */
    if( b->n==1 )
    {
        *f = b->sy*b->y.ptr.p_double[0];
        *df = (double)(0);
        *d2f = (double)(0);
        return;
    }
    if( ae_fp_eq(b->sy,(double)(0)) )
    {
        *f = (double)(0);
        *df = (double)(0);
        *d2f = (double)(0);
        return;
    }
    
    /*
     * We assume than N>1 and B.SY>0. Find:
     * 1. pivot point (X[i] closest to T)
     * 2. width of interval containing X[i]
     */
    ae_assert(ae_fp_greater(b->sy,(double)(0)), "BarycentricDiff: internal error", _state);
    *f = (double)(0);
    *df = (double)(0);
    *d2f = (double)(0);
    v = ae_fabs(b->x.ptr.p_double[0]-t, _state);
    k = 0;
    for(i=1; i<=b->n-1; i++)
    {
        vv = b->x.ptr.p_double[i];
        if( ae_fp_less(ae_fabs(vv-t, _state),v) )
        {
            v = ae_fabs(vv-t, _state);
            k = i;
        }
    }
    
    /*
     * pivot point found, calculate dNumerator and dDenominator
     */
    xk = b->x.ptr.p_double[k];
    v = t-xk;
    n0 = (double)(0);
    n1 = (double)(0);
    n2 = (double)(0);
    d0 = (double)(0);
    d1 = (double)(0);
    d2 = (double)(0);
    for(i=0; i<=b->n-1; i++)
    {
        if( i!=k )
        {
            xi = b->x.ptr.p_double[i];
            vv = ae_sqr(t-xi, _state);
            s0 = (t-xk)/(t-xi);
            s1 = (xk-xi)/vv;
            s2 = -2*(xk-xi)/(vv*(t-xi));
        }
        else
        {
            s0 = (double)(1);
            s1 = (double)(0);
            s2 = (double)(0);
        }
        vv = b->w.ptr.p_double[i]*b->y.ptr.p_double[i];
        n0 = n0+s0*vv;
        n1 = n1+s1*vv;
        n2 = n2+s2*vv;
        vv = b->w.ptr.p_double[i];
        d0 = d0+s0*vv;
        d1 = d1+s1*vv;
        d2 = d2+s2*vv;
    }
    *f = b->sy*n0/d0;
    *df = b->sy*(n1*d0-n0*d1)/ae_sqr(d0, _state);
    *d2f = b->sy*((n2*d0-n0*d2)*ae_sqr(d0, _state)-(n1*d0-n0*d1)*2*d0*d1)/ae_sqr(ae_sqr(d0, _state), _state);
}


/*************************************************************************
This subroutine performs linear transformation of the argument.

INPUT PARAMETERS:
    B       -   rational interpolant in barycentric form
    CA, CB  -   transformation coefficients: x = CA*t + CB

OUTPUT PARAMETERS:
    B       -   transformed interpolant with X replaced by T

  -- ALGLIB PROJECT --
     Copyright 19.08.2009 by Bochkanov Sergey
*************************************************************************/
void barycentriclintransx(barycentricinterpolant* b,
     double ca,
     double cb,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;


    
    /*
     * special case, replace by constant F(CB)
     */
    if( ae_fp_eq(ca,(double)(0)) )
    {
        b->sy = barycentriccalc(b, cb, _state);
        v = (double)(1);
        for(i=0; i<=b->n-1; i++)
        {
            b->y.ptr.p_double[i] = (double)(1);
            b->w.ptr.p_double[i] = v;
            v = -v;
        }
        return;
    }
    
    /*
     * general case: CA<>0
     */
    for(i=0; i<=b->n-1; i++)
    {
        b->x.ptr.p_double[i] = (b->x.ptr.p_double[i]-cb)/ca;
    }
    if( ae_fp_less(ca,(double)(0)) )
    {
        for(i=0; i<=b->n-1; i++)
        {
            if( i<b->n-1-i )
            {
                j = b->n-1-i;
                v = b->x.ptr.p_double[i];
                b->x.ptr.p_double[i] = b->x.ptr.p_double[j];
                b->x.ptr.p_double[j] = v;
                v = b->y.ptr.p_double[i];
                b->y.ptr.p_double[i] = b->y.ptr.p_double[j];
                b->y.ptr.p_double[j] = v;
                v = b->w.ptr.p_double[i];
                b->w.ptr.p_double[i] = b->w.ptr.p_double[j];
                b->w.ptr.p_double[j] = v;
            }
            else
            {
                break;
            }
        }
    }
}


/*************************************************************************
This  subroutine   performs   linear  transformation  of  the  barycentric
interpolant.

INPUT PARAMETERS:
    B       -   rational interpolant in barycentric form
    CA, CB  -   transformation coefficients: B2(x) = CA*B(x) + CB

OUTPUT PARAMETERS:
    B       -   transformed interpolant

  -- ALGLIB PROJECT --
     Copyright 19.08.2009 by Bochkanov Sergey
*************************************************************************/
void barycentriclintransy(barycentricinterpolant* b,
     double ca,
     double cb,
     ae_state *_state)
{
    ae_int_t i;
    double v;


    for(i=0; i<=b->n-1; i++)
    {
        b->y.ptr.p_double[i] = ca*b->sy*b->y.ptr.p_double[i]+cb;
    }
    b->sy = (double)(0);
    for(i=0; i<=b->n-1; i++)
    {
        b->sy = ae_maxreal(b->sy, ae_fabs(b->y.ptr.p_double[i], _state), _state);
    }
    if( ae_fp_greater(b->sy,(double)(0)) )
    {
        v = 1/b->sy;
        ae_v_muld(&b->y.ptr.p_double[0], 1, ae_v_len(0,b->n-1), v);
    }
}


/*************************************************************************
Extracts X/Y/W arrays from rational interpolant

INPUT PARAMETERS:
    B   -   barycentric interpolant

OUTPUT PARAMETERS:
    N   -   nodes count, N>0
    X   -   interpolation nodes, array[0..N-1]
    F   -   function values, array[0..N-1]
    W   -   barycentric weights, array[0..N-1]

  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void barycentricunpack(barycentricinterpolant* b,
     ae_int_t* n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_state *_state)
{
    double v;

    *n = 0;
    ae_vector_clear(x);
    ae_vector_clear(y);
    ae_vector_clear(w);

    *n = b->n;
    ae_vector_set_length(x, *n, _state);
    ae_vector_set_length(y, *n, _state);
    ae_vector_set_length(w, *n, _state);
    v = b->sy;
    ae_v_move(&x->ptr.p_double[0], 1, &b->x.ptr.p_double[0], 1, ae_v_len(0,*n-1));
    ae_v_moved(&y->ptr.p_double[0], 1, &b->y.ptr.p_double[0], 1, ae_v_len(0,*n-1), v);
    ae_v_move(&w->ptr.p_double[0], 1, &b->w.ptr.p_double[0], 1, ae_v_len(0,*n-1));
}


/*************************************************************************
Rational interpolant from X/Y/W arrays

F(t) = SUM(i=0,n-1,w[i]*f[i]/(t-x[i])) / SUM(i=0,n-1,w[i]/(t-x[i]))

INPUT PARAMETERS:
    X   -   interpolation nodes, array[0..N-1]
    F   -   function values, array[0..N-1]
    W   -   barycentric weights, array[0..N-1]
    N   -   nodes count, N>0

OUTPUT PARAMETERS:
    B   -   barycentric interpolant built from (X, Y, W)

  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void barycentricbuildxyw(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     barycentricinterpolant* b,
     ae_state *_state)
{

    _barycentricinterpolant_clear(b);

    ae_assert(n>0, "BarycentricBuildXYW: incorrect N!", _state);
    
    /*
     * fill X/Y/W
     */
    ae_vector_set_length(&b->x, n, _state);
    ae_vector_set_length(&b->y, n, _state);
    ae_vector_set_length(&b->w, n, _state);
    ae_v_move(&b->x.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&b->y.ptr.p_double[0], 1, &y->ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&b->w.ptr.p_double[0], 1, &w->ptr.p_double[0], 1, ae_v_len(0,n-1));
    b->n = n;
    
    /*
     * Normalize
     */
    ratint_barycentricnormalize(b, _state);
}


/*************************************************************************
Rational interpolant without poles

The subroutine constructs the rational interpolating function without real
poles  (see  'Barycentric rational interpolation with no  poles  and  high
rates of approximation', Michael S. Floater. and  Kai  Hormann,  for  more
information on this subject).

Input parameters:
    X   -   interpolation nodes, array[0..N-1].
    Y   -   function values, array[0..N-1].
    N   -   number of nodes, N>0.
    D   -   order of the interpolation scheme, 0 <= D <= N-1.
            D<0 will cause an error.
            D>=N it will be replaced with D=N-1.
            if you don't know what D to choose, use small value about 3-5.

Output parameters:
    B   -   barycentric interpolant.

Note:
    this algorithm always succeeds and calculates the weights  with  close
    to machine precision.

  -- ALGLIB PROJECT --
     Copyright 17.06.2007 by Bochkanov Sergey
*************************************************************************/
void barycentricbuildfloaterhormann(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t d,
     barycentricinterpolant* b,
     ae_state *_state)
{
    ae_frame _frame_block;
    double s0;
    double s;
    double v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_vector perm;
    ae_vector wtemp;
    ae_vector sortrbuf;
    ae_vector sortrbuf2;

    ae_frame_make(_state, &_frame_block);
    _barycentricinterpolant_clear(b);
    ae_vector_init(&perm, 0, DT_INT, _state);
    ae_vector_init(&wtemp, 0, DT_REAL, _state);
    ae_vector_init(&sortrbuf, 0, DT_REAL, _state);
    ae_vector_init(&sortrbuf2, 0, DT_REAL, _state);

    ae_assert(n>0, "BarycentricFloaterHormann: N<=0!", _state);
    ae_assert(d>=0, "BarycentricFloaterHormann: incorrect D!", _state);
    
    /*
     * Prepare
     */
    if( d>n-1 )
    {
        d = n-1;
    }
    b->n = n;
    
    /*
     * special case: N=1
     */
    if( n==1 )
    {
        ae_vector_set_length(&b->x, n, _state);
        ae_vector_set_length(&b->y, n, _state);
        ae_vector_set_length(&b->w, n, _state);
        b->x.ptr.p_double[0] = x->ptr.p_double[0];
        b->y.ptr.p_double[0] = y->ptr.p_double[0];
        b->w.ptr.p_double[0] = (double)(1);
        ratint_barycentricnormalize(b, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Fill X/Y
     */
    ae_vector_set_length(&b->x, n, _state);
    ae_vector_set_length(&b->y, n, _state);
    ae_v_move(&b->x.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&b->y.ptr.p_double[0], 1, &y->ptr.p_double[0], 1, ae_v_len(0,n-1));
    tagsortfastr(&b->x, &b->y, &sortrbuf, &sortrbuf2, n, _state);
    
    /*
     * Calculate Wk
     */
    ae_vector_set_length(&b->w, n, _state);
    s0 = (double)(1);
    for(k=1; k<=d; k++)
    {
        s0 = -s0;
    }
    for(k=0; k<=n-1; k++)
    {
        
        /*
         * Wk
         */
        s = (double)(0);
        for(i=ae_maxint(k-d, 0, _state); i<=ae_minint(k, n-1-d, _state); i++)
        {
            v = (double)(1);
            for(j=i; j<=i+d; j++)
            {
                if( j!=k )
                {
                    v = v/ae_fabs(b->x.ptr.p_double[k]-b->x.ptr.p_double[j], _state);
                }
            }
            s = s+v;
        }
        b->w.ptr.p_double[k] = s0*s;
        
        /*
         * Next S0
         */
        s0 = -s0;
    }
    
    /*
     * Normalize
     */
    ratint_barycentricnormalize(b, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Copying of the barycentric interpolant (for internal use only)

INPUT PARAMETERS:
    B   -   barycentric interpolant

OUTPUT PARAMETERS:
    B2  -   copy(B1)

  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void barycentriccopy(barycentricinterpolant* b,
     barycentricinterpolant* b2,
     ae_state *_state)
{

    _barycentricinterpolant_clear(b2);

    b2->n = b->n;
    b2->sy = b->sy;
    ae_vector_set_length(&b2->x, b2->n, _state);
    ae_vector_set_length(&b2->y, b2->n, _state);
    ae_vector_set_length(&b2->w, b2->n, _state);
    ae_v_move(&b2->x.ptr.p_double[0], 1, &b->x.ptr.p_double[0], 1, ae_v_len(0,b2->n-1));
    ae_v_move(&b2->y.ptr.p_double[0], 1, &b->y.ptr.p_double[0], 1, ae_v_len(0,b2->n-1));
    ae_v_move(&b2->w.ptr.p_double[0], 1, &b->w.ptr.p_double[0], 1, ae_v_len(0,b2->n-1));
}


/*************************************************************************
Normalization of barycentric interpolant:
* B.N, B.X, B.Y and B.W are initialized
* B.SY is NOT initialized
* Y[] is normalized, scaling coefficient is stored in B.SY
* W[] is normalized, no scaling coefficient is stored
* X[] is sorted

Internal subroutine.
*************************************************************************/
static void ratint_barycentricnormalize(barycentricinterpolant* b,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector p1;
    ae_vector p2;
    ae_int_t i;
    ae_int_t j;
    ae_int_t j2;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&p1, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);

    
    /*
     * Normalize task: |Y|<=1, |W|<=1, sort X[]
     */
    b->sy = (double)(0);
    for(i=0; i<=b->n-1; i++)
    {
        b->sy = ae_maxreal(b->sy, ae_fabs(b->y.ptr.p_double[i], _state), _state);
    }
    if( ae_fp_greater(b->sy,(double)(0))&&ae_fp_greater(ae_fabs(b->sy-1, _state),10*ae_machineepsilon) )
    {
        v = 1/b->sy;
        ae_v_muld(&b->y.ptr.p_double[0], 1, ae_v_len(0,b->n-1), v);
    }
    v = (double)(0);
    for(i=0; i<=b->n-1; i++)
    {
        v = ae_maxreal(v, ae_fabs(b->w.ptr.p_double[i], _state), _state);
    }
    if( ae_fp_greater(v,(double)(0))&&ae_fp_greater(ae_fabs(v-1, _state),10*ae_machineepsilon) )
    {
        v = 1/v;
        ae_v_muld(&b->w.ptr.p_double[0], 1, ae_v_len(0,b->n-1), v);
    }
    for(i=0; i<=b->n-2; i++)
    {
        if( ae_fp_less(b->x.ptr.p_double[i+1],b->x.ptr.p_double[i]) )
        {
            tagsort(&b->x, b->n, &p1, &p2, _state);
            for(j=0; j<=b->n-1; j++)
            {
                j2 = p2.ptr.p_int[j];
                v = b->y.ptr.p_double[j];
                b->y.ptr.p_double[j] = b->y.ptr.p_double[j2];
                b->y.ptr.p_double[j2] = v;
                v = b->w.ptr.p_double[j];
                b->w.ptr.p_double[j] = b->w.ptr.p_double[j2];
                b->w.ptr.p_double[j2] = v;
            }
            break;
        }
    }
    ae_frame_leave(_state);
}


void _barycentricinterpolant_init(void* _p, ae_state *_state)
{
    barycentricinterpolant *p = (barycentricinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->y, 0, DT_REAL, _state);
    ae_vector_init(&p->w, 0, DT_REAL, _state);
}


void _barycentricinterpolant_init_copy(void* _dst, void* _src, ae_state *_state)
{
    barycentricinterpolant *dst = (barycentricinterpolant*)_dst;
    barycentricinterpolant *src = (barycentricinterpolant*)_src;
    dst->n = src->n;
    dst->sy = src->sy;
    ae_vector_init_copy(&dst->x, &src->x, _state);
    ae_vector_init_copy(&dst->y, &src->y, _state);
    ae_vector_init_copy(&dst->w, &src->w, _state);
}


void _barycentricinterpolant_clear(void* _p)
{
    barycentricinterpolant *p = (barycentricinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->y);
    ae_vector_clear(&p->w);
}


void _barycentricinterpolant_destroy(void* _p)
{
    barycentricinterpolant *p = (barycentricinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->y);
    ae_vector_destroy(&p->w);
}


/*$ End $*/
