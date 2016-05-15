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
#include "spdgevd.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Algorithm for solving the following generalized symmetric positive-definite
eigenproblem:
    A*x = lambda*B*x (1) or
    A*B*x = lambda*x (2) or
    B*A*x = lambda*x (3).
where A is a symmetric matrix, B - symmetric positive-definite matrix.
The problem is solved by reducing it to an ordinary  symmetric  eigenvalue
problem.

Input parameters:
    A           -   symmetric matrix which is given by its upper or lower
                    triangular part.
                    Array whose indexes range within [0..N-1, 0..N-1].
    N           -   size of matrices A and B.
    IsUpperA    -   storage format of matrix A.
    B           -   symmetric positive-definite matrix which is given by
                    its upper or lower triangular part.
                    Array whose indexes range within [0..N-1, 0..N-1].
    IsUpperB    -   storage format of matrix B.
    ZNeeded     -   if ZNeeded is equal to:
                     * 0, the eigenvectors are not returned;
                     * 1, the eigenvectors are returned.
    ProblemType -   if ProblemType is equal to:
                     * 1, the following problem is solved: A*x = lambda*B*x;
                     * 2, the following problem is solved: A*B*x = lambda*x;
                     * 3, the following problem is solved: B*A*x = lambda*x.

Output parameters:
    D           -   eigenvalues in ascending order.
                    Array whose index ranges within [0..N-1].
    Z           -   if ZNeeded is equal to:
                     * 0, Z hasn’t changed;
                     * 1, Z contains eigenvectors.
                    Array whose indexes range within [0..N-1, 0..N-1].
                    The eigenvectors are stored in matrix columns. It should
                    be noted that the eigenvectors in such problems do not
                    form an orthogonal system.

Result:
    True, if the problem was solved successfully.
    False, if the error occurred during the Cholesky decomposition of matrix
    B (the matrix isn’t positive-definite) or during the work of the iterative
    algorithm for solving the symmetric eigenproblem.

See also the GeneralizedSymmetricDefiniteEVDReduce subroutine.

  -- ALGLIB --
     Copyright 1.28.2006 by Bochkanov Sergey
