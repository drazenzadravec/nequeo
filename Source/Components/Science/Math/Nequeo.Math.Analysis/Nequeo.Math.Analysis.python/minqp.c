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
#include "minqp.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
                    CONSTRAINED QUADRATIC PROGRAMMING

The subroutine creates QP optimizer. After initial creation,  it  contains
default optimization problem with zero quadratic and linear terms  and  no
constraints. You should set quadratic/linear terms with calls to functions
provided by MinQP subpackage.

You should also choose appropriate QP solver and set it  and  its stopping
criteria by means of MinQPSetAlgo??????() function. Then, you should start
solution process by means of MinQPOptimize() call. Solution itself can  be
obtained with MinQPResults() function.

INPUT PARAMETERS:
    N       -   problem size
    
OUTPUT PARAMETERS:
    State   -   optimizer with zero quadratic/linear terms
                and no constraints

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpcreate(ae_int_t n, minqpstate* state, ae_state *_state)
{
    ae_int_t i;

    _minqpstate_clear(state);

    ae_assert(n>=1, "MinQPCreate: N<1", _state);
    
    /*
     * initialize QP solver
     */
    state->n = n;
    state->nec = 0;
    state->nic = 0;
    state->repterminationtype = 0;
    state->absamax = (double)(1);
    state->absasum = (double)(1);
    state->absasum2 = (double)(1);
    state->akind = 0;
    state->sparseaupper = ae_false;
    cqminit(n, &state->a, _state);
    ae_vector_set_length(&state->b, n, _state);
    ae_vector_set_length(&state->bndl, n, _state);
    ae_vector_set_length(&state->bndu, n, _state);
    ae_vector_set_length(&state->havebndl, n, _state);
    ae_vector_set_length(&state->havebndu, n, _state);
    ae_vector_set_length(&state->s, n, _state);
    ae_vector_set_length(&state->startx, n, _state);
    ae_vector_set_length(&state->xorigin, n, _state);
    ae_vector_set_length(&state->xs, n, _state);
    for(i=0; i<=n-1; i++)
    {
        state->bndl.ptr.p_double[i] = _state->v_neginf;
        state->bndu.ptr.p_double[i] = _state->v_posinf;
        state->havebndl.ptr.p_bool[i] = ae_false;
        state->havebndu.ptr.p_bool[i] = ae_false;
        state->b.ptr.p_double[i] = 0.0;
        state->startx.ptr.p_double[i] = 0.0;
        state->xorigin.ptr.p_double[i] = 0.0;
        state->s.ptr.p_double[i] = 1.0;
    }
    state->havex = ae_false;
    minqpsetalgocholesky(state, _state);
    normestimatorcreate(n, n, 5, 5, &state->estimator, _state);
    qqploaddefaults(n, &state->qqpsettingsuser, _state);
    qpbleicloaddefaults(n, &state->qpbleicsettingsuser, _state);
    state->qpbleicfirstcall = ae_true;
}


/*************************************************************************
This function sets linear term for QP solver.

By default, linear term is zero.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    B       -   linear term, array[N].

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetlinearterm(minqpstate* state,
     /* Real    */ ae_vector* b,
     ae_state *_state)
{
    ae_int_t n;


    n = state->n;
    ae_assert(b->cnt>=n, "MinQPSetLinearTerm: Length(B)<N", _state);
    ae_assert(isfinitevector(b, n, _state), "MinQPSetLinearTerm: B contains infinite or NaN elements", _state);
    minqpsetlineartermfast(state, b, _state);
}


/*************************************************************************
This  function  sets  dense  quadratic  term  for  QP solver. By  default,
quadratic term is zero.

SUPPORT BY ALGLIB QP ALGORITHMS:

Dense quadratic term can be handled by any of the QP algorithms  supported
by ALGLIB QP Solver.

IMPORTANT:

This solver minimizes following  function:
    f(x) = 0.5*x'*A*x + b'*x.
Note that quadratic term has 0.5 before it. So if  you  want  to  minimize
    f(x) = x^2 + x
you should rewrite your problem as follows:
    f(x) = 0.5*(2*x^2) + x
and your matrix A will be equal to [[2.0]], not to [[1.0]]

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    A       -   matrix, array[N,N]
    IsUpper -   (optional) storage type:
                * if True, symmetric matrix  A  is  given  by  its  upper
                  triangle, and the lower triangle isn’t used
                * if False, symmetric matrix  A  is  given  by  its lower
                  triangle, and the upper triangle isn’t used
                * if not given, both lower and upper  triangles  must  be
                  filled.

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetquadraticterm(minqpstate* state,
     /* Real    */ ae_matrix* a,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t n;


    n = state->n;
    ae_assert(a->rows>=n, "MinQPSetQuadraticTerm: Rows(A)<N", _state);
    ae_assert(a->cols>=n, "MinQPSetQuadraticTerm: Cols(A)<N", _state);
    ae_assert(isfinitertrmatrix(a, n, isupper, _state), "MinQPSetQuadraticTerm: A contains infinite or NaN elements", _state);
    minqpsetquadratictermfast(state, a, isupper, 0.0, _state);
}


