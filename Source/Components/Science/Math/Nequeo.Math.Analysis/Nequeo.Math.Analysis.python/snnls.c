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
#include "snnls.h"


/*$ Declarations $*/
static void snnls_funcgradu(snnlssolver* s,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* r,
     /* Real    */ ae_vector* g,
     double* f,
     ae_state *_state);
static void snnls_func(snnlssolver* s,
     /* Real    */ ae_vector* x,
     double* f,
     ae_state *_state);
static void snnls_trdprepare(snnlssolver* s,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* diag,
     double lambdav,
     /* Real    */ ae_vector* trdd,
     /* Real    */ ae_matrix* trda,
     /* Real    */ ae_vector* tmp0,
     /* Real    */ ae_vector* tmp1,
     /* Real    */ ae_vector* tmp2,
     /* Real    */ ae_matrix* tmplq,
     ae_state *_state);
static void snnls_trdsolve(/* Real    */ ae_vector* trdd,
     /* Real    */ ae_matrix* trda,
     ae_int_t ns,
     ae_int_t nd,
     /* Real    */ ae_vector* d,
     ae_state *_state);
static void snnls_trdfixvariable(/* Real    */ ae_vector* trdd,
     /* Real    */ ae_matrix* trda,
     ae_int_t ns,
     ae_int_t nd,
     ae_int_t idx,
     /* Real    */ ae_vector* tmp,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This subroutine is used to initialize SNNLS solver.

By default, empty NNLS problem is produced, but we allocated enough  space
to store problems with NSMax+NDMax columns and  NRMax  rows.  It  is  good
place to provide algorithm with initial estimate of the space requirements,
although you may underestimate problem size or even pass zero estimates  -
in this case buffer variables will be resized automatically  when  you set
NNLS problem.

Previously allocated buffer variables are reused as much as possible. This
function does not clear structure completely, it tries to preserve as much
dynamically allocated memory as possible.

  -- ALGLIB --
     Copyright 10.10.2012 by Bochkanov Sergey
*************************************************************************/
void snnlsinit(ae_int_t nsmax,
     ae_int_t ndmax,
     ae_int_t nrmax,
     snnlssolver* s,
     ae_state *_state)
{


    s->ns = 0;
    s->nd = 0;
    s->nr = 0;
    rmatrixsetlengthatleast(&s->densea, nrmax, ndmax, _state);
    rmatrixsetlengthatleast(&s->tmpca, nrmax, ndmax, _state);
    rmatrixsetlengthatleast(&s->tmpz, ndmax, ndmax, _state);
    rvectorsetlengthatleast(&s->b, nrmax, _state);
    bvectorsetlengthatleast(&s->nnc, nsmax+ndmax, _state);
    s->debugflops = 0.0;
    s->debugmaxinnerits = 0;
}


/*************************************************************************
This subroutine is used to set NNLS problem:

        ( [ 1     |      ]   [   ]   [   ] )^2
        ( [   1   |      ]   [   ]   [   ] )
    min ( [     1 |  Ad  ] * [ x ] - [ b ] )    s.t. x>=0
        ( [       |      ]   [   ]   [   ] )
        ( [       |      ]   [   ]   [   ] )

where:
* identity matrix has NS*NS size (NS<=NR, NS can be zero)
* dense matrix Ad has NR*ND size
* b is NR*1 vector
* x is (NS+ND)*1 vector
* all elements of x are non-negative (this constraint can be removed later
  by calling SNNLSDropNNC() function)

Previously allocated buffer variables are reused as much as possible.
After you set problem, you can solve it with SNNLSSolve().

INPUT PARAMETERS:
    S   -   SNNLS solver, must be initialized with SNNLSInit() call
    A   -   array[NR,ND], dense part of the system
    B   -   array[NR], right part
    NS  -   size of the sparse part of the system, 0<=NS<=NR
    ND  -   size of the dense part of the system, ND>=0
    NR  -   rows count, NR>0

NOTE:
    1. You can have NS+ND=0, solver will correctly accept such combination
       and return empty array as problem solution.
    
  -- ALGLIB --
     Copyright 10.10.2012 by Bochkanov Sergey
*************************************************************************/
void snnlssetproblem(snnlssolver* s,
     /* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* b,
     ae_int_t ns,
     ae_int_t nd,
     ae_int_t nr,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(nd>=0, "SNNLSSetProblem: ND<0", _state);
    ae_assert(ns>=0, "SNNLSSetProblem: NS<0", _state);
    ae_assert(nr>0, "SNNLSSetProblem: NR<=0", _state);
    ae_assert(ns<=nr, "SNNLSSetProblem: NS>NR", _state);
    ae_assert(a->rows>=nr||nd==0, "SNNLSSetProblem: rows(A)<NR", _state);
    ae_assert(a->cols>=nd, "SNNLSSetProblem: cols(A)<ND", _state);
    ae_assert(b->cnt>=nr, "SNNLSSetProblem: length(B)<NR", _state);
    ae_assert(apservisfinitematrix(a, nr, nd, _state), "SNNLSSetProblem: A contains INF/NAN", _state);
    ae_assert(isfinitevector(b, nr, _state), "SNNLSSetProblem: B contains INF/NAN", _state);
    
    /*
     * Copy problem
     */
    s->ns = ns;
    s->nd = nd;
    s->nr = nr;
    if( nd>0 )
    {
        rmatrixsetlengthatleast(&s->densea, nr, nd, _state);
        for(i=0; i<=nr-1; i++)
        {
            ae_v_move(&s->densea.ptr.pp_double[i][0], 1, &a->ptr.pp_double[i][0], 1, ae_v_len(0,nd-1));
        }
    }
    rvectorsetlengthatleast(&s->b, nr, _state);
    ae_v_move(&s->b.ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,nr-1));
    bvectorsetlengthatleast(&s->nnc, ns+nd, _state);
    for(i=0; i<=ns+nd-1; i++)
    {
        s->nnc.ptr.p_bool[i] = ae_true;
    }
}


