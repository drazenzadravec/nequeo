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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using System.Web.Script.Serialization;

using Nequeo.Security;

namespace Nequeo.Net.OAuth
{
    /// <summary>
    /// OAuth client.
    /// </summary>
    public class OAuthClient
    {
        /// <summary>
        /// Create a new OAuth v2.0 draft 13 client.
        /// </summary>
        /// <param name="tokenUri">Token endpoint URI</param>
        /// <param name="clientId">Client ID that is registered with the website</param>
        /// <param name="clientSecret">Client secret that is registered with the website</param>
        /// <param name="redirectUri">URI to redirect to with the authorization code response, and that is also registered on the website</param>
        /// <param name="tokenEndpointScope">scope parameter to pass when refershing the token. May be null.</param>
        public OAuthClient(Uri tokenUri, string clientId, SecureString clientSecret, string redirectUri, string tokenEndpointScope)
        {
            if (tokenUri == null)
            {
                throw new ArgumentNullException("tokenUri");
            }
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (clientSecret == null)
            {
                throw new ArgumentNullException("clientSecret");
            }
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }

            _tokenUri = tokenUri;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;

            if (!string.IsNullOrEmpty(tokenEndpointScope))
            {
                _tokenEndpointScopeEncoded = HttpUtility.HtmlEncode(tokenEndpointScope);
            }
        }

        private Uri _tokenUri;
        private string _redirectUri;
        private string _clientId;
        private SecureString _clientSecret;
        private string _tokenEndpointScopeEncoded;

        /// <summary>
        /// The Redirect URI that the client gives to the OAuth Server
        /// </summary>
        public string RedirectUri
        {
            get { return _redirectUri; }
        }

        /// <summary>
        /// Get the URI query to start the OAuth consent flow
        /// </summary>
        /// <param name="consentUri">URI of the OAuth consent endpoint</param>
        /// <param name="permissions">Permissions to request. May be null.</param>
        /// <param name="requiredOffers">Required offers to purchase in checkout.</param>
        /// <returns>Full consent URI with query params.</returns>
        public Uri GenerateAuthorizationRequestUri(Uri consentUri, string permissions, string requiredOffers)
        {
            if (consentUri == null)
            {
                throw new ArgumentNullException("consentUri");
            }

            string query = string.Format("?response_type=code&client_id={0}&redirect_uri={1}", HttpUtility.HtmlEncode(_clientId), HttpUtility.UrlEncode(_redirectUri));

            if (!string.IsNullOrEmpty(permissions))
                query += "&x_permissions=" + permissions;

            if (!string.IsNullOrEmpty(requiredOffers))
                query += "&x_required_offers=" + requiredOffers;

            if (!string.IsNullOrEmpty(_tokenEndpointScopeEncoded))
                query += "&scope=" + _tokenEndpointScopeEncoded;

            return new Uri(consentUri, query);
        }

        /// <summary>
        /// Process an Authorization Response URI, per section 4.1.2 of the OAuth 2.0 specification
        /// </summary>
        /// <exception cref="ProtocolViolationException">If <paramref name="authRedirectUri"/> is malformed, or the access code resonse is malformed</exception>
        /// <exception cref="OAuthErrorException">If there was an error in the response. This can include the user denying the authorization request.</exception>
        /// <param name="authRedirectUri">URI to process</param>
        public OAuthCredential ProcessAuthorizationResponse(Uri authRedirectUri)
        {
            if (authRedirectUri == null) 
            {
                throw new ArgumentNullException("authRedirectUri");
            }

            if (authRedirectUri.Query == null) 
            {
                throw new ProtocolViolationException("Query params are expected in an authorization response");
            }

            var queryParams = HttpUtility.ParseQueryString(authRedirectUri.Query);
            if (queryParams.AllKeys.Contains("code"))
            {
                queryParams["grant_type"] = "authorization_code";
                queryParams["redirect_uri"] = HttpUtility.HtmlEncode(_redirectUri);
                // Authorization succeeded - extract the code and proceed to Request an Authorization token.
                return RequestOrRefreshAuthToken(queryParams);
            }
            else
            {
                // If a code wasn't received, and error occured. Parse the error parameters according to section 4.1.2.1 of the OAuth 2.0 spec
                throw new OAuthErrorException(queryParams["error"], queryParams["error_uri"], queryParams["error_description"], queryParams["state"]);
            }

        }

