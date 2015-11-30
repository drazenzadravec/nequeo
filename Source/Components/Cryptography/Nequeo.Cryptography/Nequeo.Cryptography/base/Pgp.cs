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
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using System.ComponentModel.Composition;

using Nequeo.Cryptography.Key;

namespace Nequeo.Cryptography
{
    /// <summary>
    /// Pretty good privacy cryptography.
    /// </summary>
    public class Pgp
    {
        /// <summary>
        /// Pretty good privacy cryptography.
        /// </summary>
        public Pgp() { }

        /// <summary>
        /// Decrypt the stream.
        /// </summary>
        /// <param name="decrypted">The stream containing the decrypted data.</param>
        /// <param name="input">The data to decrypt.</param>
        /// <param name="secretKey">The secret key used for decryption.</param>
        /// <param name="password">The password used to protect the secret key.</param>
        /// <returns>Returns null if no integrity packet exists; false if message failed integrity check; true if message integrity check passed.</returns>
        public bool? Decrypt(System.IO.Stream decrypted, System.IO.Stream input, System.IO.Stream secretKey, string password)
        {
            // Get decorder stream.
            input = Key.Bcpg.OpenPgp.PgpUtilities.GetDecoderStream(input);

            System.IO.Stream clear = null;
            System.IO.Stream unc = null;

            try
            {
                // Load the encrypted input stream.
                Key.Bcpg.OpenPgp.PgpEncryptedDataList encryptedDataList = null;
                Key.Bcpg.OpenPgp.PgpObjectFactory objectFactory = new Key.Bcpg.OpenPgp.PgpObjectFactory(input);
                Key.Bcpg.OpenPgp.PgpObject o = objectFactory.NextPgpObject();

                // The first object might be a PGP marker packet.
                if (o is Key.Bcpg.OpenPgp.PgpEncryptedDataList)
                {
                    // Get the data list.
                    encryptedDataList = (Key.Bcpg.OpenPgp.PgpEncryptedDataList)o;
                }
                else
                {
                    // Get the next object.
                    encryptedDataList = (Key.Bcpg.OpenPgp.PgpEncryptedDataList)objectFactory.NextPgpObject();
                }

                // Find the secret key
                Key.Bcpg.OpenPgp.PgpPrivateKey privateKey = null;
                Key.Bcpg.OpenPgp.PgpPublicKeyEncryptedData publicKeyEncryptedData = null;
                Key.Bcpg.OpenPgp.PgpSecretKeyRingBundle secretKeyRingBundle = new Key.Bcpg.OpenPgp.PgpSecretKeyRingBundle(Key.Bcpg.OpenPgp.PgpUtilities.GetDecoderStream(secretKey));

                // For each object find the secret key.
                foreach (Key.Bcpg.OpenPgp.PgpPublicKeyEncryptedData pked in encryptedDataList.GetEncryptedDataObjects())
                {
                    // Find the private key (secret key).
                    privateKey = FindSecretKey(secretKeyRingBundle, pked.KeyId, password.ToArray());

                    // If the private key exists.
                    if (privateKey != null)
                    {
                        // This is the private key.
                        publicKeyEncryptedData = pked;
                        break;
                    }
                }

                // If a private key was not found.
                if (privateKey == null)
                    throw new ArgumentException("secret key for message not found.");

                // Get the data stream.
                clear = publicKeyEncryptedData.GetDataStream(privateKey);

                // Get the key message.
                Key.Bcpg.OpenPgp.PgpObjectFactory plainFact = new Key.Bcpg.OpenPgp.PgpObjectFactory(clear);
                Key.Bcpg.OpenPgp.PgpObject message = plainFact.NextPgpObject();

                // If message is compressed.
                if (message is Key.Bcpg.OpenPgp.PgpCompressedData)
                {
                    // Decompress the message.
                    Key.Bcpg.OpenPgp.PgpCompressedData cData = (Key.Bcpg.OpenPgp.PgpCompressedData)message;
                    Key.Bcpg.OpenPgp.PgpObjectFactory pgpFact = new Key.Bcpg.OpenPgp.PgpObjectFactory(cData.GetDataStream());
                    message = pgpFact.NextPgpObject();
                }

                // If the message is literal data.
                if (message is Key.Bcpg.OpenPgp.PgpLiteralData)
                {
                    Key.Bcpg.OpenPgp.PgpLiteralData ld = (Key.Bcpg.OpenPgp.PgpLiteralData)message;

                    // Get the file name of the embedded encrypted file.
                    string outFileName = ld.FileName;

                    // Write the ecrypted data file to the decrypted stream.
                    unc = ld.GetInputStream();
                    Key.Utilities.IO.Streams.PipeAll(unc, decrypted);
                }
                else if (message is Key.Bcpg.OpenPgp.PgpOnePassSignatureList)
                {
                    throw new Exception("encrypted message contains a signed message - not literal data.");
                }
                else
                {
                    throw new Exception("message is not a simple encrypted file - type unknown.");
                }

                // If the ecrypted file contains an integrity packet associated with it.
                if (publicKeyEncryptedData.IsIntegrityProtected())
                {
                    // If it has been verified.
                    if (!publicKeyEncryptedData.Verify())
                    {
                        // Message failed integrity check.
                        return false;
                    }
                    else
                    {
                        // Message integrity check passed.
                        return true;
                    }
                }

                // No integrity packet exists.
                return null;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (clear != null)
                    clear.Close();

                if (unc != null)
                    unc.Close();
            }
        }

