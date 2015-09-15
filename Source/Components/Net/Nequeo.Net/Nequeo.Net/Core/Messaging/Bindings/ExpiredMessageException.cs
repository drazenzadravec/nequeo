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
	using System.Diagnostics.Contracts;
	using System.Globalization;

	/// <summary>
	/// An exception thrown when a message is received that exceeds the maximum message age limit.
	/// </summary>
	[Serializable]
    public class ExpiredMessageException : ProtocolException
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="ExpiredMessageException"/> class.
		/// </summary>
		/// <param name="utcExpirationDate">The date the message expired.</param>
		/// <param name="faultedMessage">The expired message.</param>
		public ExpiredMessageException(DateTime utcExpirationDate, IProtocolMessage faultedMessage)
			: base(string.Format(CultureInfo.CurrentCulture, MessagingStrings.ExpiredMessage, utcExpirationDate.ToLocalTime(), DateTime.Now), faultedMessage) {
			Requires.True(utcExpirationDate.Kind == DateTimeKind.Utc, "utcExpirationDate");
			Requires.NotNull(faultedMessage, "faultedMessage");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExpiredMessageException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> 
		/// that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The System.Runtime.Serialization.StreamingContext 
		/// that contains contextual information about the source or destination.</param>
        public ExpiredMessageException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
