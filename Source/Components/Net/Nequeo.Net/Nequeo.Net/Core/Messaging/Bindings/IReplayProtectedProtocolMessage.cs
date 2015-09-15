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

namespace Nequeo.Net.Core.Messaging.Bindings
{
	using System;
    using Nequeo.Net.Core.Messaging;

	/// <summary>
	/// The contract a message that has an allowable time window for processing must implement.
	/// </summary>
	/// <remarks>
	/// All replay-protected messages must also be set to expire so the nonces do not have
	/// to be stored indefinitely.
	/// </remarks>
    public interface IReplayProtectedProtocolMessage : IExpiringProtocolMessage, IDirectedProtocolMessage
    {
		/// <summary>
		/// Gets the context within which the nonce must be unique.
		/// </summary>
		/// <value>
		/// The value of this property must be a value assigned by the nonce consumer
		/// to represent the entity that generated the nonce.  The value must never be
		/// <c>null</c> but may be the empty string.
		/// This value is treated as case-sensitive.
		/// </value>
		string NonceContext { get; }

		/// <summary>
		/// Gets or sets the nonce that will protect the message from replay attacks.
		/// </summary>
		string Nonce { get; set; }
	}
}
