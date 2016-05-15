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

#ifndef _mcpd_h
#define _mcpd_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "linmin.h"
#include "tsort.h"
#include "reflections.h"
#include "creflections.h"
#include "hqrnd.h"
#include "matgen.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "sparse.h"
#include "rotations.h"
#include "trfac.h"
#include "trlinsolve.h"
#include "safesolve.h"
#include "rcond.h"
#include "matinv.h"
#include "hblas.h"
#include "sblas.h"
#include "ortfac.h"
#include "blas.h"
#include "bdsvd.h"
#include "svd.h"
#include "optserv.h"
#include "fbls.h"
#include "cqmodels.h"
#include "snnls.h"
#include "sactivesets.h"
#include "mincg.h"
#include "xblas.h"
#include "densesolver.h"
#include "minbleic.h"


/*$ Declarations $*/


/*************************************************************************
This structure is a MCPD (Markov Chains for Population Data) solver.

You should use ALGLIB functions in order to work with this object.

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_int_t n;
    ae_vector states;
    ae_int_t npairs;
    ae_matrix data;
    ae_matrix ec;
    ae_matrix bndl;
    ae_matrix bndu;
    ae_matrix c;
    ae_vector ct;
    ae_int_t ccnt;
    ae_vector pw;
    ae_matrix priorp;
    double regterm;
    minbleicstate bs;
    ae_int_t repinneriterationscount;
    ae_int_t repouteriterationscount;
    ae_int_t repnfev;
    ae_int_t repterminationtype;
    minbleicreport br;
    ae_vector tmpp;
    ae_vector effectivew;
    ae_vector effectivebndl;
    ae_vector effectivebndu;
    ae_matrix effectivec;
    ae_vector effectivect;
    ae_vector h;
    ae_matrix p;
} mcpdstate;


/*************************************************************************
This structure is a MCPD training report:
    InnerIterationsCount    -   number of inner iterations of the
                                underlying optimization algorithm
    OuterIterationsCount    -   number of outer iterations of the
                                underlying optimization algorithm
    NFEV                    -   number of merit function evaluations
    TerminationType         -   termination type
                                (same as for MinBLEIC optimizer, positive
                                values denote success, negative ones -
                                failure)

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_int_t inneriterationscount;
    ae_int_t outeriterationscount;
    ae_int_t nfev;
    ae_int_t terminationtype;
} mcpdreport;


/*$ Body $*/


