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
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;

namespace Nequeo.Collections.Concurrent
{
    /// <summary>
    /// Provides a thread-safe, concurrent collection for use with data binding.
    /// </summary>
    /// <typeparam name="T">Specifies the type of the elements in this collection.</typeparam>
    [DebuggerDisplay("Count={Count}")]
    [DebuggerTypeProxy(typeof(IProducerConsumerCollection_DebugView<>))]
    public class ObservableConcurrentCollection<T> : 
        ProducerConsumerCollectionBase<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly SynchronizationContext _context;

        /// <summary>
        /// Initializes an instance of the ObservableConcurrentCollection class with an underlying
        /// queue data structure.
        /// </summary>
        public ObservableConcurrentCollection() : this(new ConcurrentQueue<T>()) { }

        /// <summary>
        /// Initializes an instance of the ObservableConcurrentCollection class with the specified
        /// collection as the underlying data structure.
        /// </summary>
        public ObservableConcurrentCollection(IProducerConsumerCollection<T> collection) : base(collection)
        {
            _context = AsyncOperationManager.SynchronizationContext;
        }

        /// <summary>Event raised when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>Event raised when a property on the collection changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies observers of CollectionChanged or PropertyChanged of an update to the dictionary.
        /// </summary>
        private void NotifyObserversOfChange()
        {
            var collectionHandler = CollectionChanged;
            var propertyHandler = PropertyChanged;
            if (collectionHandler != null || propertyHandler != null)
            {
                _context.Post(s =>
                {
                    if (collectionHandler != null)
                    {
                        collectionHandler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    }
                    if (propertyHandler != null)
                    {
                        propertyHandler(this, new PropertyChangedEventArgs("Count"));
                    }
                }, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool TryAdd(T item)
        {
            // Try to add the item to the underlying collection.  If we were able to,
            // notify any listeners.
            bool result = base.TryAdd(item);
            if (result) NotifyObserversOfChange();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool TryTake(out T item)
        {
            // Try to remove an item from the underlying collection.  If we were able to,
            // notify any listeners.
            bool result = base.TryTake(out item);
            if (result) NotifyObserversOfChange();
            return result;
        }
    }
}