/*************************************************************************
This  function  sets  sparse  quadratic  term  for  QP solver. By default,
quadratic term is zero.

IMPORTANT:

This solver minimizes following  function:
    f(x) = 0.5*x'*A*x + b'*x.
Note that quadratic term has 0.5 before it. So if  you  want  to  minimize
    f(x) = x^2 + x
you should rewrite your problem as follows:
    f(x) = 0.5*(2*x^2) + x
and your matrix A will be equal to [[2.0]], not to [[1.0]]

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    A       -   matrix, array[N,N]
    IsUpper -   (optional) storage type:
                * if True, symmetric matrix  A  is  given  by  its  upper
                  triangle, and the lower triangle isn’t used
                * if False, symmetric matrix  A  is  given  by  its lower
                  triangle, and the upper triangle isn’t used
                * if not given, both lower and upper  triangles  must  be
                  filled.

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetquadratictermsparse(minqpstate* state,
     sparsematrix* a,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t t0;
    ae_int_t t1;
    ae_int_t i;
    ae_int_t j;
    double v;


    n = state->n;
    ae_assert(sparsegetnrows(a, _state)==n, "MinQPSetQuadraticTermSparse: Rows(A)<>N", _state);
    ae_assert(sparsegetncols(a, _state)==n, "MinQPSetQuadraticTermSparse: Cols(A)<>N", _state);
    sparsecopytocrsbuf(a, &state->sparsea, _state);
    state->sparseaupper = isupper;
    state->akind = 1;
    
    /*
     * Estimate norm of A
     * (it will be used later in the quadratic penalty function)
     */
    state->absamax = (double)(0);
    state->absasum = (double)(0);
    state->absasum2 = (double)(0);
    t0 = 0;
    t1 = 0;
    while(sparseenumerate(a, &t0, &t1, &i, &j, &v, _state))
    {
        if( i==j )
        {
            
            /*
             * Diagonal terms are counted only once
             */
            state->absamax = ae_maxreal(state->absamax, v, _state);
            state->absasum = state->absasum+v;
            state->absasum2 = state->absasum2+v*v;
        }
        if( (j>i&&isupper)||(j<i&&!isupper) )
        {
            
            /*
             * Offdiagonal terms are counted twice
             */
            state->absamax = ae_maxreal(state->absamax, v, _state);
            state->absasum = state->absasum+2*v;
            state->absasum2 = state->absasum2+2*v*v;
        }
    }
}


/*************************************************************************
This function sets starting point for QP solver. It is useful to have
good initial approximation to the solution, because it will increase
speed of convergence and identification of active constraints.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    X       -   starting point, array[N].

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetstartingpoint(minqpstate* state,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t n;


    n = state->n;
    ae_assert(x->cnt>=n, "MinQPSetStartingPoint: Length(B)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "MinQPSetStartingPoint: X contains infinite or NaN elements", _state);
    minqpsetstartingpointfast(state, x, _state);
}


/*************************************************************************
This  function sets origin for QP solver. By default, following QP program
is solved:

    min(0.5*x'*A*x+b'*x)
    
This function allows to solve different problem:

    min(0.5*(x-x_origin)'*A*(x-x_origin)+b'*(x-x_origin))
    
INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    XOrigin -   origin, array[N].

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetorigin(minqpstate* state,
     /* Real    */ ae_vector* xorigin,
     ae_state *_state)
{
    ae_int_t n;


    n = state->n;
    ae_assert(xorigin->cnt>=n, "MinQPSetOrigin: Length(B)<N", _state);
    ae_assert(isfinitevector(xorigin, n, _state), "MinQPSetOrigin: B contains infinite or NaN elements", _state);
    minqpsetoriginfast(state, xorigin, _state);
}


/*************************************************************************
This function sets scaling coefficients.

ALGLIB optimizers use scaling matrices to test stopping  conditions  (step
size and gradient are scaled before comparison with tolerances).  Scale of
the I-th variable is a translation invariant measure of:
a) "how large" the variable is
b) how large the step should be to make significant changes in the function

BLEIC-based QP solver uses scale for two purposes:
* to evaluate stopping conditions
* for preconditioning of the underlying BLEIC solver

INPUT PARAMETERS:
    State   -   structure stores algorithm state
    S       -   array[N], non-zero scaling coefficients
                S[i] may be negative, sign doesn't matter.

  -- ALGLIB --
     Copyright 14.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetscale(minqpstate* state,
     /* Real    */ ae_vector* s,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(s->cnt>=state->n, "MinQPSetScale: Length(S)<N", _state);
    for(i=0; i<=state->n-1; i++)
    {
        ae_assert(ae_isfinite(s->ptr.p_double[i], _state), "MinQPSetScale: S contains infinite or NAN elements", _state);
        ae_assert(ae_fp_neq(s->ptr.p_double[i],(double)(0)), "MinQPSetScale: S contains zero elements", _state);
        state->s.ptr.p_double[i] = ae_fabs(s->ptr.p_double[i], _state);
    }
}


