/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
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
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nequeo.Model.Language;

namespace Nequeo.Net.Translator.Microsoft.Cognitive
{
    /// <summary>
    /// Microsoft cognitive api translator.
    /// </summary>
    public sealed class Api
    {
        /// <summary>
        /// Microsoft api translator.
        /// </summary>
        public Api()
        {
            // Gets the microsoft translator service URI.
            _service = new Uri(Nequeo.Net.Translator.Properties.Settings.Default.MicrosoftTranslatorServiceURI_CV2);
        }

        /// <summary>
        /// Microsoft api translator.
        /// </summary>
        /// <param name="service">An absolute URI that identifies the root of a data service.</param>
        public Api(Uri service)
        {
            _service = service;
        }

        private Uri _service = null;
        private NetworkCredential _credentials = null;
        private ApiContainer _container = null;
        private string _clientIP = null;
        private string _clientID = null;
        private string _userAgent = null;

        /// <summary>
        /// Gets or sets the translator service.
        /// </summary>
        public Uri Service
        {
            get { return _service; }
            set { _service = value; }
        }

        /// <summary>
        /// Gets or sets the network credentials used to access the service (the api key for the username and password).
        /// </summary>
        public NetworkCredential Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }

        /// <summary>
        /// Gets or sets the client IP address sending the request.
        /// </summary>
        public string ClientIP
        {
            get { return _clientIP; }
            set { _clientIP = value; }
        }

        /// <summary>
        /// Gets or sets the client ID sending the request.
        /// </summary>
        public string ClientID
        {
            get { return _clientID; }
            set { _clientID = value; }
        }

        /// <summary>
        /// Gets or sets the request user agent.
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

        /// <summary>
        /// Get the access token.
        /// </summary>
        /// <param name="serviceExtension">The service extension ([serviceRoot]/issueToken).</param>
        /// <returns>The Bearer + access token.</returns>
        public string GetAccessToken(string serviceExtension = "issueToken")
        {
            Initialise();
            return _container.GetAccessToken(serviceExtension);
        }

        /// <summary>
        /// Translate the text from one language to another.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="to">The language code to translate the text into.</param>
        /// <param name="from">The language code of the translation text.</param>
        /// <param name="appid">If the Authorization is used, leave the appid field empty else specify a string containing "Bearer" + " " + access token.</param>
        /// <param name="authorization">Authorization token: "Bearer" + " " + access token. Required if the appid field is not specified.</param>
        /// <param name="contentType">The format of the text being translated. The supported formats are "text/plain" and "text/html". Any HTML needs to be well-formed.</param>
        /// <param name="category">A string containing the category (domain) of the translation. Defaults to "general".</param>
        /// <returns>The transalted text.</returns>
        public byte[] Translate(string text, string to, string from = null,
            string appid = null, string authorization = null, string contentType = null,
            string category = null)
        {
            Initialise();
            return _container.Translate(text, to, from, appid, authorization, contentType, category);
        }

        /// <summary>
        /// Obtain a list of language codes representing languages that are supported by the Translation Service.
        /// </summary>
        /// <param name="appid">If the Authorization is used, leave the appid field empty else specify a string containing "Bearer" + " " + access token.</param>
        /// <param name="authorization">Authorization token: "Bearer" + " " + access token. Required if the appid field is not specified.</param>
        /// <returns>A string array containing languages names supported by the Translator Service, localized into the requested language.</returns>
        public byte[] GetLanguagesForTranslate(string appid = null, string authorization = null)
        {
            Initialise();
            return _container.GetLanguagesForTranslate(appid, authorization);
        }

        /// <summary>
        /// Identify the language of a selected piece of text.
        /// </summary>
        /// <param name="text">The text to detect.</param>
        /// <param name="appid">If the Authorization is used, leave the appid field empty else specify a string containing "Bearer" + " " + access token.</param>
        /// <param name="authorization">Authorization token: "Bearer" + " " + access token. Required if the appid field is not specified.</param>
        /// <returns>A string containing a two-character Language code for the given text.</returns>
        public byte[] Detect(string text, string appid = null, string authorization = null)
        {
            Initialise();
            return _container.Detect(text, appid, authorization);
        }

        /// <summary>
        /// Get the translated text from the xml data.
        /// </summary>
        /// <param name="xmlData">The array of bytes containing the xml data.</param>
        /// <returns>The array of translations.</returns>
        public Translation[] Translate(byte[] xmlData)
        {
            List<Translation> translations = new List<Translation>();

            // Get the node list.
            System.Xml.XmlNodeList nodes = Nequeo.Xml.Element.Document(xmlData).ChildNodes;

            // Add the collection of nodes.
            foreach (System.Xml.XmlNode node in nodes)
                translations.Add(new Translation() { Text = node.InnerText });

            // Return the translations.
            return translations.ToArray();
        }

        /// <summary>
        /// Get the translated text from the xml data.
        /// </summary>
        /// <param name="xmlData">The array of bytes containing the xml data.</param>
        /// <returns>The array of translations.</returns>
        public Language[] GetLanguagesForTranslate(byte[] xmlData)
        {
            List<Language> translations = new List<Language>();

            // Get the node list.
            System.Xml.XmlNodeList nodes = Nequeo.Xml.Element.Document(xmlData).ChildNodes;

            // Add the collection of nodes.
            foreach (System.Xml.XmlNode root in nodes)
            {
                // If is array.
                if (root.HasChildNodes)
                {
                    // Add the collection of nodes.
                    foreach (System.Xml.XmlNode node in root.ChildNodes)
                        translations.Add(new Language() { Code = node.InnerText });
                }
            }

            // Return the translations.
            return translations.ToArray();
        }

        /// <summary>
        /// Get the translated text from the xml data.
        /// </summary>
        /// <param name="xmlData">The array of bytes containing the xml data.</param>
        /// <returns>The array of translations.</returns>
        public Language[] Detect(byte[] xmlData)
        {
            List<Language> translations = new List<Language>();

            // Get the node list.
            System.Xml.XmlNodeList nodes = Nequeo.Xml.Element.Document(xmlData).ChildNodes;

            // Add the collection of nodes.
            foreach (System.Xml.XmlNode node in nodes)
                translations.Add(new Language() { Code = node.InnerText });

            // Return the translations.
            return translations.ToArray();
        }

        /// <summary>
        /// Initialise.
        /// </summary>
        private void Initialise()
        {
            // If the container needs to be translated.
            if (_container == null)
            {
                // Create the container.
                _container = new ApiContainer(_service, _credentials.UserName, _clientIP, _clientID, _userAgent);
            }
        }
    }
}
