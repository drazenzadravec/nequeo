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
#include "minlbfgs.h"


/*$ Declarations $*/
static double minlbfgs_gtol = 0.4;
static void minlbfgs_clearrequestfields(minlbfgsstate* state,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
        LIMITED MEMORY BFGS METHOD FOR LARGE SCALE OPTIMIZATION

DESCRIPTION:
The subroutine minimizes function F(x) of N arguments by  using  a  quasi-
Newton method (LBFGS scheme) which is optimized to use  a  minimum  amount
of memory.
The subroutine generates the approximation of an inverse Hessian matrix by
using information about the last M steps of the algorithm  (instead of N).
It lessens a required amount of memory from a value  of  order  N^2  to  a
value of order 2*N*M.


REQUIREMENTS:
Algorithm will request following information during its operation:
* function value F and its gradient G (simultaneously) at given point X


USAGE:
1. User initializes algorithm state with MinLBFGSCreate() call
2. User tunes solver parameters with MinLBFGSSetCond() MinLBFGSSetStpMax()
   and other functions
3. User calls MinLBFGSOptimize() function which takes algorithm  state and
   pointer (delegate, etc.) to callback function which calculates F/G.
4. User calls MinLBFGSResults() to get solution
5. Optionally user may call MinLBFGSRestartFrom() to solve another problem
   with same N/M but another starting point and/or another function.
   MinLBFGSRestartFrom() allows to reuse already initialized structure.


INPUT PARAMETERS:
    N       -   problem dimension. N>0
    M       -   number of corrections in the BFGS scheme of Hessian
                approximation update. Recommended value:  3<=M<=7. The smaller
                value causes worse convergence, the bigger will  not  cause  a
                considerably better convergence, but will cause a fall in  the
                performance. M<=N.
    X       -   initial solution approximation, array[0..N-1].


OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state
    

NOTES:
1. you may tune stopping conditions with MinLBFGSSetCond() function
2. if target function contains exp() or other fast growing functions,  and
   optimization algorithm makes too large steps which leads  to  overflow,
   use MinLBFGSSetStpMax() function to bound algorithm's  steps.  However,
   L-BFGS rarely needs such a tuning.


  -- ALGLIB --
     Copyright 02.04.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgscreate(ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_vector* x,
     minlbfgsstate* state,
     ae_state *_state)
{

    _minlbfgsstate_clear(state);

    ae_assert(n>=1, "MinLBFGSCreate: N<1!", _state);
    ae_assert(m>=1, "MinLBFGSCreate: M<1", _state);
    ae_assert(m<=n, "MinLBFGSCreate: M>N", _state);
    ae_assert(x->cnt>=n, "MinLBFGSCreate: Length(X)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "MinLBFGSCreate: X contains infinite or NaN values!", _state);
    minlbfgscreatex(n, m, x, 0, 0.0, state, _state);
}


/*************************************************************************
The subroutine is finite difference variant of MinLBFGSCreate().  It  uses
finite differences in order to differentiate target function.

Description below contains information which is specific to  this function
only. We recommend to read comments on MinLBFGSCreate() in  order  to  get
more information about creation of LBFGS optimizer.

INPUT PARAMETERS:
    N       -   problem dimension, N>0:
                * if given, only leading N elements of X are used
                * if not given, automatically determined from size of X
    M       -   number of corrections in the BFGS scheme of Hessian
                approximation update. Recommended value:  3<=M<=7. The smaller
                value causes worse convergence, the bigger will  not  cause  a
                considerably better convergence, but will cause a fall in  the
                performance. M<=N.
    X       -   starting point, array[0..N-1].
    DiffStep-   differentiation step, >0

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

NOTES:
1. algorithm uses 4-point central formula for differentiation.
2. differentiation step along I-th axis is equal to DiffStep*S[I] where
   S[] is scaling vector which can be set by MinLBFGSSetScale() call.
3. we recommend you to use moderate values of  differentiation  step.  Too
   large step will result in too large truncation  errors, while too small
   step will result in too large numerical  errors.  1.0E-6  can  be  good
   value to start with.
4. Numerical  differentiation  is   very   inefficient  -   one   gradient
   calculation needs 4*N function evaluations. This function will work for
   any N - either small (1...10), moderate (10...100) or  large  (100...).
   However, performance penalty will be too severe for any N's except  for
   small ones.
   We should also say that code which relies on numerical  differentiation
   is   less  robust  and  precise.  LBFGS  needs  exact  gradient values.
   Imprecise gradient may slow  down  convergence,  especially  on  highly
   nonlinear problems.
   Thus  we  recommend to use this function for fast prototyping on small-
   dimensional problems only, and to implement analytical gradient as soon
   as possible.

  -- ALGLIB --
     Copyright 16.05.2011 by Bochkanov Sergey
*************************************************************************/
void minlbfgscreatef(ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_vector* x,
     double diffstep,
     minlbfgsstate* state,
     ae_state *_state)
{

    _minlbfgsstate_clear(state);

    ae_assert(n>=1, "MinLBFGSCreateF: N too small!", _state);
    ae_assert(m>=1, "MinLBFGSCreateF: M<1", _state);
    ae_assert(m<=n, "MinLBFGSCreateF: M>N", _state);
    ae_assert(x->cnt>=n, "MinLBFGSCreateF: Length(X)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "MinLBFGSCreateF: X contains infinite or NaN values!", _state);
    ae_assert(ae_isfinite(diffstep, _state), "MinLBFGSCreateF: DiffStep is infinite or NaN!", _state);
    ae_assert(ae_fp_greater(diffstep,(double)(0)), "MinLBFGSCreateF: DiffStep is non-positive!", _state);
    minlbfgscreatex(n, m, x, 0, diffstep, state, _state);
}


/*************************************************************************
This function sets stopping conditions for L-BFGS optimization algorithm.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    EpsG    -   >=0
                The  subroutine  finishes  its  work   if   the  condition
                |v|<EpsG is satisfied, where:
                * |.| means Euclidian norm
                * v - scaled gradient vector, v[i]=g[i]*s[i]
                * g - gradient
                * s - scaling coefficients set by MinLBFGSSetScale()
    EpsF    -   >=0
                The  subroutine  finishes  its work if on k+1-th iteration
                the  condition  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
                is satisfied.
    EpsX    -   >=0
                The subroutine finishes its work if  on  k+1-th  iteration
                the condition |v|<=EpsX is fulfilled, where:
                * |.| means Euclidian norm
                * v - scaled step vector, v[i]=dx[i]/s[i]
                * dx - ste pvector, dx=X(k+1)-X(k)
                * s - scaling coefficients set by MinLBFGSSetScale()
    MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                iterations is unlimited.

Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
automatic stopping criterion selection (small EpsX).

  -- ALGLIB --
     Copyright 02.04.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgssetcond(minlbfgsstate* state,
     double epsg,
     double epsf,
     double epsx,
     ae_int_t maxits,
     ae_state *_state)
{


    ae_assert(ae_isfinite(epsg, _state), "MinLBFGSSetCond: EpsG is not finite number!", _state);
    ae_assert(ae_fp_greater_eq(epsg,(double)(0)), "MinLBFGSSetCond: negative EpsG!", _state);
    ae_assert(ae_isfinite(epsf, _state), "MinLBFGSSetCond: EpsF is not finite number!", _state);
    ae_assert(ae_fp_greater_eq(epsf,(double)(0)), "MinLBFGSSetCond: negative EpsF!", _state);
    ae_assert(ae_isfinite(epsx, _state), "MinLBFGSSetCond: EpsX is not finite number!", _state);
    ae_assert(ae_fp_greater_eq(epsx,(double)(0)), "MinLBFGSSetCond: negative EpsX!", _state);
    ae_assert(maxits>=0, "MinLBFGSSetCond: negative MaxIts!", _state);
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
This function turns on/off reporting.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    NeedXRep-   whether iteration reports are needed or not

If NeedXRep is True, algorithm will call rep() callback function if  it is
provided to MinLBFGSOptimize().


  -- ALGLIB --
     Copyright 02.04.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgssetxrep(minlbfgsstate* state,
     ae_bool needxrep,
     ae_state *_state)
{


    state->xrep = needxrep;
}


/*************************************************************************
This function sets maximum step length

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    StpMax  -   maximum step length, >=0. Set StpMax to 0.0 (default),  if
                you don't want to limit step length.

Use this subroutine when you optimize target function which contains exp()
or  other  fast  growing  functions,  and optimization algorithm makes too
large  steps  which  leads  to overflow. This function allows us to reject
steps  that  are  too  large  (and  therefore  expose  us  to the possible
overflow) without actually calculating function value at the x+stp*d.

  -- ALGLIB --
     Copyright 02.04.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgssetstpmax(minlbfgsstate* state,
     double stpmax,
     ae_state *_state)
{


    ae_assert(ae_isfinite(stpmax, _state), "MinLBFGSSetStpMax: StpMax is not finite!", _state);
    ae_assert(ae_fp_greater_eq(stpmax,(double)(0)), "MinLBFGSSetStpMax: StpMax<0!", _state);
    state->stpmax = stpmax;
}


/*************************************************************************
This function sets scaling coefficients for LBFGS optimizer.

ALGLIB optimizers use scaling matrices to test stopping  conditions  (step
size and gradient are scaled before comparison with tolerances).  Scale of
the I-th variable is a translation invariant measure of:
a) "how large" the variable is
b) how large the step should be to make significant changes in the function

Scaling is also used by finite difference variant of the optimizer  - step
along I-th axis is equal to DiffStep*S[I].

In  most  optimizers  (and  in  the  LBFGS  too)  scaling is NOT a form of
preconditioning. It just  affects  stopping  conditions.  You  should  set
preconditioner  by  separate  call  to  one  of  the  MinLBFGSSetPrec...()
functions.

There  is  special  preconditioning  mode, however,  which  uses   scaling
coefficients to form diagonal preconditioning matrix. You  can  turn  this
mode on, if you want.   But  you should understand that scaling is not the
same thing as preconditioning - these are two different, although  related
forms of tuning solver.

INPUT PARAMETERS:
    State   -   structure stores algorithm state
    S       -   array[N], non-zero scaling coefficients
                S[i] may be negative, sign doesn't matter.

  -- ALGLIB --
     Copyright 14.01.2011 by Bochkanov Sergey
*************************************************************************/
void minlbfgssetscale(minlbfgsstate* state,
     /* Real    */ ae_vector* s,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(s->cnt>=state->n, "MinLBFGSSetScale: Length(S)<N", _state);
    for(i=0; i<=state->n-1; i++)
    {
        ae_assert(ae_isfinite(s->ptr.p_double[i], _state), "MinLBFGSSetScale: S contains infinite or NAN elements", _state);
        ae_assert(ae_fp_neq(s->ptr.p_double[i],(double)(0)), "MinLBFGSSetScale: S contains zero elements", _state);
        state->s.ptr.p_double[i] = ae_fabs(s->ptr.p_double[i], _state);
    }
}


/*************************************************************************
Extended subroutine for internal use only.

Accepts additional parameters:

    Flags - additional settings:
            * Flags = 0     means no additional settings
            * Flags = 1     "do not allocate memory". used when solving
                            a many subsequent tasks with  same N/M  values.
                            First  call MUST  be without this flag bit set,
                            subsequent  calls   of   MinLBFGS   with   same
                            MinLBFGSState structure can set Flags to 1.
    DiffStep - numerical differentiation step

  -- ALGLIB --
     Copyright 02.04.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgscreatex(ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_vector* x,
     ae_int_t flags,
     double diffstep,
     minlbfgsstate* state,
     ae_state *_state)
{
    ae_bool allocatemem;
    ae_int_t i;


    ae_assert(n>=1, "MinLBFGS: N too small!", _state);
    ae_assert(m>=1, "MinLBFGS: M too small!", _state);
    ae_assert(m<=n, "MinLBFGS: M too large!", _state);
    
    /*
     * Initialize
     */
    state->teststep = (double)(0);
    state->diffstep = diffstep;
    state->n = n;
    state->m = m;
    allocatemem = flags%2==0;
    flags = flags/2;
    if( allocatemem )
    {
        ae_vector_set_length(&state->rho, m, _state);
        ae_vector_set_length(&state->theta, m, _state);
        ae_matrix_set_length(&state->yk, m, n, _state);
        ae_matrix_set_length(&state->sk, m, n, _state);
        ae_vector_set_length(&state->d, n, _state);
        ae_vector_set_length(&state->xp, n, _state);
        ae_vector_set_length(&state->x, n, _state);
        ae_vector_set_length(&state->s, n, _state);
        ae_vector_set_length(&state->g, n, _state);
        ae_vector_set_length(&state->work, n, _state);
    }
    minlbfgssetcond(state, (double)(0), (double)(0), (double)(0), 0, _state);
    minlbfgssetxrep(state, ae_false, _state);
    minlbfgssetstpmax(state, (double)(0), _state);
    minlbfgsrestartfrom(state, x, _state);
    for(i=0; i<=n-1; i++)
    {
        state->s.ptr.p_double[i] = 1.0;
    }
    state->prectype = 0;
}


/*************************************************************************
Modification  of  the  preconditioner:  default  preconditioner    (simple
scaling, same for all elements of X) is used.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state

NOTE:  you  can  change  preconditioner  "on  the  fly",  during algorithm
iterations.

  -- ALGLIB --
     Copyright 13.10.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgssetprecdefault(minlbfgsstate* state, ae_state *_state)
{


    state->prectype = 0;
}


/*************************************************************************
Modification of the preconditioner: Cholesky factorization of  approximate
Hessian is used.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    P       -   triangular preconditioner, Cholesky factorization of
                the approximate Hessian. array[0..N-1,0..N-1],
                (if larger, only leading N elements are used).
    IsUpper -   whether upper or lower triangle of P is given
                (other triangle is not referenced)

After call to this function preconditioner is changed to P  (P  is  copied
into the internal buffer).

NOTE:  you  can  change  preconditioner  "on  the  fly",  during algorithm
iterations.

NOTE 2:  P  should  be nonsingular. Exception will be thrown otherwise.

  -- ALGLIB --
     Copyright 13.10.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgssetpreccholesky(minlbfgsstate* state,
     /* Real    */ ae_matrix* p,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t i;
    double mx;


    ae_assert(isfinitertrmatrix(p, state->n, isupper, _state), "MinLBFGSSetPrecCholesky: P contains infinite or NAN values!", _state);
    mx = (double)(0);
    for(i=0; i<=state->n-1; i++)
    {
        mx = ae_maxreal(mx, ae_fabs(p->ptr.pp_double[i][i], _state), _state);
    }
    ae_assert(ae_fp_greater(mx,(double)(0)), "MinLBFGSSetPrecCholesky: P is strictly singular!", _state);
    if( state->denseh.rows<state->n||state->denseh.cols<state->n )
    {
        ae_matrix_set_length(&state->denseh, state->n, state->n, _state);
    }
    state->prectype = 1;
    if( isupper )
    {
        rmatrixcopy(state->n, state->n, p, 0, 0, &state->denseh, 0, 0, _state);
    }
    else
    {
        rmatrixtranspose(state->n, state->n, p, 0, 0, &state->denseh, 0, 0, _state);
    }
}


/*************************************************************************
Modification  of  the  preconditioner:  diagonal of approximate Hessian is
used.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    D       -   diagonal of the approximate Hessian, array[0..N-1],
                (if larger, only leading N elements are used).

NOTE:  you  can  change  preconditioner  "on  the  fly",  during algorithm
iterations.

NOTE 2: D[i] should be positive. Exception will be thrown otherwise.

NOTE 3: you should pass diagonal of approximate Hessian - NOT ITS INVERSE.

  -- ALGLIB --
     Copyright 13.10.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgssetprecdiag(minlbfgsstate* state,
     /* Real    */ ae_vector* d,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(d->cnt>=state->n, "MinLBFGSSetPrecDiag: D is too short", _state);
    for(i=0; i<=state->n-1; i++)
    {
        ae_assert(ae_isfinite(d->ptr.p_double[i], _state), "MinLBFGSSetPrecDiag: D contains infinite or NAN elements", _state);
        ae_assert(ae_fp_greater(d->ptr.p_double[i],(double)(0)), "MinLBFGSSetPrecDiag: D contains non-positive elements", _state);
    }
    rvectorsetlengthatleast(&state->diagh, state->n, _state);
    state->prectype = 2;
    for(i=0; i<=state->n-1; i++)
    {
        state->diagh.ptr.p_double[i] = d->ptr.p_double[i];
    }
}


