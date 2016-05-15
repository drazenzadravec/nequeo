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
#include "minns.h"


/*$ Declarations $*/
static void minns_clearrequestfields(minnsstate* state, ae_state *_state);
static void minns_minnsinitinternal(ae_int_t n,
     /* Real    */ ae_vector* x,
     double diffstep,
     minnsstate* state,
     ae_state *_state);
static ae_bool minns_agsiteration(minnsstate* state, ae_state *_state);
static void minns_generatemeritfunction(minnsstate* state,
     ae_int_t sampleidx,
     ae_state *_state);
static void minns_unscalepointbc(minnsstate* state,
     /* Real    */ ae_vector* x,
     ae_state *_state);
static void minns_solveqp(/* Real    */ ae_matrix* sampleg,
     /* Real    */ ae_vector* diagh,
     ae_int_t nsample,
     ae_int_t nvars,
     /* Real    */ ae_vector* coeffs,
     ae_int_t* dbgncholesky,
     minnsqp* state,
     ae_state *_state);
static void minns_qpcalculategradfunc(/* Real    */ ae_matrix* sampleg,
     /* Real    */ ae_vector* diagh,
     ae_int_t nsample,
     ae_int_t nvars,
     /* Real    */ ae_vector* coeffs,
     /* Real    */ ae_vector* g,
     double* f,
     /* Real    */ ae_vector* tmp,
     ae_state *_state);
static void minns_qpcalculatefunc(/* Real    */ ae_matrix* sampleg,
     /* Real    */ ae_vector* diagh,
     ae_int_t nsample,
     ae_int_t nvars,
     /* Real    */ ae_vector* coeffs,
     double* f,
     /* Real    */ ae_vector* tmp,
     ae_state *_state);
static void minns_qpsolveu(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     ae_state *_state);
static void minns_qpsolveut(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
                  NONSMOOTH NONCONVEX OPTIMIZATION
            SUBJECT TO BOX/LINEAR/NONLINEAR-NONSMOOTH CONSTRAINTS

DESCRIPTION:

The  subroutine  minimizes  function   F(x)  of N arguments subject to any
combination of:
* bound constraints
* linear inequality constraints
* linear equality constraints
* nonlinear equality constraints Gi(x)=0
* nonlinear inequality constraints Hi(x)<=0

IMPORTANT: see MinNSSetAlgoAGS for important  information  on  performance
           restrictions of AGS solver.

REQUIREMENTS:
* starting point X0 must be feasible or not too far away from the feasible
  set
* F(), G(), H() are continuous, locally Lipschitz  and  continuously  (but
  not necessarily twice) differentiable in an open dense  subset  of  R^N.
  Functions F(), G() and H() may be nonsmooth and non-convex.
  Informally speaking, it means  that  functions  are  composed  of  large
  differentiable "patches" with nonsmoothness having  place  only  at  the
  boundaries between these "patches".
  Most real-life nonsmooth  functions  satisfy  these  requirements.  Say,
  anything which involves finite number of abs(), min() and max() is  very
  likely to pass the test.
  Say, it is possible to optimize anything of the following:
  * f=abs(x0)+2*abs(x1)
  * f=max(x0,x1)
  * f=sin(max(x0,x1)+abs(x2))
* for nonlinearly constrained problems: F()  must  be  bounded from  below
  without nonlinear constraints (this requirement is due to the fact that,
  contrary to box and linear constraints, nonlinear ones  require  special
  handling).
* user must provide function value and gradient for F(), H(), G()  at  all
  points where function/gradient can be calculated. If optimizer  requires
  value exactly at the boundary between "patches" (say, at x=0 for f=abs(x)),
  where gradient is not defined, user may resolve tie arbitrarily (in  our
  case - return +1 or -1 at its discretion).
* NS solver supports numerical differentiation, i.e. it may  differentiate
  your function for you,  but  it  results  in  2N  increase  of  function
  evaluations. Not recommended unless you solve really small problems. See
  minnscreatef() for more information on this functionality.

USAGE:

1. User initializes algorithm state with MinNSCreate() call  and   chooses
   what NLC solver to use. There is some solver which is used by  default,
   with default settings, but you should NOT rely on  default  choice.  It
   may change in future releases of ALGLIB without notice, and no one  can
   guarantee that new solver will be  able  to  solve  your  problem  with
   default settings.

   From the other side, if you choose solver explicitly, you can be pretty
   sure that it will work with new ALGLIB releases.

   In the current release following solvers can be used:
   * AGS solver (activated with MinNSSetAlgoAGS() function)

2. User adds boundary and/or linear and/or nonlinear constraints by  means
   of calling one of the following functions:
   a) MinNSSetBC() for boundary constraints
   b) MinNSSetLC() for linear constraints
   c) MinNSSetNLC() for nonlinear constraints
   You may combine (a), (b) and (c) in one optimization problem.

3. User sets scale of the variables with MinNSSetScale() function. It   is
   VERY important to set  scale  of  the  variables,  because  nonlinearly
   constrained problems are hard to solve when variables are badly scaled.

4. User sets stopping conditions with MinNSSetCond().

5. Finally, user calls MinNSOptimize()  function  which  takes   algorithm
   state and pointer (delegate, etc) to callback function which calculates
   F/G/H.

7. User calls MinNSResults() to get solution

8. Optionally user may call MinNSRestartFrom() to solve   another  problem
   with same N but another starting point. MinNSRestartFrom()  allows   to
   reuse already initialized structure.


INPUT PARAMETERS:
    N       -   problem dimension, N>0:
                * if given, only leading N elements of X are used
                * if not given, automatically determined from size of X
    X       -   starting point, array[N]:
                * it is better to set X to a feasible point
                * but X can be infeasible, in which case algorithm will try
                  to find feasible point first, using X as initial
                  approximation.

OUTPUT PARAMETERS:
    State   -   structure stores algorithm state

NOTE: minnscreatef() function may be used if  you  do  not  have  analytic
      gradient.   This   function  creates  solver  which  uses  numerical
      differentiation with user-specified step.

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnscreate(ae_int_t n,
     /* Real    */ ae_vector* x,
     minnsstate* state,
     ae_state *_state)
{

    _minnsstate_clear(state);

    ae_assert(n>=1, "MinNSCreate: N<1", _state);
    ae_assert(x->cnt>=n, "MinNSCreate: Length(X)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "MinNSCreate: X contains infinite or NaN values", _state);
    minns_minnsinitinternal(n, x, 0.0, state, _state);
}


/*************************************************************************
Version of minnscreatef() which uses numerical differentiation. I.e.,  you
do not have to calculate derivatives yourself. However, this version needs
2N times more function evaluations.

2-point differentiation formula is  used,  because  more  precise  4-point
formula is unstable when used on non-smooth functions.

INPUT PARAMETERS:
    N       -   problem dimension, N>0:
                * if given, only leading N elements of X are used
                * if not given, automatically determined from size of X
    X       -   starting point, array[N]:
                * it is better to set X to a feasible point
                * but X can be infeasible, in which case algorithm will try
                  to find feasible point first, using X as initial
                  approximation.
    DiffStep-   differentiation  step,  DiffStep>0.   Algorithm   performs
                numerical differentiation  with  step  for  I-th  variable
                being equal to DiffStep*S[I] (here S[] is a  scale vector,
                set by minnssetscale() function).
                Do not use  too  small  steps,  because  it  may  lead  to
                catastrophic cancellation during intermediate calculations.

OUTPUT PARAMETERS:
    State   -   structure stores algorithm state

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnscreatef(ae_int_t n,
     /* Real    */ ae_vector* x,
     double diffstep,
     minnsstate* state,
     ae_state *_state)
{

    _minnsstate_clear(state);

    ae_assert(n>=1, "MinNSCreateF: N<1", _state);
    ae_assert(x->cnt>=n, "MinNSCreateF: Length(X)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "MinNSCreateF: X contains infinite or NaN values", _state);
    ae_assert(ae_isfinite(diffstep, _state), "MinNSCreateF: DiffStep is infinite or NaN!", _state);
    ae_assert(ae_fp_greater(diffstep,(double)(0)), "MinNSCreateF: DiffStep is non-positive!", _state);
    minns_minnsinitinternal(n, x, diffstep, state, _state);
}


/*************************************************************************
This function sets boundary constraints.

Boundary constraints are inactive by default (after initial creation).
They are preserved after algorithm restart with minnsrestartfrom().

INPUT PARAMETERS:
    State   -   structure stores algorithm state
    BndL    -   lower bounds, array[N].
                If some (all) variables are unbounded, you may specify
                very small number or -INF.
    BndU    -   upper bounds, array[N].
                If some (all) variables are unbounded, you may specify
                very large number or +INF.

NOTE 1: it is possible to specify BndL[i]=BndU[i]. In this case I-th
variable will be "frozen" at X[i]=BndL[i]=BndU[i].

NOTE 2: AGS solver has following useful properties:
* bound constraints are always satisfied exactly
* function is evaluated only INSIDE area specified by  bound  constraints,
  even  when  numerical  differentiation is used (algorithm adjusts  nodes
  according to boundary constraints)

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnssetbc(minnsstate* state,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t n;


    n = state->n;
    ae_assert(bndl->cnt>=n, "MinNSSetBC: Length(BndL)<N", _state);
    ae_assert(bndu->cnt>=n, "MinNSSetBC: Length(BndU)<N", _state);
    for(i=0; i<=n-1; i++)
    {
        ae_assert(ae_isfinite(bndl->ptr.p_double[i], _state)||ae_isneginf(bndl->ptr.p_double[i], _state), "MinNSSetBC: BndL contains NAN or +INF", _state);
        ae_assert(ae_isfinite(bndu->ptr.p_double[i], _state)||ae_isposinf(bndu->ptr.p_double[i], _state), "MinNSSetBC: BndL contains NAN or -INF", _state);
        state->bndl.ptr.p_double[i] = bndl->ptr.p_double[i];
        state->hasbndl.ptr.p_bool[i] = ae_isfinite(bndl->ptr.p_double[i], _state);
        state->bndu.ptr.p_double[i] = bndu->ptr.p_double[i];
        state->hasbndu.ptr.p_bool[i] = ae_isfinite(bndu->ptr.p_double[i], _state);
    }
}


/*************************************************************************
This function sets linear constraints.

Linear constraints are inactive by default (after initial creation).
They are preserved after algorithm restart with minnsrestartfrom().

INPUT PARAMETERS:
    State   -   structure previously allocated with minnscreate() call.
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

NOTE: linear (non-bound) constraints are satisfied only approximately:

* there always exists some minor violation (about current sampling  radius
  in magnitude during optimization, about EpsX in the solution) due to use
  of penalty method to handle constraints.
* numerical differentiation, if used, may  lead  to  function  evaluations
  outside  of the feasible  area,   because   algorithm  does  NOT  change
  numerical differentiation formula according to linear constraints.

If you want constraints to be  satisfied  exactly, try to reformulate your
problem  in  such  manner  that  all constraints will become boundary ones
(this kind of constraints is always satisfied exactly, both in  the  final
solution and in all intermediate points).

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnssetlc(minnsstate* state,
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
    ae_assert(k>=0, "MinNSSetLC: K<0", _state);
    ae_assert(c->cols>=n+1||k==0, "MinNSSetLC: Cols(C)<N+1", _state);
    ae_assert(c->rows>=k, "MinNSSetLC: Rows(C)<K", _state);
    ae_assert(ct->cnt>=k, "MinNSSetLC: Length(CT)<K", _state);
    ae_assert(apservisfinitematrix(c, k, n+1, _state), "MinNSSetLC: C contains infinite or NaN values!", _state);
    
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
This function sets nonlinear constraints.

In fact, this function sets NUMBER of nonlinear  constraints.  Constraints
itself (constraint functions) are passed to minnsoptimize() method.   This
method requires user-defined vector function F[]  and  its  Jacobian  J[],
where:
* first component of F[] and first row  of  Jacobian  J[]  correspond   to
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
    State   -   structure previously allocated with minnscreate() call.
    NLEC    -   number of Non-Linear Equality Constraints (NLEC), >=0
    NLIC    -   number of Non-Linear Inquality Constraints (NLIC), >=0

NOTE 1: nonlinear constraints are satisfied only  approximately!   It   is
        possible   that  algorithm  will  evaluate  function  outside   of
        the feasible area!

NOTE 2: algorithm scales variables  according  to   scale   specified   by
        minnssetscale()  function,  so  it can handle problems with  badly
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
        (where S is a scale set by minnssetscale() function).

NOTE 3: nonlinear constraints are always hard to handle,  no  matter  what
        algorithm you try to use. Even basic box/linear constraints modify
        function  curvature   by  adding   valleys  and  ridges.  However,
        nonlinear constraints add valleys which are very  hard  to  follow
        due to their "curved" nature.

        It means that optimization with single nonlinear constraint may be
        significantly slower than optimization with multiple linear  ones.
        It is normal situation, and we recommend you to  carefully  choose
        Rho parameter of minnssetalgoags(), because too  large  value  may
        slow down convergence.


  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnssetnlc(minnsstate* state,
     ae_int_t nlec,
     ae_int_t nlic,
     ae_state *_state)
{


    ae_assert(nlec>=0, "MinNSSetNLC: NLEC<0", _state);
    ae_assert(nlic>=0, "MinNSSetNLC: NLIC<0", _state);
    state->ng = nlec;
    state->nh = nlic;
    ae_vector_set_length(&state->fi, 1+state->ng+state->nh, _state);
    ae_matrix_set_length(&state->j, 1+state->ng+state->nh, state->n, _state);
}


/*************************************************************************
This function sets stopping conditions for iterations of optimizer.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    EpsX    -   >=0
                The AGS solver finishes its work if  on  k+1-th  iteration
                sampling radius decreases below EpsX.
    MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                iterations is unlimited.

Passing EpsX=0  and  MaxIts=0  (simultaneously)  will  lead  to  automatic
stopping criterion selection. We do not recommend you to rely  on  default
choice in production code.

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnssetcond(minnsstate* state,
     double epsx,
     ae_int_t maxits,
     ae_state *_state)
{


    ae_assert(ae_isfinite(epsx, _state), "MinNSSetCond: EpsX is not finite number", _state);
    ae_assert(ae_fp_greater_eq(epsx,(double)(0)), "MinNSSetCond: negative EpsX", _state);
    ae_assert(maxits>=0, "MinNSSetCond: negative MaxIts!", _state);
    if( ae_fp_eq(epsx,(double)(0))&&maxits==0 )
    {
        epsx = 1.0E-6;
    }
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
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnssetscale(minnsstate* state,
     /* Real    */ ae_vector* s,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(s->cnt>=state->n, "MinNSSetScale: Length(S)<N", _state);
    for(i=0; i<=state->n-1; i++)
    {
        ae_assert(ae_isfinite(s->ptr.p_double[i], _state), "MinNSSetScale: S contains infinite or NAN elements", _state);
        ae_assert(ae_fp_neq(s->ptr.p_double[i],(double)(0)), "MinNSSetScale: S contains zero elements", _state);
        state->s.ptr.p_double[i] = ae_fabs(s->ptr.p_double[i], _state);
    }
}


