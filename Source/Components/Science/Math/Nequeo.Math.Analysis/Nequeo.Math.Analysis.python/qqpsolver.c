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
#include "qqpsolver.h"


/*$ Declarations $*/
static ae_int_t qqpsolver_quickqprestartcg = 50;
static double qqpsolver_penaltyfactor = 50.0;
static double qqpsolver_regz = 1.0E-9;
static double qqpsolver_projectedtargetfunction(qqpbuffers* sstate,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* d,
     double stp,
     /* Real    */ ae_vector* tmp0,
     ae_state *_state);
static void qqpsolver_targetgradient(qqpbuffers* sstate,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* g,
     ae_state *_state);
static void qqpsolver_quadraticmodel(qqpbuffers* sstate,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* g,
     double* d1,
     ae_int_t* d1est,
     double* d2,
     ae_int_t* d2est,
     ae_state *_state);
static void qqpsolver_findbeststepandmove(qqpbuffers* sstate,
     sactiveset* sas,
     /* Real    */ ae_vector* d,
     double stp,
     ae_bool needact,
     ae_int_t cidx,
     double cval,
     /* Real    */ ae_vector* addsteps,
     ae_int_t addstepscnt,
     /* Boolean */ ae_vector* activated,
     /* Real    */ ae_vector* tmp0,
     ae_state *_state);
static ae_bool qqpsolver_cnewtonbuild(qqpbuffers* sstate,
     ae_int_t sparsesolver,
     ae_int_t* ncholesky,
     ae_state *_state);
static ae_bool qqpsolver_cnewtonupdate(qqpbuffers* sstate,
     qqpsettings* settings,
     ae_int_t* ncupdates,
     ae_state *_state);
