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
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Data.Common;
using System.ComponentModel;

using Nequeo.Cryptography;
using Nequeo.Serialisation;

namespace Nequeo.Data.Control
{
    /// <summary>
    /// Common operation type.
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Get the current SHA1 hashcode for the data type
        /// </summary>
        /// <typeparam name="TData">The data type to examine</typeparam>
        /// <param name="data">The data to get the hash code for.</param>
        /// <returns>The hash code for the data.</returns>
        public static string GetHashCode<TData>(TData data)
        {
            GenericSerialisation<TData> serialise = new GenericSerialisation<TData>();
            string xml = System.Text.Encoding.UTF8.GetString(serialise.Serialise(data));
            string hashCode = Nequeo.Cryptography.Hashcode.GetHashcodeSHA1(xml);
            return hashCode;
        }

        /// <summary>
        /// Get the current SHA1 hashcode for the data type
        /// </summary>
        /// <param name="data">The data to get the hash code for.</param>
        /// <returns>The hash code for the data.</returns>
        public static string GetHashCode(object data)
        {
            GeneralSerialisation serialise = new GeneralSerialisation();
            string xml = System.Text.Encoding.UTF8.GetString(serialise.Serialise(data, data.GetType()));
            string hashCode = Nequeo.Cryptography.Hashcode.GetHashcodeSHA1(xml);
            return hashCode;
        }

        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <typeparam name="TData">The data type to examine</typeparam>
        /// <param name="original">The original data</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        public static bool HasChanged<TData>(TData original, TData current)
        {
            string originalHashCode = GetHashCode<TData>(original);
            string currentHashCode = GetHashCode<TData>(current);

            if (originalHashCode == currentHashCode)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <param name="originalHashCode">The original hash code</param>
        /// <param name="currentHashCode">The current hash code.</param>
        /// <returns>True if the data is different; else false.</returns>
        public static bool HasChanged(string originalHashCode, string currentHashCode)
        {
            if (originalHashCode == currentHashCode)
                return false;
            else
                return true;
        }
    }
}
