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

namespace Nequeo.Xml.Xsl
{
    /// <summary>
    /// Xsl UTF8 transformation
    /// </summary>
    public sealed class Transformation
    {
        #region UTF8 Transformation
        /// <summary>
        /// Transform from file to output file.
        /// </summary>
        /// <param name="xslFile">The xsl file transform.</param>
        /// <param name="xmlFile">The xml file to transform.</param>
        /// <param name="outputFile">The file to write the transformed result to.</param>
        public static void FromFileToFile(string xslFile, string xmlFile, string outputFile)
        {
            XslCompiledTransform xslTransform = new XslCompiledTransform();
            xslTransform.Load(xslFile);
            xslTransform.Transform(xmlFile, outputFile);
        }

        /// <summary>
        /// Transform from file to in memory text.
        /// </summary>
        /// <param name="xslFile">The xsl file transform.</param>
        /// <param name="xmlFile">The xml file to transform.</param>
        /// <returns>The resulting transformed text.</returns>
        public static string FromFileToString(string xslFile, string xmlFile)
        {
            return FromFileToString(xslFile, xmlFile, null);
        }

        /// <summary>
        /// Transform from file to in memory text.
        /// </summary>
        /// <param name="xslFile">The xsl file transform.</param>
        /// <param name="xmlFile">The xml file to transform.</param>
        /// <param name="xsltArgumentList">The xslt argument list</param>
        /// <returns>The resulting transformed text.</returns>
        public static string FromFileToString(string xslFile, string xmlFile, XsltArgumentList xsltArgumentList)
        {
            // Create a new output writer.
            using (MemoryStream streamWriter = new MemoryStream())
            {
                // Create a new transformer and load the xsl file
                XslCompiledTransform xslTransform = new XslCompiledTransform();
                xslTransform.Load(xslFile);

                // Create a new xml writer and load the
                // xml file into the x path document.
                XmlWriter writer = XmlWriter.Create(streamWriter);
                System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlFile);
                System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                // Transform the xml with the xsl
                xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                // Get the array from the resulting data.
                Byte[] outputData = streamWriter.ToArray();
                string outputFile = Encoding.UTF8.GetString(outputData);

                // Return the transformed text.
                return outputFile.Trim();
            }
        }

