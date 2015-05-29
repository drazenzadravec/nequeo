/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          PolynomialJenkinsTraub.cpp
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

#include "stdafx.h"

#include "PolynomialJenkinsTraub.h"

///	<summary>
///	Construct the equation.
///	</summary>
Nequeo::Math::PolynomialJenkinsTraub::PolynomialJenkinsTraub() : m_disposed(true), m_error("")
{
	m_disposed = false;
}

///	<summary>
///	Deconstruct the equation.
///	</summary>
Nequeo::Math::PolynomialJenkinsTraub::~PolynomialJenkinsTraub()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

///	<summary>
///	Get the current error.
///	</summary>
string Nequeo::Math::PolynomialJenkinsTraub::GetError()
{
	return m_error;
}

///	<summary>
///	Set the current error.
///	</summary>
void Nequeo::Math::PolynomialJenkinsTraub::SetError(string error)
{
	m_error = error;
	return;
}

///	<summary>
///	Calculates the roots of a polynomial (real coefficients).
///	</summary>
void Nequeo::Math::PolynomialJenkinsTraub::Calculate(double op[MDP1], int* Degree, double zeror[MAXDEGREE], double zeroi[MAXDEGREE])
{
	int i, j, jj, l, N, NM1, NN, NZ, zerok;
 
	double K[MDP1], p[MDP1], pt[MDP1], qp[MDP1], temp[MDP1];
	double bnd, df, dx, factor, ff, moduli_max, moduli_min, sc, x, xm;
	double aa, bb, cc, lzi, lzr, sr, szi, szr, t, u, xx, xxx, yy;
 

	const double RADFAC = PI / 180;				// Degrees-to-radians conversion factor = pi/180
	const double lb2 = log(2.0);				// Dummy variable to avoid re-calculating this value in loop below
	const double lo = DBL_MIN / DBL_EPSILON;
	const double cosr = cos(94.0*RADFAC);		// = -0.069756474
	const double sinr = sin(94.0*RADFAC);		// = 0.99756405
 
	if ((*Degree) > MAXDEGREE)
	{
		m_error = "\nThe entered Degree is greater than " + to_string(MAXDEGREE) + ". Exiting rpoly. No further action taken.\n";
		*Degree = -1;
		return;
	} 
 
	//Do a quick check to see if leading coefficient is 0
	if (op[0] != 0)
	{
		N = *Degree;
		xx = sqrt(0.5); // = 0.70710678
		yy = -xx;
 
		// Remove zeros at the origin, if any
		j = 0;
		while (op[N] == 0)
		{
			zeror[j] = zeroi[j] = 0.0;
			N--;
			j++;
		}
 
		NN = N + 1;
 
		// Make a copy of the coefficients
		for (i = 0; i < NN; i++) p[i] = op[i];
 
		while (N >= 1)
		{ 
			// Main loop
			// Start the algorithm for one zero
			if (N <= 2)
			{
				// Calculate the final zero or pair of zeros
				if (N < 2)
				{
					zeror[(*Degree) - 1] = -(p[1]/p[0]);
					zeroi[(*Degree) - 1] = 0.0;
				} 
				else 
				{
					Quad(p[0], p[1], p[2], &zeror[(*Degree) - 2], &zeroi[(*Degree) - 2], &zeror[(*Degree) - 1], &zeroi[(*Degree) - 1]);
				}
				break;
			}
 
			// Find the largest and smallest moduli of the coefficients
			moduli_max = 0.0;
			moduli_min = DBL_MAX;
 
			for (i = 0; i < NN; i++)
			{
				x = fabs(p[i]);
				if (x > moduli_max) moduli_max = x;
				if ((x != 0) && (x < moduli_min)) moduli_min = x;
			}
 
			// Scale if there are large or very small coefficients
			// Computes a scale factor to multiply the coefficients of the polynomial. The scaling
			// is done to avoid overflow and to avoid undetected underflow interfering with the
			// convergence criterion.
			// The factor is a power of the base.
			sc = lo / moduli_min;
 
			if (((sc <= 1.0) && (moduli_max >= 10)) || ((sc > 1.0) && (DBL_MAX/sc >= moduli_max)))
			{
				sc = ((sc == 0) ? DBL_MIN : sc);
				l = (int)(log(sc)/lb2 + 0.5);
				factor = pow(2.0, l);
				if (factor != 1.0)
				{
					for (i = 0; i < NN; i++) p[i] *= factor;
				}
			}
 
			// Compute lower bound on moduli of zeros
			for (i = 0; i < NN; i++) pt[i] = fabs(p[i]);
			pt[N] = -(pt[N]);
 
			NM1 = N - 1;
 
			// Compute upper estimate of bound
			x = exp((log(-pt[N]) - log(pt[0]))/(double)N);
 
			if (pt[NM1] != 0) 
			{
				// If Newton step at the origin is better, use it
				xm = -pt[N]/pt[NM1];
				x = ((xm < x) ? xm : x);
			}
 
			// Chop the interval (0, x) until ff <= 0
			xm = x;
			do 
			{
				x = xm;
				xm = 0.1*x;
				ff = pt[0];
				for (i = 1; i < NN; i++) ff = ff *xm + pt[i];
			} while (ff > 0);
 
			dx = x;
 
			// Do Newton iteration until x converges to two decimal places
			do 
			{
				df = ff = pt[0];
				for (i = 1; i < N; i++)
				{
					ff = x*ff + pt[i];
					df = x*df + ff;
				}

				ff = x*ff + pt[N];
				dx = ff/df;
				 x -= dx;
			} while (fabs(dx/x) > 0.005);
 
			bnd = x;
 
			// Compute the derivative as the initial K polynomial and do 5 steps with no shift
			for (i = 1; i < N; i++) K[i] = (double)(N - i)*p[i]/((double)N);
			K[0] = p[0];
 
			aa = p[N];
			bb = p[NM1];
			zerok = ((K[NM1] == 0) ? 1 : 0);
 
			for (jj = 0; jj < 5; jj++) 
			{
				cc = K[NM1];
				if (zerok)
				{
					// Use unscaled form of recurrence
					for (i = 0; i < NM1; i++)
					{
						j = NM1 - i;
						K[j] = K[j - 1];
					}
					K[0] = 0;
					zerok = ((K[NM1] == 0) ? 1 : 0);
				}
				else 
				{ 
					// Used scaled form of recurrence if value of K at 0 is nonzero
					t = -aa/cc;
					for (i = 0; i < NM1; i++)
					{
						j = NM1 - i;
						K[j] = t*K[j - 1] + p[j];
					}

					K[0] = p[0];
					zerok = ((fabs(K[NM1]) <= fabs(bb)*DBL_EPSILON*10.0) ? 1 : 0);
				}
			}
 
			// Save K for restarts with new shifts
			for (i = 0; i < N; i++) temp[i] = K[i];
 
			// Loop to select the quadratic corresponding to each new shift
			for (jj = 1; jj <= 20; jj++)
			{
				// Quadratic corresponds to a double shift to a non-real point and its
				// complex conjugate. The point has modulus BND and amplitude rotated
				// by 94 degrees from the previous shift.
				xxx = -(sinr*yy) + cosr*xx;
				yy = sinr*xx + cosr*yy;
				xx = xxx;
				sr = bnd*xx;
				u = -(2.0*sr);
 
				// Second stage calculation, fixed quadratic
				Fxshfr(20*jj, &NZ, sr, bnd, K, N, p, NN, qp, u, &lzi, &lzr, &szi, &szr);
 
				if (NZ != 0)
				{
					// The second stage jumps directly to one of the third stage iterations and 
					// returns here if successful. Deflate the polynomial, store the zero or
					// zeros, and return to the main algorithm.
					j = (*Degree) - N;
					zeror[j] = szr;
					zeroi[j] = szi;
					NN = NN - NZ;
					N = NN - 1;
					for (i = 0; i < NN; i++) p[i] = qp[i];
					if (NZ != 1)
					{
						zeror[j + 1] = lzr;
						zeroi[j + 1] = lzi;
					}
					break;
				}
				else 
				{ 
					// If the iteration is unsuccessful, another quadratic is chosen after restoring K
					for (i = 0; i < N; i++) K[i] = temp[i];
				}
			}
 
			// Return with failure if no convergence with 20 shifts
			if (jj > 20) 
			{
				m_error = "\nFailure. No convergence after 20 shifts. Program terminated.\n";
				*Degree -= N;
				break;
			}
		}
	}
	else 
	{
		m_error = "\nThe leading coefficient is zero. No further action taken. Program terminated.\n";
		*Degree = 0;
	}
 
	return; 
}

