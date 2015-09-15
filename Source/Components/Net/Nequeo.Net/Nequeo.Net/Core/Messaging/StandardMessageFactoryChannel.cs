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
	using Reflection;

	/// <summary>
	/// A channel that uses the standard message factory.
	/// </summary>
	public abstract class StandardMessageFactoryChannel : Channel {
		/// <summary>
		/// The message types receivable by this channel.
		/// </summary>
		private readonly ICollection<Type> messageTypes;

		/// <summary>
		/// The protocol versions supported by this channel.
		/// </summary>
		private readonly ICollection<Version> versions;

		/// <summary>
		/// Initializes a new instance of the <see cref="StandardMessageFactoryChannel"/> class.
		/// </summary>
		/// <param name="messageTypes">The message types that might be encountered.</param>
		/// <param name="versions">All the possible message versions that might be encountered.</param>
		/// <param name="bindingElements">
		/// The binding elements to use in sending and receiving messages.
		/// The order they are provided is used for outgoing messgaes, and reversed for incoming messages.
		/// </param>
		protected StandardMessageFactoryChannel(ICollection<Type> messageTypes, ICollection<Version> versions, params IChannelBindingElement[] bindingElements)
			: base(new StandardMessageFactory(), bindingElements) {
			Requires.NotNull(messageTypes, "messageTypes");
			Requires.NotNull(versions, "versions");

			this.messageTypes = messageTypes;
			this.versions = versions;
			this.StandardMessageFactory.AddMessageTypes(GetMessageDescriptions(this.messageTypes, this.versions, this.MessageDescriptions));
		}

		/// <summary>
		/// Gets or sets a tool that can figure out what kind of message is being received
		/// so it can be deserialized.
		/// </summary>
        public StandardMessageFactory StandardMessageFactory
        {
			get { return (Messaging.StandardMessageFactory)this.MessageFactory; }
			set { this.MessageFactory = value; }
		}

		/// <summary>
		/// Gets or sets the message descriptions.
		/// </summary>
        public sealed override MessageDescriptionCollection MessageDescriptions
        {
			get {
				return base.MessageDescriptions;
			}

			set {
				base.MessageDescriptions = value;

				// We must reinitialize the message factory so it can use the new message descriptions.
				var factory = new StandardMessageFactory();
				factory.AddMessageTypes(GetMessageDescriptions(this.messageTypes, this.versions, value));
				this.MessageFactory = factory;
			}
		}

		/// <summary>
		/// Gets or sets a tool that can figure out what kind of message is being received
		/// so it can be deserialized.
		/// </summary>
		protected sealed override IMessageFactory MessageFactory {
			get {
				return (StandardMessageFactory)base.MessageFactory;
			}

			set {
				StandardMessageFactory newValue = (StandardMessageFactory)value;
				base.MessageFactory = newValue;
			}
		}

		/// <summary>
		/// Generates all the message descriptions for a given set of message types and versions.
		/// </summary>
		/// <param name="messageTypes">The message types.</param>
		/// <param name="versions">The message versions.</param>
		/// <param name="descriptionsCache">The cache to use when obtaining the message descriptions.</param>
		/// <returns>The generated/retrieved message descriptions.</returns>
		private static IEnumerable<MessageDescription> GetMessageDescriptions(ICollection<Type> messageTypes, ICollection<Version> versions, MessageDescriptionCollection descriptionsCache)
		{
			Requires.NotNull(messageTypes, "messageTypes");
			Requires.NotNull(descriptionsCache, "descriptionsCache");
			Contract.Ensures(Contract.Result<IEnumerable<MessageDescription>>() != null);

			// Get all the MessageDescription objects through the standard cache,
			// so that perhaps it will be a quick lookup, or at least it will be
			// stored there for a quick lookup later.
			var messageDescriptions = new List<MessageDescription>(messageTypes.Count * versions.Count);
			messageDescriptions.AddRange(from version in versions
			                             from messageType in messageTypes
			                             select descriptionsCache.Get(messageType, version));

			return messageDescriptions;
		}
	}
}
