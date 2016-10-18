/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Http2.Protocol
{
    /// <summary>
    /// Frame helper.
    /// </summary>
    internal static class FrameHelper
    {
        /// <summary>
        /// Set bit.
        /// </summary>
        /// <param name="input">Input.</param>
        /// <param name="value">value.</param>
        /// <param name="offset">Offset.</param>
        /// <returns>The set bit.</returns>
        public static byte SetBit(byte input, bool value, byte offset)
        {
            Contract.Assert(offset <= 7);

            if (value == GetBit(input, offset))
            {
                return input;
            }

            return (byte)(input ^ (1 << offset));
        }

        /// <summary>
        /// Set bit.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="value">Value.</param>
        /// <param name="offset">Offset.</param>
        public static void SetBit(ref byte input, bool value, byte offset)
        {
            Contract.Assert(offset <= 7);

            if (value == GetBit(input, offset))
            {
                return;
            }

            input ^= (byte)(1 << offset);
        }

        /// <summary>
        /// Get bit.
        /// </summary>
        /// <param name="input">Input.</param>
        /// <param name="offset">Offset.</param>
        /// <returns>The bit.</returns>
        public static bool GetBit(byte input, byte offset)
        {
            Contract.Assert(offset <= 7);

            return (input >> offset) % 2 == 1;
        }

        /// <summary>
        /// Get high bit at.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">offset.</param>
        /// <returns>The high bit.</returns>
        public static bool GetHighBitAt(byte[] buffer, int offset)
        {
            Contract.Assert(offset >= 0 && offset < buffer.Length);
            return ((0x80 & buffer[offset]) == 0x80);
        }

        /// <summary>
        /// Set the high bit at.
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="value">Value.</param>
        public static void SetHighBitAt(byte[] buffer, int offset, bool value)
        {
            Contract.Assert(offset >= 0 && offset < buffer.Length);
            if (value)
            {
                buffer[offset] |= 0x80;
            }
            else
            {
                buffer[offset] &= 0x7F;
            }
        }

        /// <summary>
        /// Get high 3 bits at
        /// </summary>
        /// <param name="buffer">buffer.</param>
        /// <param name="offset">offset.</param>
        /// <returns>High 3 bits.</returns>
        public static int GetHigh3BitsAt(byte[] buffer, int offset)
        {
            Contract.Assert(offset >= 0 && offset < buffer.Length);
            return ((0xE0 & buffer[offset]) >> 5);
        }

        /// <summary>
        /// Set high 3 bits at.
        /// </summary>
        /// <param name="buffer">buffer.</param>
        /// <param name="offset">offset.</param>
        /// <param name="value">value.</param>
        public static void SetHigh3BitsAt(byte[] buffer, int offset, int value)
        {
            Contract.Assert(offset >= 0 && offset < buffer.Length);
            Contract.Assert(value >= 0 && value <= 7);
            byte lower5Bits = (byte)(buffer[offset] & 0x1F);
            byte upper3Bits = (byte)(value << 5);
            buffer[offset] = (byte)(upper3Bits | lower5Bits);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int Get5BitsAt(byte[] buffer, int offset)
        {
            Contract.Assert(offset >= 0 && offset < buffer.Length);
            return (0x1F & buffer[offset]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        public static void Set5BitsAt(byte[] buffer, int offset, int value)
        {
            Contract.Assert(offset >= 0 && offset < buffer.Length);
            Contract.Assert(value >= 0 && value <= 0x1F);
            byte lower5Bits = (byte)(value & 0x1F);
            byte upper3Bits = (byte)(buffer[offset] & 0xE0);
            buffer[offset] = (byte)(upper3Bits | lower5Bits);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int Get8BitsAt(byte[] buffer, int offset)
        {
            Contract.Assert(offset >= 0 && offset < buffer.Length);
            return (0x1F & buffer[offset]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        public static void Set8BitsAt(byte[] buffer, int offset, int value)
        {
            Contract.Assert(offset >= 0 && offset < buffer.Length);
            Contract.Assert(value >= 0 && value <= 0x1F);
            byte lower5Bits = (byte)(value & 0x1F);
            byte upper3Bits = (byte)(buffer[offset] & 0xE0);
            buffer[offset] = (byte)(upper3Bits | lower5Bits);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int Get15BitsAt(byte[] buffer, int offset)
        {
            Contract.Assert(offset >= 0 && offset + 1 < buffer.Length);
            int highByte = (buffer[offset] & 0x7F);
            return (highByte << 8) | buffer[offset + 1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        public static void Set15BitsAt(byte[] buffer, int offset, int value)
        {
            Contract.Assert(offset >= 0 && offset + 1 < buffer.Length);
            Contract.Assert(value >= 0 && value <= 0x7FFF);
            buffer[offset] |= (byte)((value >> 8) & 0x7F);
            buffer[offset + 1] = (byte)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int Get16BitsAt(byte[] buffer, int offset)
        {
            Contract.Assert(offset >= 0 && offset + 1 < buffer.Length);
            return (buffer[offset] << 8) | buffer[offset + 1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        public static void Set16BitsAt(byte[] buffer, int offset, int value)
        {
            Contract.Assert(offset >= 0 && offset + 1 < buffer.Length);
            Contract.Assert(value >= 0 && value <= 0xFFFF);
            buffer[offset] = (byte)(value >> 8);
            buffer[offset + 1] = (byte)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int Get24BitsAt(byte[] buffer, int offset)
        {
            Contract.Assert(offset >= 0 && offset + 2 < buffer.Length);
            return (buffer[offset] << 16) | (buffer[offset + 1] << 8) | buffer[offset + 2];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        public static void Set24BitsAt(byte[] buffer, int offset, int value)
        {
            Contract.Assert(offset >= 0 && offset + 2 < buffer.Length);
            Contract.Assert(value >= 0 && value <= 0xFFFFFF);
            buffer[offset] = (byte)(value >> 16);
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int Get31BitsAt(byte[] buffer, int offset)
        {
            Contract.Assert(offset >= 0 && offset + 3 < buffer.Length);
            int highByte = (buffer[offset] & 0x7F);
            return (highByte << 24)
                | buffer[offset + 1] << 16
                | buffer[offset + 2] << 8
                | buffer[offset + 3];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        public static void Set31BitsAt(byte[] buffer, int offset, int value)
        {
            Contract.Assert(offset >= 0 && offset + 3 < buffer.Length);
            Contract.Assert(value >= 0 && value <= 0x7FFFFF);
            buffer[offset] |= (byte)((value >> 24) & 0x7F);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int Get32BitsAt(byte[] buffer, int offset)
        {
            Contract.Assert(offset >= 0 && offset + 3 < buffer.Length);
            return (buffer[offset] << 24)
                | buffer[offset + 1] << 16
                | buffer[offset + 2] << 8
                | buffer[offset + 3];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        public static void Set32BitsAt(byte[] buffer, int offset, int value)
        {
            Contract.Assert(offset >= 0 && offset + 3 < buffer.Length);
            buffer[offset] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        public static void SetAsciiAt(byte[] buffer, int offset, string value)
        {
            // TODO: The spec really needs to change the header encoding to UTF8
            Contract.Assert(offset >= 0 && offset + value.Length - 1 < buffer.Length);
            Encoding.ASCII.GetBytes(value, 0, value.Length, buffer, offset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static string GetAsciiAt(ArraySegment<byte> segment)
        {
            return Encoding.ASCII.GetString(segment.Array, segment.Offset, segment.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetAsciiAt(byte[] buffer, int offset, int length)
        {
            Contract.Assert(offset >= 0 && offset + length - 1 < buffer.Length);
            return Encoding.ASCII.GetString(buffer, offset, length);
        }

        // +------------------------------------+
        // | Number of Name/Value pairs (int32) |
        // +------------------------------------+
        // |     Length of name (int32)         |
        // +------------------------------------+
        // |           Name (string)            |
        // +------------------------------------+
        // |     Length of value  (int32)       |
        // +------------------------------------+
        // |          Value   (string)          |
        // +------------------------------------+
        // |           (repeats)                |

        /// <summary>
        /// Serialize header block.
        /// </summary>
        /// <param name="pairs">The header clock pair.</param>
        /// <returns>The serialized header block.</returns>
        public static byte[] SerializeHeaderBlock(Dictionary<string, string> pairs)
        {
            int encodedLength = 4 // 32 bit count of name value pairs
                + 8 * pairs.Count; // A 32 bit size per header and value;
            foreach (var key in pairs.Keys)
            {
                encodedLength += key.Length + pairs[key].Length;
            }

            byte[] buffer = new byte[encodedLength];
            Set32BitsAt(buffer, 0, pairs.Count);
            int offset = 4;
            foreach (var key in pairs.Keys)
            {
                Set32BitsAt(buffer, offset, key.Length);
                offset += 4;
                SetAsciiAt(buffer, offset, key);
                offset += key.Length;
                Set32BitsAt(buffer, offset, pairs[key].Length);
                offset += 4;
                SetAsciiAt(buffer, offset, pairs[key]);
                offset += pairs[key].Length;
            }
            return buffer;
        }

        // +------------------------------------+
        // | Number of Name/Value pairs (int32) |
        // +------------------------------------+
        // |     Length of name (int32)         |
        // +------------------------------------+
        // |           Name (string)            |
        // +------------------------------------+
        // |     Length of value  (int32)       |
        // +------------------------------------+
        // |          Value   (string)          |
        // +------------------------------------+
        // |           (repeats)                |

        /// <summary>
        /// Deserialize header block.
        /// </summary>
        /// <param name="rawHeaders">The header block data.</param>
        /// <returns>The header clock pair.</returns>
        public static Dictionary<string, string> DeserializeHeaderBlock(byte[] rawHeaders)
        {
            var headers = new Dictionary<string, string>();

            int offset = 0;
            int headerCount = Get32BitsAt(rawHeaders, offset);
            offset += 4;
            for (int i = 0; i < headerCount; i++)
            {
                int keyLength = Get32BitsAt(rawHeaders, offset);
                Contract.Assert(keyLength > 0);
                offset += 4;
                string key = GetAsciiAt(rawHeaders, offset, keyLength);
                offset += keyLength;
                int valueLength = Get32BitsAt(rawHeaders, offset);
                offset += 4;
                string value = GetAsciiAt(rawHeaders, offset, valueLength);
                offset += valueLength;

                headers.Add(key, value);
            }
            return headers;
        }
    }
}
