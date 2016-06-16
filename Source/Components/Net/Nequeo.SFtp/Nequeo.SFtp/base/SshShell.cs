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
    /// Represents instance of the SSH shell object.
    /// </summary>
    public class SshShell : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Represents instance of the SSH shell object.
        /// </summary>
        /// <param name="sshClient">The SSH client.</param>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <param name="extendedOutput">The extended output.</param>
        /// <param name="terminalName">Name of the terminal.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="bufferSize">Size of the buffer for output stream.</param>
        internal SshShell(Renci.SshNet.SshClient sshClient, Stream input, Stream output, Stream extendedOutput, 
            string terminalName, uint columns, uint rows, uint width, uint height, int bufferSize)
        {
            _sshShell = sshClient.CreateShell(input, output, extendedOutput, terminalName, columns, rows, width, height, null, bufferSize);
            _sshShell.Starting += _sshShell_Starting;
            _sshShell.Started += _sshShell_Started;
            _sshShell.Stopping += _sshShell_Stopping;
            _sshShell.Stopped += _sshShell_Stopped;
        }
        #endregion

        #region Private fields
        private Renci.SshNet.Shell _sshShell = null;
        #endregion

        #region Public Events
        /// <summary>
        /// Occurs when shell is starting.
        /// </summary>
        public event EventHandler<EventArgs> Starting;

        /// <summary>
        /// Occurs when shell is started.
        /// </summary>
        public event EventHandler<EventArgs> Started;

        /// <summary>
        /// Occurs when shell is stopping.
        /// </summary>
        public event EventHandler<EventArgs> Stopping;

        /// <summary>
        /// Occurs when shell is stopped.
        /// </summary>
        public event EventHandler<EventArgs> Stopped;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a value indicating whether this shell is started.
        /// </summary>
        /// <value>
        /// <c>true</c> if started is started; otherwise, <c>false</c>.
        /// </value>
        public bool IsStarted
        {
            get { return _sshShell.IsStarted; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts this shell.
        /// </summary>
        public void Start()
        {
            _sshShell.Start();
        }

        /// <summary>
        /// Stops this shell.
        /// </summary>
        public void Stop()
        {
            _sshShell.Stop();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _sshShell_Stopped(object sender, EventArgs e)
        {
            Stopped?.Invoke(this, e);
        }

        /// <summary>
        /// Stopping.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _sshShell_Stopping(object sender, EventArgs e)
        {
            Stopping?.Invoke(this, e);
        }

        /// <summary>
        /// Started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _sshShell_Started(object sender, EventArgs e)
        {
            Started?.Invoke(this, e);
        }

        /// <summary>
        /// Starting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _sshShell_Starting(object sender, EventArgs e)
        {
            Starting?.Invoke(this, e);
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
                    if (_sshShell != null)
                        _sshShell.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _sshShell = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SshShell()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