///	<summary>
///	Computes up to L2 fixed shift K-polynomials, testing for convergence in the linear or
/// quadratic case. Initiates one of the variable shift iterations and returns with the
/// number of zeros found.
/// L2 limit of fixed shift steps
/// NZ number of zeros found
///	</summary>
void Nequeo::Math::PolynomialJenkinsTraub::Fxshfr(int L2, int* NZ, double sr, double v, double K[MDP1], int N, double p[MDP1], int NN, double qp[MDP1], double u, double* lzi, double* lzr, double* szi, double* szr)
{
	int fflag, i, iFlag = 1, j, spass, stry, tFlag, vpass, vtry; 
	double a, a1, a3, a7, b, betas, betav, c, d, e, f, g, h, oss, ots, otv, ovv, s, ss, ts, tss, tv, tvv, ui, vi, vv;
	double qk[MDP1], svk[MDP1];
 
	*NZ = 0;
	betav = betas = 0.25;
	oss = sr;
	ovv = v;
 
	// Evaluate polynomial by synthetic division 
	QuadSD(NN, u, v, p, qp, &a, &b);
 
	tFlag = CalcSC(N, a, b, &a1, &a3, &a7, &c, &d, &e, &f, &g, &h, K, u, v, qk);
 
	for (j = 0; j < L2; j++)
	{
		fflag = 1;
		// Calculate next K polynomial and estimate v
		NextK(N, tFlag, a, b, a1, &a3, &a7, K, qk, qp);
		tFlag = CalcSC(N, a, b, &a1, &a3, &a7, &c, &d, &e, &f, &g, &h, K, u, v, qk);
		Newest(tFlag, &ui, &vi, a, a1, a3, a7, b, c, d, f, g, h, u, v, K, N, p);
 
		vv = vi;
 
		// Estimate s
		ss = ((K[N - 1] != 0.0) ? -(p[N]/K[N - 1]) : 0.0);
 
		ts = tv = 1.0;
 
		if ((j != 0) && (tFlag != 3))
		{
			// Compute relative measures of convergence of s and v sequences
			tv = ((vv != 0.0) ? fabs((vv - ovv)/vv) : tv);
			ts = ((ss != 0.0) ? fabs((ss - oss)/ss) : ts);
 
			// If decreasing, multiply the two most recent convergence measures
			tvv = ((tv < otv) ? tv*otv : 1.0);
			tss = ((ts < ots) ? ts*ots : 1.0);
 
			// Compare with convergence criteria
			vpass = ((tvv < betav) ? 1 : 0);
			spass = ((tss < betas) ? 1 : 0);
 
			if ((spass) || (vpass))
			{
				// At least one sequence has passed the convergence test.
				// Store variables before iterating
				for (i = 0; i < N; i++) svk[i] = K[i];
 
				s = ss;
 
				// Choose iteration according to the fastest converging sequence
				stry = vtry = 0;
 
				for ( ; ; ) 
				{
					if ((fflag && ((fflag = 0) == 0)) && ((spass) && (!vpass || (tss < tvv))))
					{
						; // Do nothing. Provides a quick "short circuit".
					}
					else 
					{ 
						// else !fflag 
						QuadIT(N, NZ, ui, vi, szr, szi, lzr, lzi, qp, NN, &a, &b, p, qk, &a1, &a3, &a7, &c, &d, &e, &f, &g, &h, K);
 
						if ((*NZ) > 0)   return;
 
						// Quadratic iteration has failed. Flag that it has been tried and decrease the
						// convergence criterion
						iFlag = vtry = 1;
						betav *= 0.25;
 
						// Try linear iteration if it has not been tried and the s sequence is converging
						if (stry || (!spass))
						{
							iFlag = 0;
						}
						else 
						{
							for (i = 0; i < N; i++) K[i] = svk[i];
						}
					}
 
					if (iFlag != 0)
					{
						RealIT(&iFlag, NZ, &s, N, p, NN, qp, szr, szi, K, qk);
 
						if ((*NZ) > 0) return;
 
						// Linear iteration has failed. Flag that it has been tried and decrease the
						// convergence criterion
						stry = 1;
						betas *= 0.25;
 
						if (iFlag != 0)
						{
							// If linear iteration signals an almost double real zero, attempt quadratic iteration
							ui = -(s + s);
							vi = s*s;
							continue;
						}
					}
 
					// Restore variables
					for (i = 0; i < N; i++) K[i] = svk[i];
 
					// Try quadratic iteration if it has not been tried and the v sequence is converging
					if (!vpass || vtry) break;
				}
 
				// Re-compute qp and scalar values to continue the second stage
				QuadSD(NN, u, v, p, qp, &a, &b);
				tFlag = CalcSC(N, a, b, &a1, &a3, &a7, &c, &d, &e, &f, &g, &h, K, u, v, qk);
			}
		}
 
		ovv = vv;
		oss = ss;
		otv = tv;
		ots = ts;
	}
 
	return;
}

