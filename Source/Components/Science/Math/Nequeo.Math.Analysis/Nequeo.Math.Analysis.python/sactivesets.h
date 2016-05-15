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

#ifndef _sactivesets_h
#define _sactivesets_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "reflections.h"
#include "creflections.h"
#include "hqrnd.h"
#include "matgen.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "tsort.h"
#include "sparse.h"
#include "rotations.h"
#include "trfac.h"
#include "hblas.h"
#include "sblas.h"
#include "ortfac.h"
#include "fbls.h"
#include "snnls.h"
#include "trlinsolve.h"
#include "safesolve.h"
#include "rcond.h"
#include "matinv.h"
#include "blas.h"
#include "bdsvd.h"
#include "svd.h"
#include "optserv.h"


/*$ Declarations $*/


/*************************************************************************
This structure describes set of linear constraints (boundary  and  general
ones) which can be active and inactive. It also has functionality to  work
with current point and current  gradient  (determine  active  constraints,
move current point, project gradient into  constrained  subspace,  perform
constrained preconditioning and so on.

This  structure  is  intended  to  be  used  by constrained optimizers for
management of all constraint-related functionality.

External code may access following internal fields of the structure:
    XC          -   stores current point, array[N].
                    can be accessed only in optimization mode
    ActiveSet   -   active set, array[N+NEC+NIC]:
                    * ActiveSet[I]>0    I-th constraint is in the active set
                    * ActiveSet[I]=0    I-th constraint is at the boundary, but inactive
                    * ActiveSet[I]<0    I-th constraint is far from the boundary (and inactive)
                    * elements from 0 to N-1 correspond to boundary constraints
                    * elements from N to N+NEC+NIC-1 correspond to linear constraints
                    * elements from N to N+NEC-1 are always +1
    PBasis,
    IBasis,
    SBasis      -   after call to SASRebuildBasis() these  matrices  store
                    active constraints, reorthogonalized with  respect  to
                    some inner product:
                    a) for PBasis - one  given  by  preconditioner  matrix
                       (inverse Hessian)
                    b) for SBasis - one given by square of the scale matrix
                    c) for IBasis - traditional dot product
                    
                    array[BasisSize,N+1], where BasisSize is a  number  of
                    positive elements in  ActiveSet[N:N+NEC+NIC-1].  First
                    N columns store linear term, last column stores  right
                    part. All  three  matrices  are linearly equivalent to
                    each other, span(PBasis)=span(IBasis)=span(SBasis).
                    
                    IMPORTANT: you have to call  SASRebuildBasis()  before
                               accessing these arrays  in  order  to  make
                               sure that they are up to date.
    BasisSize   -   basis size (PBasis/SBasis/IBasis)
*************************************************************************/
typedef struct
{
    ae_int_t n;
    ae_int_t algostate;
    ae_vector xc;
    ae_bool hasxc;
    ae_vector s;
    ae_vector h;
    ae_vector activeset;
    ae_bool basisisready;
    ae_matrix sbasis;
    ae_matrix pbasis;
    ae_matrix ibasis;
    ae_int_t basissize;
    ae_bool constraintschanged;
    ae_vector hasbndl;
    ae_vector hasbndu;
    ae_vector bndl;
    ae_vector bndu;
    ae_matrix cleic;
    ae_int_t nec;
    ae_int_t nic;
    ae_vector mtx;
    ae_vector mtas;
    ae_vector cdtmp;
    ae_vector corrtmp;
    ae_vector unitdiagonal;
    snnlssolver solver;
    ae_vector scntmp;
    ae_vector tmp0;
    ae_vector tmpfeas;
    ae_matrix tmpm0;
    ae_vector rctmps;
    ae_vector rctmpg;
    ae_vector rctmprightpart;
    ae_matrix rctmpdense0;
    ae_matrix rctmpdense1;
    ae_vector rctmpisequality;
    ae_vector rctmpconstraintidx;
    ae_vector rctmplambdas;
    ae_matrix tmpbasis;
} sactiveset;


/*$ Body $*/


