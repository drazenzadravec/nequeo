/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Http2.Protocol
{
    /// <summary>
    /// Headers list class.
    /// </summary>
    internal class HeadersList : IList<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Headers list class.
        /// </summary>
        public HeadersList()
            : this(16)
        { }

        /// <summary>
        /// Headers list class.
        /// </summary>
        /// <param name="list">The header list.</param>
        public HeadersList(IEnumerable<KeyValuePair<string, string>> list)
        {
            _collection = new List<KeyValuePair<string, string>>();
            AddRange(list);
        }

        /// <summary>
        /// Headers list class.
        /// </summary>
        /// <param name="headers">The header pair.</param>
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
        /// Gets header count.
        /// </summary>
        public int Count
        {
            get { return _collection.Count; }
        }

        /// <summary>
        /// Gets is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets indexer.
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

        /// <summary>
        /// Get header value.
        /// </summary>
        /// <param name="key">The header key.</param>
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
        /// Add headers.
        /// </summary>
        /// <param name="headers">The header list.</param>
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
        /// Get enumerator.
        /// </summary>
        /// <returns>The headers.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// The size of an entry is the sum of its name's length in bytes (as
        /// defined in Section 4.1.2), of its value's length in bytes
        /// (Section 4.1.3) and of 32 bytes.  The 32 bytes are an accounting for
        /// the entry structure overhead.  For example, an entry structure using
        /// two 64-bits pointers to reference the name and the value and the
        /// entry, and two 64-bits integer for counting the number of references
        /// to these name and value would use 32 bytes.
        /// </summary>
        /// <param name="header">The header list.</param>
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
        /// <param name="header">Headers.</param>
        /// <returns>True if exists.</returns>
        public bool Contains(KeyValuePair<string, string> header)
        {
            return _collection.Contains(header);
        }

        /// <summary>
        /// Copy to.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <param name="arrayIndex">Starting index.</param>
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            Contract.Assert(arrayIndex >= 0 && arrayIndex < Count && array != null);
            _collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// The size of an entry is the sum of its name's length in bytes (as
        /// defined in Section 4.1.2), of its value's length in bytes
        /// (Section 4.1.3) and of 32 bytes.  The 32 bytes are an accounting for
        /// the entry structure overhead.  For example, an entry structure using
        /// two 64-bits pointers to reference the name and the value and the
        /// entry, and two 64-bits integer for counting the number of references
        /// to these name and value would use 32 bytes.
        /// </summary>
        /// <param name="header">header.</param>
        /// <returns>True if removed.</returns>
        public bool Remove(KeyValuePair<string, string> header)
        {
            lock (_modificationLock)
            {
                StoredHeadersSize -= header.Key.Length + header.Value.Length + 32;
                return _collection.Remove(header);
            }
        }

        /// <summary>
        /// Find index.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>The index.</returns>
        public int FindIndex(Predicate<KeyValuePair<string, string>> predicate)
        {
            return _collection.FindIndex(predicate);
        }

        /// <summary>
        /// Index of.
        /// </summary>
        /// <param name="header">Header.</param>
        /// <returns>The index.</returns>
        public int IndexOf(KeyValuePair<string, string> header)
        {
            return _collection.IndexOf(header);
        }

        /// <summary>
        /// The size of an entry is the sum of its name's length in bytes (as
        /// defined in Section 4.1.2), of its value's length in bytes
        /// (Section 4.1.3) and of 32 bytes.  The 32 bytes are an accounting for
        /// the entry structure overhead.  For example, an entry structure using
        /// two 64-bits pointers to reference the name and the value and the
        /// entry, and two 64-bits integer for counting the number of references
        /// to these name and value would use 32 bytes.
        /// </summary>
        /// <param name="index">index.</param>
        /// <param name="header">Header.</param>
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
        /// The size of an entry is the sum of its name's length in bytes (as
        /// defined in Section 4.1.2), of its value's length in bytes
        /// (Section 4.1.3) and of 32 bytes.  The 32 bytes are an accounting for
        /// the entry structure overhead.  For example, an entry structure using
        /// two 64-bits pointers to reference the name and the value and the
        /// entry, and two 64-bits integer for counting the number of references
        /// to these name and value would use 32 bytes.
        /// </summary>
        /// <param name="index">index.</param>
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
        /// The size of an entry is the sum of its name's length in bytes (as
        /// defined in Section 4.1.2), of its value's length in bytes
        /// (Section 4.1.3) and of 32 bytes.  The 32 bytes are an accounting for
        /// the entry structure overhead.  For example, an entry structure using
        /// two 64-bits pointers to reference the name and the value and the
        /// entry, and two 64-bits integer for counting the number of references
        /// to these name and value would use 32 bytes.
        /// </summary>
        /// <param name="predicate">predicate.</param>
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
        /// Contains name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>True if name exists.</returns>
        public bool ContainsName(string name)
        {
            return _collection.FindIndex(kv => kv.Key.Equals(name)) != -1;
        }
    }
}