static ae_bool qqpsolver_cnewtonstep(qqpbuffers* sstate,
     qqpsettings* settings,
     /* Real    */ ae_vector* gc,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This function initializes QQPSettings structure with default settings.

Newly created structure MUST be initialized by default settings  -  or  by
copy of the already initialized structure.

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qqploaddefaults(ae_int_t nmain, qqpsettings* s, ae_state *_state)
{


    s->epsg = 0.0;
    s->epsf = 0.0;
    s->epsx = 1.0E-6;
    s->maxouterits = 0;
    s->cgphase = ae_true;
    s->cnphase = ae_true;
    s->cgminits = 5;
    s->cgmaxits = ae_maxint(s->cgminits, ae_round(1+0.33*nmain, _state), _state);
    s->sparsesolver = 0;
    s->cnmaxupdates = ae_round(1+0.1*nmain, _state);
}


/*************************************************************************
This function initializes QQPSettings  structure  with  copy  of  another,
already initialized structure.

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qqpcopysettings(qqpsettings* src, qqpsettings* dst, ae_state *_state)
{


    dst->epsg = src->epsg;
    dst->epsf = src->epsf;
    dst->epsx = src->epsx;
    dst->maxouterits = src->maxouterits;
    dst->cgphase = src->cgphase;
    dst->cnphase = src->cnphase;
    dst->cgminits = src->cgminits;
    dst->cgmaxits = src->cgmaxits;
    dst->sparsesolver = src->sparsesolver;
    dst->cnmaxupdates = src->cnmaxupdates;
}


/*************************************************************************
This function runs QQP solver; it returns after optimization  process  was
completed. Following QP problem is solved:

    min(0.5*(x-x_origin)'*A*(x-x_origin)+b'*(x-x_origin))
    
subject to boundary constraints.

IMPORTANT: UNLIKE MANY OTHER SOLVERS, THIS FUNCTION DOES NOT  REQUIRE  YOU
           TO INITIALIZE STATE OBJECT. IT CAN BE AUTOMATICALLY INITIALIZED
           DURING SOLUTION PROCESS.

INPUT PARAMETERS:
    AC          -   for dense problems (AKind=0) A-term of CQM object
                    contains system matrix. Other terms are unspecified
                    and should not be referenced.
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
    Settings    -   QQPSettings object initialized by one of the initialization
                    functions.
    SState      -   object which stores temporaries:
                    * uninitialized object is automatically initialized
                    * previously allocated memory is reused as much
                      as possible
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
void qqpoptimize(convexquadraticmodel* ac,
     sparsematrix* sparseac,
     ae_int_t akind,
     ae_bool sparseupper,
     /* Real    */ ae_vector* bc,
     /* Real    */ ae_vector* bndlc,
     /* Real    */ ae_vector* bnduc,
     /* Real    */ ae_vector* sc,
     /* Real    */ ae_vector* xoriginc,
     ae_int_t nc,
     /* Real    */ ae_matrix* cleicc,
     ae_int_t nec,
     ae_int_t nic,
     qqpsettings* settings,
     qqpbuffers* sstate,
     /* Real    */ ae_vector* xs,
     ae_int_t* terminationtype,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t nmain;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    double vv;
    double d2;
    double d1;
    ae_int_t d1est;
    ae_int_t d2est;
    ae_bool needact;
    double reststp;
    double fullstp;
    double stpmax;
    double stp;
    ae_int_t stpcnt;
    ae_int_t cidx;
    double cval;
    ae_int_t cgcnt;
    ae_int_t cgmax;
    ae_int_t newtcnt;
    ae_int_t sparsesolver;
    double beta;
    ae_bool b;
    double fprev;
    double fcur;

    *terminationtype = 0;

    
    /*
     * Primary checks
     */
    ae_assert(akind==0||akind==1, "QQPOptimize: incorrect AKind", _state);
    sstate->nmain = nc;
    sstate->nslack = nic;
    sstate->n = nc+nic;
    sstate->nec = nec;
    sstate->nic = nic;
    sstate->akind = akind;
    n = sstate->n;
    nmain = sstate->nmain;
    *terminationtype = 0;
    sstate->repinneriterationscount = 0;
    sstate->repouteriterationscount = 0;
    sstate->repncholesky = 0;
    sstate->repncupdates = 0;
    
    /*
     * Several checks
     * * matrix size
     * * scale vector
     * * consistency of bound constraints
     * * consistency of settings
     */
    if( akind==1 )
    {
        ae_assert(sparsegetnrows(sparseac, _state)==nmain, "QQPOptimize: rows(SparseAC)<>NMain", _state);
        ae_assert(sparsegetncols(sparseac, _state)==nmain, "QQPOptimize: cols(SparseAC)<>NMain", _state);
    }
    for(i=0; i<=nmain-1; i++)
    {
        ae_assert(ae_isfinite(sc->ptr.p_double[i], _state)&&ae_fp_greater(sc->ptr.p_double[i],(double)(0)), "QQPOptimize: incorrect scale", _state);
    }
    for(i=0; i<=nmain-1; i++)
    {
        if( ae_isfinite(bndlc->ptr.p_double[i], _state)&&ae_isfinite(bnduc->ptr.p_double[i], _state) )
        {
            if( ae_fp_greater(bndlc->ptr.p_double[i],bnduc->ptr.p_double[i]) )
            {
                *terminationtype = -3;
                return;
            }
        }
    }
    ae_assert(settings->cgphase||settings->cnphase, "QQPOptimize: both phases (CG and Newton) are inactive", _state);
    
    /*
     * Allocate data structures
     */
    rvectorsetlengthatleast(&sstate->bndl, n, _state);
    rvectorsetlengthatleast(&sstate->bndu, n, _state);
    bvectorsetlengthatleast(&sstate->havebndl, n, _state);
    bvectorsetlengthatleast(&sstate->havebndu, n, _state);
    rvectorsetlengthatleast(&sstate->xs, n, _state);
    rvectorsetlengthatleast(&sstate->xp, n, _state);
    rvectorsetlengthatleast(&sstate->gc, n, _state);
    rvectorsetlengthatleast(&sstate->cgc, n, _state);
    rvectorsetlengthatleast(&sstate->cgp, n, _state);
    rvectorsetlengthatleast(&sstate->dc, n, _state);
    rvectorsetlengthatleast(&sstate->dp, n, _state);
    rvectorsetlengthatleast(&sstate->tmp0, n, _state);
    rvectorsetlengthatleast(&sstate->stpbuf, 15, _state);
    sasinit(n, &sstate->sas, _state);
    
    /*
     * Scale/shift problem coefficients:
     *
     *     min { 0.5*(x-x0)'*A*(x-x0) + b'*(x-x0) }
     *
     * becomes (after transformation "x = S*y+x0")
     *
     *     min { 0.5*y'*(S*A*S)*y + (S*b)'*y
     *
     * Modified A_mod=S*A*S and b_mod=S*(b+A*x0) are
     * stored into SState.DenseA and SState.B.
     *
     * NOTE: DenseA/DenseB are arrays whose lengths are
     *       NMain, not N=NMain+NSlack! We store reduced 
     *       matrix and vector because extend parts (last
     *       NSlack rows/columns) are exactly zero.
     *       
     */
    rvectorsetlengthatleast(&sstate->b, nmain, _state);
    for(i=0; i<=nmain-1; i++)
    {
        sstate->b.ptr.p_double[i] = sc->ptr.p_double[i]*bc->ptr.p_double[i];
    }
    if( akind==0 )
    {
        
        /*
         * Dense QP problem - just copy and scale.
         */
        rmatrixsetlengthatleast(&sstate->densea, nmain, nmain, _state);
        cqmgeta(ac, &sstate->densea, _state);
        sstate->absamax = (double)(0);
        sstate->absasum = (double)(0);
        sstate->absasum2 = (double)(0);
        for(i=0; i<=nmain-1; i++)
        {
            for(j=0; j<=nmain-1; j++)
            {
                v = sc->ptr.p_double[i]*sstate->densea.ptr.pp_double[i][j]*sc->ptr.p_double[j];
                sstate->densea.ptr.pp_double[i][j] = v;
                sstate->absamax = ae_maxreal(sstate->absamax, v, _state);
                sstate->absasum = sstate->absasum+v;
                sstate->absasum2 = sstate->absasum2+v*v;
            }
        }
    }
    else
    {
        
        /*
         * Sparse QP problem - a bit tricky. Depending on format of the
         * input we use different strategies for copying matrix:
         * * SKS matrices are copied to SKS format
         * * anything else is copied to CRS format
         */
        ae_assert(akind==1, "QQPOptimize: unexpected AKind (internal error)", _state);
        sparsecopytosksbuf(sparseac, &sstate->sparsea, _state);
        if( sparseupper )
        {
            sparsetransposesks(&sstate->sparsea, _state);
        }
        sstate->sparseupper = ae_false;
        sstate->absamax = (double)(0);
        sstate->absasum = (double)(0);
        sstate->absasum2 = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            k = sstate->sparsea.ridx.ptr.p_int[i];
            for(j=i-sstate->sparsea.didx.ptr.p_int[i]; j<=i; j++)
            {
                v = sc->ptr.p_double[i]*sstate->sparsea.vals.ptr.p_double[k]*sc->ptr.p_double[j];
                sstate->sparsea.vals.ptr.p_double[k] = v;
                if( i==j )
                {
                    
                    /*
                     * Diagonal terms are counted only once
                     */
                    sstate->absamax = ae_maxreal(sstate->absamax, v, _state);
                    sstate->absasum = sstate->absasum+v;
                    sstate->absasum2 = sstate->absasum2+v*v;
                }
                else
                {
                    
                    /*
                     * Offdiagonal terms are counted twice
                     */
                    sstate->absamax = ae_maxreal(sstate->absamax, v, _state);
                    sstate->absasum = sstate->absasum+2*v;
                    sstate->absasum2 = sstate->absasum2+2*v*v;
                }
                k = k+1;
            }
        }
    }
    
    /*
     * Load box constraints into State structure.
     *
     * We apply transformation to variables: y=(x-x_origin)/s,
     * each of the constraints is appropriately shifted/scaled.
     */
    for(i=0; i<=nmain-1; i++)
    {
        sstate->havebndl.ptr.p_bool[i] = ae_isfinite(bndlc->ptr.p_double[i], _state);
        if( sstate->havebndl.ptr.p_bool[i] )
        {
            sstate->bndl.ptr.p_double[i] = (bndlc->ptr.p_double[i]-xoriginc->ptr.p_double[i])/sc->ptr.p_double[i];
        }
        else
        {
            ae_assert(ae_isneginf(bndlc->ptr.p_double[i], _state), "QQPOptimize: incorrect lower bound", _state);
            sstate->bndl.ptr.p_double[i] = _state->v_neginf;
        }
        sstate->havebndu.ptr.p_bool[i] = ae_isfinite(bnduc->ptr.p_double[i], _state);
        if( sstate->havebndu.ptr.p_bool[i] )
        {
            sstate->bndu.ptr.p_double[i] = (bnduc->ptr.p_double[i]-xoriginc->ptr.p_double[i])/sc->ptr.p_double[i];
        }
        else
        {
            ae_assert(ae_isposinf(bnduc->ptr.p_double[i], _state), "QQPOptimize: incorrect upper bound", _state);
            sstate->bndu.ptr.p_double[i] = _state->v_posinf;
        }
    }
    for(i=nmain; i<=n-1; i++)
    {
        sstate->havebndl.ptr.p_bool[i] = ae_true;
        sstate->bndl.ptr.p_double[i] = 0.0;
        sstate->havebndu.ptr.p_bool[i] = ae_false;
        sstate->bndu.ptr.p_double[i] = _state->v_posinf;
    }
    
    /*
     * Shift/scale linear constraints with transformation y=(x-x_origin)/s:
     * * constraint "c[i]'*x = b[i]" becomes "(s[i]*c[i])'*x = b[i]-c[i]'*x_origin".
     * * after constraint is loaded into SState.CLEIC, it is additionally normalized
     */
    rmatrixsetlengthatleast(&sstate->cleic, nec+nic, n+1, _state);
    for(i=0; i<=nec+nic-1; i++)
    {
        v = (double)(0);
        vv = (double)(0);
        for(j=0; j<=nmain-1; j++)
        {
            sstate->cleic.ptr.pp_double[i][j] = cleicc->ptr.pp_double[i][j]*sc->ptr.p_double[j];
            vv = vv+ae_sqr(sstate->cleic.ptr.pp_double[i][j], _state);
            v = v+cleicc->ptr.pp_double[i][j]*xoriginc->ptr.p_double[j];
        }
        vv = ae_sqrt(vv, _state);
        for(j=nmain; j<=n-1; j++)
        {
            sstate->cleic.ptr.pp_double[i][j] = 0.0;
        }
        sstate->cleic.ptr.pp_double[i][n] = cleicc->ptr.pp_double[i][nmain]-v;
        if( i>=nec )
        {
            sstate->cleic.ptr.pp_double[i][nmain+i-nec] = 1.0;
        }
        if( ae_fp_greater(vv,(double)(0)) )
        {
            for(j=0; j<=n; j++)
            {
                sstate->cleic.ptr.pp_double[i][j] = sstate->cleic.ptr.pp_double[i][j]/vv;
            }
        }
    }
    
    /*
     * Process initial point:
     * * first NMain components are equal to XS-XOriginC
     * * last NIC components are deduced from linear constraints
     * * make sure that boundary constraints are preserved by transformation
     */
    for(i=0; i<=nmain-1; i++)
    {
        sstate->xs.ptr.p_double[i] = (xs->ptr.p_double[i]-xoriginc->ptr.p_double[i])/sc->ptr.p_double[i];
        if( sstate->havebndl.ptr.p_bool[i]&&ae_fp_less(sstate->xs.ptr.p_double[i],sstate->bndl.ptr.p_double[i]) )
        {
            sstate->xs.ptr.p_double[i] = sstate->bndl.ptr.p_double[i];
        }
        if( sstate->havebndu.ptr.p_bool[i]&&ae_fp_greater(sstate->xs.ptr.p_double[i],sstate->bndu.ptr.p_double[i]) )
        {
            sstate->xs.ptr.p_double[i] = sstate->bndu.ptr.p_double[i];
        }
        if( sstate->havebndl.ptr.p_bool[i]&&ae_fp_eq(xs->ptr.p_double[i],bndlc->ptr.p_double[i]) )
        {
            sstate->xs.ptr.p_double[i] = sstate->bndl.ptr.p_double[i];
        }
        if( sstate->havebndu.ptr.p_bool[i]&&ae_fp_eq(xs->ptr.p_double[i],bnduc->ptr.p_double[i]) )
        {
            sstate->xs.ptr.p_double[i] = sstate->bndu.ptr.p_double[i];
        }
    }
    for(i=0; i<=nic-1; i++)
    {
        v = ae_v_dotproduct(&sstate->xs.ptr.p_double[0], 1, &sstate->cleic.ptr.pp_double[nec+i][0], 1, ae_v_len(0,nmain-1));
        sstate->xs.ptr.p_double[nmain+i] = ae_maxreal(sstate->cleic.ptr.pp_double[nec+i][n]-v, 0.0, _state);
    }
    
    /*
     * Prepare "active set" structure
     */
    sassetbc(&sstate->sas, &sstate->bndl, &sstate->bndu, _state);
    sassetlcx(&sstate->sas, &sstate->cleic, 0, 0, _state);
    if( !sasstartoptimization(&sstate->sas, &sstate->xs, _state) )
    {
        *terminationtype = -3;
        return;
    }
    
    /*
     * Select sparse direct solver
     */
    if( akind==1 )
    {
        sparsesolver = settings->sparsesolver;
        if( sparsesolver==0 )
        {
            sparsesolver = 1;
        }
        if( sparseissks(&sstate->sparsea, _state) )
        {
            sparsesolver = 2;
        }
        sparsesolver = 2;
        ae_assert(sparsesolver==1||sparsesolver==2, "QQPOptimize: incorrect SparseSolver", _state);
    }
    else
    {
        sparsesolver = 0;
    }
    
    /*
     * Main loop.
     *
     * Following variables are used:
     * * GC stores current gradient (unconstrained)
     * * CGC stores current gradient (constrained)
     * * DC stores current search direction
     * * CGP stores constrained gradient at previous point
     *   (zero on initial entry)
     * * DP stores previous search direction
     *   (zero on initial entry)
     */
    cgmax = settings->cgminits;
    sstate->repinneriterationscount = 0;
    sstate->repouteriterationscount = 0;
    for(;;)
    {
        if( settings->maxouterits>0&&sstate->repouteriterationscount>=settings->maxouterits )
        {
            *terminationtype = 5;
            break;
        }
        if( sstate->repouteriterationscount>0 )
        {
            
            /*
             * Check EpsF- and EpsX-based stopping criteria.
             * Because problem was already scaled, we do not scale step before checking its length.
             * NOTE: these checks are performed only after at least one outer iteration was made.
             */
            if( ae_fp_greater(settings->epsf,(double)(0)) )
            {
                
                /*
                 * NOTE 1: here we rely on the fact that ProjectedTargetFunction() ignore D when Stp=0
                 * NOTE 2: code below handles situation when update increases function value instead
                 *         of decreasing it.
                 */
                fprev = qqpsolver_projectedtargetfunction(sstate, &sstate->xp, &sstate->dc, 0.0, &sstate->tmp0, _state);
                fcur = qqpsolver_projectedtargetfunction(sstate, &sstate->sas.xc, &sstate->dc, 0.0, &sstate->tmp0, _state);
                if( ae_fp_less_eq(fprev-fcur,settings->epsf*ae_maxreal(ae_fabs(fprev, _state), ae_maxreal(ae_fabs(fcur, _state), 1.0, _state), _state)) )
                {
                    *terminationtype = 1;
                    break;
                }
            }
            if( ae_fp_greater(settings->epsx,(double)(0)) )
            {
                v = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    v = v+ae_sqr(sstate->xp.ptr.p_double[i]-sstate->sas.xc.ptr.p_double[i], _state);
                }
                if( ae_fp_less_eq(ae_sqrt(v, _state),settings->epsx) )
                {
                    *terminationtype = 2;
                    break;
                }
            }
        }
        inc(&sstate->repouteriterationscount, _state);
        ae_v_move(&sstate->xp.ptr.p_double[0], 1, &sstate->sas.xc.ptr.p_double[0], 1, ae_v_len(0,n-1));
        if( !settings->cgphase )
        {
            cgmax = 0;
        }
        for(i=0; i<=n-1; i++)
        {
            sstate->cgp.ptr.p_double[i] = 0.0;
            sstate->dp.ptr.p_double[i] = 0.0;
        }
        for(cgcnt=0; cgcnt<=cgmax-1; cgcnt++)
        {
            
            /*
             * Calculate unconstrained gradient GC for "extended" QP problem
             * Determine active set, current constrained gradient CGC.
             * Check gradient-based stopping condition.
             *
             * NOTE: because problem was scaled, we do not have to apply scaling
             *       to gradient before checking stopping condition.
             */
            qqpsolver_targetgradient(sstate, &sstate->sas.xc, &sstate->gc, _state);
            sasreactivateconstraints(&sstate->sas, &sstate->gc, _state);
            ae_v_move(&sstate->cgc.ptr.p_double[0], 1, &sstate->gc.ptr.p_double[0], 1, ae_v_len(0,n-1));
            sasconstraineddirection(&sstate->sas, &sstate->cgc, _state);
            v = ae_v_dotproduct(&sstate->cgc.ptr.p_double[0], 1, &sstate->cgc.ptr.p_double[0], 1, ae_v_len(0,n-1));
            if( ae_fp_less_eq(ae_sqrt(v, _state),settings->epsg) )
            {
                *terminationtype = 4;
                break;
            }
            
            /*
             * Prepare search direction DC and explore it.
             *
             * We try to use CGP/DP to prepare conjugate gradient step,
             * but we resort to steepest descent step (Beta=0) in case
             * we are at I-th boundary, but DP[I]<>0.
             *
             * Such approach allows us to ALWAYS have feasible DC, with
             * guaranteed compatibility with both feasible area and current
             * active set.
             *
             * Automatic CG reset performed every time DP is incompatible
             * with current active set and/or feasible area. We also
             * perform reset every QuickQPRestartCG iterations.
             */
            ae_v_moveneg(&sstate->dc.ptr.p_double[0], 1, &sstate->cgc.ptr.p_double[0], 1, ae_v_len(0,n-1));
            v = 0.0;
            vv = 0.0;
            b = ae_false;
            for(i=0; i<=n-1; i++)
            {
                v = v+sstate->cgc.ptr.p_double[i]*sstate->cgc.ptr.p_double[i];
                vv = vv+sstate->cgp.ptr.p_double[i]*sstate->cgp.ptr.p_double[i];
                b = b||((sstate->havebndl.ptr.p_bool[i]&&ae_fp_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndl.ptr.p_double[i]))&&ae_fp_neq(sstate->dp.ptr.p_double[i],(double)(0)));
                b = b||((sstate->havebndu.ptr.p_bool[i]&&ae_fp_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndu.ptr.p_double[i]))&&ae_fp_neq(sstate->dp.ptr.p_double[i],(double)(0)));
            }
            b = b||ae_fp_eq(vv,(double)(0));
            b = b||cgcnt%qqpsolver_quickqprestartcg==0;
            if( !b )
            {
                beta = v/vv;
            }
            else
            {
                beta = 0.0;
            }
            ae_v_addd(&sstate->dc.ptr.p_double[0], 1, &sstate->dp.ptr.p_double[0], 1, ae_v_len(0,n-1), beta);
            sasconstraineddirection(&sstate->sas, &sstate->dc, _state);
            sasexploredirection(&sstate->sas, &sstate->dc, &stpmax, &cidx, &cval, _state);
            
            /*
             * Build quadratic model of F along descent direction:
             *
             *     F(xc+alpha*D) = D2*alpha^2 + D1*alpha
             *
             * Terminate algorithm if needed.
             *
             * NOTE: we do not maintain constant term D0
             */
            qqpsolver_quadraticmodel(sstate, &sstate->sas.xc, &sstate->dc, &sstate->gc, &d1, &d1est, &d2, &d2est, _state);
            if( ae_fp_eq(d1,(double)(0))&&ae_fp_eq(d2,(double)(0)) )
            {
                
                /*
                 * D1 and D2 are exactly zero, success.
                 * After this if-then we assume that D is non-zero.
                 */
                *terminationtype = 4;
                break;
            }
            if( d1est>=0 )
            {
                
                /*
                 * Numerical noise is too large, it means that we are close
                 * to minimum - and that further improvement is impossible.
                 *
                 * After this if-then we assume that D1 is definitely negative
                 * (even under presence of numerical errors).
                 */
                *terminationtype = 7;
                break;
            }
            if( d2est<=0&&cidx<0 )
            {
                
                /*
                 * Function is unbounded from below:
                 * * D1<0 (verified by previous block)
                 * * D2Est<=0, which means that either D2<0 - or it can not
                 *   be reliably distinguished from zero.
                 * * step is unconstrained
                 *
                 * If these conditions are true, we abnormally terminate QP
                 * algorithm with return code -4
                 */
                *terminationtype = -4;
                break;
            }
            
            /*
             * Perform step along DC.
             *
             * In this block of code we maintain two step length:
             * * RestStp -  restricted step, maximum step length along DC which does
             *              not violate constraints
             * * FullStp -  step length along DC which minimizes quadratic function
             *              without taking constraints into account. If problem is
             *              unbounded from below without constraints, FullStp is
             *              forced to be RestStp.
             *
             * So, if function is convex (D2>0):
             * * FullStp = -D1/(2*D2)
             * * RestStp = restricted FullStp
             * * 0<=RestStp<=FullStp
             *
             * If function is non-convex, but bounded from below under constraints:
             * * RestStp = step length subject to constraints
             * * FullStp = RestStp
             *
             * After RestStp and FullStp are initialized, we generate several trial
             * steps which are different multiples of RestStp and FullStp.
             */
            if( d2est>0 )
            {
                ae_assert(ae_fp_less(d1,(double)(0)), "QQPOptimize: internal error", _state);
                fullstp = -d1/(2*d2);
                needact = ae_fp_greater_eq(fullstp,stpmax);
                if( needact )
                {
                    ae_assert(sstate->stpbuf.cnt>=3, "QQPOptimize: StpBuf overflow", _state);
                    reststp = stpmax;
                    stp = reststp;
                    sstate->stpbuf.ptr.p_double[0] = reststp*4;
                    sstate->stpbuf.ptr.p_double[1] = fullstp;
                    sstate->stpbuf.ptr.p_double[2] = fullstp/4;
                    stpcnt = 3;
                }
                else
                {
                    reststp = fullstp;
                    stp = fullstp;
                    stpcnt = 0;
                }
            }
            else
            {
                ae_assert(cidx>=0, "QQPOptimize: internal error", _state);
                ae_assert(sstate->stpbuf.cnt>=2, "QQPOptimize: StpBuf overflow", _state);
                reststp = stpmax;
                fullstp = stpmax;
                stp = reststp;
                needact = ae_true;
                sstate->stpbuf.ptr.p_double[0] = 4*reststp;
                stpcnt = 1;
            }
            qqpsolver_findbeststepandmove(sstate, &sstate->sas, &sstate->dc, stp, needact, cidx, cval, &sstate->stpbuf, stpcnt, &sstate->activated, &sstate->tmp0, _state);
            
            /*
             * Update CG information.
             */
            ae_v_move(&sstate->dp.ptr.p_double[0], 1, &sstate->dc.ptr.p_double[0], 1, ae_v_len(0,n-1));
            ae_v_move(&sstate->cgp.ptr.p_double[0], 1, &sstate->cgc.ptr.p_double[0], 1, ae_v_len(0,n-1));
            
            /*
             * Update iterations counter
             */
            sstate->repinneriterationscount = sstate->repinneriterationscount+1;
        }
        if( *terminationtype!=0 )
        {
            break;
        }
        cgmax = settings->cgmaxits;
        
        /*
         * Generate YIdx - reordering of variables for constrained Newton phase.
         * Free variables come first, fixed are last ones.
         */
        newtcnt = 0;
        for(;;)
        {
            
            /*
             * Skip iteration if constrained Newton is turned off.
             */
            if( !settings->cnphase )
            {
                break;
            }
            
            /*
             * At the first iteration   - build Cholesky decomposition of Hessian.
             * At subsequent iterations - refine Hessian by adding new constraints.
             *
             * Loop is terminated in following cases:
             * * Hessian is not positive definite subject to current constraints
             *   (termination during initial decomposition)
             * * there were no new constraints being activated
             *   (termination during update)
             * * all constraints were activated during last step
             *   (termination during update)
             * * CNMaxUpdates were performed on matrix
             *   (termination during update)
             */
            if( newtcnt==0 )
            {
                
                /*
                 * Perform initial Newton step. If Cholesky decomposition fails,
                 * increase number of CG iterations to CGMaxIts - it should help
                 * us to find set of constraints which will make matrix positive
                 * definite.
                 */
                b = qqpsolver_cnewtonbuild(sstate, sparsesolver, &sstate->repncholesky, _state);
                if( b )
                {
                    cgmax = settings->cgminits;
                }
            }
            else
            {
                b = qqpsolver_cnewtonupdate(sstate, settings, &sstate->repncupdates, _state);
            }
            if( !b )
            {
                break;
            }
            inc(&newtcnt, _state);
            
            /*
             * Calculate gradient GC.
             */
            qqpsolver_targetgradient(sstate, &sstate->sas.xc, &sstate->gc, _state);
            
            /*
             * Bound-constrained Newton step
             */
            for(i=0; i<=n-1; i++)
            {
                sstate->dc.ptr.p_double[i] = sstate->gc.ptr.p_double[i];
            }
            if( !qqpsolver_cnewtonstep(sstate, settings, &sstate->dc, _state) )
            {
                break;
            }
            qqpsolver_quadraticmodel(sstate, &sstate->sas.xc, &sstate->dc, &sstate->gc, &d1, &d1est, &d2, &d2est, _state);
            if( d1est>=0||d2est<=0 )
            {
                break;
            }
            ae_assert(ae_fp_less(d1,(double)(0)), "QQPOptimize: internal error", _state);
            fullstp = -d1/(2*d2);
            sasexploredirection(&sstate->sas, &sstate->dc, &stpmax, &cidx, &cval, _state);
            needact = ae_fp_greater_eq(fullstp,stpmax);
            if( needact )
            {
                ae_assert(sstate->stpbuf.cnt>=3, "QQPOptimize: StpBuf overflow", _state);
                reststp = stpmax;
                stp = reststp;
                sstate->stpbuf.ptr.p_double[0] = reststp*4;
                sstate->stpbuf.ptr.p_double[1] = fullstp;
                sstate->stpbuf.ptr.p_double[2] = fullstp/4;
                stpcnt = 3;
            }
            else
            {
                reststp = fullstp;
                stp = fullstp;
                stpcnt = 0;
            }
            qqpsolver_findbeststepandmove(sstate, &sstate->sas, &sstate->dc, stp, needact, cidx, cval, &sstate->stpbuf, stpcnt, &sstate->activated, &sstate->tmp0, _state);
        }
        if( *terminationtype!=0 )
        {
            break;
        }
    }
    
    /*
     * Stop optimization and unpack results.
     *
     * Add XOriginC to XS and make sure that boundary constraints are
     * both (a) satisfied, (b) preserved. Former means that "shifted"
     * point is feasible, while latter means that point which was exactly
     * at the boundary before shift will be exactly at the boundary
     * after shift.
     */
    sasstopoptimization(&sstate->sas, _state);
    for(i=0; i<=nmain-1; i++)
    {
        xs->ptr.p_double[i] = sc->ptr.p_double[i]*sstate->sas.xc.ptr.p_double[i]+xoriginc->ptr.p_double[i];
        if( sstate->havebndl.ptr.p_bool[i]&&ae_fp_less(xs->ptr.p_double[i],bndlc->ptr.p_double[i]) )
        {
            xs->ptr.p_double[i] = bndlc->ptr.p_double[i];
        }
        if( sstate->havebndu.ptr.p_bool[i]&&ae_fp_greater(xs->ptr.p_double[i],bnduc->ptr.p_double[i]) )
        {
            xs->ptr.p_double[i] = bnduc->ptr.p_double[i];
        }
        if( sstate->havebndl.ptr.p_bool[i]&&ae_fp_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndl.ptr.p_double[i]) )
        {
            xs->ptr.p_double[i] = bndlc->ptr.p_double[i];
        }
        if( sstate->havebndu.ptr.p_bool[i]&&ae_fp_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndu.ptr.p_double[i]) )
        {
            xs->ptr.p_double[i] = bnduc->ptr.p_double[i];
        }
    }
}


