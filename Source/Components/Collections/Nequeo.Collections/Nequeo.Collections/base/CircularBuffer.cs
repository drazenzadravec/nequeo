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
using System.ComponentModel;

namespace Nequeo.Collections
{
    /// <summary>
    /// Circular buffer provider that is thread-safe, allows data to be buffered.
    /// </summary>
    /// <typeparam name="T">The type of buffered data.</typeparam>
	public class CircularBuffer<T> : IDisposable
	{
        /// <summary>
        /// Circular buffer.
        /// </summary>
        /// <param name="capacity">The capacity of the buffer; zero indicates dynamic, continuous increase.</param>
        /// <exception cref="System.IndexOutOfRangeException">Capacity can not be negative.</exception>
        public CircularBuffer(int capacity = 0)
        {
            // Make sure the capacity is not negative.
            if (capacity < 0)
                throw new IndexOutOfRangeException("Capacity can not be negative.");

            _capacity = capacity;
            _collection = new Collection<T>();
            _lockObject = new object();
        }

        private int _capacity = 0;
        private Collection<T> _collection = null;
        private object _lockObject;
        private bool _removeItemsWritten = false;
        private bool _capacityReached = false;
        private bool _removeItemsRead = true;

        private double _upperCapacityPercentage = 90.0;
        private double _lowerCapacityPercentage = 65.0;

        /// <summary>
        /// Gets or sets an indicator specifying if items read are to be removed from the collection.
        /// </summary>
        public virtual bool RemoveItemsRead
        {
            get { return _removeItemsRead; }
            set { _removeItemsRead = value; }
        }

        /// <summary>
        /// Gets or sets an indicator specifying when the capacity is reached the existing data is overwriiten.
        /// </summary>
        public virtual bool RemoveItemsWritten
        {
            get { return _removeItemsWritten; }
            set { _removeItemsWritten = value; }
        }

        /// <summary>
        /// Gets or sets an indicator specifying when the capacity is reached an exception is thrown.
        /// </summary>
        public virtual bool IndicateCapacityReached
        {
            get { return _capacityReached; }
            set { _capacityReached = value; }
        }

        /// <summary>
        /// Gets or sets the upper capacity percentage threshold.
        /// </summary>
        public virtual double UpperCapacityPercentage
        {
            get { return _upperCapacityPercentage; }
            set { _upperCapacityPercentage = value; }
        }

        /// <summary>
        /// Gets or sets the lower capacity percentage threshold.
        /// </summary>
        public virtual double LowerCapacityPercentage
        {
            get { return _lowerCapacityPercentage; }
            set { _lowerCapacityPercentage = value; }
        }

        /// <summary>
        /// Gets or sets the capacity of the collection.
        /// </summary>
        public int Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get 
            {
                lock (_lockObject)
                    return _collection.Count;
            }
        }

        /// <summary>
        /// Gets the current capacity percentage relative to the total count.
        /// </summary>
        public double CapacityPercentage
        {
            get
            {
                lock (_lockObject)
                {
                    // If a capacity is to be mantained.
                    if (_capacity > 0)
                        // Calculate the percentage.
                        return ((double)_collection.Count / (double)_capacity) * 100.0;
                    else
                        return 0.0;
                }
            }
        }

