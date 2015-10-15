/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Data.Services.Client;

using Nequeo.Model.Language;

namespace Nequeo.Net.Translator.Microsoft
{
    /// <summary>
    /// Azure data market language service query container.
    /// </summary>
    internal class AzureDataMarketContainer : Nequeo.Net.ServiceModel.DataServiceClient
    {
        /// <summary>
        /// Azure data market service query container.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        public AzureDataMarketContainer(Uri serviceRoot) :
                base(serviceRoot)
        {
        }

        /// <summary>
        /// Translate the text from one language to another.
        /// </summary>
        /// <param name="text">the text to translate Sample Values : hello</param>
        /// <param name="to">the language code to translate the text into Sample Values : nl</param>
        /// <param name="from">the language code of the translation text Sample Values : en</param>
        /// <returns>Represents a single query request to a data service.</returns>
        public DataServiceQuery<Translation> Translate(String text, String to, String from)
        {
            if ((text == null))
            {
                throw new System.ArgumentNullException("text", "Text value cannot be null");
            }
            if ((to == null))
            {
                throw new System.ArgumentNullException("to", "To value cannot be null");
            }

            DataServiceQuery<Translation> query;
            query = base.CreateQuery<Translation>("Translate");

            if ((text != null))
            {
                query = query.AddQueryOption("Text", string.Concat("\'", base.EscapeDataString(text), "\'"));
            }
            if ((to != null))
            {
                query = query.AddQueryOption("To", string.Concat("\'", base.EscapeDataString(to), "\'"));
            }
            if ((from != null))
            {
                query = query.AddQueryOption("From", string.Concat("\'", base.EscapeDataString(from), "\'"));
            }

            return query;
        }

        /// <summary>
        /// Get languages for translation.
        /// </summary>
        /// <returns>Represents a single query request to a data service.</returns>
        public DataServiceQuery<Language> GetLanguagesForTranslation()
        {
            DataServiceQuery<Language> query;
            query = base.CreateQuery<Language>("GetLanguagesForTranslation");
            return query;
        }

        /// <summary>
        /// Detect the language of the text.
        /// </summary>
        /// <param name="text">the text whose language is to be identified Sample Values : hello</param>
        /// <returns>Represents a single query request to a data service.</returns>
        public DataServiceQuery<Language> Detect(String text)
        {
            if ((text == null))
            {
                throw new System.ArgumentNullException("text", "Text value cannot be null");
            }

            DataServiceQuery<Language> query;
            query = base.CreateQuery<Language>("Detect");

            if ((text != null))
            {
                query = query.AddQueryOption("Text", string.Concat("\'", base.EscapeDataString(text), "\'"));
            }

            return query;
        }

        /// <summary>
        /// Get the translated text from the xml data.
        /// </summary>
        /// <param name="xmlData">The array of bytes containing the xml data.</param>
        /// <returns>The array of translations.</returns>
        public Translation[] TranslateResponse(byte[] xmlData)
        {
            List<Translation> translations = new List<Translation>();

            // Create the list of namespaces.
            Nequeo.Model.NameValue[] namespaces = new Model.NameValue[]
            {
                new Model.NameValue() { Name ="d", Value = "http://schemas.microsoft.com/ado/2007/08/dataservices" },
                new Model.NameValue() { Name ="m", Value = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata" },
            };

            // Get the node list.
            System.Xml.XmlNodeList nodes = Nequeo.Xml.Element.ExtractNode(xmlData, namespaces, "//d:Text");

            // Add the collection of nodes.
            foreach (System.Xml.XmlNode node in nodes)
                translations.Add(new Translation() { Text = node.InnerText } );

            // Return the translations.
            return translations.ToArray();
        }

        /// <summary>
        /// Get the detect text from the xml data.
        /// </summary>
        /// <param name="xmlData">The array of bytes containing the xml data.</param>
        /// <returns>The array of detects.</returns>
        public Language[] DetectResponse(byte[] xmlData)
        {
            List<Language> translations = new List<Language>();

            // Create the list of namespaces.
            Nequeo.Model.NameValue[] namespaces = new Model.NameValue[]
            {
                new Model.NameValue() { Name ="d", Value = "http://schemas.microsoft.com/ado/2007/08/dataservices" },
                new Model.NameValue() { Name ="m", Value = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata" },
            };

            // Get the node list.
            System.Xml.XmlNodeList nodes = Nequeo.Xml.Element.ExtractNode(xmlData, namespaces, "//d:Code");

            // Add the collection of nodes.
            foreach (System.Xml.XmlNode node in nodes)
                translations.Add(new Language() { Code = node.InnerText });

            // Return the translations.
            return translations.ToArray();
        }

        /// <summary>
        /// Get the language list text from the xml data.
        /// </summary>
        /// <param name="xmlData">The array of bytes containing the xml data.</param>
        /// <returns>The array of detects.</returns>
        public Language[] GetLanguagesForTranslationResponse(byte[] xmlData)
        {
            List<Language> translations = new List<Language>();

            // Create the list of namespaces.
            Nequeo.Model.NameValue[] namespaces = new Model.NameValue[]
            {
                new Model.NameValue() { Name ="d", Value = "http://schemas.microsoft.com/ado/2007/08/dataservices" },
                new Model.NameValue() { Name ="m", Value = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata" },
            };

            // Get the node list.
            System.Xml.XmlNodeList nodes = Nequeo.Xml.Element.ExtractNode(xmlData, namespaces, "//d:Code");

            // Add the collection of nodes.
            foreach (System.Xml.XmlNode node in nodes)
                translations.Add(new Language() { Code = node.InnerText });

            // Return the translations.
            return translations.ToArray();
        }
    }
}
