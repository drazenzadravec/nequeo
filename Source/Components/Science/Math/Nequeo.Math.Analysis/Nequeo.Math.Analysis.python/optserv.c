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
#include "optserv.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
This subroutine is used to prepare threshold value which will be used for
trimming of the target function (see comments on TrimFunction() for more
information).

This function accepts only one parameter: function value at the starting
point. It returns threshold which will be used for trimming.

  -- ALGLIB --
     Copyright 10.05.2011 by Bochkanov Sergey
*************************************************************************/
void trimprepare(double f, double* threshold, ae_state *_state)
{

    *threshold = 0;

    *threshold = 10*(ae_fabs(f, _state)+1);
}


/*************************************************************************
This subroutine is used to "trim" target function, i.e. to do following
transformation:

                   { {F,G}          if F<Threshold
    {F_tr, G_tr} = {
                   { {Threshold, 0} if F>=Threshold
                   
Such transformation allows us to  solve  problems  with  singularities  by
redefining function in such way that it becomes bounded from above.

  -- ALGLIB --
     Copyright 10.05.2011 by Bochkanov Sergey
*************************************************************************/
void trimfunction(double* f,
     /* Real    */ ae_vector* g,
     ae_int_t n,
     double threshold,
     ae_state *_state)
{
    ae_int_t i;


    if( ae_fp_greater_eq(*f,threshold) )
    {
        *f = threshold;
        for(i=0; i<=n-1; i++)
        {
            g->ptr.p_double[i] = 0.0;
        }
    }
}


/*************************************************************************
This function enforces boundary constraints in the X.

This function correctly (although a bit inefficient) handles BL[i] which
are -INF and BU[i] which are +INF.

We have NMain+NSlack  dimensional  X,  with first NMain components bounded
by BL/BU, and next NSlack ones bounded by non-negativity constraints.

INPUT PARAMETERS
    X       -   array[NMain+NSlack], point
    BL      -   array[NMain], lower bounds
                (may contain -INF, when bound is not present)
    HaveBL  -   array[NMain], if HaveBL[i] is False,
                then i-th bound is not present
    BU      -   array[NMain], upper bounds
                (may contain +INF, when bound is not present)
    HaveBU  -   array[NMain], if HaveBU[i] is False,
                then i-th bound is not present

OUTPUT PARAMETERS
    X       -   X with all constraints being enforced

It returns True when constraints are consistent,
False - when constraints are inconsistent.

  -- ALGLIB --
     Copyright 10.01.2012 by Bochkanov Sergey
*************************************************************************/
ae_bool enforceboundaryconstraints(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* bl,
     /* Boolean */ ae_vector* havebl,
     /* Real    */ ae_vector* bu,
     /* Boolean */ ae_vector* havebu,
     ae_int_t nmain,
     ae_int_t nslack,
     ae_state *_state)
{
    ae_int_t i;
    ae_bool result;


    result = ae_false;
    for(i=0; i<=nmain-1; i++)
    {
        if( (havebl->ptr.p_bool[i]&&havebu->ptr.p_bool[i])&&ae_fp_greater(bl->ptr.p_double[i],bu->ptr.p_double[i]) )
        {
            return result;
        }
        if( havebl->ptr.p_bool[i]&&ae_fp_less(x->ptr.p_double[i],bl->ptr.p_double[i]) )
        {
            x->ptr.p_double[i] = bl->ptr.p_double[i];
        }
        if( havebu->ptr.p_bool[i]&&ae_fp_greater(x->ptr.p_double[i],bu->ptr.p_double[i]) )
        {
            x->ptr.p_double[i] = bu->ptr.p_double[i];
        }
    }
    for(i=0; i<=nslack-1; i++)
    {
        if( ae_fp_less(x->ptr.p_double[nmain+i],(double)(0)) )
        {
            x->ptr.p_double[nmain+i] = (double)(0);
        }
    }
    result = ae_true;
    return result;
}


/*************************************************************************
This function projects gradient into feasible area of boundary constrained
optimization  problem.  X  can  be  infeasible  with  respect  to boundary
constraints.  We  have  NMain+NSlack  dimensional  X,   with  first  NMain 
components bounded by BL/BU, and next NSlack ones bounded by non-negativity
constraints.

INPUT PARAMETERS
    X       -   array[NMain+NSlack], point
    G       -   array[NMain+NSlack], gradient
    BL      -   lower bounds (may contain -INF, when bound is not present)
    HaveBL  -   if HaveBL[i] is False, then i-th bound is not present
    BU      -   upper bounds (may contain +INF, when bound is not present)
    HaveBU  -   if HaveBU[i] is False, then i-th bound is not present

OUTPUT PARAMETERS
    G       -   projection of G. Components of G which satisfy one of the
                following
                    (1) (X[I]<=BndL[I]) and (G[I]>0), OR
                    (2) (X[I]>=BndU[I]) and (G[I]<0)
                are replaced by zeros.

NOTE 1: this function assumes that constraints are feasible. It throws
exception otherwise.

NOTE 2: in fact, projection of ANTI-gradient is calculated,  because  this
function trims components of -G which points outside of the feasible area.
However, working with -G is considered confusing, because all optimization
source work with G.

  -- ALGLIB --
     Copyright 10.01.2012 by Bochkanov Sergey
*************************************************************************/
void projectgradientintobc(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* g,
     /* Real    */ ae_vector* bl,
     /* Boolean */ ae_vector* havebl,
     /* Real    */ ae_vector* bu,
     /* Boolean */ ae_vector* havebu,
     ae_int_t nmain,
     ae_int_t nslack,
     ae_state *_state)
{
    ae_int_t i;


    for(i=0; i<=nmain-1; i++)
    {
        ae_assert((!havebl->ptr.p_bool[i]||!havebu->ptr.p_bool[i])||ae_fp_less_eq(bl->ptr.p_double[i],bu->ptr.p_double[i]), "ProjectGradientIntoBC: internal error (infeasible constraints)", _state);
        if( (havebl->ptr.p_bool[i]&&ae_fp_less_eq(x->ptr.p_double[i],bl->ptr.p_double[i]))&&ae_fp_greater(g->ptr.p_double[i],(double)(0)) )
        {
            g->ptr.p_double[i] = (double)(0);
        }
        if( (havebu->ptr.p_bool[i]&&ae_fp_greater_eq(x->ptr.p_double[i],bu->ptr.p_double[i]))&&ae_fp_less(g->ptr.p_double[i],(double)(0)) )
        {
            g->ptr.p_double[i] = (double)(0);
        }
    }
    for(i=0; i<=nslack-1; i++)
    {
        if( ae_fp_less_eq(x->ptr.p_double[nmain+i],(double)(0))&&ae_fp_greater(g->ptr.p_double[nmain+i],(double)(0)) )
        {
            g->ptr.p_double[nmain+i] = (double)(0);
        }
    }
}


