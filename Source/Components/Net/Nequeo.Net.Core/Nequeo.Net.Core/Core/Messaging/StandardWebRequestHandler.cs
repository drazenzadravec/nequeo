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
	using System.Net.Sockets;
	using System.Reflection;
    using Nequeo.Net.Core.Messaging;

	/// <summary>
	/// The default handler for transmitting <see cref="HttpWebRequest"/> instances
	/// and returning the responses.
	/// </summary>
	public class StandardWebRequestHandler : IDirectWebRequestHandler {
		/// <summary>
		/// The set of options this web request handler supports.
		/// </summary>
		private const DirectWebRequestOptions SupportedOptions = DirectWebRequestOptions.AcceptAllHttpResponses;

		/// <summary>
		/// The value to use for the User-Agent HTTP header.
		/// </summary>
		private static string userAgentValue = Assembly.GetExecutingAssembly().GetName().Name + "/" + Assembly.GetExecutingAssembly().GetName().Version;

		/// <summary>
		/// Determines whether this instance can support the specified options.
		/// </summary>
		/// <param name="options">The set of options that might be given in a subsequent web request.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can support the specified options; otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		public bool CanSupport(DirectWebRequestOptions options) {
			return (options & ~SupportedOptions) == 0;
		}

		/// <summary>
		/// Prepares an <see cref="HttpWebRequest"/> that contains an POST entity for sending the entity.
		/// </summary>
		/// <param name="request">The <see cref="HttpWebRequest"/> that should contain the entity.</param>
		/// <returns>
		/// The writer the caller should write out the entity data to.
		/// </returns>
		/// <exception cref="ProtocolException">Thrown for any network error.</exception>
		/// <remarks>
		/// 	<para>The caller should have set the <see cref="HttpWebRequest.ContentLength"/>
		/// and any other appropriate properties <i>before</i> calling this method.</para>
		/// 	<para>Implementations should catch <see cref="WebException"/> and wrap it in a
		/// <see cref="ProtocolException"/> to abstract away the transport and provide
		/// a single exception type for hosts to catch.</para>
		/// </remarks>
		public Stream GetRequestStream(HttpWebRequest request) {
			return this.GetRequestStream(request, DirectWebRequestOptions.None);
		}

		/// <summary>
		/// Prepares an <see cref="HttpWebRequest"/> that contains an POST entity for sending the entity.
		/// </summary>
		/// <param name="request">The <see cref="HttpWebRequest"/> that should contain the entity.</param>
		/// <param name="options">The options to apply to this web request.</param>
		/// <returns>
		/// The writer the caller should write out the entity data to.
		/// </returns>
		/// <exception cref="ProtocolException">Thrown for any network error.</exception>
		/// <remarks>
		/// 	<para>The caller should have set the <see cref="HttpWebRequest.ContentLength"/>
		/// and any other appropriate properties <i>before</i> calling this method.</para>
		/// 	<para>Implementations should catch <see cref="WebException"/> and wrap it in a
		/// <see cref="ProtocolException"/> to abstract away the transport and provide
		/// a single exception type for hosts to catch.</para>
		/// </remarks>
		public Stream GetRequestStream(HttpWebRequest request, DirectWebRequestOptions options) {
			return GetRequestStreamCore(request);
		}

		/// <summary>
		/// Processes an <see cref="HttpWebRequest"/> and converts the
		/// <see cref="HttpWebResponse"/> to a <see cref="IncomingWebResponse"/> instance.
		/// </summary>
		/// <param name="request">The <see cref="HttpWebRequest"/> to handle.</param>
		/// <returns>
		/// An instance of <see cref="IncomingWebResponse"/> describing the response.
		/// </returns>
		/// <exception cref="ProtocolException">Thrown for any network error.</exception>
		/// <remarks>
		/// 	<para>Implementations should catch <see cref="WebException"/> and wrap it in a
		/// <see cref="ProtocolException"/> to abstract away the transport and provide
		/// a single exception type for hosts to catch.  The <see cref="WebException.Response"/>
		/// value, if set, should be Closed before throwing.</para>
		/// </remarks>
		public IncomingWebResponse GetResponse(HttpWebRequest request) {
			return this.GetResponse(request, DirectWebRequestOptions.None);
		}

		/// <summary>
		/// Processes an <see cref="HttpWebRequest"/> and converts the
		/// <see cref="HttpWebResponse"/> to a <see cref="IncomingWebResponse"/> instance.
		/// </summary>
		/// <param name="request">The <see cref="HttpWebRequest"/> to handle.</param>
		/// <param name="options">The options to apply to this web request.</param>
		/// <returns>
		/// An instance of <see cref="IncomingWebResponse"/> describing the response.
		/// </returns>
		/// <exception cref="ProtocolException">Thrown for any network error.</exception>
		/// <remarks>
		/// 	<para>Implementations should catch <see cref="WebException"/> and wrap it in a
		/// <see cref="ProtocolException"/> to abstract away the transport and provide
		/// a single exception type for hosts to catch.  The <see cref="WebException.Response"/>
		/// value, if set, should be Closed before throwing.</para>
		/// </remarks>
		public IncomingWebResponse GetResponse(HttpWebRequest request, DirectWebRequestOptions options) {
			// This request MAY have already been prepared by GetRequestStream, but
			// we have no guarantee, so do it just to be safe.
			PrepareRequest(request, false);

			try {
				
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				return new NetworkDirectWebResponse(request.RequestUri, response);
			} catch (WebException ex) {
				HttpWebResponse response = (HttpWebResponse)ex.Response;
				if (response != null && response.StatusCode == HttpStatusCode.ExpectationFailed &&
					request.ServicePoint.Expect100Continue) {
					// Some OpenID servers doesn't understand the Expect header and send 417 error back.
					// If this server just failed from that, alter the ServicePoint for this server
					// so that we don't send that header again next time (whenever that is).
					// "Expect: 100-Continue" HTTP header. (see Google Code Issue 72)
					// We don't want to blindly set all ServicePoints to not use the Expect header
					// as that would be a security hole allowing any visitor to a web site change
					// the web site's global behavior when calling that host.
					request.ServicePoint.Expect100Continue = false; // TODO: investigate that CAS may throw here

					// An alternative to ServicePoint if we don't have permission to set that,
					// but we'd have to set it BEFORE each request.
					////request.Expect = "";  
				}

				if ((options & DirectWebRequestOptions.AcceptAllHttpResponses) != 0 && response != null &&
					response.StatusCode != HttpStatusCode.ExpectationFailed) {
					return new NetworkDirectWebResponse(request.RequestUri, response);
				}

				// Be sure to close the response stream to conserve resources and avoid
				// filling up all our incoming pipes and denying future requests.
				// If in the future, some callers actually want to read this response
				// we'll need to figure out how to reliably call Close on exception
				// responses at all callers.
				if (response != null) {
					response.Close();
				}

				throw ErrorUtilities.Wrap(ex, MessagingStrings.ErrorInRequestReplyMessage);
			}
		}

		/// <summary>
		/// Determines whether an exception was thrown because of the remote HTTP server returning HTTP 417 Expectation Failed.
		/// </summary>
		/// <param name="ex">The caught exception.</param>
		/// <returns>
		/// 	<c>true</c> if the failure was originally caused by a 417 Exceptation Failed error; otherwise, <c>false</c>.
		/// </returns>
		internal static bool IsExceptionFrom417ExpectationFailed(Exception ex) {
			while (ex != null) {
				WebException webEx = ex as WebException;
				if (webEx != null) {
					HttpWebResponse response = webEx.Response as HttpWebResponse;
					if (response != null) {
						if (response.StatusCode == HttpStatusCode.ExpectationFailed) {
							return true;
						}
					}
				}

				ex = ex.InnerException;
			}

			return false;
		}

		/// <summary>
		/// Initiates a POST request and prepares for sending data.
		/// </summary>
		/// <param name="request">The HTTP request with information about the remote party to contact.</param>
		/// <returns>
		/// The stream where the POST entity can be written.
		/// </returns>
		private static Stream GetRequestStreamCore(HttpWebRequest request) {
			PrepareRequest(request, true);

			try {
				return request.GetRequestStream();
			} catch (SocketException ex) {
				throw ErrorUtilities.Wrap(ex, MessagingStrings.WebRequestFailed, request.RequestUri);
			} catch (WebException ex) {
				throw ErrorUtilities.Wrap(ex, MessagingStrings.WebRequestFailed, request.RequestUri);
			}
		}

		/// <summary>
		/// Prepares an HTTP request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="preparingPost"><c>true</c> if this is a POST request whose headers have not yet been sent out; <c>false</c> otherwise.</param>
		private static void PrepareRequest(HttpWebRequest request, bool preparingPost) {
			Requires.NotNull(request, "request");

			// Be careful to not try to change the HTTP headers that have already gone out.
			if (preparingPost || request.Method == "GET") {
				// Set/override a few properties of the request to apply our policies for requests.
				if (Debugger.IsAttached) {
					// Since a debugger is attached, requests may be MUCH slower,
					// so give ourselves huge timeouts.
					request.ReadWriteTimeout = (int)TimeSpan.FromHours(1).TotalMilliseconds;
					request.Timeout = (int)TimeSpan.FromHours(1).TotalMilliseconds;
				}

				// Some sites, such as Technorati, return 403 Forbidden on identity
				// pages unless a User-Agent header is included.
				if (string.IsNullOrEmpty(request.UserAgent)) {
					request.UserAgent = userAgentValue;
				}
			}
		}
	}
}
