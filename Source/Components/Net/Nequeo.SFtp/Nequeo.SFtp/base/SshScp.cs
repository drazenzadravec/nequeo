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
    /// SSH Secure copy protocol.
    /// </summary>
    public class SshScp : IDisposable
    {
        /// <summary>
        /// SSH Secure copy protocol.
        /// </summary>
        /// <param name="sshConnection">SSH connection adapter.</param>
        public SshScp(SshConnection sshConnection)
        {
            _sshConnection = sshConnection;
            _scp = new Scp(sshConnection.Host, sshConnection.Username);

            // Get the authentication used.
            if (sshConnection.IsPrivateKeyAuthentication)
            {
                // If a file exists.
                if (!String.IsNullOrEmpty(sshConnection.PrivateKeyFile) && !String.IsNullOrEmpty(sshConnection.PrivateKeyFilePassword))
                {
                    // Set the file.
                    _scp.AddIdentityFile(sshConnection.PrivateKeyFile, sshConnection.PrivateKeyFilePassword);
                }

                // If a file exists.
                if (!String.IsNullOrEmpty(sshConnection.PrivateKeyFile))
                {
                    // Set the file.
                    _scp.AddIdentityFile(sshConnection.PrivateKeyFile);
                }
            }
            else
            {
                // If a password exists.
                if (!String.IsNullOrEmpty(sshConnection.Password))
                {
                    // Set the password.
                    _scp.Password = sshConnection.Password;
                }
            }

            // Set the events.
            _scp.OnTransferStart += _sftp_OnTransferStart;
            _scp.OnTransferProgress += _sftp_OnTransferProgress;
            _scp.OnTransferEnd += _sftp_OnTransferEnd;
        }

        private Scp _scp = null;
        private SshConnection _sshConnection = null;

        /// <summary>
        /// Start a new transfer.
        /// </summary>
        public event EventHandler<string> OnStartTransfer;

        /// <summary>
        /// Transfer ended.
        /// </summary>
        public event EventHandler<string> OnEndTransfer;

        /// <summary>
        /// Transfer progress.
        /// </summary>
        public event Nequeo.Threading.EventHandler<int, string> OnTransferProgress;

        /// <summary>
        /// Gets an indicator specifying if an active connection exists.
        /// </summary>
        public bool IsConnected
        {
            get { return _scp.Connected; }
        }

        /// <summary>
        /// Gets the Cipher algorithm name used in this SSH connection.
        /// </summary>
        public string Cipher
        {
            get { return _scp.Cipher; }
        }

        /// <summary>
        /// Gets the MAC algorithm name used in this SSH connection.
        /// </summary>
        public string Mac
        {
            get { return _scp.Mac; }
        }

        /// <summary>
        /// Gets the server SSH version string.
        /// </summary>
        public string ServerVersion
        {
            get { return _scp.ServerVersion; }
        }

        /// <summary>
        /// Gets the client SSH version string.
        /// </summary>
        public string ClientVersion
        {
            get { return _scp.ClientVersion; }
        }

        /// <summary>
        /// Connect to the host.
        /// </summary>
        public void Connect()
        {
            if (!_scp.Connected)
                _scp.Connect(_sshConnection.Port);
        }

        /// <summary>
        /// Abort the transfer.
        /// </summary>
        public void Abort()
        {
            if (_scp.Connected)
                _scp.Cancel();
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        public void Close()
        {
            if (_scp.Connected)
                _scp.Close();
        }

        /// <summary>
        /// Send the data to the destination.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        public void Put(string sourcePath, string destinationPath)
        {
            if (_scp.Connected)
            {
                // Send the data.
                _scp.Put(sourcePath, destinationPath);
            }
            else
                throw new Exception("An active connection does not exist.");
        }

        /// <summary>
        /// Receive the data from the source.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        public void Get(string sourcePath, string destinationPath)
        {
            if (_scp.Connected)
            {
                // Receive the data.
                _scp.Get(sourcePath, destinationPath);
            }
            else
                throw new Exception("An active connection does not exist.");
        }

        /// <summary>
        /// Copies a file from a remote SSH machine to the local machine using SCP.
        /// </summary>
        /// <param name="remoteFile">The remmote file name.</param>
        /// <param name="localPath">The local destination path.</param>
        /// <param name="recursive">Indicating whether a recursive transfer should take place.</param>
        public void From(string remoteFile, string localPath, bool recursive = false)
        {
            if (_scp.Connected)
            {
                // Receive the data.
                _scp.From(remoteFile, localPath, recursive);
            }
            else
                throw new Exception("An active connection does not exist.");
        }

        /// <summary>
        /// Create a new directory.
        /// </summary>
        /// <param name="directoryPath">The destination directory path.</param>
        public void CreateDirectory(string directoryPath)
        {
            if (_scp.Connected)
            {
                // Get the list.
                _scp.Mkdir(directoryPath);
            }
            else
                throw new Exception("An active connection does not exist.");
        }

        /// <summary>
        /// Copies a file from local machine to a remote SSH machine.
        /// </summary>
        /// <param name="localPath">The local file path.</param>
        /// <param name="remotePath">The path of the remote file.</param>
        /// <param name="recursive">Indicating whether a recursive transfer should take place.</param>
        public void To(string localPath, string remotePath, bool recursive = false)
        {
            if (_scp.Connected)
            {
                // Receive the data.
                _scp.To(localPath, remotePath, recursive);
            }
            else
                throw new Exception("An active connection does not exist.");
        }

        /// <summary>
        /// On transfer ended.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="dst">The destination.</param>
        /// <param name="transferredBytes">The transferred bytes.</param>
        /// <param name="totalBytes">The total bytes.</param>
        /// <param name="message">A messsage.</param>
        private void _sftp_OnTransferEnd(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            if (OnEndTransfer != null)
                OnEndTransfer(this, message);
        }

        /// <summary>
        /// On transfer progress.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="dst">The destination.</param>
        /// <param name="transferredBytes">The transferred bytes.</param>
        /// <param name="totalBytes">The total bytes.</param>
        /// <param name="message">A messsage.</param>
        private void _sftp_OnTransferProgress(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            if (OnTransferProgress != null)
                OnTransferProgress(this, transferredBytes, message);
        }

        /// <summary>
        /// On transfer started.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="dst">The destination.</param>
        /// <param name="transferredBytes">The transferred bytes.</param>
        /// <param name="totalBytes">The total bytes.</param>
        /// <param name="message">A messsage.</param>
        private void _sftp_OnTransferStart(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            if (OnStartTransfer != null)
                OnStartTransfer(this, message);
        }

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
                    
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _scp = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SshScp()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
