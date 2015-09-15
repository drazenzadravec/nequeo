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
    /// General socket single base interface.
    /// </summary>
    public interface ISingleContextBase
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the unique identifier for this connection.
        /// </summary>
        string UniqueIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the current server name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets sets, port to use.
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Gets sets, the common service name.
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// Gets, the number of clients currently connected to the server.
        /// </summary>
        int NumberOfClients { get; }

        #endregion

        #region Public Methods
        /// <summary>
        /// Has the current context timed out.
        /// </summary>
        /// <param name="timeout">The time out (minutes) set for the context; -1 wait indefinitely.</param>
        /// <returns>True if the context has timed out; else false.</returns>
        bool HasTimedOut(int timeout);

        /// <summary>
        /// Send data to the client through the server context from the server.
        /// </summary>
        /// <param name="data">The data received from the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        void SentFromServer(byte[] data);

        /// <summary>
        /// Get the last error that occured.
        /// </summary>
        /// <returns>The last exception.</returns>
        Exception GetLastError();

        #endregion
    }

    /// <summary>
    /// General socket single interface.
    /// </summary>
    public interface ISingleContext : ISingleContextBase
    {
    }

    /// <summary>
    /// General socket single interface.
    /// </summary>
    public interface ISingleContextMux : ISingleContext
    {
        #region Public Properties
        /// <summary>
        /// Gets, the receive or send exception.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Gets, use a secure connection.
        /// </summary>
        bool UseSslConnection { get; }

        /// <summary>
        /// Gets, the x.509 certificate used for a secure connection.
        /// </summary>
        X509Certificate2 X509Certificate { get; }

        /// <summary>
        /// Gets, defines the possible versions of System.Security.Authentication.SslProtocols.
        /// </summary>
        SslProtocols SslProtocols { get; }

        /// <summary>
        /// Gets sets, has a secure negotiation and server authentication 
        /// been established with the client. Also is the data encrypted.
        /// </summary>
        bool IsSslAuthenticated { get; }

        /// <summary>
        /// Gets, the timeout for each client connection when in-active.
        /// </summary>
        int Timeout { get; }

        #endregion

        #region Public Methods
        /// <summary>
        /// Suspends the notifications from the server for the client socket.
        /// </summary>
        void Suspend();

        /// <summary>
        /// Resumes the notifications from the server for the client socket.
        /// </summary>
        void Resume();

        /// <summary>
        /// Close the connection and release all resources.
        /// </summary>
        void Close();

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        void BeginSslNegotiation();

        /// <summary>
        /// Get the client ip endpoint (remote end point).
        /// </summary>
        /// <returns>The client ip endpoint; else null.</returns>
        IPEndPoint GetClientIPEndPoint();

        /// <summary>
        /// Get the server ip endpoint (local end point).
        /// </summary>
        /// <returns>The server ip endpoint; else null.</returns>
        IPEndPoint GetServerIPEndPoint();

        #endregion
    }

    /// <summary>
    /// General socket single implementation.
    /// </summary>
    public class SingleContext : ISingleContextMux, IMultiplexed, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="buffer">The global buffer store.</param>
        internal SingleContext(Nequeo.Collections.StoreBuffer buffer)
        {
            _buffer = buffer;

            // Set the initial timeout time.
            _initialTimeOutTime = DateTime.Now;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SingleContext(System.Net.Sockets.Socket socket)
        {
            _socket = socket;

            // Set the initial timeout time.
            _initialTimeOutTime = DateTime.Now;
        }
        #endregion

        #region Private Fields
        private Nequeo.Collections.StoreBuffer _buffer = null;

        private Timer _timeOut = null;
        private int _timeOutInterval = -1;
        private DateTime _initialTimeOutTime;

        private bool _isListener = false;
        private bool _suspend = false;

        private bool _isSslAuthenticated = false;
        private bool _useSslConnection = false;
        private X509Certificate2 _x509Certificate = null;
        private SslProtocols _sslProtocols = SslProtocols.Tls | SslProtocols.Ssl3;

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

        // Looks for an end connection.
        private Action<bool, SingleContext> _endConnectionCallback = null;
        private Action<SingleContext> _sendToServerInfoHandler = null;
        private Action<Nequeo.Net.Provider.Context> _onReceivedActionHandler = null;

        private Action<byte[]> _receiveHandler = null;
        private Func<byte[]> _sendHandler = null;
        private Exception _exception = null;

        private Nequeo.Collections.CircularBuffer<byte> _requestBuffer = null;
        private Nequeo.Collections.CircularBuffer<byte> _responseBuffer = null;

        private RequestStream _requestStream = null;
        private ResponseStream _responseStream = null;

        // Create the context structures.
        private Request _request = null;
        private Response _response = null;
        private Context _context = null;

        // 0 for false, 1 for true.
        private int _exitWaitReceiveIndicator = 0;
        private int _exitWaitSendIndicator = 0;
        private int _isContextActive = 0;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the global buffer store.
        /// </summary>
        public Nequeo.Collections.StoreBuffer Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
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
        internal Context Context
        {
            get { return _context; }
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
        /// Gets or sets on received action handler, new data has arrived. Should be used when implementing constant data arrivals.
        /// </summary>
        internal Action<Nequeo.Net.Provider.Context> OnReceivedHandler
        {
            get { return _onReceivedActionHandler; }
            set { _onReceivedActionHandler = value; }
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
                    _timeOut = new Timer(ClientTimedOut, null,
                            new TimeSpan(0, 0, _timeOutInterval),
                            new TimeSpan(0, 0, _timeOutInterval));
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
        internal Action<bool, SingleContext> EndConnectionCallback
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
            if (!_isSslAuthenticated && _useSslConnection && _networkStream == null)
            {
                // Block the socket for now.
                _socket.Blocking = true;

                // Initialy assign the network stream
                _networkStream = new NetworkStream(_socket);
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
            // Create the buffers.
            _requestBuffer = new Collections.CircularBuffer<byte>(_requestBufferCapacity);
            _responseBuffer = new Collections.CircularBuffer<byte>(_responseBufferCapacity);

            // Create the streams.
            _requestStream = new RequestStream(_requestBuffer);
            _responseStream = new ResponseStream(_responseBuffer);

            _request = new Request();
            _response = new Response();
            _context = new Context();
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
            _context.Port = _port;
            _context.Name = _serverName;
            _context.ServiceName = _serviceName;
            _context.NumberOfClients = _clientCount;
            _context.SocketState = SocketState.Open;
            _context.IsAsyncMode = false;
            _context.ConnectionID = Guid.NewGuid().ToString();

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
                // Write to the request stream.
                _requestStream.WriteToStream(data, 0, data.Length);

                // Make sure the context exists.
                if (_context != null)
                {
                    // If the data available handler has been set
                    // then send a trigger indicating that 
                    // data is available.
                    if (_context.OnReceivedHandler != null)
                    {
                        // If not in async mode.
                        if (!_context.IsAsyncMode)
                        {
                            // Received the request from the client.
                            // Send the message context.
                            Receiver();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Received the request from the client.
        /// </summary>
        private void Receiver()
        {
            // If a response stream exists.
            if (_requestStream != null)
            {
                try
                {
                    // If the data available handler has been set
                    // then send a trigger indicating that 
                    // data is available.
                    if (_context.OnReceivedHandler != null)
                        _context.OnReceivedHandler(_context);
                }
                catch { }
            }
        }

        /// <summary>
        /// Send the response data to the client.
        /// </summary>
        private void Sender()
        {
            // If in async mode.
            if (_context != null && _context.IsAsyncMode)
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

            // Begin the secure negotiation and server authentication.
            return BeginTlsNegotiation(tlsNegotiationCommand);
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
                if (_useSslConnection)
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
                    // Get the data from the server placed it in the buffer.
                    bytesRead = _socket.Receive(_buffer.ReadBuffer, 0, _buffer.READ_BUFFER_SIZE, SocketFlags.None);

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
                    if (_useSslConnection)
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
                        // Send the command to the server.
                        _socket.Send(data, SocketFlags.None);
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

        /// <summary>
        /// Send data to the host.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        /// <param name="data">The data to send to the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        private void SendSslEx(System.IO.Stream stream, byte[] data)
        {
            // Send the command to the server.
            stream.Write(data, 0, data.Length);
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
            // If not in async mode.
            if (_context != null && !_context.IsAsyncMode)
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
                _timeOut.Change(
                    new TimeSpan(0, 0, _timeOutInterval),
                    new TimeSpan(0, 0, _timeOutInterval));
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

            // Close the connection on time out.
            Close();
        }

        /// <summary>
        /// Determines if a connection to the client exits.
        /// </summary>
        /// <returns>True is connected else false.</returns>
        private bool IsConnected()
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

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        /// <param name="tlsNegotiationCommand">Send a TLS negotiation command (e.g. OK Begin TLS negotiation now) if not null.</param>
        /// <returns>True if authentication has started; else false.</returns>
        /// <remarks>This is generally used for TLS protcol.</remarks>
        private bool BeginTlsNegotiation(string tlsNegotiationCommand = null)
        {
            if (!_isSslAuthenticated && _useSslConnection && _networkStream == null)
            {
                // Block the socket for now.
                _socket.Blocking = true;

                // Initialy assign the network stream
                _networkStream = new NetworkStream(_socket);
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

                    // If not null then send the begin TLS negotiation command.
                    // This is in plain text letting the client know to start
                    // the TLS negotiation.
                    if (!String.IsNullOrEmpty(tlsNegotiationCommand))
                    {
                        // Send the data.
                        byte[] data = Encoding.Default.GetBytes(tlsNegotiationCommand);
                        _responseStream.Write(data, 0, data.Length);
                    }
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
                        _context.Dispose();
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _socket = null;
                _sslStream = null;
                _networkStream = null;
                _x509Certificate = null;
                _endConnectionCallback = null;

                _requestBuffer = null;
                _responseBuffer = null;

                _requestStream = null;
                _responseStream = null;

                _request = null;
                _response = null;

                _context.ContextState = null;
                _context = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SingleContext()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
