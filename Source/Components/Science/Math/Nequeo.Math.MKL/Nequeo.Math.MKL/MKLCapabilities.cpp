/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MKLCapabilities.cpp
*  Purpose :       MKL Capabilities.
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

#ifdef __cplusplus
extern "C" 
{
#endif /* __cplusplus */

	/// <summary>
	/// Get the capability.
	/// </summary>
	/// <param name="capability">The capability mode.</param>
	/// <returns>The capability.</returns>
	EXPORT_NEQUEO_MKL_API int query_capability(const int capability)
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
		case 64: return 9; // revision
		case 65: return 1; // numerical consistency, precision and accuracy modes
		case 66: return 1; // threading control
		case 67: return 1; // memory management

						   // LINEAR ALGEBRA
		case 128: return 2;	// basic dense linear algebra (major - breaking)
		case 129: return 0;	// basic dense linear algebra (minor - non-breaking)

							// OPTIMIZATION
		case 256: return 0; // basic optimization

							// FFT
		case 384: return 0; // basic FFT

		default: return 0; // unknown or not supported

		}
	}

	/// <summary>
	/// Set the consistency mode.
	/// </summary>
	/// <param name="mode">The consistency mode.</param>
	EXPORT_NEQUEO_MKL_API void set_consistency_mode(const MKL_INT mode)
	{
		mkl_cbwr_set(mode);
	}

	/// <summary>
	/// Set the vml mode.
	/// </summary>
	/// <param name="mode">The vml mode.</param>
	EXPORT_NEQUEO_MKL_API void set_vml_mode(const MKL_UINT mode)
	{
		vmlSetMode(mode);
	}

	/// <summary>
	/// Set maximum threads.
	/// </summary>
	/// <param name="num_threads">The number of threads.</param>
	EXPORT_NEQUEO_MKL_API void set_max_threads(const MKL_INT num_threads)
	{
		mkl_set_num_threads(num_threads);
	}

	/// <summary>
	/// Set improved consistency.
	/// </summary>
	EXPORT_NEQUEO_MKL_API void SetImprovedConsistency(void)
	{
		// set improved consistency for MKL and vector functions
		mkl_cbwr_set(MKL_CBWR_COMPATIBLE);
		vmlSetMode(VML_HA | VML_DOUBLE_CONSISTENT);
	}

#ifdef __cplusplus
}
#endif /* __cplusplus */
