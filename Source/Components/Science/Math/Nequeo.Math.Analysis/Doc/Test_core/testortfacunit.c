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
#include "testortfacunit.h"


/*$ Declarations $*/
static double testortfacunit_rmatrixdiff(/* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state);
static void testortfacunit_rmatrixmakeacopy(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_state *_state);
static void testortfacunit_cmatrixmakeacopy(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_state *_state);
static void testortfacunit_rmatrixfillsparsea(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double sparcity,
     ae_state *_state);
static void testortfacunit_cmatrixfillsparsea(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double sparcity,
     ae_state *_state);
static void testortfacunit_internalmatrixmatrixmultiply(/* Real    */ ae_matrix* a,
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
     /* Real    */ ae_matrix* c,
     ae_int_t ci1,
     ae_int_t ci2,
     ae_int_t cj1,
     ae_int_t cj2,
     ae_state *_state);
static void testortfacunit_testrqrproblem(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* qrerrors,
     ae_state *_state);
static void testortfacunit_testcqrproblem(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* qrerrors,
     ae_state *_state);
static void testortfacunit_testrlqproblem(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* lqerrors,
     ae_state *_state);
static void testortfacunit_testclqproblem(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* lqerrors,
     ae_state *_state);
static void testortfacunit_testrbdproblem(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* bderrors,
     ae_state *_state);
static void testortfacunit_testrhessproblem(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double threshold,
     ae_bool* hesserrors,
     ae_state *_state);
static void testortfacunit_testrtdproblem(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double threshold,
     ae_bool* tderrors,
     ae_state *_state);