/*************************************************************************
Given
    a) initial point X0[NMain+NSlack]
       (feasible with respect to bound constraints)
    b) step vector alpha*D[NMain+NSlack]
    c) boundary constraints BndL[NMain], BndU[NMain]
    d) implicit non-negativity constraints for slack variables
this  function  calculates  bound  on  the step length subject to boundary
constraints.

It returns:
    *  MaxStepLen - such step length that X0+MaxStepLen*alpha*D is exactly
       at the boundary given by constraints
    *  VariableToFreeze - index of the constraint to be activated,
       0 <= VariableToFreeze < NMain+NSlack
    *  ValueToFreeze - value of the corresponding constraint.

Notes:
    * it is possible that several constraints can be activated by the step
      at once. In such cases only one constraint is returned. It is caller
      responsibility to check other constraints. This function makes  sure
      that we activate at least one constraint, and everything else is the
      responsibility of the caller.
    * steps smaller than MaxStepLen still can activate constraints due  to
      numerical errors. Thus purpose of this  function  is  not  to  guard 
      against accidental activation of the constraints - quite the reverse, 
      its purpose is to activate at least constraint upon performing  step
      which is too long.
    * in case there is no constraints to activate, we return negative
      VariableToFreeze and zero MaxStepLen and ValueToFreeze.
    * this function assumes that constraints are consistent; it throws
      exception otherwise.

INPUT PARAMETERS
    X           -   array[NMain+NSlack], point. Must be feasible with respect 
                    to bound constraints (exception will be thrown otherwise)
    D           -   array[NMain+NSlack], step direction
    alpha       -   scalar multiplier before D, alpha<>0
    BndL        -   lower bounds, array[NMain]
                    (may contain -INF, when bound is not present)
    HaveBndL    -   array[NMain], if HaveBndL[i] is False,
                    then i-th bound is not present
    BndU        -   array[NMain], upper bounds
                    (may contain +INF, when bound is not present)
    HaveBndU    -   array[NMain], if HaveBndU[i] is False,
                    then i-th bound is not present
    NMain       -   number of main variables
    NSlack      -   number of slack variables
    
OUTPUT PARAMETERS
    VariableToFreeze:
                    * negative value     = step is unbounded, ValueToFreeze=0,
                                           MaxStepLen=0.
                    * non-negative value = at least one constraint, given by
                                           this parameter, will  be  activated
                                           upon performing maximum step.
    ValueToFreeze-  value of the variable which will be constrained
    MaxStepLen  -   maximum length of the step. Can be zero when step vector
                    looks outside of the feasible area.

  -- ALGLIB --
     Copyright 10.01.2012 by Bochkanov Sergey
*************************************************************************/
void calculatestepbound(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* d,
     double alpha,
     /* Real    */ ae_vector* bndl,
     /* Boolean */ ae_vector* havebndl,
     /* Real    */ ae_vector* bndu,
     /* Boolean */ ae_vector* havebndu,
     ae_int_t nmain,
     ae_int_t nslack,
     ae_int_t* variabletofreeze,
     double* valuetofreeze,
     double* maxsteplen,
     ae_state *_state)
{
    ae_int_t i;
    double prevmax;
    double initval;

    *variabletofreeze = 0;
    *valuetofreeze = 0;
    *maxsteplen = 0;

    ae_assert(ae_fp_neq(alpha,(double)(0)), "CalculateStepBound: zero alpha", _state);
    *variabletofreeze = -1;
    initval = ae_maxrealnumber;
    *maxsteplen = initval;
    for(i=0; i<=nmain-1; i++)
    {
        if( havebndl->ptr.p_bool[i]&&ae_fp_less(alpha*d->ptr.p_double[i],(double)(0)) )
        {
            ae_assert(ae_fp_greater_eq(x->ptr.p_double[i],bndl->ptr.p_double[i]), "CalculateStepBound: infeasible X", _state);
            prevmax = *maxsteplen;
            *maxsteplen = safeminposrv(x->ptr.p_double[i]-bndl->ptr.p_double[i], -alpha*d->ptr.p_double[i], *maxsteplen, _state);
            if( ae_fp_less(*maxsteplen,prevmax) )
            {
                *variabletofreeze = i;
                *valuetofreeze = bndl->ptr.p_double[i];
            }
        }
        if( havebndu->ptr.p_bool[i]&&ae_fp_greater(alpha*d->ptr.p_double[i],(double)(0)) )
        {
            ae_assert(ae_fp_less_eq(x->ptr.p_double[i],bndu->ptr.p_double[i]), "CalculateStepBound: infeasible X", _state);
            prevmax = *maxsteplen;
            *maxsteplen = safeminposrv(bndu->ptr.p_double[i]-x->ptr.p_double[i], alpha*d->ptr.p_double[i], *maxsteplen, _state);
            if( ae_fp_less(*maxsteplen,prevmax) )
            {
                *variabletofreeze = i;
                *valuetofreeze = bndu->ptr.p_double[i];
            }
        }
    }
    for(i=0; i<=nslack-1; i++)
    {
        if( ae_fp_less(alpha*d->ptr.p_double[nmain+i],(double)(0)) )
        {
            ae_assert(ae_fp_greater_eq(x->ptr.p_double[nmain+i],(double)(0)), "CalculateStepBound: infeasible X", _state);
            prevmax = *maxsteplen;
            *maxsteplen = safeminposrv(x->ptr.p_double[nmain+i], -alpha*d->ptr.p_double[nmain+i], *maxsteplen, _state);
            if( ae_fp_less(*maxsteplen,prevmax) )
            {
                *variabletofreeze = nmain+i;
                *valuetofreeze = (double)(0);
            }
        }
    }
    if( ae_fp_eq(*maxsteplen,initval) )
    {
        *valuetofreeze = (double)(0);
        *maxsteplen = (double)(0);
    }
}


/*************************************************************************
This function postprocesses bounded step by:
* analysing step length (whether it is equal to MaxStepLen) and activating 
  constraint given by VariableToFreeze if needed
* checking for additional bound constraints to activate

This function uses final point of the step, quantities calculated  by  the
CalculateStepBound()  function.  As  result,  it  returns  point  which is 
exactly feasible with respect to boundary constraints.

NOTE 1: this function does NOT handle and check linear equality constraints
NOTE 2: when StepTaken=MaxStepLen we always activate at least one constraint

INPUT PARAMETERS
    X           -   array[NMain+NSlack], final point to postprocess
    XPrev       -   array[NMain+NSlack], initial point
    BndL        -   lower bounds, array[NMain]
                    (may contain -INF, when bound is not present)
    HaveBndL    -   array[NMain], if HaveBndL[i] is False,
                    then i-th bound is not present
    BndU        -   array[NMain], upper bounds
                    (may contain +INF, when bound is not present)
    HaveBndU    -   array[NMain], if HaveBndU[i] is False,
                    then i-th bound is not present
    NMain       -   number of main variables
    NSlack      -   number of slack variables
    VariableToFreeze-result of CalculateStepBound()
    ValueToFreeze-  result of CalculateStepBound()
    StepTaken   -   actual step length (actual step is equal to the possibly 
                    non-unit step direction vector times this parameter).
                    StepTaken<=MaxStepLen.
    MaxStepLen  -   result of CalculateStepBound()
    
OUTPUT PARAMETERS
    X           -   point bounded with respect to constraints.
                    components corresponding to active constraints are exactly
                    equal to the boundary values.
                    
RESULT:
    number of constraints activated in addition to previously active ones.
    Constraints which were DEACTIVATED are ignored (do not influence
    function value).

  -- ALGLIB --
     Copyright 10.01.2012 by Bochkanov Sergey
*************************************************************************/
ae_int_t postprocessboundedstep(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* xprev,
     /* Real    */ ae_vector* bndl,
     /* Boolean */ ae_vector* havebndl,
     /* Real    */ ae_vector* bndu,
     /* Boolean */ ae_vector* havebndu,
     ae_int_t nmain,
     ae_int_t nslack,
     ae_int_t variabletofreeze,
     double valuetofreeze,
     double steptaken,
     double maxsteplen,
     ae_state *_state)
{
    ae_int_t i;
    ae_bool wasactivated;
    ae_int_t result;


    ae_assert(variabletofreeze<0||ae_fp_less_eq(steptaken,maxsteplen), "Assertion failed", _state);
    
    /*
     * Activate constraints
     */
    if( variabletofreeze>=0&&ae_fp_eq(steptaken,maxsteplen) )
    {
        x->ptr.p_double[variabletofreeze] = valuetofreeze;
    }
    for(i=0; i<=nmain-1; i++)
    {
        if( havebndl->ptr.p_bool[i]&&ae_fp_less(x->ptr.p_double[i],bndl->ptr.p_double[i]) )
        {
            x->ptr.p_double[i] = bndl->ptr.p_double[i];
        }
        if( havebndu->ptr.p_bool[i]&&ae_fp_greater(x->ptr.p_double[i],bndu->ptr.p_double[i]) )
        {
            x->ptr.p_double[i] = bndu->ptr.p_double[i];
        }
    }
    for(i=0; i<=nslack-1; i++)
    {
        if( ae_fp_less_eq(x->ptr.p_double[nmain+i],(double)(0)) )
        {
            x->ptr.p_double[nmain+i] = (double)(0);
        }
    }
    
    /*
     * Calculate number of constraints being activated
     */
    result = 0;
    for(i=0; i<=nmain-1; i++)
    {
        wasactivated = ae_fp_neq(x->ptr.p_double[i],xprev->ptr.p_double[i])&&((havebndl->ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],bndl->ptr.p_double[i]))||(havebndu->ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],bndu->ptr.p_double[i])));
        wasactivated = wasactivated||variabletofreeze==i;
        if( wasactivated )
        {
            result = result+1;
        }
    }
    for(i=0; i<=nslack-1; i++)
    {
        wasactivated = ae_fp_neq(x->ptr.p_double[nmain+i],xprev->ptr.p_double[nmain+i])&&ae_fp_eq(x->ptr.p_double[nmain+i],0.0);
        wasactivated = wasactivated||variabletofreeze==nmain+i;
        if( wasactivated )
        {
            result = result+1;
        }
    }
    return result;
}


/*************************************************************************
The  purpose  of  this  function is to prevent algorithm from "unsticking" 
from  the  active  bound  constraints  because  of  numerical noise in the
gradient or Hessian.

It is done by zeroing some components of the search direction D.  D[i]  is
zeroed when both (a) and (b) are true:
a) corresponding X[i] is exactly at the boundary
b) |D[i]*S[i]| <= DropTol*Sqrt(SUM(D[i]^2*S[I]^2))

D  can  be  step  direction , antigradient, gradient, or anything similar. 
Sign of D does not matter, nor matters step length.

NOTE 1: boundary constraints are expected to be consistent, as well as X
        is expected to be feasible. Exception will be thrown otherwise.

INPUT PARAMETERS
    D           -   array[NMain+NSlack], direction
    X           -   array[NMain+NSlack], current point
    BndL        -   lower bounds, array[NMain]
                    (may contain -INF, when bound is not present)
    HaveBndL    -   array[NMain], if HaveBndL[i] is False,
                    then i-th bound is not present
    BndU        -   array[NMain], upper bounds
                    (may contain +INF, when bound is not present)
    HaveBndU    -   array[NMain], if HaveBndU[i] is False,
                    then i-th bound is not present
    S           -   array[NMain+NSlack], scaling of the variables
    NMain       -   number of main variables
    NSlack      -   number of slack variables
    DropTol     -   drop tolerance, >=0
    
OUTPUT PARAMETERS
    X           -   point bounded with respect to constraints.
                    components corresponding to active constraints are exactly
                    equal to the boundary values.

  -- ALGLIB --
     Copyright 10.01.2012 by Bochkanov Sergey
*************************************************************************/
void filterdirection(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* bndl,
     /* Boolean */ ae_vector* havebndl,
     /* Real    */ ae_vector* bndu,
     /* Boolean */ ae_vector* havebndu,
     /* Real    */ ae_vector* s,
     ae_int_t nmain,
     ae_int_t nslack,
     double droptol,
     ae_state *_state)
{
    ae_int_t i;
    double scalednorm;
    ae_bool isactive;


    scalednorm = 0.0;
    for(i=0; i<=nmain+nslack-1; i++)
    {
        scalednorm = scalednorm+ae_sqr(d->ptr.p_double[i]*s->ptr.p_double[i], _state);
    }
    scalednorm = ae_sqrt(scalednorm, _state);
    for(i=0; i<=nmain-1; i++)
    {
        ae_assert(!havebndl->ptr.p_bool[i]||ae_fp_greater_eq(x->ptr.p_double[i],bndl->ptr.p_double[i]), "FilterDirection: infeasible point", _state);
        ae_assert(!havebndu->ptr.p_bool[i]||ae_fp_less_eq(x->ptr.p_double[i],bndu->ptr.p_double[i]), "FilterDirection: infeasible point", _state);
        isactive = (havebndl->ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],bndl->ptr.p_double[i]))||(havebndu->ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],bndu->ptr.p_double[i]));
        if( isactive&&ae_fp_less_eq(ae_fabs(d->ptr.p_double[i]*s->ptr.p_double[i], _state),droptol*scalednorm) )
        {
            d->ptr.p_double[i] = 0.0;
        }
    }
    for(i=0; i<=nslack-1; i++)
    {
        ae_assert(ae_fp_greater_eq(x->ptr.p_double[nmain+i],(double)(0)), "FilterDirection: infeasible point", _state);
        if( ae_fp_eq(x->ptr.p_double[nmain+i],(double)(0))&&ae_fp_less_eq(ae_fabs(d->ptr.p_double[nmain+i]*s->ptr.p_double[nmain+i], _state),droptol*scalednorm) )
        {
            d->ptr.p_double[nmain+i] = 0.0;
        }
    }
}


