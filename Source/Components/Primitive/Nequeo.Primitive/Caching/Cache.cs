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
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;

namespace Nequeo.Caching
{
    /// <summary>
    /// Class that contains the current runtime cache for
    /// the current application.
    /// </summary>
    public sealed class RuntimeCache
    {
        #region Runtime Caching
        private static double _cacheDuration = 60.0;
        private static HttpRuntime _httpRuntime = null;
        private static Object _lockObject = new object();

        /// <summary>
        /// Gets the caching object for the current application.
        /// </summary>
        public static Cache Cache
        {
            get { return HttpRuntime.Cache; }
        }

        /// <summary>
        /// Gets the number of items stored in the cache.
        /// </summary>
        public static int Count
        {
            get
            {
                // Return the cache count.
                System.Web.Caching.Cache dataCache = HttpRuntime.Cache;
                return dataCache != null ? dataCache.Count : 0;
            }
        }

        /// <summary>
        /// Gets the runtime queryable cached items.
        /// </summary>
        public static System.Linq.IQueryable QueryableItems
        {
            get
            {
                // Return the queryable items.
                System.Web.Caching.Cache dataCache = HttpRuntime.Cache;
                return dataCache != null ? dataCache.AsQueryable() : null;
            }
        }

        /// <summary>
        /// Gets sets, the number of seconds to cache the current item.
        /// </summary>
        public static double CacheDuration
        {
            get { return _cacheDuration; }
            set { _cacheDuration = value; }
        }

