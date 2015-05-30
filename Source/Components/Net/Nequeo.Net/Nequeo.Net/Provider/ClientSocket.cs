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

using Nequeo.Model;
using Nequeo.Handler;
using Nequeo.Net.Configuration;

namespace Nequeo.Net.Provider
{
    /// <summary>
    /// Client socket provider.
    /// </summary>
    public abstract class ClientSocket : Nequeo.Net.Sockets.Client, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Client socket provider.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        protected ClientSocket()
            : base("localhost", 80)
        {
            Initialise();
        }

        /// <summary>
        /// Client socket provider.
        /// </summary>
        /// <param name="address">An IP address.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        protected ClientSocket(IPAddress address, int port)
            : base(address, port)
        {
            Initialise();
        }

        /// <summary>
        /// Socket client constructor.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        protected ClientSocket(string hostNameOrAddress, int port)
            : base(hostNameOrAddress, port)
        {
            Initialise();
        }
        #endregion

        #region Private Fields
        private string _remoteHostPrefix = "";
        private AutoResetEvent _dataAvailable = new AutoResetEvent(false);

        private string _remoteTypePortName = string.Empty;
        private string _remoteConfigName = "RemoteHost";
        private string _remoteSslConfigName = "RemoteHostSsl";
        private Net.Configuration.Reader _remoteHostReader = null;

        private int _maxReadLength = 0;
        private int _requestBufferCapacity = 10000000;
        private int _responseBufferCapacity = 10000000;
        private Action<Nequeo.Net.Provider.ClientContext> _onReceivedActionHandler = null;

        private object _lockReceiver = new object();
        private object _lockSender = new object();

        private Nequeo.Collections.CircularBuffer<byte> _requestBuffer = null;
        private Nequeo.Collections.CircularBuffer<byte> _responseBuffer = null;

        private NetRequestStream _requestStream = null;
        private NetResponseStream _responseStream = null;

        // Create the context structures.
        private Request _request = null;
        private Response _response = null;
        private ClientContext _context = null;

        // 0 for false, 1 for true.
        private int _exitWaitReceiveIndicator = 0;
        private int _exitWaitSendIndicator = 0;
        private int _isContextActive = 0;

        private long _headerTimeout = -1;
        private int _requestTimeout = -1;
        private int _responseTimeout = -1;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets on received action handler, new data has arrived. Should be used when implementing constant data arrivals.
        /// </summary>
        public Action<Nequeo.Net.Provider.ClientContext> OnReceivedHandler
        {
            get { return _onReceivedActionHandler; }
            set { _onReceivedActionHandler = value; }
        }

        /// <summary>
        /// Gets or sets the remote host prefix, this is only used with the configuration file.
        /// </summary>
        public string RemoteHostPrefix
        {
            get { return _remoteHostPrefix; }
            set { _remoteHostPrefix = value; }
        }