/*************************************************************************
Modification of the preconditioner: scale-based diagonal preconditioning.

This preconditioning mode can be useful when you  don't  have  approximate
diagonal of Hessian, but you know that your  variables  are  badly  scaled
(for  example,  one  variable is in [1,10], and another in [1000,100000]),
and most part of the ill-conditioning comes from different scales of vars.

In this case simple  scale-based  preconditioner,  with H[i] = 1/(s[i]^2),
can greatly improve convergence.

IMPRTANT: you should set scale of your variables  with  MinLBFGSSetScale()
call  (before  or after MinLBFGSSetPrecScale() call). Without knowledge of
the scale of your variables scale-based preconditioner will be  just  unit
matrix.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 13.10.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgssetprecscale(minlbfgsstate* state, ae_state *_state)
{


    state->prectype = 3;
}


/*************************************************************************
This function sets low-rank preconditioner for Hessian matrix  H=D+W'*C*W,
where:
* H is a Hessian matrix, which is approximated by D/W/C
* D is a NxN diagonal positive definite matrix
* W is a KxN low-rank correction
* C is a KxK positive definite diagonal factor of low-rank correction

This preconditioner is inexact but fast - it requires O(N*K)  time  to  be
applied. Preconditioner P is calculated by artificially constructing a set
of BFGS updates which tries to reproduce behavior of H:
* Sk = Wk (k-th row of W)
* Yk = (D+Wk'*Ck*Wk)*Sk
* Yk/Sk are reordered by ascending of C[k]*norm(Wk)^2

Here we assume that rows of Wk are orthogonal or nearly orthogonal,  which
allows us to have O(N*K+K^2) update instead of O(N*K^2) one. Reordering of
updates is essential for having good performance on non-orthogonal problems
(updates which do not add much of curvature are added first,  and  updates
which add very large eigenvalues are added last and override effect of the
first updates).

In practice, this preconditioner is perfect when ortogonal  correction  is
applied; on non-orthogonal problems sometimes  it  allows  to  achieve  5x
speedup (when compared to non-preconditioned solver).

  -- ALGLIB --
     Copyright 13.10.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgssetprecrankklbfgsfast(minlbfgsstate* state,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_matrix* w,
     ae_int_t cnt,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;


    n = state->n;
    state->prectype = 4;
    state->preck = cnt;
    rvectorsetlengthatleast(&state->precc, cnt, _state);
    rvectorsetlengthatleast(&state->precd, n, _state);
    rmatrixsetlengthatleast(&state->precw, cnt, n, _state);
    for(i=0; i<=n-1; i++)
    {
        state->precd.ptr.p_double[i] = d->ptr.p_double[i];
    }
    for(i=0; i<=cnt-1; i++)
    {
        state->precc.ptr.p_double[i] = c->ptr.p_double[i];
        for(j=0; j<=n-1; j++)
        {
            state->precw.ptr.pp_double[i][j] = w->ptr.pp_double[i][j];
        }
    }
}


/*************************************************************************
This function  sets  exact  low-rank  preconditioner  for  Hessian  matrix
H=D+W'*C*W, where:
* H is a Hessian matrix, which is approximated by D/W/C
* D is a NxN diagonal positive definite matrix
* W is a KxN low-rank correction
* C is a KxK semidefinite diagonal factor of low-rank correction

This preconditioner is exact but slow - it requires O(N*K^2)  time  to  be
built and O(N*K) time to be applied. Woodbury matrix identity is  used  to
build inverse matrix.

  -- ALGLIB --
     Copyright 13.10.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgssetpreclowrankexact(minlbfgsstate* state,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_matrix* w,
     ae_int_t cnt,
     ae_state *_state)
{


    state->prectype = 5;
    preparelowrankpreconditioner(d, c, w, state->n, cnt, &state->lowrankbuf, _state);
}


/*************************************************************************
NOTES:

1. This function has two different implementations: one which  uses  exact
   (analytical) user-supplied gradient,  and one which uses function value
   only  and  numerically  differentiates  function  in  order  to  obtain
   gradient.

   Depending  on  the  specific  function  used to create optimizer object
   (either MinLBFGSCreate() for analytical gradient  or  MinLBFGSCreateF()
   for numerical differentiation) you should choose appropriate variant of
   MinLBFGSOptimize() - one  which  accepts  function  AND gradient or one
   which accepts function ONLY.

   Be careful to choose variant of MinLBFGSOptimize() which corresponds to
   your optimization scheme! Table below lists different  combinations  of
   callback (function/gradient) passed to MinLBFGSOptimize()  and specific
   function used to create optimizer.


                     |         USER PASSED TO MinLBFGSOptimize()
   CREATED WITH      |  function only   |  function and gradient
   ------------------------------------------------------------
   MinLBFGSCreateF() |     work                FAIL
   MinLBFGSCreate()  |     FAIL                work

   Here "FAIL" denotes inappropriate combinations  of  optimizer  creation
   function  and  MinLBFGSOptimize()  version.   Attemps   to   use   such
   combination (for example, to create optimizer with MinLBFGSCreateF() and
   to pass gradient information to MinCGOptimize()) will lead to exception
   being thrown. Either  you  did  not pass gradient when it WAS needed or
   you passed gradient when it was NOT needed.

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool minlbfgsiteration(minlbfgsstate* state, ae_state *_state)
{
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    ae_int_t j;
    ae_int_t ic;
    ae_int_t mcinfo;
    double v;
    double vv;
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
        j = state->rstate.ia.ptr.p_int[3];
        ic = state->rstate.ia.ptr.p_int[4];
        mcinfo = state->rstate.ia.ptr.p_int[5];
        v = state->rstate.ra.ptr.p_double[0];
        vv = state->rstate.ra.ptr.p_double[1];
    }
    else
    {
        n = -983;
        m = -989;
        i = -834;
        j = 900;
        ic = -287;
        mcinfo = 364;
        v = 214;
        vv = -338;
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
    if( state->rstate.stage==9 )
    {
        goto lbl_9;
    }
    if( state->rstate.stage==10 )
    {
        goto lbl_10;
    }
    if( state->rstate.stage==11 )
    {
        goto lbl_11;
    }
    if( state->rstate.stage==12 )
    {
        goto lbl_12;
    }
    if( state->rstate.stage==13 )
    {
        goto lbl_13;
    }
    if( state->rstate.stage==14 )
    {
        goto lbl_14;
    }
    if( state->rstate.stage==15 )
    {
        goto lbl_15;
    }
    if( state->rstate.stage==16 )
    {
        goto lbl_16;
    }
    
    /*
     * Routine body
     */
    
    /*
     * Unload frequently used variables from State structure
     * (just for typing convinience)
     */
    n = state->n;
    m = state->m;
    state->userterminationneeded = ae_false;
    state->repterminationtype = 0;
    state->repiterationscount = 0;
    state->repvaridx = -1;
    state->repnfev = 0;
    
    /*
     *  Check, that transferred derivative value is right
     */
    minlbfgs_clearrequestfields(state, _state);
    if( !(ae_fp_eq(state->diffstep,(double)(0))&&ae_fp_greater(state->teststep,(double)(0))) )
    {
        goto lbl_17;
    }
    state->needfg = ae_true;
    i = 0;