///	<summary>
///	Divides p by the quadratic 1, u, v placing the quotient in q and the remainder in a, b.
///	</summary>
void Nequeo::Math::PolynomialJenkinsTraub::QuadSD(int NN, double u, double v, double p[MDP1], double q[MDP1], double* a, double* b)
{
	int i;
 
	q[0] = *b = p[0];
	q[1] = *a = -((*b)*u) + p[1];
 
	for (i = 2; i < NN; i++)
	{
		q[i] = -((*a)*u + (*b)*v) + p[i];
		*b = (*a);
		*a = q[i];
	}
 
	return;
}

///	<summary>
///	This routine calculates scalar quantities used to compute the next K polynomial and
/// new estimates of the quadratic coefficients.
/// calcSC - integer variable set here indicating how the calculations are normalized
/// to avoid overflow.
///	</summary>
int Nequeo::Math::PolynomialJenkinsTraub::CalcSC(int N, double a, double b, double* a1, double* a3, double* a7, double* c, double* d, double* e, double* f, double* g, double* h, double K[MDP1], double u, double v, double qk[MDP1])
{
	// TYPE = 3 indicates the quadratic is almost a factor of K
	int dumFlag = 3; 
 
	// Synthetic division of K by the quadratic 1, u, v
	QuadSD(N, u, v, K, qk, c, d);
 
	if (fabs((*c)) <= (100.0*DBL_EPSILON*fabs(K[N - 1]))) 
	{
		if (fabs((*d)) <= (100.0*DBL_EPSILON*fabs(K[N - 2]))) return dumFlag;
	}
 
	*h = v*b;
	if (fabs((*d)) >= fabs((*c)))
	{
		dumFlag = 2; // TYPE = 2 indicates that all formulas are divided by d
		*e = a/(*d);
		*f = (*c)/(*d);
		*g = u*b;
		*a3 = (*e)*((*g) + a) + (*h)*(b/(*d));
		*a1 = -a + (*f)*b;
		*a7 = (*h) + ((*f) + u)*a;
	} 
	else 
	{
		dumFlag = 1; // TYPE = 1 indicates that all formulas are divided by c;
		*e = a/(*c);
		*f = (*d)/(*c);
		*g = (*e)*u;
		*a3 = (*e)*a + ((*g) + (*h)/(*c))*b;
		*a1 = -(a*((*d)/(*c))) + b;
		*a7 = (*g)*(*d) + (*h)*(*f) + a;
	}
 
	return dumFlag;
}
 
