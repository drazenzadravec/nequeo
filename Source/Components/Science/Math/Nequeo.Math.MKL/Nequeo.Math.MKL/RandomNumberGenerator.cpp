/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          RandomNumberGenerator.h
*  Purpose :       Random Number Generator class.
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

#include "RandomNumberGenerator.h"

using namespace Nequeo::Math::MKL;

///	<summary>
///	Random number generator.
///	</summary>
RandomNumberGenerator::RandomNumberGenerator() : _disposed(false)
{
}

///	<summary>
///	Random number generator destructor.
///	</summary>
RandomNumberGenerator::~RandomNumberGenerator()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Basic normal distribution random number generator.
/// </summary>
/// <param name="size">The number of random numbers to generate.</param>
/// <param name="mean">The mean of the normal distribution.</param>
/// <param name="sigma">The standard deviation of the normal distribution.</param>
/// <param name="seed">The random seed.</param>
/// <returns>The list of random numbers.</returns>
std::vector<double> RandomNumberGenerator::Basic(int size, double mean, double sigma, unsigned int seed)
{
	double *numbers = new double[size];
	VSLStreamStatePtr stream;
	
	// Initializing
	vslNewStream(&stream, VSL_BRNG_MT19937, seed);

	// Generate the numbers.
	vdRngGaussian(VSL_RNG_METHOD_GAUSSIAN_ICDF, stream, size, numbers, mean, sigma);

	// Deleting the stream.
	vslDeleteStream(&stream);

	// Assign the random number.
	std::vector<double> result;
	for (size_t i = 0; i < size; i++)
	{
		// Add the number.
		result.push_back(numbers[i]);
	}

	// Delete the numbers array.
	delete numbers;

	// Return the results.
	return result;
}

/// <summary>
/// Normal distribution random number generator.
/// </summary>
/// <param name="size">The number of random numbers to generate.</param>
/// <param name="mean">The mean of the normal distribution.</param>
/// <param name="sigma">The standard deviation of the normal distribution.</param>
/// <param name="seed">The random seed.</param>
/// <returns>The list of random numbers.</returns>
std::vector<double> RandomNumberGenerator::NormalDistribution(int size, double mean, double sigma, unsigned int seed)
{
	double *numbers = new double[size];
	VSLStreamStatePtr stream;

	// Initializing
	vslNewStream(&stream, VSL_BRNG_MCG31, seed);

	// Generate the numbers.
	vdRngGaussian(VSL_RNG_METHOD_GAUSSIAN_ICDF, stream, size, numbers, mean, sigma);

	// Deleting the stream.
	vslDeleteStream(&stream);

	// Assign the random number.
	std::vector<double> result;
	for (size_t i = 0; i < size; i++)
	{
		// Add the number.
		result.push_back(numbers[i]);
	}

	// Delete the numbers array.
	delete numbers;

	// Return the results.
	return result;
}

/// <summary>
/// Beta distributed random number generator.
/// </summary>
/// <param name="size">The number of random numbers to generate.</param>
/// <param name="shapeP">The shape p.</param>
/// <param name="shapeQ">The shape q.</param>
/// <param name="a">The displacement.</param>
/// <param name="beta">The scalefactor.</param>
/// <param name="seed">The random seed.</param>
/// <returns>The list of random numbers.</returns>
std::vector<double> RandomNumberGenerator::Beta(int size, double shapeP, double shapeQ, double a, double beta, unsigned int seed)
{
	double *numbers = new double[size];
	VSLStreamStatePtr stream;

	// Initializing
	vslNewStream(&stream, VSL_BRNG_MCG31, seed);

	// Generate the numbers.
	vdRngBeta(VSL_RNG_METHOD_BETA_CJA, stream, size, numbers, shapeP, shapeQ, a, beta);

	// Deleting the stream.
	vslDeleteStream(&stream);

	// Assign the random number.
	std::vector<double> result;
	for (size_t i = 0; i < size; i++)
	{
		// Add the number.
		result.push_back(numbers[i]);
	}

	// Delete the numbers array.
	delete numbers;

	// Return the results.
	return result;
}