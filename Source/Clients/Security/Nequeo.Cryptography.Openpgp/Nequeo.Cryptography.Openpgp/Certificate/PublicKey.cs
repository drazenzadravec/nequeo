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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using Nequeo.Cryptography.Key;

namespace Nequeo.Cryptography.Openpgp
{
    /// <summary>
    /// Public key.
    /// </summary>
    public class PublicKey
    {
        private Key.Bcpg.OpenPgp.PgpPublicKey _pgpPublicKey = null;

        /// <summary>
        /// Gets or sets the pgp public key.
        /// </summary>
        internal Key.Bcpg.OpenPgp.PgpPublicKey PgpPublicKey 
        {
            get { return _pgpPublicKey; }
            set { _pgpPublicKey = value; }
        }

        /// <summary>
        /// Gets the public key algorithm type.
        /// </summary>
        public PublicKeyAlgorithmType Algorithm { get; internal set; }

        /// <summary>
        /// Gets the strength of the key in bits.
        /// </summary>
        public int BitStrength { get; internal set; }

        /// <summary>
        /// Gets the creation time of this key.
        /// </summary>
        public DateTime CreationTime { get; internal set; }

        /// <summary>
        /// Gets check if this key has an algorithm type that makes it suitable to use for encryption.
        /// </summary>
        public bool IsEncryptionKey { get; internal set; }

        /// <summary>
        /// Gets if this is a master key.
        /// </summary>
        public bool IsMasterKey { get; internal set; }

        /// <summary>
        /// Gets the keyId associated with the public key.
        /// </summary>
        public long KeyId { get; internal set; }

        /// <summary>
        /// Gets the number of valid days from creation time - zero means no expiry.
        /// </summary>
        public int ValidDays { get; internal set; }

        /// <summary>
        /// Gets the version of this key.
        /// </summary>
        public int Version { get; internal set; }

        /// <summary>
        /// Gets check whether this (sub)key has a revocation signature on it.
        /// </summary>
        public bool IsRevoked { get; internal set; }

        /// <summary>
        /// Gets the fingerprint of the key.
        /// </summary>
        public byte[] Fingerprint { get; internal set; }

        /// <summary>
        /// Gets the number of valid seconds from creation time - zero means no expiry.
        /// </summary>
        public long ValidSeconds { get; internal set; }

        /// <summary>
        /// Get the user IDs associated with the key.
        /// </summary>
        /// <returns></returns>
        public List<string> GetUserIds()
        {
            // If the pgp public key exists.
            if (_pgpPublicKey != null)
            {
                List<string> userID = new List<string>();

                // For each id found.
                foreach (object id in _pgpPublicKey.GetUserIds())
                {
                    // Add the id.
                    userID.Add(id.ToString());
                }

                // Return all user ids.
                return userID;
            }
            else
                return null;
        }

        /// <summary>
        /// The public key contained in the object.
        /// </summary>
        /// <returns>The RSA cryto service provider.</returns>
        public RSACryptoServiceProvider GetKey()
        {
            // If the pgp public key exists.
            if (_pgpPublicKey != null)
            {
                // Get the ras key parameters
                Key.Crypto.Parameters.RsaKeyParameters rsaPublicKey = (Key.Crypto.Parameters.RsaKeyParameters)_pgpPublicKey.GetKey();

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
            else
                return null;
        }
    }
}
