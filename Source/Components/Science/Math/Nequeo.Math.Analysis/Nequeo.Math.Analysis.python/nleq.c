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
#include "nleq.h"


/*$ Declarations $*/
static void nleq_clearrequestfields(nleqstate* state, ae_state *_state);
static ae_bool nleq_increaselambda(double* lambdav,
     double* nu,
     double lambdaup,
     ae_state *_state);
static void nleq_decreaselambda(double* lambdav,
     double* nu,
     double lambdadown,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
                LEVENBERG-MARQUARDT-LIKE NONLINEAR SOLVER

DESCRIPTION:
This algorithm solves system of nonlinear equations
    F[0](x[0], ..., x[n-1])   = 0
    F[1](x[0], ..., x[n-1])   = 0
    ...
    F[M-1](x[0], ..., x[n-1]) = 0
with M/N do not necessarily coincide.  Algorithm  converges  quadratically
under following conditions:
    * the solution set XS is nonempty
    * for some xs in XS there exist such neighbourhood N(xs) that:
      * vector function F(x) and its Jacobian J(x) are continuously
        differentiable on N
      * ||F(x)|| provides local error bound on N, i.e. there  exists  such
        c1, that ||F(x)||>c1*distance(x,XS)
Note that these conditions are much more weaker than usual non-singularity
conditions. For example, algorithm will converge for any  affine  function
F (whether its Jacobian singular or not).


REQUIREMENTS:
Algorithm will request following information during its operation:
* function vector F[] and Jacobian matrix at given point X
* value of merit function f(x)=F[0]^2(x)+...+F[M-1]^2(x) at given point X


USAGE:
1. User initializes algorithm state with NLEQCreateLM() call
2. User tunes solver parameters with  NLEQSetCond(),  NLEQSetStpMax()  and
   other functions
3. User  calls  NLEQSolve()  function  which  takes  algorithm  state  and
   pointers (delegates, etc.) to callback functions which calculate  merit
   function value and Jacobian.
4. User calls NLEQResults() to get solution
5. Optionally, user may call NLEQRestartFrom() to  solve  another  problem
   with same parameters (N/M) but another starting  point  and/or  another
   function vector. NLEQRestartFrom() allows to reuse already  initialized
   structure.


INPUT PARAMETERS:
    N       -   space dimension, N>1:
                * if provided, only leading N elements of X are used
                * if not provided, determined automatically from size of X
    M       -   system size
    X       -   starting point


OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state


NOTES:
1. you may tune stopping conditions with NLEQSetCond() function
2. if target function contains exp() or other fast growing functions,  and
   optimization algorithm makes too large steps which leads  to  overflow,
   use NLEQSetStpMax() function to bound algorithm's steps.
3. this  algorithm  is  a  slightly  modified implementation of the method
   described  in  'Levenberg-Marquardt  method  for constrained  nonlinear
   equations with strong local convergence properties' by Christian Kanzow
   Nobuo Yamashita and Masao Fukushima and further  developed  in  'On the
   convergence of a New Levenberg-Marquardt Method'  by  Jin-yan  Fan  and
   Ya-Xiang Yuan.


  -- ALGLIB --
     Copyright 20.08.2009 by Bochkanov Sergey
*************************************************************************/
void nleqcreatelm(ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_vector* x,
     nleqstate* state,
     ae_state *_state)
{

    _nleqstate_clear(state);

    ae_assert(n>=1, "NLEQCreateLM: N<1!", _state);
    ae_assert(m>=1, "NLEQCreateLM: M<1!", _state);
    ae_assert(x->cnt>=n, "NLEQCreateLM: Length(X)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "NLEQCreateLM: X contains infinite or NaN values!", _state);
    
    /*
     * Initialize
     */
    state->n = n;
    state->m = m;
    nleqsetcond(state, (double)(0), 0, _state);
    nleqsetxrep(state, ae_false, _state);
    nleqsetstpmax(state, (double)(0), _state);
    ae_vector_set_length(&state->x, n, _state);
    ae_vector_set_length(&state->xbase, n, _state);
    ae_matrix_set_length(&state->j, m, n, _state);
    ae_vector_set_length(&state->fi, m, _state);
    ae_vector_set_length(&state->rightpart, n, _state);
    ae_vector_set_length(&state->candstep, n, _state);
    nleqrestartfrom(state, x, _state);
}


/*************************************************************************
This function sets stopping conditions for the nonlinear solver

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    EpsF    -   >=0
                The subroutine finishes  its work if on k+1-th iteration
                the condition ||F||<=EpsF is satisfied
    MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                iterations is unlimited.

Passing EpsF=0 and MaxIts=0 simultaneously will lead to  automatic
stopping criterion selection (small EpsF).

NOTES:

  -- ALGLIB --
     Copyright 20.08.2010 by Bochkanov Sergey
*************************************************************************/
void nleqsetcond(nleqstate* state,
     double epsf,
     ae_int_t maxits,
     ae_state *_state)
{


    ae_assert(ae_isfinite(epsf, _state), "NLEQSetCond: EpsF is not finite number!", _state);
    ae_assert(ae_fp_greater_eq(epsf,(double)(0)), "NLEQSetCond: negative EpsF!", _state);
    ae_assert(maxits>=0, "NLEQSetCond: negative MaxIts!", _state);
    if( ae_fp_eq(epsf,(double)(0))&&maxits==0 )
    {
        epsf = 1.0E-6;
    }
    state->epsf = epsf;
    state->maxits = maxits;
}


/*************************************************************************
This function turns on/off reporting.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    NeedXRep-   whether iteration reports are needed or not

If NeedXRep is True, algorithm will call rep() callback function if  it is
provided to NLEQSolve().

  -- ALGLIB --
     Copyright 20.08.2010 by Bochkanov Sergey
*************************************************************************/
void nleqsetxrep(nleqstate* state, ae_bool needxrep, ae_state *_state)
{


    state->xrep = needxrep;
}


/*************************************************************************
This function sets maximum step length

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                want to limit step length.

Use this subroutine when target function  contains  exp()  or  other  fast
growing functions, and algorithm makes  too  large  steps  which  lead  to
overflow. This function allows us to reject steps that are too large  (and
therefore expose us to the possible overflow) without actually calculating
function value at the x+stp*d.

  -- ALGLIB --
     Copyright 20.08.2010 by Bochkanov Sergey
*************************************************************************/
void nleqsetstpmax(nleqstate* state, double stpmax, ae_state *_state)
{


    ae_assert(ae_isfinite(stpmax, _state), "NLEQSetStpMax: StpMax is not finite!", _state);
    ae_assert(ae_fp_greater_eq(stpmax,(double)(0)), "NLEQSetStpMax: StpMax<0!", _state);
    state->stpmax = stpmax;
}


/*************************************************************************

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool nleqiteration(nleqstate* state, ae_state *_state)
{
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    double lambdaup;
    double lambdadown;
    double lambdav;
    double rho;
    double mu;
    double stepnorm;
    ae_bool b;
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
        m = state->rstate.ia.ptr.p_int[1];
        i = state->rstate.ia.ptr.p_int[2];
        b = state->rstate.ba.ptr.p_bool[0];
        lambdaup = state->rstate.ra.ptr.p_double[0];
        lambdadown = state->rstate.ra.ptr.p_double[1];
        lambdav = state->rstate.ra.ptr.p_double[2];
        rho = state->rstate.ra.ptr.p_double[3];
        mu = state->rstate.ra.ptr.p_double[4];
        stepnorm = state->rstate.ra.ptr.p_double[5];
    }
    else
    {
        n = -983;
        m = -989;
        i = -834;
        b = ae_false;
        lambdaup = -287;
        lambdadown = 364;
        lambdav = 214;
        rho = -338;
        mu = -686;
        stepnorm = 912;
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
    
    /*
     * Routine body
     */
    
    /*
     * Prepare
     */
    n = state->n;
    m = state->m;
    state->repterminationtype = 0;
    state->repiterationscount = 0;
    state->repnfunc = 0;
    state->repnjac = 0;
    
    /*
     * Calculate F/G, initialize algorithm
     */
    nleq_clearrequestfields(state, _state);
    state->needf = ae_true;
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    state->needf = ae_false;
    state->repnfunc = state->repnfunc+1;
    ae_v_move(&state->xbase.ptr.p_double[0], 1, &state->x.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->fbase = state->f;
    state->fprev = ae_maxrealnumber;
    if( !state->xrep )
    {
        goto lbl_5;
    }
    
    /*
     * progress report
     */
    nleq_clearrequestfields(state, _state);
    state->xupdated = ae_true;
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    state->xupdated = ae_false;
lbl_5:
    if( ae_fp_less_eq(state->f,ae_sqr(state->epsf, _state)) )
    {
        state->repterminationtype = 1;
        result = ae_false;
        return result;
    }
    
    /*
     * Main cycle
     */
    lambdaup = (double)(10);
    lambdadown = 0.3;
    lambdav = 0.001;
    rho = (double)(1);
lbl_7:
    if( ae_false )
    {
        goto lbl_8;
    }
    
    /*
     * Get Jacobian;
     * before we get to this point we already have State.XBase filled
     * with current point and State.FBase filled with function value
     * at XBase
     */
    nleq_clearrequestfields(state, _state);
    state->needfij = ae_true;
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    state->needfij = ae_false;
    state->repnfunc = state->repnfunc+1;
    state->repnjac = state->repnjac+1;
    rmatrixmv(n, m, &state->j, 0, 0, 1, &state->fi, 0, &state->rightpart, 0, _state);
    ae_v_muld(&state->rightpart.ptr.p_double[0], 1, ae_v_len(0,n-1), -1);
    
    /*
     * Inner cycle: find good lambda
     */
lbl_9:
    if( ae_false )
    {
        goto lbl_10;
    }
    
    /*
     * Solve (J^T*J + (Lambda+Mu)*I)*y = J^T*F
     * to get step d=-y where:
     * * Mu=||F|| - is damping parameter for nonlinear system
     * * Lambda   - is additional Levenberg-Marquardt parameter
     *              for better convergence when far away from minimum
     */
    for(i=0; i<=n-1; i++)
    {
        state->candstep.ptr.p_double[i] = (double)(0);
    }
    fblssolvecgx(&state->j, m, n, lambdav, &state->rightpart, &state->candstep, &state->cgbuf, _state);
    
    /*
     * Normalize step (it must be no more than StpMax)
     */
    stepnorm = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_neq(state->candstep.ptr.p_double[i],(double)(0)) )
        {
            stepnorm = (double)(1);
            break;
        }
    }
    linminnormalized(&state->candstep, &stepnorm, n, _state);
    if( ae_fp_neq(state->stpmax,(double)(0)) )
    {
        stepnorm = ae_minreal(stepnorm, state->stpmax, _state);
    }
    
    /*
     * Test new step - is it good enough?
     * * if not, Lambda is increased and we try again.
     * * if step is good, we decrease Lambda and move on.
     *
     * We can break this cycle on two occasions:
     * * step is so small that x+step==x (in floating point arithmetics)
     * * lambda is so large
     */
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_addd(&state->x.ptr.p_double[0], 1, &state->candstep.ptr.p_double[0], 1, ae_v_len(0,n-1), stepnorm);
    b = ae_true;
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_neq(state->x.ptr.p_double[i],state->xbase.ptr.p_double[i]) )
        {
            b = ae_false;
            break;
        }
    }
    if( b )
    {
        
        /*
         * Step is too small, force zero step and break
         */
        stepnorm = (double)(0);
        ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
        state->f = state->fbase;
        goto lbl_10;
    }
    nleq_clearrequestfields(state, _state);
    state->needf = ae_true;
    state->rstate.stage = 3;
    goto lbl_rcomm;
