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
    /// Web socket server context.
    /// </summary>
    public sealed class WebSocketContext : Nequeo.Net.WebContext
    {
        private bool _handshakeComplete = false;

        /// <summary>
        /// Create the web socket context from the web context.
        /// </summary>
        /// <param name="webContext">The web context to create from.</param>
        /// <returns>The web socket server context.</returns>
        public static WebSocketContext CreateFrom(Nequeo.Net.WebContext webContext)
        {
            Nequeo.Net.WebSockets.WebSocketContext webSocketContext = new Nequeo.Net.WebSockets.WebSocketContext();
            webSocketContext.Context = webContext.Context;
            webSocketContext.IsStartOfConnection = webContext.IsStartOfConnection;
            webSocketContext.IsAuthenticated = webContext.IsAuthenticated;
            webSocketContext.IsSecureConnection = webContext.IsSecureConnection;
            webSocketContext.Name = webContext.Name;
            webSocketContext.NumberOfClients = webContext.NumberOfClients;
            webSocketContext.Port = webContext.Port;
            webSocketContext.RemoteEndPoint = webContext.RemoteEndPoint;
            webSocketContext.ServerEndPoint = webContext.ServerEndPoint;
            webSocketContext.ServiceName = webContext.ServiceName;
            webSocketContext.UniqueIdentifier = webContext.UniqueIdentifier;
            webSocketContext.ConnectionID = webContext.ConnectionID;
            webSocketContext.SessionID = webContext.SessionID;
            webSocketContext.User = webContext.User;
            webSocketContext.SocketState = webContext.SocketState;
            webSocketContext.IsAsyncMode = webContext.IsAsyncMode;
            webSocketContext.HandshakeComplete = false;
            return webSocketContext;
        }

        /// <summary>
        /// Gets the web socket request.
        /// </summary>
        public Nequeo.Net.WebSockets.WebSocketRequest WebSocketRequest
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the web socket response.
        /// </summary>
        public Nequeo.Net.WebSockets.WebSocketResponse WebSocketResponse
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an indicator specifying if the handshake is complete.
        /// </summary>
        public bool HandshakeComplete
        {
            get { return _handshakeComplete; }
            set { _handshakeComplete = value; }
        }
    }
}
