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
#include "ablasf.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool cmatrixrank1f(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Complex */ ae_vector* u,
     ae_int_t iu,
     /* Complex */ ae_vector* v,
     ae_int_t iv,
     ae_state *_state)
{
#ifndef ALGLIB_INTERCEPTS_ABLAS
    ae_bool result;


    result = ae_false;
    return result;
#else
    return _ialglib_i_cmatrixrank1f(m, n, a, ia, ja, u, iu, v, iv);
#endif
}


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool rmatrixrank1f(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Real    */ ae_vector* u,
     ae_int_t iu,
     /* Real    */ ae_vector* v,
     ae_int_t iv,
     ae_state *_state)
{
#ifndef ALGLIB_INTERCEPTS_ABLAS
    ae_bool result;


    result = ae_false;
    return result;
#else
    return _ialglib_i_rmatrixrank1f(m, n, a, ia, ja, u, iu, v, iv);
#endif
}


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool cmatrixmvf(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t opa,
     /* Complex */ ae_vector* x,
     ae_int_t ix,
     /* Complex */ ae_vector* y,
     ae_int_t iy,
     ae_state *_state)
{
    ae_bool result;


    result = ae_false;
    return result;
}


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool rmatrixmvf(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t opa,
     /* Real    */ ae_vector* x,
     ae_int_t ix,
     /* Real    */ ae_vector* y,
     ae_int_t iy,
     ae_state *_state)
{
    ae_bool result;


    result = ae_false;
    return result;
}


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool cmatrixrighttrsmf(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
#ifndef ALGLIB_INTERCEPTS_ABLAS
    ae_bool result;


    result = ae_false;
    return result;
#else
    return _ialglib_i_cmatrixrighttrsmf(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2);
#endif
}


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool cmatrixlefttrsmf(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
#ifndef ALGLIB_INTERCEPTS_ABLAS
    ae_bool result;


    result = ae_false;
    return result;
#else
    return _ialglib_i_cmatrixlefttrsmf(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2);
#endif
}


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool rmatrixrighttrsmf(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
#ifndef ALGLIB_INTERCEPTS_ABLAS
    ae_bool result;


    result = ae_false;
    return result;
#else
    return _ialglib_i_rmatrixrighttrsmf(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2);
#endif
}


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool rmatrixlefttrsmf(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
#ifndef ALGLIB_INTERCEPTS_ABLAS
    ae_bool result;


    result = ae_false;
    return result;
#else
    return _ialglib_i_rmatrixlefttrsmf(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2);
#endif
}


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool cmatrixherkf(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state)
{
#ifndef ALGLIB_INTERCEPTS_ABLAS
    ae_bool result;


    result = ae_false;
    return result;
#else
    return _ialglib_i_cmatrixherkf(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper);
#endif
}


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool rmatrixsyrkf(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state)
{
#ifndef ALGLIB_INTERCEPTS_ABLAS
    ae_bool result;


    result = ae_false;
    return result;
#else
    return _ialglib_i_rmatrixsyrkf(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper);
#endif
}


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool rmatrixgemmf(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     /* Real    */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
#ifndef ALGLIB_INTERCEPTS_ABLAS
    ae_bool result;


    result = ae_false;
    return result;
#else
    return _ialglib_i_rmatrixgemmf(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
#endif
}


/*************************************************************************
Fast kernel

  -- ALGLIB routine --
     19.01.2010
     Bochkanov Sergey
*************************************************************************/
ae_bool cmatrixgemmf(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     ae_complex alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     /* Complex */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     ae_complex beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
#ifndef ALGLIB_INTERCEPTS_ABLAS
    ae_bool result;


    result = ae_false;
    return result;
#else
    return _ialglib_i_cmatrixgemmf(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
#endif
}


/*************************************************************************
CMatrixGEMM kernel, basecase code for CMatrixGEMM.

This subroutine calculates C = alpha*op1(A)*op2(B) +beta*C where:
* C is MxN general matrix
* op1(A) is MxK matrix
* op2(B) is KxN matrix
* "op" may be identity transformation, transposition, conjugate transposition

Additional info:
* multiplication result replaces C. If Beta=0, C elements are not used in
  calculations (not multiplied by zero - just not referenced)
* if Alpha=0, A is not used (not multiplied by zero - just not referenced)
* if both Beta and Alpha are zero, C is filled by zeros.

IMPORTANT:

This function does NOT preallocate output matrix C, it MUST be preallocated
by caller prior to calling this function. In case C does not have  enough
space to store result, exception will be generated.

INPUT PARAMETERS
    M       -   matrix size, M>0
    N       -   matrix size, N>0
    K       -   matrix size, K>0
    Alpha   -   coefficient
    A       -   matrix
    IA      -   submatrix offset
    JA      -   submatrix offset
    OpTypeA -   transformation type:
                * 0 - no transformation
                * 1 - transposition
                * 2 - conjugate transposition
    B       -   matrix
    IB      -   submatrix offset
    JB      -   submatrix offset
    OpTypeB -   transformation type:
                * 0 - no transformation
                * 1 - transposition
                * 2 - conjugate transposition
    Beta    -   coefficient
    C       -   PREALLOCATED output matrix
    IC      -   submatrix offset
    JC      -   submatrix offset

  -- ALGLIB routine --
     27.03.2013
     Bochkanov Sergey
*************************************************************************/
void cmatrixgemmk(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     ae_complex alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     /* Complex */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     ae_complex beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_complex v;
    ae_complex v00;
    ae_complex v01;
    ae_complex v10;
    ae_complex v11;
    double v00x;
    double v00y;
    double v01x;
    double v01y;
    double v10x;
    double v10y;
    double v11x;
    double v11y;
    double a0x;
    double a0y;
    double a1x;
    double a1y;
    double b0x;
    double b0y;
    double b1x;
    double b1y;
    ae_int_t idxa0;
    ae_int_t idxa1;
    ae_int_t idxb0;
    ae_int_t idxb1;
    ae_int_t i0;
    ae_int_t i1;
    ae_int_t ik;
    ae_int_t j0;
    ae_int_t j1;
    ae_int_t jk;
    ae_int_t t;
    ae_int_t offsa;
    ae_int_t offsb;


    
    /*
     * if matrix size is zero
     */
    if( m==0||n==0 )
    {
        return;
    }
    
    /*
     * Try optimized code
     */
    if( cmatrixgemmf(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state) )
    {
        return;
    }
    
    /*
     * if K=0, then C=Beta*C
     */
    if( k==0 )
    {
        if( ae_c_neq_d(beta,(double)(1)) )
        {
            if( ae_c_neq_d(beta,(double)(0)) )
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        c->ptr.pp_complex[ic+i][jc+j] = ae_c_mul(beta,c->ptr.pp_complex[ic+i][jc+j]);
                    }
                }
            }
            else
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        c->ptr.pp_complex[ic+i][jc+j] = ae_complex_from_i(0);
                    }
                }
            }
        }
        return;
    }
    
    /*
     * This phase is not really necessary, but compiler complains
     * about "possibly uninitialized variables"
     */
    a0x = (double)(0);
    a0y = (double)(0);
    a1x = (double)(0);
    a1y = (double)(0);
    b0x = (double)(0);
    b0y = (double)(0);
    b1x = (double)(0);
    b1y = (double)(0);
    
    /*
     * General case
     */
    i = 0;
    while(i<m)
    {
        j = 0;
        while(j<n)
        {
            
            /*
             * Choose between specialized 4x4 code and general code
             */
            if( i+2<=m&&j+2<=n )
            {
                
                /*
                 * Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                 *
                 * This submatrix is calculated as sum of K rank-1 products,
                 * with operands cached in local variables in order to speed
                 * up operations with arrays.
                 */
                v00x = 0.0;
                v00y = 0.0;
                v01x = 0.0;
                v01y = 0.0;
                v10x = 0.0;
                v10y = 0.0;
                v11x = 0.0;
                v11y = 0.0;
                if( optypea==0 )
                {
                    idxa0 = ia+i+0;
                    idxa1 = ia+i+1;
                    offsa = ja;
                }
                else
                {
                    idxa0 = ja+i+0;
                    idxa1 = ja+i+1;
                    offsa = ia;
                }
                if( optypeb==0 )
                {
                    idxb0 = jb+j+0;
                    idxb1 = jb+j+1;
                    offsb = ib;
                }
                else
                {
                    idxb0 = ib+j+0;
                    idxb1 = ib+j+1;
                    offsb = jb;
                }
                for(t=0; t<=k-1; t++)
                {
                    if( optypea==0 )
                    {
                        a0x = a->ptr.pp_complex[idxa0][offsa].x;
                        a0y = a->ptr.pp_complex[idxa0][offsa].y;
                        a1x = a->ptr.pp_complex[idxa1][offsa].x;
                        a1y = a->ptr.pp_complex[idxa1][offsa].y;
                    }
                    if( optypea==1 )
                    {
                        a0x = a->ptr.pp_complex[offsa][idxa0].x;
                        a0y = a->ptr.pp_complex[offsa][idxa0].y;
                        a1x = a->ptr.pp_complex[offsa][idxa1].x;
                        a1y = a->ptr.pp_complex[offsa][idxa1].y;
                    }
                    if( optypea==2 )
                    {
                        a0x = a->ptr.pp_complex[offsa][idxa0].x;
                        a0y = -a->ptr.pp_complex[offsa][idxa0].y;
                        a1x = a->ptr.pp_complex[offsa][idxa1].x;
                        a1y = -a->ptr.pp_complex[offsa][idxa1].y;
                    }
                    if( optypeb==0 )
                    {
                        b0x = b->ptr.pp_complex[offsb][idxb0].x;
                        b0y = b->ptr.pp_complex[offsb][idxb0].y;
                        b1x = b->ptr.pp_complex[offsb][idxb1].x;
                        b1y = b->ptr.pp_complex[offsb][idxb1].y;
                    }
                    if( optypeb==1 )
                    {
                        b0x = b->ptr.pp_complex[idxb0][offsb].x;
                        b0y = b->ptr.pp_complex[idxb0][offsb].y;
                        b1x = b->ptr.pp_complex[idxb1][offsb].x;
                        b1y = b->ptr.pp_complex[idxb1][offsb].y;
                    }
                    if( optypeb==2 )
                    {
                        b0x = b->ptr.pp_complex[idxb0][offsb].x;
                        b0y = -b->ptr.pp_complex[idxb0][offsb].y;
                        b1x = b->ptr.pp_complex[idxb1][offsb].x;
                        b1y = -b->ptr.pp_complex[idxb1][offsb].y;
                    }
                    v00x = v00x+a0x*b0x-a0y*b0y;
                    v00y = v00y+a0x*b0y+a0y*b0x;
                    v01x = v01x+a0x*b1x-a0y*b1y;
                    v01y = v01y+a0x*b1y+a0y*b1x;
                    v10x = v10x+a1x*b0x-a1y*b0y;
                    v10y = v10y+a1x*b0y+a1y*b0x;
                    v11x = v11x+a1x*b1x-a1y*b1y;
                    v11y = v11y+a1x*b1y+a1y*b1x;
                    offsa = offsa+1;
                    offsb = offsb+1;
                }
                v00.x = v00x;
                v00.y = v00y;
                v10.x = v10x;
                v10.y = v10y;
                v01.x = v01x;
                v01.y = v01y;
                v11.x = v11x;
                v11.y = v11y;
                if( ae_c_eq_d(beta,(double)(0)) )
                {
                    c->ptr.pp_complex[ic+i+0][jc+j+0] = ae_c_mul(alpha,v00);
                    c->ptr.pp_complex[ic+i+0][jc+j+1] = ae_c_mul(alpha,v01);
                    c->ptr.pp_complex[ic+i+1][jc+j+0] = ae_c_mul(alpha,v10);
                    c->ptr.pp_complex[ic+i+1][jc+j+1] = ae_c_mul(alpha,v11);
                }
                else
                {
                    c->ptr.pp_complex[ic+i+0][jc+j+0] = ae_c_add(ae_c_mul(beta,c->ptr.pp_complex[ic+i+0][jc+j+0]),ae_c_mul(alpha,v00));
                    c->ptr.pp_complex[ic+i+0][jc+j+1] = ae_c_add(ae_c_mul(beta,c->ptr.pp_complex[ic+i+0][jc+j+1]),ae_c_mul(alpha,v01));
                    c->ptr.pp_complex[ic+i+1][jc+j+0] = ae_c_add(ae_c_mul(beta,c->ptr.pp_complex[ic+i+1][jc+j+0]),ae_c_mul(alpha,v10));
                    c->ptr.pp_complex[ic+i+1][jc+j+1] = ae_c_add(ae_c_mul(beta,c->ptr.pp_complex[ic+i+1][jc+j+1]),ae_c_mul(alpha,v11));
                }
            }
            else
            {
                
                /*
                 * Determine submatrix [I0..I1]x[J0..J1] to process
                 */
                i0 = i;
                i1 = ae_minint(i+1, m-1, _state);
                j0 = j;
                j1 = ae_minint(j+1, n-1, _state);
                
                /*
                 * Process submatrix
                 */
                for(ik=i0; ik<=i1; ik++)
                {
                    for(jk=j0; jk<=j1; jk++)
                    {
                        if( k==0||ae_c_eq_d(alpha,(double)(0)) )
                        {
                            v = ae_complex_from_i(0);
                        }
                        else
                        {
                            v = ae_complex_from_d(0.0);
                            if( optypea==0&&optypeb==0 )
                            {
                                v = ae_v_cdotproduct(&a->ptr.pp_complex[ia+ik][ja], 1, "N", &b->ptr.pp_complex[ib][jb+jk], b->stride, "N", ae_v_len(ja,ja+k-1));
                            }
                            if( optypea==0&&optypeb==1 )
                            {
                                v = ae_v_cdotproduct(&a->ptr.pp_complex[ia+ik][ja], 1, "N", &b->ptr.pp_complex[ib+jk][jb], 1, "N", ae_v_len(ja,ja+k-1));
                            }
                            if( optypea==0&&optypeb==2 )
                            {
                                v = ae_v_cdotproduct(&a->ptr.pp_complex[ia+ik][ja], 1, "N", &b->ptr.pp_complex[ib+jk][jb], 1, "Conj", ae_v_len(ja,ja+k-1));
                            }
                            if( optypea==1&&optypeb==0 )
                            {
                                v = ae_v_cdotproduct(&a->ptr.pp_complex[ia][ja+ik], a->stride, "N", &b->ptr.pp_complex[ib][jb+jk], b->stride, "N", ae_v_len(ia,ia+k-1));
                            }
                            if( optypea==1&&optypeb==1 )
                            {
                                v = ae_v_cdotproduct(&a->ptr.pp_complex[ia][ja+ik], a->stride, "N", &b->ptr.pp_complex[ib+jk][jb], 1, "N", ae_v_len(ia,ia+k-1));
                            }
                            if( optypea==1&&optypeb==2 )
                            {
                                v = ae_v_cdotproduct(&a->ptr.pp_complex[ia][ja+ik], a->stride, "N", &b->ptr.pp_complex[ib+jk][jb], 1, "Conj", ae_v_len(ia,ia+k-1));
                            }
                            if( optypea==2&&optypeb==0 )
                            {
                                v = ae_v_cdotproduct(&a->ptr.pp_complex[ia][ja+ik], a->stride, "Conj", &b->ptr.pp_complex[ib][jb+jk], b->stride, "N", ae_v_len(ia,ia+k-1));
                            }
                            if( optypea==2&&optypeb==1 )
                            {
                                v = ae_v_cdotproduct(&a->ptr.pp_complex[ia][ja+ik], a->stride, "Conj", &b->ptr.pp_complex[ib+jk][jb], 1, "N", ae_v_len(ia,ia+k-1));
                            }
                            if( optypea==2&&optypeb==2 )
                            {
                                v = ae_v_cdotproduct(&a->ptr.pp_complex[ia][ja+ik], a->stride, "Conj", &b->ptr.pp_complex[ib+jk][jb], 1, "Conj", ae_v_len(ia,ia+k-1));
                            }
                        }
                        if( ae_c_eq_d(beta,(double)(0)) )
                        {
                            c->ptr.pp_complex[ic+ik][jc+jk] = ae_c_mul(alpha,v);
                        }
                        else
                        {
                            c->ptr.pp_complex[ic+ik][jc+jk] = ae_c_add(ae_c_mul(beta,c->ptr.pp_complex[ic+ik][jc+jk]),ae_c_mul(alpha,v));
                        }
                    }
                }
            }
            j = j+2;
        }
        i = i+2;
    }
}