*************************************************************************/
ae_bool smatrixgevd(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isuppera,
     /* Real    */ ae_matrix* b,
     ae_bool isupperb,
     ae_int_t zneeded,
     ae_int_t problemtype,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_matrix* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_matrix r;
    ae_matrix t;
    ae_bool isupperr;
    ae_int_t j1;
    ae_int_t j2;
    ae_int_t j1inc;
    ae_int_t j2inc;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_clear(d);
    ae_matrix_clear(z);
    ae_matrix_init(&r, 0, 0, DT_REAL, _state);
    ae_matrix_init(&t, 0, 0, DT_REAL, _state);

    
    /*
     * Reduce and solve
     */
    result = smatrixgevdreduce(a, n, isuppera, b, isupperb, problemtype, &r, &isupperr, _state);
    if( !result )
    {
        ae_frame_leave(_state);
        return result;
    }
    result = smatrixevd(a, n, zneeded, isuppera, d, &t, _state);
    if( !result )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Transform eigenvectors if needed
     */
    if( zneeded!=0 )
    {
        
        /*
         * fill Z with zeros
         */
        ae_matrix_set_length(z, n-1+1, n-1+1, _state);
        for(j=0; j<=n-1; j++)
        {
            z->ptr.pp_double[0][j] = 0.0;
        }
        for(i=1; i<=n-1; i++)
        {
            ae_v_move(&z->ptr.pp_double[i][0], 1, &z->ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
        }
        
        /*
         * Setup R properties
         */
        if( isupperr )
        {
            j1 = 0;
            j2 = n-1;
            j1inc = 1;
            j2inc = 0;
        }
        else
        {
            j1 = 0;
            j2 = 0;
            j1inc = 0;
            j2inc = 1;
        }
        
        /*
         * Calculate R*Z
         */
        for(i=0; i<=n-1; i++)
        {
            for(j=j1; j<=j2; j++)
            {
                v = r.ptr.pp_double[i][j];
                ae_v_addd(&z->ptr.pp_double[i][0], 1, &t.ptr.pp_double[j][0], 1, ae_v_len(0,n-1), v);
            }
            j1 = j1+j1inc;
            j2 = j2+j2inc;
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Algorithm for reduction of the following generalized symmetric positive-
definite eigenvalue problem:
    A*x = lambda*B*x (1) or
    A*B*x = lambda*x (2) or
    B*A*x = lambda*x (3)
to the symmetric eigenvalues problem C*y = lambda*y (eigenvalues of this and
the given problems are the same, and the eigenvectors of the given problem
could be obtained by multiplying the obtained eigenvectors by the
transformation matrix x = R*y).

Here A is a symmetric matrix, B - symmetric positive-definite matrix.

Input parameters:
    A           -   symmetric matrix which is given by its upper or lower
                    triangular part.
                    Array whose indexes range within [0..N-1, 0..N-1].
    N           -   size of matrices A and B.
    IsUpperA    -   storage format of matrix A.
    B           -   symmetric positive-definite matrix which is given by
                    its upper or lower triangular part.
                    Array whose indexes range within [0..N-1, 0..N-1].
    IsUpperB    -   storage format of matrix B.
    ProblemType -   if ProblemType is equal to:
                     * 1, the following problem is solved: A*x = lambda*B*x;
                     * 2, the following problem is solved: A*B*x = lambda*x;
                     * 3, the following problem is solved: B*A*x = lambda*x.

Output parameters:
    A           -   symmetric matrix which is given by its upper or lower
                    triangle depending on IsUpperA. Contains matrix C.
                    Array whose indexes range within [0..N-1, 0..N-1].
    R           -   upper triangular or low triangular transformation matrix
                    which is used to obtain the eigenvectors of a given problem
                    as the product of eigenvectors of C (from the right) and
                    matrix R (from the left). If the matrix is upper
                    triangular, the elements below the main diagonal
                    are equal to 0 (and vice versa). Thus, we can perform
                    the multiplication without taking into account the
                    internal structure (which is an easier though less
                    effective way).
                    Array whose indexes range within [0..N-1, 0..N-1].
    IsUpperR    -   type of matrix R (upper or lower triangular).

Result:
    True, if the problem was reduced successfully.
    False, if the error occurred during the Cholesky decomposition of
        matrix B (the matrix is not positive-definite).

  -- ALGLIB --
     Copyright 1.28.2006 by Bochkanov Sergey
*************************************************************************/
ae_bool smatrixgevdreduce(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isuppera,
     /* Real    */ ae_matrix* b,
     ae_bool isupperb,
     ae_int_t problemtype,
     /* Real    */ ae_matrix* r,
     ae_bool* isupperr,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix t;
    ae_vector w1;
    ae_vector w2;
    ae_vector w3;
    ae_int_t i;
    ae_int_t j;
    double v;
    matinvreport rep;
    ae_int_t info;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(r);
    *isupperr = ae_false;
    ae_matrix_init(&t, 0, 0, DT_REAL, _state);
    ae_vector_init(&w1, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&w3, 0, DT_REAL, _state);
    _matinvreport_init(&rep, _state);

    ae_assert(n>0, "SMatrixGEVDReduce: N<=0!", _state);
    ae_assert((problemtype==1||problemtype==2)||problemtype==3, "SMatrixGEVDReduce: incorrect ProblemType!", _state);
    result = ae_true;
    
    /*
     * Problem 1:  A*x = lambda*B*x
     *
     * Reducing to:
     *     C*y = lambda*y
     *     C = L^(-1) * A * L^(-T)
     *     x = L^(-T) * y
     */
    if( problemtype==1 )
    {
        
        /*
         * Factorize B in T: B = LL'
         */
        ae_matrix_set_length(&t, n-1+1, n-1+1, _state);
        if( isupperb )
        {
            for(i=0; i<=n-1; i++)
            {
                ae_v_move(&t.ptr.pp_double[i][i], t.stride, &b->ptr.pp_double[i][i], 1, ae_v_len(i,n-1));
            }
        }
        else
        {
            for(i=0; i<=n-1; i++)
            {
                ae_v_move(&t.ptr.pp_double[i][0], 1, &b->ptr.pp_double[i][0], 1, ae_v_len(0,i));
            }
        }
        if( !spdmatrixcholesky(&t, n, ae_false, _state) )
        {
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Invert L in T
         */
        rmatrixtrinverse(&t, n, ae_false, ae_false, &info, &rep, _state);
        if( info<=0 )
        {
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Build L^(-1) * A * L^(-T) in R
         */
        ae_vector_set_length(&w1, n+1, _state);
        ae_vector_set_length(&w2, n+1, _state);
        ae_matrix_set_length(r, n-1+1, n-1+1, _state);
        for(j=1; j<=n; j++)
        {
            
            /*
             * Form w2 = A * l'(j) (here l'(j) is j-th column of L^(-T))
             */
            ae_v_move(&w1.ptr.p_double[1], 1, &t.ptr.pp_double[j-1][0], 1, ae_v_len(1,j));
            symmetricmatrixvectormultiply(a, isuppera, 0, j-1, &w1, 1.0, &w2, _state);
            if( isuppera )
            {
                matrixvectormultiply(a, 0, j-1, j, n-1, ae_true, &w1, 1, j, 1.0, &w2, j+1, n, 0.0, _state);
            }
            else
            {
                matrixvectormultiply(a, j, n-1, 0, j-1, ae_false, &w1, 1, j, 1.0, &w2, j+1, n, 0.0, _state);
            }
            
            /*
             * Form l(i)*w2 (here l(i) is i-th row of L^(-1))
             */
            for(i=1; i<=n; i++)
            {
                v = ae_v_dotproduct(&t.ptr.pp_double[i-1][0], 1, &w2.ptr.p_double[1], 1, ae_v_len(0,i-1));
                r->ptr.pp_double[i-1][j-1] = v;
            }
        }
        
        /*
         * Copy R to A
         */
        for(i=0; i<=n-1; i++)
        {
            ae_v_move(&a->ptr.pp_double[i][0], 1, &r->ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
        }
        
        /*
         * Copy L^(-1) from T to R and transpose
         */
        *isupperr = ae_true;
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=i-1; j++)
            {
                r->ptr.pp_double[i][j] = (double)(0);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            ae_v_move(&r->ptr.pp_double[i][i], 1, &t.ptr.pp_double[i][i], t.stride, ae_v_len(i,n-1));
        }
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Problem 2:  A*B*x = lambda*x
     * or
     * problem 3:  B*A*x = lambda*x
     *
     * Reducing to:
     *     C*y = lambda*y
     *     C = U * A * U'
     *     B = U'* U
     */
    if( problemtype==2||problemtype==3 )
    {
        
        /*
         * Factorize B in T: B = U'*U
         */
        ae_matrix_set_length(&t, n-1+1, n-1+1, _state);
        if( isupperb )
        {
            for(i=0; i<=n-1; i++)
            {
                ae_v_move(&t.ptr.pp_double[i][i], 1, &b->ptr.pp_double[i][i], 1, ae_v_len(i,n-1));
            }
        }
        else
        {
            for(i=0; i<=n-1; i++)
            {
                ae_v_move(&t.ptr.pp_double[i][i], 1, &b->ptr.pp_double[i][i], b->stride, ae_v_len(i,n-1));
            }
        }
        if( !spdmatrixcholesky(&t, n, ae_true, _state) )
        {
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Build U * A * U' in R
         */
        ae_vector_set_length(&w1, n+1, _state);
        ae_vector_set_length(&w2, n+1, _state);
        ae_vector_set_length(&w3, n+1, _state);
        ae_matrix_set_length(r, n-1+1, n-1+1, _state);
        for(j=1; j<=n; j++)
        {
            
            /*
             * Form w2 = A * u'(j) (here u'(j) is j-th column of U')
             */
            ae_v_move(&w1.ptr.p_double[1], 1, &t.ptr.pp_double[j-1][j-1], 1, ae_v_len(1,n-j+1));
            symmetricmatrixvectormultiply(a, isuppera, j-1, n-1, &w1, 1.0, &w3, _state);
            ae_v_move(&w2.ptr.p_double[j], 1, &w3.ptr.p_double[1], 1, ae_v_len(j,n));
            ae_v_move(&w1.ptr.p_double[j], 1, &t.ptr.pp_double[j-1][j-1], 1, ae_v_len(j,n));
            if( isuppera )
            {
                matrixvectormultiply(a, 0, j-2, j-1, n-1, ae_false, &w1, j, n, 1.0, &w2, 1, j-1, 0.0, _state);
            }
            else
            {
                matrixvectormultiply(a, j-1, n-1, 0, j-2, ae_true, &w1, j, n, 1.0, &w2, 1, j-1, 0.0, _state);
            }
            
            /*
             * Form u(i)*w2 (here u(i) is i-th row of U)
             */
            for(i=1; i<=n; i++)
            {
                v = ae_v_dotproduct(&t.ptr.pp_double[i-1][i-1], 1, &w2.ptr.p_double[i], 1, ae_v_len(i-1,n-1));
                r->ptr.pp_double[i-1][j-1] = v;
            }
        }
        
        /*
         * Copy R to A
         */
        for(i=0; i<=n-1; i++)
        {
            ae_v_move(&a->ptr.pp_double[i][0], 1, &r->ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
        }
        if( problemtype==2 )
        {
            
            /*
             * Invert U in T
             */
            rmatrixtrinverse(&t, n, ae_true, ae_false, &info, &rep, _state);
            if( info<=0 )
            {
                result = ae_false;
                ae_frame_leave(_state);
                return result;
            }
            
            /*
             * Copy U^-1 from T to R
             */
            *isupperr = ae_true;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    r->ptr.pp_double[i][j] = (double)(0);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                ae_v_move(&r->ptr.pp_double[i][i], 1, &t.ptr.pp_double[i][i], 1, ae_v_len(i,n-1));
            }
        }
        else
        {
            
            /*
             * Copy U from T to R and transpose
             */
            *isupperr = ae_false;
            for(i=0; i<=n-1; i++)
            {
                for(j=i+1; j<=n-1; j++)
                {
                    r->ptr.pp_double[i][j] = (double)(0);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                ae_v_move(&r->ptr.pp_double[i][i], r->stride, &t.ptr.pp_double[i][i], 1, ae_v_len(i,n-1));
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*$ End $*/
