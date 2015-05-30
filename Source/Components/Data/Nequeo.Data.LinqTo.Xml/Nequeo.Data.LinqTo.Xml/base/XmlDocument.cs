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
using System.Data;

namespace Nequeo.Data.LinqToXml
{
    /// <summary>
    /// Xml XQuery document reader.
    /// </summary>
	public class XmlQueryDocument
	{
        /// <summary>
        /// Load the xml document.
        /// </summary>
        /// <param name="xmlFilePath">The xml file to load.</param>
        /// <param name="xmlString">The xml string to load.</param>
        /// <param name="stream">A stream object to load.</param>
        /// <param name="textReader">A text reader object to load.</param>
        /// <param name="xmlReader">A xml reader object to load.</param>
        /// <returns>The XDocument containing the document.</returns>
        public XDocument LoadDocument(
            string xmlFilePath = null,
            string xmlString = null,
            Stream stream = null,
            TextReader textReader = null,
            XmlReader xmlReader = null)
        {
            // Return the Xml Document.
            return Nequeo.Xml.Document.LoadDocument(xmlFilePath, xmlString, stream, textReader, xmlReader);
        }

        /// <summary>
        /// Query the xml document.
        /// </summary>
        /// <param name="xDocument">The xml document.</param>
        /// <param name="descendantNodeName">The descendant node name.</param>
        /// <returns>The collection of descendant nodes within the node name.</returns>
        public IEnumerable<XElement> Query(XDocument xDocument, XName descendantNodeName = null)
        {
            if (descendantNodeName != null)
                return xDocument.Descendants(descendantNodeName);
            else
                return xDocument.Descendants();
        }

        /// <summary>
        /// Query the element using the where predicate.
        /// </summary>
        /// <param name="xElement">The element collection to query.</param>
        /// <param name="predicate">The where query predicate.</param>
        /// <returns>The collection of elements</returns>
        public IEnumerable<XElement> QueryWhere(IEnumerable<XElement> xElement, Func<XElement, bool> predicate)
        {
            return xElement.Where(predicate);
        }

        /// <summary>
        /// Query the element using the where predicate.
        /// </summary>
        /// <param name="xElement">The element collection to query.</param>
        /// <param name="predicate">The where query predicate.</param>
        /// <returns>The collection of elements</returns>
        public IEnumerable<XElement> QueryWhere(IEnumerable<XElement> xElement, Func<XElement, int, bool> predicate)
        {
            return xElement.Where(predicate);
        }
	}
}
