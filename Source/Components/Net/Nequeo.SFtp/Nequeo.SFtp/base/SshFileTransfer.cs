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
    /// SSH file transfer protocol.
    /// </summary>
    public class SshFtp : IDisposable
    {
        /// <summary>
        /// SSH file transfer protocol.
        /// </summary>
        /// <param name="sshConnection">SSH connection adapter.</param>
        public SshFtp(SshConnection sshConnection)
        {
            _sshConnection = sshConnection;
            _sftp = new Sftp(sshConnection.Host, sshConnection.Username);

            // Get the authentication used.
            if (sshConnection.IsPrivateKeyAuthentication)
            {
                // If a file exists.
                if (!String.IsNullOrEmpty(sshConnection.PrivateKeyFile) && !String.IsNullOrEmpty(sshConnection.PrivateKeyFilePassword))
                {
                    // Set the file.
                    _sftp.AddIdentityFile(sshConnection.PrivateKeyFile, sshConnection.PrivateKeyFilePassword);
                }

                // If a file exists.
                if (!String.IsNullOrEmpty(sshConnection.PrivateKeyFile))
                {
                    // Set the file.
                    _sftp.AddIdentityFile(sshConnection.PrivateKeyFile);
                }
            }
            else
            {
                // If a password exists.
                if (!String.IsNullOrEmpty(sshConnection.Password))
                {
                    // Set the password.
                    _sftp.Password = sshConnection.Password;
                }
            }

            // Set the events.
            _sftp.OnTransferStart += _sftp_OnTransferStart;
            _sftp.OnTransferProgress += _sftp_OnTransferProgress;
            _sftp.OnTransferEnd += _sftp_OnTransferEnd;
        }

        private Sftp _sftp = null;
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
            get { return _sftp.Connected; }
        }

        /// <summary>
        /// Gets the Cipher algorithm name used in this SSH connection.
        /// </summary>
        public string Cipher
        {
            get { return _sftp.Cipher; }
        }

        /// <summary>
        /// Gets the MAC algorithm name used in this SSH connection.
        /// </summary>
        public string Mac
        {
            get { return _sftp.Mac; }
        }

        /// <summary>
        /// Gets the server SSH version string.
        /// </summary>
        public string ServerVersion
        {
            get { return _sftp.ServerVersion; }
        }

        /// <summary>
        /// Gets the client SSH version string.
        /// </summary>
        public string ClientVersion
        {
            get { return _sftp.ClientVersion; }
        }

        /// <summary>
        /// Connect to the host.
        /// </summary>
        public void Connect()
        {
            if (!_sftp.Connected)
                _sftp.Connect(_sshConnection.Port);
        }

        /// <summary>
        /// Abort the transfer.
        /// </summary>
        public void Abort()
        {
            if (_sftp.Connected)
                _sftp.Cancel();
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        public void Close()
        {
            if (_sftp.Connected)
                _sftp.Close();
        }

        /// <summary>
        /// Send the data to the destination.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        public void Put(string sourcePath, string destinationPath)
        {
            if (_sftp.Connected)
            {
                // Send the data.
                _sftp.Put(sourcePath, destinationPath);
            }
            else
                throw new Exception("An active connection does not exist.");
        }

        /// <summary>
        /// Send the data to the destination.
        /// </summary>
        /// <param name="sourcePaths">The source paths.</param>
        /// <param name="destinationPath">The destination path.</param>
        public void Put(string[] sourcePaths, string destinationPath)
        {
            if (_sftp.Connected)
            {
                // Send the data.
                _sftp.Put(sourcePaths, destinationPath);
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
            if (_sftp.Connected)
            {
                // Receive the data.
                _sftp.Get(sourcePath, destinationPath);
            }
            else
                throw new Exception("An active connection does not exist.");
        }

        /// <summary>
        /// Receive the data from the source list.
        /// </summary>
        /// <param name="sourcePaths">The source paths.</param>
        /// <param name="destinationPath">The destination path.</param>
        public void Get(string[] sourcePaths, string destinationPath)
        {
            if (_sftp.Connected)
            {
                // Receive the data.
                _sftp.Get(sourcePaths, destinationPath);
            }
            else
                throw new Exception("An active connection does not exist.");
        }

        /// <summary>
        /// Get the file list within the path.
        /// </summary>
        /// <param name="sourcePath">THe file list sources path.</param>
        public void GetFileList(string sourcePath)
        {
            if (_sftp.Connected)
            {
                // Get the list.
                _sftp.GetFileList(sourcePath);
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
            if (_sftp.Connected)
            {
                // Get the list.
                _sftp.Mkdir(directoryPath);
            }
            else
                throw new Exception("An active connection does not exist.");
        }

        /// <summary>
        /// Delete a directory.
        /// </summary>
        /// <param name="directoryPath">The destination directory path.</param>
        public void DeleteDirectory(string directoryPath)
        {
            if (_sftp.Connected)
            {
                // Get the list.
                _sftp.RemoveDir(directoryPath);
            }
            else
                throw new Exception("An active connection does not exist.");
        }

        /// <summary>
        /// Delete a directory.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void DeleteFile(string filePath)
        {
            if (_sftp.Connected)
            {
                // Get the list.
                _sftp.RemoveFile(filePath);
            }
            else
                throw new Exception("An active connection does not exist.");
        }

        /// <summary>
        /// Rename a file or directory.
        /// </summary>
        /// <param name="currentPath">The current path.</param>
        /// <param name="newPath">The new path.</param>
        public void Rename(string currentPath, string newPath)
        {
            if (_sftp.Connected)
            {
                // Get the list.
                _sftp.Rename(currentPath, newPath);
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
                _sftp = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SshFtp()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
