/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MKLMemory.cpp
*  Purpose :       MKL Memory.
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

#if __cplusplus
extern "C" 
{
#endif

	/// <summary>
	/// Free the resource buffers.
	/// </summary>
	EXPORT_NEQUEO_MKL_API void free_buffers(void) 
	{
		mkl_free_buffers();
	}

	/// <summary>
	/// Free thread resources buffers.
	/// </summary>
	EXPORT_NEQUEO_MKL_API void thread_free_buffers(void) 
	{
		mkl_thread_free_buffers();
	}

	/// <summary>
	/// Disable fast memory.
	/// </summary>
	/// <returns>0 if diabled.</returns>
	EXPORT_NEQUEO_MKL_API int disable_fast_mm(void) 
	{
		return mkl_disable_fast_mm();
	}

	/// <summary>
	/// Get the memory statistics.
	/// </summary>
	/// <param name="AllocatedBuffers">The allocated buffers.</param>
	/// <returns>The statistics.</returns>
	EXPORT_NEQUEO_MKL_API MKL_INT64 mem_stat(int* AllocatedBuffers) 
	{
		return mkl_mem_stat(AllocatedBuffers);
	}

	/// <summary>
	/// Get the peak memory usage.
	/// </summary>
	/// <param name="mode">The usage mode.</param>
	/// <returns>The usage.</returns>
	EXPORT_NEQUEO_MKL_API MKL_INT64 peak_mem_usage(int mode) 
	{
		return mkl_peak_mem_usage(mode);
	}

#if __cplusplus
}
#endif