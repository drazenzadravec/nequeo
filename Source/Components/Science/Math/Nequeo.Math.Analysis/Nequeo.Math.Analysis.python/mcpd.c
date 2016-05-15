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
#include "mcpd.h"


/*$ Declarations $*/
static double mcpd_xtol = 1.0E-8;
static void mcpd_mcpdinit(ae_int_t n,
     ae_int_t entrystate,
     ae_int_t exitstate,
     mcpdstate* s,
     ae_state *_state);


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
void mcpdcreate(ae_int_t n, mcpdstate* s, ae_state *_state)
{

    _mcpdstate_clear(s);

    ae_assert(n>=1, "MCPDCreate: N<1", _state);
    mcpd_mcpdinit(n, -1, -1, s, _state);
}


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
     ae_state *_state)
{

    _mcpdstate_clear(s);

    ae_assert(n>=2, "MCPDCreateEntry: N<2", _state);
    ae_assert(entrystate>=0, "MCPDCreateEntry: EntryState<0", _state);
    ae_assert(entrystate<n, "MCPDCreateEntry: EntryState>=N", _state);
    mcpd_mcpdinit(n, entrystate, -1, s, _state);
}


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
     ae_state *_state)
{

    _mcpdstate_clear(s);

    ae_assert(n>=2, "MCPDCreateExit: N<2", _state);
    ae_assert(exitstate>=0, "MCPDCreateExit: ExitState<0", _state);
    ae_assert(exitstate<n, "MCPDCreateExit: ExitState>=N", _state);
    mcpd_mcpdinit(n, -1, exitstate, s, _state);
}


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
     ae_state *_state)
{

    _mcpdstate_clear(s);

    ae_assert(n>=2, "MCPDCreateEntryExit: N<2", _state);
    ae_assert(entrystate>=0, "MCPDCreateEntryExit: EntryState<0", _state);
    ae_assert(entrystate<n, "MCPDCreateEntryExit: EntryState>=N", _state);
    ae_assert(exitstate>=0, "MCPDCreateEntryExit: ExitState<0", _state);
    ae_assert(exitstate<n, "MCPDCreateEntryExit: ExitState>=N", _state);
    ae_assert(entrystate!=exitstate, "MCPDCreateEntryExit: EntryState=ExitState", _state);
    mcpd_mcpdinit(n, entrystate, exitstate, s, _state);
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;
    double s0;
    double s1;


    n = s->n;
    ae_assert(k>=0, "MCPDAddTrack: K<0", _state);
    ae_assert(xy->cols>=n, "MCPDAddTrack: Cols(XY)<N", _state);
    ae_assert(xy->rows>=k, "MCPDAddTrack: Rows(XY)<K", _state);
    ae_assert(apservisfinitematrix(xy, k, n, _state), "MCPDAddTrack: XY contains infinite or NaN elements", _state);
    for(i=0; i<=k-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            ae_assert(ae_fp_greater_eq(xy->ptr.pp_double[i][j],(double)(0)), "MCPDAddTrack: XY contains negative elements", _state);
        }
    }
    if( k<2 )
    {
        return;
    }
    if( s->data.rows<s->npairs+k-1 )
    {
        rmatrixresize(&s->data, ae_maxint(2*s->data.rows, s->npairs+k-1, _state), 2*n, _state);
    }
    for(i=0; i<=k-2; i++)
    {
        s0 = (double)(0);
        s1 = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            if( s->states.ptr.p_int[j]>=0 )
            {
                s0 = s0+xy->ptr.pp_double[i][j];
            }
            if( s->states.ptr.p_int[j]<=0 )
            {
                s1 = s1+xy->ptr.pp_double[i+1][j];
            }
        }
        if( ae_fp_greater(s0,(double)(0))&&ae_fp_greater(s1,(double)(0)) )
        {
            for(j=0; j<=n-1; j++)
            {
                if( s->states.ptr.p_int[j]>=0 )
                {
                    s->data.ptr.pp_double[s->npairs][j] = xy->ptr.pp_double[i][j]/s0;
                }
                else
                {
                    s->data.ptr.pp_double[s->npairs][j] = 0.0;
                }
                if( s->states.ptr.p_int[j]<=0 )
                {
                    s->data.ptr.pp_double[s->npairs][n+j] = xy->ptr.pp_double[i+1][j]/s1;
                }
                else
                {
                    s->data.ptr.pp_double[s->npairs][n+j] = 0.0;
                }
            }
            s->npairs = s->npairs+1;
        }
    }
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;


    n = s->n;
    ae_assert(ec->cols>=n, "MCPDSetEC: Cols(EC)<N", _state);
    ae_assert(ec->rows>=n, "MCPDSetEC: Rows(EC)<N", _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            ae_assert(ae_isfinite(ec->ptr.pp_double[i][j], _state)||ae_isnan(ec->ptr.pp_double[i][j], _state), "MCPDSetEC: EC containts infinite elements", _state);
            s->ec.ptr.pp_double[i][j] = ec->ptr.pp_double[i][j];
        }
    }
}


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
     ae_state *_state)
{


    ae_assert(i>=0, "MCPDAddEC: I<0", _state);
    ae_assert(i<s->n, "MCPDAddEC: I>=N", _state);
    ae_assert(j>=0, "MCPDAddEC: J<0", _state);
    ae_assert(j<s->n, "MCPDAddEC: J>=N", _state);
    ae_assert(ae_isnan(c, _state)||ae_isfinite(c, _state), "MCPDAddEC: C is not finite number or NAN", _state);
    s->ec.ptr.pp_double[i][j] = c;
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;


    n = s->n;
    ae_assert(bndl->cols>=n, "MCPDSetBC: Cols(BndL)<N", _state);
    ae_assert(bndl->rows>=n, "MCPDSetBC: Rows(BndL)<N", _state);
    ae_assert(bndu->cols>=n, "MCPDSetBC: Cols(BndU)<N", _state);
    ae_assert(bndu->rows>=n, "MCPDSetBC: Rows(BndU)<N", _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            ae_assert(ae_isfinite(bndl->ptr.pp_double[i][j], _state)||ae_isneginf(bndl->ptr.pp_double[i][j], _state), "MCPDSetBC: BndL containts NAN or +INF", _state);
            ae_assert(ae_isfinite(bndu->ptr.pp_double[i][j], _state)||ae_isposinf(bndu->ptr.pp_double[i][j], _state), "MCPDSetBC: BndU containts NAN or -INF", _state);
            s->bndl.ptr.pp_double[i][j] = bndl->ptr.pp_double[i][j];
            s->bndu.ptr.pp_double[i][j] = bndu->ptr.pp_double[i][j];
        }
    }
}


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
     ae_state *_state)
{


    ae_assert(i>=0, "MCPDAddBC: I<0", _state);
    ae_assert(i<s->n, "MCPDAddBC: I>=N", _state);
    ae_assert(j>=0, "MCPDAddBC: J<0", _state);
    ae_assert(j<s->n, "MCPDAddBC: J>=N", _state);
    ae_assert(ae_isfinite(bndl, _state)||ae_isneginf(bndl, _state), "MCPDAddBC: BndL is NAN or +INF", _state);
    ae_assert(ae_isfinite(bndu, _state)||ae_isposinf(bndu, _state), "MCPDAddBC: BndU is NAN or -INF", _state);
    s->bndl.ptr.pp_double[i][j] = bndl;
    s->bndu.ptr.pp_double[i][j] = bndu;
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;


    n = s->n;
    ae_assert(c->cols>=n*n+1, "MCPDSetLC: Cols(C)<N*N+1", _state);
    ae_assert(c->rows>=k, "MCPDSetLC: Rows(C)<K", _state);
    ae_assert(ct->cnt>=k, "MCPDSetLC: Len(CT)<K", _state);
    ae_assert(apservisfinitematrix(c, k, n*n+1, _state), "MCPDSetLC: C contains infinite or NaN values!", _state);
    rmatrixsetlengthatleast(&s->c, k, n*n+1, _state);
    ivectorsetlengthatleast(&s->ct, k, _state);
    for(i=0; i<=k-1; i++)
    {
        for(j=0; j<=n*n; j++)
        {
            s->c.ptr.pp_double[i][j] = c->ptr.pp_double[i][j];
        }
        s->ct.ptr.p_int[i] = ct->ptr.p_int[i];
    }
    s->ccnt = k;
}


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
void mcpdsettikhonovregularizer(mcpdstate* s, double v, ae_state *_state)
{


    ae_assert(ae_isfinite(v, _state), "MCPDSetTikhonovRegularizer: V is infinite or NAN", _state);
    ae_assert(ae_fp_greater_eq(v,0.0), "MCPDSetTikhonovRegularizer: V is less than zero", _state);
    s->regterm = v;
}


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
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _pp;
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_pp, pp, _state);
    pp = &_pp;

    n = s->n;
    ae_assert(pp->cols>=n, "MCPDSetPrior: Cols(PP)<N", _state);
    ae_assert(pp->rows>=n, "MCPDSetPrior: Rows(PP)<K", _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            ae_assert(ae_isfinite(pp->ptr.pp_double[i][j], _state), "MCPDSetPrior: PP containts infinite elements", _state);
            ae_assert(ae_fp_greater_eq(pp->ptr.pp_double[i][j],0.0)&&ae_fp_less_eq(pp->ptr.pp_double[i][j],1.0), "MCPDSetPrior: PP[i,j] is less than 0.0 or greater than 1.0", _state);
            s->priorp.ptr.pp_double[i][j] = pp->ptr.pp_double[i][j];
        }
    }
    ae_frame_leave(_state);
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t n;


    n = s->n;
    ae_assert(pw->cnt>=n, "MCPDSetPredictionWeights: Length(PW)<N", _state);
    for(i=0; i<=n-1; i++)
    {
        ae_assert(ae_isfinite(pw->ptr.p_double[i], _state), "MCPDSetPredictionWeights: PW containts infinite or NAN elements", _state);
        ae_assert(ae_fp_greater_eq(pw->ptr.p_double[i],(double)(0)), "MCPDSetPredictionWeights: PW containts negative elements", _state);
        s->pw.ptr.p_double[i] = pw->ptr.p_double[i];
    }
}


