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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Nequeo.Custom
{
    /// <summary>
    /// Platform helper.
    /// </summary>
    public sealed class Platform
    {
        /// <summary>
        /// Platform helper.
        /// </summary>
        public Platform()
        {
        }

        /// <summary>
        /// Get new line.
        /// </summary>
        private static readonly string NewLine = GetNewLine();

        /// <summary>
        /// Get new line.
        /// </summary>
        /// <returns>The new line string.</returns>
        public static string GetNewLine()
        {
            return Environment.NewLine;
        }

        /// <summary>
        /// Compare string.
        /// </summary>
        /// <param name="a">String a.</param>
        /// <param name="b">String b.</param>
        /// <returns>The result.</returns>
        public static int CompareIgnoreCase(string a, string b)
        {
            return String.Compare(a, b, true);
        }

        /// <summary>
        /// Get environment variable.
        /// </summary>
        /// <param name="variable">The variable name.</param>
        /// <returns>The value.</returns>
        public static string GetEnvironmentVariable(string variable)
        {
            try
            {
                return Environment.GetEnvironmentVariable(variable);
            }
            catch (System.Security.SecurityException)
            {
                // We don't have the required permission to read this environment variable,
                // which is fine, just act as if it's not set
                return null;
            }
        }

        /// <summary>
        /// Create a not implemented exception.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <returns>NotImplementedException</returns>
        public static Exception CreateNotImplementedException(string message)
        {
            return new NotImplementedException(message);
        }

        /// <summary>
        /// Create an array list.
        /// </summary>
        /// <returns>The array list.</returns>
        public static System.Collections.IList CreateArrayList()
        {
            return new ArrayList();
        }

        /// <summary>
        /// Create an array list.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <returns>The array list.</returns>
        public static System.Collections.IList CreateArrayList(int capacity)
        {
            return new ArrayList(capacity);
        }
    }
}
