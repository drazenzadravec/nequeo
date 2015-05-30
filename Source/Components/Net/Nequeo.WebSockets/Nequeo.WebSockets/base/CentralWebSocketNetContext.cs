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
    /// Web socket client context.
    /// </summary>
    public class CentralWebSocketNetContext
    {
        /// <summary>
        /// Web socket client context.
        /// </summary>
        /// <param name="context">The base web socket context.</param>
        public CentralWebSocketNetContext(WebSocketNetContext context)
        {
            _context = context;
        }

        private Nequeo.Net.WebSockets.WebSocket _webSocket = null;
        private WebSocketNetContext _context = null;

        /// <summary>
        /// Gets the web socket context.
        /// </summary>
        public WebSocketNetContext Context
        {
            get { return _context; }
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
