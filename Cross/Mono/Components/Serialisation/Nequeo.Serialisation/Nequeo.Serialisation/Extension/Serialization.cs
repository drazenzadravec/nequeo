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

namespace Nequeo.Serialisation.Extension
{
    /// <summary>
    /// Class that extends the serialization functionallity.
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Serialise the current object type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The current source object type.</param>
        /// <returns>The byte array of serialised data.</returns>
        public static byte[] SerialiseXml<T>(this T source)
        {
            GenericSerialisation<T> seril = new GenericSerialisation<T>();
            return seril.Serialise(source);
        }

        /// <summary>
        /// Serialise the current object type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The current source object type.</param>
        /// <returns>The string of serialised data.</returns>
        public static string SerialiseJson<T>(this T source)
        {
            return JavaObjectNotation.JavaScriptSerializer(source);
        }

        /// <summary>
        /// Deserialise the current object type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The current source object type.</param>
        /// <returns>The deserialised obejct type.</returns>
        public static T DeserialiseXml<T>(this byte[] source)
        {
            GenericSerialisation<T> seril = new GenericSerialisation<T>();
            return seril.Deserialise(source);
        }

        /// <summary>
        /// Deserialise the current object type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The current source object type.</param>
        /// <returns>The deserialised obejct type.</returns>
        public static T DeserialiseXml<T>(this string source)
        {
            GenericSerialisation<T> seril = new GenericSerialisation<T>();
            byte[] xml = Encoding.Default.GetBytes(source);
            return seril.Deserialise(xml);
        }

        /// <summary>
        /// Deserialise the current object type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The current source object type.</param>
        /// <returns>The deserialised obejct type.</returns>
        public static T DeserialiseJson<T>(this byte[] source)
        {
            string json = Encoding.Default.GetString(source);
            return JavaObjectNotation.JavaScriptDeserializer<T>(json);
        }

        /// <summary>
        /// Deserialise the current object type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The current source object type.</param>
        /// <returns>The deserialised obejct type.</returns>
        public static T DeserialiseJson<T>(this string source)
        {
            return JavaObjectNotation.JavaScriptDeserializer<T>(source);
        }

        /// <summary>
        /// Serialise the current object type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The current source object type.</param>
        /// <returns>The byte array of serialised data.</returns>
        public static byte[] SerialiseDataEntity<T>(this T source)
            where T : class, new()
        {
            GenericSerialisation<T> seril = new GenericSerialisation<T>();
            return seril.Serialise(source);
        }

        /// <summary>
        /// Deserialise the current object type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The current source object type.</param>
        /// <returns>The deserialised obejct type.</returns>
        public static T DeserialiseDataEntity<T>(this byte[] source)
            where T : class, new()
        {
            GenericSerialisation<T> seril = new GenericSerialisation<T>();
            return seril.Deserialise(source);
        }

        /// <summary>
        /// Deserialise the current object type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="source">The current source object type.</param>
        /// <returns>The deserialised obejct type.</returns>
        public static T DeserialiseDataEntity<T>(this string source)
            where T : class, new()
        {
            GenericSerialisation<T> seril = new GenericSerialisation<T>();
            byte[] xml = Encoding.Default.GetBytes(source);
            return seril.Deserialise(xml);
        }
    }
}
