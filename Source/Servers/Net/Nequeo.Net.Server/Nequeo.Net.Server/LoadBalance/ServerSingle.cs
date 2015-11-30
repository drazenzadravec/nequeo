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

namespace Nequeo.Net.LoadBalance
{
    /// <summary>
    /// Load balance server single.
    /// </summary>
    public class ServerSingle : IDisposable
    {
        /// <summary>
        /// Load balance server single.
        /// </summary>
        /// <param name="maxClient">The maximun number of connections.</param>
        public ServerSingle(int maxClient = 50000)
        {
            _maxClient = maxClient;

            // Initialise the server.
            Init();
        }

        private Nequeo.Net.WebServerSingle _server = null;
        private Nequeo.Server.SingleContextManager _contextManager = null;
        private SortedDictionary<string, Nequeo.Net.Data.ConnectionContext> _clients = null;
        private Nequeo.Net.Data.context _loadServers = null;
        private int _maxClient = 50000;

        private object _lockObject = new object();

        /// <summary>
        /// Gets the socket server.
        /// </summary>
        public Nequeo.Net.WebServerSingle Server
        {
            get { return _server; }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            try
            {
                // Start the server.
                if (_server != null)
                    _server.Start();
            }
            catch (Exception)
            {
                if (_server != null)
                    _server.Dispose();

                _server = null;
                _clients = null;
                throw;
            }
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Stop the server.
                if (_server != null)
                    _server.Stop();
            }
            catch { }
            finally
            {
                if (_server != null)
                    _server.Dispose();

                if (_clients != null)
                    CloseClients();

                _server = null;
                _clients = null;
            }
        }

        /// <summary>
        /// Initialise the server.
        /// </summary>
        private void Init()
        {
            try
            {
                // Create the client list.
                _clients = new SortedDictionary<string, Nequeo.Net.Data.ConnectionContext>();
                _loadServers = Data.Helper.GetLoadBalanceServer();
                _contextManager = new Nequeo.Server.SingleContextManager();

                string socketProviderHostPrefix = "LoadBalance_";
                string hostProviderFullName = socketProviderHostPrefix + "SocketProviderV6";
                string hostProviderFullNameSecure = socketProviderHostPrefix + "SocketProviderV6Ssl";

                // Get the certificate reader.
                Nequeo.Security.Configuration.Reader certificateReader = new Nequeo.Security.Configuration.Reader();
                Nequeo.Net.Configuration.Reader hostReader = new Nequeo.Net.Configuration.Reader();

                // Create the server endpoint.
                Nequeo.Net.Sockets.MultiEndpointModel[] model = new Nequeo.Net.Sockets.MultiEndpointModel[]
                {
                    // None secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = hostReader.GetServerHost(hostProviderFullName).Port,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    },
                    // Secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = hostReader.GetServerHost(hostProviderFullNameSecure).Port,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    }
                };

                // Start the server.
                _server = new Nequeo.Net.WebServerSingle(model, _maxClient);
                _server.ServerContextManager = _contextManager;
                _server.Name = "Load Balance Server";
                _server.ServiceName = "LoadBalanceServer";
                _server.SocketProviderHostPrefix = socketProviderHostPrefix;
                _server.OnClientConnected = (context) => ClientConnected(context);
                _server.OnClientDisconnected = (context) => ClientDisconnected(context);
                _server.OnWebContext += Server_OnWebContext;
                _server.Timeout = hostReader.GetServerHost(hostProviderFullName).ClientTimeOut;
                _server.ReadBufferSize = 32768;
                _server.WriteBufferSize = 32768;
                _server.HeaderTimeout = 30000;
                _server.RequestTimeout = 30000;
                _server.ResponseTimeout = 30000;
                _server.ResponseBufferCapacity = 10000000;
                _server.RequestBufferCapacity = 10000000;
                _server.MaximumReadLength = 1000000;

                // Inititalise.
                _server.Initialisation();

                try
                {
                    // Look for the certificate information in the configuration file.

                    // Get the certificate if any.
                    X509Certificate2 serverCertificate = certificateReader.GetServerCredentials();

                    // If a certificate exists.
                    if (serverCertificate != null)
                    {
                        // Get the secure servers.
                        _server.Server[2].UseSslConnection = true;
                        _server.Server[2].X509Certificate = serverCertificate;
                        _server.Server[3].UseSslConnection = true;
                        _server.Server[3].X509Certificate = serverCertificate;
                    }
                }
                catch { }

                // For each server in the collection.
                for (int i = 0; i < _server.Server.NumberOfServers; i++)
                {
                    // Set what needs to be polled.
                    _server.Server[i].PollReader(true);
                    _server.Server[i].PollWriter(false);
                    _server.Server[i].PollError(false);
                }
            }
            catch (Exception)
            {
                if (_server != null)
                    _server.Dispose();

                _server = null;
                _clients = null;
                throw;
            }
        }

        /// <summary>
        /// On client connected.
        /// </summary>
        /// <param name="context">The client context.</param>
        private void ClientConnected(Nequeo.Net.Provider.ISingleContextMux context)
        {
            // Make the unique id the same as the connection ID.
            context.UniqueIdentifier = context.ConnectionID;

            // Add the client.
            ClientConnectedEx(context);
        }

        /// <summary>
        /// On client diconnected.
        /// </summary>
        /// <param name="context">The client context.</param>
        private void ClientDisconnected(Nequeo.Net.Provider.ISingleContextBase context)
        {
            // Remove the client.
            ClientDisconnectedEx(context);
        }

