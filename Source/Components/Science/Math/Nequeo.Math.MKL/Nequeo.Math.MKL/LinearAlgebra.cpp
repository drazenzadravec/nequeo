/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          LinearAlgebra.h
*  Purpose :       Linear Algebra class.
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

#include "LinearAlgebra.h"

using namespace Nequeo::Math::MKL;

///	<summary>
///	Linear algebra solver.
///	</summary>
LinearAlgebra::LinearAlgebra() : _disposed(false)
{
}

///	<summary>
///	Linear algebra solver destructor.
///	</summary>
LinearAlgebra::~LinearAlgebra()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	The function perform a vector-vector copy operation defined as y = x.
///	</summary>
/// <param name="n">Specifies the number of elements in vectors x and y.</param>
/// <param name="x">Array, size at least (1 + (n-1)*abs(incx)).</param>
/// <param name="incx">Specifies the increment for the elements of x.</param>
/// <param name="y">Contains a copy of the vector x if n is positive. Otherwise, parameters are unaltered.</param>
/// <param name="incy">Specifies the increment for the elements of y.</param>
void  LinearAlgebra::CopyVector(int n, double x[], int incx, double *y, int incy)
{
	cblas_dcopy(n, x, incx, y, incy);
}

///	<summary>
///	This function performs a vector-vector operation of computing a scalar dot product of two real vectors x and y.
///	</summary>
/// <param name="n">Specifies the number of elements in vectors x and y.</param>
/// <param name="x">Array, size at least (1+(n-1)*abs(incx)).</param>
/// <param name="incx">Specifies the increment for the elements of x.</param>
/// <param name="y">Array, size at least (1+(n-1)*abs(incy)).</param>
/// <param name="incy">Specifies the increment for the elements of y.</param>
/// <returns>The scalar product.</returns>
double LinearAlgebra::ScalarDotProduct(int n, double x[], int incx, double y[], int incy)
{
	return cblas_ddot(n, x, incx, y, incy);
}