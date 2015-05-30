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

namespace Nequeo.Net.OAuth2.Framework.ChannelElements
{
	using System;
	using System.Collections.Generic;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Bindings;
    using Nequeo.Net.Core.Messaging.Reflection;

    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Framework.ChannelElements;

	/// <summary>
	/// A data bag that stores authorization data.
	/// </summary>
	public abstract class AuthorizationDataBag : DataBag, IAuthorizationDescription {
		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorizationDataBag"/> class.
		/// </summary>
		protected AuthorizationDataBag() {
			this.Scope = new HashSet<string>(OAuthUtilities.ScopeStringComparer);
		}

		/// <summary>
		/// Gets or sets the identifier of the client authorized to access protected data.
		/// </summary>
		[MessagePart]
		public string ClientIdentifier { get; set; }

		/// <summary>
		/// Gets the date this authorization was established or the token was issued.
		/// </summary>
		/// <value>A date/time expressed in UTC.</value>
		public DateTime UtcIssued {
			get { return this.UtcCreationDate; }
		}

		/// <summary>
		/// Gets or sets the name on the account whose data on the resource server is accessible using this authorization.
		/// </summary>
		[MessagePart]
        public string UserDataAndNonce { get; set; }

		/// <summary>
		/// Gets the scope of operations the client is allowed to invoke.
		/// </summary>
		[MessagePart(Encoder = typeof(ScopeEncoder))]
		public HashSet<string> Scope { get; private set; }
	}
}
