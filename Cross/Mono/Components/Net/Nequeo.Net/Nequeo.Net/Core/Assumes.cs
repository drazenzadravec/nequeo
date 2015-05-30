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

namespace Nequeo.Net.Core
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Text;

	/// <summary>
	/// Internal state consistency checks that throw an internal error exception when they fail.
	/// </summary>
    public static class Assumes
    {
		/// <summary>
		/// Validates some expression describing the acceptable condition evaluates to true.
		/// </summary>
		/// <param name="condition">The expression that must evaluate to true to avoid an internal error exception.</param>
		/// <param name="message">The message to include with the exception.</param>
		[Pure, DebuggerStepThrough]
        public static void True(bool condition, string message = null)
        {
			if (!condition) {
				Fail(message);
			}
		}

		/// <summary>
		/// Validates some expression describing the acceptable condition evaluates to true.
		/// </summary>
		/// <param name="condition">The expression that must evaluate to true to avoid an internal error exception.</param>
		/// <param name="unformattedMessage">The unformatted message.</param>
		/// <param name="args">Formatting arguments.</param>
		[Pure, DebuggerStepThrough]
        public static void True(bool condition, string unformattedMessage, params object[] args)
        {
			if (!condition) {
				Fail(string.Format(CultureInfo.CurrentCulture, unformattedMessage, args));
			}
		}

		/// <summary>
		/// Throws an internal error exception.
		/// </summary>
		/// <param name="message">The message.</param>
		[Pure, DebuggerStepThrough]
        public static void Fail(string message = null)
        {
			if (message != null) {
				throw new InternalErrorException(message);
			} else {
				throw new InternalErrorException();
			}
		}

		/// <summary>
		/// Throws an internal error exception.
		/// </summary>
		/// <returns>Nothing.  This method always throws.</returns>
        public static Exception NotReachable()
        {
			throw new InternalErrorException();
		}

		/// <summary>
		/// An internal error exception that should never be caught.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "This exception should never be caught.")]
		[Serializable]
		private class InternalErrorException : Exception {
			/// <summary>
			/// Initializes a new instance of the <see cref="InternalErrorException"/> class.
			/// </summary>
			internal InternalErrorException() {
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="InternalErrorException"/> class.
			/// </summary>
			/// <param name="message">The message.</param>
			internal InternalErrorException(string message)
				: base(message) {
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="InternalErrorException"/> class.
			/// </summary>
			/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
			/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
			/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
			/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
			protected InternalErrorException(
			  SerializationInfo info,
			  StreamingContext context)
				: base(info, context) {
			}
		}
	}
}
