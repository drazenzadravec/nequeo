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

EXPORT_NEQUEO_MKL_API void s_axpy(const MKL_INT n, const float alpha, const float x[], float y[]) 
{
	cblas_saxpy(n, alpha, x, 1, y, 1);
}

EXPORT_NEQUEO_MKL_API void d_axpy(const MKL_INT n, const double alpha, const double x[], double y[]) 
{
	cblas_daxpy(n, alpha, x, 1, y, 1);
}

EXPORT_NEQUEO_MKL_API void c_axpy(const MKL_INT n, const MKL_Complex8 alpha, const MKL_Complex8 x[], MKL_Complex8 y[]) 
{
	cblas_caxpy(n, &alpha, x, 1, y, 1);
}

EXPORT_NEQUEO_MKL_API void z_axpy(const MKL_INT n, const MKL_Complex16 alpha, const MKL_Complex16 x[], MKL_Complex16 y[]) 
{
	cblas_zaxpy(n, &alpha, x, 1, y, 1);
}

EXPORT_NEQUEO_MKL_API void s_scale(const MKL_INT n, const float alpha, float x[]) 
{
	cblas_sscal(n, alpha, x, 1);
}

EXPORT_NEQUEO_MKL_API void d_scale(const MKL_INT n, const double alpha, double x[]) 
{
	cblas_dscal(n, alpha, x, 1);
}

EXPORT_NEQUEO_MKL_API void c_scale(const MKL_INT n, const MKL_Complex8 alpha, MKL_Complex8 x[]) 
{
	cblas_cscal(n, &alpha, x, 1);
}

EXPORT_NEQUEO_MKL_API void z_scale(const MKL_INT n, const MKL_Complex16 alpha, MKL_Complex16 x[]) 
{
	cblas_zscal(n, &alpha, x, 1);
}

EXPORT_NEQUEO_MKL_API float s_dot_product(const MKL_INT n, const float x[], const float y[]) 
{
	return cblas_sdot(n, x, 1, y, 1);
}

EXPORT_NEQUEO_MKL_API double d_dot_product(const MKL_INT n, const double x[], const double y[]) 
{
	return cblas_ddot(n, x, 1, y, 1);
}

EXPORT_NEQUEO_MKL_API MKL_Complex8 c_dot_product(const MKL_INT n, const MKL_Complex8 x[], const MKL_Complex8 y[]) 
{
	MKL_Complex8 ret;
	cblas_cdotu_sub(n, x, 1, y, 1, &ret);
	return ret;
}

EXPORT_NEQUEO_MKL_API MKL_Complex16 z_dot_product(const MKL_INT n, const MKL_Complex16 x[], const MKL_Complex16 y[]) 
{
	MKL_Complex16 ret;
	cblas_zdotu_sub(n, x, 1, y, 1, &ret);
	return ret;
}

EXPORT_NEQUEO_MKL_API void s_matrix_multiply(const CBLAS_TRANSPOSE transA, const CBLAS_TRANSPOSE transB, const MKL_INT m, const MKL_INT n, const MKL_INT k, const float alpha, const float x[], const float y[], const float beta, float c[]) 
{
	MKL_INT lda = transA == CblasNoTrans ? m : k;
	MKL_INT ldb = transB == CblasNoTrans ? k : n;

	cblas_sgemm(CblasColMajor, transA, transB, m, n, k, alpha, x, lda, y, ldb, beta, c, m);
}

EXPORT_NEQUEO_MKL_API void d_matrix_multiply(const CBLAS_TRANSPOSE transA, const CBLAS_TRANSPOSE transB, const MKL_INT m, const MKL_INT n, const MKL_INT k, const double alpha, const double x[], const double y[], const double beta, double c[]) 
{
	MKL_INT lda = transA == CblasNoTrans ? m : k;
	MKL_INT ldb = transB == CblasNoTrans ? k : n;

	cblas_dgemm(CblasColMajor, transA, transB, m, n, k, alpha, x, lda, y, ldb, beta, c, m);
}

EXPORT_NEQUEO_MKL_API void c_matrix_multiply(const CBLAS_TRANSPOSE transA, const CBLAS_TRANSPOSE transB, const MKL_INT m, const MKL_INT n, const MKL_INT k, const MKL_Complex8 alpha, const MKL_Complex8 x[], const MKL_Complex8 y[], const MKL_Complex8 beta, MKL_Complex8 c[]) 
{
	MKL_INT lda = transA == CblasNoTrans ? m : k;
	MKL_INT ldb = transB == CblasNoTrans ? k : n;

	cblas_cgemm(CblasColMajor, transA, transB, m, n, k, &alpha, x, lda, y, ldb, &beta, c, m);
}

EXPORT_NEQUEO_MKL_API void z_matrix_multiply(const CBLAS_TRANSPOSE transA, const CBLAS_TRANSPOSE transB, const MKL_INT m, const MKL_INT n, const MKL_INT k, const MKL_Complex16 alpha, const MKL_Complex16 x[], const MKL_Complex16 y[], const MKL_Complex16 beta, MKL_Complex16 c[]) 
{
	MKL_INT lda = transA == CblasNoTrans ? m : k;
	MKL_INT ldb = transB == CblasNoTrans ? k : n;

	cblas_zgemm(CblasColMajor, transA, transB, m, n, k, &alpha, x, lda, y, ldb, &beta, c, m);
}
