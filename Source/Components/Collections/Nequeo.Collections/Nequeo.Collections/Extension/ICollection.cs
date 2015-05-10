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

namespace Nequeo.Collections.Extension
{
    /// <summary>
    /// ICollection interface extensions
    /// </summary>
    public static class ICollectionExtensions
    {
        /// <summary>
        /// Determines whether the specified collection instance is null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance to check.</param>
        /// <returns>
        /// <c>true</c> if the specified instance is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> instance)
        {
            // If the instance object is null.
            if (instance == null)
                throw new System.ArgumentNullException();

            return (instance == null) || (instance.Count == 0);
        }

        /// <summary>
        /// Determines whether the specified collection is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// <c>true</c> if the specified instance is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty<T>(this ICollection<T> instance)
        {
            // If the instance object is null.
            if (instance == null)
                throw new System.ArgumentNullException();

            return instance.Count == 0;
        }

        /// <summary>
        /// Adds the specified elements to the end of the System.Collections.Generic.ICollection&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance to add.</param>
        /// <param name="collection"> The collection whose elements should be added to the end of the ICollection&lt;T&gt;. 
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public static void AddRange<T>(this ICollection<T> instance, IEnumerable<T> collection)
        {
            // If the instance object is null.
            if (instance == null) throw new System.ArgumentNullException("instance");
            if (collection == null) throw new ArgumentNullException("collection");

            foreach (T item in collection)
            {
                instance.Add(item);
            }
        }
    }
}