        /// <summary>
        /// Gets all the values within the cache.
        /// </summary>
        public static List<object> Values
        {
            get
            {
                // Does a cache object exists.
                if (HttpRuntime.Cache != null)
                {
                    // Get the cache object and the enumerator
                    // from the caching object.
                    System.Web.Caching.Cache dataCache = HttpRuntime.Cache;
                    IDictionaryEnumerator value = dataCache.GetEnumerator();

                    // Greate a new value collection list.
                    List<object> valueCol = new List<object>();

                    // For each value in the cache add
                    // the value to the collection list.
                    while (value.MoveNext())
                        valueCol.Add(value.Value);

                    // Return all the values else null.
                    return valueCol.Count > 0 ? valueCol : null;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets all the keys within the cache.
        /// </summary>
        public static List<string> Keys
        {
            get
            {
                // Does a cache object exists.
                if (HttpRuntime.Cache != null)
                {
                    // Get the cache object and the enumerator
                    // from the caching object.
                    System.Web.Caching.Cache dataCache = HttpRuntime.Cache;
                    IDictionaryEnumerator key = dataCache.GetEnumerator();

                    // Greate a new key collection list.
                    List<string> keyCol = new List<string>();

                    // For each key in the cache add
                    // the key to the collection list.
                    while (key.MoveNext())
                        keyCol.Add(key.Key as string);

                    // Return all the keys else null.
                    return keyCol.Count > 0 ? keyCol : null;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Searches for the current value in the cache.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>True if the value exists in the cache else false.</returns>
        public static bool ContainsValue(object value)
        {
            bool ret = false;

            // Does a cache object exists.
            if (HttpRuntime.Cache != null)
            {
                // Get the cache object and the enumerator
                // from the caching object.
                System.Web.Caching.Cache dataCache = HttpRuntime.Cache;
                IDictionaryEnumerator valueItem = dataCache.GetEnumerator();

                // For each value in the cache, if
                // the value is equal to the current
                // value in the cache.
                while (valueItem.MoveNext())
                    if (valueItem.Equals(value))
                    {
                        ret = true;
                        break;
                    }
            }

            // Return an indicator if the
            // value was found.
            return ret;
        }

        /// <summary>
        /// Searches for the current key in the cache.
        /// </summary>
        /// <param name="cacheKey">The key to search for.</param>
        /// <returns>True if the key exists in the cache else false.</returns>
        public static bool ContainsKey(string cacheKey)
        {
            bool ret = false;

            // Does a cache object exists.
            if (HttpRuntime.Cache != null)
            {
                // Get the cache object and the enumerator
                // from the caching object.
                System.Web.Caching.Cache dataCache = HttpRuntime.Cache;
                IDictionaryEnumerator keyItem = dataCache.GetEnumerator();

                // For each key in the cache, if
                // the key is equal to the current
                // key in the cache.
                while (keyItem.MoveNext())
                    if ((keyItem.Key as String) == cacheKey)
                    {
                        ret = true;
                        break;
                    }
            }

            // Return an indicator if the
            // key was found.
            return ret;
        }

        /// <summary>
        /// Gets all the cache keys that have the specified 
        /// text in the key identifier.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <returns>A list of all cache keys.</returns>
        public static List<string> GetKeyGroup(string text)
        {
            // Does a cache object exists.
            if (HttpRuntime.Cache != null)
            {
                // Get the cache object and the enumerator
                // from the caching object.
                System.Web.Caching.Cache dataCache = HttpRuntime.Cache;
                IDictionaryEnumerator key = dataCache.GetEnumerator();

                // Create a new instance of the list object.
                List<string> keyItems = new List<string>();

                // For each key in the cache add
                // the key to the collection list.
                while (key.MoveNext())
                    keyItems.Add(key.Key as String);

                // Search for the specified string.
                var query = from k in keyItems
                            where k.Contains(text) == true
                            select k;

                // Create a new instance of the list object.
                List<string> keys = new List<string>();

                // Add each result found.
                foreach (var result in query)
                    keys.Add(result.Trim());

                // Return all the keys else null.
                return keys.Count > 0 ? keys : null;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets the type of object in the cache.
        /// </summary>
        /// <param name="cacheKey">The cache key to get.</param>
        /// <returns>The type of object in the cache value.</returns>
        public static Type GetValueType(string cacheKey)
        {
            return HttpRuntime.Cache[cacheKey].GetType() ?? null;
        }

        /// <summary>
        /// Add the current child item to the cache.
        /// </summary>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.</param>
        /// <param name="value">The object the cache.</param>
        /// <param name="cacheDuration">The duration of the cache item.</param>
        public static void Add(string cacheKey, object value, double cacheDuration)
        {
            _cacheDuration = cacheDuration;
            Add("ModelObject", cacheKey, value, cacheDuration);
        }

        /// <summary>
        /// Add the current child item to the cache.
        /// </summary>
        /// <param name="modelObjectName">The model object name dependancy.</param>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.</param>
        /// <param name="value">The object the cache.</param>
        /// <param name="cacheDuration">The duration of the cache item.</param>
        public static void Add(string modelObjectName, string cacheKey, object value, double cacheDuration)
        {
            _cacheDuration = cacheDuration;

            // Does a cache object exists.
            if (HttpRuntime.Cache == null)
                CreateHttpRuntime();

            // Get the current application cache object.
            System.Web.Caching.Cache dataCache = HttpRuntime.Cache;

            // If the current dependancy does not exist
            // add the dependancy to the cache.
            if (dataCache[modelObjectName] == null)
                dataCache[modelObjectName] = DateTime.Now;

            // Create a new dependancy array
            // with the current model object
            // as the dependancy.
            string[] modelObjectNames = new string[1];
            modelObjectNames[0] = modelObjectName;

            // Add a new cache dependancy.
            System.Web.Caching.CacheDependency dependency =
                new System.Web.Caching.CacheDependency(null, modelObjectNames);

            // Indicates no expiery.
            if (cacheDuration < (double)0.0)
            {
                // Insert the current object into the cache.
                dataCache.Insert(cacheKey, value);
            }
            else
            {
                // Insert the current object into the cache.
                dataCache.Insert(cacheKey, value, dependency,
                    DateTime.UtcNow.AddSeconds(cacheDuration), System.Web.Caching.Cache.NoSlidingExpiration);
            }
        }

        /// <summary>
        /// Remove the current child item from the cached dependancy.
        /// </summary>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.</param>
        public static void Remove(string cacheKey)
        {
            // Does a cache object exists.
            if (HttpRuntime.Cache != null)
            {
                // Get the current cache object.
                // Remove the current cached item.
                System.Web.Caching.Cache dataCache = HttpRuntime.Cache;
                dataCache.Remove(cacheKey);
            }
        }

        /// <summary>
        /// Invalidate the current dependancy including all child items.
        /// </summary>
        /// <param name="key">The model object dependancy name or the cache key.</param>
        public static void Invalidate(string key)
        {
            // Remove the cache dependency.
            if (HttpRuntime.Cache != null)
                HttpRuntime.Cache.Remove(key);
        }

        /// <summary>
        /// Get the current cached object.
        /// </summary>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.</param>
        /// <returns>The current cached object.</returns>
        public static object Get(string cacheKey)
        {
            return HttpRuntime.Cache[cacheKey] ?? null;
        }

        /// <summary>
        /// Set the existing cached item with a new value.
        /// </summary>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.></param>
        /// <param name="value">The object the cache.</param>
        /// <param name="cacheDuration">The duration of the cache item.</param>
        public static void Set(string cacheKey, object value, double cacheDuration)
        {
            _cacheDuration = cacheDuration;

            if (!ContainsKey(cacheKey))
                Add(cacheKey, value, cacheDuration);
            else
                HttpRuntime.Cache[cacheKey] = value;
        }

        /// <summary>
        /// Cache enumerator, allows iteration through the cache.
        /// </summary>
        /// <returns>The current cached item.</returns>
        public static IEnumerator<DictionaryEntry> GetCacheItem()
        {
            // Does a cache object exists.
            if (HttpRuntime.Cache != null)
            {
                // Get the cache object and the enumerator
                // from the caching object.
                System.Web.Caching.Cache dataCache = HttpRuntime.Cache;
                IDictionaryEnumerator item = dataCache.GetEnumerator();

                // Iterate through the dictionary entry
                // and return the key value pair.
                while (item.MoveNext())
                    yield return item.Entry;
            }
        }

        /// <summary>
        /// Cache value enumerator, allows iteration through all values.
        /// </summary>
        /// <returns>The cache value item.</returns>
        public static IEnumerator<object> GetValue()
        {
            // Does a cache object exists.
            if (HttpRuntime.Cache != null)
            {
                // Get the cache object and the enumerator
                // from the caching object.
                System.Web.Caching.Cache dataCache = HttpRuntime.Cache;
                IDictionaryEnumerator value = dataCache.GetEnumerator();

                // Iterate through the dictionary entry
                // and return the value.
                while (value.MoveNext())
                    yield return value.Value;
            }
        }

        /// <summary>
        /// Cache key enumerator, allows iteration through all keys.
        /// </summary>
        /// <returns>The cache key item.</returns>
        public static IEnumerator<string> GetKey()
        {
            // Does a cache object exists.
            if (HttpRuntime.Cache != null)
            {
                // Get the cache object and the enumerator
                // from the caching object.
                System.Web.Caching.Cache dataCache = HttpRuntime.Cache;
                IDictionaryEnumerator key = dataCache.GetEnumerator();

                // Iterate through the dictionary entry
                // and return the key.
                while (key.MoveNext())
                    yield return key.Key as String;
            }
        }

        /// <summary>
        /// Get the unique name of the cached object.
        /// </summary>
        /// <param name="modelObjectName">The model object dependancy name.</param>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.</param>
        /// <returns>The unique name of the cached object.</returns>
        private static string GetCacheKey(string modelObjectName, string cacheKey)
        {
            return string.Concat(modelObjectName, "-", cacheKey);
        }

        /// <summary>
        /// Create a new HttpRuntime object.
        /// </summary>
        private static void CreateHttpRuntime()
        {
            // If the current runtime is null.
            if (_httpRuntime == null)
            {
                try
                {
                    // Get a lock on the current class.
                    Monitor.Enter(_lockObject);
                    if (_httpRuntime == null)
                    {
                        // Create an Http Content to give us access to the cache.
                        _httpRuntime = new HttpRuntime();
                    }
                }
                finally
                {
                    // Release the lock on the current class.
                    Monitor.Exit(_lockObject);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Class that contains the current custom cache for
    /// the current application.
    /// </summary>
    public sealed class CustomCache
    {
        #region Custom Caching
        private static Dictionary<string, object> _cache = null;

        /// <summary>
        /// Gets the caching object for the current application.
        /// </summary>
        public static Dictionary<string, object> Cache
        {
            get { return _cache ?? null; }
        }

        /// <summary>
        /// Gets all the values within the cache.
        /// </summary>
        public static ICollection<object> Values
        {
            get { return _cache != null ? _cache.Values : null; }
        }

        /// <summary>
        /// Gets all the keys within the cache.
        /// </summary>
        public static ICollection<string> Keys
        {
            get { return _cache != null ? _cache.Keys : null; }
        }

        /// <summary>
        /// Gets the number of items stored in the cache.
        /// </summary>
        public static int Count
        {
            get { return _cache != null ? _cache.Count : 0; }
        }

        /// <summary>
        /// Nullifies the current cache.
        /// </summary>
        public static void NullifyCache()
        {
            _cache = null;
        }

        /// <summary>
        /// Searches for the current key in the cache.
        /// </summary>
        /// <param name="cacheKey">The key to search for.</param>
        /// <returns>True if the key exists in the cache else false.</returns>
        public static bool ContainsKey(string cacheKey)
        {
            return _cache != null ? _cache.ContainsKey(cacheKey) : false;
        }

        /// <summary>
        /// Searches for the current value in the cache.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>True if the value exists in the cache else false.</returns>
        public static bool ContainsValue(object value)
        {
            return _cache != null ? _cache.ContainsValue(value) : false;
        }

        /// <summary>
        /// Gets the type of object in the cache.
        /// </summary>
        /// <param name="cacheKey">The cache key to get.</param>
        /// <returns>The type of object in the cache value.</returns>
        public static Type GetValueType(string cacheKey)
        {
            return _cache[cacheKey].GetType() ?? null;
        }

        /// <summary>
        /// Gets all the cache keys that have the specified 
        /// text in the key identifier.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <returns>A list of all cache keys.</returns>
        public static List<string> GetKeyGroup(string text)
        {
            if (_cache != null)
            {
                // Search for the specified string.
                var query = from k in _cache.Keys
                            where k.Contains(text) == true
                            select k;

                // Create a new instance of the list object.
                List<string> keys = new List<string>();

                // Add each result found.
                foreach (var result in query)
                    keys.Add(result.Trim());

                // Return all the keys else null.
                return keys.Count > 0 ? keys : null;
            }
            else
                return null;
        }

        /// <summary>
        /// Add the current child item to the cache.
        /// </summary>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.</param>
        /// <param name="value">The object the cache.</param>
        public static void Add(string cacheKey, object value)
        {
            // If no cache exists then create it.
            if (_cache == null)
                _cache = new Dictionary<string, object>();

            // If the cache key does not exists then add it.
            if (!ContainsKey(cacheKey))
                _cache.Add(cacheKey, value);
        }

        /// <summary>
        /// Get the current cached object.
        /// </summary>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.</param>
        /// <returns>The current cached object.</returns>
        public static object Get(string cacheKey)
        {
            return _cache[cacheKey] ?? null;
        }

        /// <summary>
        /// Set the exists cached item with a new value.
        /// </summary>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.></param>
        /// <param name="value">The object the cache.</param>
        public static void Set(string cacheKey, object value)
        {
            if (!ContainsKey(cacheKey))
                Add(cacheKey, value);
            else
                _cache[cacheKey] = value;
        }

        /// <summary>
        /// Remove the current child item from the cached dependancy.
        /// </summary>
        /// <param name="cacheKey">The specific item cache key to the model dependancy.</param>
        public static void Remove(string cacheKey)
        {
            if (_cache != null)
                _cache.Remove(cacheKey);
        }

        /// <summary>
        /// Cache enumerator, allows iteration through the cache.
        /// </summary>
        /// <returns>The current cached item.</returns>
        public static IEnumerator<KeyValuePair<string, object>> GetCacheItem()
        {
            if (_cache != null)
                foreach (KeyValuePair<string, object> keyValuePair in _cache)
                    yield return keyValuePair;
        }

        /// <summary>
        /// Cache value enumerator, allows iteration through all values.
        /// </summary>
        /// <returns>The cache value item.</returns>
        public static IEnumerator<object> GetValue()
        {
            if (_cache != null)
                foreach (KeyValuePair<string, object> keyValuePair in _cache)
                    yield return keyValuePair.Value;
        }

        /// <summary>
        /// Cache key enumerator, allows iteration through all keys.
        /// </summary>
        /// <returns>The cache key item.</returns>
        public static IEnumerator<string> GetKey()
        {
            if (_cache != null)
                foreach (KeyValuePair<string, object> keyValuePair in _cache)
                    yield return keyValuePair.Key;
        }

        /// <summary>
        /// Removes all the items stored in the cache.
        /// </summary>
        public static void Clear()
        {
            if (_cache != null)
                _cache.Clear();
        }
        #endregion
    }
}