/*************************************************************************
Target function at point PROJ(X+Stp*D), where PROJ(.) is a projection into
feasible set.

NOTE: if Stp=0, D is not referenced at all. Thus,  there  is  no  need  to
      fill it by some meaningful values for Stp=0.

This subroutine uses temporary buffer Tmp, which is automatically  resized
if needed.

  -- ALGLIB --
     Copyright 21.12.2013 by Bochkanov Sergey
*************************************************************************/
static double qqpsolver_projectedtargetfunction(qqpbuffers* sstate,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* d,
     double stp,
     /* Real    */ ae_vector* tmp0,
     ae_state *_state)
{
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t n;
    ae_int_t nmain;
    ae_int_t i;
    ae_int_t j;
    double v;
    double vv;
    double result;


    n = sstate->n;
    nmain = sstate->nmain;
    nec = sstate->nec;
    nic = sstate->nic;
    rvectorsetlengthatleast(tmp0, n, _state);
    
    /*
     * Calculate projected point
     */
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_neq(stp,(double)(0)) )
        {
            v = x->ptr.p_double[i]+stp*d->ptr.p_double[i];
        }
        else
        {
            v = x->ptr.p_double[i];
        }
        if( sstate->havebndl.ptr.p_bool[i]&&ae_fp_less(v,sstate->bndl.ptr.p_double[i]) )
        {
            v = sstate->bndl.ptr.p_double[i];
        }
        if( sstate->havebndu.ptr.p_bool[i]&&ae_fp_greater(v,sstate->bndu.ptr.p_double[i]) )
        {
            v = sstate->bndu.ptr.p_double[i];
        }
        tmp0->ptr.p_double[i] = v;
    }
    
    /*
     * Function value at the Tmp0:
     *
     * f(x) = 0.5*x'*A*x + b'*x + penaltyfactor*0.5*(C*x-b)'*(C*x-b)
     */
    result = 0.0;
    for(i=0; i<=nmain-1; i++)
    {
        result = result+sstate->b.ptr.p_double[i]*tmp0->ptr.p_double[i];
    }
    if( sstate->akind==0 )
    {
        
        /*
         * Dense matrix A
         */
        for(i=0; i<=nmain-1; i++)
        {
            v = tmp0->ptr.p_double[i];
            result = result+0.5*v*v*sstate->densea.ptr.pp_double[i][i];
            vv = 0.0;
            for(j=i+1; j<=nmain-1; j++)
            {
                vv = vv+sstate->densea.ptr.pp_double[i][j]*tmp0->ptr.p_double[j];
            }
            result = result+v*vv;
        }
    }
    else
    {
        
        /*
         * sparse matrix A
         */
        ae_assert(sstate->akind==1, "QQPOptimize: unexpected AKind in ProjectedTargetFunction", _state);
        result = result+0.5*sparsevsmv(&sstate->sparsea, sstate->sparseupper, tmp0, _state);
    }
    for(i=0; i<=nec+nic-1; i++)
    {
        v = ae_v_dotproduct(&sstate->cleic.ptr.pp_double[i][0], 1, &tmp0->ptr.p_double[0], 1, ae_v_len(0,n-1));
        result = result+0.5*qqpsolver_penaltyfactor*ae_sqr(v-sstate->cleic.ptr.pp_double[i][n], _state);
    }
    return result;
}


