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
#include "minnlc.h"


/*$ Declarations $*/
static double minnlc_aulmaxgrowth = 10.0;
static ae_int_t minnlc_lbfgsfactor = 10;
static double minnlc_hessesttol = 1.0E-6;
static void minnlc_clearrequestfields(minnlcstate* state,
     ae_state *_state);
static void minnlc_minnlcinitinternal(ae_int_t n,
     /* Real    */ ae_vector* x,
     double diffstep,
     minnlcstate* state,
     ae_state *_state);
static void minnlc_clearpreconditioner(minlbfgsstate* auloptimizer,
     ae_state *_state);
static void minnlc_updatepreconditioner(ae_int_t prectype,
     ae_int_t updatefreq,
     ae_int_t* preccounter,
     minlbfgsstate* auloptimizer,
     /* Real    */ ae_vector* x,
     double rho,
     double gammak,
     /* Real    */ ae_vector* bndl,
     /* Boolean */ ae_vector* hasbndl,
     /* Real    */ ae_vector* bndu,
     /* Boolean */ ae_vector* hasbndu,
     /* Real    */ ae_vector* nubc,
     /* Real    */ ae_matrix* cleic,
     /* Real    */ ae_vector* nulc,
     /* Real    */ ae_vector* fi,
     /* Real    */ ae_matrix* jac,
     /* Real    */ ae_vector* nunlc,
     /* Real    */ ae_vector* bufd,
     /* Real    */ ae_vector* bufc,
     /* Real    */ ae_matrix* bufw,
     ae_int_t n,
     ae_int_t nec,
     ae_int_t nic,
     ae_int_t ng,
     ae_int_t nh,
     ae_state *_state);
static void minnlc_penaltybc(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* bndl,
     /* Boolean */ ae_vector* hasbndl,
     /* Real    */ ae_vector* bndu,
     /* Boolean */ ae_vector* hasbndu,
     /* Real    */ ae_vector* nubc,
     ae_int_t n,
     double rho,
     double stabilizingpoint,
     double* f,
     /* Real    */ ae_vector* g,
     ae_state *_state);
static void minnlc_penaltylc(/* Real    */ ae_vector* x,
     /* Real    */ ae_matrix* cleic,
     /* Real    */ ae_vector* nulc,
     ae_int_t n,
     ae_int_t nec,
     ae_int_t nic,
     double rho,
     double stabilizingpoint,
     double* f,
     /* Real    */ ae_vector* g,
     ae_state *_state);
static void minnlc_penaltynlc(/* Real    */ ae_vector* fi,
     /* Real    */ ae_matrix* j,
     /* Real    */ ae_vector* nunlc,
     ae_int_t n,
     ae_int_t ng,
     ae_int_t nh,
     double rho,
     double stabilizingpoint,
     double* f,
     /* Real    */ ae_vector* g,
     ae_state *_state);
static ae_bool minnlc_auliteration(minnlcstate* state, ae_state *_state);


/*$ Body $*/


/*************************************************************************
                  NONLINEARLY  CONSTRAINED  OPTIMIZATION
            WITH PRECONDITIONED AUGMENTED LAGRANGIAN ALGORITHM

DESCRIPTION:
The  subroutine  minimizes  function   F(x)  of N arguments subject to any
combination of:
* bound constraints
* linear inequality constraints
* linear equality constraints
* nonlinear equality constraints Gi(x)=0
* nonlinear inequality constraints Hi(x)<=0

REQUIREMENTS:
* user must provide function value and gradient for F(), H(), G()
* starting point X0 must be feasible or not too far away from the feasible
  set
* F(), G(), H() are twice continuously differentiable on the feasible  set
  and its neighborhood
* nonlinear constraints G() and H() must have non-zero gradient at  G(x)=0
  and at H(x)=0. Say, constraint like x^2>=1 is supported, but x^2>=0   is
  NOT supported.

USAGE:

Constrained optimization if far more complex than the  unconstrained  one.
Nonlinearly constrained optimization is one of the most esoteric numerical
procedures.

Here we give very brief outline  of  the  MinNLC  optimizer.  We  strongly
recommend you to study examples in the ALGLIB Reference Manual and to read
ALGLIB User Guide on optimization, which is available at
http://www.alglib.net/optimization/

1. User initializes algorithm state with MinNLCCreate() call  and  chooses
   what NLC solver to use. There is some solver which is used by  default,
   with default settings, but you should NOT rely on  default  choice.  It
   may change in future releases of ALGLIB without notice, and no one  can
   guarantee that new solver will be  able  to  solve  your  problem  with
   default settings.
   
   From the other side, if you choose solver explicitly, you can be pretty
   sure that it will work with new ALGLIB releases.
   
   In the current release following solvers can be used:
   * AUL solver (activated with MinNLCSetAlgoAUL() function)

2. User adds boundary and/or linear and/or nonlinear constraints by  means
   of calling one of the following functions:
   a) MinNLCSetBC() for boundary constraints
   b) MinNLCSetLC() for linear constraints
   c) MinNLCSetNLC() for nonlinear constraints
   You may combine (a), (b) and (c) in one optimization problem.
   
3. User sets scale of the variables with MinNLCSetScale() function. It  is
   VERY important to set  scale  of  the  variables,  because  nonlinearly
   constrained problems are hard to solve when variables are badly scaled.

4. User sets  stopping  conditions  with  MinNLCSetCond(). If  NLC  solver
   uses  inner/outer  iteration  layout,  this  function   sets   stopping
   conditions for INNER iterations.
   
5. User chooses one of the  preconditioning  methods.  Preconditioning  is
   very  important  for  efficient  handling  of boundary/linear/nonlinear
   constraints. Without preconditioning algorithm would require  thousands
   of iterations even for simple problems.  Two  preconditioners  can   be
   used:
   * approximate LBFGS-based  preconditioner  which  should  be  used  for
     problems with almost orthogonal  constraints  (activated  by  calling
     MinNLCSetPrecInexact)
   * exact low-rank preconditiner (activated by MinNLCSetPrecExactLowRank)
     which should be used for problems with moderate number of constraints
     which do not have to be orthogonal.

6. Finally, user calls MinNLCOptimize()  function  which  takes  algorithm
   state and pointer (delegate, etc.) to callback function which calculates
   F/G/H.

7. User calls MinNLCResults() to get solution

8. Optionally user may call MinNLCRestartFrom() to solve  another  problem
   with same N but another starting point. MinNLCRestartFrom()  allows  to
   reuse already initialized structure.


INPUT PARAMETERS:
    N       -   problem dimension, N>0:
                * if given, only leading N elements of X are used
                * if not given, automatically determined from size ofX
    X       -   starting point, array[N]:
                * it is better to set X to a feasible point
                * but X can be infeasible, in which case algorithm will try
                  to find feasible point first, using X as initial
                  approximation.

OUTPUT PARAMETERS:
    State   -   structure stores algorithm state

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlccreate(ae_int_t n,
     /* Real    */ ae_vector* x,
     minnlcstate* state,
     ae_state *_state)
{

    _minnlcstate_clear(state);

    ae_assert(n>=1, "MinNLCCreate: N<1", _state);
    ae_assert(x->cnt>=n, "MinNLCCreate: Length(X)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "MinNLCCreate: X contains infinite or NaN values", _state);
    minnlc_minnlcinitinternal(n, x, 0.0, state, _state);
}


/*************************************************************************
This subroutine is a finite  difference variant of MinNLCCreate(). It uses
finite differences in order to differentiate target function.

Description below contains information which is specific to this  function
only. We recommend to read comments on MinNLCCreate() in order to get more
information about creation of NLC optimizer.

INPUT PARAMETERS:
    N       -   problem dimension, N>0:
                * if given, only leading N elements of X are used
                * if not given, automatically determined from size ofX
    X       -   starting point, array[N]:
                * it is better to set X to a feasible point
                * but X can be infeasible, in which case algorithm will try
                  to find feasible point first, using X as initial
                  approximation.
    DiffStep-   differentiation step, >0

OUTPUT PARAMETERS:
    State   -   structure stores algorithm state

NOTES:
1. algorithm uses 4-point central formula for differentiation.
2. differentiation step along I-th axis is equal to DiffStep*S[I] where
   S[] is scaling vector which can be set by MinNLCSetScale() call.
3. we recommend you to use moderate values of  differentiation  step.  Too
   large step will result in too large TRUNCATION  errors, while too small
   step will result in too large NUMERICAL  errors.  1.0E-4  can  be  good
   value to start from.
4. Numerical  differentiation  is   very   inefficient  -   one   gradient
   calculation needs 4*N function evaluations. This function will work for
   any N - either small (1...10), moderate (10...100) or  large  (100...).
   However, performance penalty will be too severe for any N's except  for
   small ones.
   We should also say that code which relies on numerical  differentiation
   is  less   robust   and  precise.  Imprecise  gradient  may  slow  down
   convergence, especially on highly nonlinear problems.
   Thus  we  recommend to use this function for fast prototyping on small-
   dimensional problems only, and to implement analytical gradient as soon
   as possible.

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlccreatef(ae_int_t n,
     /* Real    */ ae_vector* x,
     double diffstep,
     minnlcstate* state,
     ae_state *_state)
{

    _minnlcstate_clear(state);

    ae_assert(n>=1, "MinNLCCreateF: N<1", _state);
    ae_assert(x->cnt>=n, "MinNLCCreateF: Length(X)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "MinNLCCreateF: X contains infinite or NaN values", _state);
    ae_assert(ae_isfinite(diffstep, _state), "MinNLCCreateF: DiffStep is infinite or NaN!", _state);
    ae_assert(ae_fp_greater(diffstep,(double)(0)), "MinNLCCreateF: DiffStep is non-positive!", _state);
    minnlc_minnlcinitinternal(n, x, diffstep, state, _state);
}


/*************************************************************************
This function sets boundary constraints for NLC optimizer.

Boundary constraints are inactive by  default  (after  initial  creation).
They are preserved after algorithm restart with  MinNLCRestartFrom().

You may combine boundary constraints with  general  linear ones - and with
nonlinear ones! Boundary constraints are  handled  more  efficiently  than
other types.  Thus,  if  your  problem  has  mixed  constraints,  you  may
explicitly specify some of them as boundary and save some time/space.

INPUT PARAMETERS:
    State   -   structure stores algorithm state
    BndL    -   lower bounds, array[N].
                If some (all) variables are unbounded, you may specify
                very small number or -INF.
    BndU    -   upper bounds, array[N].
                If some (all) variables are unbounded, you may specify
                very large number or +INF.

NOTE 1:  it is possible to specify  BndL[i]=BndU[i].  In  this  case  I-th
variable will be "frozen" at X[i]=BndL[i]=BndU[i].

NOTE 2:  when you solve your problem  with  augmented  Lagrangian  solver,
         boundary constraints are  satisfied  only  approximately!  It  is
         possible   that  algorithm  will  evaluate  function  outside  of
         feasible area!

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcsetbc(minnlcstate* state,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t n;


    n = state->n;
    ae_assert(bndl->cnt>=n, "MinNLCSetBC: Length(BndL)<N", _state);
    ae_assert(bndu->cnt>=n, "MinNLCSetBC: Length(BndU)<N", _state);
    for(i=0; i<=n-1; i++)
    {
        ae_assert(ae_isfinite(bndl->ptr.p_double[i], _state)||ae_isneginf(bndl->ptr.p_double[i], _state), "MinNLCSetBC: BndL contains NAN or +INF", _state);
        ae_assert(ae_isfinite(bndu->ptr.p_double[i], _state)||ae_isposinf(bndu->ptr.p_double[i], _state), "MinNLCSetBC: BndL contains NAN or -INF", _state);
        state->bndl.ptr.p_double[i] = bndl->ptr.p_double[i];
        state->hasbndl.ptr.p_bool[i] = ae_isfinite(bndl->ptr.p_double[i], _state);
        state->bndu.ptr.p_double[i] = bndu->ptr.p_double[i];
        state->hasbndu.ptr.p_bool[i] = ae_isfinite(bndu->ptr.p_double[i], _state);
    }
}


/*************************************************************************
This function sets linear constraints for MinNLC optimizer.

Linear constraints are inactive by default (after initial creation).  They
are preserved after algorithm restart with MinNLCRestartFrom().

You may combine linear constraints with boundary ones - and with nonlinear
ones! If your problem has mixed constraints, you  may  explicitly  specify
some of them as linear. It  may  help  optimizer   to   handle  them  more
efficiently.

INPUT PARAMETERS:
    State   -   structure previously allocated with MinNLCCreate call.
    C       -   linear constraints, array[K,N+1].
                Each row of C represents one constraint, either equality
                or inequality (see below):
                * first N elements correspond to coefficients,
                * last element corresponds to the right part.
                All elements of C (including right part) must be finite.
    CT      -   type of constraints, array[K]:
                * if CT[i]>0, then I-th constraint is C[i,*]*x >= C[i,n+1]
                * if CT[i]=0, then I-th constraint is C[i,*]*x  = C[i,n+1]
                * if CT[i]<0, then I-th constraint is C[i,*]*x <= C[i,n+1]
    K       -   number of equality/inequality constraints, K>=0:
                * if given, only leading K elements of C/CT are used
                * if not given, automatically determined from sizes of C/CT

NOTE 1: when you solve your problem  with  augmented  Lagrangian   solver,
        linear constraints are  satisfied  only   approximately!   It   is
        possible   that  algorithm  will  evaluate  function  outside   of
        feasible area!

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcsetlc(minnlcstate* state,
     /* Real    */ ae_matrix* c,
     /* Integer */ ae_vector* ct,
     ae_int_t k,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;


    n = state->n;
    
    /*
     * First, check for errors in the inputs
     */
    ae_assert(k>=0, "MinNLCSetLC: K<0", _state);
    ae_assert(c->cols>=n+1||k==0, "MinNLCSetLC: Cols(C)<N+1", _state);
    ae_assert(c->rows>=k, "MinNLCSetLC: Rows(C)<K", _state);
    ae_assert(ct->cnt>=k, "MinNLCSetLC: Length(CT)<K", _state);
    ae_assert(apservisfinitematrix(c, k, n+1, _state), "MinNLCSetLC: C contains infinite or NaN values!", _state);
    
    /*
     * Handle zero K
     */
    if( k==0 )
    {
        state->nec = 0;
        state->nic = 0;
        return;
    }
    
    /*
     * Equality constraints are stored first, in the upper
     * NEC rows of State.CLEIC matrix. Inequality constraints
     * are stored in the next NIC rows.
     *
     * NOTE: we convert inequality constraints to the form
     * A*x<=b before copying them.
     */
    rmatrixsetlengthatleast(&state->cleic, k, n+1, _state);
    state->nec = 0;
    state->nic = 0;
    for(i=0; i<=k-1; i++)
    {
        if( ct->ptr.p_int[i]==0 )
        {
            ae_v_move(&state->cleic.ptr.pp_double[state->nec][0], 1, &c->ptr.pp_double[i][0], 1, ae_v_len(0,n));
            state->nec = state->nec+1;
        }
    }
    for(i=0; i<=k-1; i++)
    {
        if( ct->ptr.p_int[i]!=0 )
        {
            if( ct->ptr.p_int[i]>0 )
            {
                ae_v_moveneg(&state->cleic.ptr.pp_double[state->nec+state->nic][0], 1, &c->ptr.pp_double[i][0], 1, ae_v_len(0,n));
            }
            else
            {
                ae_v_move(&state->cleic.ptr.pp_double[state->nec+state->nic][0], 1, &c->ptr.pp_double[i][0], 1, ae_v_len(0,n));
            }
            state->nic = state->nic+1;
        }
    }
}


