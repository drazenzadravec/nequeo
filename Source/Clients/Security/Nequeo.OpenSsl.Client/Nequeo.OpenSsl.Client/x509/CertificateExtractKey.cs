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

using Nequeo.Invention;
using Nequeo.Extension;
using Nequeo.Security.OpenSsl.Container;

namespace Nequeo.Security.OpenSsl.x509
{
    /// <summary>
    /// Open SSL certificate public and private key extraction implementation.
    /// </summary>
    public class CertificateExtractKey
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="openSslExecutablePath">The full path and file name of the openssl executable.</param>
        /// <param name="openSslConfigurationPath">The full path and file name of the openssl configuration file.</param>
        public CertificateExtractKey(string openSslExecutablePath, string openSslConfigurationPath)
        {
            if (String.IsNullOrEmpty(openSslExecutablePath)) throw new ArgumentNullException("openSslExecutablePath");
            if (String.IsNullOrEmpty(openSslConfigurationPath)) throw new ArgumentNullException("openSslConfigurationPath");

            _openSslExecutablePath = openSslExecutablePath;
            _openSslConfigurationPath = openSslConfigurationPath;
        }

        private string _openSslExecutablePath = string.Empty;
        private string _openSslConfigurationPath = string.Empty;

        /// <summary>
        /// Extract the public key from the certificate from the certificate
        /// </summary>
        /// <param name="certificatePassPhrase">The certificate password used for encryption.</param>
        /// <param name="certificatePath">The current certificate full path and file name.</param>
        /// <param name="publicKeyPath">The full path and file name of the public key data.</param>
        /// <returns>True if no error has been found else false.</returns>
        public bool ExtractPublicKey(string certificatePassPhrase, string certificatePath, string publicKeyPath)
        {
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");
            if (String.IsNullOrEmpty(publicKeyPath)) throw new ArgumentNullException("publicKeyPath");

            // Create the new root certificate authority.
            return ExtractPublicKeyEx(certificatePassPhrase, certificatePath, publicKeyPath);
        }

        /// <summary>
        /// Extract the private key from the certificate from the certificate
        /// </summary>
        /// <param name="exportPassPhrase">The export certificate password.</param>
        /// <param name="certificatePassPhrase">The certificate password used for encryption.</param>
        /// <param name="certificatePath">The current certificate full path and file name.</param>
        /// <param name="privateKeyPath">The full path and file name of the private key data.</param>
        /// <returns>True if no error has been found else false.</returns>
        public bool ExtractPrivateKey(string exportPassPhrase, string certificatePassPhrase, string certificatePath, string privateKeyPath)
        {
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");
            if (String.IsNullOrEmpty(privateKeyPath)) throw new ArgumentNullException("publicKeyPath");
            if (String.IsNullOrEmpty(exportPassPhrase)) throw new ArgumentNullException("caPassPhrase");

            // Create the new root certificate authority.
            return ExtractPrivateKeyEx(exportPassPhrase, certificatePassPhrase, certificatePath, privateKeyPath);
        }

        /// <summary>
        /// Extract the public key from the certificate from the certificate
        /// </summary>
        /// <param name="certificatePassPhrase">The certificate password used for encryption.</param>
        /// <param name="certificatePath">The current certificate full path and file name.</param>
        /// <param name="publicKeyPath">The full path and file name of the public key data.</param>
        /// <returns>True if no error has been found else false.</returns>
        private bool ExtractPublicKeyEx(string certificatePassPhrase, string certificatePath, string publicKeyPath)
        {
            // Construct the arguments.
            string arguments = "pkcs12 " + (String.IsNullOrEmpty(certificatePassPhrase) ? "-passin pass:" : "-passin pass:" + certificatePassPhrase) + 
                " -in \"" + certificatePath + "\" -clcerts -nokeys -out \"" + publicKeyPath + "\"";

            // Start a new application of the openssl.exe file and pass the arguments to create a root certificate authority
            ApplicationInteractionResult? result = new ApplicationInteraction().RunApplication(_openSslExecutablePath, arguments, "", 0);
            return result != null ? true : false;
        }

        /// <summary>
        /// Extract the private key from the certificate from the certificate
        /// </summary>
        /// <param name="exportPassPhrase">The export certificate password.</param>
        /// <param name="certificatePassPhrase">The certificate password used for encryption.</param>
        /// <param name="certificatePath">The current certificate full path and file name.</param>
        /// <param name="privateKeyPath">The full path and file name of the private key data.</param>
        /// <returns>True if no error has been found else false.</returns>
        private bool ExtractPrivateKeyEx(string exportPassPhrase, string certificatePassPhrase, string certificatePath, string privateKeyPath)
        {
            // Construct the arguments.
            string arguments = "pkcs12 " + (String.IsNullOrEmpty(certificatePassPhrase) ? "-passin pass:" : "-passin pass:" + certificatePassPhrase) + " " +
                (String.IsNullOrEmpty(exportPassPhrase) ? "-passout pass:" : "-passout pass:" + exportPassPhrase) + " -in \"" + certificatePath + "\" -nocerts -out \"" + privateKeyPath + "\"";

            // Start a new application of the openssl.exe file and pass the arguments to create a root certificate authority
            ApplicationInteractionResult? result = new ApplicationInteraction().RunApplication(_openSslExecutablePath, arguments, "", 0);
            return result != null ? true : false;
        }
    }
}