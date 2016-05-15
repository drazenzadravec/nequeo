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
#include "blas.h"


/*$ Declarations $*/


/*$ Body $*/


double vectornorm2(/* Real    */ ae_vector* x,
     ae_int_t i1,
     ae_int_t i2,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t ix;
    double absxi;
    double scl;
    double ssq;
    double result;


    n = i2-i1+1;
    if( n<1 )
    {
        result = (double)(0);
        return result;
    }
    if( n==1 )
    {
        result = ae_fabs(x->ptr.p_double[i1], _state);
        return result;
    }
    scl = (double)(0);
    ssq = (double)(1);
    for(ix=i1; ix<=i2; ix++)
    {
        if( ae_fp_neq(x->ptr.p_double[ix],(double)(0)) )
        {
            absxi = ae_fabs(x->ptr.p_double[ix], _state);
            if( ae_fp_less(scl,absxi) )
            {
                ssq = 1+ssq*ae_sqr(scl/absxi, _state);
                scl = absxi;
            }
            else
            {
                ssq = ssq+ae_sqr(absxi/scl, _state);
            }
        }
    }
    result = scl*ae_sqrt(ssq, _state);
    return result;
}


ae_int_t vectoridxabsmax(/* Real    */ ae_vector* x,
     ae_int_t i1,
     ae_int_t i2,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t result;


    result = i1;
    for(i=i1+1; i<=i2; i++)
    {
        if( ae_fp_greater(ae_fabs(x->ptr.p_double[i], _state),ae_fabs(x->ptr.p_double[result], _state)) )
        {
            result = i;
        }
    }
    return result;
}


ae_int_t columnidxabsmax(/* Real    */ ae_matrix* x,
     ae_int_t i1,
     ae_int_t i2,
     ae_int_t j,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t result;


    result = i1;
    for(i=i1+1; i<=i2; i++)
    {
        if( ae_fp_greater(ae_fabs(x->ptr.pp_double[i][j], _state),ae_fabs(x->ptr.pp_double[result][j], _state)) )
        {
            result = i;
        }
    }
    return result;
}


ae_int_t rowidxabsmax(/* Real    */ ae_matrix* x,
     ae_int_t j1,
     ae_int_t j2,
     ae_int_t i,
     ae_state *_state)
{
    ae_int_t j;
    ae_int_t result;


    result = j1;
    for(j=j1+1; j<=j2; j++)
    {
        if( ae_fp_greater(ae_fabs(x->ptr.pp_double[i][j], _state),ae_fabs(x->ptr.pp_double[i][result], _state)) )
        {
            result = j;
        }
    }
    return result;
}


double upperhessenberg1norm(/* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t i2,
     ae_int_t j1,
     ae_int_t j2,
     /* Real    */ ae_vector* work,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double result;


    ae_assert(i2-i1==j2-j1, "UpperHessenberg1Norm: I2-I1<>J2-J1!", _state);
    for(j=j1; j<=j2; j++)
    {
        work->ptr.p_double[j] = (double)(0);
    }
    for(i=i1; i<=i2; i++)
    {
        for(j=ae_maxint(j1, j1+i-i1-1, _state); j<=j2; j++)
        {
            work->ptr.p_double[j] = work->ptr.p_double[j]+ae_fabs(a->ptr.pp_double[i][j], _state);
        }
    }
    result = (double)(0);
    for(j=j1; j<=j2; j++)
    {
        result = ae_maxreal(result, work->ptr.p_double[j], _state);
    }
    return result;
}