/*************************************************************************
This function is used to start solution of the MCPD problem.

After return from this function, you can use MCPDResults() to get solution
and completion code.

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
void mcpdsolve(mcpdstate* s, ae_state *_state)
{
    ae_int_t n;
    ae_int_t npairs;
    ae_int_t ccnt;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t k2;
    double v;
    double vv;


    n = s->n;
    npairs = s->npairs;
    
    /*
     * init fields of S
     */
    s->repterminationtype = 0;
    s->repinneriterationscount = 0;
    s->repouteriterationscount = 0;
    s->repnfev = 0;
    for(k=0; k<=n-1; k++)
    {
        for(k2=0; k2<=n-1; k2++)
        {
            s->p.ptr.pp_double[k][k2] = _state->v_nan;
        }
    }
    
    /*
     * Generate "effective" weights for prediction and calculate preconditioner
     */
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_eq(s->pw.ptr.p_double[i],(double)(0)) )
        {
            v = (double)(0);
            k = 0;
            for(j=0; j<=npairs-1; j++)
            {
                if( ae_fp_neq(s->data.ptr.pp_double[j][n+i],(double)(0)) )
                {
                    v = v+s->data.ptr.pp_double[j][n+i];
                    k = k+1;
                }
            }
            if( k!=0 )
            {
                s->effectivew.ptr.p_double[i] = k/v;
            }
            else
            {
                s->effectivew.ptr.p_double[i] = 1.0;
            }
        }
        else
        {
            s->effectivew.ptr.p_double[i] = s->pw.ptr.p_double[i];
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            s->h.ptr.p_double[i*n+j] = 2*s->regterm;
        }
    }
    for(k=0; k<=npairs-1; k++)
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                s->h.ptr.p_double[i*n+j] = s->h.ptr.p_double[i*n+j]+2*ae_sqr(s->effectivew.ptr.p_double[i], _state)*ae_sqr(s->data.ptr.pp_double[k][j], _state);
            }
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( ae_fp_eq(s->h.ptr.p_double[i*n+j],(double)(0)) )
            {
                s->h.ptr.p_double[i*n+j] = (double)(1);
            }
        }
    }
    
    /*
     * Generate "effective" BndL/BndU
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            
            /*
             * Set default boundary constraints.
             * Lower bound is always zero, upper bound is calculated
             * with respect to entry/exit states.
             */
            s->effectivebndl.ptr.p_double[i*n+j] = 0.0;
            if( s->states.ptr.p_int[i]>0||s->states.ptr.p_int[j]<0 )
            {
                s->effectivebndu.ptr.p_double[i*n+j] = 0.0;
            }
            else
            {
                s->effectivebndu.ptr.p_double[i*n+j] = 1.0;
            }
            
            /*
             * Calculate intersection of the default and user-specified bound constraints.
             * This code checks consistency of such combination.
             */
            if( ae_isfinite(s->bndl.ptr.pp_double[i][j], _state)&&ae_fp_greater(s->bndl.ptr.pp_double[i][j],s->effectivebndl.ptr.p_double[i*n+j]) )
            {
                s->effectivebndl.ptr.p_double[i*n+j] = s->bndl.ptr.pp_double[i][j];
            }
            if( ae_isfinite(s->bndu.ptr.pp_double[i][j], _state)&&ae_fp_less(s->bndu.ptr.pp_double[i][j],s->effectivebndu.ptr.p_double[i*n+j]) )
            {
                s->effectivebndu.ptr.p_double[i*n+j] = s->bndu.ptr.pp_double[i][j];
            }
            if( ae_fp_greater(s->effectivebndl.ptr.p_double[i*n+j],s->effectivebndu.ptr.p_double[i*n+j]) )
            {
                s->repterminationtype = -3;
                return;
            }
            
            /*
             * Calculate intersection of the effective bound constraints
             * and user-specified equality constraints.
             * This code checks consistency of such combination.
             */
            if( ae_isfinite(s->ec.ptr.pp_double[i][j], _state) )
            {
                if( ae_fp_less(s->ec.ptr.pp_double[i][j],s->effectivebndl.ptr.p_double[i*n+j])||ae_fp_greater(s->ec.ptr.pp_double[i][j],s->effectivebndu.ptr.p_double[i*n+j]) )
                {
                    s->repterminationtype = -3;
                    return;
                }
                s->effectivebndl.ptr.p_double[i*n+j] = s->ec.ptr.pp_double[i][j];
                s->effectivebndu.ptr.p_double[i*n+j] = s->ec.ptr.pp_double[i][j];
            }
        }
    }
    
    /*
     * Generate linear constraints:
     * * "default" sums-to-one constraints (not generated for "exit" states)
     */
    rmatrixsetlengthatleast(&s->effectivec, s->ccnt+n, n*n+1, _state);
    ivectorsetlengthatleast(&s->effectivect, s->ccnt+n, _state);
    ccnt = s->ccnt;
    for(i=0; i<=s->ccnt-1; i++)
    {
        for(j=0; j<=n*n; j++)
        {
            s->effectivec.ptr.pp_double[i][j] = s->c.ptr.pp_double[i][j];
        }
        s->effectivect.ptr.p_int[i] = s->ct.ptr.p_int[i];
    }
    for(i=0; i<=n-1; i++)
    {
        if( s->states.ptr.p_int[i]>=0 )
        {
            for(k=0; k<=n*n-1; k++)
            {
                s->effectivec.ptr.pp_double[ccnt][k] = (double)(0);
            }
            for(k=0; k<=n-1; k++)
            {
                s->effectivec.ptr.pp_double[ccnt][k*n+i] = (double)(1);
            }
            s->effectivec.ptr.pp_double[ccnt][n*n] = 1.0;
            s->effectivect.ptr.p_int[ccnt] = 0;
            ccnt = ccnt+1;
        }
    }
    
    /*
     * create optimizer
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            s->tmpp.ptr.p_double[i*n+j] = (double)1/(double)n;
        }
    }
    minbleicrestartfrom(&s->bs, &s->tmpp, _state);
    minbleicsetbc(&s->bs, &s->effectivebndl, &s->effectivebndu, _state);
    minbleicsetlc(&s->bs, &s->effectivec, &s->effectivect, ccnt, _state);
    minbleicsetcond(&s->bs, 0.0, 0.0, mcpd_xtol, 0, _state);
    minbleicsetprecdiag(&s->bs, &s->h, _state);
    
    /*
     * solve problem
     */
    while(minbleiciteration(&s->bs, _state))
    {
        ae_assert(s->bs.needfg, "MCPDSolve: internal error", _state);
        if( s->bs.needfg )
        {
            
            /*
             * Calculate regularization term
             */
            s->bs.f = 0.0;
            vv = s->regterm;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    s->bs.f = s->bs.f+vv*ae_sqr(s->bs.x.ptr.p_double[i*n+j]-s->priorp.ptr.pp_double[i][j], _state);
                    s->bs.g.ptr.p_double[i*n+j] = 2*vv*(s->bs.x.ptr.p_double[i*n+j]-s->priorp.ptr.pp_double[i][j]);
                }
            }
            
            /*
             * calculate prediction error/gradient for K-th pair
             */
            for(k=0; k<=npairs-1; k++)
            {
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&s->bs.x.ptr.p_double[i*n], 1, &s->data.ptr.pp_double[k][0], 1, ae_v_len(i*n,i*n+n-1));
                    vv = s->effectivew.ptr.p_double[i];
                    s->bs.f = s->bs.f+ae_sqr(vv*(v-s->data.ptr.pp_double[k][n+i]), _state);
                    for(j=0; j<=n-1; j++)
                    {
                        s->bs.g.ptr.p_double[i*n+j] = s->bs.g.ptr.p_double[i*n+j]+2*vv*vv*(v-s->data.ptr.pp_double[k][n+i])*s->data.ptr.pp_double[k][j];
                    }
                }
            }
            
            /*
             * continue
             */
            continue;
        }
    }
    minbleicresultsbuf(&s->bs, &s->tmpp, &s->br, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            s->p.ptr.pp_double[i][j] = s->tmpp.ptr.p_double[i*n+j];
        }
    }
    s->repterminationtype = s->br.terminationtype;
    s->repinneriterationscount = s->br.inneriterationscount;
    s->repouteriterationscount = s->br.outeriterationscount;
    s->repnfev = s->br.nfev;
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(p);
    _mcpdreport_clear(rep);

    ae_matrix_set_length(p, s->n, s->n, _state);
    for(i=0; i<=s->n-1; i++)
    {
        for(j=0; j<=s->n-1; j++)
        {
            p->ptr.pp_double[i][j] = s->p.ptr.pp_double[i][j];
        }
    }
    rep->terminationtype = s->repterminationtype;
    rep->inneriterationscount = s->repinneriterationscount;
    rep->outeriterationscount = s->repouteriterationscount;
    rep->nfev = s->repnfev;
}


