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


#include <stdafx.h>
#include <stdio.h>
#include "testblasunit.h"


/*$ Declarations $*/
static void testblasunit_naivematrixmatrixmultiply(/* Real    */ ae_matrix* a,
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
     ae_state *_state);


/*$ Body $*/


ae_bool testblas(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t n;
    ae_int_t i;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t j2;
    ae_int_t l;
    ae_int_t k;
    ae_int_t r;
    ae_int_t i3;
    ae_int_t j3;
    ae_int_t col1;
    ae_int_t col2;
    ae_int_t row1;
    ae_int_t row2;
    ae_vector x1;
    ae_vector x2;
    ae_matrix a;
    ae_matrix b;
    ae_matrix c1;
    ae_matrix c2;
    double err;
    double e1;
    double e2;
    double e3;
    double v;
    double scl1;
    double scl2;
    double scl3;
    ae_bool was1;
    ae_bool was2;
    ae_bool trans1;
    ae_bool trans2;
    double threshold;
    ae_bool n2errors;
    ae_bool hsnerrors;
    ae_bool amaxerrors;
    ae_bool mverrors;
    ae_bool iterrors;
    ae_bool cterrors;
    ae_bool mmerrors;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&b, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c2, 0, 0, DT_REAL, _state);

    n2errors = ae_false;
    amaxerrors = ae_false;
    hsnerrors = ae_false;
    mverrors = ae_false;
    iterrors = ae_false;
    cterrors = ae_false;
    mmerrors = ae_false;
    waserrors = ae_false;
    threshold = 10000*ae_machineepsilon;
    
    /*
     * Test Norm2
     */
    passcount = 1000;
    e1 = (double)(0);
    e2 = (double)(0);
    e3 = (double)(0);
    scl2 = 0.5*ae_maxrealnumber;
    scl3 = 2*ae_minrealnumber;
    for(pass=1; pass<=passcount; pass++)
    {
        n = 1+ae_randominteger(1000, _state);
        i1 = ae_randominteger(10, _state);
        i2 = n+i1-1;
        ae_vector_set_length(&x1, i2+1, _state);
        ae_vector_set_length(&x2, i2+1, _state);
        for(i=i1; i<=i2; i++)
        {
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        v = (double)(0);
        for(i=i1; i<=i2; i++)
        {
            v = v+ae_sqr(x1.ptr.p_double[i], _state);
        }
        v = ae_sqrt(v, _state);
        e1 = ae_maxreal(e1, ae_fabs(v-vectornorm2(&x1, i1, i2, _state), _state), _state);
        for(i=i1; i<=i2; i++)
        {
            x2.ptr.p_double[i] = scl2*x1.ptr.p_double[i];
        }
        e2 = ae_maxreal(e2, ae_fabs(v*scl2-vectornorm2(&x2, i1, i2, _state), _state), _state);
        for(i=i1; i<=i2; i++)
        {
            x2.ptr.p_double[i] = scl3*x1.ptr.p_double[i];
        }
        e3 = ae_maxreal(e3, ae_fabs(v*scl3-vectornorm2(&x2, i1, i2, _state), _state), _state);
    }
    e2 = e2/scl2;
    e3 = e3/scl3;
    n2errors = (ae_fp_greater_eq(e1,threshold)||ae_fp_greater_eq(e2,threshold))||ae_fp_greater_eq(e3,threshold);
    
    /*
     * Testing VectorAbsMax, Column/Row AbsMax
     */
    ae_vector_set_length(&x1, 5+1, _state);
    x1.ptr.p_double[1] = 2.0;
    x1.ptr.p_double[2] = 0.2;
    x1.ptr.p_double[3] = -1.3;
    x1.ptr.p_double[4] = 0.7;
    x1.ptr.p_double[5] = -3.0;
    amaxerrors = (vectoridxabsmax(&x1, 1, 5, _state)!=5||vectoridxabsmax(&x1, 1, 4, _state)!=1)||vectoridxabsmax(&x1, 2, 4, _state)!=3;
    n = 30;
    ae_vector_set_length(&x1, n+1, _state);
    ae_matrix_set_length(&a, n+1, n+1, _state);
    for(i=1; i<=n; i++)
    {
        for(j=1; j<=n; j++)
        {
            a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
        }
    }
    was1 = ae_false;
    was2 = ae_false;
    for(pass=1; pass<=1000; pass++)
    {
        j = 1+ae_randominteger(n, _state);
        i1 = 1+ae_randominteger(n, _state);
        i2 = i1+ae_randominteger(n+1-i1, _state);
        ae_v_move(&x1.ptr.p_double[i1], 1, &a.ptr.pp_double[i1][j], a.stride, ae_v_len(i1,i2));
        if( vectoridxabsmax(&x1, i1, i2, _state)!=columnidxabsmax(&a, i1, i2, j, _state) )
        {
            was1 = ae_true;
        }
        i = 1+ae_randominteger(n, _state);
        j1 = 1+ae_randominteger(n, _state);
        j2 = j1+ae_randominteger(n+1-j1, _state);
        ae_v_move(&x1.ptr.p_double[j1], 1, &a.ptr.pp_double[i][j1], 1, ae_v_len(j1,j2));
        if( vectoridxabsmax(&x1, j1, j2, _state)!=rowidxabsmax(&a, j1, j2, i, _state) )
        {
            was2 = ae_true;
        }
    }
    amaxerrors = (amaxerrors||was1)||was2;
    
    /*
     * Testing upper Hessenberg 1-norm
     */
    ae_matrix_set_length(&a, 3+1, 3+1, _state);
    ae_vector_set_length(&x1, 3+1, _state);
    a.ptr.pp_double[1][1] = (double)(2);
    a.ptr.pp_double[1][2] = (double)(3);
    a.ptr.pp_double[1][3] = (double)(1);
    a.ptr.pp_double[2][1] = (double)(4);
    a.ptr.pp_double[2][2] = (double)(-5);
    a.ptr.pp_double[2][3] = (double)(8);
    a.ptr.pp_double[3][1] = (double)(99);
    a.ptr.pp_double[3][2] = (double)(3);
    a.ptr.pp_double[3][3] = (double)(1);
    hsnerrors = ae_fp_greater(ae_fabs(upperhessenberg1norm(&a, 1, 3, 1, 3, &x1, _state)-11, _state),threshold);
    
    /*
     * Testing MatrixVectorMultiply
     */
    ae_matrix_set_length(&a, 3+1, 5+1, _state);
    ae_vector_set_length(&x1, 3+1, _state);
    ae_vector_set_length(&x2, 2+1, _state);
    a.ptr.pp_double[2][3] = (double)(2);
    a.ptr.pp_double[2][4] = (double)(-1);
    a.ptr.pp_double[2][5] = (double)(-1);
    a.ptr.pp_double[3][3] = (double)(1);
    a.ptr.pp_double[3][4] = (double)(-2);
    a.ptr.pp_double[3][5] = (double)(2);
    x1.ptr.p_double[1] = (double)(1);
    x1.ptr.p_double[2] = (double)(2);
    x1.ptr.p_double[3] = (double)(1);
    x2.ptr.p_double[1] = (double)(-1);
    x2.ptr.p_double[2] = (double)(-1);
    matrixvectormultiply(&a, 2, 3, 3, 5, ae_false, &x1, 1, 3, 1.0, &x2, 1, 2, 1.0, _state);
    matrixvectormultiply(&a, 2, 3, 3, 5, ae_true, &x2, 1, 2, 1.0, &x1, 1, 3, 1.0, _state);
    e1 = ae_fabs(x1.ptr.p_double[1]+5, _state)+ae_fabs(x1.ptr.p_double[2]-8, _state)+ae_fabs(x1.ptr.p_double[3]+1, _state)+ae_fabs(x2.ptr.p_double[1]+2, _state)+ae_fabs(x2.ptr.p_double[2]+2, _state);
    x1.ptr.p_double[1] = (double)(1);
    x1.ptr.p_double[2] = (double)(2);
    x1.ptr.p_double[3] = (double)(1);
    x2.ptr.p_double[1] = (double)(-1);
    x2.ptr.p_double[2] = (double)(-1);
    matrixvectormultiply(&a, 2, 3, 3, 5, ae_false, &x1, 1, 3, 1.0, &x2, 1, 2, 0.0, _state);
    matrixvectormultiply(&a, 2, 3, 3, 5, ae_true, &x2, 1, 2, 1.0, &x1, 1, 3, 0.0, _state);
    e2 = ae_fabs(x1.ptr.p_double[1]+3, _state)+ae_fabs(x1.ptr.p_double[2]-3, _state)+ae_fabs(x1.ptr.p_double[3]+1, _state)+ae_fabs(x2.ptr.p_double[1]+1, _state)+ae_fabs(x2.ptr.p_double[2]+1, _state);
    mverrors = ae_fp_greater_eq(e1+e2,threshold);
    
    /*
     * testing inplace transpose
     */
    n = 10;
    ae_matrix_set_length(&a, n+1, n+1, _state);
    ae_matrix_set_length(&b, n+1, n+1, _state);
    ae_vector_set_length(&x1, n-1+1, _state);
    for(i=1; i<=n; i++)
    {
        for(j=1; j<=n; j++)
        {
            a.ptr.pp_double[i][j] = ae_randomreal(_state);
        }
    }
    passcount = 10000;
    was1 = ae_false;
    for(pass=1; pass<=passcount; pass++)
    {
        i1 = 1+ae_randominteger(n, _state);
        i2 = i1+ae_randominteger(n-i1+1, _state);
        j1 = 1+ae_randominteger(n-(i2-i1), _state);
        j2 = j1+(i2-i1);
        copymatrix(&a, i1, i2, j1, j2, &b, i1, i2, j1, j2, _state);
        inplacetranspose(&b, i1, i2, j1, j2, &x1, _state);
        for(i=i1; i<=i2; i++)
        {
            for(j=j1; j<=j2; j++)
            {
                if( ae_fp_neq(a.ptr.pp_double[i][j],b.ptr.pp_double[i1+(j-j1)][j1+(i-i1)]) )
                {
                    was1 = ae_true;
                }
            }
        }
    }
    iterrors = was1;
    
    /*
     * testing copy and transpose
     */
    n = 10;
    ae_matrix_set_length(&a, n+1, n+1, _state);
    ae_matrix_set_length(&b, n+1, n+1, _state);
    for(i=1; i<=n; i++)
    {
        for(j=1; j<=n; j++)
        {
            a.ptr.pp_double[i][j] = ae_randomreal(_state);
        }
    }
    passcount = 10000;
    was1 = ae_false;
    for(pass=1; pass<=passcount; pass++)
    {
        i1 = 1+ae_randominteger(n, _state);
        i2 = i1+ae_randominteger(n-i1+1, _state);
        j1 = 1+ae_randominteger(n, _state);
        j2 = j1+ae_randominteger(n-j1+1, _state);
        copyandtranspose(&a, i1, i2, j1, j2, &b, j1, j2, i1, i2, _state);
        for(i=i1; i<=i2; i++)
        {
            for(j=j1; j<=j2; j++)
            {
                if( ae_fp_neq(a.ptr.pp_double[i][j],b.ptr.pp_double[j][i]) )
                {
                    was1 = ae_true;
                }
            }
        }
    }
    cterrors = was1;
    
    /*
     * Testing MatrixMatrixMultiply
     */
    n = 10;
    ae_matrix_set_length(&a, 2*n+1, 2*n+1, _state);
    ae_matrix_set_length(&b, 2*n+1, 2*n+1, _state);
    ae_matrix_set_length(&c1, 2*n+1, 2*n+1, _state);
    ae_matrix_set_length(&c2, 2*n+1, 2*n+1, _state);
    ae_vector_set_length(&x1, n+1, _state);
    ae_vector_set_length(&x2, n+1, _state);
    for(i=1; i<=2*n; i++)
    {
        for(j=1; j<=2*n; j++)
        {
            a.ptr.pp_double[i][j] = ae_randomreal(_state);
            b.ptr.pp_double[i][j] = ae_randomreal(_state);
        }
    }
    passcount = 1000;
    was1 = ae_false;
    for(pass=1; pass<=passcount; pass++)
    {
        for(i=1; i<=2*n; i++)
        {
            for(j=1; j<=2*n; j++)
            {
                c1.ptr.pp_double[i][j] = 2.1*i+3.1*j;
                c2.ptr.pp_double[i][j] = c1.ptr.pp_double[i][j];
            }
        }
        l = 1+ae_randominteger(n, _state);
        k = 1+ae_randominteger(n, _state);
        r = 1+ae_randominteger(n, _state);
        i1 = 1+ae_randominteger(n, _state);
        j1 = 1+ae_randominteger(n, _state);
        i2 = 1+ae_randominteger(n, _state);
        j2 = 1+ae_randominteger(n, _state);
        i3 = 1+ae_randominteger(n, _state);
        j3 = 1+ae_randominteger(n, _state);
        trans1 = ae_fp_greater(ae_randomreal(_state),0.5);
        trans2 = ae_fp_greater(ae_randomreal(_state),0.5);
        if( trans1 )
        {
            col1 = l;
            row1 = k;
        }
        else
        {
            col1 = k;
            row1 = l;
        }
        if( trans2 )
        {
            col2 = k;
            row2 = r;
        }
        else
        {
            col2 = r;
            row2 = k;
        }
        scl1 = ae_randomreal(_state);
        scl2 = ae_randomreal(_state);
        matrixmatrixmultiply(&a, i1, i1+row1-1, j1, j1+col1-1, trans1, &b, i2, i2+row2-1, j2, j2+col2-1, trans2, scl1, &c1, i3, i3+l-1, j3, j3+r-1, scl2, &x1, _state);
        testblasunit_naivematrixmatrixmultiply(&a, i1, i1+row1-1, j1, j1+col1-1, trans1, &b, i2, i2+row2-1, j2, j2+col2-1, trans2, scl1, &c2, i3, i3+l-1, j3, j3+r-1, scl2, _state);
        err = (double)(0);
        for(i=1; i<=l; i++)
        {
            for(j=1; j<=r; j++)
            {
                err = ae_maxreal(err, ae_fabs(c1.ptr.pp_double[i3+i-1][j3+j-1]-c2.ptr.pp_double[i3+i-1][j3+j-1], _state), _state);
            }
        }
        if( ae_fp_greater(err,threshold) )
        {
            was1 = ae_true;
            break;
        }
    }
    mmerrors = was1;
    
    /*
     * report
     */
    waserrors = (((((n2errors||amaxerrors)||hsnerrors)||mverrors)||iterrors)||cterrors)||mmerrors;
    if( !silent )
    {
        printf("TESTING BLAS\n");
        printf("VectorNorm2:                             ");
        if( n2errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("AbsMax (vector/row/column):              ");
        if( amaxerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("UpperHessenberg1Norm:                    ");
        if( hsnerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("MatrixVectorMultiply:                    ");
        if( mverrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("InplaceTranspose:                        ");
        if( iterrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("CopyAndTranspose:                        ");
        if( cterrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("MatrixMatrixMultiply:                    ");
        if( mmerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        if( waserrors )
        {
            printf("TEST FAILED\n");
        }
        else
        {
            printf("TEST PASSED\n");
        }
        printf("\n\n");
    }
    result = !waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testblas(ae_bool silent, ae_state *_state)
{
    return testblas(silent, _state);
}


static void testblasunit_naivematrixmatrixmultiply(/* Real    */ ae_matrix* a,
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
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t arows;
    ae_int_t acols;
    ae_int_t brows;
    ae_int_t bcols;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t l;
    ae_int_t r;
    double v;
    ae_vector x1;
    ae_vector x2;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);

    
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
    ae_assert(acols==brows, "NaiveMatrixMatrixMultiply: incorrect matrix sizes!", _state);
    if( ((arows<=0||acols<=0)||brows<=0)||bcols<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    l = arows;
    r = bcols;
    k = acols;
    ae_vector_set_length(&x1, k+1, _state);
    ae_vector_set_length(&x2, k+1, _state);
    for(i=1; i<=l; i++)
    {
        for(j=1; j<=r; j++)
        {
            if( !transa )
            {
                if( !transb )
                {
                    v = ae_v_dotproduct(&b->ptr.pp_double[bi1][bj1+j-1], b->stride, &a->ptr.pp_double[ai1+i-1][aj1], 1, ae_v_len(bi1,bi2));
                }
                else
                {
                    v = ae_v_dotproduct(&b->ptr.pp_double[bi1+j-1][bj1], 1, &a->ptr.pp_double[ai1+i-1][aj1], 1, ae_v_len(bj1,bj2));
                }
            }
            else
            {
                if( !transb )
                {
                    v = ae_v_dotproduct(&b->ptr.pp_double[bi1][bj1+j-1], b->stride, &a->ptr.pp_double[ai1][aj1+i-1], a->stride, ae_v_len(bi1,bi2));
                }
                else
                {
                    v = ae_v_dotproduct(&b->ptr.pp_double[bi1+j-1][bj1], 1, &a->ptr.pp_double[ai1][aj1+i-1], a->stride, ae_v_len(bj1,bj2));
                }
            }
            if( ae_fp_eq(beta,(double)(0)) )
            {
                c->ptr.pp_double[ci1+i-1][cj1+j-1] = alpha*v;
            }
            else
            {
                c->ptr.pp_double[ci1+i-1][cj1+j-1] = beta*c->ptr.pp_double[ci1+i-1][cj1+j-1]+alpha*v;
            }
        }
    }
    ae_frame_leave(_state);
}


/*$ End $*/
