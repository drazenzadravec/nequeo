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

	/// <summary>
	/// Implemented by messages that have explicit recipients
	/// (direct requests and all indirect messages).
	/// </summary>
	public interface IDirectedProtocolMessage : IProtocolMessage {
		/// <summary>
		/// Gets the preferred method of transport for the message.
		/// </summary>
		/// <remarks>
		/// For indirect messages this will likely be GET+POST, which both can be simulated in the user agent:
		/// the GET with a simple 301 Redirect, and the POST with an HTML form in the response with javascript
		/// to automate submission.
		/// </remarks>
		HttpDeliveryMethods HttpMethods { get; }

		/// <summary>
		/// Gets the URL of the intended receiver of this message.
		/// </summary>
		Uri Recipient { get; }
	}
}
