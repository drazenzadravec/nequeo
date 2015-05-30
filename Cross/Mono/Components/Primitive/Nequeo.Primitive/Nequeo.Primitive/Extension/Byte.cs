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
using Nequeo.Custom;

namespace Nequeo.Extension
{
    /// <summary>
    /// Class that extends the Byte[] type.
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        /// Convert the source byte array to the byte order.
        /// </summary>
        /// <param name="source">The current sources</param>
        /// <param name="sourceOrder">The byte order.</param>
        /// <returns>The unsigned int.</returns>
        public static ushort ToUInt16(this Byte[] source, Nequeo.Custom.ByteOrder sourceOrder)
        {
            return BitConverter.ToUInt16(source.ToHostOrder(sourceOrder), 0);
        }

        /// <summary>
        /// Convert the source byte array to the byte order.
        /// </summary>
        /// <param name="source">The current sources</param>
        /// <param name="sourceOrder">The byte order.</param>
        /// <returns>The unsigned int.</returns>
        public static uint ToUInt32(this Byte[] source, Nequeo.Custom.ByteOrder sourceOrder)
        {
            return BitConverter.ToUInt32(source.ToHostOrder(sourceOrder), 0);
        }

        /// <summary>
        /// Convert the source byte array to the byte order.
        /// </summary>
        /// <param name="source">The current sources</param>
        /// <param name="sourceOrder">The byte order.</param>
        /// <returns>The unsigned int.</returns>
        public static ulong ToUInt64(this Byte[] source, Nequeo.Custom.ByteOrder sourceOrder)
        {
            return BitConverter.ToUInt64(source.ToHostOrder(sourceOrder), 0);
        }

