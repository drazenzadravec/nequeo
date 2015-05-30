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
using System.Threading.Tasks;

namespace Nequeo.Threading.Parallel.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Executes a map operation, converting an input list into an output list, in parallel.</summary>
        /// <typeparam name="TInput">Specifies the type of the input data.</typeparam>
        /// <typeparam name="TOutput">Specifies the type of the output data.</typeparam>
        /// <param name="input">The input list to be mapped used the transform function.</param>
        /// <param name="transform">The transform function to use to map the input data to the output data.</param>
        /// <returns>The output data, transformed using the transform function.</returns>
        public static TOutput[] Map<TInput, TOutput>(IList<TInput> input, Func<TInput, TOutput> transform)
        {
            return Map(input, s_defaultParallelOptions, transform);
        }

        /// <summary>Executes a map operation, converting an input list into an output list, in parallel.</summary>
        /// <typeparam name="TInput">Specifies the type of the input data.</typeparam>
        /// <typeparam name="TOutput">Specifies the type of the output data.</typeparam>
        /// <param name="input">The input list to be mapped used the transform function.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="transform">The transform function to use to map the input data to the output data.</param>
        /// <returns>The output data, transformed using the transform function.</returns>
        public static TOutput[] Map<TInput, TOutput>(IList<TInput> input, ParallelOptions parallelOptions, Func<TInput, TOutput> transform)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
            if (transform == null) throw new ArgumentNullException("transform");

            var output = new TOutput[input.Count];
            System.Threading.Tasks.Parallel.For(0, input.Count, parallelOptions, i => output[i] = transform(input[i]));
            return output;
        }
    }
}