/*************************************************************************
This   subroutine   is   used  to initialize active set. By default, empty
N-variable model with no constraints is  generated.  Previously  allocated
buffer variables are reused as much as possible.

Two use cases for this object are described below.

CASE 1 - STEEPEST DESCENT:

    SASInit()
    repeat:
        SASReactivateConstraints()
        SASDescentDirection()
        SASExploreDirection()
        SASMoveTo()
    until convergence

CASE 1 - PRECONDITIONED STEEPEST DESCENT:

    SASInit()
    repeat:
        SASReactivateConstraintsPrec()
        SASDescentDirectionPrec()
        SASExploreDirection()
        SASMoveTo()
    until convergence

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
void sasinit(ae_int_t n, sactiveset* s, ae_state *_state);


/*************************************************************************
This function sets scaling coefficients for SAS object.

ALGLIB optimizers use scaling matrices to test stopping  conditions  (step
size and gradient are scaled before comparison with tolerances).  Scale of
the I-th variable is a translation invariant measure of:
a) "how large" the variable is
b) how large the step should be to make significant changes in the function

During orthogonalization phase, scale is used to calculate drop tolerances
(whether vector is significantly non-zero or not).

INPUT PARAMETERS:
    State   -   structure stores algorithm state
    S       -   array[N], non-zero scaling coefficients
                S[i] may be negative, sign doesn't matter.

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
void sassetscale(sactiveset* state,
     /* Real    */ ae_vector* s,
     ae_state *_state);


/*************************************************************************
Modification  of  the  preconditioner:  diagonal of approximate Hessian is
used.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    D       -   diagonal of the approximate Hessian, array[0..N-1],
                (if larger, only leading N elements are used).

NOTE 1: D[i] should be positive. Exception will be thrown otherwise.

NOTE 2: you should pass diagonal of approximate Hessian - NOT ITS INVERSE.

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
void sassetprecdiag(sactiveset* state,
     /* Real    */ ae_vector* d,
     ae_state *_state);


/*************************************************************************
This function sets/changes boundary constraints.

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

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
void sassetbc(sactiveset* state,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     ae_state *_state);


/*************************************************************************
This function sets linear constraints for SAS object.

Linear constraints are inactive by default (after initial creation).

INPUT PARAMETERS:
    State   -   SAS structure
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
    K       -   number of equality/inequality constraints, K>=0

NOTE 1: linear (non-bound) constraints are satisfied only approximately:
* there always exists some minor violation (about Epsilon in magnitude)
  due to rounding errors
* numerical differentiation, if used, may  lead  to  function  evaluations
  outside  of the feasible  area,   because   algorithm  does  NOT  change
  numerical differentiation formula according to linear constraints.
If you want constraints to be  satisfied  exactly, try to reformulate your
problem  in  such  manner  that  all constraints will become boundary ones
(this kind of constraints is always satisfied exactly, both in  the  final
solution and in all intermediate points).

  -- ALGLIB --
     Copyright 28.11.2010 by Bochkanov Sergey
*************************************************************************/
void sassetlc(sactiveset* state,
     /* Real    */ ae_matrix* c,
     /* Integer */ ae_vector* ct,
     ae_int_t k,
     ae_state *_state);


/*************************************************************************
Another variation of SASSetLC(), which accepts  linear  constraints  using
another representation.

Linear constraints are inactive by default (after initial creation).

INPUT PARAMETERS:
    State   -   SAS structure
    CLEIC   -   linear constraints, array[NEC+NIC,N+1].
                Each row of C represents one constraint:
                * first N elements correspond to coefficients,
                * last element corresponds to the right part.
                First NEC rows store equality constraints, next NIC -  are
                inequality ones.
                All elements of C (including right part) must be finite.
    NEC     -   number of equality constraints, NEC>=0
    NIC     -   number of inequality constraints, NIC>=0

NOTE 1: linear (non-bound) constraints are satisfied only approximately:
* there always exists some minor violation (about Epsilon in magnitude)
  due to rounding errors
* numerical differentiation, if used, may  lead  to  function  evaluations
  outside  of the feasible  area,   because   algorithm  does  NOT  change
  numerical differentiation formula according to linear constraints.
If you want constraints to be  satisfied  exactly, try to reformulate your
problem  in  such  manner  that  all constraints will become boundary ones
(this kind of constraints is always satisfied exactly, both in  the  final
solution and in all intermediate points).

  -- ALGLIB --
     Copyright 28.11.2010 by Bochkanov Sergey
*************************************************************************/
void sassetlcx(sactiveset* state,
     /* Real    */ ae_matrix* cleic,
     ae_int_t nec,
     ae_int_t nic,
     ae_state *_state);