/*************************************************************************
This subroutine drops non-negativity constraint from the  problem  set  by
SNNLSSetProblem() call. This function must be called AFTER problem is set,
because each SetProblem() call resets constraints to their  default  state
(all constraints are present).

INPUT PARAMETERS:
    S   -   SNNLS solver, must be initialized with SNNLSInit() call,
            problem must be set with SNNLSSetProblem() call.
    Idx -   constraint index, 0<=IDX<NS+ND
    
  -- ALGLIB --
     Copyright 10.10.2012 by Bochkanov Sergey
*************************************************************************/
void snnlsdropnnc(snnlssolver* s, ae_int_t idx, ae_state *_state)
{


    ae_assert(idx>=0, "SNNLSDropNNC: Idx<0", _state);
    ae_assert(idx<s->ns+s->nd, "SNNLSDropNNC: Idx>=NS+ND", _state);
    s->nnc.ptr.p_bool[idx] = ae_false;
}


/*************************************************************************
This subroutine is used to solve NNLS problem.

INPUT PARAMETERS:
    S   -   SNNLS solver, must be initialized with SNNLSInit() call and
            problem must be set up with SNNLSSetProblem() call.
    X   -   possibly preallocated buffer, automatically resized if needed

OUTPUT PARAMETERS:
    X   -   array[NS+ND], solution
    
NOTE:
    1. You can have NS+ND=0, solver will correctly accept such combination
       and return empty array as problem solution.
    
    2. Internal field S.DebugFLOPS contains rough estimate of  FLOPs  used
       to solve problem. It can be used for debugging purposes. This field
       is real-valued.
    
  -- ALGLIB --
     Copyright 10.10.2012 by Bochkanov Sergey
*************************************************************************/
void snnlssolve(snnlssolver* s,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t ns;
    ae_int_t nd;
    ae_int_t nr;
    ae_bool wasactivation;
    double lambdav;
    double v0;
    double v1;
    double v;
    ae_int_t outerits;
    ae_int_t innerits;
    ae_int_t maxouterits;
    double xtol;
    double kicklength;
    ae_bool kickneeded;
    double f0;
    double f1;
    double dnrm;
    ae_int_t actidx;
    double stp;
    double stpmax;


    
    /*
     * Prepare
     */
    ns = s->ns;
    nd = s->nd;
    nr = s->nr;
    s->debugflops = 0.0;
    
    /*
     * Handle special cases:
     * * NS+ND=0
     * * ND=0
     */
    if( ns+nd==0 )
    {
        return;
    }
    if( nd==0 )
    {
        rvectorsetlengthatleast(x, ns, _state);
        for(i=0; i<=ns-1; i++)
        {
            x->ptr.p_double[i] = s->b.ptr.p_double[i];
            if( s->nnc.ptr.p_bool[i] )
            {
                x->ptr.p_double[i] = ae_maxreal(x->ptr.p_double[i], 0.0, _state);
            }
        }
        return;
    }
    
    /*
     * Main cycle of BLEIC-SNNLS algorithm.
     * Below we assume that ND>0.
     */
    rvectorsetlengthatleast(x, ns+nd, _state);
    rvectorsetlengthatleast(&s->xn, ns+nd, _state);
    rvectorsetlengthatleast(&s->xp, ns+nd, _state);
    rvectorsetlengthatleast(&s->g, ns+nd, _state);
    rvectorsetlengthatleast(&s->d, ns+nd, _state);
    rvectorsetlengthatleast(&s->r, nr, _state);
    rvectorsetlengthatleast(&s->diagaa, nd, _state);
    rvectorsetlengthatleast(&s->regdiag, ns+nd, _state);
    rvectorsetlengthatleast(&s->dx, ns+nd, _state);
    for(i=0; i<=ns+nd-1; i++)
    {
        x->ptr.p_double[i] = 0.0;
        s->regdiag.ptr.p_double[i] = 1.0;
    }
    lambdav = 1.0E6*ae_machineepsilon;
    maxouterits = 10;
    outerits = 0;
    innerits = 0;
    xtol = 1.0E3*ae_machineepsilon;
    kicklength = ae_sqrt(ae_minrealnumber, _state);
    for(;;)
    {
        
        /*
         * Initial check for correctness of X
         */
        for(i=0; i<=ns+nd-1; i++)
        {
            ae_assert(!s->nnc.ptr.p_bool[i]||ae_fp_greater_eq(x->ptr.p_double[i],(double)(0)), "SNNLS: integrity check failed", _state);
        }
        
        /*
         * Calculate gradient G and constrained descent direction D
         */
        snnls_funcgradu(s, x, &s->r, &s->g, &f0, _state);
        for(i=0; i<=ns+nd-1; i++)
        {
            if( (s->nnc.ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],(double)(0)))&&ae_fp_greater(s->g.ptr.p_double[i],(double)(0)) )
            {
                s->d.ptr.p_double[i] = 0.0;
            }
            else
            {
                s->d.ptr.p_double[i] = -s->g.ptr.p_double[i];
            }
        }
        
        /*
         * Decide whether we need "kick" stage: special stage
         * that moves us away from boundary constraints which are
         * not strictly active (i.e. such constraints that x[i]=0.0 and d[i]>0).
         *
         * If we need kick stage, we make a kick - and restart iteration.
         * If not, after this block we can rely on the fact that
         * for all x[i]=0.0 we have d[i]=0.0
         *
         * NOTE: we do not increase outer iterations counter here
         */
        kickneeded = ae_false;
        for(i=0; i<=ns+nd-1; i++)
        {
            if( (s->nnc.ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],0.0))&&ae_fp_greater(s->d.ptr.p_double[i],0.0) )
            {
                kickneeded = ae_true;
            }
        }
        if( kickneeded )
        {
            
            /*
             * Perform kick.
             * Restart.
             * Do not increase iterations counter.
             */
            for(i=0; i<=ns+nd-1; i++)
            {
                if( ae_fp_eq(x->ptr.p_double[i],0.0)&&ae_fp_greater(s->d.ptr.p_double[i],0.0) )
                {
                    x->ptr.p_double[i] = x->ptr.p_double[i]+kicklength;
                }
            }
            continue;
        }
        
        /*
         * Newton phase
         * Reduce problem to constrained triangular form and perform Newton
         * steps with quick activation of constrants  (triangular  form  is
         * updated in order to handle changed constraints).
         */
        for(i=0; i<=ns+nd-1; i++)
        {
            s->xp.ptr.p_double[i] = x->ptr.p_double[i];
        }
        snnls_trdprepare(s, x, &s->regdiag, lambdav, &s->trdd, &s->trda, &s->tmp0, &s->tmp1, &s->tmp2, &s->tmplq, _state);
        for(;;)
        {
            
            /*
             * Skip if debug limit on inner iterations count is turned on.
             */
            if( s->debugmaxinnerits>0&&innerits>=s->debugmaxinnerits )
            {
                break;
            }
            
            /*
             * Prepare step vector.
             */
            snnls_funcgradu(s, x, &s->r, &s->g, &f0, _state);
            for(i=0; i<=ns+nd-1; i++)
            {
                s->d.ptr.p_double[i] = -s->g.ptr.p_double[i];
                if( s->nnc.ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],0.0) )
                {
                    s->d.ptr.p_double[i] = 0.0;
                }
            }
            snnls_trdsolve(&s->trdd, &s->trda, ns, nd, &s->d, _state);
            
            /*
             * Perform unconstrained trial step and compare function values.
             */
            for(i=0; i<=ns+nd-1; i++)
            {
                s->xn.ptr.p_double[i] = x->ptr.p_double[i]+s->d.ptr.p_double[i];
            }
            snnls_func(s, &s->xn, &f1, _state);
            if( ae_fp_greater_eq(f1,f0) )
            {
                break;
            }
            
            /*
             * Calculate length of D, maximum step and component which is
             * activated by this step. Break if D is exactly zero.
             */
            dnrm = 0.0;
            for(i=0; i<=ns+nd-1; i++)
            {
                dnrm = dnrm+ae_sqr(s->d.ptr.p_double[i], _state);
            }
            dnrm = ae_sqrt(dnrm, _state);
            actidx = -1;
            stpmax = 1.0E50;
            for(i=0; i<=ns+nd-1; i++)
            {
                if( s->nnc.ptr.p_bool[i]&&ae_fp_less(s->d.ptr.p_double[i],0.0) )
                {
                    v = stpmax;
                    stpmax = safeminposrv(x->ptr.p_double[i], -s->d.ptr.p_double[i], stpmax, _state);
                    if( ae_fp_less(stpmax,v) )
                    {
                        actidx = i;
                    }
                }
            }
            if( ae_fp_eq(dnrm,0.0) )
            {
                break;
            }
            
            /*
             * Perform constrained step and update X
             * and triangular model.
             */
            stp = ae_minreal(1.0, stpmax, _state);
            for(i=0; i<=ns+nd-1; i++)
            {
                v = x->ptr.p_double[i]+stp*s->d.ptr.p_double[i];
                if( s->nnc.ptr.p_bool[i] )
                {
                    v = ae_maxreal(v, 0.0, _state);
                }
                s->xn.ptr.p_double[i] = v;
            }
            if( ae_fp_eq(stp,stpmax)&&actidx>=0 )
            {
                s->xn.ptr.p_double[actidx] = 0.0;
            }
            wasactivation = ae_false;
            for(i=0; i<=ns+nd-1; i++)
            {
                if( ae_fp_eq(s->xn.ptr.p_double[i],0.0)&&ae_fp_neq(x->ptr.p_double[i],0.0) )
                {
                    wasactivation = ae_true;
                    snnls_trdfixvariable(&s->trdd, &s->trda, ns, nd, i, &s->tmpcholesky, _state);
                }
            }
            for(i=0; i<=ns+nd-1; i++)
            {
                x->ptr.p_double[i] = s->xn.ptr.p_double[i];
            }
            
            /*
             * Increment iterations counter.
             * Terminate if no constraint was activated.
             */
            inc(&innerits, _state);
            if( !wasactivation )
            {
                break;
            }
        }
        
        /*
         * Update outer iterations counter.
         *
         * Break if necessary:
         * * maximum number of outer iterations performed
         * * relative change in X is small enough
         */
        inc(&outerits, _state);
        if( outerits>=maxouterits )
        {
            break;
        }
        v = (double)(0);
        for(i=0; i<=ns+nd-1; i++)
        {
            v0 = ae_fabs(s->xp.ptr.p_double[i], _state);
            v1 = ae_fabs(x->ptr.p_double[i], _state);
            if( ae_fp_neq(v0,(double)(0))||ae_fp_neq(v1,(double)(0)) )
            {
                v = ae_maxreal(v, ae_fabs(x->ptr.p_double[i]-s->xp.ptr.p_double[i], _state)/ae_maxreal(v0, v1, _state), _state);
            }
        }
        if( ae_fp_less_eq(v,xtol) )
        {
            break;
        }
    }
}


