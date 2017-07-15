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
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Security.Principal;

using Nequeo.Net.Sockets;
using Nequeo.Handler;
using Nequeo.Extension;
using Nequeo.Net.Configuration;

namespace Nequeo.Net
{
    /// <summary>
    /// Single context.
    /// </summary>
    internal class SingleContextInt : Nequeo.Net.Provider.ISingleContextMux, IMultiplexed, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SingleContextInt()
        {
            // Set the initial timeout time.
            _initialTimeOutTime = DateTime.Now;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="socket">The client socket.</param>
        public SingleContextInt(System.Net.Sockets.Socket socket)
        {
            _socket = socket;

            // Set the initial timeout time.
            _initialTimeOutTime = DateTime.Now;
        }
        #endregion

        #region Private Fields
        private int READ_BUFFER_SIZE = 8192;
        private int WRITE_BUFFER_SIZE = 8192;
        private Nequeo.Collections.StoreBuffer _buffer = null;

        private Timer _timeOut = null;
        private int _timeOutInterval = -1;
        private DateTime _initialTimeOutTime;
        private bool _timeoutTriggered = false;
        private TimeSpan _timeoutSpan;

        private bool _isListener = false;
        private bool _suspend = false;
        private bool _writePoller = false;

        private bool _waitForTlsCommand = false;
        private string _tlsCommand = "STARTTLS";
        private string _tlsAcknowledgeCommand = "200 OK Begin TLS negotiation now\r\n";
        private InterceptItem[] _interceptItems = null;

        private System.Net.Sockets.ProtocolType _protocolType = System.Net.Sockets.ProtocolType.Tcp;
        private System.Net.Sockets.SocketType _socketType = System.Net.Sockets.SocketType.Stream;

        private bool _beginSslAuthentication = true;
        private bool _isSslAuthenticated = false;
        private bool _useSslConnection = false;
        private X509Certificate2 _x509Certificate = null;
        private SslProtocols _sslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls | SslProtocols.Ssl3;
        private SocketFlags _receiveSocketFlags = SocketFlags.None;
        private SocketFlags _sendSocketFlags = SocketFlags.None;

        private System.Net.Sockets.NetworkStream _networkStream = null;
        private System.Net.Security.SslStream _sslStream = null;
        private System.Net.Sockets.Socket _socket = null;

        private int _requestBufferCapacity = 10000000;
        private int _responseBufferCapacity = 10000000;

        private int _port = -1;
        private int _clientCount = 0;
        private string _serviceName = string.Empty;
        private string _serverName = string.Empty;
        private string _uniqueIdentifier = string.Empty;
        private string _connectionID = string.Empty;

        // Looks for an end connection.
        private Action<bool, SingleContextInt> _endConnectionCallback = null;
        private Action<SingleContextInt> _sendToServerInfoHandler = null;
        private Action<Nequeo.Net.Provider.Context> _onReceivedActionHandler = null;

        private Action<byte[]> _receiveHandler = null;
        private Func<byte[]> _sendHandler = null;
        private Exception _exception = null;

        private Nequeo.Collections.CircularBuffer<byte> _requestBuffer = null;
        private Nequeo.Collections.CircularBuffer<byte> _responseBuffer = null;

        private Nequeo.Net.Provider.RequestStream _requestStream = null;
        private Nequeo.Net.Provider.ResponseStream _responseStream = null;

        // Create the context structures.
        private Nequeo.Net.Provider.Request _request = null;
        private Nequeo.Net.Provider.Response _response = null;
        private Nequeo.Net.Provider.Context _context = null;

        // 0 for false, 1 for true.
        private int _exitWaitReceiveIndicator = 0;
        private int _exitWaitSendIndicator = 0;
        private int _isContextActive = 0;

        // Remote client properties.
        private NetClient _remoteClient = null;
        private RemoteServer _remoteServer = null;

        private object _lockSendObject = new object();
        private object _lockReceiveObject = new object();
        private object _connectionLock = new object();

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the intercept command items.
        /// </summary>
        public InterceptItem[] InterceptItems
        {
            get { return _interceptItems; }
            set { _interceptItems = value; }
        }

        /// <summary>
        /// Gets or sets the remote server.
        /// </summary>
        public RemoteServer RemoteServer
        {
            get { return _remoteServer; }
            set { _remoteServer = value; }
        }

        /// <summary>
        /// Gets sets, should this server wait for and negotiate a TLS command.
        /// </summary>
        public bool WaitForTlsCommand
        {
            get { return _waitForTlsCommand; }
            set { _waitForTlsCommand = value; }
        }

        /// <summary>
        /// Gets sets, the TLS command (STARTTLS).
        /// </summary>
        public string TlsCommand
        {
            get { return _tlsCommand; }
            set { _tlsCommand = value; }
        }