/*************************************************************************
Gradient of "extended" (N>=NMain variables, original + slack ones)  target
function:

    f(x) = 0.5*x'*A*x + b'*x + penaltyfactor*0.5*(C*x-b)'*(C*x-b)
    
which is equal to

    grad = A*x + b + penaltyfactor*C'*(C*x-b)

Here:
* x is array[N] (that's why problem is called extended - N>=NMain)
* A is array[NMain,NMain]
* b is array[NMain]
* C is array[NEC+NIC,N]

INPUT PARAMETERS:
    SState  -   structure which stores function terms (not modified)
    X       -   location
    G       -   possibly preallocated buffer

OUTPUT PARAMETERS:
    G       -   array[N], gradient
            
  -- ALGLIB --
     Copyright 21.12.2013 by Bochkanov Sergey
*************************************************************************/
static void qqpsolver_targetgradient(qqpbuffers* sstate,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* g,
     ae_state *_state)
{
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t n;
    ae_int_t nmain;
    ae_int_t i;
    double v;


    n = sstate->n;
    nmain = sstate->nmain;
    nec = sstate->nec;
    nic = sstate->nic;
    rvectorsetlengthatleast(g, n, _state);
    if( sstate->akind==0 )
    {
        
        /*
         * Dense matrix A
         */
        rmatrixmv(nmain, nmain, &sstate->densea, 0, 0, 0, x, 0, g, 0, _state);
    }
    else
    {
        
        /*
         * Sparse matrix A
         */
        ae_assert(sstate->akind==1, "QQPOptimize: unexpected AKind in TargetGradient", _state);
        sparsesmv(&sstate->sparsea, sstate->sparseupper, x, g, _state);
    }
    ae_v_add(&g->ptr.p_double[0], 1, &sstate->b.ptr.p_double[0], 1, ae_v_len(0,nmain-1));
    for(i=nmain; i<=n-1; i++)
    {
        g->ptr.p_double[i] = 0.0;
    }
    for(i=0; i<=nec+nic-1; i++)
    {
        v = ae_v_dotproduct(&sstate->cleic.ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
        v = v-sstate->cleic.ptr.pp_double[i][n];
        v = v*qqpsolver_penaltyfactor;
        ae_v_addd(&g->ptr.p_double[0], 1, &sstate->cleic.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
    }
}


/*************************************************************************
First and second derivatives  of  the  "extended"  target  function  along
specified direction. Target  function  is  called  "extended"  because  of
additional slack variables and has form:

    f(x) = 0.5*x'*A*x + b'*x + penaltyfactor*0.5*(C*x-b)'*(C*x-b)
    
with gradient

    grad = A*x + b + penaltyfactor*C'*(C*x-b)
    
Quadratic model has form

    F(x0+alpha*D) = D2*alpha^2 + D1*alpha

INPUT PARAMETERS:
    SState  -   structure which is used to obtain quadratic term of the model
    X       -   current point, array[N]
    D       -   direction across which derivatives are calculated, array[N]
    G       -   gradient at current point (pre-calculated by caller), array[N]

OUTPUT PARAMETERS:
    D1      -   linear coefficient
    D1Est   -   estimate of D1 sign,  accounting  for  possible  numerical
                errors:
                * >0    means "almost surely positive"
                * <0    means "almost surely negative"
                * =0    means "pessimistic estimate  of  numerical  errors
                        in D1 is larger than magnitude of D1 itself; it is
                        impossible to reliably distinguish D1 from zero".
    D2      -   quadratic coefficient
    D2Est   -   estimate of D2 sign,  accounting  for  possible  numerical
                errors:
                * >0    means "almost surely positive"
                * <0    means "almost surely negative"
                * =0    means "pessimistic estimate  of  numerical  errors
                        in D2 is larger than magnitude of D2 itself; it is
                        impossible to reliably distinguish D2 from zero".
            
  -- ALGLIB --
     Copyright 14.05.2014 by Bochkanov Sergey
*************************************************************************/
static void qqpsolver_quadraticmodel(qqpbuffers* sstate,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* g,
     double* d1,
     ae_int_t* d1est,
     double* d2,
     ae_int_t* d2est,
     ae_state *_state)
{
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t n;
    ae_int_t nmain;
    ae_int_t i;
    ae_int_t j;
    double v;
    double vv;
    double mx;
    double mb;
    double md;

    *d1 = 0;
    *d1est = 0;
    *d2 = 0;
    *d2est = 0;

    n = sstate->n;
    nmain = sstate->nmain;
    nec = sstate->nec;
    nic = sstate->nic;
    
    /*
     * Maximums
     */
    mx = 0.0;
    md = 0.0;
    mb = 0.0;
    for(i=0; i<=n-1; i++)
    {
        mx = ae_maxreal(mx, ae_fabs(x->ptr.p_double[i], _state), _state);
        md = ae_maxreal(md, ae_fabs(d->ptr.p_double[i], _state), _state);
    }
    for(i=0; i<=nmain-1; i++)
    {
        mb = ae_maxreal(mb, ae_fabs(sstate->b.ptr.p_double[i], _state), _state);
    }
    
    /*
     * D2
     */
    if( sstate->akind==0 )
    {
        
        /*
         * Dense matrix A
         */
        *d2 = 0.0;
        for(i=0; i<=nmain-1; i++)
        {
            v = d->ptr.p_double[i];
            vv = 0.0;
            *d2 = *d2+0.5*v*v*sstate->densea.ptr.pp_double[i][i];
            for(j=i+1; j<=nmain-1; j++)
            {
                vv = vv+sstate->densea.ptr.pp_double[i][j]*d->ptr.p_double[j];
            }
            *d2 = *d2+v*vv;
        }
    }
    else
    {
        
        /*
         * Sparse matrix A
         */
        ae_assert(sstate->akind==1, "QQPOptimize: unexpected AKind in TargetGradient", _state);
        *d2 = 0.5*sparsevsmv(&sstate->sparsea, sstate->sparseupper, d, _state);
    }
    for(i=0; i<=nec+nic-1; i++)
    {
        
        /*
         * NOTE: there is no "V:=V-CLEIC[I,N]" line, and it is not an error!
         *       We estimate curvature information here, which is not dependent
         *       on right part.
         */
        v = ae_v_dotproduct(&sstate->cleic.ptr.pp_double[i][0], 1, &d->ptr.p_double[0], 1, ae_v_len(0,n-1));
        *d2 = *d2+v*v*qqpsolver_penaltyfactor*0.5;
    }
    v = ae_v_dotproduct(&d->ptr.p_double[0], 1, &g->ptr.p_double[0], 1, ae_v_len(0,n-1));
    *d1 = v;
    
    /*
     * Error estimates
     */
    estimateparabolicmodel(sstate->absasum, sstate->absasum2, mx, mb, md, *d1, *d2, d1est, d2est, _state);
}


/*************************************************************************
This function accepts quadratic model of the form

    f(x) = 0.5*x'*A*x + b'*x + penaltyfactor*0.5*(C*x-b)'*(C*x-b)
    
and list of possible steps along direction D. It chooses  best  step  (one
which achieves minimum value of the target  function)  and  moves  current
point (given by SAS object) to the new location. Step is  bounded  subject
to boundary constraints.

Candidate steps are divided into two groups:
* "default" step, which is always performed when no candidate steps LONGER
  THAN THE DEFAULT  ONE  is  given.  This  candidate  MUST  reduce  target
  function value; it is  responsibility  of  caller  to   provide  default
  candidate which reduces target function.
* "additional candidates", which may be shorter or longer than the default
  step. Candidates which are shorter that the default  step  are  ignored;
  candidates which are longer than the "default" step are tested.
  
The idea is that we ALWAYS try "default" step, and it is responsibility of
the caller to provide us with something which is worth trying.  This  step
may activate some constraint - that's why we  stopped  at  "default"  step
size. However, we may also try longer steps which may activate  additional
constraints and further reduce function value.

INPUT PARAMETERS:
    SState  -   structure which stores model
    SAS     -   active set structure which stores current point in SAS.XC
    D       -   direction for step
    Stp     -   step length for "default" candidate
    NeedAct -   whether   default  candidate  activates  some  constraint;
                if NeedAct  is True,  constraint  given  by  CIdc/CVal  is
                GUARANTEED to be activated in the final point.
    CIdx    -   if NeedAct is True, stores index of the constraint to activate
    CVal    -   if NeedAct is True, stores constrained value;
                SAS.XC[CIdx] is forced to be equal to CVal.
    AddSteps-   array[AddStepsCnt] of additional steps:
                * AddSteps[]<=Stp are ignored
                * AddSteps[]>Stp are tried
    Activated-  possibly preallocated buffer; previously allocated memory
                will be reused.
    Tmp0    -  possibly preallocated buffer; previously allocated memory
                will be reused.
    
OUTPUT PARAMETERS:
    SAS     -   SAS.XC is set to new point;  if  there  was  a  constraint
                specified  by  NeedAct/CIdx/CVal,  it  will  be  activated
                (other constraints may be activated too, but this  one  is
                guaranteed to be active in the final point).
    Activated-  elements of this array are set to True, if I-th constraint
                as inactive at previous point, but become  active  in  the
                new one.
                Situations when we deactivate xi>=0 and activate xi<=1 are
                considered as activation of previously inactive constraint
            
  -- ALGLIB --
     Copyright 14.05.2014 by Bochkanov Sergey
*************************************************************************/
static void qqpsolver_findbeststepandmove(qqpbuffers* sstate,
     sactiveset* sas,
     /* Real    */ ae_vector* d,
     double stp,
     ae_bool needact,
     ae_int_t cidx,
     double cval,
     /* Real    */ ae_vector* addsteps,
     ae_int_t addstepscnt,
     /* Boolean */ ae_vector* activated,
     /* Real    */ ae_vector* tmp0,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t k;
    double v;
    double stpbest;
    double fbest;
    double fcand;


    n = sstate->n;
    rvectorsetlengthatleast(tmp0, n, _state);
    bvectorsetlengthatleast(activated, n, _state);
    
    /*
     * Calculate initial step, store to Tmp0
     *
     * NOTE: Tmp0 is guaranteed to be feasible w.r.t. boundary constraints
     */
    for(i=0; i<=n-1; i++)
    {
        v = sas->xc.ptr.p_double[i]+stp*d->ptr.p_double[i];
        if( sstate->havebndl.ptr.p_bool[i]&&ae_fp_less(v,sstate->bndl.ptr.p_double[i]) )
        {
            v = sstate->bndl.ptr.p_double[i];
        }
        if( sstate->havebndu.ptr.p_bool[i]&&ae_fp_greater(v,sstate->bndu.ptr.p_double[i]) )
        {
            v = sstate->bndu.ptr.p_double[i];
        }
        tmp0->ptr.p_double[i] = v;
    }
    if( needact )
    {
        tmp0->ptr.p_double[cidx] = cval;
    }
    
    /*
     * Try additional steps, if AddStepsCnt>0
     */
    if( addstepscnt>0 )
    {
        
        /*
         * Find best step
         */
        stpbest = stp;
        fbest = qqpsolver_projectedtargetfunction(sstate, &sas->xc, d, stpbest, tmp0, _state);
        for(k=0; k<=addstepscnt-1; k++)
        {
            if( ae_fp_greater(addsteps->ptr.p_double[k],stp) )
            {
                fcand = qqpsolver_projectedtargetfunction(sstate, &sas->xc, d, addsteps->ptr.p_double[k], tmp0, _state);
                if( ae_fp_less(fcand,fbest) )
                {
                    fbest = fcand;
                    stpbest = addsteps->ptr.p_double[k];
                }
            }
        }
        
        /*
         * Prepare best step
         *
         * NOTE: because only AddSteps[]>Stp were checked,
         *       this step will activate constraint CIdx.
         */
        for(i=0; i<=n-1; i++)
        {
            v = sas->xc.ptr.p_double[i]+stpbest*d->ptr.p_double[i];
            if( sstate->havebndl.ptr.p_bool[i]&&ae_fp_less(v,sstate->bndl.ptr.p_double[i]) )
            {
                v = sstate->bndl.ptr.p_double[i];
            }
            if( sstate->havebndu.ptr.p_bool[i]&&ae_fp_greater(v,sstate->bndu.ptr.p_double[i]) )
            {
                v = sstate->bndu.ptr.p_double[i];
            }
            tmp0->ptr.p_double[i] = v;
        }
        if( needact )
        {
            tmp0->ptr.p_double[cidx] = cval;
        }
    }
    
    /*
     * Fill Activated array by information about activated constraints.
     * Perform step
     */
    for(i=0; i<=n-1; i++)
    {
        activated->ptr.p_bool[i] = ae_false;
        v = tmp0->ptr.p_double[i];
        if( ae_fp_eq(v,sas->xc.ptr.p_double[i]) )
        {
            continue;
        }
        if( sstate->havebndl.ptr.p_bool[i]&&ae_fp_eq(v,sstate->bndl.ptr.p_double[i]) )
        {
            activated->ptr.p_bool[i] = ae_true;
        }
        if( sstate->havebndu.ptr.p_bool[i]&&ae_fp_eq(v,sstate->bndu.ptr.p_double[i]) )
        {
            activated->ptr.p_bool[i] = ae_true;
        }
    }
    sasmoveto(sas, tmp0, needact, cidx, cval, _state);
}


/*************************************************************************
This function prepares data for  constrained  Newton  step  for  penalized
quadratic model of the form

    f(x) = 0.5*x'*A*x + b'*x + penaltyfactor*0.5*(C*x-b)'*(C*x-b)
    
where A can be dense or sparse, and model is considered subject to equality
constraints specified by SState.SAS.XC  object.  Constraint  is considered
active if XC[i] is exactly BndL[i] or BndU[i],  i.e.  we  ignore  internal
list of constraints monitored by SAS object. Our own  set  of  constraints
includes all  constraints  stored  by  SAS,  but  also  may  include  some
constraints which are inactive in SAS.

"Preparation" means that Cholesky decomposition of  the  effective  system
matrix is performed, and we can  perform  constrained  Newton  step.

This function works as black box. It uses fields of SState which are marked
as "Variables for constrained Newton phase", and only  this  function  and
its friends know about these variables. Everyone else should use:
* CNewtonBuild() to prepare initial Cholesky decomposition for step
* CNewtonStep() to perform constrained Newton step
* CNewtonUpdate() to update Cholesky matrix  after  point  was  moved  and
  constraints were updated. In some cases it  is  possible to  efficiently
  re-calculate Cholesky decomposition if you know which  constraints  were
  activated. If efficient  re-calculation  is  impossible,  this  function
  returns False.

INPUT PARAMETERS:
    SState  -   structure which stores model and temporaries for CN phase;
                in particular, SAS.XC stores current point.
    SparseSolver-which sparse solver to use for sparse model; ignored  for
                dense QP. Can be:
                * 2 -   SKS-based Cholesky
    NCholesky-  counter which is incremented after Cholesky (successful or
                failed one)
    
OUTPUT PARAMETERS:
    NCholesky-  possibly updated counter
    
RESULT:
    True, if Cholesky decomposition was successfully performed.
    False, if a) matrix was semi-definite or indefinite, or b)  particular
    combination of matrix type (sparse) and constraints  (general  linear)
    is not supported.
    
NOTE: this function may routinely return False, for indefinite matrices or
      for sparse problems with general linear constraints. You  should  be
      able to handle such situations.
            
  -- ALGLIB --
     Copyright 14.05.2014 by Bochkanov Sergey
*************************************************************************/
static ae_bool qqpsolver_cnewtonbuild(qqpbuffers* sstate,
     ae_int_t sparsesolver,
     ae_int_t* ncholesky,
     ae_state *_state)
{
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t n;
    ae_int_t nmain;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    ae_bool b;
    ae_int_t ridx0;
    ae_int_t ridx1;
    ae_int_t nfree;
    ae_bool result;


    result = ae_false;
    
    /*
     * Fetch often used fields
     */
    n = sstate->n;
    nmain = sstate->nmain;
    nec = sstate->nec;
    nic = sstate->nic;
    
    /*
     * Check problem properties.
     * Sparse problems with general linear constraints are not supported yet.
     */
    if( sstate->akind==1&&nec+nic>0 )
    {
        return result;
    }
    
    /*
     * 1. Set CNModelAge to zero
     * 2. Generate YIdx - reordering of variables such that free variables
     *    come first and are ordered by ascending, fixed are last ones and
     *    have no particular ordering.
     *
     * This step is same for dense and sparse problems.
     */
    sstate->cnmodelage = 0;
    ivectorsetlengthatleast(&sstate->yidx, n, _state);
    ridx0 = 0;
    ridx1 = n-1;
    for(i=0; i<=n-1; i++)
    {
        sstate->yidx.ptr.p_int[i] = -1;
    }
    for(i=0; i<=n-1; i++)
    {
        ae_assert(!sstate->havebndl.ptr.p_bool[i]||ae_fp_greater_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndl.ptr.p_double[i]), "CNewtonBuild: internal error", _state);
        ae_assert(!sstate->havebndu.ptr.p_bool[i]||ae_fp_less_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndu.ptr.p_double[i]), "CNewtonBuild: internal error", _state);
        b = ae_false;
        b = b||(sstate->havebndl.ptr.p_bool[i]&&ae_fp_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndl.ptr.p_double[i]));
        b = b||(sstate->havebndu.ptr.p_bool[i]&&ae_fp_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndu.ptr.p_double[i]));
        if( b )
        {
            sstate->yidx.ptr.p_int[ridx1] = i;
            ridx1 = ridx1-1;
        }
        else
        {
            sstate->yidx.ptr.p_int[ridx0] = i;
            ridx0 = ridx0+1;
        }
    }
    ae_assert(ridx0==ridx1+1, "CNewtonBuild: internal error", _state);
    nfree = ridx0;
    sstate->nfree = nfree;
    if( nfree==0 )
    {
        return result;
    }
    
    /*
     * Constrained Newton matrix: dense version
     */
    if( sstate->akind==0 )
    {
        rmatrixsetlengthatleast(&sstate->densez, n, n, _state);
        rvectorsetlengthatleast(&sstate->tmpcn, n, _state);
        if( nec+nic>0 )
        {
            
            /*
             * Initialize Z with C'*C, add A
             */
            rmatrixsyrk(n, nec+nic, qqpsolver_penaltyfactor, &sstate->cleic, 0, 0, 2, 0.0, &sstate->densez, 0, 0, ae_true, _state);
            for(i=0; i<=nmain-1; i++)
            {
                for(j=i; j<=nmain-1; j++)
                {
                    sstate->densez.ptr.pp_double[i][j] = sstate->densez.ptr.pp_double[i][j]+sstate->densea.ptr.pp_double[i][j];
                }
            }
        }
        else
        {
            
            /*
             * No linear constraints, just set Z to A
             */
            ae_assert(n==nmain, "CNewtonBuild: integrity check failed", _state);
            for(i=0; i<=nmain-1; i++)
            {
                for(j=i; j<=nmain-1; j++)
                {
                    sstate->densez.ptr.pp_double[i][j] = sstate->densea.ptr.pp_double[i][j];
                }
            }
        }
        for(i=1; i<=nfree-1; i++)
        {
            ae_assert(sstate->yidx.ptr.p_int[i]>sstate->yidx.ptr.p_int[i-1], "CNewtonBuild: integrity check failed", _state);
        }
        for(i=0; i<=nfree-1; i++)
        {
            k = sstate->yidx.ptr.p_int[i];
            for(j=i; j<=nfree-1; j++)
            {
                sstate->densez.ptr.pp_double[i][j] = sstate->densez.ptr.pp_double[k][sstate->yidx.ptr.p_int[j]];
            }
        }
        rvectorsetlengthatleast(&sstate->regdiag, n, _state);
        for(i=0; i<=nfree-1; i++)
        {
            v = 0.0;
            for(j=0; j<=i-1; j++)
            {
                v = v+ae_fabs(sstate->densez.ptr.pp_double[j][i], _state);
            }
            for(j=i; j<=nfree-1; j++)
            {
                v = v+ae_fabs(sstate->densez.ptr.pp_double[i][j], _state);
            }
            if( ae_fp_eq(v,(double)(0)) )
            {
                v = 1.0;
            }
            sstate->regdiag.ptr.p_double[i] = qqpsolver_regz*v;
        }
        for(i=0; i<=nfree-1; i++)
        {
            sstate->densez.ptr.pp_double[i][i] = sstate->densez.ptr.pp_double[i][i]+sstate->regdiag.ptr.p_double[i];
        }
        inc(ncholesky, _state);
        if( !spdmatrixcholeskyrec(&sstate->densez, 0, nfree, ae_true, &sstate->tmpcn, _state) )
        {
            return result;
        }
        for(i=nfree-1; i>=0; i--)
        {
            ae_v_move(&sstate->tmpcn.ptr.p_double[i], 1, &sstate->densez.ptr.pp_double[i][i], 1, ae_v_len(i,nfree-1));
            k = sstate->yidx.ptr.p_int[i];
            for(j=k; j<=n-1; j++)
            {
                sstate->densez.ptr.pp_double[k][j] = (double)(0);
            }
            for(j=i; j<=nfree-1; j++)
            {
                sstate->densez.ptr.pp_double[k][sstate->yidx.ptr.p_int[j]] = sstate->tmpcn.ptr.p_double[j];
            }
        }
        for(i=nfree; i<=n-1; i++)
        {
            k = sstate->yidx.ptr.p_int[i];
            sstate->densez.ptr.pp_double[k][k] = 1.0;
            for(j=k+1; j<=n-1; j++)
            {
                sstate->densez.ptr.pp_double[k][j] = (double)(0);
            }
        }
        result = ae_true;
        return result;
    }
    
    /*
     * Constrained Newton matrix: sparse version
     */
    if( sstate->akind==1 )
    {
        ae_assert(nec+nic==0, "CNewtonBuild: internal error", _state);
        ae_assert(sparsesolver==2, "CNewtonBuild: internal error", _state);
        
        /*
         * Copy sparse A to Z and fill rows/columns corresponding to active
         * constraints by zeros. Diagonal elements corresponding to active
         * constraints are filled by unit values.
         */
        sparsecopytosksbuf(&sstate->sparsea, &sstate->sparsecca, _state);
        rvectorsetlengthatleast(&sstate->tmpcn, n, _state);
        for(i=0; i<=n-1; i++)
        {
            sstate->tmpcn.ptr.p_double[i] = (double)(0);
        }
        for(i=nfree; i<=n-1; i++)
        {
            sstate->tmpcn.ptr.p_double[sstate->yidx.ptr.p_int[i]] = (double)(1);
        }
        for(i=0; i<=n-1; i++)
        {
            k = sstate->sparsecca.ridx.ptr.p_int[i];
            for(j=i-sstate->sparsecca.didx.ptr.p_int[i]; j<=i; j++)
            {
                if( ae_fp_neq(sstate->tmpcn.ptr.p_double[i],(double)(0))||ae_fp_neq(sstate->tmpcn.ptr.p_double[j],(double)(0)) )
                {
                    
                    /*
                     * I-th or J-th variable is in active set (constrained)
                     */
                    if( i==j )
                    {
                        sstate->sparsecca.vals.ptr.p_double[k] = 1.0;
                    }
                    else
                    {
                        sstate->sparsecca.vals.ptr.p_double[k] = 0.0;
                    }
                }
                k = k+1;
            }
        }
        
        /*
         * Perform sparse Cholesky
         */
        inc(ncholesky, _state);
        if( !sparsecholeskyskyline(&sstate->sparsecca, nmain, sstate->sparseupper, _state) )
        {
            return result;
        }
        result = ae_true;
        return result;
    }
    
    /*
     * Unexpected :)
     */
    ae_assert(ae_false, "CNewtonBuild: internal error", _state);
    return result;
}