/*************************************************************************
This function tells solver to use Cholesky-based algorithm. This algorithm
was deprecated in ALGLIB 3.9.0 because its performance is inferior to that
of BLEIC-QP or  QuickQP  on  high-dimensional  problems.  Furthermore,  it
supports only dense convex QP problems.

This solver is no longer active by default.

We recommend you to switch to BLEIC-QP or QuickQP solver.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetalgocholesky(minqpstate* state, ae_state *_state)
{


    state->algokind = 1;
}


/*************************************************************************
This function tells solver to use BLEIC-based algorithm and sets  stopping
criteria for the algorithm.

ALGORITHM FEATURES:

* supports dense and sparse QP problems
* supports boundary and general linear equality/inequality constraints
* can solve all types of problems  (convex,  semidefinite,  nonconvex)  as
  long as they are bounded from below under constraints.
  Say, it is possible to solve "min{-x^2} subject to -1<=x<=+1".
  Of course, global  minimum  is found only  for  positive  definite   and
  semidefinite  problems.  As  for indefinite ones - only local minimum is
  found.

ALGORITHM OUTLINE:

* BLEIC-QP solver is just a driver function for MinBLEIC solver; it solves
  quadratic  programming   problem   as   general   linearly   constrained
  optimization problem, which is solved by means of BLEIC solver  (part of
  ALGLIB, active set method).
  
ALGORITHM LIMITATIONS:

* unlike QuickQP solver, this algorithm does not perform Newton steps  and
  does not use Level 3 BLAS. Being general-purpose active set  method,  it
  can activate constraints only one-by-one. Thus, its performance is lower
  than that of QuickQP.
* its precision is also a bit  inferior  to  that  of   QuickQP.  BLEIC-QP
  performs only LBFGS steps (no Newton steps), which are good at detecting
  neighborhood of the solution, buy need many iterations to find  solution
  with more than 6 digits of precision.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    EpsG    -   >=0
                The  subroutine  finishes  its  work   if   the  condition
                |v|<EpsG is satisfied, where:
                * |.| means Euclidian norm
                * v - scaled constrained gradient vector, v[i]=g[i]*s[i]
                * g - gradient
                * s - scaling coefficients set by MinQPSetScale()
    EpsF    -   >=0
                The  subroutine  finishes its work if exploratory steepest
                descent  step  on  k+1-th iteration  satisfies   following
                condition:  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
    EpsX    -   >=0
                The  subroutine  finishes its work if exploratory steepest
                descent  step  on  k+1-th iteration  satisfies   following
                condition:  
                * |.| means Euclidian norm
                * v - scaled step vector, v[i]=dx[i]/s[i]
                * dx - step vector, dx=X(k+1)-X(k)
                * s - scaling coefficients set by MinQPSetScale()
    MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                iterations is unlimited. NOTE: this  algorithm uses  LBFGS
                iterations,  which  are  relatively  cheap,  but   improve
                function value only a bit. So you will need many iterations
                to converge - from 0.1*N to 10*N, depending  on  problem's
                condition number.

IT IS VERY IMPORTANT TO CALL MinQPSetScale() WHEN YOU USE THIS  ALGORITHM
BECAUSE ITS STOPPING CRITERIA ARE SCALE-DEPENDENT!

Passing EpsG=0, EpsF=0 and EpsX=0 and MaxIts=0 (simultaneously) will lead
to automatic stopping criterion selection (presently it is  small    step
length, but it may change in the future versions of ALGLIB).

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetalgobleic(minqpstate* state,
     double epsg,
     double epsf,
     double epsx,
     ae_int_t maxits,
     ae_state *_state)
{


    ae_assert(ae_isfinite(epsg, _state), "MinQPSetAlgoBLEIC: EpsG is not finite number", _state);
    ae_assert(ae_fp_greater_eq(epsg,(double)(0)), "MinQPSetAlgoBLEIC: negative EpsG", _state);
    ae_assert(ae_isfinite(epsf, _state), "MinQPSetAlgoBLEIC: EpsF is not finite number", _state);
    ae_assert(ae_fp_greater_eq(epsf,(double)(0)), "MinQPSetAlgoBLEIC: negative EpsF", _state);
    ae_assert(ae_isfinite(epsx, _state), "MinQPSetAlgoBLEIC: EpsX is not finite number", _state);
    ae_assert(ae_fp_greater_eq(epsx,(double)(0)), "MinQPSetAlgoBLEIC: negative EpsX", _state);
    ae_assert(maxits>=0, "MinQPSetAlgoBLEIC: negative MaxIts!", _state);
    state->algokind = 2;
    if( ((ae_fp_eq(epsg,(double)(0))&&ae_fp_eq(epsf,(double)(0)))&&ae_fp_eq(epsx,(double)(0)))&&maxits==0 )
    {
        epsx = 1.0E-6;
    }
    state->qpbleicsettingsuser.epsg = epsg;
    state->qpbleicsettingsuser.epsf = epsf;
    state->qpbleicsettingsuser.epsx = epsx;
    state->qpbleicsettingsuser.maxits = maxits;
}


/*************************************************************************
This function tells solver to use QuickQP  algorithm:  special  extra-fast
algorithm   for   problems  with  boundary-only constrants. It  may  solve
non-convex  problems  as  long  as  they  are  bounded  from  below  under
constraints.

ALGORITHM FEATURES:
* many times (from 5x to 50x!) faster than BLEIC-based QP solver; utilizes
  accelerated methods for activation of constraints.
* supports dense and sparse QP problems
* supports ONLY boundary constraints; general linear constraints  are  NOT
  supported by this solver
* can solve all types of problems  (convex,  semidefinite,  nonconvex)  as
  long as they are bounded from below under constraints.
  Say, it is possible to solve "min{-x^2} subject to -1<=x<=+1".
  In convex/semidefinite case global minimum  is  returned,  in  nonconvex
  case - algorithm returns one of the local minimums.

ALGORITHM OUTLINE:

* algorithm  performs  two kinds of iterations: constrained CG  iterations
  and constrained Newton iterations
* initially it performs small number of constrained CG  iterations,  which
  can efficiently activate/deactivate multiple constraints
* after CG phase algorithm tries to calculate Cholesky  decomposition  and
  to perform several constrained Newton steps. If  Cholesky  decomposition
  failed (matrix is indefinite even under constraints),  we  perform  more
  CG iterations until we converge to such set of constraints  that  system
  matrix becomes  positive  definite.  Constrained  Newton  steps  greatly
  increase convergence speed and precision.
* algorithm interleaves CG and Newton iterations which  allows  to  handle
  indefinite matrices (CG phase) and quickly converge after final  set  of
  constraints is found (Newton phase). Combination of CG and Newton phases
  is called "outer iteration".
* it is possible to turn off Newton  phase  (beneficial  for  semidefinite
  problems - Cholesky decomposition will fail too often)
  
ALGORITHM LIMITATIONS:

* algorithm does not support general  linear  constraints;  only  boundary
  ones are supported
* Cholesky decomposition for sparse problems  is  performed  with  Skyline
  Cholesky solver, which is intended for low-profile matrices. No profile-
  reducing reordering of variables is performed in this version of ALGLIB.
* problems with near-zero negative eigenvalues (or exacty zero  ones)  may
  experience about 2-3x performance penalty. The reason is  that  Cholesky
  decomposition can not be performed until we identify directions of  zero
  and negative curvature and activate corresponding boundary constraints -
  but we need a lot of trial and errors because these directions  are hard
  to notice in the matrix spectrum.
  In this case you may turn off Newton phase of algorithm.
  Large negative eigenvalues  are  not  an  issue,  so  highly  non-convex
  problems can be solved very efficiently.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    EpsG    -   >=0
                The  subroutine  finishes  its  work   if   the  condition
                |v|<EpsG is satisfied, where:
                * |.| means Euclidian norm
                * v - scaled constrained gradient vector, v[i]=g[i]*s[i]
                * g - gradient
                * s - scaling coefficients set by MinQPSetScale()
    EpsF    -   >=0
                The  subroutine  finishes its work if exploratory steepest
                descent  step  on  k+1-th iteration  satisfies   following
                condition:  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
    EpsX    -   >=0
                The  subroutine  finishes its work if exploratory steepest
                descent  step  on  k+1-th iteration  satisfies   following
                condition:  
                * |.| means Euclidian norm
                * v - scaled step vector, v[i]=dx[i]/s[i]
                * dx - step vector, dx=X(k+1)-X(k)
                * s - scaling coefficients set by MinQPSetScale()
    MaxOuterIts-maximum number of OUTER iterations.  One  outer  iteration
                includes some amount of CG iterations (from 5 to  ~N)  and
                one or several (usually small amount) Newton steps.  Thus,
                one outer iteration has high cost, but can greatly  reduce
                funcation value.
    UseNewton-  use Newton phase or not:
                * Newton phase improves performance of  positive  definite
                  dense problems (about 2 times improvement can be observed)
                * can result in some performance penalty  on  semidefinite
                  or slightly negative definite  problems  -  each  Newton
                  phase will bring no improvement (Cholesky failure),  but
                  still will require computational time.
                * if you doubt, you can turn off this  phase  -  optimizer
                  will retain its most of its high speed.

IT IS VERY IMPORTANT TO CALL MinQPSetScale() WHEN YOU USE THIS  ALGORITHM
BECAUSE ITS STOPPING CRITERIA ARE SCALE-DEPENDENT!

Passing EpsG=0, EpsF=0 and EpsX=0 and MaxIts=0 (simultaneously) will lead
to automatic stopping criterion selection (presently it is  small    step
length, but it may change in the future versions of ALGLIB).

  -- ALGLIB --
     Copyright 22.05.2014 by Bochkanov Sergey
*************************************************************************/
void minqpsetalgoquickqp(minqpstate* state,
     double epsg,
     double epsf,
     double epsx,
     ae_int_t maxouterits,
     ae_bool usenewton,
     ae_state *_state)
{


    ae_assert(ae_isfinite(epsg, _state), "MinQPSetAlgoQuickQP: EpsG is not finite number", _state);
    ae_assert(ae_fp_greater_eq(epsg,(double)(0)), "MinQPSetAlgoQuickQP: negative EpsG", _state);
    ae_assert(ae_isfinite(epsf, _state), "MinQPSetAlgoQuickQP: EpsF is not finite number", _state);
    ae_assert(ae_fp_greater_eq(epsf,(double)(0)), "MinQPSetAlgoQuickQP: negative EpsF", _state);
    ae_assert(ae_isfinite(epsx, _state), "MinQPSetAlgoQuickQP: EpsX is not finite number", _state);
    ae_assert(ae_fp_greater_eq(epsx,(double)(0)), "MinQPSetAlgoQuickQP: negative EpsX", _state);
    ae_assert(maxouterits>=0, "MinQPSetAlgoQuickQP: negative MaxOuterIts!", _state);
    state->algokind = 3;
    if( ((ae_fp_eq(epsg,(double)(0))&&ae_fp_eq(epsf,(double)(0)))&&ae_fp_eq(epsx,(double)(0)))&&maxouterits==0 )
    {
        epsx = 1.0E-6;
    }
    state->qqpsettingsuser.maxouterits = maxouterits;
    state->qqpsettingsuser.epsg = epsg;
    state->qqpsettingsuser.epsf = epsf;
    state->qqpsettingsuser.epsx = epsx;
    state->qqpsettingsuser.cnphase = usenewton;
}


