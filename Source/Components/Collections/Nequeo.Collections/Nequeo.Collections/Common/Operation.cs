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

namespace Nequeo.Collections.Common
{
    /// <summary>
    /// Common operation handler
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Get the value of the first item within the collection.
        /// </summary>
        /// <typeparam name="TItemModel">Item type with the collection.</typeparam>
        /// <typeparam name="TResult">The result type to return.</typeparam>
        /// <param name="items">The collection of item model types</param>
        /// <param name="itemModelPropertyName">The item model property name to match</param>
        /// <returns>The first value within the collection.</returns>
        public static TResult GetFirstValue<TItemModel, TResult>(IList items, string itemModelPropertyName)
        {
            TResult value = default(TResult);
            if (items.Count > 0)
            {
                // Create the enumerator.
                System.Collections.IEnumerator dataObjects = items.GetEnumerator();

                // Iterate through the collection.
                while (dataObjects.MoveNext())
                {
                    // Get the current object.
                    object currentDataObject = dataObjects.Current;
                    TItemModel itemType = (TItemModel)currentDataObject;

                    // Get the property value.
                    value = ((TResult)itemType.GetType().GetProperty(itemModelPropertyName).GetValue(itemType, null));
                    break;
                }
                dataObjects.Reset();
            }
            
            // Return the value.
            return value;
        }
    }
}