        // a simple class we use to parse an OAuth JSON response before we create an OAuthCredentials class
        private class OAuthAccessTokenResponseJson
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public string expires_in { get; set; }
            public string state { get; set; }
        }

        /// <summary>
        /// TODO Comment
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private OAuthCredential RequestOrRefreshAuthToken(NameValueCollection values)
        {
            OAuthCredential credentials;
            try
            {
                using (var client = new WebClient { BaseAddress = _tokenUri.ToString() })
                {
                    // Construct the Access Token Request request, per OAuth 2.0 spec at 4.1.3, or Refresh Token request in section 6
                    values["scope"] = _tokenEndpointScopeEncoded;
                    values["client_id"] = HttpUtility.HtmlEncode(_clientId);
                    values["client_secret"] = HttpUtility.HtmlEncode(_clientSecret.ConvertToString());

                    var responseBytes = client.UploadValues(_tokenUri.AbsolutePath, "POST", values);

                    OAuthAccessTokenResponseJson resultJson = null;
                    try
                    {
                        JavaScriptSerializer ser = new JavaScriptSerializer();
                        resultJson = ser.Deserialize<OAuthAccessTokenResponseJson>(client.Encoding.GetString(responseBytes));
                    }
                    // catch various exceptions and filter them to a ProtocolViolationException, as if not, it is a violation of OAuth protocol.
                    catch (ArgumentException)
                    {
                        throw new ProtocolViolationException("Response could not be parsed");
                    }
                    catch (InvalidOperationException)
                    {
                        throw new ProtocolViolationException("Response could not be parsed");
                    }

                    if (resultJson != null)
                    {
                        if (string.IsNullOrEmpty(resultJson.refresh_token))
                        {
                            // Create a new set of credentials that doesn't expire
                            credentials = new OAuthCredential(resultJson.access_token);
                        }
                        else
                        {
                            try
                            {
                                // expires_in should always be with a refresh_token.
                                int expires_in = Int32.Parse(resultJson.expires_in);
                                credentials = new OAuthCredential(resultJson.access_token, resultJson.refresh_token, expires_in, this);
                            }
                            catch (FormatException)
                            {
                                throw new ProtocolViolationException("expires_in is malformed");
                            }
                        }
                    }
                    else
                    {
                        throw new ProtocolViolationException("Response wasn't in JSON format.");
                    }
                }
            }
            catch (WebException ex)
            {
                // Error conditions are returned as 400 Bad Request, per section 5.2 of the OAuth 2.0 spec
                if (ex.Response != null && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadRequest)
                {
                    OAuthErrorException errorEx = OAuthErrorException.FromWebException(ex);
                    if (errorEx != null)
                    {
                        throw errorEx;
                    }
                }

                throw;
            }

            return credentials;
        }

        /// <summary>
        /// Refresh the access token, per Section 6 of OAuth 2.0 spec
        /// </summary>
        /// <param name="refreshToken">Refresh Token from the original Authorization Response</param>
        /// <returns>A new set of credentials. The refresh token may have been updated.</returns>
        public OAuthCredential RefreshAccessToken(SecureString refreshToken)
        {
            var values = HttpUtility.ParseQueryString("grant_type=refresh_token");
            values["refresh_token"] = HttpUtility.HtmlEncode(refreshToken.ConvertToString());
            return RequestOrRefreshAuthToken(values);
        }
    }
}
