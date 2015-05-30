#include "GaussianElimination.h"

using namespace std;

///	<summary>
///	Construct the Gaussian Elimination.
///	</summary>
Nequeo::Math::Matrix::GaussianElimination::GaussianElimination() : m_disposed(false)
{
}

///	<summary>
///	Deconstruct the Gaussian Elimination.
///	</summary>
Nequeo::Math::Matrix::GaussianElimination::~GaussianElimination()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

///	<summary>
///	Solve the NxN matrix using Guassian reduction.
///	</summary>
/// <param name='aMatrix'>The collection of A values.</param>
/// <param name='bMatrix'>The collection of B values.</param>
/// <param name='xMatrix'>The collection of X values (will contain the result when complete).</param>
/// <param name='nxnMatrixSize'>The size of the NxN matrix.</param>
void Nequeo::Math::Matrix::GaussianElimination::Solve(double** aMatrix, double* bMatrix, double* xMatrix, int nxnMatrixSize)
{
	double mult_matrix, sum_matrix;

	// Elimination of lower triangular part.
	for(int i = 0; i < nxnMatrixSize-1; i++)
	{
		for(int k = i + 1; k < nxnMatrixSize; k++)
		{
			mult_matrix = aMatrix[k][i] / aMatrix[i][i];
			for(int j = 0; j < nxnMatrixSize; j++)
			{
				aMatrix[k][j] -= mult_matrix * aMatrix[i][j];
			}
			bMatrix[k] -= mult_matrix * bMatrix[i];
		}
	}

	// Back substitution.
	for(int i = nxnMatrixSize - 1; i >= 0; i--)
	{
		sum_matrix = 0.0;
		for(int j = i + 1; j < nxnMatrixSize; j++)
		{
			sum_matrix += aMatrix[i][j] * xMatrix[j];
		}
		xMatrix[i] = (bMatrix[i] - sum_matrix) / aMatrix[i][i];
	}
}