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

namespace Nequeo.Net.OAuth2.Consumer.Session
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Text;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Consumer.Session.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

    /// <summary>
    /// A base class for common OAuth Client behaviors.
    /// </summary>
    public class ClientBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientBase"/> class.
        /// </summary>
        /// <param name="authorizationServer">The token issuer.</param>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="clientCredentialApplicator">
        /// The tool to use to apply client credentials to authenticated requests to the Authorization Server.  
        /// May be <c>null</c> for clients with no secret or other means of authentication.
        /// </param>
        protected ClientBase(AuthorizationServerDescription authorizationServer, string clientIdentifier = null, ClientCredentialApplicator clientCredentialApplicator = null)
        {
            this.AuthorizationServer = authorizationServer;
            this.Channel = new OAuth2ClientChannel();
            this.ClientIdentifier = clientIdentifier;
            this.ClientCredentialApplicator = clientCredentialApplicator;
        }

        /// <summary>
        /// Gets the token issuer.
        /// </summary>
        /// <value>The token issuer.</value>
        public AuthorizationServerDescription AuthorizationServer { get; private set; }

        /// <summary>
        /// Gets the OAuth channel.
        /// </summary>
        /// <value>The channel.</value>
        public Channel Channel { get; internal set; }

        /// <summary>
        /// Gets or sets the identifier by which this client is known to the Authorization Server.
        /// </summary>
        public string ClientIdentifier
        {
            get { return this.OAuthChannel.ClientIdentifier; }
            set { this.OAuthChannel.ClientIdentifier = value; }
        }

        /// <summary>
        /// Gets or sets the tool to use to apply client credentials to authenticated requests to the Authorization Server.
        /// </summary>
        /// <value>May be <c>null</c> if this client has no client secret.</value>
        public ClientCredentialApplicator ClientCredentialApplicator
        {
            get { return this.OAuthChannel.ClientCredentialApplicator; }
            set { this.OAuthChannel.ClientCredentialApplicator = value; }
        }

        /// <summary>
        /// Gets the OAuth client channel.
        /// </summary>
        internal IOAuth2ChannelWithClient OAuthChannel
        {
            get { return (IOAuth2ChannelWithClient)this.Channel; }
        }

        /// <summary>
        /// Adds the necessary HTTP Authorization header to an HTTP request for protected resources
        /// so that the Service Provider will allow the request through.
        /// </summary>
        /// <param name="request">The request for protected resources from the service provider.</param>
        /// <param name="accessToken">The access token previously obtained from the Authorization Server.</param>
        public static void AuthorizeRequest(HttpWebRequest request, string accessToken)
        {
            OAuthUtilities.AuthorizeWithBearerToken(request, accessToken);
        }

        /// <summary>
        /// Adds the OAuth authorization token to an outgoing HTTP request, renewing a
        /// (nearly) expired access token if necessary.
        /// </summary>
        /// <param name="request">The request for protected resources from the service provider.</param>
        /// <param name="authorization">The authorization for this request previously obtained via OAuth.</param>
        public void AuthorizeRequest(HttpWebRequest request, IAuthorizationState authorization)
        {
            ErrorUtilities.VerifyProtocol(!authorization.AccessTokenExpirationUtc.HasValue || authorization.AccessTokenExpirationUtc < DateTime.UtcNow || authorization.RefreshToken != null, ClientStrings.AuthorizationExpired);

            if (authorization.AccessTokenExpirationUtc.HasValue && authorization.AccessTokenExpirationUtc.Value < DateTime.UtcNow)
            {
                ErrorUtilities.VerifyProtocol(authorization.RefreshToken != null, ClientStrings.AccessTokenRefreshFailed);
                this.RefreshAuthorization(authorization);
            }

            AuthorizeRequest(request, authorization.AccessToken);
        }

        /// <summary>
        /// Refreshes a short-lived access token using a longer-lived refresh token
        /// with a new access token that has the same scope as the refresh token.
        /// The refresh token itself may also be refreshed.
        /// </summary>
        /// <param name="authorization">The authorization to update.</param>
        /// <param name="skipIfUsefulLifeExceeds">If given, the access token will <em>not</em> be refreshed if its remaining lifetime exceeds this value.</param>
        /// <returns>A value indicating whether the access token was actually renewed; <c>true</c> if it was renewed, or <c>false</c> if it still had useful life remaining.</returns>
        /// <remarks>
        /// This method may modify the value of the <see cref="IAuthorizationState.RefreshToken"/> property on
        /// the <paramref name="authorization"/> parameter if the authorization server has cycled out your refresh token.
        /// If the parameter value was updated, this method calls <see cref="IAuthorizationState.SaveChanges"/> on that instance.
        /// </remarks>
        public bool RefreshAuthorization(IAuthorizationState authorization, TimeSpan? skipIfUsefulLifeExceeds = null)
        {
            if (skipIfUsefulLifeExceeds.HasValue && authorization.AccessTokenExpirationUtc.HasValue)
            {
                TimeSpan usefulLifeRemaining = authorization.AccessTokenExpirationUtc.Value - DateTime.UtcNow;
                if (usefulLifeRemaining > skipIfUsefulLifeExceeds.Value)
                {
                    return false;
                }
            }

            var request = new AccessTokenRefreshRequestC(this.AuthorizationServer)
            {
                ClientIdentifier = this.ClientIdentifier,
                RefreshToken = authorization.RefreshToken,
            };

            this.ApplyClientCredential(request);

            var response = this.Channel.Request<AccessTokenSuccessResponse>(request);
            UpdateAuthorizationWithResponse(authorization, response);
            return true;
        }

        /// <summary>
        /// Gets an access token that may be used for only a subset of the scope for which a given
        /// refresh token is authorized.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="scope">The scope subset desired in the access token.</param>
        /// <returns>A description of the obtained access token, and possibly a new refresh token.</returns>
        /// <remarks>
        /// If the return value includes a new refresh token, the old refresh token should be discarded and
        /// replaced with the new one.
        /// </remarks>
        public IAuthorizationState GetScopedAccessToken(string refreshToken, HashSet<string> scope)
        {
            Contract.Ensures(Contract.Result<IAuthorizationState>() != null);

            var request = new AccessTokenRefreshRequestC(this.AuthorizationServer)
            {
                ClientIdentifier = this.ClientIdentifier,
                RefreshToken = refreshToken,
            };

            this.ApplyClientCredential(request);

            var response = this.Channel.Request<AccessTokenSuccessResponse>(request);
            var authorization = new AuthorizationState();
            UpdateAuthorizationWithResponse(authorization, response);

            return authorization;
        }

        /// <summary>
        /// Exchanges a resource owner's password credential for OAuth 2.0 refresh and access tokens.
        /// </summary>
        /// <param name="userName">The resource owner's username, as it is known by the authorization server.</param>
        /// <param name="password">The resource owner's account password.</param>
        /// <param name="scopes">The desired scope of access.</param>
        /// <returns>The result, containing the tokens if successful.</returns>
        public IAuthorizationState ExchangeUserCredentialForToken(string userName, string password, IEnumerable<string> scopes = null)
        {
            var request = new AccessTokenResourceOwnerPasswordCredentialsRequest(this.AuthorizationServer.TokenEndpoint, this.AuthorizationServer.Version)
            {
                UserName = userName,
                Password = password,
            };

            return this.RequestAccessToken(request, scopes);
        }

        /// <summary>
        /// Obtains an access token for accessing client-controlled resources on the resource server.
        /// </summary>
        /// <param name="scopes">The desired scopes.</param>
        /// <returns>The result of the authorization request.</returns>
        public IAuthorizationState GetClientAccessToken(IEnumerable<string> scopes = null)
        {
            var request = new AccessTokenClientCredentialsRequest(this.AuthorizationServer.TokenEndpoint, this.AuthorizationServer.Version);
            return this.RequestAccessToken(request, scopes);
        }

        /// <summary>
        /// Updates the authorization state maintained by the client with the content of an outgoing response.
        /// </summary>
        /// <param name="authorizationState">The authorization state maintained by the client.</param>
        /// <param name="accessTokenSuccess">The access token containing response message.</param>
        internal static void UpdateAuthorizationWithResponse(IAuthorizationState authorizationState, AccessTokenSuccessResponse accessTokenSuccess)
        {
            authorizationState.AccessToken = accessTokenSuccess.AccessToken;
            authorizationState.AccessTokenExpirationUtc = DateTime.UtcNow + accessTokenSuccess.Lifetime;
            authorizationState.AccessTokenIssueDateUtc = DateTime.UtcNow;

            // The authorization server MAY choose to renew the refresh token itself.
            if (accessTokenSuccess.RefreshToken != null)
            {
                authorizationState.RefreshToken = accessTokenSuccess.RefreshToken;
            }

            // An included scope parameter in the response only describes the access token's scope.
            // Don't update the whole authorization state object with that scope because that represents
            // the refresh token's original scope.
            if ((authorizationState.Scope == null || authorizationState.Scope.Count == 0) && accessTokenSuccess.Scope != null)
            {
                authorizationState.Scope.ResetContents(accessTokenSuccess.Scope);
            }

            authorizationState.SaveChanges();
        }

        /// <summary>
        /// Updates the authorization state maintained by the client with the content of an outgoing response.
        /// </summary>
        /// <param name="authorizationState">The authorization state maintained by the client.</param>
        /// <param name="accessTokenSuccess">The access token containing response message.</param>
        internal static void UpdateAuthorizationWithResponse(IAuthorizationState authorizationState, EndUserAuthorizationSuccessAccessTokenResponse accessTokenSuccess)
        {
            authorizationState.AccessToken = accessTokenSuccess.AccessToken;
            authorizationState.AccessTokenExpirationUtc = DateTime.UtcNow + accessTokenSuccess.Lifetime;
            authorizationState.AccessTokenIssueDateUtc = DateTime.UtcNow;
            if (accessTokenSuccess.Scope != null && accessTokenSuccess.Scope != authorizationState.Scope)
            {
                if (authorizationState.Scope != null)
                {
                }

                authorizationState.Scope.ResetContents(accessTokenSuccess.Scope);
            }

            authorizationState.SaveChanges();
        }

        /// <summary>
        /// Updates authorization state with a success response from the Authorization Server.
        /// </summary>
        /// <param name="authorizationState">The authorization state to update.</param>
        /// <param name="authorizationSuccess">The authorization success message obtained from the authorization server.</param>
        internal void UpdateAuthorizationWithResponse(IAuthorizationState authorizationState, EndUserAuthorizationSuccessAuthCodeResponse authorizationSuccess)
        {
            var accessTokenRequest = new AccessTokenAuthorizationCodeRequestC(this.AuthorizationServer)
            {
                ClientIdentifier = this.ClientIdentifier,
                Callback = authorizationState.Callback,
                AuthorizationCode = authorizationSuccess.AuthorizationCode,
            };
            this.ApplyClientCredential(accessTokenRequest);
            IProtocolMessage accessTokenResponse = this.Channel.Request(accessTokenRequest);
            var accessTokenSuccess = accessTokenResponse as AccessTokenSuccessResponse;
            var failedAccessTokenResponse = accessTokenResponse as AccessTokenFailedResponse;
            if (accessTokenSuccess != null)
            {
                UpdateAuthorizationWithResponse(authorizationState, accessTokenSuccess);
            }
            else
            {
                authorizationState.Delete();
                string error = failedAccessTokenResponse != null ? failedAccessTokenResponse.Error : "(unknown)";
                ErrorUtilities.ThrowProtocol(ClientStrings.CannotObtainAccessTokenWithReason, error);
            }
        }

        /// <summary>
        /// Applies the default client authentication mechanism given a client secret.
        /// </summary>
        /// <param name="secret">The client secret.  May be <c>null</c></param>
        /// <returns>The client credential applicator.</returns>
        protected static ClientCredentialApplicator DefaultSecretApplicator(string secret)
        {
            return secret == null ? ClientCredentialApplicator.NoSecret() : ClientCredentialApplicator.NetworkCredential(secret);
        }

        /// <summary>
        /// Applies any applicable client credential to an authenticated outbound request to the authorization server.
        /// </summary>
        /// <param name="request">The request to apply authentication information to.</param>
        protected void ApplyClientCredential(AuthenticatedClientRequestBase request)
        {
            if (this.ClientCredentialApplicator != null)
            {
                this.ClientCredentialApplicator.ApplyClientCredential(this.ClientIdentifier, request);
            }
        }

        /// <summary>
        /// Calculates the fraction of life remaining in an access token.
        /// </summary>
        /// <param name="authorization">The authorization to measure.</param>
        /// <returns>A fractional number no greater than 1.  Could be negative if the access token has already expired.</returns>
        private static double ProportionalLifeRemaining(IAuthorizationState authorization)
        {
            // Calculate what % of the total life this access token has left.
            TimeSpan totalLifetime = authorization.AccessTokenExpirationUtc.Value - authorization.AccessTokenIssueDateUtc.Value;
            TimeSpan elapsedLifetime = DateTime.UtcNow - authorization.AccessTokenIssueDateUtc.Value;
            double proportionLifetimeRemaining = 1 - (elapsedLifetime.TotalSeconds / totalLifetime.TotalSeconds);
            return proportionLifetimeRemaining;
        }

        /// <summary>
        /// Requests an access token using a partially .initialized request message.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="scopes">The scopes requested by the client.</param>
        /// <returns>The result of the request.</returns>
        private IAuthorizationState RequestAccessToken(ScopedAccessTokenRequest request, IEnumerable<string> scopes = null)
        {
            var authorizationState = new AuthorizationState(scopes);

            request.ClientIdentifier = this.ClientIdentifier;
            this.ApplyClientCredential(request);
            request.Scope.UnionWith(authorizationState.Scope);

            var response = this.Channel.Request(request);
            var success = response as AccessTokenSuccessResponse;
            var failure = response as AccessTokenFailedResponse;
            ErrorUtilities.VerifyProtocol(success != null || failure != null, MessagingStrings.UnexpectedMessageReceivedOfMany);
            if (success != null)
            {
                UpdateAuthorizationWithResponse(authorizationState, success);
            }
            else
            { 
                // failure
                authorizationState.Delete();
            }

            return authorizationState;
        }
    }
}
