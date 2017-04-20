/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
*
*  File :          Expression.h
*  Purpose :       Expression class.
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
			///	Expression.
			///	</summary>
			class EXPORT_NEQUEO_MATH_TOOLKIT_API Expression
			{
			public:
				///	<summary>
				///	Expression.
				///	</summary>
				Expression();

				///	<summary>
				///	Expression.
				///	</summary>
				virtual ~Expression();

				///	<summary>
				///	Execute the expression.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <returns>The result of the operation.</returns>
				double Execute(const std::string& expression);

				///	<summary>
				///	Execute the expression.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="variableName">The variable name in the expression.</param>
				/// <returns>The result of the operation.</returns>
				double Execute(const std::string& expression, double x, const std::string& variableName = "x");

				///	<summary>
				///	Execute the expression.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="variables">The variables in the expression.</param>
				/// <returns>The result of the operation.</returns>
				double Execute(const std::string& expression, const std::map<std::string, double>& variables);

				///	<summary>
				///	Execute the expression find the derivative.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="variableName">The variable name in the expression.</param>
				/// <param name="variables">The variables in the expression.</param>
				/// <returns>The result of the operation.</returns>
				double Derivative(const std::string& expression, double x, const std::string& variableName, const std::map<std::string, double>& variables);

				///	<summary>
				///	Execute the expression find the derivative.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <returns>The result of the operation.</returns>
				double Derivative(const std::string& expression, double x);

				///	<summary>
				///	Execute the expression find the second derivative.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="variableName">The variable name in the expression.</param>
				/// <param name="variables">The variables in the expression.</param>
				/// <returns>The result of the operation.</returns>
				double DerivativeSecond(const std::string& expression, double x, const std::string& variableName, const std::map<std::string, double>& variables);

				///	<summary>
				///	Execute the expression find the third derivative.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="variableName">The variable name in the expression.</param>
				/// <param name="variables">The variables in the expression.</param>
				/// <returns>The result of the operation.</returns>
				double DerivativeThird(const std::string& expression, double x, const std::string& variableName, const std::map<std::string, double>& variables);

				///	<summary>
				///	Execute the expression find the integral.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="variableName">The variable name in the expression.</param>
				/// <param name="a">The lower bound.</param>
				/// <param name="b">The upper bound.</param>
				/// <param name="variables">The variables in the expression.</param>
				/// <returns>The result of the operation.</returns>
				double Integrate(const std::string& expression, double x, const std::string& variableName, double a, double b, const std::map<std::string, double>& variables);

				///	<summary>
				///	Execute the expression find the integral.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="a">The lower bound.</param>
				/// <param name="b">The upper bound.</param>
				/// <returns>The result of the operation.</returns>
				double Integrate(const std::string& expression, double a, double b);

				///	<summary>
				///	Execute the expression compute the result.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="result">The result of the operation.</param>
				/// <returns>True if no error; else false.</returns>
				bool Compute(const std::string& expression, double* result);

				///	<summary>
				///	Execute the expression compute the result.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="result">The result of the operation.</param>
				/// <returns>True if no error; else false.</returns>
				bool Compute(const std::string& expression, double x, double* result);

				///	<summary>
				///	Execute the expression compute the result.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="y">The variable value.</param>
				/// <param name="result">The result of the operation.</param>
				/// <returns>True if no error; else false.</returns>
				bool Compute(const std::string& expression, double x, double y, double* result);

				///	<summary>
				///	Execute the expression compute the result.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="y">The variable value.</param>
				/// <param name="z">The variable value.</param>
				/// <param name="result">The result of the operation.</param>
				/// <returns>True if no error; else false.</returns>
				bool Compute(const std::string& expression, double x, double y, double z, double* result);

			private:
				bool _disposed;

			};
		}
	}
}