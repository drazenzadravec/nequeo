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

namespace Nequeo.Cryptography.Openssl.x509
{
    /// <summary>
    /// Open SSL personal information exchange certificate control implementation.
    /// </summary>
    public class CertificatePersonelKey
	{
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="openSslExecutablePath">The full path and file name of the openssl executable.</param>
        /// <param name="openSslConfigurationPath">The full path and file name of the openssl configuration file.</param>
        public CertificatePersonelKey(string openSslExecutablePath, string openSslConfigurationPath)
        {
            if (String.IsNullOrEmpty(openSslExecutablePath)) throw new ArgumentNullException("openSslExecutablePath");
            if (String.IsNullOrEmpty(openSslConfigurationPath)) throw new ArgumentNullException("openSslConfigurationPath");

            _openSslExecutablePath = openSslExecutablePath;
            _openSslConfigurationPath = openSslConfigurationPath;
        }

        private string _openSslExecutablePath = string.Empty;
        private string _openSslConfigurationPath = string.Empty;

        /// <summary>
        /// Create a new personal information exchange.
        /// </summary>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="exportPassPhrase">The export certificate password</param>
        /// <param name="caCertificatePath">The certificate authority certificate full path and file name.</param>
        /// <param name="certificateRequestPath">The certificate request full path and file name</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        /// <param name="friendlyName">The friendly name for the personal information exchange certificate.</param>
        /// <param name="pieCertificatePath">The full path and file name of the personal information exchange.</param>
        /// <returns>True if no error has been found else false.</returns>
        public bool Create(string certificatePassPhrase, string exportPassPhrase, string caCertificatePath, string certificateRequestPath, 
            string certificatePath, string friendlyName, string pieCertificatePath)
        {
            if (String.IsNullOrEmpty(certificatePassPhrase)) throw new ArgumentNullException("certificatePassPhrase");
            if (String.IsNullOrEmpty(exportPassPhrase)) throw new ArgumentNullException("exportPassPhrase");
            if (String.IsNullOrEmpty(certificateRequestPath)) throw new ArgumentNullException("certificateRequestPath");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");
            if (String.IsNullOrEmpty(caCertificatePath)) throw new ArgumentNullException("caCertificatePath");
            if (String.IsNullOrEmpty(friendlyName)) throw new ArgumentNullException("friendlyName");

            // Create the new personal information exchange.
            return CreateEx(certificatePassPhrase, exportPassPhrase, caCertificatePath, certificateRequestPath, certificatePath, friendlyName, pieCertificatePath);
        }

        /// <summary>
        /// Create a new personal information exchange.
        /// </summary>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="exportPassPhrase">The export certificate password</param>
        /// <param name="caCertificatePath">The certificate authority certificate full path and file name.</param>
        /// <param name="certificateRequestPath">The certificate request full path and file name</param>
        /// <param name="certificatePath">The certificate full path and file name.</param>
        /// <param name="friendlyName">The friendly name for the personal information exchange certificate.</param>
        /// <param name="pieCertificatePath">The full path and file name of the personal information exchange.</param>
        /// <returns>True if no error has been found else false.</returns>
        private bool CreateEx(string certificatePassPhrase, string exportPassPhrase, string caCertificatePath, string certificateRequestPath, 
            string certificatePath, string friendlyName, string pieCertificatePath)
        {
            // Construct the arguments.
            string arguments =
                "pkcs12 -export -in \"" + certificatePath + "\" -inkey \"" + certificateRequestPath + "\" " +
                "-passin pass:" + certificatePassPhrase + " -certfile \"" + caCertificatePath + "\" -password pass:" + exportPassPhrase + " " +
                "-out " + pieCertificatePath + " -name \"" + friendlyName + "\"";

            // Start a new application of the openssl.exe file and pass the arguments to create a personal information exchange
            ApplicationInteractionResult? result = new ApplicationInteraction().RunApplication(_openSslExecutablePath, arguments, "", 0);
            return result != null ? true : false;
        }
	}
}
