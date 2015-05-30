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
	using System.IO;

	/// <summary>
	/// An interface implemented by <see cref="DataBag"/>-derived types that support binary serialization.
	/// </summary>
	[ContractClass(typeof(IStreamSerializingDataBaContract))]
	public interface IStreamSerializingDataBag {
		/// <summary>
		/// Serializes the instance to the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void Serialize(Stream stream);

		/// <summary>
		/// Initializes the fields on this instance from the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void Deserialize(Stream stream);
	}

	/// <summary>
	/// Code Contract for the <see cref="IStreamSerializingDataBag"/> interface.
	/// </summary>
	[ContractClassFor(typeof(IStreamSerializingDataBag))]
    public abstract class IStreamSerializingDataBaContract : IStreamSerializingDataBag
    {
		/// <summary>
		/// Serializes the instance to the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IStreamSerializingDataBag.Serialize(Stream stream) {
			Contract.Requires(stream != null);
			Contract.Requires(stream.CanWrite);
			throw new NotImplementedException();
		}

		/// <summary>
		/// Initializes the fields on this instance from the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IStreamSerializingDataBag.Deserialize(Stream stream) {
			Contract.Requires(stream != null);
			Contract.Requires(stream.CanRead);
			throw new NotImplementedException();
		}
	}
}
