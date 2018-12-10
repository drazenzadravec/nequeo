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

namespace Nequeo.Net.Sockets
{
    /// <summary>
    /// General socket client.
    /// </summary>
    public partial class Client : IClient, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Socket client constructor.
        /// </summary>
        /// <param name="address">An IP address.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Client(IPAddress address, int port)
        {
            if (address == null) throw new ArgumentNullException("address");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");

            _specificAddressFamily = address.AddressFamily;
            _address = address;
            _port = port;

            Init();
        }

        /// <summary>
        /// Socket client constructor.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Client(string hostNameOrAddress, int port)
        {
            if (hostNameOrAddress == null) throw new ArgumentNullException("hostNameOrAddress");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");

            _hostNameOrAddress = hostNameOrAddress;
            _port = port;

            Init();
        }
        #endregion

        #region Private Fields
        private int READ_BUFFER_SIZE = 8192;
        private int WRITE_BUFFER_SIZE = 8192;

        private object _sendLock = new object();
        private object _readLock = new object();

        private string _hostNameOrAddress = string.Empty;
        private IPAddress _address = null;

        private bool _beginSslAuthentication = true;
        private bool _isSslAuthenticated = false;
        private bool _useSslConnection = false;
        private bool _validateSslCertificate = false;
        private X509Certificate2Info _sslCertificate = null;
        private SslProtocols _sslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls | SslProtocols.Ssl3;
        private SocketFlags _receiveSocketFlags = SocketFlags.None;
        private SocketFlags _sendSocketFlags = SocketFlags.None;

        private System.Net.Sockets.NetworkStream _networkStream = null;
        private System.Net.Security.SslStream _sslStream = null;
        private System.Net.Sockets.Socket _socket = null;

        private IPHostEntry _hostEntry = null;
        private System.Net.Sockets.ProtocolType _protocolType = System.Net.Sockets.ProtocolType.Tcp;
        private System.Net.Sockets.SocketType _socketType = System.Net.Sockets.SocketType.Stream;
        private X509CertificateCollection _clientCertificates = null;

        private byte[] _readBuffer = null;
        private byte[] _writeBuffer = null;
        private System.IO.Stream _receiveStream = null;
        private System.IO.Stream _sendStream = null;
        private IAsyncResult _arReceiver = null;
        private IAsyncResult _arSender = null;
        private bool _receiverStopped = false;
        private Action<byte[]> _receiverHandler = null;
        private Nequeo.Net.SocketClientActionType _socketActionType = SocketClientActionType.None;

        private int _timeoutSend = -1;
        private int _timeoutReceive = -1;
        private bool _isClosed = true;

        private string _connectionID = null;
        private System.Net.Sockets.AddressFamily _specificAddressFamily = AddressFamily.Unspecified;
        private int _port = -1;

        private Timer _timeOut = null;
        private int _timeOutInterval = -1;

        private int _dataReceiverBytesRead = 0;
        private bool _beginConnectMode = false;
        private Exception _beginConnectException = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the client certificates.
        /// </summary>
        public X509CertificateCollection ClientCertificates
        {
            get { return _clientCertificates; }
            set { _clientCertificates = value; }
        }

        /// <summary>
        /// Gets or sets the current unique connection identifier.
        /// </summary>
        public string ConnectionID
        {
            get { return _connectionID; }
            set { _connectionID = value; }
        }

        /// <summary>
        /// Gets, the current socket.
        /// </summary>
        public System.Net.Sockets.Socket Socket
        {
            get { return _socket; }
        }

        /// <summary>
        /// Gets, the current SSL socket.
        /// </summary>
        public System.Net.Security.SslStream SslStream
        {
            get { return _sslStream; }
        }

