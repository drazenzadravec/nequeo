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

namespace Nequeo.Net.Core.Messaging.Bindings
{
	using System;

	/// <summary>
	/// The contract a message that has an allowable time window for processing must implement.
	/// </summary>
	/// <remarks>
	/// All expiring messages must also be signed to prevent tampering with the creation date.
	/// </remarks>
    public interface IExpiringProtocolMessage : IProtocolMessage
    {
		/// <summary>
		/// Gets or sets the UTC date/time the message was originally sent onto the network.
		/// </summary>
		/// <remarks>
		/// The property setter should ensure a UTC date/time,
		/// and throw an exception if this is not possible.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// Thrown when a DateTime that cannot be converted to UTC is set.
		/// </exception>
		DateTime UtcCreationDate { get; set; }
	}
}
