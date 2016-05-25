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

#include "mkl.h"

/// <summary>
/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
/// </summary>
/// <param name="n">Specifies the number of elements in vectors x and y.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
EXPORT_NEQUEO_MKL_API void s_axpy(const MKL_INT n, const float alpha, const float x[], float y[]) 
{
	cblas_saxpy(n, alpha, x, 1, y, 1);
}

/// <summary>
/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
/// </summary>
/// <param name="n">Specifies the number of elements in vectors x and y.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
EXPORT_NEQUEO_MKL_API void d_axpy(const MKL_INT n, const double alpha, const double x[], double y[]) 
{
	cblas_daxpy(n, alpha, x, 1, y, 1);
}

/// <summary>
/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
/// </summary>
/// <param name="n">Specifies the number of elements in vectors x and y.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
EXPORT_NEQUEO_MKL_API void c_axpy(const MKL_INT n, const MKL_Complex8 alpha, const MKL_Complex8 x[], MKL_Complex8 y[]) 
{
	cblas_caxpy(n, &alpha, x, 1, y, 1);
}

/// <summary>
/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
/// </summary>
/// <param name="n">Specifies the number of elements in vectors x and y.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
EXPORT_NEQUEO_MKL_API void z_axpy(const MKL_INT n, const MKL_Complex16 alpha, const MKL_Complex16 x[], MKL_Complex16 y[]) 
{
	cblas_zaxpy(n, &alpha, x, 1, y, 1);
}

/// <summary>
/// Computes the product of a vector by a scalar x = a*x.
/// </summary>
/// <param name="n">Specifies the number of elements in vector x.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
EXPORT_NEQUEO_MKL_API void s_scale(const MKL_INT n, const float alpha, float x[]) 
{
	cblas_sscal(n, alpha, x, 1);
}

/// <summary>
/// Computes the product of a vector by a scalar x = a*x.
/// </summary>
/// <param name="n">Specifies the number of elements in vector x.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
EXPORT_NEQUEO_MKL_API void d_scale(const MKL_INT n, const double alpha, double x[]) 
{
	cblas_dscal(n, alpha, x, 1);
}

/// <summary>
/// Computes the product of a vector by a scalar x = a*x.
/// </summary>
/// <param name="n">Specifies the number of elements in vector x.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
EXPORT_NEQUEO_MKL_API void c_scale(const MKL_INT n, const MKL_Complex8 alpha, MKL_Complex8 x[]) 
{
	cblas_cscal(n, &alpha, x, 1);
}

/// <summary>
/// Computes the product of a vector by a scalar x = a*x.
/// </summary>
/// <param name="n">Specifies the number of elements in vector x.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
EXPORT_NEQUEO_MKL_API void z_scale(const MKL_INT n, const MKL_Complex16 alpha, MKL_Complex16 x[]) 
{
	cblas_zscal(n, &alpha, x, 1);
}

/// <summary>
/// Computes a vector-vector dot product with double precision.
/// </summary>
/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
EXPORT_NEQUEO_MKL_API float s_dot_product(const MKL_INT n, const float x[], const float y[]) 
{
	return cblas_sdot(n, x, 1, y, 1);
}

/// <summary>
/// Computes a vector-vector dot product with double precision.
/// </summary>
/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
EXPORT_NEQUEO_MKL_API double d_dot_product(const MKL_INT n, const double x[], const double y[]) 
{
	return cblas_ddot(n, x, 1, y, 1);
}

/// <summary>
/// Computes a vector-vector dot product with double precision.
/// </summary>
/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
EXPORT_NEQUEO_MKL_API MKL_Complex8 c_dot_product(const MKL_INT n, const MKL_Complex8 x[], const MKL_Complex8 y[]) 
{
	MKL_Complex8 ret;
	cblas_cdotu_sub(n, x, 1, y, 1, &ret);
	return ret;
}

/// <summary>
/// Computes a vector-vector dot product with double precision.
/// </summary>
/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
EXPORT_NEQUEO_MKL_API MKL_Complex16 z_dot_product(const MKL_INT n, const MKL_Complex16 x[], const MKL_Complex16 y[]) 
{
	MKL_Complex16 ret;
	cblas_zdotu_sub(n, x, 1, y, 1, &ret);
	return ret;
}

/// <summary>
/// Computes a matrix-matrix product with general matrices C := alpha*op(A)*op(B) + beta*C.
/// </summary>
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
EXPORT_NEQUEO_MKL_API void s_matrix_multiply(const CBLAS_TRANSPOSE transA, const CBLAS_TRANSPOSE transB, const MKL_INT m, const MKL_INT n, const MKL_INT k, const float alpha, const float x[], const float y[], const float beta, float c[]) 
{
	MKL_INT lda = transA == CblasNoTrans ? m : k;
	MKL_INT ldb = transB == CblasNoTrans ? k : n;

	cblas_sgemm(CblasColMajor, transA, transB, m, n, k, alpha, x, lda, y, ldb, beta, c, m);
}

/// <summary>
/// Computes a matrix-matrix product with general matrices C := alpha*op(A)*op(B) + beta*C.
/// </summary>
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
EXPORT_NEQUEO_MKL_API void d_matrix_multiply(const CBLAS_TRANSPOSE transA, const CBLAS_TRANSPOSE transB, const MKL_INT m, const MKL_INT n, const MKL_INT k, const double alpha, const double x[], const double y[], const double beta, double c[]) 
{
	MKL_INT lda = transA == CblasNoTrans ? m : k;
	MKL_INT ldb = transB == CblasNoTrans ? k : n;

	cblas_dgemm(CblasColMajor, transA, transB, m, n, k, alpha, x, lda, y, ldb, beta, c, m);
}

/// <summary>
/// Computes a matrix-matrix product with general matrices C := alpha*op(A)*op(B) + beta*C.
/// </summary>
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
EXPORT_NEQUEO_MKL_API void c_matrix_multiply(const CBLAS_TRANSPOSE transA, const CBLAS_TRANSPOSE transB, const MKL_INT m, const MKL_INT n, const MKL_INT k, const MKL_Complex8 alpha, const MKL_Complex8 x[], const MKL_Complex8 y[], const MKL_Complex8 beta, MKL_Complex8 c[]) 
{
	MKL_INT lda = transA == CblasNoTrans ? m : k;
	MKL_INT ldb = transB == CblasNoTrans ? k : n;

	cblas_cgemm(CblasColMajor, transA, transB, m, n, k, &alpha, x, lda, y, ldb, &beta, c, m);
}

/// <summary>
/// Computes a matrix-matrix product with general matrices C := alpha*op(A)*op(B) + beta*C.
/// </summary>
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
EXPORT_NEQUEO_MKL_API void z_matrix_multiply(const CBLAS_TRANSPOSE transA, const CBLAS_TRANSPOSE transB, const MKL_INT m, const MKL_INT n, const MKL_INT k, const MKL_Complex16 alpha, const MKL_Complex16 x[], const MKL_Complex16 y[], const MKL_Complex16 beta, MKL_Complex16 c[]) 
{
	MKL_INT lda = transA == CblasNoTrans ? m : k;
	MKL_INT ldb = transB == CblasNoTrans ? k : n;

	cblas_zgemm(CblasColMajor, transA, transB, m, n, k, &alpha, x, lda, y, ldb, &beta, c, m);
}
