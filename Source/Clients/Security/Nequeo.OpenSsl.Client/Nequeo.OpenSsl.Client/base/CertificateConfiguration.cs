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
using System.Text;
using System.IO;

using Nequeo.Security.OpenSsl.Common;
using Nequeo.Security.OpenSsl.x509;
using Nequeo.Security.OpenSsl.Container;

namespace Nequeo.Security.OpenSsl
{
    /// <summary>
    /// Open ssl certificate creation using the configuration file.
    /// </summary>
	public class CertificateConfiguration
	{
        /// <summary>
        /// Create and sign the new certificate.
        /// </summary>
        /// <param name="subject">The complete subject list</param>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        public void CreateCertificate(Subject subject, string certificatePassPhrase, string caPassPhrase, string certificatePath)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");

            // Create a new certificate request.
            Certificate certificate = new Certificate(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable, 
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Create the certificate.
            certificate.CreateCertificate(subject, certificatePassPhrase, caPassPhrase, certificatePath);
        }

        /// <summary>
        /// Create new request, sign and create new personal information exchange certificate.
        /// </summary>
        /// <param name="subject">The complete subject list</param>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="exportPassPhrase">The certificate export password.</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        public void CreatePIECertificate(Subject subject, string certificatePassPhrase, string caPassPhrase,
            string exportPassPhrase, string certificatePath)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");

            // Create a new certificate request.
            Certificate certificate = new Certificate(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Create the certificate.
            certificate.CreatePIECertificate(subject, certificatePassPhrase, caPassPhrase, exportPassPhrase,
                certificatePath, Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLCACertificate);
        }

        /// <summary>
        /// Create and sign the new certificate with multi domains within the configuration file.
        /// </summary>
        /// <param name="subject">The complete subject list</param>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        public void CreateCertificateMultiDomain(Subject subject, string certificatePassPhrase, string caPassPhrase, string certificatePath)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");

            // Create a new certificate request.
            Certificate certificate = new Certificate(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLMultiDomainConfiguration);

