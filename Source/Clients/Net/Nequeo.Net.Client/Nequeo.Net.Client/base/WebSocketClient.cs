/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.WebSockets;

namespace Nequeo.Net
{
    /// <summary>
    /// Web socket client.
    /// </summary>
    public class WebSocketClient : IDisposable
    {
        /// <summary>
        /// Web socket client.
        /// </summary>
        public WebSocketClient()
        {
            _client = new Nequeo.Net.WebSockets.WebSocketClient();
            _client.SentAsyncSignal = _sentAsync;
            _client.ReceivedAsyncSignal = _receiveAsync;
            _client.OnConnected += _client_OnConnected;
            _client.OnDisconnected += _client_OnDisconnected;
        }

        /// <summary>
        /// Web socket client.
        /// </summary>
        /// <param name="uri">The resource to connect to.</param>
        public WebSocketClient(Uri uri)
        {
            _client = new Nequeo.Net.WebSockets.WebSocketClient(uri);
            _client.SentAsyncSignal = _sentAsync;
            _client.ReceivedAsyncSignal = _receiveAsync;
            _client.OnConnected += _client_OnConnected;
            _client.OnDisconnected += _client_OnDisconnected;
        }

        /// <summary>
        /// Web socket client.
        /// </summary>
        /// <param name="host">The remote host.</param>
        /// <param name="useSslConnection">Use a secure connection.</param>
        /// <param name="path">The remote host path.</param>
        public WebSocketClient(string host, bool useSslConnection = false, string path = "/")
        {
            _client = new Nequeo.Net.WebSockets.WebSocketClient(host, useSslConnection, path);
            _client.SentAsyncSignal = _sentAsync;
            _client.ReceivedAsyncSignal = _receiveAsync;
            _client.OnConnected += _client_OnConnected;
            _client.OnDisconnected += _client_OnDisconnected;
        }

        private Nequeo.Net.WebSockets.WebSocketClient _client = null;
        private AutoResetEvent _sentAsync = new AutoResetEvent(false);
        private AutoResetEvent _receiveAsync = new AutoResetEvent(false);
        private object _syncSent = new object();
        private object _syncReceive = new object();

        /// <summary>
        /// On connected event handler.
        /// </summary>
        public event EventHandler OnConnected;

        /// <summary>
        /// On disconnected event handler.
        /// </summary>
        public event EventHandler OnDisconnected;

        /// <summary>
        /// Gets or sets the on receive handler.
        /// </summary>
        public Action<byte[], System.Net.WebSockets.WebSocketReceiveResult> OnReceive
        {
            get { return _client.OnReceive; }
            set { _client.OnReceive = value; }
        }

        /// <summary>
        /// Gets or sets the receive cancellation token.
        /// </summary>
        public CancellationToken? ReceiveCancellationToken
        {
            get { return _client.ReceiveCancellationToken; }
            set { _client.ReceiveCancellationToken = value; }
        }

        /// <summary>
        /// Gets or sets the close cancellation token.
        /// </summary>
        public CancellationToken? CloseCancellationToken
        {
            get { return _client.CloseCancellationToken; }
            set { _client.CloseCancellationToken = value; }
        }

        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        public Uri Uri
        {
            get { return _client.Uri; }
            set { _client.Uri = value; }
        }

        /// <summary>
        /// Gets is the client connected.
        /// </summary>
        public bool IsConnected
        {
            get { return _client.IsConnected; }
        }

        /// <summary>
        /// Gets the client web socket client options.
        /// </summary>
        public ClientWebSocketOptions Options
        {
            get { return _client.Options; }
        }

        /// <summary>
        /// Gets the web socket state.
        /// </summary>
        public WebSocketState State
        {
            get { return _client.State; }
        }

        /// <summary>
        /// Gets or sets remote host.
        /// </summary>
        public string Host
        {
            get { return _client.Host; }
            set { _client.Host = value; }
        }

        /// <summary>
        /// Gets or sets remote host path.
        /// </summary>
        public string Path
        {
            get { return _client.Path; }
            set { _client.Path = value; }
        }

        /// <summary>
        /// Gets or sets use a secure connection.
        /// </summary>
        public bool UseSslConnection
        {
            get { return _client.UseSslConnection; }
            set { _client.UseSslConnection = value; }
        }

        /// <summary>
        /// Gets or sets remote host port.
        /// </summary>
        public int Port
        {
            get { return _client.Port; }
            set { _client.Port = value; }
        }

        /// <summary>
        /// Gets or sets the receive chunk size.
        /// </summary>
        public int ReceiveSize
        {
            get { return _client.ReceiveSize; }
            set { _client.ReceiveSize = value; }
        }

        /// <summary>
        /// Gets or sets the send chunk size.
        /// </summary>
        public int SendSize
        {
            get { return _client.SendSize; }
            set { _client.SendSize = value; }
        }

        /// <summary>
        /// Gets the web socket close status.
        /// </summary>
        public WebSocketCloseStatus? CloseStatus
        {
            get { return _client.CloseStatus; }
        }

