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
	using System.Diagnostics.Contracts;

	/// <summary>
	/// An interface that must be implemented by message transforms/validators in order
	/// to be included in the channel stack.
	/// </summary>
	[ContractClass(typeof(IChannelBindingElementContract))]
	public interface IChannelBindingElement {
		/// <summary>
		/// Gets or sets the channel that this binding element belongs to.
		/// </summary>
		/// <remarks>
		/// This property is set by the channel when it is first constructed.
		/// </remarks>
		Channel Channel { get; set; }

		/// <summary>
		/// Gets the protection commonly offered (if any) by this binding element.
		/// </summary>
		/// <remarks>
		/// This value is used to assist in sorting binding elements in the channel stack.
		/// </remarks>
		MessageProtections Protection { get; }

		/// <summary>
		/// Prepares a message for sending based on the rules of this channel binding element.
		/// </summary>
		/// <param name="message">The message to prepare for sending.</param>
		/// <returns>
		/// The protections (if any) that this binding element applied to the message.
		/// Null if this binding element did not even apply to this binding element.
		/// </returns>
		/// <remarks>
		/// Implementations that provide message protection must honor the 
		/// <see cref="MessagePartAttribute.RequiredProtection"/> properties where applicable.
		/// </remarks>
		MessageProtections? ProcessOutgoingMessage(IProtocolMessage message);

		/// <summary>
		/// Performs any transformation on an incoming message that may be necessary and/or
		/// validates an incoming message based on the rules of this channel binding element.
		/// </summary>
		/// <param name="message">The incoming message to process.</param>
		/// <returns>
		/// The protections (if any) that this binding element applied to the message.
		/// Null if this binding element did not even apply to this binding element.
		/// </returns>
		/// <exception cref="ProtocolException">
		/// Thrown when the binding element rules indicate that this message is invalid and should
		/// NOT be processed.
		/// </exception>
		/// <remarks>
		/// Implementations that provide message protection must honor the 
		/// <see cref="MessagePartAttribute.RequiredProtection"/> properties where applicable.
		/// </remarks>
		MessageProtections? ProcessIncomingMessage(IProtocolMessage message);
	}

	/// <summary>
	/// Code Contract for the <see cref="IChannelBindingElement"/> interface.
	/// </summary>
	[ContractClassFor(typeof(IChannelBindingElement))]
    public abstract class IChannelBindingElementContract : IChannelBindingElement
    {
		/// <summary>
		/// Prevents a default instance of the <see cref="IChannelBindingElementContract"/> class from being created.
		/// </summary>
		private IChannelBindingElementContract() {
		}

		#region IChannelBindingElement Members

		/// <summary>
		/// Gets or sets the channel that this binding element belongs to.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// This property is set by the channel when it is first constructed.
		/// </remarks>
		Channel IChannelBindingElement.Channel {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the protection commonly offered (if any) by this binding element.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// This value is used to assist in sorting binding elements in the channel stack.
		/// </remarks>
		MessageProtections IChannelBindingElement.Protection {
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Prepares a message for sending based on the rules of this channel binding element.
		/// </summary>
		/// <param name="message">The message to prepare for sending.</param>
		/// <returns>
		/// The protections (if any) that this binding element applied to the message.
		/// Null if this binding element did not even apply to this binding element.
		/// </returns>
		/// <remarks>
		/// Implementations that provide message protection must honor the
		/// <see cref="MessagePartAttribute.RequiredProtection"/> properties where applicable.
		/// </remarks>
		MessageProtections? IChannelBindingElement.ProcessOutgoingMessage(IProtocolMessage message) {
			Requires.NotNull(message, "message");
			Requires.ValidState(((IChannelBindingElement)this).Channel != null);
			throw new NotImplementedException();
		}

		/// <summary>
		/// Performs any transformation on an incoming message that may be necessary and/or
		/// validates an incoming message based on the rules of this channel binding element.
		/// </summary>
		/// <param name="message">The incoming message to process.</param>
		/// <returns>
		/// The protections (if any) that this binding element applied to the message.
		/// Null if this binding element did not even apply to this binding element.
		/// </returns>
		/// <exception cref="ProtocolException">
		/// Thrown when the binding element rules indicate that this message is invalid and should
		/// NOT be processed.
		/// </exception>
		/// <remarks>
		/// Implementations that provide message protection must honor the
		/// <see cref="MessagePartAttribute.RequiredProtection"/> properties where applicable.
		/// </remarks>
		MessageProtections? IChannelBindingElement.ProcessIncomingMessage(IProtocolMessage message) {
			Requires.NotNull(message, "message");
			Requires.ValidState(((IChannelBindingElement)this).Channel != null);
			throw new NotImplementedException();
		}

		#endregion
	}
}