        /// <summary>
        /// Gets, the x.509 certificate info.
        /// </summary>
        public X509Certificate2Info X509Certificate2Info
        {
            get { return _sslCertificate; }
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
        /// Gets sets, the client time out (seconds) interval.
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
        /// Gets sets, IP address.
        /// </summary>
        public IPAddress IPAddress
        {
            get { return _address; }
            set
            {
                _address = value;
                _specificAddressFamily = _address.AddressFamily;
            }
        }

        /// <summary>
        /// Gets sets, the host name or IP address to resolve.
        /// </summary>
        public string HostNameOrAddress
        {
            get { return _hostNameOrAddress; }
            set { _hostNameOrAddress = value; }
        }

        /// <summary>
        /// Gets, a value that indicates whether a connection to a remote host exits.
        /// </summary>
        public bool Connected
        {
            get { return IsConnected(); }
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
        /// Gets sets, the protocol type.
        /// </summary>
        public System.Net.Sockets.ProtocolType ProtocolType
        {
            get { return _protocolType; }
            set { _protocolType = value; }
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
        /// Gets sets, the addressing scheme that an instance of the connection to use.
        /// If AddressFamily.Unspecified is used then any endpoint binding address is used.
        /// </summary>
        public System.Net.Sockets.AddressFamily AddressFamily
        {
            get { return _specificAddressFamily; }
            set { _specificAddressFamily = value; }
        }

        /// <summary>
        /// Gets sets, validate the x.509 certificate.
        /// </summary>
        public bool ValidateSslCertificate
        {
            get { return _validateSslCertificate; }
            set { _validateSslCertificate = value; }
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
        /// Gets, has a secure negotiation and server authentication 
        /// been established with the client. Also is the data encrypted.
        /// </summary>
        public bool IsSslAuthenticated
        {
            get { return _isSslAuthenticated; }
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
        public Nequeo.Net.SocketClientActionType SocketActionType
        {
            get { return _socketActionType; }
            set { _socketActionType = value; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// The on disconnected event handler, triggered when a connection has been closed.
        /// </summary>
        public event System.EventHandler OnDisconnected;

        /// <summary>
        /// Triggers when the sender has completed sending data.
        /// </summary>
        /// <remarks>Specificaly relates to the continuous sendind of data.</remarks>
        public event System.EventHandler OnSenderComplete;

        /// <summary>
        /// Triggers when the receiver has completed sending data.
        /// </summary>
        /// <remarks>Specificaly relates to the continuous receiveing of data.</remarks>
        public event System.EventHandler OnReceiverComplete;

        /// <summary>
        /// Triggers when an internal error occurs.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Exception, string> OnInternalError;

        /// <summary>
        /// Triggered when the client in-active time out has been reached.
        /// </summary>
        public event System.EventHandler OnTimedOut;

        #endregion

        #region Public Methods
        /// <summary>
        /// Send data to the host.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual void Send(String data)
        {
            try
            {
                if (String.IsNullOrEmpty(data)) throw new ArgumentNullException("data");

                byte[] byteData = Encoding.Default.GetBytes(data);
                Send(byteData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Send data to the host.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual void Send(byte[] data)
        {
            try
            {
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
                    if (_socket != null)
                    {
                        _socket.SendTimeout = _timeoutSend;

                        // Send the command to the server.
                        lock (_sendLock)
                            _socket.Send(data);
                    }
                    else
                        throw new Exception("A valid connection most be established first.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Receive data from the host.
        /// </summary>
        /// <returns>The data received; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual byte[] Receive()
        {
            byte[] data = null;
            try
            {
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
                    if (_socket != null)
                    {
                        _socket.ReceiveTimeout = _timeoutReceive;

                        lock (_readLock)
                            // Get the data from the server placed it in the buffer.
                            bytesRead = _socket.Receive(_readBuffer, 0, READ_BUFFER_SIZE, _receiveSocketFlags);

                        // Decode the data in the buffer to a string.
                        data = _readBuffer.Take(bytesRead).ToArray();
                    }
                    else
                        throw new Exception("A valid connection most be established first.");
                }

                // Return the response from the server.
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Receive data from the host.
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
        /// Start the current connection sender.
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        public virtual void StartSender()
        {
            try
            {
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
                    if (_socket != null)
                    {
                        _socket.SendTimeout = _timeoutSend;

                        // Send the command to the server.
                        lock (_sendLock)
                            _arSender = _socket.BeginSend(_writeBuffer, 0,
                                readBytes, _sendSocketFlags,
                                new AsyncCallback(DataSender), null);
                    }
                    else
                        throw new Exception("A valid connection most be established first.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Start the current connection receiver.
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        public virtual void StartReceiver()
        {
            try
            {
                if (_receiveStream == null && _receiverHandler == null)
                    throw new Exception("A valid receiver stream or receiver action handler must be created first.");

                // Validate the action handlers to the action.
                DataReceiverActionHandlerValidator();

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
                    if (_socket != null)
                    {
                        SocketBeginReceive();
                    }
                    else
                        throw new Exception("A valid connection most be established first.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Stop the current connection receiver.
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        public virtual void StopReceiver()
        {
            try
            {
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
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Close the current socket connection and disposes of all resources.
        /// </summary>
        public virtual void Close()
        {
            // Close the connection.
            CloseEx();

            // Release the resources.
            _sslStream = null;
            _networkStream = null;
            _socket = null;
        }

        /// <summary>
        /// Connect to the host socket.
        /// </summary>
        public virtual void Connect()
        {
            try
            {
                // Will contain an error list.
                string errorList = string.Empty;

                // If the connection has not been closed
                // then return and perform no action.
                if (!_isClosed)
                    return;

                // Is the host name an IP Address.
                IPAddress isIPAddress = null;
                bool ret = IPAddress.TryParse(_hostNameOrAddress, out isIPAddress);
                if (!ret)
                {
                    // Get host related information.
                    if (!String.IsNullOrEmpty(_hostNameOrAddress))
                        _hostEntry = System.Net.Dns.GetHostEntry(_hostNameOrAddress);
                    else
                    {
                        _hostNameOrAddress = _address.ToString();
                        _hostEntry = System.Net.Dns.GetHostEntry(_address);
                    }
                }
                else
                {
                    // Could not find host.
                    _hostEntry = new IPHostEntry
                    {
                        HostName = _hostNameOrAddress,
                        AddressList = new IPAddress[] { IPAddress.Parse(_hostNameOrAddress) }
                    };
                }

                // Loop through the AddressList to obtain the supported 
                // AddressFamily. This is to avoid an exception that 
                // occurs when the host IP Address is not compatible 
                // with the address family 
                // (typical in the IPv6 case).
                foreach (IPAddress address in _hostEntry.AddressList)
                {
                    // Find the address family to connect to.
                    if (_specificAddressFamily != System.Net.Sockets.AddressFamily.Unspecified)
                        if (_specificAddressFamily != address.AddressFamily)
                            continue;

                    // Get the current server endpoint for
                    // the current address.
                    IPEndPoint endPoint = new IPEndPoint(address, _port);
                    System.Net.Sockets.Socket tempSocket = null;

                    // Create a new client socket for the
                    // current endpoint.
                    tempSocket = new System.Net.Sockets.Socket(endPoint.AddressFamily, _socketType, _protocolType);

                    // Connect to the server with the
                    // current end point.
                    try
                    {
                        // Attempt to connect to the server.
                        tempSocket.Connect(endPoint);
                    }
                    catch (Exception ex)
                    {
                        errorList = ex.Message + "\r\n";
                    }

                    // If this connection succeeded then
                    // asiign the client socket and
                    // break put of the loop.
                    if (tempSocket.Connected)
                    {
                        // A client connection has been found.
                        // Break out of the loop.
                        _socket = tempSocket;
                        _isClosed = false;

                        // If socket type is streaming then
                        // create a network stream handler.
                        if (_socketType == System.Net.Sockets.SocketType.Stream)
                        {
                            // Create the network stream used for secure connections.
                            _networkStream = new NetworkStream(_socket);
                        }

                        // Is an ssl connection.
                        if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                        {
                            // Should the server authenticate with the client and start a secure connection.
                            if (_beginSslAuthentication)
                            {
                                // Begin the secure negotiation and server authentication.
                                BeginSslNegotiationEx();
                            }
                        }
                        break;
                    }
                    else continue;
                }

                // If no connection has been established.
                if (_isClosed)
                    throw new Exception(errorList);

                // Set the execetion to null.
                _beginConnectException = null;
            }
            catch (Exception ex)
            {
                // Close the connection.
                Close();

                if (!_beginConnectMode)
                    throw;
                else
                    _beginConnectException = ex;
            }
        }

        /// <summary>
        /// Begin connect to the host socket.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        /// <exception cref="System.Exception"></exception>
        public IAsyncResult BeginConnect(AsyncCallback callback, object state)
        {
            // Process the task.
            return Nequeo.Threading.AsyncOperationResult<Boolean>.
                RunTask(() => Connect()).ContinueWith(t => callback(t));
        }

        /// <summary>
        /// End connect to the host socket.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <exception cref="System.Exception"></exception>
        public void EndConnect(IAsyncResult ar)
        {
            _beginConnectMode = false;

            if (ar != null)
                ((Task)ar).Dispose();

            // Throw the exception if any.
            if (_beginConnectException != null)
                throw _beginConnectException;
        }

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        /// <param name="continueNegotiation">Continue the negotiation handler. After sending the command, wait for a 
        /// response. If the response is correct then continue the negotiation process.</param>
        /// <param name="tlsNegotiationCommand">Send a TLS negotiation command (e.g. STARTTLS) if not null.</param>
        /// <returns>True if authentication has started; else false.</returns>
        /// <remarks>This is generally used for TLS protcol.</remarks>
        public virtual bool BeginTlsNegotiation(Func<bool> continueNegotiation, string tlsNegotiationCommand = "STARTTLS\r\n")
        {
            bool ret = false;
            _useSslConnection = true;

            try
            {
                if (_useSslConnection && _socketType == System.Net.Sockets.SocketType.Stream)
                {
                    // If not authenticated.
                    if (!_isSslAuthenticated)
                    {
                        // If not null then send the begin TLS negotiation command.
                        // This is in plain text letting the client know to start
                        // the TLS negotiation.
                        if (!String.IsNullOrEmpty(tlsNegotiationCommand))
                            Send(Encoding.Default.GetBytes(tlsNegotiationCommand));

                        // If the response from the server is correct
                        // then continue negotiation and validate
                        // as client.
                        if (continueNegotiation())
                        {
                            // Remote certificate validation call back.
                            RemoteCertificateValidationCallback callback =
                                new RemoteCertificateValidationCallback(CertificateValidation);

                            // Get the current ssl stream
                            // from the socket.
                            _sslStream = new SslStream(_networkStream, true, callback);
                            
                            // Load the certificate into the
                            // secure stream used for secure communication.
                            _sslStream.BeginAuthenticateAsClient(
                                _hostEntry.HostName, _clientCertificates, _sslProtocols, false,
                                new AsyncCallback(EndAuthenticateAsClientCallback), null);

                            // Negotiation complete.
                            ret = true;
                        }
                    }
                }
                else
                    throw new Exception("Method 'BeginTlsNegotiation' can only be called when using a secure connection and the socket type must be streaming.");

                // Return the result.
                return ret;
            }
            catch (System.Security.Authentication.AuthenticationException)
            {
                if (_sslStream != null)
                    _sslStream.Dispose();

                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Certificate validator.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        public virtual bool CertificateValidation(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Create a new certificate collection
            // instance and return false.
            _sslCertificate = new X509Certificate2Info(certificate as X509Certificate2,
                chain, sslPolicyErrors);

            // Certificate should be validated.
            if (_validateSslCertificate)
            {
                // If the certificate is valid
                // return true.
                if (sslPolicyErrors == SslPolicyErrors.None)
                    return true;
                else
                    return false;
            }
            else
                // Return true.
                return true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialise.
        /// </summary>
        private void Init()
        {
            OnCreated();

            _readBuffer = new byte[READ_BUFFER_SIZE];
            _writeBuffer = new byte[WRITE_BUFFER_SIZE];

            // Create a new connection ID.
            _connectionID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// End the authenticate.
        /// </summary>
        /// <param name="ar">The async result.</param>
        private void EndAuthenticateAsClientCallback(IAsyncResult ar)
        {
            try
            {
                // End the authentication.
                _sslStream.EndAuthenticateAsClient(ar);

                // Get the state of the authentication.
                if (_sslStream.IsAuthenticated && _sslStream.IsEncrypted)
                    _isSslAuthenticated = true;
                else
                    _isSslAuthenticated = false;
            }
            catch (System.Security.Authentication.AuthenticationException ex)
            {
                if (_sslStream != null)
                    _sslStream.Dispose();

                if (OnInternalError != null)
                    OnInternalError(this, ex, "EndAuthenticateAsClientCallback");
            }
            catch (Exception ex)
            {
                if (OnInternalError != null)
                    OnInternalError(this, ex, "EndAuthenticateAsClientCallback");
            }
        }

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        private void BeginSslNegotiationEx()
        {
            try
            {
                // Remote certificate validation call back.
                RemoteCertificateValidationCallback callback =
                    new RemoteCertificateValidationCallback(CertificateValidation);

                // Get the current ssl stream
                // from the socket.
                _sslStream = new SslStream(_networkStream, true, callback);
                _sslStream.AuthenticateAsClient(_hostEntry.HostName, _clientCertificates, _sslProtocols, false);

                // Get the state of the authentication.
                if (_sslStream.IsAuthenticated && _sslStream.IsEncrypted)
                    _isSslAuthenticated = true;
                else
                    _isSslAuthenticated = false;

            }
            catch (System.Security.Authentication.AuthenticationException ex)
            {
                if (_sslStream != null)
                    _sslStream.Dispose();

                if (OnInternalError != null)
                    OnInternalError(this, ex, "BeginSslNegotiationEx");
            }
            catch (Exception ex)
            {
                if (OnInternalError != null)
                    OnInternalError(this, ex, "BeginSslNegotiationEx");
            }
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
                    _socket.EndSend(ar);
                }

                // Data sender
                DataSenderEx();
            }
            catch (Exception ex)
            {
                if (OnInternalError != null)
                    OnInternalError(this, ex, "DataSender");
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

                            // If the time out control has been created
                            // then reset the timer.
                            InActiveTimeOutSetter();
                        }
                        while (bytesRead != 0 && isConnected);
                    }
                }
            }
            catch (Exception ex)
            {
                if (OnInternalError != null)
                    OnInternalError(this, ex, "DataSender");
            }

            // Trigger sender complete event.
            if (OnSenderComplete != null)
                OnSenderComplete(this, new EventArgs());

            // If no connection exits then
            // dispose of the current client.
            if (!isConnected)
            {
                // Indicate a conenction has been closed.
                if (OnDisconnected != null)
                    OnDisconnected(this, new EventArgs());
            }
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
                    _dataReceiverBytesRead = _socket.EndReceive(ar);
                }

                // Data received
                DataReceiverEx();
            }
            catch (Exception ex)
            {
                if (OnInternalError != null)
                    OnInternalError(this, ex, "DataReceiver");
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

                        // Start a new asynchronous read into readBuffer.
                        // more data maybe present. This will wait for more
                        // data from the client.
                        lock (_readLock)
                            _arReceiver = _socket.BeginReceive(_readBuffer, 0,
                                READ_BUFFER_SIZE, _receiveSocketFlags,
                                new AsyncCallback(DataReceiver), null);
                    }
                }
            }
            catch (Exception ex)
            {
                if (OnInternalError != null)
                    OnInternalError(this, ex, "DataReceiver");
            }

            // Trigger receiver complete event.
            if (OnReceiverComplete != null)
                OnReceiverComplete(this, new EventArgs());

            // If no connection exits then
            // dispose of the current client.
            if (!isConnected)
            {
                // Indicate a conenction has been closed.
                if (OnDisconnected != null)
                    OnDisconnected(this, new EventArgs());
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
                // Select the appropriate action.
                switch (_socketActionType)
                {
                    case SocketClientActionType.ActionHandler:
                        // Write the data to the action handler.
                        if (_receiverHandler != null)
                        {
                            // Write to the handler.
                            _receiverHandler(_readBuffer.Take(bytesRead).ToArray());
                        }
                        break;

                    case SocketClientActionType.Stream:
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
                if (OnInternalError != null)
                    OnInternalError(this, ex, "DataReceiver");
            }
        }

        /// <summary>
        /// Action handler validator
        /// </summary>
        private void DataReceiverActionHandlerValidator()
        {
            try
            {
                // Select the appropriate action.
                switch (_socketActionType)
                {
                    case SocketClientActionType.ActionHandler:
                        // Write the data to the action handler.
                        if (_receiverHandler == null)
                        {
                            throw new Exception("A valid receiver action handler must be created first.");
                        }
                        break;

                    case SocketClientActionType.Stream:
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
                if (OnInternalError != null)
                    OnInternalError(this, ex, "DataReceiver");
            }
        }

        /// <summary>
        /// Disconnects the current client after the time out
        /// interval has elapsed.
        /// </summary>
        /// <param name="state">A passed object state.</param>
        private void ClientTimedOut(object state)
        {
            // Trigger the time out event.
            if (OnTimedOut != null)
                OnTimedOut(this, new EventArgs());

            // If the time out control has been created
            // then reset the timer.
            InActiveTimeOutSetter();
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
        /// Close the current socket connection and disposes of all resources.
        /// </summary>
        private void CloseEx()
        {
            _isClosed = true;

            try
            {
                // Shutdown the socket.
                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch { }

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
                    _socket.Close();

                // Close the ssl stream.
                if (_sslStream != null)
                    _sslStream.Close();

                // Close the network stream..
                if (_networkStream != null)
                    _networkStream.Close();
            }
            catch { }
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
        /// Data received asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        /// <param name="ar">The current asynchronus result.</param>
        private void DataReceiverSsl(System.IO.Stream stream, IAsyncResult ar)
        {
            // Finish asynchronous read into readBuffer 
            // and get number of bytes read.
            _dataReceiverBytesRead = stream.EndRead(ar);
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
        /// Data sender asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        /// <param name="ar">The current asynchronus result.</param>
        private void DataSenderSsl(System.IO.Stream stream, IAsyncResult ar)
        {
            // Finish asynchronous read into readBuffer 
            // and get number of bytes read.
            stream.EndWrite(ar);
        }

        /// <summary>
        /// Socket begin receive
        /// </summary>
        private void SocketBeginReceive()
        {
            _socket.ReceiveTimeout = _timeoutReceive;

            lock (_readLock)
            {
                // Start a new asynchronous read into readBuffer.
                // more data maybe present. This will wait for more
                // data from the client.
                _arReceiver = _socket.BeginReceive(_readBuffer, 0,
                        READ_BUFFER_SIZE, _receiveSocketFlags,
                        new AsyncCallback(DataReceiver), null);
            }
        }

        /// <summary>
        /// Start the current connection receiver.
        /// </summary>
        /// <param name="stream">The network stream.</param>
        private void StartReceiverSsl(System.IO.Stream stream)
        {
            stream.ReadTimeout = _timeoutReceive;

            lock (_readLock)
                // Start a new read thread.
                _arReceiver = stream.BeginRead(_readBuffer, 0,
                    READ_BUFFER_SIZE, new AsyncCallback(DataReceiver), null);
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
            bytesRead = stream.Read(_readBuffer, 0, READ_BUFFER_SIZE);

            // Decode the data in the buffer to a string.
            data = _readBuffer.Take(bytesRead).ToArray();
            return data;
        }

        /// <summary>
        /// In-active timeout setter.
        /// </summary>
        private void InActiveTimeOutSetter()
        {
            // If the time out control has been created
            // then reset the timer.
            if (_timeOut != null)
                _timeOut.Change(
                    new TimeSpan(0, 0, _timeOutInterval),
                    new TimeSpan(0, 0, _timeOutInterval));
        }
        #endregion

        #region Dispose Object Methods

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
                _sslStream = null;
                _networkStream = null;
                _socket = null;
                _timeOut = null;
                _arReceiver = null;
                _arSender = null;

                _readBuffer = null;
                _writeBuffer = null;

                _sendLock = null;
                _readLock = null;

                // The connection has been closed.
                _isClosed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Client()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
