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
#include "qpbleicsolver.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
This function initializes QPBLEICSettings structure with default settings.

Newly created structure MUST be initialized by default settings  -  or  by
copy of the already initialized structure.

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qpbleicloaddefaults(ae_int_t nmain,
     qpbleicsettings* s,
     ae_state *_state)
{


    s->epsg = 0.0;
    s->epsf = 0.0;
    s->epsx = 1.0E-6;
    s->maxits = 0;
}


/*************************************************************************
This function initializes QPBLEICSettings  structure  with  copy  of  another,
already initialized structure.

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qpbleiccopysettings(qpbleicsettings* src,
     qpbleicsettings* dst,
     ae_state *_state)
{


    dst->epsg = src->epsg;
    dst->epsf = src->epsf;
    dst->epsx = src->epsx;
    dst->maxits = src->maxits;
}


/*************************************************************************
This function runs QPBLEIC solver; it returns after optimization   process
was completed. Following QP problem is solved:

    min(0.5*(x-x_origin)'*A*(x-x_origin)+b'*(x-x_origin))
    
subject to boundary constraints.

INPUT PARAMETERS:
    AC          -   for dense problems (AKind=0), A-term of CQM object
                    contains system matrix. Other terms are unspecified
                    and should not be referenced.
    SparseAC    -   for sparse problems (AKind=1
    AKind       -   sparse matrix format:
                    * 0 for dense matrix
                    * 1 for sparse matrix
    SparseUpper -   which triangle of SparseAC stores matrix  -  upper  or
                    lower one (for dense matrices this  parameter  is  not
                    actual).
    AbsASum     -   SUM(|A[i,j]|)
    AbsASum2    -   SUM(A[i,j]^2)
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
    Settings    -   QPBLEICSettings object initialized by one of the initialization
                    functions.
    SState      -   object which stores temporaries:
                    * if uninitialized object was passed, FirstCall parameter MUST
                      be set to True; object will be automatically initialized by the
                      function, and FirstCall will be set to False.
                    * if FirstCall=False, it is assumed that this parameter was already
                      initialized by previous call to this function with same
                      problem dimensions (variable count N).
    FirstCall   -   whether it is first call of this function for this specific
                    instance of SState, with this number of variables N specified.
    XS          -   initial point, array[NC]
    
    
OUTPUT PARAMETERS:
    XS          -   last point
    FirstCall   -   uncondtionally set to False
    TerminationType-termination type:
                    *
                    *
                    *

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qpbleicoptimize(convexquadraticmodel* a,
     sparsematrix* sparsea,
     ae_int_t akind,
     ae_bool sparseaupper,
     double absasum,
     double absasum2,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     /* Real    */ ae_vector* s,
     /* Real    */ ae_vector* xorigin,
     ae_int_t n,
     /* Real    */ ae_matrix* cleic,
     ae_int_t nec,
     ae_int_t nic,
     qpbleicsettings* settings,
     qpbleicbuffers* sstate,
     ae_bool* firstcall,
     /* Real    */ ae_vector* xs,
     ae_int_t* terminationtype,
     ae_state *_state)
{
    ae_int_t i;
    double d2;
    double d1;
    double d0;
    double v;
    double v0;
    double v1;
    double md;
    double mx;
    double mb;
    ae_int_t d1est;
    ae_int_t d2est;

    *terminationtype = 0;

    ae_assert(akind==0||akind==1, "QPBLEICOptimize: unexpected AKind", _state);
    sstate->repinneriterationscount = 0;
    sstate->repouteriterationscount = 0;
    *terminationtype = 0;
    
    /*
     * Prepare solver object, if needed
     */
    if( *firstcall )
    {
        minbleiccreate(n, xs, &sstate->solver, _state);
        *firstcall = ae_false;
    }
    
    /*
     * Prepare max(|B|)
     */
    mb = 0.0;
    for(i=0; i<=n-1; i++)
    {
        mb = ae_maxreal(mb, ae_fabs(b->ptr.p_double[i], _state), _state);
    }
    
    /*
     * Temporaries
     */
    ivectorsetlengthatleast(&sstate->tmpi, nec+nic, _state);
    rvectorsetlengthatleast(&sstate->tmp0, n, _state);
    rvectorsetlengthatleast(&sstate->tmp1, n, _state);
    for(i=0; i<=nec-1; i++)
    {
        sstate->tmpi.ptr.p_int[i] = 0;
    }
    for(i=0; i<=nic-1; i++)
    {
        sstate->tmpi.ptr.p_int[nec+i] = -1;
    }
    minbleicsetlc(&sstate->solver, cleic, &sstate->tmpi, nec+nic, _state);
    minbleicsetbc(&sstate->solver, bndl, bndu, _state);
    minbleicsetdrep(&sstate->solver, ae_true, _state);
    minbleicsetcond(&sstate->solver, ae_minrealnumber, 0.0, 0.0, settings->maxits, _state);
    minbleicsetscale(&sstate->solver, s, _state);
    minbleicsetprecscale(&sstate->solver, _state);
    minbleicrestartfrom(&sstate->solver, xs, _state);
    while(minbleiciteration(&sstate->solver, _state))
    {
        
        /*
         * Line search started
         */
        if( sstate->solver.lsstart )
        {
            
            /*
             * Iteration counters:
             * * inner iterations count is increased on every line search
             * * outer iterations count is increased only at steepest descent line search
             */
            inc(&sstate->repinneriterationscount, _state);
            if( sstate->solver.steepestdescentstep )
            {
                inc(&sstate->repouteriterationscount, _state);
            }
            
            /*
             * Build quadratic model of F along descent direction:
             *
             *     F(x+alpha*d) = D2*alpha^2 + D1*alpha + D0
             *
             * Calculate estimates of linear and quadratic term
             * (term magnitude is compared with magnitude of numerical errors)
             */
            d0 = sstate->solver.f;
            d1 = ae_v_dotproduct(&sstate->solver.d.ptr.p_double[0], 1, &sstate->solver.g.ptr.p_double[0], 1, ae_v_len(0,n-1));
            d2 = (double)(0);
            if( akind==0 )
            {
                d2 = cqmxtadx2(a, &sstate->solver.d, _state);
            }
            if( akind==1 )
            {
                sparsesmv(sparsea, sparseaupper, &sstate->solver.d, &sstate->tmp0, _state);
                d2 = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    d2 = d2+sstate->solver.d.ptr.p_double[i]*sstate->tmp0.ptr.p_double[i];
                }
                d2 = 0.5*d2;
            }
            mx = 0.0;
            md = 0.0;
            for(i=0; i<=n-1; i++)
            {
                mx = ae_maxreal(mx, ae_fabs(sstate->solver.x.ptr.p_double[i], _state), _state);
                md = ae_maxreal(md, ae_fabs(sstate->solver.d.ptr.p_double[i], _state), _state);
            }
            estimateparabolicmodel(absasum, absasum2, mx, mb, md, d1, d2, &d1est, &d2est, _state);
            
            /*
             * Tests for "normal" convergence.
             *
             * This line search may be started from steepest descent
             * stage (stage 2) or from L-BFGS stage (stage 3) of the
             * BLEIC algorithm. Depending on stage type, different
             * checks are performed.
             *
             * Say, L-BFGS stage is an equality-constrained refinement
             * stage of BLEIC. This stage refines current iterate
             * under "frozen" equality constraints. We can terminate
             * iterations at this stage only when we encounter
             * unconstrained direction of negative curvature. In all
             * other cases (say, when constrained gradient is zero)
             * we should not terminate algorithm because everything may
             * change after de-activating presently active constraints.
             *
             * Tests for convergence are performed only at "steepest descent" stage
             * of the BLEIC algorithm, and only when function is non-concave
             * (D2 is positive or approximately zero) along direction D.
             *
             * NOTE: we do not test iteration count (MaxIts) here, because
             *       this stopping condition is tested by BLEIC itself.
             */
            if( sstate->solver.steepestdescentstep&&d2est>=0 )
            {
                if( d1est>=0 )
                {
                    
                    /*
                     * "Emergency" stopping condition: D is non-descent direction.
                     * Sometimes it is possible because of numerical noise in the
                     * target function.
                     */
                    *terminationtype = 4;
                    for(i=0; i<=n-1; i++)
                    {
                        xs->ptr.p_double[i] = sstate->solver.x.ptr.p_double[i];
                    }
                    break;
                }
                if( d2est>0 )
                {
                    
                    /*
                     * Stopping condition #4 - gradient norm is small:
                     *
                     * 1. rescale State.Solver.D and State.Solver.G according to
                     *    current scaling, store results to Tmp0 and Tmp1.
                     * 2. Normalize Tmp0 (scaled direction vector).
                     * 3. compute directional derivative (in scaled variables),
                     *    which is equal to DOTPRODUCT(Tmp0,Tmp1).
                     */
                    v = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        sstate->tmp0.ptr.p_double[i] = sstate->solver.d.ptr.p_double[i]/s->ptr.p_double[i];
                        sstate->tmp1.ptr.p_double[i] = sstate->solver.g.ptr.p_double[i]*s->ptr.p_double[i];
                        v = v+ae_sqr(sstate->tmp0.ptr.p_double[i], _state);
                    }
                    ae_assert(ae_fp_greater(v,(double)(0)), "QPBLEICOptimize: inernal errror (scaled direction is zero)", _state);
                    v = 1/ae_sqrt(v, _state);
                    ae_v_muld(&sstate->tmp0.ptr.p_double[0], 1, ae_v_len(0,n-1), v);
                    v = ae_v_dotproduct(&sstate->tmp0.ptr.p_double[0], 1, &sstate->tmp1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    if( ae_fp_less_eq(ae_fabs(v, _state),settings->epsg) )
                    {
                        *terminationtype = 4;
                        for(i=0; i<=n-1; i++)
                        {
                            xs->ptr.p_double[i] = sstate->solver.x.ptr.p_double[i];
                        }
                        break;
                    }
                    
                    /*
                     * Stopping condition #1 - relative function improvement is small:
                     *
                     * 1. calculate steepest descent step:   V = -D1/(2*D2)
                     * 2. calculate function change:         V1= D2*V^2 + D1*V
                     * 3. stop if function change is small enough
                     */
                    v = -d1/(2*d2);
                    v1 = d2*v*v+d1*v;
                    if( ae_fp_less_eq(ae_fabs(v1, _state),settings->epsf*ae_maxreal(d0, 1.0, _state)) )
                    {
                        *terminationtype = 1;
                        for(i=0; i<=n-1; i++)
                        {
                            xs->ptr.p_double[i] = sstate->solver.x.ptr.p_double[i];
                        }
                        break;
                    }
                    
                    /*
                     * Stopping condition #2 - scaled step is small:
                     *
                     * 1. calculate step multiplier V0 (step itself is D*V0)
                     * 2. calculate scaled step length V
                     * 3. stop if step is small enough
                     */
                    v0 = -d1/(2*d2);
                    v = (double)(0);
                    for(i=0; i<=n-1; i++)
                    {
                        v = v+ae_sqr(v0*sstate->solver.d.ptr.p_double[i]/s->ptr.p_double[i], _state);
                    }
                    if( ae_fp_less_eq(ae_sqrt(v, _state),settings->epsx) )
                    {
                        *terminationtype = 2;
                        for(i=0; i<=n-1; i++)
                        {
                            xs->ptr.p_double[i] = sstate->solver.x.ptr.p_double[i];
                        }
                        break;
                    }
                }
            }
            
            /*
             * Test for unconstrained direction of negative curvature
             */
            if( (d2est<0||(d2est==0&&d1est<0))&&!sstate->solver.boundedstep )
            {
                
                /*
                 * Function is unbounded from below:
                 * * function will decrease along D, i.e. either:
                 *   * D2<0
                 *   * D2=0 and D1<0
                 * * step is unconstrained
                 *
                 * If these conditions are true, we abnormally terminate QP
                 * algorithm with return code -4 (we can do so at any stage
                 * of BLEIC - whether it is L-BFGS or steepest descent one).
                 */
                *terminationtype = -4;
                for(i=0; i<=n-1; i++)
                {
                    xs->ptr.p_double[i] = sstate->solver.x.ptr.p_double[i];
                }
                break;
            }
            
            /*
             * Suggest new step (only if D1 is negative far away from zero,
             * D2 is positive far away from zero).
             */
            if( d1est<0&&d2est>0 )
            {
                sstate->solver.stp = safeminposrv(-d1, 2*d2, sstate->solver.curstpmax, _state);
            }
        }
        
        /*
         * Gradient evaluation
         */
        if( sstate->solver.needfg )
        {
            for(i=0; i<=n-1; i++)
            {
                sstate->tmp0.ptr.p_double[i] = sstate->solver.x.ptr.p_double[i]-xorigin->ptr.p_double[i];
            }
            if( akind==0 )
            {
                cqmadx(a, &sstate->tmp0, &sstate->tmp1, _state);
            }
            if( akind==1 )
            {
                sparsesmv(sparsea, sparseaupper, &sstate->tmp0, &sstate->tmp1, _state);
            }
            v0 = ae_v_dotproduct(&sstate->tmp0.ptr.p_double[0], 1, &sstate->tmp1.ptr.p_double[0], 1, ae_v_len(0,n-1));
            v1 = ae_v_dotproduct(&sstate->tmp0.ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
            sstate->solver.f = 0.5*v0+v1;
            ae_v_move(&sstate->solver.g.ptr.p_double[0], 1, &sstate->tmp1.ptr.p_double[0], 1, ae_v_len(0,n-1));
            ae_v_add(&sstate->solver.g.ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
        }
    }
    if( *terminationtype==0 )
    {
        
        /*
         * BLEIC optimizer was terminated by one of its inner stopping
         * conditions. Usually it is iteration counter (if such
         * stopping condition was specified by user).
         */
        minbleicresultsbuf(&sstate->solver, xs, &sstate->solverrep, _state);
        *terminationtype = sstate->solverrep.terminationtype;
    }
    else
    {
        
        /*
         * BLEIC optimizer was terminated in "emergency" mode by QP
         * solver.
         *
         * NOTE: such termination is "emergency" only when viewed from
         *       BLEIC's position. QP solver sees such termination as
         *       routine one, triggered by QP's stopping criteria.
         */
        minbleicemergencytermination(&sstate->solver, _state);
    }
}


void _qpbleicsettings_init(void* _p, ae_state *_state)
{
    qpbleicsettings *p = (qpbleicsettings*)_p;
    ae_touch_ptr((void*)p);
}


void _qpbleicsettings_init_copy(void* _dst, void* _src, ae_state *_state)
{
    qpbleicsettings *dst = (qpbleicsettings*)_dst;
    qpbleicsettings *src = (qpbleicsettings*)_src;
    dst->epsg = src->epsg;
    dst->epsf = src->epsf;
    dst->epsx = src->epsx;
    dst->maxits = src->maxits;
}


void _qpbleicsettings_clear(void* _p)
{
    qpbleicsettings *p = (qpbleicsettings*)_p;
    ae_touch_ptr((void*)p);
}


void _qpbleicsettings_destroy(void* _p)
{
    qpbleicsettings *p = (qpbleicsettings*)_p;
    ae_touch_ptr((void*)p);
}


void _qpbleicbuffers_init(void* _p, ae_state *_state)
{
    qpbleicbuffers *p = (qpbleicbuffers*)_p;
    ae_touch_ptr((void*)p);
    _minbleicstate_init(&p->solver, _state);
    _minbleicreport_init(&p->solverrep, _state);
    ae_vector_init(&p->tmp0, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp1, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpi, 0, DT_INT, _state);
}


void _qpbleicbuffers_init_copy(void* _dst, void* _src, ae_state *_state)
{
    qpbleicbuffers *dst = (qpbleicbuffers*)_dst;
    qpbleicbuffers *src = (qpbleicbuffers*)_src;
    _minbleicstate_init_copy(&dst->solver, &src->solver, _state);
    _minbleicreport_init_copy(&dst->solverrep, &src->solverrep, _state);
    ae_vector_init_copy(&dst->tmp0, &src->tmp0, _state);
    ae_vector_init_copy(&dst->tmp1, &src->tmp1, _state);
    ae_vector_init_copy(&dst->tmpi, &src->tmpi, _state);
    dst->repinneriterationscount = src->repinneriterationscount;
    dst->repouteriterationscount = src->repouteriterationscount;
}


void _qpbleicbuffers_clear(void* _p)
{
    qpbleicbuffers *p = (qpbleicbuffers*)_p;
    ae_touch_ptr((void*)p);
    _minbleicstate_clear(&p->solver);
    _minbleicreport_clear(&p->solverrep);
    ae_vector_clear(&p->tmp0);
    ae_vector_clear(&p->tmp1);
    ae_vector_clear(&p->tmpi);
}


void _qpbleicbuffers_destroy(void* _p)
{
    qpbleicbuffers *p = (qpbleicbuffers*)_p;
    ae_touch_ptr((void*)p);
    _minbleicstate_destroy(&p->solver);
    _minbleicreport_destroy(&p->solverrep);
    ae_vector_destroy(&p->tmp0);
    ae_vector_destroy(&p->tmp1);
    ae_vector_destroy(&p->tmpi);
}


/*$ End $*/
