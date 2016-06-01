/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          CudaCapabilities.cpp
*  Purpose :       CUDA Capabilities.
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

#include "cublas_v2.h"
#include "cusolverDn.h"

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

	// <summary>
	/// Get the capability.
	/// </summary>
	/// <param name="capability">The capability mode.</param>
	/// <returns>The capability.</returns>
	EXPORT_NEQUEO_CUDA_API int query_capability(const int capability)
	{
		switch (capability)
		{

			// SANITY CHECKS
		case 0:	return 0;
		case 1:	return -1;

			// PLATFORM
		case 8:
#ifdef _M_IX86
			return 1;
#else
			return 0;
#endif
		case 9:
#ifdef _M_X64
			return 1;
#else
			return 0;
#endif
		case 10:
#ifdef _M_IA64
			return 1;
#else
			return 0;
#endif

			// COMMON/SHARED
		case 64: return 1; // revision

						   // LINEAR ALGEBRA
		case 128: return 1;	// basic dense linear algebra (major - breaking)
		case 129: return 0;	// basic dense linear algebra (minor - non-breaking)

							// OPTIMIZATION
		case 256: return 0; // basic optimization

							// FFT
		case 384: return 0; // basic FFT

		default: return 0; // unknown or not supported

		}
	}

	/// <summary>
	/// Create Blas handle.
	/// </summary>
	/// <param name="blasHandle">The Blas handle.</param>
	/// <returns>The status.</returns>
	EXPORT_NEQUEO_CUDA_API cublasStatus_t createBLASHandle(cublasHandle_t *blasHandle) 
	{
		return cublasCreate(blasHandle);
	}

	/// <summary>
	/// Destroy Blas handle.
	/// </summary>
	/// <param name="blasHandle">The Blas handle.</param>
	/// <returns>The status.</returns>
	EXPORT_NEQUEO_CUDA_API cublasStatus_t destroyBLASHandle(cublasHandle_t blasHandle) 
	{
		return cublasDestroy(blasHandle);
	}

	/// <summary>
	/// Create solver handle.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <returns>The status.</returns>
	EXPORT_NEQUEO_CUDA_API cusolverStatus_t createSolverHandle(cusolverDnHandle_t *solverHandle) 
	{
		return cusolverDnCreate(solverHandle);
	}

	/// <summary>
	/// Destroy solver handle.
	/// </summary>
	/// <param name="solverHandle">The solver handle.</param>
	/// <returns>The status.</returns>
	EXPORT_NEQUEO_CUDA_API cusolverStatus_t destroySolverHandle(cusolverDnHandle_t solverHandle) 
	{
		return cusolverDnDestroy(solverHandle);
	}

#ifdef __cplusplus
}
#endif /* __cplusplus */