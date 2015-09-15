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
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Net;
	using System.Text;

	/// <summary>
	/// Cached details on the response from a direct web request to a remote party.
	/// </summary>
	[ContractVerification(true)]
	[DebuggerDisplay("{Status} {ContentType.MediaType}, length: {ResponseStream.Length}")]
    public class CachedDirectWebResponse : IncomingWebResponse
    {
		/// <summary>
		/// A seekable, repeatable response stream.
		/// </summary>
		private MemoryStream responseStream;

		/// <summary>
		/// Initializes a new instance of the <see cref="CachedDirectWebResponse"/> class.
		/// </summary>
        public CachedDirectWebResponse()
        {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CachedDirectWebResponse"/> class.
		/// </summary>
		/// <param name="requestUri">The request URI.</param>
		/// <param name="response">The response.</param>
		/// <param name="maximumBytesToRead">The maximum bytes to read.</param>
        public CachedDirectWebResponse(Uri requestUri, HttpWebResponse response, int maximumBytesToRead)
			: base(requestUri, response) {
			Requires.NotNull(requestUri, "requestUri");
			Requires.NotNull(response, "response");
			this.responseStream = CacheNetworkStreamAndClose(response, maximumBytesToRead);

			// BUGBUG: if the response was exactly maximumBytesToRead, we'll incorrectly believe it was truncated.
			this.ResponseTruncated = this.responseStream.Length == maximumBytesToRead;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CachedDirectWebResponse"/> class.
		/// </summary>
		/// <param name="requestUri">The request URI.</param>
		/// <param name="responseUri">The final URI to respond to the request.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="statusCode">The status code.</param>
		/// <param name="contentType">Type of the content.</param>
		/// <param name="contentEncoding">The content encoding.</param>
		/// <param name="responseStream">The response stream.</param>
        public CachedDirectWebResponse(Uri requestUri, Uri responseUri, WebHeaderCollection headers, HttpStatusCode statusCode, string contentType, string contentEncoding, MemoryStream responseStream)
			: base(requestUri, responseUri, headers, statusCode, contentType, contentEncoding) {
			Requires.NotNull(requestUri, "requestUri");
			Requires.NotNull(responseStream, "responseStream");
			this.responseStream = responseStream;
		}

		/// <summary>
		/// Gets a value indicating whether the cached response stream was
		/// truncated to a maximum allowable length.
		/// </summary>
		public bool ResponseTruncated { get; private set; }

		/// <summary>
		/// Gets the body of the HTTP response.
		/// </summary>
		public override Stream ResponseStream {
			get { return this.responseStream; }
		}

		/// <summary>
		/// Gets or sets the cached response stream.
		/// </summary>
        public MemoryStream CachedResponseStream
        {
			get { return this.responseStream; }
			set { this.responseStream = value; }
		}

		/// <summary>
		/// Creates a text reader for the response stream.
		/// </summary>
		/// <returns>The text reader, initialized for the proper encoding.</returns>
		public override StreamReader GetResponseReader() {
			this.ResponseStream.Seek(0, SeekOrigin.Begin);
			string contentEncoding = this.Headers[HttpResponseHeader.ContentEncoding];
			Encoding encoding = null;
			if (!string.IsNullOrEmpty(contentEncoding)) {
				try {
					encoding = Encoding.GetEncoding(contentEncoding);
				} catch (ArgumentException ex) {
                    throw ex;
				}
			}

			return encoding != null ? new StreamReader(this.ResponseStream, encoding) : new StreamReader(this.ResponseStream);
		}

		/// <summary>
		/// Gets the body of the response as a string.
		/// </summary>
		/// <returns>The entire body of the response.</returns>
        public string GetResponseString()
        {
			if (this.ResponseStream != null) {
				string value = this.GetResponseReader().ReadToEnd();
				this.ResponseStream.Seek(0, SeekOrigin.Begin);
				return value;
			} else {
				return null;
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
			return this;
		}

		/// <summary>
		/// Sets the response to some string, encoded as UTF-8.
		/// </summary>
		/// <param name="body">The string to set the response to.</param>
        public void SetResponse(string body)
        {
			if (body == null) {
				this.responseStream = null;
				return;
			}

			Encoding encoding = Encoding.UTF8;
			this.Headers[HttpResponseHeader.ContentEncoding] = encoding.HeaderName;
			this.responseStream = new MemoryStream();
			StreamWriter writer = new StreamWriter(this.ResponseStream, encoding);
			writer.Write(body);
			writer.Flush();
			this.ResponseStream.Seek(0, SeekOrigin.Begin);
		}

		/// <summary>
		/// Caches the network stream and closes it if it is open.
		/// </summary>
		/// <param name="response">The response whose stream is to be cloned.</param>
		/// <param name="maximumBytesToRead">The maximum bytes to cache.</param>
		/// <returns>The seekable Stream instance that contains a copy of what was returned in the HTTP response.</returns>
		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.Contracts.__ContractsRuntime.Assume(System.Boolean,System.String,System.String)", Justification = "No localization required.")]
		private static MemoryStream CacheNetworkStreamAndClose(HttpWebResponse response, int maximumBytesToRead) {
			Requires.NotNull(response, "response");
			Contract.Ensures(Contract.Result<MemoryStream>() != null);

			// Now read and cache the network stream
			Stream networkStream = response.GetResponseStream();
			MemoryStream cachedStream = new MemoryStream(response.ContentLength < 0 ? 4 * 1024 : Math.Min((int)response.ContentLength, maximumBytesToRead));
			try {
				Contract.Assume(networkStream.CanRead, "HttpWebResponse.GetResponseStream() always returns a readable stream."); // CC missing
				Contract.Assume(cachedStream.CanWrite, "This is a MemoryStream -- it's always writable."); // CC missing
				networkStream.CopyTo(cachedStream);
				cachedStream.Seek(0, SeekOrigin.Begin);

				networkStream.Dispose();
				response.Close();

				return cachedStream;
			} catch {
				cachedStream.Dispose();
				throw;
			}
		}
	}
}