/*************************************************************************
This subroutine turns on optimization mode:
1. feasibility in X is enforced  (in case X=S.XC and constraints  have not
   changed, algorithm just uses X without any modifications at all)
2. constraints are marked as "candidate" or "inactive"

INPUT PARAMETERS:
    S   -   active set object
    X   -   initial point (candidate), array[N]. It is expected that X
            contains only finite values (we do not check it).
    
OUTPUT PARAMETERS:
    S   -   state is changed
    X   -   initial point can be changed to enforce feasibility
    
RESULT:
    True in case feasible point was found (mode was changed to "optimization")
    False in case no feasible point was found (mode was not changed)

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
ae_bool sasstartoptimization(sactiveset* state,
     /* Real    */ ae_vector* x,
     ae_state *_state);


/*************************************************************************
This function explores search direction and calculates bound for  step  as
well as information for activation of constraints.

INPUT PARAMETERS:
    State       -   SAS structure which stores current point and all other
                    active set related information
    D           -   descent direction to explore

OUTPUT PARAMETERS:
    StpMax      -   upper  limit  on  step  length imposed by yet inactive
                    constraints. Can be  zero  in  case  some  constraints
                    can be activated by zero step.  Equal  to  some  large
                    value in case step is unlimited.
    CIdx        -   -1 for unlimited step, in [0,N+NEC+NIC) in case of
                    limited step.
    VVal        -   value which is assigned to X[CIdx] during activation.
                    For CIdx<0 or CIdx>=N some dummy value is assigned to
                    this parameter.
*************************************************************************/
void sasexploredirection(sactiveset* state,
     /* Real    */ ae_vector* d,
     double* stpmax,
     ae_int_t* cidx,
     double* vval,
     ae_state *_state);


/*************************************************************************
This subroutine moves current point to XN, which can be:
a) point in the direction previously explored  with  SASExploreDirection()
   function (in this case NeedAct/CIdx/CVal are used)
b) point in arbitrary direction, not necessarily previously  checked  with
   SASExploreDirection() function.

Step may activate one constraint. It is assumed than XN  is  approximately
feasible (small error as  large  as several  ulps  is  possible).   Strict
feasibility  with  respect  to  bound  constraints  is  enforced    during
activation, feasibility with respect to general linear constraints is  not
enforced.

This function activates boundary constraints, such that both is True:
1) XC[I] is not at the boundary
2) XN[I] is at the boundary or beyond it

INPUT PARAMETERS:
    S       -   active set object
    XN      -   new point.
    NeedAct -   True in case one constraint needs activation
    CIdx    -   index of constraint, in [0,N+NEC+NIC).
                Ignored if NeedAct is false.
                This value is calculated by SASExploreDirection().
    CVal    -   for CIdx in [0,N) this field stores value which is
                assigned to XC[CIdx] during activation. CVal is ignored in
                other cases.
                This value is calculated by SASExploreDirection().
    
OUTPUT PARAMETERS:
    S       -   current point and list of active constraints are changed.

RESULT:
    >0, in case at least one inactive non-candidate constraint was activated
    =0, in case only "candidate" constraints were activated
    <0, in case no constraints were activated by the step

NOTE: in general case State.XC<>XN because activation of  constraints  may
      slightly change current point (to enforce feasibility).

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
ae_int_t sasmoveto(sactiveset* state,
     /* Real    */ ae_vector* xn,
     ae_bool needact,
     ae_int_t cidx,
     double cval,
     ae_state *_state);


/*************************************************************************
This subroutine performs immediate activation of one constraint:
* "immediate" means that we do not have to move to activate it
* in case boundary constraint is activated, we enforce current point to be
  exactly at the boundary

INPUT PARAMETERS:
    S       -   active set object
    CIdx    -   index of constraint, in [0,N+NEC+NIC).
                This value is calculated by SASExploreDirection().
    CVal    -   for CIdx in [0,N) this field stores value which is
                assigned to XC[CIdx] during activation. CVal is ignored in
                other cases.
                This value is calculated by SASExploreDirection().

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
void sasimmediateactivation(sactiveset* state,
     ae_int_t cidx,
     double cval,
     ae_state *_state);


/*************************************************************************
This subroutine calculates descent direction subject to current active set.

INPUT PARAMETERS:
    S       -   active set object
    G       -   array[N], gradient
    D       -   possibly prealocated buffer;
                automatically resized if needed.
    
OUTPUT PARAMETERS:
    D       -   descent direction projected onto current active set.
                Components of D which correspond to active boundary
                constraints are forced to be exactly zero.
                In case D is non-zero, it is normalized to have unit norm.
                
NOTE: in  case active set has N  active  constraints  (or  more),  descent
      direction is forced to be exactly zero.

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
void sasconstraineddescent(sactiveset* state,
     /* Real    */ ae_vector* g,
     /* Real    */ ae_vector* d,
     ae_state *_state);


