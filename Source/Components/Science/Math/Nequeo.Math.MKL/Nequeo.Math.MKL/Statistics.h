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

#pragma once

#ifndef _STATISTICS_H
#define _STATISTICS_H

#include "stdafx.h"

namespace Nequeo
{
	namespace Math
	{
		namespace MKL
		{
			///	<summary>
			///	Statistical analysis.
			///	</summary>
			class EXPORT_NEQUEO_MKL_API Statistics
			{
			public:
				///	<summary>
				///	Statistical analysis.
				///	</summary>
				Statistics();

				///	<summary>
				///	Statistical analysis destructor.
				///	</summary>
				~Statistics();

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
				void Sample(std::vector<double> samples, double *mean, double *variation, double *skewness, double *kurtosis, double *covariance, double *correlation);

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
				void Sample(int number, double samples[], double *mean, double *variation, double *skewness, double *kurtosis, double *covariance, double *correlation);

			private:
				bool _disposed;

			};
		}
	}
}
#endif