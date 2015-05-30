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
using System.Threading;

namespace Nequeo.Management.NetTcp
{
    /// <summary>
    /// Class that controls all custom server instances
    /// and threads.
    /// </summary>
    public class NetTcpHostControl : IDisposable
    {
        #region Net TCP Host Controller

        private NetTcpHost streamHost = null;

        private Thread threadStreamHost = null;

        /// <summary>
        /// Initialse all objects, create new
        /// instances of all servers and cleanup
        /// objects when complete.
        /// </summary>
        /// <param name="create">Create a new instance.</param>
        /// <param name="serviceHostType">The service host type</param>
        private void Initialise(bool create, Type serviceHostType)
        {
            // Create new instances.
            if (create)
            {
                // Create a new file transfer host
                // with default configuration setting.
                streamHost = new NetTcpHost(serviceHostType);
            }
            else
            {
                // Dispose of all the servers.
                if (streamHost != null)
                    streamHost.Dispose();

                // Cleanup threads.
                threadStreamHost = null;
            }
        }

        /// <summary>
        /// Starts all server threads.
        /// </summary>
        public void StartServerThreads(Type serviceHostType)
        {
            // Initialise all custom server
            // instances.
            Initialise(true, serviceHostType);

            // Create new threads for each
            // file transfer server.
            threadStreamHost = new Thread(new ThreadStart(streamHost.OpenServiceHost));
            threadStreamHost.IsBackground = true;
            threadStreamHost.Start();
            Thread.Sleep(20);
        }

        /// <summary>
        /// Stop all server from listening and
        /// abort all server threads.
        /// </summary>
        public void StopServerThreads()
        {
            // Stop all file transfer
            // servers from listening.
            if (streamHost != null)
                streamHost.CloseServiceHost();

            // Abort all threads created
            // for file transfer instances.
            if (threadStreamHost != null)
                if (threadStreamHost.IsAlive)
                {
                    threadStreamHost.Abort();
                    threadStreamHost.Join();
                    Thread.Sleep(20);
                }

            // Clean up objects.
            Initialise(false, null);
        }

        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
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
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (streamHost != null)
                        streamHost.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                streamHost = null;
                threadStreamHost = null;

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~NetTcpHostControl()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