lbl_19:
    if( i>n-1 )
    {
        goto lbl_21;
    }
    v = state->x.ptr.p_double[i];
    state->x.ptr.p_double[i] = v-state->teststep*state->s.ptr.p_double[i];
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    state->fm1 = state->f;
    state->fp1 = state->g.ptr.p_double[i];
    state->x.ptr.p_double[i] = v+state->teststep*state->s.ptr.p_double[i];
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    state->fm2 = state->f;
    state->fp2 = state->g.ptr.p_double[i];
    state->x.ptr.p_double[i] = v;
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    
    /*
     * 2*State.TestStep   -   scale parameter
     * width of segment [Xi-TestStep;Xi+TestStep]
     */
    if( !derivativecheck(state->fm1, state->fp1, state->fm2, state->fp2, state->f, state->g.ptr.p_double[i], 2*state->teststep, _state) )
    {
        state->repvaridx = i;
        state->repterminationtype = -7;
        result = ae_false;
        return result;
    }
    i = i+1;
    goto lbl_19;
lbl_21:
    state->needfg = ae_false;
lbl_17:
    
    /*
     * Calculate F/G at the initial point
     */
    minlbfgs_clearrequestfields(state, _state);
    if( ae_fp_neq(state->diffstep,(double)(0)) )
    {
        goto lbl_22;
    }
    state->needfg = ae_true;
    state->rstate.stage = 3;
    goto lbl_rcomm;
