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
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Nequeo.Collections.Extension
{
    /// <summary>
    /// Extension methods for IProducerConsumerCollection.
    /// </summary>
    public static class IProducerConsumerCollectionExtensions
    {
        /// <summary>Clears the collection by repeatedly taking elements until it's empty.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection to be cleared.</param>
        public static void Clear<T>(this IProducerConsumerCollection<T> collection)
        {
            T ignored;
            while (collection.TryTake(out ignored)) ;
        }

        /// <summary>Creates an enumerable which will consume and return elements from the collection.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection to be consumed.</param>
        /// <returns>An enumerable that consumes elements from the collection and returns them.</returns>
        public static IEnumerable<T> GetConsumingEnumerable<T>(
            this IProducerConsumerCollection<T> collection)
        {
            T item;
            while (collection.TryTake(out item)) yield return item;
        }

        /// <summary>Adds the contents of an enumerable to the collection.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        /// <param name="target">The target collection to be augmented.</param>
        /// <param name="source">The source enumerable containing the data to be added.</param>
        public static void AddFromEnumerable<T>(this IProducerConsumerCollection<T> target, IEnumerable<T> source)
        {
            foreach (var item in source) target.TryAdd(item);
        }

        /// <summary>Adds the contents of an observable to the collection.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        /// <param name="target">The target collection to be augmented.</param>
        /// <param name="source">The source observable containing the data to be added.</param>
        /// <returns>An IDisposable that may be used to cancel the transfer.</returns>
        public static IDisposable AddFromObservable<T>(this IProducerConsumerCollection<T> target, IObservable<T> source)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (source == null) throw new ArgumentNullException("source");
            return source.Subscribe(new Nequeo.Collections.Common.DelegateBasedObserver<T>
            (
                onNext: item => target.TryAdd(item),
                onError: error => { },
                onCompleted: () => { }
            ));
        }

        /// <summary>Creates an add-only facade for the collection.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection to be wrapped.</param>
        /// <returns>
        /// An IProducerConsumerCollection that wraps the target collection and supports only add
        /// functionality, not take.
        /// </returns>
        public static IProducerConsumerCollection<T> ToProducerOnlyCollection<T>(this IProducerConsumerCollection<T> collection)
        {
            return new Nequeo.Collections.Extension.IProducerConsumerCollectionExtensions.ProduceOrConsumeOnlyCollection<T>(collection, true);
        }

        /// <summary>Creates a take-only facade for the collection.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection to be wrapped.</param>
        /// <returns>
        /// An IProducerConsumerCollection that wraps the target collection and supports only take
        /// functionality, not add.
        /// </returns>
        public static IProducerConsumerCollection<T> ToConsumerOnlyCollection<T>(this IProducerConsumerCollection<T> collection)
        {
            return new Nequeo.Collections.Extension.IProducerConsumerCollectionExtensions.ProduceOrConsumeOnlyCollection<T>(collection, false);
        }

        // Internal wrapper that throws NotSupportedException when mutating methods (add/take) are used from the wrong mode
        private sealed class ProduceOrConsumeOnlyCollection<T> : Nequeo.Collections.Concurrent.ProducerConsumerCollectionBase<T>
        {
            private readonly bool _produceOnly; // true for produce-only, false for consume-only

            public ProduceOrConsumeOnlyCollection(IProducerConsumerCollection<T> contained, bool produceOnly) :
                base(contained)
            {
                _produceOnly = produceOnly;
            }

            protected override bool TryAdd(T item)
            {
                if (!_produceOnly) throw new NotSupportedException();
                return base.TryAdd(item);
            }

            protected override bool TryTake(out T item)
            {
                if (_produceOnly) throw new NotSupportedException();
                return base.TryTake(out item);
            }
        }
    }
}
