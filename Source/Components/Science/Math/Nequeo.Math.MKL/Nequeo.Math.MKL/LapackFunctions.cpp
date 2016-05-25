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
	/// <param name="work">The work matrix.</param>
	/// <returns>Returns the value of the 1-norm, Frobenius norm, infinity-norm, or the largest absolute value of any element of a general rectangular matrix.</returns>
	EXPORT_NEQUEO_MKL_API float s_matrix_norm(char norm, MKL_INT m, MKL_INT n, float a[], float work[])
	{
		return slange_(&norm, &m, &n, a, &m, work);
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
	/// <param name="work">The work matrix.</param>
	/// <returns>Returns the value of the 1-norm, Frobenius norm, infinity-norm, or the largest absolute value of any element of a general rectangular matrix.</returns>
	EXPORT_NEQUEO_MKL_API double d_matrix_norm(char norm, MKL_INT m, MKL_INT n, double a[], double work[])
	{
		return dlange_(&norm, &m, &n, a, &m, work);
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
	/// <param name="work">The work matrix.</param>
	/// <returns>Returns the value of the 1-norm, Frobenius norm, infinity-norm, or the largest absolute value of any element of a general rectangular matrix.</returns>
	EXPORT_NEQUEO_MKL_API float c_matrix_norm(char norm, MKL_INT m, MKL_INT n, MKL_Complex8 a[], float work[])
	{
		return clange_(&norm, &m, &n, a, &m, work);
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
	/// <param name="work">The work matrix.</param>
	/// <returns>Returns the value of the 1-norm, Frobenius norm, infinity-norm, or the largest absolute value of any element of a general rectangular matrix.</returns>
	EXPORT_NEQUEO_MKL_API double z_matrix_norm(char norm, MKL_INT m, MKL_INT n, MKL_Complex16 a[], double work[])
	{
		return zlange_(&norm, &m, &n, a, &m, work);
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
		MKL_INT info = 0;
		sgetrf_(&m, &m, a, &m, ipiv, &info);
		for (MKL_INT i = 0; i < m; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		MKL_INT info = 0;
		dgetrf_(&m, &m, a, &m, ipiv, &info);
		for (MKL_INT i = 0; i < m; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		MKL_INT info = 0;
		cgetrf_(&m, &m, a, &m, ipiv, &info);
		for (MKL_INT i = 0; i < m; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		MKL_INT info = 0;
		zgetrf_(&m, &m, a, &m, ipiv, &info);
		for (MKL_INT i = 0; i < m; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		MKL_INT* ipiv = new MKL_INT[n];
		MKL_INT info = 0;
		sgetrf_(&n, &n, a, &n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			return info;
		}

		sgetri_(&n, a, &n, ipiv, work, &lwork, &info);
		delete[] ipiv;
		return info;
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
		MKL_INT* ipiv = new MKL_INT[n];
		MKL_INT info = 0;
		dgetrf_(&n, &n, a, &n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			return info;
		}

		dgetri_(&n, a, &n, ipiv, work, &lwork, &info);
		delete[] ipiv;
		return info;
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
		MKL_INT* ipiv = new MKL_INT[n];
		MKL_INT info = 0;
		cgetrf_(&n, &n, a, &n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			return info;
		}

		cgetri_(&n, a, &n, ipiv, work, &lwork, &info);
		delete[] ipiv;
		return info;
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
		MKL_INT* ipiv = new MKL_INT[n];
		MKL_INT info = 0;
		zgetrf_(&n, &n, a, &n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			return info;
		}

		zgetri_(&n, a, &n, ipiv, work, &lwork, &info);
		delete[] ipiv;
		return info;
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
		MKL_INT i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}
		MKL_INT info = 0;
		sgetri_(&n, a, &n, ipiv, work, &lwork, &info);

		for (i = 0; i < n; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		MKL_INT i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		MKL_INT info = 0;
		dgetri_(&n, a, &n, ipiv, work, &lwork, &info);

		for (i = 0; i < n; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		MKL_INT i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		MKL_INT info = 0;
		cgetri_(&n, a, &n, ipiv, work, &lwork, &info);

		for (i = 0; i < n; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		MKL_INT i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		MKL_INT info = 0;
		zgetri_(&n, a, &n, ipiv, work, &lwork, &info);

		for (i = 0; i < n; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		MKL_INT info = 0;
		MKL_INT i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		char trans = 'N';
		sgetrs_(&trans, &n, &nrhs, a, &n, ipiv, b, &n, &info);
		for (i = 0; i < n; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		MKL_INT info = 0;
		MKL_INT i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		char trans = 'N';
		dgetrs_(&trans, &n, &nrhs, a, &n, ipiv, b, &n, &info);
		for (i = 0; i < n; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		MKL_INT info = 0;
		MKL_INT i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		char trans = 'N';
		cgetrs_(&trans, &n, &nrhs, a, &n, ipiv, b, &n, &info);
		for (i = 0; i < n; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		MKL_INT info = 0;
		MKL_INT i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		char trans = 'N';
		zgetrs_(&trans, &n, &nrhs, a, &n, ipiv, b, &n, &info);
		for (i = 0; i < n; ++i) {
			ipiv[i] -= 1;
		}
		return info;
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
		float* clone = new float[n*n];
		std::memcpy(clone, a, n*n * sizeof(float));

		MKL_INT* ipiv = new MKL_INT[n];
		MKL_INT info = 0;
		sgetrf_(&n, &n, clone, &n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			delete[] clone;
			return info;
		}

		char trans = 'N';
		sgetrs_(&trans, &n, &nrhs, clone, &n, ipiv, b, &n, &info);
		delete[] ipiv;
		delete[] clone;
		return info;
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
		double* clone = new double[n*n];
		std::memcpy(clone, a, n*n * sizeof(double));

		MKL_INT* ipiv = new MKL_INT[n];
		MKL_INT info = 0;
		dgetrf_(&n, &n, clone, &n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			delete[] clone;
			return info;
		}

		char trans = 'N';
		dgetrs_(&trans, &n, &nrhs, clone, &n, ipiv, b, &n, &info);
		delete[] ipiv;
		delete[] clone;
		return info;
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
		MKL_Complex8* clone = new MKL_Complex8[n*n];
		std::memcpy(clone, a, n*n * sizeof(MKL_Complex8));

		MKL_INT* ipiv = new MKL_INT[n];
		MKL_INT info = 0;
		cgetrf_(&n, &n, clone, &n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			delete[] clone;
			return info;
		}

		char trans = 'N';
		cgetrs_(&trans, &n, &nrhs, clone, &n, ipiv, b, &n, &info);
		delete[] ipiv;
		delete[] clone;
		return info;
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
		MKL_Complex16* clone = new MKL_Complex16[n*n];
		std::memcpy(clone, a, n*n * sizeof(MKL_Complex16));

		MKL_INT* ipiv = new MKL_INT[n];
		MKL_INT info = 0;
		zgetrf_(&n, &n, clone, &n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			delete[] clone;
			return info;
		}

		char trans = 'N';
		zgetrs_(&trans, &n, &nrhs, clone, &n, ipiv, b, &n, &info);
		delete[] ipiv;
		delete[] clone;
		return info;
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
	EXPORT_NEQUEO_MKL_API MKL_INT s_cholesky_factor(MKL_INT n, float a[]) {
		char uplo = 'L';
		MKL_INT info = 0;
		spotrf_(&uplo, &n, a, &n, &info);
		for (MKL_INT i = 0; i < n; ++i)
		{
			MKL_INT index = i * n;
			for (MKL_INT j = 0; j < n && i > j; ++j)
			{
				a[index + j] = 0;
			}
		}
		return info;
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
	EXPORT_NEQUEO_MKL_API MKL_INT d_cholesky_factor(MKL_INT n, double a[]) {
		char uplo = 'L';
		MKL_INT info = 0;
		dpotrf_(&uplo, &n, a, &n, &info);
		for (MKL_INT i = 0; i < n; ++i)
		{
			MKL_INT index = i * n;
			for (MKL_INT j = 0; j < n && i > j; ++j)
			{
				a[index + j] = 0;
			}
		}
		return info;
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
	EXPORT_NEQUEO_MKL_API MKL_INT c_cholesky_factor(MKL_INT n, MKL_Complex8 a[]) {
		char uplo = 'L';
		MKL_INT info = 0;
		MKL_Complex8 zero = { 0.0f, 0.0f };
		cpotrf_(&uplo, &n, a, &n, &info);
		for (MKL_INT i = 0; i < n; ++i)
		{
			MKL_INT index = i * n;
			for (MKL_INT j = 0; j < n && i > j; ++j)
			{
				a[index + j] = zero;
			}
		}
		return info;
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
	EXPORT_NEQUEO_MKL_API MKL_INT z_cholesky_factor(MKL_INT n, MKL_Complex16 a[]) {
		char uplo = 'L';
		MKL_INT info = 0;
		MKL_Complex16 zero = { 0.0, 0.0 };
		zpotrf_(&uplo, &n, a, &n, &info);
		for (MKL_INT i = 0; i < n; ++i)
		{
			MKL_INT index = i * n;
			for (MKL_INT j = 0; j < n && i > j; ++j)
			{
				a[index + j] = zero;
			}
		}
		return info;
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
		float* clone = new float[n*n];
		std::memcpy(clone, a, n*n * sizeof(float));
		char uplo = 'L';
		MKL_INT info = 0;
		spotrf_(&uplo, &n, clone, &n, &info);

		if (info != 0) {
			delete[] clone;
			return info;
		}

		spotrs_(&uplo, &n, &nrhs, clone, &n, b, &n, &info);
		return info;
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
		double* clone = new double[n*n];
		std::memcpy(clone, a, n*n * sizeof(double));
		char uplo = 'L';
		MKL_INT info = 0;
		dpotrf_(&uplo, &n, clone, &n, &info);

		if (info != 0) {
			delete[] clone;
			return info;
		}

		dpotrs_(&uplo, &n, &nrhs, clone, &n, b, &n, &info);
		return info;
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
		MKL_Complex8* clone = new MKL_Complex8[n*n];
		std::memcpy(clone, a, n*n * sizeof(MKL_Complex8));
		char uplo = 'L';
		MKL_INT info = 0;
		cpotrf_(&uplo, &n, clone, &n, &info);

		if (info != 0) {
			delete[] clone;
			return info;
		}

		cpotrs_(&uplo, &n, &nrhs, clone, &n, b, &n, &info);
		return info;
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
		MKL_Complex16* clone = new MKL_Complex16[n*n];
		std::memcpy(clone, a, n*n * sizeof(MKL_Complex16));
		char uplo = 'L';
		MKL_INT info = 0;
		zpotrf_(&uplo, &n, clone, &n, &info);

		if (info != 0) {
			delete[] clone;
			return info;
		}

		zpotrs_(&uplo, &n, &nrhs, clone, &n, b, &n, &info);
		return info;
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
		char uplo = 'L';
		MKL_INT info = 0;
		spotrs_(&uplo, &n, &nrhs, a, &n, b, &n, &info);
		return info;
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
		char uplo = 'L';
		MKL_INT info = 0;
		dpotrs_(&uplo, &n, &nrhs, a, &n, b, &n, &info);
		return info;
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
		char uplo = 'L';
		MKL_INT info = 0;
		cpotrs_(&uplo, &n, &nrhs, a, &n, b, &n, &info);
		return info;
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
		char uplo = 'L';
		MKL_INT info = 0;
		zpotrs_(&uplo, &n, &nrhs, a, &n, b, &n, &info);
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_qr_factor(MKL_INT m, MKL_INT n, float r[], float tau[], float q[], float work[], MKL_INT len)
	{
		MKL_INT info = 0;
		sgeqrf_(&m, &n, r, &m, tau, work, &len, &info);

		for (MKL_INT i = 0; i < m; ++i)
		{
			for (MKL_INT j = 0; j < m && j < n; ++j)
			{
				if (i > j)
				{
					q[j * m + i] = r[j * m + i];
				}
			}
		}

		//compute the q elements explicitly
		if (m <= n)
		{
			sorgqr_(&m, &m, &m, q, &m, tau, work, &len, &info);
		}
		else
		{
			sorgqr_(&m, &n, &n, q, &m, tau, work, &len, &info);
		}

		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_qr_factor(MKL_INT m, MKL_INT n, double r[], double tau[], double q[], double work[], MKL_INT len)
	{
		MKL_INT info = 0;
		dgeqrf_(&m, &n, r, &m, tau, work, &len, &info);

		for (MKL_INT i = 0; i < m; ++i)
		{
			for (MKL_INT j = 0; j < m && j < n; ++j)
			{
				if (i > j)
				{
					q[j * m + i] = r[j * m + i];
				}
			}
		}

		//compute the q elements explicitly
		if (m <= n)
		{
			dorgqr_(&m, &m, &m, q, &m, tau, work, &len, &info);
		}
		else
		{
			dorgqr_(&m, &n, &n, q, &m, tau, work, &len, &info);
		}

		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_qr_factor(MKL_INT m, MKL_INT n, MKL_Complex8 r[], MKL_Complex8 tau[], MKL_Complex8 q[], MKL_Complex8 work[], MKL_INT len)
	{
		MKL_INT info = 0;
		cgeqrf_(&m, &n, r, &m, tau, work, &len, &info);

		for (MKL_INT i = 0; i < m; ++i)
		{
			for (MKL_INT j = 0; j < m && j < n; ++j)
			{
				if (i > j)
				{
					q[j * m + i] = r[j * m + i];
				}
			}
		}

		//compute the q elements explicitly
		if (m <= n)
		{
			cungqr_(&m, &m, &m, q, &m, tau, work, &len, &info);
		}
		else
		{
			cungqr_(&m, &n, &n, q, &m, tau, work, &len, &info);
		}

		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_qr_factor(MKL_INT m, MKL_INT n, MKL_Complex16 r[], MKL_Complex16 tau[], MKL_Complex16 q[], MKL_Complex16 work[], MKL_INT len)
	{
		MKL_INT info = 0;
		zgeqrf_(&m, &n, r, &m, tau, work, &len, &info);

		for (MKL_INT i = 0; i < m; ++i)
		{
			for (MKL_INT j = 0; j < m && j < n; ++j)
			{
				if (i > j)
				{
					q[j * m + i] = r[j * m + i];
				}
			}
		}

		//compute the q elements explicitly
		if (m <= n)
		{
			zungqr_(&m, &m, &m, q, &m, tau, work, &len, &info);
		}
		else
		{
			zungqr_(&m, &n, &n, q, &m, tau, work, &len, &info);
		}

		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_qr_solve(MKL_INT m, MKL_INT n, MKL_INT bn, float r[], float b[], float x[], float work[], MKL_INT len)
	{
		MKL_INT info = 0;
		float* clone_r = new float[m*n];
		std::memcpy(clone_r, r, m*n * sizeof(float));

		float* tau = new float[max(1, min(m, n))];
		sgeqrf_(&m, &n, clone_r, &m, tau, work, &len, &info);

		if (info != 0)
		{
			delete[] clone_r;
			delete[] tau;
			return info;
		}

		float* clone_b = new float[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(float));

		char side = 'L';
		char tran = 'T';
		sormqr_(&side, &tran, &m, &bn, &n, clone_r, &m, tau, clone_b, &m, work, &len, &info);
		cblas_strsm(CblasColMajor, CblasLeft, CblasUpper, CblasNoTrans, CblasNonUnit, n, bn, 1.0, clone_r, m, clone_b, m);
		for (MKL_INT i = 0; i < n; ++i)
		{
			for (MKL_INT j = 0; j < bn; ++j)
			{
				x[j * n + i] = clone_b[j * m + i];
			}
		}

		delete[] clone_r;
		delete[] tau;
		delete[] clone_b;
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_qr_solve(MKL_INT m, MKL_INT n, MKL_INT bn, double r[], double b[], double x[], double work[], MKL_INT len)
	{
		MKL_INT info = 0;
		double* clone_r = new double[m*n];
		std::memcpy(clone_r, r, m*n * sizeof(double));

		double* tau = new double[max(1, min(m, n))];
		dgeqrf_(&m, &n, clone_r, &m, tau, work, &len, &info);

		if (info != 0)
		{
			delete[] clone_r;
			delete[] tau;
			return info;
		}

		double* clone_b = new double[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(double));

		char side = 'L';
		char tran = 'T';

		dormqr_(&side, &tran, &m, &bn, &n, clone_r, &m, tau, clone_b, &m, work, &len, &info);
		cblas_dtrsm(CblasColMajor, CblasLeft, CblasUpper, CblasNoTrans, CblasNonUnit, n, bn, 1.0, clone_r, m, clone_b, m);
		for (MKL_INT i = 0; i < n; ++i)
		{
			for (MKL_INT j = 0; j < bn; ++j)
			{
				x[j * n + i] = clone_b[j * m + i];
			}
		}

		delete[] clone_b;
		delete[] tau;
		delete[] clone_r;
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_qr_solve(MKL_INT m, MKL_INT n, MKL_INT bn, MKL_Complex8 r[], MKL_Complex8 b[], MKL_Complex8 x[], MKL_Complex8 work[], MKL_INT len)
	{
		MKL_INT info = 0;
		MKL_Complex8* clone_r = new MKL_Complex8[m*n];
		std::memcpy(clone_r, r, m*n * sizeof(MKL_Complex8));

		MKL_Complex8* tau = new MKL_Complex8[min(m, n)];
		cgeqrf_(&m, &n, clone_r, &m, tau, work, &len, &info);

		if (info != 0)
		{
			delete[] clone_r;
			delete[] tau;
			return info;
		}

		char side = 'L';
		char tran = 'C';

		MKL_Complex8* clone_b = new MKL_Complex8[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(MKL_Complex8));

		cunmqr_(&side, &tran, &m, &bn, &n, clone_r, &m, tau, clone_b, &m, work, &len, &info);
		MKL_Complex8 one = { 1.0, 0.0 };
		cblas_ctrsm(CblasColMajor, CblasLeft, CblasUpper, CblasNoTrans, CblasNonUnit, n, bn, &one, clone_r, m, clone_b, m);

		for (MKL_INT i = 0; i < n; ++i)
		{
			for (MKL_INT j = 0; j < bn; ++j)
			{
				x[j * n + i] = clone_b[j * m + i];
			}
		}

		delete[] clone_r;
		delete[] tau;
		delete[] clone_b;
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_qr_solve(MKL_INT m, MKL_INT n, MKL_INT bn, MKL_Complex16 r[], MKL_Complex16 b[], MKL_Complex16 x[], MKL_Complex16 work[], MKL_INT len)
	{
		MKL_INT info = 0;
		MKL_Complex16* clone_r = new MKL_Complex16[m*n];
		std::memcpy(clone_r, r, m*n * sizeof(MKL_Complex16));

		MKL_Complex16* tau = new MKL_Complex16[min(m, n)];
		zgeqrf_(&m, &n, clone_r, &m, tau, work, &len, &info);

		if (info != 0)
		{
			delete[] clone_r;
			delete[] tau;
			return info;
		}

		char side = 'L';
		char tran = 'C';

		MKL_Complex16* clone_b = new MKL_Complex16[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(MKL_Complex16));

		zunmqr_(&side, &tran, &m, &bn, &n, clone_r, &m, tau, clone_b, &m, work, &len, &info);
		MKL_Complex16 one = { 1.0, 0.0 };
		cblas_ztrsm(CblasColMajor, CblasLeft, CblasUpper, CblasNoTrans, CblasNonUnit, n, bn, &one, clone_r, m, clone_b, m);

		for (MKL_INT i = 0; i < n; ++i)
		{
			for (MKL_INT j = 0; j < bn; ++j)
			{
				x[j * n + i] = clone_b[j * m + i];
			}
		}

		delete[] clone_r;
		delete[] tau;
		delete[] clone_b;
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_qr_solve_factored(MKL_INT m, MKL_INT n, MKL_INT bn, float r[], float b[], float tau[], float x[], float work[], MKL_INT len)
	{
		char side = 'L';
		char tran = 'T';
		MKL_INT info = 0;

		float* clone_b = new float[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(float));

		sormqr_(&side, &tran, &m, &bn, &n, r, &m, tau, clone_b, &m, work, &len, &info);
		cblas_strsm(CblasColMajor, CblasLeft, CblasUpper, CblasNoTrans, CblasNonUnit, n, bn, 1.0, r, m, clone_b, m);
		for (MKL_INT i = 0; i < n; ++i)
		{
			for (MKL_INT j = 0; j < bn; ++j)
			{
				x[j * n + i] = clone_b[j * m + i];
			}
		}

		delete[] clone_b;
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_qr_solve_factored(MKL_INT m, MKL_INT n, MKL_INT bn, double r[], double b[], double tau[], double x[], double work[], MKL_INT len)
	{
		char side = 'L';
		char tran = 'T';
		MKL_INT info = 0;

		double* clone_b = new double[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(double));

		dormqr_(&side, &tran, &m, &bn, &n, r, &m, tau, clone_b, &m, work, &len, &info);
		cblas_dtrsm(CblasColMajor, CblasLeft, CblasUpper, CblasNoTrans, CblasNonUnit, n, bn, 1.0, r, m, clone_b, m);
		for (MKL_INT i = 0; i < n; ++i)
		{
			for (MKL_INT j = 0; j < bn; ++j)
			{
				x[j * n + i] = clone_b[j * m + i];
			}
		}

		delete[] clone_b;
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_qr_solve_factored(MKL_INT m, MKL_INT n, MKL_INT bn, MKL_Complex8 r[], MKL_Complex8 b[], MKL_Complex8 tau[], MKL_Complex8 x[], MKL_Complex8 work[], MKL_INT len)
	{
		char side = 'L';
		char tran = 'C';
		MKL_INT info = 0;

		MKL_Complex8* clone_b = new MKL_Complex8[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(MKL_Complex8));

		cunmqr_(&side, &tran, &m, &bn, &n, r, &m, tau, clone_b, &m, work, &len, &info);
		MKL_Complex8 one = { 1.0f, 0.0f };
		cblas_ctrsm(CblasColMajor, CblasLeft, CblasUpper, CblasNoTrans, CblasNonUnit, n, bn, &one, r, m, clone_b, m);
		for (MKL_INT i = 0; i < n; ++i)
		{
			for (MKL_INT j = 0; j < bn; ++j)
			{
				x[j * n + i] = clone_b[j * m + i];
			}
		}

		delete[] clone_b;
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_qr_solve_factored(MKL_INT m, MKL_INT n, MKL_INT bn, MKL_Complex16 r[], MKL_Complex16 b[], MKL_Complex16 tau[], MKL_Complex16 x[], MKL_Complex16 work[], MKL_INT len)
	{
		char side = 'L';
		char tran = 'C';
		MKL_INT info = 0;

		MKL_Complex16* clone_b = new MKL_Complex16[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(MKL_Complex16));

		zunmqr_(&side, &tran, &m, &bn, &n, r, &m, tau, clone_b, &m, work, &len, &info);
		MKL_Complex16 one = { 1.0, 0.0 };
		cblas_ztrsm(CblasColMajor, CblasLeft, CblasUpper, CblasNoTrans, CblasNonUnit, n, bn, &one, r, m, clone_b, m);

		for (MKL_INT i = 0; i < n; ++i)
		{
			for (MKL_INT j = 0; j < bn; ++j)
			{
				x[j * n + i] = clone_b[j * m + i];
			}
		}

		delete[] clone_b;
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT s_svd_factor(bool compute_vectors, MKL_INT m, MKL_INT n, float a[], float s[], float u[], float v[], float work[], MKL_INT len)
	{
		MKL_INT info = 0;
		char job = compute_vectors ? 'A' : 'N';
		sgesvd_(&job, &job, &m, &n, a, &m, s, u, &m, v, &n, work, &len, &info);
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT d_svd_factor(bool compute_vectors, MKL_INT m, MKL_INT n, double a[], double s[], double u[], double v[], double work[], MKL_INT len)
	{
		MKL_INT info = 0;
		char job = compute_vectors ? 'A' : 'N';
		dgesvd_(&job, &job, &m, &n, a, &m, s, u, &m, v, &n, work, &len, &info);
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT c_svd_factor(bool compute_vectors, MKL_INT m, MKL_INT n, MKL_Complex8 a[], MKL_Complex8 s[], MKL_Complex8 u[], MKL_Complex8 v[], MKL_Complex8 work[], MKL_INT len)
	{
		MKL_INT info = 0;
		MKL_INT dim_s = min(m, n);
		float* rwork = new float[5 * dim_s];
		float* s_local = new float[dim_s];
		char job = compute_vectors ? 'A' : 'N';
		cgesvd_(&job, &job, &m, &n, a, &m, s_local, u, &m, v, &n, work, &len, rwork, &info);

		for (MKL_INT index = 0; index < dim_s; ++index) {
			MKL_Complex8 value = { s_local[index], 0.0f };
			s[index] = value;
		}

		delete[] rwork;
		delete[] s_local;
		return info;
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
	/// <param name="work">The work.</param>
	/// <param name="len">The leading dimension of a; at least max(1, m)for column major layout and max(1, n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
	/// </returns>
	EXPORT_NEQUEO_MKL_API MKL_INT z_svd_factor(bool compute_vectors, MKL_INT m, MKL_INT n, MKL_Complex16 a[], MKL_Complex16 s[], MKL_Complex16 u[], MKL_Complex16 v[], MKL_Complex16 work[], MKL_INT len)
	{
		MKL_INT info = 0;
		MKL_INT dim_s = min(m, n);
		double* rwork = new double[5 * min(m, n)];
		double* s_local = new double[dim_s];
		char job = compute_vectors ? 'A' : 'N';
		zgesvd_(&job, &job, &m, &n, a, &m, s_local, u, &m, v, &n, work, &len, rwork, &info);

		for (MKL_INT index = 0; index < dim_s; ++index) {
			MKL_Complex16 value = { s_local[index], 0.0f };
			s[index] = value;
		}

		delete[] rwork;
		delete[] s_local;
		return info;
	}
}