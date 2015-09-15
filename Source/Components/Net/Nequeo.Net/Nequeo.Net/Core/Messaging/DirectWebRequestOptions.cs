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
	using System.Net;

	/// <summary>
	/// A set of flags that can control the behavior of an individual web request.
	/// </summary>
	[Flags]
	public enum DirectWebRequestOptions {
		/// <summary>
		/// Indicates that default <see cref="HttpWebRequest"/> behavior is required.
		/// </summary>
		None = 0x0,

		/// <summary>
		/// Indicates that any response from the remote server, even those
		/// with HTTP status codes that indicate errors, should not result
		/// in a thrown exception.
		/// </summary>
		/// <remarks>
		/// Even with this flag set, <see cref="ProtocolException"/> should
		/// be thrown when an HTTP protocol error occurs (i.e. timeouts).
		/// </remarks>
		AcceptAllHttpResponses = 0x1,

		/// <summary>
		/// Indicates that the HTTP request must be completed entirely 
		/// using SSL (including any redirects).
		/// </summary>
		RequireSsl = 0x2,
	}
}