/*************************************************************************
This function calculates:
* residual vector R = A*x-b
* unconstrained gradient vector G
* function value F = 0.5*|R|^2

R and G must have at least N elements.

  -- ALGLIB --
     Copyright 15.07.2015 by Bochkanov Sergey
*************************************************************************/
static void snnls_funcgradu(snnlssolver* s,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* r,
     /* Real    */ ae_vector* g,
     double* f,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t nr;
    ae_int_t nd;
    ae_int_t ns;
    double v;

    *f = 0;

    nr = s->nr;
    nd = s->nd;
    ns = s->ns;
    *f = 0.0;
    for(i=0; i<=nr-1; i++)
    {
        v = ae_v_dotproduct(&s->densea.ptr.pp_double[i][0], 1, &x->ptr.p_double[ns], 1, ae_v_len(0,nd-1));
        if( i<ns )
        {
            v = v+x->ptr.p_double[i];
        }
        v = v-s->b.ptr.p_double[i];
        r->ptr.p_double[i] = v;
        *f = *f+0.5*v*v;
    }
    for(i=0; i<=ns-1; i++)
    {
        g->ptr.p_double[i] = r->ptr.p_double[i];
    }
    for(i=ns; i<=ns+nd-1; i++)
    {
        g->ptr.p_double[i] = 0.0;
    }
    for(i=0; i<=nr-1; i++)
    {
        v = r->ptr.p_double[i];
        ae_v_addd(&g->ptr.p_double[ns], 1, &s->densea.ptr.pp_double[i][0], 1, ae_v_len(ns,ns+nd-1), v);
    }
}