///	<summary>
///	Computes the next K polynomials using the scalars computed in calcSC.
///	</summary>
void Nequeo::Math::PolynomialJenkinsTraub::NextK(int N, int tFlag, double a, double b, double a1, double* a3, double* a7, double K[MDP1], double qk[MDP1], double qp[MDP1])
{
	int i;
	double temp;
 
	if (tFlag == 3)
	{ 
		// Use unscaled form of the recurrence
		K[1] = K[0] = 0.0;
 
		for (i = 2; i < N; i++) K[i] = qk[i - 2];
 
		return;
	}
 
	temp = ((tFlag == 1) ? b : a);
 
	if (fabs(a1) > (10.0*DBL_EPSILON*fabs(temp)))
	{
		// Use scaled form of the recurrence
 
		(*a7) /= a1;
		(*a3) /= a1;
		K[0] = qp[0];
		K[1] = -((*a7)*qp[0]) + qp[1];
 
		for (i = 2; i < N; i++) K[i] = -((*a7)*qp[i - 1]) + (*a3)*qk[i - 2] + qp[i];
 
	}
	else 
	{
		// If a1 is nearly zero, then use a special form of the recurrence
 
		K[0] = 0.0;
		K[1] = -(*a7)*qp[0];
 
		for (i = 2; i < N; i++) K[i] = -((*a7)*qp[i - 1]) + (*a3)*qk[i - 2];
	}
 
	return;
}
 