/*************************************************************************
This function tells MinNS unit to use  AGS  (adaptive  gradient  sampling)
algorithm for nonsmooth constrained  optimization.  This  algorithm  is  a
slight modification of one described in  "An  Adaptive  Gradient  Sampling
Algorithm for Nonsmooth Optimization" by Frank E. Curtisy and Xiaocun Quez.

This optimizer has following benefits and drawbacks:
+ robustness; it can be used with nonsmooth and nonconvex functions.
+ relatively easy tuning; most of the metaparameters are easy to select.
- it has convergence of steepest descent, slower than CG/LBFGS.
- each iteration involves evaluation of ~2N gradient values  and  solution
  of 2Nx2N quadratic programming problem, which  limits  applicability  of
  algorithm by small-scale problems (up to 50-100).

IMPORTANT: this  algorithm  has  convergence  guarantees,   i.e.  it  will
           steadily move towards some stationary point of the function.

           However, "stationary point" does not  always  mean  "solution".
           Nonsmooth problems often have "flat spots",  i.e.  areas  where
           function do not change at all. Such "flat spots" are stationary
           points by definition, and algorithm may be caught here.

           Nonsmooth CONVEX tasks are not prone to  this  problem. Say, if
           your function has form f()=MAX(f0,f1,...), and f_i are  convex,
           then f() is convex too and you have guaranteed  convergence  to
           solution.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    Radius  -   initial sampling radius, >=0.

                Internally multiplied  by  vector of  per-variable  scales
                specified by minnssetscale()).

                You should select relatively large sampling radius, roughly
                proportional to scaled length of the first  steps  of  the
                algorithm. Something close to 0.1 in magnitude  should  be
                good for most problems.

                AGS solver can automatically decrease radius, so too large
                radius is  not a problem (assuming that you  won't  choose
                so large radius that algorithm  will  sample  function  in
                too far away points, where gradient value is irrelevant).

                Too small radius won't cause algorithm to fail, but it may
                slow down algorithm (it may  have  to  perform  too  short
                steps).
    Penalty -   penalty coefficient for nonlinear constraints:
                * for problem with nonlinear constraints  should  be  some
                  problem-specific  positive   value,  large  enough  that
                  penalty term changes shape of the function.
                  Starting  from  some  problem-specific   value   penalty
                  coefficient becomes  large  enough  to  exactly  enforce
                  nonlinear constraints;  larger  values  do  not  improve
                  precision.
                  Increasing it too much may slow down convergence, so you
                  should choose it carefully.
                * can be zero for problems WITHOUT  nonlinear  constraints
                  (i.e. for unconstrained ones or ones with  just  box  or
                  linear constraints)
                * if you specify zero value for problem with at least  one
                  nonlinear  constraint,  algorithm  will  terminate  with
                  error code -1.

ALGORITHM OUTLINE

The very basic outline of unconstrained AGS algorithm is given below:

0. If sampling radius is below EpsX  or  we  performed  more  then  MaxIts
   iterations - STOP.
1. sample O(N) gradient values at random locations  around  current point;
   informally speaking, this sample is an implicit piecewise  linear model
   of the function, although algorithm formulation does  not  mention that
   explicitly
2. solve quadratic programming problem in order to find descent direction
3. if QP solver tells us that we  are  near  solution,  decrease  sampling
   radius and move to (0)
4. perform backtracking line search
5. after moving to new point, goto (0)

As for the constraints:
* box constraints are handled exactly  by  modification  of  the  function
  being minimized
* linear/nonlinear constraints are handled by adding L1  penalty.  Because
  our solver can handle nonsmoothness, we can  use  L1  penalty  function,
  which is an exact one  (i.e.  exact  solution  is  returned  under  such
  penalty).
* penalty coefficient for  linear  constraints  is  chosen  automatically;
  however, penalty coefficient for nonlinear constraints must be specified
  by user.

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnssetalgoags(minnsstate* state,
     double radius,
     double penalty,
     ae_state *_state)
{


    ae_assert(ae_isfinite(radius, _state), "MinNSSetAlgoAGS: Radius is not finite", _state);
    ae_assert(ae_fp_greater(radius,(double)(0)), "MinNSSetAlgoAGS: Radius<=0", _state);
    ae_assert(ae_isfinite(penalty, _state), "MinNSSetAlgoAGS: Penalty is not finite", _state);
    ae_assert(ae_fp_greater_eq(penalty,0.0), "MinNSSetAlgoAGS: Penalty<0", _state);
    state->agsrhononlinear = penalty;
    state->agsradius = radius;
    state->solvertype = 0;
}


/*************************************************************************
This function turns on/off reporting.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    NeedXRep-   whether iteration reports are needed or not

If NeedXRep is True, algorithm will call rep() callback function if  it is
provided to minnsoptimize().

  -- ALGLIB --
     Copyright 28.11.2010 by Bochkanov Sergey
*************************************************************************/
void minnssetxrep(minnsstate* state, ae_bool needxrep, ae_state *_state)
{


    state->xrep = needxrep;
}


/*************************************************************************
This subroutine submits request for termination of running  optimizer.  It
should be called from user-supplied callback when user decides that it  is
time to "smoothly" terminate optimization process.  As  result,  optimizer
stops at point which was "current accepted" when termination  request  was
submitted and returns error code 8 (successful termination).

INPUT PARAMETERS:
    State   -   optimizer structure

NOTE: after  request  for  termination  optimizer  may   perform   several
      additional calls to user-supplied callbacks. It does  NOT  guarantee
      to stop immediately - it just guarantees that these additional calls
      will be discarded later.

NOTE: calling this function on optimizer which is NOT running will have no
      effect.

NOTE: multiple calls to this function are possible. First call is counted,
      subsequent calls are silently ignored.

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnsrequesttermination(minnsstate* state, ae_state *_state)
{


    state->userterminationneeded = ae_true;
}


/*************************************************************************

NOTES:

1. This function has two different implementations: one which  uses  exact
   (analytical) user-supplied Jacobian, and one which uses  only  function
   vector and numerically  differentiates  function  in  order  to  obtain
   gradient.

   Depending  on  the  specific  function  used to create optimizer object
   you should choose appropriate variant of  minnsoptimize() -  one  which
   accepts function AND Jacobian or one which accepts ONLY function.

   Be careful to choose variant of minnsoptimize()  which  corresponds  to
   your optimization scheme! Table below lists different  combinations  of
   callback (function/gradient) passed to minnsoptimize()    and  specific
   function used to create optimizer.


                     |         USER PASSED TO minnsoptimize()
   CREATED WITH      |  function only   |  function and gradient
   ------------------------------------------------------------
   minnscreatef()    |     works               FAILS
   minnscreate()     |     FAILS               works

   Here "FAILS" denotes inappropriate combinations  of  optimizer creation
   function  and  minnsoptimize()  version.   Attemps   to    use     such
   combination will lead to exception. Either  you  did  not pass gradient
   when it WAS needed or you passed gradient when it was NOT needed.

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
ae_bool minnsiteration(minnsstate* state, ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    ae_int_t n;
    ae_int_t ng;
    ae_int_t nh;
    double v;
    double xp;
    double xm;
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
        v = state->rstate.ra.ptr.p_double[0];
        xp = state->rstate.ra.ptr.p_double[1];
        xm = state->rstate.ra.ptr.p_double[2];
    }
    else
    {
        i = -983;
        k = -989;
        n = -834;
        ng = 900;
        nh = -287;
        v = 364;
        xp = 214;
        xm = -338;
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
    
    /*
     * Init
     */
    state->replcerr = 0.0;
    state->repnlcerr = 0.0;
    state->repterminationtype = 0;
    state->repinneriterationscount = 0;
    state->repouteriterationscount = 0;
    state->repnfev = 0;
    state->repvaridx = 0;
    state->repfuncidx = 0;
    state->userterminationneeded = ae_false;
    state->dbgncholesky = 0;
    n = state->n;
    ng = state->ng;
    nh = state->nh;
    minns_clearrequestfields(state, _state);
    
    /*
     * AGS solver
     */
    if( state->solvertype!=0 )
    {
        goto lbl_4;
    }
    if( ae_fp_neq(state->diffstep,(double)(0)) )
    {
        rvectorsetlengthatleast(&state->xbase, n, _state);
        rvectorsetlengthatleast(&state->fm, 1+ng+nh, _state);
        rvectorsetlengthatleast(&state->fp, 1+ng+nh, _state);
    }
    ae_vector_set_length(&state->rstateags.ia, 13+1, _state);
    ae_vector_set_length(&state->rstateags.ba, 3+1, _state);
    ae_vector_set_length(&state->rstateags.ra, 9+1, _state);
    state->rstateags.stage = -1;
lbl_6:
    if( !minns_agsiteration(state, _state) )
    {
        goto lbl_7;
    }
    
    /*
     * Numerical differentiation (if needed) - intercept NeedFiJ
     * request and replace it by sequence of NeedFi requests
     */
    if( !(ae_fp_neq(state->diffstep,(double)(0))&&state->needfij) )
    {
        goto lbl_8;
    }
    state->needfij = ae_false;
    state->needfi = ae_true;
    ae_v_move(&state->xbase.ptr.p_double[0], 1, &state->x.ptr.p_double[0], 1, ae_v_len(0,n-1));
    k = 0;
lbl_10:
    if( k>n-1 )
    {
        goto lbl_12;
    }
    v = state->xbase.ptr.p_double[k];
    xm = v-state->diffstep*state->s.ptr.p_double[k];
    xp = v+state->diffstep*state->s.ptr.p_double[k];
    if( state->hasbndl.ptr.p_bool[k]&&ae_fp_less(xm,state->bndl.ptr.p_double[k]) )
    {
        xm = state->bndl.ptr.p_double[k];
    }
    if( state->hasbndu.ptr.p_bool[k]&&ae_fp_greater(xp,state->bndu.ptr.p_double[k]) )
    {
        xp = state->bndu.ptr.p_double[k];
    }
    ae_assert(ae_fp_less_eq(xm,xp), "MinNS: integrity check failed", _state);
    if( ae_fp_eq(xm,xp) )
    {
        goto lbl_13;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->x.ptr.p_double[k] = xm;
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    ae_v_move(&state->fm.ptr.p_double[0], 1, &state->fi.ptr.p_double[0], 1, ae_v_len(0,ng+nh));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->x.ptr.p_double[k] = xp;
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    ae_v_move(&state->fp.ptr.p_double[0], 1, &state->fi.ptr.p_double[0], 1, ae_v_len(0,ng+nh));
    ae_v_move(&state->j.ptr.pp_double[0][k], state->j.stride, &state->fp.ptr.p_double[0], 1, ae_v_len(0,ng+nh));
    ae_v_sub(&state->j.ptr.pp_double[0][k], state->j.stride, &state->fm.ptr.p_double[0], 1, ae_v_len(0,ng+nh));
    v = 1/(xp-xm);
    ae_v_muld(&state->j.ptr.pp_double[0][k], state->j.stride, ae_v_len(0,ng+nh), v);
    state->repnfev = state->repnfev+2;
    goto lbl_14;
lbl_13:
    for(i=0; i<=ng+nh; i++)
    {
        state->j.ptr.pp_double[i][k] = 0.0;
    }
lbl_14:
    k = k+1;
    goto lbl_10;
lbl_12:
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xbase.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    
    /*
     * Restore previous values of fields and continue
     */
    state->needfi = ae_false;
    state->needfij = ae_true;
    goto lbl_6;
lbl_8:
    
    /*
     * Forward request to caller
     */
    state->rstate.stage = 3;
    goto lbl_rcomm;
lbl_3:
    inc(&state->repnfev, _state);
    goto lbl_6;
lbl_7:
    result = ae_false;
    return result;
lbl_4:
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
    state->rstate.ra.ptr.p_double[0] = v;
    state->rstate.ra.ptr.p_double[1] = xp;
    state->rstate.ra.ptr.p_double[2] = xm;
    return result;
}


