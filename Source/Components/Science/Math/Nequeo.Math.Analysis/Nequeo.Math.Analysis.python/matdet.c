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
#include "matdet.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Determinant calculation of the matrix given by its LU decomposition.

Input parameters:
    A       -   LU decomposition of the matrix (output of
                RMatrixLU subroutine).
    Pivots  -   table of permutations which were made during
                the LU decomposition.
                Output of RMatrixLU subroutine.
    N       -   (optional) size of matrix A:
                * if given, only principal NxN submatrix is processed and
                  overwritten. other elements are unchanged.
                * if not given, automatically determined from matrix size
                  (A must be square matrix)

Result: matrix determinant.

  -- ALGLIB --
     Copyright 2005 by Bochkanov Sergey
*************************************************************************/
double rmatrixludet(/* Real    */ ae_matrix* a,
     /* Integer */ ae_vector* pivots,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t s;
    double result;


    ae_assert(n>=1, "RMatrixLUDet: N<1!", _state);
    ae_assert(pivots->cnt>=n, "RMatrixLUDet: Pivots array is too short!", _state);
    ae_assert(a->rows>=n, "RMatrixLUDet: rows(A)<N!", _state);
    ae_assert(a->cols>=n, "RMatrixLUDet: cols(A)<N!", _state);
    ae_assert(apservisfinitematrix(a, n, n, _state), "RMatrixLUDet: A contains infinite or NaN values!", _state);
    result = (double)(1);
    s = 1;
    for(i=0; i<=n-1; i++)
    {
        result = result*a->ptr.pp_double[i][i];
        if( pivots->ptr.p_int[i]!=i )
        {
            s = -s;
        }
    }
    result = result*s;
    return result;
}


