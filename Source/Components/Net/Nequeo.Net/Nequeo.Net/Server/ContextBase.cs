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
using System.Collections.Specialized;
using System.Reflection;

using Nequeo.Handler;
using Nequeo.Net;
using Nequeo.Net.Provider;

namespace Nequeo.Net
{
    /// <summary>
    /// Custom server context base provider.
    /// </summary>
    public abstract class CustomContext : ContextBase
    {
        /// <summary>
        /// Custom server context base provider.
        /// </summary>
        public CustomContext() { }

        private Action<Context> _onReceivedHandler = null;

        /// <summary>
        /// On web context received.
        /// </summary>
        /// <param name="context">The web context.</param>
        protected abstract void OnWebContext(WebContext context);

        /// <summary>
        /// Gets or sets the on received handler.
        /// </summary>
        protected override Action<Context> OnReceivedHandler
        {
            get
            {
                _onReceivedHandler = (context) => OnReceived(context);
                return _onReceivedHandler;
            }
        }

        /// <summary>
        /// On received.
        /// </summary>
        /// <param name="context">The base context.</param>
        private void OnReceived(Context context)
        {
            // Create the context.
            WebContext webContext = base.CreateWebContext(context);
            OnWebContext(webContext);
        }
    }

    /// <summary>
    /// Server context base provider.
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public abstract class ContextBase : Nequeo.Net.Sockets.ServerContext
    {
        /// <summary>
        /// Http socket provider server context.
        /// </summary>
        protected ContextBase() { }

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

        private int _timeOut = 0;
        private int _timeoutCallback = 40000;
        private bool _isAuthenticated = false;

        // 0 for false, 1 for true.
        private int _exitWaitReceiveIndicator = 0;
        private int _exitWaitSendIndicator = 0;
        private int _isContextActive = 0;
        private int _isAsyncModeActive = 0;

        /// <summary>
        /// Gets or sets the on received handler. 
        /// </summary>
        protected abstract Action<Nequeo.Net.Provider.Context> OnReceivedHandler { get; }

        /// <summary>
        /// Create the web context.
        /// </summary>
        /// <param name="context">The current provider context.</param>
        /// <returns>The web context.</returns>
        protected WebContext CreateWebContext(Nequeo.Net.Provider.Context context)
        {
            // If the context has not been created.
            if (context.ContextState == null)
            {
                // Create the new context.
                Nequeo.Net.WebContext webContext = new Nequeo.Net.WebContext(context);

                // Assign the current context.
                webContext.IsSecureConnection = context.IsSecureConnection;
                webContext.IsSslAuthenticated = context.IsSslAuthenticated;
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

                // Create the response and request objects.
                webContext.WebResponse = Nequeo.Net.WebResponse.Create(context.Response.Output);
                webContext.WebRequest = Nequeo.Net.WebRequest.Create(context.Request.Input);

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
        /// The remote client has been authenticated with the credentials
        /// and allowed to communicate.
        /// </summary>
        protected override void Authenticated()
        {
            // Set the original timeout.
            base.IsInValidationMode = false;
            base.TimeOut = _timeOut;
            _isAuthenticated = true;
        }

        /// <summary>
        /// Validate a new connection.
        /// </summary>
        /// <returns>True if the connection is valid and allowed; else false.</returns>
        protected override bool BeginConnect()
        {
            // Create a new context for a new connection.
            CreateContext();

            // Get the current time.
            _timeOut = base.TimeOut;

            // Assign context values.
            AssignContext();

            // Set the context and send a message to
            // the server to assign the on connected handler.
            base.State = _context;
            base.SendToServerInfo(null);

            // Allow the connection.
            return true;
        }

        /// <summary>
        /// Start an SSL server authentication.
        /// </summary>
        /// <returns>True if the authentication is started with a new connection; else false.</returns>
        protected override bool BeginSslAuthentication()
        {
            return base.BeginSslAuthenticate;
        }

        /// <summary>
        /// End of the SSL authentication started with begin SSL authentication.
        /// </summary>
        protected override void EndSslAuthentication()
        {
            // Assign the ssl authentication level.
            if (_context != null)
                _context.IsSslAuthenticated = base.IsSslAuthenticated;
        }

        /// <summary>
        /// This server context to the client has ended.
        /// </summary>
        protected override void Disconnected()
        {
            // Do not allow any operation to linger when disconnecting.
            if (_isAuthenticated)
            {
                IAsyncResult ar = null;
                Threading.ActionHandler callback = null;

                try
                {
                    // Create a new function callback delegate.
                    callback = new Threading.ActionHandler(Disconnecting);

                    // Start the async operation.
                    ar = callback.BeginInvoke(null, null);

                    // If the operation has not timed out.
                    if (ar.AsyncWaitHandle.WaitOne(_timeoutCallback, false))
                        // Return the host entry object.
                        callback.EndInvoke(ar);
                }
                catch { }
                finally
                {
                    if (callback != null)
                        callback = null;

                    if (ar != null)
                        ar.AsyncWaitHandle.Dispose();
                }
            }
        }

        /// <summary>
        /// Triggered when all resources are released and parent resources need to be released.
        /// </summary>
        protected override void DisposeOfResources()
        {
            // Set the web socket state to closed.
            if (_context != null)
                _context.SocketState = SocketState.Closed;

            // Release the receive and send spin wait handler.
            Interlocked.Exchange(ref _exitWaitReceiveIndicator, 0);
            Interlocked.Exchange(ref _exitWaitSendIndicator, 0);
            Interlocked.Exchange(ref _isContextActive, 0);
            Interlocked.Exchange(ref _isAsyncModeActive, 0);

            base.State = null;

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

            _requestBuffer = null;
            _responseBuffer = null;

            _requestStream = null;
            _responseStream = null;

            _request = null;
            _response = null;

            _context.State = null;
            _context.ContextState = null;
            _context = null;
        }

        /// <summary>
        /// The connection to the client has been established.
        /// </summary>
        /// <param name="exception">The exception is any.</param>
        protected override void EndConnect(Exception exception)
        {
            LogError(exception, MethodInfo.GetCurrentMethod());

            // Set the time out to 20 seconds, the current client has 20
            // seconds to to start communicating. This is used for DOS attacks,
            // the client connection is not allowed to linger.
            base.IsInValidationMode = true;
            base.TimeOut = 20;

            // Indicate that the remote host has been 
            // authenticated with the correct credentials.
            base.SetAuthenticated();
        }

        /// <summary>
        /// The receive data method call.
        /// </summary>
        /// <param name="data">The data received from the client.</param>
        protected override void Receive(byte[] data)
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
                    _requestStream.WriteToStream(data, 0, data.Length);
                }
            }

            // Make sure data has arrived.
            if (data.Length > 0)
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

        /// <summary>
        /// The receive data from the server method call.
        /// </summary>
        /// <param name="data">The data received from the server.</param>
        protected override void ReceiveFromServer(byte[] data)
        {
            // Only allow one thread at a time
            // to write to the response stream.
            lock (_lockFromServer)
            {
                // Write the new data to the response stream.
                _responseStream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Triggers when the receiver has completed sending data.
        /// </summary>
        protected override void ReceiverComplete() { }

        /// <summary>
        /// Triggers when the sender has completed sending data.
        /// </summary>
        protected override void SenderComplete() { }

        /// <summary>
        /// The client is being disconnected.
        /// </summary>
        private void Disconnecting() { }

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
            byte[] buffer = new byte[base.ReadBufferSize];

            // Send all in buffer until empty.
            while (_responseStream != null && _responseStream.Length > 0)
            {
                // Read the data.
                int byesRead = _responseStream.ReadFromStream(buffer, 0, buffer.Length);

                // Send to client if bytes have been read.
                if (byesRead > 0)
                {
                    // Send the data back to the client.
                    base.Send(buffer.Take(byesRead).ToArray());
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
            base.Disconnect();
        }

        /// <summary>
        /// The unique itentifier has been set.
        /// </summary>
        /// <param name="uid">The unique identifier.</param>
        private void UniqueIdentifierHasBeenSet(string uid)
        {
            // Set the unique identifier.
            base.UniqueIdentifier = uid;
        }

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        /// <param name="tlsNegotiationCommand">Send a TLS negotiation command (e.g. OK Begin TLS negotiation now) if not null.</param>
        /// <returns>True if authentication has started; else false.</returns>
        /// <remarks>This is generally used for TLS protcol.</remarks>
        private bool StartTls(string tlsNegotiationCommand = null)
        {
            base.UseSslConnection = true;
            if (_context != null)
                _context.IsSecureConnection = base.UseSslConnection;

            // Using ssl connection
            if (base.UseSslConnection && base.SocketType == System.Net.Sockets.SocketType.Stream)
            {
                // Begin the secure negotiation and server authentication.
                return base.BeginTlsNegotiation(tlsNegotiationCommand);
            }
            else
            {
                // Do not allow secure connection.
                return false;
            }
        }

        /// <summary>
        /// Create a new context for a new connection.
        /// </summary>
        private void CreateContext()
        {
            // Create the buffers.
            _requestBuffer = new Collections.CircularBuffer<byte>(base.Server.RequestBufferCapacity);
            _responseBuffer = new Collections.CircularBuffer<byte>(base.Server.ResponseBufferCapacity);

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
            _context.RemoteEndPoint = base.GetClientIPEndPoint();
            _context.ServerEndPoint = base.GetServerIPEndPoint();
            _context.IsSecureConnection = base.UseSslConnection;
            _context.IsSslAuthenticated = base.IsSslAuthenticated;
            _context.Port = base.Server.Port;
            _context.Name = base.Server.Name;
            _context.ServiceName = base.Server.ServiceName;
            _context.NumberOfClients = base.Server.NumberOfClients;
            _context.SocketState = SocketState.Open;
            _context.IsAsyncMode = false;

            // Create the connection ID.
            base.ConnectionID = Guid.NewGuid().ToString();
            _context.ConnectionID = base.ConnectionID;

            // Get the session id from the IP Address and port.
            IPEndPoint ip = _context.RemoteEndPoint;
            string ipAddress = ip.Address.ToString();
            string ipPort = ip.Port.ToString();
            _context.SessionID = Nequeo.Cryptography.Hashcode.GetHashcodeSHA1(ipAddress + "_" + ipPort);

            // Assign the structures to the context.
            _context.ContextState = null;
            _context.Request = _request;
            _context.Response = _response;

            // Get the receive data handlers.
            _context.OnReceivedHandler = OnReceivedHandler;
        }

        /// <summary>
        /// Log the error.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="member">The member info where the error occured.</param>
        private void LogError(Exception ex, MemberInfo member)
        {
            if (ex != null)
            {
                // If an event application name exists for logging.
                if (!String.IsNullOrEmpty(Helper.EventApplicationName))
                {
                    // Log the error.
                    LogHandler.WriteTypeMessage(ex.Message,
                        member, Helper.EventApplicationName);
                }
            }
        }
    }
}
