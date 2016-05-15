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
#include "normestimator.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
This procedure initializes matrix norm estimator.

USAGE:
1. User initializes algorithm state with NormEstimatorCreate() call
2. User calls NormEstimatorEstimateSparse() (or NormEstimatorIteration())
3. User calls NormEstimatorResults() to get solution.
   
INPUT PARAMETERS:
    M       -   number of rows in the matrix being estimated, M>0
    N       -   number of columns in the matrix being estimated, N>0
    NStart  -   number of random starting vectors
                recommended value - at least 5.
    NIts    -   number of iterations to do with best starting vector
                recommended value - at least 5.

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

    
NOTE: this algorithm is effectively deterministic, i.e. it always  returns
same result when repeatedly called for the same matrix. In fact, algorithm
uses randomized starting vectors, but internal  random  numbers  generator
always generates same sequence of the random values (it is a  feature, not
bug).

Algorithm can be made non-deterministic with NormEstimatorSetSeed(0) call.

  -- ALGLIB --
     Copyright 06.12.2011 by Bochkanov Sergey
*************************************************************************/
void normestimatorcreate(ae_int_t m,
     ae_int_t n,
     ae_int_t nstart,
     ae_int_t nits,
     normestimatorstate* state,
     ae_state *_state)
{

    _normestimatorstate_clear(state);

    ae_assert(m>0, "NormEstimatorCreate: M<=0", _state);
    ae_assert(n>0, "NormEstimatorCreate: N<=0", _state);
    ae_assert(nstart>0, "NormEstimatorCreate: NStart<=0", _state);
    ae_assert(nits>0, "NormEstimatorCreate: NIts<=0", _state);
    state->m = m;
    state->n = n;
    state->nstart = nstart;
    state->nits = nits;
    state->seedval = 11;
    hqrndrandomize(&state->r, _state);
    ae_vector_set_length(&state->x0, state->n, _state);
    ae_vector_set_length(&state->t, state->m, _state);
    ae_vector_set_length(&state->x1, state->n, _state);
    ae_vector_set_length(&state->xbest, state->n, _state);
    ae_vector_set_length(&state->x, ae_maxint(state->n, state->m, _state), _state);
    ae_vector_set_length(&state->mv, state->m, _state);
    ae_vector_set_length(&state->mtv, state->n, _state);
    ae_vector_set_length(&state->rstate.ia, 3+1, _state);
    ae_vector_set_length(&state->rstate.ra, 2+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
This function changes seed value used by algorithm. In some cases we  need
deterministic processing, i.e. subsequent calls must return equal results,
in other cases we need non-deterministic algorithm which returns different
results for the same matrix on every pass.

Setting zero seed will lead to non-deterministic algorithm, while non-zero 
value will make our algorithm deterministic.

INPUT PARAMETERS:
    State       -   norm estimator state, must be initialized with a  call
                    to NormEstimatorCreate()
    SeedVal     -   seed value, >=0. Zero value = non-deterministic algo.

  -- ALGLIB --
     Copyright 06.12.2011 by Bochkanov Sergey
*************************************************************************/
void normestimatorsetseed(normestimatorstate* state,
     ae_int_t seedval,
     ae_state *_state)
{


    ae_assert(seedval>=0, "NormEstimatorSetSeed: SeedVal<0", _state);
    state->seedval = seedval;
}


/*************************************************************************

  -- ALGLIB --
     Copyright 06.12.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool normestimatoriteration(normestimatorstate* state,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    ae_int_t itcnt;
    double v;
    double growth;
    double bestgrowth;
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
        n = state->rstate.ia.ptr.p_int[0];
        m = state->rstate.ia.ptr.p_int[1];
        i = state->rstate.ia.ptr.p_int[2];
        itcnt = state->rstate.ia.ptr.p_int[3];
        v = state->rstate.ra.ptr.p_double[0];
        growth = state->rstate.ra.ptr.p_double[1];
        bestgrowth = state->rstate.ra.ptr.p_double[2];
    }
    else
    {
        n = -983;
        m = -989;
        i = -834;
        itcnt = 900;
        v = -287;
        growth = 364;
        bestgrowth = 214;
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
    
    /*
     * Routine body
     */
    n = state->n;
    m = state->m;
    if( state->seedval>0 )
    {
        hqrndseed(state->seedval, state->seedval+2, &state->r, _state);
    }
    bestgrowth = (double)(0);
    state->xbest.ptr.p_double[0] = (double)(1);
    for(i=1; i<=n-1; i++)
    {
        state->xbest.ptr.p_double[i] = (double)(0);
    }
    itcnt = 0;
lbl_4:
    if( itcnt>state->nstart-1 )
    {
        goto lbl_6;
    }
    do
    {
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            state->x0.ptr.p_double[i] = hqrndnormal(&state->r, _state);
            v = v+ae_sqr(state->x0.ptr.p_double[i], _state);
        }
    }
    while(ae_fp_eq(v,(double)(0)));
    v = 1/ae_sqrt(v, _state);
    ae_v_muld(&state->x0.ptr.p_double[0], 1, ae_v_len(0,n-1), v);
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->needmv = ae_true;
    state->needmtv = ae_false;
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->mv.ptr.p_double[0], 1, ae_v_len(0,m-1));
    state->needmv = ae_false;
    state->needmtv = ae_true;
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    ae_v_move(&state->x1.ptr.p_double[0], 1, &state->mtv.ptr.p_double[0], 1, ae_v_len(0,n-1));
    v = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        v = v+ae_sqr(state->x1.ptr.p_double[i], _state);
    }
    growth = ae_sqrt(ae_sqrt(v, _state), _state);
    if( ae_fp_greater(growth,bestgrowth) )
    {
        v = 1/ae_sqrt(v, _state);
        ae_v_moved(&state->xbest.ptr.p_double[0], 1, &state->x1.ptr.p_double[0], 1, ae_v_len(0,n-1), v);
        bestgrowth = growth;
    }
    itcnt = itcnt+1;
    goto lbl_4;
