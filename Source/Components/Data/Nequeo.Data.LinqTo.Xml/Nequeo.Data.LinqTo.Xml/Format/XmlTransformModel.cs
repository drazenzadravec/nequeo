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
using System.Xml.Linq;
using System.IO;

using Nequeo.Conversion;
using Nequeo.Model.Conversion;

namespace Nequeo.Data.LinqToXml.Format
{
    /// <summary>
    /// Transformation model collection formatter type.
    /// </summary>
    public class XmlTransformModel : ITransformModelCollection
    {
        #region Constructors

        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly XmlTransformModel Instance = new XmlTransformModel();

        /// <summary>
        /// Static constructor
        /// </summary>
        static XmlTransformModel() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public XmlTransformModel()
        {
        }

        #endregion

        #region Private Fields

        private const string RootElementName = "Rows";
        private const string ElementName = "Row";
        private const string XmlVersion = "1.0";
        private const string XmlEncoding = "utf-8";
        private const string XmlStandalone = "yes";

        #endregion

        #region Public Methods

        /// <summary>
        /// Create the xml document within the xml file.
        /// </summary>
        /// <param name="node">The xml node to create.</param>
        /// <param name="xmlFilePath">The file name to write to.</param>
        /// <param name="version">The xml version.</param>
        /// <param name="encoding">The xml encoding</param>
        /// <param name="standalone">The xml standalone</param>
        /// <returns>True if the xml was saved; else false</returns>
        public virtual bool CreateDocument(XElement node, string xmlFilePath, string version, string encoding, string standalone)
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
        /// Create the xml document within the xml file.
        /// </summary>
        /// <param name="item">The item containing the xml nodes to write.</param>
        /// <param name="xmlFilePath">The file name to write to.</param>
        /// <returns>True if the xml was saved; else false</returns>
        public virtual bool CreateDocument(TransformXmlModel item, string xmlFilePath)
        {
            // If the directory does not exist then create it.
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(xmlFilePath)))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(xmlFilePath));

            // Create the top level xml document
            // and save the document to the xml file
            XDocument xDoc = new XDocument(
                new XDeclaration(item.Version, item.Encoding, item.Standalone),
                new XElement(item.RootElementName, item.Nodes));
            xDoc.Save(xmlFilePath);

            // Return true if at this point.
            return true;
        }

        /// <summary>
        /// Create the xml document within the xml file.
        /// </summary>
        /// <param name="item">The item containing the xml nodes to write.</param>
        /// <returns>True if the xml was saved; else false</returns>
        public virtual XDocument CreateDocument(TransformXmlModel item)
        {
            // Create the top level xml document
            // and save the document to the xml file
            XDocument xDoc = new XDocument(
                new XDeclaration(item.Version, item.Encoding, item.Standalone),
                new XElement(item.RootElementName, item.Nodes));

            // Return true if at this point.
            return xDoc;
        }

        /// <summary>
        /// Create the xml document.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <returns>The xml document containing the transform model collection</returns>
        public virtual XDocument CreateDocument(List<TransformModel[]> items)
        {
            return CreateDocument(items, RootElementName, ElementName);
        }

        /// <summary>
        /// Create the xml document.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="rootElementName">The root element name</param>
        /// <param name="elementName">The individual element name</param>
        /// <returns>The xml document containing the transform model collection</returns>
        public virtual XDocument CreateDocument(List<TransformModel[]> items, string rootElementName, string elementName)
        {
            // Create the top level xml document
            XDocument xDoc = new XDocument(new XDeclaration(XmlVersion, XmlEncoding, XmlStandalone), CreateElement(items, rootElementName, elementName));
            return xDoc;
        }

        /// <summary>
        /// Create the xml document within the xml file.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="xmlFilePath">The xml file name and path where to xml should be placed</param>
        /// <returns>True if the xml was saved; else false</returns>
        public virtual bool CreateDocument(List<TransformModel[]> items, string xmlFilePath)
        {
            return CreateDocument(items, xmlFilePath, RootElementName, ElementName);
        }

        /// <summary>
        /// Create the xml document within the xml file.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="xmlFilePath">The xml file name and path where to xml should be placed</param>
        /// <param name="rootElementName">The root element name</param>
        /// <param name="elementName">The individual element name</param>
        /// <returns>True if the xml was saved; else false</returns>
        public virtual bool CreateDocument(List<TransformModel[]> items, string xmlFilePath, string rootElementName, string elementName)
        {
            // If the directory does not exist then create it.
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(xmlFilePath)))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(xmlFilePath));

            // Create the top level xml document
            // and save the document to the xml file
            XDocument xDoc = new XDocument(new XDeclaration(XmlVersion, XmlEncoding, XmlStandalone), CreateElement(items, rootElementName, elementName));
            xDoc.Save(xmlFilePath);

            // Return true if at this point.
            return true;
        }

        /// <summary>
        /// Create the xml document within the text writer.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="textWriter">The text writer to where the xml nodes shoild be placed.</param>
        /// <returns>True if the xml was saved; else false</returns>
        public virtual bool CreateDocument(List<TransformModel[]> items, TextWriter textWriter)
        {
            return CreateDocument(items, textWriter, RootElementName, ElementName);
        }

        /// <summary>
        /// Create the xml document within the text writer.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="textWriter">The text writer to where the xml nodes shoild be placed.</param>
        /// <param name="rootElementName">The root element name</param>
        /// <param name="elementName">The individual element name</param>
        /// <returns>True if the xml was saved; else false</returns>
        public virtual bool CreateDocument(List<TransformModel[]> items, TextWriter textWriter, string rootElementName, string elementName)
        {
            // Create the top level xml document
            // and save the document to the xml file
            XDocument xDoc = new XDocument(new XDeclaration(XmlVersion, XmlEncoding, XmlStandalone), CreateElement(items, rootElementName, elementName));
            xDoc.Save(textWriter);

            // Return true if at this point.
            return true;
        }

        /// <summary>
        /// Create the xml document within the xml writer.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="xmlWriter">The xml writer to where the xml nodes shoild be placed.</param>
        /// <returns>True if the xml was saved; else false</returns>
        public virtual bool CreateDocument(List<TransformModel[]> items, XmlWriter xmlWriter)
        {
            return CreateDocument(items, xmlWriter, RootElementName, ElementName);
        }

        /// <summary>
        /// Create the xml document within the xml writer.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="xmlWriter">The xml writer to where the xml nodes shoild be placed.</param>
        /// <param name="rootElementName">The root element name</param>
        /// <param name="elementName">The individual element name</param>
        /// <returns>True if the xml was saved; else false</returns>
        public virtual bool CreateDocument(List<TransformModel[]> items, XmlWriter xmlWriter, string rootElementName, string elementName)
        {
            // Create the top level xml document
            // and save the document to the xml file
            XDocument xDoc = new XDocument(new XDeclaration(XmlVersion, XmlEncoding, XmlStandalone), CreateElement(items, rootElementName, elementName));
            xDoc.Save(xmlWriter);

            // Return true if at this point.
            return true;
        }

        /// <summary>
        /// Creates the xml node for the collection of models.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <returns>The xml node containing the transform model collection</returns>
        public virtual XElement CreateElement(List<TransformModel[]> items)
        {
            return CreateElement(items, RootElementName, ElementName);
        }

        /// <summary>
        /// Creates the xml node for the collection of models.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="rootElementName">The root element name</param>
        /// <param name="elementName">The individual element name</param>
        /// <returns>The xml node containing the transform model collection</returns>
        public virtual XElement CreateElement(List<TransformModel[]> items, string rootElementName, string elementName)
        {
            // Create the xml elements
            XElement xmlData =
                new XElement(rootElementName,
                    from pointsItems in items
                    let pointfields = pointsItems.ToArray()
                    select new XElement(elementName, CreateElementNodes(pointfields)));

            // Return the xml element nodes
            return xmlData;
        }

        /// <summary>
        /// Creates a collection of xml nodes for each transform model collection.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <returns>The collection of xml nodes for each model collection</returns>
        public virtual XElement[] CreateElementNodes(TransformModel[] items)
        {
            XElement[] array = new XElement[items.Count()];

            // For each item in the model
            // create the xml element node
            for (int i = 0; i < items.Count(); i++)
                array[i] = new XElement(items[i].ValueName, items[i].Value);

            // Return the array if xml element nodes
            return array;
        }

        #endregion
    }
}
