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
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Security
{
    /// <summary>
    /// This holds the user credentials.
    /// </summary>
    public class UserCredentials
    {
        /// <summary>
        /// Gets sets, the username.
        /// </summary>
        public String Username { set; get; }

        /// <summary>
        /// Gets sets, the password.
        /// </summary>
        public String Password { set; get; }

        /// <summary>
        /// Gets sets, the domain.
        /// </summary>
        public String Domain { set; get; }

        /// <summary>
        /// Gets sets, the authorisation type.
        /// </summary>
        public Nequeo.Security.AuthorisationType AuthorisationType { set; get; }

        /// <summary>
        /// Gets sets, the authentication type.
        /// </summary>
        public Nequeo.Security.AuthenticationType AuthenticationType { set; get; }

        /// <summary>
        /// Gets sets, the authorisation credentials.
        /// </summary>
        public Nequeo.Security.AuthorisationCredentials AuthorisationCredentials { set; get; }

        /// <summary>
        /// Gets sets, the application name.
        /// </summary>
        public String ApplicationName { set; get; }

        /// <summary>
        /// Gets or sets the permission levels.
        /// </summary>
        public Nequeo.Security.PermissionType Permission { set; get; }

        /// <summary>
        /// Gets or sets the password encryption provider.
        /// </summary>
        public Nequeo.Cryptography.IPasswordEncryption PasswordEncryption { set; get; }

        /// <summary>
        /// Gets or sets the secure password.
        /// </summary>
        public SecureString SecurePassword { set; get; }

        /// <summary>
        /// Set the default SecurePassword from the PasswordEncryption Decrypt method.
        /// </summary>
        public void SetDefaultSecurePassword()
        {
            if (PasswordEncryption != null)
            {
                // Get the encoded and encrypted password.
                string password = PasswordEncryption.Decrypt(Password);

                // Get the certificate path details and create
                // the x509 certificate reference.
                SecurePassword = new Nequeo.Security.SecureText().GetSecureText(password);
            }
        }

        /// <summary>
        /// Set the default SecurePassword from the PasswordEncryption Decrypt method.
        /// </summary>
        /// <param name="key">The key used to decrypt the password.</param>
        public void SetDefaultSecurePassword(string key)
        {
            if (PasswordEncryption != null)
            {
                // Get the encoded and encrypted password.
                string password = PasswordEncryption.Decrypt(Password, key);

                // Get the certificate path details and create
                // the x509 certificate reference.
                SecurePassword = new Nequeo.Security.SecureText().GetSecureText(password);
            }
        }
    }

    /// <summary>
    /// This holds the authorisation credentials.
    /// </summary>
    public class AuthorisationCredentials
    {
        /// <summary>
        /// Gets sets, the username.
        /// </summary>
        public String Username { set; get; }

        /// <summary>
        /// Gets sets, the password.
        /// </summary>
        public String Password { set; get; }

        /// <summary>
        /// Gets sets, the server.
        /// </summary>
        public String Server { set; get; }

        /// <summary>
        /// Gets sets, the secure connection.
        /// </summary>
        public Boolean SecureConnection { set; get; }

        /// <summary>
        /// Gets sets, the containerDN.
        /// </summary>
        public String ContainerDN { set; get; }

        /// <summary>
        /// Gets or sets the password encryption provider.
        /// </summary>
        public Nequeo.Cryptography.IPasswordEncryption PasswordEncryption { set; get; }

        /// <summary>
        /// Gets or sets the secure password.
        /// </summary>
        public SecureString SecurePassword { set; get; }

        /// <summary>
        /// Set the default SecurePassword from the PasswordEncryption Decrypt method.
        /// </summary>
        public void SetDefaultSecurePassword()
        {
            if (PasswordEncryption != null)
            {
                // Get the encoded and encrypted password.
                string password = PasswordEncryption.Decrypt(Password);

                // Get the certificate path details and create
                // the x509 certificate reference.
                SecurePassword = new Nequeo.Security.SecureText().GetSecureText(password);
            }
        }

        /// <summary>
        /// Set the default SecurePassword from the PasswordEncryption Decrypt method.
        /// </summary>
        /// <param name="key">The key used to decrypt the password.</param>
        public void SetDefaultSecurePassword(string key)
        {
            if (PasswordEncryption != null)
            {
                // Get the encoded and encrypted password.
                string password = PasswordEncryption.Decrypt(Password, key);

                // Get the certificate path details and create
                // the x509 certificate reference.
                SecurePassword = new Nequeo.Security.SecureText().GetSecureText(password);
            }
        }
    }

    /// <summary>
    /// This holds the X509Certificate2 credentials.
    /// </summary>
    public class X509Certificate2Model
    {
        /// <summary>
        /// Gets sets, the path.
        /// </summary>
        public String Path { set; get; }

        /// <summary>
        /// Gets sets, the password.
        /// </summary>
        public String Password { set; get; }

        /// <summary>
        /// Gets sets, use a server certificate.
        /// </summary>
        public Boolean UseServerCertificate { set; get; }

        /// <summary>
        /// Gets or sets the password encryption provider.
        /// </summary>
        public Nequeo.Cryptography.IPasswordEncryption PasswordEncryption { set; get; }

        /// <summary>
        /// Gets or sets the X509 certificate.
        /// </summary>
        public X509Certificate2 X509Certificate { set; get; }

        /// <summary>
        /// Gets or sets the secure password.
        /// </summary>
        public SecureString SecurePassword { set; get; }

        /// <summary>
        /// Set the default X509 certificate from the PasswordEncryption Decrypt method.
        /// </summary>
        public void SetDefaultX509Certificate()
        {
            if (PasswordEncryption != null)
            {
                // Get the encoded and encrypted password.
                string password = PasswordEncryption.Decrypt(Password);

                // Get the certificate path details and create
                // the x509 certificate reference.
                SecurePassword = new Nequeo.Security.SecureText().GetSecureText(password);

                // Should the server certificate be used.
                if (UseServerCertificate)
                {
                    // Get the certificate path details and create
                    // the x509 certificate reference.
                    X509Certificate = X509Certificate2Store.GetCertificate(Path, password);
                }
            }
        }

        /// <summary>
        /// Set the default X509 certificate from the PasswordEncryption Decrypt method.
        /// </summary>
        /// <param name="key">The key used to decrypt the password.</param>
        public void SetDefaultX509Certificate(string key)
        {
            if (PasswordEncryption != null)
            {
                // Get the encoded and encrypted password.
                string password = PasswordEncryption.Decrypt(Password, key);

                // Get the certificate path details and create
                // the x509 certificate reference.
                SecurePassword = new Nequeo.Security.SecureText().GetSecureText(password);

                // Should the server certificate be used.
                if (UseServerCertificate)
                {
                    // Get the certificate path details and create
                    // the x509 certificate reference.
                    X509Certificate = X509Certificate2Store.GetCertificate(Path, password);
                }   
            }
        }
    }

    /// <summary>
    /// This enum holds the authentication type.
    /// </summary>
    [Flags]
    public enum AuthenticationType : long
    {
        /// <summary>
        /// No authentication.
        /// </summary>
        None = 0,
        /// <summary>
        /// Anonymous type authentication.
        /// </summary>
        Anonymous = 1,
        /// <summary>
        /// Integrated type authentication.
        /// </summary>
        Integrated = 2,
        /// <summary>
        /// SQL type authentication.
        /// </summary>
        SQL = 4,
        /// <summary>
        /// Basic type authentication.
        /// </summary>
        Basic = 8,
        /// <summary>
        /// Digest type authentication.
        /// </summary>
        Digest = 16,
        /// <summary>
        /// NTLM type authentication.
        /// </summary>
        NTLM = 32,
        /// <summary>
        /// Passport type authentication.
        /// </summary>
        Passport = 64,
        /// <summary>
        /// Kerberos type authentication.
        /// </summary>
        Kerberos = 128,
        /// <summary>
        /// User type authentication.
        /// </summary>
        User = 256,
        /// <summary>
        /// Bearer type authentication.
        /// </summary>
        Bearer = 512,
        /// <summary>
        /// Device type authentication.
        /// </summary>
        Device = 1024,
    }

    /// <summary>
    /// This enum holds the authorisation type.
    /// </summary>
    [Flags]
    public enum AuthorisationType : long
    {
        /// <summary>
        /// No authorisation.
        /// </summary>
        None = 0,
        /// <summary>
        /// The local computer store. This represents the SAM store.
        /// </summary>
        Machine = 1,
        /// <summary>
        /// The domain controller store. This represents the AD DS store.
        /// </summary>
        Domain = 2,
        /// <summary>
        /// The application directory store. This represents the AD LDS store.
        /// </summary>
        ApplicationDirectory = 4,
        /// <summary>
        /// A sqwl store.
        /// </summary>
        SQL = 8,
        /// <summary>
        /// A no-sql store
        /// </summary>
        NoSQL = 16,
        /// <summary>
        /// An xml store.
        /// </summary>
        Xml = 32,
        /// <summary>
        /// A general text store.
        /// </summary>
        Text = 64,
        /// <summary>
        /// A general proxy server.
        /// </summary>
        Proxy = 128,
    }
}
