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
    /// Represents SSH command that can be executed.
    /// </summary>
    public class SshCommand : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SshCommand"/> class.
        /// </summary>
        /// <param name="sshClient">The SSH client.</param>
        /// <param name="commandText">The command text.</param>
        /// <exception cref="ArgumentNullException">Either <paramref name="commandText"/> is null.</exception>
        internal SshCommand(Renci.SshNet.SshClient sshClient, string commandText)
        {
            _sshCommand = sshClient.CreateCommand(commandText);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SshCommand"/> class.
        /// </summary>
        /// <param name="sshClient">The SSH client.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="encoding">The encoding to use for the results.</param>
        /// <exception cref="ArgumentNullException">Either <paramref name="commandText"/> is null.</exception>
        internal SshCommand(Renci.SshNet.SshClient sshClient, string commandText, Encoding encoding)
        {
            _sshCommand = sshClient.CreateCommand(commandText, encoding);
        }
        #endregion

        #region Private fields
        private Renci.SshNet.SshCommand _sshCommand = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the command execution result.
        /// </summary>
        public string Result
        {
            get{ return _sshCommand.Result; }
        }

        /// <summary>
        /// Gets the command execution error.
        /// </summary>
        public string Error
        {
            get{ return _sshCommand.Error; }
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        public string CommandText
        {
            get { return _sshCommand.CommandText; }
        }

        /// <summary>
        /// Gets or sets the command timeout.
        /// </summary>
        public TimeSpan CommandTimeout
        {
            get { return _sshCommand.CommandTimeout; }
            set { _sshCommand.CommandTimeout = value; }
        }

        /// <summary>
        /// Gets the command exit status.
        /// </summary>
        public int ExitStatus
        {
            get { return _sshCommand.ExitStatus; }
        }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        public Stream OutputStream
        {
            get { return _sshCommand.OutputStream; }
        }

        /// <summary>
        /// Gets the extended output stream.
        /// </summary>
        public Stream ExtendedOutputStream
        {
            get { return _sshCommand.ExtendedOutputStream; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Executes command.
        /// </summary>
        /// <returns>Command execution result.</returns>
        public string Execute()
        {
            return _sshCommand.Execute();
        }

        /// <summary>
        /// Executes the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>Command execution result</returns>
        public string Execute(string commandText)
        {
            return _sshCommand.Execute(commandText);
        }

        /// <summary>
        /// Cancels command execution in asynchronous scenarios. 
        /// </summary>
        public void CancelAsync()
        {
            _sshCommand.CancelAsync();
        }

        /// <summary>
        /// Begins an asynchronous command execution.
        /// </summary>
        /// <returns>
        /// An <see cref="System.IAsyncResult" /> that represents the asynchronous command execution, which could still be pending.
        /// </returns>
        public IAsyncResult BeginExecute()
        {
            return _sshCommand.BeginExecute();
        }

        /// <summary>
        /// Begins an asynchronous command execution.
        /// </summary>
        /// <param name="callback">An optional asynchronous callback, to be called when the command execution is complete.</param>
        /// <returns>
        /// An <see cref="System.IAsyncResult" /> that represents the asynchronous command execution, which could still be pending.
        /// </returns>
        public IAsyncResult BeginExecute(AsyncCallback callback)
        {
            return _sshCommand.BeginExecute(callback);
        }

        /// <summary>
        /// Begins an asynchronous command execution.
        /// </summary>
        /// <param name="callback">An optional asynchronous callback, to be called when the command execution is complete.</param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous read request from other requests.</param>
        /// <returns>
        /// An <see cref="System.IAsyncResult" /> that represents the asynchronous command execution, which could still be pending.
        /// </returns>
        public IAsyncResult BeginExecute(AsyncCallback callback, object state)
        {
            return _sshCommand.BeginExecute(callback, state);
        }

        /// <summary>
        /// Begins an asynchronous command execution.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="callback">An optional asynchronous callback, to be called when the command execution is complete.</param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous read request from other requests.</param>
        /// <returns>
        /// An <see cref="System.IAsyncResult" /> that represents the asynchronous command execution, which could still be pending.
        /// </returns>
        public IAsyncResult BeginExecute(string commandText, AsyncCallback callback, object state)
        {
            return _sshCommand.BeginExecute(commandText, callback, state);
        }

        /// <summary>
        /// Waits for the pending asynchronous command execution to complete.
        /// </summary>
        /// <param name="asyncResult">The reference to the pending asynchronous request to finish.</param>
        /// <returns>Command execution result.</returns>
        public string EndExecute(IAsyncResult asyncResult)
        {
            return _sshCommand.EndExecute(asyncResult);
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
                    if (_sshCommand != null)
                        _sshCommand.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _sshCommand = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SshCommand()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
