/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nequeo.Collections.Concurrent
{
    /// <summary>Provides a thread-safe object pool.</summary>
    /// <typeparam name="T">Specifies the type of the elements stored in the pool.</typeparam>
    [DebuggerDisplay("Count={Count}")]
    [DebuggerTypeProxy(typeof(IProducerConsumerCollection_DebugView<>))]
    public sealed class ObjectPool<T> : ProducerConsumerCollectionBase<T>
    {
        private readonly Func<T> _generator;

        /// <summary>Initializes an instance of the ObjectPool class.</summary>
        /// <param name="generator">The function used to create items when no items exist in the pool.</param>
        public ObjectPool(Func<T> generator) : this(generator, new ConcurrentQueue<T>()) { }

        /// <summary>Initializes an instance of the ObjectPool class.</summary>
        /// <param name="generator">The function used to create items when no items exist in the pool.</param>
        /// <param name="collection">The collection used to store the elements of the pool.</param>
        public ObjectPool(Func<T> generator, IProducerConsumerCollection<T> collection)
            : base(collection)
        {
            if (generator == null) throw new ArgumentNullException("generator");
            _generator = generator;
        }

        /// <summary>Adds the provided item into the pool.</summary>
        /// <param name="item">The item to be added.</param>
        public void PutObject(T item) { base.TryAdd(item); }

        /// <summary>Gets an item from the pool.</summary>
        /// <returns>The removed or created item.</returns>
        /// <remarks>If the pool is empty, a new item will be created and returned.</remarks>
        public T GetObject()
        {
            T value;
            return base.TryTake(out value) ? value : _generator();
        }

        /// <summary>Clears the object pool, returning all of the data that was in the pool.</summary>
        /// <returns>An array containing all of the elements in the pool.</returns>
        public T[] ToArrayAndClear()
        {
            var items = new List<T>();
            T value;
            while (base.TryTake(out value)) items.Add(value);
            return items.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool TryAdd(T item)
        {
            PutObject(item);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool TryTake(out T item)
        {
            item = GetObject();
            return true;
        }
    }
}