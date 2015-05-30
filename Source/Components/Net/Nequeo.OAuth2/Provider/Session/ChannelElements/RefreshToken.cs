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
    using System.Diagnostics.Contracts;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Bindings;
    using Nequeo.Net.Core.Messaging.Reflection;

    using Nequeo.Net.OAuth2.Storage.Basic;
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
    /// The refresh token issued to a client by an authorization server that allows the client
    /// to periodically obtain new short-lived access tokens.
    /// </summary>
    internal class RefreshToken : AuthorizationDataBag
    {
        /// <summary>
        /// The name of the bucket for symmetric keys used to sign refresh tokens.
        /// </summary>
        internal const string RefreshTokenKeyBucket = "https://localhost/dnoa/oauth_refresh_token";

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshToken"/> class.
        /// </summary>
        public RefreshToken()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshToken"/> class.
        /// </summary>
        /// <param name="authorization">The authorization this refresh token should describe.</param>
        internal RefreshToken(IAuthorizationDescription authorization)
        {
            this.ClientIdentifier = authorization.ClientIdentifier;
            this.UtcCreationDate = authorization.UtcIssued;
            this.UserDataAndNonce = authorization.UserDataAndNonce;
            this.Scope.ResetContents(authorization.Scope);
        }

        /// <summary>
        /// Creates a formatter capable of serializing/deserializing a refresh token.
        /// </summary>
        /// <param name="cryptoKeyStore">The crypto key store.</param>
        /// <returns>
        /// A DataBag formatter.  Never null.
        /// </returns>
        internal static IDataBagFormatter<RefreshToken> CreateFormatter(ICryptoKeyStore cryptoKeyStore)
        {
            Contract.Ensures(Contract.Result<IDataBagFormatter<RefreshToken>>() != null);
            return new UriStyleMessageFormatter<RefreshToken>(cryptoKeyStore, RefreshTokenKeyBucket, signed: true, encrypted: true);
        }
    }
}
