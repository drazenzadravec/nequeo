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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Security.Principal;

using Nequeo.Model;
using Nequeo.Handler;
using Nequeo.Net.Configuration;

namespace Nequeo.Net.Provider
{
    /// <summary>
    /// UDP Server socket provider.
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public abstract class UdpSingleServerSocket : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Server socket provider.
        /// </summary>
        protected UdpSingleServerSocket()
        {
        }

        /// <summary>
        /// Server socket provider.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        protected UdpSingleServerSocket(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");

            _maxNumClients = maxNumClients;
            _multiEndpointModels = multiEndpointModels;
        }
        #endregion

        #region Private Fields
        private int READ_BUFFER_SIZE = 8192;
        private int WRITE_BUFFER_SIZE = 8192;

        private Action<Nequeo.Net.Sockets.IUdpSingleServer, byte[], IPEndPoint> _onReceivedActionHandler = null;

        private IPAddress[] _addressesV4 = null;
        private IPAddress[] _addressesV6 = null;
        private IPAddress[] _sslAddressesV4 = null;
        private IPAddress[] _sslAddressesV6 = null;

        private Nequeo.Net.Provider.UdpSingleServerEndpoint _socketServer = null;
        private Net.Sockets.MultiEndpointModel[] _multiEndpointModels = null;

        private Net.Configuration.ServerHostElement _serverHostElementV4 = null;
        private Net.Configuration.ServerHostElement _serverHostElementV6 = null;
        private Net.Configuration.ServerHostElement _sslServerHostElementV4 = null;
        private Net.Configuration.ServerHostElement _sslServerHostElementV6 = null;
        private System.Configuration.AppSettingsReader _appSettingsReader = null;

        private Nequeo.Security.Configuration.Reader _securityReader = null;
        private Nequeo.Net.Configuration.Reader _serverReader = null;

        private string _hostV4 = "SocketProviderV4";
        private string _hostV6 = "SocketProviderV6";
        private string _sslHostV4 = "SocketProviderV4Ssl";
        private string _sslHostV6 = "SocketProviderV6Ssl";

        private int _collectiveMaxNumberOfClientsValue = Int32.MaxValue;
        private string _socketProviderHostPrefix = "";
        private bool _isListening = false;

        private int _requestTimeout = -1;
        private int _responseTimeout = -1;

        private string _serverName = string.Empty;
        private string _serviceName = string.Empty;
        private bool _maxNumClientsIndividualControl = true;
        private int _maxNumClients = Int32.MaxValue;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the read buffer size.
        /// </summary>
        public int ReadBufferSize
        {
            get { return READ_BUFFER_SIZE; }
            set { READ_BUFFER_SIZE = value; }
        }

        /// <summary>
        /// Gets sets, the write buffer size.
        /// </summary>
        public int WriteBufferSize
        {
            get { return WRITE_BUFFER_SIZE; }
            set { WRITE_BUFFER_SIZE = value; }
        }

        /// <summary>
        /// Gets or sets on received action handler, new data has arrived. Should be used when implementing constant data arrivals.
        /// </summary>
        public Action<Nequeo.Net.Sockets.IUdpSingleServer, byte[], IPEndPoint> OnReceivedHandler
        {
            get { return _onReceivedActionHandler; }
            set { _onReceivedActionHandler = value; }
        }

        /// <summary>
        /// Gets the socket server endpoints.
        /// </summary>
        public Nequeo.Net.Provider.UdpSingleServerEndpoint Server
        {
            get { return _socketServer; }
        }

        /// <summary>
        /// Gets, the is listening server indicator.
        /// </summary>
        public bool IsListening
        {
            get { return _isListening; }
        }

        /// <summary>
        /// Gets or sets the multi-endpoint hosts and port configuration.
        /// </summary>
        public Net.Sockets.MultiEndpointModel[] MultiEndpoint
        {
            get { return _multiEndpointModels; }
            set { _multiEndpointModels = value; }
        }

        /// <summary>
        /// Gets or sets the socket provider host prefix, this is only used with the configuration file.
        /// </summary>
        public string SocketProviderHostPrefix
        {
            get { return _socketProviderHostPrefix; }
            set { _socketProviderHostPrefix = value; }
        }

        /// <summary>
        /// Gets or sets the length of time, in milliseconds, before the request times out; -1 wait indefinitely.
        /// </summary>
        public int RequestTimeout
        {
            get { return _requestTimeout; }
            set { _requestTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the length of time, in milliseconds, before the response times out; -1 wait indefinitely.
        /// </summary>
        public int ResponseTimeout
        {
            get { return _responseTimeout; }
            set { _responseTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the current server name.
        /// </summary>
        public string Name
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        /// <summary>
        /// Gets or sets the common service name.
        /// </summary>
        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of concurrent clients that are allowed to be connected.
        /// </summary>
        public int MaxNumClients
        {
            get { return _maxNumClients; }
            set { _maxNumClients = value; }
        }

        /// <summary>
        /// Gets or sets should this server control maximum number of clients
        /// independent of all other servers within the multi-endpoint.
        /// Is this server part of a collection of multi-endpoint servers.
        /// </summary>
        public bool MaxNumClientsIndividualControl
        {
            get { return _maxNumClientsIndividualControl; }
            set { _maxNumClientsIndividualControl = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start the server listener.
        /// </summary>
        public virtual void Start()
        {
            try
            {
                // If not listening.
                if (!_isListening)
                {
                    // Initialise the server.
                    Initialisation();

                    // If created.
                    if (_socketServer != null)
                    {
                        // Start the servers.
                        _socketServer.Start();

                        // Search for errors.
                        foreach (Exception exception in _socketServer.GetLastError())
                        {
                            // If an error has been found.
                            if (exception != null)
                                throw exception;
                        }
                    }

                    _isListening = true;
                }
            }
            catch (Exception ex)
            {
                _isListening = false;

                // Dispose of the server if error.
                if (_socketServer != null)
                {
                    _socketServer.Dispose();
                    _socketServer = null;
                }

                // If an event application name exists for logging.
                if (!String.IsNullOrEmpty(Helper.EventApplicationName))
                {
                    // Log the error.
                    LogHandler.WriteTypeMessage(ex.Message,
                        MethodInfo.GetCurrentMethod(), Helper.EventApplicationName);
                }
            }
        }

        /// <summary>
        /// Stop the server listener.
        /// </summary>
        public virtual void Stop()
        {
            try
            {
                // If listening.
                if (_isListening)
                {
                    // If created.
                    if (_socketServer != null)
                    {
                        // Stop all the servers.
                        _socketServer.Stop();
                    }

                    // Search for errors.
                    foreach (Exception exception in _socketServer.GetLastError())
                    {
                        // If an error has been found.
                        if (exception != null)
                            throw exception;
                    }
                }
            }
            catch (Exception ex)
            {
                // If an event application name exists for logging.
                if (!String.IsNullOrEmpty(Helper.EventApplicationName))
                {
                    // Log the error.
                    LogHandler.WriteTypeMessage(ex.Message,
                        MethodInfo.GetCurrentMethod(), Helper.EventApplicationName);
                }
            }
            finally
            {
                _isListening = false;

                // Dispose of the server if error.
                if (_socketServer != null)
                {
                    _socketServer.Dispose();
                    _socketServer = null;
                }
            }
        }

        /// <summary>
        /// Initialise the server.
        /// </summary>
        /// <param name="useConfiguration">Use this method when loading from the configuration file.</param>
        public virtual void Initialisation(bool useConfiguration = false)
        {
            try
            {
                // Initialise the server, use this method when loading from the configuration file.
                if (useConfiguration)
                    Initialise();

                // Create the server.
                CreateServer();

                // Apply configuration.
                if (useConfiguration)
                    ApplyConfiguration();

                // Start the server.
                StartServer(useConfiguration);
            }
            catch (Exception ex)
            {
                // If an event application name exists for logging.
                if (!String.IsNullOrEmpty(Helper.EventApplicationName))
                {
                    // Log the error.
                    LogHandler.WriteTypeMessage(ex.Message,
                        MethodInfo.GetCurrentMethod(), Helper.EventApplicationName);
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Create the server.
        /// </summary>
        private void CreateServer()
        {
            // If the server has not been created.
            if (_socketServer == null)
            {
                // Create the server from the configuration details.
                _socketServer = new Nequeo.Net.Provider.UdpSingleServerEndpoint(_multiEndpointModels, _maxNumClients);
            }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        /// <param name="useConfig">True if the configuration file is used; else false.</param>
        private void StartServer(bool useConfig = false)
        {
            // For each server.
            for (int i = 0; i < _socketServer.NumberOfServers; i++)
            {
                // Not using the configuration file.
                if (!useConfig)
                {
                    _socketServer[i].ServiceName = _serviceName;
                    _socketServer[i].Name = _serverName;
                    _socketServer[i].MaxNumClientsIndividualControl = _maxNumClientsIndividualControl;
                    _socketServer[i].MaxNumClients = _maxNumClients;
                    _socketServer[i].ReadBufferSize = READ_BUFFER_SIZE;
                    _socketServer[i].WriteBufferSize = WRITE_BUFFER_SIZE;
                }
                else
                {
                    _socketServer[i].ServiceName = _serviceName;
                }

                // Assign the event handlers.
                _socketServer[i].ReceivedHandler = (server, data, endpoint) => ReceivedHandler(server, data, endpoint);
            }
        }

        /// <summary>
        /// Recevied data handler.
        /// </summary>
        /// <param name="server">The UDP server.</param>
        /// <param name="data">The data received.</param>
        /// <param name="endpoint">The end point of the client sendind the data.</param>
        private void ReceivedHandler(Nequeo.Net.Sockets.IUdpSingleServer server, byte[] data, IPEndPoint endpoint)
        {
            // Send the received to the caller.
            if (_onReceivedActionHandler != null)
            {
                // Send the data to the handler.
                _onReceivedActionHandler(server, data, endpoint);
            }
        }
        #endregion

        #region Private Configuration Methods
        /// <summary>
        /// Initialise the server, use this method when loading from the configuration file.
        /// </summary>
        private void Initialise()
        {
            // Create the configuration readers.
            _securityReader = new Nequeo.Security.Configuration.Reader();
            _serverReader = new Net.Configuration.Reader();
            _appSettingsReader = new System.Configuration.AppSettingsReader();

            // Get the non-secure IPv4 and IPv6 host details.
            _serverHostElementV4 = _serverReader.GetServerHost(_socketProviderHostPrefix + _hostV4);
            _serverHostElementV6 = _serverReader.GetServerHost(_socketProviderHostPrefix + _hostV6);

            // Get the secure IPv4 and IPv6 host details.
            _sslServerHostElementV4 = _serverReader.GetServerHost(_socketProviderHostPrefix + _sslHostV4);
            _sslServerHostElementV6 = _serverReader.GetServerHost(_socketProviderHostPrefix + _sslHostV6);

            // Get the collective maximum number
            // of clients for all servers.
            _collectiveMaxNumberOfClientsValue = Int32.MaxValue;

            // Create the non-secure IPv4 endpoint.
            _addressesV4 = new IPAddress[1];
            _addressesV4[0] = IPAddress.Any;

            // Create the non-secure IPv6 endpoint.
            _addressesV6 = new IPAddress[1];
            _addressesV6[0] = IPAddress.IPv6Any;

            // Create the secure IPv4 endpoint.
            _sslAddressesV4 = new IPAddress[1];
            _sslAddressesV4[0] = IPAddress.Any;

            // Create the secure IPv6 endpoint.
            _sslAddressesV6 = new IPAddress[1];
            _sslAddressesV6[0] = IPAddress.IPv6Any;

            // Create the milti-endpoint model.
            _multiEndpointModels = new Net.Sockets.MultiEndpointModel[4];

            // Assign the non-secure IPv4 model details.
            AssignServerMultiModelDetails(0, _addressesV4, _serverHostElementV4);

            // Assign the non-secure IPv6 model details.
            AssignServerMultiModelDetails(1, _addressesV6, _serverHostElementV6);

            // Assign the secure IPv4 model details.
            AssignServerMultiModelDetails(2, _sslAddressesV4, _sslServerHostElementV4);

            // Assign the secure IPv6 model details.
            AssignServerMultiModelDetails(3, _sslAddressesV6, _sslServerHostElementV6);

            // Assign from max collective clients.
            _maxNumClients = _collectiveMaxNumberOfClientsValue;
        }

        /// <summary>
        /// Apply the configuration data.
        /// </summary>
        private void ApplyConfiguration()
        {
            // Assign the non-secure IPv4 endpoint details.
            AssignServerDetails(0, _serverHostElementV4);

            // Assign the non-secure IPv6 endpoint details.
            AssignServerDetails(1, _serverHostElementV6);

            // Assign the secure IPv4 endpoint details.
            AssignServerDetails(2, _sslServerHostElementV4);

            // Assign the secure IPv6 endpoint details.
            AssignServerDetails(3, _sslServerHostElementV6);
        }

        /// <summary>
        /// Assign server multi model details.
        /// </summary>
        /// <param name="index">The multi-endpoint index.</param>
        /// <param name="addresses">The IP address endpoint collection.</param>
        /// <param name="serverHostElement">The server host element for the current server.</param>
        private void AssignServerMultiModelDetails(int index, IPAddress[] addresses, Net.Configuration.ServerHostElement serverHostElement)
        {
            // Assign the IPv4 or IPv6 model details.
            _multiEndpointModels[index].Addresses = addresses;
            _multiEndpointModels[index].Port = serverHostElement.Port;
        }

        /// <summary>
        /// Assign server details.
        /// </summary>
        /// <param name="index">The server index.</param>
        /// <param name="serverHostElement">The server host element for the current server.</param>
        private void AssignServerDetails(int index, Net.Configuration.ServerHostElement serverHostElement)
        {
            // If the maximum number of clients is individual control by the server.
            // such that it is not collectively controlled.
            if (serverHostElement.IndividualControl)
            {
                _socketServer[index].MaxNumClientsIndividualControl = serverHostElement.IndividualControl;
                _socketServer[index].MaxNumClients = serverHostElement.MaxNumClients;
            }
            else
            {
                _socketServer[index].MaxNumClientsIndividualControl = serverHostElement.IndividualControl;
                _socketServer[index].MaxNumClients = _collectiveMaxNumberOfClientsValue;
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
                    if (_socketServer != null)
                        _socketServer.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _socketServer = null;
                _multiEndpointModels = null;
                _addressesV4 = null;
                _addressesV6 = null;
                _sslAddressesV4 = null;
                _sslAddressesV6 = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~UdpSingleServerSocket()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
