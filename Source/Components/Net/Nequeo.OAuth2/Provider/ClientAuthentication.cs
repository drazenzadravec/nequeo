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
using System.Linq;
using System.Text;

using Nequeo.Net.OAuth2.Framework;
using Nequeo.Net.OAuth2.Storage;
using Nequeo.Net.OAuth2.Storage.Basic;
using Nequeo.Net.OAuth2.Framework.Utility;
using Nequeo.Net.OAuth2.Provider.Session;
using Nequeo.Net.OAuth2.Provider;
using Nequeo.Net.OAuth2.Provider.Session.ChannelElements;

using Nequeo.Cryptography.Parser;
using Nequeo.Cryptography.Signing;
using Nequeo.Cryptography;
using Nequeo.Security;

namespace Nequeo.Net.OAuth2.Provider
{
    /// <summary>
    /// Client authentication handler.
    /// </summary>
    public class ClientAuthentication : ClientAuthenticationModule
    {
        /// <summary>
        /// Client authentication handler.
        /// </summary>
        /// <param name="tokenStore">The token store</param>
        /// <param name="consumerStore">The consumer store</param>
        /// <param name="nonceStore">The nonce store.</param>
        public ClientAuthentication(ITokenStore tokenStore, IConsumerStore consumerStore, INonceStore nonceStore)
        {
            _tokenStore = tokenStore;
            _consumerStore = consumerStore;
            _nonceStore = nonceStore;
        }

        private ITokenStore _tokenStore = null;
        private IConsumerStore _consumerStore = null;
        private INonceStore _nonceStore = null;

        /// <summary>
        /// Try to authenticate the client.
        /// </summary>
        /// <param name="authorizationServerHost">Provides host-specific authorization server services needed by this library.</param>
        /// <param name="requestMessage">A direct message from the client to the authorization server that includes the client's credentials.</param>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <returns>Describes the various levels at which client information may be extracted from an inbound message.</returns>
        public override Framework.ChannelElements.ClientAuthenticationResult TryAuthenticateClient(
            Session.IAuthorizationServerHost authorizationServerHost, 
            Consumer.Session.Authorization.Messages.AuthenticatedClientRequestBase requestMessage, 
            out string clientIdentifier)
        {
            // Set the initial client identifier to null.
            clientIdentifier = null;
           
            // If a client identifier exists.
            if (!string.IsNullOrEmpty(requestMessage.ClientIdentifier))
            {
                // Get the client decription. IF client not found
                // then return un-authenticated.
                var client = authorizationServerHost.GetClient(requestMessage.ClientIdentifier);
                if (client != null)
                {
                    // Get the client secret. If no secret foune then
                    // return un-authenticated.
                    string consumerSecret = _consumerStore.GetConsumerSecret(requestMessage.ClientIdentifier);
                    if (!string.IsNullOrEmpty(consumerSecret))
                    {
                        // If the client secret is valid.
                        if (client.IsValidClientSecret(consumerSecret))
                        {
                            // Set the client identifier and
                            // authenticate the client.
                            clientIdentifier = requestMessage.ClientIdentifier;
                            return Framework.ChannelElements.ClientAuthenticationResult.ClientAuthenticated;
                        }
                        else
                        { 
                            // Invalid client secret
                            return Framework.ChannelElements.ClientAuthenticationResult.ClientAuthenticationRejected;
                        }
                    }
                    else
                    { 
                        // No client secret provided
                        return Framework.ChannelElements.ClientAuthenticationResult.ClientIdNotAuthenticated;
                    }
                }
                else
                { 
                    // The client identifier is not recognized.
                    return Framework.ChannelElements.ClientAuthenticationResult.ClientAuthenticationRejected;
                }
            }
            else
            { 
                // No client id provided.
                return Framework.ChannelElements.ClientAuthenticationResult.NoAuthenticationRecognized;
            }
        }
    }
}
