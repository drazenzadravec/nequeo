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
    /// Public private key certificate authority cryptography provider.
    /// </summary>
    public class CertificateAuthorityKey
    {
        /// <summary>
        /// Public private key pair cryptography provider.
        /// </summary>
        public CertificateAuthorityKey() { }

        /// <summary>
        /// Get the RSA crypto service provider for the CA public key.
        /// </summary>
        /// <param name="publicKey">The stream containing the public key data.</param>
        /// <param name="password">The password used to decrypt the key within the file.</param>
        /// <returns>The RSA cryto service provider with the public key.</returns>
        public RSACryptoServiceProvider PublicKeyProvider(StreamReader publicKey, string password = null)
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
            Cryptography.Key.X509.X509Certificate x509Certificate = (Cryptography.Key.X509.X509Certificate)publicKeyReader.ReadObject();

            // Get the ras key parameters
            Cryptography.Key.Crypto.Parameters.RsaKeyParameters rsaPublicKey = (Cryptography.Key.Crypto.Parameters.RsaKeyParameters)x509Certificate.GetPublicKey();

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
        /// Get the RSA crypto service provider for the CA private key.
        /// </summary>
        /// <param name="privateKey">The stream containing the private key data.</param>
        /// <param name="password">The password used to decrypt the key within the file.</param>
        /// <returns>The RSA cryto service provider with the private key.</returns>
        public RSACryptoServiceProvider PrivateKeyProvider(StreamReader privateKey, string password = null)
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
            Key.Crypto.Parameters.RsaPrivateCrtKeyParameters rsaPrivateKey = (Key.Crypto.Parameters.RsaPrivateCrtKeyParameters)privateKeyReader.ReadObject();

            // Assign the rsa parameters.
            RSAParameters rsaPrivateParam = new RSAParameters();
            rsaPrivateParam.D = rsaPrivateKey.Exponent.ToByteArrayUnsigned();
            rsaPrivateParam.DP = rsaPrivateKey.DP.ToByteArrayUnsigned();
            rsaPrivateParam.DQ = rsaPrivateKey.DQ.ToByteArrayUnsigned();
            rsaPrivateParam.InverseQ = rsaPrivateKey.QInv.ToByteArrayUnsigned();
            rsaPrivateParam.P = rsaPrivateKey.P.ToByteArrayUnsigned();
            rsaPrivateParam.Q = rsaPrivateKey.Q.ToByteArrayUnsigned();
            rsaPrivateParam.Modulus = rsaPrivateKey.Modulus.ToByteArrayUnsigned();
            rsaPrivateParam.Exponent = rsaPrivateKey.PublicExponent.ToByteArrayUnsigned();

            // Create the encyption provider.
            RSACryptoServiceProvider rsaDecryptProvider = new RSACryptoServiceProvider();
            rsaDecryptProvider.ImportParameters(rsaPrivateParam);

            // Return the rsa provider.
            return rsaDecryptProvider;
        }
    }
}