        /// <summary>
        /// Decrypt the stream.
        /// </summary>
        /// <param name="decrypted">The stream containing the decrypted data.</param>
        /// <param name="input">The data to decrypt.</param>
        /// <param name="password">The password used for decryption.</param>
        /// <returns>Returns null if no integrity packet exists; false if message failed integrity check; true if message integrity check passed.</returns>
        public bool? Decrypt(System.IO.Stream decrypted, System.IO.Stream input, string password)
        {
            // Get decorder stream.
            input = Key.Bcpg.OpenPgp.PgpUtilities.GetDecoderStream(input);

            System.IO.Stream clear = null;
            System.IO.Stream unc = null;

            try
            {
                // Load the encrypted input stream.
                Key.Bcpg.OpenPgp.PgpEncryptedDataList encryptedDataList = null;
                Key.Bcpg.OpenPgp.PgpObjectFactory objectFactory = new Key.Bcpg.OpenPgp.PgpObjectFactory(input);
                Key.Bcpg.OpenPgp.PgpObject o = objectFactory.NextPgpObject();

                // The first object might be a PGP marker packet.
                if (o is Key.Bcpg.OpenPgp.PgpEncryptedDataList)
                {
                    // Get the data list.
                    encryptedDataList = (Key.Bcpg.OpenPgp.PgpEncryptedDataList)o;
                }
                else
                {
                    // Get the next object.
                    encryptedDataList = (Key.Bcpg.OpenPgp.PgpEncryptedDataList)objectFactory.NextPgpObject();
                }

                // Load the password.
                Key.Bcpg.OpenPgp.PgpPbeEncryptedData pbe = (Key.Bcpg.OpenPgp.PgpPbeEncryptedData)encryptedDataList[0];
                clear = pbe.GetDataStream(password.ToArray());
                Key.Bcpg.OpenPgp.PgpObjectFactory plainFact = new Key.Bcpg.OpenPgp.PgpObjectFactory(clear);
                Key.Bcpg.OpenPgp.PgpObject message = plainFact.NextPgpObject();

                // If message is compressed.
                if (message is Key.Bcpg.OpenPgp.PgpCompressedData)
                {
                    // Decompress the message.
                    Key.Bcpg.OpenPgp.PgpCompressedData cData = (Key.Bcpg.OpenPgp.PgpCompressedData)message;
                    Key.Bcpg.OpenPgp.PgpObjectFactory pgpFact = new Key.Bcpg.OpenPgp.PgpObjectFactory(cData.GetDataStream());
                    message = pgpFact.NextPgpObject();
                }

                // If the message is literal data.
                if (message is Key.Bcpg.OpenPgp.PgpLiteralData)
                {
                    Key.Bcpg.OpenPgp.PgpLiteralData ld = (Key.Bcpg.OpenPgp.PgpLiteralData)message;

                    // Get the file name of the embedded encrypted file.
                    string outFileName = ld.FileName;

                    // Write the ecrypted data file to the decrypted stream.
                    unc = ld.GetInputStream();
                    Key.Utilities.IO.Streams.PipeAll(unc, decrypted);
                }
                else if (message is Key.Bcpg.OpenPgp.PgpOnePassSignatureList)
                {
                    throw new Exception("encrypted message contains a signed message - not literal data.");
                }
                else
                {
                    throw new Exception("message is not a simple encrypted file - type unknown.");
                }

                // If the ecrypted file contains an integrity packet associated with it.
                if (pbe.IsIntegrityProtected())
                {
                    // If it has been verified.
                    if (!pbe.Verify())
                    {
                        // Message failed integrity check.
                        return false;
                    }
                    else
                    {
                        // Message integrity check passed.
                        return true;
                    }
                }

                // No integrity packet exists.
                return null;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (clear != null)
                    clear.Close();

                if (unc != null)
                    unc.Close();
            }
        }