/*************************************************************************
This function sets nonlinear constraints for MinNLC optimizer.

In fact, this function sets NUMBER of nonlinear  constraints.  Constraints
itself (constraint functions) are passed to MinNLCOptimize() method.  This
method requires user-defined vector function F[]  and  its  Jacobian  J[],
where:
* first component of F[] and first row  of  Jacobian  J[]  corresponds  to
  function being minimized
* next NLEC components of F[] (and rows  of  J)  correspond  to  nonlinear
  equality constraints G_i(x)=0
* next NLIC components of F[] (and rows  of  J)  correspond  to  nonlinear
  inequality constraints H_i(x)<=0

NOTE: you may combine nonlinear constraints with linear/boundary ones.  If
      your problem has mixed constraints, you  may explicitly specify some
      of them as linear ones. It may help optimizer to  handle  them  more
      efficiently.

INPUT PARAMETERS:
    State   -   structure previously allocated with MinNLCCreate call.
    NLEC    -   number of Non-Linear Equality Constraints (NLEC), >=0
    NLIC    -   number of Non-Linear Inquality Constraints (NLIC), >=0

NOTE 1: when you solve your problem  with  augmented  Lagrangian   solver,
        nonlinear constraints are satisfied only  approximately!   It   is
        possible   that  algorithm  will  evaluate  function  outside   of
        feasible area!
        
NOTE 2: algorithm scales variables  according  to   scale   specified   by
        MinNLCSetScale()  function,  so  it can handle problems with badly
        scaled variables (as long as we KNOW their scales).
           
        However,  there  is  no  way  to  automatically  scale   nonlinear
        constraints Gi(x) and Hi(x). Inappropriate scaling  of  Gi/Hi  may
        ruin convergence. Solving problem with  constraint  "1000*G0(x)=0"
        is NOT same as solving it with constraint "0.001*G0(x)=0".
           
        It  means  that  YOU  are  the  one who is responsible for correct
        scaling of nonlinear constraints Gi(x) and Hi(x). We recommend you
        to scale nonlinear constraints in such way that I-th component  of
        dG/dX (or dH/dx) has approximately unit  magnitude  (for  problems
        with unit scale)  or  has  magnitude approximately equal to 1/S[i]
        (where S is a scale set by MinNLCSetScale() function).


  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcsetnlc(minnlcstate* state,
     ae_int_t nlec,
     ae_int_t nlic,
     ae_state *_state)
{


    ae_assert(nlec>=0, "MinNLCSetNLC: NLEC<0", _state);
    ae_assert(nlic>=0, "MinNLCSetNLC: NLIC<0", _state);
    state->ng = nlec;
    state->nh = nlic;
    ae_vector_set_length(&state->fi, 1+state->ng+state->nh, _state);
    ae_matrix_set_length(&state->j, 1+state->ng+state->nh, state->n, _state);
}


/*************************************************************************
This function sets stopping conditions for inner iterations of  optimizer.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    EpsG    -   >=0
                The  subroutine  finishes  its  work   if   the  condition
                |v|<EpsG is satisfied, where:
                * |.| means Euclidian norm
                * v - scaled gradient vector, v[i]=g[i]*s[i]
                * g - gradient
                * s - scaling coefficients set by MinNLCSetScale()
    EpsF    -   >=0
                The  subroutine  finishes  its work if on k+1-th iteration
                the  condition  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
                is satisfied.
    EpsX    -   >=0
                The subroutine finishes its work if  on  k+1-th  iteration
                the condition |v|<=EpsX is fulfilled, where:
                * |.| means Euclidian norm
                * v - scaled step vector, v[i]=dx[i]/s[i]
                * dx - step vector, dx=X(k+1)-X(k)
                * s - scaling coefficients set by MinNLCSetScale()
    MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                iterations is unlimited.

Passing EpsG=0, EpsF=0 and EpsX=0 and MaxIts=0 (simultaneously) will lead
to automatic stopping criterion selection.

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcsetcond(minnlcstate* state,
     double epsg,
     double epsf,
     double epsx,
     ae_int_t maxits,
     ae_state *_state)
{


    ae_assert(ae_isfinite(epsg, _state), "MinNLCSetCond: EpsG is not finite number", _state);
    ae_assert(ae_fp_greater_eq(epsg,(double)(0)), "MinNLCSetCond: negative EpsG", _state);
    ae_assert(ae_isfinite(epsf, _state), "MinNLCSetCond: EpsF is not finite number", _state);
    ae_assert(ae_fp_greater_eq(epsf,(double)(0)), "MinNLCSetCond: negative EpsF", _state);
    ae_assert(ae_isfinite(epsx, _state), "MinNLCSetCond: EpsX is not finite number", _state);
    ae_assert(ae_fp_greater_eq(epsx,(double)(0)), "MinNLCSetCond: negative EpsX", _state);
    ae_assert(maxits>=0, "MinNLCSetCond: negative MaxIts!", _state);
    if( ((ae_fp_eq(epsg,(double)(0))&&ae_fp_eq(epsf,(double)(0)))&&ae_fp_eq(epsx,(double)(0)))&&maxits==0 )
    {
        epsx = 1.0E-6;
    }
    state->epsg = epsg;
    state->epsf = epsf;
    state->epsx = epsx;
    state->maxits = maxits;
}


/*************************************************************************
This function sets scaling coefficients for NLC optimizer.

ALGLIB optimizers use scaling matrices to test stopping  conditions  (step
size and gradient are scaled before comparison with tolerances).  Scale of
the I-th variable is a translation invariant measure of:
a) "how large" the variable is
b) how large the step should be to make significant changes in the function

Scaling is also used by finite difference variant of the optimizer  - step
along I-th axis is equal to DiffStep*S[I].

INPUT PARAMETERS:
    State   -   structure stores algorithm state
    S       -   array[N], non-zero scaling coefficients
                S[i] may be negative, sign doesn't matter.

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcsetscale(minnlcstate* state,
     /* Real    */ ae_vector* s,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(s->cnt>=state->n, "MinNLCSetScale: Length(S)<N", _state);
    for(i=0; i<=state->n-1; i++)
    {
        ae_assert(ae_isfinite(s->ptr.p_double[i], _state), "MinNLCSetScale: S contains infinite or NAN elements", _state);
        ae_assert(ae_fp_neq(s->ptr.p_double[i],(double)(0)), "MinNLCSetScale: S contains zero elements", _state);
        state->s.ptr.p_double[i] = ae_fabs(s->ptr.p_double[i], _state);
    }
}