/*************************************************************************
DESCRIPTION:

This function creates MCPD (Markov Chains for Population Data) solver.

This  solver  can  be  used  to find transition matrix P for N-dimensional
prediction  problem  where transition from X[i] to X[i+1] is  modelled  as
    X[i+1] = P*X[i]
where X[i] and X[i+1] are N-dimensional population vectors (components  of
each X are non-negative), and P is a N*N transition matrix (elements of  P
are non-negative, each column sums to 1.0).

Such models arise when when:
* there is some population of individuals
* individuals can have different states
* individuals can transit from one state to another
* population size is constant, i.e. there is no new individuals and no one
  leaves population
* you want to model transitions of individuals from one state into another

USAGE:

Here we give very brief outline of the MCPD. We strongly recommend you  to
read examples in the ALGLIB Reference Manual and to read ALGLIB User Guide
on data analysis which is available at http://www.alglib.net/dataanalysis/

1. User initializes algorithm state with MCPDCreate() call

2. User  adds  one  or  more  tracks -  sequences of states which describe
   evolution of a system being modelled from different starting conditions

3. User may add optional boundary, equality  and/or  linear constraints on
   the coefficients of P by calling one of the following functions:
   * MCPDSetEC() to set equality constraints
   * MCPDSetBC() to set bound constraints
   * MCPDSetLC() to set linear constraints

4. Optionally,  user  may  set  custom  weights  for prediction errors (by
   default, algorithm assigns non-equal, automatically chosen weights  for
   errors in the prediction of different components of X). It can be  done
   with a call of MCPDSetPredictionWeights() function.

5. User calls MCPDSolve() function which takes algorithm  state and
   pointer (delegate, etc.) to callback function which calculates F/G.

6. User calls MCPDResults() to get solution

INPUT PARAMETERS:
    N       -   problem dimension, N>=1

OUTPUT PARAMETERS:
    State   -   structure stores algorithm state

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdcreate(ae_int_t n, mcpdstate* s, ae_state *_state);


/*************************************************************************
DESCRIPTION:

This function is a specialized version of MCPDCreate()  function,  and  we
recommend  you  to read comments for this function for general information
about MCPD solver.

This  function  creates  MCPD (Markov Chains for Population  Data)  solver
for "Entry-state" model,  i.e. model  where transition from X[i] to X[i+1]
is modelled as
    X[i+1] = P*X[i]
where
    X[i] and X[i+1] are N-dimensional state vectors
    P is a N*N transition matrix
and  one  selected component of X[] is called "entry" state and is treated
in a special way:
    system state always transits from "entry" state to some another state
    system state can not transit from any state into "entry" state
Such conditions basically mean that row of P which corresponds to  "entry"
state is zero.

Such models arise when:
* there is some population of individuals
* individuals can have different states
* individuals can transit from one state to another
* population size is NOT constant -  at every moment of time there is some
  (unpredictable) amount of "new" individuals, which can transit into  one
  of the states at the next turn, but still no one leaves population
* you want to model transitions of individuals from one state into another
* but you do NOT want to predict amount of "new"  individuals  because  it
  does not depends on individuals already present (hence  system  can  not
  transit INTO entry state - it can only transit FROM it).

This model is discussed  in  more  details  in  the ALGLIB User Guide (see
http://www.alglib.net/dataanalysis/ for more data).

INPUT PARAMETERS:
    N       -   problem dimension, N>=2
    EntryState- index of entry state, in 0..N-1

OUTPUT PARAMETERS:
    State   -   structure stores algorithm state

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdcreateentry(ae_int_t n,
     ae_int_t entrystate,
     mcpdstate* s,
     ae_state *_state);


/*************************************************************************
DESCRIPTION:

This function is a specialized version of MCPDCreate()  function,  and  we
recommend  you  to read comments for this function for general information
about MCPD solver.

This  function  creates  MCPD (Markov Chains for Population  Data)  solver
for "Exit-state" model,  i.e. model  where  transition from X[i] to X[i+1]
is modelled as
    X[i+1] = P*X[i]
where
    X[i] and X[i+1] are N-dimensional state vectors
    P is a N*N transition matrix
and  one  selected component of X[] is called "exit"  state and is treated
in a special way:
    system state can transit from any state into "exit" state
    system state can not transit from "exit" state into any other state
    transition operator discards "exit" state (makes it zero at each turn)
Such  conditions  basically  mean  that  column  of P which corresponds to
"exit" state is zero. Multiplication by such P may decrease sum of  vector
components.

Such models arise when:
* there is some population of individuals
* individuals can have different states
* individuals can transit from one state to another
* population size is NOT constant - individuals can move into "exit" state
  and leave population at the next turn, but there are no new individuals
* amount of individuals which leave population can be predicted
* you want to model transitions of individuals from one state into another
  (including transitions into the "exit" state)

This model is discussed  in  more  details  in  the ALGLIB User Guide (see
http://www.alglib.net/dataanalysis/ for more data).

INPUT PARAMETERS:
    N       -   problem dimension, N>=2
    ExitState-  index of exit state, in 0..N-1

OUTPUT PARAMETERS:
    State   -   structure stores algorithm state

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdcreateexit(ae_int_t n,
     ae_int_t exitstate,
     mcpdstate* s,
     ae_state *_state);


/*************************************************************************
DESCRIPTION:

This function is a specialized version of MCPDCreate()  function,  and  we
recommend  you  to read comments for this function for general information
about MCPD solver.

This  function  creates  MCPD (Markov Chains for Population  Data)  solver
for "Entry-Exit-states" model, i.e. model where  transition  from  X[i] to
X[i+1] is modelled as
    X[i+1] = P*X[i]
where
    X[i] and X[i+1] are N-dimensional state vectors
    P is a N*N transition matrix
one selected component of X[] is called "entry" state and is treated in  a
special way:
    system state always transits from "entry" state to some another state
    system state can not transit from any state into "entry" state
and another one component of X[] is called "exit" state and is treated  in
a special way too:
    system state can transit from any state into "exit" state
    system state can not transit from "exit" state into any other state
    transition operator discards "exit" state (makes it zero at each turn)
Such conditions basically mean that:
    row of P which corresponds to "entry" state is zero
    column of P which corresponds to "exit" state is zero
Multiplication by such P may decrease sum of vector components.

Such models arise when:
* there is some population of individuals
* individuals can have different states
* individuals can transit from one state to another
* population size is NOT constant
* at every moment of time there is some (unpredictable)  amount  of  "new"
  individuals, which can transit into one of the states at the next turn
* some  individuals  can  move  (predictably)  into "exit" state and leave
  population at the next turn
* you want to model transitions of individuals from one state into another,
  including transitions from the "entry" state and into the "exit" state.
* but you do NOT want to predict amount of "new"  individuals  because  it
  does not depends on individuals already present (hence  system  can  not
  transit INTO entry state - it can only transit FROM it).

This model is discussed  in  more  details  in  the ALGLIB User Guide (see
http://www.alglib.net/dataanalysis/ for more data).

INPUT PARAMETERS:
    N       -   problem dimension, N>=2
    EntryState- index of entry state, in 0..N-1
    ExitState-  index of exit state, in 0..N-1

OUTPUT PARAMETERS:
    State   -   structure stores algorithm state

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdcreateentryexit(ae_int_t n,
     ae_int_t entrystate,
     ae_int_t exitstate,
     mcpdstate* s,
     ae_state *_state);


/*************************************************************************
This  function  is  used to add a track - sequence of system states at the
different moments of its evolution.

You  may  add  one  or several tracks to the MCPD solver. In case you have
several tracks, they won't overwrite each other. For example,  if you pass
two tracks, A1-A2-A3 (system at t=A+1, t=A+2 and t=A+3) and B1-B2-B3, then
solver will try to model transitions from t=A+1 to t=A+2, t=A+2 to  t=A+3,
t=B+1 to t=B+2, t=B+2 to t=B+3. But it WONT mix these two tracks - i.e. it
wont try to model transition from t=A+3 to t=B+1.

INPUT PARAMETERS:
    S       -   solver
    XY      -   track, array[K,N]:
                * I-th row is a state at t=I
                * elements of XY must be non-negative (exception will be
                  thrown on negative elements)
    K       -   number of points in a track
                * if given, only leading K rows of XY are used
                * if not given, automatically determined from size of XY

NOTES:

1. Track may contain either proportional or population data:
   * with proportional data all rows of XY must sum to 1.0, i.e. we have
     proportions instead of absolute population values
   * with population data rows of XY contain population counts and generally
     do not sum to 1.0 (although they still must be non-negative)

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdaddtrack(mcpdstate* s,
     /* Real    */ ae_matrix* xy,
     ae_int_t k,
     ae_state *_state);


