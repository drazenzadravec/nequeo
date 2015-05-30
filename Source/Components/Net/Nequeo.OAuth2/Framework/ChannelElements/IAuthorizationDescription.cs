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
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Describes a delegated authorization between a resource server, a client, and a user.
	/// </summary>
	[ContractClass(typeof(IAuthorizationDescriptionContract))]
	public interface IAuthorizationDescription {
		/// <summary>
		/// Gets the identifier of the client authorized to access protected data.
		/// </summary>
		string ClientIdentifier { get; }

		/// <summary>
		/// Gets the date this authorization was established or the token was issued.
		/// </summary>
		/// <value>A date/time expressed in UTC.</value>
		DateTime UtcIssued { get; }

		/// <summary>
		/// Gets the user data (username or userID and nonce; e.g 234_hfdhhhh; that is the user data and nonce conbination).
        /// On the account whose data on the resource server is accessible using this authorization.
		/// </summary>
		string UserDataAndNonce { get; }

		/// <summary>
		/// Gets the scope of operations the client is allowed to invoke.
		/// </summary>
		HashSet<string> Scope { get; }
	}

	/// <summary>
	/// Code contract for the <see cref="IAuthorizationDescription"/> interface.
	/// </summary>
	[ContractClassFor(typeof(IAuthorizationDescription))]
	internal abstract class IAuthorizationDescriptionContract : IAuthorizationDescription {
		/// <summary>
		/// Prevents a default instance of the <see cref="IAuthorizationDescriptionContract"/> class from being created.
		/// </summary>
		private IAuthorizationDescriptionContract() {
		}

		/// <summary>
		/// Gets the identifier of the client authorized to access protected data.
		/// </summary>
		string IAuthorizationDescription.ClientIdentifier {
			get {
				Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets the date this authorization was established or the token was issued.
		/// </summary>
		/// <value>A date/time expressed in UTC.</value>
		DateTime IAuthorizationDescription.UtcIssued {
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the name on the account whose data on the resource server is accessible using this authorization, if applicable.
		/// </summary>
		/// <value>A username, or <c>null</c> if the authorization is to access the client's own data (not a distinct resource owner's data).</value>
        string IAuthorizationDescription.UserDataAndNonce
        {
			get {
				// Null and non-empty are allowed, but not empty.
				Contract.Ensures(Contract.Result<string>() != string.Empty);
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets the scope of operations the client is allowed to invoke.
		/// </summary>
		HashSet<string> IAuthorizationDescription.Scope {
			get {
				Contract.Ensures(Contract.Result<HashSet<string>>() != null);
				throw new NotImplementedException();
			}
		}
	}
}