/*************************************************************************
This  subroutine  calculates  preconditioned  descent direction subject to
current active set.

INPUT PARAMETERS:
    S       -   active set object
    G       -   array[N], gradient
    D       -   possibly prealocated buffer;
                automatically resized if needed.
    
OUTPUT PARAMETERS:
    D       -   descent direction projected onto current active set.
                Components of D which correspond to active boundary
                constraints are forced to be exactly zero.
                In case D is non-zero, it is normalized to have unit norm.
                
NOTE: in  case active set has N  active  constraints  (or  more),  descent
      direction is forced to be exactly zero.

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
void sasconstraineddescentprec(sactiveset* state,
     /* Real    */ ae_vector* g,
     /* Real    */ ae_vector* d,
     ae_state *_state);


/*************************************************************************
This subroutine calculates projection   of  direction  vector  to  current
active set.

INPUT PARAMETERS:
    S       -   active set object
    D       -   array[N], direction
    
OUTPUT PARAMETERS:
    D       -   direction projected onto current active set.
                Components of D which correspond to active boundary
                constraints are forced to be exactly zero.
                
NOTE: in  case active set has N  active  constraints  (or  more),  descent
      direction is forced to be exactly zero.

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
void sasconstraineddirection(sactiveset* state,
     /* Real    */ ae_vector* d,
     ae_state *_state);


/*************************************************************************
This subroutine calculates product of direction vector and  preconditioner
multiplied subject to current active set.

INPUT PARAMETERS:
    S       -   active set object
    D       -   array[N], direction
    
OUTPUT PARAMETERS:
    D       -   preconditioned direction projected onto current active set.
                Components of D which correspond to active boundary
                constraints are forced to be exactly zero.
                
NOTE: in  case active set has N  active  constraints  (or  more),  descent
      direction is forced to be exactly zero.

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
void sasconstraineddirectionprec(sactiveset* state,
     /* Real    */ ae_vector* d,
     ae_state *_state);


/*************************************************************************
This  subroutine  performs  correction of some (possibly infeasible) point
with respect to a) current active set, b) all boundary  constraints,  both
active and inactive:

0) we calculate L1 penalty term for violation of active linear constraints
   (one which is returned by SASActiveLCPenalty1() function).
1) first, it performs projection (orthogonal with respect to scale  matrix
   S) of X into current active set: X -> X1.
2) next, we perform projection with respect to  ALL  boundary  constraints
   which are violated at X1: X1 -> X2.
3) X is replaced by X2.

The idea is that this function can preserve and enforce feasibility during
optimization, and additional penalty parameter can be used to prevent algo
from leaving feasible set because of rounding errors.

INPUT PARAMETERS:
    S       -   active set object
    X       -   array[N], candidate point
    
OUTPUT PARAMETERS:
    X       -   "improved" candidate point:
                a) feasible with respect to all boundary constraints
                b) feasibility with respect to active set is retained at
                   good level.
    Penalty -   penalty term, which can be added to function value if user
                wants to penalize violation of constraints (recommended).
                
NOTE: this function is not intended to find exact  projection  (i.e.  best
      approximation) of X into feasible set. It just improves situation  a
      bit.
      Regular  use  of   this function will help you to retain feasibility
      - if you already have something to start  with  and  constrain  your
      steps is such way that the only source of infeasibility are roundoff
      errors.

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
void sascorrection(sactiveset* state,
     /* Real    */ ae_vector* x,
     double* penalty,
     ae_state *_state);


/*************************************************************************
This  subroutine returns L1 penalty for violation of active general linear
constraints (violation of boundary or inactive linear constraints  is  not
added to penalty).

Penalty term is equal to:
    
    Penalty = SUM( Abs((C_i*x-R_i)/Alpha_i) )
    
Here:
* summation is performed for I=0...NEC+NIC-1, ActiveSet[N+I]>0
  (only for rows of CLEIC which are in active set)
* C_i is I-th row of CLEIC
* R_i is corresponding right part
* S is a scale matrix
* Alpha_i = ||S*C_i|| - is a scaling coefficient which "normalizes"
  I-th summation term according to its scale.

INPUT PARAMETERS:
    S       -   active set object
    X       -   array[N], candidate point

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
double sasactivelcpenalty1(sactiveset* state,
     /* Real    */ ae_vector* x,
     ae_state *_state);


