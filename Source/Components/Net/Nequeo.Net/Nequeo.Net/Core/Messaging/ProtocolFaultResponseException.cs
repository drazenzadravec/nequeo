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

namespace Nequeo.Net.Core.Messaging
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// An exception to represent errors in the local or remote implementation of the protocol
	/// that includes the response message that should be returned to the HTTP client to comply
	/// with the protocol specification.
	/// </summary>
	public class ProtocolFaultResponseException : ProtocolException {
		/// <summary>
		/// The channel that produced the error response message, to be used in constructing the actual HTTP response.
		/// </summary>
		private readonly Channel channel;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProtocolFaultResponseException"/> class
		/// such that it can be sent as a protocol message response to a remote caller.
		/// </summary>
		/// <param name="channel">The channel to use when encoding the response message.</param>
		/// <param name="errorResponse">The message to send back to the HTTP client.</param>
		/// <param name="faultedMessage">The message that was the cause of the exception.  May be null.</param>
		/// <param name="innerException">The inner exception.</param>
		/// <param name="message">The message for the exception.</param>
        public ProtocolFaultResponseException(Channel channel, IDirectResponseProtocolMessage errorResponse, IProtocolMessage faultedMessage = null, Exception innerException = null, string message = null)
			: base(message ?? (innerException != null ? innerException.Message : null), faultedMessage, innerException) {
			Requires.NotNull(channel, "channel");
			Requires.NotNull(errorResponse, "errorResponse");
			this.channel = channel;
			this.ErrorResponseMessage = errorResponse;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProtocolFaultResponseException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> 
		/// that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The System.Runtime.Serialization.StreamingContext 
		/// that contains contextual information about the source or destination.</param>
        public ProtocolFaultResponseException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the protocol message to send back to the client to report the error.
		/// </summary>
		public IDirectResponseProtocolMessage ErrorResponseMessage { get; private set; }

		/// <summary>
		/// Creates the HTTP response to forward to the client to report the error.
		/// </summary>
		/// <returns>The HTTP response.</returns>
		public OutgoingWebResponse CreateErrorResponse() {
			var response = this.channel.PrepareResponse(this.ErrorResponseMessage);
			return response;
		}
	}
}
