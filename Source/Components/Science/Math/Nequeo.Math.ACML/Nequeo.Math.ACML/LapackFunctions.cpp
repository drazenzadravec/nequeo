/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          LapackFunctions.h
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

#include "acml.h"

extern "C"
{
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
	EXPORT_NEQUEO_ACML_API int s_lu_factor(int m, float a[], int ipiv[])
	{
		int info = 0;
		sgetrf(m, m, a, m, ipiv, &info);
		for (int i = 0; i < m; ++i) {
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
	EXPORT_NEQUEO_ACML_API int d_lu_factor(int m, double a[], int ipiv[])
	{
		int info = 0;
		dgetrf(m, m, a, m, ipiv, &info);
		for (int i = 0; i < m; ++i) {
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
	EXPORT_NEQUEO_ACML_API int c_lu_factor(int m, complex a[], int ipiv[])
	{
		int info = 0;
		cgetrf(m, m, a, m, ipiv, &info);
		for (int i = 0; i < m; ++i) {
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
	EXPORT_NEQUEO_ACML_API int z_lu_factor(int m, doublecomplex a[], int ipiv[])
	{
		int info = 0;
		zgetrf(m, m, a, m, ipiv, &info);
		for (int i = 0; i < m; ++i) {
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
	EXPORT_NEQUEO_ACML_API int s_lu_inverse(int n, float a[], float work[], int lwork)
	{
		int* ipiv = new int[n];
		int info = 0;
		sgetrf(n, n, a, n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			return info;
		}

		SGETRI(&n, a, &n, ipiv, work, &lwork, &info);
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
	EXPORT_NEQUEO_ACML_API int d_lu_inverse(int n, double a[], double work[], int lwork)
	{
		int* ipiv = new int[n];
		int info = 0;
		dgetrf(n, n, a, n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			return info;
		}

		DGETRI(&n, a, &n, ipiv, work, &lwork, &info);
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
	EXPORT_NEQUEO_ACML_API int c_lu_inverse(int n, complex a[], complex work[], int lwork)
	{
		int* ipiv = new int[n];
		int info = 0;
		cgetrf(n, n, a, n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			return info;
		}

		CGETRI(&n, a, &n, ipiv, work, &lwork, &info);
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
	EXPORT_NEQUEO_ACML_API int z_lu_inverse(int n, doublecomplex a[], doublecomplex work[], int lwork)
	{
		int* ipiv = new int[n];
		int info = 0;
		zgetrf(n, n, a, n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			return info;
		}

		ZGETRI(&n, a, &n, ipiv, work, &lwork, &info);
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
	EXPORT_NEQUEO_ACML_API int s_lu_inverse_factored(int n, float a[], int ipiv[], float work[], int lwork)
	{
		int i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}
		int info = 0;
		SGETRI(&n, a, &n, ipiv, work, &lwork, &info);

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
	EXPORT_NEQUEO_ACML_API int d_lu_inverse_factored(int n, double a[], int ipiv[], double work[], int lwork)
	{
		int i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		int info = 0;
		DGETRI(&n, a, &n, ipiv, work, &lwork, &info);

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
	EXPORT_NEQUEO_ACML_API int c_lu_inverse_factored(int n, complex a[], int ipiv[], complex work[], int lwork)
	{
		int i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		int info = 0;
		CGETRI(&n, a, &n, ipiv, work, &lwork, &info);

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
	EXPORT_NEQUEO_ACML_API int z_lu_inverse_factored(int n, doublecomplex a[], int ipiv[], doublecomplex work[], int lwork)
	{
		int i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		int info = 0;
		ZGETRI(&n, a, &n, ipiv, work, &lwork, &info);

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
	EXPORT_NEQUEO_ACML_API int s_lu_solve_factored(int n, int nrhs, float a[], int ipiv[], float b[])
	{
		int info = 0;
		int i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		char trans = 'N';
		sgetrs(trans, n, nrhs, a, n, ipiv, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int  d_lu_solve_factored(int n, int nrhs, double a[], int ipiv[], double b[])
	{
		int info = 0;
		int i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		char trans = 'N';
		dgetrs(trans, n, nrhs, a, n, ipiv, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int c_lu_solve_factored(int n, int nrhs, complex a[], int ipiv[], complex b[])
	{
		int info = 0;
		int i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		char trans = 'N';
		cgetrs(trans, n, nrhs, a, n, ipiv, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int z_lu_solve_factored(int n, int nrhs, doublecomplex a[], int ipiv[], doublecomplex b[])
	{
		int info = 0;
		int i;
		for (i = 0; i < n; ++i) {
			ipiv[i] += 1;
		}

		char trans = 'N';
		zgetrs(trans, n, nrhs, a, n, ipiv, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int s_lu_solve(int n, int nrhs, float a[], float b[])
	{
		float* clone = new float[n*n];
		std::memcpy(clone, a, n*n * sizeof(float));

		int* ipiv = new int[n];
		int info = 0;
		sgetrf(n, n, clone, n, ipiv, &info);

		if (info != 0)
		{
			delete[] ipiv;
			delete[] clone;
			return info;
		}

		char trans = 'N';
		sgetrs(trans, n, nrhs, clone, n, ipiv, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int d_lu_solve(int n, int nrhs, double a[], double b[])
	{
		double* clone = new double[n*n];
		std::memcpy(clone, a, n*n * sizeof(double));

		int* ipiv = new int[n];
		int info = 0;
		dgetrf(n, n, clone, n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			delete[] clone;
			return info;
		}

		char trans = 'N';
		dgetrs(trans, n, nrhs, clone, n, ipiv, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int c_lu_solve(int n, int nrhs, complex a[], complex b[])
	{
		complex* clone = new complex[n*n];
		std::memcpy(clone, a, n*n * sizeof(complex));

		int* ipiv = new int[n];
		int info = 0;
		cgetrf(n, n, clone, n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			delete[] clone;
			return info;
		}

		char trans = 'N';
		cgetrs(trans, n, nrhs, clone, n, ipiv, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int z_lu_solve(int n, int nrhs, doublecomplex a[], doublecomplex b[])
	{
		doublecomplex* clone = new doublecomplex[n*n];
		std::memcpy(clone, a, n*n * sizeof(doublecomplex));

		int* ipiv = new int[n];
		int info = 0;
		zgetrf(n, n, clone, n, ipiv, &info);

		if (info != 0) {
			delete[] ipiv;
			delete[] clone;
			return info;
		}

		char trans = 'N';
		zgetrs(trans, n, nrhs, clone, n, ipiv, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int s_cholesky_factor(int n, float a[])
	{
		char uplo = 'L';
		int info = 0;
		spotrf(uplo, n, a, n, &info);
		for (int i = 0; i < n; ++i)
		{
			int index = i * n;
			for (int j = 0; j < n && i > j; ++j)
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
	EXPORT_NEQUEO_ACML_API int d_cholesky_factor(int n, double a[])
	{
		char uplo = 'L';
		int info = 0;
		dpotrf(uplo, n, a, n, &info);
		for (int i = 0; i < n; ++i)
		{
			int index = i * n;
			for (int j = 0; j < n && i > j; ++j)
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
	EXPORT_NEQUEO_ACML_API int c_cholesky_factor(int n, complex a[])
	{
		char uplo = 'L';
		int info = 0;
		complex zero = { 0.0f, 0.0f };
		cpotrf(uplo, n, a, n, &info);
		for (int i = 0; i < n; ++i)
		{
			int index = i * n;
			for (int j = 0; j < n && i > j; ++j)
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
	EXPORT_NEQUEO_ACML_API int z_cholesky_factor(int n, doublecomplex a[])
	{
		char uplo = 'L';
		int info = 0;
		doublecomplex zero = { 0.0, 0.0 };
		zpotrf(uplo, n, a, n, &info);
		for (int i = 0; i < n; ++i)
		{
			int index = i * n;
			for (int j = 0; j < n && i > j; ++j)
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
	EXPORT_NEQUEO_ACML_API int s_cholesky_solve(int n, int nrhs, float a[], float b[])
	{
		float* clone = new float[n*n];
		std::memcpy(clone, a, n*n * sizeof(float));
		char uplo = 'L';
		int info = 0;
		spotrf(uplo, n, clone, n, &info);

		if (info != 0) {
			delete[] clone;
			return info;
		}

		spotrs(uplo, n, nrhs, clone, n, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int d_cholesky_solve(int n, int nrhs, double a[], double b[])
	{
		double* clone = new double[n*n];
		std::memcpy(clone, a, n*n * sizeof(double));
		char uplo = 'L';
		int info = 0;
		dpotrf(uplo, n, clone, n, &info);

		if (info != 0) {
			delete[] clone;
			return info;
		}

		dpotrs(uplo, n, nrhs, clone, n, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int c_cholesky_solve(int n, int nrhs, complex a[], complex b[])
	{
		complex* clone = new complex[n*n];
		std::memcpy(clone, a, n*n * sizeof(complex));
		char uplo = 'L';
		int info = 0;
		cpotrf(uplo, n, clone, n, &info);

		if (info != 0) {
			delete[] clone;
			return info;
		}

		cpotrs(uplo, n, nrhs, clone, n, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int z_cholesky_solve(int n, int nrhs, doublecomplex a[], doublecomplex b[])
	{
		doublecomplex* clone = new doublecomplex[n*n];
		std::memcpy(clone, a, n*n * sizeof(doublecomplex));
		char uplo = 'L';
		int info = 0;
		zpotrf(uplo, n, clone, n, &info);

		if (info != 0) {
			delete[] clone;
			return info;
		}

		zpotrs(uplo, n, nrhs, clone, n, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int s_cholesky_solve_factored(int n, int nrhs, float a[], float b[])
	{
		char uplo = 'L';
		int info = 0;
		spotrs(uplo, n, nrhs, a, n, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int d_cholesky_solve_factored(int n, int nrhs, double a[], double b[])
	{
		char uplo = 'L';
		int info = 0;
		dpotrs(uplo, n, nrhs, a, n, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int c_cholesky_solve_factored(int n, int nrhs, complex a[], complex b[])
	{
		char uplo = 'L';
		int info = 0;
		cpotrs(uplo, n, nrhs, a, n, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int z_cholesky_solve_factored(int n, int nrhs, doublecomplex a[], doublecomplex b[])
	{
		char uplo = 'L';
		int info = 0;
		zpotrs(uplo, n, nrhs, a, n, b, n, &info);
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
	EXPORT_NEQUEO_ACML_API int s_qr_factor(int m, int n, float r[], float tau[], float q[], float work[], int len)
	{
		int info = 0;
		SGEQRF(&m, &n, r, &m, tau, work, &len, &info);

		for (int i = 0; i < m; ++i)
		{
			for (int j = 0; j < m && j < n; ++j)
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
			SORGQR(&m, &m, &m, q, &m, tau, work, &len, &info);
		}
		else
		{
			SORGQR(&m, &n, &n, q, &m, tau, work, &len, &info);
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
	EXPORT_NEQUEO_ACML_API int d_qr_factor(int m, int n, double r[], double tau[], double q[], double work[], int len)
	{
		int info = 0;
		DGEQRF(&m, &n, r, &m, tau, work, &len, &info);

		for (int i = 0; i < m; ++i)
		{
			for (int j = 0; j < m && j < n; ++j)
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
			DORGQR(&m, &m, &m, q, &m, tau, work, &len, &info);
		}
		else
		{
			DORGQR(&m, &n, &n, q, &m, tau, work, &len, &info);
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
	EXPORT_NEQUEO_ACML_API int c_qr_factor(int m, int n, complex r[], complex tau[], complex q[], complex work[], int len)
	{
		int info = 0;
		CGEQRF(&m, &n, r, &m, tau, work, &len, &info);

		for (int i = 0; i < m; ++i)
		{
			for (int j = 0; j < m && j < n; ++j)
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
			CUNGQR(&m, &m, &m, q, &m, tau, work, &len, &info);
		}
		else
		{
			CUNGQR(&m, &n, &n, q, &m, tau, work, &len, &info);
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
	EXPORT_NEQUEO_ACML_API int z_qr_factor(int m, int n, doublecomplex r[], doublecomplex tau[], doublecomplex q[], doublecomplex work[], int len)
	{
		int info = 0;
		ZGEQRF(&m, &n, r, &m, tau, work, &len, &info);

		for (int i = 0; i < m; ++i)
		{
			for (int j = 0; j < m && j < n; ++j)
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
			ZUNGQR(&m, &m, &m, q, &m, tau, work, &len, &info);
		}
		else
		{
			ZUNGQR(&m, &n, &n, q, &m, tau, work, &len, &info);
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
	EXPORT_NEQUEO_ACML_API int s_qr_solve(int m, int n, int bn, float r[], float b[], float x[], float work[], int len)
	{
		int info = 0;
		float* clone_r = new float[m*n];
		std::memcpy(clone_r, r, m*n * sizeof(float));

		float* tau = new float[max(1, min(m, n))];
		SGEQRF(&m, &n, clone_r, &m, tau, work, &len, &info);

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
		char upper = 'U';
		char not = 'N';
		SORMQR(&side, &tran, &m, &bn, &n, clone_r, &m, tau, clone_b, &m, work, &len, &info, 1, 1);
		strsm(side, upper, not, not, n, bn, 1.0, clone_r, m, clone_b, m);
		for (int i = 0; i < n; ++i)
		{
			for (int j = 0; j < bn; ++j)
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
	EXPORT_NEQUEO_ACML_API int d_qr_solve(int m, int n, int bn, double r[], double b[], double x[], double work[], int len)
	{
		int info = 0;
		double* clone_r = new double[m*n];
		std::memcpy(clone_r, r, m*n * sizeof(double));

		double* tau = new double[max(1, min(m, n))];
		DGEQRF(&m, &n, clone_r, &m, tau, work, &len, &info);

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
		char upper = 'U';
		char not = 'N';

		DORMQR(&side, &tran, &m, &bn, &n, clone_r, &m, tau, clone_b, &m, work, &len, &info, 1, 1);
		dtrsm(side, upper, not, not, n, bn, 1.0, clone_r, m, clone_b, m);
		for (int i = 0; i < n; ++i)
		{
			for (int j = 0; j < bn; ++j)
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
	EXPORT_NEQUEO_ACML_API int c_qr_solve(int m, int n, int bn, complex r[], complex b[], complex x[], complex work[], int len)
	{
		int info = 0;
		complex* clone_r = new complex[m*n];
		std::memcpy(clone_r, r, m*n * sizeof(complex));

		complex* tau = new complex[min(m, n)];
		CGEQRF(&m, &n, clone_r, &m, tau, work, &len, &info);

		if (info != 0)
		{
			delete[] clone_r;
			delete[] tau;
			return info;
		}

		char side = 'L';
		char tran = 'C';
		char upper = 'U';
		char not = 'N';

		complex* clone_b = new complex[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(complex));

		CUNMQR(&side, &tran, &m, &bn, &n, clone_r, &m, tau, clone_b, &m, work, &len, &info, 1, 1);
		complex one = { 1.0, 0.0 };
		ctrsm(side, upper, not, not, n, bn, &one, clone_r, m, clone_b, m);

		for (int i = 0; i < n; ++i)
		{
			for (int j = 0; j < bn; ++j)
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
	EXPORT_NEQUEO_ACML_API int z_qr_solve(int m, int n, int bn, doublecomplex r[], doublecomplex b[], doublecomplex x[], doublecomplex work[], int len)
	{
		int info = 0;
		doublecomplex* clone_r = new doublecomplex[m*n];
		std::memcpy(clone_r, r, m*n * sizeof(doublecomplex));

		doublecomplex* tau = new doublecomplex[min(m, n)];
		ZGEQRF(&m, &n, clone_r, &m, tau, work, &len, &info);

		if (info != 0)
		{
			delete[] clone_r;
			delete[] tau;
			return info;
		}

		char side = 'L';
		char tran = 'C';
		char upper = 'U';
		char not = 'N';

		doublecomplex* clone_b = new doublecomplex[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(doublecomplex));

		ZUNMQR(&side, &tran, &m, &bn, &n, clone_r, &m, tau, clone_b, &m, work, &len, &info, 1, 1);
		doublecomplex one = { 1.0, 0.0 };
		ztrsm(side, upper, not, not, n, bn, &one, clone_r, m, clone_b, m);

		for (int i = 0; i < n; ++i)
		{
			for (int j = 0; j < bn; ++j)
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
	EXPORT_NEQUEO_ACML_API int s_qr_solve_factored(int m, int n, int bn, float r[], float b[], float tau[], float x[], float work[], int len)
	{
		char side = 'L';
		char tran = 'T';
		char upper = 'U';
		char not = 'N';
		int info = 0;

		float* clone_b = new float[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(float));

		SORMQR(&side, &tran, &m, &bn, &n, r, &m, tau, clone_b, &m, work, &len, &info, 1, 1);
		strsm(side, upper, not, not, n, bn, 1.0, r, m, clone_b, m);
		for (int i = 0; i < n; ++i)
		{
			for (int j = 0; j < bn; ++j)
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
	EXPORT_NEQUEO_ACML_API int d_qr_solve_factored(int m, int n, int bn, double r[], double b[], double tau[], double x[], double work[], int len)
	{
		char side = 'L';
		char tran = 'T';
		char upper = 'U';
		char not = 'N';
		int info = 0;

		double* clone_b = new double[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(double));

		DORMQR(&side, &tran, &m, &bn, &n, r, &m, tau, clone_b, &m, work, &len, &info, 1, 1);
		dtrsm(side, upper, not, not, n, bn, 1.0, r, m, clone_b, m);
		for (int i = 0; i < n; ++i)
		{
			for (int j = 0; j < bn; ++j)
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
	EXPORT_NEQUEO_ACML_API int c_qr_solve_factored(int m, int n, int bn, complex r[], complex b[], complex tau[], complex x[], complex work[], int len)
	{
		char side = 'L';
		char tran = 'C';
		char upper = 'U';
		char not = 'N';
		int info = 0;

		complex* clone_b = new complex[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(complex));

		CUNMQR(&side, &tran, &m, &bn, &n, r, &m, tau, clone_b, &m, work, &len, &info, 1, 1);
		complex one = { 1.0f, 0.0f };
		ctrsm(side, upper, not, not, n, bn, &one, r, m, clone_b, m);
		for (int i = 0; i < n; ++i)
		{
			for (int j = 0; j < bn; ++j)
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
	EXPORT_NEQUEO_ACML_API int z_qr_solve_factored(int m, int n, int bn, doublecomplex r[], doublecomplex b[], doublecomplex tau[], doublecomplex x[], doublecomplex work[], int len)
	{
		char side = 'L';
		char tran = 'C';
		char upper = 'U';
		char not = 'N';
		int info = 0;

		doublecomplex* clone_b = new doublecomplex[m*bn];
		std::memcpy(clone_b, b, m*bn * sizeof(doublecomplex));

		ZUNMQR(&side, &tran, &m, &bn, &n, r, &m, tau, clone_b, &m, work, &len, &info, 1, 1);
		doublecomplex one = { 1.0, 0.0 };
		ztrsm(side, upper, not, not, n, bn, &one, r, m, clone_b, m);

		for (int i = 0; i < n; ++i)
		{
			for (int j = 0; j < bn; ++j)
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
	EXPORT_NEQUEO_ACML_API int s_svd_factor(bool compute_vectors, int m, int n, float a[], float s[], float u[], float v[], float work[], int len)
	{
		int info = 0;
		char job = compute_vectors ? 'A' : 'N';
		SGESVD(&job, &job, &m, &n, a, &m, s, u, &m, v, &n, work, &len, &info, 1, 1);
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
	EXPORT_NEQUEO_ACML_API int d_svd_factor(bool compute_vectors, int m, int n, double a[], double s[], double u[], double v[], double work[], int len)
	{
		int info = 0;
		char job = compute_vectors ? 'A' : 'N';
		DGESVD(&job, &job, &m, &n, a, &m, s, u, &m, v, &n, work, &len, &info, 1, 1);
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
	EXPORT_NEQUEO_ACML_API int c_svd_factor(bool compute_vectors, int m, int n, complex a[], complex s[], complex u[], complex v[], complex work[], int len)
	{
		int info = 0;
		int dim_s = min(m, n);
		float* rwork = new float[5 * dim_s];
		float* s_local = new float[dim_s];
		char job = compute_vectors ? 'A' : 'N';
		CGESVD(&job, &job, &m, &n, a, &m, s_local, u, &m, v, &n, work, &len, rwork, &info, 1, 1);

		for (int index = 0; index < dim_s; ++index) {
			complex value = { s_local[index], 0.0f };
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
	EXPORT_NEQUEO_ACML_API int z_svd_factor(bool compute_vectors, int m, int n, doublecomplex a[], doublecomplex s[], doublecomplex u[], doublecomplex v[], doublecomplex work[], int len)
	{
		int info = 0;
		int dim_s = min(m, n);
		double* rwork = new double[5 * min(m, n)];
		double* s_local = new double[dim_s];
		char job = compute_vectors ? 'A' : 'N';
		ZGESVD(&job, &job, &m, &n, a, &m, s_local, u, &m, v, &n, work, &len, rwork, &info, 1, 1);

		for (int index = 0; index < dim_s; ++index) {
			doublecomplex value = { s_local[index], 0.0f };
			s[index] = value;
		}

		delete[] rwork;
		delete[] s_local;
		return info;
	}
}