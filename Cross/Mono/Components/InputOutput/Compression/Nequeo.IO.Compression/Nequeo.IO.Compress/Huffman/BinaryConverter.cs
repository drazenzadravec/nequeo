/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.IO.Compression.Huffman
{
    /// <summary>
    /// Binary converter.
    /// </summary>
    internal class BinaryConverter
    {
        /// <summary>
        /// Convert to bits.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>THe bits.</returns>
        public static bool[] ToBits(byte[] bytes)
        {
            var result = new bool[bytes.Length * 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                for (byte j = 0; j < 8; j++)
                {
                    result[i * 8 + j] = GetBit(bytes[i], (byte)(7 - j));
                }
            }

            return result;
        }

        /// <summary>
        /// Convert to bytes.
        /// </summary>
        /// <param name="bools">The bits.</param>
        /// <returns>The bytes.</returns>
        public static byte[] ToBytes(List<bool> bools)
        {
            var result = new byte[bools.Count % 8 == 0 ? bools.Count / 8 : bools.Count / 8 + 1];
            int offset = 0;
            byte count = 8;
            int resIndex = 0;

            while (count != 0)
            {
                result[resIndex++] = GetByte(bools, offset, count);
                offset += count;
                int roffset = bools.Count - offset;
                count = roffset >= 8 ? (byte)8 : (byte)roffset;
            }

            return result;
        }

        /// <summary>
        /// Get byte.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>The byte.</returns>
        private static byte GetByte(List<bool> bits, int offset, byte count)
        {
            if (count == 0)
                throw new ArgumentException("count is 0");
            if (count > 8)
                throw new ArgumentException("byte is 8 bits");

            byte result = 0;
            int endIndex = offset + count;
            byte bitIndex = 7;
            for (int i = offset; i < endIndex; i++, bitIndex--)
            {
                if (bits[i])
                    result |= (byte)(1 << bitIndex);
            }

            return result;
        }

        /// <summary>
        /// Get bit.
        /// </summary>
        /// <param name="b">The byte.</param>
        /// <param name="pos">The position.</param>
        /// <returns>The bit.</returns>
        private static bool GetBit(byte b, byte pos)
        {
            if (pos > 7)
                throw new ArgumentOutOfRangeException("pos > 7");

            byte mask = (byte)(1 << pos);

            byte masked = (byte)(b & mask);

            return masked != 0;
        }
    }
}
