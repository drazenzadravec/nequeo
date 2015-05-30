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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nequeo.Threading.Parallel.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Processes data in parallel, allowing the processing function to add more data to be processed.</summary>
        /// <typeparam name="T">Specifies the type of data being processed.</typeparam>
        /// <param name="initialValues">The initial set of data to be processed.</param>
        /// <param name="body">The operation to execute for each value.</param>
        public static void WhileNotEmpty<T>(IEnumerable<T> initialValues, Action<T, Action<T>> body)
        {
            WhileNotEmpty(s_defaultParallelOptions, initialValues, body);
        }

        /// <summary>Processes data in parallel, allowing the processing function to add more data to be processed.</summary>
        /// <typeparam name="T">Specifies the type of data being processed.</typeparam>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="initialValues">The initial set of data to be processed.</param>
        /// <param name="body">The operation to execute for each value.</param>
        public static void WhileNotEmpty<T>(
            ParallelOptions parallelOptions,
            IEnumerable<T> initialValues,
            Action<T, Action<T>> body)
        {
            // Validate arguments
            if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
            if (initialValues == null) throw new ArgumentNullException("initialValues");
            if (body == null) throw new ArgumentNullException("body");

            // Create two lists to alternate between as source and destination.
            var lists = new[] { new ConcurrentStack<T>(initialValues), new ConcurrentStack<T>() };

            // Iterate until no more items to be processed
            for (int i = 0; ; i++)
            {
                // Determine which list is the source and which is the destination
                int fromIndex = i % 2;
                var from = lists[fromIndex];
                var to = lists[fromIndex ^ 1];

                // If the source is empty, we're done
                if (from.IsEmpty) break;

                // Otherwise, process all source items in parallel, adding any new items into the destination
                Action<T> adder = newItem => to.Push(newItem);
                System.Threading.Tasks.Parallel.ForEach(from, parallelOptions, e => body(e, adder));

                // Clear out the source as it's now been fully processed
                from.Clear();
            }
        }
    }
}