        /// <summary>
        /// Transform from file to in file.
        /// </summary>
        /// <param name="xslString">The xsl string transform.</param>
        /// <param name="xmlFile">The xml file to transform.</param>
        /// <param name="xsltArgumentList">The xslt argument list</param>
        /// <param name="outputFile">The file to write the transformed result to.</param>
        /// <returns>The resulting transformed text.</returns>
        public static void FromFileToFile(string xslString, string xmlFile, string outputFile, XsltArgumentList xsltArgumentList)
        {
            // Create a new output writer.
            using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
            using (MemoryStream streamWriter = new MemoryStream())
            {
                // Create a new transformer and load the xsl file
                XslCompiledTransform xslTransform = new XslCompiledTransform();
                XmlReader reader = XmlReader.Create(streamXslReader);
                xslTransform.Load(reader);

                // Set the conformance level of the writer.
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Auto;
                settings.Encoding = Encoding.UTF8;

                // Create a new xml writer and load the
                // xml file into the x path document.
                XmlWriter writer = XmlWriter.Create(streamWriter, settings);
                System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlFile);
                System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                // Transform the xml with the xsl
                xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                // Get the array from the resulting data.
                Byte[] outputData = streamWriter.ToArray();

                // Write the transformed data to
                // the output file.
                using (StreamWriter output = new StreamWriter(outputFile, false, Encoding.UTF8))
                    output.Write(Encoding.UTF8.GetString(outputData).Trim());
            }
        }

        /// <summary>
        /// Transform from memory text to in memory text.
        /// </summary>
        /// <param name="xslString">The xsl text transform.</param>
        /// <param name="xmlString">The xml text to transform.</param>
        /// <returns>The resulting transformed text.</returns>
        public static string FromMemoryToMemory(string xslString, string xmlString)
        {
            return FromMemoryToMemory(xslString, xmlString, null);
        }

        /// <summary>
        /// Transform from memory text to in memory text.
        /// </summary>
        /// <param name="xslString">The xsl text transform.</param>
        /// <param name="xmlString">The xml text to transform.</param>
        /// <param name="xsltArgumentList">The xslt argument list</param>
        /// <returns>The resulting transformed text.</returns>
        public static string FromMemoryToMemory(string xslString, string xmlString, XsltArgumentList xsltArgumentList)
        {
            // Create a new output writer.
            using (MemoryStream streamXmlReader = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
            using (MemoryStream streamWriter = new MemoryStream())
            {
                // Create a new transformer and load the xsl file
                XslCompiledTransform xslTransform = new XslCompiledTransform();
                XmlReader reader = XmlReader.Create(streamXslReader);
                xslTransform.Load(reader);

                // Create a new xml writer and load the
                // xml file into the x path document.
                XmlWriter writer = XmlWriter.Create(streamWriter);
                XmlReader xmlReader = XmlReader.Create(streamXmlReader);
                System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlReader);
                System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                // Transform the xml with the xsl
                xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                // Get the array from the resulting data.
                Byte[] outputData = streamWriter.ToArray();
                string outputInfo = Encoding.UTF8.GetString(outputData);

                // Return the transformed text.
                return outputInfo.Trim();
            }
        }

        /// <summary>
        /// Transform from memory text to output file.
        /// </summary>
        /// <param name="xslString">The xsl text transform.</param>
        /// <param name="xmlString">The xml text to transform.</param>
        /// <param name="outputFile">The file to write the transformed result to.</param>
        public static void FromMemoryToFile(string xslString, string xmlString, string outputFile)
        {
            FromMemoryToFile(xslString, xmlString, outputFile, null);
        }

        /// <summary>
        /// Transform from memory text to output file.
        /// </summary>
        /// <param name="xslString">The xsl text transform.</param>
        /// <param name="xmlString">The xml text to transform.</param>
        /// <param name="outputFile">The file to write the transformed result to.</param>
        /// <param name="xsltArgumentList">The xslt argument list</param>
        public static void FromMemoryToFile(string xslString, string xmlString, string outputFile, XsltArgumentList xsltArgumentList)
        {
            // Create a new output writer.
            using (MemoryStream streamXmlReader = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
            using (MemoryStream streamWriter = new MemoryStream())
            {
                // Create a new transformer and load the xsl file
                XslCompiledTransform xslTransform = new XslCompiledTransform();
                XmlReader reader = XmlReader.Create(streamXslReader);
                xslTransform.Load(reader);

                // Create a new xml writer and load the
                // xml file into the x path document.
                XmlWriter writer = XmlWriter.Create(streamWriter);
                XmlReader xmlReader = XmlReader.Create(streamXmlReader);
                System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlReader);
                System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                // Transform the xml with the xsl
                xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);
                Byte[] outputData = streamWriter.ToArray();

                // Write the transformed data to
                // the output file.
                using (StreamWriter output = new StreamWriter(outputFile, false, Encoding.UTF8))
                    output.BaseStream.Write(outputData, 0, outputData.Length);
            }
        }
        #endregion

        /// <summary>
        /// Xsl ASCII transformation
        /// </summary>
        public class ASCII
        {
            #region ASCII Transformation
            /// <summary>
            /// Transform from file to in memory text.
            /// </summary>
            /// <param name="xslFile">The xsl file transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromFileToString(string xslFile, string xmlFile)
            {
                return FromFileToString(xslFile, xmlFile, null);
            }

            /// <summary>
            /// Transform from file to in memory text.
            /// </summary>
            /// <param name="xslFile">The xsl file transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromFileToString(string xslFile, string xmlFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    xslTransform.Load(xslFile);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlFile);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();
                    string outputFile = Encoding.ASCII.GetString(outputData);

                    // Return the transformed text.
                    return outputFile.Trim();
                }
            }

            /// <summary>
            /// Transform from memory text to in memory text.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromMemoryToMemory(string xslString, string xmlString)
            {
                return FromMemoryToMemory(xslString, xmlString, null);
            }

            /// <summary>
            /// Transform from memory text to in memory text.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromMemoryToMemory(string xslString, string xmlString, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXmlReader = new MemoryStream(Encoding.ASCII.GetBytes(xmlString)))
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    XmlReader xmlReader = XmlReader.Create(streamXmlReader);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlReader);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();
                    string outputInfo = Encoding.ASCII.GetString(outputData);

                    // Return the transformed text.
                    return outputInfo.Trim();
                }
            }

            /// <summary>
            /// Transform from memory text to output file.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            public static void FromMemoryToFile(string xslString, string xmlString, string outputFile)
            {
                FromMemoryToFile(xslString, xmlString, outputFile);
            }

            /// <summary>
            /// Transform from memory text to output file.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            public static void FromMemoryToFile(string xslString, string xmlString, string outputFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXmlReader = new MemoryStream(Encoding.ASCII.GetBytes(xmlString)))
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    XmlReader xmlReader = XmlReader.Create(streamXmlReader);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlReader);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);
                    Byte[] outputData = streamWriter.ToArray();

                    // Write the transformed data to
                    // the output file.
                    using (StreamWriter output = new StreamWriter(outputFile, false, Encoding.ASCII))
                        output.BaseStream.Write(outputData, 0, outputData.Length);
                }
            }

            /// <summary>
            /// Transform from file to in file.
            /// </summary>
            /// <param name="xslString">The xsl string transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            /// <returns>The resulting transformed text.</returns>
            public static void FromFileToFile(string xslString, string xmlFile, string outputFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Set the conformance level of the writer.
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.ConformanceLevel = ConformanceLevel.Auto;
                    settings.Encoding = Encoding.ASCII;

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter, settings);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlFile);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();

                    // Write the transformed data to
                    // the output file.
                    using (StreamWriter output = new StreamWriter(outputFile, false, Encoding.ASCII))
                        output.Write(Encoding.ASCII.GetString(outputData).Trim());
                }
            }
            #endregion
        }

        /// <summary>
        /// Xsl Unicode transformation
        /// </summary>
        public class Unicode
        {
            #region Unicode Transformation
            /// <summary>
            /// Transform from file to in memory text.
            /// </summary>
            /// <param name="xslFile">The xsl file transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromFileToString(string xslFile, string xmlFile)
            {
                return FromFileToString(xslFile, xmlFile, null);
            }

            /// <summary>
            /// Transform from file to in memory text.
            /// </summary>
            /// <param name="xslFile">The xsl file transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromFileToString(string xslFile, string xmlFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    xslTransform.Load(xslFile);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlFile);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();
                    string outputFile = Encoding.Unicode.GetString(outputData);

                    // Return the transformed text.
                    return outputFile.Trim();
                }
            }

            /// <summary>
            /// Transform from memory text to in memory text.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromMemoryToMemory(string xslString, string xmlString)
            {
                return FromMemoryToMemory(xslString, xmlString, null);
            }

            /// <summary>
            /// Transform from memory text to in memory text.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromMemoryToMemory(string xslString, string xmlString, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXmlReader = new MemoryStream(Encoding.Unicode.GetBytes(xmlString)))
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    XmlReader xmlReader = XmlReader.Create(streamXmlReader);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlReader);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();
                    string outputInfo = Encoding.Unicode.GetString(outputData);

                    // Return the transformed text.
                    return outputInfo.Trim();
                }
            }

            /// <summary>
            /// Transform from memory text to output file.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            public static void FromMemoryToFile(string xslString, string xmlString, string outputFile)
            {
                FromMemoryToFile(xslString, xmlString, outputFile, null);
            }

            /// <summary>
            /// Transform from memory text to output file.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            public static void FromMemoryToFile(string xslString, string xmlString, string outputFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXmlReader = new MemoryStream(Encoding.Unicode.GetBytes(xmlString)))
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    XmlReader xmlReader = XmlReader.Create(streamXmlReader);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlReader);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);
                    Byte[] outputData = streamWriter.ToArray();

                    // Write the transformed data to
                    // the output file.
                    using (StreamWriter output = new StreamWriter(outputFile, false, Encoding.Unicode))
                        output.BaseStream.Write(outputData, 0, outputData.Length);
                }
            }

            /// <summary>
            /// Transform from file to in file.
            /// </summary>
            /// <param name="xslString">The xsl string transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            /// <returns>The resulting transformed text.</returns>
            public static void FromFileToFile(string xslString, string xmlFile, string outputFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Set the conformance level of the writer.
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.ConformanceLevel = ConformanceLevel.Auto;
                    settings.Encoding = Encoding.Unicode;

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter, settings);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlFile);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();

                    // Write the transformed data to
                    // the output file.
                    using (StreamWriter output = new StreamWriter(outputFile, false, Encoding.Unicode))
                        output.Write(Encoding.Unicode.GetString(outputData).Trim());
                }
            }
            #endregion
        }

        /// <summary>
        /// Xsl UTF32 transformation
        /// </summary>
        public class UTF32
        {
            #region UTF32 Transformation
            /// <summary>
            /// Transform from file to in memory text.
            /// </summary>
            /// <param name="xslFile">The xsl file transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromFileToString(string xslFile, string xmlFile)
            {
                return FromFileToString(xslFile, xmlFile, null);
            }

            /// <summary>
            /// Transform from file to in memory text.
            /// </summary>
            /// <param name="xslFile">The xsl file transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromFileToString(string xslFile, string xmlFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    xslTransform.Load(xslFile);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlFile);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();
                    string outputFile = Encoding.UTF32.GetString(outputData);

                    // Return the transformed text.
                    return outputFile.Trim();
                }
            }

            /// <summary>
            /// Transform from memory text to in memory text.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromMemoryToMemory(string xslString, string xmlString)
            {
                return FromMemoryToMemory(xslString, xmlString, null);
            }

            /// <summary>
            /// Transform from memory text to in memory text.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromMemoryToMemory(string xslString, string xmlString, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXmlReader = new MemoryStream(Encoding.UTF32.GetBytes(xmlString)))
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    XmlReader xmlReader = XmlReader.Create(streamXmlReader);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlReader);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();
                    string outputInfo = Encoding.UTF32.GetString(outputData);

                    // Return the transformed text.
                    return outputInfo.Trim();
                }
            }

            /// <summary>
            /// Transform from memory text to output file.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            public static void FromMemoryToFile(string xslString, string xmlString, string outputFile)
            {
                FromMemoryToFile(xslString, xmlString, outputFile, null);
            }

            /// <summary>
            /// Transform from memory text to output file.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            public static void FromMemoryToFile(string xslString, string xmlString, string outputFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXmlReader = new MemoryStream(Encoding.UTF32.GetBytes(xmlString)))
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    XmlReader xmlReader = XmlReader.Create(streamXmlReader);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlReader);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);
                    Byte[] outputData = streamWriter.ToArray();

                    // Write the transformed data to
                    // the output file.
                    using (StreamWriter output = new StreamWriter(outputFile, false, Encoding.UTF32))
                        output.BaseStream.Write(outputData, 0, outputData.Length);
                }
            }

            /// <summary>
            /// Transform from file to in file.
            /// </summary>
            /// <param name="xslString">The xsl string transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            /// <returns>The resulting transformed text.</returns>
            public static void FromFileToFile(string xslString, string xmlFile, string outputFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Set the conformance level of the writer.
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.ConformanceLevel = ConformanceLevel.Auto;
                    settings.Encoding = Encoding.UTF32;

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter, settings);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlFile);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();

                    // Write the transformed data to
                    // the output file.
                    using (StreamWriter output = new StreamWriter(outputFile, false, Encoding.UTF32))
                        output.Write(Encoding.UTF32.GetString(outputData).Trim());
                }
            }
            #endregion
        }

        /// <summary>
        /// Xsl UTF7 transformation
        /// </summary>
        public class UTF7
        {
            #region UTF7 Transformation
            /// <summary>
            /// Transform from file to in memory text.
            /// </summary>
            /// <param name="xslFile">The xsl file transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromFileToString(string xslFile, string xmlFile)
            {
                return FromFileToString(xslFile, xmlFile, null);
            }

            /// <summary>
            /// Transform from file to in memory text.
            /// </summary>
            /// <param name="xslFile">The xsl file transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromFileToString(string xslFile, string xmlFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    xslTransform.Load(xslFile);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlFile);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();
                    string outputFile = Encoding.UTF7.GetString(outputData);

                    // Return the transformed text.
                    return outputFile.Trim();
                }
            }

            /// <summary>
            /// Transform from memory text to in memory text.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromMemoryToMemory(string xslString, string xmlString)
            {
                return FromMemoryToMemory(xslString, xmlString, null);
            }

            /// <summary>
            /// Transform from memory text to in memory text.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <returns>The resulting transformed text.</returns>
            public static string FromMemoryToMemory(string xslString, string xmlString, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXmlReader = new MemoryStream(Encoding.UTF7.GetBytes(xmlString)))
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    XmlReader xmlReader = XmlReader.Create(streamXmlReader);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlReader);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();
                    string outputInfo = Encoding.UTF7.GetString(outputData);

                    // Return the transformed text.
                    return outputInfo.Trim();
                }
            }
            /// <summary>
            /// Transform from memory text to output file.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            public static void FromMemoryToFile(string xslString, string xmlString, string outputFile)
            {
                FromMemoryToFile(xslString, xmlString, outputFile, null);
            }

            /// <summary>
            /// Transform from memory text to output file.
            /// </summary>
            /// <param name="xslString">The xsl text transform.</param>
            /// <param name="xmlString">The xml text to transform.</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            public static void FromMemoryToFile(string xslString, string xmlString, string outputFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXmlReader = new MemoryStream(Encoding.UTF7.GetBytes(xmlString)))
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter);
                    XmlReader xmlReader = XmlReader.Create(streamXmlReader);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlReader);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);
                    Byte[] outputData = streamWriter.ToArray();

                    // Write the transformed data to
                    // the output file.
                    using (StreamWriter output = new StreamWriter(outputFile, false, Encoding.UTF7))
                        output.BaseStream.Write(outputData, 0, outputData.Length);
                }
            }

            /// <summary>
            /// Transform from file to in file.
            /// </summary>
            /// <param name="xslString">The xsl string transform.</param>
            /// <param name="xmlFile">The xml file to transform.</param>
            /// <param name="xsltArgumentList">The xslt argument list</param>
            /// <param name="outputFile">The file to write the transformed result to.</param>
            /// <returns>The resulting transformed text.</returns>
            public static void FromFileToFile(string xslString, string xmlFile, string outputFile, XsltArgumentList xsltArgumentList)
            {
                // Create a new output writer.
                using (MemoryStream streamXslReader = new MemoryStream(Encoding.UTF8.GetBytes(xslString)))
                using (MemoryStream streamWriter = new MemoryStream())
                {
                    // Create a new transformer and load the xsl file
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    XmlReader reader = XmlReader.Create(streamXslReader);
                    xslTransform.Load(reader);

                    // Set the conformance level of the writer.
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.ConformanceLevel = ConformanceLevel.Auto;
                    settings.Encoding = Encoding.UTF7;

                    // Create a new xml writer and load the
                    // xml file into the x path document.
                    XmlWriter writer = XmlWriter.Create(streamWriter, settings);
                    System.Xml.XPath.XPathDocument pathDocument = new System.Xml.XPath.XPathDocument(xmlFile);
                    System.Xml.XPath.XPathNavigator pathNavigator = pathDocument.CreateNavigator();

                    // Transform the xml with the xsl
                    xslTransform.Transform(pathNavigator, (xsltArgumentList != null ? xsltArgumentList : new XsltArgumentList()), writer);

                    // Get the array from the resulting data.
                    Byte[] outputData = streamWriter.ToArray();

                    // Write the transformed data to
                    // the output file.
                    using (StreamWriter output = new StreamWriter(outputFile, false, Encoding.UTF7))
                        output.Write(Encoding.UTF7.GetString(outputData).Trim());
                }
            }
            #endregion
        }
    }
}
