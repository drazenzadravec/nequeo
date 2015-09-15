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
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
    using Nequeo.Net.Core.Messaging.Reflection;

	/// <summary>
	/// A message part encoder that translates between <c>byte[]</c> and base64web encoded strings.
	/// </summary>
    public class Base64WebEncoder : IMessagePartEncoder
    {
		/// <summary>
		/// Encodes the specified value.
		/// </summary>
		/// <param name="value">The value.  Guaranteed to never be null.</param>
		/// <returns>The <paramref name="value"/> in string form, ready for message transport.</returns>
		public string Encode(object value) {
			return MessagingUtilities.ConvertToBase64WebSafeString((byte[])value);
		}

		/// <summary>
		/// Decodes the specified value.
		/// </summary>
		/// <param name="value">The string value carried by the transport.  Guaranteed to never be null, although it may be empty.</param>
		/// <returns>The deserialized form of the given string.</returns>
		/// <exception cref="FormatException">Thrown when the string value given cannot be decoded into the required object type.</exception>
		public object Decode(string value) {
			return MessagingUtilities.FromBase64WebSafeString(value);
		}
	}
}
