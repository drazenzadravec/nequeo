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
#include "safesolve.h"


/*$ Declarations $*/
static ae_bool safesolve_cbasicsolveandupdate(ae_complex alpha,
     ae_complex beta,
     double lnmax,
     double bnorm,
     double maxgrowth,
     double* xnorm,
     ae_complex* x,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Real implementation of CMatrixScaledTRSafeSolve

  -- ALGLIB routine --
     21.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool rmatrixscaledtrsafesolve(/* Real    */ ae_matrix* a,
     double sa,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     ae_bool isupper,
     ae_int_t trans,
     ae_bool isunit,
     double maxgrowth,
     ae_state *_state)
{
    ae_frame _frame_block;
    double lnmax;
    double nrmb;
    double nrmx;
    ae_int_t i;
    ae_complex alpha;
    ae_complex beta;
    double vr;
    ae_complex cx;
    ae_vector tmp;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    ae_assert(n>0, "RMatrixTRSafeSolve: incorrect N!", _state);
    ae_assert(trans==0||trans==1, "RMatrixTRSafeSolve: incorrect Trans!", _state);
    result = ae_true;
    lnmax = ae_log(ae_maxrealnumber, _state);
    
    /*
     * Quick return if possible
     */
    if( n<=0 )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Load norms: right part and X
     */
    nrmb = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        nrmb = ae_maxreal(nrmb, ae_fabs(x->ptr.p_double[i], _state), _state);
    }
    nrmx = (double)(0);
    
    /*
     * Solve
     */
    ae_vector_set_length(&tmp, n, _state);
    result = ae_true;
    if( isupper&&trans==0 )
    {
        
        /*
         * U*x = b
         */
        for(i=n-1; i>=0; i--)
        {
            
            /*
             * Task is reduced to alpha*x[i] = beta
             */
            if( isunit )
            {
                alpha = ae_complex_from_d(sa);
            }
            else
            {
                alpha = ae_complex_from_d(a->ptr.pp_double[i][i]*sa);
            }
            if( i<n-1 )
            {
                ae_v_moved(&tmp.ptr.p_double[i+1], 1, &a->ptr.pp_double[i][i+1], 1, ae_v_len(i+1,n-1), sa);
                vr = ae_v_dotproduct(&tmp.ptr.p_double[i+1], 1, &x->ptr.p_double[i+1], 1, ae_v_len(i+1,n-1));
                beta = ae_complex_from_d(x->ptr.p_double[i]-vr);
            }
            else
            {
                beta = ae_complex_from_d(x->ptr.p_double[i]);
            }
            
            /*
             * solve alpha*x[i] = beta
             */
            result = safesolve_cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, &nrmx, &cx, _state);
            if( !result )
            {
                ae_frame_leave(_state);
                return result;
            }
            x->ptr.p_double[i] = cx.x;
        }
        ae_frame_leave(_state);
        return result;
    }
    if( !isupper&&trans==0 )
    {
        
        /*
         * L*x = b
         */
        for(i=0; i<=n-1; i++)
        {
            
            /*
             * Task is reduced to alpha*x[i] = beta
             */
            if( isunit )
            {
                alpha = ae_complex_from_d(sa);
            }
            else
            {
                alpha = ae_complex_from_d(a->ptr.pp_double[i][i]*sa);
            }
            if( i>0 )
            {
                ae_v_moved(&tmp.ptr.p_double[0], 1, &a->ptr.pp_double[i][0], 1, ae_v_len(0,i-1), sa);
                vr = ae_v_dotproduct(&tmp.ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,i-1));
                beta = ae_complex_from_d(x->ptr.p_double[i]-vr);
            }
            else
            {
                beta = ae_complex_from_d(x->ptr.p_double[i]);
            }
            
            /*
             * solve alpha*x[i] = beta
             */
            result = safesolve_cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, &nrmx, &cx, _state);
            if( !result )
            {
                ae_frame_leave(_state);
                return result;
            }
            x->ptr.p_double[i] = cx.x;
        }
        ae_frame_leave(_state);
        return result;
    }
    if( isupper&&trans==1 )
    {
        
        /*
         * U^T*x = b
         */
        for(i=0; i<=n-1; i++)
        {
            
            /*
             * Task is reduced to alpha*x[i] = beta
             */
            if( isunit )
            {
                alpha = ae_complex_from_d(sa);
            }
            else
            {
                alpha = ae_complex_from_d(a->ptr.pp_double[i][i]*sa);
            }
            beta = ae_complex_from_d(x->ptr.p_double[i]);
            
            /*
             * solve alpha*x[i] = beta
             */
            result = safesolve_cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, &nrmx, &cx, _state);
            if( !result )
            {
                ae_frame_leave(_state);
                return result;
            }
            x->ptr.p_double[i] = cx.x;
            
            /*
             * update the rest of right part
             */
            if( i<n-1 )
            {
                vr = cx.x;
                ae_v_moved(&tmp.ptr.p_double[i+1], 1, &a->ptr.pp_double[i][i+1], 1, ae_v_len(i+1,n-1), sa);
                ae_v_subd(&x->ptr.p_double[i+1], 1, &tmp.ptr.p_double[i+1], 1, ae_v_len(i+1,n-1), vr);
            }
        }
        ae_frame_leave(_state);
        return result;
    }
    if( !isupper&&trans==1 )
    {
        
        /*
         * L^T*x = b
         */
        for(i=n-1; i>=0; i--)
        {
            
            /*
             * Task is reduced to alpha*x[i] = beta
             */
            if( isunit )
            {
                alpha = ae_complex_from_d(sa);
            }
            else
            {
                alpha = ae_complex_from_d(a->ptr.pp_double[i][i]*sa);
            }
            beta = ae_complex_from_d(x->ptr.p_double[i]);
            
            /*
             * solve alpha*x[i] = beta
             */
            result = safesolve_cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, &nrmx, &cx, _state);
            if( !result )
            {
                ae_frame_leave(_state);
                return result;
            }
            x->ptr.p_double[i] = cx.x;
            
            /*
             * update the rest of right part
             */
            if( i>0 )
            {
                vr = cx.x;
                ae_v_moved(&tmp.ptr.p_double[0], 1, &a->ptr.pp_double[i][0], 1, ae_v_len(0,i-1), sa);
                ae_v_subd(&x->ptr.p_double[0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,i-1), vr);
            }
        }
        ae_frame_leave(_state);
        return result;
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Internal subroutine for safe solution of

    SA*op(A)=b
    