/*************************************************************************
This function sets preconditioner to "inexact LBFGS-based" mode.

Preconditioning is very important for convergence of  Augmented Lagrangian
algorithm because presence of penalty term makes problem  ill-conditioned.
Difference between  performance  of  preconditioned  and  unpreconditioned
methods can be as large as 100x!

MinNLC optimizer may  utilize  two  preconditioners,  each  with  its  own
benefits and drawbacks: a) inexact LBFGS-based, and b) exact low rank one.
It also provides special unpreconditioned mode of operation which  can  be
used for test purposes. Comments below discuss LBFGS-based preconditioner.

Inexact  LBFGS-based  preconditioner  uses L-BFGS  formula  combined  with
orthogonality assumption to perform very fast updates. For a N-dimensional
problem with K general linear or nonlinear constraints (boundary ones  are
not counted) it has O(N*K) cost per iteration.  This   preconditioner  has
best  quality  (less  iterations)  when   general   linear  and  nonlinear
constraints are orthogonal to each other (orthogonality  with  respect  to
boundary constraints is not required). Number of iterations increases when
constraints  are  non-orthogonal, because algorithm assumes orthogonality,
but still it is better than no preconditioner at all.

INPUT PARAMETERS:
    State   -   structure stores algorithm state

  -- ALGLIB --
     Copyright 26.09.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcsetprecinexact(minnlcstate* state, ae_state *_state)
{


    state->updatefreq = 0;
    state->prectype = 1;
}


/*************************************************************************
This function sets preconditioner to "exact low rank" mode.

Preconditioning is very important for convergence of  Augmented Lagrangian
algorithm because presence of penalty term makes problem  ill-conditioned.
Difference between  performance  of  preconditioned  and  unpreconditioned
methods can be as large as 100x!

MinNLC optimizer may  utilize  two  preconditioners,  each  with  its  own
benefits and drawbacks: a) inexact LBFGS-based, and b) exact low rank one.
It also provides special unpreconditioned mode of operation which  can  be
used for test purposes. Comments below discuss low rank preconditioner.

Exact low-rank preconditioner  uses  Woodbury  matrix  identity  to  build
quadratic model of the penalized function. It has no  special  assumptions
about orthogonality, so it is quite general. However, for a  N-dimensional
problem with K general linear or nonlinear constraints (boundary ones  are
not counted) it has O(N*K^2) cost per iteration (for  comparison:  inexact
LBFGS-based preconditioner has O(N*K) cost).

INPUT PARAMETERS:
    State   -   structure stores algorithm state
    UpdateFreq- update frequency. Preconditioner is  rebuilt  after  every
                UpdateFreq iterations. Recommended value: 10 or higher.
                Zero value means that good default value will be used.

  -- ALGLIB --
     Copyright 26.09.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcsetprecexactlowrank(minnlcstate* state,
     ae_int_t updatefreq,
     ae_state *_state)
{


    ae_assert(updatefreq>=0, "MinNLCSetPrecExactLowRank: UpdateFreq<0", _state);
    if( updatefreq==0 )
    {
        updatefreq = 10;
    }
    state->prectype = 2;
    state->updatefreq = updatefreq;
}


/*************************************************************************
This function sets preconditioner to "turned off" mode.

Preconditioning is very important for convergence of  Augmented Lagrangian
algorithm because presence of penalty term makes problem  ill-conditioned.
Difference between  performance  of  preconditioned  and  unpreconditioned
methods can be as large as 100x!

MinNLC optimizer may  utilize  two  preconditioners,  each  with  its  own
benefits and drawbacks: a) inexact LBFGS-based, and b) exact low rank one.
It also provides special unpreconditioned mode of operation which  can  be
used for test purposes.

This function activates this test mode. Do not use it in  production  code
to solve real-life problems.

INPUT PARAMETERS:
    State   -   structure stores algorithm state

  -- ALGLIB --
     Copyright 26.09.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcsetprecnone(minnlcstate* state, ae_state *_state)
{


    state->updatefreq = 0;
    state->prectype = 0;
}


/*************************************************************************
This  function  tells MinNLC unit to use  Augmented  Lagrangian  algorithm
for nonlinearly constrained  optimization.  This  algorithm  is  a  slight
modification of one described in "A Modified Barrier-Augmented  Lagrangian
Method for  Constrained  Minimization  (1999)"  by  D.GOLDFARB,  R.POLYAK,
K. SCHEINBERG, I.YUZEFOVICH.

Augmented Lagrangian algorithm works by converting problem  of  minimizing
F(x) subject to equality/inequality constraints   to unconstrained problem
of the form

    min[ f(x) + 
        + Rho*PENALTY_EQ(x)   + SHIFT_EQ(x,Nu1) + 
        + Rho*PENALTY_INEQ(x) + SHIFT_INEQ(x,Nu2) ]
    
where:
* Rho is a fixed penalization coefficient
* PENALTY_EQ(x) is a penalty term, which is used to APPROXIMATELY  enforce
  equality constraints
* SHIFT_EQ(x) is a special "shift"  term  which  is  used  to  "fine-tune"
  equality constraints, greatly increasing precision
* PENALTY_INEQ(x) is a penalty term which is used to approximately enforce
  inequality constraints
* SHIFT_INEQ(x) is a special "shift"  term  which  is  used to "fine-tune"
  inequality constraints, greatly increasing precision
* Nu1/Nu2 are vectors of Lagrange coefficients which are fine-tuned during
  outer iterations of algorithm

This  version  of  AUL  algorithm  uses   preconditioner,  which   greatly
accelerates convergence. Because this  algorithm  is  similar  to  penalty
methods,  it  may  perform  steps  into  infeasible  area.  All  kinds  of
constraints (boundary, linear and nonlinear ones) may   be   violated   in
intermediate points - and in the solution.  However,  properly  configured
AUL method is significantly better at handling  constraints  than  barrier
and/or penalty methods.

The very basic outline of algorithm is given below:
1) first outer iteration is performed with "default"  values  of  Lagrange
   multipliers Nu1/Nu2. Solution quality is low (candidate  point  can  be
   too  far  away  from  true  solution; large violation of constraints is
   possible) and is comparable with that of penalty methods.
2) subsequent outer iterations  refine  Lagrange  multipliers  and improve
   quality of the solution.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    Rho     -   penalty coefficient, Rho>0:
                * large enough  that  algorithm  converges  with   desired
                  precision. Minimum value is 10*max(S'*diag(H)*S),  where
                  S is a scale matrix (set by MinNLCSetScale) and H  is  a
                  Hessian of the function being minimized. If you can  not
                  easily estimate Hessian norm,  see  our  recommendations
                  below.
                * not TOO large to prevent ill-conditioning
                * for unit-scale problems (variables and Hessian have unit
                  magnitude), Rho=100 or Rho=1000 can be used.
                * it is important to note that Rho is internally multiplied
                  by scaling matrix, i.e. optimum value of Rho depends  on
                  scale of variables specified  by  MinNLCSetScale().
    ItsCnt  -   number of outer iterations:
                * ItsCnt=0 means that small number of outer iterations  is
                  automatically chosen (10 iterations in current version).
                * ItsCnt=1 means that AUL algorithm performs just as usual
                  barrier method.
                * ItsCnt>1 means that  AUL  algorithm  performs  specified
                  number of outer iterations
                
HOW TO CHOOSE PARAMETERS

Nonlinear optimization is a tricky area and Augmented Lagrangian algorithm
is sometimes hard to tune. Good values of  Rho  and  ItsCnt  are  problem-
specific.  In  order  to  help  you   we   prepared   following   set   of
recommendations:

* for  unit-scale  problems  (variables  and Hessian have unit magnitude),
  Rho=100 or Rho=1000 can be used.

* start from  some  small  value of Rho and solve problem  with  just  one
  outer iteration (ItcCnt=1). In this case algorithm behaves like  penalty
  method. Increase Rho in 2x or 10x steps until you  see  that  one  outer
  iteration returns point which is "rough approximation to solution".
  
  It is very important to have Rho so  large  that  penalty  term  becomes
  constraining i.e. modified function becomes highly convex in constrained
  directions.
  
  From the other side, too large Rho may prevent you  from  converging  to
  the solution. You can diagnose it by studying number of inner iterations
  performed by algorithm: too few (5-10 on  1000-dimensional  problem)  or
  too many (orders of magnitude more than  dimensionality)  usually  means
  that Rho is too large.

* with just one outer iteration you  usually  have  low-quality  solution.
  Some constraints can be violated with very  large  margin,  while  other
  ones (which are NOT violated in the true solution) can push final  point
  too far in the inner area of the feasible set.
  
  For example, if you have constraint x0>=0 and true solution  x0=1,  then
  merely a presence of "x0>=0" will introduce a bias towards larger values
  of x0. Say, algorithm may stop at x0=1.5 instead of 1.0.
  
* after you found good Rho, you may increase number of  outer  iterations.
  ItsCnt=10 is a good value. Subsequent outer iteration will refine values
  of  Lagrange  multipliers.  Constraints  which  were  violated  will  be
  enforced, inactive constraints will be dropped (corresponding multipliers
  will be decreased). Ideally, you  should  see  10-1000x  improvement  in
  constraint handling (constraint violation is reduced).
  
* if  you  see  that  algorithm  converges  to  vicinity  of solution, but
  additional outer iterations do not refine solution,  it  may  mean  that
  algorithm is unstable - it wanders around true  solution,  but  can  not
  approach it. Sometimes algorithm may be stabilized by increasing Rho one
  more time, making it 5x or 10x larger.

SCALING OF CONSTRAINTS [IMPORTANT]

AUL optimizer scales   variables   according   to   scale   specified   by
MinNLCSetScale() function, so it can handle  problems  with  badly  scaled
variables (as long as we KNOW their scales).   However,  because  function
being optimized is a mix  of  original  function and  constraint-dependent
penalty  functions, it  is   important  to   rescale  both  variables  AND
constraints.

Say,  if  you  minimize f(x)=x^2 subject to 1000000*x>=0,  then  you  have
constraint whose scale is different from that of target  function (another
example is 0.000001*x>=0). It is also possible to have constraints   whose
scales  are   misaligned:   1000000*x0>=0, 0.000001*x1<=0.   Inappropriate
scaling may ruin convergence because minimizing x^2 subject to x>=0 is NOT
same as minimizing it subject to 1000000*x>=0.

Because we  know  coefficients  of  boundary/linear  constraints,  we  can
automatically rescale and normalize them. However,  there  is  no  way  to
automatically rescale nonlinear constraints Gi(x) and  Hi(x)  -  they  are
black boxes.

It means that YOU are the one who is  responsible  for  correct scaling of
nonlinear constraints  Gi(x)  and  Hi(x).  We  recommend  you  to  rescale
nonlinear constraints in such way that I-th component of dG/dX (or  dH/dx)
has magnitude approximately equal to 1/S[i] (where S  is  a  scale  set by
MinNLCSetScale() function).

WHAT IF IT DOES NOT CONVERGE?

It is possible that AUL algorithm fails to converge to precise  values  of
Lagrange multipliers. It stops somewhere around true solution, but candidate
point is still too far from solution, and some constraints  are  violated.
Such kind of failure is specific for Lagrangian algorithms -  technically,
they stop at some point, but this point is not constrained solution.

There are exist several reasons why algorithm may fail to converge:
a) too loose stopping criteria for inner iteration
b) degenerate, redundant constraints
c) target function has unconstrained extremum exactly at the  boundary  of
   some constraint
d) numerical noise in the target function

In all these cases algorithm is unstable - each outer iteration results in
large and almost random step which improves handling of some  constraints,
but violates other ones (ideally  outer iterations should form a  sequence
of progressively decreasing steps towards solution).
   
First reason possible is  that  too  loose  stopping  criteria  for  inner
iteration were specified. Augmented Lagrangian algorithm solves a sequence
of intermediate problems, and requries each of them to be solved with high
precision. Insufficient precision results in incorrect update of  Lagrange
multipliers.

Another reason is that you may have specified degenerate constraints: say,
some constraint was repeated twice. In most cases AUL algorithm gracefully
handles such situations, but sometimes it may spend too much time figuring
out subtle degeneracies in constraint matrix.

Third reason is tricky and hard to diagnose. Consider situation  when  you
minimize  f=x^2  subject to constraint x>=0.  Unconstrained   extremum  is
located  exactly  at  the  boundary  of  constrained  area.  In  this case
algorithm will tend to oscillate between negative  and  positive  x.  Each
time it stops at x<0 it "reinforces" constraint x>=0, and each time it  is
bounced to x>0 it "relaxes" constraint (and is  attracted  to  x<0).

Such situation  sometimes  happens  in  problems  with  hidden  symetries.
Algorithm  is  got  caught  in  a  loop with  Lagrange  multipliers  being
continuously increased/decreased. Luckily, such loop forms after at  least
three iterations, so this problem can be solved by  DECREASING  number  of
outer iterations down to 1-2 and increasing  penalty  coefficient  Rho  as
much as possible.

Final reason is numerical noise. AUL algorithm is robust against  moderate
noise (more robust than, say, active set methods),  but  large  noise  may
destabilize algorithm.

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcsetalgoaul(minnlcstate* state,
     double rho,
     ae_int_t itscnt,
     ae_state *_state)
{


    ae_assert(itscnt>=0, "MinNLCSetAlgoAUL: negative ItsCnt", _state);
    ae_assert(ae_isfinite(rho, _state), "MinNLCSetAlgoAUL: Rho is not finite", _state);
    ae_assert(ae_fp_greater(rho,(double)(0)), "MinNLCSetAlgoAUL: Rho<=0", _state);
    if( itscnt==0 )
    {
        itscnt = 10;
    }
    state->aulitscnt = itscnt;
    state->rho = rho;
    state->solvertype = 0;
}


/*************************************************************************
This function turns on/off reporting.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    NeedXRep-   whether iteration reports are needed or not

If NeedXRep is True, algorithm will call rep() callback function if  it is
provided to MinNLCOptimize().

NOTE: algorithm passes two parameters to rep() callback  -  current  point
      and penalized function value at current point. Important -  function
      value which is returned is NOT function being minimized. It  is  sum
      of the value of the function being minimized - and penalty term.

  -- ALGLIB --
     Copyright 28.11.2010 by Bochkanov Sergey
*************************************************************************/
void minnlcsetxrep(minnlcstate* state, ae_bool needxrep, ae_state *_state)
{


    state->xrep = needxrep;
}


/*************************************************************************

NOTES:

1. This function has two different implementations: one which  uses  exact
   (analytical) user-supplied Jacobian, and one which uses  only  function
   vector and numerically  differentiates  function  in  order  to  obtain
   gradient.

   Depending  on  the  specific  function  used to create optimizer object
   you should choose appropriate variant of MinNLCOptimize() -  one  which
   accepts function AND Jacobian or one which accepts ONLY function.

   Be careful to choose variant of MinNLCOptimize()  which  corresponds to
   your optimization scheme! Table below lists different  combinations  of
   callback (function/gradient) passed to MinNLCOptimize()   and  specific
   function used to create optimizer.


                     |         USER PASSED TO MinNLCOptimize()
   CREATED WITH      |  function only   |  function and gradient
   ------------------------------------------------------------
   MinNLCCreateF()   |     works               FAILS
   MinNLCCreate()    |     FAILS               works

   Here "FAILS" denotes inappropriate combinations  of  optimizer creation
   function  and  MinNLCOptimize()  version.   Attemps   to    use    such
   combination will lead to exception. Either  you  did  not pass gradient
   when it WAS needed or you passed gradient when it was NOT needed.

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
ae_bool minnlciteration(minnlcstate* state, ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    ae_int_t n;
    ae_int_t ng;
    ae_int_t nh;
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
        k = state->rstate.ia.ptr.p_int[1];
        n = state->rstate.ia.ptr.p_int[2];
        ng = state->rstate.ia.ptr.p_int[3];
        nh = state->rstate.ia.ptr.p_int[4];
    }
    else
    {
        i = -983;
        k = -989;
        n = -834;
        ng = 900;
        nh = -287;
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
    if( state->rstate.stage==8 )
    {
        goto lbl_8;
    }
    
    /*
     * Routine body
     */
    
    /*
     * Init
     */
    state->repterminationtype = 0;
    state->repinneriterationscount = 0;
    state->repouteriterationscount = 0;
    state->repnfev = 0;
    state->repvaridx = 0;
    state->repfuncidx = 0;
    state->repdbgphase0its = 0;
    n = state->n;
    ng = state->ng;
    nh = state->nh;
    minnlc_clearrequestfields(state, _state);
    
    /*
     * Test gradient
     */
    if( !(ae_fp_eq(state->diffstep,(double)(0))&&ae_fp_greater(state->teststep,(double)(0))) )
    {
        goto lbl_9;
    }
    rvectorsetlengthatleast(&state->xbase, n, _state);
    rvectorsetlengthatleast(&state->fbase, 1+ng+nh, _state);
    rvectorsetlengthatleast(&state->dfbase, 1+ng+nh, _state);
    rvectorsetlengthatleast(&state->fm1, 1+ng+nh, _state);
    rvectorsetlengthatleast(&state->fp1, 1+ng+nh, _state);
    rvectorsetlengthatleast(&state->dfm1, 1+ng+nh, _state);
    rvectorsetlengthatleast(&state->dfp1, 1+ng+nh, _state);
    state->needfij = ae_true;
    ae_v_move(&state->xbase.ptr.p_double[0], 1, &state->xstart.ptr.p_double[0], 1, ae_v_len(0,n-1));
    k = 0;