/*************************************************************************
This function is used to add equality constraints on the elements  of  the
transition matrix P.

MCPD solver has four types of constraints which can be placed on P:
* user-specified equality constraints (optional)
* user-specified bound constraints (optional)
* user-specified general linear constraints (optional)
* basic constraints (always present):
  * non-negativity: P[i,j]>=0
  * consistency: every column of P sums to 1.0

Final  constraints  which  are  passed  to  the  underlying  optimizer are
calculated  as  intersection  of all present constraints. For example, you
may specify boundary constraint on P[0,0] and equality one:
    0.1<=P[0,0]<=0.9
    P[0,0]=0.5
Such  combination  of  constraints  will  be  silently  reduced  to  their
intersection, which is P[0,0]=0.5.

This  function  can  be  used  to  place equality constraints on arbitrary
subset of elements of P. Set of constraints is specified by EC, which  may
contain either NAN's or finite numbers from [0,1]. NAN denotes absence  of
constraint, finite number denotes equality constraint on specific  element
of P.

You can also  use  MCPDAddEC()  function  which  allows  to  ADD  equality
constraint  for  one  element  of P without changing constraints for other
elements.

These functions (MCPDSetEC and MCPDAddEC) interact as follows:
* there is internal matrix of equality constraints which is stored in  the
  MCPD solver
* MCPDSetEC() replaces this matrix by another one (SET)
* MCPDAddEC() modifies one element of this matrix and  leaves  other  ones
  unchanged (ADD)
* thus  MCPDAddEC()  call  preserves  all  modifications  done by previous
  calls,  while  MCPDSetEC()  completely discards all changes  done to the
  equality constraints.

INPUT PARAMETERS:
    S       -   solver
    EC      -   equality constraints, array[N,N]. Elements of  EC  can  be
                either NAN's or finite  numbers from  [0,1].  NAN  denotes
                absence  of  constraints,  while  finite  value    denotes
                equality constraint on the corresponding element of P.

NOTES:

1. infinite values of EC will lead to exception being thrown. Values  less
than 0.0 or greater than 1.0 will lead to error code being returned  after
call to MCPDSolve().

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdsetec(mcpdstate* s,
     /* Real    */ ae_matrix* ec,
     ae_state *_state);


