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

namespace Nequeo.Net.OAuth2.Provider.Session.ChannelElements
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Framework.ChannelElements;
    using Nequeo.Net.OAuth2.Consumer.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Consumer.Session.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;
    using Nequeo.Net.OAuth2.Provider.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Provider.Session.Messages;

    /// <summary>
    /// A guard for all messages to or from an Authorization Server to ensure that they are well formed,
    /// have valid secrets, callback URIs, etc.
    /// </summary>
    /// <remarks>
    /// This binding element also ensures that the code/token coming in is issued to
    /// the same client that is sending the code/token and that the authorization has
    /// not been revoked and that an access token has not expired.
    /// </remarks>
    internal class MessageValidationBindingElement : AuthServerBindingElementBase
    {
        /// <summary>
        /// The aggregating client authentication module.
        /// </summary>
        private readonly ClientAuthenticationModule clientAuthenticationModule;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidationBindingElement"/> class.
        /// </summary>
        /// <param name="clientAuthenticationModule">The aggregating client authentication module.</param>
        internal MessageValidationBindingElement(ClientAuthenticationModule clientAuthenticationModule)
        {
            this.clientAuthenticationModule = clientAuthenticationModule;
        }

        /// <summary>
        /// Gets the protection commonly offered (if any) by this binding element.
        /// </summary>
        /// <remarks>
        /// This value is used to assist in sorting binding elements in the channel stack.
        /// </remarks>
        public override MessageProtections Protection
        {
            get { return MessageProtections.None; }
        }

        /// <summary>
        /// Prepares a message for sending based on the rules of this channel binding element.
        /// </summary>
        /// <param name="message">The message to prepare for sending.</param>
        /// <returns>
        /// The protections (if any) that this binding element applied to the message.
        /// Null if this binding element did not even apply to this binding element.
        /// </returns>
        /// <remarks>
        /// Implementations that provide message protection must honor the
        /// <see cref="MessagePartAttribute.RequiredProtection"/> properties where applicable.
        /// </remarks>
        public override MessageProtections? ProcessOutgoingMessage(IProtocolMessage message)
        {
            var accessTokenResponse = message as AccessTokenSuccessResponse;
            if (accessTokenResponse != null)
            {
                var directResponseMessage = (IDirectResponseProtocolMessage)accessTokenResponse;
                var accessTokenRequest = (AccessTokenRequestBase)directResponseMessage.OriginatingRequest;
                ErrorUtilities.VerifyProtocol(accessTokenRequest.GrantType != GrantType.ClientCredentials || accessTokenResponse.RefreshToken == null, OAuthStrings.NoGrantNoRefreshToken);
            }

            return null;
        }

        /// <summary>
        /// Performs any transformation on an incoming message that may be necessary and/or
        /// validates an incoming message based on the rules of this channel binding element.
        /// </summary>
        /// <param name="message">The incoming message to process.</param>
        /// <returns>
        /// The protections (if any) that this binding element applied to the message.
        /// Null if this binding element did not even apply to this binding element.
        /// </returns>
        /// <exception cref="ProtocolException">
        /// Thrown when the binding element rules indicate that this message is invalid and should
        /// NOT be processed.
        /// </exception>
        /// <remarks>
        /// Implementations that provide message protection must honor the
        /// <see cref="MessagePartAttribute.RequiredProtection"/> properties where applicable.
        /// </remarks>
        public override MessageProtections? ProcessIncomingMessage(IProtocolMessage message)
        {
            bool applied = false;

            // Check that the client secret is correct for client authenticated messages.
            var clientCredentialOnly = message as AccessTokenClientCredentialsRequest;
            var authenticatedClientRequest = message as AuthenticatedClientRequestBase;
            var accessTokenRequest = authenticatedClientRequest as AccessTokenRequestBase; // currently the only type of message.
            var resourceOwnerPasswordCarrier = message as AccessTokenResourceOwnerPasswordCredentialsRequest;
            if (authenticatedClientRequest != null)
            {
                string clientIdentifier;
                var result = this.clientAuthenticationModule.TryAuthenticateClient(this.AuthServerChannel.AuthorizationServer, authenticatedClientRequest, out clientIdentifier);
                switch (result)
                {
                    case ClientAuthenticationResult.ClientAuthenticated:
                        break;
                    case ClientAuthenticationResult.NoAuthenticationRecognized:
                    case ClientAuthenticationResult.ClientIdNotAuthenticated:
                        // The only grant type that allows no client credentials is the resource owner credentials grant.
                        AuthServerUtilities.TokenEndpointVerify(resourceOwnerPasswordCarrier != null, accessTokenRequest, Protocol.AccessTokenRequestErrorCodes.InvalidClient, this.clientAuthenticationModule, AuthServerStrings.ClientSecretMismatch);
                        break;
                    default:
                        AuthServerUtilities.TokenEndpointVerify(false, accessTokenRequest, Protocol.AccessTokenRequestErrorCodes.InvalidClient, this.clientAuthenticationModule, AuthServerStrings.ClientSecretMismatch);
                        break;
                }

                authenticatedClientRequest.ClientIdentifier = result == ClientAuthenticationResult.NoAuthenticationRecognized ? null : clientIdentifier;
                accessTokenRequest.ClientAuthenticated = result == ClientAuthenticationResult.ClientAuthenticated;
                applied = true;
            }

            // Check that any resource owner password credential is correct.
            if (resourceOwnerPasswordCarrier != null)
            {
                try
                {
                    string canonicalUserName;
                    if (this.AuthorizationServer.IsResourceOwnerCredentialValid(resourceOwnerPasswordCarrier.UserName, resourceOwnerPasswordCarrier.Password, resourceOwnerPasswordCarrier, out canonicalUserName))
                    {
                        ErrorUtilities.VerifyHost(!string.IsNullOrEmpty(canonicalUserName), "IsResourceOwnerCredentialValid did not initialize out parameter.");
                        resourceOwnerPasswordCarrier.CredentialsValidated = true;
                        resourceOwnerPasswordCarrier.UserName = canonicalUserName;
                    }
                    else
                    {
                        throw new TokenEndpointProtocolException(accessTokenRequest, Protocol.AccessTokenRequestErrorCodes.InvalidGrant, AuthServerStrings.InvalidResourceOwnerPasswordCredential);
                    }
                }
                catch (NotSupportedException)
                {
                    throw new TokenEndpointProtocolException(accessTokenRequest, Protocol.AccessTokenRequestErrorCodes.UnsupportedGrantType);
                }
                catch (NotImplementedException)
                {
                    throw new TokenEndpointProtocolException(accessTokenRequest, Protocol.AccessTokenRequestErrorCodes.UnsupportedGrantType);
                }
            }

            // Check that authorization requests come with an acceptable callback URI.
            var authorizationRequest = message as EndUserAuthorizationRequest;
            if (authorizationRequest != null)
            {
                var client = this.AuthorizationServer.GetClientOrThrow(authorizationRequest.ClientIdentifier);
                ErrorUtilities.VerifyProtocol(authorizationRequest.Callback == null || client.IsCallbackAllowed(authorizationRequest.Callback), AuthServerStrings.ClientCallbackDisallowed, authorizationRequest.Callback);
                ErrorUtilities.VerifyProtocol(authorizationRequest.Callback != null || client.DefaultCallback != null, AuthServerStrings.NoCallback);
                applied = true;
            }

            // Check that the callback URI in a direct message from the client matches the one in the indirect message received earlier.
            var request = message as AccessTokenAuthorizationCodeRequestAS;
            if (request != null)
            {
                IAuthorizationCodeCarryingRequest tokenRequest = request;
                tokenRequest.AuthorizationDescription.VerifyCallback(request.Callback);
                applied = true;
            }

            var authCarrier = message as IAuthorizationCarryingRequest;
            if (authCarrier != null)
            {
                var accessRequest = authCarrier as AccessTokenRequestBase;
                if (accessRequest != null)
                {
                    // Make sure the client sending us this token is the client we issued the token to.
                    AuthServerUtilities.TokenEndpointVerify(string.Equals(accessRequest.ClientIdentifier, authCarrier.AuthorizationDescription.ClientIdentifier, StringComparison.Ordinal), accessTokenRequest, Protocol.AccessTokenRequestErrorCodes.InvalidClient);

                    var scopedAccessRequest = accessRequest as ScopedAccessTokenRequest;
                    if (scopedAccessRequest != null)
                    {
                        // Make sure the scope the client is requesting does not exceed the scope in the grant.
                        if (!this.AuthServerChannel.ScopeSatisfiedCheck.IsScopeSatisfied(requiredScope: scopedAccessRequest.Scope, grantedScope: authCarrier.AuthorizationDescription.Scope))
                        {
                            throw new TokenEndpointProtocolException(accessTokenRequest, Protocol.AccessTokenRequestErrorCodes.InvalidScope, AuthServerStrings.AccessScopeExceedsGrantScope);
                        }
                    }
                }

                // Make sure the authorization this token represents hasn't already been revoked.
                if (!this.AuthorizationServer.IsAuthorizationValid(authCarrier.AuthorizationDescription))
                {
                    throw new TokenEndpointProtocolException(accessTokenRequest, Protocol.AccessTokenRequestErrorCodes.InvalidGrant);
                }

                applied = true;
            }

            return applied ? (MessageProtections?)MessageProtections.None : null;
        }
    }
}
