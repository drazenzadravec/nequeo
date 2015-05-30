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
using System.Collections.Concurrent;
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
using Nequeo.Extension;

namespace Nequeo.Net.Sockets
{
    /// <summary>
    /// General socket server (multi-client).
    /// </summary>
    public partial class UdpSingleServer : IUdpSingleServer, IDisposable
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
        public UdpSingleServer(IPAddress address, int port)
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

        private int _clientCount = 0;
        private int _maxNumClients = Int32.MaxValue;

        private object _lockingObject = new object();
        internal object LockingSocket = new object();

        private string _serverName = string.Empty;
        private string _serviceName = string.Empty;

        private SocketFlags _receiveSocketFlags = SocketFlags.None;
        private SocketFlags _sendSocketFlags = SocketFlags.None;

        private System.Net.Sockets.Socket _socket = null;
        private bool _isListening = false;

        private System.Net.Sockets.ProtocolType _protocolType = System.Net.Sockets.ProtocolType.Udp;
        private System.Net.Sockets.SocketType _socketType = System.Net.Sockets.SocketType.Dgram;
        private System.Net.Sockets.AddressFamily _specificAddressFamily = AddressFamily.Unspecified;

        private Action<Nequeo.Net.Sockets.IUdpSingleServer, byte[], IPEndPoint> _receivedHandler = null;

        // Looks for connection avaliability.
        private AutoResetEvent _connAvailable =
            new AutoResetEvent(false);

        // Looks for connection avaliability.
        private AutoResetEvent _receivedData =
            new AutoResetEvent(false);

