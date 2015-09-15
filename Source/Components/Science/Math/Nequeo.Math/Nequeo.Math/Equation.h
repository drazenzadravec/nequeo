/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          Equation.h
 *  Purpose :       Solve equation class.
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

#include "DataPoint.h"
#include "MatrixData.h"
#include "MathExtension.h"
#include "PolynomialJenkinsTraub.h"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Numerics;
using namespace System::Security;
using namespace System::Security::Cryptography;

namespace Nequeo 
{
	namespace Math 
	{
		///	<summary>
		///	Function 'f(x)' calculation delegate handler.
		///	</summary>
		/// <param name='x'>The x parameter within the function.</param>
		/// <returns>The result of the function for each parameter.</returns>
		public delegate double FunctionHandler(double x);

		///	<summary>
		///	Calculates the result of an equation.
		///	</summary>
		public ref class Equation sealed
		{
			public:
				// Constructors
				Equation();
				virtual ~Equation();

				// Methods
				virtual String^ Linear(double aCoefficient, double bCoefficient);
				virtual String^ Quadratic(double aCoefficient, double bCoefficient, double cCoefficient);
				virtual array<String^>^ Polynomial(array<double>^ coefficients, int degree);
				virtual array<double>^ PolynomialRegression(array<Nequeo::Math::DataPoint^>^ dataPoints, int degree);
				virtual array<double>^ SystemLinear(array<array<double>^>^ aMatrix, array<double>^ bMatrix);
				virtual double StandardNormalProbabilityDensity(double x);
				virtual double StandardNormalDistribution(double upperLimit);

				virtual double Differential(FunctionHandler^ function, double x, int differentialOrder);
				virtual double Differential(FunctionHandler^ function, double x, int differentialOrder, double delta);

				virtual double Integral(FunctionHandler^ function, double upperLimit, double lowerLimit);
				virtual double Integral(FunctionHandler^ function, double upperLimit, double lowerLimit, int intervalFactor);
				virtual double IntegralSimpsonAdaptive(FunctionHandler^ function, 
					double upperLimit, double lowerLimit, double errorTolerance,
					[System::Runtime::InteropServices::OutAttribute] double% errorEstimate,
					[System::Runtime::InteropServices::OutAttribute] int% numberFunction);
				
				virtual double IntegralGaussian2(FunctionHandler^ function, double upperLimit, double lowerLimit);
				virtual double IntegralGaussian2(FunctionHandler^ function, double upperLimit, double lowerLimit, int intervalFactor);
				virtual double IntegralGaussian2Adaptive(FunctionHandler^ function, 
					double upperLimit, double lowerLimit, double errorTolerance,
					[System::Runtime::InteropServices::OutAttribute] double% errorEstimate,
					[System::Runtime::InteropServices::OutAttribute] int% numberFunction);

				virtual double IntegralGaussian3(FunctionHandler^ function, double upperLimit, double lowerLimit);
				virtual double IntegralGaussian3(FunctionHandler^ function, double upperLimit, double lowerLimit, int intervalFactor);
				virtual double IntegralGaussian3Adaptive(FunctionHandler^ function, 
					double upperLimit, double lowerLimit, double errorTolerance,
					[System::Runtime::InteropServices::OutAttribute] double% errorEstimate,
					[System::Runtime::InteropServices::OutAttribute] int% numberFunction);

				virtual double PolynomialDifferential(array<double>^ coefficients, double x, int differentialOrder);
				virtual double PolynomialDifferential(array<double>^ coefficients, double x, int differentialOrder, double delta);
				virtual double PolynomialIntegral(array<double>^ coefficients, double upperLimit, double lowerLimit);
				virtual double PolynomialIntegral(array<double>^ coefficients, double upperLimit, double lowerLimit, int intervalFactor);
				virtual double EvaluatePolynomial(array<double>^ coefficients, double x);

			private:
				void CalculateSum(double* xPoint, double* yPoint, double* coeA);
				void CalculateMatrix(double** sumMatrix, double* coeB, double* coeA);
				double Factorial(int nthTerm);
				double MaclaurinFunction(double nthFunction, int nth);
				double Partitions(double upperLimit, double lowerLimit, int nth);
				double SumOddEvenOrdinates(FunctionHandler^ function, double upperLimit, double lowerLimit, int nth);
				double SumEndOrdinates(FunctionHandler^ function, double upperLimit, double lowerLimit);
				double PolynomialFunctionHandler(double x);
				void Gaussian2Adaptive(FunctionHandler^ function, double sab, double a, double b, double TOL, double* approx, double* errest, int* nfunc);
				void Gaussian3Adaptive(FunctionHandler^ function, double sab, double a, double b, double TOL, double* approx, double* errest, int* nfunc);
				void SimpsonAdaptive(FunctionHandler^ function, double sab, double fa, double fc, double fb, double a, double b, double TOL, double* approx, double* errest, int* nfunc);

				// Fields
				bool m_disposed;

				array<double>^ m_coefficients;
				int m_nxn_matrix;
				int m_num_data;
				int m_degree;
				
		};
	}
}