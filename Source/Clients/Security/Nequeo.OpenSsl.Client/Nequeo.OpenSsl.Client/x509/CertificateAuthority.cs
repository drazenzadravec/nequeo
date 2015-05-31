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
    /// Open SSL root certificate authority control implementation.
    /// </summary>
	public class CertificateAuthority
	{
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="openSslExecutablePath">The full path and file name of the openssl executable.</param>
        /// <param name="openSslConfigurationPath">The full path and file name of the openssl configuration file.</param>
        public CertificateAuthority(string openSslExecutablePath, string openSslConfigurationPath)
        {
            if (String.IsNullOrEmpty(openSslExecutablePath)) throw new ArgumentNullException("openSslExecutablePath");
            if (String.IsNullOrEmpty(openSslConfigurationPath)) throw new ArgumentNullException("openSslConfigurationPath");

            _openSslExecutablePath = openSslExecutablePath;
            _openSslConfigurationPath = openSslConfigurationPath;
        }

        private string _openSslExecutablePath = string.Empty;
        private string _openSslConfigurationPath = string.Empty;

        /// <summary>
        /// Create a new root certificate authority.
        /// </summary>
        /// <param name="subject">The complete subject list</param>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="privateKeyPath">The full path and file name where the root certificate authority private key is to be place.</param>
        /// <param name="certificatePath">The full path and file name where the root certificate authority should be place.</param>
        /// <param name="days">The number of days the root certificate is to be active.</param>
        /// <returns>True if no error has been found else false.</returns>
        public bool Create(Subject subject, string certificatePassPhrase, string privateKeyPath, string certificatePath, int days = 7300)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (String.IsNullOrEmpty(privateKeyPath)) throw new ArgumentNullException("privateKeyPath");
            if (String.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath");
            if (days < 1) throw new IndexOutOfRangeException("Parameter 'days' must be greater than zero.");

            // Create the new root certificate authority.
            return CreateEx(subject, certificatePassPhrase, privateKeyPath, certificatePath, days);
        }

        /// <summary>
        /// Create a new root certificate authority.
        /// </summary>
        /// <param name="subject">The complete subject list</param>
        /// <param name="certificatePassPhrase">The certificate request password</param>
        /// <param name="privateKeyPath">The full path and file name where the root certificate authority private key is to be place.</param>
        /// <param name="certificatePath">The full path and file name where the root certificate authority should be place.</param>
        /// <param name="days">The number of days the root certificate is to be active.</param>
        /// <returns>True if no error has been found else false.</returns>
        private bool CreateEx(Subject subject, string certificatePassPhrase, string privateKeyPath, string certificatePath, int days)
        {
            // Construct the arguments.
            string arguments =
                "req -config \"" + _openSslConfigurationPath + "\" -new -x509 " +
                "-keyout \"" + privateKeyPath + "\" -out \"" + certificatePath + "\" " +
                "-days " + days.ToString() + " -sha512 " + (String.IsNullOrEmpty(certificatePassPhrase) ? "-passout pass:" : "-passout pass:" + certificatePassPhrase) + " " +
                "-subj \"" + subject.SubjectArguments() + "\"";

            // Start a new application of the openssl.exe file and pass the arguments to create a root certificate authority
            ApplicationInteractionResult? result = new ApplicationInteraction().RunApplication(_openSslExecutablePath, arguments, "", 0);
            return result != null ? true : false;
        }
	}
}
