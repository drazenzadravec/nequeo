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
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.ComponentModel.Composition;

namespace Nequeo.Cryptography
{
    #region Public Delegate
    /// <summary>
    /// This delegate handles the transport of data from sender
    /// to receiver when advanced AES encryption process occurs.
    /// </summary>
    /// <param name="sender">The current object sending the message</param>
    /// <param name="e">The object containing the event information.</param>
    public delegate void AdvancedAESEncryptionHandler(object sender, AdvancedAESEncryptionArgs e);

    /// <summary>
    /// This delegate handles the transport of data from sender
    /// to receiver when advanced AES decryption process occurs.
    /// </summary>
    /// <param name="sender">The current object sending the message</param>
    /// <param name="e">The object containing the event information.</param>
    public delegate void AdvancedAESDecryptionHandler(object sender, AdvancedAESDecryptionArgs e);
    #endregion

    /// <summary>
    /// Advanced encryption decryption of data class.
    /// </summary>
    [Export(typeof(IAdvancedAES))]
    public partial class AdvancedAES : Nequeo.Runtime.DisposableBase, IDisposable, IAdvancedAES
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly AdvancedAES Instance = new AdvancedAES();

        /// <summary>
        /// Static constructor
        /// </summary>
        static AdvancedAES() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public AdvancedAES()
        {
            OnCreated();
        }
        #endregion

        #region Private Constants
        private const int numberOfBytes = 1024;
        private const int validKeyLength = 32;
        private const int validVectorLength = 16;
        #endregion

        #region Private Fields
        // The cryptography key.
        private byte[] internalKey = new byte[] { 
            23, 80, 90, 34, 200, 215, 167, 97, 
            132, 104, 67, 34, 99, 235, 240, 57, 
            25, 79, 67, 113, 147, 156, 167, 251,
            20, 39, 69, 125, 149, 214, 202, 53};
        // The initializations vector.
        private byte[] internalIV = new byte[] { 
            18, 43, 63, 126, 169, 214, 232, 47,
            19, 41, 70, 127, 159, 224, 222, 58};

        private CipherMode _cipherMode = CipherMode.CFB;
        private PaddingMode _padding = PaddingMode.Zeros;
        private int _blockSize = 128;
        private int _keySize = 256;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the cipher mode.
        /// </summary>
        public CipherMode CipherMode
        {
            get { return _cipherMode; }
            set { _cipherMode = value; }
        }

        /// <summary>
        /// Gets or sets the padding mode.
        /// </summary>
        public PaddingMode PaddingMode
        {
            get { return _padding; }
            set { _padding = value; }
        }

        /// <summary>
        /// Gets or sets the block size.
        /// </summary>
        public int BlockSize
        {
            get { return _blockSize; }
            set { _blockSize = value; }
        }

