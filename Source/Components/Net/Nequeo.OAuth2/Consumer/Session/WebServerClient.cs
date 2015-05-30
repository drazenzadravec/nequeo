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
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Consumer.Session.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

    /// <summary>
    /// An OAuth 2.0 consumer designed for web applications.
    /// </summary>
    public class WebServerClient : ClientBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebServerClient"/> class.
        /// </summary>
        /// <param name="authorizationServer">The authorization server.</param>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        public WebServerClient(AuthorizationServerDescription authorizationServer, string clientIdentifier = null, string clientSecret = null)
            : this(authorizationServer, clientIdentifier, DefaultSecretApplicator(clientSecret))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServerClient"/> class.
        /// </summary>
        /// <param name="authorizationServer">The authorization server.</param>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="clientCredentialApplicator">
        /// The tool to use to apply client credentials to authenticated requests to the Authorization Server.
        /// May be <c>null</c> for clients with no secret or other means of authentication.
        /// </param>
        public WebServerClient(AuthorizationServerDescription authorizationServer, string clientIdentifier, ClientCredentialApplicator clientCredentialApplicator)
            : base(authorizationServer, clientIdentifier, clientCredentialApplicator)
        {
        }

        /// <summary>
        /// Gets or sets an optional component that gives you greater control to record and influence the authorization process.
        /// </summary>
        /// <value>The authorization tracker.</value>
        public IClientAuthorizationTracker AuthorizationTracker { get; set; }

        /// <summary>
        /// Prepares a request for user authorization from an authorization server.
        /// </summary>
        /// <param name="scope">The scope of authorized access requested.</param>
        /// <param name="returnTo">The URL the authorization server should redirect the browser (typically on this site) to when the authorization is completed.  If null, the current request's URL will be used.</param>
        public void RequestUserAuthorization(IEnumerable<string> scope = null, Uri returnTo = null)
        {
            var authorizationState = new AuthorizationState(scope)
            {
                Callback = returnTo,
            };
            this.PrepareRequestUserAuthorization(authorizationState).Send();
        }

        /// <summary>
        /// Prepares a request for user authorization from an authorization server.
        /// </summary>
        /// <param name="scopes">The scope of authorized access requested.</param>
        /// <param name="returnTo">The URL the authorization server should redirect the browser (typically on this site) to when the authorization is completed.  If null, the current request's URL will be used.</param>
        /// <returns>The authorization request.</returns>
        public OutgoingWebResponse PrepareRequestUserAuthorization(IEnumerable<string> scopes = null, Uri returnTo = null)
        {
            var authorizationState = new AuthorizationState(scopes)
            {
                Callback = returnTo,
            };
            return this.PrepareRequestUserAuthorization(authorizationState);
        }

        /// <summary>
        /// Prepares a request for user authorization from an authorization server.
        /// </summary>
        /// <param name="authorization">The authorization state to associate with this particular request.</param>
        /// <returns>The authorization request.</returns>
        public OutgoingWebResponse PrepareRequestUserAuthorization(IAuthorizationState authorization)
        {
            if (authorization.Callback == null)
            {
                authorization.Callback = this.Channel.GetRequestFromContext().GetPublicFacingUrl()
                    .StripMessagePartsFromQueryString(this.Channel.MessageDescriptions.Get(typeof(EndUserAuthorizationSuccessResponseBase), Protocol.Default.Version))
                    .StripMessagePartsFromQueryString(this.Channel.MessageDescriptions.Get(typeof(EndUserAuthorizationFailedResponse), Protocol.Default.Version));
                authorization.SaveChanges();
            }

            var request = new EndUserAuthorizationRequestC(this.AuthorizationServer)
            {
                ClientIdentifier = this.ClientIdentifier,
                Callback = authorization.Callback,
            };
            request.Scope.ResetContents(authorization.Scope);

            // Mitigate XSRF attacks by including a state value that would be unpredictable between users, but
            // verifiable for the same user/session.
            // If the host is implementing the authorization tracker though, they're handling this protection themselves.
            if (this.AuthorizationTracker == null)
            {
                var context = this.Channel.GetHttpContext();
                if (context.Session != null)
                {
                    request.ClientState = context.Session.SessionID;
                }
                else
                {
                }
            }

            return this.Channel.PrepareResponse(request);
        }

        /// <summary>
        /// Processes the authorization response from an authorization server, if available.
        /// </summary>
        /// <param name="request">The incoming HTTP request that may carry an authorization response.</param>
        /// <returns>The authorization state that contains the details of the authorization.</returns>
        public IAuthorizationState ProcessUserAuthorization(HttpRequestBase request = null)
        {
            if (request == null)
            {
                request = this.Channel.GetRequestFromContext();
            }

            IMessageWithClientState response;
            if (this.Channel.TryReadFromRequest<IMessageWithClientState>(request, out response))
            {
                Uri callback = MessagingUtilities.StripMessagePartsFromQueryString(request.GetPublicFacingUrl(), this.Channel.MessageDescriptions.Get(response));
                IAuthorizationState authorizationState;
                if (this.AuthorizationTracker != null)
                {
                    authorizationState = this.AuthorizationTracker.GetAuthorizationState(callback, response.ClientState);
                    ErrorUtilities.VerifyProtocol(authorizationState != null, ClientStrings.AuthorizationResponseUnexpectedMismatch);
                }
                else
                {
                    var context = this.Channel.GetHttpContext();
                    if (context.Session != null)
                    {
                        ErrorUtilities.VerifyProtocol(string.Equals(response.ClientState, context.Session.SessionID, StringComparison.Ordinal), ClientStrings.AuthorizationResponseUnexpectedMismatch);
                    }
                    else
                    {
                    }

                    authorizationState = new AuthorizationState { Callback = callback };
                }
                var success = response as EndUserAuthorizationSuccessAuthCodeResponse;
                var failure = response as EndUserAuthorizationFailedResponse;
                ErrorUtilities.VerifyProtocol(success != null || failure != null, MessagingStrings.UnexpectedMessageReceivedOfMany);
                if (success != null)
                {
                    this.UpdateAuthorizationWithResponse(authorizationState, success);
                }
                else
                { // failure
                    authorizationState.Delete();
                }

                return authorizationState;
            }

            return null;
        }
    }
}
