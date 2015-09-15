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
    /// Contains the values that indicate whether the byte order is a Little-endian or Big-endian.
    /// </summary>
    public enum ByteOrder : byte
    {
        /// <summary>
        /// Indicates a Little-endian.
        /// </summary>
        Little,
        /// <summary>
        /// Indicates a Big-endian.
        /// </summary>
        Big,
    }

    /// <summary>
    /// Custom extensions
    /// </summary>
    public static class CustomExtensions
    {
        /// <summary>
        /// Determines whether the specified <see cref="ByteOrder"/> is host (this computer
        /// architecture) byte order.
        /// </summary>
        /// <returns>
        /// <c>true</c> if <paramref name="order"/> is host byte order; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="order">
        /// One of the <see cref="ByteOrder"/> enum values, to test.
        /// </param>
        public static bool IsHostOrder(this ByteOrder order)
        {
            // true : !(true ^ true)  or !(false ^ false)
            // false: !(true ^ false) or !(false ^ true)
            return !(BitConverter.IsLittleEndian ^ (order == ByteOrder.Little));
        }
    }
}
