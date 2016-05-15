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
#include "testinverseupdateunit.h"


/*$ Declarations $*/
static void testinverseupdateunit_makeacopy(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* b,
     ae_state *_state);
static void testinverseupdateunit_matlu(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state);
static void testinverseupdateunit_generaterandomorthogonalmatrix(/* Real    */ ae_matrix* a0,
     ae_int_t n,
     ae_state *_state);
static void testinverseupdateunit_generaterandommatrixcond(/* Real    */ ae_matrix* a0,
     ae_int_t n,
     double c,
     ae_state *_state);
static ae_bool testinverseupdateunit_invmattr(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunittriangular,
     ae_state *_state);
static ae_bool testinverseupdateunit_invmatlu(/* Real    */ ae_matrix* a,
     /* Integer */ ae_vector* pivots,
     ae_int_t n,
     ae_state *_state);
static ae_bool testinverseupdateunit_invmat(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state);
static double testinverseupdateunit_matrixdiff(/* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* b,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state);
static ae_bool testinverseupdateunit_updandinv(/* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* u,
     /* Real    */ ae_vector* v,
     ae_int_t n,
     ae_state *_state);


/*$ Body $*/


ae_bool testinverseupdate(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix inva;
    ae_matrix b1;
    ae_matrix b2;
    ae_vector u;
    ae_vector v;
    ae_int_t n;
    ae_int_t maxn;
    ae_int_t i;
    ae_int_t updrow;
    ae_int_t updcol;
    double val;
    ae_int_t pass;
    ae_int_t passcount;
    ae_bool waserrors;
    double threshold;
    double c;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&inva, 0, 0, DT_REAL, _state);
    ae_matrix_init(&b1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&b2, 0, 0, DT_REAL, _state);
    ae_vector_init(&u, 0, DT_REAL, _state);
    ae_vector_init(&v, 0, DT_REAL, _state);

    waserrors = ae_false;
    maxn = 10;
    passcount = 100;
    threshold = 1.0E-6;
    
    /*
     * process
     */
    for(n=1; n<=maxn; n++)
    {
        ae_matrix_set_length(&a, n-1+1, n-1+1, _state);
        ae_matrix_set_length(&b1, n-1+1, n-1+1, _state);
        ae_matrix_set_length(&b2, n-1+1, n-1+1, _state);
        ae_vector_set_length(&u, n-1+1, _state);
        ae_vector_set_length(&v, n-1+1, _state);
        for(pass=1; pass<=passcount; pass++)
        {
            c = ae_exp(ae_randomreal(_state)*ae_log((double)(10), _state), _state);
            testinverseupdateunit_generaterandommatrixcond(&a, n, c, _state);
            testinverseupdateunit_makeacopy(&a, n, n, &inva, _state);
            if( !testinverseupdateunit_invmat(&inva, n, _state) )
            {
                waserrors = ae_true;
                break;
            }
            
            /*
             * Test simple update
             */
            updrow = ae_randominteger(n, _state);
            updcol = ae_randominteger(n, _state);
            val = 0.1*(2*ae_randomreal(_state)-1);
            for(i=0; i<=n-1; i++)
            {
                if( i==updrow )
                {
                    u.ptr.p_double[i] = val;
                }
                else
                {
                    u.ptr.p_double[i] = (double)(0);
                }
                if( i==updcol )
                {
                    v.ptr.p_double[i] = (double)(1);
                }
                else
                {
                    v.ptr.p_double[i] = (double)(0);
                }
            }
            testinverseupdateunit_makeacopy(&a, n, n, &b1, _state);
            if( !testinverseupdateunit_updandinv(&b1, &u, &v, n, _state) )
            {
                waserrors = ae_true;
                break;
            }
            testinverseupdateunit_makeacopy(&inva, n, n, &b2, _state);
            rmatrixinvupdatesimple(&b2, n, updrow, updcol, val, _state);
            waserrors = waserrors||ae_fp_greater(testinverseupdateunit_matrixdiff(&b1, &b2, n, n, _state),threshold);
            
            /*
             * Test row update
             */
            updrow = ae_randominteger(n, _state);
            for(i=0; i<=n-1; i++)
            {
                if( i==updrow )
                {
                    u.ptr.p_double[i] = (double)(1);
                }
                else
                {
                    u.ptr.p_double[i] = (double)(0);
                }
                v.ptr.p_double[i] = 0.1*(2*ae_randomreal(_state)-1);
            }
            testinverseupdateunit_makeacopy(&a, n, n, &b1, _state);
            if( !testinverseupdateunit_updandinv(&b1, &u, &v, n, _state) )
            {
                waserrors = ae_true;
                break;
            }
            testinverseupdateunit_makeacopy(&inva, n, n, &b2, _state);
            rmatrixinvupdaterow(&b2, n, updrow, &v, _state);
            waserrors = waserrors||ae_fp_greater(testinverseupdateunit_matrixdiff(&b1, &b2, n, n, _state),threshold);
            
            /*
             * Test column update
             */
            updcol = ae_randominteger(n, _state);
            for(i=0; i<=n-1; i++)
            {
                if( i==updcol )
                {
                    v.ptr.p_double[i] = (double)(1);
                }
                else
                {
                    v.ptr.p_double[i] = (double)(0);
                }
                u.ptr.p_double[i] = 0.1*(2*ae_randomreal(_state)-1);
            }
            testinverseupdateunit_makeacopy(&a, n, n, &b1, _state);
            if( !testinverseupdateunit_updandinv(&b1, &u, &v, n, _state) )
            {
                waserrors = ae_true;
                break;
            }
            testinverseupdateunit_makeacopy(&inva, n, n, &b2, _state);
            rmatrixinvupdatecolumn(&b2, n, updcol, &u, _state);
            waserrors = waserrors||ae_fp_greater(testinverseupdateunit_matrixdiff(&b1, &b2, n, n, _state),threshold);
            
            /*
             * Test full update
             */
            for(i=0; i<=n-1; i++)
            {
                v.ptr.p_double[i] = 0.1*(2*ae_randomreal(_state)-1);
                u.ptr.p_double[i] = 0.1*(2*ae_randomreal(_state)-1);
            }
            testinverseupdateunit_makeacopy(&a, n, n, &b1, _state);
            if( !testinverseupdateunit_updandinv(&b1, &u, &v, n, _state) )
            {
                waserrors = ae_true;
                break;
            }
            testinverseupdateunit_makeacopy(&inva, n, n, &b2, _state);
            rmatrixinvupdateuv(&b2, n, &u, &v, _state);
            waserrors = waserrors||ae_fp_greater(testinverseupdateunit_matrixdiff(&b1, &b2, n, n, _state),threshold);
        }
    }
    
    /*
     * report
     */
    if( !silent )
    {
        printf("TESTING INVERSE UPDATE (REAL)\n");
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
ae_bool _pexec_testinverseupdate(ae_bool silent, ae_state *_state)
{
    return testinverseupdate(silent, _state);
}


/*************************************************************************
Copy
*************************************************************************/
static void testinverseupdateunit_makeacopy(/* Real    */ ae_matrix* a,
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
LU decomposition
*************************************************************************/
static void testinverseupdateunit_matlu(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_vector* pivots,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t jp;
    ae_vector t1;
    double s;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(pivots);
    ae_vector_init(&t1, 0, DT_REAL, _state);

    ae_vector_set_length(pivots, ae_minint(m-1, n-1, _state)+1, _state);
    ae_vector_set_length(&t1, ae_maxint(m-1, n-1, _state)+1, _state);
    ae_assert(m>=0&&n>=0, "Error in LUDecomposition: incorrect function arguments", _state);
    
    /*
     * Quick return if possible
     */
    if( m==0||n==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    for(j=0; j<=ae_minint(m-1, n-1, _state); j++)
    {
        
        /*
         * Find pivot and test for singularity.
         */
        jp = j;
        for(i=j+1; i<=m-1; i++)
        {
            if( ae_fp_greater(ae_fabs(a->ptr.pp_double[i][j], _state),ae_fabs(a->ptr.pp_double[jp][j], _state)) )
            {
                jp = i;
            }
        }
        pivots->ptr.p_int[j] = jp;
        if( ae_fp_neq(a->ptr.pp_double[jp][j],(double)(0)) )
        {
            
            /*
             *Apply the interchange to rows
             */
            if( jp!=j )
            {
                ae_v_move(&t1.ptr.p_double[0], 1, &a->ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
                ae_v_move(&a->ptr.pp_double[j][0], 1, &a->ptr.pp_double[jp][0], 1, ae_v_len(0,n-1));
                ae_v_move(&a->ptr.pp_double[jp][0], 1, &t1.ptr.p_double[0], 1, ae_v_len(0,n-1));
            }
            
            /*
             *Compute elements J+1:M of J-th column.
             */
            if( j+1<m )
            {
                jp = j+1;
                s = 1/a->ptr.pp_double[j][j];
                ae_v_muld(&a->ptr.pp_double[jp][j], a->stride, ae_v_len(jp,m-1), s);
            }
        }
        if( j<ae_minint(m, n, _state)-1 )
        {
            
            /*
             *Update trailing submatrix.
             */
            jp = j+1;
            for(i=j+1; i<=m-1; i++)
            {
                s = a->ptr.pp_double[i][j];
                ae_v_subd(&a->ptr.pp_double[i][jp], 1, &a->ptr.pp_double[j][jp], 1, ae_v_len(jp,n-1), s);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Generate matrix with given condition number C (2-norm)
*************************************************************************/
static void testinverseupdateunit_generaterandomorthogonalmatrix(/* Real    */ ae_matrix* a0,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    double t;
    double lambdav;
    ae_int_t s;
    ae_int_t i;
    ae_int_t j;
    double u1;
    double u2;
    ae_vector w;
    ae_vector v;
    ae_matrix a;
    double sm;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&v, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);

    if( n<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(&w, n+1, _state);
    ae_vector_set_length(&v, n+1, _state);
    ae_matrix_set_length(&a, n+1, n+1, _state);
    ae_matrix_set_length(a0, n-1+1, n-1+1, _state);
    
    /*
     * Prepare A
     */
    for(i=1; i<=n; i++)
    {
        for(j=1; j<=n; j++)
        {
            if( i==j )
            {
                a.ptr.pp_double[i][j] = (double)(1);
            }
            else
            {
                a.ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    
    /*
     * Calculate A using Stewart algorithm
     */
    for(s=2; s<=n; s++)
    {
        
        /*
         * Prepare v and Lambda = v'*v
         */
        do
        {
            i = 1;
            while(i<=s)
            {
                u1 = 2*ae_randomreal(_state)-1;
                u2 = 2*ae_randomreal(_state)-1;
                sm = u1*u1+u2*u2;
                if( ae_fp_eq(sm,(double)(0))||ae_fp_greater(sm,(double)(1)) )
                {
                    continue;
                }
                sm = ae_sqrt(-2*ae_log(sm, _state)/sm, _state);
                v.ptr.p_double[i] = u1*sm;
                if( i+1<=s )
                {
                    v.ptr.p_double[i+1] = u2*sm;
                }
                i = i+2;
            }
            lambdav = ae_v_dotproduct(&v.ptr.p_double[1], 1, &v.ptr.p_double[1], 1, ae_v_len(1,s));
        }
        while(ae_fp_eq(lambdav,(double)(0)));
        lambdav = 2/lambdav;
        
        /*
         * A * (I - 2 vv'/v'v ) =
         *   = A - (2/v'v) * A * v * v' =
         *   = A - (2/v'v) * w * v'
         *  where w = Av
         */
        for(i=1; i<=s; i++)
        {
            t = ae_v_dotproduct(&a.ptr.pp_double[i][1], 1, &v.ptr.p_double[1], 1, ae_v_len(1,s));
            w.ptr.p_double[i] = t;
        }
        for(i=1; i<=s; i++)
        {
            t = w.ptr.p_double[i]*lambdav;
            ae_v_subd(&a.ptr.pp_double[i][1], 1, &v.ptr.p_double[1], 1, ae_v_len(1,s), t);
        }
    }
    
    /*
     *
     */
    for(i=1; i<=n; i++)
    {
        for(j=1; j<=n; j++)
        {
            a0->ptr.pp_double[i-1][j-1] = a.ptr.pp_double[i][j];
        }
    }
    ae_frame_leave(_state);
}


static void testinverseupdateunit_generaterandommatrixcond(/* Real    */ ae_matrix* a0,
     ae_int_t n,
     double c,
     ae_state *_state)
{
    ae_frame _frame_block;
    double l1;
    double l2;
    ae_matrix q1;
    ae_matrix q2;
    ae_vector cc;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&q1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&q2, 0, 0, DT_REAL, _state);
    ae_vector_init(&cc, 0, DT_REAL, _state);

    testinverseupdateunit_generaterandomorthogonalmatrix(&q1, n, _state);
    testinverseupdateunit_generaterandomorthogonalmatrix(&q2, n, _state);
    ae_vector_set_length(&cc, n-1+1, _state);
    l1 = (double)(0);
    l2 = ae_log(1/c, _state);
    cc.ptr.p_double[0] = ae_exp(l1, _state);
    for(i=1; i<=n-2; i++)
    {
        cc.ptr.p_double[i] = ae_exp(ae_randomreal(_state)*(l2-l1)+l1, _state);
    }
    cc.ptr.p_double[n-1] = ae_exp(l2, _state);
    ae_matrix_set_length(a0, n-1+1, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a0->ptr.pp_double[i][j] = (double)(0);
            for(k=0; k<=n-1; k++)
            {
                a0->ptr.pp_double[i][j] = a0->ptr.pp_double[i][j]+q1.ptr.pp_double[i][k]*cc.ptr.p_double[k]*q2.ptr.pp_double[j][k];
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
triangular inverse
*************************************************************************/
static ae_bool testinverseupdateunit_invmattr(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunittriangular,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool nounit;
    ae_int_t i;
    ae_int_t j;
    double v;
    double ajj;
    ae_vector t;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&t, 0, DT_REAL, _state);

    result = ae_true;
    ae_vector_set_length(&t, n-1+1, _state);
    
    /*
     * Test the input parameters.
     */
    nounit = !isunittriangular;
    if( isupper )
    {
        
        /*
         * Compute inverse of upper triangular matrix.
         */
        for(j=0; j<=n-1; j++)
        {
            if( nounit )
            {
                if( ae_fp_eq(a->ptr.pp_double[j][j],(double)(0)) )
                {
                    result = ae_false;
                    ae_frame_leave(_state);
                    return result;
                }
                a->ptr.pp_double[j][j] = 1/a->ptr.pp_double[j][j];
                ajj = -a->ptr.pp_double[j][j];
            }
            else
            {
                ajj = (double)(-1);
            }
            
            /*
             * Compute elements 1:j-1 of j-th column.
             */
            if( j>0 )
            {
                ae_v_move(&t.ptr.p_double[0], 1, &a->ptr.pp_double[0][j], a->stride, ae_v_len(0,j-1));
                for(i=0; i<=j-1; i++)
                {
                    if( i<j-1 )
                    {
                        v = ae_v_dotproduct(&a->ptr.pp_double[i][i+1], 1, &t.ptr.p_double[i+1], 1, ae_v_len(i+1,j-1));
                    }
                    else
                    {
                        v = (double)(0);
                    }
                    if( nounit )
                    {
                        a->ptr.pp_double[i][j] = v+a->ptr.pp_double[i][i]*t.ptr.p_double[i];
                    }
                    else
                    {
                        a->ptr.pp_double[i][j] = v+t.ptr.p_double[i];
                    }
                }
                ae_v_muld(&a->ptr.pp_double[0][j], a->stride, ae_v_len(0,j-1), ajj);
            }
        }
    }
    else
    {
        
        /*
         * Compute inverse of lower triangular matrix.
         */
        for(j=n-1; j>=0; j--)
        {
            if( nounit )
            {
                if( ae_fp_eq(a->ptr.pp_double[j][j],(double)(0)) )
                {
                    result = ae_false;
                    ae_frame_leave(_state);
                    return result;
                }
                a->ptr.pp_double[j][j] = 1/a->ptr.pp_double[j][j];
                ajj = -a->ptr.pp_double[j][j];
            }
            else
            {
                ajj = (double)(-1);
            }
            if( j<n-1 )
            {
                
                /*
                 * Compute elements j+1:n of j-th column.
                 */
                ae_v_move(&t.ptr.p_double[j+1], 1, &a->ptr.pp_double[j+1][j], a->stride, ae_v_len(j+1,n-1));
                for(i=j+1; i<=n-1; i++)
                {
                    if( i>j+1 )
                    {
                        v = ae_v_dotproduct(&a->ptr.pp_double[i][j+1], 1, &t.ptr.p_double[j+1], 1, ae_v_len(j+1,i-1));
                    }
                    else
                    {
                        v = (double)(0);
                    }
                    if( nounit )
                    {
                        a->ptr.pp_double[i][j] = v+a->ptr.pp_double[i][i]*t.ptr.p_double[i];
                    }
                    else
                    {
                        a->ptr.pp_double[i][j] = v+t.ptr.p_double[i];
                    }
                }
                ae_v_muld(&a->ptr.pp_double[j+1][j], a->stride, ae_v_len(j+1,n-1), ajj);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
LU inverse
*************************************************************************/
static ae_bool testinverseupdateunit_invmatlu(/* Real    */ ae_matrix* a,
     /* Integer */ ae_vector* pivots,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_int_t i;
    ae_int_t j;
    ae_int_t jp;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&work, 0, DT_REAL, _state);

    result = ae_true;
    
    /*
     * Quick return if possible
     */
    if( n==0 )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_vector_set_length(&work, n-1+1, _state);
    
    /*
     * Form inv(U)
     */
    if( !testinverseupdateunit_invmattr(a, n, ae_true, ae_false, _state) )
    {
        result = ae_false;
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Solve the equation inv(A)*L = inv(U) for inv(A).
     */
    for(j=n-1; j>=0; j--)
    {
        
        /*
         * Copy current column of L to WORK and replace with zeros.
         */
        for(i=j+1; i<=n-1; i++)
        {
            work.ptr.p_double[i] = a->ptr.pp_double[i][j];
            a->ptr.pp_double[i][j] = (double)(0);
        }
        
        /*
         * Compute current column of inv(A).
         */
        if( j<n-1 )
        {
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&a->ptr.pp_double[i][j+1], 1, &work.ptr.p_double[j+1], 1, ae_v_len(j+1,n-1));
                a->ptr.pp_double[i][j] = a->ptr.pp_double[i][j]-v;
            }
        }
    }
    
    /*
     * Apply column interchanges.
     */
    for(j=n-2; j>=0; j--)
    {
        jp = pivots->ptr.p_int[j];
        if( jp!=j )
        {
            ae_v_move(&work.ptr.p_double[0], 1, &a->ptr.pp_double[0][j], a->stride, ae_v_len(0,n-1));
            ae_v_move(&a->ptr.pp_double[0][j], a->stride, &a->ptr.pp_double[0][jp], a->stride, ae_v_len(0,n-1));
            ae_v_move(&a->ptr.pp_double[0][jp], a->stride, &work.ptr.p_double[0], 1, ae_v_len(0,n-1));
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Matrix inverse
*************************************************************************/
static ae_bool testinverseupdateunit_invmat(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector pivots;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&pivots, 0, DT_INT, _state);

    testinverseupdateunit_matlu(a, n, n, &pivots, _state);
    result = testinverseupdateunit_invmatlu(a, &pivots, n, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Diff
*************************************************************************/
static double testinverseupdateunit_matrixdiff(/* Real    */ ae_matrix* a,
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
Update and inverse
*************************************************************************/
static ae_bool testinverseupdateunit_updandinv(/* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* u,
     /* Real    */ ae_vector* v,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector pivots;
    ae_int_t i;
    double r;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&pivots, 0, DT_INT, _state);

    for(i=0; i<=n-1; i++)
    {
        r = u->ptr.p_double[i];
        ae_v_addd(&a->ptr.pp_double[i][0], 1, &v->ptr.p_double[0], 1, ae_v_len(0,n-1), r);
    }
    testinverseupdateunit_matlu(a, n, n, &pivots, _state);
    result = testinverseupdateunit_invmatlu(a, &pivots, n, _state);
    ae_frame_leave(_state);
    return result;
}


/*$ End $*/
