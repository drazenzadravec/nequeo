/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Nequeo.Convertible;

namespace Nequeo.Conversion
{
    /// <summary>
    /// Common conversion helper
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Converts the value into a string of bits.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <returns>The string of bits</returns>
        public static string GetBits(Int32 value)
        {
            Int32[] store = new int[8];
            string result = string.Empty;

            for (int i = 0; i < 8; i++)
            {
                if ((value % 2) > 0)
                    store[i] = 1;
                else
                    store[i] = 0;

                string[] values = (value / 2).ToString().Split(new char[] { '.' }, StringSplitOptions.None);
                value = Convert.ToInt32(values[0]);
            }

            for (int j = 7; j >= 0; j--)
                result = store[j] + result;

            return result;
        }

        /// <summary>
        /// Converts the value into a string of bits.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <returns>The string of bits</returns>
        public static string GetBits(Int64 value)
        {
            Int64[] store = new long[16];
            string result = string.Empty;

            for (int i = 0; i < 16; i++)
            {
                if ((value % 2) > 0)
                    store[i] = 1;
                else
                    store[i] = 0;

                string[] values = (value / 2).ToString().Split(new char[] { '.' }, StringSplitOptions.None);
                value = Convert.ToInt64(values[0]);
            }

            for (int j = 15; j >= 0; j--)
                result = store[j] + result;

            return result;
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
        /// Converts the integer to the equivalent string.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <returns>The string value.</returns>
        public static string IntToString(Int32 value)
        {
            string hexOutput = String.Format("{0:X}", value);
            byte[] byteValues = HexStringToByteArray(hexOutput);
            char[] chars = new char[byteValues.Length];

            for (int i = 0; i < byteValues.Length; i++)
                chars[i] = Convert.ToChar(byteValues[(byteValues.Length - 1) - i]);
                
            // Return the new string.
            return new string(chars);
        }

        /// <summary>
        /// Converts the integer to the equivalent string.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <returns>The string value.</returns>
        public static string IntToString(Int64 value)
        {
            string hexOutput = String.Format("{0:X}", value);
            byte[] byteValues = HexStringToByteArray(hexOutput);
            char[] chars = new char[byteValues.Length];

            for (int i = 0; i < byteValues.Length; i++)
                chars[i] = Convert.ToChar(byteValues[(byteValues.Length - 1) - i]);

            // Return the new string.
            return new string(chars);
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
    }
}
