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
    /// Open ssl certificate creation.
    /// </summary>
	public class Certificate
	{
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="openSslExecutablePath">The full path and file name of the openssl executable.</param>
        /// <param name="openSslConfigurationPath">The full path and file name of the openssl configuration file.</param>
        public Certificate(string openSslExecutablePath, string openSslConfigurationPath)
        {
            if (String.IsNullOrEmpty(openSslExecutablePath)) throw new ArgumentNullException("openSslExecutablePath");
            if (String.IsNullOrEmpty(openSslConfigurationPath)) throw new ArgumentNullException("openSslConfigurationPath");

            _openSslExecutablePath = openSslExecutablePath;
            _openSslConfigurationPath = openSslConfigurationPath;
        }

        private string _openSslExecutablePath = string.Empty;
        private string _openSslConfigurationPath = string.Empty;

        /// <summary>
        /// Create and sign the new certificate.
        /// </summary>
        /// <param name="subject">The complete subject list</param>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        /// <param name="multiDomain">Use the multi domain configuration arguments</param>
        public void CreateCertificate(Subject subject, string certificatePassPhrase, string caPassPhrase, string certificatePath, bool multiDomain = false)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");

            // Get each file name
            string requestPath = Helper.GetRequestPath(certificatePath);
            string certPath = Helper.GetCertificatePath(certificatePath);

            // Create a new certificate request.
            CertificateRequest request = new CertificateRequest(_openSslExecutablePath, _openSslConfigurationPath);
            request.Create(subject, certificatePassPhrase, requestPath, multiDomain);

            // Sign the certificate with the roor certificate authority
            CertificateSigning sign = new CertificateSigning(_openSslExecutablePath, _openSslConfigurationPath);
            sign.Sign(caPassPhrase, requestPath, certPath, multiDomain);
        }

        /// <summary>
        /// Create new request, sign and create new personal information exchange certificate.
        /// </summary>
        /// <param name="subject">The complete subject list</param>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="exportPassPhrase">The certificate export password.</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        /// <param name="caCertificatePath">The certificate authority certificate full path and file name.</param>
        /// <param name="multiDomain">Use the multi domain configuration arguments</param>
        public void CreatePIECertificate(Subject subject, string certificatePassPhrase, string caPassPhrase,
            string exportPassPhrase, string certificatePath, string caCertificatePath, bool multiDomain = false)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");
            if (String.IsNullOrEmpty(caCertificatePath)) throw new ArgumentNullException("caCertificatePath");

            // Request an new certificate and sign the certificate with the root certificate authority.
            CreateCertificate(subject, certificatePassPhrase, caPassPhrase, certificatePath, multiDomain);

            // Get each file name
            string requestPath = Helper.GetRequestPath(certificatePath);
            string certPath = Helper.GetCertificatePath(certificatePath);
            string friendlyName = Helper.GetFileNameWithoutExtension(certificatePath);
            string piePath = Helper.GetPIECertificatePath(certificatePath);

            // Create the personal information exchange certificate
            CertificatePersonelKey pie = new CertificatePersonelKey(_openSslExecutablePath, _openSslConfigurationPath);
            pie.Create(certificatePassPhrase, exportPassPhrase, caCertificatePath, requestPath, certPath, friendlyName, piePath);
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
            CertificateRevoke certificate = new CertificateRevoke(_openSslExecutablePath, _openSslConfigurationPath);
            certificate.RevokeCertificateList(caPassPhrase, revokeCertificatePath);
            certificate.RevokeList(revokeCertificatePath, revokeListPath);

            try
            {
                // Attempt to delete the revoke list certificate pem file.
                if (System.IO.File.Exists(revokeCertificatePath))
                    System.IO.File.Delete(revokeCertificatePath);
            }
            catch (Exception)
            {
                throw;
            }
        }
	}
}