lbl_11:
    if( k>n-1 )
    {
        goto lbl_13;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    ae_v_move(&state->fbase.ptr.p_double[0], 1, &state->fi.ptr.p_double[0], 1, ae_v_len(0,ng+nh));
    ae_v_move(&state->dfbase.ptr.p_double[0], 1, &state->j.ptr.pp_double[0][k], state->j.stride, ae_v_len(0,ng+nh));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->x.ptr.p_double[k] = state->x.ptr.p_double[k]-state->s.ptr.p_double[k]*state->teststep;
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    ae_v_move(&state->fm1.ptr.p_double[0], 1, &state->fi.ptr.p_double[0], 1, ae_v_len(0,ng+nh));
    ae_v_move(&state->dfm1.ptr.p_double[0], 1, &state->j.ptr.pp_double[0][k], state->j.stride, ae_v_len(0,ng+nh));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->x.ptr.p_double[k] = state->x.ptr.p_double[k]+state->s.ptr.p_double[k]*state->teststep;
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    ae_v_move(&state->fp1.ptr.p_double[0], 1, &state->fi.ptr.p_double[0], 1, ae_v_len(0,ng+nh));
    ae_v_move(&state->dfp1.ptr.p_double[0], 1, &state->j.ptr.pp_double[0][k], state->j.stride, ae_v_len(0,ng+nh));
    for(i=0; i<=ng+nh; i++)
    {
        if( !derivativecheck(state->fm1.ptr.p_double[i], state->dfm1.ptr.p_double[i], state->fp1.ptr.p_double[i], state->dfp1.ptr.p_double[i], state->fbase.ptr.p_double[i], state->dfbase.ptr.p_double[i], 2*state->s.ptr.p_double[k]*state->teststep, _state) )
        {
            state->repfuncidx = i;
            state->repvaridx = k;
            state->repterminationtype = -7;
            result = ae_false;
            return result;
        }
    }
    k = k+1;
    goto lbl_11;
lbl_13:
    state->needfij = ae_false;
lbl_9:
    
    /*
     * AUL solver
     */
    if( state->solvertype!=0 )
    {
        goto lbl_14;
    }
    if( ae_fp_neq(state->diffstep,(double)(0)) )
    {
        rvectorsetlengthatleast(&state->xbase, n, _state);
        rvectorsetlengthatleast(&state->fbase, 1+ng+nh, _state);
        rvectorsetlengthatleast(&state->fm2, 1+ng+nh, _state);
        rvectorsetlengthatleast(&state->fm1, 1+ng+nh, _state);
        rvectorsetlengthatleast(&state->fp1, 1+ng+nh, _state);
        rvectorsetlengthatleast(&state->fp2, 1+ng+nh, _state);
    }
    ae_vector_set_length(&state->rstateaul.ia, 8+1, _state);
    ae_vector_set_length(&state->rstateaul.ra, 7+1, _state);
    state->rstateaul.stage = -1;
lbl_16:
    if( !minnlc_auliteration(state, _state) )
    {
        goto lbl_17;
    }
    
    /*
     * Numerical differentiation (if needed) - intercept NeedFiJ
     * request and replace it by sequence of NeedFi requests
     */
    if( !(ae_fp_neq(state->diffstep,(double)(0))&&state->needfij) )
    {
        goto lbl_18;
    }
    state->needfij = ae_false;
    state->needfi = ae_true;
    ae_v_move(&state->xbase.ptr.p_double[0], 1, &state->x.ptr.p_double[0], 1, ae_v_len(0,n-1));
    k = 0;
lbl_20:
    if( k>n-1 )
    {
        goto lbl_22;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->x.ptr.p_double[k] = state->x.ptr.p_double[k]-state->s.ptr.p_double[k]*state->diffstep;
    state->rstate.stage = 3;
    goto lbl_rcomm;
lbl_3:
    ae_v_move(&state->fm2.ptr.p_double[0], 1, &state->fi.ptr.p_double[0], 1, ae_v_len(0,ng+nh));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->x.ptr.p_double[k] = state->x.ptr.p_double[k]-0.5*state->s.ptr.p_double[k]*state->diffstep;
    state->rstate.stage = 4;
    goto lbl_rcomm;
lbl_4:
    ae_v_move(&state->fm1.ptr.p_double[0], 1, &state->fi.ptr.p_double[0], 1, ae_v_len(0,ng+nh));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->x.ptr.p_double[k] = state->x.ptr.p_double[k]+0.5*state->s.ptr.p_double[k]*state->diffstep;
    state->rstate.stage = 5;
    goto lbl_rcomm;
lbl_5:
    ae_v_move(&state->fp1.ptr.p_double[0], 1, &state->fi.ptr.p_double[0], 1, ae_v_len(0,ng+nh));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->x.ptr.p_double[k] = state->x.ptr.p_double[k]+state->s.ptr.p_double[k]*state->diffstep;
    state->rstate.stage = 6;
    goto lbl_rcomm;
lbl_6:
    ae_v_move(&state->fp2.ptr.p_double[0], 1, &state->fi.ptr.p_double[0], 1, ae_v_len(0,ng+nh));
    for(i=0; i<=ng+nh; i++)
    {
        state->j.ptr.pp_double[i][k] = (8*(state->fp1.ptr.p_double[i]-state->fm1.ptr.p_double[i])-(state->fp2.ptr.p_double[i]-state->fm2.ptr.p_double[i]))/(6*state->diffstep*state->s.ptr.p_double[i]);
    }
    k = k+1;
    goto lbl_20;
lbl_22:
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->rstate.stage = 7;
    goto lbl_rcomm;
lbl_7:
    
    /*
     * Restore previous values of fields and continue
     */
    state->needfi = ae_false;
    state->needfij = ae_true;
    goto lbl_16;
lbl_18:
    
    /*
     * Forward request to caller
     */
    state->rstate.stage = 8;
    goto lbl_rcomm;
lbl_8:
    goto lbl_16;
lbl_17:
    result = ae_false;
    return result;
lbl_14:
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstate.ia.ptr.p_int[0] = i;
    state->rstate.ia.ptr.p_int[1] = k;
    state->rstate.ia.ptr.p_int[2] = n;
    state->rstate.ia.ptr.p_int[3] = ng;
    state->rstate.ia.ptr.p_int[4] = nh;
    return result;
}


/*************************************************************************
MinNLC results

INPUT PARAMETERS:
    State   -   algorithm state

OUTPUT PARAMETERS:
    X       -   array[0..N-1], solution
    Rep     -   optimization report. You should check Rep.TerminationType
                in  order  to  distinguish  successful  termination  from
                unsuccessful one:
                * -8    internal integrity control  detected  infinite or
                        NAN   values   in   function/gradient.   Abnormal
                        termination signalled.
                * -7   gradient verification failed.
                       See MinNLCSetGradientCheck() for more information.
                *  1   relative function improvement is no more than EpsF.
                *  2   scaled step is no more than EpsX.
                *  4   scaled gradient norm is no more than EpsG.
                *  5   MaxIts steps was taken
                More information about fields of this  structure  can  be
                found in the comments on MinNLCReport datatype.
   
  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcresults(minnlcstate* state,
     /* Real    */ ae_vector* x,
     minnlcreport* rep,
     ae_state *_state)
{

    ae_vector_clear(x);
    _minnlcreport_clear(rep);

    minnlcresultsbuf(state, x, rep, _state);
}


