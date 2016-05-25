/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          BlasFunctions.h
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

#include "acml.h"

///	<summary>
///	BLAS transpose type.
///	</summary>
enum TRANSPOSE 
{ 
	CblasNoTrans = 111, 
	CblasTrans = 112, 
	CblasConjTrans = 113, 
	CblasConjNoTrans = 114 
};

/// <summary>
/// Get transpose value.
/// </summary>
/// <param name="transpose">The transpose type.</param>
/// <returns>The transpose value.</returns>
char getTransChar(TRANSPOSE);

/// <summary>
/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
/// </summary>
/// <param name="n">Specifies the number of elements in vectors x and y.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
EXPORT_NEQUEO_ACML_API void s_axpy(const int n, const float alpha, float x[], float y[]) 
{
	saxpy(n, alpha, x, 1, y, 1);
}

/// <summary>
/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
/// </summary>
/// <param name="n">Specifies the number of elements in vectors x and y.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
EXPORT_NEQUEO_ACML_API void d_axpy(const int n, const double alpha, double x[], double y[]) 
{
	daxpy(n, alpha, x, 1, y, 1);
}

/// <summary>
/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
/// </summary>
/// <param name="n">Specifies the number of elements in vectors x and y.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
EXPORT_NEQUEO_ACML_API void c_axpy(const int n, complex alpha, complex x[], complex y[]) 
{
	caxpy(n, &alpha, x, 1, y, 1);
}

/// <summary>
/// Computes a vector-scalar product and adds the result to a vector y := a*x + y.
/// </summary>
/// <param name="n">Specifies the number of elements in vectors x and y.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
/// <param name="y">Array, size at least (1 + (n-1)*abs(incy)). </param>
EXPORT_NEQUEO_ACML_API void z_axpy(const int n, doublecomplex alpha, doublecomplex x[], doublecomplex y[]) 
{
	zaxpy(n, &alpha, x, 1, y, 1);
}

/// <summary>
/// Computes the product of a vector by a scalar x = a*x.
/// </summary>
/// <param name="n">Specifies the number of elements in vector x.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
EXPORT_NEQUEO_ACML_API void s_scale(const int n, const float alpha, float x[]) 
{
	sscal(n, alpha, x, 1);
}

/// <summary>
/// Computes the product of a vector by a scalar x = a*x.
/// </summary>
/// <param name="n">Specifies the number of elements in vector x.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
EXPORT_NEQUEO_ACML_API void d_scale(const int n, const double alpha, double x[]) 
{
	dscal(n, alpha, x, 1);
}

/// <summary>
/// Computes the product of a vector by a scalar x = a*x.
/// </summary>
/// <param name="n">Specifies the number of elements in vector x.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
EXPORT_NEQUEO_ACML_API void c_scale(const int n, complex alpha, complex x[]) 
{
	cscal(n, &alpha, x, 1);
}

/// <summary>
/// Computes the product of a vector by a scalar x = a*x.
/// </summary>
/// <param name="n">Specifies the number of elements in vector x.</param>
/// <param name="alpha">Specifies the scalar a.</param>
/// <param name="x">Array, size at least (1 + (n -1)*abs(incx)).</param>
EXPORT_NEQUEO_ACML_API void z_scale(const int n, doublecomplex alpha, doublecomplex x[]) 
{
	zscal(n, &alpha, x, 1);
}

/// <summary>
/// Computes a vector-vector dot product with double precision.
/// </summary>
/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
EXPORT_NEQUEO_ACML_API float s_dot_product(const int n, float x[], float y[]) 
{
	return sdot(n, x, 1, y, 1);
}

/// <summary>
/// Computes a vector-vector dot product with double precision.
/// </summary>
/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
EXPORT_NEQUEO_ACML_API double d_dot_product(const int n, double x[], double y[]) 
{
	return ddot(n, x, 1, y, 1);
}

/// <summary>
/// Computes a vector-vector dot product with double precision.
/// </summary>
/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
EXPORT_NEQUEO_ACML_API complex c_dot_product(const int n, complex x[], complex y[]) 
{
	return cdotu(n, x, 1, y, 1);
}

/// <summary>
/// Computes a vector-vector dot product with double precision.
/// </summary>
/// <param name="n">Specifies the number of elements in the input vectors x and y.</param>
/// <param name="x">Arrays, size at least (1+(n -1)*abs(incx)).</param>
/// <param name="y">Arrays, size at least (1+(n -1)*abs(incy)).</param>
EXPORT_NEQUEO_ACML_API doublecomplex z_dot_product(int n, doublecomplex x[], doublecomplex y[])
{
	return zdotu(n, x, 1, y, 1);
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
EXPORT_NEQUEO_ACML_API void s_matrix_multiply(const enum TRANSPOSE transA, const enum TRANSPOSE transB, const int m, const int n, const int k, float alpha, float x[], float y[], float beta, float c[]) 
{
	int lda = transA == CblasNoTrans ? m : k;
	int ldb = transB == CblasNoTrans ? k : n;
	char transAchar = getTransChar(transA);
	char transBchar = getTransChar(transB);
	sgemm(transAchar, transBchar, m, n, k, alpha, x, lda, y, ldb, beta, c, m);
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
EXPORT_NEQUEO_ACML_API void d_matrix_multiply(const enum TRANSPOSE transA, const enum TRANSPOSE transB, const int m, const int n, const int k, double alpha, double x[], double y[], double beta, double c[])
{
	int lda = transA == CblasNoTrans ? m : k;
	int ldb = transB == CblasNoTrans ? k : n;
	char transAchar = getTransChar(transA);
	char transBchar = getTransChar(transB);
	dgemm(transAchar, transBchar, m, n, k, alpha, x, lda, y, ldb, beta, c, m);
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
EXPORT_NEQUEO_ACML_API void c_matrix_multiply(const enum TRANSPOSE transA, const enum TRANSPOSE transB, const int m, const int n, const int k, complex alpha, complex x[], complex y[], complex beta, complex c[]) 
{
	int lda = transA == CblasNoTrans ? m : k;
	int ldb = transB == CblasNoTrans ? k : n;
	char transAchar = getTransChar(transA);
	char transBchar = getTransChar(transB);
	cgemm(transAchar, transBchar, m, n, k, &alpha, x, lda, y, ldb, &beta, c, m);
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
EXPORT_NEQUEO_ACML_API void z_matrix_multiply(const enum TRANSPOSE transA, const enum TRANSPOSE transB, const int m, const int n, const int k, doublecomplex alpha, doublecomplex x[], doublecomplex y[], doublecomplex beta, doublecomplex c[]) 
{
	int lda = transA == CblasNoTrans ? m : k;
	int ldb = transB == CblasNoTrans ? k : n;
	char transAchar = getTransChar(transA);
	char transBchar = getTransChar(transB);
	zgemm(transAchar, transBchar, m, n, k, &alpha, x, lda, y, ldb, &beta, c, m);
}

/// <summary>
/// Get transpose value.
/// </summary>
/// <param name="transpose">The transpose type.</param>
/// <returns>The transpose value.</returns>
char getTransChar(enum TRANSPOSE trans) 
{
	char cTrans;
	switch (trans) 
	{
	case  CblasNoTrans: cTrans = 'N';
		break;
	case  CblasTrans: cTrans = 'T';
		break;
	case  CblasConjTrans: cTrans = 'C';
		break;
	}
	return cTrans;
}
