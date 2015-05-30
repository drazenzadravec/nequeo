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
using System.Threading.Tasks;

namespace Nequeo.Net
{
    /// <summary>
    /// State context manager interface
    /// </summary>
    public interface IStateContextManager : IDisposable, IContextManager
    {
        /// <summary>
        /// Add the state context item.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="context">The state context to add to the list.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        void Add(string uniqueIdentifier, object context);

        /// <summary>
        /// Remove the state context item.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        void Remove(string uniqueIdentifier);

        /// <summary>
        /// Find the state context.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <returns>The state context; else null.</returns>
        object Find(string uniqueIdentifier);

        /// <summary>
        /// Find state context list for the unique identifiers.
        /// </summary>
        /// <param name="uniqueIdentifiers">The list of unique identifiers.</param>
        /// <returns>The state context list; else null.</returns>
        object[] Find(string[] uniqueIdentifiers);

        /// <summary>
        /// Find all state context list.
        /// </summary>
        /// <returns>The state context list; else null.</returns>
        object[] FindAll();

    }
}