static void testortfacunit_testctdproblem(/* Complex */ ae_matrix* a,
     ae_int_t n,
     double threshold,
     ae_bool* tderrors,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Main unittest subroutine
*************************************************************************/
ae_bool testortfac(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t maxmn;
    double threshold;
    ae_int_t passcount;
    ae_int_t mx;
    ae_matrix ra;
    ae_matrix ca;
    ae_int_t m;
    ae_int_t n;
    ae_int_t pass;
    ae_int_t i;
    ae_int_t j;
    ae_bool rqrerrors;
    ae_bool rlqerrors;
    ae_bool cqrerrors;
    ae_bool clqerrors;
    ae_bool rbderrors;
    ae_bool rhesserrors;
    ae_bool rtderrors;
    ae_bool ctderrors;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ca, 0, 0, DT_COMPLEX, _state);

    waserrors = ae_false;
    rqrerrors = ae_false;
    rlqerrors = ae_false;
    cqrerrors = ae_false;
    clqerrors = ae_false;
    rbderrors = ae_false;
    rhesserrors = ae_false;
    rtderrors = ae_false;
    ctderrors = ae_false;
    maxmn = 3*ablasblocksize(&ra, _state)+1;
    passcount = 1;
    threshold = 5*1000*ae_machineepsilon;
    
    /*
     * Different problems
     */
    for(mx=1; mx<=maxmn; mx++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * Rectangular factorizations: QR, LQ, bidiagonal
             * Matrix types: zero, dense, sparse
             */
            n = 1+ae_randominteger(mx, _state);
            m = 1+ae_randominteger(mx, _state);
            if( ae_fp_greater(ae_randomreal(_state),0.5) )
            {
                n = mx;
            }
            else
            {
                m = mx;
            }
            ae_matrix_set_length(&ra, m, n, _state);
            ae_matrix_set_length(&ca, m, n, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    ra.ptr.pp_double[i][j] = (double)(0);
                    ca.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                }
            }
            testortfacunit_testrqrproblem(&ra, m, n, threshold, &rqrerrors, _state);
            testortfacunit_testrlqproblem(&ra, m, n, threshold, &rlqerrors, _state);
            testortfacunit_testcqrproblem(&ca, m, n, threshold, &cqrerrors, _state);
            testortfacunit_testclqproblem(&ca, m, n, threshold, &clqerrors, _state);
            testortfacunit_testrbdproblem(&ra, m, n, threshold, &rbderrors, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    ra.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    ca.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                    ca.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                }
            }
            testortfacunit_testrqrproblem(&ra, m, n, threshold, &rqrerrors, _state);
            testortfacunit_testrlqproblem(&ra, m, n, threshold, &rlqerrors, _state);
            testortfacunit_testcqrproblem(&ca, m, n, threshold, &cqrerrors, _state);
            testortfacunit_testclqproblem(&ca, m, n, threshold, &clqerrors, _state);
            testortfacunit_testrbdproblem(&ra, m, n, threshold, &rbderrors, _state);
            testortfacunit_rmatrixfillsparsea(&ra, m, n, 0.95, _state);
            testortfacunit_cmatrixfillsparsea(&ca, m, n, 0.95, _state);
            testortfacunit_testrqrproblem(&ra, m, n, threshold, &rqrerrors, _state);
            testortfacunit_testrlqproblem(&ra, m, n, threshold, &rlqerrors, _state);
            testortfacunit_testcqrproblem(&ca, m, n, threshold, &cqrerrors, _state);
            testortfacunit_testclqproblem(&ca, m, n, threshold, &clqerrors, _state);
            testortfacunit_testrbdproblem(&ra, m, n, threshold, &rbderrors, _state);
            
            /*
             * Square factorizations: Hessenberg, tridiagonal
             * Matrix types: zero, dense, sparse
             */
            ae_matrix_set_length(&ra, mx, mx, _state);
            ae_matrix_set_length(&ca, mx, mx, _state);
            for(i=0; i<=mx-1; i++)
            {
                for(j=0; j<=mx-1; j++)
                {
                    ra.ptr.pp_double[i][j] = (double)(0);
                    ca.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                }
            }
            testortfacunit_testrhessproblem(&ra, mx, threshold, &rhesserrors, _state);
            for(i=0; i<=mx-1; i++)
            {
                for(j=0; j<=mx-1; j++)
                {
                    ra.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    ca.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                    ca.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                }
            }
            testortfacunit_testrhessproblem(&ra, mx, threshold, &rhesserrors, _state);
            testortfacunit_rmatrixfillsparsea(&ra, mx, mx, 0.95, _state);
            testortfacunit_cmatrixfillsparsea(&ca, mx, mx, 0.95, _state);
            testortfacunit_testrhessproblem(&ra, mx, threshold, &rhesserrors, _state);
            
            /*
             * Symetric factorizations: tridiagonal
             * Matrix types: zero, dense, sparse
             */
            ae_matrix_set_length(&ra, mx, mx, _state);
            ae_matrix_set_length(&ca, mx, mx, _state);
            for(i=0; i<=mx-1; i++)
            {
                for(j=0; j<=mx-1; j++)
                {
                    ra.ptr.pp_double[i][j] = (double)(0);
                    ca.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                }
            }
            testortfacunit_testrtdproblem(&ra, mx, threshold, &rtderrors, _state);
            testortfacunit_testctdproblem(&ca, mx, threshold, &ctderrors, _state);
            for(i=0; i<=mx-1; i++)
            {
                for(j=i; j<=mx-1; j++)
                {
                    ra.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    ca.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                    ca.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                    ra.ptr.pp_double[j][i] = ra.ptr.pp_double[i][j];
                    ca.ptr.pp_complex[j][i] = ae_c_conj(ca.ptr.pp_complex[i][j], _state);
                }
            }
            for(i=0; i<=mx-1; i++)
            {
                ca.ptr.pp_complex[i][i] = ae_complex_from_d(2*ae_randomreal(_state)-1);
            }
            testortfacunit_testrtdproblem(&ra, mx, threshold, &rtderrors, _state);
            testortfacunit_testctdproblem(&ca, mx, threshold, &ctderrors, _state);
            testortfacunit_rmatrixfillsparsea(&ra, mx, mx, 0.95, _state);
            testortfacunit_cmatrixfillsparsea(&ca, mx, mx, 0.95, _state);
            for(i=0; i<=mx-1; i++)
            {
                for(j=i; j<=mx-1; j++)
                {
                    ra.ptr.pp_double[j][i] = ra.ptr.pp_double[i][j];
                    ca.ptr.pp_complex[j][i] = ae_c_conj(ca.ptr.pp_complex[i][j], _state);
                }
            }
            for(i=0; i<=mx-1; i++)
            {
                ca.ptr.pp_complex[i][i] = ae_complex_from_d(2*ae_randomreal(_state)-1);
            }
            testortfacunit_testrtdproblem(&ra, mx, threshold, &rtderrors, _state);
            testortfacunit_testctdproblem(&ca, mx, threshold, &ctderrors, _state);
        }
    }
    
    /*
     * report
     */
    waserrors = ((((((rqrerrors||rlqerrors)||cqrerrors)||clqerrors)||rbderrors)||rhesserrors)||rtderrors)||ctderrors;
    if( !silent )
    {
        printf("TESTING ORTFAC UNIT\n");
        printf("RQR ERRORS:                              ");
        if( !rqrerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("RLQ ERRORS:                              ");
        if( !rlqerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("CQR ERRORS:                              ");
        if( !cqrerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("CLQ ERRORS:                              ");
        if( !clqerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("RBD ERRORS:                              ");
        if( !rbderrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("RHESS ERRORS:                            ");
        if( !rhesserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("RTD ERRORS:                              ");
        if( !rtderrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("CTD ERRORS:                              ");
        if( !ctderrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
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
ae_bool _pexec_testortfac(ae_bool silent, ae_state *_state)
{
    return testortfac(silent, _state);
}


/*************************************************************************
Diff
*************************************************************************/
static double testortfacunit_rmatrixdiff(/* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double result;


    result = (double)(0);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            result = ae_maxreal(result, ae_fabs(b->ptr.pp_double[i][j]-a->ptr.pp_double[i][j], _state), _state);
        }
    }
    return result;
}


/*************************************************************************
Copy
*************************************************************************/
static void testortfacunit_rmatrixmakeacopy(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(b);

    ae_matrix_set_length(b, m-1+1, n-1+1, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            b->ptr.pp_double[i][j] = a->ptr.pp_double[i][j];
        }
    }
}


/*************************************************************************
Copy
*************************************************************************/
static void testortfacunit_cmatrixmakeacopy(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* b,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(b);

    ae_matrix_set_length(b, m-1+1, n-1+1, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            b->ptr.pp_complex[i][j] = a->ptr.pp_complex[i][j];
        }
    }
}


/*************************************************************************
Sparse fill
*************************************************************************/
static void testortfacunit_rmatrixfillsparsea(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double sparcity,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( ae_fp_greater_eq(ae_randomreal(_state),sparcity) )
            {
                a->ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            else
            {
                a->ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
}


/*************************************************************************
Sparse fill
*************************************************************************/
static void testortfacunit_cmatrixfillsparsea(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double sparcity,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( ae_fp_greater_eq(ae_randomreal(_state),sparcity) )
            {
                a->ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                a->ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
            }
            else
            {
                a->ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
    }
}


/*************************************************************************
Matrix multiplication
*************************************************************************/
static void testortfacunit_internalmatrixmatrixmultiply(/* Real    */ ae_matrix* a,
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
     /* Real    */ ae_matrix* c,
     ae_int_t ci1,
     ae_int_t ci2,
     ae_int_t cj1,
     ae_int_t cj2,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t arows;
    ae_int_t acols;
    ae_int_t brows;
    ae_int_t bcols;
    ae_int_t crows;
    ae_int_t ccols;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t l;
    ae_int_t r;
    double v;
    ae_vector work;
    double beta;
    double alpha;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&work, 0, DT_REAL, _state);

    
    /*
     * Pre-setup
     */
    k = ae_maxint(ai2-ai1+1, aj2-aj1+1, _state);
    k = ae_maxint(k, bi2-bi1+1, _state);
    k = ae_maxint(k, bj2-bj1+1, _state);
    ae_vector_set_length(&work, k+1, _state);
    beta = (double)(0);
    alpha = (double)(1);
    
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
        ae_frame_leave(_state);
        return;
    }
    crows = arows;
    ccols = bcols;
    
    /*
     * Test WORK
     */
    i = ae_maxint(arows, acols, _state);
    i = ae_maxint(brows, i, _state);
    i = ae_maxint(i, bcols, _state);
    work.ptr.p_double[1] = (double)(0);
    work.ptr.p_double[i] = (double)(0);
    
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
        ae_frame_leave(_state);
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
            ae_frame_leave(_state);
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
            ae_frame_leave(_state);
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
        ae_frame_leave(_state);
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
                for(i=1; i<=crows; i++)
                {
                    work.ptr.p_double[i] = 0.0;
                }
                for(l=ai1; l<=ai2; l++)
                {
                    v = alpha*b->ptr.pp_double[r][bj1+l-ai1];
                    k = cj1+r-bi1;
                    ae_v_addd(&work.ptr.p_double[1], 1, &a->ptr.pp_double[l][aj1], 1, ae_v_len(1,crows), v);
                }
                ae_v_add(&c->ptr.pp_double[ci1][k], c->stride, &work.ptr.p_double[1], 1, ae_v_len(ci1,ci2));
            }
            ae_frame_leave(_state);
            return;
        }
        else
        {
            for(l=aj1; l<=aj2; l++)
            {
                k = ai2-ai1+1;
                ae_v_move(&work.ptr.p_double[1], 1, &a->ptr.pp_double[ai1][l], a->stride, ae_v_len(1,k));
                for(r=bi1; r<=bi2; r++)
                {
                    v = ae_v_dotproduct(&work.ptr.p_double[1], 1, &b->ptr.pp_double[r][bj1], 1, ae_v_len(1,k));
                    c->ptr.pp_double[ci1+l-aj1][cj1+r-bi1] = c->ptr.pp_double[ci1+l-aj1][cj1+r-bi1]+alpha*v;
                }
            }
            ae_frame_leave(_state);
            return;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Problem testing
*************************************************************************/
static void testortfacunit_testrqrproblem(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* qrerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_matrix b;
    ae_vector taub;
    ae_matrix q;
    ae_matrix r;
    ae_matrix q2;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&b, 0, 0, DT_REAL, _state);
    ae_vector_init(&taub, 0, DT_REAL, _state);
    ae_matrix_init(&q, 0, 0, DT_REAL, _state);
    ae_matrix_init(&r, 0, 0, DT_REAL, _state);
    ae_matrix_init(&q2, 0, 0, DT_REAL, _state);

    
    /*
     * Test decompose-and-unpack error
     */
    testortfacunit_rmatrixmakeacopy(a, m, n, &b, _state);
    rmatrixqr(&b, m, n, &taub, _state);
    rmatrixqrunpackq(&b, m, n, &taub, m, &q, _state);
    rmatrixqrunpackr(&b, m, n, &r, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &r.ptr.pp_double[0][j], r.stride, ae_v_len(0,m-1));
            *qrerrors = *qrerrors||ae_fp_greater(ae_fabs(v-a->ptr.pp_double[i][j], _state),threshold);
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=ae_minint(i, n-1, _state)-1; j++)
        {
            *qrerrors = *qrerrors||ae_fp_neq(r.ptr.pp_double[i][j],(double)(0));
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &q.ptr.pp_double[j][0], 1, ae_v_len(0,m-1));
            if( i==j )
            {
                v = v-1;
            }
            *qrerrors = *qrerrors||ae_fp_greater_eq(ae_fabs(v, _state),threshold);
        }
    }
    
    /*
     * Test for other errors
     */
    k = 1+ae_randominteger(m, _state);
    rmatrixqrunpackq(&b, m, n, &taub, k, &q2, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=k-1; j++)
        {
            *qrerrors = *qrerrors||ae_fp_greater(ae_fabs(q2.ptr.pp_double[i][j]-q.ptr.pp_double[i][j], _state),10*ae_machineepsilon);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Problem testing
*************************************************************************/
static void testortfacunit_testcqrproblem(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* qrerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_matrix b;
    ae_vector taub;
    ae_matrix q;
    ae_matrix r;
    ae_matrix q2;
    ae_complex v;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&b, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&taub, 0, DT_COMPLEX, _state);
    ae_matrix_init(&q, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&r, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&q2, 0, 0, DT_COMPLEX, _state);

    
    /*
     * Test decompose-and-unpack error
     */
    testortfacunit_cmatrixmakeacopy(a, m, n, &b, _state);
    cmatrixqr(&b, m, n, &taub, _state);
    cmatrixqrunpackq(&b, m, n, &taub, m, &q, _state);
    cmatrixqrunpackr(&b, m, n, &r, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&q.ptr.pp_complex[i][0], 1, "N", &r.ptr.pp_complex[0][j], r.stride, "N", ae_v_len(0,m-1));
            *qrerrors = *qrerrors||ae_fp_greater(ae_c_abs(ae_c_sub(v,a->ptr.pp_complex[i][j]), _state),threshold);
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=ae_minint(i, n-1, _state)-1; j++)
        {
            *qrerrors = *qrerrors||ae_c_neq_d(r.ptr.pp_complex[i][j],(double)(0));
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_cdotproduct(&q.ptr.pp_complex[i][0], 1, "N", &q.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,m-1));
            if( i==j )
            {
                v = ae_c_sub_d(v,1);
            }
            *qrerrors = *qrerrors||ae_fp_greater_eq(ae_c_abs(v, _state),threshold);
        }
    }
    
    /*
     * Test for other errors
     */
    k = 1+ae_randominteger(m, _state);
    cmatrixqrunpackq(&b, m, n, &taub, k, &q2, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=k-1; j++)
        {
            *qrerrors = *qrerrors||ae_fp_greater(ae_c_abs(ae_c_sub(q2.ptr.pp_complex[i][j],q.ptr.pp_complex[i][j]), _state),10*ae_machineepsilon);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Problem testing
*************************************************************************/
static void testortfacunit_testrlqproblem(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* lqerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_matrix b;
    ae_vector taub;
    ae_matrix q;
    ae_matrix l;
    ae_matrix q2;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&b, 0, 0, DT_REAL, _state);
    ae_vector_init(&taub, 0, DT_REAL, _state);
    ae_matrix_init(&q, 0, 0, DT_REAL, _state);
    ae_matrix_init(&l, 0, 0, DT_REAL, _state);
    ae_matrix_init(&q2, 0, 0, DT_REAL, _state);

    
    /*
     * Test decompose-and-unpack error
     */
    testortfacunit_rmatrixmakeacopy(a, m, n, &b, _state);
    rmatrixlq(&b, m, n, &taub, _state);
    rmatrixlqunpackq(&b, m, n, &taub, n, &q, _state);
    rmatrixlqunpackl(&b, m, n, &l, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&l.ptr.pp_double[i][0], 1, &q.ptr.pp_double[0][j], q.stride, ae_v_len(0,n-1));
            *lqerrors = *lqerrors||ae_fp_greater_eq(ae_fabs(v-a->ptr.pp_double[i][j], _state),threshold);
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=ae_minint(i, n-1, _state)+1; j<=n-1; j++)
        {
            *lqerrors = *lqerrors||ae_fp_neq(l.ptr.pp_double[i][j],(double)(0));
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &q.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
            if( i==j )
            {
                v = v-1;
            }
            *lqerrors = *lqerrors||ae_fp_greater_eq(ae_fabs(v, _state),threshold);
        }
    }
    
    /*
     * Test for other errors
     */
    k = 1+ae_randominteger(n, _state);
    rmatrixlqunpackq(&b, m, n, &taub, k, &q2, _state);
    for(i=0; i<=k-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            *lqerrors = *lqerrors||ae_fp_greater(ae_fabs(q2.ptr.pp_double[i][j]-q.ptr.pp_double[i][j], _state),10*ae_machineepsilon);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Problem testing
*************************************************************************/
static void testortfacunit_testclqproblem(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* lqerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_matrix b;
    ae_vector taub;
    ae_matrix q;
    ae_matrix l;
    ae_matrix q2;
    ae_complex v;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&b, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&taub, 0, DT_COMPLEX, _state);
    ae_matrix_init(&q, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&l, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&q2, 0, 0, DT_COMPLEX, _state);

    
    /*
     * Test decompose-and-unpack error
     */
    testortfacunit_cmatrixmakeacopy(a, m, n, &b, _state);
    cmatrixlq(&b, m, n, &taub, _state);
    cmatrixlqunpackq(&b, m, n, &taub, n, &q, _state);
    cmatrixlqunpackl(&b, m, n, &l, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&l.ptr.pp_complex[i][0], 1, "N", &q.ptr.pp_complex[0][j], q.stride, "N", ae_v_len(0,n-1));
            *lqerrors = *lqerrors||ae_fp_greater_eq(ae_c_abs(ae_c_sub(v,a->ptr.pp_complex[i][j]), _state),threshold);
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=ae_minint(i, n-1, _state)+1; j<=n-1; j++)
        {
            *lqerrors = *lqerrors||ae_c_neq_d(l.ptr.pp_complex[i][j],(double)(0));
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&q.ptr.pp_complex[i][0], 1, "N", &q.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,n-1));
            if( i==j )
            {
                v = ae_c_sub_d(v,1);
            }
            *lqerrors = *lqerrors||ae_fp_greater_eq(ae_c_abs(v, _state),threshold);
        }
    }
    
    /*
     * Test for other errors
     */
    k = 1+ae_randominteger(n, _state);
    cmatrixlqunpackq(&b, m, n, &taub, k, &q2, _state);
    for(i=0; i<=k-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            *lqerrors = *lqerrors||ae_fp_greater(ae_c_abs(ae_c_sub(q2.ptr.pp_complex[i][j],q.ptr.pp_complex[i][j]), _state),10*ae_machineepsilon);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Problem testing
*************************************************************************/
static void testortfacunit_testrbdproblem(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* bderrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_matrix t;
    ae_matrix pt;
    ae_matrix q;
    ae_matrix r;
    ae_matrix bd;
    ae_matrix x;
    ae_matrix r1;
    ae_matrix r2;
    ae_vector taup;
    ae_vector tauq;
    ae_vector d;
    ae_vector e;
    ae_bool up;
    double v;
    ae_int_t mtsize;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&t, 0, 0, DT_REAL, _state);
    ae_matrix_init(&pt, 0, 0, DT_REAL, _state);
    ae_matrix_init(&q, 0, 0, DT_REAL, _state);
    ae_matrix_init(&r, 0, 0, DT_REAL, _state);
    ae_matrix_init(&bd, 0, 0, DT_REAL, _state);
    ae_matrix_init(&x, 0, 0, DT_REAL, _state);
    ae_matrix_init(&r1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&r2, 0, 0, DT_REAL, _state);
    ae_vector_init(&taup, 0, DT_REAL, _state);
    ae_vector_init(&tauq, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);

    
    /*
     * Bidiagonal decomposition error
     */
    testortfacunit_rmatrixmakeacopy(a, m, n, &t, _state);
    rmatrixbd(&t, m, n, &tauq, &taup, _state);
    rmatrixbdunpackq(&t, m, n, &tauq, m, &q, _state);
    rmatrixbdunpackpt(&t, m, n, &taup, n, &pt, _state);
    rmatrixbdunpackdiagonals(&t, m, n, &up, &d, &e, _state);
    ae_matrix_set_length(&bd, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            bd.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(i=0; i<=ae_minint(m, n, _state)-1; i++)
    {
        bd.ptr.pp_double[i][i] = d.ptr.p_double[i];
    }
    if( up )
    {
        for(i=0; i<=ae_minint(m, n, _state)-2; i++)
        {
            bd.ptr.pp_double[i][i+1] = e.ptr.p_double[i];
        }
    }
    else
    {
        for(i=0; i<=ae_minint(m, n, _state)-2; i++)
        {
            bd.ptr.pp_double[i+1][i] = e.ptr.p_double[i];
        }
    }
    ae_matrix_set_length(&r, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &bd.ptr.pp_double[0][j], bd.stride, ae_v_len(0,m-1));
            r.ptr.pp_double[i][j] = v;
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&r.ptr.pp_double[i][0], 1, &pt.ptr.pp_double[0][j], pt.stride, ae_v_len(0,n-1));
            *bderrors = *bderrors||ae_fp_greater(ae_fabs(v-a->ptr.pp_double[i][j], _state),threshold);
        }
    }
    
    /*
     * Orthogonality test for Q/PT
     */
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            v = ae_v_dotproduct(&q.ptr.pp_double[0][i], q.stride, &q.ptr.pp_double[0][j], q.stride, ae_v_len(0,m-1));
            if( i==j )
            {
                *bderrors = *bderrors||ae_fp_greater(ae_fabs(v-1, _state),threshold);
            }
            else
            {
                *bderrors = *bderrors||ae_fp_greater(ae_fabs(v, _state),threshold);
            }
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&pt.ptr.pp_double[i][0], 1, &pt.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
            if( i==j )
            {
                *bderrors = *bderrors||ae_fp_greater(ae_fabs(v-1, _state),threshold);
            }
            else
            {
                *bderrors = *bderrors||ae_fp_greater(ae_fabs(v, _state),threshold);
            }
        }
    }
    
    /*
     * Partial unpacking test
     */
    k = 1+ae_randominteger(m, _state);
    rmatrixbdunpackq(&t, m, n, &tauq, k, &r, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=k-1; j++)
        {
            *bderrors = *bderrors||ae_fp_greater(ae_fabs(r.ptr.pp_double[i][j]-q.ptr.pp_double[i][j], _state),10*ae_machineepsilon);
        }
    }
    k = 1+ae_randominteger(n, _state);
    rmatrixbdunpackpt(&t, m, n, &taup, k, &r, _state);
    for(i=0; i<=k-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            *bderrors = *bderrors||ae_fp_neq(r.ptr.pp_double[i][j]-pt.ptr.pp_double[i][j],(double)(0));
        }
    }
    
    /*
     * Multiplication test
     */
    ae_matrix_set_length(&x, ae_maxint(m, n, _state)-1+1, ae_maxint(m, n, _state)-1+1, _state);
    ae_matrix_set_length(&r, ae_maxint(m, n, _state)-1+1, ae_maxint(m, n, _state)-1+1, _state);
    ae_matrix_set_length(&r1, ae_maxint(m, n, _state)-1+1, ae_maxint(m, n, _state)-1+1, _state);
    ae_matrix_set_length(&r2, ae_maxint(m, n, _state)-1+1, ae_maxint(m, n, _state)-1+1, _state);
    for(i=0; i<=ae_maxint(m, n, _state)-1; i++)
    {
        for(j=0; j<=ae_maxint(m, n, _state)-1; j++)
        {
            x.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
        }
    }
    mtsize = 1+ae_randominteger(ae_maxint(m, n, _state), _state);
    testortfacunit_rmatrixmakeacopy(&x, mtsize, m, &r, _state);
    testortfacunit_internalmatrixmatrixmultiply(&r, 0, mtsize-1, 0, m-1, ae_false, &q, 0, m-1, 0, m-1, ae_false, &r1, 0, mtsize-1, 0, m-1, _state);
    testortfacunit_rmatrixmakeacopy(&x, mtsize, m, &r2, _state);
    rmatrixbdmultiplybyq(&t, m, n, &tauq, &r2, mtsize, m, ae_true, ae_false, _state);
    *bderrors = *bderrors||ae_fp_greater(testortfacunit_rmatrixdiff(&r1, &r2, mtsize, m, _state),threshold);
    testortfacunit_rmatrixmakeacopy(&x, mtsize, m, &r, _state);
    testortfacunit_internalmatrixmatrixmultiply(&r, 0, mtsize-1, 0, m-1, ae_false, &q, 0, m-1, 0, m-1, ae_true, &r1, 0, mtsize-1, 0, m-1, _state);
    testortfacunit_rmatrixmakeacopy(&x, mtsize, m, &r2, _state);
    rmatrixbdmultiplybyq(&t, m, n, &tauq, &r2, mtsize, m, ae_true, ae_true, _state);
    *bderrors = *bderrors||ae_fp_greater(testortfacunit_rmatrixdiff(&r1, &r2, mtsize, m, _state),threshold);
    testortfacunit_rmatrixmakeacopy(&x, m, mtsize, &r, _state);
    testortfacunit_internalmatrixmatrixmultiply(&q, 0, m-1, 0, m-1, ae_false, &r, 0, m-1, 0, mtsize-1, ae_false, &r1, 0, m-1, 0, mtsize-1, _state);
    testortfacunit_rmatrixmakeacopy(&x, m, mtsize, &r2, _state);
    rmatrixbdmultiplybyq(&t, m, n, &tauq, &r2, m, mtsize, ae_false, ae_false, _state);
    *bderrors = *bderrors||ae_fp_greater(testortfacunit_rmatrixdiff(&r1, &r2, m, mtsize, _state),threshold);
    testortfacunit_rmatrixmakeacopy(&x, m, mtsize, &r, _state);
    testortfacunit_internalmatrixmatrixmultiply(&q, 0, m-1, 0, m-1, ae_true, &r, 0, m-1, 0, mtsize-1, ae_false, &r1, 0, m-1, 0, mtsize-1, _state);
    testortfacunit_rmatrixmakeacopy(&x, m, mtsize, &r2, _state);
    rmatrixbdmultiplybyq(&t, m, n, &tauq, &r2, m, mtsize, ae_false, ae_true, _state);
    *bderrors = *bderrors||ae_fp_greater(testortfacunit_rmatrixdiff(&r1, &r2, m, mtsize, _state),threshold);
    testortfacunit_rmatrixmakeacopy(&x, mtsize, n, &r, _state);
    testortfacunit_internalmatrixmatrixmultiply(&r, 0, mtsize-1, 0, n-1, ae_false, &pt, 0, n-1, 0, n-1, ae_true, &r1, 0, mtsize-1, 0, n-1, _state);
    testortfacunit_rmatrixmakeacopy(&x, mtsize, n, &r2, _state);
    rmatrixbdmultiplybyp(&t, m, n, &taup, &r2, mtsize, n, ae_true, ae_false, _state);
    *bderrors = *bderrors||ae_fp_greater(testortfacunit_rmatrixdiff(&r1, &r2, mtsize, n, _state),threshold);
    testortfacunit_rmatrixmakeacopy(&x, mtsize, n, &r, _state);
    testortfacunit_internalmatrixmatrixmultiply(&r, 0, mtsize-1, 0, n-1, ae_false, &pt, 0, n-1, 0, n-1, ae_false, &r1, 0, mtsize-1, 0, n-1, _state);
    testortfacunit_rmatrixmakeacopy(&x, mtsize, n, &r2, _state);
    rmatrixbdmultiplybyp(&t, m, n, &taup, &r2, mtsize, n, ae_true, ae_true, _state);
    *bderrors = *bderrors||ae_fp_greater(testortfacunit_rmatrixdiff(&r1, &r2, mtsize, n, _state),threshold);
    testortfacunit_rmatrixmakeacopy(&x, n, mtsize, &r, _state);
    testortfacunit_internalmatrixmatrixmultiply(&pt, 0, n-1, 0, n-1, ae_true, &r, 0, n-1, 0, mtsize-1, ae_false, &r1, 0, n-1, 0, mtsize-1, _state);
    testortfacunit_rmatrixmakeacopy(&x, n, mtsize, &r2, _state);
    rmatrixbdmultiplybyp(&t, m, n, &taup, &r2, n, mtsize, ae_false, ae_false, _state);
    *bderrors = *bderrors||ae_fp_greater(testortfacunit_rmatrixdiff(&r1, &r2, n, mtsize, _state),threshold);
    testortfacunit_rmatrixmakeacopy(&x, n, mtsize, &r, _state);
    testortfacunit_internalmatrixmatrixmultiply(&pt, 0, n-1, 0, n-1, ae_false, &r, 0, n-1, 0, mtsize-1, ae_false, &r1, 0, n-1, 0, mtsize-1, _state);
    testortfacunit_rmatrixmakeacopy(&x, n, mtsize, &r2, _state);
    rmatrixbdmultiplybyp(&t, m, n, &taup, &r2, n, mtsize, ae_false, ae_true, _state);
    *bderrors = *bderrors||ae_fp_greater(testortfacunit_rmatrixdiff(&r1, &r2, n, mtsize, _state),threshold);
    ae_frame_leave(_state);
}