/*************************************************************************
MinNS results

INPUT PARAMETERS:
    State   -   algorithm state

OUTPUT PARAMETERS:
    X       -   array[0..N-1], solution
    Rep     -   optimization report. You should check Rep.TerminationType
                in  order  to  distinguish  successful  termination  from
                unsuccessful one:
                * -8   internal integrity control  detected  infinite  or
                       NAN   values   in   function/gradient.    Abnormal
                       termination signalled.
                * -3   box constraints are inconsistent
                * -1   inconsistent parameters were passed:
                       * penalty parameter for minnssetalgoags() is zero,
                         but we have nonlinear constraints set by minnssetnlc()
                *  2   sampling radius decreased below epsx
                *  7    stopping conditions are too stringent,
                        further improvement is impossible,
                        X contains best point found so far.
                *  8    User requested termination via minnsrequesttermination()

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnsresults(minnsstate* state,
     /* Real    */ ae_vector* x,
     minnsreport* rep,
     ae_state *_state)
{

    ae_vector_clear(x);
    _minnsreport_clear(rep);

    minnsresultsbuf(state, x, rep, _state);
}


/*************************************************************************

Buffered implementation of minnsresults() which uses pre-allocated  buffer
to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
intended to be used in the inner cycles of performance critical algorithms
where array reallocation penalty is too large to be ignored.

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnsresultsbuf(minnsstate* state,
     /* Real    */ ae_vector* x,
     minnsreport* rep,
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
    rep->cerr = ae_maxreal(state->replcerr, state->repnlcerr, _state);
    rep->lcerr = state->replcerr;
    rep->nlcerr = state->repnlcerr;
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
    State   -   structure previously allocated with minnscreate() call.
    X       -   new starting point.

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
void minnsrestartfrom(minnsstate* state,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t n;


    n = state->n;
    
    /*
     * First, check for errors in the inputs
     */
    ae_assert(x->cnt>=n, "MinNSRestartFrom: Length(X)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "MinNSRestartFrom: X contains infinite or NaN values!", _state);
    
    /*
     * Set XC
     */
    ae_v_move(&state->xstart.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
    
    /*
     * prepare RComm facilities
     */
    ae_vector_set_length(&state->rstate.ia, 4+1, _state);
    ae_vector_set_length(&state->rstate.ra, 2+1, _state);
    state->rstate.stage = -1;
    minns_clearrequestfields(state, _state);
}


/*************************************************************************
Clears request fileds (to be sure that we don't forget to clear something)
*************************************************************************/
static void minns_clearrequestfields(minnsstate* state, ae_state *_state)
{


    state->needfi = ae_false;
    state->needfij = ae_false;
    state->xupdated = ae_false;
}


/*************************************************************************
Internal initialization subroutine.
Sets default NLC solver with default criteria.
*************************************************************************/
static void minns_minnsinitinternal(ae_int_t n,
     /* Real    */ ae_vector* x,
     double diffstep,
     minnsstate* state,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_matrix c;
    ae_vector ct;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);

    state->agsinitstp = 0.2;
    state->agsstattold = 1.0E-10;
    state->agsshortstpabs = 1.0E-10;
    state->agsshortstprel = 0.75;
    state->agsshortf = 10*ae_machineepsilon;
    state->agsrhononlinear = 0.0;
    state->agsraddecay = 0.2;
    state->agsalphadecay = 0.5;
    state->agsdecrease = 0.1;
    state->agsmaxraddecays = 50;
    state->agsmaxbacktrack = 20;
    state->agsmaxbacktracknonfull = 8;
    state->agspenaltylevel = 10.0;
    state->agspenaltyincrease = 20.0;
    state->agsminupdate = ae_maxint(5, n/2, _state);
    state->agssamplesize = ae_maxint(2*n+1, state->agsminupdate+1, _state);
    state->agsshortlimit = 4+state->agssamplesize/state->agsminupdate;
    
    /*
     * Initialize other params
     */
    state->n = n;
    state->diffstep = diffstep;
    ae_vector_set_length(&state->bndl, n, _state);
    ae_vector_set_length(&state->hasbndl, n, _state);
    ae_vector_set_length(&state->bndu, n, _state);
    ae_vector_set_length(&state->hasbndu, n, _state);
    ae_vector_set_length(&state->s, n, _state);
    ae_vector_set_length(&state->xstart, n, _state);
    ae_vector_set_length(&state->xc, n, _state);
    ae_vector_set_length(&state->xn, n, _state);
    ae_vector_set_length(&state->d, n, _state);
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
    minnssetlc(state, &c, &ct, 0, _state);
    minnssetnlc(state, 0, 0, _state);
    minnssetcond(state, 0.0, 0, _state);
    minnssetxrep(state, ae_false, _state);
    minnssetalgoags(state, 0.1, 1000.0, _state);
    minnsrestartfrom(state, x, _state);
    ae_frame_leave(_state);
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
     Copyright 06.06.2015 by Bochkanov Sergey
*************************************************************************/
static ae_bool minns_agsiteration(minnsstate* state, ae_state *_state)
{
    ae_int_t n;
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t ng;
    ae_int_t nh;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double radius0;
    double radius;
    ae_int_t radiusdecays;
    double alpha;
    double recommendedstep;
    double dnrm;
    double dg;
    double v;
    double vv;
    ae_int_t maxsamplesize;
    ae_int_t cursamplesize;
    double v0;
    double v1;
    ae_bool restartneeded;
    ae_bool b;
    ae_bool alphadecreased;
    ae_int_t shortstepscnt;
    ae_int_t backtrackits;
    ae_int_t maxbacktrackits;
    ae_bool fullsample;
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
    if( state->rstateags.stage>=0 )
    {
        n = state->rstateags.ia.ptr.p_int[0];
        nec = state->rstateags.ia.ptr.p_int[1];
        nic = state->rstateags.ia.ptr.p_int[2];
        ng = state->rstateags.ia.ptr.p_int[3];
        nh = state->rstateags.ia.ptr.p_int[4];
        i = state->rstateags.ia.ptr.p_int[5];
        j = state->rstateags.ia.ptr.p_int[6];
        k = state->rstateags.ia.ptr.p_int[7];
        radiusdecays = state->rstateags.ia.ptr.p_int[8];
        maxsamplesize = state->rstateags.ia.ptr.p_int[9];
        cursamplesize = state->rstateags.ia.ptr.p_int[10];
        shortstepscnt = state->rstateags.ia.ptr.p_int[11];
        backtrackits = state->rstateags.ia.ptr.p_int[12];
        maxbacktrackits = state->rstateags.ia.ptr.p_int[13];
        restartneeded = state->rstateags.ba.ptr.p_bool[0];
        b = state->rstateags.ba.ptr.p_bool[1];
        alphadecreased = state->rstateags.ba.ptr.p_bool[2];
        fullsample = state->rstateags.ba.ptr.p_bool[3];
        radius0 = state->rstateags.ra.ptr.p_double[0];
        radius = state->rstateags.ra.ptr.p_double[1];
        alpha = state->rstateags.ra.ptr.p_double[2];
        recommendedstep = state->rstateags.ra.ptr.p_double[3];
        dnrm = state->rstateags.ra.ptr.p_double[4];
        dg = state->rstateags.ra.ptr.p_double[5];
        v = state->rstateags.ra.ptr.p_double[6];
        vv = state->rstateags.ra.ptr.p_double[7];
        v0 = state->rstateags.ra.ptr.p_double[8];
        v1 = state->rstateags.ra.ptr.p_double[9];
    }
    else
    {
        n = -686;
        nec = 912;
        nic = 585;
        ng = 497;
        nh = -271;
        i = -581;
        j = 745;
        k = -533;
        radiusdecays = -77;
        maxsamplesize = 678;
        cursamplesize = -293;
        shortstepscnt = 316;
        backtrackits = 647;
        maxbacktrackits = -756;
        restartneeded = ae_false;
        b = ae_true;
        alphadecreased = ae_false;
        fullsample = ae_false;
        radius0 = 13;
        radius = -740;
        alpha = 262;
        recommendedstep = 439;
        dnrm = 327;
        dg = 222;
        v = -589;
        vv = 274;
        v0 = 845;
        v1 = 456;
    }
    if( state->rstateags.stage==0 )
    {
        goto lbl_0;
    }
    if( state->rstateags.stage==1 )
    {
        goto lbl_1;
    }
    if( state->rstateags.stage==2 )
    {
        goto lbl_2;
    }
    if( state->rstateags.stage==3 )
    {
        goto lbl_3;
    }
    
    /*
     * Routine body
     */
    ae_assert(state->solvertype==0, "MinNS: internal error", _state);
    n = state->n;
    nec = state->nec;
    nic = state->nic;
    ng = state->ng;
    nh = state->nh;
    
    /*
     * Check consistency of parameters
     */
    if( ng+nh>0&&ae_fp_eq(state->agsrhononlinear,(double)(0)) )
    {
        state->repterminationtype = -1;
        result = ae_false;
        return result;
    }
    
    /*
     * Allocate arrays.
     */
    rvectorsetlengthatleast(&state->colmax, n, _state);
    rvectorsetlengthatleast(&state->diagh, n, _state);
    rvectorsetlengthatleast(&state->signmin, n, _state);
    rvectorsetlengthatleast(&state->signmax, n, _state);
    maxsamplesize = state->agssamplesize;
    rmatrixsetlengthatleast(&state->samplex, maxsamplesize+1, n, _state);
    rmatrixsetlengthatleast(&state->samplegm, maxsamplesize+1, n, _state);
    rmatrixsetlengthatleast(&state->samplegmbc, maxsamplesize+1, n, _state);
    rvectorsetlengthatleast(&state->samplef, maxsamplesize+1, _state);
    rvectorsetlengthatleast(&state->samplef0, maxsamplesize+1, _state);
    rvectorsetlengthatleast(&state->grs, n, _state);
    
    /*
     * Prepare optimizer
     */
    rvectorsetlengthatleast(&state->tmp0, maxsamplesize, _state);
    rvectorsetlengthatleast(&state->tmp1, maxsamplesize, _state);
    ivectorsetlengthatleast(&state->tmp3, 1, _state);
    rmatrixsetlengthatleast(&state->tmp2, 1, maxsamplesize+1, _state);
    for(i=0; i<=maxsamplesize-1; i++)
    {
        state->tmp0.ptr.p_double[i] = 0.0;
        state->tmp1.ptr.p_double[i] = _state->v_posinf;
    }
    
    /*
     * Prepare RNG, seed it with fixed values so
     * that each run on same problem yeilds same results
     */
    hqrndseed(7235, 98532, &state->agsrs, _state);
    
    /*
     * Prepare initial point subject to current bound constraints and
     * perform scaling of bound constraints, linear constraints, point itself
     */
    rvectorsetlengthatleast(&state->scaledbndl, n, _state);
    rvectorsetlengthatleast(&state->scaledbndu, n, _state);
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * Check and scale constraints
         */
        if( (state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i])&&ae_fp_less(state->bndu.ptr.p_double[i],state->bndl.ptr.p_double[i]) )
        {
            state->repterminationtype = -3;
            result = ae_false;
            return result;
        }
        if( state->hasbndl.ptr.p_bool[i] )
        {
            state->scaledbndl.ptr.p_double[i] = state->bndl.ptr.p_double[i]/state->s.ptr.p_double[i];
        }
        else
        {
            state->scaledbndl.ptr.p_double[i] = _state->v_neginf;
        }
        if( state->hasbndu.ptr.p_bool[i] )
        {
            state->scaledbndu.ptr.p_double[i] = state->bndu.ptr.p_double[i]/state->s.ptr.p_double[i];
        }
        else
        {
            state->scaledbndu.ptr.p_double[i] = _state->v_posinf;
        }
        if( state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i] )
        {
            ae_assert(ae_fp_less_eq(state->scaledbndl.ptr.p_double[i],state->scaledbndu.ptr.p_double[i]), "MinNS: integrity check failed", _state);
        }
        if( (state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i])&&ae_fp_eq(state->bndl.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
        {
            ae_assert(ae_fp_eq(state->scaledbndl.ptr.p_double[i],state->scaledbndu.ptr.p_double[i]), "MinNS: integrity check failed", _state);
        }
        
        /*
         * Scale and constrain point
         */
        state->xc.ptr.p_double[i] = state->xstart.ptr.p_double[i];
        if( state->hasbndl.ptr.p_bool[i]&&ae_fp_less_eq(state->xc.ptr.p_double[i],state->bndl.ptr.p_double[i]) )
        {
            state->xc.ptr.p_double[i] = state->scaledbndl.ptr.p_double[i];
            continue;
        }
        if( state->hasbndu.ptr.p_bool[i]&&ae_fp_greater_eq(state->xc.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
        {
            state->xc.ptr.p_double[i] = state->scaledbndu.ptr.p_double[i];
            continue;
        }
        state->xc.ptr.p_double[i] = state->xc.ptr.p_double[i]/state->s.ptr.p_double[i];
        if( state->hasbndl.ptr.p_bool[i]&&ae_fp_less_eq(state->xc.ptr.p_double[i],state->scaledbndl.ptr.p_double[i]) )
        {
            state->xc.ptr.p_double[i] = state->scaledbndl.ptr.p_double[i];
        }
        if( state->hasbndu.ptr.p_bool[i]&&ae_fp_greater_eq(state->xc.ptr.p_double[i],state->scaledbndu.ptr.p_double[i]) )
        {
            state->xc.ptr.p_double[i] = state->scaledbndu.ptr.p_double[i];
        }
    }
    rmatrixsetlengthatleast(&state->scaledcleic, nec+nic, n+1, _state);
    rvectorsetlengthatleast(&state->rholinear, nec+nic, _state);
    for(i=0; i<=nec+nic-1; i++)
    {
        
        /*
         * Initial value of penalty coefficient is zero
         */
        state->rholinear.ptr.p_double[i] = 0.0;
        
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
     * Main cycle
     *
     * We maintain several variables during iteration:
     * * RecommendedStep-   current estimate of recommended step length;
     *                      must be Radius0 on first entry
     * * Radius         -   current sampling radius
     * * CurSampleSize  -   current sample size (may change in future versions)
     * * FullSample     -   whether we have full sample, or only partial one
     * * RadiusDecays   -   total number of decreases performed for sampling radius
     */
    radius = state->agsradius;
    radius0 = radius;
    recommendedstep = ae_minreal(radius0, state->agsinitstp, _state);
    cursamplesize = 1;
    radiusdecays = 0;
    shortstepscnt = 0;
    fullsample = ae_false;
lbl_4:
    if( ae_false )
    {
        goto lbl_5;
    }
    
    /*
     * First phase of iteration - central point:
     *
     * 1. evaluate function at central point - first entry in sample.
     *    Its status is ignored, it is always recalculated.
     * 2. report point and check gradient/function value for NAN/INF
     * 3. check penalty coefficients for linear terms; increase them
     *    if directional derivative of function being optimized (not
     *    merit function!) is larger than derivative of penalty.
     * 4. update report on constraint violation
     */
    cursamplesize = ae_maxint(cursamplesize, 1, _state);
    ae_v_move(&state->samplex.ptr.pp_double[0][0], 1, &state->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
    minns_unscalepointbc(state, &state->x, _state);
    minns_clearrequestfields(state, _state);
    state->needfij = ae_true;
    state->rstateags.stage = 0;
    goto lbl_rcomm;
lbl_0:
    state->needfij = ae_false;
    state->replcerr = 0.0;
    for(i=0; i<=nec+nic-1; i++)
    {
        v = -state->scaledcleic.ptr.pp_double[i][n];
        for(j=0; j<=n-1; j++)
        {
            v = v+state->scaledcleic.ptr.pp_double[i][j]*state->xc.ptr.p_double[j];
        }
        if( i>=nec&&ae_fp_less_eq(v,(double)(0)) )
        {
            continue;
        }
        state->replcerr = ae_maxreal(state->replcerr, ae_fabs(v, _state), _state);
    }
    state->repnlcerr = 0.0;
    for(i=1; i<=ng+nh; i++)
    {
        v = state->fi.ptr.p_double[i];
        if( i>ng&&ae_fp_less_eq(v,(double)(0)) )
        {
            continue;
        }
        state->repnlcerr = ae_maxreal(state->repnlcerr, ae_fabs(v, _state), _state);
    }
    for(j=0; j<=n-1; j++)
    {
        state->grs.ptr.p_double[j] = state->j.ptr.pp_double[0][j]*state->s.ptr.p_double[j];
    }
    minns_generatemeritfunction(state, 0, _state);
    if( !state->xrep )
    {
        goto lbl_6;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->f = state->samplef0.ptr.p_double[0];
    minns_unscalepointbc(state, &state->x, _state);
    minns_clearrequestfields(state, _state);
    state->xupdated = ae_true;
    state->rstateags.stage = 1;
    goto lbl_rcomm;
lbl_1:
    state->xupdated = ae_false;
lbl_6:
    if( state->userterminationneeded )
    {
        
        /*
         * User requested termination
         */
        state->repterminationtype = 8;
        goto lbl_5;
    }
    v = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        v = v+ae_sqr(state->samplegm.ptr.pp_double[0][i], _state);
    }
    if( !ae_isfinite(v, _state)||!ae_isfinite(state->samplef.ptr.p_double[0], _state) )
    {
        
        /*
         * Abnormal termination - infinities in function/gradient
         */
        state->repterminationtype = -8;
        goto lbl_5;
    }
    restartneeded = ae_false;
    for(i=0; i<=nec+nic-1; i++)
    {
        
        /*
         * Evaluate penalty function.
         *
         * Skip update if penalty is satisfied exactly (this check
         * also covers situations when I-th row is exactly zero).
         */
        v = -state->scaledcleic.ptr.pp_double[i][n];
        for(j=0; j<=n-1; j++)
        {
            v = v+state->scaledcleic.ptr.pp_double[i][j]*state->xc.ptr.p_double[j];
        }
        if( i<nec&&ae_fp_eq(v,(double)(0)) )
        {
            continue;
        }
        if( i>=nec&&ae_fp_less_eq(v,(double)(0)) )
        {
            continue;
        }
        
        /*
         * Calculate directional derivative, compare it with threshold.
         *
         * NOTE: we rely on the fact that ScaledCLEIC is normalized
         */
        ae_assert(ae_fp_greater(state->agspenaltylevel,1.0), "MinNS: integrity error", _state);
        ae_assert(ae_fp_greater(state->agspenaltyincrease,state->agspenaltylevel), "MinNS: integrity error", _state);
        v = 0.0;
        for(j=0; j<=n-1; j++)
        {
            v = v+state->grs.ptr.p_double[j]*state->scaledcleic.ptr.pp_double[i][j];
        }
        v = ae_fabs(v, _state);
        if( ae_fp_greater(v*state->agspenaltylevel,state->rholinear.ptr.p_double[i]) )
        {
            state->rholinear.ptr.p_double[i] = v*state->agspenaltyincrease;
            restartneeded = ae_true;
        }
    }
    if( restartneeded )
    {
        cursamplesize = 0;
        goto lbl_4;
    }
    
    /*
     * Check stopping conditions.
     */
    if( radiusdecays>=state->agsmaxraddecays )
    {
        
        /*
         * Too many attempts to decrease radius
         */
        state->repterminationtype = 7;
        goto lbl_5;
    }
    if( state->repinneriterationscount>=state->maxits&&state->maxits>0 )
    {
        
        /*
         * Too many iterations
         */
        state->repterminationtype = 5;
        goto lbl_5;
    }
    if( ae_fp_less_eq(radius,state->epsx*state->agsraddecay) )
    {
        
        /*
         * Radius is smaller than required step tolerance multiplied by radius decay.
         *
         * Additional decay is required in order to make sure that optimization session
         * with radius equal to EpsX was successfully done.
         */
        state->repterminationtype = 2;
        goto lbl_5;
    }
    
    /*
     * Update sample:
     *
     * 1. invalidate entries which are too far away from XC
     *    and move all valid entries to beginning of the sample.
     * 2. add new entries until we have AGSSampleSize
     *    items in our sample. We remove oldest entries from
     *    sample until we have enough place to add at least
     *    AGSMinUpdate items.
     * 3. prepare "modified" gradient sample with respect to
     *    boundary constraints.
     */
    ae_assert(cursamplesize>=1, "MinNS: integrity check failed", _state);
    k = 1;
    for(i=1; i<=cursamplesize-1; i++)
    {
        
        /*
         * If entry is outside of Radius-ball around XC, discard it.
         */
        v = 0.0;
        for(j=0; j<=n-1; j++)
        {
            v = ae_maxreal(v, ae_fabs(state->samplex.ptr.pp_double[i][j]-state->xc.ptr.p_double[j], _state), _state);
        }
        if( ae_fp_greater(v,radius) )
        {
            continue;
        }
        
        /*
         * If central point is exactly at boundary, and corresponding
         * component of entry is OUT of boundary, entry is discarded.
         */
        b = ae_false;
        for(j=0; j<=n-1; j++)
        {
            b = b||((state->hasbndl.ptr.p_bool[j]&&ae_fp_eq(state->xc.ptr.p_double[j],state->scaledbndl.ptr.p_double[j]))&&ae_fp_neq(state->samplex.ptr.pp_double[i][j],state->scaledbndl.ptr.p_double[j]));
            b = b||((state->hasbndu.ptr.p_bool[j]&&ae_fp_eq(state->xc.ptr.p_double[j],state->scaledbndu.ptr.p_double[j]))&&ae_fp_neq(state->samplex.ptr.pp_double[i][j],state->scaledbndu.ptr.p_double[j]));
        }
        if( b )
        {
            continue;
        }
        
        /*
         * Move to the beginning
         */
        ae_v_move(&state->samplex.ptr.pp_double[k][0], 1, &state->samplex.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
        ae_v_move(&state->samplegm.ptr.pp_double[k][0], 1, &state->samplegm.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
        state->samplef.ptr.p_double[k] = state->samplef.ptr.p_double[i];
        state->samplef0.ptr.p_double[k] = state->samplef0.ptr.p_double[i];
        k = k+1;
    }
    cursamplesize = k;
    if( state->agssamplesize-cursamplesize<state->agsminupdate )
    {
        
        /*
         * Remove oldest entries
         */
        k = state->agsminupdate-(state->agssamplesize-cursamplesize);
        ae_assert(k<=cursamplesize-1, "MinNS: integrity check failed", _state);
        for(i=1; i<=cursamplesize-k-1; i++)
        {
            ae_v_move(&state->samplex.ptr.pp_double[i][0], 1, &state->samplex.ptr.pp_double[i+k][0], 1, ae_v_len(0,n-1));
            ae_v_move(&state->samplegm.ptr.pp_double[i][0], 1, &state->samplegm.ptr.pp_double[i+k][0], 1, ae_v_len(0,n-1));
            state->samplef.ptr.p_double[i] = state->samplef.ptr.p_double[i+k];
            state->samplef0.ptr.p_double[i] = state->samplef0.ptr.p_double[i+k];
        }
        cursamplesize = cursamplesize-k;
    }
    k = 0;
    i = cursamplesize;
lbl_8:
    if( i>ae_minint(cursamplesize+state->agsminupdate, state->agssamplesize, _state)-1 )
    {
        goto lbl_10;
    }
    for(j=0; j<=n-1; j++)
    {
        
        /*
         * Undistorted position
         */
        state->samplex.ptr.pp_double[i][j] = state->xc.ptr.p_double[j];
        
        /*
         * Do not apply distortion, if we are exactly at boundary constraint.
         */
        if( (state->hasbndl.ptr.p_bool[j]&&state->hasbndu.ptr.p_bool[j])&&ae_fp_eq(state->scaledbndl.ptr.p_double[j],state->scaledbndu.ptr.p_double[j]) )
        {
            continue;
        }
        if( state->hasbndl.ptr.p_bool[j]&&ae_fp_eq(state->samplex.ptr.pp_double[i][j],state->scaledbndl.ptr.p_double[j]) )
        {
            continue;
        }
        if( state->hasbndu.ptr.p_bool[j]&&ae_fp_eq(state->samplex.ptr.pp_double[i][j],state->scaledbndu.ptr.p_double[j]) )
        {
            continue;
        }
        
        /*
         * Apply distortion
         */
        if( ae_fp_greater_eq(hqrnduniformr(&state->agsrs, _state),0.5) )
        {
            
            /*
             * Sample at the left side with 50% probability
             */
            v0 = state->samplex.ptr.pp_double[i][j]-radius;
            v1 = state->samplex.ptr.pp_double[i][j];
            if( state->hasbndl.ptr.p_bool[j] )
            {
                v0 = ae_maxreal(state->scaledbndl.ptr.p_double[j], v0, _state);
            }
        }
        else
        {
            
            /*
             * Sample at the right side with 50% probability
             */
            v0 = state->samplex.ptr.pp_double[i][j];
            v1 = state->samplex.ptr.pp_double[i][j]+radius;
            if( state->hasbndu.ptr.p_bool[j] )
            {
                v1 = ae_minreal(state->scaledbndu.ptr.p_double[j], v1, _state);
            }
        }
        ae_assert(ae_fp_greater_eq(v1,v0), "MinNS: integrity check failed", _state);
        state->samplex.ptr.pp_double[i][j] = boundval(v0+(v1-v0)*hqrnduniformr(&state->agsrs, _state), v0, v1, _state);
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->samplex.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
    minns_unscalepointbc(state, &state->x, _state);
    minns_clearrequestfields(state, _state);
    state->needfij = ae_true;
    state->rstateags.stage = 2;
    goto lbl_rcomm;
lbl_2:
    state->needfij = ae_false;
    minns_generatemeritfunction(state, i, _state);
    k = k+1;
    i = i+1;
    goto lbl_8;
lbl_10:
    cursamplesize = cursamplesize+k;
    fullsample = cursamplesize==state->agssamplesize;
    for(j=0; j<=cursamplesize-1; j++)
    {
        
        /*
         * For J-th element in gradient sample, process all of its components
         * and modify them according to status of box constraints
         */
        for(i=0; i<=n-1; i++)
        {
            ae_assert(!state->hasbndl.ptr.p_bool[i]||ae_fp_greater_eq(state->xc.ptr.p_double[i],state->scaledbndl.ptr.p_double[i]), "MinNS: integrity error", _state);
            ae_assert(!state->hasbndu.ptr.p_bool[i]||ae_fp_less_eq(state->xc.ptr.p_double[i],state->scaledbndu.ptr.p_double[i]), "MinNS: integrity error", _state);
            state->samplegmbc.ptr.pp_double[j][i] = state->samplegm.ptr.pp_double[j][i];
            if( (state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i])&&ae_fp_eq(state->scaledbndl.ptr.p_double[i],state->scaledbndu.ptr.p_double[i]) )
            {
                
                /*
                 * I-th box constraint is of equality type (lower bound matches upper one).
                 * Simplest case, always active.
                 */
                state->samplegmbc.ptr.pp_double[j][i] = 0.0;
                continue;
            }
            if( state->hasbndl.ptr.p_bool[i]&&ae_fp_eq(state->xc.ptr.p_double[i],state->scaledbndl.ptr.p_double[i]) )
            {
                
                /*
                 * We are at lower bound.
                 *
                 * A bit more complex:
                 * * first, we have to activate/deactivate constraint depending on gradient at XC
                 * * second, in any case, I-th column of gradient sample must be non-positive
                 */
                if( ae_fp_greater_eq(state->samplegm.ptr.pp_double[0][i],0.0) )
                {
                    state->samplegmbc.ptr.pp_double[j][i] = 0.0;
                }
                state->samplegmbc.ptr.pp_double[j][i] = ae_minreal(state->samplegmbc.ptr.pp_double[j][i], 0.0, _state);
                continue;
            }
            if( state->hasbndu.ptr.p_bool[i]&&ae_fp_eq(state->xc.ptr.p_double[i],state->scaledbndu.ptr.p_double[i]) )
            {
                
                /*
                 * We are at upper bound.
                 *
                 * A bit more complex:
                 * * first, we have to activate/deactivate constraint depending on gradient at XC
                 * * second, in any case, I-th column of gradient sample must be non-negative
                 */
                if( ae_fp_less_eq(state->samplegm.ptr.pp_double[0][i],0.0) )
                {
                    state->samplegmbc.ptr.pp_double[j][i] = 0.0;
                }
                state->samplegmbc.ptr.pp_double[j][i] = ae_maxreal(state->samplegmbc.ptr.pp_double[j][i], 0.0, _state);
                continue;
            }
        }
    }
    
    /*
     * Calculate diagonal Hessian.
     *
     * This Hessian serves two purposes:
     * * first, it improves performance of gradient descent step
     * * second, it improves condition number of QP subproblem
     *   solved to determine step
     *
     * The idea is that for each variable we check whether sample
     * includes entries with alternating sign of gradient:
     * * if gradients with different signs are present, Hessian
     *   component is set to M/R, where M is a maximum magnitude
     *   of corresponding gradient component, R is a sampling radius.
     *   Note that sign=0 and sign=1 are treated as different ones
     * * if all gradients have same sign, Hessian component is
     *   set to M/R0, where R0 is initial sampling radius.
     */
    for(j=0; j<=n-1; j++)
    {
        state->colmax.ptr.p_double[j] = 0.0;
        state->signmin.ptr.p_double[j] = (double)(1);
        state->signmax.ptr.p_double[j] = (double)(-1);
    }
    for(i=0; i<=cursamplesize-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = state->samplegmbc.ptr.pp_double[i][j];
            state->colmax.ptr.p_double[j] = ae_maxreal(state->colmax.ptr.p_double[j], ae_fabs(v, _state), _state);
            state->signmin.ptr.p_double[j] = ae_minreal(state->signmin.ptr.p_double[j], (double)(ae_sign(v, _state)), _state);
            state->signmax.ptr.p_double[j] = ae_maxreal(state->signmax.ptr.p_double[j], (double)(ae_sign(v, _state)), _state);
        }
    }
    for(j=0; j<=n-1; j++)
    {
        if( ae_fp_neq(state->signmin.ptr.p_double[j],state->signmax.ptr.p_double[j]) )
        {
            
            /*
             * Alternating signs of gradient - step is proportional to current sampling radius
             */
            ae_assert(ae_fp_neq(state->colmax.ptr.p_double[j],(double)(0)), "MinNS: integrity check failed", _state);
            ae_assert(ae_fp_neq(radius,(double)(0)), "MinNS: integrity check failed", _state);
            state->diagh.ptr.p_double[j] = state->colmax.ptr.p_double[j]/radius;
            continue;
        }
        if( ae_fp_neq(state->colmax.ptr.p_double[j],(double)(0)) )
        {
            
            /*
             * Non-alternating sign of gradient, but non-zero.
             * Step is proportional to initial sampling radius
             */
            ae_assert(ae_fp_neq(radius0,(double)(0)), "MinNS: integrity check failed", _state);
            state->diagh.ptr.p_double[j] = state->colmax.ptr.p_double[j]/radius0;
            continue;
        }
        state->diagh.ptr.p_double[j] = (double)(1);
    }
    
    /*
     * PROJECTION PHASE
     *
     * We project zero vector on convex hull of gradient sample.
     * If projection is small enough, we decrease radius and restart.
     * Otherwise, this phase returns search direction in State.D.
     *
     * NOTE: because we use iterative solver, it may have trouble
     *       dealing with ill-conditioned problems. So we also employ
     *       second, backup test for stationarity - when too many
     *       subsequent backtracking searches resulted in short steps.
     */
    minns_solveqp(&state->samplegmbc, &state->diagh, cursamplesize, n, &state->tmp0, &state->dbgncholesky, &state->nsqp, _state);
    for(j=0; j<=n-1; j++)
    {
        state->d.ptr.p_double[j] = 0.0;
    }
    for(i=0; i<=cursamplesize-1; i++)
    {
        v = state->tmp0.ptr.p_double[i];
        ae_v_addd(&state->d.ptr.p_double[0], 1, &state->samplegmbc.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
    }
    v = 0.0;
    for(j=0; j<=n-1; j++)
    {
        v = ae_maxreal(v, ae_fabs(state->d.ptr.p_double[j]/coalesce(state->colmax.ptr.p_double[j], 1.0, _state), _state), _state);
    }
    if( ae_fp_less_eq(v,state->agsstattold) )
    {
        
        /*
         * Stationarity test succeded.
         * Decrease radius and restart.
         *
         * NOTE: we also clear ShortStepsCnt on restart
         */
        radius = radius*state->agsraddecay;
        shortstepscnt = 0;
        inc(&radiusdecays, _state);
        inc(&state->repinneriterationscount, _state);
        goto lbl_4;
    }
    for(i=0; i<=n-1; i++)
    {
        state->d.ptr.p_double[i] = -state->d.ptr.p_double[i]/state->diagh.ptr.p_double[i];
    }
    
    /*
     * Perform backtracking line search.
     * Update initial step length depending on search results.
     * Here we assume that D is non-zero.
     *
     * NOTE: if AGSShortLimit subsequent line searches resulted
     *       in steps shorter than AGSStatTolStp, we decrease radius.
     */
    dnrm = 0.0;
    dg = 0.0;
    for(i=0; i<=n-1; i++)
    {
        dnrm = dnrm+ae_sqr(state->d.ptr.p_double[i], _state);
        dg = dg+state->d.ptr.p_double[i]*state->samplegmbc.ptr.pp_double[0][i];
    }
    dnrm = ae_sqrt(dnrm, _state);
    ae_assert(ae_fp_greater(dnrm,(double)(0)), "MinNS: integrity error", _state);
    alpha = recommendedstep/dnrm;
    alphadecreased = ae_false;
    backtrackits = 0;
    if( fullsample )
    {
        maxbacktrackits = state->agsmaxbacktrack;
    }
    else
    {
        maxbacktrackits = state->agsmaxbacktracknonfull;
    }
lbl_11:
    if( ae_false )
    {
        goto lbl_12;
    }
    
    /*
     * Prepare XN and evaluate merit function at XN
     */
    ae_v_move(&state->xn.ptr.p_double[0], 1, &state->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_addd(&state->xn.ptr.p_double[0], 1, &state->d.ptr.p_double[0], 1, ae_v_len(0,n-1), alpha);
    enforceboundaryconstraints(&state->xn, &state->scaledbndl, &state->hasbndl, &state->scaledbndu, &state->hasbndu, n, 0, _state);
    ae_v_move(&state->samplex.ptr.pp_double[maxsamplesize][0], 1, &state->xn.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xn.ptr.p_double[0], 1, ae_v_len(0,n-1));
    minns_unscalepointbc(state, &state->x, _state);
    minns_clearrequestfields(state, _state);
    state->needfij = ae_true;
    state->rstateags.stage = 3;
    goto lbl_rcomm;
lbl_3:
    state->needfij = ae_false;
    minns_generatemeritfunction(state, maxsamplesize, _state);
    
    /*
     * Check sufficient decrease condition
     */
    ae_assert(ae_fp_greater(dnrm,(double)(0)), "MinNS: integrity error", _state);
    if( ae_fp_less_eq(state->samplef.ptr.p_double[maxsamplesize],state->samplef.ptr.p_double[0]+alpha*state->agsdecrease*dg) )
    {
        goto lbl_12;
    }
    
    /*
     * Decrease Alpha
     */
    alpha = alpha*state->agsalphadecay;
    alphadecreased = ae_true;
    
    /*
     * Update and check iterations counter.
     */
    inc(&backtrackits, _state);
    if( backtrackits>=maxbacktrackits )
    {
        
        /*
         * Too many backtracking searches performed without success.
         * Terminate iterations.
         */
        alpha = 0.0;
        alphadecreased = ae_true;
        ae_v_move(&state->xn.ptr.p_double[0], 1, &state->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
        goto lbl_12;
    }
    goto lbl_11;
lbl_12:
    if( (ae_fp_less_eq(alpha*dnrm,state->agsshortstpabs)||ae_fp_less_eq(alpha*dnrm,state->agsshortstprel*radius))||ae_fp_less_eq(ae_fabs(state->samplef.ptr.p_double[0]-state->samplef.ptr.p_double[maxsamplesize], _state),state->agsshortf) )
    {
        inc(&shortstepscnt, _state);
    }
    else
    {
        shortstepscnt = 0;
    }
    if( shortstepscnt>=state->agsshortlimit )
    {
        
        /*
         * Too many subsequent short steps.
         *
         * It may be possible that optimizer is unable to find out
         * that we have to decrease radius because of ill-conditioned
         * gradients.
         *
         * Decrease radius and restart.
         */
        radius = radius*state->agsraddecay;
        shortstepscnt = 0;
        inc(&radiusdecays, _state);
        inc(&state->repinneriterationscount, _state);
        goto lbl_4;
    }
    if( !alphadecreased )
    {
        recommendedstep = recommendedstep*2.0;
    }
    if( alphadecreased&&fullsample )
    {
        recommendedstep = recommendedstep*0.5;
    }
    
    /*
     * Next iteration
     */
    ae_v_move(&state->xc.ptr.p_double[0], 1, &state->xn.ptr.p_double[0], 1, ae_v_len(0,n-1));
    inc(&state->repinneriterationscount, _state);
    goto lbl_4;
lbl_5:
    
    /*
     * Convert back from scaled to unscaled representation
     */
    minns_unscalepointbc(state, &state->xc, _state);
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstateags.ia.ptr.p_int[0] = n;
    state->rstateags.ia.ptr.p_int[1] = nec;
    state->rstateags.ia.ptr.p_int[2] = nic;
    state->rstateags.ia.ptr.p_int[3] = ng;
    state->rstateags.ia.ptr.p_int[4] = nh;
    state->rstateags.ia.ptr.p_int[5] = i;
    state->rstateags.ia.ptr.p_int[6] = j;
    state->rstateags.ia.ptr.p_int[7] = k;
    state->rstateags.ia.ptr.p_int[8] = radiusdecays;
    state->rstateags.ia.ptr.p_int[9] = maxsamplesize;
    state->rstateags.ia.ptr.p_int[10] = cursamplesize;
    state->rstateags.ia.ptr.p_int[11] = shortstepscnt;
    state->rstateags.ia.ptr.p_int[12] = backtrackits;
    state->rstateags.ia.ptr.p_int[13] = maxbacktrackits;
    state->rstateags.ba.ptr.p_bool[0] = restartneeded;
    state->rstateags.ba.ptr.p_bool[1] = b;
    state->rstateags.ba.ptr.p_bool[2] = alphadecreased;
    state->rstateags.ba.ptr.p_bool[3] = fullsample;
    state->rstateags.ra.ptr.p_double[0] = radius0;
    state->rstateags.ra.ptr.p_double[1] = radius;
    state->rstateags.ra.ptr.p_double[2] = alpha;
    state->rstateags.ra.ptr.p_double[3] = recommendedstep;
    state->rstateags.ra.ptr.p_double[4] = dnrm;
    state->rstateags.ra.ptr.p_double[5] = dg;
    state->rstateags.ra.ptr.p_double[6] = v;
    state->rstateags.ra.ptr.p_double[7] = vv;
    state->rstateags.ra.ptr.p_double[8] = v0;
    state->rstateags.ra.ptr.p_double[9] = v1;
    return result;
}


/*************************************************************************
This function calculates merit function (target function +  penalties  for
violation of non-box constraints),  using  State.X  (unscaled),  State.Fi,
State.J (unscaled) and State.SampleX (scaled) as inputs.

Results are loaded:
* target function value - to State.SampleF0[SampleIdx]
* merit function value - to State.SampleF[SampleIdx]
* gradient of merit function - to State.SampleGM[SampleIdx]

  -- ALGLIB --
     Copyright 02.06.2015 by Bochkanov Sergey
*************************************************************************/
static void minns_generatemeritfunction(minnsstate* state,
     ae_int_t sampleidx,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t ng;
    ae_int_t nh;
    double v;
    double s;


    n = state->n;
    nec = state->nec;
    nic = state->nic;
    ng = state->ng;
    nh = state->nh;
    
    /*
     * Integrity check
     */
    for(i=0; i<=n-1; i++)
    {
        ae_assert(!state->hasbndl.ptr.p_bool[i]||ae_fp_greater_eq(state->x.ptr.p_double[i],state->bndl.ptr.p_double[i]), "MinNS: integrity error", _state);
        ae_assert(!state->hasbndu.ptr.p_bool[i]||ae_fp_less_eq(state->x.ptr.p_double[i],state->bndu.ptr.p_double[i]), "MinNS: integrity error", _state);
    }
    
    /*
     * Prepare "raw" function
     */
    state->samplef.ptr.p_double[sampleidx] = state->fi.ptr.p_double[0];
    state->samplef0.ptr.p_double[sampleidx] = state->fi.ptr.p_double[0];
    for(j=0; j<=n-1; j++)
    {
        state->samplegm.ptr.pp_double[sampleidx][j] = state->j.ptr.pp_double[0][j]*state->s.ptr.p_double[j];
    }
    
    /*
     * Modify merit function with linear constraints
     */
    for(i=0; i<=nec+nic-1; i++)
    {
        v = -state->scaledcleic.ptr.pp_double[i][n];
        for(j=0; j<=n-1; j++)
        {
            v = v+state->scaledcleic.ptr.pp_double[i][j]*state->samplex.ptr.pp_double[sampleidx][j];
        }
        if( i>=nec&&ae_fp_less(v,(double)(0)) )
        {
            continue;
        }
        state->samplef.ptr.p_double[sampleidx] = state->samplef.ptr.p_double[sampleidx]+state->rholinear.ptr.p_double[i]*ae_fabs(v, _state);
        s = (double)(ae_sign(v, _state));
        for(j=0; j<=n-1; j++)
        {
            state->samplegm.ptr.pp_double[sampleidx][j] = state->samplegm.ptr.pp_double[sampleidx][j]+state->rholinear.ptr.p_double[i]*s*state->scaledcleic.ptr.pp_double[i][j];
        }
    }
    
    /*
     * Modify merit function with nonlinear constraints
     */
    for(i=1; i<=ng+nh; i++)
    {
        v = state->fi.ptr.p_double[i];
        if( i<=ng&&ae_fp_eq(v,(double)(0)) )
        {
            continue;
        }
        if( i>ng&&ae_fp_less_eq(v,(double)(0)) )
        {
            continue;
        }
        state->samplef.ptr.p_double[sampleidx] = state->samplef.ptr.p_double[sampleidx]+state->agsrhononlinear*ae_fabs(v, _state);
        s = (double)(ae_sign(v, _state));
        for(j=0; j<=n-1; j++)
        {
            state->samplegm.ptr.pp_double[sampleidx][j] = state->samplegm.ptr.pp_double[sampleidx][j]+state->agsrhononlinear*s*state->j.ptr.pp_double[i][j]*state->s.ptr.p_double[j];
        }
    }
}


/*************************************************************************
This function performs transformation of  X  from  scaled  coordinates  to
unscaled ones, paying special attention to box constraints:
* points which were exactly at the boundary before scaling will be  mapped
  to corresponding boundary after scaling
* in any case, unscaled box constraints will be satisfied

  -- ALGLIB --
     Copyright 02.06.2015 by Bochkanov Sergey
*************************************************************************/
static void minns_unscalepointbc(minnsstate* state,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t i;


    for(i=0; i<=state->n-1; i++)
    {
        if( state->hasbndl.ptr.p_bool[i]&&ae_fp_less_eq(x->ptr.p_double[i],state->scaledbndl.ptr.p_double[i]) )
        {
            x->ptr.p_double[i] = state->bndl.ptr.p_double[i];
            continue;
        }
        if( state->hasbndu.ptr.p_bool[i]&&ae_fp_greater_eq(x->ptr.p_double[i],state->scaledbndu.ptr.p_double[i]) )
        {
            x->ptr.p_double[i] = state->bndu.ptr.p_double[i];
            continue;
        }
        x->ptr.p_double[i] = x->ptr.p_double[i]*state->s.ptr.p_double[i];
        if( state->hasbndl.ptr.p_bool[i]&&ae_fp_less_eq(x->ptr.p_double[i],state->bndl.ptr.p_double[i]) )
        {
            x->ptr.p_double[i] = state->bndl.ptr.p_double[i];
        }
        if( state->hasbndu.ptr.p_bool[i]&&ae_fp_greater_eq(x->ptr.p_double[i],state->bndu.ptr.p_double[i]) )
        {
            x->ptr.p_double[i] = state->bndu.ptr.p_double[i];
        }
    }
}


/*************************************************************************
This function solves QP problem of the form

        [                        ]
    min [ 0.5*c'*(G*inv(H)*G')*c ] s.t. c[i]>=0, SUM(c[i])=1.0
        [                        ]

where G is stored in SampleG[] array, diagonal H is stored in DiagH[].

DbgNCholesky is incremented every time we perform Cholesky decomposition.

  -- ALGLIB --
     Copyright 02.06.2015 by Bochkanov Sergey
*************************************************************************/
static void minns_solveqp(/* Real    */ ae_matrix* sampleg,
     /* Real    */ ae_vector* diagh,
     ae_int_t nsample,
     ae_int_t nvars,
     /* Real    */ ae_vector* coeffs,
     ae_int_t* dbgncholesky,
     minnsqp* state,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    double vv;
    ae_int_t n;
    ae_int_t nr;
    ae_int_t idx0;
    ae_int_t idx1;
    ae_int_t ncandbnd;
    ae_int_t innerits;
    ae_int_t outerits;
    double dnrm;
    double stp;
    double stpmax;
    ae_int_t actidx;
    double dtol;
    ae_bool kickneeded;
    ae_bool terminationneeded;
    double kicklength;
    double lambdav;
    double maxdiag;
    ae_bool wasactivation;
    ae_bool werechanges;
    ae_int_t termcnt;


    n = nsample;
    nr = nvars;
    
    /*
     * Allocate arrays, prepare data
     */
    rvectorsetlengthatleast(coeffs, n, _state);
    rvectorsetlengthatleast(&state->xc, n, _state);
    rvectorsetlengthatleast(&state->xn, n, _state);
    rvectorsetlengthatleast(&state->x0, n, _state);
    rvectorsetlengthatleast(&state->gc, n, _state);
    rvectorsetlengthatleast(&state->d, n, _state);
    rmatrixsetlengthatleast(&state->uh, n, n, _state);
    rmatrixsetlengthatleast(&state->ch, n, n, _state);
    rmatrixsetlengthatleast(&state->rk, nsample, nvars, _state);
    rvectorsetlengthatleast(&state->invutc, n, _state);
    rvectorsetlengthatleast(&state->tmp0, n, _state);
    bvectorsetlengthatleast(&state->tmpb, n, _state);
    for(i=0; i<=n-1; i++)
    {
        state->xc.ptr.p_double[i] = 1.0/n;
        coeffs->ptr.p_double[i] = 1.0/n;
    }
    for(i=0; i<=nsample-1; i++)
    {
        for(j=0; j<=nvars-1; j++)
        {
            state->rk.ptr.pp_double[i][j] = sampleg->ptr.pp_double[i][j]/ae_sqrt(diagh->ptr.p_double[j], _state);
        }
    }
    rmatrixsyrk(nsample, nvars, 1.0, &state->rk, 0, 0, 0, 0.0, &state->uh, 0, 0, ae_true, _state);
    maxdiag = 0.0;
    for(i=0; i<=nsample-1; i++)
    {
        maxdiag = ae_maxreal(maxdiag, state->uh.ptr.pp_double[i][i], _state);
    }
    maxdiag = coalesce(maxdiag, 1.0, _state);
    
    /*
     * Main cycle:
     */
    innerits = 0;
    outerits = 0;
    dtol = 1.0E5*ae_machineepsilon;
    kicklength = ae_machineepsilon;
    lambdav = 1.0E5*ae_machineepsilon;
    terminationneeded = ae_false;
    termcnt = 0;
    for(;;)
    {
        
        /*
         * Save current point to X0
         */
        ae_v_move(&state->x0.ptr.p_double[0], 1, &state->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
        
        /*
         * Calculate gradient at initial point, solve NNLS problem
         * to determine descent direction D subject to constraints.
         *
         * In order to do so we solve following constrained
         * minimization problem:
         *         (                         )^2
         *     min ( SUM(lambda[i]*A[i]) + G )
         *         (                         )
         * Here:
         * * G is a gradient (column vector)
         * * A[i] is a column vector of I-th constraint
         * * lambda[i] is a Lagrange multiplier corresponding to I-th constraint
         *
         * NOTE: all A[i] except for last one have only one element being set,
         *       so we rely on sparse capabilities of NNLS solver. However,
         *       in order to use these capabilities we have to reorder variables
         *       in such way that sparse ones come first.
         *
         * After finding lambda[] coefficients, we can find constrained descent
         * direction by subtracting lambda[i]*A[i] from D=-G. We make use of the
         * fact that first NCandBnd columns are just columns of identity matrix,
         * so we can perform exact projection by explicitly setting elements of D
         * to zeros.
         */
        minns_qpcalculategradfunc(sampleg, diagh, nsample, nvars, &state->xc, &state->gc, &state->fc, &state->tmp0, _state);
        ivectorsetlengthatleast(&state->tmpidx, n, _state);
        rvectorsetlengthatleast(&state->tmpd, n, _state);
        rmatrixsetlengthatleast(&state->tmpc2, n, 1, _state);
        idx0 = 0;
        ncandbnd = 0;
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_eq(state->xc.ptr.p_double[i],0.0) )
            {
                ncandbnd = ncandbnd+1;
            }
        }
        idx1 = ncandbnd;
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_eq(state->xc.ptr.p_double[i],0.0) )
            {
                
                /*
                 * Candidate for activation of boundary constraint,
                 * comes first.
                 *
                 * NOTE: multiplication by -1 is due to the fact that
                 *       it is lower bound, and has specific direction
                 *       of constraint gradient.
                 */
                state->tmpidx.ptr.p_int[idx0] = i;
                state->tmpd.ptr.p_double[idx0] = (-state->gc.ptr.p_double[i])*(-1);
                state->tmpc2.ptr.pp_double[idx0][0] = 1.0*(-1);
                idx0 = idx0+1;
            }
            else
            {
                
                /*
                 * We are far away from boundary.
                 */
                state->tmpidx.ptr.p_int[idx1] = i;
                state->tmpd.ptr.p_double[idx1] = -state->gc.ptr.p_double[i];
                state->tmpc2.ptr.pp_double[idx1][0] = 1.0;
                idx1 = idx1+1;
            }
        }
        ae_assert(idx0==ncandbnd, "MinNSQP: integrity check failed", _state);
        ae_assert(idx1==n, "MinNSQP: integrity check failed", _state);
        snnlsinit(n, 1, n, &state->nnls, _state);
        snnlssetproblem(&state->nnls, &state->tmpc2, &state->tmpd, ncandbnd, 1, n, _state);
        snnlsdropnnc(&state->nnls, ncandbnd, _state);
        snnlssolve(&state->nnls, &state->tmplambdas, _state);
        for(i=0; i<=n-1; i++)
        {
            state->d.ptr.p_double[i] = -state->gc.ptr.p_double[i]-state->tmplambdas.ptr.p_double[ncandbnd];
        }
        for(i=0; i<=ncandbnd-1; i++)
        {
            if( ae_fp_greater(state->tmplambdas.ptr.p_double[i],(double)(0)) )
            {
                state->d.ptr.p_double[state->tmpidx.ptr.p_int[i]] = 0.0;
            }
        }
        
        /*
         * Additional stage to "polish" D (improve situation
         * with sum-to-one constraint and boundary constraints)
         * and to perform additional integrity check.
         *
         * After this stage we are pretty sure that:
         * * if x[i]=0.0, then d[i]>=0.0
         * * if d[i]<0.0, then x[i]>0.0
         */
        v = 0.0;
        vv = 0.0;
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_eq(state->xc.ptr.p_double[i],0.0)&&ae_fp_less(state->d.ptr.p_double[i],0.0) )
            {
                state->d.ptr.p_double[i] = 0.0;
            }
            v = v+state->d.ptr.p_double[i];
            vv = ae_maxreal(vv, ae_fabs(state->gc.ptr.p_double[i], _state), _state);
        }
        ae_assert(ae_fp_less(ae_fabs(v, _state),1.0E5*ae_sqrt((double)(n), _state)*ae_machineepsilon*ae_maxreal(vv, 1.0, _state)), "MinNSQP: integrity check failed", _state);
        
        /*
         * Decide whether we need "kick" stage: special stage
         * that moves us away from boundary constraints which are
         * not strictly active (i.e. such constraints that x[i]=0.0 and d[i]>0).
         *
         * If we need kick stage, we make a kick - and restart iteration.
         * If not, after this block we can rely on the fact that
         * for all x[i]=0.0 we have d[i]=0.0
         */
        kickneeded = ae_false;
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_eq(state->xc.ptr.p_double[i],0.0)&&ae_fp_greater(state->d.ptr.p_double[i],0.0) )
            {
                kickneeded = ae_true;
            }
        }
        if( kickneeded )
        {
            
            /*
             * Perform kick.
             * Restart.
             * Do not increase outer iterations counter.
             */
            v = 0.0;
            for(i=0; i<=n-1; i++)
            {
                if( ae_fp_eq(state->xc.ptr.p_double[i],0.0)&&ae_fp_greater(state->d.ptr.p_double[i],0.0) )
                {
                    state->xc.ptr.p_double[i] = state->xc.ptr.p_double[i]+kicklength;
                }
                v = v+state->xc.ptr.p_double[i];
            }
            ae_assert(ae_fp_greater(v,0.0), "MinNSQP: integrity check failed", _state);
            for(i=0; i<=n-1; i++)
            {
                state->xc.ptr.p_double[i] = state->xc.ptr.p_double[i]/v;
            }
            inc(&innerits, _state);
            continue;
        }
        
        /*
         * Calculate Cholesky decomposition of constrained Hessian
         * for Newton phase.
         */
        for(;;)
        {
            for(i=0; i<=n-1; i++)
            {
                
                /*
                 * Diagonal element
                 */
                if( ae_fp_greater(state->xc.ptr.p_double[i],0.0) )
                {
                    state->ch.ptr.pp_double[i][i] = state->uh.ptr.pp_double[i][i]+lambdav*maxdiag;
                }
                else
                {
                    state->ch.ptr.pp_double[i][i] = 1.0;
                }
                
                /*
                 * Offdiagonal elements
                 */
                for(j=i+1; j<=n-1; j++)
                {
                    if( ae_fp_greater(state->xc.ptr.p_double[i],0.0)&&ae_fp_greater(state->xc.ptr.p_double[j],0.0) )
                    {
                        state->ch.ptr.pp_double[i][j] = state->uh.ptr.pp_double[i][j];
                    }
                    else
                    {
                        state->ch.ptr.pp_double[i][j] = 0.0;
                    }
                }
            }
            inc(dbgncholesky, _state);
            if( !spdmatrixcholeskyrec(&state->ch, 0, n, ae_true, &state->tmp0, _state) )
            {
                
                /*
                 * Cholesky decomposition failed.
                 * Increase LambdaV and repeat iteration.
                 * Do not increase outer iterations counter.
                 */
                lambdav = lambdav*10;
                continue;
            }
            break;
        }
        
        /*
         * Newton phase
         */
        for(;;)
        {
            
            /*
             * Calculate constrained (equality and sum-to-one) descent direction D.
             *
             * Here we use Sherman-Morrison update to calculate direction subject to
             * sum-to-one constraint.
             */
            minns_qpcalculategradfunc(sampleg, diagh, nsample, nvars, &state->xc, &state->gc, &state->fc, &state->tmp0, _state);
            for(i=0; i<=n-1; i++)
            {
                if( ae_fp_greater(state->xc.ptr.p_double[i],0.0) )
                {
                    state->invutc.ptr.p_double[i] = 1.0;
                    state->d.ptr.p_double[i] = -state->gc.ptr.p_double[i];
                }
                else
                {
                    state->invutc.ptr.p_double[i] = 0.0;
                    state->d.ptr.p_double[i] = 0.0;
                }
            }
            minns_qpsolveut(&state->ch, n, &state->invutc, _state);
            minns_qpsolveut(&state->ch, n, &state->d, _state);
            v = 0.0;
            vv = 0.0;
            for(i=0; i<=n-1; i++)
            {
                vv = vv+ae_sqr(state->invutc.ptr.p_double[i], _state);
                v = v+state->invutc.ptr.p_double[i]*state->d.ptr.p_double[i];
            }
            for(i=0; i<=n-1; i++)
            {
                state->d.ptr.p_double[i] = state->d.ptr.p_double[i]-v/vv*state->invutc.ptr.p_double[i];
            }
            minns_qpsolveu(&state->ch, n, &state->d, _state);
            v = 0.0;
            k = 0;
            for(i=0; i<=n-1; i++)
            {
                v = v+state->d.ptr.p_double[i];
                if( ae_fp_neq(state->d.ptr.p_double[i],0.0) )
                {
                    k = k+1;
                }
            }
            if( k>0&&ae_fp_greater(v,0.0) )
            {
                vv = v/k;
                for(i=0; i<=n-1; i++)
                {
                    if( ae_fp_neq(state->d.ptr.p_double[i],0.0) )
                    {
                        state->d.ptr.p_double[i] = state->d.ptr.p_double[i]-vv;
                    }
                }
            }
            
            /*
             * Calculate length of D, maximum step and component which is
             * activated by this step.
             *
             * Break if D is exactly zero. We do not break here if DNrm is
             * small - this check is performed later. It is important to
             * perform last step with nearly-zero D, it allows us to have
             * extra-precision in solution which is often needed for convergence
             * of AGS algorithm.
             */
            dnrm = 0.0;
            for(i=0; i<=n-1; i++)
            {
                dnrm = dnrm+ae_sqr(state->d.ptr.p_double[i], _state);
            }
            dnrm = ae_sqrt(dnrm, _state);
            actidx = -1;
            stpmax = 1.0E50;
            for(i=0; i<=n-1; i++)
            {
                if( ae_fp_less(state->d.ptr.p_double[i],0.0) )
                {
                    v = stpmax;
                    stpmax = safeminposrv(state->xc.ptr.p_double[i], -state->d.ptr.p_double[i], stpmax, _state);
                    if( ae_fp_less(stpmax,v) )
                    {
                        actidx = i;
                    }
                }
            }
            if( ae_fp_eq(dnrm,0.0) )
            {
                break;
            }
            
            /*
             * Calculate trial function value at unconstrained full step.
             * If trial value is greater or equal to FC, terminate iterations.
             */
            for(i=0; i<=n-1; i++)
            {
                state->xn.ptr.p_double[i] = state->xc.ptr.p_double[i]+1.0*state->d.ptr.p_double[i];
            }
            minns_qpcalculatefunc(sampleg, diagh, nsample, nvars, &state->xn, &state->fn, &state->tmp0, _state);
            if( ae_fp_greater_eq(state->fn,state->fc) )
            {
                break;
            }
            
            /*
             * Perform step
             * Update Hessian
             * Update XC
             *
             * Break if:
             * a) no constraint was activated
             * b) norm of D is small enough
             */
            stp = ae_minreal(1.0, stpmax, _state);
            for(i=0; i<=n-1; i++)
            {
                state->xn.ptr.p_double[i] = ae_maxreal(state->xc.ptr.p_double[i]+stp*state->d.ptr.p_double[i], 0.0, _state);
            }
            if( ae_fp_eq(stp,stpmax)&&actidx>=0 )
            {
                state->xn.ptr.p_double[actidx] = 0.0;
            }
            wasactivation = ae_false;
            for(i=0; i<=n-1; i++)
            {
                state->tmpb.ptr.p_bool[i] = ae_fp_eq(state->xn.ptr.p_double[i],0.0)&&ae_fp_neq(state->xc.ptr.p_double[i],0.0);
                wasactivation = wasactivation||state->tmpb.ptr.p_bool[i];
            }
            ae_v_move(&state->xc.ptr.p_double[0], 1, &state->xn.ptr.p_double[0], 1, ae_v_len(0,n-1));
            if( !wasactivation )
            {
                break;
            }
            if( ae_fp_less_eq(dnrm,dtol) )
            {
                break;
            }
            spdmatrixcholeskyupdatefixbuf(&state->ch, n, ae_true, &state->tmpb, &state->tmp0, _state);
        }
        
        /*
         * Compare status of boundary constraints - if nothing changed during
         * last outer iteration, TermCnt is increased. Otherwise it is reset
         * to zero.
         *
         * When TermCnt is large enough, we terminate algorithm.
         */
        werechanges = ae_false;
        for(i=0; i<=n-1; i++)
        {
            werechanges = werechanges||ae_sign(state->x0.ptr.p_double[i], _state)!=ae_sign(state->xc.ptr.p_double[i], _state);
        }
        if( !werechanges )
        {
            inc(&termcnt, _state);
        }
        else
        {
            termcnt = 0;
        }
        if( termcnt>=2 )
        {
            break;
        }
        
        /*
         * Increase number of outer iterations.
         * Break if we performed too many.
         */
        inc(&outerits, _state);
        if( outerits==10 )
        {
            break;
        }
    }
    
    /*
     * Store result
     */
    for(i=0; i<=n-1; i++)
    {
        coeffs->ptr.p_double[i] = state->xc.ptr.p_double[i];
    }
}


/*************************************************************************
Function/gradient calculation for QP solver.

  -- ALGLIB --
     Copyright 02.06.2015 by Bochkanov Sergey
*************************************************************************/
static void minns_qpcalculategradfunc(/* Real    */ ae_matrix* sampleg,
     /* Real    */ ae_vector* diagh,
     ae_int_t nsample,
     ae_int_t nvars,
     /* Real    */ ae_vector* coeffs,
     /* Real    */ ae_vector* g,
     double* f,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;

    *f = 0;

    rvectorsetlengthatleast(g, nsample, _state);
    rvectorsetlengthatleast(tmp, nvars, _state);
    
    /*
     * Calculate GS*p
     */
    for(j=0; j<=nvars-1; j++)
    {
        tmp->ptr.p_double[j] = 0.0;
    }
    for(i=0; i<=nsample-1; i++)
    {
        v = coeffs->ptr.p_double[i];
        ae_v_addd(&tmp->ptr.p_double[0], 1, &sampleg->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1), v);
    }
    
    /*
     * Calculate F
     */
    *f = 0.0;
    for(i=0; i<=nvars-1; i++)
    {
        *f = *f+0.5*ae_sqr(tmp->ptr.p_double[i], _state)/diagh->ptr.p_double[i];
    }
    
    /*
     * Multiply by inverse Hessian
     */
    for(i=0; i<=nvars-1; i++)
    {
        tmp->ptr.p_double[i] = tmp->ptr.p_double[i]/diagh->ptr.p_double[i];
    }
    
    /*
     * Function gradient
     */
    for(i=0; i<=nsample-1; i++)
    {
        v = ae_v_dotproduct(&sampleg->ptr.pp_double[i][0], 1, &tmp->ptr.p_double[0], 1, ae_v_len(0,nvars-1));
        g->ptr.p_double[i] = v;
    }
}


