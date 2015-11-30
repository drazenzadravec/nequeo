/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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

using Nequeo.Net.Provider;
using Nequeo.Net.Http;

namespace Nequeo.Net
{
    /// <summary>
    /// Custom net server.
    /// </summary>
    public class CustomServer : IDisposable
    {
        /// <summary>
        /// Custom net server.
        /// </summary>
        /// <param name="serverContextType">The server context type, must inherit base Nequeo.Net.Sockets.ServerContext.</param>
        /// <param name="port">The port number to listen on.</param>
        /// <param name="securePort">The port number to listen on.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public CustomServer(Type serverContextType, int port, int securePort = -1, int maxNumClients = Int32.MaxValue)
        {
            // Add new endpoints
            List<Nequeo.Net.Sockets.MultiEndpointModel> model = new List<Nequeo.Net.Sockets.MultiEndpointModel>();

            // Add non-secure endpoint.
            model.Add(
                    // None secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = port,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    });

            // If a secure port exists.
            if (securePort > -1)
            {
                // Add secure endpoint.
                model.Add(
                    // None secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = securePort,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    });
            }

            _serverContextType = serverContextType;
            _maxNumClients = maxNumClients;
            _multiEndpointModels = model.ToArray();
            Initialise();
        }

        /// <summary>
        /// Custom net server.
        /// </summary>
        /// <param name="serverContextType">The server context type, must inherit base Nequeo.Net.Sockets.ServerContext.</param>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public CustomServer(Type serverContextType, Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
        {
            _serverContextType = serverContextType;
            _maxNumClients = maxNumClients;
            _multiEndpointModels = multiEndpointModels;
            Initialise();
        }

        private ServerEndpoint _server = null;
        private Type _serverContextType = null;
        private int _maxNumClients = Int32.MaxValue;
        private Net.Sockets.MultiEndpointModel[] _multiEndpointModels = null;

        /// <summary>
        /// Gets the socket server endpoints.
        /// </summary>
        public ServerEndpoint Servers
        {
            get { return _server; }
        }

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

        /// <summary>
        /// Initialise.
        /// </summary>
        private void Initialise()
        {
            // Create the server.
            _server = new ServerEndpoint(_serverContextType, _multiEndpointModels, _maxNumClients);

            // For each server.
            for (int i = 0; i < _multiEndpointModels.Length; i++)
            {
                // Assign the on connect action handler.
                _server[i].Timeout = 60;
                _server[i].ReadBufferSize = 32768;
                _server[i].WriteBufferSize = 32768;
                _server[i].ResponseBufferCapacity = 10000000;
                _server[i].RequestBufferCapacity = 10000000;
                _server[i].Name = "Custom Net Server";
                _server[i].ServiceName = "CustomNetServer";
            }
        }

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
        ~CustomServer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