/*************************************************************************
Problem testing
*************************************************************************/
static void testortfacunit_testrhessproblem(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double threshold,
     ae_bool* hesserrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix b;
    ae_matrix h;
    ae_matrix q;
    ae_matrix t1;
    ae_matrix t2;
    ae_vector tau;
    ae_int_t i;
    ae_int_t j;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&b, 0, 0, DT_REAL, _state);
    ae_matrix_init(&h, 0, 0, DT_REAL, _state);
    ae_matrix_init(&q, 0, 0, DT_REAL, _state);
    ae_matrix_init(&t1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&t2, 0, 0, DT_REAL, _state);
    ae_vector_init(&tau, 0, DT_REAL, _state);

    testortfacunit_rmatrixmakeacopy(a, n, n, &b, _state);
    
    /*
     * Decomposition
     */
    rmatrixhessenberg(&b, n, &tau, _state);
    rmatrixhessenbergunpackq(&b, n, &tau, &q, _state);
    rmatrixhessenbergunpackh(&b, n, &h, _state);
    
    /*
     * Matrix properties
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&q.ptr.pp_double[0][i], q.stride, &q.ptr.pp_double[0][j], q.stride, ae_v_len(0,n-1));
            if( i==j )
            {
                v = v-1;
            }
            *hesserrors = *hesserrors||ae_fp_greater(ae_fabs(v, _state),threshold);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=i-2; j++)
        {
            *hesserrors = *hesserrors||ae_fp_neq(h.ptr.pp_double[i][j],(double)(0));
        }
    }
    
    /*
     * Decomposition error
     */
    ae_matrix_set_length(&t1, n, n, _state);
    ae_matrix_set_length(&t2, n, n, _state);
    testortfacunit_internalmatrixmatrixmultiply(&q, 0, n-1, 0, n-1, ae_false, &h, 0, n-1, 0, n-1, ae_false, &t1, 0, n-1, 0, n-1, _state);
    testortfacunit_internalmatrixmatrixmultiply(&t1, 0, n-1, 0, n-1, ae_false, &q, 0, n-1, 0, n-1, ae_true, &t2, 0, n-1, 0, n-1, _state);
    *hesserrors = *hesserrors||ae_fp_greater(testortfacunit_rmatrixdiff(&t2, a, n, n, _state),threshold);
    ae_frame_leave(_state);
}


