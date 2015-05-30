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

namespace Nequeo.Net.OAuth2.Framework.Resource
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Net;
	using System.Security.Principal;
	using System.ServiceModel.Channels;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Web;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.Resource.ChannelElements;
	

	/// <summary>
	/// Provides services for validating OAuth access tokens.
	/// </summary>
    public class ResourceServer
    {
        /// <summary>
        /// A reusable instance of the scope satisfied checker.
        /// </summary>
        private static readonly IScopeSatisfiedCheck DefaultScopeSatisfiedCheck = new StandardScopeSatisfiedCheck();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServer"/> class.
        /// </summary>
        /// <param name="accessTokenAnalyzer">The access token analyzer.</param>
        public ResourceServer(IAccessTokenAnalyzer accessTokenAnalyzer)
        {

            this.AccessTokenAnalyzer = accessTokenAnalyzer;
            this.Channel = new OAuth2ResourceServerChannel();
            this.ResourceOwnerPrincipalPrefix = string.Empty;
            this.ClientPrincipalPrefix = "client:";
            this.ScopeSatisfiedCheck = DefaultScopeSatisfiedCheck;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServer"/> class.
        /// </summary>
        public ResourceServer()
        {
            this.Channel = new OAuth2ResourceServerChannel();
            this.ResourceOwnerPrincipalPrefix = string.Empty;
            this.ClientPrincipalPrefix = "client:";
            this.ScopeSatisfiedCheck = DefaultScopeSatisfiedCheck;
        }

        /// <summary>
        /// Gets the access token analyzer.
        /// </summary>
        /// <value>The access token analyzer.</value>
        public IAccessTokenAnalyzer AccessTokenAnalyzer { get; set; }

        /// <summary>
        /// Gets or sets the service that checks whether a granted set of scopes satisfies a required set of scopes.
        /// </summary>
        public IScopeSatisfiedCheck ScopeSatisfiedCheck { get; set; }

        /// <summary>
        /// Gets or sets the prefix to apply to a resource owner's username when used as the username in an <see cref="IPrincipal"/>.
        /// </summary>
        /// <value>The default value is the empty string.</value>
        public string ResourceOwnerPrincipalPrefix { get; set; }

        /// <summary>
        /// Gets or sets the prefix to apply to a client identifier when used as the username in an <see cref="IPrincipal"/>.
        /// </summary>
        /// <value>The default value is "client:"</value>
        public string ClientPrincipalPrefix { get; set; }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>The channel.</value>
        internal OAuth2ResourceServerChannel Channel { get; private set; }

        /// <summary>
        /// Discovers what access the client should have considering the access token in the current request.
        /// </summary>
        /// <param name="httpRequestInfo">The HTTP request info.</param>
        /// <param name="requiredScopes">The set of scopes required to approve this request.</param>
        /// <returns>
        /// The access token describing the authorization the client has.  Never <c>null</c>.
        /// </returns>
        /// <exception cref="ProtocolFaultResponseException">
        /// Thrown when the client is not authorized.  This exception should be caught and the
        /// <see cref="ProtocolFaultResponseException.ErrorResponseMessage"/> message should be returned to the client.
        /// </exception>
        public virtual AccessToken GetAccessToken(HttpRequestBase httpRequestInfo = null, params string[] requiredScopes)
        {

            if (httpRequestInfo == null)
            {
                httpRequestInfo = this.Channel.GetRequestFromContext();
            }

            AccessToken accessToken;
            AccessProtectedResourceRequest request = null;
            try
            {
                if (this.Channel.TryReadFromRequest<AccessProtectedResourceRequest>(httpRequestInfo, out request))
                {
                    accessToken = this.AccessTokenAnalyzer.DeserializeAccessToken(request, request.AccessToken);
                    ErrorUtilities.VerifyHost(accessToken != null, "IAccessTokenAnalyzer.DeserializeAccessToken returned a null reslut.");
                    if (string.IsNullOrEmpty(accessToken.UserDataAndNonce) && string.IsNullOrEmpty(accessToken.ClientIdentifier))
                    {

                        ErrorUtilities.ThrowProtocol(ResourceServerStrings.InvalidAccessToken);
                    }

                    var requiredScopesSet = OAuthUtilities.ParseScopeSet(requiredScopes);
                    if (!this.ScopeSatisfiedCheck.IsScopeSatisfied(requiredScope: requiredScopesSet, grantedScope: accessToken.Scope))
                    {
                        var response = UnauthorizedResponse.InsufficientScope(request, requiredScopesSet);
                        throw new ProtocolFaultResponseException(this.Channel, response);
                    }

                    return accessToken;
                }
                else
                {
                    var ex = new ProtocolException(ResourceServerStrings.MissingAccessToken);
                    var response = UnauthorizedResponse.InvalidRequest(ex);
                    throw new ProtocolFaultResponseException(this.Channel, response, innerException: ex);
                }
            }
            catch (ProtocolException ex)
            {
                if (ex is ProtocolFaultResponseException)
                {
                    // This doesn't need to be wrapped again.
                    throw;
                }

                var response = request != null ? UnauthorizedResponse.InvalidToken(request, ex) : UnauthorizedResponse.InvalidRequest(ex);
                throw new ProtocolFaultResponseException(this.Channel, response, innerException: ex);
            }
        }

        /// <summary>
        /// Discovers what access the client should have considering the access token in the current request.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="nonce">The access token nonce.</param>
        /// <param name="httpRequestInfo">The HTTP request info.</param>
        /// <param name="requiredScopes">The set of scopes required to approve this request.</param>
        /// <returns>
        /// The principal that contains the user and roles that the access token is authorized for.  Never <c>null</c>.
        /// </returns>
        /// <exception cref="ProtocolFaultResponseException">
        /// Thrown when the client is not authorized.  This exception should be caught and the
        /// <see cref="ProtocolFaultResponseException.ErrorResponseMessage"/> message should be returned to the client.
        /// </exception>
        public virtual IPrincipal GetPrincipal(AccessToken accessToken, out string userID, out string nonce,  HttpRequestBase httpRequestInfo = null, params string[] requiredScopes)
        {
            // Mitigates attacks on this approach of differentiating clients from resource owners
            // by checking that a username doesn't look suspiciously engineered to appear like the other type.
            ErrorUtilities.VerifyProtocol(accessToken.UserDataAndNonce == null || string.IsNullOrEmpty(this.ClientPrincipalPrefix) || !accessToken.UserDataAndNonce.StartsWith(this.ClientPrincipalPrefix, StringComparison.OrdinalIgnoreCase), ResourceServerStrings.ResourceOwnerNameLooksLikeClientIdentifier);
            ErrorUtilities.VerifyProtocol(accessToken.ClientIdentifier == null || string.IsNullOrEmpty(this.ResourceOwnerPrincipalPrefix) || !accessToken.ClientIdentifier.StartsWith(this.ResourceOwnerPrincipalPrefix, StringComparison.OrdinalIgnoreCase), ResourceServerStrings.ClientIdentifierLooksLikeResourceOwnerName);

            string[] userNonce = accessToken.UserDataAndNonce.Split(new char[] { '_' });
            userID = userNonce[0];
            nonce = userNonce[1];

            string principalUserName = !string.IsNullOrEmpty(accessToken.UserDataAndNonce)
                ? this.ResourceOwnerPrincipalPrefix + userID
                : this.ClientPrincipalPrefix + accessToken.ClientIdentifier;
            string[] principalScope = accessToken.Scope != null ? accessToken.Scope.ToArray() : new string[0];
            var principal = new OAuthPrincipal(principalUserName, principalScope);

            return principal;
        }

        /// <summary>
        /// Discovers what access the client should have considering the access token in the current request.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="nonce">The access token nonce.</param>
        /// <param name="request">HTTP details from an incoming WCF message.</param>
        /// <param name="requestUri">The URI of the WCF service endpoint.</param>
        /// <param name="requiredScopes">The set of scopes required to approve this request.</param>
        /// <returns>
        /// The principal that contains the user and roles that the access token is authorized for.  Never <c>null</c>.
        /// </returns>
        /// <exception cref="ProtocolFaultResponseException">
        /// Thrown when the client is not authorized.  This exception should be caught and the
        /// <see cref="ProtocolFaultResponseException.ErrorResponseMessage"/> message should be returned to the client.
        /// </exception>
        public virtual IPrincipal GetPrincipal(AccessToken accessToken, out string userID, out string nonce, HttpRequestMessageProperty request, Uri requestUri, params string[] requiredScopes)
        {
            return this.GetPrincipal(accessToken, out userID, out nonce, new HttpRequestInfo(request, requestUri), requiredScopes);
        }
    }
}