lbl_3:
    state->needfg = ae_false;
    goto lbl_23;
lbl_22:
    state->needf = ae_true;
    state->rstate.stage = 4;
    goto lbl_rcomm;
lbl_4:
    state->fbase = state->f;
    i = 0;
lbl_24:
    if( i>n-1 )
    {
        goto lbl_26;
    }
    v = state->x.ptr.p_double[i];
    state->x.ptr.p_double[i] = v-state->diffstep*state->s.ptr.p_double[i];
    state->rstate.stage = 5;
    goto lbl_rcomm;
lbl_5:
    state->fm2 = state->f;
    state->x.ptr.p_double[i] = v-0.5*state->diffstep*state->s.ptr.p_double[i];
    state->rstate.stage = 6;
    goto lbl_rcomm;
lbl_6:
    state->fm1 = state->f;
    state->x.ptr.p_double[i] = v+0.5*state->diffstep*state->s.ptr.p_double[i];
    state->rstate.stage = 7;
    goto lbl_rcomm;
lbl_7:
    state->fp1 = state->f;
    state->x.ptr.p_double[i] = v+state->diffstep*state->s.ptr.p_double[i];
    state->rstate.stage = 8;
    goto lbl_rcomm;
lbl_8:
    state->fp2 = state->f;
    state->x.ptr.p_double[i] = v;
    state->g.ptr.p_double[i] = (8*(state->fp1-state->fm1)-(state->fp2-state->fm2))/(6*state->diffstep*state->s.ptr.p_double[i]);
    i = i+1;
    goto lbl_24;
lbl_26:
    state->f = state->fbase;
    state->needf = ae_false;
