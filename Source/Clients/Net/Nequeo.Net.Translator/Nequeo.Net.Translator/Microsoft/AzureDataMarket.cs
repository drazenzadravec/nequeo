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
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nequeo.Model.Language;

namespace Nequeo.Net.Translator.Microsoft
{
    /// <summary>
    /// Azure data market language translator.
    /// </summary>
    public sealed class AzureDataMarket
    {
        /// <summary>
        /// Azure data market language translator.
        /// </summary>
        public AzureDataMarket()
        {
            // Gets the microsoft translator service URI.
            _service = new Uri(Nequeo.Net.Translator.Properties.Settings.Default.MicrosoftTranslatorServiceURI);
        }

        /// <summary>
        /// Azure data market language translator.
        /// </summary>
        /// <param name="service">An absolute URI that identifies the root of a data service.</param>
        public AzureDataMarket(Uri service)
        {
            _service = service;
        }

        private Uri _service = null;
        private NetworkCredential _credentials = null;
        private AzureDataMarketContainer _container = null;

        /// <summary>
        /// Gets or sets the translator service.
        /// </summary>
        public Uri Service
        {
            get { return _service; }
            set { _service = value; }
        }

        /// <summary>
        /// Gets or sets the network credentials used to access the service (the account key for the username and password).
        /// </summary>
        public NetworkCredential Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }

        /// <summary>
        /// Get the array of all languages that can be translated.
        /// </summary>
        /// <returns>The array of languages that can be translated.</returns>
        public Language[] GetLanguages()
        {
            Initialise();

            // Get the langauge list from the service.
            var dataServiceQuery = _container.GetLanguagesForTranslation();
            return dataServiceQuery.Execute().ToArray();
        }

        /// <summary>
        /// Detect the language of the text.
        /// </summary>
        /// <param name="text">The text whose language is to be identified.</param>
        /// <returns>The array of possible languages.</returns>
        public Language[] DetectLanguage(string text)
        {
            Initialise();

            // Get the possible langauge list from the service.
            var dataServiceQuery = _container.Detect(text);
            return dataServiceQuery.Execute().ToArray();
        }

        /// <summary>
        /// Translate the text from one language to another.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="from">The language code of the translation text.</param>
        /// <param name="to">The language code to translate the text into.</param>
        /// <returns>The array of possible translations.</returns>
        public Translation[] Translate(string text, string from, string to)
        {
            Initialise();

            // Get the translation list from the service.
            var dataServiceQuery = _container.Translate(text, to, from);
            return dataServiceQuery.Execute().ToArray();
        }

        /// <summary>
        /// Get the translated text from the xml data.
        /// </summary>
        /// <param name="xmlData">The array of bytes containing the xml data.</param>
        /// <returns>The array of translations.</returns>
        public Translation[] Translate(byte[] xmlData)
        {
            Initialise();
            return _container.TranslateResponse(xmlData);
        }

        /// <summary>
        /// Get the detect text from the xml data.
        /// </summary>
        /// <param name="xmlData">The array of bytes containing the xml data.</param>
        /// <returns>The array of detects.</returns>
        public Language[] DetectLanguage(byte[] xmlData)
        {
            Initialise();
            return _container.DetectResponse(xmlData);
        }

        /// <summary>
        /// Get the language list text from the xml data.
        /// </summary>
        /// <param name="xmlData">The array of bytes containing the xml data.</param>
        /// <returns>The array of detects.</returns>
        public Language[] GetLanguages(byte[] xmlData)
        {
            Initialise();
            return _container.GetLanguagesForTranslationResponse(xmlData);
        }

        /// <summary>
        /// Get the lanaguage code for each language.
        /// </summary>
        /// <returns>The array of language codes.</returns>
        public Dictionary<string, string> GetLanguageCodes()
        {
            return Nequeo.Resource.Countries.GetLanguageCodes();
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
                _container = new AzureDataMarketContainer(_service);
                _container.Credentials = _credentials;
            }
        }
    }
}