/*************************************************************************
Calculation of the determinant of a general matrix

Input parameters:
    A       -   matrix, array[0..N-1, 0..N-1]
    N       -   (optional) size of matrix A:
                * if given, only principal NxN submatrix is processed and
                  overwritten. other elements are unchanged.
                * if not given, automatically determined from matrix size
                  (A must be square matrix)

Result: determinant of matrix A.

  -- ALGLIB --
     Copyright 2005 by Bochkanov Sergey
*************************************************************************/
double rmatrixdet(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_vector pivots;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init(&pivots, 0, DT_INT, _state);

    ae_assert(n>=1, "RMatrixDet: N<1!", _state);
    ae_assert(a->rows>=n, "RMatrixDet: rows(A)<N!", _state);
    ae_assert(a->cols>=n, "RMatrixDet: cols(A)<N!", _state);
    ae_assert(apservisfinitematrix(a, n, n, _state), "RMatrixDet: A contains infinite or NaN values!", _state);
    rmatrixlu(a, n, n, &pivots, _state);
    result = rmatrixludet(a, &pivots, n, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Determinant calculation of the matrix given by its LU decomposition.

Input parameters:
    A       -   LU decomposition of the matrix (output of
                RMatrixLU subroutine).
    Pivots  -   table of permutations which were made during
                the LU decomposition.
                Output of RMatrixLU subroutine.
    N       -   (optional) size of matrix A:
                * if given, only principal NxN submatrix is processed and
                  overwritten. other elements are unchanged.
                * if not given, automatically determined from matrix size
                  (A must be square matrix)

Result: matrix determinant.

  -- ALGLIB --
     Copyright 2005 by Bochkanov Sergey
*************************************************************************/
ae_complex cmatrixludet(/* Complex */ ae_matrix* a,
     /* Integer */ ae_vector* pivots,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t s;
    ae_complex result;


    ae_assert(n>=1, "CMatrixLUDet: N<1!", _state);
    ae_assert(pivots->cnt>=n, "CMatrixLUDet: Pivots array is too short!", _state);
    ae_assert(a->rows>=n, "CMatrixLUDet: rows(A)<N!", _state);
    ae_assert(a->cols>=n, "CMatrixLUDet: cols(A)<N!", _state);
    ae_assert(apservisfinitecmatrix(a, n, n, _state), "CMatrixLUDet: A contains infinite or NaN values!", _state);
    result = ae_complex_from_i(1);
    s = 1;
    for(i=0; i<=n-1; i++)
    {
        result = ae_c_mul(result,a->ptr.pp_complex[i][i]);
        if( pivots->ptr.p_int[i]!=i )
        {
            s = -s;
        }
    }
    result = ae_c_mul_d(result,(double)(s));
    return result;
}


/*************************************************************************
Calculation of the determinant of a general matrix

Input parameters:
    A       -   matrix, array[0..N-1, 0..N-1]
    N       -   (optional) size of matrix A:
                * if given, only principal NxN submatrix is processed and
                  overwritten. other elements are unchanged.
                * if not given, automatically determined from matrix size
                  (A must be square matrix)

Result: determinant of matrix A.

  -- ALGLIB --
     Copyright 2005 by Bochkanov Sergey
*************************************************************************/
ae_complex cmatrixdet(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_vector pivots;
    ae_complex result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init(&pivots, 0, DT_INT, _state);

    ae_assert(n>=1, "CMatrixDet: N<1!", _state);
    ae_assert(a->rows>=n, "CMatrixDet: rows(A)<N!", _state);
    ae_assert(a->cols>=n, "CMatrixDet: cols(A)<N!", _state);
    ae_assert(apservisfinitecmatrix(a, n, n, _state), "CMatrixDet: A contains infinite or NaN values!", _state);
    cmatrixlu(a, n, n, &pivots, _state);
    result = cmatrixludet(a, &pivots, n, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Determinant calculation of the matrix given by the Cholesky decomposition.

Input parameters:
    A       -   Cholesky decomposition,
                output of SMatrixCholesky subroutine.
    N       -   (optional) size of matrix A:
                * if given, only principal NxN submatrix is processed and
                  overwritten. other elements are unchanged.
                * if not given, automatically determined from matrix size
                  (A must be square matrix)

As the determinant is equal to the product of squares of diagonal elements,
it’s not necessary to specify which triangle - lower or upper - the matrix
is stored in.

Result:
    matrix determinant.

  -- ALGLIB --
     Copyright 2005-2008 by Bochkanov Sergey
*************************************************************************/
double spdmatrixcholeskydet(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_bool f;
    double result;


    ae_assert(n>=1, "SPDMatrixCholeskyDet: N<1!", _state);
    ae_assert(a->rows>=n, "SPDMatrixCholeskyDet: rows(A)<N!", _state);
    ae_assert(a->cols>=n, "SPDMatrixCholeskyDet: cols(A)<N!", _state);
    f = ae_true;
    for(i=0; i<=n-1; i++)
    {
        f = f&&ae_isfinite(a->ptr.pp_double[i][i], _state);
    }
    ae_assert(f, "SPDMatrixCholeskyDet: A contains infinite or NaN values!", _state);
    result = (double)(1);
    for(i=0; i<=n-1; i++)
    {
        result = result*ae_sqr(a->ptr.pp_double[i][i], _state);
    }
    return result;
}


/*************************************************************************
Determinant calculation of the symmetric positive definite matrix.

Input parameters:
    A       -   matrix. Array with elements [0..N-1, 0..N-1].
    N       -   (optional) size of matrix A:
                * if given, only principal NxN submatrix is processed and
                  overwritten. other elements are unchanged.
                * if not given, automatically determined from matrix size
                  (A must be square matrix)
    IsUpper -   (optional) storage type:
                * if True, symmetric matrix  A  is  given  by  its  upper
                  triangle, and the lower triangle isn’t used/changed  by
                  function
                * if False, symmetric matrix  A  is  given  by  its lower
                  triangle, and the upper triangle isn’t used/changed  by
                  function
                * if not given, both lower and upper  triangles  must  be
                  filled.

Result:
    determinant of matrix A.
    If matrix A is not positive definite, exception is thrown.

  -- ALGLIB --
     Copyright 2005-2008 by Bochkanov Sergey
*************************************************************************/
double spdmatrixdet(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_bool b;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;

    ae_assert(n>=1, "SPDMatrixDet: N<1!", _state);
    ae_assert(a->rows>=n, "SPDMatrixDet: rows(A)<N!", _state);
    ae_assert(a->cols>=n, "SPDMatrixDet: cols(A)<N!", _state);
    ae_assert(isfinitertrmatrix(a, n, isupper, _state), "SPDMatrixDet: A contains infinite or NaN values!", _state);
    b = spdmatrixcholesky(a, n, isupper, _state);
    ae_assert(b, "SPDMatrixDet: A is not SPD!", _state);
    result = spdmatrixcholeskydet(a, n, _state);
    ae_frame_leave(_state);
    return result;
}


/*$ End $*/
