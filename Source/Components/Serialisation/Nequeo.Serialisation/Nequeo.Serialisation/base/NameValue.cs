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
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;

namespace Nequeo.Serialisation
{
    /// <summary>
    /// Name value collection serialisation.
    /// </summary>
    public class NameValue
    {
        /// <summary>
        /// Deserializer the name value collection.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <param name="collection">The collection of name values.</param>
        /// <returns>The type containing the data.</returns>
        public static T Deserializer<T>(NameValueCollection collection)
        {
            // Get all the properties within the type.
            // Create a new instance of the type.
            PropertyInfo[] properties = typeof(T).GetProperties();
            T dataObject = Nequeo.Reflection.TypeAccessor.CreateInstance<T>();

            // Iterate through the collection.
            foreach (string item in collection.AllKeys)
            {
                PropertyInfo propertyInfo = null;
                try
                {
                    // Get the property.
                    propertyInfo = properties.First(p => p.Name.ToLower() == item.ToLower());
                }
                catch { }
                if (propertyInfo != null)
                {
                    // Set the property value.
                    propertyInfo.SetValue(dataObject, collection[item]);
                }
            }

            // Return the T object type.
            return dataObject;
        }

        /// <summary>
        /// Serializer the name value collection.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <param name="data">The type to serialise.</param>
        /// <returns>The name value collection.</returns>
        public static NameValueCollection Serializer<T>(T data)
        {
            NameValueCollection col = new NameValueCollection();

            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo item in properties)
            {
                // Get the value from the type.
                object value = item.GetValue(data);

                // Add the value to the colelction.
                col.Add(item.Name.ToLower(), value.ToString());
            }

            // Return the collection
            return col;
        }
    }
}
