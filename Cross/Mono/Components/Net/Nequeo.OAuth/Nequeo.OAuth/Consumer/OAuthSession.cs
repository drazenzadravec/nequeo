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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.IO;

using Nequeo.Net.OAuth.Storage;
using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Consumer.Session;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;
using Nequeo.Net.OAuth.Provider.Inspectors;

namespace Nequeo.Net.OAuth.Consumer
{
    /// <summary>
    /// OAuth session
    /// </summary>
    [Serializable]
    public class OAuthSession : IOAuthSession
    {
        readonly NameValueCollection _cookies = new NameValueCollection();
        readonly NameValueCollection _formParameters = new NameValueCollection();
        readonly NameValueCollection _headers = new NameValueCollection();
        readonly NameValueCollection _queryParameters = new NameValueCollection();
        IConsumerRequestFactory _consumerRequestFactory = ConsumerRequestFactory.Instance;

        /// <summary>
        /// OAuth session
        /// </summary>
        /// <param name="consumerContext">The consumer context</param>
        public OAuthSession(IOAuthConsumerContext consumerContext)
            : this(consumerContext, (Uri)null, null, null, null)
        {
        }

        /// <summary>
        /// OAuth session
        /// </summary>
        /// <param name="consumerContext">The consumer context</param>
        /// <param name="endPointUri">The uri end point</param>
        public OAuthSession(IOAuthConsumerContext consumerContext, Uri endPointUri)
            : this(consumerContext, endPointUri, endPointUri, endPointUri, null)
        {
        }

        /// <summary>
        /// OAuth session
        /// </summary>
        /// <param name="consumerContext">The consumer context</param>
        /// <param name="requestTokenUri">The request token uri</param>
        /// <param name="userAuthorizeUri">The user authorise uri</param>
        /// <param name="accessTokenUri">The access token uri</param>
        public OAuthSession(IOAuthConsumerContext consumerContext, Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri)
            : this(consumerContext, requestTokenUri, userAuthorizeUri, accessTokenUri, null)
        {
        }

        /// <summary>
        /// OAuth session
        /// </summary>
        /// <param name="consumerContext">The consumer context</param>
        /// <param name="requestTokenUri">The request token uri</param>
        /// <param name="userAuthorizeUri">The user authorise uri</param>
        /// <param name="accessTokenUri">The access token uri</param>
        /// <param name="callBackUri">The call back uri</param>
        public OAuthSession(IOAuthConsumerContext consumerContext, Uri requestTokenUri, Uri userAuthorizeUri,
                            Uri accessTokenUri, Uri callBackUri)
        {
            ConsumerContext = consumerContext;
            RequestTokenUri = requestTokenUri;
            AccessTokenUri = accessTokenUri;
            UserAuthorizeUri = userAuthorizeUri;
            CallbackUri = callBackUri;
        }

        /// <summary>
        /// OAuth session
        /// </summary>
        /// <param name="consumerContext">The consumer context</param>
        /// <param name="requestTokenUrl">The request token url</param>
        /// <param name="userAuthorizeUrl">The user authorise url</param>
        /// <param name="accessTokenUrl">The access token url</param>
        /// <param name="callBackUrl">The call back url</param>
        public OAuthSession(IOAuthConsumerContext consumerContext, string requestTokenUrl, string userAuthorizeUrl,
                            string accessTokenUrl, string callBackUrl)
            : this(consumerContext, new Uri(requestTokenUrl), new Uri(userAuthorizeUrl), new Uri(accessTokenUrl), ParseCallbackUri(callBackUrl))
        {
        }

        /// <summary>
        /// OAuth session
        /// </summary>
        /// <param name="consumerContext">The consumer context</param>
        /// <param name="requestTokenUrl">The request token url</param>
        /// <param name="userAuthorizeUrl">The user authorise url</param>
        /// <param name="accessTokenUrl">The access token url</param>
        public OAuthSession(IOAuthConsumerContext consumerContext, string requestTokenUrl, string userAuthorizeUrl,
                            string accessTokenUrl)
            : this(consumerContext, requestTokenUrl, userAuthorizeUrl, accessTokenUrl, null)
        {
        }