        /// <summary>
        /// Encrypt the stream.
        /// </summary>
        /// <param name="encrypted">The encrypted data stream.</param>
        /// <param name="input">The data to encrypt.</param>
        /// <param name="publicKey">The public key used for encryption.</param>
        /// <param name="protectedKeys">Should the public and secret key data be protected.</param>
        /// <param name="integrityCheck">Should the cipher stream have an integrity packet associated with it.</param>
        /// <param name="symmetricKeyAlgorithm">The symmetric key algorithm used for cryptography.</param>
        public void Encrypt(System.IO.Stream encrypted, System.IO.Stream input, System.IO.Stream publicKey, bool protectedKeys = false, bool integrityCheck = false,
            Nequeo.Cryptography.SymmetricKeyAlgorithmType symmetricKeyAlgorithm = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Aes256)
        {
            // Read the public key data.
            Key.Bcpg.OpenPgp.PgpPublicKey pgpPublicKey = ReadPublicKey(publicKey);

            // If file is protected.
            if (protectedKeys)
                encrypted = new Key.Bcpg.ArmoredOutputStream(encrypted);

            System.IO.Stream encOutput = null;

            try
            {
                // Create the encypted data generator.
                Key.Bcpg.OpenPgp.PgpEncryptedDataGenerator encryptedDataGenerator = new Key.Bcpg.OpenPgp.PgpEncryptedDataGenerator(
                    GetSymmetricKeyAlgorithm(symmetricKeyAlgorithm), integrityCheck, new Key.Security.SecureRandom());
                encryptedDataGenerator.AddMethod(pgpPublicKey);

                // The input data buffer.
                byte[] buffer = Compress(input, Key.Bcpg.CompressionAlgorithmTag.Uncompressed);

                // Write the encrypted data.
                encOutput = encryptedDataGenerator.Open(encrypted, (long)buffer.Length);
                encOutput.Write(buffer, 0, buffer.Length);
                encOutput.Close();

                // If file is protected.
                if (protectedKeys)
                    encrypted.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (encOutput != null)
                    encOutput.Close();
            }
        }