        /// <summary>
        /// Gets the remote configuration type port name.
        /// </summary>
        public string RemoteTypePortName
        {
            get { return _remoteTypePortName; }
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
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialise the client, use this method when loading from the configuration file.
        /// </summary>
        public virtual void Initialisation()
        {
            // If not connected.
            if (!base.Connected)
            {
                // Get the secure connection indicator.
                if (base.UseSslConnection)
                {
                    // Get secure connection data.
                    base.HostNameOrAddress = _remoteHostReader.GetRemoteHost(_remoteHostPrefix + _remoteSslConfigName).Host;
                    base.Port = _remoteHostReader.GetRemoteHost(_remoteHostPrefix + _remoteSslConfigName).Port;
                    _remoteTypePortName = _remoteHostPrefix + _remoteSslConfigName;
                }
                else
                {
                    // Get connection data.
                    base.HostNameOrAddress = _remoteHostReader.GetRemoteHost(_remoteHostPrefix + _remoteConfigName).Host;
                    base.Port = _remoteHostReader.GetRemoteHost(_remoteHostPrefix + _remoteConfigName).Port;
                    _remoteTypePortName = _remoteHostPrefix + _remoteConfigName;
                }
            }
        }

        /// <summary>
        /// Connect to the host socket.
        /// </summary>
        public override void Connect()
        {
            // If not connected.
            if (!base.Connected)
            {
                // Assign context values.
                AssignContext();

                // Connect to the remote host.
                base.Connect();

                // Assign the state.
                if (base.Connected)
                {
                    // Set the connection state.
                    if (_context != null)
                    {
                        // Connection is open.
                        _context.SocketState = SocketState.Open;

                        // If connected then assign the context connection ID.
                        _context.ConnectionID = base.ConnectionID;
                    }
                }

                // Start the constant receiver.
                if (base.Connected)
                    base.StartReceiver();
            }
        }

        /// <summary>
        /// Send data to the host.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public override void Send(byte[] data)
        {
            // Write the new data to the request stream.
            _requestStream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Send data to the host.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public override void Send(string data)
        {
            // Write the data to the request stream.
            byte[] buffer = Encoding.Default.GetBytes(data);
            Send(buffer);
        }

        /// <summary>
        /// Receive data from the host.
        /// </summary>
        /// <returns>The data received; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public override string Read()
        {
            byte[] buffer = new byte[base.ReadBufferSize];
            int bytesRead = _responseStream.Read(buffer, 0, base.ReadBufferSize);
            if (buffer != null && buffer.Length > 0)
            {
                // Return the data stored in the buffer.
                return Encoding.Default.GetString(buffer, 0, bytesRead);
            }
            else
                return null;
        }

        /// <summary>
        /// Receive data from the host.
        /// </summary>
        /// <returns>The data received; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public override byte[] Receive()
        {
            byte[] buffer = new byte[base.ReadBufferSize];
            int bytesRead = _responseStream.Read(buffer, 0, base.ReadBufferSize);
            if (buffer != null && buffer.Length > 0)
            {
                // Return the data stored in the buffer.
                return buffer;
            }
            else
                return null;
        }

        /// <summary>
        /// Start the current connection receiver.
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        public override void StartReceiver()
        {
            base.StartReceiver();
        }

        /// <summary>
        /// Stop the current connection receiver.
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        public override void StopReceiver()
        {
            base.StopReceiver();
        }

        /// <summary>
        /// Start the current connection sender.
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        public override void StartSender()
        {
            base.StartSender();
        }

        /// <summary>
        /// Close the current socket connection and disposes of all resources.
        /// </summary>
        public override void Close()
        {
            // Set the connection state.
            if (_context != null)
                _context.SocketState = SocketState.Closed;

            base.Close();
        }

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        /// <param name="continueNegotiation">Continue the negotiation handler. After sending the command, wait for a 
        /// response. If the response is correct then continue the negotiation process.</param>
        /// <param name="tlsNegotiationCommand">Send a TLS negotiation command (e.g. STARTTLS) if not null.</param>
        /// <returns>True if authentication has started; else false.</returns>
        /// <remarks>This is generally used for TLS protcol.</remarks>
        public override bool BeginTlsNegotiation(Func<bool> continueNegotiation, string tlsNegotiationCommand = "STARTTLS\r\n")
        {
            base.UseSslConnection = true;
            return base.BeginTlsNegotiation(continueNegotiation, tlsNegotiationCommand);
        }

        /// <summary>
        /// Get the current web context.
        /// </summary>
        /// <returns>The current web context.</returns>
        public virtual NetContext GetContext()
        {
            return CreateNetContext(_context);
        }

        /// <summary>
        /// Get a new web request stream.
        /// </summary>
        /// <returns>The web request stream.</returns>
        public virtual NetRequest GetRequest()
        {
            return Nequeo.Net.NetRequest.Create(_context.Request.Output);
        }

        /// <summary>
        /// Get a new web request stream.
        /// </summary>
        /// <param name="requestUri">The request uri.</param>
        /// <returns>The web request stream.</returns>
        public virtual NetRequest GetRequest(Uri requestUri)
        {
            return Nequeo.Net.NetRequest.Create(_context.Request.Output, requestUri);
        }

        /// <summary>
        /// Get a new web response stream.
        /// </summary>
        /// <returns>The web response stream.</returns>
        public virtual NetResponse GetResponse()
        {
            return Nequeo.Net.NetResponse.Create(_context.Response.Input);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Create the web context.
        /// </summary>
        /// <param name="context">The current client context.</param>
        /// <returns>The net context.</returns>
        protected virtual Nequeo.Net.NetContext CreateNetContext(Nequeo.Net.Provider.ClientContext context)
        {
            // If the context has not been created.
            if (context.ContextState == null)
            {
                // Create the new context.
                Nequeo.Net.NetContext netContext = new Nequeo.Net.NetContext();

                // Assign the current context.
                netContext.IsSecureConnection = context.IsSecureConnection;
                netContext.Port = context.Port;
                netContext.ServiceName = context.ServiceName;
                netContext.UniqueIdentifier = context.UniqueIdentifier;
                netContext.IsAuthenticated = false;
                netContext.IsStartOfConnection = true;
                netContext.SocketState = context.SocketState;
                netContext.IsAsyncMode = context.IsAsyncMode;
                netContext.ConnectionID = context.ConnectionID;

                // Assign the request input stream and response output stream.
                AssignRequestResponseStreams(context, netContext);

                // Assign the current context.
                context.ContextState = netContext;
            }
            else
            {
                // Get the current context.
                Nequeo.Net.NetContext netContext = (Nequeo.Net.NetContext)context.ContextState;
                netContext.ServiceName = context.ServiceName;
                netContext.UniqueIdentifier = context.UniqueIdentifier;
                netContext.IsAuthenticated = context.IsAuthenticated;
                netContext.IsStartOfConnection = false;

                // Assign the current context.
                context.ContextState = netContext;
            }

            // Return the request context.
            return (Nequeo.Net.NetContext)context.ContextState;
        }

        /// <summary>
        /// Assign the request output stream and response input stream.
        /// </summary>
        /// <param name="context">The current server context.</param>
        /// <param name="netContext">The current web context.</param>
        protected virtual void AssignRequestResponseStreams(Nequeo.Net.Provider.ClientContext context, Nequeo.Net.NetContext netContext)
        {
            // Create the response and request objects.
            netContext.NetResponse = Nequeo.Net.NetResponse.Create(context.Response.Input);
            netContext.NetRequest = Nequeo.Net.NetRequest.Create(context.Request.Output);
        }

        /// <summary>
        /// Save the web context state objects.
        /// </summary>
        /// <param name="context">The current server context.</param>
        /// <param name="netContext">The current web context.</param>
        protected virtual void SaveNetContext(Nequeo.Net.Provider.ClientContext context, Nequeo.Net.NetContext netContext)
        {
            // Assign the state objects.
            context.UniqueIdentifier = netContext.UniqueIdentifier;
            context.ServiceName = netContext.ServiceName;
            context.IsAuthenticated = netContext.IsAuthenticated;
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
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialise
        /// </summary>
        private void Initialise()
        {
            // Create a new context for a new connection.
            CreateContext();

            // Assign the auto receiver.
            base.ReceiveActionHandler = (byte[] data) => ReceiveAction(data);
            base.SocketActionType = Net.SocketClientActionType.ActionHandler;
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
            _requestStream = new NetRequestStream(_requestBuffer);
            _responseStream = new NetResponseStream(_responseBuffer);

            _request = new Request();
            _response = new Response();
            _context = new ClientContext();
        }

        /// <summary>
        /// Assign context values.
        /// </summary>
        private void AssignContext()
        {
            // Assign the response read hander.
            _requestStream.ReadStreamActionHandler = () => Sender();
            _requestStream.CloseActionHandler = () => CloseContext();
            _context.OnReceivedHandler = _onReceivedActionHandler;
            _context.CloseActionHandler = () => CloseContext();

            // Assign the request streams.
            _request.Output = _requestStream;

            // Assign the response streams.
            _response.Input = _responseStream;

            // Assign the context.
            _context.RequestStream = _requestStream;
            _context.ResponseStream = _responseStream;
            _context.IsSecureConnection = base.UseSslConnection;
            _context.Port = base.Port;
            _context.SocketState = SocketState.Closed;
            _context.IsAsyncMode = false;

            // Assign the structures to the context.
            _context.ContextState = null;
            _context.Request = _request;
            _context.Response = _response;
        }

        /// <summary>
        /// Close the connection and release all resources used for communication.
        /// </summary>
        /// <remarks>Call from the context.</remarks>
        private void CloseContext()
        {
            // Set the connection state.
            if (_context != null)
                _context.SocketState = SocketState.Closed;

            // Disconnect the current client and releases all resources.
            Close();
        }
        #endregion

        #region Private Receive and Send Methods
        /// <summary>
        /// The auto receiver action handler.
        /// </summary>
        /// <param name="data">The data from the server.</param>
        private void ReceiveAction(byte[] data)
        {
            // Make sure only one thread at a time is adding to the buffer.
            lock (_lockReceiver)
            {
                // Make sure data has arrived.
                if (data.Length > 0)
                {
                    // If the upper capacity of the buffer
                    // has been reached then stop writting
                    // until the request buffer gets to the
                    // lower capacity threshold.
                    if (_responseBuffer.IsUpperCapacityPercentage())
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
                                        if (!_responseBuffer.IsLowerCapacityPercentage())
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

                    // Write to the response stream.
                    _responseStream.WriteToStream(data, 0, data.Length);
                }
            }

            // Make sure data has arrived.
            if (data.Length > 0)
            {
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
                    }
                }
            }
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
                    if (_requestBuffer.IsUpperCapacityPercentage())
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
                                        if (!_requestBuffer.IsLowerCapacityPercentage())
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
        /// Received the request from the client.
        /// </summary>
        private async void Receiver()
        {
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    // If a response stream exists.
                    if (_responseStream != null)
                    {
                        try
                        {
                            // Continue onpening the context
                            // until no more data.
                            while (_responseStream.Length > 0 && !_context.IsAsyncMode)
                            {
                                // If the data available handler has been set
                                // then send a trigger indicating that 
                                // data is available.
                                if (_context.OnReceivedHandler != null)
                                    _context.OnReceivedHandler(_context);
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
        /// Send the data to the client.
        /// </summary>
        private void SendData()
        {
            // Get the data from the response stream.
            byte[] buffer = new byte[base.ReadBufferSize];

            // Send all in buffer until empty.
            while (_requestStream != null && _requestStream.Length > 0)
            {
                // Write the data.
                int byesRead = _requestStream.ReadFromStream(buffer, 0, buffer.Length);

                // Send to client if bytes have been read.
                if (byesRead > 0)
                {
                    // Send the data back to the client.
                    base.Send(buffer.Take(byesRead).ToArray());
                }
            }
        }
        #endregion

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

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
                    if (_dataAvailable != null)
                        _dataAvailable.Dispose();

                    // Release the receive and send spin wait handler.
                    Interlocked.Exchange(ref _exitWaitReceiveIndicator, 0);
                    Interlocked.Exchange(ref _exitWaitSendIndicator, 0);
                    Interlocked.Exchange(ref _isContextActive, 0);

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
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _dataAvailable = null;
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
        #endregion
    }
}