void copymatrix(/* Real    */ ae_matrix* a,
     ae_int_t is1,
     ae_int_t is2,
     ae_int_t js1,
     ae_int_t js2,
     /* Real    */ ae_matrix* b,
     ae_int_t id1,
     ae_int_t id2,
     ae_int_t jd1,
     ae_int_t jd2,
     ae_state *_state)
{
    ae_int_t isrc;
    ae_int_t idst;


    if( is1>is2||js1>js2 )
    {
        return;
    }
    ae_assert(is2-is1==id2-id1, "CopyMatrix: different sizes!", _state);
    ae_assert(js2-js1==jd2-jd1, "CopyMatrix: different sizes!", _state);
    for(isrc=is1; isrc<=is2; isrc++)
    {
        idst = isrc-is1+id1;
        ae_v_move(&b->ptr.pp_double[idst][jd1], 1, &a->ptr.pp_double[isrc][js1], 1, ae_v_len(jd1,jd2));
    }
}


void inplacetranspose(/* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t i2,
     ae_int_t j1,
     ae_int_t j2,
     /* Real    */ ae_vector* work,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t ips;
    ae_int_t jps;
    ae_int_t l;


    if( i1>i2||j1>j2 )
    {
        return;
    }
    ae_assert(i1-i2==j1-j2, "InplaceTranspose error: incorrect array size!", _state);
    for(i=i1; i<=i2-1; i++)
    {
        j = j1+i-i1;
        ips = i+1;
        jps = j1+ips-i1;
        l = i2-i;
        ae_v_move(&work->ptr.p_double[1], 1, &a->ptr.pp_double[ips][j], a->stride, ae_v_len(1,l));
        ae_v_move(&a->ptr.pp_double[ips][j], a->stride, &a->ptr.pp_double[i][jps], 1, ae_v_len(ips,i2));
        ae_v_move(&a->ptr.pp_double[i][jps], 1, &work->ptr.p_double[1], 1, ae_v_len(jps,j2));
    }
}


void copyandtranspose(/* Real    */ ae_matrix* a,
     ae_int_t is1,
     ae_int_t is2,
     ae_int_t js1,
     ae_int_t js2,
     /* Real    */ ae_matrix* b,
     ae_int_t id1,
     ae_int_t id2,
     ae_int_t jd1,
     ae_int_t jd2,
     ae_state *_state)
{
    ae_int_t isrc;
    ae_int_t jdst;


    if( is1>is2||js1>js2 )
    {
        return;
    }
    ae_assert(is2-is1==jd2-jd1, "CopyAndTranspose: different sizes!", _state);
    ae_assert(js2-js1==id2-id1, "CopyAndTranspose: different sizes!", _state);
    for(isrc=is1; isrc<=is2; isrc++)
    {
        jdst = isrc-is1+jd1;
        ae_v_move(&b->ptr.pp_double[id1][jdst], b->stride, &a->ptr.pp_double[isrc][js1], 1, ae_v_len(id1,id2));
    }
}


void matrixvectormultiply(/* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t i2,
     ae_int_t j1,
     ae_int_t j2,
     ae_bool trans,
     /* Real    */ ae_vector* x,
     ae_int_t ix1,
     ae_int_t ix2,
     double alpha,
     /* Real    */ ae_vector* y,
     ae_int_t iy1,
     ae_int_t iy2,
     double beta,
     ae_state *_state)
{
    ae_int_t i;
    double v;


    if( !trans )
    {
        
        /*
         * y := alpha*A*x + beta*y;
         */
        if( i1>i2||j1>j2 )
        {
            return;
        }
        ae_assert(j2-j1==ix2-ix1, "MatrixVectorMultiply: A and X dont match!", _state);
        ae_assert(i2-i1==iy2-iy1, "MatrixVectorMultiply: A and Y dont match!", _state);
        
        /*
         * beta*y
         */
        if( ae_fp_eq(beta,(double)(0)) )
        {
            for(i=iy1; i<=iy2; i++)
            {
                y->ptr.p_double[i] = (double)(0);
            }
        }
        else
        {
            ae_v_muld(&y->ptr.p_double[iy1], 1, ae_v_len(iy1,iy2), beta);
        }
        
        /*
         * alpha*A*x
         */
        for(i=i1; i<=i2; i++)
        {
            v = ae_v_dotproduct(&a->ptr.pp_double[i][j1], 1, &x->ptr.p_double[ix1], 1, ae_v_len(j1,j2));
            y->ptr.p_double[iy1+i-i1] = y->ptr.p_double[iy1+i-i1]+alpha*v;
        }
    }
    else
    {
        
        /*
         * y := alpha*A'*x + beta*y;
         */
        if( i1>i2||j1>j2 )
        {
            return;
        }
        ae_assert(i2-i1==ix2-ix1, "MatrixVectorMultiply: A and X dont match!", _state);
        ae_assert(j2-j1==iy2-iy1, "MatrixVectorMultiply: A and Y dont match!", _state);
        
        /*
         * beta*y
         */
        if( ae_fp_eq(beta,(double)(0)) )
        {
            for(i=iy1; i<=iy2; i++)
            {
                y->ptr.p_double[i] = (double)(0);
            }
        }
        else
        {
            ae_v_muld(&y->ptr.p_double[iy1], 1, ae_v_len(iy1,iy2), beta);
        }
        
        /*
         * alpha*A'*x
         */
        for(i=i1; i<=i2; i++)
        {
            v = alpha*x->ptr.p_double[ix1+i-i1];
            ae_v_addd(&y->ptr.p_double[iy1], 1, &a->ptr.pp_double[i][j1], 1, ae_v_len(iy1,iy2), v);
        }
    }
}