/*************************************************************************
This function is used to add equality constraints on the elements  of  the
transition matrix P.

MCPD solver has four types of constraints which can be placed on P:
* user-specified equality constraints (optional)
* user-specified bound constraints (optional)
* user-specified general linear constraints (optional)
* basic constraints (always present):
  * non-negativity: P[i,j]>=0
  * consistency: every column of P sums to 1.0

Final  constraints  which  are  passed  to  the  underlying  optimizer are
calculated  as  intersection  of all present constraints. For example, you
may specify boundary constraint on P[0,0] and equality one:
    0.1<=P[0,0]<=0.9
    P[0,0]=0.5
Such  combination  of  constraints  will  be  silently  reduced  to  their
intersection, which is P[0,0]=0.5.

This function can be used to ADD equality constraint for one element of  P
without changing constraints for other elements.

You  can  also  use  MCPDSetEC()  function  which  allows  you  to specify
arbitrary set of equality constraints in one call.

These functions (MCPDSetEC and MCPDAddEC) interact as follows:
* there is internal matrix of equality constraints which is stored in the
  MCPD solver
* MCPDSetEC() replaces this matrix by another one (SET)
* MCPDAddEC() modifies one element of this matrix and leaves  other  ones
  unchanged (ADD)
* thus  MCPDAddEC()  call  preserves  all  modifications done by previous
  calls,  while  MCPDSetEC()  completely discards all changes done to the
  equality constraints.

INPUT PARAMETERS:
    S       -   solver
    I       -   row index of element being constrained
    J       -   column index of element being constrained
    C       -   value (constraint for P[I,J]).  Can  be  either  NAN  (no
                constraint) or finite value from [0,1].
                
NOTES:

1. infinite values of C  will lead to exception being thrown. Values  less
than 0.0 or greater than 1.0 will lead to error code being returned  after
call to MCPDSolve().

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdaddec(mcpdstate* s,
     ae_int_t i,
     ae_int_t j,
     double c,
     ae_state *_state);


/*************************************************************************
This function is used to add bound constraints  on  the  elements  of  the
transition matrix P.

MCPD solver has four types of constraints which can be placed on P:
* user-specified equality constraints (optional)
* user-specified bound constraints (optional)
* user-specified general linear constraints (optional)
* basic constraints (always present):
  * non-negativity: P[i,j]>=0
  * consistency: every column of P sums to 1.0

Final  constraints  which  are  passed  to  the  underlying  optimizer are
calculated  as  intersection  of all present constraints. For example, you
may specify boundary constraint on P[0,0] and equality one:
    0.1<=P[0,0]<=0.9
    P[0,0]=0.5
Such  combination  of  constraints  will  be  silently  reduced  to  their
intersection, which is P[0,0]=0.5.

This  function  can  be  used  to  place bound   constraints  on arbitrary
subset  of  elements  of  P.  Set of constraints is specified by BndL/BndU
matrices, which may contain arbitrary combination  of  finite  numbers  or
infinities (like -INF<x<=0.5 or 0.1<=x<+INF).

You can also use MCPDAddBC() function which allows to ADD bound constraint
for one element of P without changing constraints for other elements.

These functions (MCPDSetBC and MCPDAddBC) interact as follows:
* there is internal matrix of bound constraints which is stored in the
  MCPD solver
* MCPDSetBC() replaces this matrix by another one (SET)
* MCPDAddBC() modifies one element of this matrix and  leaves  other  ones
  unchanged (ADD)
* thus  MCPDAddBC()  call  preserves  all  modifications  done by previous
  calls,  while  MCPDSetBC()  completely discards all changes  done to the
  equality constraints.

INPUT PARAMETERS:
    S       -   solver
    BndL    -   lower bounds constraints, array[N,N]. Elements of BndL can
                be finite numbers or -INF.
    BndU    -   upper bounds constraints, array[N,N]. Elements of BndU can
                be finite numbers or +INF.

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdsetbc(mcpdstate* s,
     /* Real    */ ae_matrix* bndl,
     /* Real    */ ae_matrix* bndu,
     ae_state *_state);


