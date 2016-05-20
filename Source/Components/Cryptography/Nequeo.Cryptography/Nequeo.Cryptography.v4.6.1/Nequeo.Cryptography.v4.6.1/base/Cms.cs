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

using Nequeo.Extension;
using Nequeo.Cryptography.Key.Cms;

namespace Nequeo.Cryptography
{
    /// <summary>
    /// Cryptographic Message Syntax.
    /// </summary>
    public class Cms
    {
        /// <summary>
        /// Cryptographic Message Syntax.
        /// </summary>
        public Cms()
        {
            _cms = new EnvelopedCms();
        }

        private EnvelopedCms _cms = null;

        /// <summary>
        /// Decrypts the contents of the decoded enveloped CMS/PKCS #7 message by using the private key associated with the certificate identified.
        /// </summary>
        /// <param name="encodedMessage">An array of byte values that represent the information to be decoded.</param>
        /// <param name="certificate">The certificate used to decrypt the encoded message.</param>
        /// <returns>Cryptographic message syntax information.</returns>
        public CmsInfo Decrypt(byte[] encodedMessage, X509Certificate2 certificate)
        {
            // Decode and decrypt.
            _cms.Decode(encodedMessage);
            _cms.Decrypt(new X509Certificate2Collection(certificate));

            // CMS information.
            Cryptography.CmsInfo cmsInfo = new Cryptography.CmsInfo();

            // Set the newly set members,
            cmsInfo.Certificates = _cms.Certificates;
            cmsInfo.ContentEncryptionAlgorithm = _cms.ContentEncryptionAlgorithm;
            cmsInfo.ContentInfo = _cms.ContentInfo;
            cmsInfo.RecipientInfos = _cms.RecipientInfos;
            cmsInfo.UnprotectedAttributes = _cms.UnprotectedAttributes;
            cmsInfo.Version = _cms.Version;

            // Return the information.
            return cmsInfo;
        }

        /// <summary>
        /// Encrypts the contents of the CMS/PKCS #7 message by using the specified recipient information.
        /// </summary>
        /// <param name="contentInfo">An instance of the ContentInfo class that represents the content and its type.</param>
        /// <param name="certificate">The certificate used to encrypt the content message.</param>
        /// <returns>If the method succeeds, the method returns an array of byte values that represent the encoded information.
        /// If the method fails, it throws an exception.</returns>
        public byte[] Encrypt(ContentInfo contentInfo, X509Certificate2 certificate)
        {
            // Create a new envolope, passing the
            // content info that is to be encrypted
            // and encoded.
            _cms = new EnvelopedCms(contentInfo);

            // Encode and encrypt
            _cms.Encrypt(new CmsRecipient(certificate));
            return _cms.Encode();
        }

        /// <summary>
        /// Creates a signature using the specified signer and adds the signature
        /// to the CMS/PKCS #7 message.
        /// </summary>
        /// <param name="contentInfo">An instance of the ContentInfo class that represents the content and its type.</param>
        /// <param name="certificate">The certificate used to compute the signature for the content message.</param>
        /// <param name="detached">A Boolean value that specifies whether the SignedCms object is for a detached signature. 
        /// If detached is true, the signature is detached. If detached is false, the signature is not detached.</param>
        /// <returns>An array of byte values that represents the encoded message. The encoded
        /// message can be decoded by the System.Security.Cryptography.Pkcs.SignedCms.Decode(System.Byte[]) method.
        /// </returns>
        /// <remarks>
        /// If the detached state is false (the default), the content that is signed is included in the CMS/PKCS #7 message 
        /// along with the signature information. If the detached state is true, clients that cannot decode S/MIME messages 
        /// can still see the content of the message if it is sent separately. This might be useful in an archiving application 
        /// that archives message content whether the message sender can be verified for authenticity.
        /// </remarks>
        public byte[] ComputeSignature(ContentInfo contentInfo, X509Certificate2 certificate, bool detached = false)
        {
            // Create a new, detached SignedCms message.
            SignedCms signedCms = new SignedCms(contentInfo, detached);

            // Sign the message.
            signedCms.ComputeSignature(new CmsSigner(certificate));

            // Encode the message. 
            return signedCms.Encode();
        }