///	<summary>
///	Compute new estimates of the quadratic coefficients using the scalars computed in calcSC.
///	</summary>
void Nequeo::Math::PolynomialJenkinsTraub::Newest(int tFlag, double* uu, double* vv, double a, double a1, double a3, double a7, double b, double c, double d, double f, double g, double h, double u, double v, double K[MDP1], int N, double p[MDP1])
{
	double a4, a5, b1, b2, c1, c2, c3, c4, temp;
 
	(*vv) = (*uu) = 0.0; // The quadratic is zeroed
 
	if (tFlag != 3)
	{
		if (tFlag != 2)
		{
			a4 = a + u*b + h*f;
			a5 = c + (u + v*f)*d;
		}
		else 
		{
			a4 = (a + g)*f + h;
			a5 = (f + u)*c + v*d;
		}
 
		// Evaluate new quadratic coefficients
		b1 = -K[N - 1]/p[N];
		b2 = -(K[N - 2] + b1*p[N - 1])/p[N];
		c1 = v*b2*a1;
		c2 = b1*a7;
		c3 = b1*b1*a3;
		c4 = -(c2 + c3) + c1;
		temp = -c4 + a5 + b1*a4;

		if (temp != 0.0) 
		{
			*uu= -((u*(c3 + c2) + v*(b1*a1 + b2*a7))/temp) + u;
			*vv = v*(1.0 + c4/temp);
		}
	}
 
	return;
}
 