/*************************************************************************
Tridiagonal tester
*************************************************************************/
static void testortfacunit_testrtdproblem(/* Real    */ ae_matrix* a,
     ae_int_t n,
     double threshold,
     ae_bool* tderrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_matrix ua;
    ae_matrix la;
    ae_matrix t;
    ae_matrix q;
    ae_matrix t2;
    ae_matrix t3;
    ae_vector tau;
    ae_vector d;
    ae_vector e;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ua, 0, 0, DT_REAL, _state);
    ae_matrix_init(&la, 0, 0, DT_REAL, _state);
    ae_matrix_init(&t, 0, 0, DT_REAL, _state);
    ae_matrix_init(&q, 0, 0, DT_REAL, _state);
    ae_matrix_init(&t2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&t3, 0, 0, DT_REAL, _state);
    ae_vector_init(&tau, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);

    ae_matrix_set_length(&ua, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&la, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&t, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&q, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&t2, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&t3, n-1+1, n-1+1, _state);
    
    /*
     * fill
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            ua.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=i; j<=n-1; j++)
        {
            ua.ptr.pp_double[i][j] = a->ptr.pp_double[i][j];
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            la.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=i; j++)
        {
            la.ptr.pp_double[i][j] = a->ptr.pp_double[i][j];
        }
    }
    
    /*
     * Test 2tridiagonal: upper
     */
    smatrixtd(&ua, n, ae_true, &tau, &d, &e, _state);
    smatrixtdunpackq(&ua, n, ae_true, &tau, &q, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            t.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        t.ptr.pp_double[i][i] = d.ptr.p_double[i];
    }
    for(i=0; i<=n-2; i++)
    {
        t.ptr.pp_double[i][i+1] = e.ptr.p_double[i];
        t.ptr.pp_double[i+1][i] = e.ptr.p_double[i];
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&q.ptr.pp_double[0][i], q.stride, &a->ptr.pp_double[0][j], a->stride, ae_v_len(0,n-1));
            t2.ptr.pp_double[i][j] = v;
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&t2.ptr.pp_double[i][0], 1, &q.ptr.pp_double[0][j], q.stride, ae_v_len(0,n-1));
            t3.ptr.pp_double[i][j] = v;
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            *tderrors = *tderrors||ae_fp_greater(ae_fabs(t3.ptr.pp_double[i][j]-t.ptr.pp_double[i][j], _state),threshold);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &q.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
            if( i==j )
            {
                v = v-1;
            }
            *tderrors = *tderrors||ae_fp_greater(ae_fabs(v, _state),threshold);
        }
    }
    
    /*
     * Test 2tridiagonal: lower
     */
    smatrixtd(&la, n, ae_false, &tau, &d, &e, _state);
    smatrixtdunpackq(&la, n, ae_false, &tau, &q, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            t.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        t.ptr.pp_double[i][i] = d.ptr.p_double[i];
    }
    for(i=0; i<=n-2; i++)
    {
        t.ptr.pp_double[i][i+1] = e.ptr.p_double[i];
        t.ptr.pp_double[i+1][i] = e.ptr.p_double[i];
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&q.ptr.pp_double[0][i], q.stride, &a->ptr.pp_double[0][j], a->stride, ae_v_len(0,n-1));
            t2.ptr.pp_double[i][j] = v;
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&t2.ptr.pp_double[i][0], 1, &q.ptr.pp_double[0][j], q.stride, ae_v_len(0,n-1));
            t3.ptr.pp_double[i][j] = v;
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            *tderrors = *tderrors||ae_fp_greater(ae_fabs(t3.ptr.pp_double[i][j]-t.ptr.pp_double[i][j], _state),threshold);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &q.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
            if( i==j )
            {
                v = v-1;
            }
            *tderrors = *tderrors||ae_fp_greater(ae_fabs(v, _state),threshold);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Hermitian problem tester
