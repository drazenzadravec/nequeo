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
using System.Net;
using System.Net.WebSockets;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

namespace Nequeo.Net.Http
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
            _webSocket = new ClientWebSocket();
        }

        /// <summary>
        /// Web socket client.
        /// </summary>
        /// <param name="host">The remote host.</param>
        /// <param name="useSslConnection">Use a secure connection.</param>
        /// <param name="path">The remote host path.</param>
        public WebSocketClient(string host, bool useSslConnection = false, string path = "/")
        {
            _host = host;
            _useSslConnection = useSslConnection;
            _path = path;
            _webSocket = new ClientWebSocket();

            // Use a secure port.
            if (_useSslConnection)
                _port = 443;
        }

        private ClientWebSocket _webSocket = null;
        private bool _isConnected = false;

        private CancellationToken? _receiveCancellationToken = null;
        private CancellationToken? _closeCancellationToken = null;

        private Action<byte[], System.Net.WebSockets.WebSocketReceiveResult> _onReceive = null;

        private int _sendSize = 8192;
        private int _receiveSize = 8192;

        private string _host = "localhost";
        private int _port = 80;
        private string _path = "/";

        private const string _secureProtocol = "wss://";
        private const string _nonSecureProtocol = "ws://";
        private bool _useSslConnection = false;

        /// <summary>
        /// Gets or sets the on receive handler.
        /// </summary>
        public Action<byte[], System.Net.WebSockets.WebSocketReceiveResult> OnReceive
        {
            get { return _onReceive; }
            set { _onReceive = value; }
        }

        /// <summary>
        /// Gets or sets the receive cancellation token.
        /// </summary>
        public CancellationToken? ReceiveCancellationToken
        {
            get { return _receiveCancellationToken; }
            set { _receiveCancellationToken = value; }
        }

        /// <summary>
        /// Gets or sets the close cancellation token.
        /// </summary>
        public CancellationToken? CloseCancellationToken
        {
            get { return _closeCancellationToken; }
            set { _closeCancellationToken = value; }
        }

        /// <summary>
        /// Gets or sets is the client connected.
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected; }
        }

        /// <summary>
        /// Gets or sets the receive chunk size.
        /// </summary>
        public int ReceiveSize
        {
            get { return _receiveSize; }
            set { _receiveSize = value; }
        }

        /// <summary>
        /// Gets or sets the send chunk size.
        /// </summary>
        public int SendSize
        {
            get { return _sendSize; }
            set { _sendSize = value; }
        }

        /// <summary>
        /// Gets or sets use a secure connection.
        /// </summary>
        public bool UseSslConnection
        {
            get { return _useSslConnection; }
            set { _useSslConnection = value; }
        }

        /// <summary>
        /// Gets or sets remote host.
        /// </summary>
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        /// <summary>
        /// Gets or sets remote host path.
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// Gets or sets remote host port.
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
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
            // If not connected.
            if (!_isConnected)
            {
                string protocol = _useSslConnection ? _secureProtocol : _nonSecureProtocol;
                string url = protocol + _host.TrimEnd('/') + ":" + _port + "/" + _path.TrimStart('/');

                // Create the uri
                Uri uri = new Uri(url);

                // Attempt to make a connection.
                await _webSocket.ConnectAsync(uri, cancellationToken);

                // Indicate that a connection has been made.
                _isConnected = true;

                // Start the receiver.
                Receiver(_webSocket);
            }
        }

        /// <summary>
        /// Close connection asynchronously.
        /// </summary>
        public async void Close()
        {
            // If connected.
            if (_isConnected)
            {
                // Indicate that a connection no longer exists.
                _isConnected = false;
                CancellationToken closeCancellationToken = _closeCancellationToken != null ? _closeCancellationToken.Value : CancellationToken.None;

                // Close the connection.
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, closeCancellationToken);
            }
        }

        /// <summary>
        /// Aborts the connection and cancels any pending IO operations.
        /// </summary>
        public void Abort()
        {
            // If connected.
            if (_isConnected)
            {
                // Indicate that a connection no longer exists.
                _isConnected = false;

                // Aborts the connection and cancels any pending IO operations.
                _webSocket.Abort();
            }
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
            // If connected.
            if (_isConnected)
            {
                // While the connection is open.
                while (_webSocket.State == WebSocketState.Open)
                {
                    // Send the data.
                    await _webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, endOfMessage, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Start the web socket receiver.
        /// </summary>
        /// <param name="webSocket">The current web socket client.</param>
        private async void Receiver(ClientWebSocket webSocket)
        {
            // Setup the receive buffer.
            byte[] buffer = new byte[_receiveSize];

            CancellationToken receiveCancellationToken = _receiveCancellationToken != null ? _receiveCancellationToken.Value : CancellationToken.None;
            CancellationToken closeCancellationToken = _closeCancellationToken != null ? _closeCancellationToken.Value : CancellationToken.None;

            // While the connection is open.
            while (webSocket.State == WebSocketState.Open)
            {
                // Wait for the next set of data.
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), receiveCancellationToken);

                // If the connection has closed.
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    // Close the connection.
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, closeCancellationToken);
                }
                else
                {
                    // Send the result to the client.
                    if (_onReceive != null)
                        _onReceive(buffer, result);
                }
            }
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
                    if (_webSocket != null)
                        _webSocket.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _webSocket = null;
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
