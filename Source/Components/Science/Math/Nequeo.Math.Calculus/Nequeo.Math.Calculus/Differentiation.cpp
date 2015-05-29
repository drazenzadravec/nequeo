
#include "Differentiation.h"

using namespace std;
using namespace concurrency;

///	<summary>
///	Construct the differentiation.
///	</summary>
Nequeo::Math::Calculus::Differentiation::Differentiation() : m_disposed(false), m_degree(0)
{
}

///	<summary>
///	Deconstruct the differentiation.
///	</summary>
Nequeo::Math::Calculus::Differentiation::~Differentiation()
{
	// If not disposed.
    if (!m_disposed)
    {
		// Delete the coefficients.
		if(m_coefficients != nullptr)
		{
			delete[] m_coefficients;
		}

        m_disposed = true;
    }
}

///	<summary>
///	Calculate the differential of 'f(x)'. The technique used to calculate the differential is the Taylor Series.
///	</summary>
/// <param name='function'>The handler to the function to differentiate at x.</param>
/// <param name='differentialOrder'>The differential order to calculate (1 to 5 inclusive).</param>
/// <returns>The value of the differential at x.</returns>
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Differentiation::Equation(double (*function)(double), double x, int differentialOrder)
{
	return Equation(function, x, differentialOrder, (1.0 / 1000));
}

///	<summary>
///	Calculate the differential of 'f(x)'. The technique used to calculate the differential is the Taylor Series.
///	</summary>
/// <param name='function'>The handler to the function to differentiate at x.</param>
/// <param name='differentialOrder'>The differential order to calculate (1 to 5 inclusive).</param>
/// <param name='delta'>The delta (change as h -> 0, where delta = h) to x.</param>
/// <returns>The value of the differential at x.</returns>
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Differentiation::Equation(double (*function)(double), double x, int differentialOrder, double delta)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw exception("A valid function handler has not been supplied.");
	}

	// Make sure that the differential order is within range.
	if(differentialOrder < 1 || differentialOrder > 5)
	{
		throw exception("The differential order must be between 1 and 5 (inclusive)");
	}

	double differential;
	
	// Select the differential order to calculate.
	switch (differentialOrder)
	{
		case 1:
			differential = 
				(
					(-3 * (*function)(x + (4 * delta))) +
					(16 * (*function)(x + (3 * delta))) +
					(-36 * (*function)(x + (2 * delta))) +
					(48 * (*function)(x + (1 * delta))) +
					(-25 * (*function)(x + (0 * delta)))
				) / (12 * delta);
			break;

		case 2:
			differential = 
				(
					(11 * (*function)(x + (4 * delta))) +
					(-56 * (*function)(x + (3 * delta))) +
					(114 * (*function)(x + (2 * delta))) +
					(-104 * (*function)(x + (1 * delta))) +
					(35 * (*function)(x + (0 * delta)))
				) / (12 * delta * delta);
			break;

		case 3:
			differential = 
				(
					(-3 * (*function)(x + (4 * delta))) +
					(14 * (*function)(x + (3 * delta))) +
					(-24 * (*function)(x + (2 * delta))) +
					(18 * (*function)(x + (1 * delta))) +
					(-5 * (*function)(x + (0 * delta)))
				) / (2 * delta * delta * delta);
			break;

		case 4:
			differential = 
				(
					(1 * (*function)(x + (4 * delta))) +
					(-4 * (*function)(x + (3 * delta))) +
					(6 * (*function)(x + (2 * delta))) +
					(-4 * (*function)(x + (1 * delta))) +
					(1 * (*function)(x + (0 * delta)))
				) / (delta * delta * delta * delta);
			break;

		case 5:
			differential = 
				(
					(1 * (*function)(x + (5 * delta))) +
					(-5 * (*function)(x + (4 * delta))) +
					(10 * (*function)(x + (3 * delta))) +
					(-10 * (*function)(x + (2 * delta))) +
					(5 * (*function)(x + (1 * delta))) +
					(-1 * (*function)(x + (0 * delta)))
				) / (delta * delta * delta * delta * delta);
			break;

		default:
			differential = 0;
			break;
	}

	// Return the differential.
	return differential;
}

///	<summary>
///	Calculate the polynomial differential (real coefficients) equation 'f(x) = a[0] + a[1]x + a[2]x^2 + a[3]x^3 + .......+ a[n]x^n'. 
/// The technique used to calculate the differential is the Taylor Series.
///	</summary>
/// <param name='coefficients'>The set of (a) coefficients. lowest order (a[0]) to highest order (a[n])</param>
/// <param name='coefficientSize'>The number of coefficients in the array.</param>
/// <param name='differentialOrder'>The differential order to calculate (1 to 5 inclusive).</param>
/// <returns>The value of the differential at x.</returns>
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Differentiation::PolynomialEquation(double coefficients[], int coefficientSize, double x, int differentialOrder)
{
	return PolynomialEquation(coefficients, coefficientSize, x, differentialOrder, (1.0 / 1000));
}

///	<summary>
///	Calculate the polynomial differential (real coefficients) equation 'f(x) = a[0] + a[1]x + a[2]x^2 + a[3]x^3 + .......+ a[n]x^n'. 
/// The technique used to calculate the differential is the Taylor Series.
///	</summary>
/// <param name='coefficients'>The set of (a) coefficients. lowest order (a[0]) to highest order (a[n])</param>
/// <param name='coefficientSize'>The number of coefficients in the array.</param>
/// <param name='differentialOrder'>The differential order to calculate (1 to 5 inclusive).</param>
/// <param name='delta'>The delta (change as h -> 0, where delta = h) to x.</param>
/// <returns>The value of the differential at x.</returns>
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Differentiation::PolynomialEquation(double coefficients[], int coefficientSize, double x, int differentialOrder, double delta)
{
	// Make sure that a valid set of data points exits.
	if(coefficients == nullptr)
	{
		throw exception("A valid set of coefficients has not been supplied.");
	}

	// Make sure that the differential order is within range.
	if(differentialOrder < 1 || differentialOrder > 5)
	{
		throw exception("The differential order must be between 1 and 5 (inclusive)");
	}

	m_coefficientsSize = coefficientSize;

	// Assign the coefficients.
	m_coefficients = new double[m_coefficientsSize];
	for(int i = 0; i < m_coefficientsSize; i++)
	{
		m_coefficients[i] = coefficients[i];
	}
	
	// Assign the internal function handler.
	InternalFunctionHandler = &Nequeo::Math::Calculus::Differentiation::PolynomialFunctionHandler;

	// Return the differential.
	return Equation(InternalFunctionHandler, x, differentialOrder, delta);
}

///	<summary>
///	Polynomial function calculation handler.
///	</summary>
/// <param name='x'>The x parameter within the function to integrate.</param>
/// <returns>The result of the function for each parameter.</returns>
double Nequeo::Math::Calculus::Differentiation::PolynomialFunctionHandler(double x)
{
	double result = 0.0;

	// For each coefficient in the polynomial calculate the value.
	for(int i = 0; i < m_coefficientsSize; i++)
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
///	Polynomial coefficient collection.
///	</summary>
double* Nequeo::Math::Calculus::Differentiation::m_coefficients;

///	<summary>
///	Polynomial coefficient size.
///	</summary>
int Nequeo::Math::Calculus::Differentiation::m_coefficientsSize;
