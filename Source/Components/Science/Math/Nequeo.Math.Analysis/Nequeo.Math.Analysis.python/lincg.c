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
#include "lincg.h"


/*$ Declarations $*/
static double lincg_defaultprecision = 1.0E-6;
static void lincg_clearrfields(lincgstate* state, ae_state *_state);
static void lincg_updateitersdata(lincgstate* state, ae_state *_state);


/*$ Body $*/


/*************************************************************************
This function initializes linear CG Solver. This solver is used  to  solve
symmetric positive definite problems. If you want  to  solve  nonsymmetric
(or non-positive definite) problem you may use LinLSQR solver provided  by
ALGLIB.

USAGE:
1. User initializes algorithm state with LinCGCreate() call
2. User tunes solver parameters with  LinCGSetCond() and other functions
3. Optionally, user sets starting point with LinCGSetStartingPoint()
4. User  calls LinCGSolveSparse() function which takes algorithm state and
   SparseMatrix object.
5. User calls LinCGResults() to get solution
6. Optionally, user may call LinCGSolveSparse()  again  to  solve  another
   problem  with different matrix and/or right part without reinitializing
   LinCGState structure.
  
INPUT PARAMETERS:
    N       -   problem dimension, N>0

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
void lincgcreate(ae_int_t n, lincgstate* state, ae_state *_state)
{
    ae_int_t i;

    _lincgstate_clear(state);

    ae_assert(n>0, "LinCGCreate: N<=0", _state);
    state->n = n;
    state->prectype = 0;
    state->itsbeforerestart = n;
    state->itsbeforerupdate = 10;
    state->epsf = lincg_defaultprecision;
    state->maxits = 0;
    state->xrep = ae_false;
    state->running = ae_false;
    
    /*
     * * allocate arrays
     * * set RX to NAN (just for the case user calls Results() without 
     *   calling SolveSparse()
     * * set starting point to zero
     * * we do NOT initialize B here because we assume that user should
     *   initializate it using LinCGSetB() function. In case he forgets
     *   to do so, exception will be thrown in the LinCGIteration().
     */
    ae_vector_set_length(&state->rx, state->n, _state);
    ae_vector_set_length(&state->startx, state->n, _state);
    ae_vector_set_length(&state->b, state->n, _state);
    for(i=0; i<=state->n-1; i++)
    {
        state->rx.ptr.p_double[i] = _state->v_nan;
        state->startx.ptr.p_double[i] = 0.0;
        state->b.ptr.p_double[i] = (double)(0);
    }
    ae_vector_set_length(&state->cx, state->n, _state);
    ae_vector_set_length(&state->p, state->n, _state);
    ae_vector_set_length(&state->r, state->n, _state);
    ae_vector_set_length(&state->cr, state->n, _state);
    ae_vector_set_length(&state->z, state->n, _state);
    ae_vector_set_length(&state->cz, state->n, _state);
    ae_vector_set_length(&state->x, state->n, _state);
    ae_vector_set_length(&state->mv, state->n, _state);
    ae_vector_set_length(&state->pv, state->n, _state);
    lincg_updateitersdata(state, _state);
    ae_vector_set_length(&state->rstate.ia, 0+1, _state);
    ae_vector_set_length(&state->rstate.ra, 2+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
This function sets starting point.
By default, zero starting point is used.

INPUT PARAMETERS:
    X       -   starting point, array[N]

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
void lincgsetstartingpoint(lincgstate* state,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{


    ae_assert(!state->running, "LinCGSetStartingPoint: you can not change starting point because LinCGIteration() function is running", _state);
    ae_assert(state->n<=x->cnt, "LinCGSetStartingPoint: Length(X)<N", _state);
    ae_assert(isfinitevector(x, state->n, _state), "LinCGSetStartingPoint: X contains infinite or NaN values!", _state);
    ae_v_move(&state->startx.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,state->n-1));
}


/*************************************************************************
This function sets right part. By default, right part is zero.

INPUT PARAMETERS:
    B       -   right part, array[N].

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
void lincgsetb(lincgstate* state,
     /* Real    */ ae_vector* b,
     ae_state *_state)
{


    ae_assert(!state->running, "LinCGSetB: you can not set B, because function LinCGIteration is running!", _state);
    ae_assert(b->cnt>=state->n, "LinCGSetB: Length(B)<N", _state);
    ae_assert(isfinitevector(b, state->n, _state), "LinCGSetB: B contains infinite or NaN values!", _state);
    ae_v_move(&state->b.ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,state->n-1));
}