        /// <summary>
        /// Gets or sets the key size.
        /// </summary>
        public int KeySize
        {
            get { return _keySize; }
            set { _keySize = value; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// This event occurs when an error occurs 
        /// attempting to encrypt data.
        /// </summary>
        public event AdvancedAESEncryptionHandler OnAdvancedAESEncryptionError;

        /// <summary>
        /// This event occurs when an error occurs 
        /// attempting to decrypt data.
        /// </summary>
        public event AdvancedAESDecryptionHandler OnAdvancedAESDecryptionError;
        #endregion

        #region Public Methods
        /// <summary>
        /// This method will encrypt a decrypted.
        /// </summary>
        /// <param name="decryptedData">The decrypted data.</param>
        /// <returns>The array of encrypted data.</returns>
        public virtual byte[] EncryptToMemory(byte[] decryptedData)
        {
            return EncryptToMemoryEx(decryptedData, this.internalKey, this.internalIV);
        }

        /// <summary>
        /// This method will encrypt a decrypted.
        /// </summary>
        /// <param name="decryptedData">Thedecrypted data.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>The array of encrypted data.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        public virtual byte[] EncryptToMemory(byte[] decryptedData, byte[] Key)
        {
            if (VerifyKeySize(Key))
                return EncryptToMemoryEx(decryptedData, Key, this.internalIV);
            else
                return null;
        }

        /// <summary>
        /// This method will encrypt a decrypted.
        /// </summary>
        /// <param name="decryptedData">The decrypted data.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>The array of encrypted data.</returns>
        /// <remarks>The password must be between 0 and 32 bytes in length.</remarks>
        public virtual byte[] EncryptToMemory(byte[] decryptedData, string password)
        {
            byte[] passwordKey = GeneratePasswordBytes(password);

            if (passwordKey != null)
            {
                if (VerifyKeySize(passwordKey))
                    return EncryptToMemoryEx(decryptedData, passwordKey, this.internalIV);
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// This method will encrypt a decrypted.
        /// </summary>
        /// <param name="decryptedData">The decrypted data.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The array of encrypted data.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        public virtual byte[] EncryptToMemory(byte[] decryptedData, byte[] Key, byte[] IV)
        {
            if (VerifyKeySize(Key) && VerifyVectorSize(IV))
                return EncryptToMemoryEx(decryptedData, Key, IV);
            else
                return null;
        }

        /// <summary>
        /// This method will encrypt a decrypted stream.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <returns>The array of encrypted data.</returns>
        public virtual byte[] EncryptStream(Stream decryptedData)
        {
            return EncryptStreamEx(decryptedData, this.internalKey, this.internalIV);
        }

        /// <summary>
        /// This method will encrypt a decrypted stream.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>The array of encrypted data.</returns>
        public virtual byte[] EncryptStream(Stream decryptedData, byte[] Key)
        {
            if (VerifyKeySize(Key))
                return EncryptStreamEx(decryptedData, Key, this.internalIV);
            else
                return null;
        }

        /// <summary>
        /// This method will encrypt a decrypted stream.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>The array of encrypted data.</returns>
        public virtual byte[] EncryptStream(Stream decryptedData, string password)
        {
            byte[] passwordKey = GeneratePasswordBytes(password);

            if (passwordKey != null)
            {
                if (VerifyKeySize(passwordKey))
                    return EncryptStreamEx(decryptedData, passwordKey, this.internalIV);
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// This method will encrypt a decrypted stream.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The array of encrypted data.</returns>
        public virtual byte[] EncryptStream(Stream decryptedData, byte[] Key, byte[] IV)
        {
            if (VerifyKeySize(Key) && VerifyVectorSize(IV))
                return EncryptStreamEx(decryptedData, Key, IV);
            else
                return null;
        }

        /// <summary>
        /// This method will decrypt an encrypted stream.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <returns>The decrypted data.</returns>
        public virtual byte[] DecryptStream(Stream encryptedData)
        {
            return DecryptStreamEx(encryptedData, this.internalKey, this.internalIV);
        }

        /// <summary>
        /// This method will decrypt an encrypted stream.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>The decrypted data.</returns>
        public virtual byte[] DecryptStream(Stream encryptedData, byte[] Key)
        {
            if (VerifyKeySize(Key))
                return DecryptStreamEx(encryptedData, Key, this.internalIV);
            else
                return null;
        }

        /// <summary>
        /// This method will decrypt an encrypted stream.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>The decrypted data.</returns>
        public virtual byte[] DecryptStream(Stream encryptedData, string password)
        {
            byte[] passwordKey = GeneratePasswordBytes(password);

            if (passwordKey != null)
            {
                if (VerifyKeySize(passwordKey))
                    return DecryptStreamEx(encryptedData, passwordKey, this.internalIV);
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// This method will decrypt an encrypted stream.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The decrypted data.</returns>
        public virtual byte[] DecryptStream(Stream encryptedData, byte[] Key, byte[] IV)
        {
            if (VerifyKeySize(Key) && VerifyVectorSize(IV))
                return DecryptStreamEx(encryptedData, Key, IV);
            else
                return null;
        }

        /// <summary>
        /// This method will decrypt an encrypted byte array.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <returns>The decrypted data.</returns>
        public virtual byte[] DecryptFromMemory(byte[] encryptedData)
        {
            return DecryptFromMemoryEx(encryptedData, this.internalKey, this.internalIV);
        }

        /// <summary>
        /// This method will decrypt an encrypted byte array.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>The decrypted data.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        public virtual byte[] DecryptFromMemory(byte[] encryptedData, byte[] Key)
        {
            if (VerifyKeySize(Key))
                return DecryptFromMemoryEx(encryptedData, Key, this.internalIV);
            else
                return null;
        }

        /// <summary>
        /// This method will decrypt an encrypted byte array.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>The decrypted data.</returns>
        /// <remarks>The password must be between 0 and 32 bytes in length.</remarks>
        public virtual byte[] DecryptFromMemory(byte[] encryptedData, string password)
        {
            byte[] passwordKey = GeneratePasswordBytes(password);

            if (passwordKey != null)
            {
                if (VerifyKeySize(passwordKey))
                    return DecryptFromMemoryEx(encryptedData, passwordKey, this.internalIV);
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// This method will decrypt an encrypted byte array.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The decrypted data.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        public virtual byte[] DecryptFromMemory(byte[] encryptedData, byte[] Key, byte[] IV)
        {
            if (VerifyKeySize(Key) && VerifyVectorSize(IV))
                return DecryptFromMemoryEx(encryptedData, Key, IV);
            else
                return null;
        }

        /// <summary>
        /// This method will encrypt a file to another file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <returns>True if no error has occurred else false.</returns>
        public virtual bool EncryptFile(string pathToDecryptedFile, string pathToEncryptedFile)
        {
            return EncryptFileEx(pathToDecryptedFile, pathToEncryptedFile, this.internalKey, this.internalIV);
        }

        /// <summary>
        /// This method will encrypt a file to another file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        public virtual bool EncryptFile(string pathToDecryptedFile, string pathToEncryptedFile, byte[] Key)
        {
            if (VerifyKeySize(Key))
                return EncryptFileEx(pathToDecryptedFile, pathToEncryptedFile, Key, this.internalIV);
            else
                return false;
        }

        /// <summary>
        /// This method will encrypt a file to another file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The password must be between 0 and 32 bytes in length.</remarks>
        public virtual bool EncryptFile(string pathToDecryptedFile, string pathToEncryptedFile, string password)
        {
            byte[] passwordKey = GeneratePasswordBytes(password);

            if (passwordKey != null)
            {
                if (VerifyKeySize(passwordKey))
                    return EncryptFileEx(pathToDecryptedFile, pathToEncryptedFile, passwordKey, this.internalIV);
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// This method will encrypt a file to another file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        public virtual bool EncryptFile(string pathToDecryptedFile, string pathToEncryptedFile, byte[] Key, byte[] IV)
        {
            if (VerifyKeySize(Key) && VerifyVectorSize(IV))
                return EncryptFileEx(pathToDecryptedFile, pathToEncryptedFile, Key, IV);
            else
                return false;
        }

        /// <summary>
        /// This method will decrypt a file from an encrypted file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <returns>True if no error has occurred else false.</returns>
        public virtual bool DecryptFile(string pathToDecryptedFile, string pathToEncryptedFile)
        {
            return DecryptFileEx(pathToDecryptedFile, pathToEncryptedFile, this.internalKey, this.internalIV);
        }

        /// <summary>
        /// This method will decrypt a file from an encrypted file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        public virtual bool DecryptFile(string pathToDecryptedFile, string pathToEncryptedFile, byte[] Key)
        {
            if (VerifyKeySize(Key))
                return DecryptFileEx(pathToDecryptedFile, pathToEncryptedFile, Key, this.internalIV);
            else
                return false;
        }

        /// <summary>
        /// This method will decrypt a file from an encrypted file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The password must be between 0 and 32 bytes in length.</remarks>
        public virtual bool DecryptFile(string pathToDecryptedFile, string pathToEncryptedFile, string password)
        {
            byte[] passwordKey = GeneratePasswordBytes(password);

            if (passwordKey != null)
            {
                if (VerifyKeySize(passwordKey))
                    return DecryptFileEx(pathToDecryptedFile, pathToEncryptedFile, passwordKey, this.internalIV);
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// This method will decrypt a file from an encrypted file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        public virtual bool DecryptFile(string pathToDecryptedFile, string pathToEncryptedFile, byte[] Key, byte[] IV)
        {
            if (VerifyKeySize(Key) && VerifyVectorSize(IV))
                return DecryptFileEx(pathToDecryptedFile, pathToEncryptedFile, Key, IV);
            else
                return false;
        }

        /// <summary>
        /// This method will write the data to an encrypted file.
        /// </summary>
        /// <param name="decryptedData">The list of string data to encrypt.</param>
        /// <param name="pathToEncryptedFile">The file where encrypted data is to be placed.</param>
        /// <returns>True if the data was written to the file else false.</returns>
        public virtual bool EncryptToFile(List<string> decryptedData, string pathToEncryptedFile)
        {
            return EncryptToFileEx(decryptedData, pathToEncryptedFile, this.internalKey, this.internalIV);
        }

        /// <summary>
        /// This method will write the data to an encrypted file.
        /// </summary>
        /// <param name="decryptedData">The list of string data to encrypt.</param>
        /// <param name="pathToEncryptedFile">The file where encrypted data is to be placed.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>True if the data was written to the file else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        public virtual bool EncryptToFile(List<string> decryptedData, string pathToEncryptedFile, byte[] Key)
        {
            if (VerifyKeySize(Key))
                return EncryptToFileEx(decryptedData, pathToEncryptedFile, Key, this.internalIV);
            else
                return false;
        }

        /// <summary>
        /// This method will write the data to an encrypted file.
        /// </summary>
        /// <param name="decryptedData">The list of string data to encrypt.</param>
        /// <param name="pathToEncryptedFile">The file where encrypted data is to be placed.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>True if the data was written to the file else false.</returns>
        /// <remarks>The password must be between 0 and 32 bytes in length.</remarks>
        public virtual bool EncryptToFile(List<string> decryptedData, string pathToEncryptedFile, string password)
        {
            byte[] passwordKey = GeneratePasswordBytes(password);

            if (passwordKey != null)
            {
                if (VerifyKeySize(passwordKey))
                    return EncryptToFileEx(decryptedData, pathToEncryptedFile, passwordKey, this.internalIV);
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// This method will write the data to an encrypted file.
        /// </summary>
        /// <param name="decryptedData">The list of string data to encrypt.</param>
        /// <param name="pathToEncryptedFile">The file where encrypted data is to be placed.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>True if the data was written to the file else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        public virtual bool EncryptToFile(List<string> decryptedData, string pathToEncryptedFile, byte[] Key, byte[] IV)
        {
            if (VerifyKeySize(Key) && VerifyVectorSize(IV))
                return EncryptToFileEx(decryptedData, pathToEncryptedFile, Key, IV);
            else
                return false;
        }

        /// <summary>
        /// This method will decrypt data from an encrypted file.
        /// </summary>
        /// <param name="pathToEncryptedFile">The file where encrypted data is loaded from.</param>
        /// <returns>The list of string data decrypted.</returns>
        public virtual List<string> DecryptFromFile(string pathToEncryptedFile)
        {
            return DecryptFromFileEx(pathToEncryptedFile, this.internalKey, this.internalIV);
        }

        /// <summary>
        /// This method will decrypt data from an encrypted file.
        /// </summary>
        /// <param name="pathToEncryptedFile">The file where encrypted data is loaded from.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>The list of string data decrypted.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        public virtual List<string> DecryptFromFile(string pathToEncryptedFile, byte[] Key)
        {
            if (VerifyKeySize(Key))
                return DecryptFromFileEx(pathToEncryptedFile, Key, this.internalIV);
            else
                return null;
        }

        /// <summary>
        /// This method will decrypt data from an encrypted file.
        /// </summary>
        /// <param name="pathToEncryptedFile">The file where encrypted data is loaded from.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>The list of string data decrypted.</returns>
        /// <remarks>The password must be between 0 and 32 bytes in length.</remarks>
        public virtual List<string> DecryptFromFile(string pathToEncryptedFile, string password)
        {
            byte[] passwordKey = GeneratePasswordBytes(password);

            if (passwordKey != null)
            {
                if (VerifyKeySize(passwordKey))
                    return DecryptFromFileEx(pathToEncryptedFile, passwordKey, this.internalIV);
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// This method will decrypt data from an encrypted file.
        /// </summary>
        /// <param name="pathToEncryptedFile">The file where encrypted data is loaded from.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The list of string data decrypted.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        public virtual List<string> DecryptFromFile(string pathToEncryptedFile, byte[] Key, byte[] IV)
        {
            if (VerifyKeySize(Key) && VerifyVectorSize(IV))
                return DecryptFromFileEx(pathToEncryptedFile, Key, IV);
            else
                return null;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method will verify that the key is the correct length.
        /// </summary>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>True if the key is the right length else false.</returns>
        private bool VerifyKeySize(byte[] Key)
        {
            int keyLength = Key.Length;
            if (keyLength != validKeyLength)
                return false;
            else
                return true;
        }

        /// <summary>
        /// This method will verify that the vector is the correct length.
        /// </summary>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>True if the vector is the right length else false.</returns>
        private bool VerifyVectorSize(byte[] IV)
        {
            int vectorLength = IV.Length;
            if (vectorLength != validVectorLength)
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
            if (currentPassword.Length > validKeyLength)
                return null;

            byte[] passwordKey = new byte[validKeyLength];

            // For each byte in the array
            // create a new password.
            for (int i = 0; i < validKeyLength; i++)
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

        /// <summary>
        /// This method will encrypt a decrypted string.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The array of encrypted data.</returns>
        private byte[] EncryptToMemoryEx(byte[] decryptedData, byte[] Key, byte[] IV)
        {
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;
            
            try
            {
                // Will contain the encrypted data
                // from the memory stream.
                byte[] memEncryptedData = null;

                // Convert the passed string to a byte array.
                byte[] encryptedData = decryptedData;

                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                using (memoryStream = new MemoryStream())
                {
                    // Create a new AES provider.
                    AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
                    provider.Mode = _cipherMode;
                    provider.Padding = _padding;
                    provider.BlockSize = _blockSize;
                    provider.KeySize = _keySize;

                    // Create a CryptoStream using the MemoryStream 
                    // and the passed key and initialization vector (IV).
                    using (cryptoStream = new CryptoStream(memoryStream, provider.CreateEncryptor(Key, IV),
                        CryptoStreamMode.Write))
                    {
                        // Write the byte array to the 
                        // crypto stream and flush it.
                        cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                        cryptoStream.FlushFinalBlock();

                        // Close the CryptoStream object 
                        // release resources.
                        cryptoStream.Close();
                    }

                    // Get an array of bytes from the MemoryStream 
                    // that holds the encrypted data.
                    memEncryptedData = memoryStream.ToArray();

                    // Close the MemoryStream object 
                    // release resources.
                    memoryStream.Close();
                }

                // Return the encrypted data buffer.
                return memEncryptedData;
            }
            catch (System.Exception ex)
            {
                string errorMessage = ex.Message;

                // If the exception is an cryptography
                // exception then add text.
                if (ex is CryptographicException)
                    errorMessage += " Cryptography exception has occurred.";

                // Make sure than a receiver instance of the
                // event delegate handler was created.
                if (OnAdvancedAESEncryptionError != null)
                {
                    // Create a new instance of the advanced TDES encryption argument.
                    AdvancedAESEncryptionArgs advancedEncryptArg = new AdvancedAESEncryptionArgs(errorMessage,
                    decryptedData);

                    // Fire the the event through the AdvancedAESEncryptionHandler delegate.
                    OnAdvancedAESEncryptionError(this, advancedEncryptArg);
                }

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Release all resources.
                if (memoryStream != null)
                    memoryStream.Close();

                // Release all resources.
                if (cryptoStream != null)
                {
                    try
                    {
                        cryptoStream.Clear();
                    }
                    catch (CryptographicException ce) { string error = ce.Message; }
                }
            }
        }

        /// <summary>
        /// This method will encrypt a decrypted stream.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The array of encrypted data.</returns>
        private byte[] EncryptStreamEx(Stream decryptedData, byte[] Key, byte[] IV)
        {
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                // Will contain the encrypted data
                // from the memory stream.
                byte[] memEncryptedData = null;

                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                using (memoryStream = new MemoryStream())
                {
                    // Create a new AES provider.
                    AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
                    provider.Mode = _cipherMode;
                    provider.Padding = _padding;
                    provider.BlockSize = _blockSize;
                    provider.KeySize = _keySize;

                    // Create a CryptoStream using the FileStream 
                    // and the passed key and initialization vector (IV).
                    using (cryptoStream = new CryptoStream(memoryStream,
                        provider.CreateEncryptor(Key, IV),
                        CryptoStreamMode.Write))
                    {
                        // This will track the number of 
                        // bytes read from the stream.
                        int intBytesRead = 0;

                        // If the decrypted file contains data
                        // then re-set the bytes reader value.
                        if (decryptedData.Length > 0)
                            intBytesRead = 1;

                        // The maximum size of each read and write;
                        byte[] rwBytes = new byte[numberOfBytes];

                        // While reading the file get the number
                        // of bytes, read while greater than zero.
                        while (intBytesRead > 0)
                        {
                            // Read from the decrypted file
                            // and write the encrypted data
                            // to the encrypt file.
                            intBytesRead = decryptedData.Read(rwBytes, 0, numberOfBytes);
                            cryptoStream.Write(rwBytes, 0, intBytesRead);
                        }

                        // Close the CryptoStream object 
                        // release resources.
                        cryptoStream.Close();
                    }

                    // Get an array of bytes from the MemoryStream 
                    // that holds the encrypted data.
                    memEncryptedData = memoryStream.ToArray();

                    // Close the MemoryStream object 
                    // release resources.
                    memoryStream.Close();
                }

                // Return the encrypted data buffer.
                return memEncryptedData;
            }
            catch (System.Exception ex)
            {
                string errorMessage = ex.Message;

                // If the exception is an cryptography
                // exception then add text.
                if (ex is CryptographicException)
                    errorMessage += " Cryptography exception has occurred.";

                // Make sure than a receiver instance of the
                // event delegate handler was created.
                if (OnAdvancedAESEncryptionError != null)
                {
                    // Create a new instance of the advanced TDES encryption argument.
                    AdvancedAESEncryptionArgs advancedEncryptArg = new AdvancedAESEncryptionArgs(errorMessage,
                    null);

                    // Fire the the event through the AdvancedAESEncryptionHandler delegate.
                    OnAdvancedAESEncryptionError(this, advancedEncryptArg);
                }

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Release all resources.
                if (memoryStream != null)
                    memoryStream.Close();

                // Release all resources.
                if (cryptoStream != null)
                {
                    try
                    {
                        cryptoStream.Clear();
                    }
                    catch (CryptographicException ce) { string error = ce.Message; }
                }
            }
        }

        /// <summary>
        /// This method will decrypt an encrypted stream.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The decrypted data.</returns>
        private byte[] DecryptStreamEx(Stream encryptedData, byte[] Key, byte[] IV)
        {
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                // Will contain the encrypted data
                // from the memory stream.
                byte[] memDecryptedData = null;

                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                using (memoryStream = new MemoryStream())
                {
                    // Create a new AES provider.
                    AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
                    provider.Mode = _cipherMode;
                    provider.Padding = _padding;
                    provider.BlockSize = _blockSize;
                    provider.KeySize = _keySize;

                    // Create a CryptoStream using the FileStream 
                    // and the passed key and initialization vector (IV).
                    using (cryptoStream = new CryptoStream(memoryStream,
                        provider.CreateDecryptor(Key, IV),
                        CryptoStreamMode.Write))
                    {
                        // This will track the number of 
                        // bytes read from the stream.
                        int intBytesRead = 0;

                        // If the encrypted file contains data
                        // then re-set the bytes reader value.
                        if (encryptedData.Length > 0)
                            intBytesRead = 1;

                        // The maximum size of each read and write;
                        byte[] rwBytes = new byte[numberOfBytes];

                        // While reading the file get the number
                        // of bytes, read while greater than zero.
                        while (intBytesRead > 0)
                        {
                            // Read from the decrypted file
                            // and write the encrypted data
                            // to the encrypt file.
                            intBytesRead = encryptedData.Read(rwBytes, 0, numberOfBytes);
                            cryptoStream.Write(rwBytes, 0, intBytesRead);
                        }

                        // Close the CryptoStream object 
                        // release resources.
                        cryptoStream.Close();
                    }

                    // Get an array of bytes from the MemoryStream 
                    // that holds the encrypted data.
                    memDecryptedData = memoryStream.ToArray();

                    // Close the MemoryStream object 
                    // release resources.
                    memoryStream.Close();
                }

                // Convert the buffer into a string and return it.
                return memDecryptedData;
            }
            catch (System.Exception ex)
            {
                string errorMessage = ex.Message;

                // If the exception is an cryptography
                // exception then add text.
                if (ex is CryptographicException)
                    errorMessage += " Cryptography exception has occurred.";

                // Make sure than a receiver instance of the
                // event delegate handler was created.
                if (OnAdvancedAESDecryptionError != null)
                {
                    // Create a new instance of the advanced TDES decryption argument.
                    AdvancedAESDecryptionArgs advancedDecryptArg = new AdvancedAESDecryptionArgs(errorMessage, null);

                    // Fire the the event through the AdvancedAESDecryptionHandler delegate.
                    OnAdvancedAESDecryptionError(this, advancedDecryptArg);
                }

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Release all resources.
                if (memoryStream != null)
                    memoryStream.Close();

                // Release all resources.
                if (cryptoStream != null)
                {
                    try
                    {
                        cryptoStream.Clear();
                    }
                    catch (CryptographicException ce) { string error = ce.Message; }
                }
            }
        }

        /// <summary>
        /// This method will decrypt an encrypted byte array.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The decrypted data.</returns>
        private byte[] DecryptFromMemoryEx(byte[] encryptedData, byte[] Key, byte[] IV)
        {
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                // Create buffer to hold the decrypted data.
                byte[] decryptedData = new byte[encryptedData.Length];

                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                using (memoryStream = new MemoryStream(encryptedData))
                {
                    // Create a new AES provider.
                    AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
                    provider.Mode = _cipherMode;
                    provider.Padding = _padding;
                    provider.BlockSize = _blockSize;
                    provider.KeySize = _keySize;

                    // Create a CryptoStream using the MemoryStream 
                    // and the passed key and initialization vector (IV).
                    using (cryptoStream = new CryptoStream(memoryStream,
                        provider.CreateDecryptor(Key, IV),
                        CryptoStreamMode.Read))
                    {
                        // Read the decrypted data out of the crypto stream
                        // and place it into the temporary buffer.
                        cryptoStream.Read(decryptedData, 0, decryptedData.Length);

                        // Close the CryptoStream object 
                        // release resources.
                        cryptoStream.Close();
                    }

                    // Close the MemoryStream object 
                    // release resources.
                    memoryStream.Close();
                }

                //Convert the buffer into a string and return it.
                return decryptedData;
            }
            catch (System.Exception ex)
            {
                string errorMessage = ex.Message;

                // If the exception is an cryptography
                // exception then add text.
                if (ex is CryptographicException)
                    errorMessage += " Cryptography exception has occurred.";

                // Make sure than a receiver instance of the
                // event delegate handler was created.
                if (OnAdvancedAESDecryptionError != null)
                {
                    // Create a new instance of the advanced TDES decryption argument.
                    AdvancedAESDecryptionArgs advancedDecryptArg = new AdvancedAESDecryptionArgs(errorMessage,
                        new ASCIIEncoding().GetString(encryptedData));

                    // Fire the the event through the AdvancedAESDecryptionHandler delegate.
                    OnAdvancedAESDecryptionError(this, advancedDecryptArg);
                }

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Release all resources.
                if (memoryStream != null)
                    memoryStream.Close();

                // Release all resources.
                if (cryptoStream != null)
                {
                    try
                    {
                        cryptoStream.Clear();
                    }
                    catch (CryptographicException ce) { string error = ce.Message; }
                }
            }
        }

        /// <summary>
        /// This method will encrypt a file to another file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>True if no error has occurred else false.</returns>
        private bool EncryptFileEx(string pathToDecryptedFile, string pathToEncryptedFile, byte[] Key, byte[] IV)
        {
            FileStream decryptedFile = null;
            FileStream encryptedFile = null;
            CryptoStream cryptoStream = null;

            try
            {
                // Create the encrypt and decrypt FileStreams.
                using (decryptedFile = new FileStream(pathToDecryptedFile, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite))
                using (encryptedFile = new FileStream(pathToEncryptedFile, FileMode.Create,
                    FileAccess.Write, FileShare.ReadWrite))
                {
                    // The length of this stream to the given value.
                    // This will over write or truncate the file if
                    // data exists within the file.
                    encryptedFile.SetLength(0);

                    // Create a new AES provider.
                    AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
                    provider.Mode = _cipherMode;
                    provider.Padding = _padding;
                    provider.BlockSize = _blockSize;
                    provider.KeySize = _keySize;

                    // Create a CryptoStream using the FileStream 
                    // and the passed key and initialization vector (IV).
                    using (cryptoStream = new CryptoStream(encryptedFile,
                        provider.CreateEncryptor(Key, IV),
                        CryptoStreamMode.Write))
                    {
                        // This will track the number of 
                        // bytes read from the stream.
                        int intBytesRead = 0;

                        // If the decrypted file contains data
                        // then re-set the bytes reader value.
                        if (decryptedFile.Length > 0)
                            intBytesRead = 1;

                        // The maximum size of each read and write;
                        byte[] rwBytes = new byte[numberOfBytes];

                        // While reading the file get the number
                        // of bytes, read while greater than zero.
                        while (intBytesRead > 0)
                        {
                            // Read from the decrypted file
                            // and write the encrypted data
                            // to the encrypt file.
                            intBytesRead = decryptedFile.Read(rwBytes, 0, numberOfBytes);
                            cryptoStream.Write(rwBytes, 0, intBytesRead);
                        }

                        // Close the CryptoStream object 
                        // release resources.
                        cryptoStream.Close();
                    }

                    // Close the FileStream object 
                    // release resources.
                    decryptedFile.Close();
                    encryptedFile.Close();
                }

                // Return true no exceptions.
                return true;
            }
            catch (System.Exception ex)
            {
                string errorMessage = ex.Message;

                // If the exception is an cryptography
                // exception then add text.
                if (ex is CryptographicException)
                    errorMessage += " Cryptography exception has occurred.";

                // Make sure than a receiver instance of the
                // event delegate handler was created.
                if (OnAdvancedAESEncryptionError != null)
                {
                    // Create a new instance of the advanced AES encryption argument.
                    AdvancedAESEncryptionArgs advancedEncryptArg = new AdvancedAESEncryptionArgs(errorMessage,
                        pathToEncryptedFile, pathToDecryptedFile);

                    // Fire the the event through the AdvancedAESEncryptionHandler delegate.
                    OnAdvancedAESEncryptionError(this, advancedEncryptArg);
                }

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Release all resources.
                if (decryptedFile != null)
                    decryptedFile.Close();

                // Release all resources.
                if (encryptedFile != null)
                    encryptedFile.Close();

                // Release all resources.
                if (cryptoStream != null)
                {
                    try
                    {
                        cryptoStream.Clear();
                    }
                    catch (CryptographicException ce) { string error = ce.Message; }
                }
            }
        }

        /// <summary>
        /// This method will decrypt a file from an encrypted file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>True if no error has occurred else false.</returns>
        private bool DecryptFileEx(string pathToDecryptedFile, string pathToEncryptedFile, byte[] Key, byte[] IV)
        {
            FileStream decryptedFile = null;
            FileStream encryptedFile = null;
            CryptoStream cryptoStream = null;

            try
            {
                // Create the encrypt and decrypt FileStreams.
                using (decryptedFile = new FileStream(pathToDecryptedFile, FileMode.Create,
                    FileAccess.Write, FileShare.ReadWrite))
                using (encryptedFile = new FileStream(pathToEncryptedFile, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite))
                {
                    // The length of this stream to the given value.
                    // This will over write or truncate the file if
                    // data exists within the file.
                    decryptedFile.SetLength(0);

                    // Create a new AES provider.
                    AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
                    provider.Mode = _cipherMode;
                    provider.Padding = _padding;
                    provider.BlockSize = _blockSize;
                    provider.KeySize = _keySize;

                    // Create a CryptoStream using the FileStream 
                    // and the passed key and initialization vector (IV).
                    using (cryptoStream = new CryptoStream(decryptedFile,
                        provider.CreateDecryptor(Key, IV),
                        CryptoStreamMode.Write))
                    {
                        // This will track the number of 
                        // bytes read from the stream.
                        int intBytesRead = 0;

                        // If the encrypted file contains data
                        // then re-set the bytes reader value.
                        if (encryptedFile.Length > 0)
                            intBytesRead = 1;

                        // The maximum size of each read and write;
                        byte[] rwBytes = new byte[numberOfBytes];

                        // While reading the file get the number
                        // of bytes, read while greater than zero.
                        while (intBytesRead > 0)
                        {
                            // Read from the decrypted file
                            // and write the encrypted data
                            // to the encrypt file.
                            intBytesRead = encryptedFile.Read(rwBytes, 0, numberOfBytes);
                            cryptoStream.Write(rwBytes, 0, intBytesRead);
                        }

                        // Close the CryptoStream object 
                        // release resources.
                        cryptoStream.Close();
                    }
                    // Close the FileStream object 
                    // release resources.
                    decryptedFile.Close();
                    encryptedFile.Close();
                }

                // Return true no exceptions.
                return true;
            }
            catch (System.Exception ex)
            {
                string errorMessage = ex.Message;

                // If the exception is an cryptography
                // exception then add text.
                if (ex is CryptographicException)
                    errorMessage += " Cryptography exception has occurred.";

                // Make sure than a receiver instance of the
                // event delegate handler was created.
                if (OnAdvancedAESDecryptionError != null)
                {
                    // Create a new instance of the advanced AES decryption argument.
                    AdvancedAESDecryptionArgs advancedDecryptArg = new AdvancedAESDecryptionArgs(errorMessage,
                        pathToEncryptedFile, pathToDecryptedFile);

                    // Fire the the event through the AdvancedAESDecryptionHandler delegate.
                    OnAdvancedAESDecryptionError(this, advancedDecryptArg);
                }

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Release all resources.
                if (decryptedFile != null)
                    decryptedFile.Close();

                // Release all resources.
                if (encryptedFile != null)
                    encryptedFile.Close();

                // Release all resources.
                if (cryptoStream != null)
                {
                    try
                    {
                        cryptoStream.Clear();
                    }
                    catch (CryptographicException ce) { string error = ce.Message; }
                }
            }
        }

        /// <summary>
        /// This method will write the data to an encrypted file.
        /// </summary>
        /// <param name="decryptedData">The list of string data to encrypt.</param>
        /// <param name="pathToEncryptedFile">The file where encrypted data is to be placed.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>True if the data was written to the file else false.</returns>
        private bool EncryptToFileEx(List<string> decryptedData, string pathToEncryptedFile, byte[] Key, byte[] IV)
        {
            bool dataEncrypted = false;

            // If data to encrypt exists
            // then process the data passed.
            if (decryptedData != null)
                if (decryptedData.Count > 0)
                {
                    FileStream encryptedFile = null;
                    CryptoStream cryptoStream = null;

                    try
                    {
                        // Create or open the specified file.
                        using (encryptedFile = File.Open(pathToEncryptedFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        {
                            // Create a new AES provider.
                            AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
                            provider.Mode = _cipherMode;
                            provider.Padding = _padding;
                            provider.BlockSize = _blockSize;
                            provider.KeySize = _keySize;

                            // Create a CryptoStream using the FileStream 
                            // and the passed key and initialization vector (IV).
                            using (cryptoStream = new CryptoStream(encryptedFile,
                                provider.CreateEncryptor(Key, IV),
                                CryptoStreamMode.Write))
                            {
                                // For each data string within the
                                // list array write to the file.
                                for (int i = 0; i < decryptedData.Count; i++)
                                {
                                    // Convert the passed string to a byte array.
                                    byte[] rwBytes = Encoding.Default.GetBytes(decryptedData[i]);

                                    // Number of bytes to write.
                                    int intBytesRead = rwBytes.Length;

                                    // Write the current byte array
                                    // to the encrypted file.
                                    cryptoStream.Write(rwBytes, 0, intBytesRead);
                                }

                                // Close the CryptoStream object 
                                // release resources.
                                cryptoStream.Close();
                            }
                            // Close the FileStream object 
                            // release resources.
                            encryptedFile.Close();
                        }

                        // Return true no exceptions.
                        dataEncrypted = true;
                    }
                    catch (System.Exception ex)
                    {
                        string errorMessage = ex.Message;

                        // If the exception is an cryptography
                        // exception then add text.
                        if (ex is CryptographicException)
                            errorMessage += " Cryptography exception has occurred.";

                        // Make sure than a receiver instance of the
                        // event delegate handler was created.
                        if (OnAdvancedAESEncryptionError != null)
                        {
                            // Create a new instance of the advanced AES encryption argument.
                            AdvancedAESEncryptionArgs advancedEncryptArg = new AdvancedAESEncryptionArgs(errorMessage,
                                pathToEncryptedFile, string.Empty);

                            // Fire the the event through the AdvancedAESEncryptionHandler delegate.
                            OnAdvancedAESEncryptionError(this, advancedEncryptArg);
                        }

                        // Throw a general exception.
                        throw new System.Exception(ex.Message, ex.InnerException);
                    }
                    finally
                    {
                        // Release all resources.
                        if (encryptedFile != null)
                            encryptedFile.Close();

                        // Release all resources.
                        if (cryptoStream != null)
                        {
                            try
                            {
                                cryptoStream.Clear();
                            }
                            catch (CryptographicException ce) { string error = ce.Message; }
                        }
                    }
                }

            // Return the indicator, true
            // if data was encrypted else false.
            return dataEncrypted;
        }

        /// <summary>
        /// This method will decrypt data from an encrypted file.
        /// </summary>
        /// <param name="pathToEncryptedFile">The file where encrypted data is loaded from.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The list of string data decrypted.</returns>
        private List<string> DecryptFromFileEx(string pathToEncryptedFile, byte[] Key, byte[] IV)
        {
            FileStream decryptedFile = null;
            CryptoStream cryptoStream = null;
            List<string> decryptedData = null;

            try
            {
                // Create the decrypt FileStreams.
                using (decryptedFile = new FileStream(pathToEncryptedFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Create a new AES provider.
                    AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
                    provider.Mode = _cipherMode;
                    provider.Padding = _padding;
                    provider.BlockSize = _blockSize;
                    provider.KeySize = _keySize;

                    // Create a CryptoStream using the FileStream 
                    // and the passed key and initialization vector (IV).
                    using (cryptoStream = new CryptoStream(decryptedFile,
                        provider.CreateDecryptor(Key, IV),
                        CryptoStreamMode.Read))
                    {
                        // This will track the number of 
                        // bytes read from the stream.
                        int intBytesRead = 0;

                        // If the decrypted file contains data
                        // then re-set the bytes reader value.
                        if (decryptedFile.Length > 0)
                        {
                            // Initialize the read back.
                            intBytesRead = 1;

                            // Create a new instance of the
                            // list object.
                            decryptedData = new List<string>();
                        }

                        // The maximum size of each read and write;
                        byte[] rwBytes = new byte[numberOfBytes];

                        // While reading the file get the number
                        // of bytes, read while greater than zero.
                        while (intBytesRead > 0)
                        {
                            // Read into the buffer from the file.
                            intBytesRead = cryptoStream.Read(rwBytes, 0, numberOfBytes);

                            // Only add to the list if
                            // more than zero bytes read.
                            if (intBytesRead > 0)
                            {
                                //Convert the buffer into a string and return it.
                                string deData = Encoding.Default.GetString(rwBytes);

                                // Add the buffer to the list array.
                                decryptedData.Add(deData);
                            }
                        }

                        // Close the CryptoStream object 
                        // release resources.
                        cryptoStream.Close();
                    }
                    // Close the FileStream object 
                    // release resources.
                    decryptedFile.Close();
                }

                // Return the decrypted data.
                return decryptedData;
            }
            catch (System.Exception ex)
            {
                string errorMessage = ex.Message;

                // If the exception is an cryptography
                // exception then add text.
                if (ex is CryptographicException)
                    errorMessage += " Cryptography exception has occurred.";

                // Make sure than a receiver instance of the
                // event delegate handler was created.
                if (OnAdvancedAESDecryptionError != null)
                {
                    // Create a new instance of the advanced AES decryption argument.
                    AdvancedAESDecryptionArgs advancedEncryptArg = new AdvancedAESDecryptionArgs(errorMessage,
                        pathToEncryptedFile, string.Empty);

                    // Fire the the event through the AdvancedAESDecryptionHandler delegate.
                    OnAdvancedAESDecryptionError(this, advancedEncryptArg);
                }

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Release all resources.
                if (decryptedFile != null)
                    decryptedFile.Close();

                // Release all resources.
                if (cryptoStream != null)
                {
                    try
                    {
                        cryptoStream.Clear();
                    }
                    catch (CryptographicException ce) { string error = ce.Message; }
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Asynchronous advanced encryption decryption of data class.
    /// </summary>
    public class AsynchronousAdvancedAES : Nequeo.Runtime.DisposableBase, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly AsynchronousAdvancedAES Instance = new AsynchronousAdvancedAES();

        /// <summary>
        /// Static constructor
        /// </summary>
        static AsynchronousAdvancedAES() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public AsynchronousAdvancedAES()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Begin encrypting the set of data passed.
        /// </summary>
        /// <param name="decryptedData">The data to encrypt.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginEncryptToMemory(byte[] decryptedData,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncEncryptToMemoryAES(decryptedData, callback, state);
        }

        /// <summary>
        /// Begin encrypting the set of data passed.
        /// </summary>
        /// <param name="decryptedData">The data to encrypt.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginEncryptToMemory(byte[] decryptedData, byte[] Key,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncEncryptToMemoryAES(decryptedData, Key, callback, state);
        }

        /// <summary>
        /// Begin encrypting the set of data passed.
        /// </summary>
        /// <param name="decryptedData">The data to encrypt.</param>
        /// <param name="password">The password used to encrypt the data.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginEncryptToMemory(byte[] decryptedData, string password,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncEncryptToMemoryAES(decryptedData, password, callback, state);
        }

        /// <summary>
        /// Begin encrypting the set of data passed.
        /// </summary>
        /// <param name="decryptedData">The data to encrypt.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginEncryptToMemory(byte[] decryptedData, byte[] Key, byte[] IV,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncEncryptToMemoryAES(decryptedData, Key, IV, callback, state);
        }

        /// <summary>
        /// End encrypting the set of data passed.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The encrypted bytes.</returns>
        public byte[] EndEncryptToMemory(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncEncryptToMemoryAES.End(ar);
        }

        /// <summary>
        /// End decrypting the set of data passed.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The decrypted bytes.</returns>
        public byte[] EndDecryptFromMemory(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncDecryptFromMemoryAES.End(ar);
        }

        /// <summary>
        /// End decrypting the set of data passed.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The decrypted indicator.</returns>
        public Boolean EndDecryptFile(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncDecryptFileAES.End(ar);
        }

        /// <summary>
        /// End encrypting the set of data passed.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The encrypted indicator.</returns>
        public Boolean EndEncryptFile(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncEncryptFileAES.End(ar);
        }

        /// <summary>
        /// Begin encrypting the set of data passed.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDecryptFromMemory(byte[] encryptedData,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncDecryptFromMemoryAES(encryptedData, callback, state);
        }

        /// <summary>
        /// Begin encrypting the set of data passed.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDecryptFromMemory(byte[] encryptedData, byte[] Key,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncDecryptFromMemoryAES(encryptedData, Key, callback, state);
        }

        /// <summary>
        /// Begin encrypting the set of data passed.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="password">The array of encrypted bytes.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDecryptFromMemory(byte[] encryptedData, string password,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncDecryptFromMemoryAES(encryptedData, password, callback, state);
        }

        /// <summary>
        /// Begin encrypting the set of data passed.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDecryptFromMemory(byte[] encryptedData, byte[] Key, byte[] IV,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncDecryptFromMemoryAES(encryptedData, Key, IV, callback, state);
        }

        /// <summary>
        /// Begin encrypting the set of data passed.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginEncryptFile(string pathToDecryptedFile, string pathToEncryptedFile,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncEncryptFileAES(pathToDecryptedFile, pathToEncryptedFile, callback, state);
        }

        /// <summary>
        /// Begin encrypting the set of data passed.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginEncryptFile(string pathToDecryptedFile, string pathToEncryptedFile,
            string password, AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncEncryptFileAES(pathToDecryptedFile,
                pathToEncryptedFile, password, callback, state);
        }

        /// <summary>
        /// Begin decrypting the set of data passed.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDecryptFile(string pathToDecryptedFile, string pathToEncryptedFile,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncDecryptFileAES(pathToDecryptedFile, pathToEncryptedFile, callback, state);
        }

        /// <summary>
        /// Begin decrypting the set of data passed.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDecryptFile(string pathToDecryptedFile, string pathToEncryptedFile,
            string password, AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncDecryptFileAES(pathToDecryptedFile,
                pathToEncryptedFile, password, callback, state);
        }
        #endregion
    }

    /// <summary>
    /// Class for asynchronous cryptograhpy operations.
    /// </summary>
    internal class AsyncDecryptFromMemoryAES : Nequeo.Threading.AsynchronousResult<byte[]>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous decryption operation.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncDecryptFromMemoryAES(byte[] encryptedData,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _encryptedData = encryptedData;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDecryptFromMemoryThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous decryption operation.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncDecryptFromMemoryAES(byte[] encryptedData, byte[] Key,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _encryptedData = encryptedData;
            _Key = Key;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDecryptFromMemoryThread2));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous decryption operation.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="password">The array of encrypted bytes.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncDecryptFromMemoryAES(byte[] encryptedData, string password,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _encryptedData = encryptedData;
            _password = password;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDecryptFromMemoryThread3));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous decryption operation.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncDecryptFromMemoryAES(byte[] encryptedData, byte[] Key, byte[] IV,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _encryptedData = encryptedData;
            _Key = Key;
            _IV = IV;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDecryptFromMemoryThread4));
            Thread.Sleep(20);
        }

        private byte[] _encryptedData = null;
        private byte[] _Key = null;
        private byte[] _IV = null;
        private string _password = null;
        private Exception _exception = null;

        /// <summary>
        /// Gets the current exception.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// The async decrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDecryptFromMemoryThread1(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the decrypted data.
                    byte[] data = advTDES.DecryptFromMemory(_encryptedData);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data != null)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
        }

        /// <summary>
        /// The async decrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDecryptFromMemoryThread2(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the decrypted data.
                    byte[] data = advTDES.DecryptFromMemory(_encryptedData, _Key);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data != null)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
            
        }

        /// <summary>
        /// The async decrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDecryptFromMemoryThread3(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the decrypted data.
                    byte[] data = advTDES.DecryptFromMemory(_encryptedData, _password);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data != null)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
        }

        /// <summary>
        /// The async decrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDecryptFromMemoryThread4(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the decrypted data.
                    byte[] data = advTDES.DecryptFromMemory(_encryptedData, _Key, _IV);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data != null)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
        }
        #endregion
    }

    /// <summary>
    /// Class for asynchronous cryptograhpy operations.
    /// </summary>
    internal class AsyncEncryptToMemoryAES : Nequeo.Threading.AsynchronousResult<byte[]>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous encryption operation.
        /// </summary>
        /// <param name="decryptedData">The data to encrypt.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncEncryptToMemoryAES(byte[] decryptedData,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _decryptedData = decryptedData;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncEncryptToMemoryThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous encryption operation.
        /// </summary>
        /// <param name="decryptedData">The data to encrypt.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncEncryptToMemoryAES(byte[] decryptedData, byte[] Key,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _decryptedData = decryptedData;
            _Key = Key;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncEncryptToMemoryThread2));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous encryption operation.
        /// </summary>
        /// <param name="decryptedData">The data to encrypt.</param>
        /// <param name="password">The password used to encrypt the data.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncEncryptToMemoryAES(byte[] decryptedData, string password,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _decryptedData = decryptedData;
            _password = password;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncEncryptToMemoryThread3));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous encryption operation.
        /// </summary>
        /// <param name="decryptedData">The data to encrypt.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncEncryptToMemoryAES(byte[] decryptedData, byte[] Key, byte[] IV,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _decryptedData = decryptedData;
            _Key = Key;
            _IV = IV;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncEncryptToMemoryThread4));
            Thread.Sleep(20);
        }

        private byte[] _decryptedData = null;
        private byte[] _Key = null;
        private byte[] _IV = null;
        private string _password = null;
        private Exception _exception = null;

        /// <summary>
        /// Gets the current exception.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// The async encrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncEncryptToMemoryThread1(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the encrypted data.
                    byte[] data = advTDES.EncryptToMemory(_decryptedData);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data != null)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
        }

        /// <summary>
        /// The async encrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncEncryptToMemoryThread2(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the encrypted data.
                    byte[] data = advTDES.EncryptToMemory(_decryptedData, _Key);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data != null)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
        }

        /// <summary>
        /// The async encrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncEncryptToMemoryThread3(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the encrypted data.
                    byte[] data = advTDES.EncryptToMemory(_decryptedData, _password);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data != null)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
        }

        /// <summary>
        /// The async encrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncEncryptToMemoryThread4(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the encrypted data.
                    byte[] data = advTDES.EncryptToMemory(_decryptedData, _Key, _IV);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data != null)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
        }
        #endregion
    }

    /// <summary>
    /// Class for asynchronous cryptograhpy operations.
    /// </summary>
    internal class AsyncDecryptFileAES : Nequeo.Threading.AsynchronousResult<Boolean>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous decryption operation.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncDecryptFileAES(string pathToDecryptedFile, string pathToEncryptedFile,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _pathToDecryptedFile = pathToDecryptedFile;
            _pathToEncryptedFile = pathToEncryptedFile;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDecryptFileThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous decryption operation.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncDecryptFileAES(string pathToDecryptedFile, string pathToEncryptedFile,
            string password, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _pathToDecryptedFile = pathToDecryptedFile;
            _pathToEncryptedFile = pathToEncryptedFile;
            _password = password;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDecryptFileThread2));
            Thread.Sleep(20);
        }

        private string _pathToDecryptedFile = null;
        private string _pathToEncryptedFile = null;
        private string _password = null;
        private Exception _exception = null;

        /// <summary>
        /// Gets the current exception.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// The async decrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDecryptFileThread1(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the decrypted data.
                    bool data = advTDES.DecryptFile(_pathToDecryptedFile, _pathToEncryptedFile);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
        }

        /// <summary>
        /// The async decrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDecryptFileThread2(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the decrypted data.
                    bool data = advTDES.DecryptFile(_pathToDecryptedFile,
                        _pathToEncryptedFile, _password);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
        }
        #endregion
    }

    /// <summary>
    /// Class for asynchronous cryptograhpy operations.
    /// </summary>
    internal class AsyncEncryptFileAES : Nequeo.Threading.AsynchronousResult<Boolean>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous encryption operation.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncEncryptFileAES(string pathToDecryptedFile, string pathToEncryptedFile,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _pathToDecryptedFile = pathToDecryptedFile;
            _pathToEncryptedFile = pathToEncryptedFile;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncEncryptFileThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous encryption operation.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncEncryptFileAES(string pathToDecryptedFile, string pathToEncryptedFile,
            string password, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _pathToDecryptedFile = pathToDecryptedFile;
            _pathToEncryptedFile = pathToEncryptedFile;
            _password = password;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncEncryptFileThread2));
            Thread.Sleep(20);
        }

        private string _pathToDecryptedFile = null;
        private string _pathToEncryptedFile = null;
        private string _password = null;
        private Exception _exception = null;

        /// <summary>
        /// Gets the current exception.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// The async encrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncEncryptFileThread1(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the decrypted data.
                    bool data = advTDES.EncryptFile(_pathToDecryptedFile, _pathToEncryptedFile);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
        }

        /// <summary>
        /// The async encrypt file method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncEncryptFileThread2(Object stateInfo)
        {
            try
            {
                // Create a new advanced triple DES instance.
                using (AdvancedAES advTDES = new AdvancedAES())
                {
                    // Get the decrypted data.
                    bool data = advTDES.EncryptFile(_pathToDecryptedFile,
                        _pathToEncryptedFile, _password);

                    // If data exits then indicate that the asynchronous
                    // operation has completed and send the result to the
                    // client, else indicate that the asynchronous operation
                    // has failed and did not complete.
                    if (data)
                        base.Complete(data, true);
                    else
                        base.Complete(false);
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                base.Complete(true, ex);
            }
        }
        #endregion
    }

    /// <summary>
    /// Advanced encryption decryption of data class.
    /// </summary>
    public interface IAdvancedAES
    {
        #region Public Methods
        /// <summary>
        /// This method will encrypt a decrypted string.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <returns>The array of encrypted data.</returns>
        byte[] EncryptToMemory(byte[] decryptedData);

        /// <summary>
        /// This method will encrypt a decrypted string.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>The array of encrypted data.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        byte[] EncryptToMemory(byte[] decryptedData, byte[] Key);

        /// <summary>
        /// This method will encrypt a decrypted string.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>The array of encrypted data.</returns>
        /// <remarks>The password must be between 0 and 32 bytes in length.</remarks>
        byte[] EncryptToMemory(byte[] decryptedData, string password);

        /// <summary>
        /// This method will encrypt a decrypted string.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The array of encrypted data.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        byte[] EncryptToMemory(byte[] decryptedData, byte[] Key, byte[] IV);

        /// <summary>
        /// This method will decrypt an encrypted byte array.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <returns>The string of decrypted data.</returns>
        byte[] DecryptFromMemory(byte[] encryptedData);

        /// <summary>
        /// This method will decrypt an encrypted byte array.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>The string of decrypted data.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        byte[] DecryptFromMemory(byte[] encryptedData, byte[] Key);

        /// <summary>
        /// This method will decrypt an encrypted byte array.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>The string of decrypted data.</returns>
        /// <remarks>The password must be between 0 and 24 bytes in length.</remarks>
        byte[] DecryptFromMemory(byte[] encryptedData, string password);

        /// <summary>
        /// This method will decrypt an encrypted byte array.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The string of decrypted data.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        byte[] DecryptFromMemory(byte[] encryptedData, byte[] Key, byte[] IV);

        /// <summary>
        /// This method will encrypt a file to another file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <returns>True if no error has occurred else false.</returns>
        bool EncryptFile(string pathToDecryptedFile, string pathToEncryptedFile);

        /// <summary>
        /// This method will encrypt a file to another file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        bool EncryptFile(string pathToDecryptedFile, string pathToEncryptedFile, byte[] Key);

        /// <summary>
        /// This method will encrypt a file to another file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The password must be between 0 and 32 bytes in length.</remarks>
        bool EncryptFile(string pathToDecryptedFile, string pathToEncryptedFile, string password);

        /// <summary>
        /// This method will encrypt a file to another file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        bool EncryptFile(string pathToDecryptedFile, string pathToEncryptedFile, byte[] Key, byte[] IV);

        /// <summary>
        /// This method will decrypt a file from an encrypted file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <returns>True if no error has occurred else false.</returns>
        bool DecryptFile(string pathToDecryptedFile, string pathToEncryptedFile);

        /// <summary>
        /// This method will decrypt a file from an encrypted file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        bool DecryptFile(string pathToDecryptedFile, string pathToEncryptedFile, byte[] Key);

        /// <summary>
        /// This method will decrypt a file from an encrypted file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The password must be between 0 and 32 bytes in length.</remarks>
        bool DecryptFile(string pathToDecryptedFile, string pathToEncryptedFile, string password);

        /// <summary>
        /// This method will decrypt a file from an encrypted file.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        bool DecryptFile(string pathToDecryptedFile, string pathToEncryptedFile, byte[] Key, byte[] IV);

        /// <summary>
        /// This method will write the data to an encrypted file.
        /// </summary>
        /// <param name="decryptedData">The list of string data to encrypt.</param>
        /// <param name="pathToEncryptedFile">The file where encrypted data is to be placed.</param>
        /// <returns>True if the data was written to the file else false.</returns>
        bool EncryptToFile(List<string> decryptedData, string pathToEncryptedFile);

        /// <summary>
        /// This method will write the data to an encrypted file.
        /// </summary>
        /// <param name="decryptedData">The list of string data to encrypt.</param>
        /// <param name="pathToEncryptedFile">The file where encrypted data is to be placed.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>True if the data was written to the file else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        bool EncryptToFile(List<string> decryptedData, string pathToEncryptedFile, byte[] Key);

        /// <summary>
        /// This method will write the data to an encrypted file.
        /// </summary>
        /// <param name="decryptedData">The list of string data to encrypt.</param>
        /// <param name="pathToEncryptedFile">The file where encrypted data is to be placed.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>True if the data was written to the file else false.</returns>
        /// <remarks>The password must be between 0 and 32 bytes in length.</remarks>
        bool EncryptToFile(List<string> decryptedData, string pathToEncryptedFile, string password);

        /// <summary>
        /// This method will write the data to an encrypted file.
        /// </summary>
        /// <param name="decryptedData">The list of string data to encrypt.</param>
        /// <param name="pathToEncryptedFile">The file where encrypted data is to be placed.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>True if the data was written to the file else false.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        bool EncryptToFile(List<string> decryptedData, string pathToEncryptedFile, byte[] Key, byte[] IV);

        /// <summary>
        /// This method will decrypt data from an encrypted file.
        /// </summary>
        /// <param name="pathToEncryptedFile">The file where encrypted data is loaded from.</param>
        /// <returns>The list of string data decrypted.</returns>
        List<string> DecryptFromFile(string pathToEncryptedFile);

        /// <summary>
        /// This method will decrypt data from an encrypted file.
        /// </summary>
        /// <param name="pathToEncryptedFile">The file where encrypted data is loaded from.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>The list of string data decrypted.</returns>
        /// <remarks>The key must be exactly 32 bytes in length.</remarks>
        List<string> DecryptFromFile(string pathToEncryptedFile, byte[] Key);

        /// <summary>
        /// This method will decrypt data from an encrypted file.
        /// </summary>
        /// <param name="pathToEncryptedFile">The file where encrypted data is loaded from.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>The list of string data decrypted.</returns>
        /// <remarks>The password must be between 0 and 32 bytes in length.</remarks>
        List<string> DecryptFromFile(string pathToEncryptedFile, string password);

        /// <summary>
        /// This method will decrypt data from an encrypted file.
        /// </summary>
        /// <param name="pathToEncryptedFile">The file where encrypted data is loaded from.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The list of string data decrypted.</returns>
        /// <remarks>The key must be exactly 32 bytes in length and the
        /// vector must be exactly 16 bytes in length.</remarks>
        List<string> DecryptFromFile(string pathToEncryptedFile, byte[] Key, byte[] IV);

        /// <summary>
        /// This method will encrypt a decrypted stream.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <returns>The array of encrypted data.</returns>
        byte[] EncryptStream(Stream decryptedData);

        /// <summary>
        /// This method will encrypt a decrypted stream.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>The array of encrypted data.</returns>
        byte[] EncryptStream(Stream decryptedData, byte[] Key);

        /// <summary>
        /// This method will encrypt a decrypted stream.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>The array of encrypted data.</returns>
        byte[] EncryptStream(Stream decryptedData, string password);

        /// <summary>
        /// This method will encrypt a decrypted stream.
        /// </summary>
        /// <param name="decryptedData">The string of decrypted data.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The array of encrypted data.</returns>
        byte[] EncryptStream(Stream decryptedData, byte[] Key, byte[] IV);

        /// <summary>
        /// This method will decrypt an encrypted stream.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <returns>The decrypted data.</returns>
        byte[] DecryptStream(Stream encryptedData);

        /// <summary>
        /// This method will decrypt an encrypted stream.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <returns>The decrypted data.</returns>
        byte[] DecryptStream(Stream encryptedData, byte[] Key);

        /// <summary>
        /// This method will decrypt an encrypted stream.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="password">The password used for cryptography.</param>
        /// <returns>The decrypted data.</returns>
        byte[] DecryptStream(Stream encryptedData, string password);

        /// <summary>
        /// This method will decrypt an encrypted stream.
        /// </summary>
        /// <param name="encryptedData">The array of encrypted bytes.</param>
        /// <param name="Key">The current key for cryptography.</param>
        /// <param name="IV">The current initialising vector.</param>
        /// <returns>The decrypted data.</returns>
        byte[] DecryptStream(Stream encryptedData, byte[] Key, byte[] IV);

        #endregion
    }
}