lbl_6:
    ae_v_move(&state->x0.ptr.p_double[0], 1, &state->xbest.ptr.p_double[0], 1, ae_v_len(0,n-1));
    itcnt = 0;
lbl_7:
    if( itcnt>state->nits-1 )
    {
        goto lbl_9;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->needmv = ae_true;
    state->needmtv = ae_false;
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->mv.ptr.p_double[0], 1, ae_v_len(0,m-1));
    state->needmv = ae_false;
    state->needmtv = ae_true;
    state->rstate.stage = 3;
    goto lbl_rcomm;
lbl_3:
    ae_v_move(&state->x1.ptr.p_double[0], 1, &state->mtv.ptr.p_double[0], 1, ae_v_len(0,n-1));
    v = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        v = v+ae_sqr(state->x1.ptr.p_double[i], _state);
    }
    state->repnorm = ae_sqrt(ae_sqrt(v, _state), _state);
    if( ae_fp_neq(v,(double)(0)) )
    {
        v = 1/ae_sqrt(v, _state);
        ae_v_moved(&state->x0.ptr.p_double[0], 1, &state->x1.ptr.p_double[0], 1, ae_v_len(0,n-1), v);
    }
    itcnt = itcnt+1;
    goto lbl_7;
lbl_9:
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstate.ia.ptr.p_int[0] = n;
    state->rstate.ia.ptr.p_int[1] = m;
    state->rstate.ia.ptr.p_int[2] = i;
    state->rstate.ia.ptr.p_int[3] = itcnt;
    state->rstate.ra.ptr.p_double[0] = v;
    state->rstate.ra.ptr.p_double[1] = growth;
    state->rstate.ra.ptr.p_double[2] = bestgrowth;
    return result;
}


