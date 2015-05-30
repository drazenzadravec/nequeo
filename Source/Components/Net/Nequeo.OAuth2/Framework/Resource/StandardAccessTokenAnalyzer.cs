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

namespace Nequeo.Net.OAuth2.Framework.Resource
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Security.Cryptography;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.Resource.ChannelElements;

	/// <summary>
	/// An access token reader that understands DotNetOpenAuth authorization server issued tokens.
	/// </summary>
	public class StandardAccessTokenAnalyzer : IAccessTokenAnalyzer {
		/// <summary>
		/// Initializes a new instance of the <see cref="StandardAccessTokenAnalyzer"/> class.
		/// </summary>
        /// <param name="accessTokenSigningKey">The crypto service provider with the authorization server private signing key.</param>
        /// <param name="resourceServerEncryptionKey">The crypto service provider with the resource server public encryption key.</param>
        public StandardAccessTokenAnalyzer(RSACryptoServiceProvider accessTokenSigningKey, RSACryptoServiceProvider resourceServerEncryptionKey)
        {

            this.AccessTokenSigningKey = accessTokenSigningKey;
            this.ResourceServerEncryptionKey = resourceServerEncryptionKey;
		}

		/// <summary>
		/// Gets the authorization server private signing key.
		/// </summary>
		/// <value>The authorization server private signing key.</value>
        public RSACryptoServiceProvider AccessTokenSigningKey { get; private set; }

		/// <summary>
		/// Gets the resource server public encryption key.
		/// </summary>
		/// <value>The resource server public encryption key.</value>
        public RSACryptoServiceProvider ResourceServerEncryptionKey { get; private set; }

		/// <summary>
		/// Reads an access token to find out what data it authorizes access to.
		/// </summary>
		/// <param name="message">The message carrying the access token.</param>
		/// <param name="accessToken">The access token's serialized representation.</param>
		/// <returns>The deserialized, validated token.</returns>
		/// <exception cref="ProtocolException">Thrown if the access token is expired, invalid, or from an untrusted authorization server.</exception>
		public virtual AccessToken DeserializeAccessToken(IDirectedProtocolMessage message, string accessToken) {
            var accessTokenFormatter = AccessToken.CreateFormatter(this.AccessTokenSigningKey, this.ResourceServerEncryptionKey);
			var token = new AccessToken();
			accessTokenFormatter.Deserialize(token, message, accessToken, Protocol.access_token);
			return token;
		}
	}
}