/*************************************************************************
This function is used to add bound constraints  on  the  elements  of  the
transition matrix P.

MCPD solver has four types of constraints which can be placed on P:
* user-specified equality constraints (optional)
* user-specified bound constraints (optional)
* user-specified general linear constraints (optional)
* basic constraints (always present):
  * non-negativity: P[i,j]>=0
  * consistency: every column of P sums to 1.0

Final  constraints  which  are  passed  to  the  underlying  optimizer are
calculated  as  intersection  of all present constraints. For example, you
may specify boundary constraint on P[0,0] and equality one:
    0.1<=P[0,0]<=0.9
    P[0,0]=0.5
Such  combination  of  constraints  will  be  silently  reduced  to  their
intersection, which is P[0,0]=0.5.

This  function  can  be  used to ADD bound constraint for one element of P
without changing constraints for other elements.

You  can  also  use  MCPDSetBC()  function  which  allows to  place  bound
constraints  on arbitrary subset of elements of P.   Set of constraints is
specified  by  BndL/BndU matrices, which may contain arbitrary combination
of finite numbers or infinities (like -INF<x<=0.5 or 0.1<=x<+INF).

These functions (MCPDSetBC and MCPDAddBC) interact as follows:
* there is internal matrix of bound constraints which is stored in the
  MCPD solver
* MCPDSetBC() replaces this matrix by another one (SET)
* MCPDAddBC() modifies one element of this matrix and  leaves  other  ones
  unchanged (ADD)
* thus  MCPDAddBC()  call  preserves  all  modifications  done by previous
  calls,  while  MCPDSetBC()  completely discards all changes  done to the
  equality constraints.

INPUT PARAMETERS:
    S       -   solver
    I       -   row index of element being constrained
    J       -   column index of element being constrained
    BndL    -   lower bound
    BndU    -   upper bound

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdaddbc(mcpdstate* s,
     ae_int_t i,
     ae_int_t j,
     double bndl,
     double bndu,
     ae_state *_state);


/*************************************************************************
This function is used to set linear equality/inequality constraints on the
elements of the transition matrix P.

This function can be used to set one or several general linear constraints
on the elements of P. Two types of constraints are supported:
* equality constraints
* inequality constraints (both less-or-equal and greater-or-equal)

Coefficients  of  constraints  are  specified  by  matrix  C (one  of  the
parameters).  One  row  of  C  corresponds  to  one  constraint.   Because
transition  matrix P has N*N elements,  we  need  N*N columns to store all
coefficients  (they  are  stored row by row), and one more column to store
right part - hence C has N*N+1 columns.  Constraint  kind is stored in the
CT array.

Thus, I-th linear constraint is
    P[0,0]*C[I,0] + P[0,1]*C[I,1] + .. + P[0,N-1]*C[I,N-1] +
        + P[1,0]*C[I,N] + P[1,1]*C[I,N+1] + ... +
        + P[N-1,N-1]*C[I,N*N-1]  ?=?  C[I,N*N]
where ?=? can be either "=" (CT[i]=0), "<=" (CT[i]<0) or ">=" (CT[i]>0).

Your constraint may involve only some subset of P (less than N*N elements).
For example it can be something like
    P[0,0] + P[0,1] = 0.5
In this case you still should pass matrix  with N*N+1 columns, but all its
elements (except for C[0,0], C[0,1] and C[0,N*N-1]) will be zero.

INPUT PARAMETERS:
    S       -   solver
    C       -   array[K,N*N+1] - coefficients of constraints
                (see above for complete description)
    CT      -   array[K] - constraint types
                (see above for complete description)
    K       -   number of equality/inequality constraints, K>=0:
                * if given, only leading K elements of C/CT are used
                * if not given, automatically determined from sizes of C/CT

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdsetlc(mcpdstate* s,
     /* Real    */ ae_matrix* c,
     /* Integer */ ae_vector* ct,
     ae_int_t k,
     ae_state *_state);


