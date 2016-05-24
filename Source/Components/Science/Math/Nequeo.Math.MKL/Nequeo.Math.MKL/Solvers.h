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

#pragma once

#ifndef _SOLVERS_H
#define _SOLVERS_H

#include "stdafx.h"

namespace Nequeo
{
	namespace Math
	{
		namespace MKL
		{
			///	<summary>
			///	Vector and matrix solvers.
			///	</summary>
			class EXPORT_NEQUEO_MKL_API Solvers
			{
			public:
				///	<summary>
				///	Vector and matrix solvers.
				///	</summary>
				Solvers();

				///	<summary>
				///	Vector and matrix solvers destructor.
				///	</summary>
				~Solvers();

				/// <summary>
				/// Computes a square root of vector elements.
				/// </summary>
				/// <param name="n">The number of elements to be calculated.</param>
				/// <param name="a">An array that contains the input vector.</param>
				/// <returns>An array that contains the output vector.</returns>
				std::vector<float> Sqrt(const int n, const float a[]);

				/// <summary>
				/// Computes a square root of vector elements.
				/// </summary>
				/// <param name="n">The number of elements to be calculated.</param>
				/// <param name="a">An array that contains the input vector.</param>
				/// <returns>An array that contains the output vector.</returns>
				std::vector<double> Sqrt(const int n, const double a[]);

				/// <summary>
				/// Computes a square root of vector elements.
				/// </summary>
				/// <param name="n">The number of elements to be calculated.</param>
				/// <param name="a">An array that contains the input vector.</param>
				/// <returns>An array that contains the output vector.</returns>
				std::vector<ComplexFloat> Sqrt(const int n, const ComplexFloat a[]);

				/// <summary>
				/// Computes a square root of vector elements.
				/// </summary>
				/// <param name="n">The number of elements to be calculated.</param>
				/// <param name="a">An array that contains the input vector.</param>
				/// <returns>An array that contains the output vector.</returns>
				std::vector<ComplexDouble> Sqrt(const int n, const ComplexDouble a[]);

				/// <summary>
				/// Computes a square root of vector elements.
				/// </summary>
				/// <param name="n">The number of elements to be calculated.</param>
				/// <param name="a">An array that contains the input vector.</param>
				/// <param name="r">An array that contains the output vector.</param>
				void Sqrt(const int n, const float a[], float *r);

				/// <summary>
				/// Computes a square root of vector elements.
				/// </summary>
				/// <param name="n">The number of elements to be calculated.</param>
				/// <param name="a">An array that contains the input vector.</param>
				/// <param name="r">An array that contains the output vector.</param>
				void Sqrt(const int n, const double a[], double *r);

				/// <summary>
				/// Computes a square root of vector elements.
				/// </summary>
				/// <param name="n">The number of elements to be calculated.</param>
				/// <param name="a">An array that contains the input vector.</param>
				/// <param name="r">An array that contains the output vector.</param>
				void Sqrt(const int n, const MKL_Complex8 a[], MKL_Complex8 *r);

				/// <summary>
				/// Computes a square root of vector elements.
				/// </summary>
				/// <param name="n">The number of elements to be calculated.</param>
				/// <param name="a">An array that contains the input vector.</param>
				/// <param name="r">An array that contains the output vector.</param>
				void Sqrt(const int n, const MKL_Complex16 a[], MKL_Complex16 *r);

			private:
				bool _disposed;

			};
		}
	}
}
#endif