/*************************************************************************
Function calculation for QP solver.

  -- ALGLIB --
     Copyright 02.06.2015 by Bochkanov Sergey
*************************************************************************/
static void minns_qpcalculatefunc(/* Real    */ ae_matrix* sampleg,
     /* Real    */ ae_vector* diagh,
     ae_int_t nsample,
     ae_int_t nvars,
     /* Real    */ ae_vector* coeffs,
     double* f,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;

    *f = 0;

    rvectorsetlengthatleast(tmp, nvars, _state);
    
    /*
     * Calculate GS*p
     */
    for(j=0; j<=nvars-1; j++)
    {
        tmp->ptr.p_double[j] = 0.0;
    }
    for(i=0; i<=nsample-1; i++)
    {
        v = coeffs->ptr.p_double[i];
        ae_v_addd(&tmp->ptr.p_double[0], 1, &sampleg->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1), v);
    }
    
    /*
     * Calculate F
     */
    *f = 0.0;
    for(i=0; i<=nvars-1; i++)
    {
        *f = *f+0.5*ae_sqr(tmp->ptr.p_double[i], _state)/diagh->ptr.p_double[i];
    }
}


/*************************************************************************
Triangular solver for QP solver.

  -- ALGLIB --
     Copyright 02.06.2015 by Bochkanov Sergey
*************************************************************************/
static void minns_qpsolveu(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;


    
    /*
     * A^(-1)*X
     */
    for(i=n-1; i>=0; i--)
    {
        v = x->ptr.p_double[i];
        for(j=i+1; j<=n-1; j++)
        {
            v = v-a->ptr.pp_double[i][j]*x->ptr.p_double[j];
        }
        x->ptr.p_double[i] = v/a->ptr.pp_double[i][i];
    }
}