        /// <summary>
        /// Encrypt the file.
        /// </summary>
        /// <param name="encrypted">The encrypted data stream.</param>
        /// <param name="filename">The path and file name to encrypt.</param>
        /// <param name="publicKey">The public key used for encryption.</param>
        /// <param name="protectedKeys">Should the public and secret key data be protected.</param>
        /// <param name="integrityCheck">Should the cipher stream have an integrity packet associated with it.</param>
        /// <param name="symmetricKeyAlgorithm">The symmetric key algorithm used for cryptography.</param>
        public void Encrypt(System.IO.Stream encrypted, string filename, System.IO.Stream publicKey, bool protectedKeys = false, bool integrityCheck = false,
            Nequeo.Cryptography.SymmetricKeyAlgorithmType symmetricKeyAlgorithm = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Aes256)
        {
            // Read the public key data.
            Key.Bcpg.OpenPgp.PgpPublicKey pgpPublicKey = ReadPublicKey(publicKey);

            // If file is protected.
            if (protectedKeys)
                encrypted = new Key.Bcpg.ArmoredOutputStream(encrypted);

            System.IO.Stream encOutput = null;

            try
            {
                // Create the encypted data generator.
                Key.Bcpg.OpenPgp.PgpEncryptedDataGenerator encryptedDataGenerator = new Key.Bcpg.OpenPgp.PgpEncryptedDataGenerator(
                    GetSymmetricKeyAlgorithm(symmetricKeyAlgorithm), integrityCheck, new Key.Security.SecureRandom());
                encryptedDataGenerator.AddMethod(pgpPublicKey);

                // The input data buffer.
                Key.Bcpg.OpenPgp.PgpCompressedDataGenerator compressedData =
                    new Key.Bcpg.OpenPgp.PgpCompressedDataGenerator(Key.Bcpg.CompressionAlgorithmTag.Uncompressed);

                // Write the encrypted data.
                encOutput = encryptedDataGenerator.Open(encrypted, new byte[1 << 16]);
                Key.Bcpg.OpenPgp.PgpUtilities.WriteFileToLiteralData(
                    compressedData.Open(encOutput),
                    Key.Bcpg.OpenPgp.PgpLiteralData.Binary,
                    new FileInfo(filename),
                    new byte[1 << 16]);

                // Close the streams.
                compressedData.Close();
                encOutput.Close();

                // If file is protected.
                if (protectedKeys)
                    encrypted.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (encOutput != null)
                    encOutput.Close();
            }
        }

        /// <summary>
        /// Encrypt the stream.
        /// </summary>
        /// <param name="encrypted">The encrypted data stream.</param>
        /// <param name="input">The data to encrypt.</param>
        /// <param name="password">The password used for encryption.</param>
        /// <param name="protectedKeys">Should the data be protected.</param>
        /// <param name="integrityCheck">Should the cipher stream have an integrity packet associated with it.</param>
        /// <param name="hashAlgorithm">The preferred hash algorithm to use to create the hash value.</param>
        /// <param name="symmetricKeyAlgorithm">The symmetric key algorithm used for cryptography.</param>
        public void Encrypt(System.IO.Stream encrypted, System.IO.Stream input, string password, bool protectedKeys = false,
            bool integrityCheck = false, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512,
            Nequeo.Cryptography.SymmetricKeyAlgorithmType symmetricKeyAlgorithm = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Aes256)
        {
            // If file is protected.
            if (protectedKeys)
                encrypted = new Key.Bcpg.ArmoredOutputStream(encrypted);

            System.IO.Stream encOutput = null;

            try
            {
                // Create the encypted data generator.
                Key.Bcpg.OpenPgp.PgpEncryptedDataGenerator encryptedDataGenerator = new Key.Bcpg.OpenPgp.PgpEncryptedDataGenerator(
                    GetSymmetricKeyAlgorithm(symmetricKeyAlgorithm), integrityCheck, new Key.Security.SecureRandom());
                encryptedDataGenerator.AddMethod(password.ToArray(), GetHashAlgorithm(hashAlgorithm));

                // The input data buffer.
                byte[] buffer = Compress(input, Key.Bcpg.CompressionAlgorithmTag.Uncompressed);

                // Write the encrypted data.
                encOutput = encryptedDataGenerator.Open(encrypted, (long)buffer.Length);
                encOutput.Write(buffer, 0, buffer.Length);
                encOutput.Close();

                // If file is protected.
                if (protectedKeys)
                    encrypted.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (encOutput != null)
                    encOutput.Close();
            }
        }

