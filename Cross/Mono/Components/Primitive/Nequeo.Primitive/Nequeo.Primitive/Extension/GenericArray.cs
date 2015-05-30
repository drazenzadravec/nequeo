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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

using Nequeo.Invention;
using Nequeo.Reflection;
using Nequeo.Extension;

namespace Nequeo.Extension
{
    /// <summary>
    /// Class that extends the a generic array type.
    /// </summary>
    public static class GenericArrayExtensions
    {
        /// <summary>
        /// Retrieves a sub-array from the specified <paramref name="source"/>.
        /// A sub-array starts at the specified element position in <paramref name="source"/>.
        /// </summary>
        /// <returns>
        /// An array of T that receives a sub-array, or an empty array of T
        /// if any problems with the parameters.
        /// </returns>
        /// <param name="source">
        /// An array of T from which to retrieve a sub-array.
        /// </param>
        /// <param name="startIndex">
        /// A <see cref="long"/> that represents the zero-based starting position of
        /// a sub-array in <paramref name="source"/>.
        /// </param>
        /// <param name="length">
        /// A <see cref="long"/> that represents the number of elements to retrieve.
        /// </param>
        /// <typeparam name="T">
        /// The type of elements in <paramref name="source"/>.
        /// </typeparam>
        public static T[] SubArray<T>(this T[] source, long startIndex, long length)
        {
            long len;
            if (source == null || (len = source.LongLength) == 0)
                return new T[0];

            if (startIndex < 0 || length <= 0 || startIndex + length > len)
                return new T[0];

            if (startIndex == 0 && length == len)
                return source;

            var subArray = new T[length];
            Array.Copy(source, startIndex, subArray, 0, length);

            return subArray;
        }

        /// <summary>
        /// Reverse the array from the source.
        /// </summary>
        /// <typeparam name="T">The array type.</typeparam>
        /// <param name="source">The source array.</param>
        /// <returns>The source type array.</returns>
        public static T[] Reverse<T>(this T[] source)
        {
            var len = source.Length;
            var reverse = new T[len];

            var end = len - 1;
            for (var i = 0; i <= end; i++)
                reverse[i] = source[end - i];

            return reverse;
        }

        /// <summary>
        /// Combine the array to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static T[] Combine<T>(this T[] source, T[] arrayOne)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");

            // Set the total array size.
            T[] data = new T[source.Length + arrayOne.Length];

            // Execute the async concurrent tasks.
            Task.Factory.ContinueWhenAll(new Task[]
                {
                    SourceArray<T>(data, source, 0),
                    SourceArray<T>(data, arrayOne, (source.Length))
                }, completedTasks =>
                {
                    // Do nothing
                    int failures = completedTasks.Where(t => t.Exception != null).Count();
                }).Wait();

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static T[] CombineParallel<T>(this T[] source, T[] arrayOne)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");

            // Set the total array size.
            T[] data = new T[source.Length + arrayOne.Length];

            // Execute the async concurrent tasks.
            Parallel.For(0, 2, j =>
            {
                switch (j)
                {
                    case 0:
                        SourceArrayParallel<T>(data, source, 0);
                        break;
                    case 1:
                        SourceArrayParallel<T>(data, arrayOne, (source.Length));
                        break;
                }
            });

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel and asynchronously to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Task<T[]> CombineParallelAsync<T>(this T[] source, T[] arrayOne)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");

            // Create the new async operation
            AsyncCombineGenericArray<T> ret = new AsyncCombineGenericArray<T>(source, arrayOne, null, null, null, 2, 0, null, null);

            // Return the async task.
            return Task<T[]>.Factory.FromAsync(ret.BeginLoad(), ret.EndLoad);
        }

        /// <summary>
        /// Combine the array to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static T[] Combine<T>(this T[] source, T[] arrayOne, T[] arrayTwo)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");

            // Set the total array size.
            T[] data = new T[source.Length + arrayOne.Length + arrayTwo.Length];

            // Execute the async concurrent tasks.
            Task.Factory.ContinueWhenAll(new Task[]
                {
                    SourceArray<T>(data, source, 0),
                    SourceArray<T>(data, arrayOne, (source.Length)),
                    SourceArray<T>(data, arrayTwo, (source.Length + arrayOne.Length)),
                }, completedTasks =>
                {
                    // Do nothing
                    int failures = completedTasks.Where(t => t.Exception != null).Count();
                }).Wait();

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static T[] CombineParallel<T>(this T[] source, T[] arrayOne, T[] arrayTwo)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");

            // Set the total array size.
            T[] data = new T[source.Length + arrayOne.Length + arrayTwo.Length];

            // Execute the async concurrent tasks.
            Parallel.For(0, 3, j =>
            {
                switch (j)
                {
                    case 0:
                        SourceArrayParallel<T>(data, source, 0);
                        break;
                    case 1:
                        SourceArrayParallel<T>(data, arrayOne, (source.Length));
                        break;
                    case 2:
                        SourceArrayParallel<T>(data, arrayTwo, (source.Length + arrayOne.Length));
                        break;
                }
            });

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel and asynchronously to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Task<T[]> CombineParallelAsync<T>(this T[] source, T[] arrayOne, T[] arrayTwo)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");