*************************************************************************/
static void testortfacunit_testctdproblem(/* Complex */ ae_matrix* a,
     ae_int_t n,
     double threshold,
     ae_bool* tderrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_matrix ua;
    ae_matrix la;
    ae_matrix t;
    ae_matrix q;
    ae_matrix t2;
    ae_matrix t3;
    ae_vector tau;
    ae_vector d;
    ae_vector e;
    ae_complex v;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ua, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&la, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&t, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&q, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&t2, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&t3, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&tau, 0, DT_COMPLEX, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&e, 0, DT_REAL, _state);

    ae_matrix_set_length(&ua, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&la, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&t, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&q, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&t2, n-1+1, n-1+1, _state);
    ae_matrix_set_length(&t3, n-1+1, n-1+1, _state);
    
    /*
     * fill
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            ua.ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=i; j<=n-1; j++)
        {
            ua.ptr.pp_complex[i][j] = a->ptr.pp_complex[i][j];
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            la.ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=i; j++)
        {
            la.ptr.pp_complex[i][j] = a->ptr.pp_complex[i][j];
        }
    }
    
    /*
     * Test 2tridiagonal: upper
     */
    hmatrixtd(&ua, n, ae_true, &tau, &d, &e, _state);
    hmatrixtdunpackq(&ua, n, ae_true, &tau, &q, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            t.ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        t.ptr.pp_complex[i][i] = ae_complex_from_d(d.ptr.p_double[i]);
    }
    for(i=0; i<=n-2; i++)
    {
        t.ptr.pp_complex[i][i+1] = ae_complex_from_d(e.ptr.p_double[i]);
        t.ptr.pp_complex[i+1][i] = ae_complex_from_d(e.ptr.p_double[i]);
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&q.ptr.pp_complex[0][i], q.stride, "Conj", &a->ptr.pp_complex[0][j], a->stride, "N", ae_v_len(0,n-1));
            t2.ptr.pp_complex[i][j] = v;
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&t2.ptr.pp_complex[i][0], 1, "N", &q.ptr.pp_complex[0][j], q.stride, "N", ae_v_len(0,n-1));
            t3.ptr.pp_complex[i][j] = v;
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            *tderrors = *tderrors||ae_fp_greater(ae_c_abs(ae_c_sub(t3.ptr.pp_complex[i][j],t.ptr.pp_complex[i][j]), _state),threshold);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&q.ptr.pp_complex[i][0], 1, "N", &q.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,n-1));
            if( i==j )
            {
                v = ae_c_sub_d(v,1);
            }
            *tderrors = *tderrors||ae_fp_greater(ae_c_abs(v, _state),threshold);
        }
    }
    
    /*
     * Test 2tridiagonal: lower
     */
    hmatrixtd(&la, n, ae_false, &tau, &d, &e, _state);
    hmatrixtdunpackq(&la, n, ae_false, &tau, &q, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            t.ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        t.ptr.pp_complex[i][i] = ae_complex_from_d(d.ptr.p_double[i]);
    }
    for(i=0; i<=n-2; i++)
    {
        t.ptr.pp_complex[i][i+1] = ae_complex_from_d(e.ptr.p_double[i]);
        t.ptr.pp_complex[i+1][i] = ae_complex_from_d(e.ptr.p_double[i]);
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&q.ptr.pp_complex[0][i], q.stride, "Conj", &a->ptr.pp_complex[0][j], a->stride, "N", ae_v_len(0,n-1));
            t2.ptr.pp_complex[i][j] = v;
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&t2.ptr.pp_complex[i][0], 1, "N", &q.ptr.pp_complex[0][j], q.stride, "N", ae_v_len(0,n-1));
            t3.ptr.pp_complex[i][j] = v;
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            *tderrors = *tderrors||ae_fp_greater(ae_c_abs(ae_c_sub(t3.ptr.pp_complex[i][j],t.ptr.pp_complex[i][j]), _state),threshold);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&q.ptr.pp_complex[i][0], 1, "N", &q.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,n-1));
            if( i==j )
            {
                v = ae_c_sub_d(v,1);
            }
            *tderrors = *tderrors||ae_fp_greater(ae_c_abs(v, _state),threshold);
        }
    }
    ae_frame_leave(_state);
}


/*$ End $*/