/*************************************************************************
This function calculates function value F = 0.5*|R|^2 at X.

  -- ALGLIB --
     Copyright 15.07.2015 by Bochkanov Sergey
*************************************************************************/
static void snnls_func(snnlssolver* s,
     /* Real    */ ae_vector* x,
     double* f,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t nr;
    ae_int_t nd;
    ae_int_t ns;
    double v;

    *f = 0;

    nr = s->nr;
    nd = s->nd;
    ns = s->ns;
    *f = 0.0;
    for(i=0; i<=nr-1; i++)
    {
        v = ae_v_dotproduct(&s->densea.ptr.pp_double[i][0], 1, &x->ptr.p_double[ns], 1, ae_v_len(0,nd-1));
        if( i<ns )
        {
            v = v+x->ptr.p_double[i];
        }
        v = v-s->b.ptr.p_double[i];
        *f = *f+0.5*v*v;
    }
}


static void snnls_trdprepare(snnlssolver* s,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* diag,
     double lambdav,
     /* Real    */ ae_vector* trdd,
     /* Real    */ ae_matrix* trda,
     /* Real    */ ae_vector* tmp0,
     /* Real    */ ae_vector* tmp1,
     /* Real    */ ae_vector* tmp2,
     /* Real    */ ae_matrix* tmplq,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t ns;
    ae_int_t nd;
    ae_int_t nr;
    double v;
    double cs;
    double sn;
    double r;


    
    /*
     * Prepare
     */
    ns = s->ns;
    nd = s->nd;
    nr = s->nr;
    
    /*
     * Triangular reduction
     */
    rvectorsetlengthatleast(trdd, ns, _state);
    rmatrixsetlengthatleast(trda, ns+nd, nd, _state);
    rmatrixsetlengthatleast(tmplq, nd, nr+nd, _state);
    for(i=0; i<=ns-1; i++)
    {
        
        /*
         * Apply rotation to I-th row and corresponding row of
         * regularizer. Here V is diagonal element of I-th row,
         * which is set to 1.0 or 0.0 depending on variable
         * status (constrained or not).
         */
        v = 1.0;
        if( s->nnc.ptr.p_bool[i]&&ae_fp_eq(x->ptr.p_double[i],0.0) )
        {
            v = 0.0;
        }
        generaterotation(v, lambdav, &cs, &sn, &r, _state);
        trdd->ptr.p_double[i] = cs*v+sn*lambdav;
        for(j=0; j<=nd-1; j++)
        {
            v = s->densea.ptr.pp_double[i][j];
            trda->ptr.pp_double[i][j] = cs*v;
            tmplq->ptr.pp_double[j][i] = -sn*v;
        }
    }
    for(j=0; j<=nd-1; j++)
    {
        for(i=ns; i<=nr-1; i++)
        {
            tmplq->ptr.pp_double[j][i] = s->densea.ptr.pp_double[i][j];
        }
    }
    for(j=0; j<=nd-1; j++)
    {
        if( s->nnc.ptr.p_bool[ns+j]&&ae_fp_eq(x->ptr.p_double[ns+j],0.0) )
        {
            
            /*
             * Variable is constrained, entire row is set to zero.
             */
            for(i=0; i<=nr-1; i++)
            {
                tmplq->ptr.pp_double[j][i] = 0.0;
            }
            for(i=0; i<=ns-1; i++)
            {
                trda->ptr.pp_double[i][j] = 0.0;
            }
        }
    }
    for(i=0; i<=nd-1; i++)
    {
        for(j=0; j<=nd-1; j++)
        {
            tmplq->ptr.pp_double[j][nr+i] = 0.0;
        }
        tmplq->ptr.pp_double[i][nr+i] = lambdav*diag->ptr.p_double[i];
    }
    rvectorsetlengthatleast(tmp0, nr+nd+1, _state);
    rvectorsetlengthatleast(tmp1, nr+nd+1, _state);
    rvectorsetlengthatleast(tmp2, nr+nd+1, _state);
    rmatrixlqbasecase(tmplq, nd, nr+nd, tmp0, tmp1, tmp2, _state);
    for(i=0; i<=nd-1; i++)
    {
        if( ae_fp_less(tmplq->ptr.pp_double[i][i],0.0) )
        {
            for(j=i; j<=nd-1; j++)
            {
                tmplq->ptr.pp_double[j][i] = -tmplq->ptr.pp_double[j][i];
            }
        }
    }
    for(i=0; i<=nd-1; i++)
    {
        for(j=0; j<=i; j++)
        {
            trda->ptr.pp_double[ns+j][i] = tmplq->ptr.pp_double[i][j];
        }
    }
}


