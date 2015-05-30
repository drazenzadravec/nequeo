/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.IO;

namespace Nequeo.Net.OAuth.Framework.Utility
{
    /// <summary>
    /// String helper
    /// </summary>
    public static class StringUtility
    {
        /// <summary>
        /// Equals in constant time
        /// </summary>
        /// <param name="value">The source value</param>
        /// <param name="other">The other string value.</param>
        /// <returns>True if equals in constant time; else false.</returns>
        public static bool EqualsInConstantTime(this string value, string other)
        {
            if (value == null ^ other == null) return false;
            if (value == null) return true;
            if (value.Length != other.Length) return false;

            return CompareStringsInConstantTime(value, other);
        }

        /// <summary>
        /// Compare strings in constant time
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="other">The compare value.</param>
        /// <returns>True if strings In constant time; else false.</returns>
        static bool CompareStringsInConstantTime(string value, string other)
        {
            int result = 0;

            for (int i = 0; i < value.Length; i++)
            {
                result |= value[i] ^ other[i];
            }

            return result == 0;
        }
    }
}
