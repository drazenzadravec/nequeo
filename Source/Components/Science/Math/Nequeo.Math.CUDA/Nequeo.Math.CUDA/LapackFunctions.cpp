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

#include "GlobalCUDA.h"
#include "LapackTemplates.cpp"

#include "cublas_v2.h"
#include "cusolverDn.h"
#include "cuda_runtime.h"

extern "C" 
{
	/// <summary>
	/// Computes the LU factorization of a general m-by-n matrix. A = P*L*U.
	/// where P is a permutation matrix, L is lower triangular with unit diagonal elements (lower trapezoidal if m > n) and U is upper triangular (upper trapezoidal if m < n). The routine uses partial pivoting, with row interchanges.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column-major layout or max(1, lda*m) for row-major layout. Contains the matrix A.</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info=0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, uii is 0. The factorization has been completed, but U is exactly singular. Division by 0 will occur if you use the factor U for solving a system of linear equations.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int s_lu_factor(cusolverDnHandle_t solverHandle, int m, float a[], int ipiv[])
	{
		return lu_factor(solverHandle, m, a, ipiv, sgetrf, sgetrfbsize);
	}

	/// <summary>
	/// Computes the LU factorization of a general m-by-n matrix. A = P*L*U.
	/// where P is a permutation matrix, L is lower triangular with unit diagonal elements (lower trapezoidal if m > n) and U is upper triangular (upper trapezoidal if m < n). The routine uses partial pivoting, with row interchanges.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column-major layout or max(1, lda*m) for row-major layout. Contains the matrix A.</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info=0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, uii is 0. The factorization has been completed, but U is exactly singular. Division by 0 will occur if you use the factor U for solving a system of linear equations.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int d_lu_factor(cusolverDnHandle_t solverHandle, int m, double a[], int ipiv[])
	{
		return lu_factor(solverHandle, m, a, ipiv, dgetrf, dgetrfbsize);
	}

	/// <summary>
	/// Computes the LU factorization of a general m-by-n matrix. A = P*L*U.
	/// where P is a permutation matrix, L is lower triangular with unit diagonal elements (lower trapezoidal if m > n) and U is upper triangular (upper trapezoidal if m < n). The routine uses partial pivoting, with row interchanges.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column-major layout or max(1, lda*m) for row-major layout. Contains the matrix A.</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info=0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, uii is 0. The factorization has been completed, but U is exactly singular. Division by 0 will occur if you use the factor U for solving a system of linear equations.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int c_lu_factor(cusolverDnHandle_t solverHandle, int m, cuComplex a[], int ipiv[])
	{
		return lu_factor(solverHandle, m, a, ipiv, cgetrf, cgetrfbsize);
	}

	/// <summary>
	/// Computes the LU factorization of a general m-by-n matrix. A = P*L*U.
	/// where P is a permutation matrix, L is lower triangular with unit diagonal elements (lower trapezoidal if m > n) and U is upper triangular (upper trapezoidal if m < n). The routine uses partial pivoting, with row interchanges.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
	/// <param name="a">Array, size at least max(1, lda*n) for column-major layout or max(1, lda*m) for row-major layout. Contains the matrix A.</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info=0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, uii is 0. The factorization has been completed, but U is exactly singular. Division by 0 will occur if you use the factor U for solving a system of linear equations.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int z_lu_factor(cusolverDnHandle_t solverHandle, int m, cuDoubleComplex a[], int ipiv[])
	{
		return lu_factor(solverHandle, m, a, ipiv, zgetrf, zgetrfbsize);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int s_lu_inverse(cusolverDnHandle_t solverHandle, cublasHandle_t blasHandle, int n, float a[])
	{
		return lu_inverse(solverHandle, blasHandle, n, a, sgetrf, sgetribatched, sgetrfbsize);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int d_lu_inverse(cusolverDnHandle_t solverHandle, cublasHandle_t blasHandle, int n, double a[])
	{
		return lu_inverse(solverHandle, blasHandle, n, a, dgetrf, dgetribatched, dgetrfbsize);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int c_lu_inverse(cusolverDnHandle_t solverHandle, cublasHandle_t blasHandle, int n, cuComplex a[])
	{
		return lu_inverse(solverHandle, blasHandle, n, a, cgetrf, cgetribatched, cgetrfbsize);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int z_lu_inverse(cusolverDnHandle_t solverHandle, cublasHandle_t blasHandle, int n, cuDoubleComplex a[])
	{
		return lu_inverse(solverHandle, blasHandle, n, a, zgetrf, zgetribatched, zgetrfbsize);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int s_lu_inverse_factored(cublasHandle_t blasHandle, int n, float a[], int ipiv[])
	{
		return lu_inverse_factored(blasHandle, n, a, ipiv, sgetribatched);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int d_lu_inverse_factored(cublasHandle_t blasHandle, int n, double a[], int ipiv[])
	{
		return lu_inverse_factored(blasHandle, n, a, ipiv, dgetribatched);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int c_lu_inverse_factored(cublasHandle_t blasHandle, int n, cuComplex a[], int ipiv[])
	{
		return lu_inverse_factored(blasHandle, n, a, ipiv, cgetribatched);
	}

	/// <summary>
	/// Computes the inverse of an LU-factored general matrix.
	/// The routine computes the inverse inv(A) of a general matrix A.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">The order of the matrix A; n ? 0.</param>
	/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
	/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful. 
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int z_lu_inverse_factored(cublasHandle_t blasHandle, int n, cuDoubleComplex a[], int ipiv[])
	{
		return lu_inverse_factored(blasHandle, n, a, ipiv, zgetribatched);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="ipiv">Array, size at least max(1, n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int s_lu_solve_factored(cusolverDnHandle_t solverHandle, int n, int nrhs, float a[], int ipiv[], float b[])
	{
		return lu_solve_factored(solverHandle, n, nrhs, a, ipiv, b, sgetrs);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="ipiv">Array, size at least max(1, n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int  d_lu_solve_factored(cusolverDnHandle_t solverHandle, int n, int nrhs, double a[], int ipiv[], double b[])
	{
		return lu_solve_factored(solverHandle, n, nrhs, a, ipiv, b, dgetrs);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="ipiv">Array, size at least max(1, n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int c_lu_solve_factored(cusolverDnHandle_t solverHandle, int n, int nrhs, cuComplex a[], int ipiv[], cuComplex b[])
	{
		return lu_solve_factored(solverHandle, n, nrhs, a, ipiv, b, cgetrs);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="ipiv">Array, size at least max(1, n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int z_lu_solve_factored(cusolverDnHandle_t solverHandle, int n, int nrhs, cuDoubleComplex a[], int ipiv[], cuDoubleComplex b[])
	{
		return lu_solve_factored(solverHandle, n, nrhs, a, ipiv, b, zgetrs);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int s_lu_solve(cusolverDnHandle_t solverHandle, int n, int nrhs, float a[], float b[])
	{
		return lu_solve(solverHandle, n, nrhs, a, b, sgetrf, sgetrs, sgetrfbsize);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int d_lu_solve(cusolverDnHandle_t solverHandle, int n, int nrhs, double a[], double b[])
	{
		return lu_solve(solverHandle, n, nrhs, a, b, dgetrf, dgetrs, dgetrfbsize);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int c_lu_solve(cusolverDnHandle_t solverHandle, int n, int nrhs, cuComplex a[], cuComplex b[])
	{
		return lu_solve(solverHandle, n, nrhs, a, b, cgetrf, cgetrs, cgetrfbsize);
	}

	/// <summary>
	/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
	/// The routine solves for X the following systems of linear equations:
	/// A*X = B if trans = 'N',
	/// AT*X = B if trans = 'T',
	/// AH*X = B if trans = 'C' (for complex matrices only).
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
	/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
	/// <param name="a">Array of size max(1, lda*n).</param>
	/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int z_lu_solve(cusolverDnHandle_t solverHandle, int n, int nrhs, cuDoubleComplex a[], cuDoubleComplex b[])
	{
		return lu_solve(solverHandle, n, nrhs, a, b, zgetrf, zgetrs, zgetrfbsize);
	}

	/// <summary>
	/// Computes the Cholesky factorization of a symmetric (Hermitian) positive-definite matrix.
	/// The routine forms the Cholesky factorization of a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data  if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="a">The upper or lower triangular part of a is overwritten by the Cholesky factor U or L, as specified by uplo. Array, size max(1, lda*n. The array a contains either the upper or the lower triangular part of the matrix A (see uplo).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the leading minor of order i (and therefore the matrix A itself) is not positive-definite, and the factorization could not be completed. This may indicate an error in forming the matrix A.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int s_cholesky_factor(cusolverDnHandle_t solverHandle, int n, float a[])
	{
		return cholesky_factor(solverHandle, n, a, spotrf, spotrfbsize);
	}

	/// <summary>
	/// Computes the Cholesky factorization of a symmetric (Hermitian) positive-definite matrix.
	/// The routine forms the Cholesky factorization of a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data  if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="a">The upper or lower triangular part of a is overwritten by the Cholesky factor U or L, as specified by uplo. Array, size max(1, lda*n. The array a contains either the upper or the lower triangular part of the matrix A (see uplo).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the leading minor of order i (and therefore the matrix A itself) is not positive-definite, and the factorization could not be completed. This may indicate an error in forming the matrix A.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int d_cholesky_factor(cusolverDnHandle_t solverHandle, int n, double a[])
	{
		return cholesky_factor(solverHandle, n, a, dpotrf, dpotrfbsize);
	}

	/// <summary>
	/// Computes the Cholesky factorization of a symmetric (Hermitian) positive-definite matrix.
	/// The routine forms the Cholesky factorization of a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data  if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="a">The upper or lower triangular part of a is overwritten by the Cholesky factor U or L, as specified by uplo. Array, size max(1, lda*n. The array a contains either the upper or the lower triangular part of the matrix A (see uplo).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the leading minor of order i (and therefore the matrix A itself) is not positive-definite, and the factorization could not be completed. This may indicate an error in forming the matrix A.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int c_cholesky_factor(cusolverDnHandle_t solverHandle, int n, cuComplex a[])
	{
		return cholesky_factor(solverHandle, n, a, cpotrf, cpotrfbsize);
	}

	/// <summary>
	/// Computes the Cholesky factorization of a symmetric (Hermitian) positive-definite matrix.
	/// The routine forms the Cholesky factorization of a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data  if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="a">The upper or lower triangular part of a is overwritten by the Cholesky factor U or L, as specified by uplo. Array, size max(1, lda*n. The array a contains either the upper or the lower triangular part of the matrix A (see uplo).</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// If info = i, the leading minor of order i (and therefore the matrix A itself) is not positive-definite, and the factorization could not be completed. This may indicate an error in forming the matrix A.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int z_cholesky_factor(cusolverDnHandle_t solverHandle, int n, cuDoubleComplex a[])
	{
		return cholesky_factor(solverHandle, n, a, zpotrf, zpotrfbsize);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int s_cholesky_solve(cusolverDnHandle_t solverHandle, int n, int nrhs, float a[], float b[])
	{
		return cholesky_solve(solverHandle, n, nrhs, a, b, spotrf, spotrs, spotrfbsize);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int d_cholesky_solve(cusolverDnHandle_t solverHandle, int n, int nrhs, double a[], double b[])
	{
		return cholesky_solve(solverHandle, n, nrhs, a, b, dpotrf, dpotrs, dpotrfbsize);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int c_cholesky_solve(cusolverDnHandle_t solverHandle, int n, int nrhs, cuComplex a[], cuComplex b[])
	{
		return cholesky_solve(solverHandle, n, nrhs, a, b, cpotrf, cpotrs, cpotrfbsize);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int z_cholesky_solve(cusolverDnHandle_t solverHandle, int n, int nrhs, cuDoubleComplex a[], cuDoubleComplex b[])
	{
		return cholesky_solve(solverHandle, n, nrhs, a, b, zpotrf, zpotrs, zpotrfbsize);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int s_cholesky_solve_factored(cusolverDnHandle_t solverHandle, int n, int nrhs, float a[], float b[])
	{
		return cholesky_solve_factored(solverHandle, n, nrhs, a, b, spotrs);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int d_cholesky_solve_factored(cusolverDnHandle_t solverHandle, int n, int nrhs, double a[], double b[])
	{
		return cholesky_solve_factored(solverHandle, n, nrhs, a, b, dpotrs);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int c_cholesky_solve_factored(cusolverDnHandle_t solverHandle, int n, int nrhs, cuComplex a[], cuComplex b[])
	{
		return cholesky_solve_factored(solverHandle, n, nrhs, a, b, cpotrs);
	}

	/// <summary>
	/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
	/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
	/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
	/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <param name="n">The order of matrix A; n ? 0.</param>
	/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
	/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
	/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
	/// <returns>This function returns a value info.
	/// If info = 0, the execution is successful.
	/// If info = -i, parameter i had an illegal value.
	/// </returns>
	EXPORT_NEQUEO_CUDA_API int z_cholesky_solve_factored(cusolverDnHandle_t solverHandle, int n, int nrhs, cuDoubleComplex a[], cuDoubleComplex b[])
	{
		return cholesky_solve_factored(solverHandle, n, nrhs, a, b, zpotrs);
	}

	/// <summary>
	/// Computes the singular value decomposition of a general rectangular matrix.
	/// The routine computes the singular value decomposition (SVD) of a real/complex m-by-n matrix A, optionally computing the left and/or right singular vectors. The SVD is written as
	/// A = U*?*VT for real routines
	/// A = U*?*VH for complex routines
	/// where ? is an m - by - n matrix which is zero except for its min(m, n) diagonal elements, U is an m - by - m orthogonal / unitary matrix, and V is an n - by - n orthogonal / unitary matrix.The diagonal elements of ? are the singular values of A; they are real and non - negative, and are returned in descending order.The first min(m, n) columns of U and V are the left and right singular vectors of A.
	/// Note that the routine returns VT(for real flavors) or VH(for complex flavors), not V.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
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
	EXPORT_NEQUEO_CUDA_API int s_svd_factor(cusolverDnHandle_t solverHandle, bool compute_vectors, int m, int n, float a[], float s[], float u[], float v[])
	{
		return svd_factor(solverHandle, compute_vectors, m, n, a, s, u, v, sgesvd, sgesvdbsize);
	}

	/// <summary>
	/// Computes the singular value decomposition of a general rectangular matrix.
	/// The routine computes the singular value decomposition (SVD) of a real/complex m-by-n matrix A, optionally computing the left and/or right singular vectors. The SVD is written as
	/// A = U*?*VT for real routines
	/// A = U*?*VH for complex routines
	/// where ? is an m - by - n matrix which is zero except for its min(m, n) diagonal elements, U is an m - by - m orthogonal / unitary matrix, and V is an n - by - n orthogonal / unitary matrix.The diagonal elements of ? are the singular values of A; they are real and non - negative, and are returned in descending order.The first min(m, n) columns of U and V are the left and right singular vectors of A.
	/// Note that the routine returns VT(for real flavors) or VH(for complex flavors), not V.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
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
	EXPORT_NEQUEO_CUDA_API int d_svd_factor(cusolverDnHandle_t solverHandle, bool compute_vectors, int m, int n, double a[], double s[], double u[], double v[])
	{
		return svd_factor(solverHandle, compute_vectors, m, n, a, s, u, v, dgesvd, dgesvdbsize);
	}

	/// <summary>
	/// Computes the singular value decomposition of a general rectangular matrix.
	/// The routine computes the singular value decomposition (SVD) of a real/complex m-by-n matrix A, optionally computing the left and/or right singular vectors. The SVD is written as
	/// A = U*?*VT for real routines
	/// A = U*?*VH for complex routines
	/// where ? is an m - by - n matrix which is zero except for its min(m, n) diagonal elements, U is an m - by - m orthogonal / unitary matrix, and V is an n - by - n orthogonal / unitary matrix.The diagonal elements of ? are the singular values of A; they are real and non - negative, and are returned in descending order.The first min(m, n) columns of U and V are the left and right singular vectors of A.
	/// Note that the routine returns VT(for real flavors) or VH(for complex flavors), not V.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
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
	EXPORT_NEQUEO_CUDA_API int c_svd_factor(cusolverDnHandle_t solverHandle, bool compute_vectors, int m, int n, cuComplex a[], cuComplex s[], cuComplex u[], cuComplex v[])
	{
		return complex_svd_factor<cuComplex, float>(solverHandle, compute_vectors, m, n, a, s, u, v, cgesvd, cgesvdbsize);
	}

	/// <summary>
	/// Computes the singular value decomposition of a general rectangular matrix.
	/// The routine computes the singular value decomposition (SVD) of a real/complex m-by-n matrix A, optionally computing the left and/or right singular vectors. The SVD is written as
	/// A = U*?*VT for real routines
	/// A = U*?*VH for complex routines
	/// where ? is an m - by - n matrix which is zero except for its min(m, n) diagonal elements, U is an m - by - m orthogonal / unitary matrix, and V is an n - by - n orthogonal / unitary matrix.The diagonal elements of ? are the singular values of A; they are real and non - negative, and are returned in descending order.The first min(m, n) columns of U and V are the left and right singular vectors of A.
	/// Note that the routine returns VT(for real flavors) or VH(for complex flavors), not V.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
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
	EXPORT_NEQUEO_CUDA_API int z_svd_factor(cusolverDnHandle_t solverHandle, bool compute_vectors, int m, int n, cuDoubleComplex a[], cuDoubleComplex s[], cuDoubleComplex u[], cuDoubleComplex v[])
	{
		return complex_svd_factor<cuDoubleComplex, double>(solverHandle, compute_vectors, m, n, a, s, u, v, zgesvd, zgesvdbsize);
	}
}