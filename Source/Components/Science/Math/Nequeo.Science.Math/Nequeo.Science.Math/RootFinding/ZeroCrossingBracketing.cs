// <copyright file="ZeroCrossingBracketing.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2013 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using Nequeo.Science.Math.Properties;

namespace Nequeo.Science.Math.RootFinding
{
    public static class ZeroCrossingBracketing
    {
        public static IEnumerable<Tuple<double, double>> FindIntervalsWithin(Func<double, double> f, double lowerBound, double upperBound, int parts)
        {
            // TODO: Consider binary-style search instead of linear scan
            double fmin = f(lowerBound);
            double fmax = f(upperBound);

            if (System.Math.Sign(fmin) != System.Math.Sign(fmax))
            {
                yield return new Tuple<double, double>(lowerBound, upperBound);
                yield break;
            }

            double subdiv = (upperBound - lowerBound)/parts;
            double smin = lowerBound;
            int sign = System.Math.Sign(fmin);

            for (int k = 0; k < parts; k++)
            {
                double smax = smin + subdiv;
                double sfmax = f(smax);
                if (double.IsInfinity(sfmax))
                {
                    // expand interval to include pole
                    smin = smax;
                    continue;
                }

                if (System.Math.Sign(sfmax) != sign)
                {
                    yield return new Tuple<double, double>(smin, smax);
                    sign = System.Math.Sign(sfmax);
                }

                smin = smax;
            }
        }

        /// <summary>Detect a range containing at least one root.</summary>
        /// <param name="f">The function to detect roots from.</param>
        /// <param name="lowerBound">Lower value of the range.</param>
        /// <param name="upperBound">Upper value of the range</param>
        /// <param name="factor">The growing factor of research. Usually 1.6.</param>
        /// <param name="maxIterations">Maximum number of iterations. Usually 50.</param>
        /// <returns>True if the bracketing operation succeeded, false otherwise.</returns>
        /// <remarks>This iterative methods stops when two values with opposite signs are found.</remarks>
        public static bool Expand(Func<double, double> f, ref double lowerBound, ref double upperBound, double factor = 1.6, int maxIterations = 50)
        {
            if (lowerBound >= upperBound)
            {
                throw new ArgumentOutOfRangeException("upperBound", string.Format(Resources.ArgumentOutOfRangeGreater, "xmax", "xmin"));
            }

            double fmin = f(lowerBound);
            double fmax = f(upperBound);

            for (int i = 0; i < maxIterations; i++)
            {
                if (System.Math.Sign(fmin) != System.Math.Sign(fmax))
                {
                    return true;
                }

                if (System.Math.Abs(fmin) < System.Math.Abs(fmax))
                {
                    lowerBound += factor*(lowerBound - upperBound);
                    fmin = f(lowerBound);
                }
                else
                {
                    upperBound += factor*(upperBound - lowerBound);
                    fmax = f(upperBound);
                }
            }

            return false;
        }
    }
}
