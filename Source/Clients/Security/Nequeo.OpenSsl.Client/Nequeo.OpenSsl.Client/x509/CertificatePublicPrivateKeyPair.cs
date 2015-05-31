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
using Nequeo.Security.OpenSsl.Container;

namespace Nequeo.Security.OpenSsl.x509
{
    /// <summary>
    /// Open SSL certificate public and private key pair generation.
    /// </summary>
    public class CertificatePublicPrivateKeyPair
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="openSslExecutablePath">The full path and file name of the openssl executable.</param>
        /// <param name="openSslConfigurationPath">The full path and file name of the openssl configuration file.</param>
        public CertificatePublicPrivateKeyPair(string openSslExecutablePath, string openSslConfigurationPath)
        {
            if (String.IsNullOrEmpty(openSslExecutablePath)) throw new ArgumentNullException("openSslExecutablePath");
            if (String.IsNullOrEmpty(openSslConfigurationPath)) throw new ArgumentNullException("openSslConfigurationPath");

            _openSslExecutablePath = openSslExecutablePath;
            _openSslConfigurationPath = openSslConfigurationPath;
        }

        private string _openSslExecutablePath = string.Empty;
        private string _openSslConfigurationPath = string.Empty;

        /// <summary>
        /// Generate the public and private key pair.
        /// </summary>
        /// <param name="publicKeyPath">The name and path to the public key to generate.</param>
        /// <param name="privateKeyPath">The name and path to the private key to generate.</param>
        /// <param name="keySize">The key size.</param>
        /// <returns>True if no error has been found else false.</returns>
        public bool Generate(string publicKeyPath, string privateKeyPath, int keySize = 2048)
        {
            if (String.IsNullOrEmpty(publicKeyPath)) throw new ArgumentNullException("publicKeyPath");
            if (String.IsNullOrEmpty(privateKeyPath)) throw new ArgumentNullException("privateKeyPath");

            // Create the new root certificate authority.
            bool ret = GeneratePrivateEx(privateKeyPath, keySize);
            return GeneratePublicEx(publicKeyPath, privateKeyPath);
        }

        /// <summary>
        /// Generate the public and private key pair.
        /// </summary>
        /// <param name="privateKeyPath">The name and path to the private key to generate.</param>
        /// <param name="keySize">The key size.</param>
        /// <returns>True if no error has been found else false.</returns>
        private bool GeneratePrivateEx(string privateKeyPath, int keySize = 2048)
        {
            // Construct the arguments.
            string arguments = "genrsa -out \"" + privateKeyPath + "\" " + keySize.ToString();

            // Start a new application of the openssl.exe file and pass the arguments to create a root certificate authority
            ApplicationInteractionResult? result = new ApplicationInteraction().RunApplication(_openSslExecutablePath, arguments, "", 0);
            return result != null ? true : false;
        }

        /// <summary>
        /// Generate the public and private key pair.
        /// </summary>
        /// <param name="publicKeyPath">The name and path to the public key to generate.</param>
        /// <param name="privateKeyPath">The name and path to the private key to generate.</param>
        /// <returns>True if no error has been found else false.</returns>
        private bool GeneratePublicEx(string publicKeyPath, string privateKeyPath)
        {
            // Construct the arguments.
            string arguments = "rsa -in \"" + privateKeyPath + "\" -out \"" + publicKeyPath + "\" -pubout";

            // Start a new application of the openssl.exe file and pass the arguments to create a root certificate authority
            ApplicationInteractionResult? result = new ApplicationInteraction().RunApplication(_openSslExecutablePath, arguments, "", 0);
            return result != null ? true : false;
        }
    }
}
