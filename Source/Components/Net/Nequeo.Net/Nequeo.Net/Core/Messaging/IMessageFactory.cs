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
	using System.Diagnostics.Contracts;

	/// <summary>
	/// A tool to analyze an incoming message to figure out what concrete class
	/// is designed to deserialize it and instantiates that class.
	/// </summary>
	[ContractClass(typeof(IMessageFactoryContract))]
	public interface IMessageFactory {
		/// <summary>
		/// Analyzes an incoming request message payload to discover what kind of 
		/// message is embedded in it and returns the type, or null if no match is found.
		/// </summary>
		/// <param name="recipient">The intended or actual recipient of the request message.</param>
		/// <param name="fields">The name/value pairs that make up the message payload.</param>
		/// <returns>
		/// A newly instantiated <see cref="IProtocolMessage"/>-derived object that this message can
		/// deserialize to.  Null if the request isn't recognized as a valid protocol message.
		/// </returns>
		IDirectedProtocolMessage GetNewRequestMessage(MessageReceivingEndpoint recipient, IDictionary<string, string> fields);

		/// <summary>
		/// Analyzes an incoming request message payload to discover what kind of 
		/// message is embedded in it and returns the type, or null if no match is found.
		/// </summary>
		/// <param name="request">
		/// The message that was sent as a request that resulted in the response.
		/// </param>
		/// <param name="fields">The name/value pairs that make up the message payload.</param>
		/// <returns>
		/// A newly instantiated <see cref="IProtocolMessage"/>-derived object that this message can
		/// deserialize to.  Null if the request isn't recognized as a valid protocol message.
		/// </returns>
		IDirectResponseProtocolMessage GetNewResponseMessage(IDirectedProtocolMessage request, IDictionary<string, string> fields);
	}

	/// <summary>
	/// Code contract for the <see cref="IMessageFactory"/> interface.
	/// </summary>
	[ContractClassFor(typeof(IMessageFactory))]
    public abstract class IMessageFactoryContract : IMessageFactory
    {
		/// <summary>
		/// Prevents a default instance of the <see cref="IMessageFactoryContract"/> class from being created.
		/// </summary>
		private IMessageFactoryContract() {
		}

		#region IMessageFactory Members

		/// <summary>
		/// Analyzes an incoming request message payload to discover what kind of
		/// message is embedded in it and returns the type, or null if no match is found.
		/// </summary>
		/// <param name="recipient">The intended or actual recipient of the request message.</param>
		/// <param name="fields">The name/value pairs that make up the message payload.</param>
		/// <returns>
		/// A newly instantiated <see cref="IProtocolMessage"/>-derived object that this message can
		/// deserialize to.  Null if the request isn't recognized as a valid protocol message.
		/// </returns>
		IDirectedProtocolMessage IMessageFactory.GetNewRequestMessage(MessageReceivingEndpoint recipient, IDictionary<string, string> fields) {
			Requires.NotNull(recipient, "recipient");
			Requires.NotNull(fields, "fields");

			throw new NotImplementedException();
		}

		/// <summary>
		/// Analyzes an incoming request message payload to discover what kind of
		/// message is embedded in it and returns the type, or null if no match is found.
		/// </summary>
		/// <param name="request">The message that was sent as a request that resulted in the response.</param>
		/// <param name="fields">The name/value pairs that make up the message payload.</param>
		/// <returns>
		/// A newly instantiated <see cref="IProtocolMessage"/>-derived object that this message can
		/// deserialize to.  Null if the request isn't recognized as a valid protocol message.
		/// </returns>
		IDirectResponseProtocolMessage IMessageFactory.GetNewResponseMessage(IDirectedProtocolMessage request, IDictionary<string, string> fields) {
			Requires.NotNull(request, "request");
			Requires.NotNull(fields, "fields");
			throw new NotImplementedException();
		}

		#endregion
	}
}
