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
	using System.Net;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.ChannelElements;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.ChannelElements;

	/// <summary>
	/// A direct message from the client to the authorization server that includes the client's credentials.
	/// </summary>
	public abstract class AuthenticatedClientRequestBase : MessageBase, IHttpDirectRequest {
		/// <summary>
		/// The backing for the <see cref="Headers"/> property.
		/// </summary>
		private readonly WebHeaderCollection headers = new WebHeaderCollection();

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticatedClientRequestBase"/> class.
		/// </summary>
		/// <param name="tokenEndpoint">The Authorization Server's access token endpoint URL.</param>
		/// <param name="version">The version.</param>
		protected AuthenticatedClientRequestBase(Uri tokenEndpoint, Version version)
			: base(version, MessageTransport.Direct, tokenEndpoint) {
		}

		/// <summary>
		/// Gets the client identifier previously obtained from the Authorization Server.
		/// </summary>
		/// <value>The client identifier.</value>
		/// <remarks>
		/// Not required, because the client id may be communicate through alternate means like HTTP Basic authentication (the OAuth 2 spec allows a lot of freedom here).
		/// </remarks>
		[MessagePart(Protocol.client_id, IsRequired = false)]
		public string ClientIdentifier { get; internal set; }

		/// <summary>
		/// Gets the client secret.
		/// </summary>
		/// <value>The client secret.</value>
		/// <remarks>
		/// REQUIRED. The client secret as described in Section 2.1  (Client Credentials). OPTIONAL if no client secret was issued. 
		/// </remarks>
		[MessagePart(Protocol.client_secret, IsRequired = false)]
		public string ClientSecret { get; internal set; }

		/// <summary>
		/// Gets the HTTP headers of the request.
		/// </summary>
		/// <value>May be an empty collection, but must not be <c>null</c>.</value>
		public WebHeaderCollection Headers {
			get { return this.headers; }
		}
	}
}