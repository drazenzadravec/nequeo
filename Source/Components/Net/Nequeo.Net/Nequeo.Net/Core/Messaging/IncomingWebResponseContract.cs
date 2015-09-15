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
	/// Code contract for the <see cref="IncomingWebResponse"/> class.
	/// </summary>
	[ContractClassFor(typeof(IncomingWebResponse))]
    public abstract class IncomingWebResponseContract : IncomingWebResponse
    {
		/// <summary>
		/// Gets the body of the HTTP response.
		/// </summary>
		/// <value></value>
		public override Stream ResponseStream {
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Creates a text reader for the response stream.
		/// </summary>
		/// <returns>
		/// The text reader, initialized for the proper encoding.
		/// </returns>
		public override StreamReader GetResponseReader() {
			Contract.Ensures(Contract.Result<StreamReader>() != null);
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets an offline snapshot version of this instance.
		/// </summary>
		/// <param name="maximumBytesToCache">The maximum bytes from the response stream to cache.</param>
		/// <returns>A snapshot version of this instance.</returns>
		/// <remarks>
		/// If this instance is a <see cref="NetworkDirectWebResponse"/> creating a snapshot
		/// will automatically close and dispose of the underlying response stream.
		/// If this instance is a <see cref="CachedDirectWebResponse"/>, the result will
		/// be the self same instance.
		/// </remarks>
        public override CachedDirectWebResponse GetSnapshot(int maximumBytesToCache)
        {
			Requires.InRange(maximumBytesToCache >= 0, "maximumBytesToCache");
			Requires.ValidState(this.RequestUri != null);
			Contract.Ensures(Contract.Result<CachedDirectWebResponse>() != null);
			throw new NotImplementedException();
		}
	}
}
