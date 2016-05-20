/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using System.ComponentModel.Composition;

namespace Nequeo.Cryptography
{
    /// <summary>
    /// Elliptic Curve Digital Signature Algorithm (ECDSA) for Cryptography Next Generation (CNG) implementation.
    /// </summary>
    public class EllipticCurve
    {
        /// <summary>
        /// Elliptic Curve Digital Signature Algorithm (ECDSA) for Cryptography Next Generation (CNG) implementation.
        /// </summary>
        public EllipticCurve() { }

        /// <summary>
        /// Computes the hash value of the specified input stream using the specified
        /// hash algorithm, and signs the resulting hash value.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="key">Cryptography Next Generation (CNG) objects key.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>The signature for the specified data.</returns>
        public byte[] SignData(byte[] data, CngKey key, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            // Create an ECDSA.
            using (ECDsaCng dsa = new ECDsaCng(key))
            {
                // Sign the data.
                dsa.HashAlgorithm = GetAlgorithm(hashAlgorithm);
                byte[] signature = dsa.SignData(data);
                return signature;
            }
        }

        /// <summary>
        /// Computes the hash value of the specified input stream using the specified
        /// hash algorithm, and signs the resulting hash value.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="key">Cryptography Next Generation (CNG) objects key.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>The signature for the specified data.</returns>
        public byte[] SignData(Stream data, CngKey key, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            // Create an ECDSA.
            using (ECDsaCng dsa = new ECDsaCng(key))
            {
                // Sign the data.
                dsa.HashAlgorithm = GetAlgorithm(hashAlgorithm);
                byte[] signature = dsa.SignData(data);
                return signature;
            }
        }

        /// <summary>
        /// Computes the hash value of the specified input stream using the specified
        /// hash algorithm, and signs the resulting hash value.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="cert">x509 certificate.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>The signature for the specified data.</returns>
        public byte[] SignData(byte[] data, X509Certificate2 cert, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            // Create an ECDSA.
            using (ECDsa privateKey = cert.GetECDsaPrivateKey())
            {
                // Sign the data.
                byte[] signature = privateKey.SignData(data, GetAlgorithmName(hashAlgorithm));
                return signature;
            }
        }

        /// <summary>
        /// Computes the hash value of the specified input stream using the specified
        /// hash algorithm, and signs the resulting hash value.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="cert">x509 certificate.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>The signature for the specified data.</returns>
        public byte[] SignData(Stream data, X509Certificate2 cert, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            // Create an ECDSA.
            using (ECDsa privateKey = cert.GetECDsaPrivateKey())
            {
                // Sign the data.
                byte[] signature = privateKey.SignData(data, GetAlgorithmName(hashAlgorithm));
                return signature;
            }
        }

        /// <summary>
        /// Computes the hash value of the specified input stream using the specified
        /// hash algorithm, and signs the resulting hash value.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="privateKey">ECDsa private key.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>The signature for the specified data.</returns>
        public byte[] SignData(byte[] data, ECDsa privateKey, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            return privateKey.SignData(data, GetAlgorithmName(hashAlgorithm));
        }

        /// <summary>
        /// Computes the hash value of the specified input stream using the specified
        /// hash algorithm, and signs the resulting hash value.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="privateKey">ECDsa private key.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>The signature for the specified data.</returns>
        public byte[] SignData(Stream data, ECDsa privateKey, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            return privateKey.SignData(data, GetAlgorithmName(hashAlgorithm));
        }

