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
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Security;
using Nequeo.Threading;

namespace Nequeo.Net.Sockets
{
    /// <summary>
    /// General multi-endpoint socket server (multi-client), creates a new callback thread for each 
    /// client until the client is disconnected.
    /// </summary>
    public partial class MultiEndpointServer : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Multi-endpoint socket server constructor.
        /// </summary>
        /// <param name="serverContextType">The server context type, must inherit Nequeo.Net.Sockets.ServerContext.</param>
        /// <param name="addresses">The collection of IP address endpoints.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public MultiEndpointServer(Type serverContextType, IPAddress[] addresses, int port, int maxNumClients = Int32.MaxValue)
        {
            if (addresses == null) throw new ArgumentNullException("addresses");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");

            bool isBaseType = false;
            Type baseType = serverContextType.BaseType;

            // Do until no more base type.
            while (baseType != null)
            {
                // If the correct base type.
                if (baseType == typeof(Nequeo.Net.Sockets.ServerContext))
                {
                    isBaseType = true;
                    break;
                }
                else
                {
                    // Get the next base type.
                    baseType = baseType.BaseType;
                }
            }

            // If not base type.
            if (!isBaseType)
                throw new Exception("The server context type is not of type Nequeo.Net.Sockets.ServerContext");

            // Create the collection of servers.
            _servers = new Net.Sockets.Server[addresses.Length];

            // For each server.
            for (int i = 0; i < addresses.Length; i++)
            {
                // Assign each server the endpoint details.
                _servers[i] = new Net.Sockets.Server(serverContextType, addresses[i], port);
                _servers[i].MaxNumClients = maxNumClients;
                _servers[i].Name = port.ToString() + addresses[i].AddressFamily.ToString();
            }

            // Add the servers to the multi-endpoint collection
            // All servers are linked by maximum number of clients.
            _multiEndpoint = new Net.Sockets.MultiServerController(_servers, maxNumClients);
        }

        /// <summary>
        /// Multi-endpoint socket server constructor.
        /// </summary>
        /// <param name="serverContextType">The server context type, must inherit Nequeo.Net.Sockets.ServerContext.</param>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public MultiEndpointServer(Type serverContextType, MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");

            bool isBaseType = false;
            Type baseType = serverContextType.BaseType;

            // Do until no more base type.
            while (baseType != null)
            {
                // If the correct base type.
                if (baseType == typeof(Nequeo.Net.Sockets.ServerContext))
                {
                    isBaseType = true;
                    break;
                }
                else
                {
                    // Get the next base type.
                    baseType = baseType.BaseType;
                }
            }

            // If not base type.
            if (!isBaseType)
                throw new Exception("The server context type is not of type Nequeo.Net.Sockets.ServerContext");

            // Total number of servers
            int totalServerCount = 0;
            int serverIndex = 0;

            // Get the total number of servers.
            foreach (MultiEndpointModel item in multiEndpointModels)
                totalServerCount += item.Addresses.Length;

            // Create the collection of servers.
            _servers = new Net.Sockets.Server[totalServerCount];

            // For each endpoint model.
            for (int j = 0; j < multiEndpointModels.Length; j++)
            {
                // Get the current endpoint.
                IPAddress[] addresses = multiEndpointModels[j].Addresses;
                int port = multiEndpointModels[j].Port;

                // For each server.
                for (int i = 0; i < addresses.Length; i++)
                {
                    // Assign each server the endpoint details.
                    _servers[serverIndex] = new Net.Sockets.Server(serverContextType, addresses[i], port);
                    _servers[serverIndex].MaxNumClients = maxNumClients;
                    _servers[serverIndex].Name = port.ToString() + addresses[i].AddressFamily.ToString();

                    // Increment the server index.
                    serverIndex++;
                }
            }

