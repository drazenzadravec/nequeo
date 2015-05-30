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
    using System.Net.Mime;
    using System.Web;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Framework.ChannelElements;
    using Nequeo.Net.OAuth2.Consumer.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Consumer.Session.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.ChannelElements;
    using Nequeo.Net.OAuth2.Provider.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Provider.Session.Messages;

    /// <summary>
    /// The channel for the OAuth protocol.
    /// </summary>
    internal class OAuth2AuthorizationServerChannel : OAuth2ChannelBase, IOAuth2ChannelWithAuthorizationServer
    {
        /// <summary>
        /// The messages receivable by this channel.
        /// </summary>
        private static readonly Type[] MessageTypes = new Type[] {
			typeof(AccessTokenRefreshRequestAS),
			typeof(AccessTokenAuthorizationCodeRequestAS),
			typeof(AccessTokenResourceOwnerPasswordCredentialsRequest),
			typeof(AccessTokenClientCredentialsRequest),
			typeof(EndUserAuthorizationRequest),
			typeof(EndUserAuthorizationImplicitRequest),
			typeof(EndUserAuthorizationFailedResponse),
		};

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2AuthorizationServerChannel"/> class.
        /// </summary>
        /// <param name="authorizationServer">The authorization server.</param>
        /// <param name="clientAuthenticationModule">The aggregating client authentication module.</param>
        protected internal OAuth2AuthorizationServerChannel(IAuthorizationServerHost authorizationServer, ClientAuthenticationModule clientAuthenticationModule)
            : base(MessageTypes, InitializeBindingElements(authorizationServer, clientAuthenticationModule))
        {
            this.AuthorizationServer = authorizationServer;
        }

        /// <summary>
        /// Gets the authorization server.
        /// </summary>
        /// <value>The authorization server.</value>
        public IAuthorizationServerHost AuthorizationServer { get; private set; }

        /// <summary>
        /// Gets or sets the service that checks whether a granted set of scopes satisfies a required set of scopes.
        /// </summary>
        public IScopeSatisfiedCheck ScopeSatisfiedCheck { get; set; }

        /// <summary>
        /// Gets the protocol message that may be in the given HTTP response.
        /// </summary>
        /// <param name="response">The response that is anticipated to contain an protocol message.</param>
        /// <returns>
        /// The deserialized message parts, if found.  Null otherwise.
        /// </returns>
        /// <exception cref="ProtocolException">Thrown when the response is not valid.</exception>
        protected override IDictionary<string, string> ReadFromResponseCore(IncomingWebResponse response)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queues a message for sending in the response stream.
        /// </summary>
        /// <param name="response">The message to send as a response.</param>
        /// <returns>
        /// The pending user agent redirect based message to be sent as an HttpResponse.
        /// </returns>
        /// <remarks>
        /// This method implements spec OAuth V1.0 section 5.3.
        /// </remarks>
        protected override OutgoingWebResponse PrepareDirectResponse(IProtocolMessage response)
        {
            var webResponse = new OutgoingWebResponse();
            ApplyMessageTemplate(response, webResponse);
            string json = this.SerializeAsJson(response);
            webResponse.SetResponse(json, new ContentType(JsonEncoded));
            return webResponse;
        }

        /// <summary>
        /// Gets the protocol message that may be embedded in the given HTTP request.
        /// </summary>
        /// <param name="request">The request to search for an embedded message.</param>
        /// <returns>
        /// The deserialized message, if one is found.  Null otherwise.
        /// </returns>
        protected override IDirectedProtocolMessage ReadFromRequestCore(HttpRequestBase request)
        {
            if (!string.IsNullOrEmpty(request.Url.Fragment))
            {
                var fields = HttpUtility.ParseQueryString(request.Url.Fragment.Substring(1)).ToDictionary();

                MessageReceivingEndpoint recipient;
                try
                {
                    recipient = request.GetRecipient();
                }
                catch (ArgumentException)
                {
                    return null;
                }

                return (IDirectedProtocolMessage)this.Receive(fields, recipient);
            }

            return base.ReadFromRequestCore(request);
        }

        /// <summary>
        /// Initializes the binding elements for the OAuth channel.
        /// </summary>
        /// <param name="authorizationServer">The authorization server.</param>
        /// <param name="clientAuthenticationModule">The aggregating client authentication module.</param>
        /// <returns>
        /// An array of binding elements used to initialize the channel.
        /// </returns>
        private static IChannelBindingElement[] InitializeBindingElements(IAuthorizationServerHost authorizationServer, ClientAuthenticationModule clientAuthenticationModule)
        {
            var bindingElements = new List<IChannelBindingElement>();

            // The order they are provided is used for outgoing messgaes, and reversed for incoming messages.
            bindingElements.Add(new MessageValidationBindingElement(clientAuthenticationModule));
            bindingElements.Add(new TokenCodeSerializationBindingElement());

            return bindingElements.ToArray();
        }
    }
}