        /// <summary>
        /// Get the collection of certificates used when computing the signature.
        /// The encoded message and signature are not detached.
        /// </summary>
        /// <param name="encodedMessage">The computed signature of the content info (includes the original message).</param>
        /// <returns>The collection of certificates used when computing the signature.</returns>
        public X509Certificate2Collection GetSigningCertificates(byte[] encodedMessage)
        {
            // Create a new, detached SignedCms message.
            SignedCms signedCms = new SignedCms();

            // encodedMessage is the encoded message received from  
            // the sender.
            signedCms.Decode(encodedMessage);
            return signedCms.Certificates;

        }

        /// <summary>
        /// Get the collection of certificates used when computing the signature.
        /// The encoded message and signature are detached.
        /// </summary>
        /// <param name="encodedMessage">The computed signature of the content info (does not includes the original message).</param>
        /// <param name="contentInfo">The original message contained in the content info.</param>
        /// <returns>The collection of certificates used when computing the signature.</returns>
        public X509Certificate2Collection GetSigningCertificates(byte[] encodedMessage, ContentInfo contentInfo)
        {
            // Create a new, detached SignedCms message.
            SignedCms signedCms = new SignedCms(contentInfo, true);

            // encodedMessage is the encoded message received from  
            // the sender.
            signedCms.Decode(encodedMessage);
            return signedCms.Certificates;

        }

        /// <summary>
        /// Verifies the digital signatures on the signed CMS/PKCS #7 message
        /// by using the specified collection of certificates and, optionally, validates
        /// the signers' certificates. The encoded message and signature are not detached.
        /// </summary>
        /// <param name="encodedMessage">An array of byte values that represent the information to be checked.</param>
        /// <param name="certificate">The certificate used to check the signature for the content message.</param>
        /// <param name="verifySignatureOnly">
        /// If verifySignatureOnly is true, only the digital signatures are verified. If it is false, the digital signatures 
        /// are verified, the signers' certificates are validated, and the purposes of the certificates are validated. 
        /// The purposes of a certificate are considered valid if the certificate has no key usage or if the key usage supports 
        /// digital signatures or nonrepudiation.
        /// </param>
        /// <returns>True if valid; else false.</returns>
        public bool CheckSignature(byte[] encodedMessage, X509Certificate2 certificate, bool verifySignatureOnly = true)
        {
            bool valid = true;

            try
            {
                // Create a new, detached SignedCms message.
                SignedCms signedCms = new SignedCms();

                // encodedMessage is the encoded message received from  
                // the sender.
                signedCms.Decode(encodedMessage);

                // Verify the signature without validating the  
                // certificate.
                signedCms.CheckSignature(new X509Certificate2Collection(certificate), verifySignatureOnly);
            }
            catch { valid = false; }

            // True if valid else false.
            return valid;
        }

        /// <summary>
        /// Verifies the digital signatures on the signed CMS/PKCS #7 message
        /// by using the specified collection of certificates and, optionally, validates
        /// the signers' certificates. The encoded message and signature are detached.
        /// </summary>
        /// <param name="encodedMessage">An array of byte values that represent the information to be checked.</param>
        /// <param name="contentInfo">The computed signature of the content message.</param>
        /// <param name="certificate">The certificate used to check the signature for the content message.</param>
        /// <param name="verifySignatureOnly">
        /// If verifySignatureOnly is true, only the digital signatures are verified. If it is false, the digital signatures 
        /// are verified, the signers' certificates are validated, and the purposes of the certificates are validated. 
        /// The purposes of a certificate are considered valid if the certificate has no key usage or if the key usage supports 
        /// digital signatures or nonrepudiation.
        /// </param>
        /// <returns>True if valid; else false.</returns>
        public bool CheckSignature(byte[] encodedMessage, ContentInfo contentInfo, X509Certificate2 certificate, bool verifySignatureOnly = true)
        {
            bool valid = true;

            try
            {
                // Create a new, detached SignedCms message.
                SignedCms signedCms = new SignedCms(contentInfo, true);

                // encodedMessage is the encoded message received from  
                // the sender.
                signedCms.Decode(encodedMessage);

                // Verify the signature without validating the  
                // certificate.
                signedCms.CheckSignature(new X509Certificate2Collection(certificate), verifySignatureOnly);
            }
            catch { valid = false; }

            // True if valid else false.
            return valid;
        }