/*************************************************************************
NLC results

Buffered implementation of MinNLCResults() which uses pre-allocated buffer
to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
intended to be used in the inner cycles of performance critical algorithms
where array reallocation penalty is too large to be ignored.

  -- ALGLIB --
     Copyright 28.11.2010 by Bochkanov Sergey
*************************************************************************/
void minnlcresultsbuf(minnlcstate* state,
     /* Real    */ ae_vector* x,
     minnlcreport* rep,
     ae_state *_state)
{
    ae_int_t i;


    if( x->cnt<state->n )
    {
        ae_vector_set_length(x, state->n, _state);
    }
    rep->iterationscount = state->repinneriterationscount;
    rep->nfev = state->repnfev;
    rep->varidx = state->repvaridx;
    rep->funcidx = state->repfuncidx;
    rep->terminationtype = state->repterminationtype;
    rep->dbgphase0its = state->repdbgphase0its;
    if( state->repterminationtype>0 )
    {
        ae_v_move(&x->ptr.p_double[0], 1, &state->xc.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    }
    else
    {
        for(i=0; i<=state->n-1; i++)
        {
            x->ptr.p_double[i] = _state->v_nan;
        }
    }
}


/*************************************************************************
This subroutine restarts algorithm from new point.
All optimization parameters (including constraints) are left unchanged.

This  function  allows  to  solve multiple  optimization  problems  (which
must have  same number of dimensions) without object reallocation penalty.

INPUT PARAMETERS:
    State   -   structure previously allocated with MinNLCCreate call.
    X       -   new starting point.

  -- ALGLIB --
     Copyright 28.11.2010 by Bochkanov Sergey
*************************************************************************/
void minnlcrestartfrom(minnlcstate* state,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t n;


    n = state->n;
    
    /*
     * First, check for errors in the inputs
     */
    ae_assert(x->cnt>=n, "MinNLCRestartFrom: Length(X)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "MinNLCRestartFrom: X contains infinite or NaN values!", _state);
    
    /*
     * Set XC
     */
    ae_v_move(&state->xstart.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
    
    /*
     * prepare RComm facilities
     */
    ae_vector_set_length(&state->rstate.ia, 4+1, _state);
    state->rstate.stage = -1;
    minnlc_clearrequestfields(state, _state);
}


/*************************************************************************
This  subroutine  turns  on  verification  of  the  user-supplied analytic
gradient:
* user calls this subroutine before optimization begins
* MinNLCOptimize() is called
* prior to  actual  optimization, for each component  of  parameters being
  optimized X[i] algorithm performs following steps:
  * two trial steps are made to X[i]-TestStep*S[i] and X[i]+TestStep*S[i],
    where X[i] is i-th component of the initial point and S[i] is a  scale
    of i-th parameter
  * F(X) is evaluated at these trial points
  * we perform one more evaluation in the middle point of the interval
  * we  build  cubic  model using function values and derivatives at trial
    points and we compare its prediction with actual value in  the  middle
    point
  * in case difference between prediction and actual value is higher  than
    some predetermined threshold, algorithm stops with completion code -7;
    Rep.VarIdx is set to index of the parameter with incorrect derivative,
    and Rep.FuncIdx is set to index of the function.
* after verification is over, algorithm proceeds to the actual optimization.

NOTE 1: verification  needs  N (parameters count) gradient evaluations. It
        is very costly and you should use  it  only  for  low  dimensional
        problems,  when  you  want  to  be  sure  that  you've   correctly
        calculated  analytic  derivatives.  You  should  not use it in the
        production code (unless you want to check derivatives provided  by
        some third party).

NOTE 2: you  should  carefully  choose  TestStep. Value which is too large
        (so large that function behaviour is significantly non-cubic) will
        lead to false alarms. You may use  different  step  for  different
        parameters by means of setting scale with MinNLCSetScale().

NOTE 3: this function may lead to false positives. In case it reports that
        I-th  derivative was calculated incorrectly, you may decrease test
        step  and  try  one  more  time  - maybe your function changes too
        sharply  and  your  step  is  too  large for such rapidly chanding
        function.

INPUT PARAMETERS:
    State       -   structure used to store algorithm state
    TestStep    -   verification step:
                    * TestStep=0 turns verification off
                    * TestStep>0 activates verification

  -- ALGLIB --
     Copyright 15.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcsetgradientcheck(minnlcstate* state,
     double teststep,
     ae_state *_state)
{


    ae_assert(ae_isfinite(teststep, _state), "MinNLCSetGradientCheck: TestStep contains NaN or Infinite", _state);
    ae_assert(ae_fp_greater_eq(teststep,(double)(0)), "MinNLCSetGradientCheck: invalid argument TestStep(TestStep<0)", _state);
    state->teststep = teststep;
}


/*************************************************************************
Penalty function for equality constraints.
INPUT PARAMETERS:
    Alpha   -   function argument. Penalty function becomes large when
                Alpha approaches -1 or +1. It is defined for Alpha<=-1 or
                Alpha>=+1 - in this case infinite value is returned.
                
OUTPUT PARAMETERS:
    F       -   depending on Alpha:
                * for Alpha in (-1+eps,+1-eps), F=F(Alpha)
                * for Alpha outside of interval, F is some very large number
    DF      -   depending on Alpha:
                * for Alpha in (-1+eps,+1-eps), DF=dF(Alpha)/dAlpha, exact
                  numerical derivative.
                * otherwise, it is zero
    D2F     -   second derivative

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcequalitypenaltyfunction(double alpha,
     double* f,
     double* df,
     double* d2f,
     ae_state *_state)
{

    *f = 0;
    *df = 0;
    *d2f = 0;

    *f = 0.5*alpha*alpha;
    *df = alpha;
    *d2f = 1.0;
}


/*************************************************************************
"Penalty" function  for  inequality  constraints,  which  is multiplied by
penalty coefficient Rho.

"Penalty" function plays only supplementary role - it helps  to  stabilize
algorithm when solving non-convex problems. Because it  is  multiplied  by
fixed and large  Rho  -  not  Lagrange  multiplier  Nu  which  may  become
arbitrarily small! - it enforces  convexity  of  the  problem  behind  the
boundary of the feasible area.

This function is zero at the feasible area and in the close  neighborhood,
it becomes non-zero only at some distance (scaling is essential!) and grows
quadratically.

Penalty function must enter augmented Lagrangian as
    Rho*PENALTY(x-lowerbound)
with corresponding changes being made for upper bound or  other  kinds  of
constraints.

INPUT PARAMETERS:
    Alpha   -   function argument. Typically, if we have active constraint
                with precise Lagrange multiplier, we have Alpha  around 1.
                Large positive Alpha's correspond to  inner  area  of  the
                feasible set. Alpha<1 corresponds to  outer  area  of  the
                feasible set.
    StabilizingPoint- point where F becomes  non-zero.  Must  be  negative
                value, at least -1, large values (hundreds) are possible.
                
OUTPUT PARAMETERS:
    F       -   F(Alpha)
    DF      -   DF=dF(Alpha)/dAlpha, exact derivative
    D2F     -   second derivative
    
NOTE: it is improtant to  have  significantly  non-zero  StabilizingPoint,
      because when it  is  large,  shift  term  does  not  interfere  with
      Lagrange  multipliers  converging  to  their  final  values.   Thus,
      convergence of such modified AUL algorithm is  still  guaranteed  by
      same set of theorems.

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcinequalitypenaltyfunction(double alpha,
     double stabilizingpoint,
     double* f,
     double* df,
     double* d2f,
     ae_state *_state)
{

    *f = 0;
    *df = 0;
    *d2f = 0;

    if( ae_fp_greater_eq(alpha,stabilizingpoint) )
    {
        *f = 0.0;
        *df = 0.0;
        *d2f = 0.0;
    }
    else
    {
        alpha = alpha-stabilizingpoint;
        *f = 0.5*alpha*alpha;
        *df = alpha;
        *d2f = 1.0;
    }
}


/*************************************************************************
"Shift" function  for  inequality  constraints,  which  is  multiplied  by
corresponding Lagrange multiplier.

"Shift" function is a main factor which enforces  inequality  constraints.
Inequality penalty function plays only supplementary role  -  it  prevents
accidental step deep into infeasible area  when  working  with  non-convex
problems (read comments on corresponding function for more information).

Shift function must enter augmented Lagrangian as
    Nu/Rho*SHIFT((x-lowerbound)*Rho+1)
with corresponding changes being made for upper bound or  other  kinds  of
constraints.

INPUT PARAMETERS:
    Alpha   -   function argument. Typically, if we have active constraint
                with precise Lagrange multiplier, we have Alpha  around 1.
                Large positive Alpha's correspond to  inner  area  of  the
                feasible set. Alpha<1 corresponds to  outer  area  of  the
                feasible set.
                
OUTPUT PARAMETERS:
    F       -   F(Alpha)
    DF      -   DF=dF(Alpha)/dAlpha, exact derivative
    D2F     -   second derivative

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
void minnlcinequalityshiftfunction(double alpha,
     double* f,
     double* df,
     double* d2f,
     ae_state *_state)
{

    *f = 0;
    *df = 0;
    *d2f = 0;

    if( ae_fp_greater_eq(alpha,0.5) )
    {
        *f = -ae_log(alpha, _state);
        *df = -1/alpha;
        *d2f = 1/(alpha*alpha);
    }
    else
    {
        *f = 2*alpha*alpha-4*alpha+(ae_log((double)(2), _state)+1.5);
        *df = 4*alpha-4;
        *d2f = (double)(4);
    }
}


/*************************************************************************
Clears request fileds (to be sure that we don't forget to clear something)
*************************************************************************/
static void minnlc_clearrequestfields(minnlcstate* state,
     ae_state *_state)
{


    state->needfi = ae_false;
    state->needfij = ae_false;
    state->xupdated = ae_false;
}


/*************************************************************************
Internal initialization subroutine.
Sets default NLC solver with default criteria.
*************************************************************************/
static void minnlc_minnlcinitinternal(ae_int_t n,
     /* Real    */ ae_vector* x,
     double diffstep,
     minnlcstate* state,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_matrix c;
    ae_vector ct;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);

    
    /*
     * Default params
     */
    state->stabilizingpoint = -100.0;
    state->initialinequalitymultiplier = 1.0;
    
    /*
     * Initialize other params
     */
    state->teststep = (double)(0);
    state->n = n;
    state->diffstep = diffstep;
    ae_vector_set_length(&state->bndl, n, _state);
    ae_vector_set_length(&state->hasbndl, n, _state);
    ae_vector_set_length(&state->bndu, n, _state);
    ae_vector_set_length(&state->hasbndu, n, _state);
    ae_vector_set_length(&state->s, n, _state);
    ae_vector_set_length(&state->xstart, n, _state);
    ae_vector_set_length(&state->xc, n, _state);
    ae_vector_set_length(&state->x, n, _state);
    for(i=0; i<=n-1; i++)
    {
        state->bndl.ptr.p_double[i] = _state->v_neginf;
        state->hasbndl.ptr.p_bool[i] = ae_false;
        state->bndu.ptr.p_double[i] = _state->v_posinf;
        state->hasbndu.ptr.p_bool[i] = ae_false;
        state->s.ptr.p_double[i] = 1.0;
        state->xstart.ptr.p_double[i] = x->ptr.p_double[i];
        state->xc.ptr.p_double[i] = x->ptr.p_double[i];
    }
    minnlcsetlc(state, &c, &ct, 0, _state);
    minnlcsetnlc(state, 0, 0, _state);
    minnlcsetcond(state, 0.0, 0.0, 0.0, 0, _state);
    minnlcsetxrep(state, ae_false, _state);
    minnlcsetalgoaul(state, 1.0E-3, 0, _state);
    minnlcsetprecinexact(state, _state);
    minlbfgscreate(n, ae_minint(minnlc_lbfgsfactor, n, _state), x, &state->auloptimizer, _state);
    minnlcrestartfrom(state, x, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This function clears preconditioner for L-BFGS optimizer (sets it do default
state);

Parameters:
    AULOptimizer    -   optimizer to tune
    
  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
static void minnlc_clearpreconditioner(minlbfgsstate* auloptimizer,
     ae_state *_state)
{


    minlbfgssetprecdefault(auloptimizer, _state);
}


/*************************************************************************
This function updates preconditioner for L-BFGS optimizer.

Parameters:
    PrecType        -   preconditioner type:
                        * 0 for unpreconditioned iterations
                        * 1 for inexact LBFGS
                        * 2 for exact preconditioner update after each UpdateFreq its
    UpdateFreq      -   update frequency
    PrecCounter     -   iterations counter, must be zero on the first call,
                        automatically increased  by  this  function.  This
                        counter is used to implement "update-once-in-X-iterations"
                        scheme.
    AULOptimizer    -   optimizer to tune
    X               -   current point
    Rho             -   penalty term
    GammaK          -   current  estimate  of  Hessian  norm   (used   for
                        initialization of preconditioner). Can be zero, in
                        which case Hessian is assumed to be unit.

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
static void minnlc_updatepreconditioner(ae_int_t prectype,
     ae_int_t updatefreq,
     ae_int_t* preccounter,
     minlbfgsstate* auloptimizer,
     /* Real    */ ae_vector* x,
     double rho,
     double gammak,
     /* Real    */ ae_vector* bndl,
     /* Boolean */ ae_vector* hasbndl,
     /* Real    */ ae_vector* bndu,
     /* Boolean */ ae_vector* hasbndu,
     /* Real    */ ae_vector* nubc,
     /* Real    */ ae_matrix* cleic,
     /* Real    */ ae_vector* nulc,
     /* Real    */ ae_vector* fi,
     /* Real    */ ae_matrix* jac,
     /* Real    */ ae_vector* nunlc,
     /* Real    */ ae_vector* bufd,
     /* Real    */ ae_vector* bufc,
     /* Real    */ ae_matrix* bufw,
     ae_int_t n,
     ae_int_t nec,
     ae_int_t nic,
     ae_int_t ng,
     ae_int_t nh,
     ae_state *_state)
{
    ae_int_t i;
    double v;
    double p;
    double dp;
    double d2p;


    ae_assert(ae_fp_greater(rho,(double)(0)), "MinNLC: integrity check failed", _state);
    rvectorsetlengthatleast(bufd, n, _state);
    rvectorsetlengthatleast(bufc, nec+nic+ng+nh, _state);
    rmatrixsetlengthatleast(bufw, nec+nic+ng+nh, n, _state);
    
    /*
     * Preconditioner before update from barrier/penalty functions
     */
    if( ae_fp_eq(gammak,(double)(0)) )
    {
        gammak = (double)(1);
    }
    for(i=0; i<=n-1; i++)
    {
        bufd->ptr.p_double[i] = gammak;
    }
    
    /*
     * Update diagonal Hessian using nonlinearity from boundary constraints:
     * * penalty term from equality constraints
     * * shift term from inequality constraints
     *
     * NOTE: penalty term for inequality constraints is ignored because it
     *       is large only in exceptional cases.
     */
    for(i=0; i<=n-1; i++)
    {
        if( (hasbndl->ptr.p_bool[i]&&hasbndu->ptr.p_bool[i])&&ae_fp_eq(bndl->ptr.p_double[i],bndu->ptr.p_double[i]) )
        {
            minnlcequalitypenaltyfunction((x->ptr.p_double[i]-bndl->ptr.p_double[i])*rho, &p, &dp, &d2p, _state);
            bufd->ptr.p_double[i] = bufd->ptr.p_double[i]+d2p*rho;
            continue;
        }
        if( hasbndl->ptr.p_bool[i] )
        {
            minnlcinequalityshiftfunction((x->ptr.p_double[i]-bndl->ptr.p_double[i])*rho+1, &p, &dp, &d2p, _state);
            bufd->ptr.p_double[i] = bufd->ptr.p_double[i]+nubc->ptr.p_double[2*i+0]*d2p*rho;
        }
        if( hasbndu->ptr.p_bool[i] )
        {
            minnlcinequalityshiftfunction((bndu->ptr.p_double[i]-x->ptr.p_double[i])*rho+1, &p, &dp, &d2p, _state);
            bufd->ptr.p_double[i] = bufd->ptr.p_double[i]+nubc->ptr.p_double[2*i+1]*d2p*rho;
        }
    }
    
    /*
     * Process linear constraints
     */
    for(i=0; i<=nec+nic-1; i++)
    {
        ae_v_move(&bufw->ptr.pp_double[i][0], 1, &cleic->ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
        v = ae_v_dotproduct(&cleic->ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
        v = v-cleic->ptr.pp_double[i][n];
        if( i<nec )
        {
            
            /*
             * Equality constraint
             */
            minnlcequalitypenaltyfunction(v*rho, &p, &dp, &d2p, _state);
            bufc->ptr.p_double[i] = d2p*rho;
        }
        else
        {
            
            /*
             * Inequality constraint
             */
            minnlcinequalityshiftfunction(-v*rho+1, &p, &dp, &d2p, _state);
            bufc->ptr.p_double[i] = nulc->ptr.p_double[i]*d2p*rho;
        }
    }
    
    /*
     * Process nonlinear constraints
     */
    for(i=0; i<=ng+nh-1; i++)
    {
        ae_v_move(&bufw->ptr.pp_double[nec+nic+i][0], 1, &jac->ptr.pp_double[1+i][0], 1, ae_v_len(0,n-1));
        v = fi->ptr.p_double[1+i];
        if( i<ng )
        {
            
            /*
             * Equality constraint
             */
            minnlcequalitypenaltyfunction(v*rho, &p, &dp, &d2p, _state);
            bufc->ptr.p_double[nec+nic+i] = d2p*rho;
        }
        else
        {
            
            /*
             * Inequality constraint
             */
            minnlcinequalityshiftfunction(-v*rho+1, &p, &dp, &d2p, _state);
            bufc->ptr.p_double[nec+nic+i] = nunlc->ptr.p_double[i]*d2p*rho;
        }
    }
    if( prectype==1 )
    {
        minlbfgssetprecrankklbfgsfast(auloptimizer, bufd, bufc, bufw, nec+nic+ng+nh, _state);
    }
    if( prectype==2&&*preccounter%updatefreq==0 )
    {
        minlbfgssetpreclowrankexact(auloptimizer, bufd, bufc, bufw, nec+nic+ng+nh, _state);
    }
    inc(preccounter, _state);
}


/*************************************************************************
This subroutine adds penalty from boundary constraints to target  function
and its gradient. Penalty function is one which is used for main AUL cycle
- with Lagrange multipliers and infinite at the barrier and beyond.

Parameters:
    X[] - current point
    BndL[], BndU[] - boundary constraints
    HasBndL[], HasBndU[] - I-th element is True if corresponding constraint is present
    NuBC[] - Lagrange multipliers corresponding to constraints
    Rho - penalty term
    StabilizingPoint - branch point for inequality stabilizing term
    F - function value to modify
    G - gradient to modify

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
static void minnlc_penaltybc(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* bndl,
     /* Boolean */ ae_vector* hasbndl,
     /* Real    */ ae_vector* bndu,
     /* Boolean */ ae_vector* hasbndu,
     /* Real    */ ae_vector* nubc,
     ae_int_t n,
     double rho,
     double stabilizingpoint,
     double* f,
     /* Real    */ ae_vector* g,
     ae_state *_state)
{
    ae_int_t i;
    double p;
    double dp;
    double d2p;


    for(i=0; i<=n-1; i++)
    {
        if( (hasbndl->ptr.p_bool[i]&&hasbndu->ptr.p_bool[i])&&ae_fp_eq(bndl->ptr.p_double[i],bndu->ptr.p_double[i]) )
        {
            
            /*
             * I-th boundary constraint is of equality-type
             */
            minnlcequalitypenaltyfunction((x->ptr.p_double[i]-bndl->ptr.p_double[i])*rho, &p, &dp, &d2p, _state);
            *f = *f+p/rho-nubc->ptr.p_double[2*i+0]*(x->ptr.p_double[i]-bndl->ptr.p_double[i]);
            g->ptr.p_double[i] = g->ptr.p_double[i]+dp-nubc->ptr.p_double[2*i+0];
            continue;
        }
        if( hasbndl->ptr.p_bool[i] )
        {
            
            /*
             * Handle lower bound
             */
            minnlcinequalitypenaltyfunction(x->ptr.p_double[i]-bndl->ptr.p_double[i], stabilizingpoint, &p, &dp, &d2p, _state);
            *f = *f+rho*p;
            g->ptr.p_double[i] = g->ptr.p_double[i]+rho*dp;
            minnlcinequalityshiftfunction((x->ptr.p_double[i]-bndl->ptr.p_double[i])*rho+1, &p, &dp, &d2p, _state);
            *f = *f+p/rho*nubc->ptr.p_double[2*i+0];
            g->ptr.p_double[i] = g->ptr.p_double[i]+dp*nubc->ptr.p_double[2*i+0];
        }
        if( hasbndu->ptr.p_bool[i] )
        {
            
            /*
             * Handle upper bound
             */
            minnlcinequalitypenaltyfunction(bndu->ptr.p_double[i]-x->ptr.p_double[i], stabilizingpoint, &p, &dp, &d2p, _state);
            *f = *f+rho*p;
            g->ptr.p_double[i] = g->ptr.p_double[i]-rho*dp;
            minnlcinequalityshiftfunction((bndu->ptr.p_double[i]-x->ptr.p_double[i])*rho+1, &p, &dp, &d2p, _state);
            *f = *f+p/rho*nubc->ptr.p_double[2*i+1];
            g->ptr.p_double[i] = g->ptr.p_double[i]-dp*nubc->ptr.p_double[2*i+1];
        }
    }
}


/*************************************************************************
This subroutine adds penalty from  linear  constraints to target  function
and its gradient. Penalty function is one which is used for main AUL cycle
- with Lagrange multipliers and infinite at the barrier and beyond.

Parameters:
    X[] - current point
    CLEIC[] -   constraints matrix, first NEC rows are equality ones, next
                NIC rows are inequality ones. array[NEC+NIC,N+1]
    NuLC[]  -   Lagrange multipliers corresponding to constraints,
                array[NEC+NIC]
    N       -   dimensionalty
    NEC     -   number of equality constraints
    NIC     -   number of inequality constraints.
    Rho - penalty term
    StabilizingPoint - branch point for inequality stabilizing term
    F - function value to modify
    G - gradient to modify

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
static void minnlc_penaltylc(/* Real    */ ae_vector* x,
     /* Real    */ ae_matrix* cleic,
     /* Real    */ ae_vector* nulc,
     ae_int_t n,
     ae_int_t nec,
     ae_int_t nic,
     double rho,
     double stabilizingpoint,
     double* f,
     /* Real    */ ae_vector* g,
     ae_state *_state)
{
    ae_int_t i;
    double v;
    double vv;
    double p;
    double dp;
    double d2p;


    for(i=0; i<=nec+nic-1; i++)
    {
        v = ae_v_dotproduct(&cleic->ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
        v = v-cleic->ptr.pp_double[i][n];
        if( i<nec )
        {
            
            /*
             * Equality constraint
             */
            minnlcequalitypenaltyfunction(v*rho, &p, &dp, &d2p, _state);
            *f = *f+p/rho;
            vv = dp;
            ae_v_addd(&g->ptr.p_double[0], 1, &cleic->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), vv);
            *f = *f-nulc->ptr.p_double[i]*v;
            vv = nulc->ptr.p_double[i];
            ae_v_subd(&g->ptr.p_double[0], 1, &cleic->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), vv);
        }
        else
        {
            
            /*
             * Inequality constraint
             */
            minnlcinequalitypenaltyfunction(-v, stabilizingpoint, &p, &dp, &d2p, _state);
            *f = *f+p*rho;
            vv = dp*rho;
            ae_v_subd(&g->ptr.p_double[0], 1, &cleic->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), vv);
            minnlcinequalityshiftfunction(-v*rho+1, &p, &dp, &d2p, _state);
            *f = *f+p/rho*nulc->ptr.p_double[i];
            vv = dp*nulc->ptr.p_double[i];
            ae_v_subd(&g->ptr.p_double[0], 1, &cleic->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), vv);
        }
    }
}