/*************************************************************************
Triangular solver for QP solver.

  -- ALGLIB --
     Copyright 02.06.2015 by Bochkanov Sergey
*************************************************************************/
static void minns_qpsolveut(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;


    
    /*
     * A^(-T)*X
     */
    for(i=0; i<=n-1; i++)
    {
        x->ptr.p_double[i] = x->ptr.p_double[i]/a->ptr.pp_double[i][i];
        v = x->ptr.p_double[i];
        for(j=i+1; j<=n-1; j++)
        {
            x->ptr.p_double[j] = x->ptr.p_double[j]-a->ptr.pp_double[i][j]*v;
        }
    }
}


void _minnsqp_init(void* _p, ae_state *_state)
{
    minnsqp *p = (minnsqp*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->xc, 0, DT_REAL, _state);
    ae_vector_init(&p->xn, 0, DT_REAL, _state);
    ae_vector_init(&p->x0, 0, DT_REAL, _state);
    ae_vector_init(&p->gc, 0, DT_REAL, _state);
    ae_vector_init(&p->d, 0, DT_REAL, _state);
    ae_matrix_init(&p->uh, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->ch, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->rk, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->invutc, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp0, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpidx, 0, DT_INT, _state);
    ae_vector_init(&p->tmpd, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpc, 0, DT_REAL, _state);
    ae_vector_init(&p->tmplambdas, 0, DT_REAL, _state);
    ae_matrix_init(&p->tmpc2, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpb, 0, DT_BOOL, _state);
    _snnlssolver_init(&p->nnls, _state);
}


