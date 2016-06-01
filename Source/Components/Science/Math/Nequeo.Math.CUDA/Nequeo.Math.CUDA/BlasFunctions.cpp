/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          BlasFunctions.cpp
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
#include "BlasTemplates.cpp"

#include "cublas_v2.h"
#include "cuda_runtime.h"

extern "C" 
{
	/// <summary>
	/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in vectors x and y.</param>
	/// <param name="alpha">Specifies the scalar a.</param>
	/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
	/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
	EXPORT_NEQUEO_CUDA_API void s_axpy(const cublasHandle_t blasHandle, const int n, const float alpha, const float x[], float y[])
	{
		cuda_axpy(blasHandle, n, alpha, x, 1, y, 1, cublasSaxpy);
	}

	/// <summary>
	/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in vectors x and y.</param>
	/// <param name="alpha">Specifies the scalar a.</param>
	/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
	/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
	EXPORT_NEQUEO_CUDA_API void d_axpy(const cublasHandle_t blasHandle, const int n, const double alpha, const double x[], double y[])
	{
		cuda_axpy(blasHandle, n, alpha, x, 1, y, 1, cublasDaxpy);
	}

	/// <summary>
	/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in vectors x and y.</param>
	/// <param name="alpha">Specifies the scalar a.</param>
	/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
	/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
	EXPORT_NEQUEO_CUDA_API void c_axpy(const cublasHandle_t blasHandle, const int n, const cuComplex alpha, const cuComplex x[], cuComplex y[])
	{
		cuda_axpy(blasHandle, n, alpha, x, 1, y, 1, cublasCaxpy);
	}

	/// <summary>
	/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in vectors x and y.</param>
	/// <param name="alpha">Specifies the scalar a.</param>
	/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
	/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
	EXPORT_NEQUEO_CUDA_API void z_axpy(const cublasHandle_t blasHandle, const int n, const cuDoubleComplex alpha, const cuDoubleComplex x[], cuDoubleComplex y[])
	{
		cuda_axpy(blasHandle, n, alpha, x, 1, y, 1, cublasZaxpy);
	}

	/// <summary>
	/// Computes the product of a vector by a scalar x = a*x.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in vector x.</param>
	/// <param name="alpha">Specifies the scalar a.</param>
	/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
	EXPORT_NEQUEO_CUDA_API void s_scale(const cublasHandle_t blasHandle, const int n, const float alpha, float x[])
	{
		cuda_scal(blasHandle, n, alpha, x, 1, cublasSscal);
	}

	/// <summary>
	/// Computes the product of a vector by a scalar x = a*x.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in vector x.</param>
	/// <param name="alpha">Specifies the scalar a.</param>
	/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
	EXPORT_NEQUEO_CUDA_API void d_scale(const cublasHandle_t blasHandle, const int n, const double alpha, double x[])
	{
		cuda_scal(blasHandle, n, alpha, x, 1, cublasDscal);
	}

	/// <summary>
	/// Computes the product of a vector by a scalar x = a*x.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in vector x.</param>
	/// <param name="alpha">Specifies the scalar a.</param>
	/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
	EXPORT_NEQUEO_CUDA_API void c_scale(const cublasHandle_t blasHandle, const int n, const cuComplex alpha, cuComplex x[])
	{
		cuda_scal(blasHandle, n, alpha, x, 1, cublasCscal);
	}

	/// <summary>
	/// Computes the product of a vector by a scalar x = a*x.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in vector x.</param>
	/// <param name="alpha">Specifies the scalar a.</param>
	/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
	EXPORT_NEQUEO_CUDA_API void z_scale(const cublasHandle_t blasHandle, const int n, const cuDoubleComplex alpha, cuDoubleComplex x[])
	{
		cuda_scal(blasHandle, n, alpha, x, 1, cublasZscal);
	}

	/// <summary>
	/// Computes a vector-vector dot product with double precision.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
	/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
	/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
	EXPORT_NEQUEO_CUDA_API float s_dot_product(const cublasHandle_t blasHandle, const int n, const float x[], const float y[])
	{
		float ret;
		cuda_dot(blasHandle, n, x, 1, y, 1, &ret, cublasSdot);
		return ret;
	}

	/// <summary>
	/// Computes a vector-vector dot product with double precision.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
	/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
	/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
	EXPORT_NEQUEO_CUDA_API double d_dot_product(const cublasHandle_t blasHandle, const int n, const double x[], const double y[])
	{
		double ret;
		cuda_dot(blasHandle, n, x, 1, y, 1, &ret, cublasDdot);
		return ret;
	}

	/// <summary>
	/// Computes a vector-vector dot product with double precision.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
	/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
	/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
	EXPORT_NEQUEO_CUDA_API cuComplex c_dot_product(const cublasHandle_t blasHandle, const int n, const cuComplex x[], const cuComplex y[])
	{
		cuComplex ret;
		cuda_dot(blasHandle, n, x, 1, y, 1, &ret, cublasCdotu);
		return ret;
	}

	/// <summary>
	/// Computes a vector-vector dot product with double precision.
	/// </summary>
	/// <param name="blasHandle">The blas handle.</param>
	/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
	/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
	/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
	EXPORT_NEQUEO_CUDA_API cuDoubleComplex z_dot_product(const cublasHandle_t blasHandle, const int n, const cuDoubleComplex x[], const cuDoubleComplex y[])
	{
		cuDoubleComplex ret;
		cuda_dot(blasHandle, n, x, 1, y, 1, &ret, cublasZdotu);
		return ret;
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
	EXPORT_NEQUEO_CUDA_API void s_matrix_multiply(const cublasHandle_t blasHandle, cublasOperation_t transA, cublasOperation_t transB, const int m, const int n, const int k, const float alpha, const float x[], const float y[], const float beta, float c[])
	{
		int lda = transA == CUBLAS_OP_N ? m : k;
		int ldb = transB == CUBLAS_OP_N ? k : n;

		cuda_gemm(blasHandle, transA, transB, m, n, k, alpha, x, lda, y, ldb, beta, c, m, cublasSgemm);
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
	EXPORT_NEQUEO_CUDA_API void d_matrix_multiply(const cublasHandle_t blasHandle, cublasOperation_t transA, cublasOperation_t transB, const int m, const int n, const int k, const double alpha, const double x[], const double y[], const double beta, double c[])
	{
		int lda = transA == CUBLAS_OP_N ? m : k;
		int ldb = transB == CUBLAS_OP_N ? k : n;

		cuda_gemm(blasHandle, transA, transB, m, n, k, alpha, x, lda, y, ldb, beta, c, m, cublasDgemm);
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
	EXPORT_NEQUEO_CUDA_API void c_matrix_multiply(const cublasHandle_t blasHandle, cublasOperation_t transA, cublasOperation_t transB, const int m, const int n, const int k, const cuComplex alpha, const cuComplex x[], const cuComplex y[], const cuComplex beta, cuComplex c[])
	{
		int lda = transA == CUBLAS_OP_N ? m : k;
		int ldb = transB == CUBLAS_OP_N ? k : n;

		cuda_gemm(blasHandle, transA, transB, m, n, k, alpha, x, lda, y, ldb, beta, c, m, cublasCgemm);
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
	EXPORT_NEQUEO_CUDA_API void z_matrix_multiply(const cublasHandle_t blasHandle, cublasOperation_t transA, cublasOperation_t transB, const int m, const int n, const int k, const cuDoubleComplex alpha, const cuDoubleComplex x[], const cuDoubleComplex y[], const cuDoubleComplex beta, cuDoubleComplex c[])
	{
		int lda = transA == CUBLAS_OP_N ? m : k;
		int ldb = transB == CUBLAS_OP_N ? k : n;

		cuda_gemm(blasHandle, transA, transB, m, n, k, alpha, x, lda, y, ldb, beta, c, m, cublasZgemm);
	}
}