/*************************************************************************
This function sets boundary constraints for QP solver

Boundary constraints are inactive by default (after initial creation).
After  being  set,  they  are  preserved  until explicitly turned off with
another SetBC() call.

INPUT PARAMETERS:
    State   -   structure stores algorithm state
    BndL    -   lower bounds, array[N].
                If some (all) variables are unbounded, you may specify
                very small number or -INF (latter is recommended because
                it will allow solver to use better algorithm).
    BndU    -   upper bounds, array[N].
                If some (all) variables are unbounded, you may specify
                very large number or +INF (latter is recommended because
                it will allow solver to use better algorithm).
                
NOTE: it is possible to specify BndL[i]=BndU[i]. In this case I-th
variable will be "frozen" at X[i]=BndL[i]=BndU[i].

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetbc(minqpstate* state,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t n;


    n = state->n;
    ae_assert(bndl->cnt>=n, "MinQPSetBC: Length(BndL)<N", _state);
    ae_assert(bndu->cnt>=n, "MinQPSetBC: Length(BndU)<N", _state);
    for(i=0; i<=n-1; i++)
    {
        ae_assert(ae_isfinite(bndl->ptr.p_double[i], _state)||ae_isneginf(bndl->ptr.p_double[i], _state), "MinQPSetBC: BndL contains NAN or +INF", _state);
        ae_assert(ae_isfinite(bndu->ptr.p_double[i], _state)||ae_isposinf(bndu->ptr.p_double[i], _state), "MinQPSetBC: BndU contains NAN or -INF", _state);
        state->bndl.ptr.p_double[i] = bndl->ptr.p_double[i];
        state->havebndl.ptr.p_bool[i] = ae_isfinite(bndl->ptr.p_double[i], _state);
        state->bndu.ptr.p_double[i] = bndu->ptr.p_double[i];
        state->havebndu.ptr.p_bool[i] = ae_isfinite(bndu->ptr.p_double[i], _state);
    }
}


/*************************************************************************
This function sets linear constraints for QP optimizer.

Linear constraints are inactive by default (after initial creation).

INPUT PARAMETERS:
    State   -   structure previously allocated with MinQPCreate call.
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

NOTE 1: linear (non-bound) constraints are satisfied only approximately  -
        there always exists some minor violation (about 10^-10...10^-13)
        due to numerical errors.

  -- ALGLIB --
     Copyright 19.06.2012 by Bochkanov Sergey
*************************************************************************/
void minqpsetlc(minqpstate* state,
     /* Real    */ ae_matrix* c,
     /* Integer */ ae_vector* ct,
     ae_int_t k,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    double v;


    n = state->n;
    
    /*
     * First, check for errors in the inputs
     */
    ae_assert(k>=0, "MinQPSetLC: K<0", _state);
    ae_assert(c->cols>=n+1||k==0, "MinQPSetLC: Cols(C)<N+1", _state);
    ae_assert(c->rows>=k, "MinQPSetLC: Rows(C)<K", _state);
    ae_assert(ct->cnt>=k, "MinQPSetLC: Length(CT)<K", _state);
    ae_assert(apservisfinitematrix(c, k, n+1, _state), "MinQPSetLC: C contains infinite or NaN values!", _state);
    
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
    
    /*
     * Normalize rows of State.CLEIC: each row must have unit norm.
     * Norm is calculated using first N elements (i.e. right part is
     * not counted when we calculate norm).
     */
    for(i=0; i<=k-1; i++)
    {
        v = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            v = v+ae_sqr(state->cleic.ptr.pp_double[i][j], _state);
        }
        if( ae_fp_eq(v,(double)(0)) )
        {
            continue;
        }
        v = 1/ae_sqrt(v, _state);
        ae_v_muld(&state->cleic.ptr.pp_double[i][0], 1, ae_v_len(0,n), v);
    }
}


