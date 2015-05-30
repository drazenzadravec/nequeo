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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Nequeo.Extension
{
    /// <summary>
    /// Class that extends the System.Int32 type.
    /// </summary>
    public static class Int32Extensions
    {
        private const byte Divider = 128;

        /// <summary>
        /// Get the multiplication result.
        /// </summary>
        /// <param name="intValue">The current integer value.</param>
        /// <param name="multiplyBy">The multiplication value.</param>
        /// <returns>The new multiplied value.</returns>
        public static int Multiplication(this Int32 intValue, Int32 multiplyBy)
        {
            // Return the new value.
            return (intValue * multiplyBy);
        }

        /// <summary>
        /// Get the division result.
        /// </summary>
        /// <param name="intValue">The current integer value.</param>
        /// <param name="divideBy">The division value.</param>
        /// <returns>The new divided value.</returns>
        public static int Division(this Int32 intValue, Int32 divideBy)
        {
            // Can not divide by zero.
            if (divideBy == 0)
                throw new System.DivideByZeroException();

            // Return the new value.
            return (intValue / divideBy);
        }

        /// <summary>
        /// To unsigned int byte array.
        /// </summary>
        /// <param name="intValue">The current integer value.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>The bytes array.</returns>
        public static byte[] ToUVarInt(this Int32 intValue, byte prefix)
        {
            Contract.Assert(prefix <= 7);
            int prefixMaxValue = (1 << prefix) - 1;

            if (intValue < prefixMaxValue)
            {
                return new[] { (byte)intValue };
            }

            using (var binaryStream = new MemoryStream())
            {
                int integralPart = 1;
                intValue -= prefixMaxValue;

                binaryStream.WriteByte((byte)prefixMaxValue);

                while (integralPart > 0)
                {
                    integralPart = intValue / Divider;
                    byte fractionalPart = (byte)(intValue % Divider);

                    if (integralPart > 0)
                    {
                        //Set to one highest bit
                        fractionalPart |= 0x80;
                    }

                    binaryStream.WriteByte(fractionalPart);

                    intValue = integralPart;
                }

                var result = new byte[binaryStream.Position];
                Buffer.BlockCopy(binaryStream.GetBuffer(), 0, result, 0, result.Length);
                return result;
            }
        }

        /// <summary>
        /// From unsigned byte array.
        /// </summary>
        /// <param name="binary">The byte array.</param>
        /// <returns>The integer.</returns>
        public static Int32 FromUVarInt(byte[] binary)
        {
            int currentIntegral = 0;

            for (int i = binary.Length - 1; i >= 1; i--)
            {
                //Zero highest bit
                byte fractional = (byte)(binary[i] & 0x7f);
                currentIntegral *= Divider;
                currentIntegral += fractional;
            }

            return currentIntegral + binary[0];
        }
    }
}
