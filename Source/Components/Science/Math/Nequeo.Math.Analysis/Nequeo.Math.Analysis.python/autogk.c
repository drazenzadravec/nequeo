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
#include "autogk.h"


/*$ Declarations $*/
static ae_int_t autogk_maxsubintervals = 10000;
static void autogk_autogkinternalprepare(double a,
     double b,
     double eps,
     double xwidth,
     autogkinternalstate* state,
     ae_state *_state);
static ae_bool autogk_autogkinternaliteration(autogkinternalstate* state,
     ae_state *_state);
static void autogk_mheappop(/* Real    */ ae_matrix* heap,
     ae_int_t heapsize,
     ae_int_t heapwidth,
     ae_state *_state);
static void autogk_mheappush(/* Real    */ ae_matrix* heap,
     ae_int_t heapsize,
     ae_int_t heapwidth,
     ae_state *_state);
static void autogk_mheapresize(/* Real    */ ae_matrix* heap,
     ae_int_t* heapsize,
     ae_int_t newheapsize,
     ae_int_t heapwidth,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Integration of a smooth function F(x) on a finite interval [a,b].

Fast-convergent algorithm based on a Gauss-Kronrod formula is used. Result
is calculated with accuracy close to the machine precision.

Algorithm works well only with smooth integrands.  It  may  be  used  with
continuous non-smooth integrands, but with  less  performance.

It should never be used with integrands which have integrable singularities
at lower or upper limits - algorithm may crash. Use AutoGKSingular in such
cases.

INPUT PARAMETERS:
    A, B    -   interval boundaries (A<B, A=B or A>B)
    
OUTPUT PARAMETERS
    State   -   structure which stores algorithm state

SEE ALSO
    AutoGKSmoothW, AutoGKSingular, AutoGKResults.
    

  -- ALGLIB --
     Copyright 06.05.2009 by Bochkanov Sergey
*************************************************************************/
void autogksmooth(double a,
     double b,
     autogkstate* state,
     ae_state *_state)
{

    _autogkstate_clear(state);

    ae_assert(ae_isfinite(a, _state), "AutoGKSmooth: A is not finite!", _state);
    ae_assert(ae_isfinite(b, _state), "AutoGKSmooth: B is not finite!", _state);
    autogksmoothw(a, b, 0.0, state, _state);
}


/*************************************************************************
Integration of a smooth function F(x) on a finite interval [a,b].

This subroutine is same as AutoGKSmooth(), but it guarantees that interval
[a,b] is partitioned into subintervals which have width at most XWidth.

Subroutine  can  be  used  when  integrating nearly-constant function with
narrow "bumps" (about XWidth wide). If "bumps" are too narrow, AutoGKSmooth
subroutine can overlook them.

INPUT PARAMETERS:
    A, B    -   interval boundaries (A<B, A=B or A>B)

OUTPUT PARAMETERS
    State   -   structure which stores algorithm state

SEE ALSO
    AutoGKSmooth, AutoGKSingular, AutoGKResults.


  -- ALGLIB --
     Copyright 06.05.2009 by Bochkanov Sergey
*************************************************************************/
void autogksmoothw(double a,
     double b,
     double xwidth,
     autogkstate* state,
     ae_state *_state)
{

    _autogkstate_clear(state);

    ae_assert(ae_isfinite(a, _state), "AutoGKSmoothW: A is not finite!", _state);
    ae_assert(ae_isfinite(b, _state), "AutoGKSmoothW: B is not finite!", _state);
    ae_assert(ae_isfinite(xwidth, _state), "AutoGKSmoothW: XWidth is not finite!", _state);
    state->wrappermode = 0;
    state->a = a;
    state->b = b;
    state->xwidth = xwidth;
    state->needf = ae_false;
    ae_vector_set_length(&state->rstate.ra, 10+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
Integration on a finite interval [A,B].
Integrand have integrable singularities at A/B.

F(X) must diverge as "(x-A)^alpha" at A, as "(B-x)^beta" at B,  with known
alpha/beta (alpha>-1, beta>-1).  If alpha/beta  are  not known,  estimates
from below can be used (but these estimates should be greater than -1 too).

One  of  alpha/beta variables (or even both alpha/beta) may be equal to 0,
which means than function F(x) is non-singular at A/B. Anyway (singular at
bounds or not), function F(x) is supposed to be continuous on (A,B).

Fast-convergent algorithm based on a Gauss-Kronrod formula is used. Result
is calculated with accuracy close to the machine precision.

INPUT PARAMETERS:
    A, B    -   interval boundaries (A<B, A=B or A>B)
    Alpha   -   power-law coefficient of the F(x) at A,
                Alpha>-1
    Beta    -   power-law coefficient of the F(x) at B,
                Beta>-1

OUTPUT PARAMETERS
    State   -   structure which stores algorithm state

SEE ALSO
    AutoGKSmooth, AutoGKSmoothW, AutoGKResults.


  -- ALGLIB --
     Copyright 06.05.2009 by Bochkanov Sergey
*************************************************************************/
void autogksingular(double a,
     double b,
     double alpha,
     double beta,
     autogkstate* state,
     ae_state *_state)
{

    _autogkstate_clear(state);

    ae_assert(ae_isfinite(a, _state), "AutoGKSingular: A is not finite!", _state);
    ae_assert(ae_isfinite(b, _state), "AutoGKSingular: B is not finite!", _state);
    ae_assert(ae_isfinite(alpha, _state), "AutoGKSingular: Alpha is not finite!", _state);
    ae_assert(ae_isfinite(beta, _state), "AutoGKSingular: Beta is not finite!", _state);
    state->wrappermode = 1;
    state->a = a;
    state->b = b;
    state->alpha = alpha;
    state->beta = beta;
    state->xwidth = 0.0;
    state->needf = ae_false;
    ae_vector_set_length(&state->rstate.ra, 10+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************

  -- ALGLIB --
     Copyright 07.05.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool autogkiteration(autogkstate* state, ae_state *_state)
{
    double s;
    double tmp;
    double eps;
    double a;
    double b;
    double x;
    double t;
    double alpha;
    double beta;
    double v1;
    double v2;
    ae_bool result;


    
    /*
     * Reverse communication preparations
     * I know it looks ugly, but it works the same way
     * anywhere from C++ to Python.
     *
     * This code initializes locals by:
     * * random values determined during code
     *   generation - on first subroutine call
     * * values from previous call - on subsequent calls
     */
    if( state->rstate.stage>=0 )
    {
        s = state->rstate.ra.ptr.p_double[0];
        tmp = state->rstate.ra.ptr.p_double[1];
        eps = state->rstate.ra.ptr.p_double[2];
        a = state->rstate.ra.ptr.p_double[3];
        b = state->rstate.ra.ptr.p_double[4];
        x = state->rstate.ra.ptr.p_double[5];
        t = state->rstate.ra.ptr.p_double[6];
        alpha = state->rstate.ra.ptr.p_double[7];
        beta = state->rstate.ra.ptr.p_double[8];
        v1 = state->rstate.ra.ptr.p_double[9];
        v2 = state->rstate.ra.ptr.p_double[10];
    }
    else
    {
        s = -983;
        tmp = -989;
        eps = -834;
        a = 900;
        b = -287;
        x = 364;
        t = 214;
        alpha = -338;
        beta = -686;
        v1 = 912;
        v2 = 585;
    }
    if( state->rstate.stage==0 )
    {
        goto lbl_0;
    }
    if( state->rstate.stage==1 )
    {
        goto lbl_1;
    }
    if( state->rstate.stage==2 )
    {
        goto lbl_2;
    }
    
    /*
     * Routine body
     */
    eps = (double)(0);
    a = state->a;
    b = state->b;
    alpha = state->alpha;
    beta = state->beta;
    state->terminationtype = -1;
    state->nfev = 0;
    state->nintervals = 0;
    
    /*
     * smooth function  at a finite interval
     */
    if( state->wrappermode!=0 )
    {
        goto lbl_3;
    }
    
    /*
     * special case
     */
    if( ae_fp_eq(a,b) )
    {
        state->terminationtype = 1;
        state->v = (double)(0);
        result = ae_false;
        return result;
    }
    
    /*
     * general case
     */
    autogk_autogkinternalprepare(a, b, eps, state->xwidth, &state->internalstate, _state);
lbl_5:
    if( !autogk_autogkinternaliteration(&state->internalstate, _state) )
    {
        goto lbl_6;
    }
    x = state->internalstate.x;
    state->x = x;
    state->xminusa = x-a;
    state->bminusx = b-x;
    state->needf = ae_true;
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    state->needf = ae_false;
    state->nfev = state->nfev+1;
    state->internalstate.f = state->f;
    goto lbl_5;
lbl_6:
    state->v = state->internalstate.r;
    state->terminationtype = state->internalstate.info;
    state->nintervals = state->internalstate.heapused;
    result = ae_false;
    return result;
lbl_3:
    
    /*
     * function with power-law singularities at the ends of a finite interval
     */
    if( state->wrappermode!=1 )
    {
        goto lbl_7;
    }
    
    /*
     * test coefficients
     */
    if( ae_fp_less_eq(alpha,(double)(-1))||ae_fp_less_eq(beta,(double)(-1)) )
    {
        state->terminationtype = -1;
        state->v = (double)(0);
        result = ae_false;
        return result;
    }
    
    /*
     * special cases
     */
    if( ae_fp_eq(a,b) )
    {
        state->terminationtype = 1;
        state->v = (double)(0);
        result = ae_false;
        return result;
    }
    
    /*
     * reduction to general form
     */
    if( ae_fp_less(a,b) )
    {
        s = (double)(1);
    }
    else
    {
        s = (double)(-1);
        tmp = a;
        a = b;
        b = tmp;
        tmp = alpha;
        alpha = beta;
        beta = tmp;
    }
    alpha = ae_minreal(alpha, (double)(0), _state);
    beta = ae_minreal(beta, (double)(0), _state);
    
    /*
     * first, integrate left half of [a,b]:
     *     integral(f(x)dx, a, (b+a)/2) =
     *     = 1/(1+alpha) * integral(t^(-alpha/(1+alpha))*f(a+t^(1/(1+alpha)))dt, 0, (0.5*(b-a))^(1+alpha))
     */
    autogk_autogkinternalprepare((double)(0), ae_pow(0.5*(b-a), 1+alpha, _state), eps, state->xwidth, &state->internalstate, _state);
lbl_9:
    if( !autogk_autogkinternaliteration(&state->internalstate, _state) )
    {
        goto lbl_10;
    }
    
    /*
     * Fill State.X, State.XMinusA, State.BMinusX.
     * Latter two are filled correctly even if B<A.
     */
    x = state->internalstate.x;
    t = ae_pow(x, 1/(1+alpha), _state);
    state->x = a+t;
    if( ae_fp_greater(s,(double)(0)) )
    {
        state->xminusa = t;
        state->bminusx = b-(a+t);
    }
    else
    {
        state->xminusa = a+t-b;
        state->bminusx = -t;
    }
    state->needf = ae_true;
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    state->needf = ae_false;
    if( ae_fp_neq(alpha,(double)(0)) )
    {
        state->internalstate.f = state->f*ae_pow(x, -alpha/(1+alpha), _state)/(1+alpha);
    }
    else
    {
        state->internalstate.f = state->f;
    }
    state->nfev = state->nfev+1;
    goto lbl_9;
lbl_10:
    v1 = state->internalstate.r;
    state->nintervals = state->nintervals+state->internalstate.heapused;
    
    /*
     * then, integrate right half of [a,b]:
     *     integral(f(x)dx, (b+a)/2, b) =
     *     = 1/(1+beta) * integral(t^(-beta/(1+beta))*f(b-t^(1/(1+beta)))dt, 0, (0.5*(b-a))^(1+beta))
     */
    autogk_autogkinternalprepare((double)(0), ae_pow(0.5*(b-a), 1+beta, _state), eps, state->xwidth, &state->internalstate, _state);
lbl_11:
    if( !autogk_autogkinternaliteration(&state->internalstate, _state) )
    {
        goto lbl_12;
    }
    
    /*
     * Fill State.X, State.XMinusA, State.BMinusX.
     * Latter two are filled correctly (X-A, B-X) even if B<A.
     */
    x = state->internalstate.x;
    t = ae_pow(x, 1/(1+beta), _state);
    state->x = b-t;
    if( ae_fp_greater(s,(double)(0)) )
    {
        state->xminusa = b-t-a;
        state->bminusx = t;
    }
    else
    {
        state->xminusa = -t;
        state->bminusx = a-(b-t);
    }
    state->needf = ae_true;
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    state->needf = ae_false;
    if( ae_fp_neq(beta,(double)(0)) )
    {
        state->internalstate.f = state->f*ae_pow(x, -beta/(1+beta), _state)/(1+beta);
    }
    else
    {
        state->internalstate.f = state->f;
    }
    state->nfev = state->nfev+1;
    goto lbl_11;
lbl_12:
    v2 = state->internalstate.r;
    state->nintervals = state->nintervals+state->internalstate.heapused;
    
    /*
     * final result
     */
    state->v = s*(v1+v2);
    state->terminationtype = 1;
    result = ae_false;
    return result;
lbl_7:
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstate.ra.ptr.p_double[0] = s;
    state->rstate.ra.ptr.p_double[1] = tmp;
    state->rstate.ra.ptr.p_double[2] = eps;
    state->rstate.ra.ptr.p_double[3] = a;
    state->rstate.ra.ptr.p_double[4] = b;
    state->rstate.ra.ptr.p_double[5] = x;
    state->rstate.ra.ptr.p_double[6] = t;
    state->rstate.ra.ptr.p_double[7] = alpha;
    state->rstate.ra.ptr.p_double[8] = beta;
    state->rstate.ra.ptr.p_double[9] = v1;
    state->rstate.ra.ptr.p_double[10] = v2;
    return result;
}


/*************************************************************************
Adaptive integration results

Called after AutoGKIteration returned False.

Input parameters:
    State   -   algorithm state (used by AutoGKIteration).

Output parameters:
    V       -   integral(f(x)dx,a,b)
    Rep     -   optimization report (see AutoGKReport description)

  -- ALGLIB --
     Copyright 14.11.2007 by Bochkanov Sergey
*************************************************************************/
void autogkresults(autogkstate* state,
     double* v,
     autogkreport* rep,
     ae_state *_state)
{

    *v = 0;
    _autogkreport_clear(rep);

    *v = state->v;
    rep->terminationtype = state->terminationtype;
    rep->nfev = state->nfev;
    rep->nintervals = state->nintervals;
}


/*************************************************************************
Internal AutoGK subroutine
eps<0   - error
eps=0   - automatic eps selection

width<0 -   error
width=0 -   no width requirements
*************************************************************************/
static void autogk_autogkinternalprepare(double a,
     double b,
     double eps,
     double xwidth,
     autogkinternalstate* state,
     ae_state *_state)
{


    
    /*
     * Save settings
     */
    state->a = a;
    state->b = b;
    state->eps = eps;
    state->xwidth = xwidth;
    
    /*
     * Prepare RComm structure
     */
    ae_vector_set_length(&state->rstate.ia, 3+1, _state);
    ae_vector_set_length(&state->rstate.ra, 8+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
Internal AutoGK subroutine
*************************************************************************/
static ae_bool autogk_autogkinternaliteration(autogkinternalstate* state,
     ae_state *_state)
{
    double c1;
    double c2;
    ae_int_t i;
    ae_int_t j;
    double intg;
    double intk;
    double inta;
    double v;
    double ta;
    double tb;
    ae_int_t ns;
    double qeps;
    ae_int_t info;
    ae_bool result;


    
    /*
     * Reverse communication preparations
     * I know it looks ugly, but it works the same way
     * anywhere from C++ to Python.
     *
     * This code initializes locals by:
     * * random values determined during code
     *   generation - on first subroutine call
     * * values from previous call - on subsequent calls
     */
    if( state->rstate.stage>=0 )
    {
        i = state->rstate.ia.ptr.p_int[0];
        j = state->rstate.ia.ptr.p_int[1];
        ns = state->rstate.ia.ptr.p_int[2];
        info = state->rstate.ia.ptr.p_int[3];
        c1 = state->rstate.ra.ptr.p_double[0];
        c2 = state->rstate.ra.ptr.p_double[1];
        intg = state->rstate.ra.ptr.p_double[2];
        intk = state->rstate.ra.ptr.p_double[3];
        inta = state->rstate.ra.ptr.p_double[4];
        v = state->rstate.ra.ptr.p_double[5];
        ta = state->rstate.ra.ptr.p_double[6];
        tb = state->rstate.ra.ptr.p_double[7];
        qeps = state->rstate.ra.ptr.p_double[8];
    }
    else
    {
        i = 497;
        j = -271;
        ns = -581;
        info = 745;
        c1 = -533;
        c2 = -77;
        intg = 678;
        intk = -293;
        inta = 316;
        v = 647;
        ta = -756;
        tb = 830;
        qeps = -871;
    }
    if( state->rstate.stage==0 )
    {
        goto lbl_0;
    }
    if( state->rstate.stage==1 )
    {
        goto lbl_1;
    }
    if( state->rstate.stage==2 )
    {
        goto lbl_2;
    }
    
    /*
     * Routine body
     */
    
    /*
     * initialize quadratures.
     * use 15-point Gauss-Kronrod formula.
     */
    state->n = 15;
    gkqgenerategausslegendre(state->n, &info, &state->qn, &state->wk, &state->wg, _state);
    if( info<0 )
    {
        state->info = -5;
        state->r = (double)(0);
        result = ae_false;
        return result;
    }
    ae_vector_set_length(&state->wr, state->n, _state);
    for(i=0; i<=state->n-1; i++)
    {
        if( i==0 )
        {
            state->wr.ptr.p_double[i] = 0.5*ae_fabs(state->qn.ptr.p_double[1]-state->qn.ptr.p_double[0], _state);
            continue;
        }
        if( i==state->n-1 )
        {
            state->wr.ptr.p_double[state->n-1] = 0.5*ae_fabs(state->qn.ptr.p_double[state->n-1]-state->qn.ptr.p_double[state->n-2], _state);
            continue;
        }
        state->wr.ptr.p_double[i] = 0.5*ae_fabs(state->qn.ptr.p_double[i-1]-state->qn.ptr.p_double[i+1], _state);
    }
    
    /*
     * special case
     */
    if( ae_fp_eq(state->a,state->b) )
    {
        state->info = 1;
        state->r = (double)(0);
        result = ae_false;
        return result;
    }
    
    /*
     * test parameters
     */
    if( ae_fp_less(state->eps,(double)(0))||ae_fp_less(state->xwidth,(double)(0)) )
    {
        state->info = -1;
        state->r = (double)(0);
        result = ae_false;
        return result;
    }
    state->info = 1;
    if( ae_fp_eq(state->eps,(double)(0)) )
    {
        state->eps = 100000*ae_machineepsilon;
    }
    
    /*
     * First, prepare heap
     * * column 0   -   absolute error
     * * column 1   -   integral of a F(x) (calculated using Kronrod extension nodes)
     * * column 2   -   integral of a |F(x)| (calculated using modified rect. method)
     * * column 3   -   left boundary of a subinterval
     * * column 4   -   right boundary of a subinterval
     */
    if( ae_fp_neq(state->xwidth,(double)(0)) )
    {
        goto lbl_3;
    }
    
    /*
     * no maximum width requirements
     * start from one big subinterval
     */
    state->heapwidth = 5;
    state->heapsize = 1;
    state->heapused = 1;
    ae_matrix_set_length(&state->heap, state->heapsize, state->heapwidth, _state);
    c1 = 0.5*(state->b-state->a);
    c2 = 0.5*(state->b+state->a);
    intg = (double)(0);
    intk = (double)(0);
    inta = (double)(0);
    i = 0;
lbl_5:
    if( i>state->n-1 )
    {
        goto lbl_7;
    }
    
    /*
     * obtain F
     */
    state->x = c1*state->qn.ptr.p_double[i]+c2;
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    v = state->f;
    
    /*
     * Gauss-Kronrod formula
     */
    intk = intk+v*state->wk.ptr.p_double[i];
    if( i%2==1 )
    {
        intg = intg+v*state->wg.ptr.p_double[i];
    }
    
    /*
     * Integral |F(x)|
     * Use rectangles method
     */
    inta = inta+ae_fabs(v, _state)*state->wr.ptr.p_double[i];
    i = i+1;
    goto lbl_5;
lbl_7:
    intk = intk*(state->b-state->a)*0.5;
    intg = intg*(state->b-state->a)*0.5;
    inta = inta*(state->b-state->a)*0.5;
    state->heap.ptr.pp_double[0][0] = ae_fabs(intg-intk, _state);
    state->heap.ptr.pp_double[0][1] = intk;
    state->heap.ptr.pp_double[0][2] = inta;
    state->heap.ptr.pp_double[0][3] = state->a;
    state->heap.ptr.pp_double[0][4] = state->b;
    state->sumerr = state->heap.ptr.pp_double[0][0];
    state->sumabs = ae_fabs(inta, _state);
    goto lbl_4;
lbl_3:
    
    /*
     * maximum subinterval should be no more than XWidth.
     * so we create Ceil((B-A)/XWidth)+1 small subintervals
     */
    ns = ae_iceil(ae_fabs(state->b-state->a, _state)/state->xwidth, _state)+1;
    state->heapsize = ns;
    state->heapused = ns;
    state->heapwidth = 5;
    ae_matrix_set_length(&state->heap, state->heapsize, state->heapwidth, _state);
    state->sumerr = (double)(0);
    state->sumabs = (double)(0);
    j = 0;
lbl_8:
    if( j>ns-1 )
    {
        goto lbl_10;
    }
    ta = state->a+j*(state->b-state->a)/ns;
    tb = state->a+(j+1)*(state->b-state->a)/ns;
    c1 = 0.5*(tb-ta);
    c2 = 0.5*(tb+ta);
    intg = (double)(0);
    intk = (double)(0);
    inta = (double)(0);
    i = 0;
lbl_11:
    if( i>state->n-1 )
    {
        goto lbl_13;
    }
    
    /*
     * obtain F
     */
    state->x = c1*state->qn.ptr.p_double[i]+c2;
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    v = state->f;
    
    /*
     * Gauss-Kronrod formula
     */
    intk = intk+v*state->wk.ptr.p_double[i];
    if( i%2==1 )
    {
        intg = intg+v*state->wg.ptr.p_double[i];
    }
    
    /*
     * Integral |F(x)|
     * Use rectangles method
     */
    inta = inta+ae_fabs(v, _state)*state->wr.ptr.p_double[i];
    i = i+1;
    goto lbl_11;
lbl_13:
    intk = intk*(tb-ta)*0.5;
    intg = intg*(tb-ta)*0.5;
    inta = inta*(tb-ta)*0.5;
    state->heap.ptr.pp_double[j][0] = ae_fabs(intg-intk, _state);
    state->heap.ptr.pp_double[j][1] = intk;
    state->heap.ptr.pp_double[j][2] = inta;
    state->heap.ptr.pp_double[j][3] = ta;
    state->heap.ptr.pp_double[j][4] = tb;
    state->sumerr = state->sumerr+state->heap.ptr.pp_double[j][0];
    state->sumabs = state->sumabs+ae_fabs(inta, _state);
    j = j+1;
    goto lbl_8;
lbl_10:
lbl_4:
    
    /*
     * method iterations
     */
lbl_14:
    if( ae_false )
    {
        goto lbl_15;
    }
    
    /*
     * additional memory if needed
     */
    if( state->heapused==state->heapsize )
    {
        autogk_mheapresize(&state->heap, &state->heapsize, 4*state->heapsize, state->heapwidth, _state);
    }
    
    /*
     * TODO: every 20 iterations recalculate errors/sums
     */
    if( ae_fp_less_eq(state->sumerr,state->eps*state->sumabs)||state->heapused>=autogk_maxsubintervals )
    {
        state->r = (double)(0);
        for(j=0; j<=state->heapused-1; j++)
        {
            state->r = state->r+state->heap.ptr.pp_double[j][1];
        }
        result = ae_false;
        return result;
    }
    
    /*
     * Exclude interval with maximum absolute error
     */
    autogk_mheappop(&state->heap, state->heapused, state->heapwidth, _state);
    state->sumerr = state->sumerr-state->heap.ptr.pp_double[state->heapused-1][0];
    state->sumabs = state->sumabs-state->heap.ptr.pp_double[state->heapused-1][2];
    
    /*
     * Divide interval, create subintervals
     */
    ta = state->heap.ptr.pp_double[state->heapused-1][3];
    tb = state->heap.ptr.pp_double[state->heapused-1][4];
    state->heap.ptr.pp_double[state->heapused-1][3] = ta;
    state->heap.ptr.pp_double[state->heapused-1][4] = 0.5*(ta+tb);
    state->heap.ptr.pp_double[state->heapused][3] = 0.5*(ta+tb);
    state->heap.ptr.pp_double[state->heapused][4] = tb;
    j = state->heapused-1;
lbl_16:
    if( j>state->heapused )
    {
        goto lbl_18;
    }
    c1 = 0.5*(state->heap.ptr.pp_double[j][4]-state->heap.ptr.pp_double[j][3]);
    c2 = 0.5*(state->heap.ptr.pp_double[j][4]+state->heap.ptr.pp_double[j][3]);
    intg = (double)(0);
    intk = (double)(0);
    inta = (double)(0);
    i = 0;
lbl_19:
    if( i>state->n-1 )
    {
        goto lbl_21;
    }
    
    /*
     * F(x)
     */
    state->x = c1*state->qn.ptr.p_double[i]+c2;
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    v = state->f;
    
    /*
     * Gauss-Kronrod formula
     */
    intk = intk+v*state->wk.ptr.p_double[i];
    if( i%2==1 )
    {
        intg = intg+v*state->wg.ptr.p_double[i];
    }
    
    /*
     * Integral |F(x)|
     * Use rectangles method
     */
    inta = inta+ae_fabs(v, _state)*state->wr.ptr.p_double[i];
    i = i+1;
    goto lbl_19;
lbl_21:
    intk = intk*(state->heap.ptr.pp_double[j][4]-state->heap.ptr.pp_double[j][3])*0.5;
    intg = intg*(state->heap.ptr.pp_double[j][4]-state->heap.ptr.pp_double[j][3])*0.5;
    inta = inta*(state->heap.ptr.pp_double[j][4]-state->heap.ptr.pp_double[j][3])*0.5;
    state->heap.ptr.pp_double[j][0] = ae_fabs(intg-intk, _state);
    state->heap.ptr.pp_double[j][1] = intk;
    state->heap.ptr.pp_double[j][2] = inta;
    state->sumerr = state->sumerr+state->heap.ptr.pp_double[j][0];
    state->sumabs = state->sumabs+state->heap.ptr.pp_double[j][2];
    j = j+1;
    goto lbl_16;
lbl_18:
    autogk_mheappush(&state->heap, state->heapused-1, state->heapwidth, _state);
    autogk_mheappush(&state->heap, state->heapused, state->heapwidth, _state);
    state->heapused = state->heapused+1;
    goto lbl_14;
lbl_15:
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstate.ia.ptr.p_int[0] = i;
    state->rstate.ia.ptr.p_int[1] = j;
    state->rstate.ia.ptr.p_int[2] = ns;
    state->rstate.ia.ptr.p_int[3] = info;
    state->rstate.ra.ptr.p_double[0] = c1;
    state->rstate.ra.ptr.p_double[1] = c2;
    state->rstate.ra.ptr.p_double[2] = intg;
    state->rstate.ra.ptr.p_double[3] = intk;
    state->rstate.ra.ptr.p_double[4] = inta;
    state->rstate.ra.ptr.p_double[5] = v;
    state->rstate.ra.ptr.p_double[6] = ta;
    state->rstate.ra.ptr.p_double[7] = tb;
    state->rstate.ra.ptr.p_double[8] = qeps;
    return result;
}


static void autogk_mheappop(/* Real    */ ae_matrix* heap,
     ae_int_t heapsize,
     ae_int_t heapwidth,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t p;
    double t;
    ae_int_t maxcp;


    if( heapsize==1 )
    {
        return;
    }
    for(i=0; i<=heapwidth-1; i++)
    {
        t = heap->ptr.pp_double[heapsize-1][i];
        heap->ptr.pp_double[heapsize-1][i] = heap->ptr.pp_double[0][i];
        heap->ptr.pp_double[0][i] = t;
    }
    p = 0;
    while(2*p+1<heapsize-1)
    {
        maxcp = 2*p+1;
        if( 2*p+2<heapsize-1 )
        {
            if( ae_fp_greater(heap->ptr.pp_double[2*p+2][0],heap->ptr.pp_double[2*p+1][0]) )
            {
                maxcp = 2*p+2;
            }
        }
        if( ae_fp_less(heap->ptr.pp_double[p][0],heap->ptr.pp_double[maxcp][0]) )
        {
            for(i=0; i<=heapwidth-1; i++)
            {
                t = heap->ptr.pp_double[p][i];
                heap->ptr.pp_double[p][i] = heap->ptr.pp_double[maxcp][i];
                heap->ptr.pp_double[maxcp][i] = t;
            }
            p = maxcp;
        }
        else
        {
            break;
        }
    }
}


static void autogk_mheappush(/* Real    */ ae_matrix* heap,
     ae_int_t heapsize,
     ae_int_t heapwidth,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t p;
    double t;
    ae_int_t parent;


    if( heapsize==0 )
    {
        return;
    }
    p = heapsize;
    while(p!=0)
    {
        parent = (p-1)/2;
        if( ae_fp_greater(heap->ptr.pp_double[p][0],heap->ptr.pp_double[parent][0]) )
        {
            for(i=0; i<=heapwidth-1; i++)
            {
                t = heap->ptr.pp_double[p][i];
                heap->ptr.pp_double[p][i] = heap->ptr.pp_double[parent][i];
                heap->ptr.pp_double[parent][i] = t;
            }
            p = parent;
        }
        else
        {
            break;
        }
    }
}


static void autogk_mheapresize(/* Real    */ ae_matrix* heap,
     ae_int_t* heapsize,
     ae_int_t newheapsize,
     ae_int_t heapwidth,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix tmp;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&tmp, 0, 0, DT_REAL, _state);

    ae_matrix_set_length(&tmp, *heapsize, heapwidth, _state);
    for(i=0; i<=*heapsize-1; i++)
    {
        ae_v_move(&tmp.ptr.pp_double[i][0], 1, &heap->ptr.pp_double[i][0], 1, ae_v_len(0,heapwidth-1));
    }
    ae_matrix_set_length(heap, newheapsize, heapwidth, _state);
    for(i=0; i<=*heapsize-1; i++)
    {
        ae_v_move(&heap->ptr.pp_double[i][0], 1, &tmp.ptr.pp_double[i][0], 1, ae_v_len(0,heapwidth-1));
    }
    *heapsize = newheapsize;
    ae_frame_leave(_state);
}


void _autogkreport_init(void* _p, ae_state *_state)
{
    autogkreport *p = (autogkreport*)_p;
    ae_touch_ptr((void*)p);
}


void _autogkreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    autogkreport *dst = (autogkreport*)_dst;
    autogkreport *src = (autogkreport*)_src;
    dst->terminationtype = src->terminationtype;
    dst->nfev = src->nfev;
    dst->nintervals = src->nintervals;
}


void _autogkreport_clear(void* _p)
{
    autogkreport *p = (autogkreport*)_p;
    ae_touch_ptr((void*)p);
}


void _autogkreport_destroy(void* _p)
{
    autogkreport *p = (autogkreport*)_p;
    ae_touch_ptr((void*)p);
}


void _autogkinternalstate_init(void* _p, ae_state *_state)
{
    autogkinternalstate *p = (autogkinternalstate*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->heap, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->qn, 0, DT_REAL, _state);
    ae_vector_init(&p->wg, 0, DT_REAL, _state);
    ae_vector_init(&p->wk, 0, DT_REAL, _state);
    ae_vector_init(&p->wr, 0, DT_REAL, _state);
    _rcommstate_init(&p->rstate, _state);
}


void _autogkinternalstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    autogkinternalstate *dst = (autogkinternalstate*)_dst;
    autogkinternalstate *src = (autogkinternalstate*)_src;
    dst->a = src->a;
    dst->b = src->b;
    dst->eps = src->eps;
    dst->xwidth = src->xwidth;
    dst->x = src->x;
    dst->f = src->f;
    dst->info = src->info;
    dst->r = src->r;
    ae_matrix_init_copy(&dst->heap, &src->heap, _state);
    dst->heapsize = src->heapsize;
    dst->heapwidth = src->heapwidth;
    dst->heapused = src->heapused;
    dst->sumerr = src->sumerr;
    dst->sumabs = src->sumabs;
    ae_vector_init_copy(&dst->qn, &src->qn, _state);
    ae_vector_init_copy(&dst->wg, &src->wg, _state);
    ae_vector_init_copy(&dst->wk, &src->wk, _state);
    ae_vector_init_copy(&dst->wr, &src->wr, _state);
    dst->n = src->n;
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
}


void _autogkinternalstate_clear(void* _p)
{
    autogkinternalstate *p = (autogkinternalstate*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->heap);
    ae_vector_clear(&p->qn);
    ae_vector_clear(&p->wg);
    ae_vector_clear(&p->wk);
    ae_vector_clear(&p->wr);
    _rcommstate_clear(&p->rstate);
}


void _autogkinternalstate_destroy(void* _p)
{
    autogkinternalstate *p = (autogkinternalstate*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->heap);
    ae_vector_destroy(&p->qn);
    ae_vector_destroy(&p->wg);
    ae_vector_destroy(&p->wk);
    ae_vector_destroy(&p->wr);
    _rcommstate_destroy(&p->rstate);
}


void _autogkstate_init(void* _p, ae_state *_state)
{
    autogkstate *p = (autogkstate*)_p;
    ae_touch_ptr((void*)p);
    _autogkinternalstate_init(&p->internalstate, _state);
    _rcommstate_init(&p->rstate, _state);
}


void _autogkstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    autogkstate *dst = (autogkstate*)_dst;
    autogkstate *src = (autogkstate*)_src;
    dst->a = src->a;
    dst->b = src->b;
    dst->alpha = src->alpha;
    dst->beta = src->beta;
    dst->xwidth = src->xwidth;
    dst->x = src->x;
    dst->xminusa = src->xminusa;
    dst->bminusx = src->bminusx;
    dst->needf = src->needf;
    dst->f = src->f;
    dst->wrappermode = src->wrappermode;
    _autogkinternalstate_init_copy(&dst->internalstate, &src->internalstate, _state);
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
    dst->v = src->v;
    dst->terminationtype = src->terminationtype;
    dst->nfev = src->nfev;
    dst->nintervals = src->nintervals;
}


void _autogkstate_clear(void* _p)
{
    autogkstate *p = (autogkstate*)_p;
    ae_touch_ptr((void*)p);
    _autogkinternalstate_clear(&p->internalstate);
    _rcommstate_clear(&p->rstate);
}


void _autogkstate_destroy(void* _p)
{
    autogkstate *p = (autogkstate*)_p;
    ae_touch_ptr((void*)p);
    _autogkinternalstate_destroy(&p->internalstate);
    _rcommstate_destroy(&p->rstate);
}


/*$ End $*/
