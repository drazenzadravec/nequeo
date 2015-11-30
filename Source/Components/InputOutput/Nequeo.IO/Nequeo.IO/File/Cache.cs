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

namespace Nequeo.IO.File
{
    /// <summary>
    /// File cache provider (thread-safe)).
    /// </summary>
    public class Cache : Nequeo.Collections.Cache<string, byte[]>
    {
        private object _lockObject = new object();
        private Dictionary<string, long> _fileSize = new Dictionary<string, long>();
        private Dictionary<string, DateTime> _fileModified = new Dictionary<string, DateTime>();

        /// <summary>
        /// Gets all the cache keys that have the specified 
        /// text in the key identifier.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <returns>A list of all cache keys.</returns>
        public List<string> GetKeyGroup(string text)
        {
            lock(_lockObject)
            {
                // Search for the specified string.
                var query = from k in base.Keys
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
        }

        /// <summary>
        /// Gets the file size.
        /// </summary>
        /// <param name="file">The file to get the sixe of.</param>
        /// <returns>The file size.</returns>
        public long GetFileSize(string file)
        {
            lock (_lockObject)
            {
                return _fileSize[file];
            }
        }

        /// <summary>
        /// Get the modified time.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>The modified time; else null.</returns>
        public DateTime? GetModifiedTime(string cacheKey)
        {
            lock (_lockObject)
            {
                if (_fileModified.ContainsKey(cacheKey))
                    return _fileModified[cacheKey];
                else
                    return null;
            }
        }

        /// <summary>
        /// Set the modified time.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="modifiedDate">The modified time.</param>
        public void SetModifiedTime(string cacheKey, DateTime modifiedDate)
        {
            lock (_lockObject)
            {
                if (_fileModified.ContainsKey(cacheKey))
                    _fileModified[cacheKey] = modifiedDate;
                else
                    _fileModified.Add(cacheKey, modifiedDate);
            }
        }

        /// <summary>
        /// Check if the file has been modified.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="modifiedDate">The current data time to compare with.</param>
        /// <returns>False if not modified; else true.</returns>
        public bool HasBeenModified(string cacheKey, DateTime modifiedDate)
        {
            lock (_lockObject)
            {
                if (_fileModified.ContainsKey(cacheKey))
                {
                    if (_fileModified[cacheKey].Equals(modifiedDate))
                        return false;
                    else
                        return true;
                }
                else
                    return true;
            }
        }

        /// <summary>
        /// Set the new value.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="value">The value.</param>
        public override void Set(string cacheKey, byte[] value)
        {
            lock (_lockObject)
            {
                base.Set(cacheKey, value);
                _fileSize[cacheKey] = (long)value.Length;
            }
        }

        /// <summary>
        /// Add the file.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="value">The value to add.</param>
        public override void Add(string cacheKey, byte[] value)
        {
            lock (_lockObject)
            {
                base.Add(cacheKey, value);
                _fileSize.Add(cacheKey, (long)value.Length);
            }
        }

        /// <summary>
        /// Gets the total size of the cache.
        /// </summary>
        /// <returns>The total cache size.</returns>
        public long GetCacheSize()
        {
            lock (_lockObject)
            {
                long size = 0;
                foreach (byte[] file in base.GetValue())
                    size += (long)file.Length;

                // Return the total size.
                return size;
            }
        }

        /// <summary>
        /// Dispose of all resources.
        /// </summary>
        /// <param name="disposing">Is disposing.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_fileSize != null)
                _fileSize.Clear();

            if (_fileModified != null)
                _fileModified.Clear();

            _fileSize = null;
            _fileModified = null;
            _lockObject = null;
        }
    }
}
