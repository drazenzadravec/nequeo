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
using Nequeo.Net.Provider;

namespace Nequeo.Net.Sockets
{
    /// <summary>
    /// Delegate that handles commands received from the client.
    /// </summary>
    /// <param name="sender">The current object sending the data.</param>
    /// <param name="commandSend">The command data send by the client.</param>
    internal delegate void UdpServerContextHandler(UdpServerContext sender, string commandSend);

    /// <summary>
    /// Delegate that handles commands received from the client.
    /// </summary>
    /// <param name="sender">The current object sending the data.</param>
    /// <param name="commandSend">The command data send by the client.</param>
    internal delegate void UdpSimpleContextHandler(UdpSingleServer.UdpSimpleContext sender, string commandSend);

    /// <summary>
    /// General socket server context provider used for server client connectivity implementation.
    /// </summary>
    internal class UdpServerContext : IUdpServerContext, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="addressFamily">The ip address family.</param>
        public UdpServerContext(System.Net.Sockets.AddressFamily addressFamily)
        {
            _readBuffer = new byte[READ_BUFFER_SIZE];
            _writeBuffer = new byte[WRITE_BUFFER_SIZE];

            // Creates an IpEndPoint to capture the identity of the client.
            if (addressFamily == AddressFamily.InterNetwork)
                _client = new IPEndPoint(IPAddress.Any, 0);

            if (addressFamily == AddressFamily.InterNetworkV6)
                _client = new IPEndPoint(IPAddress.IPv6Any, 0);

            RemoteClient = (EndPoint)_client;
        }
        #endregion

        #region Private Fields
        private int READ_BUFFER_SIZE = 8192;
        private int WRITE_BUFFER_SIZE = 8192;

        private object _sendLock = new object();
        private object _readLock = new object();

        private IUdpServer _socketServer = null;
        internal UdpServer ServerRef = null;
        private string _uniqueIdentifier = string.Empty;
        private string _connectionID = string.Empty;
        private Exception _lastException = null;

        private string _serverName = string.Empty;
        private bool _isInValidationMode = false;
        private object _state = null;

        private IPEndPoint _client = null;
        internal EndPoint RemoteClient = null;
        private System.Net.Sockets.Socket _socket = null;

        private SocketFlags _receiveSocketFlags = SocketFlags.None;
        private SocketFlags _sendSocketFlags = SocketFlags.None;

        internal byte[] _readBuffer = null;
        internal byte[] _writeBuffer = null;
        
        private Action<byte[]> _receiverHandler = null;
        private Action<IUdpServer, IUdpServerContext, byte[]> _sendToServerHandler = null;
        private Action<IUdpServer, IUdpServerContext, byte[]> _sendToServerInfoHandler = null;

        private int _timeoutSend = -1;
        private int _timeoutReceive = -1;
        private int _dataReceiverBytesRead = 0;

        private Timer _timeOut = null;
        private int _timeOutInterval = -1;
        private DateTime _initialTimeOutTime;

        private object _lockReceiver = new object();
        private object _lockSender = new object();
        private object _lockFromServer = new object();

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
        private int _isAsyncModeActive = 0;

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
        /// Gets sets, the timeout (seconds) for each client connection when in-active.
        /// Disconnects the client when this time out is triggered.
        /// </summary>
        public int TimeOut
        {
            get { return _timeOutInterval; }
            set { _timeOutInterval = value; }
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
        internal Action<IUdpServer, IUdpServerContext, byte[]> SendToServerHandler
        {
            get { return _sendToServerHandler; }
            set { _sendToServerHandler = value; }
        }

        /// <summary>
        /// Gets sets, the handler to the server receive data method (use for streaming data, non-command).
        /// </summary>
        internal Action<IUdpServer, IUdpServerContext, byte[]> SendToServerInfoHandler
        {
            get { return _sendToServerInfoHandler; }
            set { _sendToServerInfoHandler = value; }
        }

        /// <summary>
        /// Gets, the current server controller.
        /// </summary>
        public IUdpServer Server
        {
            get { return _socketServer; }
            internal set { _socketServer = value; }
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
        internal event UdpServerContextHandler OnCommandSend;

        #endregion

        #region Public Methods
        /// <summary>
        /// Disconnect the current client and releases all resources.
        /// </summary>
        public void Close()
        {
            Disconnect();
        }

        /// <summary>
        /// Disconnect the current client and releases all resources.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                // Clear the last error.
                ClearLastError();
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
        /// Get the client ip endpoint (remote end point).
        /// </summary>
        /// <returns>The client ip endpoint; else null.</returns>
        public IPEndPoint GetClientIPEndPoint()
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                return (IPEndPoint)RemoteClient;
            }
            catch (Exception ex)
            {
                SetLastError(ex);
                return null;
            }
        }

