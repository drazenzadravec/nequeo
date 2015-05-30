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
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Net.OAuth2.Framework;
using Nequeo.Net.OAuth2.Storage;
using Nequeo.Net.OAuth2.Storage.Basic;
using Nequeo.Net.OAuth2.Framework.Utility;
using Nequeo.Net.OAuth2.Provider.Session;
using Nequeo.Net.OAuth2.Provider;
using Nequeo.Net.OAuth2.Provider.Session.ChannelElements;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

using Nequeo.Cryptography.Parser;
using Nequeo.Cryptography.Signing;
using Nequeo.Cryptography;
using Nequeo.Security;

namespace Nequeo.Net.OAuth2.Provider
{
    /// <summary>
    /// OAuth authorization service provider.
    /// </summary>
    public class OAuthAuthorizationServer : IAuthorizationServerHost
    {
        /// <summary>
        /// OAuth authorization service provider.
        /// </summary>
        /// <param name="tokenStore">The token store</param>
        /// <param name="consumerStore">The consumer store</param>
        /// <param name="nonceStore">The nonce store.</param>
        public OAuthAuthorizationServer(ITokenStore tokenStore, IConsumerStore consumerStore, INonceStore nonceStore)
        {
            _cryptoKeyStoreTranslate = new CryptoKeyStoreTranslate(tokenStore, consumerStore, nonceStore);
            _tokenStore = tokenStore;
            _consumerStore = consumerStore;
            _nonceStore = nonceStore;

            // Create a new instance of the absent parameter list.
            ParametersAbsent = new List<string>();
        }

        private CryptoKeyStoreTranslate _cryptoKeyStoreTranslate = null;
        private ITokenStore _tokenStore = null;
        private IConsumerStore _consumerStore = null;
        private INonceStore _nonceStore = null;

        /// <summary>
        /// Gets sets the parameters absent
        /// </summary>
        public List<string> ParametersAbsent { get; set; }

        /// <summary>
        /// Gets the cryptograhy store.
        /// </summary>
        public Storage.ICryptographyKeyStore CryptoKeyStore
        {
            get { return _cryptoKeyStoreTranslate; }
        }

        /// <summary>
        /// Gets the nonce store.
        /// </summary>
        public Storage.INonceStore NonceStore
        {
            get { return _nonceStore; }
        }

        /// <summary>
        /// Acquires the access token and related parameters that go into the formulation of the token endpoint's response to a client.
        /// </summary>
        /// <param name="accessTokenRequestMessage">Details regarding the resources that the access token will grant access to, and the identity of the client
        /// that will receive that access.
        /// Based on this information the receiving resource server can be determined and the lifetime of the access
        /// token can be set based on the sensitivity of the resources.
        /// </param>
        /// <param name="nonce">The nonce data.</param>
        /// <returns>A non-null parameters instance that will be disposed after it has been used.</returns>
        public Consumer.Session.Authorization.Messages.AccessTokenResult CreateAccessToken(Consumer.Session.Authorization.Messages.IAccessTokenRequest accessTokenRequestMessage, string nonce = null)
        {
            return _tokenStore.CreateAccessToken(accessTokenRequestMessage, nonce);
        }

        /// <summary>
        /// Gets the client with a given identifier.
        /// </summary>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <returns>The client registration.  Never null.</returns>
        /// <exception cref="ArgumentException">Thrown when no client with the given identifier is registered with this authorization server.</exception>
        public Consumer.Session.Authorization.IClientDescription GetClient(string clientIdentifier)
        {
            return _consumerStore.GetClient(clientIdentifier);
        }

        /// <summary>
        /// Determines whether a described authorization is (still) valid.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        /// <returns>True if the original authorization is still valid; otherwise, false</returns>
        public bool IsAuthorizationValid(Framework.ChannelElements.IAuthorizationDescription authorization)
        {
            return _consumerStore.IsAuthorizationValid(authorization);
        }

        /// <summary>
        /// Determines whether a given set of resource owner credentials is valid based on the authorization server's user database.
        /// </summary>
        /// <param name="userName">Username on the account.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="accessRequest">
        /// The access request the credentials came with.
        /// This may be useful if the authorization server wishes to apply some policy based on the client that is making the request.
        /// </param>
        /// <param name="canonicalUserName">
        /// Receives the canonical username (normalized for the resource server) of the user, for valid credentials;
        /// Or <c>null</c> if the return value is false.
        /// </param>
        /// <returns>
        ///   <c>true</c> if the given credentials are valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsResourceOwnerCredentialValid(string userName, string password, Consumer.Session.Authorization.Messages.IAccessTokenRequest accessRequest, out string canonicalUserName)
        {
            return _consumerStore.IsResourceOwnerCredentialValid(userName, password, accessRequest, out canonicalUserName);
        }

        /// <summary>
        /// Validate request parameters absent
        /// </summary>
        /// <param name="parameters">The collection of request parameters</param>
        public void ValidateAuthoriseRequestParametersAbsent(NameValueCollection parameters)
        {
            List<string> maditoryParameters = new List<string>()
            {
                "com_unique_uid".ToLower(),
                "client_id".ToLower(),
                "redirect_uri".ToLower(),
                "response_type".ToLower()
            };

            foreach (string parameter in maditoryParameters)
            {
                if (!parameters.AllKeys.Contains(parameter))
                    ParametersAbsent.Add(parameter);
            }
        }

        /// <summary>
        /// Gets the refresh token parameters.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="form">The forms collection parameters</param>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        public void GetRefreshTokenData(NameValueCollection queryString, NameValueCollection form, out string clientIdentifier, out string clientSecret)
        {
            clientIdentifier = null;
            clientSecret = null;

            // Attempt to find the 'client_id' parameter in the form.
            IEnumerable<string> clientIdentifiers = form.AllKeys.Where(u => u.EndsWith("client_id"));
            if (clientIdentifiers == null || clientIdentifiers.Count() < 1)
            {
                // Attempt to find the 'client_id' parameter in the query string.
                if (queryString != null || queryString.Keys.Count > 0)
                    if (queryString["client_id"] != null)
                        clientIdentifier = queryString["client_id"];
            }
            else
                clientIdentifier = form["client_id"];

            // Attempt to find the 'client_secret' parameter in the form.
            IEnumerable<string> clientSecrets = form.AllKeys.Where(u => u.EndsWith("client_secret"));
            if (clientSecrets == null || clientSecrets.Count() < 1)
            {
                // Attempt to find the 'client_secret' parameter in the query string.
                if (queryString != null || queryString.Keys.Count > 0)
                    if (queryString["client_secret"] != null)
                        clientSecret = queryString["client_secret"];
            }
            else
                clientSecret = form["client_secret"];
        }
    }
}
