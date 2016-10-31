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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Services.Client;

namespace Nequeo.Net.Translator.Microsoft.Cognitive
{
    /// <summary>
    /// Microsoft translator service query container.
    /// </summary>
    internal class ApiContainer
    {
        /// <summary>
        /// Microsoft api translator service query container.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        /// <param name="apiKey">The api key used to access this service.</param>
        /// <param name="clientIP">The client IP address sending the request.</param>
        /// <param name="clientID">The client ID sending the request.</param>
        /// <param name="userAgent">The request user agent</param>
        public ApiContainer(Uri serviceRoot, string apiKey, string clientIP = null, string clientID = null, string userAgent = null)
        {
            _apiKey = apiKey;
            _serviceRoot = serviceRoot;
            _clientIP = clientIP;
            _clientID = clientID;
            _userAgent = userAgent;
        }

        private string _apiKey = null;
        private Uri _serviceRoot = null;
        private string _clientIP = null;
        private string _clientID = null;
        private string _userAgent = null;

        /// <summary>
        /// Get the access token.
        /// </summary>
        /// <param name="serviceExtension">The service extension ([serviceRoot]/issueToken).</param>
        /// <returns>The Bearer + access token.</returns>
        public string GetAccessToken(string serviceExtension = "issueToken")
        {
            // Make the request.
            byte[] accessToken = ProcessRequest("/" + serviceExtension, null, new NetRequest(), "POST", false);
            string accessTokenBearer = Encoding.Default.GetString(accessToken);
            return "Bearer " + accessTokenBearer;
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
            string queryString = "";
            NetRequest request = new NetRequest();

            // If the authorization has been set and appid is null.
            if (String.IsNullOrEmpty(authorization) && String.IsNullOrEmpty(appid))
            {
                // Error throw
                throw new ArgumentNullException(nameof(appid) + " " + nameof(authorization), "The 'appid' or the 'authorization' must be set.");
            }

            // If the authorization has been set and appid is null.
            if (!String.IsNullOrEmpty(authorization) && String.IsNullOrEmpty(appid))
            {
                // Add the header.
                request.AddHeader("Authorization", authorization);
            }
            else
            {
                queryString += "&appid=" + System.Uri.EscapeDataString(appid);
            }

            // Set the query.
            if ((text != null))
            {
                queryString += "&text=" + System.Uri.EscapeDataString(text);
            }
            if ((to != null))
            {
                queryString += "&to=" + System.Uri.EscapeDataString(to);
            }
            if ((from != null))
            {
                queryString += "&from=" + System.Uri.EscapeDataString(from);
            }
            if ((contentType != null))
            {
                queryString += "&contentType=" + System.Uri.EscapeDataString(contentType);
            }
            if ((category != null))
            {
                queryString += "&category=" + System.Uri.EscapeDataString(category);
            }

            // Make the request.
            return ProcessRequest("/" + "Translate", queryString, request);
        }

