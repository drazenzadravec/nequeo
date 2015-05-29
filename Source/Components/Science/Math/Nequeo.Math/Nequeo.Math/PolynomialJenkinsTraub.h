/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          PolynomialJenkinsTraub.h
 *  Purpose :       Calculates the roots of a polynomial (real coefficients) 
					equation '0 = a[0] + a[1]x + a[2]x^2 + a[3]x^3 + .......+ a[n]x^n\n'. 
					Calculations are based on the Jenkins-Traub algorithm.
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

namespace Nequeo 
{
	namespace Math 
	{
		///	<summary>
		///	Calculates the roots of a polynomial (real coefficients).
		///	</summary>
		class PolynomialJenkinsTraub
		{
			public:
				// Constructors
				PolynomialJenkinsTraub();
				virtual ~PolynomialJenkinsTraub();

				// Methods
				void Calculate(double op[MDP1], int* Degree, double zeror[MAXDEGREE], double zeroi[MAXDEGREE]);
				string GetError();
				void SetError(string error);

			private:
				// Fields
				bool m_disposed;
				string m_error;

				// Methods
				void Fxshfr(int L2, int* NZ, double sr, double v, double K[MDP1], int N, double p[MDP1], int NN, double qp[MDP1], double u, double* lzi, double* lzr, double* szi, double* szr);
				void QuadSD(int NN, double u, double v, double p[MDP1], double q[MDP1], double* a, double* b);
				int CalcSC(int N, double a, double b, double* a1, double* a3, double* a7, double* c, double* d, double* e, double* f, double* g, double* h, double K[MDP1], double u, double v, double qk[MDP1]);
				void NextK(int N, int tFlag, double a, double b, double a1, double* a3, double* a7, double K[MDP1], double qk[MDP1], double qp[MDP1]);
				void Newest(int tFlag, double* uu, double* vv, double a, double a1, double a3, double a7, double b, double c, double d, double f, double g, double h, double u, double v, double K[MDP1], int N, double p[MDP1]);
				void QuadIT(int N, int* NZ, double uu, double vv, double* szr, double* szi, double* lzr, double* lzi, double qp[MDP1], int NN, double* a, double* b, double p[MDP1], double qk[MDP1], double* a1, double* a3, double* a7, double* c, double* d, double* e, double* f, double* g, double* h, double K[MDP1]);
				void RealIT(int* iFlag, int* NZ, double* sss, int N, double p[MDP1], int NN, double qp[MDP1], double* szr, double* szi, double K[MDP1], double qk[MDP1]);
				void Quad(double a, double b1, double c, double* sr, double* si, double* lr, double* li);

		};
	}
}