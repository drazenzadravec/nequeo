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
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Collections.Specialized;

using Nequeo.Model;
using Nequeo.Extension;

namespace Nequeo.Net.WebSockets
{
    /// <summary>
    /// Web socket server context.
    /// </summary>
    public class CentralWebSocketContext
    {
        /// <summary>
        /// Web socket server context.
        /// </summary>
        /// <param name="context">The base web socket context.</param>
        public CentralWebSocketContext(WebSocketContext context)
        {
            _context = context;
        }

        private string _connectionID = null;
        private string _sessionID = null;
        private bool _isSecureConnection = false;
        private bool _isSslAuthenticated = false;
        private string _uniqueIdentifier = "";
        private bool _isAuthenticated = false;
        private IPrincipal _user = null;
        private SocketState _socketState = SocketState.None;
        private string _secWebSocketKey = null;
        private string[] _secWebSocketProtocols = null;
        private string _secWebSocketVersion = null;
        private Uri _url = null;
        private NameValueCollection _headers = null;
        private CookieCollection _cookies = null;
        private Nequeo.Net.WebSockets.WebSocket _webSocket = null;
        private WebSocketContext _context = null;

        /// <summary>
        /// Gets the web socket context.
        /// </summary>
        public WebSocketContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets or sets the current session identifier.
        /// </summary>
        public string SessionID
        {
            get
            {
                if (_context != null)
                    _sessionID = _context.SessionID;

                return _sessionID;
            }
            set { _sessionID = value; }
        }

        /// <summary>
        /// Gets or sets the current unique connection identifier.
        /// </summary>
        public string ConnectionID
        {
            get
            {
                if (_context != null)
                    _connectionID = _context.ConnectionID;

                return _connectionID;
            }
            set { _connectionID = value; }
        }

        /// <summary>
        /// Gets or sets a System.Boolean value that indicates whether the TCP connection used
        /// to send the request is using the Secure Sockets Layer (SSL) protocol.
        /// </summary>
        public bool IsSecureConnection
        {
            get
            {
                if (_context != null)
                    _isSecureConnection = _context.IsSecureConnection;

                return _isSecureConnection;
            }
            set { _isSecureConnection = value; }
        }

        /// <summary>
        /// Gets or sets the current web socket state.
        /// </summary>
        public SocketState SocketState
        {
            get
            {
                if (_context != null)
                    _socketState = _context.SocketState;

                return _socketState;
            }
            set { _socketState = value; }
        }

        /// <summary>
        /// Gets or sets the unique identifier for this connection.
        /// </summary>
        public string UniqueIdentifier
        {
            get { return _uniqueIdentifier; }
            set
            {
                _uniqueIdentifier = value;
                if (_context != null)
                    _context.UniqueIdentifier = _uniqueIdentifier;
            }
        }

        /// <summary>
        /// Gets or sets, true if the context has been authenticated; else false.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                if (_context != null)
                    _isAuthenticated = _context.IsAuthenticated;

                return _isAuthenticated;
            }
            set
            {
                _isAuthenticated = value;
                if (_context != null)
                    _context.IsAuthenticated = _isAuthenticated;
            }
        }

        /// <summary>
        /// Gets or sets, has a secure negotiation and server authentication 
        /// been established with the client. Also is the data encrypted.
        /// </summary>
        public bool IsSslAuthenticated
        {
            get
            {
                if (_context != null)
                    _isSslAuthenticated = _context.IsSslAuthenticated;

                return _isSslAuthenticated;
            }
            set { _isSslAuthenticated = value; }
        }

        /// <summary>
        /// Gets or sets security information for the current request.
        /// </summary>
        public IPrincipal User
        {
            get
            {
                if (_context != null)
                    _user = _context.User;

                return _user;
            }
            set { _user = value; }
        }

        /// <summary>
        /// Gets or sets the WebSocket security key.
        /// </summary>
        public string SecWebSocketKey
        {
            get { return _secWebSocketKey; }
            set { _secWebSocketKey = value; }
        }

        /// <summary>
        /// Gets or sets the WebSocket protocols request.
        /// </summary>
        public string[] SecWebSocketProtocols
        {
            get { return _secWebSocketProtocols; }
            set { _secWebSocketProtocols = value; }
        }

        /// <summary>
        /// Gets or sets the WebSocket standard version.
        /// </summary>
        public string SecWebSocketVersion
        {
            get { return _secWebSocketVersion; }
            set { _secWebSocketVersion = value; }
        }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public Uri Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        public NameValueCollection Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        /// <summary>
        /// Gets or sets the cookies sent with the request.
        /// </summary>
        public CookieCollection Cookies
        {
            get { return _cookies; }
            set { _cookies = value; }
        }

        /// <summary>
        /// Gets The WebSocket instance used to interact (send/receive/close/etc) with the WebSocket connection.
        /// </summary>
        public Nequeo.Net.WebSockets.WebSocket WebSocket
        {
            get { return _webSocket; }
            internal set { _webSocket = value; }
        }

        /// <summary>
        /// Close and release all resources.
        /// </summary>
        public void Close()
        {
            try
            {
                // Close the web socket.
                if (_webSocket != null)
                    _webSocket.Dispose();
            }
            catch { }
            
            // Close the connection.
            if (_context != null)
                _context.Close();
        }
    }
}
