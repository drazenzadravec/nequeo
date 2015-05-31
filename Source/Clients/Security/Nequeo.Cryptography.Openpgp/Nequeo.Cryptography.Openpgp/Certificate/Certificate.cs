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
    /// Certificate utility.
    /// </summary>
    public class Certificate : IDisposable
    {
        /// <summary>
        /// Certificate utility.
        /// </summary>
        public Certificate() { }

        /// <summary>
        /// Generate a public secret key pair.
        /// </summary>
        /// <param name="publicKey">The stream where public key data is written to.</param>
        /// <param name="secretKey">The stream where secret key data is written to.</param>
        /// <param name="identity">The unique identity of the public secret key pair (Name (comments) &lt;email@company.com&gt;).</param>
        /// <param name="password">The password used to protect the secret key.</param>
        /// <param name="isCritical">True, if should be treated as critical, false otherwise.</param>
        /// <param name="secondsKeyValid">The number of seconds the key is valid, or zero if no expiry.</param>
        /// <param name="secondsSignatureValid">The number of seconds the signature is valid, or zero if no expiry.</param>
        /// <param name="protectedKeys">Should the public and secret key data be protected.</param>
        /// <param name="publicExponent">The public exponent (e; the public key is now represented as {e, n}).</param>
        /// <param name="strength">The strength of the cipher.</param>
        /// <param name="hashAlgorithm">The preferred hash algorithm to use to create the hash value.</param>
        /// <param name="publicKeyAlgorithm">The public key algorithm type.</param>
        /// <param name="certificateLevel">The certification level.</param>
        /// <param name="symmetricKeyAlgorithm">The symmetric key algorithm used for cryptography.</param>
        /// <returns>The unique key id of the public secret key pair.</returns>
        public long Generate(System.IO.Stream publicKey, System.IO.Stream secretKey, Openpgp.Identity identity,
            string password, bool isCritical = false, long secondsKeyValid = 0, long secondsSignatureValid = 0,
            bool protectedKeys = true, long publicExponent = 3, int strength = 4096, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512,
            Openpgp.PublicKeyAlgorithmType publicKeyAlgorithm = Openpgp.PublicKeyAlgorithmType.RsaGeneral,
            Openpgp.CertificateLevelType certificateLevel = Openpgp.CertificateLevelType.DefaultCertification,
            Nequeo.Cryptography.SymmetricKeyAlgorithmType symmetricKeyAlgorithm = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Aes256)
        {
            // Create the rsa key paramaters from the strength and public exponent.
            Key.Crypto.Generators.RsaKeyPairGenerator keyPair = new Key.Crypto.Generators.RsaKeyPairGenerator();
            Key.Crypto.Parameters.RsaKeyGenerationParameters keyPairParam =
                new Key.Crypto.Parameters.RsaKeyGenerationParameters(
                    Key.Math.BigInteger.ValueOf(publicExponent), new Key.Security.SecureRandom(), strength, 25);

            // Initialise the parameters and generate the public private key pair.
            keyPair.Init(keyPairParam);
            Key.Crypto.AsymmetricCipherKeyPair rsaKeyPair = keyPair.GenerateKeyPair();

            // Seperate the keys.
            Key.Crypto.Parameters.RsaKeyParameters rsaPrivatePublic = (Key.Crypto.Parameters.RsaKeyParameters)rsaKeyPair.Public;
            Key.Crypto.Parameters.RsaPrivateCrtKeyParameters rsaCrtPrivateParam = (Key.Crypto.Parameters.RsaPrivateCrtKeyParameters)rsaKeyPair.Private;

            // If file is not protected.
            if (!protectedKeys)
                secretKey = new Key.Bcpg.ArmoredOutputStream(secretKey);

            // Create the signature subpackets.
            Key.Bcpg.OpenPgp.PgpSignatureSubpacketGenerator signatureSubpacketGenerator = new Key.Bcpg.OpenPgp.PgpSignatureSubpacketGenerator();
            signatureSubpacketGenerator.SetKeyExpirationTime(isCritical, secondsKeyValid);
            signatureSubpacketGenerator.SetPreferredHashAlgorithms(isCritical, new int[] { (int)Openpgp.PublicSecretKey.GetHashAlgorithm(hashAlgorithm) });
            signatureSubpacketGenerator.SetSignatureExpirationTime(isCritical, secondsSignatureValid);

            // Create the signature subpackets.
            Key.Bcpg.OpenPgp.PgpSignatureSubpacketGenerator signatureSubpacketUnHashedGenerator = new Key.Bcpg.OpenPgp.PgpSignatureSubpacketGenerator();
            signatureSubpacketUnHashedGenerator.SetKeyExpirationTime(isCritical, secondsKeyValid);
            signatureSubpacketUnHashedGenerator.SetPreferredHashAlgorithms(isCritical, new int[] { (int)Openpgp.PublicSecretKey.GetHashAlgorithm(hashAlgorithm) });
            signatureSubpacketUnHashedGenerator.SetSignatureExpirationTime(isCritical, secondsSignatureValid);

            // Generate the packets
            Key.Bcpg.OpenPgp.PgpSignatureSubpacketVector hashedPackets = signatureSubpacketGenerator.Generate();
            Key.Bcpg.OpenPgp.PgpSignatureSubpacketVector unhashedPackets = signatureSubpacketUnHashedGenerator.Generate();

            // Create the secret key.
            Key.Bcpg.OpenPgp.PgpSecretKey pgpSecretKey = new Key.Bcpg.OpenPgp.PgpSecretKey
                (
                    GetCertificateLevelType(certificateLevel),
                    GetPublicKeyAlgorithm(publicKeyAlgorithm),
                    rsaPrivatePublic,
                    rsaCrtPrivateParam,
                    DateTime.UtcNow,
                    identity.ToString(),
                    Openpgp.PublicSecretKey.GetSymmetricKeyAlgorithm(symmetricKeyAlgorithm),
                    password.ToArray(),
                    true,
                    hashedPackets,
                    unhashedPackets,
                    new Key.Security.SecureRandom(),
                    Openpgp.PublicSecretKey.GetHashAlgorithm(hashAlgorithm)
                );

            // Encode the secret key.
            pgpSecretKey.Encode(secretKey);

            // If file is not protected.
            if (!protectedKeys)
            {
                secretKey.Close();
                publicKey = new Key.Bcpg.ArmoredOutputStream(publicKey);
            }

            // Get the public key from the secret key.
            Key.Bcpg.OpenPgp.PgpPublicKey pgpPublicKey = pgpSecretKey.PublicKey;
            pgpPublicKey.Encode(publicKey);

            // If file is not protected.
            if (!protectedKeys)
                publicKey.Close();

            // Return the key id.
            return pgpSecretKey.KeyId;
        }

        /// <summary>
        /// Load the public key from the stream.
        /// </summary>
        /// <param name="publicKey">The stream containing the public key.</param>
        /// <returns>The public key.</returns>
        public Openpgp.PublicKey LoadPublicKey(System.IO.Stream publicKey)
        {
            Openpgp.PublicKey publicKeyContainer = new PublicKey();

            // Read the public key data.
            Key.Bcpg.OpenPgp.PgpPublicKey pgpPublicKey = Openpgp.PublicPrivateKey.ReadPublicKey(publicKey);
            publicKeyContainer.PgpPublicKey = pgpPublicKey;
            publicKeyContainer.Algorithm = GetPublicKeyAlgorithmType(pgpPublicKey.Algorithm);
            publicKeyContainer.BitStrength = pgpPublicKey.BitStrength;
            publicKeyContainer.CreationTime = pgpPublicKey.CreationTime;
            publicKeyContainer.IsEncryptionKey = pgpPublicKey.IsEncryptionKey;
            publicKeyContainer.IsMasterKey = pgpPublicKey.IsMasterKey;
            publicKeyContainer.IsRevoked = pgpPublicKey.IsRevoked();
            publicKeyContainer.KeyId = pgpPublicKey.KeyId;
            publicKeyContainer.ValidDays = pgpPublicKey.ValidDays;
            publicKeyContainer.Version = pgpPublicKey.Version;
            publicKeyContainer.Fingerprint = pgpPublicKey.GetFingerprint();
            publicKeyContainer.ValidSeconds = pgpPublicKey.GetValidSeconds();

            // Return the public key.
            return publicKeyContainer;
        }

        /// <summary>
        /// Load the secret key from the stream.
        /// </summary>
        /// <param name="secretKey">The stream containing the secret key.</param>
        /// <param name="keyID">The unique key id of the public secret key pair.</param>
        /// <param name="password">The password used to protect the secret key.</param>
        /// <returns>The secret key.</returns>
        public Openpgp.SecretKey LoadSecretKey(System.IO.Stream secretKey, long keyID, string password = null)
        {
            Openpgp.SecretKey secretKeyContainer = new SecretKey();

            // Find the secret key
            Key.Bcpg.OpenPgp.PgpPrivateKey privateKey = null;
            Key.Bcpg.OpenPgp.PgpSecretKeyRingBundle secretKeyRingBundle =
                new Key.Bcpg.OpenPgp.PgpSecretKeyRingBundle(Key.Bcpg.OpenPgp.PgpUtilities.GetDecoderStream(secretKey));

            // Find the private key (secret key).
            Key.Bcpg.OpenPgp.PgpSecretKey pgpSecretKey = secretKeyRingBundle.GetSecretKey(keyID);
            privateKey = Openpgp.PublicPrivateKey.FindSecretKey(secretKeyRingBundle, keyID, password.ToArray());

            // Assign the secret key data.
            secretKeyContainer.PgpSecretKey = pgpSecretKey;
            secretKeyContainer.PgpPrivateKey = privateKey;
            secretKeyContainer.KeyId = pgpSecretKey.KeyId;
            secretKeyContainer.IsMasterKey = pgpSecretKey.IsMasterKey;
            secretKeyContainer.IsSigningKey = pgpSecretKey.IsSigningKey;
            secretKeyContainer.KeyEncryptionAlgorithm = Openpgp.PublicSecretKey.GetSymmetricKeyAlgorithmType(pgpSecretKey.KeyEncryptionAlgorithm);
            
            // Return the secret key.
            return secretKeyContainer;
        }

        /// <summary>
        /// Get the public key algorithm.
        /// </summary>
        /// <param name="publicKeyAlgorithm">The public key algorithm.</param>
        /// <returns>The public key algorithm.</returns>
        internal static Key.Bcpg.PublicKeyAlgorithmTag GetPublicKeyAlgorithm(
            Openpgp.PublicKeyAlgorithmType publicKeyAlgorithm = Openpgp.PublicKeyAlgorithmType.RsaGeneral)
        {
            Key.Bcpg.PublicKeyAlgorithmTag tag = Key.Bcpg.PublicKeyAlgorithmTag.RsaGeneral;

            // Select the algorithm.
            switch (publicKeyAlgorithm)
            {
                case PublicKeyAlgorithmType.DiffieHellman:
                    tag = Key.Bcpg.PublicKeyAlgorithmTag.DiffieHellman;
                    break;

                case PublicKeyAlgorithmType.Dsa:
                    tag = Key.Bcpg.PublicKeyAlgorithmTag.Dsa;
                    break;

                case PublicKeyAlgorithmType.EC:
                    tag = Key.Bcpg.PublicKeyAlgorithmTag.EC;
                    break;

                case PublicKeyAlgorithmType.ECDsa:
                    tag = Key.Bcpg.PublicKeyAlgorithmTag.ECDsa;
                    break;

                case PublicKeyAlgorithmType.ElGamalEncrypt:
                    tag = Key.Bcpg.PublicKeyAlgorithmTag.ElGamalEncrypt;
                    break;

                case PublicKeyAlgorithmType.ElGamalGeneral:
                    tag = Key.Bcpg.PublicKeyAlgorithmTag.ElGamalGeneral;
                    break;

                case PublicKeyAlgorithmType.RsaEncrypt:
                    tag = Key.Bcpg.PublicKeyAlgorithmTag.RsaEncrypt;
                    break;

                case PublicKeyAlgorithmType.RsaGeneral:
                    tag = Key.Bcpg.PublicKeyAlgorithmTag.RsaGeneral;
                    break;

                case PublicKeyAlgorithmType.RsaSign:
                    tag = Key.Bcpg.PublicKeyAlgorithmTag.RsaSign;
                    break;
            }

            // Return the algorithm;
            return tag;
        }

        /// <summary>
        /// Get the public key algorithm.
        /// </summary>
        /// <param name="publicKeyAlgorithm">The public key algorithm.</param>
        /// <returns>The public key algorithm.</returns>
        internal static Openpgp.PublicKeyAlgorithmType GetPublicKeyAlgorithmType(
            Key.Bcpg.PublicKeyAlgorithmTag publicKeyAlgorithm = Key.Bcpg.PublicKeyAlgorithmTag.RsaGeneral)
        {
            Openpgp.PublicKeyAlgorithmType tag = Openpgp.PublicKeyAlgorithmType.RsaGeneral;

            // Select the algorithm.
            switch (publicKeyAlgorithm)
            {
                case Key.Bcpg.PublicKeyAlgorithmTag.DiffieHellman:
                    tag = Openpgp.PublicKeyAlgorithmType.DiffieHellman;
                    break;

                case Key.Bcpg.PublicKeyAlgorithmTag.Dsa:
                    tag = Openpgp.PublicKeyAlgorithmType.Dsa;
                    break;

                case Key.Bcpg.PublicKeyAlgorithmTag.EC:
                    tag = Openpgp.PublicKeyAlgorithmType.EC;
                    break;

                case Key.Bcpg.PublicKeyAlgorithmTag.ECDsa:
                    tag = Openpgp.PublicKeyAlgorithmType.ECDsa;
                    break;

                case Key.Bcpg.PublicKeyAlgorithmTag.ElGamalEncrypt:
                    tag = Openpgp.PublicKeyAlgorithmType.ElGamalEncrypt;
                    break;

                case Key.Bcpg.PublicKeyAlgorithmTag.ElGamalGeneral:
                    tag = Openpgp.PublicKeyAlgorithmType.ElGamalGeneral;
                    break;

                case Key.Bcpg.PublicKeyAlgorithmTag.RsaEncrypt:
                    tag = Openpgp.PublicKeyAlgorithmType.RsaEncrypt;
                    break;

                case Key.Bcpg.PublicKeyAlgorithmTag.RsaGeneral:
                    tag = Openpgp.PublicKeyAlgorithmType.RsaGeneral;
                    break;

                case Key.Bcpg.PublicKeyAlgorithmTag.RsaSign:
                    tag = Openpgp.PublicKeyAlgorithmType.RsaSign;
                    break;
            }

            // Return the algorithm;
            return tag;
        }

        /// <summary>
        /// Get certificate level.
        /// </summary>
        /// <param name="certificateLevel">The certificate level.</param>
        /// <returns>The certificate level.</returns>
        internal static int GetCertificateLevelType(
            Openpgp.CertificateLevelType certificateLevel = Openpgp.CertificateLevelType.DefaultCertification)
        {
            int tag = Key.Bcpg.OpenPgp.PgpSignature.DefaultCertification;

            // Select the algorithm.
            switch (certificateLevel)
            {
                case CertificateLevelType.BinaryDocument:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.BinaryDocument;
                    break;

                case CertificateLevelType.CanonicalTextDocument:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.CanonicalTextDocument;
                    break;

                case CertificateLevelType.CasualCertification:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.CasualCertification;
                    break;

                case CertificateLevelType.CertificationRevocation:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.CertificationRevocation;
                    break;

                case CertificateLevelType.DefaultCertification:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.DefaultCertification;
                    break;

                case CertificateLevelType.DirectKey:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.DirectKey;
                    break;

                case CertificateLevelType.KeyRevocation:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.KeyRevocation;
                    break;

                case CertificateLevelType.NoCertification:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.NoCertification;
                    break;

                case CertificateLevelType.PositiveCertification:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.PositiveCertification;
                    break;

                case CertificateLevelType.PrimaryKeyBinding:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.PrimaryKeyBinding;
                    break;

                case CertificateLevelType.StandAlone:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.StandAlone;
                    break;

                case CertificateLevelType.SubkeyBinding:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.SubkeyBinding;
                    break;

                case CertificateLevelType.SubkeyRevocation:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.SubkeyRevocation;
                    break;

                case CertificateLevelType.Timestamp:
                    tag = Key.Bcpg.OpenPgp.PgpSignature.Timestamp;
                    break;
            }

            // Return the algorithm;
            return tag;
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Certificate()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
