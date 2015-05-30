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
using System.Threading;

using Nequeo.Threading.Extension;

namespace Nequeo.Web.Mvc.Extended.Factory
{
    /// <summary>
    /// Custome cache provider for Mvc runtime.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public abstract class CacheBase<TKey, TValue> : Nequeo.Runtime.DisposableBase
    {
        private readonly IDictionary<TKey, TValue> _cache;
        private readonly ReaderWriterLockSlim _syncLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="comparer">The object comparer</param>
        protected CacheBase(IEqualityComparer<TKey> comparer)
        {
            _cache = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected CacheBase()
            : this(null)
        {
        }

        /// <summary>
        /// Dispose of the object.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _cache.Clear();
            _syncLock.Dispose();
        }

        /// <summary>
        /// Get or create the cache item.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <param name="factory">The function to execute with the value.</param>
        /// <returns>The original value passed in the factory parameter.</returns>
        protected TValue GetOrCreate(TKey key, Func<TValue> factory)
        {
            // If the instance object is null.
            if (key == null) throw new System.ArgumentNullException("key");
            if (factory == null) throw new System.ArgumentNullException("factory");

            TValue value;

            using (_syncLock.ReadAndWrite())
            {
                if (!_cache.TryGetValue(key, out value))
                {
                    using (_syncLock.Write())
                    {
                        if (!_cache.TryGetValue(key, out value))
                        {
                            value = factory();
                            _cache.Add(key, value);
                        }
                    }
                }
            }

            // Return the original value.
            return value;
        }
    }

    /// <summary>
    /// Provides an attribute to change the enum value for client side.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ClientSideEnumValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSideEnumValueAttribute"/> class with the specified value for the client side.
        /// </summary>
        /// <param name="value">The value.</param>
        public ClientSideEnumValueAttribute(string value)
        {
            // If the instance object is null.
            if (value == null) throw new System.ArgumentNullException("value");

            Value = value;
        }

        /// <summary>
        /// Gets or sets the value for client side.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get; private set;
        }
    }
}
