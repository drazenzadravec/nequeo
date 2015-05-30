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
    using System.Linq;
    using System.Net;
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
    /// A base class for extensions that apply client authentication to messages for the authorization server in specific ways.
    /// </summary>
    public abstract class ClientCredentialApplicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCredentialApplicator"/> class.
        /// </summary>
        protected ClientCredentialApplicator()
        {
        }

        /// <summary>
        /// Transmits the secret the client shares with the authorization server as a parameter in the POST entity payload.
        /// </summary>
        /// <param name="clientSecret">The secret the client shares with the authorization server.</param>
        /// <returns>The credential applicator to provide to the <see cref="ClientBase"/> instance.</returns>
        public static ClientCredentialApplicator PostParameter(string clientSecret)
        {
            return new PostParameterApplicator(clientSecret);
        }

        /// <summary>
        /// Transmits the client identifier and secret in the HTTP Authorization header via HTTP Basic authentication.
        /// </summary>
        /// <param name="credential">The client id and secret.</param>
        /// <returns>The credential applicator to provide to the <see cref="ClientBase"/> instance.</returns>
        public static ClientCredentialApplicator NetworkCredential(NetworkCredential credential)
        {
            return new NetworkCredentialApplicator(credential);
        }

        /// <summary>
        /// Transmits the client identifier and secret in the HTTP Authorization header via HTTP Basic authentication.
        /// </summary>
        /// <param name="clientSecret">The secret the client shares with the authorization server.</param>
        /// <returns>The credential applicator to provide to the <see cref="ClientBase"/> instance.</returns>
        public static ClientCredentialApplicator NetworkCredential(string clientSecret)
        {
            return new NetworkCredentialApplicator(clientSecret);
        }

        /// <summary>
        /// Never transmits a secret.  Useful for anonymous clients or clients unable to keep a secret.
        /// </summary>
        /// <returns>The credential applicator to provide to the <see cref="ClientBase"/> instance.</returns>
        public static ClientCredentialApplicator NoSecret()
        {
            return null;
        }

        /// <summary>
        /// Applies the client identifier and (when applicable) the client authentication to an outbound message.
        /// </summary>
        /// <param name="clientIdentifier">The identifier by which the authorization server should recognize this client.</param>
        /// <param name="request">The outbound message to apply authentication information to.</param>
        public virtual void ApplyClientCredential(string clientIdentifier, AuthenticatedClientRequestBase request)
        {
        }

        /// <summary>
        /// Applies the client identifier and (when applicable) the client authentication to an outbound message.
        /// </summary>
        /// <param name="clientIdentifier">The identifier by which the authorization server should recognize this client.</param>
        /// <param name="request">The outbound message to apply authentication information to.</param>
        public virtual void ApplyClientCredential(string clientIdentifier, HttpWebRequest request)
        {
        }

        /// <summary>
        /// Authenticates the client via HTTP Basic.
        /// </summary>
        private class NetworkCredentialApplicator : ClientCredentialApplicator
        {
            /// <summary>
            /// The client identifier and secret.
            /// </summary>
            private readonly NetworkCredential credential;

            /// <summary>
            /// The client secret.
            /// </summary>
            private readonly string clientSecret;

            /// <summary>
            /// Initializes a new instance of the <see cref="NetworkCredentialApplicator"/> class.
            /// </summary>
            /// <param name="clientSecret">The client secret.</param>
            internal NetworkCredentialApplicator(string clientSecret)
            {
                this.clientSecret = clientSecret;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="NetworkCredentialApplicator"/> class.
            /// </summary>
            /// <param name="credential">The client credential.</param>
            internal NetworkCredentialApplicator(NetworkCredential credential)
            {
                this.credential = credential;
            }

            /// <summary>
            /// Applies the client identifier and (when applicable) the client authentication to an outbound message.
            /// </summary>
            /// <param name="clientIdentifier">The identifier by which the authorization server should recognize this client.</param>
            /// <param name="request">The outbound message to apply authentication information to.</param>
            public override void ApplyClientCredential(string clientIdentifier, AuthenticatedClientRequestBase request)
            {
                // When using network credentials, the client authentication is not done as standard message parts.
                request.ClientIdentifier = null;
                request.ClientSecret = null;
            }

            /// <summary>
            /// Applies the client identifier and (when applicable) the client authentication to an outbound message.
            /// </summary>
            /// <param name="clientIdentifier">The identifier by which the authorization server should recognize this client.</param>
            /// <param name="request">The outbound message to apply authentication information to.</param>
            public override void ApplyClientCredential(string clientIdentifier, HttpWebRequest request)
            {
                if (clientIdentifier != null)
                {
                    if (this.credential != null && this.credential.UserName == clientIdentifier)
                    {
                        ErrorUtilities.VerifyHost(false, "Client identifiers \"{0}\" and \"{1}\" do not match.", this.credential.UserName, clientIdentifier);
                    }

                    request.Credentials = this.credential ?? new NetworkCredential(clientIdentifier, this.clientSecret);
                }
            }
        }

        /// <summary>
        /// Authenticates the client via a client_secret parameter in the message.
        /// </summary>
        private class PostParameterApplicator : ClientCredentialApplicator
        {
            /// <summary>
            /// The client secret.
            /// </summary>
            private readonly string secret;

            /// <summary>
            /// Initializes a new instance of the <see cref="PostParameterApplicator"/> class.
            /// </summary>
            /// <param name="clientSecret">The client secret.</param>
            internal PostParameterApplicator(string clientSecret)
            {
                this.secret = clientSecret;
            }

            /// <summary>
            /// Applies the client identifier and (when applicable) the client authentication to an outbound message.
            /// </summary>
            /// <param name="clientIdentifier">The identifier by which the authorization server should recognize this client.</param>
            /// <param name="request">The outbound message to apply authentication information to.</param>
            public override void ApplyClientCredential(string clientIdentifier, AuthenticatedClientRequestBase request)
            {
                if (clientIdentifier != null)
                {
                    request.ClientSecret = this.secret;
                }
            }
        }
    }
}
