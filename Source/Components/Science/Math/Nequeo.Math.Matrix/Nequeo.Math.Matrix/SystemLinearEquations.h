#pragma once

#ifndef _SYSTEMLINEAREQUATIONS_H
#define _SYSTEMLINEAREQUATIONS_H

#include "Stdafx.h"

namespace Nequeo {namespace Math {namespace Matrix
{
	///	<summary>
	///	Calculate the System of Linear Equations, NxN matrix (AX = B, solving for X) 
	/// using Guassian reduction (Gaussian elimination).
	///	</summary>
    class SystemLinearEquations
    {
		public: 
			// Constructors
			SystemLinearEquations();
			~SystemLinearEquations();

			void Solve(double** aMatrix, double* bMatrix, double* xMatrix, int nxnMatrixSize);

		private:

			// Fields
			bool m_disposed;
			
    };
}}}

#endif // _SYSTEMLINEAREQUATIONS_H