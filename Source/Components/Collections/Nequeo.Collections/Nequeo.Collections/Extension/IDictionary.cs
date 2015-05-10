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
using System.Collections.Generic;
using System.Xml.Linq;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Nequeo.Collections.Extension
{
    /// <summary>
    /// Contains extension methods of IDictionary&lt;string, objectT&gt;.
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Merges the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="replaceExisting">if set to <c>true</c> [replace existing].</param>
        public static void Merge(this IDictionary<string, object> instance, string key, object value, bool replaceExisting)
        {
            // If the instance object is null.
            if (instance == null) throw new System.ArgumentNullException("instance");
            if (key == null) throw new System.ArgumentNullException("key");
            if (value == null) throw new System.ArgumentNullException("value");

            if (replaceExisting || !instance.ContainsKey(key))
            {
                instance[key] = value;
            }
        }

        /// <summary>
        /// Appends the in value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="key">The key.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="value">The value.</param>
        public static void AppendInValue(this IDictionary<string, object> instance, string key, string separator, object value)
        {
            // If the instance object is null.
            if (instance == null) throw new System.ArgumentNullException("instance");
            if (key == null) throw new System.ArgumentNullException("key");
            if (value == null) throw new System.ArgumentNullException("value");
            if (separator == null) throw new System.ArgumentNullException("separator");

            instance[key] = instance.ContainsKey(key) ? instance[key] + separator + value : value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddStyleAttribute(this IDictionary<string, object> instance, string key, string value)
        {
            // If the instance object is null.
            if (instance == null) throw new System.ArgumentNullException("instance");
            if (key == null) throw new System.ArgumentNullException("key");
            if (value == null) throw new System.ArgumentNullException("value");

            string style = string.Empty;

            if (instance.ContainsKey("style"))
                style = (string)instance["style"];

            StringBuilder builder = new StringBuilder(style);
            builder.Append(key);
            builder.Append(":");
            builder.Append(value);
            builder.Append(";");

            instance["style"] = builder.ToString();
        }
        /// <summary>
        /// Appends the specified value at the beginning of the existing value
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="key"></param>
        /// <param name="separator"></param>
        /// <param name="value"></param>
        public static void PrependInValue(this IDictionary<string, object> instance, string key, string separator, object value)
        {
            // If the instance object is null.
            if (instance == null) throw new System.ArgumentNullException("instance");
            if (key == null) throw new System.ArgumentNullException("key");
            if (value == null) throw new System.ArgumentNullException("value");
            if (separator == null) throw new System.ArgumentNullException("separator");

            instance[key] = instance.ContainsKey(key) ? value + separator + instance[key] : value.ToString();
        }

        /// <summary>
        /// Merges the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="from">From.</param>
        /// <param name="replaceExisting">if set to <c>true</c> [replace existing].</param>
        public static void Merge(this IDictionary<string, object> instance, IDictionary<string, object> from, bool replaceExisting)
        {
            // If the instance object is null.
            if (instance == null) throw new System.ArgumentNullException("instance");
            if (from == null) throw new System.ArgumentNullException("from");

            foreach (KeyValuePair<string, object> pair in from)
            {
                if (!replaceExisting && instance.ContainsKey(pair.Key))
                {
                    continue; // Try the next
                }

                instance[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Merges the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="from">From.</param>
        public static void Merge(this IDictionary<string, object> instance, IDictionary<string, object> from)
        {
            Merge(instance, from, true);
        }

        /// <summary>
        /// Merges the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="values">The values.</param>
        /// <param name="replaceExisting">if set to <c>true</c> [replace existing].</param>
        public static void Merge(this IDictionary<string, object> instance, object values, bool replaceExisting)
        {
            Merge(instance, new RouteValueDictionary(values), replaceExisting);
        }

        /// <summary>
        /// Merges the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="values">The values.</param>
        public static void Merge(this IDictionary<string, object> instance, object values)
        {
            Merge(instance, values, true);
        }

        /// <summary>
        /// Adds the value from an XDocument with the specified element name if it's not empty.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary. 
        /// </param>
        /// <param name="document">
        /// The document. 
        /// </param>
        /// <param name="elementName">
        /// Name of the element. 
        /// </param>
        public static void AddDataIfNotEmpty(
            this Dictionary<string, string> dictionary, XDocument document, string elementName)
        {
            var element = document.Root.Element(elementName);
            if (element != null)
                dictionary.AddItemIfNotEmpty(elementName, element.Value);
            
        }

        /// <summary>
        /// Adds a key/value pair to the specified dictionary if the value is not null or empty.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary. 
        /// </param>
        /// <param name="key">
        /// The key. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        public static void AddItemIfNotEmpty(this IDictionary<string, string> dictionary, string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (!string.IsNullOrEmpty(value))
                dictionary[key] = value;
            
        }
    }
}
