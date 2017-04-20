/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
*
*  File :          SpecialFunctions.h
*  Purpose :       SpecialFunctions class.
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

#include "stdafx.h"

#include "GlobalMathToolKit.h"

namespace Nequeo {
	namespace Math {
		namespace ToolKit
		{
			///	<summary>
			///	Special Functions.
			///	</summary>
			class EXPORT_NEQUEO_MATH_TOOLKIT_API SpecialFunctions
			{
			public:
				///	<summary>
				///	Special Functions.
				///	</summary>
				SpecialFunctions();

				///	<summary>
				///	Special Functions.
				///	</summary>
				virtual ~SpecialFunctions();

				///	<summary>
				///	Exponential Integral, Re*Int[1, infinity]{dt*exp(-xt)/t}
				///	</summary>
				/// <param name="x">The value at x.</param>
				/// <returns>The result of the operation.</returns>
				double ExponentialIntegral(const double x);

				///	<summary>
				///	Exponential Integral second order, Re*Int[1, infinity]{dt*exp(-xt)/t^2}
				///	</summary>
				/// <param name="x">The value at x.</param>
				/// <returns>The result of the operation.</returns>
				double ExponentialIntegralSecondOrder(const double x);

				///	<summary>
				///	Exponential Integral n order, Re*Int[1, infinity]{dt*exp(-xt)/t^n}
				///	</summary>
				/// <param name="n">The order n.</param>
				/// <param name="x">The value at x.</param>
				/// <returns>The result of the operation.</returns>
				double ExponentialIntegralNOrder(int n, const double x);

				///	<summary>
				///	Gamma Function, Int[0, infinity]{dt*t^(x-1)*exp(-t)}
				///	</summary>
				/// <param name="x">The value at x.</param>
				/// <returns>The result of the operation.</returns>
				double Gamma(const double x);

				///	<summary>
				///	Gamma Function, Gamma(a)*Gamma(b)/Gamma(a+b)
				///	</summary>
				/// <param name="a">The value at a.</param>
				/// <param name="b">The value at b.</param>
				/// <returns>The result of the operation.</returns>
				double Beta(const double a, const double b);

			private:
				bool _disposed;

			};
		}
	}
}