lbl_23:
    trimprepare(state->f, &state->trimthreshold, _state);
    if( !state->xrep )
    {
        goto lbl_27;
    }
    minlbfgs_clearrequestfields(state, _state);
    state->xupdated = ae_true;
    state->rstate.stage = 9;
    goto lbl_rcomm;
lbl_9:
    state->xupdated = ae_false;
lbl_27:
    if( state->userterminationneeded )
    {
        
        /*
         * User requested termination
         */
        state->repterminationtype = 8;
        result = ae_false;
        return result;
    }
    state->repnfev = 1;
    state->fold = state->f;
    v = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        v = v+ae_sqr(state->g.ptr.p_double[i]*state->s.ptr.p_double[i], _state);
    }
    if( ae_fp_less_eq(ae_sqrt(v, _state),state->epsg) )
    {
        state->repterminationtype = 4;
        result = ae_false;
        return result;
    }
    
    /*
     * Choose initial step and direction.
     * Apply preconditioner, if we have something other than default.
     */
    ae_v_moveneg(&state->d.ptr.p_double[0], 1, &state->g.ptr.p_double[0], 1, ae_v_len(0,n-1));
    if( state->prectype==0 )
    {
        
        /*
         * Default preconditioner is used, but we can't use it before iterations will start
         */
        v = ae_v_dotproduct(&state->g.ptr.p_double[0], 1, &state->g.ptr.p_double[0], 1, ae_v_len(0,n-1));
        v = ae_sqrt(v, _state);
        if( ae_fp_eq(state->stpmax,(double)(0)) )
        {
            state->stp = ae_minreal(1.0/v, (double)(1), _state);
        }
        else
        {
            state->stp = ae_minreal(1.0/v, state->stpmax, _state);
        }
    }
    if( state->prectype==1 )
    {
        
        /*
         * Cholesky preconditioner is used
         */
        fblscholeskysolve(&state->denseh, 1.0, n, ae_true, &state->d, &state->autobuf, _state);
        state->stp = (double)(1);
    }
    if( state->prectype==2 )
    {
        
        /*
         * diagonal approximation is used
         */
        for(i=0; i<=n-1; i++)
        {
            state->d.ptr.p_double[i] = state->d.ptr.p_double[i]/state->diagh.ptr.p_double[i];
        }
        state->stp = (double)(1);
    }
    if( state->prectype==3 )
    {
        
        /*
         * scale-based preconditioner is used
         */
        for(i=0; i<=n-1; i++)
        {
            state->d.ptr.p_double[i] = state->d.ptr.p_double[i]*state->s.ptr.p_double[i]*state->s.ptr.p_double[i];
        }
        state->stp = (double)(1);
    }
    if( state->prectype==4 )
    {
        
        /*
         * rank-k BFGS-based preconditioner is used
         */
        inexactlbfgspreconditioner(&state->d, n, &state->precd, &state->precc, &state->precw, state->preck, &state->precbuf, _state);
        state->stp = (double)(1);
    }
    if( state->prectype==5 )
    {
        
        /*
         * exact low-rank preconditioner is used
         */
        applylowrankpreconditioner(&state->d, &state->lowrankbuf, _state);
        state->stp = (double)(1);
    }
    
    /*
     * Main cycle
     */
    state->k = 0;
