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

namespace Nequeo.Net.OAuth2.Framework.Utility
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
    using Nequeo.Net.Core.Messaging;

	/// <summary>
	/// Represents the result of OAuth or OpenID authentication.
	/// </summary>
	public class AuthenticationResult {
		/// <summary>
		/// Returns an instance which indicates failed authentication.
		/// </summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
			Justification = "This type is immutable.")]
		public static readonly AuthenticationResult Failed = new AuthenticationResult(isSuccessful: false);

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticationResult"/> class.
		/// </summary>
		/// <param name="isSuccessful">
		/// if set to <c>true</c> [is successful]. 
		/// </param>
		public AuthenticationResult(bool isSuccessful)
			: this(isSuccessful, provider: null, providerUserId: null, userName: null, extraData: null) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticationResult"/> class.
		/// </summary>
		/// <param name="exception">
		/// The exception. 
		/// </param>
		public AuthenticationResult(Exception exception)
			: this(isSuccessful: false) {
			if (exception == null) {
				throw new ArgumentNullException("exception");
			}

			this.Error = exception;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticationResult"/> class.
		/// </summary>
		/// <param name="isSuccessful">
		/// if set to <c>true</c> [is successful]. 
		/// </param>
		/// <param name="provider">
		/// The provider. 
		/// </param>
		/// <param name="providerUserId">
		/// The provider user id. 
		/// </param>
		/// <param name="userName">
		/// Name of the user. 
		/// </param>
		/// <param name="extraData">
		/// The extra data. 
		/// </param>
		public AuthenticationResult(
			bool isSuccessful, string provider, string providerUserId, string userName, IDictionary<string, string> extraData) {
			this.IsSuccessful = isSuccessful;
			this.Provider = provider;
			this.ProviderUserId = providerUserId;
			this.UserName = userName;
			if (extraData != null) {
				// wrap extraData in a read-only dictionary
				this.ExtraData = new ReadOnlyDictionary<string, string>(extraData);
			}
		}

		/// <summary>
		/// Gets the error that may have occured during the authentication process
		/// </summary>
		public Exception Error { get; private set; }

		/// <summary>
		/// Gets the optional extra data that may be returned from the provider
		/// </summary>
		public IDictionary<string, string> ExtraData { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the authentication step is successful.
		/// </summary>
		/// <value> <c>true</c> if authentication is successful; otherwise, <c>false</c> . </value>
		public bool IsSuccessful { get; private set; }

		/// <summary>
		/// Gets the provider's name.
		/// </summary>
		public string Provider { get; private set; }

		/// <summary>
		/// Gets the user id that is returned from the provider.  It is unique only within the Provider's namespace.
		/// </summary>
		public string ProviderUserId { get; private set; }

		/// <summary>
		/// Gets an (insecure, non-unique) alias for the user that the user should recognize as himself/herself.
		/// </summary>
		/// <value>This may take the form of an email address, a URL, or any other value that the user may recognize.</value>
		/// <remarks>
		/// This alias may come from the Provider or may be derived by the relying party if the Provider does not supply one.
		/// It is not guaranteed to be unique and certainly does not merit any trust in any suggested authenticity.
		/// </remarks>
		public string UserName { get; private set; }
	}
}