/*************************************************************************
This function solves quadratic programming problem.

Prior to calling this function you should choose solver by means of one of
the following functions:

* MinQPSetAlgoQuickQP() - for QuickQP solver
* MinQPSetAlgoBLEIC() - for BLEIC-QP solver

These functions also allow you to control stopping criteria of the solver.
If you did not set solver,  MinQP  subpackage  will  automatically  select
solver for your problem and will run it with default stopping criteria.

However, it is better to set explicitly solver and its stopping criteria.

INPUT PARAMETERS:
    State   -   algorithm state

You should use MinQPResults() function to access results after calls
to this function.

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey.
     Special thanks to Elvira Illarionova  for  important  suggestions  on
     the linearly constrained QP algorithm.
*************************************************************************/
void minqpoptimize(minqpstate* state, ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t nbc;
    ae_int_t currentsolver;


    n = state->n;
    state->repterminationtype = -5;
    state->repinneriterationscount = 0;
    state->repouteriterationscount = 0;
    state->repncholesky = 0;
    state->repnmv = 0;
    
    /*
     * check correctness of constraints
     */
    for(i=0; i<=n-1; i++)
    {
        if( state->havebndl.ptr.p_bool[i]&&state->havebndu.ptr.p_bool[i] )
        {
            if( ae_fp_greater(state->bndl.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
            {
                state->repterminationtype = -3;
                return;
            }
        }
    }
    
    /*
     * count number of bound and linear constraints
     */
    nbc = 0;
    for(i=0; i<=n-1; i++)
    {
        if( state->havebndl.ptr.p_bool[i] )
        {
            nbc = nbc+1;
        }
        if( state->havebndu.ptr.p_bool[i] )
        {
            nbc = nbc+1;
        }
    }
    
    /*
     * Initial point:
     * * if we have starting point in StartX, we just have to bound it
     * * if we do not have StartX, deduce initial point from boundary constraints
     */
    if( state->havex )
    {
        for(i=0; i<=n-1; i++)
        {
            state->xs.ptr.p_double[i] = state->startx.ptr.p_double[i];
            if( state->havebndl.ptr.p_bool[i]&&ae_fp_less(state->xs.ptr.p_double[i],state->bndl.ptr.p_double[i]) )
            {
                state->xs.ptr.p_double[i] = state->bndl.ptr.p_double[i];
            }
            if( state->havebndu.ptr.p_bool[i]&&ae_fp_greater(state->xs.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
            {
                state->xs.ptr.p_double[i] = state->bndu.ptr.p_double[i];
            }
        }
    }
    else
    {
        for(i=0; i<=n-1; i++)
        {
            if( state->havebndl.ptr.p_bool[i]&&state->havebndu.ptr.p_bool[i] )
            {
                state->xs.ptr.p_double[i] = 0.5*(state->bndl.ptr.p_double[i]+state->bndu.ptr.p_double[i]);
                continue;
            }
            if( state->havebndl.ptr.p_bool[i] )
            {
                state->xs.ptr.p_double[i] = state->bndl.ptr.p_double[i];
                continue;
            }
            if( state->havebndu.ptr.p_bool[i] )
            {
                state->xs.ptr.p_double[i] = state->bndu.ptr.p_double[i];
                continue;
            }
            state->xs.ptr.p_double[i] = (double)(0);
        }
    }
    
    /*
     * Choose solver - user-specified or default one.
     */
    currentsolver = state->algokind;
    if( currentsolver==0 )
    {
        
        /*
         * Choose solver automatically
         */
        if( state->nec+state->nic==0 )
        {
            
            /*
             * QQP solver is used for problems without linear constraints
             */
            currentsolver = 3;
            qqploaddefaults(n, &state->qqpsettingscurrent, _state);
        }
        else
        {
            
            /*
             * QP-BLEIC solver is used for problems without linear constraints
             */
            currentsolver = 2;
            qpbleicloaddefaults(n, &state->qpbleicsettingscurrent, _state);
        }
    }
    else
    {
        ae_assert((currentsolver==1||currentsolver==2)||currentsolver==3, "MinQPOptimize: internal error", _state);
        if( currentsolver==1 )
        {
        }
        if( currentsolver==2 )
        {
            
            /*
             * QP-BLEIC solver is chosen by user
             */
            qpbleiccopysettings(&state->qpbleicsettingsuser, &state->qpbleicsettingscurrent, _state);
        }
        if( currentsolver==3 )
        {
            
            /*
             * QQP solver is chosen by user
             */
            qqpcopysettings(&state->qqpsettingsuser, &state->qqpsettingscurrent, _state);
        }
    }
    
    /*
     * QP-BLEIC solver
     */
    if( currentsolver==2 )
    {
        qpbleicoptimize(&state->a, &state->sparsea, state->akind, state->sparseaupper, state->absasum, state->absasum2, &state->b, &state->bndl, &state->bndu, &state->s, &state->xorigin, n, &state->cleic, state->nec, state->nic, &state->qpbleicsettingscurrent, &state->qpbleicbuf, &state->qpbleicfirstcall, &state->xs, &state->repterminationtype, _state);
        state->repinneriterationscount = state->qpbleicbuf.repinneriterationscount;
        state->repouteriterationscount = state->qpbleicbuf.repouteriterationscount;
        return;
    }
    
    /*
     * QuickQP solver
     */
    if( currentsolver==3 )
    {
        if( state->nec+state->nic>0 )
        {
            state->repterminationtype = -5;
            return;
        }
        qqpoptimize(&state->a, &state->sparsea, state->akind, state->sparseaupper, &state->b, &state->bndl, &state->bndu, &state->s, &state->xorigin, n, &state->cleic, state->nec, state->nic, &state->qqpsettingscurrent, &state->qqpbuf, &state->xs, &state->repterminationtype, _state);
        state->repinneriterationscount = state->qqpbuf.repinneriterationscount;
        state->repouteriterationscount = state->qqpbuf.repouteriterationscount;
        state->repncholesky = state->qqpbuf.repncholesky;
        return;
    }
    
    /*
     * Cholesky solver.
     */
    if( currentsolver==1 )
    {
        
        /*
         * Check matrix type.
         * Cholesky solver supports only dense matrices.
         */
        if( state->akind!=0 )
        {
            state->repterminationtype = -5;
            return;
        }
        qpcholeskyoptimize(&state->a, state->absamax*n, &state->b, &state->bndl, &state->bndu, &state->s, &state->xorigin, n, &state->cleic, state->nec, state->nic, &state->qpcholeskybuf, &state->xs, &state->repterminationtype, _state);
        state->repinneriterationscount = state->qqpbuf.repinneriterationscount;
        state->repouteriterationscount = state->qqpbuf.repouteriterationscount;
        state->repncholesky = state->qqpbuf.repncholesky;
        return;
    }
}


/*************************************************************************
QP solver results

INPUT PARAMETERS:
    State   -   algorithm state

OUTPUT PARAMETERS:
    X       -   array[0..N-1], solution.
                This array is allocated and initialized only when
                Rep.TerminationType parameter is positive (success).
    Rep     -   optimization report. You should check Rep.TerminationType,
                which contains completion code, and you may check  another
                fields which contain another information  about  algorithm
                functioning.
                
                Failure codes returned by algorithm are:
                * -5    inappropriate solver was used:
                        * Cholesky solver for (semi)indefinite problems
                        * Cholesky solver for problems with sparse matrix
                        * QuickQP solver for problem with  general  linear
                          constraints
                * -4    BLEIC-QP/QuickQP   solver    found   unconstrained
                        direction  of   negative  curvature  (function  is
                        unbounded from below even under constraints),   no
                        meaningful minimum can be found.
                * -3    inconsistent constraints (or maybe  feasible point
                        is too  hard  to  find).  If  you  are  sure  that
                        constraints are feasible, try to restart optimizer
                        with better initial approximation.
                        
                Completion codes specific for Cholesky algorithm:
                *  4   successful completion
                
                Completion codes specific for BLEIC/QuickQP algorithms:
                *  1   relative function improvement is no more than EpsF.
                *  2   scaled step is no more than EpsX.
                *  4   scaled gradient norm is no more than EpsG.
                *  5   MaxIts steps was taken

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpresults(minqpstate* state,
     /* Real    */ ae_vector* x,
     minqpreport* rep,
     ae_state *_state)
{

    ae_vector_clear(x);
    _minqpreport_clear(rep);

    minqpresultsbuf(state, x, rep, _state);
}


/*************************************************************************
QP results

Buffered implementation of MinQPResults() which uses pre-allocated  buffer
to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
intended to be used in the inner cycles of performance critical algorithms
where array reallocation penalty is too large to be ignored.

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpresultsbuf(minqpstate* state,
     /* Real    */ ae_vector* x,
     minqpreport* rep,
     ae_state *_state)
{


    if( x->cnt<state->n )
    {
        ae_vector_set_length(x, state->n, _state);
    }
    ae_v_move(&x->ptr.p_double[0], 1, &state->xs.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    rep->inneriterationscount = state->repinneriterationscount;
    rep->outeriterationscount = state->repouteriterationscount;
    rep->nmv = state->repnmv;
    rep->ncholesky = state->repncholesky;
    rep->terminationtype = state->repterminationtype;
}


/*************************************************************************
Fast version of MinQPSetLinearTerm(), which doesn't check its arguments.
For internal use only.

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetlineartermfast(minqpstate* state,
     /* Real    */ ae_vector* b,
     ae_state *_state)
{


    ae_v_move(&state->b.ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,state->n-1));
}


/*************************************************************************
Fast version of MinQPSetQuadraticTerm(), which doesn't check its arguments.

It accepts additional parameter - shift S, which allows to "shift"  matrix
A by adding s*I to A. S must be positive (although it is not checked).

For internal use only.

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetquadratictermfast(minqpstate* state,
     /* Real    */ ae_matrix* a,
     ae_bool isupper,
     double s,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;
    double v;
    ae_int_t j0;
    ae_int_t j1;


    n = state->n;
    state->akind = 0;
    cqmseta(&state->a, a, isupper, 1.0, _state);
    if( ae_fp_greater(s,(double)(0)) )
    {
        rvectorsetlengthatleast(&state->tmp0, n, _state);
        for(i=0; i<=n-1; i++)
        {
            state->tmp0.ptr.p_double[i] = a->ptr.pp_double[i][i]+s;
        }
        cqmrewritedensediagonal(&state->a, &state->tmp0, _state);
    }
    
    /*
     * Estimate norm of A
     * (it will be used later in the quadratic penalty function)
     */
    state->absamax = (double)(0);
    state->absasum = (double)(0);
    state->absasum2 = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        if( isupper )
        {
            j0 = i;
            j1 = n-1;
        }
        else
        {
            j0 = 0;
            j1 = i;
        }
        for(j=j0; j<=j1; j++)
        {
            v = ae_fabs(a->ptr.pp_double[i][j], _state);
            state->absamax = ae_maxreal(state->absamax, v, _state);
            state->absasum = state->absasum+v;
            state->absasum2 = state->absasum2+v*v;
        }
    }
}


