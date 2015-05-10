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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nequeo.Collections.Extension
{
    /// <summary>
    /// Class that extends the System.Collections.IList type.
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Get the value of the first item within the collection.
        /// </summary>
        /// <typeparam name="TItemModel">Item type with the collection.</typeparam>
        /// <typeparam name="TResult">The result type to return.</typeparam>
        /// <param name="source">The collection of item model types</param>
        /// <param name="itemModelPropertyName">The item model property name to match</param>
        /// <returns>The first value within the collection.</returns>
        public static TResult FirstValue<TItemModel, TResult>(this IList source, string itemModelPropertyName)
        {
            // Return the value.
            return Nequeo.Collections.Common.Operation.GetFirstValue<TItemModel, TResult>(source, itemModelPropertyName);
        }
    }
}