lbl_3:
    state->needf = ae_false;
    state->repnfunc = state->repnfunc+1;
    if( ae_fp_less(state->f,state->fbase) )
    {
        
        /*
         * function value decreased, move on
         */
        nleq_decreaselambda(&lambdav, &rho, lambdadown, _state);
        goto lbl_10;
    }
    if( !nleq_increaselambda(&lambdav, &rho, lambdaup, _state) )
    {
        
        /*
         * Lambda is too large (near overflow), force zero step and break
         */
        stepnorm = (double)(0);
        ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
        state->f = state->fbase;
        goto lbl_10;
    }
    goto lbl_9;
lbl_10:
    
    /*
     * Accept step:
     * * new position
     * * new function value
     */
    state->fbase = state->f;
    ae_v_addd(&state->xbase.ptr.p_double[0], 1, &state->candstep.ptr.p_double[0], 1, ae_v_len(0,n-1), stepnorm);
    state->repiterationscount = state->repiterationscount+1;
    
    /*
     * Report new iteration
     */
    if( !state->xrep )
    {
        goto lbl_11;
    }
    nleq_clearrequestfields(state, _state);
    state->xupdated = ae_true;
    state->f = state->fbase;
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->rstate.stage = 4;
    goto lbl_rcomm;
lbl_4:
    state->xupdated = ae_false;
