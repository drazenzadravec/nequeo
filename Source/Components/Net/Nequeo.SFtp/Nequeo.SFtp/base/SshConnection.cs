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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tamir.SharpSsh;
using Renci.SshNet;

namespace Nequeo.Net.SFtp
{
    /// <summary>
    /// SSH connection handler.
    /// </summary>
    [Serializable]
    public class SshConnection : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public SshConnection()
        {
            _privateKeyFiles = new List<PrivateKeyFile>();
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
            _privateKeyFiles = new List<PrivateKeyFile>();
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
        private int _timeout = -1;
        private int _port = 22;
        private bool _isPrivateKeyAuthentication = false;
        private List<PrivateKeyFile> _privateKeyFiles = null;
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
        /// Gets, the private key files.
        /// </summary>
        public PrivateKeyFile[] PrivateKeyFiles
        {
            get { return _privateKeyFiles.ToArray(); }
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
        /// Gets, an indicator specifying if private key authentication is used; else password authentication is used.
        /// </summary>
        public bool IsPrivateKeyAuthentication
        {
            get { return _isPrivateKeyAuthentication; }
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
        /// <param name="privateKeyFilePassword">The private key file password; null for no passphrase.</param>
        public void SetPrivateKeyFile(string privateKeyFile, string privateKeyFilePassword)
        { 
            _isPrivateKeyAuthentication = true;
            if (String.IsNullOrEmpty(privateKeyFilePassword))
                _privateKeyFiles.Add(new PrivateKeyFile(privateKeyFile));
            else
                _privateKeyFiles.Add(new PrivateKeyFile(privateKeyFile, privateKeyFilePassword));
        }

        /// <summary>
        /// Set the private key stream used for authentication.
        /// </summary>
        /// <param name="privateKeyStream">The private key stream used for authentication.</param>
        /// <param name="privateKeyFilePassword">The private key file password; null for no passphrase.</param>
        public void SetPrivateKeyFile(Stream privateKeyStream, string privateKeyFilePassword)
        {
            _isPrivateKeyAuthentication = true;
            if (String.IsNullOrEmpty(privateKeyFilePassword))
                _privateKeyFiles.Add(new PrivateKeyFile(privateKeyStream));
            else
                _privateKeyFiles.Add(new PrivateKeyFile(privateKeyStream, privateKeyFilePassword));
        }

        #endregion

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_privateKeyFiles != null)
                    {
                        // For each private key file.
                        foreach (PrivateKeyFile keyfile in _privateKeyFiles)
                        {
                            try
                            {
                                // Dispose of the resource.
                                if (keyfile != null)
                                    keyfile.Dispose();
                            }
                            catch { }
                        }
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _privateKeyFiles = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SshConnection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Represents private key information.
    /// </summary>
    public class PrivateKeyFile : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Represents private key information.
        /// </summary>
        /// <param name="privateKey">The private key stream used for authentication.</param>
        internal PrivateKeyFile(Stream privateKey)
        {
            _privateKeyFile = new Renci.SshNet.PrivateKeyFile(privateKey);
        }

        /// <summary>
        /// Represents private key information.
        /// </summary>
        /// <param name="privateKey">The private key file used for authentication.</param>
        internal PrivateKeyFile(string privateKey)
        {
            _privateKey = privateKey;
            _privateKeyFile = new Renci.SshNet.PrivateKeyFile(privateKey);
        }

        /// <summary>
        /// Represents private key information.
        /// </summary>
        /// <param name="privateKey">The private key file used for authentication.</param>
        /// <param name="privateKeyFilePassword">The private key file password.</param>
        internal PrivateKeyFile(string privateKey, string privateKeyFilePassword)
        {
            _privateKey = privateKey;
            _privateKeyPassword = privateKeyFilePassword;
            _privateKeyFile = new Renci.SshNet.PrivateKeyFile(privateKey, privateKeyFilePassword);
        }

        /// <summary>
        /// Represents private key information.
        /// </summary>
        /// <param name="privateKey">The private key stream used for authentication.</param>
        /// <param name="privateKeyFilePassword">The private key file password.</param>
        internal PrivateKeyFile(Stream privateKey, string privateKeyFilePassword)
        {
            _privateKeyFile = new Renci.SshNet.PrivateKeyFile(privateKey, privateKeyFilePassword);
        }
        #endregion

        #region Private fields
        private Renci.SshNet.PrivateKeyFile _privateKeyFile = null;
        private string _privateKey = string.Empty;
        private string _privateKeyPassword = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the private key file.
        /// </summary>
        public string PrivateKey
        {
            get { return _privateKey; }
        }

        /// <summary>
        /// Gets, the private key file password.
        /// </summary>
        public string PrivateKeyPassword
        {
            get { return _privateKeyPassword; }
        }
        #endregion

        #region Internal Properties
        /// <summary>
        /// Gets, the SSH private key file.
        /// </summary>
        internal Renci.SshNet.PrivateKeyFile SshNetPrivateKeyFile
        {
            get { return _privateKeyFile; }
        }
        #endregion

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_privateKeyFile != null)
                        _privateKeyFile.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _privateKeyFile = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~PrivateKeyFile()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
