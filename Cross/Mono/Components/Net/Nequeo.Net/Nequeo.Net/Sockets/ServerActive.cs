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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Security;
using Nequeo.Threading;
using Nequeo.Net.Provider;

namespace Nequeo.Net.Sockets
{
    /// <summary>
    /// General socket server (multi-client), creates a new active thread for each 
    /// client until the client is disconnected.
    /// </summary>
    public partial class ServerActive : IServerBase, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Socket server constructor.
        /// </summary>
        /// <param name="address">An IP address.</param>
        /// <param name="port">The port to connect to.</param>
        /// <remarks>The serverContextType must inherit Nequeo.Net.SocketServerContext.</remarks>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public ServerActive(IPAddress address, int port)
        {
            if (address == null) throw new ArgumentNullException("address");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");

            _specificAddressFamily = address.AddressFamily;
            _address = address;
            _port = port;
        }
        #endregion

        #region Private Fields
        private int READ_BUFFER_SIZE = 8192;
        private int WRITE_BUFFER_SIZE = 8192;

        private Thread _hostThread = null;
        private Exception _lastException = null;

        private int _port = -1;
        private IPAddress _address = null;

        private int _timeOut = -1;
        private int _clientCount = 0;
        private int _maxNumClients = Int32.MaxValue;
        private object _lockingObject = new object();

        private string _serverName = string.Empty;
        private string _serviceName = string.Empty;

        private bool _beginSslAuthentication = true;
        private bool _useSslConnection = false;
        private X509Certificate2 _sslCertificate = null;
        private SslProtocols _sslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls | SslProtocols.Ssl3;
        private SocketFlags _receiveSocketFlags = SocketFlags.None;
        private SocketFlags _sendSocketFlags = SocketFlags.None;

        private System.Net.Sockets.Socket _socket = null;
        private bool _isListening = false;

        private System.Net.Sockets.ProtocolType _protocolType = System.Net.Sockets.ProtocolType.Tcp;
        private System.Net.Sockets.SocketType _socketType = System.Net.Sockets.SocketType.Stream;
        private System.Net.Sockets.AddressFamily _specificAddressFamily = AddressFamily.Unspecified;

        private Action<SingleContext> _sendToServerInfoHandler = null;
        private Action<Nequeo.Net.Provider.Context> _onReceivedActionHandler = null;

        // Looks for connection avaliability.
        private AutoResetEvent _connAvailable =
            new AutoResetEvent(false);

        private int _requestBufferCapacity = 10000000;
        private int _responseBufferCapacity = 10000000;

        /// <summary>
        /// Is this server part of a collection of endpoint servers.
        /// </summary>
        private bool _maxNumClientsIndividualControl = true;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, should the server authenticate with the client and start a secure connection.
        /// </summary>
        /// <remarks>True if authentication begins within the Connect method; else false.
        /// When the Connect method is called and a secure connection has been chosen
        /// then this property is call asking if a secure connection is to be established immediatly.
        /// This is generally used for SSL protocol.</remarks>
        public bool BeginSslAuthentication
        {
            get { return _beginSslAuthentication; }
            set { _beginSslAuthentication = value; }
        }

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
        /// Gets sets the maximum request buffer capacity when using buffered stream.
        /// </summary>
        public int RequestBufferCapacity
        {
            get { return _requestBufferCapacity; }
            set { _requestBufferCapacity = value; }
        }

