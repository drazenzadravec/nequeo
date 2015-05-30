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
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using Nequeo.Xml.Xsl;
using Nequeo.Serialisation;
using Nequeo.Serialisation.Extension;

namespace Nequeo.Xml
{
    /// <summary>
    /// Xml document client.
    /// </summary>
    public sealed class Document
    {
        /// <summary>
        /// Serialise the data of type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="data">The data to transform.</param>
        /// <returns>The serialised xml.</returns>
        public static string Serialise<T>(T data)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (data == null) throw new ArgumentNullException("data");

            // Create the root serialise context object.
            RootSerializableContext<T> array = new RootSerializableContext<T>();

            // Serialise the collection.
            string xml = Encoding.Default.GetString(array.SerialiseXml());
            return xml;
        }

        /// <summary>
        /// Convert all date time elements in the xml to the format type.
        /// </summary>
        /// <param name="xml">The xml to transform.</param>
        /// <param name="format">The date time format to replace the element with.</param>
        /// <param name="dateTimeElementName">The date time element in the xml to convert.</param>
        /// <param name="matchingTemplatePath">The matching template path used to locate the date time elements.</param>
        /// <returns>The new xml with all date time elements converted.</returns>
        public static string ConvertDateTime(string xml, string format, 
            string dateTimeElementName, string matchingTemplatePath)
        {
            DateTimeFormatter formatter = new DateTimeFormatter(format);
            return PathConversion.DateFormat(xml, formatter, "ChangeFormat", "./" + dateTimeElementName, matchingTemplatePath);
        }

        /// <summary>
        /// Convert all date time elements in the xml to the format type.
        /// </summary>
        /// <param name="xml">The xml to transform.</param>
        /// <param name="format">The date time format to replace the element with.</param>
        /// <param name="matchingTemplatePath">The full matching template path used to locate the date time elements.</param>
        /// <returns>The new xml with all date time elements converted.</returns>
        public static string ConvertDateTime(string xml, string format, string matchingTemplatePath)
        {
            DateTimeFormatter formatter = new DateTimeFormatter(format);
            return PathConversion.DateFormat(xml, formatter, "ChangeFormat", ".", matchingTemplatePath);
        }

