/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          LapackTemplates.cpp
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
#include "GlobalCUDA.cpp"

#include <cstring>

#include "cublas_v2.h"
#include "cusolverDn.h"
#include "cuda_runtime.h"

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
template<typename T, typename GETRF, typename GETRFBSIZE>
inline int lu_factor(cusolverDnHandle_t solverHandle, int m, T a[], int ipiv[], GETRF getrf, GETRFBSIZE getrfbsize)
{
	int info = 0;

	T* d_A = NULL;
	cudaMalloc((void**)&d_A, m*m * sizeof(T));
	cublasSetMatrix(m, m, sizeof(T), a, m, d_A, m);

	int* d_I = NULL;
	cudaMalloc((void**)&d_I, m * sizeof(int));

	T* work = NULL;
	int lwork = 0;
	getrfbsize(solverHandle, m, m, a, m, &lwork);
	cudaMalloc((void**)&work, sizeof(T)*lwork);

	int* d_info = NULL;
	cudaMalloc((void**)&d_info, sizeof(int));

	getrf(solverHandle, m, m, d_A, m, work, d_I, d_info);

	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cublasGetMatrix(m, m, sizeof(T), d_A, m, a, m);
	cublasGetVector(m, sizeof(int), d_I, 1, ipiv, 1);

	shift_ipiv_down(m, ipiv);

	cudaFree(d_A);
	cudaFree(d_I);
	cudaFree(d_info);
	cudaFree(work);

	return info;
};

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
template<typename T, typename GETRF, typename GETRIBATCHED, typename GETRFBSIZE>
inline int lu_inverse(cusolverDnHandle_t solverHandle, cublasHandle_t blasHandle, int n, T a[], GETRF getrf, GETRIBATCHED getribatched, GETRFBSIZE getrfbsize)
{
	int info = 0;

	int* d_I = NULL;
	cudaMalloc((void**)&d_I, n * sizeof(int));

	T* d_A = NULL;
	cudaMalloc((void**)&d_A, n*n * sizeof(T));
	cublasSetMatrix(n, n, sizeof(T), a, n, d_A, n);

	T* work = NULL;
	int lwork = 0;
	getrfbsize(solverHandle, n, n, d_A, n, &lwork);
	cudaMalloc((void**)&work, sizeof(T)*lwork);

	int* d_info = NULL;
	cudaMalloc((void**)&d_info, sizeof(int));

	getrf(solverHandle, n, n, d_A, n, work, d_I, d_info);
	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cudaFree(work);

	if (info != 0)
	{
		cudaFree(d_A);
		cudaFree(d_I);
		cudaFree(d_info);
		return info;
	}

	T* d_C = NULL;
	cudaMalloc((void**)&d_C, n*n * sizeof(T));

	const T **d_Aarray = NULL;
	cudaMalloc((void**)&d_Aarray, sizeof(T*));
	cudaMemcpy(d_Aarray, &d_A, sizeof(T*), cudaMemcpyHostToDevice);

	T **d_Carray = NULL;
	cudaMalloc((void**)&d_Carray, sizeof(T*));
	cudaMemcpy(d_Carray, &d_C, sizeof(T*), cudaMemcpyHostToDevice);

	getribatched(blasHandle, n, d_Aarray, n, d_I, d_Carray, n, d_info, 1);
	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cublasGetMatrix(n, n, sizeof(T), d_C, n, a, n);

	cudaFree(d_A);
	cudaFree(d_I);
	cudaFree(d_C);
	cudaFree(d_info);
	cudaFree(d_Aarray);
	cudaFree(d_Carray);

	return info;
};

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
template<typename T, typename GETRI>
inline int lu_inverse_factored(cublasHandle_t blasHandle, int n, T a[], int ipiv[], GETRI getri)
{
	int info = 0;

	shift_ipiv_up(n, ipiv);

	T* d_A = NULL;
	cudaMalloc((void**)&d_A, n*n * sizeof(T));
	cublasSetMatrix(n, n, sizeof(T), a, n, d_A, n);

	T* d_C = NULL;
	cudaMalloc((void**)&d_C, n*n * sizeof(T));

	int* d_I = NULL;
	cudaMalloc((void**)&d_I, n * sizeof(int));
	cublasSetVector(n, sizeof(int), ipiv, 1, d_I, 1);

	int* d_info = NULL;
	cudaMalloc((void**)&d_info, sizeof(int));

	const T **d_Aarray = NULL;
	cudaMalloc((void**)&d_Aarray, sizeof(T*));
	cudaMemcpy(d_Aarray, &d_A, sizeof(T*), cudaMemcpyHostToDevice);

	T **d_Carray = NULL;
	cudaMalloc((void**)&d_Carray, sizeof(T*));
	cudaMemcpy(d_Carray, &d_C, sizeof(T*), cudaMemcpyHostToDevice);

	getri(blasHandle, n, d_Aarray, n, d_I, d_Carray, n, d_info, 1);
	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cublasGetMatrix(n, n, sizeof(T), d_C, n, a, n);
	cublasGetVector(n, sizeof(int), d_I, 1, ipiv, 1);

	shift_ipiv_down(n, ipiv);

	cudaFree(d_A);
	cudaFree(d_I);
	cudaFree(d_C);
	cudaFree(d_info);
	cudaFree(d_Aarray);
	cudaFree(d_Carray);

	return info;
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
template<typename T, typename GETRS>
inline int lu_solve_factored(cusolverDnHandle_t solverHandle, int n, int nrhs, T a[], int ipiv[], T b[], GETRS getrs)
{
	int info = 0;

	shift_ipiv_up(n, ipiv);

	T* d_A = NULL;
	cudaMalloc((void**)&d_A, n*n * sizeof(T));
	cublasSetMatrix(n, n, sizeof(T), a, n, d_A, n);

	T* d_B = NULL;
	cudaMalloc((void**)&d_B, n*nrhs * sizeof(T));
	cublasSetMatrix(n, nrhs, sizeof(T), b, n, d_B, n);

	int* d_I = NULL;
	cudaMalloc((void**)&d_I, n * sizeof(int));
	cublasSetVector(n, sizeof(int), ipiv, 1, d_I, 1);

	int* d_info = NULL;
	cudaMalloc((void**)&d_info, sizeof(int));

	getrs(solverHandle, CUBLAS_OP_N, n, nrhs, d_A, n, d_I, d_B, n, d_info);
	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cublasGetMatrix(n, nrhs, sizeof(T), d_B, n, b, n);

	shift_ipiv_down(n, ipiv);

	cudaFree(d_A);
	cudaFree(d_B);
	cudaFree(d_I);
	cudaFree(d_info);

	return info;
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
template<typename T, typename GETRF, typename GETRS, typename GETRFBSIZE>
inline int lu_solve(cusolverDnHandle_t solverHandle, int n, int nrhs, T a[], T b[], GETRF getrf, GETRS getrs, GETRFBSIZE getrfbsize)
{
	int info = 0;

	int* d_I = NULL;
	cudaMalloc((void**)&d_I, n * sizeof(int));

	T* d_A = NULL;
	cudaMalloc((void**)&d_A, n*n * sizeof(T));
	cublasSetMatrix(n, n, sizeof(T), a, n, d_A, n);

	T* work = NULL;
	int lwork = 0;
	getrfbsize(solverHandle, n, n, a, n, &lwork);
	cudaMalloc((void**)&work, sizeof(T)*lwork);

	int* d_info = NULL;
	cudaMalloc((void**)&d_info, sizeof(int));

	getrf(solverHandle, n, n, d_A, n, work, d_I, d_info);
	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cudaFree(work);

	if (info != 0)
	{
		cudaFree(d_I);
		cudaFree(d_A);
		cudaFree(d_info);
		return info;
	}

	T* d_B = NULL;
	cudaMalloc((void**)&d_B, n*nrhs * sizeof(T));
	cublasSetMatrix(n, nrhs, sizeof(T), b, n, d_B, n);

	getrs(solverHandle, CUBLAS_OP_N, n, nrhs, d_A, n, d_I, d_B, n, d_info);
	cudaMemcpy(&info, d_info, 1, cudaMemcpyDeviceToHost);

	cublasGetMatrix(n, nrhs, sizeof(T), d_B, n, b, n);

	cudaFree(d_A);
	cudaFree(d_B);
	cudaFree(d_I);
	cudaFree(d_info);

	return info;
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
template<typename T, typename POTRF, typename POTRFBSIZE>
inline int cholesky_factor(cusolverDnHandle_t solverHandle, int n, T a[], POTRF potrf, POTRFBSIZE potrfbsize)
{
	int info = 0;

	T* d_A = NULL;
	cudaMalloc((void**)&d_A, n*n * sizeof(T));
	cublasSetMatrix(n, n, sizeof(T), a, n, d_A, n);

	T* work = NULL;
	int lWork = 0;
	potrfbsize(solverHandle, CUBLAS_FILL_MODE_LOWER, n, d_A, n, &lWork);
	cudaMalloc((void**)&work, sizeof(T)*lWork);

	int* d_info = NULL;
	cudaMalloc((void**)&d_info, sizeof(int));

	potrf(solverHandle, CUBLAS_FILL_MODE_LOWER, n, d_A, n, work, lWork, d_info);
	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cublasGetMatrix(n, n, sizeof(T), d_A, n, a, n);

	T zero = T();

	for (int i = 0; i < n; ++i)
	{
		int index = i * n;

		for (int j = 0; j < n && i > j; ++j)
		{
			a[index + j] = zero;
		}
	}

	cudaFree(d_A);
	cudaFree(d_info);
	cudaFree(work);

	return info;
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
template<typename T, typename POTRF, typename POTRS, typename POTRFBSIZE>
inline int cholesky_solve(cusolverDnHandle_t solverHandle, int n, int nrhs, T a[], T b[], POTRF potrf, POTRS potrs, POTRFBSIZE potrfbsize)
{
	int info = 0;

	T* d_A = NULL;
	cudaMalloc((void**)&d_A, n*n * sizeof(T));
	cublasSetMatrix(n, n, sizeof(T), a, n, d_A, n);

	T* work = NULL;
	int lWork = 0;
	potrfbsize(solverHandle, CUBLAS_FILL_MODE_LOWER, n, d_A, n, &lWork);
	cudaMalloc((void**)&work, sizeof(T)*lWork);

	int* d_info = NULL;
	cudaMalloc((void**)&d_info, sizeof(int));

	potrf(solverHandle, CUBLAS_FILL_MODE_LOWER, n, d_A, n, work, lWork, d_info);
	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cudaFree(work);

	if (info != 0)
	{
		cudaFree(d_A);
		cudaFree(d_info);
		return info;
	}

	T* d_B = NULL;
	cudaMalloc((void**)&d_B, n*nrhs * sizeof(T));
	cublasSetMatrix(n, nrhs, sizeof(T), b, n, d_B, n);

	potrs(solverHandle, CUBLAS_FILL_MODE_LOWER, n, nrhs, d_A, n, d_B, n, d_info);
	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cublasGetMatrix(n, nrhs, sizeof(T), d_B, n, b, n);

	cudaFree(d_A);
	cudaFree(d_B);
	cudaFree(d_info);

	return info;
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
template<typename T, typename POTRS>
inline int cholesky_solve_factored(cusolverDnHandle_t solverHandle, int n, int nrhs, T a[], T b[], POTRS potrs)
{
	int info = 0;

	T* d_A = NULL;
	cudaMalloc((void**)&d_A, n*n * sizeof(T));
	cublasSetMatrix(n, n, sizeof(T), a, n, d_A, n);

	T* d_B = NULL;
	cudaMalloc((void**)&d_B, n*nrhs * sizeof(T));
	cublasSetMatrix(n, nrhs, sizeof(T), b, n, d_B, n);

	int* d_info = NULL;
	cudaMalloc((void**)&d_info, sizeof(int));

	potrs(solverHandle, CUBLAS_FILL_MODE_LOWER, n, nrhs, d_A, n, d_B, n, d_info);
	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cublasGetMatrix(n, nrhs, sizeof(T), d_B, n, b, n);

	cudaFree(d_A);
	cudaFree(d_B);
	cudaFree(d_info);

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
template<typename T, typename GESVD, typename GESVDBSIZE>
inline int svd_factor(cusolverDnHandle_t solverHandle, bool compute_vectors, int m, int n, T a[], T s[], T u[], T v[], GESVD gesvd, GESVDBSIZE gesvdbsize)
{
	int info = 0;
	int dim_s = min(m, n);

	T* d_A = NULL;
	cudaMalloc((void**)&d_A, m*n * sizeof(T));
	cublasSetMatrix(m, n, sizeof(T), a, m, d_A, m);

	T* d_S = NULL;
	cudaMalloc((void**)&d_S, dim_s * sizeof(T));

	T* d_U = NULL;
	cudaMalloc((void**)&d_U, m*m * sizeof(T));

	T* d_V = NULL;
	cudaMalloc((void**)&d_V, n*n * sizeof(T));

	T* work = NULL;
	int lWork = 0;
	gesvdbsize(solverHandle, m, n, &lWork);
	cudaMalloc((void**)&work, lWork * sizeof(T));

	T* rwork = NULL;
	cudaMalloc((void**)&rwork, 5 * dim_s * sizeof(T));

	int* d_info = NULL;
	cudaMalloc((void**)&d_info, sizeof(int));

	char job = compute_vectors ? 'A' : 'N';
	gesvd(solverHandle, job, job, m, n, d_A, m, d_S, d_U, m, d_V, n, work, lWork, rwork, d_info);
	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cublasGetVector(dim_s, sizeof(T), d_S, 1, s, 1);
	cublasGetMatrix(m, m, sizeof(T), d_U, m, u, m);
	cublasGetMatrix(n, n, sizeof(T), d_V, n, v, n);

	cudaFree(d_A);
	cudaFree(d_S);
	cudaFree(d_U);
	cudaFree(d_V);
	cudaFree(work);
	cudaFree(rwork);
	cudaFree(d_info);

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
template<typename T, typename R, typename GESVD, typename GESVDBSIZE>
inline int complex_svd_factor(cusolverDnHandle_t solverHandle, bool compute_vectors, int m, int n, T a[], T s[], T u[], T v[], GESVD gesvd, GESVDBSIZE gesvdbsize)
{
	int info = 0;
	int dim_s = min(m, n);

	T* d_A = NULL;
	cudaMalloc((void**)&d_A, m*n * sizeof(T));
	cublasSetMatrix(m, n, sizeof(T), a, m, d_A, m);

	R* s_local = new R[dim_s];
	R* d_S = NULL;
	cudaMalloc((void**)&d_S, dim_s * sizeof(R));

	T* d_U = NULL;
	cudaMalloc((void**)&d_U, m*m * sizeof(T));

	T* d_V = NULL;
	cudaMalloc((void**)&d_V, n*m * sizeof(T));

	T* work = NULL;
	int lWork = 0;
	gesvdbsize(solverHandle, m, n, &lWork);
	cudaMalloc((void**)&work, lWork * sizeof(T));

	R* rwork = NULL;
	cudaMalloc((void**)&rwork, 5 * dim_s * sizeof(R));

	int* d_info = NULL;
	cudaMalloc((void**)&d_info, sizeof(int));

	char job = compute_vectors ? 'A' : 'N';
	gesvd(solverHandle, job, job, m, n, d_A, m, d_S, d_U, m, d_V, n, work, lWork, rwork, d_info);
	cudaMemcpy(&info, d_info, sizeof(int), cudaMemcpyDeviceToHost);

	cublasGetVector(dim_s, sizeof(R), d_S, 1, s_local, 1);
	cublasGetMatrix(m, m, sizeof(T), d_U, m, u, m);
	cublasGetMatrix(n, n, sizeof(T), d_V, n, v, n);

	for (int index = 0; index < dim_s; ++index)
	{
		s[index].x = s_local[index];
	}

	delete[] s_local;
	cudaFree(d_A);
	cudaFree(d_S);
	cudaFree(d_U);
	cudaFree(d_V);
	cudaFree(work);
	cudaFree(rwork);
	cudaFree(d_info);

	return info;
}