        /// <summary>
        /// On client connected.
        /// </summary>
        /// <param name="context">The client context.</param>
        private async void ClientConnectedEx(Nequeo.Net.Provider.ISingleContextMux context)
        {
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    bool sslNegComplete = true;

                    // If this is a secure connection.
                    if (context.UseSslConnection && !context.IsSslAuthenticated)
                    {
                        try
                        {
                            // Start the ssl negotiations.
                            context.BeginSslNegotiation();
                        }
                        catch 
                        {
                            // Negotiation has failed.
                            sslNegComplete = false;
                        }
                    }

                    // If the ssl negotiation has completed or non ssl connection
                    // add the client to the connection base.
                    if (sslNegComplete)
                    {
                        lock (_lockObject)
                        {
                            try
                            {
                                // Create a new connection context.
                                Nequeo.Net.Data.ConnectionContext conn = new Data.ConnectionContext();

                                // Get the name of the server this client will connect to.
                                // Get the server that has the least amount of connections.
                                int minimum = _loadServers.servers.Min(u => u.count);
                                Data.contextServer balanceServer = _loadServers.servers.First(u => u.count <= minimum);
                                balanceServer.count += 1;

                                // Create a new connection to a server.
                                Nequeo.Net.NetClient client = new NetClient(balanceServer.host, balanceServer.port);
                                client.OnNetContext += client_OnNetContext;
                                client.UseSslConnection = balanceServer.secure;
                                client.Connect();

                                // Make the client connection ID the same as the
                                // client context connection ID.
                                client.ConnectionID = context.ConnectionID;

                                // Assign the values.
                                conn.Client = client;
                                conn.LoadBalanceServer = balanceServer.name;

                                // Add the client.
                                _clients.Add(context.ConnectionID, conn);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        // Close the connection.
                        context.Close();
                    }
                });
        }

        /// <summary>
        /// On client diconnected.
        /// </summary>
        /// <param name="context">The client context.</param>
        private async void ClientDisconnectedEx(Nequeo.Net.Provider.ISingleContextBase context)
        {
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    lock (_lockObject)
                    {
                        try
                        {
                            // Get the client.
                            Nequeo.Net.Data.ConnectionContext conn = _clients[context.ConnectionID];

                            // Close the connection.
                            Nequeo.Net.NetClient client = conn.Client;
                            client.Dispose();

                            // Remove the client.
                            _clients.Remove(context.ConnectionID);

                            // Decrement the server count. Find the current load balance server
                            // this client is connected to.
                            IEnumerable<Data.contextServer> balanceServers = 
                                _loadServers.servers.Where(u => u.name.ToLower() == conn.LoadBalanceServer.ToLower());

                            // If the server has been found.
                            if (balanceServers.Count() > 0)
                            {
                                // Get the first server.
                                Data.contextServer balanceServer = balanceServers.First();

                                // Get the current client count.
                                int currentCount = balanceServer.count;

                                // If the current count is not zero
                                // then decrement to count else do
                                // not go less than zero.
                                if (currentCount > 0)
                                    balanceServer.count = currentCount - 1;
                            }
                        }
                        catch { }
                    }
                });
        }

        /// <summary>
        /// Close all the client connections.
        /// </summary>
        private void CloseClients()
        {
            lock (_lockObject)
            {
                // For each client close the connection.
                foreach (KeyValuePair<string, Nequeo.Net.Data.ConnectionContext> connection in _clients)
                {
                    try
                    {
                        // Dispose of the client.
                        connection.Value.Client.Dispose();
                    }
                    catch { }
                }

                // Clear the collection.
                _clients.Clear();
            }
        }

        /// <summary>
        /// On web context.
        /// </summary>
        /// <param name="sender">The server that sent the context.</param>
        /// <param name="context">The current connection context.</param>
        private void Server_OnWebContext(object sender, WebContext context)
        {
            Nequeo.Net.Data.ConnectionContext conn = null;

            lock (_lockObject)
            {
                // As data comes from the client send it to the load balance server
                // this client is connected to.
                conn = _clients[context.ConnectionID];
            }

            // If the client has been found.
            if (conn != null)
            {
                Nequeo.Net.NetClient client = conn.Client;

                // Read all the bytes in the buffer.
                int count = (int)context.WebRequest.Input.Length;
                byte[] buffer = new byte[count];
                context.WebRequest.Read(buffer, 0, count);

                if (buffer != null)
                    // Write all the bytes to the balance server through the client.
                    client.Send(buffer);

                buffer = null;
            }
        }

        /// <summary>
        /// On net context.
        /// </summary>
        /// <param name="sender">The client that sent the context.</param>
        /// <param name="context">The current connection context.</param>
        private void client_OnNetContext(object sender, NetContext context)
        {
            // Read all the bytes in the buffer.
            int count = (int)context.NetResponse.Input.Length;
            byte[] buffer = new byte[count];
            context.NetResponse.Read(buffer, 0, count);

            if (buffer != null)
                // Write the data back to the client.
                _server.SendToClient(buffer, context.ConnectionID);

            buffer = null;
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

                    if (_contextManager != null)
                        _contextManager.Dispose();

                    if (_clients != null)
                        CloseClients();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _server = null;
                _contextManager = null;
                _loadServers = null;
                _clients = null;
                _lockObject = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~ServerSingle()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