/*************************************************************************
This subroutine adds penalty from nonlinear constraints to target function
and its gradient. Penalty function is one which is used for main AUL cycle
- with Lagrange multipliers and infinite at the barrier and beyond.

Parameters:
    Fi[] - function vector:
          * 1 component for function being minimized
          * NG components for equality constraints G_i(x)=0
          * NH components for inequality constraints H_i(x)<=0
    J[]  - Jacobian matrix, array[1+NG+NH,N]
    NuNLC[]  -   Lagrange multipliers corresponding to constraints,
                array[NG+NH]
    N - number of dimensions
    NG - number of equality constraints
    NH - number of inequality constraints
    Rho - penalty term
    StabilizingPoint - branch point for inequality stabilizing term
    F - function value to modify
    G - gradient to modify

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
static void minnlc_penaltynlc(/* Real    */ ae_vector* fi,
     /* Real    */ ae_matrix* j,
     /* Real    */ ae_vector* nunlc,
     ae_int_t n,
     ae_int_t ng,
     ae_int_t nh,
     double rho,
     double stabilizingpoint,
     double* f,
     /* Real    */ ae_vector* g,
     ae_state *_state)
{
    ae_int_t i;
    double v;
    double vv;
    double p;
    double dp;
    double d2p;


    
    /*
     * IMPORTANT: loop starts from 1, not zero!
     */
    for(i=1; i<=ng+nh; i++)
    {
        v = fi->ptr.p_double[i];
        if( i<=ng )
        {
            
            /*
             * Equality constraint
             */
            minnlcequalitypenaltyfunction(v*rho, &p, &dp, &d2p, _state);
            *f = *f+p/rho;
            vv = dp;
            ae_v_addd(&g->ptr.p_double[0], 1, &j->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), vv);
            *f = *f-nunlc->ptr.p_double[i-1]*v;
            vv = nunlc->ptr.p_double[i-1];
            ae_v_subd(&g->ptr.p_double[0], 1, &j->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), vv);
        }
        else
        {
            
            /*
             * Inequality constraint
             */
            minnlcinequalitypenaltyfunction(-v, stabilizingpoint, &p, &dp, &d2p, _state);
            *f = *f+p*rho;
            vv = dp*rho;
            ae_v_subd(&g->ptr.p_double[0], 1, &j->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), vv);
            minnlcinequalityshiftfunction(-v*rho+1, &p, &dp, &d2p, _state);
            *f = *f+p/rho*nunlc->ptr.p_double[i-1];
            vv = dp*nunlc->ptr.p_double[i-1];
            ae_v_subd(&g->ptr.p_double[0], 1, &j->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), vv);
        }
    }
}


