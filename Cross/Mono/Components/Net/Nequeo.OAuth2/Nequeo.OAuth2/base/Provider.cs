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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

using Nequeo.Net.Core.Messaging;
using Nequeo.Net.OAuth2.Framework;
using Nequeo.Net.OAuth2.Storage;
using Nequeo.Net.OAuth2.Framework.Utility;
using Nequeo.Net.OAuth2.Provider.Session;
using Nequeo.Net.OAuth2.Provider;
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
    /// OAuth provider
    /// </summary>
    public class AuthProvider
	{
        /// <summary>
        /// OAuth provider.
        /// </summary>
        /// <param name="tokenStore">The token store</param>
        /// <param name="consumerStore">The consumer store</param>
        /// <param name="nonceStore">The nonce store.</param>
        public AuthProvider(ITokenStore tokenStore, IConsumerStore consumerStore, INonceStore nonceStore)
        {
            _tokenStore = tokenStore;
            _consumerStore = consumerStore;
            _nonceStore = nonceStore;

            ValidateEx();

            // Create a new client authenticator
            List<ClientAuthenticationModule> clientAuth = new List<ClientAuthenticationModule>()
            {
                new ClientAuthentication(tokenStore, consumerStore, nonceStore)
            };

            // Create the OAuth servers.
            _oAuthAuthorizationServer = new OAuthAuthorizationServer(tokenStore, consumerStore, nonceStore);
            _authorizationServer = new AuthorizationServer(_oAuthAuthorizationServer, clientAuth);
        }

        private readonly AuthorizationServer _authorizationServer;
        private OAuthAuthorizationServer _oAuthAuthorizationServer;

        private ITokenStore _tokenStore = null;
        private IConsumerStore _consumerStore = null;
        private INonceStore _nonceStore = null;

        private string _tokenError = string.Empty;

        /// <summary>
        /// Create a request token from the request.
        /// </summary>
        /// <param name="httpRequest">The current http request.</param>
        /// <param name="rawUri">A System.Uri object containing information regarding the URL of the current request.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        /// <param name="form">The collection of form variables.</param>
        /// <param name="headers">The collection of HTTP headers.</param>
        /// <param name="cookies">The collection of cookies sent by the client.</param>
        /// <param name="responseHeaders">The response headers for the request.</param>
        /// <returns>The token if successful; else null.</returns>
        public string CreateTokenBody(HttpRequestBase httpRequest, Uri rawUri, NameValueCollection queryString,
            NameValueCollection form, NameValueCollection headers, HttpCookieCollection cookies, 
            out System.Net.WebHeaderCollection responseHeaders)
        {
            object ret = CreateToken(httpRequest, rawUri, queryString, form, headers, cookies, out responseHeaders, 0);
            return (String)ret ?? null;
        }
			

        /// <summary>
        /// Create a authorise token from the request. Returns the uri result.
        /// </summary>
        /// <param name="httpRequest">The current http request.</param>
        /// <param name="rawUri">A System.Uri object containing information regarding the URL of the current request.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        /// <param name="form">The collection of form variables.</param>
        /// <param name="headers">The collection of HTTP headers.</param>
        /// <param name="cookies">The collection of cookies sent by the client.</param>
        /// <param name="responseHeaders">The response headers for the request.</param>
        /// <param name="isApprovedByUser">Has the user approved the client to access the resources.</param>
        /// <returns>The formatted redirect url; else null.</returns>
        public Uri CreateAuthoriseUri(HttpRequestBase httpRequest, Uri rawUri, NameValueCollection queryString,
            NameValueCollection form, NameValueCollection headers, HttpCookieCollection cookies, 
            out System.Net.WebHeaderCollection responseHeaders, bool isApprovedByUser = false)
        {
            object ret = CreateAuthorise(httpRequest, rawUri, queryString, form, headers, cookies, 0, out responseHeaders, isApprovedByUser);
            return (Uri)ret ?? null;
        }

        /// <summary>
        /// Create a authorise token from the request. Returns the body html result.
        /// </summary>
        /// <param name="httpRequest">The current http request.</param>
        /// <param name="rawUri">A System.Uri object containing information regarding the URL of the current request.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        /// <param name="form">The collection of form variables.</param>
        /// <param name="headers">The collection of HTTP headers.</param>
        /// <param name="cookies">The collection of cookies sent by the client.</param>
        /// <param name="responseHeaders">The response headers for the request.</param>
        /// <param name="isApprovedByUser">Has the user approved the client to access the resources.</param>
        /// <returns>The formatted redirect url; else null.</returns>
        public string CreateAuthoriseBody(HttpRequestBase httpRequest, Uri rawUri, NameValueCollection queryString,
            NameValueCollection form, NameValueCollection headers, HttpCookieCollection cookies, 
            out System.Net.WebHeaderCollection responseHeaders, bool isApprovedByUser = false)
        {
            object ret = CreateAuthorise(httpRequest, rawUri, queryString, form, headers, cookies, 1, out responseHeaders, isApprovedByUser);
            return (String)ret ?? null;
        }

        /// <summary>
        /// Create a request token from the request.
        /// </summary>
        /// <param name="httpRequest">The current http request.</param>
        /// <param name="rawUri">A System.Uri object containing information regarding the URL of the current request.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        /// <param name="form">The collection of form variables.</param>
        /// <param name="headers">The collection of HTTP headers.</param>
        /// <param name="cookies">The collection of cookies sent by the client.</param>
        /// <param name="responseHeaders">The response headers for the request.</param>
        /// <param name="returnType">The type of response to return.</param>
        /// <returns>The token if successful; else null.</returns>
        private object CreateToken(HttpRequestBase httpRequest, Uri rawUri, NameValueCollection queryString,
            NameValueCollection form, NameValueCollection headers, HttpCookieCollection cookies,
            out System.Net.WebHeaderCollection responseHeaders, int returnType)
        {
            OutgoingWebResponse outgoingWebResponse = null;
            AccessTokenSuccessResponse accessTokenSuccessResponse = null;
            IProtocolMessage message = null;
            string codeKey = null;
            string refreshToken = null;
            string clientID = null;
            string nonce = null;

            try
            {
                // Make sure that all the passed parameters are valid.
                if (httpRequest == null) throw new ArgumentNullException("httpRequest");
                if (rawUri == null) throw new ArgumentNullException("rawUri");
                if (queryString == null) throw new ArgumentNullException("queryString");
                if (form == null) throw new ArgumentNullException("form");
                if (headers == null) throw new ArgumentNullException("headers");
                if (cookies == null) throw new ArgumentNullException("cookies");

                // Set the crytography key store values.
                _authorizationServer.AuthorizationServerServices.CryptoKeyStore.ExpiryDateTime = DateTime.UtcNow.AddYears(1);
                _authorizationServer.AuthorizationServerServices.CryptoKeyStore.GetCodeKey = true;

                // Attempt to find the 'code' parameter in the form.
                IEnumerable<string> codeKeys = form.AllKeys.Where(u => u.EndsWith("code"));
                if (codeKeys == null || codeKeys.Count() < 1)
                {
                    // Attempt to find the 'code' parameter in the query string.
                    if (queryString != null || queryString.Keys.Count > 0)
                        if (queryString["code"] != null)
                            codeKey = queryString["code"];
                }
                else
                    codeKey = form["code"];

                // If a code value exists.
                if (!String.IsNullOrEmpty(codeKey))
                {
                    // Get the nonce data for the code value.
                    nonce = _tokenStore.GetNonce(codeKey);
                }

                // Attempt to find the 'refresh_token' parameter in the form.
                IEnumerable<string> refreshTokens = form.AllKeys.Where(u => u.EndsWith("refresh_token"));
                if (refreshTokens == null || refreshTokens.Count() < 1)
                {
                    // Attempt to find the 'refresh_token' parameter in the query string.
                    if (queryString != null || queryString.Keys.Count > 0)
                        if (queryString["refresh_token"] != null)
                            refreshToken = queryString["refresh_token"];
                }
                else
                    refreshToken = form["refresh_token"];
                
                // Pass a refresh token
                if (!String.IsNullOrEmpty(refreshToken))
                {
                    string clientIdentifier = null;
                    string clientSecret = null;

                    // Get the refresh token data from the http request.
                    _oAuthAuthorizationServer.GetRefreshTokenData(queryString, form, out clientIdentifier, out clientSecret);

                    // Get the nonce data for the code value.
                    nonce = _tokenStore.GetNonce(refreshToken, clientIdentifier, clientSecret);
                }

                // Handles an incoming request to the authorization server's token endpoint.
                message = _authorizationServer.HandleTokenRequest(nonce, out clientID, out accessTokenSuccessResponse, httpRequest);

                // Set the crytography key store values after finding the client identifier.
                _authorizationServer.AuthorizationServerServices.CryptoKeyStore.ClientIndetifier = clientID;

                // Handles an incoming request to the authorization server's token endpoint.
                outgoingWebResponse = _authorizationServer.HandleTokenRequestPrepareResponse(message);

                // Update the access token.
                if (accessTokenSuccessResponse != null)
                    if (!String.IsNullOrEmpty(accessTokenSuccessResponse.AccessToken))
                        _tokenStore.UpdateAccessToken(accessTokenSuccessResponse.AccessToken, nonce, accessTokenSuccessResponse.RefreshToken);

                // What type should be returned.
                switch (returnType)
                {
                    case 0:
                        // The complete html body.
                        responseHeaders = outgoingWebResponse.Headers;
                        return outgoingWebResponse.Body;

                    default:
                        // Default is html body.
                        responseHeaders = outgoingWebResponse.Headers;
                        return outgoingWebResponse.Body;
                }
            }
            catch (Exception ex)
            {
                // Get the current token errors.
                responseHeaders = null;
                _tokenError = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Create a authorise token from the request. Returns the bdy html result.
        /// </summary>
        /// <param name="httpRequest">The current http request.</param>
        /// <param name="rawUri">A System.Uri object containing information regarding the URL of the current request.</param>
        /// <param name="queryString">The collection of HTTP query string variables.</param>
        /// <param name="form">The collection of form variables.</param>
        /// <param name="headers">The collection of HTTP headers.</param>
        /// <param name="cookies">The collection of cookies sent by the client.</param>
        /// <param name="returnType">The type of response to return.</param>
        /// <param name="responseHeaders">The response headers for the request.</param>
        /// <param name="isApprovedByUser">Has the user approved the client to access the resources.</param>
        /// <returns>The formatted redirect url; else null.</returns>
        private object CreateAuthorise(HttpRequestBase httpRequest, Uri rawUri, NameValueCollection queryString,
            NameValueCollection form, NameValueCollection headers, HttpCookieCollection cookies, int returnType, 
            out System.Net.WebHeaderCollection responseHeaders, bool isApprovedByUser)
        {
            IDirectedProtocolMessage response = null;
            OutgoingWebResponse webResponse = null;
            string clientID = null;
            string nonce = null;
            string codeKey = null;

            try
            {
                // Make sure that all the passed parameters are valid.
                if (httpRequest == null) throw new ArgumentNullException("httpRequest");
                if (rawUri == null) throw new ArgumentNullException("rawUri");
                if (queryString == null) throw new ArgumentNullException("queryString");
                if (form == null) throw new ArgumentNullException("form");
                if (headers == null) throw new ArgumentNullException("headers");
                if (cookies == null) throw new ArgumentNullException("cookies");

                // Read the request make sure it is valid.
                EndUserAuthorizationRequest pendingRequest = _authorizationServer.ReadAuthorizationRequest(httpRequest);
                if (pendingRequest == null)
                    throw new Exception("Missing authorization request.");

                // Only process if the user has approved the request.
                if (isApprovedByUser)
                {
                    // Make sure all maditor parameters are present.
                    _oAuthAuthorizationServer.ValidateAuthoriseRequestParametersAbsent(queryString);
                    if (_oAuthAuthorizationServer.ParametersAbsent.Count() > 0)
                        throw new Exception("Some authorisation request parameters are missing.");

                    // Assign each query string parameter.
                    clientID = pendingRequest.ClientIdentifier;
                    string callback = pendingRequest.Callback.ToString();
                    string state = pendingRequest.ClientState;
                    string scope = OAuthUtilities.JoinScopes(pendingRequest.Scope);
                    string responseType = (pendingRequest.ResponseType == EndUserAuthorizationResponseType.AccessToken ? "token" : "code");
                    string companyUniqueUserID = queryString["com_unique_uid"];

                    // Set the crytography key store values.
                    _authorizationServer.AuthorizationServerServices.CryptoKeyStore.ExpiryDateTime = DateTime.UtcNow.AddYears(1);
                    _authorizationServer.AuthorizationServerServices.CryptoKeyStore.ClientIndetifier = clientID;
                    _authorizationServer.AuthorizationServerServices.CryptoKeyStore.GetCodeKey = false;

                    // Create a new nonce and store it in the nonce store.
                    nonce = _nonceStore.GenerateNonce();
                    _nonceStore.StoreNonce(DateTime.UtcNow, nonce, clientID);

                    // Create the access token from the stores, and create a new verification code.
                    string verifier = _consumerStore.SetVerificationCode(clientID, nonce, companyUniqueUserID, scope);
                    EndUserAuthorizationSuccessAccessTokenResponse successAccessTokenResponse = null;

                    // Prepare the request. pass the nonce and join the userID and nonce of
                    // the user that approved the resource access request.
                    response = _authorizationServer.PrepareApproveAuthorizationRequest(
                        pendingRequest, 
                        companyUniqueUserID + "_" + nonce, 
                        nonce, 
                        out successAccessTokenResponse);

                    // Prepare the authorisation response.
                    webResponse = _authorizationServer.Channel.PrepareResponse(response);

                    // Create the query collection of the code request
                    // and extract the code value that is to be sent
                    // the the client.
                    NameValueCollection queryResponseString = new NameValueCollection();
                    Uri uriRequest = webResponse.GetDirectUriRequest(_authorizationServer.Channel);

                    // For each query item.
                    string[] queries = uriRequest.Query.Split(new char[] { '&' });
                    foreach (string query in queries)
                    {
                        // Add the query name and value to the collection.
                        string[] queriesNameValue = query.Split(new char[] { '=' });
                        queryResponseString.Add(queriesNameValue[0].TrimStart(new char[] { '?' }), queriesNameValue[1]);
                    }

                    // What type of response is to be handled.
                    switch (pendingRequest.ResponseType)
                    {
                        case EndUserAuthorizationResponseType.AuthorizationCode:
                            // The user has requested a code, this is
                            // used so the client can get a token later.
                            // If the code response type exits.
                            if (queryResponseString["code"] != null)
                                codeKey = HttpUtility.UrlDecode(queryResponseString["code"]);

                            // Insert the code key (code or token);
                            if (!String.IsNullOrEmpty(codeKey))
                                _tokenStore.StoreCodeKey(clientID, nonce, codeKey);
                            break;

                        case EndUserAuthorizationResponseType.AccessToken:
                            // This is used so the client is approved and a token is sent back.
                            // Update the access token.
                            if (successAccessTokenResponse != null)
                                if (!String.IsNullOrEmpty(successAccessTokenResponse.AccessToken))
                                    _tokenStore.UpdateAccessToken(successAccessTokenResponse.AccessToken, nonce);
                            break;
                    }
                }
                else
                    // Send an error response.
                    response = _authorizationServer.PrepareRejectAuthorizationRequest(pendingRequest);

                // What type should be returned.
                switch (returnType)
                {
                    case 0:
                        // A URI request redirect only.
                        responseHeaders = webResponse.Headers;
                        return webResponse.GetDirectUriRequest(_authorizationServer.Channel);

                    case 1:
                        // The complete html body.
                        responseHeaders = webResponse.Headers;
                        return webResponse.Body;

                    default:
                        // Default is the complete html body.
                        responseHeaders = webResponse.Headers;
                        return webResponse.Body;
                }
            }
            catch (Exception ex)
            {
                // Get the current token errors.
                responseHeaders = null;
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
