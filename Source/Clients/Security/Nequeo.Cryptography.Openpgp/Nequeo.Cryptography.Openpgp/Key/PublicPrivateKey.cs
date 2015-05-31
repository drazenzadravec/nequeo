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
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Cryptography.Key;

namespace Nequeo.Cryptography.Openpgp
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
        /// Public and secret key provider.
        /// </summary>
        /// <param name="publicKey">The public key data.</param>
        /// <param name="secretKey">The secret key data.</param>
        /// <param name="keyID">The unique key id of the public secret key pair.</param>
        /// <param name="password">The password used to protect the secret key.</param>
        /// <returns>The RSA cryto service provider.</returns>
        public RSACryptoServiceProvider PublicKeySecretKeyProvider(System.IO.Stream publicKey, System.IO.Stream secretKey, long keyID, string password = null)
        {
            // Read the public key data.
            Key.Bcpg.OpenPgp.PgpPublicKey pgpPublicKey = ReadPublicKey(publicKey);

            // Find the secret key
            Key.Bcpg.OpenPgp.PgpPrivateKey privateKey = null;
            Key.Bcpg.OpenPgp.PgpSecretKeyRingBundle secretKeyRingBundle =
                new Key.Bcpg.OpenPgp.PgpSecretKeyRingBundle(Key.Bcpg.OpenPgp.PgpUtilities.GetDecoderStream(secretKey));

            // Find the private key (secret key).
            privateKey = FindSecretKey(secretKeyRingBundle, keyID, password.ToArray());

            // Assign the rsa parameters.
            RSAParameters rsaPrivateParam = new RSAParameters();
            Key.Crypto.Parameters.RsaKeyParameters rsaPrivatePublic = (Key.Crypto.Parameters.RsaKeyParameters)pgpPublicKey.GetKey();
            Key.Crypto.Parameters.RsaPrivateCrtKeyParameters rsaCrtPrivateParam = (Key.Crypto.Parameters.RsaPrivateCrtKeyParameters)privateKey.Key;

            // Assign the rsa parameters.
            rsaPrivateParam.D = rsaCrtPrivateParam.Exponent.ToByteArrayUnsigned();
            rsaPrivateParam.DP = rsaCrtPrivateParam.DP.ToByteArrayUnsigned();
            rsaPrivateParam.DQ = rsaCrtPrivateParam.DQ.ToByteArrayUnsigned();
            rsaPrivateParam.InverseQ = rsaCrtPrivateParam.QInv.ToByteArrayUnsigned();
            rsaPrivateParam.P = rsaCrtPrivateParam.P.ToByteArrayUnsigned();
            rsaPrivateParam.Q = rsaCrtPrivateParam.Q.ToByteArrayUnsigned();
            rsaPrivateParam.Modulus = rsaPrivatePublic.Modulus.ToByteArrayUnsigned();
            rsaPrivateParam.Exponent = rsaPrivatePublic.Exponent.ToByteArrayUnsigned();

            // Create the encyption provider.
            RSACryptoServiceProvider rsaEncryptProvider = new RSACryptoServiceProvider();
            rsaEncryptProvider.ImportParameters(rsaPrivateParam);

            // Return the rsa provider.
            return rsaEncryptProvider;
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

        /// <summary>
        /// Search a secret key ring collection for a secret key corresponding to keyID if it exists.
        /// </summary>
        /// <param name="pgpSec">A secret key ring collection</param>
        /// <param name="keyID">The keyID we want.</param>
        /// <param name="password">The passphrase to decrypt secret key with.</param>
        /// <returns>The private key.</returns>
        internal static Key.Bcpg.OpenPgp.PgpPrivateKey FindSecretKey(Key.Bcpg.OpenPgp.PgpSecretKeyRingBundle pgpSec, long keyID, char[] password)
        {
            // Get the secret key from the unique key id.
            Key.Bcpg.OpenPgp.PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(keyID);

            // If a secret key does not exist.
            if (pgpSecKey == null)
                return null;

            // Extract the private key.
            return pgpSecKey.ExtractPrivateKey(password);
        }

        /// <summary>
        /// A simple routine that opens a key ring file and loads the first available key suitable for encryption.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <returns>The public key data.</returns>
        internal static Key.Bcpg.OpenPgp.PgpPublicKey ReadPublicKey(System.IO.Stream publicKey)
        {
            Key.Bcpg.OpenPgp.PgpPublicKeyRingBundle pgpPub = new Key.Bcpg.OpenPgp.PgpPublicKeyRingBundle(
                Key.Bcpg.OpenPgp.PgpUtilities.GetDecoderStream(publicKey));

            // Loop through the collection till we find a key suitable for encryption
            foreach (Key.Bcpg.OpenPgp.PgpPublicKeyRing keyRing in pgpPub.GetKeyRings())
            {
                foreach (Key.Bcpg.OpenPgp.PgpPublicKey key in keyRing.GetPublicKeys())
                {
                    if (key.IsEncryptionKey)
                    {
                        // Return the key.
                        return key;
                    }
                }
            }

            // If no key data has been found.
            throw new ArgumentException("Can't find encryption key in key ring.");
        }
    }
}