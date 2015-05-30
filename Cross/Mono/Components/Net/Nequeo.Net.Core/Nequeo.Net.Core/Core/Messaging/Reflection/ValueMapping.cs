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

namespace Nequeo.Net.Core.Messaging.Reflection
{
	using System;
	using System.Diagnostics.Contracts;

	/// <summary>
	/// A pair of conversion functions to map some type to a string and back again.
	/// </summary>
	[ContractVerification(true)]
    public struct ValueMapping
    {
		/// <summary>
		/// The mapping function that converts some custom type to a string.
		/// </summary>
        public readonly Func<object, string> ValueToString;

		/// <summary>
		/// The mapping function that converts some custom type to the original string
		/// (possibly non-normalized) that represents it.
		/// </summary>
        public readonly Func<object, string> ValueToOriginalString;

		/// <summary>
		/// The mapping function that converts a string to some custom type.
		/// </summary>
        public readonly Func<string, object> StringToValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueMapping"/> struct.
		/// </summary>
		/// <param name="toString">The mapping function that converts some custom value to a string.</param>
		/// <param name="toOriginalString">The mapping function that converts some custom value to its original (non-normalized) string.  May be null if the same as the <paramref name="toString"/> function.</param>
		/// <param name="toValue">The mapping function that converts a string to some custom value.</param>
        public ValueMapping(Func<object, string> toString, Func<object, string> toOriginalString, Func<string, object> toValue)
        {
			Requires.NotNull(toString, "toString");
			Requires.NotNull(toValue, "toValue");

			this.ValueToString = toString;
			this.ValueToOriginalString = toOriginalString ?? toString;
			this.StringToValue = toValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueMapping"/> struct.
		/// </summary>
		/// <param name="encoder">The encoder.</param>
        public ValueMapping(IMessagePartEncoder encoder)
        {
			Requires.NotNull(encoder, "encoder");
			var nullEncoder = encoder as IMessagePartNullEncoder;
			string nullString = nullEncoder != null ? nullEncoder.EncodedNullValue : null;

			var originalStringEncoder = encoder as IMessagePartOriginalEncoder;
			Func<object, string> originalStringEncode = encoder.Encode;
			if (originalStringEncoder != null) {
				originalStringEncode = originalStringEncoder.EncodeAsOriginalString;
			}

			this.ValueToString = obj => (obj != null) ? encoder.Encode(obj) : nullString;
			this.StringToValue = str => (str != null) ? encoder.Decode(str) : null;
			this.ValueToOriginalString = obj => (obj != null) ? originalStringEncode(obj) : nullString;
		}
	}
}
