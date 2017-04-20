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

#include <gsl/gsl_math.h>
#include <gsl/gsl_deriv.h>
#include <gsl/gsl_integration.h>

using namespace Nequeo::Math::ToolKit;

///	<summary>
///	Struct global expression function internal.
///	</summary>
struct GlobalExpressionStruct
{
	std::string expression;
	std::string variableName;
	std::map<std::string, double> variables;
	int executionIndex;
};

typedef struct GlobalExpressionStruct GlobalExpression;

///	<summary>
///	Global expression function internal.
///	</summary>
double GlobalExpressionFunctionIn(double, void*);

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
/// <param name="variables">The variables in the expression.</param>
/// <returns>The result of the operation.</returns>
double Expression::Execute(const std::string& expression, const std::map<std::string, double>& variables)
{
	// Execute.
	TemplateContainer container;
	double result = container.ExecuteExpressionMulti<double>(expression, variables);
	return result;
}
///	<summary>
///	Execute the expression find the derivative.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="x">The variable value.</param>
/// <returns>The result of the operation.</returns>
double Expression::Derivative(const std::string& expression, double x)
{
	double result, abserr;

	// Assign the expression vars.
	GlobalExpression globalExpression;
	globalExpression.expression = expression;
	globalExpression.executionIndex = 1;
	globalExpression.variableName = "x";

	// Assign the function.
	gsl_function func;
	func.function = &GlobalExpressionFunctionIn;
	func.params = &globalExpression;

	// Execute.
	gsl_deriv_central(&func, x, 1e-8, &result, &abserr);

	return result;
}

///	<summary>
///	Execute the expression find the derivative.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="x">The variable value.</param>
/// <param name="variableName">The variable name in the expression.</param>
/// <param name="variables">The variables in the expression.</param>
/// <returns>The result of the operation.</returns>
double Expression::Derivative(const std::string& expression, double x, const std::string& variableName, const std::map<std::string, double>& variables)
{
	// Execute.
	TemplateContainer container;
	double result = container.ExecuteExpressionDerivative<double>(expression, x, variableName, variables);
	return result;
}

///	<summary>
///	Execute the expression find the second derivative.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="x">The variable value.</param>
/// <param name="variableName">The variable name in the expression.</param>
/// <param name="variables">The variables in the expression.</param>
/// <returns>The result of the operation.</returns>
double Expression::DerivativeSecond(const std::string& expression, double x, const std::string& variableName, const std::map<std::string, double>& variables)
{
	// Execute.
	TemplateContainer container;
	double result = container.ExecuteExpressionSecondDerivative<double>(expression, x, variableName, variables);
	return result;
}

///	<summary>
///	Execute the expression find the third derivative.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="x">The variable value.</param>
/// <param name="variableName">The variable name in the expression.</param>
/// <param name="variables">The variables in the expression.</param>
/// <returns>The result of the operation.</returns>
double Expression::DerivativeThird(const std::string& expression, double x, const std::string& variableName, const std::map<std::string, double>& variables)
{
	// Execute.
	TemplateContainer container;
	double result = container.ExecuteExpressionThirdDerivative<double>(expression, x, variableName, variables);
	return result;
}

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
double Expression::Integrate(const std::string& expression, double x, const std::string& variableName, double a, double b, const std::map<std::string, double>& variables)
{
	// Execute.
	TemplateContainer container;
	double result = container.ExecuteExpressionIntegrate<double>(expression, x, variableName, a, b, variables);
	return result;
}

///	<summary>
///	Execute the expression find the integral.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="a">The lower bound.</param>
/// <param name="b">The upper bound.</param>
/// <returns>The result of the operation.</returns>
double Expression::Integrate(const std::string& expression, double a, double b)
{
	// Allocate.
	gsl_integration_workspace* w = gsl_integration_workspace_alloc(1000);

	double result, error;

	// Assign the expression vars.
	GlobalExpression globalExpression;
	globalExpression.expression = expression;
	globalExpression.executionIndex = 1;
	globalExpression.variableName = "x";

	// Assign the function.
	gsl_function func;
	func.function = &GlobalExpressionFunctionIn;
	func.params = &globalExpression;

	// Execute.
	gsl_integration_qags(&func, a, b, 0, 1e-7, 1000, w, &result, &error);

	// Clean-up.
	gsl_integration_workspace_free(w);
	return result;
}

///	<summary>
///	Execute the expression compute the result.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="result">The result of the operation.</param>
/// <returns>True if no error; else false.</returns>
bool Expression::Compute(const std::string& expression, double* result)
{
	// Execute.
	TemplateContainer container;
	bool ret = container.ExecuteExpressionCompute<double>(expression, *result);
	return ret;
}

///	<summary>
///	Execute the expression compute the result.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="x">The variable value.</param>
/// <param name="result">The result of the operation.</param>
/// <returns>True if no error; else false.</returns>
bool Expression::Compute(const std::string& expression, double x, double* result)
{
	// Execute.
	TemplateContainer container;
	bool ret = container.ExecuteExpressionCompute<double>(expression, x, *result);
	return ret;
}

///	<summary>
///	Execute the expression compute the result.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="x">The variable value.</param>
/// <param name="y">The variable value.</param>
/// <param name="result">The result of the operation.</param>
/// <returns>True if no error; else false.</returns>
bool Expression::Compute(const std::string& expression, double x, double y, double* result)
{
	// Execute.
	TemplateContainer container;
	bool ret = container.ExecuteExpressionCompute<double>(expression, x, y, *result);
	return ret;
}

///	<summary>
///	Execute the expression compute the result.
///	</summary>
/// <param name="expression">The expression the execute.</param>
/// <param name="x">The variable value.</param>
/// <param name="y">The variable value.</param>
/// <param name="z">The variable value.</param>
/// <param name="result">The result of the operation.</param>
/// <returns>True if no error; else false.</returns>
bool Expression::Compute(const std::string& expression, double x, double y, double z, double* result)
{
	// Execute.
	TemplateContainer container;
	bool ret = container.ExecuteExpressionCompute<double>(expression, x, y, z, *result);
	return ret;
}

///	<summary>
///	Global expression function internal.
///	</summary>
double GlobalExpressionFunctionIn(double x, void* params)
{
	// Get the math expression.
	GlobalExpression globalExpression = *(GlobalExpression*)params;
	double result;

	// Execute.
	TemplateContainer container;

	// Select the execution index.
	switch (globalExpression.executionIndex)
	{
	case 2:
		result = container.ExecuteExpressionMulti<double>(globalExpression.expression, globalExpression.variables);
		break;

	case 1:
		result = container.ExecuteExpression<double>(globalExpression.expression, x, globalExpression.variableName);
		break;

	case 0:
	default:
		result = container.ExecuteOpen<double>(globalExpression.expression);
		break;
	}
	
	// Return the result.
	return result;
}