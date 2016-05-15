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
#include "linmin.h"


/*$ Declarations $*/
static double linmin_ftol = 0.001;
static double linmin_xtol = 100*ae_machineepsilon;
static ae_int_t linmin_maxfev = 20;
static double linmin_stpmin = 1.0E-50;
static double linmin_defstpmax = 1.0E+50;
static double linmin_armijofactor = 1.3;
static void linmin_mcstep(double* stx,
     double* fx,
     double* dx,
     double* sty,
     double* fy,
     double* dy,
     double* stp,
     double fp,
     double dp,
     ae_bool* brackt,
     double stmin,
     double stmax,
     ae_int_t* info,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Normalizes direction/step pair: makes |D|=1, scales Stp.
If |D|=0, it returns, leavind D/Stp unchanged.

  -- ALGLIB --
     Copyright 01.04.2010 by Bochkanov Sergey
*************************************************************************/
void linminnormalized(/* Real    */ ae_vector* d,
     double* stp,
     ae_int_t n,
     ae_state *_state)
{
    double mx;
    double s;
    ae_int_t i;


    
    /*
     * first, scale D to avoid underflow/overflow durng squaring
     */
    mx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        mx = ae_maxreal(mx, ae_fabs(d->ptr.p_double[i], _state), _state);
    }
    if( ae_fp_eq(mx,(double)(0)) )
    {
        return;
    }
    s = 1/mx;
    ae_v_muld(&d->ptr.p_double[0], 1, ae_v_len(0,n-1), s);
    *stp = *stp/s;
    
    /*
     * normalize D
     */
    s = ae_v_dotproduct(&d->ptr.p_double[0], 1, &d->ptr.p_double[0], 1, ae_v_len(0,n-1));
    s = 1/ae_sqrt(s, _state);
    ae_v_muld(&d->ptr.p_double[0], 1, ae_v_len(0,n-1), s);
    *stp = *stp/s;
}


