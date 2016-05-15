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
#include "fbls.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Basic Cholesky solver for ScaleA*Cholesky(A)'*x = y.

This subroutine assumes that:
* A*ScaleA is well scaled
* A is well-conditioned, so no zero divisions or overflow may occur

INPUT PARAMETERS:
    CHA     -   Cholesky decomposition of A
    SqrtScaleA- square root of scale factor ScaleA
    N       -   matrix size, N>=0.
    IsUpper -   storage type
    XB      -   right part
    Tmp     -   buffer; function automatically allocates it, if it is  too
                small.  It  can  be  reused  if function is called several
                times.
                
OUTPUT PARAMETERS:
    XB      -   solution

NOTE 1: no assertion or tests are done during algorithm operation
NOTE 2: N=0 will force algorithm to silently return

  -- ALGLIB --
     Copyright 13.10.2010 by Bochkanov Sergey
*************************************************************************/
void fblscholeskysolve(/* Real    */ ae_matrix* cha,
     double sqrtscalea,
     ae_int_t n,
     ae_bool isupper,
     /* Real    */ ae_vector* xb,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;
    double v;


    if( n==0 )
    {
        return;
    }
    if( tmp->cnt<n )
    {
        ae_vector_set_length(tmp, n, _state);
    }
    
    /*
     * A = L*L' or A=U'*U
     */
    if( isupper )
    {
        
        /*
         * Solve U'*y=b first.
         */
        for(i=0; i<=n-1; i++)
        {
            xb->ptr.p_double[i] = xb->ptr.p_double[i]/(sqrtscalea*cha->ptr.pp_double[i][i]);
            if( i<n-1 )
            {
                v = xb->ptr.p_double[i];
                ae_v_moved(&tmp->ptr.p_double[i+1], 1, &cha->ptr.pp_double[i][i+1], 1, ae_v_len(i+1,n-1), sqrtscalea);
                ae_v_subd(&xb->ptr.p_double[i+1], 1, &tmp->ptr.p_double[i+1], 1, ae_v_len(i+1,n-1), v);
            }
        }
        
        /*
         * Solve U*x=y then.
         */
        for(i=n-1; i>=0; i--)
        {
            if( i<n-1 )
            {
                ae_v_moved(&tmp->ptr.p_double[i+1], 1, &cha->ptr.pp_double[i][i+1], 1, ae_v_len(i+1,n-1), sqrtscalea);
                v = ae_v_dotproduct(&tmp->ptr.p_double[i+1], 1, &xb->ptr.p_double[i+1], 1, ae_v_len(i+1,n-1));
                xb->ptr.p_double[i] = xb->ptr.p_double[i]-v;
            }
            xb->ptr.p_double[i] = xb->ptr.p_double[i]/(sqrtscalea*cha->ptr.pp_double[i][i]);
        }
    }
    else
    {
        
        /*
         * Solve L*y=b first
         */
        for(i=0; i<=n-1; i++)
        {
            if( i>0 )
            {
                ae_v_moved(&tmp->ptr.p_double[0], 1, &cha->ptr.pp_double[i][0], 1, ae_v_len(0,i-1), sqrtscalea);
                v = ae_v_dotproduct(&tmp->ptr.p_double[0], 1, &xb->ptr.p_double[0], 1, ae_v_len(0,i-1));
                xb->ptr.p_double[i] = xb->ptr.p_double[i]-v;
            }
            xb->ptr.p_double[i] = xb->ptr.p_double[i]/(sqrtscalea*cha->ptr.pp_double[i][i]);
        }
        
        /*
         * Solve L'*x=y then.
         */
        for(i=n-1; i>=0; i--)
        {
            xb->ptr.p_double[i] = xb->ptr.p_double[i]/(sqrtscalea*cha->ptr.pp_double[i][i]);
            if( i>0 )
            {
                v = xb->ptr.p_double[i];
                ae_v_moved(&tmp->ptr.p_double[0], 1, &cha->ptr.pp_double[i][0], 1, ae_v_len(0,i-1), sqrtscalea);
                ae_v_subd(&xb->ptr.p_double[0], 1, &tmp->ptr.p_double[0], 1, ae_v_len(0,i-1), v);
            }
        }
    }
}