/*************************************************************************
Internal function which allows to rewrite diagonal of quadratic term.
For internal use only.

This function can be used only when you have dense A and already made
MinQPSetQuadraticTerm(Fast) call.

  -- ALGLIB --
     Copyright 16.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqprewritediagonal(minqpstate* state,
     /* Real    */ ae_vector* s,
     ae_state *_state)
{


    cqmrewritedensediagonal(&state->a, s, _state);
}


/*************************************************************************
Fast version of MinQPSetStartingPoint(), which doesn't check its arguments.
For internal use only.

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetstartingpointfast(minqpstate* state,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t n;


    n = state->n;
    ae_v_move(&state->startx.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->havex = ae_true;
}


/*************************************************************************
Fast version of MinQPSetOrigin(), which doesn't check its arguments.
For internal use only.

  -- ALGLIB --
     Copyright 11.01.2011 by Bochkanov Sergey
*************************************************************************/
void minqpsetoriginfast(minqpstate* state,
     /* Real    */ ae_vector* xorigin,
     ae_state *_state)
{
    ae_int_t n;


    n = state->n;
    ae_v_move(&state->xorigin.ptr.p_double[0], 1, &xorigin->ptr.p_double[0], 1, ae_v_len(0,n-1));
}


void _minqpstate_init(void* _p, ae_state *_state)
{
    minqpstate *p = (minqpstate*)_p;
    ae_touch_ptr((void*)p);
    _qqpsettings_init(&p->qqpsettingsuser, _state);
    _qqpsettings_init(&p->qqpsettingscurrent, _state);
    _qpbleicsettings_init(&p->qpbleicsettingsuser, _state);
    _qpbleicsettings_init(&p->qpbleicsettingscurrent, _state);
    _convexquadraticmodel_init(&p->a, _state);
    _sparsematrix_init(&p->sparsea, _state);
    ae_vector_init(&p->b, 0, DT_REAL, _state);
    ae_vector_init(&p->bndl, 0, DT_REAL, _state);
    ae_vector_init(&p->bndu, 0, DT_REAL, _state);
    ae_vector_init(&p->s, 0, DT_REAL, _state);
    ae_vector_init(&p->havebndl, 0, DT_BOOL, _state);
    ae_vector_init(&p->havebndu, 0, DT_BOOL, _state);
    ae_vector_init(&p->xorigin, 0, DT_REAL, _state);
    ae_vector_init(&p->startx, 0, DT_REAL, _state);
    ae_matrix_init(&p->cleic, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->xs, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp0, 0, DT_REAL, _state);
    _qpbleicbuffers_init(&p->qpbleicbuf, _state);
    _qqpbuffers_init(&p->qqpbuf, _state);
    _qpcholeskybuffers_init(&p->qpcholeskybuf, _state);
    _normestimatorstate_init(&p->estimator, _state);
}


