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
	using System.Linq;
	using System.Text;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Consumer.Session.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

	/// <summary>
	/// A description of an OAuth Authorization Server as seen by an OAuth Client.
	/// </summary>
	public class AuthorizationServerDescription {
		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorizationServerDescription"/> class.
		/// </summary>
		public AuthorizationServerDescription() {
			this.ProtocolVersion = Protocol.Default.ProtocolVersion;
		}

		/// <summary>
		/// Gets or sets the Authorization Server URL from which an Access Token is requested by the Client.
		/// </summary>
		/// <value>An HTTPS URL.</value>
		/// <remarks>
		/// <para>After obtaining authorization from the resource owner, clients request an access token from the authorization server's token endpoint.</para>
		/// <para>The URI of the token endpoint can be found in the service documentation, or can be obtained by the client by making an unauthorized protected resource request (from the WWW-Authenticate response header token-uri (The 'authorization-uri' Attribute) attribute).</para>
		/// <para>The token endpoint advertised by the resource server MAY include a query component as defined by [RFC3986] (Berners-Lee, T., Fielding, R., and L. Masinter, “Uniform Resource Identifier (URI): Generic Syntax,” January 2005.) section 3.</para>
		/// <para>Since requests to the token endpoint result in the transmission of plain text credentials in the HTTP request and response, the authorization server MUST require the use of a transport-layer mechanism such as TLS/SSL (or a secure channel with equivalent protections) when sending requests to the token endpoints. </para>
		/// </remarks>
		public Uri TokenEndpoint { get; set; }

		/// <summary>
		/// Gets or sets the Authorization Server URL where the Client (re)directs the User
		/// to make an authorization request.
		/// </summary>
		/// <value>An HTTPS URL.</value>
		/// <remarks>
		/// <para>Clients direct the resource owner to the authorization endpoint to approve their access request. Before granting access, the resource owner first authenticates with the authorization server. The way in which the authorization server authenticates the end-user (e.g. username and password login, OpenID, session cookies) and in which the authorization server obtains the end-user's authorization, including whether it uses a secure channel such as TLS/SSL, is beyond the scope of this specification. However, the authorization server MUST first verify the identity of the end-user.</para>
		/// <para>The URI of the authorization endpoint can be found in the service documentation, or can be obtained by the client by making an unauthorized protected resource request (from the WWW-Authenticate response header auth-uri (The 'authorization-uri' Attribute) attribute).</para>
		/// <para>The authorization endpoint advertised by the resource server MAY include a query component as defined by [RFC3986] (Berners-Lee, T., Fielding, R., and L. Masinter, “Uniform Resource Identifier (URI): Generic Syntax,” January 2005.) section 3.</para>
		/// <para>Since requests to the authorization endpoint result in user authentication and the transmission of sensitive values, the authorization server SHOULD require the use of a transport-layer mechanism such as TLS/SSL (or a secure channel with equivalent protections) when sending requests to the authorization endpoints.</para>
		/// </remarks>
		public Uri AuthorizationEndpoint { get; set; }

		/// <summary>
		/// Gets or sets the OAuth version supported by the Authorization Server.
		/// </summary>
		public ProtocolVersion ProtocolVersion { get; set; }

		/// <summary>
		/// Gets the version of the OAuth protocol to use with this Authorization Server.
		/// </summary>
		/// <value>The version.</value>
		internal Version Version {
			get { return Protocol.Lookup(this.ProtocolVersion).Version; }
		}
	}
}
