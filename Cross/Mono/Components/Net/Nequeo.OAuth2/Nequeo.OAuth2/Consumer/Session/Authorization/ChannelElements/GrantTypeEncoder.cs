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

namespace Nequeo.Net.OAuth2.Consumer.Session.Authorization.ChannelElements
{
	using System;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

	/// <summary>
	/// Encodes/decodes the OAuth 2.0 grant_type argument.
	/// </summary>
	internal class GrantTypeEncoder : IMessagePartEncoder {
		/// <summary>
		/// Initializes a new instance of the <see cref="GrantTypeEncoder"/> class.
		/// </summary>
		public GrantTypeEncoder() {
		}

		/// <summary>
		/// Encodes the specified value.
		/// </summary>
		/// <param name="value">The value.  Guaranteed to never be null.</param>
		/// <returns>
		/// The <paramref name="value"/> in string form, ready for message transport.
		/// </returns>
		public string Encode(object value) {
			var responseType = (GrantType)value;
			switch (responseType)
			{
				case GrantType.ClientCredentials:
					return Protocol.GrantTypes.ClientCredentials;
				case GrantType.AuthorizationCode:
					return Protocol.GrantTypes.AuthorizationCode;
					case GrantType.RefreshToken:
					return Protocol.GrantTypes.RefreshToken;
				case GrantType.Password:
					return Protocol.GrantTypes.Password;
				case GrantType.Assertion:
					return Protocol.GrantTypes.Assertion;
				default:
					throw ErrorUtilities.ThrowFormat(MessagingStrings.UnexpectedMessagePartValue, Protocol.grant_type, value);
			}
		}

		/// <summary>
		/// Decodes the specified value.
		/// </summary>
		/// <param name="value">The string value carried by the transport.  Guaranteed to never be null, although it may be empty.</param>
		/// <returns>
		/// The deserialized form of the given string.
		/// </returns>
		/// <exception cref="FormatException">Thrown when the string value given cannot be decoded into the required object type.</exception>
		public object Decode(string value) {
			switch (value) {
				case Protocol.GrantTypes.ClientCredentials:
					return GrantType.ClientCredentials;
				case Protocol.GrantTypes.Assertion:
					return GrantType.Assertion;
				case Protocol.GrantTypes.Password:
					return GrantType.Password;
				case Protocol.GrantTypes.RefreshToken:
					return GrantType.RefreshToken;
				case Protocol.GrantTypes.AuthorizationCode:
					return GrantType.AuthorizationCode;
				default:
					throw ErrorUtilities.ThrowFormat(MessagingStrings.UnexpectedMessagePartValue, Protocol.grant_type, value);
			}
		}
	}
}
