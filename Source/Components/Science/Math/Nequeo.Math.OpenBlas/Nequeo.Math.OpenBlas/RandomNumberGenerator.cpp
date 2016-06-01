/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          RandomNumberGenerator.cpp
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

#include "cblas.h"

#include "RandomNumberGenerator.h"

using namespace Nequeo::Math::OpenBlas;

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
	std::vector<double> result;

	// non-deterministic generator
	// mt19937 gen(rd());
	std::random_device rd;
	std::mt19937 gen(seed);

	// Normal distribution random number generator
	std::normal_distribution<> distr(mean, sigma);

	// For the number of samples to generate.
	for (int i = 0; i < size; ++i)
	{
		// Generate the distribution.
		result.push_back(distr(gen));
	}

	// Return the results.
	return result;
}

/// <summary>
/// Uniformly distribution random number generator.
/// </summary>
/// <param name="size">The number of random numbers to generate.</param>
/// <param name="minimum">The minimum number to generate.</param>
/// <param name="maximum">The maximum number to generate.</param>
/// <param name="seed">The random seed.</param>
/// <returns>The list of random numbers.</returns>
std::vector<double> RandomNumberGenerator::Uniform(int size, double minimum, double maximum, unsigned int seed)
{
	std::vector<double> result;

	// non-deterministic generator
	// mt19937 gen(rd());
	std::random_device rd;
	std::mt19937 gen(seed);

	// Uniformly distribution random number generator
	std::uniform_real_distribution<> distr(minimum, maximum);

	// For the number of samples to generate.
	for (int i = 0; i < size; ++i)
	{
		// Generate the distribution.
		result.push_back(distr(gen));
	}

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
	std::vector<double> result;

	// non-deterministic generator
	// mt19937 gen(rd());
	std::random_device rd;
	std::mt19937 gen(seed);

	// Normal distribution random number generator
	std::normal_distribution<> distr(mean, sigma);

	// For the number of samples to generate.
	for (int i = 0; i < size; ++i)
	{
		// Generate the distribution.
		result.push_back(distr(gen));
	}

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
	std::vector<double> result;

	// non-deterministic generator
	// mt19937 gen(rd());
	std::random_device rd;
	std::mt19937 gen(seed);

	// Normal distribution random number generator
	std::gamma_distribution<> distr(a, beta);

	// For the number of samples to generate.
	for (int i = 0; i < size; ++i)
	{
		// Generate the distribution.
		result.push_back(distr(gen));
	}

	// Return the results.
	return result;
}

/// <summary>
/// Basic normal distribution random number generator.
/// </summary>
/// <param name="size">The number of random numbers to generate.</param>
/// <param name="mean">The mean of the normal distribution.</param>
/// <param name="sigma">The standard deviation of the normal distribution.</param>
/// <param name="numbers">The list of random numbers.</param>
/// <param name="seed">The random seed.</param>
/// <returns>Zero if successful; else error.</returns>
int RandomNumberGenerator::Basic(int size, double mean, double sigma, double *numbers, unsigned int seed)
{
	// non-deterministic generator
	// mt19937 gen(rd());
	std::random_device rd;
	std::mt19937 gen(seed);

	// Normal distribution random number generator
	std::normal_distribution<> distr(mean, sigma);

	// For the number of samples to generate.
	for (int i = 0; i < size; ++i)
	{
		// Generate the distribution.
		numbers[i] = distr(gen);
	}

	return 0;
}

/// <summary>
/// Normal distribution random number generator.
/// </summary>
/// <param name="size">The number of random numbers to generate.</param>
/// <param name="mean">The mean of the normal distribution.</param>
/// <param name="sigma">The standard deviation of the normal distribution.</param>
/// <param name="numbers">The list of random numbers.</param>
/// <param name="seed">The random seed.</param>
/// <returns>Zero if successful; else error.</returns>
int RandomNumberGenerator::NormalDistribution(int size, double mean, double sigma, double *numbers, unsigned int seed)
{
	// non-deterministic generator
	// mt19937 gen(rd());
	std::random_device rd;
	std::mt19937 gen(seed);

	// Normal distribution random number generator
	std::normal_distribution<> distr(mean, sigma);

	// For the number of samples to generate.
	for (int i = 0; i < size; ++i)
	{
		// Generate the distribution.
		numbers[i] = distr(gen);
	}

	return 0;
}

/// <summary>
/// Beta distributed random number generator.
/// </summary>
/// <param name="size">The number of random numbers to generate.</param>
/// <param name="shapeP">The shape p.</param>
/// <param name="shapeQ">The shape q.</param>
/// <param name="a">The displacement.</param>
/// <param name="beta">The scalefactor.</param>
/// <param name="numbers">The list of random numbers.</param>
/// <param name="seed">The random seed.</param>
/// <returns>Zero if successful; else error.</returns>
int RandomNumberGenerator::Beta(int size, double shapeP, double shapeQ, double a, double beta, double *numbers, unsigned int seed)
{
	// non-deterministic generator
	// mt19937 gen(rd());
	std::random_device rd;
	std::mt19937 gen(seed);

	// Normal distribution random number generator
	std::gamma_distribution<> distr(a, beta);

	// For the number of samples to generate.
	for (int i = 0; i < size; ++i)
	{
		// Generate the distribution.
		numbers[i] = distr(gen);
	}

	return 0;
}

/// <summary>
/// Uniformly distribution random number generator.
/// </summary>
/// <param name="size">The number of random numbers to generate.</param>
/// <param name="minimum">The minimum number to generate.</param>
/// <param name="maximum">The maximum number to generate.</param>
/// <param name="numbers">The list of random numbers.</param>
/// <param name="seed">The random seed.</param>
/// <returns>Zero if successful; else error.</returns>
int RandomNumberGenerator::Uniform(int size, double minimum, double maximum, double *numbers, unsigned int seed)
{
	// non-deterministic generator
	// mt19937 gen(rd());
	std::random_device rd;
	std::mt19937 gen(seed);

	// Uniformly distribution random number generator
	std::uniform_real_distribution<> distr(minimum, maximum);

	// For the number of samples to generate.
	for (int i = 0; i < size; ++i)
	{
		// Generate the distribution.
		numbers[i] = distr(gen);
	}

	return 0;
}