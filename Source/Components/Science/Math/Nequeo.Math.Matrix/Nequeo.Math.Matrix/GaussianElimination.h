#pragma once

#ifndef _GAUSSIANELIMINATION_H
#define _GAUSSIANELIMINATION_H

#include "Stdafx.h"

namespace Nequeo {namespace Math {namespace Matrix
{
	///	<summary>
	///	Calculate the NxN matrix (AX = B, solving for X) 
	/// using Guassian reduction (Gaussian elimination).
	///	</summary>
    class GaussianElimination
    {
		public: 
			// Constructors
			GaussianElimination();
			~GaussianElimination();

			void Solve(double** aMatrix, double* bMatrix, double* xMatrix, int nxnMatrixSize);

		private:

			// Fields
			bool m_disposed;
			
    };
}}}

#endif // _GAUSSIANELIMINATION_H