/*************************************************************************
RMatrixGEMM kernel, basecase code for RMatrixGEMM.

This subroutine calculates C = alpha*op1(A)*op2(B) +beta*C where:
* C is MxN general matrix
* op1(A) is MxK matrix
* op2(B) is KxN matrix
* "op" may be identity transformation, transposition

Additional info:
* multiplication result replaces C. If Beta=0, C elements are not used in
  calculations (not multiplied by zero - just not referenced)
* if Alpha=0, A is not used (not multiplied by zero - just not referenced)
* if both Beta and Alpha are zero, C is filled by zeros.

IMPORTANT:

This function does NOT preallocate output matrix C, it MUST be preallocated
by caller prior to calling this function. In case C does not have  enough
space to store result, exception will be generated.

INPUT PARAMETERS
    M       -   matrix size, M>0
    N       -   matrix size, N>0
    K       -   matrix size, K>0
    Alpha   -   coefficient
    A       -   matrix
    IA      -   submatrix offset
    JA      -   submatrix offset
    OpTypeA -   transformation type:
                * 0 - no transformation
                * 1 - transposition
    B       -   matrix
    IB      -   submatrix offset
    JB      -   submatrix offset
    OpTypeB -   transformation type:
                * 0 - no transformation
                * 1 - transposition
    Beta    -   coefficient
    C       -   PREALLOCATED output matrix
    IC      -   submatrix offset
    JC      -   submatrix offset

  -- ALGLIB routine --
     27.03.2013
     Bochkanov Sergey
*************************************************************************/
void rmatrixgemmk(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     /* Real    */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    
    /*
     * if matrix size is zero
     */
    if( m==0||n==0 )
    {
        return;
    }
    
    /*
     * Try optimized code
     */
    if( rmatrixgemmf(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state) )
    {
        return;
    }
    
    /*
     * if K=0, then C=Beta*C
     */
    if( k==0||ae_fp_eq(alpha,(double)(0)) )
    {
        if( ae_fp_neq(beta,(double)(1)) )
        {
            if( ae_fp_neq(beta,(double)(0)) )
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        c->ptr.pp_double[ic+i][jc+j] = beta*c->ptr.pp_double[ic+i][jc+j];
                    }
                }
            }
            else
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        c->ptr.pp_double[ic+i][jc+j] = (double)(0);
                    }
                }
            }
        }
        return;
    }
    
    /*
     * Call specialized code.
     *
     * NOTE: specialized code was moved to separate function because of strange
     *       issues with instructions cache on some systems; Having too long
     *       functions significantly slows down internal loop of the algorithm.
     */
    if( optypea==0&&optypeb==0 )
    {
        rmatrixgemmk44v00(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc, _state);
    }
    if( optypea==0&&optypeb!=0 )
    {
        rmatrixgemmk44v01(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc, _state);
    }
    if( optypea!=0&&optypeb==0 )
    {
        rmatrixgemmk44v10(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc, _state);
    }
    if( optypea!=0&&optypeb!=0 )
    {
        rmatrixgemmk44v11(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc, _state);
    }
}


