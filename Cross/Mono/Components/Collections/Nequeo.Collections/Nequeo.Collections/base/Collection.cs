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
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Nequeo.Collections
{
    /// <summary>
    /// The generic general object collection class, collects the general objects.
    /// </summary>
    /// <typeparam name="T">The generic general type to collect.</typeparam>
    public class Collection<T> : IList<T>, ICollection<T>, IList, ICollection, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Collection()
        {
            _innerArray = new List<T>();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list</param>
        public Collection(IList<T> collection)
        {
            _innerArray = new List<T>(collection);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list</param>
        public Collection(ICollection<T> collection)
        {
            _innerArray = new List<T>(collection);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list</param>
        public Collection(IEnumerable<T> collection)
        {
            _innerArray = new List<T>(collection);
        }
        #endregion

        #region Protected Fields
        /// <summary>
        /// Inner list object contains all items.
        /// </summary>
        protected List<T> _innerArray;
        /// <summary>
        /// Flag for setting collection to read-only mode.
        /// </summary>
        protected bool _isReadOnly = false;
        private object _synchronizedThread = new object();

        #endregion

        #region Public Properties
        /// <summary>
        /// Default accessor for the collection 
        /// </summary>
        /// <param name="index">The current index to access</param>
        /// <returns>The current business object in the collection.</returns>
        public T this[int index]
        {
            get { return (T)_innerArray[index]; }
            set { _innerArray[index] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object IList.this[int index]
        {
            get { return (T)_innerArray[index]; }
            set { _innerArray[index] = (T)value; }
        }

        /// <summary>
        /// Number of elements in the collection.
        /// </summary>
        public int Count
        {
            get { return _innerArray.Count; }
        }

        /// <summary>
        /// Flag sets whether or not this collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the System.Collections.ICollection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the System.Collections.ICollection.
        /// </summary>
        public object SyncRoot
        {
            get { return _synchronizedThread; }
        }

        /// <summary>
        /// Gets a value indicating whether the System.Collections.IList has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add a general object to the collection.
        /// </summary>
        /// <param name="item">The generic object to add.</param>
        public void Add(T item)
        {
            _innerArray.Add(item);
        }

        /// <summary>
        /// Add a general object to the collection.
        /// </summary>
        /// <param name="item1">The generic object to add.</param>
        /// <param name="item2">The generic object to add.</param>
        public void Add(T item1, T item2)
        {
            _innerArray.Add(item1);
            _innerArray.Add(item2);
        }

        /// <summary>
        /// Add a general object to the collection.
        /// </summary>
        /// <param name="item1">The generic object to add.</param>
        /// <param name="item2">The generic object to add.</param>
        /// <param name="item3">The generic object to add.</param>
        public void Add(T item1, T item2, T item3)
        {
            _innerArray.Add(item1);
            _innerArray.Add(item2);
            _innerArray.Add(item3);
        }

        /// <summary>
        /// Add a general object to the collection.
        /// </summary>
        /// <param name="item1">The generic object to add.</param>
        /// <param name="item2">The generic object to add.</param>
        /// <param name="item3">The generic object to add.</param>
        /// <param name="item4">The generic object to add.</param>
        public void Add(T item1, T item2, T item3, T item4)
        {
            _innerArray.Add(item1);
            _innerArray.Add(item2);
            _innerArray.Add(item3);
            _innerArray.Add(item4);
        }

        /// <summary>
        /// Add a general object to the collection.
        /// </summary>
        /// <param name="item1">The generic object to add.</param>
        /// <param name="item2">The generic object to add.</param>
        /// <param name="item3">The generic object to add.</param>
        /// <param name="item4">The generic object to add.</param>
        /// <param name="item5">The generic object to add.</param>
        public void Add(T item1, T item2, T item3, T item4, T item5)
        {
            _innerArray.Add(item1);
            _innerArray.Add(item2);
            _innerArray.Add(item3);
            _innerArray.Add(item4);
            _innerArray.Add(item5);
        }

        /// <summary>
        /// Adds the collection of items.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            _innerArray.AddRange(collection);
        }

        /// <summary>
        /// Remove first instance of a generic object from the collection.
        /// </summary>
        /// <param name="item">The current generic object.</param>
        /// <returns>True if the object was removed else false.</returns>
        public bool Remove(T item)
        {
            try
            {
                _innerArray.Remove(item);
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Does the current generic object exist in the collection.
        /// </summary>
        /// <param name="item">The current generic object.</param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return _innerArray.Contains(item);
        }

        /// <summary>
        /// Copy objects from this collection into another array.
        /// </summary>
        /// <param name="array">The array of generic objects.</param>
        public void CopyTo(T[] array)
        {
            _innerArray.CopyTo(array);
        }

        /// <summary>
        /// Copy objects from this collection into another array.
        /// </summary>
        /// <param name="array">The array of generic objects.</param>
        /// <param name="arrayIndex">The index where copy should begin.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerArray.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies a range of elements from the collection to
        /// a compatible one-dimensional array, starting at the specified index of the
        /// target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source collection at
        /// which copying begins.</param>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements
        /// copied from the collection. The System.Array must have
        /// zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            _innerArray.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>
        /// Get the collection of items.
        /// </summary>
        /// <param name="start">The index where copy should begin.</param>
        /// <param name="end">The index where copy should end.</param>
        /// <returns>The collection of items.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Index can not be greater than the count or then than zero</exception>
        public IList GetItems(int start, int end)
        {
            if ((end > _innerArray.Count - 1) || (start < 0))
                throw new IndexOutOfRangeException(
                    "Index can not be greater than the count or then than zero");

            // Create a new collection.
            Collection<T> items = new Collection<T>();

            // Get the dictionary enumerator.
            List<T>.Enumerator item = _innerArray.GetEnumerator();

            // Initial count.
            int count = 0;
            int counter = 0;

            // For each item in the dictionary.
            while (item.MoveNext())
            {
                // If the counter is between the starting
                // and ending index.
                if ((counter >= start) && (counter <= end))
                {
                    // Assign the key value pair from
                    // the current dictionary item.
                    items[count] = item.Current;
                    count++;
                }

                // Increment the counter.
                counter++;
            }

            // Return the collection of keys.
            return items;
        }

        /// <summary>
        /// Get the collection of items.
        /// </summary>
        /// <param name="start">The index where copy should begin.</param>
        /// <param name="end">The index where copy should end.</param>
        /// <returns>The collection of items.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Index can not be greater than the count or then than zero</exception>
        public IList<T> GetItemsType(int start, int end)
        {
            if ((end > _innerArray.Count - 1) || (start < 0))
                throw new IndexOutOfRangeException(
                    "Index can not be greater than the count or then than zero");

            // Create a new collection.
            Collection<T> items = new Collection<T>();

            // Get the dictionary enumerator.
            List<T>.Enumerator item = _innerArray.GetEnumerator();

            // Initial count.
            int count = 0;
            int counter = 0;

            // For each item in the dictionary.
            while (item.MoveNext())
            {
                // If the counter is between the starting
                // and ending index.
                if ((counter >= start) && (counter <= end))
                {
                    // Assign the key value pair from
                    // the current dictionary item.
                    items[count] = item.Current;
                    count++;
                }

                // Increment the counter.
                counter++;
            }

            // Return the collection of keys.
            return items;
        }

        /// <summary>
        /// Copies the elements of the System.Collections.ICollection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements
        /// copied from System.Collections.ICollection. The System.Array must have zero-based
        /// indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            if (index > _innerArray.Count - 1)
                throw new IndexOutOfRangeException("Index can not be greater than the count");

            // Get the dictionary enumerator.
            List<T>.Enumerator item = _innerArray.GetEnumerator();

            // Initial count.
            int count = 0;
            int counter = 0;

            // For each item in the dictionary.
            while (item.MoveNext())
            {
                // If counter is creater than or equal
                // to the starting index.
                if (counter >= index)
                {
                    // Lock the object.
                    lock (_synchronizedThread)
                    {
                        // Assign the key value pair from
                        // the current dictionary item.
                        array.SetValue(item.Current, count);
                        count++;
                    }
                }

                // Increment the counter.
                counter++;
            }
        }

        /// <summary>
        /// Sorts the array of list general objects.
        /// </summary>
        public virtual void Sort()
        {
            // Sort the general list.
            _innerArray.Sort();
        }

        /// <summary>
        /// Clear the collection of all it's elements.
        /// </summary>
        public void Clear()
        {
            _innerArray.Clear();
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the entire
        /// </summary>
        /// <param name="item">The object to locate in the collection. The value
        /// can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire collection,
        /// if found; otherwise, –1.</returns>
        public int IndexOf(T item)
        {
            return _innerArray.IndexOf(item);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified
        /// index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        public void Insert(int index, T item)
        {
            _innerArray.Insert(index, item);
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            _innerArray.RemoveAt(index);
        }

        /// <summary>
        /// Removes a range of elements from the collection.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public void RemoveRange(int index, int count)
        {
            _innerArray.RemoveRange(index, count);
        }

        /// <summary>
        /// Convert the collection to the array type.
        /// </summary>
        /// <returns>The collection type.</returns>
        public T[] ToArray()
        {
            return _innerArray.ToArray<T>();
        }

        /// <summary>
        /// Adds an item to the System.Collections.IList.
        /// </summary>
        /// <param name="value">The object to add to the System.Collections.IList.</param>
        /// <returns>The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection,</returns>
        public int Add(object value)
        {
            _innerArray.Add((T)value);
            return _innerArray.Count - 1;
        }

        /// <summary>
        /// Determines whether the System.Collections.IList contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the System.Collections.IList.</param>
        /// <returns>true if the System.Object is found in the System.Collections.IList; otherwise, false.</returns>
        public bool Contains(object value)
        {
            return _innerArray.Contains((T)value);
        }

        /// <summary>
        /// Determines the index of a specific item in the System.Collections.IList.
        /// </summary>
        /// <param name="value">The object to locate in the System.Collections.IList.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(object value)
        {
            return _innerArray.IndexOf((T)value);
        }

        /// <summary>
        /// Inserts an item to the System.Collections.IList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The object to insert into the System.Collections.IList.</param>
        public void Insert(int index, object value)
        {
            _innerArray.Insert(index, (T)value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the System.Collections.IList.
        /// </summary>
        /// <param name="value">The object to remove from the System.Collections.IList.</param>
        public void Remove(object value)
        {
            try
            {
                _innerArray.Remove((T)value);
            }
            catch { }
        }

        /// <summary>
        /// The custom generic enumerator for this CollectionEnumerator.
        /// </summary>
        /// <returns>The custom generic enumerator general object.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            // Return a custom enumerator object 
            // instantiated to use this GeneralCollection.
            return new CollectionEnumerator<T>(this);
        }

        /// <summary>
        /// The non-generic interface implementation for IEnumerable 
        /// extended and required by ICollection, implemented by ICollection
        /// </summary>
        /// <returns>The non-generic enumerator general object.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CollectionEnumerator<T>(this);
        }
        #endregion

        /// <summary>
        /// The generic general object enumerator class, enumerates the general
        /// object collection.
        /// </summary>
        /// <typeparam name="TInternal">The generic general type to enumerate.</typeparam>
        public class CollectionEnumerator<TInternal> : IEnumerator<TInternal>
        {
            #region Constructors
            /// <summary>
            /// Default constructor
            /// </summary>
            public CollectionEnumerator()
            {
            }

            /// <summary>
            /// Constructor which takes the collection 
            /// which this enumerator will enumerate.
            /// </summary>
            /// <param name="collection"></param>
            public CollectionEnumerator(Collection<TInternal> collection)
            {
                _collection = collection;
                _index = -1;
                _current = default(TInternal);
            }
            #endregion

            #region Protected Fields
            /// <summary>
            /// Enumerated collection.
            /// </summary>
            protected Collection<TInternal> _collection;
            /// <summary>
            /// The current enumerated index.
            /// </summary>
            protected int _index;
            /// <summary>
            /// The current enumerated object in the collection
            /// </summary>
            protected TInternal _current;

            #endregion

            #region Public Properties
            /// <summary>
            /// Gets the current enumerated object in the inner collection.
            /// </summary>
            public TInternal Current
            {
                get { return _current; }
            }

            /// <summary>
            /// Explicit non-generic interface implementation for IEnumerator 
            /// (extended and required by IEnumerator).
            /// </summary>
            object IEnumerator.Current
            {
                get { return _current; }
            }
            #endregion

            #region Public Methods
            /// <summary>
            /// General object dispose method.
            /// </summary>
            public void Dispose()
            {
                _collection = null;
                _current = default(TInternal);
                _index = -1;
            }

            /// <summary>
            /// Moves to the next element in the inner collection.
            /// </summary>
            /// <returns>True if the collection exits else false.</returns>
            public bool MoveNext()
            {
                // Make sure we are within the bounds of the collection.
                // if we are, then set the current element to the 
                // next object in the collection.
                if (++_index >= _collection.Count)
                    return false;
                else
                    _current = _collection[_index];

                //return true.
                return true;
            }

            /// <summary>
            /// Reset the enumerator.
            /// </summary>
            public void Reset()
            {
                // Reset current object.
                _current = default(TInternal);
                _index = -1;
            }
            #endregion
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_innerArray != null)
                        _innerArray.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _innerArray = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Collection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

    }
}
