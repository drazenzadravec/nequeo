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
    /// Blowfish cryptography.
    /// </summary>
    public class Blowfish
    {
        /// <summary>
        /// Blowfish cryptography.
        /// </summary>
        public Blowfish() { }

        /// <summary>
        /// Encrypted the data.
        /// </summary>
        /// <param name="data">The data to encrypted.</param>
        /// <param name="passphrase">The passphrase key used to mask the data.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] Encrypt(byte[] data, string passphrase)
        {
            // Create the key parameters.
            byte[] key = Encoding.Default.GetBytes(passphrase);
            Key.Crypto.Parameters.KeyParameter keyParameter = new KeyParameter(key);

            // Initialise the cryptography engine.
            Key.Crypto.Engines.BlowfishEngine blowfish = new BlowfishEngine();
            blowfish.Init(true, keyParameter);

            int dataLength = data.Length;
            int blockSize = blowfish.GetBlockSize();
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
                blowfish.ProcessBlock(decryptedData, (i * blockSize), encryptedData, (i * blockSize));
            }

            // Return the encrypted data.
            return encryptedData;
        }

        /// <summary>
        /// Decrypted the data.
        /// </summary>
        /// <param name="data">The data to decrypted.</param>
        /// <param name="passphrase">The passphrase key used to mask the data.</param>
        /// <returns>The decrypted data.</returns>
        public byte[] Decrypt(byte[] data, string passphrase)
        {
            // Create the key parameters.
            byte[] key = Encoding.Default.GetBytes(passphrase);
            Key.Crypto.Parameters.KeyParameter keyParameter = new KeyParameter(key);

            // Initialise the cryptography engine.
            Key.Crypto.Engines.BlowfishEngine blowfish = new BlowfishEngine();
            blowfish.Init(false, keyParameter);

            int dataLength = data.Length;
            int blockSize = blowfish.GetBlockSize();
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
                blowfish.ProcessBlock(encryptedData, (i * blockSize), decryptedData, (i * blockSize));
            }

            // Return the decrypted data.
            return decryptedData;
        }
    }
}