/*************************************************************************
This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
function. By default, SolveSparse() uses diagonal preconditioner,  but  if
you want to use solver without preconditioning, you can call this function
which forces solver to use unit matrix for preconditioning.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 19.11.2012 by Bochkanov Sergey
*************************************************************************/
void lincgsetprecunit(lincgstate* state, ae_state *_state)
{


    ae_assert(!state->running, "LinCGSetPrecUnit: you can not change preconditioner, because function LinCGIteration is running!", _state);
    state->prectype = -1;
}


/*************************************************************************
This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
function.  LinCGSolveSparse() will use diagonal of the  system  matrix  as
preconditioner. This preconditioning mode is active by default.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 19.11.2012 by Bochkanov Sergey
*************************************************************************/
void lincgsetprecdiag(lincgstate* state, ae_state *_state)
{


    ae_assert(!state->running, "LinCGSetPrecDiag: you can not change preconditioner, because function LinCGIteration is running!", _state);
    state->prectype = 0;
}


/*************************************************************************
This function sets stopping criteria.

INPUT PARAMETERS:
    EpsF    -   algorithm will be stopped if norm of residual is less than 
                EpsF*||b||.
    MaxIts  -   algorithm will be stopped if number of iterations is  more 
                than MaxIts.

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

NOTES:
If  both  EpsF  and  MaxIts  are  zero then small EpsF will be set to small 
value.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
void lincgsetcond(lincgstate* state,
     double epsf,
     ae_int_t maxits,
     ae_state *_state)
{


    ae_assert(!state->running, "LinCGSetCond: you can not change stopping criteria when LinCGIteration() is running", _state);
    ae_assert(ae_isfinite(epsf, _state)&&ae_fp_greater_eq(epsf,(double)(0)), "LinCGSetCond: EpsF is negative or contains infinite or NaN values", _state);
    ae_assert(maxits>=0, "LinCGSetCond: MaxIts is negative", _state);
    if( ae_fp_eq(epsf,(double)(0))&&maxits==0 )
    {
        state->epsf = lincg_defaultprecision;
        state->maxits = maxits;
    }
    else
    {
        state->epsf = epsf;
        state->maxits = maxits;
    }
}


/*************************************************************************
Reverse communication version of linear CG.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool lincgiteration(lincgstate* state, ae_state *_state)
{
    ae_int_t i;
    double uvar;
    double bnorm;
    double v;
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
        uvar = state->rstate.ra.ptr.p_double[0];
        bnorm = state->rstate.ra.ptr.p_double[1];
        v = state->rstate.ra.ptr.p_double[2];
    }
    else
    {
        i = -983;
        uvar = -989;
        bnorm = -834;
        v = 900;
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
    if( state->rstate.stage==3 )
    {
        goto lbl_3;
    }
    if( state->rstate.stage==4 )
    {
        goto lbl_4;
    }
    if( state->rstate.stage==5 )
    {
        goto lbl_5;
    }
    if( state->rstate.stage==6 )
    {
        goto lbl_6;
    }
    if( state->rstate.stage==7 )
    {
        goto lbl_7;
    }
    
    /*
     * Routine body
     */
    ae_assert(state->b.cnt>0, "LinCGIteration: B is not initialized (you must initialize B by LinCGSetB() call", _state);
    state->running = ae_true;
    state->repnmv = 0;
    lincg_clearrfields(state, _state);
    lincg_updateitersdata(state, _state);
    
    /*
     * Start 0-th iteration
     */
    ae_v_move(&state->rx.ptr.p_double[0], 1, &state->startx.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->rx.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    state->repnmv = state->repnmv+1;
    lincg_clearrfields(state, _state);
    state->needvmv = ae_true;
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    state->needvmv = ae_false;
    bnorm = (double)(0);
    state->r2 = (double)(0);
    state->meritfunction = (double)(0);
    for(i=0; i<=state->n-1; i++)
    {
        state->r.ptr.p_double[i] = state->b.ptr.p_double[i]-state->mv.ptr.p_double[i];
        state->r2 = state->r2+state->r.ptr.p_double[i]*state->r.ptr.p_double[i];
        state->meritfunction = state->meritfunction+state->mv.ptr.p_double[i]*state->rx.ptr.p_double[i]-2*state->b.ptr.p_double[i]*state->rx.ptr.p_double[i];
        bnorm = bnorm+state->b.ptr.p_double[i]*state->b.ptr.p_double[i];
    }
    bnorm = ae_sqrt(bnorm, _state);
    
    /*
     * Output first report
     */
    if( !state->xrep )
    {
        goto lbl_8;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->rx.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    lincg_clearrfields(state, _state);
    state->xupdated = ae_true;
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    state->xupdated = ae_false;
lbl_8:
    
    /*
     * Is x0 a solution?
     */
    if( !ae_isfinite(state->r2, _state)||ae_fp_less_eq(ae_sqrt(state->r2, _state),state->epsf*bnorm) )
    {
        state->running = ae_false;
        if( ae_isfinite(state->r2, _state) )
        {
            state->repterminationtype = 1;
        }
        else
        {
            state->repterminationtype = -4;
        }
        result = ae_false;
        return result;
    }
    
    /*
     * Calculate Z and P
     */
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->r.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    state->repnmv = state->repnmv+1;
    lincg_clearrfields(state, _state);
    state->needprec = ae_true;
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    state->needprec = ae_false;
    for(i=0; i<=state->n-1; i++)
    {
        state->z.ptr.p_double[i] = state->pv.ptr.p_double[i];
        state->p.ptr.p_double[i] = state->z.ptr.p_double[i];
    }
    
    /*
     * Other iterations(1..N)
     */
    state->repiterationscount = 0;
lbl_10:
    if( ae_false )
    {
        goto lbl_11;
    }
    state->repiterationscount = state->repiterationscount+1;
    
    /*
     * Calculate Alpha
     */
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->p.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    state->repnmv = state->repnmv+1;
    lincg_clearrfields(state, _state);
    state->needvmv = ae_true;
    state->rstate.stage = 3;
    goto lbl_rcomm;
lbl_3:
    state->needvmv = ae_false;
    if( !ae_isfinite(state->vmv, _state)||ae_fp_less_eq(state->vmv,(double)(0)) )
    {
        
        /*
         * a) Overflow when calculating VMV
         * b) non-positive VMV (non-SPD matrix)
         */
        state->running = ae_false;
        if( ae_isfinite(state->vmv, _state) )
        {
            state->repterminationtype = -5;
        }
        else
        {
            state->repterminationtype = -4;
        }
        result = ae_false;
        return result;
    }
    state->alpha = (double)(0);
    for(i=0; i<=state->n-1; i++)
    {
        state->alpha = state->alpha+state->r.ptr.p_double[i]*state->z.ptr.p_double[i];
    }
    state->alpha = state->alpha/state->vmv;
    if( !ae_isfinite(state->alpha, _state) )
    {
        
        /*
         * Overflow when calculating Alpha
         */
        state->running = ae_false;
        state->repterminationtype = -4;
        result = ae_false;
        return result;
    }
    
    /*
     * Next step toward solution
     */
    for(i=0; i<=state->n-1; i++)
    {
        state->cx.ptr.p_double[i] = state->rx.ptr.p_double[i]+state->alpha*state->p.ptr.p_double[i];
    }
    
    /*
     * Calculate R:
     * * use recurrent relation to update R
     * * at every ItsBeforeRUpdate-th iteration recalculate it from scratch, using matrix-vector product
     *   in case R grows instead of decreasing, algorithm is terminated with positive completion code
     */
    if( !(state->itsbeforerupdate==0||state->repiterationscount%state->itsbeforerupdate!=0) )
    {
        goto lbl_12;
    }
    
    /*
     * Calculate R using recurrent formula
     */
    for(i=0; i<=state->n-1; i++)
    {
        state->cr.ptr.p_double[i] = state->r.ptr.p_double[i]-state->alpha*state->mv.ptr.p_double[i];
        state->x.ptr.p_double[i] = state->cr.ptr.p_double[i];
    }
    goto lbl_13;
lbl_12:
    
    /*
     * Calculate R using matrix-vector multiplication
     */
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->cx.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    state->repnmv = state->repnmv+1;
    lincg_clearrfields(state, _state);
    state->needmv = ae_true;
    state->rstate.stage = 4;
    goto lbl_rcomm;
lbl_4:
    state->needmv = ae_false;
    for(i=0; i<=state->n-1; i++)
    {
        state->cr.ptr.p_double[i] = state->b.ptr.p_double[i]-state->mv.ptr.p_double[i];
        state->x.ptr.p_double[i] = state->cr.ptr.p_double[i];
    }
    
    /*
     * Calculating merit function
     * Check emergency stopping criterion
     */
    v = (double)(0);
    for(i=0; i<=state->n-1; i++)
    {
        v = v+state->mv.ptr.p_double[i]*state->cx.ptr.p_double[i]-2*state->b.ptr.p_double[i]*state->cx.ptr.p_double[i];
    }
    if( ae_fp_less(v,state->meritfunction) )
    {
        goto lbl_14;
    }
    for(i=0; i<=state->n-1; i++)
    {
        if( !ae_isfinite(state->rx.ptr.p_double[i], _state) )
        {
            state->running = ae_false;
            state->repterminationtype = -4;
            result = ae_false;
            return result;
        }
    }
    
    /*
     *output last report
     */
    if( !state->xrep )
    {
        goto lbl_16;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->rx.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    lincg_clearrfields(state, _state);
    state->xupdated = ae_true;
    state->rstate.stage = 5;
    goto lbl_rcomm;
lbl_5:
    state->xupdated = ae_false;
lbl_16:
    state->running = ae_false;
    state->repterminationtype = 7;
    result = ae_false;
    return result;
lbl_14:
    state->meritfunction = v;
lbl_13:
    ae_v_move(&state->rx.ptr.p_double[0], 1, &state->cx.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    
    /*
     * calculating RNorm
     *
     * NOTE: monotonic decrease of R2 is not guaranteed by algorithm.
     */
    state->r2 = (double)(0);
    for(i=0; i<=state->n-1; i++)
    {
        state->r2 = state->r2+state->cr.ptr.p_double[i]*state->cr.ptr.p_double[i];
    }
    
    /*
     *output report
     */
    if( !state->xrep )
    {
        goto lbl_18;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->rx.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    lincg_clearrfields(state, _state);
    state->xupdated = ae_true;
    state->rstate.stage = 6;
    goto lbl_rcomm;
lbl_6:
    state->xupdated = ae_false;
lbl_18:
    
    /*
     *stopping criterion
     *achieved the required precision
     */
    if( !ae_isfinite(state->r2, _state)||ae_fp_less_eq(ae_sqrt(state->r2, _state),state->epsf*bnorm) )
    {
        state->running = ae_false;
        if( ae_isfinite(state->r2, _state) )
        {
            state->repterminationtype = 1;
        }
        else
        {
            state->repterminationtype = -4;
        }
        result = ae_false;
        return result;
    }
    if( state->repiterationscount>=state->maxits&&state->maxits>0 )
    {
        for(i=0; i<=state->n-1; i++)
        {
            if( !ae_isfinite(state->rx.ptr.p_double[i], _state) )
            {
                state->running = ae_false;
                state->repterminationtype = -4;
                result = ae_false;
                return result;
            }
        }
        
        /*
         *if X is finite number
         */
        state->running = ae_false;
        state->repterminationtype = 5;
        result = ae_false;
        return result;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->cr.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    
    /*
     *prepere of parameters for next iteration
     */
    state->repnmv = state->repnmv+1;
    lincg_clearrfields(state, _state);
    state->needprec = ae_true;
    state->rstate.stage = 7;
    goto lbl_rcomm;
lbl_7:
    state->needprec = ae_false;
    ae_v_move(&state->cz.ptr.p_double[0], 1, &state->pv.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    if( state->repiterationscount%state->itsbeforerestart!=0 )
    {
        state->beta = (double)(0);
        uvar = (double)(0);
        for(i=0; i<=state->n-1; i++)
        {
            state->beta = state->beta+state->cz.ptr.p_double[i]*state->cr.ptr.p_double[i];
            uvar = uvar+state->z.ptr.p_double[i]*state->r.ptr.p_double[i];
        }
        
        /*
         *check that UVar is't INF or is't zero
         */
        if( !ae_isfinite(uvar, _state)||ae_fp_eq(uvar,(double)(0)) )
        {
            state->running = ae_false;
            state->repterminationtype = -4;
            result = ae_false;
            return result;
        }
        
        /*
         *calculate .BETA
         */
        state->beta = state->beta/uvar;
        
        /*
         *check that .BETA neither INF nor NaN
         */
        if( !ae_isfinite(state->beta, _state) )
        {
            state->running = ae_false;
            state->repterminationtype = -1;
            result = ae_false;
            return result;
        }
        for(i=0; i<=state->n-1; i++)
        {
            state->p.ptr.p_double[i] = state->cz.ptr.p_double[i]+state->beta*state->p.ptr.p_double[i];
        }
    }
    else
    {
        ae_v_move(&state->p.ptr.p_double[0], 1, &state->cz.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    }
    
    /*
     *prepere data for next iteration
     */
    for(i=0; i<=state->n-1; i++)
    {
        
        /*
         *write (k+1)th iteration to (k )th iteration
         */
        state->r.ptr.p_double[i] = state->cr.ptr.p_double[i];
        state->z.ptr.p_double[i] = state->cz.ptr.p_double[i];
    }
    goto lbl_10;
lbl_11:
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstate.ia.ptr.p_int[0] = i;
    state->rstate.ra.ptr.p_double[0] = uvar;
    state->rstate.ra.ptr.p_double[1] = bnorm;
    state->rstate.ra.ptr.p_double[2] = v;
    return result;
}


/*************************************************************************
Procedure for solution of A*x=b with sparse A.

INPUT PARAMETERS:
    State   -   algorithm state
    A       -   sparse matrix in the CRS format (you MUST contvert  it  to 
                CRS format by calling SparseConvertToCRS() function).
    IsUpper -   whether upper or lower triangle of A is used:
                * IsUpper=True  => only upper triangle is used and lower
                                   triangle is not referenced at all 
                * IsUpper=False => only lower triangle is used and upper
                                   triangle is not referenced at all
    B       -   right part, array[N]

RESULT:
    This function returns no result.
    You can get solution by calling LinCGResults()
    
NOTE: this function uses lightweight preconditioning -  multiplication  by
      inverse of diag(A). If you want, you can turn preconditioning off by
      calling LinCGSetPrecUnit(). However, preconditioning cost is low and
      preconditioner  is  very  important  for  solution  of  badly scaled
      problems.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
void lincgsolvesparse(lincgstate* state,
     sparsematrix* a,
     ae_bool isupper,
     /* Real    */ ae_vector* b,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    double v;
    double vmv;


    n = state->n;
    ae_assert(b->cnt>=state->n, "LinCGSetB: Length(B)<N", _state);
    ae_assert(isfinitevector(b, state->n, _state), "LinCGSetB: B contains infinite or NaN values!", _state);
    
    /*
     * Allocate temporaries
     */
    rvectorsetlengthatleast(&state->tmpd, n, _state);
    
    /*
     * Compute diagonal scaling matrix D
     */
    if( state->prectype==0 )
    {
        
        /*
         * Default preconditioner - inverse of matrix diagonal
         */
        for(i=0; i<=n-1; i++)
        {
            v = sparsegetdiagonal(a, i, _state);
            if( ae_fp_greater(v,(double)(0)) )
            {
                state->tmpd.ptr.p_double[i] = 1/ae_sqrt(v, _state);
            }
            else
            {
                state->tmpd.ptr.p_double[i] = (double)(1);
            }
        }
    }
    else
    {
        
        /*
         * No diagonal scaling
         */
        for(i=0; i<=n-1; i++)
        {
            state->tmpd.ptr.p_double[i] = (double)(1);
        }
    }
    
    /*
     * Solve
     */
    lincgrestart(state, _state);
    lincgsetb(state, b, _state);
    while(lincgiteration(state, _state))
    {
        
        /*
         * Process different requests from optimizer
         */
        if( state->needmv )
        {
            sparsesmv(a, isupper, &state->x, &state->mv, _state);
        }
        if( state->needvmv )
        {
            sparsesmv(a, isupper, &state->x, &state->mv, _state);
            vmv = ae_v_dotproduct(&state->x.ptr.p_double[0], 1, &state->mv.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
            state->vmv = vmv;
        }
        if( state->needprec )
        {
            for(i=0; i<=n-1; i++)
            {
                state->pv.ptr.p_double[i] = state->x.ptr.p_double[i]*ae_sqr(state->tmpd.ptr.p_double[i], _state);
            }
        }
    }
}


/*************************************************************************
CG-solver: results.

This function must be called after LinCGSolve

INPUT PARAMETERS:
    State   -   algorithm state

OUTPUT PARAMETERS:
    X       -   array[N], solution
    Rep     -   optimization report:
                * Rep.TerminationType completetion code:
                    * -5    input matrix is either not positive definite,
                            too large or too small                            
                    * -4    overflow/underflow during solution
                            (ill conditioned problem)
                    *  1    ||residual||<=EpsF*||b||
                    *  5    MaxIts steps was taken
                    *  7    rounding errors prevent further progress,
                            best point found is returned
                * Rep.IterationsCount contains iterations count
                * NMV countains number of matrix-vector calculations

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
void lincgresults(lincgstate* state,
     /* Real    */ ae_vector* x,
     lincgreport* rep,
     ae_state *_state)
{

    ae_vector_clear(x);
    _lincgreport_clear(rep);

    ae_assert(!state->running, "LinCGResult: you can not get result, because function LinCGIteration has been launched!", _state);
    if( x->cnt<state->n )
    {
        ae_vector_set_length(x, state->n, _state);
    }
    ae_v_move(&x->ptr.p_double[0], 1, &state->rx.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    rep->iterationscount = state->repiterationscount;
    rep->nmv = state->repnmv;
    rep->terminationtype = state->repterminationtype;
    rep->r2 = state->r2;
}


/*************************************************************************
This function sets restart frequency. By default, algorithm  is  restarted
after N subsequent iterations.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
void lincgsetrestartfreq(lincgstate* state,
     ae_int_t srf,
     ae_state *_state)
{


    ae_assert(!state->running, "LinCGSetRestartFreq: you can not change restart frequency when LinCGIteration() is running", _state);
    ae_assert(srf>0, "LinCGSetRestartFreq: non-positive SRF", _state);
    state->itsbeforerestart = srf;
}


/*************************************************************************
This function sets frequency of residual recalculations.

Algorithm updates residual r_k using iterative formula,  but  recalculates
it from scratch after each 10 iterations. It is done to avoid accumulation
of numerical errors and to stop algorithm when r_k starts to grow.

Such low update frequence (1/10) gives very  little  overhead,  but  makes
algorithm a bit more robust against numerical errors. However, you may
change it 

INPUT PARAMETERS:
    Freq    -   desired update frequency, Freq>=0.
                Zero value means that no updates will be done.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
void lincgsetrupdatefreq(lincgstate* state,
     ae_int_t freq,
     ae_state *_state)
{


    ae_assert(!state->running, "LinCGSetRUpdateFreq: you can not change update frequency when LinCGIteration() is running", _state);
    ae_assert(freq>=0, "LinCGSetRUpdateFreq: non-positive Freq", _state);
    state->itsbeforerupdate = freq;
}


/*************************************************************************
This function turns on/off reporting.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    NeedXRep-   whether iteration reports are needed or not

If NeedXRep is True, algorithm will call rep() callback function if  it is
provided to MinCGOptimize().

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
void lincgsetxrep(lincgstate* state, ae_bool needxrep, ae_state *_state)
{


    state->xrep = needxrep;
}


/*************************************************************************
Procedure for restart function LinCGIteration

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
void lincgrestart(lincgstate* state, ae_state *_state)
{


    ae_vector_set_length(&state->rstate.ia, 0+1, _state);
    ae_vector_set_length(&state->rstate.ra, 2+1, _state);
    state->rstate.stage = -1;
    lincg_clearrfields(state, _state);
}


/*************************************************************************
Clears request fileds (to be sure that we don't forgot to clear something)
*************************************************************************/
static void lincg_clearrfields(lincgstate* state, ae_state *_state)
{


    state->xupdated = ae_false;
    state->needmv = ae_false;
    state->needmtv = ae_false;
    state->needmv2 = ae_false;
    state->needvmv = ae_false;
    state->needprec = ae_false;
}


/*************************************************************************
Clears request fileds (to be sure that we don't forgot to clear something)
*************************************************************************/
static void lincg_updateitersdata(lincgstate* state, ae_state *_state)
{


    state->repiterationscount = 0;
    state->repnmv = 0;
    state->repterminationtype = 0;
}


void _lincgstate_init(void* _p, ae_state *_state)
{
    lincgstate *p = (lincgstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->rx, 0, DT_REAL, _state);
    ae_vector_init(&p->b, 0, DT_REAL, _state);
    ae_vector_init(&p->cx, 0, DT_REAL, _state);
    ae_vector_init(&p->cr, 0, DT_REAL, _state);
    ae_vector_init(&p->cz, 0, DT_REAL, _state);
    ae_vector_init(&p->p, 0, DT_REAL, _state);
    ae_vector_init(&p->r, 0, DT_REAL, _state);
    ae_vector_init(&p->z, 0, DT_REAL, _state);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->mv, 0, DT_REAL, _state);
    ae_vector_init(&p->pv, 0, DT_REAL, _state);
    ae_vector_init(&p->startx, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpd, 0, DT_REAL, _state);
    _rcommstate_init(&p->rstate, _state);
}


void _lincgstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    lincgstate *dst = (lincgstate*)_dst;
    lincgstate *src = (lincgstate*)_src;
    ae_vector_init_copy(&dst->rx, &src->rx, _state);
    ae_vector_init_copy(&dst->b, &src->b, _state);
    dst->n = src->n;
    dst->prectype = src->prectype;
    ae_vector_init_copy(&dst->cx, &src->cx, _state);
    ae_vector_init_copy(&dst->cr, &src->cr, _state);
    ae_vector_init_copy(&dst->cz, &src->cz, _state);
    ae_vector_init_copy(&dst->p, &src->p, _state);
    ae_vector_init_copy(&dst->r, &src->r, _state);
    ae_vector_init_copy(&dst->z, &src->z, _state);
    dst->alpha = src->alpha;
    dst->beta = src->beta;
    dst->r2 = src->r2;
    dst->meritfunction = src->meritfunction;
    ae_vector_init_copy(&dst->x, &src->x, _state);
    ae_vector_init_copy(&dst->mv, &src->mv, _state);
    ae_vector_init_copy(&dst->pv, &src->pv, _state);
    dst->vmv = src->vmv;
    ae_vector_init_copy(&dst->startx, &src->startx, _state);
    dst->epsf = src->epsf;
    dst->maxits = src->maxits;
    dst->itsbeforerestart = src->itsbeforerestart;
    dst->itsbeforerupdate = src->itsbeforerupdate;
    dst->xrep = src->xrep;
    dst->xupdated = src->xupdated;
    dst->needmv = src->needmv;
    dst->needmtv = src->needmtv;
    dst->needmv2 = src->needmv2;
    dst->needvmv = src->needvmv;
    dst->needprec = src->needprec;
    dst->repiterationscount = src->repiterationscount;
    dst->repnmv = src->repnmv;
    dst->repterminationtype = src->repterminationtype;
    dst->running = src->running;
    ae_vector_init_copy(&dst->tmpd, &src->tmpd, _state);
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
}


void _lincgstate_clear(void* _p)
{
    lincgstate *p = (lincgstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->rx);
    ae_vector_clear(&p->b);
    ae_vector_clear(&p->cx);
    ae_vector_clear(&p->cr);
    ae_vector_clear(&p->cz);
    ae_vector_clear(&p->p);
    ae_vector_clear(&p->r);
    ae_vector_clear(&p->z);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->mv);
    ae_vector_clear(&p->pv);
    ae_vector_clear(&p->startx);
    ae_vector_clear(&p->tmpd);
    _rcommstate_clear(&p->rstate);
}


void _lincgstate_destroy(void* _p)
{
    lincgstate *p = (lincgstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->rx);
    ae_vector_destroy(&p->b);
    ae_vector_destroy(&p->cx);
    ae_vector_destroy(&p->cr);
    ae_vector_destroy(&p->cz);
    ae_vector_destroy(&p->p);
    ae_vector_destroy(&p->r);
    ae_vector_destroy(&p->z);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->mv);
    ae_vector_destroy(&p->pv);
    ae_vector_destroy(&p->startx);
    ae_vector_destroy(&p->tmpd);
    _rcommstate_destroy(&p->rstate);
}


void _lincgreport_init(void* _p, ae_state *_state)
{
    lincgreport *p = (lincgreport*)_p;
    ae_touch_ptr((void*)p);
}


void _lincgreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    lincgreport *dst = (lincgreport*)_dst;
    lincgreport *src = (lincgreport*)_src;
    dst->iterationscount = src->iterationscount;
    dst->nmv = src->nmv;
    dst->terminationtype = src->terminationtype;
    dst->r2 = src->r2;
}


void _lincgreport_clear(void* _p)
{
    lincgreport *p = (lincgreport*)_p;
    ae_touch_ptr((void*)p);
}


void _lincgreport_destroy(void* _p)
{
    lincgreport *p = (lincgreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
