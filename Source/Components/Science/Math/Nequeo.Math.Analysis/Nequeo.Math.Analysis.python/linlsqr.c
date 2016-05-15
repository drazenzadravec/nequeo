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
#include "linlsqr.h"


/*$ Declarations $*/
static double linlsqr_atol = 1.0E-6;
static double linlsqr_btol = 1.0E-6;
static void linlsqr_clearrfields(linlsqrstate* state, ae_state *_state);


/*$ Body $*/


/*************************************************************************
This function initializes linear LSQR Solver. This solver is used to solve
non-symmetric (and, possibly, non-square) problems. Least squares solution
is returned for non-compatible systems.

USAGE:
1. User initializes algorithm state with LinLSQRCreate() call
2. User tunes solver parameters with  LinLSQRSetCond() and other functions
3. User  calls  LinLSQRSolveSparse()  function which takes algorithm state 
   and SparseMatrix object.
4. User calls LinLSQRResults() to get solution
5. Optionally, user may call LinLSQRSolveSparse() again to  solve  another  
   problem  with different matrix and/or right part without reinitializing 
   LinLSQRState structure.
  
INPUT PARAMETERS:
    M       -   number of rows in A
    N       -   number of variables, N>0

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
void linlsqrcreate(ae_int_t m,
     ae_int_t n,
     linlsqrstate* state,
     ae_state *_state)
{
    ae_int_t i;

    _linlsqrstate_clear(state);

    ae_assert(m>0, "LinLSQRCreate: M<=0", _state);
    ae_assert(n>0, "LinLSQRCreate: N<=0", _state);
    state->m = m;
    state->n = n;
    state->prectype = 0;
    state->epsa = linlsqr_atol;
    state->epsb = linlsqr_btol;
    state->epsc = 1/ae_sqrt(ae_machineepsilon, _state);
    state->maxits = 0;
    state->lambdai = (double)(0);
    state->xrep = ae_false;
    state->running = ae_false;
    
    /*
     * * allocate arrays
     * * set RX to NAN (just for the case user calls Results() without 
     *   calling SolveSparse()
     * * set B to zero
     */
    normestimatorcreate(m, n, 2, 2, &state->nes, _state);
    ae_vector_set_length(&state->rx, state->n, _state);
    ae_vector_set_length(&state->ui, state->m+state->n, _state);
    ae_vector_set_length(&state->uip1, state->m+state->n, _state);
    ae_vector_set_length(&state->vip1, state->n, _state);
    ae_vector_set_length(&state->vi, state->n, _state);
    ae_vector_set_length(&state->omegai, state->n, _state);
    ae_vector_set_length(&state->omegaip1, state->n, _state);
    ae_vector_set_length(&state->d, state->n, _state);
    ae_vector_set_length(&state->x, state->m+state->n, _state);
    ae_vector_set_length(&state->mv, state->m+state->n, _state);
    ae_vector_set_length(&state->mtv, state->n, _state);
    ae_vector_set_length(&state->b, state->m, _state);
    for(i=0; i<=n-1; i++)
    {
        state->rx.ptr.p_double[i] = _state->v_nan;
    }
    for(i=0; i<=m-1; i++)
    {
        state->b.ptr.p_double[i] = (double)(0);
    }
    ae_vector_set_length(&state->rstate.ia, 1+1, _state);
    ae_vector_set_length(&state->rstate.ra, 0+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
This function sets right part. By default, right part is zero.

INPUT PARAMETERS:
    B       -   right part, array[N].

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
void linlsqrsetb(linlsqrstate* state,
     /* Real    */ ae_vector* b,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(!state->running, "LinLSQRSetB: you can not change B when LinLSQRIteration is running", _state);
    ae_assert(state->m<=b->cnt, "LinLSQRSetB: Length(B)<M", _state);
    ae_assert(isfinitevector(b, state->m, _state), "LinLSQRSetB: B contains infinite or NaN values", _state);
    state->bnorm2 = (double)(0);
    for(i=0; i<=state->m-1; i++)
    {
        state->b.ptr.p_double[i] = b->ptr.p_double[i];
        state->bnorm2 = state->bnorm2+b->ptr.p_double[i]*b->ptr.p_double[i];
    }
}


/*************************************************************************
This  function  changes  preconditioning  settings of LinLSQQSolveSparse()
function. By default, SolveSparse() uses diagonal preconditioner,  but  if
you want to use solver without preconditioning, you can call this function
which forces solver to use unit matrix for preconditioning.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 19.11.2012 by Bochkanov Sergey
*************************************************************************/
void linlsqrsetprecunit(linlsqrstate* state, ae_state *_state)
{


    ae_assert(!state->running, "LinLSQRSetPrecUnit: you can not change preconditioner, because function LinLSQRIteration is running!", _state);
    state->prectype = -1;
}


/*************************************************************************
This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
function.  LinCGSolveSparse() will use diagonal of the  system  matrix  as
preconditioner. This preconditioning mode is active by default.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 19.11.2012 by Bochkanov Sergey
*************************************************************************/
void linlsqrsetprecdiag(linlsqrstate* state, ae_state *_state)
{


    ae_assert(!state->running, "LinLSQRSetPrecDiag: you can not change preconditioner, because function LinCGIteration is running!", _state);
    state->prectype = 0;
}


/*************************************************************************
This function sets optional Tikhonov regularization coefficient.
It is zero by default.

INPUT PARAMETERS:
    LambdaI -   regularization factor, LambdaI>=0

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state
    
  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
void linlsqrsetlambdai(linlsqrstate* state,
     double lambdai,
     ae_state *_state)
{


    ae_assert(!state->running, "LinLSQRSetLambdaI: you can not set LambdaI, because function LinLSQRIteration is running", _state);
    ae_assert(ae_isfinite(lambdai, _state)&&ae_fp_greater_eq(lambdai,(double)(0)), "LinLSQRSetLambdaI: LambdaI is infinite or NaN", _state);
    state->lambdai = lambdai;
}


/*************************************************************************

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool linlsqriteration(linlsqrstate* state, ae_state *_state)
{
    ae_int_t summn;
    double bnorm;
    ae_int_t i;
    ae_bool result;


    
    /*
     * Reverse communication preparations
     * I know it looks ugly, but it works the same way
     * anywhere from C++ to Python.
     *
     * This code initializes locals by:
     * * random values determined during code
     *   generation - on first subroutine call
     * * values from previous call - on subsequent calls
     */
    if( state->rstate.stage>=0 )
    {
        summn = state->rstate.ia.ptr.p_int[0];
        i = state->rstate.ia.ptr.p_int[1];
        bnorm = state->rstate.ra.ptr.p_double[0];
    }
    else
    {
        summn = -983;
        i = -989;
        bnorm = -834;
    }
    if( state->rstate.stage==0 )
    {
        goto lbl_0;
    }
    if( state->rstate.stage==1 )
    {
        goto lbl_1;
    }
    if( state->rstate.stage==2 )
    {
        goto lbl_2;
    }
    if( state->rstate.stage==3 )
    {
        goto lbl_3;
    }
    if( state->rstate.stage==4 )
    {
        goto lbl_4;
    }
    if( state->rstate.stage==5 )
    {
        goto lbl_5;
    }
    if( state->rstate.stage==6 )
    {
        goto lbl_6;
    }
    
    /*
     * Routine body
     */
    ae_assert(state->b.cnt>0, "LinLSQRIteration: using non-allocated array B", _state);
    bnorm = ae_sqrt(state->bnorm2, _state);
    state->running = ae_true;
    state->repnmv = 0;
    linlsqr_clearrfields(state, _state);
    state->repiterationscount = 0;
    summn = state->m+state->n;
    state->r2 = state->bnorm2;
    
    /*
     *estimate for ANorm
     */
    normestimatorrestart(&state->nes, _state);
lbl_7:
    if( !normestimatoriteration(&state->nes, _state) )
    {
        goto lbl_8;
    }
    if( !state->nes.needmv )
    {
        goto lbl_9;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->nes.x.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    state->repnmv = state->repnmv+1;
    linlsqr_clearrfields(state, _state);
    state->needmv = ae_true;
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    state->needmv = ae_false;
    ae_v_move(&state->nes.mv.ptr.p_double[0], 1, &state->mv.ptr.p_double[0], 1, ae_v_len(0,state->m-1));
    goto lbl_7;
lbl_9:
    if( !state->nes.needmtv )
    {
        goto lbl_11;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->nes.x.ptr.p_double[0], 1, ae_v_len(0,state->m-1));
    
    /*
     *matrix-vector multiplication
     */
    state->repnmv = state->repnmv+1;
    linlsqr_clearrfields(state, _state);
    state->needmtv = ae_true;
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    state->needmtv = ae_false;
    ae_v_move(&state->nes.mtv.ptr.p_double[0], 1, &state->mtv.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    goto lbl_7;
lbl_11:
    goto lbl_7;
lbl_8:
    normestimatorresults(&state->nes, &state->anorm, _state);
    
    /*
     *initialize .RX by zeros
     */
    for(i=0; i<=state->n-1; i++)
    {
        state->rx.ptr.p_double[i] = (double)(0);
    }
    
    /*
     *output first report
     */
    if( !state->xrep )
    {
        goto lbl_13;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->rx.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    linlsqr_clearrfields(state, _state);
    state->xupdated = ae_true;
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    state->xupdated = ae_false;
lbl_13:
    
    /*
     * LSQR, Step 0.
     *
     * Algorithm outline corresponds to one which was described at p.50 of
     * "LSQR - an algorithm for sparse linear equations and sparse least 
     * squares" by C.Paige and M.Saunders with one small addition - we
     * explicitly extend system matrix by additional N lines in order 
     * to handle non-zero lambda, i.e. original A is replaced by
     *         [ A        ]
     * A_mod = [          ]
     *         [ lambda*I ].
     *
     * Step 0:
     *     x[0]          = 0
     *     beta[1]*u[1]  = b
     *     alpha[1]*v[1] = A_mod'*u[1]
     *     w[1]          = v[1]
     *     phiBar[1]     = beta[1]
     *     rhoBar[1]     = alpha[1]
     *     d[0]          = 0
     *
     * NOTE:
     * There are three criteria for stopping:
     * (S0) maximum number of iterations
     * (S1) ||Rk||<=EpsB*||B||;
     * (S2) ||A^T*Rk||/(||A||*||Rk||)<=EpsA.
     * It is very important that S2 always checked AFTER S1. It is necessary
     * to avoid division by zero when Rk=0.
     */
    state->betai = bnorm;
    if( ae_fp_eq(state->betai,(double)(0)) )
    {
        
        /*
         * Zero right part
         */
        state->running = ae_false;
        state->repterminationtype = 1;
        result = ae_false;
        return result;
    }
    for(i=0; i<=summn-1; i++)
    {
        if( i<state->m )
        {
            state->ui.ptr.p_double[i] = state->b.ptr.p_double[i]/state->betai;
        }
        else
        {
            state->ui.ptr.p_double[i] = (double)(0);
        }
        state->x.ptr.p_double[i] = state->ui.ptr.p_double[i];
    }
    state->repnmv = state->repnmv+1;
    linlsqr_clearrfields(state, _state);
    state->needmtv = ae_true;
    state->rstate.stage = 3;
    goto lbl_rcomm;
lbl_3:
    state->needmtv = ae_false;
    for(i=0; i<=state->n-1; i++)
    {
        state->mtv.ptr.p_double[i] = state->mtv.ptr.p_double[i]+state->lambdai*state->ui.ptr.p_double[state->m+i];
    }
    state->alphai = (double)(0);
    for(i=0; i<=state->n-1; i++)
    {
        state->alphai = state->alphai+state->mtv.ptr.p_double[i]*state->mtv.ptr.p_double[i];
    }
    state->alphai = ae_sqrt(state->alphai, _state);
    if( ae_fp_eq(state->alphai,(double)(0)) )
    {
        
        /*
         * Orthogonality stopping criterion is met
         */
        state->running = ae_false;
        state->repterminationtype = 4;
        result = ae_false;
        return result;
    }
    for(i=0; i<=state->n-1; i++)
    {
        state->vi.ptr.p_double[i] = state->mtv.ptr.p_double[i]/state->alphai;
        state->omegai.ptr.p_double[i] = state->vi.ptr.p_double[i];
    }
    state->phibari = state->betai;
    state->rhobari = state->alphai;
    for(i=0; i<=state->n-1; i++)
    {
        state->d.ptr.p_double[i] = (double)(0);
    }
    state->dnorm = (double)(0);
    
    /*
     * Steps I=1, 2, ...
     */
lbl_15:
    if( ae_false )
    {
        goto lbl_16;
    }
    
    /*
     * At I-th step State.RepIterationsCount=I.
     */
    state->repiterationscount = state->repiterationscount+1;
    
    /*
     * Bidiagonalization part:
     *     beta[i+1]*u[i+1]  = A_mod*v[i]-alpha[i]*u[i]
     *     alpha[i+1]*v[i+1] = A_mod'*u[i+1] - beta[i+1]*v[i]
     *     
     * NOTE:  beta[i+1]=0 or alpha[i+1]=0 will lead to successful termination
     *        in the end of the current iteration. In this case u/v are zero.
     * NOTE2: algorithm won't fail on zero alpha or beta (there will be no
     *        division by zero because it will be stopped BEFORE division
     *        occurs). However, near-zero alpha and beta won't stop algorithm
     *        and, although no division by zero will happen, orthogonality 
     *        in U and V will be lost.
     */
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->vi.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    state->repnmv = state->repnmv+1;
    linlsqr_clearrfields(state, _state);
    state->needmv = ae_true;
    state->rstate.stage = 4;
    goto lbl_rcomm;
lbl_4:
    state->needmv = ae_false;
    for(i=0; i<=state->n-1; i++)
    {
        state->mv.ptr.p_double[state->m+i] = state->lambdai*state->vi.ptr.p_double[i];
    }
    state->betaip1 = (double)(0);
    for(i=0; i<=summn-1; i++)
    {
        state->uip1.ptr.p_double[i] = state->mv.ptr.p_double[i]-state->alphai*state->ui.ptr.p_double[i];
        state->betaip1 = state->betaip1+state->uip1.ptr.p_double[i]*state->uip1.ptr.p_double[i];
    }
    if( ae_fp_neq(state->betaip1,(double)(0)) )
    {
        state->betaip1 = ae_sqrt(state->betaip1, _state);
        for(i=0; i<=summn-1; i++)
        {
            state->uip1.ptr.p_double[i] = state->uip1.ptr.p_double[i]/state->betaip1;
        }
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->uip1.ptr.p_double[0], 1, ae_v_len(0,state->m-1));
    state->repnmv = state->repnmv+1;
    linlsqr_clearrfields(state, _state);
    state->needmtv = ae_true;
    state->rstate.stage = 5;
    goto lbl_rcomm;
lbl_5:
    state->needmtv = ae_false;
    for(i=0; i<=state->n-1; i++)
    {
        state->mtv.ptr.p_double[i] = state->mtv.ptr.p_double[i]+state->lambdai*state->uip1.ptr.p_double[state->m+i];
    }
    state->alphaip1 = (double)(0);
    for(i=0; i<=state->n-1; i++)
    {
        state->vip1.ptr.p_double[i] = state->mtv.ptr.p_double[i]-state->betaip1*state->vi.ptr.p_double[i];
        state->alphaip1 = state->alphaip1+state->vip1.ptr.p_double[i]*state->vip1.ptr.p_double[i];
    }
    if( ae_fp_neq(state->alphaip1,(double)(0)) )
    {
        state->alphaip1 = ae_sqrt(state->alphaip1, _state);
        for(i=0; i<=state->n-1; i++)
        {
            state->vip1.ptr.p_double[i] = state->vip1.ptr.p_double[i]/state->alphaip1;
        }
    }
    
    /*
     * Build next orthogonal transformation
     */
    state->rhoi = safepythag2(state->rhobari, state->betaip1, _state);
    state->ci = state->rhobari/state->rhoi;
    state->si = state->betaip1/state->rhoi;
    state->theta = state->si*state->alphaip1;
    state->rhobarip1 = -state->ci*state->alphaip1;
    state->phii = state->ci*state->phibari;
    state->phibarip1 = state->si*state->phibari;
    
    /*
     * Update .RNorm
     *
     * This tricky  formula  is  necessary  because  simply  writing
     * State.R2:=State.PhiBarIP1*State.PhiBarIP1 does NOT guarantees
     * monotonic decrease of R2. Roundoff error combined with 80-bit
     * precision used internally by Intel chips allows R2 to increase
     * slightly in some rare, but possible cases. This property is
     * undesirable, so we prefer to guard against R increase.
     */
    state->r2 = ae_minreal(state->r2, state->phibarip1*state->phibarip1, _state);
    
    /*
     * Update d and DNorm, check condition-related stopping criteria
     */
    for(i=0; i<=state->n-1; i++)
    {
        state->d.ptr.p_double[i] = 1/state->rhoi*(state->vi.ptr.p_double[i]-state->theta*state->d.ptr.p_double[i]);
        state->dnorm = state->dnorm+state->d.ptr.p_double[i]*state->d.ptr.p_double[i];
    }
    if( ae_fp_greater_eq(ae_sqrt(state->dnorm, _state)*state->anorm,state->epsc) )
    {
        state->running = ae_false;
        state->repterminationtype = 7;
        result = ae_false;
        return result;
    }
    
    /*
     * Update x, output report
     */
    for(i=0; i<=state->n-1; i++)
    {
        state->rx.ptr.p_double[i] = state->rx.ptr.p_double[i]+state->phii/state->rhoi*state->omegai.ptr.p_double[i];
    }
    if( !state->xrep )
    {
        goto lbl_17;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->rx.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    linlsqr_clearrfields(state, _state);
    state->xupdated = ae_true;
    state->rstate.stage = 6;
    goto lbl_rcomm;
lbl_6:
    state->xupdated = ae_false;
lbl_17:
    
    /*
     * Check stopping criteria
     * 1. achieved required number of iterations;
     * 2. ||Rk||<=EpsB*||B||;
     * 3. ||A^T*Rk||/(||A||*||Rk||)<=EpsA;
     */
    if( state->maxits>0&&state->repiterationscount>=state->maxits )
    {
        
        /*
         * Achieved required number of iterations
         */
        state->running = ae_false;
        state->repterminationtype = 5;
        result = ae_false;
        return result;
    }
    if( ae_fp_less_eq(state->phibarip1,state->epsb*bnorm) )
    {
        
        /*
         * ||Rk||<=EpsB*||B||, here ||Rk||=PhiBar
         */
        state->running = ae_false;
        state->repterminationtype = 1;
        result = ae_false;
        return result;
    }
    if( ae_fp_less_eq(state->alphaip1*ae_fabs(state->ci, _state)/state->anorm,state->epsa) )
    {
        
        /*
         * ||A^T*Rk||/(||A||*||Rk||)<=EpsA, here ||A^T*Rk||=PhiBar*Alpha[i+1]*|.C|
         */
        state->running = ae_false;
        state->repterminationtype = 4;
        result = ae_false;
        return result;
    }
    
    /*
     * Update omega
     */
    for(i=0; i<=state->n-1; i++)
    {
        state->omegaip1.ptr.p_double[i] = state->vip1.ptr.p_double[i]-state->theta/state->rhoi*state->omegai.ptr.p_double[i];
    }
    
    /*
     * Prepare for the next iteration - rename variables:
     * u[i]   := u[i+1]
     * v[i]   := v[i+1]
     * rho[i] := rho[i+1]
     * ...
     */
    ae_v_move(&state->ui.ptr.p_double[0], 1, &state->uip1.ptr.p_double[0], 1, ae_v_len(0,summn-1));
    ae_v_move(&state->vi.ptr.p_double[0], 1, &state->vip1.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    ae_v_move(&state->omegai.ptr.p_double[0], 1, &state->omegaip1.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    state->alphai = state->alphaip1;
    state->betai = state->betaip1;
    state->phibari = state->phibarip1;
    state->rhobari = state->rhobarip1;
    goto lbl_15;
lbl_16:
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstate.ia.ptr.p_int[0] = summn;
    state->rstate.ia.ptr.p_int[1] = i;
    state->rstate.ra.ptr.p_double[0] = bnorm;
    return result;
}


/*************************************************************************
Procedure for solution of A*x=b with sparse A.

INPUT PARAMETERS:
    State   -   algorithm state
    A       -   sparse M*N matrix in the CRS format (you MUST contvert  it 
                to CRS format  by  calling  SparseConvertToCRS()  function
                BEFORE you pass it to this function).
    B       -   right part, array[M]

RESULT:
    This function returns no result.
    You can get solution by calling LinCGResults()
    
NOTE: this function uses lightweight preconditioning -  multiplication  by
      inverse of diag(A). If you want, you can turn preconditioning off by
      calling LinLSQRSetPrecUnit(). However, preconditioning cost is   low
      and preconditioner is very important for solution  of  badly  scaled
      problems.

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
void linlsqrsolvesparse(linlsqrstate* state,
     sparsematrix* a,
     /* Real    */ ae_vector* b,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t t0;
    ae_int_t t1;
    double v;


    n = state->n;
    ae_assert(!state->running, "LinLSQRSolveSparse: you can not call this function when LinLSQRIteration is running", _state);
    ae_assert(b->cnt>=state->m, "LinLSQRSolveSparse: Length(B)<M", _state);
    ae_assert(isfinitevector(b, state->m, _state), "LinLSQRSolveSparse: B contains infinite or NaN values", _state);
    
    /*
     * Allocate temporaries
     */
    rvectorsetlengthatleast(&state->tmpd, n, _state);
    rvectorsetlengthatleast(&state->tmpx, n, _state);
    
    /*
     * Compute diagonal scaling matrix D
     */
    if( state->prectype==0 )
    {
        
        /*
         * Default preconditioner - inverse of column norms
         */
        for(i=0; i<=n-1; i++)
        {
            state->tmpd.ptr.p_double[i] = (double)(0);
        }
        t0 = 0;
        t1 = 0;
        while(sparseenumerate(a, &t0, &t1, &i, &j, &v, _state))
        {
            state->tmpd.ptr.p_double[j] = state->tmpd.ptr.p_double[j]+ae_sqr(v, _state);
        }
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_greater(state->tmpd.ptr.p_double[i],(double)(0)) )
            {
                state->tmpd.ptr.p_double[i] = 1/ae_sqrt(state->tmpd.ptr.p_double[i], _state);
            }
            else
            {
                state->tmpd.ptr.p_double[i] = (double)(1);
            }
        }
    }
    else
    {
        
        /*
         * No diagonal scaling
         */
        for(i=0; i<=n-1; i++)
        {
            state->tmpd.ptr.p_double[i] = (double)(1);
        }
    }
    
    /*
     * Solve.
     *
     * Instead of solving A*x=b we solve preconditioned system (A*D)*(inv(D)*x)=b.
     * Transformed A is not calculated explicitly, we just modify multiplication
     * by A or A'. After solution we modify State.RX so it will store untransformed
     * variables
     */
    linlsqrsetb(state, b, _state);
    linlsqrrestart(state, _state);
    while(linlsqriteration(state, _state))
    {
        if( state->needmv )
        {
            for(i=0; i<=n-1; i++)
            {
                state->tmpx.ptr.p_double[i] = state->tmpd.ptr.p_double[i]*state->x.ptr.p_double[i];
            }
            sparsemv(a, &state->tmpx, &state->mv, _state);
        }
        if( state->needmtv )
        {
            sparsemtv(a, &state->x, &state->mtv, _state);
            for(i=0; i<=n-1; i++)
            {
                state->mtv.ptr.p_double[i] = state->tmpd.ptr.p_double[i]*state->mtv.ptr.p_double[i];
            }
        }
    }
    for(i=0; i<=n-1; i++)
    {
        state->rx.ptr.p_double[i] = state->tmpd.ptr.p_double[i]*state->rx.ptr.p_double[i];
    }
}


/*************************************************************************
This function sets stopping criteria.

INPUT PARAMETERS:
    EpsA    -   algorithm will be stopped if ||A^T*Rk||/(||A||*||Rk||)<=EpsA.
    EpsB    -   algorithm will be stopped if ||Rk||<=EpsB*||B||
    MaxIts  -   algorithm will be stopped if number of iterations
                more than MaxIts.

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

NOTE: if EpsA,EpsB,EpsC and MaxIts are zero then these variables will
be setted as default values.
    
  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
void linlsqrsetcond(linlsqrstate* state,
     double epsa,
     double epsb,
     ae_int_t maxits,
     ae_state *_state)
{


    ae_assert(!state->running, "LinLSQRSetCond: you can not call this function when LinLSQRIteration is running", _state);
    ae_assert(ae_isfinite(epsa, _state)&&ae_fp_greater_eq(epsa,(double)(0)), "LinLSQRSetCond: EpsA is negative, INF or NAN", _state);
    ae_assert(ae_isfinite(epsb, _state)&&ae_fp_greater_eq(epsb,(double)(0)), "LinLSQRSetCond: EpsB is negative, INF or NAN", _state);
    ae_assert(maxits>=0, "LinLSQRSetCond: MaxIts is negative", _state);
    if( (ae_fp_eq(epsa,(double)(0))&&ae_fp_eq(epsb,(double)(0)))&&maxits==0 )
    {
        state->epsa = linlsqr_atol;
        state->epsb = linlsqr_btol;
        state->maxits = state->n;
    }
    else
    {
        state->epsa = epsa;
        state->epsb = epsb;
        state->maxits = maxits;
    }
}


/*************************************************************************
LSQR solver: results.

This function must be called after LinLSQRSolve

INPUT PARAMETERS:
    State   -   algorithm state

OUTPUT PARAMETERS:
    X       -   array[N], solution
    Rep     -   optimization report:
                * Rep.TerminationType completetion code:
                    *  1    ||Rk||<=EpsB*||B||
                    *  4    ||A^T*Rk||/(||A||*||Rk||)<=EpsA
                    *  5    MaxIts steps was taken
                    *  7    rounding errors prevent further progress,
                            X contains best point found so far.
                            (sometimes returned on singular systems)
                * Rep.IterationsCount contains iterations count
                * NMV countains number of matrix-vector calculations
                
  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
void linlsqrresults(linlsqrstate* state,
     /* Real    */ ae_vector* x,
     linlsqrreport* rep,
     ae_state *_state)
{

    ae_vector_clear(x);
    _linlsqrreport_clear(rep);

    ae_assert(!state->running, "LinLSQRResult: you can not call this function when LinLSQRIteration is running", _state);
    if( x->cnt<state->n )
    {
        ae_vector_set_length(x, state->n, _state);
    }
    ae_v_move(&x->ptr.p_double[0], 1, &state->rx.ptr.p_double[0], 1, ae_v_len(0,state->n-1));
    rep->iterationscount = state->repiterationscount;
    rep->nmv = state->repnmv;
    rep->terminationtype = state->repterminationtype;
}


/*************************************************************************
This function turns on/off reporting.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    NeedXRep-   whether iteration reports are needed or not

If NeedXRep is True, algorithm will call rep() callback function if  it is
provided to MinCGOptimize().

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
void linlsqrsetxrep(linlsqrstate* state,
     ae_bool needxrep,
     ae_state *_state)
{


    state->xrep = needxrep;
}


/*************************************************************************
This function restarts LinLSQRIteration

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
void linlsqrrestart(linlsqrstate* state, ae_state *_state)
{


    ae_vector_set_length(&state->rstate.ia, 1+1, _state);
    ae_vector_set_length(&state->rstate.ra, 0+1, _state);
    state->rstate.stage = -1;
    linlsqr_clearrfields(state, _state);
}


/*************************************************************************
Clears request fileds (to be sure that we don't forgot to clear something)
*************************************************************************/
static void linlsqr_clearrfields(linlsqrstate* state, ae_state *_state)
{


    state->xupdated = ae_false;
    state->needmv = ae_false;
    state->needmtv = ae_false;
    state->needmv2 = ae_false;
    state->needvmv = ae_false;
    state->needprec = ae_false;
}


void _linlsqrstate_init(void* _p, ae_state *_state)
{
    linlsqrstate *p = (linlsqrstate*)_p;
    ae_touch_ptr((void*)p);
    _normestimatorstate_init(&p->nes, _state);
    ae_vector_init(&p->rx, 0, DT_REAL, _state);
    ae_vector_init(&p->b, 0, DT_REAL, _state);
    ae_vector_init(&p->ui, 0, DT_REAL, _state);
    ae_vector_init(&p->uip1, 0, DT_REAL, _state);
    ae_vector_init(&p->vi, 0, DT_REAL, _state);
    ae_vector_init(&p->vip1, 0, DT_REAL, _state);
    ae_vector_init(&p->omegai, 0, DT_REAL, _state);
    ae_vector_init(&p->omegaip1, 0, DT_REAL, _state);
    ae_vector_init(&p->d, 0, DT_REAL, _state);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->mv, 0, DT_REAL, _state);
    ae_vector_init(&p->mtv, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpd, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpx, 0, DT_REAL, _state);
    _rcommstate_init(&p->rstate, _state);
}


void _linlsqrstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    linlsqrstate *dst = (linlsqrstate*)_dst;
    linlsqrstate *src = (linlsqrstate*)_src;
    _normestimatorstate_init_copy(&dst->nes, &src->nes, _state);
    ae_vector_init_copy(&dst->rx, &src->rx, _state);
    ae_vector_init_copy(&dst->b, &src->b, _state);
    dst->n = src->n;
    dst->m = src->m;
    dst->prectype = src->prectype;
    ae_vector_init_copy(&dst->ui, &src->ui, _state);
    ae_vector_init_copy(&dst->uip1, &src->uip1, _state);
    ae_vector_init_copy(&dst->vi, &src->vi, _state);
    ae_vector_init_copy(&dst->vip1, &src->vip1, _state);
    ae_vector_init_copy(&dst->omegai, &src->omegai, _state);
    ae_vector_init_copy(&dst->omegaip1, &src->omegaip1, _state);
    dst->alphai = src->alphai;
    dst->alphaip1 = src->alphaip1;
    dst->betai = src->betai;
    dst->betaip1 = src->betaip1;
    dst->phibari = src->phibari;
    dst->phibarip1 = src->phibarip1;
    dst->phii = src->phii;
    dst->rhobari = src->rhobari;
    dst->rhobarip1 = src->rhobarip1;
    dst->rhoi = src->rhoi;
    dst->ci = src->ci;
    dst->si = src->si;
    dst->theta = src->theta;
    dst->lambdai = src->lambdai;
    ae_vector_init_copy(&dst->d, &src->d, _state);
    dst->anorm = src->anorm;
    dst->bnorm2 = src->bnorm2;
    dst->dnorm = src->dnorm;
    dst->r2 = src->r2;
    ae_vector_init_copy(&dst->x, &src->x, _state);
    ae_vector_init_copy(&dst->mv, &src->mv, _state);
    ae_vector_init_copy(&dst->mtv, &src->mtv, _state);
    dst->epsa = src->epsa;
    dst->epsb = src->epsb;
    dst->epsc = src->epsc;
    dst->maxits = src->maxits;
    dst->xrep = src->xrep;
    dst->xupdated = src->xupdated;
    dst->needmv = src->needmv;
    dst->needmtv = src->needmtv;
    dst->needmv2 = src->needmv2;
    dst->needvmv = src->needvmv;
    dst->needprec = src->needprec;
    dst->repiterationscount = src->repiterationscount;
    dst->repnmv = src->repnmv;
    dst->repterminationtype = src->repterminationtype;
    dst->running = src->running;
    ae_vector_init_copy(&dst->tmpd, &src->tmpd, _state);
    ae_vector_init_copy(&dst->tmpx, &src->tmpx, _state);
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
}


