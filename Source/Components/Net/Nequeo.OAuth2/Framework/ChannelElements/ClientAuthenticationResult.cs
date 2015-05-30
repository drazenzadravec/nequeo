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

namespace Nequeo.Net.OAuth2.Framework.ChannelElements
{
	/// <summary>
	/// Describes the various levels at which client information may be extracted from an inbound message.
	/// </summary>
	public enum ClientAuthenticationResult {
		/// <summary>
		/// No client identification or authentication was discovered.
		/// </summary>
		NoAuthenticationRecognized,

		/// <summary>
		/// The client identified itself, but did not attempt to authenticate itself.
		/// </summary>
		ClientIdNotAuthenticated,

		/// <summary>
		/// The client authenticated itself (provided compelling evidence that it was who it claims to be).
		/// </summary>
		ClientAuthenticated,

		/// <summary>
		/// The client failed in an attempt to authenticate itself, claimed to be an unrecognized client, or otherwise messed up.
		/// </summary>
		ClientAuthenticationRejected,
	}
}