double pythag2(double x, double y, ae_state *_state)
{
    double w;
    double xabs;
    double yabs;
    double z;
    double result;


    xabs = ae_fabs(x, _state);
    yabs = ae_fabs(y, _state);
    w = ae_maxreal(xabs, yabs, _state);
    z = ae_minreal(xabs, yabs, _state);
    if( ae_fp_eq(z,(double)(0)) )
    {
        result = w;
    }
    else
    {
        result = w*ae_sqrt(1+ae_sqr(z/w, _state), _state);
    }
    return result;
}


void matrixmatrixmultiply(/* Real    */ ae_matrix* a,
     ae_int_t ai1,
     ae_int_t ai2,
     ae_int_t aj1,
     ae_int_t aj2,
     ae_bool transa,
     /* Real    */ ae_matrix* b,
     ae_int_t bi1,
     ae_int_t bi2,
     ae_int_t bj1,
     ae_int_t bj2,
     ae_bool transb,
     double alpha,
     /* Real    */ ae_matrix* c,
     ae_int_t ci1,
     ae_int_t ci2,
     ae_int_t cj1,
     ae_int_t cj2,
     double beta,
     /* Real    */ ae_vector* work,
     ae_state *_state)
{
    ae_int_t arows;
    ae_int_t acols;
    ae_int_t brows;
    ae_int_t bcols;
    ae_int_t crows;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t l;
    ae_int_t r;
    double v;


    
    /*
     * Setup
     */
    if( !transa )
    {
        arows = ai2-ai1+1;
        acols = aj2-aj1+1;
    }
    else
    {
        arows = aj2-aj1+1;
        acols = ai2-ai1+1;
    }
    if( !transb )
    {
        brows = bi2-bi1+1;
        bcols = bj2-bj1+1;
    }
    else
    {
        brows = bj2-bj1+1;
        bcols = bi2-bi1+1;
    }
    ae_assert(acols==brows, "MatrixMatrixMultiply: incorrect matrix sizes!", _state);
    if( ((arows<=0||acols<=0)||brows<=0)||bcols<=0 )
    {
        return;
    }
    crows = arows;
    
    /*
     * Test WORK
     */
    i = ae_maxint(arows, acols, _state);
    i = ae_maxint(brows, i, _state);
    i = ae_maxint(i, bcols, _state);
    work->ptr.p_double[1] = (double)(0);
    work->ptr.p_double[i] = (double)(0);
    
    /*
     * Prepare C
     */
    if( ae_fp_eq(beta,(double)(0)) )
    {
        for(i=ci1; i<=ci2; i++)
        {
            for(j=cj1; j<=cj2; j++)
            {
                c->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    else
    {
        for(i=ci1; i<=ci2; i++)
        {
            ae_v_muld(&c->ptr.pp_double[i][cj1], 1, ae_v_len(cj1,cj2), beta);
        }
    }
    
    /*
     * A*B
     */
    if( !transa&&!transb )
    {
        for(l=ai1; l<=ai2; l++)
        {
            for(r=bi1; r<=bi2; r++)
            {
                v = alpha*a->ptr.pp_double[l][aj1+r-bi1];
                k = ci1+l-ai1;
                ae_v_addd(&c->ptr.pp_double[k][cj1], 1, &b->ptr.pp_double[r][bj1], 1, ae_v_len(cj1,cj2), v);
            }
        }
        return;
    }
    
    /*
     * A*B'
     */
    if( !transa&&transb )
    {
        if( arows*acols<brows*bcols )
        {
            for(r=bi1; r<=bi2; r++)
            {
                for(l=ai1; l<=ai2; l++)
                {
                    v = ae_v_dotproduct(&a->ptr.pp_double[l][aj1], 1, &b->ptr.pp_double[r][bj1], 1, ae_v_len(aj1,aj2));
                    c->ptr.pp_double[ci1+l-ai1][cj1+r-bi1] = c->ptr.pp_double[ci1+l-ai1][cj1+r-bi1]+alpha*v;
                }
            }
            return;
        }
        else
        {
            for(l=ai1; l<=ai2; l++)
            {
                for(r=bi1; r<=bi2; r++)
                {
                    v = ae_v_dotproduct(&a->ptr.pp_double[l][aj1], 1, &b->ptr.pp_double[r][bj1], 1, ae_v_len(aj1,aj2));
                    c->ptr.pp_double[ci1+l-ai1][cj1+r-bi1] = c->ptr.pp_double[ci1+l-ai1][cj1+r-bi1]+alpha*v;
                }
            }
            return;
        }
    }
    
    /*
     * A'*B
     */
    if( transa&&!transb )
    {
        for(l=aj1; l<=aj2; l++)
        {
            for(r=bi1; r<=bi2; r++)
            {
                v = alpha*a->ptr.pp_double[ai1+r-bi1][l];
                k = ci1+l-aj1;
                ae_v_addd(&c->ptr.pp_double[k][cj1], 1, &b->ptr.pp_double[r][bj1], 1, ae_v_len(cj1,cj2), v);
            }
        }
        return;
    }
    
    /*
     * A'*B'
     */
    if( transa&&transb )
    {
        if( arows*acols<brows*bcols )
        {
            for(r=bi1; r<=bi2; r++)
            {
                k = cj1+r-bi1;
                for(i=1; i<=crows; i++)
                {
                    work->ptr.p_double[i] = 0.0;
                }
                for(l=ai1; l<=ai2; l++)
                {
                    v = alpha*b->ptr.pp_double[r][bj1+l-ai1];
                    ae_v_addd(&work->ptr.p_double[1], 1, &a->ptr.pp_double[l][aj1], 1, ae_v_len(1,crows), v);
                }
                ae_v_add(&c->ptr.pp_double[ci1][k], c->stride, &work->ptr.p_double[1], 1, ae_v_len(ci1,ci2));
            }
            return;
        }
        else
        {
            for(l=aj1; l<=aj2; l++)
            {
                k = ai2-ai1+1;
                ae_v_move(&work->ptr.p_double[1], 1, &a->ptr.pp_double[ai1][l], a->stride, ae_v_len(1,k));
                for(r=bi1; r<=bi2; r++)
                {
                    v = ae_v_dotproduct(&work->ptr.p_double[1], 1, &b->ptr.pp_double[r][bj1], 1, ae_v_len(1,k));
                    c->ptr.pp_double[ci1+l-aj1][cj1+r-bi1] = c->ptr.pp_double[ci1+l-aj1][cj1+r-bi1]+alpha*v;
                }
            }
            return;
        }
    }
}


/*$ End $*/
