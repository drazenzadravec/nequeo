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
#include "qpcholeskysolver.h"


/*$ Declarations $*/
static ae_int_t qpcholeskysolver_maxlagrangeits = 10;
static ae_int_t qpcholeskysolver_maxbadnewtonits = 7;
static double qpcholeskysolver_penaltyfactor = 100.0;
static double qpcholeskysolver_modelvalue(convexquadraticmodel* a,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* xc,
     ae_int_t n,
     /* Real    */ ae_vector* tmp,
     ae_state *_state);
static ae_int_t qpcholeskysolver_boundedstepandactivation(sactiveset* sas,
     /* Real    */ ae_vector* xn,
     ae_int_t n,
     /* Real    */ ae_vector* buf,
     ae_state *_state);
static ae_bool qpcholeskysolver_constrainedoptimum(sactiveset* sas,
     convexquadraticmodel* a,
     double anorm,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* xn,
     ae_int_t n,
     /* Real    */ ae_vector* tmp,
     /* Boolean */ ae_vector* tmpb,
     /* Real    */ ae_vector* lagrangec,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This function initializes QPCholeskySettings structure with default settings.

Newly created structure MUST be initialized by default settings  -  or  by
copy of the already initialized structure.

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qpcholeskyloaddefaults(ae_int_t nmain,
     qpcholeskysettings* s,
     ae_state *_state)
{


    s->epsg = 0.0;
    s->epsf = 0.0;
    s->epsx = 1.0E-6;
    s->maxits = 0;
}


/*************************************************************************
This function initializes QPCholeskySettings  structure  with  copy  of  another,
already initialized structure.

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qpcholeskycopysettings(qpcholeskysettings* src,
     qpcholeskysettings* dst,
     ae_state *_state)
{


    dst->epsg = src->epsg;
    dst->epsf = src->epsf;
    dst->epsx = src->epsx;
    dst->maxits = src->maxits;
}


/*************************************************************************
This function runs QPCholesky solver; it returns after optimization   process
was completed. Following QP problem is solved:

    min(0.5*(x-x_origin)'*A*(x-x_origin)+b'*(x-x_origin))
    
subject to boundary constraints.

INPUT PARAMETERS:
    AC          -   for dense problems (AKind=0) contains system matrix in
                    the A-term of CQM object.  OTHER  TERMS  ARE  ACTIVELY
                    USED AND MODIFIED BY THE SOLVER!
    SparseAC    -   for sparse problems (AKind=1
    AKind       -   sparse matrix format:
                    * 0 for dense matrix
                    * 1 for sparse matrix
    SparseUpper -   which triangle of SparseAC stores matrix  -  upper  or
                    lower one (for dense matrices this  parameter  is  not
                    actual).
    BC          -   linear term, array[NC]
    BndLC       -   lower bound, array[NC]
    BndUC       -   upper bound, array[NC]
    SC          -   scale vector, array[NC]:
                    * I-th element contains scale of I-th variable,
                    * SC[I]>0
    XOriginC    -   origin term, array[NC]. Can be zero.
    NC          -   number of variables in the  original  formulation  (no
                    slack variables).
    CLEICC      -   linear equality/inequality constraints. Present version
                    of this function does NOT provide  publicly  available
                    support for linear constraints. This feature  will  be
                    introduced in the future versions of the function.
    NEC, NIC    -   number of equality/inequality constraints.
                    MUST BE ZERO IN THE CURRENT VERSION!!!
    Settings    -   QPCholeskySettings object initialized by one of the initialization
                    functions.
    SState      -   object which stores temporaries:
                    * if uninitialized object was passed, FirstCall parameter MUST
                      be set to True; object will be automatically initialized by the
                      function, and FirstCall will be set to False.
                    * if FirstCall=False, it is assumed that this parameter was already
                      initialized by previous call to this function with same
                      problem dimensions (variable count N).
    XS          -   initial point, array[NC]
    
    
OUTPUT PARAMETERS:
    XS          -   last point
    TerminationType-termination type:
                    *
                    *
                    *

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qpcholeskyoptimize(convexquadraticmodel* a,
     double anorm,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     /* Real    */ ae_vector* s,
     /* Real    */ ae_vector* xorigin,
     ae_int_t n,
     /* Real    */ ae_matrix* cleic,
     ae_int_t nec,
     ae_int_t nic,
     qpcholeskybuffers* sstate,
     /* Real    */ ae_vector* xsc,
     ae_int_t* terminationtype,
     ae_state *_state)
{
    ae_int_t i;
    double noisetolerance;
    ae_bool havebc;
    double v;
    ae_int_t badnewtonits;
    double maxscaledgrad;
    double v0;
    double v1;
    ae_int_t nextaction;
    double fprev;
    double fcur;
    double fcand;
    double noiselevel;
    double d0;
    double d1;
    double d2;
    ae_int_t actstatus;

    *terminationtype = 0;

    
    /*
     * Allocate storage and prepare fields
     */
    rvectorsetlengthatleast(&sstate->rctmpg, n, _state);
    rvectorsetlengthatleast(&sstate->tmp0, n, _state);
    rvectorsetlengthatleast(&sstate->tmp1, n, _state);
    rvectorsetlengthatleast(&sstate->gc, n, _state);
    rvectorsetlengthatleast(&sstate->pg, n, _state);
    rvectorsetlengthatleast(&sstate->xs, n, _state);
    rvectorsetlengthatleast(&sstate->xn, n, _state);
    rvectorsetlengthatleast(&sstate->workbndl, n, _state);
    rvectorsetlengthatleast(&sstate->workbndu, n, _state);
    bvectorsetlengthatleast(&sstate->havebndl, n, _state);
    bvectorsetlengthatleast(&sstate->havebndu, n, _state);
    sstate->repinneriterationscount = 0;
    sstate->repouteriterationscount = 0;
    sstate->repncholesky = 0;
    noisetolerance = (double)(10);
    
    /*
     * Our formulation of quadratic problem includes origin point,
     * i.e. we have F(x-x_origin) which is minimized subject to
     * constraints on x, instead of having simply F(x).
     *
     * Here we make transition from non-zero origin to zero one.
     * In order to make such transition we have to:
     * 1. subtract x_origin from x_start
     * 2. modify constraints
     * 3. solve problem
     * 4. add x_origin to solution
     *
     * There is alternate solution - to modify quadratic function
     * by expansion of multipliers containing (x-x_origin), but
     * we prefer to modify constraints, because it is a) more precise
     * and b) easier to to.
     *
     * Parts (1)-(2) are done here. After this block is over,
     * we have:
     * * XS, which stores shifted XStart (if we don't have XStart,
     *   value of XS will be ignored later)
     * * WorkBndL, WorkBndU, which store modified boundary constraints.
     */
    havebc = ae_false;
    for(i=0; i<=n-1; i++)
    {
        sstate->havebndl.ptr.p_bool[i] = ae_isfinite(bndl->ptr.p_double[i], _state);
        sstate->havebndu.ptr.p_bool[i] = ae_isfinite(bndu->ptr.p_double[i], _state);
        havebc = (havebc||sstate->havebndl.ptr.p_bool[i])||sstate->havebndu.ptr.p_bool[i];
        if( sstate->havebndl.ptr.p_bool[i] )
        {
            sstate->workbndl.ptr.p_double[i] = bndl->ptr.p_double[i]-xorigin->ptr.p_double[i];
        }
        else
        {
            sstate->workbndl.ptr.p_double[i] = _state->v_neginf;
        }
        if( sstate->havebndu.ptr.p_bool[i] )
        {
            sstate->workbndu.ptr.p_double[i] = bndu->ptr.p_double[i]-xorigin->ptr.p_double[i];
        }
        else
        {
            sstate->workbndu.ptr.p_double[i] = _state->v_posinf;
        }
    }
    rmatrixsetlengthatleast(&sstate->workcleic, nec+nic, n+1, _state);
    for(i=0; i<=nec+nic-1; i++)
    {
        v = ae_v_dotproduct(&cleic->ptr.pp_double[i][0], 1, &xorigin->ptr.p_double[0], 1, ae_v_len(0,n-1));
        ae_v_move(&sstate->workcleic.ptr.pp_double[i][0], 1, &cleic->ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
        sstate->workcleic.ptr.pp_double[i][n] = cleic->ptr.pp_double[i][n]-v;
    }
    
    /*
     * We have starting point in StartX, so we just have to shift and bound it
     */
    for(i=0; i<=n-1; i++)
    {
        sstate->xs.ptr.p_double[i] = xsc->ptr.p_double[i]-xorigin->ptr.p_double[i];
        if( sstate->havebndl.ptr.p_bool[i] )
        {
            if( ae_fp_less(sstate->xs.ptr.p_double[i],sstate->workbndl.ptr.p_double[i]) )
            {
                sstate->xs.ptr.p_double[i] = sstate->workbndl.ptr.p_double[i];
            }
        }
        if( sstate->havebndu.ptr.p_bool[i] )
        {
            if( ae_fp_greater(sstate->xs.ptr.p_double[i],sstate->workbndu.ptr.p_double[i]) )
            {
                sstate->xs.ptr.p_double[i] = sstate->workbndu.ptr.p_double[i];
            }
        }
    }
    
    /*
     * Handle special case - no constraints
     */
    if( !havebc&&nec+nic==0 )
    {
        
        /*
         * "Simple" unconstrained Cholesky
         */
        bvectorsetlengthatleast(&sstate->tmpb, n, _state);
        for(i=0; i<=n-1; i++)
        {
            sstate->tmpb.ptr.p_bool[i] = ae_false;
        }
        sstate->repncholesky = sstate->repncholesky+1;
        cqmsetb(a, b, _state);
        cqmsetactiveset(a, &sstate->xs, &sstate->tmpb, _state);
        if( !cqmconstrainedoptimum(a, &sstate->xn, _state) )
        {
            *terminationtype = -5;
            return;
        }
        ae_v_move(&xsc->ptr.p_double[0], 1, &sstate->xn.ptr.p_double[0], 1, ae_v_len(0,n-1));
        ae_v_add(&xsc->ptr.p_double[0], 1, &xorigin->ptr.p_double[0], 1, ae_v_len(0,n-1));
        sstate->repinneriterationscount = 1;
        sstate->repouteriterationscount = 1;
        *terminationtype = 4;
        return;
    }
    
    /*
     * Prepare "active set" structure
     */
    sasinit(n, &sstate->sas, _state);
    sassetbc(&sstate->sas, &sstate->workbndl, &sstate->workbndu, _state);
    sassetlcx(&sstate->sas, &sstate->workcleic, nec, nic, _state);
    sassetscale(&sstate->sas, s, _state);
    if( !sasstartoptimization(&sstate->sas, &sstate->xs, _state) )
    {
        *terminationtype = -3;
        return;
    }
    
    /*
     * Main cycle of CQP algorithm
     */
    *terminationtype = 4;
    badnewtonits = 0;
    maxscaledgrad = 0.0;
    for(;;)
    {
        
        /*
         * Update iterations count
         */
        inc(&sstate->repouteriterationscount, _state);
        inc(&sstate->repinneriterationscount, _state);
        
        /*
         * Phase 1.
         *
         * Determine active set.
         * Update MaxScaledGrad.
         */
        cqmadx(a, &sstate->sas.xc, &sstate->rctmpg, _state);
        ae_v_add(&sstate->rctmpg.ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
        sasreactivateconstraints(&sstate->sas, &sstate->rctmpg, _state);
        v = 0.0;
        for(i=0; i<=n-1; i++)
        {
            v = v+ae_sqr(sstate->rctmpg.ptr.p_double[i]*s->ptr.p_double[i], _state);
        }
        maxscaledgrad = ae_maxreal(maxscaledgrad, ae_sqrt(v, _state), _state);
        
        /*
         * Phase 2: perform penalized steepest descent step.
         *
         * NextAction control variable is set on exit from this loop:
         * * NextAction>0 in case we have to proceed to Phase 3 (Newton step)
         * * NextAction<0 in case we have to proceed to Phase 1 (recalculate active set)
         * * NextAction=0 in case we found solution (step along projected gradient is small enough)
         */
        for(;;)
        {
            
            /*
             * Calculate constrained descent direction, store to PG.
             * Successful termination if PG is zero.
             */
            cqmadx(a, &sstate->sas.xc, &sstate->gc, _state);
            ae_v_add(&sstate->gc.ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
            sasconstraineddescent(&sstate->sas, &sstate->gc, &sstate->pg, _state);
            v0 = ae_v_dotproduct(&sstate->pg.ptr.p_double[0], 1, &sstate->pg.ptr.p_double[0], 1, ae_v_len(0,n-1));
            if( ae_fp_eq(v0,(double)(0)) )
            {
                
                /*
                 * Constrained derivative is zero.
                 * Solution found.
                 */
                nextaction = 0;
                break;
            }
            
            /*
             * Build quadratic model of F along descent direction:
             *     F(xc+alpha*pg) = D2*alpha^2 + D1*alpha + D0
             * Store noise level in the XC (noise level is used to classify
             * step as singificant or insignificant).
             *
             * In case function curvature is negative or product of descent
             * direction and gradient is non-negative, iterations are terminated.
             *
             * NOTE: D0 is not actually used, but we prefer to maintain it.
             */
            fprev = qpcholeskysolver_modelvalue(a, b, &sstate->sas.xc, n, &sstate->tmp0, _state);
            fprev = fprev+qpcholeskysolver_penaltyfactor*maxscaledgrad*sasactivelcpenalty1(&sstate->sas, &sstate->sas.xc, _state);
            cqmevalx(a, &sstate->sas.xc, &v, &noiselevel, _state);
            v0 = cqmxtadx2(a, &sstate->pg, _state);
            d2 = v0;
            v1 = ae_v_dotproduct(&sstate->pg.ptr.p_double[0], 1, &sstate->gc.ptr.p_double[0], 1, ae_v_len(0,n-1));
            d1 = v1;
            d0 = fprev;
            if( ae_fp_less_eq(d2,(double)(0)) )
            {
                
                /*
                 * Second derivative is non-positive, function is non-convex.
                 */
                *terminationtype = -5;
                nextaction = 0;
                break;
            }
            if( ae_fp_greater_eq(d1,(double)(0)) )
            {
                
                /*
                 * Second derivative is positive, first derivative is non-negative.
                 * Solution found.
                 */
                nextaction = 0;
                break;
            }
            
            /*
             * Modify quadratic model - add penalty for violation of the active
             * constraints.
             *
             * Boundary constraints are always satisfied exactly, so we do not
             * add penalty term for them. General equality constraint of the
             * form a'*(xc+alpha*d)=b adds penalty term:
             *     P(alpha) = (a'*(xc+alpha*d)-b)^2
             *              = (alpha*(a'*d) + (a'*xc-b))^2
             *              = alpha^2*(a'*d)^2 + alpha*2*(a'*d)*(a'*xc-b) + (a'*xc-b)^2
             * Each penalty term is multiplied by 100*Anorm before adding it to
             * the 1-dimensional quadratic model.
             *
             * Penalization of the quadratic model improves behavior of the
             * algorithm in the presense of the multiple degenerate constraints.
             * In particular, it prevents algorithm from making large steps in
             * directions which violate equality constraints.
             */
            for(i=0; i<=nec+nic-1; i++)
            {
                if( sstate->sas.activeset.ptr.p_int[n+i]>0 )
                {
                    v0 = ae_v_dotproduct(&sstate->workcleic.ptr.pp_double[i][0], 1, &sstate->pg.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    v1 = ae_v_dotproduct(&sstate->workcleic.ptr.pp_double[i][0], 1, &sstate->sas.xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    v1 = v1-sstate->workcleic.ptr.pp_double[i][n];
                    v = 100*anorm;
                    d2 = d2+v*ae_sqr(v0, _state);
                    d1 = d1+v*2*v0*v1;
                    d0 = d0+v*ae_sqr(v1, _state);
                }
            }
            
            /*
             * Try unbounded step.
             * In case function change is dominated by noise or function actually increased
             * instead of decreasing, we terminate iterations.
             */
            v = -d1/(2*d2);
            ae_v_move(&sstate->xn.ptr.p_double[0], 1, &sstate->sas.xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
            ae_v_addd(&sstate->xn.ptr.p_double[0], 1, &sstate->pg.ptr.p_double[0], 1, ae_v_len(0,n-1), v);
            fcand = qpcholeskysolver_modelvalue(a, b, &sstate->xn, n, &sstate->tmp0, _state);
            fcand = fcand+qpcholeskysolver_penaltyfactor*maxscaledgrad*sasactivelcpenalty1(&sstate->sas, &sstate->xn, _state);
            if( ae_fp_greater_eq(fcand,fprev-noiselevel*noisetolerance) )
            {
                nextaction = 0;
                break;
            }
            
            /*
             * Save active set
             * Perform bounded step with (possible) activation
             */
            actstatus = qpcholeskysolver_boundedstepandactivation(&sstate->sas, &sstate->xn, n, &sstate->tmp0, _state);
            fcur = qpcholeskysolver_modelvalue(a, b, &sstate->sas.xc, n, &sstate->tmp0, _state);
            
            /*
             * Depending on results, decide what to do:
             * 1. In case step was performed without activation of constraints,
             *    we proceed to Newton method
             * 2. In case there was activated at least one constraint with ActiveSet[I]<0,
             *    we proceed to Phase 1 and re-evaluate active set.
             * 3. Otherwise (activation of the constraints with ActiveSet[I]=0)
             *    we try Phase 2 one more time.
             */
            if( actstatus<0 )
            {
                
                /*
                 * Step without activation, proceed to Newton
                 */
                nextaction = 1;
                break;
            }
            if( actstatus==0 )
            {
                
                /*
                 * No new constraints added during last activation - only
                 * ones which were at the boundary (ActiveSet[I]=0), but
                 * inactive due to numerical noise.
                 *
                 * Now, these constraints are added to the active set, and
                 * we try to perform steepest descent (Phase 2) one more time.
                 */
                continue;
            }
            else
            {
                
                /*
                 * Last step activated at least one significantly new
                 * constraint (ActiveSet[I]<0), we have to re-evaluate
                 * active set (Phase 1).
                 */
                nextaction = -1;
                break;
            }
        }
        if( nextaction<0 )
        {
            continue;
        }
        if( nextaction==0 )
        {
            break;
        }
        
        /*
         * Phase 3: fast equality-constrained solver
         *
         * NOTE: this solver uses Augmented Lagrangian algorithm to solve
         *       equality-constrained subproblems. This algorithm may
         *       perform steps which increase function values instead of
         *       decreasing it (in hard cases, like overconstrained problems).
         *
         *       Such non-monononic steps may create a loop, when Augmented
         *       Lagrangian algorithm performs uphill step, and steepest
         *       descent algorithm (Phase 2) performs downhill step in the
         *       opposite direction.
         *
         *       In order to prevent iterations to continue forever we
         *       count iterations when AL algorithm increased function
         *       value instead of decreasing it. When number of such "bad"
         *       iterations will increase beyong MaxBadNewtonIts, we will
         *       terminate algorithm.
         */
        fprev = qpcholeskysolver_modelvalue(a, b, &sstate->sas.xc, n, &sstate->tmp0, _state);
        for(;;)
        {
            
            /*
             * Calculate optimum subject to presently active constraints
             */
            sstate->repncholesky = sstate->repncholesky+1;
            if( !qpcholeskysolver_constrainedoptimum(&sstate->sas, a, anorm, b, &sstate->xn, n, &sstate->tmp0, &sstate->tmpb, &sstate->tmp1, _state) )
            {
                *terminationtype = -5;
                sasstopoptimization(&sstate->sas, _state);
                return;
            }
            
            /*
             * Add constraints.
             * If no constraints was added, accept candidate point XN and move to next phase.
             */
            if( qpcholeskysolver_boundedstepandactivation(&sstate->sas, &sstate->xn, n, &sstate->tmp0, _state)<0 )
            {
                break;
            }
        }
        fcur = qpcholeskysolver_modelvalue(a, b, &sstate->sas.xc, n, &sstate->tmp0, _state);
        if( ae_fp_greater_eq(fcur,fprev) )
        {
            badnewtonits = badnewtonits+1;
        }
        if( badnewtonits>=qpcholeskysolver_maxbadnewtonits )
        {
            
            /*
             * Algorithm found solution, but keeps iterating because Newton
             * algorithm performs uphill steps (noise in the Augmented Lagrangian
             * algorithm). We terminate algorithm; it is considered normal
             * termination.
             */
            break;
        }
    }
    sasstopoptimization(&sstate->sas, _state);
    
    /*
     * Post-process: add XOrigin to XC
     */
    for(i=0; i<=n-1; i++)
    {
        if( sstate->havebndl.ptr.p_bool[i]&&ae_fp_eq(sstate->sas.xc.ptr.p_double[i],sstate->workbndl.ptr.p_double[i]) )
        {
            xsc->ptr.p_double[i] = bndl->ptr.p_double[i];
            continue;
        }
        if( sstate->havebndu.ptr.p_bool[i]&&ae_fp_eq(sstate->sas.xc.ptr.p_double[i],sstate->workbndu.ptr.p_double[i]) )
        {
            xsc->ptr.p_double[i] = bndu->ptr.p_double[i];
            continue;
        }
        xsc->ptr.p_double[i] = boundval(sstate->sas.xc.ptr.p_double[i]+xorigin->ptr.p_double[i], bndl->ptr.p_double[i], bndu->ptr.p_double[i], _state);
    }
}


/*************************************************************************
Model value: f = 0.5*x'*A*x + b'*x

INPUT PARAMETERS:
    A       -   convex quadratic model; only main quadratic term is used,
                other parts of the model (D/Q/linear term) are ignored.
                This function does not modify model state.
    B       -   right part
    XC      -   evaluation point
    Tmp     -   temporary buffer, automatically resized if needed

  -- ALGLIB --
     Copyright 20.06.2012 by Bochkanov Sergey
*************************************************************************/
static double qpcholeskysolver_modelvalue(convexquadraticmodel* a,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* xc,
     ae_int_t n,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    double v0;
    double v1;
    double result;


    rvectorsetlengthatleast(tmp, n, _state);
    cqmadx(a, xc, tmp, _state);
    v0 = ae_v_dotproduct(&xc->ptr.p_double[0], 1, &tmp->ptr.p_double[0], 1, ae_v_len(0,n-1));
    v1 = ae_v_dotproduct(&xc->ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
    result = 0.5*v0+v1;
    return result;
}


/*************************************************************************
Having feasible current point XC and possibly infeasible candidate   point
XN,  this  function  performs  longest  step  from  XC to XN which retains
feasibility. In case XN is found to be infeasible, at least one constraint
is activated.

For example, if we have:
  XC=0.5
  XN=1.2
  x>=0, x<=1
then this function will move us to X=1.0 and activate constraint "x<=1".

INPUT PARAMETERS:
    State   -   MinQP state.
    XC      -   current point, must be feasible with respect to
                all constraints
    XN      -   candidate point, can be infeasible with respect to some
                constraints. Must be located in the subspace of current
                active set, i.e. it is feasible with respect to already
                active constraints.
    Buf     -   temporary buffer, automatically resized if needed

OUTPUT PARAMETERS:
    State   -   this function changes following fields of State:
                * State.ActiveSet
                * State.ActiveC     -   active linear constraints
    XC      -   new position

RESULT:
    >0, in case at least one inactive non-candidate constraint was activated
    =0, in case only "candidate" constraints were activated
    <0, in case no constraints were activated by the step


  -- ALGLIB --
     Copyright 29.02.2012 by Bochkanov Sergey
*************************************************************************/
static ae_int_t qpcholeskysolver_boundedstepandactivation(sactiveset* sas,
     /* Real    */ ae_vector* xn,
     ae_int_t n,
     /* Real    */ ae_vector* buf,
     ae_state *_state)
{
    double stpmax;
    ae_int_t cidx;
    double cval;
    ae_bool needact;
    double v;
    ae_int_t result;


    rvectorsetlengthatleast(buf, n, _state);
    ae_v_move(&buf->ptr.p_double[0], 1, &xn->ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_sub(&buf->ptr.p_double[0], 1, &sas->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
    sasexploredirection(sas, buf, &stpmax, &cidx, &cval, _state);
    needact = ae_fp_less_eq(stpmax,(double)(1));
    v = ae_minreal(stpmax, 1.0, _state);
    ae_v_muld(&buf->ptr.p_double[0], 1, ae_v_len(0,n-1), v);
    ae_v_add(&buf->ptr.p_double[0], 1, &sas->xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
    result = sasmoveto(sas, buf, needact, cidx, cval, _state);
    return result;
}


/*************************************************************************
Optimum of A subject to:
a) active boundary constraints (given by ActiveSet[] and corresponding
   elements of XC)
b) active linear constraints (given by C, R, LagrangeC)

INPUT PARAMETERS:
    A       -   main quadratic term of the model;
                although structure may  store  linear  and  rank-K  terms,
                these terms are ignored and rewritten  by  this  function.
    ANorm   -   estimate of ||A|| (2-norm is used)
    B       -   array[N], linear term of the model
    XN      -   possibly preallocated buffer
    Tmp     -   temporary buffer (automatically resized)
    Tmp1    -   temporary buffer (automatically resized)

OUTPUT PARAMETERS:
    A       -   modified quadratic model (this function changes rank-K
                term and linear term of the model)
    LagrangeC-  current estimate of the Lagrange coefficients
    XN      -   solution

RESULT:
    True on success, False on failure (non-SPD model)

  -- ALGLIB --
     Copyright 20.06.2012 by Bochkanov Sergey
*************************************************************************/
static ae_bool qpcholeskysolver_constrainedoptimum(sactiveset* sas,
     convexquadraticmodel* a,
     double anorm,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* xn,
     ae_int_t n,
     /* Real    */ ae_vector* tmp,
     /* Boolean */ ae_vector* tmpb,
     /* Real    */ ae_vector* lagrangec,
     ae_state *_state)
{
    ae_int_t itidx;
    ae_int_t i;
    double v;
    double feaserrold;
    double feaserrnew;
    double theta;
    ae_bool result;


    
    /*
     * Rebuild basis accroding to current active set.
     * We call SASRebuildBasis() to make sure that fields of SAS
     * store up to date values.
     */
    sasrebuildbasis(sas, _state);
    
    /*
     * Allocate temporaries.
     */
    rvectorsetlengthatleast(tmp, ae_maxint(n, sas->basissize, _state), _state);
    bvectorsetlengthatleast(tmpb, n, _state);
    rvectorsetlengthatleast(lagrangec, sas->basissize, _state);
    
    /*
     * Prepare model
     */
    for(i=0; i<=sas->basissize-1; i++)
    {
        tmp->ptr.p_double[i] = sas->pbasis.ptr.pp_double[i][n];
    }
    theta = 100.0*anorm;
    for(i=0; i<=n-1; i++)
    {
        if( sas->activeset.ptr.p_int[i]>0 )
        {
            tmpb->ptr.p_bool[i] = ae_true;
        }
        else
        {
            tmpb->ptr.p_bool[i] = ae_false;
        }
    }
    cqmsetactiveset(a, &sas->xc, tmpb, _state);
    cqmsetq(a, &sas->pbasis, tmp, sas->basissize, theta, _state);
    
    /*
     * Iterate until optimal values of Lagrange multipliers are found
     */
    for(i=0; i<=sas->basissize-1; i++)
    {
        lagrangec->ptr.p_double[i] = (double)(0);
    }
    feaserrnew = ae_maxrealnumber;
    result = ae_true;
    for(itidx=1; itidx<=qpcholeskysolver_maxlagrangeits; itidx++)
    {
        
        /*
         * Generate right part B using linear term and current
         * estimate of the Lagrange multipliers.
         */
        ae_v_move(&tmp->ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
        for(i=0; i<=sas->basissize-1; i++)
        {
            v = lagrangec->ptr.p_double[i];
            ae_v_subd(&tmp->ptr.p_double[0], 1, &sas->pbasis.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
        }
        cqmsetb(a, tmp, _state);
        
        /*
         * Solve
         */
        result = cqmconstrainedoptimum(a, xn, _state);
        if( !result )
        {
            return result;
        }
        
        /*
         * Compare feasibility errors.
         * Terminate if error decreased too slowly.
         */
        feaserrold = feaserrnew;
        feaserrnew = (double)(0);
        for(i=0; i<=sas->basissize-1; i++)
        {
            v = ae_v_dotproduct(&sas->pbasis.ptr.pp_double[i][0], 1, &xn->ptr.p_double[0], 1, ae_v_len(0,n-1));
            feaserrnew = feaserrnew+ae_sqr(v-sas->pbasis.ptr.pp_double[i][n], _state);
        }
        feaserrnew = ae_sqrt(feaserrnew, _state);
        if( ae_fp_greater_eq(feaserrnew,0.2*feaserrold) )
        {
            break;
        }
        
        /*
         * Update Lagrange multipliers
         */
        for(i=0; i<=sas->basissize-1; i++)
        {
            v = ae_v_dotproduct(&sas->pbasis.ptr.pp_double[i][0], 1, &xn->ptr.p_double[0], 1, ae_v_len(0,n-1));
            lagrangec->ptr.p_double[i] = lagrangec->ptr.p_double[i]-theta*(v-sas->pbasis.ptr.pp_double[i][n]);
        }
    }
    return result;
}


void _qpcholeskysettings_init(void* _p, ae_state *_state)
{
    qpcholeskysettings *p = (qpcholeskysettings*)_p;
    ae_touch_ptr((void*)p);
}


void _qpcholeskysettings_init_copy(void* _dst, void* _src, ae_state *_state)
{
    qpcholeskysettings *dst = (qpcholeskysettings*)_dst;
    qpcholeskysettings *src = (qpcholeskysettings*)_src;
    dst->epsg = src->epsg;
    dst->epsf = src->epsf;
    dst->epsx = src->epsx;
    dst->maxits = src->maxits;
}


void _qpcholeskysettings_clear(void* _p)
{
    qpcholeskysettings *p = (qpcholeskysettings*)_p;
    ae_touch_ptr((void*)p);
}


void _qpcholeskysettings_destroy(void* _p)
{
    qpcholeskysettings *p = (qpcholeskysettings*)_p;
    ae_touch_ptr((void*)p);
}


void _qpcholeskybuffers_init(void* _p, ae_state *_state)
{
    qpcholeskybuffers *p = (qpcholeskybuffers*)_p;
    ae_touch_ptr((void*)p);
    _sactiveset_init(&p->sas, _state);
    ae_vector_init(&p->pg, 0, DT_REAL, _state);
    ae_vector_init(&p->gc, 0, DT_REAL, _state);
    ae_vector_init(&p->xs, 0, DT_REAL, _state);
    ae_vector_init(&p->xn, 0, DT_REAL, _state);
    ae_vector_init(&p->workbndl, 0, DT_REAL, _state);
    ae_vector_init(&p->workbndu, 0, DT_REAL, _state);
    ae_vector_init(&p->havebndl, 0, DT_BOOL, _state);
    ae_vector_init(&p->havebndu, 0, DT_BOOL, _state);
    ae_matrix_init(&p->workcleic, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->rctmpg, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp0, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp1, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpb, 0, DT_BOOL, _state);
}


void _qpcholeskybuffers_init_copy(void* _dst, void* _src, ae_state *_state)
{
    qpcholeskybuffers *dst = (qpcholeskybuffers*)_dst;
    qpcholeskybuffers *src = (qpcholeskybuffers*)_src;
    _sactiveset_init_copy(&dst->sas, &src->sas, _state);
    ae_vector_init_copy(&dst->pg, &src->pg, _state);
    ae_vector_init_copy(&dst->gc, &src->gc, _state);
    ae_vector_init_copy(&dst->xs, &src->xs, _state);
    ae_vector_init_copy(&dst->xn, &src->xn, _state);
    ae_vector_init_copy(&dst->workbndl, &src->workbndl, _state);
    ae_vector_init_copy(&dst->workbndu, &src->workbndu, _state);
    ae_vector_init_copy(&dst->havebndl, &src->havebndl, _state);
    ae_vector_init_copy(&dst->havebndu, &src->havebndu, _state);
    ae_matrix_init_copy(&dst->workcleic, &src->workcleic, _state);
    ae_vector_init_copy(&dst->rctmpg, &src->rctmpg, _state);
    ae_vector_init_copy(&dst->tmp0, &src->tmp0, _state);
    ae_vector_init_copy(&dst->tmp1, &src->tmp1, _state);
    ae_vector_init_copy(&dst->tmpb, &src->tmpb, _state);
    dst->repinneriterationscount = src->repinneriterationscount;
    dst->repouteriterationscount = src->repouteriterationscount;
    dst->repncholesky = src->repncholesky;
}


void _qpcholeskybuffers_clear(void* _p)
{
    qpcholeskybuffers *p = (qpcholeskybuffers*)_p;
    ae_touch_ptr((void*)p);
    _sactiveset_clear(&p->sas);
    ae_vector_clear(&p->pg);
    ae_vector_clear(&p->gc);
    ae_vector_clear(&p->xs);
    ae_vector_clear(&p->xn);
    ae_vector_clear(&p->workbndl);
    ae_vector_clear(&p->workbndu);
    ae_vector_clear(&p->havebndl);
    ae_vector_clear(&p->havebndu);
    ae_matrix_clear(&p->workcleic);
    ae_vector_clear(&p->rctmpg);
    ae_vector_clear(&p->tmp0);
    ae_vector_clear(&p->tmp1);
    ae_vector_clear(&p->tmpb);
}


void _qpcholeskybuffers_destroy(void* _p)
{
    qpcholeskybuffers *p = (qpcholeskybuffers*)_p;
    ae_touch_ptr((void*)p);
    _sactiveset_destroy(&p->sas);
    ae_vector_destroy(&p->pg);
    ae_vector_destroy(&p->gc);
    ae_vector_destroy(&p->xs);
    ae_vector_destroy(&p->xn);
    ae_vector_destroy(&p->workbndl);
    ae_vector_destroy(&p->workbndu);
    ae_vector_destroy(&p->havebndl);
    ae_vector_destroy(&p->havebndu);
    ae_matrix_destroy(&p->workcleic);
    ae_vector_destroy(&p->rctmpg);
    ae_vector_destroy(&p->tmp0);
    ae_vector_destroy(&p->tmp1);
    ae_vector_destroy(&p->tmpb);
}


/*$ End $*/
