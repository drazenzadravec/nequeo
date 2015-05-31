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

using Nequeo.Cryptography.Key;

namespace Nequeo.Cryptography.Openssl
{
    /// <summary>
    /// Public private key pair cryptography provider.
    /// </summary>
    public class PublicPrivateKey
    {
        /// <summary>
        /// Public private key pair cryptography provider.
        /// </summary>
        public PublicPrivateKey() { }

        /// <summary>
        /// Get the RSA crypto service provider for encryption with the public key.
        /// </summary>
        /// <param name="publicKey">The stream containing the public key data.</param>
        /// <param name="password">The password used to decrypt the key within the file.</param>
        /// <returns>The RSA cryto service provider with the public key.</returns>
        public RSACryptoServiceProvider PublicKeyEncryptionProvider(StreamReader publicKey, string password = null)
        {
            Key.OpenSsl.PemReader publicKeyReader = null;

            if (String.IsNullOrEmpty(password))
            {
                // Read the public key file.
                publicKeyReader = new Key.OpenSsl.PemReader(publicKey);
            }
            else
            {
                // Read the public key file.
                publicKeyReader = new Key.OpenSsl.PemReader(publicKey, new PasswordFinder(password));
            }

            // Get the ras key parameters
            Key.Crypto.Parameters.RsaKeyParameters rsaPublicKey = (Key.Crypto.Parameters.RsaKeyParameters)publicKeyReader.ReadObject();

            // Assign the rsa parameters.
            RSAParameters rsaPublicParam = new RSAParameters();
            rsaPublicParam.Exponent = rsaPublicKey.Exponent.ToByteArrayUnsigned();
            rsaPublicParam.Modulus = rsaPublicKey.Modulus.ToByteArrayUnsigned();

            // Create the encyption provider.
            RSACryptoServiceProvider rsaEncryptProvider = new RSACryptoServiceProvider();
            rsaEncryptProvider.ImportParameters(rsaPublicParam);

            // Return the rsa provider.
            return rsaEncryptProvider;
        }

        /// <summary>
        /// Get the RSA crypto service provider for decryption with the private key.
        /// </summary>
        /// <param name="privateKey">The stream containing the private key data.</param>
        /// <param name="password">The password used to decrypt the key within the file.</param>
        /// <returns>The RSA cryto service provider with the private key.</returns>
        public RSACryptoServiceProvider PrivateKeyDecryptionProvider(StreamReader privateKey, string password = null)
        {
            Key.OpenSsl.PemReader privateKeyReader = null;

            if (String.IsNullOrEmpty(password))
            {
                // Read the private key file.
                privateKeyReader = new Key.OpenSsl.PemReader(privateKey);
            }
            else
            {
                // Read the private key file.
                privateKeyReader = new Key.OpenSsl.PemReader(privateKey, new PasswordFinder(password));
            }

            // Get the rsa key parameters
            Key.Crypto.AsymmetricCipherKeyPair rsaPrivateKey = (Key.Crypto.AsymmetricCipherKeyPair)privateKeyReader.ReadObject();

            // Assign the rsa parameters.
            RSAParameters rsaPrivateParam = new RSAParameters();
            Key.Crypto.Parameters.RsaKeyParameters rsaPrivatePublic = (Key.Crypto.Parameters.RsaKeyParameters)rsaPrivateKey.Public;
            Key.Crypto.Parameters.RsaPrivateCrtKeyParameters rsaCrtPrivateParam = (Key.Crypto.Parameters.RsaPrivateCrtKeyParameters)rsaPrivateKey.Private;

            rsaPrivateParam.D = rsaCrtPrivateParam.Exponent.ToByteArrayUnsigned();
            rsaPrivateParam.DP = rsaCrtPrivateParam.DP.ToByteArrayUnsigned();
            rsaPrivateParam.DQ = rsaCrtPrivateParam.DQ.ToByteArrayUnsigned();
            rsaPrivateParam.InverseQ = rsaCrtPrivateParam.QInv.ToByteArrayUnsigned();
            rsaPrivateParam.P = rsaCrtPrivateParam.P.ToByteArrayUnsigned();
            rsaPrivateParam.Q = rsaCrtPrivateParam.Q.ToByteArrayUnsigned();
            rsaPrivateParam.Modulus = rsaCrtPrivateParam.Modulus.ToByteArrayUnsigned();
            rsaPrivateParam.Exponent = rsaCrtPrivateParam.PublicExponent.ToByteArrayUnsigned();

            // Create the encyption provider.
            RSACryptoServiceProvider rsaDecryptProvider = new RSACryptoServiceProvider();
            rsaDecryptProvider.ImportParameters(rsaPrivateParam);

            // Return the rsa provider.
            return rsaDecryptProvider;
        }