        /// <summary>
        /// Converts the order of the specified array of <see cref="byte"/> to the host byte order.
        /// </summary>
        /// <returns>
        /// An array of <see cref="byte"/> converted from <paramref name="source"/>.
        /// </returns>
        /// <param name="source">
        /// An array of <see cref="byte"/> to convert.
        /// </param>
        /// <param name="sourceOrder">
        /// One of the <see cref="ByteOrder"/> enum values, indicates the byte order of
        /// <paramref name="source"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public static Byte[] ToHostOrder(this Byte[] source, Nequeo.Custom.ByteOrder sourceOrder)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.Length > 1 && !sourceOrder.IsHostOrder()
                   ? source.Reverse()
                   : source;
        }

        /// <summary>
        /// Combine the array to the source array, append to the end of the array
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Byte[] Combine(this Byte[] source, Byte[] arrayOne)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");

            // Set the total array size.
            Byte[] data = new byte[source.Length + arrayOne.Length];

            // Execute the async concurrent tasks.
            Task.Factory.ContinueWhenAll(new Task[]
                {
                    SourceArray(data, source, 0),
                    SourceArray(data, arrayOne, (source.Length))
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
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Byte[] CombineParallel(this Byte[] source, Byte[] arrayOne)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");

            // Set the total array size.
            Byte[] data = new byte[source.Length + arrayOne.Length];

            // Execute the async concurrent tasks.
            Parallel.For(0, 2, j =>
            {
                switch (j)
                {
                    case 0:
                        SourceArrayParallel(data, source, 0);
                        break;
                    case 1:
                        SourceArrayParallel(data, arrayOne, (source.Length));
                        break;
                }
            });

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel and asynchronously to the source array, append to the end of the array
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Task<Byte[]> CombineParallelAsync(this Byte[] source, Byte[] arrayOne)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");

            // Create the new async operation
            AsyncCombineBytes ret = new AsyncCombineBytes(source, arrayOne, null, null, null, 2, 0, null, null);

            // Return the async task.
            return Task<Byte[]>.Factory.FromAsync(ret.BeginLoad(), ret.EndLoad);
        }

        /// <summary>
        /// Combine the array to the source array, append to the end of the array
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Byte[] Combine(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");

            // Set the total array size.
            Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length];

            // Execute the async concurrent tasks.
            Task.Factory.ContinueWhenAll(new Task[]
                {
                    SourceArray(data, source, 0),
                    SourceArray(data, arrayOne, (source.Length)),
                    SourceArray(data, arrayTwo, (source.Length + arrayOne.Length)),
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
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Byte[] CombineParallel(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");

            // Set the total array size.
            Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length];

            // Execute the async concurrent tasks.
            Parallel.For(0, 3, j =>
            {
                switch (j)
                {
                    case 0:
                        SourceArrayParallel(data, source, 0);
                        break;
                    case 1:
                        SourceArrayParallel(data, arrayOne, (source.Length));
                        break;
                    case 2:
                        SourceArrayParallel(data, arrayTwo, (source.Length + arrayOne.Length));
                        break;
                }
            });

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel and asynchronously to the source array, append to the end of the array
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Task<Byte[]> CombineParallelAsync(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");

            // Create the new async operation
            AsyncCombineBytes ret = new AsyncCombineBytes(source, arrayOne, arrayTwo, null, null, 3, 1, null, null);

            // Return the async task.
            return Task<Byte[]>.Factory.FromAsync(ret.BeginLoad(), ret.EndLoad);
        }

        /// <summary>
        /// Combine the array to the source array, append to the end of the array
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Byte[] Combine(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo, Byte[] arrayThree)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");

            // Set the total array size.
            Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length];

            // Execute the async concurrent tasks.
            Task.Factory.ContinueWhenAll(new Task[]
                {
                    SourceArray(data, source, 0),
                    SourceArray(data, arrayOne, (source.Length)),
                    SourceArray(data, arrayTwo, (source.Length + arrayOne.Length)),
                    SourceArray(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length))
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
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Byte[] CombineParallel(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo, Byte[] arrayThree)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");

            // Set the total array size.
            Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length];

            // Execute the async concurrent tasks.
            Parallel.For(0, 4, j =>
                {
                    switch(j)
                    {
                        case 0:
                            SourceArrayParallel(data, source, 0);
                            break;
                        case 1:
                            SourceArrayParallel(data, arrayOne, (source.Length));
                            break;
                        case 2:
                            SourceArrayParallel(data, arrayTwo, (source.Length + arrayOne.Length));
                            break;
                        case 3:
                            SourceArrayParallel(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length));
                            break;
                    }
                });

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel and asynchronously to the source array, append to the end of the array
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Task<Byte[]> CombineParallelAsync(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo, Byte[] arrayThree)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");

            // Create the new async operation
            AsyncCombineBytes ret = new AsyncCombineBytes(source, arrayOne, arrayTwo, arrayThree, null, 4, 2, null, null);

            // Return the async task.
            return Task<Byte[]>.Factory.FromAsync(ret.BeginLoad(), ret.EndLoad);
        }

        /// <summary>
        /// Combine the array to the source array, append to the end of the array
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <param name="arrayFour">The fourth array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Byte[] Combine(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo, Byte[] arrayThree, Byte[] arrayFour)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");
            if (arrayFour == null) throw new System.ArgumentNullException("arrayFour");

            // Set the total array size.
            Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length + arrayFour.Length];

            // Execute the async concurrent tasks.
            Task.Factory.ContinueWhenAll(new Task[]
                {
                    SourceArray(data, source, 0),
                    SourceArray(data, arrayOne, (source.Length)),
                    SourceArray(data, arrayTwo, (source.Length + arrayOne.Length)),
                    SourceArray(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length)),
                    SourceArray(data, arrayFour, (source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length))
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
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <param name="arrayFour">The fourth array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Byte[] CombineParallel(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo, Byte[] arrayThree, Byte[] arrayFour)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");
            if (arrayFour == null) throw new System.ArgumentNullException("arrayFour");

            // Set the total array size.
            Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length + arrayFour.Length];

            // Execute the async concurrent tasks.
            Parallel.For(0, 5, j =>
            {
                switch (j)
                {
                    case 0:
                        SourceArrayParallel(data, source, 0);
                        break;
                    case 1:
                        SourceArrayParallel(data, arrayOne, (source.Length));
                        break;
                    case 2:
                        SourceArrayParallel(data, arrayTwo, (source.Length + arrayOne.Length));
                        break;
                    case 3:
                        SourceArrayParallel(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length));
                        break;
                    case 4:
                        SourceArrayParallel(data, arrayFour, (source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length));
                        break;
                }
            });

            // Return the new array.
            return data;
        }

        /// <summary>
        /// Combine the array in parallel and asynchronously to the source array, append to the end of the array
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="arrayOne">The first array to combine.</param>
        /// <param name="arrayTwo">The second array to combine.</param>
        /// <param name="arrayThree">The third array to combine.</param>
        /// <param name="arrayFour">The fourth array to combine.</param>
        /// <returns>The new byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
        public static Task<Byte[]> CombineParallelAsync(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo, Byte[] arrayThree, Byte[] arrayFour)
        {
            // If the source object is null.
            if (source == null) throw new System.ArgumentNullException("source");
            if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
            if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
            if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");
            if (arrayFour == null) throw new System.ArgumentNullException("arrayFour");

            // Create the new async operation
            AsyncCombineBytes ret = new AsyncCombineBytes(source, arrayOne, arrayTwo, arrayThree, arrayFour, 5, 3, null, null);

            // Return the async task.
            return Task<Byte[]>.Factory.FromAsync(ret.BeginLoad(), ret.EndLoad);
        }

        /// <summary>
        /// Apply the task for the byte array.
        /// </summary>
        /// <param name="result">The result array.</param>
        /// <param name="source">The source array.</param>
        /// <param name="start">The starting index of the result array.</param>
        /// <returns>The resulting byte array.</returns>
        internal static Task<byte[]> SourceArray(Byte[] result, Byte[] source, int start)
        {
            // Create the task to be returned
            var tcs = new TaskCompletionSource<byte[]>(source);

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
        /// <param name="result">The result array.</param>
        /// <param name="source">The source array.</param>
        /// <param name="start">The starting index of the result array.</param>
        /// <returns>The resulting byte array.</returns>
        internal static void SourceArrayParallel(Byte[] result, Byte[] source, int start)
        {
            // Assign the source array.
            for (int i = 0; i < source.Length; i++)
                result[i + start] = source[i];
        }
    }

    /// <summary>
    /// Asyncronous combine bytes.
    /// </summary>
    internal sealed class AsyncCombineBytes : Nequeo.Threading.AsynchronousResult<Byte[]>
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
        public AsyncCombineBytes(Byte[] source, Byte[] arrayOne, Byte[] arrayTwo, Byte[] arrayThree, Byte[] arrayFour, 
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
        private Func<Byte[]> _loadHandler = null;
        private Exception _exception = null;

        private Byte[] _source = null;
        private Byte[] _arrayOne = null;
        private Byte[] _arrayTwo = null;
        private Byte[] _arrayThree = null;
        private Byte[] _arrayFour = null;

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
                _loadHandler = new Func<Byte[]>(FuncAsyncLoad);

            // Begin the async call.
            return _loadHandler.BeginInvoke(base.Callback, base.AsyncState);
        }

        /// <summary>
        /// End the async load.
        /// </summary>
        /// <param name="ar">The async result</param>
        /// <returns>The data table.</returns>
        public Byte[] EndLoad(IAsyncResult ar)
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
        internal Byte[] FuncAsyncLoad()
        {
            Byte[] data = null;

            try
            {
                // Create the new bytes array result size.
                switch (_index)
                {
                    case 0:
                        data = new byte[_source.Length + _arrayOne.Length];
                        break;
                    case 1:
                        data = new byte[_source.Length + _arrayOne.Length + _arrayTwo.Length];
                        break;
                    case 2:
                        data = new byte[_source.Length + _arrayOne.Length + _arrayTwo.Length + _arrayThree.Length];
                        break;
                    case 3:
                        data = new byte[_source.Length + _arrayOne.Length + _arrayTwo.Length + _arrayThree.Length + _arrayFour.Length];
                        break;
                }

                // Execute the async concurrent tasks.
                Parallel.For(0, _countExclusive, j =>
                {
                    switch (j)
                    {
                        case 0:
                            ByteExtensions.SourceArrayParallel(data, _source, 0);
                            break;
                        case 1:
                            ByteExtensions.SourceArrayParallel(data, _arrayOne, (_source.Length));
                            break;
                        case 2:
                            ByteExtensions.SourceArrayParallel(data, _arrayTwo, (_source.Length + _arrayOne.Length));
                            break;
                        case 3:
                            ByteExtensions.SourceArrayParallel(data, _arrayThree, (_source.Length + _arrayOne.Length + _arrayTwo.Length));
                            break;
                        case 4:
                            ByteExtensions.SourceArrayParallel(data, _arrayFour, (_source.Length + _arrayOne.Length + _arrayTwo.Length + _arrayThree.Length));
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