/*************************************************************************
RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
with OpTypeA=0 and OpTypeB=0.

Additional info:
* this function requires that Alpha<>0 (assertion is thrown otherwise)

INPUT PARAMETERS
    M       -   matrix size, M>0
    N       -   matrix size, N>0
    K       -   matrix size, K>0
    Alpha   -   coefficient
    A       -   matrix
    IA      -   submatrix offset
    JA      -   submatrix offset
    B       -   matrix
    IB      -   submatrix offset
    JB      -   submatrix offset
    Beta    -   coefficient
    C       -   PREALLOCATED output matrix
    IC      -   submatrix offset
    JC      -   submatrix offset

  -- ALGLIB routine --
     27.03.2013
     Bochkanov Sergey
*************************************************************************/
void rmatrixgemmk44v00(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Real    */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;
    double v00;
    double v01;
    double v02;
    double v03;
    double v10;
    double v11;
    double v12;
    double v13;
    double v20;
    double v21;
    double v22;
    double v23;
    double v30;
    double v31;
    double v32;
    double v33;
    double a0;
    double a1;
    double a2;
    double a3;
    double b0;
    double b1;
    double b2;
    double b3;
    ae_int_t idxa0;
    ae_int_t idxa1;
    ae_int_t idxa2;
    ae_int_t idxa3;
    ae_int_t idxb0;
    ae_int_t idxb1;
    ae_int_t idxb2;
    ae_int_t idxb3;
    ae_int_t i0;
    ae_int_t i1;
    ae_int_t ik;
    ae_int_t j0;
    ae_int_t j1;
    ae_int_t jk;
    ae_int_t t;
    ae_int_t offsa;
    ae_int_t offsb;


    ae_assert(ae_fp_neq(alpha,(double)(0)), "RMatrixGEMMK44V00: internal error (Alpha=0)", _state);
    
    /*
     * if matrix size is zero
     */
    if( m==0||n==0 )
    {
        return;
    }
    
    /*
     * A*B
     */
    i = 0;
    while(i<m)
    {
        j = 0;
        while(j<n)
        {
            
            /*
             * Choose between specialized 4x4 code and general code
             */
            if( i+4<=m&&j+4<=n )
            {
                
                /*
                 * Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                 *
                 * This submatrix is calculated as sum of K rank-1 products,
                 * with operands cached in local variables in order to speed
                 * up operations with arrays.
                 */
                idxa0 = ia+i+0;
                idxa1 = ia+i+1;
                idxa2 = ia+i+2;
                idxa3 = ia+i+3;
                offsa = ja;
                idxb0 = jb+j+0;
                idxb1 = jb+j+1;
                idxb2 = jb+j+2;
                idxb3 = jb+j+3;
                offsb = ib;
                v00 = 0.0;
                v01 = 0.0;
                v02 = 0.0;
                v03 = 0.0;
                v10 = 0.0;
                v11 = 0.0;
                v12 = 0.0;
                v13 = 0.0;
                v20 = 0.0;
                v21 = 0.0;
                v22 = 0.0;
                v23 = 0.0;
                v30 = 0.0;
                v31 = 0.0;
                v32 = 0.0;
                v33 = 0.0;
                
                /*
                 * Different variants of internal loop
                 */
                for(t=0; t<=k-1; t++)
                {
                    a0 = a->ptr.pp_double[idxa0][offsa];
                    a1 = a->ptr.pp_double[idxa1][offsa];
                    b0 = b->ptr.pp_double[offsb][idxb0];
                    b1 = b->ptr.pp_double[offsb][idxb1];
                    v00 = v00+a0*b0;
                    v01 = v01+a0*b1;
                    v10 = v10+a1*b0;
                    v11 = v11+a1*b1;
                    a2 = a->ptr.pp_double[idxa2][offsa];
                    a3 = a->ptr.pp_double[idxa3][offsa];
                    v20 = v20+a2*b0;
                    v21 = v21+a2*b1;
                    v30 = v30+a3*b0;
                    v31 = v31+a3*b1;
                    b2 = b->ptr.pp_double[offsb][idxb2];
                    b3 = b->ptr.pp_double[offsb][idxb3];
                    v22 = v22+a2*b2;
                    v23 = v23+a2*b3;
                    v32 = v32+a3*b2;
                    v33 = v33+a3*b3;
                    v02 = v02+a0*b2;
                    v03 = v03+a0*b3;
                    v12 = v12+a1*b2;
                    v13 = v13+a1*b3;
                    offsa = offsa+1;
                    offsb = offsb+1;
                }
                if( ae_fp_eq(beta,(double)(0)) )
                {
                    c->ptr.pp_double[ic+i+0][jc+j+0] = alpha*v00;
                    c->ptr.pp_double[ic+i+0][jc+j+1] = alpha*v01;
                    c->ptr.pp_double[ic+i+0][jc+j+2] = alpha*v02;
                    c->ptr.pp_double[ic+i+0][jc+j+3] = alpha*v03;
                    c->ptr.pp_double[ic+i+1][jc+j+0] = alpha*v10;
                    c->ptr.pp_double[ic+i+1][jc+j+1] = alpha*v11;
                    c->ptr.pp_double[ic+i+1][jc+j+2] = alpha*v12;
                    c->ptr.pp_double[ic+i+1][jc+j+3] = alpha*v13;
                    c->ptr.pp_double[ic+i+2][jc+j+0] = alpha*v20;
                    c->ptr.pp_double[ic+i+2][jc+j+1] = alpha*v21;
                    c->ptr.pp_double[ic+i+2][jc+j+2] = alpha*v22;
                    c->ptr.pp_double[ic+i+2][jc+j+3] = alpha*v23;
                    c->ptr.pp_double[ic+i+3][jc+j+0] = alpha*v30;
                    c->ptr.pp_double[ic+i+3][jc+j+1] = alpha*v31;
                    c->ptr.pp_double[ic+i+3][jc+j+2] = alpha*v32;
                    c->ptr.pp_double[ic+i+3][jc+j+3] = alpha*v33;
                }
                else
                {
                    c->ptr.pp_double[ic+i+0][jc+j+0] = beta*c->ptr.pp_double[ic+i+0][jc+j+0]+alpha*v00;
                    c->ptr.pp_double[ic+i+0][jc+j+1] = beta*c->ptr.pp_double[ic+i+0][jc+j+1]+alpha*v01;
                    c->ptr.pp_double[ic+i+0][jc+j+2] = beta*c->ptr.pp_double[ic+i+0][jc+j+2]+alpha*v02;
                    c->ptr.pp_double[ic+i+0][jc+j+3] = beta*c->ptr.pp_double[ic+i+0][jc+j+3]+alpha*v03;
                    c->ptr.pp_double[ic+i+1][jc+j+0] = beta*c->ptr.pp_double[ic+i+1][jc+j+0]+alpha*v10;
                    c->ptr.pp_double[ic+i+1][jc+j+1] = beta*c->ptr.pp_double[ic+i+1][jc+j+1]+alpha*v11;
                    c->ptr.pp_double[ic+i+1][jc+j+2] = beta*c->ptr.pp_double[ic+i+1][jc+j+2]+alpha*v12;
                    c->ptr.pp_double[ic+i+1][jc+j+3] = beta*c->ptr.pp_double[ic+i+1][jc+j+3]+alpha*v13;
                    c->ptr.pp_double[ic+i+2][jc+j+0] = beta*c->ptr.pp_double[ic+i+2][jc+j+0]+alpha*v20;
                    c->ptr.pp_double[ic+i+2][jc+j+1] = beta*c->ptr.pp_double[ic+i+2][jc+j+1]+alpha*v21;
                    c->ptr.pp_double[ic+i+2][jc+j+2] = beta*c->ptr.pp_double[ic+i+2][jc+j+2]+alpha*v22;
                    c->ptr.pp_double[ic+i+2][jc+j+3] = beta*c->ptr.pp_double[ic+i+2][jc+j+3]+alpha*v23;
                    c->ptr.pp_double[ic+i+3][jc+j+0] = beta*c->ptr.pp_double[ic+i+3][jc+j+0]+alpha*v30;
                    c->ptr.pp_double[ic+i+3][jc+j+1] = beta*c->ptr.pp_double[ic+i+3][jc+j+1]+alpha*v31;
                    c->ptr.pp_double[ic+i+3][jc+j+2] = beta*c->ptr.pp_double[ic+i+3][jc+j+2]+alpha*v32;
                    c->ptr.pp_double[ic+i+3][jc+j+3] = beta*c->ptr.pp_double[ic+i+3][jc+j+3]+alpha*v33;
                }
            }
            else
            {
                
                /*
                 * Determine submatrix [I0..I1]x[J0..J1] to process
                 */
                i0 = i;
                i1 = ae_minint(i+3, m-1, _state);
                j0 = j;
                j1 = ae_minint(j+3, n-1, _state);
                
                /*
                 * Process submatrix
                 */
                for(ik=i0; ik<=i1; ik++)
                {
                    for(jk=j0; jk<=j1; jk++)
                    {
                        if( k==0||ae_fp_eq(alpha,(double)(0)) )
                        {
                            v = (double)(0);
                        }
                        else
                        {
                            v = ae_v_dotproduct(&a->ptr.pp_double[ia+ik][ja], 1, &b->ptr.pp_double[ib][jb+jk], b->stride, ae_v_len(ja,ja+k-1));
                        }
                        if( ae_fp_eq(beta,(double)(0)) )
                        {
                            c->ptr.pp_double[ic+ik][jc+jk] = alpha*v;
                        }
                        else
                        {
                            c->ptr.pp_double[ic+ik][jc+jk] = beta*c->ptr.pp_double[ic+ik][jc+jk]+alpha*v;
                        }
                    }
                }
            }
            j = j+4;
        }
        i = i+4;
    }
}