/*************************************************************************
Internal initialization function

  -- ALGLIB --
     Copyright 23.05.2010 by Bochkanov Sergey
*************************************************************************/
static void mcpd_mcpdinit(ae_int_t n,
     ae_int_t entrystate,
     ae_int_t exitstate,
     mcpdstate* s,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    ae_assert(n>=1, "MCPDCreate: N<1", _state);
    s->n = n;
    ae_vector_set_length(&s->states, n, _state);
    for(i=0; i<=n-1; i++)
    {
        s->states.ptr.p_int[i] = 0;
    }
    if( entrystate>=0 )
    {
        s->states.ptr.p_int[entrystate] = 1;
    }
    if( exitstate>=0 )
    {
        s->states.ptr.p_int[exitstate] = -1;
    }
    s->npairs = 0;
    s->regterm = 1.0E-8;
    s->ccnt = 0;
    ae_matrix_set_length(&s->p, n, n, _state);
    ae_matrix_set_length(&s->ec, n, n, _state);
    ae_matrix_set_length(&s->bndl, n, n, _state);
    ae_matrix_set_length(&s->bndu, n, n, _state);
    ae_vector_set_length(&s->pw, n, _state);
    ae_matrix_set_length(&s->priorp, n, n, _state);
    ae_vector_set_length(&s->tmpp, n*n, _state);
    ae_vector_set_length(&s->effectivew, n, _state);
    ae_vector_set_length(&s->effectivebndl, n*n, _state);
    ae_vector_set_length(&s->effectivebndu, n*n, _state);
    ae_vector_set_length(&s->h, n*n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            s->p.ptr.pp_double[i][j] = 0.0;
            s->priorp.ptr.pp_double[i][j] = 0.0;
            s->bndl.ptr.pp_double[i][j] = _state->v_neginf;
            s->bndu.ptr.pp_double[i][j] = _state->v_posinf;
            s->ec.ptr.pp_double[i][j] = _state->v_nan;
        }
        s->pw.ptr.p_double[i] = 0.0;
        s->priorp.ptr.pp_double[i][i] = 1.0;
    }
    ae_matrix_set_length(&s->data, 1, 2*n, _state);
    for(i=0; i<=2*n-1; i++)
    {
        s->data.ptr.pp_double[0][i] = 0.0;
    }
    for(i=0; i<=n*n-1; i++)
    {
        s->tmpp.ptr.p_double[i] = 0.0;
    }
    minbleiccreate(n*n, &s->tmpp, &s->bs, _state);
}


