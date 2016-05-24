/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          VectorFunctions.cpp
*  Purpose :       Common vector functions.
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

EXPORT_NEQUEO_MKL_API void s_vector_add(const int n, const float x[], const float y[], float result[]) 
{
	vsAdd(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void s_vector_subtract(const int n, const float x[], const float y[], float result[]) 
{
	vsSub(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void s_vector_multiply(const int n, const float x[], const float y[], float result[]) 
{
	vsMul(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void s_vector_divide(const int n, const float x[], const float y[], float result[]) 
{
	vsDiv(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void d_vector_add(const int n, const double x[], const double y[], double result[]) 
{
	vdAdd(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void d_vector_subtract(const int n, const double x[], const double y[], double result[]) 
{
	vdSub(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void d_vector_multiply(const int n, const double x[], const double y[], double result[]) 
{
	vdMul(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void d_vector_divide(const int n, const double x[], const double y[], double result[]) 
{
	vdDiv(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void c_vector_add(const int n, const MKL_Complex8 x[], const MKL_Complex8 y[], MKL_Complex8 result[]) 
{
	vcAdd(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void c_vector_subtract(const int n, const MKL_Complex8 x[], const MKL_Complex8 y[], MKL_Complex8 result[]) 
{
	vcSub(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void c_vector_multiply(const int n, const MKL_Complex8 x[], const MKL_Complex8 y[], MKL_Complex8 result[]) 
{
	vcMul(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void c_vector_divide(const int n, const MKL_Complex8 x[], const MKL_Complex8 y[], MKL_Complex8 result[]) 
{
	vcDiv(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void z_vector_add(const int n, const MKL_Complex16 x[], const MKL_Complex16 y[], MKL_Complex16 result[]) 
{
	vzAdd(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void z_vector_subtract(const int n, const MKL_Complex16 x[], const MKL_Complex16 y[], MKL_Complex16 result[]) 
{
	vzSub(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void z_vector_multiply(const int n, const MKL_Complex16 x[], const MKL_Complex16 y[], MKL_Complex16 result[]) 
{
	vzMul(n, x, y, result);
}

EXPORT_NEQUEO_MKL_API void z_vector_divide(const int n, const MKL_Complex16 x[], const MKL_Complex16 y[], MKL_Complex16 result[]) 
{
	vzDiv(n, x, y, result);
}