void _minqpstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    minqpstate *dst = (minqpstate*)_dst;
    minqpstate *src = (minqpstate*)_src;
    dst->n = src->n;
    _qqpsettings_init_copy(&dst->qqpsettingsuser, &src->qqpsettingsuser, _state);
    _qqpsettings_init_copy(&dst->qqpsettingscurrent, &src->qqpsettingscurrent, _state);
    _qpbleicsettings_init_copy(&dst->qpbleicsettingsuser, &src->qpbleicsettingsuser, _state);
    _qpbleicsettings_init_copy(&dst->qpbleicsettingscurrent, &src->qpbleicsettingscurrent, _state);
    dst->algokind = src->algokind;
    dst->akind = src->akind;
    _convexquadraticmodel_init_copy(&dst->a, &src->a, _state);
    _sparsematrix_init_copy(&dst->sparsea, &src->sparsea, _state);
    dst->sparseaupper = src->sparseaupper;
    dst->absamax = src->absamax;
    dst->absasum = src->absasum;
    dst->absasum2 = src->absasum2;
    ae_vector_init_copy(&dst->b, &src->b, _state);
    ae_vector_init_copy(&dst->bndl, &src->bndl, _state);
    ae_vector_init_copy(&dst->bndu, &src->bndu, _state);
    ae_vector_init_copy(&dst->s, &src->s, _state);
    ae_vector_init_copy(&dst->havebndl, &src->havebndl, _state);
    ae_vector_init_copy(&dst->havebndu, &src->havebndu, _state);
    ae_vector_init_copy(&dst->xorigin, &src->xorigin, _state);
    ae_vector_init_copy(&dst->startx, &src->startx, _state);
    dst->havex = src->havex;
    ae_matrix_init_copy(&dst->cleic, &src->cleic, _state);
    dst->nec = src->nec;
    dst->nic = src->nic;
    ae_vector_init_copy(&dst->xs, &src->xs, _state);
    dst->repinneriterationscount = src->repinneriterationscount;
    dst->repouteriterationscount = src->repouteriterationscount;
    dst->repncholesky = src->repncholesky;
    dst->repnmv = src->repnmv;
    dst->repterminationtype = src->repterminationtype;
    ae_vector_init_copy(&dst->tmp0, &src->tmp0, _state);
    dst->qpbleicfirstcall = src->qpbleicfirstcall;
    _qpbleicbuffers_init_copy(&dst->qpbleicbuf, &src->qpbleicbuf, _state);
    _qqpbuffers_init_copy(&dst->qqpbuf, &src->qqpbuf, _state);
    _qpcholeskybuffers_init_copy(&dst->qpcholeskybuf, &src->qpcholeskybuf, _state);
    _normestimatorstate_init_copy(&dst->estimator, &src->estimator, _state);
}