        /// <summary>
        /// Encrypt the file.
        /// </summary>
        /// <param name="encrypted">The encrypted data stream.</param>
        /// <param name="filename">The path and file name to encrypt.</param>
        /// <param name="password">The password used for encryption.</param>
        /// <param name="protectedKeys">Should the data be protected.</param>
        /// <param name="integrityCheck">Should the cipher stream have an integrity packet associated with it.</param>
        /// <param name="hashAlgorithm">The preferred hash algorithm to use to create the hash value.</param>
        /// <param name="symmetricKeyAlgorithm">The symmetric key algorithm used for cryptography.</param>
        public void Encrypt(System.IO.Stream encrypted, string filename, string password, bool protectedKeys = false,
            bool integrityCheck = false, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512,
            Nequeo.Cryptography.SymmetricKeyAlgorithmType symmetricKeyAlgorithm = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Aes256)
        {
            // If file is protected.
            if (protectedKeys)
                encrypted = new Key.Bcpg.ArmoredOutputStream(encrypted);

            System.IO.Stream encOutput = null;

            try
            {
                // Create the encypted data generator.
                Key.Bcpg.OpenPgp.PgpEncryptedDataGenerator encryptedDataGenerator = new Key.Bcpg.OpenPgp.PgpEncryptedDataGenerator(
                    GetSymmetricKeyAlgorithm(symmetricKeyAlgorithm), integrityCheck, new Key.Security.SecureRandom());
                encryptedDataGenerator.AddMethod(password.ToArray(), GetHashAlgorithm(hashAlgorithm));

                // The input data buffer.
                Key.Bcpg.OpenPgp.PgpCompressedDataGenerator compressedData =
                    new Key.Bcpg.OpenPgp.PgpCompressedDataGenerator(Key.Bcpg.CompressionAlgorithmTag.Uncompressed);

                // Write the encrypted data.
                encOutput = encryptedDataGenerator.Open(encrypted, new byte[1 << 16]);
                Key.Bcpg.OpenPgp.PgpUtilities.WriteFileToLiteralData(
                    compressedData.Open(encOutput),
                    Key.Bcpg.OpenPgp.PgpLiteralData.Binary,
                    new FileInfo(filename),
                    new byte[1 << 16]);

                // Close the streams.
                compressedData.Close();
                encOutput.Close();

                // If file is protected.
                if (protectedKeys)
                    encrypted.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (encOutput != null)
                    encOutput.Close();
            }
        }