lbl_29:
    if( ae_false )
    {
        goto lbl_30;
    }
    
    /*
     * Main cycle: prepare to 1-D line search
     */
    state->p = state->k%m;
    state->q = ae_minint(state->k, m-1, _state);
    
    /*
     * Store X[k], G[k]
     */
    ae_v_move(&state->xp.ptr.p_double[0], 1, &state->x.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_moveneg(&state->sk.ptr.pp_double[state->p][0], 1, &state->x.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_moveneg(&state->yk.ptr.pp_double[state->p][0], 1, &state->g.ptr.p_double[0], 1, ae_v_len(0,n-1));
    
    /*
     * Minimize F(x+alpha*d)
     * Calculate S[k], Y[k]
     */
    state->mcstage = 0;
    if( state->k!=0 )
    {
        state->stp = 1.0;
    }
    linminnormalized(&state->d, &state->stp, n, _state);
    mcsrch(n, &state->x, &state->f, &state->g, &state->d, &state->stp, state->stpmax, minlbfgs_gtol, &mcinfo, &state->nfev, &state->work, &state->lstate, &state->mcstage, _state);
lbl_31:
    if( state->mcstage==0 )
    {
        goto lbl_32;
    }
    minlbfgs_clearrequestfields(state, _state);
    if( ae_fp_neq(state->diffstep,(double)(0)) )
    {
        goto lbl_33;
    }
    state->needfg = ae_true;
    state->rstate.stage = 10;
    goto lbl_rcomm;
lbl_10:
    state->needfg = ae_false;
    goto lbl_34;
lbl_33:
    state->needf = ae_true;
    state->rstate.stage = 11;
    goto lbl_rcomm;
lbl_11:
    state->fbase = state->f;
    i = 0;
lbl_35:
    if( i>n-1 )
    {
        goto lbl_37;
    }
    v = state->x.ptr.p_double[i];
    state->x.ptr.p_double[i] = v-state->diffstep*state->s.ptr.p_double[i];
    state->rstate.stage = 12;
    goto lbl_rcomm;
lbl_12:
    state->fm2 = state->f;
    state->x.ptr.p_double[i] = v-0.5*state->diffstep*state->s.ptr.p_double[i];
    state->rstate.stage = 13;
    goto lbl_rcomm;
lbl_13:
    state->fm1 = state->f;
    state->x.ptr.p_double[i] = v+0.5*state->diffstep*state->s.ptr.p_double[i];
    state->rstate.stage = 14;
    goto lbl_rcomm;
lbl_14:
    state->fp1 = state->f;
    state->x.ptr.p_double[i] = v+state->diffstep*state->s.ptr.p_double[i];
    state->rstate.stage = 15;
    goto lbl_rcomm;
lbl_15:
    state->fp2 = state->f;
    state->x.ptr.p_double[i] = v;
    state->g.ptr.p_double[i] = (8*(state->fp1-state->fm1)-(state->fp2-state->fm2))/(6*state->diffstep*state->s.ptr.p_double[i]);
    i = i+1;
    goto lbl_35;
lbl_37:
    state->f = state->fbase;
    state->needf = ae_false;
lbl_34:
    trimfunction(&state->f, &state->g, n, state->trimthreshold, _state);
    mcsrch(n, &state->x, &state->f, &state->g, &state->d, &state->stp, state->stpmax, minlbfgs_gtol, &mcinfo, &state->nfev, &state->work, &state->lstate, &state->mcstage, _state);
    goto lbl_31;
lbl_32:
    if( state->userterminationneeded )
    {
        
        /*
         * User requested termination.
         * Restore previous point and return.
         */
        ae_v_move(&state->x.ptr.p_double[0], 1, &state->xp.ptr.p_double[0], 1, ae_v_len(0,n-1));
        state->repterminationtype = 8;
        result = ae_false;
        return result;
    }
    if( !state->xrep )
    {
        goto lbl_38;
    }
    
    /*
     * report
     */
    minlbfgs_clearrequestfields(state, _state);
    state->xupdated = ae_true;
    state->rstate.stage = 16;
    goto lbl_rcomm;
lbl_16:
    state->xupdated = ae_false;
lbl_38:
    state->repnfev = state->repnfev+state->nfev;
    state->repiterationscount = state->repiterationscount+1;
    ae_v_add(&state->sk.ptr.pp_double[state->p][0], 1, &state->x.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_add(&state->yk.ptr.pp_double[state->p][0], 1, &state->g.ptr.p_double[0], 1, ae_v_len(0,n-1));
    
    /*
     * Stopping conditions
     */
    v = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        v = v+ae_sqr(state->g.ptr.p_double[i]*state->s.ptr.p_double[i], _state);
    }
    if( !ae_isfinite(v, _state)||!ae_isfinite(state->f, _state) )
    {
        
        /*
         * Abnormal termination - infinities in function/gradient
         */
        state->repterminationtype = -8;
        result = ae_false;
        return result;
    }
    if( state->repiterationscount>=state->maxits&&state->maxits>0 )
    {
        
        /*
         * Too many iterations
         */
        state->repterminationtype = 5;
        result = ae_false;
        return result;
    }
    if( ae_fp_less_eq(ae_sqrt(v, _state),state->epsg) )
    {
        
        /*
         * Gradient is small enough
         */
        state->repterminationtype = 4;
        result = ae_false;
        return result;
    }
    if( ae_fp_less_eq(state->fold-state->f,state->epsf*ae_maxreal(ae_fabs(state->fold, _state), ae_maxreal(ae_fabs(state->f, _state), 1.0, _state), _state)) )
    {
        
        /*
         * F(k+1)-F(k) is small enough
         */
        state->repterminationtype = 1;
        result = ae_false;
        return result;
    }
    v = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        v = v+ae_sqr(state->sk.ptr.pp_double[state->p][i]/state->s.ptr.p_double[i], _state);
    }
    if( ae_fp_less_eq(ae_sqrt(v, _state),state->epsx) )
    {
        
        /*
         * X(k+1)-X(k) is small enough
         */
        state->repterminationtype = 2;
        result = ae_false;
        return result;
    }
    
    /*
     * If Wolfe conditions are satisfied, we can update
     * limited memory model.
     *
     * However, if conditions are not satisfied (NFEV limit is met,
     * function is too wild, ...), we'll skip L-BFGS update
     */
    if( mcinfo!=1 )
    {
        
        /*
         * Skip update.
         *
         * In such cases we'll initialize search direction by
         * antigradient vector, because it  leads to more
         * transparent code with less number of special cases
         */
        state->fold = state->f;
        ae_v_moveneg(&state->d.ptr.p_double[0], 1, &state->g.ptr.p_double[0], 1, ae_v_len(0,n-1));
    }
    else
    {
        
        /*
         * Calculate Rho[k], GammaK
         */
        v = ae_v_dotproduct(&state->yk.ptr.pp_double[state->p][0], 1, &state->sk.ptr.pp_double[state->p][0], 1, ae_v_len(0,n-1));
        vv = ae_v_dotproduct(&state->yk.ptr.pp_double[state->p][0], 1, &state->yk.ptr.pp_double[state->p][0], 1, ae_v_len(0,n-1));
        if( ae_fp_eq(v,(double)(0))||ae_fp_eq(vv,(double)(0)) )
        {
            
            /*
             * Rounding errors make further iterations impossible.
             */
            state->repterminationtype = -2;
            result = ae_false;
            return result;
        }
        state->rho.ptr.p_double[state->p] = 1/v;
        state->gammak = v/vv;
        
        /*
         *  Calculate d(k+1) = -H(k+1)*g(k+1)
         *
         *  for I:=K downto K-Q do
         *      V = s(i)^T * work(iteration:I)
         *      theta(i) = V
         *      work(iteration:I+1) = work(iteration:I) - V*Rho(i)*y(i)
         *  work(last iteration) = H0*work(last iteration) - preconditioner
         *  for I:=K-Q to K do
         *      V = y(i)^T*work(iteration:I)
         *      work(iteration:I+1) = work(iteration:I) +(-V+theta(i))*Rho(i)*s(i)
         *
         *  NOW WORK CONTAINS d(k+1)
         */
        ae_v_move(&state->work.ptr.p_double[0], 1, &state->g.ptr.p_double[0], 1, ae_v_len(0,n-1));
        for(i=state->k; i>=state->k-state->q; i--)
        {
            ic = i%m;
            v = ae_v_dotproduct(&state->sk.ptr.pp_double[ic][0], 1, &state->work.ptr.p_double[0], 1, ae_v_len(0,n-1));
            state->theta.ptr.p_double[ic] = v;
            vv = v*state->rho.ptr.p_double[ic];
            ae_v_subd(&state->work.ptr.p_double[0], 1, &state->yk.ptr.pp_double[ic][0], 1, ae_v_len(0,n-1), vv);
        }
        if( state->prectype==0 )
        {
            
            /*
             * Simple preconditioner is used
             */
            v = state->gammak;
            ae_v_muld(&state->work.ptr.p_double[0], 1, ae_v_len(0,n-1), v);
        }
        if( state->prectype==1 )
        {
            
            /*
             * Cholesky preconditioner is used
             */
            fblscholeskysolve(&state->denseh, (double)(1), n, ae_true, &state->work, &state->autobuf, _state);
        }
        if( state->prectype==2 )
        {
            
            /*
             * diagonal approximation is used
             */
            for(i=0; i<=n-1; i++)
            {
                state->work.ptr.p_double[i] = state->work.ptr.p_double[i]/state->diagh.ptr.p_double[i];
            }
        }
        if( state->prectype==3 )
        {
            
            /*
             * scale-based preconditioner is used
             */
            for(i=0; i<=n-1; i++)
            {
                state->work.ptr.p_double[i] = state->work.ptr.p_double[i]*state->s.ptr.p_double[i]*state->s.ptr.p_double[i];
            }
        }
        if( state->prectype==4 )
        {
            
            /*
             * Rank-K BFGS-based preconditioner is used
             */
            inexactlbfgspreconditioner(&state->work, n, &state->precd, &state->precc, &state->precw, state->preck, &state->precbuf, _state);
        }
        if( state->prectype==5 )
        {
            
            /*
             * Exact low-rank preconditioner is used
             */
            applylowrankpreconditioner(&state->work, &state->lowrankbuf, _state);
        }
        for(i=state->k-state->q; i<=state->k; i++)
        {
            ic = i%m;
            v = ae_v_dotproduct(&state->yk.ptr.pp_double[ic][0], 1, &state->work.ptr.p_double[0], 1, ae_v_len(0,n-1));
            vv = state->rho.ptr.p_double[ic]*(-v+state->theta.ptr.p_double[ic]);
            ae_v_addd(&state->work.ptr.p_double[0], 1, &state->sk.ptr.pp_double[ic][0], 1, ae_v_len(0,n-1), vv);
        }
        ae_v_moveneg(&state->d.ptr.p_double[0], 1, &state->work.ptr.p_double[0], 1, ae_v_len(0,n-1));
        
        /*
         * Next step
         */
        state->fold = state->f;
        state->k = state->k+1;
    }
    goto lbl_29;
lbl_30:
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
    state->rstate.ia.ptr.p_int[3] = j;
    state->rstate.ia.ptr.p_int[4] = ic;
    state->rstate.ia.ptr.p_int[5] = mcinfo;
    state->rstate.ra.ptr.p_double[0] = v;
    state->rstate.ra.ptr.p_double[1] = vv;
    return result;
}


