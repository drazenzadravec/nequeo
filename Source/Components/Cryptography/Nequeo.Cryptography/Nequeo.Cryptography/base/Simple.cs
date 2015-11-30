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
using System.Security.Cryptography;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Nequeo.Cryptography
{
    #region Public Delegate
    /// <summary>
    /// This delegate handles the transport of data from sender
    /// to receiver when simple encryption process occurs.
    /// </summary>
    /// <param name="sender">The current object sending the message</param>
    /// <param name="e">The object containing the event information.</param>
    public delegate void SimpleEncryptionHandler(object sender, SimpleEncryptionArgs e);

    /// <summary>
    /// This delegate handles the transport of data from sender
    /// to receiver when simple decryption process occurs.
    /// </summary>
    /// <param name="sender">The current object sending the message</param>
    /// <param name="e">The object containing the event information.</param>
    public delegate void SimpleDecryptionHandler(object sender, SimpleDecryptionArgs e);
    #endregion

    /// <summary>
    /// Simple encryption decryption of data class.
    /// </summary>
    public class Simple : Nequeo.Runtime.DisposableBase, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public Simple()
        {
        }
        #endregion

        #region Private Constants
        private const int iKey = 20;
        private const int iVector = 8;
        #endregion

        #region Public Events
        /// <summary>
        /// This event occurs when an error occurs 
        /// attempting to encrypt data.
        /// </summary>
        public event SimpleEncryptionHandler OnSimpleEncryptionError;

        /// <summary>
        /// This event occurs when an error occurs 
        /// attempting to decrypt data.
        /// </summary>
        public event SimpleDecryptionHandler OnSimpleDecryptionError;
        #endregion

        #region Public Cryptography Methods
        /// <summary>
        /// This method encrypts the out-going data.
        /// </summary>
        /// <param name="sData">The data to encrypt.</param>
        /// <returns>The encrypted data if no error else the original data.</returns>
        public virtual string Encrypt(string sData)
        {
            try
            {
                string sEncryptedData = string.Empty;

                // Return the data to encrypt into
                // a character array.
                char[] cData = sData.ToCharArray();

                // For each character within the data
                // encrypt the character.
                for (int i = 0; i < cData.Length; i++)
                {
                    // Encrypt each character with the
                    // specified key and vector.
                    sEncryptedData += Convert.ToString(Convert.ToChar(Convert.ToInt32(Convert.ToByte(cData[i]) - iKey + iVector + 2)));
                }

                // Return the encrypt data.
                return sEncryptedData;
            }
            catch (System.Exception ex)
            {
                // Make sure than a receiver instance of the
                // event delegate handler was created.
                if (OnSimpleEncryptionError != null)
                {
                    // Create a new instance of the simple encryption argument.
                    SimpleEncryptionArgs simpleEncryptArg = new SimpleEncryptionArgs(ex.Message, sData);
                    // Fire the the event through the SimpleEncryptionHandler delegate.
                    OnSimpleEncryptionError(this, simpleEncryptArg);
                }

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// This method decrypts the in-coming data.
        /// </summary>
        /// <param name="sData">The data to decrypt.</param>
        /// <returns>The decrypted data if no error else the original data.</returns>
        /// <remarks>When decrypting data make sure that the data being passed
        /// was encrypted within this class by the Encrypt method. Decryption
        /// will fail if a different encryption algorithm was used.</remarks>
        public virtual string Decrypt(string sData)
        {
            try
            {
                string sDecryptedData = string.Empty;

                // Return the data to decrypt into
                // a character array.
                char[] cData = sData.ToCharArray();

                // For each character within the data
                // decrypt the character.
                for (int i = 0; i < cData.Length; i++)
                {
                    // Decrypt each character with the
                    // specified key and vector.
                    sDecryptedData += Convert.ToString(Convert.ToChar(Convert.ToInt32(Convert.ToByte(cData[i]) + iKey - iVector - 2)));
                }

                // Return the decrypt data.
                return sDecryptedData;
            }
            catch (System.Exception ex)
            {
                // Make sure than a receiver instance of the
                // event delegate handler was created.
                if (OnSimpleDecryptionError != null)
                {
                    // Create a new instance of the simple encryption argument.
                    SimpleDecryptionArgs simpleDecryptArg = new SimpleDecryptionArgs(ex.Message, sData);
                    // Fire the the event through the SimpleDecryptionHandler delegate.
                    OnSimpleDecryptionError(this, simpleDecryptArg);
                }

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        #region Public Static Cryptography Methods
        /// <summary>
        /// This method encrypts the out-going data.
        /// </summary>
        /// <param name="sData">The data to encrypt.</param>
        /// <returns>The encrypted data if no error else the original data.</returns>
        public static string EncryptData(string sData)
        {
            string sEncryptedData = string.Empty;

            // Return the data to encrypt into
            // a character array.
            char[] cData = sData.ToCharArray();

            // For each character within the data
            // encrypt the character.
            for (int i = 0; i < cData.Length; i++)
            {
                // Encrypt each character with the
                // specified key and vector.
                sEncryptedData += Convert.ToString(Convert.ToChar(Convert.ToInt32(Convert.ToByte(cData[i]) - iKey + iVector + 2)));
            }

            // Return the encrypt data.
            return sEncryptedData;
        }

        /// <summary>
        /// This method decrypts the in-coming data.
        /// </summary>
        /// <param name="sData">The data to decrypt.</param>
        /// <returns>The decrypted data if no error else the original data.</returns>
        /// <remarks>When decrypting data make sure that the data being passed
        /// was encrypted within this class by the Encrypt method. Decryption
        /// will fail if a different encryption algorithm was used.</remarks>
        public static string DecryptData(string sData)
        {
            string sDecryptedData = string.Empty;

            // Return the data to decrypt into
            // a character array.
            char[] cData = sData.ToCharArray();

            // For each character within the data
            // decrypt the character.
            for (int i = 0; i < cData.Length; i++)
            {
                // Decrypt each character with the
                // specified key and vector.
                sDecryptedData += Convert.ToString(Convert.ToChar(Convert.ToInt32(Convert.ToByte(cData[i]) + iKey - iVector - 2)));
            }

            // Return the decrypt data.
            return sDecryptedData;
        }
        #endregion
    }

    /// <summary>
    /// Hash algorithm generation class.
    /// </summary>
    public sealed class Hashcode : Nequeo.Runtime.DisposableBase, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public Hashcode()
        {
        }
        #endregion

        #region Static Public Methods
        /// <summary>
        /// Generate a random salt.
        /// </summary>
        /// <param name="minimum">The minimum length of the salt.</param>
        /// <param name="maximum">The minimum length of the salt.</param>
        /// <returns>The random salt value.</returns>
        public static string GenerateSalt(int minimum = 20, int maximum = 20)
        {
            // Generate a random salt.
            Nequeo.Cryptography.RandomPassword salt = new Nequeo.Cryptography.RandomPassword();
            return salt.Generate(minimum, maximum);
        }

        /// <summary>
        /// Get the hashcode from the value.
        /// </summary>
        /// <param name="value">The value to generate the hash code for.</param>
        /// <param name="hashcodeType">The hash name.</param>
        /// <returns>The generated hash code.</returns>
        public static string GetHashcode(string value, Nequeo.Cryptography.HashcodeType hashcodeType)
        {
            int i = 0;
            
            // Generate the hash code
            HashAlgorithm alg = HashAlgorithm.Create(hashcodeType.ToString());
            byte[] byteValue = Encoding.ASCII.GetBytes(value);
            byte[] hashValue = alg.ComputeHash(byteValue);

            // Get the string value of hashcode.
            string[] octetArrayByte = new string[hashValue.Count()];
            foreach (Byte item in hashValue)
                octetArrayByte[i++] = item.ToString("X2");

            // Create the octet string of bytes.
            string octetValue = String.Join("", octetArrayByte);
            return octetValue;
        }

        /// <summary>
        /// Get the hashcode from the value.
        /// </summary>
        /// <param name="value">The value to generate the hash code for.</param>
        /// <param name="hashcodeType">The hash name.</param>
        /// <returns>The generated hash code.</returns>
        public static byte[] GetHashcodeRaw(string value, Nequeo.Cryptography.HashcodeType hashcodeType)
        {
            // Generate the hash code
            HashAlgorithm alg = HashAlgorithm.Create(hashcodeType.ToString());
            byte[] byteValue = Encoding.ASCII.GetBytes(value);
            byte[] hashValue = alg.ComputeHash(byteValue);
            return hashValue;
        }

        /// <summary>
        /// Gets the MD5 hashcode from the value.
        /// </summary>
        /// <param name="value">The value to generate the hash code for.</param>
        /// <returns>The generated hash code.</returns>
        public static string GetHashcodeMD5(string value)
        {
            int i = 0;

            // Generate the hash code
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] byteValue = Encoding.ASCII.GetBytes(value);
            byte[] hashValue = md5.ComputeHash(byteValue);

            // Get the string value of hashcode.
            string[] octetArrayByte = new string[hashValue.Count()];
            foreach (Byte item in hashValue)
                octetArrayByte[i++] = item.ToString("X2");

            // Create the octet string of bytes.
            string octetValue = String.Join("", octetArrayByte);
            return octetValue;
        }

        /// <summary>
        /// Gets the MD5 hashcode from the value.
        /// </summary>
        /// <param name="value">The value to generate the hash code for.</param>
        /// <returns>The generated hash code.</returns>
        public static byte[] GetHashcodeMD5Raw(string value)
        {
            // Generate the hash code
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] byteValue = Encoding.ASCII.GetBytes(value);
            byte[] hashValue = md5.ComputeHash(byteValue);
            return hashValue;
        }

        /// <summary>
        /// Gets the SHA1 hashcode from the value.
        /// </summary>
        /// <param name="value">The value to generate the hash code for.</param>
        /// <returns>The generated hash code.</returns>
        public static string GetHashcodeSHA1(string value)
        {
            int i = 0;

            // Generate the hash code
            SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] byteValue = Encoding.ASCII.GetBytes(value);
            byte[] hashValue = sha1.ComputeHash(byteValue);

            // Get the string value of hashcode.
            string[] octetArrayByte = new string[hashValue.Count()];
            foreach (Byte item in hashValue)
                octetArrayByte[i++] = item.ToString("X2");

            // Create the octet string of bytes.
            string octetValue = String.Join("", octetArrayByte);
            return octetValue;
        }

        /// <summary>
        /// Gets the SHA1 hashcode from the value.
        /// </summary>
        /// <param name="value">The value to generate the hash code for.</param>
        /// <returns>The generated hash code.</returns>
        public static byte[] GetHashcodeSHA1Raw(string value)
        {
            // Generate the hash code
            SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] byteValue = Encoding.ASCII.GetBytes(value);
            byte[] hashValue = sha1.ComputeHash(byteValue);
            return hashValue;
        }

        /// <summary>
        /// Get the hashcode from the file.
        /// </summary>
        /// <param name="filename">The path and file name to generate the hash code for.</param>
        /// <param name="hashcodeType">The hash name.</param>
        /// <returns>The generated hash code.</returns>
        public static string GetHashcodeFile(string filename, Nequeo.Cryptography.HashcodeType hashcodeType)
        {
            FileStream file = null;
            byte[] hashValue = null;
            StringBuilder sb = null;

            try
            {
                // Open the file to generate the hash code for.
                using (file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Select the hash code type.
                    switch (hashcodeType)
                    {
                        case HashcodeType.MD5:
                            // MD5 hashcode.
                            MD5 md5 = new MD5CryptoServiceProvider();
                            hashValue = md5.ComputeHash(file);
                            break;

                        case HashcodeType.SHA1:
                            // SHA1 hashcode.
                            SHA1 sha1 = new SHA1CryptoServiceProvider();
                            hashValue = sha1.ComputeHash(file);
                            break;

                        case HashcodeType.SHA256:
                            // SHA256 hashcode.
                            SHA256 sha256 = new SHA256CryptoServiceProvider();
                            hashValue = sha256.ComputeHash(file);
                            break;

                        case HashcodeType.SHA384:
                            // SHA384 hashcode.
                            SHA384 sha384 = new SHA384CryptoServiceProvider();
                            hashValue = sha384.ComputeHash(file);
                            break;

                        case HashcodeType.SHA512:
                            // SHA512 hashcode.
                            SHA512 sha512 = new SHA512CryptoServiceProvider();
                            hashValue = sha512.ComputeHash(file);
                            break;
                    }

                    // Close the file.
                    file.Close();

                    // Get the hashcode bytes as hex-string.
                    sb = new StringBuilder();
                    for (int i = 0; i < hashValue.Length; i++)
                        sb.Append(hashValue[i].ToString("X2"));

                    // Return the hash code.
                    return sb.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Clean-up
                if (file != null)
                    file.Close();
            }
        }

        /// <summary>
        /// Get the hashcode from the file.
        /// </summary>
        /// <param name="filename">The path and file name to generate the hash code for.</param>
        /// <param name="hashcodeType">The hash name.</param>
        /// <returns>The generated hash code.</returns>
        public static byte[] GetHashcodeFileRaw(string filename, Nequeo.Cryptography.HashcodeType hashcodeType)
        {
            FileStream file = null;
            byte[] hashValue = null;

            try
            {
                // Open the file to generate the hash code for.
                using (file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Select the hash code type.
                    switch (hashcodeType)
                    {
                        case HashcodeType.MD5:
                            // MD5 hashcode.
                            MD5 md5 = new MD5CryptoServiceProvider();
                            hashValue = md5.ComputeHash(file);
                            break;

                        case HashcodeType.SHA1:
                            // SHA1 hashcode.
                            SHA1 sha1 = new SHA1CryptoServiceProvider();
                            hashValue = sha1.ComputeHash(file);
                            break;

                        case HashcodeType.SHA256:
                            // SHA256 hashcode.
                            SHA256 sha256 = new SHA256CryptoServiceProvider();
                            hashValue = sha256.ComputeHash(file);
                            break;

                        case HashcodeType.SHA384:
                            // SHA384 hashcode.
                            SHA384 sha384 = new SHA384CryptoServiceProvider();
                            hashValue = sha384.ComputeHash(file);
                            break;

                        case HashcodeType.SHA512:
                            // SHA512 hashcode.
                            SHA512 sha512 = new SHA512CryptoServiceProvider();
                            hashValue = sha512.ComputeHash(file);
                            break;
                    }

                    // Close the file.
                    file.Close();

                    // Return the hash code.
                    return hashValue;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Clean-up
                if (file != null)
                    file.Close();
            }
        }

        /// <summary>
        /// Get the MD5 hashcode from the file.
        /// </summary>
        /// <param name="filename">The path and file name to generate the hash code for.</param>
        /// <returns>The generated hash code.</returns>
        public static string GetHashcodeFileMD5(string filename)
        {
            FileStream file = null;
            byte[] hashValue = null;
            StringBuilder sb = null;

            try
            {
                // Open the file to generate the hash code for.
                using (file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // MD5 hashcode.
                    MD5 md5 = new MD5CryptoServiceProvider();
                    hashValue = md5.ComputeHash(file);

                    // Close the file.
                    file.Close();

                    // Get the hashcode bytes as hex-string.
                    sb = new StringBuilder();
                    for (int i = 0; i < hashValue.Length; i++)
                        sb.Append(hashValue[i].ToString("X2"));

                    // Return the hash code.
                    return sb.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Clean-up
                if (file != null)
                    file.Close();
            }
        }

        /// <summary>
        /// Get the MD5 hashcode from the file.
        /// </summary>
        /// <param name="filename">The path and file name to generate the hash code for.</param>
        /// <returns>The generated hash code.</returns>
        public static byte[] GetHashcodeFileMD5Raw(string filename)
        {
            FileStream file = null;
            byte[] hashValue = null;

            try
            {
                // Open the file to generate the hash code for.
                using (file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // MD5 hashcode.
                    MD5 md5 = new MD5CryptoServiceProvider();
                    hashValue = md5.ComputeHash(file);

                    // Close the file.
                    file.Close();

                    // Return the hash code.
                    return hashValue;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Clean-up
                if (file != null)
                    file.Close();
            }
        }

        /// <summary>
        /// Get the SHA1 hashcode from the file.
        /// </summary>
        /// <param name="filename">The path and file name to generate the hash code for.</param>
        /// <returns>The generated hash code.</returns>
        public static string GetHashcodeFileSHA1(string filename)
        {
            FileStream file = null;
            byte[] hashValue = null;
            StringBuilder sb = null;

            try
            {
                // Open the file to generate the hash code for.
                using (file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // SHA1 hashcode.
                    SHA1 sha1 = new SHA1CryptoServiceProvider();
                    hashValue = sha1.ComputeHash(file);

                    // Close the file.
                    file.Close();

                    // Get the hashcode bytes as hex-string.
                    sb = new StringBuilder();
                    for (int i = 0; i < hashValue.Length; i++)
                        sb.Append(hashValue[i].ToString("X2"));

                    // Return the hash code.
                    return sb.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Clean-up
                if (file != null)
                    file.Close();
            }
        }

        /// <summary>
        /// Get the SHA1 hashcode from the file.
        /// </summary>
        /// <param name="filename">The path and file name to generate the hash code for.</param>
        /// <returns>The generated hash code.</returns>
        public static byte[] GetHashcodeFileSHA1Raw(string filename)
        {
            FileStream file = null;
            byte[] hashValue = null;

            try
            {
                // Open the file to generate the hash code for.
                using (file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // SHA1 hashcode.
                    SHA1 sha1 = new SHA1CryptoServiceProvider();
                    hashValue = sha1.ComputeHash(file);

                    // Close the file.
                    file.Close();

                    // Return the hash code.
                    return hashValue;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Clean-up
                if (file != null)
                    file.Close();
            }
        }
        #endregion
    }

    /// <summary>
    /// Hashcode type.
    /// </summary>
    public enum HashcodeType
    {
        /// <summary>
        /// MD5.
        /// </summary>
        MD5 = 0,
        /// <summary>
        /// SHA1.
        /// </summary>
        SHA1 = 1,
        /// <summary>
        /// SHA256.
        /// </summary>
        SHA256 = 2,
        /// <summary>
        /// SHA384.
        /// </summary>
        SHA384 = 3,
        /// <summary>
        /// SHA512.
        /// </summary>
        SHA512 = 4
    }

    /// <summary>
    /// Cryptography type.
    /// </summary>
    public enum CryptographyType
    {
        /// <summary>
        /// No cryptography.
        /// </summary>
        None = 0,
        /// <summary>
        /// Encrypt.
        /// </summary>
        Encrypt = 1,
        /// <summary>
        /// Decrypt.
        /// </summary>
        Decrypt = 2,
    }

    /// <summary>
    /// Security type.
    /// </summary>
    public enum SecurityType
    {
        /// <summary>
        /// No security.
        /// </summary>
        None = 0,
        /// <summary>
        /// Passphase.
        /// </summary>
        Passphrase = 1,
        /// <summary>
        /// Certificate.
        /// </summary>
        Certificate = 2,
    }

    /// <summary>
    /// Describes the encryption format for storing passwords.
    /// </summary>
    public enum PasswordFormat
    {
        /// <summary>
        /// Passwords are not encrypted.
        /// </summary>
        Clear = 0,
        /// <summary>
        /// Passwords are encrypted one-way using the SHA hashing algorithm.
        /// </summary>
        Hashed = 1,
        /// <summary>
        /// Passwords are encrypted using the encryption settings determined.
        /// </summary>
        Encrypted = 2,
    }

    /// <summary>
    /// Basic tags for symmetric key algorithms.
    /// </summary>
    public enum SymmetricKeyAlgorithmType
    {
        /// <summary>
        /// Plaintext or unencrypted data
        /// </summary>
        Null = 0,
        /// <summary>
        /// IDEA [IDEA]
        /// </summary>
        Idea = 1,
        /// <summary>
        /// Triple-DES (DES-EDE, as per spec -168 bit key derived from 192)
        /// </summary>
        TripleDes = 2,
        /// <summary>
        /// Cast5 (128 bit key, as per RFC 2144)
        /// </summary>
        Cast5 = 3,
        /// <summary>
        /// Blowfish (128 bit key, 16 rounds) [Blowfish]
        /// </summary>
        Blowfish = 4,
        /// <summary>
        /// Safer-SK128 (13 rounds) [Safer]
        /// </summary>
        Safer = 5,
        /// <summary>
        /// Reserved for DES/SK
        /// </summary>
        Des = 6,
        /// <summary>
        /// Reserved for AES with 128-bit key
        /// </summary>
        Aes128 = 7,
        /// <summary>
        /// Reserved for AES with 192-bit key
        /// </summary>
        Aes192 = 8,
        /// <summary>
        /// Reserved for AES with 256-bit key
        /// </summary>
        Aes256 = 9,
        /// <summary>
        /// Reserved for Twofish
        /// </summary>
        Twofish = 10,
    }

    /// <summary>
    /// The cypher type.
    /// </summary>
    public enum CypherType
    {
        /// <summary>
        /// No cypher.
        /// </summary>
        None = 0,
        /// <summary>
        /// Triple-DES (DES-EDE, as per spec -168 bit key derived from 192).
        /// </summary>
        TripleDes = 1,
        /// <summary>
        /// Blowfish (128 bit key, 16 rounds) [Blowfish].
        /// </summary>
        Blowfish = 2,
        /// <summary>
        /// Reserved for Twofish.
        /// </summary>
        Twofish = 3,
        /// <summary>
        /// Reserved for AES with 256-bit key.
        /// </summary>
        Aes256 = 4,
        /// <summary>
        /// RC 6 key.
        /// </summary>
        RC6 = 5,
        /// <summary>
        /// Rijndael with 256-bit key.
        /// </summary>
        Rijndael = 6,
        /// <summary>
        /// Serpent key.
        /// </summary>
        Serpent = 7,
        /// <summary>
        /// RSA key.
        /// </summary>
        RSA = 8,
        /// <summary>
        /// Pretty good privacy cryptography key.
        /// </summary>
        PGP = 9,
    }
}