        /// <summary>
        /// Gets sets the consumer factory
        /// </summary>
        public IConsumerRequestFactory ConsumerRequestFactoryBase
        {
            get { return _consumerRequestFactory; }
            set
            {
                if (_consumerRequestFactory == null) throw new ArgumentNullException("value");
                _consumerRequestFactory = value;
            }
        }

        /// <summary>
        /// Gets sets the callback must be confirmed
        /// </summary>
        public bool CallbackMustBeConfirmed { get; set; }

        /// <summary>
        /// Gets sets the callback uri
        /// </summary>
        public Uri CallbackUri { get; set; }

        /// <summary>
        /// Gets sets the add body hashes to raw requests
        /// </summary>
        public bool AddBodyHashesToRawRequests { get; set; }

        /// <summary>
        /// Gets sets the response body action
        /// </summary>
        public Action<string> ResponseBodyAction { get; set; }

        /// <summary>
        /// Gets sets the consumer context
        /// </summary>
        public IOAuthConsumerContext ConsumerContext { get; set; }

        /// <summary>
        /// Gets sets the request token uri
        /// </summary>
        public Uri RequestTokenUri { get; set; }

        /// <summary>
        /// Gets sets the access token uri
        /// </summary>
        public Uri AccessTokenUri { get; set; }

        /// <summary>
        /// Gets sets the user authorize uri
        /// </summary>
        public Uri UserAuthorizeUri { get; set; }

        /// <summary>
        /// Gets sets the proxy server uri
        /// </summary>
        public Uri ProxyServerUri { get; set; }

        /// <summary>
        /// Gets sets the access token
        /// </summary>
        public IToken AccessToken { get; set; }

        /// <summary>
        /// Get the consumer request
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <returns>The consumer request.</returns>
        public IConsumerRequest Request(IToken accessToken)
        {
            var context = new OAuthContext
            {
                UseAuthorizationHeader = ConsumerContext.UseHeaderForOAuthParameters,
                IncludeOAuthRequestBodyHashInSignature = AddBodyHashesToRawRequests
            };

            context.Cookies.Add(_cookies);
            context.FormEncodedParameters.Add(_formParameters);
            context.Headers.Add(_headers);
            context.QueryParameters.Add(_queryParameters);

            IConsumerRequest consumerRequest = _consumerRequestFactory.CreateConsumerRequest(context, ConsumerContext, accessToken);

            consumerRequest.ProxyServerUri = ProxyServerUri;
            consumerRequest.ResponseBodyAction = ResponseBodyAction;

            return consumerRequest;
        }

        /// <summary>
        /// Get the consumer request.
        /// </summary>
        /// <returns>The consumer request.</returns>
        public IConsumerRequest Request()
        {
            var context = new OAuthContext
            {
                UseAuthorizationHeader = ConsumerContext.UseHeaderForOAuthParameters,
                IncludeOAuthRequestBodyHashInSignature = AddBodyHashesToRawRequests
            };

            context.Cookies.Add(_cookies);
            context.FormEncodedParameters.Add(_formParameters);
            context.Headers.Add(_headers);
            context.QueryParameters.Add(_queryParameters);

            IConsumerRequest consumerRequest = _consumerRequestFactory.CreateConsumerRequest(context, ConsumerContext, AccessToken);

            consumerRequest.ProxyServerUri = ProxyServerUri;
            consumerRequest.ResponseBodyAction = ResponseBodyAction;

            return consumerRequest;
        }

        /// <summary>
        /// Gets the request token
        /// </summary>
        /// <returns>The token</returns>
        public IToken GetRequestToken()
        {
            return GetRequestToken("GET");
        }

        /// <summary>
        /// Exchange request token for access token
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <returns>The token</returns>
        public IToken ExchangeRequestTokenForAccessToken(IToken requestToken)
        {
            return ExchangeRequestTokenForAccessToken(requestToken, "GET", null);
        }