void _minnsqp_init_copy(void* _dst, void* _src, ae_state *_state)
{
    minnsqp *dst = (minnsqp*)_dst;
    minnsqp *src = (minnsqp*)_src;
    dst->fc = src->fc;
    dst->fn = src->fn;
    ae_vector_init_copy(&dst->xc, &src->xc, _state);
    ae_vector_init_copy(&dst->xn, &src->xn, _state);
    ae_vector_init_copy(&dst->x0, &src->x0, _state);
    ae_vector_init_copy(&dst->gc, &src->gc, _state);
    ae_vector_init_copy(&dst->d, &src->d, _state);
    ae_matrix_init_copy(&dst->uh, &src->uh, _state);
    ae_matrix_init_copy(&dst->ch, &src->ch, _state);
    ae_matrix_init_copy(&dst->rk, &src->rk, _state);
    ae_vector_init_copy(&dst->invutc, &src->invutc, _state);
    ae_vector_init_copy(&dst->tmp0, &src->tmp0, _state);
    ae_vector_init_copy(&dst->tmpidx, &src->tmpidx, _state);
    ae_vector_init_copy(&dst->tmpd, &src->tmpd, _state);
    ae_vector_init_copy(&dst->tmpc, &src->tmpc, _state);
    ae_vector_init_copy(&dst->tmplambdas, &src->tmplambdas, _state);
    ae_matrix_init_copy(&dst->tmpc2, &src->tmpc2, _state);
    ae_vector_init_copy(&dst->tmpb, &src->tmpb, _state);
    _snnlssolver_init_copy(&dst->nnls, &src->nnls, _state);
}