/*************************************************************************
This   function  updates  equality-constrained   Cholesky   matrix   after
activation of the  new  equality  constraints.  Matrix  being  updated  is
quadratic term of the function below

    f(x) = 0.5*x'*A*x + b'*x + penaltyfactor*0.5*(C*x-b)'*(C*x-b)
    
where A can be dense or sparse.

This  function  uses  YIdx[]  array  (set by CNewtonBuild()  function)  to
distinguish between active and inactive constraints.

This function works as black box. It uses fields of SState which are marked
as "Variables for constrained Newton phase", and only  this  function  and
its friends know about these variables. Everyone else should use:
* CNewtonBuild() to prepare initial Cholesky decomposition for step
* CNewtonStep() to perform constrained Newton step
* CNewtonUpdate() to update Cholesky matrix  after  point  was  moved  and
  constraints were updated. In some cases it  is  possible to  efficiently
  re-calculate Cholesky decomposition if you know which  constraints  were
  activated. If efficient  re-calculation  is  impossible,  this  function
  returns False.

INPUT PARAMETERS:
    SState  -   structure which stores model and temporaries for CN phase;
                in particular, SAS.XC stores current point.
    Settings -  QQPSettings object which was  initialized  by  appropriate
                construction function.
    NCUpdates-  counter which is incremented after each update (one update
                means one variable being fixed)
    
OUTPUT PARAMETERS:
    NCUpdates-  possibly updated counter
    
RESULT:
    True, if Cholesky decomposition was successfully performed.
    False, if a) model age was too high, or b) particular  combination  of
    matrix type (sparse) and constraints (general linear) is not supported
    
NOTE: this function may routinely return False.
      You should be able to handle such situations.
            
  -- ALGLIB --
     Copyright 14.05.2014 by Bochkanov Sergey
*************************************************************************/
static ae_bool qqpsolver_cnewtonupdate(qqpbuffers* sstate,
     qqpsettings* settings,
     ae_int_t* ncupdates,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t nfree;
    ae_int_t ntofix;
    ae_bool b;
    ae_int_t ridx0;
    ae_int_t ridx1;
    ae_int_t i;
    ae_int_t k;
    ae_bool result;


    result = ae_false;
    
    /*
     * Cholesky updates for sparse problems are not supported
     */
    if( sstate->akind==1 )
    {
        return result;
    }
    
    /*
     * Fetch often used fields
     */
    n = sstate->n;
    nfree = sstate->nfree;
    
    /*
     * Determine variables to fix and move them to YIdx[NFree-NToFix:NFree-1]
     * Exit if CNModelAge increased too much.
     */
    ivectorsetlengthatleast(&sstate->tmpcni, n, _state);
    ridx0 = 0;
    ridx1 = nfree-1;
    for(i=0; i<=nfree-1; i++)
    {
        sstate->tmpcni.ptr.p_int[i] = -1;
    }
    for(k=0; k<=nfree-1; k++)
    {
        i = sstate->yidx.ptr.p_int[k];
        ae_assert(!sstate->havebndl.ptr.p_bool[i]||ae_fp_greater_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndl.ptr.p_double[i]), "CNewtonUpdate: internal error", _state);
        ae_assert(!sstate->havebndu.ptr.p_bool[i]||ae_fp_less_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndu.ptr.p_double[i]), "CNewtonUpdate: internal error", _state);
        b = ae_false;
        b = b||(sstate->havebndl.ptr.p_bool[i]&&ae_fp_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndl.ptr.p_double[i]));
        b = b||(sstate->havebndu.ptr.p_bool[i]&&ae_fp_eq(sstate->sas.xc.ptr.p_double[i],sstate->bndu.ptr.p_double[i]));
        if( b )
        {
            sstate->tmpcni.ptr.p_int[ridx1] = i;
            ridx1 = ridx1-1;
        }
        else
        {
            sstate->tmpcni.ptr.p_int[ridx0] = i;
            ridx0 = ridx0+1;
        }
    }
    ae_assert(ridx0==ridx1+1, "CNewtonUpdate: internal error", _state);
    ntofix = nfree-ridx0;
    if( ntofix==0||ntofix==nfree )
    {
        return result;
    }
    if( sstate->cnmodelage+ntofix>settings->cnmaxupdates )
    {
        return result;
    }
    for(i=0; i<=nfree-1; i++)
    {
        sstate->yidx.ptr.p_int[i] = sstate->tmpcni.ptr.p_int[i];
    }
    
    /*
     * Constrained Newton matrix: dense version.
     */
    if( sstate->akind==0 )
    {
        
        /*
         * Update Cholesky matrix with SPDMatrixCholeskyUpdateFixBuf()
         */
        bvectorsetlengthatleast(&sstate->tmpcnb, n, _state);
        for(i=0; i<=n-1; i++)
        {
            sstate->tmpcnb.ptr.p_bool[i] = ae_false;
        }
        for(i=nfree-ntofix; i<=nfree-1; i++)
        {
            sstate->tmpcnb.ptr.p_bool[sstate->yidx.ptr.p_int[i]] = ae_true;
        }
        spdmatrixcholeskyupdatefixbuf(&sstate->densez, n, ae_true, &sstate->tmpcnb, &sstate->tmpcn, _state);
        
        /*
         * Update information stored in State and exit
         */
        sstate->nfree = nfree-ntofix;
        sstate->cnmodelage = sstate->cnmodelage+ntofix;
        *ncupdates = *ncupdates+ntofix;
        result = ae_true;
        return result;
    }
    
    /*
     * Unexpected :)
     */
    ae_assert(ae_false, "CNewtonUpdate: internal error", _state);
    return result;
}


