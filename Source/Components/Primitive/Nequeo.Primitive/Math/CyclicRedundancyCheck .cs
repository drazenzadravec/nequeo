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
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Nequeo.Math
{
    /// <summary>
    /// Cyclic redundancy check polynomial (reversed).
    /// </summary>
    public enum CrcPolynomial_16 : int
    {
        /// <summary>
        /// USB token packets.
        /// </summary>
        CRC_5_USB = 0x14,
        /// <summary>
        /// CRC_8.
        /// </summary>
        CRC_8 = 0xAB,
        /// <summary>
        /// I.432.1; ATM HEC, ISDN HEC and cell delineation.
        /// </summary>
        CRC_8_CCITT = 0xE0,
        /// <summary>
        ///  AES3.
        /// </summary>
        CRC_8_SAE_J1850 = 0xB8,
        /// <summary>
        /// Mobile networks.
        /// </summary>
        CRC_8_WCDMA = 0xD9,
        /// <summary>
        /// CRC-16, CRC-16-ANSI, Modbus.
        /// </summary>
        CRC_16_Modbus = 0xA001,
        /// <summary>
        /// XMODEM, Bluetooth, PACTOR, SD.
        /// </summary>
        CRC_16_CCITT = 0x8408,
        /// <summary>
        /// Mobile networks.
        /// </summary>
        CRC_16_CDMA2000 = 0xE613,
        /// <summary>
        ///  NP, IEC 870, M-Bus.
        /// </summary>
        CRC_16_DNP = 0xA6BC,
    }

    /// <summary>
    /// Cyclic redundancy check polynomial (reversed).
    /// </summary>
    public enum CrcPolynomial_32 : long
    {
        /// <summary>
        ///  HDLC, ANSI X3.66, ITU-T V.42, Ethernet, Serial ATA, MPEG-2, PKZIP, Gzip, Bzip2, PNG.
        /// </summary>
        CRC_32 = 0xEDB88320L,
    }

    /// <summary>
    /// Cyclic redundancy check provider.
    /// </summary>
    public class CyclicRedundancyCheck
    {
        /// <summary>
        /// Cyclic redundancy check provider.
        /// </summary>
        public CyclicRedundancyCheck() { }

        /// <summary>
        /// Calculate the checksum.
        /// </summary>
        /// <param name="input">The data to check.</param>
        /// <param name="crcPolynomial">Cyclic redundancy check polynomial (reversed).</param>
        /// <param name="isInputHexData">Is the input data in hex format.</param>
        /// <returns>The checksum (remainder).</returns>
        public string Calculate(string input, int crcPolynomial = 0xA001, bool isInputHexData = false)
        {
            if (isInputHexData)
            {
                int NumberChars = input.Length;
                byte[] bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(input.Substring(i, 2), 16);

                return Calculate(bytes, crcPolynomial);
            }
            else
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(input);
                int[] data = new int[array.Length];
                for (int i = 0; i < array.Length; i++)
                    data[i] = array[i];

                return Calculate(data, crcPolynomial);
            }
        }

        /// <summary>
        /// Calculate the checksum.
        /// </summary>
        /// <param name="input">The data to check.</param>
        /// <param name="crcPolynomial">Cyclic redundancy check polynomial (reversed).</param>
        /// <returns>The checksum (remainder).</returns>
        public string Calculate(byte[] input, int crcPolynomial = 0xA001)
        {
            int[] data = new int[input.Length];
            for (int i = 0; i < input.Length; i++)
                data[i] = input[i];

            return Calculate(data, crcPolynomial);
        }

        /// <summary>
        /// Calculate the checksum.
        /// </summary>
        /// <param name="input">The data to check.</param>
        /// <param name="crcPolynomial">Cyclic redundancy check polynomial (reversed).</param>
        /// <returns>The checksum (remainder).</returns>
        public string Calculate(int[] input, int crcPolynomial = 0xA001)
        {
            int[] int_array = input;
            int int_crc = 0xFFFF;
            long int_lsb;

            // For each inpit array.
            for (int int_i = 0; int_i < int_array.Length; int_i++)
            {
                // For each bit.
                int_crc = int_crc ^ int_array[int_i];
                for (int int_j = 0; int_j < 8; int_j++)
                {
                    int_lsb = int_crc & 0x0001;  // Mask of LSB
                    int_crc = int_crc >> 1;
                    int_crc = int_crc & 0x7FFF;
                    if (int_lsb == 1) int_crc = int_crc ^ crcPolynomial;
                }
            }

            // Get the LSB and MSB.
            int int_crc_byte_a = int_crc & 0x00FF;
            int int_crc_byte_b = (int_crc >> 8) & 0x00FF;

            // Return the check sum.
            return int_crc_byte_b.ToString("X2") + int_crc_byte_a.ToString("X2");
        }

        /// <summary>
        /// Calculate the checksum.
        /// </summary>
        /// <param name="input">The data to check.</param>
        /// <param name="crcPolynomial">Cyclic redundancy check polynomial (reversed).</param>
        /// <param name="isInputHexData">Is the input data in hex format.</param>
        /// <returns>The checksum (remainder).</returns>
        public string Calculate(string input, long crcPolynomial = 0xEDB88320L, bool isInputHexData = false)
        {
            if (isInputHexData)
            {
                int NumberChars = input.Length;
                byte[] bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(input.Substring(i, 2), 16);

                return Calculate(bytes, crcPolynomial);
            }
            else
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(input);
                long[] data = new long[array.Length];
                for (long i = 0; i < array.Length; i++)
                    data[i] = array[i];

                return Calculate(data, crcPolynomial);
            }
        }

        /// <summary>
        /// Calculate the checksum.
        /// </summary>
        /// <param name="input">The data to check.</param>
        /// <param name="crcPolynomial">Cyclic redundancy check polynomial (reversed).</param>
        /// <returns>The checksum (remainder).</returns>
        public string Calculate(byte[] input, long crcPolynomial = 0xEDB88320L)
        {
            long[] data = new long[input.Length];
            for (long i = 0; i < input.Length; i++)
                data[i] = input[i];

            return Calculate(data, crcPolynomial);
        }

        /// <summary>
        /// Calculate the checksum.
        /// </summary>
        /// <param name="input">The data to check.</param>
        /// <param name="crcPolynomial">Cyclic redundancy check polynomial (reversed).</param>
        /// <returns>The checksum (remainder).</returns>
        public string Calculate(long[] input, long crcPolynomial = 0xEDB88320L)
        {
            bool intiTable = false;
            long int_crc = 0xFFFFFFFFL;
            long[] crc_tab32 = new long[256];

            for (int i = 0; i < input.Length; i++)
            {
                if (!intiTable)
                {
                    long long_crc;

                    for (int j = 0; j < 256; j++)
                    {
                        long_crc = (long)j;
                        long int_lsb;

                        for (int k = 0; k < 8; k++)
                        {
                            int_lsb = long_crc & 0x00000001L;

                            if (int_lsb == 1)
                                long_crc = (long_crc >> 1) ^ crcPolynomial;
                            else
                                long_crc = long_crc >> 1;
                        }

                        crc_tab32[j] = long_crc;
                    }
                    intiTable = true;
                }

                long long_c = 0x000000FFL & input[i];
                long tmp = int_crc ^ long_c;
                int_crc = (int_crc >> 8) ^ crc_tab32[tmp & 0xFFL];
            }

            int_crc ^= 0xffffffffL;

            // Get the significant bits.
            long int_crc_byte_a = int_crc & 0x00FF;
            long int_crc_byte_b = (int_crc >> 8) & 0x00FF;
            long int_crc_byte_c = (int_crc >> 16) & 0x00FF;
            long int_crc_byte_d = (int_crc >> 24) & 0x00FF;

            // Return the check sum.
            return int_crc.ToString("X4");
        }
    }
}
