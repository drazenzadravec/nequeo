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

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Consumer.Session.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

	/// <summary>
	/// A simple in-memory copy of an authorization state.
	/// </summary>
	[Serializable]
	public class AuthorizationState : IAuthorizationState {
		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorizationState"/> class.
		/// </summary>
		/// <param name="scopes">The scopes of access being requested or that was obtained.</param>
		public AuthorizationState(IEnumerable<string> scopes = null) {
			this.Scope = new HashSet<string>(OAuthUtilities.ScopeStringComparer);
			if (scopes != null) {
				this.Scope.AddRange(scopes);
			}
		}

		/// <summary>
		/// Gets or sets the callback URL used to obtain authorization.
		/// </summary>
		/// <value>The callback URL.</value>
		public Uri Callback { get; set; }

		/// <summary>
		/// Gets or sets the long-lived token used to renew the short-lived <see cref="AccessToken"/>.
		/// </summary>
		/// <value>The refresh token.</value>
		public string RefreshToken { get; set; }

		/// <summary>
		/// Gets or sets the access token.
		/// </summary>
		/// <value>The access token.</value>
		public string AccessToken { get; set; }

		/// <summary>
		/// Gets or sets the access token UTC expiration date.
		/// </summary>
		/// <value></value>
		public DateTime? AccessTokenExpirationUtc { get; set; }

		/// <summary>
		/// Gets or sets the access token issue date UTC.
		/// </summary>
		/// <value>The access token issue date UTC.</value>
		public DateTime? AccessTokenIssueDateUtc { get; set; }

		/// <summary>
		/// Gets the scope the token is (to be) authorized for.
		/// </summary>
		/// <value>The scope.</value>
		public HashSet<string> Scope { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is deleted.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is deleted; otherwise, <c>false</c>.
		/// </value>
		public bool IsDeleted { get; set; }

		/// <summary>
		/// Deletes this authorization, including access token and refresh token where applicable.
		/// </summary>
		/// <remarks>
		/// This method is invoked when an authorization attempt fails, is rejected, is revoked, or
		/// expires and cannot be renewed.
		/// </remarks>
		public virtual void Delete() {
			this.IsDeleted = true;
		}

		/// <summary>
		/// Saves any changes made to this authorization object's properties.
		/// </summary>
		/// <remarks>
		/// This method is invoked after DotNetOpenAuth changes any property.
		/// </remarks>
		public virtual void SaveChanges() {
		}
	}
}