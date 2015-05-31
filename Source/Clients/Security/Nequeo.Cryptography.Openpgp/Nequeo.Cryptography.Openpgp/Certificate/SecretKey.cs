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
    /// Secret key.
    /// </summary>
    public class SecretKey
    {
        private Key.Bcpg.OpenPgp.PgpSecretKey _pgpSecretKey = null;
        private Key.Bcpg.OpenPgp.PgpPrivateKey _privateKey = null;

        /// <summary>
        /// Gets or sets the pgp secret key.
        /// </summary>
        internal Key.Bcpg.OpenPgp.PgpSecretKey PgpSecretKey
        {
            get { return _pgpSecretKey; }
            set { _pgpSecretKey = value; }
        }

        /// <summary>
        /// Gets or sets the pgp private key.
        /// </summary>
        internal Key.Bcpg.OpenPgp.PgpPrivateKey PgpPrivateKey
        {
            get { return _privateKey; }
            set { _privateKey = value; }
        }

        /// <summary>
        /// Gets the algorithm the key is encrypted with.
        /// </summary>
        public Nequeo.Cryptography.SymmetricKeyAlgorithmType KeyEncryptionAlgorithm { get; internal set; }

        /// <summary>
        /// Gets the keyId associated with the public key.
        /// </summary>
        public long KeyId { get; internal set; }

        /// <summary>
        /// Gets if this is a master key.
        /// </summary>
        public bool IsMasterKey { get; internal set; }

        /// <summary>
        /// Gets check if this key has an algorithm type that makes it suitable to use for signing.
        /// </summary>
        public bool IsSigningKey { get; internal set; }

        /// <summary>
        /// Get the user IDs associated with the key.
        /// </summary>
        /// <returns></returns>
        public List<string> GetUserIds()
        {
            // If the pgp public key exists.
            if (_pgpSecretKey != null)
            {
                List<string> userID = new List<string>();

                // For each id found.
                foreach (object id in _pgpSecretKey.UserIds)
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
        /// The private key contained in the object.
        /// </summary>
        /// <returns>The RSA cryto service provider.</returns>
        public RSACryptoServiceProvider GetPrivateKey()
        {
            // If the pgp public key exists.
            if (_pgpSecretKey != null)
            {
                // Assign the rsa parameters.
                RSAParameters rsaPrivateParam = new RSAParameters();
                Key.Crypto.Parameters.RsaPrivateCrtKeyParameters rsaCrtPrivateParam = (Key.Crypto.Parameters.RsaPrivateCrtKeyParameters)_privateKey.Key;

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
            else
                return null;
        }
    }
}