            // Create the certificate.
            certificate.CreateCertificate(subject, certificatePassPhrase, caPassPhrase, certificatePath, true);
        }

        /// <summary>
        /// Create new request, sign and create new personal information exchange certificate with multi domains within the configuration file.
        /// </summary>
        /// <param name="subject">The complete subject list</param>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="exportPassPhrase">The certificate export password.</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        public void CreatePIECertificateMultiDomain(Subject subject, string certificatePassPhrase, string caPassPhrase,
            string exportPassPhrase, string certificatePath)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");

            // Create a new certificate request.
            Certificate certificate = new Certificate(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLMultiDomainConfiguration);

            // Create the certificate.
            certificate.CreatePIECertificate(subject, certificatePassPhrase, caPassPhrase, exportPassPhrase,
                certificatePath, Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLCACertificate, true);
        }

        /// <summary>
        /// Request a new certificate.
        /// </summary>
        /// <param name="subject">The complete subject list</param>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        public void RequestCertificate(Subject subject, string certificatePassPhrase, string certificatePath)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");

            // Create a new certificate request.
            CertificateRequest certificate = new CertificateRequest(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Create the certificate.
            certificate.Create(subject, certificatePassPhrase, certificatePath);
        }

        /// <summary>
        /// Request a new certificate.
        /// </summary>
        /// <param name="subject">The complete subject list</param>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        public void RequestCertificateMultiDomain(Subject subject, string certificatePassPhrase, string certificatePath)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");

            // Create a new certificate request.
            CertificateRequest certificate = new CertificateRequest(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLMultiDomainConfiguration);

            // Create the certificate.
            certificate.Create(subject, certificatePassPhrase, certificatePath, true);
        }

        /// <summary>
        /// Sign the new certificate request.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="certificateRequestPath">The full path and file name of the certificate request.</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        public void SignCertificate(string caPassPhrase, string certificateRequestPath, string certificatePath)
        {
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificateRequestPath)) throw new ArgumentNullException("certificateRequestPath");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");

            // Sign the certificate with the roor certificate authority
            CertificateSigning sign = new CertificateSigning(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Sign the certificate
            sign.Sign(caPassPhrase, certificateRequestPath, certificatePath);
        }

        /// <summary>
        /// Sign the new certificate request.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="certificateRequestPath">The full path and file name of the certificate request.</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        public void SignCertificateMultiDomain(string caPassPhrase, string certificateRequestPath, string certificatePath)
        {
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificateRequestPath)) throw new ArgumentNullException("certificateRequestPath");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");

            // Sign the certificate with the roor certificate authority
            CertificateSigning sign = new CertificateSigning(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLMultiDomainConfiguration);

            // Sign the certificate
            sign.Sign(caPassPhrase, certificateRequestPath, certificatePath, true);
        }


        /// <summary>
        /// Create a new certificate authority.
        /// </summary>
        ///  <param name="subject">The complete subject list</param>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="privateKeyPath">The full path and file name where the root certificate authority private key is to be place.</param>
        /// <param name="certificatePath">The full path and file name where the root certificate authority should be place.</param>
        /// <param name="days">The number of days the root certificate is to be active.</param>
        public void CreateCertificateAuthority(Subject subject, string certificatePassPhrase, string privateKeyPath, string certificatePath, int days = 7300)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (String.IsNullOrEmpty(privateKeyPath)) throw new ArgumentNullException("privateKeyPath");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");
            if (days < 1) throw new IndexOutOfRangeException("Parameter 'days' must be greater than zero.");

            // Create a new certificate request.
            CertificateAuthority certificate = new CertificateAuthority(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Create the certificate.
            certificate.Create(subject, certificatePassPhrase, privateKeyPath, certificatePath, days);
        }

        /// <summary>
        /// Remove the ecryption password from the certificate.
        /// </summary>
        /// <param name="certificatePassPhrase">The certificate password used for encryption.</param>
        /// <param name="privateKeyPath">The current private key full path and file name to decrypt.</param>
        /// <param name="noPassPhrasePrivateKeyPath">The decrypted private key full path and file name.</param>
        public void RemoveCertificatePassword(string certificatePassPhrase, string privateKeyPath, string noPassPhrasePrivateKeyPath)
        {
            if (String.IsNullOrEmpty(privateKeyPath)) throw new ArgumentNullException("privateKeyPath");
            if (String.IsNullOrEmpty(noPassPhrasePrivateKeyPath)) throw new ArgumentNullException("noPassPhrasePrivateKeyPath");

            // Create a new certificate request.
            CertificateRemovePassPhrase certificate = new CertificateRemovePassPhrase(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Create the certificate.
            certificate.Remove(certificatePassPhrase, privateKeyPath, noPassPhrasePrivateKeyPath);
        }

        /// <summary>
        /// Extract the private key and public key from the certificate from the certificate
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="certificatePassPhrase">The certificate password used for encryption.</param>
        /// <param name="certificatePath">The current certificate full path and file name.</param>
        /// <param name="publicKeyPath">The full path and file name of the public key data.</param>
        /// <param name="privateKeyPath">The full path and file name of the private key data.</param>
        public void ExtractCertificatePublicPrivateKeys(string caPassPhrase, string certificatePassPhrase, string certificatePath, string publicKeyPath, string privateKeyPath)
        {
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");
            if (String.IsNullOrEmpty(publicKeyPath)) throw new ArgumentNullException("publicKeyPath");
            if (String.IsNullOrEmpty(privateKeyPath)) throw new ArgumentNullException("privateKeyPath");

            // Create a new certificate request.
            CertificateExtractKey certificate = new CertificateExtractKey(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            certificate.ExtractPrivateKey(caPassPhrase, certificatePassPhrase, certificatePath, privateKeyPath);
            certificate.ExtractPublicKey(certificatePassPhrase, certificatePath, publicKeyPath);
        }

        /// <summary>
        /// Extract the private key from the certificate from the certificate
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="certificatePassPhrase">The certificate password used for encryption.</param>
        /// <param name="certificatePath">The current certificate full path and file name.</param>
        /// <param name="privateKeyPath">The full path and file name of the private key data.</param>
        public void ExtractCertificatePrivateKey(string caPassPhrase, string certificatePassPhrase, string certificatePath, string privateKeyPath)
        {
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");
            if (String.IsNullOrEmpty(privateKeyPath)) throw new ArgumentNullException("privateKeyPath");

            // Create a new certificate request.
            CertificateExtractKey certificate = new CertificateExtractKey(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            certificate.ExtractPrivateKey(caPassPhrase, certificatePassPhrase, certificatePath, privateKeyPath);
        }

        /// <summary>
        /// Extract the public key from the certificate from the certificate
        /// </summary>
        /// <param name="certificatePassPhrase">The certificate password used for encryption.</param>
        /// <param name="certificatePath">The current certificate full path and file name.</param>
        /// <param name="publicKeyPath">The full path and file name of the public key data.</param>
        public void ExtractCertificatePublicKey(string certificatePassPhrase, string certificatePath, string publicKeyPath)
        {
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");
            if (String.IsNullOrEmpty(publicKeyPath)) throw new ArgumentNullException("publicKeyPath");

            // Create a new certificate request.
            CertificateExtractKey certificate = new CertificateExtractKey(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            certificate.ExtractPublicKey(certificatePassPhrase, certificatePath, publicKeyPath);
        }

        /// <summary>
        /// Revoke the certificate.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="certificatePath">The full path and file name of the signed certificate. (extension PEM)</param>
        /// <returns>True if no error has been found else false.</returns>
        public void RevokeCertificate(string caPassPhrase, string certificatePath)
        {
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");

            // Create a new certificate request.
            CertificateRevoke certificate = new CertificateRevoke(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Create the new certificate.
            certificate.Revoke(caPassPhrase, certificatePath);
        }

        /// <summary>
        /// Update the certificate database.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <returns>True if no error has been found else false.</returns>
        public void UpdateDatabaseCertificate(string caPassPhrase)
        {
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");

            // Create a new certificate request.
            CertificateRevoke certificate = new CertificateRevoke(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Create the new certificate.
            certificate.UpdateDatabase(caPassPhrase);
        }

        /// <summary>
        /// Generate a CRL certificate revoke.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="revokeCertificatePath">The full path and file name where the list should be place. (extension PEM)</param>
        /// <returns>True if no error has been found else false.</returns>
        public void RevokeCertificateList(string caPassPhrase, string revokeCertificatePath)
        {
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(revokeCertificatePath)) throw new ArgumentNullException("revokeCertificatePath");

            // Create a new certificate request.
            CertificateRevoke certificate = new CertificateRevoke(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Create the new certificate.
            certificate.RevokeCertificateList(caPassPhrase, revokeCertificatePath);
        }

        /// <summary>
        /// Generate a CRL revoke list, this is used to send to all clients that are using the certificate.
        /// </summary>
        /// <param name="revokeCertificatePath">The full path and file name where the list should be place. (extension PEM)</param>
        /// <param name="revokeListPath">The certificate revoke list (CRL) full path and file name.  (extension CRL)</param>
        /// <returns>True if no error has been found else false.</returns>
        public void RevokeListCertificate(string revokeCertificatePath, string revokeListPath)
        {
            if (String.IsNullOrEmpty(revokeListPath)) throw new ArgumentNullException("revokeListPath");
            if (String.IsNullOrEmpty(revokeCertificatePath)) throw new ArgumentNullException("revokeCertificatePath");

            // Create a new certificate request.
            CertificateRevoke certificate = new CertificateRevoke(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Create the new certificate.
            certificate.RevokeList(revokeCertificatePath, revokeListPath);
        }

        /// <summary>
        /// Generate a CRL revoke list, this is used to send to all clients that are using the certificate.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="revokeCertificatePath">The full path and file name where the list should be place. (extension PEM)</param>
        /// <param name="revokeListPath">The certificate revoke list (CRL) full path and file name.  (extension CRL)</param>
        public void CreateRevokationList(string caPassPhrase, string revokeCertificatePath, string revokeListPath)
        {
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(revokeCertificatePath)) throw new ArgumentNullException("revokeCertificatePath");
            if (String.IsNullOrEmpty(revokeListPath)) throw new ArgumentNullException("revokeListPath");

            // Create a new certificate request.
            Certificate certificate = new Certificate(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Create a new certificate request.
            certificate.CreateRevokationList(caPassPhrase, revokeCertificatePath, revokeListPath);
        }

        /// <summary>
        /// Generate a CRL revoke list, this is used to send to all clients that are using the certificate.
        /// </summary>
        /// <param name="publicKeyPath">The name and path to the public key to generate.</param>
        /// <param name="privateKeyPath">The name and path to the private key to generate.</param>
        /// <param name="keySize">The key size.</param>
        public void CreatePublicPrivateKeyPair(string publicKeyPath, string privateKeyPath, int keySize = 2048)
        {
            if (String.IsNullOrEmpty(publicKeyPath)) throw new ArgumentNullException("publicKeyPath");
            if (String.IsNullOrEmpty(privateKeyPath)) throw new ArgumentNullException("privateKeyPath");

            // Create a new certificate request.
            CertificatePublicPrivateKeyPair certificate = new CertificatePublicPrivateKeyPair(
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLExecutable,
                Nequeo.Security.OpenSsl.Properties.Settings.Default.OpenSSLConfiguration);

            // Create a new certificate request.
            certificate.Generate(publicKeyPath, privateKeyPath, keySize);
        }
	}
}