        /// <summary>
        /// Gets the web socket close status description.
        /// </summary>
        public string CloseStatusDescription
        {
            get { return _client.CloseStatusDescription; }
        }

        /// <summary>
        /// Gets the web socket sub-protocol.
        /// </summary>
        public string SubProtocol
        {
            get { return _client.SubProtocol; }
        }

        /// <summary>
        /// Make a websocket request.
        /// </summary>
        /// <param name="uri">The resource.</param>
        /// <param name="receiveHandler">The receive data handler.</param>
        /// <param name="headers">The header collection to add to the request.</param>
        /// <param name="onConnected">The on connected handler.</param>
        /// <param name="onDisconnected">The on disconnected handler.</param>
        /// <returns>The websocket client.</returns>
        public static Nequeo.Net.WebSocketClient Request(
            Uri uri, Action<byte[], System.Net.WebSockets.WebSocketReceiveResult> receiveHandler,
            Nequeo.Model.NameValue[] headers = null, EventHandler onConnected = null, EventHandler onDisconnected = null)
        {
            // Create a new websocket client.
            Nequeo.Net.WebSocketClient client = new WebSocketClient(uri);
            client.OnReceive = receiveHandler;
            client.OnConnected = onConnected;
            client.OnDisconnected = onDisconnected;

            // If headers exist.
            if (headers != null && headers.Length > 0)
            {
                // For each header.
                foreach (Nequeo.Model.NameValue header in headers)
                    client.Options.SetRequestHeader(header.Name, header.Value);
            }

            // Make a connection.
            client.Connect();

            // Return the client.
            return client;
        }

        /// <summary>
        /// Begins an asynchronous connection request.
        /// </summary>
        public void Connect()
        {
            Connect(CancellationToken.None);
        }

        /// <summary>
        /// Begins an asynchronous connection request.
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        public async void Connect(CancellationToken cancellationToken)
        {
            Exception exception = null;
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        // Make a connection.
                        _client.Connect(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    
                });
            await result;

            // If error the throw to the client.
            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// Close connection asynchronously.
        /// </summary>
        public async void Close()
        {
            Exception exception = null;
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        if (_sentAsync != null)
                            _sentAsync.Dispose();

                        if (_receiveAsync != null)
                            _receiveAsync.Dispose();

                        _sentAsync = null;
                        _receiveAsync = null;

                        _client.SentAsyncSignal = _sentAsync;
                        _client.ReceivedAsyncSignal = _receiveAsync;

                        // Close a connection.
                        _client.Close();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }

                });
            await result;

            // If error the throw to the client.
            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// Send the data asynchronously.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="endOfMessage">Specifies whether this is the final asynchronous send. Set to true if this is the final send; false otherwise.</param>
        public void Send(byte[] data, bool endOfMessage = true)
        {
            Send(data, CancellationToken.None, endOfMessage);
        }

        /// <summary>
        /// Send the data asynchronously.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <param name="endOfMessage">Specifies whether this is the final asynchronous send. Set to true if this is the final send; false otherwise.</param>
        public async void Send(byte[] data, CancellationToken cancellationToken, bool endOfMessage = true)
        {
            Exception exception = null;
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        // Wait for signal.
                        lock (_syncSent)
                        {
                            // Send the data
                            _client.Send(data, cancellationToken, endOfMessage);
                            _sentAsync.WaitOne();
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }

                });
            await result;

            // If error the throw to the client.
            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// Send the data asynchronously.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="endOfMessage">Specifies whether this is the final asynchronous send. Set to true if this is the final send; false otherwise.</param>
        public void SendText(byte[] data, bool endOfMessage = true)
        {
            SendText(data, CancellationToken.None, endOfMessage);
        }

        /// <summary>
        /// Send the data asynchronously.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <param name="endOfMessage">Specifies whether this is the final asynchronous send. Set to true if this is the final send; false otherwise.</param>
        public async void SendText(byte[] data, CancellationToken cancellationToken, bool endOfMessage = true)
        {
            Exception exception = null;
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() =>
                {
                    try
                    {
                        // Wait for signal.
                        lock (_syncSent)
                        {
                            // Send the data
                            _client.SendText(data, cancellationToken, endOfMessage);
                            _sentAsync.WaitOne();
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }

                });
            await result;

            // If error the throw to the client.
            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// Aborts the connection and cancels any pending IO operations.
        /// </summary>
        public void Abort()
        {
            _client.Abort();
        }

        /// <summary>
        /// Client connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _client_OnConnected(object sender, EventArgs e)
        {
            OnConnected?.Invoke(sender, e);
        }

        /// <summary>
        /// Client disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _client_OnDisconnected(object sender, EventArgs e)
        {
            OnDisconnected?.Invoke(sender, e);
        }

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
                    if (_client != null)
                        _client.Dispose();

                    if (_sentAsync != null)
                        _sentAsync.Dispose();

                    if (_receiveAsync != null)
                        _receiveAsync.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _client = null;
                _sentAsync = null;
                _receiveAsync = null;
                _syncSent = null;
                _syncReceive = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~WebSocketClient()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
