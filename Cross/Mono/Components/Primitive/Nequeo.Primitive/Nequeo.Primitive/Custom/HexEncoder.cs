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
    /// Hex encoder.
    /// </summary>
    public sealed class HexEncoder
    {
        private static readonly IEncoder encoder = new HexValueEncoder();

        /// <summary>
        /// Hex encoder.
        /// </summary>
        public HexEncoder()
		{
		}

        /// <summary>
        /// Converts an array of bytes to a hex string.
        /// </summary>
        /// <param name="value">The array of bytes to convert.</param>
        /// <returns>The string of hex values.</returns>
        public static string ByteArrayToHexString(byte[] value)
        {
            int i = 0;
            string[] hexArrayByte = new string[value.Count()];
            foreach (Byte item in value)
                hexArrayByte[i++] = item.ToString("X2");

            // Create the octet string of bytes.
            string hexValue = String.Join("", hexArrayByte);
            return hexValue;
        }

        /// <summary>
        /// Converts a hex string to an array of bytes.
        /// </summary>
        /// <param name="value">The hex string value.</param>
        /// <returns>The array of bytes.</returns>
        public static byte[] HexStringToByteArray(string value)
        {
            int NumberChars = value.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);
            return bytes;
        }

        /// <summary>
        /// Converts the hex value to a number.
        /// </summary>
        /// <param name="value">The hex value as string.</param>
        /// <returns>The number equivalent to the value.</returns>
        public static long HexStringToLong(string value)
        {
            return Int64.Parse(value, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Converts the number to hex string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The hex equivalent to the number.</returns>
        public static string LongToHexString(long value)
        {
            return String.Format("{0:X}", value);
        }

        /// <summary>
        /// Convert to HEX string.
        /// </summary>
        /// <param name="data">The byte array to convert.</param>
        /// <returns>The Hex string.</returns>
		public static string ToHexString(
			byte[] data)
		{
			byte[] hex = Encode(data, 0, data.Length);
            return StringExtensions.FromAsciiByteArray(hex);
		}

        /// <summary>
        /// Convert to HEX string.
        /// </summary>
        /// <param name="data">The byte array to convert.</param>
        /// <param name="off">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>The Hex string.</returns>
		public static string ToHexString(
			byte[]	data,
			int		off,
			int		length)
		{
			byte[] hex = Encode(data, off, length);
            return StringExtensions.FromAsciiByteArray(hex);
        }

		/**
		 * encode the input data producing a Hex encoded byte array.
		 *
		 * @return a byte array containing the Hex encoded data.
		 */
		public static byte[] Encode(
			byte[] data)
		{
			return Encode(data, 0, data.Length);
		}

		/**
		 * encode the input data producing a Hex encoded byte array.
		 *
		 * @return a byte array containing the Hex encoded data.
		 */
		public static byte[] Encode(
			byte[]	data,
			int		off,
			int		length)
		{
			MemoryStream bOut = new MemoryStream(length * 2);

			encoder.Encode(data, off, length, bOut);

			return bOut.ToArray();
		}

		/**
		 * Hex encode the byte data writing it to the given output stream.
		 *
		 * @return the number of bytes produced.
		 */
		public static int Encode(
			byte[]	data,
			Stream	outStream)
		{
			return encoder.Encode(data, 0, data.Length, outStream);
		}

		/**
		 * Hex encode the byte data writing it to the given output stream.
		 *
		 * @return the number of bytes produced.
		 */
		public static int Encode(
			byte[]	data,
			int		off,
			int		length,
			Stream	outStream)
		{
			return encoder.Encode(data, off, length, outStream);
		}

		/**
		 * decode the Hex encoded input data. It is assumed the input data is valid.
		 *
		 * @return a byte array representing the decoded data.
		 */
		public static byte[] Decode(
			byte[] data)
		{
			MemoryStream bOut = new MemoryStream((data.Length + 1) / 2);

			encoder.Decode(data, 0, data.Length, bOut);

			return bOut.ToArray();
		}

		/**
		 * decode the Hex encoded string data - whitespace will be ignored.
		 *
		 * @return a byte array representing the decoded data.
		 */
		public static byte[] Decode(
			string data)
		{
			MemoryStream bOut = new MemoryStream((data.Length + 1) / 2);

			encoder.DecodeString(data, bOut);

			return bOut.ToArray();
		}

		/**
		 * decode the Hex encoded string data writing it to the given output stream,
		 * whitespace characters will be ignored.
		 *
		 * @return the number of bytes produced.
		 */
		public static int Decode(
			string	data,
			Stream	outStream)
		{
			return encoder.DecodeString(data, outStream);
		}
    }
}
