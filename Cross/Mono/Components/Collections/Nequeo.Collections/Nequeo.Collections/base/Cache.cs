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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Collections
{
    /// <summary>
    /// Collection cache provider.
    /// </summary>
    public class Cache<Key, Value> : IDisposable
    {
        /// <summary>
        /// Collection cache provider.
        /// </summary>
        public Cache()
        {
            _cache = new Dictionary<Key, Value>();
        }

        private Dictionary<Key, Value> _cache = null;

        /// <summary>
        /// Gets all the values within the cache.
        /// </summary>
        public virtual ICollection<Value> Values
        {
            get { return _cache != null ? _cache.Values : null; }
        }

        /// <summary>
        /// Gets all the keys within the cache.
        /// </summary>
        public virtual ICollection<Key> Keys
        {
            get { return _cache != null ? _cache.Keys : null; }
        }

        /// <summary>
        /// Gets the number of items stored in the cache.
        /// </summary>
        public virtual int Count
        {
            get { return _cache != null ? _cache.Count : 0; }
        }

        /// <summary>
        /// Searches for the current key in the cache.
        /// </summary>
        /// <param name="cacheKey">The key to search for.</param>
        /// <returns>True if the key exists in the cache else false.</returns>
        public virtual bool ContainsKey(Key cacheKey)
        {
            return _cache != null ? _cache.ContainsKey(cacheKey) : false;
        }

        /// <summary>
        /// Searches for the current value in the cache.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>True if the value exists in the cache else false.</returns>
        public virtual bool ContainsValue(Value value)
        {
            return _cache != null ? _cache.ContainsValue(value) : false;
        }

        /// <summary>
        /// Gets the type of object in the cache.
        /// </summary>
        /// <param name="cacheKey">The cache key to get.</param>
        /// <returns>The type of object in the cache value.</returns>
        public virtual Type GetValueType(Key cacheKey)
        {
            return _cache[cacheKey].GetType() ?? null;
        }

        /// <summary>
        /// Add the current child item to the cache.
        /// </summary>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.</param>
        /// <param name="value">The object the cache.</param>
        public virtual void Add(Key cacheKey, Value value)
        {
            // If the cache key does not exists then add it.
            if (!ContainsKey(cacheKey))
                _cache.Add(cacheKey, value);
        }

        /// <summary>
        /// Get the current cached object.
        /// </summary>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.</param>
        /// <returns>The current cached object.</returns>
        public virtual Value Get(Key cacheKey)
        {
            return _cache[cacheKey];
        }

        /// <summary>
        /// Set the exists cached item with a new value.
        /// </summary>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.></param>
        /// <param name="value">The object the cache.</param>
        public virtual void Set(Key cacheKey, Value value)
        {
            if (!ContainsKey(cacheKey))
                Add(cacheKey, value);
            else
                _cache[cacheKey] = value;
        }

        /// <summary>
        /// Cache enumerator, allows iteration through the cache.
        /// </summary>
        /// <returns>The current cached item.</returns>
        public virtual IEnumerable<KeyValuePair<Key, Value>> GetCacheItem()
        {
            if (_cache != null)
                foreach (KeyValuePair<Key, Value> keyValuePair in _cache)
                    yield return keyValuePair;
        }

        /// <summary>
        /// Cache value enumerator, allows iteration through all values.
        /// </summary>
        /// <returns>The cache value item.</returns>
        public virtual IEnumerable<Value> GetValue()
        {
            if (_cache != null)
                foreach (KeyValuePair<Key, Value> keyValuePair in _cache)
                    yield return keyValuePair.Value;
        }

        /// <summary>
        /// Cache key enumerator, allows iteration through all keys.
        /// </summary>
        /// <returns>The cache key item.</returns>
        public virtual IEnumerable<Key> GetKey()
        {
            if (_cache != null)
                foreach (KeyValuePair<Key, Value> keyValuePair in _cache)
                    yield return keyValuePair.Key;
        }

        /// <summary>
        /// Removes all the items stored in the cache.
        /// </summary>
        public virtual void Clear()
        {
            if (_cache != null)
                _cache.Clear();
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

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
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_cache != null)
                        _cache.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _cache = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Cache()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
