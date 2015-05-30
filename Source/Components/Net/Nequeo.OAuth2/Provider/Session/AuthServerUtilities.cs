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

namespace Nequeo.Net.OAuth2.Provider.Session
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
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.ChannelElements;
    using Nequeo.Net.OAuth2.Provider.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Provider.Session.Messages;

    /// <summary>
    /// Utility methods for authorization servers.
    /// </summary>
    internal static class AuthServerUtilities
    {
        /// <summary>
        /// Gets information about the client with a given identifier.
        /// </summary>
        /// <param name="authorizationServer">The authorization server.</param>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <returns>The client information.  Never null.</returns>
        internal static IClientDescription GetClientOrThrow(this IAuthorizationServerHost authorizationServer, string clientIdentifier)
        {
            Contract.Ensures(Contract.Result<IClientDescription>() != null);

            try
            {
                var result = authorizationServer.GetClient(clientIdentifier);
                ErrorUtilities.VerifyHost(result != null, OAuthStrings.ResultShouldNotBeNull, authorizationServer.GetType().FullName, "GetClient(string)");
                return result;
            }
            catch (KeyNotFoundException ex)
            {
                throw ErrorUtilities.Wrap(ex, AuthServerStrings.ClientOrTokenSecretNotFound);
            }
            catch (ArgumentException ex)
            {
                throw ErrorUtilities.Wrap(ex, AuthServerStrings.ClientOrTokenSecretNotFound);
            }
        }

        /// <summary>
        /// Verifies a condition is true or throws an exception describing the problem.
        /// </summary>
        /// <param name="condition">The condition that evaluates to true to avoid an exception.</param>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="error">A single error code from <see cref="Protocol.AccessTokenRequestErrorCodes"/>.</param>
        /// <param name="authenticationModule">The authentication module from which to glean the WWW-Authenticate header when applicable.</param>
        /// <param name="unformattedDescription">A human-readable UTF-8 encoded text providing additional information, used to assist the client developer in understanding the error that occurred.</param>
        /// <param name="args">The formatting arguments to generate the actual description.</param>
        internal static void TokenEndpointVerify(bool condition, AccessTokenRequestBase requestMessage, string error, ClientAuthenticationModule authenticationModule = null, string unformattedDescription = null, params object[] args)
        {
            if (!condition)
            {
                string description = unformattedDescription != null ? string.Format(CultureInfo.CurrentCulture, unformattedDescription, args) : null;

                string wwwAuthenticateHeader = null;
                if (authenticationModule != null)
                {
                    wwwAuthenticateHeader = authenticationModule.AuthenticateHeader;
                }

                throw new TokenEndpointProtocolException(requestMessage, error, description, authenticateHeader: wwwAuthenticateHeader);
            }
        }
    }
}