/*************************************************************************
RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
with OpTypeA=0 and OpTypeB=1.

Additional info:
* this function requires that Alpha<>0 (assertion is thrown otherwise)

INPUT PARAMETERS
    M       -   matrix size, M>0
    N       -   matrix size, N>0
    K       -   matrix size, K>0
    Alpha   -   coefficient
    A       -   matrix
    IA      -   submatrix offset
    JA      -   submatrix offset
    B       -   matrix
    IB      -   submatrix offset
    JB      -   submatrix offset
    Beta    -   coefficient
    C       -   PREALLOCATED output matrix
    IC      -   submatrix offset
    JC      -   submatrix offset

  -- ALGLIB routine --
     27.03.2013
     Bochkanov Sergey
*************************************************************************/
void rmatrixgemmk44v01(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Real    */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;
    double v00;
    double v01;
    double v02;
    double v03;
    double v10;
    double v11;
    double v12;
    double v13;
    double v20;
    double v21;
    double v22;
    double v23;
    double v30;
    double v31;
    double v32;
    double v33;
    double a0;
    double a1;
    double a2;
    double a3;
    double b0;
    double b1;
    double b2;
    double b3;
    ae_int_t idxa0;
    ae_int_t idxa1;
    ae_int_t idxa2;
    ae_int_t idxa3;
    ae_int_t idxb0;
    ae_int_t idxb1;
    ae_int_t idxb2;
    ae_int_t idxb3;
    ae_int_t i0;
    ae_int_t i1;
    ae_int_t ik;
    ae_int_t j0;
    ae_int_t j1;
    ae_int_t jk;
    ae_int_t t;
    ae_int_t offsa;
    ae_int_t offsb;


    ae_assert(ae_fp_neq(alpha,(double)(0)), "RMatrixGEMMK44V00: internal error (Alpha=0)", _state);
    
    /*
     * if matrix size is zero
     */
    if( m==0||n==0 )
    {
        return;
    }
    
    /*
     * A*B'
     */
    i = 0;
    while(i<m)
    {
        j = 0;
        while(j<n)
        {
            
            /*
             * Choose between specialized 4x4 code and general code
             */
            if( i+4<=m&&j+4<=n )
            {
                
                /*
                 * Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                 *
                 * This submatrix is calculated as sum of K rank-1 products,
                 * with operands cached in local variables in order to speed
                 * up operations with arrays.
                 */
                idxa0 = ia+i+0;
                idxa1 = ia+i+1;
                idxa2 = ia+i+2;
                idxa3 = ia+i+3;
                offsa = ja;
                idxb0 = ib+j+0;
                idxb1 = ib+j+1;
                idxb2 = ib+j+2;
                idxb3 = ib+j+3;
                offsb = jb;
                v00 = 0.0;
                v01 = 0.0;
                v02 = 0.0;
                v03 = 0.0;
                v10 = 0.0;
                v11 = 0.0;
                v12 = 0.0;
                v13 = 0.0;
                v20 = 0.0;
                v21 = 0.0;
                v22 = 0.0;
                v23 = 0.0;
                v30 = 0.0;
                v31 = 0.0;
                v32 = 0.0;
                v33 = 0.0;
                for(t=0; t<=k-1; t++)
                {
                    a0 = a->ptr.pp_double[idxa0][offsa];
                    a1 = a->ptr.pp_double[idxa1][offsa];
                    b0 = b->ptr.pp_double[idxb0][offsb];
                    b1 = b->ptr.pp_double[idxb1][offsb];
                    v00 = v00+a0*b0;
                    v01 = v01+a0*b1;
                    v10 = v10+a1*b0;
                    v11 = v11+a1*b1;
                    a2 = a->ptr.pp_double[idxa2][offsa];
                    a3 = a->ptr.pp_double[idxa3][offsa];
                    v20 = v20+a2*b0;
                    v21 = v21+a2*b1;
                    v30 = v30+a3*b0;
                    v31 = v31+a3*b1;
                    b2 = b->ptr.pp_double[idxb2][offsb];
                    b3 = b->ptr.pp_double[idxb3][offsb];
                    v22 = v22+a2*b2;
                    v23 = v23+a2*b3;
                    v32 = v32+a3*b2;
                    v33 = v33+a3*b3;
                    v02 = v02+a0*b2;
                    v03 = v03+a0*b3;
                    v12 = v12+a1*b2;
                    v13 = v13+a1*b3;
                    offsa = offsa+1;
                    offsb = offsb+1;
                }
                if( ae_fp_eq(beta,(double)(0)) )
                {
                    c->ptr.pp_double[ic+i+0][jc+j+0] = alpha*v00;
                    c->ptr.pp_double[ic+i+0][jc+j+1] = alpha*v01;
                    c->ptr.pp_double[ic+i+0][jc+j+2] = alpha*v02;
                    c->ptr.pp_double[ic+i+0][jc+j+3] = alpha*v03;
                    c->ptr.pp_double[ic+i+1][jc+j+0] = alpha*v10;
                    c->ptr.pp_double[ic+i+1][jc+j+1] = alpha*v11;
                    c->ptr.pp_double[ic+i+1][jc+j+2] = alpha*v12;
                    c->ptr.pp_double[ic+i+1][jc+j+3] = alpha*v13;
                    c->ptr.pp_double[ic+i+2][jc+j+0] = alpha*v20;
                    c->ptr.pp_double[ic+i+2][jc+j+1] = alpha*v21;
                    c->ptr.pp_double[ic+i+2][jc+j+2] = alpha*v22;
                    c->ptr.pp_double[ic+i+2][jc+j+3] = alpha*v23;
                    c->ptr.pp_double[ic+i+3][jc+j+0] = alpha*v30;
                    c->ptr.pp_double[ic+i+3][jc+j+1] = alpha*v31;
                    c->ptr.pp_double[ic+i+3][jc+j+2] = alpha*v32;
                    c->ptr.pp_double[ic+i+3][jc+j+3] = alpha*v33;
                }
                else
                {
                    c->ptr.pp_double[ic+i+0][jc+j+0] = beta*c->ptr.pp_double[ic+i+0][jc+j+0]+alpha*v00;
                    c->ptr.pp_double[ic+i+0][jc+j+1] = beta*c->ptr.pp_double[ic+i+0][jc+j+1]+alpha*v01;
                    c->ptr.pp_double[ic+i+0][jc+j+2] = beta*c->ptr.pp_double[ic+i+0][jc+j+2]+alpha*v02;
                    c->ptr.pp_double[ic+i+0][jc+j+3] = beta*c->ptr.pp_double[ic+i+0][jc+j+3]+alpha*v03;
                    c->ptr.pp_double[ic+i+1][jc+j+0] = beta*c->ptr.pp_double[ic+i+1][jc+j+0]+alpha*v10;
                    c->ptr.pp_double[ic+i+1][jc+j+1] = beta*c->ptr.pp_double[ic+i+1][jc+j+1]+alpha*v11;
                    c->ptr.pp_double[ic+i+1][jc+j+2] = beta*c->ptr.pp_double[ic+i+1][jc+j+2]+alpha*v12;
                    c->ptr.pp_double[ic+i+1][jc+j+3] = beta*c->ptr.pp_double[ic+i+1][jc+j+3]+alpha*v13;
                    c->ptr.pp_double[ic+i+2][jc+j+0] = beta*c->ptr.pp_double[ic+i+2][jc+j+0]+alpha*v20;
                    c->ptr.pp_double[ic+i+2][jc+j+1] = beta*c->ptr.pp_double[ic+i+2][jc+j+1]+alpha*v21;
                    c->ptr.pp_double[ic+i+2][jc+j+2] = beta*c->ptr.pp_double[ic+i+2][jc+j+2]+alpha*v22;
                    c->ptr.pp_double[ic+i+2][jc+j+3] = beta*c->ptr.pp_double[ic+i+2][jc+j+3]+alpha*v23;
                    c->ptr.pp_double[ic+i+3][jc+j+0] = beta*c->ptr.pp_double[ic+i+3][jc+j+0]+alpha*v30;
                    c->ptr.pp_double[ic+i+3][jc+j+1] = beta*c->ptr.pp_double[ic+i+3][jc+j+1]+alpha*v31;
                    c->ptr.pp_double[ic+i+3][jc+j+2] = beta*c->ptr.pp_double[ic+i+3][jc+j+2]+alpha*v32;
                    c->ptr.pp_double[ic+i+3][jc+j+3] = beta*c->ptr.pp_double[ic+i+3][jc+j+3]+alpha*v33;
                }
            }
            else
            {
                
                /*
                 * Determine submatrix [I0..I1]x[J0..J1] to process
                 */
                i0 = i;
                i1 = ae_minint(i+3, m-1, _state);
                j0 = j;
                j1 = ae_minint(j+3, n-1, _state);
                
                /*
                 * Process submatrix
                 */
                for(ik=i0; ik<=i1; ik++)
                {
                    for(jk=j0; jk<=j1; jk++)
                    {
                        if( k==0||ae_fp_eq(alpha,(double)(0)) )
                        {
                            v = (double)(0);
                        }
                        else
                        {
                            v = ae_v_dotproduct(&a->ptr.pp_double[ia+ik][ja], 1, &b->ptr.pp_double[ib+jk][jb], 1, ae_v_len(ja,ja+k-1));
                        }
                        if( ae_fp_eq(beta,(double)(0)) )
                        {
                            c->ptr.pp_double[ic+ik][jc+jk] = alpha*v;
                        }
                        else
                        {
                            c->ptr.pp_double[ic+ik][jc+jk] = beta*c->ptr.pp_double[ic+ik][jc+jk]+alpha*v;
                        }
                    }
                }
            }
            j = j+4;
        }
        i = i+4;
    }
}


