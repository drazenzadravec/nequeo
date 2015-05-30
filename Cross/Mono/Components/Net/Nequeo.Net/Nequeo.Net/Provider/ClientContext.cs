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

namespace Nequeo.Net.Provider
{
    /// <summary>
    /// Client socket context.
    /// </summary>
    public class ClientContext
    {
        private Action _closeActionHandler = null;
        private Action<Nequeo.Net.Provider.ClientContext> _onReceivedActionHandler = null;
        private Action<Nequeo.Net.Provider.ClientContext> _onReceivedServerActionHandler = null;

        /// <summary>
        /// Gets sets the close action handler.
        /// </summary>
        internal Action CloseActionHandler
        {
            get { return _closeActionHandler; }
            set { _closeActionHandler = value; }
        }

        /// <summary>
        /// On received action handler, new data has arrived.
        /// </summary>
        internal Action<Nequeo.Net.Provider.ClientContext> OnReceivedHandler
        {
            get { return _onReceivedActionHandler; }
            set { _onReceivedActionHandler = value; }
        }

        /// <summary>
        /// On received server action handler, new data has arrived.
        /// </summary>
        internal Action<Nequeo.Net.Provider.ClientContext> OnReceivedServerHandler
        {
            get { return _onReceivedServerActionHandler; }
            set { _onReceivedServerActionHandler = value; }
        }

        /// <summary>
        /// Response buffer.
        /// </summary>
        internal NetResponseStream ResponseStream
        {
            get;
            set;
        }

        /// <summary>
        /// Request buffer.
        /// </summary>
        internal NetRequestStream RequestStream
        {
            get;
            set;
        }

        /// <summary>
        /// Close the connection and release all resources used for communication.
        /// </summary>
        public void Close()
        {
            // Call the close action handler.
            if (_closeActionHandler != null)
                _closeActionHandler();
        }

        /// <summary>
        /// Gets the request context.
        /// </summary>
        public Request Request
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the response context.
        /// </summary>
        public Response Response
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a System.Boolean value that indicates whether the TCP connection used
        /// to send the request is using the Secure Sockets Layer (SSL) protocol.
        /// </summary>
        public bool IsSecureConnection
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the communication port number.
        /// </summary>
        public int Port
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets, the current web socket state.
        /// </summary>
        public SocketState SocketState
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the current unique connection identifier.
        /// </summary>
        public string ConnectionID
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets, the common service name.
        /// </summary>
        public string ServiceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets, the unique identifier for this connection.
        /// </summary>
        public string UniqueIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets, true if the context has been authenticated; else false.
        /// </summary>
        public bool IsAuthenticated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets, true if in async mode, then the context is only generated once.
        /// All client processing is done in async and within the current context.
        /// </summary>
        public bool IsAsyncMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current active client context. 
        /// This is used to indicate an active connection. 
        /// If this has been set then multiple requests can arrive.
        /// Add an active context object to indicate an open connection.
        /// </summary>
        internal object ContextState
        {
            get;
            set;
        }
    }
}