void _minnsqp_clear(void* _p)
{
    minnsqp *p = (minnsqp*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->xc);
    ae_vector_clear(&p->xn);
    ae_vector_clear(&p->x0);
    ae_vector_clear(&p->gc);
    ae_vector_clear(&p->d);
    ae_matrix_clear(&p->uh);
    ae_matrix_clear(&p->ch);
    ae_matrix_clear(&p->rk);
    ae_vector_clear(&p->invutc);
    ae_vector_clear(&p->tmp0);
    ae_vector_clear(&p->tmpidx);
    ae_vector_clear(&p->tmpd);
    ae_vector_clear(&p->tmpc);
    ae_vector_clear(&p->tmplambdas);
    ae_matrix_clear(&p->tmpc2);
    ae_vector_clear(&p->tmpb);
    _snnlssolver_clear(&p->nnls);
}


void _minnsqp_destroy(void* _p)
{
    minnsqp *p = (minnsqp*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->xc);
    ae_vector_destroy(&p->xn);
    ae_vector_destroy(&p->x0);
    ae_vector_destroy(&p->gc);
    ae_vector_destroy(&p->d);
    ae_matrix_destroy(&p->uh);
    ae_matrix_destroy(&p->ch);
    ae_matrix_destroy(&p->rk);
    ae_vector_destroy(&p->invutc);
    ae_vector_destroy(&p->tmp0);
    ae_vector_destroy(&p->tmpidx);
    ae_vector_destroy(&p->tmpd);
    ae_vector_destroy(&p->tmpc);
    ae_vector_destroy(&p->tmplambdas);
    ae_matrix_destroy(&p->tmpc2);
    ae_vector_destroy(&p->tmpb);
    _snnlssolver_destroy(&p->nnls);
}


