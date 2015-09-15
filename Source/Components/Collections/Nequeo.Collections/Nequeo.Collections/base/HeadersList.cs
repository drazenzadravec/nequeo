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
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Nequeo.Collections
{
    /// <summary>
    /// Headers list class.
    /// </summary>
    public class HeadersList : IList<KeyValuePair<string, string>>
    {
        private readonly List<KeyValuePair<string, string>> _collection;
        private readonly object _modificationLock = new object();

        /// <summary>
        /// Gets the size of the stored headers in bytes.
        /// </summary>
        /// <value>
        /// The size of the stored headers in bytes.
        /// </value>
        public int StoredHeadersSize { get; private set; }

        /// <summary>
        /// Headers list class.
        /// </summary>
        public HeadersList()
            : this(16)
        { }

        /// <summary>
        /// Headers list class.
        /// </summary>
        /// <param name="list">The list.</param>
        public HeadersList(IEnumerable<KeyValuePair<string, string>> list)
        {
            _collection = new List<KeyValuePair<string, string>>();
            AddRange(list);
        }

        /// <summary>
        /// Headers list class.
        /// </summary>
        /// <param name="headers">The headers.</param>
        public HeadersList(IDictionary<string, string[]> headers)
        {
            _collection = new List<KeyValuePair<string, string>>();

            //Send only first value?
            foreach (var header in headers)
            {
                _collection.Add(new KeyValuePair<string, string>(header.Key.ToLower(), header.Value[0]));
            }
        }

        /// <summary>
        /// Headers list class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public HeadersList(int capacity)
        {
            _collection = new List<KeyValuePair<string, string>>(capacity);
        }

        /// <summary>
        /// Get the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        public string GetValue(string key)
        {
            var headerFound = _collection.Find(header => header.Key == key);

            if (!headerFound.Equals(default(KeyValuePair<string, string>)))
            {
                return headerFound.Value;
            }

            return null;
        }

        /// <summary>
        /// Add range.
        /// </summary>
        /// <param name="headers">The header collection.</param>
        public void AddRange(IEnumerable<KeyValuePair<string, string>> headers)
        {
            lock (_modificationLock)
            {
                foreach (var header in headers)
                {
                    Add(header);
                }
            }
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>The enumeration.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>The enumeration.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Add the header.
        /// </summary>
        /// <param name="header">The header.</param>
        public void Add(KeyValuePair<string, string> header)
        {
            lock (_modificationLock)
            {
                _collection.Add(header);
                StoredHeadersSize += header.Key.Length + header.Value.Length + 32;
            }
        }

        /// <summary>
        /// Clear the list.
        /// </summary>
        public void Clear()
        {
            lock (_modificationLock)
            {
                _collection.Clear();
                StoredHeadersSize = 0;
            }
        }

        /// <summary>
        /// Contains header.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, string> header)
        {
            return _collection.Contains(header);
        }

        /// <summary>
        /// Copy to.
        /// </summary>
        /// <param name="array">The array of headers.</param>
        /// <param name="arrayIndex">The strat index.</param>
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            Contract.Assert(arrayIndex >= 0 && arrayIndex < Count && array != null);
            _collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove the header.
        /// </summary>
        /// <param name="header">The header.</param>
        public bool Remove(KeyValuePair<string, string> header)
        {
            lock (_modificationLock)
            {
                StoredHeadersSize -= header.Key.Length + header.Value.Length + 32;
                return _collection.Remove(header);
            }
        }

        /// <summary>
        /// Gets the header count.
        /// </summary>
        public int Count
        {
            get { return _collection.Count; }
        }

        /// <summary>
        /// Gets an indicator specifying if the collection is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Find the index.
        /// </summary>
        /// <param name="predicate">The predicate used to search for the index.</param>
        /// <returns>The index.</returns>
        public int FindIndex(Predicate<KeyValuePair<string, string>> predicate)
        {
            return _collection.FindIndex(predicate);
        }

        /// <summary>
        /// Get the index of.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>The index.</returns>
        public int IndexOf(KeyValuePair<string, string> header)
        {
            return _collection.IndexOf(header);
        }

        /// <summary>
        /// Insert header.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="header">The header to insert.</param>
        public void Insert(int index, KeyValuePair<string, string> header)
        {
            lock (_modificationLock)
            {
                Contract.Assert(index >= 0 && (index == 0 || index < Count));
                StoredHeadersSize += header.Key.Length + header.Value.Length + 32;
                _collection.Insert(index, header);
            }
        }

        /// <summary>
        /// Remove at.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        public void RemoveAt(int index)
        {
            lock (_modificationLock)
            {
                Contract.Assert(index >= 0 && index < Count);
                var header = _collection[index];
                _collection.RemoveAt(index);
                StoredHeadersSize -= header.Key.Length + header.Value.Length + 32;
            }
        }

        /// <summary>
        /// Remove all.
        /// </summary>
        /// <param name="predicate">The predicate to search on.</param>
        /// <returns>The number removed.</returns>
        public int RemoveAll(Predicate<KeyValuePair<string, string>> predicate)
        {
            lock (_modificationLock)
            {

                var predMatch = _collection.FindAll(predicate);
                int toDeleteSize = predMatch.Sum(header => header.Key.Length + header.Value.Length + 32);
                StoredHeadersSize -= toDeleteSize;

                return _collection.RemoveAll(predicate);
            }
        }

        /// <summary>
        /// Contains the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>True is the name exists.</returns>
        public bool ContainsName(string name)
        {
            return _collection.FindIndex(kv => kv.Key.Equals(name)) != -1;
        }

        /// <summary>
        /// Header indexer.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The header.</returns>
        public KeyValuePair<string, string> this[int index]
        {
            get
            {
                Contract.Assert(index >= 0 && index < Count);
                return _collection[index];
            }
            set
            {
                lock (_modificationLock)
                {
                    Contract.Assert(index >= 0 && index < Count);
                    _collection[index] = value;
                }
            }
        }
    }
}
