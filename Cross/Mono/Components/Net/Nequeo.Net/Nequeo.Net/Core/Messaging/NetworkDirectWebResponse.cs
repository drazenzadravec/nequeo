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
	using System.Diagnostics;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Net;
	using System.Text;

	/// <summary>
	/// A live network HTTP response
	/// </summary>
	[DebuggerDisplay("{Status} {ContentType.MediaType}")]
	[ContractVerification(true)]
    public class NetworkDirectWebResponse : IncomingWebResponse, IDisposable
    {
		/// <summary>
		/// The network response object, used to initialize this instance, that still needs 
		/// to be closed if applicable.
		/// </summary>
		private HttpWebResponse httpWebResponse;

		/// <summary>
		/// The incoming network response stream.
		/// </summary>
		private Stream responseStream;

		/// <summary>
		/// A value indicating whether a stream reader has already been
		/// created on this instance.
		/// </summary>
		private bool streamReadBegun;

		/// <summary>
		/// Initializes a new instance of the <see cref="NetworkDirectWebResponse"/> class.
		/// </summary>
		/// <param name="requestUri">The request URI.</param>
		/// <param name="response">The response.</param>
        public NetworkDirectWebResponse(Uri requestUri, HttpWebResponse response)
			: base(requestUri, response) {
			Requires.NotNull(requestUri, "requestUri");
			Requires.NotNull(response, "response");
			this.httpWebResponse = response;
			this.responseStream = response.GetResponseStream();
		}

		/// <summary>
		/// Gets the body of the HTTP response.
		/// </summary>
		public override Stream ResponseStream {
			get { return this.responseStream; }
		}

		/// <summary>
		/// Creates a text reader for the response stream.
		/// </summary>
		/// <returns>The text reader, initialized for the proper encoding.</returns>
		public override StreamReader GetResponseReader() {
			this.streamReadBegun = true;
			if (this.responseStream == null) {
				throw new ObjectDisposedException(GetType().Name);
			}

			string contentEncoding = this.Headers[HttpResponseHeader.ContentEncoding];
			if (string.IsNullOrEmpty(contentEncoding)) {
				return new StreamReader(this.ResponseStream);
			} else {
				return new StreamReader(this.ResponseStream, Encoding.GetEncoding(contentEncoding));
			}
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
			ErrorUtilities.VerifyOperation(!this.streamReadBegun, "Network stream reading has already begun.");
			ErrorUtilities.VerifyOperation(this.httpWebResponse != null, "httpWebResponse != null");

			this.streamReadBegun = true;
			var result = new CachedDirectWebResponse(this.RequestUri, this.httpWebResponse, maximumBytesToCache);
			this.Dispose();
			return result;
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (this.responseStream != null) {
					this.responseStream.Dispose();
					this.responseStream = null;
				}
				if (this.httpWebResponse != null) {
					this.httpWebResponse.Close();
					this.httpWebResponse = null;
				}
			}

			base.Dispose(disposing);
		}
	}
}