/*************************************************************************
This function performs actual processing for AUL algorith. It expects that
caller redirects its reverse communication  requests  NeedFiJ/XUpdated  to
external user who will provide analytic derivative (or handle reports about
progress).

In case external user does not have analytic derivative, it is responsibility
of caller to intercept NeedFiJ request and  replace  it  with  appropriate
numerical differentiation scheme.

  -- ALGLIB --
     Copyright 06.06.2014 by Bochkanov Sergey
*************************************************************************/
static ae_bool minnlc_auliteration(minnlcstate* state, ae_state *_state)
{
    ae_int_t n;
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t ng;
    ae_int_t nh;
    ae_int_t i;
    ae_int_t j;
    ae_int_t outerit;
    ae_int_t preccounter;
    double v;
    double vv;
    double p;
    double dp;
    double d2p;
    double v0;
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
    if( state->rstateaul.stage>=0 )
    {
        n = state->rstateaul.ia.ptr.p_int[0];
        nec = state->rstateaul.ia.ptr.p_int[1];
        nic = state->rstateaul.ia.ptr.p_int[2];
        ng = state->rstateaul.ia.ptr.p_int[3];
        nh = state->rstateaul.ia.ptr.p_int[4];
        i = state->rstateaul.ia.ptr.p_int[5];
        j = state->rstateaul.ia.ptr.p_int[6];
        outerit = state->rstateaul.ia.ptr.p_int[7];
        preccounter = state->rstateaul.ia.ptr.p_int[8];
        v = state->rstateaul.ra.ptr.p_double[0];
        vv = state->rstateaul.ra.ptr.p_double[1];
        p = state->rstateaul.ra.ptr.p_double[2];
        dp = state->rstateaul.ra.ptr.p_double[3];
        d2p = state->rstateaul.ra.ptr.p_double[4];
        v0 = state->rstateaul.ra.ptr.p_double[5];
        v1 = state->rstateaul.ra.ptr.p_double[6];
        v2 = state->rstateaul.ra.ptr.p_double[7];
    }
    else
    {
        n = 364;
        nec = 214;
        nic = -338;
        ng = -686;
        nh = 912;
        i = 585;
        j = 497;
        outerit = -271;
        preccounter = -581;
        v = 745;
        vv = -533;
        p = -77;
        dp = 678;
        d2p = -293;
        v0 = 316;
        v1 = 647;
        v2 = -756;
    }
    if( state->rstateaul.stage==0 )
    {
        goto lbl_0;
    }
    if( state->rstateaul.stage==1 )
    {
        goto lbl_1;
    }
    if( state->rstateaul.stage==2 )
    {
        goto lbl_2;
    }
    
    /*
     * Routine body
     */
    ae_assert(state->solvertype==0, "MinNLC: internal error", _state);
    n = state->n;
    nec = state->nec;
    nic = state->nic;
    ng = state->ng;
    nh = state->nh;
    
    /*
     * Prepare scaled problem
     */
    rvectorsetlengthatleast(&state->scaledbndl, n, _state);
    rvectorsetlengthatleast(&state->scaledbndu, n, _state);
    rmatrixsetlengthatleast(&state->scaledcleic, nec+nic, n+1, _state);
    for(i=0; i<=n-1; i++)
    {
        if( state->hasbndl.ptr.p_bool[i] )
        {
            state->scaledbndl.ptr.p_double[i] = state->bndl.ptr.p_double[i]/state->s.ptr.p_double[i];
        }
        if( state->hasbndu.ptr.p_bool[i] )
        {
            state->scaledbndu.ptr.p_double[i] = state->bndu.ptr.p_double[i]/state->s.ptr.p_double[i];
        }
        state->xc.ptr.p_double[i] = state->xstart.ptr.p_double[i]/state->s.ptr.p_double[i];
    }
    for(i=0; i<=nec+nic-1; i++)
    {
        
        /*
         * Scale and normalize linear constraints
         */
        vv = 0.0;
        for(j=0; j<=n-1; j++)
        {
            v = state->cleic.ptr.pp_double[i][j]*state->s.ptr.p_double[j];
            state->scaledcleic.ptr.pp_double[i][j] = v;
            vv = vv+v*v;
        }
        vv = ae_sqrt(vv, _state);
        state->scaledcleic.ptr.pp_double[i][n] = state->cleic.ptr.pp_double[i][n];
        if( ae_fp_greater(vv,(double)(0)) )
        {
            for(j=0; j<=n; j++)
            {
                state->scaledcleic.ptr.pp_double[i][j] = state->scaledcleic.ptr.pp_double[i][j]/vv;
            }
        }
    }
    
    /*
     * Prepare stopping criteria
     */
    minlbfgssetcond(&state->auloptimizer, state->epsg, state->epsf, state->epsx, state->maxits, _state);
    
    /*
     * Main AUL cycle:
     * * prepare Lagrange multipliers NuNB/NuLC
     * * set GammaK (current estimate of Hessian norm) to 0.0 and XKPresent to False
     */
    rvectorsetlengthatleast(&state->nubc, 2*n, _state);
    rvectorsetlengthatleast(&state->nulc, nec+nic, _state);
    rvectorsetlengthatleast(&state->nunlc, ng+nh, _state);
    rvectorsetlengthatleast(&state->xk, n, _state);
    rvectorsetlengthatleast(&state->gk, n, _state);
    rvectorsetlengthatleast(&state->xk1, n, _state);
    rvectorsetlengthatleast(&state->gk1, n, _state);
    for(i=0; i<=n-1; i++)
    {
        state->nubc.ptr.p_double[2*i+0] = 0.0;
        state->nubc.ptr.p_double[2*i+1] = 0.0;
        if( (state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i])&&ae_fp_eq(state->bndl.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
        {
            continue;
        }
        if( state->hasbndl.ptr.p_bool[i] )
        {
            state->nubc.ptr.p_double[2*i+0] = state->initialinequalitymultiplier;
        }
        if( state->hasbndu.ptr.p_bool[i] )
        {
            state->nubc.ptr.p_double[2*i+1] = state->initialinequalitymultiplier;
        }
    }
    for(i=0; i<=nec-1; i++)
    {
        state->nulc.ptr.p_double[i] = 0.0;
    }
    for(i=0; i<=nic-1; i++)
    {
        state->nulc.ptr.p_double[nec+i] = state->initialinequalitymultiplier;
    }
    for(i=0; i<=ng-1; i++)
    {
        state->nunlc.ptr.p_double[i] = 0.0;
    }
    for(i=0; i<=nh-1; i++)
    {
        state->nunlc.ptr.p_double[ng+i] = state->initialinequalitymultiplier;
    }
    state->gammak = 0.0;
    state->xkpresent = ae_false;
    ae_assert(state->aulitscnt>0, "MinNLC: integrity check failed", _state);
    minnlc_clearpreconditioner(&state->auloptimizer, _state);
    outerit = 0;
lbl_3:
    if( outerit>state->aulitscnt-1 )
    {
        goto lbl_5;
    }
    
    /*
     * Optimize with current Lagrange multipliers
     *
     * NOTE: this code expects and checks that line search ends in the
     *       point which is used as beginning for the next search. Such
     *       guarantee is given by MCSRCH function.  L-BFGS  optimizer
     *       does not formally guarantee it, but it follows same rule.
     *       Below we a) rely on such property of the optimizer, and b)
     *       assert that it is true, in order to fail loudly if it is
     *       not true.
     *
     * NOTE: security check for NAN/INF in F/G is responsibility of
     *       LBFGS optimizer. AUL optimizer checks for NAN/INF only
     *       when we update Lagrange multipliers.
     */
    preccounter = 0;
    minlbfgssetxrep(&state->auloptimizer, ae_true, _state);
    minlbfgsrestartfrom(&state->auloptimizer, &state->xc, _state);
lbl_6:
    if( !minlbfgsiteration(&state->auloptimizer, _state) )
    {
        goto lbl_7;
    }
    if( !state->auloptimizer.needfg )
    {
        goto lbl_8;
    }
    
    /*
     * Un-scale X, evaluate F/G/H, re-scale Jacobian
     */
    for(i=0; i<=n-1; i++)
    {
        state->x.ptr.p_double[i] = state->auloptimizer.x.ptr.p_double[i]*state->s.ptr.p_double[i];
    }
    state->needfij = ae_true;
    state->rstateaul.stage = 0;
    goto lbl_rcomm;
lbl_0:
    state->needfij = ae_false;
    for(i=0; i<=ng+nh; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            state->j.ptr.pp_double[i][j] = state->j.ptr.pp_double[i][j]*state->s.ptr.p_double[j];
        }
    }
    
    /*
     * Store data for estimation of Hessian norm:
     * * current point (re-scaled)
     * * gradient of the target function (re-scaled, unmodified)
     */
    ae_v_move(&state->xk1.ptr.p_double[0], 1, &state->auloptimizer.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&state->gk1.ptr.p_double[0], 1, &state->j.ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
    
    /*
     * Function being optimized
     */
    state->auloptimizer.f = state->fi.ptr.p_double[0];
    for(i=0; i<=n-1; i++)
    {
        state->auloptimizer.g.ptr.p_double[i] = state->j.ptr.pp_double[0][i];
    }
    
    /*
     * Penalty for violation of boundary/linear/nonlinear constraints
     */
    minnlc_penaltybc(&state->auloptimizer.x, &state->scaledbndl, &state->hasbndl, &state->scaledbndu, &state->hasbndu, &state->nubc, n, state->rho, state->stabilizingpoint, &state->auloptimizer.f, &state->auloptimizer.g, _state);
    minnlc_penaltylc(&state->auloptimizer.x, &state->scaledcleic, &state->nulc, n, nec, nic, state->rho, state->stabilizingpoint, &state->auloptimizer.f, &state->auloptimizer.g, _state);
    minnlc_penaltynlc(&state->fi, &state->j, &state->nunlc, n, ng, nh, state->rho, state->stabilizingpoint, &state->auloptimizer.f, &state->auloptimizer.g, _state);
    
    /*
     * To optimizer
     */
    goto lbl_6;
lbl_8:
    if( !state->auloptimizer.xupdated )
    {
        goto lbl_10;
    }
    
    /*
     * Report current point (if needed)
     */
    if( !state->xrep )
    {
        goto lbl_12;
    }
    for(i=0; i<=n-1; i++)
    {
        state->x.ptr.p_double[i] = state->auloptimizer.x.ptr.p_double[i]*state->s.ptr.p_double[i];
    }
    state->f = state->auloptimizer.f;
    state->xupdated = ae_true;
    state->rstateaul.stage = 1;
    goto lbl_rcomm;
lbl_1:
    state->xupdated = ae_false;
lbl_12:
    
    /*
     * Update GammaK
     */
    if( state->xkpresent )
    {
        
        /*
         * XK/GK store beginning of current line search, and XK1/GK1
         * store data for the end of the line search:
         * * first, we Assert() that XK1 (last point where function
         *   was evaluated) is same as AULOptimizer.X (what is
         *   reported by RComm interface
         * * calculate step length V2.
         *
         * If V2>HessEstTol, then:
         * * calculate V0 - directional derivative at XK,
         *   and V1 - directional derivative at XK1
         * * set GammaK to Max(GammaK, |V1-V0|/V2)
         */
        for(i=0; i<=n-1; i++)
        {
            ae_assert(ae_fp_less_eq(ae_fabs(state->auloptimizer.x.ptr.p_double[i]-state->xk1.ptr.p_double[i], _state),100*ae_machineepsilon), "MinNLC: integrity check failed, unexpected behavior of LBFGS optimizer", _state);
        }
        v2 = 0.0;
        for(i=0; i<=n-1; i++)
        {
            v2 = v2+ae_sqr(state->xk.ptr.p_double[i]-state->xk1.ptr.p_double[i], _state);
        }
        v2 = ae_sqrt(v2, _state);
        if( ae_fp_greater(v2,minnlc_hessesttol) )
        {
            v0 = 0.0;
            v1 = 0.0;
            for(i=0; i<=n-1; i++)
            {
                v = (state->xk.ptr.p_double[i]-state->xk1.ptr.p_double[i])/v2;
                v0 = v0+state->gk.ptr.p_double[i]*v;
                v1 = v1+state->gk1.ptr.p_double[i]*v;
            }
            state->gammak = ae_maxreal(state->gammak, ae_fabs(v1-v0, _state)/v2, _state);
        }
    }
    else
    {
        
        /*
         * Beginning of the first line search, XK is not yet initialized.
         */
        ae_v_move(&state->xk.ptr.p_double[0], 1, &state->xk1.ptr.p_double[0], 1, ae_v_len(0,n-1));
        ae_v_move(&state->gk.ptr.p_double[0], 1, &state->gk1.ptr.p_double[0], 1, ae_v_len(0,n-1));
        state->xkpresent = ae_true;
    }
    
    /*
     * Update preconsitioner using current GammaK
     */
    minnlc_updatepreconditioner(state->prectype, state->updatefreq, &preccounter, &state->auloptimizer, &state->auloptimizer.x, state->rho, state->gammak, &state->scaledbndl, &state->hasbndl, &state->scaledbndu, &state->hasbndu, &state->nubc, &state->scaledcleic, &state->nulc, &state->fi, &state->j, &state->nunlc, &state->bufd, &state->bufc, &state->bufw, n, nec, nic, ng, nh, _state);
    goto lbl_6;
lbl_10:
    ae_assert(ae_false, "MinNLC: integrity check failed", _state);
    goto lbl_6;
lbl_7:
    minlbfgsresultsbuf(&state->auloptimizer, &state->xc, &state->aulreport, _state);
    state->repinneriterationscount = state->repinneriterationscount+state->aulreport.iterationscount;
    state->repnfev = state->repnfev+state->aulreport.nfev;
    state->repterminationtype = state->aulreport.terminationtype;
    inc(&state->repouteriterationscount, _state);
    if( state->repterminationtype<=0 )
    {
        goto lbl_5;
    }
    
    /*
     * 1. Evaluate F/J
     * 2. Check for NAN/INF in F/J: we just calculate sum of their
     *    components, it should be enough to reduce vector/matrix to
     *    just one value which either "normal" (all summands were "normal")
     *    or NAN/INF (at least one summand was NAN/INF).
     * 3. Update Lagrange multipliers
     */
    for(i=0; i<=n-1; i++)
    {
        state->x.ptr.p_double[i] = state->xc.ptr.p_double[i]*state->s.ptr.p_double[i];
    }
    state->needfij = ae_true;
    state->rstateaul.stage = 2;
    goto lbl_rcomm;
lbl_2:
    state->needfij = ae_false;
    v = 0.0;
    for(i=0; i<=ng+nh; i++)
    {
        v = 0.1*v+state->fi.ptr.p_double[i];
        for(j=0; j<=n-1; j++)
        {
            v = 0.1*v+state->j.ptr.pp_double[i][j];
        }
    }
    if( !ae_isfinite(v, _state) )
    {
        
        /*
         * Abnormal termination - infinities in function/gradient
         */
        state->repterminationtype = -8;
        result = ae_false;
        return result;
    }
    for(i=0; i<=ng+nh; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            state->j.ptr.pp_double[i][j] = state->j.ptr.pp_double[i][j]*state->s.ptr.p_double[j];
        }
    }
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * Process coefficients corresponding to equality-type
         * constraints.
         */
        if( (state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i])&&ae_fp_eq(state->bndl.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
        {
            minnlcequalitypenaltyfunction((state->xc.ptr.p_double[i]-state->scaledbndl.ptr.p_double[i])*state->rho, &p, &dp, &d2p, _state);
            state->nubc.ptr.p_double[2*i+0] = state->nubc.ptr.p_double[2*i+0]-dp;
            continue;
        }
        
        /*
         * Process coefficients corresponding to inequality-type
         * constraints. These coefficients have limited growth/decay
         * per iteration which helps to stabilize algorithm.
         */
        ae_assert(ae_fp_greater(minnlc_aulmaxgrowth,1.0), "MinNLC: integrity error", _state);
        if( state->hasbndl.ptr.p_bool[i] )
        {
            minnlcinequalityshiftfunction((state->xc.ptr.p_double[i]-state->scaledbndl.ptr.p_double[i])*state->rho+1, &p, &dp, &d2p, _state);
            v = ae_fabs(dp, _state);
            v = ae_minreal(v, minnlc_aulmaxgrowth, _state);
            v = ae_maxreal(v, 1/minnlc_aulmaxgrowth, _state);
            state->nubc.ptr.p_double[2*i+0] = state->nubc.ptr.p_double[2*i+0]*v;
        }
        if( state->hasbndu.ptr.p_bool[i] )
        {
            minnlcinequalityshiftfunction((state->scaledbndu.ptr.p_double[i]-state->xc.ptr.p_double[i])*state->rho+1, &p, &dp, &d2p, _state);
            v = ae_fabs(dp, _state);
            v = ae_minreal(v, minnlc_aulmaxgrowth, _state);
            v = ae_maxreal(v, 1/minnlc_aulmaxgrowth, _state);
            state->nubc.ptr.p_double[2*i+1] = state->nubc.ptr.p_double[2*i+1]*v;
        }
    }
    for(i=0; i<=nec+nic-1; i++)
    {
        v = ae_v_dotproduct(&state->scaledcleic.ptr.pp_double[i][0], 1, &state->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
        v = v-state->scaledcleic.ptr.pp_double[i][n];
        if( i<nec )
        {
            minnlcequalitypenaltyfunction(v*state->rho, &p, &dp, &d2p, _state);
            state->nulc.ptr.p_double[i] = state->nulc.ptr.p_double[i]-dp;
        }
        else
        {
            minnlcinequalityshiftfunction(-v*state->rho+1, &p, &dp, &d2p, _state);
            v = ae_fabs(dp, _state);
            v = ae_minreal(v, minnlc_aulmaxgrowth, _state);
            v = ae_maxreal(v, 1/minnlc_aulmaxgrowth, _state);
            state->nulc.ptr.p_double[i] = state->nulc.ptr.p_double[i]*v;
        }
    }
    for(i=1; i<=ng+nh; i++)
    {
        
        /*
         * NOTE: loop index must start from 1, not zero!
         */
        v = state->fi.ptr.p_double[i];
        if( i<=ng )
        {
            minnlcequalitypenaltyfunction(v*state->rho, &p, &dp, &d2p, _state);
            state->nunlc.ptr.p_double[i-1] = state->nunlc.ptr.p_double[i-1]-dp;
        }
        else
        {
            minnlcinequalityshiftfunction(-v*state->rho+1, &p, &dp, &d2p, _state);
            v = ae_fabs(dp, _state);
            v = ae_minreal(v, minnlc_aulmaxgrowth, _state);
            v = ae_maxreal(v, 1/minnlc_aulmaxgrowth, _state);
            state->nunlc.ptr.p_double[i-1] = state->nunlc.ptr.p_double[i-1]*v;
        }
    }
    outerit = outerit+1;
    goto lbl_3;
lbl_5:
    for(i=0; i<=n-1; i++)
    {
        state->xc.ptr.p_double[i] = state->xc.ptr.p_double[i]*state->s.ptr.p_double[i];
    }
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstateaul.ia.ptr.p_int[0] = n;
    state->rstateaul.ia.ptr.p_int[1] = nec;
    state->rstateaul.ia.ptr.p_int[2] = nic;
    state->rstateaul.ia.ptr.p_int[3] = ng;
    state->rstateaul.ia.ptr.p_int[4] = nh;
    state->rstateaul.ia.ptr.p_int[5] = i;
    state->rstateaul.ia.ptr.p_int[6] = j;
    state->rstateaul.ia.ptr.p_int[7] = outerit;
    state->rstateaul.ia.ptr.p_int[8] = preccounter;
    state->rstateaul.ra.ptr.p_double[0] = v;
    state->rstateaul.ra.ptr.p_double[1] = vv;
    state->rstateaul.ra.ptr.p_double[2] = p;
    state->rstateaul.ra.ptr.p_double[3] = dp;
    state->rstateaul.ra.ptr.p_double[4] = d2p;
    state->rstateaul.ra.ptr.p_double[5] = v0;
    state->rstateaul.ra.ptr.p_double[6] = v1;
    state->rstateaul.ra.ptr.p_double[7] = v2;
    return result;
}


void _minnlcstate_init(void* _p, ae_state *_state)
{
    minnlcstate *p = (minnlcstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->s, 0, DT_REAL, _state);
    ae_vector_init(&p->bndl, 0, DT_REAL, _state);
    ae_vector_init(&p->bndu, 0, DT_REAL, _state);
    ae_vector_init(&p->hasbndl, 0, DT_BOOL, _state);
    ae_vector_init(&p->hasbndu, 0, DT_BOOL, _state);
    ae_matrix_init(&p->cleic, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->fi, 0, DT_REAL, _state);
    ae_matrix_init(&p->j, 0, 0, DT_REAL, _state);
    _rcommstate_init(&p->rstate, _state);
    _rcommstate_init(&p->rstateaul, _state);
    ae_vector_init(&p->scaledbndl, 0, DT_REAL, _state);
    ae_vector_init(&p->scaledbndu, 0, DT_REAL, _state);
    ae_matrix_init(&p->scaledcleic, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->xc, 0, DT_REAL, _state);
    ae_vector_init(&p->xstart, 0, DT_REAL, _state);
    ae_vector_init(&p->xbase, 0, DT_REAL, _state);
    ae_vector_init(&p->fbase, 0, DT_REAL, _state);
    ae_vector_init(&p->dfbase, 0, DT_REAL, _state);
    ae_vector_init(&p->fm2, 0, DT_REAL, _state);
    ae_vector_init(&p->fm1, 0, DT_REAL, _state);
    ae_vector_init(&p->fp1, 0, DT_REAL, _state);
    ae_vector_init(&p->fp2, 0, DT_REAL, _state);
    ae_vector_init(&p->dfm1, 0, DT_REAL, _state);
    ae_vector_init(&p->dfp1, 0, DT_REAL, _state);
    ae_vector_init(&p->bufd, 0, DT_REAL, _state);
    ae_vector_init(&p->bufc, 0, DT_REAL, _state);
    ae_matrix_init(&p->bufw, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->xk, 0, DT_REAL, _state);
    ae_vector_init(&p->xk1, 0, DT_REAL, _state);
    ae_vector_init(&p->gk, 0, DT_REAL, _state);
    ae_vector_init(&p->gk1, 0, DT_REAL, _state);
    _minlbfgsstate_init(&p->auloptimizer, _state);
    _minlbfgsreport_init(&p->aulreport, _state);
    ae_vector_init(&p->nubc, 0, DT_REAL, _state);
    ae_vector_init(&p->nulc, 0, DT_REAL, _state);
    ae_vector_init(&p->nunlc, 0, DT_REAL, _state);
}


void _minnlcstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    minnlcstate *dst = (minnlcstate*)_dst;
    minnlcstate *src = (minnlcstate*)_src;
    dst->stabilizingpoint = src->stabilizingpoint;
    dst->initialinequalitymultiplier = src->initialinequalitymultiplier;
    dst->solvertype = src->solvertype;
    dst->prectype = src->prectype;
    dst->updatefreq = src->updatefreq;
    dst->rho = src->rho;
    dst->n = src->n;
    dst->epsg = src->epsg;
    dst->epsf = src->epsf;
    dst->epsx = src->epsx;
    dst->maxits = src->maxits;
    dst->aulitscnt = src->aulitscnt;
    dst->xrep = src->xrep;
    dst->diffstep = src->diffstep;
    dst->teststep = src->teststep;
    ae_vector_init_copy(&dst->s, &src->s, _state);
    ae_vector_init_copy(&dst->bndl, &src->bndl, _state);
    ae_vector_init_copy(&dst->bndu, &src->bndu, _state);
    ae_vector_init_copy(&dst->hasbndl, &src->hasbndl, _state);
    ae_vector_init_copy(&dst->hasbndu, &src->hasbndu, _state);
    dst->nec = src->nec;
    dst->nic = src->nic;
    ae_matrix_init_copy(&dst->cleic, &src->cleic, _state);
    dst->ng = src->ng;
    dst->nh = src->nh;
    ae_vector_init_copy(&dst->x, &src->x, _state);
    dst->f = src->f;
    ae_vector_init_copy(&dst->fi, &src->fi, _state);
    ae_matrix_init_copy(&dst->j, &src->j, _state);
    dst->needfij = src->needfij;
    dst->needfi = src->needfi;
    dst->xupdated = src->xupdated;
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
    _rcommstate_init_copy(&dst->rstateaul, &src->rstateaul, _state);
    ae_vector_init_copy(&dst->scaledbndl, &src->scaledbndl, _state);
    ae_vector_init_copy(&dst->scaledbndu, &src->scaledbndu, _state);
    ae_matrix_init_copy(&dst->scaledcleic, &src->scaledcleic, _state);
    ae_vector_init_copy(&dst->xc, &src->xc, _state);
    ae_vector_init_copy(&dst->xstart, &src->xstart, _state);
    ae_vector_init_copy(&dst->xbase, &src->xbase, _state);
    ae_vector_init_copy(&dst->fbase, &src->fbase, _state);
    ae_vector_init_copy(&dst->dfbase, &src->dfbase, _state);
    ae_vector_init_copy(&dst->fm2, &src->fm2, _state);
    ae_vector_init_copy(&dst->fm1, &src->fm1, _state);
    ae_vector_init_copy(&dst->fp1, &src->fp1, _state);
    ae_vector_init_copy(&dst->fp2, &src->fp2, _state);
    ae_vector_init_copy(&dst->dfm1, &src->dfm1, _state);
    ae_vector_init_copy(&dst->dfp1, &src->dfp1, _state);
    ae_vector_init_copy(&dst->bufd, &src->bufd, _state);
    ae_vector_init_copy(&dst->bufc, &src->bufc, _state);
    ae_matrix_init_copy(&dst->bufw, &src->bufw, _state);
    ae_vector_init_copy(&dst->xk, &src->xk, _state);
    ae_vector_init_copy(&dst->xk1, &src->xk1, _state);
    ae_vector_init_copy(&dst->gk, &src->gk, _state);
    ae_vector_init_copy(&dst->gk1, &src->gk1, _state);
    dst->gammak = src->gammak;
    dst->xkpresent = src->xkpresent;
    _minlbfgsstate_init_copy(&dst->auloptimizer, &src->auloptimizer, _state);
    _minlbfgsreport_init_copy(&dst->aulreport, &src->aulreport, _state);
    ae_vector_init_copy(&dst->nubc, &src->nubc, _state);
    ae_vector_init_copy(&dst->nulc, &src->nulc, _state);
    ae_vector_init_copy(&dst->nunlc, &src->nunlc, _state);
    dst->repinneriterationscount = src->repinneriterationscount;
    dst->repouteriterationscount = src->repouteriterationscount;
    dst->repnfev = src->repnfev;
    dst->repvaridx = src->repvaridx;
    dst->repfuncidx = src->repfuncidx;
    dst->repterminationtype = src->repterminationtype;
    dst->repdbgphase0its = src->repdbgphase0its;
}


