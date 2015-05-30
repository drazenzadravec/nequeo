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
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// An exception to call out a configuration or runtime failure on the part of the
	/// (web) application that is hosting this library.
	/// </summary>
	/// <remarks>
	/// <para>This exception is used rather than <see cref="ProtocolException"/> for those errors
	/// that should never be caught because they indicate a major error in the app itself
	/// or its configuration.</para>
	/// <para>It is an internal exception to assist in making it uncatchable.</para>
	/// </remarks>
	[SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "We don't want this exception to be catchable.")]
	[Serializable]
    public class HostErrorException : Exception
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="HostErrorException"/> class.
		/// </summary>
        public HostErrorException()
        {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HostErrorException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
        public HostErrorException(string message)
			: base(message) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HostErrorException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="inner">The inner exception.</param>
        public HostErrorException(string message, Exception inner)
			: base(message, inner) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HostErrorException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="info"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">
		/// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
		/// </exception>
		protected HostErrorException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
