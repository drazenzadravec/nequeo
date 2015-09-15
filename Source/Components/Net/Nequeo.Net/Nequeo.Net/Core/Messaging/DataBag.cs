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
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;

	/// <summary>
	/// A collection of message parts that will be serialized into a single string,
	/// to be set into a larger message.
	/// </summary>
	public abstract class DataBag : IMessage {
		/// <summary>
		/// The default version for DataBags.
		/// </summary>
		private static readonly Version DefaultVersion = new Version(1, 0);

		/// <summary>
		/// The backing field for the <see cref="IMessage.Version"/> property.
		/// </summary>
		private Version version;

		/// <summary>
		/// A dictionary to contain extra message data.
		/// </summary>
		private Dictionary<string, string> extraData = new Dictionary<string, string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="DataBag"/> class.
		/// </summary>
		protected DataBag()
			: this(DefaultVersion) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataBag"/> class.
		/// </summary>
		/// <param name="version">The DataBag version.</param>
		protected DataBag(Version version) {
			Contract.Requires(version != null);
			this.version = version;
		}

		/// <summary>
		/// Gets the version of the protocol or extension this message is prepared to implement.
		/// </summary>
		/// <remarks>
		/// Implementations of this interface should ensure that this property never returns null.
		/// </remarks>
		Version IMessage.Version {
			get { return this.version; }
		}

		/// <summary>
		/// Gets the extra, non-standard Protocol parameters included in the message.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// Implementations of this interface should ensure that this property never returns null.
		/// </remarks>
		public IDictionary<string, string> ExtraData {
			get { return this.extraData; }
		}

		/// <summary>
		/// Gets or sets the nonce.
		/// </summary>
		/// <value>The nonce.</value>
		[MessagePart]
		public byte[] Nonce { get; set; }

		/// <summary>
		/// Gets or sets the UTC creation date of this token.
		/// </summary>
		/// <value>The UTC creation date.</value>
		[MessagePart("ts", IsRequired = true, Encoder = typeof(TimestampEncoder))]
        public DateTime UtcCreationDate { get; set; }

		/// <summary>
		/// Gets or sets the signature.
		/// </summary>
		/// <value>The signature.</value>
        public byte[] Signature { get; set; }

		/// <summary>
		/// Gets or sets the message that delivered this DataBag instance to this host.
		/// </summary>
        public IProtocolMessage ContainingMessage { get; set; }

		/// <summary>
		/// Gets the type of this instance.
		/// </summary>
		/// <value>The type of the bag.</value>
		/// <remarks>
		/// This ensures that one token cannot be misused as another kind of token.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Accessed by reflection")]
		[MessagePart("t", IsRequired = true, AllowEmpty = false)]
		protected virtual Type BagType {
			get { return this.GetType(); }
		}

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
			this.EnsureValidMessage();
		}

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
		protected virtual void EnsureValidMessage() {
		}
	}
}