static void snnls_trdsolve(/* Real    */ ae_vector* trdd,
     /* Real    */ ae_matrix* trda,
     ae_int_t ns,
     ae_int_t nd,
     /* Real    */ ae_vector* d,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;


    
    /*
     * Solve U'*y=d first.
     *
     * This section includes two parts:
     * * solve diagonal part of U'
     * * solve dense part of U'
     */
    for(i=0; i<=ns-1; i++)
    {
        d->ptr.p_double[i] = d->ptr.p_double[i]/trdd->ptr.p_double[i];
        v = d->ptr.p_double[i];
        for(j=0; j<=nd-1; j++)
        {
            d->ptr.p_double[ns+j] = d->ptr.p_double[ns+j]-v*trda->ptr.pp_double[i][j];
        }
    }
    for(i=0; i<=nd-1; i++)
    {
        d->ptr.p_double[ns+i] = d->ptr.p_double[ns+i]/trda->ptr.pp_double[ns+i][i];
        v = d->ptr.p_double[ns+i];
        for(j=i+1; j<=nd-1; j++)
        {
            d->ptr.p_double[ns+j] = d->ptr.p_double[ns+j]-v*trda->ptr.pp_double[ns+i][j];
        }
    }
    
    /*
     * Solve U*x=y then.
     *
     * This section includes two parts:
     * * solve trailing triangular part of U
     * * solve combination of diagonal and dense parts of U
     */
    for(i=nd-1; i>=0; i--)
    {
        v = 0.0;
        for(j=i+1; j<=nd-1; j++)
        {
            v = v+trda->ptr.pp_double[ns+i][j]*d->ptr.p_double[ns+j];
        }
        d->ptr.p_double[ns+i] = (d->ptr.p_double[ns+i]-v)/trda->ptr.pp_double[ns+i][i];
    }
    for(i=ns-1; i>=0; i--)
    {
        v = 0.0;
        for(j=0; j<=nd-1; j++)
        {
            v = v+trda->ptr.pp_double[i][j]*d->ptr.p_double[ns+j];
        }
        d->ptr.p_double[i] = (d->ptr.p_double[i]-v)/trdd->ptr.p_double[i];
    }
}


