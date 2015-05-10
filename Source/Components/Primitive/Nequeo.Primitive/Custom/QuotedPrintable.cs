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
    /// Encodes data that consists of printable characters in the US-ASCII character set. See RFC 2406 Section 6.7.
    /// </summary>
    public class QuotedPrintable
    {
        /// <summary>
        /// Encode the array of characters.
        /// </summary>
        /// <param name="data">The array of characters.</param>
        /// <returns>The encoded string.</returns>
        public static string Encode(char[] data)
        {
            int ascii;
            string encodedChar = "";
            StringBuilder builder = new StringBuilder();

            // For each char.
            for (int i = 0; i < data.Length; i++)
            {
                ascii = Convert.ToInt32(data[i]);
                if(ascii < 32 || ascii > 126 || ascii == 61)
                {
                    encodedChar = HexEncoder.ToHexString(new byte[] { Convert.ToByte(data[i]) });
                    if (encodedChar.Length == 1)
                        encodedChar = "0" + encodedChar;

                    builder.Append("=" + encodedChar.ToUpper());
                }
                else
                {
                    builder.Append(data[i]);
                }
            }

            // Return the encoded string
            return builder.ToString();
        }

        /// <summary>
        /// Decode the array of characters.
        /// </summary>
        /// <param name="data">The array of characters.</param>
        /// <returns>The decoded string.</returns>
        public static string Decode(char[] data)
        {
            StringBuilder builder = new StringBuilder();

            // For each char.
            for (int i = 0; i < data.Length; i++)
            {
                if(data[i] == '=')
                {
                    string value = null;
                    if (data[i + 1] == '0')
                    {
                        value = data[i + 2].ToString();
                    }
                    else
                    {
                        value = data[i + 1].ToString() + data[i + 2].ToString();
                    }

                    int intValue = Convert.ToInt32("0x" + value);
                    if (value == HexEncoder.ToHexString(new byte[] { Convert.ToByte(intValue) }))
                    {
                        builder.Append(Convert.ToChar(intValue));
                        i+= 2;
                    }
                    else
                    {
                        builder.Append(data[i]);
                    }
                }
                else
                {
                    builder.Append(data[i]);
                }
            }

            // Return the encoded string
            return builder.ToString();
        }
    }
}