        /// <summary>
        /// Gets sets the maximum response buffer capacity when using buffered stream.
        /// </summary>
        public int ResponseBufferCapacity
        {
            get { return _responseBufferCapacity; }
            set { _responseBufferCapacity = value; }
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
        /// Gets sets, the handler to the server receive only information internally.
        /// </summary>
        internal Action<SingleContext> SendToServerInfoHandler
        {
            get { return _sendToServerInfoHandler; }
            set { _sendToServerInfoHandler = value; }
        }

        /// <summary>
        /// Gets sets, should this server control maximum number of clients
        /// independent of all other servers within the multi-endpoint.
        /// Is this server part of a collection of multi-endpoint servers.
        /// </summary>
        public bool MaxNumClientsIndividualControl
        {
            get { return _maxNumClientsIndividualControl; }
            set { _maxNumClientsIndividualControl = value; }
        }

        /// <summary>
        /// Gets, the current socket listener.
        /// </summary>
        public System.Net.Sockets.Socket Socket
        {
            get { return _socket; }
        }

        /// <summary>
        /// Gets, the addressing scheme that an instance of the connection to use.
        /// </summary>
        public System.Net.Sockets.AddressFamily AddressFamily
        {
            get { return _specificAddressFamily; }
        }

        /// <summary>
        /// Gets sets, the x.509 certificate used for a secure connection.
        /// </summary>
        public X509Certificate2 X509Certificate
        {
            get { return _sslCertificate; }
            set { _sslCertificate = value; }
        }

        /// <summary>
        /// Gets sets, IP address.
        /// </summary>
        public IPAddress IPAddress
        {
            get { return _address; }
            set
            {
                if (value == null) throw new ArgumentNullException("IPAddress");
                _address = value;
                _specificAddressFamily = _address.AddressFamily;
            }
        }

        /// <summary>
        /// Gets sets, the protocol type.
        /// </summary>
        public System.Net.Sockets.ProtocolType ProtocolType
        {
            get { return _protocolType; }
            set { _protocolType = value; }
        }

        /// <summary>
        /// Gets sets, specifies socket receive behaviors.
        /// </summary>
        public SocketFlags ReceiveSocketFlags
        {
            get { return _receiveSocketFlags; }
            set { _receiveSocketFlags = value; }
        }

        /// <summary>
        /// Gets sets, specifies socket send behaviors.
        /// </summary>
        public SocketFlags SendSocketFlags
        {
            get { return _sendSocketFlags; }
            set { _sendSocketFlags = value; }
        }

        /// <summary>
        /// Gets sets, the socket type.
        /// </summary>
        public System.Net.Sockets.SocketType SocketType
        {
            get { return _socketType; }
            set { _socketType = value; }
        }

        /// <summary>
        /// Gets sets, port to use.
        /// </summary>
        public int Port
        {
            get { return _port; }
            set
            {
                if (value < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");
                _port = value;
            }
        }

        /// <summary>
        /// Gets sets, use a secure connection.
        /// </summary>
        public bool UseSslConnection
        {
            get { return _useSslConnection; }
            set { _useSslConnection = value; }
        }

        /// <summary>
        /// Gets sets, defines the possible versions of System.Security.Authentication.SslProtocols.
        /// </summary>
        public SslProtocols SslProtocols
        {
            get { return _sslProtocols; }
            set { _sslProtocols = value; }
        }

        /// <summary>
        /// Gets sets, the timeout (seconds) for each client connection when in-active.
        /// Disconnects the client when this time out is triggered.
        /// </summary>
        public int Timeout
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        /// <summary>
        /// Gets sets, the maximum number of concurrent clients that are allowed to be connected.
        /// </summary>
        public int MaxNumClients
        {
            get { return _maxNumClients; }
            set { _maxNumClients = value; }
        }

        /// <summary>
        /// Gets, the number of clients currently connected to the server.
        /// </summary>
        public int NumberOfClients
        {
            get { return _clientCount; }
        }

        /// <summary>
        /// Gets, an indicator to see if the server is listening (accepting connections).
        /// </summary>
        public bool IsListening
        {
            get { return _isListening; }
        }

        /// <summary>
        /// Gets sets, the current server name.
        /// </summary>
        public string Name
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        /// <summary>
        /// Gets sets, the common service name.
        /// </summary>
        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// When in multi endpoint server mode.
        /// </summary>
        internal event Nequeo.Threading.EventHandler<IndexAction> OnMultiEndpointServerClientCount;

        /// <summary>
        /// The on stop listening event handler, triggered when the server unexpectadly stops listening
        /// or can not listen after attempting to start the server listening.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Exception> OnStopListening;

        /// <summary>
        /// The on start listening event handler, triggered when the server starts listening.
        /// </summary>
        public event System.EventHandler OnStartListening;

        /// <summary>
        /// The on client connected event handler, triggered when a new connection is established.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ISingleContext> OnClientConnected;

        /// <summary>
        /// The on send to clients complete event handler, triggered when data sent to all clients is complete.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Exception> OnSendToClientsComplete;

        #endregion

        #region Public Methods
        /// <summary>
        /// Start listening on a new thread.
        /// </summary>
        public virtual void StartListeningThread()
        {
            try
            {
                // Clear last error.
                ClearLastError();

                // If not listening then attempt to start the server.
                if (!_isListening)
                {
                    // Create new threads for each
                    // file transfer server.
                    _hostThread = new Thread(new ThreadStart(StartListening));
                    _hostThread.IsBackground = true;
                    _hostThread.Start();
                    Thread.Sleep(20);
                }
            }
            catch (Exception ex)
            {
                _hostThread = null;
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Stop listening on the thread.
        /// </summary>
        public virtual void StopListeningThread()
        {
            try
            {
                // Clear last error.
                ClearLastError();

                StopListening();

                // Abort all threads created
                // for file transfer instances.
                if (_hostThread != null)
                {
                    if (_hostThread.IsAlive)
                    {
                        _hostThread.Abort();
                        _hostThread.Join();
                        Thread.Sleep(20);
                    }
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
            finally
            {
                _hostThread = null;
            }
        }

        /// <summary>
        /// Start listening and close the current socket connection.
        /// </summary>
        /// <remarks>This method blocks the current thread.</remarks>
        public virtual void StartListening()
        {
            try
            {
                // Clear last error.
                ClearLastError();

                // If not listening then attempt to start the server.
                if (!_isListening)
                {
                    // Create the end point from the address and port.
                    IPEndPoint endPoint = new IPEndPoint(_address, _port);

                    // If using ssl.
                    if (_useSslConnection)
                    {
                        // If ssl certificate has not been assigned.
                        if (_sslCertificate == null)
                            throw new Exception("Please add an SSL certificate for secure connections.");
                    }

                    // Create the server binding and start listening.
                    _socket = new Socket(endPoint.AddressFamily, _socketType, _protocolType);
                    _socket.Bind(endPoint);
                    _socket.Listen(0);

                    // Trigger the start listening event.
                    StartListeningEvent();

                    // Post accepts on the listening socket.
                    StartAccept();
                }
            }
            catch (Exception ex)
            {
                // Stop listening.
                StopListening();

                SetLastError(ex);

                // Trigger the stop listening event.
                StopListeningEvent(ex);
            }
        }

        /// <summary>
        /// Stop listening and close the current socket connection, also releases all resources.
        /// </summary>
        public virtual void StopListening()
        {
            // Stop listening.
            StopListeningEx();

            // Release the resources.
            _socket = null;
        }

        /// <summary>
        /// Get the last error that occured.
        /// </summary>
        /// <returns>The exception containing the error; else null.</returns>
        public Exception GetLastError()
        {
            return _lastException;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Start accepting socket connections.
        /// </summary>
        private void StartAccept()
        {
            try
            {
                // Clear last error.
                ClearLastError();

                do
                {
                    // Set the event to nonsignaled state.
                    _connAvailable.Reset();

                    // Do not allow any more clients
                    // if maximum is reached.
                    if (_clientCount < _maxNumClients)
                    {
                        System.Net.Sockets.Socket clientSocket = null;

                        // Create a new client connection handler for the current
                        // tcp client attempting to connect to the server. Creates
                        // a new channel from the client to the server.
                        clientSocket = _socket.Accept();
                            
                        // Create a new instance of the server context type.
                        object state = null;
                        state = clientSocket;
                            
                        Thread acceptContext = null;
                        try
                        {
                            // Start a new worker thread.
                            acceptContext = new Thread(CreateServerContext);
                            acceptContext.IsBackground = true;
                            acceptContext.Start(state);
                        }
                        catch (Exception ex)
                        {
                            if (clientSocket != null)
                                clientSocket.Dispose();

                            acceptContext = null;
                            SetLastError(ex);
                        }

                        Thread.Sleep(20);
                    }
                    else
                    {
                        // Blocks the current thread until a
                        // connection becomes available.
                        _connAvailable.WaitOne();
                    }

                } while (true);
            }
            catch (Exception ex)
            {
                // Stop listening.
                StopListening();

                SetLastError(ex);

                // Trigger the stop listening event.
                StopListeningEvent(ex);
            }
        }

        /// <summary>
        /// Create a new instance of the server context type.
        /// </summary>
        /// <param name="state">The current state, contains a socket client.</param>
        private void CreateServerContext(object state)
        {
            SingleContext client = null;
            bool endTriggered = false;
            
            try
            {
                // Clear last error.
                ClearLastError();

                // Create a new client host
                client = new SingleContext();

                // Assign the accepted socket.
                client.Socket = (System.Net.Sockets.Socket)state;
                client.UseSslConnection = _useSslConnection;
                client.BeginSslAuthentication = _beginSslAuthentication;
                client.X509Certificate = _sslCertificate;
                client.Name = _serverName;
                client.SslProtocols = _sslProtocols;
                client.SocketType = _socketType;
                client.ProtocolType = _protocolType;
                client.ReceiveSocketFlags = _receiveSocketFlags;
                client.SendSocketFlags = _sendSocketFlags;
                client.Timeout = _timeOut;
                client.RequestBufferCapacity = _requestBufferCapacity;
                client.ResponseBufferCapacity = _responseBufferCapacity;
                client.Port = _port;
                client.ServiceName = _serviceName;
                client.NumberOfClients = _clientCount;
                client.OnReceivedHandler = _onReceivedActionHandler;
                client.SendToServerInfoHandler = _sendToServerInfoHandler;
                client.WriteBufferSize = WRITE_BUFFER_SIZE;
                client.ReadBufferSize = READ_BUFFER_SIZE;
                client.EndConnectionCallback = (end, socketClient) =>
                {
                    // End has been triggered.
                    endTriggered = end;

                    // End the connection.
                    EndConnection(socketClient);
                };

                // Create and assign each component.
                client.Initialise();

                // Increment the count.
                IncrementCount();

                try
                {
                    // Send the new connection to the caller.
                    if (OnClientConnected != null)
                        OnClientConnected(this, client);
                }
                catch (Exception ex)
                {
                    if (client != null)
                        client.Dispose();
                    SetLastError(ex);
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Stop listening event trigger.
        /// </summary>
        /// <param name="ex">The exception.</param>
        private void StopListeningEvent(Exception ex)
        {
            try
            {
                // Trigger the stop listening event.
                if (OnStopListening != null)
                    OnStopListening(this, ex);
            }
            catch { }
        }

        /// <summary>
        /// Start listening event trigger.
        /// </summary>
        private void StartListeningEvent()
        {
            _isListening = true;

            try
            {
                // Trigger the start listening event.
                if (OnStartListening != null)
                    OnStartListening(this, new EventArgs());
            }
            catch { }
        }

        /// <summary>
        /// Stop listening and close the current socket connection, also releases all resources.
        /// </summary>
        private void StopListeningEx()
        {
            _isListening = false;

            try
            {
                // Clear last error.
                ClearLastError();

                // Shutdown the socket.
                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            try
            {
                // Close the socket.
                if (_socket != null)
                {
                    _socket.Disconnect(false);
                }
            }
            catch { }

            try
            {
                // Close the socket.
                if (_socket != null)
                {
                    _socket.Close();
                    _socket.Dispose();
                }
            }
            catch { }
        }

        /// <summary>
        /// Ends the current client connection.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        private void EndConnection(SingleContext sender)
        {
            // Decrement the count.
            DecrementCount();

            // Signal to the blocking handler
            // to un-block.
            lock (_lockingObject)
                _connAvailable.Set();

            try
            {
                lock (_lockingObject)
                {
                    // Attempt to release all the resources of the curretn server context.
                    sender.Dispose();
                    sender = null;
                }
            }
            catch { }
        }

        /// <summary>
        /// Increment the client count.
        /// </summary>
        private void IncrementCount()
        {
            IncrementClientCount();

            // If is this server part of a collection of endpoint servers.
            // Should this server control maximum number of clients
            // independent of all other servers within the multi-endpoint.
            if (!_maxNumClientsIndividualControl)
            {
                lock (_lockingObject)
                {
                    // Trigger the server list incrementor.
                    if (OnMultiEndpointServerClientCount != null)
                        OnMultiEndpointServerClientCount(this, IndexAction.Increment);
                }
            }
        }

        /// <summary>
        /// Increment the client count.
        /// </summary>
        internal void IncrementClientCount()
        {
            // Increment the count.
            Interlocked.Increment(ref _clientCount);
        }

        /// <summary>
        /// Decrement the client count.
        /// </summary>
        private void DecrementCount()
        {
            DecrementClientCount();

            // If is this server part of a collection of endpoint servers.
            // Should this server control maximum number of clients
            // independent of all other servers within the multi-endpoint.
            if (!_maxNumClientsIndividualControl)
            {
                lock (_lockingObject)
                {
                    // Trigger the server list decrementor.
                    if (OnMultiEndpointServerClientCount != null)
                        OnMultiEndpointServerClientCount(this, IndexAction.Decrement);
                }
            }
        }

        /// <summary>
        /// Decrement the client count.
        /// </summary>
        internal void DecrementClientCount()
        {
            if (_clientCount > 0)
            {
                // Decrement the count.
                Interlocked.Decrement(ref _clientCount);
            }
        }

        /// <summary>
        /// Set the last error that occured.
        /// </summary>
        /// <param name="ex">The exception containing the error.</param>
        private void SetLastError(Exception ex)
        {
            _lastException = ex;
        }

        /// <summary>
        /// Clear the last error that occured.
        /// </summary>
        private void ClearLastError()
        {
            _lastException = null;
        }

        /// <summary>
        /// Send the data to the server context clients.
        /// </summary>
        /// <param name="data">The data to send to each server context client.</param>
        /// <param name="serverContexts">The collection of server context clients.</param>
        /// <param name="isRaw">True if the data contains a command with the data; else false no command (used for streaming).</param>
        private void SendDataToClients(byte[] data, IServerContext[] serverContexts, bool isRaw = false)
        {
            byte[] dataToSend = data;
            IServerContext[] serverContextsToSend = serverContexts;

            try
            {
                // If data exist.
                if (data != null && data.Length > 0)
                {
                    // If context servers exist.
                    if (serverContexts != null && serverContexts.Length > 0)
                    {
                        // Clear last error.
                        ClearLastError();

                        // Do the work in the background
                        var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                        Task.Factory.StartNew(delegate
                        {
                            string error = string.Empty;
                            int output = 0;

                            // Send the data to the clients.
                            error = SendDataToClientsParallel(dataToSend, serverContextsToSend, isRaw);

                            // Pass results to the service
                            return new { error, output };

                        }).ContinueWith(t =>
                        {
                            try
                            {
                                // Trigger the complete event.
                                if (OnSendToClientsComplete != null)
                                    OnSendToClientsComplete(this, new Exception(t.Result.error));
                            }
                            catch (Exception ex)
                            {
                                SetLastError(ex);
                            }

                        }, uiScheduler);
                    }
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Send to each server context the specified data.
        /// </summary>
        /// <param name="data">The data to send to each server context.</param>
        /// <param name="serverContexts">The collection of server context clients.</param>
        /// <param name="isRaw">True if the data contains a command with the data; else false no command (used for streaming).</param>
        /// <returns>The list of error messages if any.</returns>
        private static string SendDataToClientsParallel(byte[] data, IServerContext[] serverContexts, bool isRaw = false)
        {
            StringBuilder errorMessage = new StringBuilder();

            // If conenction exist.
            if (serverContexts != null)
            {
                // If more than zero connections exist.
                if (serverContexts.Count() > 0)
                {
                    // Start a new parallel operation for each connection.
                    Parallel.ForEach<IServerContext>(serverContexts, u =>
                    {
                        try
                        {
                            // Make sure the current object has not be disposed.
                            if (u != null)
                            {
                                if (data != null)
                                {
                                    u.SentFromServer(data);
                                    Exception exception = u.GetLastError();

                                    // If an exception occured.
                                    if (exception != null)
                                        errorMessage.Append(exception.Message + "\r\n");
                                }
                            }
                        }
                        catch { }
                    });
                }
            }

            data = null;
            serverContexts = null;

            // Return the error message if any.
            return errorMessage.ToString();
        }
        #endregion

        #region Dispose Object Methods

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
                    if (_socket != null)
                        _socket.Dispose();

                    if (_connAvailable != null)
                        _connAvailable.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _socket = null;
                _connAvailable = null;
                _hostThread = null;
                _lockingObject = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~ServerActive()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