        /// <summary>
        /// Obtain a list of language codes representing languages that are supported by the Translation Service.
        /// </summary>
        /// <param name="appid">If the Authorization is used, leave the appid field empty else specify a string containing "Bearer" + " " + access token.</param>
        /// <param name="authorization">Authorization token: "Bearer" + " " + access token. Required if the appid field is not specified.</param>
        /// <returns>A string array containing languages names supported by the Translator Service, localized into the requested language.</returns>
        public byte[] GetLanguagesForTranslate(string appid = null, string authorization = null)
        {
            string queryString = "";
            NetRequest request = new NetRequest();

            // If the authorization has been set and appid is null.
            if (String.IsNullOrEmpty(authorization) && String.IsNullOrEmpty(appid))
            {
                // Error throw
                throw new ArgumentNullException(nameof(appid) + " " + nameof(authorization), "The 'appid' or the 'authorization' must be set.");
            }

            // If the authorization has been set and appid is null.
            if (!String.IsNullOrEmpty(authorization) && String.IsNullOrEmpty(appid))
            {
                // Add the header.
                request.AddHeader("Authorization", authorization);
            }
            else
            {
                queryString += "&appid=" + System.Uri.EscapeDataString(appid);
            }

            // Make the request.
            return ProcessRequest("/" + "GetLanguagesForTranslate", queryString, request);
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
            string queryString = "";
            NetRequest request = new NetRequest();

            // If the authorization has been set and appid is null.
            if (String.IsNullOrEmpty(authorization) && String.IsNullOrEmpty(appid))
            {
                // Error throw
                throw new ArgumentNullException(nameof(appid) + " " + nameof(authorization), "The 'appid' or the 'authorization' must be set.");
            }

            // If the authorization has been set and appid is null.
            if (!String.IsNullOrEmpty(authorization) && String.IsNullOrEmpty(appid))
            {
                // Add the header.
                request.AddHeader("Authorization", authorization);
            }
            else
            {
                queryString += "&appid=" + System.Uri.EscapeDataString(appid);
            }

            // Set the query.
            if ((text != null))
            {
                queryString += "&text=" + System.Uri.EscapeDataString(text);
            }

            // Make the request.
            return ProcessRequest("/" + "Detect", queryString, request);
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceName">The service name to call.</param>
        /// <param name="query">The query to apply.</param>
        /// <param name="request">The request.</param>
        /// <param name="method">The request method.</param>
        /// <param name="hasAccessToken">Does the request have an access token.</param>
        /// <returns>The returned type.</returns>
        private T ProcessRequestJson<T>(string serviceName, string query, Net.NetRequest request, string method = "GET", bool hasAccessToken = true)
        {
            // Construct the URI.
            Uri constructedServiceUri = new Uri(
                _serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) + 
                (String.IsNullOrEmpty(serviceName) ? "" : serviceName) + 
                (String.IsNullOrEmpty(query) ? "" : "?" + query.TrimStart(new char[] { '&' })));

            // Create the request.
            request.Method = method;
            request.AddHeader("Content-Length", (0).ToString());

            // If requesting an access token.
            if (!hasAccessToken)
            {
                request.AddHeader("Ocp-Apim-Subscription-Key", _apiKey);

                if ((_clientIP != null))
                {
                    request.AddHeader("X-Search-ClientIP", _clientIP);
                }
                if ((_clientID != null))
                {
                    request.AddHeader("X-MSEdge-ClientID", _clientID);
                }
                if ((_userAgent != null))
                {
                    request.AddHeader("User-Agent", _userAgent);
                }
            }

            // Return the result.
            return Nequeo.Net.HttpJsonClient.Request<T>(constructedServiceUri, request);
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceName">The service name to call.</param>
        /// <param name="query">The query to apply.</param>
        /// <param name="request">The request.</param>
        /// <param name="method">The request method.</param>
        /// <param name="hasAccessToken">Does the request have an access token.</param>
        /// <returns>The returned type.</returns>
        private T ProcessRequestXml<T>(string serviceName, string query, Net.NetRequest request, string method = "GET", bool hasAccessToken = true)
        {
            // Construct the URI.
            Uri constructedServiceUri = new Uri(
                _serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) +
                (String.IsNullOrEmpty(serviceName) ? "" : serviceName) +
                (String.IsNullOrEmpty(query) ? "" : "?" + query.TrimStart(new char[] { '&' })));

            // Create the request.
            request.Method = method;
            request.AddHeader("Content-Length", (0).ToString());

            // If requesting an access token.
            if (!hasAccessToken)
            {
                request.AddHeader("Ocp-Apim-Subscription-Key", _apiKey);

                if ((_clientIP != null))
                {
                    request.AddHeader("X-Search-ClientIP", _clientIP);
                }
                if ((_clientID != null))
                {
                    request.AddHeader("X-MSEdge-ClientID", _clientID);
                }
                if ((_userAgent != null))
                {
                    request.AddHeader("User-Agent", _userAgent);
                }
            }

            // Return the result.
            return Nequeo.Net.HttpXmlClient.Request<T>(constructedServiceUri, request);
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="serviceName">The service name to call.</param>
        /// <param name="query">The query to apply.</param>
        /// <param name="request">The request.</param>
        /// <param name="method">The request method.</param>
        /// <param name="hasAccessToken">Does the request have an access token.</param>
        /// <returns>The returned type.</returns>
        private byte[] ProcessRequest(string serviceName, string query, Net.NetRequest request, string method = "GET", bool hasAccessToken = true)
        {
            // Construct the URI.
            Uri constructedServiceUri = new Uri(
                _serviceRoot.AbsoluteUri.TrimEnd(new char[] { '/' }) +
                (String.IsNullOrEmpty(serviceName) ? "" : serviceName) +
                (String.IsNullOrEmpty(query) ? "" : "?" + query.TrimStart(new char[] { '&' })));

            // Create the request.
            request.Method = method;
            request.AddHeader("Content-Length", (0).ToString());

            // If requesting an access token.
            if (!hasAccessToken)
            {
                request.AddHeader("Ocp-Apim-Subscription-Key", _apiKey);

                if ((_clientIP != null))
                {
                    request.AddHeader("X-Search-ClientIP", _clientIP);
                }
                if ((_clientID != null))
                {
                    request.AddHeader("X-MSEdge-ClientID", _clientID);
                }
                if ((_userAgent != null))
                {
                    request.AddHeader("User-Agent", _userAgent);
                }
            }

            // Return the result.
            return Nequeo.Net.HttpDataClient.Request(constructedServiceUri, request);
        }
    }
}