/*************************************************************************
L-BFGS algorithm results

INPUT PARAMETERS:
    State   -   algorithm state

OUTPUT PARAMETERS:
    X       -   array[0..N-1], solution
    Rep     -   optimization report:
                * Rep.TerminationType completetion code:
                    * -8    internal integrity control  detected  infinite
                            or NAN values in  function/gradient.  Abnormal
                            termination signalled.
                    * -7    gradient verification failed.
                            See MinLBFGSSetGradientCheck() for more information.
                    * -2    rounding errors prevent further improvement.
                            X contains best point found.
                    * -1    incorrect parameters were specified
                    *  1    relative function improvement is no more than
                            EpsF.
                    *  2    relative step is no more than EpsX.
                    *  4    gradient norm is no more than EpsG
                    *  5    MaxIts steps was taken
                    *  7    stopping conditions are too stringent,
                            further improvement is impossible
                    *  8    terminated by user who called minlbfgsrequesttermination().
                            X contains point which was "current accepted" when
                            termination request was submitted.
                * Rep.IterationsCount contains iterations count
                * NFEV countains number of function calculations

  -- ALGLIB --
     Copyright 02.04.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgsresults(minlbfgsstate* state,
     /* Real    */ ae_vector* x,
     minlbfgsreport* rep,
     ae_state *_state)
{

    ae_vector_clear(x);
    _minlbfgsreport_clear(rep);

    minlbfgsresultsbuf(state, x, rep, _state);
}