        /// <summary>
        /// Get the RSA crypto service provider.
        /// </summary>
        /// <param name="publicPrivateKey">The stream containing the public and private key data.</param>
        /// <param name="password">The password used to decrypt the key within the file.</param>
        /// <returns>The RSA cryto service provider.</returns>
        public RSACryptoServiceProvider RSAProvider(StreamReader publicPrivateKey, string password = null)
        {
            Key.OpenSsl.PemReader keyReader = null;

            if (String.IsNullOrEmpty(password))
            {
                // Read the key file.
                keyReader = new Key.OpenSsl.PemReader(publicPrivateKey);
            }
            else
            {
                // Read the key file.
                keyReader = new Key.OpenSsl.PemReader(publicPrivateKey, new PasswordFinder(password));
            }

            // Get the ras key parameters
            Key.Crypto.AsymmetricCipherKeyPair rsaPrivateKey = (Key.Crypto.AsymmetricCipherKeyPair)keyReader.ReadObject();

            // Assign the rsa parameters.
            RSAParameters rsaParam = new RSAParameters();
            Key.Crypto.Parameters.RsaKeyParameters rsaPrivatePublic = (Key.Crypto.Parameters.RsaKeyParameters)rsaPrivateKey.Public;
            Key.Crypto.Parameters.RsaPrivateCrtKeyParameters rsaCrtPrivateParam = (Key.Crypto.Parameters.RsaPrivateCrtKeyParameters)rsaPrivateKey.Private;

            rsaParam.D = rsaCrtPrivateParam.Exponent.ToByteArrayUnsigned();
            rsaParam.DP = rsaCrtPrivateParam.DP.ToByteArrayUnsigned();
            rsaParam.DQ = rsaCrtPrivateParam.DQ.ToByteArrayUnsigned();
            rsaParam.InverseQ = rsaCrtPrivateParam.QInv.ToByteArrayUnsigned();
            rsaParam.P = rsaCrtPrivateParam.P.ToByteArrayUnsigned();
            rsaParam.Q = rsaCrtPrivateParam.Q.ToByteArrayUnsigned();
            rsaParam.Modulus = rsaCrtPrivateParam.Modulus.ToByteArrayUnsigned();
            rsaParam.Exponent = rsaCrtPrivateParam.PublicExponent.ToByteArrayUnsigned();

            // Create the encyption provider.
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.ImportParameters(rsaParam);

            // Return the rsa provider.
            return rsaProvider;
        }

        /// <summary>
        /// Generate a public private key pair RSA crypto service provider.
        /// </summary>
        /// <param name="publicExponent">The public exponent (e; the public key is now represented as {e, n}).</param>
        /// <param name="strength">The strength of the cipher.</param>
        /// <returns>The RSA cryto service provider.</returns>
        public RSACryptoServiceProvider Generate(long publicExponent = 3, int strength = 4096)
        {
            // Create the rsa key paramaters from the strength and public exponent.
            Key.Crypto.Generators.RsaKeyPairGenerator keyPair = new Key.Crypto.Generators.RsaKeyPairGenerator();
            Key.Crypto.Parameters.RsaKeyGenerationParameters keyPairParam = 
                new Key.Crypto.Parameters.RsaKeyGenerationParameters(
                    Key.Math.BigInteger.ValueOf(publicExponent), new Key.Security.SecureRandom(), strength, 25);

            // Initialise the parameters and generate the public private key pair.
            keyPair.Init(keyPairParam);
            Key.Crypto.AsymmetricCipherKeyPair rsaKeyPair = keyPair.GenerateKeyPair();

            // Assign the rsa parameters.
            RSAParameters rsaParam = new RSAParameters();
            Key.Crypto.Parameters.RsaKeyParameters rsaPrivatePublic = (Key.Crypto.Parameters.RsaKeyParameters)rsaKeyPair.Public;
            Key.Crypto.Parameters.RsaPrivateCrtKeyParameters rsaCrtPrivateParam = (Key.Crypto.Parameters.RsaPrivateCrtKeyParameters)rsaKeyPair.Private;

            rsaParam.D = rsaCrtPrivateParam.Exponent.ToByteArrayUnsigned();
            rsaParam.DP = rsaCrtPrivateParam.DP.ToByteArrayUnsigned();
            rsaParam.DQ = rsaCrtPrivateParam.DQ.ToByteArrayUnsigned();
            rsaParam.InverseQ = rsaCrtPrivateParam.QInv.ToByteArrayUnsigned();
            rsaParam.P = rsaCrtPrivateParam.P.ToByteArrayUnsigned();
            rsaParam.Q = rsaCrtPrivateParam.Q.ToByteArrayUnsigned();
            rsaParam.Modulus = rsaCrtPrivateParam.Modulus.ToByteArrayUnsigned();
            rsaParam.Exponent = rsaCrtPrivateParam.PublicExponent.ToByteArrayUnsigned();

            // Create the encyption provider.
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.ImportParameters(rsaParam);

            // Return the rsa provider.
            return rsaProvider;
        }

        /// <summary>
        /// Encrypt the data with the public key.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="encryptionProvider">The RSA cryto service provider with the public key.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] Encrypt(byte[] data, RSACryptoServiceProvider encryptionProvider)
        {
            return encryptionProvider.Encrypt(data, false);
        }

        /// <summary>
        /// Decrypt the data with the private key.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="decryptionProvider">The RSA cryto service provider with the private key.</param>
        /// <returns>The decrypted data.</returns>
        public byte[] Decrypt(byte[] data, RSACryptoServiceProvider decryptionProvider)
        {
            return decryptionProvider.Decrypt(data, false);
        }

        /// <summary>
        /// Computes the hash value of the specified input stream using the specified
        /// hash algorithm, and signs the resulting hash value.
        /// </summary>
        /// <param name="inputStream">The input data for which to compute the hash.</param>
        /// <param name="rsaProvider">the RSA crypto service provider.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>The System.Security.Cryptography.RSA signature for the specified data.</returns>
        public byte[] SignData(Stream inputStream, RSACryptoServiceProvider rsaProvider, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
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
        public bool VerifyData(byte[] buffer, byte[] signature, RSACryptoServiceProvider rsaProvider, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
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
    }
}