        /// <summary>
        /// Gets sets, the TLS acknowledgment command (200 OK Begin TLS negotiation now).
        /// </summary>
        public string TlsAcknowledgeCommand
        {
            get { return _tlsAcknowledgeCommand; }
            set { _tlsAcknowledgeCommand = value; }
        }

        /// <summary>
        /// Gets or sets the global buffer store.
        /// </summary>
        private Nequeo.Collections.StoreBuffer Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
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
        /// Gets sets, suspends the notifications from the socket.
        /// </summary>
        bool IMultiplexed.Suspend
        {
            get { return _suspend; }
            set { _suspend = value; }
        }

        /// <summary>
        /// Gets or sets, is the current mux a listener; else false.
        /// </summary>
        bool IMultiplexed.IsListener
        {
            get { return _isListener; }
            set { _isListener = value; }
        }

        /// <summary>
        /// Gets the current net context.
        /// </summary>
        internal Nequeo.Net.Provider.Context Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets sets, the handler to the server receive only information internally.
        /// </summary>
        internal Action<SingleContextInt> SendToServerInfoHandler
        {
            get { return _sendToServerInfoHandler; }
            set { _sendToServerInfoHandler = value; }
        }

        /// <summary>
        /// Gets or sets on received action handler, new data has arrived. Should be used when implementing constant data arrivals.
        /// </summary>
        internal Action<Nequeo.Net.Provider.Context> OnReceivedHandler
        {
            get { return _onReceivedActionHandler; }
            set { _onReceivedActionHandler = value; }
        }

        /// <summary>
        /// Gets or sets, true if context write poller is used; else false. Used with Multiplexer.PollWriter,
        /// if Multiplexer.PollWriter is false then this property is set to true.
        /// </summary>
        public bool WritePoller
        {
            get { return _writePoller; }
            set { _writePoller = value; }
        }

        /// <summary>
        /// Gets sets, the unique identifier for this connection.
        /// </summary>
        public string UniqueIdentifier
        {
            get { return _uniqueIdentifier; }
            set { _uniqueIdentifier = value; }
        }

        /// <summary>
        /// Gets sets, the receive handler.
        /// </summary>
        public Action<byte[]> ReceiveHandler
        {
            get { return _receiveHandler; }
            set { _receiveHandler = value; }
        }

        /// <summary>
        /// Gets sets, the send handler.
        /// </summary>
        public Func<byte[]> SendHandler
        {
            get { return _sendHandler; }
            set { _sendHandler = value; }
        }

        /// <summary>
        /// Gets, the receive or send exception.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Gets sets, has a secure negotiation and server authentication 
        /// been established with the client. Also is the data encrypted.
        /// </summary>
        public bool IsSslAuthenticated
        {
            get { return _isSslAuthenticated; }
        }

        /// <summary>
        /// Gets, should the server authenticate with the client and start a secure connection.
        /// </summary>
        /// <remarks>True if authentication begins within the Connect method; else false.
        /// When the Connect method is called and a secure connection has been chosen
        /// then this property is call asking if a secure connection is to be established immediatly.
        /// This is generally used for SSL protocol.</remarks>
        public bool BeginSslAuthentication
        {
            get { return _beginSslAuthentication; }
            internal set { _beginSslAuthentication = value; }
        }

        /// <summary>
        /// Gets, use a secure connection.
        /// </summary>
        public bool UseSslConnection
        {
            get { return _useSslConnection; }
            internal set { _useSslConnection = value; }
        }

        /// <summary>
        /// Gets, the x.509 certificate used for a secure connection.
        /// </summary>
        public X509Certificate2 X509Certificate
        {
            get { return _x509Certificate; }
            internal set { _x509Certificate = value; }
        }

        /// <summary>
        /// Gets, defines the possible versions of System.Security.Authentication.SslProtocols.
        /// </summary>
        public SslProtocols SslProtocols
        {
            get { return _sslProtocols; }
            internal set { _sslProtocols = value; }
        }

        /// <summary>
        /// Gets, the protocol type.
        /// </summary>
        public System.Net.Sockets.ProtocolType ProtocolType
        {
            get { return _protocolType; }
            internal set { _protocolType = value; }
        }

        /// <summary>
        /// Gets, the socket type.
        /// </summary>
        public System.Net.Sockets.SocketType SocketType
        {
            get { return _socketType; }
            internal set { _socketType = value; }
        }

        /// <summary>
        /// Gets, specifies socket receive behaviors.
        /// </summary>
        public SocketFlags ReceiveSocketFlags
        {
            get { return _receiveSocketFlags; }
            internal set { _receiveSocketFlags = value; }
        }

        /// <summary>
        /// Gets, specifies socket send behaviors.
        /// </summary>
        public SocketFlags SendSocketFlags
        {
            get { return _sendSocketFlags; }
            internal set { _sendSocketFlags = value; }
        }