        /// <summary>
        /// False if this server is part of a collection of endpoint servers; else True not part of a collection.
        /// </summary>
        private bool _maxNumClientsIndividualControl = true;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the recevied data handler.
        /// </summary>
        public Action<Nequeo.Net.Sockets.IUdpSingleServer, byte[], IPEndPoint> ReceivedHandler
        {
            get { return _receivedHandler; }
            set { _receivedHandler = value; }
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

                    // Create the server binding and start listening.
                    _socket = new Socket(endPoint.AddressFamily, _socketType, _protocolType);
                    _socket.Bind(endPoint);

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

        /// <summary>
        /// Send the data to the client end point.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="client">The client end point to send to.</param>
        public void SendTo(byte[] data, IPEndPoint client)
        {
            if (data != null && data.Length > 0)
            {
                // Lock the socket receive process.
                lock (LockingSocket)
                {
                    // Get the client end point.
                    EndPoint endpoint = (EndPoint)client;

                    // Send the data back to the client.
                    _socket.SendTo(data, 0, data.Length, _sendSocketFlags, endpoint);
                }
            }
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
                    _receivedData.Reset();

                    // Do not allow any more clients
                    // if maximum is reached.
                    if (_clientCount < _maxNumClients)
                    {
                        // Begins to asynchronously receive data from a specified network device.
                        BeginReceiveFrom();
                        Thread.Sleep(20);

                        // Blocks the current thread.
                        _receivedData.WaitOne();
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
        /// Begins to asynchronously receive data from a specified network device.
        /// </summary>
        private void BeginReceiveFrom()
        {
            // Create a new instance of the server context type.
            CreateServerContext();
        }

        /// <summary>
        /// Create a new instance of the server context type.
        /// </summary>
        private void CreateServerContext()
        {
            try
            {
                // Create a new instance of the server context type
                UdpSimpleContext context = new UdpSimpleContext(_specificAddressFamily);
                context.Socket = _socket;
                context.ReceiveSocketFlags = _receiveSocketFlags;
                context.SendSocketFlags = _sendSocketFlags;
                context.ReadBufferSize = READ_BUFFER_SIZE;
                context.WriteBufferSize = WRITE_BUFFER_SIZE;
                context.Server = this;

                // Create a new client data receive handler, this event
                // handles commands from the current client.
                context.OnCommandSend +=
                    new UdpSimpleContextHandler(SocketServer_OnCommandSend);

                // Increment the count.
                IncrementCount();

                // Lock the socket receive process.
                lock (LockingSocket)
                {
                    // Start receiving data from the client.
                    _socket.BeginReceiveFrom(context._readBuffer, 0, context.ReadBufferSize,
                        context.ReceiveSocketFlags, ref context.RemoteClient,
                        new AsyncCallback(context.DataReceiver), context);
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Processes all in-comming client command requests.
        /// </summary>
        /// <param name="sender">The current server to client connection channel.</param>
        /// <param name="commandSend">The command send by the client context.</param>
        private void SocketServer_OnCommandSend(UdpSimpleContext sender, string commandSend)
        {
            try
            {
                // Clear last error.
                ClearLastError();

                // Process the command.
                switch (commandSend.ToUpper())
                {
                    case "RECEIVED":
                        try
                        {
                            // If receiver handler has been set.
                            if (_receivedHandler != null)
                            {
                                // Send to client.
                                _receivedHandler(this, sender._readBuffer.Take(sender.DataReceiverBytesRead).ToArray(), sender.Client);
                            }
                        }
                        catch { }

                        // Data has been received.
                        DataReceived();
                        break;
                }
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }
        }

        /// <summary>
        /// Data has been received.
        /// </summary>
        private void DataReceived()
        {
            // Set the current client count.
            DecrementCount();

            // Signal to the blocking handler
            // to un-block.
            lock (_lockingObject)
                _connAvailable.Set();

            // Server has received data from client.
            // Signal to the blocking handler
            // to un-block.
            lock (_lockingObject)
                _receivedData.Set();
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

        #region Internal UDP connection context.
        /// <summary>
        /// Udp simple client context.
        /// </summary>
        internal class UdpSimpleContext
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="addressFamily">The ip address family.</param>
            public UdpSimpleContext(System.Net.Sockets.AddressFamily addressFamily)
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

            private int READ_BUFFER_SIZE = 8192;
            private int WRITE_BUFFER_SIZE = 8192;

            private IPEndPoint _client = null;
            internal EndPoint RemoteClient = null;
            private System.Net.Sockets.Socket _socket = null;

            private SocketFlags _receiveSocketFlags = SocketFlags.None;
            private SocketFlags _sendSocketFlags = SocketFlags.None;

            internal UdpSingleServer Server;
            private int _dataReceiverBytesRead = 0;

            internal byte[] _readBuffer = null;
            internal byte[] _writeBuffer = null;

            /// <summary>
            /// Gets, the total data received.
            /// </summary>
            public int DataReceiverBytesRead
            {
                get { return _dataReceiverBytesRead; }
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
            /// Gets the remote client.
            /// </summary>
            public IPEndPoint Client
            {
                get { return (IPEndPoint)RemoteClient; }
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
            /// The on command send event handler.
            /// </summary>
            internal event UdpSimpleContextHandler OnCommandSend;

            /// <summary>
            /// Data received asynchronus result method, all client commands
            /// are processed through this asynchronus result method.
            /// </summary>
            /// <param name="ar">The current asynchronus result.</param>
            internal void DataReceiver(IAsyncResult ar)
            {
                try
                {

                    // Lock the socket receive process.
                    lock (Server.LockingSocket)
                    {
                        // Get the data from the client endpoint.
                        _dataReceiverBytesRead = _socket.EndReceiveFrom(ar, ref RemoteClient);
                    }

                    // Send a Received command to the server
                    // indicating that the client has data.
                    if (OnCommandSend != null)
                        OnCommandSend(this, "RECEIVED");
                }
                catch { }
            }
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

                    if (_receivedData != null)
                        _receivedData.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _socket = null;
                _connAvailable = null;
                _receivedData = null;
                _hostThread = null;
                _lockingObject = null;
                LockingSocket = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~UdpSingleServer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