lbl_11:
    
    /*
     * Test stopping conditions on F, step (zero/non-zero) and MaxIts;
     * If one of the conditions is met, RepTerminationType is changed.
     */
    if( ae_fp_less_eq(ae_sqrt(state->f, _state),state->epsf) )
    {
        state->repterminationtype = 1;
    }
    if( ae_fp_eq(stepnorm,(double)(0))&&state->repterminationtype==0 )
    {
        state->repterminationtype = -4;
    }
    if( state->repiterationscount>=state->maxits&&state->maxits>0 )
    {
        state->repterminationtype = 5;
    }
    if( state->repterminationtype!=0 )
    {
        goto lbl_8;
    }
    
    /*
     * Now, iteration is finally over
     */
    goto lbl_7;
lbl_8:
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstate.ia.ptr.p_int[0] = n;
    state->rstate.ia.ptr.p_int[1] = m;
    state->rstate.ia.ptr.p_int[2] = i;
    state->rstate.ba.ptr.p_bool[0] = b;
    state->rstate.ra.ptr.p_double[0] = lambdaup;
    state->rstate.ra.ptr.p_double[1] = lambdadown;
    state->rstate.ra.ptr.p_double[2] = lambdav;
    state->rstate.ra.ptr.p_double[3] = rho;
    state->rstate.ra.ptr.p_double[4] = mu;
    state->rstate.ra.ptr.p_double[5] = stepnorm;
    return result;
}