where  A  is  NxN  upper/lower  triangular/unitriangular  matrix, op(A) is
either identity transform, transposition or Hermitian transposition, SA is
a scaling factor such that max(|SA*A[i,j]|) is close to 1.0 in magnutude.

This subroutine  limits  relative  growth  of  solution  (in inf-norm)  by
MaxGrowth,  returning  False  if  growth  exceeds MaxGrowth. Degenerate or
near-degenerate matrices are handled correctly (False is returned) as long
as MaxGrowth is significantly less than MaxRealNumber/norm(b).

  -- ALGLIB routine --
     21.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool cmatrixscaledtrsafesolve(/* Complex */ ae_matrix* a,
     double sa,
     ae_int_t n,
     /* Complex */ ae_vector* x,
     ae_bool isupper,
     ae_int_t trans,
     ae_bool isunit,
     double maxgrowth,
     ae_state *_state)
{
    ae_frame _frame_block;
    double lnmax;
    double nrmb;
    double nrmx;
    ae_int_t i;
    ae_complex alpha;
    ae_complex beta;
    ae_complex vc;
    ae_vector tmp;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&tmp, 0, DT_COMPLEX, _state);

    ae_assert(n>0, "CMatrixTRSafeSolve: incorrect N!", _state);
    ae_assert((trans==0||trans==1)||trans==2, "CMatrixTRSafeSolve: incorrect Trans!", _state);
    result = ae_true;
    lnmax = ae_log(ae_maxrealnumber, _state);
    
    /*
     * Quick return if possible
     */
    if( n<=0 )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Load norms: right part and X
     */
    nrmb = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        nrmb = ae_maxreal(nrmb, ae_c_abs(x->ptr.p_complex[i], _state), _state);
    }
    nrmx = (double)(0);
    
    /*
     * Solve
     */
    ae_vector_set_length(&tmp, n, _state);
    result = ae_true;
    if( isupper&&trans==0 )
    {
        
        /*
         * U*x = b
         */
        for(i=n-1; i>=0; i--)
        {
            
            /*
             * Task is reduced to alpha*x[i] = beta
             */
            if( isunit )
            {
                alpha = ae_complex_from_d(sa);
            }
            else
            {
                alpha = ae_c_mul_d(a->ptr.pp_complex[i][i],sa);
            }
            if( i<n-1 )
            {
                ae_v_cmoved(&tmp.ptr.p_complex[i+1], 1, &a->ptr.pp_complex[i][i+1], 1, "N", ae_v_len(i+1,n-1), sa);
                vc = ae_v_cdotproduct(&tmp.ptr.p_complex[i+1], 1, "N", &x->ptr.p_complex[i+1], 1, "N", ae_v_len(i+1,n-1));
                beta = ae_c_sub(x->ptr.p_complex[i],vc);
            }
            else
            {
                beta = x->ptr.p_complex[i];
            }
            
            /*
             * solve alpha*x[i] = beta
             */
            result = safesolve_cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, &nrmx, &vc, _state);
            if( !result )
            {
                ae_frame_leave(_state);
                return result;
            }
            x->ptr.p_complex[i] = vc;
        }
        ae_frame_leave(_state);
        return result;
    }
    if( !isupper&&trans==0 )
    {
        
        /*
         * L*x = b
         */
        for(i=0; i<=n-1; i++)
        {
            
            /*
             * Task is reduced to alpha*x[i] = beta
             */
            if( isunit )
            {
                alpha = ae_complex_from_d(sa);
            }
            else
            {
                alpha = ae_c_mul_d(a->ptr.pp_complex[i][i],sa);
            }
            if( i>0 )
            {
                ae_v_cmoved(&tmp.ptr.p_complex[0], 1, &a->ptr.pp_complex[i][0], 1, "N", ae_v_len(0,i-1), sa);
                vc = ae_v_cdotproduct(&tmp.ptr.p_complex[0], 1, "N", &x->ptr.p_complex[0], 1, "N", ae_v_len(0,i-1));
                beta = ae_c_sub(x->ptr.p_complex[i],vc);
            }
            else
            {
                beta = x->ptr.p_complex[i];
            }
            
            /*
             * solve alpha*x[i] = beta
             */
            result = safesolve_cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, &nrmx, &vc, _state);
            if( !result )
            {
                ae_frame_leave(_state);
                return result;
            }
            x->ptr.p_complex[i] = vc;
        }
        ae_frame_leave(_state);
        return result;
    }
    if( isupper&&trans==1 )
    {
        
        /*
         * U^T*x = b
         */
        for(i=0; i<=n-1; i++)
        {
            
            /*
             * Task is reduced to alpha*x[i] = beta
             */
            if( isunit )
            {
                alpha = ae_complex_from_d(sa);
            }
            else
            {
                alpha = ae_c_mul_d(a->ptr.pp_complex[i][i],sa);
            }
            beta = x->ptr.p_complex[i];
            
            /*
             * solve alpha*x[i] = beta
             */
            result = safesolve_cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, &nrmx, &vc, _state);
            if( !result )
            {
                ae_frame_leave(_state);
                return result;
            }
            x->ptr.p_complex[i] = vc;
            
            /*
             * update the rest of right part
             */
            if( i<n-1 )
            {
                ae_v_cmoved(&tmp.ptr.p_complex[i+1], 1, &a->ptr.pp_complex[i][i+1], 1, "N", ae_v_len(i+1,n-1), sa);
                ae_v_csubc(&x->ptr.p_complex[i+1], 1, &tmp.ptr.p_complex[i+1], 1, "N", ae_v_len(i+1,n-1), vc);
            }
        }
        ae_frame_leave(_state);
        return result;
    }
    if( !isupper&&trans==1 )
    {
        
        /*
         * L^T*x = b
         */
        for(i=n-1; i>=0; i--)
        {
            
            /*
             * Task is reduced to alpha*x[i] = beta
             */
            if( isunit )
            {
                alpha = ae_complex_from_d(sa);
            }
            else
            {
                alpha = ae_c_mul_d(a->ptr.pp_complex[i][i],sa);
            }
            beta = x->ptr.p_complex[i];
            
            /*
             * solve alpha*x[i] = beta
             */
            result = safesolve_cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, &nrmx, &vc, _state);
            if( !result )
            {
                ae_frame_leave(_state);
                return result;
            }
            x->ptr.p_complex[i] = vc;
            
            /*
             * update the rest of right part
             */
            if( i>0 )
            {
                ae_v_cmoved(&tmp.ptr.p_complex[0], 1, &a->ptr.pp_complex[i][0], 1, "N", ae_v_len(0,i-1), sa);
                ae_v_csubc(&x->ptr.p_complex[0], 1, &tmp.ptr.p_complex[0], 1, "N", ae_v_len(0,i-1), vc);
            }
        }
        ae_frame_leave(_state);
        return result;
    }
    if( isupper&&trans==2 )
    {
        
        /*
         * U^H*x = b
         */
        for(i=0; i<=n-1; i++)
        {
            
            /*
             * Task is reduced to alpha*x[i] = beta
             */
            if( isunit )
            {
                alpha = ae_complex_from_d(sa);
            }
            else
            {
                alpha = ae_c_mul_d(ae_c_conj(a->ptr.pp_complex[i][i], _state),sa);
            }
            beta = x->ptr.p_complex[i];
            
            /*
             * solve alpha*x[i] = beta
             */
            result = safesolve_cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, &nrmx, &vc, _state);
            if( !result )
            {
                ae_frame_leave(_state);
                return result;
            }
            x->ptr.p_complex[i] = vc;
            
            /*
             * update the rest of right part
             */
            if( i<n-1 )
            {
                ae_v_cmoved(&tmp.ptr.p_complex[i+1], 1, &a->ptr.pp_complex[i][i+1], 1, "Conj", ae_v_len(i+1,n-1), sa);
                ae_v_csubc(&x->ptr.p_complex[i+1], 1, &tmp.ptr.p_complex[i+1], 1, "N", ae_v_len(i+1,n-1), vc);
            }
        }
        ae_frame_leave(_state);
        return result;
    }
    if( !isupper&&trans==2 )
    {
        
        /*
         * L^T*x = b
         */
        for(i=n-1; i>=0; i--)
        {
            
            /*
             * Task is reduced to alpha*x[i] = beta
             */
            if( isunit )
            {
                alpha = ae_complex_from_d(sa);
            }
            else
            {
                alpha = ae_c_mul_d(ae_c_conj(a->ptr.pp_complex[i][i], _state),sa);
            }
            beta = x->ptr.p_complex[i];
            
            /*
             * solve alpha*x[i] = beta
             */
            result = safesolve_cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, &nrmx, &vc, _state);
            if( !result )
            {
                ae_frame_leave(_state);
                return result;
            }
            x->ptr.p_complex[i] = vc;
            
            /*
             * update the rest of right part
             */
            if( i>0 )
            {
                ae_v_cmoved(&tmp.ptr.p_complex[0], 1, &a->ptr.pp_complex[i][0], 1, "Conj", ae_v_len(0,i-1), sa);
                ae_v_csubc(&x->ptr.p_complex[0], 1, &tmp.ptr.p_complex[0], 1, "N", ae_v_len(0,i-1), vc);
            }
        }
        ae_frame_leave(_state);
        return result;
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
complex basic solver-updater for reduced linear system

    alpha*x[i] = beta

