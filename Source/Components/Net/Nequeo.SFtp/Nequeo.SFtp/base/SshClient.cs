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

using Renci.SshNet;

namespace Nequeo.Net.SFtp
{
    /// <summary>
    /// Secure shell host client.
    /// </summary>
    public class SshClient : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Secure shell host client.
        /// </summary>
        /// <param name="sshConnection">SSH connection adapter.</param>
        public SshClient(SshConnection sshConnection)
        {
            _sshConnection = sshConnection;

            // Get the authentication used.
            if (sshConnection.IsPrivateKeyAuthentication)
            {
                // Add the private key files.
                List<Renci.SshNet.PrivateKeyFile> privateKeyFile = new List<Renci.SshNet.PrivateKeyFile>();
                foreach (PrivateKeyFile keyFile in sshConnection.PrivateKeyFiles)
                    privateKeyFile.Add(keyFile.SshNetPrivateKeyFile);

                // If port number is 22 (default).
                if (sshConnection.Port == 22)
                    _sshClient = new Renci.SshNet.SshClient(sshConnection.Host, sshConnection.Username, privateKeyFile.ToArray());
                else
                    _sshClient = new Renci.SshNet.SshClient(sshConnection.Host, sshConnection.Port, sshConnection.Username, privateKeyFile.ToArray());
            }
            else
            {
                // If port number is 22 (default).
                if (sshConnection.Port == 22)
                    _sshClient = new Renci.SshNet.SshClient(sshConnection.Host, sshConnection.Username, sshConnection.Password);
                else
                    _sshClient = new Renci.SshNet.SshClient(sshConnection.Host, sshConnection.Port, sshConnection.Username, sshConnection.Password);
            }
        }
        #endregion

        #region Private fields
        private Renci.SshNet.SshClient _sshClient = null;
        private SshConnection _sshConnection = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a value indicating whether this client is connected to the server.
        /// </summary>
        public bool IsConnected
        {
            get { return _sshClient.IsConnected; }
        }

        /// <summary>
        /// Gets or sets the keep-alive interval.
        /// </summary>
        public TimeSpan KeepAliveInterval
        {
            get { return _sshClient.KeepAliveInterval; }
            set { _sshClient.KeepAliveInterval = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates the shell.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <param name="extendedOutput">The extended output.</param>
        /// <returns>Returns a representation of a <see cref="SshShell" /> object.</returns>
        public SshShell CreateShell(Stream input, Stream output, Stream extendedOutput)
        {
            return new SshShell(_sshClient, input, output, extendedOutput, string.Empty, 0, 0, 0, 0, 1024);
        }

        /// <summary>
        /// Creates the shell.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <param name="extendedOutput">The extended output.</param>
        /// <param name="terminalName">Name of the terminal.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Returns a representation of a <see cref="SshShell" /> object.</returns>
        public SshShell CreateShell(Stream input, Stream output, Stream extendedOutput, string terminalName, uint columns, uint rows, uint width, uint height)
        {
            return new SshShell(_sshClient, input, output, extendedOutput, terminalName, columns, rows, width, height, 1024);
        }

        /// <summary>
        /// Creates the shell.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <param name="extendedOutput">The extended output.</param>
        /// <param name="terminalName">Name of the terminal.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="bufferSize">Size of the internal read buffer.</param>
        /// <returns>Returns a representation of a <see cref="SshShell" /> object.</returns>
        public SshShell CreateShell(Stream input, Stream output, Stream extendedOutput, string terminalName, uint columns, uint rows, uint width, uint height, int bufferSize)
        {
            return new SshShell(_sshClient, input, output, extendedOutput, terminalName, columns, rows, width, height, bufferSize);
        }

        /// <summary>
        /// Creates the command to be executed.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns><see cref="SshCommand"/> object.</returns>
        public SshCommand CreateCommand(string commandText)
        {
            return new SshCommand(_sshClient, commandText);
        }

        /// <summary>
        /// Creates the command to be executed with specified encoding.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="encoding">The encoding to use for results.</param>
        /// <returns><see cref="SshCommand"/> object.</returns>
        public SshCommand CreateCommand(string commandText, Encoding encoding)
        {
            return new SshCommand(_sshClient, commandText, encoding);
        }

        /// <summary>
        /// Creates and executes the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns><see cref="SshCommand"/> object.</returns>
        public SshCommand RunCommand(string commandText)
        {
            SshCommand sshCommand = new SshCommand(_sshClient, commandText);
            sshCommand.Execute();
            return sshCommand;
        }

        /// <summary>
        /// Connects client to the server.
        /// </summary>
        public void Connect()
        {
            _sshClient.Connect();
        }

        /// <summary>
        /// Disconnects client from the server.
        /// </summary>
        public void Disconnect()
        {
            _sshClient.Disconnect();
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
                    if (_sshClient != null)
                        _sshClient.Dispose();

                    if (_sshConnection != null)
                        _sshConnection.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _sshClient = null;
                _sshConnection = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SshClient()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
