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

using Nequeo.Model.Conversion;

namespace Nequeo.Conversion
{
    /// <summary>
    /// Transformation model collection formatter interface.
    /// </summary>
    public interface ITransformModelCollection
    {
        /// <summary>
        /// Create the xml document within the xml file.
        /// </summary>
        /// <param name="node">The xml node to create.</param>
        /// <param name="xmlFilePath">The file name to write to.</param>
        /// <param name="version">The xml version.</param>
        /// <param name="encoding">The xml encoding</param>
        /// <param name="standalone">The xml standalone</param>
        /// <returns>True if the xml was saved; else false</returns>
        bool CreateDocument(XElement node, string xmlFilePath, string version, string encoding, string standalone);

        /// <summary>
        /// Create the xml document within the xml file.
        /// </summary>
        /// <param name="item">The item containing the xml nodes to write.</param>
        /// <param name="xmlFilePath">The file name to write to.</param>
        /// <returns>True if the xml was saved; else false</returns>
        bool CreateDocument(TransformXmlModel item, string xmlFilePath);

        /// <summary>
        /// Create the xml document.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <returns>The xml document containing the transform model collection</returns>
        XDocument CreateDocument(List<TransformModel[]> items);

        /// <summary>
        /// Create the xml document.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="rootElementName">The root element name</param>
        /// <param name="elementName">The individual element name</param>
        /// <returns>The xml document containing the transform model collection</returns>
        XDocument CreateDocument(List<TransformModel[]> items, string rootElementName, string elementName);

        /// <summary>
        /// Create the xml document within the xml file.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="xmlFilePath">The xml file name and path where to xml should be placed</param>
        /// <returns>True if the xml was saved; else false</returns>
        bool CreateDocument(List<TransformModel[]> items, string xmlFilePath);

        /// <summary>
        /// Create the xml document within the xml file.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="xmlFilePath">The xml file name and path where to xml should be placed</param>
        /// <param name="rootElementName">The root element name</param>
        /// <param name="elementName">The individual element name</param>
        /// <returns>True if the xml was saved; else false</returns>
        bool CreateDocument(List<TransformModel[]> items, string xmlFilePath, string rootElementName, string elementName);

        /// <summary>
        /// Create the xml document within the text writer.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="textWriter">The text writer to where the xml nodes shoild be placed.</param>
        /// <returns>True if the xml was saved; else false</returns>
        bool CreateDocument(List<TransformModel[]> items, TextWriter textWriter);

        /// <summary>
        /// Create the xml document within the text writer.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="textWriter">The text writer to where the xml nodes shoild be placed.</param>
        /// <param name="rootElementName">The root element name</param>
        /// <param name="elementName">The individual element name</param>
        /// <returns>True if the xml was saved; else false</returns>
        bool CreateDocument(List<TransformModel[]> items, TextWriter textWriter, string rootElementName, string elementName);

        /// <summary>
        /// Create the xml document within the xml writer.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="xmlWriter">The xml writer to where the xml nodes shoild be placed.</param>
        /// <returns>True if the xml was saved; else false</returns>
        bool CreateDocument(List<TransformModel[]> items, XmlWriter xmlWriter);

        /// <summary>
        /// Create the xml document within the xml writer.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="xmlWriter">The xml writer to where the xml nodes shoild be placed.</param>
        /// <param name="rootElementName">The root element name</param>
        /// <param name="elementName">The individual element name</param>
        /// <returns>True if the xml was saved; else false</returns>
        bool CreateDocument(List<TransformModel[]> items, XmlWriter xmlWriter, string rootElementName, string elementName);

        /// <summary>
        /// Creates the xml node for the collection of models.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <returns>The xml node containing the transform model collection</returns>
        XElement CreateElement(List<TransformModel[]> items);

        /// <summary>
        /// Creates the xml node for the collection of models.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <param name="rootElementName">The root element name</param>
        /// <param name="elementName">The individual element name</param>
        /// <returns>The xml node containing the transform model collection</returns>
        XElement CreateElement(List<TransformModel[]> items, string rootElementName, string elementName);

        /// <summary>
        /// Creates a collection of xml nodes for each transform model collection.
        /// </summary>
        /// <param name="items">The collection of transform model items</param>
        /// <returns>The collection of xml nodes for each model collection</returns>
        XElement[] CreateElementNodes(TransformModel[] items);

    }
}
