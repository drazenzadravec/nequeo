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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Nequeo.Threading.Extension;

namespace Nequeo.Threading
{
    /// <summary>Debugger type proxy for AsyncCache.</summary>
    /// <typeparam name="TKey">Specifies the type of the cache's keys.</typeparam>
    /// <typeparam name="TValue">Specifies the type of the cache's values.</typeparam>
    internal class AsyncCache_DebugView<TKey, TValue>
    {
        private readonly AsyncCache<TKey, TValue> _asyncCache;

        internal AsyncCache_DebugView(AsyncCache<TKey,TValue> asyncCache)
        {
            _asyncCache = asyncCache;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal KeyValuePair<TKey, Task<TValue>>[] Values
        {
            get { return _asyncCache.ToArray(); }
        }
    }

    /// <summary>Caches asynchronously retrieved data.</summary>
    /// <typeparam name="TKey">Specifies the type of the cache's keys.</typeparam>
    /// <typeparam name="TValue">Specifies the type of the cache's values.</typeparam>
    [DebuggerTypeProxy(typeof(AsyncCache_DebugView<,>))]
    [DebuggerDisplay("Count={Count}")]
    public class AsyncCache<TKey, TValue> : ICollection<KeyValuePair<TKey,Task<TValue>>>
    {
        /// <summary>The factory to use to create tasks.</summary>
        private readonly Func<TKey, Task<TValue>> _valueFactory;
        /// <summary>The dictionary to store all of the tasks.</summary>
        private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _map;

        /// <summary>Initializes the cache.</summary>
        /// <param name="valueFactory">A factory for producing the cache's values.</param>
        public AsyncCache(Func<TKey, Task<TValue>> valueFactory)
        {
            if (valueFactory == null) throw new ArgumentNullException("loader");
            _valueFactory = valueFactory;
            _map = new ConcurrentDictionary<TKey, Lazy<Task<TValue>>>();
        }

        /// <summary>Gets a Task to retrieve the value for the specified key.</summary>
        /// <param name="key">The key whose value should be retrieved.</param>
        /// <returns>A Task for the value of the specified key.</returns>
        public Task<TValue> GetValue(TKey key)
        {
            if (key == null) throw new ArgumentNullException("key");
            var value = new Lazy<Task<TValue>>(() => _valueFactory(key));
            return _map.GetOrAdd(key, value).Value;
        }

        /// <summary>Sets the value for the specified key.</summary>
        /// <param name="key">The key whose value should be set.</param>
        /// <param name="value">The value to which the key should be set.</param>
        public void SetValue(TKey key, TValue value)
        {
            SetValue(key, Task.Factory.FromResult(value));
        }

        /// <summary>Sets the value for the specified key.</summary>
        /// <param name="key">The key whose value should be set.</param>
        /// <param name="value">The value to which the key should be set.</param>
        public void SetValue(TKey key, Task<TValue> value)
        {
            if (key == null) throw new ArgumentNullException("key");
            _map[key] = LazyExtensions.Create(value);
        }

        /// <summary>Gets a Task to retrieve the value for the specified key.</summary>
        /// <param name="key">The key whose value should be retrieved.</param>
        /// <returns>A Task for the value of the specified key.</returns>
        public Task<TValue> this[TKey key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }

        /// <summary>Empties the cache.</summary>
        public void Clear() { _map.Clear(); }

        /// <summary>Gets the number of items in the cache.</summary>
        public int Count { get { return _map.Count; } }

        /// <summary>Gets an enumerator for the contents of the cache.</summary>
        /// <returns>An enumerator for the contents of the cache.</returns>
        public IEnumerator<KeyValuePair<TKey, Task<TValue>>> GetEnumerator()
        {
            return _map.Select(p => new KeyValuePair<TKey, Task<TValue>>(p.Key, p.Value.Value)).GetEnumerator();
        }

        /// <summary>Gets an enumerator for the contents of the cache.</summary>
        /// <returns>An enumerator for the contents of the cache.</returns>
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        /// <summary>Adds or overwrites the specified entry in the cache.</summary>
        /// <param name="item">The item to be added.</param>
        void ICollection<KeyValuePair<TKey, Task<TValue>>>.Add(KeyValuePair<TKey, Task<TValue>> item)
        {
            this[item.Key] = item.Value;
        }

        /// <summary>Determines whether the cache contains the specified key.</summary>
        /// <param name="item">The item contained the key to be searched for.</param>
        /// <returns>True if the cache contains the key; otherwise, false.</returns>
        bool ICollection<KeyValuePair<TKey, Task<TValue>>>.Contains(KeyValuePair<TKey, Task<TValue>> item)
        {
            return _map.ContainsKey(item.Key);
        }

        /// <summary>
        /// Copies the elements of the System.Collections.Generic.ICollection to an
        /// System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the elements
        /// copied from System.Collections.Generic.ICollection. The System.Array must
        /// have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        void ICollection<KeyValuePair<TKey, Task<TValue>>>.CopyTo(KeyValuePair<TKey, Task<TValue>>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, Task<TValue>>>)_map).CopyTo(array, arrayIndex);
        }

        /// <summary>Gets whether the cache is read-only.</summary>
        bool ICollection<KeyValuePair<TKey, Task<TValue>>>.IsReadOnly { get { return false; } }

        /// <summary>Removes the specified key from the cache.</summary>
        /// <param name="item">The item containing the key to be removed.</param>
        /// <returns>True if the item could be removed; otherwise, false.</returns>
        bool ICollection<KeyValuePair<TKey, Task<TValue>>>.Remove(KeyValuePair<TKey, Task<TValue>> item)
        {
            Lazy<Task<TValue>> value;
            return _map.TryRemove(item.Key, out value);
        }
    }
}