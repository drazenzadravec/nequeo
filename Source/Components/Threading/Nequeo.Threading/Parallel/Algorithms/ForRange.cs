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
using System.Threading.Tasks;

namespace Nequeo.Threading.Parallel.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        #region Int32, No Options
        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            int fromInclusive, int toExclusive, 
            Action<int, int> body)
        {
            return ForRange(fromInclusive, toExclusive, s_defaultParallelOptions, body);
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            int fromInclusive, int toExclusive,
            Action<int, int, ParallelLoopState> body)
        {
            return ForRange(fromInclusive, toExclusive, s_defaultParallelOptions, body);
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="localInit">The function delegate that returns the initial state of the local data for each thread.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <param name="localFinally">The delegate that performs a final action on the local state of each thread.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange<TLocal>(
            int fromInclusive, int toExclusive,
            Func<TLocal> localInit,
            Func<int, int, ParallelLoopState, TLocal, TLocal> body,
            Action<TLocal> localFinally)
        {
            return ForRange(fromInclusive, toExclusive, s_defaultParallelOptions,
                localInit, body, localFinally);
        }
        #endregion

        #region Int64, No Options
        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            long fromInclusive, long toExclusive,
            Action<long, long> body)
        {
            return ForRange(fromInclusive, toExclusive, s_defaultParallelOptions, body);
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            long fromInclusive, long toExclusive,
            Action<long, long, ParallelLoopState> body)
        {
            return ForRange(fromInclusive, toExclusive, s_defaultParallelOptions, body);
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="localInit">The function delegate that returns the initial state of the local data for each thread.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <param name="localFinally">The delegate that performs a final action on the local state of each thread.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange<TLocal>(
            long fromInclusive, long toExclusive,
            Func<TLocal> localInit,
            Func<long, long, ParallelLoopState, TLocal, TLocal> body,
            Action<TLocal> localFinally)
        {
            return ForRange(fromInclusive, toExclusive, s_defaultParallelOptions,
                localInit, body, localFinally);
        }
        #endregion

        #region Int32, Parallel Options
        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            int fromInclusive, int toExclusive,
            ParallelOptions parallelOptions,
            Action<int, int> body)
        {
            if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
            if (body == null) throw new ArgumentNullException("body");

            return System.Threading.Tasks.Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, range =>
            {
                body(range.Item1, range.Item2);
            });
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            int fromInclusive, int toExclusive,
            ParallelOptions parallelOptions,
            Action<int, int, ParallelLoopState> body)
        {
            if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
            if (body == null) throw new ArgumentNullException("body");

            return System.Threading.Tasks.Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, (range, loopState) =>
            {
                body(range.Item1, range.Item2, loopState);
            });
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">The parallel option to apply.</param>
        /// <param name="localInit">The function delegate that returns the initial state of the local data for each thread.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <param name="localFinally">The delegate that performs a final action on the local state of each thread.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange<TLocal>(
            int fromInclusive, int toExclusive,
            ParallelOptions parallelOptions,
            Func<TLocal> localInit,
            Func<int, int, ParallelLoopState, TLocal, TLocal> body,
            Action<TLocal> localFinally)
        {
            if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
            if (localInit == null) throw new ArgumentNullException("localInit");
            if (body == null) throw new ArgumentNullException("body");
            if (localFinally == null) throw new ArgumentNullException("localFinally");

            return System.Threading.Tasks.Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, localInit, (range, loopState, x) =>
            {
                return body(range.Item1, range.Item2, loopState, x);
            }, localFinally);
        }
        #endregion

        #region Int64, Parallel Options
        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            long fromInclusive, long toExclusive,
            ParallelOptions parallelOptions,
            Action<long, long> body)
        {
            if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
            if (body == null) throw new ArgumentNullException("body");

            return System.Threading.Tasks.Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, range =>
            {
                body(range.Item1, range.Item2);
            });
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            long fromInclusive, long toExclusive,
            ParallelOptions parallelOptions,
            Action<long, long, ParallelLoopState> body)
        {
            if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
            if (body == null) throw new ArgumentNullException("body");

            return System.Threading.Tasks.Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, (range, loopState) =>
            {
                body(range.Item1, range.Item2, loopState);
            });
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">The parallel option to apply.</param>
        /// <param name="localInit">The function delegate that returns the initial state of the local data for each thread.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <param name="localFinally">The delegate that performs a final action on the local state of each thread.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange<TLocal>(
            long fromInclusive, long toExclusive,
            ParallelOptions parallelOptions,
            Func<TLocal> localInit,
            Func<long, long, ParallelLoopState, TLocal, TLocal> body,
            Action<TLocal> localFinally)
        {
            if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
            if (localInit == null) throw new ArgumentNullException("localInit");
            if (body == null) throw new ArgumentNullException("body");
            if (localFinally == null) throw new ArgumentNullException("localFinally");

            return System.Threading.Tasks.Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, localInit, (range, loopState, x) =>
            {
                return body(range.Item1, range.Item2, loopState, x);
            }, localFinally);
        }
        #endregion
    }
}