void _minnsstate_init(void* _p, ae_state *_state)
{
    minnsstate *p = (minnsstate*)_p;
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
    _rcommstate_init(&p->rstateags, _state);
    _hqrndstate_init(&p->agsrs, _state);
    ae_vector_init(&p->xstart, 0, DT_REAL, _state);
    ae_vector_init(&p->xc, 0, DT_REAL, _state);
    ae_vector_init(&p->xn, 0, DT_REAL, _state);
    ae_vector_init(&p->grs, 0, DT_REAL, _state);
    ae_vector_init(&p->d, 0, DT_REAL, _state);
    ae_vector_init(&p->colmax, 0, DT_REAL, _state);
    ae_vector_init(&p->diagh, 0, DT_REAL, _state);
    ae_vector_init(&p->signmin, 0, DT_REAL, _state);
    ae_vector_init(&p->signmax, 0, DT_REAL, _state);
    ae_vector_init(&p->scaledbndl, 0, DT_REAL, _state);
    ae_vector_init(&p->scaledbndu, 0, DT_REAL, _state);
    ae_matrix_init(&p->scaledcleic, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->rholinear, 0, DT_REAL, _state);
    ae_matrix_init(&p->samplex, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->samplegm, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->samplegmbc, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->samplef, 0, DT_REAL, _state);
    ae_vector_init(&p->samplef0, 0, DT_REAL, _state);
    _minnsqp_init(&p->nsqp, _state);
    ae_vector_init(&p->tmp0, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp1, 0, DT_REAL, _state);
    ae_matrix_init(&p->tmp2, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp3, 0, DT_INT, _state);
    ae_vector_init(&p->xbase, 0, DT_REAL, _state);
    ae_vector_init(&p->fp, 0, DT_REAL, _state);
    ae_vector_init(&p->fm, 0, DT_REAL, _state);
}


void _minnsstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    minnsstate *dst = (minnsstate*)_dst;
    minnsstate *src = (minnsstate*)_src;
    dst->solvertype = src->solvertype;
    dst->n = src->n;
    dst->epsx = src->epsx;
    dst->maxits = src->maxits;
    dst->xrep = src->xrep;
    dst->diffstep = src->diffstep;
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
    _rcommstate_init_copy(&dst->rstateags, &src->rstateags, _state);
    _hqrndstate_init_copy(&dst->agsrs, &src->agsrs, _state);
    dst->agsradius = src->agsradius;
    dst->agssamplesize = src->agssamplesize;
    dst->agsraddecay = src->agsraddecay;
    dst->agsalphadecay = src->agsalphadecay;
    dst->agsdecrease = src->agsdecrease;
    dst->agsinitstp = src->agsinitstp;
    dst->agsstattold = src->agsstattold;
    dst->agsshortstpabs = src->agsshortstpabs;
    dst->agsshortstprel = src->agsshortstprel;
    dst->agsshortf = src->agsshortf;
    dst->agsshortlimit = src->agsshortlimit;
    dst->agsrhononlinear = src->agsrhononlinear;
    dst->agsminupdate = src->agsminupdate;
    dst->agsmaxraddecays = src->agsmaxraddecays;
    dst->agsmaxbacktrack = src->agsmaxbacktrack;
    dst->agsmaxbacktracknonfull = src->agsmaxbacktracknonfull;
    dst->agspenaltylevel = src->agspenaltylevel;
    dst->agspenaltyincrease = src->agspenaltyincrease;
    ae_vector_init_copy(&dst->xstart, &src->xstart, _state);
    ae_vector_init_copy(&dst->xc, &src->xc, _state);
    ae_vector_init_copy(&dst->xn, &src->xn, _state);
    ae_vector_init_copy(&dst->grs, &src->grs, _state);
    ae_vector_init_copy(&dst->d, &src->d, _state);
    ae_vector_init_copy(&dst->colmax, &src->colmax, _state);
    ae_vector_init_copy(&dst->diagh, &src->diagh, _state);
    ae_vector_init_copy(&dst->signmin, &src->signmin, _state);
    ae_vector_init_copy(&dst->signmax, &src->signmax, _state);
    dst->userterminationneeded = src->userterminationneeded;
    ae_vector_init_copy(&dst->scaledbndl, &src->scaledbndl, _state);
    ae_vector_init_copy(&dst->scaledbndu, &src->scaledbndu, _state);
    ae_matrix_init_copy(&dst->scaledcleic, &src->scaledcleic, _state);
    ae_vector_init_copy(&dst->rholinear, &src->rholinear, _state);
    ae_matrix_init_copy(&dst->samplex, &src->samplex, _state);
    ae_matrix_init_copy(&dst->samplegm, &src->samplegm, _state);
    ae_matrix_init_copy(&dst->samplegmbc, &src->samplegmbc, _state);
    ae_vector_init_copy(&dst->samplef, &src->samplef, _state);
    ae_vector_init_copy(&dst->samplef0, &src->samplef0, _state);
    _minnsqp_init_copy(&dst->nsqp, &src->nsqp, _state);
    ae_vector_init_copy(&dst->tmp0, &src->tmp0, _state);
    ae_vector_init_copy(&dst->tmp1, &src->tmp1, _state);
    ae_matrix_init_copy(&dst->tmp2, &src->tmp2, _state);
    ae_vector_init_copy(&dst->tmp3, &src->tmp3, _state);
    ae_vector_init_copy(&dst->xbase, &src->xbase, _state);
    ae_vector_init_copy(&dst->fp, &src->fp, _state);
    ae_vector_init_copy(&dst->fm, &src->fm, _state);
    dst->repinneriterationscount = src->repinneriterationscount;
    dst->repouteriterationscount = src->repouteriterationscount;
    dst->repnfev = src->repnfev;
    dst->repvaridx = src->repvaridx;
    dst->repfuncidx = src->repfuncidx;
    dst->repterminationtype = src->repterminationtype;
    dst->replcerr = src->replcerr;
    dst->repnlcerr = src->repnlcerr;
    dst->dbgncholesky = src->dbgncholesky;
}


void _minnsstate_clear(void* _p)
{
    minnsstate *p = (minnsstate*)_p;
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
    _rcommstate_clear(&p->rstateags);
    _hqrndstate_clear(&p->agsrs);
    ae_vector_clear(&p->xstart);
    ae_vector_clear(&p->xc);
    ae_vector_clear(&p->xn);
    ae_vector_clear(&p->grs);
    ae_vector_clear(&p->d);
    ae_vector_clear(&p->colmax);
    ae_vector_clear(&p->diagh);
    ae_vector_clear(&p->signmin);
    ae_vector_clear(&p->signmax);
    ae_vector_clear(&p->scaledbndl);
    ae_vector_clear(&p->scaledbndu);
    ae_matrix_clear(&p->scaledcleic);
    ae_vector_clear(&p->rholinear);
    ae_matrix_clear(&p->samplex);
    ae_matrix_clear(&p->samplegm);
    ae_matrix_clear(&p->samplegmbc);
    ae_vector_clear(&p->samplef);
    ae_vector_clear(&p->samplef0);
    _minnsqp_clear(&p->nsqp);
    ae_vector_clear(&p->tmp0);
    ae_vector_clear(&p->tmp1);
    ae_matrix_clear(&p->tmp2);
    ae_vector_clear(&p->tmp3);
    ae_vector_clear(&p->xbase);
    ae_vector_clear(&p->fp);
    ae_vector_clear(&p->fm);
}


void _minnsstate_destroy(void* _p)
{
    minnsstate *p = (minnsstate*)_p;
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
    _rcommstate_destroy(&p->rstateags);
    _hqrndstate_destroy(&p->agsrs);
    ae_vector_destroy(&p->xstart);
    ae_vector_destroy(&p->xc);
    ae_vector_destroy(&p->xn);
    ae_vector_destroy(&p->grs);
    ae_vector_destroy(&p->d);
    ae_vector_destroy(&p->colmax);
    ae_vector_destroy(&p->diagh);
    ae_vector_destroy(&p->signmin);
    ae_vector_destroy(&p->signmax);
    ae_vector_destroy(&p->scaledbndl);
    ae_vector_destroy(&p->scaledbndu);
    ae_matrix_destroy(&p->scaledcleic);
    ae_vector_destroy(&p->rholinear);
    ae_matrix_destroy(&p->samplex);
    ae_matrix_destroy(&p->samplegm);
    ae_matrix_destroy(&p->samplegmbc);
    ae_vector_destroy(&p->samplef);
    ae_vector_destroy(&p->samplef0);
    _minnsqp_destroy(&p->nsqp);
    ae_vector_destroy(&p->tmp0);
    ae_vector_destroy(&p->tmp1);
    ae_matrix_destroy(&p->tmp2);
    ae_vector_destroy(&p->tmp3);
    ae_vector_destroy(&p->xbase);
    ae_vector_destroy(&p->fp);
    ae_vector_destroy(&p->fm);
}


void _minnsreport_init(void* _p, ae_state *_state)
{
    minnsreport *p = (minnsreport*)_p;
    ae_touch_ptr((void*)p);
}


void _minnsreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    minnsreport *dst = (minnsreport*)_dst;
    minnsreport *src = (minnsreport*)_src;
    dst->iterationscount = src->iterationscount;
    dst->nfev = src->nfev;
    dst->cerr = src->cerr;
    dst->lcerr = src->lcerr;
    dst->nlcerr = src->nlcerr;
    dst->terminationtype = src->terminationtype;
    dst->varidx = src->varidx;
    dst->funcidx = src->funcidx;
}


void _minnsreport_clear(void* _p)
{
    minnsreport *p = (minnsreport*)_p;
    ae_touch_ptr((void*)p);
}


void _minnsreport_destroy(void* _p)
{
    minnsreport *p = (minnsreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
