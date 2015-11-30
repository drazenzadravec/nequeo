/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Net
{
    /// <summary>
    /// Generic net UDP server.
    /// </summary>
    public class UdpServer : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Generic net server.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public UdpServer(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
        {
            _maxNumClients = maxNumClients;
            _multiEndpointModels = multiEndpointModels;
            Initialise();
        }
        #endregion

        #region Private Fields
        private Nequeo.Net.WebUdpServer _server = null;

        private int _maxNumClients = Int32.MaxValue;
        private Net.Sockets.MultiEndpointModel[] _multiEndpointModels = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the read buffer size.
        /// </summary>
        public int ReadBufferSize
        {
            get { return _server.ReadBufferSize; }
            set { _server.ReadBufferSize = value; }
        }

        /// <summary>
        /// Gets sets, the write buffer size.
        /// </summary>
        public int WriteBufferSize
        {
            get { return _server.WriteBufferSize; }
            set { _server.WriteBufferSize = value; }
        }

        /// <summary>
        /// Gets the socket server endpoints.
        /// </summary>
        public Nequeo.Net.WebUdpServer Servers
        {
            get { return _server; }
        }

        /// <summary>
        /// Gets, the is listening server indicator.
        /// </summary>
        public bool IsListening
        {
            get { return _server.IsListening; }
        }

        /// <summary>
        /// Gets or sets the timeout (seconds) for each client connection when in-active.
        /// Disconnects the client when this time out is triggered.
        /// </summary>
        public int Timeout
        {
            get { return _server.Timeout; }
            set { _server.Timeout = value; }
        }

        /// <summary>
        /// Gets or sets the length of time, in milliseconds, before the request times out; -1 wait indefinitely.
        /// </summary>
        public int RequestTimeout
        {
            get { return _server.RequestTimeout; }
            set { _server.RequestTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the length of time, in milliseconds, before the response times out; -1 wait indefinitely.
        /// </summary>
        public int ResponseTimeout
        {
            get { return _server.ResponseTimeout; }
            set { _server.ResponseTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the current server name.
        /// </summary>
        public string Name
        {
            get { return _server.Name; }
            set { _server.Name = value; }
        }

        /// <summary>
        /// Gets or sets the common service name.
        /// </summary>
        public string ServiceName
        {
            get { return _server.ServiceName; }
            set { _server.ServiceName = value; }
        }

        /// <summary>
        /// Gets or sets the maximum request buffer capacity when using buffered stream.
        /// </summary>
        public int RequestBufferCapacity
        {
            get { return _server.RequestBufferCapacity; }
            set { _server.RequestBufferCapacity = value; }
        }

        /// <summary>
        /// Gets or sets the maximum response buffer capacity when using buffered stream.
        /// </summary>
        public int ResponseBufferCapacity
        {
            get { return _server.ResponseBufferCapacity; }
            set { _server.ResponseBufferCapacity = value; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// The on web context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event EventHandler<Nequeo.Net.WebContext> OnContext;

        #endregion

        #region Public Methods
        /// <summary>
        /// Start the server listener.
        /// </summary>
        public void Start()
        {
            _server.Start();
        }

        /// <summary>
        /// Stop the server listener.
        /// </summary>
        public void Stop()
        {
            _server.Stop();
        }
        #endregion

        #region Private Base server initialiser members
        /// <summary>
        /// Initialise.
        /// </summary>
        private void Initialise()
        {
            // Create the server.
            _server = new WebUdpServer(_multiEndpointModels, _maxNumClients);

            // Assign the on connect action handler.
            _server.Timeout = 60;
            _server.RequestTimeout = 30000;
            _server.ResponseTimeout = 30000;
            _server.ReadBufferSize = 32768;
            _server.WriteBufferSize = 32768;
            _server.ResponseBufferCapacity = 10000000;
            _server.RequestBufferCapacity = 10000000;
            _server.Name = "Generic Net UDP Server";
            _server.ServiceName = "GenericNetUdpServer";
            _server.OnWebContext += ManagerServer_OnWebContext;

            // Initialise the server.
            _server.Initialisation();
        }

        /// <summary>
        /// On web context receive handler.
        /// </summary>
        /// <param name="sender">The application sender.</param>
        /// <param name="context">The current web context.</param>
        private void ManagerServer_OnWebContext(object sender, Net.WebContext context)
        {
            // If the event has been assigned.
            if (OnContext != null)
                OnContext(this, context);
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
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
        /// Dispose(bool disposing) executes in two distinct scenarios.  If disposing
        /// equals true, the method has been called directly or indirectly by a user's
        /// code. Managed and unmanaged resources can be disposed.  If disposing equals
        /// false, the method has been called by the runtime from inside the finalizer
        /// and you should not reference other objects. Only unmanaged resources can
        /// be disposed.
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
                    // Dispose managed resources.
                    if (_server != null)
                        _server.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _server = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~UdpServer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