/*************************************************************************
This   function prepares equality-constrained Newton step using previously
calculated constrained Cholesky matrix of the problem

    f(x) = 0.5*x'*A*x + b'*x + penaltyfactor*0.5*(C*x-b)'*(C*x-b)
    
where A can be dense or sparse.

As  input,  this  function  accepts  gradient  at the current location. As
output, it returns step vector (replaces gradient).

This function works as black box. It uses fields of SState which are marked
as "Variables for constrained Newton phase", and only  this  function  and
its friends know about these variables. Everyone else should use:
* CNewtonBuild() to prepare initial Cholesky decomposition for step
* CNewtonStep() to perform constrained Newton step
* CNewtonUpdate() to update Cholesky matrix  after  point  was  moved  and
  constraints were updated. In some cases it  is  possible to  efficiently
  re-calculate Cholesky decomposition if you know which  constraints  were
  activated. If efficient  re-calculation  is  impossible,  this  function
  returns False.

INPUT PARAMETERS:
    SState  -   structure which stores model and temporaries for CN phase;
                in particular, SAS.XC stores current point.
    Settings -  QQPSettings object which was  initialized  by  appropriate
                construction function.
    GC       -  array[NMain+NSlack], gradient of the target function
    
OUTPUT PARAMETERS:
    GC       -  array[NMain+NSlack], step vector (on success)
    
RESULT:
    True, if step was successfully calculated.
    False, if step calculation failed:
    a) gradient was exactly zero,
    b) gradient norm was smaller than EpsG (stopping condition)
    c) all variables were equality-constrained
    
NOTE: this function may routinely return False.
      You should be able to handle such situations.
            
  -- ALGLIB --
     Copyright 14.05.2014 by Bochkanov Sergey
*************************************************************************/
static ae_bool qqpsolver_cnewtonstep(qqpbuffers* sstate,
     qqpsettings* settings,
     /* Real    */ ae_vector* gc,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t n;
    ae_int_t nfree;
    double v;
    ae_bool result;


    result = ae_false;
    n = sstate->n;
    nfree = sstate->nfree;
    for(i=nfree; i<=n-1; i++)
    {
        gc->ptr.p_double[sstate->yidx.ptr.p_int[i]] = 0.0;
    }
    v = ae_v_dotproduct(&gc->ptr.p_double[0], 1, &gc->ptr.p_double[0], 1, ae_v_len(0,n-1));
    if( ae_fp_less_eq(ae_sqrt(v, _state),settings->epsg) )
    {
        return result;
    }
    for(i=0; i<=n-1; i++)
    {
        gc->ptr.p_double[i] = -gc->ptr.p_double[i];
    }
    if( sstate->akind==0 )
    {
        
        /*
         * Dense Newton step.
         * Use straightforward Cholesky solver.
         */
        fblscholeskysolve(&sstate->densez, 1.0, n, ae_true, gc, &sstate->tmpcn, _state);
        result = ae_true;
        return result;
    }
    if( sstate->akind==1 )
    {
        
        /*
         * Sparse Newton step.
         *
         * We have T*T' = L*L' = U'*U (depending on specific triangle stored in SparseCCA).
         */
        if( sstate->sparseupper )
        {
            sparsetrsv(&sstate->sparsecca, sstate->sparseupper, ae_false, 1, gc, _state);
            sparsetrsv(&sstate->sparsecca, sstate->sparseupper, ae_false, 0, gc, _state);
        }
        else
        {
            sparsetrsv(&sstate->sparsecca, sstate->sparseupper, ae_false, 0, gc, _state);
            sparsetrsv(&sstate->sparsecca, sstate->sparseupper, ae_false, 1, gc, _state);
        }
        result = ae_true;
        return result;
    }
    ae_assert(ae_false, "CNewtonStep: internal error", _state);
    return result;
}


