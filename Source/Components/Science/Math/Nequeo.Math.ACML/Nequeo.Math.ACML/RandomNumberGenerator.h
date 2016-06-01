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

#pragma once

#ifndef _RANDOMNUMBERGENERATOR_H
#define _RANDOMNUMBERGENERATOR_H

#include "stdafx.h"

namespace Nequeo
{
	namespace Math
	{
		namespace ACML
		{
			///	<summary>
			///	Random number generator.
			///	</summary>
			class EXPORT_NEQUEO_ACML_API RandomNumberGenerator
			{
			public:
				///	<summary>
				///	Random number generator.
				///	</summary>
				RandomNumberGenerator();

				///	<summary>
				///	Random number generator destructor.
				///	</summary>
				~RandomNumberGenerator();

				/// <summary>
				/// Basic normal distribution random number generator.
				/// </summary>
				/// <param name="size">The number of random numbers to generate.</param>
				/// <param name="mean">The mean of the normal distribution.</param>
				/// <param name="sigma">The standard deviation of the normal distribution.</param>
				/// <param name="seed">The random seed.</param>
				/// <returns>The list of random numbers.</returns>
				std::vector<double> Basic(int size, double mean, double sigma, unsigned int seed = 777);

				/// <summary>
				/// Uniformly distribution random number generator.
				/// </summary>
				/// <param name="size">The number of random numbers to generate.</param>
				/// <param name="minimum">The minimum number to generate.</param>
				/// <param name="maximum">The maximum number to generate.</param>
				/// <param name="seed">The random seed.</param>
				/// <returns>The list of random numbers.</returns>
				std::vector<double> Uniform(int size, double minimum, double maximum, unsigned int seed = 777);

				/// <summary>
				/// Normal distribution random number generator.
				/// </summary>
				/// <param name="size">The number of random numbers to generate.</param>
				/// <param name="mean">The mean of the normal distribution.</param>
				/// <param name="sigma">The standard deviation of the normal distribution.</param>
				/// <param name="seed">The random seed.</param>
				/// <returns>The list of random numbers.</returns>
				std::vector<double> NormalDistribution(int size, double mean, double sigma, unsigned int seed = 777);

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
				std::vector<double> Beta(int size, double shapeP, double shapeQ, double a, double beta, unsigned int seed = 777);

				/// <summary>
				/// Basic normal distribution random number generator.
				/// </summary>
				/// <param name="size">The number of random numbers to generate.</param>
				/// <param name="mean">The mean of the normal distribution.</param>
				/// <param name="sigma">The standard deviation of the normal distribution.</param>
				/// <param name="numbers">The list of random numbers.</param>
				/// <param name="seed">The random seed.</param>
				/// <returns>Zero if successful; else error.</returns>
				int Basic(int size, double mean, double sigma, double *numbers, unsigned int seed = 777);

				/// <summary>
				/// Normal distribution random number generator.
				/// </summary>
				/// <param name="size">The number of random numbers to generate.</param>
				/// <param name="mean">The mean of the normal distribution.</param>
				/// <param name="sigma">The standard deviation of the normal distribution.</param>
				/// <param name="numbers">The list of random numbers.</param>
				/// <param name="seed">The random seed.</param>
				/// <returns>Zero if successful; else error.</returns>
				int NormalDistribution(int size, double mean, double sigma, double *numbers, unsigned int seed = 777);

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
				int Beta(int size, double shapeP, double shapeQ, double a, double beta, double *numbers, unsigned int seed = 777);

				/// <summary>
				/// Uniformly distribution random number generator.
				/// </summary>
				/// <param name="size">The number of random numbers to generate.</param>
				/// <param name="minimum">The minimum number to generate.</param>
				/// <param name="maximum">The maximum number to generate.</param>
				/// <param name="numbers">The list of random numbers.</param>
				/// <param name="seed">The random seed.</param>
				/// <returns>Zero if successful; else error.</returns>
				int Uniform(int size, double minimum, double maximum, double *numbers, unsigned int seed = 777);

			private:
				bool _disposed;

			};
		}
	}
}
#endif