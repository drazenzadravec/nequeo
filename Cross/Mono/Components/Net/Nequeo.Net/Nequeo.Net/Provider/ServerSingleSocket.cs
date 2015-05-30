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
    /// Server single socket provider.
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public abstract class ServerSingleSocket : Nequeo.Net.IInteractionContext, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Server single socket provider.
        /// </summary>
        protected ServerSingleSocket()
        {
        }

        /// <summary>
        /// Server single socket provider.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        protected ServerSingleSocket(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = 10000)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");

            _maxNumClients = maxNumClients;
            _multiEndpointModels = multiEndpointModels;
        }
        #endregion

        #region Private Fields
        private int READ_BUFFER_SIZE = 8192;
        private int WRITE_BUFFER_SIZE = 8192;

        private int _socketThreadCount = 1; // The total number of threads to use.
        private int _maxNumClients = 10000; // Maximun polling sokets.
        private int _timeoutMicrosecondsPolling = -1; // 100000 micro-seconds, 100 milli-seconds, 0.1 seconds.

        private Action<Net.Provider.ISingleContextMux> _onClientConnected = null;
        private Action<Net.Provider.ISingleContextBase> _onClientDisconnected = null;
        private Action<Nequeo.Net.Provider.Context> _onReceivedActionHandler = null;

        private IPAddress[] _addressesV4 = null;
        private IPAddress[] _addressesV6 = null;
        private IPAddress[] _sslAddressesV4 = null;
        private IPAddress[] _sslAddressesV6 = null;

        private Nequeo.Net.Provider.ServerSingleEndpoint _socketServer = null;
        private Net.Sockets.MultiEndpointModel[] _multiEndpointModels = null;

        private ISingleContextManager _timeoutManager = null;
        private ISingleContextManager _serverContextManager = null;
        private Nequeo.Net.IInteractionContext _integrationContext = null;

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

        private int _timeout = -1;
        private long _headerTimeout = -1;
        private int _requestTimeout = -1;
        private int _responseTimeout = -1;

        private string _serverName = string.Empty;
        private string _serviceName = string.Empty;
        private bool _maxNumClientsIndividualControl = true;

        private int _maxReadLength = 0;
        private int _requestBufferCapacity = 10000000;
        private int _responseBufferCapacity = 10000000;

        private ConcurrentQueue<Nequeo.Net.Provider.SendToContainer> _queue = null;
        private object _lockContextStoreObject = new object();
        #endregion

        #region Public Events
        /// <summary>
        /// The on send to clients complete event handler, triggered when data sent to all clients is complete.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Exception> OnSendToClientsComplete;

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
        /// Gets the socket server single endpoints.
        /// </summary>
        public Nequeo.Net.Provider.ServerSingleEndpoint Server
        {
            get { return _socketServer; }
        }

        /// <summary>
        /// Gets or sets on received action handler, new data has arrived. Should be used when implementing constant data arrivals.
        /// </summary>
        public Action<Nequeo.Net.Provider.Context> OnReceivedHandler
        {
            get { return _onReceivedActionHandler; }
            set { _onReceivedActionHandler = value; }
        }

        /// <summary>
        /// Gets or sets on received action handler, new data has arrived. Should be used when implementing constant data arrivals.
        /// </summary>
        public Action<Net.Provider.ISingleContextMux> OnClientConnected
        {
            get { return _onClientConnected; }
            set { _onClientConnected = value; }
        }

        /// <summary>
        /// Gets or sets on received action handler, new data has arrived. Should be used when implementing constant data arrivals.
        /// </summary>
        public Action<Net.Provider.ISingleContextBase> OnClientDisconnected
        {
            get { return _onClientDisconnected; }
            set { _onClientDisconnected = value; }
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
        /// Gets or sets the timeout (seconds) for each client connection when in-active.
        /// Disconnects the client when this time out is triggered.
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Gets or sets the maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.
        /// </summary>
        public long HeaderTimeout
        {
            get { return _headerTimeout; }
            set { _headerTimeout = value; }
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

        /// <summary>
        /// Gets or sets the maximum request buffer capacity when using buffered stream.
        /// </summary>
        public int RequestBufferCapacity
        {
            get { return _requestBufferCapacity; }
            set { _requestBufferCapacity = value; }
        }

        /// <summary>
        /// Gets or sets the maximum response buffer capacity when using buffered stream.
        /// </summary>
        public int ResponseBufferCapacity
        {
            get { return _responseBufferCapacity; }
            set { _responseBufferCapacity = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of bytes to read. This is used when reading initial headers,
        /// this prevents an infinite read of data. This is a DOS security measure.
        /// </summary>
        public int MaximumReadLength
        {
            get { return _maxReadLength; }
            set { _maxReadLength = value; }
        }

        /// <summary>
        /// Gets or sets the single context timeout manager.
        /// </summary>
        public ISingleContextManager TimeoutManager
        {
            get { return _timeoutManager; }
            set { _timeoutManager = value; }
        }

        /// <summary>
        /// Gets or sets the single context manager.
        /// </summary>
        public ISingleContextManager ServerContextManager
        {
            get { return _serverContextManager; }
            set { _serverContextManager = value; }
        }

        /// <summary>
        /// Gets or sets the interact context.
        /// </summary>
        public virtual Nequeo.Net.IInteractionContext IntegrationContext
        {
            get { return _integrationContext; }
            set { _integrationContext = value; }
        }

        /// <summary>
        /// Gets sets, the polling timeout (microseconds) for each socket state check.
        /// </summary>
        public int PollingTimeoutMicroseconds
        {
            get { return _timeoutMicrosecondsPolling; }
            set { _timeoutMicrosecondsPolling = value; }
        }

        /// <summary>
        /// Gets sets the number of threads to create when polling client sockets.
        /// </summary>
        public int SocketThreadCount
        {
            get { return _socketThreadCount; }
            set { _socketThreadCount = value; }
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
        /// Send the data to the clients connected to this system.
        /// </summary>
        /// <param name="data">The data to send to each server context client.</param>
        /// <remarks>This method does not send the data to the current context client, only
        /// to all the specified clients through the corresponding server context connections.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void SendToClients(byte[] data)
        {
            if (_serverContextManager != null)
            {
                try
                {
                    // Find all the connected clients.
                    Nequeo.Net.Provider.ISingleContextBase[] clients = _serverContextManager.FindAllSingle();

                    // Send the data to each client on this server.
                    SendToClients(data, clients);
                }
                catch { }
            }
        }

        /// <summary>
        /// Send the data to the clients connected to this system.
        /// </summary>
        /// <param name="data">The data to send to each server context client.</param>
        /// <param name="serverContexts">The collection of server context clients.</param>
        /// <remarks>This method does not send the data to the current context client, only
        /// to all the specified clients through the corresponding server context connections.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void SendToClients(byte[] data, Nequeo.Net.Provider.ISingleContextBase[] serverContexts)
        {
            // Make sure references exists.
            if (serverContexts == null || data == null)
                return;

            try
            {
                // Send the data.
                SendDataToClients(data, serverContexts);
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

        /// <summary>
        /// Send the data to the clients connected to this system.
        /// </summary>
        /// <param name="data">The data to send to each server context client.</param>
        /// <param name="uniqueIdentifiers">The collection of clients.</param>
        /// <returns>True if all the unique identifiers exist on this server; else false.</returns>
        public virtual bool SendToClients(byte[] data, string[] uniqueIdentifiers)
        {
            if (_serverContextManager != null)
            {
                try
                {
                    // Find all the clients on the current server.
                    Net.Provider.ISingleContextBase[] clients = _serverContextManager.FindSingle(uniqueIdentifiers);
                    if (clients != null && clients.Length > 0)
                    {
                        // Send the data to each client on this server.
                        SendToClients(data, clients);

                        // All clients exist on this server.
                        if (clients.Length == uniqueIdentifiers.Length)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                catch { return false; }
            }
            else
                return false;
        }

        /// <summary>
        /// Send the data to the client connected to this system.
        /// </summary>
        /// <param name="data">The data to send to each server context client.</param>
        /// <param name="uniqueIdentifier">The client to send the data to.</param>
        /// <returns>True if all the unique identifiers exist on this server; else false.</returns>
        public virtual bool SendToClient(byte[] data, string uniqueIdentifier)
        {
            if (_serverContextManager != null)
            {
                try
                {
                    // Find all the clients on the current server.
                    Net.Provider.ISingleContextBase[] clients = _serverContextManager.FindSingle(new string[] { uniqueIdentifier });
                    if (clients != null && clients.Length > 0)
                    {
                        // Send the data to each client on this server.
                        SendToClients(data, clients);

                        // All clients exist on this server.
                        if (clients.Length == 1)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                catch { return false; }
            }
            else
                return false;
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

        /// <summary>
        /// Send data to all identities and maintain an active connection (receivers that are on hosts).
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that is sending the data.</param>
        /// <param name="serviceName">The service name the unique identifier is connected to.</param>
        /// <param name="serviceNameUniqueIdentifiers">The service name the unique identifiers are connected to.</param>
        /// <param name="uniqueIdentifiers">The list of unique identities to send the data to.</param>
        /// <param name="data">The data to send to the receivers.</param>
        public virtual void SendToReceivers(string uniqueIdentifier, string serviceName, string serviceNameUniqueIdentifiers, string[] uniqueIdentifiers, byte[] data)
        {
            // Send to clients on other machines.
            if (_integrationContext != null)
                _integrationContext.SendToReceivers(uniqueIdentifier, serviceName, serviceNameUniqueIdentifiers, uniqueIdentifiers, data);
        }

        /// <summary>
        /// Send data to all identities and maintain an active connection (receivers that are on hosts). 
        /// Hosts and ports are included for an open channel to the remote nost.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that is sending the data.</param>
        /// <param name="serviceName">The service name the unique identifier is connected to.</param>
        /// <param name="serviceNameUniqueIdentifiers">The service name the unique identifiers are connected to.</param>
        /// <param name="uniqueIdentifiers">The list of unique identities to send the data to.</param>
        /// <param name="data">The data to send to the receivers.</param>
        /// <param name="hosts">The remote hosts that receivers (unique identities) are connected to.</param>
        /// <param name="ports">The remote host ports, each port must have a matching host.</param>
        public virtual void SendToReceivers(string uniqueIdentifier, string serviceName, string serviceNameUniqueIdentifiers, string[] uniqueIdentifiers, byte[] data, string[] hosts, string[] ports)
        {
            // Send to clients on other machines.
            if (_integrationContext != null)
                _integrationContext.SendToReceivers(uniqueIdentifier, serviceName, serviceNameUniqueIdentifiers, uniqueIdentifiers, data, hosts, ports);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Create the web context.
        /// </summary>
        /// <param name="context">The current server context.</param>
        /// <returns>The web context.</returns>
        protected virtual Nequeo.Net.WebContext CreateWebContext(Nequeo.Net.Provider.Context context)
        {
            // If the context has not been created.
            if (context.ContextState == null)
            {
                // Create the new context.
                Nequeo.Net.WebContext webContext = new Nequeo.Net.WebContext(context);

                // Assign the current context.
                webContext.IsSecureConnection = context.IsSecureConnection;
                webContext.RemoteEndPoint = context.RemoteEndPoint;
                webContext.ServerEndPoint = context.ServerEndPoint;
                webContext.Port = context.Port;
                webContext.Name = context.Name;
                webContext.ServiceName = context.ServiceName;
                webContext.NumberOfClients = context.NumberOfClients;
                webContext.UniqueIdentifier = context.UniqueIdentifier;
                webContext.SocketState = context.SocketState;
                webContext.IsAsyncMode = context.IsAsyncMode;
                webContext.ConnectionID = context.ConnectionID;
                webContext.SessionID = context.SessionID;
                webContext.IsAuthenticated = false;
                webContext.IsStartOfConnection = true;

                // Assign the request input stream and response output stream.
                AssignRequestResponseStreams(context, webContext);

                // Assign the current context.
                context.ContextState = webContext;
            }
            else
            {
                // Get the current context.
                Nequeo.Net.WebContext webContext = (Nequeo.Net.WebContext)context.ContextState;
                webContext.UniqueIdentifier = context.UniqueIdentifier;
                webContext.IsStartOfConnection = false;

                // Assign the current context.
                context.ContextState = webContext;
            }

            // Return the request context.
            return (Nequeo.Net.WebContext)context.ContextState;
        }

        /// <summary>
        /// Assign the request input stream and response output stream.
        /// </summary>
        /// <param name="context">The current client context.</param>
        /// <param name="webContext">The current web context.</param>
        protected virtual void AssignRequestResponseStreams(Nequeo.Net.Provider.Context context, Nequeo.Net.WebContext webContext)
        {
            // Create the response and request objects.
            webContext.WebResponse = Nequeo.Net.WebResponse.Create(context.Response.Output);
            webContext.WebRequest = Nequeo.Net.WebRequest.Create(context.Request.Input);
        }

        /// <summary>
        /// Save the web context state objects.
        /// </summary>
        /// <param name="context">The current client context.</param>
        /// <param name="webContext">The current web context.</param>
        protected virtual void SaveWebContext(Nequeo.Net.Provider.Context context, Nequeo.Net.WebContext webContext)
        {
            // Assign the state objects.
            context.UniqueIdentifier = webContext.UniqueIdentifier;
        }

        /// <summary>
        /// Parse all header data.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="resource">Get the request or response with the supplied data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected virtual List<NameValue> ParseHeaders(System.IO.Stream input, out string resource, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.ParseHeaders(input, out resource, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected virtual List<NameValue> ParseHeadersOnly(System.IO.Stream input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.ParseHeadersOnly(input, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n cariage return line feed).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        protected virtual byte[] ParseCRLF(System.IO.Stream input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.ParseCRLF(input, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        protected virtual byte[] Parse2CRLF(System.IO.Stream input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.Parse2CRLF(input, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse all header data only, no initial resources data is present.
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of headers; else null.</returns>
        protected virtual List<Nequeo.Model.NameValue> ParseHeadersOnly(byte[] input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.ParseHeadersOnly(input, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n cariage return line feed).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        protected virtual byte[] ParseCRLF(byte[] input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.ParseCRLF(input, timeout, maxReadLength);
        }

        /// <summary>
        /// Parse the input data until then end of the data is found (\r\n\r\n cariage return line feed repeat once).
        /// </summary>
        /// <param name="input">The stream containing the header data.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The bytes of data; else null.</returns>
        protected virtual byte[] Parse2CRLF(byte[] input, long timeout = -1, int maxReadLength = 0)
        {
            return Nequeo.Net.Utility.Parse2CRLF(input, timeout, maxReadLength);
        }

        /// <summary>
        /// Get a new web response stream.
        /// </summary>
        /// <param name="output">The web response output stream.</param>
        /// <returns>The web response stream.</returns>
        protected virtual Nequeo.Net.WebResponse GetResponse(System.IO.Stream output)
        {
            return Nequeo.Net.WebResponse.Create(output);
        }

        /// <summary>
        /// Add the member to the context manager.
        /// </summary>
        /// <param name="context">The member context to add.</param>
        protected virtual void AddMember(Net.Provider.ISingleContextBase context)
        {
            // Add the client.
            if (_timeoutManager != null)
                _timeoutManager.Add(context);

            if (_serverContextManager != null)
                _serverContextManager.Add(context);
        }

        /// <summary>
        /// Remove the member from the context manager.
        /// </summary>
        /// <param name="context">The member context to remove.</param>
        protected virtual void RemoveMember(Net.Provider.ISingleContextBase context)
        {
            // Remove the client.
            if (_timeoutManager != null)
                _timeoutManager.Remove(context);

            if (_serverContextManager != null)
                _serverContextManager.Remove(context);
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
                _socketServer = new Nequeo.Net.Provider.ServerSingleEndpoint(_multiEndpointModels, _maxNumClients);
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
                    _socketServer[i].Timeout = _timeout;
                    _socketServer[i].MaxNumClientsIndividualControl = _maxNumClientsIndividualControl;
                    _socketServer[i].MaxNumClients = _maxNumClients;
                    _socketServer[i].RequestBufferCapacity = _requestBufferCapacity;
                    _socketServer[i].ResponseBufferCapacity = _responseBufferCapacity;
                    _socketServer[i].ReadBufferSize = READ_BUFFER_SIZE;
                    _socketServer[i].WriteBufferSize = WRITE_BUFFER_SIZE;
                    _socketServer[i].PollingTimeoutMicroseconds = _timeoutMicrosecondsPolling;
                    _socketServer[i].SocketThreadCount = _socketThreadCount;
                }
                else
                {
                    _socketServer[i].ServiceName = _serviceName;
                }

                // Assign the event handlers.
                _socketServer[i].OnClientConnected += new Threading.EventHandler<Net.Provider.ISingleContextMux>(ServerSocket_OnClientConnected);
                _socketServer[i].OnClientDisconnected += new Threading.EventHandler<Net.Provider.ISingleContextBase>(ServerSocket_OnClientDisconnected);
                _socketServer[i].SendToServerInfoHandler = (serverContext) => SendToServerInfoHandler(serverContext);
            }
        }

        /// <summary>
        /// Client disconnected event.
        /// </summary>
        /// <param name="sender">The server sending the message.</param>
        /// <param name="e1">The server context sending the message.</param>
        private void ServerSocket_OnClientDisconnected(object sender, Net.Provider.ISingleContextBase e1)
        {
            // Remove the client.
            RemoveMember(e1);

            // Indicate the client has disconnected.
            if (_onClientDisconnected != null)
                _onClientDisconnected(e1);
        }

        /// <summary>
        /// Client connected event.
        /// </summary>
        /// <param name="sender">The server sending the message.</param>
        /// <param name="e1">The server context sending the message.</param>
        private void ServerSocket_OnClientConnected(object sender, Net.Provider.ISingleContextMux e1)
        {
            // Add the client.
            AddMember(e1);

            // Indicate the client has connected.
            if (_onClientConnected != null)
                _onClientConnected(e1);
        }

        /// <summary>
        /// Data sent from the server context to the server info.
        /// </summary>
        /// <param name="serverContext">The server context sending the data.</param>
        private void SendToServerInfoHandler(Net.Provider.SingleContext serverContext)
        {
            // Send the received to the caller.
            if (_onReceivedActionHandler != null)
            {
                // Make sure the context exists.
                if (serverContext.Context != null)
                {
                    // Assign the handler.
                    serverContext.Context.OnReceivedHandler = _onReceivedActionHandler;
                }
            }
        }

        /// <summary>
        /// Send the data to the server context clients.
        /// </summary>
        /// <param name="data">The data to send to each server context client.</param>
        /// <param name="serverContexts">The collection of server context clients.</param>
        private void SendDataToClients(byte[] data, Nequeo.Net.Provider.ISingleContextBase[] serverContexts)
        {
            try
            {
                // Create a new queue.
                if (_queue == null)
                    _queue = new ConcurrentQueue<SendToContainer>();

                // Create the container.
                Nequeo.Net.Provider.SendToContainer container = new SendToContainer()
                {
                    Data = data,
                    SingleServerContexts = serverContexts
                };

                // Queue an new container.
                _queue.Enqueue(container);

                // Send the data to the server context clients.
                SendDataToClientsAsync();
            }
            catch { }
        }

        /// <summary>
        /// Send the data to the server context clients.
        /// </summary>
        private async void SendDataToClientsAsync()
        {
            await Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        // Send to each server context the specified data.
                        SendDataToClientsParallel();
                    }
                    catch { }
                });
        }

        /// <summary>
        /// Send to each server context the specified data.
        /// </summary>
        private void SendDataToClientsParallel()
        {
            // Error message.
            StringBuilder errorMessage = new StringBuilder();

            // Get the next item in the queue.
            SendToContainer container = null;
            while (_queue.TryDequeue(out container))
            {
                // Get the data and list.
                byte[] data = container.Data;
                Nequeo.Net.Provider.ISingleContextBase[] serverContexts = container.SingleServerContexts;

                // If conenction exist.
                if (serverContexts != null)
                {
                    // If more than zero connections exist.
                    if (serverContexts.Count() > 0)
                    {
                        // Start a new parallel operation for each connection.
                        Parallel.ForEach<Nequeo.Net.Provider.ISingleContextBase>(serverContexts, u =>
                        {
                            try
                            {
                                // Make sure the current object has not be disposed.
                                if (u != null)
                                {
                                    if (data.Length > 0)
                                    {
                                        // Send the data to the current composite service.
                                        u.SentFromServer(data);

                                        // Get the current error from the server.
                                        Exception exception = u.GetLastError();
                                        if (exception != null)
                                            errorMessage.Append(exception.Message + "\r\n");
                                    }
                                }
                            }
                            catch { }
                        });
                    }
                }
            }

            try
            {
                // Trigger the complete event.
                if (OnSendToClientsComplete != null)
                    OnSendToClientsComplete(this, new Exception(errorMessage.ToString()));
            }
            catch { }
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

            // Are there any secure servers.
            bool isSecure = true;
            if (isSecure)
            {
                // Get the certificate.
                X509Certificate2 certificate = _securityReader.GetServerCredentials();

                // If a certificate has been loaded.
                if (certificate != null)
                {
                    // Set the secure servers certificate property.
                    _socketServer[2].UseSslConnection = isSecure;
                    _socketServer[3].UseSslConnection = isSecure;

                    _socketServer[2].X509Certificate = certificate;
                    _socketServer[3].X509Certificate = certificate;
                }
            }
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
            // Set the time out.
            _socketServer[index].Timeout = serverHostElement.ClientTimeOut;

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
                _queue = null;
                _lockContextStoreObject = null;
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
        ~ServerSingleSocket()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
