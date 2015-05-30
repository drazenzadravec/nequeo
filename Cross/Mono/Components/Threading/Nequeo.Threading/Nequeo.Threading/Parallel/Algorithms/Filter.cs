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
        /// <summary>Filters an input list, running a predicate over each element of the input.</summary>
        /// <typeparam name="T">Specifies the type of data in the list.</typeparam>
        /// <param name="input">The list to be filtered.</param>
        /// <param name="predicate">The predicate to use to determine which elements pass.</param>
        /// <returns>A new list containing all those elements from the input that passed the filter.</returns>
        public static IList<T> Filter<T>(IList<T> input, Func<T, Boolean> predicate)
        {
            return Filter(input, s_defaultParallelOptions, predicate);
        }

        /// <summary>Filters an input list, running a predicate over each element of the input.</summary>
        /// <typeparam name="T">Specifies the type of data in the list.</typeparam>
        /// <param name="input">The list to be filtered.</param>
        /// <param name="parallelOptions">Options to use for the execution of this filter.</param>
        /// <param name="predicate">The predicate to use to determine which elements pass.</param>
        /// <returns>A new list containing all those elements from the input that passed the filter.</returns>
        public static IList<T> Filter<T>(IList<T> input, ParallelOptions parallelOptions, Func<T, Boolean> predicate)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
            if (predicate == null) throw new ArgumentNullException("predicate");

            var results = new List<T>(input.Count);
            System.Threading.Tasks.Parallel.For(0, input.Count, parallelOptions, () => new List<T>(input.Count), (i, loop, localList) =>
            {
                var item = input[i];
                if (predicate(item)) localList.Add(item);
                return localList;
            },
            localList => { lock (results) results.AddRange(localList); });
            return results;
        }
    }
}