        /// <summary>
        /// Exchange request token for access token
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <param name="verificationCode">The verification code</param>
        /// <returns>The token</returns>
        public IToken ExchangeRequestTokenForAccessToken(IToken requestToken, string verificationCode)
        {
            return ExchangeRequestTokenForAccessToken(requestToken, "GET", verificationCode);
        }

        /// <summary>
        /// Exchange request token for access token
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <param name="method">The method</param>
        /// <param name="verificationCode">The verification code</param>
        /// <returns>The token</returns>
        public IToken ExchangeRequestTokenForAccessToken(IToken requestToken, string method, string verificationCode)
        {
            TokenBase token = BuildExchangeRequestTokenForAccessTokenContext(requestToken, method, verificationCode)
                .Select(collection =>
                        new TokenBase
                        {
                            ConsumerKey = requestToken.ConsumerKey,
                            Token = ParseResponseParameter(collection, Parameters.OAuth_Token),
                            TokenSecret = ParseResponseParameter(collection, Parameters.OAuth_Token_Secret),
                            SessionHandle = ParseResponseParameter(collection, Parameters.OAuth_Session_Handle)
                        });

            AccessToken = token;

            return token;
        }

        /// <summary>
        /// Get access token using xauth
        /// </summary>
        /// <param name="authMode">The auth mode</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The token</returns>
        public IToken GetAccessTokenUsingXAuth(string authMode, string username, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Build request token context
        /// </summary>
        /// <param name="method">The method</param>
        /// <returns>The consumer request.</returns>
        public IConsumerRequest BuildRequestTokenContext(string method)
        {
            return Request()
                .ForMethod(method)
                .AlterContext(context => context.CallbackUrl = (CallbackUri == null) ? "oob" : CallbackUri.ToString())
                .AlterContext(context => context.Token = null)
                .ForUri(RequestTokenUri)
                .SignWithoutToken();
        }

        /// <summary>
        /// Build exchange request token for access token context
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <param name="method">The method</param>
        /// <param name="verificationCode">The verification code</param>
        /// <returns>The consumer request.</returns>
        public IConsumerRequest BuildExchangeRequestTokenForAccessTokenContext(IToken requestToken, string method, string verificationCode)
        {
            return Request()
                .ForMethod(method)
                .AlterContext(context => context.Verifier = verificationCode)
                .ForUri(AccessTokenUri)
                .SignWithToken(requestToken);
        }

        /// <summary>
        /// Build access token context
        /// </summary>
        /// <param name="method">The method</param>
        /// <param name="xAuthMode">The xAuth mode</param>
        /// <param name="xAuthUsername">The xAuth username</param>
        /// <param name="xAuthPassword">The xAuth password</param>
        /// <returns>The consumer request.</returns>
        public IConsumerRequest BuildAccessTokenContext(string method, string xAuthMode, string xAuthUsername, string xAuthPassword)
        {
            return Request()
              .ForMethod(method)
              .AlterContext(context => context.XAuthUsername = xAuthUsername)
              .AlterContext(context => context.XAuthPassword = xAuthPassword)
              .AlterContext(context => context.XAuthMode = xAuthMode)
              .ForUri(AccessTokenUri)
              .SignWithoutToken();
        }

        /// <summary>
        /// Get user authorization url for token
        /// </summary>
        /// <param name="token">The token</param>
        /// <returns>The url</returns>
        public string GetUserAuthorizationUrlForToken(IToken token)
        {
            return GetUserAuthorizationUrlForToken(token, null);
        }

        /// <summary>
        /// Get user authorization url for token
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="callbackUrl">The callback url</param>
        /// <returns>The url</returns>
        public string GetUserAuthorizationUrlForToken(IToken token, string callbackUrl)
        {
            var builder = new UriBuilder(UserAuthorizeUri);

            var collection = new NameValueCollection();

            if (builder.Query != null)
            {
                collection.Add(HttpUtility.ParseQueryString(builder.Query));
            }

            if (_queryParameters != null) collection.Add(_queryParameters);

            collection[Parameters.OAuth_Token] = token.Token;

            if (!string.IsNullOrEmpty(callbackUrl))
            {
                collection[Parameters.OAuth_Callback] = callbackUrl;
            }

            builder.Query = "";

            return builder.Uri + "?" + UriUtility.FormatQueryString(collection);
        }

        /// <summary>
        /// With Form Parameters
        /// </summary>
        /// <param name="dictionary">The dictionay parameters</param>
        /// <returns>The OAuth session</returns>
        public IOAuthSession WithFormParameters(IDictionary dictionary)
        {
            return AddItems(_formParameters, dictionary);
        }

        /// <summary>
        /// With Form Parameters
        /// </summary>
        /// <param name="anonymousClass">An anonymous class</param>
        /// <returns>The OAuth session</returns>
        public IOAuthSession WithFormParameters(object anonymousClass)
        {
            return AddItems(_formParameters, anonymousClass);
        }

        /// <summary>
        /// With Query Parameters
        /// </summary>
        /// <param name="dictionary">The dictionay parameters</param>
        /// <returns>The OAuth session</returns>
        public IOAuthSession WithQueryParameters(IDictionary dictionary)
        {
            return AddItems(_queryParameters, dictionary);
        }

        /// <summary>
        /// With Query Parameters
        /// </summary>
        /// <param name="anonymousClass">An anonymous class</param>
        /// <returns>The OAuth session</returns>
        public IOAuthSession WithQueryParameters(object anonymousClass)
        {
            return AddItems(_queryParameters, anonymousClass);
        }

        /// <summary>
        /// With Cookies
        /// </summary>
        /// <param name="dictionary">The dictionay parameters</param>
        /// <returns>The OAuth session</returns>
        public IOAuthSession WithCookies(IDictionary dictionary)
        {
            return AddItems(_cookies, dictionary);
        }

        /// <summary>
        /// With Cookies
        /// </summary>
        /// <param name="anonymousClass">An anonymous class</param>
        /// <returns>The OAuth session</returns>
        public IOAuthSession WithCookies(object anonymousClass)
        {
            return AddItems(_cookies, anonymousClass);
        }

        /// <summary>
        /// With Headers
        /// </summary>
        /// <param name="dictionary">The dictionay parameters</param>
        /// <returns>The OAuth session</returns>
        public IOAuthSession WithHeaders(IDictionary dictionary)
        {
            return AddItems(_headers, dictionary);
        }

        /// <summary>
        /// With Headers
        /// </summary>
        /// <param name="anonymousClass">An anonymous class</param>
        /// <returns>The OAuth session</returns>
        public IOAuthSession WithHeaders(object anonymousClass)
        {
            return AddItems(_headers, anonymousClass);
        }

        /// <summary>
        /// Requires Callback Confirmation
        /// </summary>
        /// <returns>The OAuth session</returns>
        public IOAuthSession RequiresCallbackConfirmation()
        {
            CallbackMustBeConfirmed = true;
            return this;
        }

        /// <summary>
        /// Renew access token
        /// </summary>
        /// <param name="accessToken">The token</param>
        /// <param name="sessionHandle">The session handler</param>
        /// <returns>The new token</returns>
        public IToken RenewAccessToken(IToken accessToken, string sessionHandle)
        {
            return RenewAccessToken(accessToken, "GET", sessionHandle);
        }

        /// <summary>
        /// Renew access token
        /// </summary>
        /// <param name="accessToken">The token</param>
        /// <param name="method">The method</param>
        /// <param name="sessionHandle">The session handler</param>
        /// <returns>The new token</returns>
        public IToken RenewAccessToken(IToken accessToken, string method, string sessionHandle)
        {
            TokenBase token = BuildRenewAccessTokenContext(accessToken, method, sessionHandle)
                .Select(collection =>
                        new TokenBase
                        {
                            ConsumerKey = accessToken.ConsumerKey,
                            Token = ParseResponseParameter(collection, Parameters.OAuth_Token),
                            TokenSecret = ParseResponseParameter(collection, Parameters.OAuth_Token_Secret),
                            SessionHandle = ParseResponseParameter(collection, Parameters.OAuth_Session_Handle)
                        });

            AccessToken = token;

            return token;
        }

        /// <summary>
        /// Build renew access token context
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <param name="method">The method</param>
        /// <param name="sessionHandle">The session handler</param>
        /// <returns>The cosumer request.</returns>
        public IConsumerRequest BuildRenewAccessTokenContext(IToken requestToken, string method, string sessionHandle)
        {
            return Request()
                .ForMethod(method)
                .AlterContext(context => context.SessionHandle = sessionHandle)
                .ForUri(AccessTokenUri)
                .SignWithToken(requestToken);
        }

        /// <summary>
        /// Get request token
        /// </summary>
        /// <param name="method">The method</param>
        /// <returns>The new token</returns>
        public IToken GetRequestToken(string method)
        {
            var results = BuildRequestTokenContext(method).Select(collection =>
                    new
                    {
                        ConsumerContext.ConsumerKey,
                        Token = ParseResponseParameter(collection, Parameters.OAuth_Token),
                        TokenSecret = ParseResponseParameter(collection, Parameters.OAuth_Token_Secret),
                        CallackConfirmed = WasCallbackConfimed(collection)
                    });

            if (!results.CallackConfirmed && CallbackMustBeConfirmed)
            {
                throw Error.CallbackWasNotConfirmed();
            }

            return new TokenBase
            {
                ConsumerKey = results.ConsumerKey,
                Token = results.Token,
                TokenSecret = results.TokenSecret
            };
        }

        /// <summary>
        /// Was callback confimed
        /// </summary>
        /// <param name="parameters">The name value collection</param>
        /// <returns>True if callback confirmed; else false.</returns>
        static bool WasCallbackConfimed(NameValueCollection parameters)
        {
            string value = ParseResponseParameter(parameters, Parameters.OAuth_Callback_Confirmed);
            return (value == "true");
        }

        /// <summary>
        /// Parse callback uri
        /// </summary>
        /// <param name="callBackUrl">The callback url</param>
        /// <returns>The new callback uri</returns>
        static Uri ParseCallbackUri(string callBackUrl)
        {
            if (string.IsNullOrEmpty(callBackUrl)) return null;
            if (callBackUrl.Equals("oob", StringComparison.InvariantCultureIgnoreCase)) return null;
            return new Uri(callBackUrl);
        }

        /// <summary>
        /// Parse response parameter
        /// </summary>
        /// <param name="collection">The name value collection</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The specified parameter</returns>
        static string ParseResponseParameter(NameValueCollection collection, string parameter)
        {
            string value = (collection[parameter] ?? "").Trim();
            return (value.Length > 0) ? value : null;
        }

        /// <summary>
        /// Add items
        /// </summary>
        /// <param name="destination">The destination name value collection</param>
        /// <param name="anonymousClass">The anonymous class to add.</param>
        /// <returns>The OAuth session.</returns>
        OAuthSession AddItems(NameValueCollection destination, object anonymousClass)
        {
            return AddItems(destination, new ReflectionBasedDictionaryAdapter(anonymousClass));
        }

        /// <summary>
        /// Add items
        /// </summary>
        /// <param name="destination">The destination name value collection</param>
        /// <param name="additions">The dictionary to add.</param>
        /// <returns>The OAuth session.</returns>
        OAuthSession AddItems(NameValueCollection destination, IDictionary additions)
        {
            foreach (string parameter in additions.Keys)
            {
                destination[parameter] = Convert.ToString(additions[parameter]);
            }

            return this;
        }

        /// <summary>
        /// Enable oauth request body hashes
        /// </summary>
        /// <returns>The OAuth session.</returns>
        public IOAuthSession EnableOAuthRequestBodyHashes()
        {
            AddBodyHashesToRawRequests = true;
            return this;
        }
    }
}