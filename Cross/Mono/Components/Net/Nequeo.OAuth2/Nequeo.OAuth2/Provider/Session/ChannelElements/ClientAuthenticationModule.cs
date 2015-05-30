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
    using System.Linq;
    using System.Text;
    using System.Threading;
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
    /// A base class for extensions that can read incoming messages and extract the client identifier and
    /// possibly authentication information (like a shared secret, signed nonce, etc.)
    /// </summary>
    public abstract class ClientAuthenticationModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAuthenticationModule"/> class.
        /// </summary>
        protected ClientAuthenticationModule()
        {
        }

        /// <summary>
        /// Gets this module's contribution to an HTTP 401 WWW-Authenticate header so the client knows what kind of authentication this module supports.
        /// </summary>
        public virtual string AuthenticateHeader
        {
            get { return null; }
        }

        /// <summary>
        /// Attempts to extract client identification/authentication information from a message.
        /// </summary>
        /// <param name="authorizationServerHost">The authorization server host.</param>
        /// <param name="requestMessage">The incoming message.</param>
        /// <param name="clientIdentifier">Receives the client identifier, if one was found.</param>
        /// <returns>The level of the extracted client information.</returns>
        public abstract ClientAuthenticationResult TryAuthenticateClient(IAuthorizationServerHost authorizationServerHost, AuthenticatedClientRequestBase requestMessage, out string clientIdentifier);

        /// <summary>
        /// Validates a client identifier and shared secret against the authoriation server's database.
        /// </summary>
        /// <param name="authorizationServerHost">The authorization server host; cannot be <c>null</c>.</param>
        /// <param name="clientIdentifier">The alleged client identifier.</param>
        /// <param name="clientSecret">The alleged client secret to be verified.</param>
        /// <returns>An indication as to the outcome of the validation.</returns>
        protected static ClientAuthenticationResult TryAuthenticateClientBySecret(IAuthorizationServerHost authorizationServerHost, string clientIdentifier, string clientSecret)
        {
            if (!string.IsNullOrEmpty(clientIdentifier))
            {
                var client = authorizationServerHost.GetClient(clientIdentifier);
                if (client != null)
                {
                    if (!string.IsNullOrEmpty(clientSecret))
                    {
                        if (client.IsValidClientSecret(clientSecret))
                        {
                            return ClientAuthenticationResult.ClientAuthenticated;
                        }
                        else
                        { // invalid client secret
                            return ClientAuthenticationResult.ClientAuthenticationRejected;
                        }
                    }
                    else
                    { // no client secret provided
                        return ClientAuthenticationResult.ClientIdNotAuthenticated;
                    }
                }
                else
                { // The client identifier is not recognized.
                    return ClientAuthenticationResult.ClientAuthenticationRejected;
                }
            }
            else
            { // no client id provided.
                return ClientAuthenticationResult.NoAuthenticationRecognized;
            }
        }
    }
}
