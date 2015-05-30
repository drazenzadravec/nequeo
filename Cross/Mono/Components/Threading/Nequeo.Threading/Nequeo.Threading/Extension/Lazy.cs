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
using System.Threading;
using System.Threading.Tasks;

namespace Nequeo.Threading.Extension
{
    /// <summary>
    /// Extension methods for Lazy.
    /// </summary>
    public static class LazyExtensions
    {
        /// <summary>Forces value creation of a Lazy instance.</summary>
        /// <typeparam name="T">Specifies the type of the value being lazily initialized.</typeparam>
        /// <param name="lazy">The Lazy instance.</param>
        /// <returns>The initialized Lazy instance.</returns>
        public static Lazy<T> Force<T>(this Lazy<T> lazy)
        {
            var ignored = lazy.Value;
            return lazy;
        }

        /// <summary>Retrieves the value of a Lazy asynchronously.</summary>
        /// <typeparam name="T">Specifies the type of the value being lazily initialized.</typeparam>
        /// <param name="lazy">The Lazy instance.</param>
        /// <returns>A Task representing the Lazy's value.</returns>
        public static Task<T> GetValueAsync<T>(this Lazy<T> lazy)
        {
            return Task.Factory.StartNew(() => lazy.Value);
        }

        /// <summary>Creates a Lazy that's already been initialized to a specified value.</summary>
        /// <typeparam name="T">The type of the data to be initialized.</typeparam>
        /// <param name="value">The value with which to initialize the Lazy instance.</param>
        /// <returns>The initialized Lazy.</returns>
        public static Lazy<T> Create<T>(T value)
        {
            return new Lazy<T>(() => value, false).Force();
        }
    }
}
