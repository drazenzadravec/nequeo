/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          BlasTemplates.cpp
*  Purpose :       Common Blas functions.
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

#include "cublas_v2.h"
#include "cuda_runtime.h"

/// <summary>
/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
/// </summary>
/// <param name="blasHandle">The blas handle.</param>
/// <param name="n">Specifies the number of elements in vectors x and y.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
/// <param name="incX">Array size of x.</param>
/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
/// <param name="incY">Array size of y.</param>
template<typename T, typename AXPY>
void cuda_axpy(const cublasHandle_t blasHandle, const int n, const T alpha, const T x[], int incX, T y[], int incY, AXPY axpy)
{
	T *d_X = NULL;
	T *d_Y = NULL;

	cudaMalloc((void**)&d_X, n * sizeof(T));
	cudaMalloc((void**)&d_Y, n * sizeof(T));

	cublasSetVector(n, sizeof(T), x, incX, d_X, incX);
	cublasSetVector(n, sizeof(T), y, incY, d_Y, incY);

	axpy(blasHandle, n, &alpha, d_X, incX, d_Y, incX);

	cublasGetVector(n, sizeof(T), d_Y, incY, y, incY);

	cudaFree(d_X);
	cudaFree(d_Y);
}

/// <summary>
/// Computes the product of a vector by a scalar x = a*x.
/// </summary>
/// <param name="blasHandle">The blas handle.</param>
/// <param name="n">Specifies the number of elements in vector x.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
/// <param name="incX">Array size of x.</param>
template<typename T, typename SCAL>
void cuda_scal(const cublasHandle_t blasHandle, const int n, const T alpha, T x[], int incX, SCAL scal)
{
	T *d_X = NULL;

	cudaMalloc((void**)&d_X, n * sizeof(T));
	cublasSetVector(n, sizeof(T), x, incX, d_X, incX);

	scal(blasHandle, n, &alpha, d_X, incX);

	cublasGetVector(n, sizeof(T), d_X, incX, x, incX);

	cudaFree(d_X);
}

/// <summary>
/// Computes a vector-vector dot product with double precision.
/// </summary>
/// <param name="blasHandle">The blas handle.</param>
/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
/// <param name="incX">Array size of x.</param>
/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
/// <param name="incY">Array size of y.</param>
template<typename T, typename DOT>
void cuda_dot(const cublasHandle_t blasHandle, const int n, const T x[], int incX, const T y[], int incY, T* result, DOT dot)
{
	T *d_X = NULL;
	T *d_Y = NULL;

	cudaMalloc((void**)&d_X, n * sizeof(T));
	cudaMalloc((void**)&d_Y, n * sizeof(T));

	cublasSetVector(n, sizeof(T), x, incX, d_X, incX);
	cublasSetVector(n, sizeof(T), y, incY, d_Y, incY);

	dot(blasHandle, n, d_X, incX, d_Y, incY, result);

	cudaFree(d_X);
	cudaFree(d_Y);
}

/// <summary>
/// Computes a matrix-matrix product with general matrices C := alpha*op(A)*op(B) + beta*C.
/// </summary>
/// <param name="blasHandle">The blas handle.</param>
/// <param name="transA">Specifies the form of op(A) used in the matrix multiplication.</param>
/// <param name="transB">Specifies the form of op(B) used in the matrix multiplication.</param>
/// <param name="m">Specifies the number of rows of the matrix op(A) and of the matrix C. The value of m must be at least zero.</param>
/// <param name="n">Specifies the number of columns of the matrix op(B) and the number of columns of the matrix C. The value of n must be at least zero.</param>
/// <param name="k">Specifies the number of columns of the matrix op(A) and the number of rows of the matrix op(B). The value of k must be at least zero.</param>
/// <param name="alpha">Specifies the scalar alpha.</param>
/// <param name="x">The matrix A.</param>
/// <param name="y">The matrix B.</param>
/// <param name="beta">Specifies the scalar beta. When beta is equal to zero, then c need not be set on input.</param>
/// <param name="c">Overwritten by the m-by-n matrix (alpha*op(A)*op(B) + beta*C).</param>
template<typename T, typename GEMM>
void cuda_gemm(const cublasHandle_t handle, const cublasOperation_t transa, const cublasOperation_t transb, int m, int n, int k, const T alpha, const T A[], int lda, const T B[], int ldb, const T beta, T C[], int ldc, GEMM gemm)
{
	T *d_A = NULL;
	T *d_B = NULL;
	T *d_C = NULL;

	cudaMalloc((void**)&d_A, m*k * sizeof(T));
	cublasSetMatrix(m, k, sizeof(T), A, m, d_A, m);

	cudaMalloc((void**)&d_B, k*n * sizeof(T));
	cublasSetMatrix(k, n, sizeof(T), B, k, d_B, k);

	cudaMalloc((void**)&d_C, m*n * sizeof(T));
	cublasSetMatrix(m, n, sizeof(T), C, m, d_C, m);

	gemm(handle, transa, transb, m, n, k, &alpha, d_A, lda, d_B, ldb, &beta, d_C, ldc);

	cublasGetMatrix(m, n, sizeof(T), d_C, m, C, m);

	cudaFree(d_A);
	cudaFree(d_B);
	cudaFree(d_C);
}