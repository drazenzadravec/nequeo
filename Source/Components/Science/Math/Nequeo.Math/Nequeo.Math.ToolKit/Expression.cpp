/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
*
*  File :          Expression.cpp
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

#include "stdafx.h"

#include "Expression.h"

#include "ExpressionTemplates.cpp"

using namespace Nequeo::Math::ToolKit;

///	<summary>
///	Expression.
///	</summary>
Expression::Expression() :
	_disposed(false)
{
}

///	<summary>
///	Expression.
///	</summary>
Expression::~Expression()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Execute the expression.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <returns>The result of the operation.</returns>
double Expression::Execute(const std::string& expression)
{
	// Execute.
	TemplateContainer container;
	double result = container.ExecuteOpen<double>(expression);
	return result;
}

///	<summary>
///	Execute the expression.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="x">The variable value.</param>
/// <param name="variableName">The variable name in the expression.</param>
/// <returns>The result of the operation.</returns>
double Expression::Execute(const std::string& expression, double x, const std::string& variableName)
{
	// Execute.
	TemplateContainer container;
	double result = container.ExecuteExpression<double>(expression, x, variableName);
	return result;
}

///	<summary>
///	Execute the expression.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="x">The variable value.</param>
/// <param name="variableName">The variable name in the expression.</param>
/// <returns>The result of the operation.</returns>
float Expression::Execute(const std::string& expression, float x, const std::string& variableName)
{
	// Execute.
	TemplateContainer container;
	double result = container.ExecuteExpression<double>(expression, (double)x, variableName);
	return (float)result;
}

///	<summary>
///	Execute the expression.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="x">The variable value.</param>
/// <param name="variableName">The variable name in the expression.</param>
/// <returns>The result of the operation.</returns>
long Expression::Execute(const std::string& expression, long x, const std::string& variableName)
{
	// Execute.
	TemplateContainer container;
	double result = container.ExecuteExpression<double>(expression, (double)x, variableName);
	return (long)result;
}

///	<summary>
///	Execute the expression.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="x">The variable value.</param>
/// <param name="variableName">The variable name in the expression.</param>
/// <returns>The result of the operation.</returns>
int Expression::Execute(const std::string& expression, int x, const std::string& variableName)
{
	// Execute.
	TemplateContainer container;
	double result = container.ExecuteExpression<double>(expression, (double)x, variableName);
	return (int)result;
}

///	<summary>
///	Execute the expression.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="variables">The variables in the expression.</param>
/// <returns>The result of the operation.</returns>
double Expression::Execute(const std::string& expression, const std::map<std::string, double>& variables)
{
	// Execute.
	TemplateContainer container;
	double result = container.ExecuteExpressionMulti<double>(expression, variables);
	return result;
}