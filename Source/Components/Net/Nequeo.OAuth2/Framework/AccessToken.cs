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

namespace Nequeo.Net.OAuth2.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Security.Cryptography;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Bindings;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Framework.ChannelElements;
    using Nequeo.Net.OAuth2.Storage;
    using Nequeo.Net.OAuth2.Storage.Basic;

	/// <summary>
	/// A short-lived token that accompanies HTTP requests to protected data to authorize the request.
	/// </summary>
	public class AccessToken : AuthorizationDataBag {
		/// <summary>
		/// Initializes a new instance of the <see cref="AccessToken"/> class.
		/// </summary>
		public AccessToken() {
		}

		/// <summary>
		/// Gets or sets the lifetime of the access token.
		/// </summary>
		/// <value>The lifetime.</value>
		[MessagePart(Encoder = typeof(TimespanSecondsEncoder))]
		public TimeSpan? Lifetime { get; set; }

		/// <summary>
		/// Gets the type of this instance.
		/// </summary>
		/// <value>The type of the bag.</value>
		/// <remarks>
		/// This ensures that one token cannot be misused as another kind of token.
		/// </remarks>
		protected override Type BagType {
			get {
				// different roles (authorization server vs. Client) may derive from AccessToken, but they are all interoperable.
				return typeof(AccessToken);
			}
		}

		/// <summary>
		/// Creates a formatter capable of serializing/deserializing an access token.
		/// </summary>
		/// <param name="signingKey">The crypto service provider with the authorization server's private key used to asymmetrically sign the access token.</param>
		/// <param name="encryptingKey">The crypto service provider with the resource server's public key used to encrypt the access token.</param>
		/// <returns>An access token serializer.</returns>
		internal static IDataBagFormatter<AccessToken> CreateFormatter(RSACryptoServiceProvider signingKey, RSACryptoServiceProvider encryptingKey) {
			Contract.Requires(signingKey != null || !signingKey.PublicOnly);
			Contract.Requires(encryptingKey != null);
			Contract.Ensures(Contract.Result<IDataBagFormatter<AccessToken>>() != null);

			return new UriStyleMessageFormatter<AccessToken>(signingKey, encryptingKey);
		}

		/// <summary>
		/// Initializes this instance of the <see cref="AccessToken"/> class.
		/// </summary>
		/// <param name="authorization">The authorization to apply to this access token.</param>
		internal void ApplyAuthorization(IAuthorizationDescription authorization) {

			this.ClientIdentifier = authorization.ClientIdentifier;
			this.UtcCreationDate = authorization.UtcIssued;
            this.UserDataAndNonce = authorization.UserDataAndNonce;
			this.Scope.ResetContents(authorization.Scope);
		}

		/// <summary>
		/// Initializes this instance of the <see cref="AccessToken"/> class.
		/// </summary>
		/// <param name="scopes">The scopes.</param>
		/// <param name="username">The username of the account that authorized this token.</param>
		/// <param name="lifetime">The lifetime for this access token.</param>
		/// <remarks>
		/// The <see cref="AuthorizationDataBag.ClientIdentifier"/> is left <c>null</c> in this case because this constructor
		/// is invoked in the case where the client is <em>not</em> authenticated, and therefore no
		/// trust in the client_id is appropriate.
		/// </remarks>
		internal void ApplyAuthorization(IEnumerable<string> scopes, string username, TimeSpan? lifetime) {
			this.Scope.ResetContents(scopes);
            this.UserDataAndNonce = username;
			this.Lifetime = lifetime;
			this.UtcCreationDate = DateTime.UtcNow;
		}

		/// <summary>
		/// Serializes this instance to a simple string for transmission to the client.
		/// </summary>
		/// <returns>A non-empty string.</returns>
		protected internal virtual string Serialize() {
			Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
			throw new NotSupportedException();
		}

		/// <summary>
		/// Checks the message state for conformity to the protocol specification
		/// and throws an exception if the message is invalid.
		/// </summary>
		/// <remarks>
		/// 	<para>Some messages have required fields, or combinations of fields that must relate to each other
		/// in specialized ways.  After deserializing a message, this method checks the state of the
		/// message to see if it conforms to the protocol.</para>
		/// 	<para>Note that this property should <i>not</i> check signatures or perform any state checks
		/// outside this scope of this particular message.</para>
		/// </remarks>
		/// <exception cref="ProtocolException">Thrown if the message is invalid.</exception>
		protected override void EnsureValidMessage() {
			base.EnsureValidMessage();

			// Has this token expired?
			if (this.Lifetime.HasValue) {
				DateTime expirationDate = this.UtcCreationDate + this.Lifetime.Value;
				if (expirationDate < DateTime.UtcNow) {
					throw new ExpiredMessageException(expirationDate, this.ContainingMessage);
				}
			}
		}
	}
}