void _minqpstate_clear(void* _p)
{
    minqpstate *p = (minqpstate*)_p;
    ae_touch_ptr((void*)p);
    _qqpsettings_clear(&p->qqpsettingsuser);
    _qqpsettings_clear(&p->qqpsettingscurrent);
    _qpbleicsettings_clear(&p->qpbleicsettingsuser);
    _qpbleicsettings_clear(&p->qpbleicsettingscurrent);
    _convexquadraticmodel_clear(&p->a);
    _sparsematrix_clear(&p->sparsea);
    ae_vector_clear(&p->b);
    ae_vector_clear(&p->bndl);
    ae_vector_clear(&p->bndu);
    ae_vector_clear(&p->s);
    ae_vector_clear(&p->havebndl);
    ae_vector_clear(&p->havebndu);
    ae_vector_clear(&p->xorigin);
    ae_vector_clear(&p->startx);
    ae_matrix_clear(&p->cleic);
    ae_vector_clear(&p->xs);
    ae_vector_clear(&p->tmp0);
    _qpbleicbuffers_clear(&p->qpbleicbuf);
    _qqpbuffers_clear(&p->qqpbuf);
    _qpcholeskybuffers_clear(&p->qpcholeskybuf);
    _normestimatorstate_clear(&p->estimator);
}


void _minqpstate_destroy(void* _p)
{
    minqpstate *p = (minqpstate*)_p;
    ae_touch_ptr((void*)p);
    _qqpsettings_destroy(&p->qqpsettingsuser);
    _qqpsettings_destroy(&p->qqpsettingscurrent);
    _qpbleicsettings_destroy(&p->qpbleicsettingsuser);
    _qpbleicsettings_destroy(&p->qpbleicsettingscurrent);
    _convexquadraticmodel_destroy(&p->a);
    _sparsematrix_destroy(&p->sparsea);
    ae_vector_destroy(&p->b);
    ae_vector_destroy(&p->bndl);
    ae_vector_destroy(&p->bndu);
    ae_vector_destroy(&p->s);
    ae_vector_destroy(&p->havebndl);
    ae_vector_destroy(&p->havebndu);
    ae_vector_destroy(&p->xorigin);
    ae_vector_destroy(&p->startx);
    ae_matrix_destroy(&p->cleic);
    ae_vector_destroy(&p->xs);
    ae_vector_destroy(&p->tmp0);
    _qpbleicbuffers_destroy(&p->qpbleicbuf);
    _qqpbuffers_destroy(&p->qqpbuf);
    _qpcholeskybuffers_destroy(&p->qpcholeskybuf);
    _normestimatorstate_destroy(&p->estimator);
}


void _minqpreport_init(void* _p, ae_state *_state)
{
    minqpreport *p = (minqpreport*)_p;
    ae_touch_ptr((void*)p);
}


void _minqpreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    minqpreport *dst = (minqpreport*)_dst;
    minqpreport *src = (minqpreport*)_src;
    dst->inneriterationscount = src->inneriterationscount;
    dst->outeriterationscount = src->outeriterationscount;
    dst->nmv = src->nmv;
    dst->ncholesky = src->ncholesky;
    dst->terminationtype = src->terminationtype;
}


void _minqpreport_clear(void* _p)
{
    minqpreport *p = (minqpreport*)_p;
    ae_touch_ptr((void*)p);
}


void _minqpreport_destroy(void* _p)
{
    minqpreport *p = (minqpreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