void _mcpdstate_init(void* _p, ae_state *_state)
{
    mcpdstate *p = (mcpdstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->states, 0, DT_INT, _state);
    ae_matrix_init(&p->data, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->ec, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->bndl, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->bndu, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->c, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->ct, 0, DT_INT, _state);
    ae_vector_init(&p->pw, 0, DT_REAL, _state);
    ae_matrix_init(&p->priorp, 0, 0, DT_REAL, _state);
    _minbleicstate_init(&p->bs, _state);
    _minbleicreport_init(&p->br, _state);
    ae_vector_init(&p->tmpp, 0, DT_REAL, _state);
    ae_vector_init(&p->effectivew, 0, DT_REAL, _state);
    ae_vector_init(&p->effectivebndl, 0, DT_REAL, _state);
    ae_vector_init(&p->effectivebndu, 0, DT_REAL, _state);
    ae_matrix_init(&p->effectivec, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->effectivect, 0, DT_INT, _state);
    ae_vector_init(&p->h, 0, DT_REAL, _state);
    ae_matrix_init(&p->p, 0, 0, DT_REAL, _state);
}


void _mcpdstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    mcpdstate *dst = (mcpdstate*)_dst;
    mcpdstate *src = (mcpdstate*)_src;
    dst->n = src->n;
    ae_vector_init_copy(&dst->states, &src->states, _state);
    dst->npairs = src->npairs;
    ae_matrix_init_copy(&dst->data, &src->data, _state);
    ae_matrix_init_copy(&dst->ec, &src->ec, _state);
    ae_matrix_init_copy(&dst->bndl, &src->bndl, _state);
    ae_matrix_init_copy(&dst->bndu, &src->bndu, _state);
    ae_matrix_init_copy(&dst->c, &src->c, _state);
    ae_vector_init_copy(&dst->ct, &src->ct, _state);
    dst->ccnt = src->ccnt;
    ae_vector_init_copy(&dst->pw, &src->pw, _state);
    ae_matrix_init_copy(&dst->priorp, &src->priorp, _state);
    dst->regterm = src->regterm;
    _minbleicstate_init_copy(&dst->bs, &src->bs, _state);
    dst->repinneriterationscount = src->repinneriterationscount;
    dst->repouteriterationscount = src->repouteriterationscount;
    dst->repnfev = src->repnfev;
    dst->repterminationtype = src->repterminationtype;
    _minbleicreport_init_copy(&dst->br, &src->br, _state);
    ae_vector_init_copy(&dst->tmpp, &src->tmpp, _state);
    ae_vector_init_copy(&dst->effectivew, &src->effectivew, _state);
    ae_vector_init_copy(&dst->effectivebndl, &src->effectivebndl, _state);
    ae_vector_init_copy(&dst->effectivebndu, &src->effectivebndu, _state);
    ae_matrix_init_copy(&dst->effectivec, &src->effectivec, _state);
    ae_vector_init_copy(&dst->effectivect, &src->effectivect, _state);
    ae_vector_init_copy(&dst->h, &src->h, _state);
    ae_matrix_init_copy(&dst->p, &src->p, _state);
}


