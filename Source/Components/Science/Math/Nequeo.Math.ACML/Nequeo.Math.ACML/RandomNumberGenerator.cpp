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

#include "acml.h"
#include "fftw3.h"

#include "RandomNumberGenerator.h"

using namespace Nequeo::Math::ACML;

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

	int seeds[10], state[20];
	int genid, info, lseed, lstate, subid;

	// Use the basic generator as the base generator.
	genid = 1;
	subid = 1;

	// Populate the seed array, basic generator needs one seed, and a STATE array of length 16.
	lstate = 16;
	lseed = 1;
	seeds[0] = seed;

	// Initialize the base generator
	drandinitialize(genid, subid, seeds, &lseed, state, &lstate, &info);

	// If initialised.
	if (info == 0)
	{
		double *numbers = new double[size];

		// Generate the numbers.
		drandgaussian(size, mean, sigma, state, numbers, &info);

		// If generated.
		if (info == 0)
		{
			// Assign the random number.
			for (size_t i = 0; i < size; i++)
			{
				// Add the number.
				result.push_back(numbers[i]);
			}
		}

		// Delete the numbers array.
		delete[] numbers;
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

	int seeds[10], state[20];
	int genid, info, lseed, lstate, subid;

	// Use the basic generator as the base generator.
	genid = 1;
	subid = 1;

	// Populate the seed array, basic generator needs one seed, and a STATE array of length 16.
	lstate = 16;
	lseed = 1;
	seeds[0] = seed;

	// Initialize the base generator
	drandinitialize(genid, subid, seeds, &lseed, state, &lstate, &info);

	// If initialised.
	if (info == 0)
	{
		double *numbers = new double[size];

		// Generate the numbers.
		dranduniform(size, minimum, maximum, state, numbers, &info);

		// If generated.
		if (info == 0)
		{
			// Assign the random number.
			for (size_t i = 0; i < size; i++)
			{
				// Add the number.
				result.push_back(numbers[i]);
			}
		}

		// Delete the numbers array.
		delete[] numbers;
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

	int seeds[10], state[20];
	int genid, info, lseed, lstate, subid;

	// Use the basic generator as the base generator.
	genid = 1;
	subid = 1;

	// Populate the seed array, basic generator needs one seed, and a STATE array of length 16.
	lstate = 16;
	lseed = 1;
	seeds[0] = seed;

	// Initialize the base generator
	drandinitialize(genid, subid, seeds, &lseed, state, &lstate, &info);

	// If initialised.
	if (info == 0)
	{
		double *numbers = new double[size];

		// Generate the numbers.
		drandgaussian(size, mean, sigma, state, numbers, &info);

		// If generated.
		if (info == 0)
		{
			// Assign the random number.
			for (size_t i = 0; i < size; i++)
			{
				// Add the number.
				result.push_back(numbers[i]);
			}
		}

		// Delete the numbers array.
		delete[] numbers;
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
	
	int seeds[10], state[20];
	int genid, info, lseed, lstate, subid;

	// Use the basic generator as the base generator.
	genid = 1;
	subid = 1;

	// Populate the seed array, basic generator needs one seed, and a STATE array of length 16.
	lstate = 16;
	lseed = 1;
	seeds[0] = seed;

	// Initialize the base generator
	drandinitialize(genid, subid, seeds, &lseed, state, &lstate, &info);

	// If initialised.
	if (info == 0)
	{
		double *numbers = new double[size];

		// Generate the numbers.
		drandbeta(size, shapeP, shapeQ, state, numbers, &info);

		// If generated.
		if (info == 0)
		{
			// Assign the random number.
			for (size_t i = 0; i < size; i++)
			{
				// Add the number.
				result.push_back(numbers[i]);
			}
		}

		// Delete the numbers array.
		delete[] numbers;
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
	int seeds[10], state[20];
	int genid, info, lseed, lstate, subid;

	// Use the basic generator as the base generator.
	genid = 1;
	subid = 1;

	// Populate the seed array, basic generator needs one seed, and a STATE array of length 16.
	lstate = 16;
	lseed = 1;
	seeds[0] = seed;

	// Initialize the base generator
	drandinitialize(genid, subid, seeds, &lseed, state, &lstate, &info);

	// If initialised.
	if (info == 0)
	{
		// Generate the numbers.
		drandgaussian(size, mean, sigma, state, numbers, &info);
	}

	// Return the results.
	return info;
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
	int seeds[10], state[20];
	int genid, info, lseed, lstate, subid;

	// Use the basic generator as the base generator.
	genid = 1;
	subid = 1;

	// Populate the seed array, basic generator needs one seed, and a STATE array of length 16.
	lstate = 16;
	lseed = 1;
	seeds[0] = seed;

	// Initialize the base generator
	drandinitialize(genid, subid, seeds, &lseed, state, &lstate, &info);

	// If initialised.
	if (info == 0)
	{
		// Generate the numbers.
		drandgaussian(size, mean, sigma, state, numbers, &info);
	}

	// Return the results.
	return info;
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
	int seeds[10], state[20];
	int genid, info, lseed, lstate, subid;

	// Use the basic generator as the base generator.
	genid = 1;
	subid = 1;

	// Populate the seed array, basic generator needs one seed, and a STATE array of length 16.
	lstate = 16;
	lseed = 1;
	seeds[0] = seed;

	// Initialize the base generator
	drandinitialize(genid, subid, seeds, &lseed, state, &lstate, &info);

	// If initialised.
	if (info == 0)
	{
		// Generate the numbers.
		drandbeta(size, shapeP, shapeQ, state, numbers, &info);
	}

	// Return the results.
	return info;
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
	int seeds[10], state[20];
	int genid, info, lseed, lstate, subid;

	// Use the basic generator as the base generator.
	genid = 1;
	subid = 1;

	// Populate the seed array, basic generator needs one seed, and a STATE array of length 16.
	lstate = 16;
	lseed = 1;
	seeds[0] = seed;

	// Initialize the base generator
	drandinitialize(genid, subid, seeds, &lseed, state, &lstate, &info);

	// If initialised.
	if (info == 0)
	{
		// Generate the numbers.
		dranduniform(size, minimum, maximum, state, numbers, &info);
	}

	// Return the results.
	return info;
}