/*************************************************************************
This function estimates norm of the sparse M*N matrix A.

INPUT PARAMETERS:
    State       -   norm estimator state, must be initialized with a  call
                    to NormEstimatorCreate()
    A           -   sparse M*N matrix, must be converted to CRS format
                    prior to calling this function.

After this function  is  over  you can call NormEstimatorResults() to get 
estimate of the norm(A).

  -- ALGLIB --
     Copyright 06.12.2011 by Bochkanov Sergey
*************************************************************************/
void normestimatorestimatesparse(normestimatorstate* state,
     sparsematrix* a,
     ae_state *_state)
{


    normestimatorrestart(state, _state);
    while(normestimatoriteration(state, _state))
    {
        if( state->needmv )
        {
            sparsemv(a, &state->x, &state->mv, _state);
            continue;
        }
        if( state->needmtv )
        {
            sparsemtv(a, &state->x, &state->mtv, _state);
            continue;
        }
    }
}


/*************************************************************************
Matrix norm estimation results

INPUT PARAMETERS:
    State   -   algorithm state

OUTPUT PARAMETERS:
    Nrm     -   estimate of the matrix norm, Nrm>=0

  -- ALGLIB --
     Copyright 06.12.2011 by Bochkanov Sergey
*************************************************************************/
void normestimatorresults(normestimatorstate* state,
     double* nrm,
     ae_state *_state)
{

    *nrm = 0;

    *nrm = state->repnorm;
}


/*************************************************************************
This  function  restarts estimator and prepares it for the next estimation
round.

INPUT PARAMETERS:
    State   -   algorithm state
  -- ALGLIB --
     Copyright 06.12.2011 by Bochkanov Sergey
*************************************************************************/
void normestimatorrestart(normestimatorstate* state, ae_state *_state)
{


    ae_vector_set_length(&state->rstate.ia, 3+1, _state);
    ae_vector_set_length(&state->rstate.ra, 2+1, _state);
    state->rstate.stage = -1;
}


void _normestimatorstate_init(void* _p, ae_state *_state)
{
    normestimatorstate *p = (normestimatorstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->x0, 0, DT_REAL, _state);
    ae_vector_init(&p->x1, 0, DT_REAL, _state);
    ae_vector_init(&p->t, 0, DT_REAL, _state);
    ae_vector_init(&p->xbest, 0, DT_REAL, _state);
    _hqrndstate_init(&p->r, _state);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->mv, 0, DT_REAL, _state);
    ae_vector_init(&p->mtv, 0, DT_REAL, _state);
    _rcommstate_init(&p->rstate, _state);
}


void _normestimatorstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    normestimatorstate *dst = (normestimatorstate*)_dst;
    normestimatorstate *src = (normestimatorstate*)_src;
    dst->n = src->n;
    dst->m = src->m;
    dst->nstart = src->nstart;
    dst->nits = src->nits;
    dst->seedval = src->seedval;
    ae_vector_init_copy(&dst->x0, &src->x0, _state);
    ae_vector_init_copy(&dst->x1, &src->x1, _state);
    ae_vector_init_copy(&dst->t, &src->t, _state);
    ae_vector_init_copy(&dst->xbest, &src->xbest, _state);
    _hqrndstate_init_copy(&dst->r, &src->r, _state);
    ae_vector_init_copy(&dst->x, &src->x, _state);
    ae_vector_init_copy(&dst->mv, &src->mv, _state);
    ae_vector_init_copy(&dst->mtv, &src->mtv, _state);
    dst->needmv = src->needmv;
    dst->needmtv = src->needmtv;
    dst->repnorm = src->repnorm;
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
}


void _normestimatorstate_clear(void* _p)
{
    normestimatorstate *p = (normestimatorstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->x0);
    ae_vector_clear(&p->x1);
    ae_vector_clear(&p->t);
    ae_vector_clear(&p->xbest);
    _hqrndstate_clear(&p->r);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->mv);
    ae_vector_clear(&p->mtv);
    _rcommstate_clear(&p->rstate);
}


void _normestimatorstate_destroy(void* _p)
{
    normestimatorstate *p = (normestimatorstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->x0);
    ae_vector_destroy(&p->x1);
    ae_vector_destroy(&p->t);
    ae_vector_destroy(&p->xbest);
    _hqrndstate_destroy(&p->r);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->mv);
    ae_vector_destroy(&p->mtv);
    _rcommstate_destroy(&p->rstate);
}


/*$ End $*/