static void snnls_trdfixvariable(/* Real    */ ae_vector* trdd,
     /* Real    */ ae_matrix* trda,
     ae_int_t ns,
     ae_int_t nd,
     ae_int_t idx,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double cs;
    double sn;
    double r;
    double v;
    double vv;


    ae_assert(ns>=0, "TRDFixVariable: integrity error", _state);
    ae_assert(nd>=0, "TRDFixVariable: integrity error", _state);
    ae_assert(ns+nd>0, "TRDFixVariable: integrity error", _state);
    ae_assert(idx>=0, "TRDFixVariable: integrity error", _state);
    ae_assert(idx<ns+nd, "TRDFixVariable: integrity error", _state);
    rvectorsetlengthatleast(tmp, nd, _state);
    
    /*
     * Depending on variable index, two situations are possible
     */
    if( idx<ns )
    {
        
        /*
         * We fix variable in the diagonal part of the model. It means
         * that prior to fixing we have:
         *
         *     (     |     )
         *     (  D  |     )
         *     (     |     )
         *     (-----|  A  )
         *     (     |0    )
         *     (     |00   )
         *     (     |000  )
         *     (     |0000 )
         *     (     |00000)
         *
         * then we replace idx-th column by zeros:
         *
         *     (D 0  |     )
         *     (  0  |     )
         *     (  0 D|     )
         *     (-----|  A  )
         *     (     |     )
         *     (     |     )
         *     (     |     )
         *
         * and append row with unit element to bottom, in order to
         * regularize problem
         *
         *     (D 0  |     )
         *     (  0  |     )
         *     (  0 D|     )
         *     (-----|  A  )
         *     (     |     )
         *     (     |     )
         *     (     |     )
         *     (00100|00000) <- appended
         *
         * and then we nullify this row by applying rotations:
         *
         *     (D 0  |     )
         *     (  0  |     ) <- first rotation is applied here
         *     (  0 D|     )
         *     (-----|  A  ) <- subsequent rotations are applied to this row and rows below
         *     (     |     )
         *     (     |     )
         *     (     |     )
         *     (  0  |  0  ) <- as result, row becomes zero
         *
         * and triangular structure is preserved
         */
        if( nd==0 )
        {
            
            /*
             * Quick exit for empty dense part
             */
            trdd->ptr.p_double[idx] = 1.0;
            return;
        }
        for(j=0; j<=nd-1; j++)
        {
            
            /*
             * Apply first rotation
             */
            tmp->ptr.p_double[j] = trda->ptr.pp_double[idx][j];
            trda->ptr.pp_double[idx][j] = 0.0;
        }
        trdd->ptr.p_double[idx] = 1.0;
        for(i=0; i<=nd-1; i++)
        {
            if( ae_fp_neq(tmp->ptr.p_double[i],(double)(0)) )
            {
                
                /*
                 * Apply subsequent rotations with bottom triangular part of A
                 */
                generaterotation(trda->ptr.pp_double[ns+i][i], tmp->ptr.p_double[i], &cs, &sn, &r, _state);
                for(j=i; j<=nd-1; j++)
                {
                    v = trda->ptr.pp_double[ns+i][j];
                    vv = tmp->ptr.p_double[j];
                    trda->ptr.pp_double[ns+i][j] = v*cs+vv*sn;
                    tmp->ptr.p_double[j] = vv*cs-v*sn;
                }
            }
        }
    }
    else
    {
        
        /*
         * We fix variable in the dense part of the model. It means
         * that prior to fixing we have:
         *
         *     (     |     )
         *     (  D  |     )
         *     (     |     )
         *     (-----|  A  )
         *     (     |0    )
         *     (     |00   )
         *     (     |000  )
         *     (     |0000 )
         *     (     |00000)
         *
         * then we replace idx-th column by zeros:
         *
         *     (     |  0  )
         *     (  D  |  0  )
         *     (     |  0  )
         *     (-----|A 0 A)
         *     (     |  0  )
         *     (     |  0  )
         *     (     |  0  )
         *
         * and append row with unit element to bottom, in order to
         * regularize problem
         *
         *     (     |  0  )
         *     (  D  |  0  )
         *     (     |  0  )
         *     (-----|A 0 A)
         *     (     |  0  )
         *     (     |  0  )
         *     (     |  0  )
         *     (00000|00100) <- appended
         *
         * and then we nullify this row by applying rotations:
         *
         *     (D 0  |     )
         *     (  0  |     )
         *     (  0 D|     )
         *     (-----|  A  )
         *     (     |     )
         *     (     |     ) <- first rotation is applied here
         *     (     |     ) <- subsequent rotations are applied to rows below
         *     (  0  |  0  ) <- as result, row becomes zero
         *
         * and triangular structure is preserved.
         */
        k = idx-ns;
        for(i=0; i<=ns+nd-1; i++)
        {
            trda->ptr.pp_double[i][k] = 0.0;
        }
        for(j=k+1; j<=nd-1; j++)
        {
            
            /*
             * Apply first rotation
             */
            tmp->ptr.p_double[j] = trda->ptr.pp_double[idx][j];
            trda->ptr.pp_double[idx][j] = 0.0;
        }
        trda->ptr.pp_double[idx][k] = 1.0;
        for(i=k+1; i<=nd-1; i++)
        {
            if( ae_fp_neq(tmp->ptr.p_double[i],(double)(0)) )
            {
                
                /*
                 * Apply subsequent rotations with bottom triangular part of A
                 */
                generaterotation(trda->ptr.pp_double[ns+i][i], tmp->ptr.p_double[i], &cs, &sn, &r, _state);
                for(j=i; j<=nd-1; j++)
                {
                    v = trda->ptr.pp_double[ns+i][j];
                    vv = tmp->ptr.p_double[j];
                    trda->ptr.pp_double[ns+i][j] = v*cs+vv*sn;
                    tmp->ptr.p_double[j] = vv*cs-v*sn;
                }
            }
        }
    }
}