void _linlsqrstate_clear(void* _p)
{
    linlsqrstate *p = (linlsqrstate*)_p;
    ae_touch_ptr((void*)p);
    _normestimatorstate_clear(&p->nes);
    ae_vector_clear(&p->rx);
    ae_vector_clear(&p->b);
    ae_vector_clear(&p->ui);
    ae_vector_clear(&p->uip1);
    ae_vector_clear(&p->vi);
    ae_vector_clear(&p->vip1);
    ae_vector_clear(&p->omegai);
    ae_vector_clear(&p->omegaip1);
    ae_vector_clear(&p->d);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->mv);
    ae_vector_clear(&p->mtv);
    ae_vector_clear(&p->tmpd);
    ae_vector_clear(&p->tmpx);
    _rcommstate_clear(&p->rstate);
}


void _linlsqrstate_destroy(void* _p)
{
    linlsqrstate *p = (linlsqrstate*)_p;
    ae_touch_ptr((void*)p);
    _normestimatorstate_destroy(&p->nes);
    ae_vector_destroy(&p->rx);
    ae_vector_destroy(&p->b);
    ae_vector_destroy(&p->ui);
    ae_vector_destroy(&p->uip1);
    ae_vector_destroy(&p->vi);
    ae_vector_destroy(&p->vip1);
    ae_vector_destroy(&p->omegai);
    ae_vector_destroy(&p->omegaip1);
    ae_vector_destroy(&p->d);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->mv);
    ae_vector_destroy(&p->mtv);
    ae_vector_destroy(&p->tmpd);
    ae_vector_destroy(&p->tmpx);
    _rcommstate_destroy(&p->rstate);
}


void _linlsqrreport_init(void* _p, ae_state *_state)
{
    linlsqrreport *p = (linlsqrreport*)_p;
    ae_touch_ptr((void*)p);
}


void _linlsqrreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    linlsqrreport *dst = (linlsqrreport*)_dst;
    linlsqrreport *src = (linlsqrreport*)_src;
    dst->iterationscount = src->iterationscount;
    dst->nmv = src->nmv;
    dst->terminationtype = src->terminationtype;
}


void _linlsqrreport_clear(void* _p)
{
    linlsqrreport *p = (linlsqrreport*)_p;
    ae_touch_ptr((void*)p);
}


void _linlsqrreport_destroy(void* _p)
{
    linlsqrreport *p = (linlsqrreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
