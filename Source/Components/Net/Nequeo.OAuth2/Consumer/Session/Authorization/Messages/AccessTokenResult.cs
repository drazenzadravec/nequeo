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

namespace Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.ChannelElements;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.ChannelElements;

	/// <summary>
	/// Describes the parameters to be fed into creating a response to an access token request.
	/// </summary>
	public class AccessTokenResult {
		/// <summary>
		/// Initializes a new instance of the <see cref="AccessTokenResult"/> class.
		/// </summary>
		/// <param name="accessToken">The access token to include in this result.</param>
		public AccessTokenResult(AccessToken accessToken) {
			
			this.AllowRefreshToken = true;
			this.AccessToken = accessToken;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to provide the client with a refresh token, when applicable.
		/// </summary>
		/// <value>The default value is <c>true</c>.</value>
		/// <remarks>>
		/// The refresh token will never be provided when this value is false.
		/// The refresh token <em>may</em> be provided when this value is true.
		/// </remarks>
		public bool AllowRefreshToken { get; set; }

		/// <summary>
		/// Gets the access token.
		/// </summary>
		public AccessToken AccessToken { get; private set; }

	}
}
