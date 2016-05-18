/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Solvers.h
*  Purpose :       Solvers class.
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

#include "Solvers.h"

using namespace Nequeo::Math::MKL;

///	<summary>
///	Vector and matrix solvers.
///	</summary>
Solvers::Solvers() : _disposed(false)
{
}

///	<summary>
///	Vector and matrix solvers destructor.
///	</summary>
Solvers::~Solvers()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Computes a square root of vector elements.
/// </summary>
/// <param name="n">The number of elements to be calculated.</param>
/// <param name="a">An array that contains the input vector.</param>
/// <returns>An array that contains the output vector.</returns>
std::vector<float> Solvers::Sqrt(const int n, const float a[])
{
	float *r = new float[n];

	// Compute the square root.
	vsSqrt(n, a, r);

	// Assign the random number.
	std::vector<float> result;
	for (size_t i = 0; i < n; i++)
	{
		// Add the number.
		result.push_back(r[i]);
	}

	// Delete the r array.
	delete r;

	// Return the results.
	return result;
}

/// <summary>
/// Computes a square root of vector elements.
/// </summary>
/// <param name="n">The number of elements to be calculated.</param>
/// <param name="a">An array that contains the input vector.</param>
/// <returns>An array that contains the output vector.</returns>
std::vector<double> Solvers::Sqrt(const int n, const double a[])
{
	double *r = new double[n];

	// Compute the square root.
	vdSqrt(n, a, r);

	// Assign the random number.
	std::vector<double> result;
	for (size_t i = 0; i < n; i++)
	{
		// Add the number.
		result.push_back(r[i]);
	}

	// Delete the r array.
	delete r;

	// Return the results.
	return result;
}

/// <summary>
/// Computes a square root of vector elements.
/// </summary>
/// <param name="n">The number of elements to be calculated.</param>
/// <param name="a">An array that contains the input vector.</param>
/// <returns>An array that contains the output vector.</returns>
std::vector<ComplexFloat> Solvers::Sqrt(const int n, const ComplexFloat a[])
{
	MKL_Complex8 *r = new MKL_Complex8[n];
	MKL_Complex8 *ca = new MKL_Complex8[n];

	// Convert the type.
	for (size_t i = 0; i < n; i++)
	{
		ca[i].imag = a[i].imag;
		ca[i].real = a[i].real;
	}

	// Compute the square root.
	vcSqrt(n, ca, r);

	// Assign the random number.
	std::vector<ComplexFloat> result;
	for (size_t i = 0; i < n; i++)
	{
		ComplexFloat rc;
		rc.imag = r[i].imag;
		rc.real = r[i].real;

		// Add the number.
		result.push_back(rc);
	}

	// Delete the r array.
	delete r;
	delete ca;

	// Return the results.
	return result;
}

/// <summary>
/// Computes a square root of vector elements.
/// </summary>
/// <param name="n">The number of elements to be calculated.</param>
/// <param name="a">An array that contains the input vector.</param>
/// <returns>An array that contains the output vector.</returns>
std::vector<ComplexDouble> Solvers::Sqrt(const int n, const ComplexDouble a[])
{
	MKL_Complex16 *r = new MKL_Complex16[n];
	MKL_Complex16 *ca = new MKL_Complex16[n];

	// Convert the type.
	for (size_t i = 0; i < n; i++)
	{
		ca[i].imag = a[i].imag;
		ca[i].real = a[i].real;
	}

	// Compute the square root.
	vzSqrt(n, ca, r);

	// Assign the random number.
	std::vector<ComplexDouble> result;
	for (size_t i = 0; i < n; i++)
	{
		ComplexDouble rc;
		rc.imag = r[i].imag;
		rc.real = r[i].real;

		// Add the number.
		result.push_back(rc);
	}

	// Delete the r array.
	delete r;
	delete ca;

	// Return the results.
	return result;
}