
#include "SystemLinearEquations.h"
#include "GaussianElimination.h"

using namespace std;

///	<summary>
///	Construct the System Linear Equations.
///	</summary>
Nequeo::Math::Matrix::SystemLinearEquations::SystemLinearEquations() : m_disposed(false)
{
}

///	<summary>
///	Deconstruct the System Linear Equations.
///	</summary>
Nequeo::Math::Matrix::SystemLinearEquations::~SystemLinearEquations()
{
	// If not disposed.
    if (!m_disposed)
    {
        m_disposed = true;
    }
}

///	<summary>
///	Solve the System of Linear Equations.
///	</summary>
/// <param name='aMatrix'>The collection of A values.</param>
/// <param name='bMatrix'>The collection of B values.</param>
/// <param name='xMatrix'>The collection of X values (will contain the result when complete).</param>
/// <param name='nxnMatrixSize'>The size of the NxN matrix.</param>
void Nequeo::Math::Matrix::SystemLinearEquations::Solve(double** aMatrix, double* bMatrix, double* xMatrix, int nxnMatrixSize)
{
	// Create a new instance on the heap, through the smart pointer.
	// When out of scope then the resource 'GaussianElimination' is
	// released from the the heap, no need to delete from the heap.
	unique_ptr<GaussianElimination> gaussElim(new GaussianElimination());

	// Solve the matrix.
	gaussElim->Solve(aMatrix, bMatrix, xMatrix, nxnMatrixSize);
}