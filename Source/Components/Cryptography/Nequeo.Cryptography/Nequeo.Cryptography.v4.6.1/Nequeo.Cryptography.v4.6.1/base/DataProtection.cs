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
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.Composition;

namespace Nequeo.Cryptography
{
    /// <summary>
    /// Data protection provides methods for encrypting and decrypting data.
    /// </summary>
    public sealed class DataProtection
    {
        /// <summary>
        /// Data protection provides methods for encrypting and decrypting data.
        /// </summary>
        public DataProtection() { }

        /// <summary>
        /// Additional entropy when using protect method.
        /// </summary>
        private byte[] _entropy = new byte[] { 
            23, 84, 95, 34, 210, 219, 167, 87, 
            133, 114, 67, 34, 88, 235, 240, 77, 
            25, 79, 67, 123, 147, 156, 167, 250,
            21, 39, 72, 125, 149, 214, 205, 54};

        /// <summary>
        /// Encrypts the data in a specified byte array and returns a byte array that contains the encrypted data.
        /// </summary>
        /// <param name="data">A byte array that contains data to encrypt.</param>
        /// <param name="scope">One of the enumeration values that specifies the scope of encryption.</param>
        /// <returns>A byte array representing the encrypted data.</returns>
        public byte[] Protect(byte[] data, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            return ProtectedData.Protect(data, _entropy, scope);
        }

        /// <summary>
        /// Encrypts the data in a specified byte array and returns a byte array that contains the encrypted data.
        /// </summary>
        /// <param name="data">A byte array that contains data to encrypt.</param>
        /// <param name="entropy">Used to increase the complexity of the encryption, or null for no additional complexity.</param>
        /// <param name="scope">One of the enumeration values that specifies the scope of encryption.</param>
        /// <returns>A byte array representing the encrypted data.</returns>
        public byte[] Protect(byte[] data, byte[] entropy, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            return ProtectedData.Protect(data, entropy, scope);
        }

        /// <summary>
        /// Decrypts the data in a specified byte array and returns a byte array that contains the decrypted data.
        /// </summary>
        /// <param name="data">A byte array containing the encrypted data.</param>
        /// <param name="scope">One of the enumeration values that specifies the scope of data protection that was used to encrypt the data.</param>
        /// <returns>A byte array representing the decrypted data.</returns>
        public byte[] Unprotect(byte[] data, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            return ProtectedData.Unprotect(data, _entropy, scope);
        }

        /// <summary>
        /// Decrypts the data in a specified byte array and returns a byte array that contains the decrypted data.
        /// </summary>
        /// <param name="data">A byte array containing the encrypted data.</param>
        /// <param name="entropy">A byte array that was used to encrypt the data, or null if the additional byte array was not used.</param>
        /// <param name="scope">One of the enumeration values that specifies the scope of data protection that was used to encrypt the data.</param>
        /// <returns>A byte array representing the decrypted data.</returns>
        public byte[] Unprotect(byte[] data, byte[] entropy, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            return ProtectedData.Unprotect(data, entropy, scope);
        }

        /// <summary>
        /// Convert the data array to hex values.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>The hex array.</returns>
        public byte[] ConvertToHex(byte [] data)
        {
            return Nequeo.Custom.HexEncoder.Encode(data);
        }

        /// <summary>
        /// Convert the hex data array to the original values.
        /// </summary>
        /// <param name="data">The hex byte array.</param>
        /// <returns>The original values.</returns>
        public byte[] ConvertFromHex(byte[] data)
        {
            return Nequeo.Custom.HexEncoder.Decode(data);
        }
    }

    /// <summary>
    /// Memory protection protecting and unprotecting memory.
    /// </summary>
    public sealed class MemoryProtection
    {
        /// <summary>
        /// Memory protection protecting and unprotecting memory.
        /// </summary>
        public MemoryProtection() { }

        /// <summary>
        /// Protects the specified data.
        /// </summary>
        /// <param name="data">The byte array containing data in memory to protect. The array must be a multiple of 16 bytes.</param>
        /// <param name="scope">One of the enumeration values that specifies the scope of memory protection.</param>
        /// <returns>A byte array representing the encrypted data.</returns>
        public byte[] Protect(byte[] data, MemoryProtectionScope scope = MemoryProtectionScope.SameProcess)
        {
            ProtectedMemory.Protect(data, scope);
            return data;
        }

        /// <summary>
        /// Unprotects data in memory that was protected.
        /// </summary>
        /// <param name="data">The byte array in memory to unencrypt.</param>
        /// <param name="scope">One of the enumeration values that specifies the scope of memory protection.</param>
        /// <returns>A byte array representing the decrypted data.</returns>
        public byte[] Unprotect(byte[] data, MemoryProtectionScope scope = MemoryProtectionScope.SameProcess)
        {
            ProtectedMemory.Unprotect(data, scope);
            return data;
        }

        /// <summary>
        /// Convert the data array to hex values.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>The hex array.</returns>
        public byte[] ConvertToHex(byte[] data)
        {
            return Nequeo.Custom.HexEncoder.Encode(data);
        }

        /// <summary>
        /// Convert the hex data array to the original values.
        /// </summary>
        /// <param name="data">The hex byte array.</param>
        /// <returns>The original values.</returns>
        public byte[] ConvertFromHex(byte[] data)
        {
            return Nequeo.Custom.HexEncoder.Decode(data);
        }
    }
}