/*************************************************************************
This function returns number of bound constraints whose state was  changed
(either activated or deactivated) when making step from XPrev to X.

Constraints are considered:
* active - when we are exactly at the boundary
* inactive - when we are not at the boundary

You should note that antigradient direction is NOT taken into account when
we make decions on the constraint status.

INPUT PARAMETERS
    X           -   array[NMain+NSlack], final point.
                    Must be feasible with respect to bound constraints.
    XPrev       -   array[NMain+NSlack], initial point.
                    Must be feasible with respect to bound constraints.
    BndL        -   lower bounds, array[NMain]
                    (may contain -INF, when bound is not present)
    HaveBndL    -   array[NMain], if HaveBndL[i] is False,
                    then i-th bound is not present
    BndU        -   array[NMain], upper bounds
                    (may contain +INF, when bound is not present)
    HaveBndU    -   array[NMain], if HaveBndU[i] is False,
                    then i-th bound is not present
    NMain       -   number of main variables
    NSlack      -   number of slack variables
    
RESULT:
    number of constraints whose state was changed.

  -- ALGLIB --
     Copyright 10.01.2012 by Bochkanov Sergey
*************************************************************************/
ae_int_t numberofchangedconstraints(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* xprev,
     /* Real    */ ae_vector* bndl,
     /* Boolean */ ae_vector* havebndl,
     /* Real    */ ae_vector* bndu,
     /* Boolean */ ae_vector* havebndu,
     ae_int_t nmain,
     ae_int_t nslack,
     ae_state *_state)
{
    ae_int_t i;
    ae_bool statuschanged;
    ae_int_t result;


    result = 0;
    for(i=0; i<=nmain-1; i++)
    {
        if( ae_fp_neq(x->ptr.p_double[i],xprev->ptr.p_double[i]) )
        {
            statuschanged = ae_false;
            if( havebndl->ptr.p_bool[i]&&(ae_fp_eq(x->ptr.p_double[i],bndl->ptr.p_double[i])||ae_fp_eq(xprev->ptr.p_double[i],bndl->ptr.p_double[i])) )
            {
                statuschanged = ae_true;
            }
            if( havebndu->ptr.p_bool[i]&&(ae_fp_eq(x->ptr.p_double[i],bndu->ptr.p_double[i])||ae_fp_eq(xprev->ptr.p_double[i],bndu->ptr.p_double[i])) )
            {
                statuschanged = ae_true;
            }
            if( statuschanged )
            {
                result = result+1;
            }
        }
    }
    for(i=0; i<=nslack-1; i++)
    {
        if( ae_fp_neq(x->ptr.p_double[nmain+i],xprev->ptr.p_double[nmain+i])&&(ae_fp_eq(x->ptr.p_double[nmain+i],(double)(0))||ae_fp_eq(xprev->ptr.p_double[nmain+i],(double)(0))) )
        {
            result = result+1;
        }
    }
    return result;
}


