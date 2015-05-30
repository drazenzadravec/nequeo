/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nequeo.Extension;

namespace Nequeo.Custom
{
    /// <summary>
    /// Base64 encoder.
    /// </summary>
    public sealed class Base64Encoder
    {
        /// <summary>
        /// Base64 encoder.
        /// </summary>
        private Base64Encoder()
		{
		}

        /// <summary>
        /// Encode the input data producing a base 64 encoded byte array.
        /// </summary>
        /// <param name="data">The data to encode.</param>
        /// <returns>A byte array containing the base 64 encoded data.</returns>
		public static byte[] Encode(
			byte[] data)
		{
			string s = Convert.ToBase64String(data, 0, data.Length);
            return s.ToAsciiByteArray();
		}

        /// <summary>
        /// Encode the byte data to base 64 writing it to the given output stream.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="outStream">The stream.</param>
        /// <returns>The number of bytes produced.</returns>
		public static int Encode(
			byte[]	data,
			Stream	outStream)
		{
			string s = Convert.ToBase64String(data, 0, data.Length);
            byte[] encoded = s.ToAsciiByteArray();
			outStream.Write(encoded, 0, encoded.Length);
			return encoded.Length;
		}

        /// <summary>
        /// Encode the byte data to base 64 writing it to the given output stream.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="off">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="outStream">The stream.</param>
        /// <returns>The number of bytes produced.</returns>
		public static int Encode(
			byte[]	data,
			int		off,
			int		length,
			Stream	outStream)
		{
			string s = Convert.ToBase64String(data, off, length);
            byte[] encoded = s.ToAsciiByteArray();
			outStream.Write(encoded, 0, encoded.Length);
			return encoded.Length;
		}

        /// <summary>
        /// Decode the base 64 encoded input data. It is assumed the input data is valid.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A byte array representing the decoded data.</returns>
		public static byte[] Decode(
			byte[] data)
		{
            string s = StringExtensions.FromAsciiByteArray(data);
			return Convert.FromBase64String(s);
		}

        /// <summary>
        /// Decode the base 64 encoded string data - whitespace will be ignored.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A byte array representing the decoded data.</returns>
		public static byte[] Decode(
			string data)
		{
			return Convert.FromBase64String(data);
		}

        /// <summary>
        /// Decode the base 64 encoded string data writing it to the given output stream, whitespace characters will be ignored.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="outStream">The stream.</param>
        /// <returns>The number of bytes produced.</returns>
		public static int Decode(
			string	data,
			Stream	outStream)
		{
			byte[] decoded = Decode(data);
			outStream.Write(decoded, 0, decoded.Length);
			return decoded.Length;
		}
    }
}
