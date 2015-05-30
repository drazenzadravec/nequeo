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
using System.Linq;
using System.Text;
using System.Collections;

namespace Nequeo.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class SortedTopN<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private int _n;
        private List<TKey> _topNKeys;
        private List<TValue> _topNValues;
        private IComparer<TKey> _comparer;

        public SortedTopN(int count, IComparer<TKey> comparer)
        {
            if (count < 1) throw new ArgumentOutOfRangeException("count");
            if (comparer == null) throw new ArgumentNullException("comparer");
            _n = count;
            _topNKeys = new List<TKey>(count);
            _topNValues = new List<TValue>(count);
            _comparer = comparer;
        }

        public bool Add(KeyValuePair<TKey, TValue> item)
        {
            return Add(item.Key, item.Value);
        }

        public bool Add(TKey key, TValue value)
        {
            int position = _topNKeys.BinarySearch(key, _comparer);
            if (position < 0) position = ~position;
            if (_topNKeys.Count < _n || position != 0)
            {
                // Empty out an item if we're already full and we need to
                // add another
                if (_topNKeys.Count == _n)
                {
                    _topNKeys.RemoveAt(0);
                    _topNValues.RemoveAt(0);
                    position--;
                }

                // Insert or add based on where we're adding
                if (position < _n)
                {
                    _topNKeys.Insert(position, key);
                    _topNValues.Insert(position, value);
                }
                else
                {
                    _topNKeys.Add(key);
                    _topNValues.Add(value);
                }
                return true;
            }

            // No room for this item
            return false;
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                for (int i = _topNKeys.Count - 1; i >= 0; i--)
                {
                    yield return _topNValues[i];
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = _topNKeys.Count - 1; i >= 0; i--)
            {
                yield return new KeyValuePair<TKey, TValue>(_topNKeys[i], _topNValues[i]);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
