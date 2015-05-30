
#pragma once

#ifndef _INTEGRATION_H
#define _INTEGRATION_H

#include "Stdafx.h"
#include "Arithmetic.h"

namespace Nequeo
{
namespace Math
{
namespace Calculus
{
	///	<summary>
	///	Integral calculus calculations.
	///	</summary>
    class MATHCALCULUS_API Integration
    {
		public: 
			// Constructors
			Integration();
			~Integration();

			virtual double Equation(double (*function)(double), double upperLimit, double lowerLimit);
			virtual double Equation(double (*function)(double), double upperLimit, double lowerLimit, int intervalFactor);
			virtual double EquationSimpsonAdaptive(double (*function)(double), double upperLimit, double lowerLimit, double errorTolerance, _Out_ double* errorEstimate, _Out_ int* numberFunction);
				
			virtual double EquationGaussian2(double (*function)(double), double upperLimit, double lowerLimit);
			virtual double EquationGaussian2(double (*function)(double), double upperLimit, double lowerLimit, int intervalFactor);
			virtual double EquationGaussian2Adaptive(double (*function)(double), double upperLimit, double lowerLimit, double errorTolerance, _Out_ double* errorEstimate, _Out_ int* numberFunction);

			virtual double EquationGaussian3(double (*function)(double), double upperLimit, double lowerLimit);
			virtual double EquationGaussian3(double (*function)(double), double upperLimit, double lowerLimit, int intervalFactor);
			virtual double EquationGaussian3Adaptive(double (*function)(double), double upperLimit, double lowerLimit, double errorTolerance, _Out_ double* errorEstimate, _Out_ int* numberFunction);

			virtual double PolynomialEquation(double coefficients[], int coefficientSize, double upperLimit, double lowerLimit);
			virtual double PolynomialEquation(double coefficients[], int coefficientSize, double upperLimit, double lowerLimit, int intervalFactor);

		private:
			static double PolynomialFunctionHandler(double x);

			double Partitions(double upperLimit, double lowerLimit, int nth);
			double SumOddEvenOrdinates(double (*function)(double), double upperLimit, double lowerLimit, int nth);
			double SumEndOrdinates(double (*function)(double), double upperLimit, double lowerLimit);

			void SimpsonAdaptive(double (*function)(double), double sab, double fa, double fc, double fb, double a, double b, double TOL, double* approx, double* errest, int* nfunc);
			void Gaussian2Adaptive(double (*function)(double), double sab, double a, double b, double TOL, double* approx, double* errest, int* nfunc);
			void Gaussian3Adaptive(double (*function)(double), double sab, double a, double b, double TOL, double* approx, double* errest, int* nfunc);

			double (*InternalFunctionHandler)(double);

			// Fields
			bool m_disposed;
			int m_degree;
			static double* m_coefficients;
			static int m_coefficientsSize;

    };
}
}
}

#endif //_INTEGRATION_H