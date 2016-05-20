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
    /// Advanced encryption decryption of data class.
    /// </summary>
    [Export(typeof(IAdvancedRSA))]
    public partial class AdvancedRSA : Nequeo.Runtime.DisposableBase, IDisposable, IAdvancedRSA
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly AdvancedRSA Instance = new AdvancedRSA();

        /// <summary>
        /// Static constructor
        /// </summary>
        static AdvancedRSA() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public AdvancedRSA()
        {
            OnCreated();
        }
        #endregion

        #region Private Constants
        private const int numberOfBytes = 1024;
        private const int validKeyLength = 24;
        private const int validVectorLength = 8;
        #endregion

        #region Private Fields
        // The cryptography key.
        private byte[] internalKey = new byte[] { 23, 76, 91, 34, 200, 213, 167, 96, 
            132, 104, 67, 34, 98, 234, 240, 56, 
            25, 78, 66, 113, 148, 156, 167, 251 };
        // The initializations vector.
        private byte[] internalIV = new byte[] { 17, 47, 67, 121, 168, 214, 232, 45 };
        #endregion

        #region Public Methods
        /// <summary>
        /// Encrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <returns>The encrypted bytes.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual Byte[] Encrypt(Byte[] data, string certificatePath)
        {
            // Make sure that the parameters passed
            // have an instance.
            if (data == null)
                throw new System.ArgumentNullException("Data can not be null.",
                    new System.Exception("Valid data has not been passed."));

            // A path is specified.
            if (String.IsNullOrEmpty(certificatePath))
                throw new System.ArgumentNullException("A certificate path has not been set.",
                    new System.Exception("A valid certificate path must be set."));

            // The x509 certificate reference.
            X509Certificate2 x509Certificate = null;

            try
            {
                // Create a new instance of the
                // x509 certificate.
                x509Certificate = new X509Certificate2(certificatePath);
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate file.");
            }

            // The RSA crytograph service provider.
            RSACryptoServiceProvider rsaProvider = null;
            rsaProvider = (RSACryptoServiceProvider)x509Certificate.PublicKey.Key;

            // Will contain encrypted data.
            byte[] encryptedBytes = null;

            try
            {
                // Encrypt the data.
                encryptedBytes = rsaProvider.Encrypt(data, false);
                return encryptedBytes;
            }
            catch
            {
                throw new CryptographicException("Unable to encrypt data.");
            }
        }

        /// <summary>
        /// Encrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <param name="password">The password used to unlock the certificate.</param>
        /// <returns>The encrypted bytes.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual Byte[] Encrypt(Byte[] data, string certificatePath, string password)
        {
            // Make sure that the parameters passed
            // have an instance.
            if (data == null)
                throw new System.ArgumentNullException("Data can not be null.",
                    new System.Exception("Valid data has not been passed."));

            // A path is specified.
            if (String.IsNullOrEmpty(certificatePath))
                throw new System.ArgumentNullException("A certificate path has not been set.",
                    new System.Exception("A valid certificate path must be set."));

            // The x509 certificate reference.
            X509Certificate2 x509Certificate = null;

            try
            {
                // Create a new instance of the
                // x509 certificate.
                x509Certificate = new X509Certificate2(certificatePath, password);
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate file.");
            }

            // The RSA crytograph service provider.
            RSACryptoServiceProvider rsaProvider = null;
            rsaProvider = (RSACryptoServiceProvider)x509Certificate.PublicKey.Key;

            // Will contain encrypted data.
            byte[] encryptedBytes = null;

            try
            {
                // Encrypt the data.
                encryptedBytes = rsaProvider.Encrypt(data, false);
                return encryptedBytes;
            }
            catch
            {
                throw new CryptographicException("Unable to encrypt data.");
            }
        }

        /// <summary>
        /// Encrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="x509Certificate">A certificate with a private key.</param>
        /// <returns>The encrypted bytes.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual Byte[] Encrypt(Byte[] data, X509Certificate2 x509Certificate)
        {
            // Make sure that the parameters passed
            // have an instance.
            if ((data == null) || (x509Certificate == null))
                throw new System.ArgumentNullException("Data and x509 Certificate can not be null.",
                    new System.Exception("A valid x509 certificate and data has not been passed."));

            RSACryptoServiceProvider rsaProvider = null;
            rsaProvider = (RSACryptoServiceProvider)x509Certificate.PublicKey.Key;

            // Will contain encrypted data.
            byte[] encryptedBytes = null;

            try
            {
                // Encrypt the data.
                encryptedBytes = rsaProvider.Encrypt(data, false);
                return encryptedBytes;
            }
            catch
            {
                throw new CryptographicException("Unable to encrypt data.");
            }
        }

        /// <summary>
        /// Encrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual bool Encrypt(string pathToDecryptedFile, string pathToEncryptedFile, string certificatePath)
        {
            FileStream encryptFile = null;
            FileStream decryptFile = null;

            try
            {
                // Create the file streams
                using(encryptFile = new FileStream(pathToEncryptedFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (decryptFile = new FileStream(pathToDecryptedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Get the decrypted data.
                    byte[] decryptedData = new byte[Convert.ToInt32(decryptFile.Length)];
                    int readDecryptedBytes = decryptFile.Read(decryptedData, 0, Convert.ToInt32(decryptFile.Length));

                    // Encrypt the data.
                    byte[] encryptedData = Encrypt(decryptedData, certificatePath);

                    // Write the encrypted data to the file.
                    encryptFile.Write(encryptedData, 0, encryptedData.Length);
                    encryptFile.Flush();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Unable to encrypt data.", ex);
            }
            finally
            {
                if (encryptFile != null)
                    encryptFile.Close();

                if (decryptFile != null)
                    decryptFile.Close();
            }
        }

        /// <summary>
        /// Encrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <param name="password">The password used to unlock the certificate.</param>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        /// <returns>True if no error has occurred else false.</returns>
        public virtual bool Encrypt(string pathToDecryptedFile, string pathToEncryptedFile, string certificatePath, string password)
        {
            FileStream encryptFile = null;
            FileStream decryptFile = null;

            try
            {
                // Create the file streams
                using(encryptFile = new FileStream(pathToEncryptedFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (decryptFile = new FileStream(pathToDecryptedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Get the decrypted data.
                    byte[] decryptedData = new byte[Convert.ToInt32(decryptFile.Length)];
                    int readDecryptedBytes = decryptFile.Read(decryptedData, 0, Convert.ToInt32(decryptFile.Length));

                    // Encrypt the data.
                    byte[] encryptedData = Encrypt(decryptedData, certificatePath, password);

                    // Write the encrypted data to the file.
                    encryptFile.Write(encryptedData, 0, encryptedData.Length);
                    encryptFile.Flush();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Unable to encrypt data.", ex);
            }
            finally
            {
                if (encryptFile != null)
                    encryptFile.Close();

                if (decryptFile != null)
                    decryptFile.Close();
            }
        }

        /// <summary>
        /// Encrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file that is to be encrypted.</param>
        /// <param name="pathToEncryptedFile">Path to the file where encryption is stored.</param>
        /// <param name="x509Certificate">A certificate with a private key.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual bool Encrypt(string pathToDecryptedFile, string pathToEncryptedFile, X509Certificate2 x509Certificate)
        {
            FileStream encryptFile = null;
            FileStream decryptFile = null;

            try
            {
                // Create the file streams
                using(encryptFile = new FileStream(pathToEncryptedFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (decryptFile = new FileStream(pathToDecryptedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Get the decrypted data.
                    byte[] decryptedData = new byte[Convert.ToInt32(decryptFile.Length)];
                    int readDecryptedBytes = decryptFile.Read(decryptedData, 0, Convert.ToInt32(decryptFile.Length));

                    // Encrypt the data.
                    byte[] encryptedData = Encrypt(decryptedData, x509Certificate);

                    // Write the encrypted data to the file.
                    encryptFile.Write(encryptedData, 0, encryptedData.Length);
                    encryptFile.Flush();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Unable to encrypt data.", ex);
            }
            finally
            {
                if (encryptFile != null)
                    encryptFile.Close();

                if (decryptFile != null)
                    decryptFile.Close();
            }
        }

        /// <summary>
        /// Decrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The encrypted data to decrypt.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <returns>The collection of decrypted data.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual Byte[] Decrypt(Byte[] data, string certificatePath)
        {
            // Make sure that the parameters passed
            // have an instance.
            if (data == null)
                throw new System.ArgumentNullException("Data can not be null.",
                    new System.Exception("Valid data has not been passed."));

            // A path is specified.
            if (String.IsNullOrEmpty(certificatePath))
                throw new System.ArgumentNullException("A certificate path has not been set.",
                    new System.Exception("A valid certificate path must be set."));

            // The x509 certificate reference.
            X509Certificate2 x509Certificate = null;

            try
            {
                // Create a new instance of the
                // x509 certificate.
                x509Certificate = new X509Certificate2(certificatePath);
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate file.");
            }

            // The RSA crytograph service provider.
            RSACryptoServiceProvider rsaProvider = null;

            // If the certificate contains a private key
            // then create a new RSA service provider from
            // the private key asymmetric algorithm.
            if (x509Certificate.HasPrivateKey)
                rsaProvider = (RSACryptoServiceProvider)x509Certificate.PrivateKey;
            else
                // If the certificate does not contain
                // a private key then throw exception.
                throw new CryptographicException("Private key not contained within certificate.");

            // Will contain decrypted data.
            byte[] decryptedBytes = null;

            try
            {
                // Encrypt the data.
                decryptedBytes = rsaProvider.Decrypt(data, false);
                return decryptedBytes;
            }
            catch
            {
                throw new CryptographicException("Unable to decrypt data.");
            }
        }

        /// <summary>
        /// Decrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The encrypted data to decrypt.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <param name="password">The password used to unlock the certificate.</param>
        /// <returns>The collection of decrypted data.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual Byte[] Decrypt(Byte[] data, string certificatePath, string password)
        {
            // Make sure that the parameters passed
            // have an instance.
            if (data == null)
                throw new System.ArgumentNullException("Data can not be null.",
                    new System.Exception("Valid data has not been passed."));

            // A path is specified.
            if (String.IsNullOrEmpty(certificatePath))
                throw new System.ArgumentNullException("A certificate path has not been set.",
                    new System.Exception("A valid certificate path must be set."));

            // The x509 certificate reference.
            X509Certificate2 x509Certificate = null;

            try
            {
                // Create a new instance of the
                // x509 certificate.
                x509Certificate = new X509Certificate2(certificatePath, password);
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate file.");
            }

            // The RSA crytograph service provider.
            RSACryptoServiceProvider rsaProvider = null;

            // If the certificate contains a private key
            // then create a new RSA service provider from
            // the private key asymmetric algorithm.
            if (x509Certificate.HasPrivateKey)
                rsaProvider = (RSACryptoServiceProvider)x509Certificate.PrivateKey;
            else
                // If the certificate does not contain
                // a private key then throw exception.
                throw new CryptographicException("Private key not contained within certificate.");

            // Will contain decrypted data.
            byte[] decryptedBytes = null;

            try
            {
                // Encrypt the data.
                decryptedBytes = rsaProvider.Decrypt(data, false);
                return decryptedBytes;
            }
            catch
            {
                throw new CryptographicException("Unable to decrypt data.");
            }
        }

        /// <summary>
        /// Decrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The encrypted data to decrypt.</param>
        /// <param name="x509Certificate">The certificate that contains a private key.</param>
        /// <returns>The collection of decrypted data.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual Byte[] Decrypt(Byte[] data, X509Certificate2 x509Certificate)
        {
            // Make sure that the parameters passed
            // have an instance.
            if ((data == null) || (x509Certificate == null))
                throw new System.ArgumentNullException("Data and x509 Certificate can not be null.",
                    new System.Exception("A valid x509 certificate and data has not been passed."));

            RSACryptoServiceProvider rsaProvider = null;

            // If the certificate contains a private key
            // then create a new RSA service provider from
            // the private key asymmetric algorithm.
            if (x509Certificate.HasPrivateKey)
                rsaProvider = (RSACryptoServiceProvider)x509Certificate.PrivateKey;
            else
                // If the certificate does not contain
                // a private key then throw exception.
                throw new CryptographicException("Private key not contained within certificate.");

            // Will contain decrypted data.
            byte[] decryptedBytes = null;

            try
            {
                // Encrypt the data.
                decryptedBytes = rsaProvider.Decrypt(data, false);
                return decryptedBytes;
            }
            catch
            {
                throw new CryptographicException("Unable to decrypt data.");
            }
        }

        /// <summary>
        /// Decrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual bool Decrypt(string pathToDecryptedFile, string pathToEncryptedFile, string certificatePath)
        {
            FileStream encryptFile = null;
            FileStream decryptFile = null;

            try
            {
                // Create the file streams
                using (encryptFile = new FileStream(pathToEncryptedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (decryptFile = new FileStream(pathToDecryptedFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    // Get the encrypted data.
                    byte[] encryptedData = new byte[Convert.ToInt32(encryptFile.Length)];
                    int readEncryptedBytes = encryptFile.Read(encryptedData, 0, Convert.ToInt32(encryptFile.Length));

                    // Decrypt the data.
                    byte[] decryptedData = Decrypt(encryptedData, certificatePath);

                    // Write the decrypted data to the file.
                    decryptFile.Write(decryptedData, 0, decryptedData.Length);
                    decryptFile.Flush();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Unable to decrypt data.", ex);
            }
            finally
            {
                if (encryptFile != null)
                    encryptFile.Close();

                if (decryptFile != null)
                    decryptFile.Close();
            }
        }

        /// <summary>
        /// Decrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <param name="password">The password used to unlock the certificate.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual bool Decrypt(string pathToDecryptedFile, string pathToEncryptedFile, string certificatePath, string password)
        {
            FileStream encryptFile = null;
            FileStream decryptFile = null;

            try
            {
                // Create the file streams
                using (encryptFile = new FileStream(pathToEncryptedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (decryptFile = new FileStream(pathToDecryptedFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    // Get the encrypted data.
                    byte[] encryptedData = new byte[Convert.ToInt32(encryptFile.Length)];
                    int readEncryptedBytes = encryptFile.Read(encryptedData, 0, Convert.ToInt32(encryptFile.Length));

                    // Decrypt the data.
                    byte[] decryptedData = Decrypt(encryptedData, certificatePath, password);

                    // Write the decrypted data to the file.
                    decryptFile.Write(decryptedData, 0, decryptedData.Length);
                    decryptFile.Flush();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Unable to decrypt data.", ex);
            }
            finally
            {
                if (encryptFile != null)
                    encryptFile.Close();

                if (decryptFile != null)
                    decryptFile.Close();
            }
        }

        /// <summary>
        /// Decrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="pathToDecryptedFile">Path to the file where decrypted data is stored.</param>
        /// <param name="pathToEncryptedFile">Path to the encrypted file.</param>
        /// <param name="x509Certificate">The certificate that contains a private key.</param>
        /// <returns>True if no error has occurred else false.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual bool Decrypt(string pathToDecryptedFile, string pathToEncryptedFile, X509Certificate2 x509Certificate)
        {
            FileStream encryptFile = null;
            FileStream decryptFile = null;

            try
            {
                // Create the file streams
                using (encryptFile = new FileStream(pathToEncryptedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (decryptFile = new FileStream(pathToDecryptedFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    // Get the encrypted data.
                    byte[] encryptedData = new byte[Convert.ToInt32(encryptFile.Length)];
                    int readEncryptedBytes = encryptFile.Read(encryptedData, 0, Convert.ToInt32(encryptFile.Length));

                    // Decrypt the data.
                    byte[] decryptedData = Decrypt(encryptedData, x509Certificate);

                    // Write the decrypted data to the file.
                    decryptFile.Write(decryptedData, 0, decryptedData.Length);
                    decryptFile.Flush();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Unable to decrypt data.", ex);
            }
            finally
            {
                if (encryptFile != null)
                    encryptFile.Close();

                if (decryptFile != null)
                    decryptFile.Close();
            }
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Encrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="x509Certificate">A certificate with a private key.</param>
        /// <returns>The encrypted bytes.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public static Byte[] EncryptData(Byte[] data, X509Certificate2 x509Certificate)
        {
            // Make sure that the parameters passed
            // have an instance.
            if ((data == null) || (x509Certificate == null))
                throw new System.ArgumentNullException("Data and x509 Certificate can not be null.",
                    new System.Exception("A valid x509 certificate and data has not been passed."));

            RSACryptoServiceProvider rsaProvider = null;
            rsaProvider = (RSACryptoServiceProvider)x509Certificate.PublicKey.Key;

            // Will contain encrypted data.
            byte[] encryptedBytes = null;

            try
            {
                // Encrypt the data.
                encryptedBytes = rsaProvider.Encrypt(data, false);
                return encryptedBytes;
            }
            catch
            {
                throw new CryptographicException("Unable to encrypt data.");
            }
        }

        /// <summary>
        /// Decrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="x509Certificate">A certificate with a private key.</param>
        /// <returns>The decrypted bytes.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public static Byte[] DecryptData(Byte[] data, X509Certificate2 x509Certificate)
        {
            // Make sure that the parameters passed
            // have an instance.
            if ((data == null) || (x509Certificate == null))
                throw new System.ArgumentNullException("Data and x509 Certificate can not be null.",
                    new System.Exception("A valid x509 certificate and data has not been passed."));

            RSACryptoServiceProvider rsaProvider = null;

            // If the certificate contains a private key
            // then create a new RSA service provider from
            // the private key asymmetric algorithm.
            if (x509Certificate.HasPrivateKey)
                rsaProvider = (RSACryptoServiceProvider)x509Certificate.PrivateKey;
            else
                // If the certificate does not contain
                // a private key then throw exception.
                throw new CryptographicException("Private key not contained within certificate.");

            // Will contain decrypted data.
            byte[] decryptedBytes = null;

            try
            {
                // Encrypt the data.
                decryptedBytes = rsaProvider.Decrypt(data, false);
                return decryptedBytes;
            }
            catch
            {
                throw new CryptographicException("Unable to decrypt data.");
            }
        }

        /// <summary>
        /// Create a RSA crypto service provider.
        /// </summary>
        /// <param name="cspParameters">The CSP parameters.</param>
        /// <returns>RSA crypto service provider.</returns>
        public static RSACryptoServiceProvider CreateRSACryptoServiceProvider(CspParameters cspParameters)
        {
            return new RSACryptoServiceProvider(cspParameters);
        }

        /// <summary>
        /// Create an RSA service provider with public private keys
        /// </summary>
        /// <param name="dwKeySize">The size of the key to use in bits.</param>
        /// <returns>RSA crypto service provider.</returns>
        public static RSACryptoServiceProvider CreatePublicPrivateKeyPair(int dwKeySize = 4096)
        {
            return new RSACryptoServiceProvider(dwKeySize);
        }

        /// <summary>
        /// Extract public key parameters from a certificate.
        /// </summary>
        /// <param name="x509Certificate">The certificate.</param>
        /// <returns>The certificate parameters.</returns>
        public static RSAParameters ExtractPublicKeyParameters(X509Certificate2 x509Certificate)
        {
            RSACryptoServiceProvider rsaProvider = (RSACryptoServiceProvider)x509Certificate.PublicKey.Key;
            RSAParameters rsaParam = rsaProvider.ExportParameters(false);
            return rsaParam;
        }

        /// <summary>
        /// Extract private key parameters from a certificate.
        /// </summary>
        /// <param name="x509Certificate">The certificate.</param>
        /// <returns>The certificate parameters.</returns>
        public static RSAParameters ExtractPrivateKeyParameters(X509Certificate2 x509Certificate)
        {
            RSACryptoServiceProvider rsaProvider = null;

            // If the certificate contains a private key
            // then create a new RSA service provider from
            // the private key asymmetric algorithm.
            if (x509Certificate.HasPrivateKey)
                rsaProvider = (RSACryptoServiceProvider)x509Certificate.PrivateKey;
            else
                // If the certificate does not contain
                // a private key then throw exception.
                throw new CryptographicException("Private key not contained within certificate.");

            RSAParameters rsaParam = rsaProvider.ExportParameters(true);
            return rsaParam;
        }

        /// <summary>
        /// Extract public key service provider from a certificate.
        /// </summary>
        /// <param name="x509Certificate">The certificate.</param>
        /// <returns>The certificate service provider.</returns>
        public static RSACryptoServiceProvider ExtractPublicKey(X509Certificate2 x509Certificate)
        {
            RSACryptoServiceProvider rsaProvider = (RSACryptoServiceProvider)x509Certificate.PublicKey.Key;
            return rsaProvider;
        }

        /// <summary>
        /// Extract private key service provider from a certificate.
        /// </summary>
        /// <param name="x509Certificate">The certificate.</param>
        /// <returns>The certificate service provider.</returns>
        public static RSACryptoServiceProvider ExtractPrivateKey(X509Certificate2 x509Certificate)
        {
            RSACryptoServiceProvider rsaProvider = null;

            // If the certificate contains a private key
            // then create a new RSA service provider from
            // the private key asymmetric algorithm.
            if (x509Certificate.HasPrivateKey)
                rsaProvider = (RSACryptoServiceProvider)x509Certificate.PrivateKey;
            else
                // If the certificate does not contain
                // a private key then throw exception.
                throw new CryptographicException("Private key not contained within certificate.");

            return rsaProvider;
        }

        /// <summary>
        /// Computes the hash value of the specified input stream using the specified
        /// hash algorithm, and signs the resulting hash value.
        /// </summary>
        /// <param name="inputStream">The input data for which to compute the hash.</param>
        /// <param name="rsaProvider">the RSA crypto service provider.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>The System.Security.Cryptography.RSA signature for the specified data.</returns>
        public static byte[] SignData(Stream inputStream, RSACryptoServiceProvider rsaProvider, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            byte[] signValue = null;

            // Select the hash code type.
            switch (hashAlgorithm)
            {
                case HashcodeType.MD5:
                    // MD5 hashcode.
                    MD5 md5 = new MD5CryptoServiceProvider();
                    signValue = rsaProvider.SignData(inputStream, md5);
                    break;

                case HashcodeType.SHA1:
                    // SHA1 hashcode.
                    SHA1 sha1 = new SHA1CryptoServiceProvider();
                    signValue = rsaProvider.SignData(inputStream, sha1);
                    break;

                case HashcodeType.SHA256:
                    // SHA256 hashcode.
                    SHA256 sha256 = new SHA256CryptoServiceProvider();
                    signValue = rsaProvider.SignData(inputStream, sha256);
                    break;

                case HashcodeType.SHA384:
                    // SHA384 hashcode.
                    SHA384 sha384 = new SHA384CryptoServiceProvider();
                    signValue = rsaProvider.SignData(inputStream, sha384);
                    break;

                case HashcodeType.SHA512:
                    // SHA512 hashcode.
                    SHA512 sha512 = new SHA512CryptoServiceProvider();
                    signValue = rsaProvider.SignData(inputStream, sha512);
                    break;
            }

            // Return the signed value.
            return signValue;
        }

        /// <summary>
        /// Verifies that a digital signature is valid by determining the hash value
        /// in the signature using the provided public key and comparing it to the hash
        /// value of the provided data.
        /// </summary>
        /// <param name="buffer">The data that was signed.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <param name="rsaProvider">the RSA crypto service provider.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>True if the signature is valid; otherwise, false.</returns>
        public static bool VerifyData(byte[] buffer, byte[] signature, RSACryptoServiceProvider rsaProvider, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            bool dataVerified = false;

            // Select the hash code type.
            switch (hashAlgorithm)
            {
                case HashcodeType.MD5:
                    // MD5 hashcode.
                    MD5 md5 = new MD5CryptoServiceProvider();
                    dataVerified = rsaProvider.VerifyData(buffer, md5, signature);
                    break;

                case HashcodeType.SHA1:
                    // SHA1 hashcode.
                    SHA1 sha1 = new SHA1CryptoServiceProvider();
                    dataVerified = rsaProvider.VerifyData(buffer, sha1, signature);
                    break;

                case HashcodeType.SHA256:
                    // SHA256 hashcode.
                    SHA256 sha256 = new SHA256CryptoServiceProvider();
                    dataVerified = rsaProvider.VerifyData(buffer, sha256, signature);
                    break;

                case HashcodeType.SHA384:
                    // SHA384 hashcode.
                    SHA384 sha384 = new SHA384CryptoServiceProvider();
                    dataVerified = rsaProvider.VerifyData(buffer, sha384, signature);
                    break;

                case HashcodeType.SHA512:
                    // SHA512 hashcode.
                    SHA512 sha512 = new SHA512CryptoServiceProvider();
                    dataVerified = rsaProvider.VerifyData(buffer, sha512, signature);
                    break;
            }

            // Return the result.
            return dataVerified;
        }
        #endregion
    }

    /// <summary>
    /// Advanced encryption decryption of data class.
    /// </summary>
    public interface IAdvancedRSA
    {
        #region Public Methods
        /// <summary>
        /// Encrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <returns>The encrypted bytes.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        Byte[] Encrypt(Byte[] data, string certificatePath);

        /// <summary>
        /// Encrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <param name="password">The password used to unlock the certificate.</param>
        /// <returns>The encrypted bytes.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        Byte[] Encrypt(Byte[] data, string certificatePath, string password);

        /// <summary>
        /// Encrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="x509Certificate">A certificate with a private key.</param>
        /// <returns>The encrypted bytes.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        Byte[] Encrypt(Byte[] data, X509Certificate2 x509Certificate);

        /// <summary>
        /// Decrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The encrypted data to decrypt.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <returns>The collection of decrypted data.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        Byte[] Decrypt(Byte[] data, string certificatePath);

        /// <summary>
        /// Decrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The encrypted data to decrypt.</param>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <param name="password">The password used to unlock the certificate.</param>
        /// <returns>The collection of decrypted data.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        Byte[] Decrypt(Byte[] data, string certificatePath, string password);

        /// <summary>
        /// Decrypts the data with the private key within the x509 certificate.
        /// </summary>
        /// <param name="data">The encrypted data to decrypt.</param>
        /// <param name="x509Certificate">The certificate that contains a private key.</param>
        /// <returns>The collection of decrypted data.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        Byte[] Decrypt(Byte[] data, X509Certificate2 x509Certificate);

        #endregion
    }
}
