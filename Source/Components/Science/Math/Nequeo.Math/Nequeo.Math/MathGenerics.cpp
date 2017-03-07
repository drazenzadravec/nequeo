/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          MathGenerics.cpp
 *  Purpose :       
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

#include "MathGenerics.h"
#include "Expression.h"

///	<summary>
///	Construct the matrix data.
///	</summary>
generic <typename T>
Nequeo::Math::MathGenerics<T>::MathGenerics() : m_disposed(true)
{
	m_disposed = false;
}

///	<summary>
///	Deconstruct the matrix data.
///	</summary>
generic <typename T>
Nequeo::Math::MathGenerics<T>::~MathGenerics()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

///	<summary>
///	Execute a math expression.
///	</summary>
/// <param name="expression">The expression.</param>
/// <param variables="x">The multi variable names and values.</param>
/// <returns>The result of the operation.</returns>
generic <typename T>
generic <typename R>
R Nequeo::Math::MathGenerics<T>::ExpressionMulti(System::String^ expression, Dictionary<System::String^, R>^ variables)
{
	Nequeo::Math::ToolKit::Expression exp;
	typedef std::map<std::string, double> MapVars;
	MapVars multiVars;

	std::string expressionString;
	Nequeo::Math::MarshalString(expression, expressionString);

	for each(KeyValuePair<String^, R> kvp in variables)
	{
		std::string varKey;
		Nequeo::Math::MarshalString(kvp.Key, varKey);

		double xValue = *safe_cast<Double^>(Convert::ChangeType(kvp.Value, double::typeid));
		multiVars.insert({ varKey, xValue });
	}

	MapVars::size_type size = multiVars.size();
	double result = exp.Execute(expressionString, multiVars);

	R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
	return resultR;
}

///	<summary>
///	Execute a math expression.
///	</summary>
/// <param name="expression">The expression.</param>
/// <param name="x">The variable value.</param>
/// <param variableName="x">The variable x name (e.g 'x').</param>
/// <returns>The result of the operation.</returns>
generic <typename T>
generic <typename R>
R Nequeo::Math::MathGenerics<T>::Expression(System::String^ expression, R x, System::String^ variableName)
{
	Nequeo::Math::ToolKit::Expression exp;

	std::string expressionString;
	Nequeo::Math::MarshalString(expression, expressionString);

	std::string expressionVars;
	Nequeo::Math::MarshalString(variableName, expressionVars);

	double xValue = *safe_cast<Double^>(Convert::ChangeType(x, double::typeid));
	double result = exp.Execute(expressionString, xValue, expressionVars);

	R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
	return resultR;
}

///	<summary>
///	Execute a math expression.
///	</summary>
/// <param name="expression">The expression.</param>
/// <returns>The result of the operation.</returns>
generic <typename T>
generic <typename R>
R Nequeo::Math::MathGenerics<T>::Expression(System::String^ expression)
{
	Nequeo::Math::ToolKit::Expression exp;

	std::string expressionString;
	Nequeo::Math::MarshalString(expression, expressionString);

	double result = exp.Execute(expressionString);

	R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
	return resultR;
}

///	<summary>
///	Multiply two numbers.
///	</summary>
/// <param name="one">The first number.</param>
/// <param name="two">The second number.</param>
/// <returns>The result of the operation.</returns>
generic <typename T>
generic <typename R>
R Nequeo::Math::MathGenerics<T>::Multiply(R one, R two)
{
	// Get the type
	if (R::typeid == int::typeid)
	{
		int first = *safe_cast<Int32^>(Convert::ChangeType(one, int::typeid));
		int second = *safe_cast<Int32^>(Convert::ChangeType(two, int::typeid));

		int result = first * second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else if (R::typeid == UInt32::typeid)
	{
		unsigned int first = *safe_cast<UInt32^>(Convert::ChangeType(one, UInt32::typeid));
		unsigned int second = *safe_cast<UInt32^>(Convert::ChangeType(two, UInt32::typeid));

		unsigned int result = first * second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else if (R::typeid == UInt64::typeid)
	{
		unsigned long first = *safe_cast<UInt64^>(Convert::ChangeType(one, UInt64::typeid));
		unsigned long second = *safe_cast<UInt64^>(Convert::ChangeType(two, UInt64::typeid));

		unsigned long result = first * second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else if (R::typeid == long::typeid)
	{
		long first = *safe_cast<Int64^>(Convert::ChangeType(one, long::typeid));
		long second = *safe_cast<Int64^>(Convert::ChangeType(two, long::typeid));

		long result = first * second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else if (R::typeid == float::typeid)
	{
		float first = *safe_cast<Single^>(Convert::ChangeType(one, float::typeid));
		float second = *safe_cast<Single^>(Convert::ChangeType(two, float::typeid));

		float result = first * second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else if (R::typeid == Decimal::typeid)
	{
		Decimal first = *safe_cast<Decimal^>(Convert::ChangeType(one, Decimal::typeid));
		Decimal second = *safe_cast<Decimal^>(Convert::ChangeType(two, Decimal::typeid));

		Decimal result = first * second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else
	{
		double first = *safe_cast<Double^>(Convert::ChangeType(one, double::typeid));
		double second = *safe_cast<Double^>(Convert::ChangeType(two, double::typeid));

		double result = first * second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
}

///	<summary>
///	Divide two numbers.
///	</summary>
/// <param name="one">The first number.</param>
/// <param name="two">The second number.</param>
/// <returns>The result of the operation.</returns>
generic <typename T>
generic <typename R>
R Nequeo::Math::MathGenerics<T>::Divide(R one, R two)
{
	// Get the type
	if (R::typeid == int::typeid)
	{
		int first = *safe_cast<Int32^>(Convert::ChangeType(one, int::typeid));
		int second = *safe_cast<Int32^>(Convert::ChangeType(two, int::typeid));

		int result = first / second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else if (R::typeid == UInt32::typeid)
	{
		unsigned int first = *safe_cast<UInt32^>(Convert::ChangeType(one, UInt32::typeid));
		unsigned int second = *safe_cast<UInt32^>(Convert::ChangeType(two, UInt32::typeid));

		unsigned int result = first / second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else if (R::typeid == UInt64::typeid)
	{
		unsigned long first = *safe_cast<UInt64^>(Convert::ChangeType(one, UInt64::typeid));
		unsigned long second = *safe_cast<UInt64^>(Convert::ChangeType(two, UInt64::typeid));

		unsigned long result = first / second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else if (R::typeid == long::typeid)
	{
		long first = *safe_cast<Int64^>(Convert::ChangeType(one, long::typeid));
		long second = *safe_cast<Int64^>(Convert::ChangeType(two, long::typeid));

		long result = first / second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else if (R::typeid == float::typeid)
	{
		float first = *safe_cast<Single^>(Convert::ChangeType(one, float::typeid));
		float second = *safe_cast<Single^>(Convert::ChangeType(two, float::typeid));

		float result = first / second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else if (R::typeid == Decimal::typeid)
	{
		Decimal first = *safe_cast<Decimal^>(Convert::ChangeType(one, Decimal::typeid));
		Decimal second = *safe_cast<Decimal^>(Convert::ChangeType(two, Decimal::typeid));

		Decimal result = first / second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
	else
	{
		double first = *safe_cast<Double^>(Convert::ChangeType(one, double::typeid));
		double second = *safe_cast<Double^>(Convert::ChangeType(two, double::typeid));

		double result = first / second;
		R resultR = *safe_cast<R>(Convert::ChangeType(result, R::typeid));
		return resultR;
	}
}