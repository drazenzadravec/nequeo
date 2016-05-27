/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          LapackFunctions.cpp
*  Purpose :       Common Lapack functions.
*
*/

/*
Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#include "stdafx.h"
#include "LapackTemplates.cpp"

#include "mkl.h"

extern "C" 
{
	/// <summary>
	/// The function ?lange returns the value of the 1-norm, or the Frobenius norm, or the infinity norm, or the element of largest absolute value of a real/complex matrix A.
	/// </summary>
	/// <param name="norm">Specifies the value to be returned by the routine:
	/// = 'M' or 'm': val = max(abs(Aij)), largest absolute value of the matrix A.
	/// = '1' or 'O' or 'o' : val = norm1(A), 1 - norm of the matrix A(maximum column sum),
	/// = 'I' or 'i' : val = normI(A), infinity norm of the matrix A(maximum row sum),
	/// = 'F', 'f', 'E' or 'e' : val = normF(A), Frobenius norm of the matrix A(square root of sum of squares).
	/// </param>
	/// <param name="m">The number of rows of the matrix A. m ? 0. When m = 0, ? lange is set to zero.</param>
	/// <param name="n">The number of columns of the matrix A. n ? 0. When n = 0, ? lange is set to zero.</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column major and max(1, lda*m) for row major layout. Array a contains the m-by-n matrix A. </param>
	/// <returns>Returns the value of the 1-norm, Frobenius norm, infinity-norm, or the largest absolute value of any element of a general rectangular matrix.</returns>
	EXPORT_NEQUEO_MKL_API float s_matrix_norm(char norm, MKL_INT m, MKL_INT n, float a[])
	{
		return LAPACKE_slange(LAPACK_COL_MAJOR, norm, m, n, a, m);
	}

	/// <summary>
	/// The function ?lange returns the value of the 1-norm, or the Frobenius norm, or the infinity norm, or the element of largest absolute value of a real/complex matrix A.
	/// </summary>
	/// <param name="norm">Specifies the value to be returned by the routine:
	/// = 'M' or 'm': val = max(abs(Aij)), largest absolute value of the matrix A.
	/// = '1' or 'O' or 'o' : val = norm1(A), 1 - norm of the matrix A(maximum column sum),
	/// = 'I' or 'i' : val = normI(A), infinity norm of the matrix A(maximum row sum),
	/// = 'F', 'f', 'E' or 'e' : val = normF(A), Frobenius norm of the matrix A(square root of sum of squares).
	/// </param>
	/// <param name="m">The number of rows of the matrix A. m ? 0. When m = 0, ? lange is set to zero.</param>
	/// <param name="n">The number of columns of the matrix A. n ? 0. When n = 0, ? lange is set to zero.</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column major and max(1, lda*m) for row major layout. Array a contains the m-by-n matrix A. </param>
	/// <returns>Returns the value of the 1-norm, Frobenius norm, infinity-norm, or the largest absolute value of any element of a general rectangular matrix.</returns>
	EXPORT_NEQUEO_MKL_API double d_matrix_norm(char norm, MKL_INT m, MKL_INT n, double a[])
	{
		return LAPACKE_dlange(LAPACK_COL_MAJOR, norm, m, n, a, m);
	}

	/// <summary>
	/// The function ?lange returns the value of the 1-norm, or the Frobenius norm, or the infinity norm, or the element of largest absolute value of a real/complex matrix A.
	/// </summary>
	/// <param name="norm">Specifies the value to be returned by the routine:
	/// = 'M' or 'm': val = max(abs(Aij)), largest absolute value of the matrix A.
	/// = '1' or 'O' or 'o' : val = norm1(A), 1 - norm of the matrix A(maximum column sum),
	/// = 'I' or 'i' : val = normI(A), infinity norm of the matrix A(maximum row sum),
	/// = 'F', 'f', 'E' or 'e' : val = normF(A), Frobenius norm of the matrix A(square root of sum of squares).
	/// </param>
	/// <param name="m">The number of rows of the matrix A. m ? 0. When m = 0, ? lange is set to zero.</param>
	/// <param name="n">The number of columns of the matrix A. n ? 0. When n = 0, ? lange is set to zero.</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column major and max(1, lda*m) for row major layout. Array a contains the m-by-n matrix A. </param>
	/// <returns>Returns the value of the 1-norm, Frobenius norm, infinity-norm, or the largest absolute value of any element of a general rectangular matrix.</returns>
	EXPORT_NEQUEO_MKL_API float c_matrix_norm(char norm, MKL_INT m, MKL_INT n, MKL_Complex8 a[])
	{
		return LAPACKE_clange(LAPACK_COL_MAJOR, norm, m, n, a, m);
	}

	/// <summary>
	/// The function ?lange returns the value of the 1-norm, or the Frobenius norm, or the infinity norm, or the element of largest absolute value of a real/complex matrix A.
	/// </summary>
	/// <param name="norm">Specifies the value to be returned by the routine:
	/// = 'M' or 'm': val = max(abs(Aij)), largest absolute value of the matrix A.
	/// = '1' or 'O' or 'o' : val = norm1(A), 1 - norm of the matrix A(maximum column sum),
	/// = 'I' or 'i' : val = normI(A), infinity norm of the matrix A(maximum row sum),
	/// = 'F', 'f', 'E' or 'e' : val = normF(A), Frobenius norm of the matrix A(square root of sum of squares).
	/// </param>
	/// <param name="m">The number of rows of the matrix A. m ? 0. When m = 0, ? lange is set to zero.</param>
	/// <param name="n">The number of columns of the matrix A. n ? 0. When n = 0, ? lange is set to zero.</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column major and max(1, lda*m) for row major layout. Array a contains the m-by-n matrix A. </param>
	/// <returns>Returns the value of the 1-norm, Frobenius norm, infinity-norm, or the largest absolute value of any element of a general rectangular matrix.</returns>
	EXPORT_NEQUEO_MKL_API double z_matrix_norm(char norm, MKL_INT m, MKL_INT n, MKL_Complex16 a[])
	{
		return LAPACKE_zlange(LAPACK_COL_MAJOR, norm, m, n, a, m);
	}

	/// <summary>
	/// Computes the LU factorization of a general m-by-n matrix. A = P*L*U.
	/// where P is a permutation matrix, L is lower triangular with unit diagonal elements (lower trapezoidal if m > n) and U is upper triangular (upper trapezoidal if m < n). The routine uses partial pivoting, with row interchanges.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column-major layout or max(1, lda*m) for row-major layout. Contains the matrix A.</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info=0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, uii is 0. The factorization has been completed, but U is exactly singular. Division by 0 will occur if you use the factor U for solving a system of linear equations.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_lu_factor(MKL_INT m, float a[], MKL_INT ipiv[])
	{
		return lu_factor(m, a, ipiv, LAPACKE_sgetrf);
	}

	/// <summary>
	/// Computes the LU factorization of a general m-by-n matrix. A = P*L*U.
	/// where P is a permutation matrix, L is lower triangular with unit diagonal elements (lower trapezoidal if m > n) and U is upper triangular (upper trapezoidal if m < n). The routine uses partial pivoting, with row interchanges.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column-major layout or max(1, lda*m) for row-major layout. Contains the matrix A.</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info=0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, uii is 0. The factorization has been completed, but U is exactly singular. Division by 0 will occur if you use the factor U for solving a system of linear equations.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_lu_factor(MKL_INT m, double a[], MKL_INT ipiv[])
	{
		return lu_factor(m, a, ipiv, LAPACKE_dgetrf);
	}

	/// <summary>
	/// Computes the LU factorization of a general m-by-n matrix. A = P*L*U.
	/// where P is a permutation matrix, L is lower triangular with unit diagonal elements (lower trapezoidal if m > n) and U is upper triangular (upper trapezoidal if m < n). The routine uses partial pivoting, with row interchanges.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column-major layout or max(1, lda*m) for row-major layout. Contains the matrix A.</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info=0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, uii is 0. The factorization has been completed, but U is exactly singular. Division by 0 will occur if you use the factor U for solving a system of linear equations.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_lu_factor(MKL_INT m, MKL_Complex8 a[], MKL_INT ipiv[])
	{
		return lu_factor(m, a, ipiv, LAPACKE_cgetrf);
	}

	/// <summary>
	/// Computes the LU factorization of a general m-by-n matrix. A = P*L*U.
	/// where P is a permutation matrix, L is lower triangular with unit diagonal elements (lower trapezoidal if m > n) and U is upper triangular (upper trapezoidal if m < n). The routine uses partial pivoting, with row interchanges.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column-major layout or max(1, lda*m) for row-major layout. Contains the matrix A.</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info=0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, uii is 0. The factorization has been completed, but U is exactly singular. Division by 0 will occur if you use the factor U for solving a system of linear equations.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_lu_factor(MKL_INT m, MKL_Complex16 a[], MKL_INT ipiv[])
	{
		return lu_factor(m, a, ipiv, LAPACKE_zgetrf);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="work">The work matrix.</param>
	/// <param name="lwork">The work length.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_lu_inverse(MKL_INT n, float a[], float work[], MKL_INT lwork)
	{
		return lu_inverse(n, a, LAPACKE_sgetrf, LAPACKE_sgetri);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="work">The work matrix.</param>
	/// <param name="lwork">The work length.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_lu_inverse(MKL_INT n, double a[], double work[], MKL_INT lwork)
	{
		return lu_inverse(n, a, LAPACKE_dgetrf, LAPACKE_dgetri);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="work">The work matrix.</param>
	/// <param name="lwork">The work length.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_lu_inverse(MKL_INT n, MKL_Complex8 a[], MKL_Complex8 work[], MKL_INT lwork)
	{
		return lu_inverse(n, a, LAPACKE_cgetrf, LAPACKE_cgetri);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="work">The work matrix.</param>
	/// <param name="lwork">The work length.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_lu_inverse(MKL_INT n, MKL_Complex16 a[], MKL_Complex16 work[], MKL_INT lwork)
	{
		return lu_inverse(n, a, LAPACKE_zgetrf, LAPACKE_zgetri);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <param name="work">The work matrix.</param>
	/// <param name="lwork">The work length.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_lu_inverse_factored(MKL_INT n, float a[], MKL_INT ipiv[], float work[], MKL_INT lwork)
	{
		return lu_inverse_factored(n, a, ipiv, LAPACKE_sgetri);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <param name="work">The work matrix.</param>
	/// <param name="lwork">The work length.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_lu_inverse_factored(MKL_INT n, double a[], MKL_INT ipiv[], double work[], MKL_INT lwork)
	{
		return lu_inverse_factored(n, a, ipiv, LAPACKE_dgetri);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <param name="work">The work matrix.</param>
	/// <param name="lwork">The work length.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_lu_inverse_factored(MKL_INT n, MKL_Complex8 a[], MKL_INT ipiv[], MKL_Complex8 work[], MKL_INT lwork)
	{
		return lu_inverse_factored(n, a, ipiv, LAPACKE_cgetri);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <param name="work">The work matrix.</param>
	/// <param name="lwork">The work length.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_lu_inverse_factored(MKL_INT n, MKL_Complex16 a[], MKL_INT ipiv[], MKL_Complex16 work[], MKL_INT lwork)
	{
		return lu_inverse_factored(n, a, ipiv, LAPACKE_zgetri);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="ipiv">Array, size at least max(1, n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_lu_solve_factored(MKL_INT n, MKL_INT nrhs, float a[], MKL_INT ipiv[], float b[])
	{
		return lu_solve_factored(n, nrhs, a, ipiv, b, LAPACKE_sgetrs);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="ipiv">Array, size at least max(1, n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT  d_lu_solve_factored(MKL_INT n, MKL_INT nrhs, double a[], MKL_INT ipiv[], double b[])
	{
		return lu_solve_factored(n, nrhs, a, ipiv, b, LAPACKE_dgetrs);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="ipiv">Array, size at least max(1, n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_lu_solve_factored(MKL_INT n, MKL_INT nrhs, MKL_Complex8 a[], MKL_INT ipiv[], MKL_Complex8 b[])
	{
		return lu_solve_factored(n, nrhs, a, ipiv, b, LAPACKE_cgetrs);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="ipiv">Array, size at least max(1, n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_lu_solve_factored(MKL_INT n, MKL_INT nrhs, MKL_Complex16 a[], MKL_INT ipiv[], MKL_Complex16 b[])
	{
		return lu_solve_factored(n, nrhs, a, ipiv, b, LAPACKE_zgetrs);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_lu_solve(MKL_INT n, MKL_INT nrhs, float a[], float b[])
	{
		return lu_solve(n, nrhs, a, b, LAPACKE_sgetrf, LAPACKE_sgetrs);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_lu_solve(MKL_INT n, MKL_INT nrhs, double a[], double b[])
	{
		return lu_solve(n, nrhs, a, b, LAPACKE_dgetrf, LAPACKE_dgetrs);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_lu_solve(MKL_INT n, MKL_INT nrhs, MKL_Complex8 a[], MKL_Complex8 b[])
	{
		return lu_solve(n, nrhs, a, b, LAPACKE_cgetrf, LAPACKE_cgetrs);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_lu_solve(MKL_INT n, MKL_INT nrhs, MKL_Complex16 a[], MKL_Complex16 b[])
	{
		return lu_solve(n, nrhs, a, b, LAPACKE_zgetrf, LAPACKE_zgetrs);
	}

	/// <summary>
	/// Computes the Cholesky factorization of a symmetric (Hermitian) positive-definite matrix.
	/// The routine forms the Cholesky factorization of a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data  if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="a">The upper or lower triangular part of a is overwritten by the Cholesky factor U or L, as specified by uplo. Array, size max(1, lda*n. The array a contains either the upper or the lower triangular part of the matrix A (see uplo).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the leading minor of order i (and therefore the matrix A itself) is not positive-definite, and the factorization could not be completed. This may indicate an error in forming the matrix A.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_cholesky_factor(MKL_INT n, float a[]) 
	{
		return cholesky_factor(n, a, LAPACKE_spotrf);
	}

	/// <summary>
	/// Computes the Cholesky factorization of a symmetric (Hermitian) positive-definite matrix.
	/// The routine forms the Cholesky factorization of a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data  if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="a">The upper or lower triangular part of a is overwritten by the Cholesky factor U or L, as specified by uplo. Array, size max(1, lda*n. The array a contains either the upper or the lower triangular part of the matrix A (see uplo).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the leading minor of order i (and therefore the matrix A itself) is not positive-definite, and the factorization could not be completed. This may indicate an error in forming the matrix A.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_cholesky_factor(MKL_INT n, double a[]) 
	{
		return cholesky_factor(n, a, LAPACKE_dpotrf);
	}

	/// <summary>
	/// Computes the Cholesky factorization of a symmetric (Hermitian) positive-definite matrix.
	/// The routine forms the Cholesky factorization of a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data  if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="a">The upper or lower triangular part of a is overwritten by the Cholesky factor U or L, as specified by uplo. Array, size max(1, lda*n. The array a contains either the upper or the lower triangular part of the matrix A (see uplo).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the leading minor of order i (and therefore the matrix A itself) is not positive-definite, and the factorization could not be completed. This may indicate an error in forming the matrix A.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_cholesky_factor(MKL_INT n, MKL_Complex8 a[]) 
	{
		return cholesky_factor(n, a, LAPACKE_cpotrf);
	}

	/// <summary>
	/// Computes the Cholesky factorization of a symmetric (Hermitian) positive-definite matrix.
	/// The routine forms the Cholesky factorization of a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data  if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="a">The upper or lower triangular part of a is overwritten by the Cholesky factor U or L, as specified by uplo. Array, size max(1, lda*n. The array a contains either the upper or the lower triangular part of the matrix A (see uplo).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the leading minor of order i (and therefore the matrix A itself) is not positive-definite, and the factorization could not be completed. This may indicate an error in forming the matrix A.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_cholesky_factor(MKL_INT n, MKL_Complex16 a[]) 
	{
		return cholesky_factor(n, a, LAPACKE_zpotrf);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_cholesky_solve(MKL_INT n, MKL_INT nrhs, float a[], float b[])
	{
		return cholesky_solve(n, nrhs, a, b, LAPACKE_spotrf, LAPACKE_spotrs);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_cholesky_solve(MKL_INT n, MKL_INT nrhs, double a[], double b[])
	{
		return cholesky_solve(n, nrhs, a, b, LAPACKE_dpotrf, LAPACKE_dpotrs);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_cholesky_solve(MKL_INT n, MKL_INT nrhs, MKL_Complex8 a[], MKL_Complex8 b[])
	{
		return cholesky_solve(n, nrhs, a, b, LAPACKE_cpotrf, LAPACKE_cpotrs);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_cholesky_solve(MKL_INT n, MKL_INT nrhs, MKL_Complex16 a[], MKL_Complex16 b[])
	{
		return cholesky_solve(n, nrhs, a, b, LAPACKE_zpotrf, LAPACKE_zpotrs);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_cholesky_solve_factored(MKL_INT n, MKL_INT nrhs, float a[], float b[])
	{
		return LAPACKE_spotrs(LAPACK_COL_MAJOR, 'L', n, nrhs, a, n, b, n);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_cholesky_solve_factored(MKL_INT n, MKL_INT nrhs, double a[], double b[])
	{
		return LAPACKE_dpotrs(LAPACK_COL_MAJOR, 'L', n, nrhs, a, n, b, n);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_cholesky_solve_factored(MKL_INT n, MKL_INT nrhs, MKL_Complex8 a[], MKL_Complex8 b[])
	{
		return LAPACKE_cpotrs(LAPACK_COL_MAJOR, 'L', n, nrhs, a, n, b, n);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_cholesky_solve_factored(MKL_INT n, MKL_INT nrhs, MKL_Complex16 a[], MKL_Complex16 b[])
	{
		return LAPACKE_zpotrs(LAPACK_COL_MAJOR, 'L', n, nrhs, a, n, b, n);
	}

	/// <summary>
	/// Computes the QR factorization of a general m-by-n matrix.
	/// Generates the real orthogonal matrix Q of the QR factorization formed by ?geqrf.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="r">Overwritten by the factorization data as follows:
	/// The elements on and above the diagonal of the array contain the min(m, n) - by - n upper trapezoidal matrix R(R is upper triangular if m ? n); the elements below the diagonal, with the array tau, present the orthogonal matrix Q as a product of min(m, n) elementary reflectors(see Orthogonal Factorizations).
	/// Array a of size max(1, lda*n) for column major layout and max(1, lda*m) for row major layout contains the matrix A. </param>
	/// <param name="tau">Array, size at least max (1, min(m, n)). Contains scalars that define elementary reflectors for the matrix Qin its decomposition in a product of elementary reflectors (see Orthogonal Factorizations).</param>
	/// <param name="q">Overwritten by n leading columns of the m-by-m orthogonal matrix Q.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_qr_factor(MKL_INT m, MKL_INT n, float r[], float tau[], float q[])
	{
		return qr_factor(m, n, r, tau, q, LAPACKE_sgeqrf, LAPACKE_sorgqr);
	}

	/// <summary>
	/// Computes the QR factorization of a general m-by-n matrix.
	/// Generates the real orthogonal matrix Q of the QR factorization formed by ?geqrf.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="q">Overwritten by n leading columns of the m-by-m orthogonal matrix Q.</param>
	/// <param name="tau">Array, size at least max (1, min(m, n)). Contains scalars that define elementary reflectors for the matrix Qin its decomposition in a product of elementary reflectors (see Orthogonal Factorizations).</param>
	/// <param name="r">Overwritten by the factorization data as follows:
	/// The elements on and above the diagonal of the array contain the min(m, n) - by - n upper trapezoidal matrix R(R is upper triangular if m ? n); the elements below the diagonal, with the array tau, present the orthogonal matrix Q as a product of min(m, n) elementary reflectors(see Orthogonal Factorizations).
	/// Array a of size max(1, lda*n) for column major layout and max(1, lda*m) for row major layout contains the matrix A. </param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_qr_thin_factor(MKL_INT m, MKL_INT n, float q[], float tau[], float r[])
	{
		return qr_thin_factor(m, n, q, tau, r, LAPACKE_sgeqrf, LAPACKE_sorgqr);
	}

	/// <summary>
	/// Computes the QR factorization of a general m-by-n matrix.
	/// Generates the real orthogonal matrix Q of the QR factorization formed by ?geqrf.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="r">Overwritten by the factorization data as follows:
	/// The elements on and above the diagonal of the array contain the min(m, n) - by - n upper trapezoidal matrix R(R is upper triangular if m ? n); the elements below the diagonal, with the array tau, present the orthogonal matrix Q as a product of min(m, n) elementary reflectors(see Orthogonal Factorizations).
	/// Array a of size max(1, lda*n) for column major layout and max(1, lda*m) for row major layout contains the matrix A. </param>
	/// <param name="tau">Array, size at least max (1, min(m, n)). Contains scalars that define elementary reflectors for the matrix Qin its decomposition in a product of elementary reflectors (see Orthogonal Factorizations).</param>
	/// <param name="q">Overwritten by n leading columns of the m-by-m orthogonal matrix Q.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_qr_factor(MKL_INT m, MKL_INT n, double r[], double tau[], double q[])
	{
		return qr_factor(m, n, r, tau, q, LAPACKE_dgeqrf, LAPACKE_dorgqr);
	}

	/// <summary>
	/// Computes the QR factorization of a general m-by-n matrix.
	/// Generates the real orthogonal matrix Q of the QR factorization formed by ?geqrf.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="q">Overwritten by n leading columns of the m-by-m orthogonal matrix Q.</param>
	/// <param name="tau">Array, size at least max (1, min(m, n)). Contains scalars that define elementary reflectors for the matrix Qin its decomposition in a product of elementary reflectors (see Orthogonal Factorizations).</param>
	/// <param name="r">Overwritten by the factorization data as follows:
	/// The elements on and above the diagonal of the array contain the min(m, n) - by - n upper trapezoidal matrix R(R is upper triangular if m ? n); the elements below the diagonal, with the array tau, present the orthogonal matrix Q as a product of min(m, n) elementary reflectors(see Orthogonal Factorizations).
	/// Array a of size max(1, lda*n) for column major layout and max(1, lda*m) for row major layout contains the matrix A. </param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_qr_thin_factor(MKL_INT m, MKL_INT n, double q[], double tau[], double r[])
	{
		return qr_thin_factor(m, n, q, tau, r, LAPACKE_dgeqrf, LAPACKE_dorgqr);
	}

	/// <summary>
	/// Computes the QR factorization of a general m-by-n matrix.
	/// Generates the real orthogonal matrix Q of the QR factorization formed by ?geqrf.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="r">Overwritten by the factorization data as follows:
	/// The elements on and above the diagonal of the array contain the min(m, n) - by - n upper trapezoidal matrix R(R is upper triangular if m ? n); the elements below the diagonal, with the array tau, present the orthogonal matrix Q as a product of min(m, n) elementary reflectors(see Orthogonal Factorizations).
	/// Array a of size max(1, lda*n) for column major layout and max(1, lda*m) for row major layout contains the matrix A. </param>
	/// <param name="tau">Array, size at least max (1, min(m, n)). Contains scalars that define elementary reflectors for the matrix Qin its decomposition in a product of elementary reflectors (see Orthogonal Factorizations).</param>
	/// <param name="q">Overwritten by n leading columns of the m-by-m orthogonal matrix Q.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_qr_factor(MKL_INT m, MKL_INT n, MKL_Complex8 r[], MKL_Complex8 tau[], MKL_Complex8 q[])
	{
		return qr_factor(m, n, r, tau, q, LAPACKE_cgeqrf, LAPACKE_cungqr);
	}

	/// <summary>
	/// Computes the QR factorization of a general m-by-n matrix.
	/// Generates the real orthogonal matrix Q of the QR factorization formed by ?geqrf.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="q">Overwritten by n leading columns of the m-by-m orthogonal matrix Q.</param>
	/// <param name="tau">Array, size at least max (1, min(m, n)). Contains scalars that define elementary reflectors for the matrix Qin its decomposition in a product of elementary reflectors (see Orthogonal Factorizations).</param>
	/// <param name="r">Overwritten by the factorization data as follows:
	/// The elements on and above the diagonal of the array contain the min(m, n) - by - n upper trapezoidal matrix R(R is upper triangular if m ? n); the elements below the diagonal, with the array tau, present the orthogonal matrix Q as a product of min(m, n) elementary reflectors(see Orthogonal Factorizations).
	/// Array a of size max(1, lda*n) for column major layout and max(1, lda*m) for row major layout contains the matrix A. </param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_qr_thin_factor(MKL_INT m, MKL_INT n, MKL_Complex8 q[], MKL_Complex8 tau[], MKL_Complex8 r[])
	{
		return qr_thin_factor(m, n, q, tau, r, LAPACKE_cgeqrf, LAPACKE_cungqr);
	}

	/// <summary>
	/// Computes the QR factorization of a general m-by-n matrix.
	/// Generates the real orthogonal matrix Q of the QR factorization formed by ?geqrf.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="r">Overwritten by the factorization data as follows:
	/// The elements on and above the diagonal of the array contain the min(m, n) - by - n upper trapezoidal matrix R(R is upper triangular if m ? n); the elements below the diagonal, with the array tau, present the orthogonal matrix Q as a product of min(m, n) elementary reflectors(see Orthogonal Factorizations).
	/// Array a of size max(1, lda*n) for column major layout and max(1, lda*m) for row major layout contains the matrix A. </param>
	/// <param name="tau">Array, size at least max (1, min(m, n)). Contains scalars that define elementary reflectors for the matrix Qin its decomposition in a product of elementary reflectors (see Orthogonal Factorizations).</param>
	/// <param name="q">Overwritten by n leading columns of the m-by-m orthogonal matrix Q.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_qr_factor(MKL_INT m, MKL_INT n, MKL_Complex16 r[], MKL_Complex16 tau[], MKL_Complex16 q[])
	{
		return qr_factor(m, n, r, tau, q, LAPACKE_zgeqrf, LAPACKE_zungqr);
	}

	/// <summary>
	/// Computes the QR factorization of a general m-by-n matrix.
	/// Generates the real orthogonal matrix Q of the QR factorization formed by ?geqrf.
	/// </summary>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="q">Overwritten by n leading columns of the m-by-m orthogonal matrix Q.</param>
	/// <param name="tau">Array, size at least max (1, min(m, n)). Contains scalars that define elementary reflectors for the matrix Qin its decomposition in a product of elementary reflectors (see Orthogonal Factorizations).</param>
	/// <param name="r">Overwritten by the factorization data as follows:
	/// The elements on and above the diagonal of the array contain the min(m, n) - by - n upper trapezoidal matrix R(R is upper triangular if m ? n); the elements below the diagonal, with the array tau, present the orthogonal matrix Q as a product of min(m, n) elementary reflectors(see Orthogonal Factorizations).
	/// Array a of size max(1, lda*n) for column major layout and max(1, lda*m) for row major layout contains the matrix A. </param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_qr_thin_factor(MKL_INT m, MKL_INT n, MKL_Complex16 q[], MKL_Complex16 tau[], MKL_Complex16 r[])
	{
		return qr_thin_factor(m, n, q, tau, r, LAPACKE_zgeqrf, LAPACKE_zungqr);
	}

	/// <summary>
	/// Multiplies a real matrix by the orthogonal matrix Q of the QR factorization formed by ?geqrf or ?geqpf
	/// </summary>
	/// <param name="m">The number of rows in the matrix C (m ? 0).</param>
	/// <param name="n">The number of columns in C (n ? 0).</param>
	/// <param name="bn">The number of elementary reflectors whose product defines the matrix Q. Constraints:
	/// 0 ? k ? m if side ='L';
	/// 0 ? k ? n if side ='R'.
	/// </param>
	/// <param name="r">The size of a is max(1, lda*k) for column major layout, max(1, lda*m) for row major layout and side = 'L', and max(1, lda*n) for row major layout and side = 'R'.</param>
	/// <param name="b">Array c of size max(1, ldc*n) for column major layout and max(1, ldc*m) for row major layout contains the m-by-n matrix C. </param>
	/// <param name="x">Overwritten by the product Q*C, QT*C, C*Q, or C*QT (as specified by side and trans).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_qr_solve(MKL_INT m, MKL_INT n, MKL_INT bn, float r[], float b[], float x[])
	{
		return qr_solve(m, n, bn, r, b, x, LAPACKE_sgels);
	}

	/// <summary>
	/// Multiplies a real matrix by the orthogonal matrix Q of the QR factorization formed by ?geqrf or ?geqpf
	/// </summary>
	/// <param name="m">The number of rows in the matrix C (m ? 0).</param>
	/// <param name="n">The number of columns in C (n ? 0).</param>
	/// <param name="bn">The number of elementary reflectors whose product defines the matrix Q. Constraints:
	/// 0 ? k ? m if side ='L';
	/// 0 ? k ? n if side ='R'.
	/// </param>
	/// <param name="r">The size of a is max(1, lda*k) for column major layout, max(1, lda*m) for row major layout and side = 'L', and max(1, lda*n) for row major layout and side = 'R'.</param>
	/// <param name="b">Array c of size max(1, ldc*n) for column major layout and max(1, ldc*m) for row major layout contains the m-by-n matrix C. </param>
	/// <param name="x">Overwritten by the product Q*C, QT*C, C*Q, or C*QT (as specified by side and trans).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_qr_solve(MKL_INT m, MKL_INT n, MKL_INT bn, double r[], double b[], double x[])
	{
		return qr_solve(m, n, bn, r, b, x, LAPACKE_dgels);
	}

	/// <summary>
	/// Multiplies a real matrix by the orthogonal matrix Q of the QR factorization formed by ?geqrf or ?geqpf
	/// </summary>
	/// <param name="m">The number of rows in the matrix C (m ? 0).</param>
	/// <param name="n">The number of columns in C (n ? 0).</param>
	/// <param name="bn">The number of elementary reflectors whose product defines the matrix Q. Constraints:
	/// 0 ? k ? m if side ='L';
	/// 0 ? k ? n if side ='R'.
	/// </param>
	/// <param name="r">The size of a is max(1, lda*k) for column major layout, max(1, lda*m) for row major layout and side = 'L', and max(1, lda*n) for row major layout and side = 'R'.</param>
	/// <param name="b">Array c of size max(1, ldc*n) for column major layout and max(1, ldc*m) for row major layout contains the m-by-n matrix C. </param>
	/// <param name="x">Overwritten by the product Q*C, QT*C, C*Q, or C*QT (as specified by side and trans).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_qr_solve(MKL_INT m, MKL_INT n, MKL_INT bn, MKL_Complex8 r[], MKL_Complex8 b[], MKL_Complex8 x[])
	{
		return qr_solve(m, n, bn, r, b, x, LAPACKE_cgels);
	}

	/// <summary>
	/// Multiplies a real matrix by the orthogonal matrix Q of the QR factorization formed by ?geqrf or ?geqpf
	/// </summary>
	/// <param name="m">The number of rows in the matrix C (m ? 0).</param>
	/// <param name="n">The number of columns in C (n ? 0).</param>
	/// <param name="bn">The number of elementary reflectors whose product defines the matrix Q. Constraints:
	/// 0 ? k ? m if side ='L';
	/// 0 ? k ? n if side ='R'.
	/// </param>
	/// <param name="r">The size of a is max(1, lda*k) for column major layout, max(1, lda*m) for row major layout and side = 'L', and max(1, lda*n) for row major layout and side = 'R'.</param>
	/// <param name="b">Array c of size max(1, ldc*n) for column major layout and max(1, ldc*m) for row major layout contains the m-by-n matrix C. </param>
	/// <param name="x">Overwritten by the product Q*C, QT*C, C*Q, or C*QT (as specified by side and trans).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_qr_solve(MKL_INT m, MKL_INT n, MKL_INT bn, MKL_Complex16 r[], MKL_Complex16 b[], MKL_Complex16 x[])
	{
		return qr_solve(m, n, bn, r, b, x, LAPACKE_zgels);
	}

	/// <summary>
	/// Multiplies a real matrix by the orthogonal matrix Q of the QR factorization formed by ?geqrf or ?geqpf
	/// </summary>
	/// <param name="m">The number of rows in the matrix C (m ? 0).</param>
	/// <param name="n">The number of columns in C (n ? 0).</param>
	/// <param name="bn">The number of elementary reflectors whose product defines the matrix Q. Constraints:
	/// 0 ? k ? m if side ='L';
	/// 0 ? k ? n if side ='R'.
	/// </param>
	/// <param name="r">The size of a is max(1, lda*k) for column major layout, max(1, lda*m) for row major layout and side = 'L', and max(1, lda*n) for row major layout and side = 'R'.</param>
	/// <param name="b">Array c of size max(1, ldc*n) for column major layout and max(1, ldc*m) for row major layout contains the m-by-n matrix C. </param>
	/// <param name="tau">The size of tau must be at least max(1, k).</param>
	/// <param name="x">Overwritten by the product Q*C, QT*C, C*Q, or C*QT (as specified by side and trans).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_qr_solve_factored(MKL_INT m, MKL_INT n, MKL_INT bn, float r[], float b[], float tau[], float x[])
	{
		return qr_solve_factored(m, n, bn, r, b, tau, x, LAPACKE_sormqr, cblas_strsm);
	}

	/// <summary>
	/// Multiplies a real matrix by the orthogonal matrix Q of the QR factorization formed by ?geqrf or ?geqpf
	/// </summary>
	/// <param name="m">The number of rows in the matrix C (m ? 0).</param>
	/// <param name="n">The number of columns in C (n ? 0).</param>
	/// <param name="bn">The number of elementary reflectors whose product defines the matrix Q. Constraints:
	/// 0 ? k ? m if side ='L';
	/// 0 ? k ? n if side ='R'.
	/// </param>
	/// <param name="r">The size of a is max(1, lda*k) for column major layout, max(1, lda*m) for row major layout and side = 'L', and max(1, lda*n) for row major layout and side = 'R'.</param>
	/// <param name="b">Array c of size max(1, ldc*n) for column major layout and max(1, ldc*m) for row major layout contains the m-by-n matrix C. </param>
	/// <param name="tau">The size of tau must be at least max(1, k).</param>
	/// <param name="x">Overwritten by the product Q*C, QT*C, C*Q, or C*QT (as specified by side and trans).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_qr_solve_factored(MKL_INT m, MKL_INT n, MKL_INT bn, double r[], double b[], double tau[], double x[])
	{
		return qr_solve_factored(m, n, bn, r, b, tau, x, LAPACKE_dormqr, cblas_dtrsm);
	}

	/// <summary>
	/// Multiplies a real matrix by the orthogonal matrix Q of the QR factorization formed by ?geqrf or ?geqpf
	/// </summary>
	/// <param name="m">The number of rows in the matrix C (m ? 0).</param>
	/// <param name="n">The number of columns in C (n ? 0).</param>
	/// <param name="bn">The number of elementary reflectors whose product defines the matrix Q. Constraints:
	/// 0 ? k ? m if side ='L';
	/// 0 ? k ? n if side ='R'.
	/// </param>
	/// <param name="r">The size of a is max(1, lda*k) for column major layout, max(1, lda*m) for row major layout and side = 'L', and max(1, lda*n) for row major layout and side = 'R'.</param>
	/// <param name="b">Array c of size max(1, ldc*n) for column major layout and max(1, ldc*m) for row major layout contains the m-by-n matrix C. </param>
	/// <param name="tau">The size of tau must be at least max(1, k).</param>
	/// <param name="x">Overwritten by the product Q*C, QT*C, C*Q, or C*QT (as specified by side and trans).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_qr_solve_factored(MKL_INT m, MKL_INT n, MKL_INT bn, MKL_Complex8 r[], MKL_Complex8 b[], MKL_Complex8 tau[], MKL_Complex8 x[])
	{
		return complex_qr_solve_factored<lapack_complex_float, float>(m, n, bn, r, b, tau, x, LAPACKE_cunmqr, cblas_ctrsm);
	}

	/// <summary>
	/// Multiplies a real matrix by the orthogonal matrix Q of the QR factorization formed by ?geqrf or ?geqpf
	/// </summary>
	/// <param name="m">The number of rows in the matrix C (m ? 0).</param>
	/// <param name="n">The number of columns in C (n ? 0).</param>
	/// <param name="bn">The number of elementary reflectors whose product defines the matrix Q. Constraints:
	/// 0 ? k ? m if side ='L';
	/// 0 ? k ? n if side ='R'.
	/// </param>
	/// <param name="r">The size of a is max(1, lda*k) for column major layout, max(1, lda*m) for row major layout and side = 'L', and max(1, lda*n) for row major layout and side = 'R'.</param>
	/// <param name="b">Array c of size max(1, ldc*n) for column major layout and max(1, ldc*m) for row major layout contains the m-by-n matrix C. </param>
	/// <param name="tau">The size of tau must be at least max(1, k).</param>
	/// <param name="x">Overwritten by the product Q*C, QT*C, C*Q, or C*QT (as specified by side and trans).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_qr_solve_factored(MKL_INT m, MKL_INT n, MKL_INT bn, MKL_Complex16 r[], MKL_Complex16 b[], MKL_Complex16 tau[], MKL_Complex16 x[])
	{
		return complex_qr_solve_factored<lapack_complex_double, double>(m, n, bn, r, b, tau, x, LAPACKE_zunmqr, cblas_ztrsm);
	}

	/// <summary>
	/// Computes the singular value decomposition of a general rectangular matrix.
	/// The routine computes the singular value decomposition (SVD) of a real/complex m-by-n matrix A, optionally computing the left and/or right singular vectors. The SVD is written as
	/// A = U*?*VT for real routines
	/// A = U*?*VH for complex routines
	/// where ? is an m - by - n matrix which is zero except for its min(m, n) diagonal elements, U is an m - by - m orthogonal / unitary matrix, and V is an n - by - n orthogonal / unitary matrix.The diagonal elements of ? are the singular values of A; they are real and non - negative, and are returned in descending order.The first min(m, n) columns of U and V are the left and right singular vectors of A.
	/// Note that the routine returns VT(for real flavors) or VH(for complex flavors), not V.
	/// </summary>
	/// <param name="compute_vectors">True to compute vectors; else false.</param>
	/// <param name="m">The number of rows of the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="a">Arrays: a(size at least max(1, lda*n) for column major layout and max(1, lda*m) for row major layout) is an array containing the m - by - n matrix A.</param>
	/// <param name="s">Array, size at least max(1, min(m,n)). Contains the singular values of A sorted so that s[i] ? s[i + 1].</param>
	/// <param name="u">Array u minimum size: 
	/// jobu = 'A' max(1, ldu*m) max(1, ldu*m)
	/// jobu = 'S' max(1, ldu*min(m, n)) max(1, ldu*m)
	/// If jobu = 'A', u contains the m - by - m orthogonal / unitary matrix U.
	/// If jobu = 'S', u contains the first min(m, n) columns of U(the left singular vectors stored column - wise).
	/// If jobu = 'N' or 'O', u is not referenced.
	/// </param>
	/// <param name="v">Array v minimum size: 
	/// jobu = 'A' max(1, ldvt*n) max(1, ldvt*n)
	/// jobu = 'S' max(1, ldvt*min(m, n)) max(1, ldvt*n)
	/// If jobvt = 'A', vt contains the n - by - n orthogonal / unitary matrix VT / VH.
	/// If jobvt = 'S', vt contains the first min(m, n) rows of VT / VH(the right singular vectors stored row - wise).
	/// If jobvt = 'N' or 'O', vt is not referenced.
	/// </param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_svd_factor(bool compute_vectors, MKL_INT m, MKL_INT n, float a[], float s[], float u[], float v[])
	{
		return svd_factor(compute_vectors, m, n, a, s, u, v, LAPACKE_sgesvd);
	}

	/// <summary>
	/// Computes the singular value decomposition of a general rectangular matrix.
	/// The routine computes the singular value decomposition (SVD) of a real/complex m-by-n matrix A, optionally computing the left and/or right singular vectors. The SVD is written as
	/// A = U*?*VT for real routines
	/// A = U*?*VH for complex routines
	/// where ? is an m - by - n matrix which is zero except for its min(m, n) diagonal elements, U is an m - by - m orthogonal / unitary matrix, and V is an n - by - n orthogonal / unitary matrix.The diagonal elements of ? are the singular values of A; they are real and non - negative, and are returned in descending order.The first min(m, n) columns of U and V are the left and right singular vectors of A.
	/// Note that the routine returns VT(for real flavors) or VH(for complex flavors), not V.
	/// </summary>
	/// <param name="compute_vectors">True to compute vectors; else false.</param>
	/// <param name="m">The number of rows of the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="a">Arrays: a(size at least max(1, lda*n) for column major layout and max(1, lda*m) for row major layout) is an array containing the m - by - n matrix A.</param>
	/// <param name="s">Array, size at least max(1, min(m,n)). Contains the singular values of A sorted so that s[i] ? s[i + 1].</param>
	/// <param name="u">Array u minimum size: 
	/// jobu = 'A' max(1, ldu*m) max(1, ldu*m)
	/// jobu = 'S' max(1, ldu*min(m, n)) max(1, ldu*m)
	/// If jobu = 'A', u contains the m - by - m orthogonal / unitary matrix U.
	/// If jobu = 'S', u contains the first min(m, n) columns of U(the left singular vectors stored column - wise).
	/// If jobu = 'N' or 'O', u is not referenced.
	/// </param>
	/// <param name="v">Array v minimum size: 
	/// jobu = 'A' max(1, ldvt*n) max(1, ldvt*n)
	/// jobu = 'S' max(1, ldvt*min(m, n)) max(1, ldvt*n)
	/// If jobvt = 'A', vt contains the n - by - n orthogonal / unitary matrix VT / VH.
	/// If jobvt = 'S', vt contains the first min(m, n) rows of VT / VH(the right singular vectors stored row - wise).
	/// If jobvt = 'N' or 'O', vt is not referenced.
	/// </param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_svd_factor(bool compute_vectors, MKL_INT m, MKL_INT n, double a[], double s[], double u[], double v[])
	{
		return svd_factor(compute_vectors, m, n, a, s, u, v, LAPACKE_dgesvd);
	}

	/// <summary>
	/// Computes the singular value decomposition of a general rectangular matrix.
	/// The routine computes the singular value decomposition (SVD) of a real/complex m-by-n matrix A, optionally computing the left and/or right singular vectors. The SVD is written as
	/// A = U*?*VT for real routines
	/// A = U*?*VH for complex routines
	/// where ? is an m - by - n matrix which is zero except for its min(m, n) diagonal elements, U is an m - by - m orthogonal / unitary matrix, and V is an n - by - n orthogonal / unitary matrix.The diagonal elements of ? are the singular values of A; they are real and non - negative, and are returned in descending order.The first min(m, n) columns of U and V are the left and right singular vectors of A.
	/// Note that the routine returns VT(for real flavors) or VH(for complex flavors), not V.
	/// </summary>
	/// <param name="compute_vectors">True to compute vectors; else false.</param>
	/// <param name="m">The number of rows of the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="a">Arrays: a(size at least max(1, lda*n) for column major layout and max(1, lda*m) for row major layout) is an array containing the m - by - n matrix A.</param>
	/// <param name="s">Array, size at least max(1, min(m,n)). Contains the singular values of A sorted so that s[i] ? s[i + 1].</param>
	/// <param name="u">Array u minimum size: 
	/// jobu = 'A' max(1, ldu*m) max(1, ldu*m)
	/// jobu = 'S' max(1, ldu*min(m, n)) max(1, ldu*m)
	/// If jobu = 'A', u contains the m - by - m orthogonal / unitary matrix U.
	/// If jobu = 'S', u contains the first min(m, n) columns of U(the left singular vectors stored column - wise).
	/// If jobu = 'N' or 'O', u is not referenced.
	/// </param>
	/// <param name="v">Array v minimum size: 
	/// jobu = 'A' max(1, ldvt*n) max(1, ldvt*n)
	/// jobu = 'S' max(1, ldvt*min(m, n)) max(1, ldvt*n)
	/// If jobvt = 'A', vt contains the n - by - n orthogonal / unitary matrix VT / VH.
	/// If jobvt = 'S', vt contains the first min(m, n) rows of VT / VH(the right singular vectors stored row - wise).
	/// If jobvt = 'N' or 'O', vt is not referenced.
	/// </param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_svd_factor(bool compute_vectors, MKL_INT m, MKL_INT n, MKL_Complex8 a[], MKL_Complex8 s[], MKL_Complex8 u[], MKL_Complex8 v[])
	{
		return complex_svd_factor<MKL_Complex8, float>(compute_vectors, m, n, a, s, u, v, LAPACKE_cgesvd);
	}

	/// <summary>
	/// Computes the singular value decomposition of a general rectangular matrix.
	/// The routine computes the singular value decomposition (SVD) of a real/complex m-by-n matrix A, optionally computing the left and/or right singular vectors. The SVD is written as
	/// A = U*?*VT for real routines
	/// A = U*?*VH for complex routines
	/// where ? is an m - by - n matrix which is zero except for its min(m, n) diagonal elements, U is an m - by - m orthogonal / unitary matrix, and V is an n - by - n orthogonal / unitary matrix.The diagonal elements of ? are the singular values of A; they are real and non - negative, and are returned in descending order.The first min(m, n) columns of U and V are the left and right singular vectors of A.
	/// Note that the routine returns VT(for real flavors) or VH(for complex flavors), not V.
	/// </summary>
	/// <param name="compute_vectors">True to compute vectors; else false.</param>
	/// <param name="m">The number of rows of the matrix A (m ? 0).</param>
	/// <param name="n">The number of columns in A (n ? 0).</param>
	/// <param name="a">Arrays: a(size at least max(1, lda*n) for column major layout and max(1, lda*m) for row major layout) is an array containing the m - by - n matrix A.</param>
	/// <param name="s">Array, size at least max(1, min(m,n)). Contains the singular values of A sorted so that s[i] ? s[i + 1].</param>
	/// <param name="u">Array u minimum size: 
	/// jobu = 'A' max(1, ldu*m) max(1, ldu*m)
	/// jobu = 'S' max(1, ldu*min(m, n)) max(1, ldu*m)
	/// If jobu = 'A', u contains the m - by - m orthogonal / unitary matrix U.
	/// If jobu = 'S', u contains the first min(m, n) columns of U(the left singular vectors stored column - wise).
	/// If jobu = 'N' or 'O', u is not referenced.
	/// </param>
	/// <param name="v">Array v minimum size: 
	/// jobu = 'A' max(1, ldvt*n) max(1, ldvt*n)
	/// jobu = 'S' max(1, ldvt*min(m, n)) max(1, ldvt*n)
	/// If jobvt = 'A', vt contains the n - by - n orthogonal / unitary matrix VT / VH.
	/// If jobvt = 'S', vt contains the first min(m, n) rows of VT / VH(the right singular vectors stored row - wise).
	/// If jobvt = 'N' or 'O', vt is not referenced.
	/// </param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_svd_factor(bool compute_vectors, MKL_INT m, MKL_INT n, MKL_Complex16 a[], MKL_Complex16 s[], MKL_Complex16 u[], MKL_Complex16 v[])
	{
		return complex_svd_factor<MKL_Complex16, double>(compute_vectors, m, n, a, s, u, v, LAPACKE_zgesvd);
	}

	/// <summary>
	/// Eigensolver interface for standard eigenvalue problem with dense matrices.
	/// The routines compute all the eigenvalues and eigenvectors for standard eigenvalue problems, Ax = ?x, within a given search interval.
	/// </summary>
	/// <param name="isSymmetric">True if is symmetric; else false.</param>
	/// <param name="n">Sets the size of the problem. n > 0.</param>
	/// <param name="a">Array of dimension lda by n, contains either full matrix A or upper or lower triangular part of the matrix A, as specified by uplo.</param>
	/// <param name="vectors">Vectors (size max(1, ldvr*mm) for column major layout and max(1, ldvr*n) for row major layout) 
	/// If howmny = 'B' and side = 'R' or 'B', then vr must contain an n - by - n matrix Q(usually the matrix of Schur vectors returned by ? hseqr). .
	/// If howmny = 'A' or 'S', then vr need not be set.
	/// The array vr is not referenced if side = 'L'.
	/// </param>
	/// <param name="values">The result values.</param>
	/// <param name="d">The resulting eigen factors.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_eigen(bool isSymmetric, lapack_int n, float a[], float vectors[], lapack_complex_double values[], float d[])
	{
		if (isSymmetric)
		{
			return sym_eigen_factor<float>(n, a, vectors, values, d, LAPACKE_ssyev);
		}
		else
		{
			return eigen_factor(n, a, vectors, values, d, LAPACKE_sgees, LAPACKE_strevc);
		}
	}

	/// <summary>
	/// Eigensolver interface for standard eigenvalue problem with dense matrices.
	/// The routines compute all the eigenvalues and eigenvectors for standard eigenvalue problems, Ax = ?x, within a given search interval.
	/// </summary>
	/// <param name="isSymmetric">True if is symmetric; else false.</param>
	/// <param name="n">Sets the size of the problem. n > 0.</param>
	/// <param name="a">Array of dimension lda by n, contains either full matrix A or upper or lower triangular part of the matrix A, as specified by uplo.</param>
	/// <param name="vectors">Vectors (size max(1, ldvr*mm) for column major layout and max(1, ldvr*n) for row major layout) 
	/// If howmny = 'B' and side = 'R' or 'B', then vr must contain an n - by - n matrix Q(usually the matrix of Schur vectors returned by ? hseqr). .
	/// If howmny = 'A' or 'S', then vr need not be set.
	/// The array vr is not referenced if side = 'L'.
	/// </param>
	/// <param name="values">The result values.</param>
	/// <param name="d">The resulting eigen factors.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_eigen(bool isSymmetric, lapack_int n, double a[], double vectors[], lapack_complex_double values[], double d[])
	{
		if (isSymmetric)
		{
			return sym_eigen_factor<double>(n, a, vectors, values, d, LAPACKE_dsyev);
		}
		else
		{
			return eigen_factor(n, a, vectors, values, d, LAPACKE_dgees, LAPACKE_dtrevc);
		}
	}

	/// <summary>
	/// Eigensolver interface for standard eigenvalue problem with dense matrices.
	/// The routines compute all the eigenvalues and eigenvectors for standard eigenvalue problems, Ax = ?x, within a given search interval.
	/// </summary>
	/// <param name="isSymmetric">True if is symmetric; else false.</param>
	/// <param name="n">Sets the size of the problem. n > 0.</param>
	/// <param name="a">Array of dimension lda by n, contains either full matrix A or upper or lower triangular part of the matrix A, as specified by uplo.</param>
	/// <param name="vectors">Vectors (size max(1, ldvr*mm) for column major layout and max(1, ldvr*n) for row major layout) 
	/// If howmny = 'B' and side = 'R' or 'B', then vr must contain an n - by - n matrix Q(usually the matrix of Schur vectors returned by ? hseqr). .
	/// If howmny = 'A' or 'S', then vr need not be set.
	/// The array vr is not referenced if side = 'L'.
	/// </param>
	/// <param name="values">The result values.</param>
	/// <param name="d">The resulting eigen factors.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_eigen(bool isSymmetric, lapack_int n, lapack_complex_float a[], lapack_complex_float vectors[], lapack_complex_double values[], lapack_complex_float d[])
	{
		if (isSymmetric)
		{
			return sym_eigen_factor<float>(n, a, vectors, values, d, LAPACKE_cheev);
		}
		else
		{
			return eigen_complex_factor(n, a, vectors, values, d, LAPACKE_cgees, LAPACKE_ctrevc);
		}
	}

	/// <summary>
	/// Eigensolver interface for standard eigenvalue problem with dense matrices.
	/// The routines compute all the eigenvalues and eigenvectors for standard eigenvalue problems, Ax = ?x, within a given search interval.
	/// </summary>
	/// <param name="isSymmetric">True if is symmetric; else false.</param>
	/// <param name="n">Sets the size of the problem. n > 0.</param>
	/// <param name="a">Array of dimension lda by n, contains either full matrix A or upper or lower triangular part of the matrix A, as specified by uplo.</param>
	/// <param name="vectors">Vectors (size max(1, ldvr*mm) for column major layout and max(1, ldvr*n) for row major layout) 
	/// If howmny = 'B' and side = 'R' or 'B', then vr must contain an n - by - n matrix Q(usually the matrix of Schur vectors returned by ? hseqr). .
	/// If howmny = 'A' or 'S', then vr need not be set.
	/// The array vr is not referenced if side = 'L'.
	/// </param>
	/// <param name="values">The result values.</param>
	/// <param name="d">The resulting eigen factors.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_eigen(bool isSymmetric, lapack_int n, lapack_complex_double a[], lapack_complex_double vectors[], lapack_complex_double values[], lapack_complex_double d[])
	{
		if (isSymmetric)
		{
			return sym_eigen_factor<double>(n, a, vectors, values, d, LAPACKE_zheev);
		}
		else
		{
			return eigen_complex_factor(n, a, vectors, values, d, LAPACKE_zgees, LAPACKE_ztrevc);
		}
	}
}