void _mcpdstate_clear(void* _p)
{
    mcpdstate *p = (mcpdstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->states);
    ae_matrix_clear(&p->data);
    ae_matrix_clear(&p->ec);
    ae_matrix_clear(&p->bndl);
    ae_matrix_clear(&p->bndu);
    ae_matrix_clear(&p->c);
    ae_vector_clear(&p->ct);
    ae_vector_clear(&p->pw);
    ae_matrix_clear(&p->priorp);
    _minbleicstate_clear(&p->bs);
    _minbleicreport_clear(&p->br);
    ae_vector_clear(&p->tmpp);
    ae_vector_clear(&p->effectivew);
    ae_vector_clear(&p->effectivebndl);
    ae_vector_clear(&p->effectivebndu);
    ae_matrix_clear(&p->effectivec);
    ae_vector_clear(&p->effectivect);
    ae_vector_clear(&p->h);
    ae_matrix_clear(&p->p);
}


void _mcpdstate_destroy(void* _p)
{
    mcpdstate *p = (mcpdstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->states);
    ae_matrix_destroy(&p->data);
    ae_matrix_destroy(&p->ec);
    ae_matrix_destroy(&p->bndl);
    ae_matrix_destroy(&p->bndu);
    ae_matrix_destroy(&p->c);
    ae_vector_destroy(&p->ct);
    ae_vector_destroy(&p->pw);
    ae_matrix_destroy(&p->priorp);
    _minbleicstate_destroy(&p->bs);
    _minbleicreport_destroy(&p->br);
    ae_vector_destroy(&p->tmpp);
    ae_vector_destroy(&p->effectivew);
    ae_vector_destroy(&p->effectivebndl);
    ae_vector_destroy(&p->effectivebndu);
    ae_matrix_destroy(&p->effectivec);
    ae_vector_destroy(&p->effectivect);
    ae_vector_destroy(&p->h);
    ae_matrix_destroy(&p->p);
}


void _mcpdreport_init(void* _p, ae_state *_state)
{
    mcpdreport *p = (mcpdreport*)_p;
    ae_touch_ptr((void*)p);
}


void _mcpdreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    mcpdreport *dst = (mcpdreport*)_dst;
    mcpdreport *src = (mcpdreport*)_src;
    dst->inneriterationscount = src->inneriterationscount;
    dst->outeriterationscount = src->outeriterationscount;
    dst->nfev = src->nfev;
    dst->terminationtype = src->terminationtype;
}


void _mcpdreport_clear(void* _p)
{
    mcpdreport *p = (mcpdreport*)_p;
    ae_touch_ptr((void*)p);
}


void _mcpdreport_destroy(void* _p)
{
    mcpdreport *p = (mcpdreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
