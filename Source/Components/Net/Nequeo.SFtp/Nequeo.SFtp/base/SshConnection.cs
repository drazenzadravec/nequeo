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
using System.Threading.Tasks;

using Tamir.SharpSsh;

namespace Nequeo.Net.SFtp
{
    /// <summary>
    /// SSH connection handler.
    /// </summary>
    [Serializable]
    public class SshConnection
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public SshConnection()
        {
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="host">The sftp server.</param>
        /// <param name="username">The user name.</param>
        /// <param name="port">The sftp port.</param>
        /// <param name="timeout">The time out request.</param>
        public SshConnection(string host, string username, int port = 22, int timeout = -1)
        {
            _host = host;
            _username = username;
            _timeout = timeout;
            _port = port;
        }
        #endregion

        #region Private Fields
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _host = string.Empty;
        private string _privateKeyFile = string.Empty;
        private string _privateKeyFilePassword = string.Empty;
        private int _timeout = -1;
        private int _port = 22;
        private bool _isPrivateKeyAuthentication = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets, the user name.
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        /// <summary>
        /// Gets, the password.
        /// </summary>
        public string Password
        {
            get { return _password; }
        }

        /// <summary>
        /// Gets, the private key file.
        /// </summary>
        public string PrivateKeyFile
        {
            get { return _privateKeyFile; }
        }

        /// <summary>
        /// Gets, the private key file password.
        /// </summary>
        public string PrivateKeyFilePassword
        {
            get { return _privateKeyFilePassword; }
        }

        /// <summary>
        /// Gets or sets, the sftp server.
        /// </summary>
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        /// <summary>
        /// Gets or sets, the sftp port.
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Gets or sets, the time out request.
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Gets or sets, an indicator specifying if private key authentication is used; else password authentication is used.
        /// </summary>
        public bool IsPrivateKeyAuthentication
        {
            get { return _isPrivateKeyAuthentication; }
            set { _isPrivateKeyAuthentication = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set the password used for authentication.
        /// </summary>
        /// <param name="password">The password used for authentication.</param>
        public void SetPassword(string password)
        {
            _password = password;
            _isPrivateKeyAuthentication = false;
        }

        /// <summary>
        /// Set the private key file used for authentication.
        /// </summary>
        /// <param name="privateKeyFile">The private key file used for authentication.</param>
        /// <param name="privateKeyFilePassword">The private key file password.</param>
        public void SetPrivateKeyFile(string privateKeyFile, string privateKeyFilePassword)
        {
            _privateKeyFile = privateKeyFile;
            _privateKeyFilePassword = privateKeyFilePassword;
            _isPrivateKeyAuthentication = true;
        }
        #endregion
    }
}