/*************************************************************************
RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
with OpTypeA=1 and OpTypeB=0.

Additional info:
* this function requires that Alpha<>0 (assertion is thrown otherwise)

INPUT PARAMETERS
    M       -   matrix size, M>0
    N       -   matrix size, N>0
    K       -   matrix size, K>0
    Alpha   -   coefficient
    A       -   matrix
    IA      -   submatrix offset
    JA      -   submatrix offset
    B       -   matrix
    IB      -   submatrix offset
    JB      -   submatrix offset
    Beta    -   coefficient
    C       -   PREALLOCATED output matrix
    IC      -   submatrix offset
    JC      -   submatrix offset

  -- ALGLIB routine --
     27.03.2013
     Bochkanov Sergey
*************************************************************************/
void rmatrixgemmk44v10(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Real    */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;
    double v00;
    double v01;
    double v02;
    double v03;
    double v10;
    double v11;
    double v12;
    double v13;
    double v20;
    double v21;
    double v22;
    double v23;
    double v30;
    double v31;
    double v32;
    double v33;
    double a0;
    double a1;
    double a2;
    double a3;
    double b0;
    double b1;
    double b2;
    double b3;
    ae_int_t idxa0;
    ae_int_t idxa1;
    ae_int_t idxa2;
    ae_int_t idxa3;
    ae_int_t idxb0;
    ae_int_t idxb1;
    ae_int_t idxb2;
    ae_int_t idxb3;
    ae_int_t i0;
    ae_int_t i1;
    ae_int_t ik;
    ae_int_t j0;
    ae_int_t j1;
    ae_int_t jk;
    ae_int_t t;
    ae_int_t offsa;
    ae_int_t offsb;


    ae_assert(ae_fp_neq(alpha,(double)(0)), "RMatrixGEMMK44V00: internal error (Alpha=0)", _state);
    
    /*
     * if matrix size is zero
     */
    if( m==0||n==0 )
    {
        return;
    }
    
    /*
     * A'*B
     */
    i = 0;
    while(i<m)
    {
        j = 0;
        while(j<n)
        {
            
            /*
             * Choose between specialized 4x4 code and general code
             */
            if( i+4<=m&&j+4<=n )
            {
                
                /*
                 * Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                 *
                 * This submatrix is calculated as sum of K rank-1 products,
                 * with operands cached in local variables in order to speed
                 * up operations with arrays.
                 */
                idxa0 = ja+i+0;
                idxa1 = ja+i+1;
                idxa2 = ja+i+2;
                idxa3 = ja+i+3;
                offsa = ia;
                idxb0 = jb+j+0;
                idxb1 = jb+j+1;
                idxb2 = jb+j+2;
                idxb3 = jb+j+3;
                offsb = ib;
                v00 = 0.0;
                v01 = 0.0;
                v02 = 0.0;
                v03 = 0.0;
                v10 = 0.0;
                v11 = 0.0;
                v12 = 0.0;
                v13 = 0.0;
                v20 = 0.0;
                v21 = 0.0;
                v22 = 0.0;
                v23 = 0.0;
                v30 = 0.0;
                v31 = 0.0;
                v32 = 0.0;
                v33 = 0.0;
                for(t=0; t<=k-1; t++)
                {
                    a0 = a->ptr.pp_double[offsa][idxa0];
                    a1 = a->ptr.pp_double[offsa][idxa1];
                    b0 = b->ptr.pp_double[offsb][idxb0];
                    b1 = b->ptr.pp_double[offsb][idxb1];
                    v00 = v00+a0*b0;
                    v01 = v01+a0*b1;
                    v10 = v10+a1*b0;
                    v11 = v11+a1*b1;
                    a2 = a->ptr.pp_double[offsa][idxa2];
                    a3 = a->ptr.pp_double[offsa][idxa3];
                    v20 = v20+a2*b0;
                    v21 = v21+a2*b1;
                    v30 = v30+a3*b0;
                    v31 = v31+a3*b1;
                    b2 = b->ptr.pp_double[offsb][idxb2];
                    b3 = b->ptr.pp_double[offsb][idxb3];
                    v22 = v22+a2*b2;
                    v23 = v23+a2*b3;
                    v32 = v32+a3*b2;
                    v33 = v33+a3*b3;
                    v02 = v02+a0*b2;
                    v03 = v03+a0*b3;
                    v12 = v12+a1*b2;
                    v13 = v13+a1*b3;
                    offsa = offsa+1;
                    offsb = offsb+1;
                }
                if( ae_fp_eq(beta,(double)(0)) )
                {
                    c->ptr.pp_double[ic+i+0][jc+j+0] = alpha*v00;
                    c->ptr.pp_double[ic+i+0][jc+j+1] = alpha*v01;
                    c->ptr.pp_double[ic+i+0][jc+j+2] = alpha*v02;
                    c->ptr.pp_double[ic+i+0][jc+j+3] = alpha*v03;
                    c->ptr.pp_double[ic+i+1][jc+j+0] = alpha*v10;
                    c->ptr.pp_double[ic+i+1][jc+j+1] = alpha*v11;
                    c->ptr.pp_double[ic+i+1][jc+j+2] = alpha*v12;
                    c->ptr.pp_double[ic+i+1][jc+j+3] = alpha*v13;
                    c->ptr.pp_double[ic+i+2][jc+j+0] = alpha*v20;
                    c->ptr.pp_double[ic+i+2][jc+j+1] = alpha*v21;
                    c->ptr.pp_double[ic+i+2][jc+j+2] = alpha*v22;
                    c->ptr.pp_double[ic+i+2][jc+j+3] = alpha*v23;
                    c->ptr.pp_double[ic+i+3][jc+j+0] = alpha*v30;
                    c->ptr.pp_double[ic+i+3][jc+j+1] = alpha*v31;
                    c->ptr.pp_double[ic+i+3][jc+j+2] = alpha*v32;
                    c->ptr.pp_double[ic+i+3][jc+j+3] = alpha*v33;
                }
                else
                {
                    c->ptr.pp_double[ic+i+0][jc+j+0] = beta*c->ptr.pp_double[ic+i+0][jc+j+0]+alpha*v00;
                    c->ptr.pp_double[ic+i+0][jc+j+1] = beta*c->ptr.pp_double[ic+i+0][jc+j+1]+alpha*v01;
                    c->ptr.pp_double[ic+i+0][jc+j+2] = beta*c->ptr.pp_double[ic+i+0][jc+j+2]+alpha*v02;
                    c->ptr.pp_double[ic+i+0][jc+j+3] = beta*c->ptr.pp_double[ic+i+0][jc+j+3]+alpha*v03;
                    c->ptr.pp_double[ic+i+1][jc+j+0] = beta*c->ptr.pp_double[ic+i+1][jc+j+0]+alpha*v10;
                    c->ptr.pp_double[ic+i+1][jc+j+1] = beta*c->ptr.pp_double[ic+i+1][jc+j+1]+alpha*v11;
                    c->ptr.pp_double[ic+i+1][jc+j+2] = beta*c->ptr.pp_double[ic+i+1][jc+j+2]+alpha*v12;
                    c->ptr.pp_double[ic+i+1][jc+j+3] = beta*c->ptr.pp_double[ic+i+1][jc+j+3]+alpha*v13;
                    c->ptr.pp_double[ic+i+2][jc+j+0] = beta*c->ptr.pp_double[ic+i+2][jc+j+0]+alpha*v20;
                    c->ptr.pp_double[ic+i+2][jc+j+1] = beta*c->ptr.pp_double[ic+i+2][jc+j+1]+alpha*v21;
                    c->ptr.pp_double[ic+i+2][jc+j+2] = beta*c->ptr.pp_double[ic+i+2][jc+j+2]+alpha*v22;
                    c->ptr.pp_double[ic+i+2][jc+j+3] = beta*c->ptr.pp_double[ic+i+2][jc+j+3]+alpha*v23;
                    c->ptr.pp_double[ic+i+3][jc+j+0] = beta*c->ptr.pp_double[ic+i+3][jc+j+0]+alpha*v30;
                    c->ptr.pp_double[ic+i+3][jc+j+1] = beta*c->ptr.pp_double[ic+i+3][jc+j+1]+alpha*v31;
                    c->ptr.pp_double[ic+i+3][jc+j+2] = beta*c->ptr.pp_double[ic+i+3][jc+j+2]+alpha*v32;
                    c->ptr.pp_double[ic+i+3][jc+j+3] = beta*c->ptr.pp_double[ic+i+3][jc+j+3]+alpha*v33;
                }
            }
            else
            {
                
                /*
                 * Determine submatrix [I0..I1]x[J0..J1] to process
                 */
                i0 = i;
                i1 = ae_minint(i+3, m-1, _state);
                j0 = j;
                j1 = ae_minint(j+3, n-1, _state);
                
                /*
                 * Process submatrix
                 */
                for(ik=i0; ik<=i1; ik++)
                {
                    for(jk=j0; jk<=j1; jk++)
                    {
                        if( k==0||ae_fp_eq(alpha,(double)(0)) )
                        {
                            v = (double)(0);
                        }
                        else
                        {
                            v = 0.0;
                            v = ae_v_dotproduct(&a->ptr.pp_double[ia][ja+ik], a->stride, &b->ptr.pp_double[ib][jb+jk], b->stride, ae_v_len(ia,ia+k-1));
                        }
                        if( ae_fp_eq(beta,(double)(0)) )
                        {
                            c->ptr.pp_double[ic+ik][jc+jk] = alpha*v;
                        }
                        else
                        {
                            c->ptr.pp_double[ic+ik][jc+jk] = beta*c->ptr.pp_double[ic+ik][jc+jk]+alpha*v;
                        }
                    }
                }
            }
            j = j+4;
        }
        i = i+4;
    }
}


