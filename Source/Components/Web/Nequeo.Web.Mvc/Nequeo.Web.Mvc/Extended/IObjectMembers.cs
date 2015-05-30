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
using System.ComponentModel;

namespace Nequeo.Web.Mvc.Extended
{
    /// <summary>
    /// The base type for all controls. allows override of object members
    /// when the ToString() method is called when rendering Html data to
    /// the underling HttpResponse output stream. Hides object members from the user.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IObjectMembers
    {
        /// <summary>
        /// Equalses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if the current object and the value object are equal.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object value);

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The unique hash code of the current object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <returns>The current object type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "GetType", Justification = "This should not be visible in auto complete list of VS, distracts when writing fluent syntax.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "In that case it is an issue of the .NET Framework itself")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        /// <summary>
        /// To string conversion the the object type.
        /// </summary>
        /// <returns>The string equivalent.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}
