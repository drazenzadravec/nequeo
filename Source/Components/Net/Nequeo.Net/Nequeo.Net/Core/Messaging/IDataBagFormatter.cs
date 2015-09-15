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
	/// A serializer for <see cref="DataBag"/>-derived types
	/// </summary>
	/// <typeparam name="T">The DataBag-derived type that is to be serialized/deserialized.</typeparam>
	[ContractClass(typeof(IDataBagFormatterContract<>))]
    public interface IDataBagFormatter<in T> where T : DataBag
    {
		/// <summary>
		/// Serializes the specified message.
		/// </summary>
		/// <param name="message">The message to serialize.  Must not be null.</param>
		/// <returns>A non-null, non-empty value.</returns>
		string Serialize(T message);

		/// <summary>
		/// Deserializes a <see cref="DataBag"/>.
		/// </summary>
		/// <param name="message">The instance to deserialize into</param>
		/// <param name="containingMessage">The message that contains the <see cref="DataBag"/> serialized value.  Must not be null.</param>
		/// <param name="data">The serialized form of the <see cref="DataBag"/> to deserialize.  Must not be null or empty.</param>
		/// <param name="messagePartName">The name of the parameter whose value is to be deserialized.  Used for error message generation.</param>
		void Deserialize(T message, IProtocolMessage containingMessage, string data, string messagePartName);
	}

	/// <summary>
	/// Contract class for the IDataBagFormatter interface.
	/// </summary>
	/// <typeparam name="T">The type of DataBag to serialize.</typeparam>
	[ContractClassFor(typeof(IDataBagFormatter<>))]
    public abstract class IDataBagFormatterContract<T> : IDataBagFormatter<T> where T : DataBag, new()
    {
		/// <summary>
		/// Prevents a default instance of the <see cref="IDataBagFormatterContract&lt;T&gt;"/> class from being created.
		/// </summary>
		private IDataBagFormatterContract() {
		}

		/// <summary>
		/// Serializes the specified message.
		/// </summary>
		/// <param name="message">The message to serialize.  Must not be null.</param>
		/// <returns>A non-null, non-empty value.</returns>
		string IDataBagFormatter<T>.Serialize(T message) {
			Requires.NotNull(message, "message");
			Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Deserializes a <see cref="DataBag"/>.
		/// </summary>
		/// <param name="message">The instance to deserialize into</param>
		/// <param name="containingMessage">The message that contains the <see cref="DataBag"/> serialized value.  Must not be nulll.</param>
		/// <param name="data">The serialized form of the <see cref="DataBag"/> to deserialize.  Must not be null or empty.</param>
		/// <param name="messagePartName">Name of the message part whose value is to be deserialized.  Used for exception messages.</param>
		void IDataBagFormatter<T>.Deserialize(T message, IProtocolMessage containingMessage, string data, string messagePartName) {
			Requires.NotNull(message, "message");
			Requires.NotNull(containingMessage, "containingMessage");
			Requires.NotNullOrEmpty(data, "data");
			Requires.NotNullOrEmpty(messagePartName, "messagePartName");
			Contract.Ensures(Contract.Result<T>() != null);

			throw new System.NotImplementedException();
		}
	}
}