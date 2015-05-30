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
	/// Categorizes the various types of channel binding elements so they can be properly ordered.
	/// </summary>
	/// <remarks>
	/// The order of these enum values is significant.  
	/// Each successive value requires the protection offered by all the previous values
	/// in order to be reliable.  For example, message expiration is meaningless without
	/// tamper protection to prevent a user from changing the timestamp on a message.
	/// </remarks>
	[Flags]
	public enum MessageProtections {
		/// <summary>
		/// No protection.
		/// </summary>
		None = 0x0,

		/// <summary>
		/// A binding element that signs a message before sending and validates its signature upon receiving.
		/// </summary>
		TamperProtection = 0x1,

		/// <summary>
		/// A binding element that enforces a maximum message age between sending and processing on the receiving side.
		/// </summary>
		Expiration = 0x2,

		/// <summary>
		/// A binding element that prepares messages for replay detection and detects replayed messages on the receiving side.
		/// </summary>
		ReplayProtection = 0x4,

		/// <summary>
		/// All forms of protection together.
		/// </summary>
		All = TamperProtection | Expiration | ReplayProtection,
	}
}