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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Services.Client;

using Nequeo.Model.Language;

namespace Nequeo.Net.Translator.Google
{
    /// <summary>
    /// Google api language service query container.
    /// </summary>
    internal class ApiContainer
    {
        /// <summary>
        /// Google api language service query container.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        /// <param name="apiKey">The api key used to access this service.</param>
        public ApiContainer(Uri serviceRoot, string apiKey)
        {
            _apiKey = apiKey;
            _serviceRoot = serviceRoot;
        }

        private string _apiKey = null;
        private Uri _serviceRoot = null;

        /// <summary>
        /// Detect the language of the text.
        /// </summary>
        /// <param name="text">the text whose language is to be identified Sample Values : hello</param>
        /// <returns>Represents a single query request to a data service.</returns>
        public LanguageTranslateCollection Detect(string text)
        {
            string query = "";

            if ((text == null))
            {
                throw new System.ArgumentNullException("text", "Text value cannot be null");
            }

            if ((text != null))
            {
                query += "&q=" + System.Uri.EscapeDataString(text);
            }

            if ((_apiKey != null))
            {
                query += "&key=" + _apiKey;
            }

            // Process the request.
            return ProcessRequest<LanguageTranslateCollection>("detect", query);
        }


        /// <summary>
        /// Get languages for translation.
        /// </summary>
        /// <returns>Represents a single query request to a data service.</returns>
        public LanguageTranslateCollection GetLanguagesForTranslation()
        {
            return GetLanguagesForTranslation(null);
        }

        /// <summary>
        /// Get languages for translation.
        /// </summary>
        /// <param name="target">The target language code.</param>
        /// <returns>>Represents a single query request to a data service.</returns>
        public LanguageTranslateCollection GetLanguagesForTranslation(string target)
        {
            string query = "";

            if ((target != null))
            {
                query += "&target=" + System.Uri.EscapeDataString(target);
            }

            if ((_apiKey != null))
            {
                query += "&key=" +_apiKey;
            }

            // Process the request.
            return ProcessRequest<LanguageTranslateCollection>("languages", query);
        }

        /// <summary>
        /// Translate the text from one language to another.
        /// </summary>
        /// <param name="text">the text to translate Sample Values : hello</param>
        /// <param name="to">the language code to translate the text into Sample Values : nl</param>
        /// <returns>Represents a single query request to a data service.</returns>
        public LanguageTranslateCollection Translate(String text, String to)
        {
            return Translate(text, to, null);
        }

        /// <summary>
        /// Translate the text from one language to another.
        /// </summary>
        /// <param name="text">the text to translate Sample Values : hello</param>
        /// <param name="to">the language code to translate the text into Sample Values : nl</param>
        /// <param name="from">the language code of the translation text Sample Values : en</param>
        /// <returns>Represents a single query request to a data service.</returns>
        public LanguageTranslateCollection Translate(String text, String to, String from)
        {
            string query = "";

            if ((text == null))
            {
                throw new System.ArgumentNullException("text", "Text value cannot be null");
            }
            if ((to == null))
            {
                throw new System.ArgumentNullException("to", "To value cannot be null");
            }

            if ((text != null))
            {
                query += "&q=" + System.Uri.EscapeDataString(text);
            }
            if ((to != null))
            {
                query += "&target=" + System.Uri.EscapeDataString(to);
            }
            if ((from != null))
            {
                query += "&source=" + System.Uri.EscapeDataString(from);
            }

            if ((_apiKey != null))
            {
                query += "&key=" + _apiKey;
            }

            // Process the request.
            return ProcessRequest<LanguageTranslateCollection>("", query);
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceName">The service name to call.</param>
        /// <param name="query">The query to apply.</param>
        /// <returns>The returned type.</returns>
        private T ProcessRequest<T>(string serviceName, string query)
        {
            // Is the connection secure.
            bool isSecure = false;
            if (_serviceRoot.Scheme.ToLower() == "https")
                isSecure = true;

            // Construct the URI.
            Uri constructedServiceUri = new Uri(_serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + "/" + serviceName + "?" + query.TrimStart(new char[] { '&' }));
            T data = default(T);

            // Open a new connection.
            using (Nequeo.Net.Client client = new Client(constructedServiceUri, isSecureConnection: isSecure))
            {
                client.IsHttpProtocol = true;

                // Create the request.
                Nequeo.Net.NetRequest request = client.GetRequest();
                request.Method = "GET";
                request.ContentLength = 0;
                client.Transfer(request, null, CancellationToken.None);
                Nequeo.Net.NetContext context = client.GetContext();

                // Read all the headers.
                string resources = null;
                List<Model.NameValue> headers = Nequeo.Net.Utility.ParseHeaders(context.NetResponse.Input, out resources, client.NetClient.ResponseTimeout);

                // Parse the headers and the resource response.
                context.NetResponse.ReadNetResponseHeaders(headers, resources);

                // Open a stream.
                using (MemoryStream stream = new MemoryStream())
                {
                    // If sending chunked.
                    if (context.NetResponse.SendChunked)
                        Nequeo.Net.Utility.ReadChunkedData(context.NetResponse.Input, stream, client.NetClient.ResponseTimeout);
                    else
                        // Copy the response stream.
                        Nequeo.IO.Stream.Operation.CopyStream(context.NetResponse.Input, stream, context.NetResponse.ContentLength, client.NetClient.ResponseTimeout);

                    // Create the object from the json data.
                    string json = Encoding.UTF8.GetString(stream.ToArray());
                    data = Nequeo.Serialisation.JavaObjectNotation.Deserializer<T>(json);
                }
            }

            // Data.
            return data;
        }
    }
}