        /// <summary>
        /// Create the xml document within the xml file.
        /// </summary>
        /// <param name="node">The xml node to create.</param>
        /// <param name="xmlFilePath">The file name to write to.</param>
        /// <param name="version">The xml version.</param>
        /// <param name="encoding">The xml encoding</param>
        /// <param name="standalone">The xml standalone</param>
        /// <returns>True if the xml was saved; else false</returns>
        public static bool CreateDocument(XElement node, string xmlFilePath, string version, string encoding, string standalone)
        {
            // If the directory does not exist then create it.
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(xmlFilePath)))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(xmlFilePath));

            // Create the top level xml document
            // and save the document to the xml file
            XDocument xDoc = new XDocument(
                new XDeclaration(version, encoding, standalone), node);
            xDoc.Save(xmlFilePath);

            // Return true if at this point.
            return true;
        }

        /// <summary>
        /// Create the xml document.
        /// </summary>
        /// <param name="node">The xml node to create.</param>
        /// <param name="version">The xml version.</param>
        /// <param name="encoding">The xml encoding</param>
        /// <param name="standalone">The xml standalone</param>
        /// <returns>The xml document.</returns>
        public static XDocument CreateDocument(XElement node, string version, string encoding, string standalone)
        {
            // Create the top level xml document
            // and save the document to the xml file
            XDocument xDoc = new XDocument(
                new XDeclaration(version, encoding, standalone), node);

            // Return true if at this point.
            return xDoc;
        }

        /// <summary>
        /// Create the xml document.
        /// </summary>
        /// <param name="table">The XmlNameTable to use.</param>
        /// <returns>The xml document.</returns>
        public static XmlDocument CreateDocument(XmlNameTable table)
        {
            // Create the top level xml document
            // and save the document to the xml file
            XmlDocument xDoc = new XmlDocument(table);

            // Return true if at this point.
            return xDoc;
        }

        /// <summary>
        /// Load the xml document.
        /// </summary>
        /// <param name="xmlString">The xml string to load.</param>
        /// <returns>The XDocument containing the document.</returns>
        public static XDocument LoadDocumentParseXmlString(string xmlString)
        {
            XDocument xDoc = XDocument.Parse(xmlString);
            return xDoc;
        }

        /// <summary>
        /// Load the xml document.
        /// </summary>
        /// <param name="xmlFilePath">The xml file to load.</param>
        /// <param name="xmlString">The xml string to load.</param>
        /// <param name="stream">A stream object to load.</param>
        /// <param name="textReader">A text reader object to load.</param>
        /// <param name="xmlReader">A xml reader object to load.</param>
        /// <returns>The XDocument containing the document.</returns>
        public static XDocument LoadDocument(
            string xmlFilePath = null, 
            string xmlString = null, 
            Stream stream = null, 
            TextReader textReader = null,
            XmlReader xmlReader = null)
        {
            XDocument xDoc = null;

            if(!String.IsNullOrEmpty(xmlFilePath))
                xDoc = XDocument.Load(xmlFilePath);

            if (!String.IsNullOrEmpty(xmlString))
                xDoc = XDocument.Load(new StringReader(xmlString));

            if (stream != null)
                xDoc = XDocument.Load(stream);

            if (textReader != null)
                xDoc = XDocument.Load(textReader);

            if (xmlReader != null)
                xDoc = XDocument.Load(xmlReader);

            return xDoc;
        }

        /// <summary>
        /// Load the xml document.
        /// </summary>
        /// <param name="xmlFilePath">The xml file to load.</param>
        /// <returns>The XDocument containing the document.</returns>
        public static XDocument LoadDocumentXmlFilePath(string xmlFilePath)
        {
            return LoadDocument(xmlFilePath: xmlFilePath);
        }

        /// <summary>
        /// Load the xml document.
        /// </summary>
        /// <param name="xmlString">The xml string to load.</param>
        /// <returns>The XDocument containing the document.</returns>
        public static XDocument LoadDocumentXmlString(string xmlString)
        {
            return LoadDocument(xmlString: xmlString);
        }

        /// <summary>
        /// Load the xml document.
        /// </summary>
        /// <param name="stream">A stream object to load.</param>
        /// <returns>The XDocument containing the document.</returns>
        public static XDocument LoadDocumentStream(Stream stream)
        {
            return LoadDocument(stream: stream);
        }

        /// <summary>
        /// Load the xml document.
        /// </summary>
        /// <param name="textReader">A text reader object to load.</param>
        /// <returns>The XDocument containing the document.</returns>
        public static XDocument LoadDocumentTextReader(TextReader textReader)
        {
            return LoadDocument(textReader: textReader);
        }

        /// <summary>
        /// Load the xml document.
        /// </summary>
        /// <param name="xmlReader">A xml reader object to load.</param>
        /// <returns>The XDocument containing the document.</returns>
        public static XDocument LoadDocumentXmlReader(XmlReader xmlReader)
        {
            return LoadDocument(xmlReader: xmlReader);
        }

        /// <summary>
        /// Extract the content only from the document.
        /// </summary>
        /// <param name="xDocument">The XDocument to search in.</param>
        /// <returns>The extracted content data.</returns>
        public static string ExtractContent(XDocument xDocument)
        {
            StringBuilder builder = new StringBuilder();

            XElement root = xDocument.Root;

            if (!String.IsNullOrEmpty(root.Value))
                builder.Append(root.Value + "\r\n");

            if (root.HasElements) ExtractContent(builder, root);

            // Return the content.
            return builder.ToString();
        }

        /// <summary>
        /// Extract content interator.
        /// </summary>
        /// <param name="builder">The string builder to store the content in.</param>
        /// <param name="element">The current element.</param>
        private static void ExtractContent(StringBuilder builder, XElement element)
        {
            // For each child element.
            foreach (XElement item in element.Elements())
            {
                if (!String.IsNullOrEmpty(item.Value))
                    builder.Append(item.Value + "\r\n");

                if (item.HasElements) ExtractContent(builder, item);
            }
        }

        /// <summary>
        /// Extract the content only from the document.
        /// </summary>
        /// <param name="xDocument">The XDocument to search in.</param>
        /// <returns>The extracted content data.</returns>
        public static string ExtractContent(XmlDocument xDocument)
        {
            StringBuilder builder = new StringBuilder();

            XmlNode root = xDocument.DocumentElement;

            if (!String.IsNullOrEmpty(root.Value))
                builder.Append(root.Value + "\r\n");

            if (root.HasChildNodes) ExtractContent(builder, root);

            // Return the content.
            return builder.ToString();
        }

        /// <summary>
        /// Extract content interator.
        /// </summary>
        /// <param name="builder">The string builder to store the content in.</param>
        /// <param name="element">The current element.</param>
        private static void ExtractContent(StringBuilder builder, XmlNode element)
        {
            IEnumerator ienum = element.GetEnumerator();

            // For each child element.
            while (ienum.MoveNext()) 
            {
                XmlNode item = (XmlNode)ienum.Current;

                if (!String.IsNullOrEmpty(item.Value))
                    builder.Append(item.Value + "\r\n");

                if (item.HasChildNodes) ExtractContent(builder, item);
            }
        }
    }
}