        /// <summary>
        /// Get the last error that occured.
        /// </summary>
        /// <returns>The last exception.</returns>
        public Exception GetLastError()
        {
            return _lastException;
        }

        /// <summary>
        /// Send data to the client through the server context from the server.
        /// </summary>
        /// <param name="data">The data received from the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void SentFromServer(byte[] data)
        {
            try
            {
                // Only allow one thread at a time
                // to write to the response stream.
                lock (_lockFromServer)
                {
                    // Write the new data to the response stream.
                    _responseStream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialise the context.
        /// </summary>
        internal void Initialise()
        {
            // Create a new context for a new connection.
            CreateContext();

            // Assign context values.
            AssignContext();

            // Set the context and send a message to
            // the server to assign the on connected handler.
            State = _context;
            SendToServerInfo(null);
        }

        /// <summary>
        /// Data received asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="ar">The current asynchronus result.</param>
        internal void DataReceiver(IAsyncResult ar)
        {
            try
            {
                // Clear the last error.
                ClearLastError();

                // Lock the socket receive process.
                lock (ServerRef.LockingSocket)
                {
                    // Get the data from the client endpoint.
                    _dataReceiverBytesRead = _socket.EndReceiveFrom(ar, ref RemoteClient);
                }

                // Send a Received command to the server
                // indicating that the client has data.
                if (OnCommandSend != null)
                    OnCommandSend(this, "RECEIVED");

                // Make sure only one thread at a time is adding to the buffer.
                lock (_lockReceiver)
                {
                    // Make sure data has arrived.
                    if (_dataReceiverBytesRead > 0)
                    {
                        // If the upper capacity of the buffer
                        // has been reached then stop writting
                        // until the request buffer gets to the
                        // lower capacity threshold.
                        if (_requestBuffer.IsUpperCapacityPercentage())
                        {
                            // Create the tasks.
                            Task[] tasks = new Task[1];
                            Interlocked.Exchange(ref _exitWaitReceiveIndicator, 0);

                            try
                            {
                                // Write to the request stream task.
                                Task readFromStream = Task.Factory.StartNew(() =>
                                {
                                    // Create a new spin wait.
                                    SpinWait sw = new SpinWait();

                                    // Action to perform.
                                    while (Interlocked.CompareExchange(ref _exitWaitReceiveIndicator, 0, 1) == 0)
                                    {
                                        // The NextSpinWillYield property returns true if 
                                        // calling sw.SpinOnce() will result in yielding the 
                                        // processor instead of simply spinning. 
                                        if (sw.NextSpinWillYield)
                                        {
                                            // If the buffer is below the lower capacity
                                            // threshold then exist the spin wait.
                                            if (!_requestBuffer.IsLowerCapacityPercentage())
                                                Interlocked.Exchange(ref _exitWaitReceiveIndicator, 1);
                                        }

                                        // Performs a single spin.
                                        sw.SpinOnce();
                                    }
                                });

                                // Assign the listener task.
                                tasks[0] = readFromStream;

                                // Wait for all tasks to complete.
                                Task.WaitAll(tasks);
                            }
                            catch { }

                            // For each task.
                            foreach (Task item in tasks)
                            {
                                try
                                {
                                    // Release the resources.
                                    item.Dispose();
                                }
                                catch { }
                            }
                            tasks = null;
                        }

                        // Write to the request stream.
                        _requestStream.WriteToStream(_readBuffer, 0, _dataReceiverBytesRead);

                        // Set the timeout on first receive.
                        SetTimeOut();

                        if (_context != null)
                            // Assign the new client IP endpoint.
                            _context.RemoteEndPoint = GetClientIPEndPoint();
                    }
                }

                // Make sure data has arrived.
                if (_dataReceiverBytesRead > 0)
                {
                    // Make sure the context exists.
                    if (_context != null)
                    {
                        // If not in async mode.
                        if (!_context.IsAsyncMode)
                        {
                            // Allow an active context.
                            if (Interlocked.CompareExchange(ref _isContextActive, 1, 0) == 0)
                            {
                                // Set the active context indicator to true
                                // no other context can start.
                                Interlocked.Exchange(ref _isContextActive, 1);

                                // Received the request from the client.
                                // Send the message context.
                                Receiver();
                            }
                        }
                        else
                        {
                            // If the data available handler has been set
                            // then send a trigger indicating that 
                            // data is available.
                            if (_context.ReceivedAsyncMode != null)
                            {
                                // Allow an active context.
                                if (Interlocked.CompareExchange(ref _isAsyncModeActive, 1, 0) == 0)
                                {
                                    // Set the active context indicator to true
                                    // no other context can start.
                                    Interlocked.Exchange(ref _isAsyncModeActive, 1);

                                    // Received the request from the client.
                                    // Send the message context.
                                    AsyncModeReceiver();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Received the request from the client.
        /// </summary>
        private async void Receiver()
        {
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    // If a response stream exists.
                    if (_requestStream != null)
                    {
                        try
                        {
                            // If not in async mode.
                            if (!_context.IsAsyncMode)
                            {
                                // If the data available handler has been set
                                // then send a trigger indicating that 
                                // data is available.
                                if (_context.OnReceivedHandler != null)
                                {
                                    // Continue opening the context
                                    // until no more data.
                                    while (_requestStream.Length > 0 && !_context.IsAsyncMode)
                                    {
                                        // If the data available handler has been set
                                        // then send a trigger indicating that 
                                        // data is available.
                                        if (_context.OnReceivedHandler != null)
                                            _context.OnReceivedHandler(_context);
                                    }
                                }
                            }

                            // If in async mode.
                            if (_context.IsAsyncMode)
                            {
                                // If the data available handler has been set
                                // then send a trigger indicating that 
                                // data is available.
                                if (_context.ReceivedAsyncMode != null)
                                {
                                    // Continue opening the context
                                    // until no more data.
                                    while (_requestStream.Length > 0 && _context.IsAsyncMode)
                                    {
                                        // Trigger this handler when data has arrived.
                                        // This is triggered when in async mode is true.
                                        if (_context.ReceivedAsyncMode != null)
                                            _context.ReceivedAsyncMode();
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                });

            await result;

            // Reset the active context indicator.
            // a new context can be created.
            Interlocked.Exchange(ref _isContextActive, 0);
        }

        /// <summary>
        /// Received the request from the client.
        /// </summary>
        private async void AsyncModeReceiver()
        {
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    // If a response stream exists.
                    if (_requestStream != null)
                    {
                        try
                        {
                            // If in async mode.
                            if (_context.IsAsyncMode)
                            {
                                // If the data available handler has been set
                                // then send a trigger indicating that 
                                // data is available.
                                if (_context.ReceivedAsyncMode != null)
                                {
                                    // Continue opening the context
                                    // until no more data.
                                    while (_requestStream.Length > 0 && _context.IsAsyncMode)
                                    {
                                        // Trigger this handler when data has arrived.
                                        // This is triggered when in async mode is true.
                                        if (_context.ReceivedAsyncMode != null)
                                            _context.ReceivedAsyncMode();
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                });

            await result;

            // Reset the active context indicator.
            // a new context can be created.
            Interlocked.Exchange(ref _isAsyncModeActive, 0);
        }
        
        /// <summary>
        /// Create a new context for a new connection.
        /// </summary>
        private void CreateContext()
        {
            // Create the buffers.
            _requestBuffer = new Collections.CircularBuffer<byte>(Server.RequestBufferCapacity);
            _responseBuffer = new Collections.CircularBuffer<byte>(Server.ResponseBufferCapacity);

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
            _context.Port = Server.Port;
            _context.Name = Server.Name;
            _context.ServiceName = Server.ServiceName;
            _context.NumberOfClients = Server.NumberOfClients;
            _context.SocketState = SocketState.Open;
            _context.IsAsyncMode = false;

            // Create the connection ID.
            ConnectionID = Guid.NewGuid().ToString();
            _context.ConnectionID = ConnectionID;

            // Get the session id from the IP Address and port.
            IPEndPoint ip = _context.RemoteEndPoint;
            string ipAddress = ip.Address.ToString();
            string ipPort = ip.Port.ToString();
            _context.SessionID = Nequeo.Cryptography.Hashcode.GetHashcodeSHA1(ipAddress + "_" + ipPort);

            // Assign the structures to the context.
            _context.ContextState = null;
            _context.Request = _request;
            _context.Response = _response;
        }

        /// <summary>
        /// Send the response data to the client.
        /// </summary>
        private void Sender()
        {
            // Make sure only one thread at a time is removing from the buffer.
            lock (_lockSender)
            {
                try
                {
                    // If the upper capacity of the buffer
                    // has been reached then stop writting
                    // until the response buffer gets to the
                    // lower capacity threshold.
                    if (_responseBuffer.IsUpperCapacityPercentage())
                    {
                        // Create the tasks.
                        Task[] tasks = new Task[1];
                        Interlocked.Exchange(ref _exitWaitSendIndicator, 0);

                        try
                        {
                            // Write to the response stream task.
                            Task writeToStream = Task.Factory.StartNew(() =>
                            {
                                // Create a new spin wait.
                                SpinWait sw = new SpinWait();

                                // Action to perform.
                                while (Interlocked.CompareExchange(ref _exitWaitSendIndicator, 0, 1) == 0)
                                {
                                    // The NextSpinWillYield property returns true if 
                                    // calling sw.SpinOnce() will result in yielding the 
                                    // processor instead of simply spinning. 
                                    if (sw.NextSpinWillYield)
                                    {
                                        // Send the data to the client until
                                        // the lower capacity buffer has been
                                        // reached.
                                        SendData();

                                        // If the buffer is below the lower capacity
                                        // threshold then exist the spin wait.
                                        if (!_responseBuffer.IsLowerCapacityPercentage())
                                            Interlocked.Exchange(ref _exitWaitSendIndicator, 1);
                                    }

                                    // Performs a single spin.
                                    sw.SpinOnce();
                                }
                            });

                            // Assign the listener task.
                            tasks[0] = writeToStream;

                            // Wait for all tasks to complete.
                            Task.WaitAll(tasks);
                        }
                        catch { }

                        // For each task.
                        foreach (Task item in tasks)
                        {
                            try
                            {
                                // Release the resources.
                                item.Dispose();
                            }
                            catch { }
                        }
                        tasks = null;
                    }

                    // Send the data to the client.
                    SendData();
                }
                catch { }
            }
        }

        /// <summary>
        /// Send the data to the client.
        /// </summary>
        private void SendData()
        {
            // Get the data from the response stream.
            byte[] buffer = new byte[ReadBufferSize];

            // Send all in buffer until empty.
            while (_responseStream != null && _responseStream.Length > 0)
            {
                // Read the data.
                int byesRead = _responseStream.ReadFromStream(buffer, 0, buffer.Length);

                // Send to client if bytes have been read.
                if (byesRead > 0)
                {
                    // Lock the socket receive process.
                    lock (ServerRef.LockingSocket)
                    {
                        // Send the data back to the client.
                        _socket.SendTo(buffer, 0, byesRead, _sendSocketFlags, RemoteClient);
                    }

                    // If the time out control has been created
                    // then reset the timer.
                    InActiveTimeOutSetter();
                }
            }
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
            Disconnect();
        }

        /// <summary>
        /// The unique itentifier has been set.
        /// </summary>
        /// <param name="uid">The unique identifier.</param>
        private void UniqueIdentifierHasBeenSet(string uid)
        {
            // Set the unique identifier.
            UniqueIdentifier = uid;
        }

        /// <summary>
        /// Send the data to the server.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        private void SendToServer(byte[] data)
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
        private void SendToServerInfo(byte[] data)
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
        /// Get the server ip endpoint (local end point).
        /// </summary>
        /// <returns>The server ip endpoint; else null.</returns>
        private IPEndPoint GetServerIPEndPoint()
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
        /// Set the time out.
        /// </summary>
        private void SetTimeOut()
        {
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
        /// Triggered when all resources are released and parent resources need to be released.
        /// </summary>
        private void DisposeOfResources()
        {
            // Set the web socket state to closed.
            if (_context != null)
                _context.SocketState = SocketState.Closed;

            // Release the receive and send spin wait handler.
            Interlocked.Exchange(ref _exitWaitReceiveIndicator, 0);
            Interlocked.Exchange(ref _exitWaitSendIndicator, 0);
            Interlocked.Exchange(ref _isContextActive, 0);
            Interlocked.Exchange(ref _isAsyncModeActive, 0);

            State = null;

            _lockReceiver = null;
            _lockSender = null;
            _lockFromServer = null;

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

            _requestBuffer = null;
            _responseBuffer = null;

            _requestStream = null;
            _responseStream = null;

            _request = null;
            _response = null;

            _context.ContextState = null;
            _context = null;
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
                    // Triggered when all resources are released and parent resources need to be released.
                    DisposeOfResources();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here
                _sendLock = null;
                _readLock = null;

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
        ~UdpServerContext()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