void _snnlssolver_init(void* _p, ae_state *_state)
{
    snnlssolver *p = (snnlssolver*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->densea, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->b, 0, DT_REAL, _state);
    ae_vector_init(&p->nnc, 0, DT_BOOL, _state);
    ae_vector_init(&p->xn, 0, DT_REAL, _state);
    ae_vector_init(&p->xp, 0, DT_REAL, _state);
    ae_matrix_init(&p->tmpz, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->tmpca, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->tmplq, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->trda, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->trdd, 0, DT_REAL, _state);
    ae_vector_init(&p->crb, 0, DT_REAL, _state);
    ae_vector_init(&p->g, 0, DT_REAL, _state);
    ae_vector_init(&p->d, 0, DT_REAL, _state);
    ae_vector_init(&p->dx, 0, DT_REAL, _state);
    ae_vector_init(&p->diagaa, 0, DT_REAL, _state);
    ae_vector_init(&p->cb, 0, DT_REAL, _state);
    ae_vector_init(&p->cx, 0, DT_REAL, _state);
    ae_vector_init(&p->cborg, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpcholesky, 0, DT_REAL, _state);
    ae_vector_init(&p->r, 0, DT_REAL, _state);
    ae_vector_init(&p->regdiag, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp0, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp1, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp2, 0, DT_REAL, _state);
    ae_vector_init(&p->rdtmprowmap, 0, DT_INT, _state);
}