/*************************************************************************
THE  PURPOSE  OF  MCSRCH  IS  TO  FIND A STEP WHICH SATISFIES A SUFFICIENT
DECREASE CONDITION AND A CURVATURE CONDITION.

AT EACH STAGE THE SUBROUTINE  UPDATES  AN  INTERVAL  OF  UNCERTAINTY  WITH
ENDPOINTS  STX  AND  STY.  THE INTERVAL OF UNCERTAINTY IS INITIALLY CHOSEN
SO THAT IT CONTAINS A MINIMIZER OF THE MODIFIED FUNCTION

    F(X+STP*S) - F(X) - FTOL*STP*(GRADF(X)'S).

IF  A STEP  IS OBTAINED FOR  WHICH THE MODIFIED FUNCTION HAS A NONPOSITIVE
FUNCTION  VALUE  AND  NONNEGATIVE  DERIVATIVE,   THEN   THE   INTERVAL  OF
UNCERTAINTY IS CHOSEN SO THAT IT CONTAINS A MINIMIZER OF F(X+STP*S).

THE  ALGORITHM  IS  DESIGNED TO FIND A STEP WHICH SATISFIES THE SUFFICIENT
DECREASE CONDITION

    F(X+STP*S) .LE. F(X) + FTOL*STP*(GRADF(X)'S),

AND THE CURVATURE CONDITION

    ABS(GRADF(X+STP*S)'S)) .LE. GTOL*ABS(GRADF(X)'S).

IF  FTOL  IS  LESS  THAN GTOL AND IF, FOR EXAMPLE, THE FUNCTION IS BOUNDED
BELOW,  THEN  THERE  IS  ALWAYS  A  STEP  WHICH SATISFIES BOTH CONDITIONS.
IF  NO  STEP  CAN BE FOUND  WHICH  SATISFIES  BOTH  CONDITIONS,  THEN  THE
ALGORITHM  USUALLY STOPS  WHEN  ROUNDING ERRORS  PREVENT FURTHER PROGRESS.
IN THIS CASE STP ONLY SATISFIES THE SUFFICIENT DECREASE CONDITION.


:::::::::::::IMPORTANT NOTES:::::::::::::

NOTE 1:

This routine  guarantees that it will stop at the last point where function
value was calculated. It won't make several additional function evaluations
after finding good point. So if you store function evaluations requested by
this routine, you can be sure that last one is the point where we've stopped.

NOTE 2:

when 0<StpMax<StpMin, algorithm will terminate with INFO=5 and Stp=StpMax

NOTE 3:

this algorithm guarantees that, if MCINFO=1 or MCINFO=5, then:
* F(final_point)<F(initial_point) - strict inequality
* final_point<>initial_point - after rounding to machine precision
:::::::::::::::::::::::::::::::::::::::::


PARAMETERS DESCRIPRION

STAGE IS ZERO ON FIRST CALL, ZERO ON FINAL EXIT

N IS A POSITIVE INTEGER INPUT VARIABLE SET TO THE NUMBER OF VARIABLES.

X IS  AN  ARRAY  OF  LENGTH N. ON INPUT IT MUST CONTAIN THE BASE POINT FOR
THE LINE SEARCH. ON OUTPUT IT CONTAINS X+STP*S.

F IS  A  VARIABLE. ON INPUT IT MUST CONTAIN THE VALUE OF F AT X. ON OUTPUT
IT CONTAINS THE VALUE OF F AT X + STP*S.

G IS AN ARRAY OF LENGTH N. ON INPUT IT MUST CONTAIN THE GRADIENT OF F AT X.
ON OUTPUT IT CONTAINS THE GRADIENT OF F AT X + STP*S.

S IS AN INPUT ARRAY OF LENGTH N WHICH SPECIFIES THE SEARCH DIRECTION.

STP  IS  A NONNEGATIVE VARIABLE. ON INPUT STP CONTAINS AN INITIAL ESTIMATE
OF A SATISFACTORY STEP. ON OUTPUT STP CONTAINS THE FINAL ESTIMATE.

FTOL AND GTOL ARE NONNEGATIVE INPUT VARIABLES. TERMINATION OCCURS WHEN THE
SUFFICIENT DECREASE CONDITION AND THE DIRECTIONAL DERIVATIVE CONDITION ARE
SATISFIED.

XTOL IS A NONNEGATIVE INPUT VARIABLE. TERMINATION OCCURS WHEN THE RELATIVE
WIDTH OF THE INTERVAL OF UNCERTAINTY IS AT MOST XTOL.

STPMIN AND STPMAX ARE NONNEGATIVE INPUT VARIABLES WHICH SPECIFY LOWER  AND
UPPER BOUNDS FOR THE STEP.

MAXFEV IS A POSITIVE INTEGER INPUT VARIABLE. TERMINATION OCCURS WHEN THE
NUMBER OF CALLS TO FCN IS AT LEAST MAXFEV BY THE END OF AN ITERATION.

INFO IS AN INTEGER OUTPUT VARIABLE SET AS FOLLOWS:
    INFO = 0  IMPROPER INPUT PARAMETERS.

    INFO = 1  THE SUFFICIENT DECREASE CONDITION AND THE
              DIRECTIONAL DERIVATIVE CONDITION HOLD.

    INFO = 2  RELATIVE WIDTH OF THE INTERVAL OF UNCERTAINTY
              IS AT MOST XTOL.

    INFO = 3  NUMBER OF CALLS TO FCN HAS REACHED MAXFEV.

    INFO = 4  THE STEP IS AT THE LOWER BOUND STPMIN.

    INFO = 5  THE STEP IS AT THE UPPER BOUND STPMAX.

    INFO = 6  ROUNDING ERRORS PREVENT FURTHER PROGRESS.
              THERE MAY NOT BE A STEP WHICH SATISFIES THE
              SUFFICIENT DECREASE AND CURVATURE CONDITIONS.
              TOLERANCES MAY BE TOO SMALL.

NFEV IS AN INTEGER OUTPUT VARIABLE SET TO THE NUMBER OF CALLS TO FCN.

WA IS A WORK ARRAY OF LENGTH N.

ARGONNE NATIONAL LABORATORY. MINPACK PROJECT. JUNE 1983
JORGE J. MORE', DAVID J. THUENTE
*************************************************************************/
void mcsrch(ae_int_t n,
     /* Real    */ ae_vector* x,
     double* f,
     /* Real    */ ae_vector* g,
     /* Real    */ ae_vector* s,
     double* stp,
     double stpmax,
     double gtol,
     ae_int_t* info,
     ae_int_t* nfev,
     /* Real    */ ae_vector* wa,
     linminstate* state,
     ae_int_t* stage,
     ae_state *_state)
{
    ae_int_t i;
    double v;
    double p5;
    double p66;
    double zero;


    
    /*
     * init
     */
    p5 = 0.5;
    p66 = 0.66;
    state->xtrapf = 4.0;
    zero = (double)(0);
    if( ae_fp_eq(stpmax,(double)(0)) )
    {
        stpmax = linmin_defstpmax;
    }
    if( ae_fp_less(*stp,linmin_stpmin) )
    {
        *stp = linmin_stpmin;
    }
    if( ae_fp_greater(*stp,stpmax) )
    {
        *stp = stpmax;
    }
    
    /*
     * Main cycle
     */
    for(;;)
    {
        if( *stage==0 )
        {
            
            /*
             * NEXT
             */
            *stage = 2;
            continue;
        }
        if( *stage==2 )
        {
            state->infoc = 1;
            *info = 0;
            
            /*
             *     CHECK THE INPUT PARAMETERS FOR ERRORS.
             */
            if( ae_fp_less(stpmax,linmin_stpmin)&&ae_fp_greater(stpmax,(double)(0)) )
            {
                *info = 5;
                *stp = stpmax;
                *stage = 0;
                return;
            }
            if( ((((((n<=0||ae_fp_less_eq(*stp,(double)(0)))||ae_fp_less(linmin_ftol,(double)(0)))||ae_fp_less(gtol,zero))||ae_fp_less(linmin_xtol,zero))||ae_fp_less(linmin_stpmin,zero))||ae_fp_less(stpmax,linmin_stpmin))||linmin_maxfev<=0 )
            {
                *stage = 0;
                return;
            }
            
            /*
             *     COMPUTE THE INITIAL GRADIENT IN THE SEARCH DIRECTION
             *     AND CHECK THAT S IS A DESCENT DIRECTION.
             */
            v = ae_v_dotproduct(&g->ptr.p_double[0], 1, &s->ptr.p_double[0], 1, ae_v_len(0,n-1));
            state->dginit = v;
            if( ae_fp_greater_eq(state->dginit,(double)(0)) )
            {
                *stage = 0;
                return;
            }
            
            /*
             *     INITIALIZE LOCAL VARIABLES.
             */
            state->brackt = ae_false;
            state->stage1 = ae_true;
            *nfev = 0;
            state->finit = *f;
            state->dgtest = linmin_ftol*state->dginit;
            state->width = stpmax-linmin_stpmin;
            state->width1 = state->width/p5;
            ae_v_move(&wa->ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
            
            /*
             *     THE VARIABLES STX, FX, DGX CONTAIN THE VALUES OF THE STEP,
             *     FUNCTION, AND DIRECTIONAL DERIVATIVE AT THE BEST STEP.
             *     THE VARIABLES STY, FY, DGY CONTAIN THE VALUE OF THE STEP,
             *     FUNCTION, AND DERIVATIVE AT THE OTHER ENDPOINT OF
             *     THE INTERVAL OF UNCERTAINTY.
             *     THE VARIABLES STP, F, DG CONTAIN THE VALUES OF THE STEP,
             *     FUNCTION, AND DERIVATIVE AT THE CURRENT STEP.
             */
            state->stx = (double)(0);
            state->fx = state->finit;
            state->dgx = state->dginit;
            state->sty = (double)(0);
            state->fy = state->finit;
            state->dgy = state->dginit;
            
            /*
             * NEXT
             */
            *stage = 3;
            continue;
        }
        if( *stage==3 )
        {
            
            /*
             *     START OF ITERATION.
             *
             *     SET THE MINIMUM AND MAXIMUM STEPS TO CORRESPOND
             *     TO THE PRESENT INTERVAL OF UNCERTAINTY.
             */
            if( state->brackt )
            {
                if( ae_fp_less(state->stx,state->sty) )
                {
                    state->stmin = state->stx;
                    state->stmax = state->sty;
                }
                else
                {
                    state->stmin = state->sty;
                    state->stmax = state->stx;
                }
            }
            else
            {
                state->stmin = state->stx;
                state->stmax = *stp+state->xtrapf*(*stp-state->stx);
            }
            
            /*
             *        FORCE THE STEP TO BE WITHIN THE BOUNDS STPMAX AND STPMIN.
             */
            if( ae_fp_greater(*stp,stpmax) )
            {
                *stp = stpmax;
            }
            if( ae_fp_less(*stp,linmin_stpmin) )
            {
                *stp = linmin_stpmin;
            }
            
            /*
             *        IF AN UNUSUAL TERMINATION IS TO OCCUR THEN LET
             *        STP BE THE LOWEST POINT OBTAINED SO FAR.
             */
            if( (((state->brackt&&(ae_fp_less_eq(*stp,state->stmin)||ae_fp_greater_eq(*stp,state->stmax)))||*nfev>=linmin_maxfev-1)||state->infoc==0)||(state->brackt&&ae_fp_less_eq(state->stmax-state->stmin,linmin_xtol*state->stmax)) )
            {
                *stp = state->stx;
            }
            
            /*
             *        EVALUATE THE FUNCTION AND GRADIENT AT STP
             *        AND COMPUTE THE DIRECTIONAL DERIVATIVE.
             */
            ae_v_move(&x->ptr.p_double[0], 1, &wa->ptr.p_double[0], 1, ae_v_len(0,n-1));
            ae_v_addd(&x->ptr.p_double[0], 1, &s->ptr.p_double[0], 1, ae_v_len(0,n-1), *stp);
            
            /*
             * NEXT
             */
            *stage = 4;
            return;
        }
        if( *stage==4 )
        {
            *info = 0;
            *nfev = *nfev+1;
            v = ae_v_dotproduct(&g->ptr.p_double[0], 1, &s->ptr.p_double[0], 1, ae_v_len(0,n-1));
            state->dg = v;
            state->ftest1 = state->finit+*stp*state->dgtest;
            
            /*
             *        TEST FOR CONVERGENCE.
             */
            if( (state->brackt&&(ae_fp_less_eq(*stp,state->stmin)||ae_fp_greater_eq(*stp,state->stmax)))||state->infoc==0 )
            {
                *info = 6;
            }
            if( ((ae_fp_eq(*stp,stpmax)&&ae_fp_less(*f,state->finit))&&ae_fp_less_eq(*f,state->ftest1))&&ae_fp_less_eq(state->dg,state->dgtest) )
            {
                *info = 5;
            }
            if( ae_fp_eq(*stp,linmin_stpmin)&&((ae_fp_greater_eq(*f,state->finit)||ae_fp_greater(*f,state->ftest1))||ae_fp_greater_eq(state->dg,state->dgtest)) )
            {
                *info = 4;
            }
            if( *nfev>=linmin_maxfev )
            {
                *info = 3;
            }
            if( state->brackt&&ae_fp_less_eq(state->stmax-state->stmin,linmin_xtol*state->stmax) )
            {
                *info = 2;
            }
            if( (ae_fp_less(*f,state->finit)&&ae_fp_less_eq(*f,state->ftest1))&&ae_fp_less_eq(ae_fabs(state->dg, _state),-gtol*state->dginit) )
            {
                *info = 1;
            }
            
            /*
             *        CHECK FOR TERMINATION.
             */
            if( *info!=0 )
            {
                
                /*
                 * Check guarantees provided by the function for INFO=1 or INFO=5
                 */
                if( *info==1||*info==5 )
                {
                    v = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        v = v+(wa->ptr.p_double[i]-x->ptr.p_double[i])*(wa->ptr.p_double[i]-x->ptr.p_double[i]);
                    }
                    if( ae_fp_greater_eq(*f,state->finit)||ae_fp_eq(v,0.0) )
                    {
                        *info = 6;
                    }
                }
                *stage = 0;
                return;
            }
            
            /*
             *        IN THE FIRST STAGE WE SEEK A STEP FOR WHICH THE MODIFIED
             *        FUNCTION HAS A NONPOSITIVE VALUE AND NONNEGATIVE DERIVATIVE.
             */
            if( (state->stage1&&ae_fp_less_eq(*f,state->ftest1))&&ae_fp_greater_eq(state->dg,ae_minreal(linmin_ftol, gtol, _state)*state->dginit) )
            {
                state->stage1 = ae_false;
            }
            
            /*
             *        A MODIFIED FUNCTION IS USED TO PREDICT THE STEP ONLY IF
             *        WE HAVE NOT OBTAINED A STEP FOR WHICH THE MODIFIED
             *        FUNCTION HAS A NONPOSITIVE FUNCTION VALUE AND NONNEGATIVE
             *        DERIVATIVE, AND IF A LOWER FUNCTION VALUE HAS BEEN
             *        OBTAINED BUT THE DECREASE IS NOT SUFFICIENT.
             */
            if( (state->stage1&&ae_fp_less_eq(*f,state->fx))&&ae_fp_greater(*f,state->ftest1) )
            {
                
                /*
                 *           DEFINE THE MODIFIED FUNCTION AND DERIVATIVE VALUES.
                 */
                state->fm = *f-*stp*state->dgtest;
                state->fxm = state->fx-state->stx*state->dgtest;
                state->fym = state->fy-state->sty*state->dgtest;
                state->dgm = state->dg-state->dgtest;
                state->dgxm = state->dgx-state->dgtest;
                state->dgym = state->dgy-state->dgtest;
                
                /*
                 *           CALL CSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                 *           AND TO COMPUTE THE NEW STEP.
                 */
                linmin_mcstep(&state->stx, &state->fxm, &state->dgxm, &state->sty, &state->fym, &state->dgym, stp, state->fm, state->dgm, &state->brackt, state->stmin, state->stmax, &state->infoc, _state);
                
                /*
                 *           RESET THE FUNCTION AND GRADIENT VALUES FOR F.
                 */
                state->fx = state->fxm+state->stx*state->dgtest;
                state->fy = state->fym+state->sty*state->dgtest;
                state->dgx = state->dgxm+state->dgtest;
                state->dgy = state->dgym+state->dgtest;
            }
            else
            {
                
                /*
                 *           CALL MCSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                 *           AND TO COMPUTE THE NEW STEP.
                 */
                linmin_mcstep(&state->stx, &state->fx, &state->dgx, &state->sty, &state->fy, &state->dgy, stp, *f, state->dg, &state->brackt, state->stmin, state->stmax, &state->infoc, _state);
            }
            
            /*
             *        FORCE A SUFFICIENT DECREASE IN THE SIZE OF THE
             *        INTERVAL OF UNCERTAINTY.
             */
            if( state->brackt )
            {
                if( ae_fp_greater_eq(ae_fabs(state->sty-state->stx, _state),p66*state->width1) )
                {
                    *stp = state->stx+p5*(state->sty-state->stx);
                }
                state->width1 = state->width;
                state->width = ae_fabs(state->sty-state->stx, _state);
            }
            
            /*
             *  NEXT.
             */
            *stage = 3;
            continue;
        }
    }
}