            // Create the new async operation
            AsyncCombineGenericArray<T> ret = new AsyncCombineGenericArray<T>(source, arrayOne, arrayTwo, null, null, 3, 1, null, null);

            // Return the async task.
            return Task<T[]>.Factory.FromAsync(ret.BeginLoad(), ret.EndLoad);
        }

        /// <summary>
        /// Combine the array to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static T[] Combine<T>(this T[] source, T[] arrayOne, T[] arrayTwo, T[] arrayThree)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");

            // Set the total array size.
            T[] data = new T[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length];

            // Execute the async concurrent tasks.
            Task.Factory.ContinueWhenAll(new Task[]
                {
                    SourceArray<T>(data, source, 0),
                    SourceArray<T>(data, arrayOne, (source.Length)),
                    SourceArray<T>(data, arrayTwo, (source.Length + arrayOne.Length)),
                    SourceArray<T>(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length))
                }, completedTasks =>
                {
                    // Do nothing
                    int failures = completedTasks.Where(t => t.Exception != null).Count();
                }).Wait();

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static T[] CombineParallel<T>(this T[] source, T[] arrayOne, T[] arrayTwo, T[] arrayThree)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");

            // Set the total array size.
            T[] data = new T[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length];

            // Execute the async concurrent tasks.
            Parallel.For(0, 4, j =>
            {
                switch (j)
                {
                    case 0:
                        SourceArrayParallel<T>(data, source, 0);
                        break;
                    case 1:
                        SourceArrayParallel<T>(data, arrayOne, (source.Length));
                        break;
                    case 2:
                        SourceArrayParallel<T>(data, arrayTwo, (source.Length + arrayOne.Length));
                        break;
                    case 3:
                        SourceArrayParallel<T>(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length));
                        break;
                }
            });

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel and asynchronously to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Task<T[]> CombineParallelAsync<T>(this T[] source, T[] arrayOne, T[] arrayTwo, T[] arrayThree)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");

            // Create the new async operation
            AsyncCombineGenericArray<T> ret = new AsyncCombineGenericArray<T>(source, arrayOne, arrayTwo, arrayThree, null, 4, 2, null, null);

            // Return the async task.
            return Task<T[]>.Factory.FromAsync(ret.BeginLoad(), ret.EndLoad);
        }

        /// <summary>
        /// Combine the array to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <param name="arrayFour">The fourth array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static T[] Combine<T>(this T[] source, T[] arrayOne, T[] arrayTwo, T[] arrayThree, T[] arrayFour)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");
            if (arrayFour == null) throw new System.ArgumentNullException("arrayFour");

            // Set the total array size.
            T[] data = new T[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length + arrayFour.Length];

            // Execute the async concurrent tasks.
            Task.Factory.ContinueWhenAll(new Task[]
                {
                    SourceArray<T>(data, source, 0),
                    SourceArray<T>(data, arrayOne, (source.Length)),
                    SourceArray<T>(data, arrayTwo, (source.Length + arrayOne.Length)),
                    SourceArray<T>(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length)),
                    SourceArray<T>(data, arrayFour, (source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length))
                }, completedTasks =>
                {
                    // Do nothing
                    int failures = completedTasks.Where(t => t.Exception != null).Count();
                }).Wait();

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <param name="arrayFour">The fourth array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static T[] CombineParallel<T>(this T[] source, T[] arrayOne, T[] arrayTwo, T[] arrayThree, T[] arrayFour)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");
            if (arrayFour == null) throw new System.ArgumentNullException("arrayFour");

            // Set the total array size.
            T[] data = new T[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length + arrayFour.Length];

            // Execute the async concurrent tasks.
            Parallel.For(0, 5, j =>
            {
                switch (j)
                {
                    case 0:
                        SourceArrayParallel<T>(data, source, 0);
                        break;
                    case 1:
                        SourceArrayParallel<T>(data, arrayOne, (source.Length));
                        break;
                    case 2:
                        SourceArrayParallel<T>(data, arrayTwo, (source.Length + arrayOne.Length));
                        break;
                    case 3:
                        SourceArrayParallel<T>(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length));
                        break;
                    case 4:
                        SourceArrayParallel<T>(data, arrayFour, (source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length));
                        break;
                }
            });

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel and asynchronously to the source array, append to the end of the array
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <param name="arrayFour">The fourth array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Task<T[]> CombineParallelAsync<T>(this T[] source, T[] arrayOne, T[] arrayTwo, T[] arrayThree, T[] arrayFour)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");
            if (arrayFour == null) throw new System.ArgumentNullException("arrayFour");

            // Create the new async operation
            AsyncCombineGenericArray<T> ret = new AsyncCombineGenericArray<T>(source, arrayOne, arrayTwo, arrayThree, arrayFour, 5, 3, null, null);

            // Return the async task.
            return Task<T[]>.Factory.FromAsync(ret.BeginLoad(), ret.EndLoad);
        }

        /// <summary>
        /// Apply the task for the byte array.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="result">The result array.</param>
        /// <param name="source">The source array.</param>
        /// <param name="start">The starting index of the result array.</param>
        /// <returns>The resulting byte array.</returns>
        internal static Task<T[]> SourceArray<T>(T[] result, T[] source, int start)
        {
            // Create the task to be returned
            var tcs = new TaskCompletionSource<T[]>(source);

            // Assign the source array.
            for (int i = 0; i < source.Length; i++)
                result[i + start] = source[i];

            // Set the completion result.
            // this indicates that this task
            // has run to completion.
            tcs.SetResult(result);

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>
        /// Apply the task for the byte array.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="result">The result array.</param>
        /// <param name="source">The source array.</param>
        /// <param name="start">The starting index of the result array.</param>
        /// <returns>The resulting byte array.</returns>
        internal static void SourceArrayParallel<T>(T[] result, T[] source, int start)
        {
            // Assign the source array.
            for (int i = 0; i < source.Length; i++)
                result[i + start] = source[i];
        }
    }

    /// <summary>
    /// Asyncronous combine bytes.
    /// </summary>
    /// <typeparam name="T">The type to examine.</typeparam>
    internal sealed class AsyncCombineGenericArray<T> : Nequeo.Threading.AsynchronousResult<T[]>
    {
        /// <summary>
        /// Async combine bytes
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array.</param>
        /// <param name="arrayTwo">The second array.</param>
        /// <param name="arrayThree">The third array.</param>
        /// <param name="arrayFour">The fourth array.</param>
        /// <param name="countExclusive">The parallel for loop count exclusive.</param>
        /// <param name="index">The current result array size index.</param>
        /// <param name="callback">The callback</param>
        /// <param name="state">The state.</param>
        public AsyncCombineGenericArray(T[] source, T[] arrayOne, T[] arrayTwo, T[] arrayThree, T[] arrayFour,
            int countExclusive, int index, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _index = index;
            _countExclusive = countExclusive;
            _source = source;
            _arrayOne = arrayOne;
            _arrayTwo = arrayTwo;
            _arrayThree = arrayThree;
            _arrayFour = arrayFour;
        }

        private int _index = 0;
        private int _countExclusive = 0;
        private Func<T[]> _loadHandler = null;
        private Exception _exception = null;

        private T[] _source = null;
        private T[] _arrayOne = null;
        private T[] _arrayTwo = null;
        private T[] _arrayThree = null;
        private T[] _arrayFour = null;

        /// <summary>
        /// Gets the current execution exception
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Load error.
        /// </summary>
        public event Nequeo.Threading.EventHandler<System.Exception> OnError;

        /// <summary>
        /// Begin the async load.
        /// </summary>
        /// <returns>The async result</returns>
        public IAsyncResult BeginLoad()
        {
            if (_loadHandler == null)
                _loadHandler = new Func<T[]>(FuncAsyncLoad);

            // Begin the async call.
            return _loadHandler.BeginInvoke(base.Callback, base.AsyncState);
        }

        /// <summary>
        /// End the async load.
        /// </summary>
        /// <param name="ar">The async result</param>
        /// <returns>The data table.</returns>
        public T[] EndLoad(IAsyncResult ar)
        {
            if (_loadHandler == null)
                throw new System.InvalidOperationException("End of asynchronous" +
                    " operation attempted when one has not begun.");

            // Use the AsyncResult to complete that async operation.
            return _loadHandler.EndInvoke(ar);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        internal T[] FuncAsyncLoad()
        {
            T[] data = null;

            try
            {
                // Create the new bytes array result size.
                switch (_index)
                {
                    case 0:
                        data = new T[_source.Length + _arrayOne.Length];
                        break;
                    case 1:
                        data = new T[_source.Length + _arrayOne.Length + _arrayTwo.Length];
                        break;
                    case 2:
                        data = new T[_source.Length + _arrayOne.Length + _arrayTwo.Length + _arrayThree.Length];
                        break;
                    case 3:
                        data = new T[_source.Length + _arrayOne.Length + _arrayTwo.Length + _arrayThree.Length + _arrayFour.Length];
                        break;
                }

                // Execute the async concurrent tasks.
                Parallel.For(0, _countExclusive, j =>
                {
                    switch (j)
                    {
                        case 0:
                            GenericArrayExtensions.SourceArrayParallel(data, _source, 0);
                            break;
                        case 1:
                            GenericArrayExtensions.SourceArrayParallel(data, _arrayOne, (_source.Length));
                            break;
                        case 2:
                            GenericArrayExtensions.SourceArrayParallel(data, _arrayTwo, (_source.Length + _arrayOne.Length));
                            break;
                        case 3:
                            GenericArrayExtensions.SourceArrayParallel(data, _arrayThree, (_source.Length + _arrayOne.Length + _arrayTwo.Length));
                            break;
                        case 4:
                            GenericArrayExtensions.SourceArrayParallel(data, _arrayFour, (_source.Length + _arrayOne.Length + _arrayTwo.Length + _arrayThree.Length));
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                _exception = ex;
                if (OnError != null)
                    OnError(this, ex);
            }

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);

            // Return the data table.
            return data;
        }
    }
}
