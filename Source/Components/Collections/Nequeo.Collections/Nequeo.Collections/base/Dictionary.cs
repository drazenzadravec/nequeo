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
    /// A generic custom dictionary, implememts IDictionary interface.
    /// Additional generic dictionary implementations.
    /// </summary>
    /// <typeparam name="Key">The generic key type.</typeparam>
    /// <typeparam name="Value">The generic value type.</typeparam>
    public class CustomDictionary<Key, Value> : IDictionary<Key, Value>, IDictionary, ICollection, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CustomDictionary()
        {
            _item = new Dictionary<Key, Value>();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="capacity">Collection capacity.</param>
        public CustomDictionary(int capacity)
        {
            _capacity = capacity;
            _item = new Dictionary<Key, Value>(capacity);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="collection">The IDictionary
        /// whose elements are copied to the new Dictionary.</param>
        public CustomDictionary(IDictionary<Key, Value> collection)
        {
            _item = new Dictionary<Key, Value>(collection);
        }
        #endregion

        #region Private Fields
        private Key _key = default(Key);
        private Value _value = default(Value);
        private Dictionary<Key, Value> _item = null;
        private object _synchronizedThread = new object();
        private int _capacity = -1;
        #endregion

        #region Public Properties
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
        /// Gets the generic key type.
        /// </summary>
        public Key KeyType
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets the generic value type.
        /// </summary>
        public Value ValueType
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the collection of all keys.
        /// </summary>
        public ICollection<Key> Keys
        {
            get { return _item.Keys; }
        }

        /// <summary>
        /// Gets the collection of all values.
        /// </summary>
        public ICollection<Value> Values
        {
            get { return _item.Values; }
        }

        /// <summary>
        /// Gets the collection of all keys.
        /// </summary>
        ICollection IDictionary.Keys
        {
            get { return _item.Keys; }
        }

        /// <summary>
        /// Gets the collection of all values.
        /// </summary>
        ICollection IDictionary.Values
        {
            get { return _item.Values; }
        }

        /// <summary>
        /// Gets the number of entities in the dictionary.
        /// </summary>
        public int Count
        {
            get { return _item.Count; }
        }

        /// <summary>
        /// Is the collection read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets sets the value indexer.
        /// </summary>
        /// <param name="key">The key to get the value of.</param>
        /// <returns>The value in the dictionary; else the default of the Value type.</returns>
        public Value this[Key key]
        {
            get 
            {
                if (_item.Keys.Contains(key))
                    return (Value)_item[key];
                else
                    return default(Value);
            }
            set { _item[key] = value; }
        }

        /// <summary>
        /// Gets sets the value indexer.
        /// </summary>
        /// <param name="key">The key to get the value of.</param>
        /// <returns>The value in the dictionary; else the default of the Value type.</returns>
        public object this[object key]
        {
            get
            {
                if (_item.Keys.Contains((Key)key))
                    return (Value)_item[(Key)key];
                else
                    return default(Value);
            }
            set { _item[(Key)key] = (Value)value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a value indicating whether the System.Collections.IDictionary object has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get 
            {
                if (_capacity > -1)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Add the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(Key key, Value value)
        {
            if(!ContainsKey(key))
                _item.Add(key, value);
        }

        /// <summary>
        /// Add the specified key and value to the dictionary.
        /// </summary>
        /// <param name="item">The key value pair to add.</param>
        public void Add(KeyValuePair<Key, Value> item)
        {
            if (!ContainsKey(item.Key))
                _item.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Add the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(object key, object value)
        {
            if (!ContainsKey((Key)key))
                _item.Add((Key)key, (Value)value);
        }

        /// <summary>
        /// Determines if the dictionary conatins the key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>True if the key exists else false.</returns>
        public bool Contains(object key)
        {
            return _item.ContainsKey((Key)key);
        }

        /// <summary>
        /// Determines if the dictionary conatins the key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>True if the key exists else false.</returns>
        public bool ContainsKey(Key key)
        {
            return _item.ContainsKey(key);
        }

        /// <summary>
        /// Determines if the dictionary conatins the value.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>True if the value exists else false.</returns>
        public bool ContainsValue(Value value)
        {
            return _item.ContainsValue(value);
        }

        /// <summary>
        /// Removes the value from the dictionary for the
        /// specified key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>True if the element was removed.</returns>
        public bool Remove(Key key)
        {
            return _item.Remove(key);
        }

        /// <summary>
        /// Removes the value from the dictionary for the
        /// specified key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>True if the element was removed.</returns>
        public void Remove(object key)
        {
            _item.Remove((Key)key);
        }

        /// <summary>
        /// Removes the value from the dictionary for the
        /// specified key value pair item.
        /// </summary>
        /// <param name="item">The key value pair to remove.</param>
        /// <returns>True if the element was removed.</returns>
        public bool Remove(KeyValuePair<Key, Value> item)
        {
            return _item.Remove(item.Key);
        }

        /// <summary>
        /// Get the value associated with the key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">The value for the key.</param>
        /// <returns>True if the value was found for the key.</returns>
        public bool TryGetValue(Key key, out Value value)
        {
            // Assign the default values for the value types.
            value = default(Value);
            Value valueItem = default(Value);

            // Search for the value for the specified key.
            bool ret = _item.TryGetValue(key, out valueItem);

            // Assign the value.
            value = valueItem;

            // Return true if the value was found.
            return ret;
        }

        /// <summary>
        /// Removes all keys and values from the dictionary.
        /// </summary>
        public void Clear()
        {
            _item.Clear();
        }

        /// <summary>
        /// Determines if the item exists in the dictionary.
        /// </summary>
        /// <param name="item">The key value pair to search for.</param>
        /// <returns>True if the item exists in the dictionary else false.</returns>
        public bool Contains(KeyValuePair<Key, Value> item)
        {
            return _item.Contains(item);
        }

        /// <summary>
        /// Copies the dictionary items to a key value pair array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        public void CopyTo(KeyValuePair<Key, Value>[] array)
        {
            // Get the dictionary enumerator.
            Dictionary<Key, Value>.Enumerator item = _item.GetEnumerator();

            // Initial count.
            int count = 0;

            // For each item in the dictionary.
            while (item.MoveNext())
            {
                // Assign the key value pair from
                // the current dictionary item.
                array[count] = item.Current;
                count++;
            }
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
            if (index > _item.Count - 1)
                throw new IndexOutOfRangeException("Index can not be greater than the count.");

            // Get the dictionary enumerator.
            Dictionary<Key, Value>.Enumerator item = _item.GetEnumerator();

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
                        array.SetValue(item.Current.Key, count);
                        count++;
                    }
                }
                else
                    break;

                // Increment the counter.
                counter++;
            }
        }

        /// <summary>
        /// Copies the dictionary items to a key value pair array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="index">The index where copy should begin.</param>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Index can not be greater than the count</exception>
        public void CopyTo(KeyValuePair<Key, Value>[] array, int index)
        {
            if (index > _item.Count - 1)
                throw new IndexOutOfRangeException("Index can not be greater than the count.");

            // Get the dictionary enumerator.
            Dictionary<Key, Value>.Enumerator item = _item.GetEnumerator();

            // Initial count.
            int count = 0;
            int counter = 0;

            // For each item in the dictionary.
            while(item.MoveNext())
            {
                // If counter is creater than or equal
                // to the starting index.
                if (counter >= index)
                {
                    // Assign the key value pair from
                    // the current dictionary item.
                    array[count] = item.Current;
                    count++;
                }
                else
                    break;

                // Increment the counter.
                counter++;
            }
        }

        /// <summary>
        /// Copies the dictionary items to a key value pair array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="start">The index where copy should begin.</param>
        /// <param name="end">The index where copy should end.</param>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Index can not be greater than the count or then than zero</exception>
        public void CopyTo(KeyValuePair<Key, Value>[] array, int start, int end)
        {
            if ((end > _item.Count - 1) || (start < 0))
                throw new IndexOutOfRangeException(
                    "Index can not be greater than the count or zero.");

            // Get the dictionary enumerator.
            Dictionary<Key, Value>.Enumerator item = _item.GetEnumerator();

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
                    array[count] = item.Current;
                    count++;
                }
                else
                    break;

                // Increment the counter.
                counter++;
            }
        }

        /// <summary>
        /// Get the collection of keys.
        /// </summary>
        /// <returns>The collection of keys.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Index can not be greater than the count or then than zero</exception>
        public IList GetKeys()
        {
            // Create a new collection.
            Collection<Key> keys = new Collection<Key>();
            
            // For each value.
            foreach (KeyValuePair<Key, Value> item in _item)
            {
                // Add the key.
                keys.Add(item.Key);
            }

            // Return the collection of keys.
            return keys;
        }

        /// <summary>
        /// Get the collection of keys.
        /// </summary>
        /// <param name="predicate">The value predicate.</param>
        /// <returns>The collection of keys.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Index can not be greater than the count or then than zero</exception>
        public IList GetKeys(Func<KeyValuePair<Key, Value>, bool> predicate)
        {
            // Create a new collection.
            Collection<Key> keys = new Collection<Key>();

            // Get the values for the predicate.
            IEnumerable<KeyValuePair<Key, Value>> values = _item.Where(predicate);

            // For each value.
            foreach (KeyValuePair<Key, Value> item in values)
            {
                // Add the key.
                keys.Add(item.Key);
            }

            // Return the collection of keys.
            return keys;
        }

        /// <summary>
        /// Get the collection of keys.
        /// </summary>
        /// <param name="predicate">The value predicate.</param>
        /// <param name="count">The number to return.</param>
        /// <returns>The collection of keys.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Index can not be greater than the count or then than zero</exception>
        public IList GetKeys(Func<KeyValuePair<Key, Value>, bool> predicate, int count)
        {
            // Create a new collection.
            Collection<Key> keys = new Collection<Key>();

            // Get the values for the predicate.
            IEnumerable<KeyValuePair<Key, Value>> values = _item.Where(predicate).Take(count);

            // For each value.
            foreach (KeyValuePair<Key, Value> item in values)
            {
                // Add the key.
                keys.Add(item.Key);
            }

            // Return the collection of keys.
            return keys;
        }

        /// <summary>
        /// Get the collection of keys.
        /// </summary>
        /// <param name="start">The index where copy should begin.</param>
        /// <param name="end">The index where copy should end.</param>
        /// <returns>The collection of keys.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Index can not be greater than the count or then than zero</exception>
        public IList GetKeys(int start, int end)
        {
            if ((end > _item.Count - 1) || (start < 0))
                throw new IndexOutOfRangeException(
                     "Index can not be greater than the count or zero.");

            // Create a new collection.
            Collection<Key> keys = new Collection<Key>();

            // Get the dictionary enumerator.
            Dictionary<Key, Value>.Enumerator item = _item.GetEnumerator();

            // Initial count.
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
                    keys.Add(item.Current.Key);
                }
                else
                    break;

                // Increment the counter.
                counter++;
            }

            // Return the collection of keys.
            return keys;
        }

        /// <summary>
        /// Get the collection of keys.
        /// </summary>
        /// <param name="start">The index where copy should begin.</param>
        /// <param name="end">The index where copy should end.</param>
        /// <returns>The collection of keys.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Index can not be greater than the count or then than zero</exception>
        public IList<Key> GetKeysType(int start, int end)
        {
            if ((end > _item.Count - 1) || (start < 0))
                throw new IndexOutOfRangeException(
                     "Index can not be greater than the count or zero.");

            // Create a new collection.
            Collection<Key> keys = new Collection<Key>();

            // Get the dictionary enumerator.
            Dictionary<Key, Value>.Enumerator item = _item.GetEnumerator();

            // Initial count.
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
                    keys.Add(item.Current.Key);
                }
                else
                    break;

                // Increment the counter.
                counter++;
            }

            // Return the collection of keys.
            return keys;
        }

        /// <summary>
        /// Get the collection of values.
        /// </summary>
        /// <param name="start">The index where copy should begin.</param>
        /// <param name="end">The index where copy should end.</param>
        /// <returns>The collection of values.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Index can not be greater than the count or then than zero</exception>
        public IList GetValues(int start, int end)
        {
            if ((end > _item.Count - 1) || (start < 0))
                throw new IndexOutOfRangeException(
                     "Index can not be greater than the count or zero.");

            // Create a new collection.
            Collection<Value> values = new Collection<Value>();

            // Get the dictionary enumerator.
            Dictionary<Key, Value>.Enumerator item = _item.GetEnumerator();

            // Initial count.
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
                    values.Add(item.Current.Value);
                }
                else
                    break;

                // Increment the counter.
                counter++;
            }

            // Return the collection of values.
            return values;
        }

        /// <summary>
        /// Get the collection of values.
        /// </summary>
        /// <param name="start">The index where copy should begin.</param>
        /// <param name="end">The index where copy should end.</param>
        /// <returns>The collection of values.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Index can not be greater than the count or then than zero</exception>
        public IList<Value> GetValuesType(int start, int end)
        {
            if ((end > _item.Count - 1) || (start < 0))
                throw new IndexOutOfRangeException(
                     "Index can not be greater than the count or zero.");

            // Create a new collection.
            Collection<Value> values = new Collection<Value>();

            // Get the dictionary enumerator.
            Dictionary<Key, Value>.Enumerator item = _item.GetEnumerator();

            // Initial count.
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
                    values.Add(item.Current.Value);
                }
                else
                    break;

                // Increment the counter.
                counter++;
            }

            // Return the collection of values.
            return values;
        }

        /// <summary>
        /// Gets the type of object in the dictionary
        /// for the specified key.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <returns>The type of object in the dictionary.</returns>
        public Type GetValueType(Key key)
        {
            return _item[key].GetType();
        }

        /// <summary>
        /// Returns an enumerator that iterates through
        /// the custom dictionary.
        /// </summary>
        /// <returns>The dictionary key value pair enumerator.</returns>
        public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
        {
            return _item.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through
        /// the custom dictionary.
        /// </summary>
        /// <returns>The dictionary enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _item.GetEnumerator();
        }

        /// <summary>
        /// Dictionary value enumerator, allows iteration through all values.
        /// </summary>
        /// <returns>The dictionary value item.</returns>
        public IEnumerator<Value> GetValue()
        {
            foreach (KeyValuePair<Key, Value> keyValuePair in _item)
                yield return keyValuePair.Value;
        }

        /// <summary>
        /// Dictionary key enumerator, allows iteration through all keys.
        /// </summary>
        /// <returns>The dictionary key item.</returns>
        public IEnumerator<Key> GetKey()
        {
            foreach (KeyValuePair<Key, Value> keyValuePair in _item)
                yield return keyValuePair.Key;
        }

        /// <summary>
        /// Returns an System.Collections.IDictionaryEnumerator object for the System.Collections.IDictionary object.
        /// </summary>
        /// <returns>An System.Collections.IDictionaryEnumerator object for the System.Collections.IDictionary object.</returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _item.GetEnumerator();
        }
        #endregion

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
                    if (_item != null)
                        _item.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _item = null;

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
        ~CustomDictionary()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

    }
}
