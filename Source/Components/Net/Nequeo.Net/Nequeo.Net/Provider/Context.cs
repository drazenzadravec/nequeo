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
    /// Server socket context.
    /// </summary>
    public class Context : IDisposable
    {
        /// <summary>
        /// Server socket context.
        /// </summary>
        public Context() { }

        private Action _closeActionHandler = null;
        private Action _receivedAsyncMode = null;
        private Action<Nequeo.Net.Provider.Context> _onReceivedActionHandler = null;
        private Action<Nequeo.Net.Provider.Context> _onReceivedServerActionHandler = null;
        private Action<string> _uniqueIdentifierHasBeenSet = null;
        private Func<string, bool> _startTls = null;

        private string _uniqueIdentifier = "";

        private Nequeo.IO.Stream.StreamBufferBase _requestBuffer = null;
        private Nequeo.IO.Stream.StreamBufferBase _responseBuffer = null;

        /// <summary>
        /// Gets the request temp buffer store.
        /// </summary>
        public Nequeo.IO.Stream.StreamBufferBase RequestBufferStore
        {
            get
            {
                // If no instance then create the buffer.
                if (_requestBuffer == null)
                    _requestBuffer = new IO.Stream.StreamBufferBase();

                return _requestBuffer;
            }
            internal set { _requestBuffer = value; }
        }

        /// <summary>
        /// Gets the response temp buffer store.
        /// </summary>
        public Nequeo.IO.Stream.StreamBufferBase ResponseBufferStore
        {
            get
            {
                // If no instance then create the buffer.
                if (_responseBuffer == null)
                    _responseBuffer = new IO.Stream.StreamBufferBase();

                return _responseBuffer;
            }
            internal set { _responseBuffer = value; }
        }

        /// <summary>
        /// Gets or sets the context is only generated once used along with IsAsyncMode when set to true.
        /// All client processing is done in async and within the current context. The is triggered only
        /// when data has arrived.
        /// </summary>
        public Action ReceivedAsyncMode
        {
            get { return _receivedAsyncMode; }
            set { _receivedAsyncMode = value; }
        }

        /// <summary>
        /// Gets sets the close action handler.
        /// </summary>
        internal Action CloseActionHandler
        {
            get { return _closeActionHandler; }
            set { _closeActionHandler = value; }
        }

        /// <summary>
        /// Gets sets the unique identifier has been set action handler.
        /// </summary>
        internal Action<string> UniqueIdentifierHasBeenSet
        {
            get { return _uniqueIdentifierHasBeenSet; }
            set { _uniqueIdentifierHasBeenSet = value; }
        }

        /// <summary>
        /// On received action handler, new data has arrived.
        /// </summary>
        internal Action<Nequeo.Net.Provider.Context> OnReceivedHandler
        {
            get { return _onReceivedActionHandler; }
            set { _onReceivedActionHandler = value; }
        }

        /// <summary>
        /// On received server action handler, new data has arrived.
        /// </summary>
        internal Action<Nequeo.Net.Provider.Context> OnReceivedServerHandler
        {
            get { return _onReceivedServerActionHandler; }
            set { _onReceivedServerActionHandler = value; }
        }

        /// <summary>
        /// On start TLS negotiation.
        /// </summary>
        internal Func<string, bool> StartTls
        {
            get { return _startTls; }
            set { _startTls = value; }
        }

        /// <summary>
        /// Response buffer.
        /// </summary>
        internal ResponseStream ResponseStream
        {
            get;
            set;
        }

        /// <summary>
        /// Request buffer.
        /// </summary>
        internal RequestStream RequestStream
        {
            get;
            set;
        }

        /// <summary>
        /// Begin the secure negotiation and server authentication.
        /// </summary>
        /// <param name="tlsNegotiationCommand">Send a TLS negotiation command (e.g. OK Begin TLS negotiation now) if not null.</param>
        /// <returns>True if authentication has started; else false.</returns>
        /// <remarks>This is generally used for TLS protcol.</remarks>
        public bool BeginTlsNegotiation(string tlsNegotiationCommand = null)
        {
            if (_startTls != null)
            {
                return _startTls(tlsNegotiationCommand);
            }
            else return false;
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
        /// Gets the end point of the client.
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the end point of the server.
        /// </summary>
        public IPEndPoint ServerEndPoint
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
        /// Gets, the current server name.
        /// </summary>
        public string Name
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
            internal set;
        }

        /// <summary>
        /// Gets, the number of clients currently connected to the server.
        /// </summary>
        public int NumberOfClients
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
        /// Gets the current session identifier.
        /// </summary>
        public string SessionID
        {
            get;
            internal set;
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
        /// Gets or sets the unique identifier for this connection.
        /// </summary>
        public string UniqueIdentifier
        {
            get { return _uniqueIdentifier; }
            set 
            { 
                _uniqueIdentifier = value;

                // The unique identifier has been set.
                if (_uniqueIdentifierHasBeenSet != null)
                    _uniqueIdentifierHasBeenSet(_uniqueIdentifier);
            }
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

        /// <summary>
        /// Gets or sets a usable state container.
        /// </summary>
        public object State
        {
            get;
            set;
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
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
                    if (_requestBuffer != null)
                        _requestBuffer.Dispose();

                    if (_responseBuffer != null)
                        _responseBuffer.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _requestBuffer = null;
                _responseBuffer = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Context()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