void _qqpsettings_init(void* _p, ae_state *_state)
{
    qqpsettings *p = (qqpsettings*)_p;
    ae_touch_ptr((void*)p);
}


void _qqpsettings_init_copy(void* _dst, void* _src, ae_state *_state)
{
    qqpsettings *dst = (qqpsettings*)_dst;
    qqpsettings *src = (qqpsettings*)_src;
    dst->epsg = src->epsg;
    dst->epsf = src->epsf;
    dst->epsx = src->epsx;
    dst->maxouterits = src->maxouterits;
    dst->cgphase = src->cgphase;
    dst->cnphase = src->cnphase;
    dst->cgminits = src->cgminits;
    dst->cgmaxits = src->cgmaxits;
    dst->cnmaxupdates = src->cnmaxupdates;
    dst->sparsesolver = src->sparsesolver;
}


void _qqpsettings_clear(void* _p)
{
    qqpsettings *p = (qqpsettings*)_p;
    ae_touch_ptr((void*)p);
}


void _qqpsettings_destroy(void* _p)
{
    qqpsettings *p = (qqpsettings*)_p;
    ae_touch_ptr((void*)p);
}


void _qqpbuffers_init(void* _p, ae_state *_state)
{
    qqpbuffers *p = (qqpbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->densea, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&p->sparsea, _state);
    ae_vector_init(&p->b, 0, DT_REAL, _state);
    ae_vector_init(&p->bndl, 0, DT_REAL, _state);
    ae_vector_init(&p->bndu, 0, DT_REAL, _state);
    ae_vector_init(&p->havebndl, 0, DT_BOOL, _state);
    ae_vector_init(&p->havebndu, 0, DT_BOOL, _state);
    ae_matrix_init(&p->cleic, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->xs, 0, DT_REAL, _state);
    ae_vector_init(&p->gc, 0, DT_REAL, _state);
    ae_vector_init(&p->xp, 0, DT_REAL, _state);
    ae_vector_init(&p->dc, 0, DT_REAL, _state);
    ae_vector_init(&p->dp, 0, DT_REAL, _state);
    ae_vector_init(&p->cgc, 0, DT_REAL, _state);
    ae_vector_init(&p->cgp, 0, DT_REAL, _state);
    _sactiveset_init(&p->sas, _state);
    ae_vector_init(&p->activated, 0, DT_BOOL, _state);
    ae_matrix_init(&p->densez, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&p->sparsecca, _state);
    ae_vector_init(&p->yidx, 0, DT_INT, _state);
    ae_vector_init(&p->regdiag, 0, DT_REAL, _state);
    ae_vector_init(&p->regx0, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpcn, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpcni, 0, DT_INT, _state);
    ae_vector_init(&p->tmpcnb, 0, DT_BOOL, _state);
    ae_vector_init(&p->tmp0, 0, DT_REAL, _state);
    ae_vector_init(&p->stpbuf, 0, DT_REAL, _state);
    _sparsebuffers_init(&p->sbuf, _state);
}


