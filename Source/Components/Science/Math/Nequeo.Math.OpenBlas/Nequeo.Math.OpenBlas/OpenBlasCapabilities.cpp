/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          OpenBlasCapabilities.cpp
*  Purpose :       OpenBlas Capabilities.
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

#include "cblas.h"

#ifdef __cplusplus
extern "C"
{
#endif /* __cplusplus */

	/// <summary>
	/// Get the capability.
	/// </summary>
	/// <param name="capability">The capability mode.</param>
	/// <returns>The capability.</returns>
	EXPORT_NEQUEO_OPENBLAS_API int query_capability(const int capability)
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
		case 66: return 1; // threading control

						   // LINEAR ALGEBRA
		case 128: return 1;	// basic dense linear algebra (major - breaking)
		case 129: return 0;	// basic dense linear algebra (minor - non-breaking)

		default: return 0; // unknown or not supported

		}
	}

	/// <summary>
	/// Set maximum threads.
	/// </summary>
	/// <param name="num_threads">The number of threads.</param>
	EXPORT_NEQUEO_OPENBLAS_API void set_max_threads(const blasint num_threads)
	{
		openblas_set_num_threads(num_threads);
	}

	/// <summary>
	/// Get the build configuration.
	/// </summary>
	/// <returns>The build configuration.</returns>
	EXPORT_NEQUEO_OPENBLAS_API char* get_build_config()
	{
		return openblas_get_config();
	}

	/// <summary>
	/// Get the cpu core.
	/// </summary>
	/// <returns>The cpu core.</returns>
	EXPORT_NEQUEO_OPENBLAS_API char* get_cpu_core()
	{
		return openblas_get_corename();
	}

	/// <summary>
	/// Get the parallel type.
	/// </summary>
	/// <returns>The parallel type.</returns>
	EXPORT_NEQUEO_OPENBLAS_API int get_parallel_type()
	{
		return openblas_get_parallel();
	}

#ifdef __cplusplus
}
#endif /* __cplusplus */