            // Add the servers to the multi-endpoint collection
            // All servers are linked by maximum number of clients.
            _multiEndpoint = new Net.Sockets.MultiServerController(_servers, maxNumClients);
        }

        /// <summary>
        /// Multi-endpoint socket server constructor.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint server context model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public MultiEndpointServer(MultiEndpointServerContextModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");

            // Total number of servers
            int totalServerCount = 0;
            int serverIndex = 0;

            // Get the total number of servers.
            foreach (MultiEndpointServerContextModel item in multiEndpointModels)
            {
                bool isBaseType = false;
                Type baseType = item.ServerContextType.BaseType;

                // Do until no more base type.
                while (baseType != null)
                {
                    // If the correct base type.
                    if (baseType == typeof(Nequeo.Net.Sockets.ServerContext))
                    {
                        isBaseType = true;
                        break;
                    }
                    else
                    {
                        // Get the next base type.
                        baseType = baseType.BaseType;
                    }
                }

                // If not base type.
                if (!isBaseType)
                    throw new Exception("The server context type is not of type Nequeo.Net.Sockets.ServerContext");

                totalServerCount += item.Addresses.Length;
            }

            // Create the collection of servers.
            _servers = new Net.Sockets.Server[totalServerCount];

            // For each endpoint model.
            for (int j = 0; j < multiEndpointModels.Length; j++)
            {
                // Get the current endpoint.
                IPAddress[] addresses = multiEndpointModels[j].Addresses;
                int port = multiEndpointModels[j].Port;
                Type serverContextType = multiEndpointModels[j].ServerContextType;

                // For each server.
                for (int i = 0; i < addresses.Length; i++)
                {
                    // Assign each server the endpoint details.
                    _servers[serverIndex] = new Net.Sockets.Server(serverContextType, addresses[i], port);
                    _servers[serverIndex].MaxNumClients = maxNumClients;
                    _servers[serverIndex].Name = port.ToString() + addresses[i].AddressFamily.ToString();

                    // Increment the server index.
                    serverIndex++;
                }
            }

            // Add the servers to the multi-endpoint collection
            // All servers are linked by maximum number of clients.
            _multiEndpoint = new Net.Sockets.MultiServerController(_servers, maxNumClients);
        }
        #endregion

        #region Private Fields
        private Net.Sockets.Server[] _servers = null;
        private Net.Sockets.MultiServerController _multiEndpoint = null;

        private bool _isListening = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the is listening server indicator.
        /// </summary>
        public bool IsListening
        {
            get { return _isListening; }
        }

        /// <summary>
        /// Gets the total number of servers.
        /// </summary>
        public int NumberOfServers
        {
            get { return _servers.Length; }
        }

        /// <summary>
        /// Default accessor for the Net.Sockets.Server collection.
        /// </summary>
        /// <param name="index">The current index to access</param>
        /// <returns>The current Net.Sockets.Server in the collection.</returns>
        public Net.Sockets.Server this[int index]
        {
            get { return _servers[index]; }
        }

        /// <summary>
        /// Server name accessor for the Net.Sockets.Server collection.
        /// </summary>
        /// <param name="name">The current server name to access.</param>
        /// <returns>The current Net.Sockets.Server in the collection.</returns>
        /// <remarks>The default name of each server is the Port number + System.Net.Sockets.AddressFamily (80InterNetworkV6)</remarks>
        public Net.Sockets.Server this[string name]
        {
            get { return _servers.First(u => u.Name.ToLower() == name.ToLower()); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start listening on a new thread.
        /// </summary>
        public virtual void Start()
        {
            // If not listening then attempt to start the servers.
            if (!_isListening)
            {
                foreach (Net.Sockets.Server item in _servers)
                {
                    item.StartListeningThread();
                }

                // Listening on all servers.
                _isListening = true;
            }
        }

        /// <summary>
        /// Stop listening on the thread.
        /// </summary>
        public virtual void Stop()
        {
            foreach (Net.Sockets.Server item in _servers)
            {
                item.StopListeningThread();
            }

            // Stopped listening on all servers.
            _isListening = false;
        }

        /// <summary>
        /// Get the last error collection that occured.
        /// </summary>
        /// <returns>The exception containing the error; else null.</returns>
        public IEnumerable<Exception> GetLastError()
        {
            foreach (Net.Sockets.Server item in _servers)
            {
                yield return item.GetLastError();
            }
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
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_servers != null)
                    {
                        foreach (Net.Sockets.Server item in _servers)
                            item.Dispose();
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _servers = null;
                _multiEndpoint = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~MultiEndpointServer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// General multi-endpoint socket server (multi-client), creates a new thread for each 
    /// client until the client is disconnected.
    /// </summary>
    public partial class MultiEndpointServerDynamic : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Multi-endpoint socket server constructor.
        /// </summary>
        /// <param name="addresses">The collection of IP address endpoints.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public MultiEndpointServerDynamic(IPAddress[] addresses, int port, int maxNumClients = Int32.MaxValue)
        {
            if (addresses == null) throw new ArgumentNullException("addresses");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");

            // Create the collection of servers.
            _servers = new Net.Sockets.ServerDynamic[addresses.Length];

            // For each server.
            for (int i = 0; i < addresses.Length; i++)
            {
                // Assign each server the endpoint details.
                _servers[i] = new Net.Sockets.ServerDynamic(addresses[i], port);
                _servers[i].MaxNumClients = maxNumClients;
                _servers[i].Name = port.ToString() + addresses[i].AddressFamily.ToString();
            }

            // Add the servers to the multi-endpoint collection
            // All servers are linked by maximum number of clients.
            _multiEndpoint = new Net.Sockets.MultiServerController(_servers, maxNumClients);
        }

        /// <summary>
        /// Multi-endpoint socket server constructor.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public MultiEndpointServerDynamic(MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");

            // Total number of servers
            int totalServerCount = 0;
            int serverIndex = 0;

            // Get the total number of servers.
            foreach (MultiEndpointModel item in multiEndpointModels)
                totalServerCount += item.Addresses.Length;

            // Create the collection of servers.
            _servers = new Net.Sockets.ServerDynamic[totalServerCount];

            // For each endpoint model.
            for (int j = 0; j < multiEndpointModels.Length; j++)
            {
                // Get the current endpoint.
                IPAddress[] addresses = multiEndpointModels[j].Addresses;
                int port = multiEndpointModels[j].Port;

                // For each server.
                for (int i = 0; i < addresses.Length; i++)
                {
                    // Assign each server the endpoint details.
                    _servers[serverIndex] = new Net.Sockets.ServerDynamic(addresses[i], port);
                    _servers[serverIndex].MaxNumClients = maxNumClients;
                    _servers[serverIndex].Name = port.ToString() + addresses[i].AddressFamily.ToString();

                    // Increment the server index.
                    serverIndex++;
                }
            }

            // Add the servers to the multi-endpoint collection
            // All servers are linked by maximum number of clients.
            _multiEndpoint = new Net.Sockets.MultiServerController(_servers, maxNumClients);
        }
        #endregion

        #region Private Fields
        private Net.Sockets.ServerDynamic[] _servers = null;
        private Net.Sockets.MultiServerController _multiEndpoint = null;

        private bool _isListening = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the is listening server indicator.
        /// </summary>
        public bool IsListening
        {
            get { return _isListening; }
        }

        /// <summary>
        /// Gets, the total number of servers.
        /// </summary>
        public int NumberOfServers
        {
            get { return _servers.Length; }
        }

        /// <summary>
        /// Default accessor for the Net.Sockets.Server collection.
        /// </summary>
        /// <param name="index">The current index to access</param>
        /// <returns>The current Net.Sockets.Server in the collection.</returns>
        public Net.Sockets.ServerDynamic this[int index]
        {
            get { return _servers[index]; }
        }

        /// <summary>
        /// Server name accessor for the Net.Sockets.Server collection.
        /// </summary>
        /// <param name="name">The current server name to access.</param>
        /// <returns>The current Net.Sockets.Server in the collection.</returns>
        /// <remarks>The default name of each server is the Port number + System.Net.Sockets.AddressFamily (80InterNetworkV6)</remarks>
        public Net.Sockets.ServerDynamic this[string name]
        {
            get { return _servers.First(u => u.Name.ToLower() == name.ToLower()); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start listening on a new thread.
        /// </summary>
        public virtual void Start()
        {
            // If not listening then attempt to start the servers.
            if (!_isListening)
            {
                foreach (Net.Sockets.ServerDynamic item in _servers)
                {
                    item.StartListeningThread();
                }

                // Listening on all servers.
                _isListening = true;
            }
        }

        /// <summary>
        /// Stop listening on the thread.
        /// </summary>
        public virtual void Stop()
        {
            foreach (Net.Sockets.ServerDynamic item in _servers)
            {
                item.StopListeningThread();
            }

            // Stopped listening on all servers.
            _isListening = false;
        }

        /// <summary>
        /// Get the last error collection that occured.
        /// </summary>
        /// <returns>The exception containing the error; else null.</returns>
        public IEnumerable<Exception> GetLastError()
        {
            foreach (Net.Sockets.ServerDynamic item in _servers)
            {
                yield return item.GetLastError();
            }
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
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_servers != null)
                    {
                        foreach (Net.Sockets.ServerDynamic item in _servers)
                            item.Dispose();
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _servers = null;
                _multiEndpoint = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~MultiEndpointServerDynamic()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// General multi-endpoint socket server (multi-client), single thread for all client.
    /// until the client is disconnected.
    /// </summary>
    public partial class MultiEndpointServerSingle : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Multi-endpoint socket server constructor.
        /// </summary>
        /// <param name="addresses">The collection of IP address endpoints.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public MultiEndpointServerSingle(IPAddress[] addresses, int port, int maxNumClients = Int32.MaxValue)
        {
            if (addresses == null) throw new ArgumentNullException("addresses");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");

            // Create the collection of servers.
            _servers = new Net.Sockets.ServerSingle[addresses.Length];

            // For each server.
            for (int i = 0; i < addresses.Length; i++)
            {
                // Assign each server the endpoint details.
                _servers[i] = new Net.Sockets.ServerSingle(addresses[i], port);
                _servers[i].MaxNumClients = maxNumClients;
                _servers[i].Name = port.ToString() + addresses[i].AddressFamily.ToString();
            }

            // Add the servers to the multi-endpoint collection
            // All servers are linked by maximum number of clients.
            _multiEndpoint = new Net.Sockets.MultiServerController(_servers, maxNumClients);
        }

        /// <summary>
        /// Multi-endpoint socket server constructor.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public MultiEndpointServerSingle(MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");

            // Total number of servers
            int totalServerCount = 0;
            int serverIndex = 0;

            // Get the total number of servers.
            foreach (MultiEndpointModel item in multiEndpointModels)
                totalServerCount += item.Addresses.Length;

            // Create the collection of servers.
            _servers = new Net.Sockets.ServerSingle[totalServerCount];

            // For each endpoint model.
            for (int j = 0; j < multiEndpointModels.Length; j++)
            {
                // Get the current endpoint.
                IPAddress[] addresses = multiEndpointModels[j].Addresses;
                int port = multiEndpointModels[j].Port;

                // For each server.
                for (int i = 0; i < addresses.Length; i++)
                {
                    // Assign each server the endpoint details.
                    _servers[serverIndex] = new Net.Sockets.ServerSingle(addresses[i], port);
                    _servers[serverIndex].MaxNumClients = maxNumClients;
                    _servers[serverIndex].Name = port.ToString() + addresses[i].AddressFamily.ToString();

                    // Increment the server index.
                    serverIndex++;
                }
            }

            // Add the servers to the multi-endpoint collection
            // All servers are linked by maximum number of clients.
            _multiEndpoint = new Net.Sockets.MultiServerController(_servers, maxNumClients);
        }
        #endregion

        #region Private Fields
        private Net.Sockets.ServerSingle[] _servers = null;
        private Net.Sockets.MultiServerController _multiEndpoint = null;

        private bool _isListening = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the is listening server indicator.
        /// </summary>
        public bool IsListening
        {
            get { return _isListening; }
        }

        /// <summary>
        /// Gets, the total number of servers.
        /// </summary>
        public int NumberOfServers
        {
            get { return _servers.Length; }
        }

        /// <summary>
        /// Default accessor for the Net.Sockets.Server collection.
        /// </summary>
        /// <param name="index">The current index to access</param>
        /// <returns>The current Net.Sockets.Server in the collection.</returns>
        public Net.Sockets.ServerSingle this[int index]
        {
            get { return _servers[index]; }
        }

        /// <summary>
        /// Server name accessor for the Net.Sockets.Server collection.
        /// </summary>
        /// <param name="name">The current server name to access.</param>
        /// <returns>The current Net.Sockets.Server in the collection.</returns>
        /// <remarks>The default name of each server is the Port number + System.Net.Sockets.AddressFamily (80InterNetworkV6)</remarks>
        public Net.Sockets.ServerSingle this[string name]
        {
            get { return _servers.First(u => u.Name.ToLower() == name.ToLower()); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start listening on a new thread.
        /// </summary>
        public virtual void Start()
        {
            // If not listening then attempt to start the servers.
            if (!_isListening)
            {
                foreach (Net.Sockets.ServerSingle item in _servers)
                {
                    item.StartListeningThread();
                }

                // Listening on all servers.
                _isListening = true;
            }
        }

        /// <summary>
        /// Stop listening on the thread.
        /// </summary>
        public virtual void Stop()
        {
            foreach (Net.Sockets.ServerSingle item in _servers)
            {
                item.StopListeningThread();
            }

            // Stopped listening on all servers.
            _isListening = false;
        }

        /// <summary>
        /// Get the last error collection that occured.
        /// </summary>
        /// <returns>The exception containing the error; else null.</returns>
        public IEnumerable<Exception> GetLastError()
        {
            foreach (Net.Sockets.ServerSingle item in _servers)
            {
                yield return item.GetLastError();
            }
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
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_servers != null)
                    {
                        foreach (Net.Sockets.ServerSingle item in _servers)
                            item.Dispose();
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _servers = null;
                _multiEndpoint = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~MultiEndpointServerSingle()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// General multi-endpoint socket server (multi-client).
    /// </summary>
    public partial class MultiEndpointUdpServer : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Multi-endpoint socket server constructor.
        /// </summary>
        /// <param name="addresses">The collection of IP address endpoints.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public MultiEndpointUdpServer(IPAddress[] addresses, int port, int maxNumClients = Int32.MaxValue)
        {
            if (addresses == null) throw new ArgumentNullException("addresses");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");

            // Create the collection of servers.
            _servers = new Net.Sockets.UdpServer[addresses.Length];

            // For each server.
            for (int i = 0; i < addresses.Length; i++)
            {
                // Assign each server the endpoint details.
                _servers[i] = new Net.Sockets.UdpServer(addresses[i], port);
                _servers[i].MaxNumClients = maxNumClients;
                _servers[i].Name = port.ToString() + addresses[i].AddressFamily.ToString();
            }

            // Add the servers to the multi-endpoint collection
            // All servers are linked by maximum number of clients.
            _multiEndpoint = new Net.Sockets.MultiServerController(_servers, maxNumClients);
        }

        /// <summary>
        /// Multi-endpoint socket server constructor.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public MultiEndpointUdpServer(MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");

            // Total number of servers
            int totalServerCount = 0;
            int serverIndex = 0;

            // Get the total number of servers.
            foreach (MultiEndpointModel item in multiEndpointModels)
                totalServerCount += item.Addresses.Length;

            // Create the collection of servers.
            _servers = new Net.Sockets.UdpServer[totalServerCount];

            // For each endpoint model.
            for (int j = 0; j < multiEndpointModels.Length; j++)
            {
                // Get the current endpoint.
                IPAddress[] addresses = multiEndpointModels[j].Addresses;
                int port = multiEndpointModels[j].Port;

                // For each server.
                for (int i = 0; i < addresses.Length; i++)
                {
                    // Assign each server the endpoint details.
                    _servers[serverIndex] = new Net.Sockets.UdpServer(addresses[i], port);
                    _servers[serverIndex].MaxNumClients = maxNumClients;
                    _servers[serverIndex].Name = port.ToString() + addresses[i].AddressFamily.ToString();

                    // Increment the server index.
                    serverIndex++;
                }
            }

            // Add the servers to the multi-endpoint collection
            // All servers are linked by maximum number of clients.
            _multiEndpoint = new Net.Sockets.MultiServerController(_servers, maxNumClients);
        }
        #endregion

        #region Private Fields
        private Net.Sockets.UdpServer[] _servers = null;
        private Net.Sockets.MultiServerController _multiEndpoint = null;

        private bool _isListening = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the is listening server indicator.
        /// </summary>
        public bool IsListening
        {
            get { return _isListening; }
        }

        /// <summary>
        /// Gets the total number of servers.
        /// </summary>
        public int NumberOfServers
        {
            get { return _servers.Length; }
        }

        /// <summary>
        /// Default accessor for the Net.Sockets.Server collection.
        /// </summary>
        /// <param name="index">The current index to access</param>
        /// <returns>The current Net.Sockets.Server in the collection.</returns>
        public Net.Sockets.UdpServer this[int index]
        {
            get { return _servers[index]; }
        }

        /// <summary>
        /// Server name accessor for the Net.Sockets.Server collection.
        /// </summary>
        /// <param name="name">The current server name to access.</param>
        /// <returns>The current Net.Sockets.Server in the collection.</returns>
        /// <remarks>The default name of each server is the Port number + System.Net.Sockets.AddressFamily (80InterNetworkV6)</remarks>
        public Net.Sockets.UdpServer this[string name]
        {
            get { return _servers.First(u => u.Name.ToLower() == name.ToLower()); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start listening on a new thread.
        /// </summary>
        public virtual void Start()
        {
            // If not listening then attempt to start the servers.
            if (!_isListening)
            {
                foreach (Net.Sockets.UdpServer item in _servers)
                {
                    item.StartListeningThread();
                }

                // Listening on all servers.
                _isListening = true;
            }
        }

        /// <summary>
        /// Stop listening on the thread.
        /// </summary>
        public virtual void Stop()
        {
            foreach (Net.Sockets.UdpServer item in _servers)
            {
                item.StopListeningThread();
            }

            // Stopped listening on all servers.
            _isListening = false;
        }

        /// <summary>
        /// Get the last error collection that occured.
        /// </summary>
        /// <returns>The exception containing the error; else null.</returns>
        public IEnumerable<Exception> GetLastError()
        {
            foreach (Net.Sockets.UdpServer item in _servers)
            {
                yield return item.GetLastError();
            }
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
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_servers != null)
                    {
                        foreach (Net.Sockets.UdpServer item in _servers)
                            item.Dispose();
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _servers = null;
                _multiEndpoint = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~MultiEndpointUdpServer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// General multi-endpoint socket server (multi-client).
    /// </summary>
    public partial class MultiEndpointUdpSingleServer : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Multi-endpoint socket server constructor.
        /// </summary>
        /// <param name="addresses">The collection of IP address endpoints.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public MultiEndpointUdpSingleServer(IPAddress[] addresses, int port, int maxNumClients = Int32.MaxValue)
        {
            if (addresses == null) throw new ArgumentNullException("addresses");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");

            // Create the collection of servers.
            _servers = new Net.Sockets.UdpSingleServer[addresses.Length];

            // For each server.
            for (int i = 0; i < addresses.Length; i++)
            {
                // Assign each server the endpoint details.
                _servers[i] = new Net.Sockets.UdpSingleServer(addresses[i], port);
                _servers[i].MaxNumClients = maxNumClients;
                _servers[i].Name = port.ToString() + addresses[i].AddressFamily.ToString();
            }

            // Add the servers to the multi-endpoint collection
            // All servers are linked by maximum number of clients.
            _multiEndpoint = new Net.Sockets.MultiServerController(_servers, maxNumClients);
        }

        /// <summary>
        /// Multi-endpoint socket server constructor.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public MultiEndpointUdpSingleServer(MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");

            // Total number of servers
            int totalServerCount = 0;
            int serverIndex = 0;

            // Get the total number of servers.
            foreach (MultiEndpointModel item in multiEndpointModels)
                totalServerCount += item.Addresses.Length;

            // Create the collection of servers.
            _servers = new Net.Sockets.UdpSingleServer[totalServerCount];

            // For each endpoint model.
            for (int j = 0; j < multiEndpointModels.Length; j++)
            {
                // Get the current endpoint.
                IPAddress[] addresses = multiEndpointModels[j].Addresses;
                int port = multiEndpointModels[j].Port;

                // For each server.
                for (int i = 0; i < addresses.Length; i++)
                {
                    // Assign each server the endpoint details.
                    _servers[serverIndex] = new Net.Sockets.UdpSingleServer(addresses[i], port);
                    _servers[serverIndex].MaxNumClients = maxNumClients;
                    _servers[serverIndex].Name = port.ToString() + addresses[i].AddressFamily.ToString();

                    // Increment the server index.
                    serverIndex++;
                }
            }

            // Add the servers to the multi-endpoint collection
            // All servers are linked by maximum number of clients.
            _multiEndpoint = new Net.Sockets.MultiServerController(_servers, maxNumClients);
        }
        #endregion

        #region Private Fields
        private Net.Sockets.UdpSingleServer[] _servers = null;
        private Net.Sockets.MultiServerController _multiEndpoint = null;

        private bool _isListening = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the is listening server indicator.
        /// </summary>
        public bool IsListening
        {
            get { return _isListening; }
        }

        /// <summary>
        /// Gets the total number of servers.
        /// </summary>
        public int NumberOfServers
        {
            get { return _servers.Length; }
        }

        /// <summary>
        /// Default accessor for the Net.Sockets.Server collection.
        /// </summary>
        /// <param name="index">The current index to access</param>
        /// <returns>The current Net.Sockets.Server in the collection.</returns>
        public Net.Sockets.UdpSingleServer this[int index]
        {
            get { return _servers[index]; }
        }

        /// <summary>
        /// Server name accessor for the Net.Sockets.Server collection.
        /// </summary>
        /// <param name="name">The current server name to access.</param>
        /// <returns>The current Net.Sockets.Server in the collection.</returns>
        /// <remarks>The default name of each server is the Port number + System.Net.Sockets.AddressFamily (80InterNetworkV6)</remarks>
        public Net.Sockets.UdpSingleServer this[string name]
        {
            get { return _servers.First(u => u.Name.ToLower() == name.ToLower()); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start listening on a new thread.
        /// </summary>
        public virtual void Start()
        {
            // If not listening then attempt to start the servers.
            if (!_isListening)
            {
                foreach (Net.Sockets.UdpSingleServer item in _servers)
                {
                    item.StartListeningThread();
                }

                // Listening on all servers.
                _isListening = true;
            }
        }

        /// <summary>
        /// Stop listening on the thread.
        /// </summary>
        public virtual void Stop()
        {
            foreach (Net.Sockets.UdpSingleServer item in _servers)
            {
                item.StopListeningThread();
            }

            // Stopped listening on all servers.
            _isListening = false;
        }

        /// <summary>
        /// Get the last error collection that occured.
        /// </summary>
        /// <returns>The exception containing the error; else null.</returns>
        public IEnumerable<Exception> GetLastError()
        {
            foreach (Net.Sockets.UdpSingleServer item in _servers)
            {
                yield return item.GetLastError();
            }
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
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_servers != null)
                    {
                        foreach (Net.Sockets.UdpSingleServer item in _servers)
                            item.Dispose();
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _servers = null;
                _multiEndpoint = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~MultiEndpointUdpSingleServer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Combines all the servers into one collection, the primary use is to control
    /// the number of clients that can be connected to all the combined servers.
    /// The combined servers use a collective maximum number of clients connected 
    /// count not an individual count. If ten servers are listening then the ten
    /// servers collectively will have the maximumn number of clients connected.
    /// </summary>
    public sealed partial class MultiServerController
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="s1">The server collection.</param>
        /// <param name="maxNumClients">The collective maximum number of clients.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MultiServerController(int maxNumClients = Int32.MaxValue, params Server[] s1) :
            this(s1, null, null, null, null, maxNumClients)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="s2">The server collection.</param>
        /// <param name="maxNumClients">The collective maximum number of clients.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MultiServerController(int maxNumClients = Int32.MaxValue, params ServerDynamic[] s2) :
            this(null, s2, null, null, null, maxNumClients)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="s3">The server collection.</param>
        /// <param name="maxNumClients">The collective maximum number of clients.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MultiServerController(int maxNumClients = Int32.MaxValue, params ServerSingle[] s3) :
            this(null, null, s3, null, null, maxNumClients)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="s4">The server collection.</param>
        /// <param name="maxNumClients">The collective maximum number of clients.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MultiServerController(int maxNumClients = Int32.MaxValue, params UdpServer[] s4) :
            this(null, null, null, s4, null, maxNumClients)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="s5">The server collection.</param>
        /// <param name="maxNumClients">The collective maximum number of clients.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MultiServerController(int maxNumClients = Int32.MaxValue, params UdpSingleServer[] s5) :
            this(null, null, null, null, s5, maxNumClients)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="s1">The server collection.</param>
        /// <param name="maxNumClients">The collective maximum number of clients.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MultiServerController(Server[] s1, int maxNumClients = Int32.MaxValue) :
            this(s1, null, null, null, null, maxNumClients)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="s2">The server host collection.</param>
        /// <param name="maxNumClients">The collective maximum number of clients.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MultiServerController(ServerDynamic[] s2, int maxNumClients = Int32.MaxValue) :
            this(null, s2, null, null, null, maxNumClients)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="s3">The server host collection.</param>
        /// <param name="maxNumClients">The collective maximum number of clients.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MultiServerController(ServerSingle[] s3, int maxNumClients = Int32.MaxValue) :
            this(null, null, s3, null, null, maxNumClients)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="s4">The server host collection.</param>
        /// <param name="maxNumClients">The collective maximum number of clients.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MultiServerController(UdpServer[] s4, int maxNumClients = Int32.MaxValue) :
            this(null, null, null, s4, null, maxNumClients)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="s5">The server host collection.</param>
        /// <param name="maxNumClients">The collective maximum number of clients.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MultiServerController(UdpSingleServer[] s5, int maxNumClients = Int32.MaxValue) :
            this(null, null, null, null, s5, maxNumClients)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="s1">The server collection.</param>
        /// <param name="s2">The server host collection.</param>
        /// <param name="s3">The server host collection.</param>
        /// <param name="s4">The server host collection.</param>
        /// <param name="s5">The server host collection.</param>
        /// <param name="maxNumClients">The collective maximum number of clients.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MultiServerController(Server[] s1, ServerDynamic[] s2, ServerSingle[] s3, UdpServer[] s4, UdpSingleServer[] s5, int maxNumClients = Int32.MaxValue)
        {
            _s1 = s1;
            _s2 = s2;
            _s3 = s3;
            _s4 = s4;
            _s5 = s5;

            if (_s1 != null)
            {
                // For each server assign.
                foreach (Server item in s1)
                {
                    item.MaxNumClients = maxNumClients;
                    item.MaxNumClientsIndividualControl = false;
                    item.OnMultiEndpointServerClientCount +=
                        new Threading.EventHandler<IndexAction>(ServerMultiEndpoint_OnMultiEndpointServerClientCount);
                }
            }

            if (_s2 != null)
            {
                // For each server assign.
                foreach (ServerDynamic item in s2)
                {
                    item.MaxNumClients = maxNumClients;
                    item.MaxNumClientsIndividualControl = false;
                    item.OnMultiEndpointServerClientCount +=
                        new Threading.EventHandler<IndexAction>(ServerMultiEndpoint_OnMultiEndpointServerClientCount);
                }
            }

            if (_s3 != null)
            {
                // For each server assign.
                foreach (ServerSingle item in s3)
                {
                    item.MaxNumClients = maxNumClients;
                    item.MaxNumClientsIndividualControl = false;
                    item.OnMultiEndpointServerClientCount +=
                        new Threading.EventHandler<IndexAction>(ServerMultiEndpoint_OnMultiEndpointServerClientCount);
                }
            }

            if (s4 != null)
            {
                // For each server assign.
                foreach (UdpServer item in s4)
                {
                    item.MaxNumClients = maxNumClients;
                    item.MaxNumClientsIndividualControl = false;
                    item.OnMultiEndpointServerClientCount +=
                        new Threading.EventHandler<IndexAction>(ServerMultiEndpoint_OnMultiEndpointServerClientCount);
                }
            }

            if (s5 != null)
            {
                // For each server assign.
                foreach (UdpSingleServer item in s5)
                {
                    item.MaxNumClients = maxNumClients;
                    item.MaxNumClientsIndividualControl = false;
                    item.OnMultiEndpointServerClientCount +=
                        new Threading.EventHandler<IndexAction>(ServerMultiEndpoint_OnMultiEndpointServerClientCount);
                }
            }
        }
        #endregion

        #region Private Fields
        private Server[] _s1 = null;
        private ServerDynamic[] _s2 = null;
        private ServerSingle[] _s3 = null;
        private UdpServer[] _s4 = null;
        private UdpSingleServer[] _s5 = null;
        #endregion

        #region Private Methods
        /// <summary>
        /// On Multi Endpoint Server Client Count event.
        /// </summary>
        /// <param name="sender">The current server sending the message.</param>
        /// <param name="e1">The index action to perform.</param>
        private void ServerMultiEndpoint_OnMultiEndpointServerClientCount(object sender, IndexAction e1)
        {
            // Select the appropriate action.
            switch (e1)
            {
                case IndexAction.Increment:
                    // Make sure al list exists.
                    if (_s1 != null)
                    {
                        // For each server
                        foreach (Server item in _s1)
                        {
                            try
                            {
                                // If the current server is not the sending server.
                                if (!item.Equals(sender))
                                {
                                    // Increment the client count on the server.
                                    item.IncrementClientCount();
                                }
                            }
                            catch { }
                        }
                    }

                    // Make sure al list exists.
                    if (_s2 != null)
                    {
                        // For each server
                        foreach (ServerDynamic item in _s2)
                        {
                            try
                            {
                                // If the current server is not the sending server.
                                if (!item.Equals(sender))
                                {
                                    // Increment the client count on the server.
                                    item.IncrementClientCount();
                                }
                            }
                            catch { }
                        }
                    }

                    // Make sure al list exists.
                    if (_s3 != null)
                    {
                        // For each server
                        foreach (ServerSingle item in _s3)
                        {
                            try
                            {
                                // If the current server is not the sending server.
                                if (!item.Equals(sender))
                                {
                                    // Increment the client count on the server.
                                    item.IncrementClientCount();
                                }
                            }
                            catch { }
                        }
                    }

                    // Make sure al list exists.
                    if (_s4 != null)
                    {
                        // For each server
                        foreach (UdpServer item in _s4)
                        {
                            try
                            {
                                // If the current server is not the sending server.
                                if (!item.Equals(sender))
                                {
                                    // Increment the client count on the server.
                                    item.IncrementClientCount();
                                }
                            }
                            catch { }
                        }
                    }

                    // Make sure al list exists.
                    if (_s5 != null)
                    {
                        // For each server
                        foreach (UdpSingleServer item in _s5)
                        {
                            try
                            {
                                // If the current server is not the sending server.
                                if (!item.Equals(sender))
                                {
                                    // Increment the client count on the server.
                                    item.IncrementClientCount();
                                }
                            }
                            catch { }
                        }
                    }
                    break;

                case IndexAction.Decrement:
                    // Make sure al list exists.
                    if (_s1 != null)
                    {
                        // For each server
                        foreach (Server item in _s1)
                        {
                            try
                            {
                                // If the current server is not the sending server.
                                if (!item.Equals(sender))
                                {
                                    // Decrement the client count on the server.
                                    item.DecrementClientCount();
                                }
                            }
                            catch { }
                        }
                    }

                    // Make sure al list exists.
                    if (_s2 != null)
                    {
                        // For each server
                        foreach (ServerDynamic item in _s2)
                        {
                            try
                            {
                                // If the current server is not the sending server.
                                if (!item.Equals(sender))
                                {
                                    // Decrement the client count on the server.
                                    item.DecrementClientCount();
                                }
                            }
                            catch { }
                        }
                    }

                    // Make sure al list exists.
                    if (_s3 != null)
                    {
                        // For each server
                        foreach (ServerSingle item in _s3)
                        {
                            try
                            {
                                // If the current server is not the sending server.
                                if (!item.Equals(sender))
                                {
                                    // Decrement the client count on the server.
                                    item.DecrementClientCount();
                                }
                            }
                            catch { }
                        }
                    }

                    // Make sure al list exists.
                    if (_s4 != null)
                    {
                        // For each server
                        foreach (UdpServer item in _s4)
                        {
                            try
                            {
                                // If the current server is not the sending server.
                                if (!item.Equals(sender))
                                {
                                    // Decrement the client count on the server.
                                    item.DecrementClientCount();
                                }
                            }
                            catch { }
                        }
                    }

                    // Make sure al list exists.
                    if (_s5 != null)
                    {
                        // For each server
                        foreach (UdpSingleServer item in _s5)
                        {
                            try
                            {
                                // If the current server is not the sending server.
                                if (!item.Equals(sender))
                                {
                                    // Decrement the client count on the server.
                                    item.DecrementClientCount();
                                }
                            }
                            catch { }
                        }
                    }
                    break;

                default:
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    /// Multi-endpoint model.
    /// </summary>
    public class MultiEndpointModel
    {
        /// <summary>
        /// Gets sets, the ip address collection.
        /// </summary>
        public IPAddress[] Addresses { get; set; }

        /// <summary>
        /// Gets sets, the port number.
        /// </summary>
        public int Port { get; set; }

    }

    /// <summary>
    /// Multi-endpoint server context model.
    /// </summary>
    public class MultiEndpointServerContextModel
    {
        /// <summary>
        /// Gets sets, server context type.
        /// </summary>
        public Type ServerContextType { get; set; }

        /// <summary>
        /// Gets sets, the ip address collection.
        /// </summary>
        public IPAddress[] Addresses { get; set; }

        /// <summary>
        /// Gets sets, the port number.
        /// </summary>
        public int Port { get; set; }

    }
}
