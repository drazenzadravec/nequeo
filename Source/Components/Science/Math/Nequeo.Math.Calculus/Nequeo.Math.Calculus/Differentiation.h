
#pragma once

#ifndef _DIFFERENTIATION_H
#define _DIFFERENTIATION_H

#include "Stdafx.h"
#include "Arithmetic.h"

namespace Nequeo
{
namespace Math
{
namespace Calculus
{
	///	<summary>
	///	Differential calculus calculations.
	///	</summary>
    class MATHCALCULUS_API Differentiation
    {
		public: 
			// Constructors
			Differentiation();
			~Differentiation();

			virtual double Equation(double (*function)(double), double x, int differentialOrder);
			virtual double Equation(double (*function)(double), double x, int differentialOrder, double delta);

			virtual double PolynomialEquation(double coefficients[], int coefficientSize, double x, int differentialOrder);
			virtual double PolynomialEquation(double coefficients[], int coefficientSize, double x, int differentialOrder, double delta);

		private:
			static double PolynomialFunctionHandler(double x);
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

#endif //_DIFFERENTIATION_H