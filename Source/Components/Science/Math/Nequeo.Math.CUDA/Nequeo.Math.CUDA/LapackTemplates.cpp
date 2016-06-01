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