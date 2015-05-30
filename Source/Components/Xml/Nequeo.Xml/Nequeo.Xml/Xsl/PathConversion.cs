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

namespace Nequeo.Xml.Xsl
{
    /// <summary>
    /// Xsl path conversion helper. 
    /// </summary>
    public sealed class PathConversion
    {
        /// <summary>
        /// Transforms the collection of data to a html table format.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="collection">The collection of data to transform.</param>
        /// <param name="propertyNames">The property names that correspond to the element names</param>
        /// <param name="headerNames">The header names to display, can be null.</param>
        /// <returns>The transformed collection of data.</returns>
        public static string DataModel<T>(IEnumerable<T> collection, string[] propertyNames, string[] headerNames)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (collection == null) throw new ArgumentNullException("collection");
            if (propertyNames == null) throw new ArgumentNullException("propertyNames");

            string className = typeof(T).Name;
            string propertyHeaderNames = string.Empty;
            string propertyValueNames = string.Empty;

            // Construct the xsl value data for each property.
            foreach (string item in propertyNames)
                propertyValueNames += "<td>" + "<xsl:value-of select=\"" + item + "\"/>" + "</td>";

            // If no header names have been supplied
            // then use the property names.
            if (headerNames == null)
            {
                // Construct the xsl header data for each property.
                foreach (string item in propertyNames)
                    propertyHeaderNames += "<th>" + item + "</th>";
            }
            else
            {
                // Construct the xsl header data for each property.
                foreach (string item in headerNames)
                    propertyHeaderNames += "<th>" + item + "</th>";
            }

            // Create the root serialise context object.
            RootSerializableContextArray<T> array = new RootSerializableContextArray<T>();
            array.Items = collection.ToArray();

            // Serialise the collection.
            string xml = Encoding.UTF8.GetString(array.SerialiseDataEntity());

            // Construct the xslt from the Data Model Xslt
            string xslt = Nequeo.Resource.CustomToolXslt.DataModel.
                Replace("[ClassName]", className).
                Replace("[PropertyHeaderNames]", propertyHeaderNames).
                Replace("[PropertyValueNames]", propertyValueNames);

            // Return the transformed data.
            return Transformation.FromMemoryToMemory(xslt, xml);
        }

        /// <summary>
        /// Transforms the xml date time elements to a new xml format.
        /// </summary>
        /// <param name="xml">The xml to transform.</param>
        /// <param name="extension">The instance of the etension object that formats the date time.</param>
        /// <param name="extensionMethodName">The method in the extension object to call.</param>
        /// <param name="extensionParameters">The paramters to pass to the extension method.</param>
        /// <param name="matchingTemplatePath">The matching template path used to locate the date time elements.</param>
        /// <returns>The transformed collection of data.</returns>
        public static string DateFormat(string xml, object extension, 
            string extensionMethodName, string extensionParameters, string matchingTemplatePath)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (xml == null) throw new ArgumentNullException("xml");
            if (extension == null) throw new ArgumentNullException("extension");
            if (extensionMethodName == null) throw new ArgumentNullException("extensionMethodName");
            if (extensionParameters == null) throw new ArgumentNullException("extensionParameters");
            if (matchingTemplatePath == null) throw new ArgumentNullException("matchingTemplatePath");

            // Construct the xslt from the date format Xslt
            string xslt = Nequeo.Resource.CustomToolXslt.DateFormat.
                Replace("[TemplatePath]", matchingTemplatePath).
                Replace("[MethodName]", extensionMethodName).
                Replace("[Parameters]", extensionParameters);

            // Create a new argument list
            XsltArgumentList xslArgs = new XsltArgumentList();

            // Pass an instance of the date format object
            xslArgs.AddExtensionObject("urn:dateFormatType", extension);

            // Return the transformed data.
            return Transformation.FromMemoryToMemory(xslt, xml, xslArgs);
        }
    }
}
