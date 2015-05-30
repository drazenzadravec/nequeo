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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.ComponentModel.Composition;

using Nequeo.Cryptography.Key.Crypto.Engines;
using Nequeo.Cryptography.Key.Crypto.Parameters;

namespace Nequeo.Cryptography
{
    /// <summary>
    /// Rijndael cryptography.
    /// </summary>
    public class Rijndael
    {
        /// <summary>
        /// Rijndael cryptography.
        /// </summary>
        public Rijndael() { }

        private const int _numberOfBytes = 1024;
        private const int _validKeyLength = 32;
        private const int _validVectorLength = 16;

        /// <summary>
        /// Encrypted the data.
        /// </summary>
        /// <param name="data">The data to encrypted.</param>
        /// <param name="passphrase">The passphrase key used to mask the data.</param>
        /// <param name="blocksize">The blocksize in bits, must be 128, 192, or 256.</param>
        /// <returns>The encrypted data; else null.</returns>
        /// <remarks>The passphrase must be between 0 and 32 bytes in length.</remarks>
        public byte[] Encrypt(byte[] data, string passphrase, int blocksize = 256)
        {
            // Create the key length.
            byte[] key = GeneratePasswordBytes(passphrase);

            if (!VerifyKeySize(key))
                return null;

            // Create the key parameters.
            Key.Crypto.Parameters.KeyParameter keyParameter = new KeyParameter(key);

            // Initialise the cryptography engine.
            Key.Crypto.Engines.RijndaelEngine rijndael = new RijndaelEngine(blocksize);
            rijndael.Init(true, keyParameter);

            int dataLength = data.Length;
            int blockSize = rijndael.GetBlockSize();
            int modBlockSize = dataLength % blockSize;
            int blockCount = dataLength / blockSize;

            // If there is a remained then add en extra block count.
            if ((modBlockSize) > 0)
            {
                // Add one extra block.
                blockCount++;
            }

            // Encrypted data store.
            byte[] encryptedData = new byte[blockCount * blockSize];
            byte[] decryptedData = new byte[blockCount * blockSize];

            // Copy the decrypted data.
            for (int j = 0; j < dataLength; j++)
            {
                // Assign the data.
                decryptedData[j] = data[j];
            }

            // For each block size in the the data.
            for (int i = 0; i < blockCount; i++)
            {
                // Encrypt the block.
                rijndael.ProcessBlock(decryptedData, (i * blockSize), encryptedData, (i * blockSize));
            }

            // Return the encrypted data.
            return encryptedData;
        }

        /// <summary>
        /// Decrypted the data.
        /// </summary>
        /// <param name="data">The data to decrypted.</param>
        /// <param name="passphrase">The passphrase key used to mask the data.</param>
        /// <param name="blocksize">The blocksize in bits, must be 128, 192, or 256.</param>
        /// <returns>The decrypted data; else null.</returns>
        /// <remarks>The passphrase must be between 0 and 32 bytes in length.</remarks>
        public byte[] Decrypt(byte[] data, string passphrase, int blocksize = 256)
        {
            // Create the key length.
            byte[] key = GeneratePasswordBytes(passphrase);

            if (!VerifyKeySize(key))
                return null;

            // Create the key parameters.
            Key.Crypto.Parameters.KeyParameter keyParameter = new KeyParameter(key);

            // Initialise the cryptography engine.
            Key.Crypto.Engines.RijndaelEngine rijndael = new RijndaelEngine(blocksize);
            rijndael.Init(false, keyParameter);

            int dataLength = data.Length;
            int blockSize = rijndael.GetBlockSize();
            int modBlockSize = dataLength % blockSize;
            int blockCount = dataLength / blockSize;

            // If there is a remained then add en extra block count.
            if ((modBlockSize) > 0)
            {
                // Add one extra block.
                blockCount++;
            }

            // Decrypted data store.
            byte[] encryptedData = new byte[blockCount * blockSize];
            byte[] decryptedData = new byte[blockCount * blockSize];

            // Copy the Encrypted data.
            for (int j = 0; j < dataLength; j++)
            {
                // Assign the data.
                encryptedData[j] = data[j];
            }

            // For each block size in the the data.
            for (int i = 0; i < blockCount; i++)
            {
                // Decrypt the block.
                rijndael.ProcessBlock(encryptedData, (i * blockSize), decryptedData, (i * blockSize));
            }

            // Return the decrypted data.
            return decryptedData;
        }

        /// <summary>
        /// This method will verify that the key is the correct length.
        /// </summary>
        /// <param name="key">The current key for cryptography.</param>
        /// <returns>True if the key is the right length else false.</returns>
        private bool VerifyKeySize(byte[] key)
        {
            int keyLength = key.Length;
            if (keyLength != _validKeyLength)
                return false;
            else
                return true;
        }

        /// <summary>
        /// This method will create a new padded password
        /// with the current password included.
        /// </summary>
        /// <param name="password">The current password.</param>
        /// <returns>The new padded password.</returns>
        private byte[] GeneratePasswordBytes(string password)
        {
            // Get the byte array equivalent 
            // of the current password.
            byte[] currentPassword = new ASCIIEncoding().GetBytes(password);

            // If the current password is greater
            // then the maximum key size then return null.
            if (currentPassword.Length > _validKeyLength)
                return null;

            byte[] passwordKey = new byte[_validKeyLength];

            // For each byte in the array
            // create a new password.
            for (int i = 0; i < _validKeyLength; i++)
            {
                // if the current index is less than the current password
                // length then use the current password at the beginning.
                // else pad the remaining bytes with a valid byte.
                if (i < currentPassword.Length)
                    passwordKey[i] = currentPassword[i];
                else
                    passwordKey[i] = Convert.ToByte(i + 63);    // Pad with '?'.
            }

            // Return the new padded password.
            return passwordKey;
        }
    }
}