/*************************************************************************
RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
with OpTypeA=1 and OpTypeB=1.

Additional info:
* this function requires that Alpha<>0 (assertion is thrown otherwise)

INPUT PARAMETERS
    M       -   matrix size, M>0
    N       -   matrix size, N>0
    K       -   matrix size, K>0
    Alpha   -   coefficient
    A       -   matrix
    IA      -   submatrix offset
    JA      -   submatrix offset
    B       -   matrix
    IB      -   submatrix offset
    JB      -   submatrix offset
    Beta    -   coefficient
    C       -   PREALLOCATED output matrix
    IC      -   submatrix offset
    JC      -   submatrix offset

  -- ALGLIB routine --
     27.03.2013
     Bochkanov Sergey
*************************************************************************/
void rmatrixgemmk44v11(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Real    */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;
    double v00;
    double v01;
    double v02;
    double v03;
    double v10;
    double v11;
    double v12;
    double v13;
    double v20;
    double v21;
    double v22;
    double v23;
    double v30;
    double v31;
    double v32;
    double v33;
    double a0;
    double a1;
    double a2;
    double a3;
    double b0;
    double b1;
    double b2;
    double b3;
    ae_int_t idxa0;
    ae_int_t idxa1;
    ae_int_t idxa2;
    ae_int_t idxa3;
    ae_int_t idxb0;
    ae_int_t idxb1;
    ae_int_t idxb2;
    ae_int_t idxb3;
    ae_int_t i0;
    ae_int_t i1;
    ae_int_t ik;
    ae_int_t j0;
    ae_int_t j1;
    ae_int_t jk;
    ae_int_t t;
    ae_int_t offsa;
    ae_int_t offsb;


    ae_assert(ae_fp_neq(alpha,(double)(0)), "RMatrixGEMMK44V00: internal error (Alpha=0)", _state);
    
    /*
     * if matrix size is zero
     */
    if( m==0||n==0 )
    {
        return;
    }
    
    /*
     * A'*B'
     */
    i = 0;
    while(i<m)
    {
        j = 0;
        while(j<n)
        {
            
            /*
             * Choose between specialized 4x4 code and general code
             */
            if( i+4<=m&&j+4<=n )
            {
                
                /*
                 * Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                 *
                 * This submatrix is calculated as sum of K rank-1 products,
                 * with operands cached in local variables in order to speed
                 * up operations with arrays.
                 */
                idxa0 = ja+i+0;
                idxa1 = ja+i+1;
                idxa2 = ja+i+2;
                idxa3 = ja+i+3;
                offsa = ia;
                idxb0 = ib+j+0;
                idxb1 = ib+j+1;
                idxb2 = ib+j+2;
                idxb3 = ib+j+3;
                offsb = jb;
                v00 = 0.0;
                v01 = 0.0;
                v02 = 0.0;
                v03 = 0.0;
                v10 = 0.0;
                v11 = 0.0;
                v12 = 0.0;
                v13 = 0.0;
                v20 = 0.0;
                v21 = 0.0;
                v22 = 0.0;
                v23 = 0.0;
                v30 = 0.0;
                v31 = 0.0;
                v32 = 0.0;
                v33 = 0.0;
                for(t=0; t<=k-1; t++)
                {
                    a0 = a->ptr.pp_double[offsa][idxa0];
                    a1 = a->ptr.pp_double[offsa][idxa1];
                    b0 = b->ptr.pp_double[idxb0][offsb];
                    b1 = b->ptr.pp_double[idxb1][offsb];
                    v00 = v00+a0*b0;
                    v01 = v01+a0*b1;
                    v10 = v10+a1*b0;
                    v11 = v11+a1*b1;
                    a2 = a->ptr.pp_double[offsa][idxa2];
                    a3 = a->ptr.pp_double[offsa][idxa3];
                    v20 = v20+a2*b0;
                    v21 = v21+a2*b1;
                    v30 = v30+a3*b0;
                    v31 = v31+a3*b1;
                    b2 = b->ptr.pp_double[idxb2][offsb];
                    b3 = b->ptr.pp_double[idxb3][offsb];
                    v22 = v22+a2*b2;
                    v23 = v23+a2*b3;
                    v32 = v32+a3*b2;
                    v33 = v33+a3*b3;
                    v02 = v02+a0*b2;
                    v03 = v03+a0*b3;
                    v12 = v12+a1*b2;
                    v13 = v13+a1*b3;
                    offsa = offsa+1;
                    offsb = offsb+1;
                }
                if( ae_fp_eq(beta,(double)(0)) )
                {
                    c->ptr.pp_double[ic+i+0][jc+j+0] = alpha*v00;
                    c->ptr.pp_double[ic+i+0][jc+j+1] = alpha*v01;
                    c->ptr.pp_double[ic+i+0][jc+j+2] = alpha*v02;
                    c->ptr.pp_double[ic+i+0][jc+j+3] = alpha*v03;
                    c->ptr.pp_double[ic+i+1][jc+j+0] = alpha*v10;
                    c->ptr.pp_double[ic+i+1][jc+j+1] = alpha*v11;
                    c->ptr.pp_double[ic+i+1][jc+j+2] = alpha*v12;
                    c->ptr.pp_double[ic+i+1][jc+j+3] = alpha*v13;
                    c->ptr.pp_double[ic+i+2][jc+j+0] = alpha*v20;
                    c->ptr.pp_double[ic+i+2][jc+j+1] = alpha*v21;
                    c->ptr.pp_double[ic+i+2][jc+j+2] = alpha*v22;
                    c->ptr.pp_double[ic+i+2][jc+j+3] = alpha*v23;
                    c->ptr.pp_double[ic+i+3][jc+j+0] = alpha*v30;
                    c->ptr.pp_double[ic+i+3][jc+j+1] = alpha*v31;
                    c->ptr.pp_double[ic+i+3][jc+j+2] = alpha*v32;
                    c->ptr.pp_double[ic+i+3][jc+j+3] = alpha*v33;
                }
                else
                {
                    c->ptr.pp_double[ic+i+0][jc+j+0] = beta*c->ptr.pp_double[ic+i+0][jc+j+0]+alpha*v00;
                    c->ptr.pp_double[ic+i+0][jc+j+1] = beta*c->ptr.pp_double[ic+i+0][jc+j+1]+alpha*v01;
                    c->ptr.pp_double[ic+i+0][jc+j+2] = beta*c->ptr.pp_double[ic+i+0][jc+j+2]+alpha*v02;
                    c->ptr.pp_double[ic+i+0][jc+j+3] = beta*c->ptr.pp_double[ic+i+0][jc+j+3]+alpha*v03;
                    c->ptr.pp_double[ic+i+1][jc+j+0] = beta*c->ptr.pp_double[ic+i+1][jc+j+0]+alpha*v10;
                    c->ptr.pp_double[ic+i+1][jc+j+1] = beta*c->ptr.pp_double[ic+i+1][jc+j+1]+alpha*v11;
                    c->ptr.pp_double[ic+i+1][jc+j+2] = beta*c->ptr.pp_double[ic+i+1][jc+j+2]+alpha*v12;
                    c->ptr.pp_double[ic+i+1][jc+j+3] = beta*c->ptr.pp_double[ic+i+1][jc+j+3]+alpha*v13;
                    c->ptr.pp_double[ic+i+2][jc+j+0] = beta*c->ptr.pp_double[ic+i+2][jc+j+0]+alpha*v20;
                    c->ptr.pp_double[ic+i+2][jc+j+1] = beta*c->ptr.pp_double[ic+i+2][jc+j+1]+alpha*v21;
                    c->ptr.pp_double[ic+i+2][jc+j+2] = beta*c->ptr.pp_double[ic+i+2][jc+j+2]+alpha*v22;
                    c->ptr.pp_double[ic+i+2][jc+j+3] = beta*c->ptr.pp_double[ic+i+2][jc+j+3]+alpha*v23;
                    c->ptr.pp_double[ic+i+3][jc+j+0] = beta*c->ptr.pp_double[ic+i+3][jc+j+0]+alpha*v30;
                    c->ptr.pp_double[ic+i+3][jc+j+1] = beta*c->ptr.pp_double[ic+i+3][jc+j+1]+alpha*v31;
                    c->ptr.pp_double[ic+i+3][jc+j+2] = beta*c->ptr.pp_double[ic+i+3][jc+j+2]+alpha*v32;
                    c->ptr.pp_double[ic+i+3][jc+j+3] = beta*c->ptr.pp_double[ic+i+3][jc+j+3]+alpha*v33;
                }
            }
            else
            {
                
                /*
                 * Determine submatrix [I0..I1]x[J0..J1] to process
                 */
                i0 = i;
                i1 = ae_minint(i+3, m-1, _state);
                j0 = j;
                j1 = ae_minint(j+3, n-1, _state);
                
                /*
                 * Process submatrix
                 */
                for(ik=i0; ik<=i1; ik++)
                {
                    for(jk=j0; jk<=j1; jk++)
                    {
                        if( k==0||ae_fp_eq(alpha,(double)(0)) )
                        {
                            v = (double)(0);
                        }
                        else
                        {
                            v = 0.0;
                            v = ae_v_dotproduct(&a->ptr.pp_double[ia][ja+ik], a->stride, &b->ptr.pp_double[ib+jk][jb], 1, ae_v_len(ia,ia+k-1));
                        }
                        if( ae_fp_eq(beta,(double)(0)) )
                        {
                            c->ptr.pp_double[ic+ik][jc+jk] = alpha*v;
                        }
                        else
                        {
                            c->ptr.pp_double[ic+ik][jc+jk] = beta*c->ptr.pp_double[ic+ik][jc+jk]+alpha*v;
                        }
                    }
                }
            }
            j = j+4;
        }
        i = i+4;
    }
}


/*$ End $*/
