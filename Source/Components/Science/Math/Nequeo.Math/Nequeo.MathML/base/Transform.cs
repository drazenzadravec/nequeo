/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
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
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace Nequeo.MathML
{
    /// <summary>
    /// MathML transformation.
    /// </summary>
    public class Transform
    {
        /// <summary>
        /// MathML transformation.
        /// </summary>
        public Transform() { }

        /// <summary>
        /// Transform content MathML to presentation MathML.
        /// </summary>
        /// <param name="xml">The xml string to transform.</param>
        /// <returns>The transformed xml.</returns>
        public string ContentToPresentationValue(string xml)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;

            // Transform.
            return Nequeo.Xml.Xsl.Transformation.FromMemoryToMemory(Nequeo.MathML.Properties.Resources.ctop, xml, null, settings);
        }

        /// <summary>
        /// Transform content MathML to presentation MathML.
        /// </summary>
        /// <param name="xml">The xml string to transform.</param>
        /// <returns>The transformed xml.</returns>
        public string ContentToPresentation(string xml)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;

            XmlReaderSettings reader = new XmlReaderSettings();
            reader.ConformanceLevel = ConformanceLevel.Auto;
            reader.IgnoreWhitespace = true;

            // Transform.
            return Nequeo.Xml.Xsl.Transformation.FromMemoryToMemory(Nequeo.MathML.Properties.Resources.ctopML, xml, null, settings, reader);
        }

        /// <summary>
        /// Transform content MathML to presentation MathML.
        /// </summary>
        /// <param name="xml">The xml string to transform.</param>
        /// <returns>The transformed xml.</returns>
        public string ContentToPresentation2(string xml)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;

            XmlReaderSettings reader = new XmlReaderSettings();
            reader.ConformanceLevel = ConformanceLevel.Auto;
            reader.IgnoreWhitespace = true;

            // Transform.
            return Nequeo.Xml.Xsl.Transformation.FromMemoryToMemory(Nequeo.MathML.Properties.Resources.mmlctop2_0, xml, null, settings, reader);
        }

        /// <summary>
        /// Transform content MathML to presentation MathML.
        /// </summary>
        /// <param name="xml">The xml string to transform.</param>
        /// <param name="xslt">The xslt string to transform.</param>
        /// <returns>The transformed xml.</returns>
        public string ContentToPresentation(string xml, string xslt)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;

            XmlReaderSettings reader = new XmlReaderSettings();
            reader.ConformanceLevel = ConformanceLevel.Auto;
            reader.IgnoreWhitespace = true;

            // Transform.
            return Nequeo.Xml.Xsl.Transformation.FromMemoryToMemory(xslt, xml, null, settings, reader);
        }

        /// <summary>
        /// Transform content MathML to presentation MathML.
        /// </summary>
        /// <param name="xml">The xml string to transform.</param>
        /// <returns>The transformed xml.</returns>
        public string ContentToPresentationPMathML(string xml)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;

            // Settings.
            XsltSettings xsltSettings = new XsltSettings();
            xsltSettings.EnableDocumentFunction = true;
            xsltSettings.EnableScript = true;

            // Resolver.
            XmlUrlResolver resolver = new XmlUrlResolver();

            // Transform.
            return Nequeo.Xml.Xsl.Transformation.FromMemoryToMemory(Nequeo.MathML.Properties.Resources.pmathml, xml, null, settings, xsltSettings, resolver);
        }
    }
}