        /// <summary>
        /// Combine the detached computed signature and message.
        /// </summary>
        /// <param name="originalMessage">The original message.</param>
        /// <param name="detachedComputedSignature">The computed signature of the message.</param>
        /// <returns>The combine detached computed signature and message; 
        /// the first 10 bytes indicate the size of the detached computed signature.</returns>
        public byte[] CombineDetachedMessage(byte[] originalMessage, byte[] detachedComputedSignature)
        {
            // First 10 bytes indicate the size of the computed signature.
            int length = detachedComputedSignature.Length;
            string lengthValue = length.ToString("D10");
            byte[] signatureSize = Encoding.ASCII.GetBytes(lengthValue);

            // Return the computed signature and message.
            return signatureSize.Combine(detachedComputedSignature, originalMessage);
        }

        /// <summary>
        /// Extract the detached original message and computed signature from the combined message.
        /// </summary>
        /// <param name="combinedDetachedMessage">The combined detached computed signature and message; 
        /// the first 10 bytes indicate the size of the detached computed signature.</param>
        /// <param name="detachedComputedSignature">Returns the computed signature of the message.</param>
        /// <returns>The original message that was combined.</returns>
        public byte[] ExtractDetachedMessage(byte[] combinedDetachedMessage, ref byte[] detachedComputedSignature)
        {
            byte[] messsage = null;

            MemoryStream stream = null;
            BinaryReader reader = null;

            try
            {
                stream = new MemoryStream(combinedDetachedMessage);
                reader = new BinaryReader(stream);

                // First 10 bytes indicate the size of the computed signature.
                byte[] signatureSize = reader.ReadBytes(10);
                string lengthValue = Encoding.ASCII.GetString(signatureSize);
                int length = Int32.Parse(lengthValue);

                // Read the computed signature.
                detachedComputedSignature = reader.ReadBytes(length);

                // Assign the message.
                int lengthOfMessage = combinedDetachedMessage.Length - (10 + length);
                messsage = reader.ReadBytes(lengthOfMessage);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();

                if (reader != null)
                    reader.Dispose();
            }

            // Return the original message.
            return messsage;
        }
    }

    /// <summary>
    /// Cryptographic Message Syntax Information.
    /// </summary>
    public struct CmsInfo
    {
        /// <summary>
        /// Gets the Certificates property retrieves the set of certificates associated with the enveloped CMS/PKCS #7 message.
        /// </summary>
        public X509Certificate2Collection Certificates
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the ContentEncryptionAlgorithm property retrieves the identifier of the algorithm used to encrypt the content.
        /// </summary>
        public AlgorithmIdentifier ContentEncryptionAlgorithm
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the ContentInfo property retrieves the inner content information for the enveloped CMS/PKCS #7 message.
        /// </summary>
        public ContentInfo ContentInfo
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the RecipientInfos property retrieves the recipient information associated with the enveloped CMS/PKCS #7 message.
        /// </summary>
        public RecipientInfoCollection RecipientInfos
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the UnprotectedAttributes property retrieves the unprotected (unencrypted) attributes associated with the enveloped CMS/PKCS #7 message. 
        /// Unprotected attributes are not encrypted, and so do not have data confidentiality within an EnvelopedCms object.
        /// </summary>
        public CryptographicAttributeObjectCollection UnprotectedAttributes
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the Version property retrieves the version of the enveloped CMS/PKCS #7 message. 
        /// </summary>
        public int Version
        {
            get;
            internal set;
        }
    }
}