/*************************************************************************
This subroutine calculates scaled norm of  vector  after  projection  onto
subspace of active constraints. Most often this function is used  to  test
stopping conditions.

INPUT PARAMETERS:
    S       -   active set object
    D       -   vector whose norm is calculated
    
RESULT:
    Vector norm (after projection and scaling)
    
NOTE: projection is performed first, scaling is performed after projection
                
NOTE: if we have N active constraints, zero value (exact zero) is returned

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
double sasscaledconstrainednorm(sactiveset* state,
     /* Real    */ ae_vector* d,
     ae_state *_state);


/*************************************************************************
This subroutine turns off optimization mode.

INPUT PARAMETERS:
    S   -   active set object
    
OUTPUT PARAMETERS:
    S   -   state is changed

NOTE: this function can be called many times for optimizer which was
      already stopped.

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
void sasstopoptimization(sactiveset* state, ae_state *_state);


/*************************************************************************
This function recalculates constraints - activates  and  deactivates  them
according to gradient value at current point. Algorithm  assumes  that  we
want to make steepest descent step from  current  point;  constraints  are
activated and deactivated in such way that we won't violate any constraint
by steepest descent step.

After call to this function active set is ready to  try  steepest  descent
step (SASDescentDirection-SASExploreDirection-SASMoveTo).

Only already "active" and "candidate" elements of ActiveSet are  examined;
constraints which are not active are not examined.

INPUT PARAMETERS:
    State       -   active set object
    GC          -   array[N], gradient at XC
    
OUTPUT PARAMETERS:
    State       -   active set object, with new set of constraint

  -- ALGLIB --
     Copyright 26.09.2012 by Bochkanov Sergey
*************************************************************************/
void sasreactivateconstraints(sactiveset* state,
     /* Real    */ ae_vector* gc,
     ae_state *_state);


/*************************************************************************
This function recalculates constraints - activates  and  deactivates  them
according to gradient value at current point.

Algorithm  assumes  that  we  want  to make Quasi-Newton step from current
point with diagonal Quasi-Newton matrix H. Constraints are  activated  and
deactivated in such way that we won't violate any constraint by step.

After call to  this  function  active set is ready to  try  preconditioned
steepest descent step (SASDescentDirection-SASExploreDirection-SASMoveTo).

Only already "active" and "candidate" elements of ActiveSet are  examined;
constraints which are not active are not examined.

INPUT PARAMETERS:
    State       -   active set object
    GC          -   array[N], gradient at XC
    
OUTPUT PARAMETERS:
    State       -   active set object, with new set of constraint

  -- ALGLIB --
     Copyright 26.09.2012 by Bochkanov Sergey
*************************************************************************/
void sasreactivateconstraintsprec(sactiveset* state,
     /* Real    */ ae_vector* gc,
     ae_state *_state);


/*************************************************************************
This function builds three orthonormal basises for current active set:
* P-orthogonal one, which is orthogonalized with inner product
  (x,y) = x'*P*y, where P=inv(H) is current preconditioner
* S-orthogonal one, which is orthogonalized with inner product
  (x,y) = x'*S'*S*y, where S is diagonal scaling matrix
* I-orthogonal one, which is orthogonalized with standard dot product

NOTE: all sets of orthogonal vectors are guaranteed  to  have  same  size.
      P-orthogonal basis is built first, I/S-orthogonal basises are forced
      to have same number of vectors as P-orthogonal one (padded  by  zero
      vectors if needed).
      
NOTE: this function tracks changes in active set; first call  will  result
      in reorthogonalization

INPUT PARAMETERS:
    State   -   active set object
    H       -   diagonal preconditioner, H[i]>0

OUTPUT PARAMETERS:
    State   -   active set object with new basis
    
  -- ALGLIB --
     Copyright 20.06.2012 by Bochkanov Sergey
*************************************************************************/
void sasrebuildbasis(sactiveset* state, ae_state *_state);
void _sactiveset_init(void* _p, ae_state *_state);
void _sactiveset_init_copy(void* _dst, void* _src, ae_state *_state);
void _sactiveset_clear(void* _p);
void _sactiveset_destroy(void* _p);


/*$ End $*/
#endif