///	<summary>
///	Variable-shift K-polynomial iteration for a quadratic factor converges only if the
/// zeros are equimodular or nearly so.
///	</summary>
void Nequeo::Math::PolynomialJenkinsTraub::QuadIT(int N, int* NZ, double uu, double vv, double* szr, double* szi, double* lzr, double* lzi, double qp[MDP1], int NN, double* a, double* b, double p[MDP1], double qk[MDP1], double* a1, double* a3, double* a7, double* c, double* d, double* e, double* f, double* g, double* h, double K[MDP1])
{
	int i, j = 0, tFlag, triedFlag = 0;
	double ee, mp, omp, relstp, t, u, ui, v, vi, zm;
 
	*NZ = 0; // Number of zeros found
	u = uu; // uu and vv are coefficients of the starting quadratic
	v = vv;
 
	do 
	{
		Quad(1.0, u, v, szr, szi, lzr, lzi);
 
		// Return if roots of the quadratic are real and not close to multiple or nearly
		// equal and of opposite sign.
		if (fabs(fabs(*szr) - fabs(*lzr)) > 0.01*fabs(*lzr)) break;
 
		// Evaluate polynomial by quadratic synthetic division
		QuadSD(NN, u, v, p, qp, a, b);
 
		mp = fabs(-((*szr)*(*b)) + (*a)) + fabs((*szi)*(*b));
 
		// Compute a rigorous bound on the rounding error in evaluating p
		zm = sqrt(fabs(v));
		ee = 2.0*fabs(qp[0]);
		t = -((*szr)*(*b));
 
		for (i = 1; i < N; i++) ee = ee*zm + fabs(qp[i]);
 
		ee = ee*zm + fabs((*a) + t);
		ee = (9.0*ee + 2.0*fabs(t) - 7.0*(fabs((*a) + t) + zm*fabs((*b))))*DBL_EPSILON;
 
		// Iteration has converged sufficiently if the polynomial value is less than 20 times this bound
		if (mp <= 20.0*ee)
		{
			*NZ = 2;
			break;
		}
 
		j++;
 
		// Stop iteration after 20 steps
		if (j > 20) break;
 
		if (j >= 2)
		{
			if ((relstp <= 0.01) && (mp >= omp) && (!triedFlag))
			{
				// A cluster appears to be stalling the convergence. Five fixed shift
				// steps are taken with a u, v close to the cluster.
				relstp = ((relstp < DBL_EPSILON) ? sqrt(DBL_EPSILON) : sqrt(relstp));
 
				u -= u*relstp;
				v += v*relstp;
 
				QuadSD(NN, u, v, p, qp, a, b);
 
				for (i = 0; i < 5; i++)
				{
					tFlag = CalcSC(N, *a, *b, a1, a3, a7, c, d, e, f, g, h, K, u, v, qk);
					NextK(N, tFlag, *a, *b, *a1, a3, a7, K, qk, qp);
				}
 
				triedFlag = 1;
				j = 0;
			}
		}
 
		omp = mp;
 
		// Calculate next K polynomial and new u and v
		tFlag = CalcSC(N, *a, *b, a1, a3, a7, c, d, e, f, g, h, K, u, v, qk);
		NextK(N, tFlag, *a, *b, *a1, a3, a7, K, qk, qp);
		tFlag = CalcSC(N, *a, *b, a1, a3, a7, c, d, e, f, g, h, K, u, v, qk);
		Newest(tFlag, &ui, &vi, *a, *a1, *a3, *a7, *b, *c, *d, *f, *g, *h, u, v, K, N, p);

		// If vi is zero, the iteration is not converging
		if (vi != 0)
		{
			relstp = fabs((-v + vi)/vi);
			u = ui;
			v = vi;
		}
	} while (vi != 0);
 
	return;
}
 
