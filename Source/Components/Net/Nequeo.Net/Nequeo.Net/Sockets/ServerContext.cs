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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
    /// Delegate that handles commands received from the client.
    /// </summary>
    /// <param name="sender">The current object sending the data.</param>
    /// <param name="commandSend">The command data send by the client.</param>
    internal delegate void ServerContextHandler(ServerContext sender, string commandSend);

    /// <summary>
    /// General socket server context provider used for server client connectivity implementation.
    /// </summary>
    public abstract class ServerContext : IServerContext, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ServerContext()
        {
            _readBuffer = new byte[READ_BUFFER_SIZE];
            _writeBuffer = new byte[WRITE_BUFFER_SIZE];
        }
        #endregion

        #region Private Fields
        private int READ_BUFFER_SIZE = 8192;
        private int WRITE_BUFFER_SIZE = 8192;

        private object _sendLock = new object();
        private object _readLock = new object();
        private object _connectionLock = new object();

        private string _uniqueIdentifier = string.Empty;
        private string _connectionID = string.Empty;
        private IServer _socketServer = null;
        private Exception _lastException = null;

        private string _serverName = string.Empty;
        private bool _isInValidationMode = false;
        private object _state = null;
        
        private System.Net.Sockets.NetworkStream _networkStream = null;
        private System.Net.Security.SslStream _sslStream = null;
        private System.Net.Sockets.Socket _socket = null;

        private System.Net.Sockets.ProtocolType _protocolType = System.Net.Sockets.ProtocolType.Tcp;
        private System.Net.Sockets.SocketType _socketType = System.Net.Sockets.SocketType.Stream;

        private bool _beginSslAuthentication = true;
        private bool _isSslAuthenticated = false;
        private bool _useSslConnection = false;
        private X509Certificate2 _sslCertificate = null;
        private SslProtocols _sslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls | SslProtocols.Ssl3;
        private SocketFlags _receiveSocketFlags = SocketFlags.None;
        private SocketFlags _sendSocketFlags = SocketFlags.None;

        private byte[] _readBuffer = null;
        private byte[] _writeBuffer = null;
        private System.IO.Stream _receiveStream = null;
        private System.IO.Stream _sendStream = null;
        private IAsyncResult _arReceiver = null;
        private IAsyncResult _arSender = null;
        private bool _receiverStopped = false;
        private Action<byte[]> _receiverHandler = null;
        private Action<IServer, IServerContext, byte[]> _sendToServerHandler = null;
        private Action<IServer, IServerContext, byte[]> _sendToServerInfoHandler = null;
        private Nequeo.Net.SocketServerActionType _socketActionType = SocketServerActionType.Method;

        private int _timeoutSend = -1;
        private int _timeoutReceive = -1;
        private int _dataReceiverBytesRead = 0;

        private Timer _timeOut = null;
        private int _timeOutInterval = -1;
        private DateTime _initialTimeOutTime;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the read buffer size.
        /// </summary>
        public int ReadBufferSize
        {
            get { return READ_BUFFER_SIZE; }
            set
            {
                _readBuffer = null;
                READ_BUFFER_SIZE = value;
                _readBuffer = new byte[READ_BUFFER_SIZE];
            }
        }

        /// <summary>
        /// Gets sets, the write buffer size.
        /// </summary>
        public int WriteBufferSize
        {
            get { return WRITE_BUFFER_SIZE; }
            set
            {
                _writeBuffer = null;
                WRITE_BUFFER_SIZE = value;
                _writeBuffer = new byte[WRITE_BUFFER_SIZE];
            }
        }

        /// <summary>
        /// Gets sets, is the current connection in validation mode,
        /// is the current client connection pending validation. If true
        /// then the timeout set is in seconds and not re-initialised.
        /// </summary>
        public bool IsInValidationMode
        {
            get { return _isInValidationMode; }
            set { _isInValidationMode = value; }
        }

        /// <summary>
        /// Gets sets, the handler to the server receive data method.
        /// </summary>
        internal Action<IServer, IServerContext, byte[]> SendToServerHandler
        {
            get { return _sendToServerHandler; }
            set { _sendToServerHandler = value; }
        }

        /// <summary>
        /// Gets sets, the handler to the server receive data method (use for streaming data, non-command).
        /// </summary>
        internal Action<IServer, IServerContext, byte[]> SendToServerInfoHandler
        {
            get { return _sendToServerInfoHandler; }
            set { _sendToServerInfoHandler = value; }
        }

        /// <summary>
        /// Gets, the current server controller.
        /// </summary>
        public IServer Server
        {
            get { return _socketServer; }
            internal set { _socketServer = value; }
        }

        /// <summary>
        /// Gets, the x.509 certificate used for a secure connection.
        /// </summary>
        public X509Certificate2 X509Certificate
        {
            get { return _sslCertificate; }
            internal set { _sslCertificate = value; }
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
        /// Gets, should the server authenticate with the client and start a secure connection.
        /// </summary>
        /// <remarks>True if authentication begins within the Connect method; else false.
        /// When the Connect method is called and a secure connection has been chosen
        /// then this property is call asking if a secure connection is to be established immediatly.
        /// This is generally used for SSL protocol.</remarks>
        public bool BeginSslAuthenticate
        {
            get { return _beginSslAuthentication; }
            internal set { _beginSslAuthentication = value; }
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
        /// Gets, has a secure negotiation and server authentication 
        /// been established with the client. Also is the data encrypted.
        /// </summary>
        public bool IsSslAuthenticated
        {
            get { return _isSslAuthenticated; }
        }

        /// <summary>
        /// Gets sets, the timeout (seconds) for each client connection when in-active.
        /// Disconnects the client when this time out is triggered.
        /// </summary>
        public int TimeOut
        {
            get { return _timeOutInterval; }
            set
            {
                _timeOutInterval = value;
                if (_timeOutInterval > -1)
                {
                    // If not in validation mode.
                    if (!_isInValidationMode)
                    {
                        _timeOut = new Timer(ClientTimedOut, null,
                            new TimeSpan(0, 0, _timeOutInterval),
                            new TimeSpan(0, 0, _timeOutInterval));
                    }
                    else
                    {
                        _timeOut = new Timer(ClientTimedOut, null,
                            new TimeSpan(0, 0, _timeOutInterval),
                            new TimeSpan(0, 0, _timeOutInterval));
                    }
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
        /// Gets, a value that indicates whether a connection to a remote host exits.
        /// </summary>
        public bool Connected
        {
            get { return IsConnected(); }
        }

        /// <summary>
        /// Gets, the current socket.
        /// </summary>
        public System.Net.Sockets.Socket Socket
        {
            get { return _socket; }
            internal set { _socket = value; }
        }

        /// <summary>
        /// Gets, the current server name.
        /// </summary>
        public string Name
        {
            get { return _serverName; }
            internal set { _serverName = value; }
        }

        /// <summary>
        /// Gets, the current tcp SSL client socket.
        /// </summary>
        public System.Net.Security.SslStream SslStream
        {
            get { return _sslStream; }
            internal set { _sslStream = value; }
        }

        /// <summary>
        /// Gets sets, the send timeout.
        /// </summary>
        public int SendTimeout
        {
            get { return _timeoutSend; }
            set { _timeoutSend = value; }
        }

        /// <summary>
        /// Gets sets, the receive timeout.
        /// </summary>
        public int ReceiveTimeout
        {
            get { return _timeoutReceive; }
            set { _timeoutReceive = value; }
        }

        /// <summary>
        /// Gets sets, an indicator if data is being received 
        /// for the current connection after the receiver 
        /// has been started.
        /// </summary>
        public Action<byte[]> ReceiveActionHandler
        {
            get { return _receiverHandler; }
            set { _receiverHandler = value; }
        }

        /// <summary>
        /// Gets sets, the receive stream.
        /// </summary>
        public System.IO.Stream ReceiveStream
        {
            get { return _receiveStream; }
            set { _receiveStream = value; }
        }

        /// <summary>
        /// Gets sets, the send stream.
        /// </summary>
        public System.IO.Stream SendStream
        {
            get { return _sendStream; }
            set { _sendStream = value; }
        }

        /// <summary>
        /// Gets sets, the socket action type.
        /// </summary>
        public Nequeo.Net.SocketServerActionType SocketActionType
        {
            get { return _socketActionType; }
            set { _socketActionType = value; }
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
        /// Gets sets, the current unique connection identifier.
        /// </summary>
        public string ConnectionID
        {
            get { return _connectionID; }
            set { _connectionID = value; }
        }

        /// <summary>
        /// Gets sets, the state descriptor of the current client server context.
        /// </summary>
        public object State
        {
            get { return _state; }
            set { _state = value; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// The on command send event handler.
        /// </summary>
        internal event ServerContextHandler OnCommandSend;

        #endregion

        #region Protected Abstract Methods
        /// <summary>
        /// Triggered when a new connection has been established.
        /// </summary>
        /// <returns>True if the connection is accepted; else false.</returns>
        /// <remarks>If the connection is not accepted (false) then the 
        /// connection is automatically disconnectd.</remarks>
        protected abstract bool BeginConnect();

        /// <summary>
        /// Should the server authenticate with the client and start a secure connection.
        /// </summary>
        /// <returns>True if authentication begins straight after the BeginConnect method; else false.</returns>
        /// <remarks>If the BeginConnect returns true and a secure connection has been chosen
        /// then this method is call asking if a secure connection is to be established immediatly.
        /// This is generally used for SSL protocol.</remarks>
        protected abstract bool BeginSslAuthentication();

        /// <summary>
        /// End of the SSL authentication started with begin SSL authentication.
        /// </summary>
        protected abstract void EndSslAuthentication();

        /// <summary>
        /// Triggered when a new connection 
        /// has been established and all authentication is complete and the server is
        /// ready to accept data from the client.
        /// </summary>
        /// <param name="exception">The exception if any; else null.</param>
        protected abstract void EndConnect(Exception exception);

        /// <summary>
        /// Triggered when a connection has been closed. This also releases all resources
        /// </summary>
        protected abstract void Disconnected();

        /// <summary>
        /// Triggered when all resources are released an parent resources need to be released.
        /// </summary>
        protected abstract void DisposeOfResources();

        /// <summary>
        /// Triggered when the remote client has been authenticated with the credentials and
        /// allowed to communicate.
        /// </summary>
        protected abstract void Authenticated();

        /// <summary>
        /// Triggers when the sender has completed sending data.
        /// </summary>
        /// <remarks>Specificaly relates to the continuous sendind of data.</remarks>
        protected abstract void SenderComplete();

        /// <summary>
        /// Triggers when the receiver has completed sending data.
        /// </summary>
        /// <remarks>Specificaly relates to the continuous receiveing of data.</remarks>
        protected abstract void ReceiverComplete();

        /// <summary>
        /// The receive data method call.
        /// </summary>
        /// <param name="data">The data received from the client.</param>
        protected abstract void Receive(byte[] data);

        /// <summary>
        /// The receive data from the server method call.
        /// </summary>
        /// <param name="data">The data received from the server.</param>
        protected abstract void ReceiveFromServer(byte[] data);

        #endregion

        #region Public Methods
        /// <summary>
        /// Disconnect the current client and releases all resources.
        /// </summary>
        public virtual void Close()
        {
            Disconnect();
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
        /// Get the last error that occured.
        /// </summary>
        /// <returns>The last exception.</returns>
        public virtual Exception GetLastError()
        {
            return _lastException;
        }

        /// <summary>
        /// Get the client ip endpoint (remote end point).
        /// </summary>
        /// <returns>The client ip endpoint; else null.</returns>
        public virtual IPEndPoint GetClientIPEndPoint()
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                return ((IPEndPoint)_socket.RemoteEndPoint);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
                return null;
            }
        }

        /// <summary>
        /// Get the server ip endpoint (local end point).
        /// </summary>
        /// <returns>The server ip endpoint; else null.</returns>
        protected virtual IPEndPoint GetServerIPEndPoint()
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                return ((IPEndPoint)_socket.LocalEndPoint);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
                return null;
            }
        }

        /// <summary>
        /// Disconnect the current client and releases all resources.
        /// </summary>
        protected virtual void Disconnect()
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                // Indicate a conenction has been closed.
                Disconnected();
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            try
            {
                if (_socket != null)
                    _socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            try
            {
                if (_socket != null)
                    _socket.Disconnect(false);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            try
            {
                if (_socket != null)
                    _socket.Close();

                if (_sslStream != null)
                    _sslStream.Close();

                if (_networkStream != null)
                    _networkStream.Close();
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            try
            {
                // Send a disconnect command to the server
                // indicating that the client has been disconnected.
                if (OnCommandSend != null)
                    OnCommandSend(this, "DISCONNECT");
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Receive data from the remote host.
        /// </summary>
        /// <returns>The data received; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual byte[] Receive()
        {
            byte[] data = null;
            try
            {
                // Clear the last error.
                ClearLastError();

                // Create a new buffer byte array, stores the response.
                int bytesRead = 0;

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
                    _socket.ReceiveTimeout = _timeoutReceive;

                    // Get the data from the server placed it in the buffer.
                    lock (_readLock)
                        bytesRead = _socket.Receive(_readBuffer, 0, READ_BUFFER_SIZE, _receiveSocketFlags);

                    // Decode the data in the buffer to a string.
                    data = _readBuffer.Take(bytesRead).ToArray();
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            // If no connection exits then
            // dispose of the current client.
            if (!IsConnected())
            {
                Disconnect();
                return null;
            }
            else
            {
                // Return the response from the server.
                return data;
            }
        }

        /// <summary>
        /// Receive data from the remote host.
        /// </summary>
        /// <returns>The data received; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual string Read()
        {
            byte[] data = Receive();

            if (data != null)
                return Encoding.Default.GetString(data, 0, data.Length);
            else
                return null;
        }

        /// <summary>
        /// Send data to the remote host.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void Send(String data)
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                if (String.IsNullOrEmpty(data)) throw new ArgumentNullException("data");

                byte[] byteData = Encoding.Default.GetBytes(data);
                Send(byteData);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            // If no connection exits then
            // dispose of the current client.
            if (!IsConnected())
                Disconnect();
        }

        /// <summary>
        /// Send data to the remote host.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void Send(byte[] data)
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                SendEx(data);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            // If no connection exits then
            // dispose of the current client.
            if (!IsConnected())
                Disconnect();
        }

        /// <summary>
        /// Send data to the client through the server context from the server.
        /// </summary>
        /// <param name="data">The data received from the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void SentFromServer(byte[] data)
        {
            try
            {
                // Data sent from the server to this context
                // is sent to the server context implementation.
                ReceiveFromServer(data);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Send the data to the server.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        public virtual void SendToServer(byte[] data)
        {
            try
            {
                // Send data to the server.
                if (_sendToServerHandler != null)
                    _sendToServerHandler(_socketServer, this, data);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Send the data to the server raw.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        public virtual void SendToServerInfo(byte[] data)
        {
            try
            {
                // Send data to the server raw.
                if (_sendToServerInfoHandler != null)
                    _sendToServerInfoHandler(_socketServer, this, data);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Indicate that the remote host has been authenticated with the correct credentials.
        /// </summary>
        public virtual void SetAuthenticated()
        {
            try
            {
                // Triggered when the remote client has been authenticated with the credentials and
                // allowed to communicate.
                Authenticated();
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Start the current connection sender.
        /// </summary>
        protected virtual void StartSender()
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                if (_sendStream == null)
                    throw new Exception("A valid sender stream must be created first.");

                int readBytes = 0;

                // Get the first set of bytes from the stream.
                lock (_sendLock)
                    readBytes = _sendStream.Read(_writeBuffer, 0, WRITE_BUFFER_SIZE);

                if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                {
                    // If not authenticated.
                    if (!_isSslAuthenticated)
                    {
                        StartSenderSsl(_networkStream, readBytes);
                    }
                    else
                    {
                        StartSenderSsl(_sslStream, readBytes);
                    }
                }
                else
                {
                    _socket.SendTimeout = _timeoutSend;

                    // Send the command to the server.
                    lock (_sendLock)
                        _arSender = _socket.BeginSend(_writeBuffer, 0,
                            readBytes, _sendSocketFlags,
                            new AsyncCallback(DataSender), null);
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Start the current connection receiver.
        /// </summary>
        protected virtual void StartReceiver()
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                _receiverStopped = false;

                if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                {
                    // If not authenticated.
                    if (!_isSslAuthenticated)
                    {
                        StartReceiverSsl(_networkStream);
                    }
                    else
                    {
                        StartReceiverSsl(_sslStream);
                    }
                }
                else
                {
                    SocketBeginReceive();
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Stop the current connection receiver.
        /// </summary>
        protected virtual void StopReceiver()
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                if (_socket != null)
                {
                    // Stop blocking.
                    if (_arReceiver != null)
                    {
                        _receiverStopped = true;

                        // Close and un-block the current 
                        // receiver thread.
                        _arReceiver.AsyncWaitHandle.Close();
                        _arReceiver = null;
                    }
                }
                else
                    throw new Exception("A valid connection most be established first.");
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        /// <param name="tlsNegotiationCommand">Send a TLS negotiation command (e.g. OK Begin TLS negotiation now) if not null.</param>
        /// <returns>True if authentication has started; else false.</returns>
        /// <remarks>This is generally used for TLS protcol.</remarks>
        protected virtual bool BeginTlsNegotiation(string tlsNegotiationCommand = null)
        {
            bool ret = false;
            _useSslConnection = true;

            try
            {
                // Clear the last error.
                ClearLastError();

                if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                {
                    // If network socket has not been initalised.
                    if (_networkStream == null)
                    {
                        // Initially assign the network stream.
                        _networkStream = new NetworkStream(_socket);
                    }

                    // If not authenticated.
                    if (!_isSslAuthenticated)
                    {
                        // If ssl certificate has not been assigned.
                        if (_sslCertificate == null)
                            throw new Exception("Please add an SSL certificate for secure connections.");

                        // Get the current ssl stream
                        // from the socket.
                        _sslStream = new SslStream(_networkStream);
                        _sslStream.ReadTimeout = _timeoutReceive;

                        // Load the certificate into the
                        // secure stream used for secure communication.
                        lock (_readLock)
                        {
                            _arReceiver = _sslStream.BeginAuthenticateAsServer(
                                _sslCertificate, false, _sslProtocols, false,
                                new AsyncCallback(EndAuthenticateAsServerCallback), null);
                        }
                        ret = true;

                        // If not null then send the begin TLS negotiation command.
                        // This is in plain text letting the client know to start
                        // the TLS negotiation.
                        if (!String.IsNullOrEmpty(tlsNegotiationCommand))
                            Send(Encoding.Default.GetBytes(tlsNegotiationCommand));
                    }
                }
                else
                    throw new Exception("Method 'BeginTlsNegotiation' can only be called when using a secure connection and the socket type is streaming.");
            }
            catch (System.Security.Authentication.AuthenticationException aex)
            {
                if (_sslStream != null)
                    _sslStream.Dispose();

                SetLastError(aex);
                ret = false;
            }
            catch (Exception ex)
            {
                SetLastError(ex);
                ret = false;
            }

            // Return the result.
            return ret;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialise the connection with a new thread.
        /// </summary>
        public void InitialiseThread()
        {
            Thread initialiseThread = null;
            try
            {
                // Clear the last error.
                ClearLastError();

                // Create new threads for each
                // file transfer server.
                initialiseThread = new Thread(new ThreadStart(Initialise));
                initialiseThread.IsBackground = true;
                initialiseThread.Start();
                Thread.Sleep(20);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
            finally
            {
                initialiseThread = null;
            }
        }

        /// <summary>
        /// Initialise the connection.
        /// </summary>
        public void Initialise()
        {
            Exception exception = null;
            bool accepted = false;

            try
            {
                // Set the initial timeout time.
                _initialTimeOutTime = DateTime.Now;

                // Clear the last error.
                ClearLastError();

                // Indicate a new conenction has been established.
                accepted = BeginConnect();

                // Has the connection been accepted.
                if (accepted)
                {
                    _receiverStopped = false;

                    // Using ssl connection
                    // If socket type is streaming then
                    // create a network stream handler.
                    if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                    {
                        // Initially assign the network stream.
                        _networkStream = new NetworkStream(_socket);

                        // Should ssl authentication begin here.
                        bool auth = BeginSslAuthentication();
                        if (auth)
                        {
                            // Begin the secure negotiation and server authentication.
                            BeginSslNegotiationEx();
                        }
                        else
                        {
                            // Get the current network stream
                            // from the socket.
                            StreamIsSslAuthenticated();
                        }

                        // End ssl authentication here.
                        EndSslAuthentication();
                    }
                    else
                    {
                        SocketBeginReceive();
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                SetLastError(ex);
            }

            try
            {
                // Indicate a new conenction has been established.
                if (accepted)
                    EndConnect(exception);
                else
                    Disconnect();
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        private void BeginSslNegotiationEx()
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                // If not authenticated.
                if (!_isSslAuthenticated)
                {
                    // If ssl certificate has not been assigned.
                    if (_sslCertificate == null)
                        throw new Exception("Please add an SSL certificate for secure connections.");

                    // Get the current ssl stream
                    // from the socket.
                    _sslStream = new SslStream(_networkStream);

                    // Load the certificate into the
                    // secure stream used for secure communication.
                    _sslStream.AuthenticateAsServer(_sslCertificate, false, _sslProtocols, false);

                    // Get the state of the authentication.
                    if (_sslStream.IsAuthenticated && _sslStream.IsEncrypted)
                        _isSslAuthenticated = true;
                    else
                        _isSslAuthenticated = false;
                }
            }
            catch (System.Security.Authentication.AuthenticationException aex)
            {
                if (_sslStream != null)
                    _sslStream.Dispose();

                SetLastError(aex);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            try
            {
                // Select the correct stream.
                StreamIsSslAuthenticated();
            }
            catch { }
        }

        /// <summary>
        /// End the authenticate.
        /// </summary>
        /// <param name="ar">The async result.</param>
        private void EndAuthenticateAsServerCallback(IAsyncResult ar)
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                // End the authentication.
                _sslStream.EndAuthenticateAsServer(ar);

                // Get the state of the authentication.
                if (_sslStream.IsAuthenticated && _sslStream.IsEncrypted)
                    _isSslAuthenticated = true;
                else
                    _isSslAuthenticated = false;
            }
            catch (System.Security.Authentication.AuthenticationException aex)
            {
                if (_sslStream != null)
                    _sslStream.Dispose();

                SetLastError(aex);
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            try
            {
                // Select the correct stream.
                StreamIsSslAuthenticated();
            }
            catch { }
        }

        /// <summary>
        /// Data sender asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="ar">The current asynchronus result.</param>
        private void DataSender(IAsyncResult ar)
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                {
                    // If not authenticated.
                    if (!_isSslAuthenticated)
                    {
                        DataSenderSsl(_networkStream, ar);
                    }
                    else
                    {
                        DataSenderSsl(_sslStream, ar);
                    }
                }
                else
                {
                    // Finish asynchronous read into readBuffer 
                    // and get number of bytes read.
                    lock (_sendLock)
                        _socket.EndSend(ar);
                }

                // Data sender.
                DataSenderEx();
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Data sender asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        private void DataSenderEx()
        {
            bool isConnected = false;
            try
            {
                // Clear the last error.
                ClearLastError();

                int bytesRead = 0;
                isConnected = IsConnected();

                if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                {
                    // If not authenticated.
                    if (!_isSslAuthenticated)
                    {
                        DataSenderSslEx(_networkStream, isConnected);
                    }
                    else
                    {
                        DataSenderSslEx(_sslStream, isConnected);
                    }
                }
                else
                {
                    // The send time out.
                    _socket.SendTimeout = _timeoutSend;

                    lock (_sendLock)
                    {
                        // Read all the data in the upload file and
                        // send the data from the file to the client 
                        // through the current network stream.
                        do
                        {
                            bytesRead = _sendStream.Read(_writeBuffer, 0, WRITE_BUFFER_SIZE);
                            _socket.Send(_writeBuffer, 0, bytesRead, _sendSocketFlags);

                            InActiveTimeOutSetter();
                        }
                        while (bytesRead != 0 && isConnected);
                    }
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            try
            {
                // Trigger sender complete event.
                SenderComplete();
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            // If no connection exits then
            // dispose of the current client.
            if (!isConnected)
                Disconnect();
        }

        /// <summary>
        /// Data received asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="ar">The current asynchronus result.</param>
        private void DataReceiver(IAsyncResult ar)
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                {
                    // If not authenticated.
                    if (!_isSslAuthenticated)
                    {
                        DataReceiverSsl(_networkStream, ar);
                    }
                    else
                    {
                        DataReceiverSsl(_sslStream, ar);
                    }
                }
                else
                {
                    // Finish asynchronous read into readBuffer 
                    // and get number of bytes read.
                    lock (_readLock)
                        _dataReceiverBytesRead = _socket.EndReceive(ar);
                }

                // Data received.
                DataReceiverEx();
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Data received asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        private void DataReceiverEx()
        {
            bool isConnected = false;
            int bytesRead = 0;

            try
            {
                // Clear the last error.
                ClearLastError();

                bytesRead = _dataReceiverBytesRead;

                // If no data has been sent then
                // assume the connection has been closed.
                if (bytesRead > 0)
                    isConnected = IsConnected();
                else
                    isConnected = false;

                if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                {
                    // If not authenticated.
                    if (!_isSslAuthenticated)
                    {
                        DataReceiverSslEx(_networkStream, isConnected);
                    }
                    else
                    {
                        DataReceiverSslEx(_sslStream, isConnected);
                    }
                }
                else
                {
                    // If data exists.
                    if (bytesRead > 0)
                    {
                        DataReceiverActionHandler(bytesRead);

                        // If the time out control has been created
                        // then reset the timer.
                        InActiveTimeOutSetter();
                    }

                    // The read time out.
                    _socket.ReceiveTimeout = _timeoutReceive;

                    // While data is avaliable.
                    while (bytesRead > 0 && isConnected && _socket.Available > 0)
                    {
                        bytesRead = _socket.Receive(_readBuffer, 0, READ_BUFFER_SIZE, _receiveSocketFlags);
                        DataReceiverActionHandler(bytesRead);

                        // If the time out control has been created
                        // then reset the timer.
                        InActiveTimeOutSetter();
                    }

                    // If the receiver has not been stopped.
                    if (!_receiverStopped && isConnected)
                    {
                        Thread.Sleep(50);
                        SocketBeginReceive();
                    }
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            try
            {
                if (bytesRead > 0)
                {
                    // Trigger receiver complete event.
                    ReceiverComplete();
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            // If no connection exits then
            // dispose of the current client.
            if (!isConnected)
                Disconnect();
        }

        /// <summary>
        /// Send data to the host.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        private void SendEx(byte[] data)
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                if (data == null) throw new ArgumentNullException("data");

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
                    _socket.SendTimeout = _timeoutSend;

                    // Send the command to the server.
                    lock (_sendLock)
                        _socket.Send(data);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Stream is ssl authenticated.
        /// </summary>
        private void StreamIsSslAuthenticated()
        {
            // If authenticated.
            if (_isSslAuthenticated)
                StreamBeginReadSsl(_sslStream);
            else
                StreamBeginReadSsl(_networkStream);
        }

        /// <summary>
        /// Stream begin read ssl.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        private void StreamBeginReadSsl(System.IO.Stream stream)
        {
            stream.ReadTimeout = _timeoutReceive;

            lock (_readLock)
            {
                // Start a new read thread.
                _arReceiver = stream.BeginRead(_readBuffer, 0,
                    READ_BUFFER_SIZE, new AsyncCallback(DataReceiver), null);
            }
        }

        /// <summary>
        /// Socket begin receive
        /// </summary>
        private void SocketBeginReceive()
        {
            _socket.ReceiveTimeout = _timeoutReceive;

            // Start a new asynchronous read into readBuffer.
            // more data maybe present. This will wait for more
            // data from the client.
            lock (_readLock)
                _arReceiver = _socket.BeginReceive(_readBuffer, 0,
                    READ_BUFFER_SIZE, _receiveSocketFlags,
                    new AsyncCallback(DataReceiver), null);
        }

        /// <summary>
        /// Data received asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        /// <param name="isConnected">Is connected.</param>
        private void DataReceiverSslEx(System.IO.Stream stream, bool isConnected)
        {
            int bytesRead = 0;

            bytesRead = _dataReceiverBytesRead;

            // If data exists.
            if (bytesRead > 0)
            {
                DataReceiverActionHandler(bytesRead);

                // If the time out control has been created
                // then reset the timer.
                InActiveTimeOutSetter();
            }

            // The read time out.
            stream.ReadTimeout = _timeoutReceive;

            // While data is avaliable.
            while (bytesRead > 0 && isConnected && _socket.Available > 0)
            {
                bytesRead = stream.Read(_readBuffer, 0, READ_BUFFER_SIZE);
                DataReceiverActionHandler(bytesRead);

                // If the time out control has been created
                // then reset the timer.
                InActiveTimeOutSetter();
            }

            // If the receiver has not been stopped.
            if (!_receiverStopped && isConnected)
            {
                Thread.Sleep(50);

                // Start a new asynchronous read into readBuffer.
                // more data maybe present. This will wait for more
                // data from the client.
                lock (_readLock)
                    _arReceiver = stream.BeginRead(_readBuffer, 0,
                        READ_BUFFER_SIZE, new AsyncCallback(DataReceiver), null);
            }
        }

        /// <summary>
        /// Data sender asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        /// <param name="isConnected">Is connected.</param>
        private void DataSenderSslEx(System.IO.Stream stream, bool isConnected)
        {
            int bytesRead = 0;

            // The send time out.
            stream.WriteTimeout = _timeoutSend;

            lock (_sendLock)
            {
                // Read all the data in the upload file and
                // send the data from the file to the client 
                // through the current network stream.
                do
                {
                    bytesRead = _sendStream.Read(_writeBuffer, 0, WRITE_BUFFER_SIZE);
                    stream.Write(_writeBuffer, 0, bytesRead);

                    // If the time out control has been created
                    // then reset the timer.
                    InActiveTimeOutSetter();
                }
                while (bytesRead != 0 && isConnected);
            }
        }

        /// <summary>
        /// Data received asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        /// <param name="ar">The current asynchronus result.</param>
        private void DataReceiverSsl(System.IO.Stream stream, IAsyncResult ar)
        {
            // Finish asynchronous read into readBuffer 
            // and get number of bytes read.
            lock (_readLock)
                _dataReceiverBytesRead = stream.EndRead(ar);
        }

        /// <summary>
        /// Data sender asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        /// <param name="ar">The current asynchronus result.</param>
        private void DataSenderSsl(System.IO.Stream stream, IAsyncResult ar)
        {
            // Finish asynchronous read into readBuffer 
            // and get number of bytes read.
            lock (_sendLock)
                stream.EndWrite(ar);
        }

        /// <summary>
        /// Start the current connection receiver.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        private void StartReceiverSsl(System.IO.Stream stream)
        {
            stream.ReadTimeout = _timeoutReceive;

            lock (_readLock)
            {
                // Start a new read thread.
                _arReceiver = stream.BeginRead(_readBuffer, 0,
                    READ_BUFFER_SIZE, new AsyncCallback(DataReceiver), null);
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
            stream.WriteTimeout = _timeoutSend;

            // Send the command to the server.
            lock (_sendLock)
                stream.Write(data, 0, data.Length);
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
            byte[] data = null;
            int bytesRead = 0;

            stream.ReadTimeout = _timeoutReceive;

            // Get the data from the server placed it in the buffer.
            lock (_readLock)
                bytesRead = stream.Read(_readBuffer, 0, READ_BUFFER_SIZE);

            // Decode the data in the buffer to a string.
            data = _readBuffer.Take(bytesRead).ToArray();
            return data;
        }

        /// <summary>
        /// Start the current connection sender.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        /// <param name="readBytes">The number of byters to write.</param>
        private void StartSenderSsl(System.IO.Stream stream, int readBytes)
        {
            stream.WriteTimeout = _timeoutSend;

            // Send the command to the server.
            lock (_sendLock)
                _arSender = stream.BeginWrite(_writeBuffer, 0,
                    readBytes, new AsyncCallback(DataSender), null);
        }

        /// <summary>
        /// In-active timeout setter.
        /// </summary>
        private void InActiveTimeOutSetter()
        {
            // If not in validation mode.
            if (!_isInValidationMode)
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
        }

        /// <summary>
        /// Action handler data received asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="bytesRead">The number of bytes to read.</param>
        private void DataReceiverActionHandler(int bytesRead)
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                // Select the appropriate action.
                switch (_socketActionType)
                {
                    case SocketServerActionType.Method:
                        // Write the data to the absteact method.
                        Receive(_readBuffer.Take(bytesRead).ToArray());
                        break;

                    case SocketServerActionType.ActionHandler:
                        // Write the data to the action handler.
                        if (_receiverHandler != null)
                        {
                            // Write to the handler.
                            _receiverHandler(_readBuffer.Take(bytesRead).ToArray());
                        }
                        break;

                    case SocketServerActionType.Stream:
                        // Write the data to the stream.
                        if (_receiveStream != null)
                        {
                            lock (_readLock)
                            {
                                // Write to the stream.
                                _receiveStream.Write(_readBuffer, 0, bytesRead);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Action handler validator
        /// </summary>
        private void DataReceiverActionHandlerValidator()
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                // Select the appropriate action.
                switch (_socketActionType)
                {
                    case SocketServerActionType.ActionHandler:
                        // Write the data to the action handler.
                        if (_receiverHandler == null)
                        {
                            throw new Exception("A valid receiver action handler must be created first.");
                        }
                        break;

                    case SocketServerActionType.Stream:
                        // Write the data to the stream.
                        if (_receiveStream == null)
                        {
                            throw new Exception("A valid receiver stream must be created first.");
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
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
                // Clear the last error.
                ClearLastError();

                if (_timeOut != null)
                    _timeOut.Dispose();
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            Disconnect();
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
                    try {
                        // Release the parent resources.
                        DisposeOfResources(); } catch { }

                    if (_sslStream != null)
                        _sslStream.Dispose();

                    if (_networkStream != null)
                        _networkStream.Dispose();

                    if (_socket != null)
                        _socket.Dispose();

                    if (_timeOut != null)
                        _timeOut.Dispose();

                    if (_arReceiver != null)
                        if (_arReceiver.AsyncWaitHandle != null)
                            _arReceiver.AsyncWaitHandle.Dispose();

                    if (_arSender != null)
                        if (_arSender.AsyncWaitHandle != null)
                            _arSender.AsyncWaitHandle.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _socket = null;
                _sslStream = null;
                _networkStream = null;
                _timeOut = null;
                _arReceiver = null;
                _arSender = null;

                _sendLock = null;
                _readLock = null;
                _connectionLock = null;

                _writeBuffer = null;
                _readBuffer = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~ServerContext()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
