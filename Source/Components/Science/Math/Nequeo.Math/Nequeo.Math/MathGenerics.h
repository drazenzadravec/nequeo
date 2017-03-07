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
				/// <param variableName="x">The variable x name (e.g 'x').</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
				R Expression(System::String^ expression, R x, System::String^ variableName);

				///	<summary>
				///	Execute a math expression.
				///	</summary>
				/// <param name="expression">The expression.</param>
				/// <param variables="x">The multi variable names and values.</param>
				/// <returns>The result of the operation.</returns>
				generic <typename R>
			    R ExpressionMulti(System::String^ expression, Dictionary<System::String^, R>^ variables);

			private:
				// Fields
				bool m_disposed;
		};
	}
}