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
using System.Threading.Tasks;

using Nequeo.Collections.Concurrent.Partitioners;

namespace Nequeo.Threading.Parallel.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Invokes the specified functions, potentially in parallel, canceling outstanding invocations once one completes.</summary>
        /// <typeparam name="T">Specifies the type of data returned by the functions.</typeparam>
        /// <param name="functions">The functions to be executed.</param>
        /// <returns>A result from executing one of the functions.</returns>
        public static T SpeculativeInvoke<T>(params Func<T>[] functions)
        {
            // Run with default options
            return SpeculativeInvoke(s_defaultParallelOptions, functions);
        }

        /// <summary>Invokes the specified functions, potentially in parallel, canceling outstanding invocations once one completes.</summary>
        /// <typeparam name="T">Specifies the type of data returned by the functions.</typeparam>
        /// <param name="options">The options to use for the execution.</param>
        /// <param name="functions">The functions to be executed.</param>
        /// <returns>A result from executing one of the functions.</returns>
        public static T SpeculativeInvoke<T>(ParallelOptions options, params Func<T>[] functions)
        {
            // Validate parameters
            if (options == null) throw new ArgumentNullException("options");
            if (functions == null) throw new ArgumentNullException("functions");

            // Speculatively invoke each function
            return ParallelAlgorithms.SpeculativeForEach(functions, options, function => function());
        }
    }
}