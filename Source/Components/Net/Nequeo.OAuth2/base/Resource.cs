/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Nequeo.Net.Core.Messaging;
using Nequeo.Net.OAuth2.Framework;
using Nequeo.Net.OAuth2.Storage;
using Nequeo.Net.OAuth2.Framework.Utility;
using Nequeo.Net.OAuth2.Provider.Session;
using Nequeo.Net.OAuth2.Provider;
using Nequeo.Net.OAuth2.Framework.Resource;
using Nequeo.Net.OAuth2.Provider.Session.ChannelElements;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.ChannelElements;

using Nequeo.Cryptography.Parser;
using Nequeo.Cryptography.Signing;
using Nequeo.Cryptography;
using Nequeo.Security;

namespace Nequeo.Net.OAuth2
{
    /// <summary>
    /// OAuth resource provider
    /// </summary>
    public class AuthResource
	{
        /// <summary>
        /// OAuth resource provider.
        /// </summary>
        /// <param name="tokenStore">The token store</param>
        /// <param name="consumerStore">The consumer store</param>
        /// <param name="nonceStore">The nonce store.</param>
        public AuthResource(ITokenStore tokenStore, IConsumerStore consumerStore, INonceStore nonceStore)
        {
            _tokenStore = tokenStore;
            _consumerStore = consumerStore;
            _nonceStore = nonceStore;

            ValidateEx();

            // Create the resource provider.
            _resourceServer = new ResourceServer();
        }

        private readonly ResourceServer _resourceServer;

        private ITokenStore _tokenStore = null;
        private IConsumerStore _consumerStore = null;
        private INonceStore _nonceStore = null;

        private string _tokenError = string.Empty;

        /// <summary>
        /// Verify that the request is valid.
        /// </summary>
        /// <param name="httpRequest">The HTTP request base.</param>
        /// <param name="rawUri">A System.Uri object containing information regarding the URL of the current request.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        /// <param name="form">The collection of form variables.</param>
        /// <param name="headers">The collection of HTTP headers.</param>
        /// <param name="requiredScopes">The set of scopes required to approve this request.</param>
        /// <returns>
        /// The principal that contains the user and roles that the access token is authorized for; else null.
        /// </returns>
        public IPrincipal VerifyAuthorisation(HttpRequestBase httpRequest, Uri rawUri, NameValueCollection queryString,
            NameValueCollection form, NameValueCollection headers, params string[] requiredScopes)
        {
            return Verify(httpRequest, rawUri, queryString, form, headers, requiredScopes);
        }

        /// <summary>
        /// Verify that the request is valid.
        /// </summary>
        /// <param name="httpRequest">HTTP details from an incoming WCF message.</param>
        /// <param name="requestUri">The URI of the WCF service endpoint.</param>
        /// <param name="rawUri">A System.Uri object containing information regarding the URL of the current request.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        /// <param name="form">The collection of form variables.</param>
        /// <param name="headers">The collection of HTTP headers.</param>
        /// <param name="requiredScopes">The set of scopes required to approve this request.</param>
        /// <returns>
        /// The principal that contains the user and roles that the access token is authorized for; else null.
        /// </returns>
        public IPrincipal VerifyAuthorisation(HttpRequestMessageProperty httpRequest, Uri requestUri, Uri rawUri, NameValueCollection queryString,
            NameValueCollection form, NameValueCollection headers, params string[] requiredScopes)
        {
            return VerifyAuthorisation(new HttpRequestInfo(httpRequest, requestUri), rawUri, queryString, form, headers, requiredScopes);
        }

        /// <summary>
        /// Verify that the request is valid.
        /// </summary>
        /// <param name="httpRequest">The HTTP request base.</param>
        /// <param name="rawUri">A System.Uri object containing information regarding the URL of the current request.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        /// <param name="form">The collection of form variables.</param>
        /// <param name="headers">The collection of HTTP headers.</param>
        /// <param name="requiredScopes">The set of scopes required to approve this request.</param>
        /// <returns>
        /// The principal that contains the user and roles that the access token is authorized for; else null.
        /// </returns>
        private IPrincipal Verify(HttpRequestBase httpRequest, Uri rawUri, NameValueCollection queryString,
            NameValueCollection form, NameValueCollection headers, params string[] requiredScopes)
        {
            string clientID = null;
            string nonce = null;
            string accessToken = null;

            string tokenNonce = null;
            string userID = null;

            try
            {
                // Make sure that all the passed parameters are valid.
                if (httpRequest == null) throw new ArgumentNullException("httpRequest");
                if (rawUri == null) throw new ArgumentNullException("rawUri");
                if (queryString == null) throw new ArgumentNullException("queryString");
                if (form == null) throw new ArgumentNullException("form");
                if (headers == null) throw new ArgumentNullException("headers");

                // Attempt to find the 'access_token' parameter in the form.
                IEnumerable<string> accessTokens = form.AllKeys.Where(u => u.EndsWith("access_token"));
                if (accessTokens == null || accessTokens.Count() < 1)
                {
                    // Attempt to find the 'access_token' parameter in the query string.
                    if (queryString != null || queryString.Keys.Count > 0)
                    {
                        if (queryString["access_token"] != null)
                            accessToken = queryString["access_token"];
                    }

                    // Attempt to find the 'access_token' parameter in the headers.
                    if (headers != null || headers.Keys.Count > 0)
                    {
                        if (headers["access_token"] != null)
                            accessToken = headers["access_token"];
                    }
                }
                else
                    accessToken = form["access_token"];

                // Pass a access token
                if (!String.IsNullOrEmpty(accessToken))
                {
                    // Get the nonce data for the code value.
                    nonce = _tokenStore.GetNonceByAccessToken(accessToken);
                    clientID = _consumerStore.GetConsumerIdentifier(nonce);

                    // Make sure that the token is still valid.
                    if (!_consumerStore.IsAuthorizationValid(clientID, nonce))
                        return null;
                    else
                    {
                        // Get the encryption certificate for the client.
                        // Create a new access token decryption analyser.
                        X509Certificate2 certificate = _consumerStore.GetConsumerCertificate(clientID);
                        StandardAccessTokenAnalyzer accessTokenAnalyzer =
                            new StandardAccessTokenAnalyzer(
                                (RSACryptoServiceProvider)certificate.PrivateKey,
                                (RSACryptoServiceProvider)certificate.PublicKey.Key);

                        // Assign the analyser and get the access token
                        // data from the http request.
                        _resourceServer.AccessTokenAnalyzer = accessTokenAnalyzer;
                        AccessToken token = _resourceServer.GetAccessToken(httpRequest, requiredScopes);

                        // Get the priciple identity of the access token request.
                        IPrincipal principal = _resourceServer.GetPrincipal(token, out userID, out tokenNonce, httpRequest, requiredScopes);
                        return principal;
                    }
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                // Get the current token errors.
                _tokenError = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Get the current OAuth provider error.
        /// </summary>
        /// <returns>The current OAuth provider error.</returns>
        public string GetTokenError()
        {
            return _tokenError;
        }

        /// <summary>
        /// Validate all the parameters
        /// </summary>
        private void ValidateEx()
        {
            if (_tokenStore == null) throw new ArgumentNullException("tokenStore");
            if (_consumerStore == null) throw new ArgumentNullException("consumerStore");
            if (_nonceStore == null) throw new ArgumentNullException("nonceStore");
        }
	}
}
