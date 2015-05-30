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
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Nequeo.Data.Extension
{
    /// <summary>
    /// Class that extends the System.Ling.IQueryable type.
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Converts the System.Ling.IQueryable type
        /// to the specified type object.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="source">The current IQueryable type.</param>
        /// <returns>The ype array to return.</returns>
        public static T[] ToTypeArray<T>(this IQueryable source)
            where T : new()
        {
            // If the source object is null.
            if (source == null)
                throw new System.ArgumentNullException();

            Nequeo.Data.Control.AnonymousTypeFunction functions =
                new Nequeo.Data.Control.AnonymousTypeFunction();

            // Return the type array.
            return functions.GetTypeDataArray<T>(source);
        }
    }
}
