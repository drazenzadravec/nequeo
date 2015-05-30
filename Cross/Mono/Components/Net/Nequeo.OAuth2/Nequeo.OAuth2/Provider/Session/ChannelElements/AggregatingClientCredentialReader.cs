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
    using System.Globalization;
    using System.Linq;
    using System.Text;
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
    using Nequeo.Net.OAuth2.Provider.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Provider.Session.Messages;

    /// <summary>
    /// Applies OAuth 2 spec policy for supporting multiple methods of client authentication.
    /// </summary>
    internal class AggregatingClientCredentialReader : ClientAuthenticationModule
    {
        /// <summary>
        /// The set of authenticators to apply to an incoming request.
        /// </summary>
        private readonly IEnumerable<ClientAuthenticationModule> authenticators;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatingClientCredentialReader"/> class.
        /// </summary>
        /// <param name="authenticators">The set of authentication modules to apply.</param>
        internal AggregatingClientCredentialReader(IEnumerable<ClientAuthenticationModule> authenticators)
        {
            this.authenticators = authenticators;
        }

        /// <summary>
        /// Gets this module's contribution to an HTTP 401 WWW-Authenticate header so the client knows what kind of authentication this module supports.
        /// </summary>
        public override string AuthenticateHeader
        {
            get
            {
                var builder = new StringBuilder();
                foreach (var authenticator in this.authenticators)
                {
                    string scheme = authenticator.AuthenticateHeader;
                    if (scheme != null)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(", ");
                        }

                        builder.Append(scheme);
                    }
                }

                return builder.Length > 0 ? builder.ToString() : null;
            }
        }

        /// <summary>
        /// Attempts to extract client identification/authentication information from a message.
        /// </summary>
        /// <param name="authorizationServerHost">The authorization server host.</param>
        /// <param name="requestMessage">The incoming message.</param>
        /// <param name="clientIdentifier">Receives the client identifier, if one was found.</param>
        /// <returns>The level of the extracted client information.</returns>
        public override ClientAuthenticationResult TryAuthenticateClient(IAuthorizationServerHost authorizationServerHost, AuthenticatedClientRequestBase requestMessage, out string clientIdentifier)
        {
            ClientAuthenticationModule authenticator = null;
            ClientAuthenticationResult result = ClientAuthenticationResult.NoAuthenticationRecognized;
            clientIdentifier = null;

            foreach (var candidateAuthenticator in this.authenticators)
            {
                string candidateClientIdentifier;
                var resultCandidate = candidateAuthenticator.TryAuthenticateClient(authorizationServerHost, requestMessage, out candidateClientIdentifier);

                ErrorUtilities.VerifyProtocol(
                    result == ClientAuthenticationResult.NoAuthenticationRecognized || resultCandidate == ClientAuthenticationResult.NoAuthenticationRecognized,
                    "Message rejected because multiple forms of client authentication ({0} and {1}) were detected, which is forbidden by the OAuth 2 Protocol Framework specification.",
                    authenticator,
                    candidateAuthenticator);

                if (resultCandidate != ClientAuthenticationResult.NoAuthenticationRecognized)
                {
                    authenticator = candidateAuthenticator;
                    result = resultCandidate;
                    clientIdentifier = candidateClientIdentifier;
                }
            }

            return result;
        }
    }
}
