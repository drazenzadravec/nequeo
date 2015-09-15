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
using System.Collections.Specialized;

using Nequeo.Model;
using Nequeo.Extension;

namespace Nequeo.Net.WebSockets
{
    /// <summary>
    /// Web socket single server.
    /// </summary>
    public class WebSocketSingleServer : Nequeo.Net.Provider.ServerSingleSocket
    {
        #region Constructors
        /// <summary>
        /// Web socket single server.
        /// </summary>
        public WebSocketSingleServer()
        {
            AssignOnConnectedActionHandler();
        }

        /// <summary>
        /// Web socket single server.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public WebSocketSingleServer(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = 10000)
            : base(multiEndpointModels, maxNumClients)
        {
            AssignOnConnectedActionHandler();
        }
        #endregion

        #region Private Fields
        private int _maxHeaderBufferStore = 20000;
        #endregion

        #region Public Events
        /// <summary>
        /// The on web socket context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event Nequeo.Threading.EventHandler<CentralWebSocketContext> OnWebSocketContext;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the maximum header request buffer store read size before closing the connection. 
        /// </summary>
        public int MaxHeaderBufferStore
        {
            get { return _maxHeaderBufferStore; }
            set { _maxHeaderBufferStore = value; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Assign the on connected action handler.
        /// </summary>
        private void AssignOnConnectedActionHandler()
        {
            // Assign the on connect action handler.
            base.Timeout = 30;
            base.HeaderTimeout = 30000;
            base.RequestTimeout = 30000;
            base.ResponseTimeout = 30000;
            base.Name = "Nequeo Web Socket Server";
            base.ServiceName = "WebSocketSingleServer";
            base.SocketProviderHostPrefix = "WebSocketSingleServer_";
            base.OnReceivedHandler = (Nequeo.Net.Provider.Context context) => OnReceivedActionHandler(context);
        }

        /// <summary>
        /// On received action handler.
        /// </summary>
        /// <param name="context">The current client context.</param>
        private void OnReceivedActionHandler(Nequeo.Net.Provider.Context context)
        {
            try
            {
                // Store data until all headers have been read.
                if (Nequeo.Net.Utility.IsParse2CRLF(context.Request.Input, context.RequestBufferStore, _maxHeaderBufferStore))
                {
                    // Create the web socket context and set the headers.
                    Nequeo.Net.WebSockets.WebSocketContext webSocketContext = CreateWebSocketContext(context);

                    // Get the headers from the stream and assign the request data.
                    bool headersExist = Nequeo.Net.WebSockets.Utility.SetRequestHeaders(webSocketContext,
                        base.HeaderTimeout, base.MaximumReadLength, context.RequestBufferStore);

                    // Create the response headers.
                    CreateResponseHeaders(webSocketContext);

                    // Make sure all handshaking has complete before continuing.
                    if (!webSocketContext.HandshakeComplete)
                    {
                        // Not valid.
                        throw new Exception("Not a valid WebSocket request.");
                    }
                    else
                    {
                        // Indicate that all web sockets after handshaking
                        // is complete will run in async mode.
                        webSocketContext.IsAsyncMode = true;

                        // Create the central web socket context.
                        CentralWebSocketContext centralWebSocketContext = new CentralWebSocketContext(webSocketContext);
                        centralWebSocketContext.ConnectionID = webSocketContext.ConnectionID;
                        centralWebSocketContext.Cookies = webSocketContext.WebSocketRequest.Cookies;
                        centralWebSocketContext.Headers = webSocketContext.WebSocketRequest.Headers;
                        centralWebSocketContext.IsAuthenticated = webSocketContext.IsAuthenticated;
                        centralWebSocketContext.IsSecureConnection = webSocketContext.IsSecureConnection;
                        centralWebSocketContext.SecWebSocketKey = webSocketContext.WebSocketRequest.SecWebSocketKey;
                        centralWebSocketContext.SecWebSocketProtocols = webSocketContext.WebSocketRequest.SecWebSocketProtocols;
                        centralWebSocketContext.SecWebSocketVersion = webSocketContext.WebSocketRequest.SecWebSocketVersion;
                        centralWebSocketContext.SessionID = webSocketContext.SessionID;
                        centralWebSocketContext.SocketState = webSocketContext.SocketState;
                        centralWebSocketContext.UniqueIdentifier = webSocketContext.UniqueIdentifier;
                        centralWebSocketContext.Url = webSocketContext.WebSocketRequest.Url;
                        centralWebSocketContext.User = webSocketContext.User;

                        // If the event has been assigned.
                        if (OnWebSocketContext != null)
                            OnWebSocketContext(this, centralWebSocketContext);

                        // Write the response to the client.
                        Nequeo.Net.WebSockets.WebSocketResponse response = webSocketContext.WebSocketResponse;
                        response.WriteWebSocketHeaders();

                        // Create and assign the web socket.
                        Nequeo.Net.WebSockets.WebSocket webSocket = new Nequeo.Net.WebSockets.WebSocket();
                        centralWebSocketContext.WebSocket = webSocket;

                        // Save the web context state objects.
                        SaveWebContext(context, webSocketContext);
                    }
                }

                // If the maximum request buffer store has been reached then close the connection.
                if (context.RequestBufferStore.Length > _maxHeaderBufferStore)
                    throw new Exception("Maximum request buffer store has been reached.");
            }
            catch (Exception)
            {
                // Close the connection and release all resources used for communication.
                context.Close();
            }
        }

        /// <summary>
        /// Create the web socket context.
        /// </summary>
        /// <param name="context">The current client context.</param>
        /// <returns>The web socket context.</returns>
        private Nequeo.Net.WebSockets.WebSocketContext CreateWebSocketContext(Nequeo.Net.Provider.Context context)
        {
            // Get the underlying web context.
            Nequeo.Net.WebContext webContext = CreateWebContext(context);

            // Create the new web socket context from the web context.
            Nequeo.Net.WebSockets.WebSocketContext webSocketContext = Nequeo.Net.WebSockets.WebSocketContext.CreateFrom(webContext);

            // Assign response and request data.
            webSocketContext.WebSocketResponse = new WebSocketResponse();
            webSocketContext.WebSocketRequest = new WebSocketRequest();
            webSocketContext.WebSocketResponse.Output = webContext.WebResponse.Output;
            webSocketContext.WebSocketRequest.Input = webContext.WebRequest.Input;

            // Return the request context.
            return webSocketContext;
        }

        /// <summary>
        /// Create the web socket response headers.
        /// </summary>
        /// <param name="webSocketContext">The current web socket context.</param>
        private void CreateResponseHeaders(Nequeo.Net.WebSockets.WebSocketContext webSocketContext)
        {
            // If this is a WebSocket request then complete the handshaking.
            if (webSocketContext.WebSocketRequest.IsWebSocketRequest)
            {
                // Get the sec web socket accept key.
                byte[] webSocketKey = Nequeo.Cryptography.Hashcode.GetHashcodeSHA1Raw(webSocketContext.WebSocketRequest.SecWebSocketKey + Common.Resource.MagicGuid);
                string secWebSocketAccept = System.Convert.ToBase64String(webSocketKey);

                // Create the response and erite the data back to the client.
                Nequeo.Net.WebSockets.WebSocketResponse response = webSocketContext.WebSocketResponse;
                response.SecWebSocketAccept = secWebSocketAccept;
                response.Server = base.Name;

                // If protocols have been sent from the client
                // then assign the first one found.
                if (webSocketContext.WebSocketRequest.SecWebSocketProtocols != null && webSocketContext.WebSocketRequest.SecWebSocketProtocols.Length > 0)
                    response.SecWebSocketProtocol = webSocketContext.WebSocketRequest.SecWebSocketProtocols[0];

                // If extensions have been sent from the client
                // then assign the first one found.
                if (webSocketContext.WebSocketRequest.SecWebSocketExtensions != null && webSocketContext.WebSocketRequest.SecWebSocketExtensions.Length > 0)
                    response.SecWebSocketExtensions = webSocketContext.WebSocketRequest.SecWebSocketExtensions[0];

                // Indicate that handshaking is complete.
                webSocketContext.HandshakeComplete = true;
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Get a new web socket response stream.
        /// </summary>
        /// <param name="output">The http response output stream.</param>
        /// <returns>The web socket response stream.</returns>
        protected virtual Nequeo.Net.WebSockets.WebSocketResponse GetWebSocketResponse(System.IO.Stream output)
        {
            Nequeo.Net.WebSockets.WebSocketResponse response = new Nequeo.Net.WebSockets.WebSocketResponse();
            response.Output = output;
            return response;
        }
        #endregion
    }
}