///	<summary>
///	Variable-shift H-polynomial iteration for a real zero
/// sss - starting iterate
/// NZ - number of zeros found
/// iFlag - flag to indicate a pair of zeros near real axis
///	</summary>
void Nequeo::Math::PolynomialJenkinsTraub::RealIT(int* iFlag, int* NZ, double* sss, int N, double p[MDP1], int NN, double qp[MDP1], double* szr, double* szi, double K[MDP1], double qk[MDP1])
{
	int i, j = 0, nm1 = N - 1;
	double ee, kv, mp, ms, omp, pv, s, t;
 
	*iFlag = *NZ = 0;
	s = *sss;
 
	for ( ; ; ) 
	{
		pv = p[0];
 
		// Evaluate p at s
		qp[0] = pv;
		for (i = 1; i < NN; i++)   qp[i] = pv = pv*s + p[i];
 
		mp = fabs(pv);
 
		// Compute a rigorous bound on the error in evaluating p
		ms = fabs(s);
		ee = 0.5*fabs(qp[0]);
		for (i = 1; i < NN; i++) ee = ee*ms + fabs(qp[i]);
 
		// Iteration has converged sufficiently if the polynomial value is less than
		// 20 times this bound
		if (mp <= 20.0*DBL_EPSILON*(2.0*ee - mp))
		{
			*NZ = 1;
			*szr = s;
			*szi = 0.0;
			break;
		}
 
		j++;
 
		// Stop iteration after 10 steps
		if (j > 10) break;
 
		if (j >= 2)
		{
			if ((fabs(t) <= 0.001*fabs(-t + s)) && (mp > omp))
			{
				// A cluster of zeros near the real axis has been encountered;
				// Return with iFlag set to initiate a quadratic iteration
 
				*iFlag = 1;
				*sss = s;
				break;
			}
		}
 
		// Return if the polynomial value has increased significantly
		omp = mp;
 
		// Compute t, the next polynomial and the new iterate
		qk[0] = kv = K[0];
		for (i = 1; i < N; i++) qk[i] = kv = kv*s + K[i];
 
		if (fabs(kv) > fabs(K[nm1])*10.0*DBL_EPSILON)
		{
			// Use the scaled form of the recurrence if the value of K at s is non-zero
			t = -(pv/kv);
			K[0] = qp[0];
			for (i = 1; i < N; i++) K[i] = t*qk[i - 1] + qp[i];
		}
		else 
		{
			// Use unscaled form
			K[0] = 0.0;
			for (i = 1; i < N; i++) K[i] = qk[i - 1];
		}
 
		kv = K[0];
		for (i = 1; i < N; i++) kv = kv*s + K[i];
 
		t = ((fabs(kv) > (fabs(K[nm1])*10.0*DBL_EPSILON)) ? -(pv/kv) : 0.0);
 
		s += t;
	}
 
	return;
}
 
///	<summary>
///	Calculates the zeros of the quadratic a*Z^2 + b1*Z + c
/// The quadratic formula, modified to avoid overflow, is used to find the larger zero if the
/// zeros are real and both zeros are complex. The smaller real zero is found directly from
/// the product of the zeros c/a.
///	</summary>
void Nequeo::Math::PolynomialJenkinsTraub::Quad(double a, double b1, double c, double* sr, double* si, double* lr, double* li) 
{
	double b, d, e;
 
	*sr = *si = *lr = *li = 0.0;
 
	if (a == 0) 
	{
		*sr = ((b1 != 0) ? -(c/b1) : *sr);
		return;
	}
 
	if (c == 0)
	{
		*lr = -(b1/a);
		return;
	}
 
	// Compute discriminant avoiding overflow
	b = b1/2.0;
	if (fabs(b) < fabs(c))
	{
		e = ((c >= 0) ? a : -a);
		e = -e + b*(b/fabs(c));
		d = sqrt(fabs(e))*sqrt(fabs(c));
	}
	else 
	{
		e = -((a/b)*(c/b)) + 1.0;
		d = sqrt(fabs(e))*(fabs(b));
	}

	if (e >= 0) 
	{
		// Real zeros
		d = ((b >= 0) ? -d : d);
		*lr = (-b + d)/a;
		*sr = ((*lr != 0) ? (c/(*lr))/a : *sr);
	}
	else 
	{
		// Complex conjugate zeros
		*lr = *sr = -(b/a);
		*si = fabs(d/a);
		*li = -(*si);
	}
 
	return;
 }