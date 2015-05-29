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

#include "stdafx.h"

#include "Equation.h"

///	<summary>
///	Construct the equation.
///	</summary>
Nequeo::Math::Equation::Equation() : m_disposed(true), m_nxn_matrix(0), m_num_data(0), m_degree(0)
{
	m_disposed = false;
}

///	<summary>
///	Deconstruct the equation.
///	</summary>
Nequeo::Math::Equation::~Equation()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

///	<summary>
///	Calculate the differential of 'f(x)'. The technique used to calculate the differential is the Taylor Series.
///	</summary>
/// <param name='function'>The handler to the function to differentiate at x.</param>
/// <param name='differentialOrder'>The differential order to calculate (1 to 5 inclusive).</param>
/// <returns>The value of the differential at x.</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::Differential(Nequeo::Math::FunctionHandler^ function, double x, int differentialOrder)
{
	return Differential(function, x, differentialOrder, (1.0 / 1000));
}

///	<summary>
///	Calculate the differential of 'f(x)'. The technique used to calculate the differential is the Taylor Series.
///	</summary>
/// <param name='function'>The handler to the function to differentiate at x.</param>
/// <param name='differentialOrder'>The differential order to calculate (1 to 5 inclusive).</param>
/// <param name='delta'>The delta (change as h -> 0, where delta = h) to x.</param>
/// <returns>The value of the differential at x.</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::Differential(Nequeo::Math::FunctionHandler^ function, double x, int differentialOrder, double delta)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw gcnew ArgumentNullException("function", "A valid function handler has not been supplied.");
	}

	// Make sure that the differential order is within range.
	if(differentialOrder < 1 || differentialOrder > 5)
	{
		throw gcnew Exception("The differential order must be between 1 and 5 (inclusive)");
	}

	const char* errMessage = nullptr;
	double differential;

	try
	{
		// Select the differential order to calculate.
		switch (differentialOrder)
		{
			case 1:
				differential = 
					(
						(-3 * function(x + (4 * delta))) +
						(16 * function(x + (3 * delta))) +
						(-36 * function(x + (2 * delta))) +
						(48 * function(x + (1 * delta))) +
						(-25 * function(x + (0 * delta)))
					) / (12 * delta);
				break;

			case 2:
				differential = 
					(
						(11 * function(x + (4 * delta))) +
						(-56 * function(x + (3 * delta))) +
						(114 * function(x + (2 * delta))) +
						(-104 * function(x + (1 * delta))) +
						(35 * function(x + (0 * delta)))
					) / (12 * delta * delta);
				break;

			case 3:
				differential = 
					(
						(-3 * function(x + (4 * delta))) +
						(14 * function(x + (3 * delta))) +
						(-24 * function(x + (2 * delta))) +
						(18 * function(x + (1 * delta))) +
						(-5 * function(x + (0 * delta)))
					) / (2 * delta * delta * delta);
				break;

			case 4:
				differential = 
					(
						(1 * function(x + (4 * delta))) +
						(-4 * function(x + (3 * delta))) +
						(6 * function(x + (2 * delta))) +
						(-4 * function(x + (1 * delta))) +
						(1 * function(x + (0 * delta)))
					) / (delta * delta * delta * delta);
				break;

			case 5:
				differential = 
					(
						(1 * function(x + (5 * delta))) +
						(-5 * function(x + (4 * delta))) +
						(10 * function(x + (3 * delta))) +
						(-10 * function(x + (2 * delta))) +
						(5 * function(x + (1 * delta))) +
						(-1 * function(x + (0 * delta)))
					) / (delta * delta * delta * delta * delta);
				break;

			default:
				differential = 0;
				break;
		}

		// Return the differential.
		return differential;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the differential value.", innerException);
	}
	finally 
	{
		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the Simpson's Rule.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::Integral(Nequeo::Math::FunctionHandler^ function, double upperLimit, double lowerLimit)
{
	return Integral(function, upperLimit, lowerLimit, 2000);
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the Simpson's Rule.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='intervalFactor'>Set this value to calculate more accurate integrals (minimum is set at 2000).</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::Integral(Nequeo::Math::FunctionHandler^ function, double upperLimit, double lowerLimit, int intervalFactor)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw gcnew ArgumentNullException("function", "A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw gcnew Exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	const char* errMessage = nullptr;
	double integral;
	int n;

	try
	{
		n = 0;

		// Calculate the number of partitions (nth term).
		// The more partitions the more accurate the result.
		for(int i = lowerLimit; i <= upperLimit; i++)
		{
			n += intervalFactor;
		}

		// Calculate the integral for the function.
		integral = ((Partitions(upperLimit, lowerLimit, n) /3 ) * (SumEndOrdinates(function, upperLimit, lowerLimit) + SumOddEvenOrdinates(function, upperLimit, lowerLimit, n)));

		// Return the integral.
		return integral;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the integral value.", innerException);
	}
	finally 
	{
		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the Simpson's Rule.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='errorTolerance'>The absolute error tolerance.</param>
/// <param name='errorEstimate'>The estimate for absolute error in approximate value of integral.</param>
/// <param name='numberFunction'>The number of function evaluations used to compute approximate value of integral.</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::IntegralSimpsonAdaptive(
	Nequeo::Math::FunctionHandler^ function, double upperLimit, double lowerLimit, double errorTolerance, 
	[System::Runtime::InteropServices::OutAttribute] double% errorEstimate, 
	[System::Runtime::InteropServices::OutAttribute] int% numberFunction)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw gcnew ArgumentNullException("function", "A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw gcnew Exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	const char* errMessage = nullptr;
	double approx;
	double errest;
	int nfunc;
	double integral;
	double fa, fc, fb, sab;

	try
	{
		// Calculate the integral for the function.
		fa = function(lowerLimit);
		fc = function((lowerLimit + upperLimit) / 2.0);
		fb = function(upperLimit);
		sab = (upperLimit - lowerLimit) * (fa + (4.0 * fc) + fb) / 6.0;
		SimpsonAdaptive(function, sab, fa, fc, fb, lowerLimit, upperLimit, errorTolerance, &approx, &errest, &nfunc);
		nfunc += 3;

		// Return the integral.
		errorEstimate = errest;
		numberFunction = nfunc;
		integral = approx;
		return integral;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the integral value.", innerException);
	}
	finally 
	{
		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the 2-point Gaussian Quadrature.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::IntegralGaussian2(Nequeo::Math::FunctionHandler^ function, double upperLimit, double lowerLimit)
{
	return IntegralGaussian2(function, upperLimit, lowerLimit, 2000);
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the 2-point Gaussian Quadrature.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='intervalFactor'>Set this value to calculate more accurate integrals (minimum is set at 2000).</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::IntegralGaussian2(Nequeo::Math::FunctionHandler^ function, double upperLimit, double lowerLimit, int intervalFactor)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw gcnew ArgumentNullException("function", "A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw gcnew Exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	const char* errMessage = nullptr;
	double integral;
	double h;
    double sqthird;
    double temp;
    double sum;
	int n;

	try
	{
		n = intervalFactor;

		// Get the height of each interval
		h = (upperLimit - lowerLimit) / (2.0 * (double)n);
		sqthird = sqrt(3.0) / 3.0;
     
		// Sum the interval values.
		sum = 0.0;
		for (int i = 1; i <= n; i++) 
		{
			temp = lowerLimit + (((2 * i) - 1) * h);
			sum += (function(temp - (h * sqthird)) + function(temp + (h * sqthird)));
		}

		// Calculate the integral.
		integral = h * sum;

		// Return the integral.
		return integral;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the integral value.", innerException);
	}
	finally 
	{
		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the 2-point Gaussian Quadrature.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='errorTolerance'>The absolute error tolerance.</param>
/// <param name='errorEstimate'>The estimate for absolute error in approximate value of integral.</param>
/// <param name='numberFunction'>The number of function evaluations used to compute approximate value of integral.</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::IntegralGaussian2Adaptive(Nequeo::Math::FunctionHandler^ function, 
	double upperLimit, double lowerLimit, double errorTolerance,
	[System::Runtime::InteropServices::OutAttribute] double% errorEstimate,
	[System::Runtime::InteropServices::OutAttribute] int% numberFunction)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw gcnew ArgumentNullException("function", "A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw gcnew Exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	const char* errMessage = nullptr;
	double approx;
	double errest;
	int nfunc;
	double integral;
	double h2, fm, fp, sab;

	try
	{
		// Calculate the integral for the function.
		h2 = (upperLimit - lowerLimit) / 2.0;
		fm = function(lowerLimit + h2 - (sqrt(1.0/3.0) * h2));
		fp = function(lowerLimit + h2 + (sqrt(1.0/3.0) * h2));
		sab = (h2 * (fm + fp));
		Gaussian2Adaptive(function, sab, lowerLimit, upperLimit, errorTolerance, &approx, &errest, &nfunc);
		nfunc += 2;

		// Return the integral.
		errorEstimate = errest;
		numberFunction = nfunc;
		integral = approx;
		return integral;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the integral value.", innerException);
	}
	finally 
	{
		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the 3-point Gaussian Quadrature.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::IntegralGaussian3(Nequeo::Math::FunctionHandler^ function, double upperLimit, double lowerLimit)
{
	return IntegralGaussian3(function, upperLimit, lowerLimit, 2000);
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the 3-point Gaussian Quadrature.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='intervalFactor'>Set this value to calculate more accurate integrals (minimum is set at 2000).</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::IntegralGaussian3(Nequeo::Math::FunctionHandler^ function, double upperLimit, double lowerLimit, int intervalFactor)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw gcnew ArgumentNullException("function", "A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw gcnew Exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	const char* errMessage = nullptr;
	double integral;
	double h;
    double sq35;
    double temp;
	double c1;
	double c2;
    double sum;
	int n;

	try
	{
		n = intervalFactor;

		// Get the height of each interval
		h = (upperLimit - lowerLimit) / (2.0 * (double)n);
		sq35 = sqrt(0.6);
		c1 = 5.0 / 9.0;
		c2 = 8.0 / 9.0;
     
		// Sum the interval values.
		sum = 0.0;
		for (int i = 1; i <= n; i++) 
		{
			temp = lowerLimit + (((2 * i) - 1) * h);
			sum += c1 * (function(temp - (h * sq35)) + function(temp + (h * sq35)));
			sum += c2 * function(temp);
		}

		// Calculate the integral.
		integral = h * sum;

		// Return the integral.
		return integral;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the integral value.", innerException);
	}
	finally 
	{
		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the 3-point Gaussian Quadrature.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='errorTolerance'>The absolute error tolerance.</param>
/// <param name='errorEstimate'>The estimate for absolute error in approximate value of integral.</param>
/// <param name='numberFunction'>The number of function evaluations used to compute approximate value of integral.</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::IntegralGaussian3Adaptive(Nequeo::Math::FunctionHandler^ function, 
	double upperLimit, double lowerLimit, double errorTolerance,
	[System::Runtime::InteropServices::OutAttribute] double% errorEstimate,
	[System::Runtime::InteropServices::OutAttribute] int% numberFunction)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw gcnew ArgumentNullException("function", "A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw gcnew Exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	const char* errMessage = nullptr;
	double approx;
	double errest;
	int nfunc;
	double integral;
	double h2, fm, fc, fp, sab;

	try
	{
		// Calculate the integral for the function.
		h2 = (upperLimit - lowerLimit) / 2.0;
		fm = function(lowerLimit + h2 - (sqrt(0.6) * h2));
		fc = function(lowerLimit + h2 );
		fp = function(lowerLimit + h2 + (sqrt(0.6) * h2));
		sab = h2 * ((5.0  *fm) + (8.0 * fc) + (5.0 * fp)) / 9.0;
		Gaussian3Adaptive(function, sab, lowerLimit, upperLimit, errorTolerance, &approx, &errest, &nfunc);
		nfunc += 3;

		// Return the integral.
		errorEstimate = errest;
		numberFunction = nfunc;
		integral = approx;
		return integral;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the integral value.", innerException);
	}
	finally 
	{
		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the polynomial differential (real coefficients) equation 'f(x) = a[0] + a[1]x + a[2]x^2 + a[3]x^3 + .......+ a[n]x^n'. 
/// The technique used to calculate the differential is the Taylor Series.
///	</summary>
/// <param name='coefficients'>The set of (a) coefficients. lowest order (a[0]) to highest order (a[n])</param>
/// <param name='differentialOrder'>The differential order to calculate (1 to 5 inclusive).</param>
/// <returns>The value of the differential at x.</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::PolynomialDifferential(array<double>^ coefficients, double x, int differentialOrder)
{
	return PolynomialDifferential(coefficients, x, differentialOrder, (1.0 / 1000));
}

///	<summary>
///	Calculate the polynomial differential (real coefficients) equation 'f(x) = a[0] + a[1]x + a[2]x^2 + a[3]x^3 + .......+ a[n]x^n'. 
/// The technique used to calculate the differential is the Taylor Series.
///	</summary>
/// <param name='coefficients'>The set of (a) coefficients. lowest order (a[0]) to highest order (a[n])</param>
/// <param name='differentialOrder'>The differential order to calculate (1 to 5 inclusive).</param>
/// <param name='delta'>The delta (change as h -> 0, where delta = h) to x.</param>
/// <returns>The value of the differential at x.</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::PolynomialDifferential(array<double>^ coefficients, double x, int differentialOrder, double delta)
{
	// Make sure that a valid set of data points exits.
	if((coefficients == nullptr) || (coefficients->Length < 1))
	{
		throw gcnew ArgumentNullException("coefficients", "A valid set of coefficients has not been supplied.");
	}

	// Make sure that the differential order is within range.
	if(differentialOrder < 1 || differentialOrder > 5)
	{
		throw gcnew Exception("The differential order must be between 1 and 5 (inclusive)");
	}

	const char* errMessage = nullptr;
	FunctionHandler^ handler;

	try
	{
		m_coefficients = coefficients;

		// Create the internal function handler.
		handler = gcnew FunctionHandler(this, &Nequeo::Math::Equation::PolynomialFunctionHandler);

		// Return the differential.
		return Differential(handler, x, differentialOrder, delta);
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the differential value.", innerException);
	}
	finally 
	{
		if(handler != nullptr)
			delete handler;

		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the definate polynomial integral (real coefficients) equation 'f(x) = a[0] + a[1]x + a[2]x^2 + a[3]x^3 + .......+ a[n]x^n' integral over x = (a, b). 
/// The technique used to calculate the integral is the Simpson's Rule.
///	</summary>
/// <param name='coefficients'>The set of (a) coefficients. lowest order (a[0]) to highest order (a[n])</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::PolynomialIntegral(array<double>^ coefficients, double upperLimit, double lowerLimit)
{
	return PolynomialIntegral(coefficients, upperLimit, lowerLimit, 2000);
}

///	<summary>
///	Calculate the definate polynomial integral (real coefficients) equation 'f(x) = a[0] + a[1]x + a[2]x^2 + a[3]x^3 + .......+ a[n]x^n' integral over x = (a, b). 
/// The technique used to calculate the integral is the Simpson's Rule.
///	</summary>
/// <param name='coefficients'>The set of (a) coefficients. lowest order (a[0]) to highest order (a[n])</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='intervalFactor'>Set this value to calculate more accurate integrals (minimum is set at 2000).</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::PolynomialIntegral(array<double>^ coefficients, double upperLimit, double lowerLimit, int intervalFactor)
{
	// Make sure that a valid set of data points exits.
	if((coefficients == nullptr) || (coefficients->Length < 1))
	{
		throw gcnew ArgumentNullException("coefficients", "A valid set of coefficients has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw gcnew Exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	const char* errMessage = nullptr;
	FunctionHandler^ handler;

	try
	{
		m_coefficients = coefficients;

		// Create the internal function handler.
		handler = gcnew FunctionHandler(this, &Nequeo::Math::Equation::PolynomialFunctionHandler);

		// Return the integral.
		return Integral(handler, upperLimit, lowerLimit, intervalFactor);
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the integral value.", innerException);
	}
	finally 
	{
		if(handler != nullptr)
			delete handler;

		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Evaluate the polynomial (real coefficients) equation 'f(x) = a[0] + a[1]x + a[2]x^2 + a[3]x^3 + .......+ a[n]x^n' for x. 
///	</summary>
/// <param name='coefficients'>The set of (a) coefficients. lowest order (a[0]) to highest order (a[n])</param>
/// <param name='x'>The x parameter within the function to evaluate.</param>
/// <returns>The evaluated polynomial.</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Equation::EvaluatePolynomial(array<double>^ coefficients, double x)
{
	// Make sure that a valid set of data points exits.
	if((coefficients == nullptr) || (coefficients->Length < 1))
	{
		throw gcnew ArgumentNullException("coefficients", "A valid set of coefficients has not been supplied.");
	}

	const char* errMessage = nullptr;
	double result;

	try
	{
		m_coefficients = coefficients;

		// Evaluated the polynomial.
		result = PolynomialFunctionHandler(x);

		// The evaluated polynomial.
		return result;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the integral value.", innerException);
	}
	finally 
	{
		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculates the root of a linear equation (ax + b = 0).
///	</summary>
/// <param name='aCoefficient'>The a coefficient.</param>
/// <param name='bCoefficient'>The b coefficient.</param>
/// <returns>The root of the equation.</returns>
String^ Nequeo::Math::Equation::Linear(double aCoefficient, double bCoefficient)
{
	double x1;

	if(aCoefficient != (double)0.0)
	{
		// Calculate the root.
		x1 = ((-1 * bCoefficient) / aCoefficient);

		// Return the root.
		return gcnew String("x = " + x1.ToString());
	}
	else
	{
		// Return the root.
		return gcnew String("x = Undefined");
	}
}

///	<summary>
///	Calculates the roots of a quadratic equation (ax^2 + bx + c = 0).
///	</summary>
/// <param name='aCoefficient'>The a coefficient.</param>
/// <param name='bCoefficient'>The b coefficient.</param>
/// <param name='cCoefficient'>The c coefficient.</param>
/// <returns>The roots of the equation.</returns>
String^ Nequeo::Math::Equation::Quadratic(double aCoefficient, double bCoefficient, double cCoefficient)
{
	double discr;
	double discr1;
	double c1;
	double x1;
	double x2;

	// Calculates the discriminant of the quadratic*/
	discr = (bCoefficient  *bCoefficient) - (4  * aCoefficient * cCoefficient);

	// Calculates the roots of the equation.
	if (discr > 0)
	{
		// Calculate the roots.
		x1 = ((-1 * bCoefficient) + sqrt((bCoefficient * bCoefficient) - (4 * aCoefficient * cCoefficient))) / (2 * aCoefficient);
		x2 = ((-1 * bCoefficient) - sqrt((bCoefficient * bCoefficient) - (4 * aCoefficient * cCoefficient))) / (2 * aCoefficient);
		
		// Return the roots.
		return gcnew String("x = " + x1.ToString() + " ; x = " + x2.ToString());
	}
	else if (discr < 0)
	{
		// Calculate the complex roots.
		discr1 = sqrt(-1 * ((bCoefficient * bCoefficient) - (4 * aCoefficient * cCoefficient))) / (2 * aCoefficient);
		c1 = (-1 * bCoefficient) / (2 * aCoefficient);
		
		// Return the complex roots.
		return gcnew String("x = " + c1.ToString() + " + " + discr1.ToString() + "i ; x = " + c1.ToString() + " - " + discr1.ToString() + "i");
	}
	else
	{
		// Calculate the single root.
		x1 = ((-1 * bCoefficient) + sqrt((bCoefficient * bCoefficient) - (4 * aCoefficient * cCoefficient))) / (2 * aCoefficient);
		
		// Return the root.
		return gcnew String("x = " + x1.ToString());
	}
}

///	<summary>
///	Calculates the roots of a polynomial (real coefficients) equation '0 = a[0] + a[1]x + a[2]x^2 + a[3]x^3 + .......+ a[n]x^n'.
/// Calculations are based on the Jenkins-Traub algorithm. The number of coefficients must equal the degree plus one (degree + 1).
///	</summary>
/// <param name='coefficients'>The set of (a) coefficients. lowest order (a[0]) to highest order (a[n])</param>
/// <param name='degree'>The degree (n) of the polynomial.</param>
/// <returns>The roots of the equation.</returns>
/// <exception cref="System::Exception">Thrown when any internal exception occures.</exception>
array<String^>^ Nequeo::Math::Equation::Polynomial(array<double>^ coefficients, int degree)
{
	// Make sure that a valid set of data points exits.
	if((coefficients == nullptr) || (coefficients->Length < 1))
	{
		throw gcnew ArgumentNullException("coefficients", "A valid set of coefficients has not been supplied.");
	}

	// Make sure that a valid degree is passed.
	if(degree < 0)
	{
		throw gcnew ArgumentOutOfRangeException("degree", "The degree must be between zero and " + MAXDEGREE.ToString());
	}

	if((coefficients->Length) != (degree + 1))
	{
		throw gcnew ArgumentNullException("coefficients", "The number of coefficients must equal the degree plus one (degree + 1).");
	}

	const char* errMessage = nullptr;
	Nequeo::Math::PolynomialJenkinsTraub* pjt = nullptr;

	try
	{
		// The degree (n) of the polynomial.
		int deg;

		// Coefficient vectors.
		double op[MDP1], zeror[MAXDEGREE], zeroi[MAXDEGREE];

		pjt = new Nequeo::Math::PolynomialJenkinsTraub();

		// Input the polynomial coefficients from the file and put them in the op vector.
		for (int i = 0; i < (degree + 1); i++)
		{ 
			op[i] = coefficients[degree - i];
		}

		// Assign the degree of the polynomial.
		deg = degree;
	
		// Set no error and calculate the roots of the polynomial.
		pjt->SetError("");
		pjt->Calculate(op, &deg, zeror, zeroi);
		
		// Create the root collection.
		array<String^>^ roots = gcnew array<String^>(deg);

		// Get the current error, throw the exception
		// if an error has occurred.
		string retError = pjt->GetError();
		if(!retError.empty())
		{
			throw new exception(retError.c_str());
		}

		// Degrees if zero or less, therefore calculation failed.
		if(deg <= 0)
		{
			throw new exception("Degree had a value <= 0. Thus caculation has failed.");
		}
		else
		{
			// For each root found.
			for (int i = 0; i < deg; i++)
			{ 
				//out << zeror[i] << " + " << zeroi[i] << "i" << " \n";
				String^ root = gcnew String("0");

				// If both real and imaginary are zero.
				if((zeror[i] == (double)0.0) && (zeroi[i] == (double)0.0))
				{
					// Root is zero.
					root = gcnew String("0");
				}
				// If only real is zero.
				else if((zeror[i] == (double)0.0))
				{
					// Root is imaginary.
					root = gcnew String(zeroi[i].ToString() + "i");
				}
				// If only imaginary is zero.
				else if((zeroi[i] == (double)0.0))
				{
					// Root is real.
					root = gcnew String(zeror[i].ToString());
				}
				// If both real and imaginary are not zero.
				else
				{
					// If only imaginary is negative.
					if((zeroi[i] < (double)0.0))
					{
						// Root is real and imaginary.
						root = gcnew String(zeror[i].ToString() + " " + zeroi[i].ToString() + "i");
					}
					else
					{
						// Root is real and imaginary.
						root = gcnew String(zeror[i].ToString() + " + " + zeroi[i].ToString() + "i");
					}
				}

				// Assign the root.
				roots[i] = root;
			}
		}
	
		// Return the roots.
		return roots;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the polynomial roots.", innerException);
	}
	finally 
	{
		// Delete the Polynomial Jenkins Traub object.
		if(pjt != nullptr)
			delete pjt;

		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the polynomial regression on a set of data points (x, y) for the 
/// equation 'Y = a[0] + a[1]x + a[2]x^2 + a[3]x^3 + .......+ a[n]x^n'
///	</summary>
/// <param name='dataPoints'>The collection of dataPoints.</param>
/// <param name='degree'>The degree of the polynomial.</param>
/// <returns>The equation coefficient collection.</returns>
/// <exception cref="System::Exception">Thrown when the degree is not within the range.</exception>
array<double>^ Nequeo::Math::Equation::PolynomialRegression(array<Nequeo::Math::DataPoint^>^ dataPoints, int degree)
{
	// Make sure that a valid set of data points exits.
	if((dataPoints == nullptr) || (dataPoints->Length < 1))
	{
		throw gcnew ArgumentNullException("dataPoints", "A valid set of data points has not been supplied.");
	}

	// Make sure that a valid degree is passed.
	if((degree < 0) || (degree >= dataPoints->Length))
	{
		throw gcnew ArgumentOutOfRangeException("degree", "The degree must be between zero and one less than the data point array size.");
	}

	const char* errMessage = nullptr;
	double* xDataPoint;
	double* yDataPoint;
	double* coeA;

	try
	{
		// Get the number of data points
		m_num_data = dataPoints->Length;
		m_degree = degree;

		// the matrix will be one more than the degree of the polynomial
		m_nxn_matrix = m_degree + 1;

		// Create the data point arrays.
		xDataPoint = new double[m_num_data];
		yDataPoint = new double[m_num_data];
		coeA = new double[m_nxn_matrix];

		// Get each data point and assign to the allocated (x, y) points.
		for(int i = 0; i < m_num_data; i++)
		{
			// Assign each data point in the plan.
			xDataPoint[i] = dataPoints[i]->X;
			yDataPoint[i] = dataPoints[i]->Y;
		}

		// Calculate the sum of the polynomial regression matrix.
		CalculateSum(xDataPoint, yDataPoint, coeA);

		// Create a new A coefficient array to return.
		array<double>^ coeACol = gcnew array<double>(m_nxn_matrix);

		// For each A allocated coefficient.
		for(int i = 0; i < m_nxn_matrix; i++)
		{
			// Assign the A coefficient.
			coeACol[i] = coeA[i];
		}

		// Return the array of A coefficients.
		return coeACol;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate equation coefficient collection.", innerException);
	}
	finally 
	{
		// Delete the x data points.
		if(xDataPoint != nullptr)
			delete[] xDataPoint;

		// Delete the y data points.
		if(yDataPoint != nullptr)
			delete[] yDataPoint;

		// Delete the coefficient A values.
		if(coeA != nullptr)
			delete[] coeA;

		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the System of Linear Equations, NxN matrix (AX = B, solving for X) using Guassian reduction (Gaussian elimination).
///	</summary>
/// <param name='aMatrix'>The A matrix of values.</param>
/// <param name='bMatrix'>The B matrix of values.</param>
/// <returns>The equation X value collection.</returns>
/// <exception cref="System::Exception">Thrown when the matrix range is invalid.</exception>
array<double>^ Nequeo::Math::Equation::SystemLinear(array<array<double>^>^ aMatrix, array<double>^ bMatrix)
{
	// Make sure that a valid set of data exits.
	if((bMatrix == nullptr) || (bMatrix->Length < 1))
	{
		throw gcnew ArgumentNullException("bMatrix", "Data has not been supplied.");
	}

	// Make sure that a valid set of data exits.
	if((aMatrix == nullptr) || (aMatrix->Length < 1))
	{
		throw gcnew ArgumentNullException("aMatrix", "Data has not been supplied.");
	}

	const char* errMessage = nullptr;
	double* bMatrixData;
	double* xMatrixData;
	double** aMatrixData;
	
	try
	{
		m_nxn_matrix = bMatrix->Length;

		bMatrixData = new double[m_nxn_matrix];
		xMatrixData = new double[m_nxn_matrix];

		aMatrixData = new double*[m_nxn_matrix];
		for (int i = 0; i < m_nxn_matrix; i++)
		{
			// Create the first-dimension.
			aMatrixData[i] = new double[m_nxn_matrix];
		}

		// For each B allocated coefficient.
		for(int i = 0; i < m_nxn_matrix; i++)
		{
			// Assign the B coefficient.
			bMatrixData[i] = bMatrix[i];
		}

		// For each A allocated coefficient.
		for(int i = 0; i < m_nxn_matrix; i++)
		{
			for(int j = 0; j < m_nxn_matrix; j++)
			{
				aMatrixData[i][j] = aMatrix[i][j];
			}
		}
	
		// Calculate the matrix using Guassian reduction.
		CalculateMatrix(aMatrixData, bMatrixData, xMatrixData);

		// Create a new A coefficient array to return.
		array<double>^ coeXCol = gcnew array<double>(m_nxn_matrix);

		// For each X allocated coefficient.
		for(int i = 0; i < m_nxn_matrix; i++)
		{
			// Assign the X coefficient.
			coeXCol[i] = xMatrixData[i];
		}

		// Return the array of X coefficients.
		return coeXCol;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to calculate the X matrix values.", innerException);
	}
	finally 
	{
		// Delete the coefficient B values.
		if(bMatrixData != nullptr)
			delete[] bMatrixData;

		// Delete the coefficient X values.
		if(xMatrixData != nullptr)
			delete[] xMatrixData;

		// If the objects has been created.
		if(aMatrixData != nullptr)
		{
			// Delete the memory allocation
			// for items in the array.
			for (int i = 0; i < m_nxn_matrix; i++)
			{
				if(aMatrixData[i] != nullptr)
					delete[] aMatrixData[i];
			}

			// Delete the memory for the top-level array.
			delete[] aMatrixData;
		}

		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the standard normal probability density function (PDF) of a continuous distribution 
/// is defined as the derivative of the (cumulative) distribution function.
///	</summary>
/// <param name='x'>The x value within the function.</param>
/// <returns>The probability density value.</returns>
double Nequeo::Math::Equation::StandardNormalProbabilityDensity(double x)
{
	return ((1 / pow((2 * PI), (1 / 2))) * (exp(-pow(x, 2) / 2)));
}

///	<summary>
///	Calculate the standard normal distribution function, also called the cumulative distribution function (CDF) 
/// or cumulative frequency function, describes the probability that a variate (X) takes on a value less than 
/// or equal to a number (x). domain x = (-Infinity, Infinity). The technique used to calculate the integral is Maclaurin Series.
///	</summary>
/// <param name='upperLimit'>The upper limit (b) value. This value is replaced with domain x = (-Infinity, b).</param>
/// <returns>The distribution value.</returns>
double Nequeo::Math::Equation::StandardNormalDistribution(double upperLimit)
{
	const char* errMessage = nullptr;

	double integralFinal;
	double integral;
	double function;
	int nthTerm;

	try
	{
		nthTerm = 170;
		function = 1.0;
		integralFinal = 0.0;

		// For each term to the maximun (n-th) term.
		for(int n = 0; n <= nthTerm; n = n + 2)
		{
			// Calculate the intergal iteration.
			function = MaclaurinFunction(function, n);
			integral = ((log10(fabs(function)) + log10(pow(upperLimit, (n + 1)))) - (log10(n + 1) + log10(Factorial(n))));
			integralFinal += (function / fabs(function)) * pow(10, integral);
		}

		// Calculate the integral value.
		integral = (1 / sqrt(2 * PI)) * integralFinal + 0.5;

		// Return the integral.
		return integral;
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to standard normal distribution function value.", innerException);
	}
	finally 
	{
		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the sum of the polynomial regression matrix.
///	</summary>
/// <param name='xPoint'>The collection of X data points.</param>
/// <param name='yPoint'>The collection of Y data points.</param>
/// <param name='coeA'>The coefficient collection reference (will contain the result when complete).</param>
void Nequeo::Math::Equation::CalculateSum(double* xPoint, double* yPoint, double* coeA)
{
	const char* errMessage = nullptr;
	double* coeB;
	double** sumMatrix;

	try
	{
		coeB = new double[m_nxn_matrix];

		sumMatrix = new double*[m_nxn_matrix];
		for (int i = 0; i < m_nxn_matrix; i++)
		{
			// Create the first-dimension.
			sumMatrix[i] = new double[m_nxn_matrix];
		}

		// Set all array of B to zero.
		for(int i = 0; i < m_nxn_matrix; i++)
		{
			coeB[i] = 0;
		}

		// calculates the sum of the result determinate.
		for(int i = 0; i < m_nxn_matrix; i++)
		{
			for(int k = 0; k < m_num_data; k++)
			{
				coeB[i] += (pow(xPoint[k], i) * yPoint[k]);
			}
		}

		// Set all array of S to zero.
		for(int i = 0; i < m_nxn_matrix; i++)
		{
			for(int j = 0; j< m_nxn_matrix; j++)
			{
				sumMatrix[i][j] = 0;
			}
		}

		// Calculates the sum of the coefficent determinates.
		for(int i = 0; i < m_nxn_matrix; i++)
		{
			for(int j = 0; j < m_nxn_matrix; j++)
			{
				for(int k = 0; k < m_num_data; k++)
				{
					sumMatrix[i][j] += pow(xPoint[k], j+i);	
				}
			}
		}

		// The first term is the number of data points (num_data).
		sumMatrix[0][0] = m_num_data;

		// Calculate the matrix using Guassian reduction.
		CalculateMatrix(sumMatrix, coeB, coeA);
	}
	catch(const exception& ex) 
	{
		// Get the error thrown
		const char* errMessage = ex.what();
		throw;
	}
	finally 
	{
		// Delete the coefficient B values.
		if(coeB != nullptr)
			delete[] coeB;

		// If the objects has been created.
		if(sumMatrix != nullptr)
		{
			// Delete the memory allocation
			// for items in the array.
			for (int i = 0; i < m_nxn_matrix; i++)
			{
				if(sumMatrix[i] != nullptr)
					delete[] sumMatrix[i];
			}

			// Delete the memory for the top-level array.
			delete[] sumMatrix;
		}

		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Calculate the matrix using Guassian reduction.
///	</summary>
/// <param name='sumMatrix'>The collection of Sum values.</param>
/// <param name='coeB'>The collection of B values.</param>
/// <param name='coeA'>The coefficient collection reference (will contain the result when complete).</param>
void Nequeo::Math::Equation::CalculateMatrix(double** sumMatrix, double* coeB, double* coeA)
{
	double mult_matrix, sum_matrix;

	// Elimination of lower triangular part.
	for(int i = 0; i < m_nxn_matrix-1; i++)
	{
		for(int k = i + 1; k < m_nxn_matrix; k++)
		{
			mult_matrix = sumMatrix[k][i] / sumMatrix[i][i];
			for(int j = 0; j < m_nxn_matrix; j++)
			{
				sumMatrix[k][j] -= mult_matrix * sumMatrix[i][j];
			}
			coeB[k] -= mult_matrix * coeB[i];
		}
	}

	// Back substitution.
	for(int i = m_nxn_matrix - 1; i >= 0; i--)
	{
		sum_matrix = 0.0;
		for(int j = i + 1; j < m_nxn_matrix; j++)
		{
			sum_matrix += sumMatrix[i][j] * coeA[j];
		}
		coeA[i] = (coeB[i] - sum_matrix) / sumMatrix[i][i];
	}
}

///	<summary>
///	Calculates the n factorial of the function.
///	</summary>
/// <param name='nthTerm'>The n-th term to calculate.</param>
/// <returns>The nth factorial.</returns>
double Nequeo::Math::Equation::Factorial(int nthTerm)
{
	double result;

	result = 1;
	
	// Calculate the Factorial;
	for(int i = 1; i <= nthTerm; i++)
	{
		result = result * i;
	}

	return (result);
}

///	<summary>
///	Calculate Maclaurin values for the distribution function.
///	</summary>
/// <param name='nthFunction'>The nth function value.</param>
/// <param name='nth'>The n-th term value.</param>
/// <returns>The Maclaurin function value.</returns>
double Nequeo::Math::Equation::MaclaurinFunction(double nthFunction, int nth)
{
	return (-nthFunction * (nth-1));
}

///	<summary>
///	Calculates the height of each interval.
///	</summary>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='nth'>The number of intervals, the larger this value is the smaller the partitions then more accurate the result.</param>
/// <returns>The new partition.</returns>
double Nequeo::Math::Equation::Partitions(double upperLimit, double lowerLimit, int nth)
{
	return ((upperLimit - lowerLimit) / nth);
}

///	<summary>
///	Calculate the Even and Odd ordinates.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='nth'>The number of intervals, the larger this value is the smaller the partitions then more accurate the result.</param>
/// <returns>The sum off all odd and event partitions.</returns>
double Nequeo::Math::Equation::SumOddEvenOrdinates(Nequeo::Math::FunctionHandler^ function, double upperLimit, double lowerLimit, int nth)
{
	double odd_even, interval, partit;
	int flag_odd_even;
	
	odd_even = 0.0;
	flag_odd_even = 0;

	// Get the current interval.
	partit = Partitions(upperLimit, lowerLimit, nth);
	
	for(int i = 1; i < nth; i++)
	{
		interval = lowerLimit + (i * partit);

		if(flag_odd_even == 0)
		{
			odd_even += 4 * function(interval);
			flag_odd_even = 1;
		}
		else if(flag_odd_even == 1)
		{
			odd_even += 2 * function(interval);
			flag_odd_even = 0;
		}
	}

	// Return the odd and even value.
	return(odd_even);
}

///	<summary>
///	Calculates the upper and lower ordinates of the integral.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <returns>The sum of the upper (b) and lower (a) function values.</returns>
double Nequeo::Math::Equation::SumEndOrdinates(Nequeo::Math::FunctionHandler^ function, double upperLimit, double lowerLimit)
{
	return (function(upperLimit)) + (function(lowerLimit));
}

///	<summary>
///	Polynomial function calculation handler.
///	</summary>
/// <param name='x'>The x parameter within the function to integrate.</param>
/// <returns>The result of the function for each parameter.</returns>
double Nequeo::Math::Equation::PolynomialFunctionHandler(double x)
{
	double result = 0.0;

	// For each coefficient in the polynomial calculate the value.
	for(int i = 0; i < m_coefficients->Length; i++)
	{
		// If the zero coefficient
		if(i == 0)
		{
			// Get the zero coefficient.
			result += m_coefficients[i];
		}
		else
		{
			// Calculate all other polynomial coefficients.
			result += (m_coefficients[i] * pow(x, i));
		}
	}

	// Return the result.
	return result;
}

///	<summary>
///	Approximate the value of a definite integral using the adaptive 2-point Gaussian quadrature rule.
///	</summary>
void Nequeo::Math::Equation::Gaussian2Adaptive(Nequeo::Math::FunctionHandler^ function, double sab, double a, double b, double TOL, double* approx, double* errest, int* nfunc)
{
	double h2, fml, fpl, fmr, fpr, sac, scb, app1, app2, est1, est2, eest;
	int nf1, nf2;
     
	h2 = (b - a) / 4.0;
	fml = function(a + h2 - (sqrt(1.0 / 3.0) * h2));
	fpl = function(a + h2 + (sqrt(1.0 / 3.0) * h2));
	fmr = function(b - h2 - (sqrt(1.0 / 3.0) * h2));
	fpr = function(b - h2 + (sqrt(1.0 / 3.0) * h2));
	sac = h2 * (fml + fpl);
	scb = h2 * (fmr + fpr);
     
	eest = fabs (sab - sac - scb);
	if (eest < (10.0 * TOL)) 
	{
		*approx = sac + scb;
		*errest = eest / 10.0;
		*nfunc  = 4;
	}
	else 
	{
		Gaussian2Adaptive(function, sac, a, a + (2.0 * h2), (TOL / 2.0), &app1, &est1, &nf1);
		Gaussian2Adaptive(function, scb, a + (2.0 * h2), b, (TOL / 2.0), &app2, &est2, &nf2);
		*approx = app1 + app2;
		*errest = est1 + est2;
		*nfunc  = nf1  + nf2 + 4;
	}
}

///	<summary>
///	Approximate the value of a definite integral using the adaptive 3-point Gaussian quadrature rule.
///	</summary>
void Nequeo::Math::Equation::Gaussian3Adaptive(Nequeo::Math::FunctionHandler^ function, double sab, double a, double b, double TOL, double* approx, double* errest, int* nfunc)
{
	double h2, fml, fcl, fpl, fmr, fcr, fpr, sac, scb, app1, app2, est1, est2, eest;
    int nf1, nf2;
     
	h2 = (b - a) / 4.0;
	fml = function(a + h2 - (sqrt(0.6) * h2));
	fcl = function(a + h2 );
	fpl = function(a + h2 + (sqrt(0.6) * h2));
	fmr = function(b - h2 - (sqrt(0.6) * h2));
	fcr = function(b - h2 );
	fpr = function(b - h2 + (sqrt(0.6) * h2));
	sac = h2 * ((5.0 * fml) + (8.0 * fcl) + (5.0 * fpl)) / 9.0;
	scb = h2 * ((5.0 * fmr) + (8.0 * fcr) + (5.0 * fpr)) / 9.0;
     
	eest = fabs (sab - sac - scb);
	if (eest < (42.0 * TOL)) 
	{
		*approx = sac + scb;
		*errest = eest / 42.0;
		*nfunc  = 6;
	}
	else 
	{
		Gaussian3Adaptive(function, sac, a, a + (2.0 * h2), (TOL / 2.0), &app1, &est1, &nf1);
		Gaussian3Adaptive(function, scb, a + (2.0 * h2), b, (TOL / 2.0), &app2, &est2, &nf2);
		*approx = app1 + app2;
		*errest = est1 + est2;
		*nfunc  = nf1  + nf2 + 6;
	}
}

///	<summary>
///	Approximate the value of a definite integral using the adaptive Simpson's rule.
///	</summary>
void Nequeo::Math::Equation::SimpsonAdaptive(Nequeo::Math::FunctionHandler^ function, double sab, double fa, double fc, double fb, double a, double b, double TOL, double* approx, double* errest, int* nfunc)
{
	double c, fd, fe, sac, scb, app1, app2, est1, est2, eest;
	int nf1, nf2;
     
	c = (a + b) / 2.0;
	fd = function((a + c) / 2.0 );
	fe = function((c + b) / 2.0 );
	sac = (c - a) * (fa + (4.0 * fd) + fc) / 6.0;
	scb = (b - c) * (fc + (4.0 * fe) + fb) / 6.0;
     
	eest = fabs (sab - sac - scb);
	if (eest < (10.0 * TOL)) 
	{
		*approx = sac + scb;
		*errest = eest / 10.0;
		*nfunc  = 2;
	}
	else 
	{
		SimpsonAdaptive(function, sac, fa, fd, fc, a, c, (TOL / 2.0), &app1, &est1, &nf1);
		SimpsonAdaptive(function, scb, fc, fe, fb, c, b, (TOL / 2.0), &app2, &est2, &nf2);
		*approx = app1 + app2;
		*errest = est1 + est2;
		*nfunc  = nf1  + nf2 + 2;
	}
}