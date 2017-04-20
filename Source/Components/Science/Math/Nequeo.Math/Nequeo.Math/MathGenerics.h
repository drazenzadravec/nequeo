/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          MathGenerics.h
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

#pragma once

#include "stdafx.h"

using namespace System;
using namespace System::Numerics;
using namespace System::Collections::Generic;

namespace Nequeo
{
	namespace Math
	{
		///	<summary>
		///	MarshalString
		///	</summary>
		/// <param name="s">The string.</param>
		/// <param name="os">The native string.</param>
		void MarshalString(String^ s, std::string& os)
		{
			if (!String::IsNullOrEmpty(s))
			{
				using namespace Runtime::InteropServices;
				const char* chars = (const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
				os = chars;
				Marshal::FreeHGlobal(IntPtr((void*)chars));
			}
		}

		///	<summary>
		///	MarshalString
		///	</summary>
		/// <param name="s">The string.</param>
		/// <param name="os">The native string.</param>
		void MarshalString(String^ s, std::wstring& os)
		{
			if (!String::IsNullOrEmpty(s))
			{
				using namespace Runtime::InteropServices;
				const wchar_t* chars = (const wchar_t*)(Marshal::StringToHGlobalUni(s)).ToPointer();
				os = chars;
				Marshal::FreeHGlobal(IntPtr((void*)chars));
			}
		}
	}
}

namespace Nequeo 
{
	namespace Math 
	{
		///	<summary>
		///	Math generics.
		///	</summary>
		generic <typename T>
		public ref class MathGenerics
		{
			public:
				// Constructors
				MathGenerics();
				virtual ~MathGenerics();
				
				///	<summary>
				///	Multiply two numbers.
				///	</summary>
				/// <param name="one">The first number.</param>
				/// <param name="two">The second number.</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
				R Multiply(R one, R two);

				///	<summary>
				///	Divide two numbers.
				///	</summary>
				/// <param name="one">The first number.</param>
				/// <param name="two">The second number.</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
				R Divide(R one, R two);

				///	<summary>
				///	Execute a math expression.
				///	</summary>
				/// <param name="expression">The expression.</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
				R Expression(System::String^ expression);

				///	<summary>
				///	Execute a math expression.
				///	</summary>
				/// <param name="expression">The expression.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="variableName">The variable x name (e.g 'x').</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
				R Expression(System::String^ expression, R x, System::String^ variableName);

				///	<summary>
				///	Execute a math expression.
				///	</summary>
				/// <param name="expression">The expression.</param>
				/// <param name="variables">The multi variable names and values.</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
			    R ExpressionMulti(System::String^ expression, Dictionary<System::String^, R>^ variables);

				///	<summary>
				///	Execute the expression find the derivative.
				///	</summary>
				/// <param name="expression">The expression.</param>
				/// <param name="x">The variable value.</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
				R ExpressionDerivative(System::String^ expression, R x);

				///	<summary>
				///	Execute the expression find the derivative.
				///	</summary>
				/// <param name="expression">The expression.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="variableName">The variable x name (e.g 'x').</param>
				/// <param name="variables">The multi variable names and values.</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
				R ExpressionDerivative(System::String^ expression, R x, System::String^ variableName, Dictionary<System::String^, R>^ variables);

				///	<summary>
				///	Execute the expression find the second derivative.
				///	</summary>
				/// <param name="expression">The expression.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="variableName">The variable x name (e.g 'x').</param>
				/// <param name="variables">The multi variable names and values.</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
				R ExpressionDerivativeSecond(System::String^ expression, R x, System::String^ variableName, Dictionary<System::String^, R>^ variables);

				///	<summary>
				///	Execute the expression find the third derivative.
				///	</summary>
				/// <param name="expression">The expression.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="variableName">The variable x name (e.g 'x').</param>
				/// <param name="variables">The multi variable names and values.</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
				R ExpressionDerivativeThird(System::String^ expression, R x, System::String^ variableName, Dictionary<System::String^, R>^ variables);


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
				generic <typename R>
				R ExpressionIntegrate(System::String^ expression, R x, System::String^ variableName, R a, R b, Dictionary<System::String^, R>^ variables);

				///	<summary>
				///	Execute the expression find the integral.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="a">The lower bound.</param>
				/// <param name="b">The upper bound.</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
				R ExpressionIntegrate(System::String^ expression, R a, R b);

				///	<summary>
				///	Execute the expression compute the result.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="result">The result of the operation.</param>
				/// <returns>True if no error; else false.</returns>
				generic <typename R>
				bool ExpressionCompute(System::String^ expression,
					[System::Runtime::InteropServices::OutAttribute] R% result);

				///	<summary>
				///	Execute the expression compute the result.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="result">The result of the operation.</param>
				/// <returns>True if no error; else false.</returns>
				generic <typename R>
				bool ExpressionCompute(System::String^ expression, R x,
					[System::Runtime::InteropServices::OutAttribute] R% result);

				///	<summary>
				///	Execute the expression compute the result.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="y">The variable value.</param>
				/// <param name="result">The result of the operation.</param>
				/// <returns>True if no error; else false.</returns>
				generic <typename R>
				bool ExpressionCompute(System::String^ expression, R x, R y,
					[System::Runtime::InteropServices::OutAttribute] R% result);

				///	<summary>
				///	Execute the expression compute the result.
				///	</summary>
				/// <param name="expression">The expression the execute.</param>
				/// <param name="x">The variable value.</param>
				/// <param name="y">The variable value.</param>
				/// <param name="z">The variable value.</param>
				/// <param name="result">The result of the operation.</param>
				/// <returns>True if no error; else false.</returns>
				generic <typename R>
				bool ExpressionCompute(System::String^ expression, R x, R y, R z,
					[System::Runtime::InteropServices::OutAttribute] R% result);

			private:
				// Fields
				bool m_disposed;
		};
	}
}