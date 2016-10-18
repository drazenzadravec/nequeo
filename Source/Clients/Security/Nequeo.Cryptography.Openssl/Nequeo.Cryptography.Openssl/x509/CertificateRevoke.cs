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

using Nequeo.Invention;
using Nequeo.Extension;
using Nequeo.Cryptography.Openssl.Container;

namespace Nequeo.Cryptography.Openssl.x509
{
    /// <summary>
    /// Open SSL root certificate authority revoke control implementation.
    /// </summary>
    public class CertificateRevoke
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="openSslExecutablePath">The full path and file name of the openssl executable.</param>
        /// <param name="openSslConfigurationPath">The full path and file name of the openssl configuration file.</param>
        public CertificateRevoke(string openSslExecutablePath, string openSslConfigurationPath)
        {
            if (String.IsNullOrEmpty(openSslExecutablePath)) throw new ArgumentNullException("openSslExecutablePath");
            if (String.IsNullOrEmpty(openSslConfigurationPath)) throw new ArgumentNullException("openSslConfigurationPath");

            _openSslExecutablePath = openSslExecutablePath;
            _openSslConfigurationPath = openSslConfigurationPath;
        }

        private string _openSslExecutablePath = string.Empty;
        private string _openSslConfigurationPath = string.Empty;

        /// <summary>
        /// Revoke the certificate.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="certificatePath">The full path and file name of the signed certificate. (entension PEM)</param>
        /// <returns>True if no error has been found else false.</returns>
        public bool Revoke(string caPassPhrase, string certificatePath)
        {
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");

            // Create the new certificate.
            return RevokeEx(caPassPhrase, certificatePath);
        }

        /// <summary>
        /// Update the certificate database.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <returns>True if no error has been found else false.</returns>
        public bool UpdateDatabase(string caPassPhrase)
        {
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            
            // Create the new certificate.
            return UpdateDatabaseEx(caPassPhrase);
        }

        /// <summary>
        /// Generate a CRL certificate revoke.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="revokeCertificatePath">The full path and file name where the list should be place. (extension PEM)</param>
        /// <returns>True if no error has been found else false.</returns>
        public bool RevokeCertificateList(string caPassPhrase, string revokeCertificatePath)
        {
            if (String.IsNullOrEmpty(caPassPhrase)) throw new ArgumentNullException("caPassPhrase");
            if (String.IsNullOrEmpty(revokeCertificatePath)) throw new ArgumentNullException("revokeCertificatePath");

            // Create the new certificate.
            return RevokeCertificateEx(caPassPhrase, revokeCertificatePath);
        }

        /// <summary>
        /// Generate a CRL revoke list, this is used to send to all clients that are using the certificate.
        /// </summary>
        /// <param name="revokeCertificatePath">The full path and file name where the list should be place. (extension PEM)</param>
        /// <param name="revokeListPath">The certificate revoke list (CRL) full path and file name.  (extension CRL)</param>
        /// <returns>True if no error has been found else false.</returns>
        public bool RevokeList(string revokeCertificatePath, string revokeListPath)
        {
            if (String.IsNullOrEmpty(revokeListPath)) throw new ArgumentNullException("revokeListPath");
            if (String.IsNullOrEmpty(revokeCertificatePath)) throw new ArgumentNullException("revokeCertificatePath");

            // Create the new certificate.
            return RevokeListEx(revokeCertificatePath, revokeListPath);
        }

        /// <summary>
        /// Revoke the certificate.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="certificatePath">The full path and file name of the signed certificate. (extension PEM)</param>
        /// <returns>True if no error has been found else false.</returns>
        private bool RevokeEx(string caPassPhrase, string certificatePath)
        {
            // Construct the arguments.
            string arguments =
                "ca -config \"" + _openSslConfigurationPath + "\" -passin pass:" + caPassPhrase + " -revoke \"" + certificatePath + "\"";

            // Start a new application of the openssl.exe file and pass the arguments to create a signed certificate.
            ApplicationInteractionResult? result = new ApplicationInteraction().RunApplication(_openSslExecutablePath, arguments, "", 0);
            return result != null ? true : false;
        }

        /// <summary>
        /// Update the certificate database.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <returns>True if no error has been found else false.</returns>
        private bool UpdateDatabaseEx(string caPassPhrase)
        {
            // Construct the arguments.
            string arguments =
                "ca -config \"" + _openSslConfigurationPath + "\" -passin pass:" + caPassPhrase + " -updatedb";

            // Start a new application of the openssl.exe file and pass the arguments to create a signed certificate.
            ApplicationInteractionResult? result = new ApplicationInteraction().RunApplication(_openSslExecutablePath, arguments, "", 0);
            return result != null ? true : false;
        }

        /// <summary>
        /// Generate a CRL certificate revoke.
        /// </summary>
        /// <param name="caPassPhrase">The root certificate authority password.</param>
        /// <param name="revokeCertificatePath">The full path and file name where the list should be place. (extension PEM)</param>
        /// <returns>True if no error has been found else false.</returns>
        private bool RevokeCertificateEx(string caPassPhrase, string revokeCertificatePath)
        {
            // Construct the arguments.
            string arguments =
                "ca -config \"" + _openSslConfigurationPath + "\" -gencrl -passin pass:" + caPassPhrase + " -out \"" + revokeCertificatePath + "\"";

            // Start a new application of the openssl.exe file and pass the arguments to create a signed certificate.
            ApplicationInteractionResult? result = new ApplicationInteraction().RunApplication(_openSslExecutablePath, arguments, "", 0);
            return result != null ? true : false;
        }

        /// <summary>
        /// Generate a CRL revoke list, this is used to send to all clients that are using the certificate.
        /// </summary>
        /// <param name="revokeCertificatePath">The full path and file name where the list should be place. (extension PEM)</param>
        /// <param name="revokeListPath">The certificate revoke list (CRL) full path and file name.  (extension CRL)</param>
        /// <returns>True if no error has been found else false.</returns>
        private bool RevokeListEx(string revokeCertificatePath, string revokeListPath)
        {
            // Construct the arguments.
            string arguments =
                "crl -inform pem -outform der -in \"" + revokeCertificatePath + "\" -out \"" + revokeListPath + "\"";

            // Start a new application of the openssl.exe file and pass the arguments to create a signed certificate.
            ApplicationInteractionResult? result = new ApplicationInteraction().RunApplication(_openSslExecutablePath, arguments, "", 0);
            return result != null ? true : false;
        }
    }
}