        /// <summary>
        /// Gets sets, the timeout (seconds) for each client connection when in-active.
        /// Disconnects the client when this time out is triggered.
        /// </summary>
        public int Timeout
        {
            get { return _timeOutInterval; }
            set
            {
                _timeOutInterval = value;
                if (_timeOutInterval > -1)
                {
                    _timeoutSpan = new TimeSpan(0, 0, _timeOutInterval);
                    _timeOut = new Timer(ClientTimedOut, null, _timeoutSpan, _timeoutSpan);
                }
                else
                {
                    if (_timeOut != null)
                        _timeOut.Dispose();

                    _timeOut = null;
                }
            }
        }

        /// <summary>
        /// Gets the socket client.
        /// </summary>
        public Socket Socket
        {
            get { return _socket; }
            internal set { _socket = value; }
        }

        /// <summary>
        /// Gets sets, the ssl stream.
        /// </summary>
        public System.Net.Security.SslStream SslStream
        {
            get { return _sslStream; }
            set { _sslStream = value; }
        }

        /// <summary>
        /// Gets sets, the network stream.
        /// </summary>
        public System.Net.Sockets.NetworkStream NetworkStream
        {
            get { return _networkStream; }
            set { _networkStream = value; }
        }

        /// <summary>
        /// Gets sets, the maximum request buffer capacity when using buffered stream.
        /// </summary>
        public int RequestBufferCapacity
        {
            get { return _requestBufferCapacity; }
            set { _requestBufferCapacity = value; }
        }

        /// <summary>
        /// Gets sets, the maximum response buffer capacity when using buffered stream.
        /// </summary>
        public int ResponseBufferCapacity
        {
            get { return _responseBufferCapacity; }
            set { _responseBufferCapacity = value; }
        }

        /// <summary>
        /// Gets, the current server name.
        /// </summary>
        public string Name
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        /// <summary>
        /// Gets sets, looks for an end connection.
        /// </summary>
        internal Action<bool, SingleContextInt> EndConnectionCallback
        {
            get { return _endConnectionCallback; }
            set { _endConnectionCallback = value; }
        }