/*************************************************************************
L-BFGS algorithm results

Buffered implementation of MinLBFGSResults which uses pre-allocated buffer
to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
intended to be used in the inner cycles of performance critical algorithms
where array reallocation penalty is too large to be ignored.

  -- ALGLIB --
     Copyright 20.08.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgsresultsbuf(minlbfgsstate* state,
     /* Real    */ ae_vector* x,
     minlbfgsreport* rep,
     ae_state *_state)
{


    if( x->cnt<state->n )
    {
        ae_vector_set_length(x, state->n, _state);
    }
    ae_v_move(&x->ptr.p_double[0], 1, &state->x.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    rep->iterationscount = state->repiterationscount;
    rep->nfev = state->repnfev;
    rep->varidx = state->repvaridx;
    rep->terminationtype = state->repterminationtype;
}


/*************************************************************************
This  subroutine restarts LBFGS algorithm from new point. All optimization
parameters are left unchanged.

This  function  allows  to  solve multiple  optimization  problems  (which
must have same number of dimensions) without object reallocation penalty.

INPUT PARAMETERS:
    State   -   structure used to store algorithm state
    X       -   new starting point.

  -- ALGLIB --
     Copyright 30.07.2010 by Bochkanov Sergey
*************************************************************************/
void minlbfgsrestartfrom(minlbfgsstate* state,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{


    ae_assert(x->cnt>=state->n, "MinLBFGSRestartFrom: Length(X)<N!", _state);
    ae_assert(isfinitevector(x, state->n, _state), "MinLBFGSRestartFrom: X contains infinite or NaN values!", _state);
    ae_v_move(&state->x.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    ae_vector_set_length(&state->rstate.ia, 5+1, _state);
    ae_vector_set_length(&state->rstate.ra, 1+1, _state);
    state->rstate.stage = -1;
    minlbfgs_clearrequestfields(state, _state);
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
     Copyright 08.10.2014 by Bochkanov Sergey
*************************************************************************/
void minlbfgsrequesttermination(minlbfgsstate* state, ae_state *_state)
{


    state->userterminationneeded = ae_true;
}


/*************************************************************************
This  subroutine  turns  on  verification  of  the  user-supplied analytic
gradient:
* user calls this subroutine before optimization begins
* MinLBFGSOptimize() is called
* prior to  actual  optimization, for each component  of  parameters being
  optimized X[i] algorithm performs following steps:
  * two trial steps are made to X[i]-TestStep*S[i] and X[i]+TestStep*S[i],
    where X[i] is i-th component of the initial point and S[i] is a  scale
    of i-th parameter
  * if needed, steps are bounded with respect to constraints on X[]
  * F(X) is evaluated at these trial points
  * we perform one more evaluation in the middle point of the interval
  * we  build  cubic  model using function values and derivatives at trial
    points and we compare its prediction with actual value in  the  middle
    point
  * in case difference between prediction and actual value is higher  than
    some predetermined threshold, algorithm stops with completion code -7;
    Rep.VarIdx is set to index of the parameter with incorrect derivative.
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
        parameters by means of setting scale with MinLBFGSSetScale().

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
     Copyright 24.05.2012 by Bochkanov Sergey
*************************************************************************/
void minlbfgssetgradientcheck(minlbfgsstate* state,
     double teststep,
     ae_state *_state)
{


    ae_assert(ae_isfinite(teststep, _state), "MinLBFGSSetGradientCheck: TestStep contains NaN or Infinite", _state);
    ae_assert(ae_fp_greater_eq(teststep,(double)(0)), "MinLBFGSSetGradientCheck: invalid argument TestStep(TestStep<0)", _state);
    state->teststep = teststep;
}


/*************************************************************************
Clears request fileds (to be sure that we don't forgot to clear something)
*************************************************************************/
static void minlbfgs_clearrequestfields(minlbfgsstate* state,
     ae_state *_state)
{


    state->needf = ae_false;
    state->needfg = ae_false;
    state->xupdated = ae_false;
}


void _minlbfgsstate_init(void* _p, ae_state *_state)
{
    minlbfgsstate *p = (minlbfgsstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->s, 0, DT_REAL, _state);
    ae_vector_init(&p->rho, 0, DT_REAL, _state);
    ae_matrix_init(&p->yk, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->sk, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->xp, 0, DT_REAL, _state);
    ae_vector_init(&p->theta, 0, DT_REAL, _state);
    ae_vector_init(&p->d, 0, DT_REAL, _state);
    ae_vector_init(&p->work, 0, DT_REAL, _state);
    ae_matrix_init(&p->denseh, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->diagh, 0, DT_REAL, _state);
    ae_vector_init(&p->precc, 0, DT_REAL, _state);
    ae_vector_init(&p->precd, 0, DT_REAL, _state);
    ae_matrix_init(&p->precw, 0, 0, DT_REAL, _state);
    _precbuflbfgs_init(&p->precbuf, _state);
    _precbuflowrank_init(&p->lowrankbuf, _state);
    ae_vector_init(&p->autobuf, 0, DT_REAL, _state);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->g, 0, DT_REAL, _state);
    _rcommstate_init(&p->rstate, _state);
    _linminstate_init(&p->lstate, _state);
}


void _minlbfgsstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    minlbfgsstate *dst = (minlbfgsstate*)_dst;
    minlbfgsstate *src = (minlbfgsstate*)_src;
    dst->n = src->n;
    dst->m = src->m;
    dst->epsg = src->epsg;
    dst->epsf = src->epsf;
    dst->epsx = src->epsx;
    dst->maxits = src->maxits;
    dst->xrep = src->xrep;
    dst->stpmax = src->stpmax;
    ae_vector_init_copy(&dst->s, &src->s, _state);
    dst->diffstep = src->diffstep;
    dst->nfev = src->nfev;
    dst->mcstage = src->mcstage;
    dst->k = src->k;
    dst->q = src->q;
    dst->p = src->p;
    ae_vector_init_copy(&dst->rho, &src->rho, _state);
    ae_matrix_init_copy(&dst->yk, &src->yk, _state);
    ae_matrix_init_copy(&dst->sk, &src->sk, _state);
    ae_vector_init_copy(&dst->xp, &src->xp, _state);
    ae_vector_init_copy(&dst->theta, &src->theta, _state);
    ae_vector_init_copy(&dst->d, &src->d, _state);
    dst->stp = src->stp;
    ae_vector_init_copy(&dst->work, &src->work, _state);
    dst->fold = src->fold;
    dst->trimthreshold = src->trimthreshold;
    dst->prectype = src->prectype;
    dst->gammak = src->gammak;
    ae_matrix_init_copy(&dst->denseh, &src->denseh, _state);
    ae_vector_init_copy(&dst->diagh, &src->diagh, _state);
    ae_vector_init_copy(&dst->precc, &src->precc, _state);
    ae_vector_init_copy(&dst->precd, &src->precd, _state);
    ae_matrix_init_copy(&dst->precw, &src->precw, _state);
    dst->preck = src->preck;
    _precbuflbfgs_init_copy(&dst->precbuf, &src->precbuf, _state);
    _precbuflowrank_init_copy(&dst->lowrankbuf, &src->lowrankbuf, _state);
    dst->fbase = src->fbase;
    dst->fm2 = src->fm2;
    dst->fm1 = src->fm1;
    dst->fp1 = src->fp1;
    dst->fp2 = src->fp2;
    ae_vector_init_copy(&dst->autobuf, &src->autobuf, _state);
    ae_vector_init_copy(&dst->x, &src->x, _state);
    dst->f = src->f;
    ae_vector_init_copy(&dst->g, &src->g, _state);
    dst->needf = src->needf;
    dst->needfg = src->needfg;
    dst->xupdated = src->xupdated;
    dst->userterminationneeded = src->userterminationneeded;
    dst->teststep = src->teststep;
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
    dst->repiterationscount = src->repiterationscount;
    dst->repnfev = src->repnfev;
    dst->repvaridx = src->repvaridx;
    dst->repterminationtype = src->repterminationtype;
    _linminstate_init_copy(&dst->lstate, &src->lstate, _state);
}


void _minlbfgsstate_clear(void* _p)
{
    minlbfgsstate *p = (minlbfgsstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->s);
    ae_vector_clear(&p->rho);
    ae_matrix_clear(&p->yk);
    ae_matrix_clear(&p->sk);
    ae_vector_clear(&p->xp);
    ae_vector_clear(&p->theta);
    ae_vector_clear(&p->d);
    ae_vector_clear(&p->work);
    ae_matrix_clear(&p->denseh);
    ae_vector_clear(&p->diagh);
    ae_vector_clear(&p->precc);
    ae_vector_clear(&p->precd);
    ae_matrix_clear(&p->precw);
    _precbuflbfgs_clear(&p->precbuf);
    _precbuflowrank_clear(&p->lowrankbuf);
    ae_vector_clear(&p->autobuf);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->g);
    _rcommstate_clear(&p->rstate);
    _linminstate_clear(&p->lstate);
}


void _minlbfgsstate_destroy(void* _p)
{
    minlbfgsstate *p = (minlbfgsstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->s);
    ae_vector_destroy(&p->rho);
    ae_matrix_destroy(&p->yk);
    ae_matrix_destroy(&p->sk);
    ae_vector_destroy(&p->xp);
    ae_vector_destroy(&p->theta);
    ae_vector_destroy(&p->d);
    ae_vector_destroy(&p->work);
    ae_matrix_destroy(&p->denseh);
    ae_vector_destroy(&p->diagh);
    ae_vector_destroy(&p->precc);
    ae_vector_destroy(&p->precd);
    ae_matrix_destroy(&p->precw);
    _precbuflbfgs_destroy(&p->precbuf);
    _precbuflowrank_destroy(&p->lowrankbuf);
    ae_vector_destroy(&p->autobuf);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->g);
    _rcommstate_destroy(&p->rstate);
    _linminstate_destroy(&p->lstate);
}


void _minlbfgsreport_init(void* _p, ae_state *_state)
{
    minlbfgsreport *p = (minlbfgsreport*)_p;
    ae_touch_ptr((void*)p);
}


void _minlbfgsreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    minlbfgsreport *dst = (minlbfgsreport*)_dst;
    minlbfgsreport *src = (minlbfgsreport*)_src;
    dst->iterationscount = src->iterationscount;
    dst->nfev = src->nfev;
    dst->varidx = src->varidx;
    dst->terminationtype = src->terminationtype;
}


void _minlbfgsreport_clear(void* _p)
{
    minlbfgsreport *p = (minlbfgsreport*)_p;
    ae_touch_ptr((void*)p);
}


void _minlbfgsreport_destroy(void* _p)
{
    minlbfgsreport *p = (minlbfgsreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
