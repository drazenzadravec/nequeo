
#include "Integration.h"

using namespace std;
using namespace concurrency;

///	<summary>
///	Construct the differentiation.
///	</summary>
Nequeo::Math::Calculus::Integration::Integration() : m_disposed(false), m_degree(0)
{
}

///	<summary>
///	Deconstruct the differentiation.
///	</summary>
Nequeo::Math::Calculus::Integration::~Integration()
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
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the Simpson's Rule.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Integration::Equation(double (*function)(double), double upperLimit, double lowerLimit)
{
	return Equation(function, upperLimit, lowerLimit, 2000);
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the Simpson's Rule.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='intervalFactor'>Set this value to calculate more accurate integrals (minimum is set at 2000).</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Integration::Equation(double (*function)(double), double upperLimit, double lowerLimit, int intervalFactor)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw exception("A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	int n;
	double integral;

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
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Integration::EquationSimpsonAdaptive(double (*function)(double), double upperLimit, double lowerLimit, double errorTolerance, 
																	_Out_ double* errorEstimate, _Out_ int* numberFunction)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw exception("A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	double approx;
	double errest;
	int nfunc;
	double integral;
	double fa, fc, fb, sab;

	// Calculate the integral for the function.
	fa = (*function)(lowerLimit);
	fc = (*function)((lowerLimit + upperLimit) / 2.0);
	fb = (*function)(upperLimit);
	sab = (upperLimit - lowerLimit) * (fa + (4.0 * fc) + fb) / 6.0;
	SimpsonAdaptive(function, sab, fa, fc, fb, lowerLimit, upperLimit, errorTolerance, &approx, &errest, &nfunc);
	nfunc += 3;

	// Return the integral.
	*errorEstimate = errest;
	*numberFunction = nfunc;

	integral = approx;
	return integral;
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the 2-point Gaussian Quadrature.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Integration::EquationGaussian2(double (*function)(double), double upperLimit, double lowerLimit)
{
	return EquationGaussian2(function, upperLimit, lowerLimit, 2000);
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the 2-point Gaussian Quadrature.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='intervalFactor'>Set this value to calculate more accurate integrals (minimum is set at 2000).</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Integration::EquationGaussian2(double (*function)(double), double upperLimit, double lowerLimit, int intervalFactor)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw exception("A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	double integral;
	double h;
    double sqthird;
    double temp;
    double sum;
	int n;

	n = intervalFactor;

	// Get the height of each interval
	h = (upperLimit - lowerLimit) / (2.0 * (double)n);
	sqthird = sqrt(3.0) / 3.0;
     
	// Sum the interval values.
	sum = 0.0;
	for (int i = 1; i <= n; i++) 
	{
		temp = lowerLimit + (((2 * i) - 1) * h);
		sum += ((*function)(temp - (h * sqthird)) + (*function)(temp + (h * sqthird)));
	}

	// Calculate the integral.
	integral = h * sum;

	// Return the integral.
	return integral;
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
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Integration::EquationGaussian2Adaptive(double (*function)(double), double upperLimit, double lowerLimit, double errorTolerance, 
																	  _Out_ double* errorEstimate, _Out_ int* numberFunction)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw exception("A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	double approx;
	double errest;
	int nfunc;
	double integral;
	double h2, fm, fp, sab;

	// Calculate the integral for the function.
	h2 = (upperLimit - lowerLimit) / 2.0;
	fm = (*function)(lowerLimit + h2 - (sqrt(1.0/3.0) * h2));
	fp = (*function)(lowerLimit + h2 + (sqrt(1.0/3.0) * h2));
	sab = (h2 * (fm + fp));
	Gaussian2Adaptive(function, sab, lowerLimit, upperLimit, errorTolerance, &approx, &errest, &nfunc);
	nfunc += 2;

	// Return the integral.
	*errorEstimate = errest;
	*numberFunction = nfunc;

	integral = approx;
	return integral;
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the 3-point Gaussian Quadrature.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Integration::EquationGaussian3(double (*function)(double), double upperLimit, double lowerLimit)
{
	return EquationGaussian3(function, upperLimit, lowerLimit, 2000);
}

///	<summary>
///	Calculate the definate integral over x = (a, b). The technique used to calculate the integral is the 3-point Gaussian Quadrature.
///	</summary>
/// <param name='function'>The handler to the function to integrate over x = (a, b).</param>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='intervalFactor'>Set this value to calculate more accurate integrals (minimum is set at 2000).</param>
/// <returns>The value of the integral over x = (a, b).</returns>
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Integration::EquationGaussian3(double (*function)(double), double upperLimit, double lowerLimit, int intervalFactor)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw exception("A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	double integral;
	double h;
    double sq35;
    double temp;
	double c1;
	double c2;
    double sum;
	int n;

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
		sum += c1 * ((*function)(temp - (h * sq35)) + (*function)(temp + (h * sq35)));
		sum += c2 * (*function)(temp);
	}

	// Calculate the integral.
	integral = h * sum;

	// Return the integral.
	return integral;
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
/// <exception cref="std::exception">Thrown when any internal exception occures.</exception>
double Nequeo::Math::Calculus::Integration::EquationGaussian3Adaptive(double (*function)(double), double upperLimit, double lowerLimit, double errorTolerance, 
																	  _Out_ double* errorEstimate, _Out_ int* numberFunction)
{
	// Make sure that a valid function handler exits.
	if((function == nullptr))
	{
		throw exception("A valid function handler has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	double approx;
	double errest;
	int nfunc;
	double integral;
	double h2, fm, fc, fp, sab;

	// Calculate the integral for the function.
	h2 = (upperLimit - lowerLimit) / 2.0;
	fm = (*function)(lowerLimit + h2 - (sqrt(0.6) * h2));
	fc = (*function)(lowerLimit + h2 );
	fp = (*function)(lowerLimit + h2 + (sqrt(0.6) * h2));
	sab = h2 * ((5.0  *fm) + (8.0 * fc) + (5.0 * fp)) / 9.0;
	Gaussian3Adaptive(function, sab, lowerLimit, upperLimit, errorTolerance, &approx, &errest, &nfunc);
	nfunc += 3;

	// Return the integral.
	*errorEstimate = errest;
	*numberFunction = nfunc;

	integral = approx;
	return integral;
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
double Nequeo::Math::Calculus::Integration::PolynomialEquation(double coefficients[], int coefficientSize, double upperLimit, double lowerLimit)
{
	return PolynomialEquation(coefficients, coefficientSize, upperLimit, lowerLimit, 2000);
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
double Nequeo::Math::Calculus::Integration::PolynomialEquation(double coefficients[], int coefficientSize, double upperLimit, double lowerLimit, int intervalFactor)
{
	// Make sure that a valid set of data points exits.
	if(coefficients == nullptr)
	{
		throw exception("A valid set of coefficients has not been supplied.");
	}

	// Make sure that the upper (b) is greater then lower (a).
	if(upperLimit < lowerLimit)
	{
		throw exception("The upper limit (b) must be greater then the lower limit (a).");
	}

	m_coefficientsSize = coefficientSize;

	// Assign the coefficients.
	m_coefficients = new double[m_coefficientsSize];
	for(int i = 0; i < m_coefficientsSize; i++)
	{
		m_coefficients[i] = coefficients[i];
	}
	
	// Assign the internal function handler.
	InternalFunctionHandler = &Nequeo::Math::Calculus::Integration::PolynomialFunctionHandler;

	// Return the integral.
	return Equation(InternalFunctionHandler, upperLimit, lowerLimit, intervalFactor);
}

///	<summary>
///	Polynomial function calculation handler.
///	</summary>
/// <param name='x'>The x parameter within the function to integrate.</param>
/// <returns>The result of the function for each parameter.</returns>
double Nequeo::Math::Calculus::Integration::PolynomialFunctionHandler(double x)
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
///	Calculates the height of each interval.
///	</summary>
/// <param name='upperLimit'>The upper (b) limit value.</param>
/// <param name='lowerLimit'>The lower (a) limit value.</param>
/// <param name='nth'>The number of intervals, the larger this value is the smaller the partitions then more accurate the result.</param>
/// <returns>The new partition.</returns>
double Nequeo::Math::Calculus::Integration::Partitions(double upperLimit, double lowerLimit, int nth)
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
double Nequeo::Math::Calculus::Integration::SumOddEvenOrdinates(double (*function)(double), double upperLimit, double lowerLimit, int nth)
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
			odd_even += 4 * (*function)(interval);
			flag_odd_even = 1;
		}
		else if(flag_odd_even == 1)
		{
			odd_even += 2 * (*function)(interval);
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
double Nequeo::Math::Calculus::Integration::SumEndOrdinates(double (*function)(double), double upperLimit, double lowerLimit)
{
	return ((*function)(upperLimit)) + ((*function)(lowerLimit));
}

///	<summary>
///	Approximate the value of a definite integral using the adaptive Simpson's rule.
///	</summary>
void Nequeo::Math::Calculus::Integration::SimpsonAdaptive(double (*function)(double), double sab, double fa, double fc, double fb, double a, double b, double TOL, double* approx, double* errest, int* nfunc)
{
	double c, fd, fe, sac, scb, app1, app2, est1, est2, eest;
	int nf1, nf2;
     
	c = (a + b) / 2.0;
	fd = (*function)((a + c) / 2.0 );
	fe = (*function)((c + b) / 2.0 );
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

///	<summary>
///	Approximate the value of a definite integral using the adaptive 2-point Gaussian quadrature rule.
///	</summary>
void Nequeo::Math::Calculus::Integration::Gaussian2Adaptive(double (*function)(double), double sab, double a, double b, double TOL, double* approx, double* errest, int* nfunc)
{
	double h2, fml, fpl, fmr, fpr, sac, scb, app1, app2, est1, est2, eest;
	int nf1, nf2;
     
	h2 = (b - a) / 4.0;
	fml = (*function)(a + h2 - (sqrt(1.0 / 3.0) * h2));
	fpl = (*function)(a + h2 + (sqrt(1.0 / 3.0) * h2));
	fmr = (*function)(b - h2 - (sqrt(1.0 / 3.0) * h2));
	fpr = (*function)(b - h2 + (sqrt(1.0 / 3.0) * h2));
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
void Nequeo::Math::Calculus::Integration::Gaussian3Adaptive(double (*function)(double), double sab, double a, double b, double TOL, double* approx, double* errest, int* nfunc)
{
	double h2, fml, fcl, fpl, fmr, fcr, fpr, sac, scb, app1, app2, est1, est2, eest;
    int nf1, nf2;
     
	h2 = (b - a) / 4.0;
	fml = (*function)(a + h2 - (sqrt(0.6) * h2));
	fcl = (*function)(a + h2 );
	fpl = (*function)(a + h2 + (sqrt(0.6) * h2));
	fmr = (*function)(b - h2 - (sqrt(0.6) * h2));
	fcr = (*function)(b - h2 );
	fpr = (*function)(b - h2 + (sqrt(0.6) * h2));
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
///	Polynomial coefficient collection.
///	</summary>
double* Nequeo::Math::Calculus::Integration::m_coefficients;

///	<summary>
///	Polynomial coefficient size.
///	</summary>
int Nequeo::Math::Calculus::Integration::m_coefficientsSize;