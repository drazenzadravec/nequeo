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

namespace Nequeo.Html
{
    /// <summary>
    /// Represents a list of mixed code fragments.
    /// </summary>
    public class MixedCodeDocumentFragmentList : IEnumerable
    {
        #region Fields

        private MixedCodeDocument _doc;
        private ArrayList _items = new ArrayList();

        #endregion

        #region Constructors

        internal MixedCodeDocumentFragmentList(MixedCodeDocument doc)
        {
            _doc = doc;
        }

        #endregion

        #region Properties

        ///<summary>
        /// Gets the Document
        ///</summary>
        public MixedCodeDocument Doc
        {
            get { return _doc; }
        }

        /// <summary>
        /// Gets the number of fragments contained in the list.
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Gets a fragment from the list using its index.
        /// </summary>
        public MixedCodeDocumentFragment this[int index]
        {
            get { return _items[index] as MixedCodeDocumentFragment; }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets an enumerator that can iterate through the fragment list.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Appends a fragment to the list of fragments.
        /// </summary>
        /// <param name="newFragment">The fragment to append. May not be null.</param>
        public void Append(MixedCodeDocumentFragment newFragment)
        {
            if (newFragment == null)
            {
                throw new ArgumentNullException("newFragment");
            }
            _items.Add(newFragment);
        }

        /// <summary>
        /// Gets an enumerator that can iterate through the fragment list.
        /// </summary>
        public MixedCodeDocumentFragmentEnumerator GetEnumerator()
        {
            return new MixedCodeDocumentFragmentEnumerator(_items);
        }

        /// <summary>
        /// Prepends a fragment to the list of fragments.
        /// </summary>
        /// <param name="newFragment">The fragment to append. May not be null.</param>
        public void Prepend(MixedCodeDocumentFragment newFragment)
        {
            if (newFragment == null)
            {
                throw new ArgumentNullException("newFragment");
            }
            _items.Insert(0, newFragment);
        }

        /// <summary>
        /// Remove a fragment from the list of fragments. If this fragment was not in the list, an exception will be raised.
        /// </summary>
        /// <param name="fragment">The fragment to remove. May not be null.</param>
        public void Remove(MixedCodeDocumentFragment fragment)
        {
            if (fragment == null)
            {
                throw new ArgumentNullException("fragment");
            }
            int index = GetFragmentIndex(fragment);
            if (index == -1)
            {
                throw new IndexOutOfRangeException();
            }
            RemoveAt(index);
        }

        /// <summary>
        /// Remove all fragments from the list.
        /// </summary>
        public void RemoveAll()
        {
            _items.Clear();
        }

        /// <summary>
        /// Remove a fragment from the list of fragments, using its index in the list.
        /// </summary>
        /// <param name="index">The index of the fragment to remove.</param>
        public void RemoveAt(int index)
        {
            //MixedCodeDocumentFragment frag = (MixedCodeDocumentFragment) _items[index];
            _items.RemoveAt(index);
        }

        #endregion

        #region Internal Methods

        internal void Clear()
        {
            _items.Clear();
        }

        internal int GetFragmentIndex(MixedCodeDocumentFragment fragment)
        {
            if (fragment == null)
            {
                throw new ArgumentNullException("fragment");
            }
            for (int i = 0; i < _items.Count; i++)
            {
                if ((_items[i]) == fragment)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region Nested type: MixedCodeDocumentFragmentEnumerator

        /// <summary>
        /// Represents a fragment enumerator.
        /// </summary>
        public class MixedCodeDocumentFragmentEnumerator : IEnumerator
        {
            #region Fields

            private int _index;
            private ArrayList _items;

            #endregion

            #region Constructors

            internal MixedCodeDocumentFragmentEnumerator(ArrayList items)
            {
                _items = items;
                _index = -1;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public MixedCodeDocumentFragment Current
            {
                get { return (MixedCodeDocumentFragment) (_items[_index]); }
            }

            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            object IEnumerator.Current
            {
                get { return (Current); }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                _index++;
                return (_index < _items.Count);
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                _index = -1;
            }

            #endregion
        }

        #endregion
    }
}