/*************************************************************************
This function allows to  tune  amount  of  Tikhonov  regularization  being
applied to your problem.

By default, regularizing term is equal to r*||P-prior_P||^2, where r is  a
small non-zero value,  P is transition matrix, prior_P is identity matrix,
||X||^2 is a sum of squared elements of X.

This  function  allows  you to change coefficient r. You can  also  change
prior values with MCPDSetPrior() function.

INPUT PARAMETERS:
    S       -   solver
    V       -   regularization  coefficient, finite non-negative value. It
                is  not  recommended  to specify zero value unless you are
                pretty sure that you want it.

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdsettikhonovregularizer(mcpdstate* s, double v, ae_state *_state);


/*************************************************************************
This  function  allows to set prior values used for regularization of your
problem.

By default, regularizing term is equal to r*||P-prior_P||^2, where r is  a
small non-zero value,  P is transition matrix, prior_P is identity matrix,
||X||^2 is a sum of squared elements of X.

This  function  allows  you to change prior values prior_P. You  can  also
change r with MCPDSetTikhonovRegularizer() function.

INPUT PARAMETERS:
    S       -   solver
    PP      -   array[N,N], matrix of prior values:
                1. elements must be real numbers from [0,1]
                2. columns must sum to 1.0.
                First property is checked (exception is thrown otherwise),
                while second one is not checked/enforced.

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdsetprior(mcpdstate* s,
     /* Real    */ ae_matrix* pp,
     ae_state *_state);


/*************************************************************************
This function is used to change prediction weights

MCPD solver scales prediction errors as follows
    Error(P) = ||W*(y-P*x)||^2
where
    x is a system state at time t
    y is a system state at time t+1
    P is a transition matrix
    W is a diagonal scaling matrix

By default, weights are chosen in order  to  minimize  relative prediction
error instead of absolute one. For example, if one component of  state  is
about 0.5 in magnitude and another one is about 0.05, then algorithm  will
make corresponding weights equal to 2.0 and 20.0.

INPUT PARAMETERS:
    S       -   solver
    PW      -   array[N], weights:
                * must be non-negative values (exception will be thrown otherwise)
                * zero values will be replaced by automatically chosen values

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdsetpredictionweights(mcpdstate* s,
     /* Real    */ ae_vector* pw,
     ae_state *_state);


/*************************************************************************
This function is used to start solution of the MCPD problem.

After return from this function, you can use MCPDResults() to get solution
and completion code.

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdsolve(mcpdstate* s, ae_state *_state);


/*************************************************************************
MCPD results

INPUT PARAMETERS:
    State   -   algorithm state

OUTPUT PARAMETERS:
    P       -   array[N,N], transition matrix
    Rep     -   optimization report. You should check Rep.TerminationType
                in  order  to  distinguish  successful  termination  from
                unsuccessful one. Speaking short, positive values  denote
                success, negative ones are failures.
                More information about fields of this  structure  can  be
                found in the comments on MCPDReport datatype.


  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdresults(mcpdstate* s,
     /* Real    */ ae_matrix* p,
     mcpdreport* rep,
     ae_state *_state);
void _mcpdstate_init(void* _p, ae_state *_state);
void _mcpdstate_init_copy(void* _dst, void* _src, ae_state *_state);
void _mcpdstate_clear(void* _p);
void _mcpdstate_destroy(void* _p);
void _mcpdreport_init(void* _p, ae_state *_state);
void _mcpdreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _mcpdreport_clear(void* _p);
void _mcpdreport_destroy(void* _p);


/*$ End $*/
#endif