void _minnlcstate_clear(void* _p)
{
    minnlcstate *p = (minnlcstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->s);
    ae_vector_clear(&p->bndl);
    ae_vector_clear(&p->bndu);
    ae_vector_clear(&p->hasbndl);
    ae_vector_clear(&p->hasbndu);
    ae_matrix_clear(&p->cleic);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->fi);
    ae_matrix_clear(&p->j);
    _rcommstate_clear(&p->rstate);
    _rcommstate_clear(&p->rstateaul);
    ae_vector_clear(&p->scaledbndl);
    ae_vector_clear(&p->scaledbndu);
    ae_matrix_clear(&p->scaledcleic);
    ae_vector_clear(&p->xc);
    ae_vector_clear(&p->xstart);
    ae_vector_clear(&p->xbase);
    ae_vector_clear(&p->fbase);
    ae_vector_clear(&p->dfbase);
    ae_vector_clear(&p->fm2);
    ae_vector_clear(&p->fm1);
    ae_vector_clear(&p->fp1);
    ae_vector_clear(&p->fp2);
    ae_vector_clear(&p->dfm1);
    ae_vector_clear(&p->dfp1);
    ae_vector_clear(&p->bufd);
    ae_vector_clear(&p->bufc);
    ae_matrix_clear(&p->bufw);
    ae_vector_clear(&p->xk);
    ae_vector_clear(&p->xk1);
    ae_vector_clear(&p->gk);
    ae_vector_clear(&p->gk1);
    _minlbfgsstate_clear(&p->auloptimizer);
    _minlbfgsreport_clear(&p->aulreport);
    ae_vector_clear(&p->nubc);
    ae_vector_clear(&p->nulc);
    ae_vector_clear(&p->nunlc);
}


void _minnlcstate_destroy(void* _p)
{
    minnlcstate *p = (minnlcstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->s);
    ae_vector_destroy(&p->bndl);
    ae_vector_destroy(&p->bndu);
    ae_vector_destroy(&p->hasbndl);
    ae_vector_destroy(&p->hasbndu);
    ae_matrix_destroy(&p->cleic);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->fi);
    ae_matrix_destroy(&p->j);
    _rcommstate_destroy(&p->rstate);
    _rcommstate_destroy(&p->rstateaul);
    ae_vector_destroy(&p->scaledbndl);
    ae_vector_destroy(&p->scaledbndu);
    ae_matrix_destroy(&p->scaledcleic);
    ae_vector_destroy(&p->xc);
    ae_vector_destroy(&p->xstart);
    ae_vector_destroy(&p->xbase);
    ae_vector_destroy(&p->fbase);
    ae_vector_destroy(&p->dfbase);
    ae_vector_destroy(&p->fm2);
    ae_vector_destroy(&p->fm1);
    ae_vector_destroy(&p->fp1);
    ae_vector_destroy(&p->fp2);
    ae_vector_destroy(&p->dfm1);
    ae_vector_destroy(&p->dfp1);
    ae_vector_destroy(&p->bufd);
    ae_vector_destroy(&p->bufc);
    ae_matrix_destroy(&p->bufw);
    ae_vector_destroy(&p->xk);
    ae_vector_destroy(&p->xk1);
    ae_vector_destroy(&p->gk);
    ae_vector_destroy(&p->gk1);
    _minlbfgsstate_destroy(&p->auloptimizer);
    _minlbfgsreport_destroy(&p->aulreport);
    ae_vector_destroy(&p->nubc);
    ae_vector_destroy(&p->nulc);
    ae_vector_destroy(&p->nunlc);
}


void _minnlcreport_init(void* _p, ae_state *_state)
{
    minnlcreport *p = (minnlcreport*)_p;
    ae_touch_ptr((void*)p);
}


void _minnlcreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    minnlcreport *dst = (minnlcreport*)_dst;
    minnlcreport *src = (minnlcreport*)_src;
    dst->iterationscount = src->iterationscount;
    dst->nfev = src->nfev;
    dst->varidx = src->varidx;
    dst->funcidx = src->funcidx;
    dst->terminationtype = src->terminationtype;
    dst->dbgphase0its = src->dbgphase0its;
}


void _minnlcreport_clear(void* _p)
{
    minnlcreport *p = (minnlcreport*)_p;
    ae_touch_ptr((void*)p);
}


void _minnlcreport_destroy(void* _p)
{
    minnlcreport *p = (minnlcreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