void _snnlssolver_init_copy(void* _dst, void* _src, ae_state *_state)
{
    snnlssolver *dst = (snnlssolver*)_dst;
    snnlssolver *src = (snnlssolver*)_src;
    dst->ns = src->ns;
    dst->nd = src->nd;
    dst->nr = src->nr;
    ae_matrix_init_copy(&dst->densea, &src->densea, _state);
    ae_vector_init_copy(&dst->b, &src->b, _state);
    ae_vector_init_copy(&dst->nnc, &src->nnc, _state);
    dst->debugflops = src->debugflops;
    dst->debugmaxinnerits = src->debugmaxinnerits;
    ae_vector_init_copy(&dst->xn, &src->xn, _state);
    ae_vector_init_copy(&dst->xp, &src->xp, _state);
    ae_matrix_init_copy(&dst->tmpz, &src->tmpz, _state);
    ae_matrix_init_copy(&dst->tmpca, &src->tmpca, _state);
    ae_matrix_init_copy(&dst->tmplq, &src->tmplq, _state);
    ae_matrix_init_copy(&dst->trda, &src->trda, _state);
    ae_vector_init_copy(&dst->trdd, &src->trdd, _state);
    ae_vector_init_copy(&dst->crb, &src->crb, _state);
    ae_vector_init_copy(&dst->g, &src->g, _state);
    ae_vector_init_copy(&dst->d, &src->d, _state);
    ae_vector_init_copy(&dst->dx, &src->dx, _state);
    ae_vector_init_copy(&dst->diagaa, &src->diagaa, _state);
    ae_vector_init_copy(&dst->cb, &src->cb, _state);
    ae_vector_init_copy(&dst->cx, &src->cx, _state);
    ae_vector_init_copy(&dst->cborg, &src->cborg, _state);
    ae_vector_init_copy(&dst->tmpcholesky, &src->tmpcholesky, _state);
    ae_vector_init_copy(&dst->r, &src->r, _state);
    ae_vector_init_copy(&dst->regdiag, &src->regdiag, _state);
    ae_vector_init_copy(&dst->tmp0, &src->tmp0, _state);
    ae_vector_init_copy(&dst->tmp1, &src->tmp1, _state);
    ae_vector_init_copy(&dst->tmp2, &src->tmp2, _state);
    ae_vector_init_copy(&dst->rdtmprowmap, &src->rdtmprowmap, _state);
}


void _snnlssolver_clear(void* _p)
{
    snnlssolver *p = (snnlssolver*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->densea);
    ae_vector_clear(&p->b);
    ae_vector_clear(&p->nnc);
    ae_vector_clear(&p->xn);
    ae_vector_clear(&p->xp);
    ae_matrix_clear(&p->tmpz);
    ae_matrix_clear(&p->tmpca);
    ae_matrix_clear(&p->tmplq);
    ae_matrix_clear(&p->trda);
    ae_vector_clear(&p->trdd);
    ae_vector_clear(&p->crb);
    ae_vector_clear(&p->g);
    ae_vector_clear(&p->d);
    ae_vector_clear(&p->dx);
    ae_vector_clear(&p->diagaa);
    ae_vector_clear(&p->cb);
    ae_vector_clear(&p->cx);
    ae_vector_clear(&p->cborg);
    ae_vector_clear(&p->tmpcholesky);
    ae_vector_clear(&p->r);
    ae_vector_clear(&p->regdiag);
    ae_vector_clear(&p->tmp0);
    ae_vector_clear(&p->tmp1);
    ae_vector_clear(&p->tmp2);
    ae_vector_clear(&p->rdtmprowmap);
}


void _snnlssolver_destroy(void* _p)
{
    snnlssolver *p = (snnlssolver*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->densea);
    ae_vector_destroy(&p->b);
    ae_vector_destroy(&p->nnc);
    ae_vector_destroy(&p->xn);
    ae_vector_destroy(&p->xp);
    ae_matrix_destroy(&p->tmpz);
    ae_matrix_destroy(&p->tmpca);
    ae_matrix_destroy(&p->tmplq);
    ae_matrix_destroy(&p->trda);
    ae_vector_destroy(&p->trdd);
    ae_vector_destroy(&p->crb);
    ae_vector_destroy(&p->g);
    ae_vector_destroy(&p->d);
    ae_vector_destroy(&p->dx);
    ae_vector_destroy(&p->diagaa);
    ae_vector_destroy(&p->cb);
    ae_vector_destroy(&p->cx);
    ae_vector_destroy(&p->cborg);
    ae_vector_destroy(&p->tmpcholesky);
    ae_vector_destroy(&p->r);
    ae_vector_destroy(&p->regdiag);
    ae_vector_destroy(&p->tmp0);
    ae_vector_destroy(&p->tmp1);
    ae_vector_destroy(&p->tmp2);
    ae_vector_destroy(&p->rdtmprowmap);
}


/*$ End $*/
