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

namespace Nequeo.Net.OAuth2.Consumer.Session.Authorization
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;

	using Nequeo.Net.Core.Messaging;

	/// <summary>
	/// A description of a client from an Authorization Server's point of view.
	/// </summary>
	[ContractClass(typeof(IClientDescriptionContract))]
	public interface IClientDescription {
		/// <summary>
		/// Gets the callback to use when an individual authorization request
		/// does not include an explicit callback URI.
		/// </summary>
		/// <value>An absolute URL; or <c>null</c> if none is registered.</value>
		Uri DefaultCallback { get; }

		/// <summary>
		/// Gets the type of the client.
		/// </summary>
		ClientType ClientType { get; }

		/// <summary>
		/// Gets a value indicating whether a non-empty secret is registered for this client.
		/// </summary>
		bool HasNonEmptySecret { get; }

		/// <summary>
		/// Determines whether a callback URI included in a client's authorization request 
		/// is among those allowed callbacks for the registered client.
		/// </summary>
		/// <param name="callback">The absolute URI the client has requested the authorization result be received at.  Never null.</param>
		/// <returns>
		/// <c>true</c> if the callback URL is allowable for this client; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// At the point this method is invoked, the identity of the client has <em>not</em>
		/// been confirmed.  To avoid open redirector attacks, the alleged client's identity
		/// is used to lookup a list of allowable callback URLs to make sure that the callback URL
		/// the actual client is requesting is one of the expected ones.
		/// </para>
		/// <para>
		/// From OAuth 2.0 section 2.1: 
		/// The authorization server SHOULD require the client to pre-register
		/// their redirection URI or at least certain components such as the
		/// scheme, host, port and path.  If a redirection URI was registered,
		/// the authorization server MUST compare any redirection URI received at
		/// the authorization endpoint with the registered URI.
		/// </para>
		/// </remarks>
		bool IsCallbackAllowed(Uri callback);

		/// <summary>
		/// Checks whether the specified client secret is correct.
		/// </summary>
		/// <param name="secret">The secret obtained from the client.</param>
		/// <returns><c>true</c> if the secret matches the one in the authorization server's record for the client; <c>false</c> otherwise.</returns>
		/// <remarks>
		/// All string equality checks, whether checking secrets or their hashes,
		/// should be done using <see cref="MessagingUtilities.EqualsConstantTime"/> to mitigate timing attacks.
		/// </remarks>
		bool IsValidClientSecret(string secret);
	}

	/// <summary>
	/// Contract class for the <see cref="IClientDescription"/> interface.
	/// </summary>
	[ContractClassFor(typeof(IClientDescription))]
	internal abstract class IClientDescriptionContract : IClientDescription {
		
		/// <summary>
		/// Gets the type of the client.
		/// </summary>
		ClientType IClientDescription.ClientType {
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the callback to use when an individual authorization request
		/// does not include an explicit callback URI.
		/// </summary>
		/// <value>
		/// An absolute URL; or <c>null</c> if none is registered.
		/// </value>
		Uri IClientDescription.DefaultCallback {
			get {
				Contract.Ensures(Contract.Result<Uri>() == null || Contract.Result<Uri>().IsAbsoluteUri);
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets a value indicating whether a non-empty secret is registered for this client.
		/// </summary>
		bool IClientDescription.HasNonEmptySecret {
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Determines whether a callback URI included in a client's authorization request
		/// is among those allowed callbacks for the registered client.
		/// </summary>
		/// <param name="callback">The requested callback URI.</param>
		/// <returns>
		///   <c>true</c> if the callback is allowed; otherwise, <c>false</c>.
		/// </returns>
		bool IClientDescription.IsCallbackAllowed(Uri callback) {
	
			throw new NotImplementedException();
		}

		/// <summary>
		/// Checks whether the specified client secret is correct.
		/// </summary>
		/// <param name="secret">The secret obtained from the client.</param>
		/// <returns><c>true</c> if the secret matches the one in the authorization server's record for the client; <c>false</c> otherwise.</returns>
		/// <remarks>
		/// All string equality checks, whether checking secrets or their hashes,
		/// should be done using <see cref="MessagingUtilities.EqualsConstantTime"/> to mitigate timing attacks.
		/// </remarks>
		bool IClientDescription.IsValidClientSecret(string secret) {
			
			throw new NotImplementedException();
		}
	}
}