        /// <summary>
        /// Has the upper capacity percentage threshold been reached.
        /// </summary>
        /// <returns>True if threshold has been reached; else false.</returns>
        public virtual bool IsUpperCapacityPercentage()
        {
            if (CapacityPercentage >= _upperCapacityPercentage)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Has the lower capacity percentage threshold been reached.
        /// </summary>
        /// <returns>True if threshold has been reached; else false.</returns>
        public virtual bool IsLowerCapacityPercentage()
        {
            if (CapacityPercentage >= _lowerCapacityPercentage)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Clear the collection of all it's elements.
        /// </summary>
        public virtual void Clear()
        {
            lock (_lockObject)
                _collection.Clear();
        }

        /// <summary>
        /// Write a general object to the collection.
        /// </summary>
        /// <param name="data">The generic object to add.</param>
        public virtual void Write(T data)
        {
            lock (_lockObject)
            {
                // If items are to be removed.
                if (_removeItemsWritten)
                {
                    // If a capacity is to be mantained.
                    if (_capacity > 0 && _collection.Count > _capacity)
                    {
                        // If items exist.
                        if (_collection.Count > 0)
                        {
                            // If more elements in collection than need to be removed.
                            if (1 <= _collection.Count)
                            {
                                // Remove the elements.
                                _collection.RemoveAt(0);
                            }
                            else
                            {
                                // Clear all
                                _collection.Clear();
                            }
                        }
                    }
                }

                // Should we indicate when the capacity is reached.
                if (_capacityReached)
                {
                    // If a capacity is to be mantained.
                    if (_capacity > 0)
                    {
                        // If capacity reached throw exception.
                        if ((_collection.Count + 1) > _capacity)
                            throw new Exception("Can not store the data, not enough capacity.");
                    }
                }

                // Add the item.
                _collection.Add(data);
            }
        }

        /// <summary>
        /// Write a general object array to the collection.
        /// </summary>
        /// <param name="data">The generic objects to add.</param>
        /// <exception cref="System.Exception">Can not store the data, no enough capacity.</exception>
        public virtual void Write(T[] data)
        {
            lock (_lockObject)
            {
                // If items are to be removed.
                if (_removeItemsWritten)
                {
                    // If a capacity is to be mantained.
                    if (_capacity > 0 && _collection.Count > _capacity)
                    {
                        // If items exist.
                        if (_collection.Count > 0)
                        {
                            // If more elements in collection than need to be removed.
                            if (data.Length <= _collection.Count)
                            {
                                // Remove the elements.
                                _collection.RemoveRange(0, data.Length);
                            }
                            else
                            {
                                // Clear all
                                _collection.Clear();
                            }
                        }
                    }
                }

                // Should we indicate when the capacity is reached.
                if (_capacityReached)
                {
                    // If a capacity is to be mantained.
                    if (_capacity > 0)
                    {
                        // If capacity reached throw exception.
                        if ((_collection.Count + data.Length) > _capacity)
                            throw new Exception("Can not store the data, not enough capacity.");
                    }
                }

                // Add the collection.
                _collection.AddRange(data);
            }
        }

        /// <summary>
        /// Write a general object array to the collection.
        /// </summary>
        /// <param name="data">The generic objects to add.</param>
        /// <param name="offset">The offet to write from.</param>
        /// <param name="count">The number of items to write.</param>
        /// <exception cref="System.Exception">Can not store the data, no enough capacity.</exception>
        public virtual void Write(T[] data, int offset, int count)
        {
            lock (_lockObject)
            {
                // If items are to be removed.
                if (_removeItemsWritten)
                {
                    // If a capacity is to be mantained.
                    if (_capacity > 0 && _collection.Count > _capacity)
                    {
                        // If items exist.
                        if (_collection.Count > 0)
                        {
                            // If more elements in collection than need to be removed.
                            if (count <= _collection.Count)
                            {
                                // Remove the elements.
                                _collection.RemoveRange(0, count);
                            }
                            else
                            {
                                // Clear all
                                _collection.Clear();
                            }
                        }
                    }
                }

                // Should we indicate when the capacity is reached.
                if (_capacityReached)
                {
                    // If a capacity is to be mantained.
                    if (_capacity > 0)
                    {
                        // If capacity reached throw exception.
                        if ((_collection.Count + count) > _capacity)
                            throw new Exception("Can not store the data, not enough capacity.");
                    }
                }

                // Write the offset data.
                for (int i = offset; i < count + offset; i++)
                {
                    // Add the collection.
                    _collection.Add(data[i]);
                }
            } 
        }

        /// <summary>
        /// Write a general object array to the collection.
        /// </summary>
        /// <param name="data">The generic objects to add.</param>
        /// <param name="offset">The offet to write from.</param>
        /// <param name="count">The number of items to write.</param>
        /// <param name="position">The position to start writting at within the buffer collection type.</param>
        /// <exception cref="System.Exception">Can not store the data, no enough capacity.</exception>
        public virtual void Write(T[] data, int offset, int count, long position)
        {
            lock (_lockObject)
            {
                // If items are to be removed.
                if (_removeItemsWritten)
                {
                    // If a capacity is to be mantained.
                    if (_capacity > 0 && _collection.Count > _capacity)
                    {
                        // If items exist.
                        if (_collection.Count > 0)
                        {
                            // If more elements in collection than need to be removed.
                            if (count <= _collection.Count)
                            {
                                // Remove the elements.
                                _collection.RemoveRange(0, count);
                            }
                            else
                            {
                                // Clear all
                                _collection.Clear();
                            }
                        }
                    }
                }

                // Should we indicate when the capacity is reached.
                if (_capacityReached)
                {
                    // If a capacity is to be mantained.
                    if (_capacity > 0)
                    {
                        // If capacity reached throw exception.
                        if ((_collection.Count + count) > _capacity)
                            throw new Exception("Can not store the data, not enough capacity.");
                    }
                }

                // Set the current position
                long currentPosition = position;

                // if add to a specific position.
                if (position > 0 && position < _collection.Count)
                {
                    // Write the offset data.
                    for (int i = offset; i < count + offset; i++)
                    {
                        // Add the data.
                        if (currentPosition < _collection.Count)
                            _collection[(int)currentPosition] = data[i];
                        else
                            _collection.Add(data[i]);

                        // Increment the position.
                        currentPosition++;
                    }
                }
                else
                {
                    // Write the offset data.
                    for (int i = offset; i < count + offset; i++)
                    {
                        // Add the collection.
                        _collection.Add(data[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the object at the beginning of the collection without removing it.
        /// </summary>
        /// <param name="count">The number of items to read; else reads number available.</param>
        /// <returns>The objects in the collection.</returns>
        public virtual T[] Peek(int count)
        {
            return PeekEx(0, count);
        }

        /// <summary>
        /// Returns the object at the beginning of the collection without removing it.
        /// </summary>
        /// <param name="offset">The offet to read from.</param>
        /// <param name="count">The number of items to read; else reads number available.</param>
        /// <returns>The objects in the collection.</returns>
        public virtual T[] Peek(int offset, int count)
        {
            return PeekEx(offset, count);
        }

        /// <summary>
        /// Read the next item available. After each read the current item is removed from the collection.
        /// </summary>
        /// <returns>The object in the collection.</returns>
        public virtual T ReadNext()
        {
            return ReadEx(0, 1).First();
        }

        /// <summary>
        /// Read all items available. After each read the current items are removed from the collection.
        /// </summary>
        /// <returns>The objects in the collection.</returns>
        public virtual T[] Read()
        {
            return ReadEx(0, _collection.Count);
        }

        /// <summary>
        /// Read the next number of items available. After each read the current items are removed from the collection.
        /// </summary>
        /// <param name="count">The number of items to read; else reads number available.</param>
        /// <returns>The objects in the collection.</returns>
        public virtual T[] Read(int count)
        {
            return ReadEx(0, count);
        }

        /// <summary>
        /// Read the next number of items available. After each read the current items are removed from the collection.
        /// </summary>
        /// <param name="offset">The offet to read from.</param>
        /// <param name="count">The number of items to read; else reads number available.</param>
        /// <returns>The objects in the collection.</returns>
        public virtual T[] Read(int offset, int count)
        {
            return ReadEx(offset, count);
        }

        /// <summary>
        /// Read the next number of items available.
        /// </summary>
        /// <param name="offset">The offet to read from.</param>
        /// <param name="count">The number of items to read; else reads number available.</param>
        /// <returns>The objects in the collection.</returns>
        private T[] ReadEx(int offset, int count)
        {
            lock (_lockObject)
            {
                T[] readCollection = null;

                int index = offset;
                int available = _collection.Count;

                // If the count is less than one.
                if (count <= 1)
                {
                    // If the count is less than zero.
                    if (count <= 0)
                    {
                        // Return no collection.
                        readCollection = new T[0];
                    }
                    else
                    {
                        // If no more in the collection.
                        if (_collection.Count <= 0)
                        {
                            // Assign an empty collection.
                            readCollection = new T[0];
                        }
                        else
                        {
                            // Return one item in the collection.
                            readCollection = new T[] { _collection[index] };
                        }

                        // If items are to be remove after reading.
                        if (_removeItemsRead)
                        {
                            // Remove the items.
                            if (_collection.Count > 0)
                                _collection.RemoveAt(index);
                        }
                    }
                }
                else
                {
                    // If the available items is greater than the count
                    if (available >= count)
                    {
                        // Read the number (count) of items.
                        readCollection = new T[count];
                        ReadCollectionEx(readCollection, index, count);
                    }
                    else
                    {
                        // Read the number (available) of items.
                        readCollection = new T[available];
                        ReadCollectionEx(readCollection, index, available);
                    }
                }

                // Return the collection.
                return readCollection;
            }
        }

        /// <summary>
        /// Read the next number of items available.
        /// </summary>
        /// <param name="offset">The offet to read from.</param>
        /// <param name="count">The number of items to read; else reads number available.</param>
        /// <returns>The objects in the collection.</returns>
        private T[] PeekEx(int offset, int count)
        {
            lock (_lockObject)
            {
                T[] readCollection = null;

                int index = offset;
                int available = _collection.Count;

                // If the count is less than one.
                if (count <= 1)
                {
                    // If the count is less than zero.
                    if (count <= 0)
                    {
                        // Return no collection.
                        readCollection = new T[0];
                    }
                    else
                    {
                        // If no more in the collection.
                        if (_collection.Count <= 0)
                        {
                            // Assign an empty collection.
                            readCollection = new T[0];
                        }
                        else
                        {
                            // Return one item in the collection.
                            readCollection = new T[] { _collection[index] };
                        }
                    }
                }
                else
                {
                    // If the available items is greater than the count
                    if (available >= count)
                    {
                        // Read the number (count) of items.
                        readCollection = new T[count];
                        PeekCollectionEx(readCollection, index, count);
                    }
                    else
                    {
                        // Read the number (available) of items.
                        readCollection = new T[available];
                        PeekCollectionEx(readCollection, index, available);
                    }
                }

                // Return the collection.
                return readCollection;
            }
        }

        /// <summary>
        /// Read the collection of items and remove the previous data.
        /// </summary>
        /// <param name="readCollection"></param>
        /// <param name="index">The index to start at.</param>
        /// <param name="count">The number of items to read.</param>
        private void ReadCollectionEx(T[] readCollection, int index, int count)
        {
            // Add each read item.
            for (int i = index; i < count; i++)
                readCollection[i - index] = _collection[i];

            // If items are to be remove after reading.
            if (_removeItemsRead)
            {
                // Remove the items if the capacity has been reached.
                if (_collection.Count > 0)
                    _collection.RemoveRange(index, count);
            }
        }

        /// <summary>
        /// Read the collection of items and remove the previous data.
        /// </summary>
        /// <param name="readCollection"></param>
        /// <param name="index">The index to start at.</param>
        /// <param name="count">The number of items to read.</param>
        private void PeekCollectionEx(T[] readCollection, int index, int count)
        {
            // Add each read item.
            for (int i = index; i < count; i++)
                readCollection[i - index] = _collection[i];
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
                // Note disposing has been done.
                disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if(_collection != null)
                        _collection.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _collection = null;
                _lockObject = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~CircularBuffer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
	}
}