void _qqpbuffers_init_copy(void* _dst, void* _src, ae_state *_state)
{
    qqpbuffers *dst = (qqpbuffers*)_dst;
    qqpbuffers *src = (qqpbuffers*)_src;
    dst->n = src->n;
    dst->nmain = src->nmain;
    dst->nslack = src->nslack;
    dst->nec = src->nec;
    dst->nic = src->nic;
    dst->akind = src->akind;
    ae_matrix_init_copy(&dst->densea, &src->densea, _state);
    _sparsematrix_init_copy(&dst->sparsea, &src->sparsea, _state);
    dst->sparseupper = src->sparseupper;
    dst->absamax = src->absamax;
    dst->absasum = src->absasum;
    dst->absasum2 = src->absasum2;
    ae_vector_init_copy(&dst->b, &src->b, _state);
    ae_vector_init_copy(&dst->bndl, &src->bndl, _state);
    ae_vector_init_copy(&dst->bndu, &src->bndu, _state);
    ae_vector_init_copy(&dst->havebndl, &src->havebndl, _state);
    ae_vector_init_copy(&dst->havebndu, &src->havebndu, _state);
    ae_matrix_init_copy(&dst->cleic, &src->cleic, _state);
    ae_vector_init_copy(&dst->xs, &src->xs, _state);
    ae_vector_init_copy(&dst->gc, &src->gc, _state);
    ae_vector_init_copy(&dst->xp, &src->xp, _state);
    ae_vector_init_copy(&dst->dc, &src->dc, _state);
    ae_vector_init_copy(&dst->dp, &src->dp, _state);
    ae_vector_init_copy(&dst->cgc, &src->cgc, _state);
    ae_vector_init_copy(&dst->cgp, &src->cgp, _state);
    _sactiveset_init_copy(&dst->sas, &src->sas, _state);
    ae_vector_init_copy(&dst->activated, &src->activated, _state);
    dst->nfree = src->nfree;
    dst->cnmodelage = src->cnmodelage;
    ae_matrix_init_copy(&dst->densez, &src->densez, _state);
    _sparsematrix_init_copy(&dst->sparsecca, &src->sparsecca, _state);
    ae_vector_init_copy(&dst->yidx, &src->yidx, _state);
    ae_vector_init_copy(&dst->regdiag, &src->regdiag, _state);
    ae_vector_init_copy(&dst->regx0, &src->regx0, _state);
    ae_vector_init_copy(&dst->tmpcn, &src->tmpcn, _state);
    ae_vector_init_copy(&dst->tmpcni, &src->tmpcni, _state);
    ae_vector_init_copy(&dst->tmpcnb, &src->tmpcnb, _state);
    ae_vector_init_copy(&dst->tmp0, &src->tmp0, _state);
    ae_vector_init_copy(&dst->stpbuf, &src->stpbuf, _state);
    _sparsebuffers_init_copy(&dst->sbuf, &src->sbuf, _state);
    dst->repinneriterationscount = src->repinneriterationscount;
    dst->repouteriterationscount = src->repouteriterationscount;
    dst->repncholesky = src->repncholesky;
    dst->repncupdates = src->repncupdates;
}


void _qqpbuffers_clear(void* _p)
{
    qqpbuffers *p = (qqpbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->densea);
    _sparsematrix_clear(&p->sparsea);
    ae_vector_clear(&p->b);
    ae_vector_clear(&p->bndl);
    ae_vector_clear(&p->bndu);
    ae_vector_clear(&p->havebndl);
    ae_vector_clear(&p->havebndu);
    ae_matrix_clear(&p->cleic);
    ae_vector_clear(&p->xs);
    ae_vector_clear(&p->gc);
    ae_vector_clear(&p->xp);
    ae_vector_clear(&p->dc);
    ae_vector_clear(&p->dp);
    ae_vector_clear(&p->cgc);
    ae_vector_clear(&p->cgp);
    _sactiveset_clear(&p->sas);
    ae_vector_clear(&p->activated);
    ae_matrix_clear(&p->densez);
    _sparsematrix_clear(&p->sparsecca);
    ae_vector_clear(&p->yidx);
    ae_vector_clear(&p->regdiag);
    ae_vector_clear(&p->regx0);
    ae_vector_clear(&p->tmpcn);
    ae_vector_clear(&p->tmpcni);
    ae_vector_clear(&p->tmpcnb);
    ae_vector_clear(&p->tmp0);
    ae_vector_clear(&p->stpbuf);
    _sparsebuffers_clear(&p->sbuf);
}


void _qqpbuffers_destroy(void* _p)
{
    qqpbuffers *p = (qqpbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->densea);
    _sparsematrix_destroy(&p->sparsea);
    ae_vector_destroy(&p->b);
    ae_vector_destroy(&p->bndl);
    ae_vector_destroy(&p->bndu);
    ae_vector_destroy(&p->havebndl);
    ae_vector_destroy(&p->havebndu);
    ae_matrix_destroy(&p->cleic);
    ae_vector_destroy(&p->xs);
    ae_vector_destroy(&p->gc);
    ae_vector_destroy(&p->xp);
    ae_vector_destroy(&p->dc);
    ae_vector_destroy(&p->dp);
    ae_vector_destroy(&p->cgc);
    ae_vector_destroy(&p->cgp);
    _sactiveset_destroy(&p->sas);
    ae_vector_destroy(&p->activated);
    ae_matrix_destroy(&p->densez);
    _sparsematrix_destroy(&p->sparsecca);
    ae_vector_destroy(&p->yidx);
    ae_vector_destroy(&p->regdiag);
    ae_vector_destroy(&p->regx0);
    ae_vector_destroy(&p->tmpcn);
    ae_vector_destroy(&p->tmpcni);
    ae_vector_destroy(&p->tmpcnb);
    ae_vector_destroy(&p->tmp0);
    ae_vector_destroy(&p->stpbuf);
    _sparsebuffers_destroy(&p->sbuf);
}


/*$ End $*/
