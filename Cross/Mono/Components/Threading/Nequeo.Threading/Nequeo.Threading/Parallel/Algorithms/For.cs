/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
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
#endregion

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Nequeo.Threading.Parallel.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Executes a for loop in which iterations may run in parallel.</summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        public static void For(BigInteger fromInclusive, BigInteger toExclusive, Action<BigInteger> body)
        {
            For(fromInclusive, toExclusive, s_defaultParallelOptions, body);
        }

        /// <summary>Executes a for loop in which iterations may run in parallel.</summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="options">A System.Threading.Tasks.ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        public static void For(BigInteger fromInclusive, BigInteger toExclusive, ParallelOptions options, Action<BigInteger> body)
        {
            // Determine how many iterations to run...
            var range = toExclusive - fromInclusive;

            // ... and run them.
            if (range <= 0)
            {
                // If there's nothing to do, bail
                return;
            }
                // Fast path
            else if (range <= Int64.MaxValue)
            {
                // If the range is within the realm of Int64, we'll delegate to Parallel.For's Int64 overloads.
                // Iterate from 0 to range, and then call the user-provided body with the scaled-back value.
                System.Threading.Tasks.Parallel.For(0, (long)range, options, i => body(i + fromInclusive));
            }
                // Slower path
            else
            {
                // For a range larger than Int64.MaxValue, we'll rely on an enumerable of BigInteger.
                // We create a C# iterator that yields all of the BigInteger values in the requested range
                // and then ForEach over that range.
                System.Threading.Tasks.Parallel.ForEach(Range(fromInclusive, toExclusive), options, body);
            }
        }

        /// <summary>Creates an enumerable that iterates the range [fromInclusive, toExclusive).</summary>
        /// <param name="fromInclusive">The lower bound, inclusive.</param>
        /// <param name="toExclusive">The upper bound, exclusive.</param>
        /// <returns>The enumerable of the range.</returns>
        private static IEnumerable<BigInteger> Range(BigInteger fromInclusive, BigInteger toExclusive)
        {
            for (var i = fromInclusive; i < toExclusive; i++) yield return i;
        }
    }
}