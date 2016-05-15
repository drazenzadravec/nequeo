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
#include "sactivesets.h"


/*$ Declarations $*/
static void sactivesets_constraineddescent(sactiveset* state,
     /* Real    */ ae_vector* g,
     /* Real    */ ae_vector* h,
     /* Real    */ ae_matrix* ha,
     ae_bool normalize,
     /* Real    */ ae_vector* d,
     ae_state *_state);
static void sactivesets_reactivateconstraints(sactiveset* state,
     /* Real    */ ae_vector* gc,
     /* Real    */ ae_vector* h,
     ae_state *_state);


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
void sasinit(ae_int_t n, sactiveset* s, ae_state *_state)
{
    ae_int_t i;


    s->n = n;
    s->algostate = 0;
    
    /*
     * Constraints
     */
    s->constraintschanged = ae_true;
    s->nec = 0;
    s->nic = 0;
    rvectorsetlengthatleast(&s->bndl, n, _state);
    bvectorsetlengthatleast(&s->hasbndl, n, _state);
    rvectorsetlengthatleast(&s->bndu, n, _state);
    bvectorsetlengthatleast(&s->hasbndu, n, _state);
    for(i=0; i<=n-1; i++)
    {
        s->bndl.ptr.p_double[i] = _state->v_neginf;
        s->bndu.ptr.p_double[i] = _state->v_posinf;
        s->hasbndl.ptr.p_bool[i] = ae_false;
        s->hasbndu.ptr.p_bool[i] = ae_false;
    }
    
    /*
     * current point, scale
     */
    s->hasxc = ae_false;
    rvectorsetlengthatleast(&s->xc, n, _state);
    rvectorsetlengthatleast(&s->s, n, _state);
    rvectorsetlengthatleast(&s->h, n, _state);
    for(i=0; i<=n-1; i++)
    {
        s->xc.ptr.p_double[i] = 0.0;
        s->s.ptr.p_double[i] = 1.0;
        s->h.ptr.p_double[i] = 1.0;
    }
    
    /*
     * Other
     */
    rvectorsetlengthatleast(&s->unitdiagonal, n, _state);
    for(i=0; i<=n-1; i++)
    {
        s->unitdiagonal.ptr.p_double[i] = 1.0;
    }
}


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
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(state->algostate==0, "SASSetScale: you may change scale only in modification mode", _state);
    ae_assert(s->cnt>=state->n, "SASSetScale: Length(S)<N", _state);
    for(i=0; i<=state->n-1; i++)
    {
        ae_assert(ae_isfinite(s->ptr.p_double[i], _state), "SASSetScale: S contains infinite or NAN elements", _state);
        ae_assert(ae_fp_neq(s->ptr.p_double[i],(double)(0)), "SASSetScale: S contains zero elements", _state);
    }
    for(i=0; i<=state->n-1; i++)
    {
        state->s.ptr.p_double[i] = ae_fabs(s->ptr.p_double[i], _state);
    }
}


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
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(state->algostate==0, "SASSetPrecDiag: you may change preconditioner only in modification mode", _state);
    ae_assert(d->cnt>=state->n, "SASSetPrecDiag: D is too short", _state);
    for(i=0; i<=state->n-1; i++)
    {
        ae_assert(ae_isfinite(d->ptr.p_double[i], _state), "SASSetPrecDiag: D contains infinite or NAN elements", _state);
        ae_assert(ae_fp_greater(d->ptr.p_double[i],(double)(0)), "SASSetPrecDiag: D contains non-positive elements", _state);
    }
    for(i=0; i<=state->n-1; i++)
    {
        state->h.ptr.p_double[i] = d->ptr.p_double[i];
    }
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t n;


    ae_assert(state->algostate==0, "SASSetBC: you may change constraints only in modification mode", _state);
    n = state->n;
    ae_assert(bndl->cnt>=n, "SASSetBC: Length(BndL)<N", _state);
    ae_assert(bndu->cnt>=n, "SASSetBC: Length(BndU)<N", _state);
    for(i=0; i<=n-1; i++)
    {
        ae_assert(ae_isfinite(bndl->ptr.p_double[i], _state)||ae_isneginf(bndl->ptr.p_double[i], _state), "SASSetBC: BndL contains NAN or +INF", _state);
        ae_assert(ae_isfinite(bndu->ptr.p_double[i], _state)||ae_isposinf(bndu->ptr.p_double[i], _state), "SASSetBC: BndL contains NAN or -INF", _state);
        state->bndl.ptr.p_double[i] = bndl->ptr.p_double[i];
        state->hasbndl.ptr.p_bool[i] = ae_isfinite(bndl->ptr.p_double[i], _state);
        state->bndu.ptr.p_double[i] = bndu->ptr.p_double[i];
        state->hasbndu.ptr.p_bool[i] = ae_isfinite(bndu->ptr.p_double[i], _state);
    }
    state->constraintschanged = ae_true;
}


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
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;


    ae_assert(state->algostate==0, "SASSetLC: you may change constraints only in modification mode", _state);
    n = state->n;
    
    /*
     * First, check for errors in the inputs
     */
    ae_assert(k>=0, "SASSetLC: K<0", _state);
    ae_assert(c->cols>=n+1||k==0, "SASSetLC: Cols(C)<N+1", _state);
    ae_assert(c->rows>=k, "SASSetLC: Rows(C)<K", _state);
    ae_assert(ct->cnt>=k, "SASSetLC: Length(CT)<K", _state);
    ae_assert(apservisfinitematrix(c, k, n+1, _state), "SASSetLC: C contains infinite or NaN values!", _state);
    
    /*
     * Handle zero K
     */
    if( k==0 )
    {
        state->nec = 0;
        state->nic = 0;
        state->constraintschanged = ae_true;
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
     * Mark state as changed
     */
    state->constraintschanged = ae_true;
}


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
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;


    ae_assert(state->algostate==0, "SASSetLCX: you may change constraints only in modification mode", _state);
    n = state->n;
    
    /*
     * First, check for errors in the inputs
     */
    ae_assert(nec>=0, "SASSetLCX: NEC<0", _state);
    ae_assert(nic>=0, "SASSetLCX: NIC<0", _state);
    ae_assert(cleic->cols>=n+1||nec+nic==0, "SASSetLCX: Cols(CLEIC)<N+1", _state);
    ae_assert(cleic->rows>=nec+nic, "SASSetLCX: Rows(CLEIC)<NEC+NIC", _state);
    ae_assert(apservisfinitematrix(cleic, nec+nic, n+1, _state), "SASSetLCX: CLEIC contains infinite or NaN values!", _state);
    
    /*
     * Store constraints
     */
    rmatrixsetlengthatleast(&state->cleic, nec+nic, n+1, _state);
    state->nec = nec;
    state->nic = nic;
    for(i=0; i<=nec+nic-1; i++)
    {
        for(j=0; j<=n; j++)
        {
            state->cleic.ptr.pp_double[i][j] = cleic->ptr.pp_double[i][j];
        }
    }
    
    /*
     * Mark state as changed
     */
    state->constraintschanged = ae_true;
}


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
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_bool result;


    ae_assert(state->algostate==0, "SASStartOptimization: already in optimization mode", _state);
    result = ae_false;
    n = state->n;
    nec = state->nec;
    nic = state->nic;
    
    /*
     * Enforce feasibility and calculate set of "candidate"/"active" constraints.
     * Always active equality constraints are marked as "active", all other constraints
     * are marked as "candidate".
     */
    ivectorsetlengthatleast(&state->activeset, n+nec+nic, _state);
    for(i=0; i<=n-1; i++)
    {
        if( state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i] )
        {
            if( ae_fp_greater(state->bndl.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
            {
                return result;
            }
        }
    }
    ae_v_move(&state->xc.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
    if( state->nec+state->nic>0 )
    {
        
        /*
         * General linear constraints are present; general code is used.
         */
        rvectorsetlengthatleast(&state->tmp0, n, _state);
        rvectorsetlengthatleast(&state->tmpfeas, n+state->nic, _state);
        rmatrixsetlengthatleast(&state->tmpm0, state->nec+state->nic, n+state->nic+1, _state);
        for(i=0; i<=state->nec+state->nic-1; i++)
        {
            ae_v_move(&state->tmpm0.ptr.pp_double[i][0], 1, &state->cleic.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
            for(j=n; j<=n+state->nic-1; j++)
            {
                state->tmpm0.ptr.pp_double[i][j] = (double)(0);
            }
            if( i>=state->nec )
            {
                state->tmpm0.ptr.pp_double[i][n+i-state->nec] = 1.0;
            }
            state->tmpm0.ptr.pp_double[i][n+state->nic] = state->cleic.ptr.pp_double[i][n];
        }
        ae_v_move(&state->tmpfeas.ptr.p_double[0], 1, &state->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
        for(i=0; i<=state->nic-1; i++)
        {
            v = ae_v_dotproduct(&state->cleic.ptr.pp_double[i+state->nec][0], 1, &state->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
            state->tmpfeas.ptr.p_double[i+n] = ae_maxreal(state->cleic.ptr.pp_double[i+state->nec][n]-v, 0.0, _state);
        }
        if( !findfeasiblepoint(&state->tmpfeas, &state->bndl, &state->hasbndl, &state->bndu, &state->hasbndu, n, state->nic, &state->tmpm0, state->nec+state->nic, 1.0E-6, &i, &j, _state) )
        {
            return result;
        }
        ae_v_move(&state->xc.ptr.p_double[0], 1, &state->tmpfeas.ptr.p_double[0], 1, ae_v_len(0,n-1));
        for(i=0; i<=n-1; i++)
        {
            if( (state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i])&&ae_fp_eq(state->bndl.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
            {
                state->activeset.ptr.p_int[i] = 1;
                continue;
            }
            if( (state->hasbndl.ptr.p_bool[i]&&ae_fp_eq(state->xc.ptr.p_double[i],state->bndl.ptr.p_double[i]))||(state->hasbndu.ptr.p_bool[i]&&ae_fp_eq(state->xc.ptr.p_double[i],state->bndu.ptr.p_double[i])) )
            {
                state->activeset.ptr.p_int[i] = 0;
                continue;
            }
            state->activeset.ptr.p_int[i] = -1;
        }
        for(i=0; i<=state->nec-1; i++)
        {
            state->activeset.ptr.p_int[n+i] = 1;
        }
        for(i=0; i<=state->nic-1; i++)
        {
            if( ae_fp_eq(state->tmpfeas.ptr.p_double[n+i],(double)(0)) )
            {
                state->activeset.ptr.p_int[n+state->nec+i] = 0;
            }
            else
            {
                state->activeset.ptr.p_int[n+state->nec+i] = -1;
            }
        }
    }
    else
    {
        
        /*
         * Only bound constraints are present, quick code can be used
         */
        for(i=0; i<=n-1; i++)
        {
            state->activeset.ptr.p_int[i] = -1;
            if( (state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i])&&ae_fp_eq(state->bndl.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
            {
                state->activeset.ptr.p_int[i] = 1;
                state->xc.ptr.p_double[i] = state->bndl.ptr.p_double[i];
                continue;
            }
            if( state->hasbndl.ptr.p_bool[i]&&ae_fp_less_eq(state->xc.ptr.p_double[i],state->bndl.ptr.p_double[i]) )
            {
                state->xc.ptr.p_double[i] = state->bndl.ptr.p_double[i];
                state->activeset.ptr.p_int[i] = 0;
                continue;
            }
            if( state->hasbndu.ptr.p_bool[i]&&ae_fp_greater_eq(state->xc.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
            {
                state->xc.ptr.p_double[i] = state->bndu.ptr.p_double[i];
                state->activeset.ptr.p_int[i] = 0;
                continue;
            }
        }
    }
    
    /*
     * Change state, allocate temporaries
     */
    result = ae_true;
    state->algostate = 1;
    state->basisisready = ae_false;
    state->hasxc = ae_true;
    rmatrixsetlengthatleast(&state->pbasis, ae_minint(nec+nic, n, _state), n+1, _state);
    rmatrixsetlengthatleast(&state->ibasis, ae_minint(nec+nic, n, _state), n+1, _state);
    rmatrixsetlengthatleast(&state->sbasis, ae_minint(nec+nic, n, _state), n+1, _state);
    return result;
}


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
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t i;
    double prevmax;
    double vc;
    double vd;

    *stpmax = 0;
    *cidx = 0;
    *vval = 0;

    ae_assert(state->algostate==1, "SASExploreDirection: is not in optimization mode", _state);
    n = state->n;
    nec = state->nec;
    nic = state->nic;
    *cidx = -1;
    *vval = (double)(0);
    *stpmax = 1.0E50;
    for(i=0; i<=n-1; i++)
    {
        if( state->activeset.ptr.p_int[i]<=0 )
        {
            ae_assert(!state->hasbndl.ptr.p_bool[i]||ae_fp_greater_eq(state->xc.ptr.p_double[i],state->bndl.ptr.p_double[i]), "SASExploreDirection: internal error - infeasible X", _state);
            ae_assert(!state->hasbndu.ptr.p_bool[i]||ae_fp_less_eq(state->xc.ptr.p_double[i],state->bndu.ptr.p_double[i]), "SASExploreDirection: internal error - infeasible X", _state);
            if( state->hasbndl.ptr.p_bool[i]&&ae_fp_less(d->ptr.p_double[i],(double)(0)) )
            {
                prevmax = *stpmax;
                *stpmax = safeminposrv(state->xc.ptr.p_double[i]-state->bndl.ptr.p_double[i], -d->ptr.p_double[i], *stpmax, _state);
                if( ae_fp_less(*stpmax,prevmax) )
                {
                    *cidx = i;
                    *vval = state->bndl.ptr.p_double[i];
                }
            }
            if( state->hasbndu.ptr.p_bool[i]&&ae_fp_greater(d->ptr.p_double[i],(double)(0)) )
            {
                prevmax = *stpmax;
                *stpmax = safeminposrv(state->bndu.ptr.p_double[i]-state->xc.ptr.p_double[i], d->ptr.p_double[i], *stpmax, _state);
                if( ae_fp_less(*stpmax,prevmax) )
                {
                    *cidx = i;
                    *vval = state->bndu.ptr.p_double[i];
                }
            }
        }
    }
    for(i=nec; i<=nec+nic-1; i++)
    {
        if( state->activeset.ptr.p_int[n+i]<=0 )
        {
            vc = ae_v_dotproduct(&state->cleic.ptr.pp_double[i][0], 1, &state->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
            vc = vc-state->cleic.ptr.pp_double[i][n];
            vd = ae_v_dotproduct(&state->cleic.ptr.pp_double[i][0], 1, &d->ptr.p_double[0], 1, ae_v_len(0,n-1));
            if( ae_fp_less_eq(vd,(double)(0)) )
            {
                continue;
            }
            if( ae_fp_less(vc,(double)(0)) )
            {
                
                /*
                 * XC is strictly feasible with respect to I-th constraint,
                 * we can perform non-zero step because there is non-zero distance
                 * between XC and bound.
                 */
                prevmax = *stpmax;
                *stpmax = safeminposrv(-vc, vd, *stpmax, _state);
                if( ae_fp_less(*stpmax,prevmax) )
                {
                    *cidx = n+i;
                }
            }
            else
            {
                
                /*
                 * XC is at the boundary (or slightly beyond it), and step vector
                 * points beyond the boundary.
                 *
                 * The only thing we can do is to perform zero step and activate
                 * I-th constraint.
                 */
                *stpmax = (double)(0);
                *cidx = n+i;
            }
        }
    }
}


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
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t i;
    ae_bool wasactivation;
    ae_int_t result;


    ae_assert(state->algostate==1, "SASMoveTo: is not in optimization mode", _state);
    n = state->n;
    nec = state->nec;
    nic = state->nic;
    
    /*
     * Save previous state, update current point
     */
    rvectorsetlengthatleast(&state->mtx, n, _state);
    ivectorsetlengthatleast(&state->mtas, n+nec+nic, _state);
    for(i=0; i<=n-1; i++)
    {
        state->mtx.ptr.p_double[i] = state->xc.ptr.p_double[i];
        state->xc.ptr.p_double[i] = xn->ptr.p_double[i];
    }
    for(i=0; i<=n+nec+nic-1; i++)
    {
        state->mtas.ptr.p_int[i] = state->activeset.ptr.p_int[i];
    }
    
    /*
     * Activate constraints
     */
    wasactivation = ae_false;
    if( needact )
    {
        
        /*
         * Activation
         */
        ae_assert(cidx>=0&&cidx<n+nec+nic, "SASMoveTo: incorrect CIdx", _state);
        if( cidx<n )
        {
            
            /*
             * CIdx in [0,N-1] means that bound constraint was activated.
             * We activate it explicitly to avoid situation when roundoff-error
             * prevents us from moving EXACTLY to x=CVal.
             */
            state->xc.ptr.p_double[cidx] = cval;
        }
        state->activeset.ptr.p_int[cidx] = 1;
        wasactivation = ae_true;
    }
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * Post-check (some constraints may be activated because of numerical errors)
         */
        if( (state->hasbndl.ptr.p_bool[i]&&ae_fp_less_eq(state->xc.ptr.p_double[i],state->bndl.ptr.p_double[i]))&&ae_fp_neq(state->xc.ptr.p_double[i],state->mtx.ptr.p_double[i]) )
        {
            state->xc.ptr.p_double[i] = state->bndl.ptr.p_double[i];
            state->activeset.ptr.p_int[i] = 1;
            wasactivation = ae_true;
        }
        if( (state->hasbndu.ptr.p_bool[i]&&ae_fp_greater_eq(state->xc.ptr.p_double[i],state->bndu.ptr.p_double[i]))&&ae_fp_neq(state->xc.ptr.p_double[i],state->mtx.ptr.p_double[i]) )
        {
            state->xc.ptr.p_double[i] = state->bndu.ptr.p_double[i];
            state->activeset.ptr.p_int[i] = 1;
            wasactivation = ae_true;
        }
    }
    
    /*
     * Determine return status:
     * * -1 in case no constraints were activated
     * *  0 in case only "candidate" constraints were activated
     * * +1 in case at least one "non-candidate" constraint was activated
     */
    if( wasactivation )
    {
        
        /*
         * Step activated one/several constraints, but sometimes it is spurious
         * activation - RecalculateConstraints() tells us that constraint is
         * inactive (negative Largrange multiplier), but step activates it
         * because of numerical noise.
         *
         * This block of code checks whether step activated truly new constraints
         * (ones which were not in the active set at the solution):
         *
         * * for non-boundary constraint it is enough to check that previous value
         *   of ActiveSet[i] is negative (=far from boundary), and new one is
         *   positive (=we are at the boundary, constraint is activated).
         *
         * * for boundary constraints previous criterion won't work. Each variable
         *   has two constraints, and simply checking their status is not enough -
         *   we have to correctly identify cases when we leave one boundary
         *   (PrevActiveSet[i]=0) and move to another boundary (ActiveSet[i]>0).
         *   Such cases can be identified if we compare previous X with new X.
         *
         * In case only "candidate" constraints were activated, result variable
         * is set to 0. In case at least one new constraint was activated, result
         * is set to 1.
         */
        result = 0;
        for(i=0; i<=n-1; i++)
        {
            if( state->activeset.ptr.p_int[i]>0&&ae_fp_neq(state->xc.ptr.p_double[i],state->mtx.ptr.p_double[i]) )
            {
                result = 1;
            }
        }
        for(i=n; i<=n+state->nec+state->nic-1; i++)
        {
            if( state->mtas.ptr.p_int[i]<0&&state->activeset.ptr.p_int[i]>0 )
            {
                result = 1;
            }
        }
    }
    else
    {
        
        /*
         * No activation, return -1
         */
        result = -1;
    }
    
    /*
     * Invalidate basis
     */
    state->basisisready = ae_false;
    return result;
}


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
     ae_state *_state)
{


    ae_assert(state->algostate==1, "SASMoveTo: is not in optimization mode", _state);
    if( cidx<state->n )
    {
        state->xc.ptr.p_double[cidx] = cval;
    }
    state->activeset.ptr.p_int[cidx] = 1;
    state->basisisready = ae_false;
}


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
     ae_state *_state)
{


    ae_assert(state->algostate==1, "SASConstrainedDescent: is not in optimization mode", _state);
    sasrebuildbasis(state, _state);
    sactivesets_constraineddescent(state, g, &state->unitdiagonal, &state->ibasis, ae_true, d, _state);
}


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
     ae_state *_state)
{


    ae_assert(state->algostate==1, "SASConstrainedDescentPrec: is not in optimization mode", _state);
    sasrebuildbasis(state, _state);
    sactivesets_constraineddescent(state, g, &state->h, &state->pbasis, ae_true, d, _state);
}


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
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(state->algostate==1, "SASConstrainedAntigradientPrec: is not in optimization mode", _state);
    sasrebuildbasis(state, _state);
    sactivesets_constraineddescent(state, d, &state->unitdiagonal, &state->ibasis, ae_false, &state->cdtmp, _state);
    for(i=0; i<=state->n-1; i++)
    {
        d->ptr.p_double[i] = -state->cdtmp.ptr.p_double[i];
    }
}


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
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(state->algostate==1, "SASConstrainedAntigradientPrec: is not in optimization mode", _state);
    sasrebuildbasis(state, _state);
    sactivesets_constraineddescent(state, d, &state->h, &state->pbasis, ae_false, &state->cdtmp, _state);
    for(i=0; i<=state->n-1; i++)
    {
        d->ptr.p_double[i] = -state->cdtmp.ptr.p_double[i];
    }
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;
    double v;

    *penalty = 0;

    ae_assert(state->algostate==1, "SASCorrection: is not in optimization mode", _state);
    sasrebuildbasis(state, _state);
    n = state->n;
    rvectorsetlengthatleast(&state->corrtmp, n, _state);
    
    /*
     * Calculate penalty term.
     */
    *penalty = sasactivelcpenalty1(state, x, _state);
    
    /*
     * Perform projection 1.
     *
     * This projecton is given by:
     *
     *     x_proj = x - S*S*As'*(As*x-b)
     *
     * where x is original x before projection, S is a scale matrix,
     * As is a matrix of equality constraints (active set) which were
     * orthogonalized with respect to inner product given by S (i.e. we
     * have As*S*S'*As'=I), b is a right part of the orthogonalized
     * constraints.
     *
     * NOTE: you can verify that x_proj is strictly feasible w.r.t.
     *       active set by multiplying it by As - you will get
     *       As*x_proj = As*x - As*x + b = b.
     *
     *       This formula for projection can be obtained by solving
     *       following minimization problem.
     *
     *           min ||inv(S)*(x_proj-x)||^2 s.t. As*x_proj=b
     *       
     */
    ae_v_move(&state->corrtmp.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
    for(i=0; i<=state->basissize-1; i++)
    {
        v = -state->sbasis.ptr.pp_double[i][n];
        for(j=0; j<=n-1; j++)
        {
            v = v+state->sbasis.ptr.pp_double[i][j]*state->corrtmp.ptr.p_double[j];
        }
        for(j=0; j<=n-1; j++)
        {
            state->corrtmp.ptr.p_double[j] = state->corrtmp.ptr.p_double[j]-v*state->sbasis.ptr.pp_double[i][j]*ae_sqr(state->s.ptr.p_double[j], _state);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        if( state->activeset.ptr.p_int[i]>0 )
        {
            state->corrtmp.ptr.p_double[i] = state->xc.ptr.p_double[i];
        }
    }
    
    /*
     * Perform projection 2
     */
    for(i=0; i<=n-1; i++)
    {
        x->ptr.p_double[i] = state->corrtmp.ptr.p_double[i];
        if( state->hasbndl.ptr.p_bool[i]&&ae_fp_less(x->ptr.p_double[i],state->bndl.ptr.p_double[i]) )
        {
            x->ptr.p_double[i] = state->bndl.ptr.p_double[i];
        }
        if( state->hasbndu.ptr.p_bool[i]&&ae_fp_greater(x->ptr.p_double[i],state->bndu.ptr.p_double[i]) )
        {
            x->ptr.p_double[i] = state->bndu.ptr.p_double[i];
        }
    }
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;
    ae_int_t nec;
    ae_int_t nic;
    double v;
    double alpha;
    double p;
    double result;


    ae_assert(state->algostate==1, "SASActiveLCPenalty1: is not in optimization mode", _state);
    sasrebuildbasis(state, _state);
    n = state->n;
    nec = state->nec;
    nic = state->nic;
    
    /*
     * Calculate penalty term.
     */
    result = (double)(0);
    for(i=0; i<=nec+nic-1; i++)
    {
        if( state->activeset.ptr.p_int[n+i]>0 )
        {
            alpha = (double)(0);
            p = -state->cleic.ptr.pp_double[i][n];
            for(j=0; j<=n-1; j++)
            {
                v = state->cleic.ptr.pp_double[i][j];
                p = p+v*x->ptr.p_double[j];
                alpha = alpha+ae_sqr(v*state->s.ptr.p_double[j], _state);
            }
            alpha = ae_sqrt(alpha, _state);
            if( ae_fp_neq(alpha,(double)(0)) )
            {
                result = result+ae_fabs(p/alpha, _state);
            }
        }
    }
    return result;
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t n;
    double v;
    ae_int_t nactive;
    double result;


    ae_assert(state->algostate==1, "SASMoveTo: is not in optimization mode", _state);
    n = state->n;
    rvectorsetlengthatleast(&state->scntmp, n, _state);
    
    /*
     * Prepare basis (if needed)
     */
    sasrebuildbasis(state, _state);
    
    /*
     * Calculate descent direction
     */
    nactive = 0;
    for(i=0; i<=n-1; i++)
    {
        if( state->activeset.ptr.p_int[i]>0 )
        {
            state->scntmp.ptr.p_double[i] = (double)(0);
            nactive = nactive+1;
        }
        else
        {
            state->scntmp.ptr.p_double[i] = d->ptr.p_double[i];
        }
    }
    if( nactive+state->basissize>=n )
    {
        
        /*
         * Quick exit if number of active constraints is N or larger
         */
        result = 0.0;
        return result;
    }
    for(i=0; i<=state->basissize-1; i++)
    {
        v = ae_v_dotproduct(&state->ibasis.ptr.pp_double[i][0], 1, &state->scntmp.ptr.p_double[0], 1, ae_v_len(0,n-1));
        ae_v_subd(&state->scntmp.ptr.p_double[0], 1, &state->ibasis.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
    }
    v = 0.0;
    for(i=0; i<=n-1; i++)
    {
        v = v+ae_sqr(state->s.ptr.p_double[i]*state->scntmp.ptr.p_double[i], _state);
    }
    result = ae_sqrt(v, _state);
    return result;
}


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
void sasstopoptimization(sactiveset* state, ae_state *_state)
{


    state->algostate = 0;
}


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
     ae_state *_state)
{


    ae_assert(state->algostate==1, "SASReactivateConstraints: must be in optimization mode", _state);
    sactivesets_reactivateconstraints(state, gc, &state->unitdiagonal, _state);
}


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
     ae_state *_state)
{


    ae_assert(state->algostate==1, "SASReactivateConstraintsPrec: must be in optimization mode", _state);
    sactivesets_reactivateconstraints(state, gc, &state->h, _state);
}


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
void sasrebuildbasis(sactiveset* state, ae_state *_state)
{
    ae_int_t n;
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t i;
    ae_int_t j;
    ae_int_t t;
    ae_int_t nactivelin;
    ae_int_t nactivebnd;
    double v;
    double vmax;
    ae_int_t kmax;


    if( state->basisisready )
    {
        return;
    }
    n = state->n;
    nec = state->nec;
    nic = state->nic;
    rmatrixsetlengthatleast(&state->tmpbasis, nec+nic, n+1, _state);
    state->basissize = 0;
    state->basisisready = ae_true;
    
    /*
     * Determine number of active boundary and non-boundary
     * constraints, move them to TmpBasis. Quick exit if no
     * non-boundary constraints were detected.
     */
    nactivelin = 0;
    nactivebnd = 0;
    for(i=0; i<=nec+nic-1; i++)
    {
        if( state->activeset.ptr.p_int[n+i]>0 )
        {
            nactivelin = nactivelin+1;
        }
    }
    for(j=0; j<=n-1; j++)
    {
        if( state->activeset.ptr.p_int[j]>0 )
        {
            nactivebnd = nactivebnd+1;
        }
    }
    if( nactivelin==0 )
    {
        return;
    }
    
    /*
     * Orthogonalize linear constraints (inner product is given by preconditioner)
     * with respect to each other and boundary ones:
     * * normalize all constraints
     * * orthogonalize with respect to boundary ones
     * * repeat:
     *   * if basisSize+nactivebnd=n - TERMINATE
     *   * choose largest row from TmpBasis
     *   * if row norm is too small  - TERMINATE
     *   * add row to basis, normalize
     *   * remove from TmpBasis, orthogonalize other constraints with respect to this one
     */
    nactivelin = 0;
    for(i=0; i<=nec+nic-1; i++)
    {
        if( state->activeset.ptr.p_int[n+i]>0 )
        {
            ae_v_move(&state->tmpbasis.ptr.pp_double[nactivelin][0], 1, &state->cleic.ptr.pp_double[i][0], 1, ae_v_len(0,n));
            nactivelin = nactivelin+1;
        }
    }
    for(i=0; i<=nactivelin-1; i++)
    {
        v = 0.0;
        for(j=0; j<=n-1; j++)
        {
            v = v+ae_sqr(state->tmpbasis.ptr.pp_double[i][j], _state)/state->h.ptr.p_double[j];
        }
        if( ae_fp_greater(v,(double)(0)) )
        {
            v = 1/ae_sqrt(v, _state);
            for(j=0; j<=n; j++)
            {
                state->tmpbasis.ptr.pp_double[i][j] = state->tmpbasis.ptr.pp_double[i][j]*v;
            }
        }
    }
    for(j=0; j<=n-1; j++)
    {
        if( state->activeset.ptr.p_int[j]>0 )
        {
            for(i=0; i<=nactivelin-1; i++)
            {
                state->tmpbasis.ptr.pp_double[i][n] = state->tmpbasis.ptr.pp_double[i][n]-state->tmpbasis.ptr.pp_double[i][j]*state->xc.ptr.p_double[j];
                state->tmpbasis.ptr.pp_double[i][j] = 0.0;
            }
        }
    }
    while(state->basissize+nactivebnd<n)
    {
        
        /*
         * Find largest vector, add to basis
         */
        vmax = (double)(-1);
        kmax = -1;
        for(i=0; i<=nactivelin-1; i++)
        {
            v = 0.0;
            for(j=0; j<=n-1; j++)
            {
                v = v+ae_sqr(state->tmpbasis.ptr.pp_double[i][j], _state)/state->h.ptr.p_double[j];
            }
            v = ae_sqrt(v, _state);
            if( ae_fp_greater(v,vmax) )
            {
                vmax = v;
                kmax = i;
            }
        }
        if( ae_fp_less(vmax,1.0E4*ae_machineepsilon) )
        {
            break;
        }
        v = 1/vmax;
        ae_v_moved(&state->pbasis.ptr.pp_double[state->basissize][0], 1, &state->tmpbasis.ptr.pp_double[kmax][0], 1, ae_v_len(0,n), v);
        state->basissize = state->basissize+1;
        
        /*
         * Reorthogonalize other vectors with respect to chosen one.
         * Remove it from the array.
         */
        for(i=0; i<=nactivelin-1; i++)
        {
            if( i!=kmax )
            {
                v = (double)(0);
                for(j=0; j<=n-1; j++)
                {
                    v = v+state->pbasis.ptr.pp_double[state->basissize-1][j]*state->tmpbasis.ptr.pp_double[i][j]/state->h.ptr.p_double[j];
                }
                ae_v_subd(&state->tmpbasis.ptr.pp_double[i][0], 1, &state->pbasis.ptr.pp_double[state->basissize-1][0], 1, ae_v_len(0,n), v);
            }
        }
        for(j=0; j<=n; j++)
        {
            state->tmpbasis.ptr.pp_double[kmax][j] = (double)(0);
        }
    }
    
    /*
     * Orthogonalize linear constraints using traditional dot product
     * with respect to each other and boundary ones.
     *
     * NOTE: we force basis size to be equal to one which was computed
     *       at the previous step, with preconditioner-based inner product.
     */
    nactivelin = 0;
    for(i=0; i<=nec+nic-1; i++)
    {
        if( state->activeset.ptr.p_int[n+i]>0 )
        {
            ae_v_move(&state->tmpbasis.ptr.pp_double[nactivelin][0], 1, &state->cleic.ptr.pp_double[i][0], 1, ae_v_len(0,n));
            nactivelin = nactivelin+1;
        }
    }
    for(i=0; i<=nactivelin-1; i++)
    {
        v = 0.0;
        for(j=0; j<=n-1; j++)
        {
            v = v+ae_sqr(state->tmpbasis.ptr.pp_double[i][j], _state);
        }
        if( ae_fp_greater(v,(double)(0)) )
        {
            v = 1/ae_sqrt(v, _state);
            for(j=0; j<=n; j++)
            {
                state->tmpbasis.ptr.pp_double[i][j] = state->tmpbasis.ptr.pp_double[i][j]*v;
            }
        }
    }
    for(j=0; j<=n-1; j++)
    {
        if( state->activeset.ptr.p_int[j]>0 )
        {
            for(i=0; i<=nactivelin-1; i++)
            {
                state->tmpbasis.ptr.pp_double[i][n] = state->tmpbasis.ptr.pp_double[i][n]-state->tmpbasis.ptr.pp_double[i][j]*state->xc.ptr.p_double[j];
                state->tmpbasis.ptr.pp_double[i][j] = 0.0;
            }
        }
    }
    for(t=0; t<=state->basissize-1; t++)
    {
        
        /*
         * Find largest vector, add to basis.
         */
        vmax = (double)(-1);
        kmax = -1;
        for(i=0; i<=nactivelin-1; i++)
        {
            v = 0.0;
            for(j=0; j<=n-1; j++)
            {
                v = v+ae_sqr(state->tmpbasis.ptr.pp_double[i][j], _state);
            }
            v = ae_sqrt(v, _state);
            if( ae_fp_greater(v,vmax) )
            {
                vmax = v;
                kmax = i;
            }
        }
        if( ae_fp_eq(vmax,(double)(0)) )
        {
            for(j=0; j<=n; j++)
            {
                state->ibasis.ptr.pp_double[t][j] = 0.0;
            }
            continue;
        }
        v = 1/vmax;
        ae_v_moved(&state->ibasis.ptr.pp_double[t][0], 1, &state->tmpbasis.ptr.pp_double[kmax][0], 1, ae_v_len(0,n), v);
        
        /*
         * Reorthogonalize other vectors with respect to chosen one.
         * Remove it from the array.
         */
        for(i=0; i<=nactivelin-1; i++)
        {
            if( i!=kmax )
            {
                v = (double)(0);
                for(j=0; j<=n-1; j++)
                {
                    v = v+state->ibasis.ptr.pp_double[t][j]*state->tmpbasis.ptr.pp_double[i][j];
                }
                ae_v_subd(&state->tmpbasis.ptr.pp_double[i][0], 1, &state->ibasis.ptr.pp_double[t][0], 1, ae_v_len(0,n), v);
            }
        }
        for(j=0; j<=n; j++)
        {
            state->tmpbasis.ptr.pp_double[kmax][j] = (double)(0);
        }
    }
    
    /*
     * Orthogonalize linear constraints using inner product given by
     * scale matrix.
     *
     * NOTE: we force basis size to be equal to one which was computed
     *       with preconditioner-based inner product.
     */
    nactivelin = 0;
    for(i=0; i<=nec+nic-1; i++)
    {
        if( state->activeset.ptr.p_int[n+i]>0 )
        {
            ae_v_move(&state->tmpbasis.ptr.pp_double[nactivelin][0], 1, &state->cleic.ptr.pp_double[i][0], 1, ae_v_len(0,n));
            nactivelin = nactivelin+1;
        }
    }
    for(i=0; i<=nactivelin-1; i++)
    {
        v = 0.0;
        for(j=0; j<=n-1; j++)
        {
            v = v+ae_sqr(state->tmpbasis.ptr.pp_double[i][j]*state->s.ptr.p_double[j], _state);
        }
        if( ae_fp_greater(v,(double)(0)) )
        {
            v = 1/ae_sqrt(v, _state);
            for(j=0; j<=n; j++)
            {
                state->tmpbasis.ptr.pp_double[i][j] = state->tmpbasis.ptr.pp_double[i][j]*v;
            }
        }
    }
    for(j=0; j<=n-1; j++)
    {
        if( state->activeset.ptr.p_int[j]>0 )
        {
            for(i=0; i<=nactivelin-1; i++)
            {
                state->tmpbasis.ptr.pp_double[i][n] = state->tmpbasis.ptr.pp_double[i][n]-state->tmpbasis.ptr.pp_double[i][j]*state->xc.ptr.p_double[j];
                state->tmpbasis.ptr.pp_double[i][j] = 0.0;
            }
        }
    }
    for(t=0; t<=state->basissize-1; t++)
    {
        
        /*
         * Find largest vector, add to basis.
         */
        vmax = (double)(-1);
        kmax = -1;
        for(i=0; i<=nactivelin-1; i++)
        {
            v = 0.0;
            for(j=0; j<=n-1; j++)
            {
                v = v+ae_sqr(state->tmpbasis.ptr.pp_double[i][j]*state->s.ptr.p_double[j], _state);
            }
            v = ae_sqrt(v, _state);
            if( ae_fp_greater(v,vmax) )
            {
                vmax = v;
                kmax = i;
            }
        }
        if( ae_fp_eq(vmax,(double)(0)) )
        {
            for(j=0; j<=n; j++)
            {
                state->sbasis.ptr.pp_double[t][j] = 0.0;
            }
            continue;
        }
        v = 1/vmax;
        ae_v_moved(&state->sbasis.ptr.pp_double[t][0], 1, &state->tmpbasis.ptr.pp_double[kmax][0], 1, ae_v_len(0,n), v);
        
        /*
         * Reorthogonalize other vectors with respect to chosen one.
         * Remove it from the array.
         */
        for(i=0; i<=nactivelin-1; i++)
        {
            if( i!=kmax )
            {
                v = (double)(0);
                for(j=0; j<=n-1; j++)
                {
                    v = v+state->sbasis.ptr.pp_double[t][j]*state->tmpbasis.ptr.pp_double[i][j]*ae_sqr(state->s.ptr.p_double[j], _state);
                }
                ae_v_subd(&state->tmpbasis.ptr.pp_double[i][0], 1, &state->sbasis.ptr.pp_double[t][0], 1, ae_v_len(0,n), v);
            }
        }
        for(j=0; j<=n; j++)
        {
            state->tmpbasis.ptr.pp_double[kmax][j] = (double)(0);
        }
    }
}


/*************************************************************************
This  subroutine  calculates  preconditioned  descent direction subject to
current active set.

INPUT PARAMETERS:
    State   -   active set object
    G       -   array[N], gradient
    H       -   array[N], Hessian matrix
    HA      -   active constraints orthogonalized in such way
                that HA*inv(H)*HA'= I.
    Normalize-  whether we need normalized descent or not
    D       -   possibly preallocated buffer; automatically resized.
    
OUTPUT PARAMETERS:
    D       -   descent direction projected onto current active set.
                Components of D which correspond to active boundary
                constraints are forced to be exactly zero.
                In case D is non-zero and Normalize is True, it is
                normalized to have unit norm.
                
NOTE: if we have N active constraints, D is explicitly set to zero.

  -- ALGLIB --
     Copyright 21.12.2012 by Bochkanov Sergey
*************************************************************************/
static void sactivesets_constraineddescent(sactiveset* state,
     /* Real    */ ae_vector* g,
     /* Real    */ ae_vector* h,
     /* Real    */ ae_matrix* ha,
     ae_bool normalize,
     /* Real    */ ae_vector* d,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;
    double v;
    ae_int_t nactive;


    ae_assert(state->algostate==1, "SAS: internal error in ConstrainedDescent() - not in optimization mode", _state);
    ae_assert(state->basisisready, "SAS: internal error in ConstrainedDescent() - no basis", _state);
    n = state->n;
    rvectorsetlengthatleast(d, n, _state);
    
    /*
     * Calculate preconditioned constrained descent direction:
     *
     *     d := -inv(H)*( g - HA'*(HA*inv(H)*g) )
     *
     * Formula above always gives direction which is orthogonal to rows of HA.
     * You can verify it by multiplication of both sides by HA[i] (I-th row),
     * taking into account that HA*inv(H)*HA'= I (by definition of HA - it is
     * orthogonal basis with inner product given by inv(H)).
     */
    nactive = 0;
    for(i=0; i<=n-1; i++)
    {
        if( state->activeset.ptr.p_int[i]>0 )
        {
            d->ptr.p_double[i] = (double)(0);
            nactive = nactive+1;
        }
        else
        {
            d->ptr.p_double[i] = g->ptr.p_double[i];
        }
    }
    for(i=0; i<=state->basissize-1; i++)
    {
        v = 0.0;
        for(j=0; j<=n-1; j++)
        {
            v = v+ha->ptr.pp_double[i][j]*d->ptr.p_double[j]/h->ptr.p_double[j];
        }
        ae_v_subd(&d->ptr.p_double[0], 1, &ha->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
        nactive = nactive+1;
    }
    v = 0.0;
    for(i=0; i<=n-1; i++)
    {
        if( state->activeset.ptr.p_int[i]>0 )
        {
            d->ptr.p_double[i] = (double)(0);
        }
        else
        {
            d->ptr.p_double[i] = -d->ptr.p_double[i]/h->ptr.p_double[i];
            v = v+ae_sqr(d->ptr.p_double[i], _state);
        }
    }
    v = ae_sqrt(v, _state);
    if( nactive>=n )
    {
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            d->ptr.p_double[i] = (double)(0);
        }
    }
    if( normalize&&ae_fp_greater(v,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            d->ptr.p_double[i] = d->ptr.p_double[i]/v;
        }
    }
}


/*************************************************************************
This function recalculates constraints - activates  and  deactivates  them
according to gradient value at current point.

Algorithm  assumes  that  we  want  to make Quasi-Newton step from current
point with diagonal Quasi-Newton matrix H. Constraints are  activated  and
deactivated in such way that we won't violate any constraint by step.

Only already "active" and "candidate" elements of ActiveSet are  examined;
constraints which are not active are not examined.

INPUT PARAMETERS:
    State       -   active set object
    GC          -   array[N], gradient at XC
    H           -   array[N], Hessian matrix
    
OUTPUT PARAMETERS:
    State       -   active set object, with new set of constraint

  -- ALGLIB --
     Copyright 26.09.2012 by Bochkanov Sergey
*************************************************************************/
static void sactivesets_reactivateconstraints(sactiveset* state,
     /* Real    */ ae_vector* gc,
     /* Real    */ ae_vector* h,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t i;
    ae_int_t j;
    ae_int_t idx0;
    ae_int_t idx1;
    double v;
    ae_int_t nactivebnd;
    ae_int_t nactivelin;
    ae_int_t nactiveconstraints;
    double rowscale;


    ae_assert(state->algostate==1, "SASReactivateConstraintsPrec: must be in optimization mode", _state);
    
    /*
     * Prepare
     */
    n = state->n;
    nec = state->nec;
    nic = state->nic;
    state->basisisready = ae_false;
    
    /*
     * Handle important special case - no linear constraints,
     * only boundary constraints are present
     */
    if( nec+nic==0 )
    {
        for(i=0; i<=n-1; i++)
        {
            if( (state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i])&&ae_fp_eq(state->bndl.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
            {
                state->activeset.ptr.p_int[i] = 1;
                continue;
            }
            if( (state->hasbndl.ptr.p_bool[i]&&ae_fp_eq(state->xc.ptr.p_double[i],state->bndl.ptr.p_double[i]))&&ae_fp_greater_eq(gc->ptr.p_double[i],(double)(0)) )
            {
                state->activeset.ptr.p_int[i] = 1;
                continue;
            }
            if( (state->hasbndu.ptr.p_bool[i]&&ae_fp_eq(state->xc.ptr.p_double[i],state->bndu.ptr.p_double[i]))&&ae_fp_less_eq(gc->ptr.p_double[i],(double)(0)) )
            {
                state->activeset.ptr.p_int[i] = 1;
                continue;
            }
            state->activeset.ptr.p_int[i] = -1;
        }
        return;
    }
    
    /*
     * General case.
     * Allocate temporaries.
     */
    rvectorsetlengthatleast(&state->rctmpg, n, _state);
    rvectorsetlengthatleast(&state->rctmprightpart, n, _state);
    rvectorsetlengthatleast(&state->rctmps, n, _state);
    rmatrixsetlengthatleast(&state->rctmpdense0, n, nec+nic, _state);
    rmatrixsetlengthatleast(&state->rctmpdense1, n, nec+nic, _state);
    bvectorsetlengthatleast(&state->rctmpisequality, n+nec+nic, _state);
    ivectorsetlengthatleast(&state->rctmpconstraintidx, n+nec+nic, _state);
    
    /*
     * Calculate descent direction
     */
    ae_v_moveneg(&state->rctmpg.ptr.p_double[0], 1, &gc->ptr.p_double[0], 1, ae_v_len(0,n-1));
    
    /*
     * Determine candidates to the active set.
     *
     * After this block constraints become either "inactive" (ActiveSet[i]<0)
     * or "candidates" (ActiveSet[i]=0). Previously active constraints always
     * become "candidates".
     */
    for(i=0; i<=n+nec+nic-1; i++)
    {
        if( state->activeset.ptr.p_int[i]>0 )
        {
            state->activeset.ptr.p_int[i] = 0;
        }
        else
        {
            state->activeset.ptr.p_int[i] = -1;
        }
    }
    nactiveconstraints = 0;
    nactivebnd = 0;
    nactivelin = 0;
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * Activate boundary constraints:
         * * copy constraint index to RCTmpConstraintIdx
         * * set corresponding element of ActiveSet[] to "candidate"
         * * fill RCTmpS by either +1 (lower bound) or -1 (upper bound)
         * * set RCTmpIsEquality to False (BndL<BndU) or True (BndL=BndU)
         * * increase counters
         */
        if( (state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i])&&ae_fp_eq(state->bndl.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
        {
            
            /*
             * Equality constraint is activated
             */
            state->rctmpconstraintidx.ptr.p_int[nactiveconstraints] = i;
            state->activeset.ptr.p_int[i] = 0;
            state->rctmps.ptr.p_double[i] = 1.0;
            state->rctmpisequality.ptr.p_bool[nactiveconstraints] = ae_true;
            nactiveconstraints = nactiveconstraints+1;
            nactivebnd = nactivebnd+1;
            continue;
        }
        if( state->hasbndl.ptr.p_bool[i]&&ae_fp_eq(state->xc.ptr.p_double[i],state->bndl.ptr.p_double[i]) )
        {
            
            /*
             * Lower bound is activated
             */
            state->rctmpconstraintidx.ptr.p_int[nactiveconstraints] = i;
            state->activeset.ptr.p_int[i] = 0;
            state->rctmps.ptr.p_double[i] = -1.0;
            state->rctmpisequality.ptr.p_bool[nactiveconstraints] = ae_false;
            nactiveconstraints = nactiveconstraints+1;
            nactivebnd = nactivebnd+1;
            continue;
        }
        if( state->hasbndu.ptr.p_bool[i]&&ae_fp_eq(state->xc.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
        {
            
            /*
             * Upper bound is activated
             */
            state->rctmpconstraintidx.ptr.p_int[nactiveconstraints] = i;
            state->activeset.ptr.p_int[i] = 0;
            state->rctmps.ptr.p_double[i] = 1.0;
            state->rctmpisequality.ptr.p_bool[nactiveconstraints] = ae_false;
            nactiveconstraints = nactiveconstraints+1;
            nactivebnd = nactivebnd+1;
            continue;
        }
    }
    for(i=0; i<=nec+nic-1; i++)
    {
        if( i>=nec&&state->activeset.ptr.p_int[n+i]<0 )
        {
            
            /*
             * Inequality constraints are skipped if both (a) constraint was
             * not active, and (b) we are too far away from the boundary.
             */
            rowscale = 0.0;
            v = -state->cleic.ptr.pp_double[i][n];
            for(j=0; j<=n-1; j++)
            {
                v = v+state->cleic.ptr.pp_double[i][j]*state->xc.ptr.p_double[j];
                rowscale = ae_maxreal(rowscale, ae_fabs(state->cleic.ptr.pp_double[i][j]*state->s.ptr.p_double[j], _state), _state);
            }
            if( ae_fp_less_eq(v,-1.0E5*ae_machineepsilon*rowscale) )
            {
                
                /*
                 * NOTE: it is important to check for non-strict inequality
                 *       because we have to correctly handle zero constraint
                 *       0*x<=0
                 */
                continue;
            }
        }
        ae_v_move(&state->rctmpdense0.ptr.pp_double[0][nactivelin], state->rctmpdense0.stride, &state->cleic.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
        state->rctmpconstraintidx.ptr.p_int[nactiveconstraints] = n+i;
        state->activeset.ptr.p_int[n+i] = 0;
        state->rctmpisequality.ptr.p_bool[nactiveconstraints] = i<nec;
        nactiveconstraints = nactiveconstraints+1;
        nactivelin = nactivelin+1;
    }
    
    /*
     * Skip if no "candidate" constraints was found
     */
    if( nactiveconstraints==0 )
    {
        for(i=0; i<=n-1; i++)
        {
            if( (state->hasbndl.ptr.p_bool[i]&&state->hasbndu.ptr.p_bool[i])&&ae_fp_eq(state->bndl.ptr.p_double[i],state->bndu.ptr.p_double[i]) )
            {
                state->activeset.ptr.p_int[i] = 1;
                continue;
            }
            if( (state->hasbndl.ptr.p_bool[i]&&ae_fp_eq(state->xc.ptr.p_double[i],state->bndl.ptr.p_double[i]))&&ae_fp_greater_eq(gc->ptr.p_double[i],(double)(0)) )
            {
                state->activeset.ptr.p_int[i] = 1;
                continue;
            }
            if( (state->hasbndu.ptr.p_bool[i]&&ae_fp_eq(state->xc.ptr.p_double[i],state->bndu.ptr.p_double[i]))&&ae_fp_less_eq(gc->ptr.p_double[i],(double)(0)) )
            {
                state->activeset.ptr.p_int[i] = 1;
                continue;
            }
        }
        return;
    }
    
    /*
     * General case.
     *
     * APPROACH TO CONSTRAINTS ACTIVATION/DEACTIVATION
     *
     * We have NActiveConstraints "candidates": NActiveBnd boundary candidates,
     * NActiveLin linear candidates. Indexes of boundary constraints are stored
     * in RCTmpConstraintIdx[0:NActiveBnd-1], indexes of linear ones are stored
     * in RCTmpConstraintIdx[NActiveBnd:NActiveBnd+NActiveLin-1]. Some of the
     * constraints are equality ones, some are inequality - as specified by 
     * RCTmpIsEquality[i].
     *
     * Now we have to determine active subset of "candidates" set. In order to
     * do so we solve following constrained minimization problem:
     *         (                         )^2
     *     min ( SUM(lambda[i]*A[i]) + G )
     *         (                         )
     * Here:
     * * G is a gradient (column vector)
     * * A[i] is a column vector, linear (left) part of I-th constraint.
     *   I=0..NActiveConstraints-1, first NActiveBnd elements of A are just
     *   subset of identity matrix (boundary constraints), next NActiveLin
     *   elements are subset of rows of the matrix of general linear constraints.
     * * lambda[i] is a Lagrange multiplier corresponding to I-th constraint
     *
     * NOTE: for preconditioned setting A is replaced by A*H^(-0.5), G is
     *       replaced by G*H^(-0.5). We apply this scaling at the last stage,
     *       before passing data to NNLS solver.
     *
     * Minimization is performed subject to non-negativity constraints on
     * lambda[i] corresponding to inequality constraints. Inequality constraints
     * which correspond to non-zero lambda are activated, equality constraints
     * are always considered active.
     *
     * Informally speaking, we "decompose" descent direction -G and represent
     * it as sum of constraint vectors and "residual" part (which is equal to
     * the actual descent direction subject to constraints).
     *
     * SOLUTION OF THE NNLS PROBLEM
     *
     * We solve this optimization problem with Non-Negative Least Squares solver,
     * which can efficiently solve least squares problems of the form
     *
     *         ( [ I | AU ]     )^2
     *     min ( [   |    ]*x-b )   s.t. non-negativity constraints on some x[i]
     *         ( [ 0 | AL ]     )
     *
     * In order to use this solver we have to rearrange rows of A[] and G in
     * such way that first NActiveBnd columns of A store identity matrix (before
     * sorting non-zero elements are randomly distributed in the first NActiveBnd
     * columns of A, during sorting we move them to first NActiveBnd rows).
     *
     * Then we create instance of NNLS solver (we reuse instance left from the
     * previous run of the optimization problem) and solve NNLS problem.
     */
    idx0 = 0;
    idx1 = nactivebnd;
    for(i=0; i<=n-1; i++)
    {
        if( state->activeset.ptr.p_int[i]>=0 )
        {
            v = 1/ae_sqrt(h->ptr.p_double[i], _state);
            for(j=0; j<=nactivelin-1; j++)
            {
                state->rctmpdense1.ptr.pp_double[idx0][j] = state->rctmpdense0.ptr.pp_double[i][j]/state->rctmps.ptr.p_double[i]*v;
            }
            state->rctmprightpart.ptr.p_double[idx0] = state->rctmpg.ptr.p_double[i]/state->rctmps.ptr.p_double[i]*v;
            idx0 = idx0+1;
        }
        else
        {
            v = 1/ae_sqrt(h->ptr.p_double[i], _state);
            for(j=0; j<=nactivelin-1; j++)
            {
                state->rctmpdense1.ptr.pp_double[idx1][j] = state->rctmpdense0.ptr.pp_double[i][j]*v;
            }
            state->rctmprightpart.ptr.p_double[idx1] = state->rctmpg.ptr.p_double[i]*v;
            idx1 = idx1+1;
        }
    }
    snnlsinit(n, nec+nic, n, &state->solver, _state);
    snnlssetproblem(&state->solver, &state->rctmpdense1, &state->rctmprightpart, nactivebnd, nactiveconstraints-nactivebnd, n, _state);
    for(i=0; i<=nactiveconstraints-1; i++)
    {
        if( state->rctmpisequality.ptr.p_bool[i] )
        {
            snnlsdropnnc(&state->solver, i, _state);
        }
    }
    snnlssolve(&state->solver, &state->rctmplambdas, _state);
    
    /*
     * After solution of the problem we activate equality constraints (always active)
     * and inequality constraints with non-zero Lagrange multipliers. Then we reorthogonalize
     * active constraints.
     */
    for(i=0; i<=n+nec+nic-1; i++)
    {
        state->activeset.ptr.p_int[i] = -1;
    }
    for(i=0; i<=nactiveconstraints-1; i++)
    {
        if( state->rctmpisequality.ptr.p_bool[i]||ae_fp_greater(state->rctmplambdas.ptr.p_double[i],(double)(0)) )
        {
            state->activeset.ptr.p_int[state->rctmpconstraintidx.ptr.p_int[i]] = 1;
        }
        else
        {
            state->activeset.ptr.p_int[state->rctmpconstraintidx.ptr.p_int[i]] = 0;
        }
    }
    sasrebuildbasis(state, _state);
}


void _sactiveset_init(void* _p, ae_state *_state)
{
    sactiveset *p = (sactiveset*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->xc, 0, DT_REAL, _state);
    ae_vector_init(&p->s, 0, DT_REAL, _state);
    ae_vector_init(&p->h, 0, DT_REAL, _state);
    ae_vector_init(&p->activeset, 0, DT_INT, _state);
    ae_matrix_init(&p->sbasis, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->pbasis, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->ibasis, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->hasbndl, 0, DT_BOOL, _state);
    ae_vector_init(&p->hasbndu, 0, DT_BOOL, _state);
    ae_vector_init(&p->bndl, 0, DT_REAL, _state);
    ae_vector_init(&p->bndu, 0, DT_REAL, _state);
    ae_matrix_init(&p->cleic, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->mtx, 0, DT_REAL, _state);
    ae_vector_init(&p->mtas, 0, DT_INT, _state);
    ae_vector_init(&p->cdtmp, 0, DT_REAL, _state);
    ae_vector_init(&p->corrtmp, 0, DT_REAL, _state);
    ae_vector_init(&p->unitdiagonal, 0, DT_REAL, _state);
    _snnlssolver_init(&p->solver, _state);
    ae_vector_init(&p->scntmp, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp0, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpfeas, 0, DT_REAL, _state);
    ae_matrix_init(&p->tmpm0, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->rctmps, 0, DT_REAL, _state);
    ae_vector_init(&p->rctmpg, 0, DT_REAL, _state);
    ae_vector_init(&p->rctmprightpart, 0, DT_REAL, _state);
    ae_matrix_init(&p->rctmpdense0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->rctmpdense1, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->rctmpisequality, 0, DT_BOOL, _state);
    ae_vector_init(&p->rctmpconstraintidx, 0, DT_INT, _state);
    ae_vector_init(&p->rctmplambdas, 0, DT_REAL, _state);
    ae_matrix_init(&p->tmpbasis, 0, 0, DT_REAL, _state);
}


void _sactiveset_init_copy(void* _dst, void* _src, ae_state *_state)
{
    sactiveset *dst = (sactiveset*)_dst;
    sactiveset *src = (sactiveset*)_src;
    dst->n = src->n;
    dst->algostate = src->algostate;
    ae_vector_init_copy(&dst->xc, &src->xc, _state);
    dst->hasxc = src->hasxc;
    ae_vector_init_copy(&dst->s, &src->s, _state);
    ae_vector_init_copy(&dst->h, &src->h, _state);
    ae_vector_init_copy(&dst->activeset, &src->activeset, _state);
    dst->basisisready = src->basisisready;
    ae_matrix_init_copy(&dst->sbasis, &src->sbasis, _state);
    ae_matrix_init_copy(&dst->pbasis, &src->pbasis, _state);
    ae_matrix_init_copy(&dst->ibasis, &src->ibasis, _state);
    dst->basissize = src->basissize;
    dst->constraintschanged = src->constraintschanged;
    ae_vector_init_copy(&dst->hasbndl, &src->hasbndl, _state);
    ae_vector_init_copy(&dst->hasbndu, &src->hasbndu, _state);
    ae_vector_init_copy(&dst->bndl, &src->bndl, _state);
    ae_vector_init_copy(&dst->bndu, &src->bndu, _state);
    ae_matrix_init_copy(&dst->cleic, &src->cleic, _state);
    dst->nec = src->nec;
    dst->nic = src->nic;
    ae_vector_init_copy(&dst->mtx, &src->mtx, _state);
    ae_vector_init_copy(&dst->mtas, &src->mtas, _state);
    ae_vector_init_copy(&dst->cdtmp, &src->cdtmp, _state);
    ae_vector_init_copy(&dst->corrtmp, &src->corrtmp, _state);
    ae_vector_init_copy(&dst->unitdiagonal, &src->unitdiagonal, _state);
    _snnlssolver_init_copy(&dst->solver, &src->solver, _state);
    ae_vector_init_copy(&dst->scntmp, &src->scntmp, _state);
    ae_vector_init_copy(&dst->tmp0, &src->tmp0, _state);
    ae_vector_init_copy(&dst->tmpfeas, &src->tmpfeas, _state);
    ae_matrix_init_copy(&dst->tmpm0, &src->tmpm0, _state);
    ae_vector_init_copy(&dst->rctmps, &src->rctmps, _state);
    ae_vector_init_copy(&dst->rctmpg, &src->rctmpg, _state);
    ae_vector_init_copy(&dst->rctmprightpart, &src->rctmprightpart, _state);
    ae_matrix_init_copy(&dst->rctmpdense0, &src->rctmpdense0, _state);
    ae_matrix_init_copy(&dst->rctmpdense1, &src->rctmpdense1, _state);
    ae_vector_init_copy(&dst->rctmpisequality, &src->rctmpisequality, _state);
    ae_vector_init_copy(&dst->rctmpconstraintidx, &src->rctmpconstraintidx, _state);
    ae_vector_init_copy(&dst->rctmplambdas, &src->rctmplambdas, _state);
    ae_matrix_init_copy(&dst->tmpbasis, &src->tmpbasis, _state);
}


void _sactiveset_clear(void* _p)
{
    sactiveset *p = (sactiveset*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->xc);
    ae_vector_clear(&p->s);
    ae_vector_clear(&p->h);
    ae_vector_clear(&p->activeset);
    ae_matrix_clear(&p->sbasis);
    ae_matrix_clear(&p->pbasis);
    ae_matrix_clear(&p->ibasis);
    ae_vector_clear(&p->hasbndl);
    ae_vector_clear(&p->hasbndu);
    ae_vector_clear(&p->bndl);
    ae_vector_clear(&p->bndu);
    ae_matrix_clear(&p->cleic);
    ae_vector_clear(&p->mtx);
    ae_vector_clear(&p->mtas);
    ae_vector_clear(&p->cdtmp);
    ae_vector_clear(&p->corrtmp);
    ae_vector_clear(&p->unitdiagonal);
    _snnlssolver_clear(&p->solver);
    ae_vector_clear(&p->scntmp);
    ae_vector_clear(&p->tmp0);
    ae_vector_clear(&p->tmpfeas);
    ae_matrix_clear(&p->tmpm0);
    ae_vector_clear(&p->rctmps);
    ae_vector_clear(&p->rctmpg);
    ae_vector_clear(&p->rctmprightpart);
    ae_matrix_clear(&p->rctmpdense0);
    ae_matrix_clear(&p->rctmpdense1);
    ae_vector_clear(&p->rctmpisequality);
    ae_vector_clear(&p->rctmpconstraintidx);
    ae_vector_clear(&p->rctmplambdas);
    ae_matrix_clear(&p->tmpbasis);
}


void _sactiveset_destroy(void* _p)
{
    sactiveset *p = (sactiveset*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->xc);
    ae_vector_destroy(&p->s);
    ae_vector_destroy(&p->h);
    ae_vector_destroy(&p->activeset);
    ae_matrix_destroy(&p->sbasis);
    ae_matrix_destroy(&p->pbasis);
    ae_matrix_destroy(&p->ibasis);
    ae_vector_destroy(&p->hasbndl);
    ae_vector_destroy(&p->hasbndu);
    ae_vector_destroy(&p->bndl);
    ae_vector_destroy(&p->bndu);
    ae_matrix_destroy(&p->cleic);
    ae_vector_destroy(&p->mtx);
    ae_vector_destroy(&p->mtas);
    ae_vector_destroy(&p->cdtmp);
    ae_vector_destroy(&p->corrtmp);
    ae_vector_destroy(&p->unitdiagonal);
    _snnlssolver_destroy(&p->solver);
    ae_vector_destroy(&p->scntmp);
    ae_vector_destroy(&p->tmp0);
    ae_vector_destroy(&p->tmpfeas);
    ae_matrix_destroy(&p->tmpm0);
    ae_vector_destroy(&p->rctmps);
    ae_vector_destroy(&p->rctmpg);
    ae_vector_destroy(&p->rctmprightpart);
    ae_matrix_destroy(&p->rctmpdense0);
    ae_matrix_destroy(&p->rctmpdense1);
    ae_vector_destroy(&p->rctmpisequality);
    ae_vector_destroy(&p->rctmpconstraintidx);
    ae_vector_destroy(&p->rctmplambdas);
    ae_matrix_destroy(&p->tmpbasis);
}


/*$ End $*/