/*************************************************************************
NLEQ solver results

INPUT PARAMETERS:
    State   -   algorithm state.

OUTPUT PARAMETERS:
    X       -   array[0..N-1], solution
    Rep     -   optimization report:
                * Rep.TerminationType completetion code:
                    * -4    ERROR:  algorithm   has   converged   to   the
                            stationary point Xf which is local minimum  of
                            f=F[0]^2+...+F[m-1]^2, but is not solution  of
                            nonlinear system.
                    *  1    sqrt(f)<=EpsF.
                    *  5    MaxIts steps was taken
                    *  7    stopping conditions are too stringent,
                            further improvement is impossible
                * Rep.IterationsCount contains iterations count
                * NFEV countains number of function calculations
                * ActiveConstraints contains number of active constraints

  -- ALGLIB --
     Copyright 20.08.2009 by Bochkanov Sergey
*************************************************************************/
void nleqresults(nleqstate* state,
     /* Real    */ ae_vector* x,
     nleqreport* rep,
     ae_state *_state)
{

    ae_vector_clear(x);
    _nleqreport_clear(rep);

    nleqresultsbuf(state, x, rep, _state);
}


/*************************************************************************
NLEQ solver results

Buffered implementation of NLEQResults(), which uses pre-allocated  buffer
to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
intended to be used in the inner cycles of performance critical algorithms
where array reallocation penalty is too large to be ignored.

  -- ALGLIB --
     Copyright 20.08.2009 by Bochkanov Sergey
*************************************************************************/
void nleqresultsbuf(nleqstate* state,
     /* Real    */ ae_vector* x,
     nleqreport* rep,
     ae_state *_state)
{


    if( x->cnt<state->n )
    {
        ae_vector_set_length(x, state->n, _state);
    }
    ae_v_move(&x->ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    rep->iterationscount = state->repiterationscount;
    rep->nfunc = state->repnfunc;
    rep->njac = state->repnjac;
    rep->terminationtype = state->repterminationtype;
}


/*************************************************************************
This  subroutine  restarts  CG  algorithm from new point. All optimization
parameters are left unchanged.

This  function  allows  to  solve multiple  optimization  problems  (which
must have same number of dimensions) without object reallocation penalty.

INPUT PARAMETERS:
    State   -   structure used for reverse communication previously
                allocated with MinCGCreate call.
    X       -   new starting point.
    BndL    -   new lower bounds
    BndU    -   new upper bounds

  -- ALGLIB --
     Copyright 30.07.2010 by Bochkanov Sergey
*************************************************************************/
void nleqrestartfrom(nleqstate* state,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{


    ae_assert(x->cnt>=state->n, "NLEQRestartFrom: Length(X)<N!", _state);
    ae_assert(isfinitevector(x, state->n, _state), "NLEQRestartFrom: X contains infinite or NaN values!", _state);
    ae_v_move(&state->x.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    ae_vector_set_length(&state->rstate.ia, 2+1, _state);
    ae_vector_set_length(&state->rstate.ba, 0+1, _state);
    ae_vector_set_length(&state->rstate.ra, 5+1, _state);
    state->rstate.stage = -1;
    nleq_clearrequestfields(state, _state);
}


/*************************************************************************
Clears request fileds (to be sure that we don't forgot to clear something)
*************************************************************************/
static void nleq_clearrequestfields(nleqstate* state, ae_state *_state)
{


    state->needf = ae_false;
    state->needfij = ae_false;
    state->xupdated = ae_false;
}


/*************************************************************************
Increases lambda, returns False when there is a danger of overflow
*************************************************************************/
static ae_bool nleq_increaselambda(double* lambdav,
     double* nu,
     double lambdaup,
     ae_state *_state)
{
    double lnlambda;
    double lnnu;
    double lnlambdaup;
    double lnmax;
    ae_bool result;


    result = ae_false;
    lnlambda = ae_log(*lambdav, _state);
    lnlambdaup = ae_log(lambdaup, _state);
    lnnu = ae_log(*nu, _state);
    lnmax = 0.5*ae_log(ae_maxrealnumber, _state);
    if( ae_fp_greater(lnlambda+lnlambdaup+lnnu,lnmax) )
    {
        return result;
    }
    if( ae_fp_greater(lnnu+ae_log((double)(2), _state),lnmax) )
    {
        return result;
    }
    *lambdav = *lambdav*lambdaup*(*nu);
    *nu = *nu*2;
    result = ae_true;
    return result;
}


/*************************************************************************
Decreases lambda, but leaves it unchanged when there is danger of underflow.
*************************************************************************/
static void nleq_decreaselambda(double* lambdav,
     double* nu,
     double lambdadown,
     ae_state *_state)
{


    *nu = (double)(1);
    if( ae_fp_less(ae_log(*lambdav, _state)+ae_log(lambdadown, _state),ae_log(ae_minrealnumber, _state)) )
    {
        *lambdav = ae_minrealnumber;
    }
    else
    {
        *lambdav = *lambdav*lambdadown;
    }
}


void _nleqstate_init(void* _p, ae_state *_state)
{
    nleqstate *p = (nleqstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->fi, 0, DT_REAL, _state);
    ae_matrix_init(&p->j, 0, 0, DT_REAL, _state);
    _rcommstate_init(&p->rstate, _state);
    ae_vector_init(&p->xbase, 0, DT_REAL, _state);
    ae_vector_init(&p->candstep, 0, DT_REAL, _state);
    ae_vector_init(&p->rightpart, 0, DT_REAL, _state);
    ae_vector_init(&p->cgbuf, 0, DT_REAL, _state);
}


void _nleqstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    nleqstate *dst = (nleqstate*)_dst;
    nleqstate *src = (nleqstate*)_src;
    dst->n = src->n;
    dst->m = src->m;
    dst->epsf = src->epsf;
    dst->maxits = src->maxits;
    dst->xrep = src->xrep;
    dst->stpmax = src->stpmax;
    ae_vector_init_copy(&dst->x, &src->x, _state);
    dst->f = src->f;
    ae_vector_init_copy(&dst->fi, &src->fi, _state);
    ae_matrix_init_copy(&dst->j, &src->j, _state);
    dst->needf = src->needf;
    dst->needfij = src->needfij;
    dst->xupdated = src->xupdated;
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
    dst->repiterationscount = src->repiterationscount;
    dst->repnfunc = src->repnfunc;
    dst->repnjac = src->repnjac;
    dst->repterminationtype = src->repterminationtype;
    ae_vector_init_copy(&dst->xbase, &src->xbase, _state);
    dst->fbase = src->fbase;
    dst->fprev = src->fprev;
    ae_vector_init_copy(&dst->candstep, &src->candstep, _state);
    ae_vector_init_copy(&dst->rightpart, &src->rightpart, _state);
    ae_vector_init_copy(&dst->cgbuf, &src->cgbuf, _state);
}


void _nleqstate_clear(void* _p)
{
    nleqstate *p = (nleqstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->fi);
    ae_matrix_clear(&p->j);
    _rcommstate_clear(&p->rstate);
    ae_vector_clear(&p->xbase);
    ae_vector_clear(&p->candstep);
    ae_vector_clear(&p->rightpart);
    ae_vector_clear(&p->cgbuf);
}


void _nleqstate_destroy(void* _p)
{
    nleqstate *p = (nleqstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->fi);
    ae_matrix_destroy(&p->j);
    _rcommstate_destroy(&p->rstate);
    ae_vector_destroy(&p->xbase);
    ae_vector_destroy(&p->candstep);
    ae_vector_destroy(&p->rightpart);
    ae_vector_destroy(&p->cgbuf);
}


void _nleqreport_init(void* _p, ae_state *_state)
{
    nleqreport *p = (nleqreport*)_p;
    ae_touch_ptr((void*)p);
}


void _nleqreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    nleqreport *dst = (nleqreport*)_dst;
    nleqreport *src = (nleqreport*)_src;
    dst->iterationscount = src->iterationscount;
    dst->nfunc = src->nfunc;
    dst->njac = src->njac;
    dst->terminationtype = src->terminationtype;
}


void _nleqreport_clear(void* _p)
{
    nleqreport *p = (nleqreport*)_p;
    ae_touch_ptr((void*)p);
}


void _nleqreport_destroy(void* _p)
{
    nleqreport *p = (nleqreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