/*************************************************************************
Fast basic linear solver: linear SPD CG

Solves (A^T*A + alpha*I)*x = b where:
* A is MxN matrix
* alpha>0 is a scalar
* I is NxN identity matrix
* b is Nx1 vector
* X is Nx1 unknown vector.

N iterations of linear conjugate gradient are used to solve problem.

INPUT PARAMETERS:
    A   -   array[M,N], matrix
    M   -   number of rows
    N   -   number of unknowns
    B   -   array[N], right part
    X   -   initial approxumation, array[N]
    Buf -   buffer; function automatically allocates it, if it is too
            small. It can be reused if function is called several times
            with same M and N.
            
OUTPUT PARAMETERS:
    X   -   improved solution
    
NOTES:
*   solver checks quality of improved solution. If (because of problem
    condition number, numerical noise, etc.) new solution is WORSE than
    original approximation, then original approximation is returned.
*   solver assumes that both A, B, Alpha are well scaled (i.e. they are
    less than sqrt(overflow) and greater than sqrt(underflow)).
    
  -- ALGLIB --
     Copyright 20.08.2009 by Bochkanov Sergey
*************************************************************************/
void fblssolvecgx(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double alpha,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* buf,
     ae_state *_state)
{
    ae_int_t k;
    ae_int_t offsrk;
    ae_int_t offsrk1;
    ae_int_t offsxk;
    ae_int_t offsxk1;
    ae_int_t offspk;
    ae_int_t offspk1;
    ae_int_t offstmp1;
    ae_int_t offstmp2;
    ae_int_t bs;
    double e1;
    double e2;
    double rk2;
    double rk12;
    double pap;
    double s;
    double betak;
    double v1;
    double v2;


    
    /*
     * Test for special case: B=0
     */
    v1 = ae_v_dotproduct(&b->ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
    if( ae_fp_eq(v1,(double)(0)) )
    {
        for(k=0; k<=n-1; k++)
        {
            x->ptr.p_double[k] = (double)(0);
        }
        return;
    }
    
    /*
     * Offsets inside Buf for:
     * * R[K], R[K+1]
     * * X[K], X[K+1]
     * * P[K], P[K+1]
     * * Tmp1 - array[M], Tmp2 - array[N]
     */
    offsrk = 0;
    offsrk1 = offsrk+n;
    offsxk = offsrk1+n;
    offsxk1 = offsxk+n;
    offspk = offsxk1+n;
    offspk1 = offspk+n;
    offstmp1 = offspk1+n;
    offstmp2 = offstmp1+m;
    bs = offstmp2+n;
    if( buf->cnt<bs )
    {
        ae_vector_set_length(buf, bs, _state);
    }
    
    /*
     * x(0) = x
     */
    ae_v_move(&buf->ptr.p_double[offsxk], 1, &x->ptr.p_double[0], 1, ae_v_len(offsxk,offsxk+n-1));
    
    /*
     * r(0) = b-A*x(0)
     * RK2 = r(0)'*r(0)
     */
    rmatrixmv(m, n, a, 0, 0, 0, buf, offsxk, buf, offstmp1, _state);
    rmatrixmv(n, m, a, 0, 0, 1, buf, offstmp1, buf, offstmp2, _state);
    ae_v_addd(&buf->ptr.p_double[offstmp2], 1, &buf->ptr.p_double[offsxk], 1, ae_v_len(offstmp2,offstmp2+n-1), alpha);
    ae_v_move(&buf->ptr.p_double[offsrk], 1, &b->ptr.p_double[0], 1, ae_v_len(offsrk,offsrk+n-1));
    ae_v_sub(&buf->ptr.p_double[offsrk], 1, &buf->ptr.p_double[offstmp2], 1, ae_v_len(offsrk,offsrk+n-1));
    rk2 = ae_v_dotproduct(&buf->ptr.p_double[offsrk], 1, &buf->ptr.p_double[offsrk], 1, ae_v_len(offsrk,offsrk+n-1));
    ae_v_move(&buf->ptr.p_double[offspk], 1, &buf->ptr.p_double[offsrk], 1, ae_v_len(offspk,offspk+n-1));
    e1 = ae_sqrt(rk2, _state);
    
    /*
     * Cycle
     */
    for(k=0; k<=n-1; k++)
    {
        
        /*
         * Calculate A*p(k) - store in Buf[OffsTmp2:OffsTmp2+N-1]
         * and p(k)'*A*p(k)  - store in PAP
         *
         * If PAP=0, break (iteration is over)
         */
        rmatrixmv(m, n, a, 0, 0, 0, buf, offspk, buf, offstmp1, _state);
        v1 = ae_v_dotproduct(&buf->ptr.p_double[offstmp1], 1, &buf->ptr.p_double[offstmp1], 1, ae_v_len(offstmp1,offstmp1+m-1));
        v2 = ae_v_dotproduct(&buf->ptr.p_double[offspk], 1, &buf->ptr.p_double[offspk], 1, ae_v_len(offspk,offspk+n-1));
        pap = v1+alpha*v2;
        rmatrixmv(n, m, a, 0, 0, 1, buf, offstmp1, buf, offstmp2, _state);
        ae_v_addd(&buf->ptr.p_double[offstmp2], 1, &buf->ptr.p_double[offspk], 1, ae_v_len(offstmp2,offstmp2+n-1), alpha);
        if( ae_fp_eq(pap,(double)(0)) )
        {
            break;
        }
        
        /*
         * S = (r(k)'*r(k))/(p(k)'*A*p(k))
         */
        s = rk2/pap;
        
        /*
         * x(k+1) = x(k) + S*p(k)
         */
        ae_v_move(&buf->ptr.p_double[offsxk1], 1, &buf->ptr.p_double[offsxk], 1, ae_v_len(offsxk1,offsxk1+n-1));
        ae_v_addd(&buf->ptr.p_double[offsxk1], 1, &buf->ptr.p_double[offspk], 1, ae_v_len(offsxk1,offsxk1+n-1), s);
        
        /*
         * r(k+1) = r(k) - S*A*p(k)
         * RK12 = r(k+1)'*r(k+1)
         *
         * Break if r(k+1) small enough (when compared to r(k))
         */
        ae_v_move(&buf->ptr.p_double[offsrk1], 1, &buf->ptr.p_double[offsrk], 1, ae_v_len(offsrk1,offsrk1+n-1));
        ae_v_subd(&buf->ptr.p_double[offsrk1], 1, &buf->ptr.p_double[offstmp2], 1, ae_v_len(offsrk1,offsrk1+n-1), s);
        rk12 = ae_v_dotproduct(&buf->ptr.p_double[offsrk1], 1, &buf->ptr.p_double[offsrk1], 1, ae_v_len(offsrk1,offsrk1+n-1));
        if( ae_fp_less_eq(ae_sqrt(rk12, _state),100*ae_machineepsilon*ae_sqrt(rk2, _state)) )
        {
            
            /*
             * X(k) = x(k+1) before exit -
             * - because we expect to find solution at x(k)
             */
            ae_v_move(&buf->ptr.p_double[offsxk], 1, &buf->ptr.p_double[offsxk1], 1, ae_v_len(offsxk,offsxk+n-1));
            break;
        }
        
        /*
         * BetaK = RK12/RK2
         * p(k+1) = r(k+1)+betak*p(k)
         */
        betak = rk12/rk2;
        ae_v_move(&buf->ptr.p_double[offspk1], 1, &buf->ptr.p_double[offsrk1], 1, ae_v_len(offspk1,offspk1+n-1));
        ae_v_addd(&buf->ptr.p_double[offspk1], 1, &buf->ptr.p_double[offspk], 1, ae_v_len(offspk1,offspk1+n-1), betak);
        
        /*
         * r(k) := r(k+1)
         * x(k) := x(k+1)
         * p(k) := p(k+1)
         */
        ae_v_move(&buf->ptr.p_double[offsrk], 1, &buf->ptr.p_double[offsrk1], 1, ae_v_len(offsrk,offsrk+n-1));
        ae_v_move(&buf->ptr.p_double[offsxk], 1, &buf->ptr.p_double[offsxk1], 1, ae_v_len(offsxk,offsxk+n-1));
        ae_v_move(&buf->ptr.p_double[offspk], 1, &buf->ptr.p_double[offspk1], 1, ae_v_len(offspk,offspk+n-1));
        rk2 = rk12;
    }
    
    /*
     * Calculate E2
     */
    rmatrixmv(m, n, a, 0, 0, 0, buf, offsxk, buf, offstmp1, _state);
    rmatrixmv(n, m, a, 0, 0, 1, buf, offstmp1, buf, offstmp2, _state);
    ae_v_addd(&buf->ptr.p_double[offstmp2], 1, &buf->ptr.p_double[offsxk], 1, ae_v_len(offstmp2,offstmp2+n-1), alpha);
    ae_v_move(&buf->ptr.p_double[offsrk], 1, &b->ptr.p_double[0], 1, ae_v_len(offsrk,offsrk+n-1));
    ae_v_sub(&buf->ptr.p_double[offsrk], 1, &buf->ptr.p_double[offstmp2], 1, ae_v_len(offsrk,offsrk+n-1));
    v1 = ae_v_dotproduct(&buf->ptr.p_double[offsrk], 1, &buf->ptr.p_double[offsrk], 1, ae_v_len(offsrk,offsrk+n-1));
    e2 = ae_sqrt(v1, _state);
    
    /*
     * Output result (if it was improved)
     */
    if( ae_fp_less(e2,e1) )
    {
        ae_v_move(&x->ptr.p_double[0], 1, &buf->ptr.p_double[offsxk], 1, ae_v_len(0,n-1));
    }
}


/*************************************************************************
Construction of linear conjugate gradient solver.

State parameter passed using "var" semantics (i.e. previous state  is  NOT
erased). When it is already initialized, we can reause prevously allocated
memory.

INPUT PARAMETERS:
    X       -   initial solution
    B       -   right part
    N       -   system size
    State   -   structure; may be preallocated, if we want to reuse memory

OUTPUT PARAMETERS:
    State   -   structure which is used by FBLSCGIteration() to store
                algorithm state between subsequent calls.

NOTE: no error checking is done; caller must check all parameters, prevent
      overflows, and so on.

  -- ALGLIB --
     Copyright 22.10.2009 by Bochkanov Sergey
*************************************************************************/
void fblscgcreate(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* b,
     ae_int_t n,
     fblslincgstate* state,
     ae_state *_state)
{


    if( state->b.cnt<n )
    {
        ae_vector_set_length(&state->b, n, _state);
    }
    if( state->rk.cnt<n )
    {
        ae_vector_set_length(&state->rk, n, _state);
    }
    if( state->rk1.cnt<n )
    {
        ae_vector_set_length(&state->rk1, n, _state);
    }
    if( state->xk.cnt<n )
    {
        ae_vector_set_length(&state->xk, n, _state);
    }
    if( state->xk1.cnt<n )
    {
        ae_vector_set_length(&state->xk1, n, _state);
    }
    if( state->pk.cnt<n )
    {
        ae_vector_set_length(&state->pk, n, _state);
    }
    if( state->pk1.cnt<n )
    {
        ae_vector_set_length(&state->pk1, n, _state);
    }
    if( state->tmp2.cnt<n )
    {
        ae_vector_set_length(&state->tmp2, n, _state);
    }
    if( state->x.cnt<n )
    {
        ae_vector_set_length(&state->x, n, _state);
    }
    if( state->ax.cnt<n )
    {
        ae_vector_set_length(&state->ax, n, _state);
    }
    state->n = n;
    ae_v_move(&state->xk.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&state->b.ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_vector_set_length(&state->rstate.ia, 1+1, _state);
    ae_vector_set_length(&state->rstate.ra, 6+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
Linear CG solver, function relying on reverse communication to calculate
matrix-vector products.

See comments for FBLSLinCGState structure for more info.

  -- ALGLIB --
     Copyright 22.10.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool fblscgiteration(fblslincgstate* state, ae_state *_state)
{
    ae_int_t n;
    ae_int_t k;
    double rk2;
    double rk12;
    double pap;
    double s;
    double betak;
    double v1;
    double v2;
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
        k = state->rstate.ia.ptr.p_int[1];
        rk2 = state->rstate.ra.ptr.p_double[0];
        rk12 = state->rstate.ra.ptr.p_double[1];
        pap = state->rstate.ra.ptr.p_double[2];
        s = state->rstate.ra.ptr.p_double[3];
        betak = state->rstate.ra.ptr.p_double[4];
        v1 = state->rstate.ra.ptr.p_double[5];
        v2 = state->rstate.ra.ptr.p_double[6];
    }
    else
    {
        n = -983;
        k = -989;
        rk2 = -834;
        rk12 = 900;
        pap = -287;
        s = 364;
        betak = 214;
        v1 = -338;
        v2 = -686;
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
    
    /*
     * Routine body
     */
    
    /*
     * prepare locals
     */
    n = state->n;
    
    /*
     * Test for special case: B=0
     */
    v1 = ae_v_dotproduct(&state->b.ptr.p_double[0], 1, &state->b.ptr.p_double[0], 1, ae_v_len(0,n-1));
    if( ae_fp_eq(v1,(double)(0)) )
    {
        for(k=0; k<=n-1; k++)
        {
            state->xk.ptr.p_double[k] = (double)(0);
        }
        result = ae_false;
        return result;
    }
    
    /*
     * r(0) = b-A*x(0)
     * RK2 = r(0)'*r(0)
     */
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xk.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    ae_v_move(&state->rk.ptr.p_double[0], 1, &state->b.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_sub(&state->rk.ptr.p_double[0], 1, &state->ax.ptr.p_double[0], 1, ae_v_len(0,n-1));
    rk2 = ae_v_dotproduct(&state->rk.ptr.p_double[0], 1, &state->rk.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&state->pk.ptr.p_double[0], 1, &state->rk.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->e1 = ae_sqrt(rk2, _state);
    
    /*
     * Cycle
     */
    k = 0;
lbl_3:
    if( k>n-1 )
    {
        goto lbl_5;
    }
    
    /*
     * Calculate A*p(k) - store in State.Tmp2
     * and p(k)'*A*p(k)  - store in PAP
     *
     * If PAP=0, break (iteration is over)
     */
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->pk.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    ae_v_move(&state->tmp2.ptr.p_double[0], 1, &state->ax.ptr.p_double[0], 1, ae_v_len(0,n-1));
    pap = state->xax;
    if( !ae_isfinite(pap, _state) )
    {
        goto lbl_5;
    }
    if( ae_fp_less_eq(pap,(double)(0)) )
    {
        goto lbl_5;
    }
    
    /*
     * S = (r(k)'*r(k))/(p(k)'*A*p(k))
     */
    s = rk2/pap;
    
    /*
     * x(k+1) = x(k) + S*p(k)
     */
    ae_v_move(&state->xk1.ptr.p_double[0], 1, &state->xk.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_addd(&state->xk1.ptr.p_double[0], 1, &state->pk.ptr.p_double[0], 1, ae_v_len(0,n-1), s);
    
    /*
     * r(k+1) = r(k) - S*A*p(k)
     * RK12 = r(k+1)'*r(k+1)
     *
     * Break if r(k+1) small enough (when compared to r(k))
     */
    ae_v_move(&state->rk1.ptr.p_double[0], 1, &state->rk.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_subd(&state->rk1.ptr.p_double[0], 1, &state->tmp2.ptr.p_double[0], 1, ae_v_len(0,n-1), s);
    rk12 = ae_v_dotproduct(&state->rk1.ptr.p_double[0], 1, &state->rk1.ptr.p_double[0], 1, ae_v_len(0,n-1));
    if( ae_fp_less_eq(ae_sqrt(rk12, _state),100*ae_machineepsilon*state->e1) )
    {
        
        /*
         * X(k) = x(k+1) before exit -
         * - because we expect to find solution at x(k)
         */
        ae_v_move(&state->xk.ptr.p_double[0], 1, &state->xk1.ptr.p_double[0], 1, ae_v_len(0,n-1));
        goto lbl_5;
    }
    
    /*
     * BetaK = RK12/RK2
     * p(k+1) = r(k+1)+betak*p(k)
     *
     * NOTE: we expect that BetaK won't overflow because of
     * "Sqrt(RK12)<=100*MachineEpsilon*E1" test above.
     */
    betak = rk12/rk2;
    ae_v_move(&state->pk1.ptr.p_double[0], 1, &state->rk1.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_addd(&state->pk1.ptr.p_double[0], 1, &state->pk.ptr.p_double[0], 1, ae_v_len(0,n-1), betak);
    
    /*
     * r(k) := r(k+1)
     * x(k) := x(k+1)
     * p(k) := p(k+1)
     */
    ae_v_move(&state->rk.ptr.p_double[0], 1, &state->rk1.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&state->xk.ptr.p_double[0], 1, &state->xk1.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&state->pk.ptr.p_double[0], 1, &state->pk1.ptr.p_double[0], 1, ae_v_len(0,n-1));
    rk2 = rk12;
    k = k+1;
    goto lbl_3;
lbl_5:
    
    /*
     * Calculate E2
     */
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->xk.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    ae_v_move(&state->rk.ptr.p_double[0], 1, &state->b.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_sub(&state->rk.ptr.p_double[0], 1, &state->ax.ptr.p_double[0], 1, ae_v_len(0,n-1));
    v1 = ae_v_dotproduct(&state->rk.ptr.p_double[0], 1, &state->rk.ptr.p_double[0], 1, ae_v_len(0,n-1));
    state->e2 = ae_sqrt(v1, _state);
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstate.ia.ptr.p_int[0] = n;
    state->rstate.ia.ptr.p_int[1] = k;
    state->rstate.ra.ptr.p_double[0] = rk2;
    state->rstate.ra.ptr.p_double[1] = rk12;
    state->rstate.ra.ptr.p_double[2] = pap;
    state->rstate.ra.ptr.p_double[3] = s;
    state->rstate.ra.ptr.p_double[4] = betak;
    state->rstate.ra.ptr.p_double[5] = v1;
    state->rstate.ra.ptr.p_double[6] = v2;
    return result;
}


/*************************************************************************
Fast  least  squares  solver,  solves  well  conditioned  system   without
performing  any  checks  for  degeneracy,  and using user-provided buffers
(which are automatically reallocated if too small).

This  function  is  intended  for solution of moderately sized systems. It
uses factorization algorithms based on Level 2 BLAS  operations,  thus  it
won't work efficiently on large scale systems.

INPUT PARAMETERS:
    A       -   array[M,N], system matrix.
                Contents of A is destroyed during solution.
    B       -   array[M], right part
    M       -   number of equations
    N       -   number of variables, N<=M
    Tmp0, Tmp1, Tmp2-
                buffers; function automatically allocates them, if they are
                too  small. They can  be  reused  if  function  is   called 
                several times.
                
OUTPUT PARAMETERS:
    B       -   solution (first N components, next M-N are zero)

  -- ALGLIB --
     Copyright 20.01.2012 by Bochkanov Sergey
*************************************************************************/
void fblssolvels(/* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* b,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* tmp0,
     /* Real    */ ae_vector* tmp1,
     /* Real    */ ae_vector* tmp2,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    double v;


    ae_assert(n>0, "FBLSSolveLS: N<=0", _state);
    ae_assert(m>=n, "FBLSSolveLS: M<N", _state);
    ae_assert(a->rows>=m, "FBLSSolveLS: Rows(A)<M", _state);
    ae_assert(a->cols>=n, "FBLSSolveLS: Cols(A)<N", _state);
    ae_assert(b->cnt>=m, "FBLSSolveLS: Length(B)<M", _state);
    
    /*
     * Allocate temporaries
     */
    rvectorsetlengthatleast(tmp0, ae_maxint(m, n, _state)+1, _state);
    rvectorsetlengthatleast(tmp1, ae_maxint(m, n, _state)+1, _state);
    rvectorsetlengthatleast(tmp2, ae_minint(m, n, _state), _state);
    
    /*
     * Call basecase QR
     */
    rmatrixqrbasecase(a, m, n, tmp0, tmp1, tmp2, _state);
    
    /*
     * Multiply B by Q'
     */
    for(k=0; k<=n-1; k++)
    {
        for(i=0; i<=k-1; i++)
        {
            tmp0->ptr.p_double[i] = (double)(0);
        }
        ae_v_move(&tmp0->ptr.p_double[k], 1, &a->ptr.pp_double[k][k], a->stride, ae_v_len(k,m-1));
        tmp0->ptr.p_double[k] = (double)(1);
        v = ae_v_dotproduct(&tmp0->ptr.p_double[k], 1, &b->ptr.p_double[k], 1, ae_v_len(k,m-1));
        v = v*tmp2->ptr.p_double[k];
        ae_v_subd(&b->ptr.p_double[k], 1, &tmp0->ptr.p_double[k], 1, ae_v_len(k,m-1), v);
    }
    
    /*
     * Solve triangular system
     */
    b->ptr.p_double[n-1] = b->ptr.p_double[n-1]/a->ptr.pp_double[n-1][n-1];
    for(i=n-2; i>=0; i--)
    {
        v = ae_v_dotproduct(&a->ptr.pp_double[i][i+1], 1, &b->ptr.p_double[i+1], 1, ae_v_len(i+1,n-1));
        b->ptr.p_double[i] = (b->ptr.p_double[i]-v)/a->ptr.pp_double[i][i];
    }
    for(i=n; i<=m-1; i++)
    {
        b->ptr.p_double[i] = 0.0;
    }
}


void _fblslincgstate_init(void* _p, ae_state *_state)
{
    fblslincgstate *p = (fblslincgstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->ax, 0, DT_REAL, _state);
    ae_vector_init(&p->rk, 0, DT_REAL, _state);
    ae_vector_init(&p->rk1, 0, DT_REAL, _state);
    ae_vector_init(&p->xk, 0, DT_REAL, _state);
    ae_vector_init(&p->xk1, 0, DT_REAL, _state);
    ae_vector_init(&p->pk, 0, DT_REAL, _state);
    ae_vector_init(&p->pk1, 0, DT_REAL, _state);
    ae_vector_init(&p->b, 0, DT_REAL, _state);
    _rcommstate_init(&p->rstate, _state);
    ae_vector_init(&p->tmp2, 0, DT_REAL, _state);
}


void _fblslincgstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    fblslincgstate *dst = (fblslincgstate*)_dst;
    fblslincgstate *src = (fblslincgstate*)_src;
    dst->e1 = src->e1;
    dst->e2 = src->e2;
    ae_vector_init_copy(&dst->x, &src->x, _state);
    ae_vector_init_copy(&dst->ax, &src->ax, _state);
    dst->xax = src->xax;
    dst->n = src->n;
    ae_vector_init_copy(&dst->rk, &src->rk, _state);
    ae_vector_init_copy(&dst->rk1, &src->rk1, _state);
    ae_vector_init_copy(&dst->xk, &src->xk, _state);
    ae_vector_init_copy(&dst->xk1, &src->xk1, _state);
    ae_vector_init_copy(&dst->pk, &src->pk, _state);
    ae_vector_init_copy(&dst->pk1, &src->pk1, _state);
    ae_vector_init_copy(&dst->b, &src->b, _state);
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
    ae_vector_init_copy(&dst->tmp2, &src->tmp2, _state);
}


void _fblslincgstate_clear(void* _p)
{
    fblslincgstate *p = (fblslincgstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->ax);
    ae_vector_clear(&p->rk);
    ae_vector_clear(&p->rk1);
    ae_vector_clear(&p->xk);
    ae_vector_clear(&p->xk1);
    ae_vector_clear(&p->pk);
    ae_vector_clear(&p->pk1);
    ae_vector_clear(&p->b);
    _rcommstate_clear(&p->rstate);
    ae_vector_clear(&p->tmp2);
}


void _fblslincgstate_destroy(void* _p)
{
    fblslincgstate *p = (fblslincgstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->ax);
    ae_vector_destroy(&p->rk);
    ae_vector_destroy(&p->rk1);
    ae_vector_destroy(&p->xk);
    ae_vector_destroy(&p->xk1);
    ae_vector_destroy(&p->pk);
    ae_vector_destroy(&p->pk1);
    ae_vector_destroy(&p->b);
    _rcommstate_destroy(&p->rstate);
    ae_vector_destroy(&p->tmp2);
}


/*$ End $*/