        /// <summary>
        /// Computes the hash value of the specified input stream using the specified
        /// hash algorithm, and signs the resulting hash value.
        /// </summary>
        /// <param name="inputStream">The input data for which to compute the hash.</param>
        /// <param name="rsaProvider">The RSA crypto service provider.</param>
        /// <param name="keyID">The unique key id of the public secret key pair.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>The signature for the specified data.</returns>
        public byte[] SignData(Stream inputStream, RSACryptoServiceProvider rsaProvider, long keyID, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            MemoryStream output = null;
            Key.Bcpg.BcpgOutputStream pgpOutput = null;

            try
            {
                int ch;
                output = new MemoryStream();

                // Export the signer private key parameters.
                RSAParameters rsaPrivateKeySignerParam = rsaProvider.ExportParameters(true);
                Key.Crypto.Parameters.RsaPrivateCrtKeyParameters rsaPrivateKeySigner =
                    new Key.Crypto.Parameters.RsaPrivateCrtKeyParameters(
                        new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.Modulus),
                        new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.Exponent),
                        new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.D),
                        new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.P),
                        new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.Q),
                        new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.DP),
                        new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.DQ),
                        new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.InverseQ)
                );

                // Get the private key.
                Key.Bcpg.OpenPgp.PgpPrivateKey privateKey = new Key.Bcpg.OpenPgp.PgpPrivateKey(rsaPrivateKeySigner, keyID);

                // Create a signature generator.
                Key.Bcpg.OpenPgp.PgpSignatureGenerator signatureGenerator =
                    new Key.Bcpg.OpenPgp.PgpSignatureGenerator(Key.Bcpg.PublicKeyAlgorithmTag.RsaGeneral, GetHashAlgorithm(hashAlgorithm));
                signatureGenerator.InitSign(Key.Bcpg.OpenPgp.PgpSignature.BinaryDocument, privateKey);

                // Create the output stream.
                pgpOutput = new Key.Bcpg.BcpgOutputStream(output);

                // Read the input stream.
                while ((ch = inputStream.ReadByte()) >= 0)
                {
                    // Update the generator.
                    signatureGenerator.Update((byte)ch);
                }

                // Write the hash to the output stream.
                Key.Bcpg.OpenPgp.PgpSignature signature = signatureGenerator.Generate();
                signature.Encode(pgpOutput);

                // Return the signed value.
                return output.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (output != null)
                    output.Close();

                if (pgpOutput != null)
                    pgpOutput.Close();
            }
        }

        /// <summary>
        /// Verifies that a digital signature is valid by determining the hash value
        /// in the signature using the provided public key and comparing it to the hash
        /// value of the provided data.
        /// </summary>
        /// <param name="inputStream">The data that was signed.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <param name="rsaProvider">The RSA crypto service provider.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
        /// <returns>True if the signature is valid; otherwise, false.</returns>
        public bool VerifyData(Stream inputStream, byte[] signature, RSACryptoServiceProvider rsaProvider, Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            Stream signatureInput = null;

            try
            {
                // Export the signer public key parameters.
                RSAParameters rsaPublicKeySignerParam = rsaProvider.ExportParameters(false);
                Key.Crypto.Parameters.RsaKeyParameters rsaPublicKeySigner =
                    new Key.Crypto.Parameters.RsaKeyParameters(
                        false,
                        new Key.Math.BigInteger(1, rsaPublicKeySignerParam.Modulus),
                        new Key.Math.BigInteger(1, rsaPublicKeySignerParam.Exponent)
                    );

                signatureInput = new MemoryStream(signature);
                signatureInput = Key.Bcpg.OpenPgp.PgpUtilities.GetDecoderStream(signatureInput);

                // Get the public key.
                Key.Bcpg.OpenPgp.PgpPublicKey publicKey = new Key.Bcpg.OpenPgp.PgpPublicKey(Key.Bcpg.PublicKeyAlgorithmTag.RsaGeneral, rsaPublicKeySigner, DateTime.UtcNow);
                Key.Bcpg.OpenPgp.PgpObjectFactory pgpFact = new Key.Bcpg.OpenPgp.PgpObjectFactory(signatureInput);
                Key.Bcpg.OpenPgp.PgpSignatureList signatureList = null;
                Key.Bcpg.OpenPgp.PgpObject pgpObject = pgpFact.NextPgpObject();

                // If the message is compressed.
                if (pgpObject is Key.Bcpg.OpenPgp.PgpCompressedData)
                {
                    // Get the compression object.
                    Key.Bcpg.OpenPgp.PgpCompressedData compressedData = (Key.Bcpg.OpenPgp.PgpCompressedData)pgpObject;
                    pgpFact = new Key.Bcpg.OpenPgp.PgpObjectFactory(compressedData.GetDataStream());
                    signatureList = (Key.Bcpg.OpenPgp.PgpSignatureList)pgpFact.NextPgpObject();
                }
                else
                {
                    // Get the message list.
                    signatureList = (Key.Bcpg.OpenPgp.PgpSignatureList)pgpObject;
                }

                // Load the public key into the pgp signer.
                Key.Bcpg.OpenPgp.PgpSignature pgpSignature = signatureList[0];
                pgpSignature.InitVerify(publicKey);

                int ch;
                while ((ch = inputStream.ReadByte()) >= 0)
                {
                    // Update the generator.
                    pgpSignature.Update((byte)ch);
                }

                // Verify the signature.
                if (pgpSignature.Verify())
                {
                    // signature verified.
                    return true;
                }
                else
                {
                    // signature verification failed.
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (signatureInput != null)
                    signatureInput.Close();
            }
        }

        /// <summary>
        /// Public and secret key provider.
        /// </summary>
        /// <param name="publicKey">The public key data.</param>
        /// <param name="secretKey">The secret key data.</param>
        /// <param name="keyID">The unique key id of the public secret key pair.</param>
        /// <param name="password">The password used to protect the secret key.</param>
        /// <returns>The RSA cryto service provider.</returns>
        public RSACryptoServiceProvider PublicKeySecretKey(System.IO.Stream publicKey, System.IO.Stream secretKey, long keyID, string password = null)
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
        /// Search a secret key ring collection for a secret key corresponding to keyID if it exists.
        /// </summary>
        /// <param name="pgpSec">A secret key ring collection</param>
        /// <param name="keyID">The keyID we want.</param>
        /// <param name="password">The passphrase to decrypt secret key with.</param>
        /// <returns>The private key.</returns>
        private Key.Bcpg.OpenPgp.PgpPrivateKey FindSecretKey(Key.Bcpg.OpenPgp.PgpSecretKeyRingBundle pgpSec, long keyID, char[] password)
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
        private Key.Bcpg.OpenPgp.PgpPublicKey ReadPublicKey(System.IO.Stream publicKey)
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

        /// <summary>
        /// Compress the data.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="algorithm">The compression alogrithm.</param>
        /// <returns>The compressed data.</returns>
        private byte[] Compress(Stream input, Key.Bcpg.CompressionAlgorithmTag algorithm)
        {
            MemoryStream output = null;

            try
            {
                output = new MemoryStream();

                // Compress the data.
                Key.Bcpg.OpenPgp.PgpCompressedDataGenerator compressedData = new Key.Bcpg.OpenPgp.PgpCompressedDataGenerator(algorithm);
                Key.Bcpg.OpenPgp.PgpUtilities.WriteDataToLiteralData(compressedData.Open(output),
                    input, Key.Bcpg.OpenPgp.PgpLiteralData.Binary, Guid.NewGuid().ToString(), input.Length, DateTime.UtcNow);

                // Return the compressed data.
                compressedData.Close();
                return output.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (output != null)
                    output.Close();
            }
        }

        /// <summary>
        /// Get the hash algorithm.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <returns>The pgp hash algorithm.</returns>
        internal static Key.Bcpg.HashAlgorithmTag GetHashAlgorithm(Nequeo.Cryptography.HashcodeType hashAlgorithm = HashcodeType.SHA512)
        {
            Key.Bcpg.HashAlgorithmTag tag = Key.Bcpg.HashAlgorithmTag.Sha512;

            // Select the algorithm.
            switch (hashAlgorithm)
            {
                case HashcodeType.MD5:
                    tag = Key.Bcpg.HashAlgorithmTag.MD5;
                    break;

                case HashcodeType.SHA1:
                    tag = Key.Bcpg.HashAlgorithmTag.Sha1;
                    break;

                case HashcodeType.SHA256:
                    tag = Key.Bcpg.HashAlgorithmTag.Sha256;
                    break;

                case HashcodeType.SHA384:
                    tag = Key.Bcpg.HashAlgorithmTag.Sha384;
                    break;

                case HashcodeType.SHA512:
                    tag = Key.Bcpg.HashAlgorithmTag.Sha512;
                    break;
            }

            // Return the algorithm;
            return tag;
        }

        /// <summary>
        /// Get the symmetric key algorithm.
        /// </summary>
        /// <param name="symmetricKeyAlgorithm">The symmetric key algorithm.</param>
        /// <returns>The symmetric key algorithm.</returns>
        internal static Key.Bcpg.SymmetricKeyAlgorithmTag GetSymmetricKeyAlgorithm(
            Nequeo.Cryptography.SymmetricKeyAlgorithmType symmetricKeyAlgorithm = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Aes256)
        {
            Key.Bcpg.SymmetricKeyAlgorithmTag tag = Key.Bcpg.SymmetricKeyAlgorithmTag.Aes256;

            // Select the algorithm.
            switch (symmetricKeyAlgorithm)
            {
                case SymmetricKeyAlgorithmType.Aes128:
                    tag = Key.Bcpg.SymmetricKeyAlgorithmTag.Aes128;
                    break;

                case SymmetricKeyAlgorithmType.Aes192:
                    tag = Key.Bcpg.SymmetricKeyAlgorithmTag.Aes192;
                    break;

                case SymmetricKeyAlgorithmType.Aes256:
                    tag = Key.Bcpg.SymmetricKeyAlgorithmTag.Aes256;
                    break;

                case SymmetricKeyAlgorithmType.Blowfish:
                    tag = Key.Bcpg.SymmetricKeyAlgorithmTag.Blowfish;
                    break;

                case SymmetricKeyAlgorithmType.Cast5:
                    tag = Key.Bcpg.SymmetricKeyAlgorithmTag.Cast5;
                    break;

                case SymmetricKeyAlgorithmType.Des:
                    tag = Key.Bcpg.SymmetricKeyAlgorithmTag.Des;
                    break;

                case SymmetricKeyAlgorithmType.Idea:
                    tag = Key.Bcpg.SymmetricKeyAlgorithmTag.Idea;
                    break;

                case SymmetricKeyAlgorithmType.Null:
                    tag = Key.Bcpg.SymmetricKeyAlgorithmTag.Null;
                    break;

                case SymmetricKeyAlgorithmType.Safer:
                    tag = Key.Bcpg.SymmetricKeyAlgorithmTag.Safer;
                    break;

                case SymmetricKeyAlgorithmType.TripleDes:
                    tag = Key.Bcpg.SymmetricKeyAlgorithmTag.TripleDes;
                    break;

                case SymmetricKeyAlgorithmType.Twofish:
                    tag = Key.Bcpg.SymmetricKeyAlgorithmTag.Twofish;
                    break;
            }

            // Return the algorithm;
            return tag;
        }

        /// <summary>
        /// Get the symmetric key algorithm.
        /// </summary>
        /// <param name="symmetricKeyAlgorithm">The symmetric key algorithm.</param>
        /// <returns>The symmetric key algorithm.</returns>
        internal static Nequeo.Cryptography.SymmetricKeyAlgorithmType GetSymmetricKeyAlgorithmType(
            Key.Bcpg.SymmetricKeyAlgorithmTag symmetricKeyAlgorithm = Key.Bcpg.SymmetricKeyAlgorithmTag.Aes256)
        {
            Nequeo.Cryptography.SymmetricKeyAlgorithmType tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Aes256;

            // Select the algorithm.
            switch (symmetricKeyAlgorithm)
            {
                case Key.Bcpg.SymmetricKeyAlgorithmTag.Aes128:
                    tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Aes128;
                    break;

                case Key.Bcpg.SymmetricKeyAlgorithmTag.Aes192:
                    tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Aes192;
                    break;

                case Key.Bcpg.SymmetricKeyAlgorithmTag.Aes256:
                    tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Aes256;
                    break;

                case Key.Bcpg.SymmetricKeyAlgorithmTag.Blowfish:
                    tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Blowfish;
                    break;

                case Key.Bcpg.SymmetricKeyAlgorithmTag.Cast5:
                    tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Cast5;
                    break;

                case Key.Bcpg.SymmetricKeyAlgorithmTag.Des:
                    tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Des;
                    break;

                case Key.Bcpg.SymmetricKeyAlgorithmTag.Idea:
                    tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Idea;
                    break;

                case Key.Bcpg.SymmetricKeyAlgorithmTag.Null:
                    tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Null;
                    break;

                case Key.Bcpg.SymmetricKeyAlgorithmTag.Safer:
                    tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Safer;
                    break;

                case Key.Bcpg.SymmetricKeyAlgorithmTag.TripleDes:
                    tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.TripleDes;
                    break;

                case Key.Bcpg.SymmetricKeyAlgorithmTag.Twofish:
                    tag = Nequeo.Cryptography.SymmetricKeyAlgorithmType.Twofish;
                    break;
            }

            // Return the algorithm;
            return tag;
        }
    }
}
