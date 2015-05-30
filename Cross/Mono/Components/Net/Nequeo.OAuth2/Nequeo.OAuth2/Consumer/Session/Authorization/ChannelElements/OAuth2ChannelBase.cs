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
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

	/// <summary>
	/// The base messaging channel used by OAuth 2.0 parties.
	/// </summary>
	internal abstract class OAuth2ChannelBase : StandardMessageFactoryChannel {
		/// <summary>
		/// The protocol versions supported by this channel.
		/// </summary>
		private static readonly Version[] Versions = Protocol.AllVersions.Select(v => v.Version).ToArray();

		/// <summary>
		/// Initializes a new instance of the <see cref="OAuth2ChannelBase"/> class.
		/// </summary>
		/// <param name="messageTypes">The message types that are received by this channel.</param>
		/// <param name="channelBindingElements">
		/// The binding elements to use in sending and receiving messages.
		/// The order they are provided is used for outgoing messgaes, and reversed for incoming messages.
		/// </param>
		internal OAuth2ChannelBase(Type[] messageTypes, params IChannelBindingElement[] channelBindingElements)
            : base(messageTypes, Versions, channelBindingElements)
        {
		}

		/// <summary>
		/// Allows preprocessing and validation of message data before an appropriate message type is
		/// selected or deserialized.
		/// </summary>
		/// <param name="fields">The received message data.</param>
		protected override void FilterReceivedFields(IDictionary<string, string> fields) {
			base.FilterReceivedFields(fields);

			// Apply the OAuth 2.0 section 2.1 requirement:
			// Parameters sent without a value MUST be treated as if they were omitted from the request.
			// The authorization server SHOULD ignore unrecognized request parameters.
			var emptyKeys = from pair in fields
							where string.IsNullOrEmpty(pair.Value)
							select pair.Key;
			foreach (string emptyKey in emptyKeys.ToList()) {
				fields.Remove(emptyKey);
			}
		}
	}
}