        /// <summary>
        /// Gets sets, port to use.
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Gets sets, the common service name.
        /// </summary>
        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; }
        }

        /// <summary>
        /// Gets, the number of clients currently connected to the server.
        /// </summary>
        public int NumberOfClients
        {
            get { return _clientCount; }
            internal set { _clientCount = value; }
        }

        /// <summary>
        /// Gets the current unique connection identifier.
        /// </summary>
        public string ConnectionID
        {
            get { return _connectionID; }
            internal set { _connectionID = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Close the connection and release all resources.
        /// </summary>
        public virtual void Close()
        {
            try
            {
                // Signal to the blocking handler
                // to un-block.
                if (_endConnectionCallback != null)
                    _endConnectionCallback(true, this);
            }
            catch { }

            Dispose();
        }

        /// <summary>
        /// Get the last error that occured.
        /// </summary>
        /// <returns>The last exception.</returns>
        public virtual Exception GetLastError()
        {
            return _exception;
        }

        /// <summary>
        /// Send data to the client through the server context from the server.
        /// </summary>
        /// <param name="data">The data received from the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void SentFromServer(byte[] data)
        {
            // if the response stream exists.
            if (_responseStream != null)
            {
                // Write the new data to the response stream.
                _responseStream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Has the current context timed out.
        /// </summary>
        /// <param name="timeout">The time out (minutes) set for the context; -1 wait indefinitely.</param>
        /// <returns>True if the context has timed out; else false.</returns>
        public virtual bool HasTimedOut(int timeout)
        {
            bool ret = false;

            // If a timeout has been set.
            if (timeout > -1)
            {
                // Get the current time, subtract the initial time from the current time
                // to get the difference and assign the time span from the timeout.
                DateTime now = DateTime.Now;
                TimeSpan lapsed = now.Subtract(_initialTimeOutTime);
                TimeSpan timeoutTime = new TimeSpan(0, timeout, 0);

                // If the lapsed time is greater than then timeout span
                // than the timeout has been reached.
                if (lapsed.TotalMilliseconds >= timeoutTime.TotalMilliseconds)
                    ret = true;
            }

            // Return true if context has times out; else false.
            return ret;
        }

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.Security.Authentication.AuthenticationException"></exception>
        public virtual void BeginSslNegotiation()
        {
            // Using ssl connection
            if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
            {
                if (!_isSslAuthenticated && _useSslConnection)
                {
                    // Block the socket for now.
                    _socket.Blocking = true;
                }

                try
                {
                    // If not authenticated.
                    if (!_isSslAuthenticated)
                    {
                        // If ssl certificate has not been assigned.
                        if (_x509Certificate == null)
                            throw new Exception("Please add an SSL certificate for secure connections.");

                        // Get the current ssl stream
                        // from the socket.
                        _sslStream = new SslStream(_networkStream);

                        // Load the certificate into the
                        // secure stream used for secure communication.
                        _sslStream.AuthenticateAsServer(_x509Certificate, false, _sslProtocols, false);

                        // Get the state of the authentication.
                        if (_sslStream.IsAuthenticated && _sslStream.IsEncrypted)
                            _isSslAuthenticated = true;
                        else
                            _isSslAuthenticated = false;
                    }
                }
                catch (System.Security.Authentication.AuthenticationException)
                {
                    if (_sslStream != null)
                        _sslStream.Dispose();

                    throw;
                }
            }
        }

        /// <summary>
        /// Get the client ip endpoint (remote end point).
        /// </summary>
        /// <returns>The client ip endpoint; else null.</returns>
        public virtual IPEndPoint GetClientIPEndPoint()
        {
            return ((IPEndPoint)_socket.RemoteEndPoint);
        }

        /// <summary>
        /// Get the server ip endpoint (local end point).
        /// </summary>
        /// <returns>The server ip endpoint; else null.</returns>
        public virtual IPEndPoint GetServerIPEndPoint()
        {
            return ((IPEndPoint)_socket.LocalEndPoint);
        }

        /// <summary>
        /// Suspends the notifications from the server for the client socket.
        /// </summary>
        public virtual void Suspend()
        {
            _suspend = true;
        }

        /// <summary>
        /// Resumes the notifications from the server for the client socket.
        /// </summary>
        public virtual void Resume()
        {
            _suspend = false;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Create and assign each component.
        /// </summary>
        internal void Initialise()
        {
            // Create a new context for a new connection.
            CreateContext();

            // Assign context values.
            AssignContext();
        }

        /// <summary>
        /// Create a new context for a new connection.
        /// </summary>
        private void CreateContext()
        {
            // Create a new buffer.
            _buffer = new Nequeo.Collections.StoreBuffer(READ_BUFFER_SIZE, WRITE_BUFFER_SIZE);

            // Create the buffers.
            _requestBuffer = new Collections.CircularBuffer<byte>(_requestBufferCapacity);
            _responseBuffer = new Collections.CircularBuffer<byte>(_responseBufferCapacity);

            // Create the streams.
            _requestStream = new Nequeo.Net.Provider.RequestStream(_requestBuffer);
            _responseStream = new Nequeo.Net.Provider.ResponseStream(_responseBuffer);

            _request = new Nequeo.Net.Provider.Request();
            _response = new Nequeo.Net.Provider.Response();
            _context = new Nequeo.Net.Provider.Context();

            // Using ssl connection
            if (_useSslConnection)
            {
                // Block the socket for now.
                _socket.Blocking = true;

                // If socket type is streaming then
                // create a network stream handler.
                if (_socketType == System.Net.Sockets.SocketType.Stream)
                {
                    // Initially assign the network stream.
                    _networkStream = new NetworkStream(_socket);
                }
            }
        }

        /// <summary>
        /// Assign context values.
        /// </summary>
        private void AssignContext()
        {
            // Assign the response read hander.
            _responseStream.ReadStreamActionHandler = () => Sender();
            _responseStream.CloseActionHandler = () => CloseContext();

            _context.UniqueIdentifierHasBeenSet = (string uid) => UniqueIdentifierHasBeenSet(uid);
            _context.StartTls = (string command) => StartTls(command);
            _context.CloseActionHandler = () => CloseContext();

            // Assign the request streams.
            _request.Input = _requestStream;

            // Assign the response streams.
            _response.Output = _responseStream;

            // Assign the context.
            _context.RequestStream = _requestStream;
            _context.ResponseStream = _responseStream;
            _context.RemoteEndPoint = GetClientIPEndPoint();
            _context.ServerEndPoint = GetServerIPEndPoint();
            _context.IsSecureConnection = _useSslConnection;
            _context.IsSslAuthenticated = _isSslAuthenticated;
            _context.Port = _port;
            _context.Name = _serverName;
            _context.ServiceName = _serviceName;
            _context.NumberOfClients = _clientCount;
            _context.SocketState = SocketState.Open;
            _context.IsAsyncMode = false;
            _context.ConnectionID = Guid.NewGuid().ToString();

            // Get the session id from the IP Address and port.
            IPEndPoint ip = _context.RemoteEndPoint;
            string ipAddress = ip.Address.ToString();
            string ipPort = ip.Port.ToString();
            _context.SessionID = Nequeo.Cryptography.Hashcode.GetHashcodeSHA1(ipAddress + "_" + ipPort);

            // Set the current connection id.
            _connectionID = _context.ConnectionID;

            // Assign the structures to the context.
            _context.ContextState = null;
            _context.Request = _request;
            _context.Response = _response;

            // Assign the receive handler.
            if (_onReceivedActionHandler != null)
            {
                // Assign the server recevice handler.
                _context.OnReceivedHandler = _onReceivedActionHandler;
            }
            else if (_sendToServerInfoHandler != null)
            {
                // Assign the provider server socket
                // receive handler.
                _sendToServerInfoHandler(this);
            }

            // Assign the data receive handler.
            _receiveHandler = (data) => Receive(data);
            _sendHandler = () => SendData();

            // If not waiting for a TLS command.
            if (!_waitForTlsCommand)
            {
                // Start the SSL negotiations.
                if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                {
                    // Begin ssl authentication immediatly.
                    if (_beginSslAuthentication)
                    {
                        // Begin the secure negotiation and server authentication.
                        BeginSslNegotiation();
                    }
                }
            }
        }

        /// <summary>
        /// The receive data method call.
        /// </summary>
        /// <param name="data">The data received from the client.</param>
        private void Receive(byte[] data)
        {
            // Make sure data has arrived.
            if (data != null && data.Length > 0)
            {
                // Make sure the context exists.
                if (_context != null)
                {
                    try
                    {
                        // If the client is not connected.
                        if (_remoteClient == null)
                        {
                            // Not connected, make a connection.
                            _remoteClient = new NetClient(_remoteServer.Host, _remoteServer.Port);
                            _remoteClient.OnNetContext += _remoteClient_OnNetContext;
                            _remoteClient.OnDisconnected += _remoteClient_OnDisconnected;
                            _remoteClient.UseSslConnection = _remoteServer.Secure;
                            _remoteClient.Connect();
                        }

                        // If connected.
                        if (_remoteClient.Connected)
                        {
                            // Send the data to the remote server.
                            SendDataToRemoteServer(data);
                        }
                        else
                        {
                            // If not connected
                            _remoteClient.Connect();

                            // If connected.
                            if (_remoteClient.Connected)
                            {
                                // Send the data to the remote server.
                                SendDataToRemoteServer(data);
                            }
                            else
                                throw new Exception();
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Send the data to the remote server.
        /// </summary>
        /// <param name="data">The data received from the client.</param>
        private void SendDataToRemoteServer(byte[] data)
        {
            // If not waiting for a TLS command.
            if (!_waitForTlsCommand)
            {
                // Send the data to the remote server.
                ProcessInterceptItems(data);
            }
            else
            {
                // If the TLS authenticated has not been completed.
                if (!_isSslAuthenticated)
                {
                    // Waiting for a TLS comamnd.
                    if ((data.Length > _tlsCommand.Length - 1) &&
                        ((data.Length < _tlsCommand.Length + 1) || (data.Length < _tlsCommand.Length + 2) || (data.Length < _tlsCommand.Length + 3)))
                    {
                        // Get the command bytes.
                        string startTLS = Encoding.Default.GetString(data, 0, _tlsCommand.Length);
                        if (startTLS.ToUpper().Equals(_tlsCommand.ToUpper()))
                        {
                            try
                            {
                                // Begin the TLS negotiation and server authentication.
                                StartTls(_tlsAcknowledgeCommand);
                            }
                            catch { }
                            return;
                        }
                    }
                }

                // Send the data to the remote server.
                ProcessInterceptItems(data);
            }
        }

        /// <summary>
        /// Process any intercept items.
        /// </summary>
        /// <param name="data">The data received from the client.</param>
        private void ProcessInterceptItems(byte[] data)
        {
            // If intercept items exist.
            if (_interceptItems != null && _interceptItems.Length > 0)
            {
                bool foundCommand = false;
                string response = null;

                // For each item process the command.
                for(int i = 0; i < _interceptItems.Length; i++)
                {
                    // Get the item.
                    InterceptItem item = _interceptItems[i];
                    response = item.Response;

                    // Find the current command.
                    if ((data.Length > item.Command.Length - 1) &&
                        ((data.Length < item.Command.Length + 1) || (data.Length < item.Command.Length + 2) || (data.Length < item.Command.Length + 3)))
                    {
                        // Get the command bytes.
                        string command = Encoding.Default.GetString(data, 0, item.Command.Length);
                        if (command.ToUpper().Equals(item.Command.ToUpper()))
                        {
                            // Found an intecept command.
                            foundCommand = true;
                            break;
                        }
                    }
                }

                // If no command has been found.
                if (!foundCommand)
                {
                    // Send the data to the remote server.
                    _remoteClient.Send(data);
                }
                else
                {
                    // If a response exists.
                    if (!String.IsNullOrEmpty(response))
                    {
                        // Send the response to the client.
                        byte[] buffer = Encoding.Default.GetBytes(response);

                        // Write the data to the response stream.
                        _responseStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            else
            {
                // Send the data to the remote server.
                _remoteClient.Send(data);
            }
        }

        /// <summary>
        /// On client disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _remoteClient_OnDisconnected(object sender, EventArgs e)
        {
            // If the remote client has been disconnected then
            // close all the conenctions.
            Close();
        }

        /// <summary>
        /// On net context.
        /// </summary>
        /// <param name="sender">The client that sent the context.</param>
        /// <param name="context">The current connection context.</param>
        private void _remoteClient_OnNetContext(object sender, NetContext context)
        {
            try
            {
                // Read all the bytes in the buffer.
                int count = (int)context.NetResponse.Input.Length;
                byte[] buffer = new byte[count];
                context.NetResponse.Read(buffer, 0, count);

                if (buffer != null)
                    // Write the data to the response stream.
                    _responseStream.Write(buffer, 0, count);
                
                buffer = null;
            }
            catch { }
        }

        /// <summary>
        /// Send the response data to the client.
        /// </summary>
        private void Sender()
        {
            // If the current internal writer if on
            // then not using the multiplexer write poller.
            if (_writePoller)
            {
                // Poll the socket when data can be sent.
                if (_socket != null && _socket.Connected)
                {
                    // If read to sent.
                    bool polled = false;

                    try
                    {
                        // Send until empty.
                        while (_responseStream != null && _responseStream.Length > 0)
                        {
                            // If read to sent.
                            polled = _socket.Poll(10, SelectMode.SelectWrite);

                            // If read to send.
                            if (polled)
                            {
                                // Send the data.
                                SocketSend(SendData());
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Send the data to the client.
        /// </summary>
        /// <returns>Return the data that needs to be sent.</returns>
        private byte[] SendData()
        {
            // Get the data from the response stream.
            int byesRead = _responseStream.ReadFromStream(_buffer.WriteBuffer, 0, _buffer.WRITE_BUFFER_SIZE);

            // Send to client if bytes have been read.
            if (byesRead > 0)
                return _buffer.WriteBuffer.Take(byesRead).ToArray();
            else
                return null;
        }

        /// <summary>
        /// Close the connection and release all resources used for communication.
        /// </summary>
        /// <remarks>Call from the context.</remarks>
        private void CloseContext()
        {
            // Set the web socket state to closed.
            if (_context != null)
                _context.SocketState = SocketState.Closed;

            // Disconnect the current client and releases all resources.
            Close();
        }

        /// <summary>
        /// The unique itentifier has been set.
        /// </summary>
        /// <param name="uid">The unique identifier.</param>
        private void UniqueIdentifierHasBeenSet(string uid)
        {
            // Set the unique identifier.
            _uniqueIdentifier = uid;
        }

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        /// <param name="tlsNegotiationCommand">Send a TLS negotiation command (e.g. OK Begin TLS negotiation now) if not null.</param>
        /// <returns>True if authentication has started; else false.</returns>
        /// <remarks>This is generally used for TLS protcol.</remarks>
        private bool StartTls(string tlsNegotiationCommand = null)
        {
            _useSslConnection = true;
            if (_context != null)
                _context.IsSecureConnection = _useSslConnection;

            // Using ssl connection
            if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
            {
                // Begin the secure negotiation and server authentication.
                return BeginTlsNegotiation(tlsNegotiationCommand);
            }
            else
            {
                // Do not allow secure connection.
                return false;
            }
        }

        /// <summary>
        /// Receive data from the remote host.
        /// </summary>
        /// <returns>The data received; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        private byte[] SocketReceiver()
        {
            bool isConnected = false;
            byte[] data = null;
            int bytesRead = 0;

            try
            {
                // Using ssl connection
                if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                {
                    // If not authenticated.
                    if (!_isSslAuthenticated)
                    {
                        data = ReceiveSsl(_networkStream);
                    }
                    else
                    {
                        data = ReceiveSsl(_sslStream);
                    }
                }
                else
                {
                    // Lock the receiver.
                    lock (_lockReceiveObject)
                    {
                        // Get the data from the server placed it in the buffer.
                        bytesRead = _socket.Receive(_buffer.ReadBuffer, 0, _buffer.READ_BUFFER_SIZE, _receiveSocketFlags);
                    }

                    // Data exists.
                    if (bytesRead > 0)
                    {
                        // Decode the data in the buffer to a string.
                        data = _buffer.ReadBuffer.Take(bytesRead).ToArray();

                        // If the time out control has been created
                        // then reset the timer.
                        InActiveTimeOutSetter();
                    }
                    else
                        data = null;
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
            }

            try
            {
                // If no data has been sent then
                // assume the connection has been closed.
                if (data != null)
                    isConnected = IsConnected();
                else
                    isConnected = false;

            }
            catch { }

            // If data is null then assume the connection has closed.
            if (!isConnected || data == null)
                Close();

            // Return the data.
            return data;
        }

        /// <summary>
        /// Send data to the remote host.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        private void SocketSend(byte[] data)
        {
            // If data exists.
            if (data != null && data.Length > 0)
            {
                bool isConnected = false;

                try
                {
                    if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                    {
                        // If not authenticated.
                        if (!_isSslAuthenticated)
                        {
                            SendSslEx(_networkStream, data);
                        }
                        else
                        {
                            SendSslEx(_sslStream, data);
                        }
                    }
                    else
                    {
                        // Lock the send.
                        lock (_lockSendObject)
                        {
                            // Send the command to the server.
                            _socket.Send(data, _sendSocketFlags);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }

                try
                {
                    // If the time out control has been created
                    // then reset the timer.
                    InActiveTimeOutSetter();

                    // If no data has been sent then
                    // assume the connection has been closed.
                    if (data != null)
                        isConnected = IsConnected();
                }
                catch { }

                // If data is null then assume the connection has closed.
                if (!isConnected)
                    Close();
            }
        }

        /// <summary>
        /// Receive data from the host.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        /// <returns>The data received; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        private byte[] ReceiveSsl(System.IO.Stream stream)
        {
            int bytesRead = 0;

            // Lock the receiver.
            lock (_lockReceiveObject)
            {
                // Get the data from the server placed it in the buffer.
                bytesRead = stream.Read(_buffer.ReadBuffer, 0, _buffer.READ_BUFFER_SIZE);

                // Data exists.
                if (bytesRead > 0)
                {
                    // If the time out control has been created
                    // then reset the timer.
                    InActiveTimeOutSetter();

                    // Decode the data in the buffer to a string.
                    return _buffer.ReadBuffer.Take(bytesRead).ToArray();
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Send data to the host.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        /// <param name="data">The data to send to the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        private void SendSslEx(System.IO.Stream stream, byte[] data)
        {
            // Lock the send data.
            lock (_lockSendObject)
            {
                // Send the command to the server.
                stream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Called when socket has some data to read.
        /// </summary>
        void IMultiplexed.ReadyRead()
        {
            // If data is available.
            if (_socket != null && _socket.Connected)
            {
                if (_receiveHandler != null)
                {
                    // Receive new data from the client.
                    _receiveHandler(SocketReceiver());
                }
            }
        }

        /// <summary>
        /// Called when the socket is able to send data.
        /// </summary>
        /// <returns>True if data exists to write: else false.</returns>
        bool IMultiplexed.ReadyWrite()
        {
            // If the current internal writer if off
            // then using the multiplexer write poller.
            if (!_writePoller)
            {
                // Data exists to send.
                if (_responseStream != null && _responseStream.Length > 0)
                {
                    if (_sendHandler != null)
                    {
                        // Poll the socket when data can be sent.
                        if (_socket != null && _socket.Connected)
                        {
                            // If read to sent.
                            bool polled = false;

                            try
                            {
                                // If read to sent.
                                polled = _socket.Poll(10, SelectMode.SelectWrite);
                            }
                            catch { }

                            // If read to send.
                            if (polled)
                            {
                                // Get the data to send.
                                SocketSend(_sendHandler());
                            }
                        }
                    }

                    // If the time out has been triggered.
                    if (_timeoutTriggered)
                        return false;

                    // If more data exists after sending.
                    if (_responseStream.Length > 0)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Called when the socket goes to error state.
        /// </summary>
        void IMultiplexed.ErrorState()
        {
            Close();
        }

        /// <summary>
        /// Close the connection and release all resources.
        /// </summary>
        private void CloseEx()
        {
            try
            {
                if (_socket != null)
                    _socket.Shutdown(SocketShutdown.Both);
            }
            catch { }

            try
            {
                if (_socket != null)
                    _socket.Disconnect(false);
            }
            catch { }

            try
            {
                if (_socket != null)
                    _socket.Close();

                if (_sslStream != null)
                    _sslStream.Close();

                if (_networkStream != null)
                    _networkStream.Close();
            }
            catch { }
        }

        /// <summary>
        /// In-active timeout setter.
        /// </summary>
        private void InActiveTimeOutSetter()
        {
            // Reset the initial timeout time.
            _initialTimeOutTime = DateTime.Now;

            // If the time out control has been created
            // then reset the timer.
            if (_timeOut != null)
                _timeOut.Change(_timeoutSpan, _timeoutSpan);
        }

        /// <summary>
        /// Disconnects the current client after the time out
        /// interval has elapsed.
        /// </summary>
        /// <param name="state">A passed object state.</param>
        private void ClientTimedOut(object state)
        {
            try
            {
                if (_timeOut != null)
                    _timeOut.Dispose();
            }
            catch (Exception ex)
            {
                _exception = ex;
            }

            // Time out has been triggered.
            _timeoutTriggered = true;

            // Close the connection on time out.
            Close();
        }

        /// <summary>
        /// Determines if a connection to the client exits.
        /// </summary>
        /// <returns>True is connected else false.</returns>
        private bool IsConnected()
        {
            // Lock connection state.
            lock (_connectionLock)
            {
                bool isConnected = false;

                // Get the current socket state
                bool blockingState = true;

                try
                {
                    byte[] tmp = new byte[1];

                    if (_socket != null)
                    {
                        blockingState = _socket.Blocking;
                        _socket.Blocking = false;
                        _socket.Send(tmp, 0, 0);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (ex is SocketException)
                        {
                            SocketException socketExc = (SocketException)ex;
                            // 10035 == WAEWOULDBLOCK 
                            if (socketExc.NativeErrorCode.Equals(10035))
                                isConnected = true;
                            else
                                isConnected = false;
                        }
                    }
                    catch { }
                }
                finally
                {
                    try
                    {
                        if (_socket != null)
                            _socket.Blocking = blockingState;
                    }
                    catch { }
                }

                try
                {
                    // If not connected then do another check.
                    if (!isConnected)
                    {
                        if (_socket != null)
                            isConnected = _socket.Connected;
                    }
                }
                catch { }

                return isConnected;
            }
        }

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        /// <param name="tlsNegotiationCommand">Send a TLS negotiation command (e.g. 220 OK Begin TLS negotiation now) if not null.</param>
        /// <returns>True if authentication has started; else false.</returns>
        /// <remarks>This is generally used for TLS protcol.</remarks>
        private bool BeginTlsNegotiation(string tlsNegotiationCommand = null)
        {
            if (!_isSslAuthenticated && _useSslConnection)
            {
                // Block the socket for now.
                _socket.Blocking = true;
            }

            try
            {
                // If not authenticated.
                if (!_isSslAuthenticated)
                {
                    // If ssl certificate has not been assigned.
                    if (_x509Certificate == null)
                        throw new Exception("Please add an SSL certificate for secure connections.");

                    // If not null then send the begin TLS negotiation command.
                    // This is in plain text letting the client know to start
                    // the TLS negotiation.
                    if (!String.IsNullOrEmpty(tlsNegotiationCommand))
                    {
                        // Send the data.
                        byte[] data = Encoding.Default.GetBytes(tlsNegotiationCommand);
                        _responseStream.Write(data, 0, data.Length);
                    }

                    // Get the current ssl stream
                    // from the socket.
                    _sslStream = new SslStream(_networkStream);

                    // Load the certificate into the
                    // secure stream used for secure communication.
                    _sslStream.AuthenticateAsServer(_x509Certificate, false, _sslProtocols, false);

                    // Get the state of the authentication.
                    if (_sslStream.IsAuthenticated && _sslStream.IsEncrypted)
                        _isSslAuthenticated = true;
                    else
                        _isSslAuthenticated = false;
                }
            }
            catch (System.Security.Authentication.AuthenticationException)
            {
                if (_sslStream != null)
                    _sslStream.Dispose();

                throw;
            }

            // Return the result.
            return _isSslAuthenticated;
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
                // Set the web socket state to closed.
                if (_context != null)
                    _context.SocketState = SocketState.Closed;

                // Release the receive and send spin wait handler.
                Interlocked.Exchange(ref _exitWaitReceiveIndicator, 0);
                Interlocked.Exchange(ref _exitWaitSendIndicator, 0);
                Interlocked.Exchange(ref _isContextActive, 0);

                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Close the connection.
                    CloseEx();

                    if (_socket != null)
                        _socket.Dispose();

                    if (_sslStream != null)
                        _sslStream.Dispose();

                    if (_networkStream != null)
                        _networkStream.Dispose();

                    if (_requestBuffer != null)
                        _requestBuffer.Dispose();

                    if (_responseBuffer != null)
                        _responseBuffer.Dispose();

                    if (_requestStream != null)
                        _requestStream.Dispose();

                    if (_responseStream != null)
                        _responseStream.Dispose();

                    if (_buffer != null)
                        _buffer.Dispose();

                    if (_context != null)
                    {
                        if (_context.ContextState != null)
                        {
                            // If the current state context
                            // implements IDisposable then
                            // dispose of the resources.
                            if (_context.ContextState is IDisposable)
                            {
                                IDisposable disposable = (IDisposable)_context.ContextState;
                                disposable.Dispose();
                            }
                        }

                        if (_context.State != null)
                        {
                            // If the current state context
                            // implements IDisposable then
                            // dispose of the resources.
                            if (_context.State is IDisposable)
                            {
                                IDisposable disposable = (IDisposable)_context.State;
                                disposable.Dispose();
                            }
                        }
                        _context.Dispose();
                    }

                    if (_remoteClient != null)
                        _remoteClient.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _socket = null;
                _sslStream = null;
                _networkStream = null;
                _x509Certificate = null;
                _endConnectionCallback = null;
                _buffer = null;

                _requestBuffer = null;
                _responseBuffer = null;

                _requestStream = null;
                _responseStream = null;

                _request = null;
                _response = null;

                _context.State = null;
                _context.ContextState = null;
                _context = null;
                _remoteClient = null;

                _lockSendObject = null;
                _lockReceiveObject = null;
                _connectionLock = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SingleContextInt()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
