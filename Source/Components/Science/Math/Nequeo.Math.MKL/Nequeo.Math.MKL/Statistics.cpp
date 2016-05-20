/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Statistics.h
*  Purpose :       Statistics class.
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

#include "mkl.h"

#include "Statistics.h"

using namespace Nequeo::Math::MKL;

///	<summary>
///	Statistical analysis.
///	</summary>
Statistics::Statistics() : _disposed(false)
{
}

///	<summary>
///	Statistical analysis destructor.
///	</summary>
Statistics::~Statistics()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Calculate the sample statistics.
///	</summary>
/// <param name="samples">The sample data.</param>
/// <param name="mean">The calculated mean.</param>
/// <param name="variation">The calculated variation.</param>
/// <param name="skewness">The calculated skewness.</param>
/// <param name="kurtosis">The calculated kurtosis.</param>
/// <param name="covariance">The calculated covariance.</param>
/// <param name="correlation">The calculated correlation.</param>
void Statistics::Sample(std::vector<double> samples, double *mean, double *variation, double *skewness, double *kurtosis, double *covariance, double *correlation)
{
	// Get the vector size.
	size_t vectorSize = samples.size();
	double *samplesArray = new double[vectorSize];

	// If samples exist.
	if (vectorSize > 0)
	{
		// For each sample item.
		for (size_t i = 0; i < vectorSize; i++)
		{
			// Assign the values.
			samplesArray[i] = samples[i];
		}

		// Calculate the sample statistics.
		Sample((int)vectorSize, samplesArray, mean, variation, skewness, kurtosis, covariance, correlation);

		// Delete the sample array.
		delete[] samplesArray;
	}
}

///	<summary>
///	Calculate the sample statistics.
///	</summary>
/// <param name="number">The number of samples in the array.</param>
/// <param name="samples">The sample data.</param>
/// <param name="mean">The calculated mean.</param>
/// <param name="variation">The calculated variation.</param>
/// <param name="skewness">The calculated skewness.</param>
/// <param name="kurtosis">The calculated kurtosis.</param>
/// <param name="covariance">The calculated covariance.</param>
/// <param name="correlation">The calculated correlation.</param>
void Statistics::Sample(int number, double samples[], double *mean, double *variation, double *skewness, double *kurtosis, double *covariance, double *correlation)
{
	VSLSSTaskPtr task;

	unsigned MKL_INT64 estimate = 0;
	MKL_INT x_storage;
	MKL_INT dim;
	MKL_INT n;
	MKL_INT cov_storage;
	MKL_INT cor_storage;

	int errcode;

	double raw2[1], raw3[1], raw4[1];
	double cen2[1], cen3[1], cen4[1];

	// Assign the values.
	dim = 1;
	n = number;
	x_storage = VSL_SS_MATRIX_STORAGE_ROWS;
	cov_storage = VSL_SS_MATRIX_STORAGE_FULL;
	cor_storage = VSL_SS_MATRIX_STORAGE_FULL;

	// Create Summary Statistics task.
	errcode = vsldSSNewTask(&task, &dim, &n, &x_storage, samples, 0, 0);

	// Edit task parameters for the computed stats.
	errcode = vsldSSEditTask(task, VSL_SS_ED_MEAN, mean);
	errcode = vsldSSEditTask(task, VSL_SS_ED_VARIATION, variation);
	errcode = vsldSSEditTask(task, VSL_SS_ED_SKEWNESS, skewness);
	errcode = vsldSSEditTask(task, VSL_SS_ED_KURTOSIS, kurtosis);
	
	// Edit task parameters for computating of mean estimate and 2nd, 3rd
	// and 4th raw and central moments estimates.
	errcode = vsldSSEditMoments(task, mean, raw2, raw3, raw4, cen2, cen3, cen4);

	// Initialization of the task parameters using FULL_STORAGE
	// for covariance/correlation matrices computation.
	errcode = vsldSSEditCovCor(task, mean, covariance, &cov_storage, correlation, &cor_storage);

	// Compute the mean.
	estimate |= VSL_SS_MEAN | VSL_SS_2R_MOM | VSL_SS_3R_MOM | VSL_SS_4R_MOM |
		VSL_SS_2C_MOM | VSL_SS_3C_MOM | VSL_SS_4C_MOM;

	// Kurtosis, skewness and variation are included in the list
	// of estimates to compute.
	estimate |= VSL_SS_KURTOSIS | VSL_SS_SKEWNESS | VSL_SS_VARIATION;

	// Covariance and correlation matrices are included in the list
	// of estimates to compute.
	estimate |= VSL_SS_COV | VSL_SS_COR;

	// Compute the statistical valies.
	errcode = vsldSSCompute(task, estimate, VSL_SS_METHOD_FAST);

	// Delete the task.
	errcode = vslSSDeleteTask(&task);
	
	// Clear the buffers.
	MKL_Free_Buffers();
}