        /// <summary>
        /// Verifies that a digital signature is valid by determining the hash value
        /// in the signature using the provided public key and comparing it to the hash
        /// value of the provided data.
        /// </summary>
        /// <param name="data">The data that was signed.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <param name="key">Cryptography Next Generation (CNG) objects key.</param>
        /// <returns>True if the signature is valid; otherwise, false.</returns>
        public bool VerifyData(byte[] data, byte[] signature, CngKey key)
        {
            // Create an ECDSA.
            using (ECDsaCng dsa = new ECDsaCng(key))
            {
                // Verify the data.
                if (dsa.VerifyData(data, signature))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Verifies that a digital signature is valid by determining the hash value
        /// in the signature using the provided public key and comparing it to the hash
        /// value of the provided data.
        /// </summary>
        /// <param name="data">The data that was signed.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <param name="key">Cryptography Next Generation (CNG) objects key.</param>
        /// <returns>True if the signature is valid; otherwise, false.</returns>
        public bool VerifyData(Stream data, byte[] signature, CngKey key)
        {
            // Create an ECDSA.
            using (ECDsaCng dsa = new ECDsaCng(key))
            {
                // Verify the data.
                if (dsa.VerifyData(data, signature))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Verifies that a digital signature is valid by determining the hash value
        /// in the signature using the provided public key and comparing it to the hash
        /// value of the provided data.
        /// </summary>
        /// <param name="data">The data that was signed.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <param name="cert">x509 certificate.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>True if the signature is valid; otherwise, false.</returns>
        public bool VerifyData(byte[] data, byte[] signature, X509Certificate2 cert, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            // Create an ECDSA.
            using (ECDsa publicKey = cert.GetECDsaPublicKey())
            {
                // Verify the data.
                if (publicKey.VerifyData(data, signature, GetAlgorithmName(hashAlgorithm)))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Verifies that a digital signature is valid by determining the hash value
        /// in the signature using the provided public key and comparing it to the hash
        /// value of the provided data.
        /// </summary>
        /// <param name="data">The data that was signed.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <param name="cert">x509 certificate.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>True if the signature is valid; otherwise, false.</returns>
        public bool VerifyData(Stream data, byte[] signature, X509Certificate2 cert, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            // Create an ECDSA.
            using (ECDsa publicKey = cert.GetECDsaPublicKey())
            {
                // Verify the data.
                if (publicKey.VerifyData(data, signature, GetAlgorithmName(hashAlgorithm)))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Verifies that a digital signature is valid by determining the hash value
        /// in the signature using the provided public key and comparing it to the hash
        /// value of the provided data.
        /// </summary>
        /// <param name="data">The data that was signed.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <param name="publicKey">ECDsa public key.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>True if the signature is valid; otherwise, false.</returns>
        public bool VerifyData(byte[] data, byte[] signature, ECDsa publicKey, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            // Verify the data.
            if (publicKey.VerifyData(data, signature, GetAlgorithmName(hashAlgorithm)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Verifies that a digital signature is valid by determining the hash value
        /// in the signature using the provided public key and comparing it to the hash
        /// value of the provided data.
        /// </summary>
        /// <param name="data">The data that was signed.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <param name="publicKey">ECDsa public key.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>True if the signature is valid; otherwise, false.</returns>
        public bool VerifyData(Stream data, byte[] signature, ECDsa publicKey, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            // Verify the data.
            if (publicKey.VerifyData(data, signature, GetAlgorithmName(hashAlgorithm)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get the CNG Algorithm.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>CNG Algorithm.</returns>
        private CngAlgorithm GetAlgorithm(Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            switch (hashAlgorithm)
            {
                case HashcodeType.MD5:
                    return CngAlgorithm.MD5;
                case HashcodeType.SHA1:
                    return CngAlgorithm.Sha1;
                case HashcodeType.SHA256:
                    return CngAlgorithm.Sha256;
                case HashcodeType.SHA384:
                    return CngAlgorithm.Sha384;
                default:
                case HashcodeType.SHA512:
                    return CngAlgorithm.Sha512;
            }
        }

        /// <summary>
        /// Get the CNG Algorithm.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>CNG Algorithm.</returns>
        private HashAlgorithmName GetAlgorithmName(Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            switch (hashAlgorithm)
            {
                case HashcodeType.MD5:
                    return HashAlgorithmName.MD5;
                case HashcodeType.SHA1:
                    return HashAlgorithmName.SHA1;
                case HashcodeType.SHA256:
                    return HashAlgorithmName.SHA256;
                case HashcodeType.SHA384:
                    return HashAlgorithmName.SHA384;
                default:
                case HashcodeType.SHA512:
                    return HashAlgorithmName.SHA512;
            }
        }
    }
}
