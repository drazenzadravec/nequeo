/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          MathExtension.h
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

#include "ExpressionSB.h"

using namespace System;
using namespace System::Numerics;

using namespace Nequeo::Math::SbML;

namespace Nequeo {
	namespace Math
	{
		///	<summary>
		///	MarshalString
		///	</summary>
		/// <param name="s">The string.</param>
		/// <param name="os">The native string.</param>
		void MarshalStringEx(String^ s, std::string& os)
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
		void MarshalStringEx(String^ s, std::wstring& os)
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

namespace Nequeo {
	namespace Math 
	{
		///	<summary>
		///	Math extension.
		///	</summary>
		[System::Runtime::CompilerServices::Extension]
		public ref class MathExtension sealed
		{
			public:
				[System::Runtime::CompilerServices::Extension]
				static int Multiply(int source, int value)
				{
					return source * value;
				}

				[System::Runtime::CompilerServices::Extension]
				static double Multiply(double source, double value)
				{
					return source * value;
				}
				
				///	<summary>
				///	Get equation from MathML xml.
				///	</summary>
				/// <param name="source">The MathML xml.</param>
				/// <returns>The equation.</returns>
				[System::Runtime::CompilerServices::Extension]
				static System::String^ FromMathML(System::String^ source)
				{
					std::string expressionString;
					Nequeo::Math::MarshalStringEx(source, expressionString);

					Expression mathML;
					std::string result = mathML.FromMathML(expressionString);
					return gcnew String(result.c_str());
				}

				///	<summary>
				///	Get MathML xml from equation.
				///	</summary>
				/// <param name="source">The math equation.</param>
				/// <returns>The MathML xml.</returns>
				[System::Runtime::CompilerServices::Extension]
				static System::String^ ToMathML(System::String^ source)
				{
					std::string expressionString;
					Nequeo::Math::MarshalStringEx(source, expressionString);

					Expression mathML;
					std::string result = mathML.ToMathML(expressionString);
					return gcnew String(result.c_str());
				}
		};
	}
}