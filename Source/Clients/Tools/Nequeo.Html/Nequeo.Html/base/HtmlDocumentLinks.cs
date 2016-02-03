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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Nequeo.Html
{
    /// <summary>
    /// Represents a complete HTML document links.
    /// </summary>
    public class HtmlDocumentLinks
    {
        /// <summary>
		/// Represents a complete HTML document links.
		/// </summary>
        /// <param name="htmlDocument">The input HTML document. May not be null.</param>
        public HtmlDocumentLinks(HtmlDocument htmlDocument)
		{
            if (htmlDocument == null)
			{
				throw new ArgumentNullException("doc");
			}

            _doc = htmlDocument;
			GetLinks();
			GetReferences();
		}

        private ArrayList _links;
        private ArrayList _references;
        private HtmlDocument _doc;

        /// <summary>
        /// Gets a list of links as they are declared in the HTML document.
        /// </summary>
        public ArrayList Links
        {
            get
            {
                return _links;
            }
        }

        /// <summary>
        /// Gets a list of reference links to other HTML documents, as they are declared in the HTML document.
        /// </summary>
        public ArrayList References
        {
            get
            {
                return _references;
            }
        }

        /// <summary>
        /// Get all the links.
        /// </summary>
        private void GetLinks()
        {
            _links = new ArrayList();
            HtmlNodeCollection atts = _doc.DocumentNode.SelectNodes("//*[@background or @lowsrc or @src or @href]");
            if (atts == null)
                return;

            foreach (HtmlNode n in atts)
            {
                ParseLink(n, "background");
                ParseLink(n, "href");
                ParseLink(n, "src");
                ParseLink(n, "lowsrc");
            }
        }

        /// <summary>
        /// Get all the references.
        /// </summary>
        private void GetReferences()
        {
            _references = new ArrayList();
            HtmlNodeCollection hrefs = _doc.DocumentNode.SelectNodes("//a[@href]");
            if (hrefs == null)
                return;

            foreach (HtmlNode href in hrefs)
            {
                _references.Add(href.Attributes["href"].Value);
            }
        }

        /// <summary>
        /// Parse the links.
        /// </summary>
        /// <param name="node">The document node.</param>
        /// <param name="name">The link name.</param>
        private void ParseLink(HtmlNode node, string name)
        {
            HtmlAttribute att = node.Attributes[name];
            if (att == null)
                return;

            // if name = href, we are only interested by <link> tags
            if ((name == "href") && (node.Name != "link"))
                return;

            _links.Add(att.Value);
        }
    }
}
