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

namespace Nequeo.Net.OAuth2.Framework.Messages
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Framework.ChannelElements;

	/// <summary>
	/// A message that accompanies an HTTP request to a resource server that provides authorization.
	/// </summary>
	/// <remarks>
	/// In its current form, this class only accepts bearer access tokens. 
	/// When support for additional access token types is added, this class should probably be refactored
	/// into derived types, where each derived type supports a particular access token type.
	/// </remarks>
	internal class AccessProtectedResourceRequest : MessageBase, IAccessTokenCarryingRequest {
		/// <summary>
		/// Initializes a new instance of the <see cref="AccessProtectedResourceRequest"/> class.
		/// </summary>
		/// <param name="recipient">The recipient.</param>
		/// <param name="version">The version.</param>
		internal AccessProtectedResourceRequest(Uri recipient, Version version)
			: base(version, MessageTransport.Direct, recipient) {
			this.HttpMethods = HttpDeliveryMethods.HttpVerbMask;
		}

		#region IAccessTokenCarryingRequest Members

		/// <summary>
		/// Gets or sets the access token.
		/// </summary>
		string IAccessTokenCarryingRequest.AccessToken {
			get { return this.AccessToken; }
			set { this.AccessToken = value; }
		}

		/// <summary>
		/// Gets or sets the authorization that the token describes.
		/// </summary>
		AccessToken IAccessTokenCarryingRequest.AuthorizationDescription { get; set; }

		/// <summary>
		/// Gets the authorization that the token describes.
		/// </summary>
		IAuthorizationDescription IAuthorizationCarryingRequest.AuthorizationDescription {
			get { return ((IAccessTokenCarryingRequest)this).AuthorizationDescription; }
		}

		#endregion

		/// <summary>
		/// Gets the type of the access token.
		/// </summary>
		/// <value>
		/// Always "bearer".
		/// </value>
		[MessagePart("token_type", IsRequired = true)]
		internal static string TokenType {
			get { return Protocol.AccessTokenTypes.Bearer; }
		}

		/// <summary>
		/// Gets or sets the access token.
		/// </summary>
		/// <value>The access token.</value>
		[MessagePart("access_token", IsRequired = true)]
		internal string AccessToken { get; set; }
	}
}