solves this equation and updates it in overlfow-safe manner (keeping track
of relative growth of solution).

Parameters:
    Alpha   -   alpha
    Beta    -   beta
    LnMax   -   precomputed Ln(MaxRealNumber)
    BNorm   -   inf-norm of b (right part of original system)
    MaxGrowth-  maximum growth of norm(x) relative to norm(b)
    XNorm   -   inf-norm of other components of X (which are already processed)
                it is updated by CBasicSolveAndUpdate.
    X       -   solution

  -- ALGLIB routine --
     26.01.2009
     Bochkanov Sergey
*************************************************************************/
static ae_bool safesolve_cbasicsolveandupdate(ae_complex alpha,
     ae_complex beta,
     double lnmax,
     double bnorm,
     double maxgrowth,
     double* xnorm,
     ae_complex* x,
     ae_state *_state)
{
    double v;
    ae_bool result;

    x->x = 0;
    x->y = 0;

    result = ae_false;
    if( ae_c_eq_d(alpha,(double)(0)) )
    {
        return result;
    }
    if( ae_c_neq_d(beta,(double)(0)) )
    {
        
        /*
         * alpha*x[i]=beta
         */
        v = ae_log(ae_c_abs(beta, _state), _state)-ae_log(ae_c_abs(alpha, _state), _state);
        if( ae_fp_greater(v,lnmax) )
        {
            return result;
        }
        *x = ae_c_div(beta,alpha);
    }
    else
    {
        
        /*
         * alpha*x[i]=0
         */
        *x = ae_complex_from_i(0);
    }
    
    /*
     * update NrmX, test growth limit
     */
    *xnorm = ae_maxreal(*xnorm, ae_c_abs(*x, _state), _state);
    if( ae_fp_greater(*xnorm,maxgrowth*bnorm) )
    {
        return result;
    }
    result = ae_true;
    return result;
}


/*$ End $*/
