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

using System.Diagnostics;
using System.Web;
using System.Web.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Nequeo.Collections.Extension;

namespace Nequeo.Web.Mvc.Extended.Factory
{
    /// <summary>
    /// Encapsulates the System.Web.HttpRuntime.Cache object that contains methods for accessing System.Web.HttpRuntime.Cache object.
    /// </summary>
    public class CacheManagerWrapper : ICacheManager
    {
        /// <summary>
        /// Retrieves the specified item from the System.Web.HttpRuntime.Cache object.
        /// </summary>
        /// <param name="key">The object to be retrives from the cache.</param>
        /// <returns></returns>
        public object GetItem(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        /// <summary>
        /// Inserts an object into the System.Web.Caching.Cache object with dependencies and a delegate you can use to notify your application when the inserted item is removed from the Cache.
        /// </summary>
        /// <param name="key">The object to be inserted in the cache.</param>
        /// <param name="value">The file or cache key dependencies for the item. When any dependency changes, the object becomes invalid and is removed from the cache. If there are no dependencies, this parameter contains null.</param>
        /// <param name="onRemoveCallback">A delegate that, if provided, will be called when an object is removed from the cache. You can use this to notify applications when their objects are deleted from the cache.</param>
        /// <param name="fileDependencies">List of files that the cache item depends upon, if any of the file is changed the cache item will become invalid.</param>
        public void Insert(string key, object value, CacheItemRemovedCallback onRemoveCallback, params string[] fileDependencies)
        {
            HttpRuntime.Cache.Insert(key, value, fileDependencies.IsNullOrEmpty() ? null : 
                new CacheDependency(fileDependencies), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, onRemoveCallback);
        }
    }
}
