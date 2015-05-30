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
	using System.Linq;
	using System.Text;

	/// <summary>
	/// The interface that classes must implement to be serialized/deserialized
	/// as protocol or extension messages that uses POST multi-part data for binary content.
	/// </summary>
	[ContractClass(typeof(IMessageWithBinaryDataContract))]
	public interface IMessageWithBinaryData : IDirectedProtocolMessage {
		/// <summary>
		/// Gets the parts of the message that carry binary data.
		/// </summary>
		/// <value>A list of parts.  Never null.</value>
		IList<MultipartPostPart> BinaryData { get; }

		/// <summary>
		/// Gets a value indicating whether this message should be sent as multi-part POST.
		/// </summary>
		bool SendAsMultipart { get; }
	}

	/// <summary>
	/// The contract class for the <see cref="IMessageWithBinaryData"/> interface.
	/// </summary>
	[ContractClassFor(typeof(IMessageWithBinaryData))]
    public abstract class IMessageWithBinaryDataContract : IMessageWithBinaryData
    {
		/// <summary>
		/// Prevents a default instance of the <see cref="IMessageWithBinaryDataContract"/> class from being created.
		/// </summary>
		private IMessageWithBinaryDataContract() {
		}

		#region IMessageWithBinaryData Members

		/// <summary>
		/// Gets the parts of the message that carry binary data.
		/// </summary>
		/// <value>A list of parts.  Never null.</value>
		IList<MultipartPostPart> IMessageWithBinaryData.BinaryData {
			get {
				Contract.Ensures(Contract.Result<IList<MultipartPostPart>>() != null);
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this message should be sent as multi-part POST.
		/// </summary>
		bool IMessageWithBinaryData.SendAsMultipart {
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IMessage Properties

		/// <summary>
		/// Gets the version of the protocol or extension this message is prepared to implement.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// Implementations of this interface should ensure that this property never returns null.
		/// </remarks>
		Version IMessage.Version {
			get {
				return default(Version); // dummy return
			}
		}

		/// <summary>
		/// Gets the extra, non-standard Protocol parameters included in the message.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// Implementations of this interface should ensure that this property never returns null.
		/// </remarks>
		IDictionary<string, string> IMessage.ExtraData {
			get {
				return default(IDictionary<string, string>);
			}
		}

		#endregion

		#region IDirectedProtocolMessage Members

		/// <summary>
		/// Gets the preferred method of transport for the message.
		/// </summary>
		/// <remarks>
		/// For indirect messages this will likely be GET+POST, which both can be simulated in the user agent:
		/// the GET with a simple 301 Redirect, and the POST with an HTML form in the response with javascript
		/// to automate submission.
		/// </remarks>
		HttpDeliveryMethods IDirectedProtocolMessage.HttpMethods {
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the URL of the intended receiver of this message.
		/// </summary>
		Uri IDirectedProtocolMessage.Recipient {
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IProtocolMessage Members

		/// <summary>
		/// Gets the level of protection this message requires.
		/// </summary>
		MessageProtections IProtocolMessage.RequiredProtection {
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets a value indicating whether this is a direct or indirect message.
		/// </summary>
		MessageTransport IProtocolMessage.Transport {
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IMessage methods

		/// <summary>
		/// Checks the message state for conformity to the protocol specification
		/// and throws an exception if the message is invalid.
		/// </summary>
		/// <remarks>
		/// 	<para>Some messages have required fields, or combinations of fields that must relate to each other
		/// in specialized ways.  After deserializing a message, this method checks the state of the
		/// message to see if it conforms to the protocol.</para>
		/// 	<para>Note that this property should <i>not</i> check signatures or perform any state checks
		/// outside this scope of this particular message.</para>
		/// </remarks>
		/// <exception cref="ProtocolException">Thrown if the message is invalid.</exception>
		void IMessage.EnsureValidMessage() {
			throw new NotImplementedException();
		}

		#endregion
	}
}