/*************************************************************************
This function finds feasible point of  (NMain+NSlack)-dimensional  problem
subject to NMain explicit boundary constraints (some  constraints  can  be
omitted), NSlack implicit non-negativity constraints,  K  linear  equality
constraints.

INPUT PARAMETERS
    X           -   array[NMain+NSlack], initial point.
    BndL        -   lower bounds, array[NMain]
                    (may contain -INF, when bound is not present)
    HaveBndL    -   array[NMain], if HaveBndL[i] is False,
                    then i-th bound is not present
    BndU        -   array[NMain], upper bounds
                    (may contain +INF, when bound is not present)
    HaveBndU    -   array[NMain], if HaveBndU[i] is False,
                    then i-th bound is not present
    NMain       -   number of main variables
    NSlack      -   number of slack variables
    CE          -   array[K,NMain+NSlack+1], equality  constraints CE*x=b.
                    Rows contain constraints, first  NMain+NSlack  columns
                    contain coefficients before X[], last  column  contain
                    right part.
    K           -   number of linear constraints
    EpsI        -   infeasibility (error in the right part) allowed in the
                    solution

OUTPUT PARAMETERS:
    X           -   feasible point or best infeasible point found before
                    algorithm termination
    QPIts       -   number of QP iterations (for debug purposes)
    GPAIts      -   number of GPA iterations (for debug purposes)
    
RESULT:
    True in case X is feasible, False - if it is infeasible.

  -- ALGLIB --
     Copyright 20.01.2012 by Bochkanov Sergey
*************************************************************************/
ae_bool findfeasiblepoint(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* bndl,
     /* Boolean */ ae_vector* havebndl,
     /* Real    */ ae_vector* bndu,
     /* Boolean */ ae_vector* havebndu,
     ae_int_t nmain,
     ae_int_t nslack,
     /* Real    */ ae_matrix* ce,
     ae_int_t k,
     double epsi,
     ae_int_t* qpits,
     ae_int_t* gpaits,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _ce;
    ae_int_t i;
    ae_int_t j;
    ae_int_t idx0;
    ae_int_t idx1;
    ae_vector permx;
    ae_vector xn;
    ae_vector xa;
    ae_vector newtonstep;
    ae_vector g;
    ae_vector pg;
    ae_matrix a;
    double armijostep;
    double armijobeststep;
    double armijobestfeas;
    double v;
    double mx;
    double feaserr;
    double feaserr0;
    double feaserr1;
    double feasold;
    double feasnew;
    double pgnorm;
    double vn;
    double vd;
    double stp;
    ae_int_t vartofreeze;
    double valtofreeze;
    double maxsteplen;
    ae_bool werechangesinconstraints;
    ae_bool stage1isover;
    ae_bool converged;
    ae_vector activeconstraints;
    ae_vector tmpk;
    ae_vector colnorms;
    ae_int_t nactive;
    ae_int_t nfree;
    ae_int_t nsvd;
    ae_vector p1;
    ae_vector p2;
    apbuffers buf;
    ae_vector w;
    ae_vector s;
    ae_matrix u;
    ae_matrix vt;
    ae_int_t itscount;
    ae_int_t itswithintolerance;
    ae_int_t maxitswithintolerance;
    ae_int_t badits;
    ae_int_t maxbadits;
    ae_int_t gparuns;
    ae_int_t maxarmijoruns;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_ce, ce, _state);
    ce = &_ce;
    *qpits = 0;
    *gpaits = 0;
    ae_vector_init(&permx, 0, DT_REAL, _state);
    ae_vector_init(&xn, 0, DT_REAL, _state);
    ae_vector_init(&xa, 0, DT_REAL, _state);
    ae_vector_init(&newtonstep, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&pg, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&activeconstraints, 0, DT_REAL, _state);
    ae_vector_init(&tmpk, 0, DT_REAL, _state);
    ae_vector_init(&colnorms, 0, DT_REAL, _state);
    ae_vector_init(&p1, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);
    _apbuffers_init(&buf, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt, 0, 0, DT_REAL, _state);

    maxitswithintolerance = 3;
    maxbadits = 3;
    maxarmijoruns = 5;
    *qpits = 0;
    *gpaits = 0;
    
    /*
     * Initial enforcement of the feasibility with respect to boundary constraints
     * NOTE: after this block we assume that boundary constraints are consistent.
     */
    if( !enforceboundaryconstraints(x, bndl, havebndl, bndu, havebndu, nmain, nslack, _state) )
    {
        result = ae_false;
        ae_frame_leave(_state);
        return result;
    }
    if( k==0 )
    {
        
        /*
         * No linear constraints, we can exit right now
         */
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Scale rows of CE in such way that max(CE[i,0..nmain+nslack-1])=1 for any i=0..k-1
     */
    for(i=0; i<=k-1; i++)
    {
        v = 0.0;
        for(j=0; j<=nmain+nslack-1; j++)
        {
            v = ae_maxreal(v, ae_fabs(ce->ptr.pp_double[i][j], _state), _state);
        }
        if( ae_fp_neq(v,(double)(0)) )
        {
            v = 1/v;
            ae_v_muld(&ce->ptr.pp_double[i][0], 1, ae_v_len(0,nmain+nslack), v);
        }
    }
    
    /*
     * Allocate temporaries
     */
    ae_vector_set_length(&xn, nmain+nslack, _state);
    ae_vector_set_length(&xa, nmain+nslack, _state);
    ae_vector_set_length(&permx, nmain+nslack, _state);
    ae_vector_set_length(&g, nmain+nslack, _state);
    ae_vector_set_length(&pg, nmain+nslack, _state);
    ae_vector_set_length(&tmpk, k, _state);
    ae_matrix_set_length(&a, k, nmain+nslack, _state);
    ae_vector_set_length(&activeconstraints, nmain+nslack, _state);
    ae_vector_set_length(&newtonstep, nmain+nslack, _state);
    ae_vector_set_length(&s, nmain+nslack, _state);
    ae_vector_set_length(&colnorms, nmain+nslack, _state);
    for(i=0; i<=nmain+nslack-1; i++)
    {
        s.ptr.p_double[i] = 1.0;
        colnorms.ptr.p_double[i] = 0.0;
        for(j=0; j<=k-1; j++)
        {
            colnorms.ptr.p_double[i] = colnorms.ptr.p_double[i]+ae_sqr(ce->ptr.pp_double[j][i], _state);
        }
    }
    
    /*
     * K>0, we have linear equality constraints combined with bound constraints.
     *
     * Try to find feasible point as minimizer of the quadratic function
     *     F(x) = 0.5*||CE*x-b||^2 = 0.5*x'*(CE'*CE)*x - (b'*CE)*x + 0.5*b'*b
     * subject to boundary constraints given by BL, BU and non-negativity of
     * the slack variables. BTW, we drop constant term because it does not
     * actually influences on the solution.
     *
     * Below we will assume that K>0.
     */
    itswithintolerance = 0;
    badits = 0;
    itscount = 0;
    for(;;)
    {
        
        /*
         * Stage 0: check for exact convergence
         */
        converged = ae_true;
        feaserr = (double)(0);
        for(i=0; i<=k-1; i++)
        {
            
            /*
             * Calculate:
             * * V - error in the right part
             * * MX - maximum term in the left part
             *
             * Terminate if error in the right part is not greater than 100*Eps*MX.
             *
             * IMPORTANT: we must perform check for non-strict inequality, i.e. to use <= instead of <.
             *            it will allow us to easily handle situations with zero rows of CE.
             */
            mx = (double)(0);
            v = -ce->ptr.pp_double[i][nmain+nslack];
            for(j=0; j<=nmain+nslack-1; j++)
            {
                mx = ae_maxreal(mx, ae_fabs(ce->ptr.pp_double[i][j]*x->ptr.p_double[j], _state), _state);
                v = v+ce->ptr.pp_double[i][j]*x->ptr.p_double[j];
            }
            feaserr = feaserr+ae_sqr(v, _state);
            converged = converged&&ae_fp_less_eq(ae_fabs(v, _state),100*ae_machineepsilon*mx);
        }
        feaserr = ae_sqrt(feaserr, _state);
        feaserr0 = feaserr;
        if( converged )
        {
            result = ae_fp_less_eq(feaserr,epsi);
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Stage 1: equality constrained quadratic programming
         *
         * * treat active bound constraints as equality ones (constraint is considered 
         *   active when we are at the boundary, independently of the antigradient direction)
         * * calculate unrestricted Newton step to point XM (which may be infeasible)
         *   calculate MaxStepLen = largest step in direction of XM which retains feasibility.
         * * perform bounded step from X to XN:
         *   a) XN=XM                  (if XM is feasible)
         *   b) XN=X-MaxStepLen*(XM-X) (otherwise)
         * * X := XN
         * * if XM (Newton step subject to currently active constraints) was feasible, goto Stage 2
         * * repeat Stage 1
         *
         * NOTE 1: in order to solve constrained qudratic subproblem we will have to reorder
         *         variables in such way that ones corresponding to inactive constraints will
         *         be first, and active ones will be last in the list. CE and X are now
         *                                                       [ xi ]
         *         separated into two parts: CE = [CEi CEa], x = [    ], where CEi/Xi correspond
         *                                                       [ xa ]
         *         to INACTIVE constraints, and CEa/Xa correspond to the ACTIVE ones.
         *
         *         Now, instead of F=0.5*x'*(CE'*CE)*x - (b'*CE)*x + 0.5*b'*b, we have
         *         F(xi) = 0.5*(CEi*xi,CEi*xi) + (CEa*xa-b,CEi*xi) + (0.5*CEa*xa-b,CEa*xa).
         *         Here xa is considered constant, i.e. we optimize with respect to xi, leaving xa fixed.
         *
         *         We can solve it by performing SVD of CEi and calculating pseudoinverse of the
         *         Hessian matrix. Of course, we do NOT calculate pseudoinverse explicitly - we
         *         just use singular vectors to perform implicit multiplication by it.
         *
         */
        for(;;)
        {
            
            /*
             * Calculate G - gradient subject to equality constraints,
             * multiply it by inverse of the Hessian diagonal to obtain initial
             * step vector.
             *
             * Bound step subject to constraints which can be activated,
             * run Armijo search with increasing step size.
             * Search is terminated when feasibility error stops to decrease.
             *
             * NOTE: it is important to test for "stops to decrease" instead
             * of "starts to increase" in order to correctly handle cases with
             * zero CE.
             */
            armijobeststep = 0.0;
            armijobestfeas = 0.0;
            for(i=0; i<=nmain+nslack-1; i++)
            {
                g.ptr.p_double[i] = (double)(0);
            }
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&ce->ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                v = v-ce->ptr.pp_double[i][nmain+nslack];
                armijobestfeas = armijobestfeas+ae_sqr(v, _state);
                ae_v_addd(&g.ptr.p_double[0], 1, &ce->ptr.pp_double[i][0], 1, ae_v_len(0,nmain+nslack-1), v);
            }
            armijobestfeas = ae_sqrt(armijobestfeas, _state);
            for(i=0; i<=nmain-1; i++)
            {
                if( havebndl->ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],bndl->ptr.p_double[i]) )
                {
                    g.ptr.p_double[i] = 0.0;
                }
                if( havebndu->ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],bndu->ptr.p_double[i]) )
                {
                    g.ptr.p_double[i] = 0.0;
                }
            }
            for(i=0; i<=nslack-1; i++)
            {
                if( ae_fp_eq(x->ptr.p_double[nmain+i],0.0) )
                {
                    g.ptr.p_double[nmain+i] = 0.0;
                }
            }
            v = 0.0;
            for(i=0; i<=nmain+nslack-1; i++)
            {
                if( ae_fp_neq(ae_sqr(colnorms.ptr.p_double[i], _state),(double)(0)) )
                {
                    newtonstep.ptr.p_double[i] = -g.ptr.p_double[i]/ae_sqr(colnorms.ptr.p_double[i], _state);
                }
                else
                {
                    newtonstep.ptr.p_double[i] = 0.0;
                }
                v = v+ae_sqr(newtonstep.ptr.p_double[i], _state);
            }
            if( ae_fp_eq(v,(double)(0)) )
            {
                
                /*
                 * Constrained gradient is zero, QP iterations are over
                 */
                break;
            }
            calculatestepbound(x, &newtonstep, 1.0, bndl, havebndl, bndu, havebndu, nmain, nslack, &vartofreeze, &valtofreeze, &maxsteplen, _state);
            if( vartofreeze>=0&&ae_fp_eq(maxsteplen,(double)(0)) )
            {
                
                /*
                 * Can not perform step, QP iterations are over
                 */
                break;
            }
            if( vartofreeze>=0 )
            {
                armijostep = ae_minreal(1.0, maxsteplen, _state);
            }
            else
            {
                armijostep = (double)(1);
            }
            for(;;)
            {
                ae_v_move(&xa.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                ae_v_addd(&xa.ptr.p_double[0], 1, &newtonstep.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1), armijostep);
                enforceboundaryconstraints(&xa, bndl, havebndl, bndu, havebndu, nmain, nslack, _state);
                feaserr = 0.0;
                for(i=0; i<=k-1; i++)
                {
                    v = ae_v_dotproduct(&ce->ptr.pp_double[i][0], 1, &xa.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                    v = v-ce->ptr.pp_double[i][nmain+nslack];
                    feaserr = feaserr+ae_sqr(v, _state);
                }
                feaserr = ae_sqrt(feaserr, _state);
                if( ae_fp_greater_eq(feaserr,armijobestfeas) )
                {
                    break;
                }
                armijobestfeas = feaserr;
                armijobeststep = armijostep;
                armijostep = 2.0*armijostep;
            }
            ae_v_addd(&x->ptr.p_double[0], 1, &newtonstep.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1), armijobeststep);
            enforceboundaryconstraints(x, bndl, havebndl, bndu, havebndu, nmain, nslack, _state);
            
            /*
             * Determine number of active and free constraints
             */
            nactive = 0;
            for(i=0; i<=nmain-1; i++)
            {
                activeconstraints.ptr.p_double[i] = (double)(0);
                if( havebndl->ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],bndl->ptr.p_double[i]) )
                {
                    activeconstraints.ptr.p_double[i] = (double)(1);
                }
                if( havebndu->ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],bndu->ptr.p_double[i]) )
                {
                    activeconstraints.ptr.p_double[i] = (double)(1);
                }
                if( ae_fp_greater(activeconstraints.ptr.p_double[i],(double)(0)) )
                {
                    nactive = nactive+1;
                }
            }
            for(i=0; i<=nslack-1; i++)
            {
                activeconstraints.ptr.p_double[nmain+i] = (double)(0);
                if( ae_fp_eq(x->ptr.p_double[nmain+i],0.0) )
                {
                    activeconstraints.ptr.p_double[nmain+i] = (double)(1);
                }
                if( ae_fp_greater(activeconstraints.ptr.p_double[nmain+i],(double)(0)) )
                {
                    nactive = nactive+1;
                }
            }
            nfree = nmain+nslack-nactive;
            if( nfree==0 )
            {
                break;
            }
            *qpits = *qpits+1;
            
            /*
             * Reorder variables
             */
            tagsortbuf(&activeconstraints, nmain+nslack, &p1, &p2, &buf, _state);
            for(i=0; i<=k-1; i++)
            {
                for(j=0; j<=nmain+nslack-1; j++)
                {
                    a.ptr.pp_double[i][j] = ce->ptr.pp_double[i][j];
                }
            }
            for(j=0; j<=nmain+nslack-1; j++)
            {
                permx.ptr.p_double[j] = x->ptr.p_double[j];
            }
            for(j=0; j<=nmain+nslack-1; j++)
            {
                if( p2.ptr.p_int[j]!=j )
                {
                    idx0 = p2.ptr.p_int[j];
                    idx1 = j;
                    for(i=0; i<=k-1; i++)
                    {
                        v = a.ptr.pp_double[i][idx0];
                        a.ptr.pp_double[i][idx0] = a.ptr.pp_double[i][idx1];
                        a.ptr.pp_double[i][idx1] = v;
                    }
                    v = permx.ptr.p_double[idx0];
                    permx.ptr.p_double[idx0] = permx.ptr.p_double[idx1];
                    permx.ptr.p_double[idx1] = v;
                }
            }
            
            /*
             * Calculate (unprojected) gradient:
             * G(xi) = CEi'*(CEi*xi + CEa*xa - b)
             */
            for(i=0; i<=nfree-1; i++)
            {
                g.ptr.p_double[i] = (double)(0);
            }
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &permx.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                tmpk.ptr.p_double[i] = v-ce->ptr.pp_double[i][nmain+nslack];
            }
            for(i=0; i<=k-1; i++)
            {
                v = tmpk.ptr.p_double[i];
                ae_v_addd(&g.ptr.p_double[0], 1, &a.ptr.pp_double[i][0], 1, ae_v_len(0,nfree-1), v);
            }
            
            /*
             * Calculate Newton step using SVD of CEi:
             *     F(xi)  = 0.5*xi'*H*xi + g'*xi    (Taylor decomposition)
             *     XN     = -H^(-1)*g               (new point, solution of the QP subproblem)
             *     H      = CEi'*CEi                
             *     CEi    = U*W*V'                  (SVD of CEi)
             *     H      = V*W^2*V'                 
             *     H^(-1) = V*W^(-2)*V'
             *     step     = -V*W^(-2)*V'*g          (it is better to perform multiplication from right to left)
             *
             * NOTE 1: we do NOT need left singular vectors to perform Newton step.
             */
            nsvd = ae_minint(k, nfree, _state);
            if( !rmatrixsvd(&a, k, nfree, 0, 1, 2, &w, &u, &vt, _state) )
            {
                result = ae_false;
                ae_frame_leave(_state);
                return result;
            }
            for(i=0; i<=nsvd-1; i++)
            {
                v = ae_v_dotproduct(&vt.ptr.pp_double[i][0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,nfree-1));
                tmpk.ptr.p_double[i] = v;
            }
            for(i=0; i<=nsvd-1; i++)
            {
                
                /*
                 * It is important to have strict ">" in order to correctly 
                 * handle zero singular values.
                 */
                if( ae_fp_greater(ae_sqr(w.ptr.p_double[i], _state),ae_sqr(w.ptr.p_double[0], _state)*(nmain+nslack)*ae_machineepsilon) )
                {
                    tmpk.ptr.p_double[i] = tmpk.ptr.p_double[i]/ae_sqr(w.ptr.p_double[i], _state);
                }
                else
                {
                    tmpk.ptr.p_double[i] = (double)(0);
                }
            }
            for(i=0; i<=nmain+nslack-1; i++)
            {
                newtonstep.ptr.p_double[i] = (double)(0);
            }
            for(i=0; i<=nsvd-1; i++)
            {
                v = tmpk.ptr.p_double[i];
                ae_v_subd(&newtonstep.ptr.p_double[0], 1, &vt.ptr.pp_double[i][0], 1, ae_v_len(0,nfree-1), v);
            }
            for(j=nmain+nslack-1; j>=0; j--)
            {
                if( p2.ptr.p_int[j]!=j )
                {
                    idx0 = p2.ptr.p_int[j];
                    idx1 = j;
                    v = newtonstep.ptr.p_double[idx0];
                    newtonstep.ptr.p_double[idx0] = newtonstep.ptr.p_double[idx1];
                    newtonstep.ptr.p_double[idx1] = v;
                }
            }
            
            /*
             * NewtonStep contains Newton step subject to active bound constraints.
             *
             * Such step leads us to the minimizer of the equality constrained F,
             * but such minimizer may be infeasible because some constraints which
             * are inactive at the initial point can be violated at the solution.
             *
             * Thus, we perform optimization in two stages:
             * a) perform bounded Newton step, i.e. step in the Newton direction
             *    until activation of the first constraint
             * b) in case (MaxStepLen>0)and(MaxStepLen<1), perform additional iteration
             *    of the Armijo line search in the rest of the Newton direction.
             */
            calculatestepbound(x, &newtonstep, 1.0, bndl, havebndl, bndu, havebndu, nmain, nslack, &vartofreeze, &valtofreeze, &maxsteplen, _state);
            if( vartofreeze>=0&&ae_fp_eq(maxsteplen,(double)(0)) )
            {
                
                /*
                 * Activation of the constraints prevent us from performing step,
                 * QP iterations are over
                 */
                break;
            }
            if( vartofreeze>=0 )
            {
                v = ae_minreal(1.0, maxsteplen, _state);
            }
            else
            {
                v = 1.0;
            }
            ae_v_moved(&xn.ptr.p_double[0], 1, &newtonstep.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1), v);
            ae_v_add(&xn.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
            postprocessboundedstep(&xn, x, bndl, havebndl, bndu, havebndu, nmain, nslack, vartofreeze, valtofreeze, v, maxsteplen, _state);
            if( ae_fp_greater(maxsteplen,(double)(0))&&ae_fp_less(maxsteplen,(double)(1)) )
            {
                
                /*
                 * Newton step was restricted by activation of the constraints,
                 * perform Armijo iteration.
                 *
                 * Initial estimate for best step is zero step. We try different
                 * step sizes, from the 1-MaxStepLen (residual of the full Newton
                 * step) to progressively smaller and smaller steps.
                 */
                armijobeststep = 0.0;
                armijobestfeas = 0.0;
                for(i=0; i<=k-1; i++)
                {
                    v = ae_v_dotproduct(&ce->ptr.pp_double[i][0], 1, &xn.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                    v = v-ce->ptr.pp_double[i][nmain+nslack];
                    armijobestfeas = armijobestfeas+ae_sqr(v, _state);
                }
                armijobestfeas = ae_sqrt(armijobestfeas, _state);
                armijostep = 1-maxsteplen;
                for(j=0; j<=maxarmijoruns-1; j++)
                {
                    ae_v_move(&xa.ptr.p_double[0], 1, &xn.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                    ae_v_addd(&xa.ptr.p_double[0], 1, &newtonstep.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1), armijostep);
                    enforceboundaryconstraints(&xa, bndl, havebndl, bndu, havebndu, nmain, nslack, _state);
                    feaserr = 0.0;
                    for(i=0; i<=k-1; i++)
                    {
                        v = ae_v_dotproduct(&ce->ptr.pp_double[i][0], 1, &xa.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                        v = v-ce->ptr.pp_double[i][nmain+nslack];
                        feaserr = feaserr+ae_sqr(v, _state);
                    }
                    feaserr = ae_sqrt(feaserr, _state);
                    if( ae_fp_less(feaserr,armijobestfeas) )
                    {
                        armijobestfeas = feaserr;
                        armijobeststep = armijostep;
                    }
                    armijostep = 0.5*armijostep;
                }
                ae_v_move(&xa.ptr.p_double[0], 1, &xn.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                ae_v_addd(&xa.ptr.p_double[0], 1, &newtonstep.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1), armijobeststep);
                enforceboundaryconstraints(&xa, bndl, havebndl, bndu, havebndu, nmain, nslack, _state);
            }
            else
            {
                
                /*
                 * Armijo iteration is not performed
                 */
                ae_v_move(&xa.ptr.p_double[0], 1, &xn.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
            }
            stage1isover = ae_fp_greater_eq(maxsteplen,(double)(1))||ae_fp_eq(maxsteplen,(double)(0));
            
            /*
             * Calculate feasibility errors for old and new X.
             * These quantinies are used for debugging purposes only.
             * However, we can leave them in release code because performance impact is insignificant.
             *
             * Update X. Exit if needed.
             */
            feasold = (double)(0);
            feasnew = (double)(0);
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&ce->ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                feasold = feasold+ae_sqr(v-ce->ptr.pp_double[i][nmain+nslack], _state);
                v = ae_v_dotproduct(&ce->ptr.pp_double[i][0], 1, &xa.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                feasnew = feasnew+ae_sqr(v-ce->ptr.pp_double[i][nmain+nslack], _state);
            }
            feasold = ae_sqrt(feasold, _state);
            feasnew = ae_sqrt(feasnew, _state);
            if( ae_fp_greater_eq(feasnew,feasold) )
            {
                break;
            }
            ae_v_move(&x->ptr.p_double[0], 1, &xa.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
            if( stage1isover )
            {
                break;
            }
        }
        
        /*
         * Stage 2: gradient projection algorithm (GPA)
         *
         * * calculate feasibility error (with respect to linear equality constraints)
         * * calculate gradient G of F, project it into feasible area (G => PG)
         * * exit if norm(PG) is exactly zero or feasibility error is smaller than EpsC
         * * let XM be exact minimum of F along -PG (XM may be infeasible).
         *   calculate MaxStepLen = largest step in direction of -PG which retains feasibility.
         * * perform bounded step from X to XN:
         *   a) XN=XM              (if XM is feasible)
         *   b) XN=X-MaxStepLen*PG (otherwise)
         * * X := XN
         * * stop after specified number of iterations or when no new constraints was activated
         *
         * NOTES:
         * * grad(F) = (CE'*CE)*x - (b'*CE)^T
         * * CE[i] denotes I-th row of CE
         * * XM = X+stp*(-PG) where stp=(grad(F(X)),PG)/(CE*PG,CE*PG).
         *   Here PG is a projected gradient, but in fact it can be arbitrary non-zero 
         *   direction vector - formula for minimum of F along PG still will be correct.
         */
        werechangesinconstraints = ae_false;
        for(gparuns=1; gparuns<=k; gparuns++)
        {
            
            /*
             * calculate feasibility error and G
             */
            feaserr = (double)(0);
            for(i=0; i<=nmain+nslack-1; i++)
            {
                g.ptr.p_double[i] = (double)(0);
            }
            for(i=0; i<=k-1; i++)
            {
                
                /*
                 * G += CE[i]^T * (CE[i]*x-b[i])
                 */
                v = ae_v_dotproduct(&ce->ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                v = v-ce->ptr.pp_double[i][nmain+nslack];
                feaserr = feaserr+ae_sqr(v, _state);
                ae_v_addd(&g.ptr.p_double[0], 1, &ce->ptr.pp_double[i][0], 1, ae_v_len(0,nmain+nslack-1), v);
            }
            
            /*
             * project G, filter it (strip numerical noise)
             */
            ae_v_move(&pg.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
            projectgradientintobc(x, &pg, bndl, havebndl, bndu, havebndu, nmain, nslack, _state);
            filterdirection(&pg, x, bndl, havebndl, bndu, havebndu, &s, nmain, nslack, 1.0E-9, _state);
            for(i=0; i<=nmain+nslack-1; i++)
            {
                if( ae_fp_neq(ae_sqr(colnorms.ptr.p_double[i], _state),(double)(0)) )
                {
                    pg.ptr.p_double[i] = pg.ptr.p_double[i]/ae_sqr(colnorms.ptr.p_double[i], _state);
                }
                else
                {
                    pg.ptr.p_double[i] = 0.0;
                }
            }
            
            /*
             * Check GNorm and feasibility.
             * Exit when GNorm is exactly zero.
             */
            pgnorm = ae_v_dotproduct(&pg.ptr.p_double[0], 1, &pg.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
            feaserr = ae_sqrt(feaserr, _state);
            pgnorm = ae_sqrt(pgnorm, _state);
            if( ae_fp_eq(pgnorm,(double)(0)) )
            {
                result = ae_fp_less_eq(feaserr,epsi);
                ae_frame_leave(_state);
                return result;
            }
            
            /*
             * calculate planned step length
             */
            vn = ae_v_dotproduct(&g.ptr.p_double[0], 1, &pg.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
            vd = (double)(0);
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&ce->ptr.pp_double[i][0], 1, &pg.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
                vd = vd+ae_sqr(v, _state);
            }
            stp = vn/vd;
            
            /*
             * Calculate step bound.
             * Perform bounded step and post-process it
             */
            calculatestepbound(x, &pg, -1.0, bndl, havebndl, bndu, havebndu, nmain, nslack, &vartofreeze, &valtofreeze, &maxsteplen, _state);
            if( vartofreeze>=0&&ae_fp_eq(maxsteplen,(double)(0)) )
            {
                result = ae_false;
                ae_frame_leave(_state);
                return result;
            }
            if( vartofreeze>=0 )
            {
                v = ae_minreal(stp, maxsteplen, _state);
            }
            else
            {
                v = stp;
            }
            ae_v_move(&xn.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
            ae_v_subd(&xn.ptr.p_double[0], 1, &pg.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1), v);
            postprocessboundedstep(&xn, x, bndl, havebndl, bndu, havebndu, nmain, nslack, vartofreeze, valtofreeze, v, maxsteplen, _state);
            
            /*
             * update X
             * check stopping criteria
             */
            werechangesinconstraints = werechangesinconstraints||numberofchangedconstraints(&xn, x, bndl, havebndl, bndu, havebndu, nmain, nslack, _state)>0;
            ae_v_move(&x->ptr.p_double[0], 1, &xn.ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
            *gpaits = *gpaits+1;
            if( !werechangesinconstraints )
            {
                break;
            }
        }
        
        /*
         * Stage 3: decide to stop algorithm or not to stop
         *
         * 1. we can stop when last GPA run did NOT changed constraints status.
         *    It means that we've found final set of the active constraints even
         *    before GPA made its run. And it means that Newton step moved us to
         *    the minimum subject to the present constraints.
         *    Depending on feasibility error, True or False is returned.
         */
        feaserr = (double)(0);
        for(i=0; i<=k-1; i++)
        {
            v = ae_v_dotproduct(&ce->ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,nmain+nslack-1));
            v = v-ce->ptr.pp_double[i][nmain+nslack];
            feaserr = feaserr+ae_sqr(v, _state);
        }
        feaserr = ae_sqrt(feaserr, _state);
        feaserr1 = feaserr;
        if( ae_fp_greater_eq(feaserr1,feaserr0*(1-1000*ae_machineepsilon)) )
        {
            inc(&badits, _state);
        }
        else
        {
            badits = 0;
        }
        if( ae_fp_less_eq(feaserr,epsi) )
        {
            inc(&itswithintolerance, _state);
        }
        else
        {
            itswithintolerance = 0;
        }
        if( (!werechangesinconstraints||itswithintolerance>=maxitswithintolerance)||badits>=maxbadits )
        {
            result = ae_fp_less_eq(feaserr,epsi);
            ae_frame_leave(_state);
            return result;
        }
        itscount = itscount+1;
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
    This function checks that input derivatives are right. First it scales
parameters DF0 and DF1 from segment [A;B] to [0;1]. Then it builds Hermite
spline and derivative of it in 0.5. Search scale as Max(DF0,DF1, |F0-F1|).
Right derivative has to satisfy condition:
    |H-F|/S<=0,01, |H'-F'|/S<=0,01.
    
INPUT PARAMETERS:
    F0  -   function's value in X-TestStep point;
    DF0 -   derivative's value in X-TestStep point;
    F1  -   function's value in X+TestStep point;
    DF1 -   derivative's value in X+TestStep point;
    F   -   testing function's value;
    DF  -   testing derivative's value;
   Width-   width of verification segment.

RESULT:
    If input derivatives is right then function returns true, else 
    function returns false.
    
  -- ALGLIB --
     Copyright 29.05.2012 by Bochkanov Sergey
*************************************************************************/
ae_bool derivativecheck(double f0,
     double df0,
     double f1,
     double df1,
     double f,
     double df,
     double width,
     ae_state *_state)
{
    double s;
    double h;
    double dh;
    ae_bool result;


    df = width*df;
    df0 = width*df0;
    df1 = width*df1;
    s = ae_maxreal(ae_maxreal(ae_fabs(df0, _state), ae_fabs(df1, _state), _state), ae_fabs(f1-f0, _state), _state);
    h = 0.5*f0+0.125*df0+0.5*f1-0.125*df1;
    dh = -1.5*f0-0.25*df0+1.5*f1-0.25*df1;
    if( ae_fp_neq(s,(double)(0)) )
    {
        if( ae_fp_greater(ae_fabs(h-f, _state)/s,0.001)||ae_fp_greater(ae_fabs(dh-df, _state)/s,0.001) )
        {
            result = ae_false;
            return result;
        }
    }
    else
    {
        if( ae_fp_neq(h-f,0.0)||ae_fp_neq(dh-df,0.0) )
        {
            result = ae_false;
            return result;
        }
    }
    result = ae_true;
    return result;
}


/*************************************************************************
Having quadratic target function

    f(x) = 0.5*x'*A*x + b'*x + penaltyfactor*0.5*(C*x-b)'*(C*x-b)
    
and its parabolic model along direction D

    F(x0+alpha*D) = D2*alpha^2 + D1*alpha
    
this function estimates numerical errors in the coefficients of the model.
    
It is important that this  function  does  NOT calculate D1/D2  -  it only
estimates numerical errors introduced during evaluation and compares their
magnitudes against magnitudes of numerical errors. As result, one of three
outcomes is returned for each coefficient:
    * "true" coefficient is almost surely positive
    * "true" coefficient is almost surely negative
    * numerical errors in coefficient are so large that it can not be
      reliably distinguished from zero

INPUT PARAMETERS:
    AbsASum -   SUM(|A[i,j]|)
    AbsASum2-   SUM(A[i,j]^2)
    MB      -   max(|B|)
    MX      -   max(|X|)
    MD      -   max(|D|)
    D1      -   linear coefficient
    D2      -   quadratic coefficient

OUTPUT PARAMETERS:
    D1Est   -   estimate of D1 sign,  accounting  for  possible  numerical
                errors:
                * >0    means "almost surely positive" (D1>0 and large)
                * <0    means "almost surely negative" (D1<0 and large)
                * =0    means "pessimistic estimate  of  numerical  errors
                        in D1 is larger than magnitude of D1 itself; it is
                        impossible to reliably distinguish D1 from zero".
    D2Est   -   estimate of D2 sign,  accounting  for  possible  numerical
                errors:
                * >0    means "almost surely positive" (D2>0 and large)
                * <0    means "almost surely negative" (D2<0 and large)
                * =0    means "pessimistic estimate  of  numerical  errors
                        in D2 is larger than magnitude of D2 itself; it is
                        impossible to reliably distinguish D2 from zero".
            
  -- ALGLIB --
     Copyright 14.05.2014 by Bochkanov Sergey
*************************************************************************/
void estimateparabolicmodel(double absasum,
     double absasum2,
     double mx,
     double mb,
     double md,
     double d1,
     double d2,
     ae_int_t* d1est,
     ae_int_t* d2est,
     ae_state *_state)
{
    double d1esterror;
    double d2esterror;
    double eps;
    double e1;
    double e2;

    *d1est = 0;
    *d2est = 0;

    
    /*
     * Error estimates:
     *
     * * error in D1=d'*(A*x+b) is estimated as
     *   ED1 = eps*MAX_ABS(D)*(MAX_ABS(X)*ENORM(A)+MAX_ABS(B))
     * * error in D2=0.5*d'*A*d is estimated as
     *   ED2 = eps*MAX_ABS(D)^2*ENORM(A)
     *
     * Here ENORM(A) is some pseudo-norm which reflects the way numerical
     * error accumulates during addition. Two ways of accumulation are
     * possible - worst case (errors always increase) and mean-case (errors
     * may cancel each other). We calculate geometrical average of both:
     * * ENORM_WORST(A) = SUM(|A[i,j]|)         error in N-term sum grows as O(N)
     * * ENORM_MEAN(A)  = SQRT(SUM(A[i,j]^2))   error in N-term sum grows as O(sqrt(N))
     * * ENORM(A)       = SQRT(ENORM_WORST(A),ENORM_MEAN(A))
     */
    eps = 4*ae_machineepsilon;
    e1 = eps*md*(mx*absasum+mb);
    e2 = eps*md*(mx*ae_sqrt(absasum2, _state)+mb);
    d1esterror = ae_sqrt(e1*e2, _state);
    if( ae_fp_less_eq(ae_fabs(d1, _state),d1esterror) )
    {
        *d1est = 0;
    }
    else
    {
        *d1est = ae_sign(d1, _state);
    }
    e1 = eps*md*md*absasum;
    e2 = eps*md*md*ae_sqrt(absasum2, _state);
    d2esterror = ae_sqrt(e1*e2, _state);
    if( ae_fp_less_eq(ae_fabs(d2, _state),d2esterror) )
    {
        *d2est = 0;
    }
    else
    {
        *d2est = ae_sign(d2, _state);
    }
}


/*************************************************************************
This function calculates inexact rank-K preconditioner for Hessian  matrix
H=D+W'*C*W, where:
* H is a Hessian matrix, which is approximated by D/W/C
* D is a diagonal matrix with positive entries
* W is a rank-K correction
* C is a diagonal factor of rank-K correction

This preconditioner is inexact but fast - it requires O(N*K)  time  to  be
applied. Its main purpose - to be  used  in  barrier/penalty/AUL  methods,
where ill-conditioning is created by combination of two factors:
* simple bounds on variables => ill-conditioned D
* general barrier/penalty => correction W  with large coefficient C (makes
  problem ill-conditioned) but W itself is well conditioned.

Preconditioner P is calculated by artificially constructing a set of  BFGS
updates which tries to reproduce behavior of H:
* Sk = Wk (k-th row of W)
* Yk = (D+Wk'*Ck*Wk)*Sk
* Yk/Sk are reordered by ascending of C[k]*norm(Wk)^2

Here we assume that rows of Wk are orthogonal or nearly orthogonal,  which
allows us to have O(N*K+K^2) update instead of O(N*K^2) one. Reordering of
updates is essential for having good performance on non-orthogonal problems
(updates which do not add much of curvature are added first,  and  updates
which add very large eigenvalues are added last and override effect of the
first updates).

On input this function takes direction S and components of H.
On output it returns inv(H)*S

  -- ALGLIB --
     Copyright 30.06.2014 by Bochkanov Sergey
*************************************************************************/
void inexactlbfgspreconditioner(/* Real    */ ae_vector* s,
     ae_int_t n,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_matrix* w,
     ae_int_t k,
     precbuflbfgs* buf,
     ae_state *_state)
{
    ae_int_t idx;
    ae_int_t i;
    ae_int_t j;
    double v;
    double v0;
    double v1;
    double vx;
    double vy;


    rvectorsetlengthatleast(&buf->norms, k, _state);
    rvectorsetlengthatleast(&buf->alpha, k, _state);
    rvectorsetlengthatleast(&buf->rho, k, _state);
    rmatrixsetlengthatleast(&buf->yk, k, n, _state);
    ivectorsetlengthatleast(&buf->idx, k, _state);
    
    /*
     * Check inputs
     */
    for(i=0; i<=n-1; i++)
    {
        ae_assert(ae_fp_greater(d->ptr.p_double[i],(double)(0)), "InexactLBFGSPreconditioner: D[]<=0", _state);
    }
    for(i=0; i<=k-1; i++)
    {
        ae_assert(ae_fp_greater_eq(c->ptr.p_double[i],(double)(0)), "InexactLBFGSPreconditioner: C[]<0", _state);
    }
    
    /*
     * Reorder linear terms according to increase of second derivative.
     * Fill Norms[] array.
     */
    for(idx=0; idx<=k-1; idx++)
    {
        v = ae_v_dotproduct(&w->ptr.pp_double[idx][0], 1, &w->ptr.pp_double[idx][0], 1, ae_v_len(0,n-1));
        buf->norms.ptr.p_double[idx] = v*c->ptr.p_double[idx];
        buf->idx.ptr.p_int[idx] = idx;
    }
    tagsortfasti(&buf->norms, &buf->idx, &buf->bufa, &buf->bufb, k, _state);
    
    /*
     * Apply updates
     */
    for(idx=0; idx<=k-1; idx++)
    {
        
        /*
         * Select update to perform (ordered by ascending of second derivative)
         */
        i = buf->idx.ptr.p_int[idx];
        
        /*
         * Calculate YK and Rho
         */
        v = ae_v_dotproduct(&w->ptr.pp_double[i][0], 1, &w->ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
        v = v*c->ptr.p_double[i];
        for(j=0; j<=n-1; j++)
        {
            buf->yk.ptr.pp_double[i][j] = (d->ptr.p_double[j]+v)*w->ptr.pp_double[i][j];
        }
        v = 0.0;
        v0 = 0.0;
        v1 = 0.0;
        for(j=0; j<=n-1; j++)
        {
            vx = w->ptr.pp_double[i][j];
            vy = buf->yk.ptr.pp_double[i][j];
            v = v+vx*vy;
            v0 = v0+vx*vx;
            v1 = v1+vy*vy;
        }
        if( (ae_fp_greater(v,(double)(0))&&ae_fp_greater(v0*v1,(double)(0)))&&ae_fp_greater(v/ae_sqrt(v0*v1, _state),n*10*ae_machineepsilon) )
        {
            buf->rho.ptr.p_double[i] = 1/v;
        }
        else
        {
            buf->rho.ptr.p_double[i] = 0.0;
        }
    }
    for(idx=k-1; idx>=0; idx--)
    {
        
        /*
         * Select update to perform (ordered by ascending of second derivative)
         */
        i = buf->idx.ptr.p_int[idx];
        
        /*
         * Calculate Alpha[] according to L-BFGS algorithm
         * and update S[]
         */
        v = ae_v_dotproduct(&w->ptr.pp_double[i][0], 1, &s->ptr.p_double[0], 1, ae_v_len(0,n-1));
        v = buf->rho.ptr.p_double[i]*v;
        buf->alpha.ptr.p_double[i] = v;
        ae_v_subd(&s->ptr.p_double[0], 1, &buf->yk.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
    }
    for(j=0; j<=n-1; j++)
    {
        s->ptr.p_double[j] = s->ptr.p_double[j]/d->ptr.p_double[j];
    }
    for(idx=0; idx<=k-1; idx++)
    {
        
        /*
         * Select update to perform (ordered by ascending of second derivative)
         */
        i = buf->idx.ptr.p_int[idx];
        
        /*
         * Calculate Beta according to L-BFGS algorithm
         * and update S[]
         */
        v = ae_v_dotproduct(&buf->yk.ptr.pp_double[i][0], 1, &s->ptr.p_double[0], 1, ae_v_len(0,n-1));
        v = buf->alpha.ptr.p_double[i]-buf->rho.ptr.p_double[i]*v;
        ae_v_addd(&s->ptr.p_double[0], 1, &w->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
    }
}


/*************************************************************************
This function prepares exact low-rank preconditioner  for  Hessian  matrix
H=D+W'*C*W, where:
* H is a Hessian matrix, which is approximated by D/W/C
* D is a diagonal matrix with positive entries
* W is a rank-K correction
* C is a diagonal factor of rank-K correction, positive semidefinite

This preconditioner is exact but relatively slow -  it  requires  O(N*K^2)
time to be prepared and O(N*K) time to be applied. It is  calculated  with
the help of Woodbury matrix identity.

It should be used as follows:
* PrepareLowRankPreconditioner() call PREPARES data structure
* subsequent calls to ApplyLowRankPreconditioner() APPLY preconditioner to
  user-specified search direction.

  -- ALGLIB --
     Copyright 30.06.2014 by Bochkanov Sergey
*************************************************************************/
void preparelowrankpreconditioner(/* Real    */ ae_vector* d,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_matrix* w,
     ae_int_t n,
     ae_int_t k,
     precbuflowrank* buf,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_bool b;


    
    /*
     * Check inputs
     */
    ae_assert(n>0, "PrepareLowRankPreconditioner: N<=0", _state);
    ae_assert(k>=0, "PrepareLowRankPreconditioner: N<=0", _state);
    for(i=0; i<=n-1; i++)
    {
        ae_assert(ae_fp_greater(d->ptr.p_double[i],(double)(0)), "PrepareLowRankPreconditioner: D[]<=0", _state);
    }
    for(i=0; i<=k-1; i++)
    {
        ae_assert(ae_fp_greater_eq(c->ptr.p_double[i],(double)(0)), "PrepareLowRankPreconditioner: C[]<0", _state);
    }
    
    /*
     * Prepare buffer structure; skip zero entries of update.
     */
    rvectorsetlengthatleast(&buf->d, n, _state);
    rmatrixsetlengthatleast(&buf->v, k, n, _state);
    rvectorsetlengthatleast(&buf->bufc, k, _state);
    rmatrixsetlengthatleast(&buf->bufw, k+1, n, _state);
    buf->n = n;
    buf->k = 0;
    for(i=0; i<=k-1; i++)
    {
        
        /*
         * Estimate magnitude of update row; skip zero rows (either W or C are zero)
         */
        v = 0.0;
        for(j=0; j<=n-1; j++)
        {
            v = v+w->ptr.pp_double[i][j]*w->ptr.pp_double[i][j];
        }
        v = v*c->ptr.p_double[i];
        if( ae_fp_eq(v,(double)(0)) )
        {
            continue;
        }
        ae_assert(ae_fp_greater(v,(double)(0)), "PrepareLowRankPreconditioner: internal error", _state);
        
        /*
         * Copy non-zero update to buffer
         */
        buf->bufc.ptr.p_double[buf->k] = c->ptr.p_double[i];
        for(j=0; j<=n-1; j++)
        {
            buf->v.ptr.pp_double[buf->k][j] = w->ptr.pp_double[i][j];
            buf->bufw.ptr.pp_double[buf->k][j] = w->ptr.pp_double[i][j];
        }
        inc(&buf->k, _state);
    }
    
    /*
     * Reset K (for convenience)
     */
    k = buf->k;
    
    /*
     * Prepare diagonal factor; quick exit for K=0
     */
    for(i=0; i<=n-1; i++)
    {
        buf->d.ptr.p_double[i] = 1/d->ptr.p_double[i];
    }
    if( k==0 )
    {
        return;
    }
    
    /*
     * Use Woodbury matrix identity
     */
    rmatrixsetlengthatleast(&buf->bufz, k, k, _state);
    for(i=0; i<=k-1; i++)
    {
        for(j=0; j<=k-1; j++)
        {
            buf->bufz.ptr.pp_double[i][j] = 0.0;
        }
    }
    for(i=0; i<=k-1; i++)
    {
        buf->bufz.ptr.pp_double[i][i] = 1/buf->bufc.ptr.p_double[i];
    }
    for(j=0; j<=n-1; j++)
    {
        buf->bufw.ptr.pp_double[k][j] = 1/ae_sqrt(d->ptr.p_double[j], _state);
    }
    for(i=0; i<=k-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            buf->bufw.ptr.pp_double[i][j] = buf->bufw.ptr.pp_double[i][j]*buf->bufw.ptr.pp_double[k][j];
        }
    }
    rmatrixgemm(k, k, n, 1.0, &buf->bufw, 0, 0, 0, &buf->bufw, 0, 0, 1, 1.0, &buf->bufz, 0, 0, _state);
    b = spdmatrixcholeskyrec(&buf->bufz, 0, k, ae_true, &buf->tmp, _state);
    ae_assert(b, "PrepareLowRankPreconditioner: internal error (Cholesky failure)", _state);
    rmatrixlefttrsm(k, n, &buf->bufz, 0, 0, ae_true, ae_false, 1, &buf->v, 0, 0, _state);
    for(i=0; i<=k-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            buf->v.ptr.pp_double[i][j] = buf->v.ptr.pp_double[i][j]*buf->d.ptr.p_double[j];
        }
    }
}


/*************************************************************************
This function apply exact low-rank preconditioner prepared by
PrepareLowRankPreconditioner function (see its comments for more information).

  -- ALGLIB --
     Copyright 30.06.2014 by Bochkanov Sergey
*************************************************************************/
void applylowrankpreconditioner(/* Real    */ ae_vector* s,
     precbuflowrank* buf,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t k;
    ae_int_t i;
    ae_int_t j;
    double v;


    n = buf->n;
    k = buf->k;
    rvectorsetlengthatleast(&buf->tmp, n, _state);
    for(j=0; j<=n-1; j++)
    {
        buf->tmp.ptr.p_double[j] = buf->d.ptr.p_double[j]*s->ptr.p_double[j];
    }
    for(i=0; i<=k-1; i++)
    {
        v = 0.0;
        for(j=0; j<=n-1; j++)
        {
            v = v+buf->v.ptr.pp_double[i][j]*s->ptr.p_double[j];
        }
        for(j=0; j<=n-1; j++)
        {
            buf->tmp.ptr.p_double[j] = buf->tmp.ptr.p_double[j]-v*buf->v.ptr.pp_double[i][j];
        }
    }
    for(i=0; i<=n-1; i++)
    {
        s->ptr.p_double[i] = buf->tmp.ptr.p_double[i];
    }
}


void _precbuflbfgs_init(void* _p, ae_state *_state)
{
    precbuflbfgs *p = (precbuflbfgs*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->norms, 0, DT_REAL, _state);
    ae_vector_init(&p->alpha, 0, DT_REAL, _state);
    ae_vector_init(&p->rho, 0, DT_REAL, _state);
    ae_matrix_init(&p->yk, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->idx, 0, DT_INT, _state);
    ae_vector_init(&p->bufa, 0, DT_REAL, _state);
    ae_vector_init(&p->bufb, 0, DT_INT, _state);
}


void _precbuflbfgs_init_copy(void* _dst, void* _src, ae_state *_state)
{
    precbuflbfgs *dst = (precbuflbfgs*)_dst;
    precbuflbfgs *src = (precbuflbfgs*)_src;
    ae_vector_init_copy(&dst->norms, &src->norms, _state);
    ae_vector_init_copy(&dst->alpha, &src->alpha, _state);
    ae_vector_init_copy(&dst->rho, &src->rho, _state);
    ae_matrix_init_copy(&dst->yk, &src->yk, _state);
    ae_vector_init_copy(&dst->idx, &src->idx, _state);
    ae_vector_init_copy(&dst->bufa, &src->bufa, _state);
    ae_vector_init_copy(&dst->bufb, &src->bufb, _state);
}


void _precbuflbfgs_clear(void* _p)
{
    precbuflbfgs *p = (precbuflbfgs*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->norms);
    ae_vector_clear(&p->alpha);
    ae_vector_clear(&p->rho);
    ae_matrix_clear(&p->yk);
    ae_vector_clear(&p->idx);
    ae_vector_clear(&p->bufa);
    ae_vector_clear(&p->bufb);
}


void _precbuflbfgs_destroy(void* _p)
{
    precbuflbfgs *p = (precbuflbfgs*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->norms);
    ae_vector_destroy(&p->alpha);
    ae_vector_destroy(&p->rho);
    ae_matrix_destroy(&p->yk);
    ae_vector_destroy(&p->idx);
    ae_vector_destroy(&p->bufa);
    ae_vector_destroy(&p->bufb);
}


void _precbuflowrank_init(void* _p, ae_state *_state)
{
    precbuflowrank *p = (precbuflowrank*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->d, 0, DT_REAL, _state);
    ae_matrix_init(&p->v, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->bufc, 0, DT_REAL, _state);
    ae_matrix_init(&p->bufz, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->bufw, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp, 0, DT_REAL, _state);
}


void _precbuflowrank_init_copy(void* _dst, void* _src, ae_state *_state)
{
    precbuflowrank *dst = (precbuflowrank*)_dst;
    precbuflowrank *src = (precbuflowrank*)_src;
    dst->n = src->n;
    dst->k = src->k;
    ae_vector_init_copy(&dst->d, &src->d, _state);
    ae_matrix_init_copy(&dst->v, &src->v, _state);
    ae_vector_init_copy(&dst->bufc, &src->bufc, _state);
    ae_matrix_init_copy(&dst->bufz, &src->bufz, _state);
    ae_matrix_init_copy(&dst->bufw, &src->bufw, _state);
    ae_vector_init_copy(&dst->tmp, &src->tmp, _state);
}


void _precbuflowrank_clear(void* _p)
{
    precbuflowrank *p = (precbuflowrank*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->d);
    ae_matrix_clear(&p->v);
    ae_vector_clear(&p->bufc);
    ae_matrix_clear(&p->bufz);
    ae_matrix_clear(&p->bufw);
    ae_vector_clear(&p->tmp);
}


void _precbuflowrank_destroy(void* _p)
{
    precbuflowrank *p = (precbuflowrank*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->d);
    ae_matrix_destroy(&p->v);
    ae_vector_destroy(&p->bufc);
    ae_matrix_destroy(&p->bufz);
    ae_matrix_destroy(&p->bufw);
    ae_vector_destroy(&p->tmp);
}


/*$ End $*/