/*************************************************************************
These functions perform Armijo line search using  at  most  FMAX  function
evaluations.  It  doesn't  enforce  some  kind  of  " sufficient decrease"
criterion - it just tries different Armijo steps and returns optimum found
so far.

Optimization is done using F-rcomm interface:
* ArmijoCreate initializes State structure
  (reusing previously allocated buffers)
* ArmijoIteration is subsequently called
* ArmijoResults returns results

INPUT PARAMETERS:
    N       -   problem size
    X       -   array[N], starting point
    F       -   F(X+S*STP)
    S       -   step direction, S>0
    STP     -   step length
    STPMAX  -   maximum value for STP or zero (if no limit is imposed)
    FMAX    -   maximum number of function evaluations
    State   -   optimization state

  -- ALGLIB --
     Copyright 05.10.2010 by Bochkanov Sergey
*************************************************************************/
void armijocreate(ae_int_t n,
     /* Real    */ ae_vector* x,
     double f,
     /* Real    */ ae_vector* s,
     double stp,
     double stpmax,
     ae_int_t fmax,
     armijostate* state,
     ae_state *_state)
{


    if( state->x.cnt<n )
    {
        ae_vector_set_length(&state->x, n, _state);
    }
    if( state->xbase.cnt<n )
    {
        ae_vector_set_length(&state->xbase, n, _state);
    }
    if( state->s.cnt<n )
    {
        ae_vector_set_length(&state->s, n, _state);
    }
    state->stpmax = stpmax;
    state->fmax = fmax;
    state->stplen = stp;
    state->fcur = f;
    state->n = n;
    ae_v_move(&state->xbase.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&state->s.ptr.p_double[0], 1, &s->ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_vector_set_length(&state->rstate.ia, 0+1, _state);
    ae_vector_set_length(&state->rstate.ra, 0+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
This is rcomm-based search function

  -- ALGLIB --
     Copyright 05.10.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool armijoiteration(armijostate* state, ae_state *_state)
{
    double v;
    ae_int_t n;
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
        n = state->rstate.ia.ptr.p_int[0];
        v = state->rstate.ra.ptr.p_double[0];
    }
    else
    {
        n = -983;
        v = -989;
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
    
    /*
     * Routine body
     */
    if( (ae_fp_less_eq(state->stplen,(double)(0))||ae_fp_less(state->stpmax,(double)(0)))||state->fmax<2 )
    {
        state->info = 0;
        result = ae_false;
        return result;
    }
    if( ae_fp_less_eq(state->stplen,linmin_stpmin) )
    {
        state->info = 4;
        result = ae_false;
        return result;
    }
    n = state->n;
    state->nfev = 0;
    
    /*
     * We always need F
     */
    state->needf = ae_true;
    
    /*
     * Bound StpLen
     */
    if( ae_fp_greater(state->stplen,state->stpmax)&&ae_fp_neq(state->stpmax,(double)(0)) )
    {
        state->stplen = state->stpmax;
    }
    
    /*
     * Increase length
     */
    v = state->stplen*linmin_armijofactor;
    if( ae_fp_greater(v,state->stpmax)&&ae_fp_neq(state->stpmax,(double)(0)) )
    {
        v = state->stpmax;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_addd(&state->x.ptr.p_double[0], 1, &state->s.ptr.p_double[0], 1, ae_v_len(0,n-1), v);
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    state->nfev = state->nfev+1;
    if( ae_fp_greater_eq(state->f,state->fcur) )
    {
        goto lbl_4;
    }
    state->stplen = v;
    state->fcur = state->f;
lbl_6:
    if( ae_false )
    {
        goto lbl_7;
    }
    
    /*
     * test stopping conditions
     */
    if( state->nfev>=state->fmax )
    {
        state->info = 3;
        result = ae_false;
        return result;
    }
    if( ae_fp_greater_eq(state->stplen,state->stpmax) )
    {
        state->info = 5;
        result = ae_false;
        return result;
    }
    
    /*
     * evaluate F
     */
    v = state->stplen*linmin_armijofactor;
    if( ae_fp_greater(v,state->stpmax)&&ae_fp_neq(state->stpmax,(double)(0)) )
    {
        v = state->stpmax;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_addd(&state->x.ptr.p_double[0], 1, &state->s.ptr.p_double[0], 1, ae_v_len(0,n-1), v);
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    state->nfev = state->nfev+1;
    
    /*
     * make decision
     */
    if( ae_fp_less(state->f,state->fcur) )
    {
        state->stplen = v;
        state->fcur = state->f;
    }
    else
    {
        state->info = 1;
        result = ae_false;
        return result;
    }
    goto lbl_6;
lbl_7:
lbl_4:
    
    /*
     * Decrease length
     */
    v = state->stplen/linmin_armijofactor;
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_addd(&state->x.ptr.p_double[0], 1, &state->s.ptr.p_double[0], 1, ae_v_len(0,n-1), v);
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    state->nfev = state->nfev+1;
    if( ae_fp_greater_eq(state->f,state->fcur) )
    {
        goto lbl_8;
    }
    state->stplen = state->stplen/linmin_armijofactor;
    state->fcur = state->f;
lbl_10:
    if( ae_false )
    {
        goto lbl_11;
    }
    
    /*
     * test stopping conditions
     */
    if( state->nfev>=state->fmax )
    {
        state->info = 3;
        result = ae_false;
        return result;
    }
    if( ae_fp_less_eq(state->stplen,linmin_stpmin) )
    {
        state->info = 4;
        result = ae_false;
        return result;
    }
    
    /*
     * evaluate F
     */
    v = state->stplen/linmin_armijofactor;
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_addd(&state->x.ptr.p_double[0], 1, &state->s.ptr.p_double[0], 1, ae_v_len(0,n-1), v);
    state->rstate.stage = 3;
    goto lbl_rcomm;
lbl_3:
    state->nfev = state->nfev+1;
    
    /*
     * make decision
     */
    if( ae_fp_less(state->f,state->fcur) )
    {
        state->stplen = state->stplen/linmin_armijofactor;
        state->fcur = state->f;
    }
    else
    {
        state->info = 1;
        result = ae_false;
        return result;
    }
    goto lbl_10;
lbl_11:
lbl_8:
    
    /*
     * Nothing to be done
     */
    state->info = 1;
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstate.ia.ptr.p_int[0] = n;
    state->rstate.ra.ptr.p_double[0] = v;
    return result;
}


/*************************************************************************
Results of Armijo search

OUTPUT PARAMETERS:
    INFO    -   on output it is set to one of the return codes:
                * 0     improper input params
                * 1     optimum step is found with at most FMAX evaluations
                * 3     FMAX evaluations were used,
                        X contains optimum found so far
                * 4     step is at lower bound STPMIN
                * 5     step is at upper bound
    STP     -   step length (in case of failure it is still returned)
    F       -   function value (in case of failure it is still returned)

  -- ALGLIB --
     Copyright 05.10.2010 by Bochkanov Sergey
*************************************************************************/
void armijoresults(armijostate* state,
     ae_int_t* info,
     double* stp,
     double* f,
     ae_state *_state)
{


    *info = state->info;
    *stp = state->stplen;
    *f = state->fcur;
}


static void linmin_mcstep(double* stx,
     double* fx,
     double* dx,
     double* sty,
     double* fy,
     double* dy,
     double* stp,
     double fp,
     double dp,
     ae_bool* brackt,
     double stmin,
     double stmax,
     ae_int_t* info,
     ae_state *_state)
{
    ae_bool bound;
    double gamma;
    double p;
    double q;
    double r;
    double s;
    double sgnd;
    double stpc;
    double stpf;
    double stpq;
    double theta;


    *info = 0;
    
    /*
     *     CHECK THE INPUT PARAMETERS FOR ERRORS.
     */
    if( ((*brackt&&(ae_fp_less_eq(*stp,ae_minreal(*stx, *sty, _state))||ae_fp_greater_eq(*stp,ae_maxreal(*stx, *sty, _state))))||ae_fp_greater_eq(*dx*(*stp-(*stx)),(double)(0)))||ae_fp_less(stmax,stmin) )
    {
        return;
    }
    
    /*
     *     DETERMINE IF THE DERIVATIVES HAVE OPPOSITE SIGN.
     */
    sgnd = dp*(*dx/ae_fabs(*dx, _state));
    
    /*
     *     FIRST CASE. A HIGHER FUNCTION VALUE.
     *     THE MINIMUM IS BRACKETED. IF THE CUBIC STEP IS CLOSER
     *     TO STX THAN THE QUADRATIC STEP, THE CUBIC STEP IS TAKEN,
     *     ELSE THE AVERAGE OF THE CUBIC AND QUADRATIC STEPS IS TAKEN.
     */
    if( ae_fp_greater(fp,*fx) )
    {
        *info = 1;
        bound = ae_true;
        theta = 3*(*fx-fp)/(*stp-(*stx))+(*dx)+dp;
        s = ae_maxreal(ae_fabs(theta, _state), ae_maxreal(ae_fabs(*dx, _state), ae_fabs(dp, _state), _state), _state);
        gamma = s*ae_sqrt(ae_sqr(theta/s, _state)-*dx/s*(dp/s), _state);
        if( ae_fp_less(*stp,*stx) )
        {
            gamma = -gamma;
        }
        p = gamma-(*dx)+theta;
        q = gamma-(*dx)+gamma+dp;
        r = p/q;
        stpc = *stx+r*(*stp-(*stx));
        stpq = *stx+*dx/((*fx-fp)/(*stp-(*stx))+(*dx))/2*(*stp-(*stx));
        if( ae_fp_less(ae_fabs(stpc-(*stx), _state),ae_fabs(stpq-(*stx), _state)) )
        {
            stpf = stpc;
        }
        else
        {
            stpf = stpc+(stpq-stpc)/2;
        }
        *brackt = ae_true;
    }
    else
    {
        if( ae_fp_less(sgnd,(double)(0)) )
        {
            
            /*
             *     SECOND CASE. A LOWER FUNCTION VALUE AND DERIVATIVES OF
             *     OPPOSITE SIGN. THE MINIMUM IS BRACKETED. IF THE CUBIC
             *     STEP IS CLOSER TO STX THAN THE QUADRATIC (SECANT) STEP,
             *     THE CUBIC STEP IS TAKEN, ELSE THE QUADRATIC STEP IS TAKEN.
             */
            *info = 2;
            bound = ae_false;
            theta = 3*(*fx-fp)/(*stp-(*stx))+(*dx)+dp;
            s = ae_maxreal(ae_fabs(theta, _state), ae_maxreal(ae_fabs(*dx, _state), ae_fabs(dp, _state), _state), _state);
            gamma = s*ae_sqrt(ae_sqr(theta/s, _state)-*dx/s*(dp/s), _state);
            if( ae_fp_greater(*stp,*stx) )
            {
                gamma = -gamma;
            }
            p = gamma-dp+theta;
            q = gamma-dp+gamma+(*dx);
            r = p/q;
            stpc = *stp+r*(*stx-(*stp));
            stpq = *stp+dp/(dp-(*dx))*(*stx-(*stp));
            if( ae_fp_greater(ae_fabs(stpc-(*stp), _state),ae_fabs(stpq-(*stp), _state)) )
            {
                stpf = stpc;
            }
            else
            {
                stpf = stpq;
            }
            *brackt = ae_true;
        }
        else
        {
            if( ae_fp_less(ae_fabs(dp, _state),ae_fabs(*dx, _state)) )
            {
                
                /*
                 *     THIRD CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                 *     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DECREASES.
                 *     THE CUBIC STEP IS ONLY USED IF THE CUBIC TENDS TO INFINITY
                 *     IN THE DIRECTION OF THE STEP OR IF THE MINIMUM OF THE CUBIC
                 *     IS BEYOND STP. OTHERWISE THE CUBIC STEP IS DEFINED TO BE
                 *     EITHER STPMIN OR STPMAX. THE QUADRATIC (SECANT) STEP IS ALSO
                 *     COMPUTED AND IF THE MINIMUM IS BRACKETED THEN THE THE STEP
                 *     CLOSEST TO STX IS TAKEN, ELSE THE STEP FARTHEST AWAY IS TAKEN.
                 */
                *info = 3;
                bound = ae_true;
                theta = 3*(*fx-fp)/(*stp-(*stx))+(*dx)+dp;
                s = ae_maxreal(ae_fabs(theta, _state), ae_maxreal(ae_fabs(*dx, _state), ae_fabs(dp, _state), _state), _state);
                
                /*
                 *        THE CASE GAMMA = 0 ONLY ARISES IF THE CUBIC DOES NOT TEND
                 *        TO INFINITY IN THE DIRECTION OF THE STEP.
                 */
                gamma = s*ae_sqrt(ae_maxreal((double)(0), ae_sqr(theta/s, _state)-*dx/s*(dp/s), _state), _state);
                if( ae_fp_greater(*stp,*stx) )
                {
                    gamma = -gamma;
                }
                p = gamma-dp+theta;
                q = gamma+(*dx-dp)+gamma;
                r = p/q;
                if( ae_fp_less(r,(double)(0))&&ae_fp_neq(gamma,(double)(0)) )
                {
                    stpc = *stp+r*(*stx-(*stp));
                }
                else
                {
                    if( ae_fp_greater(*stp,*stx) )
                    {
                        stpc = stmax;
                    }
                    else
                    {
                        stpc = stmin;
                    }
                }
                stpq = *stp+dp/(dp-(*dx))*(*stx-(*stp));
                if( *brackt )
                {
                    if( ae_fp_less(ae_fabs(*stp-stpc, _state),ae_fabs(*stp-stpq, _state)) )
                    {
                        stpf = stpc;
                    }
                    else
                    {
                        stpf = stpq;
                    }
                }
                else
                {
                    if( ae_fp_greater(ae_fabs(*stp-stpc, _state),ae_fabs(*stp-stpq, _state)) )
                    {
                        stpf = stpc;
                    }
                    else
                    {
                        stpf = stpq;
                    }
                }
            }
            else
            {
                
                /*
                 *     FOURTH CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                 *     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DOES
                 *     NOT DECREASE. IF THE MINIMUM IS NOT BRACKETED, THE STEP
                 *     IS EITHER STPMIN OR STPMAX, ELSE THE CUBIC STEP IS TAKEN.
                 */
                *info = 4;
                bound = ae_false;
                if( *brackt )
                {
                    theta = 3*(fp-(*fy))/(*sty-(*stp))+(*dy)+dp;
                    s = ae_maxreal(ae_fabs(theta, _state), ae_maxreal(ae_fabs(*dy, _state), ae_fabs(dp, _state), _state), _state);
                    gamma = s*ae_sqrt(ae_sqr(theta/s, _state)-*dy/s*(dp/s), _state);
                    if( ae_fp_greater(*stp,*sty) )
                    {
                        gamma = -gamma;
                    }
                    p = gamma-dp+theta;
                    q = gamma-dp+gamma+(*dy);
                    r = p/q;
                    stpc = *stp+r*(*sty-(*stp));
                    stpf = stpc;
                }
                else
                {
                    if( ae_fp_greater(*stp,*stx) )
                    {
                        stpf = stmax;
                    }
                    else
                    {
                        stpf = stmin;
                    }
                }
            }
        }
    }
    
    /*
     *     UPDATE THE INTERVAL OF UNCERTAINTY. THIS UPDATE DOES NOT
     *     DEPEND ON THE NEW STEP OR THE CASE ANALYSIS ABOVE.
     */
    if( ae_fp_greater(fp,*fx) )
    {
        *sty = *stp;
        *fy = fp;
        *dy = dp;
    }
    else
    {
        if( ae_fp_less(sgnd,0.0) )
        {
            *sty = *stx;
            *fy = *fx;
            *dy = *dx;
        }
        *stx = *stp;
        *fx = fp;
        *dx = dp;
    }
    
    /*
     *     COMPUTE THE NEW STEP AND SAFEGUARD IT.
     */
    stpf = ae_minreal(stmax, stpf, _state);
    stpf = ae_maxreal(stmin, stpf, _state);
    *stp = stpf;
    if( *brackt&&bound )
    {
        if( ae_fp_greater(*sty,*stx) )
        {
            *stp = ae_minreal(*stx+0.66*(*sty-(*stx)), *stp, _state);
        }
        else
        {
            *stp = ae_maxreal(*stx+0.66*(*sty-(*stx)), *stp, _state);
        }
    }
}


void _linminstate_init(void* _p, ae_state *_state)
{
    linminstate *p = (linminstate*)_p;
    ae_touch_ptr((void*)p);
}


void _linminstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    linminstate *dst = (linminstate*)_dst;
    linminstate *src = (linminstate*)_src;
    dst->brackt = src->brackt;
    dst->stage1 = src->stage1;
    dst->infoc = src->infoc;
    dst->dg = src->dg;
    dst->dgm = src->dgm;
    dst->dginit = src->dginit;
    dst->dgtest = src->dgtest;
    dst->dgx = src->dgx;
    dst->dgxm = src->dgxm;
    dst->dgy = src->dgy;
    dst->dgym = src->dgym;
    dst->finit = src->finit;
    dst->ftest1 = src->ftest1;
    dst->fm = src->fm;
    dst->fx = src->fx;
    dst->fxm = src->fxm;
    dst->fy = src->fy;
    dst->fym = src->fym;
    dst->stx = src->stx;
    dst->sty = src->sty;
    dst->stmin = src->stmin;
    dst->stmax = src->stmax;
    dst->width = src->width;
    dst->width1 = src->width1;
    dst->xtrapf = src->xtrapf;
}


void _linminstate_clear(void* _p)
{
    linminstate *p = (linminstate*)_p;
    ae_touch_ptr((void*)p);
}


void _linminstate_destroy(void* _p)
{
    linminstate *p = (linminstate*)_p;
    ae_touch_ptr((void*)p);
}


void _armijostate_init(void* _p, ae_state *_state)
{
    armijostate *p = (armijostate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->xbase, 0, DT_REAL, _state);
    ae_vector_init(&p->s, 0, DT_REAL, _state);
    _rcommstate_init(&p->rstate, _state);
}


void _armijostate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    armijostate *dst = (armijostate*)_dst;
    armijostate *src = (armijostate*)_src;
    dst->needf = src->needf;
    ae_vector_init_copy(&dst->x, &src->x, _state);
    dst->f = src->f;
    dst->n = src->n;
    ae_vector_init_copy(&dst->xbase, &src->xbase, _state);
    ae_vector_init_copy(&dst->s, &src->s, _state);
    dst->stplen = src->stplen;
    dst->fcur = src->fcur;
    dst->stpmax = src->stpmax;
    dst->fmax = src->fmax;
    dst->nfev = src->nfev;
    dst->info = src->info;
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
}


void _armijostate_clear(void* _p)
{
    armijostate *p = (armijostate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->xbase);
    ae_vector_clear(&p->s);
    _rcommstate_clear(&p->rstate);
}


void _armijostate_destroy(void* _p)
{
    armijostate *p = (armijostate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->xbase);
    ae_vector_destroy(&p->s);
    _rcommstate_destroy(&p->rstate);
}


/*$ End $*/
