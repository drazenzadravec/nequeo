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
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using Nequeo.Serialisation;
using Nequeo.Serialisation.Extension;

namespace Nequeo.Xml
{
    /// <summary>
    /// The root serializable container for any generic type.
    /// </summary>
    /// <typeparam name="T">The type to examine.</typeparam>
    [XmlRoot("Root")]
    [SerializableAttribute()]
    public class RootSerializableContext<T>
    {
        /// <summary>
        /// The types to serialise within this root container.
        /// </summary>
        [XmlElement("Context")]
        public T Item;
    }

    /// <summary>
    /// The root serializable container for any generic type.
    /// </summary>
    /// <typeparam name="T">The type to examine.</typeparam>
    [XmlRoot("Root")]
    [SerializableAttribute()]
    public class RootSerializableContextArray<T>
    {
        /// <summary>
        /// The array of types to serialise within this root container.
        /// </summary>
        [XmlArray]
        public T[] Items;

    }

    /// <summary>
    /// The root serializable container for any generic type.
    /// </summary>
    /// <typeparam name="T1">The type to examine.</typeparam>
    /// <typeparam name="T2">The type to examine.</typeparam>
    [XmlRoot("Root")]
    [SerializableAttribute()]
    public class RootSerializableContextArray<T1, T2>
    {
        /// <summary>
        /// The array of types to serialise within this root container.
        /// </summary>
        [XmlArray]
        public T1[] Items1;

        /// <summary>
        /// The array of types to serialise within this root container.
        /// </summary>
        [XmlArray]
        public T2[] Items2;

    }

    /// <summary>
    /// Date time formatter provider
    /// </summary>
    public class DateTimeFormatter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="format">A date time format.</param>
        public DateTimeFormatter(string format)
        {
            _format = format;
        }

        private string _format = string.Empty;

        /// <summary>
        /// Change the date time to the format type.
        /// </summary>
        /// <param name="xslDateTime">The date time data to format.</param>
        /// <returns>The new formatted date time</returns>
        public string ChangeFormat(string xslDateTime)
        {
            DateTime dateTime = DateTime.Parse(xslDateTime);
            return dateTime.ToString(_format);
        }
    }
}
