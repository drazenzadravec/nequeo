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
	using System.Text;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.ChannelElements;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.ChannelElements;

	/// <summary>
	/// A message sent by a web application Client to the AuthorizationServer
	/// via the user agent to obtain authorization from the user and prepare
	/// to issue an access token to the client if permission is granted.
	/// </summary>
	[Serializable]
	public class EndUserAuthorizationImplicitRequest : EndUserAuthorizationRequest, IAccessTokenRequestInternal {
		/// <summary>
		/// Gets or sets the grant type that the client expects of the authorization server.
		/// </summary>
		/// <value>Always <see cref="EndUserAuthorizationResponseType.AccessToken"/>.  Other response types are not supported.</value>
		[MessagePart(Protocol.response_type, IsRequired = true, Encoder = typeof(EndUserAuthorizationResponseTypeEncoder))]
		private const EndUserAuthorizationResponseType ResponseTypeConst = EndUserAuthorizationResponseType.AccessToken;

		/// <summary>
		/// Initializes a new instance of the <see cref="EndUserAuthorizationImplicitRequest"/> class.
		/// </summary>
		/// <param name="authorizationEndpoint">The Authorization Server's user authorization URL to direct the user to.</param>
		/// <param name="version">The protocol version.</param>
		protected EndUserAuthorizationImplicitRequest(Uri authorizationEndpoint, Version version)
			: base(authorizationEndpoint, version) {
		}

		/// <summary>
		/// Gets the grant type that the client expects of the authorization server.
		/// </summary>
		public override EndUserAuthorizationResponseType ResponseType {
			get { return ResponseTypeConst; }
		}

		/// <summary>
		/// Gets or sets the result of calling the authorization server host's access token creation method.
		/// </summary>
		AccessTokenResult IAccessTokenRequestInternal.AccessTokenResult { get; set; }

		/// <summary>
		/// Gets a value indicating whether the client requesting the access token has authenticated itself.
		/// </summary>
		/// <value>
		/// Always false because authorization requests only include the client_id, without a secret.
		/// </value>
		bool IAccessTokenRequest.ClientAuthenticated {
			get { return false; }
		}
	}
}
